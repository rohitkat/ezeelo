using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using Franchise.Models.ViewModel;
using Franchise.Models;

namespace Franchise.Controllers
{
    public class CartLogController : Controller
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
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[SessionDetails]", "Can't assign Session Details..!" + Environment.NewLine + myEx.Message);
            }
            //if (!Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel))
            //{
            //    if (Session["ID"] != null)
            //    {
            //        Session["ID"] = null;
            //    }
            //    TempData["ServerMsg"] = "You are not CustomerCare Person";
            //    Response.Redirect(System.Web.Configuration.WebConfigurationManager.AppSettings["UrlForInvalidCustomerCare"]);
            //}
        }

        // GET: /CartLog/
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrder/CanRead")]

        public ActionResult Index(long CartID)
        {
            List<CartLogViewModel> CartLogViewModels = new List<CartLogViewModel>();
            try
            {
                var cartlogs = db.CartLogs.Where(x => x.CartID == CartID).Include(c => c.Cart);

                var Status = from ModelLayer.Models.Enum.ORDER_STATUS d in Enum.GetValues(typeof(ModelLayer.Models.Enum.ORDER_STATUS))
                             select new { ID = (int)d, Name = d.ToString() };
                CartLogViewModels = (from cartlog in cartlogs
                                     join ul in db.UserLogins on cartlog.CreateBy equals ul.ID
                                     join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                     select new CartLogViewModel
                                     {
                                         ID = cartlog.ID,
                                         CartID = cartlog.CartID,
                                         Description = cartlog.Description,
                                         Status = cartlog.Status,
                                         IsActive = cartlog.IsActive,
                                         CreateDate = cartlog.CreateDate,
                                         CreateBy = cartlog.CreateBy,
                                         ModifyDate = cartlog.ModifyDate,
                                         ModifyBy = cartlog.ModifyBy,
                                         NetworkIP = cartlog.NetworkIP,
                                         DeviceType = cartlog.DeviceType,
                                         DeviceID = cartlog.DeviceID,

                                         //- Extra Added.
                                         StatusVal = "",
                                         CreateByUsername = ul.Mobile,
                                         CreateByPersonName = pd.FirstName
                                     }).ToList();
            }
            catch (Exception)
            {

                throw;
            }
            return View(CartLogViewModels.ToList());
        }

        //// GET: /CartLog/Details/5
        //public ActionResult Details(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CartLog cartlog = db.CartLogs.Find(id);
        //    if (cartlog == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(cartlog);
        //}

        // GET: /CartLog/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrder/CanRead")]
        public ActionResult Create(long CartID)
        {
            Cart lCart = db.Carts.Find(CartID);
            if(lCart == null)
            {
                return View("Error");
            }
            var Status = from ModelLayer.Models.Enum.ORDER_STATUS d in Enum.GetValues(typeof(ModelLayer.Models.Enum.ORDER_STATUS))
                select new { ID = (int)d, Name = d.ToString() };

            ViewBag.Status = new SelectList(Status.Where(x => x.ID == (int)ModelLayer.Models.Enum.ORDER_STATUS.PENDING ||
                                                         x.ID == (int)ModelLayer.Models.Enum.ORDER_STATUS.PLACED || 
                                                         x.ID == (int)ModelLayer.Models.Enum.ORDER_STATUS.CANCELLED), "ID", "Name", lCart.Status);
            return View();
        }

        // POST: /CartLog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrder/CanRead")]
        public ActionResult Create([Bind(Include = "ID,CartID,Description,Status,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] CartLog cartlog, long CartID)
        {
            if (ModelState.IsValid)
            {
                db.CartLogs.Add(cartlog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CartID = new SelectList(db.Carts, "ID", "Name", cartlog.CartID);
            return View(cartlog);
        }
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrder/CanRead")]
        public string CreateCartLog(long CartID, string Description, int Status, string GBOD)
        {
            CartLog cartlog = new CartLog();
            try
            {
                try
                {
                    string lServerMsg = "";
                    Boolean lCartUpdate = UpdateCart(CartID, Status, GBOD, ref lServerMsg);
                    if(lCartUpdate == false)
                    {
                        return lServerMsg;
                    }
                }
                catch (Exception ex)
                {
                    
                }
                cartlog.CartID = CartID;
                cartlog.Description = Description;
                cartlog.Status = Status;
                cartlog.IsActive = true;
                cartlog.CreateDate = DateTime.Now;
                cartlog.CreateBy = Convert.ToInt64(Session["ID"]);
                cartlog.ModifyDate = null;
                cartlog.ModifyBy = null;
                cartlog.NetworkIP = null;
                cartlog.DeviceType = null;
                cartlog.DeviceID = null;
                if (ModelState.IsValid)
                {
                    db.CartLogs.Add(cartlog);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return "";
            }
            return "Success : Log inserted Successfully.";
        }

        private Boolean UpdateCart(long CartID, int Status, string GBOD, ref string ServerMsg)
        {
            try
            {
                Cart lCart = db.Carts.Find(CartID);
                if (lCart == null)
                {
                    ServerMsg = "Error : Cart not found";
                    return false;
                }
                lCart.Status = Status;
                lCart.IsActive = false;
                if (Status == (int)ModelLayer.Models.Enum.ORDER_STATUS.PLACED)
                {
                    if(lCart.CustomerOrderID != null)
                    {
                        ServerMsg = "Error : Some one already placed this Cart";
                        return false;
                    }
                    CustomerOrder lCustomerOrder = db.CustomerOrders.FirstOrDefault(x => x.OrderCode.Trim() == GBOD.Trim());
                    if (lCustomerOrder != null)
                    {
                        lCart.CustomerOrderID = lCustomerOrder.ID;
                        lCart.ModifyDate = DateTime.Now;
                        lCart.ModifyBy = Convert.ToInt64(Session["PERSONAL_ID"]);
                        if (ModelState.IsValid)
                        {
                            db.Entry(lCart).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        ServerMsg = "Error: GBOD does not exist";
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                ServerMsg = "Error : Updating Cart";
                return false;
            }
            return true;
        }
        //// GET: /CartLog/Edit/5
        //public ActionResult Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CartLog cartlog = db.CartLogs.Find(id);
        //    if (cartlog == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.CartID = new SelectList(db.Carts, "ID", "Name", cartlog.CartID);
        //    return View(cartlog);
        //}

        //// POST: /CartLog/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="ID,CartID,Description,Status,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] CartLog cartlog)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(cartlog).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.CartID = new SelectList(db.Carts, "ID", "Name", cartlog.CartID);
        //    return View(cartlog);
        //}

        //// GET: /CartLog/Delete/5
        //public ActionResult Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CartLog cartlog = db.CartLogs.Find(id);
        //    if (cartlog == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(cartlog);
        //}

        //// POST: /CartLog/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(long id)
        //{
        //    CartLog cartlog = db.CartLogs.Find(id);
        //    db.CartLogs.Remove(cartlog);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
