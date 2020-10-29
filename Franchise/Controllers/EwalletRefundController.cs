using BusinessLogicLayer;
using Franchise.Models;
using Franchise.Models.ViewModel;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Franchise.Controllers
{
    //Added by Rumana on 19/04/2019
    public class EwalletRefundController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 10;
        private static int PendingCount;
        public void SessionDetails()
        {
            try
            {
                customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
                customerCareSessionViewModel.Username = Session["USER_NAME"].ToString();
                customerCareSessionViewModel.PersonalDetailID = Convert.ToInt64(Session["PERSONAL_ID"]);
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

        // GET: /Cart/
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrder/CanRead")]
        public ActionResult Index(string FromDate, string ToDate, int? Status, int? page)
        {

            SessionDetails();
            int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.SelectedStatus = Status;
            //EwalletRefundviewmodel obj = new EwalletRefundviewmodel();
            List<EwalletRefundviewmodel> Llist_EwalletRefundviewmodel = new List<EwalletRefundviewmodel>();
            try
            {
                var customerorders = (from CO in db.CustomerOrders
                                      join COD in db.CustomerOrderDetails on CO.ID equals (COD.CustomerOrderID)
                                      join PD in db.PersonalDetails on CO.UserLoginID equals PD.UserLoginID
                                      join SS in db.ShopStocks on COD.ShopStockID equals SS.ID
                                      join SP in db.ShopProducts on SS.ShopProductID equals SP.ID
                                      join S in db.Shops on SP.ShopID equals S.ID
                                      //join WA in db.eWalletRefund_Table on CO.ID equals WA.CustomerOrderId
                                      where S.FranchiseID == franchiseID && COD.OrderStatus == (int)Common.Constant.ORDER_STATUS.CANCELLED && CO.MLMAmountUsed > 0
                                      select new Models.ViewModel.CustomerOrderViewModel
                                      {
                                          ID = CO.ID,

                                      }).Distinct().OrderByDescending(x => x.ID).ToList();

                List<long> Ids = customerorders.Select(x => x.ID).Distinct().ToList();
           
                Llist_EwalletRefundviewmodel = (from n in db.eWalletRefund_Table.Where(x => Ids.Contains(x.CustomerOrderId)).ToList()
                                                join PD in db.PersonalDetails on n.UserLoginId equals PD.UserLoginID

                                                select new EwalletRefundviewmodel
                                                {

                                                    ID = n.ID,
                                                    RefundAmt = n.RefundAmt,
                                                    RequsetAmt = n.RequsetAmt,
                                                    UserLoginId = n.UserLoginId,
                                                    CustomerOrderId = n.CustomerOrderId,
                                                    Comment = n.Comment,
                                                    Isactive = n.Isactive,
                                                    Date = n.Date,
                                                    Status = n.Status,
                                                    UserName= PD.FirstName +' '+ PD.LastName
                                                }).ToList();
                //Llist_EwalletRefundviewmodel = Llist_EwalletRefundviewmodel.Where(x => Ids.Contains(x.CustomerOrderId));
                foreach (var item in Llist_EwalletRefundviewmodel)
                {
                    
                    if (item.Status == 0)
                    {
                        item.ReturnStatus = "Pending";
                    }
                    if (item.Status == 1)
                    {
                        item.ReturnStatus = "Accepted";
                    }
                    if (item.Status == 2)
                    {
                        item.ReturnStatus = "Cancelled";
                    }
                }

               
                if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
                {
                   
                    DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
                    DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);

                    Llist_EwalletRefundviewmodel = Llist_EwalletRefundviewmodel.Where(x => x.Date.Date >= lFromDate.Date && x.Date.Date <= lToDate.Date).ToList();
                }
                if (Status != null)
                {
                    Llist_EwalletRefundviewmodel = Llist_EwalletRefundviewmodel.Where(x => x.Status == Status).ToList();
                }
                Llist_EwalletRefundviewmodel = Llist_EwalletRefundviewmodel.OrderByDescending(x => x.ID).ToList();
                List<long> EWalletRefundIds = Llist_EwalletRefundviewmodel.Select(x => x.ID).Distinct().ToList();
                foreach (var item in Llist_EwalletRefundviewmodel)
                {
                    item.MlmWalletlogViewModel = db.mlmWalletlogs.Where(x => x.EwalletRefund_TableID==item.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            //obj.ewallets.ToPagedList(pageNumber, pageSize)
            return View(Llist_EwalletRefundviewmodel);
        }

        //Ended by Rumana on 19/04/2019
    }
}
