// Set up connection the Blackboard Hub
var blackboardHub = $.connection.blackboardHub,
    $bb = $('#canvas'),
    //// Send a maximum of 10 messages per second
    //// (mouse movements trigger a lot of messages)
    //messageFrequency = 1000,
    //// Determine how often to send messages in
    //// time to abide by the messageFrequency
    //updateRate = 10 / messageFrequency,
    clientModel =
    serverModel = {
        oX: 0,
        oY: 0,
        nX: 0,
        nY: 0,
        lineWidth: '3',
        color: '#000',
        group: lessonId,
    },
    moved = false;

// Update Blackboard
$(function () {
    blackboardHub.client.updateChalk = function (model) {
        serverModel = model;
        print();
    };
    $.connection.hub.start().done(function () {
        // Join the Lesson's Blackboard
        blackboardHub.server.joinGroup(lessonId);

        paint();
        // Start the client side server update interval
        //setInterval(updateServerModel, updateRate);
    });
});

// Scaling function
// Based on Choose scale by MarkE
//http://stackoverflow.com/questions/19672426/chose-scale-in-coordinate-system

function remap(value, actualMin, actualMax, newMin, newMax) {
    return(newMin + (newMax - newMin) * (value - actualMin) / (actualMax - actualMin));
}


// Canvas Functions
// Users Paint function

// Based on the Drawable HTML 5 Canvas created by Lokar
//http://stackoverflow.com/questions/10122553/create-a-realistic-pencil-tool-for-a-painting-app-with-html5-canvas

var canvas = document.getElementById("canvas"),
    ctx = canvas.getContext("2d"),
    painting = false,
    lX = 0,
    lY = 0,
    lineThickness = 3;

var blackboardWidth = $('#blackBoard').width();
var blackboardHeight = $('#blackBoard').height();
var rect = canvas.getBoundingClientRect();

canvas.width = blackboardWidth;
canvas.height = blackboardHeight;

var useImg = false,
    imgData = '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';

function clearBoard() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    if (useImg == true) {
        var img = new Image()
        img.src = "data:image/png;base64," + imgData;
        img.onload = function () {
            ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
            $('#chalkColor').val('#000000');
            clientModel.color = '#000';
        };
    } else {
        ctx.fillStyle = '#000';
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        $('#chalkColor').val('#ffffff');
        clientModel.color = '#fff';
    }
}

clearBoard();
canvas.onmousedown = function (e) {
    painting = true;
    lX = e.offsetX; //e.pageX - rect.left - window.scrollX;
    lY = e.offsetY //e.pageY - rect.top - window.scrollY;
    paint(lX, lY, lX - 1, lY - 1, clientModel.lineWidth, clientModel.color);
    clientModel.oX = remap(lX, 0, blackboardWidth, 0, 600);
    clientModel.oY = remap(lY, 0, blackboardHeight, 0, 600);
    clientModel.nX = remap(lX - 1, 0, blackboardWidth, 0, 600);
    clientModel.nY = remap(lY - 1, 0, blackboardHeight, 0, 600);
    blackboardHub.server.updateModel(clientModel)
};

canvas.onmouseup = function (e) {
    painting = false;
}

canvas.onmousemove = function (e) {
    if (painting) {
        var x = e.offsetX // e.pageX - rect.left - window.scrollX;
        var y = e.offsetY //e.pageY - rect.top - window.scrollY;
        paint(lX, lY, x, y, clientModel.lineWidth, clientModel.color);
        clientModel.oX = remap(lX, 0, blackboardWidth, 0, 600);
        clientModel.oY = remap(lY, 0, blackboardHeight, 0, 600);
        clientModel.nX = remap(x, 0, blackboardWidth, 0, 600);
        clientModel.nY = remap(y, 0, blackboardHeight, 0, 600);
        blackboardHub.server.updateModel(clientModel)

        lX = x;
        lY = y;
    }
}

function paint(oX, oY, nX, nY, lW, sS) {
    ctx.beginPath();
    ctx.moveTo(oX, oY);
    ctx.lineWidth = lW;
    ctx.strokeStyle = sS;
    ctx.lineTo(nX, nY);
    ctx.stroke();
}


// Server's Print function
function print() {
    var oX = remap(serverModel.oX, 0, 600, 0, blackboardWidth);
    var oY = remap(serverModel.oY, 0, 600, 0, blackboardHeight);
    var nX = remap(serverModel.nX, 0, 600, 0, blackboardWidth);
    var nY = remap(serverModel.nY, 0, 600, 0, blackboardHeight);

    paint(oX, oY, nX, nY, serverModel.lineWidth, serverModel.color);
}
// end of Canvas function

// Change Chalk color
function changeColor(color) {
    clientModel.color = color;
}
// end of change chalk color

// Change Line Width
function changeLW(lW) {
    clientModel.lineWidth = lW;
}
// end of change Line Width

// Save image function
function saveImg(lessonId) {
    var img2SaveRaw = canvas.toDataURL('image/png'),
        img2SaveArray = img2SaveRaw.split(','),
        img2Save = img2SaveArray[1],
        title = $('#saveTitle').val();
    $('#imgSaveFail').addClass('hidden');
    $('#imgSaveSuccess').addClass('hidden');


    // For muckup purpose
    if (title == '') {
        $('#imgSaveFail').removeClass('hidden');
    } else {

        $.ajax({
            type: "POST", // Type of request
            url: "../api/lessons/upload", //The controller/Action
            dataType: "json",
            data: {
                "lessonid": lessonId,
                "title": title,
                "data": img2Save,
            },

            success: function (data) {
                console.log("save succesful");
                $('#imgSaveSuccess').removeClass('hidden');
            },

            error: function (err) {
                console.log("error[" + err.status + "]: " + err.statusText);
                $('#imgSaveFail').removeClass('hidden');
            }
        })
    }
}
// End of Save image function

// Load image function
function loadImg(id) {
    useImg = true;

    $.ajax({
        type: "POST", // Type of request
        url: "../api/lessons/getattachment", //The controller/Action
        dataType: "json",
        data: {
            "attachmentid": id
        },

        success: function (data) {
            console.log("get image stream succesful");
            imgData = data
            clearBoard();
        },

        error: function (err) {
            console.log("error[" + err.status + "]: " + err.statusText);
//            $('#imgSaveFail').removeClass('hidden');
        }
    })
}
// End of Save image function

// Default Board (blank) function
function defaultBoard() {
    useImg = false;
    data = '';
    clearBoard();
}
// End of Default Board