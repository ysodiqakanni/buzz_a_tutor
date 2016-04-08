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

var otherWidth = $("#other").width();
var otherHeight = $("#other").height()


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
        var options = {
            width: otherWidth,
            height: otherHeight,
            nameDisplayMode: "off"
        }

        stream = event.stream;
        console.log("New stream in the session: " + stream.streamId);
        var streamName = stream.name;
        var streamNameArray = streamName.split('-');
        var streamLessonID = streamNameArray[0];

        // Debugging
        //console.log(streamLessonID);

        if (streamLessonID == lessonId) {
            session.subscribe(stream, streamBoxOther, options);
        } else {
            console.log("Not the lesson.")
        }
    });

    session.on("streamDestroyed", function (event) {
        console.log("Stream stopped. Reason: " + event.reason);
        $("#other").append('<div id="streamBoxOther"></div>')
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
var targetElement = 'streamBoxSelf'; // The element on page to be replaced with tokbox video element.

// Creating a stream
var startStream = function (sessionId, token) {
    if (connected == true) {
        publisher = OT.initPublisher(targetElement, {
            resolution: '320x240',
            frameRate: 15,
            width: selfWidth,
            height: selfHeight,
            name: streamName,
            nameDisplayMode: "off"
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

    $("#self").append('<div id="streamBoxSelf"></div>')
    $('#start').removeClass('hidden');
    $('#stop').addClass('hidden');
}
