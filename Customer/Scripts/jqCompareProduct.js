var cucity = "";
var cufranchiseId = 0;////added
var str = "";

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

function SetCookiesToDiv(cookies, _url, _cmpurl) {   
    str = getCookie("CityCookie");
    if (str != null && str != "" && str != undefined) {
        var str1 = [];
        str1 = str.split('$');
        cucity = str1[1].toLowerCase();
        cufranchiseId = str1[2];////added
    }
    var categoryID;
    var categoryName;
    
    $.ajax({
        // url: _url,
        //url: "/" + cucity + "/Product/cookiesList",////hide
        url: "/" + cucity + "/" + cufranchiseId + "/Product/cookiesList",////added "/" + cufranchiseId +
        data: { 'cookiesList': cookies },
        type: "Post",
        cache: false,
        //url: "/Product/cookiesList?cookies=" + cookies,
        //contentType: "application/json; charset = utf-8",
        //dataType: "json",
        success: function (response) {
           // alert(response);
            $("#CompareProductListing ul").empty();
            $.each(response, function (index, item) {
               // alert(item.ProductID);
                var Previewurl = _url.replace("_ProductID_", item.ProductID).replace("_Name_", item.Name.toLowerCase().substring(0, 30).replace(/ /g, '-').replace('&', '%20')).replace("_cucity_", cucity).replace("_cufranchiseId_", cufranchiseId);////added .replace("_cufranchiseId_", cufranchiseId)
               //  alert(Previewurl)
                $("#CompareProductListing ul").append("<li id=" + item.ProductID + ">" +
                        "<div>" +
                        //"<a href='/PreviewItem?itemId=" + item.ProductID + "' target='_blank'>" +
                        //"<img src='" + item.ImagePath + "'/>" +
                        //"</a>" +

                        "<a href='" + Previewurl + "' target='_blank'>" +
                        "<img src='" + item.ImagePath + "' alt='" + item.Name + "'/>" +
                        "</a>" +
                        "<span style='display:none;'>" + item.ProductID + "</span>" +
                        "<a href='" + Previewurl + "' target='_blank'>" +
                        "<span>" + item.Name.substring(0, 40) + "</span>" +
                        "</a>" +
                        "</div>" +
                        "<span onclick='RemoveCompareList(" + item.ProductID + ");' class='RemoveCompareList' id=" + item.ProductID + "></span>" +
                        "</li>"
                    ); 
                var btn = $('#compare_' + item.ProductID);
                btn.css({ 'background': 'url(/../Content/Images/tags/r-arrow.png) no-repeat 10px 8px', 'color': '#026800' });
                categoryID = item.CategoryID;
                categoryName = item.CategoryName.toLowerCase().substring(0, 30).replace(/ /g, '-').replace('&', '%20');
            });

            var a = getCookie("CompareCount")
            //alert(a);
            if (a == null || a === "0") {
                $("#CompareProductButton").css('display', 'none');
                $(".CompareProductAnchore").attr('href', '#none')
                $("#compare").hide();

            }
            else {
               
                var compareUrl = _cmpurl.replace("_categoryID_", categoryID).replace("_categoryName_", categoryName).replace("_cucity_", cucity).replace("_cufranchiseId_", cufranchiseId);////added .replace("_cufranchiseId_", cufranchiseId)
                
                $("#CompareProductButton").css('display', 'block');
                //$(".CompareProductAnchore").attr('href', '../ProductCompare?categoryID=' + categoryID);
                $(".CompareProductAnchore").attr('href', compareUrl);
                $(".CompareProductAnchore").attr('target', '_blank');
                var a = getCookie("CompareCount");
                //alert(a);
                $("[id*=spanCompareProductCount]").text(a);

                $("#compare").show();
            }

        },
        failure: function (response) {
            alert("fail");
        },
        error: function (response) {
            alert("error1");
        }
    });
    //$(".RemoveCompareList").click(function () {

    //    var itemid = $(this).attr(id);

    //});
}

