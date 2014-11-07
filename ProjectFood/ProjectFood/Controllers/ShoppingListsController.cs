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
    public class ShoppingListsController : Controller
    {
        private ShoppingListContext db = new ShoppingListContext();

        // GET: ShoppingLists
        public ActionResult Index()
        {
            return View(db.ShoppingLists.Include(s => s.Items).ToList());
        }

        // GET: ShoppingLists/Details/5
        public ActionResult Details(int? id)
        {
            if(id == null) {
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingList shoppingList = findShoppingListFromID(id);

            ViewBag.ShoppingList_Item = db.ShoppingList_Item.Where(x => x.ShoppingListID == id).ToList();

            if(shoppingList == null) {
                return HttpNotFound();
            }
            foreach (var item in shoppingList.Items)
            {
                item.Offers = GetOffersForItem(item);
            }

            db.SaveChanges();

            return View(shoppingList);
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
                db.ShoppingLists.Add(shoppingList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(shoppingList);
        }

        // GET: ShoppingLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if(id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingList shoppingList = db.ShoppingLists.Find(id);
            if(shoppingList == null) {
                return HttpNotFound();
            }
            return View(shoppingList);
        }

        // POST: ShoppingLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title")] ShoppingList shoppingList)
        {
            if(ModelState.IsValid) {
                db.Entry(shoppingList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shoppingList);
        }

        // GET: ShoppingLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if(id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingList shoppingList = db.ShoppingLists.Find(id);
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
            ShoppingList shoppingList = db.ShoppingLists.Find(id);
            db.ShoppingLists.Remove(shoppingList);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult AddItem(int id, string name, double? amount, string unit)
        {
            if(name.Trim() == string.Empty) {
                // TODO: make a proper error message (Use: http://lipis.github.io/bootstrap-sweetalert/ ??)
                // Tilføj snackbar i else ?!
                return RedirectToAction("Details/" + id);
            }
            if(amount == null) {
                amount = 0;
            }
            ShoppingList shoppingList = findShoppingListFromID(id);
            Item tmpItem;

            //Search in GenericLItems for item
            Item knownItem = null;
            if(db.Items.Count() > 0)
                knownItem = db.Items.Where(i => i.Name.CompareTo(name) == 0).SingleOrDefault();


            if(knownItem != null) {
                tmpItem = knownItem;
            } else {
                tmpItem = new Item() { Name = name };
            }

            if(shoppingList.Items.Contains(tmpItem)) {
                db.ShoppingList_Item.Where(x => x.ItemID == tmpItem.ID && x.ShoppingListID == id).Single().Amount += (double)amount;
            } else {
                var shoppingListItem = new ShoppingList_Item { Item = tmpItem, ShoppingList = shoppingList, Amount = (double)amount, Unit = unit };

                db.ShoppingList_Item.Add(shoppingListItem);

                shoppingList.Items.Add(tmpItem);
            }

            db.SaveChanges();
            return RedirectToAction("Details/" + id);
        }

        public ActionResult RemoveItem(int id, int itemID)
        {

            ShoppingList shoppingList = findShoppingListFromID(id);

            //Find the item to be deleted, and remove it from the shopping list
            var rmItem = shoppingList.Items.ToList().Find(x => x.ID == itemID);
            shoppingList.Items.Remove(rmItem);

            //Find the item in the ShoppingList_Item table
            var rmShoppingListItem = db.ShoppingList_Item
                .Where(x => x.ItemID == itemID
                && x.ShoppingListID == id)
                .SingleOrDefault();
            //... and remove it
            if(rmShoppingListItem != null)
                db.ShoppingList_Item.Remove(rmShoppingListItem);

            //Save the changes in the database
            db.SaveChanges();

            //Update the users view of the shoppinglist
            return RedirectToAction("Details/" + id);
        }

        public ActionResult ClearShoppingList(int id)
        {
            var tmp = db.ShoppingLists.Include(s => s.Items).ToList();
            ShoppingList shoppingList = tmp.Find(x => x.ID == id);

            shoppingList.Items.Clear();

            db.SaveChanges();

            return RedirectToAction("Details/" + id);
        }

        private List<Offer> GetOffersForItem(Item item)
        {
            return db.Offers.Where(x => x.Heading.ToLower().Contains(item.Name.ToLower())).ToList();
        }

        //Find relevant shoppingList and include the items
        private ShoppingList findShoppingListFromID(int? id)
        {
            return db.ShoppingLists.Include(s => s.Items.Select(x => x.Offers)).ToList().Find(x => x.ID == id);
        } 

        public ActionResult ChooseOffer(int shoppingListId, int ItemId, int offerId)
        {

            ShoppingList list = findShoppingListFromID(shoppingListId);
            var item = list.Items.First(x => x.ID == ItemId);

            db.ShoppingList_Item.First(x => x.ItemID == ItemId).selectedOffer = db.Offers.First(x => x.ID == offerId);
            db.SaveChanges();

            return RedirectToAction("Details/" + shoppingListId);

        }
        
    }
}
