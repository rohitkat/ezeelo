$(function () {
    var isLogin = $("#ID").val();
    var isCaptchaWorng = $("#CaptchaWrong").val();
    //alert("1");
    if (isLogin <= 0) {

        //$("#btnLoginDetails").removeAttr('disabled');
        $("#btnLoginDetails").addClass('active');
        $("#btnShippingDetails").addClass('disabled');
        $("#btnModeDetails").addClass('disabled');
        $("#btnOrderDetails").addClass('disabled');
    }
    else {

        //Change by Harshda for subscription and corporate gift to enable only online payment option
        var Subscription = "@ViewBag.Subscription";
        var Corporategift = "@ViewBag.CorporateGift";
        if (Subscription == 2 || Corporategift == 2) {
            $("#btnLoginDetails").removeAttr('disabled');
            $("#btnShippingDetails").attr('disabled', 'disabled');
            $("#btnModeDetails").removeAttr('disabled');
            $("#btnShippingDetails").removeClass("divdisable");
            $("#btnOrderDetails").attr('disabled', 'disabled');
            $("#btnOrderDetails").removeClass("divactives").addClass("divcomplete");

            ShowModeDetails();
        }
        else if (isCaptchaWorng == 1) {
            //alert("captcha");
            $("#btnLoginDetails").addClass('active');
            $("#btnShippingDetails").addClass('active');
            $("#btnModeDetails").addClass('active');
            $("#btnShippingDetails").addClass('active');
            $("#btnOrderDetails").addClass('active');
            $("#divShippingAddress").css("display", "none");
            $("#captchaCode").val("");
            ShowModeDetails();
        }
        else {
            //alert(isLogin);
            $("#btnLoginDetails").addClass('active');
            //$("#btnLoginDetails").removeClass("divactives").addClass("divcomplete");

            $("#btnShippingDetails").addClass('active');
            // $("#btnShippingDetails").removeClass("divdisable").addClass("divactives");

            $("#btnModeDetails").addClass('disabled');
            ShowShippingAddress();

            $("#btnOrderDetails").addClass('disabled');
        }
    }
});

$(function () {
    debugger
    $("#btnLoginDetails").click(function (e) {
        if (window.location.href.indexOf("payment-process") != -1) {
            var _url = window.location.href.substring(0, window.location.href.indexOf("payment-process")) + "payment-process-login";
            history.replaceState(null, document.title, _url);
        }
        // ShowLogonDetails();

        $("#divLogonDetail").css("display", "block");
        $("#btnLoginDetails").addClass("active");

        $("#divShippingAddress").css("display", "none");
        $("#divCaptchaCode").css("display", "none");
        $("#divCartDetailsShow").css("display", "none");
        $("#addrcontent").removeAttr("style");
        e.stopPropagation();
        $("#btnShippingDetails").addClass('disabled');
        $("#btnModeDetails").addClass('disabled');
        $("#btnOrderDetails").addClass('disabled');

    });

    $("#btnShippingDetails").click(function (e) {
        //ShowShippingAddress();

        $("#divLogonDetail").css("display", "none");
        e.stopPropagation();
        $("#btnLoginDetails").addClass("active");
        $("#divShippingAddress").css("display", "block");
        $("#btnShippingDetails").addClass('active');
        // alert($("#divShippingAddress").closest().find(".content"));
        //$("#divShippingAddress").closest().find(".content").css("display", "block");
        $("#addrcontent").css("display", "block");
        $("#divCaptchaCode").css("display", "none");
        $("#divCartDetailsShow").css("display", "none");

        // $("#btnLoginDetails").addClass('active');
        // $("#btnShippingDetails").addClass('active');
        $("#btnModeDetails").addClass('disabled');
        $("#btnOrderDetails").addClass('disabled');

    });

    //$("#btnOrderDetails").click(function (e) {
    //    $("#btnModeDetails").addClass('disabled');

    //    var isExpressBuy = $("#hdnIsExpressBuy").val();

    //    var orderUrl = '@Url.Action("LoadCart", "PaymentProcess", new { IsExpressBuy = "_IsExpressBuy_" })';

    //    $('#orderdetails').load(orderUrl.replace("_IsExpressBuy_", isExpressBuy));
    //    alert(orderUrl.replace("_IsExpressBuy_", isExpressBuy));
    //    ShowOrderDetails();
    //    $("#btnOrderDetailCartNext").css("display", "block");
    //    e.stopPropagation();

    //});
});

//Yashaswi Start 02-02-2019 for BUG 11 to Verify cart 
function verifyCartBeforOrderPlace(e) {
    $.ajax({
        //url: '@Url.Action("VerifyCartOnOrderPLace", "ShoppingCart")',
        url:'ShoppingCart/VerifyCartOnOrderPLace',
        type: 'GET',
        dataType: "json",
        success: function (response) {
            if (response == 0) {
                $("#btnLoginDetails").addClass('active');
                $("#btnShippingDetails").addClass('active');
                $("#btnModeDetails").removeClass('disabled');
                $("#btnModeDetails").addClass('active');
                $("#btnOrderDetails").addClass('active');
                $("#divCaptchaCode").css("display", "block");
                e.stopPropagation();
                ShowModeDetails();
            }
            else {
                var link = $("#shoppingcart").attr('href');
                window.location.href = link;
            }
        },
        error: function (response) {

        }
    });
}

