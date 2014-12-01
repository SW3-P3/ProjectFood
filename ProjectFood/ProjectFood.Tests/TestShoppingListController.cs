using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http.Results;
using System.Net;
using ProjectFood.Models;
using ProjectFood.Controllers;

namespace ProjectFood.Tests
{
    [TestClass]
    public class TestShoppingListsController
    {
        [TestMethod]
        public void PostShoppingLists_ShouldReturnSameShoppingLists()
        {
            var controller = new ShoppingListsController(new TestProjectFoodContext());

            var DemoSList = GetDemoShoppingLists();

            var result =
                controller.Create(DemoSList, "details") as CreatedAtRouteNegotiatedContentResult<ShoppingList>;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteName, "DefaultApi");
            Assert.AreEqual(result.RouteValues["id"], result.Content.ID);
            Assert.AreEqual(result.Content.Title, DemoSList.Title);
        }

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

        ShoppingList GetDemoShoppingLists()
        {
         /*   var DemoItem = new Item() {ID = 1, Name = "DemoItem"};
            var DemoList = new List<Item>();
            DemoList.Add(DemoItem);
            var DemoUser = new User() {ID = 1, Name = "DemoName"} */
            return new ShoppingList() { ID = 3, Title = "Demo name"};
        }
    }
}
