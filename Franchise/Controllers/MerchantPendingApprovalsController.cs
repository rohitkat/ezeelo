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
using Franchise.Models;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class MerchantPendingApprovalsController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();        
        //
        // GET: /MerchantPendingApprovals/
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantPendingApprovals/CanRead")]
        public ActionResult Index()
        {
            try
            {
                int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]); //GetFranchiseID();

                var lMav = (from ul in db.UserLogins
                           join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                           join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                           join s in db.Shops on bd.ID equals s.BusinessDetailID
                           where bd.BusinessType.Prefix == "GBMR" && ul.IsLocked == true && s.FranchiseID == franchiseID 
                           select new MerchantPendingApprovalViewModel
                           {
                               UserLoginID = bd.UserLoginID,
                               BusinessTypePrefix = bd.BusinessType.Prefix,
                               Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName,
                               OwnerId = s.ID
                           }).OrderBy(x => x.Name);

                return View(lMav);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantPendingApprovalsController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantPendingApprovalsController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        //private int GetFranchiseID()
        //{
        //    //Session["ID"] = 5;
        //    long UID = Convert.ToInt64(Session["ID"]);
        //    long BusinessDetailID = 0;
        //    int FranchiseID = 0;
        //    try
        //    {
        //        if (UID > 0)
        //        {
        //         long.TryParse(db.BusinessDetails.Where(x => x.UserLoginID == UID).Select(x => x.ID).First().ToString(),out BusinessDetailID);
        //         int.TryParse(db.Franchises.Where(x => x.BusinessDetailID == BusinessDetailID).Select(x => x.ID).First().ToString(), out FranchiseID);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessLogicLayer.MyException("[GetFranchiseID]", "Can't Get Franchise Details! in Method !" + Environment.NewLine + ex.Message);
        //    }
        //    return FranchiseID;
        //}

        //public ActionResult ShowFranchiseList()
        //{
        //    //Session["FranchiseID"] = 6;

        //    if (Session["FranchiseID"] == null)
        //    {
        //        var lfav = from ul in db.UserLogins
        //                   join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
        //                   join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
        //                   join f in db.Franchises on bd.ID equals f.BusinessDetailID
        //                   where bd.BusinessType.Prefix == "GBFR" && ul.IsLocked == false
        //                   select new FranchisePendingApprovalViewModel
        //                   {
        //                       UserLoginID = bd.UserLoginID,
        //                       BusinessTypePrefix = bd.BusinessType.Prefix,
        //                       Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName,
        //                       OwnerId = f.ID,
        //                   };
        //        return View(lfav);
        //    }
        //    else
        //    {
        //        return View("Index", new { franchiseID = Convert.ToInt32(Session["FranchiseID"]) });
        //    }
        //}
    }
}