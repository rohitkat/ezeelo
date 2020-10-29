using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Leaders.Controllers;
using System.Data.SqlClient;
using BusinessLogicLayer;
using System.Web.Configuration;
using System.Transactions;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using Leaders.Filter;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class EzeeMoneyPayoutController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // EzeeloDBContext db1 = new EzeeloDBContext();

        public ActionResult Index()
        {
            EzeeMoneyPayoutViewModel obj = new EzeeMoneyPayoutViewModel();

            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            year = (month == 1) ? (year - 1) : year;

            obj.FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);   //amit
            obj.ToDate = obj.FromDate.AddMonths(1).AddDays(-1).AddHours(11).AddMinutes(30); //amit
            obj.isFrzDtDisable = true;
            obj.isGoDisable = false;
            obj.isPayEzMnyDisable = true;

            EzeeMoneyPayout objEzeeMoneyPayoutDate = db.EzeeMoneyPayouts.Where(p => p.IsPaid == true).OrderByDescending(p => p.PaidDate).Take(1).FirstOrDefault();
            if (objEzeeMoneyPayoutDate != null)
            {
                obj.LastFromPayoutDate = objEzeeMoneyPayoutDate.FromDate;
                obj.LastToPayoutDate = objEzeeMoneyPayoutDate.ToDate;
            }
            else
            {

            }

            //Coin Rate
            MLMCoinRate objCoin = db.MLMCoinRates.FirstOrDefault(p => p.IsActive == true);
            if (objCoin != null)
            {
                obj.CoinRate = objCoin.Rate;
            }

            //Check For Freeze Data
            EzeeMoneyPayout objEzeeMoneyPayout = db.EzeeMoneyPayouts.FirstOrDefault(p => p.IsPaid == false);
            if (objEzeeMoneyPayout != null)
            {
                obj.EzeeMoneyPayoutId = objEzeeMoneyPayout.Id;
                obj.FromDate = objEzeeMoneyPayout.FromDate;
                obj.ToDate = objEzeeMoneyPayout.ToDate;
                obj.CoinRate = objEzeeMoneyPayout.CoinRate;
                obj.TotalAmount = objEzeeMoneyPayout.TotalAmount;
                obj.TotalERP = objEzeeMoneyPayout.TotalERP;
                obj.PayableAmount = objEzeeMoneyPayout.PayableAmount;
                obj.PayableERP = objEzeeMoneyPayout.PayableERP;
                obj.DiffAmount = obj.TotalAmount - obj.PayableAmount;
                obj.DiffERP = obj.TotalERP - obj.PayableERP;
                obj.isFrzDtDisable = true;
                obj.isGoDisable = true;
                obj.isPayEzMnyDisable = false;
                obj.isGoClick = true;
                obj.UserCount = objEzeeMoneyPayout.TotalUserCount;
                obj.ActiveUserCount = objEzeeMoneyPayout.ActiveUserCount;
                obj.DelOrdCount = objEzeeMoneyPayout.DeliveredOrdCount;
            }

            return View(obj);
        }

        public ActionResult Go(EzeeMoneyPayoutViewModel model)
        {
            EzeeMoneyPayout objEzeeMoneyPayoutDate = db.EzeeMoneyPayouts.Where(p => p.IsPaid == true).OrderByDescending(p => p.PaidDate).Take(1).FirstOrDefault();
            if (objEzeeMoneyPayoutDate != null)
            {
                if ((model.FromDate <= objEzeeMoneyPayoutDate.FromDate && model.FromDate <= objEzeeMoneyPayoutDate.ToDate) || (model.ToDate <= objEzeeMoneyPayoutDate.FromDate && model.ToDate <= objEzeeMoneyPayoutDate.ToDate))
                {
                    TempData["ErrorMsg"] = "You Cant select this Date, Please Select date greater than Last Payout To date";
                    return RedirectToAction("Index");
                }
            }
            List<PayoutFreezeDataReportViewModel> list_rep = new List<PayoutFreezeDataReportViewModel>();
            model = GetRecord(model.FromDate, model.ToDate, 0, out list_rep);
            model.isFrzDtDisable = false;
            model.isGoDisable = false;
            model.isPayEzMnyDisable = true;
            return View("Index", model);
        }

        public EzeeMoneyPayoutViewModel GetRecord(DateTime FromDate, DateTime ToDate, int ForPayout_, out List<PayoutFreezeDataReportViewModel> list_rep)
        {
            list_rep = PayoutFreezeData(FromDate, ToDate);
            EzeeMoneyPayoutViewModel model = new EzeeMoneyPayoutViewModel();
            model.FromDate = FromDate;
            //model.ToDate = ToDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            //Count of Order Delivered

            model.ToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59); // added by amit

            model.DelOrdCount = list_rep.Sum(p => p.DelOrdCount);
            //Coin Rate
            MLMCoinRate objCoin = db.MLMCoinRates.FirstOrDefault(p => p.IsActive == true);
            if (objCoin != null)
            {
                model.CoinRate = objCoin.Rate;
            }
            model.UserCount = list_rep.Where(p => p.DelOrdCount != 0).Count();
            model.ActiveUserCount = list_rep.Where(p => p.Status == true).Count();
            model.TotalERP = Math.Round(list_rep.Sum(p => p.ERP), 2);
            model.TotalAmount = Math.Round(list_rep.Sum(p => p.EzeeMoney), 2);
            model.PayableERP = Math.Round(list_rep.Where(p => p.Status == true).Sum(p => p.ERP), 2);
            model.PayableAmount = Math.Round(list_rep.Where(p => p.Status == true).Sum(p => p.EzeeMoney), 2);
            model.DiffAmount = Math.Round(model.TotalAmount - model.PayableAmount, 2);
            model.DiffERP = Math.Round(model.TotalERP - model.PayableERP, 2);
            model.isGoClick = true;
            model.listEzeeMoney = list_rep.Where(p => p.Status == true).ToList();

            if (ForPayout_ == 1)
            {
                foreach (var item in model.listEzeeMoney)
                {
                    var fromDate = new SqlParameter
                    {
                        ParameterName = "DateFrom",
                        Value = model.ToDate
                    };
                    var toDate = new SqlParameter
                    {
                        ParameterName = "DateTo",
                        Value = model.FromDate
                    };
                    var Hour = new SqlParameter
                    {
                        ParameterName = "Hour",
                        Value = (new LeadersDashboard()).getHour()// WebConfigurationManager.AppSettings["Del_Hour"]
                    };
                    var UserId = new SqlParameter
                    {
                        ParameterName = "UserID",
                        Value = item.UserLoginId
                    };
                    db.Database.ExecuteSqlCommand("EXEC EzeeMoneyPayoutFreezeData @UserID,@Hour,@DateTo,@DateFrom", UserId, Hour, toDate, fromDate);
                }

            }

            //EzeeMoneyPayoutViewModel model = new EzeeMoneyPayoutViewModel();
            //model.FromDate = FromDate;
            //model.ToDate = ToDate;
            //double QRP = (double)(new DashboardController()).GetQRPMasterValue();

            ////Count of Order Delivered
            //List<MLMWalletTransaction> listDeleOrd = db.MLMWalletTransactions.Where(p => p.CreateDate > model.FromDate && p.CreateDate <= model.ToDate && p.TransactionTypeID == 7 && p.OrderAmount > 0).ToList();
            //model.DelOrdCount = listDeleOrd.Count();



            ////ActiveUser Count
            //var listUser = db.MLMWallet_DirectIncomes.Where(p => p.CreateDate > model.FromDate && p.CreateDate <= model.ToDate)
            //    .GroupBy(p => p.CurrentLevel_UserLoginId)
            //    .Select(o => new
            //    {
            //        CurrentLevel_UserLoginId = o.Key,
            //        TransactionPoints = o.Sum(q => q.TransactionPoints)
            //    })
            //    .ToList();
            //if (listUser != null && listUser.Count() != 0)
            //{
            //    model.UserCount = listUser.Count();
            //    int Count = 0;
            //    foreach (var item in listUser)
            //    {
            //        if (item.TransactionPoints >= QRP)
            //        {
            //            Count = Count + 1;
            //        }
            //    }
            //    model.ActiveUserCount = Count;
            //}

            //var fromDate = new SqlParameter
            //{
            //    ParameterName = "DateFrom",
            //    Value = model.ToDate
            //};
            //var toDate = new SqlParameter
            //{
            //    ParameterName = "DateTo",
            //    Value = model.FromDate
            //};
            //var MW = new SqlParameter
            //{
            //    ParameterName = "MW",
            //    Value = 1
            //};
            //var DI = new SqlParameter
            //{
            //    ParameterName = "DI",
            //    Value = 1
            //};
            //var Hour = new SqlParameter
            //{
            //    ParameterName = "Hour",
            //    Value = WebConfigurationManager.AppSettings["Del_Hour"]
            //};


            //db.Database.ExecuteSqlCommand("EXEC Leaders_Calculate_TotalERP @DateTo,@DateFrom,@MW,@DI,@Hour", toDate, fromDate, MW, DI, Hour);

            //List<Network_User_Extend> listEzeeMoney = db.Network_User_Extends.Distinct().ToList();

            //double T_MLMWallet = listEzeeMoney.Sum(p => p.ERPPoints_MLMWallet);
            //double T_DirectIncome = listEzeeMoney.Sum(p => p.ERPPoints_DirectIncome);
            //double P_MLMWallet = listEzeeMoney.Where(p => p.UserStatus == 1).Sum(p => p.ERPPoints_MLMWallet);
            //double P_DirectIncome = listEzeeMoney.Where(p => p.UserStatus == 1).Sum(p => p.ERPPoints_DirectIncome);

            //model.listEzeeMoney = listEzeeMoney.Where(p => p.UserStatus == 1).ToList();

            //if (ForPayout_ == 1)
            //{
            //    foreach (var item in model.listEzeeMoney)
            //    {
            //        var UserId = new SqlParameter
            //        {
            //            ParameterName = "UserID",
            //            Value = item.UserId
            //        };
            //        db.Database.ExecuteSqlCommand("EXEC EzeeMoneyPayoutFreezeData @UserID,@Hour,@DateTo,@DateFrom", UserId, Hour, toDate, fromDate);
            //    }
            //}

            //model.TotalERP = Math.Round((decimal)(T_MLMWallet + T_DirectIncome), 2);
            //model.TotalAmount = Math.Round((decimal)(model.TotalERP * model.CoinRate), 2);
            //model.PayableERP = Math.Round((decimal)(P_MLMWallet + P_DirectIncome), 2);
            //model.PayableAmount = Math.Round((decimal)(model.PayableERP * model.CoinRate), 2);
            //model.DiffAmount = Math.Round(model.TotalAmount - model.PayableAmount, 2);
            //model.DiffERP = Math.Round(model.TotalERP - model.PayableERP, 2);
            //model.isGoClick = true;
            return model;
        }

        public ActionResult FreezeData(DateTime FromDate, DateTime ToDate)
        {
            // using (TransactionScope tscope = new TransactionScope())
            using (TransactionScope scope =
              new TransactionScope(TransactionScopeOption.Required,
                                    new System.TimeSpan(0, 15, 0)))

            {
                int i = 0;
                try
                {
                    List<PayoutFreezeDataReportViewModel> list_rep = new List<PayoutFreezeDataReportViewModel>();
                    EzeeMoneyPayoutViewModel model = GetRecord(FromDate, ToDate, 1, out list_rep);
                    if (model.listEzeeMoney.Count() != 0)
                    {
                        EzeeMoneyPayout obj = new EzeeMoneyPayout();
                        obj.FromDate = FromDate;
                        //obj.ToDate = ToDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
                        obj.ToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59); ;
                        obj.CoinRate = model.CoinRate;
                        obj.TotalAmount = model.TotalAmount;
                        obj.TotalERP = model.TotalERP;
                        obj.PayableAmount = model.PayableAmount;
                        obj.PayableERP = model.PayableERP;
                        obj.FreezeBy = Convert.ToInt64(Session["ID"]);
                        obj.FreezeDate = DateTime.Now;
                        obj.IsPaid = false;
                        obj.DeliveredOrdCount = model.DelOrdCount;
                        obj.ActiveUserCount = model.ActiveUserCount;
                        obj.TotalUserCount = model.UserCount;
                        obj.NetworkIP = CommonFunctions.GetClientIP();
                        db.EzeeMoneyPayouts.Add(obj);
                        db.SaveChanges();

                        foreach (var item in model.listEzeeMoney)
                        {
                            MLMWalletDetails objMLMWalletDetails = new MLMWalletDetails();
                            objMLMWalletDetails.EzeeMoneyPayoutId = obj.Id;
                            objMLMWalletDetails.UserLoginId = item.UserLoginId;
                            objMLMWalletDetails.ERP = item.ERP;
                            objMLMWalletDetails.Amount = item.EzeeMoney;
                            db.MLMWalletDetails.Add(objMLMWalletDetails);
                            db.SaveChanges();
                        }

                        foreach (var item in list_rep.Where(p => p.Status == true || p.ERP != 0 || p.TotalRetailPoints != 0).ToList())
                        {
                            i = i + 1;
                            EzeeMoneyPayoutDetails objEMPD = new EzeeMoneyPayoutDetails();
                            if (!db.EzeeMoneyPayoutDetail.Any(p => p.UserLoginId == item.UserLoginId && p.EzeeMoneyPayoutID == obj.Id))
                            {
                                objEMPD.UserLoginId = item.UserLoginId;
                                objEMPD.Name = item.Name;
                                objEMPD.EmailId = item.EmailId;
                                objEMPD.PhoneNo = item.PhoneNo;
                                objEMPD.DelOrdCount = item.DelOrdCount;
                                objEMPD.TotalOrdAmt = item.TotalOrdAmt;
                                objEMPD.TotalRetailPoints = item.TotalRetailPoints;
                                objEMPD.ERP = item.ERP;
                                objEMPD.Status = item.Status;
                                objEMPD.EzeeMoney = item.EzeeMoney;
                                objEMPD.QRP = item.QRP;
                                objEMPD.EzeeMoneyPayoutID = obj.Id;
                                objEMPD.OrdCode = item.OrdCode; //yashaswi 22-1-19
                                objEMPD.TransID = item.TransID; //yashaswi 22-1-19
                                db.EzeeMoneyPayoutDetail.Add(objEMPD);
                                db.SaveChanges();
                                //db1.Dispose();
                            }
                        }
                        scope.Complete();
                        //tscope.Complete();
                        return Json(obj.Id, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("No Record For Payout", JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    int abc = i;
                    //scope.Complete();
                    scope.Dispose();
                    return Json(ex.Message, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult PayOut_EzeeMoney(long EzeeMoneyPayoutId)
        {
            EzeeMoneyPayout objEzeeMoneyPayout = db.EzeeMoneyPayouts.FirstOrDefault(p => p.IsPaid == false);
            if (objEzeeMoneyPayout != null)
            {
                if (objEzeeMoneyPayout.Id != EzeeMoneyPayoutId)
                {
                    return Json("Ezee Money Payout Freeze Id Does not match with Requested Id", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    using (TransactionScope tscope = new TransactionScope())
                    {
                        try
                        {
                            EzeeMoneyPayout objEzeeMoneyPayout_ = db.EzeeMoneyPayouts.FirstOrDefault(p => p.IsPaid == false && p.Id == EzeeMoneyPayoutId);
                            objEzeeMoneyPayout_.IsPaid = true;
                            objEzeeMoneyPayout_.PaidDate = DateTime.Now;
                            db.SaveChanges();

                            List<MLMWalletDetails> listMLMWalletDetails = db.MLMWalletDetails.Where(p => p.EzeeMoneyPayoutId == EzeeMoneyPayoutId).ToList();
                            foreach (var item in listMLMWalletDetails)
                            {
                                MLMWallet objMLMWallet = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == item.UserLoginId);
                                objMLMWallet.Amount = objMLMWallet.Amount + item.Amount;
                                objMLMWallet.Points = objMLMWallet.Points + item.ERP;
                                objMLMWallet.IsActive = true;
                                objMLMWallet.LastModifyBy = Convert.ToInt64(Session["ID"]);
                                objMLMWallet.LastModifyDate = DateTime.Now;
                                db.SaveChanges();

                                if (item.Amount > 0)
                                {
                                    (new SendFCMNotification()).SendNotification("payout", item.UserLoginId);
                                }
                            }

                            //Current Level
                            List<MLMWallet_DirectIncome> listMLMWallet_DirectIncomeCurrent = db.MLMWallet_DirectIncomes.Where(p => p.CurrentLevel_IsPaid == false).ToList();
                            foreach (var item in listMLMWallet_DirectIncomeCurrent)
                            {
                                MLMWallet_DirectIncome obj_ = db.MLMWallet_DirectIncomes.FirstOrDefault(p => p.ID == item.ID);
                                obj_.CurrentLevel_IsPaid = true;
                                obj_.CurrentLevel_PayoutDate = DateTime.Now;
                                db.SaveChanges();
                            }

                            //UpLine 1
                            List<MLMWallet_DirectIncome> listMLMWallet_DirectIncomeUpLine1 = db.MLMWallet_DirectIncomes.Where(p => p.UpLine1_IsPaid == false).ToList();
                            foreach (var item in listMLMWallet_DirectIncomeUpLine1)
                            {
                                MLMWallet_DirectIncome obj_ = db.MLMWallet_DirectIncomes.FirstOrDefault(p => p.ID == item.ID);
                                obj_.UpLine1_IsPaid = true;
                                obj_.UpLine1_PayoutDate = DateTime.Now;
                                db.SaveChanges();
                            }

                            //UpLine 2
                            List<MLMWallet_DirectIncome> listMLMWallet_DirectIncomeUpLine2 = db.MLMWallet_DirectIncomes.Where(p => p.UpLine2_IsPaid == false).ToList();
                            foreach (var item in listMLMWallet_DirectIncomeUpLine2)
                            {
                                MLMWallet_DirectIncome obj_ = db.MLMWallet_DirectIncomes.FirstOrDefault(p => p.ID == item.ID);
                                obj_.UpLine2_IsPaid = true;
                                obj_.UpLine2_PayoutDate = DateTime.Now;
                                db.SaveChanges();
                            }

                            //UpLine 3
                            List<MLMWallet_DirectIncome> listMLMWallet_DirectIncomeUpLine3 = db.MLMWallet_DirectIncomes.Where(p => p.UpLine3_IsPaid == false).ToList();
                            foreach (var item in listMLMWallet_DirectIncomeUpLine3)
                            {
                                MLMWallet_DirectIncome obj_ = db.MLMWallet_DirectIncomes.FirstOrDefault(p => p.ID == item.ID);
                                obj_.UpLine3_IsPaid = true;
                                obj_.UpLine3_PayoutDate = DateTime.Now;
                                db.SaveChanges();
                            }

                            //UpLine 4
                            List<MLMWallet_DirectIncome> listMLMWallet_DirectIncomeUpLine4 = db.MLMWallet_DirectIncomes.Where(p => p.UpLine4_IsPaid == false).ToList();
                            foreach (var item in listMLMWallet_DirectIncomeUpLine4)
                            {
                                MLMWallet_DirectIncome obj_ = db.MLMWallet_DirectIncomes.FirstOrDefault(p => p.ID == item.ID);
                                obj_.UpLine4_IsPaid = true;
                                obj_.UpLine4_PayoutDate = DateTime.Now;
                                db.SaveChanges();
                            }

                            //UpLine 5
                            List<MLMWallet_DirectIncome> listMLMWallet_DirectIncomeUpLine5 = db.MLMWallet_DirectIncomes.Where(p => p.UpLine5_IsPaid == false).ToList();
                            foreach (var item in listMLMWallet_DirectIncomeUpLine5)
                            {
                                MLMWallet_DirectIncome obj_ = db.MLMWallet_DirectIncomes.FirstOrDefault(p => p.ID == item.ID);
                                obj_.UpLine5_IsPaid = true;
                                obj_.UpLine5_PayoutDate = DateTime.Now;
                                db.SaveChanges();
                            }

                            //UpLine 6
                            List<MLMWallet_DirectIncome> listMLMWallet_DirectIncomeUpLine6 = db.MLMWallet_DirectIncomes.Where(p => p.UpLine6_IsPaid == false).ToList();
                            foreach (var item in listMLMWallet_DirectIncomeUpLine6)
                            {
                                MLMWallet_DirectIncome obj_ = db.MLMWallet_DirectIncomes.FirstOrDefault(p => p.ID == item.ID);
                                obj_.UpLine6_IsPaid = true;
                                obj_.UpLine6_PayoutDate = DateTime.Now;
                                db.SaveChanges();
                            }

                            // change by lokesh


                            tscope.Complete();
                            return Json(1, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            tscope.Dispose();
                            return Json(ex.Message, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            return View();
        }

        public List<PayoutFreezeDataReportViewModel> PayoutFreezeData(DateTime FromDate, DateTime ToDate)
        {
            List<PayoutFreezeDataReportViewModel> list = new List<PayoutFreezeDataReportViewModel>();
            ToDate = ToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            var fromDate = new SqlParameter
            {
                ParameterName = "DateFrom",
                Value = ToDate
            };
            var toDate = new SqlParameter
            {
                ParameterName = "DateTo",
                Value = FromDate
            };
            var Hour = new SqlParameter
            {
                ParameterName = "Hour",
                Value = (new LeadersDashboard()).getHour()//WebConfigurationManager.AppSettings["Del_Hour"]
            };
            list = db.Database.SqlQuery<PayoutFreezeDataReportViewModel>("exec PayoutFreezeDataReport @DateTo,@DateFrom,@Hour", toDate, fromDate, Hour).ToList<PayoutFreezeDataReportViewModel>();
            return list;
        }

        public ActionResult PayoutFreezeData_Report()
        {
            EzeeMoneyPayout objEzeeMoneyPayout = db.EzeeMoneyPayouts.FirstOrDefault(p => p.IsPaid == false);

            List<EzeeMoneyPayoutDetails> list_rep = new List<EzeeMoneyPayoutDetails>();
            if (objEzeeMoneyPayout != null)
            {
                list_rep = db.EzeeMoneyPayoutDetail.Where(p => p.EzeeMoneyPayoutID == objEzeeMoneyPayout.Id).ToList();
            }


            foreach (var item in list_rep)
            {
                if (item.UserLoginId != 0)
                {

                    item.LastFirstMonthRP = UserLastfirstmonthRP(Convert.ToInt32(item.UserLoginId));
                    item.LastSecoundMonthRP = UserSecoundfirstmonthRP(Convert.ToInt32(item.UserLoginId));
                }


            }



            return View(list_rep);
        }

        public decimal UserLastfirstmonthRP(int LoginUserId)
        {

            decimal RP = 0;
            long value = 0;
            try
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "UserID",
                    Value = LoginUserId

                };

                var TotalPoints = new SqlParameter
                {
                    ParameterName = "TotalPoints",
                    Direction = System.Data.ParameterDirection.Output,
                    DbType = System.Data.DbType.Decimal,
                    Precision = 18,
                    Scale = 4
                };
                db.Database.ExecuteSqlCommand("sp_getlastfirstmonthrp @UserID,@TotalPoints output", idParam, TotalPoints);
                if (TotalPoints != null)
                {
                    RP = Convert.ToDecimal(TotalPoints.Value);
                }

            }
            catch (Exception ex)
            {

            }


            return RP;
        }


        public decimal UserSecoundfirstmonthRP(int LoginUserId)
        {

            decimal RP = 0;
            long value = 0;
            try
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "UserID",
                    Value = LoginUserId

                };

                var TotalPoints = new SqlParameter
                {
                    ParameterName = "TotalPoints",
                    Direction = System.Data.ParameterDirection.Output,
                    DbType = System.Data.DbType.Decimal,
                    Precision = 18,
                    Scale = 4
                };
                db.Database.ExecuteSqlCommand("sp_getlastSecoundMonthrp @UserID,@TotalPoints output", idParam, TotalPoints);
                if (TotalPoints != null)
                {
                    RP = Convert.ToDecimal(TotalPoints.Value);
                }

            }
            catch (Exception ex)
            {

            }


            return RP;
        }



        public ActionResult ExportToExcel()
        {
            var gv = new GridView();
            string FileName = "";
            EzeeMoneyPayout objEzeeMoneyPayout = db.EzeeMoneyPayouts.FirstOrDefault(p => p.IsPaid == false);

            List<EzeeMoneyPayoutDetails> list_rep = new List<EzeeMoneyPayoutDetails>();
            if (objEzeeMoneyPayout != null)
            {
                list_rep = db.EzeeMoneyPayoutDetail.Where(p => p.EzeeMoneyPayoutID == objEzeeMoneyPayout.Id).ToList();
                FileName = "Ezee Money Payout Report From " + objEzeeMoneyPayout.FromDate.ToString("dd/MM/yyyy") + " " + objEzeeMoneyPayout.ToDate.ToString("dd/MM/yyyy");
            }

            gv.DataSource = list_rep.Select(p => new { p.UserLoginId, p.Name, p.EmailId, p.PhoneNo, Status = p.Status.ToString(), p.DelOrdCount, p.TotalOrdAmt, p.TotalRetailPoints, p.QRP, p.ERP, p.EzeeMoney, p.OrdCode, p.TransID }); //Yashaswi 22-1-19 add ord code
            gv.DataBind();


            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + FileName + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);

            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("Index");
        }

        public ActionResult OrderWiseERP() // added by lokesh panwar
        {
            EzeeMoneyPayoutViewModel obj = new EzeeMoneyPayoutViewModel();

            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            year = (month == 1) ? (year - 1) : year;

            obj.FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            obj.ToDate = obj.FromDate.AddMonths(1).AddDays(-1).AddHours(11).AddMinutes(30);



            return View(obj);
        }


        public ActionResult GetOrderWiseERP(EzeeMoneyPayoutViewModel model)
        {
            //EzeeMoneyPayout objEzeeMoneyPayoutDate = db.EzeeMoneyPayouts.Where(p => p.IsPaid == true).OrderByDescending(p => p.PaidDate).Take(1).FirstOrDefault();
            //if (objEzeeMoneyPayoutDate != null)
            //{
            //    if ((model.FromDate <= objEzeeMoneyPayoutDate.FromDate && model.FromDate <= objEzeeMoneyPayoutDate.ToDate) || (model.ToDate <= objEzeeMoneyPayoutDate.FromDate && model.ToDate <= objEzeeMoneyPayoutDate.ToDate))
            //    {
            //        TempData["ErrorMsg"] = "You Cant select this Date, Please Select date greater than Last Payout To date";
            //        return RedirectToAction("Index");
            //    }
            //}
            //List<PayoutFreezeDataReportViewModel> list_rep = new List<PayoutFreezeDataReportViewModel>();
            model = GetRecordOrderWiseERP(model.FromDate, model.ToDate);
            model.isFrzDtDisable = false;
            model.isGoDisable = false;
            model.isPayEzMnyDisable = true;
            return View("OrderWiseERP", model);
        }

        public EzeeMoneyPayoutViewModel GetRecordOrderWiseERP(DateTime FromDate, DateTime ToDate)
        {
            EzeeMoneyPayoutViewModel model = new EzeeMoneyPayoutViewModel();

            var fromDate = new SqlParameter
            {
                ParameterName = "DateFrom",
                Value = FromDate
            };
            var toDate = new SqlParameter
            {
                ParameterName = "DateTo",
                Value = ToDate
            };
            model.isGoClick = true;
            model.ListOrderWiseGrid_ERP = db.Database.SqlQuery<OrderWiseGrid_ERP>("exec CalculateERPForPayout ").ToList<OrderWiseGrid_ERP>();
            //ListOrderWiseGrid_ERP =  db.Database.ExecuteSqlCommand("EXEC CalculateERPForPayout @DateFrom,@DateTo", fromDate, toDate );

            return model;
        }

        public ActionResult Designation_Report() // added by lokesh panwar
        {
            EzeeMoneyPayoutViewModel obj = new EzeeMoneyPayoutViewModel();

            obj.DesignationReportlist = db.Database.SqlQuery<DesignationReport>("exec Sp_GetDesignationReport").ToList<DesignationReport>();



            return View(obj);
        }

        public ActionResult GetDeginationdetails(int Designationid)
        {

            EzeeMoneyPayoutViewModel obj = new EzeeMoneyPayoutViewModel();
            obj.DesignationReportlist = db.Database.SqlQuery<DesignationReport>("exec Sp_GetDesignationReportDetails '" + Designationid + "'").ToList<DesignationReport>();
            return View(obj);
        }



    }
}