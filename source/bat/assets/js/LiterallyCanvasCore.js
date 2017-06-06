
var blackboardHub = $.connection.blackboardHub;
var teacherCanvas;
var studentCanvas;
var onInit = true;
var allUsers;

$(function () {

    blackboardHub.client.refreshList = function (connectedUsers) {
        allUsers = connectedUsers;
        if ($("#connected-users")[0] != undefined) {
            //const element = document.getElementById('users')
            //element.dispatchEvent(new Event('focus', { bubbles: true }))
            $("#users").empty();
            SetInitialValue();
            $.each(allUsers, function (index, user) {
                if (user.IsHost != "true")
                    $('#users').append($('<option>', {
                        value: user.UserId,
                        text: user.UserName,
                    }));
            });
        }
    };

    blackboardHub.client.getTeacherSnapshot = function () {
        var snaps = teacherCanvas.getSnapshot();
        blackboardHub.server.uploadSnapshotOnInit(JSON.stringify(snaps), lessonId);
    };
    blackboardHub.client.loadOnInitWithSnapShot = function (snapshotString) {
        studentCanvas = LC.init(document.getElementById("lc"), {
            imageURLPrefix: '../Content/lc-images',
            snapshot: JSON.parse(snapshotString),
            toolbarPosition: 'hidden',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30]
        });
    };

    blackboardHub.client.loadOnInitWithoutSnapShot = function () {
        studentCanvas = LC.init(document.getElementById("lc"), {
            imageURLPrefix: '../Content/lc-images',
            toolbarPosition: 'hidden',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30]
        });
    };

    blackboardHub.client.loadSnapShot = function (snapshotString) {
        studentCanvas.loadSnapshot(JSON.parse(snapshotString));
    };

    blackboardHub.client.loadShape = function (shapeString, previousShapeId) {
        var shapes = LC.snapshotToShapes(studentCanvas.getSnapshot());
        var shapeToRender = LC.JSONToShape(JSON.parse(shapeString));
        studentCanvas.saveShape(shapeToRender, true, previousShapeId);
    };

    blackboardHub.client.clearBoard = function () {
        studentCanvas.clear();
    };
    blackboardHub.client.undoAction = function () {
        studentCanvas.undo();
    };
    blackboardHub.client.redoAction = function () {
        studentCanvas.redo();
    };
    blackboardHub.client.panAction = function (coordsString) {
        var coordsObj = JSON.parse(coordsString);
        studentCanvas.setPan(coordsObj.x, coordsObj.y);
    };
    blackboardHub.client.colorChange = function (colorType, colorValue) {
        studentCanvas.setColor(colorType, colorValue);
    };

    blackboardHub.client.assignHandle = function (snapshotString) {
        studentCanvas.teardown();
        studentCanvas = LC.init(document.getElementById("lc"), {
            imageURLPrefix: '../assets/img/lc-images',
            snapshot: JSON.parse(snapshotString),
            toolbarPosition: 'bottom',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30]
        });
    };
    blackboardHub.client.removeHandle = function (snapshotString) {
        studentCanvas.teardown();
        studentCanvas = LC.init(document.getElementById("lc"), {
            imageURLPrefix: '../assets/img/lc-images',
            snapshot: JSON.parse(snapshotString),
            toolbarPosition: 'hidden',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30]
        });
    };
});

