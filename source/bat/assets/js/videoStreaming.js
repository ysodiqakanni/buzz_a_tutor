﻿//Tokbox

// Shared variables

var apiKey = tbKey;

var session; // Session object
var publisher; // Publisher object

var sessionId; // Session Id

var token; // Client's token

var lessonId; // the lesson's id
var streamName; // The stream's name: lessonId - clients ID

//Get dimentions of stream windows
var selfWidth = $("#self").width();
var selfHeight = $("#self").height();

var teacherWidth = $("#teacher").width();
var teacherHeight = $("#teacher").height();

var otherWidth = $("div[id^=other-]").width();
var otherHeight = $("div[id^=other-]").height();

var selfBox = 'streamBoxSelf';
var teacherBox = 'streamBoxTeacher';
var otherBox;

//  **** Sessions *****
var connected = false; // Check if connected to the session

//Connecting to the session
var connect = function (sessionId) {
    session = OT.initSession(apiKey, sessionId);

    //Logs when clients joins and disconnects from the session
    var connectionCount = 0;
    session.on({
        connectionCreated: function (event) {
            connectionCount++;
            if (event.connection.connectionId != session.connection.connectionId) {
                console.log('Another client connected. ' + connectionCount + ' total.');
            }
        },
        connectionDestroyed: function connectionDestroyedHandler(event) {
            connectionCount--;
            console.log('A client disconnected. ' + connectionCount + ' total.');
        },
        sessionReconnecting: function (event) {
            console.log('Disconnected from the session. Attempting to reconnect...');
        },
        sessionReconnected: function (event) {
            console.log('Reconnected to the session.');
        },
        sessionDisconnected: function (event) {
            console.log('Disconnected from the session.');
        }
    });

    session.connect(token, function (error) {
        if (error) {
            if (error.code === 1006) {
                alert('Failed to connect. Please check your connection and try connecting again.');
            } else {
                alert('An unknown error occurred connecting. Please try again later.');
            }
        } else {
            connected = true;
            console.log("Connected to the session");
        }
    });

    // ***** Subscribing *****
    session.on("streamCreated", function (event) {
        var subStreamWidth,
            subStreamHeight;

        var options = {
            width: subStreamWidth,
            height: subStreamHeight,
            nameDisplayMode: "off"
        }

        stream = event.stream;
        console.log("New stream in the session: " + stream.streamId);
        var streamName = stream.name;
        var streamNameArray = streamName.split('-');
        var streamUserId = streamNameArray[0];
        var streamRole = streamNameArray[1];

        // Debugging
        //console.log(streamLessonID);       
        //if teacher
        if (streamRole == 'Teacher') {
            teacherWidth = $("#teacher").width();
            teacherHeight = $("#teacher").height();
            options.width = teacherWidth;
            options.height = teacherHeight;
            session.subscribe(stream, teacherBox, options);
        } else {
            otherWidth = $("div[id^=other-]").width();
            otherHeight = $("div[id^=other-]").height();
            options.width = otherWidth;
            options.height = otherHeight;
            otherBox = 'streamBoxOther-' + streamUserId;

            var dummyRow = "";
            var rowNum = $("#students").children().length-1;
            var currentStreamRow = $("#videoStreamRow-" + rowNum);
            var studentVideoCount = currentStreamRow.children().length;
            if (studentVideoCount == 2) {
                var newRowNum = rowNum + 1;
                dummyRow = $("#hdnRow").clone();
                $("#students").append(dummyRow);
                $("#students #hdnRow").attr("id", "videoStreamRow-" + newRowNum);
                $("#students #videoStreamRow-" + newRowNum).css("display", "block");
                var subContainer = document.createElement('div');
                subContainer.id = 'stream-' + streamUserId;
                subContainer.className = "StudentVideo";
                document.getElementById('videoStreamRow-' + newRowNum).appendChild(subContainer);
                session.subscribe(stream, subContainer, options);
            } else {
                var subContainer = document.createElement('div');
                subContainer.id = 'stream-' + streamUserId;
                subContainer.className = "StudentVideo";
                document.getElementById('videoStreamRow-' + rowNum).appendChild(subContainer);
                session.subscribe(stream, subContainer, options);
            }
            //var subContainer = document.createElement('div');
            //console.log(subContainer);
            //subContainer.id = 'stream-' + streamUserId;
            //document.getElementById('students').appendChild(subContainer);
            //session.subscribe(stream, subContainer, options);
            
        }
    });

    session.on("streamDestroyed", function (event) {
        console.log("Stream stopped. Reason: " + event.reason);

        stream = event.stream;
        var streamName = stream.name;
        var streamNameArray = streamName.split('-');
        var streamUserId = streamNameArray[0];
        var streamRole = streamNameArray[1];
        if (streamRole == 'Teacher') {
            $("#teacher").append('<div id="streamBoxTeacher"></div>')
        } else {
            $("#students").remove("#stream-" + streamUserId);
            //$("#other-"+ streamUserId).append('<div id="streamBoxOther-' + streamUserId + '"></div>')
        }
    });
}

// Disconnecting for the session
var sessionDisconnect = function () {
    session.disconnect();
    connected = false;
    console.log("Disconnected from session");
}

// ***** Publishing *****
// Variables
var targetElement; // The element on page to be replaced with tokbox video element.
var streamWidth,
    streamHeight;
