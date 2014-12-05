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
            DeleteAllRecipiesHelper();
            return RedirectToAction("Index");
	    }

	    private void DeleteAllRecipiesHelper()
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

        public ActionResult DeleteAllRecipiesRatings()
        {
            DeleteAllRecipiesRatingsHelper();
            return RedirectToAction("Index");
        }

        private void DeleteAllRecipiesRatingsHelper()
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
	}
}