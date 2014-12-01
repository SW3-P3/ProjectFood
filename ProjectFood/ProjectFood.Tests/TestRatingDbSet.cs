using System;
using System.Linq;
using ProjectFood.Models;

namespace ProjectFood.Tests
{
    class TestRatingDbSet : TestDbSet<Rating>
    {
        public override Rating Find(params object[] keyValues)
        {
            return this.SingleOrDefault(shoppinglist => shoppinglist.ID == (int)keyValues.Single());
        }
    }
}