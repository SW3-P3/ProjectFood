using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectFood.Controllers
{
    public class OfferController : Controller
    {
        // GET: Offer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ImportOffers()
        {


            return RedirectToAction("Index");
        }
    }
}