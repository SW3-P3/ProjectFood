using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using ProjectFood.Models;
using ProjectFood.Controllers;


namespace ProjectFood.Tests
{
    [TestClass]
    public class TestRecipesController
    {
        [TestMethod]
        public void DeleteRecipe_Test()
        {
            //Setup
            var mockData = new TestProjectFoodContext();
            var controller = new RecipesController();
            var recipe = DemoGetMethods.GetDemoRecipeWithItem(5, 1);
            mockData.Recipes.Add(recipe);
            //Compute
            var result = controller.Delete(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, mockData.Recipes.First().ID);
        }

        [TestMethod]
        public void DeleteConfirmedRecipe_Test()
        {
            //Setup
            var mockData = new TestProjectFoodContext();
            var controller = new RecipesController();
            var recipe = DemoGetMethods.GetDemoRecipeWithItem(5, 1);
            mockData.Recipes.Add(recipe);
            //Compute
            var result = controller.DeleteConfirmed(1);
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, mockData.Recipes.First().Ingredients.Count);
        }

        [TestMethod]
        public void RemoveIngredient_Test()
        {
            //Setup
            var mockData = new TestProjectFoodContext();
            var controller = new RecipesController();
            var recipe = DemoGetMethods.GetDemoRecipeWithItem(5, 1);
            mockData.Recipes.Add(recipe);
            //Compute
            var result = controller.RemoveIngredient(1, 1);
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, mockData.Recipes.First().Ingredients.Count);
        }

        [TestMethod]
        public void AddItemToShoppingList_Test()
        {
            //Setup
            var mockData = new TestProjectFoodContext();
            var controller = new RecipesController();
            var recipe = DemoGetMethods.GetDemoRecipeWithItem(5, 1);
            mockData.Recipes.Add(recipe);
            //Compute
            // var result = controller.AddIngredient()
            //Assert
            //Assert.IsNotNull(result);
            Assert.AreEqual(6, mockData.Recipes.First().Ingredients.Count);
        }

    }
}
