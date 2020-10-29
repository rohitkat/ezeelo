using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.Helpers;
using Leaders.Filter;
using Newtonsoft.Json;
using System.Web.Routing;

namespace Leaders.Controllers
{
    public class DashboardController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private LeadersDashboard objLeaderDashboard = new LeadersDashboard();
        public void InitializeController(RequestContext context)
        {
            base.Initialize(context);
        }


        [SessionExpire]
        public ActionResult Index()
        {
            InviteUser objInviteUser = new InviteUser();
            objInviteUser.Message = "Hi!, i would like to invite you to this awesome scheme launched by ezeelo.com check the link in email for more info";

            HighPerformer objHighPerformer = new HighPerformer();
            objHighPerformer.searchId = 1;
            objHighPerformer.SearchParameter = new SelectList(new[]
            {
                new { ID = "1", Name = "Top Recruiters" },
                new { ID = "2", Name = "Top Buyers" },
                new { ID = "3", Name = "Top Earners" },
            },
            "ID", "Name", 1);

            RefferalByFilter objRefferalByFilter = new RefferalByFilter();
            objRefferalByFilter.searchId = 1;
            objRefferalByFilter.SearchParameter = new SelectList(new[]
            {
                new { ID = "1", Name = "This Month" },
                new { ID = "2", Name = "Last 5 Month" }
            },
            "ID", "Name", 1);



            objHighPerformer.listHighPerformerViewModel = new List<HighPerformerViewModel>();
            long LoginUserId = 0;
            if (Session["ID"] != null)
            {

                LoginUserId = Convert.ToInt64(Session["ID"]);
            }

            DashboardViewModel objDashboardViewModel = new DashboardViewModel();

            DashboardViewModel ObjDb = new DashboardViewModel();
            ObjDb = objLeaderDashboard.GetallDashboarddatabysp(LoginUserId);
            objDashboardViewModel.Referrals = ObjDb.Referrals;
            objDashboardViewModel.Referrals_5Month = ObjDb.Referrals_5Month;
            objDashboardViewModel.TOTAL_MEMBERS = ObjDb.TOTAL_MEMBERS;
            objDashboardViewModel.QUALIFYING_RETAIL_POINTS = ObjDb.QUALIFYING_RETAIL_POINTS;
            objDashboardViewModel.Withdrawn = ObjDb.Withdrawn;
            objDashboardViewModel.ERP = ObjDb.ERP;
            objDashboardViewModel.RP_ON_MY_PURCHASE = ObjDb.RP_ON_MY_PURCHASE;
            objDashboardViewModel.INACTIVE_MEMBERS = ObjDb.INACTIVE_MEMBERS;
            objDashboardViewModel.Payout_Requested = ObjDb.Payout_Requested;
            objDashboardViewModel.EZEE_MONEY = ObjDb.EZEE_MONEY;
            objDashboardViewModel.UserDesignation = ObjDb.UserDesignation;

            objDashboardViewModel.CasbackPoints = ObjDb.CasbackPoints;
            objDashboardViewModel.CasbackEzeeMoney = ObjDb.CasbackEzeeMoney;

            // objDashboardViewModel.Referrals = 0;
            // objDashboardViewModel.Withdrawn = objLeaderDashboard.GetWithdrawnAmount(LoginUserId);
            // objDashboardViewModel.ERP = objLeaderDashboard.getERP(LoginUserId);
            // objDashboardViewModel.Payout_Requested = objLeaderDashboard.GetPayout_Requested(LoginUserId);
            // objDashboardViewModel.TOTAL_MEMBERS = objLeaderDashboard.GetTotalMemberCount(LoginUserId);
            // objDashboardViewModel.EZEE_MONEY = objLeaderDashboard.GetEzzeMoney(LoginUserId);
            // objDashboardViewModel.QUALIFYING_RETAIL_POINTS = objLeaderDashboard.GetQRP(LoginUserId);
            //   objDashboardViewModel.RP_ON_MY_PURCHASE = objLeaderDashboard.GetRpOnPurchase(LoginUserId);
            //  objDashboardViewModel.INACTIVE_MEMBERS = objLeaderDashboard.GetInactiveMemberCount(LoginUserId);
            // objLeaderDashboard.Distribute_Designation(Convert.ToInt32(LoginUserId));
            // objDashboardViewModel.UserDesignation = objLeaderDashboard.UserDesignation(Convert.ToInt32(LoginUserId));

            objDashboardViewModel.Inactive_Points = objLeaderDashboard.GetInactivePoints(LoginUserId);
            objDashboardViewModel.Pending_EzeeMoney = objLeaderDashboard.GetPending_EzeeMoney(LoginUserId);

            objDashboardViewModel.DAYS_LEFT = objLeaderDashboard.GetDayLeft();
            objDashboardViewModel.CYCLE_START_DATE = objLeaderDashboard.GetCycleStartDate();
            objDashboardViewModel.CYCLE_LAST_DATE = objLeaderDashboard.GetCycleLastDate();
            objDashboardViewModel.MY_PURCHASES = objLeaderDashboard.GetMyPurchaseCount(LoginUserId);
            objDashboardViewModel.EXPECTED_ERP = objLeaderDashboard.ExpectedERP(LoginUserId);

            objDashboardViewModel.objInviteUser = objInviteUser;
            objDashboardViewModel.listRecentJoinees = objLeaderDashboard.GetRecentJoinees(LoginUserId);
            objDashboardViewModel.objHighPerformer = objHighPerformer;
            objDashboardViewModel.objRefferalByFilter = objRefferalByFilter;
            objDashboardViewModel.isUserActive = objLeaderDashboard.IsUserActive(LoginUserId);



            //Start 31/10/2018  For Graphics display only in Oct month Payout
            bool UserStatus = CheckUserStatus();
            //EzeeMoneyPayout objEzeeMoneyPayout = db.EzeeMoneyPayouts.FirstOrDefault(p => p.Id == 4 && p.IsPaid == true);
            //if (objEzeeMoneyPayout != null)
            //{
            //    UserStatus = false;
            //}
            if (UserStatus == true)
            {
                ViewBag.UserStatus = "1";
            }
            else
            {
                ViewBag.UserStatus = "0";
            }
            //End 31/10/2018  For Graphics display only in Oct month Payout
            return View(objDashboardViewModel);
        }


