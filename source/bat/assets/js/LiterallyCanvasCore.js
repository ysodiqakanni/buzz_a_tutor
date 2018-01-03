
var blackboardHub = $.connection.blackboardHub;
var buzzCanvas;
var previewCanvas;
var onInit = true;
var allUsers = [];
var isHost = IsHost;
var options;
var isHaveControl;
var btnStatus = "Grant";
var selectedUserId;
var useImg = false,
    imgData = '';

var selectedResourceId = 0;

var zoomPercent = 1.0;

var timeIntervalToAutoSaveCanvas = 300000; // Time interval is 5 minutes default 300000
var defaultTitle = "WhiteBoardImage_" + lessonId; // This title is used when canvas save
var isFromAutoSave = false;

var imageModel = {
    group: lessonId,
    clear: true,
    imageId: ""
}

var ListUpdate = {
    group: lessonId,
    update: false,
}

//Users Tool
var MyTool = function (lc) {  // take lc as constructor arg
    var self = this;

    return {
        usesSimpleAPI: false,  // DO NOT FORGET THIS!!!
        name: 'User List',
        iconName: 'user',
        strokeWidth: lc.opts.defaultStrokeWidth,
        optionsStyle: 'userList',

        didBecomeActive: function (lc) {
        },
        willBecomeInactive: function (lc) {
        }
    }
};

//User list at bottom tool
LC.defineOptionsStyle("userList", React.createClass({
    displayName: 'userList',
    getInitialState: function () {
        blackboardHub.server.fetchOnlineUsers(lessonId);
        return {

        };
    },
    handleChangeUser: function (event) {
        var selectedUserId = event.target.selectedOptions[0].value;
        var result = $.grep(allUsers, function (e) { return e.UserId == selectedUserId; });
        if (result.length == 0) {
            // not found
        } else if (result.length == 1) {
            // access the foo property using result[0].foo
            var user = result[0];
            $("#hdnConnId").val(user.ConnectionId);
            $("#hdnUserId").val(user.UserId);
            if (user.IsHaveControl == "true") {
                $("#btnAction").val("Revoke");
            }
            else {
                $("#btnAction").val("Grant");
            }
        } else {
            // multiple items found
        }
    },
    RefreshList: function (event) {

    },
    AssignHandle: function (event) {
        var connectionId = $("#hdnConnId").val();
        var selectedUserId = $("#hdnUserId").val();
        var result = $.grep(allUsers, function (e) { return e.UserId == selectedUserId; });
        var user = result[0];
        if (user != undefined) {
            if (user.Status == "Disconnected") {
                swal({
                    title: "Error!",
                    text: user.UserName + " is offline now.",
                    type: "warning",
                    timer: 3000
                });
            }
            else {
                if (event.currentTarget.value == "Grant")
                    GiveControl(connectionId);
                else
                    RevokeControl(connectionId);
            }
        }
    },
    render: function () {
        var br, div, input, label, lc, optgroup, option, ref4, select, span, ul;
        lc = this.props.lc;
        ref4 = React.DOM, div = ref4.div, input = ref4.input, select = ref4.select, option = ref4.option, br = ref4.br, label = ref4.label, span = ref4.span, optgroup = ref4.optgroup, button = ref4.button, ul = ref4.ul;
        return div({
            className: 'user-List',
            id: "connected-users"
        }, select({
            className: "col-lg-3 dropdown",
            id: "users",
            onChange: this.handleChangeUser,
            onFocus: this.RefreshList,
        }, allUsers.map((function (_this) {
            return function (arg) {
                var user;
                user = arg;
                if (user.IsHost == "false") {
                    $("#hdnConnId").val(allUsers[0].ConnectionId);
                    $("#hdnUserId").val(allUsers[0].UserId);
                    btnStatus = allUsers[0].IsHaveControl == "true" ? "Revoke" : "Grant";
                    return option({
                        value: user.UserId,
                        key: user.UserId
                    }, user.UserName);
                }
            };
        })(this))), span({}, input({
            type: 'button',
            id: 'btnAction',
            value: btnStatus,
            className: "btn btn-primary btn-sm",
            optionsStyle: "float:right",
            onClick: this.AssignHandle,
        })));
    }
}));

