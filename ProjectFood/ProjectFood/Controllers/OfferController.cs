using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using ProjectFood.Models;
using ProjectFood.Models.Api;
using RestSharp;
using System.Data.Entity;
using System.Net.Mail;


namespace ProjectFood.Controllers
{
    public class OfferController : Controller
    {

        private readonly DataBaseContext _db = new DataBaseContext();

        // GET: Offer
        public ActionResult Index(int? shoppingListID)
        {
            if (User.Identity.IsAuthenticated)
            {
                var tmpUser = _db.Users
                    .Include(s => s.ShoppingLists)
                    .First(u => u.Username == User.Identity.Name);
                if(tmpUser.ShoppingLists.Count > 0) {
                
                int tmpShoppingListID = shoppingListID == null ? tmpUser.ShoppingLists.First().ID : (int)shoppingListID;
                ViewBag.SelectedShoppingListID = tmpShoppingListID;
                
                ViewBag.ShoppingLists = tmpUser.ShoppingLists.ToList();
                ViewBag.OffersOnListByID = _db.ShoppingList_Item
                    .Where(s => s.ShoppingListID == tmpShoppingListID && s.selectedOffer != null)
                    .Select<ShoppingList_Item, int>(o => o.selectedOffer.ID);           
                }
                ViewBag.Stores = _db
                    .OffersFilteredByUserPrefs(_db.Users.First(u => u.Username == User.Identity.Name))
                    .OrderBy(d => d.Store)
                    .Select(x => x.Store)
                    .Distinct();
                Session["ScreenName"] = _db.Users.First(u => u.Username == User.Identity.Name).Name;
                ViewBag.Stores = _db.OffersFilteredByUserPrefs(_db.Users.First(u => u.Username == User.Identity.Name)).OrderBy(d => d.Store).Select(x => x.Store).Distinct();
                ViewBag.ShoppingLists = _db.Users.Include(s => s.ShoppingLists).First(u => u.Username == User.Identity.Name).ShoppingLists.ToList();

                NotifyWatchers(_db.Offers.ToList());

                return View(_db.OffersFiltered().ToList());            
            }
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action() });
        }

        public ActionResult ImportOffers()
        {
            string apikey, secret;
            /* Write Apikey and Secret to global variables */
            try
            {
                apikey = System.IO.File.ReadAllText(@"C:\apikey.secret");
                secret = System.IO.File.ReadAllText(@"C:\secret.secret");
            }
            catch (IOException ex)
            {
                Console.WriteLine("File not found. " + ex);
                throw;
            }

            /* Create a RestClient*/
            var client = new RestClient("https://api.etilbudsavis.dk");

            /* Initiate the first request to get a session */
            var sessionRequest = new RestRequest("v2/sessions", Method.POST);
            sessionRequest.AddParameter("api_key", apikey);

            /* Map the response to the class "Session" */
            IRestResponse<Session> response2 = client.Execute<Session>(sessionRequest);

            /* Save the response in an object */
            var sessionobj = response2.Data;

            /* Load aditional infomation into the object */
            sessionobj.Secret = secret;
            sessionobj.Apikey = apikey;
            sessionobj.Signature = Models.Api.Session.Sha256(sessionobj.Secret + sessionobj.Token);
            
            /* Copy the sessionobj to a globalSession */
            Global.Session = sessionobj;

            /* Offers request (Every offer from the catalog found above */
            var offersRequest = new RestRequest("v2/offers", Method.GET);
            offersRequest.AddParameter("r_lat", Global.Latitude);
            offersRequest.AddParameter("r_lng", Global.Longitude);
            offersRequest.AddParameter("r_radius", Global.Radius);
            offersRequest.AddHeader("X-Token", Global.Session.Token);
            offersRequest.AddHeader("X-Signature", Global.Session.Signature);
            //offersRequest.AddParameter("catalog_ids", string.Join(",", catalogsResult.Select(x => x.id)));
            offersRequest.AddParameter("limit", "100");
            offersRequest.AddParameter("dealer_ids", "9ba51,8c4da,bdf5A,11deC,c1edq,71c90,101cD,0b1e8,ecddz,8c4da,1e1eB"); //8c4da,bdf5A,11deC,c1edq,71c90,101cD,0b1e8,ecddz,8c4da,1e1eB,d311fg

            //TODO: This should be converted to an Async method for each store if we need a speed-up
            List<ApiOffer> offersResult = client.Execute<List<ApiOffer>>(offersRequest).Data;
            List<ApiOffer> listofApiOffers = offersResult;
            while (offersResult.Count == 100)
            {
                var nextOffersRequest = new RestRequest("v2/offers", Method.GET);
                nextOffersRequest.AddParameter("r_lat", Global.Latitude);
                nextOffersRequest.AddParameter("r_lng", Global.Longitude);
                nextOffersRequest.AddParameter("r_radius", Global.Radius);
                nextOffersRequest.AddHeader("X-Token", Global.Session.Token);
                nextOffersRequest.AddHeader("X-Signature", Global.Session.Signature);
                nextOffersRequest.AddParameter("limit", "100");
                nextOffersRequest.AddParameter("dealer_ids", "9ba51,8c4da,bdf5A,11deC,c1edq,71c90,101cD,0b1e8,ecddz,8c4da,1e1eB");
                nextOffersRequest.AddParameter("offset", listofApiOffers.Count);
                offersResult = client.Execute<List<ApiOffer>>(nextOffersRequest).Data;
                listofApiOffers.AddRange(offersResult);
            }

            foreach( var a in _db.Offers)
            {
                _db.Offers.Remove(a);
            }

            foreach (var o in listofApiOffers)
            {
                _db.Offers.Add(new Offer
                {
                    Heading = o.heading,
                    Begin = o.run_from,
                    End = o.run_till,
                    Store = o.branding.name,
                    Price = o.pricing.price,
                    Unit = o.quantity.unit != null ? o.quantity.size.@from + " " + o.quantity.unit.symbol : " "
                });
            }

            NotifyWatchers(_db.Offers.ToList());

            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddOfferToShoppingList(int offerId, int? shoppingListId)
        {
            var tmpOffer = _db.Offers.Find(offerId);

            var tmpItem = new Item {Name = tmpOffer.Heading};
            tmpItem.Offers.Add(tmpOffer);

            var shoppingList = _db.ShoppingLists.First(l => l.ID == shoppingListId);

            var shoppingListItem = new ShoppingList_Item { 
                Item = tmpItem, 
                ShoppingList = shoppingList, 
                Amount = 0, 
                Unit = tmpOffer.Unit,
                selectedOffer = tmpOffer};

            _db.ShoppingList_Item.Add(shoppingListItem);

            shoppingList.Items.Add(tmpItem);

            _db.SaveChanges();

            return Json(new
            {
                Message = "Hajtroels",
                OfferId = offerId,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(string id)
        {
            if(id != null) 
            {
                ViewBag.Offers = GetOffersForItem(id);
                ViewBag.SearchTerm = id;
            }
            else
            {
                ViewBag.Offers = new List<Offer>();
            }
            
            ViewBag.ShoppingLists = _db.Users
                .Include(s => s.ShoppingLists)
                .First(u => u.Username == User.Identity.Name)
                .ShoppingLists
                .ToList();

            return View();
        }

        public ActionResult SearchDone(string id)
        {
            return RedirectToAction("Search/" + id);
        }

        private List<Offer> GetOffersForItem(string str)
        {
            return _db.OffersFilteredByUserPrefs(_db.Users.FirstOrDefault(x => x.Username.Equals(User.Identity.Name)))
                .Where(x => x.Heading.ToLower().Contains(str.ToLower()))
                .ToList();
        }

        private void NotifyWatchers(List<Offer> offers)
        {
            var users = _db.Users.Include(w => w.WatchList.Items).Where(u => u.WatchList != null && u.WatchList.Items.Count > 0).ToArray();

            foreach (var user in users)
            {
                List<Offer> relevantOffers = new List<Offer>();
                foreach (var item in user.WatchList.Items)
                {
                    relevantOffers.AddRange(GetOffersForItem(item));
                }   
            }
        }

        private List<Offer> GetOffersForItem(Item item)
        {
            return _db.Offers
                .Where(x => x.Heading.ToLower().Contains(item.Name.ToLower() + " ") || x.Heading.ToLower().Contains(" " + item.Name.ToLower()))
                .ToList();
        }
    }
}