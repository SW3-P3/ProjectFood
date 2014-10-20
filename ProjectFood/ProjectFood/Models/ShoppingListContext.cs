using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProjectFood.Models
{
    public class ShoppingListContext : DbContext
    {
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<Item> Items { get; set; }

        public ShoppingListContext() : base("Foodz")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShoppingList>().
              HasMany(c => c.Items).
              WithMany(p => p.OnLists).
              Map(
               m =>
               {
                   m.MapLeftKey("ShoppingListID");
                   m.MapRightKey("ItemID");
                   m.ToTable("ShoppingList_Items");
               });
        }
    }
}