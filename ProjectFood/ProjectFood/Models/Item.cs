﻿using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    public class Item
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public ICollection<ShoppingList> OnLists { get; set; }

        public ICollection<Offer> Offers { get; set; }
        public ICollection<Recipe> InRecipes { get; set; }

        public Item()
        {
            OnLists = new List<ShoppingList>();
            Offers = new List<Offer>();
            InRecipes = new List<Recipe>();
        }
    }
}