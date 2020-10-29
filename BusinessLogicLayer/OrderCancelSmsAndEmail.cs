using ModelLayer.Models.ViewModel;
//-----------------------------------------------------------------------
// <copyright file="OrderCancelSmsAndEmail" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 Handed over to Pradnyakar Sir
 */
namespace BusinessLogicLayer
{
    public class OrderCancelSmsAndEmail : CustomerOrder, ISmsAndEmail
    {

        public OrderCancelSmsAndEmail(System.Web.HttpServerUtility server)
            : base(server)
        {
        }
        /// <summary>
        /// Send Mail to customer for order Cancelled
        /// </summary>
        /// <param name="userLoginID">Customer Login ID</param>
        /// <param name="orderId">Order ID</param>
        public void SendMailToCustomer(long userLoginID, long orderId)
        {
            try
            {
                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();

                var lOrder = db.CustomerOrders.Where(x => x.ID == orderId).ToList();
                var lPersonalDetails = db.PersonalDetails.Where(x => x.UserLoginID == userLoginID).FirstOrDefault();
                var lUserLogin = db.UserLogins.Where(x => x.ID == userLoginID).FirstOrDefault();
                string city = "nagpur";
                int franchiseId = 2;////added
                if (System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value != null)
                {
                    city = System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                    franchiseId =Convert.ToInt32( System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[2]);////added
                }
                //  DataTable dtCustomerDetails = this.GetCustomerDetails();

                if (lOrder.Count() > 0)
                {
                    DateTime dateTimeOrderDate = Convert.ToDateTime(lOrder.FirstOrDefault().CreateDate);
                    string orderDate = dateTimeOrderDate.ToString("MMM dd, yyyy");
                    string orderTime = dateTimeOrderDate.ToShortTimeString();

                    dictEmailValues.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseId + "/login");////added "/" + franchisrId +
                    dictEmailValues.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseId + "/cust-o/my-order");////added "/" + franchisrId +
                    dictEmailValues.Add("<!--ORDER_NO-->", lOrder.FirstOrDefault().OrderCode);
                    dictEmailValues.Add("<!--NAME-->", Convert.ToString(lPersonalDetails.FirstName));
                    dictEmailValues.Add("<!--ORDER_DATE-->", orderDate);
                    dictEmailValues.Add("<!--ORDER_TIME-->", orderTime);

                    //dictEmailValues.Add("<!--CONTACT_NUMBER-->", Convert.ToString(lOrder.FirstOrDefault().PrimaryMobile));

                    StringBuilder sb = new StringBuilder();

                    if (Convert.ToString(lOrder.FirstOrDefault().ShippingAddress) != string.Empty)
                        sb.Append(Convert.ToString(lOrder.FirstOrDefault().ShippingAddress) + "<br/>");
                    if (Convert.ToString(lOrder.FirstOrDefault().Pincode) != string.Empty)
                        sb.Append(Convert.ToString(lOrder.FirstOrDefault().Pincode) + "<br/>");

                    dictEmailValues.Add("<!--DELIVERY_ADDRESS-->", sb.ToString());

                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                    string customerEmail = string.Empty;
                    if (lUserLogin != null)
                    {
                        customerEmail = lUserLogin.Email;//db.UserLogins.Where(x => x.ID == userLoginID).FirstOrDefault().Email;
                    }

                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_ORD_CANCELLED_REPLY, new string[] { customerEmail, "sales@ezeelo.com" }, dictEmailValues, true);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderController][M:SentMailToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderController][M:SentMailToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }
        /// <summary>
        /// Send Mail to all merchants involved in order Cancelled
        /// </summary>
        /// <param name="userLoginID">Customer Login ID</param>
        /// <param name="orderId">Order ID</param>
        public void SendMailToMerchant(long userLoginID, long orderId)
        {
            try
            {
                var lOrder = db.CustomerOrders.Where(x => x.ID == orderId).ToList();
                var lOrderDetails = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == orderId).ToList();
                var lPersonalDetails = db.PersonalDetails.Where(x => x.UserLoginID == userLoginID).FirstOrDefault();
                var lUserLogin = db.UserLogins.Where(x => x.ID == userLoginID).FirstOrDefault();

                if (lOrder.Count() > 0)
                {
                    var filterMerchant = lOrderDetails.Select(x => new { x.ShopID }).Distinct().ToList();

                    for (int i = 0; i < filterMerchant.Count(); i++)
                    {
                        int shopId = 0;
                        int.TryParse(Convert.ToString(filterMerchant[i].ShopID), out shopId);

                        var lShopDetails = db.Shops.Where(x => x.ID == shopId).FirstOrDefault();

                        //if (lShopDetails.Count > 0)
                        //{
                        DateTime dateTimeOrderDate = Convert.ToDateTime(lOrder.FirstOrDefault().CreateDate);
                        string orderDate = dateTimeOrderDate.ToString("MMM dd, yyyy");
                        string orderTime = dateTimeOrderDate.ToShortTimeString();

                        Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                        dictEmailValues.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "");
                        dictEmailValues.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "/Placed");
                        dictEmailValues.Add("<!--ORDER_NO-->", orderId.ToString());
                        dictEmailValues.Add("<!--ORDER_DATE-->", orderDate);
                        dictEmailValues.Add("<!--ORDER_TIME-->", orderTime);
                        dictEmailValues.Add("<!--NAME-->", Convert.ToString(lShopDetails.ContactPerson));
                        dictEmailValues.Add("<!--CUSTOMER_NAME-->", Convert.ToString(lPersonalDetails.FirstName));
                        dictEmailValues.Add("<!--CONTACT_NUMBER-->", Convert.ToString(lOrder.FirstOrDefault().PrimaryMobile));

                        StringBuilder sb = new StringBuilder();

                        if (Convert.ToString(lOrder.FirstOrDefault().ShippingAddress) != string.Empty)
                            sb.Append(Convert.ToString(lOrder.FirstOrDefault().ShippingAddress) + "<br/>");
                        if (Convert.ToString(lOrder.FirstOrDefault().Pincode) != string.Empty)
                            sb.Append(Convert.ToString(lOrder.FirstOrDefault().Pincode) + "<br/>");

                        dictEmailValues.Add("<!--DELIVERY_ADDRESS-->", sb.ToString());

                        BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                        ReadConfig readConfig = new ReadConfig(System.Web.HttpContext.Current.Server);
                        gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ORD_CANCELLED_CUST, new string[] { lShopDetails.Email, readConfig.CRM_EMAIL, "sales@ezeelo.com" }, dictEmailValues, true);
                        //}
                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OrderCancelSmsAndEmail][M:SentMailToMerchant]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OrderCancelSmsAndEmail][M:SentMailToMerchant]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }
        /// <summary>
        /// Send SMS to customer for order Cancelled
        /// </summary>
        /// <param name="userLoginID">Customer Login ID</param>
        /// <param name="orderId">Order ID</param>
        public void SendSMSToCustomer(long userLoginID, long orderId)
        {
            try
            {
                var lOrder = db.CustomerOrders.Where(x => x.ID == orderId).ToList();
                var lPersonalDetails = db.PersonalDetails.Where(x => x.UserLoginID == userLoginID).ToList();

                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();

                if (lOrder.Count() > 0)
                {
                    dictSMSValues.Add("#--NAME--#", Convert.ToString(lPersonalDetails.FirstOrDefault().FirstName));
                    dictSMSValues.Add("#--ORD_NUM--#", lOrder.FirstOrDefault().OrderCode.ToString());

                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                    gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_CAN, new string[] { lOrder.FirstOrDefault().PrimaryMobile.ToString()}, dictSMSValues);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OrderCancelSmsAndEmail][M:SendSMSToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OrderCancelSmsAndEmail][M:SendSMSToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }
        /// <summary>
        /// Send SMS to all Merchants for order Cancelled
        /// </summary>
        /// <param name="userLoginID">Customer Login ID</param>
        /// <param name="orderId">Order ID</param>
        public void SendSMSToMerchant(long userLoginID, long orderId)
        {
            try
            {
                var lOrder = db.CustomerOrders.Where(x => x.ID == orderId).ToList();
                var lOrderDetails = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == orderId).ToList();
                var lPersonalDetails = db.PersonalDetails.Where(x => x.UserLoginID == userLoginID).ToList();
                var lUserLogin = db.UserLogins.Where(x => x.ID == userLoginID).ToList();

                if (lOrder.Count() > 0)
                {
                    var filterMerchant = lOrderDetails.Select(x => new { x.ShopID }).Distinct().ToList();

                    for (int i = 0; i < filterMerchant.Count(); i++)
                    {
                        int shopId = 0;
                        int.TryParse(Convert.ToString(filterMerchant[i].ShopID), out shopId);

                        var lShopDetails = db.Shops.Where(x => x.ID == shopId).ToList();

                        if (lShopDetails.Count > 0)
                        {
                            DateTime dateTimeOrderDate = Convert.ToDateTime(lOrder.FirstOrDefault().CreateDate);
                            string orderDate = dateTimeOrderDate.ToString("MMM dd, yyyy");
                            string orderTime = dateTimeOrderDate.ToShortTimeString();

                            Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                            dictSMSValues.Add("#--ORD_NUM--#", orderId.ToString());

                            dictSMSValues.Add("#--NAME--#", Convert.ToString(lShopDetails.FirstOrDefault().ContactPerson));

                            dictSMSValues.Add("#--CANCELBY--#", Convert.ToString(lPersonalDetails.FirstOrDefault().FirstName));

                            StringBuilder sb = new StringBuilder();

                            BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                            ReadConfig readConfig = new ReadConfig(System.Web.HttpContext.Current.Server);
                            //gateWay.SendSMS(GateWay.SMSGateWays.SUMIT, GateWay.SMSOptions.SINGLE, GateWay.SMSTypes.MER_ORD_CAN, new string[] { lShopDetails.FirstOrDefault().Mobile, readConfig.CRM_MOBILE,}, dictSMSValues);
                            gateWay.SendSMS(GateWay.SMSGateWays.SUMIT, GateWay.SMSOptions.SINGLE, GateWay.SMSTypes.MER_ORD_CAN, new string[] { lShopDetails.FirstOrDefault().Mobile}, dictSMSValues);
                        }
                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OrderCancelSmsAndEmail][M:SendSMSToMerchant]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OrderCancelSmsAndEmail][M:SendSMSToMerchant]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }
    }
}
