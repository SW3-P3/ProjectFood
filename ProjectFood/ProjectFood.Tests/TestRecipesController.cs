using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProjectFood.Controllers;


namespace ProjectFood.Tests
{
    [TestClass]
    public class TestRecipesController
    {

        [TestMethod]
        public void Recipe_DeleteConfirmedRecipe_Test()
        {
            //Setup
            var mockData = new TestProjectFoodContext();
            var controller = new RecipesController(mockData);
            var recipe = DemoGetMethods.GetDemoRecipeWithItem(5, 1);
            mockData.Recipes.Add(recipe);
            //Compute
            var result = controller.DeleteConfirmed(1);
            //Assert
            Assert.AreEqual(0, mockData.Recipes.Count());
        }

        [TestMethod]
        public void Recipe_RemoveIngredient_Test()
        {
            //Setup
            var mockData = new TestProjectFoodContext();
            var controller = new RecipesController(mockData);
            var recipe = DemoGetMethods.GetDemoRecipeWithItem(5, 1);
            mockData.Recipes.Add(recipe);
            //Compute
            var result = controller.RemoveIngredient(1, 1);
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, mockData.Recipes.First().Ingredients.Count);
        }

        [TestMethod]
        public void Recipe_AddIngredient_Test()
        {
            //Setup
            var mockData = new TestProjectFoodContext();
            var controller = new RecipesController(mockData);
            var recipe = DemoGetMethods.GetDemoRecipeWithItem(5, 1);
            mockData.Recipes.Add(recipe);
            //Compute
            var result = controller.AddIngredient(1, "test", 10, "kg", 4);
            //Assert
            Assert.AreEqual(6, mockData.Recipes.First().Ingredients.Count);
        }

        [TestMethod]
        public void Recipe_Create_Test()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new RecipesController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            mockdata.Users.Add(user);
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            //Compute
            var result = controller.Create();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [TestMethod]
        public void Recipe_Index_Test()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new RecipesController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            mockdata.Users.Add(user);
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            //Compute
            var result = controller.Index("Old");
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [TestMethod]
        public void Recipe_Details_Test()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new RecipesController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            mockdata.Users.Add(user);
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);

            var recipe = DemoGetMethods.GetDemoRecipeWithItem(5, 1);
            mockdata.Recipes.Add(recipe);
            //Compute
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            //Assert
            var result = controller.Details(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [TestMethod]
        public void Recipe_Edit_Test()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new RecipesController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            mockdata.Users.Add(user);
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);

            var recipe = DemoGetMethods.GetDemoRecipeWithItem(5, 1);
            mockdata.Recipes.Add(recipe);
            //Compute
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            //Assert
            var result = controller.Edit(1, false);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [TestMethod]
        public void Recipe_Ingredients_Test_Test()
        {
            //Setup
            var mockdata = new TestProjectFoodContext();
            var controller = new RecipesController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            mockdata.Users.Add(user);
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);

            var recipe = DemoGetMethods.GetDemoRecipeWithItem(5, 1);
            mockdata.Recipes.Add(recipe);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            //Compute
            var result = controller.Ingredients(1, 4);
            //Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [TestMethod]
        public void Recipe_AddRating_Test()
        {
            //Setup

            var mockdata = new TestProjectFoodContext();
            var controller = new RecipesController(mockdata);
            var user = DemoGetMethods.GetDemoUser(1);
            mockdata.Users.Add(user);
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);

            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;

            var recipe = DemoGetMethods.GetDemoRecipeWithItem(5, 1);
            mockdata.Recipes.Add(recipe);
            //Compute
            var result = controller.AddRating(1, 4);
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, mockdata.Recipes.First().Ratings.Count);
        }

        public void Recipe_AddItemToShoppingList_Test()
        {
            //Setup

            var mockdata = new TestProjectFoodContext();
            var controller = new RecipesController(mockdata);

            var user = DemoGetMethods.GetDemoUser(1);
            mockdata.Users.Add(user);
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(user.Name);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;

            var recipe = DemoGetMethods.GetDemoRecipeWithItem(5, 1);
            mockdata.Recipes.Add(recipe);

            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            mockdata.ShoppingLists.Add(shoppinglist);

            //Compute
            var result = controller.AddItemToShoppingList(1, 1, 100, "gram");
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, mockdata.ShoppingLists.First().Items.Count);
        }
    }
}
