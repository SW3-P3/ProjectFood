﻿function ChangeToCheck(json) {
    var offerButtonAll = '#AddOfferAll_' + json.OfferId;
    var offerButton = '#AddOffer_' + json.OfferId;

    $(offerButtonAll)
        .removeClass('btn-info')
        .addClass('btn-success')
        .blur();
    $(offerButtonAll + ' span')
        .removeClass('glyphicon-plus')
        .addClass('glyphicon-ok');

    $(offerButton)
        .removeClass('btn-info')
        .addClass('btn-success')
        .blur();
    $(offerButton + ' span')
        .removeClass('glyphicon-plus')
        .addClass('glyphicon-ok');
};

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

function ToggleLineThrough(element) {
    $(this).parent().toggleClass('text-strikethrough');
};

function ToggleBool() {
    if ($('input#RememberMe').val() == 'true') {
        $('input#RememberMe').val('false');
        $('span#check').removeClass('glyphicon-check');
        $('span#check').addClass('glyphicon-unchecked');
    } else {
        $('input#RememberMe').val('true');
        $('span#check').removeClass('glyphicon-unchecked');
        $('span#check').addClass('glyphicon-check');
    }
};

function ClickedStar(rating) {

    $('input#rateVal').val(rating);

    $('#starForm').submit();
};

function DrawStars(json) {

    for (var i = 1; i <= 5; i++) {
        if(json.rating >= i){
            $('div#' + i).children().removeClass('glyphicon-star-empty');
            $('div#' + i).children().addClass('glyphicon-star');
        } else {
            $('div#' + i).children().removeClass('glyphicon-star');
            $('div#' + i).children().addClass('glyphicon-star-empty');
        }
    }
};