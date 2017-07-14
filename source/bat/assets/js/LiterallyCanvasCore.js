
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

var zoomPercent = 1.0;

var unsubscribeClearEvent;
var unsubscribePanEvent;
var unsubscribeUndoEvent;
var unsubscribeRedoEvent;
var unsubscribePrimaryColorEvent;
var unsubscribeZoomEvent;
var unsubscribeEvent;

var imageModel = {
    group: lessonId,
    clear: true,
    imageId: ""
}

var ListUpdate = {
    group: lessonId,
    update: false,
}

var MyTool = function (lc) {  // take lc as constructor arg
    var self = this;

    return {
        usesSimpleAPI: false,  // DO NOT FORGET THIS!!!
        name: 'User List',
        iconName: 'user',
        strokeWidth: lc.opts.defaultStrokeWidth,
        optionsStyle: 'userList',

        didBecomeActive: function (lc) {
            //console.log("activeted");
        },

        willBecomeInactive: function (lc) {
            //console.log("activeted sonn");
        }
    }
};

var NoTool = function (lc) {  // take lc as constructor arg
    var self = this;

    return {
        usesSimpleAPI: false,  // DO NOT FORGET THIS!!!
        name: 'NoTool',
        didBecomeActive: function (lc) {
            //console.log("activeted");
        },

        willBecomeInactive: function (lc) {
            //console.log("activeted sonn");
        }
    }
};

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

function replicateOtherStudent(studentId, studentName) {
    return "<div id='otherBox-" + studentId + "' class='oc-item'><div id='otherCon-" + studentId + "' class='streamBody'><div id='other-" + studentId + "' class='streamWindow' style='width: 100%; height: 111px'><div id='streamBoxOther-" + studentId + "'></div></div></div><div class='name4'>" + studentName + "</div></div>";
}

