// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function bindSelect(data, ctrlid1, defaultOptions, defaultvalue) {
    //debugger;
    $(ctrlid1).find('option').remove().end();
    if (defaultOptions) {
        $.each(defaultOptions, function (key, value) {
            $(ctrlid1).append($("<option></option>").val
                (value).html(key).text(key))
        });
    }
    if (data != undefined) {
        $.each(data, function (key, value) {
            if (value)
                $(ctrlid1).append($("<option></option>").val
                    (value).html(key).text(key))
        });
    }
    if (defaultvalue) {
        $(ctrlid1).val(defaultvalue);
    }
}