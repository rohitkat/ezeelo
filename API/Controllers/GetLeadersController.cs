using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace API.Controllers
{
    public class GetLeadersController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        [HttpGet]
        [Route("api/GetLeaders/GetDashboardData")]
        public object GetDashboardData(int UserLoginId)
        {
            object obj = new object();
            try
            {
                if (UserLoginId == null || UserLoginId <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid parameter.", data = string.Empty };
                }
                MLMUser objMLMUser = db.MLMUsers.Where(x => x.UserID == UserLoginId).FirstOrDefault();
                if (objMLMUser != null)
                {
                    MLMDashboard dashboard = new MLMDashboard();
                    dashboard = this.GetDashboardDataBySp(UserLoginId);
                    dashboard.Id = objMLMUser.Id_Ref;
                    // comment by lokesh
                    //dashboard.TotalMember = this.GetTotalMemberCount(UserLoginId);
                    //dashboard.EzeeMoney = this.GetEzzeMoney(UserLoginId);
                    //dashboard.QRP = this.GetQRP(UserLoginId);
                    //dashboard.RpOnPurchase = this.GetRpOnPurchase(UserLoginId);
                    //dashboard.UserDesignation = UserDesignation(Convert.ToInt32(UserLoginId));
                    
                    dashboard.TotalMember = dashboard.TotalMember;
                    dashboard.EzeeMoney = dashboard.EzeeMoney;
                    dashboard.QRP = dashboard.QRP;
                    dashboard.RpOnPurchase = dashboard.RpOnPurchase;
                    dashboard.UserDesignation = dashboard.UserDesignation;

                    obj = new { Success = 1, Message = "Successfull", data = dashboard };
                }
                else
                {
                    BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                    string ImageName = rcKey.LOCALIMG_PATH + "Content/img/" + "Leader.png";
                    obj = new { Success = 1, Message = "This is not MLMUser.", data = ImageName };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }


        public DateTime GetCycleStartDate()
        {
            return GetStartEndDate()[0];
        }

        public DateTime GetCycleLastDate()
        {
            return GetStartEndDate()[1];
        }

        private DateTime[] GetStartEndDate()
        {
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();
            //changes by roshan
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
           
            //if (DateTime.Now.Day > 25)
            //{
            //    StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 26);
            //    EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 26);
            //}
            //else
            //{
            //    if (DateTime.Now.Month == 1)
            //    {
            //        StartDate = new DateTime(DateTime.Now.Year - 1, 12, 26);
            //    }
            //    else
            //    {
            //        StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 26);
            //    }
            //    EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 26);
            //}

            DateTime[] dateTimes = new DateTime[2];

            dateTimes[0] = StartDate;
            dateTimes[1] = EndDate;

            return dateTimes;
        }

        public long GetTotalMemberCount(long LoginUserId)
        {
            long count = 0;
            try
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "UserID",
                    Value = LoginUserId
                };
                List<NetworkUserViewModel> obj = new List<NetworkUserViewModel>();
                obj = GetTotalMember(LoginUserId);
                if (obj != null)
                {
                    count = obj.Count;
                }
                return count;
            }
            catch { }
            return 0;
        }

        public MLMDashboard GetDashboardDataBySp(long LoginUserId)
        {
            List<MLMDashboard> obj = new List<MLMDashboard>();

            var idParam = new SqlParameter
            {
                ParameterName = "UserID",
                Value = LoginUserId
            };

            obj = db.Database.SqlQuery<MLMDashboard>("EXEC GetdashboardApp " + LoginUserId + "").ToList<MLMDashboard>();

            return obj[0];

        }

        public List<NetworkUserViewModel> GetTotalMember(long LoginUserId)
        {
            try
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "UserID",
                    Value = LoginUserId
                };
                db.Database.ExecuteSqlCommand("Leaders_NetworkUsers_New @UserID", idParam);
                List<NetworkUserViewModel> result = db.NetworkUsersViewModel.Where(p => p.UserId > 0).ToList();
                return result.Where(p => p.NetworkLevel <= 6).ToList();
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public decimal GetEzzeMoney(long LoginUserId)
        {
            MLMWallet obj = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == LoginUserId);
            if (obj != null)
            {
                decimal WalAmt = obj.Amount;
                //Get CashbackWallet Amount 03-10-2019
                CashbackWallet wallet = db.cashbackWallets.FirstOrDefault(p => p.UserLoginID == LoginUserId);
                if (wallet != null)
                {
                    WalAmt = WalAmt + wallet.Amount;
                }

                return WalAmt;
            }
            else
            {
                return 0;
            }
        }

        public long GetQRP(long UserLoginId)
        {
            try
            {
                long Result = 0;
                decimal QRP = GetQRPMasterValue();
                decimal RP = GetRpOnPurchase(UserLoginId);
                if (QRP > RP)
                {
                    Result = Convert.ToInt64(QRP - RP);
                }
                else
                {
                    Result = 0;
                }
                return Result;
            }
            catch
            {
            }
            return 0;
        }
        public decimal GetQRPMasterValue()
        {
            try
            {
                QRPMaster objQRP = db.QRPMasters.FirstOrDefault(p => p.ID == 1);
                if (objQRP != null)
                {
                    return Convert.ToDecimal(objQRP.Current_QRP);
                }
            }
            catch
            {
            }
            return 0;
        }
        public decimal GetRpOnPurchase(long LoginUserId)
        {
            try
            {
                decimal Result = 0;

                int currentMonth = DateTime.Now.Month;

                DateTime StartDate = GetStartEndDate()[0];
                DateTime EndDate = GetStartEndDate()[1];


                List<MLMWalletTransaction> obj = new List<MLMWalletTransaction>();
                if (currentMonth == 8)
                {
                    // obj = db.MLMWalletTransactions.Where(p => (p.CreateDate.Month == currentMonth || p.CreateDate.Month == currentMonth - 1) && p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0).ToList();
                    obj = db.MLMWalletTransactions.Where(p => p.CreateDate > StartDate && p.CreateDate < EndDate && p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0).ToList();
                }
                else
                {
                    // obj = db.MLMWalletTransactions.Where(p => p.CreateDate.Month == currentMonth && p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0).ToList();
                    obj = db.MLMWalletTransactions.Where(p => p.CreateDate >= StartDate && p.CreateDate < EndDate && p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0).ToList();
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

        public string UserDesignation(int LoginUserId)
        {

            string Designation = string.Empty;
            long value = 0;
            try
            {
                //value = db.MLMUsers.FirstOrDefault(q => q.UserID == UserLoginId).islifestyleachiever;
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

       




    }
}
public class MLMDashboard
{
    public long Id { get; set; }
    public long TotalMember { get; set; }
    public decimal EzeeMoney { get; set; }
    public decimal QRP { get; set; }
    public decimal RpOnPurchase { get; set; }
    public string UserDesignation { get; set; }

    public List<MLMDashboard> Dashboarddata { get; set; }
}

