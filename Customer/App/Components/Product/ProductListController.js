//var ProductListControllers = angular.module('ProductListControllers', []);
gbApp.controller('ProductListController', ['$scope', '$rootScope', '$http', 'productCartService', '$location', '$localStorage', '$sessionStorage', 'filterFilter',
                  function ($scope, $rootScope, $http, productCartService, $location, $localStorage, $sessionStorage, filterFilter) {
                      var lUrl = $location.absUrl();
                      var lUrlSplitArr = lUrl.split("/");
                      var lParentCategoryID = lUrlSplitArr[5];
                      var lLevel = lUrlSplitArr[6];
                      var lCityID = lUrlSplitArr[7];
                      $scope.$storage = $localStorage;
                      $scope.waiting = true;
                      $scope.show = false;
                      $scope.city = getCookie("CityCookie").split('$')[1].toLowerCase();
                      $scope.Offer_set = "Offer_set";

                      $http.get(CONSTANTS['FOODBAGH_API'] + 'ShopProduct?ParentCatID=' + lParentCategoryID
                                                      + '&Level=' + lLevel
                                                      + '&CityID=' + lCityID).success(function (data) {
                                                          var lProducts = processData(data);
                                                          $scope.Products = lProducts;

                                                          //- For Refinment....
                                                          $scope.ProductVarients = selectVarient($scope.Products);

                                                          $scope.waiting = false;
                                                          $scope.show = true;

                                                          // create empty search model (object) to trigger $watch on update
                                                          $scope.search = {};

                                                          //$scope.resetFilters = function () {
                                                          //    // needs to be a function or it won't trigger a $watch
                                                          //    $scope.search = {};
                                                          //};

                                                          // pagination controls
                                                          $scope.currentPage = 1;
                                                          $scope.totalItems = $scope.Products.length;
                                                          $scope.entryLimit = 40; // items per page
                                                          $scope.noOfPages = Math.ceil($scope.totalItems / $scope.entryLimit);
                                                          $scope.maxSize = Math.ceil($scope.noOfPages / 2);
                                                          //alert($scope.maxSize);
                                                          // $watch search to update pagination
                                                          $scope.$watch('search', function (newVal, oldVal) {
                                                              $scope.filtered = filterFilter($scope.Products, newVal);
                                                              $scope.totalItems = $scope.filtered.length;
                                                              $scope.noOfPages = Math.ceil($scope.totalItems / $scope.entryLimit);
                                                              $scope.maxSize = Math.ceil($scope.noOfPages / 2);
                                                              $scope.currentPage = 1;
                                                          }, true);
                                                      });

                      $scope.orderProp = '-Offer';
                      $scope.productCategory = $location.search().CatName;
                      //if ($scope.productCategory.length > 15) {
                      //    $scope.productCategory = $scope.productCategory.substring(0, 14) + "...";
                      //}

                      //---------------------------------------------------------- Start used for css design of windows tile and list view -//
                      $scope.layout = 'gb-tile';
                      $scope.tileHeader = 'gb-tile-header';
                      $scope.tileHeaderRs = 'gb-rs';
                      $scope.tileHeaderSelectVarient = 'gb-select-varieant';

                      $scope.tileBottom = 'gb-tile-bottom';
                      $scope.tileBottomAddProduct = 'gb-addProduct';

                      $scope.imgView = 'gb-img-medium';
                      $scope.description = 'Description';

                      $scope.showDescription = function (obj) {
                          $scope.description = "Description : " + obj.ProductDescription;
                      }

                      $scope.toggle = function (layout) {
                          $scope.layout = layout;
                          if (layout == 'thumbnail') {
                              $scope.imgView = 'gb-img-small';
                              $scope.tileHeader = 'gb-thumbnail-header';
                              $scope.tileHeaderRs = 'gb-rs';
                              $scope.tileHeaderSelectVarient = 'gb-select-varieant';

                              $scope.tileBottom = 'gb-thumbnail-bottom';
                              $scope.tileBottomAddProduct = 'gb-addProduct';
                          }
                          else {
                              $scope.imgView = 'gb-img-medium';
                              $scope.tileHeader = 'gb-tile-header';
                              $scope.tileHeaderRs = 'gb-rs';
                              $scope.tileHeaderSelectVarient = 'gb-select-varieant';

                              $scope.tileBottom = 'gb-tile-bottom';
                              $scope.tileBottomAddProduct = 'gb-addProduct';
                          }
                      };
                      //------------------------------------------------------------ End used for css design of windows tile and list view -//



                      $scope.ChangeQty = function (operator, obj) {
                          var lProductID = operator + obj.ProductID;
                          var img = $('#' + lProductID).parent().parent().find('.avid').find("#img1");

                          if (operator == 'Minus') {
                              if (obj.BuyQty > 1) {
                                  obj.BuyQty = obj.BuyQty - 1;
                                  obj.BuyAmount = obj.RetailerRate * obj.BuyQty;
                                  if (obj.AddedInCart) {
                                      //productCartService.updateQty(obj);
                                      try {
                                          flyFromElement($(img), $('#basket'));
                                      }
                                      catch (ex) { }
                                      productCartService.updateQty(obj);
                                      $rootScope.cartAmount = productCartService.getCartAmount();
                                      $rootScope.deliveryCharge = productCartService.getDeliveryCharge();
                                      $rootScope.payableAmount = productCartService.getpayableAmount();
                                  }
                              }
                          }
                          else if (operator == 'Plus') {
                              if (obj.BuyQty < obj.MaxQty) {
                                  obj.BuyQty = obj.BuyQty + 1;
                                  obj.BuyAmount = obj.RetailerRate * obj.BuyQty;
                                  if (obj.AddedInCart) {
                                      //productCartService.updateQty(obj);
                                      try {
                                          flyToElement($(img), $('#basket'));
                                      }
                                      catch (ex) { }
                                      productCartService.updateQty(obj);
                                      $rootScope.cartAmount = productCartService.getCartAmount();
                                      $rootScope.deliveryCharge = productCartService.getDeliveryCharge();
                                      $rootScope.payableAmount = productCartService.getpayableAmount();
                                  }
                              }
                          }
                      }

                      $rootScope.cartAmount = productCartService.getCartAmount();
                      $rootScope.deliveryCharge = productCartService.getDeliveryCharge();
                      $rootScope.payableAmount = productCartService.getpayableAmount();

                      $scope.AddToCart = function (obj) {

                          obj.AddedInCart = !obj.AddedInCart;
                          var lProductID = obj.ProductID;
                          var img = $('#' + lProductID).parent().parent().parent().find('.avid').find("#img1");

                          if (obj.AddedInCart) {

                              productCartService.addProductToCart(obj);
                              //alert(img);
                              try {
                                  flyToElement($(img), $('#basket'));
                              }
                              catch (ex) { }

                              $rootScope.cartAmount = productCartService.getCartAmount();
                              $rootScope.deliveryCharge = productCartService.getDeliveryCharge();
                              $rootScope.payableAmount = productCartService.getpayableAmount();

                          }
                          else {
                              productCartService.removeFromCart(obj);
                              try {
                                  flyFromElement($(img), $('#basket'));
                              }
                              catch (ex) { }
                              $rootScope.cartAmount = productCartService.getCartAmount();
                              $rootScope.deliveryCharge = productCartService.getDeliveryCharge();
                              $rootScope.payableAmount = productCartService.getpayableAmount();
                          }

                      }

                      $scope.changeVarient = function (product, item) {
                          product.ShopStockID = item.ID;
                          product.BuyQty = item.BuyQty;
                          product.MRP = item.MRP;
                          product.RetailerRate = item.RetailerRate;
                          product.BuyAmount = item.BuyAmount;
                          product.ProductVarientSize = item.ProductVarientSize;

                          product.ShopID = item.ShopID;


                          //-- start new added on 14-dec-2015. ------------//
                          //productCartService.removeFromCart(item);
                          //try {
                          //    flyFromElement($(img), $('#basket'));
                          //}
                          //catch (ex) { }
                          $rootScope.cartAmount = productCartService.getCartAmount();
                          $rootScope.deliveryCharge = productCartService.getDeliveryCharge();
                          $rootScope.payableAmount = productCartService.getpayableAmount();
                          //-- end ----------------------------------------//

                      }

                      $scope.AddToPreview = function (obj) {
                          $rootScope.PreviewItem = obj;
                          //alert($rootScope.PreviewItem.ShopStockByVarients[0].RetailerRate);
                      }


                      $scope.isDeliveryPossible = isDeliverPossibleToday();

                  } ]);

