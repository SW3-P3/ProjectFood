using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    public class Item
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public List<Offer> Offers = new List<Offer>();

        public Item(string name)
        {
            Name = name;
            Category = String.Empty;
            Offers.Add(new Offer(){Store = "Erotica", Price = 999999.3M});
        }
    }
}