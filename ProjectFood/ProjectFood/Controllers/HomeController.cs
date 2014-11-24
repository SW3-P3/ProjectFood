using ProjectFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectFood.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataBaseContext _db = new DataBaseContext();

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                Session["ScreenName"] = _db.Users.First(u => u.Username == User.Identity.Name).Name;
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