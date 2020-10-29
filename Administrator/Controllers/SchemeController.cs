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
using System.Transactions;
using System.Data.Entity.Validation;
using BusinessLogicLayer;

namespace Administrator.Controllers
{
    public class SchemeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /Scheme/
          [Authorize(Roles = "Scheme/CanRead")]
        public ActionResult Index()
        {
            try
            {
                List<SchemeType> lSchemeTypes = db.SchemeTypes.ToList();
                List<TodayScheme> lTodaySchemes = db.TodaySchemes.ToList();

                List<Scheme> lSchemes = (from st in lSchemeTypes
                                         join ts in lTodaySchemes on st.ID equals ts.SchemeTypeID
                                         where st.IsActive == true
                                         select new ModelLayer.Models.ViewModel.Scheme
                                         {
                                             ID = st.ID,
                                             Name = st.Name,
                                             ValueInRs = st.ValueInRs,
                                             ApplicableOnPurchaseOfRs = st.ApplicableOnPurchaseOfRs,
                                             SchemeCode = st.SchemeCode,
                                             OwnerName = st.PersonalDetail.FirstName,
                                             BussinessTypeName = st.BusinessType.Name,
                                             OwnerId = st.OwnerId,
                                             BusinessTypeID = (int)st.BussinessTypeID,
                                             IsActive = st.IsActive,
                                             StartDatetime = ts.StartDatetime,
                                             EndDatetime = ts.EndDatetime,
                                             CreateDate = st.CreateDate

                                         }).ToList();
                return View(lSchemes);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Scheme Index page !!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SchemeController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Scheme Index page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SchemeController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
           

            return View();
        }

        // GET: /Scheme/Details/5
          [Authorize(Roles = "Scheme/CanRead")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                List<SchemeType> lSchemeTypes = db.SchemeTypes.ToList();
                List<TodayScheme> lTodaySchemes = db.TodaySchemes.ToList();

                Scheme lSchemes = (from st in lSchemeTypes
                                   join ts in lTodaySchemes on st.ID equals ts.SchemeTypeID
                                   where st.ID == id && st.IsActive == true
                                   select new ModelLayer.Models.ViewModel.Scheme
                                   {
                                       Name = st.Name,
                                       ValueInRs = st.ValueInRs,
                                       ApplicableOnPurchaseOfRs = st.ApplicableOnPurchaseOfRs,
                                       SchemeCode = st.SchemeCode,
                                       OwnerName = st.PersonalDetail.FirstName,
                                       BussinessTypeName = st.BusinessType.Name,
                                       OwnerId = st.OwnerId,
                                       BusinessTypeID = (int)st.BussinessTypeID,
                                       IsActive = st.IsActive,
                                       StartDatetime = ts.StartDatetime,
                                       EndDatetime = ts.EndDatetime,
                                       CreateDate = st.CreateDate

                                   }).FirstOrDefault();
                if (lSchemes == null)
                {
                    return HttpNotFound();
                }
                return View(lSchemes);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Scheme Detail page !!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SchemeController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Scheme Detail page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SchemeController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();

        }

        // GET: /Scheme/Create
          [Authorize(Roles = "Scheme/CanRead")]
        public ActionResult Create()
        {
            ViewBag.BussinessType = new SelectList(db.BusinessTypes, "ID", "Name");
            return View();
        }

        // POST: /Scheme/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
         [Authorize(Roles = "Scheme/CanWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
          public ActionResult Create([Bind(Include = "ID,Name,ValueInRs,ApplicableOnPurchaseOfRs,SchemeCode,OwnerId,BusinessTypeID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID,StartDatetime,EndDatetime")] Scheme scheme, string StartDatetime1, string EndDatetime1)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!db.SchemeTypes.Any(m => m.Name == scheme.Name))
                    {
                        ViewBag.BussinessType = new SelectList(db.BusinessTypes, "ID", "Name", scheme.BusinessTypeID);
                        using (TransactionScope ts = new TransactionScope())
                        {
                           

                            long PersonalDetailID = this.GetPersonalDetailID();

                            SchemeType lSchemeType = new SchemeType();
                            lSchemeType.Name = scheme.Name;
                            lSchemeType.ValueInRs = scheme.ValueInRs;
                            lSchemeType.ApplicableOnPurchaseOfRs = scheme.ApplicableOnPurchaseOfRs;
                            lSchemeType.SchemeCode = this.GetSchemeCode(); //Generate dynamically
                            lSchemeType.BussinessTypeID = scheme.BusinessTypeID;
                            lSchemeType.OwnerId = 1;    // Add comment
                            lSchemeType.IsActive = true;
                            lSchemeType.CreateDate = DateTime.UtcNow;
                            lSchemeType.CreateBy = PersonalDetailID;
                            lSchemeType.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                            lSchemeType.DeviceID = "x";
                            lSchemeType.DeviceType = "x";
                            db.SchemeTypes.Add(lSchemeType);
                            db.SaveChanges();

                            TodayScheme lTodayScheme = new TodayScheme();

                            DateTime lStartDatetime = DateTime.Now;
                            DateTime lEndDatetime = DateTime.Now;
                            if (StartDatetime1 != "")
                            {
                                lStartDatetime = CommonFunctions.GetProperDateTime(StartDatetime1);
                                lTodayScheme.StartDatetime = lStartDatetime;
                            }
                            if (EndDatetime1 != "")
                            {
                                lEndDatetime = CommonFunctions.GetProperDateTime(EndDatetime1);
                                lTodayScheme.EndDatetime = lEndDatetime;
                            }
                            lTodayScheme.SchemeTypeID = lSchemeType.ID; //Fetch from current entry
                            lTodayScheme.TodaysValueInRs = scheme.ValueInRs;
                            lTodayScheme.ApplicableOnPurchaseOfRs = scheme.ApplicableOnPurchaseOfRs;
                            //lTodayScheme.StartDatetime = scheme.StartDatetime;
                            lTodayScheme.StartDatetime = lStartDatetime;
                            //lTodayScheme.EndDatetime = scheme.EndDatetime;
                            lTodayScheme.EndDatetime = lEndDatetime;
                            lTodayScheme.IsActive = true;
                            lTodayScheme.CreateDate = DateTime.UtcNow;
                            lTodayScheme.CreateBy = PersonalDetailID;
                            lTodayScheme.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                            lTodayScheme.DeviceID = "x";
                            lTodayScheme.DeviceType = "x";
                            db.TodaySchemes.Add(lTodayScheme);
                            db.SaveChanges();

                            ts.Complete();
                            //db.Schemes.Add(scheme);
                            //db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Scheme Name is already Exists";
                        return View(scheme);
                    }
                  
                }
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => new { x.ErrorMessage });

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                ViewBag.Message = fullErrorMessage.Replace("ErrorMessage = ", " ");

                return View(scheme);
            }

