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
using Administrator.Models;
using BusinessLogicLayer;

namespace Administrator.Controllers
{
    public class CancelledController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        [SessionExpire]
        // GET: /Cancelled/
        public ActionResult Index()
        {
            TrackOrderViewModelList TOVML = new TrackOrderViewModelList();
            List<TrackOrderViewModel> listTrackOrder = new List<TrackOrderViewModel>();
            try
            {
                var IsDeliveryOutSource = (from S in db.Shops select new { S.ID,S.IsDeliveryOutSource }).ToList();
                foreach (var IsDelivery in IsDeliveryOutSource)
                {
                    if (IsDelivery.IsDeliveryOutSource.Equals(false))
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
                    where
                        //COD.ShopID.Equals(ShopID) && 
                    COD.OrderStatus.Equals((int)ORDER_STATUS.CANCELLED)
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
                    }).ToList();

                        foreach (var ReadRecord in queryResult)
                        {
                            TrackOrderViewModel tovm = new TrackOrderViewModel();
                            tovm.CustomerOrderID = ReadRecord.CustomerOrderID;
                            tovm.OrderCode = ReadRecord.OrderCode;
                            tovm.ShopOrderCode = ReadRecord.ShopOrderCode;
                            tovm.FirstName = ReadRecord.FirstName + " " + ReadRecord.MiddleName + " " + ReadRecord.LastName;
                            tovm.CreateDate = ReadRecord.CreateDate;
                            tovm.PName = ReadRecord.Name;
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
                            tovm.ImageLocation = AdminCommonFunctions.FindProductDefaultImageLocation(ReadRecord.ProductID);
                            listTrackOrder.Add(tovm);
                        }



                    }
                    if (IsDelivery.IsDeliveryOutSource.Equals(true))
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
                    where
                        //COD.ShopID.Equals(ShopID) && 
                    COD.OrderStatus.Equals((int)ORDER_STATUS.CANCELLED)
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
                    }).ToList();


                        foreach (var ReadRecord in queryResult)
                        {
                            TrackOrderViewModel tovm = new TrackOrderViewModel();
                            tovm.CustomerOrderID = ReadRecord.CustomerOrderID;
                            tovm.OrderCode = ReadRecord.OrderCode;
                            tovm.ShopOrderCode = ReadRecord.ShopOrderCode;
                            tovm.FirstName = ReadRecord.FirstName + " " + ReadRecord.MiddleName + " " + ReadRecord.LastName;
                            tovm.CreateDate = ReadRecord.CreateDate;
                            tovm.PName = ReadRecord.Name;
                            tovm.PackSize = ReadRecord.PackSize;
                            tovm.CODQty = ReadRecord.Qty;
                            tovm.Name = ReadRecord.UnitName;//pack unit 
                            tovm.RetailerRate = ReadRecord.RetailerRate;
                            tovm.TotalAmount = ReadRecord.TotalAmount;
                            tovm.OrderAmount = ReadRecord.OrderAmount;
                            tovm.GandhibaghCharge = ReadRecord.GandhibaghCharge;
                            tovm.PayableAmount = ReadRecord.PayableAmount;
                            tovm.IsDeliveryOutSource = ReadRecord.IsDeliveryOutSource;
                            tovm.ImageLocation = AdminCommonFunctions.FindProductDefaultImageLocation(ReadRecord.ProductID);
                            listTrackOrder.Add(tovm);
                        }



                    }
                } TOVML.LtrackOrderViewModelList = listTrackOrder;
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CancelledController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CancelledController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View(TOVML);
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
