//-----------------------------------------------------------------------
// <copyright file="CustomerOrder.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.ModelBinding;
using System.Web.Script.Serialization;
//using System.Globalization;
/*
 Handed over on 15-09-2015(9:45 AM to 12:00 AM) to Ajit, AVi, Mohit, Manoj
 */
namespace BusinessLogicLayer
{
    public class ShopStockViewModel
    {
        public long ID { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class CustomerOrder : CustomerManagement
    {

        /// <summary>
        /// Base class Constructor
        /// </summary>
        /// <param name="server">System.Web.HttpContext.Current.Server</param>
        public CustomerOrder(System.Web.HttpServerUtility server)
            : base(server)
        {
        }

        /// <summary>
        /// Declare the object of DbContextClass to interact with the database 
        /// </summary>
        private EzeeloDBContext db = new EzeeloDBContext();


        /// <summary>
        /// This method place the customer order i.e Saves entry in  CustomerOrder and order transactions in CustomerOrderDetails. 
        /// </summary>
        /// <param name="lOrder">Object of type OrderViewModel</param>
        /// <returns>Customer Order ID</returns>
        ///<exception cref="MyException">MyException</exception>
        public long PlaceCustomerOrder(OrderViewModel lOrder)
        {

            //Order ID to be returned
            long outOrderID = 0;


            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    /*Master order entry*/
                    lOrder.CustomerOrder.OrderCode = GetNextOrderCode();
                    lOrder.CustomerOrder.CreateBy = CommonFunctions.GetPersonalDetailsID(lOrder.CustomerOrder.UserLoginID);
                    lOrder.CustomerOrder.CreateDate = CommonFunctions.GetLocalTime();
                    lOrder.CustomerOrder.NetworkIP = CommonFunctions.GetClientIP();

                    db.CustomerOrders.Add(lOrder.CustomerOrder);
                    db.SaveChanges();
                    //@@Identity of newly inserted order entry.
                    long fOrderID = lOrder.CustomerOrder.ID;

                    /*Shop wise delivery charges in one order*/
                    List<ShopWiseDeliveryCharges> sDelCharges = lOrder.shopWiseDeliveryCharges;

                    Dictionary<long, string> dicMRCode = new Dictionary<long, string>();
                    /*Order details entry */
                    List<CustomerOrderDetail> CustomerOrderDetail = lOrder.CustomerOrderDetail;
                    CustomerOrderHistory lOrderHistory = new CustomerOrderHistory();
                    string lShopOrderCode = string.Empty;
                    /*New Additinal Code*/
                    string MROD_Code = string.Empty;

                    /*
                     Place Order Group by the Merchant Order 
                     */
                    var ShopList = from n in lOrder.CustomerOrderDetail
                                   group n by n.ShopID into newGroup
                                   orderby newGroup.Key
                                   select newGroup;

                    /*loop throughout shop code*/
                    foreach (var shopCode in ShopList)
                    {
                        long ID = Convert.ToInt64(shopCode.Key);
                        MROD_Code = GetNextShopOrderCode(ID, fOrderID, lShopOrderCode);
                        /*loop throught product in shop*/
                        foreach (var item in CustomerOrderDetail.Where(x => x.ShopID == ID).ToList())
                        /*end of new Additional Code*/
                        {
                            item.CustomerOrderID = fOrderID;

                            item.CreateBy = CommonFunctions.GetPersonalDetailsID(lOrder.CustomerOrder.UserLoginID);
                            item.CreateDate = CommonFunctions.GetLocalTime();
                            item.NetworkIP = CommonFunctions.GetClientIP();

                            //Add shop Order code in front of Shopwise delivery charges in List<ShopWiseDeliveryCharges>
                            var shopWiseDelivery = sDelCharges.Where(x => x.ShopID == item.ShopID).ToList();

                            if (shopWiseDelivery != null && shopWiseDelivery.Count() > 0)
                            {
                                //item.ShopOrderCode = shopWiseDelivery.FirstOrDefault().ShopOrderCode;
                                item.ShopOrderCode = MROD_Code;
                                //dicMRCode.Add(item.ShopID, MROD_Code);
                                sDelCharges.FirstOrDefault(x => x.ShopID == item.ShopID).ShopOrderCode = MROD_Code;

                            }

                            /*Check for is Shop order code is available; not empty */
                            if (item.ShopOrderCode.Length != 15)
                                throw new Exception("Invalid Shop order Code; Problem in generating Shop Order Code");

                            //Start- Added By yashaswi 04-02-2018 
                            //Add current shopstock and warehousestock qty
                            int SSQty = db.ShopStocks.FirstOrDefault(s => s.ID == item.ShopStockID).Qty;
                            int WSQty = db.WarehouseStocks.FirstOrDefault(s => s.ID == item.WarehouseStockID).AvailableQuantity;
                            SSQty = SSQty - item.Qty;
                            item.CurrentShopStockQty = SSQty;
                            item.CurrentWarehouseStockQty = WSQty;
                            //End- Added By yashaswi 04-02-2018 

                            db.CustomerOrderDetails.Add(item);
                            db.SaveChanges();
                            /* *********************************
                             * Taxation Management Start Changes
                             * Purpose :- Taxes On Purchase Product 
                             * Developed By : Pradnyakar Badge
                             * 29-03-2016
                             ***********************************/
                            //if (item.TaxOnOrders != null)
                            //{
                            //    List<TaxOnOrder> taxonOrder = new List<TaxOnOrder>();
                            //    taxonOrder = (from n in item.TaxOnOrders
                            //                  select new TaxOnOrder
                            //                  {
                            //                      CustomerOrderDetailID = item.ID,
                            //                      Amount = n.Amount,
                            //                      ProductTaxID = n.ProductTaxID,
                            //                      CreateBy = item.CreateBy,
                            //                      CreateDate = item.CreateDate,
                            //                      NetworkIP = item.NetworkIP,
                            //                      DeviceID = item.DeviceID,
                            //                      DeviceType = item.DeviceType
                            //                  }).ToList();
                            //    db.TaxOnOrders.AddRange(taxonOrder);
                            //    db.SaveChanges();
                            //}
                            /****************************************************/
                            /*********End OF Tax On Order Changes****************/
                            /****************************************************/
                            //================================================= SAVE TAXATION ==========================================================================
                            if (lOrder.lCalulatedTaxesRecord != null)
                            {
                                List<CalulatedTaxesRecord> lobj = lOrder.lCalulatedTaxesRecord.Where(x => x.ShopStockID == item.ShopStockID).ToList();
                                this.SaveTaxation(item.ID, item.Qty, lobj);
                            }

                            //stock less
                            ManageStock(item.ShopStockID, -item.Qty, item.WarehouseStockID, item.OrderStatus, item.ID); //last Parameter added by Zubair for Inventory on 28-03-2018

                            /*Maintain Customer Order History*/
                            lOrderHistory = new CustomerOrderHistory();
                            lOrderHistory.CreateBy = CommonFunctions.GetPersonalDetailsID(lOrder.CustomerOrder.UserLoginID);
                            lOrderHistory.CreateDate = CommonFunctions.GetLocalTime();
                            lOrderHistory.NetworkIP = CommonFunctions.GetClientIP();
                            lOrderHistory.IsActive = true;
                            lOrderHistory.CustomerOrderID = fOrderID;
                            lOrderHistory.ShopStockID = item.ShopStockID;
                            //-- Add by Ashish for Pending status from App --//
                            if (item.OrderStatus == 0)
                            { lOrderHistory.Status = (int)ModelLayer.Models.Enum.ORDER_STATUS.PENDING; }
                            else
                            // End //
                            { lOrderHistory.Status = (int)ModelLayer.Models.Enum.ORDER_STATUS.PLACED; }

                            db.CustomerOrderHistories.Add(lOrderHistory);
                            db.SaveChanges();                            
                        }
                        
                    }
                    if (db.MLMUsers.Any(p => p.UserID == lOrder.CustomerOrder.UserLoginID))
                    {
                        //Save Details in Booster Plan subscriber if present
                        BoosterPlanMaster planMaster = db.BoosterPlanMaster.FirstOrDefault(p => p.IsActive == true);
                        if (planMaster != null)
                        {
                            if (lOrder.IsBoostPlan)
                            {
                                BoosterPlanSubscriber planSubscriber = new BoosterPlanSubscriber();
                                planSubscriber.BoosterPlanMasterId = planMaster.ID;
                                planSubscriber.CustomerOrderId = fOrderID;
                                planSubscriber.IsActive = true;
                                planSubscriber.IsPaid = false;
                                planSubscriber.CreateBy = lOrder.CustomerOrder.UserLoginID;
                                planSubscriber.CreateDate = DateTime.Now;
                                planSubscriber.NetworkIP = CommonFunctions.GetClientIP();
                                planSubscriber.DeviceType = lOrder.CustomerOrder.DeviceType;
                                db.BoosterPlanSubscribers.Add(planSubscriber);
                                db.SaveChanges();

                                string CategoryName = db.Categories.FirstOrDefault(p => p.ID == planMaster.BoosterCategoryId).Name;
                                SensSMStoUplineForBoostPlan(lOrder.CustomerOrder.UserLoginID);
                            }
                        }
                    }

                    /*Delivery order details entry */
                    var lDeliveryPartner = db.DeliveryPartners.Where(x => x.ID == 1).ToList();
                    if (lDeliveryPartner.Count > 0)
                    {
                        /*Default Delivery Partner With ID = 1 Exists in database*/
                        foreach (var item in sDelCharges)
                        {
                            /*Saves the entry of delivery charges to be collected against each Shop Order*/
                            DeliveryOrderDetail lDelOrderDetail = new DeliveryOrderDetail();
                            lDelOrderDetail.CreateBy = CommonFunctions.GetPersonalDetailsID(lOrder.CustomerOrder.UserLoginID);
                            lDelOrderDetail.CreateDate = CommonFunctions.GetLocalTime();
                            lDelOrderDetail.DeliveryPartnerID = item.DeliveryPartnerID == 0 ? 1 : item.DeliveryPartnerID;
                            lDelOrderDetail.DeliveryCharge = item.DeliveryCharge;
                            lDelOrderDetail.DeliveryType = item.DeliveryType;
                            lDelOrderDetail.GandhibaghCharge = item.DeliveryCharge;
                            lDelOrderDetail.Weight = item.Weight;
                            lDelOrderDetail.IsActive = false;
                            lDelOrderDetail.IsMyPincode = true;
                            lDelOrderDetail.OrderAmount = item.OrderAmount;
                            lDelOrderDetail.NetworkIP = CommonFunctions.GetClientIP();

                            /*Generate Shop order Code for each (distinct) shop */
                            /*Added by Pradnyakar sir on 28-feb-2016
                             Purpose : To avoid extra entry in delivery order detail(For avoiding order place error)
                             */
                            //MROD_Code = GetNextShopOrderCode(item.ShopID, fOrderID, lShopOrderCode);
                            //if (db.CustomerOrderDetails.Where(x => x.ShopOrderCode == MROD_Code).Count() > 0)
                            if (db.CustomerOrderDetails.Count(x => x.ShopOrderCode == item.ShopOrderCode) > 0)
                            {
                                lDelOrderDetail.ShopOrderCode = item.ShopOrderCode;
                                //item.ShopOrderCode = lDelOrderDetail.ShopOrderCode;

                                db.DeliveryOrderDetails.Add(lDelOrderDetail);
                                db.SaveChanges();
                            }
                            //lShopOrderCode = lDelOrderDetail.ShopOrderCode;
                        }
                    }

                    /*Update redeemed voucher status*/
                    if (lOrder.CustomerOrder.CoupenCode != null && !(lOrder.CustomerOrder.CoupenCode.Trim().Equals(string.Empty)) && lOrder.CustomerOrder.CoupenAmount > 0)
                        RedeemVoucher(lOrder.CustomerOrder.CoupenCode, lOrder.CustomerOrder.UserLoginID, fOrderID, lOrder.CustomerOrder.CoupenAmount);

                    //RedeemVoucher(lOrder.CustomerOrder.CoupenCode, lOrder.CustomerOrder.CreateBy, fOrderID, lOrder.CustomerOrder.CoupenAmount);


                    var lPersonalDetail = db.PersonalDetails.Where(x => x.UserLoginID == lOrder.CustomerOrder.UserLoginID).FirstOrDefault();
                    /*To Save Save Schedule ID and Date
                     Changes By Pradnyakar Badge
                     * Dated 23-12-2015
                     * suggested by Sumit and Tejswee
                     */
                    int scheduleID = lOrder.ScheduleID == null ? 1 : Convert.ToInt32(lOrder.ScheduleID);
                    DateTime dt = lOrder.ScheduleDate == null ? DateTime.UtcNow.AddHours(5.30) : Convert.ToDateTime(lOrder.ScheduleDate);
                    this.SaveDeliveryScheduleDetail(lOrder.CustomerOrder.UserLoginID, fOrderID, scheduleID, dt);

                    this.SaveUsedEarnAmount(lOrder.lEarnDetail, fOrderID, lPersonalDetail.ID);

                    outOrderID = fOrderID;

                    //Added by Zubair for MLM on 09-04-2018
                    //Call OnOrderPlace method
                    MLMUser objMLMUser = db.MLMUsers.Where(x => x.UserID == lOrder.CustomerOrder.UserLoginID).FirstOrDefault();
                    if (objMLMUser != null && objMLMUser.UserID > 0)
                    {
                        if (outOrderID > 0)   // && lOrder.CustomerOrder.BusinessPointsTotal > 0 || Convert.ToDecimal(lOrder.CustomerOrder.MLMAmountUsed) > 0
                        {
                            MLMWalletPoints objMLMWalletPoints = new MLMWalletPoints();
                            object ret = objMLMWalletPoints.MLMWalletPostRequest(false,1, lOrder.CustomerOrder.UserLoginID, outOrderID, lOrder.CustomerOrder.BusinessPointsTotal, lOrder.CustomerOrder.PayableAmount, DateTime.UtcNow, Convert.ToDecimal(lOrder.CustomerOrder.MLMAmountUsed), lOrder.CustomerOrder.UserLoginID);
                        }
                    }
                    // Transaction complete
                    ts.Complete();
                }
                catch (Exception exception)
                {
                    Transaction.Current.Rollback();
                    BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Placing Customer Order :" + exception.Message + exception.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                    ts.Dispose();
                    outOrderID = 0;
                    //throw;
                }
            }
            try
            {
                this.InsertTransactionInput(outOrderID);
            }
            catch (Exception exception)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Placing Customer Order :" + exception.Message + exception.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }

            return outOrderID;
        }


        private void SaveTaxation(long CustOrderDetID, int qty, List<CalulatedTaxesRecord> lCalulatedTaxesRecord)
        {
            try
            {
                //==============Tejaswee for taxation ===========
                List<TaxOnOrder> prodTaxOnOrder = new List<TaxOnOrder>();
                TaxOnOrder lTaxOnOrder = new TaxOnOrder();
                //==============Tejaswee for taxation ===========
                //==============Tejaswee for taxation ===========
                foreach (var taxDetail in lCalulatedTaxesRecord)
                {
                    lTaxOnOrder.Amount = taxDetail.TaxableAmount * qty;
                    lTaxOnOrder.CustomerOrderDetailID = CustOrderDetID;
                    lTaxOnOrder.IsGSTInclusive = taxDetail.IsGSTInclusive; // Added by Zubair for GST on 06-07-2017
                    lTaxOnOrder.DeviceID = "X";
                    lTaxOnOrder.DeviceType = "X";
                    lTaxOnOrder.CreateDate = DateTime.UtcNow;
                    lTaxOnOrder.CreateBy = 1;// userLoginID;
                    lTaxOnOrder.ModifyBy = null;
                    lTaxOnOrder.ModifyDate = null;
                    lTaxOnOrder.NetworkIP = CommonFunctions.GetClientIP();
                    lTaxOnOrder.ProductTaxID = taxDetail.ProductTaxID;
                    // prodTaxOnOrder.Add(lTaxOnOrder);
                    db.TaxOnOrders.Add(lTaxOnOrder);
                    db.SaveChanges();
                }
                //==============Tejaswee for taxation ===========
            }
            catch (Exception)
            {

                throw;
            }
        }


