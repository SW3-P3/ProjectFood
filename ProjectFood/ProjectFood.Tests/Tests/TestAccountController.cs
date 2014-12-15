using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using ProjectFood.Controllers;
using ProjectFood.Models;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ProjectFood.Tests.Tests
{
    [TestFixture]
    public class TestAccountController
    {
        #region setup

        [SetUp]
        public void initialize()
        {
            _mockdata = new TestProjectFoodContext();
            _controller = new AccountController(_mockdata);


        }

        private TestProjectFoodContext _mockdata;
        private AccountController _controller;

        #endregion
        [Test]
        public void AccountRegister_ModelStateInvalid_ShouldNotRegister()
        {
            //Arrange
            _controller.ModelState.Add("testError", new ModelState());
            _controller.ModelState.AddModelError("testError", "test");

            //Act
            var result = _controller.Register(new RegisterViewModel(){ConfirmPassword = "password", Email = "test@mail.dk", Password = "password"}, "hej");

            //Assert
            Assert.IsInstanceOfType(result, typeof (Task<ActionResult>));
            Assert.IsFalse(_mockdata.Users.Any());
        }
        [Test]
        public void AccountRegister_NameEmpty_ShouldNotRegister()
        {
            //Act
            var result = _controller.Register(new RegisterViewModel() { ConfirmPassword = "password", Email = "test@mail.dk", Password = "password" }, "");

            //Assert
            Assert.IsInstanceOfType(result, typeof(Task<ActionResult>));
            Assert.IsFalse(_mockdata.Users.Any());
        }

        [Test]
        public void AccountCreateProjectFoodUser_AllInfo_ShouldCreateUserWithShoppingList()
        {
            //Act
            _controller.CreateProjectFoodUser(new RegisterViewModel() { ConfirmPassword = "password", Email = "test@mail.dk", Password = "password" }, "Test");

            //Assert
            Assert.IsTrue(_mockdata.Users.Any(x=>x.Name == "Test"));
            Assert.IsTrue(_mockdata.Users.Any(x => x.Username == "test@mail.dk"));
            Assert.IsTrue(_mockdata.Users.Any(x => x.ShoppingLists.Any(l=> l.Title == "Min Indkøbsliste")));
        }
    }
}
