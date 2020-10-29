//================================= Load products on index count =============================================

var routeUrl = "";
//============== Call function on Document.ready ==========================
function GetProducts(_url, sortVal, fromSorting) {
    routeUrl = _url;
    $(".loader").show();
    var mydata = {
        "cityId": $("#CityID").val().split('$')[0],
        "franchiseId": $("#CityID").val().split('$')[2],////added
        "keyword": $("#Keyword").val(),
        "shopId": $("#ShopID").val(),
        "categoryId": $("#ParentCategory").val(),
        "brands": $("#Brands").val(),
        "colors": $("#Colors").val(),
        "sizes": $("#Sizes").val(),
        "dimensions": $("#Dimensions").val(),
        "catDescID": $("#SpecificationIDs").val(),
        "catDescValue": $("#SpecificationValues").val(),
        "minPrice": $("#MinPrices").val(),
        "maxPrice": $("#MaxPrices").val(),
        "pageIndex": 1,
        "pageSize": 24,
        "FromPaging": false,
        "SortVal": sortVal,
        "FromSorting": fromSorting
    };
    $.get(_url, mydata, function (data) {
        /* data is the pure html returned from action method, load it to your page */
        $('#ProductListing').html(data);

        /* little fade in effect */
        $('#ProductListing').fadeIn('fast');
        $(".loader").hide();
    });
}

//============== Call function on Page Index ==========================

function GetProducts1111(id) {
    //alert($("#id").find("#ProductListing").html());
    var sortval = $('#SortList option:selected').val();
    //alert(sortval);
    $(".loader").show();
    var mydata = {
        "cityId": $("#CityID").val(),
        "franchiseId": $("#FranchiseID").val(),////added
        "keyword": $("#Keyword").val(),
        "shopId": $("#ShopID").val(),
        "categoryId": $("#ParentCategory").val(),
        "brands": $("#Brands").val(),
        "colors": $("#Colors").val(),
        "sizes": $("#Sizes").val(),
        "dimensions": $("#Dimensions").val(),
        "catDescID": $("#SpecificationIDs").val(),
        "catDescValue": $("#SpecificationValues").val(),
        "minPrice": $("#MinPrices").val(),
        "maxPrice": $("#MaxPrices").val(),
        "pageIndex": id.id,
        "pageSize": 24,
        "FromPaging": true,
        "SortVal": sortval

    };
    $.get(routeUrl, mydata, function (data) {
        /* data is the pure html returned from action method, load it to your page */
        $('#ProductListing').html(data);

        /* little fade in effect */
        $('#ProductListing').fadeIn('fast');
        $(".loader").hide();
    });
    //});
}
$("[id*=SortList]").change(function () {
    //var _url = '@(Html.Raw(Url.Action("GetProductOnScroll", "Product")))';
    GetProducts(routeUrl, $('#SortList option:selected').val(), true);
    //$("#SortList").val($('#SortList option:selected').val());
});

//$("[id*=SortList]").change(function () {
//    alert("Handler for .change() called.");
//});







//});
//$(window).load(function () {

//    var a = getCookie("ProductID");

//    if (a != null) {
//        SetCookiesToDiv(a);

//    }


//});
//$(function () {
//    var str = $("#ulProduct").length;
//    //alert(str);
//    if (str <= 0) {
//        $(".charm").css("display", "none");
//    }
//    else {
//        $(".charm").css("display", "block");
//    }
//});
//$(window).scroll(function () {
//    if ($(this).scrollTop() > 10) {
//        $("#compare").addClass('compareTop');
//    } else {
//        $("#compare").removeClass('compareTop');
//    }
//})



