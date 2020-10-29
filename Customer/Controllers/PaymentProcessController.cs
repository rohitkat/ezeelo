
//-----------------------------------------------------------------------
// <copyright file="PaymentProcessController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Gaurav Dixit</author>
//-----------------------------------------------------------------------

//-----------------------------------------------------------------------
// <copyright file="PaymentProcessController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <modify-author>Tejaswee Taktewale</modify-author>
//----------------------------------------------------------------------- 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using ModelLayer.Models;
using CaptchaMvc.HtmlHelpers;
using System.Text;
using System.Data;
using System.Security.Cryptography;
using System.Configuration;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using CCA.Util;
using System.Collections.Specialized;
using System.Globalization;

namespace Gandhibagh.Controllers
{

    public class PaymentProcessController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        public static int lIsSubscription = -1;
        public static long lReferredByLoginID = -1;
        int lIsAlreadyExist = -1;
        public static int lIsCorporateGift = -1;
        //
        // GET: /PaymentProcess/
        public ActionResult CustomerPaymentProcess()
        {
            /*
              Indents:
            * Description: This method is used when customer wants to buy any product. This method checks wheather customer login or not.
             *              If login, then fill customer logged in details to viewModel(By Code) and return to shiiping address tab
             *              Else Return to login tab and fill customer log in details by customer.
             
            * Parameters: 
             
            * Precondition: 
            * Postcondition:
            * Logic: 1) Get customer userLoginId from session Variable
             *       2) if (Request.QueryString["checkOTP"] == null) means customer is already registered with us and return to view(NOT GUEST USER)
             *       2.1) if (userLoginID > 0) means customer is logged in and session variable is maintained(UID is maintained)
             *       2.1.1) Check isExpress Buy and store in variable
             *       2.1.2) Check Coupon is already used by customer. if yes, return to cart(to remove particular items from shopping cart).
             *       2.1.3) If all is well, set customer logged in details to view model and return to view
             *       2.1.4) set order detais(cart details) to view model and return to view
             *       2.1.5) Set all payment modes to model
             *       3) else return View and ask for login details to customer
             *          
            */

            try
            {
                if (TempData["ReturnFromUrlpurchaseComplete"] != null)
                {
                    return RedirectToRoute("Home");
                }

                if (Request.QueryString["Subscription"] != null)
                {
                    ViewBag.Subscription = "2";
                    TempData["IsSubscription"] = Request.QueryString["Subscription"];
                    TempData["IsAlreadyExist"] = Request.QueryString["IsAlreadyExist"];
                    TempData["ReferredByLoginID"] = Request.QueryString["ReferredByLoginID"];
                    TempData.Keep();
                }
                if (Request.QueryString["CorporateGift"] != null)
                {
                    ViewBag.CorporateGift = "2";
                    TempData["IsCorporateGift"] = Request.QueryString["CorporateGift"];
                    TempData.Keep();
                }
                long userLoginID = 0; // 1
                long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);

                // ViewBag.Salutation = new SelectList(db.Salutations, "ID", "Name");

                PaymentProcessViewModel paymentProcessViewModel = new PaymentProcessViewModel();

                if (Request.QueryString["checkOTP"] == null)  // 2
                {
                    if (userLoginID > 0)  // 2.1
                    {
                        bool isExpressBuy = string.IsNullOrEmpty(Request.QueryString["IsExpressBuy"]) ? false : true;  // 2.1.1

                        this.IsCouponUsedByCustomer(userLoginID, isExpressBuy); //2.1.2

                        LogonDetailsViewModel logonDetailsViewModel = this.GetCustomerInformation(userLoginID);//get customermobile and email no from loginid
                        paymentProcessViewModel.LogonDetails = logonDetailsViewModel; // 2.1.3

                        ShopProductVarientViewModelCollection cartDetail = this.OrderDetailsCart();// 2.1.4

                        paymentProcessViewModel.shoppingCartDetail = cartDetail;

                        ViewBag.ID = userLoginID;

                        paymentProcessViewModel = this.SetPaymentModes(paymentProcessViewModel);// 2.1.5
                        return View(paymentProcessViewModel);
                    }
                }

                ViewBag.ID = userLoginID;

                return View();  // 3
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][GET:CustomerPaymentProcess]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View("HttpError");
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][GET:CustomerPaymentProcess]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View("HttpError");
            }
        }

        private PaymentProcessViewModel SetPaymentModes(PaymentProcessViewModel paymentProcessViewModel)
        {
            //get payment modes
            OrderPaymentModes payModes = new OrderPaymentModes();
            ShopListViewModel shopList = new ShopListViewModel();

            ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();
            if (Request.QueryString["IsExpressBuy"] != null && Convert.ToBoolean(Request.QueryString["IsExpressBuy"]) == true)
                lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["ExpressBuyCollection"];
            else
                lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["CartCollection"];

            TempData.Keep();

            List<long> shopIDs = new List<long>();
            if (lShoppingCartCollection != null)
            {
                shopIDs = lShoppingCartCollection.lShopWiseDeliveryCharges.Select(x => x.ShopID).Distinct().ToList();
            }

            shopList.shopList = shopIDs;

            paymentProcessViewModel.PaymentModes = payModes.GetPaymentModes(shopList);

            return paymentProcessViewModel;

        }

        public ActionResult CreateShippingAddress()
        {
            try
            {
                return PartialView("_NewShippingAddress");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][GET:CreateShippingAddress]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View("HttpError");
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][GET:CreateShippingAddress]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View("HttpError");
            }
        }

        private LogonDetailsViewModel GetCustomerInformation(long userLoginID)
        {
            try
            {
                LogonDetailsViewModel logonDetailsViewModel = new LogonDetailsViewModel();
                var customerDetails = db.UserLogins.Select(x => new { x.Mobile, x.Email, x.ID }).Where(x => x.ID == userLoginID).ToList();

                if (customerDetails.Count() > 0)
                {
                    for (int i = 0; i < customerDetails.Count(); i++)
                    {
                        logonDetailsViewModel.MobileNo = customerDetails[i].Mobile;
                        logonDetailsViewModel.EmailId = customerDetails[i].Email;
                    }
                }
                return logonDetailsViewModel;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[PaymentProcessController][M:GetCustomerInformation]", "Can't get customer information !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PaymentProcessController][M:GetCustomerInformation]", "Can't get customer information !" + Environment.NewLine + ex.Message);
            }
        }

        //[HttpPost]
        //public ActionResult CustomerPaymentProcess(string SelectedAddress, string IsOnlinePayment, bool isExpressBuy, string CaptchaRefresh, string captchaCode)
        //{
        //    /*
        //      Indents:
        //    * Description: This method is used when customer place order by click on CompleteOrder button.
        //     *              This can be online payment, cash on delivery or any other payment mode.
        //     *              

        //    * Parameters: SelectedAddress: Contains Id of select shipping address, IsOnlinePayment: Check IsOnlinePayment payment,
        //     *            isExpressBuy: Check isExpressBuy, CaptchaRefresh: Check isCaptchaRefresh OR NOT, captchaCode: Contains captchaCode

        //    * Precondition: 
        //    * Postcondition:
        //    * Logic: 1) Get customer userLoginId from session Variable
        //     *       2) if (CaptchaRefresh == "0") means Customer not request for captcha refresh and wants to place order (Cash on Delivery)
        //     *       2.1) if (captchaCode == this.Session["CaptchaImageText"].ToString()) Check Entered Captcha is right or not
        //     *       2.1.1) If Ok, Place Order,
        //     *       2.1.1.1) If Result>0 (Order Placed Successfully)  return orderId and send SMS,EMAIL to customer/Merchant
        //     *       2.1.1.2) Show message Problem in placing order and return
        //     *       2.2) Show message Entered captcha is wrong and return     
        //    */


        //    try
        //    {
        //        long userLoginID = 0;
        //        long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);

        //        ViewBag.ID = userLoginID;

        //        PaymentProcessViewModel paymentProcessViewModel = new PaymentProcessViewModel();
        //        LogonDetailsViewModel logonDetailsViewModel = this.GetCustomerInformation(userLoginID);
        //        paymentProcessViewModel.LogonDetails = logonDetailsViewModel;
        //        paymentProcessViewModel = this.SetPaymentModes(paymentProcessViewModel);

        //        if (IsOnlinePayment == "1")
        //        {
        //            // Manoj
        //            this.GetUserInfo(userLoginID, SelectedAddress, isExpressBuy);
        //            //this.PlaceOrder(userLoginID, SelectedAddress, isExpressBuy);
        //        }

        //        if (CaptchaRefresh == "0")
        //        {
        //            // Code for validating the CAPTCHA  
        //            if (captchaCode == this.Session["CaptchaImageText"].ToString())
        //            {
        //                //if (this.IsCaptchaValid("Captcha is not valid"))
        //                //{
        //                long result = this.PlaceOrder(userLoginID, SelectedAddress, isExpressBuy);

        //                // if order placed successfully
        //                if (result > 0)
        //                {
        //                    BusinessLogicLayer.OrderPlacedSmsAndEmail orderPlaced = new OrderPlacedSmsAndEmail(System.Web.HttpContext.Current.Server);
        //                    orderPlaced.SendMailToCustomer(userLoginID, result);
        //                    orderPlaced.SendMailToMerchant(userLoginID, result);
        //                    orderPlaced.SendSMSToCustomer(userLoginID, result);

        //                    ShoppingCartInitialization s = new ShoppingCartInitialization();

        //                    s.DeleteShoppingCartCookie();
        //                    s.DeleteCouponCookie();

        //                    TempData["OrderMessage"] = "Order Placed Successfully";
        //                    return RedirectToAction("PurchaseComplete", "CustomerOrder", new { orderID = result });
        //                    //return RedirectToAction("Test");
        //                }
        //                else
        //                {
        //                    ViewBag.CaptchaWrong = 1;
        //                    ViewBag.ErrorMessage = "Problem in placing order!!";
        //                    return View(paymentProcessViewModel);
        //                }
        //            }
        //            else
        //            {
        //                ViewBag.CaptchaWrong = 1;
        //                ViewBag.ErrorMessage = "Error: captcha is not valid.";
        //                return View(paymentProcessViewModel);
        //            }
        //            //}
        //        }

        //        ViewBag.CaptchaWrong = 1;
        //        return View(paymentProcessViewModel);
        //    }
        //    catch (BusinessLogicLayer.MyException myEx)
        //    {
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
        //            + "[PaymentProcessController][POST:CustomerPaymentProcess]" + myEx.EXCEPTION_PATH,
        //            BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

        //        return View("HttpError");
        //    }
        //    catch (Exception ex)
        //    {
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[PaymentProcessController][POST:CustomerPaymentProcess]",
        //            BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

        //        return View("HttpError");
        //    }
        //}

        [HttpPost]
        //this function is added from new eZeelo version
        public ActionResult CustomerPaymentProcess(string SelectedAddress, string IsOnlinePayment, bool isExpressBuy, string CaptchaRefresh, string captchaCode)
        {
            /*
              Indents:
            * Description: This method is used when customer place order by click on CompleteOrder button.
             *              This can be online payment, cash on delivery or any other payment mode.
             *              
             
            * Parameters: SelectedAddress: Contains Id of select shipping address, IsOnlinePayment: Check IsOnlinePayment payment,
             *            isExpressBuy: Check isExpressBuy, CaptchaRefresh: Check isCaptchaRefresh OR NOT, captchaCode: Contains captchaCode
             
            * Precondition: 
            * Postcondition:
            * Logic: 1) Get customer userLoginId from session Variable
             *       2) if (CaptchaRefresh == "0") means Customer not request for captcha refresh and wants to place order (Cash on Delivery)
             *       2.1) if (captchaCode == this.Session["CaptchaImageText"].ToString()) Check Entered Captcha is right or not
             *       2.1.1) If Ok, Place Order,
             *       2.1.1.1) If Result>0 (Order Placed Successfully)  return orderId and send SMS,EMAIL to customer/Merchant
             *       2.1.1.2) Show message Problem in placing order and return
             *       2.2) Show message Entered captcha is wrong and return     
            */


            try
            {
                //Yashaswi Start 04-02-2019 for BUG 11 to Verify cart 
                int resultVerifyCart = (new ShoppingCartInitialization()).VerifyCartOnOrderPLace();
                if (resultVerifyCart == 1)
                {
                    return RedirectToAction("Index", "ShoppingCart");
                }
                else
                {
                    //Yashaswi END 04-02-2019 for BUG 11 to Verify cart 

                    long userLoginID = 0;
                    long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);

                    // string delSchedID = string.Empty;

                    ViewBag.ID = userLoginID;
                    ViewBag.SelectedAddress = SelectedAddress;
                    PaymentProcessViewModel paymentProcessViewModel = new PaymentProcessViewModel();
                    LogonDetailsViewModel logonDetailsViewModel = this.GetCustomerInformation(userLoginID);
                    paymentProcessViewModel.LogonDetails = logonDetailsViewModel;
                    paymentProcessViewModel = this.SetPaymentModes(paymentProcessViewModel);
                    ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();

                    //Check for coupon code is valid at the time of order completion.
                    if (Session["OrderCouponCode"] != null && Session["OrderCouponAmount"] != null)
                    {
                        //ShoppingCartInitialization sci = new ShoppingCartInitialization();
                        string userMessage = string.Empty;
                        int validityCode = 0;
                        CouponManagement obj = new CouponManagement();
                        long cityId = Convert.ToInt64(Request.Cookies["CityCookie"].Value.Split('$')[0]);
                        int franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added
                        if (isExpressBuy == true)
                        {
                            lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["ExpressBuyCollection"];
                            // lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount = Convert.ToDecimal(obj.VerifyCouponAgainstCart(lShoppingCartCollection.lShopWiseDeliveryCharges, Session["OrderCouponCode"].ToString(), out userMessage, out validityCode, userLoginID, cityId));////hide
                            lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount = Convert.ToDecimal(obj.VerifyCouponAgainstCart(lShoppingCartCollection.lShopWiseDeliveryCharges, Session["OrderCouponCode"].ToString(), out userMessage, out validityCode, userLoginID, cityId, franchiseId));////added franchiseId
                            lShoppingCartCollection.lShoppingCartOrderDetails.CoupenCode = Session["OrderCouponCode"].ToString();

                            TempData["ExpressBuyCollection"] = lShoppingCartCollection;
                        }
                        else
                        {
                            lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["CartCollection"];
                            //lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount = Convert.ToDecimal(obj.VerifyCouponAgainstCart(lShoppingCartCollection.lShopWiseDeliveryCharges, Session["OrderCouponCode"].ToString(), out userMessage, out validityCode, userLoginID, cityId));////hide
                            lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount = Convert.ToDecimal(obj.VerifyCouponAgainstCart(lShoppingCartCollection.lShopWiseDeliveryCharges, Session["OrderCouponCode"].ToString(), out userMessage, out validityCode, userLoginID, cityId, franchiseId));////added franchiseId
                            lShoppingCartCollection.lShoppingCartOrderDetails.CoupenCode = Session["OrderCouponCode"].ToString();
                            TempData["CartCollection"] = lShoppingCartCollection;
                        }
                        //delSchedID = lShoppingCartCollection.DeliveryScheduleID;

                        TempData.Keep();

                        if (validityCode != 1 && Session["OrderCouponCode"] != null && Session["OrderCouponCode"].ToString() != string.Empty)
                        {
                            TempData["CouponMessage"] = userMessage;
                            return RedirectToAction("Index", "ShoppingCart", lShoppingCartCollection);
                        }


                    }
                    //1 for payumoney and 2 for ccavenue
                    if (IsOnlinePayment == "1" || IsOnlinePayment == "2")
                    {
                        // Manoj
                        this.GetUserInfo(userLoginID, SelectedAddress, isExpressBuy, IsOnlinePayment);
                        //this.PlaceOrder(userLoginID, SelectedAddress, isExpressBuy);
                    }
                    var crty = Convert.ToInt64(Request.Cookies["CartID"].Value);
                    Session["crty"] = crty;
                    if (CaptchaRefresh == "0")
                    {
                        // Code for validating the CAPTCHA  
                        //Start Yashaswi 16-7-2018 Hide Catpcha
                        //if (captchaCode == this.Session["CaptchaImageText"].ToString())
                        //{
                        //End Yashaswi 16-7-2018 Hide Catpcha

                        //if (this.IsCaptchaValid("Captcha is not valid"))
                        //{
                        //long result = this.PlaceOrder(userLoginID, SelectedAddress, isExpressBuy, IsOnlinePayment, out delSchedID);
                        long result = this.PlaceOrder(userLoginID, SelectedAddress, isExpressBuy, IsOnlinePayment);

                        // if order placed successfully
                        if (result > 0)
                        {
                            //====================== Save Delivery schedule detail in database =====================
                            //if (isExpressBuy != true)
                            //{
                            //SaveDeliveryScheduleDetail(userLoginID, result, delSchedID);
                            //}



                            BusinessLogicLayer.OrderPlacedSmsAndEmail orderPlaced = new OrderPlacedSmsAndEmail(System.Web.HttpContext.Current.Server);
                            //try
                            //{
                            //    orderPlaced.SendSMSToMerchant(userLoginID, result);
                            //}
                            //catch (Exception ex)
                            //{

                            //    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                            //    + Environment.NewLine + ex.Message + Environment.NewLine
                            //    + "[PaymentProcessController][POST:CustomerPaymentProcess]",
                            //    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                            //}
                            try
                            {
                                orderPlaced.SendSMSToCustomer(userLoginID, result);
                                (new SendFCMNotification()).SendNotification("placed", result); //Yashaswi 2-7-2019
                            }
                            catch (Exception ex)
                            {

                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + ex.Message + Environment.NewLine
                                + "[PaymentProcessController][POST:CustomerPaymentProcess]",
                                BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                            }
                            try
                            {

                                //Send email and sms to merchant    
                                orderPlaced.SendMailToMerchant(userLoginID, result);
                            }
                            catch (Exception ex)
                            {

                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + ex.Message + Environment.NewLine
                                + "[PaymentProcessController][POST:CustomerPaymentProcess]",
                                BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                            }
                            try
                            {
                                orderPlaced.SendMailToCustomer(userLoginID, result);
                            }
                            catch (Exception ex)
                            {

                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + ex.Message + Environment.NewLine
                                + "[PaymentProcessController][POST:CustomerPaymentProcess]",
                                BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                            }

                            ShoppingCartInitialization s = new ShoppingCartInitialization();

                            s.DeleteShoppingCartCookie();
                            s.DeleteCouponCookie();

                            TempData["OrderMessage"] = "Order Placed Successfully";
                            //return RedirectToAction("PurchaseComplete", "CustomerOrder", new { orderID = result });
                            string cityName = "";
                            int franchiseID = 0;////added
                            if (Request.Cookies["CityCookie"] != null && Request.Cookies["CityCookie"].Value != "")
                            {
                                cityName = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                                franchiseID = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added
                            }
                            //- Change made by Avi Verma and Pradnyakar Sir. Date :- 02-July-2016.
                            //- Reason :- SEO PPC required static URL for getting results.
                            //- As said by Reena ma'am, Bhavan and Bhusan.

                            //return RedirectToRoute("PurchaseComplete", new { orderID = result, city = cityName });

                            TempData["orderID"] = result;
                            // return RedirectToRoute("PurchaseComplete", new { city = cityName });////hided
                            return RedirectToRoute("PurchaseComplete", new { city = cityName, franchiseId = franchiseID });////added  franchiseId = franchiseID

                            Session["OrderCouponCode"] = null;
                            Session["OrderCouponAmount"] = null;
                            //return RedirectToAction("Test");
                        }
                        else
                        {
                            ViewBag.CaptchaWrong = 1;
                            ViewBag.ErrorMessage = "Problem in placing order!!";
                            return View(paymentProcessViewModel);
                        }
                        //Start Yashaswi 16-7-2018 Hide Catpcha
                        //}
                        //else
                        //{
                        //    ViewBag.CaptchaWrong = 1;
                        //    ViewBag.ErrorMessage = "Error: captcha is not valid.";
                        //    return View(paymentProcessViewModel);
                        //}
                        //}
                    }
                    else
                    {
                        ViewBag.CaptchaWrong = 1;
                        //if (isExpressBuy == true)
                        //{
                        //    lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["ExpressBuyCollection"];
                        //    TempData["ExpressBuyCollection"] = lShoppingCartCollection;
                        //}
                        //else
                        //{
                        //    lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["CartCollection"];
                        //    TempData["CartCollection"] = lShoppingCartCollection;
                        //}
                    }

                    return View(paymentProcessViewModel);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][POST:CustomerPaymentProcess]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View("HttpError");
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][POST:CustomerPaymentProcess]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View("HttpError");
            }
        }

        //public long PlaceOrder(long userLoginID, string SelectedAddress, bool IsExpressBuy, string IsOnlinePayment, out string delSchedID)
        public long PlaceOrder(long userLoginID, string SelectedAddress, bool IsExpressBuy, string IsOnlinePayment)
        {

            // Shopping cart list
            long result = 0;
            try
            {
                ShoppingCartInitialization sci = new ShoppingCartInitialization();

                ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();

                //if (TempData["IsCorporateGift"] != null && TempData["CorporateGiftCartDetail"] != null && TempData["CorporateAddressDetail"] != null)
                //{

                //   long CustOrderID= this.PlaceCorporateOrder(userLoginID);
                //   return CustOrderID;


                //}
                if (IsExpressBuy == true)
                    lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["ExpressBuyCollection"];
                else
                    lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["CartCollection"];

                // delSchedID = lShoppingCartCollection.DeliveryScheduleID;


                // List of shipping address
                var listOfAddress = (List<CustomerShippingAddress>)TempData["ShippingAddress"];
                if (listOfAddress != null && lShoppingCartCollection != null)
                {
                    // Find selected shipping address
                    var shippingAddress = listOfAddress.Where(x => x.ID == Convert.ToInt32(SelectedAddress)).ToList().FirstOrDefault();

                    CustomerDetails customerDetails = new CustomerDetails(System.Web.HttpContext.Current.Server);

                    // Get customer detail
                    // CustomerDetailsViewModel customerDetailsViewModel = customerDetails.GetCustomerDetails(userLoginID);

                    OrderViewModel orderViewModel = new OrderViewModel();

                    ModelLayer.Models.CustomerOrder modelCustomerOrder = new ModelLayer.Models.CustomerOrder();

                    // Set value to customer order
                    modelCustomerOrder.ID = 0;
                    modelCustomerOrder.OrderCode = string.Empty;
                    modelCustomerOrder.UserLoginID = userLoginID;
                    modelCustomerOrder.OrderAmount = lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount;
                    modelCustomerOrder.NoOfPointUsed = lShoppingCartCollection.lShoppingCartOrderDetails.NoOfPointUsed;
                    modelCustomerOrder.ValuePerPoint = lShoppingCartCollection.lShoppingCartOrderDetails.ValuePerPoint;

                    //modelCustomerOrder.CoupenCode = lShoppingCartCollection. lShopProductVarientViewModel.FirstOrDefault().CouponCode;
                    //modelCustomerOrder.CoupenAmount = 0;
                    //if (lShoppingCartCollection.lShopProductVarientViewModel.FirstOrDefault().CouponValueRs > 0)
                    //    modelCustomerOrder.CoupenAmount = lShoppingCartCollection.lShopProductVarientViewModel.FirstOrDefault().CouponValueRs;
                    //else if (lShoppingCartCollection.lShopProductVarientViewModel.FirstOrDefault().CouponValuePercent > 0)
                    //{
                    //    decimal percent = 0;
                    //    decimal.TryParse(Convert.ToString(lShoppingCartCollection.lShopProductVarientViewModel.FirstOrDefault().CouponValuePercent), out percent);

                    //    decimal decimalAmount = (Convert.ToDecimal(lShoppingCartCollection.lShopProductVarientViewModel.FirstOrDefault().MRP) * percent) / 100;

                    //    modelCustomerOrder.CoupenAmount = decimalAmount;
                    //}
                    modelCustomerOrder.CoupenCode = lShoppingCartCollection.lShoppingCartOrderDetails.CoupenCode;
                    modelCustomerOrder.CoupenAmount = lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount;

                    modelCustomerOrder.PAN = string.Empty;

                    // for payment option
                    if (IsOnlinePayment == "1")
                    {
                        modelCustomerOrder.PaymentMode = "ONLINE";
                    }
                    else
                    {
                        modelCustomerOrder.PaymentMode = "COD";
                    }
                    modelCustomerOrder.CashbackPointsTotal = lShoppingCartCollection.lShoppingCartOrderDetails.CashbackPointsTotal;
                    modelCustomerOrder.PayableAmount = lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount;// -lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount;
                    //modelCustomerOrder.PayableAmount = lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount - 
                    //    lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount + 
                    //    lShoppingCartCollection.lShopWiseDeliveryCharges.FirstOrDefault().DeliveryCharge+
                    //    lShoppingCartCollection;
                    modelCustomerOrder.PrimaryMobile = shippingAddress.PrimaryMobile;
                    modelCustomerOrder.SecondoryMobile = shippingAddress.SecondaryMobile;
                    modelCustomerOrder.ShippingAddress = shippingAddress.ShippingAddress;
                    modelCustomerOrder.PincodeID = shippingAddress.PincodeID;
                    modelCustomerOrder.AreaID = shippingAddress.AreaID;
                    modelCustomerOrder.BusinessPointsTotal = lShoppingCartCollection.lShoppingCartOrderDetails.BusinessPointsTotal; //Added by Zubair for MLM on 05-01-2018
                    modelCustomerOrder.MLMAmountUsed = lShoppingCartCollection.lShoppingCartOrderDetails.WalletAmountUsed; //Added by Zubair for MLM on 25-01-2018

                    //Check at current MLMAmountUsed is availabel in wallet or not
                    if (modelCustomerOrder.MLMAmountUsed > 0)
                    {
                        decimal TotalWalAmt = 0;
                        MLMWallet wallet = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == modelCustomerOrder.UserLoginID);
                        if (wallet != null)
                        {
                            TotalWalAmt = wallet.Amount;
                        }
                        CashbackWallet wallet_ = db.cashbackWallets.FirstOrDefault(p => p.UserLoginID == modelCustomerOrder.UserLoginID);
                        if (wallet_ != null)
                        {
                            TotalWalAmt = TotalWalAmt + wallet_.Amount;
                        }
                        if (TotalWalAmt < modelCustomerOrder.MLMAmountUsed)
                        {
                            decimal remainingAmt = 0;
                            remainingAmt = modelCustomerOrder.MLMAmountUsed.Value - TotalWalAmt;
                            if (remainingAmt > 0)
                            {
                                modelCustomerOrder.MLMAmountUsed = TotalWalAmt;
                                modelCustomerOrder.PayableAmount = modelCustomerOrder.PayableAmount + remainingAmt;
                            }
                        }
                    }
                    //End
                    modelCustomerOrder.CreateDate = DateTime.UtcNow;
                    modelCustomerOrder.CreateBy = userLoginID;
                    modelCustomerOrder.ModifyDate = null;
                    modelCustomerOrder.ModifyBy = null;
                    modelCustomerOrder.NetworkIP = CommonFunctions.GetClientIP();
                    modelCustomerOrder.DeviceType = "x";
                    modelCustomerOrder.DeviceID = "x";

                    orderViewModel.CustomerOrder = modelCustomerOrder;

                    List<ShoppingCartViewModel> listShoppingCart = lShoppingCartCollection.lShopProductVarientViewModel;

                    // List of order details

                    List<CustomerOrderDetail> listOrderDetails = new List<CustomerOrderDetail>();
                    //==============Tejaswee for taxation ===========
                    List<TaxOnOrder> prodTaxOnOrder = new List<TaxOnOrder>();
                    TaxOnOrder lTaxOnOrder = new TaxOnOrder();
                    List<CalulatedTaxesRecord> listCalTaxRec = new List<CalulatedTaxesRecord>();
                    //==============Tejaswee for taxation ===========
                    foreach (var item in listShoppingCart)
                    {
                        CustomerOrderDetail customerOrderDetail = new CustomerOrderDetail();

                        customerOrderDetail.ID = 0;
                        customerOrderDetail.ShopOrderCode = string.Empty;
                        customerOrderDetail.CustomerOrderID = 0;
                        customerOrderDetail.ShopStockID = item.ShopStockID;
                        customerOrderDetail.WarehouseStockID = item.WarehouseStockID; //Added by Zubair for Inventory on 28-03-2018
                        customerOrderDetail.ShopID = item.ShopID;
                        customerOrderDetail.Qty = item.PurchaseQuantity;
                        if (IsOnlinePayment == "1" || IsOnlinePayment == "2")
                        {
                            customerOrderDetail.OrderStatus = 0;
                        }
                        else
                        {
                            customerOrderDetail.OrderStatus = 1;
                        }
                        customerOrderDetail.MRP = item.MRP;
                        customerOrderDetail.SaleRate = item.SaleRate;
                        customerOrderDetail.OfferPercent = 0;
                        customerOrderDetail.OfferRs = 0;
                        customerOrderDetail.IsInclusivOfTax = true;
                        //customerOrderDetail.TotalAmount = 10000;
                        customerOrderDetail.TotalAmount = item.SaleRate * item.PurchaseQuantity;
                        customerOrderDetail.BusinessPointPerUnit = item.BusinessPointPerUnit; //Added by Zubair for MLM on 05-01-2018
                        customerOrderDetail.BusinessPoints = item.BusinessPointPerUnit * item.PurchaseQuantity; //Added by Zubair for MLM on 05-01-2018
                        customerOrderDetail.CashbackPointPerUnit = item.CashbackPoitPerUnit;// Added by Yashaswi 3-10-2019
                        customerOrderDetail.CashbackPoints = item.CashbackPoitPerUnit * item.PurchaseQuantity;// Added by Yashaswi 3-10-2019
                        customerOrderDetail.IsActive = true;
                        orderViewModel.CustomerOrder.CreateDate = DateTime.UtcNow;
                        orderViewModel.CustomerOrder.CreateBy = userLoginID;
                        orderViewModel.CustomerOrder.ModifyDate = null;
                        orderViewModel.CustomerOrder.ModifyBy = null;
                        orderViewModel.CustomerOrder.NetworkIP = CommonFunctions.GetClientIP();
                        orderViewModel.CustomerOrder.DeviceType = "x";
                        orderViewModel.CustomerOrder.DeviceID = "x";

                        listOrderDetails.Add(customerOrderDetail);

                        //==============Tejaswee for taxation ===========
                        //foreach (var taxDetail in item.lCalulatedTaxesRecord)
                        //{
                        //    lTaxOnOrder.Amount = taxDetail.TaxableAmount;
                        //    lTaxOnOrder.CustomerOrderDetailID = 0;
                        //    lTaxOnOrder.DeviceID = "X";
                        //    lTaxOnOrder.DeviceType = "X";
                        //    lTaxOnOrder.CreateDate = DateTime.UtcNow;
                        //    lTaxOnOrder.CreateBy = userLoginID;
                        //    lTaxOnOrder.ModifyBy = null;
                        //    lTaxOnOrder.ModifyDate = null;
                        //    lTaxOnOrder.NetworkIP = CommonFunctions.GetClientIP();
                        //    lTaxOnOrder.ProductTaxID = taxDetail.ProductTaxID;
                        //    prodTaxOnOrder.Add(lTaxOnOrder);
                        //}
                        if (item.lCalulatedTaxesRecord != null)
                        {
                            listCalTaxRec.AddRange(item.lCalulatedTaxesRecord);
                        }
                        // orderViewModel.lCalulatedTaxesRecord.AddRange(item.lCalulatedTaxesRecord);
                        //customerOrderDetail.TaxOnOrders = prodTaxOnOrder;
                        //==============Tejaswee for taxation ===========
                    }

                    orderViewModel.CustomerOrderDetail = listOrderDetails;
                    orderViewModel.lCalulatedTaxesRecord = listCalTaxRec;
                    //==============Tejaswee for taxation ===========
                    //orderViewModel.lTaxOnOrder = prodTaxOnOrder;

                    //==============Tejaswee for taxation ===========

                    // List of shipping charges

                    List<ShopWiseDeliveryCharges> listShopWiseDeliveryCharges = new List<ShopWiseDeliveryCharges>();

                    //orderViewModel.shopWiseDeliveryCharges = listShopWiseDeliveryCharges;
                    orderViewModel.shopWiseDeliveryCharges = lShoppingCartCollection.lShopWiseDeliveryCharges;
                    // Place customer order

                    BusinessLogicLayer.CustomerOrder customerOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);

                    if (lShoppingCartCollection.DeliveryScheduleID != null && lShoppingCartCollection.DeliveryScheduleID != string.Empty)
                    {
                        orderViewModel.ScheduleID = Convert.ToInt32(lShoppingCartCollection.DeliveryScheduleID.Split('$')[0]);
                        orderViewModel.ScheduleDate = Convert.ToDateTime(lShoppingCartCollection.DeliveryScheduleID.Split('$')[1]);
                    }

                    /*========== Save used amount from GB vallet ============*/
                    if (lShoppingCartCollection.lShoppingCartOrderDetails.UsedEarnAmount > 0)
                    {
                        EarnDetail lED = new EarnDetail();

                        lED.EarnAmount = lShoppingCartCollection.lShoppingCartOrderDetails.EarnAmount;
                        lED.RemainingAmount = lShoppingCartCollection.lShoppingCartOrderDetails.RemainEarnAmt;
                        lED.UsedAmount = lShoppingCartCollection.lShoppingCartOrderDetails.UsedEarnAmount;
                        lED.EarnUID = userLoginID;
                        orderViewModel.lEarnDetail = lED;

                    }
                    orderViewModel.IsBoostPlan = lShoppingCartCollection.IsBoosterPlan;
                    result = customerOrder.PlaceCustomerOrder(orderViewModel);

                    //This method check order wise earn if order wise earn is applicable then insert earn amount in respected table 
                    ReferAndEarn lReferAndEarn = new ReferAndEarn();
                    lReferAndEarn.GetReferAndEarnDetails(userLoginID, lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount, result);

                    //Added by Zubair for MLM on 26-02-2018
                    //Call OnOrderPlace method 
                    //if (Convert.ToDecimal(modelCustomerOrder.MLMAmountUsed) > 0 && result>0)  //|| modelCustomerOrder.BusinessPointsTotal > 0
                    //{
                    //    MLMWalletPoints objMLMWalletPoints = new MLMWalletPoints();
                    //    object ret = objMLMWalletPoints.MLMWalletPostRequest(1, userLoginID, result, modelCustomerOrder.BusinessPointsTotal, modelCustomerOrder.PayableAmount, DateTime.UtcNow, Convert.ToDecimal(modelCustomerOrder.MLMAmountUsed));
                    //}
                    TempData["OrderDetail"] = orderViewModel;

                    //Insert Subscription detail
                    this.InsertSubscriptionDetail(result);

                    if (IsExpressBuy == true)
                    {
                        long UserID = Convert.ToInt64(Session["UID"]);
                        string MobileNo = db.UserLogins.Where(x => x.ID == UserID).Select(x => x.Mobile).FirstOrDefault();
                        if (ControllerContext.HttpContext.Request.Cookies["ExpressBuyCookie"] != null && ControllerContext.HttpContext.Request.Cookies["ExpressBuyCookie"].Value != string.Empty)
                        {
                            string val = ControllerContext.HttpContext.Request.Cookies["ExpressBuyCookie"].Value;
                            string[] cookieVal = val.Split(',');
                            foreach (string item in cookieVal)
                            {
                                if (item != string.Empty)
                                {
                                    string[] indivItmDet = item.Split('$');
                                    if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
                                    {
                                        string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                                        Nullable<long> lCartID = null;
                                        if (ControllerContext.HttpContext.Request.Cookies["CartID"] != null)
                                        {
                                            lCartID = Convert.ToInt64(ControllerContext.HttpContext.Request.Cookies["CartID"].Value.ToString());
                                        }
                                        //TrackCartBusiness.InsertCartDetails(lCartID, null, UserID, Convert.ToInt64(indivItmDet[0]), MobileNo, "CHECK_OUT", "", "", "", "", cookieValue.Split('$')[1], "");
                                        //- For Manoj
                                        // TrackCartBusiness.InsertCartDetails(UserID, Convert.ToInt64(indivItmDet[0]), MobileNo, "CHECK_OUT", "", "", "", "", cookieValue.Split('$')[1], "");////hide
                                        int qty = indivItmDet[2] != null ? Convert.ToInt32(indivItmDet[2]) : 0;
                                        TrackCartBusiness.InsertCartDetails(lCartID, qty, UserID, Convert.ToInt64(indivItmDet[0]), MobileNo, "CHECK_OUT", "", "", "", "", cookieValue.Split('$')[1], "", Convert.ToInt32(cookieValue.Split('$')[2]));//--added by Ashish for multiple franchise in same city--//
                                    }

                                }
                            }


                        }
                    }
                    else
                    {
                        long UserID = Convert.ToInt64(Session["UID"]);
                        string MobileNo = db.UserLogins.Where(x => x.ID == UserID).Select(x => x.Mobile).FirstOrDefault();
                        TrackCartBusiness lTrackCart = new TrackCartBusiness();

                        if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
                        {
                            string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                            Nullable<long> lCartID = null;
                            if (ControllerContext.HttpContext.Request.Cookies["CartID"] != null)
                            {
                                lCartID = Convert.ToInt64(ControllerContext.HttpContext.Request.Cookies["CartID"].Value.ToString());
                            }
                            //lTrackCart.SaveDetailOnPaymentProcess(lCartID, null, UserID, MobileNo, "CHECK_OUT", cookieValue.Split('$')[1]);
                            //- For Manoj
                            // lTrackCart.SaveDetailOnPaymentProcess(UserID, MobileNo, "CHECK_OUT", cookieValue.Split('$')[1]);////hide
                            lTrackCart.SaveDetailOnPaymentProcess(lCartID, null, UserID, MobileNo, "CHECK_OUT", cookieValue.Split('$')[1], Convert.ToInt32(cookieValue.Split('$')[2]));//--added by Ashish for multiple franchise in same city--//
                        }

                    }

                }
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[PaymentProcessController][M:PlaceOrder]", "Can't place order !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PaymentProcessController][M:PlaceOrder]", "Can't place order !" + Environment.NewLine + ex.Message);
            }
            return result;
        }


        public ActionResult Logout(string ID)
        {
            try
            {
                Session["UID"] = null;
                Session["UserName"] = null;
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][GET:Logout]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][GET:Logout]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            return RedirectToAction("CustomerPaymentProcess");
        }

        private void IsCouponUsedByCustomer(long userLoginId, bool IsExpressBuy)
        {
            ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();

            if (IsExpressBuy == true)
                lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["ExpressBuyCollection"];
            else
                lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["CartCollection"];

            TempData.Keep();

            if (lShoppingCartCollection != null)
            {
                if (ControllerContext.HttpContext.Request.Cookies["CouponManagementCookie"] != null && ControllerContext.HttpContext.Request.Cookies["CouponManagementCookie"].Value != string.Empty)
                {
                    string[] cookieDetails = ControllerContext.HttpContext.Request.Cookies["CouponManagementCookie"].Value.Split('$');

                    foreach (var item in lShoppingCartCollection.lShopProductVarientViewModel)
                    {
                        if (Convert.ToInt64(cookieDetails[0]) == item.ShopStockID)
                        {
                            BusinessLogicLayer.CouponManagement coupon = new BusinessLogicLayer.CouponManagement();
                            long cityId = Convert.ToInt64(Request.Cookies["CityCookie"].Value.Split('$')[0]);
                            int franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city
                            DataTable dt = coupon.CheckCouponCode(cookieDetails[1], item.ShopID, item.ProductID, CommonFunctions.GetPersonalDetailsID(userLoginId), cityId, franchiseId);////added cityId->franchiseId old by Ashish for multiple MCO in same city
                            if (dt.Rows.Count > 0)
                            {
                                if (Convert.ToString(dt.Rows[0]["VALIDITY CODE"]) == "8")
                                {
                                    TempData["CouponMessage"] = "Coupon is already used. Please remove items from shopping cart";

                                    //HttpCookie CouponManagementCookie = new HttpCookie("CouponManagementCookie");
                                    ////Delete whole cookie
                                    //if (ControllerContext.HttpContext.Request.Cookies["CouponManagementCookie"] != null)
                                    //{
                                    //    CouponManagementCookie.Expires = DateTime.Now.AddDays(-1);
                                    //    ControllerContext.HttpContext.Response.Cookies.Add(CouponManagementCookie);
                                    //}
                                    //if (CouponManagementCookie.Expires < DateTime.Now)
                                    //{
                                    //    ControllerContext.HttpContext.Request.Cookies.Remove("CouponManagementCookie");
                                    //}

                                    Response.Redirect("~/ShoppingCart/Index");
                                    //RedirectToAction("Index", "ShoppingCart");
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GetUserInfo(long userLoginID, string SelectedAddress, bool IsExpressBuy, string IsOnlinePayment)
        {
            try
            {
                if (TempData["IsSubscription"] != null)
                {
                    if (int.TryParse(TempData["IsSubscription"].ToString(), out lIsSubscription)) { }
                }
                if (TempData["IsAlreadyExist"] != null)
                {
                    if (int.TryParse(TempData["IsAlreadyExist"].ToString(), out lIsAlreadyExist)) { }
                }
                if (TempData["ReferredByLoginID"] != null)
                {
                    if (long.TryParse(TempData["ReferredByLoginID"].ToString(), out lReferredByLoginID)) { }
                }

                if (TempData["IsCorporateGift"] != null)
                {
                    if (int.TryParse(TempData["IsCorporateGift"].ToString(), out lIsCorporateGift)) { }
                }

                ShoppingCartInitialization sci = new ShoppingCartInitialization();

                ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();

                if (IsExpressBuy == true)
                    lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["ExpressBuyCollection"];
                else
                    lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["CartCollection"];

                TempData.Keep();

                string txnid = Generatetxnid();
                // List of shipping address
                var listOfAddress = (List<CustomerShippingAddress>)TempData["ShippingAddress"];
                if (listOfAddress != null && lShoppingCartCollection != null)
                {
                    // Find selected shipping address
                    var shippingAddress = listOfAddress.Where(x => x.ID == Convert.ToInt32(SelectedAddress)).ToList().FirstOrDefault();

                    CustomerDetails customerDetails = new CustomerDetails(System.Web.HttpContext.Current.Server);

                    // Get customer detail
                    // CustomerDetailsViewModel customerDetailsViewModel = customerDetails.GetCustomerDetails(userLoginID);

                    OrderViewModel orderViewModel = new OrderViewModel();

                    ModelLayer.Models.CustomerOrder modelCustomerOrder = new ModelLayer.Models.CustomerOrder();

                    // Set value to customer order

                    modelCustomerOrder.ID = 0;
                    modelCustomerOrder.OrderCode = string.Empty;
                    modelCustomerOrder.UserLoginID = userLoginID;
                    //modelCustomerOrder.OrderAmount = 10000;
                    modelCustomerOrder.OrderAmount = lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount;
                    //modelCustomerOrder.NoOfPointUsed = 0;
                    modelCustomerOrder.NoOfPointUsed = lShoppingCartCollection.lShoppingCartOrderDetails.NoOfPointUsed;
                    //modelCustomerOrder.ValuePerPoint = 0;
                    modelCustomerOrder.ValuePerPoint = lShoppingCartCollection.lShoppingCartOrderDetails.ValuePerPoint;
                    //modelCustomerOrder.CoupenCode = string.Empty;
                    modelCustomerOrder.CoupenCode = lShoppingCartCollection.lShoppingCartOrderDetails.CoupenCode;
                    //modelCustomerOrder.CoupenAmount = 0;
                    modelCustomerOrder.CoupenAmount = lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount;
                    modelCustomerOrder.PAN = string.Empty;
                    modelCustomerOrder.PaymentMode = "ONLINE"; //If payment option is online then this method will call
                    //modelCustomerOrder.PayableAmount = 10000;
                    modelCustomerOrder.PayableAmount = lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount;// -lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount; 
                    modelCustomerOrder.PrimaryMobile = shippingAddress.PrimaryMobile;
                    modelCustomerOrder.SecondoryMobile = shippingAddress.SecondaryMobile;
                    modelCustomerOrder.ShippingAddress = shippingAddress.ShippingAddress;
                    modelCustomerOrder.PincodeID = shippingAddress.PincodeID;
                    modelCustomerOrder.AreaID = shippingAddress.AreaID;
                    modelCustomerOrder.BusinessPointsTotal = lShoppingCartCollection.lShoppingCartOrderDetails.BusinessPointsTotal; //Added by Zubair for MLM on 05-01-2018
                    modelCustomerOrder.MLMAmountUsed = lShoppingCartCollection.lShoppingCartOrderDetails.WalletAmountUsed; //Added by Zubair for MLM on 25-01-2018
                    modelCustomerOrder.CreateDate = DateTime.UtcNow;
                    modelCustomerOrder.CreateBy = userLoginID;
                    modelCustomerOrder.ModifyDate = null;
                    modelCustomerOrder.ModifyBy = null;
                    modelCustomerOrder.NetworkIP = CommonFunctions.GetClientIP();
                    modelCustomerOrder.DeviceType = "x";
                    modelCustomerOrder.DeviceID = "x";

                    orderViewModel.CustomerOrder = modelCustomerOrder;

                    List<ShoppingCartViewModel> listShoppingCart = lShoppingCartCollection.lShopProductVarientViewModel;

                    // List of order details

                    List<CustomerOrderDetail> listOrderDetails = new List<CustomerOrderDetail>();

                    foreach (var item in listShoppingCart)
                    {
                        CustomerOrderDetail customerOrderDetail = new CustomerOrderDetail();

                        customerOrderDetail.ID = 0;
                        customerOrderDetail.ShopOrderCode = string.Empty;
                        customerOrderDetail.CustomerOrderID = 0;
                        customerOrderDetail.ShopStockID = item.ShopStockID;
                        customerOrderDetail.WarehouseStockID = item.WarehouseStockID; // Added by Zubair for Inventory on 28-03-2018
                        customerOrderDetail.ShopID = item.ShopID;
                        customerOrderDetail.Qty = item.PurchaseQuantity;
                        customerOrderDetail.OrderStatus = 1;
                        customerOrderDetail.MRP = item.MRP;
                        customerOrderDetail.SaleRate = item.SaleRate;
                        customerOrderDetail.OfferPercent = 0;
                        customerOrderDetail.OfferRs = 0;
                        customerOrderDetail.IsInclusivOfTax = true;
                        //customerOrderDetail.TotalAmount = 10000;
                        customerOrderDetail.TotalAmount = item.SaleRate * item.PurchaseQuantity;
                        customerOrderDetail.BusinessPointPerUnit = item.BusinessPointPerUnit;//Added by Zubair for MLM on 05-01-2018
                        customerOrderDetail.BusinessPoints = item.BusinessPointPerUnit * item.PurchaseQuantity; //Added by Zubair for MLM on 05-01-2018
                        customerOrderDetail.IsActive = true;
                        orderViewModel.CustomerOrder.CreateDate = DateTime.UtcNow;
                        orderViewModel.CustomerOrder.CreateBy = userLoginID;
                        orderViewModel.CustomerOrder.ModifyDate = null;
                        orderViewModel.CustomerOrder.ModifyBy = null;
                        orderViewModel.CustomerOrder.NetworkIP = CommonFunctions.GetClientIP();
                        orderViewModel.CustomerOrder.DeviceType = "x";
                        orderViewModel.CustomerOrder.DeviceID = "x";

                        listOrderDetails.Add(customerOrderDetail);
                    }
                    orderViewModel.CustomerOrderDetail = listOrderDetails;

                    // List of shipping charges

                    List<ShopWiseDeliveryCharges> listShopWiseDeliveryCharges = new List<ShopWiseDeliveryCharges>();

                    //for (int i = 0; i < 2; i++)
                    //{
                    //    ShopWiseDeliveryCharges shopWiseDeliveryCharge = new ShopWiseDeliveryCharges();

                    //    shopWiseDeliveryCharge.ShopID = 10;
                    //    shopWiseDeliveryCharge.Weight = 1;
                    //    shopWiseDeliveryCharge.OrderAmount = 10000;
                    //    shopWiseDeliveryCharge.DeliveryCharge = 200;
                    //    shopWiseDeliveryCharge.DeliveryType = "Express";
                    //    shopWiseDeliveryCharge.ShopOrderCode = string.Empty;

                    //}

                    //orderViewModel.shopWiseDeliveryCharges = listShopWiseDeliveryCharges;
                    orderViewModel.shopWiseDeliveryCharges = lShoppingCartCollection.lShopWiseDeliveryCharges;
                    // Place customer order
                    SetOnlinePaymentCookie(SelectedAddress, IsExpressBuy);

                    long result = PlaceOrder(Convert.ToInt64(userLoginID), SelectedAddress, IsExpressBuy, "1");

                    GetwayPaymentTransaction getwayPaymentTransaction = new ModelLayer.Models.GetwayPaymentTransaction();
                    getwayPaymentTransaction.PaymentMode = "Online Payment";
                    getwayPaymentTransaction.FromUID = userLoginID;
                    getwayPaymentTransaction.ToUID = 1;
                    getwayPaymentTransaction.PaymentGetWayTransactionId = txnid;

                    if (IsOnlinePayment == "1")
                    {
                        getwayPaymentTransaction.Description = "PAYUMONEY";
                    }
                    else
                    {
                        getwayPaymentTransaction.Description = "HDFC";  // added by amit on 11-2-19
                                                                        // getwayPaymentTransaction.Description = "CCAvenue";
                    }
                    getwayPaymentTransaction.IsActive = true;
                    getwayPaymentTransaction.TransactionDate = DateTime.UtcNow;
                    getwayPaymentTransaction.CreateDate = DateTime.UtcNow;
                    getwayPaymentTransaction.Status = 0;
                    getwayPaymentTransaction.CreateBy = 1;
                    getwayPaymentTransaction.DeviceType = "Web";
                    getwayPaymentTransaction.CustomerOrderID = result;
                    db.GetwayPaymentTransactions.Add(getwayPaymentTransaction);
                    db.SaveChanges();

                    if (IsOnlinePayment == "1")
                    {
                        // this.Payumoney(userLoginID, orderViewModel.CustomerOrder.PayableAmount, "My Product", txnid);
                        PayumoneyBIZ(userLoginID, orderViewModel.CustomerOrder.PayableAmount, "My Product", txnid);
                    }
                    else
                    {
                        this.HdfcPayment(userLoginID, orderViewModel.CustomerOrder.PayableAmount, "My Product", txnid);  //added by amit on 11-2-19
                    }

                }

                //Code Added by Harshada for Subscription and corporate gifting to integrate Pau money
                else if (lIsSubscription != -1)
                {
                    SetOnlinePaymentCookie(SelectedAddress, IsExpressBuy);
                    long SubPlanId = lIsSubscription;
                    decimal PlanValue = db.SubscriptionPlans.Where(x => x.ID == lIsSubscription).Select(x => x.Fees).FirstOrDefault();
                    // this.Payumoney(userLoginID, PlanValue, "My Subscription Plan");
                    if (IsOnlinePayment == "1")
                    {
                        this.Payumoney(userLoginID, PlanValue, "My Subscription Plan", txnid);
                    }
                    else
                    {
                        this.CCAvenue(userLoginID, PlanValue, "My Subscription Plan", txnid);
                    }

                }
                else if (lIsCorporateGift != -1)
                {
                    SetOnlinePaymentCookie(SelectedAddress, IsExpressBuy);
                    if (TempData["CorporateGiftCartDetail"] != null)
                    {
                        ShopProductVarientViewModelCollection ShoppingCartCollection = new ShopProductVarientViewModelCollection();
                        ShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["CorporateGiftCartDetail"];
                        decimal PaybleAmount = ShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount;
                        //this.Payumoney(userLoginID, PaybleAmount, "My Subscription Plan");
                        if (IsOnlinePayment == "1")
                        {
                            this.Payumoney(userLoginID, PaybleAmount, "My Subscription Plan", txnid);
                        }
                        else
                        {
                            this.CCAvenue(userLoginID, PaybleAmount, "My Subscription Plan", txnid);
                        }

                    }
                    //End Code Added by Harshada for Subscription and corporate gifting to integrate Pau money
                    //decimal PlanValue = db.SubscriptionPlans.Where(x => x.ID == lIsSubscription).Select(x => x.Fees).FirstOrDefault();

                }
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[PaymentProcessController][M:PlaceOrder]", "Can't place order !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PaymentProcessController][M:PlaceOrder]", "Can't place order !" + Environment.NewLine + ex.Message);
            }
            //return result;
        }

        public ActionResult FailedTransaction(FormCollection frm)
        {
            long result = 0;
            string order_id = string.Empty;
            order_id = Request.Form["txnid"];
            // result = (long)db.GetwayPaymentTransactions.Where(x => x.PaymentGetWayTransactionId == order_id).Select(x => x.CustomerOrderID).FirstOrDefault();
            List<CustomerOrderDetail> lCustomerOrderDetail = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == result).ToList();

            //foreach (var item in lCustomerOrderDetail)
            //{
            //    CustomerOrderDetail objCustomerOrderDetails = db.CustomerOrderDetails.Where(x => x.ID == item.ID).FirstOrDefault();
            //    objCustomerOrderDetails.OrderStatus = 10;

            //    objCustomerOrderDetails.ModifyBy = 1;
            //    objCustomerOrderDetails.ModifyDate = System.DateTime.Now;
            //    db.SaveChanges();
            //}


            TempData["err"] = frm["error"].ToString();
            TempData["errMsg"] = frm["error_Message"].ToString();
            return RedirectToRoute("PurchaseFailure", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]) });////added by Ashish for multiple MCO in same city

        }
        public ActionResult Thankyou(FormCollection form)
        {
            //==============================================================getting value from Payumoney======================
            string city1 = "";
            int FranchiseID1 = 0;////added by Ashish for multiple MCO in same city
            if (Request.Cookies["CityCookie"] != null && (Request.Cookies["CityCookie"].Value != null || Request.Cookies["CityCookie"].Value != string.Empty))
            {
                city1 = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                FranchiseID1 = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city
            }
            long result = 0;
            string[] merc_hash_vars_seq;
            string merc_hash_string = string.Empty;
            string merc_hash = string.Empty;
            string order_id = string.Empty;

            string hash_seq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";

            decimal RequestedAmount = Convert.ToDecimal(form["amount"].ToString());   //amit
            decimal requestedMpid = Convert.ToDecimal(form["mihpayid"].ToString());   //amit
            decimal netAmountDebited = Convert.ToDecimal(form["net_amount_debit"].ToString());  //amit

            string ResponseHash = form["hash"];


            //long temp = Convert.ToInt64(Request.Form["txnid"]);
            //var OldTransID = db.GetwayPaymentTransactions.Where(x => x.AccountTransactionId == temp);



            //if (form["status"].ToString() == "success" && !db.GetwayPaymentTransactions.Any(m => m.PaymentGetWayTransactionId == Request.Form["txnid"]))
            //if (form["status"].ToString() == "success" && OldTransID!=null)
            order_id = form["txnid"].ToString();
            if (form["status"].ToString() == "success")
            {

                merc_hash_vars_seq = hash_seq.Split(new char[]
                {
                    '|'
                });
                Array.Reverse(merc_hash_vars_seq);

                // merc_hash_string = ConfigurationManager.AppSettings["SALT"] + "|" + form["status"].ToString();
                // merc_hash_string = "MpHxx9Oc" + "|" + form["status"].ToString();  // added SALT in url by amit


                var data= form["status"].ToString();
                if (base.Session["salt"] == null)
                {
                    Session["salt"] = "wAdpaW8T"; 

                }
                merc_hash_string = Convert.ToString(Session["salt"]) + "|" + Convert.ToString(form["status"]);   // added by amit

                foreach (string merc_hash_var in merc_hash_vars_seq)
                {
                    merc_hash_string += "|";
                    merc_hash_string = merc_hash_string + (form[merc_hash_var] != null ? form[merc_hash_var] : "");

                }

                Response.Write(merc_hash_string);


                merc_hash = Generatehash512(merc_hash_string).ToLower();

                if (merc_hash.Equals(ResponseHash))
                {
                    //order_id = Request.Form["txnid"];

                    GetwayPaymentTransaction model = db.GetwayPaymentTransactions.Where(x => x.PaymentGetWayTransactionId == order_id && x.Status == 0).FirstOrDefault();// added by amit
                    
                    long count = db.GetwayPaymentTransactions.Where(x => x.PaymentGetWayTransactionId == order_id && x.Status == 0).Count();// added by amit

                    var customerID = model.CustomerOrderID;
                    decimal PayableAmount = db.CustomerOrders.Where(x => x.ID == customerID).Select(y => y.PayableAmount).FirstOrDefault();  //amit

                    if (RequestedAmount == netAmountDebited && RequestedAmount == PayableAmount)   //amit
                    {
                        //if (!db.GetwayPaymentTransactions.Any(m => m.PaymentGetWayTransactionId == order_id))
                        //{

                        //================================================================================================================

                        if (count > 0)
                        {

                            long userLoginID = 0;

                            long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);
                            // string delSchedID = string.Empty;
                            if (userLoginID == 0)
                            {
                                var pay = db.PaymentDatas.Where(x => x.TxnId == order_id).FirstOrDefault();

                                //var pay = db.PaymentDatas.Where(x => x.TxnId.ToString() == order_id).FirstOrDefault();
                                if (pay != null)
                                {
                                    userLoginID = pay.UserId;
                                    Session["UID"] = userLoginID;
                                    long personalDetailId = CommonFunctions.GetPersonalDetailsID(userLoginID);
                                    if (personalDetailId == 0)
                                    {
                                        Session["FirstName"] = "";
                                    }
                                    else
                                    {
                                        Session["FirstName"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(db.PersonalDetails.Find(personalDetailId).FirstName.ToLower());
                                    }
                                }
                            }
                            ViewBag.ID = userLoginID;

                            //if (ControllerContext.HttpContext.Request.Cookies["OnlinePaymentCookie"] != null)
                            //{
                            //    string[] cookieDetails = ControllerContext.HttpContext.Request.Cookies["OnlinePaymentCookie"].Value.Split('$');

                            //Change made by Harshada for Subscription Date 23 rd sep 15
                            if (TempData["IsSubscription"] != null)
                            {
                                long SubPlanId = lIsSubscription;
                                int NoOfDays = db.SubscriptionPlans.Where(x => x.ID == lIsSubscription).Select(x => x.NoOfDays).FirstOrDefault();
                                BusinessLogicLayer.OrderPlacedSmsAndEmail orderPlaced = new OrderPlacedSmsAndEmail(System.Web.HttpContext.Current.Server);
                                orderPlaced.SendSubScriptionMailToCustomer(userLoginID);
                                orderPlaced.SendSubScriptionSMSToCustomer(userLoginID, NoOfDays);
                                //orderPlaced.SendSMSToCustomer(userLoginID, 0);
                                //orderPlaced.SendMailToCustomer(userLoginID, 0);
                                //orderPlaced.SendMailToMerchant(userLoginID, 0);

                                //------------------------------------------------------------------------------------------------------------------------
                                //GetwayPaymentTransaction getwayPaymentTransaction = new ModelLayer.Models.GetwayPaymentTransaction();
                                //getwayPaymentTransaction.PaymentMode = "Online Payment";
                                //getwayPaymentTransaction.FromUID = userLoginID;
                                //getwayPaymentTransaction.ToUID = 1;
                                //getwayPaymentTransaction.PaymentGetWayTransactionId = order_id;
                                //getwayPaymentTransaction.Description = "PAYUMONEY";
                                //getwayPaymentTransaction.IsActive = true;
                                //getwayPaymentTransaction.TransactionDate = DateTime.UtcNow;
                                //getwayPaymentTransaction.CreateDate = DateTime.UtcNow;
                                //getwayPaymentTransaction.CreateBy = 1;
                                //getwayPaymentTransaction.DeviceType = "Web";
                                //db.GetwayPaymentTransactions.Add(getwayPaymentTransaction);
                                //db.SaveChanges();

                                //-----------------------------------------------------------------------------------------------------------------------

                                //ShoppingCartInitialization s = new ShoppingCartInitialization();

                                //s.DeleteShoppingCartCookie();
                                //s.DeleteCouponCookie();

                                //TempData["OrderMessage"] = "Order Placed Successfully";
                                //------------------Comment By mohit------------------------//
                                //long SubPlanID = Convert.ToInt64(TempData["IsSubscription"]);
                                //int AlreadyExist = Convert.ToInt32(TempData["IsAlreadyExist"]);
                                //long ReferredByLoginID = Convert.ToInt64(TempData["ReferredByLoginID"]);
                                //return RedirectToAction("SaveCustSubscriptionPlan", "SubscriptionPlan", new { SubPlanID = SubPlanID, AlreadyExist = AlreadyExist });
                                //--End of comment 
                                //return RedirectToAction("SaveCustSubscriptionPlan", "SubscriptionPlan", new { SubPlanID = SubPlanID, AlreadyExist = AlreadyExist, ReferredByLoginID = ReferredByLoginID });
                                return RedirectToAction("SaveCustSubscriptionPlan", "SubscriptionPlan", new { SubPlanID = lIsSubscription, AlreadyExist = lIsAlreadyExist, ReferredByLoginID = lReferredByLoginID });
                            }
                            //change made by harshada for corporate gifting
                            if (TempData["IsCorporateGift"] != null)
                            {
                                //GetwayPaymentTransaction getwayPaymentTransaction = new ModelLayer.Models.GetwayPaymentTransaction();
                                //getwayPaymentTransaction.PaymentMode = "Online Payment";
                                //getwayPaymentTransaction.FromUID = userLoginID;
                                //getwayPaymentTransaction.ToUID = 1;
                                //getwayPaymentTransaction.PaymentGetWayTransactionId = order_id;
                                //getwayPaymentTransaction.Description = "PAYUMONEY";
                                //getwayPaymentTransaction.IsActive = true;
                                //getwayPaymentTransaction.TransactionDate = DateTime.UtcNow;
                                //getwayPaymentTransaction.CreateDate = DateTime.UtcNow;
                                //getwayPaymentTransaction.CreateBy = 1;
                                //getwayPaymentTransaction.DeviceType = "Web";
                                //db.GetwayPaymentTransactions.Add(getwayPaymentTransaction);
                                //db.SaveChanges();

                                return RedirectToAction("ThankYouCorporate1", "Product");
                            }
                            //long result = PlaceOrder(Convert.ToInt64(userLoginID), cookieDetails[0], Convert.ToBoolean(cookieDetails[1]), "1", out delSchedID);
                            //long result = PlaceOrder(Convert.ToInt64(userLoginID), cookieDetails[0], Convert.ToBoolean(cookieDetails[1]), "1");
                            result = (long)db.GetwayPaymentTransactions.Where(x => x.PaymentGetWayTransactionId == order_id).Select(x => x.CustomerOrderID).FirstOrDefault();
                            // GetwayPaymentTransaction model = db.GetwayPaymentTransactions.Where(x => x.PaymentGetWayTransactionId == order_id).FirstOrDefault();


                            // if order placed successfully
                            if (result > 0)
                            {
                                //    //------------------------------------------------------------------------------------------------------------------------
                                //    GetwayPaymentTransaction getwayPaymentTransaction = new ModelLayer.Models.GetwayPaymentTransaction();
                                //    getwayPaymentTransaction.PaymentMode = "Online Payment";
                                //    getwayPaymentTransaction.FromUID = userLoginID;
                                //    getwayPaymentTransaction.ToUID = 1;
                                //    getwayPaymentTransaction.PaymentGetWayTransactionId = order_id;
                                //    getwayPaymentTransaction.Description = "PAYUMONEY";
                                //    getwayPaymentTransaction.IsActive = true;
                                //    getwayPaymentTransaction.TransactionDate = DateTime.UtcNow;
                                //    getwayPaymentTransaction.CreateDate = DateTime.UtcNow;
                                //    getwayPaymentTransaction.CreateBy = 1;
                                //    getwayPaymentTransaction.DeviceType = "Web";
                                //    getwayPaymentTransaction.CustomerOrderID = result;
                                //    db.GetwayPaymentTransactions.Add(getwayPaymentTransaction);
                                //    db.SaveChanges();

                                //    //-----------------------------------------------------------------------------------------------------------------------

                                //    ShoppingCartInitialization s = new ShoppingCartInitialization();

                                //    s.DeleteShoppingCartCookie();
                                //    s.DeleteCouponCookie();

                                //    TempData["OrderMessage"] = "Order Placed Successfully";

                                //    //return RedirectToAction("PurchaseComplete", "CustomerOrder", new { orderID = result });
                                //    //return RedirectToRoute("PurchaseComplete", new { orderID = result, city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower() });
                                //    //return RedirectToAction("Test");
                                //    TempData["orderID"] = result;
                                //    return RedirectToRoute("PurchaseComplete", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower() });


                                GetwayPaymentTransaction lGetwayPaymentTransaction = db.GetwayPaymentTransactions.Where(x => x.PaymentGetWayTransactionId == order_id).FirstOrDefault();
                                lGetwayPaymentTransaction.Status = 1;
                                db.SaveChanges();

                                List<CustomerOrderDetail> lCustomerOrderDetail = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == result).ToList();

                                foreach (var item in lCustomerOrderDetail)
                                {
                                    CustomerOrderDetail objCustomerOrderDetails = db.CustomerOrderDetails.Where(x => x.ID == item.ID).FirstOrDefault();
                                    objCustomerOrderDetails.OrderStatus = 1;

                                    objCustomerOrderDetails.ModifyBy = 1;
                                    objCustomerOrderDetails.ModifyDate = System.DateTime.Now;
                                    db.SaveChanges();
                                }


                                // db.SaveChanges();

                                BusinessLogicLayer.OrderPlacedSmsAndEmail orderPlaced = new OrderPlacedSmsAndEmail(System.Web.HttpContext.Current.Server);

                                orderPlaced.SendSMSToCustomer(userLoginID, result);
                                orderPlaced.SendMailToCustomer(userLoginID, result);
                                orderPlaced.SendMailToMerchant(userLoginID, result);
                                (new SendFCMNotification()).SendNotification("placed", result);
                                TempData["orderID"] = result;

                                //========= Delete cart cookie and coupon cookie ============
                                ShoppingCartInitialization s = new ShoppingCartInitialization();
                                s.DeleteShoppingCartCookie();
                                s.DeleteCouponCookie();
                                //========= Delete cart cookie and coupon cookie ============
                                string city = "";
                                int FranchiseID = 0;////added by Ashish for multiple MCO in same city
                                if (Request.Cookies["CityCookie"] != null && (Request.Cookies["CityCookie"].Value != null || Request.Cookies["CityCookie"].Value != string.Empty))
                                {
                                    city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                                    FranchiseID = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city
                                    Session["SelectedCity"] = city;
                                    Session["SelectedFranchiseId"] = FranchiseID;
                                   // var datacity = Session["SelectedCity"].ToString();
                                }
                                ShoppingCartInitialization sa = new ShoppingCartInitialization();

                                sa.DeleteShoppingCartCookie();
                                sa.DeleteCouponCookie();
                                var pay = db.PaymentDatas.Where(x => x.TxnId == order_id).FirstOrDefault();

                                //var pay = db.PaymentDatas.Where(x => x.TxnId.ToString() == order_id).FirstOrDefault();
                                if (pay != null)
                                {
                                    city = pay.City;
                                    FranchiseID = pay.FranchiseId;
                                    Session["Crty"] = pay.CartId;
                                    Response.Cookies["CartID"].Value = pay.CartId.ToString();
                                }
                                return RedirectToRoute("PurchaseComplete", new { city = city, franchiseId = FranchiseID });


                                ////added by Ashish for multiple MCO in same city

                                //return RedirectToRoute("PurchaseComplete", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]) });////added by Ashish for multiple MCO in same city
                            }

                            else
                            {
                                result = (long)db.GetwayPaymentTransactions.Where(x => x.PaymentGetWayTransactionId == order_id).Select(x => x.CustomerOrderID).FirstOrDefault();
                                List<CustomerOrderDetail> lCustomerOrderDetail = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == result).ToList();

                                //foreach (var item in lCustomerOrderDetail)
                                //{
                                //    CustomerOrderDetail objCustomerOrderDetails = db.CustomerOrderDetails.Where(x => x.ID == item.ID).FirstOrDefault();
                                //    objCustomerOrderDetails.OrderStatus = 10;

                                //    objCustomerOrderDetails.ModifyBy = 1;
                                //    objCustomerOrderDetails.ModifyDate = System.DateTime.Now;
                                //    db.SaveChanges();
                                //}
                                TempData["err"] = form["error"].ToString();
                                TempData["errMsg"] = form["error_Message"].ToString();
                                return RedirectToRoute("PurchaseFailure", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]) });////added by Ashish for multiple MCO in same city

                            }

                            //}
                            //}
                        }

                        form["error_Message"] = "Requested amount is not valid";

                    }
                    else
                    {
                        result = (long)db.GetwayPaymentTransactions.Where(x => x.PaymentGetWayTransactionId == order_id).Select(x => x.CustomerOrderID).FirstOrDefault();
                        List<CustomerOrderDetail> lCustomerOrderDetail = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == result).ToList();

                        //foreach (var item in lCustomerOrderDetail)
                        //{
                        //    CustomerOrderDetail objCustomerOrderDetails = db.CustomerOrderDetails.Where(x => x.ID == item.ID).FirstOrDefault();
                        //    objCustomerOrderDetails.OrderStatus = 10;

                        //    objCustomerOrderDetails.ModifyBy = 1;
                        //    objCustomerOrderDetails.ModifyDate = System.DateTime.Now;
                        //    db.SaveChanges();
                        //}
                        TempData["err"] = form["error"].ToString();
                        TempData["errMsg"] = form["error_Message"].ToString();
                        return RedirectToRoute("PurchaseFailure", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]) });////added by Ashish for multiple MCO in same city

                    }
                    //form["error_Message"] = "Requested amount is not valid";
                }
                else
                {
                    result = (long)db.GetwayPaymentTransactions.Where(x => x.PaymentGetWayTransactionId == order_id).Select(x => x.CustomerOrderID).FirstOrDefault();
                    List<CustomerOrderDetail> lCustomerOrderDetail = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == result).ToList();

                    //foreach (var item in lCustomerOrderDetail)
                    //{
                    //    CustomerOrderDetail objCustomerOrderDetails = db.CustomerOrderDetails.Where(x => x.ID == item.ID).FirstOrDefault();
                    //    objCustomerOrderDetails.OrderStatus = 10;

                    //    objCustomerOrderDetails.ModifyBy = 1;
                    //    objCustomerOrderDetails.ModifyDate = System.DateTime.Now;
                    //    db.SaveChanges();
                    //}
                    TempData["err"] = form["error"].ToString();
                    TempData["errMsg"] = form["error_Message"].ToString();
                    return RedirectToRoute("PurchaseFailure", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]) });////added by Ashish for multiple MCO in same city

                }
            }
            else
            {
                result = (long)db.GetwayPaymentTransactions.Where(x => x.PaymentGetWayTransactionId == order_id).Select(x => x.CustomerOrderID).FirstOrDefault();
                List<CustomerOrderDetail> lCustomerOrderDetail = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == result).ToList();

                foreach (var item in lCustomerOrderDetail)
                {
                    CustomerOrderDetail objCustomerOrderDetails = db.CustomerOrderDetails.Where(x => x.ID == item.ID).FirstOrDefault();
                    // objCustomerOrderDetails.OrderStatus = 10;

                    // objCustomerOrderDetails.ModifyBy = 1;
                    // objCustomerOrderDetails.ModifyDate = System.DateTime.Now;
                    // db.SaveChanges();
                    TempData["err"] = form["error"].ToString();
                    TempData["errMsg"] = form["error_Message"].ToString();
                    return RedirectToRoute("PurchaseFailure", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]) });////added by Ashish for multiple MCO in same city

                }

            }
            return RedirectToRoute("PurchaseComplete", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]) });////added by Ashish for multiple MCO in same city
        }

        public ActionResult ThankyouCCAvenue(object sender)
        {
            try
            {
                string workingKey = "1F77E0F956AC4378351C03BE64CD22C3";//old gandhibagh-> "61E07D550F80389793187FD5298AC00B";
                //string workingKey = "A3927F246CDE29AA3042DA0D3F8EA5D7";//Testing key
                CCACrypto ccaCrypto = new CCACrypto();
                string encResponse = ccaCrypto.Decrypt(Request.Form["encResp"], workingKey);
                NameValueCollection Params = new NameValueCollection();
                string[] segments = encResponse.Split('&');

                //if (segments[3] != "order_status=Failure")
                if (segments[3] == "order_status=Success")
                {

                    //-----------------------------
                    string order_id = TempData["CCAvenueInformatn"].ToString();
                    long userLoginID = 0;
                    long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);
                    // string delSchedID = string.Empty;

                    ViewBag.ID = userLoginID;

                    //if (ControllerContext.HttpContext.Request.Cookies["OnlinePaymentCookie"] != null)
                    //{
                    //    string[] cookieDetails = ControllerContext.HttpContext.Request.Cookies["OnlinePaymentCookie"].Value.Split('$');

                    //Change made by Harshada for Subscription Date 23 rd sep 15
                    if (TempData["IsSubscription"] != null)
                    {
                        long SubPlanId = lIsSubscription;
                        int NoOfDays = db.SubscriptionPlans.Where(x => x.ID == lIsSubscription).Select(x => x.NoOfDays).FirstOrDefault();
                        BusinessLogicLayer.OrderPlacedSmsAndEmail orderPlaced = new OrderPlacedSmsAndEmail(System.Web.HttpContext.Current.Server);
                        orderPlaced.SendSubScriptionMailToCustomer(userLoginID);
                        orderPlaced.SendSubScriptionSMSToCustomer(userLoginID, NoOfDays);

                        //orderPlaced.SendSMSToCustomer(userLoginID, 0);
                        //orderPlaced.SendMailToCustomer(userLoginID, 0);
                        //orderPlaced.SendMailToMerchant(userLoginID, 0);

                        //------------------------------------------------------------------------------------------------------------------------
                        //GetwayPaymentTransaction getwayPaymentTransaction = new ModelLayer.Models.GetwayPaymentTransaction();
                        //getwayPaymentTransaction.PaymentMode = "Online Payment";
                        //getwayPaymentTransaction.FromUID = userLoginID;
                        //getwayPaymentTransaction.ToUID = 1;
                        //getwayPaymentTransaction.PaymentGetWayTransactionId = order_id;
                        //getwayPaymentTransaction.Description = "CCAVENUE";
                        //getwayPaymentTransaction.IsActive = true;
                        //getwayPaymentTransaction.TransactionDate = DateTime.UtcNow;
                        //getwayPaymentTransaction.CreateDate = DateTime.UtcNow;
                        //getwayPaymentTransaction.CreateBy = 1;
                        //getwayPaymentTransaction.DeviceType = "Web";
                        //db.GetwayPaymentTransactions.Add(getwayPaymentTransaction);
                        //db.SaveChanges();

                        //-----------------------------------------------------------------------------------------------------------------------

                        ShoppingCartInitialization s = new ShoppingCartInitialization();

                        //s.DeleteShoppingCartCookie();
                        //s.DeleteCouponCookie();

                        //TempData["OrderMessage"] = "Order Placed Successfully";
                        //------------------Comment By mohit------------------------//
                        //long SubPlanID = Convert.ToInt64(TempData["IsSubscription"]);
                        //int AlreadyExist = Convert.ToInt32(TempData["IsAlreadyExist"]);
                        //long ReferredByLoginID = Convert.ToInt64(TempData["ReferredByLoginID"]);
                        //return RedirectToAction("SaveCustSubscriptionPlan", "SubscriptionPlan", new { SubPlanID = SubPlanID, AlreadyExist = AlreadyExist });
                        //--End of comment 
                        //return RedirectToAction("SaveCustSubscriptionPlan", "SubscriptionPlan", new { SubPlanID = SubPlanID, AlreadyExist = AlreadyExist, ReferredByLoginID = ReferredByLoginID });
                        return RedirectToAction("SaveCustSubscriptionPlan", "SubscriptionPlan", new { SubPlanID = lIsSubscription, AlreadyExist = lIsAlreadyExist, ReferredByLoginID = lReferredByLoginID });
                    }
                    //change made by harshada for corporate gifting
                    if (TempData["IsCorporateGift"] != null)
                    {
                        //GetwayPaymentTransaction getwayPaymentTransaction = new ModelLayer.Models.GetwayPaymentTransaction();
                        //getwayPaymentTransaction.PaymentMode = "Online Payment";
                        //getwayPaymentTransaction.FromUID = userLoginID;
                        //getwayPaymentTransaction.ToUID = 1;
                        //getwayPaymentTransaction.PaymentGetWayTransactionId = order_id;
                        //getwayPaymentTransaction.Description = "CCAVENUE";
                        //getwayPaymentTransaction.IsActive = true;
                        //getwayPaymentTransaction.TransactionDate = DateTime.UtcNow;
                        //getwayPaymentTransaction.CreateDate = DateTime.UtcNow;
                        //getwayPaymentTransaction.CreateBy = 1;
                        //getwayPaymentTransaction.DeviceType = "Web";
                        //db.GetwayPaymentTransactions.Add(getwayPaymentTransaction);
                        //db.SaveChanges();

                        return RedirectToAction("ThankYouCorporate1", "Product");
                    }
                    //long result = PlaceOrder(Convert.ToInt64(userLoginID), cookieDetails[0], Convert.ToBoolean(cookieDetails[1]), "1", out delSchedID);
                    //long result = PlaceOrder(Convert.ToInt64(userLoginID), cookieDetails[0], Convert.ToBoolean(cookieDetails[1]), "1");
                    long result = (long)db.GetwayPaymentTransactions.Where(x => x.PaymentGetWayTransactionId == order_id).Select(x => x.CustomerOrderID).FirstOrDefault();
                    // if order placed successfully
                    if (result > 0)
                    {
                        //BusinessLogicLayer.OrderPlacedSmsAndEmail orderPlaced = new OrderPlacedSmsAndEmail(System.Web.HttpContext.Current.Server);

                        //orderPlaced.SendSMSToCustomer(userLoginID, result);
                        //orderPlaced.SendMailToCustomer(userLoginID, result);
                        //orderPlaced.SendMailToMerchant(userLoginID, result);

                        //------------------------------------------------------------------------------------------------------------------------
                        //GetwayPaymentTransaction getwayPaymentTransaction = new ModelLayer.Models.GetwayPaymentTransaction();
                        //getwayPaymentTransaction.PaymentMode = "Online Payment";
                        //getwayPaymentTransaction.FromUID = userLoginID;
                        //getwayPaymentTransaction.ToUID = 1;
                        //getwayPaymentTransaction.PaymentGetWayTransactionId = order_id;
                        //getwayPaymentTransaction.Description = "CCAVENUE";
                        //getwayPaymentTransaction.IsActive = true;
                        //getwayPaymentTransaction.TransactionDate = DateTime.UtcNow;
                        //getwayPaymentTransaction.CreateDate = DateTime.UtcNow;
                        //getwayPaymentTransaction.CreateBy = 1;
                        //getwayPaymentTransaction.DeviceType = "Web";
                        //getwayPaymentTransaction.CustomerOrderID = result;
                        //db.GetwayPaymentTransactions.Add(getwayPaymentTransaction);
                        //db.SaveChanges();

                        //-----------------------------------------------------------------------------------------------------------------------

                        //ShoppingCartInitialization s = new ShoppingCartInitialization();

                        //s.DeleteShoppingCartCookie();
                        //s.DeleteCouponCookie();

                        //TempData["OrderMessage"] = "Order Placed Successfully";

                        //return RedirectToAction("PurchaseComplete", "CustomerOrder", new { orderID = result });
                        //return RedirectToRoute("PurchaseComplete", new { orderID = result, city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower() });
                        //return RedirectToAction("Test");
                        GetwayPaymentTransaction lGetwayPaymentTransaction = db.GetwayPaymentTransactions.Where(x => x.PaymentGetWayTransactionId == order_id).FirstOrDefault();
                        lGetwayPaymentTransaction.Status = 1;
                        db.SaveChanges();

                        CustomerOrderDetail lCustomerOrderDetail = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == result).FirstOrDefault();
                        lCustomerOrderDetail.OrderStatus = 1;
                        lCustomerOrderDetail.ModifyBy = 1;
                        db.SaveChanges();

                        BusinessLogicLayer.OrderPlacedSmsAndEmail orderPlaced = new OrderPlacedSmsAndEmail(System.Web.HttpContext.Current.Server);

                        orderPlaced.SendSMSToCustomer(userLoginID, result);
                        orderPlaced.SendMailToCustomer(userLoginID, result);
                        orderPlaced.SendMailToMerchant(userLoginID, result);
                        TempData["orderID"] = result;

                        //========= Delete cart cookie and coupon cookie ============
                        ShoppingCartInitialization s = new ShoppingCartInitialization();
                        s.DeleteShoppingCartCookie();
                        s.DeleteCouponCookie();
                        //========= Delete cart cookie and coupon cookie ============

                        //return RedirectToRoute("PurchaseComplete", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower() });////hide by Ashish for multiple MCO in same city
                        return RedirectToRoute("PurchaseComplete", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]) });////added by Ashish for multiple MCO in same city
                    }
                }
                //}
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with loading shopping cart!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][GET:ThankyouCCAvenue]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with loading shopping cart!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][GET:ThankyouCCAvenue]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }

            TempData["TransFailuer"] = "Your transaction is failed....Please try again.";
            return RedirectToRoute("PaymentProcess");
        }
        //added by amit
        public void HdfcPayment(long userLoginId, decimal totalOrderAmount, string productName, string txnid)
        {

            var personalDetails = from u in db.UserLogins
                                  join p in db.PersonalDetails on u.ID equals p.UserLoginID
                                  where u.ID == userLoginId
                                  select new { u.Email, u.Mobile, p.FirstName };

            string city = "";
            int FranchiseID = 0;////added by Ashish for multiple MCO in same city
            if (Request.Cookies["CityCookie"].Value != null || Request.Cookies["CityCookie"].Value != string.Empty)
            {
                city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                FranchiseID = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city
            }
            string firstName = personalDetails.FirstOrDefault().FirstName;
            decimal amount = totalOrderAmount;
            string productInfo = productName;
            string email = personalDetails.FirstOrDefault().Email;
            string phone = personalDetails.FirstOrDefault().Mobile;
            // string surl = "http://localhost:1427/CustomerOrder/PurchaseComplete?orderID=" + 1;
            //string surl = this.GetUrl() + city + "/PaymentProcess/Thankyou";////hide by Ashish for multiple MCO in same city
            //string furl = this.GetUrl() + city + "/payment-process";////hide by Ashish for multiple MCO in same city

            string surl = this.GetUrl() + city + "/" + FranchiseID + "/PaymentProcess/Thankyou";////added "/" + FranchiseID by Ashish for multiple MCO in same city
            string furl = this.GetUrl() + city + "/" + FranchiseID + "/PaymentProcess/FailedTransaction";////added "/" + FranchiseID by Ashish for multiple MCO in same city

            RemotePost myremotepost = new RemotePost();
            //Old
            //string key = "X7tbZJ";// "oOGoT0";// "gtKFFx";
            //string salt = "ZZ5PdW7W";// "CwPlCmp2";//"eCwWELxi";

            //New added on 28-09-2017 by Zubair
            // string key = "7rnFly";// "oOGoT0";// "gtKFFx";
            // string salt = "pjVQAWpA";// "CwPlCmp2";//"eCwWELxi";

            // New Live Credential on 4-3-19
            string key = "gVKEuP";// "oOGoT0";// "gtKFFx";
            string salt = "MpHxx9Oc";// "CwPlCmp2";//"eCwWELxi";

            Session["salt"] = salt;   // added by amit

            // myremotepost.Url = "https://test.payu.in/_payment";

            myremotepost.Url = "https://secure.payu.in/_payment";


            //posting all the parameters required for integration.

            // myremotepost.Url = "https://secure.payu.in/_payment";// "https://test.payu.in/_payment";//"https://secure.payu.in/_payment";

            //Old
            //myremotepost.Add("key", "X7tbZJ");// "oOGoT0";// "gtKFFx");

            ////New added on 28-09-2017 by Zubair
            //myremotepost.Add("key", "VPket8");// "oOGoT0";// "gtKFFx");


            myremotepost.Add("key", "gVKEuP");

            myremotepost.Add("txnid", txnid);
            myremotepost.Add("amount", totalOrderAmount.ToString());
            myremotepost.Add("productinfo", productInfo);
            myremotepost.Add("firstname", firstName);
            myremotepost.Add("phone", phone);
            myremotepost.Add("email", email);
            // string txnid = Generatetxnid();


            myremotepost.Add("surl", surl);//Change the success url here depending upon the port number of your local system.
            myremotepost.Add("furl", furl);//Change the failure url here depending upon the port number of your local system.
            myremotepost.Add("service_provider", "");// "payu_paisa");
            string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|||||||||||" + salt;

            //string hashString = "3Q5c3q|2590640|3053.00|OnlineBooking|vimallad|ladvimal@gmail.com|||||||||||mE2RxRwx";
            string hash = Generatehash512(hashString);
            myremotepost.Add("hash", hash);

            myremotepost.Post();


        }
        //----------------end by amit--------------//

        //-----------End---------------------------------------------
        public void Payumoney(long userLoginId, decimal totalOrderAmount, string productName, string txnid)
        {
            var personalDetails = from u in db.UserLogins
                                  join p in db.PersonalDetails on u.ID equals p.UserLoginID
                                  where u.ID == userLoginId
                                  select new { u.Email, u.Mobile, p.FirstName };

            string city = "";
            int FranchiseID = 0;////added by Ashish for multiple MCO in same city
            if (Request.Cookies["CityCookie"].Value != null || Request.Cookies["CityCookie"].Value != string.Empty)
            {
                city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                FranchiseID = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city
            }
            string firstName = personalDetails.FirstOrDefault().FirstName;
            decimal amount = totalOrderAmount;
            string productInfo = productName;
            string email = personalDetails.FirstOrDefault().Email;
            string phone = personalDetails.FirstOrDefault().Mobile;
            // string surl = "http://localhost:1427/CustomerOrder/PurchaseComplete?orderID=" + 1;
            //string surl = this.GetUrl() + city + "/PaymentProcess/Thankyou";////hide by Ashish for multiple MCO in same city
            //string furl = this.GetUrl() + city + "/payment-process";////hide by Ashish for multiple MCO in same city

            string surl = this.GetUrl() + city + "/" + FranchiseID + "/PaymentProcess/Thankyou";////added "/" + FranchiseID by Ashish for multiple MCO in same city
            string furl = this.GetUrl() + city + "/" + FranchiseID + "/payment-process";////added "/" + FranchiseID by Ashish for multiple MCO in same city


            RemotePost myremotepost = new RemotePost();

            string key = "z1u2iD";       //live credential
            string salt = "M3bxdMaT";


            //string key = "dt5pbm";
            //string salt = "UHSBi87J";



            //posting all the parameters required for integration.
            // myremotepost.Url = "https://test.payu.in/_payment";  //added by amit for testing
            myremotepost.Url = "https://secure.payu.in/_payment";   // live URL
            myremotepost.Method = "POST";
            myremotepost.Add("key", "7rnFly");
            // string txnid = Generatetxnid();
            myremotepost.Add("txnid", txnid);
            myremotepost.Add("amount", totalOrderAmount.ToString());
            myremotepost.Add("productinfo", productInfo);
            myremotepost.Add("firstname", firstName);
            myremotepost.Add("phone", phone);
            myremotepost.Add("email", email);
            myremotepost.Add("surl", surl);//Change the success url here depending upon the port number of your local system.
            myremotepost.Add("furl", furl);//Change the failure url here depending upon the port number of your local system.
            myremotepost.Add("service_provider", "payu_paisa");
            string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|||||||||||" + salt;
            //string hashString = "3Q5c3q|2590640|3053.00|OnlineBooking|vimallad|ladvimal@gmail.com|||||||||||mE2RxRwx";
            string hash = Generatehash512(hashString);
            myremotepost.Add("hash", hash);

            myremotepost.Post();


        }
        public void PayumoneyBIZ(long userLoginId, decimal totalOrderAmount, string productName, string txnid)
        {
            var personalDetails = from u in db.UserLogins
                                  join p in db.PersonalDetails on u.ID equals p.UserLoginID
                                  where u.ID == userLoginId
                                  select new { u.Email, u.Mobile, p.FirstName };

            string city = "";

            int FranchiseID = 0;////added by Ashish for multiple MCO in same city
            if (Request.Cookies["CityCookie"].Value != null || Request.Cookies["CityCookie"].Value != string.Empty)
            {
                city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                FranchiseID = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city
                Session["SelectedCity"] = city;
                Session["SelectedFranchiseId"] = FranchiseID;
            }
            var crty= Convert.ToInt64(ControllerContext.HttpContext.Request.Cookies["CartID"].Value);
            var data = new PaymentData();
            data.City = city;
            data.FranchiseId = FranchiseID;
            data.TxnId = txnid;
            data.UserId = userLoginId;
            data.CartId = crty;
            db.PaymentDatas.Add(data);
            db.SaveChanges();
            string firstName = personalDetails.FirstOrDefault().FirstName;
            decimal amount = totalOrderAmount;
            string productInfo = productName;
            string email = personalDetails.FirstOrDefault().Email;
            string phone = personalDetails.FirstOrDefault().Mobile;
            // string surl = "http://localhost:1427/CustomerOrder/PurchaseComplete?orderID=" + 1;
            //string surl = this.GetUrl() + city + "/PaymentProcess/Thankyou";////hide by Ashish for multiple MCO in same city
            //string furl = this.GetUrl() + city + "/payment-process";////hide by Ashish for multiple MCO in same city

            string surl = this.GetUrl() + city + "/" + FranchiseID + "/PaymentProcess/Thankyou";////added "/" + FranchiseID by Ashish for multiple MCO in same city
            string furl = this.GetUrl() + city + "/" + FranchiseID + "/PaymentProcess/FailedTransaction";////added "/" + FranchiseID by Ashish for multiple MCO in same city


            RemotePost myremotepost = new RemotePost();
            //Old
            //string key = "X7tbZJ";// "oOGoT0";// "gtKFFx";
            //string salt = "ZZ5PdW7W";// "CwPlCmp2";//"eCwWELxi";

            //New added on 28-09-2017 live by Zubair
            string key = "VPket8";// "oOGoT0";// "gtKFFx";
            string salt = "wAdpaW8T";// "CwPlCmp2";//"eCwWELxi";

            //string key = "dt5pbm";
            //string salt = "UHSBi87J";

            // string key = "7rnFly";// "oOGoT0";// "gtKFFx";     //added by amit for testing 
            // string salt = "pjVQAWpA";// "CwPlCmp2";//"eCwWELxi";


            Session["salt"] = salt;   // added by amit
            //posting all the parameters required for integration.

            myremotepost.Url = "https://secure.payu.in/_payment";// "https://test.payu.in/_payment";//"https://secure.payu.in/_payment";

            // myremotepost.Url = "https://test.payu.in/_payment";
            //Old
            //myremotepost.Add("key", "X7tbZJ");// "oOGoT0";// "gtKFFx");

            ////New added on 28-09-2017 by Zubair
            ///
            


            myremotepost.Add("key", "VPket8");// "oOGoT0";// "gtKFFx");

            // string txnid = Generatetxnid();
            myremotepost.Add("txnid", txnid);
            myremotepost.Add("amount", totalOrderAmount.ToString());
            myremotepost.Add("productinfo", productInfo);
            myremotepost.Add("firstname", firstName);
            myremotepost.Add("phone", phone);
            myremotepost.Add("email", email);
            myremotepost.Add("surl", surl);//Change the success url here depending upon the port number of your local system.
            myremotepost.Add("furl", furl);//Change the failure url here depending upon the port number of your local system.
            myremotepost.Add("service_provider", "");// "payu_paisa");
            string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|||||||||||" + salt;
            //string hashString = "3Q5c3q|2590640|3053.00|OnlineBooking|vimallad|ladvimal@gmail.com|||||||||||mE2RxRwx";
            string hash = Generatehash512(hashString);
            myremotepost.Add("hash", hash);

            myremotepost.Post();


        }

        public void CCAvenue(long userLoginId, decimal totalOrderAmount, string productName, string txnid)
        {
            var personalDetails = from u in db.UserLogins
                                  join p in db.PersonalDetails on u.ID equals p.UserLoginID
                                  join s in db.CustomerShippingAddresses on u.ID equals s.UserLoginID
                                  where u.ID == userLoginId
                                  select new { u.Email, u.Mobile, p.FirstName, s.ShippingAddress, s.PincodeID };

            string city = "";
            long CityID = 4968;
            int FranchiseID = 2;////added by Ashish for multiple MCO in same city
            if (Request.Cookies["CityCookie"].Value != null || Request.Cookies["CityCookie"].Value != string.Empty)
            {
                city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                CityID = Convert.ToInt64(Request.Cookies["CityCookie"].Value.Split('$')[0]);
                FranchiseID = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city
            }

            string state = db.Cities.FirstOrDefault(x => x.ID == CityID).District.State.Name;
            long pincodeID = personalDetails.FirstOrDefault().PincodeID;
            string Pincode = db.Pincodes.Where(x => x.ID == pincodeID).FirstOrDefault().Name;
            string firstName = personalDetails.FirstOrDefault().FirstName;
            decimal amount = totalOrderAmount;
            string productInfo = productName;
            string email = personalDetails.FirstOrDefault().Email;
            string Mobile = personalDetails.FirstOrDefault().Mobile;
            string Address = personalDetails.FirstOrDefault().ShippingAddress;
            //string surl = this.GetUrl() + city + "/thankyou-ccavenue";////hide by Ashish for multiple MCO in same city
            //string furl = this.GetUrl() + city + "/payment-process";////hide by Ashish for multiple MCO in same city
            string surl = this.GetUrl() + city + "/" + FranchiseID + "/thankyou-ccavenue";////added "/" + FranchiseID by Ashish for multiple MCO in same city
            //string furl = this.GetUrl() + city + "/PaymentProcess/CustomerPaymentProcess";
            string furl = this.GetUrl() + city + "/" + FranchiseID + "/payment-process";////added "/" + FranchiseID by Ashish for multiple MCO in same city
            RemotePost myremotepost = new RemotePost();

            CCACrypto ccaCrypto = new CCACrypto();

            string workingKey = "1F77E0F956AC4378351C03BE64CD22C3";//old gandhibagh-> "61E07D550F80389793187FD5298AC00B";
                                                                   // string workingKey = "A3927F246CDE29AA3042DA0D3F8EA5D7"; // For testing
            string ccaRequest = "";
            string strEncRequest = "";
            string strAccessCode = "AVME70ED03AF40EMFA";//old gandhibagh-> "AVVA63DB90AK01AVKA";
                                                        //  string strAccessCode = "AVPU65DG96AH88UPHA";// For testing

            long MerchantID = 125345;// 89949;
            //string txnid = Generatetxnid();

            string ccaRequestSave = txnid;
            TempData["CCAvenueInformatn"] = ccaRequestSave;

            ccaRequest = "&merchant_id=" + MerchantID + "&order_id=" + txnid + "&amount=" + amount +
                "&currency=" + "INR" + "&redirect_url=" + surl + "&cancel_url=" + furl +
                "&billing_name=" + firstName + "&billing_address=" + Address + "&billing_city=" + city + "&billing_franchise=" + FranchiseID + ////added "&billing_franchise=" + FranchiseID + by Ashish for multiple MCO in same city
                "&billing_state=" + state + "&billing_zip=" + Pincode + "&billing_country=" + "India" + "&billing_tel=" + Mobile +
                "&billing_email=" + email;

            strEncRequest = ccaCrypto.Encrypt(ccaRequest, workingKey);
            myremotepost.Url = "https://secure.ccavenue.com/transaction/transaction.do?command=initiateTransaction&encRequest=" + strEncRequest + "&access_code=" + strAccessCode;
            myremotepost.Post();

        }


        public string GetUrl()
        {
            /*
               Indents:
             * Description: This method get location protocol
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */
            String strPathAndQuery = HttpContext.Request.Url.PathAndQuery;
            String strUrl = HttpContext.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");

            return strUrl;
        }


        public class RemotePost
        {
            private System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();


            public string Url = "";
            public string Method = "post";
            public string FormName = "form1";

            public void Add(string name, string value)
            {
                Inputs.Add(name, value);
            }

            public void Post()
            {
                
                System.Web.HttpContext.Current.Response.Clear();

                System.Web.HttpContext.Current.Response.Write("<html><head>");

                System.Web.HttpContext.Current.Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">", FormName));
                System.Web.HttpContext.Current.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));
                for (int i = 0; i < Inputs.Keys.Count; i++)
                {
                    System.Web.HttpContext.Current.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", Inputs.Keys[i], Inputs[Inputs.Keys[i]]));
                }
                System.Web.HttpContext.Current.Response.Write("</form>");
                System.Web.HttpContext.Current.Response.Write("</body></html>");

                System.Web.HttpContext.Current.Response.End();
            }
        }

        //Hash generation Algorithm

        public string Generatehash512(string text)
        {

            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;

        }

        public string Generatetxnid()
        {

            Random rnd = new Random();
            string x = rnd.Next(Int32.MaxValue).ToString();
            string strHash = Generatehash512(x + DateTime.Now);
            string txnid1 = strHash.ToString().Substring(0, 20);
            return txnid1;
        }
        //===========================================================================//

        private void SetOnlinePaymentCookie(string address, bool isExpresBuy)
        {
            HttpCookie OnlinePaymentCookie = new HttpCookie("OnlinePaymentCookie");

            //Delete whole cookie
            if (ControllerContext.HttpContext.Request.Cookies["OnlinePaymentCookie"] != null)
            {
                OnlinePaymentCookie.Expires = DateTime.Now.AddDays(-1);
                ControllerContext.HttpContext.Response.Cookies.Add(OnlinePaymentCookie);
            }
            if (OnlinePaymentCookie.Expires < DateTime.Now)
            {
                ControllerContext.HttpContext.Request.Cookies.Remove("OnlinePaymentCookie");
            }
            ControllerContext.HttpContext.Response.Cookies["OnlinePaymentCookie"].Value = address + "$" + isExpresBuy;
            ControllerContext.HttpContext.Request.Cookies.Add(OnlinePaymentCookie);
            OnlinePaymentCookie.Expires = System.DateTime.Now.AddDays(30);
        }

        //Yashaswi allow only selected franchise area new parameter selectedArea
        public JsonResult CheckSelectedPincode(string addressID, string IsExpressBuy, long? selectedArea)
        {

            int IDaddress = Convert.ToInt32(addressID);
            CustomerShippingAddress custAddress = new CustomerShippingAddress();
            int pincodeID = db.CustomerShippingAddresses.Where(x => x.ID == IDaddress).FirstOrDefault().PincodeID;
            string pincode = db.Pincodes.Where(x => x.ID == pincodeID).FirstOrDefault().Name;

            PincodeVerification pv = new PincodeVerification();
            //yashaswi
            ///New Yashaswi 20/6/2018
            int? NewArea = db.CustomerShippingAddresses.Where(x => x.ID == IDaddress).FirstOrDefault().AreaID;
            long NewAreaId = 0;
            if (NewArea != null)
            {
                NewAreaId = (long)NewArea;
            }
            //End
            long SelectedAreaId = selectedArea ?? 0;
            if (pv.IsDeliverableArea(NewAreaId, SelectedAreaId))
            {
                //End Yashaswi
                DeliveryCharges dc = new DeliveryCharges();
                ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();
                List<ShopWiseDeliveryCharges> listShopWiseDeliveryCharges = new List<ShopWiseDeliveryCharges>();

                DeliveryScheduleBLL lDeliveryScheduleBLL = new DeliveryScheduleBLL(System.Web.HttpContext.Current.Server);
                long cityId = Convert.ToInt64(Request.Cookies["CityCookie"].Value.Split('$')[0]);
                int franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city
                decimal totTax = 0;
                decimal exclusiveGstAmt = 0;



                if (IsExpressBuy == "True")
                {
                    lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["ExpressBuyCollection"];
                    //listShopWiseDeliveryCharges = dc.CalculateDeliveryCharge(lShoppingCartCollection, pincode, "Express");
                    decimal NetOrderAmount = lShoppingCartCollection.lShopWiseDeliveryCharges.Sum(t => t.OrderAmount);
                    decimal deliveryCharge = dc.GetDeliveryCharges(pincode, lShoppingCartCollection.lShopProductVarientViewModel[0].ActualWeight, true, NetOrderAmount);

                    lShoppingCartCollection.lShopWiseDeliveryCharges.FirstOrDefault().DeliveryCharge = deliveryCharge;
                    totTax = 0;
                    if (lShoppingCartCollection.lCalculatedTaxList != null && lShoppingCartCollection.lCalculatedTaxList.Count() > 0)
                    {
                        totTax = lShoppingCartCollection.lCalculatedTaxList.Sum(od => od.Amount);

                        // Added by Zubair for GST on 06-07-2017
                        //Calculating only exclusive GST Amount to add in Payable amount
                        exclusiveGstAmt = lShoppingCartCollection.lCalculatedTaxList.Where(x => x.IsGSTInclusive == false).Sum(x => x.Amount);
                        // End GST
                    }
                    //******* Added below Line to Avoid Negative Amount  **************//

                    //Commented and modified by Zubair for GST on 06-07-2017
                    //decimal checkAmout = lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount +
                    //        deliveryCharge - lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount + totTax -
                    //         lShoppingCartCollection.lShoppingCartOrderDetails.UsedEarnAmount;
                    //if (checkAmout >= 0)
                    //{
                    //    lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount = lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount +
                    //        deliveryCharge - lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount + totTax -
                    //         lShoppingCartCollection.lShoppingCartOrderDetails.UsedEarnAmount;
                    //}
                    //else { lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount = 0; }


                    decimal checkAmout = lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount +
                            deliveryCharge - lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount + exclusiveGstAmt -
                             lShoppingCartCollection.lShoppingCartOrderDetails.UsedEarnAmount - lShoppingCartCollection.lShoppingCartOrderDetails.WalletAmountUsed; //"-WalletAmountUsed" Added by Zubair for MLM on 31-01-2018;
                    if (checkAmout >= 0)
                    {
                        lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount = lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount +
                            deliveryCharge - lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount + exclusiveGstAmt -
                             lShoppingCartCollection.lShoppingCartOrderDetails.UsedEarnAmount - lShoppingCartCollection.lShoppingCartOrderDetails.WalletAmountUsed; //"-WalletAmountUsed" Added by Zubair for MLM on 31-01-2018;
                    }
                    else { lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount = 0; }
                    // End GST

                    //listShopWiseDeliveryCharges.FirstOrDefault().DeliveryCharge = deliveryCharge;

                    //==================== Add Delivery schedule Id (add first slot for express buy) ===============//

                    // List<DeliveryScheduleViewModel> lDeliverySchedule = lDeliveryScheduleBLL.SetDeliverySchedule(cityId, pincode);////hide by Ashish for multiple MCO in same city
                    List<DeliveryScheduleViewModel> lDeliverySchedule = lDeliveryScheduleBLL.SetDeliverySchedule(cityId, pincode, franchiseId);////added franchiseId by Ashish for multiple MCO in same city
                    lShoppingCartCollection.DeliveryScheduleID = lDeliverySchedule.FirstOrDefault().delScheduleId;

                }
                else
                {

                    lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["CartCollection"];
                    listShopWiseDeliveryCharges = dc.CalculateDeliveryCharge(lShoppingCartCollection, pincode, "Normal");
                    totTax = 0;
                    if (lShoppingCartCollection.lCalculatedTaxList != null && lShoppingCartCollection.lCalculatedTaxList.Count() > 0)
                    {
                        totTax = lShoppingCartCollection.lCalculatedTaxList.Sum(od => od.Amount);

                        // Added by Zubair for GST on 06-07-2017
                        //Calculating only exclusive GST Amount to add in Payable amount
                        exclusiveGstAmt = lShoppingCartCollection.lCalculatedTaxList.Where(x => x.IsGSTInclusive == false).Sum(x => x.Amount);
                        // End GST
                    }
                    //******* Added below Line to Avoid Negative Amount  **************//


                    //Commented and modified by Zubair for GST on 06-07-2017
                    //decimal checkAmout = lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount +
                    //    listShopWiseDeliveryCharges.FirstOrDefault().DeliveryCharge - lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount + totTax -
                    //    lShoppingCartCollection.lShoppingCartOrderDetails.UsedEarnAmount;
                    //if (checkAmout >= 0) {
                    //lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount = lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount +
                    //    listShopWiseDeliveryCharges.FirstOrDefault().DeliveryCharge - lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount + totTax -
                    //    lShoppingCartCollection.lShoppingCartOrderDetails.UsedEarnAmount;
                    //}
                    //else { lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount = 0; }
                    //lShoppingCartCollection.lShopWiseDeliveryCharges = listShopWiseDeliveryCharges;

                    decimal checkAmout = lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount +
                        listShopWiseDeliveryCharges.FirstOrDefault().DeliveryCharge - lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount + exclusiveGstAmt -
                        lShoppingCartCollection.lShoppingCartOrderDetails.UsedEarnAmount - lShoppingCartCollection.lShoppingCartOrderDetails.WalletAmountUsed; //"-WalletAmountUsed" Added by Zubair for MLM on 31-01-2018;
                    if (checkAmout >= 0)
                    {
                        lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount = lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount +
                            listShopWiseDeliveryCharges.FirstOrDefault().DeliveryCharge - lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount + exclusiveGstAmt -
                            lShoppingCartCollection.lShoppingCartOrderDetails.UsedEarnAmount - lShoppingCartCollection.lShoppingCartOrderDetails.WalletAmountUsed; //"-WalletAmountUsed" Added by Zubair for MLM on 31-01-2018;
                    }
                    else { lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount = 0; }
                    lShoppingCartCollection.lShopWiseDeliveryCharges = listShopWiseDeliveryCharges;
                    //End GST

                    //Verify delivery schedule for IIT Kanpur
                    if (pincode == "208016")
                    {
                        if (ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"] != null && ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"].Value != pincode)
                        {
                            //List<DeliveryScheduleViewModel> lDeliverySchedule = lDeliveryScheduleBLL.SetDeliverySchedule(cityId, pincode);////hide by Ashish for multiple MCO in same city
                            List<DeliveryScheduleViewModel> lDeliverySchedule = lDeliveryScheduleBLL.SetDeliverySchedule(cityId, pincode, franchiseId);////added franchiseId by Ashish for multiple MCO in same city
                            lShoppingCartCollection.DeliveryScheduleID = lDeliverySchedule.FirstOrDefault().delScheduleId;
                            TempData["DelSchedChangeMsg"] = "Your product will be delivered at " + lDeliverySchedule.FirstOrDefault().date;
                        }
                        else
                        {
                            TempData["DelSchedChangeMsg"] = "";
                        }
                    }
                    else
                    {
                        TempData["DelSchedChangeMsg"] = "";
                    }
                }

                //listShopWiseDeliveryCharges = dc.CalculateDeliveryCharge(lShoppingCartCollection, pincode, "Normal");
                //lShoppingCartCollection.lShopWiseDeliveryCharges = listShopWiseDeliveryCharges;

                if (IsExpressBuy == "True")
                {
                    TempData["ExpressBuyCollection"] = lShoppingCartCollection;
                }
                else
                {
                    TempData["CartCollection"] = lShoppingCartCollection;
                }
                //ViewBag.Cart = lShoppingCartCollection;
                TempData.Keep();
                return Json("true", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
            //return pv.IsDeliverablePincode(pincode) ? Json("true", JsonRequestBehavior.AllowGet) : Json("false", JsonRequestBehavior.AllowGet);

        }
        //Yashaswi 
        public JsonResult CheckSelectedAreaCode(long? NewAreaId, long? OldAreaId)
        {
            try
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                + Environment.NewLine + "AreaId " + NewAreaId + Environment.NewLine
                + "[PincodeVerification][IsVerifiedArea] SelectedAreaID" + OldAreaId,
                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                long OldAreaId_ = 0;
                if (OldAreaId == null)
                {
                    OldAreaId_ = 0;
                }
                else
                {
                    OldAreaId_ = Convert.ToInt64(OldAreaId);
                }

                long NewAreaId_ = 0;
                if (NewAreaId == null)
                {
                    NewAreaId_ = 0;
                }
                else
                {
                    NewAreaId_ = Convert.ToInt64(NewAreaId);
                }

                PincodeVerification pv = new PincodeVerification();
                bool result = pv.IsDeliverableArea(NewAreaId_, OldAreaId_);

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()

               + "[PincodeVerification][IsVerifiedArea] SelectedAreaID" + result,
               BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                  + Environment.NewLine + ex.Message + Environment.NewLine
                  + "[PincodeVerification][IsVerifiedArea]" + ex.InnerException,
                  BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult IsOutstationDelivery(int CustaddrID)
        {
            long cityId = Convert.ToInt64(Request.Cookies["CityCookie"].Value.Split('$')[0]);
            int franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city
            string pinCode = string.Empty;
            if (CustaddrID > 0)
            {
                //pinCode = db.CustomerShippingAddresses.Where(x => x.ID == CustaddrID).Select(x => x.Area.Pincode.Name).FirstOrDefault();
                pinCode = db.CustomerShippingAddresses.Where(x => x.ID == CustaddrID).Select(x => x.Pincode.Name).FirstOrDefault();
            }

            var IsSameCity = (from pin in db.Pincodes
                              where pin.Name == pinCode && pin.CityID == cityId
                              select pin).ToList();

            if (IsSameCity.Count() <= 0 || IsSameCity == null)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("true", JsonRequestBehavior.AllowGet);
            }
        }

        #region Static Cart

        private ShopProductVarientViewModelCollection OrderDetailsCart()
        {
           
            long userLoginID = 0; // 1
            decimal subscriptionAmount = 0;
            decimal totalDeliveryCharges = 0;
            long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);
            //ShoppingCartInitialization sci = new ShoppingCartInitialization();
            ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();
            try
            {
                string IsExpressBuy = string.Empty;
                if (Request.QueryString["IsExpressBuy"] != null && Request.QueryString["IsExpressBuy"] != string.Empty && Request.QueryString["IsExpressBuy"].ToLower() == "true")
                {
                    IsExpressBuy = "true";
                }
                int result = 0;

                SubscriptionCalculator.IsUserSubscribed(userLoginID, ref result);

                if (IsExpressBuy == "true")
                {
                    lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["ExpressBuyCollection"];
                    TempData.Keep("ExpressBuyCollection");
                }
                else
                {
                    lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["CartCollection"];
                    TempData.Keep("CartCollection");
                }

                if (result > 0)
                {



                    if (result == 100)
                    {
                        //subscription is on
                        List<OrderDetailsCartShopStock> lShopStocks = (from sc in lShoppingCartCollection.lShopProductVarientViewModel
                                                                       select new OrderDetailsCartShopStock { shopStockId = sc.ShopStockID, Quantity = sc.PurchaseQuantity }).ToList();
                        TempData["cartShopStockIdList"] = lShopStocks;

                        //var CartShopStockID = lShoppingCartCollection.lShopProductVarientViewModel.ShopStockID;
                        List<SubscribedDiscountOnCategoryViewModel> lSubscribedDiscountOnCategoryViewModel = SubscriptionCalculator.SubscriberDiscountOnCategory(userLoginID, lShopStocks);

                        List<SubscribedFacilityViewModel> lSubscribedFacilityViewModel = SubscriptionCalculator.SubscribedFacility(userLoginID);

                        lShoppingCartCollection.lSubscribedDiscountOnCategoryViewModel = lSubscribedDiscountOnCategoryViewModel;

                        foreach (var item in lSubscribedDiscountOnCategoryViewModel)
                        {
                            subscriptionAmount = subscriptionAmount + item.Amount;
                        }

                        foreach (var item in lShoppingCartCollection.lShopWiseDeliveryCharges)
                        {
                            totalDeliveryCharges = totalDeliveryCharges + item.DeliveryCharge;
                        }
                        //lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount = subscriptionAmount;
                        lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount = lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount - subscriptionAmount;
                        decimal facilityValue = 0;
                        if (lSubscribedFacilityViewModel.Count > 0)
                        {

                            //int merCount = lShoppingCartCollection.lShopProductVarientViewModel.Select(p => p.ShopID).Distinct().Count();
                            facilityValue = lSubscribedFacilityViewModel.Where(x => x.Name.Contains("Free Home Delivery")).Select(x => x.FacilityValue).FirstOrDefault();
                            //if (facilityValue > merCount || facilityValue == merCount)
                            if (facilityValue > 0)
                            {
                                TempData["IsFreeDelivery"] = true;
                                //lSubscribedFacilityViewModel.Where(x => x.Name.Contains("Free Home Delivery")).ToList().ForEach(y => y.FacilityValue = y.FacilityValue - merCount);
                                lSubscribedFacilityViewModel.Where(x => x.Name.Contains("Free Home Delivery")).ToList().ForEach(y => y.FacilityValue = y.FacilityValue - 1);
                                lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount = lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount - totalDeliveryCharges;
                                TempData["SubscribedFacility"] = lSubscribedFacilityViewModel;
                                //listOfCompany.Where(c => c.id == 1).ToList().ForEach(cc => cc.Name = "Whatever Name");
                                lShoppingCartCollection.lShopWiseDeliveryCharges.ForEach(p => p.DeliveryCharge = 0);
                            }
                            else
                            {
                                TempData["IsFreeDelivery"] = false;
                            }

                        }

                        if (subscriptionAmount > 0)
                        {
                            TempData["subscriptionAmount"] = subscriptionAmount;
                        }

                        if (IsExpressBuy == "true")
                            TempData["ExpressBuyCollection"] = lShoppingCartCollection;
                        else
                            TempData["CartCollection"] = lShoppingCartCollection;

                        TempData.Keep();
                    }
                    else if (result == 101)
                    {
                        //Subscription not started subscription started from this date
                    }
                    else if (result == 102)
                    {
                        //Subscription over
                    }
                    else if (result == 103)
                    {
                        //subscription not applicable.
                    }
                }






                //lShoppingCartCollection = sci.GetCookie();
                //if (ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"] != null)
                //{
                //    lShoppingCartCollection = sci.GetDeliveryCharge(lShoppingCartCollection);
                //}
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with loading shopping cart!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PaymentProcessController][GET:OrderDetailsCart]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with loading shopping cart!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][GET:OrderDetailsCart]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            //return View("_OrderDetailCart", lShoppingCartCollection);
            return lShoppingCartCollection;
        }

        private void InsertSubscriptionDetail(long orderId)
        {
            try
            {
                long userLoginID = 0; // 1
                long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);
                if (userLoginID > 0)
                {
                    int result = 0;
                    SubscriptionCalculator.IsUserSubscribed(userLoginID, ref result);
                    if (result == 100)
                    {
                        SubscriptionCalculator lSubscriptionCalculator = new SubscriptionCalculator(System.Web.HttpContext.Current.Server);

                        List<OrderDetailsCartShopStock> lShopStocks = (List<OrderDetailsCartShopStock>)TempData["cartShopStockIdList"];

                        List<SubscribedFacilityViewModel> lSubscribedFacility = (List<SubscribedFacilityViewModel>)TempData["SubscribedFacility"];

                        if (lSubscribedFacility != null)
                        {
                            lSubscriptionCalculator.InsertSubscriptionPlanAmountUsedBy(userLoginID, lShopStocks, orderId, lSubscribedFacility);
                        }
                        else
                        {
                            lSubscriptionCalculator.InsertSubscriptionPlanAmountUsedBy(userLoginID, lShopStocks, orderId, null);
                        }

                        if (TempData["subscriptionAmount"] != null || (bool)TempData.Peek("IsFreeDelivery") == true)
                        {
                            // var obj = db.UserLogins.Where(x => x.ID == userLoginID);
                            decimal subscriptionAmount = Convert.ToDecimal(TempData["subscriptionAmount"]);
                            try
                            {
                                SendSubscriptionMailToCustomer(userLoginID, subscriptionAmount, lSubscribedFacility.FirstOrDefault().FacilityValue, (bool)TempData.Peek("IsFreeDelivery"));
                            }
                            catch (Exception ex)
                            {
                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + ex.Message + Environment.NewLine
                                + "[PaymentProcessController][POST:CustomerPaymentProcess]",
                                BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                            }
                            try
                            {
                                SendSebscriptionSMSToCustomer(userLoginID, subscriptionAmount, lSubscribedFacility.FirstOrDefault().FacilityValue, (bool)TempData.Peek("IsFreeDelivery"));
                            }
                            catch (Exception ex)
                            {

                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + ex.Message + Environment.NewLine
                                + "[PaymentProcessController][POST:CustomerPaymentProcess]",
                                BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                            }

                            //SendSubscriptionMailToCustomer("tejaswee1.taktewale@gmail.com", subscriptionAmount);
                            //SendSebscriptionSMSToCustomer("9767117660", subscriptionAmount);
                        }
                    }
                    else
                    {
                        //Subscription plan not applicable/activated
                    }
                }
                else
                {
                    //User not logged in
                }
            }
            catch (Exception ex)
            {

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PaymentProcessController][POST:CustomerPaymentProcess]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }


        private void SendSubscriptionMailToCustomer(long UID, decimal subscriptionAmount, decimal balanceDelivery, bool IsFreeDelivery)
        {
            try
            {
                var obj = db.UserLogins.Where(x => x.ID == UID).FirstOrDefault();
                string emailId = obj.Email;
                var lPersonalDetail = db.PersonalDetails.Where(x => x.UserLoginID == UID).FirstOrDefault();

                OrderViewModel lOrderViewModel = (OrderViewModel)TempData.Peek("OrderDetail");

                string city = URLsFromConfig.GetDefaultData("CITY_NAME");//Yashaswi 01/12/2018 Default City Change 
                int franchiseId = Convert.ToInt32(URLsFromConfig.GetDefaultData("FRANCHISE_ID"));//Yashaswi 01/12/2018 Default City Change 
                if (System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value != null)
                {
                    city = System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                    franchiseId = Convert.ToInt32(System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city
                }

                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                //dictEmailValues.Add("<!--ACCOUNT_URL-->", "http://www.ezeelo.com/CustomerOrder/MyOrders");
                //dictEmailValues.Add("<!--ORDERS_URL-->", "http://customer.identical/CustomerOrder/MyOrders");
                dictEmailValues.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseId + "/login");////added  "/" + franchiseId + by Ashish for multiple MCO in same city
                dictEmailValues.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseId + "/cust-o/my-order"); ////added "/" + franchiseId +  by Ashish for multiple MCO in same city
                dictEmailValues.Add("<!--ORDER_NO-->", lOrderViewModel.CustomerOrder.OrderCode);
                dictEmailValues.Add("<!--ORDER_AMOUNT-->", lOrderViewModel.CustomerOrder.OrderAmount.ToString());
                dictEmailValues.Add("<!--SUBSCRIPTION_AMOUNT-->", subscriptionAmount.ToString());
                dictEmailValues.Add("<!--DELEVERY_COUNT-->", balanceDelivery.ToString());
                dictEmailValues.Add("<!--SALUTATION_NAME-->", lPersonalDetail.Salutation.Name);
                dictEmailValues.Add("<!--NAME-->", lPersonalDetail.FirstName);
                if (IsFreeDelivery)
                {
                    dictEmailValues.Add("<!--DELEVERY-->", "b)Free Delivery");
                }

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_NEW_SUBSCRIPTION, new string[] { emailId, "sales@ezeelo.com" }, dictEmailValues, true);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SendSebscriptionSMSToCustomer(long UID, decimal subscriptionAmount, decimal balanceDelivery, bool IsFreeDelivery)
        {
            try
            {
                var obj = db.UserLogins.Where(x => x.ID == UID).FirstOrDefault();
                string mobileNo = obj.Mobile;

                CommonFunctions cf = new CommonFunctions();

                Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();

                dictSMSValues.Add("#--SUBSCRIPTION_AMT--#", subscriptionAmount.ToString());

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_SUBSCR_ORDER_PLACE, new string[] { mobileNo }, dictSMSValues);

            }
            catch (Exception ex)
            {

            }
        }

        public ActionResult LoadCart(string IsExpressBuy)
        {
            //string IsExpressBuy = string.Empty;
            ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();
            //if (Request.QueryString["IsExpressBuy"] != null && Request.QueryString["IsExpressBuy"] != string.Empty)
            //{
            //    IsExpressBuy = "true";
            //}
            //if (IsExpressBuy == "true" || IsExpressBuy.ToUpper() == "TRUE")
            //    lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["ExpressBuyCollection"];
            //else
            //    lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["CartCollection"];
            lShoppingCartCollection = this.OrderDetailsCart();
            TempData.Keep();
            return PartialView("_OrderDetailCart", lShoppingCartCollection);
            //return View();
        }




        #endregion

        //Pradnyakar sir's function use for saving delivery schedule
        //private void SaveDeliveryScheduleDetail(long UID, long customerOrderID, string deliveryScheduleID)
        //{
        //    try
        //    {
        //        string[] scheduleDetail = deliveryScheduleID.Split('$');
        //        OrderDeliveryScheduleDetail lOrderDeliveryScheduleDetail = new OrderDeliveryScheduleDetail();

        //        var lPersonalDetail = db.PersonalDetails.Where(x => x.UserLoginID == UID).FirstOrDefault();

        //        lOrderDeliveryScheduleDetail.CreateBy = lPersonalDetail.ID;
        //        lOrderDeliveryScheduleDetail.CreateDate = DateTime.UtcNow;
        //        lOrderDeliveryScheduleDetail.CustomerOrderID = customerOrderID;
        //        //lOrderDeliveryScheduleDetail.DeliveryScheduleID = deliveryScheduleID;
        //        lOrderDeliveryScheduleDetail.DeliveryScheduleID = Convert.ToInt64(scheduleDetail[0]);//delivery schedule id
        //        lOrderDeliveryScheduleDetail.DeliveryDate = Convert.ToDateTime(scheduleDetail[1]);

        //        lOrderDeliveryScheduleDetail.IsActive = true;
        //        lOrderDeliveryScheduleDetail.ModifyDate = null;
        //        lOrderDeliveryScheduleDetail.ModifyBy = null;
        //        lOrderDeliveryScheduleDetail.NetworkIP = CommonFunctions.GetClientIP();
        //        lOrderDeliveryScheduleDetail.DeviceType = "x";
        //        lOrderDeliveryScheduleDetail.DeviceID = "x";

        //        db.OrderDeliveryScheduleDetails.Add(lOrderDeliveryScheduleDetail);
        //        db.SaveChanges();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}



    }
}