
using Franchise.Models;
using Franchise.Models.ViewModel;
using Franchise.SalesOrder;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;


namespace Franchise.Controllers
{
    public class CustomerOrderDetailAlignmentController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
        public void SessionDetails()
        {
            try
            {
                customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
                customerCareSessionViewModel.Username = Session["USER_NAME"].ToString();
                customerCareSessionViewModel.PersonalDetailID = Convert.ToInt64(Session["PERSONAL_ID"]);
                Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[SessionDetails]", "Can't assign Session Details..!" + Environment.NewLine + myEx.Message);
            }
        }

        #region INDEX
        //
        // GET: /CustomerOrderDetailAlignment/
        [CustomAuthorize(Roles = "CustomerOrderDetailAlignment/CanRead")]
        public ActionResult Index(long COID)
        {
            CustomerOrderAndItsCustomerOrderDetailViewModel lCustomerOrderAndItsCustomerOrderDetailViewModel = Get(COID);
            GetwayPaymentTransaction lGetwayPaymentTransaction = db.GetwayPaymentTransactions.FirstOrDefault(x => x.CustomerOrderID == COID);
            ViewBag.GetwayPaymentTransaction = lGetwayPaymentTransaction;
            List<CustomerOrderDetail> lCustomerOrderDetails = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == COID && x.IsActive == true).ToList();
            List<TaxOnOrderViewModel> lTaxOnOrderViewModels = getTaxOnOrderViewModels(COID);
            ViewBag.TaxOnOrderViewModel = lTaxOnOrderViewModels;


            decimal lTotalDiscount = 0;
            List<TotalDiscountViewModel> lTotalDiscountViewModels = (from cod in db.CustomerOrderDetails
                                                                     join codlog in db.CustomerOrderDetailLogs on cod.ShopOrderCode equals codlog.ReferenceShopOrderCode
                                                                     where cod.CustomerOrderID == COID
                                                                     && codlog.ShopOrderCode == cod.ID + "RF0"
                                                                     select new TotalDiscountViewModel
                                                                     {
                                                                         CustomerOrderID = cod.CustomerOrderID,
                                                                         CustomerOrderDetailID = cod.ID,
                                                                         CustomerOrderDetailShopOrderCode = cod.ShopOrderCode,
                                                                         CustomerOrderDetailLogID = codlog.ID,
                                                                         CustomerOrderDetailLogShopOrderCode = codlog.ShopOrderCode,
                                                                         SaleRate = cod.SaleRate,
                                                                         SaleRateLog = codlog.SaleRate,
                                                                         CurrentQty = cod.Qty,
                                                                         Discount = (codlog.SaleRate - cod.SaleRate) * cod.Qty
                                                                     }).ToList();

            if (lTotalDiscountViewModels != null && lTotalDiscountViewModels.Count > 0)
            {
                lTotalDiscount = lTotalDiscountViewModels.Sum(x => x.Discount);
            }
            ViewBag.TotalDiscount = lTotalDiscount;

            if (db.EarnDetails.FirstOrDefault(x => x.CustomerOrderID == COID) != null)
            {
                ViewBag.EarnedAmount = db.EarnDetails.FirstOrDefault(x => x.CustomerOrderID == COID).UsedAmount;
            }