//Blank tool for students
var NoTool = function (lc) {  // take lc as constructor arg
    var self = this;
    return {
        usesSimpleAPI: false,  // DO NOT FORGET THIS!!!
        name: 'NoTool',
        didBecomeActive: function (lc) {
        },
        willBecomeInactive: function (lc) {
        }
    }
};

//SignalR client side events
$(function () {
    blackboardHub.client.refreshList = function (connectedUsers) {
        allUsers = [];
        $("ul[id=attendees]").empty();
        $("#attendees").append("<span style='font-weight:bold'>Attendees</span>");
        $.each(connectedUsers, function (i, user) {
            if (user.IsHost != "true") {
                allUsers.push(user);
                $("#attendees").append("<li><i class='fa fa-user'></i> <span>" + user.UserName + "</span></li>");
            }
        });
        if ($("#connected-users")[0] != undefined) {
            $("#users").empty();
            $.each(allUsers, function (index, user) {
                if (user.IsHost != "true") {
                    $('#users').append($('<option>', {
                        value: user.UserId,
                        text: user.UserName,
                    }));
                }
            });
            SetInitialValue();
        }
    };

    //For fetch online users and display them at left side pannel
    blackboardHub.client.fetchOnlineUsers = function (connectedUsers) {
        allUsers = [];
        $("ul[id=attendees]").empty();
        $("#attendees").append("<span style='font-weight:bold'>Attendees</span>");
        $.each(connectedUsers, function (i, user) {
            if (user.IsHost != "true") {
                allUsers.push(user);
                $("#attendees").append("<li><i class='fa fa-user'></i> <span>" + user.UserName + "</span></li>");
            }
        });
        if ($("#connected-users")[0] != undefined) {
            $("#users").empty();
            $.each(allUsers, function (index, user) {
                if (user.IsHost != "true") {
                    $('#users').append($('<option>', {
                        value: user.UserId,
                        text: user.UserName,
                    }));
                }
            });
            SetInitialValue();
        }
    };

    blackboardHub.client.getTeacherSnapshot = function (userConnectionId) {
        var snaps = buzzCanvas.getSnapshot();
        blackboardHub.server.uploadSnapshotOnInit(JSON.stringify(snaps), userConnectionId, lessonId);
    };
    blackboardHub.client.loadOnInitWithSnapShot = function (snapshotString) {
        options = {
            imageURLPrefix: '../assets/img/lc-images',
            snapshot: JSON.parse(snapshotString),
            toolbarPosition: 'hidden',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30],
            tools: [NoTool]
        };
        isHaveControl = "false";
        InitCanvas(options, isHaveControl);
    };

    blackboardHub.client.loadOnInitWithoutSnapShot = function () {
        options = {
            imageURLPrefix: '../assets/img/lc-images',
            toolbarPosition: 'hidden',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30],
            tools: [NoTool]
        };
        isHaveControl = "false";
        InitCanvas(options, isHaveControl);
    };

    blackboardHub.client.loadSnapShot = function (snapshotString) {
        buzzCanvas.loadSnapshot(JSON.parse(snapshotString));
    };

    blackboardHub.client.loadShape = function (shapeString, previousShapeId) {
        var shapes = LC.snapshotToShapes(buzzCanvas.getSnapshot());
        var shapeToRender = LC.JSONToShape(JSON.parse(shapeString));
        buzzCanvas.saveShape(shapeToRender, false, previousShapeId);
    };

    blackboardHub.client.clearBoard = function () {
        if (isHaveControl == "true") {
            return false;
        }
        else {
            buzzCanvas.clear();
        }
    };

    blackboardHub.client.undoAction = function () {
        if (isHaveControl == "true") {
            return false;
        }
        else {
            buzzCanvas.undo();
        }
    };

    blackboardHub.client.redoAction = function () {
        if (isHaveControl == "true") {
            return false;
        }
        else {
            buzzCanvas.redo();
        }
    };

    blackboardHub.client.panAction = function (coordsString) {
        if (isHaveControl == "true") {
            return false;
        }
        else {
            var coordsObj = JSON.parse(coordsString);
            buzzCanvas.setPan(coordsObj.x, coordsObj.y);
        }
    };

    blackboardHub.client.colorChange = function (colorType, colorValue) {
        if (isHaveControl == "true") {
            return false;
        }
        else {
            buzzCanvas.setColor(colorType, colorValue);
        }
    };

    blackboardHub.client.assignHandle = function (snapshotString) {
        options = {
            imageURLPrefix: '../assets/img/lc-images',
            snapshot: JSON.parse(snapshotString),
            secondaryColor: 'transparent',
            toolbarPosition: 'bottom',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30],
            tools: [LC.tools.Pencil, LC.tools.Eraser, LC.tools.Line, LC.tools.Ellipse, LC.tools.Rectangle, LC.tools.Text, LC.tools.Pan]
        };
        isHaveControl = "true";
        InitCanvas(options, isHaveControl);


    };

    blackboardHub.client.revertControl = function (connectedUsers) {
        var snapshotString = buzzCanvas.getSnapshot();
        var selectedUser = returnSelectedUser(connectedUsers);
        options = {
            imageURLPrefix: '../assets/img/lc-images',
            snapshot: snapshotString,
            toolbarPosition: 'bottom',
            defaultStrokeWidth: 2,
            secondaryColor: 'transparent',
            strokeWidths: [1, 2, 3, 5, 30],
            tools: [LC.tools.Pencil, LC.tools.Eraser, LC.tools.Line, LC.tools.Ellipse, LC.tools.Rectangle, LC.tools.Text, LC.tools.Pan, MyTool],//, SaveWhiteboardTool, DownloadWhiteboardTool
        };
        isHaveControl = "true";
        InitCanvas(options, isHaveControl);
        if (selectedUser != undefined) {
            setSelectedOption(selectedUser);
        }
    };

    blackboardHub.client.removeHandle = function (snapshotString) {
        options = {
            imageURLPrefix: '../assets/img/lc-images',
            snapshot: JSON.parse(snapshotString),
            toolbarPosition: 'hidden',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30],
            tools: [NoTool]
        };
        isHaveControl = "false";
        InitCanvas(options, isHaveControl);

    };

    blackboardHub.client.updateList = function (model) {
        update = model;
        if (update.update === true) {
            updateImageList(lessonId);
        }
    }

    blackboardHub.client.boardImage = function (model) {
        imageModel = model;
        if (imageModel.clear === true) {
            useImg = false;
            clearOrLoadBoard();
        } else {
            useImg = true;
            loadBackground(model.imageId);
        }
    };

    blackboardHub.client.removeEvents = function (snapshotString, connectedUsers) {
        var selectedUser = returnSelectedUser(connectedUsers);
        options = {
            imageURLPrefix: '../assets/img/lc-images',
            snapshot: JSON.parse(snapshotString),
            toolbarPosition: 'bottom',
            defaultStrokeWidth: 2,
            secondaryColor: 'transparent',
            strokeWidths: [1, 2, 3, 5, 30],
            tools: [MyTool],
        };
        isHaveControl = "false";
        InitCanvas(options, isHaveControl);
        if (selectedUser != undefined) {
            setSelectedOption(selectedUser);
        }
    }

    blackboardHub.client.assignEvents = function (snapshotString, connectedUsers) {
        var selectedUser = returnSelectedUser(connectedUsers);
        options = {
            imageURLPrefix: '../assets/img/lc-images',
            snapshot: JSON.parse(snapshotString),
            toolbarPosition: 'bottom',
            defaultStrokeWidth: 2,
            secondaryColor: 'transparent',
            strokeWidths: [1, 2, 3, 5, 30],
            tools: [LC.tools.Pencil, LC.tools.Eraser, LC.tools.Line, LC.tools.Ellipse, LC.tools.Rectangle, LC.tools.Text, LC.tools.Pan, MyTool],//, SaveWhiteboardTool, DownloadWhiteboardTool
        };
        isHaveControl = "true";
        InitCanvas(options, isHaveControl);
        if (selectedUser != undefined) {
            setSelectedOption(selectedUser);
        }
    }

});

