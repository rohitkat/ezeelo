using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using Merchant.Models;
using BusinessLogicLayer;

namespace Merchant.Controllers
{
    public class BusinessDetailController : Controller
    {

        #region Genral Code
        private EzeeloDBContext db = new EzeeloDBContext();

        private long GetPersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
            long PersonalDetailID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PlacedController][GetShopID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }

        private long GetBusinessID()
        {
            long businessDetailID = 0;
            try
            {
                long UID = Convert.ToInt64(Session["ID"]);
                businessDetailID = db.BusinessDetails.Where(x => x.UserLoginID == UID).Select(x => x.ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetBusinessID]", "Can't Get Business ID! in Method !" + Environment.NewLine + ex.Message);
            }
            return businessDetailID;
        }

        #endregion

        // GET: /BusinessDetail/
        public ActionResult Index()
        {
            var businessdetails = db.BusinessDetails.Include(b => b.BusinessType).Include(b => b.PersonalDetail).Include(b => b.PersonalDetail1).Include(b => b.Pincode).Include(b => b.SourceOfInfo).Include(b => b.UserLogin);
            return View(businessdetails.ToList());
        }

        // GET: /BusinessDetail/Details/5
        public ActionResult Details(long? id)
        {
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

        // GET: /BusinessDetail/Create
        public ActionResult Create()
        {
            ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name");
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name");
            ViewBag.SourceOfInfoID = new SelectList(db.SourceOfInfoes, "ID", "Name");
            ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile");
            return View();
        }

        // POST: /BusinessDetail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserLoginID,Name,BusinessTypeID,ContactPerson,Mobile,Email,Landline1,Landline2,FAX,Address,PincodeID,Website,YearOfEstablishment,SourceOfInfoID,SourcesInfoDescription,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] BusinessDetail businessdetail)
        {
            if (ModelState.IsValid)
            {
                db.BusinessDetails.Add(businessdetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", businessdetail.BusinessTypeID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.ModifyBy);
            ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", businessdetail.PincodeID);
            ViewBag.SourceOfInfoID = new SelectList(db.SourceOfInfoes, "ID", "Name", businessdetail.SourceOfInfoID);
            ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", businessdetail.UserLoginID);
            return View(businessdetail);
        }

        // GET: /BusinessDetail/Edit/5
        [SessionExpire]
        public ActionResult Edit()
        {
            //long id = GetPersonalDetailID();
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
            ViewBag.PincodeID = new SelectList(db.Pincodes.Where(x => x.IsActive == true), "ID", "Name", businessdetail.PincodeID);
            ViewBag.SourceOfInfoID = new SelectList(db.SourceOfInfoes, "ID", "Name", businessdetail.SourceOfInfoID);
            ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", businessdetail.UserLoginID);
            ViewBag.YOE = Convert.ToDateTime(businessdetail.YearOfEstablishment).ToString("dd/MM/yyyy");
            return View(businessdetail);
        }

        // POST: /BusinessDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Pincode, [Bind(Include = "ID,UserLoginID,Name,BusinessTypeID,ContactPerson,Mobile,Email,Landline1,Landline2,FAX,Address,PincodeID,Website,YearOfEstablishment,SourceOfInfoID,SourcesInfoDescription,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] BusinessDetail businessdetail, string YOE)
        {
            try
            {


                long PersonalDetailID = GetPersonalDetailID();

                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", businessdetail.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", businessdetail.PincodeID);
                ViewBag.SourceOfInfoID = new SelectList(db.SourceOfInfoes, "ID", "Name", businessdetail.SourceOfInfoID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", businessdetail.UserLoginID);

                BusinessDetail bd = db.BusinessDetails.Find(businessdetail.ID);

                DateTime lDisplayProductFromDate = DateTime.Now;
                if (YOE != "")
                {
                    //if (DateTime.TryParse(DOB1, out lDisplayProductFromDate)) { }
                    //personaldetail.DOB = lDisplayProductFromDate;
                    lDisplayProductFromDate = CommonFunctions.GetProperDateTime(YOE);
                    businessdetail.YearOfEstablishment = lDisplayProductFromDate;
                }
                ViewBag.YOE = Convert.ToDateTime(businessdetail.YearOfEstablishment).ToString("dd/MM/yyyy");

                bd.UserLoginID = businessdetail.UserLoginID;
                bd.Name = businessdetail.Name;
                bd.BusinessTypeID = businessdetail.BusinessTypeID;
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

                bd.PincodeID = businessdetail.PincodeID;
                bd.Website = businessdetail.Website;
                bd.YearOfEstablishment = businessdetail.YearOfEstablishment;
                bd.SourceOfInfoID = businessdetail.SourceOfInfoID;
                bd.SourcesInfoDescription = businessdetail.SourcesInfoDescription;
                bd.IsActive = true;
                bd.CreateBy = businessdetail.CreateBy;
                bd.CreateDate = businessdetail.CreateDate;
                bd.ModifyDate = DateTime.UtcNow;
                bd.ModifyBy = PersonalDetailID;
                bd.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                bd.DeviceID = "x";
                bd.DeviceType = "x";
                db.SaveChanges();
                ViewBag.Message = "Record Saved Successfully.";
                return View(businessdetail);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", businessdetail.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", businessdetail.PincodeID);
                ViewBag.SourceOfInfoID = new SelectList(db.SourceOfInfoes, "ID", "Name", businessdetail.SourceOfInfoID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", businessdetail.UserLoginID);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes, "ID", "Name", businessdetail.BusinessTypeID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", businessdetail.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", businessdetail.PincodeID);
                ViewBag.SourceOfInfoID = new SelectList(db.SourceOfInfoes, "ID", "Name", businessdetail.SourceOfInfoID);
                ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", businessdetail.UserLoginID);
            }
            return View(businessdetail);
        }

        // GET: /BusinessDetail/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessDetail businessdetail = db.BusinessDetails.Find(id);
            if (businessdetail == null)
            {
                return HttpNotFound();
            }
            return View(businessdetail);
        }

        // POST: /BusinessDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            BusinessDetail businessdetail = db.BusinessDetails.Find(id);
            db.BusinessDetails.Remove(businessdetail);
            db.SaveChanges();
            return RedirectToAction("Index");
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

            if (!(db.Pincodes.Any(p => p.Name == Pincode)))
            {
                //var errorMsg = "Pincode Dose Not Exist, Please Contact Admin!";
                //return View(new { success = false, Error = errorMsg });
                return Json("1", JsonRequestBehavior.AllowGet);
            }


            return Json("0", JsonRequestBehavior.AllowGet);
        }
    }
}
