//-----------------------------------------------------------------------
// <copyright file="OrderPlacedSmsAndEmail" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
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
    public class OrderPlacedSmsAndEmail : CustomerOrder, ISmsAndEmail
    {
        public OrderPlacedSmsAndEmail(System.Web.HttpServerUtility server)
            : base(server)
        {
        }
        /// <summary>
        /// Send Mail to customer for order placement
        /// </summary>
        /// <param name="userLoginID">Customer Login ID</param>
        /// <param name="orderId">Order ID</param>
        public void SendMailToCustomer(long pUserLoginID, long pOrderID)
        {
            try
            {
                EzeeloDBContext db = new EzeeloDBContext();

                var personalDetail = db.PersonalDetails.Where(x => x.UserLoginID == pUserLoginID).ToList();
                var userLogin = db.UserLogins.Where(x => x.ID == pUserLoginID).ToList();
                var customerOrder = db.CustomerOrders.Find(pOrderID);

                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();

                dictEmailValues.Add("<!--ORDER_NO-->", customerOrder.OrderCode);
                dictEmailValues.Add("<!--ORDER_DATE-->", DateTime.Now.ToString("MMM dd, yyyy"));
                dictEmailValues.Add("<!--ORDER_TIME-->", DateTime.Now.ToShortTimeString());
                dictEmailValues.Add("<!--NAME-->", personalDetail.FirstOrDefault().FirstName);
                dictEmailValues.Add("<!--PAY_METHOD-->", " by " + customerOrder.PaymentMode + " payement method");


                //var listOfAddress = (List<CustomerShippingAddress>)TempData["ShippingAddress"];
                if (customerOrder != null)
                {
                    // Find selected shipping address
                    //var shippingAddress = listOfAddress.Where(x => x.ID == Convert.ToInt32(SelectedAddress)).ToList().FirstOrDefault();

                    dictEmailValues.Add("<!--CONTACT_NUMBER-->", Convert.ToString(customerOrder.PrimaryMobile));

                    StringBuilder sb = new StringBuilder();

                    if (Convert.ToString(customerOrder.ShippingAddress) != string.Empty)
                    {
                        sb.Append(Convert.ToString(customerOrder.ShippingAddress) + "<br/>");
                        sb.Append(Convert.ToString(db.Pincodes.Find(customerOrder.PincodeID).Name) + "<br/>");
                    }
                    dictEmailValues.Add("<!--DELIVERY_ADDRESS-->", sb.ToString());
                }
                else
                {
                    dictEmailValues.Add("<!--SHIPPING_MOBILE-->", string.Empty);
                    dictEmailValues.Add("<!--DELIVERY_ADDRESS-->", string.Empty);
                }

                string uid = "0";
                //----Comment by mohit as code was not working and session will never avalable for app(moblie)
                //if(System.Web.HttpContext.Current.Session["UID"] != null)
                //uid=System.Web.HttpContext.Current.Session["UID"].ToString();

                uid = pUserLoginID.ToString();

                string city = "";
                int franchiseId = 0;
                if (System.Web.HttpContext.Current.Request.Cookies["CityCookie"] != null && System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value != "")
                {
                    city = System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                    franchiseId =Convert.ToInt32( System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[2]);
                }
                
                dictEmailValues.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseId + "/login");////added "/" + franchiseId +
                dictEmailValues.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseId + "/cust-o/my-order");////added "/" + franchiseId +
                dictEmailValues.Add("<!--URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseId + "/" + customerOrder.ID.ToString() + "/" + uid + "/cust-o/order-h");////added "/" + franchiseId +

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                //code changes by mohit//
                string EmailID = db.UserLogins.Where(x => x.ID == pUserLoginID).Select(x => x.Email).FirstOrDefault();


                // gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_ORD_PLACED, new string[] { userLogin.FirstOrDefault().Email.ToString() }, dictEmailValues, true);
                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_ORD_PLACED, new string[] { EmailID, "sales@ezeelo.com" }, dictEmailValues, true);


            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendMailToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendMailToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }
        /// <summary>
        /// Send Mail to All merchants involved in this order placement
        /// </summary>
        /// <param name="userLoginID">Customer Login ID</param>
        /// <param name="orderId">Order ID</param>
        public void SendMailToMerchant(long pUserLoginID, long pOrderID)
        {
            try
            {
                Dictionary<string, string> dictEmailValuesMerchant = new Dictionary<string, string>();
                var customerOrder = db.CustomerOrders.Find(pOrderID);

                dictEmailValuesMerchant.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "");
                dictEmailValuesMerchant.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "/Placed");


                StringBuilder sbHtml = new StringBuilder(
                       "<table border=\"0\" cellpadding=\"5\" cellspacing=\"0\" width=\"100%\" style=\"text-align: center; font-family: Calibri; font-size: 1.5vw; color: #4f4f4f;\">" + // table header
                       "<thead>" +
                       "<tr>" +
                       "<th style=\"border: 1px solid #b8b8b7; border-right:none; border-bottom:none;\">Preferred Delivery Time</th>" +
                       "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">Product Name</th>" +
                       "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">Quantity</th>" +
                       "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">Delivery Type</th>" +
                       "</tr>" +
                       "</thead>" +
                       "<tbody>"
                    );


                //dictEmailValuesMerchant.Add("<!--ORDER_NO-->", customerOrder.OrderCode);
                //dictEmailValuesMerchant.Add("<!--ORDER_DATE-->", DateTime.Now.ToString("MMM dd, yyyy"));
                //dictEmailValuesMerchant.Add("<!--ORDER_TIME-->", DateTime.Now.ToShortTimeString());

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                //ShopProductVarientViewModelCollection lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["CartCollection"];

                var distinctMerchant = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == customerOrder.ID).Select(x => new { x.ShopID }).Distinct().ToList();
                var personalDetail = db.PersonalDetails.Where(x => x.UserLoginID == pUserLoginID).ToList();

                if (distinctMerchant.Count() > 0)
                {
                    for (int i = 0; i < distinctMerchant.Count(); i++)
                    {

                        long shopID = distinctMerchant[i].ShopID;
                        var shopDetails = db.Shops.Where(x => x.ID == shopID).ToList();

                        int shopId = 0;
                        int.TryParse(shopDetails.FirstOrDefault().ID.ToString(), out shopId);



                        this.SendSMSToMerchant(shopId, pOrderID);

                        if (shopDetails.Count() > 0)
                        {
                            dictEmailValuesMerchant.Add("<!--NAME-->", Convert.ToString(shopDetails.FirstOrDefault().ContactPerson));

                            //var listOfAddress = (List<CustomerShippingAddress>)TempData["ShippingAddress"];
                            if (customerOrder != null)
                            {
                                // Find selected shipping address
                                //var shippingAddress = listOfAddress.Where(x => x.ID == Convert.ToInt32(SelectedAddress)).ToList().FirstOrDefault();

                                //dictEmailValuesMerchant.Add("<!--CUSTOMER_NAME-->", Convert.ToString(personalDetail.FirstOrDefault().FirstName));
                                //dictEmailValuesMerchant.Add("<!--CONTACT_NUMBER-->", Convert.ToString(customerOrder.PrimaryMobile));

                                StringBuilder sb = new StringBuilder();

                                if (Convert.ToString(customerOrder.ShippingAddress) != string.Empty)
                                {
                                    sb.Append(Convert.ToString(customerOrder.ShippingAddress) + "<br/>");
                                    sb.Append(Convert.ToString(db.Pincodes.Find(customerOrder.PincodeID).Name) + "<br/>");
                                }

                                //dictEmailValuesMerchant.Add("<!--DELIVERY_ADDRESS-->", sb.ToString());


                                var merchantProductList = (from cod in db.CustomerOrderDetails
                                                           join ss in db.ShopStocks on cod.ShopStockID equals ss.ID
                                                           join SP in db.ShopProducts on ss.ShopProductID equals SP.ID
                                                           join p in db.Products on SP.ProductID equals p.ID
                                                           join dod in db.DeliveryOrderDetails on cod.ShopOrderCode equals dod.ShopOrderCode
                                                           where SP.ShopID == shopID && cod.CustomerOrderID == pOrderID

                                                           select new
                                                           {
                                                               ProductName = p.Name,
                                                               Qty = cod.Qty,
                                                               OrderDate = cod.CreateDate,
                                                               DeliveryType = dod.DeliveryType

                                                           }).ToList();

                                if (merchantProductList != null)
                                {
                                    foreach (var item in merchantProductList)
                                    {

                                        sbHtml.AppendFormat(
                                           "<tr>" +
                                           "<th style=\"border: 1px solid #b8b8b7; border-right:none; border-bottom:none;\">{0}</th>" +
                                           "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{1}</th>" +
                                           "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{2}</th>" +
                                           "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{3}</th>" +
                                           "</tr>", "No Preffered Time Available", item.ProductName.ToString().Trim(), item.Qty, item.DeliveryType.ToString().Trim()
                                            );
                                    }


                                }


                                //db.CustomerOrderDetails.Where(x => x.CustomerOrderID == pOrderID && x.ShopID == distinctMerchant[i].ShopID).ToList();
                            }
                            else
                            {
                                //dictEmailValuesMerchant.Add("<!--CUSTOMER_NAME-->", string.Empty);
                                //dictEmailValuesMerchant.Add("<!--CONTACT_NUMBER-->", string.Empty);
                                //dictEmailValuesMerchant.Add("<!--DELIVERY_ADDRESS-->", string.Empty);
                                sbHtml.AppendFormat(
                                           "<tr>" +
                                           "<th style=\"border: 1px solid #b8b8b7; border-right:none; border-bottom:none;\">{0}</th>" +
                                           "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{0}</th>" +
                                           "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{0}</th>" +
                                           "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{0}</th>" +
                                           "</tr>", "No Details Available"
                                            );
                            }
                            sbHtml.Append("</tbody></table>");
                            dictEmailValuesMerchant.Add("<!--ORDER_DETAILS-->", sbHtml.ToString());
                            dictEmailValuesMerchant.Add("<!--URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "/Placed");

                            //---Added by mohit on 23-01-16 for help line number as per city---//
                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                            List<CityHelpLineNoViewModel> lCitlHelpLine = new List<CityHelpLineNoViewModel>();
                            lCitlHelpLine = BusinessLogicLayer.CityHelpLineNo.GetCityHelpLineNo(shopId).ToList();
                            dictEmailValuesMerchant.Add("<!--HELP_DESK-->", lCitlHelpLine[0].HelpLineNumber);
                            //---End Of Code By Mohit---//

                            ReadConfig readConfig = new ReadConfig(System.Web.HttpContext.Current.Server);
                            gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.MER_ORD_PLACED, new string[] { shopDetails.FirstOrDefault().Email.ToString(), readConfig.CRM_EMAIL, "sales@ezeelo.com" }, dictEmailValuesMerchant, true);
                            dictEmailValuesMerchant.Remove("<!--NAME-->");
                            dictEmailValuesMerchant.Remove("<!--URL-->");
                            dictEmailValuesMerchant.Remove("<!--ORDER_DETAILS-->");


                        }
                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendMailToMerchant]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendMailToMerchant]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }
        /// <summary>
        /// Send Mail to customer for order placement
        /// </summary>
        /// <param name="userLoginID">Customer Login ID</param>
        /// <param name="orderId">Order ID</param>
        public void SendSMSToCustomer(long pUserLoginID, long pOrderID)
        {
            try
            {
                CommonFunctions cf = new CommonFunctions();

                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();

                var personalDetail = db.PersonalDetails.Where(x => x.UserLoginID == pUserLoginID).ToList();
                var customerOrder = db.CustomerOrders.Find(pOrderID);
                //---Code to get detail of Customer order details ---added on 23-01-16---//
                CustomerOrderDetail lCustomerOrderDetail = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == pOrderID).FirstOrDefault();
                //---End of code by mohit on 23-01-16---//

                dictSMSValues.Add("#--NAME--#", personalDetail.FirstOrDefault().FirstName);
                dictSMSValues.Add("#--ORD_NUM--#", customerOrder.OrderCode);


                //---Added by mohit on 23-01-16 for help line number as per city---//
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                List<CityHelpLineNoViewModel> lCitlHelpLine = new List<CityHelpLineNoViewModel>();
                lCitlHelpLine = BusinessLogicLayer.CityHelpLineNo.GetCityHelpLineNo(Convert.ToInt64(lCustomerOrderDetail.ShopID)).ToList();
                dictSMSValues.Add("#--HELP_DESK--#", lCitlHelpLine[0].HelpLineNumber);
                //---End Of Code By Mohit---//

                //================== Commented by Tejaswee for not sending SMS to other mobiles (7-6-2016)
                //if (customerOrder != null)
                //{

                //    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                //    ReadConfig readConfig = new ReadConfig(System.Web.HttpContext.Current.Server);

                //    string[] mobileNos = readConfig.ADMIN_MOBILE.Split(',');

                //    string[] recipients = new string[mobileNos.Length + 3];

                //    recipients[0] = customerOrder.PrimaryMobile.ToString();
                //    //recipients[1] = "9422149985";
                //    recipients[1] = lCitlHelpLine.FirstOrDefault().ManagmentNumber;//---Added by mohit on 25-01-2016 for SMS to city CRM--
                //    for (int i = 3; i < recipients.Length; i++)
                //        recipients[i] = mobileNos[i - 3];

                //    gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_PLCD, recipients, dictSMSValues);
                //}


                if (customerOrder != null)
                {
                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                    gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_PLCD,new string[]{ customerOrder.PrimaryMobile.ToString()}, dictSMSValues);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendSMSToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendSMSToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }

        /// <summary>
        /// Send SMS to customer for order placement
        /// </summary>
        /// <param name="userLoginID">Customer Login ID</param>
        /// <param name="orderId">Order ID</param>
        public void SendSubScriptionMailToCustomer(long pUserLoginID)
        {
            try
            {
                CommonFunctions cf = new CommonFunctions();

                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();

                var personalDetail = db.PersonalDetails.Where(x => x.UserLoginID == pUserLoginID).FirstOrDefault();
                string EmailID = db.UserLogins.Where(x => x.ID == pUserLoginID).Select(x => x.Email).FirstOrDefault();
                dictEmailValues.Add("<!--SALUTATION_NAME-->", personalDetail.Salutation.Name);
                dictEmailValues.Add("<!--NAME-->", personalDetail.FirstName);


                if (personalDetail != null)
                {

                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                    ReadConfig readConfig = new ReadConfig(System.Web.HttpContext.Current.Server);
                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_PURCHASE_SUBSCRIPTION, new string[] { EmailID, "sales@ezeelo.com" }, dictEmailValues, true);
                    //gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_PLCD, new string[] { customerOrder.PrimaryMobile.ToString(), readConfig.ADMIN_MOBILE }, dictSMSValues);
                
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendSubScriptionMailToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendSubScriptionMailToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }
        /// <summary>
        /// Send Mail to All merchants involved in this order placement
        /// </summary>
        /// <param name="userLoginID">Customer Login ID</param>
        /// <param name="orderId">Order ID</param>
        public void SendSubScriptionSMSToCustomer(long pUserLoginID, int NoOfDays)
        {
            try
            {
                CommonFunctions cf = new CommonFunctions();

                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();

                var personalDetail = db.PersonalDetails.Where(x => x.UserLoginID == pUserLoginID).ToList();
                var MobileNo = db.UserLogins.Where(x => x.ID == pUserLoginID).Select(x => x.Mobile).FirstOrDefault();

                dictSMSValues.Add("#--SUBSCRIPTION_DAYS--#", NoOfDays.ToString());
                //dictSMSValues.Add("#--ORD_NUM--#", customerOrder.OrderCode);

                if (NoOfDays != null)
                {

                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                    ReadConfig readConfig = new ReadConfig(System.Web.HttpContext.Current.Server);
                    //gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_SUBSCR_PURCHASE, new string[] { MobileNo, readConfig.ADMIN_MOBILE }, dictSMSValues);
                    gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_SUBSCR_PURCHASE, new string[] { MobileNo }, dictSMSValues);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendSubScriptionSMSToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendSubScriptionSMSToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }
        public void SendSMSToMerchant(long shopId, long orderId)
        {
            try
            {
                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();

                var shopDetails = db.Shops.Where(x => x.ID == shopId);
                var customerOrder = db.CustomerOrders.Find(orderId);
                var amt = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == orderId && x.ShopID == shopId).Sum(x => x.TotalAmount);
                //var amt = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == orderId).Sum(x=>x.TotalAmount); //g.Where(c => c.Type == "A").Sum(c => c.Qty)
                if (shopDetails.Count() > 0)
                {
                    dictSMSValues.Add("#--NAME--#", Convert.ToString(shopDetails.FirstOrDefault().ContactPerson));
                    dictSMSValues.Add("#--ORD_NUM--#", customerOrder.CustomerOrderDetails.Where(x => x.CustomerOrderID == orderId && x.ShopID == shopId).FirstOrDefault().ShopOrderCode);
                    //dictSMSValues.Add("#--ORD_NUM--#", customerOrder.OrderCode);
                    //dictSMSValues.Add("#--AMOUNT--#", customerOrder.OrderAmount.ToString());
                    dictSMSValues.Add("#--AMOUNT--#", amt.ToString());
                    dictSMSValues.Add("#--LINK--#", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "/placed");
                    dictSMSValues.Add("#--TIME--#", customerOrder.CreateDate.AddHours(2).ToShortTimeString());

                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                    ReadConfig readConfig = new ReadConfig(System.Web.HttpContext.Current.Server);
                    //gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.MER_ORD_PLCD, new string[] { shopDetails.FirstOrDefault().Mobile.ToString(), readConfig.CRM_MOBILE }, dictSMSValues);
                    gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.MER_ORD_PLCD, new string[] { shopDetails.FirstOrDefault().Mobile.ToString()}, dictSMSValues);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][M:SendSMSToMerchant]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][M:SendSMSToMerchant]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }

    }
}
