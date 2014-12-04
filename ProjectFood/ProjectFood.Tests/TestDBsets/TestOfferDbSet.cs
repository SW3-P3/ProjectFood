using System.Linq;
using ProjectFood.Models;

namespace ProjectFood.Tests.TestDBsets
{
    class TestOfferDbSet : TestDbSet<Offer>
    {
        public override Offer Find(params object[] keyValues)
        {
            return this.SingleOrDefault(shoppinglist => shoppinglist.ID == (int)keyValues.Single());
        }
    }
}