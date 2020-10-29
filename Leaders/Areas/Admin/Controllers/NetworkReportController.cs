using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System.Data.SqlClient;
using Leaders.Controllers;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Globalization;
using System.Data;
using System.Web.Configuration;
using BusinessLogicLayer;
using Leaders.Filter;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class NetworkReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private LeadersDashboard objLeaderDashboard = new LeadersDashboard();//Added by Sonali for call common function on 04-02-2018
        // long MainUserLoginID = 0;
        public double getERP(long LoginUserId)
        {
            DashboardController objDashboardController = new DashboardController();
            objDashboardController.InitializeController(this.Request.RequestContext);
            try
            {
                // long LoginUserId = 0;
                if (LoginUserId != null)
                {
                    LoginUserId = Convert.ToInt64(LoginUserId);
                }
                else
                {
                    LoginUserId = 0;
                }
                double Result = 0;
                Result = objLeaderDashboard.getERP_User((int)LoginUserId, 0);//Added by Sonali for call common function 04-02-2019
                Result = Math.Round(Result, 2);
                return (Result == 0) ? 0.00d : Result;
            }
            catch
            {

            }
            return 0;
        }

        public List<NetworkReportViewModel> GetNetworkReportList(int? id)
        {

            int currentMonth = DateTime.Now.Month;

            DashboardController objDashboard = new DashboardController();
            objDashboard.InitializeController(this.Request.RequestContext);

            var idParam = new SqlParameter
            {
                ParameterName = "UserID",
                Value = id

            };

            db.Database.ExecuteSqlCommand("Leaders_NetworkUsers_New @UserID", idParam);
            // var networkList = db.Database.SqlQuery<NetworkUserViewModel>("EXEC Leaders_NetworkUsers @UserID ", idParam).ToList<NetworkUserViewModel>();
            db.SaveChanges();

            var report2List = db.Database.SqlQuery<NetworkReportViewModel>("EXEC Leaders_Report2Query").ToList<NetworkReportViewModel>().GroupBy(y => y.UserID).Select(group => group.FirstOrDefault());

            ViewBag.MemberCount = report2List.Count();

            foreach (var item in report2List)
            {
                long userLoginID = item.UserID ?? 0;
                item.FullName = db.PersonalDetails.Where(x => x.UserLoginID == item.UserID).Select(y => y.FirstName + ""  + (y.LastName==null?"": y.LastName)).FirstOrDefault() ?? "NA";
                item.Mobile = db.UserLogins.Where(x => x.ID == item.UserID).Select(y => y.Mobile).FirstOrDefault() ?? "NA";
                item.UserEmail = db.UserLogins.Where(x => x.ID == item.UserID).Select(y => y.Email).FirstOrDefault() ?? "NA";
                var referedID = db.MLMUsers.Where(x => x.UserID == item.UserID).Select(y => y.Refered_Id_ref).FirstOrDefault();
                var parentID = db.MLMUsers.Where(x => x.Ref_Id == referedID).Select(y => y.UserID).FirstOrDefault();
                // long userLoginID = (item.UserID??0);
                item.ReferalID = db.MLMUsers.Where(x => x.UserID == item.UserID).Select(t => t.Ref_Id).FirstOrDefault();
                item.ParentName = db.PersonalDetails.Where(x => x.UserLoginID == parentID).Select(y => y.FirstName + "" + (y.LastName==null?"":y.LastName)).FirstOrDefault();
                item.PendingPoints = (objLeaderDashboard.GetQRPMasterValue() - GetRpOnPurchase(userLoginID));
                item.LastTransaction = db.MLMWalletTransactions.Where(x => x.UserLoginID == item.UserID && x.TransactionTypeID == 1).OrderByDescending(x => x.CreateDate).Select(y => y.CreateDate).FirstOrDefault();
                item.JoinDate = db.MLMUsers.Where(x => x.UserID == item.UserID).Select(y => y.Join_date_ref).FirstOrDefault();
                // item.RPOnMyPurchase = db.MLMWalletTransactions.Where(p => p.UserLoginID == item.UserID && p.CreateDate.Month == currentMonth && p.TransactionTypeID == 7 && p.OrderAmount > 0).Sum(y => y.TransactionPoints);
                item.RPOnMyPurchase = GetRpOnPurchase(userLoginID);

                item.ERP = Convert.ToDecimal(getERP(userLoginID));
                item.ReferalID = db.MLMUsers.Where(x => x.UserID == item.UserID).Select(y => y.Ref_Id).FirstOrDefault();
                item.InActivePoints =Convert.ToDecimal(GetInactivePoints(userLoginID));
                // GetRpOnPurchase(item.UserID);

            }
            return report2List.ToList();
        }
        public ActionResult Index(int? id)
        {
            int currentMonth = DateTime.Now.Month;
            Session["ObjUserLoginID"] = id;

            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            year = (month == 1) ? (year - 1) : year;

            DateTime FromDate = new DateTime(year, DateTime.Now.Month, 1);
            string startDate = FromDate.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);

            DateTime ToDate = FromDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            
            string endDate = ToDate.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);

            DashboardController objDashboard = new DashboardController();
            objDashboard.InitializeController(this.Request.RequestContext);
            ViewBag.UserName = db.PersonalDetails.Where(x => x.UserLoginID == id).Select(y => y.FirstName + "" + y.MiddleName + "" + y.LastName).FirstOrDefault();
            ViewBag.Email = db.UserLogins.Where(x => x.ID == id).Select(y => y.Email).FirstOrDefault();
            ViewBag.Mobile = db.UserLogins.Where(x => x.ID == id).Select(y => y.Mobile).FirstOrDefault();
            ViewBag.DateRange = startDate + "-" + endDate;
            ViewBag.Designation = UserDesignation(Convert.ToInt32(id));
            List<NetworkReportViewModel> obj = new List<NetworkReportViewModel>();
            obj = GetNetworkReportList(id);
            return View(obj);


        }


        public string UserDesignation(int LoginUserId)
        {
            string Designation = string.Empty;
            long value = 0;
            try
            {
                value = db.MLMUsers.FirstOrDefault(x => x.UserID == LoginUserId).CURRENTMONTHDESIGNTAIONID;
                if (value == 0)
                {
                    Designation = "";
                }
                else
                {
                    Designation = db.DesignationMaster.FirstOrDefault(x => x.Id == value).Designation;

                    return Designation;
                }
            }
            catch (Exception ex)
            {
            }
            return Designation;
        }


        public double GetInactivePoints(long LoginUserId)
        {
            try
            {
                //long LoginUserId = 0;
                
                double Result = 0;
                try
                {
                    int year = DateTime.Now.Year;
                    int month = DateTime.Now.Month;
                    year = (month == 1) ? (year - 1) : year;
                    DateTime FromDate = new DateTime(year, DateTime.Now.Month - 1, 1);
                    DateTime ToDate = FromDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
                    var idParam = new SqlParameter
                    {
                        ParameterName = "LoginUserId",
                        Value = LoginUserId
                    };
                    var Hour = new SqlParameter
                    {
                        ParameterName = "Hour",
                        Value = (new LeadersDashboard()).getHour()//WebConfigurationManager.AppSettings["Del_Hour"]
                    };
                    var DateFrom = new SqlParameter
                    {
                        ParameterName = "DateFrom",
                        Value = ToDate
                    };
                    var DateTo = new SqlParameter
                    {
                        ParameterName = "DateTo",
                        Value = FromDate
                    };
                    var TotalPoints = new SqlParameter
                    {
                        ParameterName = "TotalPoints",
                        Direction = ParameterDirection.Output,
                        DbType = DbType.Decimal,
                        Precision = 18,
                        Scale = 4
                    };
                    db.Database.ExecuteSqlCommand("Leaders_SingleUser_InactivePoints @LoginUserId ,@DateTo,@DateFrom,@Hour,@TotalPoints output", idParam, DateTo, DateFrom, Hour, TotalPoints);
                    if (TotalPoints != null)
                    {
                        Result = Convert.ToDouble(TotalPoints.Value);
                    }
                }
                catch (Exception ex)
                {
                }
                return Math.Round(Result, 2);
            }
            catch
            {
                return 0;
            }
        }

        public decimal GetRpOnPurchase(long UserLoginID)
        {
            try
            {
                long LoginUserId = 0;
                if (UserLoginID != null)
                {
                    LoginUserId = Convert.ToInt64(UserLoginID);
                }
                decimal Result = 0;

                int currentMonth = DateTime.Now.Month;
                List<MLMWalletTransaction> obj = new List<MLMWalletTransaction>();
                if (currentMonth == 8)
                {
                    obj = db.MLMWalletTransactions.Where(p => (p.CreateDate.Month == currentMonth || p.CreateDate.Month == currentMonth - 1) && p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0).ToList();
                }
                else
                {
                    obj = db.MLMWalletTransactions.Where(p => p.CreateDate.Month == currentMonth && p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0).ToList();
                }
                if (obj != null)
                {
                    Result = obj.Sum(p => p.TransactionPoints);
                }
                else
                {
                    Result = 0;
                }
                Result = Math.Round(Result, 2);
                return (Result == 0) ? 0.00M : Result;
            }
            catch
            {

            }
            return 0;
        }

        public ActionResult ExportToExcel()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            year = (month == 1) ? (year - 1) : year;
            DateTime FromDate = new DateTime(year, DateTime.Now.Month, 1);
            DateTime ToDate = FromDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

            int userLoginID = Convert.ToInt32(Session["ObjUserLoginID"].ToString());
            List<NetworkReportViewModel> leadersList = GetNetworkReportList(userLoginID);
            string UserName=db.PersonalDetails.Where(x=>x.UserLoginID==userLoginID).Select(y=>y.FirstName+""+(y.LastName==null?"": y.LastName)).FirstOrDefault();

            var gv = new GridView();
            gv.DataSource = leadersList;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=NetworkReportExcel.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            string headerTable = @"<Table><tr><td>Network Report of" + "" + UserName + "</td></tr><tr><td></td></tr></Table>";
            Response.Write(headerTable);
            Response.Output.Write(objStringWriter.ToString());

            Response.Flush();
            Response.End();
            return View("Index");
        }
	}
}