#include <BluetoothSerial.h>

BluetoothSerial BTSerial;

const int analogPin = A0;

// pin 38 correlates to the ESP32's SW38 built-in button
const int BUTTON_PIN = 38;
// built-in LED on Adafruit Feather ESP32 V2
const int LED_PIN = 13;

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
}


void loop() {

  // ===============================
  // 1. CHECK FOR BLUETOOTH DATA
  // ===============================

  if (BTSerial.available()) {

    Serial.println("Bluetooth data available...");

    String data = BTSerial.readStringUntil('\n');
    data.trim();

    Serial.print("Received: ");
    Serial.println(data);

    if (data == "grab") {
      digitalWrite(13, HIGH);
    } else if (data == "release") {
      digitalWrite(13, LOW);
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
      BTSerial.println("off");
    } else {
      Serial.println("Button RELEASED");
      BTSerial.println("on");
    }
    lastButtonState = buttonState;
  }

  delay(50);  // small debounce delay

  int potVal = analogRead(A0);
  Serial.println(potVal);
  delay(50);

  int sensorValue = analogRead(analogPin);
  BTSerial.println(sensorValue);
}