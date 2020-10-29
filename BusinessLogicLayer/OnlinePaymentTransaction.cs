using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models;
using System.Data.Entity;

/*
 Handed over to Mohit, Tejaswee, Pradnyakar Sir
 */
namespace BusinessLogicLayer
{
    public class OnlinePaymentTransaction
    {
        /// <summary>
        /// save gateway payment transaction details.
        /// </summary>
        /// <param name="paymentDetails">Gateway payment details</param>
        /// <returns>operation status</returns>
        public int SavePaymentDetails(OnlinePaymentTransactionViewModel paymentDetails)
        {

            int oprStatus = 0;

            GetwayPaymentTransaction objPayment = new GetwayPaymentTransaction();
            //-----------Update GetwayPaymentTransactions status from 0 to 1 from App(mobile), when Online is successful. By Ashish----------//
            EzeeloDBContext db = new EzeeloDBContext();
            int count = db.GetwayPaymentTransactions.Where(x => x.CustomerOrderID == paymentDetails.CustomerOrderID).Count();
            if (count > 0)
            {
                objPayment = db.GetwayPaymentTransactions.Where(x => x.CustomerOrderID == paymentDetails.CustomerOrderID).FirstOrDefault();
                objPayment.Status = paymentDetails.PaymentStatus;
                /// objPayment.AccountTransactionId=paymentDetails.a
                db.SaveChanges();
                oprStatus = 1;//Sonali_31-01-2019
            }
            //-----------End Update GetwayPaymentTransactions status from 0 to 1 from App(mobile), when Online is successful.----------//
            else
            {
                objPayment.CreateBy = CommonFunctions.GetPersonalDetailsID(paymentDetails.CustomerLoginID);
                objPayment.CreateDate = DateTime.UtcNow.AddHours(5.5);
                objPayment.FromUID = paymentDetails.CustomerLoginID;
                /*
                 * Cutomer Order ID
                 * 20-01-2016
                 * pradnyakar badge
                 */
                objPayment.CustomerOrderID = paymentDetails.CustomerOrderID;
                /***********************************************************/
                objPayment.ToUID = 1;
                objPayment.IsActive = true;
                objPayment.PaymentGetWayTransactionId = paymentDetails.PaymentGateWayTransactionId;
                objPayment.Status = paymentDetails.PaymentStatus;
                objPayment.PaymentMode = paymentDetails.PaymentMode;
                objPayment.TransactionDate = DateTime.UtcNow.AddHours(5.5);

                objPayment.Description = paymentDetails.Description;
                objPayment.DeviceType = paymentDetails.DeviceType;

                objPayment.Status = paymentDetails.PaymentStatus;
                try
                {
                    ////EzeeloDBContext db = new EzeeloDBContext();
                    db.GetwayPaymentTransactions.Add(objPayment);
                    db.SaveChanges();
                    oprStatus = 1;

                }
                catch (Exception exception)
                {
                    oprStatus = 0;
                    BusinessLogicLayer.ErrorLog.ErrorLogFile("problem in saving getway payment transaction details :" + exception.Message + exception.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

                }
            }
            return oprStatus;

        }

        //Added by Sonali_04-01-2019
        public int SavePayubizPaymentDetails(PayubizTransaction payubizTransaction)
        {
            int Id = 0;
            try
            {
                EzeeloDBContext db = new EzeeloDBContext();
                //payubizTransaction.CreatedDate = DateTime.UtcNow.AddHours(5.30);
                //payubizTransaction.CreatedBy = payubizTransaction.UserLoginId;
                //db.PayubizTransactions.Add(payubizTransaction);
                //db.SaveChanges();
                //oprStatus = 1;

                //// count = db.GetwayPaymentTransactions.Where(x => x.PaymentGetWayTransactionId == paymentDetails.PaymentGateWayTransactionId).Count();
                int count = db.PayubizTransactions.Where(x => x.OrderId == payubizTransaction.OrderId).Count();
                if (count > 0)
                {
                  PayubizTransaction  OldpayubizTransaction = db.PayubizTransactions.Where(x => x.OrderId == payubizTransaction.OrderId).FirstOrDefault();
                    //objPayment = db.GetwayPaymentTransactions.Where(x => x.PaymentGetWayTransactionId == paymentDetails.PaymentGateWayTransactionId).FirstOrDefault();
                    OldpayubizTransaction.Status = payubizTransaction.Status;
                    OldpayubizTransaction.ModifiedBy = payubizTransaction.UserLoginId;
                    OldpayubizTransaction.ModifiedDate = DateTime.UtcNow.AddHours(5.30);
                    OldpayubizTransaction.AddonDate = payubizTransaction.AddonDate;
                    OldpayubizTransaction.Amount = payubizTransaction.Amount;
                    OldpayubizTransaction.FirstName = payubizTransaction.FirstName;
                    OldpayubizTransaction.Mode = payubizTransaction.Mode;
                    OldpayubizTransaction.Payid = payubizTransaction.Payid;
                    OldpayubizTransaction.TxtnId = payubizTransaction.TxtnId;
                    OldpayubizTransaction.UnMappedStatus = payubizTransaction.UnMappedStatus;
                    db.Entry(OldpayubizTransaction).State = EntityState.Modified;
                    // objPayment.AccountTransactionId=paymentDetails.a
                    db.SaveChanges();
                    Id = (Int32)payubizTransaction.ID;
                }
                //-----------End Update GetwayPaymentTransactions status from 0 to 1 from App(mobile), when Online is successful.----------//
                else
                {

                    db.PayubizTransactions.Add(payubizTransaction);
                    db.SaveChanges();
                    Id = (Int32)payubizTransaction.ID;
                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("problem in saving payubiz payment transaction details :" + ex.Message + ex.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                Id = 0;
            }
            return Id;
        }
    }
}
