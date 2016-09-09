var canvasPDF;

$("document").ready(function () {
    canvasPDF = document.getElementById('pdfCanvas');
    $("#pdfData").change(function (event) {
        var file = event.target.files[0];
        var fileName = file.name;
        //Step 2: Read the file using file reader
        var fileReader = new FileReader();
        fileReader.onload = function () {
            //Step 4:turn array buffer into typed array
            var typedarray = new Uint8Array(this.result);
            //Step 5:PDFJS should be able to read this
            PDFJS.getDocument(typedarray).then(function (pdf) {
                // do stuff
                pdf.getPage(1).then(function getPageHelloWorld(page) {
                    //
                    // Prepare canvas using PDF page dimensions
                    //
                    var scale = 0.8;
                    var context = canvasPDF.getContext('2d');
                    var viewport = page.getViewport(scale);
                    canvasPDF.width = viewport.width;
                    canvasPDF.height = viewport.height;
                    //
                    // Render PDF page into canvas context
                    //
                    var renderContext = {
                        canvasContext: context,
                        viewport: viewport
                    };
                    page.render(renderContext);
                    $('#myModalLabel').text(fileName);
                    $('#myModal').modal();
                })
            });
        }

        //Step 3:Read the file as ArrayBuffer
        fileReader.readAsArrayBuffer(file);
    });
});

// Save image function
function savePDF(lessonId) {
    var img2SaveRaw = canvasPDF.toDataURL('image/png'),
        img2SaveArray = img2SaveRaw.split(','),
        img2Save = img2SaveArray[1],
        title = $('#myModalLabel').text();

        $.ajax({
            type: "POST", // Type of request
            url: "../api/lessons/upload", //The controller/Action
            dataType: "json",
            data: {
                "lessonid": lessonId,
                "title": title,
                "data": img2Save,
            },

            success: function (data) {
                $('#myModal').modal('toggle');
            },

            error: function (err) {
                console.log("error[" + err.status + "]: " + err.statusText);
            }
        })
}
// End of Save image function