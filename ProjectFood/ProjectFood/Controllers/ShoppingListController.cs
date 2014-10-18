using System.Web.Mvc;
using ProjectFood.Models;

namespace ProjectFood.Controllers
{
    public class ShoppingListController : Controller
    {
        // GET: /ShoppingList/
        public ActionResult Index()
        {   
            return View(Global.GlobalShoppingList);
        }

        public ActionResult Add(string name)
        {
            Item item = new Item(name);
            Global.GlobalShoppingList.Items.Add(item);
            return RedirectToAction("Index", "ShoppingList");
        }

        public ActionResult Remove(string id)
        {

            Global.GlobalShoppingList.Items.RemoveAt(int.Parse(id));
            return RedirectToAction("Index", "ShoppingList");
        }
    }
}