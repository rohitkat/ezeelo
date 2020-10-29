using Administrator.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Administrator.Controllers
{
    public class ApprovedMerchantListController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /ApprovedMerchantList/
        [SessionExpire]
        [CustomAuthorize(Roles = "ApprovedMerchantList/CanRead")]
        public ActionResult Index(int franchiseID)
        {
            try
            {
                var lMav = (from ul in db.UserLogins
                           join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                           join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                           join s in db.Shops on bd.ID equals s.BusinessDetailID
                           join op in db.OwnerPlans on s.ID equals op.OwnerID
                           join p in db.Plans on op.PlanID equals p.ID
                           where bd.BusinessType.Prefix == "GBMR" && ul.IsLocked == false && s.FranchiseID == franchiseID && p.PlanCode.StartsWith("GBMR") && op.IsActive==true
                           select new MerchantApprovedViewModel
                           {
                               UserLoginID = bd.UserLoginID,
                               BusinessTypePrefix = bd.BusinessType.Prefix,
                               Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName +" ("+s.Name+")",
                               OwnerId = s.ID,
                               PlanName=p.ShortName,
                               NoOfProducttAllowed=p.NoOfEntitiesAllowed,
                               StartDate=op.StartDate,
                               EndDate=op.EndDate
                           }).OrderBy(x => x.Name);

                return View(lMav);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ApprovedMerchantListController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ApprovedMerchantListController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "ApprovedMerchantList/CanRead")]
        public ActionResult ShowFranchiseList()
        {
            try
            {
                var result = (from ul in db.UserLogins
                           join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                           join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                           join f in db.Franchises on bd.ID equals f.BusinessDetailID
                           where bd.BusinessType.Prefix == "GBFR" && ul.IsLocked == false && f.ID != 1
                           select new FranchisePendingApprovalViewModel
                           {
                               UserLoginID = bd.UserLoginID,
                               BusinessTypePrefix = bd.BusinessType.Prefix,
                               Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName + " (" + bd.Name + ")",
                               OwnerId = f.ID,
                           }).ToList().OrderBy(x => x.Name);

                return View(result);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ApprovedMerchantListController][GET:ShowFranchiseList]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ApprovedMerchantListController][GET:ShowFranchiseList]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ApprovedMerchantList/CanWrite")]
        public ActionResult Disapprove(long UID,long OwnerID)
        {
            try
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {

                    UserLogin ul = db.UserLogins.Find(UID);
                    if (ul == null)
                    {
                        return View("Error");
                    }

                    //WriteToLogTable(ul, ModelLayer.Models.Enum.COMMAND.UPDATE);

                    ul.IsLocked = true;
                    ul.ModifyDate = DateTime.UtcNow;
                    ul.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    TryUpdateModel(ul);
                    db.Entry(ul).State = EntityState.Modified;
                    db.SaveChanges();

                    Shop sp = db.Shops.Find(OwnerID);
                    if (sp == null)
                    {
                        return View("Error");
                    }
                    sp.IsLive = false;
                    db.SaveChanges();

                    dbContextTransaction.Commit();

                    return RedirectToAction("Index", new { franchiseID = GetFranchiseID(UID) });
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ApprovedMerchantListController][GET:Disapprove]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ApprovedMerchantListController][GET:Disapprove]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        private int GetFranchiseID(long UID)
        {
            long BusinessDetailID = 0;
            long ShopID = 0;
            int FranchiseID = 0;
            try
            {
                if (UID > 0)
                {
                    BusinessDetailID = Convert.ToInt32(db.BusinessDetails.Where(x => x.UserLoginID == UID).Select(x => x.ID).First());
                    ShopID = Convert.ToInt32(db.Shops.Where(x => x.BusinessDetailID == BusinessDetailID).Select(x => x.ID).First());
                    FranchiseID = Convert.ToInt32(db.Shops.Where(x => x.ID == ShopID).Select(x => x.FranchiseID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetFranchiseID]", "Can't Get Franchise Details! in Method !" + Environment.NewLine + ex.Message);
            }
            return FranchiseID;
        }
	}
}