using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IronPython.Hosting;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Data.Entity;


namespace BusinessLogicLayer
{

    //public class MLMRequest
    //{
    //    public string userid {get;set;}
    //    public string orderid{get;set;}
    //    public string points_tr{get;set;}
    //    public string order_amt {get;set;}
    //    public string date {get;set;}
    //    public string wallet_pt {get;set;}
    //    public string status {get;set;}
    //}
    public class MLMWalletPoints
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        #region MLM Wallet Transcation
        [HttpPost]
        public string MLMWalletPostRequest(bool IsFullReturn, int Status, long userLoginId, long customerOrderID, decimal businessPointsTotal, decimal payableAmount, DateTime Date, decimal mLMAmountUsed, long ActiveUser)
        {
            string responseString = "1";
            //if (Status == 7 || Status == 9)Removed by Rumana
            BoosterPlanSubscriber IsBoosterPlanOrder = db.BoosterPlanSubscribers.FirstOrDefault(p => p.CustomerOrderId == customerOrderID);

            if (Status == 7)
            {
                mLMAmountUsed = 0;
            }
            try
            {
                if (IsBoosterPlanOrder == null)
                {
                    LeadersOrderTranscation(Status, userLoginId, customerOrderID, businessPointsTotal, payableAmount, mLMAmountUsed, ActiveUser);
                }
                else
                {
                    BoosterPlanOrderTranscation(customerOrderID, Status, userLoginId, businessPointsTotal, payableAmount, IsBoosterPlanOrder, ActiveUser, mLMAmountUsed);
                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                   + Environment.NewLine + ex.Message + Environment.NewLine
                   + ex.InnerException + Environment.NewLine
                   + "[MLMWalletPoints][M:MLMWalletPostRequest]",
                   BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return responseString;
        }


        //Yashaswi 6-8-2018
        class Reference
        {
            public decimal PointsUsed { get; set; }
            public decimal? StatusRef { get; set; }
            public decimal AmountUsed { get; set; }
            public string TrRef { get; set; }
            public decimal CurrentPoints { get; set; }
            public string NetStat { get; set; }
            public string NewAmt { get; set; }

        }
        void LeadersOrderTranscation(int Status, long userLoginId, long customerOrderID, decimal businessPointsTotal, decimal payableAmount, decimal mLMAmountUsed, long ActiveUser)
        {
            string data = "Status : " + Status + " userLoginId : " + userLoginId + " customerOrderID : " + customerOrderID + " payableAmount : " + payableAmount;
            try
            {
                MLMCoinRate objCoin = db.MLMCoinRates.FirstOrDefault(p => p.IsActive == true);
                long CoinId = 0;
                decimal? CoinRate = 0;
                decimal newAmount = 0;
                decimal newPoint = 0;
                long MLMWalletTransactionId = 0;
                decimal CalPoints = 0;
                if (objCoin != null)
                {
                    CoinId = objCoin.ID;
                    CoinRate = objCoin.Rate;
                }
                MLMWallet objMLMWallet = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == userLoginId);
                CashbackWallet wallet = db.cashbackWallets.FirstOrDefault(p => p.UserLoginID == userLoginId);
                if (wallet == null)
                {
                    CreateCashbackWallet(true, userLoginId, ActiveUser, 0, 0);
                }
                if (Status == 1)
                {
                    MLMWalletTransaction objMLMWalletTransaction_ = db.MLMWalletTransactions.FirstOrDefault(p => p.CustomerOrderID == customerOrderID && p.UserLoginID == userLoginId && p.TransactionTypeID == 1);
                    if (objMLMWalletTransaction_ == null)
                    {
                        if (objMLMWallet == null)
                        {
                            SaveMLMWallet(true, userLoginId, ActiveUser, 0, 0, 0);
                        }
                        else
                        {
                            if (objMLMWallet.Amount > 0)
                            {
                                CalPoints = Math.Round(Convert.ToDecimal(mLMAmountUsed / CoinRate), 2);
                                newAmount = (objMLMWallet.Amount >= mLMAmountUsed) ? objMLMWallet.Amount - mLMAmountUsed : 0;
                                newPoint = (objMLMWallet.Points >= CalPoints) ? objMLMWallet.Points - CalPoints : 0;
                            }

                            if (mLMAmountUsed > 0)
                            {
                                CashbackWallet wallet_ = db.cashbackWallets.FirstOrDefault(p => p.UserLoginID == userLoginId);
                                decimal cashWalAmt = wallet_.Amount;
                                decimal RemWallAmtUsed = cashWalAmt < mLMAmountUsed ? mLMAmountUsed - cashWalAmt : 0;

                                if (RemWallAmtUsed == 0)
                                {
                                    CalPoints = Math.Round(Convert.ToDecimal(mLMAmountUsed / CoinRate), 2);
                                    CreateCashbackWallet(false, userLoginId, ActiveUser, (wallet_.Points - CalPoints), (wallet_.Amount - mLMAmountUsed));
                                    CalPoints = Math.Round(Convert.ToDecimal(0 / CoinRate), 2);
                                    newAmount = (objMLMWallet.Amount >= 0) ? objMLMWallet.Amount - 0 : 0;
                                    newPoint = (objMLMWallet.Points >= CalPoints) ? objMLMWallet.Points - CalPoints : 0;
                                    CreateCashbackWalletLog(userLoginId, mLMAmountUsed, 1, customerOrderID);
                                }
                                else
                                {
                                    if (wallet_.Amount != 0)
                                    {
                                        CreateCashbackWallet(false, userLoginId, ActiveUser, 0, 0);
                                        CreateCashbackWalletLog(userLoginId, cashWalAmt, 1, customerOrderID);
                                    }
                                    CalPoints = Math.Round(Convert.ToDecimal(RemWallAmtUsed / CoinRate), 2);
                                    newAmount = (objMLMWallet.Amount >= RemWallAmtUsed) ? objMLMWallet.Amount - RemWallAmtUsed : 0;
                                    newPoint = (objMLMWallet.Points >= CalPoints) ? objMLMWallet.Points - CalPoints : 0;
                                    CreateMLMWalletLog(userLoginId, RemWallAmtUsed, 1, customerOrderID);
                                }
                            }

                        }
                        Reference objReference = new Reference();
                        objReference.TrRef = "WALLET USED";
                        objReference.CurrentPoints = businessPointsTotal;
                        objReference.PointsUsed = CalPoints;
                        objReference.AmountUsed = mLMAmountUsed;

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        string jsonData = js.Serialize(objReference);

                        SaveMLMWalletTransaction(true, Status, userLoginId, customerOrderID, businessPointsTotal, payableAmount, mLMAmountUsed, ActiveUser, CoinId, newAmount, 0, jsonData, out MLMWalletTransactionId);
                        SaveMLMWallet(false, userLoginId, ActiveUser, newPoint, newAmount, MLMWalletTransactionId);
                    }
                }
                else if (Status == 9)
                {
                    MLMWalletTransaction objMLMWalletTransaction = db.MLMWalletTransactions.FirstOrDefault(p => p.CustomerOrderID == customerOrderID && p.TransactionTypeID == 1 && p.UserLoginID == userLoginId);
                    MLMWalletTransaction objMLMWalletTransaction_ = db.MLMWalletTransactions.FirstOrDefault(p => p.CustomerOrderID == customerOrderID && p.TransactionTypeID == 9 && p.UserLoginID == userLoginId);
                    if (objMLMWalletTransaction_ == null)
                    {
                        if (objMLMWalletTransaction != null)
                        {
                            //
                            //newAmount = (decimal)(objMLMWallet.Amount + objMLMWalletTransaction.WalletAmountUsed);
                            //JavaScriptSerializer js = new JavaScriptSerializer();
                            //Reference objRef = js.Deserialize<Reference>(objMLMWalletTransaction.Reference);
                            //CalPoints = objRef.PointsUsed;
                            //newPoint = objMLMWallet.Points + CalPoints;
                            //SaveMLMWalletTransaction(true, Status, userLoginId, customerOrderID, objMLMWalletTransaction.TransactionPoints, objMLMWalletTransaction.OrderAmount, objMLMWalletTransaction.WalletAmountUsed, ActiveUser, CoinId, objMLMWalletTransaction.CurrentWalletAmount, 1, objMLMWalletTransaction.Reference, out MLMWalletTransactionId);
                            SaveMLMWalletTransaction(true, Status, userLoginId, customerOrderID, objMLMWalletTransaction.TransactionPoints, payableAmount, objMLMWalletTransaction.WalletAmountUsed, ActiveUser, CoinId, objMLMWalletTransaction.CurrentWalletAmount, 1, objMLMWalletTransaction.Reference, out MLMWalletTransactionId);
                            //SaveMLMWallet(false, userLoginId, ActiveUser, newPoint, newAmount, MLMWalletTransactionId);
                        }
                    }
                }
                else if (Status == 8)
                {
                    MLMWalletTransaction objMLMWalletTransaction = db.MLMWalletTransactions.FirstOrDefault(p => p.CustomerOrderID == customerOrderID && p.TransactionTypeID == 7 && p.UserLoginID == userLoginId);
                    if (objMLMWalletTransaction != null)
                    {
                        //newAmount = (decimal)(objMLMWallet.Amount + objMLMWalletTransaction.WalletAmountUsed);
                        //JavaScriptSerializer js = new JavaScriptSerializer();
                        //Reference objRef = js.Deserialize<Reference>(objMLMWalletTransaction.Reference);
                        //CalPoints = objRef.PointsUsed;
                        //newPoint = objMLMWallet.Points + CalPoints;
                        decimal deliveryCharges = IsCompleteOrderReturn(customerOrderID);
                        payableAmount = payableAmount + deliveryCharges;

                        SaveMLMWalletTransaction(true, Status, userLoginId, customerOrderID, businessPointsTotal, payableAmount, objMLMWalletTransaction.WalletAmountUsed, ActiveUser, CoinId, objMLMWalletTransaction.CurrentWalletAmount, 1, objMLMWalletTransaction.Reference, out MLMWalletTransactionId);
                        //SaveMLMWallet(false, userLoginId, ActiveUser, newPoint, newAmount, MLMWalletTransactionId);


                        decimal newbusinessPointsTotal = objMLMWalletTransaction.TransactionPoints - businessPointsTotal;
                        decimal newpayableAmount = objMLMWalletTransaction.OrderAmount - payableAmount;
                        decimal newmLMAmountUsed = 0;


                        //newpayableAmount = objMLMWalletTransaction.OrderAmount - payableAmount;

                        //Need to work on MLM wallet Amount Used on Partial Return

                        SaveMLMWalletTransaction(false, Status, userLoginId, customerOrderID, newbusinessPointsTotal, newpayableAmount, newmLMAmountUsed, ActiveUser, CoinId, objMLMWalletTransaction.CurrentWalletAmount, 1, objMLMWalletTransaction.Reference, out MLMWalletTransactionId);
                        SaveMLMWallet_DirectIncome(false, MLMWalletTransactionId, (double)newbusinessPointsTotal, userLoginId, ActiveUser);

                    }
                }
                else if (Status == 7)
                {
                    MLMWalletTransaction objMLMWallet_Transaction = db.MLMWalletTransactions.FirstOrDefault(p => p.CustomerOrderID == customerOrderID && p.TransactionTypeID == 1 && p.UserLoginID == userLoginId);
                    MLMWalletTransaction objMLMWalletTransaction_ = db.MLMWalletTransactions.FirstOrDefault(p => p.CustomerOrderID == customerOrderID && p.UserLoginID == userLoginId && p.TransactionTypeID == 7);
                    if (objMLMWalletTransaction_ == null)
                    {
                        MLMWalletTransaction objMLMWalletTransaction = db.MLMWalletTransactions.FirstOrDefault(p => p.CustomerOrderID == customerOrderID && p.UserLoginID == userLoginId);
                        SaveMLMWalletTransaction(true, Status, userLoginId, customerOrderID, businessPointsTotal, payableAmount, objMLMWallet_Transaction.WalletAmountUsed, ActiveUser, CoinId, objMLMWallet.Amount, 2, objMLMWalletTransaction.Reference, out MLMWalletTransactionId);
                        SaveMLMWallet_DirectIncome(true, MLMWalletTransactionId, (double)businessPointsTotal, userLoginId, ActiveUser);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("MLMWalletPoints : LeadersOrderTranscation() " + ex.Message + " Data " + data);
            }

        }

        void SaveMLMWalletTransaction(bool IsInsert, int Status, long userLoginId, long customerOrderID, decimal businessPointsTotal, decimal payableAmount, decimal? mLMAmountUsed, long ActiveUser, long CoinId, decimal? CurrentWalletAmount, long AddOrSub, string Reference, out long MLMWalletTransactionId)
        {
            try
            {
                if (IsInsert)
                {
                    MLMWalletTransaction objMLMWalletTransaction = new MLMWalletTransaction();
                    objMLMWalletTransaction.UserLoginID = userLoginId;
                    objMLMWalletTransaction.TransactionTypeID = Status;
                    objMLMWalletTransaction.TransactionPoints = businessPointsTotal;
                    objMLMWalletTransaction.MLMCoinRateID = CoinId;
                    objMLMWalletTransaction.OrderAmount = payableAmount;
                    objMLMWalletTransaction.Reference = Reference;
                    objMLMWalletTransaction.CustomerOrderID = customerOrderID;
                    objMLMWalletTransaction.IsAdded = true;
                    objMLMWalletTransaction.CurrentWalletAmount = CurrentWalletAmount;
                    objMLMWalletTransaction.AddOrSub = AddOrSub;
                    objMLMWalletTransaction.WalletAmountUsed = mLMAmountUsed;
                    objMLMWalletTransaction.CreateDate = DateTime.Now;
                    objMLMWalletTransaction.CreateBy = ActiveUser;
                    objMLMWalletTransaction.NetworkIP = CommonFunctions.GetClientIP();
                    objMLMWalletTransaction.DeviceID = "X";
                    objMLMWalletTransaction.DeviceType = "X";
                    db.MLMWalletTransactions.Add(objMLMWalletTransaction);
                    db.SaveChanges();
                    MLMWalletTransactionId = objMLMWalletTransaction.ID;
                }
                else
                {
                    MLMWalletTransaction objMLMWalletTransaction = db.MLMWalletTransactions.FirstOrDefault(p => p.UserLoginID == userLoginId && p.CustomerOrderID == customerOrderID && p.TransactionTypeID == 7);
                    objMLMWalletTransaction.OrderAmount = payableAmount;
                    objMLMWalletTransaction.TransactionPoints = businessPointsTotal;
                    objMLMWalletTransaction.WalletAmountUsed = mLMAmountUsed;
                    objMLMWalletTransaction.CurrentWalletAmount = CurrentWalletAmount;
                    objMLMWalletTransaction.ModifyDate = DateTime.Now;
                    objMLMWalletTransaction.ModifyBy = ActiveUser;
                    db.SaveChanges();
                    MLMWalletTransactionId = objMLMWalletTransaction.ID;
                }
            }
            catch (Exception ex)
            {
                MLMWalletTransactionId = 0;
                throw new Exception("MLMWalletPoints : SaveMLMWalletTransaction() " + ex.Message);
            }
        }

        void CreateCashbackWalletLog(long UserLoginId, decimal Amount, int Type, long customerOrderId)
        {
            string TransactionType = "";
            string Remark = "";
            switch (Type)
            {
                case 1:
                    TransactionType = "Debit";
                    Remark = "Amount used in Order";
                    break;
                case 2:
                    TransactionType = "Credit";
                    Remark = "Refund for partially cancelled order in cashback wallet";
                    break;
                case 3:
                    TransactionType = "Credit";
                    Remark = "Refund for cancelled order in cashback wallet";
                    break;
                case 4:
                    TransactionType = "Credit";
                    Remark = "Refund for order returned order in cashback wallet";
                    break;
                case 5:
                    TransactionType = "Credit";
                    Remark = "Refund for partialy returned order in cashback wallet";
                    break;
                case 6:
                    TransactionType = "Credit";
                    Remark = "Refund for abandoned order in cashback wallet";
                    break;
            }
            CashbackWalletLog log = new CashbackWalletLog();
            log.UserLoginID = UserLoginId;
            log.CustomerOrderID = customerOrderId;
            log.CashbackPayoutID = 0;
            log.Amount = Amount;
            log.TransactionType = TransactionType;
            log.Remark = Remark;
            log.ModifyDate = DateTime.Now;
            db.cashbackWallLog.Add(log);
            db.SaveChanges();
        }

        void CreateMLMWalletLog(long UserLoginId, decimal Amount, int Type, long customerOrderId)
        {
            string Remark = "";
            switch (Type)
            {
                case 1:
                    Remark = "Amount used in Order"; //Do not changes text used in Refund method
                    break;
                case 2:
                    Remark = "Refund for partially cancelled order in wallet";
                    break;
                case 3:
                    Remark = "Refund for cancelled order in wallet";
                    break;
                case 4:
                    Remark = "Refund for order returned order in wallet";
                    break;
                case 5:
                    Remark = "Refund for partialy returned order in wallet";
                    break;
                case 6:
                    Remark = "Refund for abandoned order in wallet";
                    break;
            }
            EWalletRefund_Table log = new EWalletRefund_Table();
            log.UserLoginId = UserLoginId;
            log.CustomerOrderId = customerOrderId;
            log.RequsetAmt = Amount;
            log.RefundAmt = Amount;
            log.Comment = Remark;
            log.Date = DateTime.Now;
            if (Type == 1)
            {
                log.Status = 0;
                log.Isactive = false;
            }
            else
            {
                log.Status = 1;
                log.Isactive = true;
            }
            db.eWalletRefund_Table.Add(log);
            db.SaveChanges();
        }
        void CreateCashbackWallet(bool isInsert, long userLoginId, long ActiveUser, decimal Point, decimal Amount)
        {
            if (isInsert)
            {
                CashbackWallet wallet = new CashbackWallet();
                wallet.UserLoginID = userLoginId;
                wallet.Points = Point;
                wallet.Amount = Amount;
                wallet.ModifyBy = ActiveUser;
                wallet.ModifyDate = DateTime.Now;
                db.cashbackWallets.Add(wallet);
                db.SaveChanges();
            }
            else
            {
                CashbackWallet wallet = db.cashbackWallets.FirstOrDefault(p => p.UserLoginID == userLoginId);
                wallet.Points = Point;
                wallet.Amount = Amount;
                wallet.ModifyBy = ActiveUser;
                wallet.ModifyDate = DateTime.Now;
                db.SaveChanges();
            }
        }
        void SaveMLMWallet(bool IsInsert, long userLoginId, long ActiveUser, decimal Point, decimal Amount, long OrderId)
        {
            try
            {
                if (IsInsert)
                {
                    MLMWallet objMLMWallet = new MLMWallet();
                    objMLMWallet.UserLoginID = userLoginId;
                    objMLMWallet.Points = Point;
                    objMLMWallet.Amount = Amount;
                    objMLMWallet.IsMLMUser = true;
                    objMLMWallet.LastWalletTransactionID = OrderId;
                    objMLMWallet.IsActive = false;
                    objMLMWallet.LastModifyBy = ActiveUser;
                    objMLMWallet.LastModifyDate = DateTime.Now;
                    db.MLMWallets.Add(objMLMWallet);
                    db.SaveChanges();
                }
                else
                {
                    MLMWallet objMLMWallet = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == userLoginId);
                    objMLMWallet.Points = Point;
                    objMLMWallet.Amount = Amount;
                    objMLMWallet.LastWalletTransactionID = OrderId;
                    objMLMWallet.LastModifyDate = DateTime.Now;
                    objMLMWallet.LastModifyBy = ActiveUser;
                    objMLMWallet.IsMLMUser = true;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("MLMWalletPoints : SaveMLMWallet() " + ex.Message);
            }
        }


        void Distribute_Designation(long Userloginid)
        {
            List<TabularView> list = new List<TabularView>();

            var idParam = new SqlParameter
            {
                ParameterName = "UserID",
                Value = Userloginid
            };

            db.Database.SqlQuery<TabularView>("EXEC Sp_DistributeDesignationForAllLevel @UserID", idParam).ToList<TabularView>();


        }

        void SaveMLMWallet_DirectIncome(bool IsInsert, long MLMWalletTransactionId, double TransactionPoints, long UserLoginId, long ActiveUser)
        {
            try
            {
                //double current = 0.0666;
                //double Up1 = 0.13;
                //double Up2 = 0.14;
                //double Up3 = 0.15;
                //double Up4 = 0.16;
                //double up5 = 0.17;
                //double up6 = 0.25;

                // changes by lokesh panwar



                double current = GetUpLineValue(0);
                double Up1 = GetUpLineValue(1);
                double Up2 = GetUpLineValue(2);
                double Up3 = GetUpLineValue(3);
                double Up4 = GetUpLineValue(4);
                double Up5 = GetUpLineValue(5);
                double Up6 = GetUpLineValue(6);

                double Up7 = GetUpLineValue(7);
                double Up8 = GetUpLineValue(8);
                double Up9 = GetUpLineValue(9);
                double Up10 = GetUpLineValue(10);
                double Up11 = GetUpLineValue(11);
                double Up12 = GetUpLineValue(12);
                double Up13 = GetUpLineValue(13);
                double Up14 = GetUpLineValue(14);
                double Up15 = GetUpLineValue(15);
                double Up16 = GetUpLineValue(16);

                double UpR7 = GetUpLineValue(7);
                double UpR8 = GetUpLineValue(8);
                double UpR9 = GetUpLineValue(9);
                double UpR10 = GetUpLineValue(10);
                double UpR11 = GetUpLineValue(11);
                double UpR12 = GetUpLineValue(12);
                double UpR13 = GetUpLineValue(13);
                double UpR14 = GetUpLineValue(14);
                double UpR15 = GetUpLineValue(15);
                double UpR16 = GetUpLineValue(16);






                long CurrentLevel_UserLoginId = UserLoginId;
                double CurrentLevel = TransactionPoints * current;

                Distribute_Designation(UserLoginId);


                long UpLine1_UserLoginId = GetUpLine(UserLoginId);
                double UpLine1 = TransactionPoints * Up1;

                long UpLine2_UserLoginId = GetUpLine(UpLine1_UserLoginId);
                double UpLine2 = TransactionPoints * Up2;

                long UpLine3_UserLoginId = GetUpLine(UpLine2_UserLoginId);
                double UpLine3 = TransactionPoints * Up3;

                long UpLine4_UserLoginId = GetUpLine(UpLine3_UserLoginId);
                double UpLine4 = TransactionPoints * Up4;

                long UpLine5_UserLoginId = GetUpLine(UpLine4_UserLoginId);
                double UpLine5 = TransactionPoints * Up5;

                long UpLine6_UserLoginId = GetUpLine(UpLine5_UserLoginId);
                double UpLine6 = TransactionPoints * Up6;



                long UpLine7_UserLoginId = GetUpLine(UpLine6_UserLoginId);
                long UpLine7IsRoyaltyActiver = GetUpLineRoraltyactiver(UpLine7_UserLoginId);
                long UpLine7IsLifestyleActiver = GetUpLineLifestyleactiver(UpLine7_UserLoginId);
                double UpLine7 = TransactionPoints * Up7 * UpLine7IsRoyaltyActiver;
                double UpLineR7 = TransactionPoints * Up7 * UpLine7IsRoyaltyActiver;


                long UpLine8_UserLoginId = GetUpLine(UpLine7_UserLoginId);
                long UpLine8IsRoyaltyActiver = GetUpLineRoraltyactiver(UpLine8_UserLoginId);
                long UpLine8IsLifestyleActiver = GetUpLineLifestyleactiver(UpLine8_UserLoginId);
                double UpLine8 = TransactionPoints * Up8 * UpLine8IsLifestyleActiver;
                double UpLineR8 = TransactionPoints * Up8 * UpLine8IsRoyaltyActiver;

                long UpLine9_UserLoginId = GetUpLine(UpLine8_UserLoginId);
                long UpLine9IsLifestyleActiver = GetUpLineLifestyleactiver(UpLine9_UserLoginId);
                long UpLine9IsRoyaltyActiver = GetUpLineRoraltyactiver(UpLine9_UserLoginId);
                double UpLine9 = TransactionPoints * Up9 * UpLine9IsLifestyleActiver;
                double UpLineR9 = TransactionPoints * Up9 * UpLine9IsRoyaltyActiver;

                long UpLine10_UserLoginId = GetUpLine(UpLine9_UserLoginId);
                long UpLine10IsLifestyleActiver = GetUpLineLifestyleactiver(UpLine10_UserLoginId);
                long UpLine10IsRoyaltyActiver = GetUpLineRoraltyactiver(UpLine10_UserLoginId);
                double UpLine10 = TransactionPoints * Up10 * UpLine10IsLifestyleActiver;
                double UpLineR10 = TransactionPoints * Up10 * UpLine10IsRoyaltyActiver;

                long UpLine11_UserLoginId = GetUpLine(UpLine10_UserLoginId);
                long UpLine11IsLifestyleActiver = GetUpLineLifestyleactiver(UpLine11_UserLoginId);
                long UpLine11IsRoyaltyActiver = GetUpLineRoraltyactiver(UpLine11_UserLoginId);
                double UpLine11 = TransactionPoints * Up11 * UpLine11IsLifestyleActiver;
                double UpLineR11 = TransactionPoints * Up11 * UpLine11IsRoyaltyActiver;


                long UpLine12_UserLoginId = GetUpLine(UpLine11_UserLoginId);
                long UpLine12IsLifestyleActiver = GetUpLineLifestyleactiver(UpLine12_UserLoginId);
                long UpLine12IsRoyaltyActiver = GetUpLineRoraltyactiver(UpLine12_UserLoginId);
                double UpLine12 = TransactionPoints * Up12 * UpLine12IsLifestyleActiver;
                double UpLineR12 = TransactionPoints * Up12 * UpLine12IsRoyaltyActiver;

                long UpLine13_UserLoginId = GetUpLine(UpLine12_UserLoginId);
                long UpLine13IsLifestyleActiver = GetUpLineLifestyleactiver(UpLine13_UserLoginId);
                long UpLine13IsRoyaltyActiver = GetUpLineRoraltyactiver(UpLine13_UserLoginId);
                double UpLine13 = TransactionPoints * Up13 * UpLine13IsLifestyleActiver;
                double UpLineR13 = TransactionPoints * Up13 * UpLine13IsRoyaltyActiver;

                long UpLine14_UserLoginId = GetUpLine(UpLine13_UserLoginId);
                long UpLine14IsLifestyleActiver = GetUpLineLifestyleactiver(UpLine14_UserLoginId);
                long UpLine14IsRoyaltyActiver = GetUpLineRoraltyactiver(UpLine14_UserLoginId);
                double UpLine14 = TransactionPoints * Up14 * UpLine14IsLifestyleActiver;
                double UpLineR14 = TransactionPoints * Up14 * UpLine14IsRoyaltyActiver;

                long UpLine15_UserLoginId = GetUpLine(UpLine14_UserLoginId);
                long UpLine15IsLifestyleActiver = GetUpLineLifestyleactiver(UpLine15_UserLoginId);
                long UpLine15IsRoyaltyActiver = GetUpLineRoraltyactiver(UpLine15_UserLoginId);
                double UpLine15 = TransactionPoints * Up15 * UpLine15IsLifestyleActiver;
                double UpLineR15 = TransactionPoints * Up15 * UpLine15IsRoyaltyActiver;

                long UpLine16_UserLoginId = GetUpLine(UpLine15_UserLoginId);
                long UpLine16IsLifestyleActiver = GetUpLineLifestyleactiver(UpLine16_UserLoginId);
                long UpLine16IsRoyaltyActiver = GetUpLineRoraltyactiver(UpLine16_UserLoginId);
                double UpLine16 = TransactionPoints * Up16 * UpLine16IsLifestyleActiver;
                double UpLineR16 = TransactionPoints * Up16 * UpLine16IsRoyaltyActiver;




                if (IsInsert)
                {
                    MLMWallet_DirectIncome objMLMWallet_DirectIncome = new MLMWallet_DirectIncome();
                    objMLMWallet_DirectIncome.MLMWalletTransactionId = MLMWalletTransactionId;
                    objMLMWallet_DirectIncome.TransactionPoints = TransactionPoints;
                    objMLMWallet_DirectIncome.CurrentLevel = CurrentLevel;
                    objMLMWallet_DirectIncome.CurrentLevel_UserLoginId = CurrentLevel_UserLoginId;
                    objMLMWallet_DirectIncome.UpLine1 = UpLine1;
                    objMLMWallet_DirectIncome.UpLine1_UserLoginId = UpLine1_UserLoginId;
                    objMLMWallet_DirectIncome.UpLine2 = UpLine2;
                    objMLMWallet_DirectIncome.UpLine2_UserLoginId = UpLine2_UserLoginId;
                    objMLMWallet_DirectIncome.UpLine3 = UpLine3;
                    objMLMWallet_DirectIncome.UpLine3_UserLoginId = UpLine3_UserLoginId;
                    objMLMWallet_DirectIncome.UpLine4 = UpLine4;
                    objMLMWallet_DirectIncome.UpLine4_UserLoginId = UpLine4_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine5 = UpLine5;
                    objMLMWallet_DirectIncome.UpLine5_UserLoginId = UpLine5_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine6 = UpLine6;
                    objMLMWallet_DirectIncome.UpLine6_UserLoginId = UpLine6_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine7 = UpLine7;
                    objMLMWallet_DirectIncome.UpLineR7 = UpLineR7;
                    objMLMWallet_DirectIncome.UpLine7_UserLoginId = UpLine7_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine8 = UpLine8;
                    objMLMWallet_DirectIncome.UpLineR8 = UpLineR8;
                    objMLMWallet_DirectIncome.UpLine8_UserLoginId = UpLine8_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine9 = UpLine9;
                    objMLMWallet_DirectIncome.UpLineR9 = UpLineR9;
                    objMLMWallet_DirectIncome.UpLine9_UserLoginId = UpLine9_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine10 = UpLine10;
                    objMLMWallet_DirectIncome.UpLineR10 = UpLineR10;
                    objMLMWallet_DirectIncome.UpLine10_UserLoginId = UpLine10_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine11 = UpLine11;
                    objMLMWallet_DirectIncome.UpLineR11 = UpLineR11;
                    objMLMWallet_DirectIncome.UpLine11_UserLoginId = UpLine11_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine12 = UpLine12;
                    objMLMWallet_DirectIncome.UpLineR12 = UpLineR12;
                    objMLMWallet_DirectIncome.UpLine12_UserLoginId = UpLine12_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine13 = UpLine13;
                    objMLMWallet_DirectIncome.UpLineR13 = UpLineR13;
                    objMLMWallet_DirectIncome.UpLine13_UserLoginId = UpLine13_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine14 = UpLine14;
                    objMLMWallet_DirectIncome.UpLineR14 = UpLineR14;
                    objMLMWallet_DirectIncome.UpLine14_UserLoginId = UpLine14_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine15 = UpLine15;
                    objMLMWallet_DirectIncome.UpLineR15 = UpLineR15;
                    objMLMWallet_DirectIncome.UpLine15_UserLoginId = UpLine15_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine16 = UpLine16;
                    objMLMWallet_DirectIncome.UpLineR16 = UpLineR16;
                    objMLMWallet_DirectIncome.UpLine16_UserLoginId = UpLine16_UserLoginId;


                    objMLMWallet_DirectIncome.CreateDate = DateTime.Now;
                    objMLMWallet_DirectIncome.CreateBy = ActiveUser;
                    db.MLMWallet_DirectIncomes.Add(objMLMWallet_DirectIncome);
                    db.SaveChanges();
                }
                else
                {
                    MLMWallet_DirectIncome objMLMWallet_DirectIncome = db.MLMWallet_DirectIncomes.FirstOrDefault(p => p.MLMWalletTransactionId == MLMWalletTransactionId && p.CurrentLevel_UserLoginId == UserLoginId);
                    objMLMWallet_DirectIncome.TransactionPoints = TransactionPoints;
                    objMLMWallet_DirectIncome.CurrentLevel = CurrentLevel;
                    objMLMWallet_DirectIncome.CurrentLevel_UserLoginId = CurrentLevel_UserLoginId;
                    objMLMWallet_DirectIncome.UpLine1 = UpLine1;
                    objMLMWallet_DirectIncome.UpLine1_UserLoginId = UpLine1_UserLoginId;
                    objMLMWallet_DirectIncome.UpLine2 = UpLine2;
                    objMLMWallet_DirectIncome.UpLine2_UserLoginId = UpLine2_UserLoginId;
                    objMLMWallet_DirectIncome.UpLine3 = UpLine3;
                    objMLMWallet_DirectIncome.UpLine3_UserLoginId = UpLine3_UserLoginId;
                    objMLMWallet_DirectIncome.UpLine4 = UpLine4;


                    objMLMWallet_DirectIncome.UpLine5 = UpLine5;
                    objMLMWallet_DirectIncome.UpLine5_UserLoginId = UpLine5_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine6 = UpLine6;
                    objMLMWallet_DirectIncome.UpLine6_UserLoginId = UpLine6_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine7 = UpLine7;
                    objMLMWallet_DirectIncome.UpLineR7 = UpLineR7;
                    objMLMWallet_DirectIncome.UpLine7_UserLoginId = UpLine7_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine8 = UpLine8;
                    objMLMWallet_DirectIncome.UpLineR8 = UpLineR8;
                    objMLMWallet_DirectIncome.UpLine8_UserLoginId = UpLine8_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine9 = UpLine9;
                    objMLMWallet_DirectIncome.UpLineR9 = UpLineR9;
                    objMLMWallet_DirectIncome.UpLine9_UserLoginId = UpLine9_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine10 = UpLine10;
                    objMLMWallet_DirectIncome.UpLineR10 = UpLineR10;
                    objMLMWallet_DirectIncome.UpLine10_UserLoginId = UpLine10_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine11 = UpLine11;
                    objMLMWallet_DirectIncome.UpLineR11 = UpLineR11;
                    objMLMWallet_DirectIncome.UpLine11_UserLoginId = UpLine11_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine12 = UpLine12;
                    objMLMWallet_DirectIncome.UpLineR12 = UpLineR12;
                    objMLMWallet_DirectIncome.UpLine12_UserLoginId = UpLine12_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine13 = UpLine13;
                    objMLMWallet_DirectIncome.UpLineR13 = UpLineR13;
                    objMLMWallet_DirectIncome.UpLine13_UserLoginId = UpLine13_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine14 = UpLine14;
                    objMLMWallet_DirectIncome.UpLineR14 = UpLineR14;
                    objMLMWallet_DirectIncome.UpLine14_UserLoginId = UpLine14_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine15 = UpLine15;
                    objMLMWallet_DirectIncome.UpLineR15 = UpLineR15;
                    objMLMWallet_DirectIncome.UpLine15_UserLoginId = UpLine15_UserLoginId;

                    objMLMWallet_DirectIncome.UpLine16 = UpLine16;
                    objMLMWallet_DirectIncome.UpLineR16 = UpLineR16;
                    objMLMWallet_DirectIncome.UpLine16_UserLoginId = UpLine16_UserLoginId;


                    objMLMWallet_DirectIncome.UpLine4_UserLoginId = UpLine4_UserLoginId;
                    objMLMWallet_DirectIncome.ModifyDate = DateTime.Now;
                    objMLMWallet_DirectIncome.ModifyBy = ActiveUser;
                    db.SaveChanges();
                }
                // UpLine1_UserLoginId update degination upline 
                Distribute_Designation(UserLoginId);


            }
            catch (Exception ex)
            {
                throw new Exception("MLMWalletPoints : SaveMLMWallet_DirectIncome() " + ex.Message);
            }
        }

      public  long GetUpLine(long UserLoginId)
        {
            try
            {
                MLMUser obj = db.MLMUsers.FirstOrDefault(q => q.Ref_Id == db.MLMUsers.FirstOrDefault(p => p.UserID == UserLoginId).Refered_Id_ref);
                return (obj == null) ? 0 : obj.UserID;
            }
            catch (Exception ex)
            {
                throw new Exception("MLMWalletPoints : GetUpLine() " + ex.Message);
            }
        }


        long GetUpLineRoraltyactiver(long UserLoginId)
        {
            long? value = 0;
            try
            {
                value = db.MLMUsers.FirstOrDefault(q => q.UserID == UserLoginId).isroyaltyachiever;
                return (value == null) ? 0 : value.Value;

            }
            catch (Exception ex)
            {
                return (value == null) ? 0 : value.Value;
            }
        }

        long GetUpLineLifestyleactiver(long UserLoginId)
        {
            long? value = 0;
            try
            {
                value = db.MLMUsers.FirstOrDefault(q => q.UserID == UserLoginId).islifestyleachiever;
                return (value == null) ? 0 : value.Value;
            }
            catch (Exception ex)
            {
                return (value == null) ? 0 : value.Value;
            }
        }




      public  double GetUpLineValue(int Col)
        {
            double? value = 0;

            switch (Col)
            {
                case 0:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).Level0;
                    break;
                case 1:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).Level1;
                    break;
                case 2:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).Level2;
                    break;
                case 3:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).Level3;
                    break;
                case 4:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).Level4;
                    break;

