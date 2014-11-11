function ChangeToCheckRecipe(json) {
    var recipeButton = '#AddItem_' + json.ItemId;

    $(recipeButton)
        .removeClass('btn-info')
        .addClass('btn-success')
        .blur();
    $(recipeButton + ' span')
        .removeClass('glyphicon-plus')
        .addClass('glyphicon-ok');
};