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

        public static ShoppingList GetDemoShoppingListThreeItems()
        {
            var demoList = new ShoppingList() {ID = 1, Title = "DemoShopList"};
            demoList.Items.Add(GetDemoItem1());
            demoList.Items.Add(GetDemoItem2());
            demoList.Items.Add(GetDemoItem3());
            return demoList;
        }
        public static ShoppingList GetDemoShoppingListWithItem()
        {
            var demoList = new ShoppingList() {ID = 1, Title = "DemoShopList"};
            demoList.Items.Add(GetDemoItem1());
            return demoList;
        }

        public static ShoppingList GetDemoShoppingListEmpty()
        {
            return new ShoppingList() {ID = 1, Title = "DemoShopList"};
        }

        public static Item GetDemoItem1()
        {
            return new Item() {ID = 1, Name = "DemoItem"};
        }
        public static Item GetDemoItem2()
        {
            return new Item() { ID = 2, Name = "DemoItem" };
        }
        public static Item GetDemoItem3()
        {
            return new Item() { ID = 3, Name = "DemoItem" };
        }

        public static Offer GetDemoOffer()
        {
            return new Offer() {Heading = "DemoOffer", ID = 1, Price = 20, Store = "Netto"};
        }

        public static User GetDemoUser()
        {
            return new User() {ID = 1, Name = "DemoUser"};
        }

        public static Rating GetDemoRating()
        {
            return new Rating() {ID = 1, Score = 3};
        }
    }
}