                case 5:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).Level5;
                    break;

                case 6:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).Level6;
                    break;

                case 7:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).LevelR7;

                    break;
                case 8:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).LevelR8;
                    break;
                case 9:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).LevelR9;
                    break;
                case 10:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).LevelR10;
                    break;
                case 11:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).LevelR11;
                    break;
                case 12:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).LevelR12;
                    break;
                case 13:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).LevelR13;
                    break;
                case 14:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).LevelR14;
                    break;
                case 15:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).LevelR15;
                    break;
                case 16:
                    value = db.LeadersIncomeMasters.FirstOrDefault(p => p.ID == 1).LevelR16;
                    break;


            }

            return Convert.ToDouble(value);
        }

        decimal IsCompleteOrderReturn(long CustomerOrderId)
        {
            decimal DeliveryCharge = 0;
            int TotalItems = db.CustomerOrderDetails.Where(p => p.CustomerOrderID == CustomerOrderId).Count();
            int TotalReturnItems = db.CustomerOrderDetails.Where(p => p.CustomerOrderID == CustomerOrderId && p.OrderStatus == 8).Count();
            if (TotalItems == TotalReturnItems)
            {
                string ShopOrderCode = db.CustomerOrderDetails.FirstOrDefault(p => p.CustomerOrderID == CustomerOrderId && p.OrderStatus == 8).ShopOrderCode;
                DeliveryCharge = db.DeliveryOrderDetails.FirstOrDefault(p => p.ShopOrderCode == ShopOrderCode).DeliveryCharge;
            }
            return DeliveryCharge;
        }

        public void BoosterPlanOrderTranscation(long CustomerOrderId, int Status, long UserloginId, decimal RetailPonts, decimal OrderAmount, BoosterPlanSubscriber planSubscriber, long ActiveUser, decimal WalAmtUsed)
        {
            try
            {
                BoosterPlanSubscriberTranscations obj = db.planSubscriberTranscations.FirstOrDefault(p => p.CustomerOrderID == CustomerOrderId && p.Status == Status && p.BoosterPlanSubscriberID == planSubscriber.ID);
                if (obj == null)
                {
                    BoosterPlanSubscriberTranscations transcations = new BoosterPlanSubscriberTranscations();
                    transcations.UserLoginID = UserloginId;
                    transcations.CustomerOrderID = CustomerOrderId;
                    transcations.BoosterPlanSubscriberID = planSubscriber.ID;
                    transcations.RetailPoints = RetailPonts;
                    transcations.OrderAmount = OrderAmount;
                    transcations.Status = Status;
                    if (Status == 8 || Status == 9)
                    {
                        transcations.IsActive = false;
                        BoosterPlanSubscriberTranscations obj_ = db.planSubscriberTranscations.FirstOrDefault(p => p.CustomerOrderID == CustomerOrderId && p.Status == 7 && p.BoosterPlanSubscriberID == planSubscriber.ID);
                        if (obj_ != null)
                        {
                            obj_.IsActive = false;
                            obj_.ModifyDate = DateTime.Now;
                            obj_.ModifyBy = UserloginId;
                            obj_.NetworkIP = CommonFunctions.GetClientIP();
                        }
                    }
                    else
                    {
                        transcations.IsActive = true;
                    }
                    transcations.CreateDate = DateTime.Now;
                    transcations.CreateBy = UserloginId;
                    transcations.NetworkIP = CommonFunctions.GetClientIP();
                    db.planSubscriberTranscations.Add(transcations);
                }
                else
                {
                    obj.RetailPoints = RetailPonts;
                    obj.OrderAmount = OrderAmount;
                    obj.ModifyDate = DateTime.Now;
                    obj.ModifyBy = UserloginId;
                    obj.NetworkIP = CommonFunctions.GetClientIP();
                }
                db.SaveChanges();


                MLMWallet objMLMWallet = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == UserloginId);
                if (objMLMWallet == null)
                {
                    SaveMLMWallet(true, UserloginId, ActiveUser, 0, 0, 0);
                }
                CashbackWallet wallet = db.cashbackWallets.FirstOrDefault(p => p.UserLoginID == UserloginId);
                if (wallet == null)
                {
                    CreateCashbackWallet(true, UserloginId, ActiveUser, 0, 0);
                }
                if (Status == 1)
                {
                    if (WalAmtUsed > 0)
                    {
                        //MLMWallet objMLMWallet_ = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == UserloginId);
                        decimal CoinRate = db.MLMCoinRates.FirstOrDefault(p => p.IsActive == true).Rate.Value;
                        //decimal CalPoints = Math.Round(Convert.ToDecimal(WalAmtUsed / CoinRate), 2);
                        //decimal newAmount = (objMLMWallet_.Amount >= WalAmtUsed) ? objMLMWallet_.Amount - WalAmtUsed : 0;
                        //decimal newPoint = (objMLMWallet_.Points >= CalPoints) ? objMLMWallet_.Points - CalPoints : 0;
                        //SaveMLMWallet(false, UserloginId, ActiveUser, newPoint, newAmount, 0);

                        decimal CalPoints = 0;
                        decimal newAmount = 0;
                        decimal newPoint = 0;
                        CashbackWallet wallet_ = db.cashbackWallets.FirstOrDefault(p => p.UserLoginID == UserloginId);
                        decimal cashWalAmt = wallet_.Amount;
                        decimal RemWallAmtUsed = cashWalAmt < WalAmtUsed ? WalAmtUsed - cashWalAmt : 0;

                        if (RemWallAmtUsed == 0)
                        {
                            CalPoints = Math.Round(Convert.ToDecimal(WalAmtUsed / CoinRate), 2);
                            CreateCashbackWallet(false, UserloginId, ActiveUser, (wallet_.Points - CalPoints), (wallet_.Amount - WalAmtUsed));
                            //CalPoints = Math.Round(Convert.ToDecimal(0 / CoinRate), 2);
                            // newAmount = (objMLMWallet.Amount >= 0) ? objMLMWallet.Amount - 0 : 0;
                            //newPoint = (objMLMWallet.Points >= CalPoints) ? objMLMWallet.Points - CalPoints : 0;
                            //SaveMLMWallet(false, UserloginId, ActiveUser, newPoint, newAmount, 0);
                            CreateCashbackWalletLog(UserloginId, WalAmtUsed, 1, CustomerOrderId);
                        }
                        else
                        {
                            CreateCashbackWallet(false, UserloginId, ActiveUser, 0, 0);
                            CalPoints = Math.Round(Convert.ToDecimal(RemWallAmtUsed / CoinRate), 2);
                            newAmount = (objMLMWallet.Amount >= RemWallAmtUsed) ? objMLMWallet.Amount - RemWallAmtUsed : 0;
                            newPoint = (objMLMWallet.Points >= CalPoints) ? objMLMWallet.Points - CalPoints : 0;
                            SaveMLMWallet(false, UserloginId, ActiveUser, newPoint, newAmount, 0);
                            CreateCashbackWalletLog(UserloginId, cashWalAmt, 1, CustomerOrderId);
                            CreateMLMWalletLog(UserloginId, RemWallAmtUsed, 1, CustomerOrderId);
                        }
                    }
                }
                if (Status == 7 && RetailPonts >= db.QRPMasters.FirstOrDefault(p => p.ID == 1).Current_QRP)
                {
                    //Update designation if user is prospect
                    MLMUser mLMUser = db.MLMUsers.FirstOrDefault(p => p.UserID == UserloginId && p.CURRENTMONTHDESIGNTAIONID == 0);
                    if (mLMUser != null)
                    {
                        mLMUser.DESIGNTAIONID = 1;
                        mLMUser.CURRENTMONTHDESIGNTAIONID = 1;

                        planSubscriber.IsDesignationChanged = true;
                        db.SaveChanges();
                    }
                }
                if (Status == 8 && planSubscriber.IsDesignationChanged)
                {
                    //Rollback designation if user is member because of Booster Plan order
                    MLMUser mLMUser = db.MLMUsers.FirstOrDefault(p => p.UserID == UserloginId && p.CURRENTMONTHDESIGNTAIONID == 1);
                    if (mLMUser != null)
                    {
                        //check for other booster plan order, may be user remains Member
                        var result =
                                (from bps in db.BoosterPlanSubscribers
                                 join co in db.CustomerOrders on bps.CustomerOrderId equals co.ID
                                 join bt in db.planSubscriberTranscations on co.UserLoginID equals bt.UserLoginID
                                 where bt.Status == 7 && bt.IsActive == true && bt.UserLoginID == UserloginId
                                 select new { bt.UserLoginID }).ToList();

                        if (result == null || result.Count() == 0)
                        {
                            //no other booster plan order, eligible to rollaback
                            //Check for repurchase benifits order for current payout cycle, may be user remains Member
                            LeadersDashboard dashboard = new LeadersDashboard();
                            long QRP = dashboard.GetQRP(UserloginId);
                            if (QRP != 0)
                            {
                                //not active, eligible to rollaback
                                //Check for previous payout cycle, may be user remains Member
                                if (!db.EzeeMoneyPayoutDetail.Any(p => p.UserLoginId == UserloginId && Status == 1))
                                {
                                    //not active in any payout cycle, eligible to rollaback
                                    mLMUser.DESIGNTAIONID = 0;
                                    mLMUser.CURRENTMONTHDESIGNTAIONID = 0;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public void RefundUsedWalletAmount(decimal UsedAmount, long CustomerOrderID, int Type, long userLoginId, int status)
        {
            try
            {
                //Type 1 for complete refund
                //Type 2 for partial refund
                decimal CoinRate = db.MLMCoinRates.FirstOrDefault(p => p.IsActive == true).Rate.Value;
                int TT = 0;

                if (status == 9)
                {
                    TT = (Type == 1) ? 3 : 2;
                }
                else if (status == 8)
                {
                    TT = (Type == 2) ? 5 : 4;
                }
                else
                {
                    TT = 6;
                }
                decimal UsedCashbackWaletAmount = 0;
                decimal RemaininAmount = 0;
                CashbackWalletLog log = db.cashbackWallLog.FirstOrDefault(p => p.CustomerOrderID == CustomerOrderID && p.TransactionType == "Debit" && p.UserLoginID == userLoginId);
                if (log != null)
                {
                    //Complete Order return or Cancelled
                    UsedCashbackWaletAmount = UsedAmount > log.Amount ? log.Amount : UsedAmount;
                    List<CashbackWalletLog> log_ = db.cashbackWallLog.Where(p => p.CustomerOrderID == CustomerOrderID && p.TransactionType == "Credit" && p.UserLoginID == userLoginId).ToList();
                    if (log_.Count() != 0)
                    {
                        decimal refundedAmt = log_.Sum(p => p.Amount);
                        decimal RemaingRefundAmount = log.Amount - refundedAmt;
                        UsedCashbackWaletAmount = UsedCashbackWaletAmount > RemaingRefundAmount ? RemaingRefundAmount : UsedCashbackWaletAmount;
                    }
                    CashbackWallet wallet = db.cashbackWallets.FirstOrDefault(p => p.UserLoginID == userLoginId);
                    if (wallet != null)
                    {
                        //To refund Cashback wallete amount
                        if (UsedCashbackWaletAmount > 0)
                        {
                            decimal CalPoints = Math.Round(Convert.ToDecimal(UsedCashbackWaletAmount / CoinRate), 2);
                            decimal NewPt = wallet.Points + CalPoints;
                            decimal NewAmt = wallet.Amount + UsedCashbackWaletAmount;
                            CreateCashbackWallet(false, userLoginId, userLoginId, NewPt, NewAmt);
                            CreateCashbackWalletLog(userLoginId, UsedCashbackWaletAmount, TT, CustomerOrderID);
                        }
                    }
                }

                if (UsedCashbackWaletAmount != UsedAmount)
                {
                    EWalletRefund_Table Usedlog_ = db.eWalletRefund_Table.FirstOrDefault(p => p.Status == 0 && p.Isactive == false && p.CustomerOrderId == CustomerOrderID);
                    RemaininAmount = UsedAmount - UsedCashbackWaletAmount;
                    if (Usedlog_ != null)
                    {
                        List<EWalletRefund_Table> log_ = db.eWalletRefund_Table.Where(p => p.Status == 1 && p.Isactive == true && p.CustomerOrderId == CustomerOrderID).ToList();
                        if (log_.Count() != 0)
                        {
                            decimal refundedAmt = log_.Sum(p => p.RefundAmt);
                            decimal RemaingRefundAmount = Usedlog_.RequsetAmt - refundedAmt;
                            RemaingRefundAmount = RemaingRefundAmount < 0 ? 0 : RemaingRefundAmount;
                            RemaininAmount = RemaininAmount > RemaingRefundAmount ? RemaingRefundAmount : RemaininAmount;
                        }
                    }
                    if (RemaininAmount > 0)
                    {
                        MLMWallet wallet = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == userLoginId);
                        RemaininAmount = UsedAmount - UsedCashbackWaletAmount;
                        decimal CalPoints = Math.Round(Convert.ToDecimal(RemaininAmount / CoinRate), 2);
                        decimal NewPt = wallet.Points + CalPoints;
                        decimal NewAmt = wallet.Amount + RemaininAmount;
                        SaveMLMWallet(false, userLoginId, userLoginId, NewPt, NewAmt, CustomerOrderID);
                        CreateMLMWalletLog(userLoginId, RemaininAmount, TT, CustomerOrderID);
                    }
                }
            }
            catch
            {

            }
        }
        #endregion

        #region Leaders Registration
        public string LeadersSingUp(long UserLoginID, string Password, string Email, string RefCode)
        {
            string responseString = "0";
            string data = "UserLoginID=" + UserLoginID + "&Password=" + Password + "&Email=" + Email + "&RefCode=" + RefCode;
            try
            {
                RefCode = RefCode.ToUpper().Trim();
                if (db.MLMUsers.Any(p => p.Ref_Id == RefCode))
                {
                    responseString = SaveMLMUser(UserLoginID, Password, Email, RefCode);
                }
                return responseString;
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + ex.InnerException + Environment.NewLine
                    + "[MLMWalletPoints][M:LeadersSingUp]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                return responseString;
            }
        }

        string SaveMLMUser(long UserLoginID, string Password, string Email, string RefCode)
        {
            string result = "0";
            try
            {
                UserLogin objUserLogin = db.UserLogins.FirstOrDefault(p => p.ID == UserLoginID);
                if (objUserLogin != null)
                {
                    MLMUser obj = db.MLMUsers.FirstOrDefault(p => p.UserID == UserLoginID);
                    if (obj == null)
                    {
                        PersonalDetail objPersonalDetail = db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == objUserLogin.ID);
                        DateTime dt = DateTime.Now.Date;
                        string Gender = "Male";
                        if (objPersonalDetail != null)
                        {
                            if (objPersonalDetail.DOB != null)
                            {
                                dt = Convert.ToDateTime(objPersonalDetail.DOB);
                                Gender = objPersonalDetail.Gender;
                            }
                        }
                        string pic = (Gender == "Female") ? "global/portraits/1.png" : "global/portraits/2.png";
                        bool isRefferalCodeUsed = false;
                        string RefferalCode = "";
                        int counter = 5;
                        do
                        {
                            RefferalCode = GenerateRefferalCode(objUserLogin.Mobile.Replace("0", ""), objUserLogin.Email.Replace("0", ""), dt);
                            isRefferalCodeUsed = db.MLMUsers.Any(p => p.Ref_Id == RefferalCode);
                            RefferalCode = (isRefferalCodeUsed == true) ? "" : RefferalCode;
                            counter = counter - 1;
                        }
                        while (isRefferalCodeUsed == true || counter == 0);
                        if (RefferalCode == "")
                        {
                            RefferalCode = GenerateRefferalCode("123456789", "ABCDEFGHIJKLMNOPQRSTUVWXYZ", dt);
                            isRefferalCodeUsed = db.MLMUsers.Any(p => p.Ref_Id == RefferalCode);
                            RefferalCode = (isRefferalCodeUsed == true) ? "" : RefferalCode;
                        }
                        if (RefferalCode == "")
                        {
                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                   + Environment.NewLine + " Problem in Generating Refferal Code. " + Environment.NewLine
                   + "[MLMWalletPoints][M:SaveMLMUser]",
                   BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                            result = "";
                        }
                        else
                        {
                            MLMUser objMLMUser = new MLMUser();
                            objMLMUser.UserID = objUserLogin.ID;
                            objMLMUser.Ref_Id = RefferalCode.ToUpper();
                            objMLMUser.Join_date_ref = DateTime.Now;
                            objMLMUser.Status_ref = false;
                            objMLMUser.Activate_date_ref = DateTime.Now;
                            objMLMUser.Refered_Id_ref = RefCode.ToUpper();
                            objMLMUser.ProfilePicture = pic;
                            db.MLMUsers.Add(objMLMUser);
                            db.SaveChanges();

                            MLMUserInvites objMLMUserInvites = db.MLMUserInvite.FirstOrDefault(p => p.Email == objUserLogin.Email || p.Mobile == objUserLogin.Mobile);
                            if (objMLMUserInvites != null)
                            {
                                objMLMUserInvites.IsAccepted = true;
                                db.SaveChanges();
                            }

                            result = "R_DONE";
                        }
                    }
                    else
                    {
                        result = "ALREADY_R";
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("MLMWalletPoints : SaveMLMUser() " + ex.Message);
            }
        }

        string GenerateRefferalCode(string Mobile, string email, DateTime dt)
        {
            email = email.Replace(".", "");
            RefferalCodeGenerator objRefferalCodeGenerator = new RefferalCodeGenerator();
            string Characters = "";
            string Numbers = "";

            Numbers = (Mobile + dt.ToString("ddMMyyyy")).Replace(" ", "");
            Characters = email;
            string CodeString = objRefferalCodeGenerator.CreateCode(5, Characters);
            string CodeNumeric = objRefferalCodeGenerator.CreateCode(4, Numbers);
            string RefferalCode = CodeString + CodeNumeric;
            return RefferalCode;
        }
        #endregion
    }
}