function LastDivCount(container) {
    var lastDiv = $('ul[id=' + container + ']').children().length;
    lastDiv += 1;
    return lastDiv;
}
function PopulateActiveUsers(referenceContainer, referenceRow, appendRowTo, activeUserId, activeUserName, userConnectionId, access) {
    var newRowCount = LastDivCount(appendRowTo);
    $("#" + appendRowTo).append('<li id="listitem_' + activeUserId + '"></li>');
    $('div[name=' + referenceContainer + ']').children().clone().appendTo("#listitem_" + activeUserId);
    $("#" + appendRowTo + " #" + referenceRow + "0").attr("id", referenceRow + newRowCount);
    $("#" + appendRowTo + " #" + referenceRow + newRowCount).find("#user").html(activeUserName);
    $("#" + appendRowTo + " #" + referenceRow + newRowCount).find("#user").addClass("LoggedInUsers");
    $("#" + appendRowTo + " #" + referenceRow + newRowCount).find('button').attr("id", "btn" + referenceRow + userConnectionId);
    $("#btn" + referenceRow + userConnectionId).html(access);
    $("#" + appendRowTo + " #" + referenceRow + newRowCount).find('button').on('click', function () {
        ToggleButton($(this).attr("id"), userConnectionId, access);
        return false;
    });
    $('#' + referenceRow + newRowCount).css("display", "block");
}
function ToggleButton(buttonId, userConnectionId, access) {
    if ($("#" + buttonId).html() == "Grant") {
        access = "Revoke";
        $("#" + buttonId).html("Revoke");
        GiveControl(userConnectionId);
    }
    else {
        access = "Grant";
        $("#" + buttonId).html("Grant");
        RevokeControl(userConnectionId);
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
        blackboardHub.server.joinGroup(lessonId, id, username, IsHost, IsHaveControl);
        var MyTool = function (lc) {  // take lc as constructor arg
            var self = this;

            return {
                usesSimpleAPI: false,  // DO NOT FORGET THIS!!!
                name: 'user',
                iconName: 'user',
                strokeWidth: lc.opts.defaultStrokeWidth,
                optionsStyle: 'userList',

                didBecomeActive: function (lc) {
                    var onPointerDown = function (pt) {
                        self.currentShape = LC.createShape('Line', {
                            x1: pt.x, y1: pt.y, x2: pt.x, y2: pt.y,
                            strokeWidth: self.strokeWidth, color: lc.getColor('primary')
                        });
                        lc.setShapesInProgress([self.currentShape]);
                        lc.repaintLayer('main');
                    };

                    var onPointerDrag = function (pt) {
                        self.currentShape.x2 = pt.x;
                        self.currentShape.y2 = pt.y;
                        lc.setShapesInProgress([self.currentShape]);
                        lc.repaintLayer('main');
                    };

                    var onPointerUp = function (pt) {
                        self.currentShape.x2 = pt.x;
                        self.currentShape.y2 = pt.y;
                        lc.setShapesInProgress([]);
                        lc.saveShape(self.currentShape);
                    };

                    var onPointerMove = function (pt) {
                        console.log("Mouse moved to", pt);
                    };

                    // lc.on() returns a function that unsubscribes us. capture it.
                    self.unsubscribeFuncs = [
                      lc.on('lc-pointerdown', onPointerDown),
                      lc.on('lc-pointerdrag', onPointerDrag),
                      lc.on('lc-pointerup', onPointerUp),
                      lc.on('lc-pointermove', onPointerMove)
                    ];
                },

                willBecomeInactive: function (lc) {
                    // call all the unsubscribe functions
                    self.unsubscribeFuncs.map(function (f) { f() });
                }
            }
        };

        LC.defineOptionsStyle("userList", React.createClass({
            displayName: 'userList',
            getInitialState: function () {
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
                //console.log(event);
                //$("#users").empty();
                //$.each(allUsers, function (index, user) {
                //    if (user.IsHost != "true")
                //    $('#users').append($('<option>', {
                //        value: user.UserId,
                //        text: user.UserName,
                //    }));
                //});
            },
            AssignHandle: function (event) {
                var connectionId = $("#hdnConnId").val();
                if (event.currentTarget.value == "Grant")
                    GiveControl(connectionId);
                else
                    RevokeControl(connectionId);
            },
            render: function () {
                var br, div, input, label, lc, optgroup, option, ref4, select, span,ul;
                lc = this.props.lc;
                ref4 = React.DOM, div = ref4.div, input = ref4.input, select = ref4.select, option = ref4.option, br = ref4.br, label = ref4.label, span = ref4.span, optgroup = ref4.optgroup,button = ref4.button,ul=ref4.ul;
                return div({
                    className: 'user-List',
                    id: "connected-users"
                    }, select({
                        className: "col-lg-3 dropdown",
                        id:"users",
                        onChange: this.handleChangeUser,
                        onFocus:this.RefreshList,
                    }, allUsers.map((function (_this) {
                        return function (arg) {
                            var user;
                            user = arg;
                            if (user.IsHost == "false") {
                                return option({
                                    value: user.UserId,
                                    key: user.UserId
                                }, user.UserName);
                            }
                        };
                    })(this))), span({},input({
                        type: 'button',
                        id: 'btnAction',
                        value: 'Grant',
                        className: "btn btn-primary",
                        optionsStyle: "float:right",
                        onClick : this.AssignHandle,
                    })));
           }
        }));
        if (IsHost === "true") {
            teacherCanvas = LC.init(document.getElementById("lc"), {
                imageURLPrefix: '../assets/img/lc-images',
                toolbarPosition: 'bottom',
                defaultStrokeWidth: 2,
                strokeWidths: [1, 2, 3, 5, 30],
                tools: LC.defaultTools.concat([MyTool])
            });
            if (onInit) {
                onInit = false;
                var snaps = teacherCanvas.getSnapshot();
                blackboardHub.server.uploadSnapshotOnInit(JSON.stringify(snaps), lessonId);
            }
            teacherCanvas.on("shapeSave", function (shape, previousShapeId) {
                var shapeString = LC.shapeToJSON(shape.shape);
                blackboardHub.server.uploadShape(JSON.stringify(shapeString), previousShapeId, lessonId);
            });
            teacherCanvas.on("clear", function () {
                blackboardHub.server.clearBoard(lessonId);
            });

            teacherCanvas.on("pan", function (coords) {
                blackboardHub.server.panAction(JSON.stringify(coords), lessonId);
            });

            teacherCanvas.on("undo", function () {
                blackboardHub.server.undoAction(lessonId);
            });
            teacherCanvas.on("redo", function () {
                blackboardHub.server.redoAction(lessonId);
            });
            teacherCanvas.on("primaryColorChange", function (newColor) {
                blackboardHub.server.ColorChange("primary", newColor, lessonId);
            });
            teacherCanvas.on("secondaryColorChange", function (newColor) {
                blackboardHub.server.ColorChange("secondary", newColor, lessonId);
            });
            teacherCanvas.on("backgroundColorChange", function (newColor) {
                blackboardHub.server.colorChange("background", newColor, lessonId);
            });            
        }
        else {
            resizeStudentCanvas();
            setTimeout(function () { blackboardHub.server.getTeacherSnapshot(lessonId); }, 3000);
        }
    });
});

