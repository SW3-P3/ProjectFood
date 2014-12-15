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

            var rating2 = new Rating()
            {
                
                Recipe = recipe,
                Score = 4,
                User = _user
            };

            _mockdata.Ratings.Add(rating);
            recipe.Ratings.Add(rating);

            _mockdata.Ratings.Add(rating2);
            recipe.Ratings.Add(rating2);

            var shoppingList = new ShoppingList() {ID = 1, Title = "DemoShoppingList"};
            _user.ShoppingLists.Add(shoppingList);

            _mockdata.Recipes.Add(recipe);
            _mockdata.Recipes.Add(recipe2);
            _mockdata.Recipes.Add(recipe3);

            var shoppinglist = DemoGetMethods.GetDemoShoppingListEmpty(_user);
            _mockdata.ShoppingLists.Add(shoppinglist);
        }

        private User _user = new User();
        private RecipesController _controller = new RecipesController();
        private TestProjectFoodContext _mockdata = new TestProjectFoodContext();

        #endregion

        [Test]
        public void RecipeDeleteConfirmed_RecipeID_ShouldBeRemoved()
        {
            //Precondition
            Assert.IsTrue(_mockdata.Recipes.Any(x=> x.ID == 1));

            //Act
            _controller.DeleteConfirmed(1);

            //Assert
            Assert.IsFalse(_mockdata.Recipes.Any(x => x.ID == 1));
        }

        [Test]
        public void RecipeRemoveIngredient_RecipeAndIngredient_ShouldBeRemoved()
        {
            //Precondition
            Assert.IsTrue(_mockdata.Recipes.First(x => x.ID == 1).Ingredients.Any(x => x.ID == 1));

            //Act
            var result = _controller.RemoveIngredient(1, 1);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_mockdata.Recipes.First(x => x.ID == 1).Ingredients.Any(x => x.ID == 1));
        }

        [Test]
        public void RecipeAddIngredient_Itemvalues_ShouldBeAdded()
        {
            //Precondition
            Assert.IsFalse(_mockdata.Recipes.First(x=> x.ID == 1).Ingredients.Any(x=> x.Name == "test"));
            Assert.IsFalse(_mockdata.Items.Any(x => x.Name == "test"));

            //Act
            var result = _controller.AddIngredient(1, "test", 10, "kg", 4);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.Recipes.First(x => x.ID == 1).Ingredients.Any(x => x.Name == "test"));
        }

        [Test]
        public void RecipeAddIngredient_ItemNoName_ShouldNotBeAdded()
        {
            //Precondition
            Assert.IsFalse(_mockdata.Recipes.First(x => x.ID == 1).Ingredients.Any(x => x.Name == "test"));

            //Act
            var result = _controller.AddIngredient(1, "", 10, "kg", 4);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_mockdata.Recipes.First(x => x.ID == 1).Ingredients.Any(x => x.Name == "test"));
        }
        [Test]
        public void RecipeAddIngredient_ItemNameNoAmountPerPerson_ShouldBeAdded()
        {
            //Precondition
            Assert.IsFalse(_mockdata.Recipes.First(x => x.ID == 1).Ingredients.Any(x => x.Name == "test"));

            //Act
            var result = _controller.AddIngredient(1, "test", null, "kg", 4);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.Recipes.First(x => x.ID == 1).Ingredients.Any(x => x.Name == "test"));
            Assert.IsTrue(_mockdata.Recipe_Ingredient.First(x=>x.Ingredient.Name == "test").AmountPerPerson == 0);
        }

        [Test]
        public void RecipeAddIngredient_ItemNameAlreadyThere_ShouldBeAddedItemAlreadyThere()
        {
            //Arrange
            _mockdata.Items.Add(new Item() { Name = "test" });

            //Precondition
            Assert.IsTrue(_mockdata.Items.Any(x=>x.Name =="test"));
            Assert.IsFalse(_mockdata.Recipes.First(x => x.ID == 1).Ingredients.Any(x => x.Name == "test"));

            //Act
            var result = _controller.AddIngredient(1, "test", null, "kg", 4);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.Recipes.First(x => x.ID == 1).Ingredients.Any(x => x.Name == "test"));
            Assert.IsFalse(_mockdata.Items.Count(x => x.Name == "test") == 2);
        }

        [Test]
        public void RecipeAddIngredient_ItemAlreadyOnList_ShouldBeAddedItemAlreadyThere()
        {
            //Arrange
            var item = new Item() {Name = "test", ID = 0};
            _mockdata.Items.Add(item);
            _mockdata.Recipes.First(x=>x.ID == 1).Ingredients.Add(item);
            _mockdata.Recipe_Ingredient.Add(new Recipe_Ingredient()
            {
                AmountPerPerson = 2.0,
                Ingredient = item,
                IngredientID = 0,
                Recipe = _mockdata.Recipes.First(x => x.ID == 1),
                RecipeID = 1,
                Unit = "stk"
            });

            //Precondition
            Assert.IsTrue(_mockdata.Items.Any(x => x.Name == "test"));
            Assert.IsTrue(_mockdata.Recipes.First(x => x.ID == 1).Ingredients.Any(x => x.Name == "test"));

            //Act
            var result = _controller.AddIngredient(1, "test", null, "kg", 4);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.Recipes.First(x => x.ID == 1).Ingredients.Any(x => x.Name == "test"));
            Assert.IsFalse(_mockdata.Items.Count(x => x.Name == "test") == 2);
        }



        [Test]
        public void RecipeCreate_NoInput_ShouldOpenCreateWindow()
        {
            //Act
            var result = _controller.Create();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [TestCase("Bøf", "New")]
        [TestCase("Ost", "Old")]
        [TestCase("Bøf", "High")]
        public void RecipeIndex_DifferentSortsAndDifferentSearches_ShouldResultIn2RecipesWithSort(string searchWord, string sort)
        {
            //PreCondition
            Assert.IsFalse(_mockdata.Recipes.All(x => x.Title.Contains(searchWord)));

            //Act
            var result = _controller.Index(sort,searchWord);
            var viewModel = _controller.ViewData.Model as IEnumerable<Recipe>;
            var vb = ((ViewResult)result).ViewBag;

            //Assert
            Assert.IsNotNull(vb);
            Assert.AreEqual(vb.Selected, sort);
            Assert.IsTrue(viewModel.All(x=>x.Title.Contains(searchWord)));
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [TestCase("Bøf", "Recommend")]
        public void RecipeIndex_OneSearchAndRecommend_ShouldResultInAllRecipesWithRecommend(string searchWord, string sort)
        {
            //PreCondition
            Assert.IsFalse(_mockdata.Recipes.All(x => x.Title.Contains(searchWord)));

            //Act
            var result = _controller.Index(sort, searchWord);
            var viewModel = _controller.ViewData.Model as IEnumerable<Recipe>;
            var vb = ((ViewResult)result).ViewBag;

            //Assert
            Assert.IsNotNull(vb);
            Assert.AreEqual(vb.Selected, sort);
            Assert.IsFalse(viewModel.All(x => x.Title.Contains(searchWord)));
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [TestCase("Bøf", "nope")]
        public void RecipeIndex_OneSearchAndNoSort_ShouldResultIn2WithNewSort(string searchWord, string sort)
        {
            //PreCondition
            Assert.IsFalse(_mockdata.Recipes.All(x => x.Title.Contains(searchWord)));

            //Act
            var result = _controller.Index(sort, searchWord);
            var viewModel = _controller.ViewData.Model as IEnumerable<Recipe>;
            var vb = ((ViewResult)result).ViewBag;

            //Assert
            Assert.IsNotNull(vb);
            Assert.IsTrue(vb.Selected == "New");
            Assert.IsTrue(viewModel.All(x => x.Title.Contains(searchWord)));
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [TestCase("New")]
        [TestCase("Old")]
        [TestCase("Recommend")]
        [TestCase("High")]

        public void RecipeIndex_DifferentSortsNoSearch_ShouldReturnAllRecipesWithSort(string sort)
        {

            //Act
            var result = _controller.Index(sort, "");
            //Assert
            var vb = ((ViewResult)result).ViewBag;
            Assert.IsNotNull(vb);
            Assert.AreEqual(vb.Selected, sort);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }
        [TestCase("Nope")]
        public void RecipeIndex_NoSortNoSearch_ShouldReturnAllRecipesNoSort(string sort)
        {

            //Act
            var result = _controller.Index(sort, "");
            //Assert
            var vb = ((ViewResult)result).ViewBag;
            Assert.IsNotNull(vb);
            Assert.AreEqual(vb.Selected, "New");
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [Test]
        public void RecipeDetails_RecipeID1_ShouldLoadViewWithData()
        {
            
            //Act
            var result = _controller.Details(1) as ViewResult;
            var shoppingList = getvalue("ShoppingLists", result) as List<ShoppingList>;
            var averageRating = getvalue("AverageScore", result);
            var ratingCount = getvalue("RatingCount", result);
            
            //Assert
            Assert.IsTrue(result.ViewBag.Author.Username == "DemoUser");
            Assert.IsTrue(result.ViewBag.UserRating.Score == 1);
            Assert.IsTrue(shoppingList.First().Title == "DemoShoppingList");
            Assert.AreEqual(3M, averageRating );
            Assert.AreEqual(4, ratingCount);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [Test]
        public void RecipeEdit_RecipeID1False_ShouldReturnView()
        {
            //Act
            var result = _controller.Edit(1, false);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [Test]
        public void RecipeEditFork_RecipeID1True_ShouldReturnView()
        {
            //Act
            var result = _controller.Edit(1, true) as ViewResult;
            var viewModel = _controller.ViewData.Model as Recipe;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewBag.Forked == true);
            Assert.AreNotEqual(_mockdata.Recipes.First(x => x.ID == 1).ID, viewModel.ID);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void RecipeIngredients_NumberOfPersonsHasValueRecipeID1_ShouldSetNumberToValue(int? numPersons)
        {
            //Act
            var result = _controller.Ingredients(1, numPersons) as ViewResult;

            //Assert
            Assert.AreEqual(result.ViewBag.numPersons, numPersons);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        public void RecipeIngredients_NumberOfPersonsHasNoValueRecipeID1_ShouldBeFour()
        {
            //Act
            var result = _controller.Ingredients(1, null) as ViewResult;

            //Assert
            Assert.AreEqual(result.ViewBag.numPersons, 4);   
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [Test]
        public void RecipeAddRating_RecipeId2Rating4_ShouldAddRating()
        {
            //PreCondition
            Assert.IsFalse(_mockdata.Recipes.First(x => x.ID == 2).Ratings.Any(x=>x.Score == 5));

            //Act
            var result = _controller.AddRating(2, 5);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.Recipes.First(x => x.ID == 2).Ratings.Any(x=>x.Score == 5));
        }

        [Test]
        public void RecipeAddItemToShoppingList_ItemValues_ShouldAddItem()
        {
            //PreCondition
            Assert.IsFalse(_mockdata.ShoppingLists.First(x => x.ID == 1).Items.Any());

            //Act
            var result = _controller.AddItemToShoppingList(1, 1, 100, "gram");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_mockdata.ShoppingLists.First(x=>x.ID == 1).Items.Any());
        }

        [Test]
        public void RecipeRecommendRecipe_Test()
        {

            //Three recipes, recipe from Initialize(), and another two from this setup
            //The recipes from this setup has two items in common, which is different than from the Initialize()
            
            //Arrange
            var recipe2 = DemoGetMethods.GetDemoRecipeWithItem(5, "TestRecipe2", 4);
            var recipe3 = DemoGetMethods.GetDemoRecipeWithItem(5, "TestRecipe3", 5);
            var item = DemoGetMethods.GetDemoItem(10, "laks");
            
            recipe2.Ingredients.Add(item);
            recipe3.Ingredients.Add(item);
            _mockdata.Recipes.Add(recipe2);
            _mockdata.Recipes.Add(recipe3);

            //Act

            _controller.AddRating(4, 5);
            var result = _controller.RecommendRecipes(_user);

            //Assert
            Assert.AreEqual(recipe2, result.First());
            Assert.AreEqual(_mockdata.Recipes.First(), result.Last());
        }

        private object getvalue(string key, ViewResult view)
        {
            object value;
            view.ViewData.TryGetValue(key, out value);

            return value;
        }
    }
}
