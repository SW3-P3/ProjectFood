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
        /*
        [TestMethod]
        public void PutShoppingLists_ShouldReturnStatusCode()
        {
            var controller = new ShoppingListsController(new TestProjectFoodContext());

            var item = GetDemoShoppingLists();

            var result = controller.PutShoppingList(item.Id, item) as StatusCodeResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [TestMethod]
        public void PutShoppingLists_ShouldFail_WhenDifferentID()
        {
            var controller = new ShoppingListsController(new TestProjectFoodContext());

            var badresult = controller.PutShoppingList(999, GetDemoShoppingLists());
            Assert.IsInstanceOfType(badresult, typeof(BadRequestResult));
        }

        [TestMethod]
        public void GetShoppingLists_ShouldReturnShoppingListsWithSameID()
        {
            var context = new TestProjectFoodContext();
            context.ShoppingLists.Add(GetDemoShoppingLists());

            var controller = new ShoppingListsController(context);
            var result = controller.GetShoppingList(3) as OkNegotiatedContentResult<ShoppingList>;
            var resuls = controller.
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Content.ID);
        }

        [TestMethod]
        public void GetShoppingListss_ShouldReturnAllShoppingListss()
        {
            var context = new TestProjectFoodContext();
            context.ShoppingLists.Add(new ShoppingList { ID = 1, Title = "Demo1", Price = 20 });
            context.ShoppingLists.Add(new ShoppingList { ID = 2, Title = "Demo2", Price = 30 });
            context.ShoppingLists.Add(new ShoppingList { ID = 3, Title = "Demo3", Price = 40 });

            var controller = new ShoppingListsController(context);
            var result = controller.GetShoppingLists() as TestShoppingListDbSet;

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Local.Count);
        }

        [TestMethod]
        public void DeleteShoppingLists_ShouldReturnOK()
        {
            var context = new TestProjectFoodContext();
            var item = GetDemoShoppingLists();
            context.ShoppingLists.Add(item);

            var controller = new ShoppingListsController(context);
            var result = controller.DeleteShoppingList(3) as OkNegotiatedContentResult<ShoppingList>;

            Assert.IsNotNull(result);
            Assert.AreEqual(item.Id, result.Content.Id);
        }

       */ 
    }
}
