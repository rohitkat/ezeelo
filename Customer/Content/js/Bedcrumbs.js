
function ShowBedcrumbs(categoryID)
{
    var catID = categoryID.id.split('$');
    $("[id*=thirdLevelBedCrubmbs]").html( catID[1].replace(/#/g, ' ') );
    $("[id*=fourthLevelBedCrubmbs]").html(catID[3].replace(/#/g, ' '));
    $("[id*=fourthLevelBedCrubmbs]").attr("href", "../Product/Products?parentCategoryId=" + catID[2] + "&item=" + catID[3]);
}