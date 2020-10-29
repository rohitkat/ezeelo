using BusinessLogicLayer;
using ClosedXML.Excel;
using Leaders.Areas.Admin.Models;
using Leaders.Filter;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Leaders.Areas.Admin.Controllers
{
    public class BBPReportsController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        BoosterPlanData BD = new BoosterPlanData();
        #region Payout Report
        [AdminSessionExpire]
        public ActionResult Index()
        {
            if (Session["RoleName"].ToString() == "superadmin")
            {
                BBPViewModel obj = new BBPViewModel();
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
                        Name = p.ToDate.ToString("dd-MM-yyyy") + " To " + p.FromDate.ToString("dd-MM-yyyy")
                    }), "ID", "Name").ToList();
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
                    objData = GetData_Payout(obj.PayoutDateFilterID);
                    List<BoosterPlanPayout> planPayout = db.boosterPlanPayouts.OrderByDescending(p => p.ID).ToList();
                    objData.PayoutDateFilter = new SelectList(planPayout.
                        Select(p => new
                        {
                            ID = p.ID,
                            FromDate = p.FromDate,
                            ToDate = p.ToDate,
                        }).ToList()
                        .Select(p => new
                        {
                            ID = p.ID,
                            Name = p.ToDate.ToString("dd-MM-yyyy") + " To " + p.FromDate.ToString("dd-MM-yyyy")
                        }), "ID", "Name").ToList();
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

        public BBPViewModel GetData_Payout(long BoosterPayoutID)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@BoosterPlanPayoutID", SqlDbType = SqlDbType.BigInt, Value= BoosterPayoutID},
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("BoosterPlanPayoutReport", sp);
            BBPViewModel obj = new BBPViewModel();
            List<BBPPayoutViewModel> bBPPayout = new List<BBPPayoutViewModel>();
            List<BBPPayoutDetailsViewModel> details = new List<BBPPayoutDetailsViewModel>();
            List<BBPPayoutUserWiseViewModel> userwise = new List<BBPPayoutUserWiseViewModel>();
            List<BBPPayoutOrderWiseViewModel> orderwise = new List<BBPPayoutOrderWiseViewModel>();

            if (ds.Tables.Count > 0)
            {
                ds.Tables[0].TableName = "Payout";
                bBPPayout = BusinessLogicLayer.Helper.CreateListFromTable<BBPPayoutViewModel>(ds.Tables[0]);
                Session["BBPDateRangeReport"] = " from" + bBPPayout.FirstOrDefault().FromDate + " to " + bBPPayout.FirstOrDefault().ToDate;
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
            Session["BBPDataSetReport"] = ds;


            obj.bBPPayout = bBPPayout.FirstOrDefault();
            obj.List_details = details;
            obj.List_userwise = userwise;
            obj.List_orderwise = orderwise;
            return obj;
        }

        public ActionResult ExportToExcel()
        {
            string ReportName = "Business Booster Payout Report";
            if (Session["BBPDataSetReport"] != null)
            {
                DataSet dataSet = (Session["BBPDataSetReport"]) as DataSet;
                ReportName = ReportName+ " " + Session["BBPDateRangeReport"].ToString();
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
        #endregion

        #region Subscribers
        public List<BBPSubscribers> GetData_Subscribers(long BoosterPayoutID, DateTime FromDate, DateTime ToDate)
        {
            Session["SubscriberRptTodate"] = ToDate;
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@FromDate", SqlDbType = SqlDbType.Date, Value= FromDate},
                new SqlParameter() {ParameterName = "@ToDate", SqlDbType = SqlDbType.DateTime, Value= ToDate},
                new SqlParameter() {ParameterName = "@BoosterPayoutID", SqlDbType = SqlDbType.BigInt, Value= BoosterPayoutID}
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("BoosterPLanSubscribers", sp);
            List<BBPSubscribers> subscribers = new List<BBPSubscribers>();
            if (ds.Tables.Count > 0)
            {
                subscribers = BusinessLogicLayer.Helper.CreateListFromTable<BBPSubscribers>(ds.Tables[0]);
            }
            Session["BBPSubscriber"] = ds;
            return subscribers;
        }

        public BBPUserStatusData GetData_UserStatusReport(long UserLoginId)
        {
            DateTime Todate = Convert.ToDateTime(Session["SubscriberRptTodate"]);
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@UserLoginID", SqlDbType = SqlDbType.BigInt, Value= UserLoginId},
                new SqlParameter() {ParameterName = "@PayoutToDate", SqlDbType = SqlDbType.DateTime, Value= Todate}
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("BoosterUserStausDescriber", sp);
            BBPUserStatusData data = new BBPUserStatusData();
            List<BBPUserStatusReport> report = new List<BBPUserStatusReport>();
            List<BBPUserStatusResult> result = new List<BBPUserStatusResult>();
            List<BBPUserStatusOrdReport> order = new List<BBPUserStatusOrdReport>();
            if (ds.Tables.Count > 0)
            {
                report = BusinessLogicLayer.Helper.CreateListFromTable<BBPUserStatusReport>(ds.Tables[0]);
            }
            if (ds.Tables.Count > 1)
            {
                result = BusinessLogicLayer.Helper.CreateListFromTable<BBPUserStatusResult>(ds.Tables[1]);
            }
            if (ds.Tables.Count > 2)
            {
                order = BusinessLogicLayer.Helper.CreateListFromTable<BBPUserStatusOrdReport>(ds.Tables[2]);
            }
            data.order = order;
            data.result = result;
            data.report = report;
            return data;
        }
               

        [HttpPost]
        public ActionResult GetStatusReport(long UserLoginID)
        {            
            return Json(GetData_UserStatusReport(UserLoginID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportToExcel_Inactive()
        {
            string ReportName = "Business Booster Inactive Suscriber";
            if (Session["InactiveSubscriber"] != null)
            {
                var gv = new GridView();
                gv.DataSource = Session["InactiveSubscriber"] as List<BBPPayoutDetailsViewModel>;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=" + ReportName + ".xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
            }
            return RedirectToAction("Index");
        }

        [AdminSessionExpire]
        public ActionResult BBPInactiveSubscriber(int? PayoutDateFilterID, string Search)
        {
            if (Session["RoleName"].ToString() == "superadmin")
            {
                BBPDashboardViewModel objData = new BBPDashboardViewModel();
                DateTime FromDate = new DateTime();
                DateTime ToDate = new DateTime();
                int filterid = PayoutDateFilterID ?? 0;
                objData.PayoutDateFilterID = filterid;
                SetDate(objData.PayoutDateFilterID, out FromDate, out ToDate);
                objData.InactiveSubscriber = GetData_InactiveSubscribers( FromDate, ToDate,false);
                objData.PayoutDateFilter = BindFilter();
                objData.PayoutDateFilter = objData.PayoutDateFilter.Where(p => p.Value == "0" || p.Value == "-1").ToList();
                TempData["Search"] = Search;
                objData.Msg = " For uninterrupted Booster Payout Please ensure your 1000 RP purchase. Team eZeelo";
                return View(objData);
            }
            TempData["Result"] = "Unauthorized Access!!!";
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        [HttpPost]
        [AdminSessionExpire]
        [MultipleButton(Name = "action", Argument = "Report")]
        public ActionResult BBPInactiveSubscriber(BBPDashboardViewModel objData)
        {
            if (Session["RoleName"].ToString() == "superadmin")
            {
                DateTime FromDate = new DateTime();
                DateTime ToDate = new DateTime();
                SetDate(objData.PayoutDateFilterID, out FromDate, out ToDate);
                objData.InactiveSubscriber = GetData_InactiveSubscribers(FromDate, ToDate, objData.NearToInactiveStatus);
                objData.PayoutDateFilter = BindFilter();
                objData.PayoutDateFilter = objData.PayoutDateFilter.Where(p => p.Value == "0" || p.Value == "-1").ToList();
                return View("BBPInactiveSubscriber", objData);
            }
            TempData["Result"] = "Unauthorized Access!!!";
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        [HttpPost]
        [AdminSessionExpire]
        [MultipleButton(Name = "action", Argument = "SendSMS")]
        public ActionResult BBPInactiveSubscriber(BBPDashboardViewModel objData,string str)
        {
            if (Session["RoleName"].ToString() == "superadmin")
            {
                List<BBPPayoutDetailsViewModel> listDetail = new List<BBPPayoutDetailsViewModel>();
                listDetail = objData.InactiveSubscriber.Where(p => p.IsSelected == true).ToList();
                if (listDetail.Count == 0)
                {
                    TempData["BBPResult"] = "Plaese Select at least one Subscriber";
                }
                else if (objData.Msg == null || objData.Msg.Trim() == "")
                {
                    TempData["BBPResult"] = "Please add SMS text.";
                }
                else
                {
                    SendSMS(listDetail, objData.Msg);
                    TempData["BBPResult"] = "SMS sent successfully to " + listDetail.Count() + " users";
                }
                 return RedirectToAction("BBPInactiveSubscriber");
            }
            TempData["Result"] = "Unauthorized Access!!!";
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
        public void SendSMS(List<BBPPayoutDetailsViewModel> listDetail,string Msg)
        {
            try
            {
                foreach(var item in listDetail)
                {
                    item.Name = item.Name.Trim();
                    Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                    dictSMSValues.Add("#--NAME--#", item.Name);
                    dictSMSValues.Add("#--TEXT--#", Msg);

                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                    gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.KYC_CMPT_REQUEST, new string[] { item.Mobile }, dictSMSValues);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public List<BBPPayoutDetailsViewModel> GetData_InactiveSubscribers( DateTime FromDate, DateTime ToDate,bool NearToInactiveStatus)
        {
            List<BBPPayoutDetailsViewModel> listDetail = new List<BBPPayoutDetailsViewModel>();
            BBPPayoutController pay = new BBPPayoutController();
            BBPViewModel obj = new BBPViewModel();
            obj = pay.GetData(FromDate, ToDate, 0,false);
            listDetail = obj.List_details;
            if (!NearToInactiveStatus)
            {
                listDetail = listDetail.Where(p => p.Status == false).ToList();
            }
            else
            {
                List<SqlParameter> sp = new List<SqlParameter>()
                {
                new SqlParameter() {ParameterName = "@FromDate", SqlDbType = SqlDbType.Date, Value= FromDate},
                new SqlParameter() {ParameterName = "@ToDate", SqlDbType = SqlDbType.DateTime, Value= ToDate}
                };
                DataSet ds = new DataSet();
                ds = BD.GetData("BoosterSubscriberNearToInactiveStatus", sp);
                List<BBPPayoutDetailsViewModel> subscribers = new List<BBPPayoutDetailsViewModel>();
                if (ds.Tables.Count > 0)
                {
                    subscribers = BusinessLogicLayer.Helper.CreateListFromTable<BBPPayoutDetailsViewModel>(ds.Tables[0]);
                }
                listDetail = listDetail.Where(q => subscribers.Select(p => p.UserloginId).Contains(q.UserloginId)).ToList();
                listDetail.AddRange(subscribers.Where(q => !listDetail.Select(p => p.UserloginId).Contains(q.UserloginId)).ToList());
            }
            Session["InactiveSubscriber"] = listDetail;
            return listDetail;
        }

        [AdminSessionExpire]
        public ActionResult BBPSubscribers(int? PayoutDateFilterID, string Search)
        {
            if (Session["RoleName"].ToString() == "superadmin")
            {
                BBPDashboardViewModel objData = new BBPDashboardViewModel();
                DateTime FromDate = new DateTime();
                DateTime ToDate = new DateTime();
                int filterid = PayoutDateFilterID ?? 0;
                objData.PayoutDateFilterID = filterid;
                SetDate(objData.PayoutDateFilterID, out FromDate,out ToDate);
                objData.Subscribers = GetData_Subscribers(objData.PayoutDateFilterID, FromDate, ToDate);
                objData.FromDate = FromDate.ToString("dd-MM-yyyy");
                objData.ToDate = ToDate.ToString("dd-MM-yyyy");
                objData.PayoutDateFilter = BindFilter();
                TempData["Search"] = Search;
                return View(objData);
            }
            TempData["Result"] = "Unauthorized Access!!!";
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        [HttpPost]
        [AdminSessionExpire]
        public ActionResult BBPSubscribers(BBPDashboardViewModel objData)
        {
            if (Session["RoleName"].ToString() == "superadmin")
            {
                DateTime FromDate = new DateTime();
                DateTime ToDate = new DateTime();
                SetDate(objData.PayoutDateFilterID, out FromDate, out ToDate);
                objData.Subscribers = GetData_Subscribers(objData.PayoutDateFilterID, FromDate, ToDate);
                objData.FromDate = FromDate.ToString("dd-MM-yyyy");
                objData.ToDate = ToDate.ToString("dd-MM-yyyy");
                objData.PayoutDateFilter = BindFilter();
                return View(objData);
            }
            TempData["Result"] = "Unauthorized Access!!!";
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        public ActionResult ExportToExcel_Subscriber()
        {
            string ReportName = "Business Booster Subscriber";
            if (Session["BBPSubscriber"] != null)
            {
                DataSet dataSet = (Session["BBPSubscriber"]) as DataSet;
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
        #endregion

        #region Orders
        public List<BBPOrders> GetData_Orders(long BoosterPayoutID, DateTime FromDate, DateTime ToDate)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@FromDate", SqlDbType = SqlDbType.Date, Value= FromDate},
                new SqlParameter() {ParameterName = "@ToDate", SqlDbType = SqlDbType.DateTime, Value= ToDate},
                new SqlParameter() {ParameterName = "@BoosterPlanPayoutID", SqlDbType = SqlDbType.BigInt, Value= BoosterPayoutID}
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("BoosterPlanOrder", sp);
            List<BBPOrders> orders = new List<BBPOrders>();
            if (ds.Tables.Count > 0)
            {
                orders = BusinessLogicLayer.Helper.CreateListFromTable<BBPOrders>(ds.Tables[0]);
            }
            Session["BBPOrders"] = ds;
            return orders;
        }

        public ActionResult ExportToExcel_Orders()
        {
            string ReportName = "Business Booster Orders";
            if (Session["BBPOrders"] != null)
            {
                DataSet dataSet = (Session["BBPOrders"]) as DataSet;
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
        public ActionResult BBPOrders(int? PayoutDateFilterID,string Search)
        {
            if (Session["RoleName"].ToString() == "superadmin")
            {
                BBPDashboardViewModel objData = new BBPDashboardViewModel();
                DateTime FromDate = new DateTime();
                DateTime ToDate = new DateTime();
                int filterid = PayoutDateFilterID ?? 0;
                objData.PayoutDateFilterID = filterid;
                SetDate(objData.PayoutDateFilterID, out FromDate, out ToDate);
                objData.Orders = GetData_Orders(objData.PayoutDateFilterID, FromDate, ToDate);
                objData.PayoutDateFilter = BindFilter();
                TempData["Search"] = Search;
                return View(objData);
            }
            TempData["Result"] = "Unauthorized Access!!!";
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        [HttpPost]
        [AdminSessionExpire]
        public ActionResult BBPOrders(BBPDashboardViewModel objData)
        {
            if (Session["RoleName"].ToString() == "superadmin")
            {
                DateTime FromDate = new DateTime();
                DateTime ToDate = new DateTime();
                SetDate(objData.PayoutDateFilterID, out FromDate, out ToDate);
                objData.Orders = GetData_Orders(objData.PayoutDateFilterID, FromDate, ToDate);
                objData.PayoutDateFilter = BindFilter();
                return View(objData);
            }
            TempData["Result"] = "Unauthorized Access!!!";
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

       
        #endregion

        public void SetDate(long BoosterPayoutID, out DateTime FromDate, out DateTime ToDate)
        {
            DateTime baseDate = DateTime.Today;
            FromDate = DateTime.Today;
            ToDate = DateTime.Today;
            if (BoosterPayoutID <= 0)
            {
                var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                var lastWeekStart = thisWeekStart.AddDays(-7);
                var lastWeekEnd = thisWeekStart.AddSeconds(-1);
                switch (BoosterPayoutID)
                {
                    case 0:
                        FromDate = thisWeekStart;
                        ToDate = thisWeekEnd;
                        break;
                    case -1:
                        FromDate = lastWeekStart;
                        ToDate = lastWeekEnd;
                        break;
                    case -2:
                        FromDate = lastWeekStart;
                        ToDate = thisWeekEnd;
                        break;
                }
            }
        }

        public List<SelectListItem> BindFilter()
        {
            List<SelectListItem> PayoutDateFilter = new List<SelectListItem>();
            var Current = new SelectListItem { Text = "Current Week", Value = "0", Selected = true };
            var Pending = new SelectListItem { Text = "Pending Week", Value = "-1" };
            var All = new SelectListItem { Text = "All", Value = "-2" };
            PayoutDateFilter = new List<SelectListItem>();
            PayoutDateFilter.Add(All);
            PayoutDateFilter.Add(Current);
            PayoutDateFilter.Add(Pending);
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
            return PayoutDateFilter;
        }

        [AdminSessionExpire]
        public ActionResult BBPInactivePointReport()
        {
            if (Session["RoleName"].ToString() == "superadmin")
            {
                BBPViewModel obj = new BBPViewModel();
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
                obj.List_InactivePoints = GetData_InactivePointReport(null, null);
                return View(obj);
            }
            TempData["Result"] = "Unauthorized Access!!!";
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        public List<BBPInactivePoints> GetData_InactivePointReport(long? Id, int? Pay)
        {
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@BoosterPlanPayoutId", SqlDbType = SqlDbType.BigInt, Value= Id},
                new SqlParameter() {ParameterName = "@IsPaid", SqlDbType = SqlDbType.Int, Value= Pay}
            };
            DataSet ds = new DataSet();
            ds = BD.GetData("BBPInactivePointsPayoutReport", sp);
            Session["BBPInactivePointRpt"] = ds;
            List<BBPInactivePoints> details = new List<BBPInactivePoints>();
            Session["BBPInactivePoints"] = ds;
            if (ds.Tables.Count > 0)
            {
                ds.Tables[0].TableName = "Inactive Points Payout Report";
                details = BusinessLogicLayer.Helper.CreateListFromTable<BBPInactivePoints>(ds.Tables[0]);
            }
            return details;
        }

        [HttpPost]
        [AdminSessionExpire]
        public ActionResult BBPInactivePointReport(BBPViewModel obj)
        {
            if (Session["RoleName"].ToString() == "superadmin")
            {
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
                long? Id = null;
                int? IsPaid = null;
                Id = obj.FilterID == 0 ? Id : obj.FilterID;
                IsPaid = obj.OnlyPaid == false ? IsPaid : 1;
                obj.List_InactivePoints = GetData_InactivePointReport(Id, IsPaid);
                return View(obj);
            }
            TempData["Result"] = "Unauthorized Access!!!";
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        public ActionResult ExportToExcel_InactivePoints()
        {
            string ReportName = "Business Booster Inactive Points";
            if (Session["BBPInactivePoints"] != null)
            {
                DataSet dataSet = (Session["BBPInactivePoints"]) as DataSet;
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
    }
}