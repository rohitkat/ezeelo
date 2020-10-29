using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using PagedList;
using PagedList.Mvc;
using DeliveryPartner.Models.ViewModel;
using System.Collections;
using DeliveryPartner.Models;

namespace DeliveryPartner.Controllers
{
    [SessionExpire]
    public class EmployeeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private DeliveryPartnerSessionViewModel deliveryPartnerSessionViewModel = new DeliveryPartnerSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            deliveryPartnerSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            deliveryPartnerSessionViewModel.Username = Session["UserName"].ToString();
            Common.Common.GetAllLoginDetailFromSession(ref deliveryPartnerSessionViewModel);
        }
        [SessionExpire]
        public ActionResult Index(int? page, string SearchString = "")
        {
            SessionDetails();
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;

            var employees = db.Employees.Include(e => e.UserLogin).Where(x => x.OwnerID == deliveryPartnerSessionViewModel.UserLoginID).ToList().OrderBy(x => x.CreateDate);
            if (SearchString != "")
            {
                return View(employees.Where(x => x.EmployeeCode.Contains(SearchString)).ToPagedList(pageNumber, pageSize));

            }
            return View(employees.ToPagedList(pageNumber, pageSize));
        }

           
        // GET: /Employee/Details/5
        [SessionExpire]
        public ActionResult Details(long? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            if (employee.OwnerID != deliveryPartnerSessionViewModel.UserLoginID)
            {
                return View("AccessDenied");
            }
            return PartialView("_Details", employee);
        }

        #region Comment
        //// GET: /Employee/Create
        //public ActionResult Create()
        //{
        //    ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile");
        //    return View();
        //}

        //// POST: /Employee/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include="ID,UserLoginID,EmployeeCode,OwnerID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] Employee employee)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Employees.Add(employee);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", employee.UserLoginID);
        //    return View(employee);
        //}

        //// GET: /Employee/Edit/5
        //public ActionResult Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Employee employee = db.Employees.Find(id);
        //    if (employee == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", employee.UserLoginID);
        //    return View(employee);
        //}

        //// POST: /Employee/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="ID,UserLoginID,EmployeeCode,OwnerID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] Employee employee)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(employee).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", employee.UserLoginID);
        //    return View(employee);
        //}
        #endregion
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
