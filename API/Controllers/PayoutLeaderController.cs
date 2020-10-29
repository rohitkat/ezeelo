using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class PayoutLeaderController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public object Get(long userID)
        {
            object obj = new object();
            try
            {
                PersonalDetail objPersonel = db.PersonalDetails.Where(x => x.UserLoginID == userID).FirstOrDefault();
                KYCModel objKYC = db.KYCModels.Where(y => y.UserLoginID == userID).FirstOrDefault();
                LeadersPayoutRequestViewModel objPayout = new LeadersPayoutRequestViewModel();

                MLMWallet objWalletTransaction = db.MLMWallets.Where(x => x.UserLoginID == userID).FirstOrDefault();
                LeadersPayoutMaster objPayoutMaster = db.LeadersPayoutMasters.FirstOrDefault();


                if (objWalletTransaction != null)
                {
                    objPayout.EzeeloWalletCash = objWalletTransaction.Amount;
                    Decimal reservedAmount = Convert.ToDecimal(objWalletTransaction.Amount * objPayoutMaster.Min_Resereved * Convert.ToDecimal(0.01));
                    Decimal RedeamableCash = objWalletTransaction.Amount - reservedAmount;
                    decimal WalAmt = Math.Round(RedeamableCash, 2);
                    //Get CashbackWallet Amount 03-10-2019
                    CashbackWallet wallet = db.cashbackWallets.FirstOrDefault(p => p.UserLoginID == userID);
                    if (wallet != null)
                    {
                        WalAmt = WalAmt + wallet.Amount;
                    }
                    // objPayout.RedeamableCash = RedeamableCash;
                    objPayout.RedeamableCash = WalAmt;
                }
                else
                {
                    objPayout.EzeeloWalletCash = 0;
                    objPayout.RedeamableCash = 0;
                }

                if (objKYC != null)
                {

                    objPayout.AccountNo = objKYC.AccountNo;
                    objPayout.AccountType = objKYC.AccountType;
                    objPayout.AdhaarNo = objKYC.AdhaarNo;
                    objPayout.BankID = objKYC.BankID;
                    objPayout.BankIFSC = objKYC.BankIFSC;
                    objPayout.BranchName = objKYC.BranchName;
                    objPayout.UserLoginID = objKYC.UserLoginID;
                    objPayout.AccountHolderName = objPersonel.FirstName + " " + objPersonel.MiddleName + " " + objPersonel.LastName;

                    objPayout.BankName = db.Banks.Where(y => y.ID == objKYC.BankID).Select(y => y.Name).FirstOrDefault();
                }
                obj = new { Success = 1, Message = "Successfull", data = objPayout };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        [Route("api/PayoutLeader/PostPayoutRequest")]
        [HttpPost]
        public object PostPayoutRequest(string gstAmount, string requestedAmount, string tdsAmount, string totalAmount, long userID)
        {
            object obj = new object();
            try
            {
                LeadersPayoutRequest objPayoutRequest = new LeadersPayoutRequest();
                LeadersPayoutMaster objMaster = db.LeadersPayoutMasters.FirstOrDefault();
                MLMWallet objWalletTransaction = db.MLMWallets.Where(x => x.UserLoginID == userID).FirstOrDefault();

                objPayoutRequest.GSTAmount = Convert.ToDecimal(gstAmount);
                objPayoutRequest.GST = objMaster.GST;
                objPayoutRequest.TDSAmount = Convert.ToDecimal(tdsAmount);
                objPayoutRequest.TDS = objMaster.TDS;
                objPayoutRequest.ProcessingFees = objMaster.Processing_Fees;
                objPayoutRequest.MinReserved = objMaster.Min_Resereved;
                objPayoutRequest.UserLoginID = userID;
                objPayoutRequest.TotalAmount = Convert.ToDecimal(totalAmount);
                objPayoutRequest.RequestedAmount = Convert.ToDecimal(requestedAmount);
                if (objWalletTransaction != null)
                {
                    Decimal minReservedAmnt = Convert.ToDecimal(objWalletTransaction.Amount * objMaster.Min_Resereved * Convert.ToDecimal(0.01));
                    objPayoutRequest.MinReservedAmount = minReservedAmnt;
                    Decimal RedeamableCash = objWalletTransaction.Amount - minReservedAmnt;
                    decimal WalAmt = Math.Round(RedeamableCash, 2);
                    //Get CashbackWallet Amount 03-10-2019
                    CashbackWallet wallet = db.cashbackWallets.FirstOrDefault(p => p.UserLoginID == userID);
                    if (wallet != null)
                    {
                        WalAmt = WalAmt + wallet.Amount;
                    }
                    objPayoutRequest.RedeamableAmount = WalAmt;
                }
                else
                {
                    objPayoutRequest.RedeamableAmount = 0;
                    objPayoutRequest.MinReservedAmount = 0;
                }
                objPayoutRequest.RequestStatus = 0;
                objPayoutRequest.Create_Date = System.DateTime.Now;
                db.LeadersPayoutRequests.Add(objPayoutRequest);
                db.SaveChanges();
                obj = new { Success = 1, Message = "Successfully request send", data = string.Empty };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        [Route("api/PayoutLeader/GetPayoutSummary")]
        [HttpGet]
        public object GetPayoutSummary(string amount)
        {
            object obj = new object();
            try
            {
                LeadersPayoutMaster objPayout = db.LeadersPayoutMasters.FirstOrDefault();
                decimal Amount = Convert.ToDecimal(amount);

                var gstAmnt = Amount * Convert.ToDecimal(0.01) * objPayout.GST;
                var TDSAmount = Amount * Convert.ToDecimal(0.01) * objPayout.TDS;
                var ProcessingFees = Amount * objPayout.Processing_Fees * Convert.ToDecimal(0.01);
                var Penalty = objPayout.Penalty;

                var TotalAmount = (Amount - gstAmnt - TDSAmount - ProcessingFees - Penalty);


                LeadersPayoutRequestViewModel objPayoutRequest = new LeadersPayoutRequestViewModel();
                objPayoutRequest.RequestedAmount = Amount;
                objPayoutRequest.GSTAmount = gstAmnt;
                objPayoutRequest.TDSAmount = TDSAmount;
                objPayoutRequest.ProcessingFees = ProcessingFees;
                objPayoutRequest.PenaltyAmount = Penalty;
                objPayoutRequest.TotalAmount = TotalAmount;
                obj = new { Success = 1, Message = "Successfull.", data = objPayoutRequest };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        [Route("api/PayoutLeader/PayoutCheckPassword")]
        [HttpGet]
        public object PayoutCheckPassword(string redeamableAmount, string password, string requestAmount, long userID)
        {
            object obj = new object();
            try
            {
                if (string.IsNullOrEmpty(redeamableAmount) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(requestAmount) || userID <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid data", data = string.Empty };
                }
                string Password = string.Empty;
                LeadersPayoutRequest objPayoutRequest = db.LeadersPayoutRequests.Where(x => x.UserLoginID == userID).FirstOrDefault();

                if (requestAmount != "" && password != "")
                {
                    var redeamableAmnt = Convert.ToDecimal(redeamableAmount);
                    var requestAmnt = Convert.ToDecimal(requestAmount);
                    if (objPayoutRequest == null)
                    {
                        if (redeamableAmnt != 0)
                        {
                            if (requestAmnt <= redeamableAmnt)
                            {
                                Password = password;
                                var existingPassword = db.UserLogins.Where(x => x.ID == userID).Select(y => y.Password).FirstOrDefault();
                                if (existingPassword == password)
                                {
                                    obj = new { Success = 1, Message = "Succesfully Password is matched!!", data = string.Empty };
                                }
                                else
                                {
                                    obj = new { Success = 0, Message = "Password Is Incorrect!!", data = string.Empty };
                                }
                            }
                            else
                            {
                                obj = new { Success = 0, Message = "Requested Amount Should Be Small Than Redeamable Amount!!", data = string.Empty };
                            }
                        }
                        else
                        {
                            obj = new { Success = 0, Message = "Sorry You are not Eligible!!", data = string.Empty };
                        }
                    }
                    else
                    {
                        obj = new { Success = 0, Message = "Your Previous Request Is Under Process, Please Try after Previous Request is Completed!!", data = string.Empty };
                    }
                }
                else
                {
                    obj = new { Success = 0, Message = "Please Enter Details!!", data = string.Empty };
                }

            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
