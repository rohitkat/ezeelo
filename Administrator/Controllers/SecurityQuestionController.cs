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
using System.Web.Mvc;
using ModelLayer.Models;
using System.Text;
using BusinessLogicLayer;
using Administrator.Models;

namespace  Administrator.Controllers
{
    [SessionExpire]
    public class SecurityQuestionController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
        Environment.NewLine
        + "ErrorLog Controller : SecurityQuestionController" + Environment.NewLine);
        
        // GET: /SecurityQuestion/
        [CustomAuthorize(Roles = "SecurityQuestion/CanRead")]
        public ActionResult Index()
        {
            try
            {
                return View(db.SecurityQuestions.ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Index view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SecurityQuestionController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Generate Index view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SecurityQuestionController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
           
        }

        // GET: /SecurityQuestion/Details/5
        [CustomAuthorize(Roles = "SecurityQuestion/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                SecurityQuestion securityquestion = db.SecurityQuestions.Find(id);
                if (securityquestion == null)
                {
                    return HttpNotFound();
                }
                return View(securityquestion);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Detail!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SecurityQuestionController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Detail!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SecurityQuestionController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            
        }

        // GET: /SecurityQuestion/Create
        [CustomAuthorize(Roles = "SecurityQuestion/CanWrite")]
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
                    + "[SecurityQuestionController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SecurityQuestionController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
           
        }

        // POST: /SecurityQuestion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "SecurityQuestion/CanWrite")]
        public ActionResult Create([Bind(Include = "ID,Question,IsActive")] SecurityQuestion securityquestion)
        {
            try
            {
                if (db.SecurityQuestions.Where(x => x.Question == securityquestion.Question).Count() > 0)
                {
                    ViewBag.lblMessage = "Question is already exist...!!";
                    return View(securityquestion);
                }

                securityquestion.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                securityquestion.CreateDate = DateTime.UtcNow.AddHours(5.5);

                if (ModelState.IsValid)
                {
                    db.SecurityQuestions.Add(securityquestion);
                    db.SaveChanges();
                    //return RedirectToAction("Index");

                    ViewBag.Messaage = "security question Inserted successfully";
                    return View(securityquestion);
                }

                return View(securityquestion);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SecurityQuestionController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);


                ViewBag.Messaage = "Unable to Insert security question Detail ";
                return View(securityquestion);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Generate Create View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SecurityQuestionController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);


                ViewBag.Messaage = "Unable to Insert security question Detail ";
                return View(securityquestion);
            }

        }

        // GET: /SecurityQuestion/Edit/5
        [CustomAuthorize(Roles = "SecurityQuestion/CanWrite")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                SecurityQuestion securityquestion = db.SecurityQuestions.Find(id);
                if (securityquestion == null)
                {
                    return HttpNotFound();
                }
                return View(securityquestion);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SecurityQuestionController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SecurityQuestionController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            
        }

        // POST: /SecurityQuestion/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "SecurityQuestion/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,Question,IsActive")] SecurityQuestion securityquestion)
        {
            try
            {

                if (db.SecurityQuestions.Where(x => x.Question == securityquestion.Question && x.ID != securityquestion.ID).Count() > 0)
                {
                    ViewBag.lblMessage = "Question is already exist...!!";
                    return View(securityquestion);
                }

                SecurityQuestion lSecurityQuestionDB = db.SecurityQuestions.Single(x => x.ID == securityquestion.ID);

                securityquestion.CreateBy = lSecurityQuestionDB.CreateBy;
                securityquestion.CreateDate = lSecurityQuestionDB.CreateDate;
                securityquestion.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                securityquestion.ModifyDate = DateTime.UtcNow.AddHours(5.5);

                if (ModelState.IsValid)
                {
                    db.Entry(lSecurityQuestionDB).CurrentValues.SetValues(securityquestion);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "security question Updated successfully";
                    return View(securityquestion);
                }
                return View(securityquestion);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SecurityQuestionController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Update security question Detail ";
                return View(securityquestion);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SecurityQuestionController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Update security question Detail ";
                return View(securityquestion);
            }
            
        }

        // GET: /SecurityQuestion/Delete/5
        [CustomAuthorize(Roles = "SecurityQuestion/CanDelete")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                SecurityQuestion securityquestion = db.SecurityQuestions.Find(id);
                if (securityquestion == null)
                {
                    return HttpNotFound();
                }
                return View(securityquestion);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SecurityQuestionController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SecurityQuestionController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            
        }

        // POST: /SecurityQuestion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "SecurityQuestion/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                SecurityQuestion securityquestion = db.SecurityQuestions.Find(id);
                db.SecurityQuestions.Remove(securityquestion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SecurityQuestionController][POST:DeleteConfirmed]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Deleted Securityquestion Detail ";
                return View(db.SecurityQuestions.Where(x => x.ID == id).FirstOrDefault());
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SecurityQuestionController][POST:DeleteConfirmed]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Deleted Securityquestion Detail ";
                return View(db.SecurityQuestions.Where(x => x.ID == id).FirstOrDefault());
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
