using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Leaders.Filter;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;


namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class IncomeMasterController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
       
        public ActionResult Index()
        {
            ViewBag.LogIncomeList = db.LogLeadersIncomeMasters.ToList();
            List<LeadersIncomeMaster> IncomeList = db.LeadersIncomeMasters.ToList();
            return View(IncomeList);
        }

        [HttpPost]
        public ActionResult LogIndex()
        {
           List<LogLeadersIncomeMaster> logList = db.LogLeadersIncomeMasters.ToList();
           return Json(new { data = logList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int? id)
        {
            LeadersIncomeMaster objIncome = db.LeadersIncomeMasters.Where(x => x.ID == id).FirstOrDefault();
            return View(objIncome);
        }
        [HttpPost]
        public ActionResult Edit(LeadersIncomeMaster collection)
        {
            LeadersIncomeMaster objIncome = db.LeadersIncomeMasters.Where(x => x.ID == collection.ID).FirstOrDefault();
            objIncome.Level0 = collection.Level0;
            objIncome.Level1 = collection.Level1;
            objIncome.Level2 = collection.Level2;
            objIncome.Level3 = collection.Level3;
            objIncome.Level4 = collection.Level4;
            objIncome.Level5 = collection.Level5;
            objIncome.Level6 = collection.Level6;
            objIncome.Level7 = collection.Level7;
            objIncome.Level8 = collection.Level8;
            objIncome.Level9 = collection.Level9;
            objIncome.Level10 = collection.Level10;
            objIncome.Level11 = collection.Level11;
            objIncome.Level12 = collection.Level12;
            objIncome.Level13 = collection.Level13;
            objIncome.Level14 = collection.Level14;
            objIncome.Modify_Date = System.DateTime.Now;


            LogLeadersIncomeMaster objLog = new LogLeadersIncomeMaster();
            objLog.Level0 = collection.Level0;
            objLog.Level1 = collection.Level1;
            objLog.Level2 = collection.Level2;
            objLog.Level3 = collection.Level3;
            objLog.Level4 = collection.Level4;
            objLog.Level5 = collection.Level5;
            objLog.Level6 = collection.Level6;
            objLog.Level7 = collection.Level7;
            objLog.Level8 = collection.Level8;
            objLog.Level9 = collection.Level9;
            objLog.Level10 = collection.Level10;
            objLog.Level11 = collection.Level11;
            objLog.Level12 = collection.Level12;
            objLog.Level13 = collection.Level13;
            objLog.Level14 = collection.Level14;
            objLog.Last_Modify_Date = System.DateTime.Now;
            db.LogLeadersIncomeMasters.Add(objLog);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult EditDirectIncome(int? id)
        {
            LeadersIncomeMaster objIncome = db.LeadersIncomeMasters.Where(x => x.ID ==id).FirstOrDefault();
            return View(objIncome);
        }
        [HttpPost]
        public ActionResult EditDirectIncome(LeadersIncomeMaster collection)
        {
            
            LeadersIncomeMaster objIncome = db.LeadersIncomeMasters.Where(x => x.ID == collection.ID).FirstOrDefault();
            objIncome.Level0 = collection.Level0;
            objIncome.Level1 = collection.Level1;
            objIncome.Level2 = collection.Level2;
            objIncome.Level3 = collection.Level3;
            objIncome.Level4 = collection.Level4;
            objIncome.Modify_Date = System.DateTime.Now;


            LogLeadersIncomeMaster objLog = new LogLeadersIncomeMaster();
            objLog.Level0 = collection.Level0;
            objLog.Level1 = collection.Level1;
            objLog.Level2 = collection.Level2;
            objLog.Level3 = collection.Level3;
            objLog.Level4 = collection.Level4;
            objLog.Last_Modify_Date = System.DateTime.Now;
            db.LogLeadersIncomeMasters.Add(objLog);

            db.SaveChanges();
           
            
            return RedirectToAction("Index");
        }


        public ActionResult EditInDirectIncome(int? id)
        {
            LeadersIncomeMaster objIncome = db.LeadersIncomeMasters.Where(x => x.ID == id).FirstOrDefault();
            return View(objIncome);
        }
        [HttpPost]
        public ActionResult EditInDirectIncome(LeadersIncomeMaster collection)
        {
            LogLeadersIncomeMaster objLog = new LogLeadersIncomeMaster();
            LeadersIncomeMaster objIncome = db.LeadersIncomeMasters.Where(x => x.ID == collection.ID).FirstOrDefault();
            objIncome.Level5 = collection.Level5;
            objIncome.Level6 = collection.Level6;
            objIncome.Level7 = collection.Level7;
            objIncome.Level8 = collection.Level8;
            objIncome.Level9 = collection.Level9;
            objIncome.Level10 = collection.Level10;
            objIncome.Level11 = collection.Level11;
            objIncome.Level12 = collection.Level12;
            objIncome.Level13 = collection.Level13;
            objIncome.Level14 = collection.Level14;
            objIncome.Modify_Date = System.DateTime.Now;


            objLog.Level5 = collection.Level5;
            objLog.Level6 = collection.Level6;
            objLog.Level7 = collection.Level7;
            objLog.Level8 = collection.Level8;
            objLog.Level9 = collection.Level9;
            objLog.Level10 = collection.Level10;
            objLog.Level11 = collection.Level11;
            objLog.Level12 = collection.Level12;
            objLog.Level13 = collection.Level13;
            objLog.Level14 = collection.Level14;
            objLog.Last_Modify_Date = System.DateTime.Now;
            db.LogLeadersIncomeMasters.Add(objLog);
          


            db.SaveChanges();


            return RedirectToAction("Index");
        }
	}
}