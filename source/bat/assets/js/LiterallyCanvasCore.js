$(document).ready(function () {
    debugger;
    if (isHost === "True") {
        var lc = LC.init(document.getElementById("lc"), {
            imageURLPrefix: '../assets/img/lc-images',

            toolbarPosition: 'bottom',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30]
        });
    }
    else {
        var lc = LC.init(document.getElementById("lc"), {
            imageURLPrefix: '../assets/img/lc-images',

            toolbarPosition: 'hidden',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30]
        });
    }
});