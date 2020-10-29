using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using BusinessLogicLayer;
using Gandhibagh.Models;

namespace Gandhibagh.Controllers
{
    public class SubscriptionPlanController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        public class WebmethodParams
        {
            public long SubPlanID { get; set; }
            public int Alreadyexist { get; set; }
            //public string SailedByEmployeeID { get; set; }
            public string ReferredByLoginID { get; set; }
        }
        // GET: /SubscriptionPlan/
        public ActionResult Index()
        {
            //Set Cookie for Url saving & Use in Continue shopping
            URLCookie.SetCookies();

            var Category = db.Categories.Where(x => x.Level == 1).ToList();
            ViewBag.CategoryList = Category;

            List<SubscriptionPlan> lSubscriptionPlans = db.SubscriptionPlans.Where(x => x.IsActive == true).ToList();
            return View(lSubscriptionPlans);
        }
        //----------Add two parameter SailedByEmployeeID, ReferredByLoginID for MLM Process -----------------//
        //public ActionResult PaymentProcess(long SubPlanID, int Alreadyexist, string SailedByEmployeeID, string ReferredByLoginID)
        public ActionResult PaymentProcess(WebmethodParams myParam)
        {


            long SubPlanID;
            int Alreadyexist;
            //string SailedByEmployeeID; 
            string ReferredByLoginID;
            long rID= 0;

            SubPlanID = myParam.SubPlanID;
            Alreadyexist = myParam.Alreadyexist;
            //SailedByEmployeeID = myParam.SailedByEmployeeID;
            ReferredByLoginID = myParam.ReferredByLoginID;


            if (ReferredByLoginID != null)
            {
                var isMember = (from ul in db.UserLogins where ul.Email == ReferredByLoginID select new { ul.ID }).FirstOrDefault();
                rID = isMember.ID;
            }
            return Json(new { ok = true, newurl = Url.Action("CustomerPaymentProcess", "PaymentProcess", new { Subscription = SubPlanID, Alreadyexist = Alreadyexist, ReferredByLoginID = rID }) });
            //return Json(new { ok = true, newurl = Url.Action("CustomerPaymentProcess", "PaymentProcess", new { Subscription = SubPlanID, Alreadyexist = Alreadyexist, SailedByEmployeeID = SailedByEmployeeID, ReferredByLoginID = ReferredByLoginID }) });

        }

        [HttpPost]
        public ActionResult CheckPlan(long SubPlanID)
        {
            string Message = "";
            long UserLoginID = 0;
            long SubscriptionPlanID = 0;
            try
            {
                if (Session["UID"] != null)
                {
                    UserLoginID = Convert.ToInt64(Session["UID"]);

                    if (db.SubscriptionPlanPurchasedBies.Any(x =>x.UserLoginID==UserLoginID) == false) //Apply ReferrebByCode when First time Buy
                    {
                        return Json(new { ok = false, newurl = Url.Action("SubscriptionReferredBy", "SubscriptionPlan") });
                    }

                    else
                    {
                        SubscriptionPlanID = db.SubscriptionPlanPurchasedBies.Where(x => x.SubscriptionPlanID == SubPlanID && x.UserLoginID == UserLoginID).Select(x => x.ID).FirstOrDefault();
                        if (SubscriptionPlanID > 0)
                        {
                            Message = "1";
                            return Json(Message);
                            //return RedirectToAction("PaymentProcess", new { SubPlanID = SubPlanID, Alreadyexist = 1 });
                        }
                        else
                        {
                            var SubcriptionPlanID = db.SubscriptionPlanPurchasedBies.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).ToList();
                            if (SubcriptionPlanID.Count() > 0)
                            {
                                Message = "2";
                                return Json(Message);
                            }
                            else
                            {
                                Message = "4";
                                return Json(Message);
                            }
                        }
                    }


                }

                else
                {
                    // Message = "3";
                    return Json(new { ok = true, newurl = Url.Action("SubscriptionLogin", "SubscriptionPlan", new { Subscription = SubPlanID }) });
                    // return View("_Login", "_Layout1");
                    // return RedirectToAction("LoginPartial");
                    //return Json(new { Url = Url.Action("_Login") });
                    // return Json(new { ok = true, newurl = Url.Action("_Login") });
                }
                //Message = "Plan Purchased Succesfully";
                //return Json(Message);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in check plan method!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SubscriptionPlan][POST:CheckPlan]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in check plan method!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SubscriptionPlan][POST:CheckPlan]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            //long Message = ShopStockID;
            return Json(Message);

        }


