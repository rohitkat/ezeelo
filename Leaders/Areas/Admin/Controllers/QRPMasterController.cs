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
    public class QRPMasterController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public ActionResult Index()
        {
            List<QRPMaster> listQrp = db.QRPMasters.ToList();
            return View(listQrp);

        }

        public ActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Create(QRPMaster collection)
        {
            QRPMaster objQRP = new QRPMaster();
            objQRP.City = collection.City;
            objQRP.Create_Date = System.DateTime.Now;
            objQRP.Current_QRP = collection.Current_QRP;
            objQRP.Franchise_ID = collection.ID;
            objQRP.Max_QRP = collection.Max_QRP;
            objQRP.Min_QRP = collection.Min_QRP;
            db.QRPMasters.Add(objQRP);

            // for save in log table
            LogQRPMaster objLog = new LogQRPMaster();
            objLog.Max_QRP = collection.Max_QRP;
            objLog.Min_QRP = collection.Min_QRP;

            objLog.Franchise_ID = collection.Franchise_ID;
            objLog.City = collection.City;
            objLog.Current_QRP = collection.Current_QRP;
            objLog.Last_Create_Date = System.DateTime.Now;
            db.LogQRPMasters.Add(objLog);
            db.SaveChanges();


            return View();
        }


        public ActionResult Edit(int? id)
        {
            QRPMaster objQRP = db.QRPMasters.Where(x => x.ID == id).FirstOrDefault();

            return View(objQRP);
        }
        [HttpPost]
        public ActionResult Edit(QRPMaster collection)
        {
            QRPMaster objQRP = db.QRPMasters.Where(x => x.ID == collection.ID).FirstOrDefault();
            objQRP.Max_QRP = collection.Max_QRP;
            objQRP.Min_QRP = collection.Min_QRP;
            objQRP.Modify_Date = System.DateTime.Now;
            objQRP.Franchise_ID = collection.Franchise_ID;
            objQRP.City = collection.City;
            objQRP.Current_QRP = collection.Current_QRP;
            db.SaveChanges();

            // for save in log table
            LogQRPMaster objLog = new LogQRPMaster();
            objLog.Max_QRP = collection.Max_QRP;
            objLog.Min_QRP = collection.Min_QRP;

            objLog.Franchise_ID = collection.Franchise_ID;
            objLog.City = collection.City;
            objLog.Current_QRP = collection.Current_QRP;
            objLog.Last_Create_Date = System.DateTime.Now;
            db.LogQRPMasters.Add(objLog);
            db.SaveChanges();


            return RedirectToAction("Index");//Added By Sonali for redirect to index
        }
    }
}