        public long PlaceCorporateOrder(ShopProductVarientViewModelCollection lShoppingCartCollection, List<CorporateDetail> CorporateDetails, List<CorporateCustomerShippingAddressViewModel> corporateshippingAddress, List<CorporateFacilityDetailsViewModel> corporatefacility, long userLoginID)
        {
            long CustomerOrderId = 0;
            long CustomerOrderHistoryID = 0;
            int status = 0;
            //using (TransactionScope ts = new TransactionScope())
            //{
            try
            {
                ModelLayer.Models.CustomerOrder modelCustomerOrder = new ModelLayer.Models.CustomerOrder();
                int TotalQty = CorporateDetails[0].TotalQuantity;


                modelCustomerOrder.ID = 0;
                modelCustomerOrder.OrderCode = GetNextOrderCode();
                modelCustomerOrder.UserLoginID = userLoginID;
                modelCustomerOrder.OrderAmount = lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount;
                modelCustomerOrder.NoOfPointUsed = lShoppingCartCollection.lShoppingCartOrderDetails.NoOfPointUsed;
                modelCustomerOrder.ValuePerPoint = lShoppingCartCollection.lShoppingCartOrderDetails.ValuePerPoint;
                modelCustomerOrder.CoupenCode = lShoppingCartCollection.lShoppingCartOrderDetails.CoupenCode;
                modelCustomerOrder.CoupenAmount = lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount;
                modelCustomerOrder.PAN = string.Empty;
                modelCustomerOrder.PaymentMode = "ONLINE";
                modelCustomerOrder.PayableAmount = lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount;
                modelCustomerOrder.PrimaryMobile = CorporateDetails[0].FromPrimaryMob;
                modelCustomerOrder.SecondoryMobile = CorporateDetails[0].FromSecondaryMob;
                modelCustomerOrder.ShippingAddress = CorporateDetails[0].FromAddress;
                modelCustomerOrder.PincodeID = CorporateDetails[0].FromPincodeID;
                modelCustomerOrder.AreaID = CorporateDetails[0].AreaID;
                modelCustomerOrder.BusinessPointsTotal = lShoppingCartCollection.lShoppingCartOrderDetails.BusinessPointsTotal; //Added by Zubair for MLM on 06-01-2018
                modelCustomerOrder.MLMAmountUsed = lShoppingCartCollection.lShoppingCartOrderDetails.WalletAmountUsed;//Added by Zubair for MLM on 31-01-2018
                modelCustomerOrder.CreateDate = DateTime.UtcNow;
                modelCustomerOrder.CreateBy = userLoginID;
                modelCustomerOrder.ModifyDate = null;
                modelCustomerOrder.ModifyBy = null;
                modelCustomerOrder.NetworkIP = CommonFunctions.GetClientIP();
                modelCustomerOrder.DeviceType = "x";
                modelCustomerOrder.DeviceID = "x";
                db.CustomerOrders.Add(modelCustomerOrder);
                db.SaveChanges();
                CustomerOrderId = modelCustomerOrder.ID;
                //CustomerOrderCode = modelCustomerOrder.OrderCode;
                List<ShoppingCartViewModel> listShoppingCart = lShoppingCartCollection.lShopProductVarientViewModel;

                CustomerOrderDetail customerOrderDetail = new CustomerOrderDetail();
                string lastShopOrderCode = db.CustomerOrderDetails.OrderByDescending(x => x.ID).Select(x => x.ShopOrderCode).FirstOrDefault();
                if (CustomerOrderId > 0)
                {
                    foreach (var item in listShoppingCart)
                    {
                        customerOrderDetail.ID = 0;
                        customerOrderDetail.ShopOrderCode = GetNextShopOrderCode(customerOrderDetail.ShopID, CustomerOrderId, lastShopOrderCode);
                        customerOrderDetail.CustomerOrderID = CustomerOrderId;
                        customerOrderDetail.ShopStockID = item.ShopStockID;
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
                        customerOrderDetail.BusinessPointPerUnit = item.BusinessPointPerUnit;//Added by Zubair for MLM on 06-01-2018
                        customerOrderDetail.BusinessPoints = item.BusinessPointPerUnit * item.PurchaseQuantity; //Added by Zubair for MLM on 06-01-2018
                        customerOrderDetail.IsActive = true;
                        customerOrderDetail.CreateDate = DateTime.UtcNow;
                        customerOrderDetail.CreateBy = userLoginID;
                        customerOrderDetail.ModifyDate = null;
                        customerOrderDetail.ModifyBy = null;
                        customerOrderDetail.NetworkIP = CommonFunctions.GetClientIP();
                        customerOrderDetail.DeviceType = "x";
                        customerOrderDetail.DeviceID = "x";
                        db.CustomerOrderDetails.Add(customerOrderDetail);
                        db.SaveChanges();

                        //listOrderDetails.Add(customerOrderDetail);
                    }
                }
                long CustomerOrderDetailID = customerOrderDetail.ID;
                string ShopCode = customerOrderDetail.ShopOrderCode;

                if (CustomerOrderDetailID > 0)
                {
                    //=========================================================SAVE TAXATION================================================

                    if (corporatefacility != null)
                    {
                        corporatefacility[0].CustomerOrderDetailID = CustomerOrderDetailID;
                        CorporateGifting cg = new CorporateGifting(lShoppingCartCollection.lShopProductVarientViewModel[0].ProductID, userLoginID, CustomerOrderDetailID, TotalQty, corporateshippingAddress, corporatefacility);
                        status = cg.oprStatus;

                    }
                    else
                    {
                        CorporateGifting cg = new CorporateGifting(lShoppingCartCollection.lShopProductVarientViewModel[0].ProductID, userLoginID, CustomerOrderDetailID, TotalQty, corporateshippingAddress);
                        status = cg.oprStatus;
                    }


                    DeliveryOrderDetail lDeliveryOrderDetail = new DeliveryOrderDetail();
                    if (status == 1)
                    {
                        lDeliveryOrderDetail.DeliveryPartnerID = 1;
                        lDeliveryOrderDetail.ShopOrderCode = ShopCode;
                        lDeliveryOrderDetail.Weight = lShoppingCartCollection.lShopProductVarientViewModel[0].ActualWeight;
                        lDeliveryOrderDetail.OrderAmount = lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount;
                        lDeliveryOrderDetail.GandhibaghCharge = CorporateDetails[0].TotalDeliveryCharge;
                        lDeliveryOrderDetail.DeliveryCharge = 0;
                        lDeliveryOrderDetail.DeliveryType = "Normal";
                        lDeliveryOrderDetail.IsMyPincode = true;
                        lDeliveryOrderDetail.IsActive = false;
                        lDeliveryOrderDetail.CreateDate = DateTime.UtcNow;
                        lDeliveryOrderDetail.CreateBy = userLoginID;
                        lDeliveryOrderDetail.ModifyDate = null;
                        lDeliveryOrderDetail.ModifyBy = null;
                        lDeliveryOrderDetail.NetworkIP = CommonFunctions.GetClientIP();
                        lDeliveryOrderDetail.DeviceType = "Corporate Gifting";
                        lDeliveryOrderDetail.DeviceID = "x";
                        db.DeliveryOrderDetails.Add(lDeliveryOrderDetail);
                        db.SaveChanges();
                    }

                    long DeliveryOrderDetailID = lDeliveryOrderDetail.ID;
                    CustomerOrderHistory lCustomerOrderHistory = new CustomerOrderHistory();
                    if (DeliveryOrderDetailID > 0)
                    {
                        lCustomerOrderHistory.CustomerOrderID = CustomerOrderId;
                        lCustomerOrderHistory.ShopStockID = lShoppingCartCollection.lShopProductVarientViewModel[0].ShopStockID;
                        lCustomerOrderHistory.Status = 1;
                        lCustomerOrderHistory.IsActive = true;
                        lCustomerOrderHistory.CreateDate = DateTime.UtcNow;
                        lCustomerOrderHistory.CreateBy = userLoginID;
                        lCustomerOrderHistory.ModifyDate = null;
                        lCustomerOrderHistory.ModifyBy = null;
                        lCustomerOrderHistory.NetworkIP = CommonFunctions.GetClientIP();
                        lCustomerOrderHistory.DeviceType = "x";
                        lCustomerOrderHistory.DeviceID = "x";
                        db.CustomerOrderHistories.Add(lCustomerOrderHistory);
                        db.SaveChanges();

                    }
                    CustomerOrderHistoryID = lCustomerOrderHistory.ID;
                }
            }

            catch (Exception exception)
            {
                // Rollback transaction
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Placing Corporate gift Order :" + exception.Message + exception.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                //ts.Dispose();

            }
            // }
            if (CustomerOrderHistoryID > 0)
            {
                return CustomerOrderId;
            }
            else
            {
                return CustomerOrderHistoryID;
            }

        }
        /// <summary>
        /// Selecting first 10 records
        /// And Filtering by Index --- By Ashish 
        /// </summary>
        /// <param name="lCustLoginID"></param>
        /// <param name="Index"></param>
        /// <returns></returns>
        public ViewCustomerOrderViewModel GetCustomerOrders(long lCustLoginID, int Index, long? lCustomerOrderID = null)
        {
            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);//Added by Sonali for Api_30-10-2018
            ViewCustomerOrderViewModel lViewOrder = new ViewCustomerOrderViewModel();
            List<CustomerOrderViewModel> lOrderList = new List<CustomerOrderViewModel>();
            List<CustomerOrderDetailViewModel> lOrderProductsList = new List<CustomerOrderDetailViewModel>();
            if (lCustomerOrderID == null)
            {
                //get Master Customer Order
                lOrderList = (from co in db.CustomerOrders
                              join pd in db.PersonalDetails on co.UserLoginID equals pd.UserLoginID
                              join ed in db.EarnDetails on co.ID equals ed.CustomerOrderID into ed_temp
                              from edd in ed_temp.DefaultIfEmpty()
                              where co.UserLoginID == lCustLoginID
                              && co.ReferenceCustomerOrderID == null    //Added by Tejaswee

                              select new CustomerOrderViewModel
                              {
                                  CoupenAmount = co.CoupenAmount,
                                  CoupenCode = co.CoupenCode,
                                  CustomerOrderID = co.ID,
                                  NoOfPointUsed = co.NoOfPointUsed,
                                  OrderAmount = co.OrderAmount,
                                  OrderCode = co.OrderCode,
                                  PayableAmount = co.PayableAmount,
                                  PaymentMode = co.PaymentMode,
                                  ValuePerPoint = co.ValuePerPoint,
                                  IsCancellable = true,
                                  FullName = pd.FirstName + " " + pd.LastName,
                                  OrderDate = co.CreateDate,
                                  AreaID = co.AreaID,
                                  PincodeID = co.PincodeID,
                                  PrimaryMobile = co.PrimaryMobile,
                                  SecondoryMobile = co.SecondoryMobile,
                                  ShippingAddress = co.ShippingAddress,
                                  UsedWalletAnount = edd.UsedAmount,
                                  GSTNo = rcKey.GST_CONSTANT//Added by Sonali for Api_30-10-2018
                              }).OrderByDescending(x => x.CustomerOrderID).Skip(Index).Take(5).ToList();
                List<int> ordelist5 = new List<int>() { };
                foreach (var item in lOrderList)
                {
                    item.TotallDeliveryCharge = GetTotalOrderDeliveryCharges(item.CustomerOrderID);
                    item.EmailID = db.UserLogins.Find(lCustLoginID).Email;
                    item.Pincode = db.Pincodes.Find(item.PincodeID).Name;
                    item.CityName = db.Cities.Find(db.Pincodes.Find(item.PincodeID).CityID).Name;

                    ordelist5.Add((int)item.CustomerOrderID);
                }

                lViewOrder.Orders = lOrderList;

                //Get Customer order Details  

                lOrderProductsList = (from co in db.CustomerOrders
                                      join cod in db.CustomerOrderDetails on co.ID equals cod.CustomerOrderID
                                      join ss in db.ShopStocks on cod.ShopStockID equals ss.ID
                                      join unt in db.Units on ss.PackUnitID equals unt.ID
                                      join pv in db.ProductVarients on ss.ProductVarientID equals pv.ID
                                      join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                                      join prod in db.Products on sp.ProductID equals prod.ID
                                      join shop in db.Shops on sp.ShopID equals shop.ID
                                      join cl in db.Colors on pv.ColorID equals cl.ID
                                      join sz in db.Sizes on pv.SizeID equals sz.ID
                                      join dim in db.Dimensions on pv.DimensionID equals dim.ID
                                      join mat in db.Materials on pv.MaterialID equals mat.ID
                                      where co.UserLoginID == lCustLoginID
                                      && cod.IsActive == true    //Added by Tejaswee
                                      && ordelist5.Contains((int)cod.CustomerOrderID)////added for mobile for first 10 orderlist -Ashish
                                      select new CustomerOrderDetailViewModel
                                      {
                                          ColorCode = cl.HtmlCode,
                                          ColorName = cl.Name,
                                          CustomerOrderDetailID = cod.ID,
                                          CustomerOrderID = co.ID,
                                          DimensionName = dim.Name,
                                          IsInclusivOfTax = cod.IsInclusivOfTax,
                                          MaterialName = mat.Name,
                                          MRP = cod.MRP,
                                          SaleRate = cod.SaleRate,
                                          OfferPercent = cod.OfferPercent,
                                          OfferRs = cod.OfferRs,
                                          OrderStatus = cod.OrderStatus,
                                          PackSize = ss.PackSize,
                                          PackUnitName = unt.Name,
                                          SizeName = sz.Name,
                                          ProductID = prod.ID,
                                          ProductName = prod.Name,
                                          Qty = cod.Qty,
                                          ShopID = shop.ID,
                                          ShopName = shop.Name,
                                          ShopOrderCode = cod.ShopOrderCode,
                                          OrderCode = co.OrderCode,
                                          ShopStockID = cod.ShopStockID,
                                          TotalAmount = cod.TotalAmount,
                                          //Added by Tejaswee (19-jan-2016)
                                          StockStatus = ss.StockStatus,
                                          StockQty = ss.Qty,
                                          IsShopLive = shop.IsLive,
                                          IsProductLive = prod.IsActive
                                      }).ToList();

                foreach (var item in lOrderProductsList)
                {
                    /* Added By the Pradnyakar for Taxation Detail for Order History detail
                     * Dated :- 31-03-2016                 * 
                     */
                    if (db.TaxOnOrders.Where(x => x.CustomerOrderDetailID == item.CustomerOrderDetailID).Count() > 0)
                    {
                        item.TaxesOnProduct = (from n in db.TaxOnOrders
                                               join pt in db.ProductTaxes on n.ProductTaxID equals pt.ID
                                               join tm in db.TaxationMasters on pt.TaxID equals tm.ID
                                               where n.CustomerOrderDetailID == item.CustomerOrderDetailID
                                               select new CalulatedTaxesRecord
                                               {
                                                   ProductTaxID = n.ProductTaxID,
                                                   TaxableAmount = n.Amount,
                                                   ShopStockID = pt.ShopStockID,
                                                   TaxName = tm.Name,
                                                   TaxPrefix = tm.Prefix,
                                                   IsGSTInclusive = n.IsGSTInclusive //Added by Zubair for GST on 06-07-2017
                                               }).ToList();
                    }
                    /*End OF Taxation Detail By Pradnyakar*/


                    //Check Is Cancel button should be visible 
                    if (item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.CANCELLED || item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.DELIVERED ||
                        item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.RETURNED || item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.DISPATCHED_FROM_SHOP
                        || item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.PACKED || item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.IN_GODOWN ||
                        item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.DISPATCHED_FROM_GODOWN)
                    {
                        lOrderList.Where(x => x.CustomerOrderID == item.CustomerOrderID).FirstOrDefault().IsCancellable = false;
                    }
                    item.OrderStatusName = Enum.GetName(typeof(ModelLayer.Models.Enum.ORDER_STATUS), item.OrderStatus);

                    //Change by Harshada for image display
                    //If colorname is present then image will be called from the folder of that colr name otherwise image will be called from Default folder
                    //For time being Default is called,it may change according to condition.
                    //##

                    //Change by tejaswee (5-11-2015) 
                    //differnt function call for image display and if color name is present then image will display from color folder
                    if (item.ColorName == string.Empty || item.ColorName == "N/A")
                    {
                        //item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    }
                    else
                    {
                        //item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.ColorName, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    }
                }

                lViewOrder.OrderProducts = lOrderProductsList;
                return lViewOrder;
            }
            else
            {
                //----Added by Ashwini Meshram 14-Dec-2016
                //decimal Amount;
                //GCMMsgCustOrderDetail lViewOrder = new GCMMsgCustOrderDetail();
                //List<CustomerOrderDetailViewModel> lOrderProductsList = new List<CustomerOrderDetailViewModel>();
                //List<CustomerOrderViewModel> lCustomerOrderViewModel = new List<CustomerOrderViewModel>();
                DataTable lCustOrderdt = new DataTable();
                DataTable lCustOrderDetaildt = new DataTable();

                if (lCustomerOrderID == 0)
                {
                    lCustOrderdt = GetCustomerOrder(lCustLoginID, Index, lCustomerOrderID);
                    lViewOrder.Orders = BusinessLogicLayer.Helper.CreateListFromTable<CustomerOrderViewModel>(lCustOrderdt);


                    //code to seprate similar shop name
                    foreach (var item in lViewOrder.Orders)
                    {
                        string shopname = string.Empty;
                        string shp = string.Empty;

                        item.IsCancellable = true;
                        if (item.ShopName != null)
                        {
                            var shops = item.ShopName.Split(',').ToList();

                            for (int i = 0; i < shops.Count; i++)
                            {

                                if (shp != shops[i])
                                {
                                    shp = shops[i];
                                    if (shopname == "")
                                    {
                                        shopname = shp;
                                    }
                                    else
                                    {
                                        shopname = shopname + "," + shp;
                                    }
                                }

                            }
                            item.ShopName = shopname;
                        }

                    }
                    lViewOrder.Orders = lViewOrder.Orders.Skip(Index).Take(5).ToList();
                }
                else
                {

                    lCustOrderdt = GetCustomerOrder(lCustLoginID, Index, lCustomerOrderID);
                    lViewOrder.Orders = BusinessLogicLayer.Helper.CreateListFromTable<CustomerOrderViewModel>(lCustOrderdt);

                    //lViewOrder.Orders = lViewOrder.Orders.Skip(Index).Take(5).ToList();

                    lCustOrderDetaildt = GetCustomerOrderDetail(lCustomerOrderID);

                    foreach (var item in lViewOrder.Orders)
                    {
                        item.IsCancellable = true;
                        item.OrderProducts = BusinessLogicLayer.Helper.CreateListFromTable<CustomerOrderDetailViewModel>(lCustOrderDetaildt);

                        foreach (var items in item.OrderProducts)
                        {
                            /* Added By the Pradnyakar for Taxation Detail for Order History detail
                             * Dated :- 31-03-2016                 * 
                             */
                            if (db.TaxOnOrders.Where(x => x.CustomerOrderDetailID == items.CustomerOrderDetailID).Count() > 0)
                            {
                                items.TaxesOnProduct = (from n in db.TaxOnOrders
                                                        join pt in db.ProductTaxes on n.ProductTaxID equals pt.ID
                                                        join tm in db.TaxationMasters on pt.TaxID equals tm.ID
                                                        where n.CustomerOrderDetailID == items.CustomerOrderDetailID
                                                        select new CalulatedTaxesRecord
                                                        {
                                                            ProductTaxID = n.ProductTaxID,
                                                            TaxableAmount = n.Amount,
                                                            ShopStockID = pt.ShopStockID,
                                                            TaxName = tm.Name,
                                                            TaxPrefix = tm.Prefix,
                                                            IsGSTInclusive = n.IsGSTInclusive // Added by Zubair for GST on 06-07-2017
                                                        }).ToList();
                            }
                            /*End OF Taxation Detail By Pradnyakar*/


                            //Check Is Cancel button should be visible 
                            if (items.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.PACKED ||
                                items.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.DISPATCHED_FROM_SHOP ||
                                items.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.IN_GODOWN ||
                                items.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.DISPATCHED_FROM_GODOWN ||
                                items.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.DELIVERED ||
                                items.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.RETURNED ||
                                items.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.CANCELLED)
                            {
                                lViewOrder.Orders.Where(x => x.CustomerOrderID == item.CustomerOrderID).FirstOrDefault().IsCancellable = false;
                            }
                            items.OrderStatusName = Enum.GetName(typeof(ModelLayer.Models.Enum.ORDER_STATUS), items.OrderStatus);


                            //Change by Harshada for image display
                            //If colorname is present then image will be called from the folder of that colr name otherwise image will be called from Default folder
                            //For time being Default is called,it may change according to condition.
                            //##

                            //Change by tejaswee (5-11-2015) 
                            //differnt function call for image display and if color name is present then image will display from color folder
                            if (items.ColorName == string.Empty || items.ColorName == "N/A")
                            {
                                //item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                                items.StockThumbPath = ImageDisplay.SetProductThumbPath(items.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                            }
                            else
                            {
                                //item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                                items.StockThumbPath = ImageDisplay.SetProductThumbPath(items.ProductID, items.ColorName, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                            }


                        }
                    }
                }

                //lViewOrder.OrderProducts = BusinessLogicLayer.Helper.CreateListFromTable<CustomerOrderDetailViewModel>(lCustOrderDetaildt);                
                return lViewOrder;
            }


        }
        /// <summary>
        /// Selecting first 10 records
        /// And Filtering by Date --- By Ashish not in use now
        /// </summary>
        /// <param name="lCustLoginID"></param>
        /// <param name="FrmDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public ViewCustomerOrderViewModel GetCustomerOrders(long lCustLoginID, string FrmDate, string ToDate)
        {
            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);//Added by Sonali for Api_30-10-2018
            ViewCustomerOrderViewModel lViewOrder = new ViewCustomerOrderViewModel();
            List<CustomerOrderViewModel> lOrderList = new List<CustomerOrderViewModel>();
            List<CustomerOrderDetailViewModel> lOrderProductsList = new List<CustomerOrderDetailViewModel>();
            if (FrmDate == null && ToDate == null)
            {
                //get Master Customer Order

                lOrderList = (from co in db.CustomerOrders
                              join pd in db.PersonalDetails on co.UserLoginID equals pd.UserLoginID
                              join ed in db.EarnDetails on co.ID equals ed.CustomerOrderID into ed_temp
                              from edd in ed_temp.DefaultIfEmpty()
                              where co.UserLoginID == lCustLoginID
                              && co.ReferenceCustomerOrderID == null    //Added by Tejaswee

                              select new CustomerOrderViewModel
                              {
                                  CoupenAmount = co.CoupenAmount,
                                  CoupenCode = co.CoupenCode,
                                  CustomerOrderID = co.ID,
                                  NoOfPointUsed = co.NoOfPointUsed,
                                  OrderAmount = co.OrderAmount,
                                  OrderCode = co.OrderCode,
                                  PayableAmount = co.PayableAmount,
                                  PaymentMode = co.PaymentMode,
                                  ValuePerPoint = co.ValuePerPoint,
                                  IsCancellable = true,
                                  FullName = pd.FirstName + " " + pd.LastName,
                                  OrderDate = co.CreateDate,
                                  AreaID = co.AreaID,
                                  PincodeID = co.PincodeID,
                                  PrimaryMobile = co.PrimaryMobile,
                                  SecondoryMobile = co.SecondoryMobile,
                                  ShippingAddress = co.ShippingAddress,
                                  UsedWalletAnount = edd.UsedAmount,
                                  CashbackPoints = co.CashbackPointsTotal,
                                  GSTNo = rcKey.GST_CONSTANT//Added by Sonali for Api_30-10-2018
                              }).OrderByDescending(x => x.CustomerOrderID).Take(10).ToList();

            }
            else
            {
                DateTime FrmDate1 = FrmDate == null ? FrmDate1 = DateTime.MinValue : FrmDate1 = Convert.ToDateTime(FrmDate);
                DateTime ToDate1 = ToDate == null ? ToDate1 = DateTime.MinValue : ToDate1 = Convert.ToDateTime(ToDate);

                // DayOfWeek date2 = FrmDate.DayOfWeek;
                // int date3 = FrmDate.DayOfYear;
                //  DateTime date2 = ToDate.Date;

                lOrderList = (from co in db.CustomerOrders
                              join pd in db.PersonalDetails on co.UserLoginID equals pd.UserLoginID
                              where co.UserLoginID == lCustLoginID
                              && co.ReferenceCustomerOrderID == null    //Added by Tejaswee
                                                                        // && (co.CreateDate >= FrmDate && co.CreateDate <= ToDate)
                            && (EntityFunctions.TruncateTime(co.CreateDate) >= FrmDate1.Date && EntityFunctions.TruncateTime(co.CreateDate) <= ToDate1.Date)////added for mobile date filter -Ashish

                              select new CustomerOrderViewModel
                              {
                                  CoupenAmount = co.CoupenAmount,
                                  CoupenCode = co.CoupenCode,
                                  CustomerOrderID = co.ID,
                                  NoOfPointUsed = co.NoOfPointUsed,
                                  OrderAmount = co.OrderAmount,
                                  OrderCode = co.OrderCode,
                                  PayableAmount = co.PayableAmount,
                                  PaymentMode = co.PaymentMode,
                                  ValuePerPoint = co.ValuePerPoint,
                                  IsCancellable = true,
                                  FullName = pd.FirstName + " " + pd.LastName,
                                  OrderDate = co.CreateDate,
                                  AreaID = co.AreaID,
                                  PincodeID = co.PincodeID,
                                  PrimaryMobile = co.PrimaryMobile,
                                  SecondoryMobile = co.SecondoryMobile,
                                  ShippingAddress = co.ShippingAddress,
                                  CashbackPoints = co.CashbackPointsTotal
                              }).OrderByDescending(x => x.CustomerOrderID).ToList();

            }
            List<int> ordelist10 = new List<int>() { };
            foreach (var item in lOrderList)
            {
                item.TotallDeliveryCharge = GetTotalOrderDeliveryCharges(item.CustomerOrderID);
                item.EmailID = db.UserLogins.Find(lCustLoginID).Email;
                item.Pincode = db.Pincodes.Find(item.PincodeID).Name;
                item.CityName = db.Cities.Find(db.Pincodes.Find(item.PincodeID).CityID).Name;

                ordelist10.Add((int)item.CustomerOrderID);
            }

            lViewOrder.Orders = lOrderList;

            //Get Customer order Details            

            if (FrmDate == null && ToDate == null)
            {
                lOrderProductsList = (from co in db.CustomerOrders
                                      join cod in db.CustomerOrderDetails on co.ID equals cod.CustomerOrderID
                                      join ss in db.ShopStocks on cod.ShopStockID equals ss.ID
                                      join unt in db.Units on ss.PackUnitID equals unt.ID
                                      join pv in db.ProductVarients on ss.ProductVarientID equals pv.ID
                                      join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                                      join prod in db.Products on sp.ProductID equals prod.ID
                                      join shop in db.Shops on sp.ShopID equals shop.ID
                                      join cl in db.Colors on pv.ColorID equals cl.ID
                                      join sz in db.Sizes on pv.SizeID equals sz.ID
                                      join dim in db.Dimensions on pv.DimensionID equals dim.ID
                                      join mat in db.Materials on pv.MaterialID equals mat.ID
                                      where co.UserLoginID == lCustLoginID
                                      && cod.IsActive == true    //Added by Tejaswee
                                          && ordelist10.Contains((int)cod.CustomerOrderID)////added for mobile for first 10 orderlist -Ashish
                                      select new CustomerOrderDetailViewModel
                                      {
                                          ColorCode = cl.HtmlCode,
                                          ColorName = cl.Name,
                                          CustomerOrderDetailID = cod.ID,
                                          CustomerOrderID = co.ID,
                                          DimensionName = dim.Name,
                                          IsInclusivOfTax = cod.IsInclusivOfTax,
                                          MaterialName = mat.Name,
                                          MRP = cod.MRP,
                                          SaleRate = cod.SaleRate,
                                          OfferPercent = cod.OfferPercent,
                                          OfferRs = cod.OfferRs,
                                          OrderStatus = cod.OrderStatus,
                                          PackSize = ss.PackSize,
                                          PackUnitName = unt.Name,
                                          SizeName = sz.Name,
                                          ProductID = prod.ID,
                                          ProductName = prod.Name,
                                          Qty = cod.Qty,
                                          ShopID = shop.ID,
                                          ShopName = shop.Name,
                                          ShopOrderCode = cod.ShopOrderCode,
                                          OrderCode = co.OrderCode,
                                          ShopStockID = cod.ShopStockID,
                                          TotalAmount = cod.TotalAmount,
                                          //Added by Tejaswee (19-jan-2016)
                                          StockStatus = ss.StockStatus,
                                          StockQty = ss.Qty,
                                          IsShopLive = shop.IsLive,
                                          IsProductLive = prod.IsActive,
                                          CashbackPoint = cod.CashbackPoints,
                                          CashbackPointPerUnit = cod.CashbackPointPerUnit
                                      }).ToList();
            }
            else
            {
                DateTime FrmDate1 = FrmDate == null ? FrmDate1 = DateTime.MinValue : FrmDate1 = Convert.ToDateTime(FrmDate);
                DateTime ToDate1 = ToDate == null ? ToDate1 = DateTime.MinValue : ToDate1 = Convert.ToDateTime(ToDate);

                lOrderProductsList = (from co in db.CustomerOrders
                                      join cod in db.CustomerOrderDetails on co.ID equals cod.CustomerOrderID
                                      join ss in db.ShopStocks on cod.ShopStockID equals ss.ID
                                      join unt in db.Units on ss.PackUnitID equals unt.ID
                                      join pv in db.ProductVarients on ss.ProductVarientID equals pv.ID
                                      join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                                      join prod in db.Products on sp.ProductID equals prod.ID
                                      join shop in db.Shops on sp.ShopID equals shop.ID
                                      join cl in db.Colors on pv.ColorID equals cl.ID
                                      join sz in db.Sizes on pv.SizeID equals sz.ID
                                      join dim in db.Dimensions on pv.DimensionID equals dim.ID
                                      join mat in db.Materials on pv.MaterialID equals mat.ID
                                      where co.UserLoginID == lCustLoginID
                                      && cod.IsActive == true    //Added by Tejaswee
                                                                 // && (co.CreateDate >= FrmDate && co.CreateDate <= ToDate)
                                    && (EntityFunctions.TruncateTime(co.CreateDate) >= FrmDate1.Date && EntityFunctions.TruncateTime(co.CreateDate) <= ToDate1.Date)////added for mobile date filter -Ashish
                                      select new CustomerOrderDetailViewModel
                                      {
                                          ColorCode = cl.HtmlCode,
                                          ColorName = cl.Name,
                                          CustomerOrderDetailID = cod.ID,
                                          CustomerOrderID = co.ID,
                                          DimensionName = dim.Name,
                                          IsInclusivOfTax = cod.IsInclusivOfTax,
                                          MaterialName = mat.Name,
                                          MRP = cod.MRP,
                                          SaleRate = cod.SaleRate,
                                          OfferPercent = cod.OfferPercent,
                                          OfferRs = cod.OfferRs,
                                          OrderStatus = cod.OrderStatus,
                                          PackSize = ss.PackSize,
                                          PackUnitName = unt.Name,
                                          SizeName = sz.Name,
                                          ProductID = prod.ID,
                                          ProductName = prod.Name,
                                          Qty = cod.Qty,
                                          ShopID = shop.ID,
                                          ShopName = shop.Name,
                                          ShopOrderCode = cod.ShopOrderCode,
                                          OrderCode = co.OrderCode,
                                          ShopStockID = cod.ShopStockID,
                                          TotalAmount = cod.TotalAmount,
                                          //Added by Tejaswee (19-jan-2016)
                                          StockStatus = ss.StockStatus,
                                          StockQty = ss.Qty,
                                          IsShopLive = shop.IsLive,
                                          IsProductLive = prod.IsActive,
                                          CashbackPoint = cod.CashbackPoints,
                                          CashbackPointPerUnit = cod.CashbackPointPerUnit
                                      }).ToList();

            }

            foreach (var item in lOrderProductsList)
            {
                item.ProductName = item.ProductName.Replace("+", " ");//Added by Sonali on 13-02-2019
                /* Added By the Pradnyakar for Taxation Detail for Order History detail
                 * Dated :- 31-03-2016                 * 
                 */
                if (db.TaxOnOrders.Where(x => x.CustomerOrderDetailID == item.CustomerOrderDetailID).Count() > 0)
                {
                    item.TaxesOnProduct = (from n in db.TaxOnOrders
                                           join pt in db.ProductTaxes on n.ProductTaxID equals pt.ID
                                           join tm in db.TaxationMasters on pt.TaxID equals tm.ID
                                           where n.CustomerOrderDetailID == item.CustomerOrderDetailID
                                           select new CalulatedTaxesRecord
                                           {
                                               ProductTaxID = n.ProductTaxID,
                                               TaxableAmount = n.Amount,
                                               ShopStockID = pt.ShopStockID,
                                               TaxName = tm.Name,
                                               TaxPrefix = tm.Prefix,
                                               IsGSTInclusive = n.IsGSTInclusive  //Added by Zubair for GST on 06-07-2017
                                           }).ToList();
                }
                /*End OF Taxation Detail By Pradnyakar*/


                //Check Is Cancel button should be visible 
                if (item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.CANCELLED || item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.DELIVERED ||
                    item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.RETURNED || item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.DISPATCHED_FROM_SHOP
                    || item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.PACKED || item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.IN_GODOWN ||
                    item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.DISPATCHED_FROM_GODOWN)
                {
                    lOrderList.Where(x => x.CustomerOrderID == item.CustomerOrderID).FirstOrDefault().IsCancellable = false;
                }
                item.OrderStatusName = Enum.GetName(typeof(ModelLayer.Models.Enum.ORDER_STATUS), item.OrderStatus);

                //Change by Harshada for image display
                //If colorname is present then image will be called from the folder of that colr name otherwise image will be called from Default folder
                //For time being Default is called,it may change according to condition.
                //##

                //Change by tejaswee (5-11-2015) 
                //differnt function call for image display and if color name is present then image will display from color folder
                if (item.ColorName == string.Empty || item.ColorName == "N/A")
                {
                    //item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                    item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                }
                else
                {
                    //item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                    item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.ColorName, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                }


            }

            lViewOrder.OrderProducts = lOrderProductsList;
            return lViewOrder;


        }

        /// <summary>
        /// Get all Order's Details for Customer
        /// </summary>
        /// <param name="lCustLoginID">Customer Login ID</param>
        /// <returns>List of objects of type ViewCustomerOrdersViewModel</returns>
        public ViewCustomerOrderViewModel GetCustomerOrders(long lCustLoginID)
        {
            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);//Added by Sonali for Api_30/10/2018
            ViewCustomerOrderViewModel lViewOrder = new ViewCustomerOrderViewModel();
            List<CustomerOrderViewModel> lOrderList = new List<CustomerOrderViewModel>();
            List<CustomerOrderDetailViewModel> lOrderProductsList = new List<CustomerOrderDetailViewModel>();
            //get Master Customer Order
            lOrderList = (from co in db.CustomerOrders
                          join pd in db.PersonalDetails on co.UserLoginID equals pd.UserLoginID
                          join ed in db.EarnDetails on co.ID equals ed.CustomerOrderID into ed_temp
                          from edd in ed_temp.DefaultIfEmpty()
                          where co.UserLoginID == lCustLoginID
                          && co.ReferenceCustomerOrderID == null    //Added by Tejaswee
                          select new CustomerOrderViewModel
                          {
                              CoupenAmount = co.CoupenAmount,
                              CoupenCode = co.CoupenCode,
                              CustomerOrderID = co.ID,
                              NoOfPointUsed = co.NoOfPointUsed,
                              OrderAmount = co.OrderAmount,
                              OrderCode = co.OrderCode,
                              PayableAmount = co.PayableAmount,
                              PaymentMode = co.PaymentMode,
                              ValuePerPoint = co.ValuePerPoint,
                              IsCancellable = true,
                              FullName = pd.FirstName + " " + pd.LastName,
                              OrderDate = co.CreateDate,
                              AreaID = co.AreaID,
                              PincodeID = co.PincodeID,
                              PrimaryMobile = co.PrimaryMobile,
                              SecondoryMobile = co.SecondoryMobile,
                              ShippingAddress = co.ShippingAddress,
                              UsedWalletAnount = edd.UsedAmount,
                              // GSTNo = rcKey.GST_CONSTANT,//Added by Sonali for Api_30/10/2018
                              BusinessPointsTotal = co.BusinessPointsTotal, //Added by Sonali for Api_30/10/2018
                              CashbackPoints = co.CashbackPointsTotal,
                          }).OrderByDescending(x => x.CustomerOrderID).ToList();

            foreach (var item in lOrderList)
            {
                item.TotallDeliveryCharge = GetTotalOrderDeliveryCharges(item.CustomerOrderID);
                item.EmailID = db.UserLogins.Find(lCustLoginID).Email;
                item.Pincode = db.Pincodes.Find(item.PincodeID).Name;
                item.CityName = db.Cities.Find(db.Pincodes.Find(item.PincodeID).CityID).Name;
                int FranchiseId = db.FranchiseLocations.Where(x => x.IsActive && x.AreaID == item.AreaID).Select(x => x.FranchiseID ?? 0).FirstOrDefault();//Added by Sonali for Api on 31-12-2018
                item.GSTNo = db.WarehouseFranchises.Where(x => x.FranchiseID == FranchiseId && x.IsActive).Select(x => x.Warehouse.GSTNumber).FirstOrDefault();//Added by Sonali for Api on 31-12-2018
            }

            lViewOrder.Orders = lOrderList;

            //Get Customer order Details  

            lOrderProductsList = (from co in db.CustomerOrders
                                  join cod in db.CustomerOrderDetails on co.ID equals cod.CustomerOrderID
                                  join ss in db.ShopStocks on cod.ShopStockID equals ss.ID
                                  join unt in db.Units on ss.PackUnitID equals unt.ID
                                  join pv in db.ProductVarients on ss.ProductVarientID equals pv.ID
                                  join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                                  join prod in db.Products on sp.ProductID equals prod.ID
                                  join shop in db.Shops on sp.ShopID equals shop.ID
                                  join cl in db.Colors on pv.ColorID equals cl.ID
                                  join sz in db.Sizes on pv.SizeID equals sz.ID
                                  join dim in db.Dimensions on pv.DimensionID equals dim.ID
                                  join mat in db.Materials on pv.MaterialID equals mat.ID
                                  where co.UserLoginID == lCustLoginID
                                  && cod.IsActive == true    //Added by Tejaswee
                                  && cod.OrderStatus != 10  //Added by Sonali for not display ABANDONED Order Product on 04-12-2018
                                  && cod.OrderStatus != 0  //Added by Rumana for not display Order Product which is only saved in DB on 19/03/2019
                                  //|| cod.OrderStatus!=1 ) // added by amit
                                  select new CustomerOrderDetailViewModel
                                  {
                                      ColorCode = cl.HtmlCode,
                                      ColorName = cl.Name,
                                      CustomerOrderDetailID = cod.ID,
                                      CustomerOrderID = co.ID,
                                      DimensionName = dim.Name,
                                      IsInclusivOfTax = cod.IsInclusivOfTax,
                                      MaterialName = mat.Name,
                                      MRP = cod.MRP,
                                      SaleRate = cod.SaleRate,
                                      OfferPercent = cod.OfferPercent,
                                      OfferRs = cod.OfferRs,
                                      OrderStatus = cod.OrderStatus,
                                      PackSize = ss.PackSize,
                                      PackUnitName = unt.Name,
                                      SizeName = sz.Name,
                                      ProductID = prod.ID,
                                      ProductName = prod.Name,
                                      Qty = cod.Qty,
                                      ShopID = shop.ID,
                                      ShopName = shop.Name,
                                      ShopOrderCode = cod.ShopOrderCode,
                                      OrderCode = co.OrderCode,
                                      ShopStockID = cod.ShopStockID,
                                      TotalAmount = cod.TotalAmount,
                                      //Added by Tejaswee (19-jan-2016)
                                      StockStatus = ss.StockStatus,
                                      StockQty = ss.Qty,
                                      IsShopLive = shop.IsLive,
                                      IsProductLive = prod.IsActive,
                                      BusinessPointPerUnit = cod.BusinessPointPerUnit == null ? 0 : cod.BusinessPointPerUnit,//Added by Sonali for Api on 31-10-2018
                                      BusinessPoints = cod.BusinessPoints == null ? 0 : cod.BusinessPoints, //Added by Zubair for Sonali on 31-10-2018
                                      CashbackPoint= cod.CashbackPoints,
                                      CashbackPointPerUnit = cod.CashbackPointPerUnit
                                  }).ToList();

            foreach (var item in lOrderProductsList)
            {
                item.ProductName = item.ProductName.Replace("+", " ");//Added by Sonali on 13-02-2019
                /* Added By the Pradnyakar for Taxation Detail for Order History detail
                 * Dated :- 31-03-2016                 * 
                 */
                if (db.TaxOnOrders.Where(x => x.CustomerOrderDetailID == item.CustomerOrderDetailID).Count() > 0)
                {
                    item.TaxesOnProduct = (from n in db.TaxOnOrders
                                           join pt in db.ProductTaxes on n.ProductTaxID equals pt.ID
                                           join tm in db.TaxationMasters on pt.TaxID equals tm.ID
                                           where n.CustomerOrderDetailID == item.CustomerOrderDetailID
                                           select new CalulatedTaxesRecord
                                           {
                                               ProductTaxID = n.ProductTaxID,
                                               TaxableAmount = n.Amount,
                                               ShopStockID = pt.ShopStockID,
                                               TaxName = tm.Name,
                                               TaxPrefix = tm.Prefix,
                                               IsGSTInclusive = n.IsGSTInclusive //Added by Zubair for GST on 06-07-2017
                                           }).ToList();
                }
                /*End OF Taxation Detail By Pradnyakar*/


                //Check Is Cancel button should be visible 
                if (item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.CANCELLED || item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.DELIVERED ||
                    item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.RETURNED || item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.DISPATCHED_FROM_SHOP
                    || item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.PACKED || item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.IN_GODOWN ||
                    item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.DISPATCHED_FROM_GODOWN)
                {
                    lOrderList.Where(x => x.CustomerOrderID == item.CustomerOrderID).FirstOrDefault().IsCancellable = false;
                }
                item.OrderStatusName = Enum.GetName(typeof(ModelLayer.Models.Enum.ORDER_STATUS), item.OrderStatus);

                //Change by Harshada for image display
                //If colorname is present then image will be called from the folder of that colr name otherwise image will be called from Default folder
                //For time being Default is called,it may change according to condition.
                //##

                //Change by tejaswee (5-11-2015) 
                //differnt function call for image display and if color name is present then image will display from color folder
                if (item.ColorName == string.Empty || item.ColorName == "N/A")
                {
                    //item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                    item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                }
                else
                {
                    //item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                    item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.ColorName, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                }


            }

            lViewOrder.OrderProducts = lOrderProductsList;
            return lViewOrder;

        }
        // <summary>
        /// Get Order Details for Customer
        /// </summary>
        /// <param name="lCustLoginID">Customer Login ID</param>
        /// <param name="lOrderID">order ID</param>
        /// <returns>List of objects of type ViewCustomerOrdersViewModel</returns>
        public ViewCustomerOrderViewModel GetCustomerOrders(long lCustLoginID, long lOrderID)
        {
            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);//Sonali_30-10-2018
            ViewCustomerOrderViewModel lViewOrder = new ViewCustomerOrderViewModel();
            List<CustomerOrderViewModel> lOrderList = new List<CustomerOrderViewModel>();
            List<CustomerOrderDetailViewModel> lOrderProductsList = new List<CustomerOrderDetailViewModel>();

            //get Master Customer Order
            lOrderList = (from co in db.CustomerOrders
                          join pd in db.PersonalDetails on co.UserLoginID equals pd.UserLoginID
                          join ed in db.EarnDetails on co.ID equals ed.CustomerOrderID into ed_temp
                          from edd in ed_temp.DefaultIfEmpty()
                          where co.UserLoginID == lCustLoginID && co.ID == lOrderID
                          && co.ReferenceCustomerOrderID == null  //Added by Tejaswee
                          select new CustomerOrderViewModel
                          {
                              CoupenAmount = co.CoupenAmount,
                              CoupenCode = co.CoupenCode,
                              CustomerOrderID = co.ID,
                              NoOfPointUsed = co.NoOfPointUsed,
                              OrderAmount = co.OrderAmount,
                              OrderCode = co.OrderCode,
                              PayableAmount = co.PayableAmount,
                              PaymentMode = co.PaymentMode,
                              ValuePerPoint = co.ValuePerPoint,
                              IsCancellable = true,
                              AreaName = co.Area.Name,
                              AreaID = co.AreaID,
                              PincodeID = co.PincodeID,
                              PrimaryMobile = co.PrimaryMobile,
                              SecondoryMobile = co.SecondoryMobile,
                              ShippingAddress = co.ShippingAddress,
                              OrderDate = co.CreateDate,
                              FullName = pd.FirstName + " " + pd.LastName,
                              UsedWalletAnount = edd.UsedAmount,
                              BusinessPointsTotal = co.BusinessPointsTotal, //Added by Zubair for MLM on 06-01-2018
                              MLMAmountUsed = co.MLMAmountUsed == null ? 0 : co.MLMAmountUsed, //Added by Zubair for MLM on 31-01-2018
                              //GSTNo = rcKey.GST_CONSTANT//Added by Sonali for Api on 30-10-2018
                              CashbackPoints = co.CashbackPointsTotal
                          }).OrderByDescending(x => x.CustomerOrderID).ToList();
            if (lOrderList.Count > 0)
            {
                lOrderList.First().Pincode = db.Pincodes.Find(lOrderList.First().PincodeID).Name;

                foreach (var item in lOrderList)
                {
                    item.TotallDeliveryCharge = GetTotalOrderDeliveryCharges(item.CustomerOrderID);
                    item.EmailID = db.UserLogins.Find(lCustLoginID).Email;
                    item.Pincode = db.Pincodes.Find(item.PincodeID).Name;
                    item.CityName = db.Cities.Find(db.Pincodes.Find(item.PincodeID).CityID).Name;
                    int FranchiseId = db.FranchiseLocations.Where(x => x.IsActive && x.AreaID == item.AreaID).Select(x => x.FranchiseID ?? 0).FirstOrDefault();//Added by Sonali for Api on 31-12-2018
                    item.GSTNo = db.WarehouseFranchises.Where(x => x.FranchiseID == FranchiseId && x.IsActive).Select(x => x.Warehouse.GSTNumber).FirstOrDefault();//Added by Sonali for Api on 31-12-2018
                    //item.AreaName = string.Empty;
                }
                lViewOrder.Orders = lOrderList;

            }
            //Get Customer order Details             
            lOrderProductsList = (from cod in db.CustomerOrderDetails
                                  join ss in db.ShopStocks on cod.ShopStockID equals ss.ID
                                  join unt in db.Units on ss.PackUnitID equals unt.ID
                                  join pv in db.ProductVarients on ss.ProductVarientID equals pv.ID
                                  join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                                  join prod in db.Products on sp.ProductID equals prod.ID
                                  join shop in db.Shops on sp.ShopID equals shop.ID
                                  join cl in db.Colors on pv.ColorID equals cl.ID
                                  join sz in db.Sizes on pv.SizeID equals sz.ID
                                  join dim in db.Dimensions on pv.DimensionID equals dim.ID
                                  join mat in db.Materials on pv.MaterialID equals mat.ID
                                  where cod.CustomerOrderID == lOrderID
                                  && cod.IsActive == true   //Added by Tejaswee

                                  select new CustomerOrderDetailViewModel
                                  {
                                      ColorCode = cl.HtmlCode,
                                      ColorName = cl.Name,
                                      CustomerOrderDetailID = cod.ID,
                                      CustomerOrderID = cod.CustomerOrderID,
                                      DimensionName = dim.Name,
                                      IsInclusivOfTax = cod.IsInclusivOfTax,
                                      MaterialName = mat.Name,
                                      MRP = cod.MRP,
                                      SaleRate = cod.SaleRate,
                                      OfferPercent = cod.OfferPercent,
                                      OfferRs = cod.OfferRs,
                                      OrderStatus = cod.OrderStatus,
                                      PackSize = ss.PackSize,
                                      PackUnitName = unt.Name,
                                      ProductID = prod.ID,
                                      ProductName = prod.Name,
                                      HSNCode = prod.HSNCode, // Added by Zubairfor GST on 10-07-2017
                                      SizeName = sz.Name,
                                      Qty = cod.Qty,
                                      ShopID = shop.ID,
                                      ShopName = shop.Name,
                                      ShopOrderCode = cod.ShopOrderCode,
                                      ShopStockID = cod.ShopStockID,
                                      TotalAmount = cod.TotalAmount,
                                      BusinessPointPerUnit = cod.BusinessPointPerUnit == null ? 0 : cod.BusinessPointPerUnit,//Added by Zubair for MLM on 06-01-2018
                                      BusinessPoints = cod.BusinessPoints == null ? 0 : cod.BusinessPoints, //Added by Zubair for MLM on 06-01-2018
                                      CashbackPoint = cod.CashbackPoints,
                                      CashbackPointPerUnit = cod.CashbackPointPerUnit
                                  }).ToList();


            foreach (var item in lOrderProductsList)
            {
                item.ProductName = item.ProductName.Replace("+", " ");//Added by Sonali_13-02-2019
                /* Added By the Tejaswee for Taxation Detail for Order History detail
                 * Dated :- 11-04-2016                 * 
                 */

                //Comments removed by Zubair for GST on 11-07-2017
                if (db.TaxOnOrders.Where(x => x.CustomerOrderDetailID == item.CustomerOrderDetailID).Count() > 0)
                {
                    item.TaxesOnProduct = (from n in db.TaxOnOrders
                                           join pt in db.ProductTaxes on n.ProductTaxID equals pt.ID
                                           join tm in db.TaxationMasters on pt.TaxID equals tm.ID
                                           where n.CustomerOrderDetailID == item.CustomerOrderDetailID
                                           select new CalulatedTaxesRecord
                                           {
                                               ProductTaxID = n.ProductTaxID,
                                               TaxableAmount = n.Amount,
                                               ShopStockID = pt.ShopStockID,
                                               TaxName = tm.Name,
                                               TaxPrefix = tm.Prefix,
                                               IsGSTInclusive = n.IsGSTInclusive //Added by Zubair for GST on 11-07-2017
                                           }).ToList();
                }
                //End GST
                /*End OF Taxation Detail By Pradnyakar*/



                if (item.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.CANCELLED)
                {
                    lOrderList.Where(x => x.CustomerOrderID == item.CustomerOrderID).FirstOrDefault().IsCancellable = false;
                }
                item.OrderStatusName = Enum.GetName(typeof(ModelLayer.Models.Enum.ORDER_STATUS), item.OrderStatus);
                //Change by Harshada for image display
                //If colorname is present then image will be called from the folder of that colr name otherwise image will be called from Default folder
                //For time being Default is called,it may change according to condition.
                //##

                //Change by tejaswee (5-11-2015) 
                //differnt function call for image display and if color name is present then image will display from color folder
                if (item.ColorName == string.Empty || item.ColorName == "N/A")
                {
                    //item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                    item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                }
                else
                {
                    //item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                    item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.ColorName, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                }
            }

            lViewOrder.OrderProducts = lOrderProductsList;
            return lViewOrder;

        }

        /// <summary>
        /// Update customer order status from pending to Placed, when Online is successful from App 
        /// By Ashish.
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="lCustLoginID"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public int UpdatePendingCustomerOrder(long orderID, long lCustLoginID, string Description)
        {
            int oprStatus = 0;
            if (orderID > 0)
            {
                var lOrder = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == orderID).ToList();
                foreach (var item in lOrder)
                {
                    //Check if already Placed
                    if (db.CustomerOrderDetails.Where(x => x.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.PLACED
                      && x.CustomerOrderID == orderID && x.ShopStockID == item.ShopStockID).ToList().Count == 0)
                    {
                        CustomerOrderDetail lcod = db.CustomerOrderDetails.Find(item.ID);

                        lcod.OrderStatus = (int)ModelLayer.Models.Enum.ORDER_STATUS.PLACED;
                        lcod.ModifyBy = CommonFunctions.GetPersonalDetailsID(lCustLoginID);
                        lcod.ModifyDate = DateTime.UtcNow.AddHours(5.5);

                        db.Entry(lcod).State = EntityState.Modified;
                        db.SaveChanges();

                        //Commented by yashaswi, no need to check stock here, Stock already deduct when order placed with pending status
                        ////stock Add
                        //ShopStock shopStock = db.ShopStocks.Where(x => x.ID == lcod.ShopStockID && x.WarehouseStockID == lcod.WarehouseStockID).FirstOrDefault(); //Modified by Zubair for Inventory on 28-03-2018
                        //if (shopStock.Qty >= lcod.Qty)
                        //{
                        //    //ManageStock(lcod.ShopStockID, -lcod.Qty, lcod.WarehouseStockID, lcod.OrderStatus, lcod.ID); //last Parameter added by Zubair for Inventory on 28-03-2018
                        //}
                        //else
                        //{
                        //    oprStatus = 500;
                        //    return oprStatus;
                        //}

                        /*Maintain Customer Order History*/
                        CustomerOrderHistory lOrderHistory = new CustomerOrderHistory();
                        lOrderHistory = new CustomerOrderHistory();
                        lOrderHistory.CreateBy = CommonFunctions.GetPersonalDetailsID(lCustLoginID);
                        lOrderHistory.CreateDate = CommonFunctions.GetLocalTime();
                        lOrderHistory.NetworkIP = CommonFunctions.GetClientIP();
                        lOrderHistory.CustomerOrderID = item.CustomerOrderID;
                        lOrderHistory.ShopStockID = item.ShopStockID;
                        lOrderHistory.Status = (int)ModelLayer.Models.Enum.ORDER_STATUS.PLACED;

                        db.CustomerOrderHistories.Add(lOrderHistory);
                        db.SaveChanges();
                    }
                }
                if (lOrder.Count > 0)
                {
                    /*Add Descriptiopn to User Defined Log*/
                    CustomerOrderUserDefinedLog lCustomerOrderUserDefinedLog = new CustomerOrderUserDefinedLog();
                    lCustomerOrderUserDefinedLog.CustomerOrderID = lOrder.FirstOrDefault().CustomerOrderID;
                    lCustomerOrderUserDefinedLog.Description = Description.Trim();
                    lCustomerOrderUserDefinedLog.IsActive = true;
                    lCustomerOrderUserDefinedLog.CreateBy = CommonFunctions.GetPersonalDetailsID(lCustLoginID);
                    lCustomerOrderUserDefinedLog.CreateDate = CommonFunctions.GetLocalTime();
                    lCustomerOrderUserDefinedLog.NetworkIP = CommonFunctions.GetClientIP();
                    lCustomerOrderUserDefinedLog.DeviceType = "x";
                    db.CustomerOrderUserDefinedLogs.Add(lCustomerOrderUserDefinedLog);
                    db.SaveChanges();
                }
                oprStatus = 103;
            }
            return oprStatus;
        }
        /// <summary>
        /// Cancel order/ orderProduct. If customer wants to cancel complete order send 0 (zero) in  shopStockID
        /// </summary>
        /// <param name="orderID">OrderID</param>
        /// <param name="shopStockID">Stock ID</param>
        /// <param name="lCustLoginID">Customer Login ID</param>
        /// <returns>returns 103: Successfully Cancelled; 106: Record Nor exists </returns>
        public int CancelCustomerOrder(long orderID, long shopStockID, long lCustLoginID, string Description)
        {
            int oprStatus = 0;

            using (TransactionScope tscope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(60)))
            {
                try
                {
                    //cancel 1 product from Oder
                    if (shopStockID > 0)
                    {
                        var lOrder = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == orderID && x.ShopStockID == shopStockID).FirstOrDefault();
                        if (lOrder != null && lOrder.ID > 0)
                        {
                            CustomerOrderDetail lcod = db.CustomerOrderDetails.Find(lOrder.ID);
                            lcod.OrderStatus = (int)ModelLayer.Models.Enum.ORDER_STATUS.CANCELLED;
                            lcod.ModifyBy = CommonFunctions.GetPersonalDetailsID(lCustLoginID);
                            lcod.ModifyDate = DateTime.UtcNow.AddHours(5.5);

                            db.Entry(lcod).State = EntityState.Modified;
                            db.SaveChanges();
                            oprStatus = 103;

                            /*Maintain Customer Order History*/
                            CustomerOrderHistory lOrderHistory = new CustomerOrderHistory();
                            lOrderHistory = new CustomerOrderHistory();
                            lOrderHistory.CreateBy = CommonFunctions.GetPersonalDetailsID(lCustLoginID);
                            lOrderHistory.CreateDate = CommonFunctions.GetLocalTime();
                            lOrderHistory.NetworkIP = CommonFunctions.GetClientIP();
                            lOrderHistory.CustomerOrderID = orderID;
                            lOrderHistory.ShopStockID = shopStockID;
                            lOrderHistory.Status = (int)ModelLayer.Models.Enum.ORDER_STATUS.CANCELLED;

                            db.CustomerOrderHistories.Add(lOrderHistory);
                            db.SaveChanges();

                            /*Add Descriptiopn to User Defined Log*/
                            CustomerOrderUserDefinedLog lCustomerOrderUserDefinedLog = new CustomerOrderUserDefinedLog();
                            lCustomerOrderUserDefinedLog.CustomerOrderID = orderID;
                            lCustomerOrderUserDefinedLog.Description = Description.Trim();
                            lCustomerOrderUserDefinedLog.IsActive = true;
                            lCustomerOrderUserDefinedLog.CreateBy = CommonFunctions.GetPersonalDetailsID(lCustLoginID);
                            lCustomerOrderUserDefinedLog.CreateDate = CommonFunctions.GetLocalTime();
                            lCustomerOrderUserDefinedLog.NetworkIP = CommonFunctions.GetClientIP();
                            lCustomerOrderUserDefinedLog.DeviceType = "x";
                            db.CustomerOrderUserDefinedLogs.Add(lCustomerOrderUserDefinedLog);
                            db.SaveChanges();

                            ////Added by Zubair for MLM on 6-03-2018
                            ////Call OnOrderCancel

                            //var Order = db.CustomerOrders.Where(x => x.ID == orderID).FirstOrDefault();
                            //if (Convert.ToDecimal(Order.MLMAmountUsed) > 0 || lOrder.BusinessPoints > 0)
                            //{
                            //    MLMWalletPoints objMLMWalletPoints = new MLMWalletPoints();
                            //    object ret = objMLMWalletPoints.MLMWalletPostRequest(9, lCustLoginID, orderID, Order.BusinessPointsTotal, Order.PayableAmount, DateTime.UtcNow, Convert.ToDecimal(Order.MLMAmountUsed));
                            //}
                            ////End MLM

                        }
                        else
                            oprStatus = 106;
                    }
                    //Cancel Complete Order
                    else
                    {
                        var lOrder = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == orderID).ToList();
                        foreach (var item in lOrder)
                        {
                            //Check if already cancelled
                            if (db.CustomerOrderDetails.Where(x => x.OrderStatus == (int)ModelLayer.Models.Enum.ORDER_STATUS.CANCELLED
                              && x.CustomerOrderID == orderID && x.ShopStockID == item.ShopStockID).ToList().Count == 0)
                            {
                                CustomerOrderDetail lcod = db.CustomerOrderDetails.Find(item.ID);

                                lcod.OrderStatus = (int)ModelLayer.Models.Enum.ORDER_STATUS.CANCELLED;
                                lcod.ModifyBy = CommonFunctions.GetPersonalDetailsID(lCustLoginID);
                                lcod.ModifyDate = DateTime.UtcNow.AddHours(5.5);

                                db.Entry(lcod).State = EntityState.Modified;
                                db.SaveChanges();

                                //stock Add
                                ManageStock(lcod.ShopStockID, lcod.Qty, item.WarehouseStockID, item.OrderStatus, item.ID); //last Parameter added by Zubair for Inventory on 28-03-2018

                                /*Maintain Customer Order History*/
                                CustomerOrderHistory lOrderHistory = new CustomerOrderHistory();
                                lOrderHistory = new CustomerOrderHistory();
                                lOrderHistory.CreateBy = CommonFunctions.GetPersonalDetailsID(lCustLoginID);
                                lOrderHistory.CreateDate = CommonFunctions.GetLocalTime();
                                lOrderHistory.NetworkIP = CommonFunctions.GetClientIP();
                                lOrderHistory.CustomerOrderID = item.CustomerOrderID;
                                lOrderHistory.ShopStockID = item.ShopStockID;
                                lOrderHistory.Status = (int)ModelLayer.Models.Enum.ORDER_STATUS.CANCELLED;

                                db.CustomerOrderHistories.Add(lOrderHistory);
                                db.SaveChanges();
                            }
                        }
                        if (lOrder.Count > 0)
                        {
                            /*Add Descriptiopn to User Defined Log*/
                            CustomerOrderUserDefinedLog lCustomerOrderUserDefinedLog = new CustomerOrderUserDefinedLog();
                            lCustomerOrderUserDefinedLog.CustomerOrderID = lOrder.FirstOrDefault().CustomerOrderID;
                            lCustomerOrderUserDefinedLog.Description = Description.Trim();
                            lCustomerOrderUserDefinedLog.IsActive = true;
                            lCustomerOrderUserDefinedLog.CreateBy = CommonFunctions.GetPersonalDetailsID(lCustLoginID);
                            lCustomerOrderUserDefinedLog.CreateDate = CommonFunctions.GetLocalTime();
                            lCustomerOrderUserDefinedLog.NetworkIP = CommonFunctions.GetClientIP();
                            lCustomerOrderUserDefinedLog.DeviceType = "x";
                            db.CustomerOrderUserDefinedLogs.Add(lCustomerOrderUserDefinedLog);
                            db.SaveChanges();
                        }

                        BoosterPlanSubscriber planSubscriber = db.BoosterPlanSubscribers.FirstOrDefault(p => p.CustomerOrderId == orderID);
                        if (planSubscriber != null)
                        {
                            planSubscriber.ModifyBy = lCustLoginID;
                            planSubscriber.IsActive = false;
                            planSubscriber.ModifyDate = DateTime.Now;
                            planSubscriber.NetworkIP = CommonFunctions.GetClientIP();
                            db.SaveChanges();
                        }
                        oprStatus = 103;
                    }

                    //Added by Zubair for MLM on 6-03-2018
                    //Call OnOrderCancel
                    var Order = db.CustomerOrders.Where(x => x.ID == orderID).FirstOrDefault();
                    MLMUser objMLMUser = db.MLMUsers.Where(x => x.UserID == Order.UserLoginID).FirstOrDefault();
                    if (objMLMUser != null && objMLMUser.UserID > 0)
                    {
                        MLMWalletPoints objMLMWalletPoints = new MLMWalletPoints();
                        object ret = objMLMWalletPoints.MLMWalletPostRequest(false,9, Order.UserLoginID, orderID, Order.BusinessPointsTotal, Order.PayableAmount, DateTime.UtcNow, Convert.ToDecimal(Order.MLMAmountUsed), Order.UserLoginID);
                    }
                    //End MLM
                    tscope.Complete();
                }
                catch (Exception ex)
                {
                    Transaction.Current.Rollback();
                    tscope.Dispose();
                    throw ex;
                }
                return oprStatus;
            }
        }


        /// <summary>
        /// This method is used to update status of voucher is redeemed  for provided customerLogin ID
        /// </summary>
        /// <param name="voucherCode">Voucher Code</param>
        /// <param name="custLoginID">Customer Login ID</param>
        /// <returns>operation Status</returns>
        private int RedeemVoucher(string voucherCode, long custLoginID, long custOrderID, decimal? voucherAmt)
        {
            int oprStatus = 0;
            string query = string.Empty;

            //--------------------------

            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);
            con.Open();
            try
            {
                SqlParameter parm = new SqlParameter("@return", SqlDbType.Int);
                SqlCommand sqlComm = new SqlCommand("INSERTUPDATE_REDEEM_PROMO_VOUCHER", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                parm.Direction = ParameterDirection.ReturnValue;
                sqlComm.Parameters.AddWithValue("@CUST_ID", custLoginID);
                sqlComm.Parameters.AddWithValue("@VOUCHER_CODE", voucherCode);

                //=========================== added by Tejaswee =============================
                sqlComm.Parameters.AddWithValue("@CUST_ORDER_ID", custOrderID);
                //sqlComm.Parameters.AddWithValue("@VOUCHER_AMOUNT", voucherAmt);
                sqlComm.Parameters.AddWithValue("@VOUCHER_AMOUNT", voucherAmt);
                //=========================== added by Tejaswee =============================

                sqlComm.Parameters.AddWithValue("@IsRedeemed", 1);
                sqlComm.Parameters.AddWithValue("@Mode", "UPDATE");
                sqlComm.Parameters.Add("@QryResult", SqlDbType.Int, 4).Direction = ParameterDirection.Output;

                sqlComm.Parameters.Add(parm);
                sqlComm.ExecuteNonQuery();
                //Result = Convert.ToInt32(parm.Value);
                con.Close();
            }
            //--------------------------


            //ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            //SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);
            //con.Open();
            //query = "[INSERTUPDATE_REDEEM_PROMO_VOUCHER]";

            //SqlCommand cmd = new SqlCommand(query);
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Parameters.AddWithValue("@CUST_ID", custLoginID);
            //cmd.Parameters.AddWithValue("@VOUCHER_CODE", voucherCode);
            //cmd.Parameters.AddWithValue("@IsRedeemed", 1);
            //cmd.Parameters.AddWithValue("@Mode", "UPDATE");
            //cmd.Parameters.Add("@QryResult", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
            //try
            //{
            //    oprStatus = cmd.ExecuteNonQuery();
            //    oprStatus = 101;
            //    con.Close();
            //}
            catch (Exception ex)
            {
                oprStatus = 0;
                con.Close();
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in updating redeemed voucher status while placing order:" + ex.Message + ex.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

            }
            return oprStatus;

        }
        /// <summary>
        /// Get total delivery charges irrespective of shop
        /// </summary>
        /// <param name="lCustOrderID">Customer Order ID</param>
        /// <returns>Sum of delivery Charges against each shop</returns>
        private decimal GetTotalOrderDeliveryCharges(long lCustOrderID)
        {
            decimal TotallDeliveryCharge = 0;
            var OrderDeliveryAmount = (from cd in db.CustomerOrderDetails
                                       join dc in db.DeliveryOrderDetails on cd.ShopOrderCode equals dc.ShopOrderCode
                                       where cd.CustomerOrderID == lCustOrderID
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
                TotallDeliveryCharge = 0;
            return TotallDeliveryCharge;
        }
        /// <summary>
        /// Get Customer Order Code
        /// </summary>
        /// <returns>Order Code e.g. GBOD15061200000</returns>
        private string GetNextOrderCode()
        {
            //GBOD15061200000
            string newOrderCode = string.Empty;
            int lYear = 0;
            int lMonth = 0;
            int lDay = 0;
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Year.ToString(), out lYear);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Month.ToString(), out lMonth);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Day.ToString(), out lDay);
            string lOrderPrefix = "EZOD" + lYear.ToString().Substring(2, 2) + lMonth.ToString("00") + lDay.ToString("00");

            //- Changes Done by Avi Verma. Date: 16-Aug-2016.
            //- New Logic for GBOD by using Sequence of SQL SERVER.
            try
            {
                OrderManagement lOrderManagement = new OrderManagement();
                int lGBOD = lOrderManagement.GetNextGBOD();
                if (lGBOD > 0)
                {
                    newOrderCode = lOrderPrefix + lGBOD.ToString("00000");
                    return newOrderCode;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;

            ////- Changes Done by Avi Verma. Date: 16-Aug-2016
            ////- Old Logic. 

            //var lastOrder = db.CustomerOrders.OrderByDescending(x => x.ID).FirstOrDefault();
            //if (lastOrder != null)
            //{

            //    string OrderCode = lastOrder.OrderCode;
            //    if (OrderCode.Length == 15)
            //    {

            //        if (lOrderPrefix.Equals(OrderCode.ToString().Substring(0, 10)))
            //        {
            //            //Check if current date orders available
            //            //Add 1 to last order digit
            //            newOrderCode = lOrderPrefix + (Convert.ToInt32(OrderCode.ToString().Substring(10, 5)) + 1).ToString("00000");

            //        }
            //        else
            //            newOrderCode = lOrderPrefix + "00001";
            //    }
            //    else
            //    {

            //        //If order code length exceeds 15 digit
            //        var lastCurrentDateOrder = db.CustomerOrders.Where(x => x.OrderCode.StartsWith(lOrderPrefix)).OrderByDescending(x => x.ID).FirstOrDefault();
            //        if (lastCurrentDateOrder != null)
            //        {
            //            string CurrentDateOrderCode = lastCurrentDateOrder.OrderCode;
            //            if (CurrentDateOrderCode.Length == 15)
            //            {

            //                if (lOrderPrefix.Equals(CurrentDateOrderCode.ToString().Substring(0, 10)))
            //                {

            //                    newOrderCode = lOrderPrefix + (Convert.ToInt32(CurrentDateOrderCode.ToString().Substring(10, 5)) + 1).ToString("00000");

            //                }
            //            }

            //        }
            //        else
            //        {
            //            newOrderCode = lOrderPrefix + "00001";
            //        }
            //    }

            //}
            //else
            //{
            //    //FirstOrder
            //    newOrderCode = lOrderPrefix + "00001";

            //}


            //return newOrderCode;

        }
        /// <summary>
        /// Get Shop Order Code
        /// </summary>
        /// <param name="shopID">Shop ID</param>
        /// <param name="orderID">Order ID</param>
        /// <param name="lastShopOrderCode">Last Shop Order Code</param>
        /// <returns>Shop Order Code e.g. MROD15061200000 </returns>
        private string GetNextShopOrderCode(long shopID, long orderID, string lastShopOrderCode)
        {
            //MROD15061200000
            string newOrderCode = string.Empty;
            int lYear = 0;
            int lMonth = 0;
            int lDay = 0;
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Year.ToString(), out lYear);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Month.ToString(), out lMonth);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Day.ToString(), out lDay);
            string lOrderPrefix = "MROD" + lYear.ToString().Substring(2, 2) + lMonth.ToString("00") + lDay.ToString("00");

