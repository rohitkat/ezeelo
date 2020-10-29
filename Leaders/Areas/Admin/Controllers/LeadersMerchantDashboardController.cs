using BusinessLogicLayer;
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
    public class LeadersMerchantDashboardController : Controller
    {
        // GET: Admin/LeadersMerchantDashboard
        EzeeloDBContext db = new EzeeloDBContext();
        BoosterPlanData BD = new BoosterPlanData();
        public ActionResult Index()
        {
            List<MerchantAdminDashboard> list = GetDashboardData();
            return View(list.FirstOrDefault());
        }

        public List<MerchantAdminDashboard> GetDashboardData()
        {
            List<MerchantAdminDashboard> list = new List<MerchantAdminDashboard>();
            List<SqlParameter> sp = new List<SqlParameter>()
            {
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("MerchantAdminDashboard", sp);
            Session["TransactionReport"] = ds;
            if (ds.Tables.Count > 0)
            {
                list = BusinessLogicLayer.Helper.CreateListFromTable<MerchantAdminDashboard>(ds.Tables[0]);
            }
            return list;
        }
        public List<MerchantDashboardAdminReport> GetDashboardReport(int Type)
        {
            List<MerchantDashboardAdminReport> list = new List<MerchantDashboardAdminReport>();
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@Type", SqlDbType = SqlDbType.Int, Value= Type},
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("MerchantAdminDashReport", sp);
            Session["TransactionReport"] = ds;
            if (ds.Tables.Count > 0)
            {
                list = BusinessLogicLayer.Helper.CreateListFromTable<MerchantDashboardAdminReport>(ds.Tables[0]);
            }
            return list;
        }

        public ActionResult MerchantDashReport(int Type)
        {
            ViewBag.Type = Type;
            List<MerchantDashboardAdminReport> list = GetDashboardReport(Type);
            return View(list);
        }

    }
}