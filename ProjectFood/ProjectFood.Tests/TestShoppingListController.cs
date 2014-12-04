using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.UI;
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
            var result = controller.Details(1);
            //Assert            
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
        public void WatchList_ShouldCreateList()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            var offer = DemoGetMethods.GetDemoOffer("Demo", 1, 20);
            var item = DemoGetMethods.GetDemoItem(1, "Demo");
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();           
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);
            mockdata.Users.Add(user);
            mockdata.Items.Add(item);
            mockdata.Offers.Add(offer);          
            //Compute
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            Assert.IsNull(user.WatchList);
            Assert.IsTrue(mockdata.Items.First().Offers.Count == 0);
            var resultCreateList = controller.WatchList(null);
            mockdata.Users.First().WatchList.Items.Add(mockdata.Items.First());
            var resultAddItemGetOffers = controller.WatchList(null);
            //Assert
            Assert.IsNotNull(resultAddItemGetOffers);
            Assert.IsNotNull(resultCreateList);
            Assert.IsNotNull(user.WatchList);
            Assert.IsTrue(mockdata.Items.First().Offers.Count == 1);
            Assert.IsInstanceOfType(resultCreateList, typeof(ViewResult));
            Assert.IsInstanceOfType(resultAddItemGetOffers, typeof(ViewResult));
        }
        // Kan laves hvis der findes løsning på  return RedirectToAction("Login", "Account", new { returnUrl = Url.Action() });
        /* [TestMethod]
        public void WatchList_ShouldFailUserNotAuthenticated()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            var offer = DemoGetMethods.GetDemoOffer("Demo", 1, 20);
            var item = DemoGetMethods.GetDemoItem(1, "Demo");
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);
            mockdata.Users.Add(user);
            mockdata.Items.Add(item);
            mockdata.Offers.Add(offer);
            //Compute
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            Assert.IsNull(user.WatchList);
            Assert.IsTrue(mockdata.Items.First().Offers.Count == 0);
            var resultCreateList = controller.WatchList(null);
            //Assert
            Assert.IsNotNull(resultCreateList);
            Assert.IsNull(user.WatchList);
            Assert.IsTrue(mockdata.Items.First().Offers.Count == 0);
            Assert.IsInstanceOfType(resultCreateList, typeof(RedirectToRouteResult));
        */

        [TestMethod]
        public void ShareList_ShouldShare()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            user.Username = "DemoMail";
            var user2 = DemoGetMethods.GetDemoUser(2);
            user2.Username = "DemoMail2";
            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            user.ShoppingLists.Add(shoppinglist);
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);
            mockdata.Users.Add(user);
            mockdata.Users.Add(user2);
            mockdata.ShoppingLists.Add(shoppinglist);
            //Compute
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            Assert.IsTrue(mockdata.Users.First(i => i.Username == "DemoMail").ShoppingLists.First().ID == 1);
            Assert.IsTrue(mockdata.Users.First(i => i.Username == "DemoMail2").ShoppingLists.FirstOrDefault() == null);
            var result = controller.ShareList(1, "DemoMail2");
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(mockdata.Users.First(i => i.Username == "DemoMail2").ShoppingLists.First().ID == 1);
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
        public void Edit_ShouldEdit()
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
            var result = controller.Edit(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        [TestMethod]
        public void Edit_ShouldNotEditNoList()
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
            var result = controller.Edit(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }
        [TestMethod]
        public void Edit_ShouldNotEditNoAuthentication()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);
            user.ShoppingLists.Add(shoppinglist);
            mockdata.ShoppingLists.Add(shoppinglist);
            mockdata.Users.Add(user);
            //Compute
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            var result = controller.Edit(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void AREdit_ShouldEdit()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);
            user.ShoppingLists.Add(shoppinglist);
            mockdata.ShoppingLists.Add(shoppinglist);
            mockdata.Users.Add(user);
            //Compute
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            var result = controller.Edit(shoppinglist);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }
        [TestMethod]
        public void AREdit_ShouldNotEdit()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);
            user.ShoppingLists.Add(shoppinglist);
            mockdata.ShoppingLists.Add(shoppinglist);
            mockdata.Users.Add(user);
            //Compute
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            controller.ModelState.Add("testError", new ModelState());
            controller.ModelState.AddModelError("testError", "test");
            var result = controller.Edit(shoppinglist);            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Delete_ShouldReturnView()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockdata);
            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            mockdata.ShoppingLists.Add(shoppinglist);
            //Compute
            var result = controller.Delete(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        [TestMethod]
        public void Delete_ShouldRedirect()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockdata);
            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            mockdata.ShoppingLists.Add(shoppinglist);
            //Compute
            var result = controller.Delete(null);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }
        [TestMethod]
        public void Delete_Should404()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockdata);
            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            mockdata.ShoppingLists.Add(shoppinglist);
            //Compute
            var result = controller.Delete(3);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void DeleteConfirmed_ShouldDelete()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new ShoppingListsController(mockdata);
            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            mockdata.ShoppingLists.Add(shoppinglist);
            //Compute
            Assert.IsTrue(mockdata.ShoppingLists.Count() == 1);
            var result = controller.DeleteConfirmed(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.IsTrue(!mockdata.ShoppingLists.Any());
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
