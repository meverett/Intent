// Global adapter names/identities
var adapters = {
    midiToConsole:  "MIDI to Console",  midiToOsc:  "MIDI to OSC",
    oscToConsole:   "OSC to Console",   oscToDmx:   "OSC to DMX"
};

// Different color modes
var HSV = 0;
var RGB = 1;

// Create global light structure
var flurryInit = { pan: 100, tilt: 64, motorSpeed: 0, dimmer: 255 };

var lights = {
    // Blizzard Flurry Tri Lights
    flurries: [
        new FlurryTri(0, flurryInit), new FlurryTri(1, flurryInit), new FlurryTri(2, flurryInit), new FlurryTri(3, flurryInit),
    ],
    // Blizzard Q12+ Puck Lights
    pucks: [
        new PuckQ12(4), new PuckQ12(5), new PuckQ12(6), new PuckQ12(7),
    ],

    // Master light intensity
    master: 1,

    // Whether or not group mode has been enabled
    groupMode: false,

    // Different control groups of lighting channels/fixtures
    groups: [
        [1, 2, 3, 4, 5, 6, 7, 8],   // 1  all lights
        [1, 2, 5, 6],               // 2  left
        [3, 4, 7, 8],               // 3  right
        [5, 6, 7, 8],               // 4  top
        [1, 2, 3, 4],               // 5  bottom
        [3, 4, 5, 6],               // 6  top left, bottom right
        [1, 2, 7, 8],               // 7  bottom left, top right
        [1, 3],                     // 8  bottom 1st & 3rd
        [2, 4],                     // 9  bottom 2nd & 4th
        [5, 7],                     // 10 top 1st and 3rd
        [6, 8],                     // 11 top 2nd and 4th
        [1, 4],                     // 12 bottom 1st and 4th
        [2, 3],                     // 13 bottom 2nd and 3rd
        [5, 8],                     // 14 top 1st and 4th
        [6, 7],                     // 15 top 2nd and 3rd
    ],
};

// Add a convenient way to address all lights in a single list
lights.all = [];
for (var i = 0; i < lights.flurries.length; i++)    lights.all.push(lights.flurries[i]);
for (var i = 0; i < lights.pucks.length; i++)       lights.all.push(lights.pucks[i]);

// CC to lighting property mapping
var CC = {
    h:          2,      // color hue
    s:          3,      // color saturation
    v:          4,      // color value
    r:          5,      // color red
    g:          6,      // color green
    b:          7,      // color blue
    a:          8,      // color amber
    w:          9,      // color white
    mode:       10,     // color mode RGB/HSV 
    pan:        11,     // motor pan
    tilt:       12,     // motor tilt
    motorSpeed: 13,     // motor motor speed
    dimmer:     14,     // motor color dimmer/strobe
    master:     126,    // master lighting intenisty level
    groupMode:  127,    // enable/disable group mode
};

// Converts a MIDI CC value into a light property name
var getPropertyFromCC = function (cc) {
    for (var name in CC) {
        if (CC[name] == cc) return name;
    };
};
// Gets a parameter's value from a light given the MIDI CC #
var getValueFromCC = function (light, cc) {
    return light[getPropertyFromCC(cc)];
};

// Sets a parameter's value for a light given the MIDI CC #
var setValueFromCC = function (light, cc, value) {
    value = cc <= CC.v || cc == CC.intensity ? value / 127 : value * 2;
    light[getPropertyFromCC(cc)] = value;
};

// MIDI CC handler for light settings
var updateFromCC = function (type, channel, cc, value) {
    // Is this an update to the lighting control groups?
    if (cc == CC.groupMode) {
        // Update the lighting group mode
        lights.groupMode = value < 64 ? false : true;

        // Nothing to update light-wise - this just a control scheme update message
        return null;
    };

    // Get the current control group configuration
    var group = lights.groupMode ? lights.groups[channel - 1] : [channel];

    // Create the outgoing lighting state
    var state = { channel: [], value: [] };

    // Adjust master light intensity
    if (cc == CC.master) {
        lights.master = value / 127;
    }

    // Update each light in the group
    for (var i = 0; i < group.length; i++) {
        // Update light according to MIDI channel 1, 2, 3, 4 = Flurry lights | 5, 6, 7, 8 = Puck Lights
        var lightIndex = group[i] - 1;
        var light = lights.all[lightIndex]; // get light
        setValueFromCC(light, cc, value);   // update local light state/value

        // If this was a color update of some sort...
        if (cc <= CC.mode || cc == CC.master) {
            // Update lighting color mode as necessary
            if (cc == CC.mode) {
                light.mode = value < 64 ? HSV : RGB;
            }

            // Update HSV (direct change, intensity change, or light mode change)
            if (light.mode == HSV && (cc <= CC.v || cc == CC.master || cc == CC.mode)) {
                // Add the light's RGB channels for update
                state.channel.push(light.dmx("r"));
                state.channel.push(light.dmx("g"));
                state.channel.push(light.dmx("b"));
                state.value.push(light.h + ";" + light.s + ";" + (light.v * lights.master));
            }
            // Update RGB
            else if (light.mode == RGB) {
                // If this is an intensity or mode adjustment all colors must be updated
                if (cc == CC.v || cc == CC.master || cc == CC.mode) {
                    // Add RGB channels
                    state.channel.push(light.dmx("r"));
                    state.channel.push(light.dmx("g"));
                    state.channel.push(light.dmx("b"));

                    // Add RGB values
                    state.value.push(parseInt(light.r * light.v * lights.master));
                    state.value.push(parseInt(light.g * light.v * lights.master));
                    state.value.push(parseInt(light.b * light.v * lights.master));
                }
                // Otherwise just update single color channel
                else {
                    switch (cc) {
                        case CC.r:
                            state.channel.push(light.dmx("r"));
                            state.value.push(parseInt(light.r * light.v * lights.master));
                            break;

                        case CC.g:
                            state.channel.push(light.dmx("g"));
                            state.value.push(parseInt(light.g * light.v * lights.master));
                            break;

                        case CC.b:
                            state.channel.push(light.dmx("b"));
                            state.value.push(parseInt(light.b * light.v * lights.master));
                            break;
                    };
                }
            }

            // Treat amber/white LED updates separately
            if (light.channels.a && (cc == CC.a || cc == CC.w || cc == CC.v || cc == CC.master)) {
                // Update both amber and white
                if (cc == CC.v || cc == CC.master) {
                    // Add AW channels
                    state.channel.push(light.dmx("a"));
                    state.channel.push(light.dmx("w"));

                    // Add RGB values
                    state.value.push(parseInt(light.a * light.v * lights.master));
                    state.value.push(parseInt(light.w * light.v * lights.master));
                }
                // Update amber OR white
                else {
                    state.channel.push(cc == CC.a ? light.dmx("a") : light.dmx("w"));
                    state.value.push(parseInt((cc == CC.a ? light.a : light.w) * light.v * lights.master));
                }
            }
        }
        else {
            // Get the property and value to update
            var property = getPropertyFromCC(cc);
            var dmx = light.dmx(property);

            // Make sure this light has the property in question
            if (!dmx) continue;

            // Catch all - just apply the value directly to the channel for the light
            state.channel.push(dmx);
            state.value.push(light[property]);
        }
    }

    // Return the populated lighting state
    return state.channel.length > 0 ? state : null;
};