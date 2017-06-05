
var blackboardHub = $.connection.blackboardHub;
var teacherCanvas;
var studentCanvas;
var onInit = true;
var allUsers;
    //[
    //    { name: "Nitin", id: "1" },
    //    { name: "Yogesh", id: "2" }
    //];

$(function () {

    blackboardHub.client.refreshList = function (connectedUsers) {
        $("#connected-users").empty();
        //$.each(connectedUsers, function (index, user) {
        //    if (user.IsHost != "true") {
        //        var userObj = {
        //            name: user.UserName,
        //            id: user.UserId
        //        }
        //        allUsers.push(userObj);
        //    }
        //});
        allUsers = connectedUsers;
        //RefreshUserList("hidden-row-container", "row_", "connected-users", connectedUsers, id);
    };

    blackboardHub.client.getTeacherSnapshot = function () {
        var snaps = teacherCanvas.getSnapshot();
        blackboardHub.server.uploadSnapshotOnInit(JSON.stringify(snaps), lessonId);
    };
    blackboardHub.client.loadSnapShotOnInit = function (snapshotString) {
        studentCanvas = LC.init(document.getElementById("lc"), {
            imageURLPrefix: '../Content/lc-images',
            snapshot: JSON.parse(snapshotString),
            toolbarPosition: 'hidden',
            defaultStrokeWidth: 2,
            strokeWidths: [1, 2, 3, 5, 30]
        });
    };

    blackboardHub.client.loadSnapShot = function (snapshotString) {
        studentCanvas.loadSnapshot(JSON.parse(snapshotString));
    };

    blackboardHub.client.loadShape = function (shapeString, previousShapeId) {

        //var snaps = teacherCanvas.getSnapshot();
        var shapes = LC.snapshotToShapes(studentCanvas.getSnapshot());
        $.each(shapes, function (index, shape) {
            console.log(shape);
        })
        var shapeToRender = LC.JSONToShape(JSON.parse(shapeString));
        studentCanvas.saveShape(shapeToRender, true, previousShapeId);
        //var shapes = LC.snapshotToShapes(studentCanvas.getSnapshot())
        //console.log(shapes);
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
});

function test() {
    alert("Nitin");
}

function LastDivCount(container) {
    var lastDiv = $('div[name=' + container + ']').children().last().attr('id');//$('ul[id=' + container + ']').children().length//.last().attr('id');//$('div[name=' + container + ']').children().last().attr('id');
    //var count = 0;
    //if (lastDiv != 0) { count = count + 1;}//count = parseInt(lastDiv.split('_')[2]); }
    ////count = count + 1;
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
                    isItalic: false,
                    isBold: false,
                    fontName: 'Helvetica',
                    fontSizeIndex: 4
                };
            },
            getFontSizes: function () {
                return [9, 10, 12, 14, 18, 24, 36, 48, 64, 72, 96, 144, 288];
            },
            //updateTool: function (newState) {
            //    var fontSize, items, k;
            //    if (newState == null) {
            //        newState = {};
            //    }
            //    for (k in this.state) {
            //        if (!(k in newState)) {
            //            newState[k] = this.state[k];
            //        }
            //    }
            //    fontSize = this.getFontSizes()[newState.fontSizeIndex];
            //    items = [];
            //    if (newState.isItalic) {
            //        items.push('italic');
            //    }
            //    if (newState.isBold) {
            //        items.push('bold');
            //    }
            //    items.push(fontSize + "px");
            //    items.push(FONT_NAME_TO_VALUE[newState.fontName]);
            //    this.props.lc.tool.font = items.join(' ');
            //    return this.props.lc.trigger('setFont', items.join(' '));
            //},
            //handleFontSize: function (event) {
            //    var newState;
            //    newState = {
            //        fontSizeIndex: event.target.value
            //    };
            //    this.setState(newState);
            //    return this.updateTool(newState);
            //},
            //handleFontFamily: function (event) {
            //    var newState;
            //    newState = {
            //        fontName: event.target.selectedOptions[0].innerHTML
            //    };
            //    this.setState(newState);
            //    //return this.updateTool(newState);
            //},
            AssignHandle: function (event) {
                if (event.currentTarget.value == "Grant")
                    event.currentTarget.value = "Revoke";
                else
                    event.currentTarget.value = "Grant";
                //return this.updateTool(newState);
            },
            //handleItalic: function (event) {
            //    var newState;
            //    newState = {
            //        isItalic: !this.state.isItalic
            //    };
            //    this.setState(newState);
            //    return this.updateTool(newState);
            //},
            //handleBold: function (event) {
            //    var newState;
            //    newState = {
            //        isBold: !this.state.isBold
            //    };
            //    this.setState(newState);
            //    return this.updateTool(newState);
            //},
            //componentDidMount: function () {
            //    return this.updateTool();
            //},
            render: function () {
                var br, div, input, label, lc, optgroup, option, ref4, select, span;
                lc = this.props.lc;
                ref4 = React.DOM, div = ref4.div, input = ref4.input, select = ref4.select, option = ref4.option, br = ref4.br, label = ref4.label, span = ref4.span, optgroup = ref4.optgroup,button = ref4.button;
                return div({
                    className: 'user-List',
                    id: "connected-users"
                },
                //}, select({
                //    className:"col-lg-3 dropdown",
                //    value: this.state.fontName,
                //    onChange: this.handleFontFamily
                //}, allUsers.map((function (_this) {
                //    return function (arg) {
                //        var user;
                //        user = arg;
                //        return option({
                //            value: user.name,
                //            key: user.id
                //        }, user.name);
                //        //fonts.map(function (user, ix) {
                //        //    return option({
                //        //        value: user.name,
                //        //        key: ix
                //        //    }, user.name);
                //        //});
                //    };
                //})(this))), span({},input({
                //    type: 'button',
                //    id: 'btnAction',
                //    value: 'Grant',
                //    className: "btn btn-primary",
                //    optionsStyle : "float:right",
                    //})), 
                    div({
                        name : "hidden-row-container",
                    }, div({
                        className: "row",
                        name : "row_0",
                    }, div({
                        className: "col-xs-7",
                        id : "user",
                    }), div({
                        className : "col-xs-5",
                    }, input({
                        className: "btn btn-primary btn-xs",
                        value: "Grant",
                        onClick : this.AssignHandle,
                    })))));
                //test();
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
                //console.log(shape);
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

            //teacherCanvas.on("drawStart", function (tool) {
            //    var toolName = tool.tool.name;
            //    switch (toolName) {
            //        case "Rectangle":
            //            //console.log("Rectangle is drawing");
            //            uploadSnapShot();
            //            break;
            //        case "Ellipse":
            //            //console.log("Ellipse is drawing");
            //            uploadSnapShot();
            //            break;
            //        case "Pencil":
            //            //console.log("Pencil is drawing");
            //            uploadSnapShot();
            //            break;
            //        case "Line":
            //            //console.log("Line is drawing");
            //            uploadSnapShot();
            //            break;
            //        case "Eraser":
            //            //console.log(tool.tool);
            //            //console.log("eraser is drawing");
            //            uploadSnapShot();
            //            break;
            //        default:
            //            break;
            //    }
            //    //var shape = tool.tool.currentShape;
            //    //var shapeString = LC.shapeToJSON(shape);
            //    //blackboardHub.server.uploadShape(JSON.stringify(shapeString), "", lessonId);
            //});

            //teacherCanvas.on("drawContinue", function (tool) {
            //    var toolName = tool.tool.name;
            //    switch (toolName) {
            //        case "Rectangle":
            //            //console.log("Rectangle is drawing");
            //            //var shape = tool.tool.currentShape;
            //            //var shapeString = LC.shapeToJSON(shape);
            //            //blackboardHub.server.uploadShape(JSON.stringify(shapeString), "", lessonId);
            //            uploadSnapShot();
            //            break;
            //        case "Ellipse":
            //            //console.log("Ellipse is drawing");
            //            uploadSnapShot();
            //            break;
            //        case "Pencil":
            //            console.log("Pencil is drawing");
            //            uploadSnapShot();
            //            break;
            //        case "Line":
            //            console.log("Line is drawing");
            //            uploadSnapShot();
            //            break;
            //        case "Eraser":
            //            //console.log(tool.tool);
            //            console.log("eraser is drawing");
            //            uploadSnapShot();
            //            break;
            //        default:
            //            break;
            //    }
            //    //var shape = tool.tool.currentshape;
            //    //var shapestring = lc.shapetojson(shape);
            //    //blackboardhub.server.uploadshape(json.stringify(shapestring), "", lessonid);
            //});

            //teacherCanvas.on("drawEnd", function (tool) {
            //    var toolName = tool.tool.name;
            //    switch (toolName) {
            //        case "Rectangle":
            //            //console.log("Rectangle is drawing");
            //            uploadSnapShot();
            //            break;
            //        case "Ellipse":
            //            console.log("Ellipse is drawing");
            //            uploadSnapShot();
            //            break;
            //        case "Pencil":
            //            console.log("Pencil is drawing");
            //            uploadSnapShot();
            //            break;
            //        case "Line":
            //            console.log("Line is drawing");
            //            uploadSnapShot();
            //            break;
            //        case "Eraser":
            //            //console.log(tool.tool);
            //            //console.log("eraser is drawing");
            //            uploadSnapShot();
            //            break;
            //        default:
            //            break;
            //    }
            //    //console.log(tool);
            //    //var shape = tool.tool.currentShape;
            //    //var shapeString = LC.shapeToJSON(shape);
            //    //blackboardHub.server.uploadShape(JSON.stringify(shapeString), "", lessonId);
            //});
        }
        else {
            resizeStudentCanvas();
            setTimeout(function () { blackboardHub.server.getTeacherSnapshot(lessonId); }, 3000);
            //blackboardHub.server.getTeacherSnapshot(lessonId);
        }
       // test();
    });
});

function uploadSnapShot() {
    var snaps = teacherCanvas.getSnapshot();
    blackboardHub.server.uploadSnapshot(JSON.stringify(snaps), lessonId);
}

//function test1() {
//    var elementToMatch = $(".with-gui")[0];
//    var elementsToResize = $(".lc-drawing.with-gui canvas");
//    var scale = 1;
//    LC.util.matchElementSize(elementToMatch, elementsToResize, scale, callback = null);
//}
function resizeTeacherCanvas() {
    if(IsHost === "true" )
        setTimeout(function () {
            var elementToMatch = $(".with-gui")[0];
            var elementsToResize = $(".lc-drawing.with-gui canvas");
            var scale = 1;
            LC.util.matchElementSize(elementToMatch, elementsToResize, scale, callback = null);
        }, 1000);
}

function resizeStudentCanvas() {
    var elementToMatch = $("#lc")[0];
    var elementsToResize = $(".lc-drawing.with-gui canvas");
    var scale = 1;
    LC.util.matchElementSize(elementToMatch, elementsToResize, scale, callback = null);
}