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
    else {
      delay(250);
    }
  }
}

void loop() {
  if (Serial.available() > 0) {    
    // 2 byte packet, byte 1 = DMX channel 1-256, byte 2 = DMX value 0-255
    char buffer [2];
        
    // Read in a 2 byte packet/request
    Serial.readBytes(buffer, 2);
    
    // The decoded DMX channel and value
    unsigned channel = ((uint8_t)buffer[0]) + 1;
    uint8_t value = (uint8_t)buffer[1];
        
    // Pass on the DMX request to the connected DMX port
    DmxMaster.write(channel, value);
  }
}