function returnSelectedUser(connectedUsers) {
    allUsers = [];
    $.each(connectedUsers, function (i, user) {
        if (user.IsHost != "true") {
            allUsers.push(user);
        }
    });
    var selectedUser;
    var selectedUserId = $("#hdnUserId").val();
    if (selectedUserId != undefined && selectedUserId != "") {
        if (allUsers.length > 0) {
            var result = $.grep(allUsers, function (e) { return e.UserId == selectedUserId; });
            selectedUser = result[0];
        }
    }
    return selectedUser;
}

function setSelectedOption(selectedUser) {
    $("#hdnConnId").val(selectedUser.ConnectionId);
    $("#hdnUserId").val(selectedUser.UserId);
    $('#users option[value="' + selectedUser.UserId + '"]').attr('selected', 'selected');
    if (selectedUser.IsHaveControl == "true") {
        $("#btnAction").val("Revoke");
    }
    else {
        $("#btnAction").val("Grant");
    }
}

function RefreshUserList(referenceContainer, referenceRow, appendRowTo, connectedUsers, activeUserId) {
    $.each(connectedUsers, function (i, k) {
        if (connectedUsers[i].UserId != activeUserId) {
            var access = "Grant";
            if (connectedUsers[i].IsHaveControl == "true") {
                access = "Revoke";
            }
            if (connectedUsers[i].Status == "Online" && connectedUsers[i].GroupName == lessonId)
                PopulateActiveUsers("hidden-row-container", "row_", "connected-users", connectedUsers[i].UserId, connectedUsers[i].UserName, connectedUsers[i].ConnectionId, access);
        }
    });
}

