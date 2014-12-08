using System.Linq;
using ProjectFood.Models;

namespace ProjectFood.Tests.TestDBsets
{
    class TestShoppingListDbSet : TestDbSet<ShoppingList>
    {
        public override ShoppingList Find(params object[] keyValues)
        {
            return this.FirstOrDefault(shoppinglist => shoppinglist.ID == (int)keyValues.First());
        }
    }
}