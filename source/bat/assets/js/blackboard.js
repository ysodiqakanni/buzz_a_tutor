// Set up connection the Blackboard Hub
var blackboardHub = $.connection.blackboardHub,
    $bb = $('#canvas'),
    //// Send a maximum of 10 messages per second
    //// (mouse movements trigger a lot of messages)
    //messageFrequency = 1000,
    //// Determine how often to send messages in
    //// time to abide by the messageFrequency
    //updateRate = 10 / messageFrequency,
    chalkModel = {
        x: 0,
        y: 0,
        color: '#fff',
        group: lessonId,
    },
    moved = false;

// Update Blackboard
$(function () {
    blackboardHub.client.updateChalk = function (model) {
        chalkModel = model;
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
ctx.fillRect(0, 0, canvas.width, canvas.height);

canvas.onmousedown = function (e) {
    painting = true;
    ctx.fillStyle = chalkModel.color;
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

        chalkModel.x = remap(cursorX, 0, blackboardWidth, 0, 600);
        chalkModel.y = remap(cursorY, 0, blackboardHeight, 0, 600);
        blackboardHub.server.updateModel(chalkModel);

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
    ctx.fillStyle = chalkModel.color;

    var adjustWidth = remap(chalkModel.x, 0, 600, 0, blackboardWidth);
    var adjustHeight = remap(chalkModel.y, 0, 600, 0, blackboardHeight);
    ctx.fillRect(adjustWidth, adjustHeight, 4, 4);
}
// end of Canvas function

// Change Chalk color
function changeColor(color) {
    switch (color) {
        case 'White':
            chalkModel.color = '#fff';
            break;
        case 'Blue':
            chalkModel.color = '#0026ff';
            break;
        case 'Red':
            chalkModel.color = '#ff0000';
            break;
        case 'Green':
            chalkModel.color = '#4cff00';
            break;
    }
}
// end of change chalk color


