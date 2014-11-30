function ChangeToCheck(json) {
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

function ShowOffers(json) {
    var offers = JSON.parse(json.jsonOffers);
    var offerTable = "<table class='table table-striped'>";
    offerTable += "<thead><tr>"
                    + "<th class='col-md-1 text-center'>Tilføj</th>"
                    + "<th class='col-md-7'>Navn</th>"
                    + "<th class='col-md-1'>Pris</th>"
                    + "<th class='col-md-1'>Mængde</th>"
                    + "<th class='col-md-1'>Udløber</th>";
    offerTable += json.store == 'all' ? "<th class='col-md-1'>Store</th></tr></thead>" : "</tr></thead>";
    var all = json.store == 'all' ? "All" : "";
    for (var i = 0; i < offers.length; i++) {
        var endDate = new Date(parseInt(offers[i].End.substr(6)));
        offerTable += "<tr id='" + offers[i].ID + "' ><td>" +
            "<form action='/Offer/AddOfferToShoppingList?offerID=" + offers[i].ID +
            "' data-ajax='true' data-ajax-method='POST' data-ajax-success='ChangeToCheck' id='form0' method='post'>" +
            "<input type='hidden' name='shoppingListId' value=''>" + "<button id='AddOffer" + all + "_" + offers[i].ID + "' type='submit' class='btn btn-info btn-xs btn-block'>" +
            "<span class='glyphicon glyphicon-plus'></span></button></form>" + 
            offers[i].ID + "</td><td>" +
            offers[i].Heading + "</td><td class='col-md-1'>" +
            offers[i].Price + "</td><td class='col-md-1'>" +
            offers[i].Unit + " kr.</td><td class='col-md-1'>" +
            endDate.toLocaleDateString("da-DK") + "</td>";
        offerTable += json.store == 'all' ? "<td class='col-md-1'>" + offers[i].Store + "</td></tr>" : "</tr>";
    }
    offerTable += "</table>";
    $('div#' + json.store).children('#offerTable').html(offerTable);
    alert(json.store);
    alert(json.page);
    $('div#' + json.store).children('#pages').children('nav').children('ul').children('li').removeClass('active');
    $('div#' + json.store).children('#pages').children('nav').children('ul').children('li#' + json.page).addClass('active');
};

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

function EditAmount(itemName, itemID, amount, unit) {
    $('#modalTitle').html('Sæt ny mængde for ' + itemName);
    $('input#itemID').val(itemID);
    $('input#amount').val(amount);
    $('input#unit').val(unit);

    $('#EditModal').modal('show');
};

$(function() {
    $('[data-toggle="tooltip"]').tooltip()
});

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
