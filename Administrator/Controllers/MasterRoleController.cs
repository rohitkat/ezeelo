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

namespace Administrator.Controllers
{
    [SessionExpire]
    public class MasterRoleController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /MasterRole/

        public ActionResult Index()
        {
            try
            {
                return View(db.Roles.OrderBy(x => x.Name).ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterRoleController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterRoleController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /MasterRole/Details/5
        [CustomAuthorize(Roles = "MasterRole/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Role role = db.Roles.Find(id);
                List<Role> lData = db.Roles.ToList();
                ViewBag.RoleList = new SelectList(lData.OrderBy(x => x.Name), "ID", "RoleName", role.ParentID);
                if (role == null)
                {
                    return HttpNotFound();
                }
                return View(role);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterRoleController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterRoleController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /MasterRole/Create
        [CustomAuthorize(Roles = "MasterRole/CanRead")]
        public ActionResult Create()
        {
            try
            {
                List<Role> lData = db.Roles.ToList();
                ViewBag.RoleList = new SelectList(lData.OrderBy(x => x.Name), "ID", "Name");
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterRoleController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterRoleController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /MasterRole/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize(Roles = "MasterRole/CanRead")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,ParentID,Level,IsActive")] Role role, int? RoleList)
        {
            try
            {
                if (db.Roles.Where(x => x.Name == role.Name).Count() > 0)
                {
                    ViewBag.Messaage = "Can not save because Basic Role already exists...!";
                    return View(role);
                }

                role.ParentID = RoleList;
                role.CreateDate = DateTime.UtcNow.AddHours(5.30);
                role.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                role.DeviceID = string.Empty;
                role.DeviceType = string.Empty;
                role.NetworkIP = string.Empty;
                List<Role> lData = db.Roles.ToList();
                ViewBag.RoleList = new SelectList(lData.OrderBy(x => x.Name), "ID", "Name");
                if (ModelState.IsValid)
                {
                    db.Roles.Add(role);
                    db.SaveChanges();
                    // return RedirectToAction("Index");
                    ViewBag.Messaage = "Basic Role Inserted Successfully";
                }

                return View(role);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterRoleController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                
                ViewBag.Messaage = "Unable to Insert Basic Role Detail ";
                return View(role);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterRoleController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                
                ViewBag.Messaage = "Unable to Insert Basic Role Detail ";
                return View(role);
            }
            //return View();
        }

        // GET: /MasterRole/Edit/5
        [CustomAuthorize(Roles = "MasterRole/CanRead")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Role role = db.Roles.Find(id);
                List<Role> lData = db.Roles.ToList();
                ViewBag.RoleList = new SelectList(lData.OrderBy(x => x.Name), "ID", "Name", role.ParentID);
                if (role == null)
                {
                    return HttpNotFound();
                }
                return View(role);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterRoleController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterRoleController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /MasterRole/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [CustomAuthorize(Roles = "MasterRole/CanRead")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,ParentID,Level,IsActive")] Role role, int? RoleList)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    ViewBag.RoleList = new SelectList(db.Roles.OrderBy(x => x.Name).ToList(), "ID", "Name", RoleList);

                    Role lData = db.Roles.Single(x => x.ID == role.ID);
                    if (db.Roles.Where(x => x.Name == role.Name && x.ID != role.ID).Count() > 0)
                    {
                        ViewBag.Messaage = "Can not save because Basic Role already exists...!";
                        return View(role);
                    }
                    //Log Table Insertion
                    //LogTable logTable = new LogTable();
                    //logTable.TableName = "Role";//table Name(Model Name)
                    //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(lData);
                    //logTable.TableRowID = lData.ID;
                    //logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                    //long? rowOwnerID = (lData.ModifyBy >= 0 ? lData.ModifyBy : lData.CreateBy);
                    //logTable.RowOwnerID = (long)rowOwnerID;
                    //logTable.CreateDate = DateTime.UtcNow;
                    //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                    //db.LogTables.Add(logTable);
                    /**************************************/
                    role.ParentID = RoleList;
                    role.CreateDate = lData.CreateDate;
                    role.CreateBy = lData.CreateBy;
                    role.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    role.ModifyDate = DateTime.UtcNow.AddHours(5.30);

                    if (ModelState.IsValid)
                    {
                        db.Entry(lData).CurrentValues.SetValues(role);
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        //return RedirectToAction("Index");
                        ViewBag.Messaage = "Basic Role Updated Successfully";
                    }


                    return View(role);
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[MasterRoleController][POST:Edit]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                    ViewBag.Messaage = "Unable to Updated Basic Role";
                    return View(role);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[MasterRoleController][POST:Edit]",
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                    ViewBag.Messaage = "Unable to Updated Basic Role" ;
                }
            }
            return View();
        }

        // GET: /MasterRole/Delete/5
        [CustomAuthorize(Roles = "MasterRole/CanRead")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Role role = db.Roles.Find(id);
                List<Role> lData = db.Roles.ToList();
                ViewBag.RoleList = new SelectList(lData.OrderBy(x => x.Name), "ID", "RoleName", role.ParentID);
                if (role == null)
                {
                    return HttpNotFound();
                }
                return View(role);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterRoleController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterRoleController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /MasterRole/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorize(Roles = "MasterRole/CanRead")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    Role role = db.Roles.Find(id);

                    //Log Table Insertion
                    //LogTable logTable = new LogTable();
                    //logTable.TableName = "Role";//table Name(Model Name)
                    //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(role);
                    //logTable.TableRowID = role.ID;
                    //logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                    //long? rowOwnerID = (role.ModifyBy >= 0 ? role.ModifyBy : role.CreateBy);
                    //logTable.RowOwnerID = (long)rowOwnerID;
                    //logTable.CreateDate = DateTime.UtcNow;
                    //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                    //db.LogTables.Add(logTable);
                    /**************************************/

                    db.Roles.Remove(role);
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[MasterRoleController][POST:Delete]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                    ViewBag.Messaage = "Unable to Delete Basic Role";
                    return View(db.Roles.Where(x => x.ID == id).FirstOrDefault());
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[MasterRoleController][POST:Delete]",
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                    ViewBag.Messaage = "Unable to Delete Basic Role";
                    return View(db.Roles.Where(x => x.ID == id).FirstOrDefault());
                }
            }
            //return View(db.Roles.Where(x => x.ID == id).FirstOrDefault());
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
