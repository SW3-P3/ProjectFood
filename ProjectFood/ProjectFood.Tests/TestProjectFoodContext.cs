using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ProjectFood.Models;
using ProjectFood.Tests.TestDBsets;

namespace ProjectFood.Tests
{
    public class TestProjectFoodContext : IDataBaseContext
    {
        public TestProjectFoodContext()
        {
            this.ShoppingLists = new TestShoppingListDbSet();
            this.Items = new TestItemDbSet();
            this.Offers = new TestOfferDbSet();
            this.ShoppingList_Item = new TestShoppingListItemDbSet();
            this.Recipes = new TestRecipesDbSet();
            this.Recipe_Ingredient = new TestRecipeIngredientDbSet();
            this.Users = new TestUserDbSet();
            this.Preferences = new TestPreferenceDbSet();
            this.Ratings = new TestRatingDbSet();
        }

        public int SaveChanges()
        {
            return 0;
        }

        public void MarkAsModified(ShoppingList item) { }

        public void MarkAsModified(Recipe item) { }
        public void Dispose() { }

        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<Item> Items { get; private set; }
        public DbSet<Offer> Offers { get; private set; }
        public DbSet<ShoppingList_Item> ShoppingList_Item { get; private set; }
        public DbSet<Recipe> Recipes { get; private set; }
        public DbSet<Recipe_Ingredient> Recipe_Ingredient { get; private set; }
        public DbSet<User> Users { get; private set; }
        public DbSet<Pref> Preferences { get; private set; }
        public DbSet<Rating> Ratings { get; private set; }

        public IEnumerable<Offer> OffersFilteredWithString(params string[] args)
        {
            var blacklist = new List<string> { ",", "eller" };

            var fromArgs = new List<string>();
            foreach (var str in args)
            {
                fromArgs.AddRange(str.Split(','));
            }
            blacklist.AddRange(fromArgs);
            // If an empty strings if any was given
            blacklist.RemoveAll(x => x.Trim().Equals(string.Empty));

            var res = new List<Offer>();

            foreach (var o in Offers)
            {
                bool flag = !(o.End < DateTime.Now);

                foreach (var item in blacklist)
                {
                    if (o.Heading.ToLower().Contains(item.ToLower()) || o.Store.ToLower().Contains(item.ToLower()))
                        flag = false;
                }

                if (flag && o.Unit.Trim() != "")
                    res.Add(o);
            }
            return res;
        }

        public IEnumerable<Offer> OffersFiltered()
        {
            return OffersFilteredWithString();
        }

        public IEnumerable<Offer> OffersFilteredByUserPrefs(User u)
        {
            var prefs = Users.Include(a => a.Preferences).First(x => x.Username == u.Username).Preferences;
            var storesBlackListed = prefs.Select(x => x.Value).ToArray();
            return OffersFilteredWithString(storesBlackListed);
        }
    }
}