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
        public DbSet<Offer> Offers { get; set; }

        public ShoppingListContext() : base("Foodzz")
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

            modelBuilder.Entity<Item>().
             HasMany(i => i.Offers).
             WithMany(o => o.GenericItems).
             Map(
              m =>
              {
                  m.MapLeftKey("ItemID");
                  m.MapRightKey("OfferID");
                  m.ToTable("Items_Offers");
              });
        }
    }
}