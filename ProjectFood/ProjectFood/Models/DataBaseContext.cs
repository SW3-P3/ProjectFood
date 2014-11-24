using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProjectFood.Models
{
    public class DataBaseContext : DbContext
    {
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<ShoppingList_Item> ShoppingList_Item { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Recipe_Ingredient> Recipe_Ingredient { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Pref> Preferences { get; set; }


        public IEnumerable<Offer> OffersFiltered()
        {
            return OffersFilteredWithString();
        }

        public IEnumerable<Offer> OffersFilteredByUserPrefs(User u)
        {
            var prefs = Users.Include(a => a.Preferences).First(x => x.Username == u.Username).Preferences;
            var storesBlackListed = prefs.Select(x => x.Value).ToArray();
            return OffersFilteredWithString(storesBlackListed);
        }

        public IEnumerable<Offer> OffersFilteredWithString(params string[] args)
        {
            var blacklist = new List<string> { ",", "eller" };

            var fromArgs = new List<string>();
            foreach (var str in args)
            {
                fromArgs.AddRange(str.Split(','));
            }
            blacklist.AddRange(fromArgs);
            // If an empty strings if any was given
            blacklist.RemoveAll(x => x.Trim().Equals(string.Empty));
            
            var res = new List<Offer>();

            foreach (var o in Offers)
            {
                bool flag = !(o.End < DateTime.Now);

                foreach (var item in blacklist)
                {
                    if (o.Heading.ToLower().Contains(item.ToLower()) || o.Store.ToLower().Contains(item.ToLower()))
                        flag = false;
                }

                if (flag && o.Unit.Trim() != "")
                    res.Add(o);
            }
            return res;
        }

        public DataBaseContext()
            : base("Food")
        {
        }

/*        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        //    modelBuilder.Entity<ShoppingList>().
        //      HasMany(c => c.Items).
        //      WithMany(p => p.OnLists).
        //      Map(
        //       m =>
        //       {
        //           m.MapLeftKey("ShoppingListID");
        //           m.MapRightKey("ItemID");
        //           m.ToTable("ShoppingList_Items");
        //       });

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
        }*/
    }
}