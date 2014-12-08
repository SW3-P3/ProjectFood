using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using ProjectFood.Controllers;
using ProjectFood.Models;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ProjectFood.Tests.Tests
{
    [TestFixture]
    public class TestShoppingListsController
    {
        [SetUp]
        public void Initialize()
        {
            _mockdata = new TestProjectFoodContext();
            _controller = new ShoppingListsController(_mockdata);
            _user = DemoGetMethods.GetDemoUser(1);
            _shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            _user.ShoppingLists.Add(_shoppinglist);
            _mockdata.Users.Add(_user);
            _mockdata.ShoppingLists.Add(_shoppinglist);
            var controllerContext = new Mock<ControllerContext>();
            _principal = new Moq.Mock<IPrincipal>();
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            _principal.SetupGet(x => x.Identity.Name).Returns(_user.Name);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(_principal.Object);
            _controller.ControllerContext = controllerContext.Object;

        }

        private User _user = new User();
        private ShoppingList _shoppinglist = new ShoppingList();
        private Moq.Mock<IPrincipal> _principal = new Moq.Mock<IPrincipal>();
        private ShoppingListsController _controller = new ShoppingListsController();
        private TestProjectFoodContext _mockdata = new TestProjectFoodContext();

        [Test]
        public void Index_ShouldReturnView()
        {
            //Compute
            var result = _controller.Index();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [Test]
        public void Details_ShouldReturnDetailsView()
        {
            //Compute
            var result = _controller.Details(1);
            //Assert            
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        [Test]
        public void Details_ShouldRedirectToView()
        {
            //Compute
            _mockdata.ShoppingLists.Remove(_shoppinglist);
            var result = _controller.Details(1);            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [Test]
        public void WatchList_ShouldCreateList()
        {
            //Setup
            var offer = DemoGetMethods.GetDemoOffer("Demo", 1, 20);
            var item = DemoGetMethods.GetDemoItem(1, "Demo");
            _mockdata.Items.Add(item);
            _mockdata.Offers.Add(offer);          
            //Compute
            var resultCreateList = _controller.WatchList(null);
            _mockdata.Users.First().WatchList.Items.Add(_mockdata.Items.First());
            var resultAddItemGetOffers = _controller.WatchList(null);
            //Assert
            Assert.IsNotNull(resultAddItemGetOffers);
            Assert.IsNotNull(resultCreateList);
            Assert.IsNotNull(_user.WatchList);
            Assert.IsTrue(_mockdata.Users.First().WatchList.Items.First().Offers.Count == 1);
            Assert.IsInstanceOfType(resultCreateList, typeof(ViewResult));
            Assert.IsInstanceOfType(resultAddItemGetOffers, typeof(ViewResult));
        }

        [Test]
        public void ShareList_ShouldShare()
        {
            //Setup
            _user.Username = "DemoMail";
            var user2 = DemoGetMethods.GetDemoUser(2);
            _mockdata.Users.Add(user2); 
            user2.Username = "DemoMail2";
            //Compute
            Assert.IsTrue(_mockdata.Users.First(i => i.Username == "DemoMail").ShoppingLists.First().ID == 1);
            Assert.IsTrue(_mockdata.Users.First(i => i.Username == "DemoMail2").ShoppingLists.FirstOrDefault() == null);
            var result = _controller.ShareList(1, "DemoMail2");
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.Users.First(i => i.Username == "DemoMail2").ShoppingLists.First().ID == 1);
            Assert.IsTrue(_mockdata.Users.First(i => i.Username == "DemoMail2").ShoppingLists.First().ID == 
                _mockdata.Users.First(i => i.Username == "DemoMail").ShoppingLists.First().ID);
        }
        [Test]
        public void Create_ShouldCreate()
        {
            //Compute
            var result = _controller.Create();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        }
        [Test]
        public void ARCreate_ShouldCreate()
        {
            //Compute
            var result = _controller.Create(_shoppinglist, "Index");
            var resultlist = _mockdata.ShoppingLists.FirstOrDefault();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.IsNotNull(resultlist);
        }

        [Test]
        public void ARCreate_ShouldNotCreate()
        {
            //Setup
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);

            //Compute
            _user.ShoppingLists.Clear();
            var result = _controller.Create(_shoppinglist, "Index");
            var resultlist = _mockdata.ShoppingLists.FirstOrDefault();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.IsNotNull(resultlist);
        }

        [Test]
        public void Edit_ShouldEdit()
        {
            //Compute
            var result = _controller.Edit(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        [Test]
        public void Edit_ShouldNotEditNoList()
        {
            //Setup
            _mockdata.ShoppingLists.Remove(_shoppinglist);
            //Compute
            var result = _controller.Edit(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }
        [Test]
        public void Edit_ShouldNotEditNoAuthentication()
        {
            //Setup
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);
            //Compute
            var result = _controller.Edit(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [Test]
        public void AREdit_ShouldEdit()
        {
            //Setup
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);
            //Compute
            var result = _controller.Edit(_shoppinglist);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }
        [Test]
        public void AREdit_ShouldNotEdit()
        {
            //Setup
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);
            //Compute
            _controller.ModelState.Add("testError", new ModelState());
            _controller.ModelState.AddModelError("testError", "test");
            var result = _controller.Edit(_shoppinglist);            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        /*
        [Test]
        public void Delete_ShouldReturnView()
        {
            //Compute
            var result = _controller.Delete(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        [Test]
        public void Delete_ShouldRedirect()
        {
            //Compute
            var result = _controller.Delete(null);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }
        [Test]
        public void Delete_Should404()
        {
            //Compute
            var result = _controller.Delete(3);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }
        */
        [Test]
        public void DeleteConfirmed_ShouldDelete()
        {
            //Compute
            Assert.IsTrue(_mockdata.ShoppingLists.Count() == 1);
            var result = _controller.DeleteConfirmed(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.IsTrue(!_mockdata.ShoppingLists.Any());
        }
        [Test]
        public void RemoveItem_ShouldBeGone()
        {
            //Setup
            var tmp = _mockdata.Items.Add(DemoGetMethods.GetDemoItem(1));
            _mockdata.ShoppingLists.First().Items.Add(tmp);
            //Compute
            Assert.AreEqual(1, _mockdata.ShoppingLists.First().Items.Count);
            var result = _controller.RemoveItem(1,1);
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, _mockdata.ShoppingLists.First().Items.Count);
        }
        [Test]
        public void ClearShoppingList_ShouldBeCleared()
        {
            //Setup
            _mockdata.ShoppingLists.Add(DemoGetMethods.GetDemoShoppingListWithItem(5, 2));
            _mockdata.ShoppingLists.First(x => x.ID == 2).Users.Add(_mockdata.Users.First(x => x.Name == "DemoUser"));
            _mockdata.Users.First(x => x.Name == "DemoUser").ShoppingLists.Add(_mockdata.ShoppingLists.First(x => x.ID == 2));
            //Compute
            Assert.AreEqual(5, _mockdata.ShoppingLists.First(i => i.ID == 2).Items.Count);
            var result = _controller.ClearShoppingList(2);
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, _mockdata.ShoppingLists.First(i=> i.ID == 2).Items.Count);
        }

        [Test]
        public void AddItem_ShouldBeAdded()
        {
            //Setup
            _mockdata.Items.Add(new Item(){Name = "DemoItem", ID = 1});
            //Compute
            var result = _controller.AddItem(1, "DemoItem", null, "", null, 1);
            var resultitem = _mockdata.ShoppingLists.First().Items.First();
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, _mockdata.ShoppingLists.First().Items.Count);
            Assert.AreEqual(resultitem.Name, "DemoItem");
            Assert.AreEqual(resultitem.ID, 1);
        }
        /*
        [Test]
        public void MoveItemToBought_ShouldBeMoved()
        {
            //Setup
            var _shoppinglist = DemoGetMethods.GetDemoShoppingListWithItem(2, 2);
            _mockdata.ShoppingList_Item.Add(DemoGetMethods.GetDemoShoppingListItemRelation(_shoppinglist, 2, 1));
            //Compute
            var result = _controller.MoveItemToBought(2, 1);
            var resultitem = (_mockdata.ShoppingList_Item.FirstOrDefault(i => i.ItemID == _shoppinglist.Items.First().ID && i.ShoppingListID == _shoppinglist.ID));
            //Assert
            Assert.IsNotNull(result);  
            Assert.IsNotNull(resultitem);
            Assert.IsTrue(resultitem.Bought);
        } 
         */

        [Test]
        public void GetOffersForItem_ShouldGetOffer()
        {
            //Setup
            var _shoppinglist = DemoGetMethods.GetDemoShoppingListWithItem(1,2);
            _shoppinglist.Items.Add(DemoGetMethods.GetDemoItem(2, "DemoOfferItem"));
            var offer = DemoGetMethods.GetDemoOffer("DemoOfferItem Bacon", 1, 20);
            var offerExact = DemoGetMethods.GetDemoOffer("DemoOfferItem", 2, 20);
            _mockdata.Offers.Add(offer);
            _mockdata.Offers.Add(offerExact);
            var item = _shoppinglist.Items.FirstOrDefault(i => i.Name == "DemoOfferItem");
            _mockdata.Items.Add(item);
            //Compute
            var result = ShoppingListsController.GetOffersForItem(_mockdata, item);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result.First(i => i.ID == 1).Price == 20);
            Assert.IsTrue(result.First(i => i.ID == 2).Price == 20);
        }

        [Test]
        public void EditAmount_ShouldEditAmount()
        {
            //Setup;
            var shoppinglist = DemoGetMethods.GetDemoShoppingListWithItem(2,2);
            _mockdata.ShoppingLists.Add(shoppinglist);
            _mockdata.ShoppingList_Item.Add(DemoGetMethods.GetDemoShoppingListItemRelation(shoppinglist, 2, 1));
            _mockdata.ShoppingList_Item.Add(DemoGetMethods.GetDemoShoppingListItemRelation(shoppinglist, 2, 2));
            _mockdata.ShoppingList_Item.FirstOrDefault(i => i.ItemID == 1).Amount = 12;
            _mockdata.ShoppingList_Item.FirstOrDefault(i => i.ItemID == 2).Amount = 10;
            //Compute
            var result = _controller.EditAmount(2, 1, "15", "");
            var resultitem = _mockdata.ShoppingList_Item.FirstOrDefault(i => i.ItemID == 1);
            var resultunchanged = _mockdata.ShoppingList_Item.FirstOrDefault(i => i.ItemID == 2);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultitem);
            Assert.IsNotNull(resultunchanged);
            Assert.IsTrue(resultitem.Amount == 15);
            Assert.IsTrue(resultunchanged.Amount == 10);
        }
        [Test]
        public void ChooseOffer_ShouldChooseOffer()
        {
            //Setup
            var item = DemoGetMethods.GetDemoItem(1, "DemoOfferItem");
            var offer = DemoGetMethods.GetDemoOffer("DemoOfferItem", 1, 20);
            item.Offers.Add(offer);
            _shoppinglist.Items.Add(item);
            _mockdata.ShoppingLists.Add(_shoppinglist);
            _mockdata.Offers.Add(offer);
            _mockdata.Items.Add(item);
            _mockdata.ShoppingList_Item.Add(DemoGetMethods.GetDemoShoppingListItemRelation(_shoppinglist, 1, 1));
            //Compute
            var result = _controller.ChooseOffer(1, 1, 1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.ShoppingList_Item.First().selectedOffer == offer);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }
    }
}