function replicateSelfStudent(studentId, studentName) {
    return "<div id='selfBox-" + studentId + "' class='oc-item'><div id='self' class='streamWindow' style='width: 125px; height: 111px'><div id='streamBoxSelf'></div></div><div class='name4'>" + studentName + "</div></div>";
}

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
        blackboardHub.server.fetchOnlineUsersExceptTeacher(lessonId);
        //createGridStructureForAllUsers(connectedUsers);
    };

    blackboardHub.client.fetchOnlineUsersExceptTeacher = function (connectedUsers) {//, userId, userFirstName
        createGridStructureForAllUsers(connectedUsers);//, userId, userFirstName
    };

    blackboardHub.client.fetchUserList = function (connectedUsers, userId) {
        //$.each(connectedUsers, function (index, user) {
        //$("#teacher").css("height", $("#video-wrap").height() - 111);
        //$("#streamBoxTeacher").css("height", $("#video-wrap").height() - 111);
        //if (userId == user.UserId) {
        //    if ($("#otherBox-" + user.UserId + "").length == 0) {
        //        $('.owl-carousel').trigger('add.owl.carousel', [replicateOtherStudent(user.UserId, user.UserName.split(" ")[0])]).trigger('refresh.owl.carousel');
        //    }
        //} 
        var gridString = "";
        debugger;
        //$("#shop").html(gridString);
        var flag = false;
        for (var i = 0; i < connectedUsers.length; i++) {
            if (connectedUsers[i].UserId == userId) {
                if ($("#otherBox-" + connectedUsers[i].UserId + "").length == 0) {
                    if (i == 0) {
                        flag = false;
                        gridString += "<div class='row'>";
                        gridString += "<div class='col-lg-3 col-sm-3 col-md-3'>" + replicateOtherStudent(connectedUsers[i].UserId, connectedUsers[i].UserName.split(" ")[0]) + "</div>";
                    }
                    else if (i != 0 && i % 4 == 0) {
                        flag = true;
                        gridString += "</div>";
                    }
                    else {
                        flag = false;
                        gridString += "<div class='col-lg-3 col-sm-3 col-md-3'>" + replicateOtherStudent(connectedUsers[i].UserId, connectedUsers[i].UserName.split(" ")[0]) + "</div>";
                    }
                }
            }
        }
        if (flag) {
            gridString += "</div>";
        }
        debugger;
        $("#shop").html(gridString);

        //});
    };

    blackboardHub.client.onInitRenderStream = function (streamUsers) {
        //$.each(streamUsers, function (index, user) {
        //    $("#teacher").css("height", $("#video-wrap").height() - 111);
        //    $("#streamBoxTeacher").css("height", $("#video-wrap").height() - 111);
        //    if ($("#otherBox-" + user.UserId + "").length == 0) {
        //        $('.owl-carousel').trigger('add.owl.carousel', [replicateOtherStudent(user.UserId, user.UserName.split(" ")[0])]).trigger('refresh.owl.carousel');
        //    }
        //});
        var gridString = "";
        $("#shop").html(gridString);
        var flag = false;
        for (var i = 0; i < streamUsers.length; i++) {
            if (i == 0) {
                flag = false;
                gridString += "<div class='row'>";
                gridString += "<div class='col-lg-3 col-sm-3 col-md-3'>" + replicateOtherStudent(streamUsers[i].UserId, streamUsers[i].UserName.split(" ")[0]) + "</div>";
            }
            else if (i != 0 && i % 4 == 0) {
                flag = true;
                gridString += "</div>";
            }
            else {
                flag = false;
                gridString += "<div class='col-lg-3 col-sm-3 col-md-3'>" + replicateOtherStudent(connectedUsers[i].UserId, connectedUsers[i].UserName.split(" ")[0]) + "</div>";
            }
        }
        if (flag) {
            gridString += "</div>";
        }
        debugger;
        $("#shop").html(gridString);
    };

    blackboardHub.client.fetchUserListOnDisconnect = function (connectedUsers, userId) {
        var itemCount = $('.owl-item').length;
        var elemIndex = -1;
        if (itemCount > 0) {
            var itemArr = $('.owl-item').children();
            $.each(itemArr, function (index, value) {
                var id = value.id.split("-")[1];
                if (id == userId) {
                    elemIndex = index;
                }
            });
        }
        if (elemIndex != -1) {
            $(".owl-carousel").trigger('remove.owl.carousel', [elemIndex]);
        }
        if ($('.owl-item').length == 0) {
            $("#teacher").css("height", $("#video-wrap").height() - $("#shop").height());
            $("#streamBoxTeacher").css("height", $("#video-wrap").height() - $("#shop").height());
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
        //buzzCanvas.clear();
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

$(document).ready(function () {
    $.connection.hub.start().done(function () {
        $("#lesssonDetailedDescrition").html(decodeURIComponent($("#lesssonDetailedDescrition").html().replace(/\+/g, ' ')));
        blackboardHub.server.joinGroup(lessonId, id, username, isHost, IsHaveControl);



        //EndStreamingStudent();
        //blackboardHub.server.addToStreamStudents(sessionId, token, id, lessonId, username, "false", isHost);
        //blackboardHub.server.onInitRenderStream(lessonId);
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
        }
        else {
            blackboardHub.server.getTeacherSnapshot(lessonId);
            $("#btnSaveWhiteboard").hide();
            $("#btnDownlaodWhiteboard").hide();
            $("#btnUploadWhiteboard").hide();
        }
        blackboardHub.server.fetchOnlineUsers(lessonId);
        //blackboardHub.server.fetchOnlineUsersExceptTeacher(lessonId, id, userFirstName);
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



function InitCanvas(options, isHost) {
    if (buzzCanvas != undefined)
        buzzCanvas.teardown();
    buzzCanvas = LC.init(document.getElementById("lc"), options);
    if (isHost == "false" && isHaveControl == "false") {
        if (role === 2) {
            $(".literally.toolbar-at-bottom").css("min-height", "400px");
        } else {
            $(".literally.toolbar-hidden").css("min-height", "400px");
        }
        $(".literally .lc-drawing.with-gui").addClass("custom");
        buzzCanvas.respondToSizeChange();
    }
    else {
        $(".literally.toolbar-at-bottom").css("min-height", "400px");
    }
    $("#teacher").css("height", $("#video-wrap").height() - $("#shop").height());
    $("#streamBoxTeacher").css("height", $("#video-wrap").height() - $("#shop").height());
    if (isHaveControl == "true") {
        bindEvent();
    }
}

function bindEvent() {
    unsubscribeEvent = buzzCanvas.on("shapeSave", function (shape, previousShapeId) {
        var shapeString = LC.shapeToJSON(shape.shape);
        blackboardHub.server.uploadShape(JSON.stringify(shapeString), previousShapeId, lessonId);
    });
    unsubscribeClearEvent = buzzCanvas.on("clear", function () {
        blackboardHub.server.clearBoard(lessonId);
    });
    unsubscribePanEvent = buzzCanvas.on("pan", function (coords) {
        blackboardHub.server.panAction(JSON.stringify(coords), lessonId);
    });
    unsubscribeUndoEvent = buzzCanvas.on("undo", function () {
        blackboardHub.server.undoAction(lessonId);
    });
    unsubscribeRedoEvent = buzzCanvas.on("redo", function () {
        blackboardHub.server.redoAction(lessonId);
    });
    unsubscribePrimaryColorEvent = buzzCanvas.on("primaryColorChange", function (newColor) {
        blackboardHub.server.colorChange("primary", newColor, lessonId);
    });
}

function uploadSnapShot() {
    var snaps = buzzCanvas.getSnapshot();
    blackboardHub.server.uploadSnapshot(JSON.stringify(snaps), lessonId);
}

function GiveControl(connectionId) {
    isHaveControl = "false";
    btnStatus = "Revoke";
    var snaps = buzzCanvas.getSnapshot();
    blackboardHub.server.assignHandle(lessonId, connectionId, JSON.stringify(snaps));
}
function RevokeControl(connectionId) {
    isHaveControl = "true";
    btnStatus = "Grant";
    var snaps = buzzCanvas.getSnapshot();
    blackboardHub.server.removeHandle(lessonId, connectionId, JSON.stringify(snaps));
}
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

var SaveWhiteboardTool = function (lc) {  // take lc as constructor arg
    var self = this;

    return {
        usesSimpleAPI: false,  // DO NOT FORGET THIS!!!
        name: 'Save',
        iconName: 'save',
        strokeWidth: lc.opts.defaultStrokeWidth,

        didBecomeActive: function (lc) {
            var img2SaveRaw = LC.renderSnapshotToImage(buzzCanvas.getSnapshot(), null).toDataURL('image/png'),
                              img2SaveArray = img2SaveRaw.split(','),
                              img2Save = img2SaveArray[1],
                              imgExtension = img2SaveArray[0];
            imgData = img2Save;
            $('#previewCanvas').css("height", buzzCanvas.canvas.clientHeight);
            $('#previewCanvas').css("width", buzzCanvas.canvas.clientWidth);
            $('#previewCanvas').css("background-image", "url('" + imgExtension + "," + img2Save + "')");
            $('#previewCanvas').css("background-repeat", "no-repeat");
            $('#previewTitle').val('White-Board');
            $('#previewModal').modal();
        },

        willBecomeInactive: function (lc) {
        }
    }
};

function SaveWhiteBoard() {
    if (buzzCanvas.getSnapshot().shapes.length > 0) {
        var img2SaveRaw = LC.renderSnapshotToImage(buzzCanvas.getSnapshot(), null).toDataURL('image/png'),
                                  img2SaveArray = img2SaveRaw.split(','),
                                  img2Save = img2SaveArray[1],
                                  imgExtension = img2SaveArray[0];
        imgData = img2Save;
        $('#previewCanvas').css("height", buzzCanvas.canvas.clientHeight);
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

var DownloadWhiteboardTool = function (lc) {  // take lc as constructor arg
    var self = this;

    return {
        usesSimpleAPI: false,  // DO NOT FORGET THIS!!!
        name: 'Download',
        iconName: 'download',
        strokeWidth: lc.opts.defaultStrokeWidth,

        didBecomeActive: function (lc) {
            $('#BBListModal').modal();
        },

        willBecomeInactive: function (lc) {

        }
    }
};

function DownloadWhiteboard() {
    $('#BBListModal').modal();
}

function uploadCanvas() {
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
                ListUpdate.update = true;
                blackboardHub.server.updateList(ListUpdate);
                $('#previewModal').modal('toggle');
            },
            error: function (err) {
                console.log("error[" + err.status + "]: " + err.statusText);
                $('#imgSaveFail').removeClass('hidden');
            }
        })
    }
}

function loadCloudImg(id) {
    useImg = true;
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
            $('#BBListModal').modal('toggle');
        },
        error: function (err) {
            console.log("error[" + err.status + "]: " + err.statusText);
        }
    });
}

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
            $("#bbImage-list").empty();
            $(jQuery.parseJSON(data)).each(function () {
                var id = this.id;
                var title = this.title;
                var attachmentLink =
                    '<div style="overflow: auto;"><button class="btn btn-link pull-left" onclick="loadCloudImg(' + id + ')">' + title + '</button>' +
                    '<a href="' + deleteResourceUrl + '/?resourceId=' + id + '" class="pull-right" style="padding: 10px;">' +
                    '<i class="fa fa-remove"></i>' +
                    '</a>' +
                    '</div>';
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
    });
}

