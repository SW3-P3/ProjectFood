using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    public class Item
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public List<Offer> Offers { get; set; }

        public Item(string name)
        {
            Name = name;
        }
    }
}