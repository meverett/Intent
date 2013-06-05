// Blizzard Q12+ static par cans
PuckQ12 = function (index, options) {
    this.offset = (index * 16) + 1;
    this.mode = HSV;

    this.r = 0; this.g = 0; this.b = 0;
    this.a = 0; this.w = 0;
    this.h = 0; this.s = 1; this.v = 1;
        
    if (options != null) {
        for (var name in options) {
            this[name] = options[name];
        }
    }

    // channel offsets
    this.channels = { r: 0, g: 1, b: 2, a: 3, w: 4 };
};

// Returns the offset DMX channel number for the given relative DMX channel
PuckQ12.prototype.dmx = function (channel) {
    var type = typeof (channel);
    
    switch (type) {
        case "string": return this.offset + this.channels[channel];
        case "number": return this.offset + channel - 1;
    }

    return false;
}