function flyToElement(flyer, flyingTo, callBack /*callback is optional*/) {
    var $func = $(this);

    var divider = 3;

    var flyerClone = $(flyer).clone();
    $(flyerClone).css({
        position: 'absolute',
        top: $(flyer).offset().top + "px",
        left: $(flyer).offset().left + "px",
        opacity: 1,
        'z-index': 1000
    });
    $('body').append($(flyerClone));

    var gotoX = $(flyingTo).offset().left + ($(flyingTo).width() / 2) - ($(flyer).width() / divider) / 2;
    var gotoY = $(flyingTo).offset().top + ($(flyingTo).height() / 2) - ($(flyer).height() / divider) / 2;

    $(flyerClone).animate({
        opacity: 0.4,
        left: gotoX,
        top: gotoY,
        width: $(flyer).width() / divider,
        height: $(flyer).height() / divider
    }, 700,
    function () {
        $(flyingTo).fadeOut('fast', function () {
            $(flyingTo).fadeIn('fast', function () {
                $(flyerClone).fadeOut('fast', function () {
                    $(flyerClone).remove();
                    if (callBack != null) {
                        callBack.apply($func);
                    }
                });
            });
        });
    });
}

function flyFromElement(flyer, flyingTo, callBack /*callback is optional*/) {
    var $func = $(this);

    var divider = 3;

    var beginAtX = $(flyingTo).offset().left + ($(flyingTo).width() / 2) - ($(flyer).width() / divider) / 2;
    var beginAtY = $(flyingTo).offset().top + ($(flyingTo).width() / 2) - ($(flyer).height() / divider) / 2;

    var gotoX = $(flyer).offset().left;
    var gotoY = $(flyer).offset().top;

    var flyerClone = $(flyer).clone();

    $(flyerClone).css({
        position: 'absolute',
        top: beginAtY + "px",
        left: beginAtX + "px",
        opacity: 0.4,
        'z-index': 1000,
        width: $(flyer).width() / divider,
        height: $(flyer).height() / divider
    });
    $('body').append($(flyerClone));

    $(flyerClone).animate({
        opacity: 1,
        left: gotoX,
        top: gotoY,
        width: $(flyer).width(),
        height: $(flyer).height()
    }, 700,
    function () {
        $(flyerClone).remove();
        $(flyer).fadeOut('fast', function () {
            $(flyer).fadeIn('fast', function () {
                if (callBack != null) {
                    callBack.apply($func);
                }
            });
        });
    });
}

