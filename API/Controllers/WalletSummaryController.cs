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
    public class WalletSummaryController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public object Get(long UserLoginId)
        {
            object obj = new object();
            try
            {
                if (UserLoginId == null || UserLoginId <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid Data", data = string.Empty };
                }


                string sort = string.Empty;

                List<MLMWalletDetails> PayoutDetailsList = db.MLMWalletDetails.Where(x => x.UserLoginId == UserLoginId).ToList();

                List<PromotionalERPPayoutDetails> objPromotionalERPPayout = db.PromotionalERPPayoutDetails.Where(x => x.UserLoginId == UserLoginId).ToList();

                List<LeadersPayoutRequest> LeadersPayoutRequestList = db.LeadersPayoutRequests.Where(x => x.UserLoginID == UserLoginId).ToList();


                List<CustomerOrder> objMlmAmountUsed = db.CustomerOrders.Where(x => x.UserLoginID == UserLoginId && x.MLMAmountUsed > 0).ToList();

                List<EWalletRefund_Table> objEwallet = db.eWalletRefund_Table.Where(x => x.UserLoginId == UserLoginId && x.Isactive == true && x.Status == 1).ToList();//--addedd by Roshan 23-4-2019


                List<MLMWalletTransaction> objWalletTransaction = db.MLMWalletTransactions.Where(x => x.UserLoginID == UserLoginId && x.WalletAmountUsed > 0 && x.TransactionTypeID == 9).ToList();
                List<CashbackWalletLog> objCwallet = db.cashbackWallLog.Where(x => x.UserLoginID == UserLoginId && x.TransactionType == "Credit").ToList();

                //LeadersWalletPayBack walletPaybackList = new LeadersWalletPayBack();
                List<BoosterPlanPayoutDetails> BoosterPayoutdetails = db.boosterPayoutDetails.Where(p => p.UserLoginID == UserLoginId && p.Status == true).ToList();

                List<WalletSummaryViewModel> objWalletSummary = new List<WalletSummaryViewModel>();
                List<CustomerOrder> objCustomerOrder = db.CustomerOrders.Where(x => x.UserLoginID == UserLoginId).ToList();

                if (objPromotionalERPPayout.Count() > 0)
                {
                    foreach (var promotional in objPromotionalERPPayout)
                    {
                        PromotionalERPPayout objPromotionalERP = db.PromotionalERPPayouts.Where(x => x.Id == promotional.PromotionalERPPayoutId && x.IsPaid == true).FirstOrDefault();
                        if (objPromotionalERP != null)
                        {

                            objWalletSummary.Add(new WalletSummaryViewModel
                            {
                                OrderCode = "",
                                Amount = promotional.EzeeMoney,
                                Description = objPromotionalERP.ReferenceText,
                                WidrawlCreadit = "Credit",
                                Status = "Complete",
                                TransactionDate = objPromotionalERP.PaidDate

                            });
                        }
                    }
                }

                var OrderCode = "";
                if (PayoutDetailsList.Count() > 0)
                {
                    foreach (var payout in PayoutDetailsList)
                    {
                        EzeeMoneyPayout EzeeMoneyPayoutList = db.EzeeMoneyPayouts.Where(X => X.Id == payout.EzeeMoneyPayoutId).FirstOrDefault();
                        string startDate = EzeeMoneyPayoutList.FromDate.ToString("dd/MM/yyyy");
                        string toDate = EzeeMoneyPayoutList.ToDate.ToString("dd/MM/yyyy");

                        objWalletSummary.Add(new WalletSummaryViewModel
                        {
                            OrderCode = "",
                            Amount = payout.Amount,
                            Description = "Credited!! In Wallet For Payout of :" + startDate + "-" + toDate,
                            WidrawlCreadit = "Credit",
                            Status = "Complete",
                            TransactionDate = EzeeMoneyPayoutList.PaidDate

                        });

                        List<EzeeMoneyPayoutDetails> EzeeMoneyInactivePtPayoutList = db.EzeeMoneyPayoutDetail.Where(X => X.Ref_EzeeMoneyPayoutID == payout.EzeeMoneyPayoutId && X.IsInactivePaid == true && X.UserLoginId == UserLoginId).ToList();
                        if (EzeeMoneyInactivePtPayoutList != null && EzeeMoneyInactivePtPayoutList.Count() != 0)
                        {
                            objWalletSummary.Add(new WalletSummaryViewModel
                            {
                                OrderCode = "",
                                Amount = EzeeMoneyInactivePtPayoutList.Sum(p => p.InactiveEzeeMoney) ?? 0,
                                Description = "Credited!! In Wallet For Inactive Points into EzeeMoney for " + startDate + "-" + toDate,
                                WidrawlCreadit = "Credit",
                                Status = "Complete",
                                TransactionDate = EzeeMoneyPayoutList.InactivePaidDate

                            });
                        }
                    }
                }
                foreach (var bbp in BoosterPayoutdetails)
                {
                    BoosterPlanPayout boosterPlan = db.boosterPlanPayouts.Where(x => x.ID == bbp.BoosterPlanPayoutID).FirstOrDefault();
                    if (boosterPlan != null)
                    {
                        string startDate = boosterPlan.FromDate.ToString("dd/MM/yyyy");
                        string toDate = boosterPlan.ToDate.ToString("dd/MM/yyyy");
                        objWalletSummary.Add(new WalletSummaryViewModel
                        {
                            OrderCode = "",
                            Amount = bbp.TotalERP * boosterPlan.CoinRate,
                            Description = "Credited!! In Wallet For Booster Plan Payout of :" + startDate + "-" + toDate,
                            WidrawlCreadit = "Credit",
                            Status = "Complete",
                            TransactionDate = boosterPlan.CreateDate

                        });
                    }
                }
                //added BY Roshan
                if (objEwallet.Count() > 0)
                {
                    foreach (var item in objEwallet)
                    {
                        OrderCode = db.CustomerOrders.Where(x => x.ID == item.CustomerOrderId).Select(x => x.OrderCode).FirstOrDefault();
                        
                        objWalletSummary.Add(new WalletSummaryViewModel
                        {
                            OrderCode = OrderCode,
                            Amount = (decimal)item.RefundAmt,
                            Description = item.Comment + " "+OrderCode,
                            WidrawlCreadit = "Credit",
                            Status = "Complete",
                            TransactionDate = item.Date,

                        });
                        
                    }
                }

                // End By Roshan
                foreach (var item in objCwallet)
                {
                    if (item.CustomerOrderID > 0)
                    {
                        OrderCode = db.CustomerOrders.Where(x => x.ID == item.CustomerOrderID).Select(x => x.OrderCode).FirstOrDefault();
                        objWalletSummary.Add(new WalletSummaryViewModel
                        {
                            OrderCode = OrderCode,
                            Amount = (decimal)item.Amount,
                            Description = item.Remark + " " + OrderCode,
                            WidrawlCreadit = "Credit",
                            Status = "Complete",
                            TransactionDate = item.ModifyDate,
                        });
                    }
                    else
                    {
                        CashbackPointsPayout payout = db.cashbackPointsPayouts.Where(X => X.ID == item.CashbackPayoutID).FirstOrDefault();
                        if (payout != null)
                        {
                            EzeeMoneyPayout payout_ = db.EzeeMoneyPayouts.FirstOrDefault(p => p.Id == payout.EzeeMoneyPayoutID);
                            if (payout_ != null)
                            {
                                string startDate = payout_.FromDate.ToString("dd/MM/yyyy");
                                string toDate = payout_.ToDate.ToString("dd/MM/yyyy");

                                objWalletSummary.Add(new WalletSummaryViewModel
                                {
                                    OrderCode = "",
                                    Amount = item.Amount,
                                    Description = "Credited!! In CB Wallet For CB Points Payout of :" + startDate + "-" + toDate,
                                    WidrawlCreadit = "Credit",
                                    Status = "Complete",
                                    TransactionDate = item.ModifyDate
                                });
                            }
                        }
                    }
                }
                if (objMlmAmountUsed.Count() > 0)
                {
                    foreach (var item in objMlmAmountUsed)
                    {
                        OrderCode = item.OrderCode;
                        long customerOrderID = db.CustomerOrders.Where(x => x.OrderCode == OrderCode).Select(y => y.ID).FirstOrDefault();
                        CashbackWalletLog Cashbacklog = db.cashbackWallLog.FirstOrDefault(p => p.CustomerOrderID == item.ID && p.TransactionType == "Debit" && p.UserLoginID == item.UserLoginID);
                        EWalletRefund_Table Walllog = db.eWalletRefund_Table.FirstOrDefault(p => p.Status == 0 && p.Isactive == false && p.CustomerOrderId == item.ID);

                        decimal UserAmt = 0;
                        if (Walllog != null)
                        {
                            UserAmt = Walllog.RequsetAmt;
                        }
                        if (Cashbacklog != null)
                        {
                            UserAmt = UserAmt + Cashbacklog.Amount;
                        }
                        UserAmt = UserAmt != 0 ? UserAmt : (decimal)item.MLMAmountUsed;

                        //MLMWalletTransaction walleteTransaction = db.MLMWalletTransactions.Where(x => x.UserLoginID == item.UserLoginID && x.CustomerOrderID == customerOrderID).OrderByDescending(y => y.CreateDate).FirstOrDefault();

                        //if (walleteTransaction != null)
                        //{
                            objWalletSummary.Add(new WalletSummaryViewModel
                            {
                                OrderCode = OrderCode,
                                Amount = UserAmt,
                                Description = "Wallet Amount Used!! For Order Number " + OrderCode,
                                WidrawlCreadit = "Debit",
                                Status =  "Complete",
                                //TransactionTypeID = walleteTransaction.TransactionTypeID,
                                TransactionDate = item.CreateDate

                            });

                       // }
                    }
                }

                if (LeadersPayoutRequestList.Count() > 0)
                {
                    foreach (var item in LeadersPayoutRequestList)
                    {
                        objWalletSummary.Add(new WalletSummaryViewModel
                        {
                            Amount = (decimal)item.RequestedAmount,
                            Description = "Withdrawn!! For Payout Request",
                            WidrawlCreadit = "Withdrawn",
                            Status = item.RequestStatus.ToString(),
                            TransactionDate = item.Create_Date

                        });

                    }
                }

                foreach (var item in objWalletSummary)
                {
                    if (item.Status == "0")
                    {
                        item.Status = "Pending";
                    }
                    if (item.Status == "1")
                    {
                        item.Status = "Accepted";
                    }
                    if (item.Status == "2")
                    {
                        item.Status = "Complete";
                    }
                    if (item.Status == "3")
                    {
                        item.Status = "Cancelled";
                    }

                    if (item.TransactionTypeID == 7)
                    {
                        item.Status = "Complete";
                    }
                    else if (item.TransactionTypeID == 8)
                    {
                        item.Status = "Returned";
                    }
                    else if (item.TransactionTypeID == 9)
                    {
                        item.Status = "Cancelled";
                    }


                    else if (item.TransactionTypeID == 1)
                    {
                        item.Status = "Placed";
                    }
                    else if (item.TransactionTypeID == 2)
                    {
                        item.Status = "Confirm";
                    }
                    else if (item.TransactionTypeID == 3)
                    {
                        item.Status = "Packed";
                    }
                    else if (item.TransactionTypeID == 4)
                    {
                        item.Status = "Dispach from shop";
                    }
                    else if (item.TransactionTypeID == 5)
                    {
                        item.Status = "In Godown";
                    }
                    else if (item.TransactionTypeID == 6)
                    {
                        item.Status = "Dispach from Godown";
                    }
                    else if (item.TransactionTypeID == 10)
                    {
                        item.Status = "Abandoned";
                    }


                }

                objWalletSummary = objWalletSummary.OrderByDescending(x => x.TransactionDate).ToList();

                obj = new { Success = 1, Message = "Success", data = objWalletSummary };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
