// Blizzard Flurry Tri RGB moving wash lights
FlurryTri = function (index, options) {
    this.offset = (index * 16) + 1;
    this.mode = HSV;

    this.intensity = 1;
    this.r = 0; this.g = 0; this.b = 0;
    this.h = 0; this.s = 1; this.v = 1;
    this.motorSpeed = 0; this.dimmer = 0;
    this.pan = 0; this.tilt = 0;

    if (options != null) {
        for (var name in options) {
            this[name] = options[name];
        }
    }
};

// Returns the DMX channel/address for the light's Red channel
FlurryTri.prototype.redDmx = function () {
    return this.offset + 6;
};

// Returns the DMX channel/address for the light's Green channel
FlurryTri.prototype.greenDmx = function () {
    return this.offset + 7;
};

// Returns the DMX channel/address for the light's Blue channel
FlurryTri.prototype.blueDmx = function () {
    return this.offset + 8;
};

// Returns the DMX channel/address for the light's Pan channel
FlurryTri.prototype.panDmx = function () {
    return this.offset + 0;
};

// Returns the DMX channel/address for the light's Tilt channel
FlurryTri.prototype.tiltDmx = function () {
    return this.offset + 2;
};

// Returns the DMX channel/address for the light's Motor Speed channel
FlurryTri.prototype.motorSpeedDmx = function () {
    return this.offset + 4;
};

// Returns the DMX channel/address for the light's dimmer/strobe channel
FlurryTri.prototype.dimmerDmx = function () {
    return this.offset + 5;
};

// Returns the offset DMX channel number for the given relative DMX channel
FlurryTri.prototype.toGlobalDmx = function (channel) {
    if (channel == null) return this.offset;
    return this.offset + channel - 1;
}
