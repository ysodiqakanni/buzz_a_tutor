var demo;
(function (demo) {
    var Blah = (function () {
        function Blah() {
        }
        Blah.prototype.Hello = function (paramIn) {
            console.log(paramIn);
        };
        return Blah;
    }());
    demo.Blah = Blah;
})(demo || (demo = {}));
//# sourceMappingURL=demo.js.map