        public JsonResult MakeOtherPlanDeactive()
        {
            string Message = "";
            long UserLoginID = 0;


            try
            {
                if (Session["UID"] != null)
                {
                    UserLoginID = Convert.ToInt64(Session["UID"]);

                    var SubcriptionPlanID = db.SubscriptionPlanPurchasedBies.Where(x => x.UserLoginID == UserLoginID).ToList();
                    if (SubcriptionPlanID.Count() > 0)
                    {
                        foreach (var i in SubcriptionPlanID)
                        {
                            SubscriptionPlanPurchasedBy sub = db.SubscriptionPlanPurchasedBies.Find(i.ID);
                            sub.IsActive = false;
                            db.SaveChanges();
                        }
                    }
                }


                //Message = "Plan Purchased Succesfully";
                return Json(Message);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in MakeOtherPlanDeactive method!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SubscriptionPlan][POST:MakeOtherPlanDeactive]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in MakeOtherPlanDeactive method!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SubscriptionPlan][POST:MakeOtherPlanDeactive]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            //long Message = ShopStockID;
            return Json(Message);

        }
        public ActionResult SaveCustSubscriptionPlan(long SubPlanID, int AlreadyExist, long ReferredByLoginID)
        {
            try
            {
                SavePlan(SubPlanID, AlreadyExist, ReferredByLoginID);
                //return Json(Message);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong while Saving Subscription Plan!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SubscriptionPlan][POST:SaveCustSubscriptionPlan]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Saving Saving Subscription Plan!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SubscriptionPlan][POST:SaveCustSubscriptionPlan]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            //long Message = ShopStockID;
            return RedirectToAction("Index");

        }


        private void SavePlan(long SubPlanID, int AlreadyExist, long ReferredByLoginID)
        {
            string Message = "";
            int NoOfDays = 0;
            long UserLoginID = 0;
            DateTime EndDate = new DateTime();
            if (Session["UID"] != null)
            {
                UserLoginID = Convert.ToInt64(Session["UID"]);
            }

            if (SubPlanID > 0)
            {
                NoOfDays = db.SubscriptionPlans.Where(x => x.ID == SubPlanID).Select(x => x.NoOfDays).FirstOrDefault();
                EndDate = db.SubscriptionPlanPurchasedBies.OrderByDescending(x => x.CreateDate).Where(x => x.UserLoginID == UserLoginID).Select(x => x.EndDate).FirstOrDefault();

            }

            SubscriptionPlanPurchasedBy subscriptionplanpurchasedby = new SubscriptionPlanPurchasedBy();
            subscriptionplanpurchasedby.UserLoginID = UserLoginID;
            subscriptionplanpurchasedby.SailedByEmployeeID = null;
            subscriptionplanpurchasedby.SubscriptionPlanID = SubPlanID;

            //----------------Code Changes by mohit for Referred By Login ID and Referred Point Used----------------//
            if (db.SubscriptionPlanPurchasedBies.Find(UserLoginID) != null)
            {
                subscriptionplanpurchasedby.ReferredByLoginID = null;
                subscriptionplanpurchasedby.ReferredPointUsed = false;
            }
            else
            {
                subscriptionplanpurchasedby.ReferredByLoginID = ReferredByLoginID;
                subscriptionplanpurchasedby.ReferredPointUsed = false;

            }
            //----------------End of Code Changes by mohit for Referred By Login ID and Referred Point Used----------------//


            if (AlreadyExist == 1)
            {
                subscriptionplanpurchasedby.IsActive = false;
                subscriptionplanpurchasedby.StartDate = EndDate.AddDays(1);
                subscriptionplanpurchasedby.EndDate = EndDate.AddDays(NoOfDays);
            }
            else
            {
                subscriptionplanpurchasedby.IsActive = true;
                subscriptionplanpurchasedby.StartDate = DateTime.UtcNow;
                subscriptionplanpurchasedby.EndDate = DateTime.UtcNow.AddDays(NoOfDays);
            }
            subscriptionplanpurchasedby.CreateDate = DateTime.UtcNow;
            subscriptionplanpurchasedby.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["UID"]));
            subscriptionplanpurchasedby.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            subscriptionplanpurchasedby.DeviceID = "x";
            subscriptionplanpurchasedby.DeviceType = "x";

            db.SubscriptionPlanPurchasedBies.Add(subscriptionplanpurchasedby);
            db.SaveChanges();
            TempData["Message"] = "Plan Purchased Succesfully";
        }

        // GET: /SubscriptionPlan/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionPlan subscriptionplan = db.SubscriptionPlans.Find(id);
            if (subscriptionplan == null)
            {
                return HttpNotFound();
            }
            return View(subscriptionplan);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult SubscriptionLogin()
        {
            try
            {
                if (Request.QueryString["Subscription"] != null)
                {

                    TempData["Subscription"] = Request.QueryString["Subscription"];
                    TempData.Keep();

                }
            }
            catch (Exception)
            {

                throw;
            }
            return View();

        }

        public ActionResult SubscriptionReferredBy()
        {
            //try
            //{
            //    if (Request.QueryString["Subscription"] != null)
            //    {

            //        TempData["Subscription"] = Request.QueryString["Subscription"];
            //        TempData.Keep();

            //    }
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
            return View("SubscriptionReferred");

        }

        public JsonResult IsEmailAvailable(string Email)
        {
            //var isMember = (from ul in db.UserLogins
            //                   join sppb in db.SubscriptionPlanPurchasedBies on ul.ID equals sppb.UserLoginID
            //                   where ul.Email == Email select ul).Count();
            var isMember = (from ul in db.UserLogins where ul.Email == Email select new { ul.ID }).FirstOrDefault();
            if (db.UserLogins.Any(x => x.Email == Email))
            {
                if (Convert.ToInt64(Session["UID"]) != isMember.ID)
                {
                    if (db.SubscriptionPlanPurchasedBies.Any(x => x.UserLoginID == isMember.ID))
                    {
                        //Provided Email is Valid subscribed Email Id
                        return Json("1", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //Provided Email is Not Valid Subscribed Email ID
                        return Json("2", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("3", JsonRequestBehavior.AllowGet);
                }
            }
            //Provide Email Is Not Registerd To eZeelo
            return Json("0", JsonRequestBehavior.AllowGet); ;
        }
        //public ActionResult AddReferredBy()
        //{
        //    string Message = "";
        //    long UserLoginID = 0;
        //    long SubscriptionPlanID = 0;
        //    try
        //    {
        //        if (Session["UID"] != null)
        //        {
        //            UserLoginID = Convert.ToInt64(Session["UID"]);
        //            //SubscriptionPlanID = db.SubscriptionPlanPurchasedBies.Where(x => x.SubscriptionPlanID == SubPlanID && x.UserLoginID == UserLoginID).Select(x => x.ID).FirstOrDefault();
        //            if (db.SubscriptionPlanPurchasedBies.Find(UserLoginID) != null) //Apply ReferrebByCode when First time Buy
        //            {
        //                Message = "1";
        //                return Json(Message);

        //            }
        //            else
        //            {
        //                Message = "2";
        //                return Json(Message);
        //            }

        //            //if (SubscriptionPlanID > 0)
        //            //{
        //            //    Message = "1";
        //            //    return Json(Message);
        //            //    //return RedirectToAction("PaymentProcess", new { SubPlanID = SubPlanID, Alreadyexist = 1 });
        //            //}
        //            //else
        //            //{
        //            //    var SubcriptionPlanID = db.SubscriptionPlanPurchasedBies.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).ToList();
        //            //    if (SubcriptionPlanID.Count() > 0)
        //            //    {
        //            //        Message = "2";
        //            //        return Json(Message);
        //            //    }
        //            //    else
        //            //    {
        //            //        Message = "4";
        //            //        return Json(Message);
        //            //    }
        //            //}


        //        }

        //        else
        //        {
        //            // Message = "3";
        //            return Json(new { ok = true, newurl = Url.Action("SubscriptionLogin", "SubscriptionPlan", new { Subscription = SubPlanID }) });
        //            // return View("_Login", "_Layout1");
        //            // return RedirectToAction("LoginPartial");
        //            //return Json(new { Url = Url.Action("_Login") });
        //            // return Json(new { ok = true, newurl = Url.Action("_Login") });
        //        }
        //        //Message = "Plan Purchased Succesfully";
        //        //return Json(Message);

        //    }
        //    catch (BusinessLogicLayer.MyException myEx)
        //    {
        //        ModelState.AddModelError("Error", "There's Something wrong in check plan method!!");

        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
        //            + "[SubscriptionPlan][POST:CheckPlan]" + myEx.EXCEPTION_PATH,
        //            BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
        //    }
        //    catch (Exception ex)
        //    {

        //        ModelState.AddModelError("Error", "There's Something wrong in check plan method!!");

        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[SubscriptionPlan][POST:CheckPlan]",
        //            BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
        //    }
        //    //long Message = ShopStockID;
        //    return Json(Message);

        //}

    }
}
