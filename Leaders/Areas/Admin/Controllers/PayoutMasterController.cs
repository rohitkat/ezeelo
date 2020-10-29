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
    public class PayoutMasterController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        public ActionResult Index()
        {
            List<LeadersPayoutMaster> payoutList = db.LeadersPayoutMasters.ToList();
            return View(payoutList);
        }

        public ActionResult Create()
        {
            return View();

        }
        [HttpPost]
        public ActionResult Create(LeadersPayoutMaster collection)
        {
            LeadersPayoutMaster objPayout = new LeadersPayoutMaster();
            objPayout.GST = collection.GST;
            objPayout.Min_Resereved = collection.Min_Resereved;
            objPayout.Penalty = collection.Penalty;
            objPayout.Processing_Fees = collection.Processing_Fees;
            objPayout.TDS = collection.TDS;
            objPayout.Create_Date = System.DateTime.Now;

            db.LeadersPayoutMasters.Add(objPayout);
           


            LogLeadersPayout objLog = new LogLeadersPayout();
            objLog.GST = collection.GST;
            objLog.Min_Resereved = collection.Min_Resereved;
            objLog.Penalty = collection.Penalty;
            objLog.Processing_Fees = collection.Processing_Fees;
            objLog.TDS = collection.TDS;
            objLog.Last_Create_Date = System.DateTime.Now;
            db.LogLeadersPayouts.Add(objLog);

            db.SaveChanges();

            return RedirectToAction("Index");

        }

        public ActionResult Edit(int id)
        {
            LeadersPayoutMaster objPayout = db.LeadersPayoutMasters.Where(x => x.ID == id).FirstOrDefault();


            return View(objPayout);

        }
        [HttpPost]
        public ActionResult Edit(LeadersPayoutMaster collection)
        {
            LogLeadersPayout objLog = new LogLeadersPayout();
            LeadersPayoutMaster objPayout = db.LeadersPayoutMasters.Where(x => x.ID == collection.ID).FirstOrDefault();
            objPayout.GST = collection.GST;
            objPayout.Min_Resereved = collection.Min_Resereved;
            objPayout.Penalty = collection.Penalty;
            objPayout.Processing_Fees = collection.Processing_Fees;
            objPayout.TDS = collection.TDS;
            objPayout.Modify_Date = System.DateTime.Now;
            db.SaveChanges();

            // for save in log table
            objLog.GST = collection.GST;
            objLog.Min_Resereved = collection.Min_Resereved;
            objLog.Penalty = collection.Penalty;
            objLog.Processing_Fees = collection.Processing_Fees;
            objLog.TDS = collection.TDS;
            objLog.Last_Create_Date = System.DateTime.Now;
            db.LogLeadersPayouts.Add(objLog);
            db.SaveChanges();
            

            return RedirectToAction("Index");
        }
	}
}