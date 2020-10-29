
//function AddPlan(id) {


//    var SubPlanID = id;
//    $.ajax({
//        url: '@Url.Action("CheckPlan", "SubscriptionPlan")',
//        data: { 'SubPlanID': SubPlanID },
//        type: "Post",
//        cache: false,
//        success: function (data) {
//            //alert(data);
//            if (data.ok == false) {
//                window.location = data.newurl;
//                return;
//            }
//            if (data.ok) {
//                //alert("Please Login to Continue");
//                window.location = data.newurl;
//                return;
//            }
//            else if (data == 1) {
//                PaymentProcess(id, 1);
//                //Commented by Ansari for static coupon
//                //var msg = $("[id*='" + id + "']").closest('tr').find('.savemsg');
//                //var msg = $(".savemsg");
//                //$(msg).text("You are already subscribed.");
//                //$(msg).show();
//                //setTimeout(function () { $(msg).hide(); }, 5000);
//            }
//            else if (data == 2) {
//                if (!confirm('Already a plan is assigned,Click yes to Override')) { //if click on cancel
//                    return;
//                }
//                else {
//                    MakeOtherPlanDeactive(id);


//                }
//            }
//            else if (data == 4) {
//                PaymentProcess(id, 2);
//            }
//        },
//        error: function (data) {

//        }
//    });


//}


function PaymentProcess(id, AlreadyExist) {
    SubPlanID = id;
   // alert(id + " " + AlreadyExist);
    $.ajax({
        url: '@Url.Action("PaymentProcess", "SubscriptionPlan")',
        data: "{'SubPlanID':" + SubPlanID + ",'AlreadyExist':" + AlreadyExist + "}",
        //data: { 'SubPlanID': SubPlanID, 'AlreadyExist': AlreadyExist },
        type: "Post",
        cache: false,
        success: function (data) {
            if (data.ok) {
                window.location = data.newurl;
            }
        },
        error: function (data) {

        }
    });
}

function MakeOtherPlanDeactive(id) {
    $.ajax({
        url: '@Url.Action("MakeOtherPlanDeactive", "SubscriptionPlan")',
        data: {},
        type: "Post",
        cache: false,
        success: function (data) {
            PaymentProcess(id);
            //SavePlan(id);
        },
        error: function (data) {

        }
    });
}

function SavePlan(id) {
    var SubPlanID = id;
    $.ajax({
        url: '@Url.Action("SaveCustSubscriptionPlan", "SubscriptionPlan")',
        data: { 'SubPlanID': SubPlanID },
        type: "Post",
        cache: false,
        success: function (data) {

            var msg = $("[id*='" + id + "']").closest('tr').find('.savemsg');
            $(msg).text(data);
            $(msg).show();
            setTimeout(function () { $(msg).hide(); }, 5000);
        },
        error: function (data) {

        }
    });
}