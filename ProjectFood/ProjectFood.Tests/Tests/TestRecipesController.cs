using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Moq;
using NUnit.Framework;
using ProjectFood.Controllers;
using ProjectFood.Models;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ProjectFood.Tests.Tests
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

            var recipe = DemoGetMethods.GetDemoRecipeWithItem(5,"Bøf med Bacon", 1);
            var recipe2 = DemoGetMethods.GetDemoRecipeWithItem(5,"Bacon og Ost" , 2);
            var recipe3 = DemoGetMethods.GetDemoRecipeWithItem(5,"Bøf med Ost" ,3);

            var rating = new Rating()
            {
                
                Recipe = recipe,
                Score = 1,
                User = _user
            };
            recipe.Ratings.Add(rating);
            _mockdata.Ratings.Add(rating);

            var shoppingList = new ShoppingList() {ID = 1};
            _user.ShoppingLists.Add(shoppingList);

            _mockdata.Recipes.Add(recipe);
            _mockdata.Recipes.Add(recipe2);
            _mockdata.Recipes.Add(recipe3);

            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty();
            _mockdata.ShoppingLists.Add(shoppinglist);
        }

        private User _user = new User();
        private RecipesController _controller = new RecipesController();
        private TestProjectFoodContext _mockdata = new TestProjectFoodContext();

        #endregion

        [Test]
        public void Recipe_DeleteConfirmedRecipe_Test()
        {

            Assert.IsTrue(_mockdata.Recipes.Any(x=> x.ID == 1));
            //Compute
            _controller.DeleteConfirmed(1);
            //Assert
            Assert.IsFalse(_mockdata.Recipes.Any(x => x.ID == 1));
        }

        [Test]
        public void Recipe_RemoveIngredient_Test()
        {
            Assert.IsTrue(_mockdata.Recipes.First(x => x.ID == 1).Ingredients.Any(x => x.ID == 1));
            //Compute
            var result = _controller.RemoveIngredient(1, 1);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_mockdata.Recipes.First(x => x.ID == 1).Ingredients.Any(x => x.ID == 1));
        }

        [Test]
        public void Recipe_AddIngredient_Test()
        {

            Assert.IsFalse(_mockdata.Recipes.First(x=> x.ID == 1).Ingredients.Any(x=> x.Name == "test"));
            //Compute
            var result = _controller.AddIngredient(1, "test", 10, "kg", 4);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.Recipes.First(x => x.ID == 1).Ingredients.Any(x => x.Name == "test"));
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

        [TestCase("Bøf")]
        [TestCase("Ost")]
        [TestCase("Bacon")]
        public void Recipe_IndexWithSearch_Test(string searchWord)
        {

            Assert.IsFalse(_mockdata.Recipes.All(x => x.Title.Contains(searchWord)));

            //Compute
            var result = _controller.Index("Old",searchWord);
            var viewModel = _controller.ViewData.Model as IEnumerable<Recipe>;
            var vb = ((ViewResult)result).ViewBag;

            //Assert
            Assert.IsNotNull(vb);
            Assert.AreEqual(vb.Selected, "Old");

            Assert.IsTrue(viewModel.All(x=>x.Title.Contains(searchWord)));

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [Test]
        public void Recipe_IndexNoSearch_Test()
        {

            //Compute
            var result = _controller.Index("Old", "");
            //Assert
            var vb = ((ViewResult)result).ViewBag;
            Assert.IsNotNull(vb);
            Assert.AreEqual(vb.Selected, "Old");
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [Test]
        public void Recipe_Details_Test()
        {
            
            //Compute
            var result = _controller.Details(1) as ViewResult;
            //Assert
            Assert.IsTrue(result.ViewBag.Author.Username == "DemoUser");
            Assert.IsTrue(result.ViewBag.UserRating.Score == 3);

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
        public void Recipe_EditFork_Test()
        {
            //Compute
            var result = _controller.Edit(1, true);
            var viewModel = _controller.ViewData.Model as Recipe;
            //Assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(_mockdata.Recipes.First(x => x.ID == 1).ID, viewModel.ID);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [Test]
        public void Recipe_Ingredients_Test()
        {
            //Compute
            var result = _controller.Ingredients(1, 4) as ViewResult;
            //Assert
            Assert.AreEqual(result.ViewBag.numPersons, 4);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [Test]
        public void Recipe_AddRating_Test()
        {

            Assert.IsFalse(_mockdata.Recipes.First(x => x.ID == 2).Ratings.Any(x=>x.ID == 0));
            //Compute
            var result = _controller.AddRating(2, 4);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.Recipes.First(x => x.ID == 2).Ratings.Any(x=>x.ID == 0));
            Assert.AreEqual(4, _mockdata.Recipes.First(x => x.ID == 2).Ratings.First(x => x.ID == 0).Score);
        }

        [Test]
        public void Recipe_AddItemToShoppingList_Test()
        {
            Assert.IsFalse(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any());
            //Compute
            var result = _controller.AddItemToShoppingList(1, 1, 100, "gram");
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.ShoppingLists.First(x=>x.ID == 1).Items.Any());
        }

        [Test]
        public void Recipe_RecommendRecipe_Test()
        {

            //Three recipes, recipe from Initialize(), and another two from this setup
            //The recipes from this setup has two items in common, which is different than from the Initialize()
            //Setup
            var recipe2 = DemoGetMethods.GetDemoRecipeWithItem(5, "TestRecipe2", 4);
            var recipe3 = DemoGetMethods.GetDemoRecipeWithItem(5, "TestRecipe3", 5);
            var item = DemoGetMethods.GetDemoItem(10, "laks");

            recipe2.Ingredients.Add(item);
            recipe3.Ingredients.Add(item);
            _mockdata.Recipes.Add(recipe2);
            _mockdata.Recipes.Add(recipe3);

            //Compute

            _controller.AddRating(4, 5);
            var result = _controller.RecommendRecipes(_user);

            //Assert
            Assert.AreEqual(recipe2, result.First());
            Assert.AreEqual(_mockdata.Recipes.First(), result.Last());
        }
    }
}
