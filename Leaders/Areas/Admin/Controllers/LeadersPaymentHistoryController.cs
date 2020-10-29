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
    public class LeadersPaymentHistoryController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();


        public ActionResult Index(int? userID)
        {
            LeadersPayoutMaster objPayout = db.LeadersPayoutMasters.FirstOrDefault();


            List<LeadersPaymentHistory> paymentHistoryList = db.LeadersPaymentHistorys.ToList();
            return View(paymentHistoryList);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Ezeelo_Payment_History collection)
        {

            return View();
        }
	}
}