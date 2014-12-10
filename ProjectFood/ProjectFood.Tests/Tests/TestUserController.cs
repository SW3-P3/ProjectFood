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
    public class TestUserController
    {
        #region Setup

        [SetUp]
        public void Initialize()
        {
            _mockdata = new TestProjectFoodContext();
            _controller = new UserController(_mockdata);
            _user = DemoGetMethods.GetDemoUser(1);
            _mockdata.Users.Add(_user);
            var controllerContext = new Mock<ControllerContext>();
            _principal = new Moq.Mock<IPrincipal>();
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            _principal.SetupGet(x => x.Identity.Name).Returns(_user.Name);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(_principal.Object);
            _controller.ControllerContext = controllerContext.Object;

            var pref1 = DemoGetMethods.GetDemoPref(1, true, "Kvickly");
            var pref2 = DemoGetMethods.GetDemoPref(2, false, "Fisk");
            var pref3 = DemoGetMethods.GetDemoPref(3, false, "Kylling");

            _mockdata.Preferences.Add(pref1);
            _mockdata.Preferences.Add(pref2);
            _mockdata.Preferences.Add(pref3);

            _user.Preferences.Add(pref1);
            _user.Preferences.Add(pref2);
            _user.Preferences.Add(pref3);

            var offer = new Offer() {Store = "Kvickly", Heading = "Bacon"};
            _mockdata.Offers.Add(offer);


        }

        private User _user = new User();
        private Moq.Mock<IPrincipal> _principal = new Moq.Mock<IPrincipal>(); 
        private UserController _controller = new UserController();
        private TestProjectFoodContext _mockdata = new TestProjectFoodContext();

        #endregion  

        [Test]
        public void GetIndexView_NoInputsNeeded_ShouldReturnViewResult()
        {
            //Act
            var result = _controller.Index();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            
        }

        [Test]
        public void GetIndexView_UserNotLoggedIn_ShouldReturnRedirectToRouteResult()
        {
            //Arrance
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);

            //Act
            var result = _controller.Index();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

        }

        [TestCase("Lars", Result = "Lars")]
        [TestCase("Peter", Result = "Peter")]
        [TestCase("Søren", Result = "Søren")]
        public string EditName_DifferentNames_ShouldChangeName(string name)
        {
            //PreCondition
            Assert.AreNotEqual(name,_user.Name);

            //Act
            _controller.EditName(_user.Username, name);

            return _user.Name;
        }

        [TestCase("Lars")]
        [TestCase("Peter")]
        [TestCase("Søren")]
        public void EditName_UserNotLoggedIn_ShouldReturnRedirectToRouteResult(string name)
        {
            //Arrange
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);

            //Act
            var result = _controller.EditName(_user.Username, name);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

        }

        [Test]
        public void EditPreferencesView_NoInputIsNeeded_ShouldReturnViewResult()
        {
            //Act
            var result = _controller.EditPreferences("abcd") as ViewResult;
            var stores = getvalue("Store", result) as List<string>;
            var prefs = getvalue("Prefs", result) as List<Pref>;

            //Assert
            Assert.AreEqual(stores.First(), "Kvickly");
            Assert.IsTrue(prefs.First(x=>x.ID == 1).Store);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof (ViewResult));

        }

        [Test]
        public void EditPreferencesView_UserNotLoggedIn_ShouldReturnRedirectToRouteResult()
        {
            //Arrange
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);

            //Act
            var result = _controller.EditPreferences("abcd");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

        }

        [TestCase("DemoUser", "Kål", false)]
        [TestCase("DemoUser", "Nødder", false)]
        [TestCase("DemoUser", "Lam", false)]
        public void AddPreference_DiferentPreferences_ShouldHaveANewPreference(string username, string pref, bool store)
        {
            //PreCondition
            Assert.IsFalse(_user.Preferences.Any(x=>x.ID == 0));
            Assert.IsFalse(_user.Preferences.Exists(x => x.Value == pref));

            //Act
            _controller.AddPreference(username, pref, store);

            //Assert
            Assert.IsTrue(_user.Preferences.Any(x => x.ID == 0));
            Assert.AreEqual(_user.Preferences.Count(), 4);
            Assert.IsTrue(_user.Preferences.Exists(x=> x.Value == pref));
        }

        [TestCase("DemoUser", "Kål", false)]
        [TestCase("DemoUser", "Nødder", false)]
        [TestCase("DemoUser", "Lam", false)]
        public void AddPreference_DiferentPreferences_ShouldHaveSamePreferencesCauseFailed(string username, string pref, bool store)
        {
            //Arrange
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);

            //Precondition
            Assert.AreEqual(3, _user.Preferences.Count());
            Assert.IsFalse(_user.Preferences.Any(x => x.ID == 0));
            Assert.IsFalse(_user.Preferences.Exists(x => x.Value == pref));

            //Arrange
            _controller.AddPreference(username, pref, store);

            //Act
            Assert.IsFalse(_user.Preferences.Any(x => x.ID == 0));
            Assert.AreEqual(_user.Preferences.Count(), 3);
            Assert.AreNotEqual(_user.Preferences.Count(), 4);
            Assert.IsFalse(_user.Preferences.Exists(x => x.Value == pref));
        }

        [TestCase("DemoUser", 1)]
        [TestCase("DemoUser", 2)]
        [TestCase("DemoUser", 3)]
        public void RemovePreference_AllThreePreferencesOnUser_ShouldBeGone(string username, int preferenceID)
        {
            //PreCondition
            Assert.AreEqual(3,_user.Preferences.Count());
            Assert.IsTrue(_user.Preferences.Exists(x => x.ID == preferenceID));

            //Act
            _controller.RemovePreference(username, preferenceID);

            //Assert
            Assert.AreNotEqual(3,_user.Preferences.Count());
            Assert.IsFalse(_user.Preferences.Exists(x=> x.ID == preferenceID));
        }

        [TestCase("DemoUser", 1)]
        [TestCase("DemoUser", 2)]
        [TestCase("DemoUser", 3)]
        public void RemovePreference_AllThreePreferencesOnUser_ShouldStillBeThereCauseFailed(string username, int preferenceID)
        {
            //Arrange
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);

            //PreCondition
            Assert.AreEqual(3, _user.Preferences.Count());
            Assert.IsTrue(_user.Preferences.Exists(x => x.ID == preferenceID));

            //Act
            _controller.RemovePreference(username, preferenceID);

            //Assert
            Assert.AreEqual(3, _user.Preferences.Count());
            Assert.IsTrue(_user.Preferences.Exists(x => x.ID == preferenceID));
        }

        [TestCase("Bilka")]
        [TestCase("Føtex")]
        [TestCase("Netto")]
        public void EditStore_DifferentNewStores_PreferenceAdded(string storename)
        {
            //PreCondition
            Assert.AreEqual(_user.Preferences.Count(), 3);
            Assert.IsFalse(_user.Preferences.Exists(x => x.Value == storename));

            //Act
            _controller.EditStore(storename);

            //Assert
            Assert.AreNotEqual(_user.Preferences.Count(), 3);
            Assert.AreEqual(_user.Preferences.Count(), 4);
            Assert.IsTrue(_user.Preferences.Exists(x => x.Value == storename));

        }


        [TestCase("Kvickly")]
        public void EditStore_DifferentAlreadyThereStrings_PreferenceRemoved(string storename)
        {
            //PreCondition
            Assert.AreEqual(_user.Preferences.Count(), 3);
            Assert.IsTrue(_user.Preferences.Exists(x => x.Value == storename));

            //Act
            _controller.EditStore(storename);

            //Assert
            Assert.AreNotEqual(_user.Preferences.Count(), 3);
            Assert.AreEqual(_user.Preferences.Count(), 2);
            Assert.IsFalse(_user.Preferences.Exists(x => x.Value == storename));

        }

        private object getvalue(string key, ViewResult view)
        {
            object value;
            view.ViewData.TryGetValue(key, out value);

            return value;
        }
    }
}
