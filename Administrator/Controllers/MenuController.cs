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
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace  Administrator.Controllers
{
    [SessionExpire]
    public class MenuController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /Menu/
        [CustomAuthorize(Roles = "Menu/CanRead")]
        public ActionResult Index(int? ParentID)
        {
            try
            {
                ViewBag.ParentID = new SelectList(db.Menus, "ID", "Name", "Parent Menu");
                List<Menu> menus = new List<Menu>();
                if (ParentID > 0)
                    menus = db.Menus.Where(m => m.ParentID == ParentID).ToList();
                else
                    menus = db.Menus.ToList();
                return View(menus.OrderBy(x => x.Name).ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MenuController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MenuController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /Menu/Details/5
        [CustomAuthorize(Roles = "Menu/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Menu menu = db.Menus.Find(id);
                if (menu == null)
                {
                    return HttpNotFound();
                }
                return View(menu);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MenuController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MenuController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /Menu/Create
        [CustomAuthorize(Roles = "Menu/CanRead")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.ParentID = new SelectList(db.Menus.OrderBy(x => x.Name), "ID", "Name", "Parent Menu");
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MenuController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MenuController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /Menu/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize(Roles = "Menu/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,DisplayName,IsActive")] Menu menu, int? ParentID)
        {
            try
            {
                menu.ParentID = ParentID;
                menu.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                menu.CreateDate = DateTime.UtcNow.AddHours(5.30);
                menu.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                menu.DeviceID = string.Empty;
                menu.DeviceType = string.Empty;
                
                if (ModelState.IsValid)
                {
                    db.Menus.Add(menu);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Menu Inserted Successfully";
                }

                ViewBag.ParentID = new SelectList(db.Menus.OrderBy(x => x.Name), "ID", "Name", menu.ParentID);
                return View(menu);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MenuController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Insert Basic Role Detail ";
                return View(menu);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MenuController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Insert Basic Role Detail ";
                return View(menu);
            }
            //return View();
        }

        // GET: /Menu/Edit/5
        [CustomAuthorize(Roles = "Menu/CanRead")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Menu menu = db.Menus.Find(id);
                if (menu == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ParentID = new SelectList(db.Menus.OrderBy(x => x.Name), "ID", "Name", menu.ParentID);
                return View(menu);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MenuController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MenuController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /Menu/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize(Roles = "Menu/CanWrite")]        
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,DisplayName,ParentID,IsActive")] Menu menu, int? ParentID)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    Menu lData = db.Menus.Single(x => x.ID == menu.ID);

                    //Log Table Insertion
                    /*LogTable logTable = new LogTable();
                    logTable.TableName = "Menu";//table Name(Model Name)
                    logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(lData);
                    logTable.TableRowID = lData.ID;
                    logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                    long? rowOwnerID = (lData.ModifyBy >= 0 ? lData.ModifyBy : lData.CreateBy);
                    logTable.RowOwnerID = (long)rowOwnerID;
                    logTable.CreateDate = DateTime.UtcNow;
                    logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                    db.LogTables.Add(logTable);*/
                    /**************************************/

                    menu.ParentID = ParentID;
                    menu.CreateBy = lData.CreateBy;
                    menu.CreateDate = lData.CreateDate;
                    menu.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    menu.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                    menu.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    menu.DeviceID = string.Empty;
                    menu.DeviceType = string.Empty;

                    if (ModelState.IsValid)
                    {
                        db.Entry(lData).CurrentValues.SetValues(menu);
                        //db.Entry(menu).State = EntityState.Modified;
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        //return RedirectToAction("Index");
                        ViewBag.Messaage = "Menu Updated Successfully";
                    }
                    ViewBag.ParentID = new SelectList(db.Menus.OrderBy(x => x.Name), "ID", "Name", menu.ParentID);
                    return View(menu);
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[MenuController][POST:Edit]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                    ViewBag.Messaage = "Unable to Update menu Detail ";
                    return View(menu);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[MenuController][POST:Edit]",
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                    ViewBag.Messaage = "Unable to Update menu Detail ";
                    return View(menu);
                }
            }
            //return View();
        }

        // GET: /Menu/Delete/5
        [CustomAuthorize(Roles = "Menu/CanRead")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Menu menu = db.Menus.Find(id);
                if (menu == null)
                {
                    return HttpNotFound();
                }
                return View(menu);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MenuController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Delete menu Detail ";
                return View(db.Menus.Where(x => x.ID == id).FirstOrDefault());
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MenuController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Delete menu Detail ";
                return View(db.Menus.Where(x => x.ID == id).FirstOrDefault());
            }
            //return View();
        }

        // POST: /Menu/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorize(Roles = "Menu/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    Menu menu = db.Menus.Find(id);

                    //Log Table Insertion
                    //LogTable logTable = new LogTable();
                    //logTable.TableName = "Menu";//table Name(Model Name)
                    //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(menu);
                    //logTable.TableRowID = menu.ID;
                    //logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                    //long? rowOwnerID = (menu.ModifyBy >= 0 ? menu.ModifyBy : menu.CreateBy);
                    //logTable.RowOwnerID = (long)rowOwnerID;
                    //logTable.CreateDate = DateTime.UtcNow;
                    //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                    //db.LogTables.Add(logTable);
                    /**************************************/

                    db.Menus.Remove(menu);
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[MenuController][POST:Delete]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                    ViewBag.Messaage = "Unable to Delete Menu";
                    return View(db.Roles.Where(x => x.ID == id).FirstOrDefault());
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[MenuController][POST:Delete]",
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                    ViewBag.Messaage = "Unable to Delete Menu";
                    return View(db.Roles.Where(x => x.ID == id).FirstOrDefault()); 
                }
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
