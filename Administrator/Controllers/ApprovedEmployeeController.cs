using Administrator.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Administrator.Controllers
{
    public class ApprovedEmployeeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
       Environment.NewLine
       + "ErrorLog Controller : ApprovalEmployeeController" + Environment.NewLine);
        //
        // GET: /ApprovedEmployee/

        [SessionExpire]
        [CustomAuthorize(Roles = "EmployeeApproval/CanRead")]
        public ActionResult Index()
        {
            ViewBag.BusinessType = new SelectList(db.BusinessTypes, "ID", "Name");

            List<SelectListItem> lData = new List<SelectListItem>();
            lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
            ViewBag.BusinessOwner = new SelectList(lData, "Value", "Text");



            return View(db.Employees.ToList());
        }

        #region ----- Web Methods ------

        public JsonResult getOwnerID(int? businessTypeID)
        {
            OwnerDetailByPrefix objODP = new OwnerDetailByPrefix();
            List<OwnerDetailByPrefix> lownerType = new List<OwnerDetailByPrefix>();

            lownerType = objODP.OwnerDetail(businessTypeID, System.Web.HttpContext.Current.Server);
            return Json(lownerType, JsonRequestBehavior.AllowGet);
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
        public List<OwnerDetailByPrefix> FillOwnerID(int? businessTypeID)
        {
            OwnerDetailByPrefix objODP = new OwnerDetailByPrefix();
            List<OwnerDetailByPrefix> lownerType = new List<OwnerDetailByPrefix>();

            lownerType = objODP.OwnerDetail(businessTypeID, System.Web.HttpContext.Current.Server);
            return lownerType;
        }

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

                //var data = db.Employees.ToList();
                var data = (from e in db.Employees
                            join ul in db.UserLogins on e.UserLoginID equals ul.ID
                            join p in db.PersonalDetails on e.UserLoginID equals p.UserLoginID
                            where e.OwnerID == OwnerID && ul.IsLocked == false
                            select new
                            {
                                ul.Mobile,
                                ul.Email,
                                e.IsActive,
                                e.EmployeeCode,
                                name = p.FirstName + " " + p.MiddleName + " " + p.LastName,
                                e.OwnerID,
                                e.UserLoginID
                            }).ToList();

                foreach (var item in data)
                {
                    EmployeeDetail dd = new EmployeeDetail();
                    dd.EmployeeCode = item.EmployeeCode;
                    dd.OwnerID = (Int64)item.OwnerID;
                    dd.LoginID = item.UserLoginID;
                    dd.IsActive = item.IsActive;
                    dd.Mobile = item.Mobile;
                    dd.Email = item.Email;
                    dd.EmpName = item.name;

                    empLst.Add(dd);
                }

            }
            else
            {
                string prefix = db.BusinessTypes.Where(x => x.ID == businessTypeID).FirstOrDefault().Prefix;

                var data = (from e in db.Employees
                            join ul in db.UserLogins on e.UserLoginID equals ul.ID
                            join p in db.PersonalDetails on e.UserLoginID equals p.UserLoginID
                            where e.OwnerID == OwnerID && ul.IsLocked == false
                            select new
                            {
                                ul.Mobile,
                                ul.Email,
                                e.IsActive,
                                e.EmployeeCode,
                                e.OwnerID,
                                e.UserLoginID,
                                name = p.FirstName + " " + p.MiddleName + " " + p.LastName
                            }).ToList();


                foreach (var item in data)
                {
                    if (item.EmployeeCode.Substring(0, 4).Equals(prefix))
                    {
                        EmployeeDetail dd = new EmployeeDetail();
                        dd.EmployeeCode = item.EmployeeCode;
                        dd.OwnerID = (Int64)item.OwnerID;
                        dd.IsActive = item.IsActive;
                        dd.LoginID = item.UserLoginID;
                        dd.Mobile = item.Mobile;
                        dd.Email = item.Email;
                        dd.EmpName = item.name;

                        empLst.Add(dd);
                    }
                }
            }

            return empLst;

        }

        #endregion

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

        public List<SelectListItem> FillGender()
        {
            List<SelectListItem> lData = new List<SelectListItem>();
            lData.Add(new SelectListItem { Text = "Male", Value = "Male" });
            lData.Add(new SelectListItem { Text = "Female", Value = "Female" });
            lData.Add(new SelectListItem { Text = "Transgender", Value = "Transgender" });
            return (lData);
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "EmployeeApproval/CanWrite")]
        public ActionResult Disapprove(int id)
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
                ViewBag.OwnerType = new SelectList(db.BusinessTypes, "ID", "Name", lBusinessType.ID);


                return View(lEmployeeManagement);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Disapprove[HttpGet]" + Environment.NewLine +
                                   "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                       ex.Message.ToString() + Environment.NewLine +
                             "====================================================================================="
                                   );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Disapprove!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Disapprove!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }

        }

        [SessionExpire]
        [CustomAuthorize(Roles = "EmployeeApproval/CanWrite")]
        [HttpPost]
        public ActionResult Disapprove(EmployeeManagement employeeManagement)
        {
            try
            {
                EmployeeManagement lEmployeeManagement = new EmployeeManagement();
                lEmployeeManagement = FillEmployeeDetail(employeeManagement.UserLoginID);

                ViewBag.Salutation = new SelectList(db.Salutations, "ID", "Name", lEmployeeManagement.SalutationID);
                ViewBag.Gender = new SelectList(this.FillGender(), "Value", "Text", lEmployeeManagement.Gender);


                BusinessType lBusinessType = new BusinessType();
                lBusinessType = db.BusinessTypes.Where(x => x.Prefix == lEmployeeManagement.EmployeeCode.Substring(0, 4)).FirstOrDefault();

                //ViewBag.OwnerID = new SelectList(db.BusinessTypes.Where(x => x.Prefix == lEmployeeManagement.EmployeeCode.Substring(0, 3)).ToList(), "ID", "Name");

                List<OwnerDetailByPrefix> lOwnerID = new List<OwnerDetailByPrefix>();
                lOwnerID = FillOwnerID(lBusinessType.ID);
                ViewBag.BusinessOwner = new SelectList(lOwnerID, "ID", "Name", lEmployeeManagement.OwnerID);
                ViewBag.OwnerType = new SelectList(db.BusinessTypes, "ID", "Name", lBusinessType.ID);

                UserLogin lData = new UserLogin();
                lData = db.UserLogins.Where(x => x.ID == employeeManagement.UserLoginID).FirstOrDefault();

                //UserLogin lUserLogin = new UserLogin();
                //lUserLogin.ID = lData.ID;
                //lUserLogin.IsLocked = true;
                //lUserLogin.Mobile = lData.Mobile;
                //lUserLogin.Email = lData.Email;
                //lUserLogin.Password = lData.Password;
                //lUserLogin.IsLocked = lData.IsLocked;
                //lUserLogin.CreateBy = lData.CreateBy;
                //lUserLogin.CreateDate = lData.CreateDate;
                lData.IsLocked = true;
                lData.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                lData.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                lData.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                lData.DeviceType = string.Empty;
                lData.DeviceID = string.Empty;


                //db.Entry(lData).CurrentValues.SetValues(lUserLogin);
                if (ModelState.IsValid)
                {
                    //db.UserLogins.Add(lData);
                    db.Entry(lData).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Message = "Employee Record Disapprove Successfully!!";
                }

                

                return View(employeeManagement);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Disapprove[HttpPost]" + Environment.NewLine +
                                   "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                       ex.Message.ToString() + Environment.NewLine +
                             "====================================================================================="
                                   );
                ViewBag.Message = "Sorry! Problem in Record Disapprove!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Disapprove!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Disapprove!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }
    
    }
}