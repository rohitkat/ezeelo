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
using CRM.Models;
using CRM.Models.ViewModel;

namespace CRM.Controllers
{
    //public class OrderCodeYesNo
    //{
    //    public int ID { get; set; }
    //    public string Name { get; set; }
    //}
    public class ReceiveOrderOnCallController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            customerCareSessionViewModel.Username = Session["UserName"].ToString();
            Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel);
        }

        [SessionExpire]
        public ActionResult Index(string FromDate, string ToDate, int? page, int? OrderCodeStatus, string Mobile = "", string OrderCode = "")
        {
            SessionDetails();
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.OrderCode = OrderCode;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.Mobile = Mobile;
            var OrderCodeYesNos = from CRM.Common.Constant.RECEIVE_ORDER_ON_CALL_STATUS d in Enum.GetValues(typeof(CRM.Common.Constant.RECEIVE_ORDER_ON_CALL_STATUS))
                             select new { ID = (int)d, Name = d.ToString() };

            ViewBag.OrderCodeStatus1 = OrderCodeStatus;
            ViewBag.OrderCodeStatus = new SelectList(OrderCodeYesNos, "ID", "Name", OrderCodeStatus);
            
            
            var receiveorderoncalls = db.ReceiveOrderOnCalls.Include(r => r.CustomerOrder).Include(r => r.PersonalDetail).Include(r => r.PersonalDetail1).Include(r => r.PersonalDetail2).OrderByDescending(x => x.ID).ToList();

            if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
            {
                DateTime lFromDate = DateTime.Now;
                if (DateTime.TryParse(FromDate, out lFromDate)) { }

                DateTime lToDate = DateTime.Now;
                if (DateTime.TryParse(ToDate, out lToDate)) { }

                receiveorderoncalls = receiveorderoncalls.Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).ToList();
            }

            if (Mobile != null && Mobile != "")
            {
                receiveorderoncalls = receiveorderoncalls.Where(x => x.PrimaryMobile.Trim().Contains(Mobile.Trim())).ToList();
            }

            if (OrderCode != null && OrderCode != "")
            {
                receiveorderoncalls = receiveorderoncalls.Where(x => x.CustomerOrder != null && x.CustomerOrder.OrderCode.Contains(OrderCode)).ToList();
            }


            if (OrderCodeStatus != null)
            {
                switch (OrderCodeStatus)
                { 
                    case 1:
                        receiveorderoncalls = receiveorderoncalls.Where(x => x.CustomerOrderID != null).ToList();
                        break;
                    case 2:
                        receiveorderoncalls = receiveorderoncalls.Where(x => x.CustomerOrderID == null).ToList();
                        break;
                }
            }


            return View(receiveorderoncalls.ToPagedList(pageNumber, pageSize));
        }


        // GET: /ReceiveOrderOnCall/Details/5
        [SessionExpire]
        public ActionResult Details(int? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiveOrderOnCall receiveorderoncall = db.ReceiveOrderOnCalls.Find(id);
            if (receiveorderoncall == null)
            {
                return HttpNotFound();
            }
            return View(receiveorderoncall);
        }

        // GET: /ReceiveOrderOnCall/Create
        [SessionExpire]
        public ActionResult Create()
        {
            SessionDetails();
            ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode");
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.OrderReceivedPersonalDetailID = new SelectList(db.PersonalDetails, "ID", "FirstName");
            return View();
        }

        // POST: /ReceiveOrderOnCall/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Name,PrimaryMobile,SecondaryMobile,Email,ShippingAddress,Description,CustomerOrderID")] ReceiveOrderOnCall receiveorderoncall)
        {
            SessionDetails();
            try
            {
                receiveorderoncall.OrderReceivedPersonalDetailID = customerCareSessionViewModel.PersonalDetailID;
                receiveorderoncall.CreateDate = DateTime.Now;
                receiveorderoncall.CreateBy = customerCareSessionViewModel.PersonalDetailID;
                if (ModelState.IsValid)
                {
                    db.ReceiveOrderOnCalls.Add(receiveorderoncall);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode", receiveorderoncall.CustomerOrderID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", receiveorderoncall.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", receiveorderoncall.ModifyBy);
                ViewBag.OrderReceivedPersonalDetailID = new SelectList(db.PersonalDetails, "ID", "FirstName", receiveorderoncall.OrderReceivedPersonalDetailID);
                return View(receiveorderoncall);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with receive order on call values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ReceiveOrderOnCall][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.CRM, System.Web.HttpContext.Current.Server);

                ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode", receiveorderoncall.CustomerOrderID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", receiveorderoncall.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", receiveorderoncall.ModifyBy);
                ViewBag.OrderReceivedPersonalDetailID = new SelectList(db.PersonalDetails, "ID", "FirstName", receiveorderoncall.OrderReceivedPersonalDetailID);
                return View(receiveorderoncall);
            }
        }

        // GET: /ReceiveOrderOnCall/Edit/5
        [SessionExpire]
        public ActionResult Edit(int? id)
        {
            SessionDetails();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReceiveOrderOnCall receiveorderoncall = db.ReceiveOrderOnCalls.Find(id);
            if (receiveorderoncall == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode", receiveorderoncall.CustomerOrderID);
            CustomerOrder lCustomerOrder = db.CustomerOrders.Find(receiveorderoncall.CustomerOrderID);
            if (lCustomerOrder != null)
            {
                ViewBag.OrderCode = lCustomerOrder.OrderCode;
                ViewBag.ReferenceOrderCode = Common.Common.GetRefOrderCode(lCustomerOrder.ReferenceCustomerOrderID);
            }

            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", receiveorderoncall.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", receiveorderoncall.ModifyBy);
            ViewBag.OrderReceivedPersonalDetailID = new SelectList(db.PersonalDetails.Where(x => x.ID == receiveorderoncall.OrderReceivedPersonalDetailID), "ID", "FirstName", receiveorderoncall.OrderReceivedPersonalDetailID);
            return View(receiveorderoncall);
        }

        // POST: /ReceiveOrderOnCall/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string OrderCode, string ReferenceOrderCode, [Bind(Include = "ID,Name,PrimaryMobile,SecondaryMobile,Email,ShippingAddress,Description,CustomerOrderID")] ReceiveOrderOnCall receiveorderoncall)
        {
            SessionDetails();
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                ReceiveOrderOnCall lReceiveOrderOnCall = db1.ReceiveOrderOnCalls.Find(receiveorderoncall.ID);
                receiveorderoncall.OrderReceivedPersonalDetailID = lReceiveOrderOnCall.OrderReceivedPersonalDetailID;
                receiveorderoncall.CustomerOrderID = lReceiveOrderOnCall.CustomerOrderID;
                CustomerOrder lCustomerOrder = db.CustomerOrders.SingleOrDefault(x => x.OrderCode.Trim().ToUpper().Equals(OrderCode.Trim().ToUpper()));
                if (lCustomerOrder != null)
                {
                    receiveorderoncall.CustomerOrderID = lCustomerOrder.ID;
                    ViewBag.OrderCode = lCustomerOrder.OrderCode;

                    //----------------------------- Receive order on call must have Unique CustomerOrderID -//
                    if (db.ReceiveOrderOnCalls.Any(x => x.CustomerOrderID == lCustomerOrder.ID))
                    {
                        throw new Exception("Order Code : " + OrderCode + " already placed with other User.");
                    }

                    CustomerOrder lRefCustomerOrder = db.CustomerOrders.SingleOrDefault(x => x.OrderCode.Trim().ToUpper().Equals(ReferenceOrderCode.Trim().ToUpper()));
                    if (lRefCustomerOrder != null)
                    {
                        lCustomerOrder.ReferenceCustomerOrderID = lRefCustomerOrder.ID;
                        lCustomerOrder.UserLoginID = lRefCustomerOrder.UserLoginID;
                        db.SaveChanges();
                        ViewBag.ReferenceOrderCode = lRefCustomerOrder.OrderCode;
                    }
                }


                if (receiveorderoncall.Description == null)
                {
                    receiveorderoncall.Description = lReceiveOrderOnCall.Description;
                }

                receiveorderoncall.CreateBy = lReceiveOrderOnCall.CreateBy;
                receiveorderoncall.CreateDate = lReceiveOrderOnCall.CreateDate;
                receiveorderoncall.ModifyBy = customerCareSessionViewModel.PersonalDetailID;
                receiveorderoncall.ModifyDate = DateTime.Now;
                db1.Dispose();

                if (ModelState.IsValid)
                {
                    db.Entry(receiveorderoncall).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode", receiveorderoncall.CustomerOrderID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", receiveorderoncall.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", receiveorderoncall.ModifyBy);
                ViewBag.OrderReceivedPersonalDetailID = new SelectList(db.PersonalDetails, "ID", "FirstName", receiveorderoncall.OrderReceivedPersonalDetailID);
                return View(receiveorderoncall);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with receive order on call values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ReceiveOrderOnCall][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.CRM, System.Web.HttpContext.Current.Server);

                ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode", receiveorderoncall.CustomerOrderID);
                //CustomerOrder lCustomerOrder = db.CustomerOrders.Find(receiveorderoncall.CustomerOrderID);
                //if (lCustomerOrder != null)
                {
                    ViewBag.OrderCode = null;
                }
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", receiveorderoncall.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", receiveorderoncall.ModifyBy);
                ViewBag.OrderReceivedPersonalDetailID = new SelectList(db.PersonalDetails, "ID", "FirstName", receiveorderoncall.OrderReceivedPersonalDetailID);
                return View(receiveorderoncall);
            }
        }

        //[SessionExpire]
        public string GetCustomerDetail(string Mobile)
        {
           // SessionDetails();
            ReceiveOrderOnCall lReceiveOrderOnCall = db.ReceiveOrderOnCalls.OrderByDescending(x => x.ID).FirstOrDefault(x => x.PrimaryMobile == Mobile || x.SecondaryMobile == Mobile);
            if (lReceiveOrderOnCall != null)
            {
                return lReceiveOrderOnCall.Name + ";" + lReceiveOrderOnCall.SecondaryMobile + ";" + lReceiveOrderOnCall.Email + ";" + lReceiveOrderOnCall.ShippingAddress;
            }
            return "";
        }

        public JsonResult IsValidOrder(string OrderCode)
        {
            /*This Action Responces to AJAX Call
             * After entering Pincode returens City, District and State Information
             * */
            OrderCode = OrderCode.Trim().ToUpper();
            if (db.CustomerOrders.Any(x => x.OrderCode.Trim().ToUpper() == OrderCode))
            {
                //var errorMsg = "Order Code Dose Not Exist.";
                return Json("true", JsonRequestBehavior.AllowGet);
            }
            return Json("false", JsonRequestBehavior.AllowGet);
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
