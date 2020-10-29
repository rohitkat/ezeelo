//var gbApp = angular.module('gbApp', ['ngRoute', 'ngStorage', 'ngSanitize', 'ui.bootstrap']);
var gbApp = angular.module('gbApp', ['ngStorage', 'ngSanitize', 'ui.bootstrap']);
//alert('app');
//var gbApp = angular.module('gbApp', ['ngStorage', 'ngSanitize', 'ui.bootstrap']);
//var gbApp = angular.module('gbApp', ['ngRoute', 'ngStorage']);

gbApp.controller('TopMenuController', ['$scope', '$rootScope', '$http', '$localStorage', '$sessionStorage', '$location', function ($scope, $rootScope, $http, $localStorage, $sessionStorage, $location) {
    //   $http.get(CONSTANTS['FOODBAGH_API'] + 'CityList').success(function (data) {
    data = [
                {
                    "ID": 4968,
                    "Name": "Nagpur",
                    "CustomerCareNo": "9860890890",
                    "ThemeID": "4968.css"
                },
                {
                    "ID": 10909,
                    "Name": "Kanpur",
                    "CustomerCareNo": "9860890890",
                    "ThemeID": "10909.css"
                },
                {
                    "ID": 10908,
                    "Name": "Varanasi",
                    "CustomerCareNo": "9860890890",
                    "ThemeID": "10908.css"
                }
          ];
    $scope.CityList = data;
    $scope.$storage = $localStorage;
    //try
    //{
    if ($location.search().cityId != null) {
        var lCityID = $location.search().cityId;
        for (var i = 0; i < $scope.CityList.length; i++) {
            if ($scope.CityList[i].ID == lCityID) {
                $scope.selectedCity = $scope.CityList[i];
                break;
            }
        }
    }
    else if (localStorage.getItem("ngStorage-cityID") != null) {
        var lCityID = localStorage.getItem("ngStorage-cityID");
        for (var i = 0; i < $scope.CityList.length; i++) {
            if ($scope.CityList[i].ID == lCityID) {
                $scope.selectedCity = $scope.CityList[i];
                break;
            }
        }
    }
    else {
        $scope.selectedCity = $scope.CityList[0];
    }
    //}
    //catch(err)
    //{
    //    $scope.selectedCity = $scope.CityList[0];
    //}
    $scope.$storage.cityID = $scope.selectedCity.ID;
    $rootScope.CustomerCareNo = $scope.selectedCity.CustomerCareNo;
    $rootScope.CityName = $scope.selectedCity.Name;

    $scope.ChangeCity = function (obj) {
        $rootScope.CustomerCareNo = obj.CustomerCareNo;
        $rootScope.CityName = obj.Name;
        $scope.$storage.cityID = obj.ID;
        window.location.assign(CONSTANTS["HOME_PAGE"]);
        window.localStorage.clear();
    }
    //    });
}])



