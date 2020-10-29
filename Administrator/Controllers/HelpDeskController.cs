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
using PagedList;
using PagedList.Mvc;
using System.Text;
using Administrator.Models;

namespace Administrator.Controllers
{
    public class HelpDeskController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
       Environment.NewLine
       + "ErrorLog Controller : DeliveryScheduleController" + Environment.NewLine);

        // GET: /HelpDesk/
        [SessionExpire]
        [CustomAuthorize(Roles = "HelpDesk/CanRead")]
        public ActionResult Index(int? page, Int64? city)
        {
             try
            {

            var helpdeskdetails = db.HelpDeskDetails.Include(h => h.City).Include(h => h.Franchise);
            return View(helpdeskdetails.ToList());
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

        // GET: /HelpDesk/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "HelpDesk/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HelpDeskDetails helpdeskdetails = db.HelpDeskDetails.Find(id);
            if (helpdeskdetails == null)
            {
                return HttpNotFound();
            }
            return View(helpdeskdetails);
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

        // GET: /HelpDesk/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "HelpDesk/CanRead")]
        public ActionResult Create()
        {
          try
            {
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
             /* var lCityByFranchise=(from F in db.Franchises 
                                    join BD in db.BusinessDetails on F.BusinessDetailID equals BD.ID
                                    join UL in db.UserLogins  on BD.UserLoginID equals UL.ID 
                                    join P in db.Pincodes on BD.PincodeID equals P.ID
                                    join C in db.Cities  on P.CityID equals C.ID  
                                    where F.ID!=1 && F.IsActive==true && C.IsActive== true && UL.IsLocked==false
                                        
                                    Select new{
                                       ID=C.ID,
                                       Name=C.Name

                                    }).AsEnumerable();*/
              
                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
               //// ViewBag.FranchiseID = new SelectList(db.Franchises.OrderBy(x => x.ContactPerson).Where(c => c.IsActive == true).ToList(), "ID", "ContactPerson");

            return View();
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

        // POST: /HelpDesk/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "HelpDesk/CanRead")]
        public ActionResult Create([Bind(Include="ID,CityID,FranchiseID,HelpLineNumber,ManagmentNumber,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] HelpDeskDetails helpdeskdetails)
        {
             try
            {
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
               
                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");
               //// ViewBag.FranchiseID = new SelectList(db.Franchises.OrderBy(x => x.ContactPerson).Where(c => c.IsActive == true).ToList(), "ID", "ContactPerson");

                 //if(db.HelpDeskDetails.Where(x=>x.HelpLineNumber==helpdeskdetails.HelpLineNumber).Count()>0)
                 //{
                 //    ViewBag.lblError = "Help Line Number is already Exist..!!";
                 //    return View(helpdeskdetails);
                 //}
                 if (db.HelpDeskDetails.Where(x => x.ManagmentNumber == helpdeskdetails.ManagmentNumber && helpdeskdetails.ManagmentNumber!=null).Count() > 0)
                 {
                     ViewBag.lblError = "Management Number is already Exist..!!";
                     return View(helpdeskdetails);
                 }
                helpdeskdetails.CreateDate = DateTime.UtcNow.AddHours(5.3);
                helpdeskdetails.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                helpdeskdetails.NetworkIP = CommonFunctions.GetClientIP();
            if (ModelState.IsValid)
            {
                db.HelpDeskDetails.Add(helpdeskdetails);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(helpdeskdetails);
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

        // GET: /HelpDesk/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "HelpDesk/CanRead")]
        public ActionResult Edit(int? id)
        {
           try
            {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HelpDeskDetails helpdeskdetails = db.HelpDeskDetails.Find(id);
            helpdeskdetails.CityID = db.HelpDeskDetails.Where(x => x.ID == id).Select(y => y.CityID).FirstOrDefault();
            helpdeskdetails.FranchiseID = db.HelpDeskDetails.Where(x => x.ID == id).Select(y => y.FranchiseID).FirstOrDefault();
            ViewBag.OldHelpLineNumber = helpdeskdetails.HelpLineNumber;
            ViewBag.OldManagmentNumber = helpdeskdetails.ManagmentNumber;
            if (helpdeskdetails == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", helpdeskdetails.CityID);
            ////ViewBag.FranchiseID = new SelectList(db.Franchises.OrderBy(x => x.ContactPerson).Where(c => c.IsActive == true).ToList(), "ID", "ContactPerson", helpdeskdetails.FranchiseID);

            ViewBag.FranchiseID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
               && x.BusinessDetail.Pincode.City.ID == helpdeskdetails.CityID).ToList(), "ID", "ContactPerson", helpdeskdetails.FranchiseID);

            return View(helpdeskdetails);
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

        // POST: /HelpDesk/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "HelpDesk/CanRead")]
        public ActionResult Edit(HelpDeskDetails helpdeskdetails, string hdnOldHelpLineNumber, string hdnOldManagmentNumber)
        {
             try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                ViewBag.CityID = new SelectList(db.Cities.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.FranchiseID = new SelectList(lData, "Value", "Text");               

                //if (helpdeskdetails.HelpLineNumber != hdnOldHelpLineNumber)
                //{
                //    if (db.HelpDeskDetails.Where(x => x.HelpLineNumber == helpdeskdetails.HelpLineNumber).Count() > 0)
                //    {
                //        ViewBag.lblError = "Help Line Number is already Exist..!!";
                //        return View(helpdeskdetails);
                //    }
                //}

                if (helpdeskdetails.ManagmentNumber !=null  && helpdeskdetails.ManagmentNumber != hdnOldManagmentNumber)
                {
                    if (db.HelpDeskDetails.Where(x => x.ManagmentNumber == helpdeskdetails.ManagmentNumber).Count() > 0 && (helpdeskdetails.ManagmentNumber != null))
                    {
                        ViewBag.lblError = "Management Number is already Exist..!!";
                        return View(helpdeskdetails);
                    }
                }
                HelpDeskDetails lhelpdeskdetails = db1.HelpDeskDetails.Find(helpdeskdetails.ID);
                helpdeskdetails.CreateDate = lhelpdeskdetails.CreateDate;
                helpdeskdetails.CreateBy = lhelpdeskdetails.CreateBy;
                helpdeskdetails.ModifyDate = DateTime.UtcNow.AddHours(5.5);
                helpdeskdetails.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                helpdeskdetails.NetworkIP = CommonFunctions.GetClientIP();
                helpdeskdetails.DeviceType = string.Empty;
                helpdeskdetails.DeviceID = string.Empty;
                db1.Dispose();
            if (ModelState.IsValid)
            {
                db.Entry(helpdeskdetails).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.CityID = new SelectList(db.Cities, "ID", "Name", helpdeskdetails.CityID);
            //ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber", helpdeskdetails.FranchiseID);
            return View(helpdeskdetails);
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

        // GET: /HelpDesk/Delete/5
       /* [SessionExpire]
        [CustomAuthorize(Roles = "HelpDesk/CanRead")]
        public ActionResult Delete(int? id)
        {
          try
            {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HelpDeskDetails helpdeskdetails = db.HelpDeskDetails.Find(id);
            if (helpdeskdetails == null)
            {
                return HttpNotFound();
            }
            return View(helpdeskdetails);
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

        // POST: /HelpDesk/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "HelpDesk/CanRead")]
        public ActionResult DeleteConfirmed(int id)
        {
          try
            {
            HelpDeskDetails helpdeskdetails = db.HelpDeskDetails.Find(id);
            db.HelpDeskDetails.Remove(helpdeskdetails);
            db.SaveChanges();
            return RedirectToAction("Index");
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
              ViewBag.referenceMsg = "Entry you wish to delete is already used by customer while placing order";
              return View();
          }
        }*/

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public JsonResult getFranchise(int CityID)
        {
            List<tempData> objODP = new List<tempData>();

            /*objODP = db.Districts
                .Where(x => x.StateID == CityID)
                .Select(x => new tempData { text = x.Name, value = x.ID }
                ).OrderBy(x => x.text)
                .ToList();

            return Json(objODP, JsonRequestBehavior.AllowGet);*/
            /////
           /* objODP = (from f in db.Franchises
                           where f.ID != 1 && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                           && f.BusinessDetail.Pincode.City.ID == CityID
                      select f.BusinessDetail.Pincode.CityID).ToList();*/

            objODP = db.Franchises
                    .Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
                    && x.BusinessDetail.Pincode.City.ID == CityID)
                    .Select(x => new tempData { text = x.ContactPerson, value = x.ID }
                    ).OrderBy(x => x.text)
                    .ToList();

            return Json(objODP, JsonRequestBehavior.AllowGet);
        }
        public class tempData
        {
            public Int64 value;
            public string text;
        }
    }
}
