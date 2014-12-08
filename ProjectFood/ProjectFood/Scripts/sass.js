$(function () {
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

    MakeSnackbar("Gemt&hellip;", "glyphicon-ok");
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
    MakeSnackbar("&nbsp;&nbsp;" + message + "&hellip;", "glyphicon-ok");
};

function ShareStatus(json) {
    if (json.Success == 'true') {
        $('#errorMessage').addClass('hidden');
        ShoppingListShare(json.Message);
    } else {
        MakeSnackbar(json.Message, "glyphicon-exclamation-sign");
        $('#errorMessage').html(json.Message).removeClass('hidden');
    }
};

function EditAmount(itemName, itemID, amount, unit) {
    $('#editModalTitle').html('Sæt ny mængde for ' + itemName);
    $('input#itemID').val(itemID);
    $('input#amount').val(parseFloat(amount.replace(",", ".")).toString());
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

    MakeSnackbar("Vurdering givet&hellip;", "glyphicon-star");
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

    MakeSnackbar("Vare tilføjet til " + json.shoppingListTitle, "glyphicon-ok")
};

function GetSelectedList() {
    $('input#shoppingListId').val($('#Selection').val());
    MakeSnackbar("Indkøbsliste valgt&hellip;");
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
    var store = (json.store).replace("ø", "");
    var table = $('<table>', { 'class': 'table table-striped' });
    var table_head = $('<thead>').html("<tr>"
                    + "<th class='col-md-1 text-center'>Tilføj</th>"
                    + "<th class='col-md-7'>Navn</th>"
                    + "<th class='col-md-1'>Pris</th>"
                    + "<th class='col-md-1'>Mængde</th>"
                    + "<th class='col-md-1'><span class='hidden-xs'>Udløber</span></th>"
                    + "<th class='col-md-1" +
                    (store == "all" ? "" : " hidden") + 
                    "'>Butik</th></tr>")
    table.append(table_head);
    $.each(jsonOffers, function (index, offer) {
        var table_row = $('<tr>');
        var all = store == "all" ? "All_" : "_";
        var add_button = $('<button>', { id: 'AddOffer' + all + offer.ID, 'class': 'btn btn-info btn-xs btn-block' })
            .html("<span class='glyphicon glyphicon-plus'></span>");
        add_button.click(function () {
            $('input#offerID').val(offer.ID).parents().submit();
        })
        var table_add = $('<td>').append(add_button);
        var table_heading = $('<td>', { html: offer.Heading});
        var table_price = $('<td>', { html: offer.Price + " kr." });
        var table_unit = $('<td>', { html: offer.Unit });
        var table_end = $('<td>', { html: (new Date(parseInt(offer.End.substr(6))).toLocaleDateString('da-DK')), 'class':"hidden-xs" });
        var table_store = $('<td>', { html: offer.Store, 'class': store == "all" ? "" : "hidden"});
        table_row.append(table_add).append(table_heading).append(table_price).append(table_unit).append(table_end).append(table_store);
        table.append(table_row);
    })

    $('div#' + store).children('#offerTable').html(table);

    //page magic
    var pager = $('div#' + store).children('#pages').children('nav').children('ul');
    var curr = pager.children('li#currentPage');
    var currVal = curr.children().children('span#val').html();
    if (currVal != json.page) {
        var prev = pager.children('li#previousPage');
        var next = pager.children('li#nextPage');
 
        if (json.page > 1) {
            prev.removeClass('disabled');
        } else {
            prev.addClass('disabled');
        }
        if (json.page == curr.children().children('span#maxPage').html()) {
            next.addClass('disabled');
        } else {
            next.removeClass('disabled');
        }

        prev.children().children('#val').html(json.page - 1);
        next.children().children('#val').html(json.page + 1);
        curr.children().children('#val').html(json.page);
    }
};

function ChangePage(element) {
    if (!$(element).parents().hasClass('disabled')) {
        var gotoPage = $(element).children('#val').html();
        $(element).parentsUntil('div#pager').children('form').children('input#page').val(gotoPage);
        $(element).parentsUntil('div#pager').children().submit();
    }    
};
//END_Offers

//WatchList
function GetSelectedListWL() {
    $('input#SelectedList').val($('#Selection').val());
    MakeSnackbar("Indkøbsliste valgt");
};

function ChangeButton(json) {
    $('#AddButton_' + json.offerID).removeClass('btn-info').addClass('btn-success');
    $('#AddButton_' + json.offerID).children().removeClass('glyphicon-plus').addClass('glyphicon-ok');
    MakeSnackbar("Tilføjet til indkøbsliste&hellip;", "glyphicon-ok");
};
//END_WatchList

function MakeSnackbar(text, glyphicon) {
    if (typeof glyphicon !== "undefined") {
        var glyphiconContainer = $('<div>');
        var span = $('<span>').addClass('glyphicon').addClass(glyphicon);
        glyphiconContainer.append(span);
        $.snackbar({ content: glyphiconContainer.html() + "&nbsp;&nbsp;" + text });
    } else {
        $.snackbar({ content: text });
    }
};