function uploadSnapShot() {
    var snaps = teacherCanvas.getSnapshot();
    blackboardHub.server.uploadSnapshot(JSON.stringify(snaps), lessonId);
}
function resizeTeacherCanvas() {
    if (IsHost === "true") {
        setTimeout(function () {
            var elementToMatch = $(".with-gui")[0];
            var elementsToResize = $(".lc-drawing.with-gui canvas");
            var scale = 1;
            LC.util.matchElementSize(elementToMatch, elementsToResize, scale, callback = null);
        }, 1000);
    }
}

function resizeStudentCanvas() {
    var elementToMatch = $("#lc")[0];
    var elementsToResize = $(".lc-drawing.with-gui canvas");
    var scale = 1;
    LC.util.matchElementSize(elementToMatch, elementsToResize, scale, callback = null);
}

function GiveControl(connectionId) {
    var snaps = teacherCanvas.getSnapshot();
    blackboardHub.server.assignHandle(lessonId, connectionId, JSON.stringify(snaps));
}
function RevokeControl(connectionId) {
    var snaps = teacherCanvas.getSnapshot();
    blackboardHub.server.removeHandle(lessonId, connectionId, JSON.stringify(snaps));
}
function SetInitialValue() {
    var selectedUserConnId = $("#hdnConnId").val();
    if (selectedUserConnId != undefined) {
        var result = $.grep(allUsers, function (e) { return e.ConnectionId == selectedUserConnId; });
        var user = result[0];
        $("#hdnConnId").val(user.ConnectionId);
        if (user.IsHaveControl == "true") {
            $("#btnAction").val("Revoke");
        }
        else {
            $("#btnAction").val("Grant");
        }
    }
}