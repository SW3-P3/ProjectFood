using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFood.Models
{
    public interface IDataBaseContext : IDisposable
    {
        DbSet<ShoppingList> ShoppingLists { get; }
        DbSet<Item> Items { get; }
        DbSet<Offer> Offers { get; }
        DbSet<ShoppingList_Item> ShoppingList_Item { get; }
        DbSet<Recipe> Recipes { get; }
        DbSet<Recipe_Ingredient> Recipe_Ingredient { get;  }
        DbSet<User> Users { get; }
        DbSet<Pref> Preferences { get; }
        DbSet<Rating> Ratings { get; }

        int SaveChanges();
        void MarkAsModified(ShoppingList item);

        IEnumerable<Offer> OffersFilteredWithString(params string[] args);
        IEnumerable<Offer> OffersFilteredByUserPrefs(User u);
        IEnumerable<Offer> OffersFiltered();

        void MarkAsModified(Recipe item);
    }
}
