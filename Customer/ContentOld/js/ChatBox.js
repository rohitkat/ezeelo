/// <reference path="../../Scripts/Cookie.js" />


$(function () {
    //alert("CheckBoxFunction");
    $("#chatuserlogin_div_id").hide();
    $("#chatguestlogin_div_id").hide();
    $(".chatbox").show(1000);
    $("#chatstart").hide(1000);
    $(".chatmin").click(function () {//working
        $(".chatbox").hide(1000);
        $("#chatstart").show(1000);
    });
    $("#chatstart").click(function () {//working
        $(".chatbox").show(1000);
        $("#chatstart").hide(1000);
    });
    WelcomeGuest();
    function WelcomeGuest() {
        var SessionID = $("#hdnSessionID").val();
        var CookiesID = $("#hdnCookiesID").val();
       // alert($.cookie('UserName'));
        alert("WelcomeGuest: "+SessionID+"="+CookiesID);
        if ((SessionID == null || SessionID == "") || (CookiesID == null || CookiesID == "")) {
           // $("#chatWelcome").val("Welcome - Guest");
            $("#chatWelcome").text("Welcome: Guest");
        }
        else {

        }
    }

    $(".chatsendbtn").click(function () {
        var User = $("#chatWelcome").text();
        var check = User.split('Welcome: ');
        alert(check[1]);
        if (check[1] == 'Guest') {
            $("#lblRegisterValidation").text('').css({ backgroundColor: '#FFFFFF' });
            $("#chatmain_div_id").hide();
            $("#chatguestlogin_div_id").show();
        }
        else {
            alert($("input[id=hdnUserLoginId]").val());
        }
    });
    $(".btnloginnew").click(function () {
        //alert('1');
       $("#chatmain_div_id").hide();
        $("#chatguestlogin_div_id").hide();
        $("#chatuserlogin_div_id").show();
    });


    $(".btnsubmitnew").click(function () {
        var name = $("#txtName").val();
        var mobile = $("#txtRegMobile").val();
        var email = $("#txtRegEmail").val();
        // 10 digit number with one of these four prefixes +91-, +91, 0 or : 
        // var pattern = /(\+91-?|0)?\d{10}/;
        // 10 digit number
        var mobilePattern = /^\d{10}$/;
        var namePattern = /^[a-zA-Z\s]+$/;
        var emailPattern = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;

        if (!namePattern.test(name)) {
            $("#txtName").focus();
            $("#lblRegisterValidation").text('Please Enter Alphabate').css({ backgroundColor: '#fcc' });
            return false;
        }
        else if (name.length == 0) {
            $("#txtName").focus();
            $("#lblRegisterValidation").text('Name should not be blank.').css({ backgroundColor: '#fcc' });
            return false;
        }
        else if (!(name.length >= 2 && name.length <= 50)) {
            $("#txtName").focus();
            $("#lblRegisterValidation").text('Length should be between 2-50 lenght.').css({ backgroundColor: '#fcc' });
            return false;
        }
        else if (mobile.length > 0) {
            if (!mobilePattern.test(mobile)) {
                $("#txtRegMobile").focus();
                $("#lblRegisterValidation").text('InValid Mobile Number.').css({ backgroundColor: '#fcc' });
                return false;
            }
        }
        else if (!emailPattern.test(email)) {
            $("#txtRegEmail").focus();
            $("#lblRegisterValidation").text('InValid Email.').css({ backgroundColor: '#fcc' });
            return false;
        }
        else if (mobile.length == 0 && email.length == 0) {
            $("#txtRegMobile").focus();
            $("#lblRegisterValidation").text('Mobile/Email should not be blank.').css({ backgroundColor: '#fcc' });
            return false;
        }
       /* alert(mobile + " " + email);
        mobile = mobile.substring(1, mobile.length - 1);
        email = email.substring(1, email.length - 1);
        alert(mobile + " " + email);*/
        var url = '~/Chat/CheckChatUserRegistration?mobile=' + mobile + '&email=' + email;
        //'/DeliveryOrderDetail/IsAssignToDifferentPerson?OrderCode=' + ordercode + '&ShopOrderCode=' + shopordercode;
      
       // var url = '/Chat/Test';
        CallControllerAction(url,'Register');
        var IsBlank = $("#lblRegisterValidation").html();
        if (IsBlank != "")
        { return false; }
        else {
            var url = '~/Chat/RegisterChatUser?chatname=' + name + '&mobile=' + mobile + '&email=' + email;
            CallControllerAction(url, 'Register');
        }
        $("#lblRegisterValidation").text('').css({ backgroundColor: '#FFFFFF' });
        $("#chatuserlogin_div_id").hide();
        $("#chatguestlogin_div_id").hide();
        $("#chatmain_div_id").show();

       var chatUserName= $("input[id=hdnUserLoginId]").val()
       $("#chatWelcome").text("Welcome: " + chatUserName);


    });
    $("#btn_chatuserlogin").click(function () {
        //alert('3');
        $("#lblLoginValidation").text('').css({ backgroundColor: '#FFFFFF' });
        var mobile = $("#txtLoginMobile").val();
        var email = $("#txtLoginEmail").val();
        var url = '~/Chat/CheckChatUserRegistration?mobile=' + mobile + '&email=' + email;
        CallControllerAction(url,'Login');
        var IsBlank = $("#lblLoginValidation").html();
        if (IsBlank != "")
        { return false; }

        $("#chatguestlogin_div_id").hide();
        $("#chatuserlogin_div_id").hide();
        $("#chatmain_div_id").show();
    });


    //-- synchronous Call --//
    function CallControllerAction(url,mode) {
        //url: '@Url.Action("Details", "DeliveryOrderDetail")?id=' + id,
         alert(url);
        $.ajax({
             url: url,
            type: "POST", //POST, GET
            async: false,
            cache: false,
            //datatype:'json',
            success: function (result) {
                alert('Successq: ' + result);
                debugger;
                alert(result.Name);
                if (result.Name != "False" && mode == 'Register') {
                    $("input[id=hdnUserName]").val(result.Name);
                    $("input[id=hdnUserLoginId]").val(result.UserLoginId);
                    $("#lblRegisterValidation").text('Mobile/Email already exist. Please login').css({ backgroundColor: '#fcc' });
                    return false;
                }
               /* else if (result == "False" && mode == 'Register') {
                    // $("#lblRegisterValidation").text('Not exist.').css({ backgroundColor: '#fcc' });
                    return;
                }*/
                else if (result.Name == "False" && mode == 'Login') {
                    $("#lblLoginValidation").text('Mobile/Email not exist.').css({ backgroundColor: '#fcc' });
                    return false;
                }
                else if (result.Name != "False" && mode == 'Login') {
                    $("#chatWelcome").text("Welcome: " + result.Name);
                    $("input[id=hdnUserLoginId]").val(result.UserLoginId);
                   // alert($("input[id=hdnUserLoginId]").val());
                    return false;
                }
                else {
                    $("#lblLoginValidation").text('').css({ backgroundColor: '#FFFFFF' });
                    $("#lblRegisterValidation").text('').css({ backgroundColor: '#FFFFFF' });

                    return true;
                }
              //  $('#loader_img').hide();
              //  $("#lblAssign").html("");

            },
            error: function (result) {
               // alert('Failed: '+result.toString());
                // $('#diliveryMemo').empty();
              //  $('#loader_img').hide();
                //  $("#lblAssign").html("");
                $("#lblRegisterValidation").text('Some Error.').css({ backgroundColor: '#fcc' });
                $("#lblLoginValidation").text('Some Error.').css({ backgroundColor: '#fcc' });
                return false;
            }
        });
    }

});
