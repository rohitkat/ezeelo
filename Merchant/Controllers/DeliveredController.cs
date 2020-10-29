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
    public class DeliveredController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private long GetShopID()
        {
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
                throw new BusinessLogicLayer.MyException("[DeliveredController][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
            }
            return ShopID;
        }
        private long GetPersonalDetailID()
        {
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
                throw new BusinessLogicLayer.MyException("[DeliveredController][GetShopID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }
        // GET: /Delivered/
         [SessionExpire]
         [Authorize(Roles = "Delivered/CanRead")]
        public ActionResult Index()
        {
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
                where COD.ShopID.Equals(ShopID) && COD.OrderStatus.Equals((int)ORDER_STATUS.DELIVERED)
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
                where COD.ShopID.Equals(ShopID) && COD.OrderStatus.Equals((int)ORDER_STATUS.DELIVERED)
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
                    + "[DeliveredController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveredController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View(TOVML);
        }
         [HttpPost]
         [SessionExpire]
         [Authorize(Roles = "Delivered/CanWrite")]
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
                         GateWay gateWay = new Email(System.Web.HttpContext.Current.Server);
                         Dictionary<string, string> dictionary = new Dictionary<string, string>();
                         bool Flag = false;
                         try
                         {
                             //------Key value add in Email---------//
                             dictionary.Add("<!--NAME-->", TOVM.FirstName);
                             dictionary.Add("<!--ORDER_NO-->", TOVM.OrderCode);
                             dictionary.Add("<!--ORDER_DATE-->", TOVM.CreateDate.ToString("MMM dd, yyyy"));
                             Flag = gateWay.SendEmail(GateWay.EmailGateWays.GANDHIBAGH, GateWay.SenderMail.INFO, GateWay.EMailTypes.CUST_ORD_RTND, new string[] { TOVM.Email }, dictionary, true);
                         }
                         catch (BusinessLogicLayer.MyException myEx)
                         {
                             BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                 + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                 + "[DispatchedController][POST:Returned][Email]" + myEx.EXCEPTION_PATH,
                                 BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                         }
                         catch (Exception ex)
                         {
                             BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                 + Environment.NewLine + ex.Message + Environment.NewLine
                                 + "[DispatchedController][POST:Returned][Email]",
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
                             Flag = gateWay.SendSMS(GateWay.SMSGateWays.SUMIT, GateWay.SMSOptions.MULTIPLE, GateWay.SMSTypes.CUST_ORD_RTRND, new string[] { TOVM.Mobile }, dictionary);
                         }
                         catch (BusinessLogicLayer.MyException myEx)
                         {
                             BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                 + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                 + "[DispatchedController][POST:Returned][SMS]" + myEx.EXCEPTION_PATH,
                                 BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                         }
                         catch (Exception ex)
                         {
                             BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                 + Environment.NewLine + ex.Message + Environment.NewLine
                                 + "[DispatchedController][POST:Returned][SMS]",
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
