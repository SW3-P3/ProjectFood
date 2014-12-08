using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using ProjectFood.Models;
using ProjectFood.Models.Api;
using EntityFramework.BulkInsert.Extensions;

namespace ProjectFood.Controllers
{
    public class AdminPanelController : Controller
    {
        private readonly DataBaseContext _db = new DataBaseContext();

        //
        // GET: /AdminPanel/
        public ActionResult Index()
        {
            #region Dropdown
            var datapointdates = GetJsonFileNames().Select(item => DateTime.FromFileTime(long.Parse(item.Substring(item.LastIndexOf('\\') + 1, 18)))).ToList();
            datapointdates.Add(DateTime.Now);
            ViewBag.DateTimes = datapointdates.Any() ? datapointdates : new List<DateTime>();
            #endregion
            return View();
        }
        #region Database
        public ActionResult DeleteEntireDatabase()
        {
            var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)_db).ObjectContext;

            var tables = new List<string>
            {
                "Items",
                "OfferItems",
                "Offers",
                "OfferSentTo",
                "Prefs",
                "Ratings",
                "Recipe_INgredient",
                "Recipes",
                "Recipie_Item",
                "ShoppingList_Item",
                "ShoppingListItems",
                "ShoppingLists",
                "Users",
                "UserShoppingLists"
            };

            objCtx.ExecuteStoreCommand("exec sp_MSforeachtable \"declare @name nvarchar(max); set @name = parsename('?', 1); exec sp_MSdropconstraints @name\"");


            foreach (var derp in tables.Select(con => string.Format("TRUNCATE TABLE [{0}]", con)))
            {
                objCtx.ExecuteStoreCommand(derp);
            }

            _db.SaveChanges();

            Response.Cookies.Remove(".AspNet.ApplicationCookie");
            return RedirectToAction("Index");

        }
        #endregion
        #region Offers
        public IEnumerable<string> GetJsonFileNames()
        {
            var path = Server.MapPath("~/App_Data/jsonData");
            var filePaths = Directory.GetFiles(path);
            return filePaths.ToList();
        }

        public ActionResult ImportOffersFromFiles()
        {
            var filePaths = GetJsonFileNames();
            var apiOfferList = new List<ApiOffer>();

            foreach (var text in filePaths.Select(filePath => new StreamReader(filePath)).Select(streamReader => streamReader.ReadToEnd()))
            {
                apiOfferList.AddRange(JsonConvert.DeserializeObject<List<ApiOffer>>(text));
            }

            _db.BulkInsert(apiOfferList.Select(ApiOfferToOffer));

            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        private Offer ApiOfferToOffer(ApiOffer o)
        {
            return new Offer
            {
                eTilbudsavisID = o.id,
                Heading = o.heading,
                Begin = o.run_from,
                End = o.run_till,
                Store = o.branding.name,
                Price = o.pricing.price,
                Unit = o.quantity.unit != null ? o.quantity.size.@from + " " + o.quantity.unit.symbol : " "
            };
        }
        public ActionResult LoadNewOffers()
        {
            var offerControler = new OfferController();
            offerControler.ImportOffers();
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTimePeriodDropdown(FormCollection form)
        {
            try
            {
                var date = DateTime.Parse(form["ListSelect"]);
                GlobalVariables.CurrentSystemTime = date;

            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTimePeriod(int? year, int? month, int? day)
        {
            if (year == null || month == null || day == null)
                return RedirectToAction("Index");
            try
            {
                var date = new DateTime((int)year, (int)month, (int)day);
                GlobalVariables.CurrentSystemTime = date;

            }
            catch (ArgumentOutOfRangeException exception)
            {
                Debug.WriteLine(exception);
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