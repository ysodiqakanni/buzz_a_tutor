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

$("document").ready(function () {
    $(function () {
        $('#attachment-list').slimScroll({
            height: '250px'
        });
    });
    //Load Preview of upload item
    // Sets up input event on document load.
    $("#pdfData").change(function (event) {
        var file = event.target.files[0];
        var fileName = file.name;
        var fileExt = fileName.split('.').pop().toLowerCase();
        if (fileExt == "pdf") {
            //Step 2: Read the file using file reader
            var fileReader = new FileReader();
            fileReader.onload = function () {
                //Step 4:turn array buffer into typed array
                var typedarray = new Uint8Array(this.result);
                //Step 5:PDFJS should be able to read this
                PDFJS.getDocument(typedarray).then(function (pdf) {
                    pdf.getPage(1).then(function getPageHelloWorld(page) {
                        //
                        // Prepare canvas using PDF page dimensions
                        //
                        var scale = 0.8;
                        var context = previewCanvas.getContext('2d');
                        var viewport = page.getViewport(scale);
                        previewCanvas.width = viewport.width;
                        previewCanvas.height = viewport.height;
                        //
                        // Render PDF page into canvas context
                        //
                        var renderContext = {
                            canvasContext: context,
                            viewport: viewport
                        };
                        page.render(renderContext);
                        $('#previewTitle').val(fileName);
                        $('#previewModal').modal();
                    })
                });
            }
            //Step 3:Read the file as ArrayBuffer
            fileReader.readAsArrayBuffer(file);
        } else if (fileExt == "png" || fileExt == "jpg" || fileExt == "tif") {
            var fileReader = new FileReader();
            fileReader.addEventListener("load", function () {
                var context = previewCanvas.getContext('2d');
                previewCanvas.width = 568;
                previewCanvas.height = 600;
                var img = new Image();
                img.src = fileReader.result;
                img.onload = function () {
                    context.drawImage(img, 0, 0, canvas.width, canvas.height);
                };
                $('#previewTitle').val(fileName);
                $('#previewModal').modal();
            }, false);

            //Step 3:Read the file as ArrayBuffer
            fileReader.readAsDataURL(file);
        } else {
            //Not accepted format
        }
    });
});

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
                // update list
                $("#attachment-list").empty()
                $(jQuery.parseJSON(data)).each(function () {
                    var id = this.id;
                    var title = this.title;
                    var attachmentBtn = '<button class="btn btn-link btn-block" onclick="loadImg(' + id + ')">' + title + '</button>';
                    $("#attachment-list").append(attachmentBtn);
                });
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

// Save image function
function savePreview(lessonId) {
    var img2SaveRaw = previewCanvas.toDataURL('image/png'),
        img2SaveArray = img2SaveRaw.split(','),
        img2Save = img2SaveArray[1],
        title = $('#previewTitle').val();

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
            // update list
            $("#attachment-list").empty()
            $(jQuery.parseJSON(data)).each(function () {
                var id = this.id;
                var title = this.title;
                var attachmentBtn = '<button class="btn btn-link btn-block" onclick="loadImg(' + id + ')">' + title + '</button>';
                $("#attachment-list").append(attachmentBtn);
            });
            $('#previewModal').modal('toggle');
        },

        error: function (err) {
            console.log("error[" + err.status + "]: " + err.statusText);
        }
    })
}
// End of Save image function

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