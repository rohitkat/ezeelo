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
using Franchise.Models;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class BusinessDetailController : Controller
    {

        #region Genral Code
        private EzeeloDBContext db = new EzeeloDBContext();

        private long GetBusinessID()
        {
            long businessDetailID = 0;
            try
            {
                long FID = Convert.ToInt64(Session["FRANCHISE_ID"]);
                businessDetailID = db.Franchises.Where(x => x.ID == FID).Select(x => x.BusinessDetailID).FirstOrDefault();
                //businessDetailID = db.BusinessDetails.Where(x => x.UserLoginID == UID).Select(x => x.ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetBusinessID]", "Can't Get Business ID! in Method !" + Environment.NewLine + ex.Message);
            }
            return businessDetailID;
        }
        #endregion

        // GET: /BusinessDetail/
        //[SessionExpire]
        //[CustomAuthorize(Roles = "BusinessDetail/CanRead")]
        //public ActionResult Index()
        //{
        //    try
        //    {
        //    var businessdetails = db.BusinessDetails.Include(b => b.BusinessType).Include(b => b.PersonalDetail).Include(b => b.PersonalDetail1).Include(b => b.Pincode).Include(b => b.SourceOfInfo).Include(b => b.UserLogin);
        //    return View(businessdetails.ToList());
        //}
        //    catch (BusinessLogicLayer.MyException myEx)
        //    {
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
        //            + "[BusinessDetailController][GET:Index]" + myEx.EXCEPTION_PATH,
        //            BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
        //    }
        //    catch (Exception ex)
        //    {
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[BusinessDetailController][GET:Index]",
        //            BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
        //    }
        //    return View();
        //}        


        // GET: /BusinessDetail/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "BusinessDetail/CanRead")]
        public ActionResult Details()
        {
            try
            {
                long id = GetBusinessID();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BusinessDetail businessdetail = db.BusinessDetails.Find(id);
                if (businessdetail == null)
                {
                    return HttpNotFound();
                }
                Pincode lPincode = db.Pincodes.Find(businessdetail.PincodeID);
                if (lPincode != null)
                {
                    ViewBag.Pincode = lPincode.Name;
                }

                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", businessdetail.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", businessdetail.PincodeID);
                ViewBag.SourceOfInfoID = new SelectList(db.SourceOfInfoes, "ID", "Name", businessdetail.SourceOfInfoID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", businessdetail.UserLoginID);
                return View(businessdetail);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BusinessDetailController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BusinessDetailController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /BusinessDetail/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "BusinessDetail/CanRead")]
        public ActionResult Edit()
        {
            try
            {
                long id = GetBusinessID();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BusinessDetail businessdetail = db.BusinessDetails.Find(id);
                if (businessdetail == null)
                {
                    return HttpNotFound();
                }
                Pincode lPincode = db.Pincodes.Find(businessdetail.PincodeID);
                if (lPincode != null)
                {
                    ViewBag.Pincode = lPincode.Name;
                }

                ViewBag.YOE = businessdetail.YearOfEstablishment.ToString();
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", businessdetail.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", businessdetail.PincodeID);
                ViewBag.SourceOfInfoID = new SelectList(db.SourceOfInfoes, "ID", "Name", businessdetail.SourceOfInfoID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", businessdetail.UserLoginID);
                return View(businessdetail);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BusinessDetailController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BusinessDetailController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /BusinessDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "BusinessDetail/CanWrite")]
        public ActionResult Edit(string Pincode, [Bind(Include = "ID,UserLoginID,Name,BusinessTypeID,ContactPerson,Mobile,Email,Landline1,Landline2,FAX,Address,PincodeID,Website,YearOfEstablishment,SourceOfInfoID,SourcesInfoDescription,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] BusinessDetail businessdetail, string YOE)
        {
            try
            {
                long PersonalDetailID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));

                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", businessdetail.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", businessdetail.PincodeID);
                ViewBag.SourceOfInfoID = new SelectList(db.SourceOfInfoes, "ID", "Name", businessdetail.SourceOfInfoID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", businessdetail.UserLoginID);

                BusinessDetail bd = db.BusinessDetails.Find(businessdetail.ID);

                bd.UserLoginID = businessdetail.UserLoginID;
                bd.Name = businessdetail.Name;
                bd.ContactPerson = businessdetail.ContactPerson;
                bd.Mobile = businessdetail.Mobile;
                bd.Email = businessdetail.Email;
                bd.Landline1 = businessdetail.Landline1;
                bd.Landline2 = businessdetail.Landline2;
                bd.FAX = businessdetail.FAX;
                bd.Address = businessdetail.Address;

                Pincode lPincode = db.Pincodes.Single(x => x.Name == Pincode);
                if (lPincode == null)
                {
                    return View("Error");
                }
                businessdetail.PincodeID = lPincode.ID;

                //DateTime lYOE = DateTime.Now;
                //if (YOE != "")
                //{
                //    if (DateTime.TryParse(YOE, out lYOE)) { }
                //    businessdetail.YearOfEstablishment = lYOE;
                //} 
                DateTime? lYOE = CommonFunctions.GetDate(YOE);
                if (lYOE != null)
                { businessdetail.YearOfEstablishment = lYOE; }
                ViewBag.YOE = businessdetail.YearOfEstablishment.ToString();
                if (businessdetail.YearOfEstablishment > DateTime.Now)
                {
                    ModelState.AddModelError("CustomError", "Year Of Establishment can't be in future");
                }
                bd.PincodeID = businessdetail.PincodeID;
                bd.Website = businessdetail.Website;
                bd.YearOfEstablishment = businessdetail.YearOfEstablishment;
                bd.SourceOfInfoID = businessdetail.SourceOfInfoID;
                bd.SourcesInfoDescription = businessdetail.SourcesInfoDescription;
                bd.IsActive = true;
                bd.ModifyDate = DateTime.UtcNow;
                bd.ModifyBy = PersonalDetailID;
                bd.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                bd.DeviceID = "x";
                bd.DeviceType = "x";
                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                    ViewBag.Message = "Bussiness Detail Save Successfully.";
                }
                return View(businessdetail);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BusinessDetailController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BusinessDetailController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult GetAddress(string Pincode)
        {
            try
            {
                if (!(db.Pincodes.Any(p => p.Name == Pincode)))
                {
                    //var errorMsg = "Pincode Dose Not Exist, Please Contact Admin!";
                    //return View(new { success = false, Error = errorMsg });
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetAddress]", "Can't Get Address! in Method !" + Environment.NewLine + ex.Message);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }
    }
}
