using ProjectFood.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;



namespace ProjectFood.Controllers
{
    public class UserController : Controller
    {
        private IDataBaseContext _db = new DataBaseContext();

        public UserController() { }

        public UserController(IDataBaseContext context)
        {
            _db = context;
        }

        // GET: User
        public ActionResult Index()
        {
            if(User.Identity.IsAuthenticated) {
                return View(User);
            }

            return RedirectToAction("Index", "Home");
        }
        [ValidateAntiForgeryToken]
        public ActionResult EditName(string username, string name)
        {
            if(User.Identity.IsAuthenticated && User.Identity.Name == username && name.Split(',').First().Trim() != string.Empty) {
                if(!IsNameLegal(name)) {
                    return RedirectToAction("EditPreferences", new { BadName = name.Trim() });
                }

                _db.Users.FirstOrDefault(u => u.Username == username).Name = (name.Contains(",") == true ? name.Split(',').First() : name);
                _db.SaveChanges();
            }
            return RedirectToAction("EditPreferences");
        }


        public ActionResult EditPreferences(string BadName)
        {
            if(User.Identity.IsAuthenticated) {
                ViewBag.Store = _db.Offers.Select(x => x.Store).Distinct().OrderBy(x => x.ToLower()).ToList();
                ViewBag.Prefs = _db.Preferences.ToList();
                ViewBag.BadWord = BadName;
                return View(_db.Users.Include(s => s.Preferences).First(u => u.Username == User.Identity.Name));

            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult AddPreference(string username, string pref, bool store)
        {
            if(User.Identity.IsAuthenticated && User.Identity.Name == username) {
                var user = _db.Users.Include(u => u.Preferences).First(u => u.Username == username);
                var toAdd = pref.Trim().Split(',');
                foreach(var lePref in toAdd) {
                    user.Preferences.Add(new Pref { Value = lePref, Store = store });
                }

                _db.SaveChanges();
                return RedirectToAction(store ? "EditStores" : "EditPreferences", new { username });
            }
            return RedirectToAction("EditPreferences");
        }
        public ActionResult RemovePreference(string username, int prefId)
        {
            if(User.Identity.IsAuthenticated && User.Identity.Name == username) {
                var user = _db.Users.Include(u => u.Preferences).First(u => u.Username == username);
                var tmpPref = user.Preferences.First(p => p.ID == prefId);

                user.Preferences.Remove(tmpPref);

                _db.SaveChanges();
                return RedirectToAction(tmpPref.Store ? "EditStores" : "EditPreferences", new { username });
            }
            return RedirectToAction("EditPreferences");
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

        public static bool IsNameLegal(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ProjectFood.Content.bad-words.dat";

            using(Stream stream = assembly.GetManifestResourceStream(resourceName))
            using(StreamReader reader = new StreamReader(stream)) {
                var badWords = reader.ReadToEnd();
                foreach(string sub in name.Split(' ')) {
                    if(badWords.Contains(sub.Trim().ToLower())) {
                        return false;
                    }
                }
                foreach(string sub in name.Split(',')) {
                    if(badWords.Contains(sub.Trim().ToLower())) {
                        return false;
                    }
                }
                foreach(string sub in SplitCamelCase(name)) {
                    if(badWords.Contains(sub)) {
                        return false;
                    }
                }
                
            }

            return true;
        }

        private static string[] SplitCamelCase(string source)
        {
            return Regex.Split(source, @"(?<!^)(?=[A-Z])");
        }

    }
}