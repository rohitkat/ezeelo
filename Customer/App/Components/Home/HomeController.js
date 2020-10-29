gbApp.controller('HomeController', ['$scope', '$http', '$localStorage', '$sessionStorage', '$rootScope', function ($scope, $http, $localStorage, $sessionStorage, $rootScope) {
    $rootScope.waiting = false;
    $rootScope.show = true;

    $scope.trackOrder = function (orderCode) {
        $rootScope.waiting = true;
        $rootScope.show = false;
        $http.get(CONSTANTS['FOODBAGH_API'] + 'TrackOrder?OrderCode=' + orderCode).success(function (data) {
            $scope.trackOrderList = data;
            $rootScope.waiting = false;
            $rootScope.show = true;
            //alert($scope.trackOrderList.CustomerOrderID);
        });
    }

    $scope.myTrackOrderCode = function (orderCode) {
        $rootScope.trackOrderCode = orderCode;
        $rootScope.waiting = false;
        $rootScope.show = false;
    }


    $scope.checkPincode = function (pincode, isJustReadyForShop) {
        $scope.waiting = true;
        $scope.show = false;
        $http.get(CONSTANTS['GANDHIBAGH_API'] + 'api/VerifyPincode?pincode=' + pincode).success(function (data) {
            $scope.pincodeData = data;
            $scope.pincodeStatus = $scope.pincodeData.status;
            $scope.pincodeStatusColor = 'red';
            $scope.pincodeMsg = "Sorry, We are currently not available at " + pincode + " Location.";

            if (isJustReadyForShop == 1) {
                $scope.pincodeStatus = 'True';
                $scope.$storage = $localStorage;
                $scope.$storage.pincode = pincode;
                localStorage.setItem("pincode", pincode);
            }


            if ($scope.pincodeStatus == 'True') {
                //$scope.$storage = $localStorage;
                //$scope.$storage.pincode = pincode;
                //localStorage.setItem("pincode", pincode);
                $scope.pincodeMsg = "Let's Start Shopping.";
                $scope.pincodeStatusColor = '#A8CF45';
            }
            $scope.waiting = false;
            $scope.show = true;
        });
    }

}])