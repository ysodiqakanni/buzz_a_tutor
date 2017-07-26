//Tokbox

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

var displayVideoCount;

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
        if (streamRole == 'Teacher') {
            teacherWidth = $("#teacher").width();
            teacherHeight = $("#teacher").height();
            options.width = teacherWidth;
            options.height = teacherHeight;
            session.subscribe(stream, teacherBox, options);
        } else {
            options = setVideoSize();

            var subContainer = document.createElement('div');
            subContainer.id = 'stream-' + streamUserId;
            subContainer.className = "StudentVideo";
            document.getElementById('videoStreamRow-0').appendChild(subContainer);
            session.subscribe(stream, subContainer, options);
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
}


// Creating a stream
var startStream = function (sessionId, token) {
    if (connected == true) {
        if (role == '2') {
            streamWidth = $("#teacher").width();
            streamHeight = $("#teacher").height();
        } else {

            targetElement = document.createElement('div');
            targetElement.id = 'stream-' + id;
            targetElement.className = "StudentVideo";
            document.getElementById('videoStreamRow-0').appendChild(targetElement);

            //streamWidth = $("#self").width();
            //streamHeight = $("#self").height();
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
    if (role == '2') {
        $("#teacher").append('<div id="streamBoxTeacher"></div>')
    } else {
        $("#students").remove("#stream-" + id);
    }
    $('#start').removeClass('hidden');
    $('#stop').addClass('hidden');
}

$(function () {
    var windowWidth = $(window).width();
    switch (windowWidth) {
        case 320:
            displayVideoCount = 2;
            break;
        case 375:
            displayVideoCount = 2;
            break;
        case 425:
            displayVideoCount = 3;
            break;
        case 768:
            displayVideoCount = 3;
            break;
        case 1024:
            displayVideoCount = 4;
            break;
        case 1440:
            displayVideoCount = 5;
            break;
        case 2560:
            displayVideoCount = 6;
            break;
        default:
            displayVideoCount = 4;
            break;
    }
});

function setVideoSize() {
    var windowWidth = $(window).width();
    var newOptions = {};
    switch (windowWidth) {
        case 320:
            newOptions = {
                width: "45%",
                height: "45%",
                nameDisplayMode: "off"
            }
            break;
        case 375:
            newOptions = {
                width: "45%",
                height: "45%",
                nameDisplayMode: "off"
            }
            break;
        case 425:
            newOptions = {
                width: "45%",
                height: "45%",
                nameDisplayMode: "off"
            }
            break;
        case 768:
            newOptions = {
                width: "45%",
                height: "45%",
                nameDisplayMode: "off"
            }
            break;
        case 1024:
            newOptions = {
                width: "45%",
                height: "45%",
                nameDisplayMode: "off"
            }
            break;
        case 1440:
            newOptions = {
                width: "45%",
                height: "45%",
                nameDisplayMode: "off"
            }
            break;
        case 2560:
            newOptions = {
                width: "45%",
                height: "45%",
                nameDisplayMode: "off"
            }
            break;
        default:
            newOptions = {
                width: "45%",
                height: "45%",
                nameDisplayMode: "off"
            }
            break;
    }
    return newOptions;
}