//-----------------------------------------------------------------------
// <copyright file="PlaceOrderController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------

using API.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;
using ModelLayer;
using ModelLayer.Models;
using System.Web.Configuration;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using System.Configuration;
using Newtonsoft.Json;

/*
 Handed over on 15-09-2015 to Ajit, AVi, Mohit, Manoj
 */
namespace API.Controllers
{
    public class PlaceOrderController : ApiController
    {
        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
        private EzeeloDBContext db = new EzeeloDBContext();
        /// <summary>
        /// Get Customer Ordrs
        /// </summary>
        /// <param name="lCustLoginID">Customer Login ID</param>
        /// <returns>List of order summaries</returns>
        // [TokenVerification]
        [ApiException]
        [ValidateModel]
        // GET api/placeorder/5
        public HttpResponseMessage Get(long lCustLoginID)
        {
            BusinessLogicLayer.CustomerOrder lCustOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
            var formatter = new JsonMediaTypeFormatter();
            var json = formatter.SerializerSettings;
            json.Converters.Add(new MyDateTimeConvertor());
            return Request.CreateResponse(HttpStatusCode.OK, lCustOrder.GetCustomerOrders(lCustLoginID), formatter);

        }


        /// <summary>
        /// Place Customer order
        /// </summary>
        /// <param name="pOrderViewModel"> Order object which contains the list of order products and shop wise delivery charges.</param>
        /// <returns>Operation Status.</returns>
        [TokenVerification]
        [ApiException]
        [ValidateModel]
        // GET api/placeorder
        [Route("api/PlaceOrder")]
        public object POST(APIOrderViewModel pOrderViewModel)
        {
            object obj = new object();
            try
            {
                string Message = JsonConvert.SerializeObject(pOrderViewModel);
                BusinessLogicLayer.OrderLog.OrderLogFile("Order placed api called" + Message, "API", System.Web.HttpContext.Current.Server);

                if (pOrderViewModel == null || pOrderViewModel.CustomerOrder.UserLoginID == null || pOrderViewModel.CustomerOrderDetail == null || pOrderViewModel.CartId == null)
                {
                    return obj = new { Success = 0, Message = "Invalid request. Please verify, Shop wise delivery charges, customer order details cannot be empty or null.", data = string.Empty };
                    // return new { HTTPStatusCode = "400", UserMessage = "", OrderID = 0 };

                }
                var City = (from pin in db.Pincodes
                            join city in db.Cities on pin.CityID equals city.ID
                            where pin.ID == pOrderViewModel.CustomerOrder.PincodeID && pin.IsActive == true
                            select new
                            {
                                CityName = city.Name,
                                CityId = city.ID
                            }).FirstOrDefault();

                /*Check for product and its stock is available for given shop*/
                if (pOrderViewModel.UsedEarnAmount > pOrderViewModel.CustomerOrder.PayableAmount)
                {
                    return obj = new { Success = 0, Message = "Used E-wallet Amount can't be more than Payable Amount.", data = string.Empty };
                }
                
                //Yashaswi 29-8-2019 for Booster plan
                bool isBoosterPlan = false;
                int BoosterPlanProductCount = 0;
                decimal TotalRP = 0;
                long BoosterPlanCategoryId = db.BoosterPlanMaster.FirstOrDefault(p => p.IsActive == true).BoosterCategoryId;
                foreach (var item in pOrderViewModel.CustomerOrderDetail)
                {
                    ShopStock SS = db.ShopStocks.FirstOrDefault(s => s.ID == item.ShopStockID);
                    long ProductId = db.ShopProducts.FirstOrDefault(sp => sp.ID == SS.ShopProductID).ProductID;
                    int? ThirdLevelCat = db.Categories.FirstOrDefault(q => q.ID == (db.Categories.FirstOrDefault(p => p.ID == (db.Products.FirstOrDefault(pp => pp.ID == ProductId).CategoryID) && p.Level == 3).ParentCategoryID) && q.Level == 2).ParentCategoryID;
                    if (BoosterPlanCategoryId == ThirdLevelCat)
                    {
                        BoosterPlanProductCount = BoosterPlanProductCount + 1;
                    }
                    TotalRP = TotalRP + item.Qty * SS.BusinessPoints;
                }

                if (BoosterPlanProductCount != 0)
                {
                    decimal MaxBoostRP = db.BoosterPlanMaster.FirstOrDefault(p => p.BoosterCategoryId == BoosterPlanCategoryId).RetailPoints;
                    string CategoryName = db.Categories.FirstOrDefault(p => p.ID == BoosterPlanCategoryId).Name;
                    if (BoosterPlanProductCount != pOrderViewModel.CustomerOrderDetail.Count())
                    {
                        return obj = new { Success = 0, Message = "Your cart contain " + CategoryName + " category product. " + CategoryName + " category product can not be purchase with other category product, to go further please remove other category products from your cart!!!", data = string.Empty };
                    }
                    else if (MaxBoostRP > TotalRP)
                    {
                        return obj = new { Success = 0, Message = "To place " + CategoryName + " order, cart Retail Points total should be more than " + MaxBoostRP + " RP", data = string.Empty };
                    }
                    else
                    {
                        isBoosterPlan = true;
                    }
                }



                foreach (var item in pOrderViewModel.CustomerOrderDetail)
                {

                    string ProductName = db.Products.FirstOrDefault(p => p.ID == (db.ShopProducts.FirstOrDefault(sp => sp.ID == (db.ShopStocks.FirstOrDefault(ss => ss.ID == item.ShopStockID).ShopProductID)).ProductID)).Name; //Addedby yashaswi 12-6-19

                    var productDetails = (from sp in db.ShopProducts
                                          join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                                          join ws in db.WarehouseStocks on ss.WarehouseStockID equals ws.ID
                                          where sp.ShopID == item.ShopID && sp.IsActive == true && ss.ID == item.ShopStockID
                                          select new
                                          {
                                              ShopStockID = ss.ID,
                                              StockQty = ss.Qty,
                                              WarehouseQty = ws.AvailableQuantity
                                          }).ToList();


                    if (productDetails == null || productDetails.Count == 0)
                    {
                        return obj = new { Success = 0, Message = "Invalid request. Provided ShopStockDetails are invalid.", data = string.Empty };
                        // return new { HTTPStatusCode = "400", UserMessage = "Invalid request. Provided ShopStockDetails are invalid.", OrderID = 0 };

                    }
                    //if (!(pOrderViewModel.shopWiseDeliveryCharges.Select(x => x.ShopID).ToList().Contains(item.ShopID)))
                    //{
                    //    return obj = new { Success = 0, Message = "Invalid request. Please provide delivery charges for Shop ID " + item.ShopID, data = string.Empty };
                    //    //return new { HTTPStatusCode = "400", UserMessage = "Invalid request. Please provide delivery charges for Shop ID " + item.ShopID, OrderID = 0 };
                    //}
                    if (productDetails.Select(x => x.StockQty).FirstOrDefault() <= 0)
                    {
                        return obj = new { Success = 0, Message = "Product " + ProductName + " is out of stock! Please remove product from cart and continue.", data = string.Empty };
                    }
                    if (productDetails.Select(x => x.WarehouseQty).FirstOrDefault() <= 0)
                    {
                        return obj = new { Success = 0, Message = "Product " + ProductName + " is out of stock! Please remove product from cart and continue.", data = string.Empty };
                    }
                    if (item.Qty > productDetails.Select(x => x.StockQty).FirstOrDefault())
                    {
                        return obj = new { Success = 0, Message = "We're sorry! We are able to accommodate only " + productDetails.Select(x => x.StockQty).FirstOrDefault() + " units of " + ProductName + ".if more Qty required call to customer care.", data = string.Empty };
                    }
                    if (item.Qty > productDetails.Select(x => x.WarehouseQty).FirstOrDefault())
                    {
                        return obj = new { Success = 0, Message = "We're sorry! We are able to accommodate only " + productDetails.Select(x => x.WarehouseQty).FirstOrDefault() + " units of " + ProductName + ".if more Qty required call to customer care.", data = string.Empty };
                    }

                    var validCustomerDetails = db.PersonalDetails.Where(x => x.UserLoginID == pOrderViewModel.CustomerOrder.UserLoginID).FirstOrDefault();
                    if (validCustomerDetails == null)
                    {
                        return obj = new { Success = 0, Message = "Invalid request. CustomerLoginID is not valid. ", data = string.Empty };
                        // return new { HTTPStatusCode = "400", UserMessage = "Invalid request. CustomerLoginID is not valid.", OrderID = 0 };
                    }
                    int NewPurchaseQty;
                    string msg;
                    ShoppingCartInitialization objCart = new ShoppingCartInitialization();
                    objCart.VerifyCartItem(productDetails.Select(x => x.StockQty).FirstOrDefault(), item.Qty, productDetails.Select(x => x.WarehouseQty).FirstOrDefault(), out NewPurchaseQty, out msg);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return obj = new { Success = 0, Message = msg, data = string.Empty };
                    }
                    TrackCartBusiness.InsertCartDetails(pOrderViewModel.CartId, item.Qty, pOrderViewModel.CustomerOrder.UserLoginID, item.ShopStockID, "", "PAYMENT_MODE", "", "", "Mobile", "", City.CityName, "", pOrderViewModel.FranchiseId);////added params item.FranchiseID for Multiple MCO

                }

