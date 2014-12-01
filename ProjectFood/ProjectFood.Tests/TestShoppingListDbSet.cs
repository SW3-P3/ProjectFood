using System;
using System.Linq;
using ProjectFood.Models;

namespace ProjectFood.Tests
{
    class TestShoppingListDbSet : TestDbSet<ShoppingList>
    {
        public override ShoppingList Find(params object[] keyValues)
        {
            return this.SingleOrDefault(shoppinglist => shoppinglist.ID == (int)keyValues.Single());
        }
    }
}