using BusinessLogicLayer;
using Leaders.Filter;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Leaders.Controllers
{
    [SessionExpire]
    public class BBPUsersDashboardController : Controller
    {
        BoosterPlanData BD = new BoosterPlanData();
        public ActionResult Index()
        {
            long UserLoginId = Convert.ToInt64(Session["ID"]);
            BBPUserDashboard dashboard = new BBPUserDashboard();
            dashboard = BD.GetData_Dashboard(UserLoginId);
            return View(dashboard);
        }

    }
}