using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    public class Recipe
    {
        public int ID { get; set; }
        public string Titel { get; set; }
        public ICollection<double> Amount { get; set; }
        public ICollection<String> Unit { get; set; }
        public ICollection<Item> Ingredients { get; set; }
        public int Minutes { get; set; }
        public string Instructions { get; set; }
    }
}