

function ValidateNumber(e) {
    var evt = (e) ? e : window.event;
    var charCode = (evt.keyCode) ? evt.keyCode : evt.which;
    if ((charCode > 45 && charCode < 58) || (charCode > 95 && charCode < 106) || charCode == 8 || charCode == 127 || charCode == 37 || charCode == 39) { // (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return true;
    }
    else if (charCode == 9) {
        SendKeys("{tab}");
        return true;
    }
    return false;
};

function ValidateName(e) {
    var evt = (e) ? e : window.event;
    var charCode = (evt.keyCode) ? evt.keyCode : evt.which;
    //if ((charCode >= 65 && charCode <= 90) || (charCode >= 97 && charCode <= 122) || charCode == 8 || charCode == 127 || charCode == 37 || charCode == 39 ) { // (charCode > 31 && (charCode < 48 || charCode > 57)) {
    if ((charCode >= 65 && charCode <= 90) || charCode == 8 || charCode == 127 || charCode == 37 || charCode == 39) { // (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return true;
    }
    else if (charCode == 9) {
        SendKeys("{tab}");
        return true;
    }
    return false;
};

function ValidateAlphaNumeric(e) {
    var evt = (e) ? e : window.event;
    var charCode = (evt.keyCode) ? evt.keyCode : evt.which;
    if ((charCode >= 65 && charCode <= 90) || (charCode > 45 && charCode < 58) || (charCode > 95 && charCode < 106) || charCode == 8 || charCode == 127 || charCode == 37 || charCode == 39 || charCode == 189 || charCode == 109) { // (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return true;
    }
    else if (charCode == 9) {
        SendKeys("{tab}");
        return true;
    }
    return false;
};

function ValidateEmail(obj) {
    var email = $(obj).val();
    var filter = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    $('#divEmailError').hide();
    if (filter.test(email)) {
        return true;
    }
    else {
        $('#divEmailError').show();
        $('#divEmailError').html('Invalid Email [' + email + ']');
        $('#userLogin_Email').val('');
        $('#userLogin_Email').focus();
        return false;
    }
}

function ValidateMobile(obj) {
    var mobile = $(obj).val();
    var filter = /^[7-9]{1}[0-9]{9}$/;
    $('#divMobileError').hide();
    if (filter.test(mobile)) {
        return true;
    }
    else {
        $('#divMobileError').show();
        $('#divMobileError').html('Invalid Mobile [' + mobile + ']');
        $('#Mobile').val('');
        $('#Mobile').focus();
        return false;
    }
}

function ValidatePincode() {
    var pin = $('#Pincode').val();
    var filter = /\d{6}/;
    $('#divPincodeError').hide();
    if (filter.test(pin)) {
        return true;
    }
    else {
        $('#divPincodeError').show();
        $('#divPincodeError').html('Invalid Pincode [' + pin + ']');
        $('#Pincode').val('');
        $('#Pincode').focus();
        return false;
    }
}

function ValidateConfirmPassword(obj) {
    var pwd = $('#userLogin_Password').val();
    var cpwd = $('#ConfirmPassword').val();
    $('#divPwdError').hide();
    if (pwd == cpwd)
    { return true; }
    else
    {
        $('#divPwdError').show();
        $('#divPwdError').html('Invalid Confirm Password');
        $(obj).val('');
        //$(obj).select();
        $(obj).focus();
        return false;
    }
};

function setFocus() {
    var pwd = $('#userLogin_Password').val();
    $('#divPwdError').hide();
    if (pwd == '') {
        $('#ConfirmPassword').val('');
        $('#userLogin_Password').focus();
    }
    else {
        $('#ConfirmPassword').val('');
        $('#ConfirmPassword').focus();
    }
};

//for employee
function ValidateConfirmPassword1(obj) {
    var pwd = $('#Password').val();
    var cpwd = $('#ConfirmPassword').val();
    $('#divPwdError').hide();
    if (pwd == cpwd)
    { return true; }
    else
    {
        $('#divPwdError').show();
        $('#divPwdError').html('Invalid Confirm Password');
        $(obj).val('');
        //$(obj).select();
        $(obj).focus();
        return false;
    }
};