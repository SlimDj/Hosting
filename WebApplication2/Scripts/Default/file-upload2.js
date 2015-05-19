// Get the template HTML and remove it from the doumenthe template HTML and remove it from the doument
var previewNode = document.querySelector("#template");
previewNode.id = "";
var totalFiles = 0;
var previewTemplate = previewNode.parentNode.innerHTML;
previewNode.parentNode.removeChild(previewNode);
var myDropzone = new Dropzone(document.body, { // Make the whole body a dropzone
    url: "Home/SaveUploadedFile", // Set the url
    thumbnailWidth: 80,
    maxFiles: 3,
    thumbnailHeight: 80,
    parallelUploads: 20,
    previewTemplate: previewTemplate,
    autoQueue: false, // Make sure the files aren't queued until manually added
    previewsContainer: "#previews", // Define the container to display the previews
    clickable: ".fileinput-button", // Define the element that should be used as click trigger to select files.
    
});
myDropzone.on("addedfile", function (file) {
    // Hookup the start button
    file.previewElement.querySelector(".start").onclick = function () { myDropzone.enqueueFile(file); };
    if (myDropzone.files.length >= 2) {
        document.querySelector("#actions .start").setAttribute("style", "visibility:visible");
        document.querySelector("#actions .cancel").setAttribute("style", "visibility:visible");
    }
    if (myDropzone.files.length == 4) {
        myDropzone.removeFile(myDropzone.files[3]);
        $("#myAllert").attr("style","display:block")
    }
});

// Update the total progress bar
myDropzone.on("totaluploadprogress", function (progress) {
    document.querySelector("#total-progress .progress-bar").style.width = progress + "%";
});

myDropzone.on("sending", function (file) {
    // Show the total progress bar when upload starts
    document.querySelector("#total-progress").style.opacity = "1";
    // And disable the start button
    file.previewElement.querySelector(".start").setAttribute("disabled", "disabled");
});

// Hide the total progress bar when nothing's uploading anymore
myDropzone.on("queuecomplete", function (progress) {
    document.querySelector("#total-progress").style.opacity = "0";
});

// Setup the buttons for all transfers
// The "add files" button doesn't need to be setup because the config
// `clickable` has already been specified.
document.querySelector("#actions .start").onclick = function () {
    myDropzone.enqueueFiles(myDropzone.getFilesWithStatus(Dropzone.ADDED));
};
document.querySelector("#actions .cancel").onclick = function () {
    myDropzone.removeAllFiles(true);
};

$('#myModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget) // Button that triggered the modal
    var fileUrl = button.data('whatever')
    var modal = $(this)
    var ww;
    modal.find('.modal-body #gvncode').val(fileUrl)
 
    $.ajax({
        url: '/home/Giveme',
        type: 'POST',
        dataType: 'json',
        data: "{path:" + JSON.stringify(fileUrl) + "}",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            
            // get the result and do some magic with it
            modal.find('.modal-title').text(data.Name)
            modal.find('.modal-body #recipient-name').val(data.Name)
            modal.find('.modal-body textarea').val(data.Description)
            modal.find('.modal-body #check').prop("checked", data.Privacy)
            document.getElementById("select").selectedIndex = data.ExpDate
            
        }
    });
    // Extract info from data-* attributes
    // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
    // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
    modal.find('.modal-body #recipient-link').val('https://localhost:44300/Download/' + fileUrl)
})
$('#myModal').on('hidden.bs.modal', function (event) {
    $('#mySucc').fadeOut(1);
    document.getElementById("select").selectedIndex = -1;
});
$('#SaveChgs').on('click', function () {
    //request to server with additional data
    var modal = $('#myModal')
    var addinf = {
        "text": modal.find('.modal-body textarea').val(),//description
        "publc": modal.find('.modal-body #check').prop("checked"),//checkBox
        "name": modal.find('.modal-body #recipient-name').val(),// new file name
        "expDate": document.getElementById("select").selectedIndex,//combobox expiration date
        "link": modal.find('.modal-body #gvncode').val()
    }
    $.ajax({
        url: '/home/UpdateRecords',
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify({ tr: addinf }),
        contentType: 'application/json; charset=utf-8',
        success: function (status) {
            $('#mySucc').fadeOut(1);
            $('#mySucc').fadeIn(200);
        },

    });

})
myDropzone.on("success", function (file) {
    // Handle the responseText here. For example, add the text to the preview element:
    ww = JSON.parse(file.xhr.responseText);
    file.previewElement.querySelector(".more").setAttribute("data-whatever", ww.Message)
    document.querySelector("#actions .start").setAttribute("style", "visibility:hidden");
    document.querySelector("#actions .cancel").setAttribute("style", "visibility:hidden");
});
