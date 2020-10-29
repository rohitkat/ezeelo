using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using BusinessLogicLayer;
using Administrator.Models;

namespace Administrator.Controllers
{
    public class SpecificationController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        [CustomAuthorize(Roles = "Specification/CanRead")]
        // GET: /Specification/
        public ActionResult Index()
        {
            var specifications = db.Specifications.Include(s => s.PersonalDetail).Include(s => s.PersonalDetail1).Include(s => s.Specification2);
            return View(specifications.ToList());
        }

        [CustomAuthorize(Roles = "Specification/CanRead")]
        // GET: /Specification/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Specification specification = db.Specifications.Find(id);
            if (specification == null)
            {
                return HttpNotFound();
            }
            return View(specification);
        }

        [CustomAuthorize(Roles = "Specification/CanWrite")]
        // GET: /Specification/Create
        public ActionResult Create()
        {
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ParentSpecificationID = new SelectList(db.Specifications, "ID", "Name");
            return View();
        }

        // POST: /Specification/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Specification/CanWrite")]
        public ActionResult Create([Bind(Include="ID,Name,ParentSpecificationID,Level,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] Specification specification)
        {
            specification.CreateDate = DateTime.UtcNow;
            specification.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
            specification.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            specification.IsActive = true;

            if (specification.ParentSpecificationID == null )
            {
                specification.Level = 1;
            }
            else
            {
                specification.Level = 2;
            }
            if (ModelState.IsValid)
            {
                db.Specifications.Add(specification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", specification.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", specification.ModifyBy);
            ViewBag.ParentSpecificationID = new SelectList(db.Specifications, "ID", "Name", specification.ParentSpecificationID);
            return View(specification);
        }

        // GET: /Specification/Edit/5
         [CustomAuthorize(Roles = "Specification/CanWrite")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Specification specification = db.Specifications.Find(id);
           
           
            if (specification == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", specification.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", specification.ModifyBy);
            ViewBag.ParentSpecificationID = new SelectList(db.Specifications, "ID", "Name", specification.ParentSpecificationID);
            return View(specification);
        }

        // POST: /Specification/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Specification/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,Name,ParentSpecificationID,Level,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] Specification specification)
        {
            EzeeloDBContext db1 = new EzeeloDBContext();
            Specification sp = db1.Specifications.Find(specification.ID);
            specification.CreateDate = sp.CreateDate;
            specification.CreateBy = sp.CreateBy;
            if (specification.ParentSpecificationID == null)
            {
                specification.Level = 1;
            }
            else
            {
                specification.Level = 2;
            }
            specification.ModifyDate = DateTime.UtcNow;
            specification.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
            specification.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            db1.Dispose();
            if (ModelState.IsValid)
            {
                db.Entry(specification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", specification.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", specification.ModifyBy);
            ViewBag.ParentSpecificationID = new SelectList(db.Specifications, "ID", "Name", specification.ParentSpecificationID);
            return View(specification);
        }

        // GET: /Specification/Delete/5
         [CustomAuthorize(Roles = "Specification/CanDelete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Specification specification = db.Specifications.Find(id);
            if (specification == null)
            {
                return HttpNotFound();
            }
            return View(specification);
        }

        // POST: /Specification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Specification/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Specification specification = db.Specifications.Find(id);
            db.Specifications.Remove(specification);
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
