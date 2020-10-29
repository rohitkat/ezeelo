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
    public class ApprovedFranchiseListController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /ApprovedFranchiseList/
        [HttpGet]
        [SessionExpire]
        [CustomAuthorize(Roles = "ApprovedFranchiseList/CanRead")]
        public ActionResult Index()
        {
            try
            {
                var lfav = (from ul in db.UserLogins
                            join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                            join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                            join f in db.Franchises on bd.ID equals f.BusinessDetailID
                            join op in db.OwnerPlans on f.ID equals op.OwnerID
                            join p in db.Plans on op.PlanID equals p.ID
                            //join opc in db.OwnerPlanCategoryCharges on op.ID equals opc.OwnerPlanID
                            where bd.BusinessType.Prefix == "GBFR" && ul.IsLocked == false && p.PlanCode.StartsWith("GBFR") && op.IsActive==true
                            select new FranchiseApprovedViewModel
                            {
                                UserLoginID = bd.UserLoginID,
                                BusinessTypePrefix = bd.BusinessType.Prefix,
                                Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName + " (" + bd.Name + ")",
                                OwnerId = f.ID,
                                PlanName = p.ShortName,
                                NoOfMerchantAllowed=p.NoOfEntitiesAllowed,
                                StartDate = op.StartDate,
                                EndDate = op.EndDate
                            }).Distinct().OrderBy(x => x.Name);

                return View(lfav);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ApprovedFranchiseListController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ApprovedFranchiseListController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ApprovedFranchiseList/CanWrite")]
        public ActionResult Disapprove(long UID)
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
                    dbContextTransaction.Commit();

                    return RedirectToAction("Index");
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ApprovedFranchiseListController][GET:Disapprove]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ApprovedFranchiseListController][GET:Disapprove]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }
	}
}