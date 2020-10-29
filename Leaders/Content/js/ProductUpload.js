$('#search').autocomplete({

    source: function (request, response) {
        alert("this is search");
        if ($('#search').val().length <= 2)
            return false;
        var autocompleteUrl = '/AutoComplete/Index?term=' + $("#search").val() + '&searchBy=' + 2;
        $.ajax({
            url: autocompleteUrl,
            type: 'GET',
            cache: false,
            dataType: 'json',
            success: function (json) {
                // call autocomplete callback method with results
                response($.map(json, function (data, id) {
                    //alert(JSON.stringify(data));
                    return {
                        ID: data.ID,
                        label: data.Name,
                        Head: data.Head,
                        Abbr: data.Abbr,
                        Seperator: data.Seperator,
                        Products: data.Products

                    };
                }));
            },
            error: function (xmlHttpRequest, textStatus, errorThrown) {
                console.log('some error occured', textStatus, errorThrown);
            }
        });
    },
    minLength: 2,
    focus: function (event, ui) {
        //alert(JSON.stringify(ui.item));
        //alert(ui.item.label);
        $('[id*=search]').val(ui.item.label);

        return false;
    },
    select: function (event, ui) {
        //alert(JSON.stringify(ui.item));
        //alert(ui.item.value);
        $('#search').val(ui.item.label);
        //$('#search').val(ui.item.Abbr);
        //$('#selectedID').html(ui.item.ID);
        //$('#selectedHead').html(ui.item.Head);


        if (ui.item.Head == "Product") {

            window.location.href = '/PreviewItem/Index?itemID=' + ui.item.ID + "&itemName=" + ui.item.label.replace(" & ", "+%26+");
        }
        else if (ui.item.Head == "Category") {

            window.location.href = '/Product/Products?item=' + ui.item.label.replace(" & ", "+%26+") + "&parentCategoryId=" + ui.item.ID;
        }
        else {
            window.location.href = '/Product/Products?shopID=' + ui.item.ID + "&shopName=" + ui.item.label;
        }

        return false;
    }


});
$(function () {
    $("#search").val("");
    //$("#search").focus();
});