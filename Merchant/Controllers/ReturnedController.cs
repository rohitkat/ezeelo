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
    public class ReturnedController : Controller
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
                throw new BusinessLogicLayer.MyException("[ReturnedController][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
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
                throw new BusinessLogicLayer.MyException("[ReturnedController][GetShopID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }
        // GET: /Returned/
        [SessionExpire]
        [Authorize(Roles = "Returned/CanRead")]
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
                where COD.ShopID.Equals(ShopID) && COD.OrderStatus.Equals((int)ORDER_STATUS.RETURNED)
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
                }).ToList();
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
                where COD.ShopID.Equals(ShopID) && COD.OrderStatus.Equals((int)ORDER_STATUS.RETURNED)
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
                }).ToList();
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
                    + "[ReturnedController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ReturnedController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
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
