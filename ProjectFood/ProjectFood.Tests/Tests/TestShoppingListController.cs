using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Helpers;
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

            GlobalVariables.CurrentSystemTime = DateTime.Now;

             _mockdata.Offers.Add(DemoGetMethods.GetDemoOffer("Leverpostej", 1, 10.00M));
             _mockdata.Offers.Add(DemoGetMethods.GetDemoOffer("Bacon", 2, 11.00M));
             var offer = _mockdata.Offers.Add(DemoGetMethods.GetDemoOffer("Ost", 3, 13.00M));
             _mockdata.Offers.Add(DemoGetMethods.GetDemoOffer("Cola", 4, 10.00M, "Føtex"));
             _mockdata.Offers.Add(DemoGetMethods.GetDemoOffer("Revet ost", 5, 10.00M, "Føtex"));
             _mockdata.Offers.Add(DemoGetMethods.GetDemoOffer("God Kage", 6, 10.00M, "Føtex"));

            var item1 = _mockdata.Items.Add(DemoGetMethods.GetDemoItem(4, "Ost"));
            var item2 = _mockdata.Items.Add(DemoGetMethods.GetDemoItem(5, "Bacon"));
            var item3 = _mockdata.Items.Add(DemoGetMethods.GetDemoItem(6, "Leverpostej"));
            var item4 = _mockdata.Items.Add(DemoGetMethods.GetDemoItem(7, "Kage"));


            _shoppinglist.Items.Add(item1);
            _shoppinglist.Items.Add(item2);
            _shoppinglist.Items.Add(item3);

           _mockdata.ShoppingLists.Add(_shoppinglist);

           _mockdata.ShoppingList_Item.Add(new ShoppingList_Item()
           {
               Item = _mockdata.Items.First(x => x.ID == 4),
               ShoppingList = _shoppinglist,
               ItemID = 4,
               ShoppingListID = 1,
               Amount = 2.0,
               Bought = false,
               selectedOffer = offer,
               Unit = "tsk"
           });
           _mockdata.ShoppingList_Item.Add(new ShoppingList_Item()
           {
               Item = _mockdata.Items.First(x => x.ID == 5),
               ShoppingList = _shoppinglist,
               ItemID = 5,
               ShoppingListID = 1,
               Amount = 2.0,
               Bought = false,
               selectedOffer = null,
               Unit = "tsk"
           });
           _mockdata.ShoppingList_Item.Add(new ShoppingList_Item()
           {
               Item = _mockdata.Items.First(x => x.ID == 6),
               ShoppingList = _shoppinglist,
               ItemID = 6,
               ShoppingListID = 1,
               Amount = 2.0,
               Bought = false,
               selectedOffer = null,
               Unit = "tsk"
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
            var user2 = DemoGetMethods.GetDemoUser(2);
            user2.Username = "DemoMail2";
            _mockdata.Users.Add(user2); 

            //PreCondition
            Assert.IsFalse(_mockdata.ShoppingLists.First(x=>x.ID == 1).Users.Count == 2);
            Assert.IsTrue(_mockdata.Users.First(i => i.Username == "DemoUser").ShoppingLists.Any(x=>x.ID == 1));
            Assert.IsFalse(_mockdata.Users.First(i => i.Username == "DemoMail2").ShoppingLists.Any(x=>x.ID == 1));

            //Act
            var result = _controller.ShareList(1, "DemoMail2");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.Users.First(i => i.Username == "DemoUser").ShoppingLists.Any(x => x.ID == 1));
            Assert.IsTrue(_mockdata.Users.First(i => i.Username == "DemoMail2").ShoppingLists.Any(x => x.ID == 1));
            Assert.IsTrue(_mockdata.ShoppingLists.First(x => x.ID == 1).Users.Count == 2);
        }

        [Test]
        public void ShoppingListShareList_Unknownuser_ShouldNotShare()
        {
            //PreCondition
            Assert.IsTrue(_mockdata.Users.First(i => i.Username == "DemoUser").ShoppingLists.Any(x => x.ID == 1));
            Assert.IsFalse(_mockdata.ShoppingLists.First(x => x.ID == 1).Users.Count == 2);

            //Act
            _controller.ShareList(1, "DemoMail2");

            //Assert

            Assert.IsTrue(_mockdata.Users.First(i => i.Username == "DemoUser").ShoppingLists.Any(x => x.ID == 1));
            Assert.IsFalse(_mockdata.ShoppingLists.First(x => x.ID == 1).Users.Count == 2);
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
            //PreCondition
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
            //PreCondition
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

        [Test]
        public void ShoppingListEdit_Id1_ShouldEdit()
        {
            //Act
            var result = _controller.Edit(1);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }
        [Test]
        public void ShoppingListEdit_InvalidID_ShouldNotEditNoList()
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
        public void ShoppingListEdit_NullAsInput_ShouldReturnRedirectToRoute()
        {
            //Act
            var result = _controller.Edit(null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [Test]
        public void ShoppingListAREdit_ShoppingListAndIndexAsInput_ShouldEdit()
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
        public void ShoppingListAREdit_ModelStateNotValid_ShouldNotEdit()
        {
            //Act
            _controller.ModelState.Add("testError", new ModelState());
            _controller.ModelState.AddModelError("testError", "test");
            var result = _controller.Edit(_shoppinglist, "Index");  
          
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [Test]
        public void ShoppingListDeleteConfirmed_ShoppingListID1_ShouldDelete()
        {
            //PreCondition
            Assert.IsTrue(_mockdata.ShoppingLists.Any(x => x.ID == 1));

            //Act
            var result = _controller.DeleteConfirmed(1);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.IsFalse(_mockdata.ShoppingLists.Any(x => x.ID == 1));
        }

        [Test]
        public void DeleteConfirmed_2Users_ShouldRemoveUserFromListListStillThere()
        {
            //PreCondition
            Assert.IsTrue(_mockdata.ShoppingLists.Any(x => x.ID == 1));
            Assert.IsTrue(_mockdata.ShoppingLists.First(x => x.ID == 1).Users.Any(x => x.ID == 1));

            //Arrange
            var tmpUser =  _mockdata.Users.Add(new User(){ID = 2});
            tmpUser.ShoppingLists.Add(_shoppinglist);
            _mockdata.ShoppingLists.First(x=>x.ID==1).Users.Add(tmpUser);

            //Act
            var result = _controller.DeleteConfirmed(1);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.IsTrue(_mockdata.ShoppingLists.Any(x => x.ID == 1));
            Assert.IsFalse(_mockdata.ShoppingLists.First(x=>x.ID == 1).Users.Any(x=>x.ID == 1));
        }
        [Test]
        public void ShoppingListRemoveItem_ItemIDAndShoppingListId_ShouldBeGone()
        {
            //Precondition
            Assert.IsTrue(_mockdata.Items.Any(x => x.ID == 4));
            Assert.IsTrue(_mockdata.ShoppingLists.First(x=>x.ID == 1).Items.Any(x=>x.ID ==4));
           
            //Act
            var result = _controller.RemoveItem(1,4);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any(x => x.ID == 4));
        }
        [Test]
        public void ShoppingListClearShoppingList_ListID1_ShouldBeCleared()
        {
            //Act
            Assert.IsTrue(_mockdata.ShoppingLists.First(i => i.ID == 1).Items.Any());
            var result = _controller.ClearShoppingList(1);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_mockdata.ShoppingLists.First(i=> i.ID == 1).Items.Any());
        }

        [Test]
        public void ShoppingListAddItemAjax_ItemInfoShoppingListId1_ShouldAdd()
        {
            //PreCondition
            Assert.IsFalse(_mockdata.ShoppingLists.First(x=>x.ID==1).Items.Any(x=>x.Name=="Oster"));

            //Act
            var result =  _controller.AddItemAjax(1, "Oster", null, "", null, null);

            //Assert
            Assert.IsTrue(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any(x => x.Name == "Oster"));
        }

        [Test]
        public void ShoppingListAddItem_ItemInfoShoppingListId1_ShouldAdd()
        {

            //Act
            var result = _controller.AddItem(1, "DemoItem", null, "", null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any(u=>u.Name=="DemoItem"));

        }

        [Test]
        public void ShoppingListAddItem_ItemInfoNoNameShoppingListId1_ShouldNotAdd()
        {
            //PreCondition
            Assert.IsFalse(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any(u => u.Name == ""));

            //Act
            var result = _controller.AddItem(1, "", null, "", null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any(u => u.Name == ""));

        }

        [Test]
        public void ShoppingListAddItem_ItemInfoAlreadyThereShoppingListId1_ShouldAdd()
        {
            //PreCondition
            Assert.IsTrue(_mockdata.Items.Any(u => u.Name == "Kage"));
            Assert.IsFalse(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any(u => u.Name == "Kage"));

            //Act
            var result = _controller.AddItem(1, "Kage", null, "", null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any(u => u.Name == "Kage"));

        }

        [Test]
        public void ShoppingListAddItem_ItemInfoAlreadyThereItemIDNotNullShoppingListId1_ShouldAdd()
        {
            //PreCondition
            Assert.IsTrue(_mockdata.Items.Any(u => u.Name == "Kage"));
            Assert.IsFalse(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any(u => u.Name == "Kage"));

            //Act
            var result = _controller.AddItem(1, "Kage", null, "", null, 7);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any(u => u.Name == "Kage"));

        }
        [Test]
        public void ShoppingListAddItem_ItemAlreadyOnListItemIDNoSelectedOfferShoppingListId1_ShouldNotAdd()
        {
            //PreCondition
            Assert.IsTrue(_mockdata.Items.Any(u => u.Name == "Bacon"));
            Assert.IsTrue(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any(u => u.Name == "Bacon"));

            //Act
            var result = _controller.AddItem(1, "Bacon", null, "", null, 5);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any(u => u.Name == "Bacon"));
            Assert.IsFalse(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Count(x => x.Name == "Ost") == 2);

        }

        [Test]
        public void ShoppingListAddItem_ItemAlreadyOnListItemIDSelectedOfferShoppingListId1_ShouldAdd()
        {
            //PreCondition
            Assert.IsTrue(_mockdata.Items.Any(u => u.Name == "Ost"));
            Assert.IsTrue(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any(u => u.Name == "Ost"));


            //Act
            var result = _controller.AddItem(1, "Ost", null, "", null, 4);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Count(x=> x.Name == "Ost") == 2);

        }
        [Test]
        public void ShoppingListAddItem_ItemAlreadyOnListItemIDSelectedOfferOfferIdNotNullShoppingListId1_ShouldAdd()
        {
            //PreCondition
            Assert.IsTrue(_mockdata.Items.Any(u => u.Name == "Ost"));
            Assert.IsTrue(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any(u => u.Name == "Ost"));


            //Act
            var result = _controller.AddItem(1, "Ost", 2.0, "", 5, 4);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Count(x => x.Name == "Ost") == 2);

        }

        [Test]
        public void ShoppingListAddItem_ItemAlreadyOnListItemIDNoOfferIdNotNullNotIsSelectedOfferShoppingListId1_ShouldAdd()
        {
            //PreCondition
            Assert.IsTrue(_mockdata.Items.Any(u => u.Name == "Bacon"));
            Assert.IsTrue(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any(u => u.Name == "Bacon"));
            Assert.IsTrue(_mockdata.ShoppingList_Item.First(x=>x.ItemID==5).selectedOffer == null);

            //Act
            var result = _controller.AddItem(1, "Bacon", 2.0, "", 2, 5);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Count(x => x.Name == "Bacon") == 1);
            Assert.IsTrue(_mockdata.ShoppingList_Item.First(x => x.ItemID == 5).selectedOffer == _mockdata.Offers.First(x=>x.ID==2));

        }


        [Test]
        public void ShoppingListGetOffersForItem_ItemOst_ShouldGetOffers()
        {
            //Act
            var result = ShoppingListsController.GetOffersForItem(_mockdata, _mockdata.Items.First(x=>x.Name=="Ost"));

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(result.First(i => i.ID == 3).Price == 13);
            Assert.IsTrue(result.First(i => i.ID == 5).Price == 10);
        }

        [Test]
        public void ShoppingListEditAmount_OstAmountTo15_ShouldEditAmountForOstTo15()
        {
            //Act
            var result = _controller.EditAmount(1, 4, "15", "tsk");
            var resultitem = _mockdata.ShoppingList_Item.FirstOrDefault(i => i.ItemID == 4);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultitem);
            Assert.IsTrue(resultitem.Amount == 15);
        }
        [Test]
        public void ShoppingListChooseOffer_RevetOstItemAndOstOffer_ShouldChooseOfferRevetOst()
        {
            //PreCondition
            Assert.IsFalse(_mockdata.ShoppingList_Item.First().selectedOffer.Heading == "Revet ost");

            //Act
            var result = _controller.ChooseOffer(1, 4, 5);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.ShoppingList_Item.First().selectedOffer.Heading == "Revet ost");
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