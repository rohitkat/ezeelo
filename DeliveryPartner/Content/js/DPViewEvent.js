
/// <reference path="../../Scripts/js/jquery-1.7.2.min.js" />



$(function () {
    //-- Global Variables --//
    var Merchant;
    var Dispatched;
    var Delivered;
    var Returned;
    var Cancelled;


   // var IsSendEmailSMS = 0;
    //-- Change Event for OrderStatus in View/DeliveryOrderDetail/Edit.cshtml --//
    var Status = $("[name=DeliveryStatus] option:selected");
    if (Status.text() != "DELIVERED" || Status.text() != "DISPATCHED_FROM_GODOWN" || Status.text() !="RETURNED") {
        $("#pnlSendSMS").hide();
        $("#pnlMerchantCount").hide();
        $("#pnlSendSMSPrtial").hide();
    } else {
        $("#pnlSendSMS").show();
        $("#pnlMerchantCount").show();
        $("#pnlSendSMSPrtial").show();
    }
    $("[name=DeliveryStatus]").change(function () {
        $("#insertHere").html("");
        //alert($('option:selected', $(this)).index());
        var Status = $("[name=DeliveryStatus] option:selected");
       // alert(Status.val());
        if (Status.text() == "CANCELLED" || Status.text() == "RETURNED" || Status.text() == "DELIVERED" || Status.text() == "DISPATCHED_FROM_GODOWN") { //DELIVERED
            if ($('option:selected', $(this)).index()==0) {
                //alert("IN");
                $("#pnlSendSMS").hide();
                $("#pnlMerchantCount").hide();
                $("#pnlSendSMSPrtial").hide();
            }
            else
            {
            //alert($("#lblMerchantCount").html());
            var str = $("#lblMerchantCount").html(); //This Customer order having 5 Merchant(s), 1 Dispatched(s), 1 Delivered(s), 1 Returned(s) and 1 Cancelled(s) From Godown.
            var strM = str.indexOf("having ");
            strM += 7;
            var strDisp = str.indexOf("Merchant(s), ");
            strDisp += 13;
            var strDeli = str.indexOf("Dispatched, "); 
            strDeli += 12;
            var strRet = str.indexOf("Delivered, ");
            strRet += 11;
            var strCan = str.indexOf("and ");
            strCan += 4;
            Merchant = str.substring(strM, str.indexOf(" Merchant(s)"));
            Dispatched = str.substring(strDisp, str.indexOf(" Dispatched"));
            Delivered = str.substring(strDeli, str.indexOf(" Delivered"));
            Returned = str.substring(strRet, str.indexOf(" Returned"));
            Cancelled = str.substring(strCan, str.indexOf(" Cancelled"));
            
           //alert(parseInt(Merchant) + "=" + parseInt(Dispatched) + "=" + parseInt(Delivered) + "=" + parseInt(Returned) + "=" + parseInt(Cancelled))
            if (parseInt(Merchant) == 1) {
                //alert("IN");
                $("#pnlSendSMS").show();
            }
                /* else if (parseInt(Merchant) == (parseInt(Delivered) + parseInt(Dispatched)) + 1) {
                     $("#pnlSendSMS").show();
                 }*/
            else {
                //alert("Out");
                $("#pnlSendSMSPrtial").show();
            }
            $("#lblSendSMSValidation").html("");
            $("#pnlMerchantCount").show();
        }
        } else {
            $("#pnlSendSMS").hide();
            $("#pnlMerchantCount").hide();
            $("#pnlSendSMSPrtial").hide();
        }
    });

   /* $("input[name='IsSendEmailSMS']").click(function () {
        IsSendEmailSMS = $("input[name='IsSendEmailSMS']:checked").length;
    });*/
    //-- Click Event for Update Status  Button in View/DeliveryOrderDetail/Edit.cshtml --//
    $("#btnUpdateStatus").click(function (e) {
       // alert($("#hidShopOrderCode").val());
        //alert($("[name=DeliveryStatus]").val());

        $("#insertHere").html("");
        if ($('option:selected', $("[name=DeliveryStatus]")).index() == 0)
        {
            $("#insertHere").html("Please change 'Order Status'.").css("color", "red");
            //return false;
            e.preventDefault();
        }
        var Status = $("[name=DeliveryStatus] option:selected");
        if (Status.text() == "DISPATCHED_FROM_GODOWN") {
            var shopordercode = $("#hidShopOrderCode").val();
            var url = '/DeliveryOrderDetail/IsJobAssigned?ShopOrderCode=' + shopordercode;
            CallControllerActionSync(url, "IsJobAssigned");
            var IsBlank=$("#insertHere").html();
            if (IsBlank != "")
            {//return false;
                e.preventDefault();
            }

            var ordercode = $("#hidOrderCode").val();
            var url = '/DeliveryOrderDetail/IsAssignToDifferentPerson?OrderCode=' + ordercode + '&ShopOrderCode=' + shopordercode;
            CallControllerActionSync(url,"IsAssignToDifferentPerson");
            var IsBlank = $("#insertHere").html();
            if (IsBlank != "")
            { //return false;
                e.preventDefault();
            }

            var ordercode = $("#hidOrderCode").val();
            var url = '/DeliveryOrderDetail/IsDifferentDateAndScheduleAssigned?OrderCode=' + ordercode + '&ShopOrderCode=' + shopordercode;
            CallControllerActionSync(url, "IsDifferentDateAndScheduleAssigned");
            var IsBlank = $("#insertHere").html();
            if (IsBlank != "")
            {//return false;
                e.preventDefault();
            }

            //Add new for Task Type match 05-08-2016
            var ordercode = $("#hidOrderCode").val();
            var url = '/DeliveryOrderDetail/IsTaskTypeMatach?OrderCode=' + ordercode + '&ShopOrderCode=' + shopordercode;
            CallControllerActionSync(url, "IsTaskTypeMatach");
            var IsBlank = $("#insertHere").html();
            if (IsBlank != "") {//return false;
                e.preventDefault();
            }
              
       }
        var IsSendEmailSMS = $("input[name='IsSendEmailSMS']:checked").length;
        var IsSendEmailSMSPartial = $("input[name='IsSendEmailSMSPartial']:checked").length;
        //alert("IN " + parseInt(Merchant) + "=" + parseInt(Delivered) + "=" + parseInt(Dispatched) + "=" + parseInt(IsSendEmailSMS) + "=" + parseInt(IsSendEmailSMSPartial) + "=" + Status.text());
        //alert((parseInt(Delivered) + parseInt(Dispatched)) + 1);
        //alert($("input[name='IsSendEmailSMS']:checked"));
        //debugger;
        if (parseInt(Merchant) == 1 && (Status.text() == "DISPATCHED_FROM_GODOWN" || Status.text() == "DELIVERED") && (parseInt(IsSendEmailSMS) == 0 && parseInt(IsSendEmailSMSPartial) == 0)) {
            $("#lblSendSMSValidation").show();
            $("#lblSendSMSValidation").html("Please select the checkbox...");
            //return false;
            e.preventDefault();
        }
        else if (Status.text() == "DISPATCHED_FROM_GODOWN" && parseInt(Merchant) == (parseInt(Dispatched) + parseInt(Delivered) + parseInt(Returned) + parseInt(Cancelled)) + 1 && (parseInt(IsSendEmailSMS) == 0 && parseInt(IsSendEmailSMSPartial) == 0)) {
            $("#lblSendSMSValidation").show();
            $("#lblSendSMSValidation").html("Please select the checkbox...");
            //return false;
            e.preventDefault();
        }
        else if (Status.text() == "DELIVERED" && parseInt(Merchant) == (parseInt(Delivered) + parseInt(Returned) + parseInt(Cancelled) + 1) && (parseInt(IsSendEmailSMS) == 0 && parseInt(IsSendEmailSMSPartial) == 0)) {
            $("#lblSendSMSValidation").show();
            $("#lblSendSMSValidation").html("Please select the checkbox...");
            //return false;
            e.preventDefault();
        }
        else if (Status.text() == "RETURNED" && parseInt(Merchant) == (parseInt(Returned) + parseInt(Cancelled) + 1) && (parseInt(IsSendEmailSMS) == 0 && parseInt(IsSendEmailSMSPartial) == 0)) {
            $("#lblSendSMSValidation").show();
            $("#lblSendSMSValidation").html("Please select the checkbox...");
            //return false;
            e.preventDefault();
        }
        else if (Status.text() == "CANCELLED" && parseInt(Merchant) == (parseInt(Cancelled) + 1) && (parseInt(IsSendEmailSMS) == 0 && parseInt(IsSendEmailSMSPartial) == 0)) {
            $("#lblSendSMSValidation").show();
            $("#lblSendSMSValidation").html("Please select the checkbox...");
            //return false;
            e.preventDefault();
        }
        else {
            // alert("O");
            $("#lblOtpValidation").hide();
        }

        /*//var ShopOrderCode = $("#shopOrderCode").val();
        var ID = $("#ID").val();
        var OrderCode = $("#orderCode").val();

        //var url = '/DeliveryOrderDetail/IsOTPGenerated?OrderCode=' + OrderCode + '&ShopOrderCode=' + ShopOrderCode;
        var url = '/DeliveryOrderDetail/IsOTPGenerated?OrderCode=' + OrderCode + '&ID=' + ID;
        CallControllerAction(url);*/
       // debugger;
        //var data = SuccessData();
        //alert(data);
       // alert($("#countShopOrderCode").html());
       // alert($("#countShopOrderCode").text());
    });
    //-- CheckedAll Event for Assign in View/EmployeeAssignment/Index --//
    $("#CheckAll").click(function () {
        $("input[name='TaskToAssign']").attr("checked", this.checked);
    });
    //-- Single-Checked Event for Assign in View/EmployeeAssignment/Index --//
    $("input[name='TaskToAssign']").click(function () {
        if ($("input[name='TaskToAssign']").length == $("input[name='TaskToAssign']:checked").length) {
            $("#CheckAll").attr("checked", "checked");
        }
        else {
            $("#CheckAll").removeAttr("checked");
        }

    });
    //-- Click Event for Assign Button in View/EmployeeAssignment/Index --//
    $("#btnAssign").click(function (e) {
        //$("#accordion2").html = "";
        var count = $("input[name='TaskToAssign']:checked").length;
        var deliverTo = $("#DeliverTo").children("option").filter(":selected").text();
        //var employeeList = $("#EmployeeList").children("option").filter(":selected").val();//not working
        var employeeList = $("#EmployeeList").val(); //OR $("#EmployeeList option:selected").val();
        //alert(count + "=" + deliverTo + "=" + employeeList);
       // var orderCode_ShopeOrderCode = "";
        var shopOrderCode = "";
        var Status = "";
       // var shopOrderCode = "";
       // var t = $("input[name='TaskToAssign']:checked").val();
        $("input[name='TaskToAssign']:checked").each(function () {
            var values = $(this).val();
            //alert(values);
            var SOC_Status = $(this).val().split('|');

            //alert(SOC_Status[0]);
            shopOrderCode += SOC_Status[0] + ",";
            Status = SOC_Status[1];
            //alert(shopOrderCode)
            // Status = "IN_GODOWN";

            /*if (Status == "IN_GODOWN" || Status == "DISPATCHED_FROM_GODOWN") // This Stops multiple assignment for TO_CUSTOMER when Status="IN_GODOWN" OR "DISPATCHED_FROM_GODOWN"
            {
                //return false;
            e.preventDefault();
            }*/


            //shopOrderCode += values + ",";
        });
        shopOrderCode = shopOrderCode.substring(0, shopOrderCode.length - 1);
       // alert(shopOrderCode);
        // alert(Status);

        //orderCode_ShopeOrderCode = orderCode_ShopeOrderCode.substring(0, orderCode_ShopeOrderCode.length - 1);
        /*orderCode = orderCode.substring(0, orderCode.length - 1);
        shopOrderCode = shopOrderCode.substring(0, shopOrderCode.length - 1);*/

        //var params ="{EmployeeList:"+ employeeList + "," +"DeliverTo:"+ deliverTo + "," +"ID:"+ id+"}";
        //alert(employeeList + "|" + deliverTo + " " + ID + " RowSelected:" + count);
        //alert(params);
        if (count == 0) {
            //alert("No rows selected to assign");
            $("#lblAssign").html("No rows selected to assign").css("color", "red");
            //return false;
            e.preventDefault();
        }
        else if (deliverTo == "Select") {
            $("#lblAssign").html("No 'DeliverTo' selected to assign").css("color", "red");
            //return false;
            e.preventDefault();
        }
        else if (employeeList == "") {//"Select"
            $("#lblAssign").html("No 'EmployeeList' selected to assign").css("color", "red");
            //return false;
            e.preventDefault();
        }
        else if ((Status == "IN_GODOWN" || Status == "DISPATCHED_FROM_GODOWN") && deliverTo == "PICKUP") {
            $("#lblAssign").html("One of the selected item(s) having 'IN_GODOWN/DISPATCHED_FROM_GODOWN' status. Please unselect that.").css("color", "red");
            //return false;
            e.preventDefault();
        }
        else {
            // return confirm(count + " row(s) will be assigned");
            var url = '/EmployeeAssignment/Assign?EmployeeName=' + employeeList + '&DeliverTo=' + deliverTo + '&ShopOrderCode=' + shopOrderCode;//+ '&ShopeOrderCode=' + shopOrderCode;
            CallControllerAction(url);
        }
    });
    //-- Click Event for UnAssign Button in View/EmployeeAssignment/Index --//
    $("#btnUnAssign").click(function (e) {
        debugger;
        var count = $("input[name='TaskToAssign']:checked").length;
        var shopOrderCode = "";
        var assignment = "";
        //var employeeList = "";
        //var deliverTo = "";
        $("input[name='TaskToAssign']:checked").each(function () {
            var values = $(this).val();
            // alert(values);
            var SOC_Status_Assign = $(this).val().split('|');
            shopOrderCode += SOC_Status_Assign[0] + ",";

            assignment = SOC_Status_Assign[2];
           // alert(assignment)
            if (assignment == "UNASSIGN") {
                //return false;
                e.preventDefault();
            }
        });
        shopOrderCode = shopOrderCode.substring(0, shopOrderCode.length - 1);
       

        if (count == 0) {
            //alert("No rows selected to Unassign");
            $("#lblAssign").html("No rows selected to Unassign").css("color", "red");
            //return false;
            e.preventDefault();
        }
        else if (assignment == "UNASSIGN") {
             $("#lblAssign").html("One of the selected item(s) having 'Unassign' status. Please unselect that.").css("color", "red");
            //return false;
             e.preventDefault();
        }
        else {
            // return confirm(count + " row(s) will be assigned");
            var url = '/EmployeeAssignment/UnAssign?EmployeeName=' + '&ShopOrderCode=' + shopOrderCode;
            CallControllerAction(url);
        }
    });
    //-- Click Event for Clear Button of Assignment in View/EmployeeAssignment/Index --//
    $("#btnAssignClear").click(function () {
        $("#chkReturned").removeAttr("checked");
        $("#CheckAll").removeAttr("checked");
        $("input[name='TaskToAssign']").removeAttr("checked");
        $("#DeliverTo option[value='']").attr('selected', 'selected');
        $("#EmployeeList option[value='']").attr('selected', 'selected');
        $("#lblAssign").html("");
    });
    //-- Click Event for Clear Button of Search in View/EmployeeAssignment/Index --//
    $("#btnSearchClear").click(function () {
        $("#DeliveryType option[value='']").attr('selected', 'selected');
        $("#DeliveryStatus option[value='']").attr('selected', 'selected');
        $("#EmployeeListSearch option[value='']").attr('selected', 'selected');
        $("#AssignStatus option[value='']").attr('selected', 'selected');
        $("#SearchString").val("");
        $("#FromDate").val("");
        $("#ToDate").val("");
    });

    //-- Asynchronous Call --//
    function CallControllerAction(url) {
        //url: '@Url.Action("Details", "DeliveryOrderDetail")?id=' + id,
        // alert(url);
        $.ajax({
            url: url,
            data: {},//JSON.stringify(Data),
            type: "POST", //POST, GET
            async: false,//Synchronous Call for firefox. As In firefox, ajax not work for async:true
            cache: false,
            success: function (result) {
                //alert(result);
                $('#loader_img').hide();
                $("#lblAssign").html("");
               
            },
            error: function (result) {
                //alert(result);
               // $('#diliveryMemo').empty();
                $('#loader_img').hide();
                $("#lblAssign").html("");
                return false;
            }
        });
    }
    //-- Synchronous Call --//
    function CallControllerActionSync(url,mode) {
        //url: '@Url.Action("Details", "DeliveryOrderDetail")?id=' + id,
        //alert(url+"="+mode);
        $.ajax({
            url: url,
            data: {},//JSON.stringify(Data),
            type: "POST", //POST, GET
            async: false,//Synchronous Call
            cache: false,
            success: function (result) {
                //alert(result + "=" + mode);
                if (result=="True") {
                    $("#insertHere").html("");
                    return;
                }
                else if (result == "False" && mode == "IsJobAssigned") {
                    $("#insertHere").html("<p>No Job is assigned for this Customer Order. Please assign it.</p>").css("color", "red");
                    return false;
                }
                else if (result == "False" && mode == "IsAssignToDifferentPerson") {
                    $("#insertHere").html("<p>Some item(s) of this Customer order is already assigned to different person for delivery. Please assign to same person for single OTP.</p>").css("color", "red");
                    return false;
                }
                else if (result == "False" && mode == "IsDifferentDateAndScheduleAssigned") {
                    $("#insertHere").html("<p>Do not club different Date & Schedule assignment for generating single OTP. Please assign same Data & Schedule for single OTP.</p>").css("color", "red");
                    return false;
                }
                    //Add new for Task Type match 05-08-2016
                else if (result == "False" && mode == "IsTaskTypeMatach") {
                       $("#insertHere").html("<p>Please change delivery boy 'Task Type' to 'Delivery'.</p>").css("color", "red");
                return false;
            }
              
                $('#loader_img').hide();

            },
            error: function (result) {
                // $('#diliveryMemo').empty();
                $('#loader_img').hide();
                $("#insertHere").html("");
                return false;
               
            }/*,
            complete:function(result){
                
            }*/
        });
    }

});
