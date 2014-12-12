using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using ProjectFood.Controllers;
using ProjectFood.Models;
using ProjectFood.Models.Api;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ProjectFood.Tests.Tests
{
    [TestFixture]
    public class TestOfferController
    {
        #region setup

        [SetUp]
        public void initialize()
        {
            _mockdata = new TestProjectFoodContext();
            _controller = new OfferController(_mockdata);
            _user = DemoGetMethods.GetDemoUser(1);
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

            var pref = new Pref(){ID = 0, Store = false, Value = "Ost"};
            _mockdata.Preferences.Add(pref);

            _user.Preferences.Add(pref);

            var list =  _mockdata.ShoppingLists.Add(DemoGetMethods.GetDemoShoppingListWithItem(3, 1));
            _mockdata.Users.First().ShoppingLists.Add(list);

            var baconItem = DemoGetMethods.GetDemoItem(5, "Bacon");

            _mockdata.Items.Add(DemoGetMethods.GetDemoItem(4, "Ost"));
            _mockdata.Items.Add(baconItem);
            _mockdata.Items.Add(DemoGetMethods.GetDemoItem(6, "Leverpostej"));

            _user.WatchList = new ShoppingList();
            _user.WatchList.Items.Add(baconItem);
            _user.LastSentNotification = DateTime.Now.AddDays(-7);
            _user.MaxSendEmailsEveryDays = 2;

            GlobalVariables.CurrentSystemTime = DateTime.Now;
        }

        #endregion

        private User _user = new User();
        private Moq.Mock<IPrincipal> _principal = new Moq.Mock<IPrincipal>();
        private OfferController _controller = new OfferController();
        private TestProjectFoodContext _mockdata = new TestProjectFoodContext();



        [TestCase("Ost")]
        [TestCase("Leverpostej")]
        public void IndexView_UserLoggedInWithOSTPref_ShouldReturnViewResultOffersNotThere(string offerName)
        {
            //Act
            var result = _controller.Index(_user.ShoppingLists.First().ID) as ViewResult;
            var viewmodel = result.ViewData.Model as IEnumerable<Offer>;
            var shoppinglists = getvalue("ShoppingLists", result) as List<ShoppingList>;
            var stores = getvalue("Stores", result) as IEnumerable<string>;

            //Assert
            Assert.AreEqual(stores.First(), "Netto");
            Assert.IsTrue(shoppinglists.First().ID == 1);
            Assert.IsTrue(result.ViewBag.SelectedShoppingListID == 1);
            Assert.IsFalse(viewmodel.Any(x=>x.Heading == offerName));

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [Test]
        public void NotifyWatcher()
        {
             _controller.NotifyWatchers();
            Assert.IsTrue(_user.SentOffers.Any());
        }

        [TestCase("Bacon")]
        public void IndexView_UserLoggedInWithOSTPref_ShouldReturnViewResultOfferThere(string offerName)
        {
            //Act
            var result = _controller.Index(_user.ShoppingLists.First().ID) as ViewResult;
            var viewmodel = result.ViewData.Model as IEnumerable<Offer>;
            var shoppinglists = getvalue("ShoppingLists", result) as List<ShoppingList>;
            var stores = getvalue("Stores", result) as IEnumerable<string>;

            //Assert
            Assert.AreEqual(stores.First(), "Netto");
            Assert.IsTrue(shoppinglists.First().ID == 1);
            Assert.IsTrue(result.ViewBag.SelectedShoppingListID == 1);
            Assert.IsTrue(viewmodel.Any(x => x.Heading == offerName));
            Assert.IsTrue(_mockdata.Offers.Any(x=>x.Heading == "Ost"));
            Assert.IsFalse(viewmodel.Any(x => x.Heading == "Ost"));

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }


        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(3, 1)]
        public void AddOfferToShoppingList_Offers_ShouldAddOffer(int offerId, int shoppingListId)
        {
            //PreCondition
            Assert.AreEqual(_user.ShoppingLists.First(x=>x.ID == 1).Items.Count(), 3);

            //Act
            _controller.AddOfferToShoppingList(offerId, shoppingListId);

            //Assert
            Assert.AreEqual(_user.ShoppingLists.First(x=>x.ID == 1).Items.Count(), 4);
        }

        [TestCase("Bacon")]
        public void GetOfferForItem_DifferentStrings_ShouldFindOffers(string id)
        {
            //Act
            var result = _controller.GetOffersForItem(id);
            
            //Assert
            Assert.AreEqual(result.Count(), 1);
        }


        [TestCase("Bacon")]
        public void GetofferForItem_AnItem_ShouldFind(string name)
        {
            //Act
            var result = _controller.GetOffersForItem(_mockdata.Items.First(x=> x.Name == name));

            //Assert
            Assert.IsTrue(result.First().Heading.Contains(name));

        }

        private object getvalue(string key, ViewResult view)
        {
            object value;
            view.ViewData.TryGetValue(key, out value);

            return value;
        }

    }
}
