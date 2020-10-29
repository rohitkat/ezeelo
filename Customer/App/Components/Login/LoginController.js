var md5 = function (value) {
    return CryptoJS.MD5(value).toString();
}


gbApp.controller('LoginController', ['$scope', '$http', '$localStorage',
                                     '$sessionStorage', '$rootScope', '$interval',
                                      function ($scope, $http, $localStorage, $sessionStorage, $rootScope, $interval) {

                                          //$rootScope.username = $scope.username;

                                          $scope.login = function (username, password) {
                                              var LoginViewModel = { 'Username': username, 'Password': md5(password) };
                                              $.post(CONSTANTS['FOODBAGH_API'] + 'Login', LoginViewModel, function (data, status) {
                                                  $rootScope.username = data.Username;
                                              });

                                              //$interval(callAtTimeout, 5000);
                                              //function callAtTimeout() {
                                              //    //$scope.username = $rootScope.username;
                                              //    //console.log($scope.username);
                                              //}
                                          }

                                      }]);