function RemoveCompareList(itemid) {
    var btn = $('#compare_' + itemid);
    str = getCookie("CityCookie");
    if (str != null && str != "" && str != undefined) {
        var str1 = [];
        str1 = str.split('$');
        cucity = str1[1].toLowerCase();
        cufranchiseId = str1[2];////added
    }
    $.ajax({
        type: "POST",
        //url: "/" + cucity + "/Product/DeleteFromCompareProduct?itemID=" + itemid,////hide
        url: "/" + cucity + "/" + cufranchiseId + "/Product/DeleteFromCompareProduct?itemID=" + itemid,////added "/" + cufranchiseId +
        contentType: "application/json; charset = utf-8",
        dataType: "json",
        success: function (response) {
            var a = getCookie("CompareCount");
            $("[id*=spanCompareProductCount]").text(a);
            if (a == null || a === "0") {
                $("#CompareProductButton").css('display', 'none');
                $("#CompareProductAnchore").attr('href', '#none');
                $("#compare").hide();
                btn.removeAttr('style');
            }

            else {
                $("#CompareProductButton").css('display', 'block');
                $("#compare").show();
                btn.removeAttr('style');
            }
        },
        failure: function (response) {
            alert("fail");
        },
        error: function (response) {
            alert("error");
        }
    });
    $('#' + itemid).closest("li").remove();
}

/*Add Data into Cookies for Product Comparision*/
function AddProductToCompare(ProductID, ImgPath, Name, ShopStockID, _url, _cmpurl) {
    str = getCookie("CityCookie");
    if (str != null && str != "" && str != undefined) {
        var str1 = [];
        str1 = str.split('$');
        cucity = str1[1].toLowerCase();
        cufranchiseId = str1[2];////added
    }
    //alert(cucity);
    // _url = "~/Product/AddProductInCompareList";
    //alert(_url);
    var btn = $('#compare_' + ProductID);
    var myData = {
        itemId: ProductID,
        ImgPath: ImgPath,
        ProductName: Name,
        ShopStockID: ShopStockID
    };
    //alert(JSON.stringify(myData));
    $.ajax({
        //url: "/" + cucity + "/Product/AddProductInCompareList",////hide
        url: "/" + cucity + "/" + cufranchiseId + "/Product/AddProductInCompareList",////added "/" + cufranchiseId +
        data: myData,
        type: "Post",
        cache: false,
        // type: "GET",
        // url: _url,
        //data:myData,
        // data: { 'itemId': ProductID, 'ImgPath': ImgPath, 'ProductName': Name, 'ShopStockID': ShopStockID },
        //  data: { 'itemId': ProductID},
        //contentType: "application/json; charset = utf-8",
        //dataType: "json",
        //url: "/Product/AddProductInCompareList?itemId=" + ProductID + "&ImgPath=" + ImgPath + "&Name=" + Name + "&ShopStockID=" + ShopStockID,
        //data: "{'itemId':" + ProductID + ",'ImgPath':'" + ImgPath + "','ProductName':'" + Name + "','ShopStockID':" + ShopStockID + "}",
        success: function (response) {
            //alert("success");
            if (response == "1") {
                // alert("Item Is Added In Your Compare Cart!!!");
                var vv = getCookie("ProductID");
                // alert(vv);
                var a = getCookie("CompareCount");
                $("[id*=spanCompareProductCount]").text(a);
                //$("#CompareProductListing ul").empty();
                SetCookiesToDiv(vv, _url, _cmpurl);
                btn.css({ 'background': 'url(/../Content/Images/tags/r-arrow.png) no-repeat 10px 8px', 'color': '#026800' });

            }
            else if (response == '2') {
                alert("Product Already Added");
            }
            else if (response == '3') {
                alert("Compare Product limit exceed");

            }
            else if (response == '4') {
                alert("Compare Product Category Not Similar");
            }
            else {

                alert("Unable to Add in Compare Product!!!");
                var a = getCookie("CompareCount");

                $("[id*=spanCompareProductCount]").text(a);
            }


        },
        failure: function (response) {
            alert("fail");
        },
        error: function (response) {
            alert("error");
        }
    });

}