
function AddProductToShortList(id,_url) {
    
    var prodId = id.id.split('_')[1];
   
    var mydata = {
        "shopStockID": $("[id=" + id.id + "]").parent().find("#hdnShopStockID").val(),
        "productName": $("[id=" + id.id + "]").parent().find("#hdnProductName").val(),
        "productID": prodId
    };
    var productID = id.id;
    $.ajax({
        type: "POST",
        //url: "/PreviewItem/AddProductToShortList",
        url: _url,
        data: "{'myParam':" + JSON.stringify(mydata) + "}",
        contentType: "application/json; charset = utf-8",
        dataType: "json",
        success: function (response) {
            //$("[id*=" + id.id + "]").addClass('mif-star-full button activeListNew');
            $("[id=" + id.id + "]").addClass('activeListNew');
           
            var a = getCookie("ShortListCookie");
            
            $("[id*=spanShortlistCount]").text(a.split(',').length);
            //RenderPartialView("_ShortListItemPartial");
            // $("#ShortListDiv").load('_ShortListItemPartial.html');

        },
        failure: function (response) {
            $("[id=" + id.id + "]").addClass('activeListNew');
            //$("#anchorShortList").addClass('activeListNew');
        },
        error: function (response) {
            $("[id=" + id.id + "]").addClass('activeListNew');
            //$("#anchorShortList").addClass('activeListNew');
        }
    });
}

function getCookie(name) {
    var dc = document.cookie;
    var prefix = name + "=";
    var begin = dc.indexOf("; " + prefix);
    if (begin == -1) {
        begin = dc.indexOf(prefix);
        if (begin != 0) return null;
    } else {
        begin += 2;
    }
    var end = document.cookie.indexOf(";", begin);
    if (end == -1) {
        end = dc.length;
    }
    return unescape(dc.substring(begin + prefix.length, end));
}


//This function add product in shortlist from home page
//Parameter : id ==> contains productId + shopstockid + productName

function AddProductToShortListFromHome(id) {
    var prodId = id.id.split('#')[0];
     
    var mydata = {
        "shopStockID": id.id.split('#')[1],
        "productName": id.id.split('#')[2],
        "productID": prodId
    }; 
    var productID = id.id;
    var str = getCookie("CityCookie");
    var cucity = "";
    var cufranchiseId = 0;
    if (str != null && str != "" && str != undefined) {
        var str1 = [];
        str1 = str.split('$');
        cucity = str1[1].toLowerCase();
        cufranchiseId = str1[2];////added
    }
    $.ajax({
        type: "POST",
        //url: "/PreviewItem/AddProductToShortList",////hide
        url: "/" + cucity + "/" + cufranchiseId + '/PreviewItem/AddProductToShortList',////added cufranchiseId
        data: "{'myParam':" + JSON.stringify(mydata) + "}",
        contentType: "application/json; charset = utf-8",
        dataType: "json",
        success: function (response) {
            var a = getCookie("ShortListCookie");
            $("[id*=spanShortlistCount]").text(a.split(',').length);
            $("[id=" + id.id + "]").addClass('active11');
        },
        failure: function (response) {
            $("[id=" + id.id + "]").addClass('active11');
            //$("#anchorShortList").addClass('activeListNew');
        },
        error: function (response) {
            $("[id=" + id.id + "]").addClass('active11');
            //$("#anchorShortList").addClass('activeListNew');
        }
    });
}












//function AddProductToShortList(id) {
//    var productID = id.id;
//    $.ajax({
//        type: "POST",
//        url: "/PreviewItem/AddProductToShortList",
//        data: '{ "productID":' + productID + ', "productName":"' + $("[id=" + id.id + "]").parent().find("#hdnProductName").val() + '" }',
//        contentType: "application/json; charset = utf-8",
//        dataType: "json",
//        success: function (response) {
//            //$("[id*=" + id.id + "]").addClass('mif-star-full button activeListNew');
//            $("[id=" + id.id + "]").addClass('activeListNew');
//            //RenderPartialView("_ShortListItemPartial");
//           // $("#ShortListDiv").load('_ShortListItemPartial.html');

//        },
//        failure: function (response) {
//            $("[id=" + id.id + "]").addClass('activeListNew');
//            //$("#anchorShortList").addClass('activeListNew');
//        },
//        error: function (response) {
//            $("[id=" + id.id + "]").addClass('activeListNew');
//            //$("#anchorShortList").addClass('activeListNew');
//        }
//    });
//}






//function AddProductToShortList(prodID,name) {
//    alert(prodID);
//    //var productID = id.split('#')[0];
    
//    $.ajax({
//        type: "POST",
//        url: "/PreviewItem/AddProductToShortList",
//        //data: '{ "productID":' + productID + ', "productName":"' + $("#hdnProductName").val() + '" }',
//        data: '{ "productID":' + prodID + ', "productName":"' + name + '" }',
//        contentType: "application/json; charset = utf-8",
//        dataType: "json",
//        success: function (response) {
//            $("#anchorShortList").addClass('mif-star-full button activeListNew');

//            //RenderPartialView("_ShortListItemPartial");
//           // $("#ShortListDiv").load('_ShortListItemPartial.html');

//        },
//        failure: function (response) {
//             //$("[id*=" + id + "]").addClass('mif-star-full button activeListNew');
//           $("#anchorShortList").addClass('activeListNew');
//        },
//        error: function (response) {
//            $("[id*=" + id + "]").addClass('mif-star-full button activeListNew');
//            //$("#anchorShortList").addClass('activeListNew');
//        }
//    });
//}