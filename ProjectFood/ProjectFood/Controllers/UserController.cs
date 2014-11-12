using ProjectFood.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;



namespace ProjectFood.Controllers
{
    public class UserController : Controller
    {
        private ShoppingListContext db = new ShoppingListContext();

        // GET: User
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }
            return RedirectToAction("Index", "Home");
        }

        [ValidateAntiForgeryToken]
        public ActionResult EditName(string username, string name)
        {
            if (User.Identity.IsAuthenticated && User.Identity.Name == username)
            {
                db.Users.SingleOrDefault(u => u.Username == username).Name = name;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult EditPreferences(string username)
        {
            string usernameDecode = HttpUtility.HtmlDecode(username);
            if (User.Identity.IsAuthenticated && User.Identity.Name == usernameDecode)
            {
                return View(db.Users.Include(s => s.Preferences).First(u => u.Username == User.Identity.Name));
                
            }
            return RedirectToAction("Index");
        }

        public ActionResult AddPreference(string username, string pref)
        {
            if (User.Identity.IsAuthenticated && User.Identity.Name == username)
            {
                var user = db.Users.Include(u => u.Preferences).Single(u => u.Username == username);
                var toAdd = pref.Trim().Split(',');
                foreach (var lePref in toAdd)
                {
                    user.Preferences.Add(new Pref { value = lePref });
                }

                db.SaveChanges();
                return RedirectToAction("EditPreferences", new { username });
            }
            return RedirectToAction("Index");
        }
        public ActionResult RemovePreference(string username, int prefId)
        {
            if (User.Identity.IsAuthenticated && User.Identity.Name == username)
            {
                var user = db.Users.Include(u => u.Preferences).Single(u => u.Username == username);

                user.Preferences.Remove(user.Preferences.First(p => p.ID == prefId));

                db.SaveChanges();
                return RedirectToAction("EditPreferences", new { username });
            }
            return RedirectToAction("Index");
        }

    }
}