        #region Dashboard Count
        //public decimal GetWithdrawnAmount()
        //{
        //    try
        //    {
        //        long LoginUserId = 0;
        //        if (Session["ID"] != null)
        //        {
        //            LoginUserId = Convert.ToInt64(Session["ID"]);
        //        }

        //        List<LeadersPayoutRequest> obj = db.LeadersPayoutRequests.Where(p => p.RequestStatus == 2 && p.UserLoginID == LoginUserId).ToList();
        //        if (obj != null && obj.Count != 0)
        //        {
        //            return (decimal)obj.Sum(p => p.RequestedAmount);
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //    catch { return 0; }
        //}

        //public bool IsUserActive()
        //{
        //    try
        //    {
        //        int Hour = Convert.ToInt16(WebConfigurationManager.AppSettings["Del_Hour"]);

        //        long LoginUserId = 0;
        //        if (Session["ID"] != null)
        //        {
        //            LoginUserId = Convert.ToInt64(Session["ID"]);
        //        }
        //        int currentMonth = DateTime.Now.Month;
        //        decimal CurrentQRP = GetQRPMasterValue();
        //        decimal Result = 0;
        //        DateTime CurrentDate = DateTime.Now;
        //        List<MLMWalletTransaction> obj = new List<MLMWalletTransaction>();

        //        if (currentMonth == 8)
        //        {
        //            obj = db.MLMWalletTransactions.Where(p => (p.CreateDate.Month == currentMonth || p.CreateDate.Month == currentMonth - 1) && p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0 && System.Data.Entity.DbFunctions.AddHours(p.CreateDate, Hour) <= CurrentDate).ToList();
        //        }
        //        else
        //        {
        //            obj = db.MLMWalletTransactions.Where(p => p.CreateDate.Month == currentMonth && p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0 && System.Data.Entity.DbFunctions.AddHours(p.CreateDate, Hour) <= CurrentDate).ToList();
        //        }

        //        if (obj != null)
        //        {
        //            Result = obj.Sum(p => p.TransactionPoints);
        //        }
        //        else
        //        {
        //            Result = 0;
        //        }
        //        if (CurrentQRP > Result)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //    catch { }
        //    return false;
        //}

        //public double GetInactivePoints()
        //{
        //    try
        //    {
        //        long LoginUserId = 0;
        //        if (Session["ID"] != null)
        //        {
        //            LoginUserId = Convert.ToInt64(Session["ID"]);
        //        }

        //        //double Result = getERPBySP(LoginUserId, 1, 0, 0);

        //        //int currentMonth = DateTime.Now.Month;
        //        //List<MLMWalletTransaction> obj = new List<MLMWalletTransaction>();
        //        ////if (currentMonth == 9)
        //        ////{
        //        ////    obj = db.MLMWalletTransactions.Where(p => (p.CreateDate.Month == currentMonth - 1 || p.CreateDate.Month == currentMonth - 2) && p.TransactionTypeID == 11 && p.UserLoginID == LoginUserId && p.IsAdded == false).ToList();
        //        ////    if (obj != null)
        //        ////    {
        //        ////        Result = Result + Convert.ToDouble(obj.Sum(p => p.TransactionPoints));
        //        ////    }
        //        ////}
        //        ////else
        //        ////{
        //        ////    obj = db.MLMWalletTransactions.Where(p => (p.CreateDate.Month == currentMonth - 1) && p.TransactionTypeID == 11 && p.UserLoginID == LoginUserId && p.IsAdded == false).ToList();
        //        ////    if (obj != null)
        //        ////    {
        //        ////        Result = Result + Convert.ToDouble(obj.Sum(p => p.TransactionPoints));
        //        ////    }
        //        ////}
        //        //Comented by yashaswi
        //        //By yashaswi 20-11-2018 show all inactive points
        //        //double Result = 0;
        //        //var idParam = new SqlParameter
        //        //{
        //        //    ParameterName = "UserID",
        //        //    Value = LoginUserId
        //        //};
        //        //var TotalPoints = new SqlParameter
        //        //{
        //        //    ParameterName = "TotalPoints",
        //        //    Direction = ParameterDirection.Output,
        //        //    DbType = DbType.Decimal,
        //        //    Precision = 18,
        //        //    Scale = 4
        //        //};
        //        //db.Database.ExecuteSqlCommand("Leaders_SingleUser_InactivePoints @UserID,@TotalPoints output", idParam, TotalPoints);
        //        //if (TotalPoints != null)
        //        //{
        //        //    Result = Convert.ToDouble(TotalPoints.Value);
        //        //}

        //        //return Math.Round(Result, 2);

