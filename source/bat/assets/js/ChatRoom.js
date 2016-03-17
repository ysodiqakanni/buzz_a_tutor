// script to update the page and send messages
$(function () {
    // Declare a proxy to reference the hub.
    var chat = $.connection.chatHub;

    // Create a function that the hub can call to broadcast messages.
    chat.client.broadcastMessage = function (name, message) {
        // Html encode display name and message.
        var encodedName = $('<div />').text(name).html();
        var encodedMsg = $('<div />').text(message).html();
        // Add the message to the page.
        $('#discussion').append('<li><strong>' + encodedName
            + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
    };

    // Get the user name and store it to prepend to messages.
    var displayname = hostName;

    // Start the connection.
    $.connection.hub.start().done(function () {

        // Join the Lesson's Chat room
        chat.server.joinGroup(lessonId);

        function sendMessage() {
            // Checks if message input is blank.
            if ($('#message').val() !== '') {
                // Call the Send method on the hub.
                var messagePacket = ({ Name: displayname, Msg: $('#message').val(), Group: lessonId });
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
});