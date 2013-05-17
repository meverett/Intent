//var count = 0;
//var updateIntensity = 0;

//var midiDate = function (type, channel, value1, value2) {
//    var state = { channel: [], value: [] };

//    for (var i = 0; i < lights.all.length; i++) {
//        var light = lights.all[i];
//        state.channel.push(light.dmx("r"));
//        state.channel.push(light.dmx("g"));
//        state.channel.push(light.dmx("b"));
//        state.value.push(light.h + ";" + light.s + ";" + (light.v * updateIntensity));
//    }

//    return state;
//};

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
//        data: midiDate
//    });
//};

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
        "Note On": {
            message: "note on",
            address: "/lights",
            data: updateFromCC
        }
    },

    // Update loop
    //update: midiUpdate
};

addAdapter(adapters.midiToOsc, midiToOsc);