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
using System.IO;
using Administrator.Models;

namespace Administrator.Controllers
{
    public class ChargeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        private int fUserId = -1;
        private int fUserTypeId = -1;
        private int pageSize = 10;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            /* This Methode Initialize feild fUserId and fUserTypeId
             * to current logged user's UserID and UserTypeID  
             */
            fUserId = 1;
            fUserTypeId = 1;
            if (requestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                fUserId = 1;
                fUserTypeId = 1;
            }
        }

        // GET: /Charge/
        [SessionExpire]
        [CustomAuthorize(Roles = "Charge/CanRead")]
        public ActionResult Index(int? chargeStageID)
        
        {    
            var lCharge = db.Charges.Include(c => c.ChargeStage);

            if (chargeStageID != null)
            {
                lCharge = lCharge.Where(e => e.ChargeStageID == chargeStageID);
            }
            return View(lCharge.ToList());

            //var charges = db.Charges.Include(c => c.ChargeStage).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1);
            //return View(charges.ToList());
        }

        // GET: /Charge/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "Charge/CanRead")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Charge charge = db.Charges.Find(id);
            if (charge == null)
            {
                return HttpNotFound();
            }
            return View(charge);
        }

        // GET: /Charge/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "Charge/CanWrite")]
        public ActionResult Create()
        {
            ViewBag.ChargeStageID = new SelectList(db.ChargeStages, "ID", "Name");
            
            return View();
        }

        // POST: /Charge/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Charge/CanWrite")]
        public ActionResult Create([Bind(Include="ID,ChargeStageID,Name,IsActive")] Charge charge)
        {
            ViewBag.ChargeStageID = new SelectList(db.ChargeStages, "ID", "Name", charge.ChargeStageID);

            if (db.Charges.Where(x => x.ChargeStageID == charge.ChargeStageID && x.Name == charge.Name).Count() > 0)
            {
                ViewBag.Message = "Can not Save Charge Stage and Charge Name Already Exists...!";
                return View(charge);
                
            }

            charge.CreateDate = DateTime.UtcNow.AddHours(5.30);
            charge.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); ;
            charge.ModifyDate = null;
            charge.ModifyBy = null;
            charge.NetworkIP = "";
            charge.DeviceType = "";
            charge.DeviceID = "";

            if (ModelState.IsValid)
            {
                db.Charges.Add(charge);
                db.SaveChanges();
                ViewBag.Message = "Charge stage save successfully...!";
                return RedirectToAction("Index");
               
            }

           
           
            return View(charge);
        }

        // GET: /Charge/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "Charge/CanWrite")]
        public ActionResult Edit(int? id)
        {


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Charge charge = db.Charges.Find(id);
            if (charge == null)
            {
                return HttpNotFound();
            }
            ViewBag.ChargeStageID = new SelectList(db.ChargeStages, "ID", "Name", charge.ChargeStageID);
         
            return View(charge);
        }

        // POST: /Charge/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Charge/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,ChargeStageID,Name,IsActive")] Charge charge)
        {
            ViewBag.ChargeStageID = new SelectList(db.ChargeStages, "ID", "Name", charge.ChargeStageID);

            if (db.Charges.Where(x => x.ChargeStageID == charge.ChargeStageID && x.Name == charge.Name && x.ID != charge.ID).Count() > 0)
            {
                ViewBag.Message = "Can not Save Charge Stage and Charge Name Already Exists...!";
                return View(charge);

            }


            Charge lcharge = db.Charges.Find(charge.ID);
            charge.CreateDate = lcharge.CreateDate;
            charge.CreateBy = lcharge.CreateBy;
            charge.ModifyDate = DateTime.UtcNow.AddHours(5.30);
            charge.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); ;
            charge.NetworkIP = string.Empty;
            charge.DeviceType = string.Empty;
            charge.DeviceID = string.Empty;

            if (ModelState.IsValid)
            {
                //db.Entry(charge).State = EntityState.Modified;
                db.Entry(lcharge).CurrentValues.SetValues(charge);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ChargeStageID = new SelectList(db.ChargeStages, "ID", "Name", charge.ChargeStageID);
           
            return View(charge);
        }

        // GET: /Charge/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "Charge/CanDelete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Charge charge = db.Charges.Find(id);
            if (charge == null)
            {
                return HttpNotFound();
            }
            return View(charge);
        }

        // POST: /Charge/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Charge/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Charge charge = db.Charges.Find(id);
            db.Charges.Remove(charge);
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
