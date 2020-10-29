
//-----------------------------------------------------------------------
// <copyright file="CustomerOrderController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Gaurav Dixit</author>
//-----------------------------------------------------------------------

using BusinessLogicLayer;
using Gandhibagh.Models;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Gandhibagh.Controllers
{
    public class CustomerOrderController : Controller
    {
        public class WebMethodParamSimilar
        {
            public long cityID { get; set; }
            public int franchiseId { get; set; }////added 
            public int categoryID { get; set; }
            public long productID { get; set; }
            public long shopID { get; set; }
            public int pageIndex { get; set; }
            public int pageSize { get; set; }
        }

        private EzeeloDBContext db = new EzeeloDBContext();

        [SessionExpire]
        public ActionResult MyOrders()
        {
            //Set Cookie for Url saving & Use in Continue shopping
            URLCookie.SetCookies();
            /*
               Indents:
             * Description: This method is used to list all orders and its related product list of session user.
             *              This method call BusinessLogicLayer.CustomerOrder.GetCustomerOrders(userLoginID); to get all 
             *               order list
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */
            try
            {
                long userLoginID = 0;
                long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);

                BusinessLogicLayer.CustomerOrder customerOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                ViewCustomerOrderViewModel customerOrders = customerOrder.GetCustomerOrders(userLoginID);

                return View(customerOrders);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderController][GET:MyOrders]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View();
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderController][GET:MyOrders]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        [SessionExpire]
        public ActionResult CancelOrder(long orderID, long? shopStockID)
        {
            //Set Cookie for Url saving & Use in Continue shopping
            URLCookie.SetCookies();

            /*
               Indents:
             * Description: This method calls GET request for cancelling order.
             *              we can cancel Complete order, or single item from particular order.
             *              But now functionality for cancelling item from particular order is hidden.
             
             * Parameters: orderID: Get details of particular order
             *             shopStockID: Nullable, If customer want to cancel entire order
             *                          Not Nullable, If customer wants to cancel particular item in current order.
             *                          (This provision is blocked right now as discussed in SOP)
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */

            CancelOrderViewModel cancelOrderViewModel = new CancelOrderViewModel();

            long ssID = 0, userLoginID = 0;
            long.TryParse(Convert.ToString(shopStockID), out ssID);

            long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);

            ModelLayer.Models.CustomerOrder lCustomerOrder = new ModelLayer.Models.CustomerOrder();
            lCustomerOrder = db.CustomerOrders.Find(orderID);

            if (lCustomerOrder == null)
            {
                return View("HttpError");
            }
            else if (lCustomerOrder.UserLoginID != userLoginID)
            {
                return View("AccessDenied");
            }

            cancelOrderViewModel.OrderID = orderID;
            cancelOrderViewModel.OrderNo = lCustomerOrder.OrderCode;
            cancelOrderViewModel.ShopStockID = ssID;

            return View(cancelOrderViewModel);
        }

        [SessionExpire]
        [HttpPost]
        public ActionResult CancelOrder(CancelOrderViewModel cancelOrderViewModel, string ddlDescription, string txtArea, string hdnOrderNo)
        {
            /*
               Indents:
             * Description: This method calls POST request for cancelling order.
             *              we can cancel Complete order, or single item from particular order.
             *              But now functionality for cancelling item from particular order is hidden.
             
             * Parameters: cancelOrderViewModel.orderID: Get details of particular order
             *             cancelOrderViewModel.shopStockID: Nullable, If customer want to cancel entire order
             *                          Not Nullable, If customer wants to cancel particular item in current order.
             *                          (This provision is blocked right now as discussed in SOP)
             
             * Precondition: 
             * Postcondition:
             * Logic: 1) Get UserLoginId and ShopStockID
             *        2) Get Customer Order Details by orderId
             *        3) If Null, then return
             *        4) if (isCancellable == true) means order is cancellable otherwise return
                      4.1) Cancel order, send SMS and return to view.
             *        5)  Else order is not cancellable and return .
             */

            try
            {
                long userLoginID = 0, ssID = 0;

                // 1
                long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);
                long.TryParse(Convert.ToString(cancelOrderViewModel.ShopStockID), out ssID);

                // 2
                ModelLayer.Models.CustomerOrder lOrder = db.CustomerOrders.Find(cancelOrderViewModel.OrderID);


                if (lOrder == null) // 3
                {
                    return View("HttpError");
                }
                else if (lOrder.UserLoginID != userLoginID)
                {
                    return View("AccessDenied");
                }

                BusinessLogicLayer.CustomerOrder customerOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                ViewCustomerOrderViewModel lCustomerOrder = customerOrder.GetCustomerOrders(userLoginID, cancelOrderViewModel.OrderID);

                bool isCancellable = false;

                foreach (var item in lCustomerOrder.OrderProducts)
                {
                    if (item.OrderStatus != 9)
                    {
                        isCancellable = true;
                    }
                }

                if (isCancellable == true)  // 4
                {
                    int result = customerOrder.CancelCustomerOrder(cancelOrderViewModel.OrderID, ssID, userLoginID, ddlDescription + ". " + txtArea);  // 4.1
                                                                                                                                                       //Added by Rumana
                    bool IsMailSend = false;
                    if (result == 103)
                    {
                        if (lOrder.MLMAmountUsed > 0)
                        {
                            //customerOrder.Insert_RefundRequest_EwalletRefund(cancelOrderViewModel.OrderID);//Added by Rumana on 19/04/2019  

                            //customerOrder.Send_EWalletRefund_Mail(cancelOrderViewModel.OrderID, IsMailSend);      
                            (new MLMWalletPoints()).RefundUsedWalletAmount(lOrder.MLMAmountUsed.Value, cancelOrderViewModel.OrderID, 1, userLoginID,9);
                        }


                        BusinessLogicLayer.OrderCancelSmsAndEmail orderCancel = new OrderCancelSmsAndEmail(System.Web.HttpContext.Current.Server);
                        orderCancel.SendSMSToCustomer(userLoginID, cancelOrderViewModel.OrderID);
                        orderCancel.SendSMSToMerchant(userLoginID, cancelOrderViewModel.OrderID);
                        orderCancel.SendMailToCustomer(userLoginID, cancelOrderViewModel.OrderID);
                        orderCancel.SendMailToMerchant(userLoginID, cancelOrderViewModel.OrderID);
                        (new SendFCMNotification()).SendNotification("cancelled", cancelOrderViewModel.OrderID);
                        ViewBag.Message = "Your order has been cancelled!!";
                        //Yashaswi 27-7-2018
                        TempData["IsLeadersSignUp"] = "Your order has been cancelled Successfully!!";
                        return RedirectToRoute("Home", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Request.Cookies["CityCookie"].Value.Split('$')[2].Trim() });
                    }
                    else
                    {
                        ViewBag.Message = "Sorry! Problem in cancelling order!!";
                    }
                }
                else  // 5
                {
                    ViewBag.Message = "Sorry! Problem in cancelling order!!";
                }

                cancelOrderViewModel.OrderNo = hdnOrderNo;
                return View(cancelOrderViewModel);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderController][POST:CancelOrder]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                cancelOrderViewModel.OrderNo = hdnOrderNo;
                return View(cancelOrderViewModel);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderController][POST:CancelOrder]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                ViewBag.Message = ex.Message;
                cancelOrderViewModel.OrderNo = hdnOrderNo;
                return View(cancelOrderViewModel);
            }
        }

        public ActionResult OrderHistory(long orderID, long? uid)
        {
            //Set Cookie for Url saving & Use in Continue shopping
            URLCookie.SetCookies();
            /*
              Indents:
            * Description: This method is used for view the details of order by orderId
            *              This method can be called from two ends:
             *              1) Order history from my orders on customer dashboard
            *               2) Track Order link present on home page

            * Parameters: orderID: Is used to fetch the order detail from customer order table
                          uid:  Nullable reason not required for already lgged in person
             *             if user track order without login then we need uid, because you can't track other person order

            * Precondition: if uid!=NULL, OrderStatus() will be called first then OrderStatus() internally called
             *              post action method of orderhistory() that calls GET action method of orderhistory()
            * Postcondition:
            * Logic: 1)if (uid <= 0 || uid == null)  that means it is called from MyOrders from dashboard
             *        1.1) Find order by orderId
             *        1.2) If NULL, then return
             *        1.3) Else return particular order to view
             *        2) Else that means it is called from track order. ( See Precondition)
             *        2.1) Find order by OrderId and return to view
            */

            try
            {
                ViewCustomerOrderViewModel currentOrder = new ViewCustomerOrderViewModel();
                if (uid <= 0 || uid == null)  //1
                {
                    long userLoginID = 0;
                    long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);

                    ModelLayer.Models.CustomerOrder lCustomerOrder = new ModelLayer.Models.CustomerOrder();

                    lCustomerOrder = db.CustomerOrders.Find(orderID);  // 1.1

                    if (lCustomerOrder == null)             // 1.2
                    {
                        return View("HttpError");
                    }
                    else if (lCustomerOrder.UserLoginID != userLoginID)
                    {
                        return View("AccessDenied");
                    }
                    BusinessLogicLayer.CustomerOrder customerOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);

                    currentOrder = customerOrder.GetCustomerOrders(userLoginID, orderID);  // 1.3
                    currentOrder.lCalculatedTaxList = customerOrder.GetTaxDetails(orderID);
                    return View(currentOrder);
                }
                else //2
                {
                    long lLoginID = 0;
                    long.TryParse(uid.ToString(), out lLoginID);

                    BusinessLogicLayer.CustomerOrder customerOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);

                    currentOrder = customerOrder.GetCustomerOrders(lLoginID, orderID); // 2.1

                    currentOrder.lCalculatedTaxList = customerOrder.GetTaxDetails(orderID);

                    return View(currentOrder);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderController][GET:OrderHistory]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View("HttpError");
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderController][GET:OrderHistory]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View("HttpError");
            }
        }

        [HttpPost]
        public ActionResult OrderHistory(OrderStatusViewModel orderStatusViewModel)
        {
            /*
              Indents:
            * Description: This method is used to track order when user is not login
            *              
             
            * Parameters: 
             
            * Precondition: This method will only be called from track order(CustomerOrderController->OrderStatus())
            * Postcondition:
            * Logic: 1) if (orderStatusViewModel.OrderNo == null || orderStatusViewModel.OrderNo == string.Empty) then return
             *        2) Else executed
             *        2.1) if (IsEmailValid == true) then fetch record by email and order no and return
             *              else return error
             *        2.2) if (IsMobileValid == true) then fetch record by mobile and order no and return
             *              else return error
            */


            try
            {
                bool IsEmailValid = false, IsMobileValid = false;

                if (orderStatusViewModel.OrderNo == null || orderStatusViewModel.OrderNo == string.Empty) //1
                {
                    TempData["Message"] = "Please enter Order Number.";
                    //TempData["Message"] = "Enter valid Order Number & Mobile Number or Email you used at the time of purchase";
                    return RedirectToAction("OrderStatus");
                }
                else  // 2
                {
                    IsEmailValid = CommonFunctions.IsValidEmailId(orderStatusViewModel.EmailId);
                    IsMobileValid = CommonFunctions.IsValidMobile(orderStatusViewModel.EmailId);

                    if (IsEmailValid == true)  // 2.1)
                    {
                        var lCustomerOrder = (from co in db.CustomerOrders
                                              join ul in db.UserLogins on co.UserLoginID equals ul.ID
                                              where co.OrderCode == orderStatusViewModel.OrderNo && ul.Email == orderStatusViewModel.EmailId
                                              && co.ReferenceCustomerOrderID == null      //Added by tejaswee
                                              select new
                                              {
                                                  co.ID,
                                                  co.UserLoginID
                                              }).FirstOrDefault();

                        if (lCustomerOrder != null)
                        {
                            return RedirectToAction("OrderHistory", new { orderId = lCustomerOrder.ID, uid = lCustomerOrder.UserLoginID });
                        }
                        else
                        {
                            TempData["Message"] = "Enter valid Order Number or Email you used at the time of purchase";
                            return RedirectToAction("OrderStatus");
                        }
                    }
                    else if (IsMobileValid == true) // 2.2
                    {
                        var lCustomerOrder = (from co in db.CustomerOrders
                                              where co.OrderCode == orderStatusViewModel.OrderNo && co.PrimaryMobile == orderStatusViewModel.EmailId
                                              && co.ReferenceCustomerOrderID == null      //Added by tejaswee
                                              select new
                                              {
                                                  co.ID,
                                                  co.UserLoginID
                                              }).FirstOrDefault();

                        if (lCustomerOrder != null)
                        {
                            return RedirectToAction("OrderHistory", new { orderId = lCustomerOrder.ID, uid = lCustomerOrder.UserLoginID });
                        }
                        else
                        {
                            TempData["Message"] = "Enter valid Order Number or Mobile you used at the time of purchase";
                            return RedirectToAction("OrderStatus");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Enter valid Mobile Number or Email you used at the time of purchase";
                        return RedirectToAction("OrderStatus");
                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderController][POST:OrderHistory]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View("HttpError");
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderController][POST:OrderHistory]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View("HttpError");
            }
        }

        public ActionResult OrderStatus()
        {
            //Set Cookie for Url saving & Use in Continue shopping
            URLCookie.SetCookies();
            /*
              Indents:
            * Description: This method is called by track order link present on home page
             
            * Parameters: 
             
            * Precondition: 
            * Postcondition:
            * Logic: 
            */

            try
            {
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderController][GET:OrderStatus]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View("HttpError");
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderController][GET:OrderStatus]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View("HttpError");
            }
        }

        [SessionExpire]
        //- Change made by Avi Verma and Pradnyakar Sir. Date :- 02-July-2016.
        //- Reason :- SEO PPC required static URL for getting results.
        //- As said by Reena ma'am, Bhavan and Bhusan.
        //public ActionResult PurchaseComplete(long orderID)
        public ActionResult PurchaseFailure()
        {
            TempData.Keep();
            ViewBag.err = TempData["err"].ToString();
            ViewBag.errMsg = TempData["errMsg"].ToString();

            long orderID = 0;
            if (TempData["orderID"] != null)
                orderID = Convert.ToInt64(TempData["orderID"].ToString());
            long userLoginID = 0;
            long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);

            ModelLayer.Models.CustomerOrder lCustomerOrder = new ModelLayer.Models.CustomerOrder();

            lCustomerOrder = db.CustomerOrders.Find(orderID);

            //if (lCustomerOrder == null)
            //{
            //    return View("HttpError");
            //}
            //else if (lCustomerOrder.UserLoginID != userLoginID)
            //{
            //    return View("AccessDenied");
            //}


            long lShopID = 0, lCityID = 0, lItemID = 0;
            int lFranchiseID = 0;////added
            BusinessLogicLayer.CustomerOrder customerOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
            ViewCustomerOrderViewModel currentOrder = customerOrder.GetCustomerOrders(userLoginID, orderID);

            PurchaseCompleteViewModel purchaseComplete = new PurchaseCompleteViewModel();

            purchaseComplete.CustomerOrder = currentOrder;
            // purchaseComplete.FrequentlyProducts = frequentlyBuyedProducts;

            purchaseComplete.lCalculatedTaxList = customerOrder.GetTaxDetails(orderID);
            return View(purchaseComplete);
        }
        public ActionResult PurchaseComplete()
        {
            /*
               Indents:
             * Description: This method is called after order is successfully placed. This method is basically used to display order details
             *              to customer, and we are also suggesting frequently buyed products related to purchased product.
             
             * Parameters: orderID: orderID is used to fetch current order details
             
             * Precondition: Order
             * Postcondition:
             * Logic: 
             */
            long orderID = 0;

            if (TempData["orderID"] != null)
                orderID = Convert.ToInt64(TempData["orderID"].ToString());

            //- Start : Added By Avi Verma. [Reference ID : CART_UPDATE]
            //- Date : 08-Sep-2016.
            //- Reason : Abandoned Cart, Update order is placed by Customer only not by CRM Login.
            try
            {
                long lCartID = -1;
               
                if (ControllerContext.HttpContext.Request.Cookies["CartID"] != null &&
                    ControllerContext.HttpContext.Request.Cookies["CartID"].Value != null)
                {
                    lCartID = Convert.ToInt64(ControllerContext.HttpContext.Request.Cookies["CartID"].Value);
                }
                else
                {
                    var orderid = Convert.ToString(orderID);
                    var pay = db.PaymentDatas.Where(x => x.TxnId == orderid).FirstOrDefault();

                    //var pay = db.PaymentDatas.Where(x => x.TxnId.ToString() == order_id).FirstOrDefault();
                    if (pay != null)
                    {
                        lCartID = pay.CartId;
                        ControllerContext.HttpContext.Response.Cookies["CartID"].Value = lCartID.ToString();
                    }
                }
                if (lCartID != null && lCartID != -1)
                {
                    TrackCartBusiness lTrackCartBusiness = new TrackCartBusiness();
                    Cart lCart = lTrackCartBusiness.UpdateCart(lCartID, orderID, null, (int)ModelLayer.Models.Enum.ORDER_STATUS.PLACED);

                    //- Create a new Cart for tracking.
                    if (Session["UID"] != null)
                    {
                        CreateVirtualAbandonedCart(Convert.ToInt64(Session["UID"]));
                    }
                }
            }
            catch (Exception myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                + Environment.NewLine + myEx.Message + Environment.NewLine
                + "[CustomerOrderController][GET:PurchaseComplete]" + myEx.Message,
                BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            //- End : Added By Avi Verma.


            try
            {
                long userLoginID = 0;
                //userLoginID = Convert.ToInt64(Convert.ToString(Session["UID"]));
                long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);
               
                ModelLayer.Models.CustomerOrder lCustomerOrder = new ModelLayer.Models.CustomerOrder();

                lCustomerOrder = db.CustomerOrders.Find(orderID);
                if (userLoginID == 0)
                {
                    userLoginID = lCustomerOrder.UserLoginID;
                    Session["UID"] = userLoginID;
                }
                if (lCustomerOrder == null)
                {
                    return View("HttpError");
                }
                else if (lCustomerOrder.UserLoginID != userLoginID)
                {
                    return View("AccessDenied");
                }


                long lShopID = 0, lCityID = 0, lItemID = 0;
                int lFranchiseID = 0;////added
                BusinessLogicLayer.CustomerOrder customerOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                ViewCustomerOrderViewModel currentOrder = customerOrder.GetCustomerOrders(userLoginID, orderID);

                long.TryParse(currentOrder.OrderProducts.FirstOrDefault().ShopID.ToString(), out lShopID);
                long.TryParse(currentOrder.OrderProducts.FirstOrDefault().ProductID.ToString(), out lItemID);
                if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
                {
                    string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                    string[] arr = cookieValue.Split('$');
                    lCityID = Convert.ToInt32(arr[0]);
                    lFranchiseID = Convert.ToInt32(arr[2]);////added
                }

                ViewBag.PID = lItemID;
                ViewBag.SID = lShopID;

                RelatedProductsViewModel frequentlyBuyedProducts = this.SearchSimilarProducts(0, 0, 24, lItemID, lShopID, 1, 5);////added 0

                PurchaseCompleteViewModel purchaseComplete = new PurchaseCompleteViewModel();

                purchaseComplete.CustomerOrder = currentOrder;
                purchaseComplete.FrequentlyProducts = frequentlyBuyedProducts;

                purchaseComplete.lCalculatedTaxList = customerOrder.GetTaxDetails(orderID);


                //Added for avoiding multiple order place for single cart
                TempData["ReturnFromUrlpurchaseComplete"] = "purchaseComplete";

                //Added by Zubair for MLM on 31-01-2018
                //To reset MLM Amount after purchase completed
                if (System.Web.HttpContext.Current.Request.Cookies["EWalletAmountUsed"] != null && Convert.ToDecimal(System.Web.HttpContext.Current.Request.Cookies["EWalletAmountUsed"].Value) > 0)
                {
                    System.Web.HttpContext.Current.Request.Cookies["EWalletAmountUsed"].Value = "0";
                    System.Web.HttpContext.Current.Response.Cookies.Add(System.Web.HttpContext.Current.Request.Cookies["EWalletAmountUsed"]);
                }
                //End


                return View(purchaseComplete);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderController][GET:PurchaseComplete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View("HttpError");
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderController][GET:PurchaseComplete]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View("HttpError");
            }
        }

        private RelatedProductsViewModel SearchFrequentlyBuyedProducts(long cityID, int franchiseID, long productID, long shopID, int pageIndex, int pageSize)//added int franchiseID
        {
            /*
               Indents:
             * Description: This method is used to get FrequentlyBuyedProducts from database
             
             * Parameters: cityID: Contains cityId
             *             productID: Contains ProductId
             *             shopID: Contains Shop Id
             *             pageIndex: Used when load more items
             *             pageSize: Used how many products to load at a time
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */

            try
            {
                SearchFrequentlyBuyedProductViewModel searchFrequentlyBuyedProductViewModel = new SearchFrequentlyBuyedProductViewModel();

                searchFrequentlyBuyedProductViewModel.CityID = cityID;
                searchFrequentlyBuyedProductViewModel.FranchiseID = franchiseID;////added
                searchFrequentlyBuyedProductViewModel.ProductID = productID;
                searchFrequentlyBuyedProductViewModel.ShopID = shopID;
                searchFrequentlyBuyedProductViewModel.PageIndex = pageIndex;
                searchFrequentlyBuyedProductViewModel.PageSize = pageSize;

                ProductDetails productDetails = new ProductDetails(System.Web.HttpContext.Current.Server);

                RelatedProductsViewModel relatedProductsViewModel = productDetails.GetFrequentlyBuyedProducts(searchFrequentlyBuyedProductViewModel);

                return relatedProductsViewModel;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[CustomerOrderController][M:SearchFrequentlyBuyedProducts]", "Can't search frequently buyed products !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[CustomerOrderController][M:SearchFrequentlyBuyedProducts]", "Can't search frequently buyed products !" + Environment.NewLine + ex.Message);
            }
        }

        private RelatedProductsViewModel SearchSimilarProducts(long cityID, int franchiseID, int categoryID, long productID, long shopID, int pageIndex, int pageSize)////added int franchiseID
        {
            /*
               Indents:
             * Description: This method is used to get SimilarProducts from database
             
             * Parameters: cityID: Contains cityId
             *             categoryID: Contains category Id
             *             productID: Contains ProductId
             *             shopID: Contains Shop Id
             *             pageIndex: Used when load more items
             *             pageSize: Used how many products to load at a time
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */
            try
            {
                SearchSimilarProductViewModel searchSimilarProductViewModel = new SearchSimilarProductViewModel();

                searchSimilarProductViewModel.CityID = cityID;
                searchSimilarProductViewModel.FranchiseID = franchiseID;////added
                searchSimilarProductViewModel.CategoryID = categoryID;
                searchSimilarProductViewModel.ProductID = productID;
                searchSimilarProductViewModel.ShopID = shopID;
                searchSimilarProductViewModel.PageIndex = pageIndex;
                searchSimilarProductViewModel.PageSize = pageSize;

                ProductDetails productDetails = new ProductDetails(System.Web.HttpContext.Current.Server);

                RelatedProductsViewModel relatedProductsViewModel = productDetails.GetSimillarProducts(searchSimilarProductViewModel);

                return relatedProductsViewModel;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[CustomerOrderController][M:SearchSimilarProducts]", "Can't search similar products !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[CustomerOrderController][M:SearchSimilarProducts]", "Can't search similar products !" + Environment.NewLine + ex.Message);
            }
        }

        public JsonResult TrackOrder(long orderId, long shopStockID)
        {
            //Set Cookie for Url saving & Use in Continue shopping
            // URLCookie.SetCookies();
            /*
               Indents:
             * Description: This method is used to get order shipment details from database
             
             * Parameters: orderId: Contains orderId
             *             shopStockID: Contains shopStockID
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */

            try
            {
                TrackCustomerOrder trackCustomerOrder = new TrackCustomerOrder(System.Web.HttpContext.Current.Server);

                List<TrackShipmentViewModel> listHistory = trackCustomerOrder.GetOrderProductHistory(orderId, shopStockID);

                return Json(listHistory, JsonRequestBehavior.AllowGet);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderController][JSON:TrackOrder]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderController][JSON:TrackOrder]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult BindFrequentlyBuyedProducts(WebMethodParamSimilar myParam)
        {
            /*
               Indents:
             * Description: This method is used to load FrequentlyBuyedProducts using jquery 
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */
            try
            {
                // RelatedProductsViewModel frequentlyBuyedProducts = this.SearchFrequentlyBuyedProducts(myParam.cityID, myParam.productID, myParam.shopID, myParam.pageIndex, myParam.pageSize);
                RelatedProductsViewModel similarProducts = this.SearchSimilarProducts(myParam.cityID, myParam.franchiseId, myParam.categoryID, 29, myParam.shopID, myParam.pageIndex, myParam.pageSize);////added myParam.franchiseId
                return Json(similarProducts.ProductList, JsonRequestBehavior.AllowGet);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderController][JSON:BindFrequentlyBuyedProducts]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderController][JSON:BindFrequentlyBuyedProducts]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
        BusinessLogicLayer.NextBuyEnquiry nextbuyObj = new BusinessLogicLayer.NextBuyEnquiry(fConnectionString);

        //
        // GET: /NextBuyEnquiry/Create
        public ActionResult NextBuyEnquiry(long CustomerOrderID)
        {
            if ((nextbuyObj.Select_NextBuyEnquiry(CustomerOrderID)).Rows.Count > 0)
            {
                return PartialView("_emptyView");
            }
            else
            {
                ViewBag.CustomerOrderID = CustomerOrderID;
                ViewBag.NextBuySchedle = new SelectList(nextbuyObj.NextBuyScheduleList(), "id", "text");
                ViewBag.ThreeLevelCategory = new SelectList(db.Categories.Where(x => x.Level == 3 && x.IsActive == true).OrderBy(x => x.Name).ToList(), "id", "Name");
                return PartialView("_NextBuyEnquiry");
            }

        }

        public JsonResult NextBuyPost(int id, string remarks, long orderId, long CategoryID)
        {
            try
            {
                if (id < 1 || id > 7)
                {
                    return Json("Please Select your Choice ", JsonRequestBehavior.AllowGet);
                }

                long userLoginID = 0;
                long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);

                UserLogin userLogin = new UserLogin();
                userLogin = db.UserLogins.Find(userLoginID);
                List<object> paramValues = new List<object>();
                paramValues.Add(DBNull.Value);
                paramValues.Add(userLogin.Email); // Email as Name
                paramValues.Add(userLogin.Email);
                paramValues.Add(userLogin.Mobile);
                DateTime dt = new DateTime();
                dt = nextbuyObj.NextBuySchedule(id);
                paramValues.Add(dt);
                dt = DateTime.UtcNow.AddHours(5.30);
                paramValues.Add(dt);
                paramValues.Add(orderId);
                paramValues.Add(CategoryID);
                paramValues.Add(remarks);
                paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                paramValues.Add("x");
                paramValues.Add("x");
                paramValues.Add("INSERT");
                paramValues.Add(0);

                string Message = nextbuyObj.InsertUpdate_nextbuyenquiry(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT);


                return Json(Message, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Sorry Somethink is wrong ", JsonRequestBehavior.AllowGet);
            }
        }

        private Cart CreateVirtualAbandonedCart(long UserLoginID)
        {
            Cart lCart = new Cart();
            try
            {
                long cityID = 0;
                Franchise lFranchise = new Franchise();
                if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
                {
                    string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                    string[] arr = cookieValue.Split('$');
                    cityID = Convert.ToInt64(arr[0]);

                    lFranchise = db.Franchises.FirstOrDefault(x => x.BusinessDetail.Pincode.CityID == cityID);
                }

                TrackCartBusiness lTrackCartBusiness = new TrackCartBusiness();
                lCart = lTrackCartBusiness.CreateVirtualAbandonedCart(UserLoginID, cityID, lFranchise.ID, string.Empty, string.Empty);
                ControllerContext.HttpContext.Response.Cookies["CartName"].Value = lCart.Name;
                ControllerContext.HttpContext.Response.Cookies["CartID"].Value = lCart.ID.ToString();
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[LoginController][CreateCart]" + ex.Message,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            return lCart;
        }

        //public JsonResult GetFeedback(string feedbackMsg, string OrderCode, string emailAddress, string mobile)  
        public JsonResult GetFeedback(FeedbackParams myParam)
        {
            int oprStatus = 0;
            long cityID = 0;
            ModelLayer.Models.FeedbackManagment feedbackManagment = new ModelLayer.Models.FeedbackManagment();
            if (Session["UID"] == null)
            {
                var luserLogin = (from ul in db.UserLogins
                                  join co in db.CustomerOrders on ul.ID equals co.UserLoginID
                                  where co.OrderCode == myParam.OrderCode.Trim()
                                  select new
                                  {
                                      ul.Email,
                                      co.PrimaryMobile
                                  }).FirstOrDefault();
                feedbackManagment.Email = luserLogin.Email;
                feedbackManagment.Mobile = luserLogin.PrimaryMobile;

            }
            else
            {
                feedbackManagment.Email = myParam.emailAddress;
                feedbackManagment.Mobile = myParam.mobile.Trim();
                feedbackManagment.CreateBy = Convert.ToInt64(Session["UID"].ToString());
            }
            //FeedbackCategaryID = 6 For Product request
            feedbackManagment.FeedbackCategaryID = 7; // Feedback from customer order page
            feedbackManagment.Message = myParam.feedbackMsg;
            feedbackManagment.IsActive = true;
            feedbackManagment.CustOrderCode = myParam.OrderCode.Trim();
            feedbackManagment.CreateDate = DateTime.UtcNow.AddHours(5.5);

            feedbackManagment.FeedBackTypeID = 1;
            if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
            {
                string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                string[] arr = cookieValue.Split('$');
                cityID = Convert.ToInt64(arr[0]);
            }
            feedbackManagment.CityID = cityID;
            try
            {
                if (ModelState.IsValid)
                {
                    EzeeloDBContext db1 = new EzeeloDBContext();
                    db1.FeedbackManagments.Add(feedbackManagment);
                    db1.SaveChanges();

                    oprStatus = 1;
                }

            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => new { x.ErrorMessage });

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                ViewBag.Message = fullErrorMessage.Replace("ErrorMessage = ", " ");

            }

            catch (Exception ex)
            {
                string msg = "";
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        msg += error + "-";
                    }
                }
                oprStatus = 0;
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in saving details of customer looking for :" + ex.Message + ex.InnerException, ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

            }
            return Json(oprStatus.ToString(), JsonRequestBehavior.AllowGet);
        }

        public class FeedbackParams
        {
            public string emailAddress { get; set; }
            public string feedbackMsg { get; set; }
            public string OrderCode { get; set; }
            public string mobile { get; set; }

        }
    }
}
