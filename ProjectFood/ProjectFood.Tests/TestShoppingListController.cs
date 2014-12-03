using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using ProjectFood.Models;
using ProjectFood.Controllers;

namespace ProjectFood.Tests
{
    [TestClass]
    public class TestShoppingListsController
    {
        [TestMethod]
        public void RemoveItem_ShouldBeGone()
        {
            //Setup
            var mockData = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockData);           
            var shoppinglist = DemoGetMethods.GetDemoShoppingListWithItem(1);
            mockData.ShoppingLists.Add(shoppinglist);
            //Compute
            var result = controller.RemoveItem(1,1);
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0 ,mockData.ShoppingLists.First().Items.Count);
        }
        [TestMethod]
        public void ClearShoppingList_ShouldBeCleared()
        {
            //Setup
            var mockData = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockData);
            var shoppinglist = DemoGetMethods.GetDemoShoppingListWithItem(3);
            mockData.ShoppingLists.Add(shoppinglist);
            //Compute
            var result = controller.ClearShoppingList(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, mockData.ShoppingLists.First().Items.Count);
        }

        [TestMethod]
        public void AddItem_ShouldBeAdded()
        {
            //Setup
            var mockData = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockData);
            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            mockData.ShoppingLists.Add(shoppinglist);
            mockData.Items.Add(new Item(){Name = "DemoItem", ID = 1});
            //Compute
            var result = controller.AddItem(1, "DemoItem", null, "", null, 1);
            var resultitem = mockData.ShoppingLists.First().Items.First();
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, mockData.ShoppingLists.First().Items.Count);
            Assert.AreEqual(resultitem.Name, "DemoItem");
            Assert.AreEqual(resultitem.ID, 1);
        }
        [TestMethod]
        public void MoveItemToBought_ShouldBeMoved()
        {
            //Setup
            var mockData = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockData);
            var shoppinglist = DemoGetMethods.GetDemoShoppingListWithItem(2);
            mockData.ShoppingLists.Add(shoppinglist);
            mockData.ShoppingList_Item.Add(new ShoppingList_Item(){ Item = shoppinglist.Items.First(), ShoppingList = shoppinglist, ItemID = 1, ShoppingListID = 1});
            //Compute
            var result = controller.MoveItemToBought(1, 1);
            var resultitem = (mockData.ShoppingList_Item.FirstOrDefault(i => i.ItemID == shoppinglist.Items.First().ID && i.ShoppingListID == shoppinglist.ID));
            //Assert
            Assert.IsNotNull(result);  
            Assert.IsNotNull(resultitem);
            Assert.IsTrue(resultitem.Bought);
        }

        [TestMethod]
        public void GetOffersForItem_ShouldGetOffer()
        {
            //Setup
            var mockData = new TestProjectFoodContext();
            var shoppinglist = DemoGetMethods.GetDemoShoppingListWithItem(1);
            shoppinglist.Items.Add(DemoGetMethods.GetDemoItem(2, "DemoOfferItem"));
            var offer = DemoGetMethods.GetDemoOffer("DemoOfferItem Bacon", 1, 20);
            var offerExact = DemoGetMethods.GetDemoOffer("DemoOfferItem", 2, 20);
            mockData.ShoppingLists.Add(shoppinglist);
            mockData.Offers.Add(offer);
            mockData.Offers.Add(offerExact);
            //Compute
            var item = shoppinglist.Items.FirstOrDefault(i => i.Name == "DemoOfferItem");
            mockData.Items.Add(item);
            var result = ShoppingListsController.GetOffersForItem(mockData, item);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result.First(i => i.ID == 1).Price == 20);
            Assert.IsTrue(result.First(i => i.ID == 2).Price == 20);
        }
    }
}
