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
    }
};

addAdapter(adapters.midiToOsc, midiToOsc);