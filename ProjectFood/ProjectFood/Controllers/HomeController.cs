using ProjectFood.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;


namespace ProjectFood.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataBaseContext _db = new DataBaseContext();

        public ActionResult Index()
        {

            if(User.Identity.IsAuthenticated) {
                var user = _db.Users
                    .Include(w => w.WatchList.Items.Select(i => i.Offers))
                    .Include(u => u.ShoppingLists.Select(s => s.Items))
                    .First(u => u.Username == User.Identity.Name);

                foreach(var item in user.WatchList.Items) {
                    item.Offers = ShoppingListsController.GetOffersForItem(_db, item).OrderBy(x => x.Store).ToList();
                }
                ViewBag.WatchList = user.WatchList;
                ViewBag.ShoppingLists = user.ShoppingLists;
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}