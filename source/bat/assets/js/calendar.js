var userid,
    getEventsPath,
    getDayPath,
    getLessonPath;

$(document).ready(function () {
    userid = $("#userID").val();
    getEventsPath = $("#getEventsPath").val();
    getDayPath = $("#getDayPath").val();
    getLessonPath = $("#getLessonPath").val();
    // page is now ready, initialize the calendar...
    $.ajax({
        type: "POST", // Type of request
        url: getEventsPath, //The controller/Action
        dataType: "json",
        data: {
            "userid": userid
        },

        success: function (data) {
            $('#calendar').fullCalendar({
                events: jQuery.parseJSON(data),
                // put your options and callbacks here
                dayClick: function (date) {
                    getDay(userid, date.format());
                    console.log(date.format());
                },
                eventClick: function (calEvent) {
                    getDay(userid, calEvent.start.format("YYYY-MM-DD"));
                    //// change the border color just for fun
                    //$(this).css('background-color', 'red');
                    console.log(calEvent.start.format("YYYY-MM-DD"));
                },
            })
        },

        error: function (err) {
            console.log("error[" + err.status + "]: " + err.statusText);
        }
    })
});

function getDay(userid, date) {
    $('#agendaDay').fullCalendar('destroy');
    $.ajax({
        type: "POST", // Type of request
        url: getDayPath, //The controller/Action
        dataType: "json",
        data: {
            "userid": userid,
            "date" : date
        },

        success: function (data) {
            $('#agendaDay').fullCalendar({
                defaultView: 'agendaDay',
                header: {
                    left: false,
                    center: 'title',
                    right: false
                },
                defaultDate: date,
                dayClick: function (date) {
                    console.log(date.format());
                },
                events: jQuery.parseJSON(data),
                eventClick: function (calEvent) {
                    window.location.href = getLessonPath + "/" + calEvent.id;
                    //// change the border color just for fun
                    //$(this).css('background-color', 'red');
                },
            })
        },

        error: function (err) {
            console.log("error[" + err.status + "]: " + err.statusText);
        }
    })
}