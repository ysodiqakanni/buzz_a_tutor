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
        x: 0,
        y: 0,
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
    lastX = 0,
    lastY = 0,
    lineThickness = 1;

var blackboardWidth = $('#blackBoard').width();
var blackboardHeight = $('#blackBoard').height();

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
    ctx.fillStyle = clientModel.color;
    lastX = e.pageX - this.offsetLeft;
    lastY = e.pageY - this.offsetTop;
};

canvas.onmouseup = function (e) {
    painting = false;
}

canvas.onmousemove = function (e) {
    if (painting) {
        var cursorX = e.pageX - this.offsetLeft;
        var cursorY = e.pageY - this.offsetTop;
        paint(cursorX, cursorY);

        //debugging
        //console.log("color: " + shapeModel.color)

        // Sends Chalks x y and color to server.
        var adjustedWidth;
        var adjustedHeight;

        clientModel.x = remap(cursorX, 0, blackboardWidth, 0, 600);
        clientModel.y = remap(cursorY, 0, blackboardHeight, 0, 600);
        blackboardHub.server.updateModel(clientModel);

    }
}

function paint(x,y) {
    var mouseX = x;
    var mouseY = y;

    //debugging
    //console.log("Paint - X: " + mouseX + ", Y: " + mouseY)

    // find all points between        
    var x1 = mouseX,
        x2 = lastX,
        y1 = mouseY,
        y2 = lastY;


    var steep = (Math.abs(y2 - y1) > Math.abs(x2 - x1));
    if (steep) {
        var x = x1;
        x1 = y1;
        y1 = x;

        var y = y2;
        y2 = x2;
        x2 = y;
    }
    if (x1 > x2) {
        var x = x1;
        x1 = x2;
        x2 = x;

        var y = y1;
        y1 = y2;
        y2 = y;
    }

    var dx = x2 - x1,
        dy = Math.abs(y2 - y1),
        error = 0,
        de = dy / dx,
        yStep = -1,
        y = y1;

    if (y1 < y2) {
        yStep = 1;
    }

    lineThickness = 5 - Math.sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1)) / 10;
    if (lineThickness < 1) {
        lineThickness = 1;
    }

    for (var x = x1; x < x2; x++) {
        if (steep) {
            ctx.fillRect(y, x, lineThickness, lineThickness);
        } else {
            ctx.fillRect(x, y, lineThickness, lineThickness);
        }

        error += de;
        if (error >= 0.5) {
            y += yStep;
            error -= 1.0;
        }
    }

    lastX = mouseX;
    lastY = mouseY;
}


// Server's Print function
function print() {
    ctx.fillStyle = serverModel.color;

    var adjustWidth = remap(serverModel.x, 0, 600, 0, blackboardWidth);
    var adjustHeight = remap(serverModel.y, 0, 600, 0, blackboardHeight);
    ctx.fillRect(adjustWidth, adjustHeight, 4, 4);
}
// end of Canvas function

// Change Chalk color
function changeColor(color) {
    clientModel.color = color;
    //switch (color) {
    //    case 'White':
    //        chalkModel.color = '#fff';
    //        break;
    //    case 'Blue':
    //        chalkModel.color = '#0026ff';
    //        break;
    //    case 'Red':
    //        chalkModel.color = '#ff0000';
    //        break;
    //    case 'Green':
    //        chalkModel.color = '#4cff00';
    //        break;
    //}
}
// end of change chalk color

function saveImg() {
    var img2SaveRaw = canvas.toDataURL('image/png'),
        img2SaveArray = img2SaveRaw.split(','),
        img2Save = img2SaveArray[1];

    // For muckup purpose
    var title = prompt("Title:", "Please supply a title");

    $.ajax({
        type: "POST", // Type of request
        url: "../api/lessons/upload", //The controller/Action
        dataType: "json",
        data: {
            "Title": title,
            "Data": img2Save,
        },

        success: function (data) {
            console.log("save succesful");
            $('#imgSaveIcon').removeClass('hidden');
        },

        error: function (err) {
            console.log("error[" + err.status + "]: " + err.statusText);
        }
    })
}

function loadImg(data) {
    useImg = true;
    imgData = data;
    
    clearBoard();
}

function defaultBoard() {
    useImg = false;
    data = '';

    clearBoard();

}