using System.Linq;
using ProjectFood.Models;

namespace ProjectFood.Tests.TestDBsets
{
    class TestRatingDbSet : TestDbSet<Rating>
    {
        public override Rating Find(params object[] keyValues)
        {
            return this.FirstOrDefault(shoppinglist => shoppinglist.ID == (int)keyValues.First());
        }
    }
}