using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    public class Item
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ICollection<Offer> Offers { get; set; }

        public Item()
        {
            Offers = new List<Offer>();
        }
    }
}