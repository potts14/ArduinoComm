void setup()
{
  pinMode(A0, INPUT);
  Serial.begin(115200);
}

void loop()
{
  Serial.println(map(analogRead(A0),0,1023,0,5000),DEC);
  delay(100);
}
