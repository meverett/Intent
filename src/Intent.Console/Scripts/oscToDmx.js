// Setup state for DMX lights
var dmxSetup = [];

// Populate Blizzard Flurry Tri init states
for (var i = 0; i < lights.flurries.length; i++) {
    var light = lights.flurries[i];
    dmxSetup.push({ channel: light.dmx("motorSpeed"),   value: light.motorSpeed }); // Set motors to max speed
    dmxSetup.push({ channel: light.dmx("dimmer"),       value: light.dimmer });     // Set master dimmer/strobe channel to full open color
    dmxSetup.push({ channel: light.dmx("r"),            value: light.r });          // Red
    dmxSetup.push({ channel: light.dmx("g"),            value: light.g });          // Green
    dmxSetup.push({ channel: light.dmx("b"),            value: light.b });          // Blue
    dmxSetup.push({ channel: light.dmx("pan"),          value: light.pan });        // Pan
    dmxSetup.push({ channel: light.dmx("tilt"),         value: light.tilt });       // Tilt
};

// Populate Blizzard Puck Q12+ init states
for (var i = 0; i < lights.pucks.length; i++) {
    var light = lights.pucks[i];    
    dmxSetup.push({ channel: light.dmx("r"), value: light.r });                     // Red
    dmxSetup.push({ channel: light.dmx("g"), value: light.g });                     // Green
    dmxSetup.push({ channel: light.dmx("b"), value: light.b });                     // Blue
    dmxSetup.push({ channel: light.dmx("a"), value: light.a });                     // Amber
    dmxSetup.push({ channel: light.dmx("w"), value: light.w });                     // White
}

// Create the OSC to DMX adapter
var oscToDmx = { serial: "COM3", maxChannel: 129, setup: dmxSetup };
addAdapter(adapters.oscToDmx, oscToDmx);
//addAdapter(adapters.oscToConsole, oscToDmx);