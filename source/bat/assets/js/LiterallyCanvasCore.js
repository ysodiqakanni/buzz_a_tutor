
var blackboardHub = $.connection.blackboardHub;
var teacherCanvas;
var studentCanvas;

$(function () {
    blackboardHub.client.getTeacherSnapshot = function () {
       
        var snaps = teacherCanvas.getSnapshot();
        console.log(JSON.stringify(snaps));
        blackboardHub.server.uploadSnapshot(JSON.stringify(snaps), lessonId);
    };
    blackboardHub.client.loadSnapShot = function (snapshotString) {
        debugger
        var lc1 = LC.init(document.getElementById("lc"), {
            imageURLPrefix: '../Content/lc-images',
            snapshot: JSON.parse(snapshotString),
            toolbarPosition: 'hidden',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30]
        });
    };
});

$(document).ready(function () {

    $.connection.hub.start().done(function () {
        // Join the Lesson's Blackboard
        //blackboardHub.server.joinGroup(lessonId);
        debugger;
        blackboardHub.server.joinGroup(lessonId, id, username, IsHost, IsHaveControl);
        if (IsHost === "true") {
            teacherCanvas = LC.init(document.getElementById("lc"), {
                imageURLPrefix: '../assets/img/lc-images',

                toolbarPosition: 'bottom',
                defaultStrokeWidth: 2,
                strokeWidths: [1, 2, 3, 5, 30]
            });
        }
        else {
            blackboardHub.server.getTeacherSnapshot(lessonId);
        }
        //paint();
        // Start the client side server update interval
        //setInterval(updateServerModel, updateRate);
    });

    //if (isHost === "True") {
    //    teacherCanvas = LC.init(document.getElementById("lc"), {
    //        imageURLPrefix: '../assets/img/lc-images',

    //        toolbarPosition: 'bottom',
    //        defaultStrokeWidth: 2,
    //        strokeWidths: [1, 2, 3, 5, 30]
    //    });
    //}
    //else {
    //    studentCanvas = LC.init(document.getElementById("lc"), {
    //        imageURLPrefix: '../assets/img/lc-images',

    //        toolbarPosition: 'hidden',
    //        defaultStrokeWidth: 2,
    //        strokeWidths: [1, 2, 3, 5, 30]
    //    });
    //}
});

//function UploadSnapshot(snapShot) {
//    debugger;
    
//}