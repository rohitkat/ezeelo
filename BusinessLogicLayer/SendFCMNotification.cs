using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using ModelLayer.Models;

namespace BusinessLogicLayer
{
    //Yashaswi for Notification 2-7-2019
    public class SendFCMNotification
    {
        EzeeloDBContext db = new EzeeloDBContext();

        public void PushFCM(string deviceType, string FCMRegistartionId, string body_, string moveto_, long categoryID_, long brandId_, string keyword_,long order_id_,string OrderStatusName_)
        {
            string Response = "";
            OrderStatusName_ = OrderStatusName_.ToUpper();
            try
            {
                string AuthorizationKey = WebConfigurationManager.AppSettings["FCM_AUTHORIZATION_KEY"];
                if (deviceType.ToLower() == "android")
                {
                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    var data = new
                    {
                        to = FCMRegistartionId,
                        data = new
                        {
                            body = body_,
                            moveto = moveto_,
                            order_id = order_id_,
                            OrderStatusName = OrderStatusName_,
                            categoryID = categoryID_,
                            brandId = brandId_,
                            keyword = keyword_,
                        }
                    };
                    var serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(data);
                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                    tRequest.ContentLength = byteArray.Length;
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", AuthorizationKey));
                    tRequest.Method = "post";
                    tRequest.ContentType = "application/json";

                    using (Stream dataStream = tRequest.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        using (WebResponse tResponse = tRequest.GetResponse())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = tReader.ReadToEnd();
                                    Response = sResponseFromServer;
                                }
                            }
                        }
                    }
                    BusinessLogicLayer.ErrorLog.ErrorLogFile("Notification send in android :" + Response, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
                }
                else
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile("Notification send in IOS :" + Response, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in sending notification :" + ex.Message + ex.InnerException, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
            }
        }

        public string GenerateMsg(string code)
        {
            string msg = "";
            switch (code)
            {
                case "placed":
                    msg = "Dear <!--UNAME-->, Order No. <!--ORDCODE--> @ Ezeelo has been successfully placed. Helpdesk 9172221910";
                    break;
                case "delivered":
                    msg = "Delivered: Your Order <!--ORDCODE--> has been delivered.";
                    break;
                case "cancelled":
                    msg = "Order cancelled <!--ORDCODE--> Your order has been cancelled on <!--DATE--> as per your request for more info contact on 9172221910";
                    break;
                case "returned":
                    msg = "Dear <!--UNAME-->, order no.<!--ORDCODE--> @Ezeelo has been returned, If you'd like the delivery to be reattemted, Please call on 9172221910";
                    break;
                case "qrp_complete":
                    msg = "Congratulations ..Dear Leader You have successfully completed your QRP for this cycle.";
                    break;
                case "desg_changed":
                    msg = "BINGO!!!!Dear <!--UNAME-->, now you are promoted to the designation <!--DESIGNATION-->,";
                    break;
                case "days_rem_payout":
                    msg = "Dear <!--UNAME-->, In current cycle <!--DAYS-->, days are left to become active and maximise your benefits";
                    break;
                case "payout":
                    msg = "Dear <!--UNAME-->, for last cycle which ended on 25th of <!--MONTH--> payout is released , For more info contact on 9172221910";
                    break;
                case "inactive_points_payout":
                    msg = "Dear <!--UNAME-->, Your INACTIVE Points payout released today. Verify it on your dashboard.";
                    break;
                case "withdrawn_send":
                    msg = "Dear <!--UNAME-->, we have received your wallet payout, we will get back to you very soon…";
                    break;
                case "withdrawn_accept":
                    msg = "Congrates Dear <!--UNAME-->, Your request of amount <!--AMOUNT--> has been accepted from the accounts section , you will get your transfer with in 2-3 working days.";
                    break;
                case "withdrawn_approve":
                    msg = "Dear <!--UNAME-->, we have successfully transferred Rs.<!--APPRAMOUNT--> in you bank Account.";
                    break;
                case "withdrawn_cancel":
                    msg = "Dear Customer Your request for payout has beed cancelled for more info contact on 9172221910";
                    break;
                case "downline_active":
                    msg = "Dear Leader today in your <!--LEVEL--> downline <!--UNAME--> is now active for this cycle.";
                    break;
                    //case "offer":
                    //    msg = "Hello <!--UNAME-->, check here some exciting offer.";
                    //    break;
            }
            return msg;
        }

