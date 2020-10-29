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
    public class ApprovedDeliveryPartnerListController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /ApprovedDeliveryPartnerList/
        [HttpGet]
        [SessionExpire]
        [CustomAuthorize(Roles = "ApprovedDeliveryPartnerList/CanRead")]
        public ActionResult Index()
        {
            try
            {
                DeliveryPartnerApprovalViewModelList objDpList = new DeliveryPartnerApprovalViewModelList();
                var lDpav = (from ul in db.UserLogins
                             join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                             join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                             join dp in db.DeliveryPartners on bd.ID equals dp.BusinessDetailID
                             where bd.BusinessType.Prefix == "GBDP" && ul.IsLocked == false
                             select new DeliveryPartnerApprovalViewModel
                             {
                                 UserLoginID = bd.UserLoginID,
                                 BusinessTypePrefix = bd.BusinessType.Prefix,
                                 Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName,
                                 OwnerId = dp.ID
                             }).OrderBy(x => x.Name).ToList();

                objDpList.dpList = lDpav;
                return View(objDpList);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ApprovedDeliveryPartnerList][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ApprovedDeliveryPartnerList][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ApprovedDeliveryPartnerList/CanWrite")]
        public ActionResult Disapprove(long UID, long ownerId)
        {            
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    UserLogin ul = db.UserLogins.Find(UID);
                    ul.IsLocked = true;
                    ul.ModifyDate = DateTime.UtcNow;
                    ul.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    TryUpdateModel(ul);

                    DeliveryPartner dp = db.DeliveryPartners.Find(ownerId);
                    dp.IsActive = false;
                    dp.IsLive = false;
                    dp.ModifyDate = DateTime.UtcNow;
                    dp.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    TryUpdateModel(dp);                    

                    if (ModelState.IsValid)
                    {
                        db.Entry(ul).State = EntityState.Modified;
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        //sendSMS(ul.ID);
                        //sendEmail(ul.ID);
                        return RedirectToAction("Index");
                    }
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[ApprovedDeliveryPartnerList][POST:Index]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[ApprovedDeliveryPartnerList][POST:Index]",
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                }
            }
            return RedirectToAction("Index");
        }
	}
}