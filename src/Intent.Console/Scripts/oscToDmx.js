// Setup state for DMX lights
var dmxSetup = [];

// Populate Blizzard Flurry Tri init states
for (var i = 0; i < lights.flurries.length; i++) {
    var light = lights.flurries[i];
    dmxSetup.push({ channel: light.motorSpeedDmx(), value: light.motorSpeed });   // Set motors to max speed
    dmxSetup.push({ channel: light.dimmerDmx(), value: light.dimmer });       // Set master dimmer/strobe channel to full open color
    dmxSetup.push({ channel: light.redDmx(), value: light.r });            // Red
    dmxSetup.push({ channel: light.greenDmx(), value: light.g });            // Blue
    dmxSetup.push({ channel: light.blueDmx(), value: light.b });            // Green
    dmxSetup.push({ channel: light.panDmx(), value: light.pan });          // Pan
    dmxSetup.push({ channel: light.tiltDmx(), value: light.tilt });         // Tilt
};

// Populate Blizzard Puck Q12+ init states
for (var i = 0; i < lights.pucks.length; i++) {
    var light = lights.pucks[i];
    dmxSetup.push({ channel: light.redDmx(), value: light.r });            // Red
    dmxSetup.push({ channel: light.greenDmx(), value: light.g });            // Blue
    dmxSetup.push({ channel: light.blueDmx(), value: light.b });            // Green
    dmxSetup.push({ channel: light.amberDmx(), value: light.a });            // Amber
    dmxSetup.push({ channel: light.whiteDmx(), value: light.w });            // White
}

// Create the OSC to DMX adapter
var oscToDmx = { maxChannel: 129, setup: dmxSetup };
addAdapter(adapters.oscToDmx, oscToDmx);