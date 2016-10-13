var uploadPath = $("#uploadPath").val();
var file;
//Load Preview of upload item
// Sets up input event on document load.
$("#ClassResource").change(function (event) {
    file = event.target.files[0];
    var fileName = file.name;
    var fileExt = fileName.split('.').pop().toLowerCase();
    if (fileExt == "png" || fileExt == "jpg" || fileExt == "tif" || fileExt == "pdf") {
        $('#toImageModal').modal();
    } else {
        //Send file to upload
    }
});

// Load image preview

function loadPreview() {
    $('#toImageModal').modal('hide');
    var fileName = file.name;
    var fileExt = fileName.split('.').pop().toLowerCase();
    if (fileExt == "pdf") {
        //Step 2: Read the file using file reader
        var fileReader = new FileReader();
        fileReader.onload = function () {
            //Step 4:turn array buffer into typed array
            var typedarray = new Uint8Array(this.result);
            //Step 5:PDFJS should be able to read this
            PDFJS.getDocument(typedarray).then(function (pdf) {
                function createCanvas(num) {
                    console.log(num);
                    canvasId = "previewCanvas-" + num;
                    if (num == 1) {
                        $('#canvasCarousel').append('<div class="item active"><canvas id="'+canvasId+'"></canvas><div class="carousel-caption"></div></div>');
                    } else {
                        $('#canvasCarousel').append('<div class="item"><canvas id='+canvasId+'"></canvas><div class="carousel-caption"></div></div>');
                    }
                }
                function getPage(pagenum) {
                    console.log(pagenum);
                    pdf.getPage(pagenum)
                }
                for(var i = 1; i <= pdf.numPages; i++) {
                    console.log('Number of Pages: ' + i);
                    $.when(createCanvas(i))
                    .then(pdf.getPage(i)
                    .then(function getPage(page) {
                        //
                        // Prepare canvas using PDF page dimensions
                        //
                        canvasId = "previewCanvas-" + pagenum;
                        console.log(canvasId);
                        var canvas = document.getElementById(canvasId);
                        var scale = 0.8;
                        var context = canvas.getContext('2d');
                        var viewport = page.getViewport(scale);
                        canvas.width = viewport.width;
                        canvas.height = viewport.height;
                        //
                        // Render PDF page into canvas context
                        //
                        var renderContext = {
                            canvasContext: context,
                            viewport: viewport
                        };
                        page.render(renderContext);
                        $('#previewTitle').val(fileName);
                        $('#previewModal').modal();
                    }))
                    .then(function () {
                        console.log("Requests succeeded and animations completed");
                    }).fail(function () {
                        console.log("something went wrong!");
                    })
                }
            });
        }
        //Step 3:Read the file as ArrayBuffer
        fileReader.readAsArrayBuffer(file);
    } else if (fileExt == "png" || fileExt == "jpg" || fileExt == "tif") {
        var fileReader = new FileReader();
        fileReader.addEventListener("load", function () {
            var context = previewCanvas.getContext('2d');
            previewCanvas.width = 568;
            previewCanvas.height = 600;
            var img = new Image();
            img.src = fileReader.result;
            img.onload = function () {
                context.drawImage(img, 0, 0, canvas.width, canvas.height);
            };
            $('#previewTitle').val(fileName);
            $('#previewModal').modal();
        }, false);

        //Step 3:Read the file as ArrayBuffer
        fileReader.readAsDataURL(file);
    }
}

// Save image function
function savePreview(lessonId) {
    var img2SaveRaw = previewCanvas.toDataURL('image/png'),
        img2SaveArray = img2SaveRaw.split(','),
        img2Save = img2SaveArray[1],
        title = $('#previewTitle').val();
    console.log(uploadPath);

    $.ajax({
        type: "POST", // Type of request
        url: "../../api/lessons/uploadtocloud", //"../api/lessons/upload", //The controller/Action
        dataType: "json",
        data: {
            "lessonid": lessonId,
            "title": title,
            "data": img2Save,
        },

        success: function (data) {
            console.log(data);
        },

        error: function (err) {
            console.log("error[" + err.status + "]: " + err.statusText);
        }
    })
}
// End of Save image function