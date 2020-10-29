using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Text;
using BusinessLogicLayer;
using Administrator.Models;
using System.Web.Configuration;

namespace Administrator.Controllers
{
    public class FranchiseTaxDetailController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
          Environment.NewLine
          + "ErrorLog Controller : FranchiseTaxDetailController" + Environment.NewLine);

        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
        // GET: /FranchiseTaxDetail/
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseTaxDetail/CanRead")]
        public ActionResult Index()
        {
            try
            {
                var franchisetaxdetails = db.FranchiseTaxDetails.Include(f => f.City).Include(f => f.Franchise).Include(f => f.TaxationMaster).Where(x => x.IsActive == true);
                return View(franchisetaxdetails.ToList());
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Index[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Index view!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        // GET: /FranchiseTaxDetail/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseTaxDetail/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                FranchiseTaxDetail franchisetaxdetail = db.FranchiseTaxDetails.Find(id);
                if (franchisetaxdetail == null)
                {
                    return HttpNotFound();
                }
                return View(franchisetaxdetail);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Details[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Detail!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Detail!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        // GET: /FranchiseTaxDetail/Create
        public ActionResult Create()
        {
            try
            {
              //  ViewBag.CityID = new SelectList(db.Cities, "ID", "Name");
                ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ContactPerson");
                ViewBag.TaxationID = new SelectList(db.TaxationMasters, "ID", "Name");
                return View();
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Create[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Create View " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        // POST: /FranchiseTaxDetail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseTaxDetail/CanWrite")]
        public ActionResult Create([Bind(Include = "ID,FranchiseID,TaxationID,InPercentage,InRupees,IsDirect,IsCustomerSide,IsOnTaxSum,IsMinusTaxs,IsIncludeSaleRate,IsPercentage,LowerLimit,UpperLimit,IsActive")] FranchiseTaxDetail franchisetaxdetail)
        {
            try
            {
                ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ContactPerson", franchisetaxdetail.FranchiseID);
                ViewBag.TaxationID = new SelectList(db.TaxationMasters, "ID", "Name", franchisetaxdetail.TaxationID);

                if (franchisetaxdetail.IsDirect == false && franchisetaxdetail.IsOnTaxSum == false && franchisetaxdetail.IsMinusTaxs == false && franchisetaxdetail.IsIncludeSaleRate == false)
                {
                    ViewBag.Message = "Plese Select one of the setting Direct/Isminus/IssumOnTax/Include SaleRate";
                    return View(franchisetaxdetail);
                }
                else if (franchisetaxdetail.FranchiseID < 1)
                {
                    ViewBag.Message = "Select Franchise";
                }
                else if (franchisetaxdetail.TaxationID < 1)
                {
                    ViewBag.Message = "Select Taax";
                }
                else if (db.FranchiseTaxDetails.Where(x => x.FranchiseID == franchisetaxdetail.FranchiseID && x.TaxationID == franchisetaxdetail.TaxationID && x.IsActive == true).Count() > 0)
                {
                    ViewBag.Message = "Selected tax Already Present in Franchise";
                }
                else
                {
                    if (franchisetaxdetail.IsDirect == true)
                    {
                        franchisetaxdetail.IsMinusTaxs = false;
                        franchisetaxdetail.IsOnTaxSum = false;
                        franchisetaxdetail.IsIncludeSaleRate = false;
                    }
                    else if (franchisetaxdetail.IsMinusTaxs)
                    {
                        franchisetaxdetail.IsDirect = false;
                        franchisetaxdetail.IsOnTaxSum = false;
                        franchisetaxdetail.IsIncludeSaleRate = false;
                    }
                    else if (franchisetaxdetail.IsIncludeSaleRate == true)
                    {
                        franchisetaxdetail.IsDirect = false;
                        franchisetaxdetail.IsOnTaxSum = true;
                        franchisetaxdetail.IsMinusTaxs = false;
                    }


                    Int64? PincodeID = db.Franchises.Where(x => x.ID == franchisetaxdetail.FranchiseID).FirstOrDefault().PincodeID;
                    Int64 CityID = db.Pincodes.Where(x => x.ID == PincodeID).FirstOrDefault().CityID;
                    franchisetaxdetail.IsPercentage = true;
                    franchisetaxdetail.CityID = CityID;
                    franchisetaxdetail.CreateDate = DateTime.UtcNow.AddHours(5.3);
                    franchisetaxdetail.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    franchisetaxdetail.NetworkIP = CommonFunctions.GetClientIP();
                    franchisetaxdetail.InRupees = 0;
                    franchisetaxdetail.IsActive = true;
                    if (ModelState.IsValid)
                    {
                        db.FranchiseTaxDetails.Add(franchisetaxdetail);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }



                //ViewBag.CityID = new SelectList(db.Cities, "ID", "Name", franchisetaxdetail.CityID);
                return View(franchisetaxdetail);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Create[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }

        }

        // GET: /FranchiseTaxDetail/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseTaxDetail/CanWrite")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                FranchiseTaxDetail franchisetaxdetail = db.FranchiseTaxDetails.Find(id);
                if (franchisetaxdetail == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CityID = db.Cities.Where(x => x.ID == franchisetaxdetail.CityID).FirstOrDefault().Name; //new SelectList(db.Cities, "ID", "Name", franchisetaxdetail.CityID);
                ViewBag.FranchiseID = db.Franchises.Where(x => x.ID == franchisetaxdetail.FranchiseID).FirstOrDefault().ContactPerson;//new SelectList(db.Franchises, "ID", "ServiceNumber", franchisetaxdetail.FranchiseID);
                ViewBag.TaxationID = db.TaxationMasters.Where(x => x.ID == franchisetaxdetail.TaxationID).FirstOrDefault().Name; //new SelectList(db.TaxationMasters, "ID", "Prefix", franchisetaxdetail.TaxationID);
                return View(franchisetaxdetail);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Edit[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Update!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Update!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        // POST: /FranchiseTaxDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseTaxDetail/CanWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CityID,FranchiseID,TaxationID,InPercentage,InRupees,IsDirect,IsCustomerSide,IsOnTaxSum,IsMinusTaxs,IsIncludeSaleRate,IsPercentage,LowerLimit,UpperLimit,IsActive")] FranchiseTaxDetail franchisetaxdetail)
        {
            try
            {
                FranchiseTaxDetail lfranchisetaxdetail = db.FranchiseTaxDetails.Find(franchisetaxdetail.ID);
                ViewBag.CityID = db.Cities.Where(x => x.ID == franchisetaxdetail.CityID).FirstOrDefault().Name; //new SelectList(db.Cities, "ID", "Name", franchisetaxdetail.CityID);
                ViewBag.FranchiseID = db.Franchises.Where(x => x.ID == franchisetaxdetail.FranchiseID).FirstOrDefault().ContactPerson;//new SelectList(db.Franchises, "ID", "ServiceNumber", franchisetaxdetail.FranchiseID);
                ViewBag.TaxationID = db.TaxationMasters.Where(x => x.ID == franchisetaxdetail.TaxationID).FirstOrDefault().Name; //new SelectList(db.TaxationMasters, "ID", "Prefix", franchisetaxdetail.TaxationID);

                if (franchisetaxdetail.IsDirect == false && franchisetaxdetail.IsOnTaxSum == false && franchisetaxdetail.IsMinusTaxs == false && franchisetaxdetail.IsIncludeSaleRate == false)
                {
                    ViewBag.Message = "Plese Select one of the setting Direct/Isminus/IssumOnTax/Include SaleRate";
                    return View(franchisetaxdetail);
                }
                else if (franchisetaxdetail.IsDirect == true)
                {
                    franchisetaxdetail.IsMinusTaxs = false;
                    franchisetaxdetail.IsOnTaxSum = false;
                    franchisetaxdetail.IsIncludeSaleRate = false;
                }
                else if (franchisetaxdetail.IsMinusTaxs)
                {
                    franchisetaxdetail.IsDirect = false;
                    franchisetaxdetail.IsOnTaxSum = false;
                    franchisetaxdetail.IsIncludeSaleRate = false;
                }
                else if (franchisetaxdetail.IsIncludeSaleRate == true)
                {
                    franchisetaxdetail.IsDirect = false;
                    franchisetaxdetail.IsOnTaxSum = true;
                    franchisetaxdetail.IsMinusTaxs = false;
                }
                List<object> paramValues = new List<object>();
                paramValues.Add(franchisetaxdetail.ID);
                paramValues.Add(franchisetaxdetail.CityID);
                paramValues.Add(franchisetaxdetail.FranchiseID);
                paramValues.Add(franchisetaxdetail.TaxationID);
                paramValues.Add(franchisetaxdetail.InPercentage);
                paramValues.Add(0);
                paramValues.Add(franchisetaxdetail.IsDirect);
                paramValues.Add(franchisetaxdetail.IsCustomerSide);
                paramValues.Add(franchisetaxdetail.IsOnTaxSum);
                paramValues.Add(franchisetaxdetail.IsMinusTaxs);
                paramValues.Add(franchisetaxdetail.IsIncludeSaleRate);
                paramValues.Add(franchisetaxdetail.IsActive);
                paramValues.Add(true); //franchisetaxdetail.IsPercentage
                paramValues.Add(franchisetaxdetail.LowerLimit);
                paramValues.Add(franchisetaxdetail.UpperLimit);
                paramValues.Add(lfranchisetaxdetail.CreateDate);
                paramValues.Add(lfranchisetaxdetail.CreateBy);
                paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                paramValues.Add(CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                paramValues.Add(CommonFunctions.GetClientIP());
                paramValues.Add("Browser");
                paramValues.Add("Browser");
                paramValues.Add("UPDATE");
                paramValues.Add(0);

                BusinessLogicLayer.TaxationManagement obj = new TaxationManagement(fConnectionString);
                string msg = obj.InsertUpdate_FranchiseTaxDetail(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE);

                ViewBag.Message = msg;

                 return View(franchisetaxdetail);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Edit[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Updation!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Updation!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        // GET: /FranchiseTaxDetail/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseTaxDetail/CanDelete")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                FranchiseTaxDetail franchisetaxdetail = db.FranchiseTaxDetails.Find(id);
                if (franchisetaxdetail == null)
                {
                    return HttpNotFound();
                }
                return View(franchisetaxdetail);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Delete[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Deletion!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        // POST: /FranchiseTaxDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseTaxDetail/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                FranchiseTaxDetail franchisetaxdetail = db.FranchiseTaxDetails.Find(id);
                db.FranchiseTaxDetails.Remove(franchisetaxdetail);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Delete[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Deletion!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Delete Bank Detail :- " + ex.InnerException.ToString();
                return View();
            }
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
