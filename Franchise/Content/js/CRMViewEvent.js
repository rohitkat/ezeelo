/// <reference path="jquery-1.9.1.min.js" />

$(function () {
    
    //-- Change Event for OrderStatus in View/CustomerOrderDetailCall/Create --//
    var Status = $("[name=OrderStatus] option:selected");
    if (Status.text() != "DELIVERED") {
    $("#pnlDelevered").hide();
    } else {
        $("#pnlDelevered").show();
    }
    $("[name=OrderStatus]").change(function () {
        
        var Status = $("[name=OrderStatus] option:selected");
        //alert(Status.text());
        if (Status.text() == "DELIVERED") { //DELIVERED
            $("#pnlDelevered").show();
           // $("#pnlDELIVERED").css( 'display', 'block' );
            $("#lblOtpValidation").html("");
        } else {
            $("#pnlDelevered").hide();
        }
    });
    //-- End --//

    //-- Click Event for Create button in View/CustomerOrderDetailCall/Create --//
    $("#btnSave").click(function () {
        var Status = $("[name=OrderStatus] option:selected");
        var PaymentMode = $("[name=PaymentMode]").val();
       // PaymentMode = "ONLINE";
        //if (Status.text() == "DELIVERED" && $("[name=PaymentMode]").val() == "COD" && $("[name=OTP").val() == "" && $("#lblOTP").html() != "OTP Not Generated") {
        if (Status.text() == "DELIVERED" && (PaymentMode == "ONLINE") && $("[name=OTP").val() == "" && $("#lblOTP").html() != "OTP Not Generated") {
            $("#lblOtpValidation").show();
            $("#lblOtpValidation").html("Please enter OTP...");
            return false;
        }
        else {
            $("#lblOtpValidation").hide();
        }
    });
    //-- End --//


});