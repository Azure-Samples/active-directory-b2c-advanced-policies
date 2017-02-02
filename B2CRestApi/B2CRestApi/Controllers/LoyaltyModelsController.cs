using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using B2CRestApi.Data;
using B2CRestApi.Models;

namespace B2CRestApi.Controllers
{
    public class LoyaltyModelsController : Controller
    {
        private LoyaltyDbContext db = new LoyaltyDbContext();

        // GET: LoyaltyModels
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: LoyaltyModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoyaltyModel loyaltyModel = db.Users.Find(id);
            if (loyaltyModel == null)
            {
                return HttpNotFound();
            }
            return View(loyaltyModel);
        }

        // GET: LoyaltyModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LoyaltyModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,LoyaltyNumber")] LoyaltyModel loyaltyModel)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(loyaltyModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(loyaltyModel);
        }

        // GET: LoyaltyModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoyaltyModel loyaltyModel = db.Users.Find(id);
            if (loyaltyModel == null)
            {
                return HttpNotFound();
            }
            return View(loyaltyModel);
        }

        // POST: LoyaltyModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,LoyaltyNumber")] LoyaltyModel loyaltyModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loyaltyModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(loyaltyModel);
        }

        // GET: LoyaltyModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoyaltyModel loyaltyModel = db.Users.Find(id);
            if (loyaltyModel == null)
            {
                return HttpNotFound();
            }
            return View(loyaltyModel);
        }

        // POST: LoyaltyModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LoyaltyModel loyaltyModel = db.Users.Find(id);
            db.Users.Remove(loyaltyModel);
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
    }
}