function selectVarient(Products)
{
    var lProductVarients = [];
    for(i=0;i<Products.length;i++)
    {
        var lProduct = Products[i];
        for (j = 0; j < lProduct.ShopStockByVarients.length;j++)
        {
            lProductVarients.push(lProduct.ShopStockByVarients[j]);
        }
    }
    return lProductVarients;
}

function isDeliverPossibleToday()
{
    var d = new Date();
    var n = d.getDay();
    if(n==5)
    {
        return false;
    }
    return true;
}

function processData(lst)
{
    for (i = 0; i < lst.length; i++)
    {
        //lst[i].URLPName = lst[i].ProductName.replace(/&/gi, "and");
        //lst[i].URLPName = lst[i].ProductName.replace(/[\/\\#,+()$~%.':*?<>{} ]/g, "-").toLowerCase();

        lst[i].URLPName = lst[i].ProductName.toLowerCase().replace(/&/g, 'and').replace(/[\/\\#,+()$~%.'":*?<>{} ]/g, '-');

        if (lst[i].ProductName.length > 30) {
            lst[i].URLPName = (lst[i].ProductName.toLowerCase().replace(/&/g, 'and').replace(/[\/\\#,+()$~%.'":*?<>{} ]/g, '-').substr(0, 30));//.replace("&", "and").toLowerCase();
        }
        //lst[i].URLPName = "avi";
    }
    return lst;
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