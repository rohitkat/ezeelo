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
using DeliveryPartner.Models.ViewModel;
using System.Collections;
using DeliveryPartner.Models;
using ModelLayer.Models.ViewModel;




namespace DeliveryPartner.Controllers
{
    public class DeliveryOrderDetailController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private DeliveryPartner.Models.ViewModel.DeliveryPartnerSessionViewModel deliveryPartnerSessionViewModel = new DeliveryPartner.Models.ViewModel.DeliveryPartnerSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            deliveryPartnerSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            deliveryPartnerSessionViewModel.Username = Session["UserName"].ToString();
            Common.Common.GetAllLoginDetailFromSession(ref deliveryPartnerSessionViewModel);
        }
        [SessionExpire]
        public ActionResult Index(string FromDate, string ToDate, int? page, long? DeliveryStatus, string SearchString = "", string DeliveryType = "")
        {
            SessionDetails();
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.DeliveryStatus1 = DeliveryStatus;
            ViewBag.DeliveryType1 = DeliveryType;

            //-- Add By Ashish Nagrale --//
            //-- For checking OTP is generated or NOT --//
            TempData["FromDate"] = FromDate;
            TempData["ToDate"] = ToDate;
            TempData["page"] = pageNumber;
            TempData["DeliveryStatus"] = DeliveryStatus;
            TempData["SearchString"] = SearchString;
            TempData["DeliveryType"] = DeliveryType;
            TempData.Keep();
            //-- End Add --//

            List<DeliveryTypeViewModel> DeliveryTypeViewModels = new List<DeliveryTypeViewModel>();
            DeliveryTypeViewModels.Add(new DeliveryTypeViewModel { ID = 1, Name = "NORMAL" });
            DeliveryTypeViewModels.Add(new DeliveryTypeViewModel { ID = 2, Name = "EXPRESS" });
            ViewBag.DeliveryType = new SelectList(DeliveryTypeViewModels, "Name", "Name");

            var Status = from DeliveryPartner.Common.Constant.ORDER_STATUS d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.ORDER_STATUS))
                         select new { ID = (int)d, Name = d.ToString() };

            ViewBag.DeliveryStatus = new SelectList(Status.Where(x => x.ID >= (int)Common.Constant.ORDER_STATUS.PACKED), "ID", "Name");



            var deliveryorderdetails = (from DOD in db.DeliveryOrderDetails
                                        join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
                                        join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID //Add By Ashish Nagrale for getting PaymentMode
                                        where DOD.ShopOrderCode == COD.ShopOrderCode &&
                                        COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS.PACKED &&
                                        DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID
                                        select new DeliveryIndexViewModel
                                        {
                                            ID = DOD.ID,
                                            DeliveryPartnerID = DOD.DeliveryPartnerID,
                                            GandhibaghOrderID = COD.CustomerOrderID,//Added by Mohit 19-10-15
                                            GandhibaghOrderCode = COD.CustomerOrder.OrderCode,
                                            ShopOrderCode = DOD.ShopOrderCode,
                                            Weight = DOD.Weight,
                                            OrderAmount = DOD.OrderAmount,
                                            DeliveryCharge = DOD.DeliveryCharge,
                                            GandhibaghCharge = DOD.GandhibaghCharge,
                                            DeliveryType = DOD.DeliveryType,
                                            IsMyPincode = DOD.IsMyPincode,
                                            IsActive = DOD.IsActive,
                                            CreateDate = DOD.CreateDate,
                                            CreateBy = DOD.CreateBy,
                                            ModifyDate = DOD.ModifyDate,
                                            ModifyBy = DOD.ModifyBy,
                                            NetworkIP = DOD.NetworkIP,
                                            DeviceType = DOD.DeviceType,
                                            DeviceID = DOD.DeviceID,


                                            //----------------------------- Extra added -//
                                            OrderStatus = COD.OrderStatus,
                                            DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
                                            DeliveryScheduleName = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
                                            PaymentMode = CO.PaymentMode//Add bt Ashish Nagrale
                                        }).Distinct().OrderByDescending(x => x.ID).ToList();

            //-----------------Added By Mohit----On 19-10-15---------------------------------//
            List<SubscriptionPlanUsedBy> lSubscriptionPlanUsedBies = new List<SubscriptionPlanUsedBy>();
            lSubscriptionPlanUsedBies = db.SubscriptionPlanUsedBies.ToList();

            foreach (DeliveryIndexViewModel lDeliveryIndexViewModel in deliveryorderdetails)
            {

                SubscriptionPlanUsedBy lSubscriptionPlanUsedBy = lSubscriptionPlanUsedBies.FirstOrDefault(x => x.CustomerOrderID == lDeliveryIndexViewModel.GandhibaghOrderID);

                if (lSubscriptionPlanUsedBy != null)
                {
                    lDeliveryIndexViewModel.GandhibaghOrderCode = lDeliveryIndexViewModel.GandhibaghOrderCode + "*";
                }

            }
            //-----------------End of Code By Mohit----On 19-10-15---------------------------------//
            //var deliveryorderdetails = db.DeliveryOrderDetails.Include(d => d.DeliveryPartner).Include(d => d.PersonalDetail).Include(d => d.PersonalDetail1).ToList().Where(x => x.DeliveryPartnerID == fUserId);
            if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
            {
               // DateTime lFromDate = DateTime.Now;
                //if (DateTime.TryParse(FromDate, out lFromDate)) { }
               // DateTime lToDate = DateTime.Now;
                //if (DateTime.TryParse(ToDate, out lToDate)) { }

                DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
                DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);
                deliveryorderdetails = deliveryorderdetails.Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).ToList();
            }
            SearchString = SearchString.Trim();
            if (SearchString != "")
            {
                deliveryorderdetails = deliveryorderdetails.Where(x => x.ShopOrderCode.ToString().Contains(SearchString) || x.GandhibaghOrderCode.ToString().Contains(SearchString)).ToList();

            }
            if (DeliveryType != "")
            {
                deliveryorderdetails = deliveryorderdetails.Where(x => x.DeliveryType.ToUpper().ToString() == DeliveryType.ToUpper()).ToList();
            }
            if (DeliveryStatus != null)
            {
                deliveryorderdetails = deliveryorderdetails.Where(x => x.OrderStatus == DeliveryStatus).ToList();
            }

            return View(deliveryorderdetails.ToPagedList(pageNumber, pageSize));
        }

        // GET: /DeliveryOrderDetail/Details/5
        [SessionExpire]
        public ActionResult Details(long? id)
        {
            #region validation
            SessionDetails();
            ViewBag.DeliveryPartnerID = deliveryPartnerSessionViewModel.DeliveryPartnerID;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryOrderDetail deliveryorderdetail = db.DeliveryOrderDetails.Find(id);
            if (deliveryorderdetail == null)
            {
                return HttpNotFound();
            }
            if (deliveryorderdetail.DeliveryPartnerID != deliveryPartnerSessionViewModel.DeliveryPartnerID)
            {
                return View("AccessDenied");
            }

            CustomerOrderDetail lCustomerOrderDetail = db.CustomerOrderDetails.FirstOrDefault(x => x.ShopOrderCode == deliveryorderdetail.ShopOrderCode);
            if (lCustomerOrderDetail == null)
            {
                return View("Error");
            }

            CustomerOrder lCustomerOrder = db.CustomerOrders.Find(lCustomerOrderDetail.CustomerOrderID);
            if (lCustomerOrder == null)
            {
                return View("Error");
            }

            PersonalDetail lPersonalDetail = db.PersonalDetails.FirstOrDefault(x => x.UserLoginID == lCustomerOrder.UserLoginID);
            if (lPersonalDetail == null)
            {
                return View("Error");
            }
            #endregion

            List<DeliveryPartner.Models.ViewModel.TrackOrderViewModel> TrackOrderViewModels = new List<DeliveryPartner.Models.ViewModel.TrackOrderViewModel>();
            if (db.TransactionInputs.Any(x => x.CustomerOrderDetailID == lCustomerOrderDetail.ID))
            {
                TrackOrderViewModels = (
                from CO in db.CustomerOrders
                join COD in db.CustomerOrderDetails on CO.ID equals COD.CustomerOrderID
                join DOD in db.DeliveryOrderDetails on COD.ShopOrderCode equals DOD.ShopOrderCode
                join SS in db.ShopStocks on COD.ShopStockID equals SS.ID
                join U in db.Units on SS.PackUnitID equals U.ID
                join SP in db.ShopProducts on SS.ShopProductID equals SP.ID
                join P in db.Products on SP.ProductID equals P.ID
                join UL in db.UserLogins on CO.UserLoginID equals UL.ID
                join PD in db.PersonalDetails on CO.UserLoginID equals PD.UserLoginID
                join TI in db.TransactionInputs on COD.ID equals TI.CustomerOrderDetailID
                where COD.ShopOrderCode.Equals(lCustomerOrderDetail.ShopOrderCode)
                select new DeliveryPartner.Models.ViewModel.TrackOrderViewModel
                {
                    OrderCode = CO.OrderCode,
                    ID = COD.ID,
                    ShopOrderCode = COD.ShopOrderCode,
                    ShopStockID = SS.ID,
                    ShopID = SP.ShopID,
                    CustomerOrderID = COD.CustomerOrderID,
                    FirstName = PD.FirstName,
                    MiddleName = PD.MiddleName,
                    LastName = PD.LastName,
                    CreateDate = CO.CreateDate,
                    Name = P.Name,
                    PackSize = SS.PackSize,
                    PackUnitName = U.Name,
                    CODQty = COD.Qty,
                    SaleRate = TI.SaleRatePerUnit,
                    RetailerRate = SS.RetailerRate,
                    TotalAmount = (COD.Qty * TI.SaleRatePerUnit),
                    OrderAmount = CO.OrderAmount,
                    GandhibaghCharge = DOD.GandhibaghCharge,
                    PayableAmount = CO.PayableAmount,
                    ProductID = P.ID,
                    MerchantCopyLandingPrice = (TI.LandingPriceByShopPerUnit == null) ? (TI.SaleRatePerUnit - 
                                                                                                            (decimal)(
                                                                                                                        (TI.SaleRatePerUnit * TI.ChargeINPercentByGBPerUnit / 100) +
                                                                                                                        ((TI.SaleRatePerUnit * TI.ChargeINPercentByGBPerUnit / 100) * TI.ServiceTAX / 100) 
                                                                                                                     )
                                                                                        ) 
                                                                                      : (decimal)TI.LandingPriceByShopPerUnit
                }).ToList();
            }
            else
            {
                TrackOrderViewModels = (
                           from CO in db.CustomerOrders
                           join COD in db.CustomerOrderDetails on CO.ID equals COD.CustomerOrderID
                           join DOD in db.DeliveryOrderDetails on COD.ShopOrderCode equals DOD.ShopOrderCode
                           join SS in db.ShopStocks on COD.ShopStockID equals SS.ID
                           join U in db.Units on SS.PackUnitID equals U.ID
                           join SP in db.ShopProducts on SS.ShopProductID equals SP.ID
                           join P in db.Products on SP.ProductID equals P.ID
                           join UL in db.UserLogins on CO.UserLoginID equals UL.ID
                           join PD in db.PersonalDetails on CO.UserLoginID equals PD.UserLoginID
                           where COD.ShopOrderCode.Equals(lCustomerOrderDetail.ShopOrderCode)
                           select new DeliveryPartner.Models.ViewModel.TrackOrderViewModel
                           {
                               OrderCode = CO.OrderCode,
                               ID = COD.ID,
                               ShopOrderCode = COD.ShopOrderCode,
                               ShopStockID = SS.ID,
                               ShopID = SP.ShopID,
                               CustomerOrderID = COD.CustomerOrderID,
                               FirstName = PD.FirstName,
                               MiddleName = PD.MiddleName,
                               LastName = PD.LastName,
                               CreateDate = CO.CreateDate,
                               Name = P.Name,
                               PackSize = SS.PackSize,
                               PackUnitName = U.Name,
                               CODQty = COD.Qty,
                               SaleRate = COD.SaleRate,
                               RetailerRate = SS.RetailerRate,
                               TotalAmount = COD.TotalAmount,
                               OrderAmount = CO.OrderAmount,
                               GandhibaghCharge = DOD.GandhibaghCharge,
                               PayableAmount = CO.PayableAmount,
                               ProductID = P.ID
                           }).ToList();
            }

            long lShopId = TrackOrderViewModels.FirstOrDefault().ShopID;
            Shop lShop = db.Shops.Find(lShopId);
            if (lShop == null)
            {
                return View("Error");
            }

            #region TAX
            //-- Start Tax Include on 31-march-2016 , By Avi Verma. 
            List<TaxOnOrderViewModel> lTaxOnOrderViewModels = (from tovm in TrackOrderViewModels
                                                               join too in db.TaxOnOrders on tovm.ID equals too.CustomerOrderDetailID
                                                               join PrdTax in db.ProductTaxes on too.ProductTaxID equals PrdTax.ID
                                                               join taxMas in db.TaxationMasters on PrdTax.TaxID equals taxMas.ID
                                                               select new TaxOnOrderViewModel
                                                               {
                                                                   TaxOnOrderID = too.ID,
                                                                   CustomerOrderDetailID = tovm.ID,
                                                                   ProductTaxID = PrdTax.ID,
                                                                   TaxAmount = too.Amount,
                                                                   TaxID = PrdTax.TaxID,
                                                                   TaxPrefix = taxMas.Prefix,
                                                                   TaxName = taxMas.Name
                                                               }).ToList();

            ViewBag.TaxOnOrderViewModels = lTaxOnOrderViewModels;
            //-- End Tax Include on 31-march-2016 , By Avi Verma. 
            #endregion

            DeliveryDetailViewModel lDeliveryDetailViewModel = new DeliveryDetailViewModel();
            lDeliveryDetailViewModel.ID = deliveryorderdetail.ID;
            lDeliveryDetailViewModel.ShopOrderCode = deliveryorderdetail.ShopOrderCode;
            lDeliveryDetailViewModel.DeliveryCharge = deliveryorderdetail.DeliveryCharge;
            //----Changes By Mohit------------on 06-18-2015----------------------------//
            lDeliveryDetailViewModel.GandhibaghCharge = deliveryorderdetail.GandhibaghCharge;
            //----End Changes By Mohit------------on 06-18-2015----------------------------//

            lDeliveryDetailViewModel.ShopName = lShop.Name;
            lDeliveryDetailViewModel.PickUpName = lShop.ContactPerson;
            lDeliveryDetailViewModel.PickUpAddress = lShop.Address;
            lDeliveryDetailViewModel.PickUpContact = lShop.Mobile;
            lDeliveryDetailViewModel.PickUpAlternateContact = lShop.Landline;

            lDeliveryDetailViewModel.DeliverToName = lPersonalDetail.Salutation.Name + ". " + lPersonalDetail.FirstName;
            lDeliveryDetailViewModel.DeliverToEmail = lCustomerOrder.UserLogin.Email;
            lDeliveryDetailViewModel.DeliverToAddress = lCustomerOrder.ShippingAddress;
            lDeliveryDetailViewModel.DeliveryToContact = lCustomerOrder.PrimaryMobile;
            lDeliveryDetailViewModel.DeliveryToAlternateContact = lCustomerOrder.SecondoryMobile;
            //----Changes By Mohit------------on 19-11-2015----------------------------//
            lDeliveryDetailViewModel.GandhibaghOrderCode = lCustomerOrder.OrderCode;
            //----End Changes By Mohit------------on 19-11-2015----------------------------//


            lDeliveryDetailViewModel.TrackOrderViewModels = TrackOrderViewModels;

            //------------------- new fields added on 08-sep-2015.
            //-- Changes made by AVI VERMA.
            //-- For Getting Mode of Payment. 
            //-- Reason : - If payment mode is online. then, for display on delivery memo.. COD = 0 Rs. As it is paid online.
            lDeliveryDetailViewModel.PaymentMode = lCustomerOrder.PaymentMode;


            return View("_Details", lDeliveryDetailViewModel);
        }
        [SessionExpire]
        public ActionResult Edit(long? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryOrderDetail deliveryorderdetail = db.DeliveryOrderDetails.Find(id);
            if (deliveryorderdetail == null)
            {
                return HttpNotFound();
            }

            if (deliveryorderdetail.DeliveryPartnerID != deliveryPartnerSessionViewModel.DeliveryPartnerID)
            {
                return View("AccessDenied");
            }

            var deliveryorderdetails = (from DOD in db.DeliveryOrderDetails
                                        join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
                                        join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID //Add By Ashish Nagrale for getting PaymentMode
                                        where DOD.ShopOrderCode == COD.ShopOrderCode &&
                                        COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS.PACKED &&
                                        DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&
                                        DOD.ID == id
                                        select new DeliveryIndexViewModel
                                        {
                                            ID = DOD.ID,
                                            DeliveryPartnerID = DOD.DeliveryPartnerID,
                                            GandhibaghOrderCode = CO.OrderCode,//Add by Ashish
                                            ShopOrderCode = DOD.ShopOrderCode,
                                            Weight = DOD.Weight,
                                            OrderAmount = DOD.OrderAmount,
                                            DeliveryCharge = DOD.DeliveryCharge,
                                            GandhibaghCharge = DOD.GandhibaghCharge,
                                            DeliveryType = DOD.DeliveryType,
                                            IsMyPincode = DOD.IsMyPincode,
                                            IsActive = DOD.IsActive,
                                            CreateDate = DOD.CreateDate,
                                            CreateBy = DOD.CreateBy,
                                            ModifyDate = DOD.ModifyDate,
                                            ModifyBy = DOD.ModifyBy,
                                            NetworkIP = DOD.NetworkIP,
                                            DeviceType = DOD.DeviceType,
                                            DeviceID = DOD.DeviceID,


                                            //----------------------------- Extra added -//
                                            OrderStatus = COD.OrderStatus,
                                            PaymentMode = CO.PaymentMode, //Add By Ashish Nagrale
                                            Assignment = "This Customer order having " + db.CustomerOrderDetails.Where(x => x.CustomerOrderID == COD.CustomerOrderID && x.OrderStatus >= 3).Select(x => x.ShopOrderCode).Distinct().Count() + " Merchant(s), "
                                                         + db.CustomerOrderDetails.Where(x => x.CustomerOrderID == COD.CustomerOrderID && x.OrderStatus == 6).Select(x => x.ShopOrderCode).Distinct().Count() + " Dispatched, "
                                                        + db.CustomerOrderDetails.Where(x => x.CustomerOrderID == COD.CustomerOrderID && x.OrderStatus == 7).Select(x => x.ShopOrderCode).Distinct().Count() + " Delivered, "
                                                          + db.CustomerOrderDetails.Where(x => x.CustomerOrderID == COD.CustomerOrderID && x.OrderStatus == 8).Select(x => x.ShopOrderCode).Distinct().Count() + " Returned and "
                                                           + db.CustomerOrderDetails.Where(x => x.CustomerOrderID == COD.CustomerOrderID && x.OrderStatus == 9).Select(x => x.ShopOrderCode).Distinct().Count() + " Cancelled."
                                            //Add By Ashish Nagrale
                                        }).Distinct().ToList();
            DeliveryIndexViewModel lDeliveryIndexViewModel = deliveryorderdetails.FirstOrDefault();
            if (lDeliveryIndexViewModel == null)
            {
                return HttpNotFound();
            }
            //-- Add By Ashish Nagrale --//
            // Hide from Ashish for Live
           /* string OrderCode = lDeliveryIndexViewModel.GandhibaghOrderCode;
            TempData["OrderCode"] = OrderCode;
            TempData["PaymentMode"] = lDeliveryIndexViewModel.PaymentMode;
            TempData.Keep();
            // ViewBag.MerchantOrderCode = deliveryorderdetails.FirstOrDefault().ShopOrderCode;
            ViewBag.IsChecked = false;
            string MDDC = deliveryorderdetails.FirstOrDefault().Assignment;
            ViewBag.MerchantDeliveryDispatchCount = MDDC;*/

            //-- End Add --//

            var Status = from DeliveryPartner.Common.Constant.ORDER_STATUS d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.ORDER_STATUS))
                         select new { ID = (int)d, Name = d.ToString() };
            ViewBag.DeliveryStatus = new SelectList(Status.Where(x => x.ID >= lDeliveryIndexViewModel.OrderStatus && x.ID <= lDeliveryIndexViewModel.OrderStatus + 1), "ID", "Name", lDeliveryIndexViewModel.OrderStatus);

            //--- changes made by avi verma. As discussed with team date : 28-july-2015
            if (lDeliveryIndexViewModel.OrderStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_GODOWN)
            {
                ViewBag.DeliveryStatus = new SelectList(Status.Where(x => x.ID >= lDeliveryIndexViewModel.OrderStatus && x.ID <= lDeliveryIndexViewModel.OrderStatus + 2), "ID", "Name", lDeliveryIndexViewModel.OrderStatus);
            }

            return View(lDeliveryIndexViewModel);
        }

        /* public JsonResult IsOTPAvailable(string OTP)
         {
             string OrderCode = TempData["OrderCode"].ToString();
             TempData.Keep();
             //string OrderCode = Request.QueryString["OrderCode"].ToString();

             return Json(db.OTPs.Where(x => x.OrderCode == OrderCode && x.OTP1 == OTP).Count() > 0 ? db.OTPs.Any(y => y.OrderCode == OrderCode && y.OTP1 == OTP) : false
                , JsonRequestBehavior.AllowGet);
         }*/

        // POST: /DeliveryOrderDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        
        public ActionResult Edit([Bind(Include = "ID,DeliveryStatus")] DeliveryIndexViewModel deliveryIndexViewModel, int DeliveryStatus) //Add by Ashish SendEmailSMS IsSendEmailSMSPartial  // Open from Ashish for Live
        ////public ActionResult Edit([Bind(Include = "ID,DeliveryStatus")] DeliveryIndexViewModel deliveryIndexViewModel, int DeliveryStatus, bool IsSendEmailSMS, bool IsSendEmailSMSPartial) //Add by Ashish SendEmailSMS IsSendEmailSMSPartial  // Hide from Ashish for Live
        {
            SessionDetails();
            DeliveryIndexViewModel lDeliveryIndexViewModel = new DeliveryIndexViewModel();
            try
            {
                var deliveryorderdetails = (from DOD in db.DeliveryOrderDetails
                                            join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
                                            join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID //Add By Ashish for getting PaymentMode
                                            where DOD.ShopOrderCode == COD.ShopOrderCode &&
                                            COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS.PACKED &&
                                            DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&
                                            DOD.ID == deliveryIndexViewModel.ID
                                            select new DeliveryIndexViewModel
                                            {
                                                ID = DOD.ID,
                                                DeliveryPartnerID = DOD.DeliveryPartnerID,
                                                ShopOrderCode = DOD.ShopOrderCode,
                                                Weight = DOD.Weight,
                                                OrderAmount = DOD.OrderAmount,
                                                DeliveryCharge = DOD.DeliveryCharge,
                                                GandhibaghCharge = DOD.GandhibaghCharge,
                                                DeliveryType = DOD.DeliveryType,
                                                IsMyPincode = DOD.IsMyPincode,
                                                IsActive = DOD.IsActive,
                                                CreateDate = DOD.CreateDate,
                                                CreateBy = DOD.CreateBy,
                                                ModifyDate = DOD.ModifyDate,
                                                ModifyBy = DOD.ModifyBy,
                                                NetworkIP = DOD.NetworkIP,
                                                DeviceType = DOD.DeviceType,
                                                DeviceID = DOD.DeviceID,


                                                //----------------------------- Extra added -//
                                                OrderStatus = COD.OrderStatus,
                                                PaymentMode = CO.PaymentMode,//Add By Ashish
                                                GandhibaghOrderCode = CO.OrderCode//Add By Ashish
                                            }).Distinct().ToList();
                lDeliveryIndexViewModel = deliveryorderdetails.FirstOrDefault();
                if (lDeliveryIndexViewModel == null)
                {
                    return HttpNotFound();
                }

                string OrderCode = lDeliveryIndexViewModel.GandhibaghOrderCode;//Add By Ashish
                //*****************Is this Order is Assigned**********************
                /*string shopordercode = lDeliveryIndexViewModel.ShopOrderCode;
                bool IsAssigned = IsAssigned(shopordercode);
                if(IsAssigned)
                    return View(deliveryIndexViewModel);
                else*/
                //*****************************************
                // this code written by prashant
                // To manage the dipaly of table into email
                System.Text.StringBuilder sbHtml = new System.Text.StringBuilder(
                    "<table border=\"0\" cellpadding=\"5\" cellspacing=\"0\" width=\"100%\" style=\"text-align: center; font-family: Calibri; font-size: 1.5vw; color: #4f4f4f;\">" +
                    "<thead>" +
                    "<tr>" +
                    "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">Preferred Delivery Time</th>" +
                    "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">Product Name</th>" +
                    "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">Product Quantity</th>" +
                    "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">Order Date</th>" +
                    "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">Delivery Type</th>" +
                    "</tr>" +
                    "</thead>" +
                    "<tbody>"
                    );

                List<CustomerOrderDetail> customerOrderDetails = db.CustomerOrderDetails.Where(x => x.ShopOrderCode == lDeliveryIndexViewModel.ShopOrderCode).ToList();
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
                    lCustomerOrderDetail.OrderStatus = DeliveryStatus;
                    lCustomerOrderDetail.ModifyDate = DateTime.Now;
                    lCustomerOrderDetail.ModifyBy = deliveryPartnerSessionViewModel.PersonalDetailID;
                    EzeeloDBContext db1 = new EzeeloDBContext();
                    db1.Entry(lCustomerOrderDetail).State = EntityState.Modified;
                    db1.SaveChanges();



                    //----------------------------------- Insert into CustomerOrderHistory -//
                    //EzeeloDBContext db2 = new EzeeloDBContext();
                    CustomerOrderHistory lCustomerOrderHistory = new CustomerOrderHistory();
                    lCustomerOrderHistory.CustomerOrderID = lCustomerOrderDetail.CustomerOrderID;
                    lCustomerOrderHistory.ShopStockID = lCustomerOrderDetail.ShopStockID;
                    lCustomerOrderHistory.Status = lCustomerOrderDetail.OrderStatus;
                    lCustomerOrderHistory.CreateBy = deliveryPartnerSessionViewModel.PersonalDetailID;
                    lCustomerOrderHistory.CreateDate = DateTime.Now;
                    db1.CustomerOrderHistories.Add(lCustomerOrderHistory);
                    db1.SaveChanges();
                    db1.Dispose();






                    var merchantProductList = (from cod in db.CustomerOrderDetails
                                               join ss in db.ShopStocks on cod.ShopStockID equals ss.ID
                                               join SP in db.ShopProducts on ss.ShopProductID equals SP.ID
                                               join p in db.Products on SP.ProductID equals p.ID
                                               join dod in db.DeliveryOrderDetails on cod.ShopOrderCode equals dod.ShopOrderCode
                                               where SP.ShopID == lCustomerOrderDetail.ShopID && cod.CustomerOrderID == lCustomerOrderDetail.CustomerOrderID

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
                               "<th style=\"border: 1px solid #b8b8b7; border-right:none; border-bottom:none;\">{0}</th>" +  // preffered time
                               "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{1}</th>" + // product name
                               "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{2}</th>" + // quantity
                                "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{3}</th>" + // Order date
                               "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{4}</th>" + // delivery type
                               "</tr>", "No Preffered Time Available", item.ProductName.ToString().Trim(), item.Qty, item.OrderDate.ToString("MMM dd, yyyy hh:mm tt"), item.DeliveryType.ToString().Trim()
                                );
                        }


                    }


                }


                var Status = from DeliveryPartner.Common.Constant.ORDER_STATUS d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.ORDER_STATUS))
                             select new { ID = (int)d, Name = d.ToString() };
                ViewBag.DeliveryStatus = new SelectList(Status.Where(x => x.ID >= lDeliveryIndexViewModel.OrderStatus && x.ID <= lDeliveryIndexViewModel.OrderStatus + 1), "ID", "Name", lDeliveryIndexViewModel.OrderStatus);

                //--- changes made by avi verma. As discussed with team date : 28-july-2015
                if (lDeliveryIndexViewModel.OrderStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_GODOWN)
                {
                    ViewBag.DeliveryStatus = new SelectList(Status.Where(x => x.ID >= lDeliveryIndexViewModel.OrderStatus && x.ID <= lDeliveryIndexViewModel.OrderStatus + 2), "ID", "Name", lDeliveryIndexViewModel.OrderStatus);
                }



                //----------------------------------- write logs -//
                DeliveryDetailLog lDeliveryDetailLog = new DeliveryDetailLog();
                lDeliveryDetailLog.DeliveryOrderDetailID = lDeliveryIndexViewModel.ID;
                lDeliveryDetailLog.Description = "STATUS CHANGED : " + Enum.GetName(typeof(Common.Constant.ORDER_STATUS), DeliveryStatus);
                lDeliveryDetailLog.CreateDate = DateTime.Now;
                lDeliveryDetailLog.CreateBy = deliveryPartnerSessionViewModel.PersonalDetailID;

                db.DeliveryDetailLogs.Add(lDeliveryDetailLog);
                db.SaveChanges();


                string deliveryType = "Normal";

                try
                {
                    deliveryType = lDeliveryDetailLog.DeliveryOrderDetail.DeliveryType;
                }
                catch { deliveryType = "Normal"; }


                sbHtml.Append("</tbody>" +
                              "</table>");

                //-----------Calculate Sum Amount of ShopOderCode who's OTP is not generated ------------//
                // Hide from Ashish for Live
                /*string lShopOrderCode = customerOrderDetails.FirstOrDefault().ShopOrderCode;
                long ID = customerOrderDetails.FirstOrDefault().CustomerOrderID;
                long customerOrderDetailID = customerOrderDetails.FirstOrDefault().ID;// For Exclusive Tax
                List<string> listShopOrderCode = new List<string>();//For generating multiple OTP
                listShopOrderCode.Add(lShopOrderCode);

                long Shopcount = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == ID).Select(x => x.ShopOrderCode).Distinct().Count();
                decimal TotalPayableAmount = db.CustomerOrders.Where(x => x.OrderCode == OrderCode).FirstOrDefault().PayableAmount;
                decimal ExclusiveTax = db.TaxOnOrders.Where(x => x.CustomerOrderDetailID == customerOrderDetailID).Count() > 0 ? db.TaxOnOrders.Where(x => x.CustomerOrderDetailID == customerOrderDetailID).Sum(x => x.Amount) : 0; //Add Exclusive Tax
                decimal SumOrderAmount = customerOrderDetails.Sum(x => x.TotalAmount);//Current Amount
                // if (ExclusiveTax!=null)
                SumOrderAmount += ExclusiveTax;//Current Amount + Exclusive Tax

                if (Shopcount > 1)
                {
                    List<string> ShopOrderList = new List<string>();
                    ShopOrderList = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == ID && x.OrderStatus >= 2 && x.OrderStatus <= 7).Select(x => x.ShopOrderCode).Distinct().ToList();
                    decimal SumOfDispatchAmountWithOTP = 0; //Dispatched from Godown with OTP
                    foreach (string List in ShopOrderList)
                    {
                        int orderStatus = db.CustomerOrderDetails.Where(x => x.ShopOrderCode == List && x.ShopOrderCode != lShopOrderCode && x.OrderStatus == 6).Select(x => x.OrderStatus).Distinct().Count();// Only for those which are dispatch from godown without OTP generate but not for current lShopOrderCode(B'coz it got updated its orderstatus to 6
                        int IsOTPGenerated = db.OTPs.Where(x => x.ShopOrderCode == List).Count();
                        if (IsOTPGenerated != 0 || orderStatus != 0)
                        {
                            //-- Only for wihtout OTP generated merchant (lShopOrderCode) --//
                            if (orderStatus != 0 && IsOTPGenerated == 0)
                            {
                                //-- Add previous dispatched from godown (without OTP) Amount  to current Amount. --//
                                SumOrderAmount += db.CustomerOrderDetails.Where(x => x.ShopOrderCode == List).Sum(x => x.TotalAmount);
                                listShopOrderCode.Add(List);
                            }
                            else if (IsOTPGenerated != 0)//if
                            {
                                SumOfDispatchAmountWithOTP += db.CustomerOrderDetails.Where(x => x.ShopOrderCode == List).Sum(x => x.TotalAmount);
                            }
                            decimal CurrentPlusDeliveredAmount = SumOrderAmount + SumOfDispatchAmountWithOTP;
                            if (CurrentPlusDeliveredAmount > TotalPayableAmount)
                            {
                                //-- Calculation for deduction of Coupon amount which is included in PayableAmount of the order. --//
                                SumOrderAmount = SumOrderAmount - (CurrentPlusDeliveredAmount - TotalPayableAmount);
                                if (SumOrderAmount < 0)
                                    SumOrderAmount = 0;
                            }
                        }
                    }
                }
                */
                // string[] ArrylistShopOrderCode = listShopOrderCode.ToArray();
                //-----------End Calculate Sum Amount of ShopOderCode who's OTP is not generated ------------//


                //--------------------------------- SMS & Email Function call --------------------------------//
                ////if ((IsSendEmailSMS || IsSendEmailSMSPartial) && (DeliveryStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_GODOWN || DeliveryStatus == (int)Common.Constant.ORDER_STATUS.DELIVERED))//Add by Ashish IsSendEmailSMS & IsSendEmailSMSPartial // Hide from Ashish for Live
                if ((DeliveryStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_GODOWN || DeliveryStatus == (int)Common.Constant.ORDER_STATUS.DELIVERED))//Add by Ashish IsSendEmailSMS & IsSendEmailSMSPartial // Open from Ashish for Live
                {
                    string lEmail = customerOrderDetails.FirstOrDefault().CustomerOrder.UserLogin.Email;
                    string lShopOrderCode = customerOrderDetails.FirstOrDefault().ShopOrderCode;//Hide //Open EPOD from Ashish for Live
                    string lOrderDate = customerOrderDetails.FirstOrDefault().CreateDate.ToString();
                    string lMobile = customerOrderDetails.FirstOrDefault().CustomerOrder.UserLogin.Mobile;
                    string lOrderAmount = customerOrderDetails.FirstOrDefault().CustomerOrder.OrderAmount.ToString();//Hide //Open EPOD from Ashish for Live

                    //-- Add By Ashish Nagrale --//
                    //Hide EPOD from Ashish for Live
                    /*string lOrderAmount = "";
                    if (IsSendEmailSMS)
                        //lOrderAmount = customerOrderDetails.FirstOrDefault().CustomerOrder.PayableAmount.ToString();//Add By Ashish Nagrale
                        lOrderAmount = TotalPayableAmount.ToString();
                    else if (IsSendEmailSMSPartial)
                    {
                        //long lOrderCode = customerOrderDetails.FirstOrDefault().CustomerOrder.ID = ID;
                        //lOrderAmount = customerOrderDetails.Sum(x=>x.TotalAmount).ToString();//Add By Ashish Nagrale
                        lOrderAmount = SumOrderAmount.ToString();
                    }*/
                    // End Add //
                    //lEmail = "avirakeshverma@gmail.com";
                    //lMobile = "9028415132";
                    //lEmail = "nagraleashish@yahoo.com";
                    //lMobile = "7276274344";

                    //------Declartion Email---------//
                    BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    bool Flag = false;

                    //-- Start Add By Ashish Nagrale --//
                    //Hide EPOD from Ashish for Live
                    /*Dictionary<string, string> SmsEmailOtp = new Dictionary<string, string>();
                    try
                    {
                        // Sending email to the user
                        SmsEmailOtp = BusinessLogicLayer.OTP.GenerateOTP("DPS");//CDOTP

                    }
                    catch (BusinessLogicLayer.MyException myEx)
                    {
                        ModelState.AddModelError("Message", "Customer Order/OTP Confirm Succesfully, there might be problem sending SMS, please check your mobile or contact administrator!");//added
                        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send SMS..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH, BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                        // throw new Exception("Unable to Send Email");
                    }*/
                    //End Add
                    try
                    {
                        //------Key value add in Email---------//
                        string city = "nagpur";
                        int franchiseID = 2;////added for Multiple MCO in Same City
                        if (System.Web.HttpContext.Current.Request.Cookies["CityCookie"] != null && System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value != "")
                        {
                            city = System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                            franchiseID = Convert.ToInt32(System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[2]); ////added for Multiple MCO in Same City
                        }
                       
                        dictionary.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseID + "/login");////added "/" + franchiseID + for Multiple MCO in Same City
                        dictionary.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseID + "/cust-o/my-order");////added "/" + franchiseID + for Multiple MCO in Same City
                        dictionary.Add("<!--NAME-->", lEmail);
                        //dictionary.Add("<!-- ORDER_DETAILS -->", sbHtml.ToString());
                        //dictionary.Add("<!--ORDER_NO-->", lShopOrderCode);
                        //dictionary.Add("<!--ORDER_DATE-->", lOrderDate);
                        if (DeliveryStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_GODOWN)
                        {
                            //Add by Ashish Nagrale
                            //Hide EPOD from Ashish for Live
                            try
                            {
                                //------Key value add in Email---------//
                                //dictionary.Add("<!--NAME-->", lEmail);
                                if (DeliveryStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_GODOWN)
                                {
                                    dictionary.Add("<!--ORDER_NO-->", OrderCode); //lShopOrderCode
                                    dictionary.Add("<!--ORDER_DATE-->", lOrderDate);
                                    //dictionary.Add("#--TIME--#", DateTime.Now.ToShortTimeString());
                                  
                                    //Commented by Tejaswee on 19-9-2016
                                   // dictionary.Add("#--D_OTP--#", SmsEmailOtp["OTP"]);//added

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
                                   // Flag = gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_ORD_DSPTCH, new string[] { lEmail }, dictionary, true); 
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                           // Flag = gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_ORD_DLVRD, new string[] { lEmail }, dictionary, true); 
                        }
                        else if (DeliveryStatus == (int)Common.Constant.ORDER_STATUS.DELIVERED)
                        {
                            Flag = gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_ORD_DLVRD, new string[] { lEmail }, dictionary, true);
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
                        //------Key value add in SMS---------//
                        dictionary.Add("#--NAME--#", lEmail);
                        dictionary.Add("#--ORD_NUM--#", lShopOrderCode);
                        if (DeliveryStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_GODOWN)
                        {
                            //-- Add by Ashish --//
                            //Hide EPOD from Ashish for Live
                            /*listShopOrderCode.Add(lOrderAmount);
                            string[] ArrylistShopOrderCode = listShopOrderCode.ToArray();
                            dictionary.Add("#--D_OTP--#", SmsEmailOtp["OTP"]);
                            BusinessLogicLayer.OTP insertOtp = new BusinessLogicLayer.OTP();
                            insertOtp.InsertOTPDetails(SmsEmailOtp["USC"], SmsEmailOtp["OTP"], ArrylistShopOrderCode);
                            */
                            //-- End Add --//

                            //dictionary.Add("#--TIME--#", DateTime.Now.ToShortTimeString());
                            //dictionary.Add("#--AMOUNT--#", lOrderAmount);
                            //===== added by Tejaswee ======//
                            //=======Get Payment mode from customerOrder table ==============//
                            long custOrderID = customerOrderDetails.FirstOrDefault().CustomerOrderID;
                            string payMode = db.CustomerOrders.Where(x => x.ID == custOrderID).Select(x => x.PaymentMode).FirstOrDefault();

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

                            //Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_DSPTCH, new string[] { lMobile, rcKey.DEFAULT_ALL_SMS }, dictionary);
                        }
                        else if (DeliveryStatus == (int)Common.Constant.ORDER_STATUS.DELIVERED)
                        {
                            Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_DLVRD, new string[] { lMobile }, dictionary);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                //--------------------------------------------------------------------------------------------//

























                //------------------------ refresh parent window -//
                Response.Write("<script>parent.location.reload();</script>");
                return View(deliveryIndexViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with the delivery index values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryIndex][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);

                var Status = from DeliveryPartner.Common.Constant.ORDER_STATUS d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.ORDER_STATUS))
                             select new { ID = (int)d, Name = d.ToString() };
                ViewBag.DeliveryStatus = new SelectList(Status.Where(x => x.ID >= lDeliveryIndexViewModel.OrderStatus && x.ID <= lDeliveryIndexViewModel.OrderStatus + 1), "ID", "Name", lDeliveryIndexViewModel.OrderStatus);

                //--- changes made by avi verma. As discussed with team date : 28-july-2015
                if (lDeliveryIndexViewModel.OrderStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_GODOWN)
                {
                    ViewBag.DeliveryStatus = new SelectList(Status.Where(x => x.ID >= lDeliveryIndexViewModel.OrderStatus && x.ID <= lDeliveryIndexViewModel.OrderStatus + 2), "ID", "Name", lDeliveryIndexViewModel.OrderStatus);
                }


                return View(deliveryIndexViewModel);
            }
        }

        //---------------------------------------Hide EPOD from Ashish for Live----------------------------------------------------------
        /*
        /// <summary>
        /// Add by Ashish
        /// 
        /// </summary>
        /// <param name="OrderCode"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [SessionExpire]
        public JsonResult IsOTPGenerated(string OrderCode, long ID)
        {
            string FromDate1 = TempData["FromDate"] != null ? TempData["FromDate"].ToString() : null;
            string ToDate1 = TempData["ToDate"] != null ? TempData["ToDate"].ToString() : null;
            string SearchString1 = TempData["SearchString"] != null ? TempData["SearchString"].ToString() : "";
            string DeliveryType1 = TempData["DeliveryType"] != null ? TempData["DeliveryType"].ToString() : "";
            int? page1 = TempData["page"] != null ? (int)TempData["page"] : 1;
            long? DeliveryStatus1 = TempData["DeliveryStatus"] != null ? (long)TempData["DeliveryStatus"] : DeliveryStatus1 = null;
            TempData["IsUpdateStatusClicked"] = true;
            TempData.Keep();

            int CountShopOrderCode = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == ID && x.OrderStatus >= 3).Count();//Upto Cancelled
            ////int CountShopOrderCodeDelivered = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == ID && x.OrderStatus >= 8).Count();//delivered, Returned & Cancelled
            ////int CountOTP = db.OTPs.Where(x => x.OrderCode == OrderCode).Count();


            return Json(db.OTPs.Where(x => x.OrderCode == OrderCode).Count() > 0 ? db.OTPs.Where(x => x.OrderCode == OrderCode).Count() + CountShopOrderCode : 0 + CountShopOrderCode
                 , JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Add by Ashish
        /// </summary>
        /// <param name="ShopOrderCode"></param>
        /// <returns></returns>
        [HttpPost]
        [SessionExpire]
        public bool IsJobAssigned(string ShopOrderCode)
        {
            int checkAssign = db.EmployeeAssignment.Where(x => x.ShopOrderCode == ShopOrderCode && x.EmployeeCode != null).Count();
            if (checkAssign > 0)
                return true;
            else
                return false;
        }
        public bool IsDifferentDateAndScheduleAssigned(string OrderCode, string ShopOrderCode)
        {
            // var TotalMerchant = db.EmployeeAssignment.Where(x => x.OrderCode == OrderCode && x.OrderStatus == 6 && x.EmployeeCode != null).Count();

            //-- For Current Order Date --//
            var curentOrderCode = (from EA in db.EmployeeAssignment
                                   where EA.OrderCode == OrderCode && EA.ShopOrderCode == ShopOrderCode
                                   group EA by EA.DeliveryDate into g
                                   select new { DeliveryDate = g.Key.ToString().Substring(0, 10), ShopOrderCode = g.ToList() }).ToList();
            // db.EmployeeAssignment.Where(x => x.OrderCode == OrderCode && x.ShopOrderCode==ShopOrderCode).Select(x => x.DeliveryDate).FirstOrDefault();
            //-- For Current Order Schedule --//
            var curentOrderCode2 = db.EmployeeAssignment.Join(db.CustomerOrders,
                                    EA => EA.OrderCode, CO => CO.OrderCode,
                                    (EA, CO) => new
                                    {
                                        ID = CO.ID,
                                        OrderCode = EA.OrderCode,
                                        ShopOrderCode = EA.ShopOrderCode

                                    }).Join(db.OrderDeliveryScheduleDetails,
                                        a => a.ID, ODSD => ODSD.CustomerOrderID,
                                        (a, ODSD) => new
                                        {
                                            DeliveryScheduleID = ODSD.DeliveryScheduleID,
                                            OrderCode = a.OrderCode,
                                            ShopOrderCode = a.ShopOrderCode
                                        }).Join(db.DeliverySchedules,
                                            b => b.DeliveryScheduleID, DS => DS.ID,
                                            (b, DS) => new
                                            {
                                                ID = DS.ID,
                                                OrderCode = b.OrderCode,
                                                ShopOrderCode = b.ShopOrderCode
                                            }
                                        ).Where(x => x.OrderCode == OrderCode && x.ShopOrderCode == ShopOrderCode).Select(x => x.ID).FirstOrDefault();



            var CurrentOrderDate = curentOrderCode.FirstOrDefault().DeliveryDate.ToString();
            var CurrentOrderSchedule = curentOrderCode2.ToString();
            var innerquery = db.OTPs.Where(x => x.OrderCode == OrderCode).Select(x => x.ShopOrderCode);

            var DatewiseOrder = (from EA in db.EmployeeAssignment
                                 where EA.OrderCode == OrderCode && EA.OrderStatus == 6 && EA.EmployeeCode != null && EA.DeliveryDate != null
                                 && !innerquery.Contains(EA.ShopOrderCode)
                                 group EA by EA.DeliveryDate into g
                                 select new { DeliveryDate = g.Key.ToString().Substring(0, 10), ShopOrderCode = g.ToList() }).ToList();

            var SchedulewiseOrder = (from EA in db.EmployeeAssignment
                                     join CO in db.CustomerOrders on EA.OrderCode equals CO.OrderCode
                                     join ODSD in db.OrderDeliveryScheduleDetails on CO.ID equals ODSD.CustomerOrderID
                                     join DS in db.DeliverySchedules on ODSD.DeliveryScheduleID equals DS.ID
                                     where EA.OrderCode == OrderCode && EA.OrderStatus == 6 && EA.EmployeeCode != null && EA.DeliverySchedule != null && ODSD.IsActive == true && DS.IsActive == true
                                     && !innerquery.Contains(EA.ShopOrderCode)
                                     group EA by DS.ID into g
                                     select new { DeliveryScheduleID = g.Key, DisplayName = g.ToList() }).ToList();
            if (DatewiseOrder.Count() == 0 && SchedulewiseOrder.Count() == 0)// For First record OrderStatus change
            {
                return true;
            }
            else if (DatewiseOrder.Count() == 1 && SchedulewiseOrder.Count() == 1)//For Sigle Date and Single Schedule
            {
                var OrderStatus6Date = DatewiseOrder.FirstOrDefault().DeliveryDate.ToString();
                var OrderStatus6Schedule = SchedulewiseOrder.FirstOrDefault().DeliveryScheduleID.ToString();

                if (CurrentOrderDate == OrderStatus6Date && OrderStatus6Schedule == CurrentOrderSchedule)
                    return true;
                else
                    return false;
            }
            else if (DatewiseOrder.Count() == 1 && SchedulewiseOrder.Count() > 1)//For Sigle Date and Multiple Schedule
                return false;
            else if (DatewiseOrder.Count() > 1)// For Multiple Date
            {

                foreach (var lst in DatewiseOrder)
                {
                    var SchedulewiseOrder2 = (from EA in db.EmployeeAssignment
                                              join CO in db.CustomerOrders on EA.OrderCode equals CO.OrderCode
                                              join ODSD in db.OrderDeliveryScheduleDetails on CO.ID equals ODSD.CustomerOrderID
                                              join DS in db.DeliverySchedules on ODSD.DeliveryScheduleID equals DS.ID
                                              where EA.OrderCode == OrderCode && EA.OrderStatus == 6 && EA.EmployeeCode != null && EA.DeliverySchedule != null && ODSD.IsActive == true && DS.IsActive == true
                                              && !innerquery.Contains(EA.ShopOrderCode)
                                              group EA by DS.ID into g
                                              select new { DeliveryScheduleID = g.Key, DisplayName = g.ToList() }).ToList();
                    if (SchedulewiseOrder2.Count() == 1)// For Multiple Date and Single Schedule
                    {
                        var OrderStatus6Schedule = SchedulewiseOrder2.FirstOrDefault().DisplayName.ToString();

                        if (OrderStatus6Schedule == CurrentOrderSchedule)
                            return true;
                        else
                            return false;
                    }

                    else if (SchedulewiseOrder2.Count() > 1)// For Multiple Date and Multiple Schedule
                        return false;
                    //else
                    //    return true;
                }
            }
            //  else
            return false;  //For any fail like ODSD.IsActive == true && DS.IsActive == true
        }
        /// <summary>
        /// Add by Ashish
        /// Same delivery boy should assign the job for multiple merchant of same customer. 
        /// As single OPT and COD amount is generated for multiple merchant of same customer.
        /// </summary>
        /// <param name="ShopOrderCode"></param>
        /// <returns></returns>
        public bool IsAssignToDifferentPerson(string OrderCode, string ShopOrderCode)
        {
            var curentOrderCode = db.EmployeeAssignment.Where(x => x.ShopOrderCode == ShopOrderCode).Select(x => x.EmployeeCode).FirstOrDefault();

            var innerquery = db.OTPs.Where(x => x.OrderCode == OrderCode).Select(x => x.ShopOrderCode);
            var checkdifferentPerson = (from EA in db.EmployeeAssignment
                                        where EA.OrderCode == OrderCode && EA.EmployeeCode != null && EA.OrderStatus == 6  //Only for Dispatch from Godown status for generating single OTP. This will work for CRM/Frenchise also.
                                       && !innerquery.Contains(EA.ShopOrderCode)
                                        group EA by EA.EmployeeCode into g
                                        select new { EmployeeCode = g.Key }).ToList();
            var queryOrderCode = checkdifferentPerson.Select(x => x.EmployeeCode).FirstOrDefault();
            if (checkdifferentPerson.Count() > 1)
                return false;
            else if (checkdifferentPerson.Count() == 1 && curentOrderCode.ToString() != queryOrderCode.ToString())
                return false;
            else
                return true;

        }

        public bool IsTaskTypeMatach(string OrderCode, string ShopOrderCode)
        {
            var curentTaskType = db.EmployeeAssignment.Where(x => x.ShopOrderCode == ShopOrderCode && x.EmployeeCode != null).Select(x=>x.DeliveredType).FirstOrDefault();
            var curentOrderStatus = db.CustomerOrderDetails.Where(x => x.ShopOrderCode == ShopOrderCode ).Select(x => x.OrderStatus).FirstOrDefault();
            if (curentTaskType == "DELIVERY" && curentOrderStatus == 5) //before orderstatus become 6
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        */
        //----------------------------------------------------------------------------------------------------------------
        
        /* private bool IsAssigned(string ShopOrderCode)// use for future
         {
             int checkAssign = db.EmployeeAssignment.Where(x => x.ShopOrderCode == ShopOrderCode).Count();
             if (checkAssign > 0)
                 return true;
             else
                 return false;
         }*/
    }
}
