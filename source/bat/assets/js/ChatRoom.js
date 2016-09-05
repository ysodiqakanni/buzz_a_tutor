// script to update the page and send messages
$(function () {
    // Declare a proxy to reference the hub.
    var chat = $.connection.chatHub;
    var getChatPath = $("#getChatHistoryPath").val();

    // Create a function that the hub can call to broadcast messages.
    chat.client.broadcastMessage = function (name, message) {
        // Html encode display name and message.
        var encodedName = $('<div />').text(name).html();
        var encodedMsg = $('<div />').text(message).html();
        // Add the message to the page.
        $('#discussion').prepend('<li class="chatMessage"><strong>' + encodedName
            + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
    };

    // Get the user name and store it to prepend to messages.
    var displayname = username;

    // Get Chat's History
    $.ajax({
        type: "GET", // Type of request
        url: getChatPath, //The controller/Action
        dataType: "json",
        data: {
            "lessonId": lessonId
        },
        success: function (data) {
            var message = JSON.parse(data);
            for (i = 0; i < message.length; i++) {
                // Html encode display name and message.
                var encodedName = $('<div />').text(message[i].name).html();
                var encodedMsg = $('<div />').text(message[i].msg).html();
                // Add the message to the page.
                $('#discussion').prepend('<li class="chatMessage"><strong>' + encodedName
                    + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
            }
        },
        error: function (err) {
            console.log("error[" + err.status + "]: " + err.statusText);
        }
    });

    // Start the connection.
    $.connection.hub.start().done(function () {       
        // Join the Lesson's Chat room
        chat.server.joinGroup(lessonId, displayname);

        function sendMessage() {
            // Checks if message input is blank.
            if ($('#message').val() !== '') {
                // Call the Send method on the hub.
                var messagePacket = ({ Name: displayname, Msg: $('#message').val(), GroupName: lessonId });
                chat.server.send(messagePacket);
                // Clear text box and reset focus for next comment.
                $('#message').val('').focus();
            }
        }

        $('#sendmessage').click(function () {
            sendMessage();
        });

        $("#message").keydown(function (event) {
            if (event.which == 13) {
                sendMessage();
            }
        })
    });

    // Lost connection with Server
    $.connection.hub.reconnecting(function () {
        // Add the disconnect message to the page.
        $('#discussion').prepend('<li class="chatMessage"><strong> Classroom:</strong> Rejoining Classroom</li>');
    })
});