//document ready event
$(document).ready(function () {
    $.connection.hub.start().done(function () {
        $("#lesssonDetailedDescrition").html(decodeURIComponent($("#lesssonDetailedDescrition").html().replace(/\+/g, ' ')));
        blackboardHub.server.joinGroup(lessonId, id, username, isHost, IsHaveControl);
        if ($(window).width() > 768) {
            $("#chat_window_1").draggable();
            $("#innerChatDiv").resizable({
                helper: "ui-resizable-helper",
                grid: [10, 10]
            });
            $("#innerChatDiv").on("resizestop", function (event, ui) {
                $(".panel-body").css("height", ui.size.height - 50)
                $(".panel-body").css("width", ui.size.width)
                $(".chatBody").css("height", ui.size.height - 107)
                $(".chatBody").css("width", ui.size.width - 30)
            });
        }

        if (isHost === "true") {
            options = {
                imageURLPrefix: '../assets/img/lc-images',
                toolbarPosition: 'bottom',
                defaultStrokeWidth: 2,
                secondaryColor: 'transparent',
                strokeWidths: [1, 2, 3, 5, 30],
                tools: [LC.tools.Pencil, LC.tools.Eraser, LC.tools.Line, LC.tools.Ellipse, LC.tools.Rectangle, LC.tools.Text, LC.tools.Pan, MyTool],//, SaveWhiteboardTool, DownloadWhiteboardTool
            };
            isHaveControl = "true";
            InitCanvas(options, isHaveControl);
            if (onInit) {
                onInit = false;
                var snaps = buzzCanvas.getSnapshot();
                blackboardHub.server.uploadSnapshotOnInit(JSON.stringify(snaps), lessonId);
            }
            $("#btnSaveWhiteboard").show();
            $("#btnDownlaodWhiteboard").show();
            $("#btnUploadWhiteboard").show();

            //fetch an image assosiated with this lesson
            updateImageList(lessonId);

            //execute this method on a specific time to auto save whiteboard
            saveCanvasOnTimeInterval(timeIntervalToAutoSaveCanvas);
        }
        else {
            blackboardHub.server.getTeacherSnapshot(lessonId);
            $("#btnSaveWhiteboard").hide();
            $("#btnDownlaodWhiteboard").hide();
            $("#btnUploadWhiteboard").hide();
        }
        blackboardHub.server.fetchOnlineUsers(lessonId);
    });
    // Lost connection with Server
    $.connection.hub.reconnecting(function () {
        console.log("reconnecting hub for literally canvas");
    });
    $.connection.hub.disconnected(function () {
        setTimeout(function () {
            $.connection.hub.start();
        }, 5000); // Re-start connection after 5 seconds
    });
});