$(function () {
    // Set cookie for checking which refinement is chedked
    $(".brandCheckBox").click(function () {
        setCookie("CheckCookie", "BRAND", 30);
    });
    $(".colorCheckBox").click(function () {
        setCookie("CheckCookie", "COLOR", 30);
    });
    $(".sizeCheckBox").click(function () {
        setCookie("CheckCookie", "SIZE", 30);
    });
    $(".dimensionCheckBox").click(function () {
        setCookie("CheckCookie", "DIMENSION", 30);
    });
    $(".detailHeadCheckBox").click(function () {
        setCookie("CheckCookie", "CDH", 30);
    });
    $(".priceCheckBox").click(function () {
        setCookie("CheckCookie", "PRICE", 30);
    });

    // Set cookie for checking which refineement needs to be clear
    // var model =@Html.Raw(Json.Encode(Model))

    $(".clearBrand").click(function () {
        setCookie("CheckCookie", "C_BRAND", 30);
        $('#tblBrand input:checkbox').attr('checked', false);
        $("form").submit();
    });
    $(".clearColor").click(function () {
        setCookie("CheckCookie", "C_COLOR", 30);
        $('#tblColor input:checkbox').attr('checked', false);
        $("form").submit();
    });
    $(".clearSize").click(function () {
        setCookie("CheckCookie", "C_SIZE", 30);
        $('#tblSize input:checkbox').attr('checked', false);
        $("form").submit();
    });
    $(".clearDimension").click(function () {
        setCookie("CheckCookie", "C_DIMENSION", 30);
        $('#tblDimension input:checkbox').attr('checked', false);
        $("form").submit();
    });
    $(".clearDetailHead").click(function () {
        setCookie("CheckCookie", "C_CDH", 30);
        $(this).parent().parent().parent().find('#tblSubSpecification input:checkbox').attr('checked', false);
        $("form").submit();
    });
    $(".clearPrice").click(function () {
        setCookie("CheckCookie", "C_PRICE", 30);
        $('#tblPrice input:checkbox').attr('checked', false);
        $("form").submit();
    });

});

