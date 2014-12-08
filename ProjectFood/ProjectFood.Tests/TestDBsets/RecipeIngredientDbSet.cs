using System.Linq;
using ProjectFood.Models;

namespace ProjectFood.Tests.TestDBsets
{
    class TestRecipeIngredientDbSet : TestDbSet<Recipe_Ingredient>
    {
        public override Recipe_Ingredient Find(params object[] keyValues)
        {
            return this.FirstOrDefault(shoppinglist => shoppinglist.IngredientID == (int)keyValues.FirstOrDefault());
        }
    }
}