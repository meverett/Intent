var count = 0;
var updateIntensity = 0.25;
var updateHue = 0;
var pos = { x: 0, y: 0 };

var midiData = function (type, channel, value1, value2) {
    var state = { channel: [], value: [] };

    for (var i = 0; i < lights.all.length; i++) {
        var light = lights.all[i];
        state.channel.push(light.dmx("r"));
        state.channel.push(light.dmx("g"));
        state.channel.push(light.dmx("b"));
        //state.value.push(light.h + ";" + light.s + ";" + (light.v * updateIntensity));
        state.value.push(updateHue + ";" + light.s + ";" + (light.v * updateIntensity));
    }

    return state;
};

var midiMouse = function (type, channel, value1, value2) {
    var state = { channel: [], value: [] };

    // Convert mouse X to DMX pan value range
    var dmxPan = (255 - parseInt(255 * (pos.x / 1680))) / 2;

    // Convert mouse X to DMX pan value range
    var dmxTilt = (parseInt(255 * (pos.y / 1050)) / 2);

    for (var i = 0; i < lights.flurries.length; i++) {
        state.channel.push(lights.flurries[i].dmx("pan"));
        state.value.push(dmxPan);
    }

    for (var i = 0; i < lights.flurries.length; i++) {
        state.channel.push(lights.flurries[i].dmx("tilt"));
        state.value.push(dmxTilt);
    }

    return state;
};

//var midiUpdate = function () {
//    if (!midiToOsc.id) return;

//    updateIntensity = (Math.sin(count) / 2) + 0.5;
//    count += 0.1;

//    sendMessage(midiToOsc.id, {
//        type: "control change",
//        channel: 1,
//        value1: CC.v,
//        value2: 0,
//        address: "/lights",
//        data: midiData
//    });
//};

var midiUpdate = function () {
    if (!midiToOsc.id) return;

    //updateIntensity = (Math.sin(count) / 2) + 0.5;
    //count += 0.1;

    // If we are increasing intensity, do so
    if (midiToOsc.intensity) {
        updateIntensity += 0.01;
    }
    // Otherwise if cycling hue do so
    else if (midiToOsc.hue) {
        updateHue += 0.01;
    }
    // Otherwise decay the intensity
    else {
        updateIntensity *= 0.975;
    }

    // Clamp values
    if (updateIntensity > 1) updateIntensity = 1;
    else if (updateIntensity < 0.25) updateIntensity = 0.25;
    if (updateHue > 1) updateHue = 0;

    // Get mouse position    
    //pos = getMousePos();

    //sendMessage(midiToOsc.id, {
    //    type: "note on",
    //    channel: 1,
    //    value1: 0,
    //    value2: 0,
    //    address: "/lights",
    //    data: midiMouse
    //});

    // Send the lighting update message
    sendMessage(midiToOsc.id, {
        type: "note on",
        channel: 1,
        value1: 0,
        value2: 0,
        address: "/lights",
        data: midiData
    });
};

// Create the MIDI to OSC adapter
var midiToOsc = {   
    // MIDI Input device
    device: "02. Internal MIDI",

    routing: {
        // Update from MIDI Control Change messages
        "Control Change": {
            message: "control change",
            address: "/lights",
            data: updateFromCC
        },
        // Update from MIDI note on messages
        //"Note On": {
        //    message: "note on",
        //    address: "/lights",
        //    data: updateFromCC
        //}

        "Intesity Note On": {
            message: "note on",
            value1: "C3",
            data: function (type, channel, value1, value2) {
                midiToOsc.intensity = true;
                //print("intensity: true");
            }
        },

        "Intesity Note Off": {
            message: "note off",
            value1: "C3",
            data: function (type, channel, value1, value2) {
                midiToOsc.intensity = false;
                //print("intensity: false");
            }
        },

        "Hue Note On": {
            message: "note on",
            value1: "D3",
            data: function (type, channel, value1, value2) {
                midiToOsc.hue = true;
                //print("hue: true");
            }
        },

        "Hue Note Off": {
            message: "note off",
            value1: "D3",
            data: function (type, channel, value1, value2) {
                midiToOsc.hue = false;
                //print("hue: false");
            }
        }
    },

    // Update loop
    //update: midiUpdate
};

addAdapter(adapters.midiToOsc, midiToOsc);
//addAdapter(adapters.midiToConsole, midiToOsc);