﻿using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web.Mvc;
using Moq;
using System.Web.UI.WebControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectFood.Models;
using ProjectFood.Controllers;

namespace ProjectFood.Tests
{
    [TestClass]
    public class TestShoppingListsController
    {
        [TestMethod]
        public void Index_ShouldReturnView()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            mockdata.Users.Add(user);
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            //Compute
            var result = controller.Index();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Details_ShouldReturnDetailsView()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);
            user.ShoppingLists.Add(shoppinglist);
            mockdata.ShoppingLists.Add(shoppinglist);
            mockdata.Users.Add(user);
            //Compute
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            //Assert
            var result = controller.Details(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        [TestMethod]
        public void Details_ShouldRedirectToView()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);
            mockdata.Users.Add(user);
            //Compute
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            var result = controller.Details(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void Create_ShouldCreate()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockdata);
            //Compute
            var result = controller.Create();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        }
        [TestMethod]
        public void ARCreate_ShouldCreate()
        {
            var mockdata = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);
            mockdata.Users.Add(user);
            //Compute
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            var result = controller.Create(shoppinglist, "Index");
            var resultlist = mockdata.ShoppingLists.FirstOrDefault();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.IsNotNull(resultlist);
        }

        [TestMethod]
        public void ARCreate_ShouldNotCreate()
        {
            var mockdata = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);
            user.ShoppingLists.Clear();
            mockdata.Users.Add(user);
            //Compute
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            var result = controller.Create(shoppinglist, "Index");
            var resultlist = mockdata.ShoppingLists.FirstOrDefault();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.IsNull(resultlist);
        }
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
            mockData.ShoppingList_Item.Add(DemoGetMethods.GetDemoShoppingListItemRelation(shoppinglist, 1, 1));
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
            var item = shoppinglist.Items.FirstOrDefault(i => i.Name == "DemoOfferItem");
            mockData.Items.Add(item);
            //Compute
            var result = ShoppingListsController.GetOffersForItem(mockData, item);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result.First(i => i.ID == 1).Price == 20);
            Assert.IsTrue(result.First(i => i.ID == 2).Price == 20);
        }

        [TestMethod]
        public void EditAmount_ShouldEditAmount()
        {
            //Setup
            var mockData = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockData);
            var shoppinglist = DemoGetMethods.GetDemoShoppingListWithItem(2);
            mockData.ShoppingLists.Add(shoppinglist);
            mockData.ShoppingList_Item.Add(DemoGetMethods.GetDemoShoppingListItemRelation(shoppinglist, 1, 1));
            mockData.ShoppingList_Item.Add(DemoGetMethods.GetDemoShoppingListItemRelation(shoppinglist, 1, 2));
            mockData.ShoppingList_Item.FirstOrDefault(i => i.ItemID == 1).Amount = 12;
            mockData.ShoppingList_Item.FirstOrDefault(i => i.ItemID == 2).Amount = 10;
            //Compute
            var result = controller.EditAmount(1, 1, 15, "");
            var resultitem = mockData.ShoppingList_Item.FirstOrDefault(i => i.ItemID == 1);
            var resultunchanged = mockData.ShoppingList_Item.FirstOrDefault(i => i.ItemID == 2);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultitem);
            Assert.IsNotNull(resultunchanged);
            Assert.IsTrue(resultitem.Amount == 15);
            Assert.IsTrue(resultunchanged.Amount == 10);
        }
        [TestMethod]
        public void ChooseOffer_ShouldChooseOffer()
        {
            //Setup
            var mockData = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockData);
            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            var item = DemoGetMethods.GetDemoItem(1, "DemoOfferItem");
            var offer = DemoGetMethods.GetDemoOffer("DemoOfferItem", 1, 20);
            item.Offers.Add(offer);
            shoppinglist.Items.Add(item);
            mockData.ShoppingLists.Add(shoppinglist);
            mockData.Offers.Add(offer);
            mockData.Items.Add(item);
            mockData.ShoppingList_Item.Add(DemoGetMethods.GetDemoShoppingListItemRelation(shoppinglist, 1, 1));
            //Compute
            var result = controller.ChooseOffer(1, 1, 1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(mockData.ShoppingList_Item.First().selectedOffer == offer);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }
    }
}
