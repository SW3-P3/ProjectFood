using System;
using System.Linq;
using ProjectFood.Models;

namespace ProjectFood.Tests
{
    class TestPreferenceDbSet : TestDbSet<Pref>
    {
        public override Pref Find(params object[] keyValues)
        {
            return this.SingleOrDefault(shoppinglist => shoppinglist.ID == (int)keyValues.Single());
        }
    }
}