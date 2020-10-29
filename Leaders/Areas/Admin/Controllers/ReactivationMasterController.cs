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
    public class ReactivationMasterController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public ActionResult Index()
        {
            List<ReactivationMaster> listReactivation = db.ReactivationMasters.ToList();
            return View(listReactivation);
        }
        [HttpPost]
        public ActionResult LogIndex()
        {
            List<LogLeadersReactivationMaster> listLog = db.LogLeadersReactivationMasters.ToList();
            return Json(new { data = listLog }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Create(ReactivationMaster collection)
        {
            LogLeadersReactivationMaster objLog = new LogLeadersReactivationMaster();

            ReactivationMaster objReactivation = new ReactivationMaster();
            objReactivation.Activation_Fees = collection.Activation_Fees;
            objReactivation.Leaders_Status = collection.Leaders_Status;
            objReactivation.No_Of_Days = collection.No_Of_Days;
            objReactivation.Penalty_Amount = collection.Penalty_Amount;
            objReactivation.Penalty_On = collection.Penalty_On;
            objReactivation.Penalty_Percentage = collection.Penalty_Percentage;
            objReactivation.Display_Status = collection.Display_Status;

            objReactivation.Create_Date = System.DateTime.Now;
            objReactivation.Modify_Date = System.DateTime.Now;
            db.ReactivationMasters.Add(objReactivation);
            

            db.SaveChanges();

            // for save in log table
            objLog.Activation_Fees = collection.Activation_Fees;
            objLog.Leaders_Status = collection.Leaders_Status;
            objLog.No_Of_Days = collection.No_Of_Days;
            objLog.Penalty_Amount = collection.Penalty_Amount;
            objLog.Penalty_On = collection.Penalty_On;
            objLog.Penalty_Percentage = collection.Penalty_Percentage;
            objLog.Last_Create_Date = System.DateTime.Now;

            db.LogLeadersReactivationMasters.Add(objLog);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            ReactivationMaster objReactivation = db.ReactivationMasters.Where(x => x.ID == id).FirstOrDefault();
            return View(objReactivation);
        }
        [HttpPost]
        public ActionResult Edit(ReactivationMaster collection)
        {
            ReactivationMaster objReactivation = db.ReactivationMasters.Where(x => x.ID == collection.ID).FirstOrDefault();
            LogLeadersReactivationMaster objLog = new LogLeadersReactivationMaster();
            
            objReactivation.Activation_Fees = collection.Activation_Fees;
            objReactivation.Leaders_Status = collection.Leaders_Status;
            objReactivation.No_Of_Days = collection.No_Of_Days;
            objReactivation.Penalty_Amount = collection.Penalty_Amount;
            objReactivation.Penalty_On = collection.Penalty_On;
            objReactivation.Penalty_Percentage = collection.Penalty_Percentage;
            objReactivation.Current_Status = collection.Current_Status;
            objReactivation.Display_Status = collection.Display_Status;
            objReactivation.Create_Date = System.DateTime.Now;
            objReactivation.Modify_Date = System.DateTime.Now;

            db.SaveChanges();
            objLog.Activation_Fees = collection.Activation_Fees;
            objLog.Leaders_Status = collection.Leaders_Status;
           
            objLog.No_Of_Days = collection.No_Of_Days;
            objLog.Penalty_Amount = collection.Penalty_Amount;
            objLog.Penalty_On = collection.Penalty_On;
            objLog.Penalty_Percentage = collection.Penalty_Percentage;
            objLog.Last_Create_Date = System.DateTime.Now;

            db.LogLeadersReactivationMasters.Add(objLog);
            db.SaveChanges();




           



            return RedirectToAction("Index");
        }
	}
}