using System.Linq;
using ProjectFood.Models;

namespace ProjectFood.Tests.TestDBsets
{
    class TestRecipesDbSet : TestDbSet<Recipe>
    {
        public override Recipe Find(params object[] keyValues)
        {
            return this.FirstOrDefault(shoppinglist => shoppinglist.ID == (int)keyValues.FirstOrDefault());
        }
    }
}