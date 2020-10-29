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

namespace  Administrator.Controllers
{
    public class BulkLogController : Controller
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

        // GET: /BulkLog/
        public ActionResult Index()
        {
            var bulklogs = db.BulkLogs.Include(b => b.Shop);
            return View(bulklogs.ToList());
        }

        // GET: /BulkLog/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BulkLog bulklog = db.BulkLogs.Find(id);
            if (bulklog == null)
            {
                return HttpNotFound();
            }
            return View(bulklog);
        }

        // GET: /BulkLog/Create
        public ActionResult Create()
        {
            ViewBag.ShopID = new SelectList(db.Shops, "ID", "Name");
            ViewBag.BulkType=new SelectList(new[]
                                          {
                                              new {ID="1",Name="ShopProduct"},
                                              new{ID="2",Name="ShopStock"},                                              
                                          },
                            "ID", "Name");
                      
            return View();
        }

        // POST: /BulkLog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.               
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,ShopID,ExcelSheetName,BulkType,CreateDate,CreateBy,NetworkIP,DeviceType,DeviceID")] BulkLog bulklog)
        {
            bulklog.CreateDate = CommonFunctions.GetLocalTime() ;
            bulklog.CreateBy = fUserId;
            bulklog.NetworkIP = CommonFunctions.GetClientIP();


            if (ModelState.IsValid)
            {
                db.BulkLogs.Add(bulklog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ShopID = new SelectList(db.Shops, "ID", "Name", bulklog.ShopID);
            return View(bulklog);
        }

        // GET: /BulkLog/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BulkLog bulklog = db.BulkLogs.Find(id);
            if (bulklog == null)
            {
                return HttpNotFound();
            }
            ViewBag.ShopID = new SelectList(db.Shops, "ID", "Name", bulklog.ShopID);
            return View(bulklog);
        }

        // POST: /BulkLog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ShopID,ExcelSheetName,BulkType,CreateDate,CreateBy,NetworkIP,DeviceType,DeviceID")] BulkLog bulklog)
        {
            BulkLog lBulkLog = db.BulkLogs.Find(bulklog.ID);
            if (lBulkLog == null)
            {
                return View("Error");
            }
            bulklog.CreateDate = CommonFunctions.GetLocalTime();
            bulklog.CreateBy = fUserId;
            bulklog.NetworkIP = CommonFunctions.GetClientIP();
            TryUpdateModel(bulklog);

            if (ModelState.IsValid)
            {
                db.Entry(lBulkLog).CurrentValues.SetValues(bulklog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ShopID = new SelectList(db.Shops, "ID", "Name", bulklog.ShopID);
            return View(bulklog);
        }

        // GET: /BulkLog/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BulkLog bulklog = db.BulkLogs.Find(id);
            if (bulklog == null)
            {
                return HttpNotFound();
            }
            return View(bulklog);
        }

        // POST: /BulkLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BulkLog bulklog = db.BulkLogs.Find(id);
            db.BulkLogs.Remove(bulklog);
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
