using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using BusinessLogicLayer;
using Gandhibagh.Models;

namespace Gandhibagh.Controllers
{
    public class ReferDetailsController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /ReferDetails/
        public ActionResult Index()
        {
            var referdetailviewmodels = db.ReferDetailViewModels.Include(r => r.ReferAndEarnSchema);
            return View(referdetailviewmodels.ToList());
        }

        // GET: /ReferDetails/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReferDetailViewModel referdetailviewmodel = db.ReferDetailViewModels.Find(id);
            if (referdetailviewmodel == null)
            {
                return HttpNotFound();
            }
            return View(referdetailviewmodel);
        }

        // GET: /ReferDetails/Create
        public ActionResult Create()
        {
            URLCookie.SetCookies();
            long cityId = 4968;
            int franchiseId = 2;//added
            //ViewBag.totalEarn = new SelectList(db.EarnDetails, "ID", "Name");
            if (Session["UID"] != null)
            {
                if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value != string.Empty)
                {
                    cityId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0].Trim());
                    franchiseId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2].Trim());//added
                }
                //var schemelist=db.ReferAndEarnSchemas.Where(x => x.IsActive == true && x.CityID == cityId).ToList();//hide
                var schemelist = db.ReferAndEarnSchemas.Where(x => x.IsActive == true && x.FranchiseID == franchiseId).ToList();//--added by Ashish for multiple franchise in same city--//
                ReferDetailViewModel lReferDetailViewModel = new ReferDetailViewModel();
                if (schemelist != null && schemelist.Count()>0)
                {

                    //ViewBag.ReferAndEarnSchemaID = new SelectList(schemelist, "ID", "Name");
                    ViewBag.ReferAndEarnSchemaName = schemelist.FirstOrDefault().Name;
                    ViewBag.ReferAndEarnSchemaID = schemelist.FirstOrDefault().ID;
                    List<SubReferDetail> ci = new List<SubReferDetail> { new SubReferDetail { ID = 0, Email = "", Mobile = "" } };
                    
                    // lReferDetailViewModel.DisplayProductFromDate = DateTime.Now;
                    lReferDetailViewModel.lSubReferDetail = ci;
                    //this.GetBudgetDetails(db.ReferAndEarnSchemas.Where(x => x.IsActive == true && x.CityID==cityId).Select(x=>x.ID).FirstOrDefault());
                    this.GetBudgetDetails(schemelist.FirstOrDefault().ID);

                    
                }
                else
                {
                    ViewBag.SchemeNotAvailMsg = "Sorry! Scheme is not available for this MCO.";
                }
                return View(lReferDetailViewModel);

            }
            else
            {
                return RedirectToRoute("Login");
            }

        }

        // POST: /ReferDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ReferAndEarnSchemaID,UserID,ReferenceID,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID,lSubReferDetail")] ReferDetailViewModel referdetailviewmodel)
        {
            try
            {
                if (Session["UID"] != null)
                {
                    long cityId = 4968;
                    int franchiseId = 2;//added
                    if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value != string.Empty)
                    {
                        cityId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0].Trim());
                        franchiseId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2].Trim());//added
                    }
                    ReferAndEarn lReferAndEarn = new ReferAndEarn();
                    referdetailviewmodel = lReferAndEarn.InsertReferDetail(referdetailviewmodel, Convert.ToInt64(Session["UID"]));
                    //ViewBag.ReferAndEarnSchemaID = new SelectList(db.ReferAndEarnSchemas.Where(x => x.IsActive == true && x.CityID == cityId), "ID", "Name", referdetailviewmodel.ReferAndEarnSchemaID);
                   
                    //ViewBag.ReferAndEarnSchemaID = db.ReferAndEarnSchemas.Where(x => x.IsActive == true && x.CityID == cityId).FirstOrDefault().Name;//hide
                    //ViewBag.ReferAndEarnSchemaName = db.ReferAndEarnSchemas.Where(x => x.IsActive == true && x.CityID == cityId).Select(x => x.Name).FirstOrDefault();//hide

                    ViewBag.ReferAndEarnSchemaID = db.ReferAndEarnSchemas.Where(x => x.IsActive == true && x.FranchiseID == franchiseId).FirstOrDefault().Name;//--added by Ashish for multiple franchise in same city--//
                    ViewBag.ReferAndEarnSchemaName = db.ReferAndEarnSchemas.Where(x => x.IsActive == true && x.FranchiseID == franchiseId).Select(x => x.Name).FirstOrDefault();//--added by Ashish for multiple franchise in same city--//

                    ViewBag.ReferAndEarnSchemaID = referdetailviewmodel.ReferAndEarnSchemaID;

                    this.GetBudgetDetails(referdetailviewmodel.ReferAndEarnSchemaID);
                    return View(referdetailviewmodel);
                }
                else
                {
                    return RedirectToRoute("Login");
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Refer Detail Create!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ReferDetails][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                ViewBag.ReferAndEarnSchemaID = new SelectList(db.ReferAndEarnSchemas, "ID", "Name", referdetailviewmodel.ReferAndEarnSchemaID);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Refer Detail Create!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ReferDetail][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                ViewBag.ReferAndEarnSchemaID = new SelectList(db.ReferAndEarnSchemas, "ID", "Name", referdetailviewmodel.ReferAndEarnSchemaID);
            }
            return View();

        }

        // GET: /ReferDetails/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReferDetailViewModel referdetailviewmodel = db.ReferDetailViewModels.Find(id);
            if (referdetailviewmodel == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReferAndEarnSchemaID = new SelectList(db.ReferAndEarnSchemas, "ID", "Name", referdetailviewmodel.ReferAndEarnSchemaID);
            return View(referdetailviewmodel);
        }

        // POST: /ReferDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ReferAndEarnSchemaID,UserID,ReferenceID,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] ReferDetailViewModel referdetailviewmodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(referdetailviewmodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ReferAndEarnSchemaID = new SelectList(db.ReferAndEarnSchemas, "ID", "Name", referdetailviewmodel.ReferAndEarnSchemaID);
            return View(referdetailviewmodel);
        }

        // GET: /ReferDetails/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReferDetailViewModel referdetailviewmodel = db.ReferDetailViewModels.Find(id);
            if (referdetailviewmodel == null)
            {
                return HttpNotFound();
            }
            return View(referdetailviewmodel);
        }

        // POST: /ReferDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ReferDetailViewModel referdetailviewmodel = db.ReferDetailViewModels.Find(id);
            db.ReferDetailViewModels.Remove(referdetailviewmodel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void GetBudgetDetails(long schemeID)
        {
            try
            {
                ViewBag.RemBudgetAmt = db.SchemeBudgetTransactions.Where(x=>x.ReferAndEarnSchemaID==schemeID).Select(x=>x.RemainingAmt).FirstOrDefault();
                //ViewBag.ExpiryDate = db.SchemeBudgetTransactions.Where(x => x.ReferAndEarnSchemaID == schemeID).Select(x => x.ExpiryDate).FirstOrDefault() - DateTime.Now;
                DateTime date = db.SchemeBudgetTransactions.Where(x => x.ReferAndEarnSchemaID == schemeID).Select(x => x.ExpiryDate).FirstOrDefault();
                if (date.Date > DateTime.UtcNow.Date)
                {
                    ViewBag.ExpiryDate = date.Subtract(DateTime.UtcNow).Days.ToString();
                }
            }
            catch (Exception)
            {
                
                throw;
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
