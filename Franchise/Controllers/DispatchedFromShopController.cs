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
using Franchise.Models;
using BusinessLogicLayer;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
    public class DispatchedFromShopController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        //private long GetFranchiseID()
        //{
        //    //Session["USER_LOGIN_ID"] = 2;
        //    long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
        //    long BusinessDetailID = 0;
        //    long FranchiseID = 0;
        //    try
        //    {
        //        if (UserLoginID > 0)
        //        {
        //            BusinessDetailID = Convert.ToInt32(db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
        //            FranchiseID = Convert.ToInt32(db.Franchises.Where(x => x.BusinessDetailID == BusinessDetailID).Select(x => x.ID).First());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessLogicLayer.MyException("[DispatchedFromShopController][GetFranchiseID]", "Can't find FranchiseID !" + Environment.NewLine + ex.Message);
        //    }
        //    return FranchiseID;
        //}
        // GET: /DispatchedFromShop/
        [SessionExpire]
        [CustomAuthorize(Roles = "DispatchedFromShop/CanRead")]
        public ActionResult Index()
        {
            TrackOrderViewModelList TOVML = new TrackOrderViewModelList();
            List<TrackOrderViewModel> listTrackOrder = new List<TrackOrderViewModel>();
            long FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //GetFranchiseID();
            try
            {
                var IsDeliveryOutSource = (from S in db.Shops.Where(x => x.FranchiseID==(FranchiseID)) select new { S.ID, S.IsDeliveryOutSource }).ToList();
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
                    where COD.ShopID.Equals(IsDelivery.ID) && COD.OrderStatus.Equals((int)ORDER_STATUS.DISPATCHED_FROM_SHOP)
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
                        ProductID = P.ID,
                        ShopName=S.Name
                    }).ToList();


                        foreach (var ReadRecord in queryResult)
                        {
                            TrackOrderViewModel tovm = new TrackOrderViewModel();
                            tovm.ShopName = ReadRecord.ShopName;
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
                            tovm.ImageLocation = FranchiseCommonFunction.FindProductDefaultImageLocation(ReadRecord.ProductID);
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
                    where COD.ShopID.Equals(IsDelivery.ID) && COD.OrderStatus.Equals((int)ORDER_STATUS.DISPATCHED_FROM_SHOP)
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
                        ProductID = P.ID,
                        ShopName = S.Name
                    }).ToList();


                        foreach (var ReadRecord in queryResult)
                        {
                            TrackOrderViewModel tovm = new TrackOrderViewModel();
                            tovm.ShopName = ReadRecord.ShopName;
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
                            tovm.ImageLocation = FranchiseCommonFunction.FindProductDefaultImageLocation(ReadRecord.ProductID);
                            listTrackOrder.Add(tovm);
                        }



                    }
                }
                TOVML.LtrackOrderViewModelList = listTrackOrder;
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DispatchedFromShopController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DispatchedFromShopController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
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
