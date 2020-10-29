using BusinessLogicLayer;
using Leaders.Filter;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Leaders.Controllers
{
    [SessionExpire]
    public class MerchantUserDashboardController : Controller
    {
        // GET: RMDashboard
        BoosterPlanData BD = new BoosterPlanData();
        public ActionResult Index()
        {
            long UserLoginId = Convert.ToInt64(Session["ID"]);
            MerchantUserDashboardViewModel dashboard = new MerchantUserDashboardViewModel();
            dashboard = GetDashboardCount(UserLoginId);
            return View(dashboard);
        }

        public MerchantUserDashboardViewModel GetDashboardCount(long? UserLoginId)
        {
            List<MerchantUserDashboardViewModel> list = new List<MerchantUserDashboardViewModel>();
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@UserLoginId", SqlDbType = SqlDbType.BigInt, Value= UserLoginId},
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("MerchantLeaderDashboard", sp);
            if (ds.Tables.Count > 0)
            {
                list = BusinessLogicLayer.Helper.CreateListFromTable<MerchantUserDashboardViewModel>(ds.Tables[0]);
            }
            return list.FirstOrDefault();
        }
        public List<LeaderERPFromMerchant> GetERPReport(long? UserLoginId,int L)
        {
            List<LeaderERPFromMerchant> list = new List<LeaderERPFromMerchant>();
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@UserLoginId", SqlDbType = SqlDbType.BigInt, Value= UserLoginId},
                new SqlParameter() {ParameterName = "@L", SqlDbType = SqlDbType.Int, Value= L}
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("LeaderERPFromMerchant", sp);
            if (ds.Tables.Count > 0)
            {
                list = BusinessLogicLayer.Helper.CreateListFromTable<LeaderERPFromMerchant>(ds.Tables[0]);
            }
            return list;
        }

        public ActionResult GetReport(int L)
        {
            ViewBag.MessageFor = L;
            long UserLoginId = Convert.ToInt64(Session["ID"]);
            List<LeaderERPFromMerchant> list = new List<LeaderERPFromMerchant>();
            list = GetERPReport(UserLoginId, L);
            return View(list);
        }
    }
}