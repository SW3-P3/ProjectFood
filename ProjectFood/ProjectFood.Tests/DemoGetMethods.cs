using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using ProjectFood.Models;
using ProjectFood.Controllers;

namespace ProjectFood.Tests
{
    class DemoGetMethods
    {
        public static ShoppingList GetDemoShoppingListWithItem(int amount)
        {
            var demoList = new ShoppingList() {ID = 1, Title = "DemoShopList"};
            
            for (int i = 0, setid = 1; i < amount; i++, setid++)
            {                
                demoList.Items.Add(GetDemoItem(setid));
            }
            return demoList;
        }
        public static ShoppingList GetDemoShoppingListEmpty()
        {
            return new ShoppingList() {ID = 1, Title = "DemoShopList"};
        }

        public static Item GetDemoItem(int id)
        {
            return new Item() { ID = id, Name = "DemoItem" };
        }

        public static Item GetDemoItem(int id, string name)
        {
            return new Item(){ID = id, Name = name};
        }
        public static Offer GetDemoOffer(string heading, int id, decimal price)
        {
            return new Offer() {Heading = heading, ID = id, Price = price, Store = "Netto"};
        }

        public static User GetDemoUser(int id)
        {
            return new User() {ID = id, Name = "DemoUser"};
        }

        public static Rating GetDemoRating(int id, int score)
        {
            return new Rating() {ID = id, Score = score};
        }


#region RecipeController

        public static Recipe GetDemoRecipeWithItem(int amount, int recipeID)
        {
            var demoRecipe = new Recipe() { ID = 1, Title = "DemoRecipe" };

            for (int i = 0, IngID = 0; i < amount; i++, IngID++)
            {
                demoRecipe.Ingredients.Add(GetDemoItem(IngID));
            }

            return demoRecipe;
        }

        public static Recipe_Ingredient GetDemoingredient(int IngID, int RecID)
        {
            return new Recipe_Ingredient() {IngredientID = IngID, RecipeID = RecID, Ingredient = GetDemoItem(1)};
        }
#endregion 
    }
}
