using API.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace API.Controllers
{
    public class OnlinePaymentTransactionController : ApiController
    {
        // POST api/onlinepaymenttransaction
        /// <summary>
        /// Save online payment transaction details 
        /// </summary>
        /// <param name="paymentDetails">Gateway payment details; Send Status = 1 when payment successfull else, 0 in case of failure.</param>
        /// <returns>Status of operation</returns>
        [TokenVerification]
        [ValidateModel]
        public object Post(OnlinePaymentTransactionViewModel paymentDetails)
        {
            object obj = new object();
            try
            {
                if (paymentDetails == null)
                {
                    return obj = new { Success = 0, Message = "Unsupported Query Parameters.", data = string.Empty };
                    // return obj = new { HTTPStatusCode = "400", UserMessage = "Unsupported Query Parameters", ValidationError = "Please provide payment details." };
                }
                else
                {
                    string reqBy = Request.Headers.GetValues("ReqBy").First();
                    //System.Web.Http.Controllers.HttpActionContext actionContext=new System.Web.Http.Controllers.HttpActionContext();
                    //string reqBy = actionContext.Request.Headers.GetValues("ReqBy").First();
                    paymentDetails.DeviceType = reqBy;
                    OnlinePaymentTransaction payTrans = new OnlinePaymentTransaction();
                    int oprStatus = payTrans.SavePaymentDetails(paymentDetails);

                    if (oprStatus == 1)
                    {
                        OrderPlacedSmsAndEmail orderPlaced = new OrderPlacedSmsAndEmail(System.Web.HttpContext.Current.Server);//Added for sms and email send by Sonali_04-01-2019
                        orderPlaced.SendSMSToCustomer(paymentDetails.CustomerLoginID, paymentDetails.CustomerOrderID);//Added for sms and email send by Sonali_04-01-2019
                        orderPlaced.SendSMSToMerchant(paymentDetails.CustomerLoginID, paymentDetails.CustomerOrderID);//Added for sms and email send by Sonali_04-01-2019
                        orderPlaced.SendMailToCustomer(paymentDetails.CustomerLoginID, paymentDetails.CustomerOrderID);//Added for sms and email send by Sonali_04-01-2019
                        orderPlaced.SendMailToMerchant(paymentDetails.CustomerLoginID, paymentDetails.CustomerOrderID);//Added for sms and email send by Sonali_04-01-2019
                        obj = new { Success = 1, Message = "Payment details saved successfully.", data = oprStatus };
                    }

                    // obj = new { HTTPStatusCode = "200", UserMessage = "Payment details saved successfully." };
                    else if (oprStatus == 0)
                        obj = new { Success = 0, Message = "Internal Server Error.", data = oprStatus };
                    //   obj = new { HTTPStatusCode = "500", UserMessage = "Internal Server Error", ValidationError = "problem in saving payment transaction details." };
                }
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }



        //[TokenVerification]
        //[ValidateModel]
        //[Route("api/OnlinePaymentTransaction/SavePayubizData")]
        //public object SavePayubizData(long UserLoginId, long OrderId, PayubizTransactionViewModel payubiz)
        //{
        //    object obj = new object();
        //    try
        //    {
        //        if (payubiz == null || UserLoginId <= 0 || OrderId <= 0 || !ModelState.IsValid)
        //        {
        //            return obj = new { Success = 0, Message = "Enter valid data", data = string.Empty };
        //        }
        //        PayubizTransaction objPayubiz = new PayubizTransaction();
        //        objPayubiz.Payid = payubiz.mihpayid;
        //        objPayubiz.Mode = payubiz.Mode;
        //        objPayubiz.Status = payubiz.Status;
        //        objPayubiz.Key = payubiz.Key;
        //        objPayubiz.TxtnId = payubiz.TxtnId;
        //        objPayubiz.Amount = payubiz.Amount;
        //        objPayubiz.CardCategory = payubiz.CardCategory;
        //        objPayubiz.Discount = payubiz.Discount;
        //        objPayubiz.Net_Amount_Debit = payubiz.Net_Amount_Debit;
        //        objPayubiz.AddonDate = payubiz.addedon.Date;
        //        objPayubiz.FirstName = payubiz.FirstName;
        //        objPayubiz.Country = payubiz.Country;
        //        objPayubiz.Email = payubiz.Email;
        //        objPayubiz.Hash = payubiz.Hash;
        //        objPayubiz.Payment_source = payubiz.Payment_source;
        //        objPayubiz.PG_TYPE = payubiz.PG_TYPE;
        //        objPayubiz.Bank_ref_num = payubiz.Bank_ref_num;
        //        objPayubiz.Bankcode = payubiz.Bankcode;
        //        objPayubiz.Name_on_card = payubiz.Name_on_card;
        //        objPayubiz.Error_Message = payubiz.Error_Message;
        //        objPayubiz.CardNum = payubiz.CardNum;
        //        objPayubiz.Device_type = payubiz.Device_type;
        //        objPayubiz.UserLoginId = UserLoginId;
        //        objPayubiz.OrderId = OrderId;
        //        OnlinePaymentTransaction payTrans = new OnlinePaymentTransaction();
        //        int status = payTrans.SavePayubizPaymentDetails(objPayubiz);

        //        string reqBy = Request.Headers.GetValues("ReqBy").First();
        //        //System.Web.Http.Controllers.HttpActionContext actionContext=new System.Web.Http.Controllers.HttpActionContext();
        //        //string reqBy = actionContext.Request.Headers.GetValues("ReqBy").First();
        //        OnlinePaymentTransactionViewModel paymentDetails = new OnlinePaymentTransactionViewModel();
        //        paymentDetails.DeviceType = reqBy;
        //        paymentDetails.CustomerLoginID = UserLoginId;
        //        paymentDetails.PaymentMode = objPayubiz.Mode;
        //        paymentDetails.PaymentGateWayTransactionId = objPayubiz.TxtnId;
        //        if (objPayubiz.Status == "success")
        //            paymentDetails.PaymentStatus = 1;
        //        else
        //            paymentDetails.PaymentStatus = 0;
        //        paymentDetails.CustomerOrderID = OrderId;
        //        paymentDetails.Description = "Payubiz";

        //        int oprStatus = payTrans.SavePaymentDetails(paymentDetails);

        //        if (oprStatus == 1)
        //        {
        //            int lStatus = 0;
        //            string OrderStatus = "Placed";
        //            BusinessLogicLayer.CustomerOrder lCustOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
        //            oprStatus = lCustOrder.UpdatePendingCustomerOrder(OrderId, UserLoginId, "Update Order status from Pending to Placed By APP");
        //            if (oprStatus == 103)
        //            {
        //                lCustOrder.SendAlertforPushNotification(OrderId, OrderStatus);
        //                ////lStatus = lCustOrder.SendPushNotification(orderID);
        //                lStatus = lCustOrder.AndroidPushFCM(OrderId); //FCM Notification method called
        //                obj = new { Success = 1, Message = "Payment details saved successfully.", data = new { Status = lStatus } };
        //            }
        //            if (oprStatus == 500)
        //                obj = new { Success = 0, Message = "Internal server error.", string.Empty };
        //            // obj = new { HTTPStatusCode = "500", UserMessage = "Internal server error." };
        //            if (oprStatus == 106)
        //                obj = new { Success = 0, Message = "Invalid request. The order you want to Update is not present for given customer login ID.", string.Empty };
        //            // obj = new { HTTPStatusCode = "400", UserMessage = "Invalid request. The order you want to Update is not present for given customer login ID." };

        //        }
        //        // obj = new { HTTPStatusCode = "200", UserMessage = "Payment details saved successfully." };
        //        else if (oprStatus == 0)
        //        {
        //            obj = new { Success = 0, Message = "Internal Server Error.", data = oprStatus };
        //            //   obj = new { HTTPStatusCode = "500", UserMessage = "Internal Server Error", ValidationError = "problem in saving payment transaction details." };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EzeeloDBContext db = new EzeeloDBContext();
        //        PayubizTransaction objPayubiz = new PayubizTransaction();
        //        objPayubiz.UserLoginId = UserLoginId;
        //        objPayubiz.OrderId = OrderId;
        //        objPayubiz.responceString = string.Empty;
        //        db.PayubizTransactions.Add(objPayubiz);
        //        db.SaveChanges();
        //        obj = new { Success = 1, Message = "Payment details saved successfully.", data = new { Status = string.Empty } };
        //        //obj = new { Success = 0, Message = ex.Message, data = string.Empty };
        //    }
        //    return obj;
        //}

        //[TokenVerification]
        //[ValidateModel]
        //[Route("api/OnlinePaymentTransaction/SavePayubizData_Online")]
        //public object SavePayubizData_Online(long UserLoginId, long OrderId, [FromBody] PayubizViewModel model)
        //{
        //    object obj = new object();
        //    try
        //    {
        //        if (UserLoginId == null || UserLoginId <= 0 || OrderId == 0 || OrderId <= 0 || model == null)
        //        {
        //            return obj = new { Success = 0, Message = "Invalid parameter.", data = string.Empty };
        //        }
        //        PayubizTransaction objPayubiz = new PayubizTransaction();
        //        objPayubiz.Amount = model.Amount;
        //        objPayubiz.Email = model.Email;
        //        objPayubiz.FirstName = model.FirstName;
        //        objPayubiz.Mode = model.Mode;
        //        objPayubiz.OrderId = OrderId;
        //        objPayubiz.Payid = model.Payid;
        //        objPayubiz.TxtnId = model.TxtnId;
        //        objPayubiz.UserLoginId = UserLoginId;
        //        objPayubiz.AddonDate = model.AddonDate;
        //        objPayubiz.Status = model.Status;
        //        objPayubiz.UnMappedStatus = model.UnmappedStatus;
        //        objPayubiz.responceString = model.Payubiz;
        //        BusinessLogicLayer.PayubizLog.PayubizLogFile("Save Payubiz data:" + model.Payubiz, "API", System.Web.HttpContext.Current.Server);

        //        //objPayubiz.responceString = model.Payubiz;


        //        //var json_serializer = new JavaScriptSerializer();
        //        //IDictionary<string, object> routes_list = (IDictionary<string, object>)json_serializer.DeserializeObject(payubiz);
        //        //objPayubiz.Amount = (routes_list.ContainsKey("amount")) ? Convert.ToDecimal(routes_list["amount"]) : 0;
        //        //objPayubiz.Email = (routes_list.ContainsKey("email")) ? routes_list["email"].ToString() : "";
        //        //objPayubiz.FirstName = (routes_list.ContainsKey("firstname")) ? routes_list["firstname"].ToString() : "";
        //        //objPayubiz.Mode = (routes_list.ContainsKey("mode")) ? routes_list["mode"].ToString() : "";
        //        //objPayubiz.OrderId = OrderId;
        //        //objPayubiz.Payid = (routes_list.ContainsKey("id")) ? routes_list["id"].ToString() : "";
        //        //objPayubiz.TxtnId = (routes_list.ContainsKey("txnid")) ? routes_list["txnid"].ToString() : "";
        //        //objPayubiz.UserLoginId = UserLoginId;
        //        //objPayubiz.AddonDate = (routes_list.ContainsKey("addondate")) ? Convert.ToDateTime(routes_list["addondate"].ToString()) : DateTime.Now;
        //        //objPayubiz.Status = (routes_list.ContainsKey("status")) ? routes_list["status"].ToString() : "";
        //        //objPayubiz.responceString = payubiz;

        //        OnlinePaymentTransaction payTrans = new OnlinePaymentTransaction();
        //        int status = payTrans.SavePayubizPaymentDetails(objPayubiz);

        //        string reqBy = Request.Headers.GetValues("ReqBy").First();
        //        //System.Web.Http.Controllers.HttpActionContext actionContext=new System.Web.Http.Controllers.HttpActionContext();
        //        //string reqBy = actionContext.Request.Headers.GetValues("ReqBy").First();
        //        OnlinePaymentTransactionViewModel paymentDetails = new OnlinePaymentTransactionViewModel();
        //        paymentDetails.DeviceType = reqBy;
        //        paymentDetails.CustomerLoginID = UserLoginId;
        //        paymentDetails.PaymentMode = objPayubiz.Mode;
        //        paymentDetails.PaymentGateWayTransactionId = objPayubiz.TxtnId;
        //        if (objPayubiz.Status == "success")
        //            paymentDetails.PaymentStatus = 1;
        //        else
        //            paymentDetails.PaymentStatus = 0;
        //        paymentDetails.CustomerOrderID = OrderId;
        //        paymentDetails.Description = "Payubiz";

        //        int oprStatus = payTrans.SavePaymentDetails(paymentDetails);

        //        if (oprStatus == 1)
        //        {
        //            int lStatus = 0;
        //            string OrderStatus = "Placed";
        //            BusinessLogicLayer.CustomerOrder lCustOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
        //            oprStatus = lCustOrder.UpdatePendingCustomerOrder(OrderId, UserLoginId, "Update Order status from Pending to Placed By APP");
        //            if (oprStatus == 103)
        //            {
        //                lCustOrder.SendAlertforPushNotification(OrderId, OrderStatus);
        //                ////lStatus = lCustOrder.SendPushNotification(orderID);
        //                //lStatus = lCustOrder.AndroidPushFCM(OrderId); //FCM Notification method called
        //                obj = new { Success = 1, Message = "Payment details saved successfully.", data = new { Status = lStatus } };
        //            }
        //            if (oprStatus == 500)
        //                obj = new { Success = 0, Message = "Internal server error.", string.Empty };
        //            // obj = new { HTTPStatusCode = "500", UserMessage = "Internal server error." };
        //            if (oprStatus == 106)
        //                obj = new { Success = 0, Message = "Invalid request. The order you want to Update is not present for given customer login ID.", string.Empty };
        //            // obj = new { HTTPStatusCode = "400", UserMessage = "Invalid request. The order you want to Update is not present for given customer login ID." };

        //        }
        //        // obj = new { HTTPStatusCode = "200", UserMessage = "Payment details saved successfully." };
        //        else if (oprStatus == 0)
        //        {
        //            obj = new { Success = 0, Message = "Internal Server Error.", data = oprStatus };
        //            //   obj = new { HTTPStatusCode = "500", UserMessage = "Internal Server Error", ValidationError = "problem in saving payment transaction details." };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EzeeloDBContext db = new EzeeloDBContext();
        //        PayubizTransaction objPayubiz = new PayubizTransaction();
        //        objPayubiz.UserLoginId = UserLoginId;
        //        objPayubiz.OrderId = OrderId;
        //        BusinessLogicLayer.PayubizLog.PayubizLogFile("Save Payubiz data:" + OrderId, "API", System.Web.HttpContext.Current.Server);
        //        //objPayubiz.responceString = model.Payubiz;
        //        db.PayubizTransactions.Add(objPayubiz);
        //        db.SaveChanges();
        //        obj = new { Success = 1, Message = "Payment details saved successfully.", data = new { Status = string.Empty } };
        //        //obj = new { Success = 0, Message = ex.Message, data = string.Empty };
        //    }
        //    return obj;
        //}

        [TokenVerification]
        [ValidateModel]
        [Route("api/OnlinePaymentTransaction/SavePayubizData_New")]
        [HttpPost]
        public object SavePayubizData_New([FromBody] PayubizViewModel model)
        {
            object obj = new object();
            try
            {
                BusinessLogicLayer.PayubizLog.PayubizLogFile(model.ResponseString, "API", System.Web.HttpContext.Current.Server);
                if (model == null || model.UserLoginId == null || model.UserLoginId <= 0 || model.OrderId == 0 || model.OrderId <= 0 || string.IsNullOrEmpty(model.ResponseString))
                {
                    return obj = new { Success = 0, Message = "Invalid parameter.", data = string.Empty };
                }
                PayubizTransaction objPayubiz = new PayubizTransaction();
                objPayubiz.OrderId = model.OrderId;
                objPayubiz.UserLoginId = model.UserLoginId;
                objPayubiz.responceString = model.ResponseString;
                OnlinePaymentTransaction payTrans = new OnlinePaymentTransaction();
                int PayubizTransactionId = payTrans.SavePayubizPaymentDetails(objPayubiz);

                objPayubiz = ConversionResponseStringToObj(model.ResponseString);
                objPayubiz.ID = PayubizTransactionId;
                objPayubiz.OrderId = model.OrderId;
                objPayubiz.UserLoginId = model.UserLoginId;
                objPayubiz.responceString = model.ResponseString;
                PayubizTransactionId = payTrans.SavePayubizPaymentDetails(objPayubiz);
                string reqBy = Request.Headers.GetValues("ReqBy").First();
                //System.Web.Http.Controllers.HttpActionContext actionContext=new System.Web.Http.Controllers.HttpActionContext();
                //string reqBy = actionContext.Request.Headers.GetValues("ReqBy").First();
                OnlinePaymentTransactionViewModel paymentDetails = new OnlinePaymentTransactionViewModel();
                paymentDetails.DeviceType = reqBy;
                paymentDetails.CustomerLoginID = model.UserLoginId;
                paymentDetails.PaymentMode = objPayubiz.Mode;
                paymentDetails.PaymentGateWayTransactionId = objPayubiz.TxtnId;
                if (objPayubiz.Status == "success")
                    paymentDetails.PaymentStatus = 1;
                else
                    paymentDetails.PaymentStatus = 0;
                paymentDetails.CustomerOrderID = model.OrderId;
                paymentDetails.Description = "Payubiz";

                int oprStatus = payTrans.SavePaymentDetails(paymentDetails);

                if (oprStatus == 1 && objPayubiz.Status == "success")
                {
                    int lStatus = 0;
                    string OrderStatus = "Placed";
                    BusinessLogicLayer.CustomerOrder lCustOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                    oprStatus = lCustOrder.UpdatePendingCustomerOrder(model.OrderId, model.UserLoginId, "Update Order status from Pending to Placed By APP");
                    if (oprStatus == 103)
                    {
                        lCustOrder.SendAlertforPushNotification(model.OrderId, OrderStatus);
                        OrderPlacedSmsAndEmail orderPlaced = new OrderPlacedSmsAndEmail(System.Web.HttpContext.Current.Server);//Added for sms and email send by Sonali_04-01-2019
                        orderPlaced.SendSMSToCustomer(model.UserLoginId, model.OrderId);//Added for sms and email send by Sonali_04-01-2019
                        orderPlaced.SendSMSToMerchant(model.UserLoginId, model.OrderId);//Added for sms and email send by Sonali_04-01-2019
                        orderPlaced.SendMailToCustomer(model.UserLoginId, model.OrderId);//Added for sms and email send by Sonali_04-01-2019
                        orderPlaced.SendMailToMerchant(model.UserLoginId, model.OrderId);//Added for sms and email send by Sonali_04-01-2019
                        ////lStatus = lCustOrder.SendPushNotification(orderID);
                        //lStatus = lCustOrder.AndroidPushFCM(OrderId); //FCM Notification method called
                        obj = new { Success = 1, Message = "Payment details saved successfully.", data = new { Status = lStatus } };
                    }
                    if (oprStatus == 500)
                        obj = new { Success = 0, Message = "Internal server error.", string.Empty };
                    // obj = new { HTTPStatusCode = "500", UserMessage = "Internal server error." };
                    if (oprStatus == 106)
                        obj = new { Success = 0, Message = "Invalid request. The order you want to Update is not present for given customer login ID.", string.Empty };
                    // obj = new { HTTPStatusCode = "400", UserMessage = "Invalid request. The order you want to Update is not present for given customer login ID." };

                }

                // obj = new { HTTPStatusCode = "200", UserMessage = "Payment details saved successfully." };
                else if (oprStatus == 0)
                {
                    obj = new { Success = 1, Message = "Payment Failed ! please contact to support", data = new { Status = 0 } };
                    //obj = new { Success = 0, Message = "Internal Server Error.", data = oprStatus };
                    //   obj = new { HTTPStatusCode = "500", UserMessage = "Internal Server Error", ValidationError = "problem in saving payment transaction details." };
                }
                else
                {
                    obj = new { Success = 1, Message = "Payment Failed ! please contact to support", data = new { Status = 0 } };
                }

                //string[] result = model.ResponseString.Split(',');
                //if (result != null && result.Count() > 0)
                //{
                //    foreach (var item in result)
                //    {
                //        if (item.Contains("mihpayid"))
                //        {
                //            objPayubiz.Payid = item.Substring(item.IndexOf("=") + 1);
                //        }
                //        else if (item.Contains("mode"))
                //        {
                //            objPayubiz.Mode = item.Substring(item.IndexOf("=") + 1);
                //        }
                //        else if (item.Contains("unmappedstatus"))
                //        {
                //            objPayubiz.UnMappedStatus = item.Substring(item.IndexOf("=") + 1);
                //        }
                //        else if (item.Contains("status"))
                //        {
                //            objPayubiz.Status = item.Substring(item.IndexOf("=") + 1);
                //        }
                //        else if (item.Contains("txnid"))
                //        {
                //            objPayubiz.TxtnId = item.Substring(item.IndexOf("=") + 1);
                //        }
                //        else if (item.Contains("addedon"))
                //        {
                //            objPayubiz.AddonDate = Convert.ToDateTime(item.Substring(item.IndexOf("=") + 1));
                //        }
                //        else if (item.Contains("firstname"))
                //        {
                //            objPayubiz.FirstName = item.Substring(item.IndexOf("=") + 1);
                //        }
                //        else if (item.Contains("email"))
                //        {
                //            objPayubiz.Email = item.Substring(item.IndexOf("=") + 1);
                //        }
                //        else if (item.Contains("amount"))
                //        {
                //            string subStringVal = item.Substring(0, item.IndexOf("="));
                //            if (!string.IsNullOrEmpty(subStringVal) && subStringVal.Equals("amount"))
                //            {
                //                objPayubiz.Amount = Convert.ToDecimal(item.Substring(item.IndexOf("=") + 1));
                //            }
                //        }
                //    }

                //    objPayubiz.ID = PayubizTransactionId;

                //    PayubizTransactionId = payTrans.SavePayubizPaymentDetails(objPayubiz);

                //    //objPayubiz.responceString = model.Payubiz;


                //    //var json_serializer = new JavaScriptSerializer();
                //    //IDictionary<string, object> routes_list = (IDictionary<string, object>)json_serializer.DeserializeObject(payubiz);
                //    //objPayubiz.Amount = (routes_list.ContainsKey("amount")) ? Convert.ToDecimal(routes_list["amount"]) : 0;
                //    //objPayubiz.Email = (routes_list.ContainsKey("email")) ? routes_list["email"].ToString() : "";
                //    //objPayubiz.FirstName = (routes_list.ContainsKey("firstname")) ? routes_list["firstname"].ToString() : "";
                //    //objPayubiz.Mode = (routes_list.ContainsKey("mode")) ? routes_list["mode"].ToString() : "";
                //    //objPayubiz.OrderId = OrderId;
                //    //objPayubiz.Payid = (routes_list.ContainsKey("id")) ? routes_list["id"].ToString() : "";
                //    //objPayubiz.TxtnId = (routes_list.ContainsKey("txnid")) ? routes_list["txnid"].ToString() : "";
                //    //objPayubiz.UserLoginId = UserLoginId;
                //    //objPayubiz.AddonDate = (routes_list.ContainsKey("addondate")) ? Convert.ToDateTime(routes_list["addondate"].ToString()) : DateTime.Now;
                //    //objPayubiz.Status = (routes_list.ContainsKey("status")) ? routes_list["status"].ToString() : "";
                //    //objPayubiz.responceString = payubiz;



                //    string reqBy = Request.Headers.GetValues("ReqBy").First();
                //    //System.Web.Http.Controllers.HttpActionContext actionContext=new System.Web.Http.Controllers.HttpActionContext();
                //    //string reqBy = actionContext.Request.Headers.GetValues("ReqBy").First();
                //    OnlinePaymentTransactionViewModel paymentDetails = new OnlinePaymentTransactionViewModel();
                //    paymentDetails.DeviceType = reqBy;
                //    paymentDetails.CustomerLoginID = model.UserLoginId;
                //    paymentDetails.PaymentMode = objPayubiz.Mode;
                //    paymentDetails.PaymentGateWayTransactionId = objPayubiz.TxtnId;
                //    if (objPayubiz.Status == "success")
                //        paymentDetails.PaymentStatus = 1;
                //    else
                //        paymentDetails.PaymentStatus = 0;
                //    paymentDetails.CustomerOrderID = model.OrderId;
                //    paymentDetails.Description = "Payubiz";

                //    int oprStatus = payTrans.SavePaymentDetails(paymentDetails);

                //    if (oprStatus == 1 && objPayubiz.Status == "success")
                //    {
                //        int lStatus = 0;
                //        string OrderStatus = "Placed";
                //        BusinessLogicLayer.CustomerOrder lCustOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                //        oprStatus = lCustOrder.UpdatePendingCustomerOrder(model.OrderId, model.UserLoginId, "Update Order status from Pending to Placed By APP");
                //        if (oprStatus == 103)
                //        {
                //            lCustOrder.SendAlertforPushNotification(model.OrderId, OrderStatus);
                //            ////lStatus = lCustOrder.SendPushNotification(orderID);
                //            //lStatus = lCustOrder.AndroidPushFCM(OrderId); //FCM Notification method called
                //            obj = new { Success = 1, Message = "Payment details saved successfully.", data = new { Status = lStatus } };
                //        }
                //        if (oprStatus == 500)
                //            obj = new { Success = 0, Message = "Internal server error.", string.Empty };
                //        // obj = new { HTTPStatusCode = "500", UserMessage = "Internal server error." };
                //        if (oprStatus == 106)
                //            obj = new { Success = 0, Message = "Invalid request. The order you want to Update is not present for given customer login ID.", string.Empty };
                //        // obj = new { HTTPStatusCode = "400", UserMessage = "Invalid request. The order you want to Update is not present for given customer login ID." };

                //    }

                //    // obj = new { HTTPStatusCode = "200", UserMessage = "Payment details saved successfully." };
                //    else if (oprStatus == 0)
                //    {
                //        obj = new { Success = 1, Message = "Payment Failed ! please contact to support", data = new { Status = 0 } };
                //        //obj = new { Success = 0, Message = "Internal Server Error.", data = oprStatus };
                //        //   obj = new { HTTPStatusCode = "500", UserMessage = "Internal Server Error", ValidationError = "problem in saving payment transaction details." };
                //    }
                //    else
                //    {
                //        obj = new { Success = 1, Message = "Payment Failed ! please contact to support", data = new { Status = 0 } };
                //    }
                //}
                //else
                //{
                //    string reqBy = Request.Headers.GetValues("ReqBy").First();
                //    //System.Web.Http.Controllers.HttpActionContext actionContext=new System.Web.Http.Controllers.HttpActionContext();
                //    //string reqBy = actionContext.Request.Headers.GetValues("ReqBy").First();
                //    OnlinePaymentTransactionViewModel paymentDetails = new OnlinePaymentTransactionViewModel();
                //    paymentDetails.DeviceType = reqBy;
                //    paymentDetails.CustomerLoginID = model.UserLoginId;
                //    paymentDetails.PaymentMode = objPayubiz.Mode;
                //    paymentDetails.PaymentGateWayTransactionId = objPayubiz.TxtnId;
                //    if (objPayubiz.Status == "success")
                //        paymentDetails.PaymentStatus = 1;
                //    else
                //        paymentDetails.PaymentStatus = 0;
                //    paymentDetails.CustomerOrderID = model.OrderId;
                //    paymentDetails.Description = "Payubiz";
                //    int oprStatus = payTrans.SavePaymentDetails(paymentDetails);
                //    obj = new { Success = 0, Message = "Payment Failed ! please contact to support", data = 0 };
                //}
            }
            catch (Exception ex)
            {
                EzeeloDBContext db = new EzeeloDBContext();
                PayubizTransaction objPayubiz = new PayubizTransaction();
                objPayubiz.UserLoginId = model.UserLoginId;
                objPayubiz.OrderId = model.OrderId;
                BusinessLogicLayer.PayubizLog.PayubizLogFile("Save Payubiz data:" + model.OrderId, "API", System.Web.HttpContext.Current.Server);
                //objPayubiz.responceString = model.Payubiz;
                db.PayubizTransactions.Add(objPayubiz);
                db.SaveChanges();
                obj = new { Success = 1, Message = "Payment Failed ! please contact to support", data = new { Status = string.Empty } };

                //obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        PayubizTransaction ConversionResponseStringToObj(string ResponseString)
        {
            PayubizTransaction objPayubiz = new PayubizTransaction();
            try
            {
                if (ResponseString.Contains("success") || ResponseString.Contains("Success"))
                {
                    if (ResponseString.Contains("mihpayid"))
                    {
                        string[] result = ResponseString.Split(',');
                        if (result != null && result.Count() > 1)
                        {
                            foreach (var item in result)
                            {
                                if (item.Contains("mihpayid"))
                                {
                                    objPayubiz.Payid = item.Substring(item.IndexOf("=") + 1);
                                }
                                else if (item.Contains("mode"))
                                {
                                    objPayubiz.Mode = item.Substring(item.IndexOf("=") + 1);
                                }
                                else if (item.Contains("unmappedstatus"))
                                {
                                    objPayubiz.UnMappedStatus = item.Substring(item.IndexOf("=") + 1);
                                }
                                else if (item.Contains("status"))
                                {
                                    objPayubiz.Status = item.Substring(item.IndexOf("=") + 1);
                                }
                                else if (item.Contains("txnid"))
                                {
                                    objPayubiz.TxtnId = item.Substring(item.IndexOf("=") + 1);
                                }
                                else if (item.Contains("addedon"))
                                {
                                    objPayubiz.AddonDate = Convert.ToDateTime(item.Substring(item.IndexOf("=") + 1));
                                }
                                else if (item.Contains("firstname"))
                                {
                                    objPayubiz.FirstName = item.Substring(item.IndexOf("=") + 1);
                                }
                                else if (item.Contains("email"))
                                {
                                    objPayubiz.Email = item.Substring(item.IndexOf("=") + 1);
                                }
                                else if (item.Contains("amount"))
                                {
                                    string subStringVal = item.Substring(0, item.IndexOf("="));
                                    if (!string.IsNullOrEmpty(subStringVal) && subStringVal.Equals("amount"))
                                    {
                                        objPayubiz.Amount = Convert.ToDecimal(item.Substring(item.IndexOf("=") + 1));
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        var json_serializer = new JavaScriptSerializer();
                        IDictionary<string, object> routes_list = (IDictionary<string, object>)json_serializer.DeserializeObject(ResponseString);
                        objPayubiz.Amount = (routes_list.ContainsKey("amount")) ? Convert.ToDecimal(routes_list["amount"]) : 0;
                        objPayubiz.Email = (routes_list.ContainsKey("email")) ? routes_list["email"].ToString() : "";
                        objPayubiz.FirstName = (routes_list.ContainsKey("firstname")) ? routes_list["firstname"].ToString() : "";
                        objPayubiz.Mode = (routes_list.ContainsKey("mode")) ? routes_list["mode"].ToString() : "";
                        objPayubiz.Payid = (routes_list.ContainsKey("id")) ? routes_list["id"].ToString() : "";
                        objPayubiz.TxtnId = (routes_list.ContainsKey("txnid")) ? routes_list["txnid"].ToString() : "";
                        objPayubiz.AddonDate = (routes_list.ContainsKey("addondate")) ? Convert.ToDateTime(routes_list["addondate"].ToString()) : DateTime.Now;
                        objPayubiz.Status = (routes_list.ContainsKey("status")) ? routes_list["status"].ToString() : "";
                    }
                }
                else
                {
                    var json_serializer = new JavaScriptSerializer();
                    IDictionary<string, object> routes_list = (IDictionary<string, object>)json_serializer.DeserializeObject(ResponseString);
                    objPayubiz.Amount = (routes_list.ContainsKey("amount")) ? Convert.ToDecimal(routes_list["amount"]) : 0;
                    objPayubiz.Email = (routes_list.ContainsKey("email")) ? routes_list["email"].ToString() : "";
                    objPayubiz.FirstName = (routes_list.ContainsKey("firstname")) ? routes_list["firstname"].ToString() : "";
                    objPayubiz.Mode = (routes_list.ContainsKey("mode")) ? routes_list["mode"].ToString() : "";
                    objPayubiz.Payid = (routes_list.ContainsKey("id")) ? routes_list["id"].ToString() : "";
                    objPayubiz.TxtnId = (routes_list.ContainsKey("txnid")) ? routes_list["txnid"].ToString() : "";
                    objPayubiz.AddonDate = (routes_list.ContainsKey("addondate")) ? Convert.ToDateTime(routes_list["addondate"].ToString()) : DateTime.Now;
                    objPayubiz.Status = (routes_list.ContainsKey("status")) ? routes_list["status"].ToString() : "";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with conversion of response string.");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrder][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
            }
            return objPayubiz;
        }
    }
}
