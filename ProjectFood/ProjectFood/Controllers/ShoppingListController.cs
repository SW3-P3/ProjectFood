using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectFood.Models;

namespace ProjectFood.Controllers
{
    public class ShoppingListController : Controller
    {
        
        private ShoppingList shoppinglist = new ShoppingList();

        // GET: /ShoppingList/
        public ActionResult Index()
        {   Item item = new Item("Torsk");
            shoppinglist.Items.Add(item);
            Item item2 = new Item("Torsk");
            shoppinglist.Items.Add(item2);
            Item item3 = new Item("Torsk");
            shoppinglist.Items.Add(item3);
            Item item4 = new Item("Torsk");
            shoppinglist.Items.Add(item4);
            return View(shoppinglist);
        }

        public void AddItem(string nameOfItem)
        {
            Item item = new Item(nameOfItem);
            shoppinglist.Items.Add(item);
            Index();
        }
	}
}