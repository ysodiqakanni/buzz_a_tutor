
var blackboardHub = $.connection.blackboardHub;
var teacherCanvas;
var studentCanvas;
var onInit = true;

$(function () {
    blackboardHub.client.getTeacherSnapshot = function () {      
        var snaps = teacherCanvas.getSnapshot();
        blackboardHub.server.uploadSnapshotOnInit(JSON.stringify(snaps), lessonId);
    };
    blackboardHub.client.loadSnapShotOnInit = function (snapshotString) {
        studentCanvas = LC.init(document.getElementById("lc"), {
            imageURLPrefix: '../Content/lc-images',
            snapshot: JSON.parse(snapshotString),
            toolbarPosition: 'hidden',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30]
        });
    };
    blackboardHub.client.loadSnapShot = function (snapshotString) {
        studentCanvas.loadSnapshot(JSON.parse(snapshotString));
    };

    blackboardHub.client.loadShape = function (shapeString, previousShapeId) {
        var shapeToRender = LC.JSONToShape(JSON.parse(shapeString));
        studentCanvas.saveShape(shapeToRender, true, previousShapeId);
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
            if (onInit) {
                onInit = false;
                var snaps = teacherCanvas.getSnapshot();
                blackboardHub.server.uploadSnapshotOnInit(JSON.stringify(snaps), lessonId);
            }
            teacherCanvas.on("shapeSave", function (shape, previousShapeId) {               
                var shapeString = LC.shapeToJSON(shape.shape);
                blackboardHub.server.uploadShape(JSON.stringify(shapeString),previousShapeId, lessonId);
            });
            teacherCanvas.on("clear", function () {
                var snaps = teacherCanvas.getSnapshot();
                blackboardHub.server.uploadSnapshot(JSON.stringify(snaps), lessonId);
            });
            teacherCanvas.on("undo", function () {
                var snaps = teacherCanvas.getSnapshot();
                blackboardHub.server.uploadSnapshot(JSON.stringify(snaps), lessonId);
            });
            teacherCanvas.on("redo", function () {
                var snaps = teacherCanvas.getSnapshot();
                blackboardHub.server.uploadSnapshot(JSON.stringify(snaps), lessonId);
            });
            teacherCanvas.on("primaryColorChange", function () {
                var snaps = teacherCanvas.getSnapshot();
                blackboardHub.server.uploadSnapshot(JSON.stringify(snaps), lessonId);
            });
            teacherCanvas.on("secondaryColorChange", function () {
                var snaps = teacherCanvas.getSnapshot();
                blackboardHub.server.uploadSnapshot(JSON.stringify(snaps), lessonId);
            });
            teacherCanvas.on("backgroundColorChange", function () {
                var snaps = teacherCanvas.getSnapshot();
                blackboardHub.server.uploadSnapshot(JSON.stringify(snaps), lessonId);
            });
        }
        else {
            blackboardHub.server.getTeacherSnapshot(lessonId);
        }
    });
});