using API.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    /*
     * Developed By :- Pradnyakar N. Badge
     * Dated :- 05/01/2016
     * Purpose to cancel the Place order by the authenticate user
     * 
     */
    public class CancelOrderController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        // GET api/cancalorder
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/cancalorder/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/cancelorder
        [TokenVerification]
        [ApiException]
        [ValidateModel]

        public object Post(long orderID, long shopStockID, long lCustLoginID, int ReasonId, string ReasonInComment)
        {
            object obj = new object();
            try
            {
                if (lCustLoginID == null || lCustLoginID == 0 || orderID == null || orderID == 0 || ReasonId == null || ReasonId <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid EmailId or MobileNo.", data = string.Empty };
                }
                ModelLayer.Models.CustomerOrder co = db.CustomerOrders.FirstOrDefault(p => p.ID == orderID);
                OrderCancelSmsAndEmail orderCancel = new OrderCancelSmsAndEmail(System.Web.HttpContext.Current.Server);//Added for send sms and email by Sonali_04-01-2019
                if (shopStockID > 0)
                {
                    var lOrder = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == orderID && x.ShopStockID == shopStockID).FirstOrDefault();
                    if (lOrder != null && lOrder.ID > 0)
                    {
                        if (lOrder.OrderStatus < 3)
                        {
                            string CancelOrderReason = db.CancelOrderReason.Where(x => x.Id == ReasonId).Select(x => x.Reason).FirstOrDefault() + ". " + ReasonInComment;
                            BusinessLogicLayer.CustomerOrder lCustOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                            int oprStatus = lCustOrder.CancelCustomerOrder(orderID, shopStockID, lCustLoginID, CancelOrderReason);
                            //-----------------added by Ashwini Meshram 09-Jan-2017 For Push Notification--------------------//
                            int lStatus = 0;
                            string OrderStatus = "Cancelled";
                            if (orderID != 0 && oprStatus == 103)
                            {
                                if (co.MLMAmountUsed > 0)
                                {
                                    //lCustOrder.Insert_RefundRequest_EwalletRefund(orderID);//Added by Rumana on 19/04/2019  

                                    //lCustOrder.Send_EWalletRefund_Mail(orderID, false);
                                    (new MLMWalletPoints()).RefundUsedWalletAmount(co.MLMAmountUsed.Value, orderID, 1, co.UserLoginID,9);
                                }
                                (new SendFCMNotification()).SendNotification("cancelled", orderID); //Yashaswi 2-7-2019
                                lCustOrder.SendAlertforPushNotification(orderID, OrderStatus);
                                // lStatus = lCustOrder.SendPushNotification(orderID);
                                orderCancel.SendSMSToCustomer(lCustLoginID, orderID);//Added for send sms and email by Sonali_04-01-2019
                                orderCancel.SendSMSToMerchant(lCustLoginID, orderID);//Added for send sms and email by Sonali_04-01-2019
                                orderCancel.SendMailToCustomer(lCustLoginID, orderID);//Added for send sms and email by Sonali_04-01-2019
                                orderCancel.SendMailToMerchant(lCustLoginID, orderID);//Added for send sms and email by Sonali_04-01-2019
                                
                            }
                            //-----------------------------------------------------------------------------------------//

                            if (oprStatus == 103)
                                obj = new { Success = 1, Message = "Order has been cancelled successfully.", data = new { Status = lStatus } };
                            //obj = new { HTTPStatusCode = "200", UserMessage = "Order has been cancelled successfully.", Status = lStatus };
                            if (oprStatus == 500)
                                obj = new { Success = 0, Message = "Internal server error.", data = string.Empty };
                            // obj = new { HTTPStatusCode = "500", UserMessage = "Internal server error." };
                            if (oprStatus == 106)
                                obj = new { Success = 0, Message = "Invalid request. The order you want to cancel is not present for given customer login ID.", data = string.Empty };
                            //  obj = new { HTTPStatusCode = "400", UserMessage = "Invalid request. The order you want to cancel is not present for given customer login ID." };
                        }
                        else
                            obj = new { Success = 0, Message = "Your order is already “Packed” so you can not cancel your order. Please get in touch with our customer service team on +91 9172221910 for any further assistance.", data = string.Empty };
                    }
                    else
                        obj = new { Success = 0, Message = "Invalid request. The order you want to cancel is not present for given customer login ID.", data = string.Empty };
                }
                else
                {
                    var lOrder = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == orderID).ToList();
                    if (lOrder != null)
                    {
                        var orderstatus = lOrder.Where(x => x.OrderStatus > 3).Any();
                        if (orderstatus)
                        {
                            return obj = new { Success = 0, Message = "Your order is already “Packed” so you can not cancel your order. Please get in touch with our customer service team on +91 9172221910 for any further assistance.", data = string.Empty };
                        }
                        string CancelOrderReason = db.CancelOrderReason.Where(x => x.Id == ReasonId).Select(x => x.Reason).FirstOrDefault() + ". " + ReasonInComment;
                        BusinessLogicLayer.CustomerOrder lCustOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                        int oprStatus = lCustOrder.CancelCustomerOrder(orderID, shopStockID, lCustLoginID, CancelOrderReason);
                        //-----------------added by Ashwini Meshram 09-Jan-2017 For Push Notification--------------------//
                        int lStatus = 0;
                        string OrderStatus = "Cancelled";
                        if (orderID != 0 && oprStatus == 103)
                        {
                            if (co.MLMAmountUsed > 0)
                            {
                                //lCustOrder.Insert_RefundRequest_EwalletRefund(orderID);//Added by Rumana on 19/04/2019  

                                //lCustOrder.Send_EWalletRefund_Mail(orderID, false);
                                (new MLMWalletPoints()).RefundUsedWalletAmount(co.MLMAmountUsed.Value, orderID, 1, co.UserLoginID,9);
                            }
                             (new SendFCMNotification()).SendNotification("cancelled", orderID); //Yashaswi 2-7-2019
                            lCustOrder.SendAlertforPushNotification(orderID, OrderStatus);
                            // lStatus = lCustOrder.SendPushNotification(orderID);
                            orderCancel.SendSMSToCustomer(lCustLoginID, orderID);//Added for send sms and email by Sonali_04-01-2019
                            orderCancel.SendSMSToMerchant(lCustLoginID, orderID);//Added for send sms and email by Sonali_04-01-2019
                            orderCancel.SendMailToCustomer(lCustLoginID, orderID);//Added for send sms and email by Sonali_04-01-2019
                            orderCancel.SendMailToMerchant(lCustLoginID, orderID);//Added for send sms and email by Sonali_04-01-2019
                        }
                        //-----------------------------------------------------------------------------------------//

                        if (oprStatus == 103)
                            obj = new { Success = 1, Message = "Order has been cancelled successfully.", data = new { Status = lStatus } };
                        //obj = new { HTTPStatusCode = "200", UserMessage = "Order has been cancelled successfully.", Status = lStatus };
                        if (oprStatus == 500)
                            obj = new { Success = 0, Message = "Internal server error.", data = string.Empty };
                        // obj = new { HTTPStatusCode = "500", UserMessage = "Internal server error." };
                        if (oprStatus == 106)
                            obj = new { Success = 0, Message = "Invalid request. The order you want to cancel is not present for given customer login ID.", data = string.Empty };
                    }
                    else
                        obj = new { Success = 0, Message = "Invalid request. The order you want to cancel is not present for given customer login ID.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        // PUT api/cancalorder/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/cancalorder/5
        public void Delete(int id)
        {
        }
    }
}