//This method is called on specific time interval. Default is 5 minutes.
function saveCanvasOnTimeInterval(milliseconds) {
    setInterval(function () {
        AutoSaveWhiteboard();
    }, milliseconds);
}

//This method is used to capture screenshot of canvas and save it to db.
function AutoSaveWhiteboard() {
    if (buzzCanvas.getSnapshot().shapes.length > 0) {
        isFromAutoSave = true;
        var img2SaveRaw = LC.renderSnapshotToImage(buzzCanvas.getSnapshot(), null).toDataURL('image/png'),
            img2SaveArray = img2SaveRaw.split(','),
            img2Save = img2SaveArray[1],
            imgExtension = img2SaveArray[0];
        imgData = img2Save;
        $('#previewTitle').val(defaultTitle);
        uploadCanvas();
    }
}

//Initialise canvas 
function InitCanvas(options, isHost) {
    if (buzzCanvas != undefined)
        buzzCanvas.teardown();
    buzzCanvas = LC.init(document.getElementById("lc"), options);
    setCanvasSize();
    setZIndexForToolBar();
    if (isHaveControl == "true") {
        bindEvent();
    }

}

//Render canvas with aproprite size depending on device
function setCanvasSize() {
    $("#teacher").css("height", $("#video-wrap").height() - $("#shop").height());
    $("#streamBoxTeacher").css("height", $("#video-wrap").height() - $("#shop").height());
    var windowHeight = parseFloat($(window).height());
    var whiteboardHeight;
    var multiplier = getMultiplier(windowHeight);
    whiteboardHeight = windowHeight * (parseFloat(multiplier) / 100);
    if (isHost == "false" && isHaveControl == "false") {
        if (role === 2) {
            $(".literally.toolbar-at-bottom").css("min-height", whiteboardHeight);
        } else {
            $(".literally.toolbar-hidden").css("min-height", whiteboardHeight);
        }
        $(".literally .lc-drawing.with-gui").addClass("custom");
    }
    else {
        $(".literally.toolbar-at-bottom").css("min-height", whiteboardHeight);
    }
    buzzCanvas.respondToSizeChange();

}

//get percent of canvas size depending on device
function getMultiplier(height) {
    var multiplier = 50;
    switch (height) {
        case 1414:
            multiplier = 70;
            break;
        case 707:
            multiplier = 45;
            break;
        case 744:
            multiplier = 45;
            break;
        default:
            multiplier = 45;
            break;
    }
    return multiplier;
}

//Bind signalR event to canvas
function bindEvent() {
    buzzCanvas.on("shapeSave", function (shape, previousShapeId) {
        var shapeString = LC.shapeToJSON(shape.shape);
        blackboardHub.server.uploadShape(JSON.stringify(shapeString), previousShapeId, lessonId);
    });
    buzzCanvas.on("clear", function () {
        blackboardHub.server.clearBoard(lessonId);
    });
    buzzCanvas.on("pan", function (coords) {
        blackboardHub.server.panAction(JSON.stringify(coords), lessonId);
    });
    buzzCanvas.on("undo", function () {
        blackboardHub.server.undoAction(lessonId);
    });
    buzzCanvas.on("redo", function () {
        blackboardHub.server.redoAction(lessonId);
    });
    buzzCanvas.on("primaryColorChange", function (newColor) {
        blackboardHub.server.colorChange("primary", newColor, lessonId);
    });
}

function uploadSnapShot() {
    var snaps = buzzCanvas.getSnapshot();
    blackboardHub.server.uploadSnapshot(JSON.stringify(snaps), lessonId);
}

//Give whiteboard control to particular student
function GiveControl(connectionId) {
    isHaveControl = "false";
    btnStatus = "Revoke";
    var snaps = buzzCanvas.getSnapshot();
    blackboardHub.server.assignHandle(lessonId, connectionId, JSON.stringify(snaps));
}

//Revert back whiteboard control from student
function RevokeControl(connectionId) {
    isHaveControl = "true";
    btnStatus = "Grant";
    var snaps = buzzCanvas.getSnapshot();
    blackboardHub.server.removeHandle(lessonId, connectionId, JSON.stringify(snaps));
}

