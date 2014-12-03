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
            return new User() {ID = id, Name = "DemoUser", Username = "DemoUser"};
        }

        public static Rating GetDemoRating(int id, int score)
        {
            return new Rating() {ID = id, Score = score};
        }

        public static Pref GetDemoPref(int id, bool store, string name )
        {
            return new Pref(){ID = id, Store = store, Value = name };
        }

        public static ShoppingList_Item GetDemoShoppingListItemRelation(ShoppingList shoppinglist, int shopID, int itemid)
        {
            return new ShoppingList_Item()
            {
                Item = shoppinglist.Items.FirstOrDefault(i => i.ID == itemid),
                ShoppingList = shoppinglist,
                ItemID = itemid,
                ShoppingListID = shopID
            };
        }

#region RecipeController

        public static Recipe GetDemoRecipeWithItem(int amount, int recipeID)
        {
            var demoRecipe = new Recipe() { ID = 1, Title = "DemoRecipe", AuthorName = "DemoUser" };

            for (int i = 0; i < amount; i++)
            {
                demoRecipe.Ingredients.Add(GetDemoItem(i));
            }

            return demoRecipe;
        }

        public static Recipe_Ingredient GetDemoingredient(int IngID, int RecID, Recipe recipe)
        {
            return new Recipe_Ingredient() {IngredientID = IngID, RecipeID = RecID, Ingredient = GetDemoItem(1, "jens"), Recipe = recipe};
        }
#endregion 
    }
}
