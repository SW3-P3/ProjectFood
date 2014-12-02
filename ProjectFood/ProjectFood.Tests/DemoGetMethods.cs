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
            int setid = 1;
            for (int i = 0; i < amount; i++)
            {                
                demoList.Items.Add(GetDemoItem(setid));
                setid++;
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

        public static Offer GetDemoOffer(int id, decimal price)
        {
            return new Offer() {Heading = "DemoOffer", ID = id, Price = price, Store = "Netto"};
        }

        public static User GetDemoUser(int id)
        {
            return new User() {ID = id, Name = "DemoUser"};
        }

        public static Rating GetDemoRating(int id, int score)
        {
            return new Rating() {ID = id, Score = score};
        }
    }
}