        public string GetMoveToCode(string code, long Id, string body_, out long UserloginId, out string body)
        {
            string moveto_ = "";
            UserloginId = 0;
            long CustomerOrderId = 0;
            switch (code)
            {
                case "placed":
                case "delivered":
                case "cancelled":
                case "returned":
                case "partial_return":
                    moveto_ = "my-ord";
                    CustomerOrderId = Id;
                    string OrdCode = "";
                    string Date = "";
                    ModelLayer.Models.CustomerOrder customerOrder = db.CustomerOrders.FirstOrDefault(p => p.ID == CustomerOrderId);
                    OrdCode = customerOrder.OrderCode;
                    UserloginId = customerOrder.UserLoginID;
                    Id = customerOrder.UserLoginID;
                    if (customerOrder.ModifyDate != null)
                    {
                        Date = Convert.ToDateTime(customerOrder.ModifyDate).ToString("dd-MMM-yy");
                    }
                    body_ = body_.Replace("<!--ORDCODE-->", OrdCode);
                    body_ = body_.Replace("<!--DATE-->", Date);
                    break;
                case "qrp_complete":
                case "desg_changed":
                case "days_rem_payout":
                    moveto_ = "ldrs-dash";
                    UserloginId = Id;
                    break;
                case "payout":
                case "inactive_points_payout":
                    moveto_ = "ldrs-ezemny";
                    UserloginId = Id;
                    EzeeMoneyPayout ezeeMoneyPayout = db.EzeeMoneyPayouts.OrderByDescending(p => p.Id).FirstOrDefault();
                    if (ezeeMoneyPayout != null)
                    {
                        if (db.EzeeMoneyPayoutDetail.Any(p => p.UserLoginId == Id && p.EzeeMoneyPayoutID == ezeeMoneyPayout.Id))
                        {
                            body_ = body_.Replace("<!--MONTH-->", ezeeMoneyPayout.FreezeDate.ToString("MMMM"));
                        }
                    }
                    break;
                case "withdrawn_send":
                case "withdrawn_accept":
                case "withdrawn_approve":
                case "withdrawn_cancel":
                    moveto_ = "ldrs-rpt-ezesmry";
                    LeadersPayoutRequest leadersPayoutRequest = db.LeadersPayoutRequests.Where(p => p.UserLoginID == Id).OrderByDescending(p => p.ID).FirstOrDefault();
                    if (leadersPayoutRequest != null)
                    {
                        body_ = body_.Replace("<!--AMOUNT-->", leadersPayoutRequest.RequestedAmount.ToString());
                        body_ = body_.Replace("<!--APPRAMOUNT-->", leadersPayoutRequest.TotalAmount.ToString());
                    }
                    UserloginId = Id;
                    break;
                case "offer":
                    moveto_ = "home-pg";
                    UserloginId = Id;
                    break;
            }
            PersonalDetail personalDetail = db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == Id);
            body_ = body_.Replace("<!--UNAME-->", personalDetail.FirstName + " " + personalDetail.MiddleName ?? "" + " " + personalDetail.LastName ?? "");
            body = body_;
            return moveto_;
        }

        public void SendNotification(string code, long Id)
        {
            try
            {
                long UserloginId = 0;
                string body_ = GenerateMsg(code);
                string moveto_ = GetMoveToCode(code, Id, body_, out UserloginId, out body_);
                string FCMRegistartionId = "";
                long lUserLoginID = UserloginId;
                if(Id == UserloginId)
                {
                    Id = 0;
                }
                //<!--UNAME--> <!--DESIGNATION--> <!--ORDCODE--> <!--DATE--> <!--MONTH--> <!--AMOUNT--> <!--APPRAMOUNT--> <!--LEVEL--> <!--UNAME-->
                FCMUser fCMUser = db.FCMUsers.FirstOrDefault(p => p.UserLoginId == lUserLoginID);
                if (fCMUser != null)
                {
                    FCMRegistartionId = fCMUser.FCMRegistartionId;
                    PushFCM(fCMUser.DeviceType, fCMUser.FCMRegistartionId, body_, moveto_, 0, 0, "", Id, code);
                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in sending notification SendNotification():" + ex.Message + ex.InnerException, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
            }

        }

        public void DemoSendNotification(string code, string FCMRegistartionId)
        {
            try
            {
                long Id = 0;
                long UserloginId = 0;
                string body_ = GenerateMsg(code);
                switch (code)
                {
                    case "placed":
                    case "delivered":
                    case "cancelled":
                    case "return":
                    case "partial_return":
                        Id = 86987;
                        break;
                    default:
                        Id = 347816;
                        break;
                }
                string moveto_ = GetMoveToCode(code, Id, body_, out UserloginId, out body_);

                if (Id == UserloginId)
                {
                    Id = 0;
                }
                
                if (code == "return")
                {
                    code = "returned";
                }
                PushFCM("android", FCMRegistartionId, body_, moveto_, 0, 0, "", Id, code);

            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in sending notification SendNotification():" + ex.Message + ex.InnerException, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
            }

        }
    }
}
