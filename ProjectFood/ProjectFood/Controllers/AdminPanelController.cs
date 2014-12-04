using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using ProjectFood.Models;

namespace ProjectFood.Controllers
{
	public class AdminPanelController : Controller
	{
		private readonly DataBaseContext _db = new DataBaseContext();

		//
		// GET: /AdminPanel/
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult LoadNewOffers()
		{
			var offerControler = new OfferController();
			offerControler.ImportOffers();
			return RedirectToAction("Index");
		}

		public ActionResult DeleteAllShoppinglistItems()
		{
			DeleteAllShoppinglistItemsHelper();
			return RedirectToAction("Index");
		}

		private void DeleteAllShoppinglistItemsHelper()
		{
			while (_db.ShoppingList_Item.Any())
			{
				if (_db.ShoppingList_Item.FirstOrDefault().Item != null)
					_db.Items.Remove(_db.ShoppingList_Item.FirstOrDefault().Item);

				_db.ShoppingList_Item.Remove(_db.ShoppingList_Item.First());
				_db.SaveChanges();
			}
		}

		public ActionResult DeleteAllShoppinglists()
		{
			DeleteAllShoppinglistsHelper();
			return RedirectToAction("Index");
		}

		private void DeleteAllShoppinglistsHelper()
		{
			DeleteAllShoppinglistItemsHelper();

			while (_db.ShoppingLists.Any(x => x.Title != "watchlist"))
			{
				_db.ShoppingLists.Remove(_db.ShoppingLists.First(x => x.Title != "watchlist"));
				_db.SaveChanges();
			}
		}

	    public ActionResult DeleteAllRecipies()
	    {
	        DeleteAllRecipieIngredients();
            return RedirectToAction("Index");
	    }

	    private void DeleteAllRecipieIngredients()
	    {
            while (_db.Recipe_Ingredient.Any())
            {
                if (_db.Recipe_Ingredient.FirstOrDefault().Ingredient != null)
                    _db.Items.Remove(_db.Recipe_Ingredient.First().Ingredient);

                _db.Recipe_Ingredient.Remove(_db.Recipe_Ingredient.First());
                _db.SaveChanges();
            }
	    }
	}
}