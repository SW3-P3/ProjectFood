using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    public class ShoppingList
    {
        public string Title { get; set; }
        public List<Item> Items { get; set; }
        public List<Item> BoughtItems { get; set; }
    }
}