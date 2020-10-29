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
    public class LedgerHeadController : Controller
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
        // GET: /LedgerHead/
        public ActionResult Index()
        {
            var ledgerheads = db.LedgerHeads.Include(l => l.AccountingHead).Include(l => l.PersonalDetail).Include(l => l.PersonalDetail1);
            return View(ledgerheads.ToList());
        }

        // GET: /LedgerHead/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LedgerHead ledgerhead = db.LedgerHeads.Find(id);
            if (ledgerhead == null)
            {
                return HttpNotFound();
            }
            return View(ledgerhead);
        }

        // GET: /LedgerHead/Create
        public ActionResult Create()
        {
            ViewBag.AccountingHeadID = new SelectList(db.AccountingHeads, "ID", "Name");
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            return View();
        }

        // POST: /LedgerHead/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,AccountingHeadID,IsActive,CreateDate,CreateBy")] LedgerHead ledgerhead)
        {
            try
            {
                LedgerHead lLedgerHead = new LedgerHead();
                ledgerhead.CreateDate = DateTime.Now;
                ledgerhead.CreateBy = fUserId;

                if (ModelState.IsValid)
                {
                    db.LedgerHeads.Add(ledgerhead);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.AccountingHeadID = new SelectList(db.AccountingHeads, "ID", "Name", ledgerhead.AccountingHeadID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ledgerhead.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ledgerhead.ModifyBy);
                return View(ledgerhead);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.AccountingHeadID = new SelectList(db.AccountingHeads, "ID", "Name", ledgerhead.AccountingHeadID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ledgerhead.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ledgerhead.ModifyBy);
                return View(ledgerhead);

            }
        }

        // GET: /LedgerHead/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LedgerHead ledgerhead = db.LedgerHeads.Find(id);
            if (ledgerhead == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountingHeadID = new SelectList(db.AccountingHeads, "ID", "Name", ledgerhead.AccountingHeadID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ledgerhead.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ledgerhead.ModifyBy);
            return View(ledgerhead);
        }

        // POST: /LedgerHead/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,AccountingHeadID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy")] LedgerHead ledgerhead)
        {
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                LedgerHead lLedgerHead = db1.LedgerHeads.Find(ledgerhead.ID);
                ledgerhead.CreateDate = lLedgerHead.CreateDate;
                ledgerhead.CreateBy = lLedgerHead.CreateBy;
                ledgerhead.ModifyDate = DateTime.Now;
                ledgerhead.ModifyBy = fUserId;

                if (ModelState.IsValid)
                {
                    db.Entry(ledgerhead).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.AccountingHeadID = new SelectList(db.AccountingHeads, "ID", "Name", ledgerhead.AccountingHeadID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ledgerhead.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ledgerhead.ModifyBy);
                return View(ledgerhead);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.AccountingHeadID = new SelectList(db.AccountingHeads, "ID", "Name", ledgerhead.AccountingHeadID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ledgerhead.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ledgerhead.ModifyBy);
                return View(ledgerhead);
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
