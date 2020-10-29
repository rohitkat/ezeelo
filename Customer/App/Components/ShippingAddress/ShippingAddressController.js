var md5 = function (value) {
    return CryptoJS.MD5(value).toString();
}


gbApp.controller('ShippingAddressController', ['$scope', '$http', '$localStorage',
                                     '$sessionStorage', '$rootScope',
                                      function ($scope, $http, $localStorage, $sessionStorage, $rootScope) {
                                          $scope.ID = "";
                                          $scope.UserLoginID = "";
                                          $scope.PrimaryMobile = "";
                                          $scope.ShippingAddress = "";
                                          $scope.PincodeName = "";

                                          $http.get(CONSTANTS['GANDHIBAGH_API_LOCAL'] + 'GBShippingAddress?custLoginID=' + '501').success(function (data) {
                                              $scope.shippingAddress = data;
                                              $scope.edit = function(obj)
                                              {
                                                  $scope.ID = obj.ID;
                                                  $scope.UserLoginID = obj.UserLoginID;
                                                  $scope.PrimaryMobile = obj.PrimaryMobile;
                                                  $scope.ShippingAddress = obj.ShippingAddress;
                                                  $scope.PincodeName = obj.PincodeName;
                                              }
                                          });
                                      }]);