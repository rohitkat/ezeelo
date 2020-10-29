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
using Merchant.Common;

namespace Merchant.Controllers
{
    public class ConfirmController : Controller
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
                throw new BusinessLogicLayer.MyException("[ConfirmController][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
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
                throw new BusinessLogicLayer.MyException("[ConfirmController][GetShopID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }

        [SessionExpire]
        [Authorize(Roles = "Confirm/CanRead")]
        public ActionResult Index()
        {
            List<TrackOrderViewModel> listTrackOrder = new List<TrackOrderViewModel>();
            long ShopID = GetShopID();
            try
            {
                DateTime dt = new DateTime(1, 1, 1);
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
                where COD.ShopID.Equals(ShopID) && COD.OrderStatus.Equals((int)ORDER_STATUS.CONFIRM)
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
                    ProductID = P.ID,
                    //CO.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
                    DeliveryDate = CO.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate == null ? dt : CO.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
                    CO.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName
                }).OrderByDescending(x => x.CreateDate).ToList();
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
                    tovm.ImageLocation = MerchantCommonFunction.FindProductDefaultImageLocation(ReadRecord.ProductID);
                    
                    tovm.DeliveryDate = ReadRecord.DeliveryDate;
                    tovm.DeliveryScheduleName = ReadRecord.DisplayName;

                    listTrackOrder.Add(tovm);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ConfirmController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ConfirmController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            TrackOrderViewModelList TOVML = new TrackOrderViewModelList();
            TOVML.LtrackOrderViewModelList = listTrackOrder;
            return View(TOVML);
        }

        [HttpPost]
        [SessionExpire]
        [Authorize(Roles = "Confirm/CanWrite")]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "Packed")]
        public ActionResult Packed(TrackOrderViewModelList trackOrderViewModelList)
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
                                    COD.OrderStatus = (int)ORDER_STATUS.PACKED;
                                    COD.ModifyDate = DateTime.UtcNow;
                                    COD.ModifyBy = GetPersonalDetailID();//Session ID;
                                    COD.NetworkIP = CommonFunctions.GetClientIP();
                                    //COD.DeviceType = "x";
                                    //COD.DeviceID = "x";
                                    db.Entry(COD).State = EntityState.Modified;
                                    db.SaveChanges();

                                    //Customer Order History Table Status Insert
                                    CustomerOrderHistory COH = new CustomerOrderHistory();
                                    COH.CustomerOrderID = COD.CustomerOrderID;
                                    COH.ShopStockID = COD.ShopStockID;
                                    COH.Status = COD.OrderStatus;
                                    COH.IsActive = true;
                                    COH.CreateDate = DateTime.UtcNow;
                                    COH.CreateBy = GetPersonalDetailID();//Session ID;
                                    COH.NetworkIP = CommonFunctions.GetClientIP();
                                    //COH.DeviceType = "x";
                                    //COH.DeviceID = "x";
                                    db.CustomerOrderHistories.Add(COH);
                                    db.SaveChanges();
                                    dbContextTransaction.Commit();


                                    // send sms to delivery partner after consignment packed


                                    //GateWay gateway = new SMS(System.Web.HttpContext.Current.Server);
                                    //Dictionary<string, string> dictionary = new Dictionary<string, string>();


                                    ////------Key value add in SMS---------//
                                    //dictionary.Add("#--NAME--#", "Delivery Partner");
                                    //dictionary.Add("#--ORD_NUM--#", TOVM.OrderCode);
                                    //dictionary.Add("#--TIME--#", "N.A.");
                                    //dictionary.Add("#--ADDRESS--#", TOVM.ShippingAddress);

                                    //gateway.SendSMS(GateWay.SMSGateWays.SUMIT, GateWay.SMSOptions.MULTIPLE, GateWay.SMSTypes.DP_DSPTCH, new string[] { TOVM.Mobile }, dictionary);


                                }
                                catch (BusinessLogicLayer.MyException myEx)
                                {
                                    dbContextTransaction.Rollback();
                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                        + "[ConfirmController][POST:Packed]" + myEx.EXCEPTION_PATH,
                                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                                }
                                catch (Exception ex)
                                {
                                    dbContextTransaction.Rollback();
                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + ex.Message + Environment.NewLine
                                        + "[ConfirmController][POST:Packed]",
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

                                //-----------Code of delivery Partner change----------------//
                                long MerchentId = GetShopID();
                                int lDeliveryPartnerId = DOD.DeliveryPartnerID;
                                Boolean lIsShopHandleDeliveryProcess = Common.Common.IsShopHandleDeliveryProcess(MerchentId, ref lDeliveryPartnerId);

                                if (lIsShopHandleDeliveryProcess == false)
                                {
                                    decimal DeliveryCharges = Common.Common.GetDeliveryCharges(lDeliveryPartnerId, Convert.ToDecimal(0), DOD.DeliveryType, DOD.Weight);
                                    DOD.DeliveryPartnerID = lDeliveryPartnerId;
                                    DOD.DeliveryCharge = DeliveryCharges;
                                }

                                //---------------------End of Code------------------------//
                                ////Log Table Insertion
                                //LogTable logTable = new LogTable();
                                //logTable.TableName = "DeliveryOrderDetail";//table Name(Model Name)
                                //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(DOD);
                                //logTable.TableRowID = DOD.ID;
                                //logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                                //long? rowOwnerID = (DOD.ModifyBy >= 0 ? DOD.ModifyBy : DOD.CreateBy);
                                //logTable.RowOwnerID = (long)rowOwnerID;
                                //logTable.CreateDate = DateTime.UtcNow;
                                //logTable.CreateBy = GetPersonalDetailID();//Session ID
                                //db.LogTables.Add(logTable);

                                DOD.IsActive = true;
                                DOD.ModifyDate = DateTime.UtcNow;
                                DOD.ModifyBy = GetPersonalDetailID();
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
                                    + "[ConfirmController][POST:Packed]" + myEx.EXCEPTION_PATH,
                                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                            }
                            catch (Exception ex)
                            {
                                dbContextTransaction.Rollback();
                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                    + "[ConfirmController][POST:Packed]",
                                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                            }
                            //try
                            //{
                            //    GateWay gateway = new SMS(System.Web.HttpContext.Current.Server);
                            //    Dictionary<string, string> dictionary = new Dictionary<string, string>();


                            //    //------Key value add in SMS---------//
                            //    dictionary.Add("#--NAME--#", "Delivery Partner");
                            //    dictionary.Add("#--ORD_NUM--#", TOVM.OrderCode);
                            //    dictionary.Add("#--TIME--#", "N.A.");
                            //    dictionary.Add("#--ADDRESS--#", TOVM.ShippingAddress);

                            //    //---Added by mohit on 23-01-16 for help line number as per city---//
                            //    List<CityHelpLineNoViewModel> lCitlHelpLine = new List<CityHelpLineNoViewModel>();
                            //    lCitlHelpLine = BusinessLogicLayer.CityHelpLineNo.GetCityHelpLineNo(TOVM.ShopID).ToList();
                            //    dictionary.Add("<!--HELP_DESK-->", lCitlHelpLine[0].HelpLineNumber);
                            //    //---End Of Code By Mohit---//

                            //    gateway.SendSMS(GateWay.SMSGateWays.SUMIT, GateWay.SMSOptions.MULTIPLE, GateWay.SMSTypes.DP_DSPTCH, new string[] { TOVM.Mobile }, dictionary);

                            //}
                            //catch (BusinessLogicLayer.MyException myEx)
                            //{
                               
                            //    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                            //        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                            //        + "[ConfirmController][POST:Packed]" + myEx.EXCEPTION_PATH,
                            //        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                            //}
                            //catch (Exception ex)
                            //{
                               
                            //    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                            //        + Environment.NewLine + ex.Message + Environment.NewLine
                            //        + "[ConfirmController][POST:Packed]",
                            //        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                            //}
                        }
                        ////--------------------SMS & Email Function call------------------//
                        ////------Declartion Email---------//
                        //GateWay gateWay = new Email(System.Web.HttpContext.Current.Server);
                        //Dictionary<string, string> dictionary = new Dictionary<string, string>();
                        //bool Flag = false;
                        //try
                        //{
                        //    //------Key value add in Email---------//
                        //    dictionary.Add("<!--NAME-->", TOVM.FirstName);
                        //    dictionary.Add("<!--ORDER_NO-->", TOVM.OrderCode);
                        //    dictionary.Add("<!--ORDER_DATE-->", TOVM.CreateDate.ToString());
                        //    Flag = gateWay.SendEmail(GateWay.EmailGateWays.GANDHIBAGH, GateWay.SenderMail.INFO, GateWay.EMailTypes.CUST_ORD_CNF, new string[] { TOVM.Email }, dictionary, true);
                        //}
                        //catch (MyException myException)
                        //{
                        //    ErrorLog.ErrorLogFile(myException.EXCEPTION_MSG + "\n" + myException.EXCEPTION_PATH, ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                        //}
                        ////------Declartion SMS---------//
                        //gateWay = new SMS(System.Web.HttpContext.Current.Server);
                        //dictionary.Clear();
                        //Flag = false;
                        //try
                        //{
                        //    //------Key value add in SMS---------//
                        //    dictionary.Add("#--NAME--#", TOVM.FirstName);
                        //    dictionary.Add("#--ORD_NUM--#", TOVM.OrderCode);
                        //    Flag = gateWay.SendSMS(GateWay.SMSGateWays.SUMIT, GateWay.SMSOptions.MULTIPLE, GateWay.SMSTypes.CUST_ORD_CNF, new string[] { TOVM.Mobile }, dictionary);
                        //}
                        //catch (MyException myException)
                        //{
                        //    ErrorLog.ErrorLogFile(myException.EXCEPTION_MSG + "\n" + myException.EXCEPTION_PATH, ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                        //}
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [SessionExpire]
        [Authorize(Roles = "Confirm/CanWrite")]
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
                                {  //Log Table Insertion
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
                                    COD.ModifyBy = GetPersonalDetailID();
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
                                    //---------------------------------------------------------------------//

                                    SS.Qty = SS.Qty + COD.Qty;
                                    SS.ModifyDate = DateTime.UtcNow;
                                    SS.ModifyBy = GetPersonalDetailID();
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
                                    COH.CreateBy = GetPersonalDetailID();
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
                                        + "[ConfirmController][POST:Cancel]" + myEx.EXCEPTION_PATH,
                                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                                }
                                catch (Exception ex)
                                {
                                    dbContextTransaction.Rollback();
                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + ex.Message + Environment.NewLine
                                        + "[ConfirmController][POST:Cancel]",
                                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                                }
                            }
                        }


                        //--------------------SMS & Email Function call------------------//
                        //------Declartion Email---------//

                        CustomerOrderDetail cord = db.CustomerOrderDetails.Where(x => x.ShopOrderCode == TOVM.ShopOrderCode).FirstOrDefault();

                        Shop sp = db.Shops.Where(x => x.ID == cord.ShopID).FirstOrDefault();

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
                            dictionary.Add("<!--NAME-->", sp.Name);
                            dictionary.Add("<!--ORDER_NO-->", cord.ShopOrderCode);
                            dictionary.Add("<!--REASON-->", "N/A");

                            Flag = gateWay.SendEmail(GateWay.EmailGateWays.GANDHIBAGH, GateWay.SenderMail.INFO, GateWay.EMailTypes.ORD_CANCELLED_MER, new string[] { sp.Email }, dictionary, true);
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
                            Flag = gateWay.SendSMS(GateWay.SMSGateWays.SUMIT, GateWay.SMSOptions.SINGLE, GateWay.SMSTypes.MER_ORD_CAN, new string[] { sp.Mobile }, dictionary);
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
