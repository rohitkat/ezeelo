using System;
using System.Collections.Generic;
using System.Linq;
using ModelLayer.Models;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;
using ModelLayer.Models.ViewModel;

namespace BusinessLogicLayer
{
    /// -----<summary>
    /// Created By Yashaswi
    /// Date : 01-02-2019
    /// Use : Load Leaders Dashboard
    /// Called From : Leader Module, API module
    /// </summary>
    public class LeadersDashboard
    {
        EzeeloDBContext db = new EzeeloDBContext();
        #region Dashboard Count

        /// <summary>
        /// Return Withdrawn Amount from wallet
        /// </summary>
        /// <param name="LoginUserId"></param>
        /// <returns> Withdrawn Amount</returns>
        public decimal GetWithdrawnAmount(long LoginUserId)
        {
            try
            {
                List<LeadersPayoutRequest> obj = db.LeadersPayoutRequests.Where(p => p.RequestStatus == 2 && p.UserLoginID == LoginUserId).ToList();
                if (obj != null && obj.Count != 0)
                {
                    return (decimal)obj.Sum(p => p.RequestedAmount);
                }
                else
                {
                    return 0;
                }
            }
            catch { return 0; }
        }

        /// <summary>
        /// Check is user fullfill its active criteria
        /// </summary>
        /// <param name="LoginUserId"></param>
        /// <returns>True/False</returns>
        public bool IsUserActive(long LoginUserId)
        {
            try
            {
                decimal Result = GetRpOnPurchase(LoginUserId);
                decimal CurrentQRP = GetQRPMasterValue();
                DateTime CurrentDate = DateTime.Now;
                
                if (CurrentQRP > Result)
                {
                    ////Added By Yashaswi
                    ////if business booster order placed by customer and delivered then he is active for next 30 days
                    //var result = (from placed in db.planSubscriberTranscations
                    //              join delivered in db.planSubscriberTranscations
                    //              on placed.CustomerOrderID equals delivered.CustomerOrderID
                    //              where delivered.IsActive == true && delivered.Status == 7 && placed.Status == 1 && placed.UserLoginID == LoginUserId
                    //              && CurrentDate >= placed.CreateDate && CurrentDate <= placed.CreateDate.AddDays(30)
                    //              select new
                    //              {
                    //                  placed.CustomerOrderID,
                    //                  placed.UserLoginID
                    //              });
                    //if(result == null || result.Count() == 0)
                    //{
                    //    return false;
                    //}
                    //else
                    //{
                    //    return true;
                    //}
                    ////end
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch { }
            return false;
        }


        public void Distribute_Designation(long Userloginid)
        {
            List<TabularView> list = new List<TabularView>();

            var idParam = new SqlParameter
            {
                ParameterName = "UserID",
                Value = Userloginid
            };

            db.Database.SqlQuery<TabularView>("EXEC Sp_DistributeDesignationForAllLevel @UserID", idParam).ToList<TabularView>();


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
                    Designation = "Prospect";
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


        /// <summary>
        /// Return total Inactive Points 
        /// </summary>
        /// <param name="LoginUserId"></param>
        /// <returns></returns>
        public double GetInactivePoints(long LoginUserId)
        {
            try
            {
                double Result = 0;
                try
                {
                    DateTime FromDate = GetStartEndDate()[0];
                    DateTime ToDate = GetStartEndDate()[1];
                    var idParam = new SqlParameter
                    {
                        ParameterName = "LoginUserId",
                        Value = LoginUserId
                    };
                    var CurrentPayoutStartDate = new SqlParameter
                    {
                        ParameterName = "CurrentPayoutStartDate",
                        Value = FromDate
                    };
                    var CurrentPayoutEndDate = new SqlParameter
                    {
                        ParameterName = "CurrentPayoutEndDate",
                        Value = ToDate
                    };
                    var TotalPoints = new SqlParameter
                    {
                        ParameterName = "TotalPoints",
                        Direction = ParameterDirection.Output,
                        DbType = DbType.Decimal,
                        Precision = 18,
                        Scale = 4
                    };
                    db.Database.ExecuteSqlCommand("Leaders_SingleUser_InactivePoints @LoginUserId ,@CurrentPayoutStartDate,@CurrentPayoutEndDate,@TotalPoints output", idParam, CurrentPayoutStartDate, CurrentPayoutEndDate, TotalPoints);
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



        public DashboardViewModel GetallDashboarddatabysp(long UserLoginId)
        {
            DashboardViewModel obj = new DashboardViewModel();

            DateTime FromDate = DateTime.Now.Date;
            DateTime ToDate;
            DateTime FromDate1 = DateTime.Now.Date;
            DateTime ToDate1;
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            year = (month == 1) ? (year - 1) : year;
            FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ToDate = FromDate.AddMonths(1).AddDays(-1);


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
            year = DateTime.Now.Year;
            year = (month == 1) ? (year - 1) : year;

            FromDate1 = new DateTime(year, Last5th, 1);
            ToDate1 = (new DateTime(year, DateTime.Now.Month, 1)).AddMonths(1).AddDays(-1);

            var hour = getHour(); // WebConfigurationManager.AppSettings["Del_Hour"];


            //var idParam = new SqlParameter
            //{
            //    ParameterName = "UserId",
            //    Value = UserLoginId
            //};
            //var Hour = new SqlParameter
            //{
            //    ParameterName = "hour",
            //    Value = WebConfigurationManager.AppSettings["Del_Hour"]
            //};
            //var DateFrom = new SqlParameter
            //{
            //    ParameterName = "FromeDate",
            //    Value = ToDate
            //};
            //var DateTo = new SqlParameter
            //{
            //    ParameterName = "ToDate",
            //    Value = FromDate
            //};

            //var DateFrom1 = new SqlParameter
            //{
            //    ParameterName = "FromeDate1",
            //    Value = ToDate1
            //};
            //var DateTo1 = new SqlParameter
            //{
            //    ParameterName = "ToDate1",
            //    Value = FromDate1
            //};



            // List<DashboardViewModel> obj1 = new List<DashboardViewModel>();

            ReadConfig lReadCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            string query = "[GetdashboardLeaderDataApp]";

            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", UserLoginId);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);////added franchiseId 
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            cmd.Parameters.AddWithValue("@FromDate1", FromDate1);
            cmd.Parameters.AddWithValue("@ToDate1", ToDate1);
            cmd.Parameters.AddWithValue("@hour", hour);

            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(lReadCon.DB_CONNECTION))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    sda.Fill(ds);
                }
            }

            obj.Referrals = Convert.ToInt64(ds.Tables[0].Rows[0]["Referrals"]);
            obj.Referrals_5Month = Convert.ToInt64(ds.Tables[0].Rows[0]["RefferalByFilter_5Month"]);
            obj.TOTAL_MEMBERS = Convert.ToInt64(ds.Tables[0].Rows[0]["TotalMember"]);
            obj.QUALIFYING_RETAIL_POINTS = Convert.ToInt64(ds.Tables[0].Rows[0]["QRP"]);
            obj.Withdrawn = Convert.ToDecimal(ds.Tables[0].Rows[0]["Withdrawn"]);
            obj.ERP = Convert.ToDouble(ds.Tables[0].Rows[0]["ERP"]);
            obj.RP_ON_MY_PURCHASE = Convert.ToDecimal(ds.Tables[0].Rows[0]["RpOnPurchase"]);
            obj.INACTIVE_MEMBERS = Convert.ToInt32(ds.Tables[0].Rows[0]["Inactivemember"]);
            obj.Payout_Requested = Convert.ToDecimal(ds.Tables[0].Rows[0]["RequestedAmount"]);
            obj.EZEE_MONEY = Convert.ToDecimal(ds.Tables[0].Rows[0]["EzeeMoney"]);
            obj.UserDesignation = ds.Tables[0].Rows[0]["UserDesignation"].ToString();
            obj.CasbackPoints = Convert.ToDecimal(ds.Tables[0].Rows[0]["CasbackPoints"]);
            obj.CasbackEzeeMoney = Convert.ToDecimal(ds.Tables[0].Rows[0]["CasbackEzeeMoney"]);

            //public List<NetworkUserViewModel> userlist { get; set; }


            obj.userlist = (from DataRow dr in ds.Tables[1].Rows
                            select new NetworkUserViewModel()
                            {
                                UserId = Convert.ToInt64(dr["USERID"]),
                                NetworkLevel = Convert.ToInt32(dr["LEVELNO"]),
                            }).ToList();








            //  obj1 = db.Database.SqlQuery<DashboardViewModel>("GetdashboardLeaderDataApp @UserId,@FromeDate,@ToDate,@FromeDate1,@ToDate1,@hour", idParam, DateFrom, DateTo, DateFrom1, DateTo1, Hour).ToList<DashboardViewModel>();



            return obj;
        }


        /// <summary>
        ///  Return Requested Withdrawn Amount from wallet
        /// </summary>
        /// <param name="LoginUserId"></param>
        /// <returns></returns>
        public decimal GetPayout_Requested(long LoginUserId)
        {
            try
            {
                LeadersPayoutRequest obj = db.LeadersPayoutRequests.FirstOrDefault(p => (p.RequestStatus == 0 || p.RequestStatus == 1) && p.UserLoginID == LoginUserId && p.IsActive == true);  // added by amit
                if (obj != null)
                {
                    return (decimal)obj.RequestedAmount;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Return Pending EzeeMoney according to payout cycle
        /// </summary>
        /// <param name="LoginUserId"></param>
        /// <returns></returns>
        public decimal GetPending_EzeeMoney(long LoginUserId)
        {
            try
            {
                decimal Result = 0;
                try
                {
                    DateTime FromDate = GetStartEndDate()[0];
                    DateTime ToDate = GetStartEndDate()[1];

                    var idParam = new SqlParameter
                    {
                        ParameterName = "LoginUserId",
                        Value = LoginUserId
                    };
                    var Hour = new SqlParameter
                    {
                        ParameterName = "Hour",
                        Value = getHour() //WebConfigurationManager.AppSettings["Del_Hour"]
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
                    db.Database.ExecuteSqlCommand("GetPendingEzeeMoney @LoginUserId ,@DateTo,@DateFrom,@Hour,@TotalPoints output", idParam, DateTo, DateFrom, Hour, TotalPoints);
                    if (TotalPoints != null)
                    {
                        Result = Convert.ToDecimal(TotalPoints.Value);
                    }
                    // Result = 0;//Added by sonali for Pending EzeeMoney calculation on 10-04-2019
                }
                catch (Exception ex)
                {
                }
                return Math.Round(Result, 2);
            }
            catch { return 0; }

        }

        /// <summary>
        /// Return Ezee Money in wallet
        /// </summary>
        /// <param name="LoginUserId"></param>
        /// <returns></returns>
        public decimal GetEzzeMoney(long LoginUserId)
        {
            MLMWallet obj = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == LoginUserId);
            if (obj != null)
            {
                return obj.Amount;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Return Expected ERP:
        /// Calculated ERP from order delivered datetime to next 48hour
        /// </summary>
        /// <returns></returns>
        public decimal ExpectedERP(long LoginUserId)
        {
            try
            {
                decimal result = (decimal)getERPBySP(LoginUserId, 0, 1, 0);

                return (result == 0) ? 0.00M : result;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Return Total Member count
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Return Total Member count by calling stored procedure
        /// </summary>
        /// <param name="LoginUserId"></param>
        /// <returns></returns>
        public List<NetworkUserViewModel> GetTotalMember(long LoginUserId)
        {
            try
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "UserID",
                    Value = LoginUserId
                };
                db.Database.ExecuteSqlCommand("Leaders_NetworkUsers @UserID", idParam);
                List<NetworkUserViewModel> result = db.NetworkUsersViewModel.Where(p => p.UserId > 0).ToList();
                return result.Where(p => p.NetworkLevel <= 6).ToList();
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Return start date of leaders payout cycle 
        /// </summary>
        /// <returns></returns>
        public DateTime GetCycleStartDate()
        {
            return GetStartEndDate()[0];
        }

        /// <summary>
        /// Return end date of leaders payout cycle
        /// </summary>
        /// <returns></returns>
        public DateTime GetCycleLastDate()
        {
            return GetStartEndDate()[1];
        }

        /// <summary>
        /// Return start and End date of leaders payout cycle. At 0 index return start date and at 1 index return end date
        /// </summary>
        /// <returns></returns>
        /// 
       
        private DateTime[] GetStartEndDate()
        {
            ReadConfig lReadCon = new ReadConfig(System.Web.HttpContext.Current.Server);
          
            string query = " select * from GetPayoutCycleDate(GETDATE())";

            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = CommandType.Text;          

            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(lReadCon.DB_CONNECTION))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    sda.Fill(ds);
                }
            }
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();
            StartDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["StartDate"]);
            EndDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["EndDate"]);

            //StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //EndDate = StartDate.AddMonths(1).AddDays(-1);

            // undo changes by lokesh for 1st date to last date
            //commented by yashaswi
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

        /// <summary>
        /// Return days left to complete leaders payout cycle
        /// </summary>
        /// <returns></returns>
        public int GetDayLeft()
        {
            //Cmmented By Yashaswi
            //var noOfDays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            //var currentMonthDays = 0;
            //if (noOfDays == 31)
            //{
            //    currentMonthDays = noOfDays - 6; //25
            //}
            //else if (noOfDays == 28)
            //{
            //    currentMonthDays = noOfDays - 3; //25
            //}
            //else if (noOfDays == 29)
            //{
            //    currentMonthDays = noOfDays - 4;
            //}
            //else
            //    currentMonthDays = noOfDays - 5;
            //var DifferentDays = 0;


            //if (DateTime.Now.Day > 25)
            //{
            //    DateTime StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 26);
            //    DateTime EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 26);
            //    DifferentDays = (int)(EndDate - StartDate).TotalDays;
            //}
            //else
            //{
            //    DateTime StartDate = new DateTime();
            //    if (DateTime.Now.Month == 1)
            //    {
            //        StartDate = new DateTime(DateTime.Now.Year - 1, 12, 26);
            //    }
            //    else
            //    {
            //        StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 26);
            //    }
            //    DateTime EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 26);
            //    DifferentDays = (int)(EndDate - StartDate).TotalDays;
            //}
            //var LeftDays = 0;
            //if (DateTime.Now.Day > currentMonthDays)
            //{
            //    var RemoveDays = DateTime.Now.Day - 25;
            //    LeftDays = DifferentDays - RemoveDays;
            //}
            //else
            //{
            //    LeftDays = currentMonthDays - DateTime.Now.Day;  //04-feb 2019
            //}


            //DateTime StartDate1 = new  DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //DateTime firtstdayofmonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //DateTime EndDate1 = firtstdayofmonth.AddMonths(1).AddDays(-1);
            int LeftDays = 0;


            DateTime Todate = GetCycleLastDate();
            LeftDays = Convert.ToInt16((Todate - DateTime.Now).TotalDays);
            return LeftDays;
        }

        /// <summary>
        /// Return count of total number of delivered order within payout cycle
        /// </summary>
        /// <param name="LoginUserId"></param>
        /// <returns></returns>
        public long GetMyPurchaseCount(long LoginUserId)
        {
            try
            {
                int currentMonth = DateTime.Now.Month;

                DateTime StartDate = GetStartEndDate()[0];
                DateTime EndDate = GetStartEndDate()[1];

                //return db.MLMWalletTransactions.Where(p => p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0

                //)
                //.Select(p => p.CustomerOrderID).Distinct().Count();

                return db.MLMWalletTransactions.Where(p => p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && (p.OrderAmount > 0 || (p.OrderAmount == 0 && p.WalletAmountUsed != 0 && p.TransactionTypeID == 7))
               && (db.MLMWalletTransactions.Where(q => q.UserLoginID == LoginUserId && q.TransactionTypeID == 1 && q.CreateDate > StartDate && q.CreateDate < EndDate).Select(q => q.CustomerOrderID)).Contains(p.CustomerOrderID)).ToList().Count();

            }
            catch
            {
            }
            return 0;
        }

        /// <summary>
        /// Return Qualifying RP within payout cycle
        /// </summary>
        /// <returns></returns>
        public long GetQRP(long LoginUserId)
        {
            try
            {
                long Result = 0;
                decimal QRP = GetQRPMasterValue();
                decimal RP = GetRpOnPurchase(LoginUserId);
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

        /// <summary>
        /// Return Max QRP from Master
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Return Retail points within payout cycle
        /// </summary>
        /// <returns></returns>
        public decimal GetRpOnPurchase(long LoginUserId)
        {
            try
            {
                decimal Result = 0;

                int currentMonth = DateTime.Now.Month;

                DateTime StartDate = GetStartEndDate()[0];
                DateTime EndDate = GetStartEndDate()[1];

                List<MLMWalletTransaction> obj = new List<MLMWalletTransaction>();

                //obj = db.MLMWalletTransactions.Where(p => p.CreateDate > StartDate && p.CreateDate < EndDate && p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && p.OrderAmount > 0).ToList();
                obj = db.MLMWalletTransactions.Where(p => p.TransactionTypeID == 7 && p.UserLoginID == LoginUserId && (p.OrderAmount > 0 || (p.OrderAmount == 0 && p.WalletAmountUsed != 0 && p.TransactionTypeID == 7))
                && (db.MLMWalletTransactions.Where(q => q.UserLoginID == LoginUserId && q.TransactionTypeID == 1 && q.CreateDate > StartDate && q.CreateDate < EndDate).Select(q => q.CustomerOrderID)).Contains(p.CustomerOrderID)).ToList();


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

        /// <summary>
        /// Return ERP within payout cycle
        /// </summary>
        /// <param name="LoginUserId"></param>
        /// <returns></returns>
        public double getERP(long LoginUserId)
        {
            try
            {
                double Result = 0;
                Result = getERP_User((int)LoginUserId, 0);
                Result = Math.Round(Result, 2);
                return (Result == 0) ? 0.00d : Result;
            }
            catch
            {

            }
            return 0;
        }

        /// <summary>
        /// Return ERP within payout cycle by calling stored procedure
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="IsERP_"></param>
        /// <param name="CurrentMonth_"></param>
        /// <param name="AllData_"></param>
        /// <returns></returns>
        public double getERPBySP(long UserId, int IsERP_, int CurrentMonth_, int AllData_)
        {
            try
            {
                double Result = 0;
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                year = (month == 1) ? (year - 1) : year;
                DateTime FromDate = GetStartEndDate()[0];
                DateTime ToDate = GetStartEndDate()[1];



                var idParam = new SqlParameter
                {
                    ParameterName = "UserID",
                    Value = UserId
                };

                var IsERP = new SqlParameter
                {
                    ParameterName = "IsERP",
                    Value = IsERP_
                };
                var CurrentMonth = new SqlParameter
                {
                    ParameterName = "CurrentMonth",
                    Value = CurrentMonth_
                };
                var Hour = new SqlParameter
                {
                    ParameterName = "Hour",
                    Value = getHour() // WebConfigurationManager.AppSettings["Del_Hour"]
                };
                var AllData = new SqlParameter
                {
                    ParameterName = "AllData",
                    Value = AllData_
                };
                var DateFrom = new SqlParameter
                {
                    ParameterName = "DateFrom",
                    Value = FromDate
                };
                var DateTo = new SqlParameter
                {
                    ParameterName = "DateTo",
                    Value = ToDate
                };
                var TotalPoints = new SqlParameter
                {
                    ParameterName = "TotalPoints",
                    Direction = ParameterDirection.Output,
                    DbType = DbType.Decimal,
                    Precision = 18,
                    Scale = 4
                };
                db.Database.ExecuteSqlCommand("Leaders_SingleUser_ERP @UserID, @IsERP,@CurrentMonth,@Hour,@AllData,@DateFrom,@DateTo,@TotalPoints output", idParam, IsERP, CurrentMonth, Hour, AllData, DateFrom, DateTo, TotalPoints);
                if (TotalPoints != null)
                {
                    Result = Convert.ToDouble(TotalPoints.Value);
                }
                return Math.Round(Result, 2);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Return ERP
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ForTopEarner">0 : Return ERP within payout cycle, 1 : Total ERP from start of leaders module</param>
        /// <returns></returns>
        public double getERP_User(long UserId, int ForTopEarner)
        {
            try
            {
                double Result = getERPBySP(UserId, 1, 1, ForTopEarner);

                int currentMonth = DateTime.Now.Month;

                DateTime FromDate = GetStartEndDate()[0];
                DateTime ToDate = GetStartEndDate()[1];

                List<MLMWalletTransaction> obj = new List<MLMWalletTransaction>();
                if (currentMonth <= 8)
                {
                    if (ForTopEarner == 0)
                    {
                        obj = db.MLMWalletTransactions.Where(p => ((p.CreateDate > FromDate && p.CreateDate < ToDate)) && p.TransactionTypeID == 11 && p.UserLoginID == UserId && p.IsAdded == false).ToList();
                    }
                    else
                    {
                        obj = db.MLMWalletTransactions.Where(p => p.TransactionTypeID == 11 && p.UserLoginID == UserId).ToList();
                    }
                    if (obj != null)
                    {
                        Result = Result + Convert.ToDouble(obj.Sum(p => p.TransactionPoints));
                    }
                }
                else
                {
                    if (ForTopEarner == 0)
                    {
                        obj = db.MLMWalletTransactions.Where(p => (p.CreateDate > FromDate && p.CreateDate < ToDate) && p.TransactionTypeID == 11 && p.UserLoginID == UserId && p.IsAdded == false).ToList();
                    }
                    else
                    {
                        obj = db.MLMWalletTransactions.Where(p => p.TransactionTypeID == 11 && p.UserLoginID == UserId).ToList();
                    }
                    if (obj != null)
                    {
                        Result = Result + Convert.ToDouble(obj.Sum(p => p.TransactionPoints));
                    }
                }
                return Math.Round(Result, 2);
            }
            catch
            {

            }
            return 0;
        }

        /// <summary>
        /// Return Inactive Member count within payout cycle
        /// </summary>
        /// <param name="LoginUserId"></param>
        /// <returns></returns>
        public long GetInactiveMemberCount(long LoginUserId)
        {
            try
            {
                int Hour = getHour();// Convert.ToInt16(WebConfigurationManager.AppSettings["Del_Hour"]);
                DateTime CurrentDate = DateTime.Now;
                int currentMonth = DateTime.Now.Month;
                decimal CurrentQRP = GetQRPMasterValue();

                //DateTime FromDate = GetStartEndDate()[0]; comment by lokesh
                //DateTime ToDate = GetStartEndDate()[1]; comment by lokesh

                DateTime FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 26);
                DateTime ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 26);


                List<NetworkUserViewModel> memberList = new List<NetworkUserViewModel>();
                memberList = GetTotalMember(LoginUserId);
                List<long> UserLoginIDs = memberList.Select(x => x.UserId).ToList();
                var MLMWalletTransactions = db.MLMWalletTransactions.Where(p => p.CreateDate > FromDate && p.CreateDate < ToDate && p.TransactionTypeID == 7 && UserLoginIDs.Contains(p.UserLoginID) && p.OrderAmount > 0 && System.Data.Entity.DbFunctions.AddHours(p.CreateDate, Hour) <= CurrentDate).ToList();
                List<NetworkUserStatusViewModel> memberInActiveList = new List<NetworkUserStatusViewModel>();
                foreach (var item in memberList)
                {
                    NetworkUserStatusViewModel obj_ = new NetworkUserStatusViewModel();
                    List<MLMWalletTransaction> obj = new List<MLMWalletTransaction>();
                    if (currentMonth == 8)
                    {
                        obj = MLMWalletTransactions.Where(p => p.CreateDate > FromDate && p.CreateDate < ToDate && p.TransactionTypeID == 7 && p.UserLoginID == item.UserId && p.OrderAmount > 0 && System.Data.Entity.DbFunctions.AddHours(p.CreateDate, Hour) <= CurrentDate).ToList();
                    }
                    else
                    {
                        obj = MLMWalletTransactions.Where(p => p.CreateDate > FromDate && p.CreateDate < ToDate && p.TransactionTypeID == 7 && p.UserLoginID == item.UserId && p.OrderAmount > 0 && System.Data.Entity.DbFunctions.AddHours(p.CreateDate, Hour) <= CurrentDate).ToList();
                    }
                    if (obj != null)
                    {
                        if (CurrentQRP > obj.Sum(p => p.TransactionPoints))
                        {
                            obj_.IsActive = false;
                            obj_.UserId = (int)item.UserId;
                            memberInActiveList.Add(obj_);
                        }
                    }
                }
                return memberInActiveList.Count;
            }
            catch (Exception ex) { }
            return 0;
        }
        #endregion

        #region Dashboard Report
        /// <summary>
        /// Return Recentaly join user
        /// </summary>
        /// <param name="LoginUserId"></param>
        /// <returns></returns>
        public List<LeadersRecentJoineesViewModel> GetRecentJoinees(long LoginUserId)
        {
            List<LeadersRecentJoineesViewModel> list = new List<LeadersRecentJoineesViewModel>();
            try
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "UserID",
                    Value = LoginUserId
                };

                list = db.Database.SqlQuery<LeadersRecentJoineesViewModel>("EXEC Leaders_RecentJoinees @UserID", idParam).ToList<LeadersRecentJoineesViewModel>();
            }
            catch { }
            return list;
        }

        /// <summary>
        /// Return high performer user list
        /// </summary>
        /// <param name="obj"> 
        /// obj.searchId = 1 : Return Top recruiters
        /// obj.searchId = 2 : Return Top Buyer
        /// obj.searchId = 3 : Return Top Earner
        /// </param>
        /// <param name="LoginUserId"></param>
        /// <returns></returns>
        public List<HighPerformerViewModel> GetHighPerformer(HighPerformer obj, long LoginUserId)
        {
            List<HighPerformerViewModel> list = new List<HighPerformerViewModel>();

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
                memberList = GetTotalMember(LoginUserId);
                if (memberList.Count != 0)
                {
                    List<long> UserLoginIds = memberList.Select(x => x.UserId).ToList();
                    var PersonalDetails = db.PersonalDetails.Where(x => UserLoginIds.Contains(x.UserLoginID)).ToList();
                    foreach (var item in memberList)
                    {
                        HighPerformerViewModel objHighPerformerViewModel = new HighPerformerViewModel();
                        objHighPerformerViewModel.money = (decimal)getERP_User(item.UserId, 1);
                        objHighPerformerViewModel.Level = (int)item.NetworkLevel;
                        PersonalDetail pD = PersonalDetails.FirstOrDefault(p => p.UserLoginID == item.UserId);
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
            return list;
        }

        /// <summary>
        /// Return Refferal count 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="LoginUserId"></param>
        /// <returns></returns>
        public long GetRefferalCountByFilter(RefferalByFilter obj, long LoginUserId)
        {
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
            return obj.Count;
        }

        #endregion

        public int getHour()
        {
            return db.QRPMasters.Select(p => p.Hour).FirstOrDefault();
        }
    }
}
