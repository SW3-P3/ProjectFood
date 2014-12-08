using System.Linq;
using ProjectFood.Models;

namespace ProjectFood.Tests.TestDBsets
{
    class TestOfferDbSet : TestDbSet<Offer>
    {
        public override Offer Find(params object[] keyValues)
        {
            return this.FirstOrDefault(shoppinglist => shoppinglist.ID == (int)keyValues.First());
        }
    }
}