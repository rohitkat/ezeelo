using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;

namespace Administrator.Controllers
{
    public class AccountingHeadController : Controller
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

        // GET: /AccountingHead/
        public ActionResult Index()
        {
            var accountingheads = db.AccountingHeads.Include(a => a.PersonalDetail).Include(a => a.PersonalDetail1);
            return View(accountingheads.ToList());
        }

        // GET: /AccountingHead/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountingHead accountinghead = db.AccountingHeads.Find(id);
            if (accountinghead == null)
            {
                return HttpNotFound();
            }
            return View(accountinghead);
        }

        // GET: /AccountingHead/Create
        public ActionResult Create()
        {
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            return View();
        }

        // POST: /AccountingHead/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,IsActive,CreateDate,CreateBy")] AccountingHead accountinghead)
        {
            try
            {
                accountinghead.CreateDate = DateTime.Now;
                accountinghead.CreateBy = fUserId;
                if (ModelState.IsValid)
                {
                    db.AccountingHeads.Add(accountinghead);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", accountinghead.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", accountinghead.ModifyBy);
                return View(accountinghead);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", accountinghead.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", accountinghead.ModifyBy);
                return View(accountinghead); 
            }

            }

        // GET: /AccountingHead/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountingHead accountinghead = db.AccountingHeads.Find(id);
            if (accountinghead == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", accountinghead.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", accountinghead.ModifyBy);
            return View(accountinghead);
        }

        // POST: /AccountingHead/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] AccountingHead accountinghead)
        {
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                AccountingHead lAccountingHead = db1.AccountingHeads.Find(accountinghead.ID);
                accountinghead.CreateDate = lAccountingHead.CreateDate;
                accountinghead.CreateBy = lAccountingHead.CreateBy;
                accountinghead.ModifyDate = DateTime.Now;
                accountinghead.ModifyBy = fUserId;
                db1.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(accountinghead).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", accountinghead.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", accountinghead.ModifyBy);
                return View(accountinghead);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", accountinghead.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", accountinghead.ModifyBy);
                return View(accountinghead);
            }
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