                //  lTrackCart.SaveDetailOnPaymentProcess(lCartID, null, UserID, MobileNo, "PAYMENT_MODE", cookieValue.Split('$')[1], Convert.ToInt32(cookieValue.Split('$')[2]));//--added by Ashish for multiple franchise in same city--//

                //List<CalculatedTaxList> listCalculatedTaxList = new List<CalculatedTaxList>();

                List<CalulatedTaxesRecord> listCalTaxRec = new List<CalulatedTaxesRecord>();
                OrderViewModel orderViewModel = new OrderViewModel();
                ModelLayer.Models.CustomerOrder modelCustomerOrder = new ModelLayer.Models.CustomerOrder();

                // Set value to customer order
                modelCustomerOrder.UserLoginID = pOrderViewModel.CustomerOrder.UserLoginID;
                modelCustomerOrder.OrderAmount = pOrderViewModel.CustomerOrder.OrderAmount;
                // modelCustomerOrder.TotalOrderAmount = pOrderViewModel.CustomerOrder.OrderAmount;
                modelCustomerOrder.NoOfPointUsed = pOrderViewModel.CustomerOrder.NoOfPointUsed;
                modelCustomerOrder.ValuePerPoint = pOrderViewModel.CustomerOrder.ValuePerPoint;
                modelCustomerOrder.CoupenCode = pOrderViewModel.CustomerOrder.CoupenCode;
                modelCustomerOrder.CoupenAmount = pOrderViewModel.CustomerOrder.CoupenAmount;

                if (pOrderViewModel.CustomerOrder.PAN != null)
                    modelCustomerOrder.PAN = pOrderViewModel.CustomerOrder.PAN;
                else
                    modelCustomerOrder.PAN = string.Empty;
                if (pOrderViewModel.CustomerOrder.PaymentMode == "Cash On Delivery")
                    modelCustomerOrder.PaymentMode = "COD";
                else
                    modelCustomerOrder.PaymentMode = "ONLINE";

                modelCustomerOrder.PayableAmount = pOrderViewModel.CustomerOrder.PayableAmount;
                modelCustomerOrder.PrimaryMobile = pOrderViewModel.CustomerOrder.PrimaryMobile;
                modelCustomerOrder.SecondoryMobile = pOrderViewModel.CustomerOrder.SecondoryMobile;
                modelCustomerOrder.ShippingAddress = pOrderViewModel.CustomerOrder.ShippingAddress;
                modelCustomerOrder.PincodeID = pOrderViewModel.CustomerOrder.PincodeID;

                modelCustomerOrder.AreaID = pOrderViewModel.CustomerOrder.AreaID;
                modelCustomerOrder.CreateDate = CommonFunctions.GetLocalTime();
                modelCustomerOrder.DeviceType = "Mobile";

                modelCustomerOrder.MLMAmountUsed = pOrderViewModel.CustomerOrder.WalletAmountUsed; //Added by Zubair for MLM on 25-01-2018
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
                    if(TotalWalAmt< modelCustomerOrder.MLMAmountUsed)
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
                orderViewModel.CustomerOrder = modelCustomerOrder;

