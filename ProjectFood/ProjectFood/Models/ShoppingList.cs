using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    public class ShoppingList
    {
        public string Title { get; set; }
        public List<Item> Items = new List<Item>();
        public List<Item> BoughtItems { get; set; }

        public ShoppingList()
        {
            Title = "liste";

        }
    }
}