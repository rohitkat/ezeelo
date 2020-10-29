//-----------------------------------------------------------------------
// <copyright file="EmployeeApprovalController.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
//
// </copyright>
// <author>Snehal Shende</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using BusinessLogicLayer;
using System.Transactions;
using System.Net;
using System.Data.Entity;
using Administrator.Models;

namespace Administrator.Controllers
{
    /// <summary>
    /// Developed By:- Pradnyakar Badge
    /// Purpose :- To approve the created employee under any business unit like
    /// Delivery Parter, Franchis, Shop, CRM, GrandhibaghAdmin, Advertiser etc.
    /// and allow these employee to login into respective web portal
    /// </summary>
    public class EmployeeApprovalController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        //
        // GET: /EmployeeApproval/
        [SessionExpire]
        [CustomAuthorize(Roles = "EmployeeApproval/CanRead")]
        public ActionResult Index()
        {
            try
            {
                ViewBag.BusinessType = new SelectList(db.BusinessTypes, "ID", "Name");
                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.BusinessOwner = new SelectList(lData, "Value", "Text");
                List<Employee> lemp = new List<Employee>();
                lemp = db.Employees.Where(x => x.UserLogin.IsLocked == true).ToList();
                return View(lemp);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[EmployeeApprovalController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[EmployeeApprovalController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        //
        // GET: /EmployeeApproval/Details/5

        /// <summary>
        /// to View the detail of Employee
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns></returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "EmployeeApproval/CanRead")]
        public ActionResult Details(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                // EmployeeManagement employeemanagement = db.EmployeeManagements.Find(id);
                EmployeeManagement lEmployeeManagement = new EmployeeManagement();
                //to get list of employee detail
                lEmployeeManagement = FillEmployeeDetail(id);
                if (lEmployeeManagement == null)
                {
                    return HttpNotFound();
                }
                return View(lEmployeeManagement);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[EmployeeApprovalController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[EmployeeApprovalController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        //
        // GET: /EmployeeApproval/Edit/5

        /// <summary>
        /// Modify the existing employee detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "EmployeeApproval/CanRead")]
        public ActionResult Edit(int id)
        {
            try
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                UserLogin lUserLogin = db.UserLogins.Find(id);
                if (lUserLogin == null)
                {
                    return HttpNotFound();
                }

                EmployeeManagement lEmployeeManagement = new EmployeeManagement();
                lEmployeeManagement = FillEmployeeDetail(id);

                ViewBag.Salutation = new SelectList(db.Salutations, "ID", "Name", lEmployeeManagement.SalutationID);
                ViewBag.Gender = new SelectList(this.FillGender(), "Value", "Text", lEmployeeManagement.Gender);


                BusinessType lBusinessType = new BusinessType();
                lBusinessType = db.BusinessTypes.Where(x => x.Prefix == lEmployeeManagement.EmployeeCode.Substring(0, 4)).FirstOrDefault();

                //ViewBag.OwnerID = new SelectList(db.BusinessTypes.Where(x => x.Prefix == lEmployeeManagement.EmployeeCode.Substring(0, 3)).ToList(), "ID", "Name");

                List<OwnerDetailByPrefix> lData = new List<OwnerDetailByPrefix>();
                lData = FillOwnerID(lBusinessType.ID);
                ViewBag.BusinessOwner = new SelectList(lData, "ID", "Name", lEmployeeManagement.OwnerID);
                ViewBag.OwnerID = new SelectList(db.BusinessTypes, "ID", "Name", lBusinessType.ID);


                return View(lEmployeeManagement);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[EmployeeApprovalController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[EmployeeApprovalController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /EmployeeManagement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "EmployeeApproval/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserLoginID,EmployeeCode,OwnerID,SalutationID,FirstName,MiddleName,LastName,DOB,Gender,PincodeID,Address,AlternateMobile,AlternateEmail,Mobile,Email,Password,IsActive")] EmployeeManagement employeemanagement, int Salutation, int BusinessOwner)
        {
            try
            {
                ViewBag.Salutation = new SelectList(db.Salutations, "ID", "Name");
                ViewBag.OwnerID = new SelectList(db.BusinessTypes, "ID", "Name");
                ViewBag.Gender = new SelectList(this.FillGender(), "Value", "Text");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.BusinessOwner = new SelectList(lData, "Value", "Text");

                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        try
                        {

                            employeemanagement.SalutationID = Salutation;
                            employeemanagement.OwnerID = BusinessOwner;



                            Int64 ID = UpdateUserLogin(employeemanagement);

                            UpdatePersonalDetail(employeemanagement, ID);

                            UpdateEmployeeDetail(employeemanagement, ID);

                            ts.Complete();
                            //Model Data Dispose
                            ModelState.Clear();
                            //Successfull message
                            //ViewBag.Message = "Menu for Role Inserted Successfully!!";
                            ModelState.AddModelError("Message", "Employee Record Updated Successfully!!");
                            employeemanagement = null;
                            //return RedirectToAction("Index");
                        }
                        catch (BusinessLogicLayer.MyException myEx)
                        {
                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                + "[EmployeeApprovalController][GET:Index]" + myEx.EXCEPTION_PATH,
                                BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                        }
                        catch (Exception ex)
                        {
                            ts.Dispose();
                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + ex.Message + Environment.NewLine
                                + "[EmployeeApprovalController][GET:Index]",
                                BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                        }

                    }
                }

                return View(employeemanagement);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[EmployeeApprovalController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[EmployeeApprovalController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();

        }
        /// <summary>
        /// To Approve the newly created Employee
        /// </summary>
        /// <param name="id">EmployeeID</param>
        /// <returns></returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "EmployeeApproval/CanRead")]
        public ActionResult Approve(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                // EmployeeManagement employeemanagement = db.EmployeeManagements.Find(id);
                EmployeeManagement lEmployeeManagement = new EmployeeManagement();
                lEmployeeManagement = FillEmployeeDetail(id);
                if (lEmployeeManagement == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Roles = new SelectList(db.Roles, "ID", "Name");
                return View(lEmployeeManagement);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[EmployeeApprovalController][GET:Approve]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[EmployeeApprovalController][GET:Approve]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "EmployeeApproval/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult Approve([Bind(Include = "UserLoginID")] EmployeeManagement employeemanagement, int Role)
        {
            try
            {
                ViewBag.Roles = new SelectList(db.Roles, "ID", "Name", Role);
                ApproveEmployee(employeemanagement, Role);


                //Email n SMS to be sent from here 
                sendSMS(Convert.ToInt64(employeemanagement.UserLoginID));
                sendEmail(Convert.ToInt64(employeemanagement.UserLoginID));
                ModelState.Clear();
                //Successfull message
                ModelState.AddModelError("Message", "Employee Record Approved Successfully!!");
                //return RedirectToAction("Index");
                EmployeeManagement lEmployeeManagement = new EmployeeManagement();
                lEmployeeManagement = FillEmployeeDetail(Convert.ToInt64(employeemanagement.UserLoginID));
                return View(lEmployeeManagement);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[EmployeeApprovalController][POST:Approve]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[EmployeeApprovalController][POST:Approve]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();

        }

        /// <summary>
        /// Update Exisrting any Employee Detail
        /// </summary>
        /// <param name="employeemanagement">EmployeeDetail Class</param>
        /// <param name="ID">EmployeeID</param>
        private void UpdateEmployeeDetail(EmployeeManagement employeemanagement, long ID)
        {
            try
            {

                Employee lData = new Employee();
                lData = db.Employees.Where(x => x.UserLoginID == employeemanagement.UserLoginID).FirstOrDefault();

                //Log Table Insertion
                //LogTable logTable = new LogTable();
                //logTable.TableName = "Employee";//table Name(Model Name)
                //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(lData);
                //logTable.TableRowID = lData.ID;
                //logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                //long? rowOwnerID = (lData.ModifyBy >= 0 ? lData.ModifyBy : lData.CreateBy);
                //logTable.RowOwnerID = (long)rowOwnerID;
                //logTable.CreateDate = DateTime.UtcNow;
                //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                //db.LogTables.Add(logTable);
                /**************************************/

                Employee lEmployee = new Employee();
                lEmployee.ID = lData.ID;
                lEmployee.UserLoginID = employeemanagement.UserLoginID;
                lEmployee.EmployeeCode = employeemanagement.EmployeeCode;
                lEmployee.OwnerID = employeemanagement.OwnerID;
                lEmployee.IsActive = employeemanagement.IsActive;
                lEmployee.CreateBy = lData.CreateBy;
                lEmployee.CreateDate = lData.CreateDate;
                lEmployee.ModifyDate = DateTime.UtcNow;
                lEmployee.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                lEmployee.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                lEmployee.DeviceType = string.Empty;
                lEmployee.DeviceID = string.Empty;


                db.Entry(lData).CurrentValues.SetValues(lEmployee);
                //db.UserLogins.Add(lUserLogin);
                db.SaveChanges();


            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Insert Recored in Employee Detail :- " + ex.InnerException);

            }
        }

        /// <summary>
        /// Modify Existing Employee Personal Detail
        /// </summary>
        /// <param name="employeemanagement">EmployeeID</param>
        /// <param name="ID"></param>
        private void UpdatePersonalDetail(EmployeeManagement employeemanagement, long ID)
        {
            try
            {
                PersonalDetail lData = new PersonalDetail();
                lData = db.PersonalDetails.Where(x => x.UserLoginID == employeemanagement.UserLoginID).FirstOrDefault();

                //Log Table Insertion
                //LogTable logTable = new LogTable();
                //logTable.TableName = "PersonalDetail";//table Name(Model Name)
                //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(lData);
                //logTable.TableRowID = lData.ID;
                //logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                //long? rowOwnerID = (lData.ModifyBy >= 0 ? lData.ModifyBy : lData.CreateBy);
                //logTable.RowOwnerID = (long)rowOwnerID;
                //logTable.CreateDate = DateTime.UtcNow;
                //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                //db.LogTables.Add(logTable);
                /**************************************/

                PersonalDetail lPersonalDetail = new PersonalDetail();
                lPersonalDetail.ID = lData.ID;
                lPersonalDetail.UserLoginID = ID;
                lPersonalDetail.SalutationID = employeemanagement.SalutationID;
                lPersonalDetail.FirstName = employeemanagement.FirstName;
                lPersonalDetail.MiddleName = employeemanagement.MiddleName;
                lPersonalDetail.LastName = employeemanagement.LastName;
                lPersonalDetail.Gender = employeemanagement.Gender;
                lPersonalDetail.Address = employeemanagement.Address;
                lPersonalDetail.AlternateEmail = employeemanagement.AlternateEmail;
                lPersonalDetail.AlternateMobile = employeemanagement.AlternateMobile;
                lPersonalDetail.IsActive = employeemanagement.IsActive;

                lPersonalDetail.CreateBy = lData.CreateBy;
                lPersonalDetail.CreateDate = lData.CreateDate;
                lPersonalDetail.ModifyDate = DateTime.UtcNow;
                lPersonalDetail.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                lPersonalDetail.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                lPersonalDetail.DeviceType = string.Empty;
                lPersonalDetail.DeviceID = string.Empty;


                db.Entry(lData).CurrentValues.SetValues(lPersonalDetail);
                //db.UserLogins.Add(lUserLogin);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Insert Recored in Personal Detail :- " + ex.InnerException);

            }

        }

        /// <summary>
        /// Modify Existing Employee Login Detail
        /// </summary>
        /// <param name="employeemanagement"></param>
        /// <param name="ID">EmployeeID</param>
        private long UpdateUserLogin(EmployeeManagement employeemanagement)
        {
            try
            {

                var iteam = db.UserLogins.Where(x => x.Mobile == employeemanagement.Mobile && x.ID != employeemanagement.UserLoginID).FirstOrDefault();
                if (iteam != null)
                {
                    throw new Exception("Mobile No is already exists...!");
                }
                iteam = db.UserLogins.Where(x => x.Email == employeemanagement.Email && x.ID != employeemanagement.UserLoginID).FirstOrDefault();
                if (iteam != null)
                {
                    throw new Exception("Email ID is already exists...!");
                }

                UserLogin lData = new UserLogin();
                lData = db.UserLogins.Where(x => x.ID == employeemanagement.UserLoginID).FirstOrDefault();

                //Log Table Insertion
                //LogTable logTable = new LogTable();
                //logTable.TableName = "UserLogin";//table Name(Model Name)
                //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(lData);
                //logTable.TableRowID = lData.ID;
                //logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                //long? rowOwnerID = (lData.ModifyBy >= 0 ? lData.ModifyBy : lData.CreateBy);
                //logTable.RowOwnerID = (long)rowOwnerID;
                //logTable.CreateDate = DateTime.UtcNow;
                //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                //db.LogTables.Add(logTable);
                /**************************************/

                UserLogin lUserLogin = new UserLogin();
                lUserLogin.ID = lData.ID;
                lUserLogin.Mobile = employeemanagement.Mobile;
                lUserLogin.Email = employeemanagement.Email;
                lUserLogin.Password = employeemanagement.Password;
                lUserLogin.IsLocked = true;
                lUserLogin.CreateBy = lData.CreateBy;
                lUserLogin.CreateDate = lData.CreateDate;
                lUserLogin.ModifyDate = DateTime.UtcNow;
                lUserLogin.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); ;
                lUserLogin.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                lUserLogin.DeviceType = string.Empty;
                lUserLogin.DeviceID = string.Empty;


                db.Entry(lData).CurrentValues.SetValues(lUserLogin);
                //db.UserLogins.Add(lUserLogin);
                db.SaveChanges();

                return lUserLogin.ID;

            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Insert Recored in Login Detail :- " + ex.InnerException);

            }

        }
        /// <summary>
        /// Allocate Role to newly created record for login
        /// </summary>
        /// <param name="employeemanagement">EmployeeDetail</param>
        /// <param name="ID">EmployeeID</param>
        private void ApproveEmployee(EmployeeManagement employeemanagement, int Role)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    UserLogin lData = db.UserLogins.Where(x => x.ID == employeemanagement.UserLoginID).FirstOrDefault();
                    //Log Table Insertion
                    //LogTable logTable = new LogTable();
                    //logTable.TableName = "UserLogin";//table Name(Model Name)
                    //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(lData);
                    //logTable.TableRowID = lData.ID;
                    //logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                    //long? rowOwnerID = (lData.ModifyBy >= 0 ? lData.ModifyBy : lData.CreateBy);
                    //logTable.RowOwnerID = (long)rowOwnerID;
                    //logTable.CreateDate = DateTime.UtcNow;
                    //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                    //db.LogTables.Add(logTable);
                    /**************************************/

                    lData.IsLocked = false;
                    lData.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    lData.ModifyDate = DateTime.UtcNow;
                    //db.Entry(lData).State = EntityState.Modified;
                    db.SaveChanges();
                    InsertRole(lData.ID, Role);
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to Insert Recored in Login Detail :- " + ex.InnerException);

                }
            }
        }
        /// <summary>
        /// to retrive list of employee detail Like 
        /// Personal detail, Login Detail, Owner detail
        /// </summary>
        /// <param name="id">employee id</param>
        /// <returns>employee detail</returns>
        private EmployeeManagement FillEmployeeDetail(long? id)
        {
            try
            {
                UserLogin LoginDetail = new UserLogin();
                LoginDetail = db.UserLogins.Where(x => x.ID == id).FirstOrDefault();

                /*User Login Detail*/
                EmployeeManagement lemployee = new EmployeeManagement();

                lemployee.UserLoginID = Convert.ToInt64(id);
                lemployee.Mobile = LoginDetail.Mobile;
                lemployee.Email = LoginDetail.Email;
                lemployee.Password = LoginDetail.Password;
                lemployee.IsLocked = LoginDetail.IsLocked;

                /*Personal Detail*/
                PersonalDetail lPersonalDetail = new PersonalDetail();
                lPersonalDetail = db.PersonalDetails.Where(x => x.UserLoginID == id).FirstOrDefault();
                lemployee.SalutationID = lPersonalDetail.SalutationID;
                lemployee.FirstName = lPersonalDetail.FirstName;
                lemployee.MiddleName = lPersonalDetail.MiddleName;
                lemployee.LastName = lPersonalDetail.LastName;
                lemployee.Gender = lPersonalDetail.Gender;
                lemployee.Address = lPersonalDetail.Address;
                lemployee.AlternateEmail = lPersonalDetail.AlternateEmail;
                lemployee.AlternateMobile = lPersonalDetail.AlternateMobile;
                lemployee.IsActive = lPersonalDetail.IsActive;

                /*Employee Detail*/
                Employee lEmployee = new Employee();
                lEmployee = db.Employees.Where(x => x.UserLoginID == id).FirstOrDefault();
                lemployee.EmployeeCode = lEmployee.EmployeeCode;
                lemployee.OwnerID = lEmployee.OwnerID;

                return lemployee;



            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Fill EmployeeDetail :- " + ex.InnerException.ToString());
            }

        }
        /// <summary>
        /// to Fill gender dropdown
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> FillGender()
        {
            List<SelectListItem> lData = new List<SelectListItem>();
            try
            {
                lData.Add(new SelectListItem { Text = "Male", Value = "Male" });
                lData.Add(new SelectListItem { Text = "Female", Value = "Female" });
                lData.Add(new SelectListItem { Text = "Transgender", Value = "Transgender" });
                return (lData);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Fill Gender :- " + ex.InnerException.ToString());
            }
            return (lData);
        }
        /// <summary>
        /// to fill all owner detial in drop down like name and id
        /// </summary>
        /// <param name="businessTypeID">Business Detail Type ID Like CRM ID,ShopID, Franchise ID</param>
        /// <returns></returns>
        public List<OwnerDetailByPrefix> FillOwnerID(int? businessTypeID)
        {
            List<OwnerDetailByPrefix> lownerType = new List<OwnerDetailByPrefix>();
            try
            {
                OwnerDetailByPrefix objODP = new OwnerDetailByPrefix();

                lownerType = objODP.OwnerDetail(businessTypeID, System.Web.HttpContext.Current.Server);
                return lownerType;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Fill OwnerID :- " + ex.InnerException.ToString());
            }
        }
        /// <summary>
        /// call by ajax method
        /// to retrive list of employee 
        /// </summary>
        /// <param name="businessTypeID">Business Type ID</param>
        /// <param name="OwnerID">Owner ID</param>
        /// <returns>Employee List</returns>
        public JsonResult EmployeeList(int businessTypeID, int OwnerID)
        {
            List<EmployeeDetail> lst = new List<EmployeeDetail>();
            lst = GetEmployeeList(businessTypeID, OwnerID);

            return Json(lst, JsonRequestBehavior.AllowGet);

        }

        public class EmployeeDetail
        {
            public string Mobile { get; set; }
            public string Email { get; set; }
            public string EmployeeCode { get; set; }
            public string EmpName { get; set; }
            public Int64 OwnerID { get; set; }
            public Int64 LoginID { get; set; }
            public bool IsActive { get; set; }
        }
        public List<EmployeeDetail> GetEmployeeList(int businessTypeID, int OwnerID)
        {
            List<EmployeeDetail> empLst = new List<EmployeeDetail>();

            if (businessTypeID < 1 || OwnerID < 1)
            {
                empLst = (from e in db.Employees
                          join p in db.PersonalDetails on e.UserLoginID equals p.UserLoginID
                          where e.UserLogin.IsLocked == true
                          select new EmployeeDetail
                          {
                              EmployeeCode = e.EmployeeCode,
                              EmpName = p.Salutation.Name + " " + p.FirstName + " " + p.LastName,
                              Email = e.UserLogin.Email,
                              Mobile = e.UserLogin.Mobile,
                              OwnerID = (Int64)e.OwnerID,
                              LoginID = e.UserLoginID,
                              IsActive = e.IsActive
                          }).ToList();
            }
            else
            {
                string prefix = db.BusinessTypes.Where(x => x.ID == businessTypeID).FirstOrDefault().Prefix;

                empLst = (from e in db.Employees
                          join p in db.PersonalDetails on e.UserLoginID equals p.UserLoginID
                          where e.UserLogin.IsLocked == true && e.EmployeeCode.StartsWith(prefix) && e.OwnerID==OwnerID
                          select new EmployeeDetail
                          {
                              EmployeeCode = e.EmployeeCode,
                              EmpName = p.Salutation.Name + " " + p.FirstName + " " + p.LastName,
                              Email = e.UserLogin.Email,
                              Mobile = e.UserLogin.Mobile,
                              OwnerID = (Int64)e.OwnerID,
                              LoginID = e.UserLoginID,
                              IsActive = e.IsActive
                          }).ToList();
            }
            return empLst;
        }

        private void InsertRole(long userLoginID, int Role)
        {
            try
            {
                UserRole lUserRole = db.UserRoles.Where(x => x.UserLoginID == userLoginID).FirstOrDefault();
                if (lUserRole == null)
                {
                    lUserRole = new UserRole();
                    lUserRole.UserLoginID = userLoginID;
                    lUserRole.RoleID = Role;
                    lUserRole.IsActive = true;
                    lUserRole.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    lUserRole.CreateDate = DateTime.Now;
                    lUserRole.NetworkIP = CommonFunctions.GetClientIP();
                    //if (ModelState.IsValid)
                    {
                        db.UserRoles.Add(lUserRole);
                        db.SaveChanges();
                    }
                }
                else if (lUserRole.RoleID != Role)
                {
                    lUserRole.RoleID = Role;
                    lUserRole.UserLoginID = userLoginID;
                    lUserRole.IsActive = true;
                    lUserRole.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    lUserRole.ModifyDate = DateTime.Now;
                    lUserRole.NetworkIP = CommonFunctions.GetClientIP();
                    //if (ModelState.IsValid)
                    {
                        db.SaveChanges();
                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[InsertRole]", "Can't assign Role..!" + Environment.NewLine + myEx.Message);
            }
        }

        //private string GetRoleName(long userLoginID)
        //{
        //    string roleName = string.Empty;
        //    try
        //    {
        //        string empCode = db.Employees.Where(x => x.UserLoginID == userLoginID).Select(x => x.EmployeeCode).FirstOrDefault();
        //        if (string.IsNullOrEmpty(empCode))
        //        {
        //            switch (empCode.Substring(0, 4))
        //            {
        //                case "GBMR": roleName = "MERCHANT_EMPLOYEE";
        //                    break;
        //                case "GBFR": roleName = "FRANCHISE_EMPLOYEE";
        //                    break;
        //                case "GBDP": roleName = "DELIVERY_EMPLOYEE";
        //                    break;
        //                case "GBGM": roleName = "ADMIN_EMPLOYEE";
        //                    break;
        //                case "GBCC": roleName = "CRM_EMPLOYEE";
        //                    break;
        //                case "GBAC": roleName = "ACCOUNT_EMPLOYEE";
        //                    break;
        //            }
        //        }
        //    }
        //    catch (BusinessLogicLayer.MyException myEx)
        //    {
        //        throw new BusinessLogicLayer.MyException("[GetRoleName]", "Can't Get Role Name..!" + Environment.NewLine + myEx.Message);
        //    }
        //    return roleName;
        //}
        /// <summary>
        /// To send email to given userloginID
        /// </summary>
        /// <param name="uid">userLoginID</param>
        public void sendEmail(long uid)
        {
            try
            {
                PersonalDetail lPD = db.PersonalDetails.Find(CommonFunctions.GetPersonalDetailsID(uid));
                string email = db.UserLogins.Find(uid).Email;
                // var merchantDetail= db.UserLogins.Find(uid);

                // Sending email to the user
                BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                Dictionary<string, string> emailParaMetres = new Dictionary<string, string>();
                emailParaMetres.Add("<!--NAME-->", lPD.FirstName);
                emailParaMetres.Add("<!--URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "");

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.EMP_APPROVED, new string[] { email, rcKey.DEFAULT_ALL_EMAIL }, emailParaMetres, true);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Employee Approved Succesfully, there might be problem sending email, please check your email or contact administrator!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[EmployeeApprovalController][sendEmail]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[EmployeeApprovalController][sendEmail]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
        }
        /// <summary>
        /// To send SMS to given userloginID
        /// </summary>
        /// <param name="uid">userLoginID</param>
        public void sendSMS(long uid)
        {
            try
            {
                PersonalDetail lPD = db.PersonalDetails.Find(CommonFunctions.GetPersonalDetailsID(uid));
                string mbno = db.UserLogins.Find(uid).Mobile;

                // Sending sms to the user
                BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                Dictionary<string, string> otp = BusinessLogicLayer.OTP.GenerateOTP("MRG");

                Dictionary<string, string> smsValues = new Dictionary<string, string>();
                smsValues.Add("#--NAME--#", lPD.FirstName);

                //gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.EMP_APRVD, new string[] { mbno, rcKey.DEFAULT_ALL_SMS }, smsValues);
                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.EMP_APRVD, new string[] { mbno}, smsValues);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Employee Approved Succesfully, there might be problem sending sms, please check your email or contact administrator!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[EmployeeApprovalController][sendSMS]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[EmployeeApprovalController][sendSMS]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
        }
        /// <summary>
        /// To list of business Owner
        /// </summary>
        /// <param name="uid">OwnerType</param>
        public JsonResult getOwnerID(int? businessTypeID)
        {
            OwnerDetailByPrefix objODP = new OwnerDetailByPrefix();
            List<OwnerDetailByPrefix> lownerType = new List<OwnerDetailByPrefix>();

            lownerType = objODP.OwnerDetail(businessTypeID, System.Web.HttpContext.Current.Server);
            return Json(lownerType, JsonRequestBehavior.AllowGet);
        }
    }
}
