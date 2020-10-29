using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using ModelLayer.Models.Enum;
using Merchant.Models;
using BusinessLogicLayer;

namespace Merchant.Controllers
{
    public class DispatchedController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private long GetShopID()
        {
            EzeeloDBContext db = new EzeeloDBContext();
            //Session["USER_LOGIN_ID"] = 2;
            long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
            long BusinessDetailID = 0;
            long ShopID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    BusinessDetailID = Convert.ToInt32(db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                    ShopID = Convert.ToInt32(db.Shops.Where(x => x.BusinessDetailID == BusinessDetailID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[DispatchedController][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
            }
            return ShopID;
        }
        private long GetPersonalDetailID()
        {
            // EzeeloDBContext db = new EzeeloDBContext();
            //Session["USER_LOGIN_ID"] = 2;
            long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
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
                throw new BusinessLogicLayer.MyException("[DispatchedController][GetShopID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }
        
        // GET: /Dispatched/
        [SessionExpire]
        [Authorize(Roles = "Dispatched/CanRead")]
        public ActionResult Index()
        {
            //EzeeloDBContext db = new EzeeloDBContext();
            TrackOrderViewModelList TOVML = new TrackOrderViewModelList();
            long ShopID = GetShopID();
            try
            {
                var IsDeliveryOutSource = (from S in db.Shops.Where(x => x.ID.Equals(ShopID)) select new { S.IsDeliveryOutSource }).First();
                if (IsDeliveryOutSource.IsDeliveryOutSource.Equals(false))
                {
                    var queryResult = (
                from CO in db.CustomerOrders
                join COD in db.CustomerOrderDetails on CO.ID equals COD.CustomerOrderID
                join DOD in db.DeliveryOrderDetails on COD.ShopOrderCode equals DOD.ShopOrderCode
                join SS in db.ShopStocks on COD.ShopStockID equals SS.ID
                join U in db.Units on SS.PackUnitID equals U.ID
                join SP in db.ShopProducts on SS.ShopProductID equals SP.ID
                join P in db.Products on SP.ProductID equals P.ID
                join UL in db.UserLogins on CO.UserLoginID equals UL.ID
                join PD in db.PersonalDetails on CO.UserLoginID equals PD.UserLoginID
                join S in db.Shops on COD.ShopID equals S.ID
                where COD.ShopID.Equals(ShopID) && COD.OrderStatus.Equals((int)ORDER_STATUS.DISPATCHED_FROM_SHOP)
                select new
                {
                    CO.OrderCode,
                    COD.ShopOrderCode,
                    COD.CustomerOrderID,
                    PD.FirstName,
                    PD.MiddleName,
                    PD.LastName,
                    CO.CreateDate,
                    P.Name,
                    ColorName = SS.ProductVarient.Color.Name,
                    SizeName = SS.ProductVarient.Size.Name,
                    DimensionName = SS.ProductVarient.Dimension.Name,
                    MaterialName = SS.ProductVarient.Material.Name,
                    SS.PackSize,
                    UnitName = U.Name,
                    COD.Qty,
                    SS.RetailerRate,
                    COD.TotalAmount,
                    CO.OrderAmount,
                    DOD.GandhibaghCharge,
                    CO.PayableAmount,
                    S.IsDeliveryOutSource,
                    UL.Email,
                    UL.Mobile,
                    CO.ShippingAddress,
                    AreaName = CO.Area.Name,
                    CityName = CO.Pincode.City.Name,
                    PincodeName = CO.Pincode.Name,
                    ProductID = P.ID
                }).OrderByDescending(x => x.CreateDate).ToList();
                    List<TrackOrderViewModel> listTrackOrder = new List<TrackOrderViewModel>();

                    foreach (var ReadRecord in queryResult)
                    {
                        TrackOrderViewModel tovm = new TrackOrderViewModel();
                        tovm.CustomerOrderID = ReadRecord.CustomerOrderID;
                        tovm.OrderCode = ReadRecord.OrderCode;
                        tovm.ShopOrderCode = ReadRecord.ShopOrderCode;
                        tovm.FirstName = ReadRecord.FirstName + " " + ReadRecord.MiddleName + " " + ReadRecord.LastName;
                        tovm.CreateDate = ReadRecord.CreateDate;
                        tovm.PName = ReadRecord.Name;
                        if (ReadRecord.ColorName != "N/A") tovm.PName = tovm.PName + " " + ReadRecord.ColorName;
                        if (ReadRecord.SizeName != "N/A") tovm.PName = tovm.PName + " " + ReadRecord.SizeName;
                        if (ReadRecord.DimensionName != "N/A") tovm.PName = tovm.PName + " " + ReadRecord.DimensionName;
                        if (ReadRecord.MaterialName != "N/A") tovm.PName = tovm.PName + " " + ReadRecord.MaterialName;
                        tovm.PackSize = ReadRecord.PackSize;
                        tovm.CODQty = ReadRecord.Qty;
                        tovm.Name = ReadRecord.UnitName;//pack unit 
                        tovm.RetailerRate = ReadRecord.RetailerRate;
                        tovm.TotalAmount = ReadRecord.TotalAmount;
                        tovm.OrderAmount = ReadRecord.OrderAmount;
                        tovm.GandhibaghCharge = ReadRecord.GandhibaghCharge;
                        tovm.PayableAmount = ReadRecord.PayableAmount;
                        tovm.IsDeliveryOutSource = ReadRecord.IsDeliveryOutSource;
                        tovm.Email = ReadRecord.Email;
                        tovm.Mobile = ReadRecord.Mobile;
                        tovm.ShippingAddress = ReadRecord.ShippingAddress;
                        tovm.AreaName = ReadRecord.AreaName;
                        tovm.CityName = ReadRecord.CityName;
                        tovm.PincodeName = ReadRecord.PincodeName;
                        tovm.ImageLocation = MerchantCommonFunction.FindProductDefaultImageLocation(ReadRecord.ProductID);
                        listTrackOrder.Add(tovm);
                    }


                    TOVML.LtrackOrderViewModelList = listTrackOrder;
                }
                if (IsDeliveryOutSource.IsDeliveryOutSource.Equals(true))
                {
                    var queryResult = (
                from CO in db.CustomerOrders
                join COD in db.CustomerOrderDetails on CO.ID equals COD.CustomerOrderID
                join DOD in db.DeliveryOrderDetails on COD.ShopOrderCode equals DOD.ShopOrderCode
                join SS in db.ShopStocks on COD.ShopStockID equals SS.ID
                join U in db.Units on SS.PackUnitID equals U.ID
                join SP in db.ShopProducts on SS.ShopProductID equals SP.ID
                join P in db.Products on SP.ProductID equals P.ID
                join UL in db.UserLogins on CO.UserLoginID equals UL.ID
                join PD in db.PersonalDetails on CO.UserLoginID equals PD.UserLoginID
                join S in db.Shops on COD.ShopID equals S.ID
                where COD.ShopID.Equals(ShopID) && COD.OrderStatus.Equals((int)ORDER_STATUS.DISPATCHED_FROM_SHOP)
                select new
                {
                    CO.OrderCode,
                    COD.ShopOrderCode,
                    COD.CustomerOrderID,
                    PD.FirstName,
                    PD.MiddleName,
                    PD.LastName,
                    CO.CreateDate,
                    P.Name,
                    ColorName = SS.ProductVarient.Color.Name,
                    SizeName = SS.ProductVarient.Size.Name,
                    DimensionName = SS.ProductVarient.Dimension.Name,
                    MaterialName = SS.ProductVarient.Material.Name,
                    SS.PackSize,
                    UnitName = U.Name,
                    COD.Qty,
                    SS.RetailerRate,
                    COD.TotalAmount,
                    CO.OrderAmount,
                    DOD.GandhibaghCharge,
                    CO.PayableAmount,
                    S.IsDeliveryOutSource,
                    ProductID = P.ID
                }).OrderByDescending(x => x.CreateDate).ToList();
                    List<TrackOrderViewModel> listTrackOrder = new List<TrackOrderViewModel>();

                    foreach (var ReadRecord in queryResult)
                    {
                        TrackOrderViewModel tovm = new TrackOrderViewModel();
                        tovm.CustomerOrderID = ReadRecord.CustomerOrderID;
                        tovm.OrderCode = ReadRecord.OrderCode;
                        tovm.ShopOrderCode = ReadRecord.ShopOrderCode;
                        tovm.FirstName = ReadRecord.FirstName + " " + ReadRecord.MiddleName + " " + ReadRecord.LastName;
                        tovm.CreateDate = ReadRecord.CreateDate;
                        tovm.PName = ReadRecord.Name;
                        if (ReadRecord.ColorName != "N/A") tovm.PName = tovm.PName + " " + ReadRecord.ColorName;
                        if (ReadRecord.SizeName != "N/A") tovm.PName = tovm.PName + " " + ReadRecord.SizeName;
                        if (ReadRecord.DimensionName != "N/A") tovm.PName = tovm.PName + " " + ReadRecord.DimensionName;
                        if (ReadRecord.MaterialName != "N/A") tovm.PName = tovm.PName + " " + ReadRecord.MaterialName;
                        tovm.PackSize = ReadRecord.PackSize;
                        tovm.CODQty = ReadRecord.Qty;
                        tovm.Name = ReadRecord.UnitName;//pack unit 
                        tovm.RetailerRate = ReadRecord.RetailerRate;
                        tovm.TotalAmount = ReadRecord.TotalAmount;
                        tovm.OrderAmount = ReadRecord.OrderAmount;
                        tovm.GandhibaghCharge = ReadRecord.GandhibaghCharge;
                        tovm.PayableAmount = ReadRecord.PayableAmount;
                        tovm.IsDeliveryOutSource = ReadRecord.IsDeliveryOutSource;
                        tovm.ImageLocation = MerchantCommonFunction.FindProductDefaultImageLocation(ReadRecord.ProductID);
                        listTrackOrder.Add(tovm);
                    }


                    TOVML.LtrackOrderViewModelList = listTrackOrder;
                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DispatchedController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DispatchedController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }

            return View(TOVML);
        }
        
        [HttpPost]
        [SessionExpire]
        [Authorize(Roles = "Dispatched/CanWrite")]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "Delivered")]
        public ActionResult Delivered(TrackOrderViewModelList trackOrderViewModelList)
        {
            if (trackOrderViewModelList.LtrackOrderViewModelList != null)
            {

                string shopOrderCode = string.Empty; 
                long shopID=0;

                foreach (TrackOrderViewModel TOVM in trackOrderViewModelList.LtrackOrderViewModelList)
                {

                    shopOrderCode = TOVM.ShopOrderCode.ToString();
                    shopID = TOVM.ShopID;

                    if (TOVM.CheckBoxStatus)
                    {
                        

                        List<CustomerOrderDetail> ListCOD = db.CustomerOrderDetails.Where(x => x.ShopOrderCode == TOVM.ShopOrderCode).ToList();
                        foreach (CustomerOrderDetail COD in ListCOD)
                        {
                            using (var dbContextTransaction = db.Database.BeginTransaction())
                            {
                                try
                                {
                                    ////Log Table Insertion
                                    //LogTable logTable = new LogTable();
                                    //logTable.TableName = "CustomerOrderDetail";//table Name(Model Name)
                                    //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(COD);
                                    //logTable.TableRowID = COD.ID;
                                    //logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                                    //long? rowOwnerID = (COD.ModifyBy >= 0 ? COD.ModifyBy : COD.CreateBy);
                                    //logTable.RowOwnerID = (long)rowOwnerID;
                                    //logTable.CreateDate = DateTime.UtcNow;
                                    //logTable.CreateBy = GetPersonalDetailID();//Session ID
                                    //db.LogTables.Add(logTable);

                                    //Customer Order Detail Table Status Update
                                    COD.OrderStatus = (int)ORDER_STATUS.DELIVERED;
                                    COD.ModifyDate = DateTime.UtcNow;
                                    COD.ModifyBy = GetPersonalDetailID();//Session ID
                                    COD.NetworkIP = CommonFunctions.GetClientIP();
                                    //COD.DeviceType = "x";
                                    //COD.DeviceID = "x";
                                    db.Entry(COD).State = EntityState.Modified;
                                    db.SaveChanges();

                                    //Customer Order History Table Status Update
                                    CustomerOrderHistory COH = new CustomerOrderHistory();
                                    COH.CustomerOrderID = COD.CustomerOrderID;
                                    COH.ShopStockID = COD.ShopStockID;
                                    COH.Status = COD.OrderStatus;
                                    COH.IsActive = true;
                                    COH.CreateDate = DateTime.UtcNow;
                                    COH.CreateBy = GetPersonalDetailID();//Session ID
                                    COH.NetworkIP = CommonFunctions.GetClientIP();
                                    //COH.DeviceType = "x";
                                    //COH.DeviceID = "x";
                                    db.CustomerOrderHistories.Add(COH);
                                    db.SaveChanges();
                                    dbContextTransaction.Commit();

                                }
                                catch (BusinessLogicLayer.MyException myEx)
                                {
                                    dbContextTransaction.Rollback();
                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                        + "[DispatchedController][POST:Delivered]" + myEx.EXCEPTION_PATH,
                                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                                }
                                catch (Exception ex)
                                {
                                    dbContextTransaction.Rollback();
                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + ex.Message + Environment.NewLine
                                        + "[DispatchedController][POST:Delivered]",
                                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                                }
                            }
                        }
                        //--------------------SMS & Email Function call------------------//
                        //------Declartion Email---------//
                        GateWay gateWay = new Email(System.Web.HttpContext.Current.Server);
                        Dictionary<string, string> dictionary = new Dictionary<string, string>();
                        bool Flag = false;
                        try
                        {
                            //------Key value add in Email---------//
                            dictionary.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "login/login");
                            dictionary.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "login/login");
                            dictionary.Add("<!--NAME-->", TOVM.FirstName);



                           

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


                            var merchantProductList = (from cod in db.CustomerOrderDetails
                                                        join ss in db.ShopStocks on cod.ShopStockID equals ss.ID
                                                        join SP in db.ShopProducts on ss.ShopProductID equals SP.ID
                                                        join p in db.Products on SP.ProductID equals p.ID
                                                        join dod in db.DeliveryOrderDetails on cod.ShopOrderCode equals dod.ShopOrderCode 
                                                        where dod.ShopOrderCode == shopOrderCode && SP.ShopID == shopID

                                                        select new
                                                        {
                                                            ProductName = p.Name,
                                                            Qty = cod.Qty,
                                                            OrderDate = cod.CreateDate ,
                                                            DeliveryType = dod.DeliveryType 

                                                        }).ToList();

                             if (merchantProductList != null)
                             {
                                 foreach (var item in merchantProductList)
                                 {

                                     sbHtml.AppendFormat(
                                        "<tr>" +
                                        "<th style=\"border: 1px solid #b8b8b7; border-right:none; border-bottom:none;\">{0}</th>" +
                                        "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{1}</th>" +
                                        "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{2}</th>" +
                                        "<th style=\"border: 1px solid #b8b8b7; border-right: none; border-bottom: none;\">{3}</th>" +
                                        "</tr>", "No Preffered Time Available", item.ProductName.ToString().Trim(), item.Qty, item.DeliveryType.ToString().Trim()
                                         );
                                 }
                                 

                             }

                            
                            sbHtml.Append("</tbody></table>");
                            dictionary.Add("<!--ORDER_DETAILS-->", sbHtml.ToString());
                            dictionary.Add("<!--URL-->", "" + (new URLsFromConfig()).GetURL("MERCHANT") + "Placed");

                           
                            Flag = gateWay.SendEmail(GateWay.EmailGateWays.GANDHIBAGH, GateWay.SenderMail.INFO, GateWay.EMailTypes.CUST_ORD_DLVRD, new string[] { TOVM.Email }, dictionary, true);
                        }
                        catch (BusinessLogicLayer.MyException myEx)
                        {
                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                + "[DispatchedController][POST:Delivered][Email]" + myEx.EXCEPTION_PATH,
                                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                        }
                        catch (Exception ex)
                        {
                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + ex.Message + Environment.NewLine
                                + "[DispatchedController][POST:Delivered][Email]",
                                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                        }
                        //------Declartion SMS---------//
                        gateWay = new SMS(System.Web.HttpContext.Current.Server);
                        dictionary.Clear();
                        Flag = false;
                        try
                        {
                            //------Key value add in SMS---------//
                            dictionary.Add("#--NAME--#", TOVM.FirstName);
                            dictionary.Add("#--ORD_NUM--#", TOVM.OrderCode);
                            Flag = gateWay.SendSMS(GateWay.SMSGateWays.SUMIT, GateWay.SMSOptions.MULTIPLE, GateWay.SMSTypes.CUST_ORD_DLVRD, new string[] { TOVM.Mobile }, dictionary);

                        }
                        catch (BusinessLogicLayer.MyException myEx)
                        {
                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                + "[DispatchedController][POST:Delivered][SMS]" + myEx.EXCEPTION_PATH,
                                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                        }
                        catch (Exception ex)
                        {
                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + ex.Message + Environment.NewLine
                                + "[DispatchedController][POST:Delivered][SMS]",
                                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [SessionExpire]
        [Authorize(Roles = "Dispatched/CanWrite")]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "Cancel")]
        public ActionResult Cancel(TrackOrderViewModelList trackOrderViewModelList)
        {
            if (trackOrderViewModelList.LtrackOrderViewModelList != null)
            {

                foreach (TrackOrderViewModel TOVM in trackOrderViewModelList.LtrackOrderViewModelList)
                {
                    if (TOVM.CheckBoxStatus)
                    {
                        List<CustomerOrderDetail> ListCOD = db.CustomerOrderDetails.Where(x => x.ShopOrderCode == TOVM.ShopOrderCode).ToList();
                        foreach (CustomerOrderDetail COD in ListCOD)
                        {
                            using (var dbContextTransaction = db.Database.BeginTransaction())
                            {
                                try
                                {
                                    ////Log Table Insertion
                                    //LogTable logTableForCOD = new LogTable();
                                    //logTableForCOD.TableName = "CustomerOrderDetail";//table Name(Model Name)
                                    //logTableForCOD.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(COD);
                                    //logTableForCOD.TableRowID = COD.ID;
                                    //logTableForCOD.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                                    //long? rowOwnerIDforCOD = (COD.ModifyBy >= 0 ? COD.ModifyBy : COD.CreateBy);
                                    //logTableForCOD.RowOwnerID = (long)rowOwnerIDforCOD;
                                    //logTableForCOD.CreateDate = DateTime.UtcNow;
                                    //logTableForCOD.CreateBy = GetPersonalDetailID();//Session ID
                                    //db.LogTables.Add(logTableForCOD);

                                    //Customer Order Detail Table Status Update
                                    COD.OrderStatus = (int)ORDER_STATUS.CANCELLED;
                                    COD.ModifyDate = DateTime.UtcNow;
                                    COD.ModifyBy = GetPersonalDetailID();//Session ID
                                    COD.NetworkIP = CommonFunctions.GetClientIP();
                                    //COD.DeviceType = "x";
                                    //COD.DeviceID = "x";
                                    db.Entry(COD).State = EntityState.Modified;
                                    db.SaveChanges();

                                    //Shop Stock Table Update - Stock Qty Shop Product Wise
                                    ShopStock SS = db.ShopStocks.Find(COD.ShopStockID);

                                    ////Log Table Insertion
                                    //LogTable logTableForSS = new LogTable();
                                    //logTableForSS.TableName = "ShopStock";//table Name(Model Name)
                                    //logTableForSS.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(SS);
                                    //logTableForSS.TableRowID = SS.ID;
                                    //logTableForSS.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                                    //long? rowOwnerIDforSS = (SS.ModifyBy >= 0 ? SS.ModifyBy : SS.CreateBy);
                                    //logTableForSS.RowOwnerID = (long)rowOwnerIDforSS;
                                    //logTableForSS.CreateDate = DateTime.UtcNow;
                                    //logTableForSS.CreateBy = GetPersonalDetailID();//Session ID
                                    //db.LogTables.Add(logTableForSS);
                                    ////---------------------------------------------------------------------//

                                    SS.Qty = SS.Qty + COD.Qty;
                                    SS.ModifyDate = DateTime.UtcNow;
                                    SS.ModifyBy = GetPersonalDetailID();//Session ID
                                    SS.NetworkIP = CommonFunctions.GetClientIP();
                                    //SS.DeviceType = "x";
                                    //SS.DeviceID = "x";
                                    db.Entry(SS).State = EntityState.Modified;
                                    db.SaveChanges();

                                    //Customer Order History Table Status Insert
                                    CustomerOrderHistory COH = new CustomerOrderHistory();
                                    COH.CustomerOrderID = COD.CustomerOrderID;
                                    COH.ShopStockID = COD.ShopStockID;
                                    COH.Status = COD.OrderStatus;
                                    COH.IsActive = true;
                                    COH.CreateDate = DateTime.UtcNow;
                                    COH.CreateBy = GetPersonalDetailID();//Session ID
                                    COH.NetworkIP = CommonFunctions.GetClientIP();
                                    //COH.DeviceType = "x";
                                    //COH.DeviceID = "x";
                                    db.CustomerOrderHistories.Add(COH);
                                    db.SaveChanges();
                                    dbContextTransaction.Commit();

                                }
                                catch (BusinessLogicLayer.MyException myEx)
                                {
                                    dbContextTransaction.Rollback();
                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                        + "[DispatchedController][POST:Cancel]" + myEx.EXCEPTION_PATH,
                                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                                }
                                catch (Exception ex)
                                {
                                    dbContextTransaction.Rollback();
                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + ex.Message + Environment.NewLine
                                        + "[DispatchedController][POST:Cancel]",
                                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                                }
                            }

                        }
                        using (var dbContextTransaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                //Delivery Status Updated 
                                DeliveryOrderDetail DOD = db.DeliveryOrderDetails.Single(x => x.ShopOrderCode == TOVM.ShopOrderCode);
                                DOD.IsActive = false;
                                DOD.ModifyBy = GetPersonalDetailID();//Session ID
                                DOD.ModifyDate = DateTime.UtcNow;
                                DOD.NetworkIP = CommonFunctions.GetClientIP();
                                //DOD.DeviceType = "x";
                                //DOD.DeviceID = "x";
                                db.Entry(DOD).State = EntityState.Modified;
                                db.SaveChanges();
                                dbContextTransaction.Commit();

                            }
                            catch (BusinessLogicLayer.MyException myEx)
                            {
                                dbContextTransaction.Rollback();
                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                    + "[DispatchedController][POST:Cancel]" + myEx.EXCEPTION_PATH,
                                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                            }
                            catch (Exception ex)
                            {
                                dbContextTransaction.Rollback();
                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                    + "[DispatchedController][POST:Cancel]",
                                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                            }

                        }

                        CustomerOrderDetail cord = db.CustomerOrderDetails.Where(x => x.ShopOrderCode == TOVM.ShopOrderCode).FirstOrDefault();

                        Shop sp = db.Shops.Where(x => x.ID == cord.ShopID).FirstOrDefault();

                        //--------------------SMS & Email Function call------------------//
                        //------Declartion Email---------//
                        GateWay gateWay = new Email(System.Web.HttpContext.Current.Server);
                        Dictionary<string, string> dictionary = new Dictionary<string, string>();
                        bool Flag = false;
                        try
                        {
                            //------Key value add in Email---------//                           
                            string city = "nagpur";
                            int franchiseID = 2;////added for Multiple MCO in Same City
                            if (System.Web.HttpContext.Current.Request.Cookies["CityCookie"]!=null && System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value != "")
                            {
                                city = System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                                franchiseID = Convert.ToInt32(System.Web.HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[2]); ////added for Multiple MCO in Same City
                            }
                            dictionary.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseID + "/login");////added "/" + franchiseID + for Multiple MCO in Same City
                            dictionary.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + city + "/" + franchiseID + "/cust-o/my-order");////added "/" + franchiseID + for Multiple MCO in Same City
                            dictionary.Add("<!--NAME-->", sp.ContactPerson);
                            dictionary.Add("<!--ORDER_NO-->", cord.ShopOrderCode);
                            dictionary.Add("<!--REASON-->", "N/A");
                            Flag = gateWay.SendEmail(GateWay.EmailGateWays.GANDHIBAGH, GateWay.SenderMail.INFO, GateWay.EMailTypes.ORD_CANCELLED_MER, new string[] { sp.Email }, dictionary, true);
                        }
                        catch (BusinessLogicLayer.MyException myEx)
                        {
                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                + "[DispatchedController][POST:Cancel][Email]" + myEx.EXCEPTION_PATH,
                                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                        }
                        catch (Exception ex)
                        {
                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + ex.Message + Environment.NewLine
                                + "[DispatchedController][POST:Cancel][Email]",
                                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                        }
                        //------Declartion SMS---------//
                        gateWay = new SMS(System.Web.HttpContext.Current.Server);
                        dictionary.Clear();
                        Flag = false;
                        try
                        {
                            //------Key value add in SMS---------//
                            dictionary.Add("#--NAME--#", sp.ContactPerson);
                            dictionary.Add("#--ORD_NUM--#", cord.ShopOrderCode);
                            Flag = gateWay.SendSMS(GateWay.SMSGateWays.SUMIT, GateWay.SMSOptions.MULTIPLE, GateWay.SMSTypes.MER_ORD_CAN, new string[] { sp.Mobile }, dictionary);
                        }
                        catch (BusinessLogicLayer.MyException myEx)
                        {
                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                + "[DispatchedController][POST:Cancel][SMS]" + myEx.EXCEPTION_PATH,
                                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                        }
                        catch (Exception ex)
                        {
                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + ex.Message + Environment.NewLine
                                + "[DispatchedController][POST:Cancel][SMS]",
                                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [SessionExpire]
        [Authorize(Roles = "Dispatched/CanWrite")]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "Returned")]
        public ActionResult Returned(TrackOrderViewModelList trackOrderViewModelList)
        {
            if (trackOrderViewModelList.LtrackOrderViewModelList != null)
            {
                foreach (TrackOrderViewModel TOVM in trackOrderViewModelList.LtrackOrderViewModelList)
                {
                    if (TOVM.CheckBoxStatus)
                    {
                        List<CustomerOrderDetail> ListCOD = db.CustomerOrderDetails.Where(x => x.ShopOrderCode == TOVM.ShopOrderCode).ToList();
                        foreach (CustomerOrderDetail COD in ListCOD)
                        {
                            //EzeeloDBContext db1 = new EzeeloDBContext();
                            using (var dbContextTransaction = db.Database.BeginTransaction())
                            {
                                try
                                {
                                    ////Log Table Insertion
                                    //LogTable logTable = new LogTable();
                                    //logTable.TableName = "CustomerOrderDetail";//table Name(Model Name)
                                    //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(COD);
                                    //logTable.TableRowID = COD.ID;
                                    //logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                                    //long? rowOwnerID = (COD.ModifyBy >= 0 ? COD.ModifyBy : COD.CreateBy);
                                    //logTable.RowOwnerID = (long)rowOwnerID;
                                    //logTable.CreateDate = DateTime.UtcNow;
                                    //logTable.CreateBy = GetPersonalDetailID();//Session ID
                                    //db1.LogTables.Add(logTable);

                                    //Customer Order Detail Table Status Update
                                    COD.OrderStatus = (int)ORDER_STATUS.RETURNED;
                                    COD.ModifyDate = DateTime.UtcNow;
                                    COD.ModifyBy = GetPersonalDetailID();//Session ID
                                    COD.NetworkIP = CommonFunctions.GetClientIP();
                                    //COD.DeviceType = "x";
                                    //COD.DeviceID = "x";
                                    db.Entry(COD).State = EntityState.Modified;
                                    db.SaveChanges();

                                    ////Shop Stock Table Update - Stock Qty Shop Product Wise
                                    //EzeeloDBContext db2 = new EzeeloDBContext();
                                    //ShopStock SS2 = db2.ShopStocks.Find(COD.ShopStockID);
                                    ShopStock SS = db.ShopStocks.Find(COD.ShopStockID);

                                    //////Log Table Insertion
                                    //LogTable logTableForSS = new LogTable();
                                    //logTableForSS.TableName = "ShopStock";//table Name(Model Name)
                                    //logTableForSS.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(SS2);
                                    //logTableForSS.TableRowID = SS.ID;
                                    //logTableForSS.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                                    //long? rowOwnerIDforSS = (SS.ModifyBy >= 0 ? SS.ModifyBy : SS.CreateBy);
                                    //logTableForSS.RowOwnerID = (long)rowOwnerIDforSS;
                                    //logTableForSS.CreateDate = DateTime.UtcNow;
                                    //logTableForSS.CreateBy = GetPersonalDetailID();//Session ID
                                    //db1.LogTables.Add(logTableForSS);
                                    //////---------------------------------------------------------------------//

                                    SS.Qty = SS.Qty + COD.Qty;
                                    SS.ModifyDate = DateTime.UtcNow;
                                    SS.ModifyBy = GetPersonalDetailID();//Session ID
                                    SS.NetworkIP = CommonFunctions.GetClientIP();
                                    //SS.DeviceType = "x";
                                    //SS.DeviceID = "x";
                                    db.Entry(SS).State = EntityState.Modified;
                                    db.SaveChanges();

                                    //Customer Order History Table Status Insert
                                    CustomerOrderHistory COH = new CustomerOrderHistory();
                                    COH.CustomerOrderID = COD.CustomerOrderID;
                                    COH.ShopStockID = COD.ShopStockID;
                                    COH.Status = COD.OrderStatus;
                                    COH.IsActive = true;
                                    COH.CreateDate = DateTime.UtcNow;
                                    COH.CreateBy = GetPersonalDetailID();//Session ID
                                    COH.NetworkIP = CommonFunctions.GetClientIP();
                                    //COH.DeviceType = "x";
                                    //COH.DeviceID = "x";
                                    db.CustomerOrderHistories.Add(COH);
                                    db.SaveChanges();
                                    dbContextTransaction.Commit();
                                }
                                catch (BusinessLogicLayer.MyException myEx)
                                {
                                    dbContextTransaction.Rollback();
                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                        + "[DispatchedController][POST:Returned]" + myEx.EXCEPTION_PATH,
                                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                                }
                                catch (Exception ex)
                                {
                                    dbContextTransaction.Rollback();
                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + ex.Message + Environment.NewLine
                                        + "[DispatchedController][POST:Returned]",
                                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                                }
                            }
                        }


                        //--------------------SMS & Email Function call------------------//
                        //------Declartion Email---------//

                        CustomerOrderDetail cord = db.CustomerOrderDetails.Where(x => x.ShopOrderCode == TOVM.ShopOrderCode).FirstOrDefault();

                        Shop sp = db.Shops.Where(x => x.ID == cord.ShopID).FirstOrDefault();

                        ModelLayer.Models.CustomerOrder custOrder = db.CustomerOrders.Where(x => x.ID == cord.CustomerOrderID).FirstOrDefault();

                        PersonalDetail pd = db.PersonalDetails.Where(x => x.UserLoginID == custOrder.UserLoginID).FirstOrDefault();

                        UserLogin ul = db.UserLogins.Where(x => x.ID == pd.UserLoginID).FirstOrDefault();

                        GateWay gateWay = new Email(System.Web.HttpContext.Current.Server);
                        Dictionary<string, string> dictionary = new Dictionary<string, string>();
                        bool Flag = false;
                        try
                        {
                            //------Key value add in Email---------//
                            dictionary.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "login/login");
                            dictionary.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "login/login");
                            dictionary.Add("<!--NAME-->", sp.Name);

                            dictionary.Add("<!--ORDER_NO-->", cord.ShopOrderCode);
                            dictionary.Add("<!--CUSTOMER_NAME-->", pd.FirstName + " " + pd.MiddleName + " " + pd.LastName);
                            dictionary.Add("<!--DELIVERY_ADDRESS-->", custOrder.ShippingAddress);

                            dictionary.Add("<!--ORDER_DATE-->", custOrder.CreateDate.ToString("MMM dd, yyyy"));
                            dictionary.Add("<!--ORDER_TIME-->", custOrder.CreateDate.ToShortTimeString());
                            dictionary.Add("<!--CONTACT_NUMBER-->", custOrder.PrimaryMobile);


                            Flag = gateWay.SendEmail(GateWay.EmailGateWays.GANDHIBAGH, GateWay.SenderMail.INFO, GateWay.EMailTypes.CUST_ORD_RTND, new string[] { sp.Email }, dictionary, true);
                        }
                        catch (BusinessLogicLayer.MyException myEx)
                        {
                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                + "[PlacedController][POST:Cancel][Email]" + myEx.EXCEPTION_PATH,
                                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                        }
                        catch (Exception ex)
                        {
                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + ex.Message + Environment.NewLine
                                + "[PlacedController][POST:Cancel][Email]",
                                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                        }

                        //------Declartion SMS---------//
                        gateWay = new SMS(System.Web.HttpContext.Current.Server);
                        dictionary.Clear();
                        Flag = false;
                        try
                        {
                            //------Key value add in SMS---------//
                            dictionary.Add("#--NAME--#", sp.ContactPerson);
                            dictionary.Add("#--ORD_NUM--#", cord.ShopOrderCode);
                            Flag = gateWay.SendSMS(GateWay.SMSGateWays.SUMIT, GateWay.SMSOptions.MULTIPLE, GateWay.SMSTypes.CUST_ORD_RTRND, new string[] { sp.Mobile, custOrder.PrimaryMobile }, dictionary);
                        }
                        catch (BusinessLogicLayer.MyException myEx)
                        {
                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                + "[PlacedController][POST:Cancel][SMS]" + myEx.EXCEPTION_PATH,
                                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                        }
                        catch (Exception ex)
                        {
                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + ex.Message + Environment.NewLine
                                + "[PlacedController][POST:Cancel][SMS]",
                                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }
       
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
