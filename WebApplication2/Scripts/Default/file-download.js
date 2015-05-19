$("#downloadLink").click(function () {
   
    var pt = document.getElementById("hidDocumentID").innerText;
    document.location.href = '/Save/' +pt;

});

$(document).ready(function () {

    var pt = document.getElementById("hidDocumentID2").innerText;
    if (pt.indexOf('image') != -1) {
        $("#lowpriority2").show();
    }
    else {
        if (pt.indexOf('audio') != -1) {
            $("#lowpriority").show();
        }
        else {
            if (pt.indexOf('video') != -1) {

            }
        }
    }


});