﻿// Blizzard Flurry Tri RGB moving wash lights
FlurryTri = function (index, options) {
    this.offset = (index * 16) + 1;
    this.mode = HSV;

    this.r = 0; this.g = 0; this.b = 0;
    this.h = 0; this.s = 1; this.v = 1;
    this.motorSpeed = 0; this.dimmer = 255;
    this.pan = 0; this.tilt = 0;

    if (options != null) {
        for (var name in options) {
            this[name] = options[name];
        }
    }

    // channel offsets
    this.channels = { r: 6, g: 7, b: 8, pan: 0, tilt: 2, motorSpeed: 4, dimmer: 5 };
};

// Returns the offset DMX channel number for the given relative DMX channel
FlurryTri.prototype.dmx = function (channel) {
    var type = typeof (channel);

    switch (type) {
        case "string": return this.offset + this.channels[channel];
        case "number": return this.offset + channel - 1;
    }

    return false;
}