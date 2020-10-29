//-----------------------------------------------------------------------
// <copyright file="CustomerLogin.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Ashish Nagrale</author>
//-----------------------------------------------------------------------
using ModelLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace BusinessLogicLayer
{
    //---------------------------------------Hide EPOD from Ashish for Live----------------------------------------------------------
   
    //public class AssignmentDeliveryCharge
    //{
    //    public string ShopOrderCode { get; set; }
    //    public Decimal DeliveryCharge { get; set; }
    //}
    //public class Assignment
    //{
    //    public static List<EmployeeAssignmentList> GetAssignList(int userloginId)
    //    {
    //        //*************** Top Data *****************//
    //        EzeeloDBContext dbgb=new EzeeloDBContext();
    //       // EmployeeAssignmentList empAssignList = new EmployeeAssignmentList();
    //        List<EmployeeAssignmentList> empAssignList = new List<EmployeeAssignmentList>();
    //        var queryResult = (from DBA in dbgb.DeliveryBoyAttendances
    //                           join E in dbgb.Employees on DBA.UserLoginID equals E.UserLoginID
    //                           //join PD in dbgb.PersonalDetails on E.UserLoginID equals PD.UserLoginID
    //                           join EA in dbgb.EmployeeAssignment on E.EmployeeCode equals EA.EmployeeCode
    //                           join CO in dbgb.CustomerOrders on EA.OrderCode equals CO.OrderCode
    //                           join otp in dbgb.OTPs on EA.ShopOrderCode equals otp.ShopOrderCode into LOJ
    //                           from otp in LOJ.DefaultIfEmpty()//-- For Left Outer Join --//
    //                           where DBA.UserLoginID == userloginId && (EA.OrderStatus >= 3 && EA.OrderStatus <= 6)//OrderStatus<=6 will remove when task assign for return order after delivery.
    //                            && EA.IsActive==true && DBA.LoginDateTime == (from DBAA in dbgb.DeliveryBoyAttendances where DBAA.UserLoginID == userloginId select DBAA.LoginDateTime).Max()
    //                               && DBA.LogoutDateTime == null
    //                           select new EmployeeAssignmentList  // Top Data
    //                           {
    //                               ID = EA.ID,
    //                               OrderCode = EA.OrderCode,
    //                               ShopOrderCode = EA.ShopOrderCode,
    //                               GodownCode = EA.GodownCode,
    //                               EmployeeCode = EA.EmployeeCode,
    //                               OrderStatus = EA.OrderStatus,
    //                               Name = (from COO in dbgb.CustomerOrders join UL in dbgb.UserLogins on COO.UserLoginID equals UL.ID join PD in dbgb.PersonalDetails on COO.UserLoginID equals PD.UserLoginID where COO.OrderCode == CO.OrderCode select PD.FirstName != null ? PD.FirstName: ""  + " " + PD.MiddleName != null ? PD.MiddleName: "" + " " + PD.LastName != null ?  PD.LastName:"" ).FirstOrDefault(), //--Customer Name: --// PD.FirstName + " " + PD.LastName,
    //                               FromAddress = EA.FromAddress,// Merchant OR Godown
    //                               DeliveredType = EA.DeliveredType,
    //                               DeliveryType = EA.DeliveryType,
    //                               DeliveryDate = EA.DeliveryDate,
    //                               DeliverySchedule = EA.DeliverySchedule,
    //                               //DeliveredTime = EA.DeliveredTime,
    //                               ToAddress = EA.ToAddress,//Godown OR Customer
    //                               X = EA.X,
    //                               Y = EA.Y,
    //                               PrimaryMobile = CO.PrimaryMobile == null ? "No Contact" : CO.PrimaryMobile, // Customer Mobile
    //                               SecondaryMobile = CO.SecondoryMobile == null ? "No Contact" : CO.SecondoryMobile, // Customer Alternate Mobile
    //                               PaymentMode = CO.PaymentMode,
    //                               Amount = otp.PayableAmount == null ? 0 : otp.PayableAmount
    //                               //MerchantCopy = MC,
    //                               //CustomerCopy = CC

    //                           }
    //                   ).OrderByDescending(x => x.OrderCode).ToList();

    //        foreach (var lst in queryResult){
    //           // string name = (from CO in dbgb.CustomerOrders join UL in dbgb.UserLogins on CO.UserLoginID equals UL.ID join PD in dbgb.PersonalDetails on CO.UserLoginID equals PD.UserLoginID where CO.OrderCode == lst.OrderCode select PD.FirstName + " " + PD.MiddleName +" " + PD.LastName).FirstOrDefault();
    //           // lst.Name = name;

    //            //----------------------------------------------------------------------------------------------------------------------
    //            if ((lst.OrderStatus >= 3 && lst.OrderStatus <= 4) && lst.DeliveredType == "PICKUP")// For Merchant Copy
    //            {
    //                var ID = dbgb.DeliveryOrderDetails.Where(x => x.ShopOrderCode == lst.ShopOrderCode).Select(x=>x.ID).FirstOrDefault() ;
    //                DeliveryOrderDetail deliveryorderdetail = new DeliveryOrderDetail();
    //                deliveryorderdetail = dbgb.DeliveryOrderDetails.Find(ID);

    //                CustomerOrderDetail lCustomerOrderDetail = new CustomerOrderDetail();
    //                lCustomerOrderDetail = dbgb.CustomerOrderDetails.FirstOrDefault(x => x.ShopOrderCode == lst.ShopOrderCode);

    //                ModelLayer.Models.CustomerOrder lCustomerOrder = new ModelLayer.Models.CustomerOrder();
    //                lCustomerOrder = dbgb.CustomerOrders.Find(lCustomerOrderDetail.CustomerOrderID);

    //                PersonalDetail lPersonalDetail = new PersonalDetail();
    //                lPersonalDetail = dbgb.PersonalDetails.FirstOrDefault(x => x.UserLoginID == userloginId);
    //                try
    //                {
    //                    //****************** AssignedTrackOrderReceive Data ****************//
    //                    EzeeloDBContext dbtrack = new EzeeloDBContext();
    //                    List<BusinessLogicLayer.AssignedTrackOrderReceive> TrackOrderViewModels = new List<BusinessLogicLayer.AssignedTrackOrderReceive>();
    //                    TrackOrderViewModels = (
    //                        from CO in dbtrack.CustomerOrders
    //                        join COD in dbtrack.CustomerOrderDetails on CO.ID equals COD.CustomerOrderID
    //                        join DOD in dbtrack.DeliveryOrderDetails on COD.ShopOrderCode equals DOD.ShopOrderCode
    //                        join SS in dbtrack.ShopStocks on COD.ShopStockID equals SS.ID
    //                        join U in dbtrack.Units on SS.PackUnitID equals U.ID
    //                        join SP in dbtrack.ShopProducts on SS.ShopProductID equals SP.ID
    //                        join P in dbtrack.Products on SP.ProductID equals P.ID
    //                        join UL in dbtrack.UserLogins on CO.UserLoginID equals UL.ID
    //                        join PD in dbtrack.PersonalDetails on CO.UserLoginID equals PD.UserLoginID
    //                        where COD.ShopOrderCode == lCustomerOrderDetail.ShopOrderCode
    //                        select new BusinessLogicLayer.AssignedTrackOrderReceive //AssignedTrackOrderReceive Data
    //                        {
    //                            OrderCode = CO.OrderCode,
    //                            ID = COD.ID,
    //                            ShopOrderCode = COD.ShopOrderCode,
    //                            ShopStockID = SS.ID,
    //                            ShopID = SP.ShopID,
    //                            CustomerOrderID = COD.CustomerOrderID,
    //                            FirstName = PD.FirstName,//Customer Name
    //                            MiddleName = PD.MiddleName,
    //                            LastName = PD.LastName,
    //                            CreateDate = CO.CreateDate,
    //                            Name = P.Name,
    //                            PackSize = SS.PackSize,
    //                            PackUnitName = U.Name,
    //                            CODQty = COD.Qty,
    //                            SaleRate = COD.SaleRate,
    //                            RetailerRate = SS.RetailerRate,
    //                            TotalAmount = COD.TotalAmount,
    //                            OrderAmount = CO.OrderAmount,
    //                            GandhibaghCharge = DOD.GandhibaghCharge,
    //                            PayableAmount = CO.PayableAmount,
    //                            ProductID = P.ID
    //                        }).ToList();

    //                    long lShopId = TrackOrderViewModels.FirstOrDefault().ShopID;
    //                    Shop lShop = dbgb.Shops.Find(lShopId);


    //                    //-- Start Tax Include on 31-march-2016 , By Avi Verma. 
    //                    List<AssignmentTaxOnOrder> lTaxOnOrder = new List<AssignmentTaxOnOrder>();
    //                    lst.TaxOnOrder = new List<AssignmentTaxOnOrder>();//Added on 8-8-2016
    //                        lTaxOnOrder = (from tovm in TrackOrderViewModels
    //                                                                       join too in dbgb.TaxOnOrders on tovm.ID equals too.CustomerOrderDetailID
    //                                                                       join PrdTax in dbgb.ProductTaxes on too.ProductTaxID equals PrdTax.ID
    //                                                                       join taxMas in dbgb.TaxationMasters on PrdTax.TaxID equals taxMas.ID
    //                                                                        select new AssignmentTaxOnOrder
    //                                                                       {
    //                                                                           TaxOnOrderID = too.ID,
    //                                                                           CustomerOrderDetailID = tovm.ID,
    //                                                                           ProductTaxID = PrdTax.ID,
    //                                                                           TaxAmount = too.Amount,
    //                                                                           TaxID = PrdTax.TaxID,
    //                                                                           TaxPrefix = taxMas.Prefix,
    //                                                                           TaxName = taxMas.Name
    //                                                                       }).ToList();
    //                        ////AssignmentTaxOnOrder lAssignmentTaxOnOrder = new AssignmentTaxOnOrder();//Hide on 8-8-2016
    //                        if (lTaxOnOrder.Count() != 0)
    //                        {
    //                            foreach (var lTax in lTaxOnOrder) //Added on 8-8-2016 working
    //                            {
    //                                //******************** Merchant Tax Data **********************//

    //                                //lAssignmentTaxOnOrder.TaxOnOrderID = lTaxOnOrder.FirstOrDefault().TaxOnOrderID;
    //                                //lAssignmentTaxOnOrder.CustomerOrderDetailID = lTaxOnOrder.FirstOrDefault().CustomerOrderDetailID;
    //                                //lAssignmentTaxOnOrder.ProductTaxID = lTaxOnOrder.FirstOrDefault().ProductTaxID;
    //                                //lAssignmentTaxOnOrder.TaxAmount = lTaxOnOrder.FirstOrDefault().TaxAmount;
    //                                //lAssignmentTaxOnOrder.TaxID = lTaxOnOrder.FirstOrDefault().TaxID;
    //                                //lAssignmentTaxOnOrder.TaxPrefix = lTaxOnOrder.FirstOrDefault().TaxPrefix;
    //                                //lAssignmentTaxOnOrder.TaxName = lTaxOnOrder.FirstOrDefault().TaxName;

    //                                //-----------New 8-8-2016-----------------------//
    //                                AssignmentTaxOnOrder lAssignmentTaxOnOrder = new AssignmentTaxOnOrder(); //Added on 8-8-2016
    //                                lAssignmentTaxOnOrder.TaxOnOrderID = lTax.TaxOnOrderID;
    //                                lAssignmentTaxOnOrder.CustomerOrderDetailID = lTax.CustomerOrderDetailID;
    //                                lAssignmentTaxOnOrder.ProductTaxID = lTax.ProductTaxID;
    //                                lAssignmentTaxOnOrder.TaxAmount = lTax.TaxAmount;
    //                                lAssignmentTaxOnOrder.TaxID = lTax.TaxID;
    //                                lAssignmentTaxOnOrder.TaxPrefix = lTax.TaxPrefix;
    //                                lAssignmentTaxOnOrder.TaxName = lTax.TaxName;
    //                                lst.TaxOnOrder.Add(lAssignmentTaxOnOrder);
    //                                //-----------New End 8-8-2016-----------------------//

    //                            }
    //                        }

    //                    //-- End Tax Include on 31-march-2016 , By Avi Verma. 



    //                    //********************** Merchant Copy Data ***********************//
    //                    AssignmentReceiveDetail lAssignmentReceiveDetail = new AssignmentReceiveDetail();

    //                    lAssignmentReceiveDetail.ID = deliveryorderdetail.ID;
    //                    lAssignmentReceiveDetail.ShopOrderCode = deliveryorderdetail.ShopOrderCode;
    //                    lAssignmentReceiveDetail.DeliveryCharge = deliveryorderdetail.DeliveryCharge;
    //                    //----Changes By Mohit------------on 06-18-2015----------------------------//
    //                    lAssignmentReceiveDetail.GandhibaghCharge = deliveryorderdetail.GandhibaghCharge;
    //                    //----End Changes By Mohit------------on 06-18-2015----------------------------//

    //                    lAssignmentReceiveDetail.ShopName = lShop.Name; //Merchant Shop Name
    //                    lAssignmentReceiveDetail.PickUpName = lShop.ContactPerson;// Merchant Name
    //                    lAssignmentReceiveDetail.PickUpAddress = lShop.Address;// Merchant Address
    //                    lAssignmentReceiveDetail.PickUpContact = lShop.Mobile;// Merchant Contact
    //                    lAssignmentReceiveDetail.PickUpAlternateContact = lShop.Landline == null ? "No Address" : lShop.Landline;// Merchant Alternate Contact

    //                    lAssignmentReceiveDetail.DeliverToName = lPersonalDetail.Salutation.Name + ". " + lPersonalDetail.FirstName;//Delivery Boy name as per userloginid
    //                    lAssignmentReceiveDetail.DeliverToEmail = lCustomerOrder.UserLogin.Email; // Customer email
    //                    lAssignmentReceiveDetail.DeliverToAddress = lCustomerOrder.ShippingAddress; // Customer Address
    //                    lAssignmentReceiveDetail.DeliveryToContact = lst.PrimaryMobile == null ? "No Contact" : lst.PrimaryMobile;//lCustomerOrder.PrimaryMobile; // Customer Contact
    //                    lAssignmentReceiveDetail.DeliveryToAlternateContact = lst.SecondaryMobile == null ? "No Contact" : lst.SecondaryMobile;// lCustomerOrder.SecondoryMobile; // Customer Alternate Contact
    //                    //----Changes By Mohit------------on 19-11-2015----------------------------//
    //                    lAssignmentReceiveDetail.GandhibaghOrderCode = lst.OrderCode;// lCustomerOrder.OrderCode;
    //                    //----End Changes By Mohit------------on 19-11-2015----------------------------//


    //                    lAssignmentReceiveDetail.AssignedTrackOrderReceive = TrackOrderViewModels;

    //                    //------------------- new fields added on 08-sep-2015.
    //                    //-- Changes made by AVI VERMA.
    //                    //-- For Getting Mode of Payment. 
    //                    //-- Reason : - If payment mode is online. then, for display on delivery memo.. COD = 0 Rs. As it is paid online.
    //                    lAssignmentReceiveDetail.PaymentMode = lCustomerOrder.PaymentMode;
    //                    ////////////////////////////////////////////////////////
    //                    //-----------------------
    //                    lst.MerchantCopy = new List<AssignmentReceiveDetail>();
    //                    lst.MerchantCopy.Add(lAssignmentReceiveDetail);

    //                    /* if (lTaxOnOrder.Count() != 0) //Hide on 8-8-2016
    //                     {
    //                         lst.TaxOnOrder = new List<AssignmentTaxOnOrder>();
    //                         lst.TaxOnOrder.Add(lAssignmentTaxOnOrder);
    //                     }*/
    //                    //-----------------------
    //                }
    //                catch (DbEntityValidationException ex)
    //                {
    //                    // Retrieve the error messages as a list of strings.
    //                    var errorMessages = ex.EntityValidationErrors
    //                            .SelectMany(x => x.ValidationErrors)
    //                            .Select(x => x.ErrorMessage);

    //                    // Join the list to a single string.
    //                    var fullErrorMessage = string.Join("; ", errorMessages);

    //                    // Combine the original exception message with the new one.
    //                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

    //                    // Throw a new DbEntityValidationException with the improved exception message.
    //                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
    //                }
    //            }
    //            else if ((lst.OrderStatus >= 5 && lst.OrderStatus <= 6) && lst.DeliveredType == "DELIVERY")// For Customer Copy
    //            {
    //                var customerorderid = dbgb.CustomerOrders.Where(x => x.OrderCode == lst.OrderCode).Select(x => x.ID).FirstOrDefault();
    //               var customerorderdetails = dbgb.CustomerOrderDetails.Include(c => c.CustomerOrder).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.Shop).Include(c => c.ShopStock).Where(x => x.CustomerOrderID == customerorderid &&(x.OrderStatus>=5 && x.OrderStatus<=6)).OrderByDescending(x => x.ID).ToList();

    //                //********************** Customer Copy Data ***************************//
    //                List<AssignmentDeliveryDetail> lAssignDeliveryDetail = new List<AssignmentDeliveryDetail>();
    //               lAssignDeliveryDetail = (from cod in dbgb.CustomerOrderDetails
    //                                        join otp in dbgb.OTPs on cod.ShopOrderCode equals otp.ShopOrderCode
    //                                        join co in dbgb.CustomerOrders on cod.CustomerOrderID equals co.ID
    //                                        join pd in dbgb.PersonalDetails on co.UserLoginID equals pd.UserLoginID
    //                                        join ss in dbgb.ShopStocks on cod.ShopStockID equals ss.ID
    //                                        join sp in dbgb.ShopProducts on ss.ShopProductID equals sp.ID
    //                                        join s in dbgb.Shops on sp.ShopID equals s.ID
    //                                        where cod.CustomerOrderID==(long)customerorderid
    //                                        select new AssignmentDeliveryDetail // Customer Copy Data
    //                                        {
    //                                            ID=cod.ID,
    //                                           OrderNo=co.OrderCode,
    //                                           OrderDate=co.CreateDate,
    //                                             CustomerName=pd.FirstName != null ? pd.FirstName: ""  + " " + pd.MiddleName != null ? pd.MiddleName: "" + " " + pd.LastName != null ?  pd.LastName:"",//--Customer Name: --// pd.FirstName +" "+pd.MiddleName +" "+pd.LastName,
    //                                             Address=co.ShippingAddress,
    //                                             Pincode = co.Pincode.Name,
    //                                             Area=co.Area.Name,
    //                                             Mobile=co.PrimaryMobile+","+co.SecondoryMobile+","+co.UserLogin.Mobile,
    //                                             OTP=otp.OTP1,
    //                                            OTPAmount = otp.PayableAmount
    //                                        }
    //                           ).Distinct().ToList();
                 

    //              decimal lTotalDeliveryCharge = 0;

    //              List<AssignmentDeliveryCharge> lAssignmentDeliveryCharge = new List<AssignmentDeliveryCharge>();
    //              lAssignmentDeliveryCharge = (from dod in dbgb.DeliveryOrderDetails
    //                                          join cod in dbgb.CustomerOrderDetails on dod.ShopOrderCode equals cod.ShopOrderCode
    //                                          join co in dbgb.CustomerOrders on cod.CustomerOrderID equals co.ID
    //                                          where co.ID == customerorderid
    //                                          select new AssignmentDeliveryCharge
    //                                          {
    //                                              ShopOrderCode = dod.ShopOrderCode,
    //                                              DeliveryCharge = dod.GandhibaghCharge,
    //                                          }).Distinct().ToList();


    //              decimal lDeliveryCharge = lAssignmentDeliveryCharge.Sum(x => x.DeliveryCharge);

    //                int countItem=1;
    //               // decimal lTotalAmt = 0;
    //               // decimal lTotalMRP = 0;
    //                decimal lTotalSave = 0;
    //                AssignedTrackOrderDelivery lAssignedTrackOrderDelivery = new AssignedTrackOrderDelivery();
    //                AssignedDeliveryOrderList lAssignedDeliveryOrderList = new AssignedDeliveryOrderList();
    //                lAssignedTrackOrderDelivery.AssignedDeliveryOrderList = new List<AssignedDeliveryOrderList>();
    //                List<AssignmentTaxOnOrder> lTaxOnOrder = new List<AssignmentTaxOnOrder>();//Added
    //                lst.TaxOnOrder = new List<AssignmentTaxOnOrder>();//Added
    //                foreach (var Clst in customerorderdetails)
    //                {
    //                    //********************* AssignedDeliveryOrderList Data ***************//
    //                    AssignedDeliveryOrderList lAssignedDOList = new AssignedDeliveryOrderList();
    //                    lAssignedDOList.ID = Clst.ID;
    //                    lAssignedDOList.SrNo = countItem++;
    //                    lAssignedDOList.Particular = Clst.ShopStock.ShopProduct.Product.Name + " " + Clst.ShopStock.ProductVarient.Size.Name;
    //                    lAssignedDOList.MRP = (Clst.Qty * Clst.MRP);// lTotalMRP + (Clst.Qty * Clst.MRP);
    //                    lAssignedDOList.Rate = (Clst.Qty * Clst.SaleRate);// lTotalMRP + (Clst.Qty * Clst.SaleRate);
    //                    lAssignedDOList.Qty = Clst.Qty;
    //                    lAssignedDOList.Saving = (Clst.Qty * Clst.MRP) - (Clst.Qty * Clst.SaleRate);
    //                    lAssignedDOList.Amount = Clst.TotalAmount;
    //                    lAssignedDOList.PaymentMode = Clst.CustomerOrder.PaymentMode == "ONLINE" ? "PAID ONLINE" : Clst.CustomerOrder.PaymentMode;

    //                    lAssignedDeliveryOrderList.PaymentMode = Clst.CustomerOrder.PaymentMode == "ONLINE" ? "PAID ONLINE" : Clst.CustomerOrder.PaymentMode;//+ " " + Clst.CustomerOrder.PayableAmount;
    //                   // lTotalAmt=lTotalAmt+ Clst.TotalAmount;
    //                    lTotalSave= lTotalSave + ((Clst.Qty * Clst.MRP) - (Clst.Qty * Clst.SaleRate));

    //                    lAssignedTrackOrderDelivery.AssignedDeliveryOrderList.Add(lAssignedDOList);
    //                    //-------------------------------------------------------------------------
    //                    //-- Start Tax Include on 31-march-2016 , By Avi Verma. 
    //                   // List<AssignmentTaxOnOrder> lTaxOnOrder = new List<AssignmentTaxOnOrder>(); //hide
    //                    lTaxOnOrder = (from cod in customerorderdetails
    //                                   join too in dbgb.TaxOnOrders on cod.ID equals too.CustomerOrderDetailID
    //                                   join PrdTax in dbgb.ProductTaxes on too.ProductTaxID equals PrdTax.ID
    //                                   join taxMas in dbgb.TaxationMasters on PrdTax.TaxID equals taxMas.ID
    //                                   where too.CustomerOrderDetailID == Clst.ID
    //                                   select new AssignmentTaxOnOrder
    //                                   {
    //                                       TaxOnOrderID = too.ID,
    //                                       CustomerOrderDetailID = Clst.ID,//cod.ID,
    //                                       ProductTaxID = PrdTax.ID,
    //                                       TaxAmount = too.Amount,
    //                                       TaxID = PrdTax.TaxID,
    //                                       TaxPrefix = taxMas.Prefix,
    //                                       TaxName = taxMas.Name
    //                                   }).ToList();
    //                    AssignmentTaxOnOrder lAssignmentTaxOnOrder = new AssignmentTaxOnOrder();
    //                    if (lTaxOnOrder.Count() != 0)
    //                    {
    //                        //**************** Customer Tax Data **********************//
    //                        lAssignmentTaxOnOrder.TaxOnOrderID = lTaxOnOrder.FirstOrDefault().TaxOnOrderID;
    //                        lAssignmentTaxOnOrder.CustomerOrderDetailID = lTaxOnOrder.FirstOrDefault().CustomerOrderDetailID;
    //                        lAssignmentTaxOnOrder.ProductTaxID = lTaxOnOrder.FirstOrDefault().ProductTaxID;
    //                        lAssignmentTaxOnOrder.TaxAmount = lTaxOnOrder.Sum(x => x.TaxAmount);//.FirstOrDefault().TaxAmount;
    //                        lAssignmentTaxOnOrder.TaxID = lTaxOnOrder.FirstOrDefault().TaxID;
    //                        lAssignmentTaxOnOrder.TaxPrefix = lTaxOnOrder.FirstOrDefault().TaxPrefix;
    //                        lAssignmentTaxOnOrder.TaxName = lTaxOnOrder.FirstOrDefault().TaxName;

    //                        // lst.TaxOnOrder = new List<AssignmentTaxOnOrder>();
    //                        lst.TaxOnOrder.Add(lAssignmentTaxOnOrder);
    //                    }

    //                    //-- End Tax Include on 31-march-2016 , By Avi Verma. 
    //                    //--------------------------------------------------------------------------
    //                }
    //                if (lAssignDeliveryDetail.Count() != 0)
    //                {
    //                    //*********************** AssignedTrackOrderDelivery Data ***********************//
    //                    if (lAssignedDeliveryOrderList.PaymentMode == "COD")
    //                    {
    //                        lAssignedTrackOrderDelivery.PaymentMode = "COD" + " " + lAssignDeliveryDetail[0].OTPAmount;//lTotalAmt ;
    //                    }
    //                    countItem--;
    //                    lAssignedTrackOrderDelivery.TotalSaving = lTotalSave;
    //                    lAssignedTrackOrderDelivery.TotalItem = countItem;
    //                    lAssignedTrackOrderDelivery.DeliveryCharges = lDeliveryCharge;
    //                    lAssignedTrackOrderDelivery.TotalAmount = lAssignDeliveryDetail[0].OTPAmount;//lTotalAmt;
    //                    lAssignedTrackOrderDelivery.BillAmount = lAssignDeliveryDetail[0].OTPAmount;// lTotalAmt;
    //                }
    //                else
    //                {
    //                    lAssignedDeliveryOrderList.PaymentMode = "No OTP Generated. Please do not deliver the order.";
    //                    lAssignedTrackOrderDelivery.TotalAmount = 0;//lTotalAmt;
    //                    lAssignedTrackOrderDelivery.BillAmount = 0;// lTotalAmt;
    //                }

    //                //--

    //                //--

    //                //-----------------------
    //                AssignmentDeliveryDetail cAssignmentDeliveryDetail=new AssignmentDeliveryDetail();
    //                if (lAssignDeliveryDetail.Count() != 0)
    //                {
    //                    //********************** Customer Copy Data ***************************//

    //                cAssignmentDeliveryDetail.ID=lAssignDeliveryDetail.FirstOrDefault().ID;
    //                cAssignmentDeliveryDetail.OrderNo=lAssignDeliveryDetail.FirstOrDefault().OrderNo;
    //                cAssignmentDeliveryDetail.OrderDate=lAssignDeliveryDetail.FirstOrDefault().OrderDate;
    //                cAssignmentDeliveryDetail.CustomerName=lAssignDeliveryDetail.FirstOrDefault().CustomerName;
    //                cAssignmentDeliveryDetail.Address=lAssignDeliveryDetail.FirstOrDefault().Address;
    //                cAssignmentDeliveryDetail.Pincode=lAssignDeliveryDetail.FirstOrDefault().Pincode;
    //                cAssignmentDeliveryDetail.Area=lAssignDeliveryDetail.FirstOrDefault().Area;
    //                cAssignmentDeliveryDetail.Mobile=lAssignDeliveryDetail.FirstOrDefault().Mobile;
    //                cAssignmentDeliveryDetail.OTP = lAssignDeliveryDetail.FirstOrDefault().OTP;
    //                cAssignmentDeliveryDetail.OTPAmount=lAssignDeliveryDetail.FirstOrDefault().OTPAmount;
    //                cAssignmentDeliveryDetail.AssignedTrackOrderDelivery=new List<AssignedTrackOrderDelivery>();
    //                cAssignmentDeliveryDetail.AssignedTrackOrderDelivery.Add(lAssignedTrackOrderDelivery);
    //                }
    //                    lst.CustomerCopy = new List<AssignmentDeliveryDetail>();
    //                    lst.CustomerCopy.Add(cAssignmentDeliveryDetail);
    //                    //if (lTaxOnOrder.Count() != 0)
    //                    //{
    //                    //    lst.TaxOnOrder = new List<AssignmentTaxOnOrder>();
    //                    //    lst.TaxOnOrder.Add(lAssignmentTaxOnOrder);
    //                    //}
    //                //-----------------------



    //            }
    //            else if ((lst.OrderStatus >= 3 && lst.OrderStatus <= 6) && lst.DeliveredType == "DELIVERY")// For Both Merchant and Customer Copy
    //            {
    //                if ((lst.OrderStatus >= 3 && lst.OrderStatus <= 4))//<=4 B'coz if assignment switch to another delivery boy
    //                {
    //                    var ID = dbgb.DeliveryOrderDetails.Where(x => x.ShopOrderCode == lst.ShopOrderCode).Select(x => x.ID).FirstOrDefault();
    //                    DeliveryOrderDetail deliveryorderdetail = new DeliveryOrderDetail();
    //                    deliveryorderdetail = dbgb.DeliveryOrderDetails.Find(ID);

    //                    CustomerOrderDetail lCustomerOrderDetail = new CustomerOrderDetail();
    //                    lCustomerOrderDetail = dbgb.CustomerOrderDetails.FirstOrDefault(x => x.ShopOrderCode == lst.ShopOrderCode);

    //                    ModelLayer.Models.CustomerOrder lCustomerOrder = new ModelLayer.Models.CustomerOrder();
    //                    lCustomerOrder = dbgb.CustomerOrders.Find(lCustomerOrderDetail.CustomerOrderID);

    //                    PersonalDetail lPersonalDetail = new PersonalDetail();
    //                    lPersonalDetail = dbgb.PersonalDetails.FirstOrDefault(x => x.UserLoginID == userloginId);
    //                    try
    //                    {
    //                        //****************** AssignedTrackOrderReceive Data ****************//
    //                        EzeeloDBContext dbtrack = new EzeeloDBContext();
    //                        List<BusinessLogicLayer.AssignedTrackOrderReceive> TrackOrderViewModels = new List<BusinessLogicLayer.AssignedTrackOrderReceive>();
    //                        TrackOrderViewModels = (
    //                            from CO in dbtrack.CustomerOrders
    //                            join COD in dbtrack.CustomerOrderDetails on CO.ID equals COD.CustomerOrderID
    //                            join DOD in dbtrack.DeliveryOrderDetails on COD.ShopOrderCode equals DOD.ShopOrderCode
    //                            join SS in dbtrack.ShopStocks on COD.ShopStockID equals SS.ID
    //                            join U in dbtrack.Units on SS.PackUnitID equals U.ID
    //                            join SP in dbtrack.ShopProducts on SS.ShopProductID equals SP.ID
    //                            join P in dbtrack.Products on SP.ProductID equals P.ID
    //                            join UL in dbtrack.UserLogins on CO.UserLoginID equals UL.ID
    //                            join PD in dbtrack.PersonalDetails on CO.UserLoginID equals PD.UserLoginID
    //                            where COD.ShopOrderCode == lCustomerOrderDetail.ShopOrderCode
    //                            select new BusinessLogicLayer.AssignedTrackOrderReceive //AssignedTrackOrderReceive Data
    //                            {
    //                                OrderCode = CO.OrderCode,
    //                                ID = COD.ID,
    //                                ShopOrderCode = COD.ShopOrderCode,
    //                                ShopStockID = SS.ID,
    //                                ShopID = SP.ShopID,
    //                                CustomerOrderID = COD.CustomerOrderID,
    //                                FirstName = PD.FirstName,//Customer Name
    //                                MiddleName = PD.MiddleName,
    //                                LastName = PD.LastName,
    //                                CreateDate = CO.CreateDate,
    //                                Name = P.Name,
    //                                PackSize = SS.PackSize,
    //                                PackUnitName = U.Name,
    //                                CODQty = COD.Qty,
    //                                SaleRate = COD.SaleRate,
    //                                RetailerRate = SS.RetailerRate,
    //                                TotalAmount = COD.TotalAmount,
    //                                OrderAmount = CO.OrderAmount,
    //                                GandhibaghCharge = DOD.GandhibaghCharge,
    //                                PayableAmount = CO.PayableAmount,
    //                                ProductID = P.ID
    //                            }).ToList();

    //                        long lShopId = TrackOrderViewModels.FirstOrDefault().ShopID;
    //                        Shop lShop = dbgb.Shops.Find(lShopId);


    //                        //-- Start Tax Include on 31-march-2016 , By Avi Verma. 
    //                        List<AssignmentTaxOnOrder> lTaxOnOrder = new List<AssignmentTaxOnOrder>();
    //                        lst.TaxOnOrder = new List<AssignmentTaxOnOrder>();//Added on 8-8-2016
    //                        lTaxOnOrder = (from tovm in TrackOrderViewModels
    //                                       join too in dbgb.TaxOnOrders on tovm.ID equals too.CustomerOrderDetailID
    //                                       join PrdTax in dbgb.ProductTaxes on too.ProductTaxID equals PrdTax.ID
    //                                       join taxMas in dbgb.TaxationMasters on PrdTax.TaxID equals taxMas.ID
    //                                       select new AssignmentTaxOnOrder
    //                                       {
    //                                           TaxOnOrderID = too.ID,
    //                                           CustomerOrderDetailID = tovm.ID,
    //                                           ProductTaxID = PrdTax.ID,
    //                                           TaxAmount = too.Amount,
    //                                           TaxID = PrdTax.TaxID,
    //                                           TaxPrefix = taxMas.Prefix,
    //                                           TaxName = taxMas.Name
    //                                       }).ToList();

                          
    //                        ////AssignmentTaxOnOrder lAssignmentTaxOnOrder = new AssignmentTaxOnOrder();////Hide on 8-8-2016
    //                        if (lTaxOnOrder.Count() != 0)
    //                        {
    //                            foreach (var lTax in lTaxOnOrder) //Added on 8-8-2016 working
    //                            {
    //                                //********************** Merchant Tax Data ***********************//

    //                                //lAssignmentTaxOnOrder.TaxOnOrderID = lTaxOnOrder.FirstOrDefault().TaxOnOrderID;
    //                                //lAssignmentTaxOnOrder.CustomerOrderDetailID = lTaxOnOrder.FirstOrDefault().CustomerOrderDetailID;
    //                                //lAssignmentTaxOnOrder.ProductTaxID = lTaxOnOrder.FirstOrDefault().ProductTaxID;
    //                                //lAssignmentTaxOnOrder.TaxAmount = lTaxOnOrder.FirstOrDefault().TaxAmount;
    //                                //lAssignmentTaxOnOrder.TaxID = lTaxOnOrder.FirstOrDefault().TaxID;
    //                                //lAssignmentTaxOnOrder.TaxPrefix = lTaxOnOrder.FirstOrDefault().TaxPrefix;
    //                                //lAssignmentTaxOnOrder.TaxName = lTaxOnOrder.FirstOrDefault().TaxName;

    //                                //-----------New 8-8-2016-----------------------//
    //                                AssignmentTaxOnOrder lAssignmentTaxOnOrder = new AssignmentTaxOnOrder(); //Added on 8-8-2016
    //                                lAssignmentTaxOnOrder.TaxOnOrderID = lTax.TaxOnOrderID;
    //                                lAssignmentTaxOnOrder.CustomerOrderDetailID = lTax.CustomerOrderDetailID;
    //                                lAssignmentTaxOnOrder.ProductTaxID = lTax.ProductTaxID;
    //                                lAssignmentTaxOnOrder.TaxAmount = lTax.TaxAmount;
    //                                lAssignmentTaxOnOrder.TaxID = lTax.TaxID;
    //                                lAssignmentTaxOnOrder.TaxPrefix = lTax.TaxPrefix;
    //                                lAssignmentTaxOnOrder.TaxName = lTax.TaxName;
    //                                lst.TaxOnOrder.Add(lAssignmentTaxOnOrder);
    //                                //-----------New End 8-8-2016-----------------------//
    //                            }
    //                        }

    //                        //-- End Tax Include on 31-march-2016 , By Avi Verma. 

    //                        //********************** Merchant Copy Data ***********************//

    //                        AssignmentReceiveDetail lAssignmentReceiveDetail = new AssignmentReceiveDetail();

    //                        lAssignmentReceiveDetail.ID = deliveryorderdetail.ID;
    //                        lAssignmentReceiveDetail.ShopOrderCode = deliveryorderdetail.ShopOrderCode;
    //                        lAssignmentReceiveDetail.DeliveryCharge = deliveryorderdetail.DeliveryCharge;
    //                        //----Changes By Mohit------------on 06-18-2015----------------------------//
    //                        lAssignmentReceiveDetail.GandhibaghCharge = deliveryorderdetail.GandhibaghCharge;
    //                        //----End Changes By Mohit------------on 06-18-2015----------------------------//

    //                        lAssignmentReceiveDetail.ShopName = lShop.Name; //Merchant Shop Name
    //                        lAssignmentReceiveDetail.PickUpName = lShop.ContactPerson; // Merchant Name
    //                        lAssignmentReceiveDetail.PickUpAddress = lShop.Address; // Merchant Address
    //                        lAssignmentReceiveDetail.PickUpContact = lShop.Mobile; // Merchant Contact
    //                        lAssignmentReceiveDetail.PickUpAlternateContact = lShop.Landline == null ? "No Address" : lShop.Landline; // Merchant Alternate Contact

    //                        lAssignmentReceiveDetail.DeliverToName = lPersonalDetail.Salutation.Name + ". " + lPersonalDetail.FirstName; // Delivery Boy Name as per userloginid
    //                        lAssignmentReceiveDetail.DeliverToEmail = lCustomerOrder.UserLogin.Email; // Customer email
    //                        lAssignmentReceiveDetail.DeliverToAddress = lCustomerOrder.ShippingAddress; // Cuatomer Address
    //                        lAssignmentReceiveDetail.DeliveryToContact = lst.PrimaryMobile == null ? "No Contact" : lst.PrimaryMobile;//lCustomerOrder.PrimaryMobile; // Cuatomer Contact
    //                        lAssignmentReceiveDetail.DeliveryToAlternateContact = lst.SecondaryMobile == null ? "No Contact" : lst.SecondaryMobile;// lCustomerOrder.SecondoryMobile; // Cuatomer Alternate Contact
    //                        //----Changes By Mohit------------on 19-11-2015----------------------------//
    //                        lAssignmentReceiveDetail.GandhibaghOrderCode = lst.OrderCode;// lCustomerOrder.OrderCode;
    //                        //----End Changes By Mohit------------on 19-11-2015----------------------------//


    //                        lAssignmentReceiveDetail.AssignedTrackOrderReceive = TrackOrderViewModels;

    //                        //------------------- new fields added on 08-sep-2015.
    //                        //-- Changes made by AVI VERMA.
    //                        //-- For Getting Mode of Payment. 
    //                        //-- Reason : - If payment mode is online. then, for display on delivery memo.. COD = 0 Rs. As it is paid online.
    //                        lAssignmentReceiveDetail.PaymentMode = lCustomerOrder.PaymentMode;
    //                        ////////////////////////////////////////////////////////
    //                        //-----------------------
    //                        lst.MerchantCopy = new List<AssignmentReceiveDetail>();
    //                        lst.MerchantCopy.Add(lAssignmentReceiveDetail);
    //                        /*if (lTaxOnOrder.Count() != 0)//Hide on 8-8-2016
    //                        {
    //                            lst.TaxOnOrder = new List<AssignmentTaxOnOrder>();
    //                            lst.TaxOnOrder.Add(lAssignmentTaxOnOrder);
    //                        }*/
    //                        //-----------------------
    //                    }
    //                    catch (DbEntityValidationException ex)
    //                    {
    //                        // Retrieve the error messages as a list of strings.
    //                        var errorMessages = ex.EntityValidationErrors
    //                                .SelectMany(x => x.ValidationErrors)
    //                                .Select(x => x.ErrorMessage);

    //                        // Join the list to a single string.
    //                        var fullErrorMessage = string.Join("; ", errorMessages);

    //                        // Combine the original exception message with the new one.
    //                        var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

    //                        // Throw a new DbEntityValidationException with the improved exception message.
    //                        throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
    //                    }
    //                }
    //                else if ((lst.OrderStatus >= 5 && lst.OrderStatus <= 6))
    //                {
    //                    var customerorderid = dbgb.CustomerOrders.Where(x => x.OrderCode == lst.OrderCode).Select(x => x.ID).FirstOrDefault();
    //                    var customerorderdetails = dbgb.CustomerOrderDetails.Include(c => c.CustomerOrder).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.Shop).Include(c => c.ShopStock).Where(x => x.CustomerOrderID == customerorderid && (x.OrderStatus >= 5 && x.OrderStatus <= 6)).OrderByDescending(x => x.ID).ToList();
                        
    //                    //********************** Customer Copy Data ***************************//
    //                    List<AssignmentDeliveryDetail> lAssignDeliveryDetail = new List<AssignmentDeliveryDetail>();
    //                    lAssignDeliveryDetail = (from cod in dbgb.CustomerOrderDetails
    //                                             join otp in dbgb.OTPs on cod.ShopOrderCode equals otp.ShopOrderCode
    //                                             join co in dbgb.CustomerOrders on cod.CustomerOrderID equals co.ID
    //                                             join pd in dbgb.PersonalDetails on co.UserLoginID equals pd.UserLoginID
    //                                             join ss in dbgb.ShopStocks on cod.ShopStockID equals ss.ID
    //                                             join sp in dbgb.ShopProducts on ss.ShopProductID equals sp.ID
    //                                             join s in dbgb.Shops on sp.ShopID equals s.ID
    //                                             where cod.CustomerOrderID == (long)customerorderid
    //                                             select new AssignmentDeliveryDetail //Customer Copy Data
    //                                             {
    //                                                 ID = cod.ID,
    //                                                 OrderNo = co.OrderCode,
    //                                                 OrderDate = co.CreateDate,
    //                                                 CustomerName = pd.FirstName != null ? pd.FirstName : "" + " " + pd.MiddleName != null ? pd.MiddleName : "" + " " + pd.LastName != null ? pd.LastName : "",//--Customer Name: --//  pd.FirstName + " " + pd.MiddleName + " " + pd.LastName,
    //                                                 Address = co.ShippingAddress,
    //                                                 Pincode = co.Pincode.Name,
    //                                                 Area = co.Area.Name,
    //                                                 Mobile = co.PrimaryMobile + "," + co.SecondoryMobile + "," + co.UserLogin.Mobile,
    //                                                 OTP=otp.OTP1,
    //                                                 OTPAmount = otp.PayableAmount
    //                                                 //---------------------------
    //                                                 /*TotalSaving = (cod.Qty * cod.MRP) - (cod.Qty * cod.SaleRate),
    //                                                 PaymentMode=cod.CustomerOrder.PaymentMode == "ONLINE" ? "PAID ONLINE" : cod.CustomerOrder.PaymentMode,
    //                                                 TotalSaving = (cod.Qty * cod.MRP) - (cod.Qty * cod.SaleRate)*/
    //                                             }
    //                                ).Distinct().ToList();


    //                    decimal lTotalDeliveryCharge = 0;

    //                    List<AssignmentDeliveryCharge> lAssignmentDeliveryCharge = new List<AssignmentDeliveryCharge>();
    //                    lAssignmentDeliveryCharge = (from dod in dbgb.DeliveryOrderDetails
    //                                                 join cod in dbgb.CustomerOrderDetails on dod.ShopOrderCode equals cod.ShopOrderCode
    //                                                 join co in dbgb.CustomerOrders on cod.CustomerOrderID equals co.ID
    //                                                 where co.ID == customerorderid
    //                                                 select new AssignmentDeliveryCharge
    //                                                 {
    //                                                     ShopOrderCode = dod.ShopOrderCode,
    //                                                     DeliveryCharge = dod.GandhibaghCharge,
    //                                                 }).Distinct().ToList();


    //                    decimal lDeliveryCharge = lAssignmentDeliveryCharge.Sum(x => x.DeliveryCharge);

    //                    int countItem = 1;
    //                    // decimal lTotalAmt = 0;
    //                    // decimal lTotalMRP = 0;
    //                    decimal lTotalSave = 0;
    //                    AssignedTrackOrderDelivery lAssignedTrackOrderDelivery = new AssignedTrackOrderDelivery();
    //                    AssignedDeliveryOrderList lAssignedDeliveryOrderList = new AssignedDeliveryOrderList();
    //                    lAssignedTrackOrderDelivery.AssignedDeliveryOrderList = new List<AssignedDeliveryOrderList>();
    //                    List<AssignmentTaxOnOrder> lTaxOnOrder = new List<AssignmentTaxOnOrder>();//Added
    //                    lst.TaxOnOrder = new List<AssignmentTaxOnOrder>();//Added
    //                    foreach (var Clst in customerorderdetails)
    //                    {
    //                        //*********************** AssignedDeliveryOrderList Data ***********************// 
    //                        AssignedDeliveryOrderList lAssignedDOList = new AssignedDeliveryOrderList();
    //                        lAssignedDOList.SrNo = countItem++;
    //                        lAssignedDOList.Particular = Clst.ShopStock.ShopProduct.Product.Name + " " + Clst.ShopStock.ProductVarient.Size.Name;
    //                        lAssignedDOList.MRP = (Clst.Qty * Clst.MRP);// lTotalMRP + (Clst.Qty * Clst.MRP);
    //                        lAssignedDOList.Rate = (Clst.Qty * Clst.SaleRate);// lTotalMRP + (Clst.Qty * Clst.SaleRate);
    //                        lAssignedDOList.Qty = Clst.Qty;
    //                        lAssignedDOList.Saving = (Clst.Qty * Clst.MRP) - (Clst.Qty * Clst.SaleRate);
    //                        lAssignedDOList.Amount = Clst.TotalAmount;
    //                        lAssignedDOList.PaymentMode = Clst.CustomerOrder.PaymentMode == "ONLINE" ? "PAID ONLINE" : Clst.CustomerOrder.PaymentMode;

    //                        lAssignedDeliveryOrderList.PaymentMode = Clst.CustomerOrder.PaymentMode == "ONLINE" ? "PAID ONLINE" : Clst.CustomerOrder.PaymentMode;//+ " " + Clst.CustomerOrder.PayableAmount;
    //                        // lTotalAmt=lTotalAmt+ Clst.TotalAmount;
    //                        lTotalSave = lTotalSave + ((Clst.Qty * Clst.MRP) - (Clst.Qty * Clst.SaleRate));

    //                        lAssignedTrackOrderDelivery.AssignedDeliveryOrderList.Add(lAssignedDOList);
    //                        //-------------------------------------------------------------------------
    //                        //-- Start Tax Include on 31-march-2016 , By Avi Verma. 
    //                        lTaxOnOrder = (from cod in customerorderdetails
    //                                       join too in dbgb.TaxOnOrders on cod.ID equals too.CustomerOrderDetailID
    //                                       join PrdTax in dbgb.ProductTaxes on too.ProductTaxID equals PrdTax.ID
    //                                       join taxMas in dbgb.TaxationMasters on PrdTax.TaxID equals taxMas.ID
    //                                       where too.CustomerOrderDetailID == Clst.ID
    //                                       select new AssignmentTaxOnOrder
    //                                       {
    //                                           TaxOnOrderID = too.ID,
    //                                           CustomerOrderDetailID = Clst.ID,//cod.ID,
    //                                           ProductTaxID = PrdTax.ID,
    //                                           TaxAmount = too.Amount,
    //                                           TaxID = PrdTax.TaxID,
    //                                           TaxPrefix = taxMas.Prefix,
    //                                           TaxName = taxMas.Name
    //                                       }).ToList();
    //                        AssignmentTaxOnOrder lAssignmentTaxOnOrder = new AssignmentTaxOnOrder();
    //                        if (lTaxOnOrder.Count() != 0)
    //                        {
    //                            //********************** Customer Tax Data ***********************//

    //                            lAssignmentTaxOnOrder.TaxOnOrderID = lTaxOnOrder.FirstOrDefault().TaxOnOrderID;
    //                            lAssignmentTaxOnOrder.CustomerOrderDetailID = lTaxOnOrder.FirstOrDefault().CustomerOrderDetailID;
    //                            lAssignmentTaxOnOrder.ProductTaxID = lTaxOnOrder.FirstOrDefault().ProductTaxID;
    //                            lAssignmentTaxOnOrder.TaxAmount = lTaxOnOrder.Sum(x => x.TaxAmount);
    //                            lAssignmentTaxOnOrder.TaxID = lTaxOnOrder.FirstOrDefault().TaxID;
    //                            lAssignmentTaxOnOrder.TaxPrefix = lTaxOnOrder.FirstOrDefault().TaxPrefix;
    //                            lAssignmentTaxOnOrder.TaxName = lTaxOnOrder.FirstOrDefault().TaxName;

    //                            lst.TaxOnOrder.Add(lAssignmentTaxOnOrder);
    //                        }

    //                        //-- End Tax Include on 31-march-2016 , By Avi Verma. 
    //                        //--------------------------------------------------------------------------
    //                    }
    //                    if (lAssignDeliveryDetail.Count() != 0)
    //                    {
    //                        //*********************** AssignedTrackOrderDelivery Data ***********************//
    //                        if (lAssignedDeliveryOrderList.PaymentMode == "COD")
    //                        {
    //                            lAssignedTrackOrderDelivery.PaymentMode = "COD" + " " + lAssignDeliveryDetail[0].OTPAmount;//lTotalAmt ;
    //                        }
    //                        countItem--;
    //                        lAssignedTrackOrderDelivery.TotalSaving = lTotalSave;
    //                        lAssignedTrackOrderDelivery.TotalItem = countItem;
    //                        lAssignedTrackOrderDelivery.DeliveryCharges = lDeliveryCharge;
    //                        lAssignedTrackOrderDelivery.TotalAmount = lAssignDeliveryDetail[0].OTPAmount;//lTotalAmt;
    //                        lAssignedTrackOrderDelivery.BillAmount = lAssignDeliveryDetail[0].OTPAmount;// lTotalAmt;
    //                    }
    //                    else
    //                    {
    //                        lAssignedDeliveryOrderList.PaymentMode = "No OTP Generated. Please do not deliver the order.";
    //                        lAssignedTrackOrderDelivery.TotalAmount = 0;//lTotalAmt;
    //                        lAssignedTrackOrderDelivery.BillAmount = 0;// lTotalAmt;
    //                    }


    //                    //-----------------------

    //                    AssignmentDeliveryDetail cAssignmentDeliveryDetail = new AssignmentDeliveryDetail();
    //                    if (lAssignDeliveryDetail.Count() != 0)
    //                    {
    //                        //*********************** Customer Copy Data ***********************//
    //                        cAssignmentDeliveryDetail.ID = lAssignDeliveryDetail.FirstOrDefault().ID;
    //                        cAssignmentDeliveryDetail.OrderNo = lAssignDeliveryDetail.FirstOrDefault().OrderNo;
    //                        cAssignmentDeliveryDetail.OrderDate = lAssignDeliveryDetail.FirstOrDefault().OrderDate;
    //                        cAssignmentDeliveryDetail.CustomerName = lAssignDeliveryDetail.FirstOrDefault().CustomerName;
    //                        cAssignmentDeliveryDetail.Address = lAssignDeliveryDetail.FirstOrDefault().Address;
    //                        cAssignmentDeliveryDetail.Pincode = lAssignDeliveryDetail.FirstOrDefault().Pincode;
    //                        cAssignmentDeliveryDetail.Area = lAssignDeliveryDetail.FirstOrDefault().Area;
    //                        cAssignmentDeliveryDetail.Mobile = lAssignDeliveryDetail.FirstOrDefault().Mobile;
    //                        cAssignmentDeliveryDetail.OTP = lAssignDeliveryDetail.FirstOrDefault().OTP;
    //                        cAssignmentDeliveryDetail.OTPAmount = lAssignDeliveryDetail.FirstOrDefault().OTPAmount;
    //                        cAssignmentDeliveryDetail.AssignedTrackOrderDelivery = new List<AssignedTrackOrderDelivery>();
    //                        cAssignmentDeliveryDetail.AssignedTrackOrderDelivery.Add(lAssignedTrackOrderDelivery);
    //                    }
    //                    lst.CustomerCopy = new List<AssignmentDeliveryDetail>();
    //                    lst.CustomerCopy.Add(cAssignmentDeliveryDetail);

    //                    //-----------------------
    //                }
    //            }
    //            //----------------------------------------------------------------------------------------------------------------------

    //        }
    //        empAssignList = queryResult.ToList();
    //       /* List<EmployeeAssignment> employeeAssign =new List<EmployeeAssignment>();

    //        foreach(var ReadRecord in queryResult){
    //        EmployeeAssignment empassign=new EmployeeAssignment();
    //        empassign.ID = ReadRecord.ID;
    //        empassign.OrderCode = ReadRecord.OrderCode;
    //        empassign.ShopOrderCode = ReadRecord.ShopOrderCode;
    //        empassign.GodownCode = ReadRecord.GodownCode;
    //        empassign.EmployeeCode = ReadRecord.EmployeeCode;
    //        empassign.OrderStatus = ReadRecord.OrderStatus;
    //        empassign.FromAddress = ReadRecord.FromAddress;
    //        empassign.DeliveredType =ReadRecord.DeliveredType;
    //        empassign.DeliveryType = ReadRecord.DeliveryType;
    //        empassign.DeliveryDate = ReadRecord.DeliveryDate;
    //        empassign.DeliverySchedule = ReadRecord.DeliverySchedule;
    //        //empassign.DeliveredTime = Convert.ToDateTime(ReadRecord.DeliveredTime);
    //        empassign.ToAddress = ReadRecord.ToAddress;
    //        empassign.X = ReadRecord.X;
    //        empassign.Y = ReadRecord.Y;
    //        employeeAssign.Add(empassign);
    //        }*/



    //        return empAssignList;
    //    }

    //}
    //------------------------------------------------------------------------------------------------------

}
