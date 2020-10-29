using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using Administrator.Models;
using ModelLayer.Models.Enum;
using BusinessLogicLayer;

namespace Administrator.Controllers
{
    public class HomeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        [SessionExpire]
        public ActionResult Index()
        {
            DateTime dateTime = DateTime.Now.Date;
            try
            {
                //--------------------------------1--------------------------------------//
                ViewBag.TotalCustomer =
                 (from P in db.PersonalDetails
                  where !
                  (from B in db.BusinessDetails
                   select new
                   {
                       B.UserLoginID
                   }).Contains(new { UserLoginID = P.UserLoginID })
                  select new { P.UserLoginID }).Count();
                //--------------------------------2--------------------------------------//
                ViewBag.TodayRegisterCustomer =
                (from P in db.PersonalDetails
                 where P.CreateDate == dateTime &&
                     !(from B in db.BusinessDetails
                       select new
                       {
                           B.UserLoginID
                       }).Contains(new { UserLoginID = P.UserLoginID })
                 select new { P.UserLoginID }).Count();
                //--------------------------------3--------------------------------------//
                ViewBag.TodayApprovedProduct =
                (from SP in db.ShopProducts
                 where SP.CreateDate == dateTime
                 select new { SP.ID }).Count();
                //--------------------------------4--------------------------------------//
                ViewBag.PendingProduct =
                (from TSP in db.TempShopProducts
                 where TSP.IsActive == false
                 select new { TSP.ID }).Count();
                //--------------------------------5--------------------------------------//
                ViewBag.TodayTotalOrder =
                 (from COD in db.CustomerOrderDetails
                  where COD.CreateDate == dateTime
                  select new { COD.ID }).Count();
                //--------------------------------6--------------------------------------//
                ViewBag.TodayPendingOrder =
                (from COD in db.CustomerOrderDetails
                 where COD.CreateDate == dateTime && COD.OrderStatus == (int)ORDER_STATUS.PLACED
                 select new { COD.ID }).Count();
                //--------------------------------7--------------------------------------//
                ViewBag.TotalFranchise =
                (from F in db.Franchises
                 select new { F.ID }).Count();
                //--------------------------------8--------------------------------------//
                ViewBag.PendingFranchise =
                (from F in db.Franchises
                 where F.IsActive == false
                 select new { F.ID }).Count();
                //--------------------------------9--------------------------------------//
                ViewBag.TotalShop =
                (from S in db.Shops
                 select new { S.ID }).Count();
                //--------------------------------10--------------------------------------//
                ViewBag.PendingShop =
                (from S in db.Shops
                 where S.IsLive == false
                 select new { S.ID }).Count();
                //--------------------------------11--------------------------------------//
                ViewBag.TotalEmployee =
                (from E in db.Employees
                 select new { E.ID }).Count();
                //--------------------------------12--------------------------------------//
                ViewBag.PendingEmployee =
                (from E in db.Employees
                 where E.IsActive == false
                 select new { E.ID }).Count();
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

        
        //[HttpPost]
        [SessionExpire]
        public ActionResult Logout()
        {
            Session.Abandon();
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            //Response.Cache.SetNoStore(); 
            ////Session["ID"] = null;

            //HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            //HttpContext.Response.Cache.SetValidUntilExpires(false);
            //HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            //HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //HttpContext.Response.Cache.SetNoStore();
            return RedirectToAction("Login", "Login");
        }

        [SessionExpire]
        public ActionResult AccessDenied(string returnUrl)
        {
            ViewBag.returnurl = returnUrl.Replace("/", "").ToString();
            //Response.StatusCode = 403;
            //return View();
            return View();
        }

        //Yashaswi For PartnerRequestModule 9/5/2018
        [HttpGet]
        [SessionExpire]
        public ActionResult MailReceiver()
        {
            List<MailReceiver> list = db.MailReceivers.OrderBy(p => p.ID).ToList();
            return View(list);
        }

        [HttpGet]
        [SessionExpire]
        public ActionResult AddMailReceiver()
        {
            return View();
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult AddMailReceiver(MailReceiver obj)
        {
            MailReceiver objMail = new MailReceiver();
            objMail.IsActive = true;
            objMail.CreatedDate = DateTime.Now;
            objMail.CreatedBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
            objMail.DeviceID = "X";
            objMail.DeviceType = "X";
            objMail.EmailID = obj.EmailID;
            objMail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            db.MailReceivers.Add(objMail);
            db.SaveChanges();
            return RedirectToAction("MailReceiver");
        }
        [HttpGet]
        [SessionExpire]
        public ActionResult DisableMailReceiver(long id)
        {
            MailReceiver objMail = db.MailReceivers.FirstOrDefault(p => p.ID == id);
            return View(objMail);
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult DisableMailReceiver(MailReceiver obj)
        {
            MailReceiver objMail = db.MailReceivers.FirstOrDefault(p => p.ID == obj.ID);
            objMail.IsActive = obj.IsActive;
            objMail.ModifiedDate = DateTime.Now;
            objMail.ModifiedBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
            objMail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            db.SaveChanges();
            return RedirectToAction("MailReceiver");
        }
    }
}