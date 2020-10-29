gbApp.controller('GlobalSearchController', ['$scope', '$rootScope', '$http', 'productCartService', '$location', '$localStorage', '$sessionStorage', 'filterFilter',
                  function ($scope, $rootScope, $http, productCartService, $location, $localStorage, $sessionStorage, filterFilter) {
                      $scope.$storage = $localStorage;
                      $scope.noRecordsFound = "none";

                      $scope.searchClass = "none";
                      $scope.hideGlobalSearch = function () {
                          $scope.searchClass = "none";
                      }

                      $scope.searchEnable = false;

                      $scope.SearchGlobalProduct = function (SearchKeyword) {
                          $scope.searchEnable = true;
                          if (SearchKeyword.length == 0)
                          {
                              $scope.SearchData = [];
                              $scope.searchClass = "none";
                          }
                          else if (SearchKeyword.length >= 1)
                          {
                              $scope.searchClass = "block";
                              $http.get(CONSTANTS['FOODBAGH_API'] + 'SearchProduct?ParentCatID=0&Level=0&CityID=' + $scope.$storage.cityID
                                                                  + '&SearchKeyword=' + SearchKeyword).success(function (data) {
                                                                      $scope.SearchData = data;
                                                                      if($scope.SearchData.length == 0)
                                                                      {
                                                                          $scope.noRecordsFound = "block";
                                                                      }
                                                                      else
                                                                      {
                                                                          $scope.noRecordsFound = "none";
                                                                      }
                                                                  });
                          }
                          $scope.searchEnable = false;
                      }


                      $scope.orderProp = 'ProductName';
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
                                      //flyFromElement($(img), $('#basket'));
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
                                      //flyToElement($(img), $('#basket'));
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
                              //flyToElement($(img), $('#basket'));
                              $rootScope.cartAmount = productCartService.getCartAmount();
                              $rootScope.deliveryCharge = productCartService.getDeliveryCharge();
                              $rootScope.payableAmount = productCartService.getpayableAmount();
                          }
                          else {
                              productCartService.removeFromCart(obj);
                              //flyFromElement($(img), $('#basket'));
                              $rootScope.cartAmount = productCartService.getCartAmount();
                              $rootScope.deliveryCharge = productCartService.getDeliveryCharge();
                              $rootScope.payableAmount = productCartService.getpayableAmount();
                          }

                      }

                      $scope.changeVarient = function (product, item) {
                          product.ShopStockID = item.ID;
                          product.BuyQty = item.BuyQty;
                          product.RetailerRate = item.RetailerRate;
                          product.BuyAmount = item.BuyAmount;
                          product.ProductVarientSize = item.ProductVarientSize;
                      }
                  }]);

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