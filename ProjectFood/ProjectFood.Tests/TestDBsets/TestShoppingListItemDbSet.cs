using System.Linq;
using ProjectFood.Models;

namespace ProjectFood.Tests.TestDBsets
{
    class TestShoppingListItemDbSet : TestDbSet<ShoppingList_Item>
    {
        public override ShoppingList_Item Find(params object[] keyValues)
        {
            return this.FirstOrDefault(shoppinglist => shoppinglist.ItemID == (int)keyValues.First());
        }
    }
}