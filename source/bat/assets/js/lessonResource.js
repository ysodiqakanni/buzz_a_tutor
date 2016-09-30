var uploadPath = $("#uploadPath").val();

//Load Preview of upload item
// Sets up input event on document load.
$("#ClassResource").change(function (event) {
    var file = event.target.files[0];
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
                pdf.getPage(1).then(function getPageHelloWorld(page) {
                    //
                    // Prepare canvas using PDF page dimensions
                    //
                    var scale = 0.8;
                    var context = previewCanvas.getContext('2d');
                    var viewport = page.getViewport(scale);
                    previewCanvas.width = viewport.width;
                    previewCanvas.height = viewport.height;
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
                })
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
    } else {
        //Not accepted format
    }
});

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