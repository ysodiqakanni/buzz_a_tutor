"use strict";
// Scaling function
// Based on Choose scale by MarkE
//http://stackoverflow.com/questions/19672426/chose-scale-in-coordinate-system

// Canvas Functions
// Based on the Drawable HTML 5 Canvas created by Lokar
//http://stackoverflow.com/questions/10122553/create-a-realistic-pencil-tool-for-a-painting-app-with-html5-canvas


var whiteBoard = {
    s: '',
    settings: {
        blackboardHub: $.connection.blackboardHub,
        canvasName : 'canvas',
        frameName : 'blackBoard',
        container : 'blackBoardCon',
        //frame: $('#whiteBoard'),
        defaultBtn: $('#defaultBtn'),
        redBtn: $('#redBtn'),
        blueBtn: $('#blueBtn'),
        greenBtn: $('#greenBtn'),

        painting: false,
        lastX: 0,
        lastY: 0,
        lineThickness: 1,
        chalkModel: {
            x: 0,
            y: 0,
            color: '#fff',
            group: lessonId
        },
        moved: false,
        useData: true,
        imgID : 0,
        imgData : ''
    },

    init: function () {
        this.s = this.settings;
        this.createElements();
        this.bindUIActions();
    },

    signalr: function () {
        var blackboardHubProxy = $.connection.blackboardHub;
        whiteBoard.s.blackboardHub.client.updateChalk = function (model) {
            whiteBoard.s.chalkModel = model;
            whiteBoard.print();
        };
        $.connection.hub.start().done(function () {
            console.log(whiteBoard.s);

            // Join the Lesson's Blackboard
            blackboardHubProxy.server.joinGroup(lessonId);
            whiteBoard.paint();
            // Start the client side server update interval
            //setInterval(updateServerModel, updateRate);
        });
    },

    createElements: function () {
        var frame = "<div id =" + this.s.frameName + "> </div>";
        $("#" + this.s.container).append(frame);

        this.s.frameW = $('#' + this.s.frameName).width(),
        this.s.frameH = $('#' + this.s.frameName).height();


        var canvas = "<canvas id=" + this.s.canvasName + " style='align-content:center' width=" + whiteBoard.s.frameW + " height=" + whiteBoard.s.frameH + "></canvas>";
        $("#" + this.s.frameName).append(canvas);
        this.s.canvas = document.getElementById(this.s.canvasName);
        this.s.btx = this.s.canvas.getContext('2d');
        console.log(whiteBoard.s);
        if (this.s.useData) {
            // Get Image
            var img = new Image();
            img.src = "data:image/png;base64," + whiteBoard.s.imgData;
            img.onload = function () {
                whiteBoard.s.btx.drawImage(img, 0, 0, 600, 600);
            }
        } else {
            this.s.btx.fillRect(0, 0, this.s.canvas.width, this.s.canvas.height);
        }
    },

    bindUIActions: function () {
        this.s.defaultBtn.on("click", function () {
            whiteBoard.changeColor('white');
        });
        this.s.redBtn.on("click", function () {
            whiteBoard.changeColor('red');
        });
        this.s.blueBtn.on("click", function () {
            whiteBoard.changeColor('blue');
        });
        this.s.greenBtn.on("click", function () {
            whiteBoard.changeColor('green');
        });

        this.s.canvas.onmousedown = function (e) {
            whiteBoard.s.painting = true;
            whiteBoard.s.btx.fillStyle = whiteBoard.s.chalkModel.color;
            whiteBoard.s.lastX = e.pageX - this.offsetLeft;
            whiteBoard.s.lastY = e.pageY - this.offsetTop;
        },

        this.s.canvas.onmouseup = function (e) {
            whiteBoard.s.painting = false;
        },

        this.s.canvas.onmousemove = function (e) {
            if (whiteBoard.s.painting) {
                var cursorX = e.pageX - this.offsetLeft;
                var cursorY = e.pageY - this.offsetTop;
                whiteBoard.paint(cursorX, cursorY);

                //debugging
                //console.log("color: " + shapeModel.color)

                // Sends Chalks x y and color to server.
                whiteBoard.s.chalkModel.x = whiteBoard.remap(cursorX, 0, whiteBoard.s.frameW, 0, 600);
                whiteBoard.s.chalkModel.y = whiteBoard.remap(cursorY, 0, whiteBoard.s.frameH, 0, 600);

                whiteBoard.s.blackboardHub.server.updateModel(whiteBoard.s.chalkModel);
            }
        }
    },

    remap: function (value, actualMin, actualMax, newMin, newMax) {
        return (newMin + (newMax - newMin) * (value - actualMin) / (actualMax - actualMin));
    },

    paint : function (x, y) {
        var mouseX = x;
        var mouseY = y;

        //debugging
        //console.log("Paint - X: " + mouseX + ", Y: " + mouseY)

        // find all points between   
        //console.log(mouseX, lastX, mouseY, lastY);

        var x1 = mouseX,
            x2 = this.s.lastX,
            y1 = mouseY,
            y2 = this.s.lastY;


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
                this.s.btx.fillRect(y, x, lineThickness, lineThickness);
            } else {
                this.s.btx.fillRect(x, y, lineThickness, lineThickness);
            }

            error += de;
            if (error >= 0.5) {
                y += yStep;
                error -= 1.0;
            }
        }

        this.s.lastX = mouseX;
        this.s.lastY = mouseY;
    } ,

    changeColor: function (color) {
        switch (color) {
            case 'white':
                whiteBoard.s.chalkModel.color = '#fff';
                break;
            case 'blue':
                whiteBoard.s.chalkModel.color = '#0026ff';
                break;
            case 'red':
                whiteBoard.s.chalkModel.color = '#ff0000';
                break;
            case 'green':
                whiteBoard.s.chalkModel.color = '#4cff00';
                break;
        }
    },

    print: function () {
        whiteBoard.s.btx.fillStyle = whiteBoard.s.chalkModel.color;

        var adjustWidth = whiteBoard.remap(whiteBoard.s.chalkModel.x, 0, 600, 0, whiteBoard.s.frameW);
        var adjustHeight = whiteBoard.remap(whiteBoard.s.chalkModel.y, 0, 600, 0, whiteBoard.s.frameH);
        whiteBoard.s.btx.fillRect(adjustWidth, adjustHeight, 4, 4);
    },

    send: function () {
        var img = canvas.toDataURL('image/png');

        // For muckup purpose
        var title = prompt("Title:", "Please supply a title");

        $.Ajax({
            type: "POST", // Type of request
            url: "lessons/upload", //The controller/Action
            dataType: "json",
            data: {
                "Title": title,
                "Data": img
            },

            success: function(data) {
                console.log(data);
            },

            error: function(err) {
                console.log(err);
            }
        });

    }
}