function clearOrLoadBoard() {
    if (useImg === true) {
        if (buzzCanvas != undefined) {
            buzzCanvas.clear();
            var img = new Image();
            img.src = "data:image/png;base64," + imgData;
            buzzCanvas.saveShape(LC.createShape('Image', { x: 10, y: 10, image: img }));
        }
    }
}

$(document).on('click', '.panel-heading span.icon_minim', function (e) {
    var $this = $(this);
    if (!$this.hasClass('panel-collapsed')) {
        $this.parents('.panel').find('.panel-body').slideUp();
        $this.addClass('panel-collapsed');
        $this.removeClass('glyphicon-minus').addClass('glyphicon-fullscreen');
    } else {
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
    //$(this).parent().parent().parent().parent().remove();
    $("#chat_window_1").remove();
});


function createGridStructureForAllUsers(connectedUsers) {//, userId, userName
    var gridString = "";
    var flag = false;
    if (role == '1') {
        var checkForDiv = $("#selfBox-" + id);
        if (checkForDiv.length < 1) {
            gridString += "<div class='row'>";
            gridString += "<div class='col-lg-2 col-sm-2 col-md-2'>" + replicateSelfStudent(id, userFirstName) + "</div>";
        } else {
            gridString += "<div class='row'>";
            gridString += "<div class='col-lg-2 col-sm-2 col-md-2'>" + $("#selfBox-" + id)[0].outerHTML + "</div>";
        }
    }
    for (var i = 0; i < connectedUsers.length; i++) {
        if (i == 0) {
            flag = false;
            if (parseInt(connectedUsers[i].UserId) == id) {
               
            } else {
                var checkForDiv = $("#otherBox-" + id);
                if (checkForDiv.length < 1) {
                    gridString += "<div class='col-lg-2 col-sm-2 col-md-2'>" + replicateOtherStudent(connectedUsers[i].UserId, connectedUsers[i].UserName.split(" ")[0]) + "</div>";
                } else {
                    gridString += "<div class='col-lg-2 col-sm-2 col-md-2'>" + $("#otherBox-" + id)[0].outerHTML + "</div>";
                }
            }
        }
        else if (i != 0 && i % 4 == 0) {
            flag = true;
            gridString += "</div>";
        }
        else {
            flag = false;
            if (parseInt(connectedUsers[i].UserId) == id) {
                
            } else {
                var checkForDiv = $("#otherBox-" + id);
                if (checkForDiv.length < 1) {
                    gridString += "<div class='col-lg-2 col-sm-2 col-md-2'>" + replicateOtherStudent(connectedUsers[i].UserId, connectedUsers[i].UserName.split(" ")[0]) + "</div>";
                }
                else {
                    gridString += "<div class='col-lg-2 col-sm-2 col-md-2'>" + $("#otherBox-" + id)[0].outerHTML + "</div>";
                }
            }
        }
    }
    if (flag) {
        gridString += "</div>";
    }
    $("#shop").html(gridString);
}
