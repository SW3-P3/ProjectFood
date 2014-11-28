$(function() {
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
    var offers = JSON.parse(json.jsonOffers);
    $('#modalTitle').html('Tilbud på ' + json.itemName);
    var offerTable = "<table class='table table-hover table-condensed'>";
    for (var i = 0; i < offers.length; i++) {
        offerTable += "<tr id='" +
            offers[i].ID + "' ><td>" +
            offers[i].Heading + "</td><td class='col-md-1'>" +
            offers[i].Unit + "</td><td class='col-md-1'>" +
            offers[i].Price + " kr.</td><td class='col-md-1'>" +
            offers[i].Store + "</td></tr>";
    }
    offerTable += "</table>";
    $('#modalBody').html(offerTable);
    $('#offerModal').modal('show');
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