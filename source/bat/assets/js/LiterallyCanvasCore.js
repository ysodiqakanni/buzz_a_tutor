
var blackboardHub = $.connection.blackboardHub;
var buzzCanvas;
var buzzCanvas;
var buzzCanvas;
var onInit = true;
var allUsers = [];
var isHost = IsHost;
var options;
var isHaveControl;
var btnStatus = "Grant";
var selectedUserId;

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
                //console.log("Mouse moved to", pt);
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
        //this.setState(selectedUserId);
    },
    RefreshList: function (event) {

    },
    AssignHandle: function (event) {
        var connectionId = $("#hdnConnId").val();
        if (event.currentTarget.value == "Grant")
            GiveControl(connectionId);
        else
            RevokeControl(connectionId);
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

$(function () {

    blackboardHub.client.refreshList = function (connectedUsers) {
        allUsers = [];
        $.each(connectedUsers, function (i, user) {
            if (user.IsHost != "true") {
                allUsers.push(user);
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
        blackboardHub.server.uploadSnapshotOnInit(JSON.stringify(snaps),userConnectionId, lessonId);
    };
    blackboardHub.client.loadOnInitWithSnapShot = function (snapshotString) {
        options = {
            imageURLPrefix: '../assets/img/lc-images',
            snapshot: JSON.parse(snapshotString),
            toolbarPosition: 'hidden',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30]
        };
        isHaveControl = "false";
        InitCanvas(options, isHaveControl);
    };

    blackboardHub.client.loadOnInitWithoutSnapShot = function () {
        options = {
            imageURLPrefix: '../assets/img/lc-images',
            toolbarPosition: 'hidden',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30]
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
            //if (colorType == "background") {
            //    if ($(".literally.toolbar-hidden") != undefined) {
            //        $(".literally.toolbar-hidden").css("background-color", colorValue);
            //        $(".lc-drawing.with-gui").css("background-color", "transparent");
            //    }
            //}
            //else
                buzzCanvas.setColor(colorType, colorValue);
        }
    };

    blackboardHub.client.zoomAction = function (zoomAmount) {
        if (isHaveControl == "true") {
            return false;
        }
        else {
            var coordsObj = JSON.parse(zoomAmount);
            buzzCanvas.setZoom(coordsObj.newScale);
        }
    };

    blackboardHub.client.assignHandle = function (snapshotString) {
        options = {
            imageURLPrefix: '../assets/img/lc-images',
            snapshot: JSON.parse(snapshotString),
            toolbarPosition: 'bottom',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30],
            tools: [LC.tools.Pencil, LC.tools.Eraser, LC.tools.Line, LC.tools.Ellipse, LC.tools.Rectangle, LC.tools.Text, LC.tools.Pan]
        };
        isHaveControl = "true";
        InitCanvas(options, isHaveControl);


    };
    blackboardHub.client.removeHandle = function (snapshotString) {
        options = {
            imageURLPrefix: '../assets/img/lc-images',
            snapshot: JSON.parse(snapshotString),
            toolbarPosition: 'hidden',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30]
        };
        isHaveControl = "false";
        InitCanvas(options, isHaveControl);

    };

});

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
        blackboardHub.server.joinGroup(lessonId, id, username, isHost, IsHaveControl);

        if (isHost === "true") {
            options = {
                imageURLPrefix: '../assets/img/lc-images',
                toolbarPosition: 'bottom',
                defaultStrokeWidth: 2,
                strokeWidths: [1, 2, 3, 5, 30],
                tools: [LC.tools.Pencil, LC.tools.Eraser, LC.tools.Line, LC.tools.Ellipse, LC.tools.Rectangle, LC.tools.Text, LC.tools.Pan, MyTool],
                //tools: LC.defaultTools.concat([MyTool])
            };
            isHaveControl = "true";
            InitCanvas(options, isHaveControl);
            if (onInit) {
                onInit = false;
                var snaps = buzzCanvas.getSnapshot();
                blackboardHub.server.uploadSnapshotOnInit(JSON.stringify(snaps), lessonId);
            }


        }
        else {
            resizeStudentCanvas();
            setTimeout(function () { blackboardHub.server.getTeacherSnapshot(lessonId); }, 3000);
        }

    });
});



function InitCanvas(options, isHost) {
    if (buzzCanvas != undefined)
        buzzCanvas.teardown();
    buzzCanvas = LC.init(document.getElementById("lc"), options);
    if (isHaveControl == "true") {
        var unsubscribe = buzzCanvas.on("shapeSave", function (shape, previousShapeId) {
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
        buzzCanvas.on("secondaryColorChange", function (newColor) {
            blackboardHub.server.colorChange("secondary", newColor, lessonId);
        });
        buzzCanvas.on("backgroundColorChange", function (newColor) {
            blackboardHub.server.colorChange("background", newColor, lessonId);
        });
        buzzCanvas.on("zoom", function (amount) {
            blackboardHub.server.zoomAction(JSON.stringify(amount), lessonId);
        });
    }
}

function uploadSnapShot() {
    var snaps = buzzCanvas.getSnapshot();
    blackboardHub.server.uploadSnapshot(JSON.stringify(snaps), lessonId);
}
function resizeTeacherCanvas() {
    if (isHost === "true") {
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
    if (selectedUserConnId != undefined && selectedUserConnId != "") {
        var result = $.grep(allUsers, function (e) { return e.ConnectionId == selectedUserConnId; });
        var user = result[0];
        $("#hdnConnId").val(user.ConnectionId);
        $('#users option[value="' + user.UserId + '"]').attr('selected', 'selected');
        if (user.IsHaveControl == "true") {
            $("#btnAction").val("Revoke");
        }
        else {
            $("#btnAction").val("Grant");
        }
    }
    else {
        $("#hdnConnId").val(allUsers[0].ConnectionId);
        $('#users option[value="' + allUsers[0].UserId + '"]');
    }
}