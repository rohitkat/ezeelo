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
using ModelLayer.Models.ViewModel;
using System.Web.Configuration;
using Administrator.Models;

namespace Administrator.Controllers
{
    public class TaxationBaseController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
          Environment.NewLine
          + "ErrorLog Controller : TaxationBaseController" + Environment.NewLine);
        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();

        // GET: /TaxationBase/
        [SessionExpire]
        [CustomAuthorize(Roles = "TaxationBase/CanRead")]
        public ActionResult Index()
        {
            try
            {
                BusinessLogicLayer.TaxationManagement obj = new TaxationManagement(fConnectionString);
                List<IndirectTaxSetting> ls = new List<IndirectTaxSetting>();
                ls = obj.Select_IndirectTaxSetting(0, 0);
                // var taxationbases = db.TaxationBases.Include(t => t.FranchiseTaxDetail).Include(t => t.TaxationMaster);
                return View(ls.ToList());
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

        // GET: /TaxationBase/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "TaxationBase/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TaxationBase taxationbase = db.TaxationBases.Find(id);
                if (taxationbase == null)
                {
                    return HttpNotFound();
                }
                FranchiseTaxDetail ftd = db.FranchiseTaxDetails.Find(taxationbase.FranchiseTaxDetailID);
                ViewBag.Franchise = db.Franchises.Find(ftd.FranchiseID).ContactPerson;
                ViewBag.TaxationID = db.TaxationMasters.Find(ftd.TaxationID).Name;
                ViewBag.FranchiseTaxDetailID = new SelectList(db.FranchiseTaxDetails, "ID", "NetworkIP", taxationbase.FranchiseTaxDetailID);
                ViewBag.TaxationDependOn = db.TaxationMasters.Find(taxationbase.TaxationID).Name;
                return View(taxationbase);
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

        // GET: /TaxationBase/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "TaxationBase/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.Franchise = new SelectList(db.Franchises, "ID", "ContactPerson");
                ViewBag.FranchiseTaxDetailID = new SelectList(db.FranchiseTaxDetails, "ID", "NetworkIP");
                List<DropdownList> ddl = new List<DropdownList>();
                ddl = (from n in db.TaxationMasters
                      join ft in db.FranchiseTaxDetails on n.ID equals ft.TaxationID
                      where n.IsActive == true && ft.IsActive== true && ft.IsDirect==false
                      select new DropdownList
                      {
                          Name = n.Name,
                          ID = n.ID
                      }).ToList();
                ViewBag.TaxationID = new SelectList(ddl.Distinct().ToList(), "ID", "Name");

                List<DropdownList> ddl1 = new List<DropdownList>();
                ddl1 = (from n in db.TaxationMasters
                      join ft in db.FranchiseTaxDetails on n.ID equals ft.TaxationID
                      where n.IsActive == true && ft.IsActive== true && ft.IsDirect==true
                      select new DropdownList
                      {
                          Name = n.Name,
                          ID = n.ID
                      }).ToList();
                ViewBag.TaxationDependOn = new SelectList(ddl1.Distinct().ToList(), "ID", "Name");
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

        // POST: /TaxationBase/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "TaxationBase/CanWrite")]
        public ActionResult Create([Bind(Include = "ID,TaxationID,FranchiseTaxDetailID,Description,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] TaxationBase taxationbase, int Franchise, int TaxationDependOn)
        {
            try
            {

                //ViewBag.TaxationDependOn = new SelectList(db.TaxationMasters, "ID", "Prefix", TaxationDependOn);
                ViewBag.Franchise = new SelectList(db.Franchises, "ID", "ContactPerson", Franchise);
                ViewBag.FranchiseTaxDetailID = new SelectList(db.FranchiseTaxDetails, "ID", "NetworkIP", taxationbase.FranchiseTaxDetailID);
                //ViewBag.TaxationID = new SelectList(db.TaxationMasters, "ID", "Prefix", taxationbase.TaxationID);

                List<DropdownList> ddl = new List<DropdownList>();
                ddl = (from n in db.TaxationMasters
                       join ft in db.FranchiseTaxDetails on n.ID equals ft.TaxationID
                       where n.IsActive == true && ft.IsActive == true && ft.IsDirect == false
                       select new DropdownList
                       {
                           Name = n.Name,
                           ID = n.ID
                       }).ToList();
                ViewBag.TaxationID = new SelectList(ddl, "ID", "Name");

                List<DropdownList> ddl1 = new List<DropdownList>();
                ddl1 = (from n in db.TaxationMasters
                        join ft in db.FranchiseTaxDetails on n.ID equals ft.TaxationID
                        where n.IsActive == true && ft.IsActive == true && ft.IsDirect == true
                        select new DropdownList
                        {
                            Name = n.Name,
                            ID = n.ID
                        }).ToList();
                ViewBag.TaxationDependOn = new SelectList(ddl1, "ID", "Name");

                int taxID = taxationbase.TaxationID;
                if (db.FranchiseTaxDetails.Where(x => x.FranchiseID == Franchise && x.TaxationID == taxID && x.IsDirect == false && x.IsActive == true).Count() > 0)
                {
                    int franchiseDetailtaxID = db.FranchiseTaxDetails.Where(x => x.FranchiseID == Franchise && x.TaxationID == taxID && x.IsDirect == false && x.IsActive == true).FirstOrDefault().ID;
                    if (db.TaxationBases.Where(x => x.TaxationID == TaxationDependOn && x.FranchiseTaxDetailID == franchiseDetailtaxID && x.IsActive == true).Count() < 1)
                    {
                        taxationbase.TaxationID = TaxationDependOn;
                        taxationbase.FranchiseTaxDetailID = franchiseDetailtaxID;
                        taxationbase.CreateDate = DateTime.UtcNow.AddHours(5.3);
                        taxationbase.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                        taxationbase.NetworkIP = CommonFunctions.GetClientIP();
                        if (ModelState.IsValid)
                        {
                            db.TaxationBases.Add(taxationbase);
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                    ViewBag.Message = "Indeirect tax entry already exist in Franchise!!";

                    return View(taxationbase);
                }
                ViewBag.Message = "Sorry! No Indeirect tax Found in Franchise!!";
                

                return View(taxationbase);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Create[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                ViewBag.Message = "Sorry! Problem in Create Record!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        // GET: /TaxationBase/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TaxationBase taxationbase = db.TaxationBases.Find(id);
                if (taxationbase == null)
                {
                    return HttpNotFound();
                }

                FranchiseTaxDetail ftd = db.FranchiseTaxDetails.Find(taxationbase.FranchiseTaxDetailID);
                ViewBag.Franchise = db.Franchises.Find(ftd.FranchiseID).ContactPerson;
                ViewBag.TaxationDependOn = db.TaxationMasters.Find(ftd.TaxationID).Name;
                ViewBag.FranchiseTaxDetailID = new SelectList(db.FranchiseTaxDetails, "ID", "NetworkIP", taxationbase.FranchiseTaxDetailID);
                ViewBag.TaxationID = db.TaxationMasters.Find(taxationbase.TaxationID).Name;
                return View(taxationbase);
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

        // POST: /TaxationBase/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "TaxationBase/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,TaxationID,FranchiseTaxDetailID,Description,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] TaxationBase taxationbase)
        {
            try
            {
                TaxationBase ltaxationbase = db.TaxationBases.Find(taxationbase.ID);
                //taxationbase.CreateDate = ltaxationbase.CreateDate;
                //taxationbase.CreateBy = ltaxationbase.CreateBy;
                ltaxationbase.IsActive = taxationbase.IsActive;
                ltaxationbase.Description = taxationbase.Description;
                ltaxationbase.ModifyDate = DateTime.UtcNow.AddHours(5.5);
                ltaxationbase.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                ltaxationbase.NetworkIP = CommonFunctions.GetClientIP();
                ltaxationbase.DeviceType = string.Empty;
                ltaxationbase.DeviceID = string.Empty;


                if (ModelState.IsValid)
                {
                    db.Entry(ltaxationbase).State = EntityState.Modified;
                    //db.Entry(taxationbase).CurrentValues.SetValues(ltaxationbase);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.FranchiseTaxDetailID = new SelectList(db.FranchiseTaxDetails, "ID", "NetworkIP", taxationbase.FranchiseTaxDetailID);
                ViewBag.TaxationID = new SelectList(db.TaxationMasters, "ID", "Prefix", taxationbase.TaxationID);
                return View(taxationbase);
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

        // GET: /TaxationBase/Delete/5

        [SessionExpire]
        [CustomAuthorize(Roles = "TaxationBase/CanDelete")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TaxationBase taxationbase = db.TaxationBases.Find(id);
                if (taxationbase == null)
                {
                    return HttpNotFound();
                }
                return View(taxationbase);
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

        // POST: /TaxationBase/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "TaxationBase/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                TaxationBase taxationbase = db.TaxationBases.Find(id);
                db.TaxationBases.Remove(taxationbase);
                db.SaveChanges();
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
