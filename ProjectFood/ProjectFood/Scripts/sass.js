﻿$(function () {
    $('[data-toggle="tooltip"]').tooltip()
});

//User
function ToggleBoolByID(store) {
    if ($('input#storeCheck_' + store).val() == 'true') {
        $('input#storeCheck_' + store).val('false');
        $('span#check_' + store).removeClass('glyphicon-check').addClass('glyphicon-unchecked');
        $('span#storeName_' + store).addClass('text-muted').addClass('text-strikethrough');
    } else {
        $('input#storeCheck_' + store).val('true');
        $('span#check_' + store).removeClass('glyphicon-unchecked').addClass('glyphicon-check');
        $('span#storeName_' + store).removeClass('text-muted').removeClass('text-strikethrough');
    }

    $.snackbar({ content: '<span class="glyphicon glyphicon-ok"></span>&nbsp;&nbsp; Gemt&hellip;' });
};

function ToggleBoolRememberMe() {
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
//END_User

//ShoppingList
function ToggleBoughtStatus(element) {
    $(element).siblings('#itemTitle').toggleClass('text-strikethrough').toggleClass('text-muted');
    $(element).siblings('#hint').toggleClass('hidden');
};

function ShoppingListShare(message) {
    $.snackbar({ content: '<span class="glyphicon glyphicon-ok"></span>&nbsp;&nbsp;' + message + '&hellip;' });
};

function ShareStatus(json) {
    if (json.Success == 'true') {
        $('#errorMessage').addClass('hidden');
        ShoppingListShare(json.Message);
    } else {
        $('#errorMessage').html(json.Message).removeClass('hidden');
    }
};

function EditAmount(itemName, itemID, amount, unit) {
    $('#modalTitle').html('Sæt ny mængde for ' + itemName);
    $('input#itemID').val(itemID);
    $('input#amount').val(amount);
    $('input#unit').val(unit);

    $('#EditModal').modal('show');
};
//END_ShoppingList

//Recipe
function ClickedStar(rating) {
    $('input#rateVal').val(rating);
    $('#starForm').submit();
};

function UpdateStarsAndAvg(json) {
    $('span#avgRating').html(json.avgRating);
    $('span#numRatings').html(json.numRatings);

    DrawStars(json.rating);

    $.snackbar({ content: '<span class="glyphicon glyphicon-star"></span>&nbsp;&nbsp; Vurdering givet&hellip;' });
};

function DrawStars(rating) {
    for (var i = 1; i <= 5; i++) {
        if (rating >= i) {
            $('span#' + i).removeClass('glyphicon-star-empty');
            $('span#' + i).addClass('glyphicon-star');
        } else {
            $('span#' + i).removeClass('glyphicon-star');
            $('span#' + i).addClass('glyphicon-star-empty');
        }
    }
};

function ChangeToCheckRecipe(json) {
    var recipeButton = '#AddItem_' + json.itemID;

    $(recipeButton)
        .removeClass('btn-info')
        .addClass('btn-success')
        .blur();
    $(recipeButton + ' span')
        .removeClass('glyphicon-plus')
        .addClass('glyphicon-ok');

    $.snackbar({ content: '<span class="glyphicon glyphicon-ok"></span>&nbsp;&nbsp;Vare tilføjet til ' + json.shoppingListTitle });
};

function GetSelectedList() {
    $('input#shoppingListId').val($('#Selection').val());
};

function AddAllTheItems(numItems) {
    for (var i = 0; i < numItems; i++) {
        $('form#AddItem_' + i).submit();
    }
};

function ChangeNumPersons(element, numItems) {
    var numPersons = $(element).val();
    var amountPP;
    var amount;

    for (var i = 0; i < numItems; i++) {
        amountPP = $('span#amountPP_' + i).html();
        amount = amountPP * numPersons;
        $('input#amount_' + i).val(parseFloat(amount).toLocaleString('da-DK'));
        $('span#amount_' + i).html(parseFloat(amount).toLocaleString('da-DK'));
    }
};
//END_Recipe

//Offers
function ChangeToCheckOffer(json) {
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

function ShowOffers(json) {
    var jsonOffers = JSON.parse(json.jsonOffers);
    var table = $('<table>', { 'class': 'table table-striped' });
    var table_head = $('<thead>').html("<tr>"
                    + "<th class='col-md-1 text-center'>Tilføj</th>"
                    + "<th class='col-md-7'>Navn</th>"
                    + "<th class='col-md-1'>Pris</th>"
                    + "<th class='col-md-1'>Mængde</th>"
                    + "<th class='col-md-1'>Udløber</th>"
                    + "<th class='col-md-1" +
                    (json.store == "all" ? "" : " hidden") + 
                    "'>Butik</th></tr>")
    table.append(table_head);
    $.each(jsonOffers, function (index, offer) {
        var table_row = $('<tr>');
        var all = json.store == "all" ? "All_" : "_";
        var add_button = $('<button>', {id: 'AddOffer' + all + offer.ID, 'class': 'btn btn-info btn-xs btn-block' }).html("<span class='glyphicon glyphicon-plus'></span>");
        add_button.click(function () {
            $('input#offerID').val(offer.ID).parents().submit();
        })
        var table_add = $('<td>').append(add_button);
        var table_heading = $('<td>', { html: offer.Heading});
        var table_price = $('<td>', { html: offer.Price });
        var table_unit = $('<td>', { html: offer.Unit });
        var table_end = $('<td>', { html: (new Date(parseInt(offer.End.substr(6))).toLocaleDateString('da-DK')) });
        var table_store = $('<td>', { html: offer.Store, 'class': json.store == "all" ? "" : "hidden"});
        table_row.append(table_add).append(table_heading).append(table_price).append(table_unit).append(table_end).append(table_store);
        table.append(table_row);
    })

    $('div#' + json.store).children('#offerTable').html(table);
    $('div#' + json.store).children('#pages').children('nav').children('ul').children('li').removeClass('active');
    $('div#' + json.store).children('#pages').children('nav').children('ul').children('li#' + json.page).addClass('active');
};

//END_Offers

//WatchList
function GetSelectedListWL() {
    $('input#SelectedList').val($('#Selection').val());

    $.snackbar({ content: "Indkøbsliste valgt", style: "snackbar" });
};

function ChangeButton(json) {
    $('#AddButton_' + json.offerID).removeClass('btn-primary').addClass('btn-success');
    $('#AddButton_' + json.offerID).children().removeClass('glyphicon-plus').addClass('glyphicon-ok');
};
//END_WatchList
