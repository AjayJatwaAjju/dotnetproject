// Calling this function to get script
function GetScript(DatabaseType, FileName, cb_func) {
    $.ajax({
        type: "Get",
        url: "api/getdata/Scriptfile?DatabaseType=" + DatabaseType + "&FileName="
            + FileName,
        success: cb_func,
        error: function (request, error) {
            alert('An error occurred attempting to get new e-number');
            // console.log(request, error);
        }
    });
}
// Using this function for get file details
function GetFileDetail(FileName, cb_func) {
    $.ajax({
        type: "Get",
        url: "api/getdata/FileString?FileName="
            + FileName,
        success: cb_func,
        error: function (request, error) {
            alert('An error occurred attempting to get new e-number');
            // console.log(request, error);
        }
    });
}