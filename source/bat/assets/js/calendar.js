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
    rendorCalendar(userid)
   
});

function rendorCalendar(userid) {
    $.ajax({
        type: "POST", // Type of request
        url: getEventsPath, //The controller/Action
        dataType: "json",
        data: {
            "userid": userid
        },

        success: function (data) {
            $('#calendar').fullCalendar({
                //timezone: "Europe/London",
                events: jQuery.parseJSON(data),
                eventLimit: true,
                // put your options and callbacks here
                dayClick: function (date) {
                    getDay(userid, date.format());
                },
                eventClick: function (calEvent) {
                    getDay(userid, calEvent.start.format());
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
                //timezone: "Europe/London",

                nowIndicator: true,
                header: {
                    left: false,
                    center: 'title',
                    right: false
                },
                defaultDate: date,
                events: jQuery.parseJSON(data),
                dayClick: function (date) {
                },
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