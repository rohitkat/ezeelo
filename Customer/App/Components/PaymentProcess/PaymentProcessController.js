var md5 = function (value) {
    return CryptoJS.MD5(value).toString();
}


gbApp.controller('PaymentProcessController', ['$scope', '$http', '$localStorage',
                                     '$sessionStorage', '$rootScope',
                                      function ($scope, $http, $localStorage, $sessionStorage, $rootScope) {
                                          $scope.checkOut = function () 
                                          {
                                              if (typeof $rootScope.username == "undefined")
                                              {
                                                  window.location.assign('#/login');
                                              }
                                              else
                                              {
                                                  window.location.assign('#/shippingAddress');
                                              }
                                          }

}]);
