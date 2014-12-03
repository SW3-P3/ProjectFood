using System;
using System.Data.Entity;
using ProjectFood.Models;

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
    }
}