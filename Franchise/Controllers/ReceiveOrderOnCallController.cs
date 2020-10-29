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
using Franchise.Models;
using Franchise.Models.ViewModel;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
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
            try
            {
                customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
                customerCareSessionViewModel.Username = Session["USER_NAME"].ToString();
                customerCareSessionViewModel.PersonalDetailID = Convert.ToInt64(Session["PERSONAL_ID"]);
                //Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[SessionDetails]", "Can't assign Session Details..!" + Environment.NewLine + myEx.Message);
            }
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "ReceiveOrderOnCall/CanRead")]
        public ActionResult Index(string FromDate, string ToDate, int? page, int? OrderCodeStatus, string Mobile = "", string OrderCode = "")
        {
            try
            {
                SessionDetails();
                int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                int pageNumber = (page ?? 1);
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.OrderCode = OrderCode;
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                ViewBag.Mobile = Mobile;
                var OrderCodeYesNos = from Franchise.Common.Constant.RECEIVE_ORDER_ON_CALL_STATUS d in Enum.GetValues(typeof(Franchise.Common.Constant.RECEIVE_ORDER_ON_CALL_STATUS))
                                      select new { ID = (int)d, Name = d.ToString() };

                ViewBag.OrderCodeStatus1 = OrderCodeStatus;
                ViewBag.OrderCodeStatus = new SelectList(OrderCodeYesNos, "ID", "Name", OrderCodeStatus);


                var receiveorderoncalls = db.ReceiveOrderOnCalls.Include(r => r.CustomerOrder).Include(r => r.PersonalDetail).Include(r => r.PersonalDetail1).Include(r => r.PersonalDetail2).OrderByDescending(x => x.ID).ToList();

                List<long> list = GetPersonalDetailIDList(franchiseID);
                receiveorderoncalls = receiveorderoncalls.Where(x => list.Contains(x.CreateBy)).ToList();

                if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
                {
                    DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
                    DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);

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
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ReceiveOrderOnCall][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ReceiveOrderOnCall][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        private List<long> GetPersonalDetailIDList(int franchiseID)
        {
            List<long> PIDList = new List<long>();
            try
            {

                PIDList = (from emp in db.Employees
                           join pd in db.PersonalDetails on emp.UserLoginID equals pd.UserLoginID
                           where emp.OwnerID == franchiseID && emp.EmployeeCode.StartsWith(Franchise.Common.Constant.FRANCHISE_CODE)
                           select pd.ID).ToList();

                long PID = BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(db.Franchises.Find(franchiseID).BusinessDetail.UserLoginID);
                PIDList.Add(PID);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[GetPersonalDetailIDList]", "Can't Get PersonalDetail ID List..!" + Environment.NewLine + myEx.Message);
            }
            return PIDList;
        }


        // GET: /ReceiveOrderOnCall/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "ReceiveOrderOnCall/CanRead")]
        public ActionResult Details(int? id)
        {
            try
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
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ReceiveOrderOnCall][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ReceiveOrderOnCall][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /ReceiveOrderOnCall/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "ReceiveOrderOnCall/CanRead")]
        public ActionResult Create()
        {
            try
            {
                SessionDetails();
                ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode");
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
                ViewBag.OrderReceivedPersonalDetailID = new SelectList(db.PersonalDetails, "ID", "FirstName");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ReceiveOrderOnCall][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ReceiveOrderOnCall][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /ReceiveOrderOnCall/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [CustomAuthorize(Roles = "ReceiveOrderOnCall/CanWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,PrimaryMobile,SecondaryMobile,Email,ShippingAddress,Description,CustomerOrderID")] ReceiveOrderOnCall receiveorderoncall)
        {
            try
            {
                SessionDetails();
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
        [CustomAuthorize(Roles = "ReceiveOrderOnCall/CanRead")]
        public ActionResult Edit(int? id)
        {
            try
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
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ReceiveOrderOnCall][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ReceiveOrderOnCall][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /ReceiveOrderOnCall/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [CustomAuthorize(Roles = "ReceiveOrderOnCall/CanWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string OrderCode, string ReferenceOrderCode, [Bind(Include = "ID,Name,PrimaryMobile,SecondaryMobile,Email,ShippingAddress,Description,CustomerOrderID")] ReceiveOrderOnCall receiveorderoncall)
        {
            try
            {
                SessionDetails();
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
            try
            {
                // SessionDetails();
                ReceiveOrderOnCall lReceiveOrderOnCall = db.ReceiveOrderOnCalls.OrderByDescending(x => x.ID).FirstOrDefault(x => x.PrimaryMobile == Mobile || x.SecondaryMobile == Mobile);
                if (lReceiveOrderOnCall != null)
                {
                    return lReceiveOrderOnCall.Name + ";" + lReceiveOrderOnCall.SecondaryMobile + ";" + lReceiveOrderOnCall.Email + ";" + lReceiveOrderOnCall.ShippingAddress;
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[GetCustomerDetail]", "Can't Get Customer Detail..!" + Environment.NewLine + myEx.Message);
            }
            return string.Empty;
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
