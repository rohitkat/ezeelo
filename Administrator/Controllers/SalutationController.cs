//-----------------------------------------------------------------------
// <copyright file="ProductList.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Pradnyakar N. Badge</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using PagedList;
using PagedList.Mvc;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Text;
using BusinessLogicLayer;
using Administrator.Models;

namespace  Administrator.Controllers
{
    [SessionExpire]
    public class SalutationController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
     Environment.NewLine
     + "ErrorLog Controller : SalutationController" + Environment.NewLine);

        // GET: /Salutation/
        [CustomAuthorize(Roles = "Salutation/CanRead")]
        public ActionResult Index(int? page)
        {
            try
            {
                var salutation = db.Salutations.ToList();
                return View(salutation.ToList().ToPagedList(page ?? 1, 10));

                //return View(db.Salutations.ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Index view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SalutationController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Generate Index view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SalutationController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // GET: /Salutation/Details/5
        [CustomAuthorize(Roles = "Salutation/CanRead")]
        public ActionResult Details(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Salutation salutation = db.Salutations.Find(id);
                if (salutation == null)
                {
                    return HttpNotFound();
                }
                return View(salutation);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Detail!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SalutationController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Detail!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SalutationController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
           
        }

        // GET: /Salutation/Create
        [CustomAuthorize(Roles = "Salutation/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create View!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SalutationController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
              
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SalutationController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            
        }

        // POST: /Salutation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Salutation/CanWrite")]
        public ActionResult Create([Bind(Include="ID,Name,IsActive")] Salutation salutation)
        {
            try
            {

                if (db.Salutations.Where(x => x.Name == salutation.Name).Count() > 0)
                {
                    ViewBag.ErrorMessage = "Can not Insert Because Salutation Name Already Exists..!";
                    return View(salutation);
                }

                salutation.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                salutation.CreateDate = DateTime.UtcNow.AddHours(5.5);
                if (ModelState.IsValid)
                {
                    db.Salutations.Add(salutation);
                    db.SaveChanges();
                    ViewBag.ErrorMessage = "salutation Updated successfully";
                    return View(salutation);
                }

                return View(salutation);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SalutationController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Insert Salutations Detail ";
                return View(salutation);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SalutationController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Insert Salutations Detail ";
                return View(salutation);
            }
            
        }

        // GET: /Salutation/Edit/5
        [CustomAuthorize(Roles = "Salutation/CanWrite")]
        public ActionResult Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Salutation salutation = db.Salutations.Find(id);
                if (salutation == null)
                {
                    return HttpNotFound();
                }
                return View(salutation);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SalutationController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SalutationController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            
        }

        // POST: /Salutation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Salutation/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,Name,IsActive")] Salutation salutation)
        {
            try
            {
                if (db.Salutations.Where(x => x.Name == salutation.Name && x.ID != salutation.ID).Count() > 0)
                {
                    ViewBag.ErrorMessage = "Can not Insert Because Salutation Name Already Exists..!";
                    return View(salutation);
                }

                Salutation lSalutationDB = db.Salutations.Single(x => x.ID == salutation.ID);

                salutation.CreateBy = lSalutationDB.CreateBy;
                salutation.CreateDate = lSalutationDB.CreateDate;
                salutation.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                salutation.ModifyDate = DateTime.UtcNow.AddHours(5.5);

                TryUpdateModel(salutation);

                if (ModelState.IsValid)
                {
                    db.Entry(lSalutationDB).CurrentValues.SetValues(salutation);
                    //db.Entry(salutation).State = EntityState.Modified;
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.ErrorMessage = "salutation Updated successfully";
                    return View(salutation);
                }
                return View(salutation);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SalutationController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Update Salutations Detail ";
                return View(salutation);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SalutationController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Update Salutations Detail ";
                return View(salutation);
            }
            
        }

        // GET: /Salutation/Delete/5
        [CustomAuthorize(Roles = "Salutation/CanDelete")]
        public ActionResult Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Salutation salutation = db.Salutations.Find(id);
                if (salutation == null)
                {
                    return HttpNotFound();
                }
                return View(salutation);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SalutationController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SalutationController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
                    
        }

        // POST: /Salutation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Salutation/CanDelete")]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                Salutation salutation = db.Salutations.Find(id);
                db.Salutations.Remove(salutation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SalutationController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Deleted Salutations Detail ";
                return View(db.Salutations.Where(x => x.ID == id).FirstOrDefault());
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SalutationController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Deleted Salutations Detail ";
                return View(db.Salutations.Where(x => x.ID == id).FirstOrDefault());
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

        public class EmployeeManagement
        {
            public long ID { get; set; }
            public long UserLoginID { get; set; }
            public string EmployeeCode { get; set; }
            public Nullable<long> OwnerID { get; set; }
            public int SalutationID { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public Nullable<System.DateTime> DOB { get; set; }
            public string Gender { get; set; }
            public Nullable<int> PincodeID { get; set; }
            public string Address { get; set; }
            public string AlternateMobile { get; set; }
            public string AlternateEmail { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public bool IsLocked { get; set; }
            public bool IsActive { get; set; }


        }
    }
}
