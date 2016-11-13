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
    $("#canvasCarousel").empty();
    $("#bbImageInput").empty();
    $(".carousel-control").addClass("hidden");
    $("#modal-button-container").append(cancelBtn);
    $('#uploadModal').modal();
};
$("#bbImageInput").change(function (event) {
    $("#canvasCarousel").empty();
    $("#modal-button-container").empty();
    $("#modal-button-container").append(cancelBtn);
    $("upload-Canvas").empty();
    event.preventDefault();
    $("#bbImageError").empty();
    imageFile = event.target.files[0];
    var fileName = imageFile.name;
    var fileExt = fileName.split('.').pop().toLowerCase();
    if (fileExt == "png" || fileExt == "jpg" || fileExt == "tif") {
        var fileReader = new FileReader();
        fileReader.addEventListener("load", function () {
            var currPage = 1; //Pages are 1-based not 0-based
            var id = "p" + 1;
            $('#canvasCarousel').append('<div id="' + id + '"class="item active hidden"></div>');
            var carouselItem = document.getElementById(id)
            var canvas = document.createElement("canvas");
            canvas.style.display = "block";
            var context = canvas.getContext('2d');
            canvas.id = "p" + currPage + "-canvas";
            canvas.height = 568;
            canvas.width = 600;

            //Draw it on the canvas
            var img = new Image();
            img.src = fileReader.result;
            img.onload = function () {
                context.drawImage(img, 0, 0, canvas.width, canvas.height);
            };

            carouselItem.appendChild(canvas);
            $('#' + id).append('<div class="carousel-caption"><input class="hidden" id="' + id + '-check" type="checkbox" checked/></div');
        }, false);

        //Step 3:Read the file as ArrayBuffer
        fileReader.readAsDataURL(imageFile);
        $("#modal-button-container").append(uploadBtn);
    } else if (fileExt == "pdf") {
        //Step 2: Read the file using file reader
        var fileReader = new FileReader();
        fileReader.onload = function () {
            //Step 4:turn array buffer into typed array
            var typedarray = new Uint8Array(this.result);
            //Step 5:PDFJS should be able to read this
            var currPage = 1; //Pages are 1-based not 0-based
            var numPages = 0;
            var thePDF = null;
            PDFJS.getDocument(typedarray).then(function (pdf) {
                //Set PDFJS global object (so we can easily access in our page functions
                thePDF = pdf;

                //How many pages it has
                numPages = pdf.numPages;

                //Start with first page
                pdf.getPage(1).then(handlePages);
            });

            function handlePages(page) {
                //This gives us the page's dimensions at full scale
                var viewport = page.getViewport(1);

                //We'll create a canvas for each page to draw it on
                var id = 'p' + currPage;
                if (currPage == 1) {
                    $('#canvasCarousel').append('<div id="' + id + '"class="item active"></div>');
                } else {
                    $('#canvasCarousel').append('<div id="' + id + '"class="item"></div>');
                }

                var carouselItem = document.getElementById(id)

                var canvas = document.createElement("canvas");
                canvas.id = "p" + currPage + "-canvas";
                canvas.style.display = "block";
                var context = canvas.getContext('2d');
                canvas.height = viewport.height;
                canvas.width = viewport.width;

                //Draw it on the canvas
                page.render({ canvasContext: context, viewport: viewport });

                //Add it to the web page
                carouselItem.appendChild(canvas);
                $('#' + id).append('<div class="carousel-caption"><input id="' + id + '-check" type="checkbox"/></div');

                if (currPage == numPages) {
                    $("#preview-carousel").append('<a class="left carousel-control" href="#preview-carousel" role="button" data-slide="prev"><span class="glyphicon glyphicon-chevron-left" aria-hidden="true"></span><span class="sr-only">Previous</span></a>');
                    $("#preview-carousel").append('<a class="right carousel-control" href="#preview-carousel" role="button" data-slide="next"><span class="glyphicon glyphicon-chevron-right" aria-hidden="true"></span><span class="sr-only">Next</span></a>');
                    $('#preview-carousel').carousel();
                }

                //Move to next page

                currPage++;
                if (thePDF !== null && currPage <= numPages) {
                    thePDF.getPage(currPage).then(handlePages);
                }
            }
        }
        //Step 3:Read the file as ArrayBuffer
        fileReader.readAsArrayBuffer(imageFile);
        $("#modal-button-container").append(uploadBtn);
    } else {
        errorMessage = "Incorrect file type.";
        $("#bbImageError").append(errorStart + errorMessage + errorEnd);
    }
    this.value = null;
});
function uploadImage() {
    $("#modal-button-container").empty();
    $("#modal-button-container").append(uploadingIcon);
    var page = 1;
    var pages = $("#canvasCarousel .item").length;
    function postCanvas(id) {
        var canvas = document.getElementById(id + "-canvas");
        var img2SaveRaw = canvas.toDataURL('image/png'),
        img2SaveArray = img2SaveRaw.split(','),
        img2Save = img2SaveArray[1],
        title = imageFile.name;
        //window.open("data:image/png;base64," + img2Save);
        $.ajax({
            type: "POST", // Type of request
            url: "../api/lessons/uploadtocloud", //"../api/lessons/upload", //The controller/Action
            dataType: "json",
            data: {
                "lessonid": lessonId,
                "title": title + "-" + id,
                "data": img2Save,
            },

            success: function (data) {
            },

            error: function (err) {
                errorMessage = "Something went wrong, try again later.";
                $("#bbImageError").append(errorStart + errorMessage + errorEnd);
                console.log("error[" + err.status + "]: " + err.statusText);
            }
        })
    }
    function ifchecked() {
        var id = 'p' + page;
        var inputId = id + "-check";
        if ($("#" + inputId).is(":checked")) {
            postCanvas(id);
        }
        page++
        if (page <= pages) {
            ifchecked();
        } else if (page > pages) {
            console.log("done");
            $("#modal-button-container").empty();
            blackboardHub.server.updateList(listModel);
            $("#modal-button-container").append(successBtn);
        }
    }
    if (page < pages || page == pages) {
        ifchecked();
    }
}