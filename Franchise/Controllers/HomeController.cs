using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Franchise.Models;
using ModelLayer.Models;
using System.Data.Entity.Core.Objects;
using BusinessLogicLayer;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
    public class HomeController : Controller
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
        //        throw new BusinessLogicLayer.MyException("[HomeController][GetFranchiseID]", "Can't find FranchiseID !" + Environment.NewLine + ex.Message);
        //    }
        //    return FranchiseID;
        //}
        [SessionExpire]        
        public ActionResult Index()
        {
            try
            {
                DateTime dateTime = DateTime.Now.Date;
                long FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);// GetFranchiseID();

                Session["ID"] = Session["USER_LOGIN_ID"];
                //--------------------------------1--------------------------------------//
                ViewBag.TotalShop =
                (from S in db.Shops
                 where S.FranchiseID == FranchiseID
                 select new { S.ID }).Count();
                //--------------------------------2--------------------------------------//
                //ViewBag.PendingShop =
                //(from S in db.Shops
                // where S.IsLive == false && S.FranchiseID == FranchiseID
                // select new { S.ID }).Count();

                ViewBag.TodayApprovedShop =
                (from S in db.Shops
                 join bd in db.BusinessDetails on S.BusinessDetailID equals bd.ID
                 join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                 where ul.IsLocked == false && S.FranchiseID == FranchiseID && EntityFunctions.TruncateTime(ul.ModifyDate)==dateTime 
                 select new { S.ID }).Count();
                //--------------------------------3--------------------------------------//
                ViewBag.TodayApprovedProduct =               
                db.ShopProducts.Where(x=>EntityFunctions.TruncateTime(x.CreateDate)==dateTime && x.Shop.FranchiseID == FranchiseID).Select(x=>x.ID).Count();
                //--------------------------------4--------------------------------------//
                int approvalStatus = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.APPROVED);
                ViewBag.PendingProduct =
                (from TSP in db.TempShopProducts
                 join TP in db.TempProducts on TSP.ProductID equals TP.ID
                 where TSP.IsActive == true && TSP.Shop.FranchiseID == FranchiseID && TP.ApprovalStatus != approvalStatus
                 select new { TSP.ID }).Count();
                //----------------------------------5------------------------------------//
                //ViewBag.OrderPlaced = (from CO in db.CustomerOrders
                //                       join COD in db.CustomerOrderDetails on CO.ID equals (COD.CustomerOrderID)
                //                       join SS in db.ShopStocks on COD.ShopStockID equals SS.ID
                //                       join SP in db.ShopProducts on SS.ShopProductID equals SP.ID
                //                       join S in db.Shops on SP.ShopID equals S.ID
                //                       where S.FranchiseID == FranchiseID && COD.OrderStatus==(int)Common.Constant.ORDER_STATUS.PLACED
                //                       select new {CO.ID }).Distinct().Count();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[HomeController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[HomeController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }

            return View();
        }

        [SessionExpire]
        public ActionResult AccessDenied(string returnUrl)
        {
            ViewBag.returnurl = returnUrl.Replace("/", "").ToString();
            //Response.StatusCode = 403;
            //return View();
            return View();
        }
    }
}