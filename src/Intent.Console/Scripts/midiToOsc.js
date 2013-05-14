// Create the MIDI to OSC adapter
var midiToOsc = {
    routing: {
        // Update from MIDI Control Change messages
        "Control Change": {
            message: "control change",
            address: "/global",
            data: updateFromCC
        },
        // Update from MIDI note on messages
        "Note On": {
            message: "note on",
            address: "/global",
            data: updateFromCC
        }
    }
};

addAdapter(adapters.midiToOsc, midiToOsc);