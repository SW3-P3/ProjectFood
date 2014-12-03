using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
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
            //Assert.IsNotNull(result);
            Assert.AreEqual(6, mockData.Recipes.First().Ingredients.Count);
        }

    }
}
