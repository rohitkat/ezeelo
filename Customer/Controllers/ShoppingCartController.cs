using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogicLayer;
using System.Text;
using ModelLayer.Models;
using System.Web.Configuration;
using Gandhibagh.Models;

namespace Gandhibagh.Controllers
{
    public class ShoppingCartController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        private string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();

        /* static int FranchiseID =2;// Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2].Trim());
         public int MinOrder = (Int32)db.Weekly_Seasona_Festival_Message.Where(x => x.FranchiseID == FranchiseID && x.IsActive == true && x.MessageType.MessageType1 == "MinimumOrder").Select(i => i.MinimumOrderInRupee).FirstOrDefault();
         public string WeekHoliday=db.Weekly_Seasona_Festival_Message.Where(x => x.FranchiseID == FranchiseID && x.IsActive == true && x.MessageType.MessageType1 == "WeeklyHoliday").Select(i =>i.WeeklyHoliday).FirstOrDefault();
         public string Message = db.Weekly_Seasona_Festival_Message.Where(x => x.FranchiseID == FranchiseID && x.IsActive == true && x.MessageType.MessageType1 == "WeeklyHoliday").Select(i => i.Message).FirstOrDefault();
   */
        [SessionExpire]
        public ActionResult Index()
        {
            //TempData["IsCheckPinCode"] = "false";
            TempData["ReturnFromUrlpurchaseComplete"] = null; ////added change by Tejaswee on 28/9/2016
            ShoppingCartInitialization sci = new ShoppingCartInitialization();
            Session["OrderCouponAmount"] = null;
            Session["OrderCouponCode"] = null;

            string lPincode = string.Empty;
            ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();
            //-- Add by Ashish for Dynamic Weekly Holiday Message --//
            List<WeeklySeasonalFestivalPageMessage> WSFMsg1 = new List<WeeklySeasonalFestivalPageMessage>();
            WSFMsg1 = CallGetPageMessageAPI();
            ViewBag.WeekHoliday = WSFMsg1.Where(x => x.MessageType == "WeeklyHoliday").Select(i => i.WeeklyHoliday).FirstOrDefault();
            ViewBag.HolidayMessage = WSFMsg1.Where(x => x.MessageType == "WeeklyHoliday").Select(i => i.Message).FirstOrDefault();
            ViewBag.MinOrder = WSFMsg1.Where(x => x.MessageType == "MinimumOrder").Select(i => i.MinimumOrderInRupee).FirstOrDefault();

            /* int FranchiseID =Convert.ToInt32( ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2].Trim());
             ViewBag.WeekHoliday = db.Weekly_Seasona_Festival_Message.Where(x => x.FranchiseID == FranchiseID && x.IsActive == true && x.MessageType.MessageType1 == "WeeklyHoliday").Select(i =>i.WeeklyHoliday).FirstOrDefault();//i => new  {i.WeeklyHoliday,i.Message }
             ViewBag.HolidayMessage = db.Weekly_Seasona_Festival_Message.Where(x => x.FranchiseID == FranchiseID && x.IsActive == true && x.MessageType.MessageType1 == "WeeklyHoliday").Select(i =>  i.Message ).FirstOrDefault();
             ViewBag.MinOrder = db.Weekly_Seasona_Festival_Message.Where(x => x.FranchiseID == FranchiseID && x.IsActive == true && x.MessageType.MessageType1 == "MinimumOrder").Select(i => i.MinimumOrderInRupee).FirstOrDefault(); */

            //-- End --//
            //lShoppingCartCollection.fcon = fConnectionString;
            try
            {
                if (Session["UID"] != null)
                {
                    Session["IsEarnUse"] = true;
                    lShoppingCartCollection = sci.GetCookie(fConnectionString, true);
                    if (ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"] != null)
                    {
                        lShoppingCartCollection = sci.GetDeliveryCharge(lShoppingCartCollection);
                        lPincode = ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"].Value;
                    }
                    ModelState.Clear();

                    //Code for clearing message property
                    if (lShoppingCartCollection.lShopProductVarientViewModel != null)
                    {
                        lShoppingCartCollection.lShopProductVarientViewModel.Select(c => { c.Message = null; return c; }).ToList();
                    }

                    SetDeliverySchedule(lPincode);
                    this.GetCustCareNo();
                    return View(lShoppingCartCollection);
                }
                else
                {
                    return RedirectToRoute("Login");
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with loading shopping cart!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShoppingCartController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with loading shopping cart!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShoppingCartController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View(lShoppingCartCollection);

        }
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "Conform")]
        public ActionResult DeleteItemFromCart(string shopStockID, string hdnWeekHoliday, string hdnMessage, string hdnMinOrder, decimal? hdnDelCharge, decimal? hdnOrdAmt) //Yashaswi add new parameter decimal? hdnOrdAmt, decimal? hdnDeliveryCharge
        {
            ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();
            try
            {
                long cityId = Convert.ToInt64(Request.Cookies["CityCookie"].Value.Split('$')[0]);
                int franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);
                //-- For Dynamic Message --//
                ViewBag.WeekHoliday = hdnWeekHoliday;
                ViewBag.MinOrder = hdnMinOrder;
                string str = hdnMessage;
                if (str != null)
                {
                    str = str.Replace("|", " ");
                }
                ViewBag.HolidayMessage = str;
                ///////////////
                string lPincode = string.Empty;
                ShoppingCartInitialization sci = new ShoppingCartInitialization();
                lShoppingCartCollection = new ShopProductVarientViewModelCollection();
                // List<ShoppingCart> lstSC = sci.GetCookie();
                decimal? RemovedProductAmount = 0;
                sci.DeleteItemCookie(shopStockID, out RemovedProductAmount);
                sci.DeleteCouponCookie(shopStockID);
                //ShoppingCartCollection lShoppingCartCollection = sci.GetCookie();
                if (Session["IsEarnUse"] == null)
                {
                    lShoppingCartCollection = sci.GetCookie(fConnectionString, true);
                }
                else
                {
                    lShoppingCartCollection = sci.GetCookie(fConnectionString, (bool)Session["IsEarnUse"]);
                }
                if (ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"] != null)
                {
                    lShoppingCartCollection = sci.GetDeliveryCharge(lShoppingCartCollection);
                    lPincode = ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"].Value;
                }


                // if (Session["OrderCouponCode"] != null && Session["OrderCouponAmount"]!=null)
                if (Session["OrderCouponCode"] != null)
                {

                    string userMessage = string.Empty;
                    int validityCode = 0;
                    long userLoginID = 0;
                    long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);
                    CouponManagement obj = new CouponManagement();

                    ////added
                    Session["OrderCouponAmount"] = Convert.ToDecimal(obj.VerifyCouponAgainstCart(lShoppingCartCollection.lShopWiseDeliveryCharges, Session["OrderCouponCode"].ToString(), out userMessage, out validityCode, userLoginID, cityId, franchiseId));////added cityId->franchiseId
                    Session["OrderCouponCode"] = Session["OrderCouponCode"].ToString();

                    //if(lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount!= Convert.ToDecimal(Session["OrderCouponAmount"]))
                    //{
                    //    TempData["CouponMessage"] = userMessage;
                    //}
                }
                SetDeliverySchedule(lPincode);

                //sci.DeleteWalleteAmtUsed(lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount, RemovedProductAmount, franchiseId, cityId); //Added by yashaswi
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with deleting Shopping Cart item!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShoppingCartController][POST:DeleteItemFromCart]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with deleting Shopping Cart item!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShoppingCartController][POST:DeleteItemFromCart]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View("Index", lShoppingCartCollection);

        }


        public ActionResult ChangeCartItemQuantity(string shopStockID, string txtQuantity, string hdnWeekHoliday, string hdnMessage, string hdnMinOrder, decimal? hdnOrdAmt)
        {
            ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();
            try
            {
                long cityId = Convert.ToInt64(Request.Cookies["CityCookie"].Value.Split('$')[0]);
                int franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);
                //-- For Dynamic Message --//
                ViewBag.WeekHoliday = hdnWeekHoliday;
                ViewBag.MinOrder = hdnMinOrder;
                string str = hdnMessage;
                str = str.Replace("|", " ");
                ViewBag.HolidayMessage = str;
                //////////////
                string lPincode = string.Empty;
                ShoppingCartInitialization sci = new ShoppingCartInitialization();
                lShoppingCartCollection = new ShopProductVarientViewModelCollection();
                decimal? RemovedProductAmount = 0;
                sci.ChangeItemQuantity(shopStockID, txtQuantity, out RemovedProductAmount);

                // ShoppingCartCollection lShoppingCartCollection = sci.GetCookie();
                // lShoppingCartCollection = sci.GetCookie(fConnectionString);
                if (Session["IsEarnUse"] == null)
                {
                    lShoppingCartCollection = sci.GetCookie(fConnectionString, true);
                }
                else
                {
                    lShoppingCartCollection = sci.GetCookie(fConnectionString, (bool)Session["IsEarnUse"]);
                }
                if (ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"] != null)
                {
                    lShoppingCartCollection = sci.GetDeliveryCharge(lShoppingCartCollection);
                    lPincode = ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"].Value;
                }



                // lShoppingCartCollection = sci.GetDeliveryCharge(lShoppingCartCollection);

                // if (Session["OrderCouponCode"] != null && Session["OrderCouponAmount"]!=null)
                if (Session["OrderCouponCode"] != null)
                {

                    string userMessage = string.Empty;
                    int validityCode = 0;
                    long userLoginID = 0;
                    long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);
                    CouponManagement obj = new CouponManagement();
                    Session["OrderCouponAmount"] = Convert.ToDecimal(obj.VerifyCouponAgainstCart(lShoppingCartCollection.lShopWiseDeliveryCharges, Session["OrderCouponCode"].ToString(), out userMessage, out validityCode, userLoginID, cityId, franchiseId));////added cityId->franchiseId
                    Session["OrderCouponCode"] = Session["OrderCouponCode"].ToString();

                    //if(lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount!= Convert.ToDecimal(Session["OrderCouponAmount"]))
                    //{
                    //    TempData["CouponMessage"] = userMessage;
                    //}
                }
                SetDeliverySchedule(lPincode);

                //sci.DeleteWalleteAmtUsed(lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount, RemovedProductAmount, franchiseId, cityId); //Added by yashaswi
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with changing Shopping Cart item quantity!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShoppingCartController][POST:ChangeCartItemQuantity]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with changing Shopping Cart item quantity!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShoppingCartController][POST:ChangeCartItemQuantity]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View("Index", lShoppingCartCollection);
        }

        //[HttpPost]
        //public ActionResult ContinuePaymentProcess(string hdnCategoryID, string hdnCategoryName, string hdnColorCode, string hdnColorID,string hdnColorName,
        //    string hdnDimensionID, string hdnDimensionName, string hdnMaterialID, string hdnMaterialName, string hdnMRP, string hdnPackSize, string hdnPackUnitName,
        //    string hdnProductID, string hdnProductName, string hdnProductVarientID, string hdnSaleRate, string hdnShopID, string hdnShopName, string hdnShopStockID,
        //    string hdnSizeID, string hdnSizeName, string hdnStockQty)
        //{
        //    return View();
        //}

        [HttpPost]
        //public ActionResult ContinuePaymentProcess(ShopProductVarientViewModelCollection s, ShopProductVarientViewModelCollection itemId)
        public ActionResult ContinuePaymentProcess(ShopProductVarientViewModelCollection s, ShopProductVarientViewModelCollection itemId, string hdnOrderAmount,
            string hdnOrderPayableAmount, string txtPinCode, string DeliveryScheduleID, string hdnRemaiEarnAmt, string hdnBusinessPointsTotal, string hdnWalletAmountUsed, string hdnWalletAmountRemaining)
        {
            try
            {
                ShoppingCartInitialization sci = new ShoppingCartInitialization();
                // ShopProductVarientViewModelCollection lShoppingCartCollection = sci.GetCookie(fConnectionString);
                ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();
                if (Session["IsEarnUse"] == null)
                {
                    lShoppingCartCollection = sci.GetCookie(fConnectionString, true);
                }
                else
                {
                    lShoppingCartCollection = sci.GetCookie(fConnectionString, (bool)Session["IsEarnUse"]);
                }

                //Yashaswi Start 02-02-2019 for BUG 11 to Verify cart 
                if (lShoppingCartCollection.lShopProductVarientViewModel.Any(p => p.CartVerificationMsg != null && p.CartVerificationMsg != ""))
                {
                    return RedirectToAction("Index");
                }
                //Yashaswi Start 02-02-2019 for BUG 11 to Verify cart 

                //Yashaswi
                if (lShoppingCartCollection.CartMessage!="")
                {
                    int MaxBoostRP = db.BoosterPlanMaster.FirstOrDefault(p => p.IsActive).RetailPoints;
                    if (MaxBoostRP > lShoppingCartCollection.lShoppingCartOrderDetails.BusinessPointsTotal)
                    {
                        TempData["DisplayMsg"] = "1";
                    }
                    return RedirectToAction("Index");
                }

                //ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();
                //lShoppingCartCollection.lShopProductVarientViewModel = sci.GetCookie();


                ShoppingCartOrderDetails scartOrder = new ShoppingCartOrderDetails();
                if (lShoppingCartCollection.lShoppingCartOrderDetails != null && lShoppingCartCollection.lShoppingCartOrderDetails.EarnAmount > 0 && lShoppingCartCollection.lShoppingCartOrderDetails.IsUse == true)
                {
                    scartOrder.EarnAmount = 0;
                    scartOrder.RemainEarnAmt = lShoppingCartCollection.lShoppingCartOrderDetails.RemainEarnAmt;
                    scartOrder.UsedEarnAmount = lShoppingCartCollection.lShoppingCartOrderDetails.EarnAmount - lShoppingCartCollection.lShoppingCartOrderDetails.RemainEarnAmt;
                }
                else if (lShoppingCartCollection.lShoppingCartOrderDetails != null && lShoppingCartCollection.lShoppingCartOrderDetails.EarnAmount > 0 && lShoppingCartCollection.lShoppingCartOrderDetails.IsUse == false)
                {
                    scartOrder.EarnAmount = lShoppingCartCollection.lShoppingCartOrderDetails.EarnAmount;
                    scartOrder.RemainEarnAmt = lShoppingCartCollection.lShoppingCartOrderDetails.EarnAmount;
                    scartOrder.UsedEarnAmount = 0;
                }
                if (Session["OrderCouponAmount"] != null && Session["OrderCouponCode"] != null)
                {
                    scartOrder.CoupenAmount = Convert.ToDecimal(Session["OrderCouponAmount"]);
                    scartOrder.CoupenCode = Session["OrderCouponCode"].ToString();
                }
                else
                {
                    scartOrder.CoupenAmount = 0;
                    scartOrder.CoupenCode = string.Empty;
                }
                scartOrder.NoOfPointUsed = 0;
                scartOrder.ValuePerPoint = 0;
                //Grand Total
                scartOrder.TotalOrderAmount = lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount;
                //Offer Less, Delivery Charges Add in Grand Total
                scartOrder.PayableAmount = lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount;
                scartOrder.BusinessPointsTotal = lShoppingCartCollection.lShoppingCartOrderDetails.BusinessPointsTotal;  //Added by Zubair for MLM on 05-01-2018
                scartOrder.WalletAmountUsed = lShoppingCartCollection.lShoppingCartOrderDetails.WalletAmountUsed;//Added by Zubair for MLM on 22-01-2018
                scartOrder.CashbackPointsTotal = lShoppingCartCollection.lShoppingCartOrderDetails.CashbackPointsTotal;
                lShoppingCartCollection.lShoppingCartOrderDetails = scartOrder;

                // lShoppingCartCollection.lMLMWallets.Amount = Convert.ToDecimal(hdnWalletAmountRemaining); //Added by Zubair for MLM on 22-01-2018


                //============================= Shop Delivery Charge ========================================
                //DeliveryCharges dc = new DeliveryCharges();
                //List<ShopWiseDeliveryCharges> listShopWiseDeliveryCharges = new List<ShopWiseDeliveryCharges>();
                //List<long> merId = lShoppingCartCollection.lShopProductVarientViewModel.Select(p => p.ShopID).Distinct().ToList();
                //for (int i = 0; i < merId.Count; i++)
                //{
                //    var orgProducts = lShoppingCartCollection.lShopProductVarientViewModel.Select(x => new
                //    {
                //        x.ShopID,
                //        x.ActualWeight,
                //        x.SaleRate,
                //        x.PurchaseQuantity
                //    })
                //   .Where(x => x.ShopID == merId[i]).ToList();
                //    decimal orgProductWeight = 0;
                //    decimal MerchantWiseSubTotal = 0;
                //    for (int j = 0; j < orgProducts.Count; j++)
                //    {
                //        orgProductWeight = orgProductWeight + (orgProducts[j].PurchaseQuantity * orgProducts[j].ActualWeight);
                //        MerchantWiseSubTotal = MerchantWiseSubTotal + Convert.ToInt64(orgProducts[j].SaleRate * orgProducts[j].PurchaseQuantity);
                //    }

                //    ShopWiseDeliveryCharges lShopWiseDeliveryCharges = new ShopWiseDeliveryCharges();
                //    //===================== Initialize Property ======================================
                //    lShopWiseDeliveryCharges.ShopID = merId[i];
                //    //if (MerchantWiseSubTotal < 500)
                //    //{
                //    //    if (ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"] != null)
                //    //    {
                //    //        lShopWiseDeliveryCharges.DeliveryCharge = dc.GetDeliveryCharges(ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"].Value, orgProductWeight, false);
                //    //    }
                //    //    //lShopWiseDeliveryCharges.OrderAmount = MerchantWiseSubTotal;
                //    //    //lShopWiseDeliveryCharges.Weight = orgProductWeight;
                //    //    //lShopWiseDeliveryCharges.DeliveryType = "Normal";

                //    //}
                //    //else
                //    //{
                //    //    //lShoppingCartCollection.lShopProductVarientViewModel[i].DeliveryCharge = 0;
                //    //    lShopWiseDeliveryCharges.DeliveryCharge = 0;

                //    //}

                //    lShopWiseDeliveryCharges.OrderAmount = MerchantWiseSubTotal;
                //    lShopWiseDeliveryCharges.Weight = orgProductWeight;
                //    lShopWiseDeliveryCharges.DeliveryType = "Normal";

                //    listShopWiseDeliveryCharges.Add(lShopWiseDeliveryCharges);
                //}
                ////lShoppingCartCollection.lShopWiseDeliveryCharges = listShopWiseDeliveryCharges;

                ///*===============*/
                //GetShopWiseDeliveryChargesViewModel lGetShopWiseDeliveryChargesViewModel = new GetShopWiseDeliveryChargesViewModel();
                //lGetShopWiseDeliveryChargesViewModel.ShopWiseDelivery = listShopWiseDeliveryCharges;
                //lGetShopWiseDeliveryChargesViewModel.Pincode = Request.Cookies["DeliverablePincode"].Value;
                //lGetShopWiseDeliveryChargesViewModel.IsExpress = false;

                //listShopWiseDeliveryCharges = dc.GetDeliveryCharges(lGetShopWiseDeliveryChargesViewModel);
                //// }


                //lShoppingCartCollection.lShopWiseDeliveryCharges = listShopWiseDeliveryCharges;

                //=====================================================================================================================

                lShoppingCartCollection = sci.GetDeliveryCharge(lShoppingCartCollection);




                lShoppingCartCollection.DeliveryScheduleID = DeliveryScheduleID;

                TempData["CartCollection"] = lShoppingCartCollection;
                //================ Manoj Yadav =========================
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
                    //lTrackCart.SaveDetailOnPaymentProcess(lCartID, null, UserID, MobileNo, "PAYMENT_MODE", cookieValue.Split('$')[1]);
                    //- For Manoj
                    // lTrackCart.SaveDetailOnPaymentProcess(UserID, MobileNo, "PAYMENT_MODE", cookieValue.Split('$')[1]);////hide
                    lTrackCart.SaveDetailOnPaymentProcess(lCartID, null, UserID, MobileNo, "PAYMENT_MODE", cookieValue.Split('$')[1], Convert.ToInt32(cookieValue.Split('$')[2]));//--added by Ashish for multiple franchise in same city--//
                }

                //return RedirectToAction("CustomerPaymentProcess", "PaymentProcess");
                return RedirectToRoute("PaymentProcess");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with redirecting to CustomerPaymentProcess page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShoppingCartController][POST:ContinuePaymentProcess]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return View("Index");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with redirecting to CustomerPaymentProcess page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShoppingCartController][POST:ContinuePaymentProcess]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return View("Index");
            }

        }

        public ActionResult VerifyPincode(string txtPinCode, ShopProductVarientViewModelCollection model, string hdnWeekHoliday, string hdnMessage, string hdnMinOrder, string hdnWalletAmountUsed)
        {
            try
            {
                //-- For Dynamic Message --//
                ViewBag.WeekHoliday = hdnWeekHoliday;
                ViewBag.MinOrder = hdnMinOrder;
                string str = "";
                if (hdnMessage != null)
                {
                    str = hdnMessage;
                    str = str.Replace("|", " ");

                }
                ViewBag.HolidayMessage = str;
                ////////////////////
                PincodeVerification pv = new PincodeVerification();
                bool flag = pv.IsDeliverablePincode(txtPinCode);
                if (flag == true)
                {
                    ShoppingCartInitialization sci = new ShoppingCartInitialization();
                    sci.SetVerifiedPincode(txtPinCode);
                    //=============== calculate delivery charges ======================
                    DeliveryCharges dc = new DeliveryCharges();

                    ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();
                    //lShoppingCartCollection = sci.GetCookie(fConnectionString);

                    if (Session["IsEarnUse"] == null)
                    {
                        lShoppingCartCollection = sci.GetCookie(fConnectionString, true);
                    }
                    else
                    {
                        lShoppingCartCollection = sci.GetCookie(fConnectionString, (bool)Session["IsEarnUse"]);
                    }

                    lShoppingCartCollection = sci.GetDeliveryCharge(lShoppingCartCollection);

                    //Added by Zubair for MLm on 31-01-2018
                    if (Convert.ToDecimal(hdnWalletAmountUsed) > 0)
                    {
                        lShoppingCartCollection.lShoppingCartOrderDetails.WalletAmountUsed = Convert.ToDecimal(hdnWalletAmountUsed);
                        lShoppingCartCollection = this.CalculateMLMWalletAmount(lShoppingCartCollection);
                    }
                    //End MLM


                    if (Session["OrderCouponCode"] != null)
                    {
                        string userMessage = string.Empty;
                        int validityCode = 0;
                        long userLoginID = 0;
                        long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);
                        CouponManagement obj = new CouponManagement();
                        long cityId = Convert.ToInt64(Request.Cookies["CityCookie"].Value.Split('$')[0]);
                        int franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city
                        Session["OrderCouponAmount"] = Convert.ToDecimal(obj.VerifyCouponAgainstCart(lShoppingCartCollection.lShopWiseDeliveryCharges, Session["OrderCouponCode"].ToString(), out userMessage, out validityCode, userLoginID, cityId, franchiseId));////added cityId->franchiseId by Ashish for multiple MCO in same city
                        Session["OrderCouponCode"] = Session["OrderCouponCode"].ToString();
                        //lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount = Convert.ToDecimal(Session["OrderCouponAmount"]);
                        //lShoppingCartCollection.lShoppingCartOrderDetails.CoupenCode = Session["OrderCouponCode"].ToString();

                    }

                    //List<long> merId = lShoppingCartCollection.lShopProductVarientViewModel.Select(p => p.ShopID).Distinct().ToList();
                    //for (int i = 0; i < merId.Count; i++)
                    //{
                    //    var orgProducts = lShoppingCartCollection.lShopProductVarientViewModel.Select(x => new
                    //    {
                    //        x.ShopID,
                    //        x.ActualWeight
                    //    })
                    //   .Where(x => x.ShopID == merId[i]).ToList();
                    //    decimal orgProductWeight = 0;
                    //    for (int j = 0; j < orgProducts.Count; j++)
                    //    {
                    //        orgProductWeight = orgProductWeight + orgProducts[j].ActualWeight;
                    //    }
                    //    lShoppingCartCollection.lShopProductVarientViewModel[i].DeliveryCharge = dc.GetDeliveryCharges(txtPinCode, orgProductWeight, false);
                    //}
                    //=============== calculate delivery charges ======================
                    TempData["WarningMessage"] = "Items can be shipped to your location!!$green";
                    TempData["IsCheckPinCode"] = "true";
                    /* if (txtPinCode == "208016")
                     {
                         ///////////////////////////////////////
                         string lPincode = string.Empty;
                         //ModelState.AddModelError("PinCode", "Sorry! This items can't be delivered at your location!!");
                         // TempData["WarningMessage"] = "Sorry! This items can't be delivered at your location!!";
                         TempData["WarningMessage"] = "Items cannot be shipped to your location.$red";
                         TempData["IsCheckPinCode"] = "false";
                         if (ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"] != null)
                         {
                             lPincode = ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"].Value;
                         }
                         SetDeliverySchedule(lPincode);
                         DeleteDeliveryPincodeCookie();
                         return RedirectToAction("Index", "ShoppingCart");
                         ////////////////////////////////////////
                     }
                     else
                     {*/
                    SetDeliverySchedule(txtPinCode);
                    // }
                    return View("Index", lShoppingCartCollection);
                }
                else
                {
                    string lPincode = string.Empty;
                    //ModelState.AddModelError("PinCode", "Sorry! This items can't be delivered at your location!!");
                    // TempData["WarningMessage"] = "Sorry! This items can't be delivered at your location!!";
                    TempData["WarningMessage"] = "Items cannot be shipped to your location.$red";
                    TempData["IsCheckPinCode"] = "false";
                    if (ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"] != null)
                    {
                        lPincode = ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"].Value;
                    }
                    SetDeliverySchedule(lPincode);
                    DeleteDeliveryPincodeCookie();
                    return RedirectToAction("Index", "ShoppingCart");
                }


            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with verifying pincode!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShoppingCartController][POST:VerifyPincode]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return View("Index");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with verifying pincode!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShoppingCartController][POST:VerifyPincode]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return View("Index");
            }
        }


        //Added by Zubair for MLM on 19-01-2018
        public ActionResult EWalletAmountUsed(ShopProductVarientViewModelCollection model, string txtEWalletAmountUsed)
        {
            try
            {
                ShoppingCartInitialization sci = new ShoppingCartInitialization();
                ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();
                if (Session["IsEarnUse"] == null)
                {
                    lShoppingCartCollection = sci.GetCookie(fConnectionString, true);
                }
                else
                {
                    lShoppingCartCollection = sci.GetCookie(fConnectionString, (bool)Session["IsEarnUse"]);
                }

                string lPincode = string.Empty;
                HttpCookie EWalletAmountUsed = new HttpCookie("EWalletAmountUsed");
                long userLoginID = Convert.ToInt64(System.Web.HttpContext.Current.Session["UID"]);
                decimal walletAmount = 0;
                MLMWallet objWallet = new MLMWallet();
                var amount = Convert.ToDecimal(db.MLMWallets.Where(x => x.UserLoginID == userLoginID).Select(x => x.Amount).FirstOrDefault());//Remove IsActive and IsMlmUser by Sonali on 03-04-2019
                if (amount > 0)
                {
                    walletAmount = amount;
                }

                //Get Usable Amount Yashaswi 7-9-2018
                LeadersPayoutMaster objLeadersPayoutMaster = db.LeadersPayoutMasters.SingleOrDefault(p => p.ID == 1);
                if (objLeadersPayoutMaster != null)
                {
                    decimal minResAmt = Convert.ToDecimal(objLeadersPayoutMaster.Min_Resereved);
                    decimal Amt = amount - (amount * minResAmt) / 100;
                    Amt = Math.Round(Amt, 2);
                    
                    if (Amt < 0)
                    {
                        Amt = 0;
                    }
                    walletAmount = Amt;
                }

                //Get CashbackWallet Amount 03-10-2019

                CashbackWallet wallet = db.cashbackWallets.FirstOrDefault(p => p.UserLoginID == userLoginID);
                if (wallet != null)
                {
                    walletAmount = walletAmount + wallet.Amount;
                }

                decimal payableAmount = lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount + lShoppingCartCollection.lShoppingCartOrderDetails.WalletAmountUsed;

                if (txtEWalletAmountUsed == null || txtEWalletAmountUsed == "" || Convert.ToDecimal(txtEWalletAmountUsed) <= 0)
                {
                    txtEWalletAmountUsed = "0";
                    //TempData["MLMEWalletAmountUsedMessage"] = "Entered amount should be greater than 0";
                    EWalletAmountUsed.Value = txtEWalletAmountUsed.ToString();
                    System.Web.HttpContext.Current.Response.Cookies.Add(EWalletAmountUsed);

                    if (System.Web.HttpContext.Current.Request.Cookies["EWalletAmountUsed"] != null)
                    {
                        EWalletAmountUsed.Expires = DateTime.Now.AddDays(-1);
                        System.Web.HttpContext.Current.Response.Cookies.Add(EWalletAmountUsed);
                    }
                    if (EWalletAmountUsed.Expires < DateTime.Now)
                    {
                        System.Web.HttpContext.Current.Request.Cookies.Remove("EWalletAmountUsed");
                    }

                    System.Web.HttpContext.Current.Response.Cookies["EWalletAmountUsed"].Value = Convert.ToString(txtEWalletAmountUsed);
                    System.Web.HttpContext.Current.Response.Cookies.Add(System.Web.HttpContext.Current.Response.Cookies["EWalletAmountUsed"]);
                    System.Web.HttpContext.Current.Response.Cookies["EWalletAmountUsed"].Expires = System.DateTime.Now.AddDays(30);


                    if (ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"] != null)
                    {
                        lPincode = ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"].Value;
                        lShoppingCartCollection = sci.GetDeliveryCharge(lShoppingCartCollection);
                    }

                    lShoppingCartCollection.lShoppingCartOrderDetails.WalletAmountUsed = Convert.ToDecimal(txtEWalletAmountUsed);
                    lShoppingCartCollection = this.CalculateMLMWalletAmount(lShoppingCartCollection);
                    SetDeliverySchedule(lPincode);
                }
                else if (walletAmount < Convert.ToDecimal(txtEWalletAmountUsed))
                {
                    txtEWalletAmountUsed = "0";
                    //Yashaswi 6-9-2018 
                    TempData["MLMEWalletAmountUsedMessage"] = "Entered Amount should be less than EWallet Usable Amount.";
                    EWalletAmountUsed.Value = txtEWalletAmountUsed.ToString();
                    System.Web.HttpContext.Current.Response.Cookies.Add(EWalletAmountUsed);

                    //if (Session["IsEarnUse"] == null)
                    //{
                    //    lShoppingCartCollection = sci.GetCookie(fConnectionString, true);
                    //}
                    //else
                    //{
                    //    lShoppingCartCollection = sci.GetCookie(fConnectionString, (bool)Session["IsEarnUse"]);
                    //}

                    if (ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"] != null)
                    {
                        lPincode = ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"].Value;
                        lShoppingCartCollection = sci.GetDeliveryCharge(lShoppingCartCollection);
                    }

                    SetDeliverySchedule(lPincode);
                }
                else if (payableAmount < Convert.ToDecimal(txtEWalletAmountUsed))
                {
                    txtEWalletAmountUsed = "0";
                    TempData["MLMEWalletAmountUsedMessage"] = "Used E-wallet Amount can't be more than Payable Amount";
                    EWalletAmountUsed.Value = txtEWalletAmountUsed.ToString();
                    System.Web.HttpContext.Current.Response.Cookies.Add(EWalletAmountUsed);


                    if (ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"] != null)
                    {
                        lPincode = ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"].Value;
                        lShoppingCartCollection = sci.GetDeliveryCharge(lShoppingCartCollection);
                    }

                    SetDeliverySchedule(lPincode);
                }
                else
                {
                    if (System.Web.HttpContext.Current.Request.Cookies["EWalletAmountUsed"] != null)
                    {
                        EWalletAmountUsed.Expires = DateTime.Now.AddDays(-1);
                        System.Web.HttpContext.Current.Response.Cookies.Add(EWalletAmountUsed);
                    }
                    if (EWalletAmountUsed.Expires < DateTime.Now)
                    {
                        System.Web.HttpContext.Current.Request.Cookies.Remove("EWalletAmountUsed");
                    }

                    System.Web.HttpContext.Current.Response.Cookies["EWalletAmountUsed"].Value = Convert.ToString(txtEWalletAmountUsed);
                    System.Web.HttpContext.Current.Response.Cookies.Add(System.Web.HttpContext.Current.Response.Cookies["EWalletAmountUsed"]);
                    System.Web.HttpContext.Current.Response.Cookies["EWalletAmountUsed"].Expires = System.DateTime.Now.AddDays(30);



                    lShoppingCartCollection.lShoppingCartOrderDetails.WalletAmountUsed = Convert.ToDecimal(txtEWalletAmountUsed);
                    lShoppingCartCollection = this.CalculateMLMWalletAmount(lShoppingCartCollection);
                    TempData["MLMEWalletAmountUsedMessage"] = null;
                }
                lShoppingCartCollection.lShoppingCartOrderDetails = sci.CalculateCartAmount(lShoppingCartCollection);

                return View("Index", lShoppingCartCollection);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShoppingCartController][POST:EWalletAmountUsed]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return View("Index");
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShoppingCartController][POST:EWalletAmountUsed]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return View("Index");
            }
        }

        public ShopProductVarientViewModelCollection CalculateMLMWalletAmount(ShopProductVarientViewModelCollection model)
        {
            //ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();
            ShoppingCartInitialization sci = new ShoppingCartInitialization();
            try
            {
                decimal eWalletTotalAmount = 0;
                decimal eWalletAmountUsed = 0;

                eWalletTotalAmount = model.lMLMWallets.Amount;
                eWalletAmountUsed = model.lShoppingCartOrderDetails.WalletAmountUsed;
                if (System.Web.HttpContext.Current.Request.Cookies["EWalletAmountUsed"].Value == null || Convert.ToDecimal(System.Web.HttpContext.Current.Request.Cookies["EWalletAmountUsed"].Value) <= 0)
                {
                    model.lMLMWallets.Amount = eWalletTotalAmount - eWalletAmountUsed;
                }
                model.lShoppingCartOrderDetails.WalletAmountUsed = eWalletAmountUsed;

                string lPincode = string.Empty;
                if (ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"] != null)
                {
                    lPincode = ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"].Value;
                    model = sci.GetDeliveryCharge(model);
                }
                SetDeliverySchedule(lPincode);
                //DeleteDeliveryPincodeCookie();

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShoppingCartController][POST:CalculateMLMWalletAmount]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShoppingCartController][POST:CalculateMLMWalletAmount]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return model;
        }
        //End MLM


        //public ActionResult ContinueShopping()
        //{
        //        //return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        //        return RedirectToAction("Index", "Home");
        //}

        public ActionResult ContinueShopping()
        {
            string Url = "";
            string cityName = "";
            int franchiseID = 0;////added by Ashish for multiple MCO in same city
            try
            {
                /*----------Get Url from cookie to redirect to its previous position-------*/
                if (Request.Cookies["UrlCookie"] != null)
                {
                    Url = Request.Cookies["UrlCookie"].Value.ToString();
                }
                //return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value != string.Empty)
                {
                    cityName = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower().Trim();
                    franchiseID = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city
                    if (!Url.ToLower().Trim().Contains(cityName))
                    {
                        return RedirectToRoute("Home", new { city = cityName, franchiseId = franchiseID });////added franchiseId=franchiseID by Ashish for multiple MCO in same city
                    }
                }
                return Redirect(Url);
                /*----------End Get Url from cookie to redirect to its previous position-------*/
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with redirecting to Previous page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShoppingCartController][POST:ContinueShopping]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with redirecting to Previous page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShoppingCartController][POST:ContinueShopping]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return RedirectToAction("Index", "Home");
            }
        }



        public ActionResult VerifyCouponcode(string txtCouponCode, ShopProductVarientViewModelCollection model, string hdnWeekHoliday, string hdnMessage, string hdnMinOrder, string hdnWalletAmountUsed)
        {
            try
            {
                //-- For Dynamic Message --//
                ViewBag.WeekHoliday = hdnWeekHoliday;
                ViewBag.MinOrder = hdnMinOrder;
                string str = hdnMessage;
                str = str.Replace("|", " ");
                ViewBag.HolidayMessage = str;
                ///////////////////
                ShoppingCartInitialization sci = new ShoppingCartInitialization();
                ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();
                // lShoppingCartCollection = sci.GetCookie(fConnectionString);
                if (Session["IsEarnUse"] == null)
                {
                    lShoppingCartCollection = sci.GetCookie(fConnectionString, true);
                }
                else
                {
                    lShoppingCartCollection = sci.GetCookie(fConnectionString, (bool)Session["IsEarnUse"]);
                }
                if (ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"] != null)
                {
                    lShoppingCartCollection = sci.GetDeliveryCharge(lShoppingCartCollection);
                }
                else
                {
                    TempData["WarningMessage"] = "Please Check Pincode$red";
                }

                //Added by Zubair for MLM on 31-01-2018
                lShoppingCartCollection.lShoppingCartOrderDetails.WalletAmountUsed = Convert.ToDecimal(hdnWalletAmountUsed);
                lShoppingCartCollection = this.CalculateMLMWalletAmount(lShoppingCartCollection);
                //End MLM

                string userMessage = string.Empty;
                int validityCode = 0;
                long userLoginID = 0;
                long.TryParse(Convert.ToString(Session["UID"]), out userLoginID);
                CouponManagement obj = new CouponManagement();

                long cityId = Convert.ToInt64(Request.Cookies["CityCookie"].Value.Split('$')[0]);
                int franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city
                Session["OrderCouponCode"] = txtCouponCode;
                Session["OrderCouponAmount"] = Convert.ToDecimal(obj.VerifyCouponAgainstCart(lShoppingCartCollection.lShopWiseDeliveryCharges, txtCouponCode, out userMessage, out validityCode, userLoginID, cityId, franchiseId));////added cityId->franchiseId by Ashish for multiple MCO in same city
                //lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount = Convert.ToDecimal(Session["OrderCouponAmount"]);
                //lShoppingCartCollection.lShoppingCartOrderDetails.CoupenCode = txtCouponCode;

                TempData["CouponMessage"] = userMessage;
                SetDeliverySchedule();
                return View("Index", lShoppingCartCollection);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with verifying pincode!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShoppingCartController][POST:VerifyPincode]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return View("Index");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with verifying pincode!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShoppingCartController][POST:VerifyPincode]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return View("Index");
            }
        }

        //not in use
        private void SetDeliverySchedule()
        {
            try
            {
                DeliveryScheduleBLL lDeliveryScheduleBLL = new DeliveryScheduleBLL(System.Web.HttpContext.Current.Server);
                List<DeliverySchedule1> lDeliverySchedule = lDeliveryScheduleBLL.SetDeliverySchedule();
                ViewBag.DeliveryScheduleID = new SelectList(lDeliverySchedule, "delScheduleId", "date");
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetDeliverySchedule(string pincode)
        {
            try
            {
                DeliveryScheduleBLL lDeliveryScheduleBLL = new DeliveryScheduleBLL(System.Web.HttpContext.Current.Server);

                long cityId = Convert.ToInt64(Request.Cookies["CityCookie"].Value.Split('$')[0]);
                int franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city
                List<DeliveryScheduleViewModel> lDeliverySchedule = new List<DeliveryScheduleViewModel>();
                lDeliverySchedule = lDeliveryScheduleBLL.SetDeliverySchedule(cityId, pincode, franchiseId);////added franchiseId by Ashish for multiple MCO in same city
                ViewBag.DeliveryScheduleID = new SelectList(lDeliverySchedule, "delScheduleId", "date");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void DeleteDeliveryPincodeCookie()
        {
            try
            {
                HttpCookie DeliverablePincode = new HttpCookie("DeliverablePincode");

                //Delete whole cookie
                if (Request.Cookies["DeliverablePincode"] != null)
                {
                    DeliverablePincode.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(DeliverablePincode);
                }
                if (DeliverablePincode.Expires < DateTime.Now)
                {
                    Request.Cookies.Remove("DeliverablePincode");
                }
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartController][DeleteDeliveryPincodeCookie]", "Can't delete delivery pincode cookie!" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartController][DeleteDeliveryPincodeCookie]", "Can't delete delivery pincode cookie!" + Environment.NewLine + ex.Message);
            }
        }



        public ActionResult EarnAmountUse(bool checkValue)
        {
            ShoppingCartInitialization sci = new ShoppingCartInitialization();
            ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();


            if (checkValue == true)
            {
                lShoppingCartCollection = sci.GetCookie(fConnectionString, true);
                Session["IsEarnUse"] = true;
            }
            else
            {
                lShoppingCartCollection = sci.GetCookie(fConnectionString, false);
                Session["IsEarnUse"] = false;
            }
            SetDeliverySchedule();
            return View("Index", lShoppingCartCollection);
        }


        /// <summary>
        /// This function return current city Customer care number.
        /// </summary>
        private void GetCustCareNo()
        {
            CommonFunctions obj = new CommonFunctions();
            string telNo = obj.GetCustCareNo();
            TempData["CustCareNo"] = telNo;

            //try
            //{
            //    long cityID=4968;
            //    if (Request.Cookies["CityCookie"] != null && Request.Cookies["CityCookie"].Value != "")
            //    {
            //        cityID = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[0]);
            //    }
            //    telNo =db.HelpDeskDetails.Where(x=>x.CityID==cityID).Select(x=>x.HelpLineNumber).FirstOrDefault();
            //    TempData["CustCareNo"] = telNo;
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
            //return telNo;
        }

        /// <summary>
        /// Geting value from API BussinessLayer for Dynamic Message
        /// By Ashish
        /// </summary>
        /// <returns></returns>
        private List<WeeklySeasonalFestivalPageMessage> CallGetPageMessageAPI()
        {
            int FranchiseID = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2].Trim());

            List<WeeklySeasonalFestivalPageMessage> WSFMsg = new List<WeeklySeasonalFestivalPageMessage>();
            WSFMsg = FranchisePageMessages.GetFranchisePageMessage(FranchiseID);

            return WSFMsg;
        }


        //Yashaswi Start 02-02-2019 for BUG 11 to Verify cart 
        [HttpGet]
        public JsonResult VerifyCartOnOrderPLace()
        {
            int result = (new ShoppingCartInitialization()).VerifyCartOnOrderPLace();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //Yashaswi end 02-02-2019 for BUG 11 to Verify cart 
        //Added by Rumana on 9/03/2019
        public JsonResult GetDeliveryChargeNotification(decimal GrandTotal)
        {
            long franchiseID = 0;
            long cityId = Convert.ToInt64(Request.Cookies["CityCookie"].Value.Split('$')[0]);
            franchiseID = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city

            string Message = "";
            DeliveryCharges obj = new DeliveryCharges();
            Message = obj.CheckFreeDeliveryCharge(GrandTotal, cityId, franchiseID);
            return Json(Message, JsonRequestBehavior.AllowGet);
        }


    }

}