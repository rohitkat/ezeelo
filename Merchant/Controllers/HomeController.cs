using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.Enum;
using Merchant.Models;
using Merchant.Common;

namespace Merchant.Controllers
{
    [AllowCrossSiteAttributes]
    public class HomeController : Controller
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
                throw new BusinessLogicLayer.MyException("[HomeController][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
            }
            return ShopID;
        }
        [SessionExpire]
       // [Authorize(Roles = "Home/CanRead")]
        public ActionResult Index()
        {

            DateTime dateTime = DateTime.Now.Date;
            long ShopID = GetShopID();
            //--------------------------------1--------------------------------------//
            ViewBag.Pending =
                (from COD in db.CustomerOrderDetails
                 where COD.OrderStatus == (int)ORDER_STATUS.PLACED && COD.ShopID == ShopID
                 select new { COD.CustomerOrderID }).Distinct().Count();
            //--------------------------------2--------------------------------------//
            ViewBag.Confirm =
                (from COD in db.CustomerOrderDetails
                 where COD.OrderStatus == (int)ORDER_STATUS.CONFIRM && COD.ShopID == ShopID
                 select new { COD.CustomerOrderID }).Distinct().Count();
            //--------------------------------3--------------------------------------//
            ViewBag.Packed =
                (from COD in db.CustomerOrderDetails
                 where COD.OrderStatus == (int)ORDER_STATUS.PACKED && COD.ShopID == ShopID
                 select new { COD.CustomerOrderID }).Distinct().Count();
            //--------------------------------4--------------------------------------//
            ViewBag.Dispatched =
                (from COD in db.CustomerOrderDetails
                 where COD.OrderStatus == (int)ORDER_STATUS.DISPATCHED_FROM_SHOP && COD.ShopID == ShopID
                 select new { COD.CustomerOrderID }).Distinct().Count();
            //--------------------------------5--------------------------------------//
            ViewBag.Delivered =
                (from COD in db.CustomerOrderDetails
                 where COD.OrderStatus == (int)ORDER_STATUS.DELIVERED && COD.ShopID == ShopID
                 select new { COD.CustomerOrderID }).Distinct().Count();
            //--------------------------------6--------------------------------------//
            ViewBag.Returned =
                (from COD in db.CustomerOrderDetails
                 where COD.OrderStatus == (int)ORDER_STATUS.RETURNED && COD.ShopID == ShopID
                 select new { COD.CustomerOrderID }).Distinct().Count();
            //--------------------------------7--------------------------------------//
            ViewBag.Cancelled =
                (from COD in db.CustomerOrderDetails
                 where COD.OrderStatus == (int)ORDER_STATUS.CANCELLED && COD.ShopID == ShopID
                 select new { COD.CustomerOrderID }).Distinct().Count();
            //--------------------------------8--------------------------------------//
            ViewBag.TodayApprovedProduct =
            (from SP in db.ShopProducts
             where SP.CreateDate == dateTime && SP.ShopID == ShopID
             select new { SP.ID }).Count();
            //--------------------------------9--------------------------------------//
            ViewBag.PendingProduct =
            (from TSP in db.TempShopProducts
             where TSP.ShopID == ShopID
             //where TSP.IsActive == false && TSP.ShopID == ShopID
             select new { TSP.ID }).Count();
            //--------------------------------10--------------------------------------//
            ViewBag.TotalProduct =
            (from SP in db.ShopProducts
             where SP.IsActive == true && SP.ShopID == ShopID
             select new { SP.ID }).Count();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}