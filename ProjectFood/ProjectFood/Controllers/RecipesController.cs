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
        private ShoppingListContext db = new ShoppingListContext();

        // GET: Recipes
        public ActionResult Index()
        {
            return View(db.Recipes.Include(r => r.Ingredients).ToList());
        }

        // GET: Recipes/Details/5
        public ActionResult Details(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.ShoppingLists =
                    db.Users.Include(s => s.ShoppingLists)
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
            Recipe recipe = db.Recipes.Include(r => r.Ingredients).Single(x => x.ID == id);
            if (recipe.Ingredients.Count > 0)
            {
                ViewBag.Recipe_Ingredient = db.Recipe_Ingredient.Where(x => x.RecipeID == id).ToList();
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
            return View();
        }
        // POST: Recipes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Amount,Unit,Ingredients,Minutes,Instructions,Tags")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                db.Recipes.Add(recipe);
                db.SaveChanges();
                return RedirectToAction("CreateSecond/" + recipe.ID);
            }

            return View(recipe);
        }

        // GET: Recipes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("CreateSecond/" + id);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Minutes,Instructions,Tags")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipe).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
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
            Recipe recipe = db.Recipes.Find(id);


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
            Recipe recipe = db.Recipes.Find(id);
            db.Recipes.Remove(recipe);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult AddIngredient(int id, string name, double? amount, string unit)
        {
            var recipe = db.Recipes.Include(r => r.Ingredients).Single(x => x.ID == id);
            var tmpIngredient = new Item() { Name = name };

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
            if (db.Items.Count() > 0)
            {
                knownItem = db.Items.Where(i => i.Name.CompareTo(name) == 0).SingleOrDefault();
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
            db.Recipe_Ingredient.Add(recipeIngredient);
            db.SaveChanges();
            return RedirectToAction("CreateSecond/" + id);
        }
        public ActionResult CreateSecond(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Recipe recipe = db.Recipes.Include(r => r.Ingredients).Single(x => x.ID == id);
            if (recipe.Ingredients.Count > 0)
            {
                ViewBag.Recipe_Ingredient = db.Recipe_Ingredient.Where(x => x.RecipeID == id).ToList();
            }
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        [HttpPost]
        public ActionResult AddItemToShoppingList(int itemId, int? shoppingListId, double? amount, string unit)
        {
            var tmpItem = db.Items.Find(itemId);

            var shoppingList = db.ShoppingLists.First(l => l.ID == shoppingListId);
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

            db.ShoppingList_Item.Add(shoppingListItem);

            shoppingList.Items.Add(tmpItem);

            db.SaveChanges();

            return Json(new
            {
                Message = "Hej troels",
                ItemId = itemId,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
