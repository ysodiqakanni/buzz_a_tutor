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

var load = false;
$('a[href="#blackboard-Tab"]').on('shown.bs.tab', function (e) {
    if(load != true) {
        blackboardWidth = $('#blackBoard').width();
        canvas.width = blackboardWidth;
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        load = true;
    }
})

var rect = canvas.getBoundingClientRect();


var useImg = false,
    imgData = '';

canvas.width = blackboardWidth;
canvas.height = blackboardHeight;
var previewCanvas = document.getElementById("previewCanvas");

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
    listModel = {
        group: lessonId,
        update: true
    },
    imageModel = {
        group: lessonId,
        clear: true,
        imageId: ""
    }
moved = false;

// Update Blackboard
$(function () {
    blackboardHub.client.updateChalk = function (model) {
        serverModel = model;
        print();
    };
    blackboardHub.client.boardImage = function (model) {
        imageModel = model;
        if (imageModel.clear == true) {
            useImg = false;
            clearBoard();
        } else {
            useImg = true;
            loadBackground(model.imageId);
        }
    };
    blackboardHub.client.updateList = function (model) {
        update = model;
        console.log(update.update);
        if (update.update == true) {
            updateImageList(lessonId);
        }
    }
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
    return (newMin + (newMax - newMin) * (value - actualMin) / (actualMax - actualMin));
}

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
            imageModel.clear = false;
            imageModel.imageId = id;
            blackboardHub.server.boardImage(imageModel);
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

// Load image function
function loadBackground(id) {
    $.ajax({
        type: "POST", // Type of request
        url: "../api/lessons/getattachment", //The controller/Action
        dataType: "json",
        data: {
            "attachmentid": id
        },
        success: function (data) {
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
    imageModel.clear = true;
    blackboardHub.server.boardImage(imageModel).done(function () {
       clearBoard();
    })
}
// End of Default Board

// Save Preview canvas
function saveCanvas() {
    var img2SaveRaw = canvas.toDataURL('image/png')
    var context = previewCanvas.getContext('2d');
    previewCanvas.width = blackboardWidth;
    previewCanvas.height = blackboardHeight;
    var img = new Image();
    img.src = img2SaveRaw;
    img.onload = function () {
        context.drawImage(img, 0, 0, previewCanvas.width, previewCanvas.height);
    };
    $('#previewTitle').val('Blackboard');
    $('#previewModal').modal();
}
// End of Save Preview Canvas

function uploadCanvas() {
    var img2SaveRaw = previewCanvas.toDataURL('image/png'),
        img2SaveArray = img2SaveRaw.split(','),
        img2Save = img2SaveArray[1],
        title = $('#previewTitle').val();

    if (title == '') {
        $('#imgSaveFail').removeClass('hidden');
    } else {
        $.ajax({
            type: "POST", // Type of request
            url: "../api/lessons/uploadtocloud", //The controller/Action
            dataType: "json",
            data: {
                "lessonid": lessonId,
                "title": title,
                "data": img2Save,
            },
            success: function (data) {
                // update list
            },

            error: function (err) {
                console.log("error[" + err.status + "]: " + err.statusText);
                $('#imgSaveFail').removeClass('hidden');
            }
        })
    }
}

// Load image from cloud function
function loadCloudImg(id) {
    //show spinner
    $("#loading-spinner").removeClass("hidden");

    useImg = true;
    $.ajax({
        type: "POST", // Type of request
        url: "../api/lessons/downloadfromcloud", //The controller/Action
        dataType: "json",
        data: {
            "attachmentid": id
        },
        success: function (data) {
            //console.log(data);
            //var image = "data:image/png;base64," + data;
            //window.open(image)
            imageModel.clear = false;
            imageModel.imageId = id;
            blackboardHub.server.boardImage(imageModel);
            imgData = data
            //end spinner
            $("#loading-spinner").addClass("hidden");
            clearBoard();

        },
        error: function (err) {
            console.log("error[" + err.status + "]: " + err.statusText);
            //            $('#imgSaveFail').removeClass('hidden');
        }
    })
}
// End of Load image from cloud function

// update list
function updateImageList(lessonId) {
    $.ajax({
        type: "POST", // Type of request
        url: "../api/lessons/getbblist", //"../api/lessons/upload", //The controller/Action
        dataType: "json",
        data: {
            "lessonid": lessonId,
        },
        success: function (data) {
            $("#bbImage-list").empty();
            $(jQuery.parseJSON(data)).each(function () {
                var id = this.id;
                var title = this.title;
                var attachmentLink = '<tr><td><button class="btn btn-link btn-block" onclick="loadCloudImg(' + id + ')">' + title + '</button></td></tr>'
                $("#bbImage-list").append(attachmentLink);
                $('#bbImage-list').slimScroll({
                    height: '500px'
                });
            });
        },
        error: function (err) {
            errorMessage = "Something went wrong, try again later.";
            $("#bbImageError").append(errorStart + errorMessage + errorEnd);
            console.log("error[" + err.status + "]: " + err.statusText);
        }
    })
}