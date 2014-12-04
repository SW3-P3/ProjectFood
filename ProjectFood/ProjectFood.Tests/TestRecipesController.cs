using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using ProjectFood.Controllers;
using ProjectFood.Models;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;


namespace ProjectFood.Tests
{
    [TestFixture]
    public class TestRecipesController
    {
        #region Setup

        [SetUp]
        public void Initialize()
        {
            _mockdata = new TestProjectFoodContext();
            _controller = new RecipesController(_mockdata);
            _user = DemoGetMethods.GetDemoUser(1);
            _mockdata.Users.Add(_user);
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Moq.Mock<IPrincipal>();
            principal.Setup(x => x.Identity.IsAuthenticated).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns(_user.Name);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            _controller.ControllerContext = controllerContext.Object;

            var recipe = DemoGetMethods.GetDemoRecipeWithItem(5);
            _mockdata.Recipes.Add(recipe);
        }

        private User _user = new User();
        private RecipesController _controller = new RecipesController();
        private TestProjectFoodContext _mockdata = new TestProjectFoodContext();

        #endregion

        [Test]
        public void Recipe_DeleteConfirmedRecipe_Test()
        {

            //Compute
            var result = _controller.DeleteConfirmed(1);
            //Assert
            Assert.AreEqual(0, _mockdata.Recipes.Count());
        }

        [Test]
        public void Recipe_RemoveIngredient_Test()
        {
            //Compute
            var result = _controller.RemoveIngredient(1, 1);
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, _mockdata.Recipes.First().Ingredients.Count);
        }

        [Test]
        public void Recipe_AddIngredient_Test()
        {
            //Compute
            var result = _controller.AddIngredient(1, "test", 10, "kg", 4);
            //Assert
            Assert.AreEqual(6, _mockdata.Recipes.First().Ingredients.Count);
        }

        [Test]
        public void Recipe_Create_Test()
        {
            //Compute
            var result = _controller.Create();
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [Test]
        public void Recipe_Index_Test()
        {

            //Compute
            var result = _controller.Index("Old");
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [Test]
        public void Recipe_Details_Test()
        {
            //Compute
            var result = _controller.Details(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [Test]
        public void Recipe_Edit_Test()
        {
            //Compute
            var result = _controller.Edit(1, false);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [Test]
        public void Recipe_Ingredients_Test_Test()
        {
            //Compute
            var result = _controller.Ingredients(1, 4);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [Test]
        public void Recipe_AddRating_Test()
        {
            //Compute
            var result = _controller.AddRating(1, 4);
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, _mockdata.Recipes.First().Ratings.Count);
            Assert.AreEqual(4, _mockdata.Recipes.First().Ratings.First().Score);
        }

        [Test]
        public void Recipe_AddItemToShoppingList_Test()
        {
            //Setup
            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            _mockdata.ShoppingLists.Add(shoppinglist);

            //Compute
            var result = _controller.AddItemToShoppingList(1, 1, 100, "gram");
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, _mockdata.ShoppingLists.First().Items.Count);
        }

        [Test]
        public void Recipe_RecommendRecipe_Test()
        {

            // tre recipes, recipe fra TestFixture, og yderlige to fra dette setup.
            // De to fra dette setup har et items tilfældes, som er forskelligt fra TestFixture recipe.
            //Setup
            var recipe2 = DemoGetMethods.GetDemoRecipeWithItem(5, "TestRecipe2", 2);
            var recipe3 = DemoGetMethods.GetDemoRecipeWithItem(5, "TestRecipe3", 3);
            _mockdata.Recipes.Add(recipe2);
            _mockdata.Recipes.Add(recipe3);
            //Compute
            _controller.AddRating(1, 1);
            _controller.AddRating(2, 5);
            var result = _controller.RecommendRecipes(_user);
            Assert.AreEqual(recipe2, result.First());
            Assert.AreEqual(_mockdata.Recipes.First(), result.Last());
        }
    }
}
