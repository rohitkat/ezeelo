
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
using System.Transactions;
using System.Text;
using Franchise.Models;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    [SessionExpire]
    public class DeliveryBoyManagementController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
       Environment.NewLine
       + "ErrorLog Controller : DeliveryBoyManagementController" + Environment.NewLine);

        [CustomAuthorize(Roles = "EmployeeManagement/CanRead")]
        // GET: /EmployeeManagement/
        public ActionResult Index()
        {

          
            Int64 franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //this.getFranchiseID();
            ViewBag.BusinessOwner = new SelectList(db.Shops.Where(x => x.FranchiseID == franchiseID).ToList(), "ID", "Name");
            return View(db.DeliveryBoys.Where(x => x.FranchiseID == franchiseID).ToList());

        }

        [CustomAuthorize(Roles = "EmployeeManagement/CanRead")]
        // GET: /EmployeeManagement/Details/5
        public ActionResult Details(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
               
                Int64 franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //this.getFranchiseID();

                ModelLayer.Models.DeliveryBoy lDeliveryBoyManagement = db.DeliveryBoys.Find(id);
                if (lDeliveryBoyManagement == null)
                {
                    return HttpNotFound();
                }


                return View(lDeliveryBoyManagement);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Detail[HttpGet]" + Environment.NewLine +
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

        [CustomAuthorize(Roles = "EmployeeManagement/CanWrite")]
        // GET: /EmployeeManagement/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.Salutation = new SelectList(db.Salutations, "ID", "Name");
                ViewBag.OwnerType = new SelectList(db.BusinessTypes.Where(x => x.Prefix == "GBMR" || x.Prefix == "GBFR"), "ID", "Name");
                //ViewBag.Gender = new SelectList(this.FillGender(), "Value", "Text");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.BusinessOwner = new SelectList(lData, "Value", "Text");

                Int64 franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //this.getFranchiseID();
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
                ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        // POST: /EmployeeManagement/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserLoginID,EmployeeCode,OwnerID,SalutationID,FirstName,MiddleName,LastName,DOB,Gender,PincodeID,Address,AlternateMobile,AlternateEmail,Mobile,Email,Password,IsActive")] EmployeeManagement employeemanagement, int? Salutation, int? BusinessOwner, int? OwnerType)
        {
            try
            {
                ViewBag.Salutation = new SelectList(db.Salutations, "ID", "Name");
                ViewBag.OwnerType = new SelectList(db.BusinessTypes.Where(x => x.Prefix == "GBMR" || x.Prefix == "GBFR"), "ID", "Name");
               

                Int64 franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //this.getFranchiseID();
                ViewBag.BusinessOwner = new SelectList(db.Franchises.Where(x => x.ID == franchiseID), "ID", "ContactPerson");

                if (BusinessOwner == null)
                {
                    ModelState.AddModelError("BusinessOwner", "Please Select the Business Owner");
                    ViewBag.ErrorMessage = "Please Select the Business Owner";
                    return View();
                }
                if (OwnerType == null)
                {
                    ModelState.AddModelError("BusinessOwner", "Please Select the Business Type");
                    ViewBag.ErrorMessage = "Please Select the Business Type";
                    return View();
                }
                if (Salutation == null)
                {
                    ModelState.AddModelError("Salutation", "Please Select the Salutation");
                    ViewBag.ErrorMessage = "Please Select the Salutation";
                    return View();
                }

                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        try
                        {
                            employeemanagement.IsLocked = true;

                            employeemanagement.OwnerID = BusinessOwner;

                            string EmployeeCode = GenerateEmployeeCode(employeemanagement.OwnerID, Convert.ToInt32(OwnerType));

                            employeemanagement.EmployeeCode = EmployeeCode;

                            employeemanagement.SalutationID = Convert.ToInt16(Salutation);

                            Int64 ID = InsertUserLogin(employeemanagement);

                            InsertPersonalDetail(employeemanagement, ID);

                            InsertEmployeeDetail(employeemanagement, ID);

                            ts.Complete();

                            /*Send Email Message*/
                            this.SendEmail(employeemanagement);
                            /*Send Message on Mobile*/
                            this.SendSMS(employeemanagement);
                            //Model Data Dispose
                            ModelState.Clear();
                            //Successfull message
                            // ViewBag.Message = "Menu for Role Inserted Successfully!!";
                            ModelState.AddModelError("Message", "Employee Record Created Successfully!!");
                            ViewBag.ErrorMessage = "Employee Record Created Successfully!!";
                            employeemanagement = null;
                            //return RedirectToAction("Index");
                        }
                        catch (Exception ex)
                        {
                            //View bag to fill role dropdown again

                            //Incase of Insertion fail Message to be Display
                            ViewBag.Message = "Sorry! Problem in Inserting Menu for Employee!!";
                            //RollBack All Transaction
                            ts.Dispose();


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
                }

                return View(employeemanagement);
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

        [CustomAuthorize(Roles = "EmployeeManagement/CanWrite")]
        // GET: /EmployeeManagement/Edit/5
        public ActionResult Edit(long? id)
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

                Int64 franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //this.getFranchiseID();

                var ShopempList = (from e in db.Employees
                                   join s in db.Shops on e.OwnerID equals s.ID
                                   join f in db.Franchises on s.FranchiseID equals f.ID
                                   where f.ID == franchiseID && e.EmployeeCode.Substring(0, 4) == "GBMR" && e.ID == id
                                   select new
                                   {
                                       e.EmployeeCode,
                                       e.ID,
                                       e.OwnerID
                                   }).ToList();

                var franchiseEmplList = db.Employees.Where(x => x.OwnerID == franchiseID && x.EmployeeCode.Substring(0, 4) == "GBFR" && x.ID == id).ToList();

                if (franchiseEmplList.Count() < 0 && ShopempList.Count() < 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }



               


                BusinessType lBusinessType = new BusinessType();
                lBusinessType = db.BusinessTypes.Where(x => x.Prefix == lEmployeeManagement.EmployeeCode.Substring(0, 4)).FirstOrDefault();

                //ViewBag.OwnerID = new SelectList(db.BusinessTypes.Where(x => x.Prefix == lEmployeeManagement.EmployeeCode.Substring(0, 3)).ToList(), "ID", "Name");

                List<OwnerDetailByPrefix> lData = new List<OwnerDetailByPrefix>();
                lData = FillOwnerID(lBusinessType.ID);
                ViewBag.BusinessOwner = new SelectList(lData, "ID", "Name", lEmployeeManagement.OwnerID);
                //ViewBag.OwnerType = new SelectList(db.BusinessTypes, "ID", "Name", lBusinessType.ID);


                return View(lEmployeeManagement);
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

        [CustomAuthorize(Roles = "EmployeeManagement/CanWrite")]
        // POST: /EmployeeManagement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserLoginID,EmployeeCode,OwnerID,SalutationID,FirstName,MiddleName,LastName,DOB,Gender,PincodeID,Address,AlternateMobile,AlternateEmail,Mobile,Email,Password,IsActive")] EmployeeManagement employeemanagement, int? Salutation, int? BusinessOwner)
        {
            try
            {
                ViewBag.Salutation = new SelectList(db.Salutations, "ID", "Name");
                ViewBag.OwnerType = new SelectList(db.BusinessTypes, "ID", "Name");
               

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.BusinessOwner = new SelectList(lData, "Value", "Text");

                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        try
                        {


                            employeemanagement.SalutationID = Convert.ToInt16(Salutation);

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
                        catch (Exception ex)
                        {
                            //View bag to fill role dropdown again

                            //Incase of Insertion fail Message to be Display
                            ViewBag.Message = "Sorry! Problem in Inserting Menu for Employee!!";
                            //RollBack All Transaction
                            ts.Dispose();


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
                }



                return View(employeemanagement);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Edit[HttpPost]" + Environment.NewLine +
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

        [CustomAuthorize(Roles = "EmployeeManagement/CanDelete")]
        // GET: /EmployeeManagement/Delete/5
        public ActionResult Delete(long? id)
        {
            try
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //EmployeeManagement employeemanagement = db.EmployeeManagements.Find(id);
                EmployeeManagement lEmployeeManagement = new EmployeeManagement();
             
                if (lEmployeeManagement == null)
                {
                    return HttpNotFound();
                }
                return View(lEmployeeManagement);
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

        [CustomAuthorize(Roles = "EmployeeManagement/CanDelete")]
        // POST: /EmployeeManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        Employee emp = new Employee();
                        emp = db.Employees.Where(x => x.UserLoginID == id).FirstOrDefault();
                        db.Employees.Remove(emp);
                        db.SaveChanges();

                        PersonalDetail lPersonalDetail = new PersonalDetail();
                        lPersonalDetail = db.PersonalDetails.Where(x => x.UserLoginID == id).FirstOrDefault();
                        db.PersonalDetails.Remove(lPersonalDetail);
                        db.SaveChanges();

                        UserLogin lUserLogin = db.UserLogins.Find(id);
                        db.UserLogins.Remove(lUserLogin);
                        db.SaveChanges();

                        ts.Complete();
                        //Model Data Dispose
                        ModelState.Clear();
                        //Successfull message
                        // ViewBag.Message = "Menu for Role Inserted Successfully!!";
                        ModelState.AddModelError("Message", "Employee Record Created Successfully!!");

                        //return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        //View bag to fill role dropdown again

                        //Incase of Insertion fail Message to be Display
                        ViewBag.Message = "Sorry! Problem in Inserting Menu for Employee!!";
                        //RollBack All Transaction
                        ts.Dispose();


                        errStr.Append("Method Name[Http Request] :- Delete[HttpPost]" + Environment.NewLine +
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

                //EmployeeManagement employeemanagement = db.EmployeeManagements.Find(id);
                //db.EmployeeManagements.Remove(employeemanagement);
                //db.SaveChanges();
                return RedirectToAction("Index");
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        #region -----GENERAL METHOD -----

        private void SendSMS(EmployeeManagement employeemanagement)
        {
            try
            {
                // Sending email to the user
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);

                Dictionary<string, string> otp = BusinessLogicLayer.OTP.GenerateOTP("MRG");

                Dictionary<string, string> smsValues = new Dictionary<string, string>();
                smsValues.Add("#--NAME--#", employeemanagement.FirstName);
                smsValues.Add("#--OTP--#", otp["OTP"]);

                gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT, BusinessLogicLayer.GateWay.SMSOptions.MULTIPLE, BusinessLogicLayer.GateWay.SMSTypes.OTP_MER_REG, new string[] { employeemanagement.Mobile, rcKey.DEFAULT_ALL_SMS }, smsValues);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Employee Registered Succesfully, there might be problem sending SMS, please check your mobile or contact administrator!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send SMS..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH, BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                // throw new Exception("Unable to Send Email");
            }
        }

        private void SendEmail(EmployeeManagement employeemanagement)
        {
            try
            {
                // Sending email to the user
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                Dictionary<string, string> emailParaMetres = new Dictionary<string, string>();
                emailParaMetres.Add("<!--ACCOUNT_URL-->", "" + (new URLsFromConfig()).GetURL("PARTNER") + "");
                emailParaMetres.Add("<!--ORDERS_URL-->", "" + (new URLsFromConfig()).GetURL("PARTNER") + "/CustomerOrder");
                emailParaMetres.Add("<!--NAME-->", employeemanagement.FirstName);
                emailParaMetres.Add("<!--URL-->", "" + (new URLsFromConfig()).GetURL("PARTNER") + "");

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ACT_LINK, new string[] { employeemanagement.Email, rcKey.DEFAULT_ALL_EMAIL }, emailParaMetres, true);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Employee Registered Succesfully, there might be problem sending email, please check your email or contact administrator!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send Email..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH, BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                //throw new Exception("Unable to Send SMS");
            }
        }

        private string GenerateEmployeeCode(long? OwnerID, int OwnerType)
        {
            BusinessType lBusinessType = new BusinessType();
            lBusinessType = db.BusinessTypes.Where(x => x.ID == OwnerType).FirstOrDefault();

            long lCount = db.Employees.ToList().Count();

            string lcode = (lBusinessType.Prefix + DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day + lCount);

            return lcode;
        }

        private void InsertEmployeeDetail(EmployeeManagement employeemanagement, long ID)
        {
            try
            {
                Employee lEmployee = new Employee();
                lEmployee.UserLoginID = ID;
                lEmployee.EmployeeCode = employeemanagement.EmployeeCode;
                lEmployee.OwnerID = employeemanagement.OwnerID;
                lEmployee.IsActive = employeemanagement.IsActive;
                lEmployee.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                lEmployee.CreateDate = DateTime.UtcNow.AddHours(5.30);
                lEmployee.DeviceID = string.Empty;
                lEmployee.DeviceType = string.Empty;

                db.Employees.Add(lEmployee);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Insert Recored in Employee Detail :- " + ex.InnerException);

            }
        }

        private void InsertPersonalDetail(EmployeeManagement employeemanagement, long ID)
        {
            try
            {
                PersonalDetail lPersonalDetail = new PersonalDetail();
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

                lPersonalDetail.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                lPersonalDetail.CreateDate = DateTime.UtcNow.AddHours(5.30);
                lPersonalDetail.DeviceID = string.Empty;
                lPersonalDetail.DeviceType = string.Empty;

                db.PersonalDetails.Add(lPersonalDetail);
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Insert Recored in Personal Detail :- " + ex.InnerException);

            }

        }

        private long InsertUserLogin(EmployeeManagement employeemanagement)
        {
            try
            {
                var iteam = db.UserLogins.Where(x => x.Mobile == employeemanagement.Mobile).FirstOrDefault();
                if (iteam != null)
                {
                    throw new Exception("Mobile No is already exists...!");
                }
                iteam = db.UserLogins.Where(x => x.Email == employeemanagement.Email).FirstOrDefault();
                if (iteam != null)
                {
                    throw new Exception("Email ID is already exists...!");
                }

                UserLogin lUserLogin = new UserLogin();
                lUserLogin.Mobile = employeemanagement.Mobile;
                lUserLogin.Email = employeemanagement.Email;
                lUserLogin.Password = employeemanagement.Password;
                lUserLogin.IsLocked = true;// employeemanagement.IsLocked;
                lUserLogin.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                lUserLogin.CreateDate = DateTime.UtcNow.AddHours(5.30);
                lUserLogin.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                lUserLogin.DeviceType = string.Empty;
                lUserLogin.DeviceID = string.Empty;



                db.UserLogins.Add(lUserLogin);
                db.SaveChanges();

                return lUserLogin.ID;

            }
            catch (Exception ex)
            {
                throw new Exception("Unable to Insert Recored in Login Detail :- " + ex.InnerException);

            }

        }

        private void UpdateEmployeeDetail(EmployeeManagement employeemanagement, long ID)
        {
            try
            {

                Employee lData = new Employee();
                lData = db.Employees.Where(x => x.UserLoginID == employeemanagement.UserLoginID).FirstOrDefault();


                Employee lEmployee = new Employee();
                lEmployee.ID = lData.ID;
                lEmployee.UserLoginID = employeemanagement.UserLoginID;
                lEmployee.EmployeeCode = employeemanagement.EmployeeCode;
                lEmployee.OwnerID = employeemanagement.OwnerID;
                lEmployee.IsActive = employeemanagement.IsActive;
                lEmployee.CreateBy = lData.CreateBy;
                lEmployee.CreateDate = lData.CreateDate;
                lEmployee.ModifyDate = DateTime.UtcNow.AddHours(5.30);
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

        private void UpdatePersonalDetail(EmployeeManagement employeemanagement, long ID)
        {
            try
            {
                PersonalDetail lData = new PersonalDetail();
                lData = db.PersonalDetails.Where(x => x.UserLoginID == employeemanagement.UserLoginID).FirstOrDefault();

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
                lPersonalDetail.ModifyDate = DateTime.UtcNow.AddHours(5.30);
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

                UserLogin lUserLogin = new UserLogin();
                lUserLogin.ID = lData.ID;
                lUserLogin.IsLocked = lData.IsLocked;
                lUserLogin.Mobile = employeemanagement.Mobile;
                lUserLogin.Email = employeemanagement.Email;
                lUserLogin.Password = employeemanagement.Password;
                lUserLogin.IsLocked = lData.IsLocked;
                lUserLogin.CreateBy = lData.CreateBy;
                lUserLogin.CreateDate = lData.CreateDate;
                lUserLogin.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                lUserLogin.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
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



       

        //private Int64 getFranchiseID()
        //{
        //    int LoginID = Convert.ToInt32(Session["ID"].ToString());

        //    Int64 BusinessDetailID = db.BusinessDetails.Where(x => x.UserLoginID == LoginID).FirstOrDefault().ID;
        //    Int64 franchiseID = db.Franchises.Where(x => x.BusinessDetailID == BusinessDetailID).FirstOrDefault().ID;
        //    return franchiseID;
        //}




        #endregion


        #region ----- Web Methods ------

        public JsonResult getOwnerID(int businessTypeID)
        {
            List<OwnerDetailByPrefix> MerchantList = new List<OwnerDetailByPrefix>();

            Int64 franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //getFranchiseID();

            if (db.BusinessTypes.Where(x => x.ID == businessTypeID).FirstOrDefault().Prefix == "GBMR")
            {
                MerchantList = (from S in db.Shops
                                where S.FranchiseID == franchiseID
                                //join f in db.Franchises on S.FranchiseID equals f.ID
                                //join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                                //join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
                                //where bt.Prefix == "GBFR" && f.ID == franchiseID
                                select new OwnerDetailByPrefix
                                {
                                    ID = S.ID,
                                    Name = S.Name
                                }
                                   ).ToList();

            }
            if (db.BusinessTypes.Where(x => x.ID == businessTypeID).FirstOrDefault().Prefix == "GBFR")
            {
                MerchantList = (from f in db.Franchises
                                join b in db.BusinessDetails on f.BusinessDetailID equals b.ID
                                where f.ID == franchiseID
                                select new OwnerDetailByPrefix
                                {
                                    ID = f.ID,
                                    Name = b.Name
                                }
                                   ).ToList();

            }

            return Json(MerchantList, JsonRequestBehavior.AllowGet);
        }

        public List<OwnerDetailByPrefix> FillOwnerID(int? businessTypeID)
        {
            ////OwnerDetailByPrefix objODP = new OwnerDetailByPrefix();
            //List<OwnerDetailByPrefix> lownerType = new List<OwnerDetailByPrefix>();

            //// lownerType = objODP.OwnerDetail(businessTypeID,System.Web.HttpContext.Current.Server);

            //var MerchantList = (from S in db.Shops
            //                    join f in db.Franchises on S.FranchiseID equals f.ID
            //                    join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
            //                    join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
            //                    where bt.Prefix == "GBFR" && f.ID == 4
            //                    select new
            //                    {
            //                        S.ID,
            //                        S.Name
            //                    }
            //                        );


            //foreach (var iteam in MerchantList)
            //{
            //    OwnerDetailByPrefix dd = new OwnerDetailByPrefix();
            //    dd.ID = iteam.ID;
            //    dd.Name = iteam.Name;

            //    lownerType.Add(dd);
            //}

            //return lownerType;
            ////OwnerDetailByPrefix objODP = new OwnerDetailByPrefix();
            ////List<OwnerDetailByPrefix> lownerType = new List<OwnerDetailByPrefix>();

            ////lownerType = objODP.OwnerDetail(businessTypeID,System.Web.HttpContext.Current.Server);
            ////return lownerType;
            List<OwnerDetailByPrefix> MerchantList = new List<OwnerDetailByPrefix>();
            Int64 franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //getFranchiseID();
            if (db.BusinessTypes.Where(x => x.ID == businessTypeID).FirstOrDefault().Prefix == "GBMR")
            {
                MerchantList = (from S in db.Shops
                                where S.FranchiseID == franchiseID
                                //join f in db.Franchises on S.FranchiseID equals f.ID
                                //join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                                //join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
                                //where bt.Prefix == "GBFR" && f.ID == franchiseID
                                select new OwnerDetailByPrefix
                                {
                                    ID = S.ID,
                                    Name = S.Name
                                }
                                   ).ToList();

            }
            if (db.BusinessTypes.Where(x => x.ID == businessTypeID).FirstOrDefault().Prefix == "GBFR")
            {
                MerchantList = (from f in db.Franchises
                                join b in db.BusinessDetails on f.BusinessDetailID equals b.ID
                                where f.ID == franchiseID
                                select new OwnerDetailByPrefix
                                {
                                    ID = f.ID,
                                    Name = b.Name
                                }
                                   ).ToList();

            }

            return MerchantList.ToList();
        }

        public JsonResult EmployeeList(int OwnerID)
        {
            List<EmployeeDetail> lst = new List<EmployeeDetail>();

            int businessTypeID = db.BusinessTypes.Where(x => x.Prefix == "GBMR").FirstOrDefault().ID;

            lst = GetEmployeeList(businessTypeID, OwnerID);

            return Json(lst, JsonRequestBehavior.AllowGet);

        }

        public class EmployeeDetail
        {
            public string EmployeeCode { get; set; }
            public string EmployeeName { get; set; }
            public Int64 OwnerID { get; set; }
            public Int64 LoginID { get; set; }
            public bool IsActive { get; set; }
            public bool IsLocked { get; set; }
        }


        public List<EmployeeDetail> GetEmployeeList(int businessTypeID, int OwnerID)
        {
            List<EmployeeDetail> empLst = new List<EmployeeDetail>();

            if (OwnerID == 0)
            {
                Int64 franchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //this.getFranchiseID();               
                var data = db.Employees.Where(x=>x.OwnerID == franchiseID && x.EmployeeCode.Substring(0, 4).ToUpper() == "GBFR").ToList();
                foreach (var item in data)
                {
                    EmployeeDetail dd = new EmployeeDetail();
                    dd.EmployeeCode = item.EmployeeCode;
                    dd.OwnerID = (Int64)item.OwnerID;
                    dd.LoginID = item.UserLoginID;
                    dd.IsActive = item.IsActive;
                    dd.IsLocked = item.UserLogin.IsLocked;
                    dd.EmployeeName = Franchise.Models.FranchiseCommonFunction.GetEmployeeName(item.UserLoginID);
                    empLst.Add(dd);
                }

            }
            else if (businessTypeID < 1 || OwnerID < 1)
            {

                var data = db.Employees.ToList();
                foreach (var item in data)
                {
                    EmployeeDetail dd = new EmployeeDetail();
                    dd.EmployeeCode = item.EmployeeCode;
                    dd.OwnerID = (Int64)item.OwnerID;
                    dd.LoginID = item.UserLoginID;
                    dd.IsActive = item.IsActive;
                    dd.IsLocked = item.UserLogin.IsLocked;
                    dd.EmployeeName = Franchise.Models.FranchiseCommonFunction.GetEmployeeName(item.UserLoginID);
                    empLst.Add(dd);
                }

            }
            else
            {
                string prefix = db.BusinessTypes.Where(x => x.ID == businessTypeID).FirstOrDefault().Prefix;

                var data = db.Employees.Where(x => x.OwnerID == OwnerID).ToList();

                foreach (var item in data)
                {
                    if (item.EmployeeCode.Substring(0, 4).Equals(prefix))
                    {
                        EmployeeDetail dd = new EmployeeDetail();
                        dd.EmployeeCode = item.EmployeeCode;
                        dd.OwnerID = (Int64)item.OwnerID;
                        dd.IsActive = item.IsActive;
                        dd.LoginID = item.UserLoginID;
                        dd.IsLocked = item.UserLogin.IsLocked;
                        dd.EmployeeName = Franchise.Models.FranchiseCommonFunction.GetEmployeeName(item.UserLoginID);
                        empLst.Add(dd);
                    }
                }
            }

            return empLst;

        }


        public JsonResult CheckEmail(string strEmail)
        {
            int mailCount = 0;
            if (!strEmail.Equals(string.Empty))
            {
                mailCount = db.UserLogins.Where(x => x.Email.ToLower() == strEmail.ToLower()).Count();
            }
            return Json(mailCount, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckMobile(string strMobile)
        {
            int mobileCount = 0;
            if (!strMobile.Equals(string.Empty))
            {
                mobileCount = db.UserLogins.Where(x => x.Mobile == strMobile).Count();
            }
            return Json(mobileCount, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckEditEmail(string strEmail, string strUserLoginID)
        {
            int mailCount = 0;
            if (!strEmail.Equals(string.Empty))
            {
                Int64 loginID = Convert.ToInt64(strUserLoginID);
                mailCount = db.UserLogins.Where(x => x.Email.ToLower() == strEmail.ToLower() && x.ID != loginID).Count();
            }
            return Json(mailCount, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckEditMobile(string strMobile, string strUserLoginID)
        {
            int mobileCount = 0;
            if (!strMobile.Equals(string.Empty))
            {
                Int64 loginID = Convert.ToInt64(strUserLoginID);
                mobileCount = db.UserLogins.Where(x => x.Mobile == strMobile && x.ID != loginID).Count();
            }
            return Json(mobileCount, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
