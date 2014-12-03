using System;
using System.Linq;
using ProjectFood.Models;

namespace ProjectFood.Tests
{
    class TestRecipesDbSet : TestDbSet<Recipe>
    {
        public override Recipe Find(params object[] keyValues)
        {
            return this.SingleOrDefault(shoppinglist => shoppinglist.ID == (int)keyValues.Single());
        }
    }
}