//Set initial values for teacher when logged in and display whiteboard access button according
function SetInitialValue() {
    var selectedUserConnId = $("#hdnConnId").val();
    var selectedUserId = $("#hdnUserId").val();
    if (selectedUserConnId != undefined && selectedUserConnId != "") {
        var result = $.grep(allUsers, function (e) { return e.UserId == selectedUserId; });
        var user = result[0];
        if (user != undefined) {
            $("#hdnConnId").val(user.ConnectionId);
            $("#hdnUserId").val(user.UserId);
            $('#users option[value="' + user.UserId + '"]').attr('selected', 'selected');
            $("#btnAction").prop('disabled', false);
            if (user.IsHaveControl == "true") {
                $("#btnAction").val("Revoke");
            }
            else {
                $("#btnAction").val("Grant");
            }
        }
    }
    else {
        if (allUsers.length > 0) {
            $("#btnAction").prop('disabled', false);
            $("#hdnConnId").val(allUsers[0].ConnectionId);
            $("#hdnUserId").val(allUsers[0].UserId);
            $('#users option[value="' + allUsers[0].UserId + '"]');
        } else {
            $("#btnAction").prop('disabled', true);
        }
    }
}

//Zoom In whiteboard
function zoomIn() {
    zoomPercent = parseFloat(zoomPercent.toFixed(2));
    zoomPercent += 0.2;
    if (zoomPercent == 4.0) {
        $("#btnZoomIn").prop("disabled", true);
    }
    else {
        $("#btnZoomOut").prop("disabled", false);
    }
    buzzCanvas.setZoom(zoomPercent);
}

//Zoom out whiteboard
function zoomOut() {
    zoomPercent = parseFloat(zoomPercent.toFixed(2));
    zoomPercent -= 0.2;
    if (zoomPercent == 0.2) {
        $("#btnZoomOut").prop("disabled", true);
    }
    else {
        $("#btnZoomIn").prop("disabled", false);
    }
    buzzCanvas.setZoom(zoomPercent);
}

//Save whiteboard
function SaveWhiteBoard() {
    if (buzzCanvas.getSnapshot().shapes.length > 0) {
        var img2SaveRaw = LC.renderSnapshotToImage(buzzCanvas.getSnapshot(), null).toDataURL('image/png'),
            img2SaveArray = img2SaveRaw.split(','),
            img2Save = img2SaveArray[1],
            imgExtension = img2SaveArray[0];
        imgData = img2Save;
        $('#pdfContainer').css("height", buzzCanvas.canvas.clientHeight);
        $('#previewCanvas').css("height", buzzCanvas.canvas.clientWidth);
        $('#previewCanvas').css("width", buzzCanvas.canvas.clientWidth);
        $('#previewCanvas').css("background-image", "url('" + imgExtension + "," + img2Save + "')");
        $('#previewCanvas').css("background-repeat", "no-repeat");
        $('#previewTitle').val('White-Board');
        $('#previewModal').modal();
    } else {
        swal({
            title: "Error!",
            text: "No drawings to save!!.",
            type: "warning",
            timer: 3000
        });
    }
}

function uploadCanvas() {
    $("#modal-button-container").empty();
    $("#modal-button-container").append(uploadingIcon);
    var title = $('#previewTitle').val();
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
                "data": imgData,
            },
            success: function (data) {
                imgData = "";
                updateImageList(lessonId);
                updateSavedTime("autoSavedWhitboard");
                if (isFromAutoSave) {
                    isFromAutoSave = false;
                } else {
                    $('#previewModal').modal('toggle');
                }
            },
            error: function (err) {
                console.log("error[" + err.status + "]: " + err.statusText);
                $('#imgSaveFail').removeClass('hidden');
            }
        })
    }
}

// This method is used to display a last auto saved time.
function updateSavedTime(containerId) {
    var now = new Date();
    var hh = now.getHours();
    var min = now.getMinutes();

    var ampm = (hh >= 12) ? 'pm' : 'am';
    hh = hh % 12;
    hh = hh ? hh : 12;
    hh = hh < 10 ? '0' + hh : hh;
    min = min < 10 ? '0' + min : min;

    var time = hh + " : " + min + " " + ampm;
    $("#" + containerId).text("");
    $("#" + containerId).text(time);
}

