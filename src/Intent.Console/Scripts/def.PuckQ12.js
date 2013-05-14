// Blizzard Q12+ static par cans
PuckQ12 = function (index, options) {
    this.offset = (index * 16) + 1;
    this.mode = RGB;

    this.intensity = 1;
    this.r = 0; this.g = 0; this.b = 0;
    this.a = 0; this.w = 0;
    this.h = 0; this.s = 1; this.v = 1;

    if (options != null) {
        for (var name in options) {
            this[name] = options[name];
        }
    }
};

// Returns the DMX channel/address for the light's Red channel
PuckQ12.prototype.redDmx = function () {
    return this.offset + 0;
};

// Returns the DMX channel/address for the light's Green channel
PuckQ12.prototype.greenDmx = function () {
    return this.offset + 1;
};

// Returns the DMX channel/address for the light's Blue channel
PuckQ12.prototype.blueDmx = function () {
    return this.offset + 2;
};

// Returns the DMX channel/address for the light's Pan channel
PuckQ12.prototype.amberDmx = function () {
    return this.offset + 3;
};

// Returns the DMX channel/address for the light's Tilt channel
PuckQ12.prototype.whiteDmx = function () {
    return this.offset + 4;
};

// Returns the offset DMX channel number for the given relative DMX channel
PuckQ12.prototype.toGlobalDmx = function (channel) {
    if (channel == null) return this.offset;
    return this.offset + channel - 1;
}
