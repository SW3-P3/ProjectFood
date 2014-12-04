using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime;
using Microsoft.Ajax.Utilities;
using ProjectFood.Models;
using System.Diagnostics;
using WebGrease.Css.Extensions;

namespace ProjectFood.Controllers
{
    public class RecipesController : Controller
    {
        private readonly DataBaseContext _db = new DataBaseContext();

        // GET: Recipes
        public ActionResult Index(string sort)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Selected = "New";

                if (sort.IsNullOrWhiteSpace() || sort.Equals("New"))
                {
                    var rec = _db.Recipes.Include(r => r.Ingredients).Include(x => x.Ratings).ToList();
                    rec.Reverse();
                    return View(rec);
                }

                if (sort.Equals("Old")) {
                    ViewBag.Selected = "Old";
                    return View(_db.Recipes.Include(r => r.Ingredients).Include(x => x.Ratings).ToList());
                } else if (sort.Equals("Recommend")) {
                    var sortedRecipes = RecommendRecipes(_db.Users.First(u => u.Username == User.Identity.Name));
                    ViewBag.Selected = "Recommend";
                    return View(sortedRecipes.ToList());
                } else if (sort.Equals("High")) {
                    ViewBag.Selected = "High";
                    return View(_db.Recipes.Include(x => x.Ingredients).Include(x => x.Ratings).OrderByDescending(x => x.Ratings.Select(y => y.Score).Average()));
                } else {
                    var rec = _db.Recipes.Include(r => r.Ingredients).Include(x => x.Ratings).ToList();
                    rec.Reverse();
                    return View(rec);
                }                
            }

            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action() });
            
         }

        // GET: Recipes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("index");
            }

            var recipe = _db.Recipes.Include(r => r.Ingredients).Include(r => r.Ratings).FirstOrDefault(x => x.ID == id);
            if (recipe == null)
            {
                return RedirectToAction("index");
            }
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
                ViewBag.UserRating = recipe.Ratings.FirstOrDefault(r => r.User.Username == User.Identity.Name);
            }
            else
            {
                ViewBag.ShoppingLists = null;
                ViewBag.UserRating = 0;
            }

            //calculate average 

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
        public ActionResult Create([Bind(Include = "ID,Title,OriginalAuthorName,AuthorName,Minutes,Instructions")] Recipe recipe)
        {
            _db.Recipes.Add(recipe);
            _db.SaveChanges();
            return RedirectToAction("Ingredients/" + recipe.ID);
        }

        // GET: Recipes/Edit/5
        public ActionResult Edit(int? id, bool fork)
        {
            if (id == null)
            {
                return RedirectToAction("index");
            }

            var recipe = _db.Recipes.Include(r => r.Ingredients).First(r => r.ID == id);
            ViewBag.Forked = fork;

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
                        Title = originalRecipe.Title + ", ny version",
                        Ingredients = new List<Item>(originalRecipe.Ingredients),
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
        public ActionResult Edit([Bind(Include = "ID,Title,OriginalAuthorName,AuthorName,Minutes,Instructions")] Recipe recipe, bool done)
        {

            if (ModelState.IsValid)
            {
                _db.Entry(recipe).State = EntityState.Modified;
                _db.SaveChanges();
                if(_db.Recipes.FirstOrDefault(r => r.ID == recipe.ID).Title == null) {
                    return View(recipe);
                }
                return RedirectToAction((done ? "Details/" : "Ingredients/") + recipe.ID);
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
            var recipe = _db.Recipes.Include(r => r.Ratings).First(x => x.ID == id);

            if (recipe.Ratings.Count > 0)
            {
                _db.Ratings.RemoveRange(_db.Ratings.Where(r => r.Recipe.ID == id));
                recipe.Ratings.Clear();
            }

            // denne var også en løsning:
            //var recipe = _db.Recipes.Include(x => x.Ratings).Include(x => x.Ingredients).FirstOrDefault(x => x.ID.Equals(id));

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
                return RedirectToAction("Ingredients/" + id);
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
            return RedirectToAction("Ingredients/" + id);
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
            return RedirectToAction("Ingredients/" + id);
        }

        public ActionResult Ingredients(int? id, int? numPersons)
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


        [HttpPost]
        public ActionResult AddRating(int id, int rating)
        {
            Recipe recipe = _db.Recipes.Include(r => r.Ratings).FirstOrDefault(x => x.ID == id);
            User user = _db.Users.Include(u => u.Ratings).SingleOrDefault(u => u.Username == User.Identity.Name);
            Rating prevRating = recipe.Ratings.FirstOrDefault(x => x.User.ID == user.ID);

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

            return Json(new
            {
                rating = rating,
                avgRating = recipe.Ratings.Count > 0 ? recipe.Ratings.Select(r => r.Score).Average().ToString("0.0") : "0.0",
                numRatings = recipe.Ratings.Count()
            });
        }

        private IEnumerable<Recipe> RecommendRecipes(User user)
        {
            // Get all recipies rated by the user
            var recipiesRatedByUser = _db.Recipes.Include(x => x.Ratings).Include(x => x.Ingredients).Where(x => x.Ratings.Select(y => y.User.ID).Contains(user.ID));

            // Pair the ingrdients with the rating given
            // If two ingredients are the same, take the avg of both ratings.
            var itemsWithRatings = new List<Tuple<Item, List<decimal>>>();

            foreach (var recipie in recipiesRatedByUser.ToList())
            {
                var score = recipie.Ratings.FirstOrDefault(x => x.User == user).Score;
                foreach (var ingredient in recipie.Ingredients)
                {
                    // If the element is already rated, add the new rating to the list of ratings
                    if (itemsWithRatings.Select(x => x.Item1).Contains(ingredient))
                    {
                        itemsWithRatings.FirstOrDefault(x => x.Item1 == ingredient).Item2.Add(score);
                    }
                    else
                    {
                        var tmp = new Tuple<Item, List<Decimal>>(ingredient, new List<decimal>());
                        tmp.Item2.Add(score);
                        itemsWithRatings.Add(tmp);
                    }
                }
            }

            var avgRating = 2;// recipiesRatedByUser.Average(x => x.Ratings.FirstOrDefault(y => y.User.ID == user.ID).Score);

            var recipiesRated = new List<Tuple<Recipe, decimal>>();

            // Calculate the score for every recipie
            foreach (var recipie in _db.Recipes.Include(x => x.Ingredients).Include(x => x.Ratings))
            {
                if (recipie.Ratings.FirstOrDefault(y => y.User.ID == user.ID) != null)
                {
                    recipiesRated.Add(new Tuple<Recipe, decimal>(recipie, recipie.Ratings.FirstOrDefault(y => y.User.ID == user.ID).Score));
                }
                else
                {
                    var score = new List<decimal>();
                    foreach (var ingredient in recipie.Ingredients)
                    {
                        var derp = (itemsWithRatings.FirstOrDefault(x => x.Item1.Equals(ingredient)));
                        score.Add(derp != null ? derp.Item2.Average() : avgRating);
                    }
                    recipiesRated.Add(new Tuple<Recipe, decimal>(item1: recipie, item2: score.Any() ? score.Average() : 0M));
                }

            }

            // Sort the recipies descending
            // return itz
            return recipiesRated.OrderByDescending(x => x.Item2).Select(x => x.Item1);
        }
    }
}
