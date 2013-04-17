#include <DmxMaster.h>

// The maximum DMX channel number
int maxChannel = 0;

void setup() {
  // start serial port
  Serial.begin(250000);
  while (!Serial) { ; } // wait for serial port to connect. Needed for Leonardo only  
  
  // Ask for the max DMX channel
  while (!maxChannel) {
    // Send request to host to get the max DMX channel and setup values
    Serial.println("setup");
    
    if (Serial.available() > 0) {
      // Read in the MSB + LSB of the max channel number
      char buffer[2];
      Serial.readBytes(buffer, 2); 
      maxChannel = (buffer[0] << 8) | (buffer[1] & 0xff);
      
      // Set the DMX max channel
      DmxMaster.maxChannel(maxChannel);
    }
    
    delay(250);
  }
}

void loop() {
  if (Serial.available() > 0) {
    // Channel and Value are read in as two separate 2 byte values made of msb + lsb
    char buffer [4];
    
    // Read in a 4 byte packet/request
    Serial.readBytes(buffer, 4);
    
    // The decoded DMX channel and value
    int channel = -1, value = -1;
    
    // Read the DMX channel
    channel = (buffer[0] << 8) | (buffer[1] & 0xff);
    
    // Read teh DMX value
    value = (buffer[2] << 8) | (buffer[3] & 0xff);
    
    // Pass on the DMX request to the connected DMX port
    DmxMaster.write(channel, value);
    //Serial.print("c:"); Serial.println(channel);
    //Serial.print("v:"); Serial.println(value);
  }
}