        //        double Result = 0;
        //        try
        //        {
        //            int year = DateTime.Now.Year;
        //            int month = DateTime.Now.Month;
        //            year = (month == 1) ? (year - 1) : year;
        //            month = (month == 1) ? (12) : month;
        //            DateTime FromDate = new DateTime(year, month, 1);
        //            DateTime ToDate = FromDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
        //            var idParam = new SqlParameter
        //            {
        //                ParameterName = "LoginUserId",
        //                Value = LoginUserId
        //            };
        //            var Hour = new SqlParameter
        //            {
        //                ParameterName = "Hour",
        //                Value = WebConfigurationManager.AppSettings["Del_Hour"]
        //            };
        //            var DateFrom = new SqlParameter
        //            {
        //                ParameterName = "DateFrom",
        //                Value = ToDate
        //            };
        //            var DateTo = new SqlParameter
        //            {
        //                ParameterName = "DateTo",
        //                Value = FromDate
        //            };
        //            var TotalPoints = new SqlParameter
        //            {
        //                ParameterName = "TotalPoints",
        //                Direction = ParameterDirection.Output,
        //                DbType = DbType.Decimal,
        //                Precision = 18,
        //                Scale = 4
        //            };
        //            db.Database.ExecuteSqlCommand("Leaders_SingleUser_InactivePoints @LoginUserId ,@DateTo,@DateFrom,@Hour,@TotalPoints output", idParam, DateTo, DateFrom, Hour, TotalPoints);
        //            if (TotalPoints != null)
        //            {
        //                Result = Convert.ToDouble(TotalPoints.Value);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //        return Math.Round(Result, 2);
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}
        //public decimal GetPayout_Requested()
        //{
        //    try
        //    {
        //        long LoginUserId = 0;
        //        if (Session["ID"] != null)
        //        {
        //            LoginUserId = Convert.ToInt64(Session["ID"]);
        //        }

        //        LeadersPayoutRequest obj = db.LeadersPayoutRequests.FirstOrDefault(p => p.RequestStatus == 1 && p.UserLoginID == LoginUserId);
        //        if (obj != null)
        //        {
        //            return (decimal)obj.RequestedAmount;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}
        //public decimal GetPending_EzeeMoney()
        //{
        //    try
        //    {
        //        long LoginUserId = 0;
        //        if (Session["ID"] != null)
        //        {
        //            LoginUserId = Convert.ToInt64(Session["ID"]);
        //        }
        //        //decimal resultValue = 0;
        //        //var result = db.EzeeMoneyPayouts.Where(p => p.IsPaid == false)
        //        //    .Join(db.MLMWalletDetails.Where(m => m.UserLoginId == LoginUserId), p => p.Id, m => m.EzeeMoneyPayoutId,
        //        //    (p, m) => new
        //        //    {
        //        //        m.Amount
        //        //    }).ToList();
        //        //if (result != null && result.Count() != 0)
        //        //{
        //        //    resultValue = result.FirstOrDefault().Amount;
        //        //}
        //        //return Math.Round(resultValue, 2);
        //        decimal Result = 0;
        //        try
        //        {
        //            int year = DateTime.Now.Year;
        //            int month = DateTime.Now.Month;
        //            year = (month == 1) ? (year - 1) : year;
        //            month = (month == 1) ? (12) : month;
        //          //  DateTime FromDate = new DateTime(year, month, 1);
        //          //  DateTime ToDate = FromDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

        //            DateTime FromDate = GetStartEndDate()[0];
        //            DateTime ToDate = GetStartEndDate()[1];

        //            var idParam = new SqlParameter
        //            {
        //                ParameterName = "LoginUserId",
        //                Value = LoginUserId
        //            };
        //            var Hour = new SqlParameter
        //            {
        //                ParameterName = "Hour",
        //                Value = WebConfigurationManager.AppSettings["Del_Hour"]
        //            };
        //            var DateFrom = new SqlParameter
        //            {
        //                ParameterName = "DateFrom",
        //                Value = ToDate
        //            };
        //            var DateTo = new SqlParameter
        //            {
        //                ParameterName = "DateTo",
        //                Value = FromDate
        //            };
        //            var TotalPoints = new SqlParameter
        //            {
        //                ParameterName = "TotalPoints",
        //                Direction = ParameterDirection.Output,
        //                DbType = DbType.Decimal,
        //                Precision = 18,
        //                Scale = 4
        //            };
        //            db.Database.ExecuteSqlCommand("GetPendingEzeeMoney @LoginUserId ,@DateTo,@DateFrom,@Hour,@TotalPoints output", idParam, DateTo, DateFrom, Hour, TotalPoints);
        //            if (TotalPoints != null)
        //            {
        //                Result = Convert.ToDecimal(TotalPoints.Value);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //        return Math.Round(Result, 2);
        //    }
        //    catch { return 0; }

        //}


        //public decimal GetEzzeMoney()
        //{
        //    long LoginUserId = 0;
        //    if (Session["ID"] != null)
        //    {
        //        LoginUserId = Convert.ToInt64(Session["ID"]);
        //    }
        //    MLMWallet obj = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == LoginUserId);
        //    if (obj != null)
        //    {
        //        return obj.Amount;
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}
        //public decimal ExpectedERP()
        //{
        //    try
        //    {
        //        long LoginUserId = 0;
        //        if (Session["ID"] != null)
        //        {
        //            LoginUserId = Convert.ToInt64(Session["ID"]);
        //        }
        //        decimal result = (decimal)getERPBySP(LoginUserId, 0, 1, 0);

        //        return (result == 0) ? 0.00M : result;
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}
        //public long GetTotalMemberCount()
        //{
        //    long count = 0;
        //    try
        //    {
        //        long LoginUserId = 0;
        //        if (Session["ID"] != null)
        //        {
        //            LoginUserId = Convert.ToInt64(Session["ID"]);
        //        }

        //        var idParam = new SqlParameter
        //        {
        //            ParameterName = "UserID",
        //            Value = LoginUserId
        //        };
        //        List<NetworkUserViewModel> obj = new List<NetworkUserViewModel>();
        //        obj = GetTotalMember();
        //        if (obj != null)
        //        {
        //            count = obj.Count;
        //        }
        //        return count;
        //    }
        //    catch { }
        //    return 0;
        //}
        //public List<NetworkUserViewModel> GetTotalMember()
        //{
        //    try
        //    {
        //        long LoginUserId = 0;
        //        if (Session["ID"] != null)
        //        {
        //            LoginUserId = Convert.ToInt64(Session["ID"]);
        //        }

        //        var idParam = new SqlParameter
        //        {
        //            ParameterName = "UserID",
        //            Value = LoginUserId
        //        };
        //        db.Database.ExecuteSqlCommand("Leaders_NetworkUsers @UserID", idParam);
        //        List<NetworkUserViewModel> result = db.NetworkUsersViewModel.Where(p => p.UserId > 0).ToList();
        //        return result.Where(p => p.NetworkLevel <= 4).ToList();
        //    }
        //    catch { }
        //    return null;
        //}

        //public DateTime GetCycleStartDate()
        //{
        //    return GetStartEndDate()[0];
        //}

        //public DateTime GetCycleLastDate()
        //{
        //    return GetStartEndDate()[1];
        //}

        //private DateTime[] GetStartEndDate()
        //{
        //    DateTime StartDate = new DateTime();
        //    DateTime EndDate = new DateTime();

        //    if (DateTime.Now.Day > 25)
        //    {
        //        StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 26);
        //        EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 26);
        //    }
        //    else
        //    {
        //        if (DateTime.Now.Month == 1)
        //        {
        //            StartDate = new DateTime(DateTime.Now.Year - 1, 12, 26);
        //        }
        //        else
        //        {
        //            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 26);
        //        }
        //        EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 26);
        //    }

        //    DateTime[] dateTimes = new DateTime[2];

        //    dateTimes[0] = StartDate;
        //    dateTimes[1] = EndDate;

        //    return dateTimes;
        //}

        //public int GetDayLeft()
        //{
        //    var noOfDays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        //    var currentMonthDays = 0;
        //    if (noOfDays == 31)
        //    {
        //        currentMonthDays = noOfDays - 6; //25
        //    }
        //    else if (noOfDays == 28)
        //    {
        //        currentMonthDays = noOfDays - 3; //25
        //    }
        //    else if (noOfDays == 29)
        //    {
        //        currentMonthDays = noOfDays - 4;
        //    }
        //    else
        //        currentMonthDays = noOfDays - 5;
        //    var DifferentDays = 0;


        //    if (DateTime.Now.Day > 25)
        //    {
        //        DateTime StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 26);
        //        DateTime EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 26);
        //        DifferentDays = (int)(EndDate - StartDate).TotalDays;
        //    }
        //    else
        //    {
        //        DateTime StartDate = new DateTime();
        //        if (DateTime.Now.Month == 1)
        //        {
        //            StartDate = new DateTime(DateTime.Now.Year - 1, 12, 26);
        //        }
        //        else
        //        {
        //            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 26);
        //        }
        //        DateTime EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 26);
        //        DifferentDays = (int)(EndDate - StartDate).TotalDays;
        //    }
        //    var LeftDays = 0;
        //    if (DateTime.Now.Day > currentMonthDays)
        //    {
        //        //var Days = DateTime.Now.Day - currentMonthDays;   //30 Jan 2019
        //        //LeftDays = currentMonthDays + Days;   //30 days

        //        var RemoveDays = DateTime.Now.Day - 25;
        //        LeftDays = DifferentDays - RemoveDays;
        //    }
        //    else
        //    {
        //        LeftDays = currentMonthDays - DateTime.Now.Day;  //04-feb 2019
        //    }
        //    return LeftDays;
        //}
        //public long GetMyPurchaseCount()
        //{
        //    try
        //    {
        //        long LoginUserId = 0;
        //        if (Session["ID"] != null)
        //        {
        //            LoginUserId = Convert.ToInt64(Session["ID"]);
        //        }
        //        int currentMonth = DateTime.Now.Month;

        //        DateTime StartDate = GetStartEndDate()[0];
        //        DateTime EndDate = GetStartEndDate()[1];

        //        if (currentMonth == 8)
        //        {
        //            /*   return db.MLMWalletTransactions.Where(p => (p.CreateDate.Month == currentMonth || p.CreateDate.Month == currentMonth - 1) && p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0)
        //               .Select(p => p.CustomerOrderID).Distinct().Count();
        //               */
        //            return db.MLMWalletTransactions.Where(p => (p.CreateDate > StartDate && p.CreateDate < EndDate) && p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0)
        //            .Select(p => p.CustomerOrderID).Distinct().Count();
        //        }
        //        else
        //        {
        //            return db.MLMWalletTransactions.Where(p => (p.CreateDate > StartDate && p.CreateDate < EndDate) && p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0)
        //            .Select(p => p.CustomerOrderID).Distinct().Count();
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    return 0;
        //}
        //public long GetQRP()
        //{
        //    try
        //    {
        //        long Result = 0;
        //        decimal QRP = GetQRPMasterValue();
        //        decimal RP = GetRpOnPurchase();
        //        if (QRP > RP)
        //        {
        //            Result = Convert.ToInt64(QRP - RP);
        //        }
        //        else
        //        {
        //            Result = 0;
        //        }
        //        return Result;
        //    }
        //    catch
        //    {
        //    }
        //    return 0;
        //}
        //public decimal GetQRPMasterValue()
        //{
        //    try
        //    {
        //        QRPMaster objQRP = db.QRPMasters.FirstOrDefault(p => p.ID == 1);
        //        if (objQRP != null)
        //        {
        //            return Convert.ToDecimal(objQRP.Current_QRP);
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    return 0;
        //}


        //public decimal GetRpOnPurchase()
        //{
        //    try
        //    {
        //        long LoginUserId = 0;
        //        if (Session["ID"] != null)
        //        {
        //            LoginUserId = Convert.ToInt64(Session["ID"]);
        //        }
        //        decimal Result = 0;

        //        int currentMonth = DateTime.Now.Month;

        //        DateTime StartDate = GetStartEndDate()[0];
        //        DateTime EndDate = GetStartEndDate()[1];

        //        List<MLMWalletTransaction> obj = new List<MLMWalletTransaction>();
        //        if (currentMonth == 8)
        //        {
        //            // obj = db.MLMWalletTransactions.Where(p => (p.CreateDate.Month == currentMonth || p.CreateDate.Month == currentMonth - 1) && p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0).ToList();
        //            obj = db.MLMWalletTransactions.Where(p => p.CreateDate > StartDate && p.CreateDate < EndDate && p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0).ToList();
        //        }
        //        else
        //        {
        //            //obj = db.MLMWalletTransactions.Where(p => p.CreateDate.Month == currentMonth && p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0).ToList();
        //            obj = db.MLMWalletTransactions.Where(p => p.CreateDate > StartDate && p.CreateDate < EndDate && p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0).ToList();
        //        }
        //        if (obj != null)
        //        {
        //            Result = obj.Sum(p => p.TransactionPoints);
        //        }
        //        else
        //        {
        //            Result = 0;
        //        }
        //        Result = Math.Round(Result, 2);
        //        return (Result == 0) ? 0.00M : Result;
        //    }
        //    catch
        //    {

        //    }
        //    return 0;
        //}
        //public double getERP()
        //{
        //    try
        //    {    
        //        long LoginUserId = 0;
        //        if (Session["ID"] != null)
        //        {
        //            LoginUserId = Convert.ToInt64(Session["ID"]);
        //        }
        //        double Result = 0;
        //        Result = objLeaderDashboard.getERP_User((int)LoginUserId, 0);//Added by Sonali for call common function 04-02-2019
        //        Result = Math.Round(Result, 2);
        //        return (Result == 0) ? 0.00d : Result;
        //    }
        //    catch
        //    {

        //    }
        //    return 0;
        //}
        //public double getERPBySP(long UserId, int IsERP_, int CurrentMonth_, int AllData_)
        //{
        //    try
        //    {
        //        double Result = 0;
        //        int year = DateTime.Now.Year;
        //        int month = DateTime.Now.Month;
        //        year = (month == 1) ? (year - 1) : year;
        //        /*
        //        DateTime FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        //        DateTime ToDate = FromDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
        //        */

        //        DateTime FromDate = GetStartEndDate()[0];
        //        DateTime ToDate = GetStartEndDate()[1];

        //        var idParam = new SqlParameter
        //        {
        //            ParameterName = "UserID",
        //            Value = UserId
        //        };

        //        var IsERP = new SqlParameter
        //        {
        //            ParameterName = "IsERP",
        //            Value = IsERP_
        //        };
        //        var CurrentMonth = new SqlParameter
        //        {
        //            ParameterName = "CurrentMonth",
        //            Value = CurrentMonth_
        //        };
        //        var Hour = new SqlParameter
        //        {
        //            ParameterName = "Hour",
        //            Value = WebConfigurationManager.AppSettings["Del_Hour"]
        //        };
        //        var AllData = new SqlParameter
        //        {
        //            ParameterName = "AllData",
        //            Value = AllData_
        //        };
        //        var DateFrom = new SqlParameter
        //        {
        //            ParameterName = "DateFrom",
        //            Value = FromDate
        //        };
        //        var DateTo = new SqlParameter
        //        {
        //            ParameterName = "DateTo",
        //            Value = ToDate
        //        };
        //        var TotalPoints = new SqlParameter
        //        {
        //            ParameterName = "TotalPoints",
        //            Direction = ParameterDirection.Output,
        //            DbType = DbType.Decimal,
        //            Precision = 18,
        //            Scale = 4
        //        };
        //        db.Database.ExecuteSqlCommand("Leaders_SingleUser_ERP @UserID, @IsERP,@CurrentMonth,@Hour,@AllData,@DateFrom,@DateTo,@TotalPoints output", idParam, IsERP, CurrentMonth, Hour, AllData, DateFrom, DateTo, TotalPoints);
        //        if (TotalPoints != null)
        //        {
        //            Result = Convert.ToDouble(TotalPoints.Value);
        //        }
        //        return Math.Round(Result, 2);
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}
        //public double getERP_User(long UserId, int ForTopEarner)
        //{
        //    try
        //    {
        //        double Result = getERPBySP(UserId, 1, 1, ForTopEarner);

        //        int currentMonth = DateTime.Now.Month;

        //        DateTime FromDate = GetStartEndDate()[0];
        //        DateTime ToDate = GetStartEndDate()[1];

        //        List<MLMWalletTransaction> obj = new List<MLMWalletTransaction>();
        //        if (currentMonth <= 8)
        //        {
        //            if (ForTopEarner == 0)
        //            {
        //                //obj = db.MLMWalletTransactions.Where(p => (p.CreateDate.Month == currentMonth || p.CreateDate.Month == currentMonth - 1) && p.TransactionTypeID == 11 && p.UserLoginID == UserId && p.IsAdded == false).ToList();

        //                obj = db.MLMWalletTransactions.Where(p => ((p.CreateDate > FromDate && p.CreateDate < ToDate)) && p.TransactionTypeID == 11 && p.UserLoginID == UserId && p.IsAdded == false).ToList();
        //            }
        //            else
        //            {
        //                obj = db.MLMWalletTransactions.Where(p => p.TransactionTypeID == 11 && p.UserLoginID == UserId).ToList();
        //            }
        //            if (obj != null)
        //            {
        //                Result = Result + Convert.ToDouble(obj.Sum(p => p.TransactionPoints));
        //            }
        //        }
        //        else
        //        {
        //            if (ForTopEarner == 0)
        //            {
        //                obj = db.MLMWalletTransactions.Where(p => (p.CreateDate > FromDate && p.CreateDate < ToDate) && p.TransactionTypeID == 11 && p.UserLoginID == UserId && p.IsAdded == false).ToList();
        //            }
        //            else
        //            {
        //                obj = db.MLMWalletTransactions.Where(p => p.TransactionTypeID == 11 && p.UserLoginID == UserId).ToList();
        //            }
        //            if (obj != null)
        //            {
        //                Result = Result + Convert.ToDouble(obj.Sum(p => p.TransactionPoints));
        //            }
        //        }
        //        //double level0 = 0, level1 = 0, level2 = 0, level3 = 0, level4 = 0;


        //        //var Current_level = db.MLMWallet_DirectIncomes
        //        //    .Where(p => p.CurrentLevel_UserLoginId == UserId)
        //        //    .Join(db.MLMWalletTransactions.Where(q => q.CreateDate.Month == currentMonth)
        //        //    , p => p.MLMWalletTransactionId, q => q.ID,
        //        //    (p, q) => new
        //        //    {
        //        //        p.CurrentLevel
        //        //    }).ToList();
        //        //if (Current_level != null && Current_level.Count != 0)
        //        //{
        //        //    level0 = Current_level.Sum(p => p.CurrentLevel);
        //        //}

        //        //var level_One = db.MLMWallet_DirectIncomes
        //        //    .Where(p => p.UpLine1_UserLoginId == UserId)
        //        //    .Join(db.MLMWalletTransactions.Where(q => q.CreateDate.Month == currentMonth)
        //        //    , p => p.MLMWalletTransactionId, q => q.ID,
        //        //    (p, q) => new
        //        //    {
        //        //        p.UpLine1
        //        //    }).ToList();
        //        //if (level_One != null && level_One.Count != 0)
        //        //{
        //        //    level1 = level_One.Sum(p => p.UpLine1);
        //        //}

        //        //var level_Two = db.MLMWallet_DirectIncomes
        //        //    .Where(p => p.UpLine2_UserLoginId == UserId)
        //        //    .Join(db.MLMWalletTransactions.Where(q => q.CreateDate.Month == currentMonth)
        //        //    , p => p.MLMWalletTransactionId, q => q.ID,
        //        //    (p, q) => new
        //        //    {
        //        //        p.UpLine2
        //        //    }).ToList();
        //        //if (level_Two != null && level_Two.Count != 0)
        //        //{
        //        //    level2 = level_Two.Sum(p => p.UpLine2);
        //        //}

        //        //var level_Three = db.MLMWallet_DirectIncomes
        //        //    .Where(p => p.UpLine3_UserLoginId == UserId)
        //        //    .Join(db.MLMWalletTransactions.Where(q => q.CreateDate.Month == currentMonth)
        //        //    , p => p.MLMWalletTransactionId, q => q.ID,
        //        //    (p, q) => new
        //        //    {
        //        //        p.UpLine3
        //        //    }).ToList();
        //        //if (level_Three != null && level_Three.Count != 0)
        //        //{
        //        //    level3 = level_Three.Sum(p => p.UpLine3);
        //        //}

        //        //var level_four = db.MLMWallet_DirectIncomes
        //        //    .Where(p => p.UpLine3_UserLoginId == UserId)
        //        //    .Join(db.MLMWalletTransactions.Where(q => q.CreateDate.Month == currentMonth)
        //        //    , p => p.MLMWalletTransactionId, q => q.ID,
        //        //    (p, q) => new
        //        //    {
        //        //        p.UpLine4
        //        //    }).ToList();
        //        //if (level_four != null && level_four.Count != 0)
        //        //{
        //        //    level4 = level_four.Sum(p => p.UpLine4);
        //        //}
        //        //Result = Result + level0 + level1 + level2 + level3 + level4;

        //        return Math.Round(Result, 2);
        //    }
        //    catch
        //    {

        //    }
        //    return 0;
        //}
        //public long GetInactiveMemberCount()
        //{
        //    try
        //    {
        //        int Hour = Convert.ToInt16(WebConfigurationManager.AppSettings["Del_Hour"]);
        //        DateTime CurrentDate = DateTime.Now;
        //        int currentMonth = DateTime.Now.Month;
        //        decimal CurrentQRP = GetQRPMasterValue();

        //        DateTime FromDate = GetStartEndDate()[0];
        //        DateTime ToDate = GetStartEndDate()[1];

        //        List<NetworkUserViewModel> memberList = new List<NetworkUserViewModel>();
        //        memberList = GetTotalMember();
        //        List<NetworkUserStatusViewModel> memberInActiveList = new List<NetworkUserStatusViewModel>();
        //        foreach (var item in memberList)
        //        {
        //            NetworkUserStatusViewModel obj_ = new NetworkUserStatusViewModel();
        //            List<MLMWalletTransaction> obj = new List<MLMWalletTransaction>();
        //            if (currentMonth == 8)
        //            {
        //                // obj = db.MLMWalletTransactions.Where(p => (p.CreateDate.Month == currentMonth || p.CreateDate.Month == currentMonth - 1) && p.TransactionTypeID == 7 && p.UserLoginID == item.UserId && p.OrderAmount > 0 && System.Data.Entity.DbFunctions.AddHours(p.CreateDate, Hour) <= CurrentDate).ToList();

        //                obj = db.MLMWalletTransactions.Where(p => p.CreateDate > FromDate && p.CreateDate < ToDate && p.TransactionTypeID == 7 && p.UserLoginID == item.UserId && p.OrderAmount > 0 && System.Data.Entity.DbFunctions.AddHours(p.CreateDate, Hour) <= CurrentDate).ToList();
        //            }
        //            else
        //            {
        //                obj = db.MLMWalletTransactions.Where(p => p.CreateDate > FromDate && p.CreateDate < ToDate && p.TransactionTypeID == 7 && p.UserLoginID == item.UserId && p.OrderAmount > 0 && System.Data.Entity.DbFunctions.AddHours(p.CreateDate, Hour) <= CurrentDate).ToList();
        //            }
        //            if (obj != null)
        //            {
        //                if (CurrentQRP > obj.Sum(p => p.TransactionPoints))
        //                {
        //                    obj_.IsActive = false;
        //                    obj_.UserId = (int)item.UserId;
        //                    memberInActiveList.Add(obj_);
        //                }
        //            }
        //        }
        //        return memberInActiveList.Count;
        //    }
        //    catch { }
        //    return 0;
        //}
        #endregion

        #region Invite User
        [HttpPost]
        public ActionResult InviteUser(InviteUser user)
        {
            SendEmail_InviteUser(user.Message, user.MobileNo, user.Name, user.Email);
            SaveMLMUserInvites(user.Message, user.MobileNo, user.Name, user.Email);
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        public void SaveMLMUserInvites(string msg, string Mobile, string uName, string email)
        {
            long LoginUserId = 0;
            if (Session["ID"] != null)
            {
                LoginUserId = Convert.ToInt64(Session["ID"]);
            }
            bool isInviteIDUsed = false;
            string InviteID = "";
            int counter = 5;
            do
            {
                InviteID = GenerateInviteID("123456789", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                isInviteIDUsed = db.MLMUserInvite.Any(p => p.InviteID == InviteID);
                InviteID = (isInviteIDUsed == true) ? "" : InviteID;
                counter = counter - 1;
            }
            while (isInviteIDUsed == true || counter == 0);

            if (InviteID != "")
            {
                MLMUserInvites obj = new MLMUserInvites();
                obj.CreateBy = LoginUserId;
                obj.CreateDate = DateTime.Now;
                obj.Email = email;
                obj.IsAccepted = false;
                obj.Message = msg;
                obj.Mobile = Mobile;
                obj.Name = uName;
                obj.UserLoginID = LoginUserId;
                obj.InviteID = InviteID;
                db.MLMUserInvite.Add(obj);
                db.SaveChanges();
            }
        }
        string GenerateInviteID(string No, string Alpha)
        {
            RefferalCodeGenerator objRefferalCodeGenerator = new RefferalCodeGenerator();
            string Characters = "";
            string Numbers = "";

            Numbers = No;
            Characters = Alpha;
            string CodeString = objRefferalCodeGenerator.CreateCode(5, Characters);
            string CodeNumeric = objRefferalCodeGenerator.CreateCode(4, Numbers);
            string RefferalCode = CodeString + CodeNumeric;
            return RefferalCode;
        }
        public void SendEmail_InviteUser(string msg, string Mobile, string uName, string Email)
        {
            try
            {
                long LoginUserId = 0;
                if (Session["ID"] != null)
                {
                    LoginUserId = Convert.ToInt64(Session["ID"]);
                }
                var UserLogin = db.PersonalDetails.Where(u => u.UserLoginID == LoginUserId)
                    .Join(db.MLMUsers, u => u.UserLoginID, m => m.UserID, (u, m) => new
                    {
                        RefferalCode = m.Ref_Id,
                        Name = u.FirstName
                    }).ToList();
                if (UserLogin != null && UserLogin.Count() > 0)
                {
                    string Name = UserLogin.First().Name;
                    string RefferalCode = UserLogin.First().RefferalCode;
                    string URL = "http://www.ezeelo.com/nagpur/2/login?Phone=" + Mobile + "&ReferalCode=" + RefferalCode + "&Name=" + uName + "&Email=" + Email;
                    Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                    dictEmailValues.Add("<!--RefferalCode-->", RefferalCode);
                    dictEmailValues.Add("<!--UserName-->", Name);
                    dictEmailValues.Add("<!--Message-->", msg);
                    dictEmailValues.Add("<!--URL-->", URL);
                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                    string EmailID = Email;
                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.Leaders_InviteUser, new string[] { EmailID, "sales@ezeelo.com" }, dictEmailValues, true);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DashboardController][M:SendEmail_InviteUser]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DashboardController][M:SendEmail_InviteUser]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
        }
        #endregion

        #region Report
        //public List<LeadersRecentJoineesViewModel> GetRecentJoinees()
        //{
        //    List<LeadersRecentJoineesViewModel> list = new List<LeadersRecentJoineesViewModel>();
        //    try
        //    {
        //        long LoginUserId = 0;
        //        if (Session["ID"] != null)
        //        {
        //            LoginUserId = Convert.ToInt64(Session["ID"]);
        //        }

        //        var idParam = new SqlParameter
        //        {
        //            ParameterName = "UserID",
        //            Value = LoginUserId
        //        };

        //        list = db.Database.SqlQuery<LeadersRecentJoineesViewModel>("EXEC Leaders_RecentJoinees @UserID", idParam).ToList<LeadersRecentJoineesViewModel>();
        //    }
        //    catch { }
        //    return list;
        //}

        [HttpPost]
        public ActionResult GetHighPerformer(HighPerformer obj)
        {
            List<HighPerformerViewModel> list = new List<HighPerformerViewModel>();
            long LoginUserId = 0;
            if (Session["ID"] != null)
            {
                LoginUserId = Convert.ToInt64(Session["ID"]);
            }
            var idParam = new SqlParameter
            {
                ParameterName = "UserID",
                Value = LoginUserId
            };
            if (obj.searchId == 1)
            {
                list = db.Database.SqlQuery<HighPerformerViewModel>("EXEC Leaders_TopRecruiters @UserID", idParam).ToList<HighPerformerViewModel>();
            }
            else if (obj.searchId == 2)
            {
                list = db.Database.SqlQuery<HighPerformerViewModel>("EXEC Leaders_TopBuyers @UserID", idParam).ToList<HighPerformerViewModel>();
            }
            else if (obj.searchId == 3)
            {
                List<NetworkUserViewModel> memberList = new List<NetworkUserViewModel>();
                memberList = objLeaderDashboard.GetTotalMember(LoginUserId);//Added by Sonali for call common function 04-02-2019
                if (memberList.Count != 0)
                {
                    foreach (var item in memberList)
                    {
                        HighPerformerViewModel objHighPerformerViewModel = new HighPerformerViewModel();
                        objHighPerformerViewModel.money = (decimal)objLeaderDashboard.getERP_User(item.UserId, 1);//Added by Sonali for call common function 04-02-2019
                        objHighPerformerViewModel.Level = (int)item.NetworkLevel;
                        PersonalDetail pD = db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == item.UserId);
                        if (pD != null)
                        {
                            objHighPerformerViewModel.Name = pD.FirstName + ' ' + pD.LastName;
                        }
                        objHighPerformerViewModel.searchBy = 3;
                        objHighPerformerViewModel.UsersCont = 0;
                        list.Add(objHighPerformerViewModel);
                    }
                    list = list.Where(p => p.money > 0).OrderByDescending(p => p.money).Take(10).ToList();
                }
            }
            obj.listHighPerformerViewModel = list;
            return PartialView("_GetHighPerformer", list);
        }

        [HttpPost]
        public ActionResult GetRefferalCountByFilter(RefferalByFilter obj)
        {
            long LoginUserId = 0;
            if (Session["ID"] != null)
            {
                LoginUserId = Convert.ToInt64(Session["ID"]);
            }

            DateTime FromDate = DateTime.Now.Date;
            DateTime ToDate;
            if (obj.searchId == 1)
            {
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                year = (month == 1) ? (year - 1) : year;
                FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                ToDate = FromDate.AddMonths(1).AddDays(-1);
            }
            else
            {
                int month = DateTime.Now.Month;
                int Last5th;
                switch (month)
                {
                    case 1:
                        Last5th = 8;
                        break;
                    case 2:
                        Last5th = 9;
                        break;
                    case 3:
                        Last5th = 10;
                        break;
                    case 4:
                        Last5th = 11;
                        break;
                    case 5:
                        Last5th = 12;
                        break;
                    default:
                        Last5th = month - 5;
                        break;
                }
                int year = DateTime.Now.Year;
                year = (month == 1) ? (year - 1) : year;

                FromDate = new DateTime(year, Last5th, 1);
                ToDate = (new DateTime(year, DateTime.Now.Month, 1)).AddMonths(1).AddDays(-1);
            }

            var idParam = new SqlParameter
            {
                ParameterName = "UserID",
                Value = LoginUserId
            };
            var fDt = new SqlParameter
            {
                ParameterName = "FromeDate",
                Value = FromDate
            };
            var tDt = new SqlParameter
            {
                ParameterName = "ToDate",
                Value = ToDate
            };
            var NetworkUserViewModelct = db.Database.SqlQuery<NetworkUserViewModel>("Leaders_Refferal @UserID, @FromeDate, @ToDate", idParam, fDt, tDt).ToList<NetworkUserViewModel>();
            if (NetworkUserViewModelct != null && NetworkUserViewModelct.Count != 0)
            {
                obj.Count = NetworkUserViewModelct.Count;
            }
            return PartialView("_GetRefCount", obj);
        }

        #endregion

        [SessionExpire]
        public ActionResult Notification()
        {
            long LoginUserId = 0;
            if (Session["ID"] != null)
            {
                LoginUserId = Convert.ToInt64(Session["ID"]);
            }
            List<LeadersNotification> listLeadersNotification = db.LeadersNotifications.Where(p => p.UserLoginId == LoginUserId).OrderByDescending(p => p.Id).ToList();
            return View(listLeadersNotification);
        }

        [HttpGet]
        public JsonResult GetWithDrawnByFilter(int filter)
        {
            return Json("GetWithDrawnByFilter Response from Get", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetWithDrawnByERP(int filter)
        {
            return Json("GetWithDrawnByERP Response from Get", JsonRequestBehavior.AllowGet);
        }

        class UserStatus
        {
            public decimal TotalRetailPoints { get; set; }
        }
        public bool CheckUserStatus()
        {
            //Start 31/10/2018  For Graphics display only in Oct month Payout
            try
            {
                Decimal? currentQRP = db.QRPMasters.Select(y => y.Current_QRP).FirstOrDefault();

                bool status = false;
                long LoginUserId = 0;
                if (Session["ID"] != null)
                {
                    LoginUserId = Convert.ToInt64(Session["ID"]);
                }
                List<UserStatus> result = db.Database.SqlQuery<UserStatus>(@"
                select  SUM(mwt.TransactionPoints) as TotalRetailPoints
                from Mlm_User m
                join  MLMWalletTransaction mwt on m.UserID = mwt.UserLoginID
                left join MLMWalletTransaction m1 on mwt.CustomerOrderID = m1.CustomerOrderID
                where (mwt.OrderAmount > 0 and  (
                m1.CreateDate between '2018/10/01 00:00:00' and '2018/10/31 23:59:59')
                and m1.TransactionTypeID = 1 and mwt.TransactionTypeID = 7 and m.UserID = " + LoginUserId + " )").ToList<UserStatus>();
                if (result != null)
                {
                    if (result.Count() != 0)
                    {
                        if (result.Sum(p => p.TotalRetailPoints) >= currentQRP)
                        {
                            status = true;
                        }
                        else
                        {
                            status = false;
                        }
                    }
                }
                return status;
            }
            catch
            {
                return false;
            }

        }
    }
}