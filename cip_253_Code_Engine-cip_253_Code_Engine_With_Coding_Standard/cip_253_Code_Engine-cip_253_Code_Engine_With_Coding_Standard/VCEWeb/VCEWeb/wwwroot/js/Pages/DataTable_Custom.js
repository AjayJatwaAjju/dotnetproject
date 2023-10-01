// Map data types to DataTable data types
const SqlToDtDataTypeMaping = {
    date: "date",
    datetime2: "date",
    datetime: "date",
    decimal: "number",
    usdcurrency: "usdcurrency",
    checkbox: "checkbox",
    drop_down: "drop_down",
    blankcheckbox: "blankcheckbox",
    uniqueidentifier: "text",
    varchar: "text",
    nvarchar: "text",
    int: "integer",
    bit: "bit",
    char: "text",
    Password: "Password",
    Phone: "Phone",
    Email: "Email",
    hidden: "hidden",
    bigint: "integer",
    "character varying": "text",
    "integer": "integer",
    "timestamp without time zone": "datetime",
    "text": "text",
    "smallint": "integer",
};

// Initialize an empty object for mapping data types
let SqlToDtDataTypeMapingList = {};
// Loop through the SqlToDtDataTypeMaping object and create a reverse mapping
$.each(SqlToDtDataTypeMaping, function (data, key) {
    SqlToDtDataTypeMapingList[key] = key;
})
// Define DataTable data types
const DatatableDataType = {
    date: "date",
    number: "number",
    usdcurrency: "usdcurrency",
    checkbox: "checkbox",
    drop_down: "drop_down",
    blankcheckbox: "blankcheckbox",
    integer: "integer",
    bit: "bit",
    actionbutton: "actionbutton",
    datatype1: "datatype1",
    datetime: "datetime",
};
// Define a function for rendering data in DataTables
dataTablerender = function (data, type, full, indx) {
    var datatype = indx.settings.aoColumns[indx.col].datatype;
    var cdata = indx.settings.aoColumns[indx.col].data;
    var sClass = indx.settings.aoColumns[indx.col].sClass;
    var rowId = full[indx.settings.rowId];
    if (data == undefined || data == null) { data = ""; }
    var calhtml = data;
    switch (datatype) {
        case DatatableDataType.date:
            {
                if (data != "" && data != "1900-01-01T00:00:00") {
                    data = FormateDate(data, 1);
                } else {
                    data = "";
                }
                calhtml = data;
                break;
            }
        case DatatableDataType.datetime:
            {
                if (data != "" && data != "1900-01-01T00:00:00") {
                    data = FormateDatetime(data, 1, 12);
                } else {
                    data = "";
                }
                calhtml = data;
                break;
            }
        case DatatableDataType.number:
            {
                if (data != "") {
                    data = '$' + Numberformat(data, '', 2);
                } else if (parseFloat(data) == 0) {
                    data = '$' + Numberformat(00, '', 2);
                } else {
                    data = "";
                }
                calhtml = data;
                break;
            }
        case DatatableDataType.integer:
            {
                if (data != "") {
                    data = Numberformat(data, '', 0);
                } else if (parseFloat(data) == 0) {
                    data = Numberformat(00, '', 0);
                } else {
                    data = "";
                }
                calhtml = data;
                break;
            }
        case DatatableDataType.usdcurrency:
            {
                if (data != "") {
                    data = '$' + Numberformat(data, '', 2);
                } else if (parseFloat(data) == 0) {
                    data = '$' + Numberformat(00, '', 2);
                } else {
                    data = "";
                }
                calhtml = data;
                break;
            }
        case DatatableDataType.checkbox:
            {
                calhtml = '<div id="divcheckbox_' + cdata + '" class="text"  ><input type="checkbox" ' + ((data == 'True' || data) ? 'checked' : '') + ' id="Checkbox_' + cdata + '" /></div>';
                break;
            }
        case DatatableDataType.datatype1:
            {
                calhtml = '<div id="divcheckbox_' + cdata + '" class="text"  ><Select id="' + rowId + 'dataTypes" class="form-control" disabled></select></div>';
                bindSelect(SqlToDtDataTypeMapingList, '#' + rowId + 'dataTypes', { "Select Type": 0 }, data);
                break;
            }
        case DatatableDataType.blankcheckbox:
            {
                calhtml = '<div id="divcheckbox_' + rowId + '" class="text"  ><input type="checkbox" id="Checkbox_' + rowId + '" onclick="checkUncheck(this)" /></div>';
                break;
            }
        case DatatableDataType.drop_down:
            {
                calhtml = data;
            }
            break;
        case DatatableDataType.actionbutton:
            {
                calhtml = '<a onClick="editEntity(\'' + data + '\',this)" class="editEntity"><i class="fa fa-edit"></i></a><a onClick="deleteEntity(\'' + data + '\')" class="deleteEntity"><i class="fa fa-trash"></i></a>';
            }
            break;
        case DatatableDataType.integer:
            {
                if (data == "1" || data) {
                    data = 'True';
                }
                else {
                    data = "False";
                }
                calhtml = data;
                break;
            }
        case DatatableDataType.bit:
            {
                if (data == "1") {
                    data = 'True';
                }
                else {
                    data = "False";
                }
                calhtml = data;
                break;
            }
            break;
    }
    return calhtml;
}
// Define a function for formatting dates
FormateDate = function (date, Mode) {
    if (date == null || date == "Invalid date" || date == "") {
        return "";
    }

    switch (Mode) {
        case 1:
            date = new Date(date);
            var vDate = date.getDate();
            var vMonth = (date.getMonth() + 1);
            date = ((vMonth < 10 ? '0' + vMonth : vMonth) + '/' + (vDate < 10 ? '0' + vDate : vDate) + '/' + date.getFullYear());
            break;
        case 2:
            date = new Date(date);
            var vDate = date.getDate();
            var vMonth = (date.getMonth() + 1);
            date = ((vMonth < 10 ? '0' + vMonth : vMonth) + '/' + (vDate < 10 ? '0' + vDate : vDate) + '/' + date.getFullYear());
            break;
        case 3:
            date = new Date(date);
            var vDate = date.getDate();
            var vMonth = (date.getMonth() + 1);
            date = (date.getFullYear() + '/' + (vMonth < 10 ? '0' + vMonth : vMonth) + '/' + (vDate < 10 ? '0' + vDate : vDate));
            break;
        case 4:
            date = new Date(date);
            var vDate = date.getDate();
            var vMonth = (date.getMonth() + 1);
            date = (date.getFullYear() + '-' + (vMonth < 10 ? '0' + vMonth : vMonth) + '-' + (vDate < 10 ? '0' + vDate : vDate));
            break;
    }
    return date;
}
// Define a function for formatting date and time
FormateDatetime = function (date, Mode, type) {
    if (date == null || date == "Invalid date" || date == "") {
        return "";
    }

    date = new Date(date);
    var outputdate = "";
    switch (Mode) {
        case 1:
            var vDate = date.getDate();
            var vMonth = (date.getMonth() + 1);
            outputdate = ((vMonth < 10 ? '0' + vMonth : vMonth) + '/' + (vDate < 10 ? '0' + vDate : vDate) + '/' + date.getFullYear());
            break;
        case 2:
            var vDate = date.getDate();
            var vMonth = (date.getMonth() + 1);
            outputdate = ((vMonth < 10 ? '0' + vMonth : vMonth) + '/' + (vDate < 10 ? '0' + vDate : vDate) + '/' + date.getFullYear());
            break;
        case 3:
            var vDate = date.getDate();
            var vMonth = (date.getMonth() + 1);
            outputdate = (date.getFullYear() + '/' + (vMonth < 10 ? '0' + vMonth : vMonth) + '/' + (vDate < 10 ? '0' + vDate : vDate));
            break;
        case 4:
            var vDate = date.getDate();
            var vMonth = (date.getMonth() + 1);
            outputdate = (date.getFullYear() + '-' + (vMonth < 10 ? '0' + vMonth : vMonth) + '-' + (vDate < 10 ? '0' + vDate : vDate));
            break;
    }
    var time = "";
    switch (type) {
        case 12:
            var vHours = date.getHours();
            if (vHours > 12) {
                time = 'PM'
                vHours = vHours - 12;
            }
            else {
                time = 'AM'
            }
            var vMinutes = date.getMinutes();
            time = ((vHours < 10 ? '0' + vHours : vHours) + ':' + (vMinutes < 10 ? '0' + vMinutes : vMinutes)) + " " + time;
            break;
        case 24:
            var vHours = date.getHours();
            var vMinutes = date.getMinutes();
            time = ((vHours < 10 ? '0' + vHours : vHours) + ':' + (vMinutes < 10 ? '0' + vMinutes : vMinutes));
            break;
    }
    return outputdate + " " + time;
}
// Define a function for formatting numbers
function Numberformat(num, prifix, decimalplace) {
    if (prifix == undefined || prifix == null) prifix = "";
    var postfix = "";
    if (num < 0) {
        prifix = '(' + prifix;
        postfix = ")"
    }
    if (num == '.0' || num == '.') {
        num = '0.0';
    }
    var p = parseFloat(num).toFixed(decimalplace).split(".");
    if (decimalplace == null || decimalplace == undefined) {
        p = num.toString().split('.')
        return prifix + p[0].split("").reverse().reduce(function (acc, num, i, orig) {
            return num == "-" ? acc : num + (i && !(i % 3) ? "," : "") + acc;
        }, "") + postfix;
    }
    else if (decimalplace == 0) {
        p = num.toString().split('.')[0]
        return prifix + parseInt(num) + postfix;
    }
    else {
        return prifix + p[0].split("").reverse().reduce(function (acc, num, i, orig) {
            return num == "-" ? acc : num + (i && !(i % 3) ? "," : "") + acc;
        }, "") + "." + p[1] + postfix;
    }
}
// Define a function for allowing only numeric input
function AllowNumeric(ctrl) {
    $("#" + ctrl.id).bind("keypress", function (event) {
        if (event.charCode != 0) {
            var regex;
            regex = new RegExp("^[0-9 ]+$");
            var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
            if (!regex.test(key)) {
                event.preventDefault();
                return false;
            }
        }
    });
}
// Define a function for validating floating-point keypress
function validateFloatKeyPress(el, evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    var number = el.value.split('.');

    if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 45) {
        return false;
    }
    //just one dot (thanks ddlab)
    if (number.length > 1 && charCode == 46) {
        return false;
    }
    if (el.value.length != 0 && charCode == 45) {
        return false;
    }
    //get the carat position
    var caratPos = getSelectionStart(el);
    var dotPos = el.value.indexOf(".");
    if (caratPos > dotPos && dotPos > -1 && (number[1].length > 1)) {
        return false;
    }
    return true;
}
// Define a function for getting the selection start position in an input element
function getSelectionStart(o) {
    if (o.createTextRange) {
        var r = document.selection.createRange().duplicate()
        r.moveEnd('character', o.value.length)
        if (r.text == '') return o.value.length
        return o.value.lastIndexOf(r.text)
    } else return o.selectionStart
}
// Define a function for converting formatted numbers back to numeric values
function UnNumberformat(number, prifix) {
    try {
        if (number) {
            if (number.toString().indexOf("(") > -1) {
                return parseFloat("-" + number.toString().replace(prifix, '').replace(/,/g, "").replace('(', '').replace(')', ''));
            }
            else
                return parseFloat(number.toString().replace(prifix, '').replace(/,/g, ""));
        }
        else {
            return "";
        }
    }
    catch (ex) {
        return 0;
    }
}
// Define a function for converting formatted integers back to numeric values
function UnIntegerformat(number, prifix) {
    try {
        if (number) {
            if (number.toString().indexOf("(") > -1) {
                return parseInt("-" + number.toString().replace(prifix, '').replace(/,/g, "").replace('(', '').replace(')', ''));
            }
            else
                return parseInt(number.toString().replace(prifix, '').replace(/,/g, ""));
        }
        else {
            return "";
        }
    }
    catch (ex) {
        return 0;
    }
}
// Define a function for validating floating-point keypress
function validateFloatKeyPress(el, evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    var number = el.value.split('.');

    if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 45) {
        return false;
    }
    //just one dot (thanks ddlab)
    if (number.length > 1 && charCode == 46) {
        return false;
    }
    if (el.value.length != 0 && charCode == 45) {
        return false;
    }
    //get the carat position
    var caratPos = getSelectionStart(el);
    var dotPos = el.value.indexOf(".");
    if (caratPos > dotPos && dotPos > -1 && (number[1].length > 1)) {
        return false;
    }
    return true;
}
// Define a function for changing the currency format on focus
function ChangeCurrecyFormatOnFocus(ctrl, prifix) {
    if ($('#' + ctrl.id).val() != "")
        $('#' + ctrl.id).val(UnNumberformat($('#' + ctrl.id).val(), prifix));
}
// Define a function for validating phone numbers
function IsValidPhone(phone) {
    if (phone.trim() != '') {
        var filter = /^((\+[1-9]{1,4}[ \-]*)|(\([0-9]{2,3}\)[ \-]*)|([0-9]{2,4})[ \-]*)*?[0-9]{3,4}?[ \-]*[0-9]{3,4}?$/;
        return filter.test(phone);
    }
    else
        return true;// returns boolean
}
// Define a function for validating email addresses
function IsValidEmail(Email) {
    if (Email.trim() != '') {
        var filter = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        return filter.test(Email);
    }
    else
        return true;// returns boolean
}