function loadCloudImg(id) {
    id = selectedResourceId;
    if (id != 0) {
        useImg = true;
        $("#modal-button-container1").empty();
        $("#modal-button-container1").append(uploadingIcon);
        $.ajax({
            type: "POST", // Type of request
            url: "../api/lessons/downloadfromcloud", //The controller/Action
            dataType: "json",
            data: {
                "attachmentid": id
            },
            success: function (data) {
                imageModel.clear = false;
                imageModel.imageId = id;
                blackboardHub.server.boardImage(imageModel);
                imgData = data;
                clearOrLoadBoard();
                $("#modal-button-container1").empty();
            },
            error: function (err) {
                console.log("error[" + err.status + "]: " + err.statusText);
            }
        });
    } else {
        swal({
            title: "Error!",
            text: "No drawings to download!!.",
            type: "warning",
            timer: 3000
        });
    }
}

/////////////////////////Start section Events for pdf//////////////////

//move to first page
$("a.FirstPage").click(function () {
    toggleButton("a.FirstPage", true);
    toggleButton("a.LastPage", false);
    toggleButton("a.NextPage", false);
    toggleButton("a.PrevPage", true);
    $("#canvasCarousel div.item.active").removeClass("active");
    $("#canvasCarousel div.item").first().addClass("active");
});

//move to last page
$("a.LastPage").click(function () {
    toggleButton("a.FirstPage", false);
    toggleButton("a.LastPage", true);
    toggleButton("a.NextPage", true);
    toggleButton("a.PrevPage", false);
    $("#canvasCarousel div.item.active").removeClass("active");
    $("#canvasCarousel div.item").last().addClass("active");
});

//move to next page
$("a.NextPage").click(function () {
    var $toHighlight = $('.active').next().length > 0 ? $('.active').next() : $('#lessonThree li').first();
    if ($toHighlight.length != 0) {
        $('.active').removeClass('active');
        $toHighlight.addClass('active');
        $("a.PrevPage").removeClass("disabled");
        $("a.FirstPage").removeClass("disabled");
    } else {
        $("a.NextPage").addClass("disabled");
        $("a.LastPage").addClass("disabled");
    }
});

//move to prev page
$("a.PrevPage").click(function () {
    var $toHighlight = $('.active').prev().length > 0 ? $('.active').prev() : $('#lessonThree li').last();
    if ($toHighlight.length != 0) {
        $('.active').removeClass('active');
        $toHighlight.addClass('active');
        $("a.NextPage").removeClass("disabled");
        $("a.LastPage").removeClass("disabled");
    } else {
        $("a.PrevPage").addClass("disabled");
        $("a.FirstPage").addClass("disabled");
    }
});

//disable/enable button
function toggleButton(button, status) {
    if (status) {
        $(button).addClass("disabled");
    } else {
        $(button).removeClass("disabled");
    }
}

//////////////////////////////close section////////////////////////////////

function loadBackground(id) {
    $.ajax({
        type: "POST", // Type of request
        url: "../api/lessons/downloadfromcloud",//"../api/lessons/getattachment", //The controller/Action
        dataType: "json",
        data: {
            "attachmentid": id
        },
        success: function (data) {
            imgData = data;
            clearOrLoadBoard();
        },
        error: function (err) {
            console.log("error[" + err.status + "]: " + err.statusText);
        }
    });
}

function updateImageList(lessonId) {
    $.ajax({
        type: "POST", // Type of request
        url: "../api/lessons/getbblist", //"../api/lessons/upload", //The controller/Action
        dataType: "json",
        data: {
            "lessonid": lessonId
        },
        success: function (data) {
            var resource = JSON.parse(data);
            if (resource.id != null) {
                selectedResourceId = resource.id;
                $("#btnDownlaodWhiteboard").bind("click", loadCloudImg);
            } else {
                selectedResourceId = 0;
                $("#btnDownlaodWhiteboard").bind("click", loadCloudImg);
            }
        },
        error: function (err) {
            errorMessage = "Something went wrong, try again later.";
            $("#bbImageError").append(errorStart + errorMessage + errorEnd);
            console.log("error[" + err.status + "]: " + err.statusText);
        }
    });
}

