using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Leaders.Filter;
using ModelLayer.Models;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class CoinRateMasterController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        public ActionResult Index()
        {

            List<MLMCoinRate> userList = new List<MLMCoinRate>();
            userList = db.MLMCoinRates.ToList();
            return View(userList);
        }

        public ActionResult LogIndex()
        {
            List<Log_MLMCoinRate> logCoinList=db.Log_MLMCoinRates.ToList();
            return Json(new { data = logCoinList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(long? id)
        {
            ViewBag.UserList = db.MLMCoinRates.Where(h => h.IsActive == true).ToList();
            MLMCoinRate model = db.MLMCoinRates.Where(h =>h.ID==id).SingleOrDefault();
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(MLMCoinRate collection, string startDate, string endDate)
        {
            MLMCoinRate oCoinRate = db.MLMCoinRates.Where(h => h.ID == collection.ID).SingleOrDefault();
            oCoinRate.StartDate = collection.StartDate;
            oCoinRate.EndDate = collection.EndDate;
            oCoinRate.Rate = collection.Rate;
            oCoinRate.CreateBy = Convert.ToInt64(Session["ID"]);
                oCoinRate.CreateDate=System.DateTime.Now;
                oCoinRate.ModifyBy = Convert.ToInt64(Session["ID"]);
                oCoinRate.ModifyDate = System.DateTime.Now;
            oCoinRate.IsActive=true;
            db.SaveChanges();

            // for save in log tab;e

            Log_MLMCoinRate objLog = new Log_MLMCoinRate();
            objLog.StartDate = collection.StartDate;
            objLog.EndDate = collection.EndDate;
            objLog.Rate = collection.Rate;
            objLog.CreateBy = Convert.ToInt64(Session["ID"]);
            objLog.CreateDate = System.DateTime.Now;
            objLog.ModifyBy = Convert.ToInt64(Session["ID"]);
            objLog.ModifyDate = System.DateTime.Now;
            objLog.IsActive = true;

            objLog.Last_Create_Date = System.DateTime.Now;

            db.Log_MLMCoinRates.Add(objLog);


            //MLMCoinRate obj = db.MLMCoinRates.Where(h => h.ID == collection.ID).SingleOrDefault();
            //if (obj == null)
            //{
            //    db.MLMCoinRates.Add(oCoinRate);
            //    db.SaveChanges();
            //    TempData["Alert"] = "Add";
            //}
            //else
            //{
            //    db.SaveChanges();
            //    TempData["Alert"] = "Update";
            //}

           // List<MLMCoinRate> userList = db.MLMCoinRates.Where(x=>x.IsActive==true).ToList();
            //foreach (var item in userList)
            //{
            //    if (collection.ID == item.ID)
            //    {
            //        db.SaveChanges();
            //        goto updateEntry;
            //       // break;
            //    }
            //}


           // db.MLMCoinRates.Add(oCoinRate);
           // updateEntry:
         //   db.SaveChanges();
            
            return RedirectToAction("Create", new { area="Admin"});
        }

        public ActionResult Delete(long? id)
        {
            MLMCoinRate obj = db.MLMCoinRates.Where(x => x.ID == id).SingleOrDefault();

            obj.IsActive = false;
            db.SaveChanges();
            TempData["Alert"] = "Delete";
            return RedirectToAction("Create", new { area = "Admin", id = 0 });

        }
	}
}