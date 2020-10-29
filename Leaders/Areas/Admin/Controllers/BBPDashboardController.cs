using BusinessLogicLayer;
using Leaders.Filter;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Leaders.Areas.Admin.Controllers
{
    public class BBPDashboardController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        BBPReportsController rpt = new BBPReportsController();
        BoosterPlanData BD = new BoosterPlanData();
        [AdminSessionExpire]
        public ActionResult Index()
        {
            if (Session["RoleName"].ToString() == "superadmin")
            {
                BBPDashboardViewModel objData = new BBPDashboardViewModel();
                objData = GetData(0);
                objData.PayoutDateFilter = rpt.BindFilter();
                return View(objData);
            }
            TempData["Result"] = "Unauthorized Access!!!";
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        [AdminSessionExpire]
        [HttpPost]
        public ActionResult Index(BBPDashboardViewModel obj)
        {
            if (Session["RoleName"].ToString() == "superadmin")
            {
                try
                {
                    obj = GetData(obj.PayoutDateFilterID);
                    obj.PayoutDateFilter = rpt.BindFilter();
                    return View(obj);
                }
                catch (Exception ex)
                {
                    TempData["Result"] = ex.Message;
                }
            }
            else
            {
                TempData["Result"] = "Unauthorized Access!!!";
            }
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        public BBPDashboardViewModel GetData(long BoosterPayoutID)
        {
            Session["PayoutDateFilterID"] = BoosterPayoutID;
            List<BBPDashboardViewModel> obj = new List<BBPDashboardViewModel>();
            DateTime FromDate = DateTime.Today;
            DateTime ToDate = DateTime.Today;           

            rpt.SetDate(BoosterPayoutID, out FromDate, out ToDate);            
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@BoosterPlanPayoutID", SqlDbType = SqlDbType.BigInt, Value= BoosterPayoutID},
                new SqlParameter() {ParameterName = "@FromDate", SqlDbType = SqlDbType.Date, Value= FromDate},
                new SqlParameter() {ParameterName = "@ToDate", SqlDbType = SqlDbType.DateTime, Value= ToDate}
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("BoosterPlanDashboard", sp);
            
            if (ds.Tables.Count > 0)
            {
                obj = BusinessLogicLayer.Helper.CreateListFromTable<BBPDashboardViewModel>(ds.Tables[0]);
            }
            
            return obj.FirstOrDefault();
        }
    }
}