function clearOrLoadBoard() {
    if (useImg === true) {
        if (buzzCanvas != undefined) {
            var selectedTool = new LC.tools.Pan();
            buzzCanvas.clear();
            var img = new Image();
            img.src = "data:image/png;base64," + imgData;
            var scale = 1;
            if ($(window).width() > 768)
                scale = 1;
            else
                scale = 0.3;
            buzzCanvas.saveShape(LC.createShape('Image', { x: 10, y: 10, image: img, scale: scale }));
            buzzCanvas.setTool(selectedTool);
            if ($(".lc-pick-tool")[6] != undefined)
                $(".lc-pick-tool")[6].click();
        }
    }
}

//This function is used for upload image to canvas from upload model
function loadWhiteBoardWithImage() {
    if (useImg === true) {
        if (buzzCanvas != undefined) {
            var selectedTool = new LC.tools.Pan();
            buzzCanvas.clear();
            var img = new Image();
            img.src = imgData;
            var scale = 1;
            if ($(window).width() > 768)
                scale = 1;
            else
                scale = 0.3;
            buzzCanvas.saveShape(LC.createShape('Image', { x: 10, y: 10, image: img, scale: scale }));
            buzzCanvas.setTool(selectedTool);
            if ($(".lc-pick-tool")[6] != undefined)
                $(".lc-pick-tool")[6].click();
        }
    }
}

//Chat box events
$(document).on('click', '.panel-heading span.icon_minim', function (e) {
    var $this = $(this);
    var windowHeight = $(window).height();
    if (!$this.hasClass('panel-collapsed')) {
        if ($(window).width() > 768) {
            $("#chat_window_1").draggable('disable');
            $("#innerChatDiv").resizable('disable');
            $("#chat_window_1").css({ 'top': windowHeight - 50, 'left': '0', 'width': '30%' });
            $("#innerChatDiv").css({ 'width': '100%' })
            $(".chatBody").css({ 'width': '100%' })
            $(".panel-body").css({ "width": '100%' })
        }
        $this.parents('.panel').find('.panel-body').slideUp();
        $this.addClass('panel-collapsed');
        $this.removeClass('glyphicon-minus').addClass('glyphicon-fullscreen');
    } else {
        if ($(window).width() > 768) {
            $("#chat_window_1").css({ 'top': windowHeight - 403, 'left': '0' })
            $("#innerChatDiv").css({ 'height': '402px' })
            $(".chatBody").css({ 'height': '300px' })
            $(".panel-body").css({ "height": '355px' })
            $("#chat_window_1").draggable('enable');
            $("#innerChatDiv").resizable('enable');
        }
        $this.parents('.panel').find('.panel-body').slideDown();
        $this.removeClass('panel-collapsed');
        $this.removeClass('glyphicon-fullscreen').addClass('glyphicon-minus');
    }
});
$(document).on('focus', '.panel-footer input.chat_input', function (e) {
    var $this = $(this);
    if ($('#minim_chat_window').hasClass('panel-collapsed')) {
        $this.parents('.panel').find('.panel-body').slideDown();
        $('#minim_chat_window').removeClass('panel-collapsed');
        $('#minim_chat_window').removeClass('glyphicon-fullscreen').addClass('glyphicon-minus');
    }
});
$(document).on('click', '#new_chat', function (e) {
    var size = $(".chat-window:last-child").css("margin-left");
    size_total = parseInt(size) + 400;
    alert(size_total);
    var clone = $("#chat_window_1").clone().appendTo(".container");
    clone.css("margin-left", size_total);
});
$(document).on('click', '.icon_close', function (e) {
    $("#chat_window_1").remove();
});

function setZIndexForToolBar() {
    if ($(window).width() <= 425) {
        $(".literally .lc-picker").css("z-index", "0");
    }
    else {
        console.log("big screens")
        $(".literally .lc-picker").css("z-index", "1001");
    }
}
