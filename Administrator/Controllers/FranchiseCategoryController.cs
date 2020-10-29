using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using Administrator.Models;

namespace  Administrator.Controllers
{
    public class FranchiseCategoryController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /FranchiseCategory/
        [CustomAuthorize(Roles = "FranchiseCategory/CanRead")]
        public ActionResult Index()
        {
            var franchisecategories = db.FranchiseCategories.Include(f => f.Category).Include(f => f.Category1).Include(f => f.FranchiseLocation);
            return View(franchisecategories.ToList());
        }

        // GET: /FranchiseCategory/Details/5
        [CustomAuthorize(Roles = "FranchiseCategory/CanRead")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FranchiseCategory franchisecategory = db.FranchiseCategories.Find(id);
            if (franchisecategory == null)
            {
                return HttpNotFound();
            }
            return View(franchisecategory);
        }

        // GET: /FranchiseCategory/Create
        [CustomAuthorize(Roles = "FranchiseCategory/CanWrite")]
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            ViewBag.ParentCategoryId = new SelectList(db.Categories, "ID", "Name");
            ViewBag.FranchiseLocationID = new SelectList(db.FranchiseLocations, "ID", "NetworkIP");
            return View();
        }

        // POST: /FranchiseCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "FranchiseCategory/CanWrite")]
        public ActionResult Create([Bind(Include="ID,FranchiseLocationID,CategoryID,ParentCategoryId,Level,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] FranchiseCategory franchisecategory)
        {
            if (ModelState.IsValid)
            {
                db.FranchiseCategories.Add(franchisecategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", franchisecategory.CategoryID);
            ViewBag.ParentCategoryId = new SelectList(db.Categories, "ID", "Name", franchisecategory.ParentCategoryId);
            ViewBag.FranchiseLocationID = new SelectList(db.FranchiseLocations, "ID", "NetworkIP", franchisecategory.FranchiseLocationID);
            return View(franchisecategory);
        }

        // GET: /FranchiseCategory/Edit/5
        [CustomAuthorize(Roles = "FranchiseCategory/CanWrite")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FranchiseCategory franchisecategory = db.FranchiseCategories.Find(id);
            if (franchisecategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", franchisecategory.CategoryID);
            ViewBag.ParentCategoryId = new SelectList(db.Categories, "ID", "Name", franchisecategory.ParentCategoryId);
            ViewBag.FranchiseLocationID = new SelectList(db.FranchiseLocations, "ID", "NetworkIP", franchisecategory.FranchiseLocationID);
            return View(franchisecategory);
        }

        // POST: /FranchiseCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "FranchiseCategory/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,FranchiseLocationID,CategoryID,ParentCategoryId,Level,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] FranchiseCategory franchisecategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(franchisecategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", franchisecategory.CategoryID);
            ViewBag.ParentCategoryId = new SelectList(db.Categories, "ID", "Name", franchisecategory.ParentCategoryId);
            ViewBag.FranchiseLocationID = new SelectList(db.FranchiseLocations, "ID", "NetworkIP", franchisecategory.FranchiseLocationID);
            return View(franchisecategory);
        }

        // GET: /FranchiseCategory/Delete/5
        [CustomAuthorize(Roles = "FranchiseCategory/CanDelete")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FranchiseCategory franchisecategory = db.FranchiseCategories.Find(id);
            if (franchisecategory == null)
            {
                return HttpNotFound();
            }
            return View(franchisecategory);
        }

        // POST: /FranchiseCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "FranchiseCategory/CanDelete")]
        public ActionResult DeleteConfirmed(long id)
        {
            FranchiseCategory franchisecategory = db.FranchiseCategories.Find(id);
            db.FranchiseCategories.Remove(franchisecategory);
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