function setCookie(c_name, value, expiredays) {
    document.cookie = c_name + "=" + value;
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



//========================================= Tejaswee ==========================================================
///Login changed by yashaswi
function AddProductToShppingCart(shopStockId, itemId, _url) {

    var str = getCookie("CityCookie");
    var cucity = "";
    var cufranchiseId = 0;////added
    if (str != null && str != "" && str != undefined) {
        var str1 = [];
        str1 = str.split('$');
        cucity = str1[1].toLowerCase();
        cufranchiseId = str1[2];////added
    }
    else {
        cucity = "kanpur";//Yashaswi 01/12/2018 Default City Change 
        cufranchiseId = 1052;//Yashaswi 01/12/2018 Default City Change 
    }
    var custSessionId = $("#hdnSessionValue").val();
    //alert( "temp "+custSessionId);

    var quantity = $('#' + itemId).parent().find('.productQuantity').val();
    if (quantity == 0 || quantity == "") {
        quantity = 1;
    }
    if (custSessionId == null || custSessionId == "" || custSessionId == 0) {
        // alert("You are not logged in! Please log in to continue....." + cucity);
        // window.location.href = '/' + cucity + '/login?callfrom=cart/' + shopStockId + '/' + itemId + '/' + quantity;////hide
        window.location.href = '/' + cucity + '/' + cufranchiseId + '/login?callfrom=cart/' + shopStockId + '/' + itemId + '/' + quantity;////added
    }
    else {

        $.ajax({
            type: "POST",
            url: _url, //"/Product/AddProductInShoppingCart",
            data: "{'shopStockId':" + shopStockId + ",'itemId':" + itemId + ",'quantity':" + quantity + "}",
            contentType: "application/json; charset = utf-8",
            dataType: "json",
            success: function (response) {
                if ($('#QtyValidationMsg').length > 0) {
                    $("#QtyValidationMsg").text("");
                    $("#QtyValidationMsg").hide();
                }
                if (response == "2") {
                    if ($('#QtyValidationMsg').length > 0) {
                        $("#QtyValidationMsg").text("Item Is Already Added In Your Cart!!!");
                        $("#QtyValidationMsg").show();
                    }
                    //$("[id*=shoppingCartMessage]").text("Item is already added in your cart");
                    alert("Item Is Already Added In Your Cart!!!");
                }
                
                else if (response == "3") { //Product is out of stock
                    if ($('#QtyValidationMsg').length > 0) {
                        $("#QtyValidationMsg").text("Sorry! Product is out of stock.");
                        $("#QtyValidationMsg").show();
                    }
                    alert("Sorry! Product is out of stock.");
                    window.location.reload();
                }
                else if (response.indexOf("#") > -1) { //current available quantity is less than required quantity
                    var avalQty = response.replace("#", "");
                    if ($('#QtyValidationMsg').length > 0) {
                        $("#QtyValidationMsg").text("We're sorry! We are able to accommodate only " + avalQty + " units of this product.");
                        $("#QtyValidationMsg").show();
                    }
                    alert("We're sorry! We are able to accommodate only " + avalQty + " units of this product.");
                    if ($('#productQuantity').length > 0) {
                        $("#productQuantity").val(avalQty);
                    }
                    if ($('#txtQuantity').length > 0) {
                        $("#txtQuantity").val(avalQty);
                    }
                }
                else if (response.indexOf("$") > -1) {
                    var msg = response.replace("$", "");
                    if ($('#QtyValidationMsg').length > 0) {
                        $("#QtyValidationMsg").text(msg);
                        $("#QtyValidationMsg").show();
                    }
                    alert(msg);
                }
                else {
                    var a = getCookie("ShoppingCartCookie");
                    $("[id*=spanShoppingCartCount]").text(a.split(',').length);

                    $('#myAlert').css('display', 'block');
                    setTimeout(function () {
                        $("#myAlert").hide('blind', {}, 300)
                    }, 1000);
                }

                //if ($("[id*=spanShoppingCartCount]").text == "") {
                //    $("[id*=spanShoppingCartCount]").text(1);
                //}
                //else {
                //    var currentValue = $("[id*=spanShoppingCartCount]").text();
                //    $("[id*=spanShoppingCartCount]").text(parseInt(currentValue) + 1);
                //}
            },
            failure: function (response) {
                // alert("hello failuer");
            },
            error: function (response) {
                //alert("hello error");
            }
        });
    }
}

function AddProductToShppingCartFromPreview(shopStockId, itemId, couponCode, couponAmount, couponPercent) {
    var str = getCookie("CityCookie");
    var cucity = "";
    var cufranchiseId = 0;////added
    if (str != null && str != "" && str != undefined) {
        var str1 = [];
        str1 = str.split('$');
        cucity = str1[1].toLowerCase();
        cufranchiseId = str1[2];////added
    }
    else {
        cucity = "kanpur";//Yashaswi 01/12/2018 Default City Change 
        cufranchiseId = 1052;//Yashaswi 01/12/2018 Default City Change 
    }
    var custSessionId = $("#hdnSessionValue").val();
    // alert("5555");
    if (custSessionId == null || custSessionId == "" || custSessionId == 0) {
        // alert("You are not logged in! Please log in to continue....." + cucity);
        //window.location.href = '/' + cucity + '/login?callfrom=cart/' + shopStockId + '/' + itemId + '/1';////hide
        window.location.href = '/' + cucity + '/' + cufranchiseId + '/login?callfrom=cart/' + shopStockId + '/' + itemId + '/1';////added
    }
    else {
        if (couponAmount == "") {
            couponAmount = "0";
        }
        if (couponPercent == "") {
            couponPercent = "0";
        }
        //alert("1");
        $.ajax({
            type: "POST",
            url: "/Product/AddProductInShoppingCartFromPreview",
            data: "{'shopStockId':" + shopStockId + ",'itemId':" + itemId + ",'couponCode':'" + couponCode + "','couponAmount':" + couponAmount + ",'couponPercent':" + couponPercent + "}",
            contentType: "application/json; charset = utf-8",
            dataType: "json",
            success: function (response) {
                if ($('#QtyValidationMsg').length > 0) {
                    $("#QtyValidationMsg").text("");
                    $("#QtyValidationMsg").hide();
                }
                if (response == "2") {
                    if ($('#QtyValidationMsg').length > 0) {
                        $("#QtyValidationMsg").text("Item Is Already Added In Your Cart!!!");
                        $("#QtyValidationMsg").show();
                    }
                    //$("[id*=shoppingCartMessage]").text("Item is already added in your cart");
                    alert("Item Is Already Added In Your Cart!!!");
                }
                else if (response == "3") { //Product is out of stock
                    if ($('#QtyValidationMsg').length > 0) {
                        $("#QtyValidationMsg").text("Sorry! Product is out of stock.");
                        $("#QtyValidationMsg").show();
                    }
                    alert("Sorry! Product is out of stock.");
                    window.location.reload();
                } else if (response.indexOf("#") > -1) { //current available quantity is less than required quantity
                    var avalQty = response.replace("#", "");
                    if ($('#QtyValidationMsg').length > 0) {
                        $("#QtyValidationMsg").text("We're sorry! We are able to accommodate only " + avalQty + " units of this product.");
                        $("#QtyValidationMsg").show();
                    }
                    alert("We're sorry! We are able to accommodate only " + avalQty + " units of this product.");
                    if ($('#productQuantity').length > 0) {
                        $("#productQuantity").val(avalQty);
                    }
                    if ($('#txtQuantity').length > 0) {
                        $("#txtQuantity").val(avalQty);
                    }
                }
                else if (response.indexOf("$") > -1) {
                    var msg = response.replace("$", "");
                    if ($('#QtyValidationMsg').length > 0) {
                        $("#QtyValidationMsg").text(msg);
                        $("#QtyValidationMsg").show();
                    }
                    alert(msg);
                }
                else {
                    var a = getCookie("ShoppingCartCookie");
                    $("[id*=spanShoppingCartCount]").text(a.split(',').length);

                    $('#myAlert').css('display', 'block');
                    setTimeout(function () {
                        $("#myAlert").hide('blind', {}, 300)
                    }, 1000);
                }

                //if ($("[id*=spanShoppingCartCount]").text == "") {
                //    $("[id*=spanShoppingCartCount]").text(1);
                //}
                //else {
                //    var currentValue = $("[id*=spanShoppingCartCount]").text();
                //    $("[id*=spanShoppingCartCount]").text(parseInt(currentValue) + 1);
                //}
            },
            failure: function (response) {
                // alert("hello failuer");
            },
            error: function (response) {
                //alert("hello error");
            }
        });
    }
}


//This function add product in shopping cart without quantity
//function used in similar products,recently viewed products, compare products
function AddProductToShppingCartWithoutQuantity(shopStockId, itemId, _url) {
    var str = getCookie("CityCookie");
    var cucity = "";
    var cufranchiseId = 0;////added
    if (str != null && str != "" && str != undefined) {
        var str1 = [];
        str1 = str.split('$');
        cucity = str1[1].toLowerCase();
        cufranchiseId = str1[2];////added
    }
    else {
        cucity = "kanpur";//Yashaswi 01/12/2018 Default City Change 
        cufranchiseId = 1052;//Yashaswi 01/12/2018 Default City Change 
    }

    var custSessionId = $("#hdnSessionValue").val();
    //alert(custSessionId);
    if (custSessionId == null || custSessionId == "" || custSessionId == 0) {
        // alert("You are not logged in! Please log in to continue....." + cucity);
        //window.location.href = '/' + cucity + '/login?callfrom=cart/' + shopStockId + '/' + itemId + '/1';////hide
        window.location.href = '/' + cucity + '/' + cufranchiseId + '/login?callfrom=cart/' + shopStockId + '/' + itemId + '/1';////added
    }
    else {

        $.ajax({
            type: "POST",
            url: _url,
            data: "{'shopStockId':" + shopStockId + ",'itemId':" + itemId + ",'quantity':" + 1 + "}",
            contentType: "application/json; charset = utf-8",
            dataType: "json",
            success: function (response) {
                if ($('#QtyValidationMsg').length > 0) {
                    $("#QtyValidationMsg").text("");
                    $("#QtyValidationMsg").hide();
                }
                debugger;
                if (response == "2") {
                    if ($('#QtyValidationMsg').length > 0) {
                        $("#QtyValidationMsg").text("Item Is Already Added In Your Cart!!!");
                        $("#QtyValidationMsg").show();
                    }
                    //$("[id*=shoppingCartMessage]").text("Item is already added in your cart");
                    alert("Item Is Already Added In Your Cart!!!");
                }
                else if (response == "3") { //Product is out of stock
                    if ($('#QtyValidationMsg').length > 0) {
                        $("#QtyValidationMsg").text("Sorry! Product is out of stock.");
                        $("#QtyValidationMsg").show();
                    }
                    alert("Sorry! Product is out of stock.");
                    window.location.reload();
                } else if (response.indexOf("#") > -1) { //current available quantity is less than required quantity
                    var avalQty = response.replace("#", "");
                    if ($('#QtyValidationMsg').length > 0) {
                        $("#QtyValidationMsg").text("We're sorry! We are able to accommodate only " + avalQty + " units of this product.");
                        $("#QtyValidationMsg").show();
                    }
                    alert("We're sorry! We are able to accommodate only " + avalQty + " units of this product.");
                    if ($('#productQuantity').length > 0) {
                        $("#productQuantity").val(avalQty);
                    }
                    if ($('#txtQuantity').length > 0) {
                        $("#txtQuantity").val(avalQty);
                    }
                }
                else if (response.indexOf("$") > -1) {
                    var msg = response.replace("$", "");
                    if ($('#QtyValidationMsg').length > 0) {
                        $("#QtyValidationMsg").text(msg);
                        $("#QtyValidationMsg").show();
                    }
                    alert(msg);
                }
                else {
                    var a = getCookie("ShoppingCartCookie");
                    $("[id*=spanShoppingCartCount]").text(a.split(',').length);

                    $('#myAlert').css('display', 'block');
                    setTimeout(function () {
                        $("#myAlert").hide('blind', {}, 300)
                    }, 1000);
                }

                //if ($("[id*=spanShoppingCartCount]").text == "") {
                //    $("[id*=spanShoppingCartCount]").text(1);
                //}
                //else {
                //    var currentValue = $("[id*=spanShoppingCartCount]").text();
                //    $("[id*=spanShoppingCartCount]").text(parseInt(currentValue) + 1);
                //}
            },
            failure: function (response) {
                // alert("hello failuer");
            },
            error: function (response) {
                //alert("hello error");
            }
        });
    }
}



// End














//============================================  Code not in use ==============================================
/// <reference path="jqCompareProduct.js" />

// Javascript for load products on scroll

var pageIndex = 1;
var pageCount = $("#PageCount").val();

//$(window).scroll(function () {
//    if ($(document).height() == ($(window).scrollTop() + $(window).height())) {
//        //alert("1");
//        $("#divLoadingImage").css("display", "block");
//        GetRecords();
//    }
//});

var height = $(window).height() + 300;

var counter1 = 0;
var counter2 = 1;

//$(window).scroll(function () {
//    //if (counter1 > 1) {
//        //height = $(document).height() * (0.30);
//        height = $(document).height() * (0.60);
//    //}

//    if ($(window).scrollTop() >= ($(document).height() - height)) {// - $(window).height()) {

//        $("#divLoadingImage").css("display", "block");
//        setTimeout(GetRecords(), 5000);
//        counter1++;
//    }
//});

function GetRecords() {

    pageIndex++;

    var mydata = {
        "cityId": $("#CityID").val(),
        "franchiseId": $("#FranchiseID").val(),////added
        "keyword": $("#Keyword").val(),
        "shopId": $("#ShopID").val(),
        "categoryId": $("#ParentCategory").val(),
        "brands": $("#Brands").val(),
        "colors": $("#Colors").val(),
        "sizes": $("#Sizes").val(),
        "dimensions": $("#Dimensions").val(),
        "catDescID": $("#SpecificationIDs").val(),
        "catDescValue": $("#SpecificationValues").val(),
        "minPrice": $("#MinPrices").val(),
        "maxPrice": $("#MaxPrices").val(),
        "pageIndex": pageIndex,
        "pageSize": 12
    };


    if (pageIndex == 2 || pageIndex <= pageCount) {
        $.ajax({
            type: "POST",
            url: "/Product/GetProductOnScroll",
            data: "{'myParam':" + JSON.stringify(mydata) + "}",
            contentType: "application/json; charset = utf-8",
            dataType: "json",
            success: OnSuccess,
            failure: function (response) {
                $("#divLoadingImage").css("display", "block");


            },
            error: function (response) {
                $("#divLoadingImage").css("display", "none");
            }
        });

        var aProductID = getCookie("ProductID");
        if (aProductID != null) {
            SetCookiesToDiv(aProductID);
        }
    }
    else {
        $("#divLoadingImage").css("display", "none");
    }
}

function OnSuccess(response) {

    $.each(response, function (j, dataVal) {
        var li = $("[id*=ulProduct] li").eq(0).clone(true);
        if (shopId == 0 || shopId == null) {
            $(".anchorRedirect", li).attr("href", "/PreviewItem?itemId=" + dataVal.ProductID);
        }
        else {
            $(".anchorRedirect", li).attr("href", "/PreviewItem?itemId=" + dataVal.ProductID + "&shopID=" + shopId);


        }
        $(".divimg", li).attr("src", dataVal.ProductThumbPath);




        //show pack size and pack unit 
        //$(".packSize", li).html(dataVal.PackSize + " " + dataVal.PackUnit);

        //show size
        //show size only for fruit & vegetables
        //because of that hardcoded condition is checked
        if (dataVal.Size != null && dataVal.Size != "N/A" && dataVal.FirstLevelCatId == 15)
            $(".productSize", li).html(dataVal.Size);

        //show Material
        //if (dataVal.Material != null && dataVal.Material != "N/A")
        //    $(".productMaterial", li).html(dataVal.Material);

        //show Diamention
        //if (dataVal.Dimension != null && dataVal.Dimension != "N/A")
        //    $(".productDiamension", li).html(dataVal.Dimension);

        //show Color
        //if (dataVal.Color != null && dataVal.Color != "N/A" && dataVal.HtmlColorCode != null)
        //    $(".productColor", li).css("background-color", dataVal.HtmlColorCode);



        if (dataVal.MRP > dataVal.SaleRate) {

            //$(".lblMRP", li).html("<b>Rs. </b>" + "<b style='color: brown;'><del>" + Math.round(dataVal.MRP) + "</del></b>");
            $(".lblMRP", li).html("<b>Rs. </b>" + "<b style='color: brown;'><del>" + (dataVal.MRP).toFixed(2) + "</del></b>");
            $(".offer-div", li)

            // Code for calculating offer percentage
            var offerValue = ((parseFloat(dataVal.MRP) - parseFloat(dataVal.SaleRate)) / parseFloat(dataVal.MRP)) * 100;
            var offerPercentage = Math.round(offerValue);

            if (offerPercentage > 0) {
                $(".offer-div", li).css("display", "block");
                $(".offerPercentage", li).text(offerPercentage + " % OFF");
            }
            else {
                $(".offer-div", li).css("display", "none");
                $(".offerPercentage", li).text("");
            }
        }
        else {
            $(".offer-div", li).css("display", "none");

            $(".offerPercentage", li).text("");
            $(".lblMRP", li).html("");
        }
        //$(".lblSaleRate", li).html("<b>Rs. </b>" + "<b style='color: brown;'>" + Math.round(dataVal.SaleRate) + "</b>");
        $(".lblSaleRate", li).html("<b class='fa fa-inr'> </b>" + "<b style='color: brown;'>" + (dataVal.SaleRate).toFixed(2) + "</b>");
        //<input type="number" min="1" max="5" step="1" value="1" id="productQuantity" class="productQuantity" style="width:41px;float:right; text-align:center;" />

        if (dataVal.StockStatus == 1 && dataVal.StockQty > 0) {
            //all condition are reinitialize because when first product is sold out then products loading by jquery shows as sold out 
            $(".productQuantity", li).css("display", "block");
            $(".soldOut", li).html("");
            $(".soldOut", li).removeClass("soldout-div");
            $(".productQuantity", li).css("display", "block");
            $(".addToCartJquery", li).css("display", "block");
            $(".product-img", li).removeClass("product-imgnew");

            $(".productQuantity", li).attr("id", dataVal.ProductID);
            $(".productQuantity", li).val("1");
            $(".productQuantity", li).attr("max", dataVal.StockQty);
        }
        else {
            $(".productQuantity", li).css("display", "none");
            $(".soldOut", li).html("Sold </br> Out");
            $(".soldOut", li).addClass("soldout-div");
            $(".product-img", li).addClass("product-imgnew");
            $(".offer-div", li).css("display", "none");
            $(".offerPercentage", li).text("");

            $(".addToCartJquery", li).css("display", "none");
            $(".corporateGift", li).css("display", "none");



        }



        //alert(dataVal.CategoryID);
        //alert("2"+dataVal.catDescValue);alert("3"+dataVal.categoryId);alert("4"+dataVal.FirstLevelCatId);
        if (dataVal.CategoryID == 1368) {
            //alert("1");
            $(".corporateGift", li).css("display", "inline-block");
            $(".addToCartJquery", li).css("display", "none");
            $(".productQuantity", li).css("display", "none");
        }
        else {
            //alert("2");
            var shopId = $("#ShopID").val();
            $(".corporateGift", li).css("display", "none");
            if (shopId == 0 || shopId == null) {
                $(".btnView", li).attr("href", "/PreviewItem?itemId=" + dataVal.ProductID);
                if (dataVal.StockStatus == 1 && dataVal.StockQty > 0) {

                    //$(".btnBuy", li).attr("href", "/PreviewItem?itemId=" + dataVal.ProductID + "&shopStockId=" + dataVal.ShopStockID);
                    $(".btnBuy", li).attr("onclick", "AddProductToShppingCart(" + dataVal.ShopStockID + "," + dataVal.ProductID + ")");
                }
                else {
                    $(".btnBuy", li).css("display", "none");
                }
            }
            else {
                $(".btnView", li).attr("href", "/PreviewItem?itemId=" + dataVal.ProductID + "&shopID=" + shopId);
                if (dataVal.StockStatus == 1 && dataVal.StockQty > 0) {
                    //$(".btnBuy", li).attr("href", "/PreviewItem?itemId=" + dataVal.ProductID + "&shopStockId=" + dataVal.ShopStockID);
                    $(".btnBuy", li).attr("onclick", "AddProductToShppingCart(" + dataVal.ShopStockID + "," + dataVal.ProductID + ")");
                }
                else {
                    $(".btnBuy", li).css("display", "none");
                }
            }
        }


        //============= Tejaswee (24/6/2015) ====================
        $(".hdnProductName", li).val(dataVal.Name);
        var shortListHtml;
        var VarientEdit = getCookie("ShortListCookie");
        if (VarientEdit != null && VarientEdit != "") {
            var cookRecView = VarientEdit;
            if (cookRecView.indexOf(dataVal.ProductID + "$" + dataVal.ShopStockID + "$") >= 0) {
                $(".fa fa-star", li).attr("id", dataVal.ProductID);
                $(".fa fa-star", li).addClass("activeListNew");
                //shortListHtml = "<button id='" + dataVal.ProductID + "#" + dataVal.Name + "' title='Added To Shortlist' class='mif-star-full button activeListNew'>" +
                //"</button>";
            }
            else {
                $(".fa fa-star", li).attr("id", dataVal.ProductID);
                $(".fa fa-star", li).attr("onclick", "AddProductToShortList(this)");
                $(".fa fa-star", li).removeClass("activeListNew");
            }
        }
        else {
            $(".fa fa-star", li).attr("id", dataVal.ProductID);
            $(".fa fa-star", li).attr("onclick", "AddProductToShortList(this)");
            $(".fa fa-star", li).removeClass("activeListNew");
        }
        //============= Tejaswee (24/6/2015) ====================


        if (dataVal.Name.length > 60) {
            $(".lablehead", li).html(dataVal.Name.substring(0, 57) + "...");
        }
        else {
            $(".lablehead", li).html(dataVal.Name);
        }

        $(".lableheadright v-align-bottom", li).html("<b>Rs. </b>" + dataVal.SaleRate);

        $("[id*=ulProduct]").append(li);

        $('.addToCompare-btn', li).removeAttr('style');

        if (dataVal.FirstLevelCatId == 7) {
            //var _url = '@Html.Raw(@Url.Action("AddProductInCompareList", "Product"))';

            //var _url = '@Url.Action("AddProductInCompareList", "Product")';
            $('.addToCompare-btn', li).css('display', 'block');
            $('.addToCompare-btn', li).attr("id", "compare_" + dataVal.ProductID);
            $(".addToCompare-btn", li).attr("onclick", "AddProductToCompare('" + dataVal.ProductID + "','" +
                dataVal.ProductThumbPath + "','" + dataVal.Name + "','" + dataVal.ShopStockID + "')");
        }
        else {
            $('.addToCompare-btn', li).css('display', 'none');
        }



        var img = $(".divimg", li);



        var loader = $("<img class = 'loader' src ='../Content/Images/gb-loader.gif' style='margin-top: 85px;' />");
        img.after(loader);
        img.hide();
        img.attr("src", dataVal.ProductThumbPath);
        img.load(function () {
            $(this).parent().find(".loader").remove();
            $(this).fadeIn();
        });
    });
    $("#divLoadingImage").css("display", "none");

}
//============================================  Code not in use ==============================================