                List<long> StockIDList = pOrderViewModel.CustomerOrderDetail.Select(x => x.ShopStockID).Distinct().ToList();
                List<ShopStockIDs> ShopStockIds = new List<ShopStockIDs>();
                foreach (var item in StockIDList)
                {
                    ShopStockIDs shopsId = new ShopStockIDs();
                    shopsId.ssID = item;
                    ShopStockIds.Add(shopsId);
                }
                ProductDetails prod = new ProductDetails(System.Web.HttpContext.Current.Server);
                var result = prod.GetShopStockVarients(ShopStockIds);

                List<CustomerOrderDetail> CustomerOrderDetail = new List<ModelLayer.Models.CustomerOrderDetail>();
                CustomerOrderDetail customerOrderDetail = new CustomerOrderDetail();

                foreach (var item in pOrderViewModel.CustomerOrderDetail)
                {
                    var orderitem = result.Where(x => x.ShopStockID == item.ShopStockID).FirstOrDefault();
                    customerOrderDetail = new CustomerOrderDetail();

                    customerOrderDetail.ID = 0;
                    customerOrderDetail.ShopOrderCode = string.Empty;
                    customerOrderDetail.CustomerOrderID = 0;
                    customerOrderDetail.ShopStockID = item.ShopStockID;
                    customerOrderDetail.ShopID = item.ShopID;
                    customerOrderDetail.Qty = item.Qty;
                    if (pOrderViewModel.IsOnlinePayment == "1" || pOrderViewModel.IsOnlinePayment == "2")
                        customerOrderDetail.OrderStatus = 0;
                    else
                        customerOrderDetail.OrderStatus = 1;
                    customerOrderDetail.MRP = orderitem.MRP;
                    customerOrderDetail.SaleRate = orderitem.SaleRate;
                    customerOrderDetail.OfferPercent = 0;
                    customerOrderDetail.OfferRs = 0;
                    if (orderitem.TaxesOnProduct == null || orderitem.TaxesOnProduct.Count <= 0)
                    {
                        customerOrderDetail.IsInclusivOfTax = false;
                    }
                    else
                    {
                        customerOrderDetail.IsInclusivOfTax = true;
                    }
                    customerOrderDetail.WarehouseStockID = orderitem.WareHouseStockId; // Added by Sonali for Inventory on 28-03-2018
                    //customerOrderDetail.TotalAmount = 10000;
                    customerOrderDetail.TotalAmount = item.Qty * orderitem.SaleRate;
                    customerOrderDetail.IsActive = true;
                    customerOrderDetail.BusinessPointPerUnit = orderitem.RetailPoint;//Added by Sonali for MLM on 18-09-2018
                    customerOrderDetail.BusinessPoints = orderitem.RetailPoint * item.Qty; //Added by Sonali for MLM on 18-09-2018

                    customerOrderDetail.CashbackPointPerUnit = prod.getCasbackPointsOnProduct(orderitem.WareHouseStockId.Value); ;
                    customerOrderDetail.CashbackPoints = customerOrderDetail.CashbackPointPerUnit * item.Qty;
                    if (orderitem.TaxesOnProduct != null)
                    {
                        //TaxationManagement objTaxationManagement = new TaxationManagement(fConnectionString);
                        //item.TaxesOnProduct = objTaxationManagement.CalculateTaxForProduct(item.ShopStockID);

                        //if (!item.IsInclusivOfTax)
                        //{
                        //List<TaxList> lTaxList = this.GetTaxMasterList();
                        //List<CalculatedTaxList> taxlist = this.GetTaxCalCulatedList(item.TaxesOnProduct, lTaxList, item.Qty);
                        //listCalculatedTaxList.AddRange(taxlist);
                        // }
                        //customerOrderDetail.TaxOnOrders = (from n in item.TaxesOnProduct.AsEnumerable()
                        //                                   select new TaxOnOrder
                        //                                   {
                        //                                       ProductTaxID = n.ProductTaxID,
                        //                                       Amount = n.TaxableAmount
                        //                                   }).ToList();
                        //listCalculatedTaxList = (from row in listCalculatedTaxList
                        //                         group row by new { row.TaxName } into g
                        //                         select new CalculatedTaxList()
                        //                         {
                        //                             TaxName = g.Key.TaxName,
                        //                             Amount = g.Sum(x => x.Amount)
                        //                         }).ToList();
                        //   shopProductCollection.lCalculatedTaxList = listCalculatedTaxList;
                        if (orderitem.TaxesOnProduct != null)
                        {
                            listCalTaxRec.AddRange(orderitem.TaxesOnProduct);
                        }
                    }
                    CustomerOrderDetail.Add(customerOrderDetail);
                }
                orderViewModel.CustomerOrderDetail = CustomerOrderDetail;
                orderViewModel.CustomerOrder.CashbackPointsTotal = orderViewModel.CustomerOrderDetail.Sum(p => p.CashbackPoints);
                /*Delivery Schedule Detail ID
                * Pradnyakar
                * 11-01-2016
                */
                orderViewModel.ScheduleID = pOrderViewModel.ScheduleID;
                orderViewModel.ScheduleDate = pOrderViewModel.ScheduleDate;
                orderViewModel.CustomerOrder.BusinessPointsTotal = orderViewModel.CustomerOrderDetail.Sum(x => x.BusinessPoints ?? 0); //Added by Zubair for MLM on 05-01-2018
                orderViewModel.CustomerOrder.OrderAmount = orderViewModel.CustomerOrderDetail.Sum(x => x.TotalAmount);

                DeliveryCharges dc = new DeliveryCharges();
                GetShopWiseDeliveryChargesViewModel ShopListAndPincode = new GetShopWiseDeliveryChargesViewModel();
                ShopListAndPincode.CityID = City.CityId;
                ShopListAndPincode.IsExpress = false;
                ShopListAndPincode.Pincode = db.Pincodes.Where(x => x.ID == pOrderViewModel.CustomerOrder.PincodeID).Select(x => x.Name).FirstOrDefault();
                ShopWiseDeliveryCharges shopwiseDeliverycharge = new ShopWiseDeliveryCharges();
                shopwiseDeliverycharge.ShopID = result.Select(x => x.ShopID).FirstOrDefault();
                shopwiseDeliverycharge.OrderAmount = orderViewModel.CustomerOrder.OrderAmount;
                shopwiseDeliverycharge.DeliveryType = "Normal";
                ShopListAndPincode.ShopWiseDelivery = new List<ShopWiseDeliveryCharges>();
                ShopListAndPincode.ShopWiseDelivery.Add(shopwiseDeliverycharge);
                orderViewModel.shopWiseDeliveryCharges = dc.GetDeliveryCharges(ShopListAndPincode);
                orderViewModel.lCalulatedTaxesRecord = listCalTaxRec;

