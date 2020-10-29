

function GetShopList(catID) {

    $(".loader").show();
    var mydata = {
        "ShopcityId": $("#CityID").val().split('$')[0],
        "ShopfranchiseId": $("#CityID").val().split('$')[2],////added 
        "ShopcategoryId": catID,
    };
   // $.get("/" + $("#CityID").val().split('$')[1] + '/AllShops/GetCategoryWiseShopList/', mydata, function (data) {////hide
    $.get("/" + $("#CityID").val().split('$')[1] + "/" + $("#CityID").val().split('$')[2] + '/AllShops/GetCategoryWiseShopList/', mydata, function (data) {////added

        $('#divShopList').html(data);
        $('#divShopList').fadeIn('fast');
        $(".loader").hide();
    });

}