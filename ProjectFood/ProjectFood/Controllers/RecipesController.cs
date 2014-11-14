using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectFood.Models;
using System.Diagnostics;

namespace ProjectFood.Controllers
{
    public class RecipesController : Controller
    {
        private readonly DataBaseContext _db = new DataBaseContext();

        // GET: Recipes
        public ActionResult Index()
        {
            return View(_db.Recipes.Include(r => r.Ingredients).ToList());
        }

        // GET: Recipes/Details/5
        public ActionResult Details(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.ShoppingLists =
                    _db.Users.Include(s => s.ShoppingLists)
                        .First(u => u.Username == User.Identity.Name)
                        .ShoppingLists.ToList();
            }
            else
            {
                ViewBag.ShoppingLists = null;
            }

            if (id == null)
            {
                return RedirectToAction("index");
            }

            var recipe = _db.Recipes.Include(r => r.Ingredients).Single(x => x.ID == id);
            ViewBag.Author = _db.Users.First(u => u.Username == recipe.AuthorName).Name;
            if (recipe.Ingredients.Count > 0)
            {
                ViewBag.Recipe_Ingredient = _db.Recipe_Ingredient.Where(x => x.RecipeID == id).ToList();
            }
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // GET: Recipes/Create
        public ActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Register", "Account");
            }
        }
        // POST: Recipes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,AuthorName,Minutes,Instructions,Tags")] Recipe recipe)
        {
            _db.Recipes.Add(recipe);
            _db.SaveChanges();
            return RedirectToAction("CreateSecond/" + recipe.ID);

            //return View(recipe);
        }

        // GET: Recipes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (User.Identity.IsAuthenticated)
            {//test if user is the author of the recipe
                if (User.Identity.Name == null || User.Identity.Name != (_db.Recipes.FirstOrDefault(r => r.ID == id)).AuthorName)
                    return RedirectToAction("Index");
            }
            else { return RedirectToAction("Index"); }

            Recipe recipe = _db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,AuthorName,Minutes,Instructions,Tags")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(recipe).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("CreateSecond/" + recipe.ID);
            }
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var recipe = _db.Recipes.Find(id);


            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var recipe = _db.Recipes.Find(id);
            _db.Recipes.Remove(recipe);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult AddIngredient(int id, string name, double? amount, string unit)
        {
            var recipe = _db.Recipes.Include(r => r.Ingredients).Single(x => x.ID == id);
            Item tmpIngredient;

            if (name.Trim() == string.Empty)
            {
                return RedirectToAction("CreateSecond/" + id);
            }
            //allows for ingredienst with no amount and unit
            if (amount == null)
            {
                amount = 0;
            }
            //Search in GenericLItems for item
            Item knownItem = null;
            if (_db.Items.Any())
            {
                knownItem = _db.Items.SingleOrDefault(i => i.Name.CompareTo(name) == 0);
            }

            if (knownItem != null)
            {
                tmpIngredient = knownItem;
            }
            else
            {
                tmpIngredient = new Item() { Name = name };
            }

            var recipeIngredient = new Recipe_Ingredient() { RecipeID = id, Ingredient = tmpIngredient, Amount = (double)amount, Unit = unit };

            recipe.Ingredients.Add(tmpIngredient);
            _db.Recipe_Ingredient.Add(recipeIngredient);
            _db.SaveChanges();
            return RedirectToAction("CreateSecond/" + id);
        }
        public ActionResult CreateSecond(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            if (User.Identity.IsAuthenticated)
            {
                if (User.Identity.Name != (_db.Recipes.FirstOrDefault(r => r.ID == id)).AuthorName)
                    return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index"); 
                
            }

            
            Recipe recipe = _db.Recipes.Include(r => r.Ingredients).Single(x => x.ID == id);
            if (recipe.Ingredients.Count > 0)
            {
                ViewBag.Recipe_Ingredient = _db.Recipe_Ingredient.Where(x => x.RecipeID == id).ToList();
            }
            
            return View(recipe);
        }

        [HttpPost]
        public ActionResult AddItemToShoppingList(int itemId, int? shoppingListId, double? amount, string unit)
        {
            var tmpItem = _db.Items.Find(itemId);

            var shoppingList = _db.ShoppingLists.First(l => l.ID == shoppingListId);
            if (amount == null)
            {
                amount = 0;
            }
            var shoppingListItem = new ShoppingList_Item
            {
                Item = tmpItem,
                ShoppingList = shoppingList,
                Amount = (double)amount,
                Unit = unit,
            };

            _db.ShoppingList_Item.Add(shoppingListItem);

            shoppingList.Items.Add(tmpItem);

            _db.SaveChanges();

            return Json(new
            {
                Message = "Hajtroels",
                ItemId = itemId,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
