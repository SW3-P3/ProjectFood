using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    public class Offer
    {
        public Item GenericItem { get; set; }
        public string Brand { get; set; }
        public string Store { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
    }
}