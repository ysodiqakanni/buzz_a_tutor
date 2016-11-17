var uploadPath = $("#uploadPath").val();
var file;
var lessonId;
//Load Preview of upload item
// Sets up input event on document load.
$("#classResource").change(function (event) {
    $("#message-container").empty();
    $("#modal-button-container").empty();
    event.preventDefault();
    file = event.target.files[0];
    var fileName = file.name;
    $("#message-container").append("<p>" + fileName + " is ready to upload</p>");
    $("#modal-button-container").append('<button type="button" class="btn btn-default" data-dismiss="modal">No</button>');
    $("#modal-button-container").append('<button type="button" class="btn btn-primary" onclick="submitResource(0)">Yes</button>');
    $('#messageModal').modal();
});

function submitResource(trig) {
    if ($("#classResource").val() != null && trig === 0) {       
        var fileName = file.name;
        var fileExt = fileName.split('.').pop().toLowerCase();
        if (fileExt === "png" || fileExt === "jpg" || fileExt === "tif" || fileExt === "pdf") {
            $("#message-container").empty();
            $("#modal-button-container").empty();
            $("#message-container").append("<p>Whould you like to be able to display on blackboard?</p>");
            $("#modal-button-container").append('<button type="button" class="btn btn-default" onclick="submitResource(1)">No</button>');
            $("#modal-button-container").append('<button type="button" class="btn btn-primary" onclick="loadPreview()">Yes</button>');
        } else {
            $("#resourceForm").submit();
            $("#message-container").empty();
            $("#modal-button-container").empty();
            $("#message-container").append("<p> Upload was succesful</p>");
        }
    } else if ($("#classResource").val() != null && trig === 1) {
        $("#resourceForm").submit();
        $("#message-container").empty();
        $("#modal-button-container").empty();
        $("#message-container").append("<p> Upload was succesful</p>");
    }
}

// Load image preview
function loadPreview() {
    $('#messageModal').modal('hide');
    $('#canvasCarousel').empty();
    var fileName = file.name;
    var fileExt = fileName.split('.').pop().toLowerCase();
    var title = fileName.split('.')[0];
    if (fileExt === "pdf") {
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
                var id = 'item' + currPage;
                if (currPage === 1) {
                    $('#canvasCarousel').append('<div id="' + id + '"class="item active"></div>');
                } else {
                    $('#canvasCarousel').append('<div id="' + id + '"class="item"></div>');
                }

                var carouselItem = document.getElementById(id);

                var canvas = document.createElement("canvas");
                canvas.id = "page-"+currPage;
                canvas.style.display = "block";
                var context = canvas.getContext('2d');
                canvas.height = viewport.height;
                canvas.width = viewport.width;

                //Draw it on the canvas
                page.render({ canvasContext: context, viewport: viewport });

                //Add it to the web page
                carouselItem.appendChild(canvas);
                $('#' + id).append('<div class="carousel-caption"><input id="'+id+'check" type="checkbox"/></div');
                $('#previewTitle').val(title);

                if (currPage === numPages) {
                    $("#preview-carousel").append('<a class="left carousel-control" href="#preview-carousel" role="button" data-slide="prev"><span class="glyphicon glyphicon-chevron-left" aria-hidden="true"></span><span class="sr-only">Previous</span></a>');
                    $("#preview-carousel").append('<a class="right carousel-control" href="#preview-carousel" role="button" data-slide="next"><span class="glyphicon glyphicon-chevron-right" aria-hidden="true"></span><span class="sr-only">Next</span></a>');
                    $('#preview-carousel').carousel();
                }

                $('#previewModal').modal();

                //Move to next page

                currPage++;
                if (thePDF !== null && currPage <= numPages) {
                    thePDF.getPage(currPage).then(handlePages);
                }
            }
        }
        //Step 3:Read the file as ArrayBuffer
        fileReader.readAsArrayBuffer(file);
    } else if (fileExt === "png" || fileExt === "jpg" || fileExt === "tif") {
        var fileReader = new FileReader();
        fileReader.addEventListener("load", function () {
            var id = "item" + 1;

            $('#canvasCarousel').append('<div id="' + id + '"class="item active"></div>');

            var carouselItem = document.getElementById(id);

            var canvas = document.createElement("canvas");
            canvas.id = "page-" + 1;
            canvas.style.display = "block";
            var context = canvas.getContext('2d');
            canvas.height = 568;
            canvas.width = 600;

            //Draw it on the canvas
            var img = new Image();
            img.src = fileReader.result;
            img.onload = function () {
                context.drawImage(img, 0, 0, canvas.width, canvas.height);
            };

            //Add it to the web page
            carouselItem.appendChild(canvas);
            $('#previewTitle').val(fileName);
            $('#previewModal').modal();
        }, false);

        //Step 3:Read the file as ArrayBuffer
        fileReader.readAsDataURL(file);
    }
}

// Save image function
function savePreview(lessonId) {
    $('#previewModal').modal("hide");
    var page = 1;
    var pages = $("#canvasCarousel .item").length;
    function postCanvas(id) {
        var canvas = document.getElementById(id);
        var img2SaveRaw = canvas.toDataURL('image/png'),
        img2SaveArray = img2SaveRaw.split(','),
        img2Save = img2SaveArray[1],
        title = $('#previewTitle').val();
            $.ajax({
                type: "POST", // Type of request
                url: "../../api/lessons/uploadtocloud", //"../api/lessons/upload", //The controller/Action
                dataType: "json",
                data: {
                    "lessonid": lessonId,
                "title": title + "-" + id,
                "data": img2Save
                },

            success: function(data) {
                    console.log(data);
                    blackboardHub.server.updateList(listModel);
                },

            error: function(err) {
                    console.log("error[" + err.status + "]: " + err.statusText);
                }
        });
    }
    function ifchecked() {
        var id = 'page-' + page;
        var inputId = 'item' + page + 'check';
        if ($("#" + inputId).is(":checked")) {
            postCanvas(id);
        }
        page++;
        if (page <= pages) {
            ifchecked();
        } else if (page > pages) {
            $("#resourceForm").submit();
        }
    }
    if (page < pages || page === pages) {
        ifchecked();
    }  
}
// End of Save image function