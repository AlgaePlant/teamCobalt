const int b1Pin = 7;  //variable for button 1 pin
const int b2Pin = 8;  //variable for button 2 pin
const int b3Pin = 10;  //variable for button 3 pin
// const int b4Pin = 12;  //variable for button 4 pin

void setup() {
  Serial.begin(9600);

  pinMode(b1Pin, INPUT_PULLUP);
  pinMode(b2Pin, INPUT_PULLUP);
  pinMode(b3Pin, INPUT_PULLUP);
  // pinMode(b4Pin, INPUT_PULLUP);
}

void loop(){
  //read from button pin and store value 
  int b1Val = digitalRead(b1Pin);   
  int b2Val = digitalRead(b2Pin);
  int b3Val = digitalRead(b3Pin);
  // int b4Val = digitalRead(b4Pin);
  
  //check if button is pressed. If it is, the state is LOW
  int b1Pressed = (b1Val == LOW) ? 1:0;
  int b2Pressed = (b2Val == LOW) ? 1:0;
  int b3Pressed = (b3Val == LOW) ? 1:0;
  // int b4Pressed = (b4Val == LOW) ? 1:0;

  Serial.println(String(b1Pressed) + "," + String(b2Pressed) + "," + String(b3Pressed));

  delay(20);
}