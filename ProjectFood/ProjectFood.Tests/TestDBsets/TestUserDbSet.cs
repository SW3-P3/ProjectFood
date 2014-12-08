using System.Linq;
using ProjectFood.Models;

namespace ProjectFood.Tests.TestDBsets
{
    class TestUserDbSet : TestDbSet<User>
    {
        public override User Find(params object[] keyValues)
        {
            return this.FirstOrDefault(shoppinglist => shoppinglist.ID == (int)keyValues.First());
        }
    }
}