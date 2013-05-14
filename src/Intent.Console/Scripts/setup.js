// Global adapter names/identities
var adapters = {
    midiToConsole:  "MIDI to Console",  midiToOsc:  "MIDI to OSC",
    oscToConsole:   "OSC to Console",   oscToDmx:   "OSC to DMX"
};

// Different color modes
var HSV = 0;
var RGB = 1;

// Create global light structure
var flurryInit =    { pan: 100, tilt: 64 };

var lights = {
    // Blizzard Flurry Tri Lights
    flurries: [
        new FlurryTri(0, flurryInit), new FlurryTri(1, flurryInit), new FlurryTri(2, flurryInit), new FlurryTri(3, flurryInit),
    ],
    // Blizzard Q12+ Puck Lights
    pucks: [
        new PuckQ12(4), new PuckQ12(5), new PuckQ12(6), new PuckQ12(7),
    ],

    // Whether or not group mode has been enabled
    groupMode: false,

    // Different control groups of lighting channels/fixtures
    groups: [
        [1, 2, 3, 4, 5, 6, 7, 8],   // all lights
        [1, 2, 5, 6],               // left light tree
        [3, 4, 7, 8],               // right light tree
        [5, 6, 7, 8],               // top lights
        [1, 2, 3, 4],               // bottom lights
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
    intensity:  10,     // color intensity
    mode:       11,     // color mode RGB/HSV 
    pan:        12,     // motor pan
    tilt:       13,     // motor tilt
    motorSpeed: 14,     // motor motor speed
    dimmer:     15,     // motor color dimmer/strobe
    groupMode:  127,    // set control group
};

// Converts a MIDI CC value into a light property name
var getParameterFromCC = function (cc) {
    for (var name in CC) {
        if (CC[name] == cc) return name;
    };
};
// Gets a parameter's value from a light given the MIDI CC #
var getValueFromCC = function (light, cc) {
    return light[getParameterFromCC(cc)];
};

// Sets a parameter's value for a light given the MIDI CC #
var setValueFromCC = function (light, cc, value) {
    value = cc <= CC.v || cc == CC.intensity ? value / 127 : value * 2;
    light[getParameterFromCC(cc)] = value;
};

// MIDI CC handler for light settings
var updateFromCC = function (type, channel, cc, value) {
    // Is this an update to the lighting control groups?
    if (cc == CC.groupMode) {
        // Update the lighting group mode
        lights.groupMode = value < 64 ? false : true;

        print("updating group mode: " + lights.groupMode);

        // Nothing to update light-wise - this just a control scheme update message
        return null;
    };

    // Get the current control group configuration
    var group = lights.groupMode ? lights.groups[channel - 1] : [channel];

    print("group.length = " + group.length);

    // Create the outgoing lighting state
    var state = { channel: [], value: [] };

    // Update each light in the group
    for (var i = 0; i < group.length; i++) {
        // Update light according to MIDI channel 1, 2, 3, 4 = Flurry lights | 5, 6, 7, 8 = Puck Lights
        var light = lights.all[group[i] - 1];   // get light
        setValueFromCC(light, cc, value);       // update local light state/value

        // If this was a color update...
        if (cc <= CC.mode) {
            // Update lighting color mode as necessary
            if (cc == CC.mode) {
                light.mode = value < 64 ? HSV : RGB;
            }

            // Update HSV mode
            if (light.mode == HSV && (cc <= CC.v || cc == CC.mode)) {
                // Add the light's RGB channels for update
                state.channel.push(light.redDmx());
                state.channel.push(light.greenDmx());
                state.channel.push(light.blueDmx());
                state.value.push(light.h + ";" + light.s + ";" + (light.v * light.intensity));
            }
            // Update RGB mode
            else if (light.mode == RGB && cc >= CC.r) {
                // If this is an intensity or mode adjustment all colors must be updated
                if (cc == CC.intensity || cc == CC.mode) {
                    // Add RGB channels
                    state.channel.push(light.redDmx());
                    state.channel.push(light.greenDmx());
                    state.channel.push(light.blueDmx());

                    // Add RGB values
                    state.value.push(parseInt(light.r * light.intensity));
                    state.value.push(parseInt(light.g * light.intensity));
                    state.value.push(parseInt(light.b * light.intensity));
                }
                // Treat amber/white LED updates separate from RGB
                else if (cc == CC.a || cc == CC.w) {
                    state.channel.push(cc == CC.a ? light.amberDmx() : light.whiteDmx());
                    state.value.push(parseInt((cc == CC.a ? light.a : light.w) * light.intensity));
                }
                // Otherwise just update single color channel
                else {
                    switch (cc) {
                        case CC.r:
                            state.channel.push(light.redDmx());
                            state.value.push(parseInt(light.r * light.intensity));
                            break;

                        case CC.g:
                            state.channel.push(light.greenDmx());
                            state.value.push(parseInt(light.g * light.intensity));
                            break;

                        case CC.b:
                            state.channel.push(light.blueDmx());
                            state.value.push(parseInt(light.b * light.intensity));
                            break;
                    };
                }
            }
        }
        else {
            // Catch all - just apply the value directly to the channel for the light
            state.channel.push(light.toGlobalDmx(getParameterFromCC(cc)));
            state.value.push(getValueFromCC(light, cc));
        }
    }

    // Return the populated lighting state
    return state.channel.length > 0 ? state : null;
};