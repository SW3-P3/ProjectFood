using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
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
        #region Offers
        public ActionResult LoadNewOffers()
		{
			var offerControler = new OfferController();
			offerControler.ImportOffers();
			return RedirectToAction("Index");
		}

	    public ActionResult ChangeTimePeriod(int? year, int? month, int? day)
	    {
	        if(year == null || month == null || day == null)
                return RedirectToAction("Index");
	        try
	        {
                var date = new DateTime((int)year, (int)month, (int)day);
                GlobalVariables.CurrentSystemTime = date;

	        }
	        catch (ArgumentOutOfRangeException exception)
	        {
                return RedirectToAction("Index");
            }
	        return RedirectToAction("Index");
	    }
        #endregion
        #region ShoppingList
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
        #endregion 
        #region Recipes
        public ActionResult DeleteAllRecipes()
	    {
            DeleteAllRecipesHelper();
            return RedirectToAction("Index");
	    }

	    private void DeleteAllRecipesHelper()
	    {
	        while (_db.Recipes.Any())
	        {
	            var recipe = _db.Recipes.Include(r => r.Ratings).FirstOrDefault();

	            if (recipe.Ratings.Count > 0)
	            {
	                _db.Ratings.RemoveRange(_db.Ratings.Where(r => r.Recipe.ID == recipe.ID));
	                recipe.Ratings.Clear();
	            }

	            _db.Recipes.Remove(recipe);
	            _db.SaveChanges();
	        }
	    }

        public ActionResult DeleteAllRecipesRatings()
        {
            DeleteAllRecipesRatingsHelper();
            return RedirectToAction("Index");
        }

        private void DeleteAllRecipesRatingsHelper()
        {
            while (_db.Recipes.Include(x => x.Ratings).ToList().Any(x => x.Ratings.Any()))
            {
                var recipe = _db.Recipes.Include(r => r.Ratings).FirstOrDefault(x => x.Ratings.Any());

                if (recipe.Ratings.Count > 0)
                {
                    _db.Ratings.RemoveRange(_db.Ratings.Where(r => r.Recipe.ID == recipe.ID));
                }

                _db.SaveChanges();
            }
        }
        #endregion
    }
}