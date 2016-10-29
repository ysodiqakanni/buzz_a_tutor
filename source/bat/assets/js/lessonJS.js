var cancelBtn = '<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>';
var successBtn = '<button type="button" class="btn btn-success" data-dismiss="modal">Close</button>';
var errorStart = '<p style="color:red"><i class="fa fa-times" aria-hidden="true"></i> ';
var errorMessage = '';
var errorEnd = '</p>';
var imageFile;
var uploadBtn = '<button type="button" class="btn btn-primary" onclick="uploadImage()">Upload</button>';
var uploadingIcon = '<i class="fa fa-spinner fa-pulse fa-3x fa-fw"></i><span class="sr-only">Loading...</span>';

var uploadImageModal = function () {
    $("#modal-button-container").empty();
    $("#upload-Canvas").empty();
    $("#bbImageInput").empty();
    $("#modal-button-container").append(cancelBtn);
    $('#uploadModal').modal();
};
$("#bbImageInput").change(function (event) {
    $("upload-Canvas").empty();
    event.preventDefault();
    $("#bbImageError").empty();
    imageFile = event.target.files[0];
    var fileName = imageFile.name;
    var fileExt = fileName.split('.').pop().toLowerCase();
    if (fileExt == "png" || fileExt == "jpg" || fileExt == "tif") {
        var fileReader = new FileReader();
        fileReader.addEventListener("load", function () {
            var canvas = document.createElement("canvas");
            canvas.style.display = "block";
            var context = canvas.getContext('2d');
            canvas.id = 'uploadCanvas';
            canvas.height = 568;
            canvas.width = 600;

            //Draw it on the canvas
            var img = new Image();
            img.src = fileReader.result;
            img.onload = function () {
                context.drawImage(img, 0, 0, canvas.width, canvas.height);
            };

            $("#upload-Canvas").append(canvas);
        }, false);

        //Step 3:Read the file as ArrayBuffer
        fileReader.readAsDataURL(imageFile);
        $("#modal-button-container").append(uploadBtn);
    } else {
        errorMessage = "Incorrect file type.";
        $("#bbImageError").append(errorStart + errorMessage + errorEnd);
    }
});
function uploadImage() {
    $("#modal-button-container").empty();
    $("#modal-button-container").append(uploadingIcon);
    var img2SaveRaw = document.getElementById('uploadCanvas').toDataURL('image/png'),
    img2SaveArray = img2SaveRaw.split(','),
    img2Save = img2SaveArray[1],
    title = imageFile.name;
    $.ajax({
        type: "POST", // Type of request
        url: "../api/lessons/uploadtocloud", //"../api/lessons/upload", //The controller/Action
        dataType: "json",
        data: {
            "lessonid": lessonId,
            "title": title,
            "data": img2Save,
        },

        success: function (data) {
            $("#modal-button-container").empty();
            $("#modal-button-container").append(successBtn);
        },

        error: function (err) {
            errorMessage = "Something went wrong, try again later.";
            $("#bbImageError").append(errorStart + errorMessage + errorEnd);
            console.log("error[" + err.status + "]: " + err.statusText);
        }
    })
}