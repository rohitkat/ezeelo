
var specialKeys = new Array();
specialKeys.push(8); //Backspace
function IsNumeric(e) {
    var keyCode = e.which ? e.which : e.keyCode
    var ret = ((keyCode >= 48 && keyCode <= 57) || specialKeys.indexOf(keyCode) != -1);
    //document.getElementById("error").style.display = ret ? "none" : "inline";
    return ret;
}





function SendAppLink()
{
    
    var mobile = $(".mobileInput").val();
    //alert(mobile);
    if (mobile != "") {



        var filter = /^[0-9-+]+$/;
        if (filter.test(mobile)) {
            $.ajax({
                type: "POST",
                url: "/Home/SendSMSForAppLink",
                data: "{'mob':" + mobile + "}",
                contentType: "application/json; charset = utf-8",
                dataType: "json",
                success: function (response) {
                    var msg = $(".appLinkMsg");
                    $(msg).text("Thanx for your interest! Enjoy local market at your fingertip");
                    $(msg).show();
                    setTimeout(function () { $(msg).hide(); }, 4000);
                    // $(".appLinkMsg").text("Thanx for your interest! Enjoy local market at your fingertip");
                    setTimeout(function () { $("#boxes").hide(); }, 5000);

                },
                failure: function (response) {
                    var msg = $(".appLinkMsg");
                    $(msg).text("Thanx for your interest! Enjoy local market at your fingertip");
                    $(msg).show();
                    setTimeout(function () { $(msg).hide(); }, 4000);
                    setTimeout(function () { $("#boxes").hide(); }, 5000);
                },
                error: function (response) {
                    var msg = $(".appLinkMsg");
                    $(msg).text("Thanx for your interest! Enjoy local market at your fingertip");
                    $(msg).show();
                    setTimeout(function () { $(msg).hide(); }, 4000);
                    setTimeout(function () { $("#boxes").hide(); }, 5000);
                }
            });

        }
        else {
            $(".appLinkMsg").text("Please!Enter Valid mobile no.");
        }
    }
    else
    {
        $(".appLinkMsg").text("Please!Enter mobile no.");
    }
}