


//Validate email and mobile no. on forget password link

//function validateEmailMobile() {
//    var userName = $("#txtUserName").val();
//    var status = true;
//    if (userName == "") {
//        $("#lblMessage").text("Please Enter Email or Mobile No.");
//        return false;
//    }
//        //else if()
//        //{
//        //    //validate email and mobile formate
//        //}
//    else {
//        $.ajax({
//            type: "POST",
//            url: "/Login/CheckEmailMobile",
//            data: "{ 'userName':'" + userName + "'}",
//            contentType: "application/json; charset = utf-8",
//            dataType: "json",
//            success: function (response) {
//                alert(response)
//                if (response.ok) {
//                    alert(response.newurl);
//                    //window.location = "ForgotPassword/ForgetPassword";
                    
//                    window.location.href = "/Login/ForgotPassword";
//                    alert("hello");
//                    //setInterval(function () { alert("hello"); }, 5000);
//                    //setTimeout(function () { alert("Hello"); }, 10000);
//                    return true;
//                }
//                else {
//                    alert("q");
//                    $("#lblMessage").text("Please enter a valid email/mobile.");
//                    return false;
//                }
//            },
//            failure: function (response) {
//                alert("q1");
//                $("#lblMessage").text("Please enter a valid email/mobile.");
//                return false;
//            },
//            error: function (response) {
//                alert("q2");
//                $("#lblMessage").text("Please enter a valid email/mobile.");
//                return false;
//            }
//        });

//    }

//}

function validateEmailMobile()
{
    var userName = $("#txtUserName").val();
    $.ajax({
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        url: '@Url.Action("SendForgotPassword", "Login")',
        data: "{ 'pUsername': " + userName + " }",

        type: "POST",
        success: function (result) {
            if (result != "True") {
                alert(1);
            }
            else {
                alert(2);
                //window.location.reload();
            }
        },
        error: function (data) {
            alert(3);
        }
    });
}