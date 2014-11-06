using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectFood.Models
{
    public class ShoppingList_Item
    {
        [Key, Column(Order = 0)]
        public int ShoppingListID { get; set; }
        [Key, Column(Order = 1)]
        public int ItemID { get; set; }

        public virtual ShoppingList ShoppingList { get; set; }
        public virtual Item Item { get; set; }
        public virtual Offer selectedOffer { get; set; }

        public double Amount { get; set; }
        public string Unit { get; set; }
    }
}