function EditShoppingListTitle(element, id){
	if (id == "unedited") {
		var title = $(element).text();
		$(element).html('<input type="text" value="' + title.trim() + '" />');
		$(element).id = "edited";
	}
};