                BusinessLogicLayer.CustomerOrder lCustOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);

                /*Save vallet amount if customer use vallet amount while placing order.*/

                if (pOrderViewModel.UsedEarnAmount > 0)
                {
                    EarnDetail lED = new EarnDetail();

                    lED.EarnAmount = pOrderViewModel.EarnAmount;
                    lED.RemainingAmount = pOrderViewModel.RemainingAmount;
                    lED.UsedAmount = pOrderViewModel.UsedEarnAmount;
                    lED.EarnUID = pOrderViewModel.CustomerOrder.UserLoginID;
                    orderViewModel.lEarnDetail = lED;

                }
                orderViewModel.IsBoostPlan = isBoosterPlan;
                long OrderID = lCustOrder.PlaceCustomerOrder(orderViewModel);
                if (OrderID > 0)
                {
                    string txnid = Generatetxnid();
                    OrderPlacedSmsAndEmail orderPlaced = new OrderPlacedSmsAndEmail(System.Web.HttpContext.Current.Server);//Added for sms and email send by Sonali_04-01-2019
                    string CustomerOrdercode = db.CustomerOrders.Where(x => x.ID == OrderID).Select(x => x.OrderCode).FirstOrDefault();
                    if (pOrderViewModel.IsOnlinePayment == "1")
                    {

                        //=========== Tejaswee Change in new Project also ==================//
                        //This method check order wise earn if order wise earn is applicable then insert earn amount in respected table 
                        ReferAndEarn lReferAndEarn = new ReferAndEarn();
                        lReferAndEarn.GetReferAndEarnDetails(pOrderViewModel.CustomerOrder.UserLoginID, pOrderViewModel.CustomerOrder.PayableAmount, OrderID);

                        //Insert Subscription detail
                        // this.InsertSubscriptionDetail(result);
                        bool IsExpressBuy = false;
                        if (IsExpressBuy == true)
                        {
                            long UserID = pOrderViewModel.CustomerOrder.UserLoginID;
                            string MobileNo = db.UserLogins.Where(x => x.ID == UserID).Select(x => x.Mobile).FirstOrDefault();
                            if ("ExpressBuyCookie" != null)
                            {
                                string val = "ExpressBuyCookie";
                                string[] cookieVal = val.Split(',');
                                foreach (string item in cookieVal)
                                {
                                    if (item != string.Empty)
                                    {
                                        string[] indivItmDet = item.Split('$');
                                        if (City.CityName != null)
                                        {
                                            int qty = indivItmDet[2] != null ? Convert.ToInt32(indivItmDet[2]) : 0;
                                            TrackCartBusiness.InsertCartDetails(pOrderViewModel.CartId, qty, UserID, Convert.ToInt64(indivItmDet[0]), MobileNo, "CHECK_OUT", "", "", "Mobile", "", City.CityName, "", pOrderViewModel.FranchiseId);//--added by Ashish for multiple franchise in same city--//
                                            //TrackCartBusiness lTrackCartBusiness = new TrackCartBusiness();
                                            //  Cart lCart = lTrackCartBusiness.UpdateCart(pOrderViewModel.CartId.Value, OrderID, null, (int)ModelLayer.Models.Enum.ORDER_STATUS.PLACED);

                                        }
                                    }
                                }
                            }
                        }
                        else
                        {

                            long UserID = pOrderViewModel.CustomerOrder.UserLoginID;
                            string MobileNo = db.UserLogins.Where(x => x.ID == UserID).Select(x => x.Mobile).FirstOrDefault();
                            TrackCartBusiness lTrackCart = new TrackCartBusiness();

                            if (City.CityName != null)
                            {
                                foreach (var item in pOrderViewModel.CustomerOrderDetail)
                                    TrackCartBusiness.InsertCartDetails(pOrderViewModel.CartId, item.Qty, pOrderViewModel.CustomerOrder.UserLoginID, item.ShopStockID, "", "CHECK_OUT", "", "", "", "", City.CityName, "", pOrderViewModel.FranchiseId);////added params item.FranchiseID for Multiple MCO
                            }
                            TrackCartBusiness lTrackCartBusiness = new TrackCartBusiness();
                            Cart lCart = lTrackCartBusiness.UpdateCart(pOrderViewModel.CartId.Value, OrderID, null, (int)ModelLayer.Models.Enum.ORDER_STATUS.PLACED);

                            // TrackCartBusiness lTrackCartBusiness = new TrackCartBusiness();
                            //Cart lCart = lTrackCartBusiness.UpdateCart(pOrderViewModel.CartId.Value, OrderID, null, (int)ModelLayer.Models.Enum.ORDER_STATUS.PLACED);

                        }

                        //GetwayPaymentTransaction getwayPaymentTransaction = new ModelLayer.Models.GetwayPaymentTransaction();
                        //getwayPaymentTransaction.PaymentMode = "Online Payment";
                        //getwayPaymentTransaction.FromUID = pOrderViewModel.CustomerOrder.UserLoginID;
                        //getwayPaymentTransaction.ToUID = 1;
                        //getwayPaymentTransaction.PaymentGetWayTransactionId = txnid;
                        //if (pOrderViewModel.IsOnlinePayment == "1")
                        //{
                        //    getwayPaymentTransaction.Description = "PAYUMONEY";
                        //}
                        //else
                        //{
                        //    getwayPaymentTransaction.Description = "CCAVENUE";
                        //}
                        //getwayPaymentTransaction.IsActive = true;
                        //getwayPaymentTransaction.TransactionDate = DateTime.UtcNow;
                        //getwayPaymentTransaction.CreateDate = DateTime.UtcNow;
                        //getwayPaymentTransaction.Status = 0;
                        //getwayPaymentTransaction.CreateBy = 1;
                        //getwayPaymentTransaction.DeviceType = "Web";
                        //getwayPaymentTransaction.CustomerOrderID = OrderID;
                        //db.GetwayPaymentTransactions.Add(getwayPaymentTransaction);
                        //db.SaveChanges();
                        //orderPlaced.SendSMSToCustomer(pOrderViewModel.CustomerOrder.UserLoginID, OrderID);//Added for sms and email send by Sonali_04-01-2019
                        //orderPlaced.SendSMSToMerchant(pOrderViewModel.CustomerOrder.UserLoginID, OrderID);//Added for sms and email send by Sonali_04-01-2019
                        //orderPlaced.SendMailToCustomer(pOrderViewModel.CustomerOrder.UserLoginID, OrderID);//Added for sms and email send by Sonali_04-01-2019
                        //orderPlaced.SendMailToMerchant(pOrderViewModel.CustomerOrder.UserLoginID, OrderID);//Added for sms and email send by Sonali_04-01-2019
                        BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                        string surl = rcKey.LOCALIMG_PATH + "api/Thankyou";
                        string furl = rcKey.LOCALIMG_PATH + "api/FailedTransaction";
                        //string surl = this.GetUrl() + city + "/" + FranchiseID + "/PaymentProcess/Thankyou";////added "/" + FranchiseID by Ashish for multiple MCO in same city
                        //string furl = this.GetUrl() + city + "/" + FranchiseID + "/PaymentProcess/FailedTransaction";////added "/" + FranchiseID by Ashish for multiple MCO in same city
                        obj = new { Success = 1, Message = "You will receive an order confirmation on email & sms.", data = new { OrderID = OrderID, surl = surl, furl = furl, OrderNo = CustomerOrdercode } };
                    }
                    else
                    {
                        //added by Ashwini Meshram for Push Notification
                        int lStatus = 0;
                        string OrderStatus = "Placed";

                        if (OrderID != 0 && OrderID != null && orderViewModel.CustomerOrder.DeviceType == "Mobile" && pOrderViewModel.GcmRegID != null && pOrderViewModel.GcmRegID != "" && customerOrderDetail.OrderStatus == 1)
                        {
                            lCustOrder.SendAlertforPushNotification(OrderID, OrderStatus);
                            ////lStatus = lCustOrder.SendPushNotification(OrderID);
                            //lStatus = lCustOrder.AndroidPushFCM(OrderID);//FCM Notification Method called
                            //Yashaswi 2-7-2019
                            (new SendFCMNotification()).SendNotification("placed", OrderID);
                        }


                        //=========== Tejaswee Change in new Project also ==================//
                        //This method check order wise earn if order wise earn is applicable then insert earn amount in respected table 
                        ReferAndEarn lReferAndEarn = new ReferAndEarn();
                        lReferAndEarn.GetReferAndEarnDetails(pOrderViewModel.CustomerOrder.UserLoginID, pOrderViewModel.CustomerOrder.PayableAmount, OrderID);

                        //Insert Subscription detail
                        // this.InsertSubscriptionDetail(result);
                        bool IsExpressBuy = false;
                        if (IsExpressBuy == true)
                        {
                            long UserID = pOrderViewModel.CustomerOrder.UserLoginID;
                            string MobileNo = db.UserLogins.Where(x => x.ID == UserID).Select(x => x.Mobile).FirstOrDefault();
                            if ("ExpressBuyCookie" != null)
                            {
                                string val = "ExpressBuyCookie";
                                string[] cookieVal = val.Split(',');
                                foreach (string item in cookieVal)
                                {
                                    if (item != string.Empty)
                                    {
                                        string[] indivItmDet = item.Split('$');
                                        if (City.CityName != null)
                                        {
                                            int qty = indivItmDet[2] != null ? Convert.ToInt32(indivItmDet[2]) : 0;
                                            TrackCartBusiness.InsertCartDetails(pOrderViewModel.CartId, qty, UserID, Convert.ToInt64(indivItmDet[0]), MobileNo, "CHECK_OUT", "", "", "Mobile", "", City.CityName, "", pOrderViewModel.FranchiseId);//--added by Ashish for multiple franchise in same city--//
                                            TrackCartBusiness lTrackCartBusiness = new TrackCartBusiness();
                                            Cart lCart = lTrackCartBusiness.UpdateCart(pOrderViewModel.CartId.Value, OrderID, null, (int)ModelLayer.Models.Enum.ORDER_STATUS.PLACED);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {

                            long UserID = pOrderViewModel.CustomerOrder.UserLoginID;
                            string MobileNo = db.UserLogins.Where(x => x.ID == UserID).Select(x => x.Mobile).FirstOrDefault();
                            TrackCartBusiness lTrackCart = new TrackCartBusiness();

                            if (City.CityName != null)
                            {
                                foreach (var item in pOrderViewModel.CustomerOrderDetail)
                                    TrackCartBusiness.InsertCartDetails(pOrderViewModel.CartId, item.Qty, pOrderViewModel.CustomerOrder.UserLoginID, item.ShopStockID, "", "CHECK_OUT", "", "", "", "", City.CityName, "", pOrderViewModel.FranchiseId);////added params item.FranchiseID for Multiple MCO
                            }
                            TrackCartBusiness lTrackCartBusiness = new TrackCartBusiness();
                            Cart lCart = lTrackCartBusiness.UpdateCart(pOrderViewModel.CartId.Value, OrderID, null, (int)ModelLayer.Models.Enum.ORDER_STATUS.PLACED);

                        }
                        if (OrderID == 0)
                            obj = new { Success = 0, Message = "Please check the parameters there may be possibility that shop Ids sent in request is not available in database.", data = string.Empty };
                        //  return new { HTTPStatusCode = "500", UserMessage = "Internal server error", ServerError = "Please check the parameters there may be possibility that shop Ids sent in request is not available in database.", OrderID = 0 };
                        else
                        {
                            orderPlaced.SendSMSToCustomer(pOrderViewModel.CustomerOrder.UserLoginID, OrderID);//Added for sms and email send by Sonali_04-01-2019
                            orderPlaced.SendSMSToMerchant(pOrderViewModel.CustomerOrder.UserLoginID, OrderID);//Added for sms and email send by Sonali_04-01-2019
                            orderPlaced.SendMailToCustomer(pOrderViewModel.CustomerOrder.UserLoginID, OrderID);//Added for sms and email send by Sonali_04-01-2019
                            orderPlaced.SendMailToMerchant(pOrderViewModel.CustomerOrder.UserLoginID, OrderID);//Added for sms and email send by Sonali_04-01-2019
                            obj = new { Success = 1, Message = "You will receive an order confirmation on email & sms.", data = new { OrderID = OrderID, Status = lStatus, OrderNo = CustomerOrdercode } };
                        }
                        //obj = new { Success = 1, Message = "Order Placed Successfully.", data = new { OrderID = OrderID, Status = lStatus, OrderNo = CustomerOrdercode } };
                    }
                }
                // return new { HTTPStatusCode = "200", UserMessage = "Order Placed Successfully.", OrderID = OrderID, Status = lStatus };
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Placing Customer Order :" + ex.Message + ex.InnerException, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
                obj = new { Success = 0, Message = ex.InnerException, data = string.Empty };
            }
            return obj;
        }

        /*
         [TokenVerification]
         [ValidateModel]
         [ApiException] 
        // GET api/placeorder
        public long GET(int id)
        {

            APIOrderViewModel pOrderViewModel = new APIOrderViewModel();

            //BusinessLogicLayer.ErrorLog.ErrorLogFile("Enter " + pOrderViewModel.CustomerOrder.UserLoginID, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
            OrderViewModel orderViewModel = new OrderViewModel();
            ModelLayer.Models.CustomerOrder modelCustomerOrder = new ModelLayer.Models.CustomerOrder();

            // Set value to customer order
            //modelCustomerOrder.UserLoginID = pOrderViewModel.CustomerOrder.UserLoginID;
            modelCustomerOrder.UserLoginID = 277;           
            modelCustomerOrder.OrderAmount = 10000;
            //modelCustomerOrder.OrderAmount = pOrderViewModel.CustomerOrder.OrderAmount;
            modelCustomerOrder.NoOfPointUsed = 0;
            //modelCustomerOrder.NoOfPointUsed = pOrderViewModel.CustomerOrder.NoOfPointUsed;
            modelCustomerOrder.ValuePerPoint = 0;
            //modelCustomerOrder.ValuePerPoint = pOrderViewModel.CustomerOrder.ValuePerPoint;
            modelCustomerOrder.CoupenCode = string.Empty;
            //modelCustomerOrder.CoupenCode = pOrderViewModel.CustomerOrder.CoupenCode;
            modelCustomerOrder.CoupenAmount = 0;
            //modelCustomerOrder.CoupenAmount = pOrderViewModel.CustomerOrder.CoupenAmount;
            modelCustomerOrder.PAN = string.Empty;
            //modelCustomerOrder.PaymentMode = pOrderViewModel.CustomerOrder.PaymentMode;
            modelCustomerOrder.PaymentMode = "COD";
            //modelCustomerOrder.PayableAmount = pOrderViewModel.CustomerOrder.PayableAmount;
            modelCustomerOrder.PayableAmount = 10000;
            //modelCustomerOrder.PrimaryMobile = pOrderViewModel.CustomerOrder.PrimaryMobile;
            modelCustomerOrder.PrimaryMobile = "9503507124";
            //modelCustomerOrder.SecondoryMobile = pOrderViewModel.CustomerOrder.SecondoryMobile;
            //modelCustomerOrder.ShippingAddress = pOrderViewModel.CustomerOrder.ShippingAddress;
            modelCustomerOrder.ShippingAddress = "S Nagpur";
            //modelCustomerOrder.PincodeID = pOrderViewModel.CustomerOrder.PincodeID;
            modelCustomerOrder.PincodeID = 8304;
            //modelCustomerOrder.AreaID = pOrderViewModel.CustomerOrder.AreaID;
            modelCustomerOrder.CreateDate = CommonFunctions.GetLocalTime();

            orderViewModel.CustomerOrder = modelCustomerOrder;

            //BusinessLogicLayer.ErrorLog.ErrorLogFile("Second " + pOrderViewModel.CustomerOrder.UserLoginID, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);


            List<CustomerOrderDetail> CustomerOrderDetail = new List<ModelLayer.Models.CustomerOrderDetail>();
         
             CustomerOrderDetail customerOrderDetail = new CustomerOrderDetail();

            customerOrderDetail.ID = 0;
            customerOrderDetail.ShopOrderCode = string.Empty;
            customerOrderDetail.CustomerOrderID = 0;
            customerOrderDetail.ShopStockID = 16264;
            customerOrderDetail.ShopID = 221;
            customerOrderDetail.Qty = 1;
            customerOrderDetail.OrderStatus =1;
            customerOrderDetail.MRP = 500;
            customerOrderDetail.SaleRate = 500;
            customerOrderDetail.OfferPercent = 0;
            customerOrderDetail.OfferRs =0;
            customerOrderDetail.IsInclusivOfTax = true;
            customerOrderDetail.TotalAmount = 10000;          

            CustomerOrderDetail.Add(customerOrderDetail);

            customerOrderDetail = new CustomerOrderDetail();
            customerOrderDetail.ID = 0;
            customerOrderDetail.ShopOrderCode = string.Empty;
            customerOrderDetail.CustomerOrderID = 0;
            customerOrderDetail.ShopStockID = 1429;
            customerOrderDetail.ShopID = 197;
            customerOrderDetail.Qty = 1;
            customerOrderDetail.OrderStatus = 1;
            customerOrderDetail.MRP = 500;
            customerOrderDetail.SaleRate = 500;
            customerOrderDetail.OfferPercent = 0;
            customerOrderDetail.OfferRs = 0;
            customerOrderDetail.IsInclusivOfTax = true;
            customerOrderDetail.TotalAmount = 10000;

            CustomerOrderDetail.Add(customerOrderDetail);

            //customerOrderDetail = new CustomerOrderDetail();
            //customerOrderDetail.ID = 0;
            //customerOrderDetail.ShopOrderCode = string.Empty;
            //customerOrderDetail.CustomerOrderID = 0;
            //customerOrderDetail.ShopStockID = 9067;
            //customerOrderDetail.ShopID = 34;
            //customerOrderDetail.Qty = 1;
            //customerOrderDetail.OrderStatus = 1;
            //customerOrderDetail.MRP = 500;
            //customerOrderDetail.SaleRate = 500;
            //customerOrderDetail.OfferPercent = 0;
            //customerOrderDetail.OfferRs = 0;
            //customerOrderDetail.IsInclusivOfTax = true;
            //customerOrderDetail.TotalAmount = 10000;

            //CustomerOrderDetail.Add(customerOrderDetail);

            //foreach (var item in pOrderViewModel.CustomerOrderDetail)
            //{                
            //    customerOrderDetail.ID = 0;
            //    customerOrderDetail.ShopOrderCode = string.Empty;
            //    customerOrderDetail.CustomerOrderID = 0;
            //    customerOrderDetail.ShopStockID = item.ShopStockID;
            //    customerOrderDetail.ShopID = item.ShopID;
            //    customerOrderDetail.Qty = item.Qty;
            //    customerOrderDetail.OrderStatus = item.OrderStatus;
            //    customerOrderDetail.MRP = item.MRP;
            //    customerOrderDetail.SaleRate = item.SaleRate;
            //    customerOrderDetail.OfferPercent = item.OfferPercent;
            //    customerOrderDetail.OfferRs = item.OfferRs;
            //    customerOrderDetail.IsInclusivOfTax = item.IsInclusivOfTax;
            //    //customerOrderDetail.TotalAmount = 10000;
            //    customerOrderDetail.TotalAmount = item.Qty*item.SaleRate;

            //    CustomerOrderDetail.Add(customerOrderDetail);

            //}

            //BusinessLogicLayer.ErrorLog.ErrorLogFile("Third " + pOrderViewModel.CustomerOrder.UserLoginID, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);


            orderViewModel.CustomerOrderDetail = CustomerOrderDetail;
        
             List<ShopWiseDeliveryCharges> sDelcharges = new List<ShopWiseDeliveryCharges>();
             sDelcharges.Add(new ShopWiseDeliveryCharges { DeliveryCharge = 50, DeliveryType = "Normal", OrderAmount = 500, ShopID = 221, Weight = 0 });
             sDelcharges.Add(new ShopWiseDeliveryCharges { DeliveryCharge = 50, DeliveryType = "Normal", OrderAmount = 500, ShopID = 197, Weight = 0 });

            orderViewModel.shopWiseDeliveryCharges = sDelcharges;
            //orderViewModel.shopWiseDeliveryCharges = pOrderViewModel.shopWiseDeliveryCharges;
            //BusinessLogicLayer.ErrorLog.ErrorLogFile("forth " + pOrderViewModel.CustomerOrder.UserLoginID, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);

            BusinessLogicLayer.CustomerOrder lCustOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
            return lCustOrder.PlaceCustomerOrder(orderViewModel);

        }
       */
        // [ApiException] 
        //// Get api/placeorder
        //public void get()
        //{

        //    OrderViewModel orderViewModel = new OrderViewModel();

        //    ModelLayer.Models.CustomerOrder modelCustomerOrder = new ModelLayer.Models.CustomerOrder();

        //    // Set value to customer order

        //    modelCustomerOrder.ID = 0;
        //    modelCustomerOrder.OrderCode = string.Empty;
        //    modelCustomerOrder.UserLoginID = 3;
        //    //modelCustomerOrder.OrderAmount = 10000;
        //    modelCustomerOrder.OrderAmount = 5000;
        //    //modelCustomerOrder.NoOfPointUsed = 0;
        //    modelCustomerOrder.NoOfPointUsed = 0;
        //    //modelCustomerOrder.ValuePerPoint = 0;
        //    modelCustomerOrder.ValuePerPoint = 0;
        //    //modelCustomerOrder.CoupenCode = string.Empty;
        //    modelCustomerOrder.CoupenCode = "";
        //    //modelCustomerOrder.CoupenAmount = 0;
        //    modelCustomerOrder.CoupenAmount = 0;
        //    modelCustomerOrder.PAN = string.Empty;
        //    modelCustomerOrder.PaymentMode = "COD";
        //    //modelCustomerOrder.PayableAmount = 10000;
        //    modelCustomerOrder.PayableAmount = 50000;
        //    modelCustomerOrder.PrimaryMobile = "9503507124";
        //    modelCustomerOrder.SecondoryMobile = "";
        //    modelCustomerOrder.ShippingAddress = "Nagpur";
        //    modelCustomerOrder.PincodeID = 8304;
        //    modelCustomerOrder.AreaID = null;
        //    modelCustomerOrder.CreateDate = DateTime.UtcNow;

        //    orderViewModel.CustomerOrder = modelCustomerOrder;
        //    List<CustomerOrderDetail> CustomerOrderDetail = new List<ModelLayer.Models.CustomerOrderDetail>();

        //    ModelLayer.Models.CustomerOrderDetail customerOrderDetail = new CustomerOrderDetail();

        //    customerOrderDetail.ID = 0;
        //    customerOrderDetail.ShopOrderCode = string.Empty;
        //    customerOrderDetail.CustomerOrderID = 0;
        //    customerOrderDetail.ShopStockID = 37;
        //    customerOrderDetail.ShopID = 11;
        //    customerOrderDetail.Qty = 1;
        //    customerOrderDetail.OrderStatus = 1;
        //    customerOrderDetail.MRP = 50000;
        //    customerOrderDetail.SaleRate = 50000;
        //    customerOrderDetail.OfferPercent = 0;
        //    customerOrderDetail.OfferRs = 0;
        //    customerOrderDetail.IsInclusivOfTax = true;
        //    //customerOrderDetail.TotalAmount = 10000;
        //    customerOrderDetail.TotalAmount = 1 * 50000;

        //    CustomerOrderDetail.Add(customerOrderDetail);

        //    orderViewModel.CustomerOrderDetail = CustomerOrderDetail;

        //    // List of shipping charges

        //    List<ShopWiseDeliveryCharges> listShopWiseDeliveryCharges = new List<ShopWiseDeliveryCharges>();

        //    //for (int i = 0; i < 2; i++)
        //    //{
        //    ShopWiseDeliveryCharges shopWiseDeliveryCharge = new ShopWiseDeliveryCharges();

        //    shopWiseDeliveryCharge.ShopID = 10;
        //    shopWiseDeliveryCharge.Weight = 1;
        //    shopWiseDeliveryCharge.OrderAmount = 10000;
        //    shopWiseDeliveryCharge.DeliveryCharge = 200;
        //    shopWiseDeliveryCharge.DeliveryType = "Express";
        //    shopWiseDeliveryCharge.ShopOrderCode = string.Empty;

        //    listShopWiseDeliveryCharges.Add(shopWiseDeliveryCharge);
        //    //}

        //    orderViewModel.shopWiseDeliveryCharges = listShopWiseDeliveryCharges;



        //    BusinessLogicLayer.CustomerOrder lCustOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
        //    lCustOrder.PlaceCustomerOrder(orderViewModel);

        //}

        /// <summary>
        /// Cancel order by authorized  customer
        /// </summary>
        /// <param name="orderID">Order Id</param>
        /// <param name="shopStockID">Shop Stock ID</param>
        /// <param name="lCustLoginID">Customer Login ID</param>
        /// <returns>Operation Status</returns>
        //[TokenVerification]
        [ApiException]
        [ValidateModel]
        // DELETE api/placeorder/5
        public object Delete(long orderID, long shopStockID, long lCustLoginID)
        {
            BusinessLogicLayer.CustomerOrder lCustOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
            int oprStatus = lCustOrder.CancelCustomerOrder(orderID, shopStockID, lCustLoginID, "Cancelled ORder By APP");
            object obj = new object();
            if (oprStatus == 103)
            {
                if ((db.CustomerOrders.FirstOrDefault(p => p.ID == orderID)).MLMAmountUsed > 0)
                {
                    lCustOrder.Insert_RefundRequest_EwalletRefund(orderID);//Added by Rumana on 19/04/2019  

                    lCustOrder.Send_EWalletRefund_Mail(orderID, false);
                }
                obj = new { HTTPStatusCode = "200", UserMessage = "Order has been cancelled successfully." };
            }
            if (oprStatus == 500)
                obj = new { HTTPStatusCode = "500", UserMessage = "Internal server error." };
            if (oprStatus == 106)
                obj = new { HTTPStatusCode = "400", UserMessage = "Invalid request. The order you want to cancel is not present for given customer login ID." };

            return obj;

        }

        /// <summary>
        /// Update order status from Pending to Placed from App(mobile), when Online is successful.
        /// By Ashish
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="lCustLoginID"></param>
        /// <returns></returns>
        [TokenVerification]
        [ApiException]
        [ValidateModel]
        // DELETE api/placeorder/5
        public object Put(long orderID, long lCustLoginID)
        {
            object obj = new object();
            try
            {
                if (orderID <= 0 || lCustLoginID <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid data.", data = string.Empty };
                }
                int lStatus = 0;
                string OrderStatus = "Placed";
                BusinessLogicLayer.CustomerOrder lCustOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                int oprStatus = lCustOrder.UpdatePendingCustomerOrder(orderID, lCustLoginID, "Update Order status from Pending to Placed By APP");

                if (oprStatus == 103)
                {
                    lCustOrder.SendAlertforPushNotification(orderID, OrderStatus);
                    ////lStatus = lCustOrder.SendPushNotification(orderID);
                    //lStatus = lCustOrder.AndroidPushFCM(orderID); //FCM Notification method called
                    obj = new { Success = 1, Message = "Order has been Update successfully.", data = new { Status = lStatus } };
                }
                if (oprStatus == 500)
                    obj = new { Success = 0, Message = "Internal server error.", string.Empty };
                // obj = new { HTTPStatusCode = "500", UserMessage = "Internal server error." };
                if (oprStatus == 106)
                    obj = new { Success = 0, Message = "Invalid request. The order you want to Update is not present for given customer login ID.", string.Empty };
                // obj = new { HTTPStatusCode = "400", UserMessage = "Invalid request. The order you want to Update is not present for given customer login ID." };

            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }

            return obj;

        }

        //  [System.Web.Routing.Route("api/PlaceOrder/FailedTransaction")]
        //public object FailedTransaction(FormCollection frm)
        //{
        //    object obj = new object();
        //    try
        //    {
        //        string Error = frm["error"].ToString();
        //        string ErrorMsg = frm["error_Message"].ToString();
        //        obj = new { Success = 0, Message = Error, data = ErrorMsg };
        //    }
        //    catch (Exception ex)
        //    {
        //        obj = new { Success = 0, Message = ex.Message, data = string.Empty };
        //    }
        //    return obj;
        //    // return RedirectToRoute("PurchaseFailure", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]) });////added by Ashish for multiple MCO in same city

        //}


        private List<CalculatedTaxList> GetTaxCalCulatedList(List<CalulatedTaxesRecord> lCalulatedTaxesRecord, List<TaxList> lTaxList, int qty)
        {
            try
            {
                List<CalculatedTaxList> listCalculatedTaxList = new List<CalculatedTaxList>();
                foreach (var item in lCalulatedTaxesRecord)
                {
                    foreach (var item1 in lTaxList)
                    {
                        if (item.TaxPrefix == item1.TaxPrefix)
                        {
                            CalculatedTaxList lCalculatedTaxList = new CalculatedTaxList();
                            lCalculatedTaxList.TaxName = item1.TaxPrefix;
                            lCalculatedTaxList.Amount = item.TaxableAmount * qty;
                            lCalculatedTaxList.IsGSTInclusive = item.IsGSTInclusive;
                            listCalculatedTaxList.Add(lCalculatedTaxList);
                        }
                    }

                }
                return listCalculatedTaxList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private List<TaxList> GetTaxMasterList()
        {
            try
            {
                List<TaxList> lTaxList = new List<TaxList>();
                lTaxList = (from tx in db.TaxationMasters

                            group tx by new { tx.Prefix } into g
                            select new TaxList
                            {
                                TaxPrefix = g.Key.Prefix
                            }).ToList();
                return lTaxList;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public string Generatetxnid()
        {

            Random rnd = new Random();
            string x = rnd.Next(Int32.MaxValue).ToString();
            string strHash = Generatehash512(x + DateTime.Now);
            string txnid1 = strHash.ToString().Substring(0, 20);
            return txnid1;
        }
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


    }
}
