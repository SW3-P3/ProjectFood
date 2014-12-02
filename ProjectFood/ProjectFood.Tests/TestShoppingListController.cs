using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
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
            //Compute
            var result = controller.AddItem(1, "DemoName", null, "", null, 1);
            var resultitem = mockData.ShoppingLists.First().Items.First();
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, mockData.ShoppingLists.First().Items.Count);
            Assert.AreEqual(resultitem.Name, "DemoName");
            Assert.AreEqual(resultitem.ID, 0);
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
    }
}
