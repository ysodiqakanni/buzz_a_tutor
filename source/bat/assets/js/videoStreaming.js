//Tokbox
var apiKey = "45496652"; //API Key

var session;
var publisher;
var connected = false;
var targetElement = 'streamBoxSelf';

var connect = function (sessionId) {

    session = OT.initSession(apiKey, sessionId);

    //Logs when someone joins and disconnects
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
}
   

var startStream = function (sessionId, token) {
    if (connected == true) {
        publisher = OT.initPublisher(targetElement, {
            resolution: '320x240',
            frameRate: 15,
            width:400, 
            height: 300,
            name: streamName
        });

        session.publish(publisher, function(error) {
            if (error) {
                console.log(error);
            } else {
                console.log('Publishing a stream.');
                $('#stop').removeClass('hidden');
                $('#start').addClass('hidden');
            }
        });

        //session.on("streamCreated", function (event) {
        //    session.subscribe(event.stream);
        //});
    } else {
        console.log("Not connected to session");
    }
}

var sessionDisconnect = function () {
    session.disconnect();
    connected = false;
    console.log("Disconnected from session");
}

var stopStream = function () {
    session.unpublish(publisher);
    console.log("Stopped streaming")

    $("#streamCon").append('<div id="streamBoxSelf"></div>')
    $('#start').removeClass('hidden');
    $('#stop').addClass('hidden');
}

function resizePublisher() {
    publisher.element.style.width = "1000px";
    publisher.element.style.height = "750px";
}