            //CustomerOrderLog lCustomerOrderLog=db.CustomerOrderLogs.Where(x=>x.ReferenceCustomerOrderID==COID && x.OrderCode==COID+"RF0").FirstOrDefault();
            //ViewBag.TotalDiscount = ((lCustomerOrderLog.OrderAmount - lCustomerOrderAndItsCustomerOrderDetailViewModel.customerOrderViewModel.OrderAmount > 0) ? lCustomerOrderLog.OrderAmount - lCustomerOrderAndItsCustomerOrderDetailViewModel.customerOrderViewModel.OrderAmount : 0);
            lCustomerOrderAndItsCustomerOrderDetailViewModel.CustomerOrderDetailViewModels = lCustomerOrderAndItsCustomerOrderDetailViewModel.CustomerOrderDetailViewModels.OrderBy(x => x.ShopOrderCode).ToList();
            return View(lCustomerOrderAndItsCustomerOrderDetailViewModel);
        }
        #endregion

        #region SHIPPING ADDRESS

        [SessionExpire]
        // GET: /CustomerOrderDetailAlignment/EditShippingAddress/5
        [CustomAuthorize(Roles = "CustomerOrderDetailAlignment/CanRead")]
        public ActionResult EditShippingAddress(long id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelLayer.Models.CustomerOrder customerorder = db.CustomerOrders.Find(id);
            if (customerorder == null)
            {
                return HttpNotFound();
            }
            ViewBag.Pincode = "";
            ModelLayer.Models.Pincode lPincode = db.Pincodes.Find(customerorder.PincodeID);
            if (lPincode != null)
            {
                ViewBag.Pincode = lPincode.Name;
            }


            ViewBag.AreaID = new SelectList(db.Areas, "ID", "Name", customerorder.AreaID);
            ViewBag.ReferenceCustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode", customerorder.ReferenceCustomerOrderID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorder.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorder.ModifyBy);
            ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", customerorder.PincodeID);
            ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", customerorder.UserLoginID);
            return View(customerorder);
        }


        // POST: /CustomerOrderDetailAlignment/EditShippingAddress/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        // [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrderDetailAlignment/CanWrite")]
        public ActionResult EditShippingAddress(long id, [Bind(Include = "ID,PrimaryMobile,SecondoryMobile,ShippingAddress,PincodeID,AreaID")] ModelLayer.Models.CustomerOrder customerorder, string Pincode, string Area, string Log)
        {
            SessionDetails();
            ModelLayer.Models.CustomerOrder lCustomerOrder = db.CustomerOrders.Find(customerorder.ID);
            if (lCustomerOrder == null)
            {
                return View("Error");
            }
            ModelLayer.Models.Pincode lPincode = db.Pincodes.FirstOrDefault(x => x.Name == Pincode);
            if (lPincode == null)
            {
                return View("Error");
            }
            int? lAreaID = null;
            if (Area != null)
            {
                lAreaID = Convert.ToInt32(Area);
            }
            else
            {
                lAreaID = lCustomerOrder.AreaID;
            }
            ModelLayer.Models.Area lArea = db.Areas.Find(Convert.ToInt64(Area));
            //if (lArea == null)
            //{
            //    return View("Error");
            //}
            customerorder.ID = lCustomerOrder.ID;
            customerorder.OrderCode = lCustomerOrder.OrderCode;
            customerorder.UserLoginID = lCustomerOrder.UserLoginID;
            customerorder.ReferenceCustomerOrderID = lCustomerOrder.ReferenceCustomerOrderID;
            customerorder.OrderAmount = lCustomerOrder.OrderAmount;
            customerorder.NoOfPointUsed = lCustomerOrder.NoOfPointUsed;
            customerorder.ValuePerPoint = lCustomerOrder.ValuePerPoint;
            customerorder.CoupenCode = lCustomerOrder.CoupenCode;
            customerorder.CoupenAmount = lCustomerOrder.CoupenAmount;
            customerorder.PAN = lCustomerOrder.PAN;
            customerorder.PaymentMode = lCustomerOrder.PaymentMode;
            customerorder.PayableAmount = lCustomerOrder.PayableAmount;
            //customerorder.PrimaryMobile = lCustomerOrder.PrimaryMobile;
            //customerorder.SecondoryMobile = lCustomerOrder.SecondoryMobile;
            //customerorder.ShippingAddress = lCustomerOrder.ShippingAddress;
            customerorder.PincodeID = lPincode.ID;
            customerorder.AreaID = lAreaID;
            customerorder.CreateDate = lCustomerOrder.CreateDate;
            customerorder.CreateBy = lCustomerOrder.CreateBy;
            customerorder.ModifyDate = DateTime.Now;
            customerorder.ModifyBy = customerCareSessionViewModel.PersonalDetailID;
            customerorder.NetworkIP = lCustomerOrder.NetworkIP;
            customerorder.DeviceType = lCustomerOrder.DeviceType;
            customerorder.DeviceID = lCustomerOrder.DeviceID;

            if (ModelState.IsValid)
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                db1.Entry(customerorder).State = EntityState.Modified;
                db1.SaveChanges();
                db1.Dispose();
                //Response.Redirect("#/CustomerOrderDetail?COID=" + id);
                //-- Start Write UserLog to CustomerOrderUserDefinedLog Table ------------------------------------------------------------//
                CustomerOrderUserDefinedLog lCustomerOrderUserDefinedLog = new CustomerOrderUserDefinedLog();
                lCustomerOrderUserDefinedLog.CustomerOrderID = customerorder.ID;
                lCustomerOrderUserDefinedLog.Description = Log;
                lCustomerOrderUserDefinedLog.IsActive = true;
                lCustomerOrderUserDefinedLog.CreateBy = customerCareSessionViewModel.PersonalDetailID;
                lCustomerOrderUserDefinedLog.CreateDate = DateTime.Now;
                lCustomerOrderUserDefinedLog.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                lCustomerOrderUserDefinedLog.DeviceType = "x";
                db.CustomerOrderUserDefinedLogs.Add(lCustomerOrderUserDefinedLog);
                db.SaveChanges();

                //-- End Write UserLog to CustomerOrderUserDefinedLog Table --------------------------------------------------------------//
                return RedirectToAction("Index", "CustomerOrderDetailAlignment", new { COID = id });
                //return View("Index");
            }
            ViewBag.AreaID = new SelectList(db.Areas, "ID", "Name", customerorder.AreaID);
            ViewBag.ReferenceCustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode", customerorder.ReferenceCustomerOrderID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorder.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorder.ModifyBy);
            ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", customerorder.PincodeID);
            ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", customerorder.UserLoginID);
            return View(customerorder);
        }

        public ActionResult GetAddress(string Pincode, long OldPincodeID)
        {
            /*This Action Responces to AJAX Call
             * After entering Pincode returens City, District and State Information
             * */
            long OldCity = db.Pincodes.Find(OldPincodeID).CityID;

            Pincode lPincode = db.Pincodes.FirstOrDefault(p => p.Name == Pincode);
            if (lPincode == null)
            {
                var errorMsg = "Pincode Dose Not Exist, Please Contact Admin!";
                return View(new { success = false, Error = errorMsg });
            }

            if (lPincode.CityID != OldCity)
            {
                var errorMsg = "Please Enter The Pincode in Current City!";
                return View(new { success = false, Error = errorMsg });
            }
            else
            {
                long CityId = lPincode.CityID;
                ViewBag.City = db.Cities.Single(c => c.ID == CityId).Name.ToString();

                long DistrictId = db.Cities.Single(c => c.ID == CityId).DistrictID;
                ViewBag.District = db.Districts.Single(d => d.ID == DistrictId).Name.ToString();

                long StateId = db.Districts.Single(d => d.ID == DistrictId).StateID;
                ViewBag.State = db.States.Single(d => d.ID == StateId).Name.ToString();

                ViewBag.Area = new SelectList(db.Areas.Where(x => x.PincodeID == lPincode.ID), "ID", "Name");

                return PartialView("_Address");
            }
        }

        #endregion

        #region ORDER LIST

        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrderDetailAlignment/CanRead")]
        public ActionResult Edit(long COID, string Log)
        {
            TempData.Remove("CustomerOrderDetailViewModel");
            TempData.Remove("ProductList");
            return PartialView();
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrderDetailAlignment/CanRead")]
        public ActionResult EditOrderList(long COID, string Log)
        {
            SessionDetails();
            //Dictionary<long, string> lDictProductList = new Dictionary<long, string>();
            NewCustomerOrderDetailListViewModel lCustomerOrderDetailViewModel = new NewCustomerOrderDetailListViewModel();
            if (TempData["CustomerOrderDetailViewModel"] != null)
            {
                lCustomerOrderDetailViewModel = (NewCustomerOrderDetailListViewModel)TempData["CustomerOrderDetailViewModel"];
            }
            else
            {
                lCustomerOrderDetailViewModel.COID = COID;
                List<CustomerOrderDetail> customerorderdetails = db.CustomerOrderDetails.Include(c => c.CustomerOrder).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.Shop).Include(c => c.ShopStock).Where(x => x.CustomerOrderID == COID && x.ReferenceShopOrderCode == null).OrderBy(x => x.ShopOrderCode).ToList();
                lCustomerOrderDetailViewModel.customerOrderDetails = customerorderdetails;
                TempData["CustomerOrderDetailViewModel"] = lCustomerOrderDetailViewModel;
            }

            if (TempData["ProductList"] != null)
            {
                List<Franchise.Models.ViewModel.ProductList> lProductList = (List<Franchise.Models.ViewModel.ProductList>)TempData["ProductList"];
            }
            else
            {
                List<Franchise.Models.ViewModel.ProductList> lProductList = (from SHPSTK in db.ShopStocks
                                                                             join COD in db.CustomerOrderDetails.Where(x => x.CustomerOrderID == COID) on SHPSTK.ID equals COD.ShopStockID
                                                                             join WS in db.WarehouseStocks on COD.WarehouseStockID equals WS.ID into ps
                                                                             from WS in ps.DefaultIfEmpty()
                                                                             select new Franchise.Models.ViewModel.ProductList
                                                                             {
                                                                                 ShopStockID = SHPSTK.ID,
                                                                                 Name = SHPSTK.ShopProduct.Product.Name,
                                                                                 Batch = WS.BatchCode
                                                                             }).ToList();
                TempData["ProductList"] = lProductList;
            }

            TempData.Keep();

            var StatusList = from Franchise.Common.Constant.ORDER_STATUS d in Enum.GetValues(typeof(Franchise.Common.Constant.ORDER_STATUS))
                             select new { ID = (int)d, Name = d.ToString() };
            //Remove other status as disscussed with Reena mam
            int[] OrderStatusList = { 1, 4, 7, 8, 9 };
            StatusList = StatusList.Where(p => OrderStatusList.Contains(p.ID));
            //
            ViewBag.OrderStatus = new SelectList(StatusList.Where(x => x.ID > (int)Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING), "ID", "Name");

            //Added by Rumana for Delivery Charge Amount set to Delivery charge dropdown
            decimal TotallDeliveryCharge = 0;
            var OrderDeliveryAmount = (from cd in db.CustomerOrderDetails
                                       join dc in db.DeliveryOrderDetails on cd.ShopOrderCode equals dc.ShopOrderCode
                                       where cd.CustomerOrderID == COID
                                       select new
                                       {
                                           ShopOrderCode = dc.ShopOrderCode,
                                           ShopDeliveryCharges = dc.GandhibaghCharge
                                       });
            if (OrderDeliveryAmount != null)
            {
                TotallDeliveryCharge = OrderDeliveryAmount.Select(x => new { x.ShopOrderCode, x.ShopDeliveryCharges }).Distinct().AsEnumerable().Sum(x => x.ShopDeliveryCharges);
            }
            else
            {
                TotallDeliveryCharge = 0;
            }
            ViewBag.DeliveryCharge = Math.Round(TotallDeliveryCharge);
            ViewBag.PaymentMode = db.CustomerOrders.Find(COID).PaymentMode.Trim().ToUpper();
            lCustomerOrderDetailViewModel.IsBusinessBoosterOrder = db.BoosterPlanSubscribers.Any(p => p.CustomerOrderId == lCustomerOrderDetailViewModel.COID);
            return PartialView(lCustomerOrderDetailViewModel);
        }



        class TempOrderDetail
        {
            public long id { get; set; }
            public int Status { get; set; }
            public int Qty { get; set; }
        }

        // POST: /CustomerOrderDetailAlignment/EditOrderList/3
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrderDetailAlignment/CanWrite")]
        public ActionResult EditOrderList(long COID, NewCustomerOrderDetailListViewModel obj, decimal DeliveryCharge, string ProductDetail, string Log, string sendSMSToCustomer, string CType)
        {
            List<CustomerOrderDetail> objForMLM = db.CustomerOrderDetails.Where(p => p.CustomerOrderID == COID).ToList();
            List<TempOrderDetail> objTempOrderDetail = new List<TempOrderDetail>();
            foreach (var item in objForMLM)
            {
                TempOrderDetail o = new TempOrderDetail();
                o.id = item.ID;
                o.Status = item.OrderStatus;
                o.Qty = item.Qty;
                objTempOrderDetail.Add(o);
            }
            try
            {
                //Check order is business booster plan order and is modified
                bool IsBusinessBoosterPlan = false;
                IsBusinessBoosterPlan = db.BoosterPlanSubscribers.Any(p => p.CustomerOrderId == COID);
                if (IsBusinessBoosterPlan)
                {
                    bool AllowTocontinue = true;
                    int FirstStatus = obj.customerOrderDetails.FirstOrDefault().OrderStatus;
                    if (obj.customerOrderDetails.Where(p => p.OrderStatus == FirstStatus).Count() != obj.customerOrderDetails.Count())
                    {
                        //all product not have same status or extra product is added
                        AllowTocontinue = false;
                    }
                    if (AllowTocontinue)
                    {
                        List<TempOrderDetail> objTempOrderDetail_ =
                            (from temp in objTempOrderDetail
                             join cod in obj.customerOrderDetails
                             on temp.id equals cod.ID
                             where temp.Qty == cod.Qty
                             select new TempOrderDetail
                             {
                                 id = temp.id
                             }).ToList();
                        if (objTempOrderDetail_.Count() != objTempOrderDetail.Count())
                        {
                            //product qty is changed or extra product is added
                            AllowTocontinue = false;
                        }
                    }
                    if (!AllowTocontinue)
                    {
                        TempData["Message"] = "In Business Booster plan order, changes are not allowed.";
                        return RedirectToAction("Index", "CustomerOrderDetailAlignment", new { COID = COID });
                    }
                }

                SessionDetails();
                /*-- Steps -----------------------------------------------------------------------------------------------
                 * 1) Insert the newly added products in customerOrderDetail and their respective changes 
                 *    by calling PlacePartialOrder() and reflection of TAX of new Products.
                 * 2) Update CustomerOrder Table.                 
                 * 3) Update taxes of edited products in taxonorder.
                 * 4) Update the edited products in customerOrderDetail.
                 * 5) Update DeliveryOrderDetail.
                 * 6) Insert New Records in CustomerOrderHistory Table.
                 * 7) Insert into UserDefined Log.
                 * Complete Transaction Lock
                 * 8) Insert into TransactionInput for Accounts.
                ----------------------------------------------------------------------------------------------------------*/
                ViewBag.PaymentMode = db.CustomerOrders.Find(COID).PaymentMode.Trim().ToUpper();
                List<CustomerOrderDetail> lCustomerOrderDetails = obj.customerOrderDetails;
                foreach (CustomerOrderDetail customerOrderDetail in lCustomerOrderDetails)
                {
                    if (customerOrderDetail.IsActive == false)
                    {
                        customerOrderDetail.OrderStatus = (int)Common.Constant.ORDER_STATUS.CANCELLED;
                    }

                    if (customerOrderDetail.OrderStatus == (int)Common.Constant.ORDER_STATUS.CANCELLED)
                    {
                        customerOrderDetail.IsActive = false;
                    }
                }

                List<CustomerOrderDetail> lNewCustomerOrderDetails = obj.customerOrderDetails.Where(x => x.ID == 0 && x.IsActive == true).ToList();
                List<CustomerOrderDetail> lOldCustomerOrderDetails = obj.customerOrderDetails.Where(x => x.ID > 0).ToList();

                List<ShopStockViewModel> lNewShopStockViewModelsForUpdate = new List<ShopStockViewModel>();

                //-- 1 ---------------------------------------------------------------------------------------------------//
                if (lNewCustomerOrderDetails.Count() > 0)
                {
                    lNewShopStockViewModelsForUpdate = (from cod in lNewCustomerOrderDetails
                                                        select new ShopStockViewModel
                                                        {
                                                            ID = cod.ShopStockID,
                                                            MRP = cod.MRP,
                                                            SaleRate = cod.SaleRate,
                                                            BusinessPointPerUnit = cod.BusinessPointPerUnit,
                                                            WarehouseStockID = cod.WarehouseStockID //Added by Zubair for Inventory on 28-03-2018
                                                        }).ToList();

                    lNewCustomerOrderDetails.ForEach(x => x.TotalAmount = x.SaleRate * x.Qty);
                    //lNewCustomerOrderDetails.ForEach(x => x.BusinessPointPerUnit = x.BusinessPointPerUnit); //Added by Zubair for MLm on 6-01-2018
                    lNewCustomerOrderDetails.ForEach(x => x.BusinessPoints = x.BusinessPointPerUnit * x.Qty); //Added by Zubair for MLm on 6-01-2018
                    lNewCustomerOrderDetails.ForEach(x => x.CreateDate = DateTime.Now);
                    lNewCustomerOrderDetails.ForEach(x => x.CreateBy = customerCareSessionViewModel.PersonalDetailID);
                    lNewCustomerOrderDetails.ForEach(x => x.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP());
                    lNewCustomerOrderDetails.ForEach(x => x.DeviceType = "x");
                    lNewCustomerOrderDetails.ForEach(x => x.DeviceID = "x");

                    BusinessLogicLayer.CustomerOrder customerOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                    Boolean lNewCustomerOrderDetailStatus = customerOrder.PlacePartialOrder(lNewCustomerOrderDetails, COID, fConnectionString, DeliveryCharge);

                    //Added by Zubair on 27-04-2018 for Inventory
                    //Update Shop Stock Qty
                    if (lNewCustomerOrderDetailStatus == true)
                    {
                        BusinessLogicLayer.CustomerOrder objC = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                        foreach (var item in lNewCustomerOrderDetails)
                        {
                            //stock less
                            objC.ManageStock(item.ShopStockID, -item.Qty, item.WarehouseStockID, item.OrderStatus, item.ID); //added by Zubair for Inventory on 03-04-2018
                        }
                    }
                }
                //parameters for sending refund mail added by rumana on 01-06-2019
              
                bool IsFullReturn = true;
                //Ended parameters for sending refund mail added by rumana on 01-06-2019
                BusinessLogicLayer.ProductDetails prod = new BusinessLogicLayer.ProductDetails(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.CustomerOrder BL_customerOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required,
                                    new System.TimeSpan(0, 15, 0)))
                {
                    try
                    {
                        //-- 2 ---------------------------------------------------------------------------------------------------//
                        //Nullable<decimal> totalNewBusinessPoints = 0;
                        foreach (CustomerOrderDetail customerOrderDetail in lNewCustomerOrderDetails)
                        {
                            customerOrderDetail.MRP = lNewShopStockViewModelsForUpdate.FirstOrDefault(x => x.ID == customerOrderDetail.ShopStockID).MRP;
                            customerOrderDetail.SaleRate = lNewShopStockViewModelsForUpdate.FirstOrDefault(x => x.ID == customerOrderDetail.ShopStockID).SaleRate;
                            customerOrderDetail.TotalAmount = customerOrderDetail.Qty * customerOrderDetail.SaleRate;
                            customerOrderDetail.BusinessPointPerUnit = lNewShopStockViewModelsForUpdate.FirstOrDefault(x => x.ID == customerOrderDetail.ShopStockID).BusinessPointPerUnit;//Added by Zubair for MLm on 6-01-2018
                            customerOrderDetail.BusinessPoints = customerOrderDetail.Qty * customerOrderDetail.BusinessPointPerUnit; //Added by Zubair for MLm on 6-01-2018
                            customerOrderDetail.IsActive = true;
                            customerOrderDetail.CashbackPointPerUnit = prod.getCasbackPointsOnProduct(customerOrderDetail.WarehouseStockID.Value);
                            //totalNewBusinessPoints += customerOrderDetail.BusinessPoints;
                        }
                        lOldCustomerOrderDetails.AddRange(lNewCustomerOrderDetails);

                        //Added by Zubair for Inventory on 16-04-2018
                        //Restrict user to enter item qty which is not available
                        foreach (var item in lOldCustomerOrderDetails)
                        {
                            var availableQty = db.ShopStocks.Where(x => x.ID == item.ShopStockID && x.WarehouseStockID == item.WarehouseStockID && x.Qty > 0).Select(x => x.Qty).FirstOrDefault();
                            ShopStockOrderDetailLog objlog = db.ShopStockOrderDetailLogs.Where(x => x.ShopStockID == item.ShopStockID && x.CustomerOrderDetailID == item.ID).FirstOrDefault();
                            int requiredQty = 0;
                            if (objlog != null && objlog.ID > 0)
                            {
                                if (objlog.Quantity < item.Qty)
                                {
                                    requiredQty = item.Qty - objlog.Quantity;
                                }

                                if (objlog.OrderStatus == 9 && objlog.OrderStatus != item.OrderStatus)
                                {
                                    requiredQty = objlog.Quantity + item.Qty;
                                }

                                if (requiredQty > 0)
                                {
                                    NewCustomerOrderDetailListViewModel lCustomerOrderDetailViewModel = new NewCustomerOrderDetailListViewModel();
                                    Franchise.Models.ViewModel.ProductList ProductName = (from SHPSTK in db.ShopStocks
                                                                                          join COD in db.CustomerOrderDetails.Where(x => x.CustomerOrderID == COID) on SHPSTK.ID equals COD.ShopStockID
                                                                                          select new Franchise.Models.ViewModel.ProductList
                                                                                          {
                                                                                              ShopStockID = SHPSTK.ID,
                                                                                              Name = SHPSTK.ShopProduct.Product.Name + " " + SHPSTK.ProductVarient.Size.Name
                                                                                          }).FirstOrDefault();

                                    if (availableQty == 0)
                                    {
                                        lCustomerOrderDetailViewModel.COID = COID;
                                        List<CustomerOrderDetail> customerorderdetails = db.CustomerOrderDetails.Include(c => c.CustomerOrder).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.Shop).Include(c => c.ShopStock).Where(x => x.CustomerOrderID == COID && x.ReferenceShopOrderCode == null).OrderBy(x => x.ShopOrderCode).ToList();
                                        lCustomerOrderDetailViewModel.customerOrderDetails = customerorderdetails;
                                        TempData["CustomerOrderDetailViewModel"] = lCustomerOrderDetailViewModel;

                                        List<Franchise.Models.ViewModel.ProductList> lProductList = (from SHPSTK in db.ShopStocks
                                                                                                     join COD in db.CustomerOrderDetails.Where(x => x.CustomerOrderID == COID) on SHPSTK.ID equals COD.ShopStockID
                                                                                                     join WS in db.WarehouseStocks on COD.WarehouseStockID equals WS.ID into ps
                                                                                                     from WS in ps.DefaultIfEmpty()
                                                                                                     select new Franchise.Models.ViewModel.ProductList
                                                                                                     {
                                                                                                         ShopStockID = SHPSTK.ID,
                                                                                                         Name = SHPSTK.ShopProduct.Product.Name,
                                                                                                         Batch = WS.BatchCode
                                                                                                     }).ToList();
                                        TempData["ProductList"] = lProductList;



                                        var StatusList = from Franchise.Common.Constant.ORDER_STATUS d in Enum.GetValues(typeof(Franchise.Common.Constant.ORDER_STATUS))
                                                         select new { ID = (int)d, Name = d.ToString() };
                                        //Remove other status as disscussed with Reena mam
                                        int[] OrderStatusList = { 1, 4, 7, 8, 9 };
                                        StatusList = StatusList.Where(p => OrderStatusList.Contains(p.ID));
                                        //
                                        ViewBag.OrderStatus = new SelectList(StatusList.Where(x => x.ID > (int)Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING), "ID", "Name");
                                        ViewBag.DeliveryCharge = "";
                                        //Added by Rumana for Delivery Charge Amount set to Delivery charge dropdown
                                        decimal TotallDeliveryCharge = 0;
                                        var OrderDeliveryAmount = (from cd in db.CustomerOrderDetails
                                                                   join dc in db.DeliveryOrderDetails on cd.ShopOrderCode equals dc.ShopOrderCode
                                                                   where cd.CustomerOrderID == COID
                                                                   select new
                                                                   {
                                                                       ShopOrderCode = dc.ShopOrderCode,
                                                                       ShopDeliveryCharges = dc.GandhibaghCharge
                                                                   });
                                        if (OrderDeliveryAmount != null)
                                        {
                                            TotallDeliveryCharge = OrderDeliveryAmount.Select(x => new { x.ShopOrderCode, x.ShopDeliveryCharges }).Distinct().AsEnumerable().Sum(x => x.ShopDeliveryCharges);
                                        }
                                        else
                                        {
                                            TotallDeliveryCharge = 0;
                                        }
                                        ViewBag.DeliveryCharge = Math.Round(TotallDeliveryCharge);

                                        ViewBag.PaymentMode = db.CustomerOrders.Find(COID).PaymentMode.Trim().ToUpper();
                                        ViewBag.ShortageQty = "Batch of " + ProductName.Name + " is out of Stock from the Shop! Quantity can't be increased.";
                                        return PartialView(lCustomerOrderDetailViewModel);
                                        //return RedirectToAction("Edit");
                                    }
                                    else if (availableQty < requiredQty)
                                    {
                                        lCustomerOrderDetailViewModel.COID = COID;
                                        List<CustomerOrderDetail> customerorderdetails = db.CustomerOrderDetails.Include(c => c.CustomerOrder).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.Shop).Include(c => c.ShopStock).Where(x => x.CustomerOrderID == COID && x.ReferenceShopOrderCode == null).OrderBy(x => x.ShopOrderCode).ToList();
                                        lCustomerOrderDetailViewModel.customerOrderDetails = customerorderdetails;
                                        TempData["CustomerOrderDetailViewModel"] = lCustomerOrderDetailViewModel;

                                        List<Franchise.Models.ViewModel.ProductList> lProductList = (from SHPSTK in db.ShopStocks
                                                                                                     join COD in db.CustomerOrderDetails.Where(x => x.CustomerOrderID == COID) on SHPSTK.ID equals COD.ShopStockID
                                                                                                     join WS in db.WarehouseStocks on COD.WarehouseStockID equals WS.ID into ps
                                                                                                     from WS in ps.DefaultIfEmpty()
                                                                                                     select new Franchise.Models.ViewModel.ProductList
                                                                                                     {
                                                                                                         ShopStockID = SHPSTK.ID,
                                                                                                         Name = SHPSTK.ShopProduct.Product.Name,
                                                                                                         Batch = WS.BatchCode
                                                                                                     }).ToList();
                                        TempData["ProductList"] = lProductList;
                                        var StatusList = from Franchise.Common.Constant.ORDER_STATUS d in Enum.GetValues(typeof(Franchise.Common.Constant.ORDER_STATUS))
                                                         select new { ID = (int)d, Name = d.ToString() };
                                        //Remove other status as disscussed with Reena mam
                                        int[] OrderStatusList = { 1, 4, 7, 8, 9 };
                                        StatusList = StatusList.Where(p => OrderStatusList.Contains(p.ID));
                                        //
                                        ViewBag.OrderStatus = new SelectList(StatusList.Where(x => x.ID > (int)Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING), "ID", "Name");
                                        ViewBag.DeliveryCharge = "";
                                        //Added by Rumana for Delivery Charge Amount set to Delivery charge dropdown
                                        decimal TotallDeliveryCharge = 0;
                                        var OrderDeliveryAmount = (from cd in db.CustomerOrderDetails
                                                                   join dc in db.DeliveryOrderDetails on cd.ShopOrderCode equals dc.ShopOrderCode
                                                                   where cd.CustomerOrderID == COID
                                                                   select new
                                                                   {
                                                                       ShopOrderCode = dc.ShopOrderCode,
                                                                       ShopDeliveryCharges = dc.GandhibaghCharge
                                                                   });
                                        if (OrderDeliveryAmount != null)
                                        {
                                            TotallDeliveryCharge = OrderDeliveryAmount.Select(x => new { x.ShopOrderCode, x.ShopDeliveryCharges }).Distinct().AsEnumerable().Sum(x => x.ShopDeliveryCharges);
                                        }
                                        else
                                        {
                                            TotallDeliveryCharge = 0;
                                        }
                                        ViewBag.DeliveryCharge = Math.Round(TotallDeliveryCharge);

                                        ViewBag.PaymentMode = db.CustomerOrders.Find(COID).PaymentMode.Trim().ToUpper();
                                        ViewBag.ShortageQty = "Out of Stock! Only " + availableQty + " more quantity is available for " + ProductName.Name;
                                        return PartialView(lCustomerOrderDetailViewModel);
                                    }
                                    else
                                    {
                                        ViewBag.ShortageQty = null;
                                    }
                                }
                            }

                            item.CashbackPoints = item.CashbackPointPerUnit * item.Qty;
                        }
                        //End
                        long lCustomerOrderID = 0;
                        decimal lPayableAmount = 0;
                        //Boolean lCustomerOrderDetailStatus;
                        //decimal _Cust_PayableAmt;
                        //decimal _Cust_MLMAmtUsed;
                        //Added by Rumana

                        //decimal CancelledproductTotal_Amt = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == COID && x.OrderStatus == 9).Sum(x => x.SaleRate * x.Qty);
                        //decimal MLMRefundAmt = CancelledproductTotal_Amt - PayableAmount;
                        //decimal RefundOrignalAmount = CancelledproductTotal_Amt - MLMRefundAmt;
                        //var lOrder = db.CustomerOrders.Where(x => x.ID == COID).FirstOrDefault();
                        //var TempCancelCount = lCustomerOrderDetails.Where(x => x.OrderStatus == 9).ToList();
                        //var DBCancelCount = db.CustomerOrderDetails.Where(p => p.CustomerOrderID == COID && p.OrderStatus == 9).ToList();
                        //var TempReturnCount = lCustomerOrderDetails.Where(x => x.OrderStatus == 8).ToList();
                        //var DBReturnCount = db.CustomerOrderDetails.Where(p => p.CustomerOrderID == COID && p.OrderStatus == 8).ToList();


                        //if (lCustomerOrderDetails.Any(x => x.OrderStatus == 9)) // OrderStatus = 9 =>Cancel and OrderStatus = 8 =>Return
                        //{

                        //    if (TempCancelCount.Count() != DBCancelCount.Count() || DBCancelCount.Count() == 0)
                        //    {
                        //        //Added by Rumana for Delivery Charge Amount
                        //        decimal TotallDeliveryCharge = 0;
                        //        var OrderDeliveryAmount = (from cd in db.CustomerOrderDetails
                        //                                   join dc in db.DeliveryOrderDetails on cd.ShopOrderCode equals dc.ShopOrderCode
                        //                                   where cd.CustomerOrderID == COID
                        //                                   select new
                        //                                   {
                        //                                       ShopOrderCode = dc.ShopOrderCode,
                        //                                       ShopDeliveryCharges = dc.GandhibaghCharge
                        //                                   });
                        //        if (OrderDeliveryAmount != null)
                        //        {
                        //            TotallDeliveryCharge = OrderDeliveryAmount.Select(x => new { x.ShopOrderCode, x.ShopDeliveryCharges }).Distinct().AsEnumerable().Sum(x => x.ShopDeliveryCharges);
                        //        }
                        //        else
                        //        {
                        //            TotallDeliveryCharge = 0;
                        //        }
                        //        //Added by Rumana for Delivery Charge Amount

                        //        ModelLayer.Models.CustomerOrder _CustomerOrder = db.CustomerOrders.Find(COID);

                        //        decimal OrderAmount = lCustomerOrderDetails.Where(x => x.IsActive == true).Sum(x => x.SaleRate * x.Qty);
                        //        decimal MLMAmountUsed = _CustomerOrder.MLMAmountUsed != null ? _CustomerOrder.MLMAmountUsed.Value : 0;
                        //        decimal PayableAmount = db.CustomerOrders.Where(x => x.ID == COID).FirstOrDefault().PayableAmount;
                        //        PayableAmount = PayableAmount - TotallDeliveryCharge;
                        //        PayableAmount = PayableAmount + DeliveryCharge;


                        //        decimal CancelledproductTotal_Amt = lCustomerOrderDetails.Where(x => x.IsActive == false).Sum(x => x.SaleRate * x.Qty);
                        //        decimal _Cus_toTotalBusinessPoints = Convert.ToDecimal(lCustomerOrderDetails.Where(x => x.IsActive == true).Sum(x => x.BusinessPointPerUnit * x.Qty));
                        //        //bool IsSendMail = false;

                        //        if (_CustomerOrder.MLMAmountUsed > 0)
                        //        {

                        //            if (CancelledproductTotal_Amt >= PayableAmount) //refund Ewallet
                        //            {
                        //                if (OrderAmount > 0)//partial cancelled with MLM amount used, no need to refund delivery charge
                        //                {
                        //                    MLMRefundAmt = CancelledproductTotal_Amt - PayableAmount;
                        //                    RefundOrignalAmount = CancelledproductTotal_Amt - MLMRefundAmt;
                        //                }
                        //                else//full cancelled with MLM amount used,need to refund Delivery charge
                        //                {
                        //                    MLMRefundAmt = MLMAmountUsed;
                        //                    RefundOrignalAmount = CancelledproductTotal_Amt;

                        //                }


                        //                EWalletRefund_Table obj_ = db.eWalletRefund_Table.FirstOrDefault(p => p.UserLoginId == lOrder.ID && p.CustomerOrderId == lOrder.ID && p.Status == 0);
                        //                decimal DbMLMRefundAmt = 0;
                        //                if (obj_ == null)
                        //                {
                        //                    if (lOrder != null && lOrder.MLMAmountUsed != 0)
                        //                    {
                        //                        EWalletRefund_Table _obj = new EWalletRefund_Table();
                        //                        _obj.RequsetAmt = MLMRefundAmt;
                        //                        _obj.Date = BusinessLogicLayer.CommonFunctions.GetLocalTime();
                        //                        _obj.CustomerOrderId = lOrder.ID;
                        //                        _obj.UserLoginId = lOrder.UserLoginID;
                        //                        _obj.Status = 0;
                        //                        _obj.Isactive = false;
                        //                        _obj.Createdby = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(lOrder.UserLoginID);
                        //                        _obj.CreatedDate = BusinessLogicLayer.CommonFunctions.GetLocalTime();
                        //                        _obj.NetworkID = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        //                        _obj.DeviceID = string.Empty;
                        //                        _obj.DeviceType = string.Empty;
                        //                        db.eWalletRefund_Table.Add(_obj);
                        //                        db.SaveChanges();
                        //                        result_SendMail = BL_customerOrder.Send_EWalletRefund_Mail_FromPartner(COID, MLMRefundAmt, RefundOrignalAmount, IsSendMail);
                        //                    }
                        //                }
                        //                else
                        //                {
                        //                    DbMLMRefundAmt = obj_.RequsetAmt;
                        //                    if (obj_.RequsetAmt != MLMRefundAmt)
                        //                    {
                        //                        obj_.RequsetAmt = MLMRefundAmt;
                        //                        obj_.ModifiedDate = DateTime.UtcNow;
                        //                        obj_.NetworkID = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        //                        obj_.DeviceID = "X";
                        //                        obj_.DeviceType = "X";
                        //                        db.SaveChanges();
                        //                        result_SendMail = BL_customerOrder.Send_EWalletRefund_Mail_FromPartner(COID, MLMRefundAmt, RefundOrignalAmount, IsSendMail);
                        //                    }
                        //                }

                        //                if (obj_ == null && DbMLMRefundAmt != MLMRefundAmt)
                        //                {
                        //                    //result_SendMail = BL_customerOrder.Send_EWalletRefund_Mail_FromPartner(COID, MLMRefundAmt, RefundOrignalAmount, IsSendMail);
                        //                    //Update customer table0
                        //                    _Cust_PayableAmt = PayableAmount - RefundOrignalAmount;
                        //                    if (_Cust_PayableAmt < 0)
                        //                    {
                        //                        _Cust_PayableAmt = 0;
                        //                    }
                        //                    _Cust_MLMAmtUsed = MLMAmountUsed - MLMRefundAmt;
                        //                    lCustomerOrderID = _UpdateCustomerOrder(COID, OrderAmount, _Cust_PayableAmt, _Cust_MLMAmtUsed, _Cus_toTotalBusinessPoints);
                        //                    lCustomerOrderDetailStatus = UpdateCustomerOrderDetail(COID, lOldCustomerOrderDetails);
                        //                }
                        //            }
                        //            else //Refund Orignal Cancelled Amt
                        //            {
                        //                MLMRefundAmt = 0;
                        //                _Cust_PayableAmt = PayableAmount - CancelledproductTotal_Amt;
                        //                lCustomerOrderID = _UpdateCustomerOrder(COID, OrderAmount, _Cust_PayableAmt, MLMAmountUsed, _Cus_toTotalBusinessPoints);
                        //                lCustomerOrderDetailStatus = UpdateCustomerOrderDetail(COID, lOldCustomerOrderDetails);
                        //                //result_SendMail = BL_customerOrder.Send_EWalletRefund_Mail_FromPartner(COID, MLMRefundAmt, CancelledproductTotal_Amt, IsSendMail);
                        //            }

                        //        }

                        //        else
                        //        {
                        //            if (OrderAmount > 0)
                        //            {
                        //                _Cust_PayableAmt = PayableAmount - CancelledproductTotal_Amt;

                        //            }
                        //            else
                        //            {
                        //                CancelledproductTotal_Amt = PayableAmount;
                        //                _Cust_PayableAmt = PayableAmount - CancelledproductTotal_Amt;
                        //            }
                        //            MLMRefundAmt = 0;
                        //            lCustomerOrderID = _UpdateCustomerOrder(COID, OrderAmount, _Cust_PayableAmt, 0, _Cus_toTotalBusinessPoints);
                        //            lCustomerOrderDetailStatus = UpdateCustomerOrderDetail(COID, lOldCustomerOrderDetails);
                        //            //result_SendMail = BL_customerOrder.Send_EWalletRefund_Mail_FromPartner(COID, MLMRefundAmt, CancelledproductTotal_Amt, IsSendMail);

                        //        }
                        //        if (result_SendMail == true)
                        //        {

                        //        }
                        //        else
                        //        {
                        //            ViewBag.result_SendMailMSG = "Something went wrong";
                        //        }
                        //    }
                        //    else
                        //    {
                        //        lCustomerOrderID = COID;
                        //        lCustomerOrderDetailStatus = UpdateCustomerOrderDetail(COID, lOldCustomerOrderDetails);
                        //    }
                        //}
                        ////Ended by Rumana
                        //else if (lCustomerOrderDetails.Any(x => x.OrderStatus == 8)) // OrderStatus = 9 =>Cancel and OrderStatus = 8 =>Return
                        //{
                        //    if (TempReturnCount.Count() != DBReturnCount.Count() || DBReturnCount.Count() == 0)
                        //    {

                        //        if (lCustomerOrderDetails.Any(x => x.OrderStatus == 0 || x.OrderStatus == 1 || x.OrderStatus == 2 || x.OrderStatus == 3 || x.OrderStatus == 4 || x.OrderStatus == 5 || x.OrderStatus == 6 || x.OrderStatus == 7))
                        //        {
                        //            IsFullReturn = false;
                        //        }

                        //        ModelLayer.Models.CustomerOrder _CustomerOrder = db.CustomerOrders.Find(COID);
                        //        decimal lTotalTax = CaluclateTaxTotal(lOldCustomerOrderDetails.Where(x => x.IsActive == true).ToList());
                        //        decimal OrderAmount = lCustomerOrderDetails.Where(x => x.IsActive == true).Sum(x => x.SaleRate * x.Qty);
                        //        decimal MLMAmountUsed = _CustomerOrder.MLMAmountUsed != null ? _CustomerOrder.MLMAmountUsed.Value : 0;
                        //        decimal PayableAmount = OrderAmount + DeliveryCharge + lTotalTax;
                        //        PayableAmount = PayableAmount - MLMAmountUsed;
                        //        decimal CancelledproductTotal_Amt = lCustomerOrderDetails.Where(x => x.OrderStatus == 8).Sum(x => x.SaleRate * x.Qty);
                        //        decimal _Cus_toTotalBusinessPoints = Convert.ToDecimal(lCustomerOrderDetails.Where(x => x.OrderStatus == 7).Sum(x => x.BusinessPointPerUnit * x.Qty));
                        //        //bool IsSendMail = false;
                        //        if (_CustomerOrder.MLMAmountUsed > 0)
                        //        {

                        //            if (CancelledproductTotal_Amt >= PayableAmount) //refund Ewallet
                        //            {

                        //                MLMRefundAmt = CancelledproductTotal_Amt - PayableAmount;
                        //                RefundOrignalAmount = CancelledproductTotal_Amt - MLMRefundAmt;

                        //                EWalletRefund_Table obj_ = db.eWalletRefund_Table.FirstOrDefault(p => p.UserLoginId == lOrder.ID && p.CustomerOrderId == lOrder.ID && p.Status == 0);
                        //                decimal DbMLMRefundAmt = 0;
                        //                if (obj_ == null)
                        //                {
                        //                    if (lOrder != null && lOrder.MLMAmountUsed != 0)
                        //                    {
                        //                        EWalletRefund_Table _obj = new EWalletRefund_Table();
                        //                        _obj.RequsetAmt = MLMRefundAmt;
                        //                        _obj.Date = BusinessLogicLayer.CommonFunctions.GetLocalTime();
                        //                        _obj.CustomerOrderId = lOrder.ID;
                        //                        _obj.UserLoginId = lOrder.UserLoginID;
                        //                        _obj.Status = 0;
                        //                        _obj.Isactive = false;
                        //                        _obj.Createdby = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(lOrder.UserLoginID);
                        //                        _obj.CreatedDate = BusinessLogicLayer.CommonFunctions.GetLocalTime();
                        //                        _obj.NetworkID = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        //                        _obj.DeviceID = string.Empty;
                        //                        _obj.DeviceType = string.Empty;
                        //                        db.eWalletRefund_Table.Add(_obj);
                        //                        db.SaveChanges();
                        //                        result_SendMail = BL_customerOrder.Send_EWalletRefund_Mail_FromPartner(COID, MLMRefundAmt, RefundOrignalAmount, IsSendMail);
                        //                    }
                        //                }
                        //                else
                        //                {
                        //                    DbMLMRefundAmt = obj_.RequsetAmt;
                        //                    if (obj_.RequsetAmt != MLMRefundAmt)
                        //                    {
                        //                        obj_.RequsetAmt = MLMRefundAmt;
                        //                        obj_.ModifiedDate = DateTime.UtcNow;
                        //                        obj_.NetworkID = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        //                        obj_.DeviceID = "X";
                        //                        obj_.DeviceType = "X";
                        //                        db.SaveChanges();
                        //                        result_SendMail = BL_customerOrder.Send_EWalletRefund_Mail_FromPartner(COID, MLMRefundAmt, RefundOrignalAmount, IsSendMail);
                        //                    }
                        //                }

                        //                if (obj_ == null && DbMLMRefundAmt != MLMRefundAmt)
                        //                {
                        //                    //result_SendMail = BL_customerOrder.Send_EWalletRefund_Mail_FromPartner(COID, MLMRefundAmt, RefundOrignalAmount, IsSendMail);
                        //                    //Update customer table
                        //                    _Cust_PayableAmt = PayableAmount - RefundOrignalAmount;
                        //                    if (_Cust_PayableAmt < 0)
                        //                    {
                        //                        _Cust_PayableAmt = 0;
                        //                    }
                        //                    _Cust_MLMAmtUsed = MLMAmountUsed - MLMRefundAmt;
                        //                    lCustomerOrderID = _UpdateCustomerOrder(COID, OrderAmount, _Cust_PayableAmt, _Cust_MLMAmtUsed, _Cus_toTotalBusinessPoints);
                        //                    lCustomerOrderDetailStatus = UpdateCustomerOrderDetail(COID, lOldCustomerOrderDetails);
                        //                }

                        //            }
                        //            else //Refund Orignal Cancelled Amt
                        //            {
                        //                //if (OrderAmount > 0)
                        //                //{
                        //                MLMRefundAmt = 0;
                        //                RefundOrignalAmount = CancelledproductTotal_Amt;
                        //                //}
                        //                //else
                        //                //{
                        //                //    MLMRefundAmt = 0;
                        //                //    RefundOrignalAmount = CancelledproductTotal_Amt;

                        //                //}
                        //                _Cust_PayableAmt = PayableAmount - CancelledproductTotal_Amt;
                        //                lCustomerOrderID = _UpdateCustomerOrder(COID, OrderAmount, _Cust_PayableAmt, MLMAmountUsed, _Cus_toTotalBusinessPoints);
                        //                lCustomerOrderDetailStatus = UpdateCustomerOrderDetail(COID, lOldCustomerOrderDetails);
                        //                //result_SendMail = BL_customerOrder.Send_EWalletRefund_Mail_FromPartner(COID, MLMRefundAmt, CancelledproductTotal_Amt, IsSendMail);
                        //            }

                        //        }

                        //        else //Refund Orignal Cancelled Amt
                        //        {
                        //            MLMRefundAmt = 0;
                        //            if (OrderAmount > 0)
                        //            {
                        //                _Cust_PayableAmt = PayableAmount - CancelledproductTotal_Amt;

                        //            }
                        //            else
                        //            {
                        //                //CancelledproductTotal_Amt = PayableAmount;
                        //                _Cust_PayableAmt = PayableAmount - CancelledproductTotal_Amt;
                        //            }
                        //            lCustomerOrderID = _UpdateCustomerOrder(COID, OrderAmount, _Cust_PayableAmt, 0, _Cus_toTotalBusinessPoints);
                        //            lCustomerOrderDetailStatus = UpdateCustomerOrderDetail(COID, lOldCustomerOrderDetails);
                        //            //result_SendMail = BL_customerOrder.Send_EWalletRefund_Mail_FromPartner(COID, MLMRefundAmt, CancelledproductTotal_Amt, IsSendMail);

                        //        }
                        //        if (result_SendMail == true)
                        //        {

                        //        }
                        //        else
                        //        {
                        //            ViewBag.result_SendMailMSG = "Something went wrong";
                        //        }
                        //    }
                        //    else
                        //    {
                        //        lCustomerOrderID = COID;
                        //        lCustomerOrderDetailStatus = UpdateCustomerOrderDetail(COID, lOldCustomerOrderDetails);
                        //    }
                        //}
                        //else
                        //{
                        decimal WalletRefundAmount = 0;
                        ModelLayer.Models.CustomerOrder lCustomerOrder_ = db.CustomerOrders.Find(COID);
                      decimal ReturnproductToal_CashbackPoint= lCustomerOrderDetails.Where(x => x.OrderStatus == 8).Sum(x => x.CashbackPointPerUnit * x.Qty);
                        decimal ReturenedproductTotal_Amt = lCustomerOrderDetails.Where(x => x.OrderStatus == 8).Sum(x => x.SaleRate * x.Qty);
                        decimal ReturenedproductTotal_RP = Convert.ToDecimal(lCustomerOrderDetails.Where(x => x.OrderStatus == 8).Sum(x => x.BusinessPointPerUnit * x.Qty));
                        decimal lTotalTax = CaluclateTaxTotal(lOldCustomerOrderDetails.Where(x => x.IsActive == true).ToList());
                        decimal lOrderAmount = lCustomerOrderDetails.Where(x => x.IsActive == true).Sum(x => x.SaleRate * x.Qty);
                        decimal OrdAmt = lOrderAmount;
                        OrdAmt = OrdAmt - ReturenedproductTotal_Amt;
                        if (OrdAmt < 0)
                        {
                            OrdAmt = 0;
                        }
                        OrdAmt = Math.Round(OrdAmt);
                        lOrderAmount = Math.Round(lOrderAmount);
                        if (lOrderAmount == 0)
                        {
                            //If Order is completly cancelled then no need to add delivery changes in payable amount
                            lPayableAmount = OrdAmt + lTotalTax;
                        }
                        else
                        {
                            lPayableAmount = OrdAmt + DeliveryCharge + lTotalTax;
                        }
                        lPayableAmount = Math.Ceiling(lPayableAmount);
                        decimal lTotalBusinessPoints = Convert.ToDecimal(lCustomerOrderDetails.Where(x => x.IsActive == true).Sum(x => x.BusinessPointPerUnit * x.Qty));
                        lTotalBusinessPoints = lTotalBusinessPoints - ReturenedproductTotal_RP;
                        if (lTotalBusinessPoints < 0)
                        {
                            lTotalBusinessPoints = 0;
                        }
                        lTotalBusinessPoints = Math.Round(lTotalBusinessPoints);

                        decimal CashbackPoints = lCustomerOrderDetails.Where(x => x.IsActive == true).Sum(x => x.CashbackPointPerUnit * x.Qty);
                        CashbackPoints = CashbackPoints - ReturnproductToal_CashbackPoint;
                        if (CashbackPoints < 0)
                        {
                            CashbackPoints = 0;
                        }
                        CashbackPoints = Math.Round(CashbackPoints);

                        lCustomerOrderID = UpdateCustomerOrder(COID, lOrderAmount, lPayableAmount, lTotalBusinessPoints, CashbackPoints, out WalletRefundAmount);//, lTotalOrderAmount); //Yashaswi 18-9-2018

                        //-- 4 ---------------------------------------------------------------------------------------------------// 
                        Boolean lCustomerOrderDetailStatus = UpdateCustomerOrderDetail(COID, lOldCustomerOrderDetails);
                        
                        //}
                        //Added by Zubair for MLm on 07-03-2018
                        int status = 0;
                        bool IsReturn = false;
                        bool IsCancel = false;
                        decimal ReturnAmount = 0;
                        decimal ReturnBusinessPoints = 0;

                        foreach (CustomerOrderDetail customerOrderDetail in lOldCustomerOrderDetails)
                        {
                            if (customerOrderDetail.OrderStatus == (int)Common.Constant.ORDER_STATUS.DELIVERED)
                            {
                                status = 7;
                            }
                            else if (customerOrderDetail.OrderStatus == (int)Common.Constant.ORDER_STATUS.RETURNED)
                            {
                                //if (TempReturnCount.Count() != DBReturnCount.Count() || DBReturnCount.Count() == 0)
                                //{
                                if (objTempOrderDetail.Any(p => p.id == customerOrderDetail.ID && p.Status == 7))
                                {
                                    ReturnAmount += (customerOrderDetail.SaleRate * customerOrderDetail.Qty);
                                    ReturnBusinessPoints += Convert.ToDecimal(customerOrderDetail.BusinessPointPerUnit * customerOrderDetail.Qty);
                                    IsReturn = true;
                                }
                                //}
                            }
                            //else if (customerOrderDetail.OrderStatus == (int)Common.Constant.ORDER_STATUS.CANCELLED)
                            //{
                            //    //if (TempCancelCount.Count() != DBCancelCount.Count() || DBCancelCount.Count() == 0)
                            //    //{
                            //        ReturnAmount += (customerOrderDetail.SaleRate * customerOrderDetail.Qty);
                            //        ReturnBusinessPoints += Convert.ToDecimal(customerOrderDetail.BusinessPointPerUnit * customerOrderDetail.Qty);
                            //        IsCancel = true;
                            //    //}
                            //}
                        }


                        ModelLayer.Models.CustomerOrder lCustomerOrder = db.CustomerOrders.Find(lCustomerOrderID);
                        //if(lCustomerOrder.BusinessPointsTotal>0 || lCustomerOrder.MLMAmountUsed>0)
                        //{
                        if (IsReturn == true)
                        {
                            //Yashaswi 31-8-2018
                            //For Partial Return
                            ReturnAmount = Math.Round(ReturnAmount);
                            //lCustomerOrder.PayableAmount = lCustomerOrder.PayableAmount - ReturnAmount;

                            //lCustomerOrder.PayableAmount = ReturnAmount;
                            //Yashaswi 31-8-2018
                            //For Partial Return
                            //lCustomerOrder.BusinessPointsTotal = lCustomerOrder.BusinessPointsTotal - ReturnBusinessPoints;

                            //if (lCustomerOrder.PayableAmount < Convert.ToDecimal(lCustomerOrder.MLMAmountUsed))
                            //{
                            //    lCustomerOrder.MLMAmountUsed = lCustomerOrder.MLMAmountUsed - lCustomerOrder.PayableAmount;
                            //}
                            status = 8;
                        }
                        //if (IsCancel == true)
                        //{
                        //    ReturnAmount = Math.Round(ReturnAmount);
                        //    status = 9;
                        //}
                        BusinessLogicLayer.MLMWalletPoints objMLMWalletPoints = new BusinessLogicLayer.MLMWalletPoints();
                        if (lCustomerOrder.OrderAmount == 0)
                        {
                            status = 9;
                        }

                        //Check whether it is MLMuser or not
                        MLMUser objMLMUser = db.MLMUsers.Where(x => x.UserID == lCustomerOrder.UserLoginID).FirstOrDefault();
                        if (objMLMUser != null && objMLMUser.UserID > 0)
                        {
                            if (status == 7)
                            {
                                object ret = objMLMWalletPoints.MLMWalletPostRequest(IsFullReturn, status, lCustomerOrder.UserLoginID, lCustomerOrderID, lCustomerOrder.BusinessPointsTotal, lCustomerOrder.PayableAmount, System.DateTime.Now, Convert.ToDecimal(lCustomerOrder.MLMAmountUsed), Convert.ToInt64(Session["ID"]));
                            }
                            //else if (status == 8 || lCustomerOrder.MLMAmountUsed > 0)
                            else if (status == 8)
                            {
                                object ret = objMLMWalletPoints.MLMWalletPostRequest(IsFullReturn, status, lCustomerOrder.UserLoginID, lCustomerOrderID, ReturnBusinessPoints, ReturnAmount, System.DateTime.Now, Convert.ToDecimal(lCustomerOrder.MLMAmountUsed), Convert.ToInt64(Session["ID"]));
                            }
                            //Yashaswi 31-8-2018 Call When Order Cancel
                            else if (status == 9)
                            {
                                object ret = objMLMWalletPoints.MLMWalletPostRequest(IsFullReturn, status, lCustomerOrder.UserLoginID, lCustomerOrderID, ReturnBusinessPoints, ReturnAmount, System.DateTime.Now, Convert.ToDecimal(lCustomerOrder.MLMAmountUsed), Convert.ToInt64(Session["ID"]));
                                //object ret = objMLMWalletPoints.MLMWalletPostRequest(status, lCustomerOrder.UserLoginID, lCustomerOrderID, lCustomerOrder.BusinessPointsTotal, lCustomerOrder.PayableAmount, System.DateTime.Now, Convert.ToDecimal(lCustomerOrder.MLMAmountUsed), Convert.ToInt64(Session["ID"]));
                            }
                        }

                        if (status == 7)
                        {
                            (new BusinessLogicLayer.SendFCMNotification()).SendNotification("delivered", lCustomerOrderID);
                        }
                        else if (status == 8)
                        {
                            if ((objTempOrderDetail.Count()) == (lOldCustomerOrderDetails.Where(p => p.OrderStatus == 8)).Count())
                            {
                                (new BusinessLogicLayer.SendFCMNotification()).SendNotification("returned", lCustomerOrderID);
                                BoosterPlanSubscriber planSubscriber = db.BoosterPlanSubscribers.FirstOrDefault(p => p.CustomerOrderId == COID);
                                if (planSubscriber != null)
                                {
                                    planSubscriber.IsActive = false;
                                    planSubscriber.ModifyDate = DateTime.Now;
                                    planSubscriber.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                    db.SaveChanges();
                                }
                                if (WalletRefundAmount != 0)
                                {
                                    //Refund for complete return
                                    (new BusinessLogicLayer.MLMWalletPoints()).RefundUsedWalletAmount(WalletRefundAmount, COID, 1, lCustomerOrder.UserLoginID, 8);
                                    WalletRefundAmount = 0;
                                }
                            }
                            else
                            {
                                if (WalletRefundAmount != 0)
                                {
                                    //Refund for complete return
                                    (new BusinessLogicLayer.MLMWalletPoints()).RefundUsedWalletAmount(WalletRefundAmount, COID, 2, lCustomerOrder.UserLoginID, 8);
                                    WalletRefundAmount = 0;
                                }
                            }
                        }
                        else if (status == 9)
                        {
                            if ((objTempOrderDetail.Count()) == (lOldCustomerOrderDetails.Where(p => p.OrderStatus == 9).Count()))
                            {
                                (new BusinessLogicLayer.SendFCMNotification()).SendNotification("cancelled", lCustomerOrderID);
                                BoosterPlanSubscriber planSubscriber = db.BoosterPlanSubscribers.FirstOrDefault(p => p.CustomerOrderId == COID);
                                if (planSubscriber != null)
                                {
                                    planSubscriber.IsActive = false;
                                    planSubscriber.ModifyDate = DateTime.Now;
                                    planSubscriber.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                    db.SaveChanges();
                                }
                                if (WalletRefundAmount != 0)
                                {
                                    //Refund for complete Cancel
                                    (new BusinessLogicLayer.MLMWalletPoints()).RefundUsedWalletAmount(WalletRefundAmount, COID, 1, lCustomerOrder.UserLoginID, 9);
                                    WalletRefundAmount = 0;
                                }
                            }
                        }
                        //End
                        if (WalletRefundAmount != 0)
                        {
                            //Refund for partial Cancel
                            (new BusinessLogicLayer.MLMWalletPoints()).RefundUsedWalletAmount(WalletRefundAmount, COID, 2, lCustomerOrder.UserLoginID, 9);
                        }
                        //Added by Zubair for Inventory on 13-01-2018
                        //Boolean lUpdateWarehouseStock = UpdateWarehouseStock(COID, lOldCustomerOrderDetails);
                        int aQty = 0;
                        long ErrorOnShopStockID = 0;
                        Boolean lUpdateWarehouseStock = UpdateWarehouseStock(COID, lOldCustomerOrderDetails, out aQty, out ErrorOnShopStockID);

                        if (lUpdateWarehouseStock == false && ErrorOnShopStockID > 0) //Apply validation whether item available to deliver or not
                        {
                            Franchise.Models.ViewModel.ProductList ProductName = (from SHPSTK in db.ShopStocks
                                                                                  join COD in db.CustomerOrderDetails.Where(x => x.CustomerOrderID == COID) on SHPSTK.ID equals COD.ShopStockID
                                                                                  where SHPSTK.ID == ErrorOnShopStockID
                                                                                  select new Franchise.Models.ViewModel.ProductList
                                                                                  {
                                                                                      ShopStockID = SHPSTK.ID,
                                                                                      Name = SHPSTK.ShopProduct.Product.Name + " " + SHPSTK.ProductVarient.Size.Name
                                                                                  }).FirstOrDefault();

                            NewCustomerOrderDetailListViewModel lCustomerOrderDetailViewModel = new NewCustomerOrderDetailListViewModel();
                            lCustomerOrderDetailViewModel.COID = COID;
                            List<CustomerOrderDetail> customerorderdetails = db.CustomerOrderDetails.Include(c => c.CustomerOrder).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.Shop).Include(c => c.ShopStock).Where(x => x.CustomerOrderID == COID && x.ReferenceShopOrderCode == null).OrderBy(x => x.ShopOrderCode).ToList();
                            lCustomerOrderDetailViewModel.customerOrderDetails = customerorderdetails;
                            TempData["CustomerOrderDetailViewModel"] = lCustomerOrderDetailViewModel;

                            List<Franchise.Models.ViewModel.ProductList> lProductList = (from SHPSTK in db.ShopStocks
                                                                                         join COD in db.CustomerOrderDetails.Where(x => x.CustomerOrderID == COID) on SHPSTK.ID equals COD.ShopStockID
                                                                                         join WS in db.WarehouseStocks on COD.WarehouseStockID equals WS.ID into ps
                                                                                         from WS in ps.DefaultIfEmpty()
                                                                                         select new Franchise.Models.ViewModel.ProductList
                                                                                         {
                                                                                             ShopStockID = SHPSTK.ID,
                                                                                             Name = SHPSTK.ShopProduct.Product.Name,
                                                                                             Batch = WS.BatchCode
                                                                                         }).ToList();
                            TempData["ProductList"] = lProductList;
                            var StatusList = from Franchise.Common.Constant.ORDER_STATUS d in Enum.GetValues(typeof(Franchise.Common.Constant.ORDER_STATUS))
                                             select new { ID = (int)d, Name = d.ToString() };
                            //Remove other status as disscussed with Reena mam
                            int[] OrderStatusList = { 1, 4, 7, 8, 9 };
                            StatusList = StatusList.Where(p => OrderStatusList.Contains(p.ID));
                            //
                            ViewBag.OrderStatus = new SelectList(StatusList.Where(x => x.ID > (int)Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING), "ID", "Name");
                            ViewBag.DeliveryCharge = "";
                            //Added by Rumana for Delivery Charge Amount set to Delivery charge dropdown
                            decimal TotallDeliveryCharge = 0;
                            var OrderDeliveryAmount = (from cd in db.CustomerOrderDetails
                                                       join dc in db.DeliveryOrderDetails on cd.ShopOrderCode equals dc.ShopOrderCode
                                                       where cd.CustomerOrderID == COID
                                                       select new
                                                       {
                                                           ShopOrderCode = dc.ShopOrderCode,
                                                           ShopDeliveryCharges = dc.GandhibaghCharge
                                                       });
                            if (OrderDeliveryAmount != null)
                            {
                                TotallDeliveryCharge = OrderDeliveryAmount.Select(x => new { x.ShopOrderCode, x.ShopDeliveryCharges }).Distinct().AsEnumerable().Sum(x => x.ShopDeliveryCharges);
                            }
                            else
                            {
                                TotallDeliveryCharge = 0;
                            }
                            ViewBag.DeliveryCharge = Math.Round(TotallDeliveryCharge);

                            ViewBag.PaymentMode = db.CustomerOrders.Find(COID).PaymentMode.Trim().ToUpper();
                            ViewBag.ShortageQty = "This batch out of Stock! " + aQty + " quantity is available for " + ProductName.Name;
                            return PartialView(lCustomerOrderDetailViewModel);
                        }
                        //End
                        // -- 3 --------------------------------------------------------------------------------------------------//
                        List<TaxOnOrder> lTaxOnOrder = db.TaxOnOrders.Where(x => x.CustomerOrderDetail.CustomerOrderID == COID).ToList();
                        Boolean lTaxOnOrderStatus = UpdateTaxOnOrderDetail(lTaxOnOrder, lOldCustomerOrderDetails.Where(x => x.IsActive == true).ToList());

                        //-- 5 ---------------------------------------------------------------------------------------------------// 
                        List<CustomerOrderDetail> lCustomerOrderDetailsFromDatabase = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == COID).ToList();
                        Boolean lDeliveryOrderDetailStatus = UpdateDeliveryOrderDetail(lCustomerOrderDetailsFromDatabase, DeliveryCharge, DeliveryCharge);

                        //-- 6 ---------------------------------------------------------------------------------------------------//
                        Boolean lCustomerOrderHistoryStatus = InsertCustomerOrderHistory(COID, lCustomerOrderDetails);

                        //-- 7 ---------------------------------------------------------------------------------------------------//
                        long lUserDefinedLogID = InsertUserDefinedLog(COID, Log);

                        if (sendSMSToCustomer != null)
                        {
                            Boolean lSmsEmailSentStatus = SendEmaiAndSMSToCustomer(CType, lCustomerOrderDetailsFromDatabase, lPayableAmount, DeliveryCharge);
                        }

                        ts.Complete();
                    }
                    catch (BusinessLogicLayer.MyException myEx)
                    {
                        Transaction.Current.Rollback();
                        ts.Dispose();
                        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                            + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                            + "[CustomerOrderDetailAlignment][POST:EditOrderList]" + myEx.EXCEPTION_PATH,
                            BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                    }
                    catch (Exception ex)
                    {
                        Transaction.Current.Rollback();
                        ts.Dispose();
                        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                            + Environment.NewLine + ex.Message + Environment.NewLine
                            + "[CustomerOrderDetailAlignment][POST:EditOrderList]",
                            BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                    }
                }
                //-- 8 ---------------------------------------------------------------------------------------------------//
                Boolean lIsReflectedInAccount = InsertUpdateTransactionInputOnOrderAlignment(COID);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderDetailAlignment][POST:EditOrderList]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderDetailAlignment][POST:EditOrderList]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return RedirectToAction("Index", "CustomerOrderDetailAlignment", new { COID = COID });
        }
        private long _UpdateCustomerOrder(long pCustomerOrderID, decimal pOrderAmount, decimal pPayableAmount, decimal MLMAmtUsed, decimal pTotalBusinessPoints)//, decimal pTotalOrderAmount) //Added by Rumana
        {
            try
            {
                //EzeeloDBContext db1 = new EzeeloDBContext();
                ModelLayer.Models.CustomerOrder lCustomerOrder = db.CustomerOrders.Find(pCustomerOrderID);
                if (lCustomerOrder == null)
                {
                    return -1;
                }
                //lCustomerOrder.TotalOrderAmount = pTotalOrderAmount;//Yashaswi 18-9-2018
                lCustomerOrder.OrderAmount = pOrderAmount;
                lCustomerOrder.PayableAmount = pPayableAmount;
                lCustomerOrder.MLMAmountUsed = MLMAmtUsed;
                lCustomerOrder.BusinessPointsTotal = pTotalBusinessPoints;
                if (lCustomerOrder.CoupenAmount != null && lCustomerOrder.CoupenAmount > 0)
                {
                    lCustomerOrder.PayableAmount = lCustomerOrder.PayableAmount - (decimal)lCustomerOrder.CoupenAmount;
                }
                //Condition added by Zubair on 15-09-2017 for handling Wallet amount if used by customer
                decimal lEarnUsedAmount = Convert.ToDecimal(db.EarnDetails.Where(x => x.CustomerOrderID == pCustomerOrderID).Select(x => x.UsedAmount).FirstOrDefault());
                if (lEarnUsedAmount > 0)
                {
                    if ((lCustomerOrder.PayableAmount - (decimal)lEarnUsedAmount) > 0)
                    {
                        lCustomerOrder.PayableAmount = lCustomerOrder.PayableAmount - (decimal)lEarnUsedAmount;
                    }
                }
                lCustomerOrder.ModifyBy = customerCareSessionViewModel.PersonalDetailID;
                lCustomerOrder.ModifyDate = DateTime.Now;
                //db1.Entry(lCustomerOrder).State = EntityState.Modified;
                db.SaveChanges();
                return lCustomerOrder.ID;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[UpdateCustomerOrder]", "Can't Update CustomerOrder" + Environment.NewLine + "[Exception : " + ex.Message + "], [InnerException : " + ex.InnerException.Message + "]");
            }
        }
        #endregion

        #region ADD NEW PRODUCT
        public ActionResult GetProducts(string PName)
        {
            List<ProductListViewModel> lProductList = new List<ProductListViewModel>();
            try
            {
                if (!string.IsNullOrEmpty(PName))
                {
                    CustomerDetailReport lOrder = new CustomerDetailReport();
                    int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                    lProductList = lOrder.ProductWithStock(PName, franchiseID);


                    NewCustomerOrderDetailListViewModel lCustomerOrderDetailViewModel = new NewCustomerOrderDetailListViewModel();
                    if (TempData["CustomerOrderDetailViewModel"] != null)
                    {
                        lCustomerOrderDetailViewModel = (NewCustomerOrderDetailListViewModel)TempData["CustomerOrderDetailViewModel"];
                        TempData.Keep();
                    }

                    lProductList = lProductList.Where(p => !lCustomerOrderDetailViewModel.customerOrderDetails.Any(p2 => p2.ShopStockID == p.ShopStockID)).ToList();

                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderDetail][GET:GetProducts]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderDetail][GET:GetProducts]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return PartialView("_AddNewProduct", lProductList);
        }

        public JsonResult AddNewProduct(long COID, string ProductDetail)
        {
            try
            {
                string[] str = ProductDetail.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<CustomerOrderDetail> listOrderDetails = new List<CustomerOrderDetail>();

                BusinessLogicLayer.CustomerOrder customerOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                NewCustomerOrderDetailListViewModel lCustomerOrderDetailViewModel = (NewCustomerOrderDetailListViewModel)TempData["CustomerOrderDetailViewModel"];
                List<Franchise.Models.ViewModel.ProductList> lProductList = (List<Franchise.Models.ViewModel.ProductList>)TempData["ProductList"];
                for (int i = 0; i < str.Length; i++)
                {
                    long ShopStockID = 0;//, ShopID = 0;
                    // int Qty = 0;
                    // string[] values = str[i].Split(new Char[] { '$' }, StringSplitOptions.RemoveEmptyEntries);
                    //long.TryParse(values[0], out ShopStockID);
                    //long.TryParse(values[1], out ShopID);
                    //  int.TryParse(values[2], out Qty);

                    long.TryParse(str[i], out ShopStockID);

                    var item = db.ShopStocks.Find(ShopStockID);
                    CustomerOrderDetail customerOrderDetail = new CustomerOrderDetail();
                    customerOrderDetail.ID = 0;
                    customerOrderDetail.ShopOrderCode = string.Empty;
                    customerOrderDetail.CustomerOrderID = COID;
                    customerOrderDetail.ShopStock = item;
                    customerOrderDetail.ShopStockID = ShopStockID;
                    customerOrderDetail.WarehouseStockID = item.WarehouseStockID; //Added by Zubair for Inventory on 28-03-2018
                    customerOrderDetail.ShopID = item.ShopProduct.ShopID;
                    customerOrderDetail.Qty = 1;
                    customerOrderDetail.OrderStatus = 1;
                    customerOrderDetail.MRP = item.MRP;
                    customerOrderDetail.SaleRate = item.RetailerRate;
                    customerOrderDetail.OfferPercent = 0;
                    customerOrderDetail.OfferRs = 0;
                    customerOrderDetail.IsInclusivOfTax = item.IsInclusiveOfTax;
                    customerOrderDetail.IsActive = true;
                    customerOrderDetail.TotalAmount = item.RetailerRate;// *1;
                    customerOrderDetail.BusinessPointPerUnit = item.BusinessPoints; //Changes by Zubair for MLm on 6-01-2018
                    customerOrderDetail.BusinessPoints = item.BusinessPoints; //Changes by Zubair for MLm on 6-01-2018

                    lCustomerOrderDetailViewModel.customerOrderDetails.Add(customerOrderDetail);

                    Franchise.Models.ViewModel.ProductList newProduct = new Franchise.Models.ViewModel.ProductList();
                    newProduct.Name = item.ShopProduct.Product.Name;
                    newProduct.ShopStockID = item.ID;
                    lProductList.Add(newProduct);
                }
                TempData["CustomerOrderDetailViewModel"] = lCustomerOrderDetailViewModel;
                TempData["ProductList"] = lProductList;
                TempData.Keep();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
                throw;
            }


        }
        #endregion

        #region VIEW LOG

        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrderDetailAlignment/CanRead")]
        public ActionResult ViewLogs(long COID)
        {
            //List<CustomerOrderUserDefinedLog> lCustomerOrderUserDefinedLogs = db.CustomerOrderUserDefinedLogs.Where(x => x.CustomerOrderID == COID).ToList();
            List<CustomerOrderUserDefinedLogViewModel> lCustomerOrderUserDefinedLogViewModels = (from coudl in db.CustomerOrderUserDefinedLogs.Where(x => x.CustomerOrderID == COID).ToList()
                                                                                                 join pd in db.PersonalDetails on coudl.CreateBy equals pd.ID
                                                                                                 select new CustomerOrderUserDefinedLogViewModel
                                                                                                 {
                                                                                                     ID = coudl.ID,
                                                                                                     CustomerOrderID = coudl.CustomerOrderID,
                                                                                                     Description = coudl.Description,
                                                                                                     IsActive = coudl.IsActive,
                                                                                                     CreateDate = coudl.CreateDate,
                                                                                                     CreateBy = coudl.CreateBy,
                                                                                                     ModifyDate = coudl.ModifyDate,
                                                                                                     ModifyBy = coudl.ModifyBy,
                                                                                                     NetworkIP = coudl.NetworkIP,
                                                                                                     DeviceType = coudl.DeviceType,
                                                                                                     DeviceID = coudl.DeviceID,
                                                                                                     PersonName = pd.FirstName
                                                                                                 }).OrderByDescending(x => x.CreateDate).ToList();

            ViewBag.CustomerOrderUserDefinedLogViewModels = lCustomerOrderUserDefinedLogViewModels;
            return View();
        }

        #endregion

        #region VIEW SYSTEM LOG

        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrderDetailAlignment/CanRead")]
        public ActionResult ViewSystemLog(long COID)
        {
            List<CustomerOrderSystemLogViewModel> lCustomerOrderSystemLogViewModels = new List<CustomerOrderSystemLogViewModel>();
            try
            {
                lCustomerOrderSystemLogViewModels = (from COLog in db.CustomerOrderLogs
                                                     join pd in db.PersonalDetails on COLog.CreateBy equals pd.ID
                                                     join COD in db.CustomerOrderDetails on COLog.ReferenceCustomerOrderID equals COD.CustomerOrderID
                                                     join DOD in db.DeliveryOrderDetails on COD.ShopOrderCode equals DOD.ShopOrderCode
                                                     join AR in db.Areas on COLog.AreaID equals AR.ID into B
                                                     from A in B.DefaultIfEmpty()
                                                     join P in db.Pincodes on COLog.PincodeID equals P.ID
                                                     where COLog.ReferenceCustomerOrderID == COID && COD.IsActive == true
                                                     select new CustomerOrderSystemLogViewModel
                                                     {
                                                         ID = COLog.ID,
                                                         COID = (long)COLog.ReferenceCustomerOrderID,
                                                         PersonName = pd.FirstName + " " + pd.LastName,
                                                         CreateDate = COLog.CreateDate,
                                                         OrderAmount = COLog.OrderAmount,
                                                         CoupenAmount = COLog.CoupenAmount,
                                                         DeliveryCharge = DOD.DeliveryCharge,
                                                         PayableAmount = COLog.PayableAmount,
                                                         ShippingAddr = COLog.ShippingAddress,
                                                         Area = A.Name,
                                                         Pincode = P.Name,
                                                         City = P.City.Name,
                                                         PrimaryMobile = COLog.PrimaryMobile,
                                                         SecondaryMobile = COLog.SecondoryMobile
                                                     }).Distinct().ToList();



                foreach (CustomerOrderSystemLogViewModel cosl in lCustomerOrderSystemLogViewModels)
                {
                    List<CustomerOrderDetailSystemLogViewModel> lCustomerOrderDetailSystemLogViewModels = (from CODLog in db.CustomerOrderDetailLogs
                                                                                                           join pd in db.PersonalDetails on CODLog.CreateBy equals pd.ID
                                                                                                           join ss in db.ShopStocks on CODLog.ShopStockID equals ss.ID
                                                                                                           where CODLog.CustomerOrderID == cosl.ID
                                                                                                           select new CustomerOrderDetailSystemLogViewModel
                                                                                                           {
                                                                                                               ID = CODLog.ID,
                                                                                                               ReferenceShopOrderCode = CODLog.ReferenceShopOrderCode,
                                                                                                               ShopStockID = CODLog.ShopStockID,
                                                                                                               ProductName = ss.ShopProduct.Product.Name,
                                                                                                               OrderStatus = CODLog.OrderStatus,
                                                                                                               MRP = CODLog.MRP,
                                                                                                               SaleRate = CODLog.SaleRate,
                                                                                                               Qty = CODLog.Qty,
                                                                                                               TotalAmount = CODLog.TotalAmount,
                                                                                                               CreateBy = CODLog.CreateBy,
                                                                                                               CreateDate = CODLog.CreateDate,
                                                                                                               ChangeByPersonName = pd.FirstName + " " + pd.LastName
                                                                                                           }).Distinct().ToList();
                    cosl.CustomerOrderDetailSystemLogList = lCustomerOrderDetailSystemLogViewModels.OrderBy(x => x.ReferenceShopOrderCode).ToList();
                }

            }
            catch (Exception ex)
            {

                throw;
            }

            //   List<CustomerOrderSystemLogViewModel> lCustomerOrderSystemLogViewModel_Final = lCustomerOrderSystemLogViewModels.Where(x => x.CustomerOrderDetailSystemLogList.Count() == 0 && x.ShippingAddr != db.CustomerOrders.Find(COID).ShippingAddress).ToList();
            //  lCustomerOrderSystemLogViewModels = lCustomerOrderSystemLogViewModels.Where(x => x.CustomerOrderDetailSystemLogList.Count() > 0).ToList();

            return View(lCustomerOrderSystemLogViewModels.OrderByDescending(x => x.CreateDate));
        }

        #endregion

        #region POD
        //Print Memo
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrderDetailAlignment/CanPrint")]
        public ActionResult ForPrint(long CustomerOrderID)
        {
            var customerorderdetails = db.CustomerOrderDetails.Include(c => c.CustomerOrder).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.Shop).Include(c => c.ShopStock).Where(x => x.CustomerOrderID == CustomerOrderID).OrderByDescending(x => x.ID).ToList();
            //-- Start Tax Include on 31-march-2016 , By Avi Verma. 
            List<TaxOnOrderViewModel> lTaxOnOrderViewModels = getTaxOnOrderViewModels(CustomerOrderID);

            ViewBag.TaxOnOrderViewModels = lTaxOnOrderViewModels;
            //-- End Tax Include on 31-march-2016 , By Avi Verma. 

            List<DeliveryChargeViewModel> lDeliveryChargeViewModel = (from dod in db.DeliveryOrderDetails
                                                                      join cod in db.CustomerOrderDetails on dod.ShopOrderCode equals cod.ShopOrderCode
                                                                      join co in db.CustomerOrders on cod.CustomerOrderID equals co.ID
                                                                      where co.ID == CustomerOrderID
                                                                      select new DeliveryChargeViewModel
                                                                      {
                                                                          ShopOrderCode = dod.ShopOrderCode,
                                                                          DeliveryCharge = dod.GandhibaghCharge,
                                                                      }).Distinct().ToList();


            //decimal lDeliveryCharge = lDeliveryChargeViewModel.Sum(x => x.DeliveryCharge);
            decimal lDeliveryCharge = lDeliveryChargeViewModel.FirstOrDefault().DeliveryCharge;
            ViewBag.DeliveryCharge = lDeliveryCharge;

            if (db.EarnDetails.FirstOrDefault(x => x.CustomerOrderID == CustomerOrderID) != null)
            {
                ViewBag.EarnedAmount = db.EarnDetails.FirstOrDefault(x => x.CustomerOrderID == CustomerOrderID).UsedAmount;
            }



            return View("_ForPrintMemo", customerorderdetails.ToList());
        }

        #endregion

        # region METHODS

        public CustomerOrderAndItsCustomerOrderDetailViewModel Get(long COID)
        {
            CustomerOrder lCustomerOrderForDateRange = db.CustomerOrders.Find(COID);
            if (lCustomerOrderForDateRange == null)
            {
                return null;
            }
            int FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
            CustomerDetailReport lOrder = new CustomerDetailReport();
            List<OrderListStatusCount> lStatusCounts = new List<OrderListStatusCount>();
            NewCustomerOrderViewModel lCustomerOrderViewModel = lOrder.ListOrders(null, FranchiseID, lCustomerOrderForDateRange.CreateDate.AddMinutes(-1), lCustomerOrderForDateRange.CreateDate.AddMinutes(1), 1, 1, lCustomerOrderForDateRange.OrderCode, "", "", 0, 0, "", "", "", out lStatusCounts).FirstOrDefault(x => x.COID == COID);
            //Commented and modified by Zubair on 19-06-2018. take only before 7 days orders. Ref:Ashis Sahu
            //List<NewCustomerOrderViewModel> lThisCustomerPreviousOrderViewModels = lOrder.ListOrders(null, FranchiseID, lCustomerOrderForDateRange.CreateDate.AddYears(-1), lCustomerOrderForDateRange.CreateDate).Where(x => x.UserLoginID == lCustomerOrderViewModel.UserLoginID && x.COID != COID).OrderByDescending(x => x.COID).Take(5).ToList();
            List<NewCustomerOrderViewModel> lThisCustomerPreviousOrderViewModels = lOrder.ListOrders(null, FranchiseID, lCustomerOrderForDateRange.CreateDate.AddDays(-6), lCustomerOrderForDateRange.CreateDate, 1, 5, "", "", lCustomerOrderViewModel.Customer, 0, 0, "", "", "", out lStatusCounts).Where(x => x.UserLoginID == lCustomerOrderViewModel.UserLoginID && x.COID != COID).OrderByDescending(x => x.COID).Take(5).ToList();
            //End
            List<NewCustomerOrderDetailViewModel> lCustomerOrderDetailViewModels = lOrder.CustomerOrderDetail(COID);
            CustomerOrderAndItsCustomerOrderDetailViewModel lCustomerOrderAndItsCustomerOrderDetailViewModel = new CustomerOrderAndItsCustomerOrderDetailViewModel();
            lCustomerOrderAndItsCustomerOrderDetailViewModel.customerOrderViewModel = lCustomerOrderViewModel;
            lCustomerOrderAndItsCustomerOrderDetailViewModel.lThisCustomerPreviousOrderViewModel = lThisCustomerPreviousOrderViewModels;
            lCustomerOrderAndItsCustomerOrderDetailViewModel.CustomerOrderDetailViewModels = lCustomerOrderDetailViewModels;
            return lCustomerOrderAndItsCustomerOrderDetailViewModel;
        }

        private Boolean SendEmaiAndSMSToCustomer(string CType, List<CustomerOrderDetail> obj, decimal pPayableAmount, decimal DeliveryCharge)
        {
            try
            {
                bool Flag = false;
                long UID = obj.FirstOrDefault().CustomerOrder.UserLoginID;
                string lFirstName = Convert.ToString(db.PersonalDetails.FirstOrDefault(x => x.UserLoginID == UID).FirstName);
                string lOrderCode = obj.FirstOrDefault().CustomerOrder.OrderCode;
                decimal lOrderAmount = pPayableAmount;
                string lMobile = obj.FirstOrDefault().CustomerOrder.UserLogin.Mobile;
                string lEmail = obj.FirstOrDefault().CustomerOrder.UserLogin.Email;
                var deli_schedule = obj.FirstOrDefault().CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault();
                string delSlot = deli_schedule.DeliveryDate.ToString("dd/MM/yyyy") + " - " + deli_schedule.DeliverySchedule.DisplayName;
                string lShippingAddr = obj.FirstOrDefault().CustomerOrder.ShippingAddress;
                string contact = obj.FirstOrDefault().CustomerOrder.PrimaryMobile;

                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.GateWay gateWay = null;
                Dictionary<string, string> dictionary = new Dictionary<string, string>();

                try
                {
                    string city = "nagpur";
                    int franchiseID = 2;////added for Multiple MCO in Same City
                    if (System.Web.HttpContext.Current.Request.Cookies["CityCookie"] != null && System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value != "")
                    {
                        city = System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                        franchiseID = Convert.ToInt32(System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[2]); ////added for Multiple MCO in Same City
                    }
                    gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                    dictionary.Add("<!--NAME-->", lFirstName);
                    dictionary.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseID + "/login");////added "/" + franchiseID + for Multiple MCO in Same City
                    dictionary.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseID + "/cust-o/my-order");////added "/" + franchiseID + for Multiple MCO in Same City
                    dictionary.Add("<!--DELIVERY_ADDRESS-->", lShippingAddr);
                    dictionary.Add("<!--ORDER_NUMBER-->", lOrderCode);
                    dictionary.Add("<!--DELI_SLOT-->", delSlot);
                    dictionary.Add("<!--CONTACT_NUMBER-->", contact);
                    dictionary.Add("<!--PRODUCT_DETAILS-->", GetProductDetails(obj, DeliveryCharge));

                    BusinessLogicLayer.GateWay.EMailTypes EmailType = BusinessLogicLayer.GateWay.EMailTypes.CUST_ORD_FULFILL;
                    switch (CType)
                    {
                        case "CUST_ORD_FULFILL":
                        case "CUST_PTL_ORD_FULFILL":
                            EmailType = BusinessLogicLayer.GateWay.EMailTypes.CUST_ORD_FULFILL;
                            break;
                        case "CUST_CHN_ORD":
                            EmailType = BusinessLogicLayer.GateWay.EMailTypes.CUST_CHN_ORD;
                            break;
                    }
                    //Flag = gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, EmailType, new string[] { "snehal.shende@ezeelo.com" }, dictionary, true);
                    Flag = gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, EmailType, new string[] { lEmail, rcKey.DEFAULT_ALL_EMAIL }, dictionary, true);
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    ModelState.AddModelError("Message", "There might be problem sending email, please check your email or contact administrator!");
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[CustomerOrderDetailAlignment][SendEmailToCustomer]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[CustomerOrderDetailAlignment][SendEmailToCustomer]",
                        BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                }

                //------Key value add in SMS---------//
                Flag = false;
                gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                dictionary.Clear();
                if (!string.IsNullOrEmpty(lFirstName))
                {
                    dictionary.Add("#--NAME--#", lFirstName);
                }
                else
                {
                    dictionary.Add("#--NAME--#", lEmail);
                }
                dictionary.Add("#--ORD_NUM--#", lOrderCode);
                dictionary.Add("#--DELI_SLOT--#", delSlot);
                //dictionary.Add("#--AMOUNT--#", lOrderAmount.ToString());
                BusinessLogicLayer.GateWay.SMSTypes CommType = BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_FULFILL;
                switch (CType)
                {
                    case "CUST_ORD_FULFILL":
                        CommType = BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_FULFILL;
                        break;
                    case "CUST_PTL_ORD_FULFILL":
                        CommType = BusinessLogicLayer.GateWay.SMSTypes.CUST_PTL_ORD_FULFILL;
                        break;
                    case "CUST_CHN_ORD":
                        CommType = BusinessLogicLayer.GateWay.SMSTypes.CUST_CHN_ORD;
                        break;
                }
                Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, CommType, new string[] { lMobile, rcKey.DEFAULT_ALL_SMS }, dictionary);

                return Flag;
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "There might be problem sending email, please check your email or contact administrator!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderDetailAlignment][SendEmaiAndSMSToCustomer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderDetailAlignment][SendEmaiAndSMSToCustomer]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return false;
        }

        private string GetProductDetails(List<CustomerOrderDetail> obj, decimal DeliveryCharge)
        {
            try
            {
                List<TaxOnOrderViewModel> lTaxOnOrderViewModels = getTaxOnOrderViewModels(obj.FirstOrDefault().CustomerOrderID);
                StringBuilder lProductDetails = new StringBuilder();
                lProductDetails.Append("<table style='border:1px solid black;border-collapse: collapse;width:100%;'><tr>" +
                    "<th style='border:1px solid black;border-collapse: collapse;width:40%;'>Product</th>" +
                    "<th style='border:1px solid black;border-collapse: collapse;width:10%;'>MRP</th>" +
                    "<th style='border:1px solid black;border-collapse: collapse;width:10%;'>Rate</th>" +
                    "<th style='border:1px solid black;border-collapse: collapse;width:10%;'>Qty</th>" +
                    "<th style='border:1px solid black;border-collapse: collapse;width:20%;'>Saving</th>" +
                    "<th style='border:1px solid black;border-collapse: collapse;width:10%;'>Amount</th></tr>");

                foreach (CustomerOrderDetail lCustomerOrderDetail in obj.Where(x => x.IsActive == true))
                {
                    lProductDetails.Append("<tr><td style='border:1px solid black;text-align: left;border-collapse: collapse;width:40%;'>" + lCustomerOrderDetail.ShopStock.ShopProduct.Product.Name + "<b> " + lCustomerOrderDetail.ShopStock.ProductVarient.Size.Name + "</b></td>" +
                        "<td style='border:1px solid black;text-align: center;border-collapse: collapse;width:10%;'>" + lCustomerOrderDetail.MRP + "</td>" +
                        "<td style='border:1px solid black;text-align: center;border-collapse: collapse;width:10%;'>" + lCustomerOrderDetail.SaleRate + "</td>" +
                        "<td style='border:1px solid black;text-align: center;border-collapse: collapse;width:10%;'>" + lCustomerOrderDetail.Qty + "</td>" +
                        "<td style='border:1px solid black;text-align: center;border-collapse: collapse;width:20%;'>" + ((lCustomerOrderDetail.Qty * lCustomerOrderDetail.MRP) - lCustomerOrderDetail.TotalAmount) + "</td>" +
                        "<td style='border:1px solid black;text-align: center;border-collapse: collapse;width:10%;'>" + lCustomerOrderDetail.TotalAmount + "</td></tr>");
                }
                foreach (CustomerOrderDetail lCustomerOrderDetail in obj.Where(x => x.IsActive == false))
                {
                    lProductDetails.Append("<tr><td style='border:1px solid black;text-align: left;border-collapse: collapse;width:40%;'>" + lCustomerOrderDetail.ShopStock.ShopProduct.Product.Name + "<b> " + lCustomerOrderDetail.ShopStock.ProductVarient.Size.Name + " (Not Available)</b></td>" +
                        "<td style='border:1px solid black;text-align: center;border-collapse: collapse;width:10%;'>" + lCustomerOrderDetail.MRP + "</td>" +
                        "<td style='border:1px solid black;text-align: center;border-collapse: collapse;width:10%;'>" + lCustomerOrderDetail.SaleRate + "</td>" +
                        "<td style='border:1px solid black;text-align: center;border-collapse: collapse;width:10%;'>" + lCustomerOrderDetail.Qty + "</td>" +
                        "<td style='border:1px solid black;text-align: center;border-collapse: collapse;width:20%;'>Nil</td>" +
                        "<td style='border:1px solid black;text-align: center;border-collapse: collapse;width:10%;'>Nil</td></tr>");
                }
                lProductDetails.Append("</table><br/>");
                lProductDetails.Append("<div><span style='width:10%'> Total Items : </span> <b>" + obj.Where(x => x.IsActive == true).Count() + "</b><br />" +
                    " <span style='width:10%'>Total Amount : </span><b>" + obj.FirstOrDefault().CustomerOrder.OrderAmount + "</b><br />");



                List<TaxOnOrderViewModel> lGrpsTaxOnViewModes = (from tovm in lTaxOnOrderViewModels
                                                                 group tovm by tovm.TaxID into g
                                                                 select new TaxOnOrderViewModel
                                                                 {
                                                                     TaxID = g.Key,
                                                                     TaxAmount = g.Where(x => x.TaxID == g.Key).Sum(x => x.TaxAmount),
                                                                     TaxPrefix = g.FirstOrDefault(x => x.TaxID == g.Key).TaxPrefix,
                                                                     TaxName = g.FirstOrDefault(x => x.TaxID == g.Key).TaxName,
                                                                 }).ToList();

                foreach (TaxOnOrderViewModel lTaxOnOrder in lGrpsTaxOnViewModes.OrderBy(x => x.TaxID))
                {
                    lProductDetails.Append(" <span style='width:10%'>" + lTaxOnOrder.TaxPrefix + " : </span><b>" + lTaxOnOrder.TaxAmount + "</b><br />");
                }

                if (obj.Where(x => x.IsActive == true).FirstOrDefault().CustomerOrder.CoupenAmount > 0)
                {
                    lProductDetails.Append(" <span style='width:10%'>Coupon Amount : </span><b>" + obj.Where(x => x.IsActive == true).FirstOrDefault().CustomerOrder.CoupenAmount + "</b><br />");
                }
                lProductDetails.Append(" <span style='width:10%'> Delivery Charges : </span><b>" + DeliveryCharge + "</b><br />");
                lProductDetails.Append(" <span style='width:10%'> Bill Amount : </span><b>" + obj.FirstOrDefault().CustomerOrder.PayableAmount + "</b></div>");
                return lProductDetails.ToString();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetProductDetails]", "Can't Get Product Details" + Environment.NewLine + "[Exception : " + ex.Message + "], [InnerException : " + ex.InnerException.Message + "]");
            }
        }


        private List<TaxOnOrderViewModel> getTaxOnOrderViewModels(long CustomerOrderId)
        {
            //List<TaxOnOrderViewModel> lTaxOnOrderViewModels = (from cod in pCustomerOrderDetails
            //                                                   join too in db.TaxOnOrders on cod.ID equals too.CustomerOrderDetailID
            //                                                   join PrdTax in db.ProductTaxes on too.ProductTaxID equals PrdTax.ID
            //                                                   join taxMas in db.TaxationMasters on PrdTax.TaxID equals taxMas.ID
            //                                                   select new TaxOnOrderViewModel
            //                                                   {
            //                                                       TaxOnOrderID = too.ID,
            //                                                       CustomerOrderDetailID = cod.ID,
            //                                                       ProductTaxID = PrdTax.ID,
            //                                                       TaxAmount = too.Amount,
            //                                                       TaxID = PrdTax.TaxID,
            //                                                       TaxPrefix = taxMas.Prefix,
            //                                                       TaxName = taxMas.Name
            //                                                   }).ToList();
            List<TaxOnOrderViewModel> lTaxOnOrderViewModels = (new CustomerDetailReport()).getTaxOnOrderViewModels(CustomerOrderId);
            return lTaxOnOrderViewModels;
        }
        private decimal CaluclateTaxTotal(List<CustomerOrderDetail> pCustomerOrderDetails)
        {
            decimal lTotalTax = 0;
            BusinessLogicLayer.TaxationManagement objTaxationManagement = new BusinessLogicLayer.TaxationManagement(fConnectionString.Trim());
            foreach (CustomerOrderDetail customerOrderDetail in pCustomerOrderDetails)
            {
                List<ModelLayer.Models.ViewModel.CalulatedTaxesRecord> lCalulatedTaxesRecords = objTaxationManagement.CalculateTaxForProduct(customerOrderDetail.ShopStockID);
                decimal lSumOfAllTaxOnShopStockID = lCalulatedTaxesRecords.Where(x => x.IsGSTInclusive == false).Sum(x => x.TaxableAmount * customerOrderDetail.Qty);  //Where condition added by Zubair for GST/MLM on 08-03-2018
                lTotalTax += lSumOfAllTaxOnShopStockID;
            }
            return lTotalTax;
        }
        private bool UpdateTaxOnOrderDetail(List<TaxOnOrder> lTaxOnOrder, List<CustomerOrderDetail> lOldCustomerOrderDetails)
        {

            try
            {
                BusinessLogicLayer.TaxationManagement objTaxationManagement = new BusinessLogicLayer.TaxationManagement(fConnectionString.Trim());
                foreach (CustomerOrderDetail customerOrderDetail in lOldCustomerOrderDetails)
                {
                    List<ModelLayer.Models.ViewModel.CalulatedTaxesRecord> lCalulatedTaxesRecords = objTaxationManagement.CalculateTaxForProduct(customerOrderDetail.ShopStockID);
                    foreach (ModelLayer.Models.ViewModel.CalulatedTaxesRecord pCalulatedTaxesRecord in lCalulatedTaxesRecords)
                    {
                        TaxOnOrder to = lTaxOnOrder.Where(x => x.CustomerOrderDetailID == customerOrderDetail.ID && x.ProductTaxID == pCalulatedTaxesRecord.ProductTaxID).FirstOrDefault();
                        if (to != null)
                        {
                            to.Amount = pCalulatedTaxesRecord.TaxableAmount * customerOrderDetail.Qty;
                            to.ModifyBy = customerCareSessionViewModel.PersonalDetailID;
                            to.ModifyDate = DateTime.Now;
                            db.SaveChanges();
                        }
                        else
                        {
                            to = new TaxOnOrder();
                            to.Amount = pCalulatedTaxesRecord.TaxableAmount * customerOrderDetail.Qty;
                            to.CustomerOrderDetailID = customerOrderDetail.ID;
                            to.DeviceID = "X";
                            to.DeviceType = "X";
                            to.CreateDate = DateTime.Now;
                            to.CreateBy = customerCareSessionViewModel.PersonalDetailID;
                            to.ModifyBy = null;
                            to.ModifyDate = null;
                            to.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                            to.ProductTaxID = pCalulatedTaxesRecord.ProductTaxID;
                            db.TaxOnOrders.Add(to);
                            db.SaveChanges();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[UpdateTaxOnOrderDetail]", "Can't Update TaxOnOrder Detail" + Environment.NewLine + "[Exception : " + ex.Message + "], [InnerException : " + ex.InnerException.Message + "]");
            }
        }
        private Boolean UpdateCustomerOrderDetail(long COID, List<CustomerOrderDetail> pCustomerOrderDetail)
        {
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                List<CustomerOrderDetail> lCustomerOrderDetails = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == COID).ToList();

                foreach (CustomerOrderDetail customerOrderDetail in lCustomerOrderDetails)
                {
                    customerOrderDetail.Qty = pCustomerOrderDetail.FirstOrDefault(x => x.ID == customerOrderDetail.ID).Qty;
                    customerOrderDetail.MRP = pCustomerOrderDetail.FirstOrDefault(x => x.ID == customerOrderDetail.ID).MRP;
                    customerOrderDetail.SaleRate = pCustomerOrderDetail.FirstOrDefault(x => x.ID == customerOrderDetail.ID).SaleRate;
                    customerOrderDetail.TotalAmount = customerOrderDetail.SaleRate * customerOrderDetail.Qty;
                    customerOrderDetail.OrderStatus = pCustomerOrderDetail.FirstOrDefault(x => x.ID == customerOrderDetail.ID).OrderStatus;
                    customerOrderDetail.IsActive = pCustomerOrderDetail.FirstOrDefault(x => x.ID == customerOrderDetail.ID).IsActive;
                    //Yashaswi 9-1-2018
                    customerOrderDetail.BusinessPoints = customerOrderDetail.BusinessPointPerUnit * customerOrderDetail.Qty;
                    customerOrderDetail.CashbackPointPerUnit = customerOrderDetail.CashbackPointPerUnit;
                    customerOrderDetail.CashbackPoints = customerOrderDetail.CashbackPointPerUnit * customerOrderDetail.Qty;
                    customerOrderDetail.ModifyBy = customerCareSessionViewModel.PersonalDetailID;
                    customerOrderDetail.ModifyDate = DateTime.Now;
                    db.Entry(customerOrderDetail).State = EntityState.Modified;
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[UpdateCustomerOrderDetail]", "Can't Update CustomerOrderDetail" + Environment.NewLine + "[Exception : " + ex.Message + "], [InnerException : " + ex.InnerException.Message + "]");
            }
        }


        //Added by Zubair for Inventory on 13-01-2018
        //Update quantity of product from WarehouseStock
        private Boolean UpdateWarehouseStock(long COID, List<CustomerOrderDetail> pCustomerOrderDetail, out int availableQty, out long ErrorOnShopStockID)
        {
            bool result = true;
            availableQty = 0;
            ErrorOnShopStockID = 0;

            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                //List<CustomerOrderDetail> lCustomerOrderDetails = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == COID).ToList();

                foreach (CustomerOrderDetail customerOrderDetail in pCustomerOrderDetail)
                {
                    //Added by Zubair for Inventory on 04-04-2018
                    //Used to update ShopStock table 
                    var getLog = db.ShopStockOrderDetailLogs.Where(x => x.ShopStockID == customerOrderDetail.ShopStockID && x.CustomerOrderDetailID == customerOrderDetail.ID).FirstOrDefault();
                    BusinessLogicLayer.CustomerOrder obj = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);

                    if (getLog != null && getLog.ID > 0)
                    {
                        int oldStatus = getLog.OrderStatus;
                        int oldQty = getLog.Quantity;
                        int newQty = 0;

                        if (oldQty != customerOrderDetail.Qty && oldStatus != customerOrderDetail.OrderStatus)
                        {
                            newQty = oldQty - customerOrderDetail.Qty;

                            if (customerOrderDetail.OrderStatus == 1 || customerOrderDetail.OrderStatus == 2 || customerOrderDetail.OrderStatus == 3
                                || customerOrderDetail.OrderStatus == 4 || customerOrderDetail.OrderStatus == 5 || customerOrderDetail.OrderStatus == 6)
                            {
                                obj.ManageStock(customerOrderDetail.ShopStockID, newQty, customerOrderDetail.WarehouseStockID, customerOrderDetail.OrderStatus, customerOrderDetail.ID); //last Parameter added by Zubair for Inventory on 28-03-2018
                            }
                        }
                        else if (oldQty == customerOrderDetail.Qty && oldStatus != customerOrderDetail.OrderStatus)
                        {
                            if (customerOrderDetail.OrderStatus == 9 || customerOrderDetail.OrderStatus == 8)
                            {
                                newQty = customerOrderDetail.Qty;
                                obj.ManageStock(customerOrderDetail.ShopStockID, newQty, customerOrderDetail.WarehouseStockID, customerOrderDetail.OrderStatus, customerOrderDetail.ID); //last Parameter added by Zubair for Inventory on 28-03-2018
                            }
                        }
                        else if (oldQty != customerOrderDetail.Qty && oldStatus == customerOrderDetail.OrderStatus)
                        {
                            newQty = oldQty - customerOrderDetail.Qty;

                            if (customerOrderDetail.OrderStatus == 1 || customerOrderDetail.OrderStatus == 2 || customerOrderDetail.OrderStatus == 3
                                || customerOrderDetail.OrderStatus == 4 || customerOrderDetail.OrderStatus == 5 || customerOrderDetail.OrderStatus == 6)
                            {
                                obj.ManageStock(customerOrderDetail.ShopStockID, newQty, customerOrderDetail.WarehouseStockID, customerOrderDetail.OrderStatus, customerOrderDetail.ID); //last Parameter added by Zubair for Inventory on 28-03-2018
                            }
                        }

                        //End

                        // if (oldStatus != customerOrderDetail.OrderStatus || oldQty != customerOrderDetail.Qty)
                        // {
                        //     if (customerOrderDetail.OrderStatus == 1 || customerOrderDetail.OrderStatus == 9 || customerOrderDetail.OrderStatus == 8)
                        //     {
                        //         obj.ManageStock(customerOrderDetail.ShopStockID, newQty, customerOrderDetail.WarehouseStockID, customerOrderDetail.OrderStatus, customerOrderDetail.ID); //last Parameter added by Zubair for Inventory on 28-03-2018
                        //     }
                        //}
                    }
                    //End



                    long shopStockID = pCustomerOrderDetail.FirstOrDefault(x => x.ID == customerOrderDetail.ID).ShopStockID;
                    long Qty = pCustomerOrderDetail.FirstOrDefault(x => x.ID == customerOrderDetail.ID).Qty;

                    if (customerOrderDetail.WarehouseStockID != null && customerOrderDetail.WarehouseStockID > 0)
                    {
                        long WarehouseStockID = Convert.ToInt64(customerOrderDetail.WarehouseStockID);
                        if (customerOrderDetail.OrderStatus == 4 || customerOrderDetail.OrderStatus == 7 || customerOrderDetail.OrderStatus == 8) // If Item is Delivered or Returned
                        {
                            var logId = db.WarehouseStockOrderDetailLogs.Where(x => x.WarehouseStockID == WarehouseStockID && x.CustomerOrderDetailID == customerOrderDetail.ID
                       && x.OrderStatus == customerOrderDetail.OrderStatus && x.Quantity == Qty).Select(x => x.ID).FirstOrDefault();

                            if (logId == null || logId == 0)
                            {
                                result = ManageWarehouseStock(customerOrderDetail.ID, WarehouseStockID, shopStockID, Qty, customerOrderDetail.OrderStatus, out availableQty, out ErrorOnShopStockID);
                                if (result == false)
                                {
                                    return result;
                                }
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
                //throw new BusinessLogicLayer.MyException("[UpdateWarehouseStock]", "Can't Update CustomerOrderDetail" + Environment.NewLine + "[Exception : " + ex.Message + "], [InnerException : " + ex.InnerException.Message + "]");
            }
        }

        //Qty will never changed while updating delivered to Return or wise warsa
        public bool ManageWarehouseStock(long CustomerOrderDetailID, long WarehouseStockID, long shopStockID, long qty, int orderStatus, out int AvailableQuantity, out long ErrorOnShopStockID)
        {
            bool flag = false;
            AvailableQuantity = 0;
            ErrorOnShopStockID = shopStockID;
            try
            {
                var stockRow = (from ws in db.WarehouseStocks
                                where ws.ID == WarehouseStockID
                                select new
                                {
                                    ID = ws.ID,
                                    BatchCode = ws.BatchCode,
                                    AvailableQuantity = ws.AvailableQuantity,
                                    InvoiceID = ws.InvoiceID,
                                    ExpiryDate = ws.ExpiryDate
                                }).Where(x => x.AvailableQuantity > 0).OrderBy(x => x.InvoiceID).FirstOrDefault();

                if (stockRow != null || orderStatus == 8)//ensure product is available in stock
                {
                    //if (stockRow.AvailableQuantity>= qty) //ensure stock is able to deliver required quantity
                    //{
                    WarehouseStock warehouseStocks = db.WarehouseStocks.Where(x => x.ID == WarehouseStockID).FirstOrDefault();
                    if (orderStatus == 8)
                    {
                        if (stockRow == null)
                        {
                            AvailableQuantity = 0;
                        }
                        else
                        {
                            AvailableQuantity = stockRow.AvailableQuantity;
                        }
                    }
                    else
                    {
                        AvailableQuantity = stockRow.AvailableQuantity;
                    }
                    if (AvailableQuantity < qty && orderStatus == 7)
                    {
                        return flag;
                    }
                    else if (orderStatus == 4 && AvailableQuantity >= qty)
                    {
                        flag = true;
                        return flag;
                    }

                    if (orderStatus == 7 && AvailableQuantity >= qty) //On Delivered
                    {
                        warehouseStocks.AvailableQuantity = warehouseStocks.AvailableQuantity - Convert.ToInt32(qty);
                    }
                    else if (orderStatus == 8) //On Returned
                    {
                        warehouseStocks.AvailableQuantity = warehouseStocks.AvailableQuantity + Convert.ToInt32(qty);
                    }
                    else
                    {
                        return flag;
                        //throw new BusinessLogicLayer.MyException("[ManageWarehouseStock]", "Can't Update WarehouseStock! items not available.");
                    }
                    //if after updation it will zero, make status as Out Of Stock
                    if (warehouseStocks.AvailableQuantity == 0)
                    {
                        warehouseStocks.StockStatus = false;
                    }
                    //if after updation it will not zero, make status as In Stock 
                    else
                    {
                        warehouseStocks.StockStatus = true;
                    }
                    db.SaveChanges();

                    InsertWarehouseStockOrderDetailLog(CustomerOrderDetailID, WarehouseStockID, Convert.ToInt32(qty), orderStatus);

                    //Change Stock level in WarehouseReorderLevel
                    var WarehouseStock = db.WarehouseStocks.Where(x => x.ID == WarehouseStockID).FirstOrDefault();

                    var WarehouseReorderLevelRow = (from ws in db.WarehouseReorderLevels
                                                    where ws.WarehouseID == WarehouseStock.WarehouseID && ws.ProductID == WarehouseStock.ProductID && ws.ProductVarientID == WarehouseStock.ProductVarientID
                                                    //&& ws.AvailableQuantity > 0
                                                    select new
                                                    {
                                                        ID = ws.ID,
                                                        AvailableQuantity = ws.AvailableQuantity
                                                    }).FirstOrDefault();

                    WarehouseReorderLevel WarehouseReorderLevel = db.WarehouseReorderLevels.Where(x => x.ID == WarehouseReorderLevelRow.ID).FirstOrDefault();
                    if (orderStatus == 7) //On Delivered
                    {
                        WarehouseReorderLevel.AvailableQuantity = WarehouseReorderLevel.AvailableQuantity - Convert.ToInt32(qty);
                    }
                    else if (orderStatus == 8) //On Returned
                    {
                        WarehouseReorderLevel.AvailableQuantity = WarehouseReorderLevel.AvailableQuantity + Convert.ToInt32(qty);
                    }

                    db.SaveChanges();
                    flag = true;  //if successful
                }
                return flag;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ManageWarehouseStock]", "Can't Update CustomerOrderDetail" + Environment.NewLine + "[Exception : " + ex.Message + "], [InnerException : " + ex.InnerException.Message + "]");
            }
        }

        private void InsertWarehouseStockOrderDetailLog(long CustomerOrderDetailID, long WarehouseStockID, int Quantity, int OrderStatus)
        {
            try
            {
                long PersonalDetailID = GetPersonalDetailID();
                EzeeloDBContext db1 = new EzeeloDBContext();

                var logId = db.WarehouseStockOrderDetailLogs.Where(x => x.WarehouseStockID == WarehouseStockID && x.CustomerOrderDetailID == CustomerOrderDetailID
                           && (x.OrderStatus != OrderStatus || x.Quantity != Quantity)).Select(x => x.ID).FirstOrDefault();

                if (logId != null && logId > 0)
                {
                    WarehouseStockOrderDetailLog objlog = db.WarehouseStockOrderDetailLogs.Where(x => x.ID == logId).FirstOrDefault();
                    objlog.OrderStatus = OrderStatus;
                    objlog.Quantity = Quantity;
                    objlog.CreateDate = System.DateTime.Now;
                    objlog.CreateBy = PersonalDetailID;
                    objlog.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    objlog.DeviceID = "x";
                    objlog.DeviceType = "x";
                    db.SaveChanges();
                }
                else
                {
                    WarehouseStockOrderDetailLog TP = new WarehouseStockOrderDetailLog();
                    TP.WarehouseStockID = WarehouseStockID;
                    TP.Quantity = Quantity;
                    TP.CustomerOrderDetailID = CustomerOrderDetailID;
                    TP.OrderStatus = OrderStatus;
                    TP.CreateDate = System.DateTime.Now;
                    TP.CreateBy = PersonalDetailID;
                    TP.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    TP.DeviceID = "x";
                    TP.DeviceType = "x";
                    db1.WarehouseStockOrderDetailLogs.Add(TP);
                    db1.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[InsertWarehouseStockOrderDetailLog]", "Can't Update WarehouseStockOrderDetailLog" + Environment.NewLine + "[Exception : " + ex.Message + "], [InnerException : " + ex.InnerException.Message + "]");
            }
        }

        private long GetPersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            long UserLoginID = Convert.ToInt32(Session["ID"]);
            long PersonalDetailID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[CustomerOrderDetailAlignmentController][GetShopID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }

        //End


        //Parameter added by Zubair for MLM on 07-03-2018
        // decimal pTotalBusinessPoints
        private long UpdateCustomerOrder(long pCustomerOrderID, decimal pOrderAmount, decimal pPayableAmount, decimal pTotalBusinessPoints,decimal CashbackPoints, out decimal WalletRefundAmount)//, decimal pTotalOrderAmount) //Yashaswi 18-9-2018
        {
            WalletRefundAmount = 0;
            try
            {
                //EzeeloDBContext db1 = new EzeeloDBContext();
                ModelLayer.Models.CustomerOrder lCustomerOrder = db.CustomerOrders.Find(pCustomerOrderID);
                if (lCustomerOrder == null)
                {
                    return -1;
                }
                decimal MlmAmountUsed = lCustomerOrder.MLMAmountUsed.Value;
                //lCustomerOrder.TotalOrderAmount = pTotalOrderAmount;//Yashaswi 18-9-2018
                lCustomerOrder.OrderAmount = pOrderAmount;
                lCustomerOrder.PayableAmount = pPayableAmount;
                if (lCustomerOrder.CoupenAmount != null && lCustomerOrder.CoupenAmount > 0)
                {
                    lCustomerOrder.PayableAmount = lCustomerOrder.PayableAmount - (decimal)lCustomerOrder.CoupenAmount;
                }
                //Condition added by Zubair on 15-09-2017 for handling Wallet amount if used by customer
                decimal lEarnUsedAmount = Convert.ToDecimal(db.EarnDetails.Where(x => x.CustomerOrderID == pCustomerOrderID).Select(x => x.UsedAmount).FirstOrDefault());
                if (lEarnUsedAmount > 0)
                {
                    if ((lCustomerOrder.PayableAmount - (decimal)lEarnUsedAmount) > 0)
                    {
                        lCustomerOrder.PayableAmount = lCustomerOrder.PayableAmount - (decimal)lEarnUsedAmount;
                    }
                }
                //End by Zubair

                //Added by Zubair for MLM on 07-03-2018
                lCustomerOrder.BusinessPointsTotal = pTotalBusinessPoints;
                if (lCustomerOrder.PayableAmount < lCustomerOrder.MLMAmountUsed)
                {
                    WalletRefundAmount = lCustomerOrder.MLMAmountUsed.Value - lCustomerOrder.PayableAmount;
                    lCustomerOrder.MLMAmountUsed = lCustomerOrder.PayableAmount;
                }

                if (lCustomerOrder.MLMAmountUsed > 0)
                {
                    lCustomerOrder.PayableAmount = lCustomerOrder.PayableAmount - Convert.ToDecimal(lCustomerOrder.MLMAmountUsed);
                }

                //End MLM
                lCustomerOrder.CashbackPointsTotal = CashbackPoints;
                lCustomerOrder.ModifyBy = customerCareSessionViewModel.PersonalDetailID;
                lCustomerOrder.ModifyDate = DateTime.Now;
                if(MlmAmountUsed == WalletRefundAmount)
                {
                    lCustomerOrder.MLMAmountUsed = MlmAmountUsed;
                }
                //db1.Entry(lCustomerOrder).State = EntityState.Modified;
                db.SaveChanges();
                return lCustomerOrder.ID;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[UpdateCustomerOrder]", "Can't Update CustomerOrder" + Environment.NewLine + "[Exception : " + ex.Message + "], [InnerException : " + ex.InnerException.Message + "]");
            }
        }
        private Boolean UpdateDeliveryOrderDetail(List<CustomerOrderDetail> pCustomerOrderDetail, decimal pDeliveryCharge, decimal pGandhibaghCharge)
        {
            try
            {
                //    EzeeloDBContext db1 = new EzeeloDBContext();
                List<CustomerOrderDetail> lCustomerOrderDetails = (from cod in pCustomerOrderDetail
                                                                   group cod by cod.ShopOrderCode into grps
                                                                   select new CustomerOrderDetail
                                                                   {
                                                                       ShopOrderCode = grps.Key,
                                                                       ShopID = grps.Where(x => x.ShopOrderCode == grps.Key).FirstOrDefault().ShopID,
                                                                       TotalAmount = grps.Where(x => x.ShopOrderCode == grps.Key).Sum(x => x.TotalAmount),
                                                                       IsActive = grps.Any(x => x.IsActive == true) ? true : false
                                                                   }).ToList();
                foreach (CustomerOrderDetail customerOrderDetail in lCustomerOrderDetails)
                {
                    DeliveryOrderDetail DOD = db.DeliveryOrderDetails.FirstOrDefault(x => x.ShopOrderCode == customerOrderDetail.ShopOrderCode);
                    DOD.OrderAmount = customerOrderDetail.TotalAmount;
                    DOD.DeliveryCharge = pDeliveryCharge;
                    DOD.GandhibaghCharge = pGandhibaghCharge;
                    int DPID = UpdateDeliveryPartner(DOD.ID, customerOrderDetail.ShopID);
                    if (DPID > 0)
                    {
                        DOD.DeliveryPartnerID = DPID;
                    }
                    DOD.IsActive = customerOrderDetail.IsActive;
                    DOD.ModifyBy = customerCareSessionViewModel.PersonalDetailID;
                    DOD.ModifyDate = DateTime.Now;
                    //db1.Entry(DOD).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[UpdateDeliveryOrderDetail]", "Can't update DeliveryOrderDetail" + Environment.NewLine + "[Exception : " + ex.Message + "], [InnerException : " + ex.InnerException.Message + "]");
            }
        }
        private Boolean InsertCustomerOrderHistory(long pCustomerOrderID, List<CustomerOrderDetail> pCustomerOrderDetail)
        {
            try
            {
                // EzeeloDBContext db1 = new EzeeloDBContext();
                foreach (CustomerOrderDetail customerOrderDetail in pCustomerOrderDetail)
                {
                    CustomerOrderHistory lCustomerOrderHistory = new CustomerOrderHistory();
                    lCustomerOrderHistory.CustomerOrderID = pCustomerOrderID;
                    lCustomerOrderHistory.ShopStockID = customerOrderDetail.ShopStockID;
                    lCustomerOrderHistory.Status = customerOrderDetail.OrderStatus;
                    lCustomerOrderHistory.CreateBy = customerCareSessionViewModel.PersonalDetailID;
                    lCustomerOrderHistory.CreateDate = DateTime.Now;
                    db.CustomerOrderHistories.Add(lCustomerOrderHistory);
                }
                db.SaveChanges();
                // db1.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[InsertCustomerOrderHistory]", "Can't insert CustomerOrderHistory" + Environment.NewLine + "[Exception : " + ex.Message + "], [InnerException : " + ex.InnerException.Message + "]");
            }
        }
        private long InsertUserDefinedLog(long pCustomerOrderID, string pDescription)
        {
            try
            {
                //  EzeeloDBContext db1 = new EzeeloDBContext();
                CustomerOrderUserDefinedLog lCustomerOrderUserDefinedLog = new CustomerOrderUserDefinedLog();
                lCustomerOrderUserDefinedLog.CustomerOrderID = pCustomerOrderID;
                lCustomerOrderUserDefinedLog.Description = pDescription.Trim();
                lCustomerOrderUserDefinedLog.IsActive = true;
                lCustomerOrderUserDefinedLog.CreateBy = customerCareSessionViewModel.PersonalDetailID;
                lCustomerOrderUserDefinedLog.CreateDate = DateTime.Now;
                lCustomerOrderUserDefinedLog.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                lCustomerOrderUserDefinedLog.DeviceType = "x";
                db.CustomerOrderUserDefinedLogs.Add(lCustomerOrderUserDefinedLog);
                db.SaveChanges();
                //  db1.Dispose();
                return lCustomerOrderUserDefinedLog.ID;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[InsertUpdateUserDefinedLog]", "Can't insert UserDefinedLog" + Environment.NewLine + "[Exception : " + ex.Message + "], [InnerException : " + ex.InnerException.Message + "]");
            }
        }

        public Boolean InsertUpdateTransactionInputOnOrderAlignment(long custOrderID)
        {
            // Result = 0;
            try
            {
                BusinessLogicLayer.ReadConfig readCon = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);
                con.Open();
                SqlCommand sqlComm = new SqlCommand("InsertUpdateTransactionInputOnOrderAlignment", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@lCustomerOrderID", custOrderID);
                sqlComm.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[InsertUpdateTransactionInputOnOrderAlignment]", "Can't Insert Update TransactionInput OnOrderAlignment" + Environment.NewLine + "[Exception : " + ex.Message + "], [InnerException : " + ex.InnerException.Message + "]");
            }
        }

        public int UpdateDeliveryPartner(long DeliveryOrderDetailID, long ShopID)
        {
            try
            {
                long MerchentId = ShopID; //GetShopID();

                DeliveryOrderDetail DOD = db.DeliveryOrderDetails.Find(DeliveryOrderDetailID);

                int lDeliveryPartnerId = DOD.DeliveryPartnerID;
                Boolean lIsShopHandleDeliveryProcess = Common.Common.IsShopHandleDeliveryProcess(MerchentId, ref lDeliveryPartnerId);

                if (lIsShopHandleDeliveryProcess == false)
                {
                    decimal DeliveryCharges = Common.Common.GetDeliveryCharges(lDeliveryPartnerId, Convert.ToDecimal(0), DOD.DeliveryType, DOD.Weight);
                    DOD.DeliveryPartnerID = lDeliveryPartnerId;
                    DOD.DeliveryCharge = DeliveryCharges;
                }
                return lDeliveryPartnerId;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[UpdateDeliveryPartner]", "Can't Update DeliveryPartner" + Environment.NewLine + "[Exception : " + ex.Message + "], [InnerException : " + ex.InnerException.Message + "]");
            }
        }

        #endregion
    }
}