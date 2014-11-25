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