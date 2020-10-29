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
using System.Text;
using Administrator.Models;

namespace Administrator.Controllers
{
    public class ReferAndEarnSchemaController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
         Environment.NewLine
         + "ErrorLog Controller : ReferAndEarnSchemaController" + Environment.NewLine);

        [SessionExpire]
        [CustomAuthorize(Roles = "ReferAndEarnSchema/CanRead")]
        // GET: /ReferAndEarnSchema/
        public ActionResult Index()
        {
            try
            {

                return View(db.ReferAndEarnSchemas.ToList());
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

        [SessionExpire]
        [CustomAuthorize(Roles = "ReferAndEarnSchema/CanRead")]
        // GET: /ReferAndEarnSchema/Details/5
        public ActionResult Details(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ReferAndEarnSchema referandearnschema = db.ReferAndEarnSchemas.Find(id);
                if (referandearnschema == null)
                {
                    return HttpNotFound();
                }
                return View(referandearnschema);
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

        [SessionExpire]
        [CustomAuthorize(Roles = "ReferAndEarnSchema/CanWrite")]
        // GET: /ReferAndEarnSchema/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");

                ////////////added
                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
                /////////////////
                
                return View();
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Details[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Create View!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }


        [SessionExpire]
        [CustomAuthorize(Roles = "ReferAndEarnSchema/CanWrite")]
        // POST: /ReferAndEarnSchema/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,OrderwiseEarn,UserwiseEarn,EarnInRS,EarnInPercentage,MaxNoOfOrders,MaxPurchaseAmount,ExpirationDays,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID,CityID,FranchiseID")] ReferAndEarnSchema referandearnschema)////added ,FranchiseID
        {
            try
            {
                ///////////added
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
                ///////////

                referandearnschema.CreateDate = DateTime.UtcNow.AddHours(5.3);
                referandearnschema.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                referandearnschema.NetworkIP = CommonFunctions.GetClientIP();
                referandearnschema.IsActive = true;
                if (ModelState.IsValid)
                {
                    db.ReferAndEarnSchemas.Add(referandearnschema);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(referandearnschema);
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


        [SessionExpire]
        [CustomAuthorize(Roles = "ReferAndEarnSchema/CanWrite")]
        // GET: /ReferAndEarnSchema/Edit/5
        public ActionResult Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                ReferAndEarnSchema referandearnschema = db.ReferAndEarnSchemas.Find(id);
                ///////////added
                referandearnschema.CityID = db.ReferAndEarnSchemas.Where(x => x.ID == id).Select(y => y.CityID).FirstOrDefault();
                referandearnschema.FranchiseID = db.ReferAndEarnSchemas.Where(x => x.ID == id).Select(y => y.FranchiseID).FirstOrDefault();
                if (referandearnschema == null)
                {
                    return HttpNotFound();
                }
                ////////////
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", referandearnschema.CityID);

                ViewBag.FranchiseID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
                && x.BusinessDetail.Pincode.City.ID == referandearnschema.CityID).ToList(), "ID", "ContactPerson", referandearnschema.FranchiseID);////added 2 "ContactPerson"->"ID" referandearnschema.FranchiseID

                if (referandearnschema == null)
                {
                    return HttpNotFound();
                }
                return View(referandearnschema);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Edit[GET]" + Environment.NewLine +
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


        [SessionExpire]
        [CustomAuthorize(Roles = "ReferAndEarnSchema/CanWrite")]
        // POST: /ReferAndEarnSchema/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,OrderwiseEarn,UserwiseEarn,EarnInRS,EarnInPercentage,MaxNoOfOrders,MaxPurchaseAmount,ExpirationDays,IsActive,CityID,FranchiseID")] ReferAndEarnSchema referandearnschema, string rbtIsRupee)////added ,FranchiseID
        {

            try
            {
                //////////////added
                EzeeloDBContext db1 = new EzeeloDBContext();
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
                //////////////

                ReferAndEarnSchema lReferAndEarnSchema = db.ReferAndEarnSchemas.Single(x => x.ID == referandearnschema.ID);
                lReferAndEarnSchema.Name = referandearnschema.Name;
                lReferAndEarnSchema.OrderwiseEarn = referandearnschema.OrderwiseEarn;
                lReferAndEarnSchema.UserwiseEarn = referandearnschema.UserwiseEarn;
                if (rbtIsRupee.Trim().ToLower() == "EarnInRS".Trim().ToLower())
                {
                    lReferAndEarnSchema.EarnInRS = referandearnschema.EarnInRS;
                    lReferAndEarnSchema.EarnInPercentage = null;
                }
                else
                {
                    lReferAndEarnSchema.EarnInRS = null;
                    lReferAndEarnSchema.EarnInPercentage = referandearnschema.EarnInPercentage;
                }

                lReferAndEarnSchema.MaxNoOfOrders = referandearnschema.MaxNoOfOrders;
                lReferAndEarnSchema.MaxPurchaseAmount = referandearnschema.MaxPurchaseAmount;
                lReferAndEarnSchema.ExpirationDays = referandearnschema.ExpirationDays;
                lReferAndEarnSchema.IsActive = referandearnschema.IsActive;
                lReferAndEarnSchema.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                lReferAndEarnSchema.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                lReferAndEarnSchema.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                lReferAndEarnSchema.DeviceType = "WEB";
                lReferAndEarnSchema.DeviceID = string.Empty;
                db.SaveChanges();
                return RedirectToAction("Index");
                //referandearnschema.CreateDate = data.CreateDate;
                //referandearnschema.CreateBy = data.CreateBy;
                //referandearnschema.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                //referandearnschema.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                //referandearnschema.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                //referandearnschema.DeviceType = string.Empty;
                //referandearnschema.DeviceID = string.Empty;

                //if (rbtIsRupee == "EarnInRS")
                //{
                //    referandearnschema.EarnInPercentage = null;
                //}
                //else
                //{
                //    referandearnschema.EarnInRS = null;
                //}

                //if (ModelState.IsValid)
                //{
                //   // db.Entry(referandearnschema).State = EntityState.Modified;
                //    db.SaveChanges();
                //    return RedirectToAction("Index");
                //}
                // return View(referandearnschema);
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


        [SessionExpire]
        [CustomAuthorize(Roles = "ReferAndEarnSchema/CanDelete")]
        // GET: /ReferAndEarnSchema/Delete/5
        public ActionResult Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ReferAndEarnSchema referandearnschema = db.ReferAndEarnSchemas.Find(id);
                if (referandearnschema == null)
                {
                    return HttpNotFound();
                }
                return View(referandearnschema);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Delete[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Deletion!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        // POST: /ReferAndEarnSchema/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "ReferAndEarnSchema/CanDelete")]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                ReferAndEarnSchema referandearnschema = db.ReferAndEarnSchemas.Find(id);
                db.ReferAndEarnSchemas.Remove(referandearnschema);
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
                ViewBag.Messaage = "Unable to Delete Refer and Earn Detail :- " + ex.InnerException.ToString();
                return View();
            }
        }


        public JsonResult getFranchise(int CityID)////added
        {
            List<tempData> objODP = new List<tempData>();

            objODP = db.Franchises
                    .Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
                    && x.BusinessDetail.Pincode.City.ID == CityID)
                   // .Select(x => new tempData { text = x.ID.ToString(), value = x.ID } ////ContactPerson->ID
                   // .Select(x => new tempData { text = x.BusinessDetail.Name.ToString(), value = x.ID } ////ID->BusinessDetail.Name
                    .Select(x => new tempData { text = x.ContactPerson.ToString(), value = x.ID }
                    ).OrderBy(x => x.text)
                    .ToList();

            return Json(objODP, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public class tempData////added
        {
            public Int64 value;
            public string text;
        }
    }
}
