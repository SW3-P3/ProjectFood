using System.Linq;
using ProjectFood.Models;

namespace ProjectFood.Tests.TestDBsets
{
    class TestItemDbSet : TestDbSet<Item>
    {
        public override Item Find(params object[] keyValues)
        {
            return this.SingleOrDefault(shoppinglist => shoppinglist.ID == (int)keyValues.Single());
        }
    }
}