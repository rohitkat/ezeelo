//var CartControllers = angular.module('CartControllers', []);
//alert('controller');
gbApp.controller('CartController', ['$scope', 'productCartService',
                                    function ($scope, productCartService) {
                                        $scope.cart = productCartService.getProductsFromCart();
                                    }

]);

