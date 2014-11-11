using ProjectFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

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
                return View(db.Users.SingleOrDefault(u => u.Username == usernameDecode));
            }
            return RedirectToAction("Index");
        }

        public ActionResult AddPreference(string username, string pref)
        {
            if (User.Identity.IsAuthenticated && User.Identity.Name == username)
            {
                db.Users.First(u => u.Name == username).Preferences.Add(pref);

                db.SaveChanges();
                return RedirectToAction("EditPreferences", new { username });
            }
            return RedirectToAction("Index");
        }
        public ActionResult RemovePreference(string username, string pref)
        {
            if (User.Identity.IsAuthenticated && User.Identity.Name == username)
            {
                var user = db.Users.SingleOrDefault(u => u.Username == username);
                
                user.Preferences.Remove(pref);

                db.SaveChanges();
                return RedirectToAction("EditPreferences", new { username });
            }
            return RedirectToAction("Index");
        }

    }
}