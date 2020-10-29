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
    public class UnitController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /Unit/
        [SessionExpire]
        [CustomAuthorize(Roles = "Unit/CanRead")]        
        public ActionResult Index()
        {
            var units = db.Units.Include(u => u.PersonalDetail).Include(u => u.PersonalDetail1).OrderBy(u => u.Name);
            return View(units.ToList());
        }

        // GET: /Unit/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "Unit/CanRead")]        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unit unit = db.Units.Find(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            return View(unit);
        }

        // GET: /Unit/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "Unit/CanWrite")]        
        public ActionResult Create()
        {
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            return View();
        }

        // POST: /Unit/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Unit/CanWrite")]        
        public ActionResult Create([Bind(Include="ID,Name,IsActive")] Unit unit)
        {

            if (db.Units.Where(x => x.Name == unit.Name).Count() > 0)
            {
                ViewBag.Messaage = "Unit Name is Already Exist..!!";
                return View(unit);
            }


            unit.CreateDate = DateTime.UtcNow.AddHours(5.30);
            unit.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
            unit.NetworkIP = CommonFunctions.GetClientIP();

            if (ModelState.IsValid)
            {
                db.Units.Add(unit);
                db.SaveChanges();
                //return View(unit);
                ViewBag.Messaage = "Unit Name Inserted Successfully..!!";
            }
            return View(unit);
        }

        // GET: /Unit/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "Unit/CanWrite")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unit unit = db.Units.Find(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", unit.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", unit.ModifyBy);
            return View(unit);
        }

        // POST: /Unit/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Unit/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,Name,IsActive")] Unit unit)
        {

            if (db.Units.Where(x => x.Name == unit.Name && x.ID != unit.ID).Count() > 0)
            {
                ViewBag.Messaage = "unit name is already Exist..!!";
                return View(unit);
            }

            Unit lunit = db.Units.Find(unit.ID);
            unit.CreateDate = lunit.CreateDate;
            unit.CreateBy = lunit.CreateBy;
            unit.ModifyDate = DateTime.UtcNow.AddHours(5.5);
            unit.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
            unit.NetworkIP = CommonFunctions.GetClientIP();
            unit.DeviceType = string.Empty;
            unit.DeviceID = string.Empty;

            if (ModelState.IsValid)
            {
                //db.Entry(unit).State = EntityState.Modified;
                //db.SaveChanges();
                //return RedirectToAction("Index");
                db.Entry(lunit).CurrentValues.SetValues(unit);
                db.SaveChanges();
                ViewBag.Messaage = "unit Detail Modified Successfully";
            }
            
            
            return View(unit);
        }

        // GET: /Unit/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "Unit/CanDelete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unit unit = db.Units.Find(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            return View(unit);
        }

        // POST: /Unit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Unit/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Unit unit = db.Units.Find(id);
            db.Units.Remove(unit);
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
