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
    public class LeadersDesignationQualifierMasterController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        public ActionResult Index()
        {
            List<LeadersDesignationQualifierModel> desigQuaList = db.LeadersDesignationQualifierModels.Where(x => x.Status == true).ToList();
            return View(desigQuaList);
        }
        public ActionResult Index2()
        {
            List<LeadersDesignationQualifierModel> desigQuaList = db.LeadersDesignationQualifierModels.Where(x => x.Status == true).ToList();
            return View(desigQuaList);
        }

        public ActionResult Update(int? id)
        {
            LeadersDesignationQualifierModel objDesg = db.LeadersDesignationQualifierModels.Where(x => x.ID == id).SingleOrDefault();
            return View(objDesg);
        }
        [HttpPost]
        public ActionResult Update(LeadersDesignationQualifierModel collection)
        {
            LeadersDesignationQualifierModel objDesig = db.LeadersDesignationQualifierModels.Where(x => x.ID == collection.ID).SingleOrDefault();
            objDesig.At_Level = collection.At_Level;
            objDesig.Designation = collection.Designation;
            objDesig.Display_Name = collection.Display_Name;
            objDesig.Downline_Current_Level = collection.Downline_Current_Level;
            objDesig.Min_Active_User = collection.Min_Active_User;
            objDesig.Min_Designation = collection.Min_Designation;
            objDesig.Min_User = collection.Min_User;
            objDesig.Short_Name = collection.Short_Name;

            db.SaveChanges();

            return RedirectToAction("Index");


        }
	}
}