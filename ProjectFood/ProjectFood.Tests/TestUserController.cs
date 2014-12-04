using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using ProjectFood.Controllers;
using ProjectFood.Models;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ProjectFood.Tests
{
    [TestFixture]
    public class TestUserController
    {
        #region Setup

        [SetUp]
        public void Initialize()
        {
            var mockdata = new TestProjectFoodContext();
            _controller = new UserController(mockdata);
            _user = DemoGetMethods.GetDemoUser(1);
            mockdata.Users.Add(_user);
            var controllerContext = new Mock<ControllerContext>();
            _principal = new Moq.Mock<IPrincipal>();
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            _principal.SetupGet(x => x.Identity.Name).Returns(_user.Name);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(_principal.Object);
            _controller.ControllerContext = controllerContext.Object;

            _user.Preferences.Add(DemoGetMethods.GetDemoPref(1,true,"Kvickly"));
            _user.Preferences.Add(DemoGetMethods.GetDemoPref(2,false,"Fisk"));
            _user.Preferences.Add(DemoGetMethods.GetDemoPref(3,false,"Kyling"));
        }

        private User _user = new User();
        private Moq.Mock<IPrincipal> _principal = new Moq.Mock<IPrincipal>(); 
        private UserController _controller = new UserController();

        #endregion  

        [Test]
        public void GetIndexView_NoInputsNeeded_ShouldReturnViewResult()
        {
            var result = _controller.Index();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            
        }

        [Test]
        public void GetIndexView_UserNotLoggedIn_ShouldReturnRedirectToRouteResult()
        {
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);

            var result = _controller.Index();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

        }

        [TestCase("Lars", Result = "Lars")]
        [TestCase("Peter", Result = "Peter")]
        [TestCase("Søren", Result = "Søren")]
        public string EditName_DifferentNames_ShouldChangeName(string name)
        {
            Assert.AreNotEqual(name,_user.Name);

            _controller.EditName(_user.Username, name);

            return _user.Name;
        }

        [TestCase("Lars")]
        [TestCase("Peter")]
        [TestCase("Søren")]
        public void EditName_UserNotLoggedIn_ShouldReturnRedirectToRouteResult(string name)
        {
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);

            var result = _controller.EditName(_user.Username, name);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

        }

        [Test]
        public void EditPreferencesView_NoInputIsNeeded_ShouldReturnViewResult()
        {


            var result = _controller.EditPreferences();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof (ViewResult));

        }

        [Test]
        public void EditPreferencesView_UserNotLoggedIn_ShouldReturnRedirectToRouteResult()
        {
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);

            var result = _controller.EditPreferences();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

        }

        [TestCase("DemoUser", "Kål", false)]
        [TestCase("DemoUser", "Nødder", false)]
        [TestCase("DemoUser", "Lam", false)]
        public void AddPreference_DiferentPreferences_ShouldHaveANewPreference(string username, string pref, bool store)
        {
            Assert.AreEqual(3, _user.Preferences.Count());
            Assert.IsFalse(_user.Preferences.Exists(x => x.Value == pref));

            _controller.AddPreference(username, pref, store);

            Assert.AreNotEqual(_user.Preferences.Count(), 3);
            Assert.AreEqual(_user.Preferences.Count(), 4);
            Assert.IsTrue(_user.Preferences.Exists(x=> x.Value == pref));
        }

        [TestCase("DemoUser", "Kål", false)]
        [TestCase("DemoUser", "Nødder", false)]
        [TestCase("DemoUser", "Lam", false)]
        public void AddPreference_DiferentPreferences_ShouldHaveSamePreferencesCauseFailed(string username, string pref, bool store)
        {
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);

            Assert.AreEqual(3, _user.Preferences.Count());
            Assert.IsFalse(_user.Preferences.Exists(x => x.Value == pref));

            _controller.AddPreference(username, pref, store);

            Assert.AreEqual(_user.Preferences.Count(), 3);
            Assert.AreNotEqual(_user.Preferences.Count(), 4);
            Assert.IsFalse(_user.Preferences.Exists(x => x.Value == pref));
        }

        [TestCase("DemoUser", 1)]
        [TestCase("DemoUser", 2)]
        [TestCase("DemoUser", 3)]
        public void RemovePreference_AllThreePreferencesOnUser_ShouldBeGone(string username, int preferenceID)
        {
            Assert.AreEqual(3,_user.Preferences.Count());
            Assert.IsTrue(_user.Preferences.Exists(x => x.ID == preferenceID));

            _controller.RemovePreference(username, preferenceID);

            Assert.AreNotEqual(3,_user.Preferences.Count());
            Assert.IsFalse(_user.Preferences.Exists(x=> x.ID == preferenceID));
        }

        [TestCase("DemoUser", 1)]
        [TestCase("DemoUser", 2)]
        [TestCase("DemoUser", 3)]
        public void RemovePreference_AllThreePreferencesOnUser_ShouldStillBeThereCauseFailed(string username, int preferenceID)
        {
            _principal.Setup(x => x.Identity.IsAuthenticated).Returns(false);

            Assert.AreEqual(3, _user.Preferences.Count());
            Assert.IsTrue(_user.Preferences.Exists(x => x.ID == preferenceID));

            _controller.RemovePreference(username, preferenceID);

            Assert.AreEqual(3, _user.Preferences.Count());
            Assert.IsTrue(_user.Preferences.Exists(x => x.ID == preferenceID));
        }

        [TestCase("Bilka")]
        [TestCase("Føtex")]
        [TestCase("Netto")]
        public void EditStore_DifferentNewStores_PreferenceAdded(string storename)
        {
            Assert.AreEqual(_user.Preferences.Count(), 3);
            Assert.IsFalse(_user.Preferences.Exists(x => x.Value == storename));

            _controller.EditStore(storename);

            Assert.AreNotEqual(_user.Preferences.Count(), 3);
            Assert.AreEqual(_user.Preferences.Count(), 4);
            Assert.IsTrue(_user.Preferences.Exists(x => x.Value == storename));

        }


        [TestCase("Kvickly")]
        public void EditStore_DifferentAlreadyThereStrings_PreferenceRemovd(string storename)
        {
            Assert.AreEqual(_user.Preferences.Count(), 3);
            Assert.IsTrue(_user.Preferences.Exists(x => x.Value == storename));

            _controller.EditStore(storename);

            Assert.AreNotEqual(_user.Preferences.Count(), 3);
            Assert.AreEqual(_user.Preferences.Count(), 2);
            Assert.IsFalse(_user.Preferences.Exists(x => x.Value == storename));

        }
    }
}