            return View(scheme);
        }

        // GET: /Scheme/Edit/5
          [Authorize(Roles = "Scheme/CanRead")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<SchemeType> lSchemeTypes = db.SchemeTypes.ToList();
            List<TodayScheme> lTodaySchemes = db.TodaySchemes.ToList();

            Scheme lSchemes = (from st in lSchemeTypes
                               join ts in lTodaySchemes on st.ID equals ts.SchemeTypeID
                               where st.ID == id && st.IsActive == true
                               select new ModelLayer.Models.ViewModel.Scheme
                               {
                                   Name = st.Name,
                                   ValueInRs = st.ValueInRs,
                                   ApplicableOnPurchaseOfRs = st.ApplicableOnPurchaseOfRs,
                                   SchemeCode = st.SchemeCode,
                                   OwnerName = st.PersonalDetail.FirstName,
                                   BussinessTypeName = st.BusinessType.Name,
                                   OwnerId = st.OwnerId,
                                   BusinessTypeID = (int)st.BussinessTypeID,
                                   IsActive = st.IsActive,
                                   StartDatetime = ts.StartDatetime,
                                   EndDatetime = ts.EndDatetime,
                                   CreateDate = st.CreateDate
                                   
                               }).FirstOrDefault();
            ViewBag.StartDatetime1 = lSchemes.StartDatetime.ToString();//productuploadtempviewmodel.DisplayProductFromDate.ToString("dd/MM/yyyy");
            ViewBag.EndDatetime1 = lSchemes.EndDatetime.ToString();
            if (lSchemes == null)
            {
                return HttpNotFound();
            }
            return View(lSchemes);
        }


        // POST: /Scheme/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
         [Authorize(Roles = "Scheme/CanWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
          public ActionResult Edit([Bind(Include = "ID,Name,ValueInRs,ApplicableOnPurchaseOfRs,SchemeCode,OwnerId,BusinessTypeID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID,StartDatetime,EndDatetime")] Scheme scheme, string StartDatetime1, string EndDatetime1)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    using (TransactionScope ts = new TransactionScope())
                    {
                        long PersonalDetailID = this.GetPersonalDetailID();


                        SchemeType lSchemeType = db.SchemeTypes.Where(x => x.ID == scheme.ID).FirstOrDefault();


                        //SchemeType lSchemeType = new SchemeType();
                        lSchemeType.Name = scheme.Name;
                        lSchemeType.ValueInRs = scheme.ValueInRs;
                        lSchemeType.ApplicableOnPurchaseOfRs = scheme.ApplicableOnPurchaseOfRs;
                        lSchemeType.SchemeCode = this.GetSchemeCode(); //Generate dynamically
                        lSchemeType.BussinessTypeID = scheme.BusinessTypeID;
                        lSchemeType.OwnerId = 1;    // Add comment
                        lSchemeType.IsActive = true;
                        lSchemeType.CreateDate = DateTime.UtcNow;
                        lSchemeType.CreateBy = PersonalDetailID;
                        lSchemeType.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        lSchemeType.DeviceID = "x";
                        lSchemeType.DeviceType = "x";
                        //db.Entry(lSchemeType).State = EntityState.Modified;
                        db.SaveChanges();

                        //db.SchemeTypes.Add(lSchemeType);
                        //db.SaveChanges();
                        TodayScheme lTodayScheme = db.TodaySchemes.Where(x => x.SchemeTypeID == lSchemeType.ID).FirstOrDefault();
                        
                        DateTime lStartDatetime = DateTime.Now;
                        DateTime lEndDatetime = DateTime.Now;
                        if (StartDatetime1 != "")
                        {
                            lStartDatetime = CommonFunctions.GetProperDateTime(StartDatetime1);
                            lTodayScheme.StartDatetime = lStartDatetime;
                        }
                        if (EndDatetime1 != "")
                        {
                            lEndDatetime = CommonFunctions.GetProperDateTime(EndDatetime1);
                            lTodayScheme.EndDatetime = lEndDatetime;
                        }

                        //TodayScheme lTodayScheme = new TodayScheme();
                        lTodayScheme.SchemeTypeID = lSchemeType.ID; //Fetch from current entry
                        lTodayScheme.TodaysValueInRs = scheme.ValueInRs;
                        lTodayScheme.ApplicableOnPurchaseOfRs = scheme.ApplicableOnPurchaseOfRs;
                        //lTodayScheme.StartDatetime = scheme.StartDatetime;
                        //lTodayScheme.EndDatetime = scheme.EndDatetime;
                        lTodayScheme.StartDatetime = lStartDatetime;
                        lTodayScheme.EndDatetime = lEndDatetime;

                        lTodayScheme.IsActive = true;
                        lTodayScheme.CreateDate = DateTime.UtcNow;
                        lTodayScheme.CreateBy = PersonalDetailID;
                        lTodayScheme.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        lTodayScheme.DeviceID = "x";
                        lTodayScheme.DeviceType = "x";

                        // db.Entry(lTodayScheme).State = EntityState.Modified;
                        db.SaveChanges();
                        //db.TodaySchemes.Add(lTodayScheme);
                        //db.SaveChanges();

                        ts.Complete();
                        //db.Schemes.Add(scheme);
                        //db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    //db.Entry(scheme).State = EntityState.Modified;
                    //db.SaveChanges();
                    //return RedirectToAction("Index");
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Scheme Edit page !!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SchemeController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Scheme Edit page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SchemeController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
           
            return RedirectToAction("Index");  
        }

        // GET: /Scheme/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<SchemeType> lSchemeTypes = db.SchemeTypes.ToList();
            List<TodayScheme> lTodaySchemes = db.TodaySchemes.ToList();

            Scheme lSchemes = (from st in lSchemeTypes
                               join ts in lTodaySchemes on st.ID equals ts.SchemeTypeID
                               where st.ID == id && st.IsActive == true
                               select new ModelLayer.Models.ViewModel.Scheme
                               {
                                   Name = st.Name,
                                   ValueInRs = st.ValueInRs,
                                   ApplicableOnPurchaseOfRs = st.ApplicableOnPurchaseOfRs,
                                   SchemeCode = st.SchemeCode,
                                   OwnerName = st.PersonalDetail.FirstName,
                                   BussinessTypeName = st.BusinessType.Name,
                                   OwnerId = st.OwnerId,
                                   BusinessTypeID = (int)st.BussinessTypeID,
                                   IsActive = st.IsActive,
                                   StartDatetime = ts.StartDatetime,
                                   EndDatetime = ts.EndDatetime,
                                   CreateDate = st.CreateDate

                               }).FirstOrDefault();


           // Scheme scheme = db.Schemes.Find(id);
            if (lSchemes == null)
            {
                return HttpNotFound();
            }
            return View(lSchemes);
        }

        // POST: /Scheme/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            TodayScheme todayScheme = db.TodaySchemes.Where(x => x.SchemeTypeID == id).FirstOrDefault(); //db.TodaySchemes.Find(id);
            db.TodaySchemes.Remove(todayScheme);
            db.SaveChanges();


           // SchemaType lSchemaType = db.SchemeTypes.Find(id); 
            SchemeType scheme = db.SchemeTypes.Find(id);
            db.SchemeTypes.Remove(scheme);
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

        private long GetPersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            long UserLoginID = Convert.ToInt32(Session["ID"]);
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

        private string GetSchemeCode()
        {
            string lOneTimePassword = string.Empty;
            try
            {
                Random random = new Random();
                lOneTimePassword = "GBSM" + random.Next(1000, 9999).ToString();
            }
            catch (Exception)
            {

                throw;
            }
            return lOneTimePassword;
        }

        public JsonResult GetCouponOwners(int bussinessTypeID)
        {
            //List<CategoryList> lcategory = new List<CategoryList>();
            //lcategory = (from n in db.Categories
            //             where n.ParentCategoryID == 15 && n.Level == 2
            //             select new CategoryList
            //             {
            //                 ID = n.ID,
            //                 name = n.Name
            //             }).OrderBy(x => x.name).ToList();

            List<SchemeUsers> lSchemeUsers = new List<SchemeUsers>();

            lSchemeUsers = (from BD in db.BusinessDetails
                            join UL in db.UserLogins on BD.UserLoginID equals UL.ID
                            where BD.BusinessTypeID == bussinessTypeID
                            select new SchemeUsers
                            {
                                OwnerID = UL.ID,
                                UserName = BD.Name
                            }).OrderBy(x => x.UserName).ToList();

            //var queryResult = (
            //        from UL in db.UserLogins
            //        join PD in db.PersonalDetails on UL.ID equals PD.UserLoginID
            //        join UR in db.UserRoles on UL.ID equals UR.UserLoginID
            //        join BD in db.BusinessDetails on UL.ID equals BD.UserLoginID into t
            //        from BD in t.DefaultIfEmpty()
            //        where PD.IsActive == true && UR.IsActive == true && UR.RoleID == 2 && BD.UserLoginID == null
            //        && (PD.FirstName.Contains(searchString) || UL.Mobile.Contains(searchString) || UL.Email.Contains(searchString))
            //        select new
            //        {
            //            ID = UL.ID,
            //            Name = PD.FirstName + "" + PD.MiddleName + "" + PD.LastName,
            //            PincodeID = PD.PincodeID,
            //            mobile = UL.Mobile,
            //            email = UL.Email,
            //            Islock = UL.IsLocked
            //        }).ToList();



            return Json(lSchemeUsers, JsonRequestBehavior.AllowGet);

        }
    }
}
