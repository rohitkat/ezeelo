using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using PagedList;
using PagedList.Mvc;
using CRM.Models;
using CRM.Models.ViewModel;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
//Added
using System.Web.UI;
using System.Data.Entity.Validation;



namespace CRM.Controllers
{
    public class CustomerOrderDetailCallController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            customerCareSessionViewModel.Username = Session["UserName"].ToString();
            Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel);
        }

        //[SessionExpire]
        // GET: /CustomerOrderDetailCall/
        public ActionResult Index(long ShopID, long DeliveryPartnerID, int OrderStatus, string ShopOrderCode, Boolean? IsPosted)
        {
            //SessionDetails();
            var customerorderdetailcalls = db.CustomerOrderDetailCalls.Include(c => c.BusinessType).Include(c => c.PersonalDetail).Where(x => x.ShopOrderCode.Equals(ShopOrderCode)).ToList().OrderByDescending(x => x.ID);

            Dictionary<long, string> CallTo = new Dictionary<long, string>();
            foreach (CustomerOrderDetailCall customerOrderDetailCall in customerorderdetailcalls)
            {
                switch (customerOrderDetailCall.BusinessTypeID)
                { 
                    case (int)Common.Constant.BusinessType.MERCHANT_SHOP:
                        CallTo.Add(customerOrderDetailCall.ID, "Merchant, " + db.Shops.FirstOrDefault(x => x.ID == customerOrderDetailCall.OwnerID).Name);
                        break;
                    case (int)Common.Constant.BusinessType.DELIVERY_PARTNER:
                        CallTo.Add(customerOrderDetailCall.ID, "Delivery, " + db.DeliveryPartners.FirstOrDefault(x => x.ID == customerOrderDetailCall.OwnerID).ContactPerson);//hide //Open EPOD from Ashish for Live
                        //-- Add by Ashish --//
                        //Hide EPOD from Ashish for Live
                       /*int count = db.DeliveryPartners.Where(x => x.ID == customerOrderDetailCall.OwnerID).Count();
                        if (count == 0)
                            CallTo.Add(customerOrderDetailCall.ID, "Delivery, " +"");
                        else
                        CallTo.Add(customerOrderDetailCall.ID, "Delivery, " + db.DeliveryPartners.FirstOrDefault(x => x.ID == customerOrderDetailCall.OwnerID).ContactPerson);
                        */
                       //-- End --//
                        break;
                }
            }
            ViewBag.CallToList = CallTo;
            if (IsPosted != null)
            {
                //Response.Write("<script>parent.location.reload();</script>");
                IsPosted = null;
            }
            return View(customerorderdetailcalls.ToList());
        }

        //---------------------------------------Hide EPOD from Ashish for Live----------------------------------------------------------
       /* public JsonResult IsOTPAvailable(string OTP)
        {
            string OrderCode = TempData["OrderCode"].ToString();
            TempData.Keep();
            //string OrderCode = Request.QueryString["OrderCode"].ToString();

            return Json(db.OTPs.Where(x => x.OrderCode == OrderCode && x.OTP1 == OTP).Count() > 0 ? db.OTPs.Any(y => y.OrderCode == OrderCode && y.OTP1 == OTP) : false
               , JsonRequestBehavior.AllowGet);
        }*/

        //-----------------------------------------------------------------------------------------------------------------------------
        [SessionExpire]
        // GET: /CustomerOrderDetailCall/Create
        public ActionResult Create(long ShopID, long DeliveryPartnerID, int OrderStatus, string ShopOrderCode)
        {
            // Add By Ashish Nagrale //
            // Hide from Ashish for Live
           /* string OrderCode = Request.QueryString["OrderCode"].ToString();
            TempData["OrderCode"] = OrderCode;
            TempData.Keep();
            // string GetDbOTP = db.OTPs.Where(x => x.OrderCode == OrderCode).Count() > 0 ? db.OTPs.Where(x => x.OrderCode == OrderCode).FirstOrDefault().OTP1 : "OTP Not Generated"; // hide now
            string GetDbOTP = db.OTPs.Where(x => x.ShopOrderCode == ShopOrderCode).Count() > 0 ? db.OTPs.Where(x => x.ShopOrderCode == ShopOrderCode).FirstOrDefault().OTP1 : "OTP Not Generated";//added
            if (OrderStatus == 7)
            { 
                ViewBag.lblOTP = GetDbOTP;// "OTPttt";
                ViewBag.lblOtpValidation = "";
            }
            else
            {
                ViewBag.lblOTP = GetDbOTP;
                ViewBag.lblOtpValidation = "";
            }
            */
            // End Add //

            SessionDetails();
            ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes.Where(x => x.ID == (int)Common.Constant.BusinessType.MERCHANT_SHOP || 
                                                                                x.ID == (int)Common.Constant.BusinessType.DELIVERY_PARTNER), "ID", "Name");
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");

            var StatusList = from CRM.Common.Constant.ORDER_STATUS d in Enum.GetValues(typeof(CRM.Common.Constant.ORDER_STATUS))
                         select new { ID = (int)d, Name = d.ToString() };

            if(OrderStatus == (int)Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING)
            {
                StatusList = StatusList.Where(x => x.ID == (int)Common.Constant.ORDER_STATUS.PLACED || x.ID == (int)Common.Constant.ORDER_STATUS.CANCELLED).ToList();
            }
            else
            {
                StatusList = StatusList.Where(x => x.ID != (int)Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING).ToList();
            }
            ViewBag.OrderStatus = new SelectList(StatusList, "ID", "Name", OrderStatus);

            return View();
        }

        // POST: /CustomerOrderDetailCall/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[SessionExpire]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(long ShopID, long DeliveryPartnerID, int OrderStatus, string ShopOrderCode, string sendSMSToCustomer, [Bind(Include = "OrderStatus,BusinessTypeID,Description,OTP")] CustomerOrderDetailCall customerorderdetailcall)
        {
            SessionDetails();
            var StatusList = from CRM.Common.Constant.ORDER_STATUS d in Enum.GetValues(typeof(CRM.Common.Constant.ORDER_STATUS))
                             select new { ID = (int)d, Name = d.ToString() };

            if (OrderStatus == (int)Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING)
            {
                StatusList = StatusList.Where(x => x.ID == (int)Common.Constant.ORDER_STATUS.PLACED || x.ID == (int)Common.Constant.ORDER_STATUS.CANCELLED).ToList();
            }
            else
            {
                StatusList = StatusList.Where(x => x.ID != (int)Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING).ToList();
            }

            ViewBag.OrderStatus = new SelectList(StatusList, "ID", "Name", OrderStatus);
            try
            {
                
                //-- Add by Ashish Nagrale --//
                // Hide from Ashish for Live
                /*string PaymentMode = Request.QueryString["PaymentMode"].ToString(); 
                string OrderCode = Request.QueryString["OrderCode"].ToString();*/
                //-- End Add --//
                List<CustomerOrderDetail> customerOrderDetails = db.CustomerOrderDetails.Where(x => x.ShopOrderCode == ShopOrderCode).ToList();

                foreach (CustomerOrderDetail customerOrderDetail in customerOrderDetails)
                {
                    CustomerOrderDetail lCustomerOrderDetail = new CustomerOrderDetail();
                    lCustomerOrderDetail.ID = customerOrderDetail.ID;
                    lCustomerOrderDetail.ShopOrderCode = customerOrderDetail.ShopOrderCode;
                    lCustomerOrderDetail.CustomerOrderID = customerOrderDetail.CustomerOrderID;
                    lCustomerOrderDetail.ShopStockID = customerOrderDetail.ShopStockID;
                    lCustomerOrderDetail.ShopID = customerOrderDetail.ShopID;
                    lCustomerOrderDetail.Qty = customerOrderDetail.Qty;
                    //lCustomerOrderDetail.OrderStatus = customerOrderDetail.OrderStatus;
                    lCustomerOrderDetail.MRP = customerOrderDetail.MRP;
                    lCustomerOrderDetail.SaleRate = customerOrderDetail.SaleRate;
                    lCustomerOrderDetail.OfferPercent = customerOrderDetail.OfferPercent;
                    lCustomerOrderDetail.OfferRs = customerOrderDetail.OfferRs;
                    lCustomerOrderDetail.IsInclusivOfTax = customerOrderDetail.IsInclusivOfTax;
                    lCustomerOrderDetail.TotalAmount = customerOrderDetail.TotalAmount;
                    lCustomerOrderDetail.IsActive = customerOrderDetail.IsActive;
                    lCustomerOrderDetail.CreateDate = customerOrderDetail.CreateDate;
                    lCustomerOrderDetail.CreateBy = customerOrderDetail.CreateBy;
                    //lCustomerOrderDetail.ModifyDate = customerOrderDetail.ModifyDate;
                    //lCustomerOrderDetail.ModifyBy = customerOrderDetail.ModifyBy;
                    lCustomerOrderDetail.NetworkIP = customerOrderDetail.NetworkIP;
                    lCustomerOrderDetail.DeviceType = customerOrderDetail.DeviceType;
                    lCustomerOrderDetail.DeviceID = customerOrderDetail.DeviceID;

                    //----------------- extra 
                    lCustomerOrderDetail.OrderStatus = OrderStatus;
                    lCustomerOrderDetail.ModifyDate = DateTime.Now;
                    lCustomerOrderDetail.ModifyBy = customerCareSessionViewModel.PersonalDetailID;
                    EzeeloDBContext db1 = new EzeeloDBContext();
                    db1.Entry(lCustomerOrderDetail).State = EntityState.Modified;
                    db1.SaveChanges();



                    //----------------------------------- Insert into CustomerOrderHistory -//
                    //EzeeloDBContext db2 = new EzeeloDBContext();
                    CustomerOrderHistory lCustomerOrderHistory = new CustomerOrderHistory();
                    lCustomerOrderHistory.CustomerOrderID = lCustomerOrderDetail.CustomerOrderID;
                    lCustomerOrderHistory.ShopStockID = lCustomerOrderDetail.ShopStockID;
                    lCustomerOrderHistory.Status = lCustomerOrderDetail.OrderStatus;
                    lCustomerOrderHistory.CreateBy = customerCareSessionViewModel.PersonalDetailID;
                    lCustomerOrderHistory.CreateDate = DateTime.Now;
                    db1.CustomerOrderHistories.Add(lCustomerOrderHistory);
                    db1.SaveChanges();
                    db1.Dispose();
                }

                switch (customerorderdetailcall.BusinessTypeID)
                {
                    case (int)Common.Constant.BusinessType.MERCHANT_SHOP:
                        customerorderdetailcall.OwnerID = ShopID;
                        break;
                    case (int)Common.Constant.BusinessType.FRANCHISE:
                        break;
                    case (int)Common.Constant.BusinessType.DELIVERY_PARTNER:
                        customerorderdetailcall.OwnerID = DeliveryPartnerID;
                        break;
                }
                // OrderStatus = 3;
                //----------------New Code for  GCM Alert  date 11-Nov-2016 added by Ashwini Meshram-----------------------------------------------------//
                int Status=0;
                string lOrderCode;
                long lCustomerOrderID,lCustLoginID;
                string DeviceType = string.Empty;
                DeviceType = customerOrderDetails.FirstOrDefault(x => x.ShopOrderCode == ShopOrderCode).CustomerOrder.DeviceType;
                if (DeviceType == "Mobile" && (OrderStatus == (int)Common.Constant.ORDER_STATUS.CONFIRM || OrderStatus == (int)Common.Constant.ORDER_STATUS.DELIVERED || OrderStatus == (int)Common.Constant.ORDER_STATUS.CANCELLED))
                {
                    if (ModelState.IsValid)
                    {
                         try
                         {
                             lOrderCode = customerOrderDetails.FirstOrDefault().CustomerOrder.OrderCode;
                             lCustomerOrderID = customerOrderDetails.FirstOrDefault().CustomerOrder.ID;
                             lCustLoginID = customerOrderDetails.FirstOrDefault().CustomerOrder.UserLoginID;
                             //lCustLoginID = 186713;
                             customerorderdetailcall.ShopOrderCode = ShopOrderCode;
                             customerorderdetailcall.OrderStatus = OrderStatus;
                             customerorderdetailcall.CreateBy = customerCareSessionViewModel.PersonalDetailID;
                             customerorderdetailcall.CreateDate = DateTime.Now;
                             //------------------------------To save data into CustomerOrderDetailCalls call table----------------------------// 
                             db.CustomerOrderDetailCalls.Add(customerorderdetailcall);
                             db.SaveChanges();
                             //----------------------------------------------------------------------------------------------//


                             //-----------------------------Code to send data to Customer from data base date 11-Nov-2016----------------------
                             if (OrderStatus != 0)
                             {
                                 DateTime CreateDate = DateTime.Now;
                                 OrderStatusEnum OrderStatusEnumVal = GetEnumValue<OrderStatusEnum>(OrderStatus);
                                 BusinessLogicLayer.OrderStatusSMSandEMAIL lOrderStatusSMSandEMAIL = new BusinessLogicLayer.OrderStatusSMSandEMAIL();
                                 lOrderStatusSMSandEMAIL.GCMMsgTemplate(Convert.ToString(OrderStatusEnumVal), lOrderCode, lCustomerOrderID, CreateDate);
                                 if (lCustLoginID != 0)
                                 {
                                     Status = lOrderStatusSMSandEMAIL.SendNotification(lCustLoginID, Convert.ToString(OrderStatusEnumVal));
                                 }

                             }
                         }
                         catch
                         {
                             throw;
                         }
                            //--------------------------------------------------------------------------------------------//
                            //------------------------ refresh parent window -//
                             //Response.Write("<script>parent.location.reload();</script>");
                            //ViewBag.Status = Status;
                            return RedirectToAction("Index", new { ShopID = ShopID, DeliveryPartnerID = DeliveryPartnerID, OrderStatus = OrderStatus, ShopOrderCode = ShopOrderCode, Isposted = true });

                        }
                        ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes.Where(x => x.ID != (int)Common.Constant.BusinessType.FRANCHISE), "ID", "Name");
                        ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorderdetailcall.CreateBy);
                        return View(customerorderdetailcall);
             
                }
               
                //------------------------------------------------------old code----------------------------------------------------//
                else
                 {
                    if (ModelState.IsValid)
                    {
                        customerorderdetailcall.ShopOrderCode = ShopOrderCode;
                        customerorderdetailcall.OrderStatus = OrderStatus;
                        customerorderdetailcall.CreateBy = customerCareSessionViewModel.PersonalDetailID;
                        customerorderdetailcall.CreateDate = DateTime.Now;
                        db.CustomerOrderDetailCalls.Add(customerorderdetailcall);
                        db.SaveChanges();

                        //--------------------------------- SMS & Email Function call --------------------------------//
                        if (OrderStatus == (int)Common.Constant.ORDER_STATUS.CONFIRM ||
                            OrderStatus == (int)Common.Constant.ORDER_STATUS.PACKED ||
                            OrderStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_GODOWN ||
                            OrderStatus == (int)Common.Constant.ORDER_STATUS.DELIVERED ||
                            OrderStatus == (int)Common.Constant.ORDER_STATUS.CANCELLED ||
                            OrderStatus == (int)Common.Constant.ORDER_STATUS.RETURNED)
                        {

                            string lEmail = customerOrderDetails.FirstOrDefault().CustomerOrder.UserLogin.Email;
                            string lFirstName = "";
                            long lUserLoginID = customerOrderDetails.FirstOrDefault().CustomerOrder.UserLoginID;

                            PersonalDetail lPersonalDetail = db.PersonalDetails.FirstOrDefault(x => x.UserLoginID == lUserLoginID);
                            if (lPersonalDetail != null)
                            {
                                lFirstName = lPersonalDetail.FirstName;
                            }

                            string lShopOrderCode = customerOrderDetails.FirstOrDefault().ShopOrderCode;
                            string lOrderDate = customerOrderDetails.FirstOrDefault().CreateDate.ToString("MMM dd, yyyy");
                            string lMobile = customerOrderDetails.FirstOrDefault().CustomerOrder.UserLogin.Mobile;
                            string lOrderAmount = customerOrderDetails.FirstOrDefault().CustomerOrder.OrderAmount.ToString();
                            string firstItemName = customerOrderDetails.FirstOrDefault().ShopStock.ShopProduct.Product.Name;
                            int lProductCount = customerOrderDetails.Count();

                            long lShopId = customerOrderDetails.FirstOrDefault().ShopID;

                            //lEmail = "avirakeshverma@gmail.com";
                            //lMobile = "9028415132";
                            //lEmail = "nagraleashish@yahoo.com";
                            //lMobile = "9350507296";
                            //------Declartion Email---------//
                            string city = "nagpur";
                            int franchiseID = 2;////added for Multiple MCO in Same City
                            if (System.Web.HttpContext.Current.Request.Cookies["CityCookie"] != null && System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value != "")
                            {
                                city = System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                                franchiseID = Convert.ToInt32(System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[2]); ////added for Multiple MCO in Same City
                            }

                            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                            BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                            dictionary.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseID + "/login");////added "/" + franchiseID + for Multiple MCO in Same City
                            dictionary.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseID + "/cust-o/my-order");////added "/" + franchiseID + for Multiple MCO in Same City


                            bool Flag = false;
                            //-- Start Add By Ashish Nagrale --//
                            // Hide from Ashish for Live
                            /*  Dictionary<string, string> SmsEmailOtp = new Dictionary<string, string>();
                              try
                              {
                                  // Sending email to the user
                                  SmsEmailOtp = BusinessLogicLayer.OTP.GenerateOTP("CRM");//CDOTP

                              }
                              catch (BusinessLogicLayer.MyException myEx)
                              {
                                  ModelState.AddModelError("Message", "Customer Order/OTP Confirm Succesfully, there might be problem sending SMS, please check your mobile or contact administrator!");//added
                                  BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send SMS..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH, BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                                  // throw new Exception("Unable to Send Email");
                              }*/
                            //-- End Add --//
                            try
                            {
                                //------Key value add in Email---------//
                                if (lFirstName != "")
                                {
                                    dictionary.Add("<!--NAME-->", lFirstName);
                                }
                                else
                                {
                                    dictionary.Add("<!--NAME-->", lEmail);
                                }
                                //dictionary.Add("<!--ORDER_NO-->", lShopOrderCode);
                                //dictionary.Add("<!--ORDER_DATE-->", lOrderDate);


                                if (OrderStatus == (int)Common.Constant.ORDER_STATUS.CONFIRM)
                                {
                                    string subject = (lProductCount == 1 ? "Your order #" + lShopOrderCode + " for " + firstItemName + " has been placed on the merchant" : "Your order #" + lShopOrderCode + " fulfilled ");

                                    dictionary.Add("<!--ORDER_NO-->", lShopOrderCode);
                                    dictionary.Add("<!--ORDER_DATE-->", lOrderDate);
                                    dictionary.Add("#--SUBJECT--#", subject);
                                    Flag = gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_ORD_CNF, new string[] { lEmail, rcKey.DEFAULT_ALL_EMAIL }, dictionary, true);
                                }
                                else if (OrderStatus == (int)Common.Constant.ORDER_STATUS.PACKED)
                                {

                                    using (var dbContextTransaction = db.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            //-----------Code of delivery Partner change----------------//

                                            // copied as it is from merchant module common class as per suggested by Mohit Sinha 

                                            long MerchentId = ShopID; //GetShopID();

                                            DeliveryOrderDetail DOD = db.DeliveryOrderDetails.Single(x => x.ShopOrderCode == lShopOrderCode);

                                            int lDeliveryPartnerId = DOD.DeliveryPartnerID;
                                            Boolean lIsShopHandleDeliveryProcess = Common.Common.IsShopHandleDeliveryProcess(MerchentId, ref lDeliveryPartnerId);

                                            if (lIsShopHandleDeliveryProcess == false)
                                            {
                                                decimal DeliveryCharges = Common.Common.GetDeliveryCharges(lDeliveryPartnerId, Convert.ToDecimal(0), DOD.DeliveryType, DOD.Weight);
                                                DOD.DeliveryPartnerID = lDeliveryPartnerId;
                                                DOD.DeliveryCharge = DeliveryCharges;
                                            }

                                            DOD.IsActive = true;
                                            DOD.ModifyDate = DateTime.UtcNow;
                                            DOD.ModifyBy = customerCareSessionViewModel.UserLoginID; // GetPersonalDetailID();
                                            DOD.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                            DOD.DeviceType = "x";
                                            DOD.DeviceID = "x";
                                            db.Entry(DOD).State = EntityState.Modified;
                                            db.SaveChanges();
                                            dbContextTransaction.Commit();

                                        }
                                        catch (BusinessLogicLayer.MyException myEx)
                                        {
                                            dbContextTransaction.Rollback();
                                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                                + "[ConfirmController][POST:Packed]" + myEx.EXCEPTION_PATH,
                                                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                                        }
                                        catch (Exception ex)
                                        {
                                            dbContextTransaction.Rollback();
                                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                + Environment.NewLine + ex.Message + Environment.NewLine
                                                + "[ConfirmController][POST:Packed]",
                                                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                                        }
                                    }
                                    //Flag = gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_ORD_DLVRD, new string[] { lEmail }, dictionary, true);
                                }
                                else if (OrderStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_GODOWN)
                                {
                                    //-- Add By Ashish Nagrale --//
                                    //dictionary.Add("<!--ORDER_NO-->", OrderCode); //lShopOrderCode close  Hide from Ashish for Live
                                    dictionary.Add("<!--ORDER_NO-->", lShopOrderCode); //lShopOrderCode open old code
                                    dictionary.Add("<!--ORDER_DATE-->", lOrderDate);
                                    dictionary.Add("#--TIME--#", DateTime.Now.ToShortTimeString());
                                    ////dictionary.Add("#--D_OTP--#", SmsEmailOtp["OTP"]); // Hide from Ashish for Live
                                    // End Add --//

                                    //===== added by Tejaswee ======//
                                    //=======Get Payment mode from customerOrder table ==============//
                                    long custOrderID = customerOrderDetails.FirstOrDefault().CustomerOrderID;
                                    string payMode = db.CustomerOrders.Where(x => x.ID == custOrderID).Select(x => x.PaymentMode).FirstOrDefault();
                                    //dictionary.Add("#--TIME--#", DateTime.Now.ToShortTimeString());

                                    if (payMode == "COD")
                                    {
                                        dictionary.Add("#--AMOUNT--#", lOrderAmount);
                                        Flag = gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_ORD_DSPTCH_COD, new string[] { lEmail }, dictionary, true);
                                    }
                                    else
                                    {
                                        Flag = gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_ORD_DSPTCH_COD_ONLINE, new string[] { lEmail }, dictionary, true);
                                    }
                                    //End Add
                                }
                                else if (OrderStatus == (int)Common.Constant.ORDER_STATUS.DELIVERED)
                                {
                                    string subject = (lProductCount == 1 ? "Your order #" + lShopOrderCode + " for " + firstItemName + " has been delivered" : "Your order #" + lShopOrderCode + " for " + firstItemName + " + " + (lProductCount - 1) + " more item/s has been delivered");

                                    dictionary.Add("<!--ORDER_NO-->", lShopOrderCode);
                                    dictionary.Add("<!--ORDER_DATE-->", lOrderDate);
                                    dictionary.Add("#--SUBJECT--#", subject);

                                    System.Text.StringBuilder sbHtml = new System.Text.StringBuilder(
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


                                    string productName = string.Empty;
                                    try
                                    {
                                        productName = customerOrderDetails.FirstOrDefault().ShopStock.ShopProduct.Product.Name.ToString();
                                    }
                                    catch { productName = "Not Available"; }

                                    string productQty = string.Empty;
                                    try
                                    {
                                        productQty = customerOrderDetails.FirstOrDefault().Qty.ToString();
                                    }
                                    catch { productQty = "Not Available"; }

                                    string delStatus = string.Empty;
                                    try
                                    {
                                        delStatus = customerOrderDetails.FirstOrDefault().OrderStatus.ToString();
                                    }
                                    catch { delStatus = "Not Available"; }

                                    sbHtml.AppendFormat(
                                            "<tr>" +
                                            "<th style=\"border: 1px solid #b8b8b7; border-right:none; border-bottom:none;\">{0}</th>" +
                                            "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{1}</th>" +
                                            "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{2}</th>" +
                                            "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{3}</th>" +
                                            "</tr>", "No Preffered Time Available", productName, productQty, delStatus
                                             );

                                    sbHtml.Append("</tbody></table>");
                                    //string 
                                    subject = (lProductCount == 1 ? "Your order #" + lShopOrderCode + " for " + firstItemName + " has been placed on the merchant" : "Your order #" + lShopOrderCode + " for " + firstItemName + " + " + (lProductCount - 1) + " more item/s has been placed on the merchant");
                                    dictionary.Add("#--SUBJECT--#", subject);
                                    //---Added by mohit on 23-01-16 for help line number as per city---//
                                    List<CityHelpLineNoViewModel> lCitlHelpLine = new List<CityHelpLineNoViewModel>();
                                    lCitlHelpLine = BusinessLogicLayer.CityHelpLineNo.GetCityHelpLineNo(lShopId).ToList();
                                    dictionary.Add("<!--HELP_DESK-->", lCitlHelpLine[0].HelpLineNumber);
                                    //---End Of Code By Mohit---//
                                    Flag = gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_ORD_DLVRD, new string[] { lEmail, rcKey.DEFAULT_ALL_EMAIL }, dictionary, true);
                                }
                                else if (OrderStatus == (int)Common.Constant.ORDER_STATUS.CANCELLED)
                                {
                                    dictionary.Add("<!--ORDER_NO-->", lShopOrderCode);
                                    dictionary.Add("<!--ORDER_DATE-->", customerOrderDetails.FirstOrDefault().CreateDate.ToString("MMM dd, yyyy"));
                                    dictionary.Add("<!--ORDER_TIME-->", customerOrderDetails.FirstOrDefault().CreateDate.ToShortTimeString());

                                    Flag = gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_ORD_CANCELLED_REPLY, new string[] { lEmail, rcKey.DEFAULT_ALL_EMAIL }, dictionary, true);
                                }
                                else if (OrderStatus == (int)Common.Constant.ORDER_STATUS.RETURNED)
                                {
                                    dictionary.Add("<!--ORDER_NO-->", lShopOrderCode);
                                    dictionary.Add("<!--ORDER_DATE-->", lOrderDate);

                                    Flag = gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_ORD_RTND, new string[] { lEmail, rcKey.DEFAULT_ALL_EMAIL }, dictionary, true);
                                }

                            }
                            catch (Exception ex)
                            {
                            }
                            //------Declartion SMS---------//
                            gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                            dictionary.Clear();
                            Flag = false;
                            try
                            {
                                if (sendSMSToCustomer != null) //hide
                                {
                                    //------Key value add in SMS---------//
                                    if (lFirstName != "")
                                    {
                                        dictionary.Add("#--NAME--#", lFirstName);
                                    }
                                    else
                                    {
                                        dictionary.Add("#--NAME--#", lEmail);
                                    }

                                    dictionary.Add("#--ORD_NUM--#", lShopOrderCode);

                                    if (OrderStatus == (int)Common.Constant.ORDER_STATUS.CONFIRM)
                                    {
                                        //Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_CNF, new string[] { lMobile, rcKey.DEFAULT_ALL_SMS }, dictionary);
                                        Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_CNF, new string[] { lMobile }, dictionary);
                                    }
                                    else if (OrderStatus == (int)Common.Constant.ORDER_STATUS.PACKED)
                                    {
                                        //Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_DLVRD, new string[] { lMobile }, dictionary);
                                    }
                                    else if (OrderStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_GODOWN)
                                    {
                                        //-- Add By Ashish Nagrale --//
                                        //// dictionary.Add("#--TIME--#", DateTime.Now.ToShortTimeString());// Not use in SMS
                                        // Hide from Ashish for Live
                                        /* dictionary.Add("#--D_OTP--#", SmsEmailOtp["OTP"]);
                                         BusinessLogicLayer.OTP insertOtp = new BusinessLogicLayer.OTP();
                                         insertOtp.InsertOTPDetails(SmsEmailOtp["USC"], SmsEmailOtp["OTP"], lShopOrderCode);//added OrderCode now replace with lShopOrderCode
                                         */
                                        //-- End Add --//

                                        //===== added by Tejaswee ======//
                                        //=======Get Payment mode from customerOrder table ==============//
                                        long custOrderID = customerOrderDetails.FirstOrDefault().CustomerOrderID;
                                        string payMode = db.CustomerOrders.Where(x => x.ID == custOrderID).Select(x => x.PaymentMode).FirstOrDefault();
                                        //dictionary.Add("#--TIME--#", DateTime.Now.ToShortTimeString());

                                        if (payMode == "COD")
                                        {
                                            dictionary.Add("#--AMOUNT--#", lOrderAmount);
                                            //Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_DSPTCH_COD, new string[] { lMobile, rcKey.DEFAULT_ALL_SMS }, dictionary);
                                            Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_DSPTCH_COD, new string[] { lMobile }, dictionary);
                                        }
                                        else
                                        {
                                            //Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_DSPTCH_COD, new string[] { lMobile, rcKey.DEFAULT_ALL_SMS }, dictionary);
                                            Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_DSPTCH_COD, new string[] { lMobile }, dictionary);
                                        }
                                    }
                                    else if (OrderStatus == (int)Common.Constant.ORDER_STATUS.DELIVERED)
                                    {
                                        //Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_DLVRD, new string[] { lMobile, rcKey.DEFAULT_ALL_SMS }, dictionary);
                                        Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_DLVRD, new string[] { lMobile }, dictionary);
                                    }
                                    else if (OrderStatus == (int)Common.Constant.ORDER_STATUS.CANCELLED)
                                    {
                                        //Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_CAN, new string[] { lMobile, rcKey.DEFAULT_ALL_SMS }, dictionary);
                                        Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_CAN, new string[] { lMobile }, dictionary);
                                        //Flag = gateWay.SendSMS(GateWay.SMSGateWays.SUMIT, GateWay.SMSOptions.SINGLE, GateWay.SMSTypes.MER_ORD_CAN, new string[] { lShopDetails.FirstOrDefault().Mobile, readConfig.CRM_MOBILE, "9422149985" }, dictSMSValues);
                                    }
                                    else if (OrderStatus == (int)Common.Constant.ORDER_STATUS.RETURNED)
                                    {
                                        //Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_RTRND, new string[] { lMobile, rcKey.DEFAULT_ALL_SMS }, dictionary);
                                        Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_RTRND, new string[] { lMobile }, dictionary);
                                    }
                                }//hide
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        //--------------------------------------------------------------------------------------------//
                        //------------------------ refresh parent window -//
                        //Response.Write("<script>parent.location.reload();</script>");
                        return RedirectToAction("Index", new { ShopID = ShopID, DeliveryPartnerID = DeliveryPartnerID, OrderStatus = OrderStatus, ShopOrderCode = ShopOrderCode, Isposted = true });
                    }

                    ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes.Where(x => x.ID != (int)Common.Constant.BusinessType.FRANCHISE), "ID", "Name");
                    ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorderdetailcall.CreateBy);
                    return View(customerorderdetailcall);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with the customer order detail call values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderDetailCall][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.CRM, System.Web.HttpContext.Current.Server);

                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes.Where(x => x.ID != (int)Common.Constant.BusinessType.FRANCHISE), "ID", "Name");
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorderdetailcall.CreateBy);
                return View(customerorderdetailcall);
            }

        }

        public string SendSMS(string CustomerOrderID)
        {
            long lCustomerOrderID = Convert.ToInt64(CustomerOrderID);
            ModelLayer.Models.CustomerOrder lCustomerOrder = db.CustomerOrders.Find(lCustomerOrderID);
            string lRegMobileNo = "";
            string lCustomerOrderCode = "";
            string lRegEmail = "";
            string lFirstName = "";
            if(lCustomerOrder != null)
            {
                lRegMobileNo = lCustomerOrder.UserLogin.Mobile;
                lRegEmail = lCustomerOrder.UserLogin.Email;
                
                PersonalDetail lPersonalDetail = db.PersonalDetails.FirstOrDefault(x => x.UserLoginID == lCustomerOrder.UserLoginID);
                if(lPersonalDetail != null)
                {
                    lFirstName = lPersonalDetail.FirstName;
                }



                lCustomerOrderCode = lCustomerOrder.OrderCode;
            }
            //lEmail = "avirakeshverma@gmail.com";
            //lMobile = "9028415132";
            //------Declartion Email---------//
            string city = "nagpur";
            int franchiseID = 2;////added for Multiple MCO in Same City
            if (System.Web.HttpContext.Current.Request.Cookies["CityCookie"] != null && System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value != "")
            {
                city = System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                franchiseID = Convert.ToInt32(System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[2]); ////added for Multiple MCO in Same City
            }
            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
            BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseID + "/login");////added "/" + franchiseID + for Multiple MCO in Same City
            dictionary.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseID + "/cust-o/my-order");////added "/" + franchiseID + for Multiple MCO in Same City


            bool Flag = false;
            //------Declartion SMS---------//
            gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
            dictionary.Clear();
            Flag = false;
            try
            {
                //------Key value add in SMS---------//
                if(lFirstName != "")
                {
                    dictionary.Add("#--NAME--#", lFirstName);
                }
                else
                {
                    dictionary.Add("#--NAME--#", lRegEmail);
                }

                dictionary.Add("#--ORD_NUM--#", lCustomerOrderCode);
                //Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_DLVRD, new string[] { lRegMobileNo, rcKey.DEFAULT_ALL_SMS }, dictionary);
                Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_DLVRD, new string[] { lRegMobileNo }, dictionary);
            }
            catch (Exception ex)
            {
            }
            return Flag.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public enum OrderStatusEnum
        {
            NONE=0,
            ORD_PLAC = 1,
            ORD_CONF = 2,
            ORD_DIL = 7,
            ORD_CANC = 9
        }
        public T GetEnumValue<T>(int intValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }
            T OrderStatusEnumVal = ((T[])Enum.GetValues(typeof(T)))[0];

            foreach (T enumValue in (T[])Enum.GetValues(typeof(T)))
            {
                if (Convert.ToInt32(enumValue).Equals(intValue))
                {
                    OrderStatusEnumVal = enumValue;
                    break;
                }
            }
            return OrderStatusEnumVal;
        }
    }
}
