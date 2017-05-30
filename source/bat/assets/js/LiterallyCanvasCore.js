
var blackboardHub = $.connection.blackboardHub;
var teacherCanvas;
var studentCanvas;

$(function () {
    blackboardHub.client.getTeacherSnapshot = function () {      
        var snaps = teacherCanvas.getSnapshot();
        blackboardHub.server.uploadSnapshotOnInit(JSON.stringify(snaps), lessonId);
    };
    blackboardHub.client.loadSnapShotOnInit = function (snapshotString) {
        console.log("load before init called");
        studentCanvas = LC.init(document.getElementById("lc"), {
            imageURLPrefix: '../Content/lc-images',
            snapshot: JSON.parse(snapshotString),
            toolbarPosition: 'hidden',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30]
        });
    };
    blackboardHub.client.loadSnapShot = function (snapshotString) {
        console.log("load after init called");
        studentCanvas.loadSnapshot(JSON.parse(snapshotString));
    };
});

$(document).ready(function () {

    $.connection.hub.start().done(function () {
        blackboardHub.server.joinGroup(lessonId, id, username, IsHost, IsHaveControl);
        if (IsHost === "true") {
            teacherCanvas = LC.init(document.getElementById("lc"), {
                imageURLPrefix: '../assets/img/lc-images',

                toolbarPosition: 'bottom',
                defaultStrokeWidth: 2,
                strokeWidths: [1, 2, 3, 5, 30]
            });
            teacherCanvas.repaintAllLayers();
            teacherCanvas.on("mousedown", function () {
                var snaps = teacherCanvas.getSnapshot();
                blackboardHub.server.uploadSnapshot(JSON.stringify(snaps), lessonId);
            });
            teacherCanvas.on("mousemove", function () {
                var snaps = teacherCanvas.getSnapshot();
                blackboardHub.server.uploadSnapshot(JSON.stringify(snaps), lessonId);
            });
            teacherCanvas.on("mouseup", function () {
                var snaps = teacherCanvas.getSnapshot();
                blackboardHub.server.uploadSnapshot(JSON.stringify(snaps), lessonId);
            });
        }
        else {
            blackboardHub.server.getTeacherSnapshot(lessonId);
        }
    });
});