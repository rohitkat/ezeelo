/// <reference path="../../../Scripts/Products.js" />



gbApp.service('productCartService', function ($http) {
   

   

    var cartList = [];
    var init = function () {
        for (var key in localStorage) {
            if (key.indexOf("myCart") > -1) {
                var lStorageObj = JSON.parse(localStorage[key]);
                cartList = lStorageObj;
            }
        }
    }
    init();
    
    var addProductToCart = function (newObj)
    {
        /* to Add in shopping Cart
        Added by Pradnyakar 
        29-12-2015
        */
       // var cityName = getCookie("CityCookie");
        var str = getCookie("CityCookie");
        var cityName = "";
        var franchise = 0;////added

        if (str != null && str != "" && str != undefined) {

            var str1 = [];
            str1 = str.split('$');
            cityName = str1[1].toLowerCase();
            franchise = str1[2];////added

            cityName = "/" + cityName.split('$')[1].toLowerCase();///hide 
            cityName = "/" + cityName.split('$')[1].toLowerCase() + "/" + franchise; ////added +"/" +franchise
        }
        
        $http({
            url: cityName+'/Product/AddProductInShoppingCart',
            method: 'POST',
            params: { shopStockId: newObj.ShopStockID, itemId: newObj.ProductID, quantity: newObj.BuyQty }

        }).success(function (abc, status, headers, config) {           
            if (abc == "2") {                
                alert("Item Is Already Added In Your Cart!!!");
            }
            else {
                var a = getCookie("ShoppingCartCookie");        
                $("[id*=spanShoppingCartCount]").text(a.split(',').length);
               
                $('#myAlert').css('display', 'block');
                setTimeout(function () {
                    $("#myAlert").hide('blind', {}, 300)
                }, 1000);
            }            
        });

        //cartList.push(newObj);
        //localStorage.setItem("myCart", JSON.stringify(cartList));
    };

    var removeFromCart = function (newObj) {
        var index = cartList.indexOf(newObj);
        if (index > -1) {
            cartList.splice(index, 1);
        }
        localStorage.setItem("myCart", JSON.stringify(cartList));
        //cartList.pop(newObj);
    };

    var updateQty = function (newObj) {
        var index = cartList.indexOf(newObj);
        if (index > -1) {
            cartList.splice(index, 1);
        }
        cartList.push(newObj);
        localStorage.setItem("myCart", JSON.stringify(cartList));
    }

    var getProductsFromCart = function () {
        //cartList = localStorage.getItem("myCart");
        //alert(cartList);
        //for (var key in localStorage) {
        //    if (key.indexOf("myCart") > -1) {
        //        var lStorageObj = JSON.parse(localStorage[key]);
        //        cartList = lStorageObj;
        //    }
        //}
        return cartList;
    };

    var getCartAmount = function () {
        var cartAmount = 0;
        for (i = 0; i < cartList.length; i++) {
            cartAmount = cartAmount + cartList[i].BuyAmount;
        }
        localStorage.setItem("totalCartAmount", cartAmount);
        return cartAmount;
    }

    var getDeliveryCharge = function () {
        var lCartAmount = getCartAmount();
        var lDeliveryCharge = 0;
        if (lCartAmount >= 500)
        {
            lDeliveryCharge = 0;
        }
        else if(lCartAmount > 0)
        {
            lDeliveryCharge = 50;
        }
        else
        {
            lDeliveryCharge = 0;
        }

        var lFreshMarketCategories = [1281, 1282, 1307, 1338, 1308];
        var lFreshMarketCartAmount = 0;
        for (i = 0; i < cartList.length; i++) {
            var lCategoryID = cartList[i].CategoryID;
            if (lFreshMarketCategories.indexOf(lCategoryID) > -1)
            {
                lFreshMarketCartAmount = lFreshMarketCartAmount + cartList[i].BuyAmount;
            }
        }

        var lCityID = parseInt(localStorage.getItem("ngStorage-cityID"));
       // alert(lCityID);
        switch (lCityID)
        {
            case 10908: //- Varansai
                //alert(lCityID);
                if (lFreshMarketCartAmount >= 300) {
                    lDeliveryCharge = 0;
                }
                break;
            default:
                if (lFreshMarketCartAmount >= 200) {
                    lDeliveryCharge = 0;
                }
                break;
        }

        return lDeliveryCharge;


        //--------------------------------------------------------- start main logic before 28-nov-2015 -//
        //var lCartAmount = getCartAmount();
        //if(lCartAmount > 500)
        //{
        //    return 0;
        //}
        //else if(lCartAmount > 0)
        //{
        //    return 50;
        //}
        //else
        //{
        //    return 0;
        //}
        //----------------------------------------------------------- end main logic before 28-nov-2015 -//
    }

    var getpayableAmount = function () {
        var lPayableAmount = 0;
        var lCartAmount = getCartAmount();
        var lDeliveryCharge = getDeliveryCharge();
        lPayableAmount = lCartAmount + lDeliveryCharge;
        return lPayableAmount;
    }

    /* to get cookies value
    Added by Pradnyakar 
    29-12-2015
    */
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

    return {
        addProductToCart: addProductToCart,
        removeFromCart: removeFromCart,
        updateQty:updateQty,
        getProductsFromCart: getProductsFromCart,
        getCartAmount: getCartAmount,
        getDeliveryCharge: getDeliveryCharge,
        getpayableAmount: getpayableAmount
    };

});


//function sortByKey(array, key) {
//    return array.sort(function (a, b) {
//        var x = a[key]; var y = b[key];
//        return ((x < y) ? -1 : ((x > y) ? 1 : 0));
//    });
//}