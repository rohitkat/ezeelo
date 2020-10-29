using ModelLayer.Models;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BusinessLogicLayer
{
     //--------------------------------------Hide EPOD from Ashish for Live--------------------------------------------------
    //public class EPODDetail : EPODManager 
    //{

    //    /// <summary>
    //    /// calling Base class Constructor
    //    /// </summary>
    //    /// <param name="server">System.Web.HttpContext.Current.Server</param>
    //    public EPODDetail(System.Web.HttpServerUtility server)
    //        : base(server)
    //    {
      
    //    }

    //    /// <summary>
    //    /// Generate Electronic Proof Of Delivery for both merchant and customer 
    //    /// </summary>
    //    /// <param name="EpodMod"></param>
    //    /// <param name="ReqBy"></param>
    //    /// <param name="IMEI"></param>
    //    /// <returns></returns>
    //   public EPODStaus CreateElectronicSign(EPODModel EpodMod , string ReqBy, string IMEI)
    //    {
    //     //   ModelLayer.Models.EPOD epodInsert = new ModelLayer.Models.EPOD();

    //        if (ReqBy == "m" && IMEI.Length > 5)
    //        {
    //            ReqBy = "Mobile";
    //        }
    //        else
    //        {
    //            ReqBy = "Desktop";
    //            IMEI = "";
    //        }

    //        EPODStaus lEPODStaus = new EPODStaus();
    //        //---------------------------------------------------------------------------------------------------------

    //        using (TransactionScope ts = new TransactionScope())
    //        {
    //            try
    //            {
    //                long createBy = (from PD in db.PersonalDetails
    //                                join UL in db.UserLogins on PD.UserLoginID equals UL.ID
    //                                where PD.UserLoginID == EpodMod.userlogin
    //                                select PD.ID).FirstOrDefault();
    //                int OwnerID = (int)(from PD in db.PersonalDetails
    //                               join UL in db.UserLogins on PD.UserLoginID equals UL.ID
    //                               where PD.UserLoginID == EpodMod.userlogin
    //                               select UL.CreateBy).FirstOrDefault();
    //                string Mode = "EPOD";
    //                //----------------------------------------------------------------------
    //                //-- Insert in EPOD table --//
    //                InsertEPOD(EpodMod, ReqBy, IMEI, createBy);


    //                //-- Update in CustomerOrderDetails table --//
    //                UpdateTables(EpodMod, ReqBy, IMEI, createBy, OwnerID, Mode);
                    
    //                //----------------------------------------------------------------------
                    

    //                ts.Complete();
    //                lEPODStaus.Status = "Successfull.";


    //            }
    //            catch (Exception ex)
    //            {
    //              /*  //View bag to fill role dropdown again

    //                //Incase of Insertion fail Message to be Display
    //                ViewBag.Message = "Sorry! Problem in Inserting Menu for Employee!!";*/
    //                //RollBack All Transaction
    //                ts.Dispose();

    //                lEPODStaus.Status = "Failed.";
    //               /* errStr.Append("Method Name[Http Request] :- Create[HttpPost]" + Environment.NewLine +
    //                       "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
    //                           ex.Message.ToString() + Environment.NewLine +
    //                 "====================================================================================="
    //                       );
    //                //ViewBag.Message = "Sorry! Problem in customer registration!!";
    //                ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");*/
    //                ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine 
    //                    , ErrorLog.Module.API, System.Web.HttpContext.Current.Server);


    //            }
    //        }
          
    //        //---------------------------------------------------------------------------------------------------------
    //        return lEPODStaus;

    //    }

    //   public EPODStaus Delivered(EPODModel EpodMod, string ReqBy, string IMEI)
    //   {
    //       if (ReqBy == "m" && IMEI.Length > 5)
    //       {
    //           ReqBy = "Mobile";
    //       }
    //       else
    //       {
    //           ReqBy = "Desktop";
    //           IMEI = "";
    //       }

    //       EPODStaus lEPODStaus = new EPODStaus();
    //       //---------------------------------------------------------------------------------------------------------

    //       using (TransactionScope ts = new TransactionScope())
    //       {
    //           try
    //           {
    //               long createBy = (from PD in db.PersonalDetails
    //                                join UL in db.UserLogins on PD.UserLoginID equals UL.ID
    //                                where PD.UserLoginID == EpodMod.userlogin
    //                                select PD.ID).FirstOrDefault();
    //               int OwnerID = (int)(from PD in db.PersonalDetails
    //                                   join UL in db.UserLogins on PD.UserLoginID equals UL.ID
    //                                   where PD.UserLoginID == EpodMod.userlogin
    //                                   select UL.CreateBy).FirstOrDefault();
    //               string Mode = "Delivered";
    //               //----------------------------------------------------------------------

    //               //-- Update in CustomerOrderDetails table --//
    //               UpdateTables(EpodMod, ReqBy, IMEI, createBy, OwnerID,Mode);

    //               //----------------------------------------------------------------------


    //               ts.Complete();
    //               lEPODStaus.Status = "Successfull.";


    //           }
    //           catch (Exception ex)
    //           {
    //               /*  //View bag to fill role dropdown again

    //                 //Incase of Insertion fail Message to be Display
    //                 ViewBag.Message = "Sorry! Problem in Inserting Menu for Employee!!";*/
    //               //RollBack All Transaction
    //               ts.Dispose();

    //               lEPODStaus.Status = "Failed.";
    //               /* errStr.Append("Method Name[Http Request] :- Create[HttpPost]" + Environment.NewLine +
    //                       "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
    //                           ex.Message.ToString() + Environment.NewLine +
    //                 "====================================================================================="
    //                       );
    //                //ViewBag.Message = "Sorry! Problem in customer registration!!";
    //                ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");*/
    //               ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine
    //                   , ErrorLog.Module.API, System.Web.HttpContext.Current.Server);


    //           }
    //       }

    //       //---------------------------------------------------------------------------------------------------------
    //       return lEPODStaus;

    //       return lEPODStaus;
    //   }

    //        #region -----GENERAL METHOD -----
    //         private void InsertEPOD(EPODModel EpodMod, string ReqBy, string IMEI, long createBy)
    //         {

    //           //ModelLayer.Models.EPOD epodInsert = new ModelLayer.Models.EPOD();//hide
    //           var IstableEmpty = from epod in db.EPOD select epod.ID;

    //           if (IstableEmpty.Count() > 0)
    //           {
    //              // var exist = (from epod in db.EPOD where epod.ShopOrderCode == EpodMod.ShopOrderCode select new { epod.ID, epod.CreateDate, epod.CreateBy }).Distinct();//hide
    //               var multiSoc1 = EpodMod.ShopOrderCode.Split(',');
    //               var exist = (from epod in db.EPOD where multiSoc1.Contains(epod.ShopOrderCode) select new { epod.ID, epod.ShopOrderCode, epod.CreateDate, epod.CreateBy }).Distinct().ToList();//.Distinct();//added
    //               if (exist.Count() > 0)
    //               {
    //                   try
    //                   {
    //                       EzeeloDBContext db1 = new EzeeloDBContext();

    //                       foreach (var item in exist)
    //                       {
    //                           ModelLayer.Models.EPOD epodUpdate = new ModelLayer.Models.EPOD();

    //                           epodUpdate.ID = item.ID;
    //                           epodUpdate.OrderCode = EpodMod.OrderCode;
    //                           epodUpdate.ShopOrderCode = item.ShopOrderCode; // EpodMod.ShopOrderCode;
    //                           epodUpdate.SignatureFromMerchant = EpodMod.SignatureFromMerchant;
    //                           epodUpdate.SignatureFromCustomer = EpodMod.SignatureFromCustomer;
    //                           epodUpdate.PayableAmount = EpodMod.PayableAmount;
    //                           epodUpdate.CreateDate = item.CreateDate;
    //                           epodUpdate.CreateBy = item.CreateBy;
    //                           epodUpdate.ModifyDate = CommonFunctions.GetLocalTime();
    //                           epodUpdate.ModifyBy = createBy;
    //                           epodUpdate.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
    //                           epodUpdate.DeviceType = ReqBy;
    //                           epodUpdate.DeviceID = IMEI;
    //                           //db.EPOD.Add(epod);
    //                           db1.Entry(epodUpdate).State = EntityState.Modified;
    //                           db1.SaveChanges();
    //                          // countSoc++;
    //                       }
    //                   }
    //                   catch (Exception ex)
    //                   {
    //                       throw new Exception("Unable to Update Recored in EPOD :- " + ex.InnerException);

    //                   }
    //               }
    //               else
    //               {
    //                   try
    //                   {
    //                       var multiSoc= EpodMod.ShopOrderCode.Split(',');
    //                       foreach (var soc in multiSoc)
    //                       {
    //                           ModelLayer.Models.EPOD epodInsert = new ModelLayer.Models.EPOD();//added
    //                           epodInsert.ID = 0;
    //                           epodInsert.OrderCode = EpodMod.OrderCode;
    //                           epodInsert.ShopOrderCode = soc.ToString();// EpodMod.ShopOrderCode;
    //                           epodInsert.SignatureFromMerchant = EpodMod.SignatureFromMerchant;
    //                           epodInsert.SignatureFromCustomer = EpodMod.SignatureFromCustomer;
    //                           epodInsert.PayableAmount = EpodMod.PayableAmount;
    //                           epodInsert.CreateDate = CommonFunctions.GetLocalTime();
    //                           epodInsert.CreateBy = createBy;
    //                           epodInsert.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
    //                           epodInsert.DeviceType = ReqBy;
    //                           epodInsert.DeviceID = IMEI;
    //                           db.EPOD.Add(epodInsert);
    //                           db.SaveChanges();
    //                       }
    //                   }
    //                   catch (Exception ex)
    //                   {
    //                       throw new Exception("Unable to Insert Recored in EPOD :- " + ex.InnerException);

    //                   }
    //               }
    //           }
    //           else
    //           {
    //               try
    //               {
    //                   var multiSoc = EpodMod.ShopOrderCode.Split(',');
    //                  // foreach (var soc in EpodMod.ShopOrderCode)
    //                   foreach (var soc in multiSoc)
    //                   {
    //                       ModelLayer.Models.EPOD epodInsert = new ModelLayer.Models.EPOD();//added
    //                       epodInsert.ID = 0;
    //                       epodInsert.OrderCode = EpodMod.OrderCode;
    //                       epodInsert.ShopOrderCode = soc.ToString();// EpodMod.ShopOrderCode;
    //                       epodInsert.SignatureFromMerchant = EpodMod.SignatureFromMerchant;
    //                       epodInsert.SignatureFromCustomer = EpodMod.SignatureFromCustomer;
    //                       epodInsert.PayableAmount = EpodMod.PayableAmount;
    //                       epodInsert.CreateDate = CommonFunctions.GetLocalTime();
    //                       epodInsert.CreateBy = createBy;
    //                       epodInsert.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
    //                       epodInsert.DeviceType = ReqBy;
    //                       epodInsert.DeviceID = IMEI;
    //                       db.EPOD.Add(epodInsert);
    //                       db.SaveChanges();
    //                   }
    //               }
    //               catch (Exception ex)
    //               {
    //                   throw new Exception("Unable to Insert Recored in EPOD :- " + ex.InnerException);

    //               }
    //           }


    //        }
    //         private void UpdateTables(EPODModel EpodMod, string ReqBy, string IMEI, long createBy, int OwnerID, string Mode)
    //       {
    //           var multiSoc1 = EpodMod.ShopOrderCode.Split(',');
    //           var GetCustomerOrderDetail = (from COD in db.CustomerOrderDetails
    //                                         where multiSoc1.Contains(COD.ShopOrderCode)//COD.ShopOrderCode == EpodMod.ShopOrderCode
    //                                         select new
    //                                         {
    //                                             ID = COD.ID,
    //                                             ShopOrderCode = COD.ShopOrderCode,
    //                                             ReferenceShopOrderCode = COD.ReferenceShopOrderCode,
    //                                             CustomerOrderID = COD.CustomerOrderID,
    //                                             ShopStockID = COD.ShopStockID,
    //                                             ShopID = COD.ShopID,
    //                                             Qty = COD.Qty,
    //                                             OrderStatus = COD.OrderStatus,
    //                                             MRP = COD.MRP,
    //                                             SaleRate = COD.SaleRate,
    //                                             OfferPercent = COD.OfferPercent,
    //                                             OfferRs = COD.OfferRs,
    //                                             IsInclusivOfTax = COD.IsInclusivOfTax,
    //                                             TotalAmount = COD.TotalAmount,
    //                                             IsActive = COD.IsActive,
    //                                             CreateDate = COD.CreateDate,
    //                                             CreateBy = COD.CreateBy,
    //                                             ModifyDate = COD.ModifyDate,
    //                                             ModifyBy = COD.ModifyBy,
    //                                             NetworkIP = COD.NetworkIP,
    //                                             DeviceType = COD.DeviceType,
    //                                             DeviceID = COD.DeviceID
    //                                         }).Distinct().ToList();
    //            if (GetCustomerOrderDetail.Count() > 0)
    //           {
    //               try
    //               {

    //                   EzeeloDBContext db2 = new EzeeloDBContext();
    //                   int sendSMSCount = 0;// use for Multiple ShopOrderCode for "Delivery"
    //                   foreach (var item in GetCustomerOrderDetail)
    //                   {
    //                       if (item.OrderStatus < 4 && Mode=="EPOD")
    //                       {
    //                           ModelLayer.Models.CustomerOrderDetail cod = new ModelLayer.Models.CustomerOrderDetail();
    //                           cod.ID = item.ID;
    //                           cod.ShopOrderCode = item.ShopOrderCode;
    //                           cod.ReferenceShopOrderCode = item.ReferenceShopOrderCode;
    //                           cod.CustomerOrderID = item.CustomerOrderID;
    //                           cod.ShopStockID = item.ShopStockID;
    //                           cod.ShopID = item.ShopID;
    //                           cod.Qty = item.Qty;
    //                           cod.OrderStatus = 4;
    //                           cod.MRP = item.MRP;
    //                           cod.SaleRate = item.SaleRate;
    //                           cod.OfferPercent = item.OfferPercent;
    //                           cod.OfferRs = item.OfferRs;
    //                           cod.IsInclusivOfTax = item.IsInclusivOfTax;
    //                           cod.TotalAmount = item.TotalAmount;
    //                           cod.IsActive = item.IsActive;
    //                           cod.CreateDate = item.CreateDate;
    //                           cod.CreateBy = item.CreateBy;
    //                           cod.ModifyDate = CommonFunctions.GetLocalTime();
    //                           cod.ModifyBy = createBy;
    //                           cod.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
    //                           cod.DeviceType = ReqBy;
    //                           cod.DeviceID = IMEI;
    //                           db2.Entry(cod).State = EntityState.Modified;
    //                           db2.SaveChanges();

    //                           //----------------------------------- Insert into CustomerOrderHistory -//
    //                           //EzeeloDBContext db3 = new EzeeloDBContext();
    //                           CustomerOrderHistory lCustomerOrderHistory = new CustomerOrderHistory();
    //                           lCustomerOrderHistory.CustomerOrderID = cod.CustomerOrderID;
    //                           lCustomerOrderHistory.ShopStockID = cod.ShopStockID;
    //                           lCustomerOrderHistory.Status = 4;
    //                           lCustomerOrderHistory.IsActive = cod.IsActive;
    //                           lCustomerOrderHistory.CreateBy = createBy;// customerCareSessionViewModel.PersonalDetailID;
    //                           lCustomerOrderHistory.CreateDate = DateTime.Now;
    //                           lCustomerOrderHistory.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
    //                           lCustomerOrderHistory.DeviceType = ReqBy;
    //                           lCustomerOrderHistory.DeviceID = IMEI;
    //                           db2.CustomerOrderHistories.Add(lCustomerOrderHistory);
    //                           db2.SaveChanges();

    //                           //----------------------------------- Insert into CustomerOrderDetailCall -//
    //                           //EzeeloDBContext db4 = new EzeeloDBContext();
    //                           CustomerOrderDetailCall lCustomerOrderDetailCall = new CustomerOrderDetailCall();
    //                           lCustomerOrderDetailCall.ID = 0;
    //                           lCustomerOrderDetailCall.BusinessTypeID = 3;
    //                           lCustomerOrderDetailCall.OwnerID = (long)OwnerID;
    //                           lCustomerOrderDetailCall.ShopOrderCode = EpodMod.ShopOrderCode;
    //                           lCustomerOrderDetailCall.OrderStatus =4;
    //                           lCustomerOrderDetailCall.CreateBy = createBy;// customerCareSessionViewModel.PersonalDetailID;
    //                           lCustomerOrderDetailCall.CreateDate = DateTime.Now;
    //                           lCustomerOrderDetailCall.Description = "From Delivery Boy App";
    //                           //lCustomerOrderDetailCall.OTP = EpodMod.OTP;
    //                           db2.CustomerOrderDetailCalls.Add(lCustomerOrderDetailCall);
    //                           db2.SaveChanges();

    //                       }
    //                       /*else if (item.OrderStatus ==6 && Mode == "EPOD")
    //                       {
    //                           //We are not modifing table CustomerOrderDetail B'coz of same OrserStatus and not inserting table CustomerOrderDetailCall,CustomerOrderHistory  B'coz of same OrserStatus
    //                       }*/
    //                       else if (Mode == "Delivered")
    //                       {
    //                           // To manage the dipaly of table into email
    //                           System.Text.StringBuilder sbHtml = new System.Text.StringBuilder(
    //                               "<table border=\"0\" cellpadding=\"5\" cellspacing=\"0\" width=\"100%\" style=\"text-align: center; font-family: Calibri; font-size: 1.5vw; color: #4f4f4f;\">" +
    //                               "<thead>" +
    //                               "<tr>" +
    //                               "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">Preferred Delivery Time</th>" +
    //                               "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">Product Name</th>" +
    //                               "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">Product Quantity</th>" +
    //                               "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">Order Date</th>" +
    //                               "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">Delivery Type</th>" +
    //                               "</tr>" +
    //                               "</thead>" +
    //                               "<tbody>"
    //                               );

    //                          /* if (EpodMod.OTP != null)
    //                           {
    //                               int Rcount = db.OTPs.Where(x => x.OrderCode == EpodMod.OrderCode && x.ShopOrderCode == EpodMod.ShopOrderCode && x.OTP1 == EpodMod.OTP).Count();

    //                               if (Rcount == 0)
    //                               {
    //                                   throw new Exception("Failed. Wrong OTP");
    //                               }
    //                               else
    //                               {*/
    //                           ModelLayer.Models.CustomerOrderDetail lCustomerOrderDetail = new ModelLayer.Models.CustomerOrderDetail();
    //                           lCustomerOrderDetail.ID = item.ID;
    //                           lCustomerOrderDetail.ShopOrderCode = item.ShopOrderCode;
    //                           lCustomerOrderDetail.ReferenceShopOrderCode = item.ReferenceShopOrderCode;
    //                           lCustomerOrderDetail.CustomerOrderID = item.CustomerOrderID;
    //                           lCustomerOrderDetail.ShopStockID = item.ShopStockID;
    //                           lCustomerOrderDetail.ShopID = item.ShopID;
    //                           lCustomerOrderDetail.Qty = item.Qty;
    //                           lCustomerOrderDetail.OrderStatus = 7;
    //                           lCustomerOrderDetail.MRP = item.MRP;
    //                           lCustomerOrderDetail.SaleRate = item.SaleRate;
    //                           lCustomerOrderDetail.OfferPercent = item.OfferPercent;
    //                           lCustomerOrderDetail.OfferRs = item.OfferRs;
    //                           lCustomerOrderDetail.IsInclusivOfTax = item.IsInclusivOfTax;
    //                           lCustomerOrderDetail.TotalAmount = item.TotalAmount;
    //                           lCustomerOrderDetail.IsActive = item.IsActive;
    //                           lCustomerOrderDetail.CreateDate = item.CreateDate;
    //                           lCustomerOrderDetail.CreateBy = item.CreateBy;
    //                           lCustomerOrderDetail.ModifyDate = CommonFunctions.GetLocalTime();
    //                           lCustomerOrderDetail.ModifyBy = createBy;
    //                           lCustomerOrderDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
    //                           lCustomerOrderDetail.DeviceType = ReqBy;
    //                           lCustomerOrderDetail.DeviceID = IMEI;
    //                           db2.Entry(lCustomerOrderDetail).State = EntityState.Modified;
    //                                   db2.SaveChanges();

    //                                   //----------------------------------- Insert into CustomerOrderHistory -//
    //                                   //EzeeloDBContext db3 = new EzeeloDBContext();
    //                                   CustomerOrderHistory lCustomerOrderHistory = new CustomerOrderHistory();
    //                                   lCustomerOrderHistory.ID = 0;
    //                                   lCustomerOrderHistory.CustomerOrderID = lCustomerOrderDetail.CustomerOrderID;
    //                                   lCustomerOrderHistory.ShopStockID = lCustomerOrderDetail.ShopStockID;
    //                                   lCustomerOrderHistory.Status = 7;
    //                                   lCustomerOrderHistory.IsActive = lCustomerOrderDetail.IsActive;
    //                                   lCustomerOrderHistory.CreateBy = createBy;// customerCareSessionViewModel.PersonalDetailID;
    //                                   lCustomerOrderHistory.CreateDate = DateTime.Now;
    //                                   lCustomerOrderHistory.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
    //                                   lCustomerOrderHistory.DeviceType = ReqBy;
    //                                   lCustomerOrderHistory.DeviceID = IMEI;
    //                                   db2.CustomerOrderHistories.Add(lCustomerOrderHistory);
    //                                   db2.SaveChanges();
    //                           //--------------------------------------------------------------------------------------------
    //                                   var merchantProductList = (from cod in db.CustomerOrderDetails
    //                                                              join ss in db.ShopStocks on cod.ShopStockID equals ss.ID
    //                                                              join SP in db.ShopProducts on ss.ShopProductID equals SP.ID
    //                                                              join p in db.Products on SP.ProductID equals p.ID
    //                                                              join dod in db.DeliveryOrderDetails on cod.ShopOrderCode equals dod.ShopOrderCode
    //                                                              where SP.ShopID == lCustomerOrderDetail.ShopID && cod.CustomerOrderID == lCustomerOrderDetail.CustomerOrderID

    //                                                              select new
    //                                                              {
    //                                                                  ProductName = p.Name,
    //                                                                  Qty = cod.Qty,
    //                                                                  OrderDate = cod.CreateDate,
    //                                                                  DeliveryType = dod.DeliveryType

    //                                                              }).ToList();

    //                                   if (merchantProductList != null)
    //                                   {
    //                                       foreach (var items in merchantProductList)
    //                                       {

    //                                           sbHtml.AppendFormat(
    //                                              "<tr>" +
    //                                              "<th style=\"border: 1px solid #b8b8b7; border-right:none; border-bottom:none;\">{0}</th>" +  // preffered time
    //                                              "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{1}</th>" + // product name
    //                                              "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{2}</th>" + // quantity
    //                                               "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{3}</th>" + // Order date
    //                                              "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{4}</th>" + // delivery type
    //                                              "</tr>", "No Preffered Time Available", items.ProductName.ToString().Trim(), items.Qty, items.OrderDate.ToString("MMM dd, yyyy hh:mm tt"), items.DeliveryType.ToString().Trim()
    //                                               );
    //                                       }


    //                                   }
    //                           //----------------------------------------------------------------------------------------
    //                                   //----------------------------------- Insert into CustomerOrderDetailCall -//
    //                                   if (EpodMod.OTP != null)
    //                                   {
    //                                       //EzeeloDBContext db4 = new EzeeloDBContext();
    //                                       CustomerOrderDetailCall lCustomerOrderDetailCall = new CustomerOrderDetailCall();
    //                                       lCustomerOrderDetailCall.ID = 0;
    //                                       lCustomerOrderDetailCall.BusinessTypeID = 3;
    //                                       lCustomerOrderDetailCall.OwnerID = (long)OwnerID;
    //                                       lCustomerOrderDetailCall.ShopOrderCode = item.ShopOrderCode;// EpodMod.ShopOrderCode;
    //                                       lCustomerOrderDetailCall.OrderStatus = 7;
    //                                       lCustomerOrderDetailCall.CreateBy = createBy;// customerCareSessionViewModel.PersonalDetailID;
    //                                       lCustomerOrderDetailCall.CreateDate = DateTime.Now;
    //                                       lCustomerOrderDetailCall.Description = "From Delivery Boy App";
    //                                       lCustomerOrderDetailCall.OTP = EpodMod.OTP;
    //                                       db2.CustomerOrderDetailCalls.Add(lCustomerOrderDetailCall);
    //                                       db2.SaveChanges();
    //                                   }
    //                                   else
    //                                   {
    //                                       CustomerOrderDetailCall lCustomerOrderDetailCall = new CustomerOrderDetailCall();
    //                                       lCustomerOrderDetailCall.ID = 0;
    //                                       lCustomerOrderDetailCall.BusinessTypeID = 3;
    //                                       lCustomerOrderDetailCall.OwnerID = (long)OwnerID;
    //                                       lCustomerOrderDetailCall.ShopOrderCode = item.ShopOrderCode;//EpodMod.ShopOrderCode;
    //                                       lCustomerOrderDetailCall.OrderStatus = 7;
    //                                       lCustomerOrderDetailCall.CreateBy = createBy;// customerCareSessionViewModel.PersonalDetailID;
    //                                       lCustomerOrderDetailCall.CreateDate = DateTime.Now;
    //                                       lCustomerOrderDetailCall.Description = "From Delivery Boy App";
    //                                       //lCustomerOrderDetailCall.OTP = EpodMod.OTP;
    //                                       db2.CustomerOrderDetailCalls.Add(lCustomerOrderDetailCall);
    //                                       db2.SaveChanges();
    //                                   }
    //                           //-------------------------------------------------------------------------------------------------------------------


    //                                   if (sendSMSCount == 0)
    //                                   {
    //                                       sendSMSCount++;
    //                                       string SOD = multiSoc1[0].ToString();
    //                                       List<CustomerOrderDetail> customerOrderDetails = db.CustomerOrderDetails.Where(x => x.ShopOrderCode == SOD).ToList();
    //                                       string lEmail = customerOrderDetails.FirstOrDefault().CustomerOrder.UserLogin.Email;
    //                                       string lMobile = customerOrderDetails.FirstOrDefault().CustomerOrder.UserLogin.Mobile;

    //                                       //lEmail = "nagraleashish@yahoo.com";
    //                                       //lMobile = "9350507296";
    //                                       //------Declartion Email---------//
    //                                       BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
    //                                       BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
    //                                       Dictionary<string, string> dictionary = new Dictionary<string, string>();
    //                                       bool Flag = false;

    //                                       //------Key value add in Email---------//
    //                                       string city = "nagpur";
    //                                       if (System.Web.HttpContext.Current.Request.Cookies["CityCookie"] != null && System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value != "")
    //                                       {
    //                                           city = System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
    //                                       }
    //                                       dictionary.Add("<!--ACCOUNT_URL-->", "http://ezeelo.com/" + city + "/login");
    //                                       dictionary.Add("<!--ORDERS_URL-->", "http://www.ezeelo.com/" + city + "/cust-o/my-order");
    //                                       //dictionary.Add("<!--ACCOUNT_URL-->", @"http://www.ezeelo.com");
    //                                       //dictionary.Add("<!--ORDERS_URL-->", @"http://www.ezeelo.com");
    //                                       dictionary.Add("<!--NAME-->", lEmail);
    //                                       dictionary.Add("<!-- ORDER_DETAILS -->", sbHtml.ToString());

    //                                       try
    //                                       {
    //                                           Flag = gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_ORD_DLVRD, new string[] { lEmail }, dictionary, true);
    //                                       }
    //                                       catch (Exception ex)
    //                                       {

    //                                       }
    //                                       finally
    //                                       {
    //                                           //------Declartion SMS---------//
    //                                           gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
    //                                           dictionary.Clear();
    //                                           Flag = false;
    //                                           try
    //                                           {
    //                                               Flag = gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_ORD_DLVRD, new string[] { lMobile }, dictionary);
    //                                           }
    //                                           catch (Exception ex)
    //                                           {

    //                                           }
    //                                           finally
    //                                           {
    //                                               dictionary.Clear();
    //                                           }
    //                                       }
    //                                   }
    //                            //   }
    //                          // }

                                  
    //                       }
    //                   }
    //               }
    //               catch (Exception ex)
    //               {
    //                   throw new Exception("Unable to Update Recored in CustomerOrderDetail :- " + ex.InnerException);

    //               }
    //           }
    //       }
    //        #endregion

    //}
    //    /// <summary>
    ///// This Class is to to send the response after successfull login.
    ///// </summary>
    //public class EPODStaus
    //{
    //    public string Status { get; set; }
    //}

    //----------------------------------------------------------------------------------------
}
