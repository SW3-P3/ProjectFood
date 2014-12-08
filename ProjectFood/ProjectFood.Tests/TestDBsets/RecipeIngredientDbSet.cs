﻿using System.Linq;
using ProjectFood.Models;

namespace ProjectFood.Tests.TestDBsets
{
    class TestRecipeIngredientDbSet : TestDbSet<Recipe_Ingredient>
    {
        public override Recipe_Ingredient Find(params object[] keyValues)
        {
            return this.SingleOrDefault(shoppinglist => shoppinglist.IngredientID == (int)keyValues.Single());
        }
    }
}