
//File Upload response from the server
Dropzone.options.dropzoneForm = {
    maxFiles: 3,
    
    init: function () {
        this.on("maxfilesexceeded", function (data) {
            var res = eval('(' + data.xhr.responseText + ')');
        });
        this.on("success", function (file, responseText) {
            // Handle the responseText here. For example, add the text to the preview element:

            ww = JSON.parse(file.xhr.responseText);

            //window.location.replace('/File/Edit?link='+ww.Message);

            $('#myModal').on('show.bs.modal', function (event) {
                
                // Extract info from data-* attributes
                // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
                // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
                var modal = $(this)
                modal.find('.modal-title').text('Файл успешно загружен')
                modal.find('.modal-body input').val(ww.Message)
            })
            $('#myModal').modal('show')
        });

    }
};







