using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectFood.Models;

namespace ProjectFood.Controllers
{
    public class ShoppingListsController : Controller
    {
        private ShoppingListContext db = new ShoppingListContext();

        // GET: ShoppingLists
        public ActionResult Index()
        {
            return View(db.ShoppingLists.ToList());
        }

        // GET: ShoppingLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tmp = db.ShoppingLists.Include(s => s.Items).ToList();
            ShoppingList shoppingList = tmp.Find(x => x.ID == id);
            
            if (shoppingList == null)
            {
                return HttpNotFound();
            }
            return View(shoppingList);
        }

        // GET: ShoppingLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShoppingLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title")] ShoppingList shoppingList)
        {
            if (ModelState.IsValid)
            {
                db.ShoppingLists.Add(shoppingList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(shoppingList);
        }

        // GET: ShoppingLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingList shoppingList = db.ShoppingLists.Find(id);
            if (shoppingList == null)
            {
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
            if (ModelState.IsValid)
            {
                db.Entry(shoppingList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shoppingList);
        }

        // GET: ShoppingLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingList shoppingList = db.ShoppingLists.Find(id);
            if (shoppingList == null)
            {
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
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult AddItem(int id, string name)
        {
            if (name.Trim() == string.Empty)
            {
                // TODO: make a proper error message (Use: http://lipis.github.io/bootstrap-sweetalert/ ??)
                // Tilføj snackbar i else ?!
                return RedirectToAction("Details/" + id);
            }
            Item tmp = new Item();
            tmp.Name = name;
            tmp.Category = "Diverse";

            ShoppingList shoppingList = db.ShoppingLists.Find(id);
            
            shoppingList.Items.Add(tmp);

            db.SaveChanges();
            return RedirectToAction("Details/" + id);
        }

        public ActionResult RemoveItem(int id, int itemID)
        {
            var tmp = db.ShoppingLists.Include(s => s.Items).ToList();
            ShoppingList shoppingList = tmp.Find(x => x.ID == id);

            var rmItem = shoppingList.Items.ToList().Find(x => x.ID == itemID);
            shoppingList.Items.Remove(rmItem);
            
            db.SaveChanges();

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
    }
}
