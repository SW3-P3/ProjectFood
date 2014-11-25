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
        private readonly DataBaseContext _db = new DataBaseContext();

        // GET: User
        public ActionResult Index()
        {
            if(User.Identity.IsAuthenticated) {
                Session["ScreenName"] = _db.Users.First(u => u.Username == User.Identity.Name).Name;
                return RedirectToAction("Index", "Manage");
            }

            return RedirectToAction("Index", "Home");
        }

        [ValidateAntiForgeryToken]
        public ActionResult EditName(string username, string name)
        {
            if(User.Identity.IsAuthenticated && User.Identity.Name == username) {
                _db.Users.SingleOrDefault(u => u.Username == username).Name = name;
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult EditPreferences(string username)
        {
            var usernameDecode = HttpUtility.HtmlDecode(username);
            if(User.Identity.IsAuthenticated && User.Identity.Name == usernameDecode) {
                ViewBag.Prefs = _db.Preferences.ToList();
                return View(_db.Users.Include(s => s.Preferences).First(u => u.Username == User.Identity.Name));

            }
            return RedirectToAction("Index");
        }

        public ActionResult AddPreference(string username, string pref, bool store)
        {
            if(User.Identity.IsAuthenticated && User.Identity.Name == username) {
                var user = _db.Users.Include(u => u.Preferences).Single(u => u.Username == username);
                var toAdd = pref.Trim().Split(',');
                foreach(var lePref in toAdd) {
                    user.Preferences.Add(new Pref { Value = lePref, Store = store });
                }

                _db.SaveChanges();
                return RedirectToAction(store ? "EditStores" : "EditPreferences", new { username });
            }
            return RedirectToAction("Index");
        }
        public ActionResult RemovePreference(string username, int prefId)
        {
            if(User.Identity.IsAuthenticated && User.Identity.Name == username) {
                var user = _db.Users.Include(u => u.Preferences).Single(u => u.Username == username);
                var tmpPref = user.Preferences.First(p => p.ID == prefId);

                user.Preferences.Remove(tmpPref);

                _db.SaveChanges();
                return RedirectToAction(tmpPref.Store ? "EditStores" : "EditPreferences", new { username });
            }
            return RedirectToAction("Index");
        }

        public ActionResult EditStores(string username)
        {
            var usernameDecode = HttpUtility.HtmlDecode(username);
            if(User.Identity.IsAuthenticated && User.Identity.Name == usernameDecode) {

                ViewBag.Prefs = _db.Preferences.ToList();
                var tmpList = new List<string>();
                var tmp = _db.Offers;
                foreach(var offer in tmp) {
                   
                }
                ViewBag.Store = tmpList;

                return View(_db.Users.Include(s => s.Preferences).First(u => u.Username == User.Identity.Name));

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public void EditStore(string storename)
        {
            var tmpUser = _db.Users.Include(u => u.Preferences).First(u => u.Username == User.Identity.Name);

            if(tmpUser.Preferences.Any(x => x.Store == true && x.Value == storename)) {
                tmpUser.Preferences.Remove(tmpUser.Preferences.First(x => x.Store == true && x.Value == storename));
            } else {
                tmpUser.Preferences.Add(new Pref() { Store = true, Value = storename });
            }

            _db.SaveChanges();
        }

    }
}