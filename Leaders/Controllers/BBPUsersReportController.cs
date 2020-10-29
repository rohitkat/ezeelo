using BusinessLogicLayer;
using Leaders.Areas.Admin.Controllers;
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

namespace Leaders.Controllers
{
    [SessionExpire]
    public class BBPUsersReportController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        BoosterPlanData BD = new BoosterPlanData();
        // GET: BBPUsersReport
        public ActionResult WeeklyEarning()
        {
            long UserLoginId = Convert.ToInt64(Session["ID"]);
            List<BBPWeeklyEarning> earnings = new List<BBPWeeklyEarning>();
            earnings = BD.GetData_Earning(UserLoginId);
            return View(earnings);
        }

       

        public ActionResult BBPTabularView(string search)
        {
            BBPTabularviewModel obj = new BBPTabularviewModel();
            long UserLoginId = Convert.ToInt64(Session["ID"]);
            List<BBPTabularview> Tabluar = new List<BBPTabularview>();
            Tabluar = BD.GetData_Tabular(UserLoginId, null);
            TempData["Search"] = search;
            List<SelectListItem> PayoutDateFilter = new List<SelectListItem>();
            List<BoosterPlanPayout> planPayout = db.boosterPlanPayouts.OrderByDescending(p => p.ID).ToList();
            PayoutDateFilter.AddRange(new SelectList(planPayout.
                Select(p => new
                {
                    ID = p.ID,
                    FromDate = p.FromDate,
                    ToDate = p.ToDate,
                }).ToList()
                .Select(p => new
                {
                    ID = p.ID,
                    Name = p.FromDate.ToString("dd MMM yy") + " To " + p.ToDate.ToString("dd MMM yy")
                }), "ID", "Name").ToList());
            obj.list = Tabluar;
            obj.PayoutDateFilter = PayoutDateFilter;
            return View(obj);
        }

        [HttpPost]
        public ActionResult BBPTabularView(BBPTabularviewModel obj,string LevelId,string chkSub)
        {
            long UserLoginId = Convert.ToInt64(Session["ID"]);
            List<BBPTabularview> Tabluar = new List<BBPTabularview>();
            Tabluar = BD.GetData_Tabular(UserLoginId, obj.PayoutDateFilterId);
            List<SelectListItem> PayoutDateFilter = new List<SelectListItem>();
            List<BoosterPlanPayout> planPayout = db.boosterPlanPayouts.OrderByDescending(p => p.ID).ToList();
            PayoutDateFilter.AddRange(new SelectList(planPayout.
                Select(p => new
                {
                    ID = p.ID,
                    FromDate = p.FromDate,
                    ToDate = p.ToDate,
                }).ToList()
                .Select(p => new
                {
                    ID = p.ID,
                    Name = p.FromDate.ToString("dd MMM yy") + " To " + p.ToDate.ToString("dd MMM yy")
                }), "ID", "Name").ToList());
            if(chkSub != null)
            {
                TempData["Search"] = "1";
            }
            if (LevelId != "0")
            {
                //LevelId = "L" + LevelId;
                TempData["SearchLevel"] = LevelId;
                //Tabluar = Tabluar.Where(p => p.Level == LevelId).ToList();
            }
            else
            {
                TempData["SearchLevel"] = null;
            }
            obj.list = Tabluar;
            obj.PayoutDateFilter = PayoutDateFilter;
            return View(obj);
        }



        public ActionResult BBPNetwork()
        {
            long UserLoginId = Convert.ToInt64(Session["ID"]);
            BBPNetworkViewVeiwModel obj = new BBPNetworkViewVeiwModel();
            List<SelectListItem> PayoutDateFilter = new List<SelectListItem>();
            List<BoosterPlanPayout> planPayout = db.boosterPlanPayouts.OrderByDescending(p => p.ID).ToList();
            PayoutDateFilter.AddRange(new SelectList(planPayout.
                Select(p => new
                {
                    ID = p.ID,
                    FromDate = p.FromDate,
                    ToDate = p.ToDate,
                }).ToList()
                .Select(p => new
                {
                    ID = p.ID,
                    Name = p.FromDate.ToString("dd MMM yy") + " To " + p.ToDate.ToString("dd MMM yy")
                }), "ID", "Name").ToList());
            int LastId = Convert.ToInt16(PayoutDateFilter.FirstOrDefault().Value);
            obj = BD.GetData_Network(LastId, UserLoginId, 0, 0);
            obj.PayoutDateFilter = PayoutDateFilter;
            obj.PayoutDateFilterId = LastId;
            return View(obj);
        }

        [HttpPost]
        public ActionResult BBPNetwork(BBPNetworkViewVeiwModel obj)
        {
            long UserLoginId = Convert.ToInt64(Session["ID"]);
            List<SelectListItem> PayoutDateFilter = new List<SelectListItem>();
            List<BoosterPlanPayout> planPayout = db.boosterPlanPayouts.OrderByDescending(p => p.ID).ToList();
            PayoutDateFilter.AddRange(new SelectList(planPayout.
                Select(p => new
                {
                    ID = p.ID,
                    FromDate = p.FromDate,
                    ToDate = p.ToDate,
                }).ToList()
                .Select(p => new
                {
                    ID = p.ID,
                    Name = p.FromDate.ToString("dd MMM yy") + " To " + p.ToDate.ToString("dd MMM yy")
                }), "ID", "Name").ToList());
            obj = BD.GetData_Network(obj.PayoutDateFilterId, UserLoginId, 0, 0);
            obj.PayoutDateFilter = PayoutDateFilter;
            return View(obj);
        }

      
        public ActionResult BBPNetworkDetails(long BoosterPlanPayoutId, int level)
        {
            long UserLoginId = Convert.ToInt64(Session["ID"]);
            BBPNetworkViewVeiwModel obj = new BBPNetworkViewVeiwModel();
            obj = BD.GetData_Network(BoosterPlanPayoutId, UserLoginId, 1, level);
            return PartialView("_NetworkDetails", obj.details);
        }

        public ActionResult BBPOrderReport()
        {
            DateTime baseDate = DateTime.Today;
            DateTime thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            DateTime thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
            TempData["StartDtae"] = thisWeekStart.ToString("dd-MM-yyyy");
            TempData["EndDtae"] = thisWeekEnd.ToString("dd-MM-yyyy");
            long UserLoginId = Convert.ToInt64(Session["ID"]);
            BBPUserStatusData data = new BBPUserStatusData();
            data = BD.GetData_UserStatusReport(UserLoginId);
            return View(data);
        }
    }
}