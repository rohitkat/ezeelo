using BusinessLogicLayer;
using Franchise.Models;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class FranchiseSettingController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /FranchiseSetting/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseSetting/CanRead")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /FranchiseSetting/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseSetting/CanWrite")]
        public ActionResult Create([Bind(Include = "ID,Password,NewPassword,ConfirmNewPassword")] FranchiseSettingViewModel franchiseSetting)
        {
            try
            {
                // TODO: Add insert logic here
                long UID = Convert.ToInt64(Session["ID"]);
                UserLogin ul = db.UserLogins.Find(UID);
                if (ul != null)
                {
                    if (ul.Password == franchiseSetting.Password)
                    {
                        if (franchiseSetting.NewPassword == franchiseSetting.ConfirmNewPassword)
                        {
                            if (ul.Password != franchiseSetting.NewPassword)
                            {
                                ul.Password = franchiseSetting.NewPassword;
                                db.SaveChanges();
                                ViewBag.Message = "Password changed successfully.. Please login again for confirmation !! ";
                                return View("SuccessMessage");
                               // return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ModelState.AddModelError("CustomError", "You can not use old password as new password !!");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("CustomError", "New password and confirm password is mismatched !!");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("CustomError", "Invalid old password");
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }                
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseSetting][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseSetting][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }


    }
}
