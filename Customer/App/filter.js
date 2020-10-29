gbApp.filter('unique', function () {
    return function (collection, keyname) {
        var output = [],
            keys = [];

        angular.forEach(collection, function (item) {
            var key = item[keyname];
            if (keys.indexOf(key) === -1) {
                keys.push(key);
                output.push(item);
            }
        });

        return output;
    };
});


//- used for pagination -//
gbApp.filter('startFrom', function () {
    return function (input, start) {
        if (!input || !input.length) { return; }
        start = +start; //parse to int
        return input.slice(start);
    }
});


gbApp.filter('trim', function () {
    return function (value) {
        if (!angular.isString(value)) {
            return value;
        }
        return value.replace(/^\s+|\s+$/g, ''); // you could use .trim, but it's not going to work in IE<9
    };
});

gbApp.filter('num', function () {
    return function (input) {
        return parseInt(input, 10);
    };
});

gbApp.filter('trustAsResourceUrl', ['$sce', function ($sce) {
    return function (val) {
        return $sce.trustAsResourceUrl(val);
    };
}]);





//--- This filter is not currently used... but it is a important function.. don't delete.

//gbApp.filter('orderObjectBy', function () {
//    return function (input, attribute) {
//        if (!angular.isObject(input)) return input;

//        var array = [];
//        for (var objectKey in input) {
//            array.push(input[objectKey]);
//        }

//        array.sort(function (a, b) {
//            a = parseInt(a[attribute]);
//            b = parseInt(b[attribute]);
//            return a - b;
//        });
//        return array;
//    }
//});





function trim(value) {
    if (!angular.isString(value)) {
        return value;
    }
    return value.replace(/^\s+|\s+$/g, ''); // you could use .trim, but it's not going to work in IE<9
}