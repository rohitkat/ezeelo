function GetOfferProducts(offerStatus, catID)
{
    //alert(catID);
    //var _url = $("#CityID").val().split('$')[1] + "/CategoryWiseOffers/GetOfferStatusWiseProducts";//  '@(Html.Raw(Url.Action("GetOfferStatusWiseProducts", "CategoryWiseOffers")))';
    //alert(_url);
    var mydata = {
        "cityId": $("#CityID").val().split('$')[0],//---  #CityID-> from hiddenfield in CategoryWiseOffer->Index
        "franchiseId": $("#CityID").val().split('$')[2],////added
        "offerStatus": offerStatus,
        "categoryId": catID,
        "partialFlag":false,
        "pageIndex": 1,
        "pageSize": 12
    };
   // $.get("/" + $("#CityID").val().split('$')[1] + '/CategoryWiseOffers/GetOfferStatusWiseProducts/', mydata, function (data) {//hide
    $.get("/" + $("#CityID").val().split('$')[1] + "/" + $("#CityID").val().split('$')[2] + '/CategoryWiseOffers/GetOfferStatusWiseProducts/', mydata, function (data) {////added
        /* data is the pure html returned from action method, load it to your page */
        $('#divProducts').html(data);
       
        /* little fade in effect */
        $('#divProducts').fadeIn('fast');
        
        
        //alert(offerStatus);
        //if (offerStatus == 0) {
        //    alert("missed");
        //    // $("#divMissedDeal").a('class', 'textcolor');
        //}
        //else if (offerStatus == 1) {

        //}
    });
}



//Load products depending on category id
//This method used because offer product loads on different div
function GetOfferProductsByCategory(offerStatus, catID) {
    $(".loader").show();
    //alert("catoffer"+catID);
    var mydata = {
        "cityId": $("#CityID").val().split('$')[0],
        "franchiseId": $("#CityID").val().split('$')[2],////added
        "offerStatus": offerStatus,
        "categoryId": catID,
        "partialFlag": true,
        "pageIndex": 1,
        "pageSize": 12
    };
    
    //$.get("/" + $("#CityID").val().split('$')[1] + '/CategoryWiseOffers/GetOfferStatusWiseProducts/', mydata, function (data) {////hide
    $.get("/" + $("#CityID").val().split('$')[1] + "/" + $("#CityID").val().split('$')[2] + '/CategoryWiseOffers/GetOfferStatusWiseProducts/', mydata, function (data) {////added

        $('#divCatOfferProducts').html(data);
        $('#divCatOfferProducts').fadeIn('fast');
        // $(".loader").attr("style", "display:none");
        $(".loader").hide();
        scrollTo('#divCatOfferProducts');
       
       
    });
    
}