if (role == '2') {
 
    targetElement = 'streamBoxTeacher';
    streamWidth = teacherWidth;
    streamHeight = teacherHeight;
} else {
    targetElement = 'streamBoxSelf';
    streamWidth = selfWidth;
    streamHeight = selfHeight;

    //var dummyRow = "";
    //var rowNum = $("#students").children().length - 1;
    //var currentStreamRow = $("#videoStreamRow-" + rowNum);
    //var studentVideoCount = currentStreamRow.children().length;
    //if (studentVideoCount == 2) {
    //    var newRowNum = rowNum + 1;
    //    dummyRow = $("#hdnRow").clone();
    //    $("#students").append(dummyRow);
    //    $("#students #hdnRow").attr("id", "videoStreamRow-" + newRowNum);
    //    $("#students #videoStreamRow-" + newRowNum).css("display", "block");
    //    targetElement = document.createElement('div');
    //    targetElement.id = 'stream-' + id;
    //    targetElement.className = "StudentVideo";
    //    document.getElementById('videoStreamRow-' + newRowNum).appendChild(targetElement);
    //} else {
    //    targetElement = document.createElement('div');
    //    targetElement.id = 'stream-' + id;
    //    targetElement.className = "StudentVideo";
    //    document.getElementById('videoStreamRow-' + rowNum).appendChild(targetElement);
    //}

    //targetElement = document.createElement('div');
    
    //targetElement.id = 'stream-' + id;
    //document.getElementById('students').appendChild(targetElement);
}


// Creating a stream
var startStream = function (sessionId, token) {
    if (connected == true) {
        if (role == '2') {
            streamWidth = $("#teacher").width();
            streamHeight = $("#teacher").height();
        } else {
            //blackboardHub.server.updateStreamStudents(lessonId, id, "true");
            //BeforeStartStreamingStudent();


            var dummyRow = "";
            var rowNum = $("#students").children().length - 1;
            var currentStreamRow = $("#videoStreamRow-" + rowNum);
            var studentVideoCount = currentStreamRow.children().length;
            if (studentVideoCount == 2) {
                var newRowNum = rowNum + 1;
                dummyRow = $("#hdnRow").clone();
                $("#students").append(dummyRow);
                $("#students #hdnRow").attr("id", "videoStreamRow-" + newRowNum);
                $("#students #videoStreamRow-" + newRowNum).css("display", "block");
                targetElement = document.createElement('div');
                targetElement.id = 'stream-' + id;
                targetElement.className = "StudentVideo";
                document.getElementById('videoStreamRow-' + newRowNum).appendChild(targetElement);
            } else {
                targetElement = document.createElement('div');
                targetElement.id = 'stream-' + id;
                targetElement.className = "StudentVideo";
                document.getElementById('videoStreamRow-' + rowNum).appendChild(targetElement);
            }

            streamWidth = $("#self").width();
            streamHeight = $("#self").height();
        }
        publisher = OT.initPublisher(targetElement, {
            resolution: '320x240',
            frameRate: 15,
            width: streamWidth,
            height: streamHeight,
            name: streamName,
            nameDisplayMode: "on"
        });

        session.publish(publisher, function (error) {
            if (error) {
                console.log(error);
            } else {
                console.log('Publishing a stream.');
                $('#stop').removeClass('hidden');
                $('#start').addClass('hidden');
            }
        });

    } else {
        console.log("Not connected to session");
    }
}

// Stop publishing
var stopStream = function () {
    session.unpublish(publisher);
    console.log("Stopped streaming")
    //blackboardHub.server.updateStreamStudents(lessonId, id, "false");
    if (role == '2') {
        $("#teacher").append('<div id="streamBoxTeacher"></div>')
    } else {
        $("#students").remove("#stream-" + id);
        //$("#self").append('<div id="streamBoxSelf"></div>');
        //EndStreamingStudent();
    }
    $('#start').removeClass('hidden');
    $('#stop').addClass('hidden');
}

function BeforeStartStreamingStudent() {
    //$("#selfBox-" + id).css("display", "block");
    //blackboardHub.server.changeVideoState(lessonId, id,"true");
    //$("#teacher").css("height", $("#video-wrap").height() - 111);
    //$("#streamBoxTeacher").css("height", $("#video-wrap").height() - 111);
    //$('.owl-carousel').trigger('add.owl.carousel', [replicateSelfStudent(id, userFirstName)]).trigger('refresh.owl.carousel');
    //blackboardHub.server.fetchUserOnStartClickList(lessonId,id);
}

function EndStreamingStudent() {
    //$("#selfBox-" + id).css("display", "none");
    //blackboardHub.server.changeVideoState(lessonId, id, "true");
    //var itemCount = $('.owl-item').length;
    //var elemIndex = -1;
    //if (itemCount > 0) {
    //    var itemArr = $('.owl-item').children();
    //    $.each(itemArr, function (index, value) {
    //        var tempId = value.id.split("-")[1];
    //        if (tempId == id) {
    //            elemIndex = index;
    //        }
    //    });
    //}
    //if (elemIndex != -1) {
    //    $(".owl-carousel").trigger('remove.owl.carousel', [elemIndex]);
    //}
    //$("#teacher").css("height", $("#video-wrap").height() - $("#shop").height());
    //$("#streamBoxTeacher").css("height", $("#video-wrap").height() - $("#shop").height());
    //blackboardHub.server.fetchUserListOnDisconnect(lessonId, $.connection.hub.id, id);
}
