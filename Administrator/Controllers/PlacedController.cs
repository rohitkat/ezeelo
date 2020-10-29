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
    public class PlacedController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        //private int ShopID = 6;
        // GET: /Placed/
        [SessionExpire]
        public ActionResult Index()
        {
            List<TrackOrderViewModel> listTrackOrder = new List<TrackOrderViewModel>();
            try
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
                where 
                //COD.ShopID.Equals(ShopID) && 
                COD.OrderStatus.Equals((int)ORDER_STATUS.PLACED)
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
                    tovm.ImageLocation = AdminCommonFunctions.FindProductDefaultImageLocation(ReadRecord.ProductID);
                    listTrackOrder.Add(tovm);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PlacedController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PlacedController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            TrackOrderViewModelList TOVML = new TrackOrderViewModelList();
            TOVML.LtrackOrderViewModelList = listTrackOrder;
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
