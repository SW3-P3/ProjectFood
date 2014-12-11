using System.Collections.Generic;
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
            _shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty(_user);
            _user.ShoppingLists.Add(_shoppinglist);
            _mockdata.Users.Add(_user);
            var controllerContext = new Mock<ControllerContext>();
            _principal = new Moq.Mock<IPrincipal>();
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            _principal.SetupGet(x => x.Identity.Name).Returns(_user.Name);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(_principal.Object);
            _controller.ControllerContext = controllerContext.Object;

            _mockdata.Offers.Add(DemoGetMethods.GetDemoOffer("Leverpostej", 1, 10.00M));
            _mockdata.Offers.Add(DemoGetMethods.GetDemoOffer("Bacon", 2, 11.00M));
            _mockdata.Offers.Add(DemoGetMethods.GetDemoOffer("Ost", 3, 13.00M));
            _mockdata.Offers.Add(DemoGetMethods.GetDemoOffer("Cola", 4, 10.00M, "Føtex"));

            var item1 = _mockdata.Items.Add(DemoGetMethods.GetDemoItem(4, "Ost"));
            var item2 = _mockdata.Items.Add(DemoGetMethods.GetDemoItem(5, "Bacon"));
            var item3 = _mockdata.Items.Add(DemoGetMethods.GetDemoItem(6, "Leverpostej"));

            _shoppinglist.Items.Add(item1);
            _shoppinglist.Items.Add(item2);
            _shoppinglist.Items.Add(item3);

           _mockdata.ShoppingLists.Add(_shoppinglist);

           _mockdata.ShoppingList_Item.Add(new ShoppingList_Item()
           {
               Item = _mockdata.Items.First(x => x.ID == 4),
               ShoppingList = _shoppinglist,
               ItemID = 4,
               ShoppingListID = 1
           });
        }

        private User _user = new User();
        private ShoppingList _shoppinglist = new ShoppingList();
        private Moq.Mock<IPrincipal> _principal = new Moq.Mock<IPrincipal>();
        private ShoppingListsController _controller = new ShoppingListsController();
        private TestProjectFoodContext _mockdata = new TestProjectFoodContext();

        [Test]
        public void ShoppingListIndex_NoInputNoSharedLists_ShouldReturnViewWithUserListsNoSharedLists()
        {
            //Act
            var result = _controller.Index() as ViewResult;
            var sharedList = getvalue("sharedLists", result) as IEnumerable<ShoppingList>;
            var viewModel = _controller.ViewData.Model as IEnumerable<ShoppingList>;
            

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(viewModel.Any(x=>x.ID == _shoppinglist.ID));
            Assert.IsFalse(sharedList.Any());
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [Test]
        public void ShoppingListDetails_InputID1_ShouldReturnDetailsView()
        {
            //Act
            var result = _controller.Details(1);
            //Assert            
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        [Test]
        public void ShoppingListDetails_InputIDNull_ShouldReturnRedirectToRoute()
        {
            //Act
            var result = _controller.Details(null);
            //Assert            
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [Test]
        public void ShoppingListDetails_InputInvalid_ShouldGiveHttpNotFound()
        {
            //Act
            _mockdata.ShoppingLists.Remove(_shoppinglist);
            var result = _controller.Details(1);        
    
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [Test]
        public void WatchList_ShouldCreateList()
        {
            //Act
            var resultCreateList = _controller.WatchList(null);
            _mockdata.Users.First(x=>x.ID == 1).WatchList.Items.Add(_mockdata.Items.First(x=> x.ID == 4));
            var resultAddItemGetOffers = _controller.WatchList(null);

            //Assert
            Assert.IsNotNull(resultAddItemGetOffers);
            Assert.IsNotNull(resultCreateList);
            Assert.IsNotNull(_user.WatchList);
            Assert.IsTrue(_mockdata.Users.First(x => x.ID == 1).WatchList.Items.First(x => x.ID == 4).Offers.Any(x=>x.Heading == "Ost"));
            Assert.IsInstanceOfType(resultCreateList, typeof(ViewResult));
            Assert.IsInstanceOfType(resultAddItemGetOffers, typeof(ViewResult));
        }

        [Test]
        public void ShoppingListShareList_DemoMail2UserAsInput_ShouldShare()
        {
            //Arrange
            _user.Username = "DemoMail";
            var user2 = DemoGetMethods.GetDemoUser(2);
            _mockdata.Users.Add(user2); 
            user2.Username = "DemoMail2";
            //Act
            Assert.IsTrue(_mockdata.Users.First(i => i.Username == "DemoMail").ShoppingLists.Any(x=>x.ID == 1));
            Assert.IsFalse(_mockdata.Users.First(i => i.Username == "DemoMail2").ShoppingLists.Any(x=>x.ID == 1));
            var result = _controller.ShareList(1, "DemoMail2");
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.Users.First(i => i.Username == "DemoMail").ShoppingLists.Any(x => x.ID == 1));
            Assert.IsTrue(_mockdata.Users.First(i => i.Username == "DemoMail2").ShoppingLists.Any(x => x.ID == 1));
        }
        [Test]
        public void ShoppingListCreate_NoInput_ShouldCreate()
        {
            //Act
            var result = _controller.Create();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        }
        [Test]
        public void ShoppingListARCreate_ShoppingListAndIndex_ShouldCreate()
        {

            Assert.IsFalse(_mockdata.ShoppingLists.Any(x => x.ID == 2));
            //Act
            var result = _controller.Create(DemoGetMethods.GetDemoShoppingListWithItem(1,2), "Index");
            var resultlist = _mockdata.ShoppingLists.FirstOrDefault();
            //Assert

            Assert.IsTrue(_mockdata.ShoppingLists.Any(x=>x.ID==2));
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.IsNotNull(resultlist);
        }

        [Test]
        public void ShoppingListARCreate_UserNotLoggedIn_ShouldNotCreate()
        {
            Assert.IsFalse(_mockdata.ShoppingLists.Any(x => x.ID == 2));
            //Arrange
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);

            //Act
            _user.ShoppingLists.Clear();
            var result = _controller.Create(DemoGetMethods.GetDemoShoppingListWithItem(1, 2), "Index");
            var resultlist = _mockdata.ShoppingLists.FirstOrDefault();
            //Assert
            Assert.IsFalse(_mockdata.ShoppingLists.Any(x => x.ID == 2));
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.IsNotNull(resultlist);
        }


        //DU ER NÅET HERTIL
        //DU ER NÅET HERTIL
        //DU ER NÅET HERTIL
        //DU ER NÅET HERTIL
        //DU ER NÅET HERTIL
        //DU ER NÅET HERTIL

        [Test]
        public void Edit_ShouldEdit()
        {
            //Act
            var result = _controller.Edit(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }
        [Test]
        public void Edit_ShouldNotEditNoList()
        {
            //Arrange
            _mockdata.ShoppingLists.Remove(_shoppinglist);
            //Act
            var result = _controller.Edit(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }
        [Test]
        public void Edit_Should()
        {
            //Arrange

            //Act
            var result = _controller.Edit(null);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [Test]
        public void AREdit_ShouldEdit()
        {
            //Arrange
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);
            //Act
            var result = _controller.Edit(_shoppinglist, "Index");
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
        }
        [Test]
        public void AREdit_ShouldNotEdit()
        {
            //Arrange
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);
            //Act
            _controller.ModelState.Add("testError", new ModelState());
            _controller.ModelState.AddModelError("testError", "test");
            var result = _controller.Edit(_shoppinglist, "Index");            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [Test]
        public void DeleteConfirmed_ShouldDelete()
        {
            //Act
            Assert.IsTrue(_mockdata.ShoppingLists.Any(x=>x.ID == 1));
            var result = _controller.DeleteConfirmed(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.IsTrue(!_mockdata.ShoppingLists.Any(x => x.ID == 1));
        }
        [Test]
        public void RemoveItem_ShouldBeGone()
        {

            //Arrange
            var tmp = _mockdata.Items.Add(DemoGetMethods.GetDemoItem(1));
            _mockdata.ShoppingLists.First(x=>x.ID == 1).Items.Add(tmp);
            Assert.IsTrue(_mockdata.Items.Any(x => x.ID == 1));
            Assert.IsTrue(_mockdata.ShoppingLists.First(x=>x.ID == 1).Items.Any(x=>x.ID ==1));
           
            //Act
            var result = _controller.RemoveItem(1,1);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any(x => x.ID == 1));
        }
        [Test]
        public void ClearShoppingList_ShouldBeCleared()
        {
            //Arrange
            _mockdata.ShoppingLists.Add(DemoGetMethods.GetDemoShoppingListWithItem(5, 2));
            _mockdata.ShoppingLists.First(x => x.ID == 2).Users.Add(_mockdata.Users.First(x => x.Name == "DemoUser"));
            _mockdata.Users.First(x => x.Name == "DemoUser").ShoppingLists.Add(_mockdata.ShoppingLists.First(x => x.ID == 2));
            //Act
            Assert.AreEqual(5, _mockdata.ShoppingLists.First(i => i.ID == 2).Items.Count);
            var result = _controller.ClearShoppingList(2);
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, _mockdata.ShoppingLists.First(i=> i.ID == 2).Items.Count);
        }

        [Test]
        public void AddItem_ShouldBeAdded()
        {

            //Arrange
            _mockdata.Items.Add(new Item(){Name = "DemoItem", ID = 1});
            //Act
            var result = _controller.AddItem(1, "DemoItem", null, "", null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.ShoppingLists.Any(x=>x.Items.Any(u=>u.Name=="DemoItem")));

        }

        [Test]
        public void GetOffersForItem_ShouldGetOffer()
        {
            //Arrange
            var _shoppinglist = DemoGetMethods.GetDemoShoppingListWithItem(1,2);
            _shoppinglist.Items.Add(DemoGetMethods.GetDemoItem(2, "DemoOfferItem"));
            var offer = DemoGetMethods.GetDemoOffer("DemoOfferItem Bacon", 1, 20);
            var offerExact = DemoGetMethods.GetDemoOffer("DemoOfferItem", 2, 20);
            _mockdata.Offers.Add(offer);
            _mockdata.Offers.Add(offerExact);
            var item = _shoppinglist.Items.FirstOrDefault(i => i.Name == "DemoOfferItem");
            _mockdata.Items.Add(item);
            //Act
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
            //Arrange;
            var shoppinglist = DemoGetMethods.GetDemoShoppingListWithItem(2,2);
            _mockdata.ShoppingLists.Add(shoppinglist);
            _mockdata.ShoppingList_Item.Add(DemoGetMethods.GetDemoShoppingListItemRelation(shoppinglist, 2, 1));
            _mockdata.ShoppingList_Item.Add(DemoGetMethods.GetDemoShoppingListItemRelation(shoppinglist, 2, 2));
            _mockdata.ShoppingList_Item.FirstOrDefault(i => i.ItemID == 1).Amount = 12;
            _mockdata.ShoppingList_Item.FirstOrDefault(i => i.ItemID == 2).Amount = 10;
            //Act
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
            //Act
            var result = _controller.ChooseOffer(1, 4, 3);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.ShoppingList_Item.First().selectedOffer.Heading == "Ost");
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }
        private object getvalue(string key, ViewResult view)
        {
            object value;
            view.ViewData.TryGetValue(key, out value);

            return value;
        }
    }
}