            //- Changes Done by Avi Verma. Date: 16-Aug-2016
            //- New Logic for MROD by using Sequence of SQL SERVER.
            try
            {
                OrderManagement lOrderManagement = new OrderManagement();
                int lMROD = lOrderManagement.GetNextMROD();
                if (lMROD > 0)
                {
                    newOrderCode = lOrderPrefix + lMROD.ToString("00000");
                    return newOrderCode;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;


            ////- Changes Done by Avi Verma. Date: 16-Aug-2016
            ////- Old Logic. 

            ////var lastOrder = db.CustomerOrderDetails.Where(x => x.ShopID == shopID).OrderByDescending(x => x.ID).FirstOrDefault();
            ////Get last Shop order code
            //var lastOrder = db.CustomerOrderDetails.OrderByDescending(x => x.ID).FirstOrDefault();
            //if (!string.IsNullOrEmpty(lastShopOrderCode))
            //{
            //    //Add 1 to last order
            //    return lOrderPrefix + (Convert.ToInt32(lastShopOrderCode.Substring(10, 5)) + 1).ToString("00000");
            //}

            //if (lastOrder != null)
            //{

            //    string OrderCode = lastOrder.ShopOrderCode;
            //    if (OrderCode.Length == 15)
            //    {
            //        // if shop order code is 15 digit long
            //        if (lOrderPrefix.Equals(OrderCode.ToString().Substring(0, 10)))
            //        {
            //            var subStringMatchingOrders = db.CustomerOrderDetails.Where(x => x.ShopOrderCode.StartsWith(lOrderPrefix)).Select(x => new { x.ShopOrderCode }).Distinct().ToList();

            //            if (subStringMatchingOrders != null)
            //            {

            //                var sortedList = subStringMatchingOrders.OrderByDescending(x => Convert.ToInt32(x.ShopOrderCode.Substring(10, 5))).ToList();
            //                newOrderCode = lOrderPrefix + (Convert.ToInt32(sortedList.First().ShopOrderCode.Substring(10, 5)) + 1).ToString("00000");
            //            }
            //            else
            //            {
            //                newOrderCode = lOrderPrefix + (Convert.ToInt32(OrderCode.ToString().Substring(10, 5)) + 1).ToString("00000");
            //            }
            //        }
            //        else
            //            newOrderCode = lOrderPrefix + "00001";
            //    }
            //    else
            //    {
            //        //Current date order not availlable 
            //        var lastCurrentDateOrder = db.CustomerOrderDetails.Where(x => x.ShopOrderCode.StartsWith(lOrderPrefix)).OrderByDescending(x => x.ID).FirstOrDefault();

            //        if (lastCurrentDateOrder != null)
            //        {
            //            string CurrentDateOrderCode = lastCurrentDateOrder.ShopOrderCode;

            //            if (!string.IsNullOrEmpty(lastShopOrderCode))
            //            {
            //                CurrentDateOrderCode = lastShopOrderCode;
            //            }

            //            if (CurrentDateOrderCode.Length == 15)
            //            {

            //                if (lOrderPrefix.Equals(CurrentDateOrderCode.ToString().Substring(0, 10)))
            //                {
            //                    //Add 1 to last order
            //                    newOrderCode = lOrderPrefix + (Convert.ToInt32(CurrentDateOrderCode.ToString().Substring(10, 5)) + 1).ToString("00000");

            //                }
            //            }

            //        }
            //        else
            //        {
            //            newOrderCode = lOrderPrefix + "00001";
            //        }
            //    }

            //}
            //else
            //{
            //    //FirstOrder
            //    newOrderCode = lOrderPrefix + "00001";

            //}
            //return newOrderCode;

        }

        /// <summary>
        /// This method is used to subtract qty when order placed  And Add  when order is getting Cancelled
        /// Send -ve qty when order placed and +ve Qty when Cancelled
        /// </summary>
        /// <param name="shopStockID">Shop Stock ID</param>
        /// <param name="qty">Purchased Qty</param>
        /// /// <param name="WarehouseStockID">Purchased WarehouseStockID</param>
        public void ManageStock(long shopStockID, int qty, long? WarehouseStockID, int orderStatus, long customerOrderDetailID) //Modified by Zubair for Inventory on 28-03-2018
        {
            //If product is linked with Warehouse then check WarehouseStockID
            //Added by Zubair for Inventory on 28-03-2018
            Nullable<long> warehouseStockID = null;
            if (WarehouseStockID != null && WarehouseStockID > 0)
            {
                warehouseStockID = Convert.ToInt64(WarehouseStockID);
            }
            //End

            //check stock row exists
            var stockRow = (from ss in db.ShopStocks
                            join SP in db.ShopProducts on ss.ShopProductID equals SP.ID
                            join S in db.Shops on SP.ShopID equals S.ID
                            where ss.ID == shopStockID && S.IsManageInventory == true && ss.WarehouseStockID == warehouseStockID//Modified by Zubair for Inventory on 28-03-2018
                            select new
                            {
                                Qty = ss.Qty
                            });

            if (stockRow != null && stockRow.Count() > 0)
            {
                //ShopStock shopStock = db.ShopStocks.Where(x => x.ID == shopStockID && x.Qty > 0).FirstOrDefault();
                /*Pradnyakar badge changes condition */
                ShopStock shopStock = db.ShopStocks.Where(x => x.ID == shopStockID && x.WarehouseStockID == warehouseStockID).FirstOrDefault(); //Modified by Zubair for Inventory on 28-03-2018
                                                                                                                                                //Condtion on qty=>when have to add qty to shopstockQty..If the shopStock.Qty + qty is greater than AvailableQty of warehouseStock Qty then no need to add qty to shopStock Qty, it should be less than warehouseQty.

                /*end of changes*/
                if (qty > 0 && orderStatus != 8)//Added by Rumana on 13/05/2019
                {
                    var wStock_AvailableQty = db.WarehouseStocks.Where(x => x.ID == warehouseStockID).Select(x => x.AvailableQuantity).FirstOrDefault();
                    var sStock_Qty = shopStock.Qty + qty;
                    if (sStock_Qty <= wStock_AvailableQty)
                    {
                        shopStock.Qty = shopStock.Qty + qty;
                        if (shopStock.Qty == 0)
                        {
                            shopStock.StockStatus = false;
                        }
                        else
                        {
                            shopStock.StockStatus = true;
                        }
                        db.SaveChanges();
                    }
                }
                else
                {
                    shopStock.Qty = shopStock.Qty + qty;
                    //if after updation it will zero, make status as Out Of Stock
                    if (shopStock.Qty == 0)
                    {
                        shopStock.StockStatus = false;
                    }
                    //if after updation it will not zero, make status as In Stock *** change by snehal on 25/02/2016
                    else
                    {
                        shopStock.StockStatus = true;
                    }
                    db.SaveChanges();
                }
                //Ended by Rumana on 13/05/2019
                //Commented by Rumana on 13/05/2019
                //shopStock.Qty = shopStock.Qty + qty;
                ////if after updation it will zero, make status as Out Of Stock
                //if (shopStock.Qty == 0)
                //{
                //    shopStock.StockStatus = false;
                //}
                ////if after updation it will not zero, make status as In Stock *** change by snehal on 25/02/2016
                //else
                //{
                //    shopStock.StockStatus = true;
                //}
                //db.SaveChanges();
                //Comment Ended by Rumana on 13/05/2019

                //Update ShopStockOrderDetailLog table for maintaing ShopStock inventory
                //Added on 07-04-2018
                int OrderQty = db.CustomerOrderDetails.Where(x => x.ID == customerOrderDetailID && x.ShopStockID == shopStockID).Select(x => x.Qty).FirstOrDefault();
                InsertShopStockOrderDetailLog(customerOrderDetailID, shopStockID, OrderQty, orderStatus);

                //Allot new batch
                if (shopStock.Qty == 0 && warehouseStockID > 0)
                {
                    SetNewBatchToShopStock(shopStockID, warehouseStockID);
                }
            }
            else
            {
                ShopStock Actvess = new ShopStock();
                WarehouseStock stock = db.WarehouseStocks.FirstOrDefault(p => p.ID == warehouseStockID);
                List<WarehouseStock> stockList = db.WarehouseStocks.Where(p => p.WarehouseID == stock.WarehouseID && p.ProductVarientID == stock.ProductVarientID).ToList();
                foreach(var item in stockList)
                {
                    ShopStock ss = db.ShopStocks.FirstOrDefault(p => p.WarehouseStockID == item.ID);
                    if (ss != null)
                    {
                        Actvess = ss;
                        break;
                    }
                }
                if(Actvess.Qty == 0)
                {
                    SetNewBatchToShopStock(Actvess.ID, warehouseStockID);
                }
            }
        }
        public void SetNewBatchToShopStock(long ShopStockID, long? WarehouseStockID)
        {
            try
            {
                //Update batch in ShopStock
                bool flag = true;
                long WarehouseID = db.WarehouseStocks.Where(x => x.ID == WarehouseStockID).Select(x => x.WarehouseID).FirstOrDefault();
                long productVarientID = db.ShopStocks.Where(x => x.ID == ShopStockID).Select(x => x.ProductVarientID).FirstOrDefault();

                //yashaswi 4/4/2018
                var productID_Query = from ss in db.ShopStocks
                                      join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                                      where ss.ID == ShopStockID
                                      select new { productID = sp.ProductID };

                if (productID_Query.Count() > 0)//yashaswi 4/4/2018
                {
                    long productID = productID_Query.Single().productID;//yashaswi 4/4/2018
                    //var usedBatchID = db.CustomerOrderDetails.Where(x => x.ShopStockID == ShopStockID).Select(x => x.WarehouseStockID).Distinct().ToList();

                    List<WarehouseStock> lStock = db.WarehouseStocks.Where(x => x.WarehouseID == WarehouseID && x.ProductID == productID
                                           && x.ProductVarientID == productVarientID && x.AvailableQuantity > 0).OrderBy(x => x.InvoiceID).ToList();

                    WarehouseStock wStock = new WarehouseStock();
                    int remainingQty = 0;
                    for (int i = 0; i < lStock.Count; i++)
                    {
                        int PlacedQty = 0;
                        int PendingQty = 0;
                        int reserveQty = 0;
                        long warehouseStockID = 0;
                        warehouseStockID = lStock[i].ID;
                        //var reserveQty = db.CustomerOrderDetails.Where(x => x.WarehouseStockID == warehouseStockID && (x.OrderStatus == 1
                        //    || x.OrderStatus == 2 || x.OrderStatus == 3 || x.OrderStatus == 4 || x.OrderStatus == 5 || x.OrderStatus == 6)).Select(x => x.Qty).DefaultIfEmpty(0).Sum();
                        PlacedQty = db.CustomerOrderDetails.Where(x => x.WarehouseStockID == warehouseStockID && (x.OrderStatus == 1
                           || x.OrderStatus == 2 || x.OrderStatus == 3 || x.OrderStatus == 4 || x.OrderStatus == 5 || x.OrderStatus == 6)).Select(x => x.Qty).DefaultIfEmpty(0).Sum();
                        PendingQty = db.CustomerOrderDetails.Where(x => x.WarehouseStockID == warehouseStockID && (x.OrderStatus == 0)).Select(x => x.Qty).DefaultIfEmpty(0).Sum();
                        reserveQty = PlacedQty + PendingQty;

                        if (reserveQty != null && reserveQty < lStock[i].AvailableQuantity)
                        {
                            remainingQty = lStock[i].AvailableQuantity - reserveQty;

                            wStock = db.WarehouseStocks.Where(x => x.WarehouseID == WarehouseID && x.ProductID == productID
                                           && x.ProductVarientID == productVarientID && x.AvailableQuantity > 0 &&
                                           x.ID == warehouseStockID).FirstOrDefault();
                            break;
                        }
                        //else
                        //{
                        //    wStock = db.WarehouseStocks.Where(x => x.WarehouseID == WarehouseID && x.ProductID == productID
                        //                  && x.ProductVarientID == productVarientID && x.AvailableQuantity > 0 &&
                        //                  x.ID == warehouseStockID).OrderBy(x => x.InvoiceID).FirstOrDefault();
                        //}
                    }

                    ///if quanitity is 0 then directly update quantity from invoice
                    ///


                    if (wStock != null && wStock.AvailableQuantity > 0)
                    {
                        ProductDetails prod = new ProductDetails(System.Web.HttpContext.Current.Server);
                        ShopStock objlog = db.ShopStocks.Where(x => x.ID == ShopStockID).FirstOrDefault();
                        if (remainingQty > 0)
                        {
                            objlog.Qty = remainingQty;
                        }
                        else
                        {
                            objlog.Qty = wStock.AvailableQuantity;
                        }
                        objlog.MRP = Convert.ToDecimal(wStock.MRP);
                        objlog.RetailerRate = wStock.SaleRatePerUnit;
                        objlog.WarehouseStockID = wStock.ID;
                        objlog.StockStatus = true;
                        objlog.ModifyDate = DateTime.Now;
                        objlog.ModifyBy = 1;
                        objlog.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        objlog.CashbackPoints =  prod.getCasbackPointsOnProductFromWarehouse(WarehouseStockID.Value);
                        objlog.BusinessPoints = wStock.BusinessPoints.Value;
                        objlog.DeviceID = "x";
                        objlog.DeviceType = "x";
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw;
            }
        }
        //public void SetNewBatchToShopStock(long ShopStockID, long? WarehouseStockID)
        //{
        //    try
        //    {
        //        //Update batch in ShopStock
        //        bool flag = true;
        //        long WarehouseID = db.WarehouseStocks.Where(x => x.ID == WarehouseStockID).Select(x => x.WarehouseID).FirstOrDefault();
        //        long productVarientID = db.ShopStocks.Where(x => x.ID == ShopStockID).Select(x => x.ProductVarientID).FirstOrDefault();

        //        //yashaswi 4/4/2018
        //        var productID_Query = from ss in db.ShopStocks
        //                              join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
        //                              where ss.ID == ShopStockID
        //                              select new { productID = sp.ProductID };

        //        if (productID_Query.Count() > 0)//yashaswi 4/4/2018
        //        {
        //            long productID = productID_Query.Single().productID;//yashaswi 4/4/2018
        //            //var usedBatchID = db.CustomerOrderDetails.Where(x => x.ShopStockID == ShopStockID).Select(x => x.WarehouseStockID).Distinct().ToList();

        //            List<WarehouseStock> lStock = db.WarehouseStocks.Where(x => x.WarehouseID == WarehouseID && x.ProductID == productID
        //                                   && x.ProductVarientID == productVarientID && x.AvailableQuantity > 0).OrderBy(x => x.InvoiceID).ToList();

        //            WarehouseStock wStock = new WarehouseStock();
        //            int remainingQty = 0;
        //            for (int i = 0; i < lStock.Count; i++)
        //            {
        //                long warehouseStockID = lStock[i].ID;
        //                var PlacedQty = db.CustomerOrderDetails.Where(x => x.WarehouseStockID == warehouseStockID && (x.OrderStatus == 1
        //                    || x.OrderStatus == 2 || x.OrderStatus == 3 || x.OrderStatus == 4 || x.OrderStatus == 5 || x.OrderStatus == 6)).Select(x => x.Qty).DefaultIfEmpty(0).Sum();
        //                var PendingQty = db.CustomerOrderDetails.Where(x => x.WarehouseStockID == warehouseStockID && (x.OrderStatus == 0)).Select(x => x.Qty).DefaultIfEmpty(0).Sum();
        //                var reserveQty = PlacedQty + PendingQty;
        //                if (reserveQty > 0 && reserveQty <= lStock[i].AvailableQuantity)
        //                {
        //                    remainingQty = lStock[i].AvailableQuantity - reserveQty;

        //                    wStock = db.WarehouseStocks.Where(x => x.WarehouseID == WarehouseID && x.ProductID == productID
        //                                   && x.ProductVarientID == productVarientID && x.AvailableQuantity > 0 &&
        //                                   x.ID == warehouseStockID).FirstOrDefault();
        //                    //break;
        //                    if (wStock != null && wStock.AvailableQuantity > 0)
        //                    {
        //                        ShopStock objlog = db.ShopStocks.Where(x => x.ID == ShopStockID).FirstOrDefault();
        //                        objlog.Qty = remainingQty;
        //                        objlog.MRP = Convert.ToDecimal(wStock.MRP);
        //                        objlog.RetailerRate = wStock.SaleRatePerUnit;
        //                        objlog.WarehouseStockID = wStock.ID;
        //                        objlog.StockStatus = true;
        //                        objlog.ModifyDate = DateTime.Now;
        //                        objlog.ModifyBy = 1;
        //                        objlog.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
        //                        objlog.DeviceID = "x";
        //                        objlog.DeviceType = "x";
        //                        db.SaveChanges();
        //                    }
        //                }
        //                //else
        //                //{
        //                //    wStock = db.WarehouseStocks.Where(x => x.WarehouseID == WarehouseID && x.ProductID == productID
        //                //                  && x.ProductVarientID == productVarientID && x.AvailableQuantity > 0 &&
        //                //                  x.ID == warehouseStockID).OrderBy(x => x.InvoiceID).FirstOrDefault();
        //                //}
        //            }

        //            ///if quanitity is 0 then directly update quantity from invoice
        //            ///



        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //Added by Zubair for Inventory on 4-4-2018
        //Manage ShopStock Log as per order
        private void InsertShopStockOrderDetailLog(long CustomerOrderDetailID, long ShopStockID, int Quantity, int OrderStatus)
        {
            try
            {
                var logId = db.ShopStockOrderDetailLogs.Where(x => x.ShopStockID == ShopStockID && x.CustomerOrderDetailID == CustomerOrderDetailID
                           && (x.OrderStatus != OrderStatus || x.Quantity != Quantity)).Select(x => x.ID).FirstOrDefault();

                if (logId != null && logId > 0)
                {
                    ShopStockOrderDetailLog objlog = db.ShopStockOrderDetailLogs.Where(x => x.ID == logId).FirstOrDefault();
                    objlog.OrderStatus = OrderStatus;
                    objlog.Quantity = Quantity;
                    objlog.LastModifyDate = CommonFunctions.GetLocalTime();
                    objlog.ModifyBy = 1;
                    objlog.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    objlog.DeviceID = "x";
                    objlog.DeviceType = "x";
                    db.SaveChanges();
                }
                else
                {
                    ShopStockOrderDetailLog TP = new ShopStockOrderDetailLog();
                    TP.ShopStockID = ShopStockID;
                    TP.Quantity = Quantity;
                    TP.CustomerOrderDetailID = CustomerOrderDetailID;
                    TP.OrderStatus = OrderStatus;
                    TP.LastModifyDate = CommonFunctions.GetLocalTime();
                    TP.ModifyBy = 1;
                    TP.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    TP.DeviceID = "x";
                    TP.DeviceType = "x";
                    db.ShopStockOrderDetailLogs.Add(TP);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[InsertShopStockOrderDetailLog]", "Can't Update InsertShopStockOrderDetailLog" + Environment.NewLine + "[Exception : " + ex.Message + "], [InnerException : " + ex.InnerException.Message + "]");
            }
        }




        //End Inventory part

        /*To Save Save Schedule ID and Date
        * Changes By Tejswee
        * Dated 23-12-2015
        * 
        */
        private void SaveDeliveryScheduleDetail(long UID, long customerOrderID, int deliveryScheduleID, DateTime deliveryScheduleDate)
        {
            try
            {

                OrderDeliveryScheduleDetail lOrderDeliveryScheduleDetail = new OrderDeliveryScheduleDetail();

                var lPersonalDetail = db.PersonalDetails.Where(x => x.UserLoginID == UID).FirstOrDefault();

                lOrderDeliveryScheduleDetail.CreateBy = lPersonalDetail.ID;
                lOrderDeliveryScheduleDetail.CreateDate = DateTime.UtcNow;
                lOrderDeliveryScheduleDetail.CustomerOrderID = customerOrderID;
                //lOrderDeliveryScheduleDetail.DeliveryScheduleID = deliveryScheduleID;
                lOrderDeliveryScheduleDetail.DeliveryScheduleID = Convert.ToInt64(deliveryScheduleID);//delivery schedule id
                lOrderDeliveryScheduleDetail.DeliveryDate = Convert.ToDateTime(deliveryScheduleDate);

                lOrderDeliveryScheduleDetail.IsActive = true;
                lOrderDeliveryScheduleDetail.ModifyDate = null;
                lOrderDeliveryScheduleDetail.ModifyBy = null;
                lOrderDeliveryScheduleDetail.NetworkIP = CommonFunctions.GetClientIP();
                lOrderDeliveryScheduleDetail.DeviceType = "x";
                lOrderDeliveryScheduleDetail.DeviceID = "x";

                db.OrderDeliveryScheduleDetails.Add(lOrderDeliveryScheduleDetail);
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<CalculatedTaxList> GetTaxDetails(long orderID)
        {
            try
            {
                //List<CalculatedTaxList> lCalculatedTaxList
                //List<CalculatedTaxList> lCalculatedTaxList = (from to in db.TaxOnOrders
                //              join cod in db.CustomerOrderDetails on to.CustomerOrderDetailID equals cod.ID
                //              where cod.CustomerOrderID == orderID 
                //              select new CalculatedTaxList { TaxName=to.ProductTax.TaxationMaster.Name,TaxPrefix=to.ProductTax.TaxationMaster.Prefix,
                //                  Amount=to.Amount
                //              }).ToList();


                //lCalculatedTaxList =(from row in lCalculatedTaxList
                //                    group row by new { row.TaxName } into g
                //                    select new CalculatedTaxList()
                //                    {
                //                        TaxName = g.Key.TaxName,
                //                        TaxPrefix=g.Select(x=>x.TaxPrefix).ToString(),
                //                        Amount = g.Sum(x => x.Amount)
                //                    }).ToList();


                List<CalculatedTaxList> lCalculatedTaxList = (from to in db.TaxOnOrders
                                                              join cod in db.CustomerOrderDetails on to.CustomerOrderDetailID equals cod.ID
                                                              where cod.CustomerOrderID == orderID
                                                              group new { to } by new
                                                              {
                                                                  to.ProductTax.TaxationMaster.Name,
                                                                  to.IsGSTInclusive
                                                              } into g
                                                              select new CalculatedTaxList()
                                                              {
                                                                  TaxName = g.Key.Name,
                                                                  TaxPrefix = g.Select(x => x.to.ProductTax.TaxationMaster.Prefix).FirstOrDefault(),
                                                                  Amount = g.Sum(x => x.to.Amount),
                                                                  IsGSTInclusive = g.Select(x => x.to.IsGSTInclusive).FirstOrDefault()  // Added by Zubair for GST on 06-07-2017
                                                              }).ToList();

                return lCalculatedTaxList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// This method save used amount use by customer from GB vallet while placing order
        /// </summary>
        public void SaveUsedEarnAmount(EarnDetail lEarnDetail, long custOrderID, long persID)
        {
            try
            {
                if (lEarnDetail != null)
                {
                    lEarnDetail.CreateBy = persID;
                    lEarnDetail.CreateDate = DateTime.UtcNow.AddHours(5.5);
                    lEarnDetail.CustomerOrderID = custOrderID;

                    lEarnDetail.IsActive = true;
                    lEarnDetail.ModifyDate = DateTime.UtcNow.AddHours(5.5);
                    lEarnDetail.ModifyBy = null;
                    lEarnDetail.NetworkIP = CommonFunctions.GetClientIP();
                    lEarnDetail.DeviceType = "x";
                    lEarnDetail.DeviceID = "x";

                    db.EarnDetails.Add(lEarnDetail);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="custOrderID"></param>
        public void InsertTransactionInput(long custOrderID)
        {
            // Result = 0;
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);
                con.Open();
                SqlCommand sqlComm = new SqlCommand("InsertUpdateTransactionInput", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@lCustomerOrderID", custOrderID);
                sqlComm.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception)
            {
                //  Result = 103; //Exception Found
                throw;
            }
            //return Result;
        }

        // Added by snehal: to add new products in customer order by crm
        public Boolean PlacePartialOrder(List<CustomerOrderDetail> lCustOrderList, long COID, string pConnectionString, decimal DeliveryCharge)
        {
            ProductDetails prod = new ProductDetails(System.Web.HttpContext.Current.Server);

            EzeeloDBContext db0 = new EzeeloDBContext();
            List<ShopStockViewModel> lShopStockViewModels = (from cod in lCustOrderList
                                                             join shpstk in db0.ShopStocks on cod.ShopStockID equals shpstk.ID
                                                             select new ShopStockViewModel
                                                             {
                                                                 ID = shpstk.ID,
                                                                 MRP = shpstk.MRP,
                                                                 SaleRate = shpstk.RetailerRate
                                                             }).ToList();

            foreach (CustomerOrderDetail customerOrderDetail in lCustOrderList)
            {
                customerOrderDetail.MRP = lShopStockViewModels.FirstOrDefault(x => x.ID == customerOrderDetail.ShopStockID).MRP;
                customerOrderDetail.SaleRate = lShopStockViewModels.FirstOrDefault(x => x.ID == customerOrderDetail.ShopStockID).SaleRate;
                customerOrderDetail.TotalAmount = customerOrderDetail.Qty * customerOrderDetail.SaleRate;
                customerOrderDetail.BusinessPoints = Convert.ToDecimal(customerOrderDetail.BusinessPointPerUnit) * customerOrderDetail.Qty; //Added by Zubair for MLM on 09-01-2018
                customerOrderDetail.CashbackPointPerUnit = prod.getCasbackPointsOnProduct(customerOrderDetail.WarehouseStockID.Value);
                customerOrderDetail.CashbackPoints = customerOrderDetail.CashbackPointPerUnit * customerOrderDetail.Qty;
                customerOrderDetail.IsActive = true;
            }
            ////Order ID to be returned
            //long outOrderID = 0;

            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    CustomerOrderHistory lOrderHistory = new CustomerOrderHistory();

                    decimal lPayableAmount = 0, lOrderAmount = 0, lTotalTaxAmount = 0;
                    decimal lBusinessPointsTotal = 0; //Added by Zubair for MLM on 6-01-2018
                    decimal lCashbackPontsTotal = 0;
                    /*Shop wise delivery charges in one order*/
                    List<ShopWiseDeliveryCharges> sDelCharges = new List<ShopWiseDeliveryCharges>();

                    var ShopList = lCustOrderList.Select(x => x.ShopID).Distinct();

                    /*loop throughout shop code*/
                    foreach (var sID in ShopList)
                    {
                        long ShopID = Convert.ToInt64(sID);
                        var lCount = db0.CustomerOrderDetails.Where(x => x.CustomerOrderID == COID && x.ShopID == ShopID).Count();
                        string MROD_Code = string.Empty;

                        if (lCount > 0)
                        {
                            MROD_Code = db0.CustomerOrderDetails.Where(x => x.CustomerOrderID == COID && x.ShopID == ShopID).FirstOrDefault().ShopOrderCode;
                        }
                        else if (string.IsNullOrEmpty(MROD_Code))
                        {
                            MROD_Code = GetNextShopOrderCode(ShopID, COID, string.Empty);
                        }

                        /*loop throught product in shop*/
                        foreach (var item in lCustOrderList.Where(x => x.ShopID == ShopID).ToList())
                        /*end of new Additional Code*/
                        {
                            item.ShopOrderCode = MROD_Code;
                            item.CustomerOrderID = COID;

                            /*Check for is Shop order code is available; not empty */
                            if (item.ShopOrderCode.Length != 15)
                                throw new Exception("Invalid Shop order Code; Problem in generating Shop Order Code");

                            EzeeloDBContext db1 = new EzeeloDBContext();
                            db1.CustomerOrderDetails.Add(item);
                            db1.SaveChanges();
                            db1.Dispose();

                            lOrderAmount = lOrderAmount + (item.Qty * item.SaleRate);
                            lBusinessPointsTotal = lBusinessPointsTotal + (item.Qty * Convert.ToDecimal(item.BusinessPointPerUnit)); //Added by Zubair for MLM on 6-01-2018
                            lCashbackPontsTotal = lCashbackPontsTotal + (item.Qty * Convert.ToDecimal(item.CashbackPointPerUnit));
                            /* *********************************
                             * Taxation Management Start Changes
                             * Purpose :- Taxes On Purchase Product 
                             * Developed By : Pradnyakar Badge
                             * 29-03-2016
                             ***********************************/

                            if (!db.ShopStocks.Where(x => x.ID == item.ShopStockID).FirstOrDefault().IsInclusiveOfTax)
                            {
                                BusinessLogicLayer.TaxationManagement objTaxationManagement = new BusinessLogicLayer.TaxationManagement(pConnectionString.Trim());
                                List<ModelLayer.Models.ViewModel.CalulatedTaxesRecord> lCalulatedTaxesRecord = objTaxationManagement.CalculateTaxForProduct(item.ShopStockID);

                                List<TaxOnOrder> prodTaxOnOrder = new List<TaxOnOrder>();

                                foreach (var taxDetail in lCalulatedTaxesRecord)
                                {
                                    TaxOnOrder lTaxOnOrder = new TaxOnOrder();

                                    lTaxOnOrder.Amount = taxDetail.TaxableAmount * item.Qty;
                                    lTaxOnOrder.CustomerOrderDetailID = item.ID;
                                    lTaxOnOrder.IsGSTInclusive = taxDetail.IsGSTInclusive;
                                    lTaxOnOrder.DeviceID = "X";
                                    lTaxOnOrder.DeviceType = "X";
                                    lTaxOnOrder.CreateDate = item.CreateDate;
                                    lTaxOnOrder.CreateBy = item.CreateBy;
                                    lTaxOnOrder.ModifyBy = null;
                                    lTaxOnOrder.ModifyDate = null;
                                    lTaxOnOrder.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                    lTaxOnOrder.ProductTaxID = taxDetail.ProductTaxID;
                                    prodTaxOnOrder.Add(lTaxOnOrder);
                                }
                                EzeeloDBContext db2 = new EzeeloDBContext();
                                db2.TaxOnOrders.AddRange(prodTaxOnOrder);
                                db2.SaveChanges();
                                db2.Dispose();
                                lTotalTaxAmount = prodTaxOnOrder.Sum(x => x.Amount);
                            }
                            /****************************************************/
                            /*********End OF Tax On Order Changes****************/
                            /****************************************************/
                            //================================================= SAVE TAXATION ==========================================================================

                            //stock less
                            //ManageStock(item.ShopStockID, -item.Qty);

                            /*Maintain Customer Order History*/
                            lOrderHistory = new CustomerOrderHistory();
                            lOrderHistory.CreateBy = item.CreateBy;
                            lOrderHistory.CreateDate = item.CreateDate;
                            lOrderHistory.NetworkIP = CommonFunctions.GetClientIP();
                            lOrderHistory.IsActive = true;
                            lOrderHistory.CustomerOrderID = item.CustomerOrderID;
                            lOrderHistory.ShopStockID = item.ShopStockID;
                            lOrderHistory.Status = item.OrderStatus;
                            EzeeloDBContext db3 = new EzeeloDBContext();
                            db3.CustomerOrderHistories.Add(lOrderHistory);
                            db3.SaveChanges();
                            db3.Dispose();


                            /*Delivery order details entry */
                            EzeeloDBContext db4 = new EzeeloDBContext();
                            var lDeliveryPartner = db4.DeliveryPartners.Where(x => x.ID == 1).ToList();

                            if (lDeliveryPartner.Count > 0)
                            {
                                if (db4.DeliveryOrderDetails.Where(x => x.ShopOrderCode == MROD_Code).Count() == 0)
                                {
                                    /*Saves the entry of delivery charges to be collected against each Shop Order*/
                                    DeliveryOrderDetail lDelOrderDetail = new DeliveryOrderDetail();
                                    lDelOrderDetail.ShopOrderCode = MROD_Code;
                                    lDelOrderDetail.CreateBy = item.CreateBy;
                                    lDelOrderDetail.CreateDate = item.CreateDate;
                                    lDelOrderDetail.DeliveryPartnerID = lDeliveryPartner.FirstOrDefault().ID;
                                    lDelOrderDetail.DeliveryCharge = DeliveryCharge;
                                    lDelOrderDetail.DeliveryType = "normal";
                                    lDelOrderDetail.GandhibaghCharge = DeliveryCharge;
                                    lDelOrderDetail.Weight = 0;// item.ShopStock.ShopProduct.Product.WeightInGram;
                                    lDelOrderDetail.IsActive = true;
                                    lDelOrderDetail.IsMyPincode = true;
                                    lDelOrderDetail.OrderAmount = item.TotalAmount;
                                    lDelOrderDetail.NetworkIP = item.NetworkIP;
                                    db4.DeliveryOrderDetails.Add(lDelOrderDetail);
                                    db4.SaveChanges();
                                }
                            }
                            db4.Dispose();
                        }
                    }
                    EzeeloDBContext db5 = new EzeeloDBContext();
                    ModelLayer.Models.CustomerOrder lCustomerOrder = db5.CustomerOrders.Find(COID);

                    lPayableAmount = lCustomerOrder.PayableAmount + lOrderAmount + lTotalTaxAmount;
                    lOrderAmount = lCustomerOrder.OrderAmount + lOrderAmount;
                    lCustomerOrder.OrderAmount = lOrderAmount;
                    //lCustomerOrder.TotalOrderAmount = lCustomerOrder.TotalOrderAmount + lOrderAmount; //Yashaswi 18-9-2018
                    lCustomerOrder.PayableAmount = lPayableAmount;
                    lCustomerOrder.BusinessPointsTotal = lCustomerOrder.BusinessPointsTotal + lBusinessPointsTotal; //Added by Zubair for MLM on 10-01-2018
                    lCustomerOrder.CashbackPointsTotal = lCustomerOrder.CashbackPointsTotal + lCashbackPontsTotal;
                    lCustomerOrder.ModifyBy = lCustOrderList.FirstOrDefault().CreateBy;
                    lCustomerOrder.ModifyDate = lCustOrderList.FirstOrDefault().CreateDate;


                    db5.Entry(lCustomerOrder).State = EntityState.Modified;
                    db5.SaveChanges();
                    db5.Dispose();
                    // Transaction complete
                    ts.Complete();
                    return true;
                }
                catch (Exception exception)
                {
                    // Rollback transaction
                    BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Placing Customer Order :" + exception.Message + exception.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                    //ts.Dispose();
                    db0.Dispose();
                    throw;
                }
                db0.Dispose();
            }
            //try
            //{
            //    this.InsertTransactionInput(outOrderID);
            //}
            //catch (Exception exception)
            //{
            //    BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Placing Customer Order :" + exception.Message + exception.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            //}

        }


        //--Added by Ashwini Meshram 03-Nov-2016 ------//
        //public GCMMsgCustOrderDetail GetCustomerOrders(long lCustLoginID, int index, long lCustomerOrderID)
        //{

        //}

        public DataTable GetCustomerOrder(long lCustLoginID, int Index, long? lCustomerOrderID = null)
        {
            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            string conn = readCon.DB_CONNECTION;
            SqlConnection con = new SqlConnection(conn);
            SqlCommand sqlComm = new SqlCommand("CustomerOrderForGetOrderDetailsAPI", con);
            sqlComm.Parameters.AddWithValue("@UserLoginID", SqlDbType.Int).Value = lCustLoginID;
            sqlComm.Parameters.AddWithValue("@CustomerOrderID", SqlDbType.Int).Value = lCustomerOrderID;
            sqlComm.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(sqlComm);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;

        }

        public DataTable GetCustomerOrderDetail(long? pCustomerOrderID = null)
        {
            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            string conn = readCon.DB_CONNECTION;
            SqlConnection con = new SqlConnection(conn);
            SqlCommand sqlComm = new SqlCommand("Get_CustomerOrderDetailForGCMMsgAlert", con);
            sqlComm.Parameters.AddWithValue("@CustomerOrderID", SqlDbType.Int).Value = pCustomerOrderID;
            sqlComm.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(sqlComm);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;

        }

        public int SendPushNotification(long OrderID)
        {
            //Added Try catch by sonali_04-12-2018
            try
            {
                long lUserLoginID = 0;
                DataTable ldt = new DataTable();
                ldt = GetPlaceOrderDetail(OrderID);

                foreach (DataRow dr in ldt.Rows)
                {
                    //lOrderStatus = dr["OrderStatus"].ToString();
                    lUserLoginID = Convert.ToInt64(dr["UserLoginID"].ToString());
                }

                List<string> lRegIds = new List<string>();
                List<GcmUser> lGcmUsers = new List<GcmUser>();
                lGcmUsers = (from c in db.GcmUsers
                             where c.UserLoginID == lUserLoginID && c.IsActive == true
                             select c).ToList();

                lRegIds = lGcmUsers.Select(x => x.GcmRegID.ToString()).ToList();

                if (lRegIds == null)
                {
                    return 0;
                }
                string lRegdata = "";
                lRegdata = string.Join("\",\"", lRegIds);
                string Status = "Order";
                //lRegdata = string.Join("\",\"", pAPIOrderViewModel.GcmRegID);
                //lRegdata = pAPIOrderViewModel.GcmRegID;

                // applicationID means google Api key                                                                                                     
                var applicationID = "AIzaSyDSymqUriO1nwSuUSgawXBXGaMvgsy26zE";
                // SENDER_ID is nothing but your ProjectID (from API Console- google code)//                                          
                var SENDER_ID = "555896632846";
                WebRequest tRequest;
                tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
                tRequest.Method = "post";
                //tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
                tRequest.ContentType = "application/json;charset=UTF-8";
                //tRequest.ContentType = "application/json";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
                string postData = "{ \"registration_ids\": [ \"" + lRegdata + "\" ], " +
                                      "\"data\": {\"message\":\"" + "Order Placed Successfully" + "\"," +
                                       "\"Status\":\"" + Status + "\"}}";
                // "\"data\": {\"message\":\"" + "hiii pihu" + "\"}}";
                //"\"title\":\"" + pGCMNotification.gCMMsgDetailViewModel.Title + "\", " +
                //"\"imageurl\":\"" + pGCMNotification.gCMMsgDetailViewModel.ImgUrl + "\", " +
                //"\"cat_id\":\"" + pGCMNotification.gCMMsgDetailViewModel.LevelToID + "\", " +
                //"\"level\":\"" + pGCMNotification.gCMMsgDetailViewModel.Level + "\", " +
                //"\"cityid\": \"" + pGCMNotification.GcmUsers.FirstOrDefault().City + "\"}}";

                Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                tRequest.ContentLength = byteArray.Length;
                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse tResponse = tRequest.GetResponse();
                dataStream = tResponse.GetResponseStream();
                StreamReader tReader = new StreamReader(dataStream);
                String sResponseFromServer = tReader.ReadToEnd();   //Get response from GCM server.
                string lServerMsg = sResponseFromServer;  //Assigning GCM response to Label text 
                Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(lServerMsg); //JObject.Parse(lServerMsg);

                tReader.Close();
                dataStream.Close();
                tResponse.Close();

                var data = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(lServerMsg);
                string status = data["success"].Value<string>();
                return Convert.ToInt32(status);
            }
            catch (Exception ex)
            {

            }
            return 1;  //Return 1 by sonali_04-12-2018
            //return -1;
        }

        //*******Code For FCM Added by harshada***********//

        public int AndroidPushFCM(long OrderID)
        {
            long lUserLoginID = 0;
            DataTable ldt = new DataTable();
            ldt = GetPlaceOrderDetail(OrderID);

            foreach (DataRow dr in ldt.Rows)
            {
                //lOrderStatus = dr["OrderStatus"].ToString();
                lUserLoginID = Convert.ToInt64(dr["UserLoginID"].ToString());
            }

            List<string> lRegIds = new List<string>();
            List<GcmUser> lGcmUsers = new List<GcmUser>();
            lGcmUsers = (from c in db.GcmUsers
                         where c.UserLoginID == lUserLoginID && c.IsActive == true
                         select c).ToList();

            lRegIds = lGcmUsers.Select(x => x.GcmRegID.ToString()).ToList();

            if (lRegIds == null)
            {
                return 0;
            }
            string lRegdata = "";
            lRegdata = string.Join("\",\"", lRegIds);
            string Status = "Order";

            string Response = "1";
            try
            {

                string applicationID = "AAAAluPov4U:APA91bFGwjdn8BMsMSV95hCjIF3_nc3c6J8MC1GJe-8MRt9y7VKU_cZgxMwB24nEJxhsn-kYWyZQNFj_pKUN_DzhQUwUCPWhWWOb6drnKNQn-7jLhgwDqPzO2Spn3WowfPY_REH5WA2b";

                string senderId = "648068775813";

                string deviceId = "eOEBmgMRfg0:APA91bHzwKlih6Wnfqz4156JQl2cfLEY74gk-nkZcI0M4AOcY7GSJJ2DWpgqSY1jSY4madX_PMVqvPbo7o1b9tyeOJ1ZhJQnikLT_ZYaNVL3vArs6fYuz5erc_50IG0l1pRjuTOxKWzN";

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data1 = new
                {

                    to = deviceId, // this is for topic 

                    data = new
                    {
                        message = "Order Placed Successfully",
                        Status = Status
                    }
                };
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data1);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.ContentLength = byteArray.Length;
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));


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


                //Get response from GCM server.
                string lServerMsg = Response;    //Assigning GCM response to Label text 
                JObject json1 = JObject.Parse(lServerMsg);
                var data11 = (JObject)JsonConvert.DeserializeObject(lServerMsg);
                string status = data11["success"].Value<string>();
                return Convert.ToInt32(status);
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                return Convert.ToInt32(Response);
            }

        }

        //*******End Code For FCM Added by harshada***********//

        public void SendAlertforPushNotification(long OrderCode, string OrderStatus)
        {
            string lOrderStatus = string.Empty;
            if (OrderStatus == "Placed")
            {
                lOrderStatus = "ORD_PLAC";
            }
            else if (OrderStatus == "Cancelled")
            {
                lOrderStatus = "ORD_CANC";
            }
            string lOrderCode = string.Empty;
            DateTime CreateDate = DateTime.Now;

            try
            {
                DataTable ldt = new DataTable();
                ldt = GetPlaceOrderDetail(OrderCode);
                foreach (DataRow dr in ldt.Rows)
                {
                    //lOrderStatus = dr["OrderStatus"].ToString();
                    lOrderCode = dr["OrderCode"].ToString();
                }

                OrderStatusSMSandEMAIL lOrderStatusSMSandEMAIL = new OrderStatusSMSandEMAIL();

                lOrderStatusSMSandEMAIL.GCMMsgTemplate(lOrderStatus, lOrderCode, OrderCode, CreateDate);
            }
            catch (Exception ex)
            {
                // throw;//Comment by Sonali for Test_04-12-2018
            }
        }

        public DataTable GetPlaceOrderDetail(long OrderCode)
        {
            DataTable ldt = new DataTable();
            try
            {
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                string conn = readCon.DB_CONNECTION;
                SqlConnection con = new SqlConnection(conn);
                SqlCommand sqlComm = new SqlCommand("[Get_DetailsForPlaceOrder]", con);
                sqlComm.Parameters.AddWithValue("@OrderID", OrderCode);
                sqlComm.CommandType = CommandType.StoredProcedure;
                con.Open();
                sqlComm.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                da.Fill(ldt);
                con.Close();
                //return ldt;

            }
            catch (Exception ex)
            {
                throw;
            }
            return ldt;
        }
        //Added by Rumana on 19/04/2019
        public bool Insert_RefundRequest_EwalletRefund(long OrderID)
        {
            bool flag = false;
            try
            {
                var lOrder = db.CustomerOrders.Where(x => x.ID == OrderID).FirstOrDefault();
                if (lOrder != null && lOrder.MLMAmountUsed != 0)
                {
                    EWalletRefund_Table obj = new EWalletRefund_Table();
                    obj.RequsetAmt = lOrder.MLMAmountUsed != null ? lOrder.MLMAmountUsed.Value : 0;
                    obj.Date = CommonFunctions.GetLocalTime();
                    obj.CustomerOrderId = lOrder.ID;
                    obj.UserLoginId = lOrder.UserLoginID;
                    obj.Status = 0;
                    obj.Isactive = false;
                    obj.Createdby = CommonFunctions.GetPersonalDetailsID(lOrder.UserLoginID);
                    obj.CreatedDate = CommonFunctions.GetLocalTime();
                    obj.NetworkID = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    obj.DeviceID = string.Empty;
                    obj.DeviceType = string.Empty;
                    db.eWalletRefund_Table.Add(obj);
                    db.SaveChanges();
                    flag = true;
                }
            }
            catch (Exception ex)
            {

            }
            return flag;
        }
        public long Insert_EwalletRefund_OnAbandonedStatus(long OrderID)
        {
            EWalletRefund_Table obj = new EWalletRefund_Table();
            try
            {
                var lOrder = db.CustomerOrders.Where(x => x.ID == OrderID).FirstOrDefault();
                if (lOrder != null && lOrder.MLMAmountUsed != 0)
                {

                    obj.RequsetAmt = lOrder.MLMAmountUsed != null ? lOrder.MLMAmountUsed.Value : 0;
                    obj.RefundAmt = lOrder.MLMAmountUsed != null ? lOrder.MLMAmountUsed.Value : 0;
                    obj.Date = CommonFunctions.GetLocalTime();
                    obj.CustomerOrderId = lOrder.ID;
                    obj.UserLoginId = lOrder.UserLoginID;
                    obj.Status = 1;
                    obj.Isactive = true;
                    obj.Comment = "OnAbandonedStatus";
                    obj.Createdby = CommonFunctions.GetPersonalDetailsID(lOrder.UserLoginID);
                    obj.CreatedDate = CommonFunctions.GetLocalTime();
                    obj.NetworkID = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    obj.DeviceID = string.Empty;
                    obj.DeviceType = string.Empty;
                    db.eWalletRefund_Table.Add(obj);
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {

            }
            return obj.ID;
        }
        //Ended by Rumana on 19/04/2019
       
        //Added by Rumana
        public bool Send_EWalletRefund_Mail(long orderId, bool IsMailSend)
        {


            try
            {
                string cancelledDate;
                int CancelledproductTotal_Amt;
                var CustomerOrder = db.CustomerOrders.Where(x => x.ID == orderId).FirstOrDefault();
                var personalDetail = db.PersonalDetails.Where(x => x.UserLoginID == CustomerOrder.UserLoginID).FirstOrDefault();
                CancelledproductTotal_Amt = Convert.ToInt32(CustomerOrder.PayableAmount);
                //bool CustomerOrderDetail_Ids = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == orderId).Any(x => x.OrderStatus == 1 || x.OrderStatus == 7);

                //if (CustomerOrderDetail_Ids == true || db.CustomerOrderDetails.Where(x => x.CustomerOrderID == orderId).Count() == 1)
                //{
                //    CancelledproductTotal_Amt = Convert.ToInt32(db.CustomerOrderDetails.Where(x => x.CustomerOrderID == orderId && x.OrderStatus == 9).Sum(x => x.SaleRate * x.Qty));
                //    CancelledproductTotal_Amt = Convert.ToInt32(CancelledproductTotal_Amt) - Convert.ToInt32(CustomerOrder.MLMAmountUsed);
                //}
                //else
                //{
                //    CancelledproductTotal_Amt = Convert.ToInt32(CustomerOrder.PayableAmount);
                //}

                if (CustomerOrder.MLMAmountUsed != 0)
                {

                    cancelledDate = Convert.ToString(db.eWalletRefund_Table.Where(x => x.CustomerOrderId == orderId).FirstOrDefault().CreatedDate);
                }
                else
                {
                    cancelledDate = Convert.ToString(DateTime.Now);
                    //CustomerOrderDetail.MLMAmountUsed = "Not applicable";
                }


                if (orderId != null && orderId > 0)
                {
                    string Name = personalDetail.FirstName + " " + personalDetail.LastName;
                    //string URL = "http://www.ezeelo.com/nagpur/2/login?Phone=" + mobile + "&ReferalCode=" + RefferalCode + "&Name=" + name + "&Email=" + Email;
                    //string URL = "" + (new URLsFromConfig()).GetURL("LEADERS") + "LeadersLogin/Login/?UserName=" + Email + "&Password=" + Password;

                    Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                    dictEmailValues.Add("<!--Order_Code-->", CustomerOrder.OrderCode);
                    dictEmailValues.Add("<!--Order_Cancelled_Date-->", cancelledDate);
                    dictEmailValues.Add("<!--Customer_Name-->", Name);
                    dictEmailValues.Add("<!--EWallet_Refund_Amount-->", Convert.ToString(CustomerOrder.MLMAmountUsed));
                    dictEmailValues.Add("<!--Refund Amountt-->", Convert.ToString(CancelledproductTotal_Amt));
                    dictEmailValues.Add("<!--Pay_Mode-->", CustomerOrder.PaymentMode);

                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                    // string EmailID = "tech@ezeelo.com";
                    //ReadConfig readConfig = new ReadConfig(server);

                    //string MailAddress = readConfig.EwalletRefundMailId;
                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ON_REQUEST_SEND_CRM_EWALLETREFUND, new string[] { System.Web.Configuration.WebConfigurationManager.AppSettings["Ewallet_Refund_Mail"].ToString() }, dictEmailValues, true);
                    IsMailSend = true;
                }
            }
            catch (Exception ex)
            {

            }
            return IsMailSend;
        }
        //Ended by Rumana on 19/04/2019
        //Added by Rumana
        public bool Send_EWalletRefund_Mail_FromPartner(long orderId, decimal MLMRefundAmount, decimal RefundOrignalAmount, bool IsSendMail)
        {

            try
            {

                var CustomerOrder = db.CustomerOrders.Where(x => x.ID == orderId).FirstOrDefault();
                var personalDetail = db.PersonalDetails.Where(x => x.UserLoginID == CustomerOrder.UserLoginID).FirstOrDefault();


                if (orderId != null && orderId > 0)
                {
                    string Name = personalDetail.FirstName + " " + personalDetail.LastName;
                    //string URL = "http://www.ezeelo.com/nagpur/2/login?Phone=" + mobile + "&ReferalCode=" + RefferalCode + "&Name=" + name + "&Email=" + Email;
                    //string URL = "" + (new URLsFromConfig()).GetURL("LEADERS") + "LeadersLogin/Login/?UserName=" + Email + "&Password=" + Password;

                    Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                    dictEmailValues.Add("<!--Order_Code-->", CustomerOrder.OrderCode);
                    dictEmailValues.Add("<!--Order_Cancelled_Date-->", Convert.ToString(DateTime.Now));
                    dictEmailValues.Add("<!--Customer_Name-->", Name);
                    dictEmailValues.Add("<!--EWallet_Refund_Amount-->", Convert.ToString(MLMRefundAmount));
                    dictEmailValues.Add("<!--Refund Amountt-->", Convert.ToString(RefundOrignalAmount));
                    dictEmailValues.Add("<!--Pay_Mode-->", CustomerOrder.PaymentMode);

                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                    // string EmailID = "tech@ezeelo.com";
                    //ReadConfig readConfig = new ReadConfig(server);

                    //string MailAddress = readConfig.EwalletRefundMailId;
                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ON_REQUEST_SEND_CRM_EWALLETREFUND, new string[] { System.Web.Configuration.WebConfigurationManager.AppSettings["Ewallet_Refund_Mail"].ToString() }, dictEmailValues, true);
                    IsSendMail = true;
                }
            }
            catch (Exception ex)
            {

            }
            return IsSendMail;
        }
        //Added by Rumana for return MLM Amount Used on Abandoned Status on 20-05-2019
        public void EWalletAmountUsed_Return(long CustomerOrderID, long UserLognID)
        {
            long userLoginID = UserLognID;
            if (UserLognID == null)
            {
                userLoginID = Convert.ToInt64(System.Web.HttpContext.Current.Session["UID"]);
            }

            decimal MLMAmountUsed = 0;

            MLMAmountUsed = Convert.ToDecimal(db.CustomerOrders.Where(x => x.ID == CustomerOrderID).Select(x => x.MLMAmountUsed).FirstOrDefault());


            if (MLMAmountUsed > 0)
            {
                MLMWallet objWallet = db.MLMWallets.Where(x => x.UserLoginID == userLoginID).FirstOrDefault();
                objWallet.Amount = objWallet.Amount + MLMAmountUsed;
                db.SaveChanges();
            }
        }
        public void SensSMStoUplineForBoostPlan(long UserLoginID)
        {
            try
            {
                string UserName = db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == UserLoginID).FirstName;
                string fConnectionString = System.Configuration.ConfigurationSettings.AppSettings["DB_CON"].ToString();
                DataTable lDataTableCustomerOrder = new DataTable();
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand sqlComm = new SqlCommand("GetUplineUserForBoostPlan", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@UserLoginId", SqlDbType.BigInt).Value = UserLoginID;
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        DataRow dr = dt.Rows[i];
                        Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                        dictEmailValues.Add("#--NAME--#", dr["FirstName"].ToString());
                        dictEmailValues.Add("#--CNAME--#", UserName);
                        BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                        gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.UPLINE_FOR_BOOST_PLAN, new string[] { dr["Mobile"].ToString() }, dictEmailValues);
                    }
                    catch { }
                }
            }
            catch(Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Placing Customer Order :" + ex.Message + ex.InnerException ?? "", ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

            }
        }
    }






    //- Added by Avi Verma : //
    public class OrderManagement
    {

        private string fConnectionString = System.Configuration.ConfigurationSettings.AppSettings["DB_CON"].ToString();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetNextGBOD()
        {
            int lGBOD = -1;
            using (TransactionScope Scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    DataTable lDataTableCustomerOrder = new DataTable();
                    SqlConnection con = new SqlConnection(fConnectionString);
                    SqlCommand sqlComm = new SqlCommand("SelectNextGBOD", con);
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    //sqlComm.Parameters.AddWithValue("@pFranchiseID", SqlDbType.Int).Value = pFranchiseID;
                    con.Open();
                    //object o = sqlComm.ExecuteScalar();
                    SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        lGBOD = Convert.ToInt32(dt.Rows[0][0]);
                    }
                    con.Close();
                    return lGBOD;
                }
                catch (Exception ex)
                {
                    throw new BusinessLogicLayer.MyException("[CustomerOrder -> GetNextGBOD]", "Problem in getting GBOD" + Environment.NewLine + ex.Message);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetNextMROD()
        {
            int lMROD = -1;
            using (TransactionScope Scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    DataTable lDataTableCustomerOrder = new DataTable();
                    SqlConnection con = new SqlConnection(fConnectionString);
                    SqlCommand sqlComm = new SqlCommand("SelectNextMROD", con);
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    //sqlComm.Parameters.AddWithValue("@pFranchiseID", SqlDbType.Int).Value = pFranchiseID;
                    con.Open();
                    //object o = sqlComm.ExecuteScalar();
                    SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        lMROD = Convert.ToInt32(dt.Rows[0][0]);
                    }
                    con.Close();
                    return lMROD;
                }
                catch (Exception ex)
                {
                    throw new BusinessLogicLayer.MyException("[CustomerOrder -> GetNextMROD]", "Problem in getting MROD" + Environment.NewLine + ex.Message);
                }
                //return -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetNextCart()
        {
            int lCartID = -1;
            using (TransactionScope Scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    DataTable lDataTableCustomerOrder = new DataTable();
                    SqlConnection con = new SqlConnection(fConnectionString);
                    SqlCommand sqlComm = new SqlCommand("SelectNextCART", con);
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    //sqlComm.Parameters.AddWithValue("@pFranchiseID", SqlDbType.Int).Value = pFranchiseID;
                    con.Open();
                    //object o = sqlComm.ExecuteScalar();
                    SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        lCartID = Convert.ToInt32(dt.Rows[0][0]);
                    }
                    con.Close();
                    return lCartID;
                }
                catch (Exception ex)
                {
                    throw new BusinessLogicLayer.MyException("[CustomerOrder -> GetNextCart]", "Problem in getting CARTID" + Environment.NewLine + ex.Message);
                }
                //return -1;
            }
        }

        

    }
}
