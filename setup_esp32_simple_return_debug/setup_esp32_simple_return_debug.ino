#include <BluetoothSerial.h>    //For connecting ESP32 to VR headset and Unity

BluetoothSerial BTSerial;

const int analogPin = A0;

// pin 38 correlates to the ESP32's SW38 built-in button
const int BUTTON_PIN = 38;
// built-in LED on Adafruit Feather ESP32 V2
const int LED_PIN = 13;

//The numbers for Guitar controller string pins
const int S1Pin = 27;  //string 1 pin
const int S2Pin = 15;  //string 2 pin
const int S3Pin = 14;  //string 3 pin

//The numbers for LED pins. LED lights are for showing the circuit is running properly
const int led1Pin = 12;
const int led2Pin = 33;
const int led3Pin = 32;

//Current state of LED pin
int led1State = LOW;
int led2State = LOW;
int led3State = LOW;

//Variables for string state
int S1State = HIGH;
int S2State = HIGH;
int S3State = HIGH;

//previous reading from string pin
int lastS1Reading = HIGH;
int lastS2Reading = HIGH;
int lastS3Reading = HIGH;

unsigned long S1DebounceTime = 0;
unsigned long S2DebounceTime = 0;
unsigned long S3DebounceTime = 0;

const unsigned long stringDebounceDelay = 5;

void setup() {
  // Start USB Serial Debugging
  Serial.begin(9600);
  Serial.println("Starting Bluetooth Device...");

  // Start Bluetooth
  if (!BTSerial.begin("teamCobalt")) {
    Serial.println("Bluetooth failed to start!");
  } else {
    Serial.println("Bluetooth started successfully.");
    Serial.println("Device name: teamCobalt");
  }

  pinMode(LED_PIN, OUTPUT);

  // Use internal pullup (button reads LOW when pressed)

  pinMode(BUTTON_PIN, INPUT_PULLUP);

  Serial.println("Setup complete.");

  pinMode(analogPin, INPUT);

  //Initialize Guitar controller's pins as INPUT pins
  pinMode(S1Pin, INPUT);
  pinMode(S2Pin, INPUT);
  pinMode(S3Pin, INPUT);

  //Initialize LED pins as OUTPUT pins
  pinMode(led1Pin, OUTPUT);
  pinMode(led2Pin, OUTPUT);
  pinMode(led3Pin, OUTPUT);

  // set initial LED state
  digitalWrite(led1Pin, led1State);
  digitalWrite(led2Pin, led2State);
  digitalWrite(led3Pin, led3State);
}


void loop() {

  // ===============================
  // 1. CHECK FOR BLUETOOTH DATA
  // ===============================

  if (BTSerial.available()) {

    Serial.println("Bluetooth data available...");

    String data = BTSerial.readStringUntil('\n');   //read commands from Unity
    data.trim();

    Serial.print("Received: ");   //print the received command on Serial
    Serial.println(data);

    //Turning on or off the LED light based on Unity's command
    if (data == "grab") {             //If Unity sent the "grab" command
      digitalWrite(13, HIGH);         //Turn on the light
    } else if (data == "release") {   //Else if Unity sent the "release" command
      digitalWrite(13, LOW);          //Turn off the LED light
    }
  }

  // ===============================
  // 2. CHECK BUTTON STATE
  // ===============================

  static bool lastButtonState = HIGH;
  bool buttonState = digitalRead(BUTTON_PIN);

  // Only send if state changed (prevents spam)
  if (buttonState != lastButtonState) {
    if (buttonState == LOW) {
      Serial.println("Button PRESSED");
      BTSerial.println("off");    //send the message "off" to Unity via Bluetooth
    } else {
      Serial.println("Button RELEASED");
      BTSerial.println("on");     //send the message "on" to Unity via Bluetooth
    }
    lastButtonState = buttonState;
  }

  delay(50);  // small debounce delay

  // ===============================
  // 3. GUITAR CONTROLLER
  // The guitar string input uses a pull-up resistor:
  // Not touched = HIGH (1)
  // Touched by grounded pick = LOW (0)
  //
  // When the string is touched, turn on the corresponding LED.
  // ===============================

  //Read the state of each guitar string input pin
  int S1Reading = digitalRead(S1Pin);
  int S2Reading = digitalRead(S2Pin);
  int S3Reading = digitalRead(S3Pin);

  //Check the guitar string pin's state. If the string circuit is connected by the pick, the input reads LOW (0),
  // ---------- String 1 ----------
  if (S1Reading != lastS1Reading) {
    S1DebounceTime = millis();
  }

  if ((millis() - S1DebounceTime) > stringDebounceDelay) {
    if (S1Reading != S1State) {
      S1State = S1Reading;
    }
  }

  lastS1Reading = S1Reading;


  // ---------- String 2 ----------
  if (S2Reading != lastS2Reading) {
    S2DebounceTime = millis();
  }

  if ((millis() - S2DebounceTime) > stringDebounceDelay) {
    if (S2Reading != S2State) {
      S2State = S2Reading;
    }
  }

  lastS2Reading = S2Reading;


  // ---------- String 3 ----------
  if (S3Reading != lastS3Reading) {
    S3DebounceTime = millis();
  }

  if ((millis() - S3DebounceTime) > stringDebounceDelay) {
    if (S3Reading != S3State) {
      S3State = S3Reading;
    }
  }

  lastS3Reading = S3Reading;

  // Control LEDs
  if (S1State == 0) {
      digitalWrite(led1Pin, HIGH);
  } else {
      digitalWrite(led1Pin, LOW);
  }

  if (S2State == 0) {
      digitalWrite(led2Pin, HIGH);
  } else {
      digitalWrite(led2Pin, LOW);
  }

  if (S3State == 0) {
      digitalWrite(led3Pin, HIGH);
  } else {
      digitalWrite(led3Pin, LOW);
  }

  Serial.println(String(S1State) + "," + String(S2State) + "," + String(S3State));    //print guitar string pin's state on Serial
  BTSerial.println(String(S1State) + "," + String(S2State) + "," + String(S3State));    //send the guitar string pin's state to Bluetooth, so Unity can receive it
}