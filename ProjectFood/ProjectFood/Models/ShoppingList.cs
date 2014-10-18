using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace ProjectFood.Models
{
    public class ShoppingList
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public List<Item> Items = new List<Item>();
        public List<Item> BoughtItems = new List<Item>();

        public ShoppingList()
        {
            Title = "liste";

        }

    }
    public class ShoppingListDBContext : DbContext
    {
        public DbSet<ShoppingList> ShoppingLists { get; set; }
    }
}