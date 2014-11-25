using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Web;
using System.Web.Mvc;
using ProjectFood.Models;
using ProjectFood.Models.Api;

namespace ProjectFood.Controllers
{
    public class ShoppingListsController : Controller
    {
        private readonly DataBaseContext _db = new DataBaseContext();

        // GET: ShoppingLists
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                Session["ScreenName"] = _db.Users.First(u => u.Username == User.Identity.Name).Name;
                ViewBag.NumItems = _db.ShoppingLists.Include(s => s.Items);
                return View(_db.Users.Include(s => s.ShoppingLists).First(u => u.Username == User.Identity.Name).ShoppingLists);
            }

            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action()});
        }

        // GET: ShoppingLists/Details/5
        public ActionResult Details(int? id)
        {           

            if(id == null) {
                return RedirectToAction("Index");
            }
            ShoppingList shoppingList = findShoppingListFromID(id);

            ViewBag.ShoppingList_Item = _db.ShoppingList_Item.Where(x => x.ShoppingListID == id).ToList();

            if(shoppingList == null) {
                return HttpNotFound();
            }

            foreach (var item in shoppingList.Items)
            {
                item.Offers = GetOffersForItem(item).OrderBy(x=>x.Store).ToList();
            }

            _db.SaveChanges();

            if (User.Identity.IsAuthenticated &&
                _db.Users
                .Include(s => s.ShoppingLists)
                .First(u => u.Username == User.Identity.Name)
                .ShoppingLists.Exists(s => s.ID == id))
            {
                return View(shoppingList);
            }
                return RedirectToAction("Index");
        }

        // GET: ShoppingLists/Create
        public PartialViewResult Create()
        {
            return PartialView("_CreateShoppingList");
        }

        // POST: ShoppingLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title")] ShoppingList shoppingList)
        {
            if(ModelState.IsValid) {
                _db.ShoppingLists.Add(shoppingList);
                if (User.Identity.IsAuthenticated)
                {
                    _db.Users.Include(u => u.ShoppingLists).First(u => u.Username == User.Identity.Name).ShoppingLists.Add(shoppingList);
                }
               

                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(shoppingList);
        }

        // GET: ShoppingLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if(id == null) {
                return RedirectToAction("Index");
            }
            ShoppingList shoppingList = _db.ShoppingLists.Find(id);
            if(shoppingList == null) {
                return HttpNotFound();
            }


            if (User.Identity.IsAuthenticated &&
                _db.Users
                .Include(s => s.ShoppingLists)
                .First(u => u.Username == User.Identity.Name)
                .ShoppingLists.Exists(s => s.ID == id))
            {
                return View(shoppingList);
            }

            return RedirectToAction("Index");
        }

        // POST: ShoppingLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title")] ShoppingList shoppingList)
        {
            if(ModelState.IsValid) {
                _db.Entry(shoppingList).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shoppingList);
        }

        // GET: ShoppingLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if(id == null) {
                return RedirectToAction("Index");
            }
            ShoppingList shoppingList = _db.ShoppingLists.Find(id);
            if(shoppingList == null) {
                return HttpNotFound();
            }
            return View(shoppingList);
        }

        // POST: ShoppingLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ShoppingList shoppingList = _db.ShoppingLists.Find(id);
            _db.ShoppingLists.Remove(shoppingList);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing) {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult WatchList()
        {
            if(User.Identity.IsAuthenticated) {
                var user = _db.Users.Include(w => w.WatchList.Items.Select(i => i.Offers)).First(u => u.Username == User.Identity.Name);
                if(user.WatchList == null) {    
                    user.WatchList = new ShoppingList { Title = "watchList"};
                    user.RelevantOffers = new ShoppingList { Title = "relevantOffers" };

                    _db.SaveChanges();
                }

                if (user.RelevantOffers == null)
                {
                    user.RelevantOffers = new ShoppingList { Title = "relevantOffers" };
                }

                foreach (var item in user.WatchList.Items)
                {
                    item.Offers = GetOffersForItem(item).OrderBy(x => x.Store).ToList();
                }

                return View(user.WatchList);
            }

            return RedirectToAction("Index");
        }

        public ActionResult AddItem(int id, string name, double? amount, string unit)
        {
            if(name.Trim() == string.Empty) {
                return RedirectToAction("Details/" + id);
            }
            if(amount == null) {
                amount = 0;
            }
            ShoppingList shoppingList = findShoppingListFromID(id);
            Item tmpItem;

            //Search in GenericLItems for item
            Item knownItem = null;
            if(_db.Items.Any()) { 
                knownItem = _db.Items.SingleOrDefault(i => i.Name.CompareTo(name) == 0);
            }

            if(knownItem != null) {
                tmpItem = knownItem;
            } else {
                tmpItem = new Item() { Name = name };
            }

            if(shoppingList.Items.Contains(tmpItem)) {
                if(isOfferSelectedOnItem(id, tmpItem)) {
                    Item sameNameItem = new Item() { Name = name }; 
                    addToShoppingList_Item(sameNameItem, shoppingList, amount, unit);
                } else { 
                    _db.ShoppingList_Item.Single(x => x.ItemID == tmpItem.ID && x.ShoppingListID == id).Amount += (double)amount;
                    _db.SaveChanges();
                }
            } else {
                addToShoppingList_Item(tmpItem, shoppingList, amount, unit);
            }

            return shoppingList.Title == "watchList" ? RedirectToAction("WatchList") : RedirectToAction("Details/" + id);
        }

        public ActionResult RemoveItem(int id, int itemID)
        {

            ShoppingList shoppingList = findShoppingListFromID(id);

            //Find the item to be deleted, and remove it from the shopping list
            var rmItem = shoppingList.Items.ToList().Find(x => x.ID == itemID);
            shoppingList.Items.Remove(rmItem);

            //Find the item in the ShoppingList_Item table
            var rmShoppingListItem = _db.ShoppingList_Item.SingleOrDefault(x => x.ItemID == itemID && x.ShoppingListID == id);
            //... and remove it
            if(rmShoppingListItem != null)
                _db.ShoppingList_Item.Remove(rmShoppingListItem);

            //Save the changes in the database
            _db.SaveChanges();

            //Update the users view of the shoppinglist
            return shoppingList.Title == "watchList" ? RedirectToAction("WatchList") : RedirectToAction("Details/" + id);
        }

        public ActionResult ClearShoppingList(int id)
        {
            ShoppingList shoppingList = findShoppingListFromID(id);

            shoppingList.Items.Clear();

            _db.SaveChanges();

            return shoppingList.Title == "watchList" ? RedirectToAction("WatchList") : RedirectToAction("Details/" + id);
        }

        [HttpPost]
        public ActionResult MoveItemToBought(int id, int itemID)
        {
            var tmpBought = _db.ShoppingList_Item.First(i => i.ItemID == itemID && i.ShoppingListID == id);

            if(tmpBought != null) {
                tmpBought.Bought = true;
                _db.SaveChanges();
                return Json(new {
                    Message = "Hajtroels",
                    itemID = itemID,
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new {
                Message = "DID NOT TWERK",
                itemID = itemID,
            }, JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost]
        public ActionResult ToggleItemBought(int id, int itemID)
        {
            var tmpItem = _db.ShoppingList_Item.First(i => i.ItemID == itemID && i.ShoppingListID == id);

            if (tmpItem == null)
                return Json(new
                {
                    Message = "DID NOT TWERK",
                    itemID = itemID,
                }, JsonRequestBehavior.AllowGet);

            tmpItem.Bought = tmpItem.Bought == false;

            _db.SaveChanges();
            return Json(new {
                Message = "Hajtroels",
                itemID = itemID,
            }, JsonRequestBehavior.AllowGet);
        }

        private List<Offer> GetOffersForItem(Item item)
        {
            return _db.Offers
                .Where(x => x.Heading.ToLower().Contains(item.Name.ToLower() + " ") || x.Heading.ToLower().Contains(" " + item.Name.ToLower()))
                .ToList();
        }
        [HttpPost]
        public ActionResult GetOffersForItem(int id)
        {
            var item = _db.Items.Find(id);
            var offers = _db.Offers
                .Where(x => x.Heading.ToLower().Contains(item.Name.ToLower() + " ") || x.Heading.ToLower().Contains(" " + item.Name.ToLower()))
                .ToList();

            var jsonSerialiser = new JavaScriptSerializer();
            var jsonOffers = jsonSerialiser.Serialize(offers);

            return Json(new { itemName = item.Name ,jsonOffers }, JsonRequestBehavior.DenyGet);
        }

        //Find relevant shoppingList and include the items
        private ShoppingList findShoppingListFromID(int? id)
        {
            return _db.ShoppingLists.Include(s => s.Items.Select(x => x.Offers)).ToList().Find(x => x.ID == id);
        } 

        public ActionResult ChooseOffer(int shoppingListId, int ItemId, int offerId)
        {

            ShoppingList list = findShoppingListFromID(shoppingListId);
            var item = list.Items.First(x => x.ID == ItemId);

            _db.ShoppingList_Item.First(x => x.ItemID == ItemId).selectedOffer = _db.Offers.First(x => x.ID == offerId);
            _db.SaveChanges();

            return RedirectToAction("Details/" + shoppingListId);

        }

        private void addToShoppingList_Item(Item item, ShoppingList shoppingList, double? amount, string unit)
        {
            shoppingList.Items.Add(item);
            var shoppingListItem = new ShoppingList_Item { Item = item, ShoppingList = shoppingList, Amount = (double)amount, Unit = unit };
            _db.ShoppingList_Item.Add(shoppingListItem);

            _db.SaveChanges();
        }

        private bool isOfferSelectedOnItem(int id, Item item){
            return _db.ShoppingList_Item.Single(x => x.ItemID == item.ID && x.ShoppingListID == id).selectedOffer != null;
        }
        
    }
}
