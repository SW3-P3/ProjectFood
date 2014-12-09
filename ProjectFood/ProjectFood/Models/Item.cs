using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace ProjectFood.Models
{
    public class Item
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ICollection<Offer> Offers { get; set; }

        public ICollection<Recipe> OnRecipes { get; set; }

        public ICollection<ShoppingList> OnShoppinglists { get; set; }


        public Item()
        {
            OnRecipes = new List<Recipe>();
            OnShoppinglists = new List<ShoppingList>();
            Offers = new List<Offer>();
        }
    }
}