using System;
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
            controller = new UserController(mockdata);
            _user = DemoGetMethods.GetDemoUser(1);
            mockdata.Users.Add(_user);
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(_user.Name);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
        }

        private User _user = new User();
        private UserController controller = new UserController();

        #endregion  

        [Test]
        public void GetIndexView_NoInputsNeeded_ShouldReturnViewResult()
        {
            var result = controller.Index();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            
        }

        [TestCase("Lars", Result = "Lars")]
        [TestCase("Peter", Result = "Peter")]
        [TestCase("Søren", Result = "Søren")]
        public string EditName_DifferentNames_ShouldChangeName(string name)
        {

            Assert.AreNotEqual(name,_user.Name);

            controller.EditName(_user.Username, name);

            return _user.Name;
        }
    }
}
