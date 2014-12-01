using System;
using System.Linq;
using ProjectFood.Models;

namespace ProjectFood.Tests
{
    class TestShoppingListItemDbSet : TestDbSet<ShoppingList_Item>
    {
        public override ShoppingList_Item Find(params object[] keyValues)
        {
            return this.SingleOrDefault(shoppinglist => shoppinglist.ItemID == (int)keyValues.Single());
        }
    }
}