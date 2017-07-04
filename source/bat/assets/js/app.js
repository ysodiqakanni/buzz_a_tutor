// replace these values with those generated in your TokBox Account
var apiKey = "45897022";
var sessionId = "1_MX40NTg5NzAyMn5-MTQ5ODAyMzM1NTMwMn5DdFFoc3BRMktvdjVZUGNpZm9KV2s2ajR-fg";
var token = "T1==cGFydG5lcl9pZD00NTg5NzAyMiZzaWc9ZDVkZDkyNzg5ZTE2YWRlNGFhN2MzYWRjOTc3NjUzMzAzYWNlNWQ0MjpzZXNzaW9uX2lkPTFfTVg0ME5UZzVOekF5TW41LU1UUTVPREF5TXpNMU5UTXdNbjVEZEZGb2MzQlJNa3R2ZGpWWlVHTnBabTlLVjJzMmFqUi1mZyZjcmVhdGVfdGltZT0xNDk4MDM3MDI1Jm5vbmNlPTAuOTkxMDcwMzY0NjI2OTU2MSZyb2xlPXB1Ymxpc2hlciZleHBpcmVfdGltZT0xNTAwNjI5MDI0";

// (optional) add server code here
initializeSession();

// Handling all of our errors here by alerting them
function handleError(error) {
    if (error) {
        alert(error.message);
    }
}

function initializeSession() {
    var session = OT.initSession(apiKey, sessionId);

    // Subscribe to a newly created stream
    session.on('streamCreated', function (event) {
        session.subscribe(event.stream, 'subscriber', {
            insertMode: 'append',
            width: '100%',
            height: '100%'
        }, handleError);
    });

    // Create a publisher
    var publisher = OT.initPublisher('publisher', {
        insertMode: 'append',
        width: '100%',
        height: '100%'
    }, handleError);

    // Connect to the session
    session.connect(token, function (error) {
        // If the connection is successful, publish to the session
        if (error) {
            handleError(error);
        } else {
            session.publish(publisher, handleError);
        }
    });
}