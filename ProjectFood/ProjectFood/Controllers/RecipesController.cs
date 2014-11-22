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
            if (User.Identity.IsAuthenticated)
            {
                Session["ScreenName"] = _db.Users.First(u => u.Username == User.Identity.Name).Name;
            }

            return View(_db.Recipes.Include(r => r.Ingredients).ToList());
        }

        // GET: Recipes/Details/5
        public ActionResult Details(int? id, int? numPersons)
        {
            if (id == null)
            {
                return RedirectToAction("index");
            }

            var recipe = _db.Recipes.Include(r => r.Ingredients).Include(r => r.Ratings).Single(x => x.ID == id);
            ViewBag.Author = _db.Users.First(u => u.Username == recipe.AuthorName);
            ViewBag.OriginalAuthor = _db.Users.SingleOrDefault(u => u.Username == recipe.OriginalAuthorName);
            if (recipe.Ingredients.Count > 0)
            {
                ViewBag.Recipe_Ingredient = _db.Recipe_Ingredient.Where(x => x.RecipeID == id).ToList();
            }

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.ShoppingLists =
                    _db.Users.Include(s => s.ShoppingLists)
                        .First(u => u.Username == User.Identity.Name)
                        .ShoppingLists.ToList();
                ViewBag.UserRating = recipe.Ratings.SingleOrDefault(r => r.User.Username == User.Identity.Name);
            }
            else
            {
                ViewBag.ShoppingLists = null;
                ViewBag.UserRating = 0;
            }

            // ?? means if null assign the right side, else the left side.
            ViewBag.numPersons = numPersons ?? 4;

            //calculate average score

            ViewBag.AverageScore = recipe.Ratings.Count > 0 ? (decimal)recipe.Ratings.Select(r => r.Score).Average() : 0;
            ViewBag.RatingCount = recipe.Ratings.Count();

            return View(recipe);
        }

        // GET: Recipes/Create
        public ActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Register", "Account");
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,OriginalAuthorName,AuthorName,Minutes,Instructions,Tags")] Recipe recipe)
        {
            _db.Recipes.Add(recipe);
            _db.SaveChanges();
            return RedirectToAction("CreateSecond/" + recipe.ID);
        }

        // GET: Recipes/Edit/5
        public ActionResult Edit(int? id, bool fork)
        {
            if (id == null)
            {
                return RedirectToAction("index");
            }

            var recipe = _db.Recipes.Include(r => r.Ingredients).First(r => r.ID == id);

            if (recipe == null)
            {
                return HttpNotFound();
            }

            if (User.Identity.IsAuthenticated)
            {
                if (fork)
                {
                    var originalRecipe = recipe;
                    var forkedRecipe = new Recipe
                    {
                        OriginalAuthorName = originalRecipe.OriginalAuthorName,
                        AuthorName = User.Identity.Name,
                        Title = originalRecipe.Title,
                        Ingredients = new List<Item>(originalRecipe.Ingredients),
                        Tags = originalRecipe.Tags,
                        Minutes = originalRecipe.Minutes,
                        Instructions = originalRecipe.Instructions
                    };
                    _db.Recipes.Add(forkedRecipe);

                    foreach (var ingredient in _db.Recipe_Ingredient.Where(i => i.RecipeID == originalRecipe.ID))
                    {
                        var tmpRecipeIngredient = new Recipe_Ingredient
                        {
                            RecipeID = forkedRecipe.ID,
                            IngredientID = ingredient.IngredientID,
                            Recipe = forkedRecipe,
                            Ingredient = ingredient.Ingredient,
                            AmountPerPerson = ingredient.AmountPerPerson,
                            Unit = ingredient.Unit
                        };
                        _db.Recipe_Ingredient.Add(tmpRecipeIngredient);
                    }
                    _db.SaveChanges();
                    recipe = forkedRecipe;
                }
            }
            else
            {
                return RedirectToAction("Details/" + id);
            }

            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,OriginalAuthorName,AuthorName,Minutes,Instructions,Tags")] Recipe recipe)
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
                return RedirectToAction("index");
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

        public ActionResult AddIngredient(int id, string name, double? amountPerPerson, string unit, int numPersons)
        {
            var recipe = _db.Recipes.Include(r => r.Ingredients).Single(x => x.ID == id);

            if (name.Trim() == string.Empty)
            {
                return RedirectToAction("CreateSecond/" + id);
            }
            //allows for ingredienst with no amount and unit
            if (amountPerPerson == null)
            {
                amountPerPerson = 0;
            }

            // If numpersons is set to zero, this value shouldn't be infinity (will give db exception)
            amountPerPerson = numPersons > 0 ? (double)amountPerPerson / numPersons : 0;

            //Search in GenericLItems for item
            Item knownItem = null;
            if (_db.Items.Any())
            {
                knownItem = _db.Items.SingleOrDefault(i => i.Name.CompareTo(name) == 0);
            }

            var tmpIngredient = knownItem ?? new Item { Name = name };

            if (recipe.Ingredients.Contains(tmpIngredient))
            {
                _db.Recipe_Ingredient.First(i => i.Recipe == recipe && i.Ingredient == tmpIngredient).AmountPerPerson = (double)amountPerPerson;
            }
            else
            {
                var recipeIngredient = new Recipe_Ingredient { RecipeID = id, Ingredient = tmpIngredient, AmountPerPerson = (double)amountPerPerson, Unit = unit };
                recipe.Ingredients.Add(tmpIngredient);
                _db.Recipe_Ingredient.Add(recipeIngredient);
            }

            _db.SaveChanges();
            return RedirectToAction("CreateSecond/" + id);
        }

        public ActionResult RemoveIngredient(int id, int ingredientId)
        {
            var recipe = _db.Recipes.Include(r => r.Ingredients).Single(x => x.ID == id);

            //Find the item to be deleted, and remove it from the shopping list
            var rmIngredient = recipe.Ingredients.ToList().Find(x => x.ID == ingredientId);
            recipe.Ingredients.Remove(rmIngredient);

            //Find the item in the ShoppingList_Item table
            var rmRecipeIngredient = _db.Recipe_Ingredient.SingleOrDefault(x => x.IngredientID == ingredientId && x.RecipeID == id);
            //... and remove it
            if (rmRecipeIngredient != null)
                _db.Recipe_Ingredient.Remove(rmRecipeIngredient);

            //Save the changes in the database
            _db.SaveChanges();

            //Update the users view of the shoppinglist
            return RedirectToAction("CreateSecond/" + id);
        }

        public ActionResult CreateSecond(int? id, int? numPersons)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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

            var recipe = _db.Recipes.Include(r => r.Ingredients).Single(x => x.ID == id);
            if (recipe.Ingredients.Count > 0)
            {
                ViewBag.Recipe_Ingredient = _db.Recipe_Ingredient.Where(x => x.RecipeID == id).ToList();
            }

            ViewBag.numPersons = numPersons ?? 4;

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
        public ActionResult RateRecipe(User u, decimal s, Recipe r)
        {
            var rating = new Rating
            {
                Recipe = r,
                Score = s,
                User = u
            };
            _db.Ratings.Add(rating);
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddRating(int id, int rating)
        {
            Recipe recipe = _db.Recipes.Include(r => r.Ratings).Single(x => x.ID == id);
            User user = _db.Users.Include(u => u.Ratings).SingleOrDefault(u => u.Username == User.Identity.Name);
            //Den når aldrig forbi ?? pga. *OrDefault(), vel?
            Rating prevRating = user.Ratings.FirstOrDefault(r => r.Recipe.ID == id) ?? null; 

            if (prevRating != null)
            {
                prevRating.Score = rating;
            }
            else
            {
                Rating rate = new Rating
                {
                    Recipe = recipe,
                    Score = rating,
                    User = user
                };

                user.Ratings.Add(rate);
                recipe.Ratings.Add(rate);
                _db.Ratings.Add(rate);
            }

            _db.SaveChanges();

            return Json(new { 
                rating = rating, 
                avgRating = recipe.Ratings.Count > 0 ? recipe.Ratings.Select(r => r.Score).Average().ToString("0.0") : "0.0",
                numRatings = recipe.Ratings.Count()
            });
        }
    }
}
