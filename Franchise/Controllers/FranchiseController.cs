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
    public class FranchiseController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        //private int GetFranchiseID()
        //{
        //    int franchiseID = 0;
        //    try
        //    {
        //        long UID = Convert.ToInt64(Session["ID"]);               
        //        long businessDetailID = 0;
        //        businessDetailID = db.BusinessDetails.Where(x => x.UserLoginID == UID).Select(x => x.ID).FirstOrDefault();
        //        franchiseID = db.Franchises.Where(x => x.BusinessDetailID == businessDetailID).Select(x => x.ID).FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessLogicLayer.MyException("[GetFranchiseID]", "Can't Get Franchise ID! in Method !" + Environment.NewLine + ex.Message);
        //    }
        //    return franchiseID;
        //}

        // GET: /Franchise/
        //public ActionResult Index()
        //{
        //    var franchises = db.Franchises.Include(f => f.BusinessDetail).Include(f => f.PersonalDetail).Include(f => f.PersonalDetail1).Include(f => f.Pincode);
        //    return View(franchises.ToList());
        //}

        // GET: /Franchise/Details/5

        [SessionExpire]
        [CustomAuthorize(Roles = "Franchise/CanRead")]
        public ActionResult Details()
        {
            try
            {
                int id = Convert.ToInt32(Session["FRANCHISE_ID"]); //GetFranchiseID();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ModelLayer.Models.Franchise franchise = db.Franchises.Find(id);
                if (franchise == null)
                {
                    return HttpNotFound();
                }
                return View(franchise);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }               

        
        // GET: /Franchise/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "Franchise/CanRead")]
        public ActionResult Edit()
        {
            try
            {
                int id = Convert.ToInt32(Session["FRANCHISE_ID"]); //GetFranchiseID();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ModelLayer.Models.Franchise franchise = db.Franchises.Find(id);
                if (franchise == null)
                {
                    return HttpNotFound();
                }
                Pincode lPincode = db.Pincodes.Find(franchise.PincodeID);
                if (lPincode != null)
                {
                    ViewBag.Pincode = lPincode.Name;
                }
                //ViewBag.BusinessDetailID = new SelectList(db.BusinessDetails, "ID", "Name", franchise.BusinessDetailID);
                ViewBag.SalutationID = new SelectList(db.Salutations, "ID", "Name");
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", franchise.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", franchise.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", franchise.PincodeID);
                return View(franchise);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /Franchise/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Franchise/CanWrite")]
        public ActionResult Edit(string Pincode, [Bind(Include = "ID,BusinessDetailID,ServiceNumber,ContactPerson,Mobile,Email,Landline,FAX,Address,PincodeID,IsActive,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] ModelLayer.Models.Franchise franchise)
        {
            try
            { 
                //ViewBag.BusinessDetailID = new SelectList(db.BusinessDetails, "ID", "Name", franchise.BusinessDetailID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", franchise.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", franchise.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", franchise.PincodeID);

                Pincode lPincode = db.Pincodes.Single(x => x.Name == Pincode);
                if (lPincode == null)
                {
                    return View("Error");
                }
                ModelLayer.Models.Franchise lfranchise = db.Franchises.Find(franchise.ID);
                franchise.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                franchise.ModifyDate = DateTime.UtcNow;
                franchise.CreateBy = lfranchise.CreateBy;
                franchise.CreateDate = lfranchise.CreateDate;
                franchise.PincodeID = lPincode.ID;
                franchise.BusinessDetailID = lfranchise.BusinessDetailID;
                franchise.IsActive = true;
                if (ModelState.IsValid)
                {
                    db.Entry(lfranchise).CurrentValues.SetValues(franchise);
                    db.SaveChanges();
                    ViewBag.Message ="Franchise Detail Save Successfully.";
                    //return RedirectToAction("Details");
                }
               
                return View(franchise);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseController][POST:Edit]",
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
    }
}
