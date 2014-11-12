using ProjectFood.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectFood.Models;
using ProjectFood.Models.Api;

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
                var toAdd = pref.Trim().Split(',');
                if (db.Users.First(x => x.Username == User.Identity.Name).Preferences == null)
                    db.Users.First(x => x.Username == User.Identity.Name).Preferences = new List<string>();
                foreach (var s in toAdd)
                {
                    db.Users.First(x => x.Username == User.Identity.Name).Preferences.Add(s);
                }

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