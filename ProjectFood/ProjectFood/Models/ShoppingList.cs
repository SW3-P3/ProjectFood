using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace ProjectFood.Models
{
    public class ShoppingList
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public List<Item> Items { get; set;}

        public ShoppingList()
        {
            Items = new List<Item>();
            //Items.Add(new Item("Havregryn"));
        }

    }
    public class ShoppingListDBContext : DbContext
    {
        public DbSet<ShoppingList> ShoppingLists { get; set; }
    }
}