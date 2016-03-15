var userID;

$('#btn_login').click(function () {

    Zoom.init("http://www.zoom.us/api/v1");

    Zoom.login({email:"steve@syntronian.com",password:"zoomtronian123!!"},function(result){

        $('#btn_login').val("login success");
        userID = result.user_id;
    });

    return false;

});

$('#btn_list').click(function(){

    Zoom.listMeeting({page_size:10,page_number:1},function(result){

        $('#api_list').html("List Meeting");
        for (i = 0; i < result.meetings.length; i++) {
            var meeting = result.meetings[i];
            $('#meetingList').append("<li>" + meeting.id + ": " + meeting.topic + ".</li>");
        }

    });

    return false;

});

$('#btn_create').click(function(){

    Zoom.createMeeting(JSON.parse($('#meetingInfo').val()),

    function(result){

        $('#api_title').html("Create Meeting");

        $('#errMsg').html(JSON.stringify(result));

    });

    return false;

});

$('#btn_get').click(function(){

    if($('#meeting_number').val().trim().length < 8){

        alert("Please enter meeting number.");

        return ;

    }

    Zoom.getMeeting({meeting_number: $('#meeting_number').val()},

    function(result){

        $('#api_title').html("Get Meeting");
        var meeting = result;

        if (meeting.host_id == userID) {
            $('#errMsg').html("<a href=" + meeting.start_url + ">Topic: <b> " + meeting.topic + "</b>. Starting: " + meeting.start_time + ". <b>Hosted by you.</b> </a>");
        } else {
            $('#errMsg').html("<a href=" + meeting.join_url + ">Topic: <b> " + meeting.topic + "</b>. Starting: " + meeting.start_time + "</a>");
        }


    });

    return false;

});

$('#btn_end').click(function(){

    if($('#meeting_number').val().trim().length < 8){

        alert("Please enter meeting number.");

        return ;

    }

    Zoom.endMeeting({meeting_number: $('#meeting_number').val()},

    function(result){

        $('#api_title').html("End Meeting");

        $('#errMsg').html(JSON.stringify(result));

    });

    return false;

});

$('#btn_del').click(function(){

    if($('#meeting_number').val().trim().length < 8){

        alert("Please enter meeting number.");

        return ;

    }

    Zoom.deleteMeeting({meeting_number: $('#meeting_number').val()},

    function(result){

        $('#api_title').html("Delete Meeting");

        $('#errMsg').html(JSON.stringify(result));

    });

    return false;

});

$('#btn_pmi').click(function(){

    Zoom.getPMI(

    function(result){

        $('#api_title').html("GET PMI");

        $('#errMsg').html(JSON.stringify(result));

    });

    return false;

});

$('#btn_update').click(function(){

    if($('#meeting_number').val().trim().length < 8){

        alert("Please enter meeting number.");

        return ;

    }

    var data = JSON.parse($('#meetingInfo').val());

    data.meeting_number = $('#meeting_number').val().trim();

    Zoom.updateMeeting(data,

    function(result){

        $('#api_title').html("Update Meeting");

        $('#errMsg').html(JSON.stringify(result));

    });

    return false;

});