$(function () {
    $("#btnOrderDetailCartNext").click(function (e) {
        verifyCartBeforOrderPlace(e);
        //Commented by yashaswi and add thin on success call of verifyCartBeforOrderPlace
        //$("#btnLoginDetails").addClass('active');
        //$("#btnShippingDetails").addClass('active');
        //$("#btnModeDetails").removeClass('disabled');
        //$("#btnModeDetails").addClass('active');
        ////$("#btnShippingDetails").removeClass("divdisable");
        //$("#btnOrderDetails").addClass('active');
        ////$("#btnOrderDetails").removeClass('active');
        //$("#divCaptchaCode").css("display", "block");
        //e.stopPropagation();
        //ShowModeDetails();
    });
});
//Yashaswi End 02-02-2019 for BUG 11 to Verify cart 
//New added
function ChangeLoginContinue(e) {
    $("#btnLoginDetails").addClass('active');
    $("[id=btnShippingDetails]").removeClass('disabled');
    $("[id=btnShippingDetails]").addClass("active");
    e.stopPropagation();
    $("#btnModeDetails").addClass('disabled');
    ShowShippingAddress();
    $("#btnOrderDetails").addClass('disabled');
}

function ShowShippingAddress() {
    $("#divLogonDetail").css("display", "none");
    $("#divShippingAddress").css("display", "block");
    $("#divCaptchaCode").css("display", "none");
    $("#divCartDetailsShow").css("display", "none");
    //var id = $("#SelectedAddress").val();
    //var shippingUrl = '@Url.Action("Index", "ShippingAddress", new { CurrentAddressID = "_CurrentAddrID_" })';
    //$('#bindShippingAddr').load(shippingUrl.replace("_CurrentAddrID_", id));


}

function ShowModeDetails() {
    SetModeForOutstation();
}


////Add new tab to show order details
function ShowOrderDetails() {
    $("#divLogonDetail").css("display", "none");
    $("#divShippingAddress").css("display", "none");
    $("#divCaptchaCode").css("display", "none");
    $("#divCartDetailsShow").css("display", "block");

    $("#btnOrderDetails").addClass('active');

}

//========================== Use for display div of cash on delivery or online payment ===================

function DisplayView(li) {
    debugger
    var id = li.id;

    if (id == "OP") {
        $("#" + id).removeClass('Onlinepay');
        $("#" + id).addClass('Onlinepay1');
        $("#COD").removeClass('COD1');
        $("#COD").addClass('COD');
        $("#divCashNCarry").hide();
        $("#divCOD").hide();
        $("#divNetBanking").hide();
        $("#divOnlinePayment").show();
    }
    else if (id == "NB") {
        $("#divCashNCarry").hide();
        $("#divCOD").hide();
        $("#divNetBanking").show();
        $("#divOnlinePayment").hide();

    }
    else if (id == "COD") {
        $("#" + id).removeClass('COD');
        $("#" + id).addClass('COD1');
        $("#OP").removeClass('Onlinepay1');
        $("#OP").addClass('Onlinepay');
        $("#divCashNCarry").hide();
        $("#divCOD").show();
        $("#divNetBanking").hide();
        $("#divOnlinePayment").hide();

    }
    else if (id == "CC") {
        $("#divCashNCarry").show();
        $("#divCOD").hide();
        $("#divNetBanking").hide();
        $("#divOnlinePayment").hide();

    }



}

//================ Used to set value for online payment or COD
function RefreshCaptcha(e) {
    // alert("bkfgkj");
    $("#CaptchaRefresh").val("1");
    $("#captchaCode").val("");
    e.stopPropagation();
}


$(function () {
    //$("#btnRefreshCaptcha").click(function (e) {
    //    alert(e);
    //   // e.stopPropagation();
    //    $("#CaptchaRefresh").val("1");
    //    $("#captchaCode").val("");
    //    //$('input:checkbox[name=chkAddress]:checked').each(function () {
    //    //    alert($(this).parent().find("#CheckedID").val());
    //    //    window.parent.$("[id*=SelectedAddress]").val($(this).parent().find("#CheckedID").val());
    //    //});

    //    $("#divShippingAddress").attr("style", "display:none");

    //});

    $("#btnCompleteOrder").click(function () {
        if ($(".captchaCode").val() == "") {
            $(".Error").text("Please enter captcha code!!");
            $("#spnLoader").css("display", "none");
            $("#btnCompleteOrder").css("cursor", "pointer");
            return false;
        }
        else {
            $(".Error").text("");
            $("#CaptchaRefresh").val("0");
            return true;
        }
    });
});

$(function () {
    if ($("#CaptchaWrong").val() == "1") {
        $("#divCashNCarry").hide();
        $("#divCOD").show();
        $("#divNetBanking").hide();
        $("#divOnlinePayment").hide();
    }



    $("#btnMakePayment").click(function () {
        $("#IsOnlinePayment").val("1");
    });
    $("#COD").click(function () {
        $("#IsOnlinePayment").val("0");
    });

    $("[id*=btnCCAvenue]").click(function () {
        $("#IsOnlinePayment").val("2");
    });


});



