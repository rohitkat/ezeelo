using BusinessLogicLayer;
using ClosedXML.Excel;
using Leaders.Filter;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;

namespace Leaders.Areas.Admin.Controllers
{
    public class BBPPayoutController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        BoosterPlanData BD = new BoosterPlanData();
        [AdminSessionExpire]
        public ActionResult Index()
        {
            if (Session["RoleName"].ToString() == "superadmin")
            {
                BBPViewModel obj = new BBPViewModel();
                BoosterPlanPayout planPayout = db.boosterPlanPayouts.ToList().LastOrDefault();
                if (planPayout == null)
                {
                    obj.FromDate = new DateTime(2019, 8, 1);
                    obj.ToDate = new DateTime(2019, 8, 1).AddDays(7);
                }
                else
                {
                    obj.LastFromDate = planPayout.FromDate;
                    obj.LastToDate = planPayout.ToDate;
                    obj.FromDate = planPayout.ToDate.AddDays(1);
                    obj.ToDate = obj.FromDate.AddDays(6);
                }
                return View(obj);
            }
            TempData["Result"] = "Unauthorized Access!!!";
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        [AdminSessionExpire]
        [HttpPost]
        public ActionResult Index(BBPViewModel obj)
        {
            if (Session["RoleName"].ToString() == "superadmin")
            {
                try
                {
                    BBPViewModel objData = new BBPViewModel();
                    objData = GetData(obj.FromDate, obj.ToDate, 0, true);
                    objData.bBPPayout.FromDate = obj.FromDate.ToString("dd/MM/yyyy");
                    objData.bBPPayout.ToDate = obj.ToDate.ToString("dd/MM/yyyy");
                    return View(objData);
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

        public BBPViewModel GetData(DateTime FromDate, DateTime ToDate, int Pay, bool flag)
        {
            if (flag)
            {
                Session["BBPFromDate"] = FromDate;
                Session["BBPToDate"] = ToDate;
                ToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@FromDate", SqlDbType = SqlDbType.Date, Value= FromDate},
                new SqlParameter() {ParameterName = "@ToDate", SqlDbType = SqlDbType.DateTime, Value= ToDate},
                new SqlParameter() {ParameterName = "@Pay", SqlDbType = SqlDbType.BigInt, Value= Pay},

            };
            DataSet ds = new DataSet();
            ds = BD.GetData("BoosterPlanPayoutProc", sp);
            BBPViewModel obj = new BBPViewModel();
            List<BBPPayoutViewModel> bBPPayout = new List<BBPPayoutViewModel>();
            List<BBPPayoutDetailsViewModel> details = new List<BBPPayoutDetailsViewModel>();
            List<BBPPayoutUserWiseViewModel> userwise = new List<BBPPayoutUserWiseViewModel>();
            List<BBPPayoutOrderWiseViewModel> orderwise = new List<BBPPayoutOrderWiseViewModel>();
            if (Pay == 0)
            {
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "Payout";
                    bBPPayout = BusinessLogicLayer.Helper.CreateListFromTable<BBPPayoutViewModel>(ds.Tables[0]);
                }
                if (ds.Tables.Count > 1)
                {
                    ds.Tables[1].TableName = "Payout Details";
                    details = BusinessLogicLayer.Helper.CreateListFromTable<BBPPayoutDetailsViewModel>(ds.Tables[1]);
                }
                if (ds.Tables.Count > 2)
                {
                    ds.Tables[2].TableName = "Payout Userwise Details";
                    userwise = BusinessLogicLayer.Helper.CreateListFromTable<BBPPayoutUserWiseViewModel>(ds.Tables[2]);
                }
                if (ds.Tables.Count > 3)
                {
                    ds.Tables[3].TableName = "Payout Orderwise Details";
                    orderwise = BusinessLogicLayer.Helper.CreateListFromTable<BBPPayoutOrderWiseViewModel>(ds.Tables[3]);
                }
                if (flag)
                {
                    Session["BBPDataSet"] = ds;

                    Session["BBPDateRange"] = " from" + FromDate.ToString("dd-MM-yyyy") + " to " + ToDate.ToString("dd-MM-yyyy");
                }
            }
            obj.bBPPayout = bBPPayout.FirstOrDefault();
            obj.List_details = details;
            obj.List_userwise = userwise;
            obj.List_orderwise = orderwise;
            return obj;
        }

        public ActionResult Payout()
        {
            try
            {
                if (Session["BBPFromDate"] != null)
                {
                    DateTime FromDate = Convert.ToDateTime(Session["BBPFromDate"]);
                    DateTime ToDate = Convert.ToDateTime(Session["BBPToDate"]);
                    BBPViewModel objData = new BBPViewModel();
                    objData = GetData(FromDate, ToDate, 1, true);
                    TempData["BBPResult"] = "Paid Successfully!!!";
                }
            }
            catch (Exception ex)
            {
                TempData["BBPResult"] = ex.Message;
            }
            Session["BBPDataSet"] = null;
            Session["BBPFromDate"] = null;
            Session["BBPToDate"] = null;
            Session["BBPDateRange"] = null;
            return RedirectToAction("Index");
        }

        public ActionResult ExportToExcel()
        {
            string ReportName = "Business Booster Payout Report";
            if (Session["BBPDataSet"] != null)
            {
                DataSet dataSet = (Session["BBPDataSet"]) as DataSet;
                ReportName = ReportName + " " + Session["BBPDateRange"].ToString();
                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in dataSet.Tables)
                    {
                        wb.Worksheets.Add(dt);
                    }
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=" + ReportName + ".xlsx");
                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [AdminSessionExpire]
        public ActionResult BBPInactivePointPayout()
        {
            if (Session["RoleName"].ToString() == "superadmin")
            {
                BBPViewModel obj = new BBPViewModel();
                List<BoosterPlanPayout> planPayout = db.boosterPlanPayouts.OrderByDescending(p => p.ID).ToList();
                ViewBag.PayableBBPId = planPayout.FirstOrDefault().ID;
                obj.PayoutDateFilter = new SelectList(planPayout.
                    Select(p => new
                    {
                        ID = p.ID,
                        FromDate = p.FromDate,
                        ToDate = p.ToDate,
                    }).ToList()
                    .Select(p => new
                    {
                        ID = p.ID,
                        Name = p.FromDate.ToString("dd-MM-yyyy") + " To " + p.ToDate.ToString("dd-MM-yyyy")
                    }), "ID", "Name").ToList();
                return View(obj);
            }
            TempData["Result"] = "Unauthorized Access!!!";
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        [AdminSessionExpire]
        [HttpPost]
        public ActionResult BBPInactivePointPayout(BBPViewModel obj, string GO, string Payout)
        {
            int Pay = 0;
            if (Payout != null)
            {
                Pay = 1;
                TempData["BBPResult"] = "Paid Successfully";
            }
            List<BoosterPlanPayout> planPayout = db.boosterPlanPayouts.OrderByDescending(p => p.ID).ToList();
            obj.PayoutDateFilter = new SelectList(planPayout.
                Select(p => new
                {
                    ID = p.ID,
                    FromDate = p.FromDate,
                    ToDate = p.ToDate,
                }).ToList()
                .Select(p => new
                {
                    ID = p.ID,
                    Name = p.FromDate.ToString("dd-MM-yyyy") + " To " + p.ToDate.ToString("dd-MM-yyyy")
                }), "ID", "Name").ToList();

            if (planPayout.FirstOrDefault().ID != obj.PayoutDateFilterID)
            {
                Pay = 0;
            }
            obj.List_details = GetData_InactivePointPayoutReport(obj.PayoutDateFilterID, Pay);
            ViewBag.PayableBBPId = planPayout.FirstOrDefault().ID;
            if(Pay == 1)
            {
                obj.List_details = new List<BBPPayoutDetailsViewModel>();
            }
            return View(obj);
        }

        public List<BBPPayoutDetailsViewModel> GetData_InactivePointPayoutReport(long Id, int Pay)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@BoosterPlanPayoutId", SqlDbType = SqlDbType.BigInt, Value= Id},
                new SqlParameter() {ParameterName = "@Pay", SqlDbType = SqlDbType.Int, Value= Pay}
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("BoosterPlanInactivePointPayout", sp);
            Session["BBPInactivePointRpt"] = ds;
            List<BBPPayoutDetailsViewModel> details = new List<BBPPayoutDetailsViewModel>();

            if (ds.Tables.Count > 0)
            {
                ds.Tables[0].TableName = "Inactive Points Payout Report";
                details = BusinessLogicLayer.Helper.CreateListFromTable<BBPPayoutDetailsViewModel>(ds.Tables[0]);
            }
            return details;
        }

        public ActionResult ExportToExcel_InactivePoint()
        {
            string ReportName = "Business Booster Payout Inactive Point Report";
            if (Session["BBPInactivePointRpt"] != null)
            {
                DataSet dataSet = (Session["BBPInactivePointRpt"]) as DataSet;
                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in dataSet.Tables)
                    {
                        wb.Worksheets.Add(dt);
                    }
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=" + ReportName + ".xlsx");
                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            return RedirectToAction("BBPInactivePointPayout");
        }

    }
}