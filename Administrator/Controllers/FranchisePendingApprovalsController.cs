//-----------------------------------------------------------------------
// <copyright file="FranchisePendingApprovalsController.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
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
using BusinessLogicLayer;
using System.IO;
using System.Text;
using Administrator.Models;

namespace Administrator.Controllers
{
    public class FranchisePendingApprovalsController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        //
        // GET: /FranchisePendingApprovals/
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchisePendingApprovals/CanRead")]
        public ActionResult Index()
        {
            try
            {
                var lfav = (from ul in db.UserLogins
                            join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                            join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                            join f in db.Franchises on bd.ID equals f.BusinessDetailID
                            join fl in db.FranchiseLocations on f.ID equals fl.FranchiseID
                            where bd.BusinessType.Prefix == "GBFR" && ul.IsLocked == true && f.ID != 1
                            select new FranchisePendingApprovalViewModel
                            {
                                UserLoginID = bd.UserLoginID,
                                BusinessTypePrefix = bd.BusinessType.Prefix,
                                Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName + " (" + bd.Name + ")",
                                OwnerId = f.ID
                            }).Distinct();

                return View(lfav);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchisePendingApprovalsController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchisePendingApprovalsController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }
    }
}