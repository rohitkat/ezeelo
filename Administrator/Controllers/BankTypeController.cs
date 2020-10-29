//-----------------------------------------------------------------------
// <copyright file="ProductList.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Pradnyakar N. Badge</author>
//-----------------------------------------------------------------------

using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusinessLogicLayer;
using System.Text;
using Administrator.Models;
namespace  Administrator.Controllers
{

    /// <summary>
    /// Developed By :- Pradnyakar Badge
    /// To Create Bank Account Type Name as Master Entry
    /// </summary>

    [SessionExpire]
    public class BankTypeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
          Environment.NewLine
          + "ErrorLog Controller : BankTypeController" + Environment.NewLine);

        //
        // GET: /BankType/
        [SessionExpire]
        [CustomAuthorize(Roles = "BankType/CanRead")]
        public ActionResult Index()
        {
            try
            {
                return View(db.BankAccountTypes.OrderBy(x => x.Name).ToList());
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

        //
        // GET: /BankType/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "BankType/CanRead")]
        public ActionResult Details(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BankAccountType bankAcounttype = db.BankAccountTypes.Find(id);
                if (bankAcounttype == null)
                {
                    return HttpNotFound();
                }
                return View(bankAcounttype);
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

        //
        // GET: /BankType/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "BankType/CanWrite")]
        public ActionResult Create()
        {
            try
            {
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

        //
        // POST: /BankType/Create
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "BankType/CanWrite")]
        public ActionResult Create(BankAccountType collection)
        {
            try
            {
                if (db.BankAccountTypes.Where(x => x.Name == collection.Name).Count() > 0)
                {
                    ViewBag.Message = "Bank Account Type Already exists..!!";
                    return View(collection);
                }


                //collection.ID = null;
                collection.CreateDate = DateTime.UtcNow.AddHours(5.30);
                collection.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                collection.DeviceID = string.Empty;
                collection.DeviceType = string.Empty;
                collection.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                List<Role> lData = db.Roles.ToList();
                ViewBag.RoleList = new SelectList(lData, "ID", "Name");
                if (ModelState.IsValid)
                {
                    db.BankAccountTypes.Add(collection);
                    db.SaveChanges();
                    ViewBag.Messaage = "Bank Account Type Detail Created Successfully";
                }

                return View(collection);
                //return RedirectToAction("Index");
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
                ViewBag.Messaage = "Unable to Insert Bank Account Type Detail";
                return View(collection);
            }
        }

        //
        // GET: /BankType/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "BankType/CanWrite")]
        public ActionResult Edit(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BankAccountType lData = db.BankAccountTypes.Find(id);
                if (lData == null)
                {
                    return HttpNotFound();
                }
                return View(lData);
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

        //
        // POST: /BankType/Edit/5
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "BankType/CanWrite")]
        public ActionResult Edit(BankAccountType collection)
        {
            try
            {
                if (db.BankAccountTypes.Where(x => x.Name == collection.Name && x.ID != collection.ID).Count() > 0)
                {
                    ViewBag.Message = "Bank Account Type Already exists..!!";
                    return View(collection);
                }

                BankAccountType lData = db.BankAccountTypes.Single(x => x.ID == collection.ID);

                collection.CreateDate = lData.CreateDate;
                collection.CreateBy = lData.CreateBy;
                collection.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                collection.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                collection.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                collection.DeviceType = string.Empty;
                collection.DeviceID = string.Empty;
                if (ModelState.IsValid)
                {
                    db.Entry(lData).CurrentValues.SetValues(collection);
                    db.SaveChanges();
                    ViewBag.Messaage = "Bank Account Type Detail Modified Successfully";

                }
                return View(collection);
                //return RedirectToAction("Index");
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

                ViewBag.Messaage = "Unable to Update Bank Account Type Detail";
                return View(collection);
            }
        }

        //
        // GET: /BankType/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "BankType/CanDelete")]
        public ActionResult Delete(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BankAccountType collection = db.BankAccountTypes.Find(id);

                if (collection == null)
                {
                    return HttpNotFound();
                }
                return View(collection);
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

        //
        // POST: /BankType/Delete/5
        [HttpPost, ActionName("Delete")]
        [SessionExpire]
        [CustomAuthorize(Roles = "BankType/CanDelete")]
        public ActionResult ConformDelete(int id)
        {
            try
            {
                BankAccountType role = db.BankAccountTypes.Find(id);
                db.BankAccountTypes.Remove(role);
                db.SaveChanges();
                ViewBag.Messaage = "Bank Account Type Detail Modified Successfully";
                return View();
                //return RedirectToAction("Index");


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

                ViewBag.Messaage = "Unable to Delete Bank Account Type Detail";
                return View(db.BankAccountTypes.Where(x => x.ID == id).FirstOrDefault());
            }
        }
    }
}
