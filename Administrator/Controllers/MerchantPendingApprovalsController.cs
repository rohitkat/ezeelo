//-----------------------------------------------------------------------
// <copyright file="MerchantPendingApprovalsController.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Snehal Shende</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using Administrator.Models;
using System.Text;
using BusinessLogicLayer;

namespace Administrator.Controllers
{    
    public class MerchantPendingApprovalsController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

         //
        // GET: /MerchantPendingApprovals/
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantPendingApprovals/CanRead")]
        public ActionResult Index(int franchiseID)
        {
            try
            {
                ViewBag.FranchiseID = franchiseID;
                var lMav = (from ul in db.UserLogins
                           join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                           join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                           join s in db.Shops on bd.ID equals s.BusinessDetailID
                           where bd.BusinessType.Prefix == "GBMR" && ul.IsLocked == true && s.FranchiseID == franchiseID
                           select new MerchantPendingApprovalViewModel
                           {
                               UserLoginID = bd.UserLoginID,
                               BusinessTypePrefix = bd.BusinessType.Prefix,
                               Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName+" (" + s.Name + ")",
                               OwnerId = s.ID
                           }).OrderBy(x=>x.Name);

                return View(lMav);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantPendingApprovalsController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantPendingApprovalsController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantPendingApprovals/CanRead")]
        public ActionResult ShowFranchiseList()
        {
            try
            {
                //if (Session["FranchiseID"] == null)
                //{
                    //var lfav = from ul in db.UserLogins
                    //           join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                    //           join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                    //           join f in db.Franchises on bd.ID equals f.BusinessDetailID
                    //           where bd.BusinessType.Prefix == "GBFR" && ul.IsLocked == false && f.ID!=1
                    //           select new FranchisePendingApprovalViewModel
                    //           {
                    //               UserLoginID = bd.UserLoginID,
                    //               BusinessTypePrefix = bd.BusinessType.Prefix,
                    //               Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName + " (" + bd.Name + ")",
                    //               OwnerId = f.ID,
                    //           };

                var result = db.UserLogins.Where(u => u.IsLocked == false).Join(db.BusinessDetails.Where(b => db.BusinessTypes.Where(bt => bt.Prefix == "GBFR").Select(bt => bt.ID).Contains(b.BusinessTypeID)), u => u.ID, b => b.UserLoginID, (u, b) => new { UserLoginId = u.ID, BdName = (b.Name == null) ? "" : b.Name, BdId = b.ID })
                     .Join(db.PersonalDetails, u => u.UserLoginId, p => p.UserLoginID, (u, p) => new
                     {
                         UserLoginId = u.UserLoginId,
                         FirstName = (p.FirstName == null) ? "" : p.FirstName,
                         LastName = (p.LastName == null) ? "" : p.LastName,
                         BDName = u.BdName,
                         BdId = u.BdId
                     })
                     .Join(db.Franchises, u => u.BdId, f => f.BusinessDetailID, (u, f) => new FranchisePendingApprovalViewModel
                     {
                         UserLoginID = u.UserLoginId,
                         BusinessTypePrefix = "GBFR",
                         Name = u.FirstName + " " + u.LastName + " (" + u.BDName + ")",
                         OwnerId = f.ID,
                     }).ToList();
                return View(result);
                //}
                //else
                //{
                //    return View("Index", new { franchiseID = Convert.ToInt32(Session["FranchiseID"]) });
                //}
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantPendingApprovalsController][GET:ShowFranchiseList]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantPendingApprovalsController][GET:ShowFranchiseList]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }
    }
}