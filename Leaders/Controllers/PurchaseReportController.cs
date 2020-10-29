using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Leaders.Controllers
{
    public class PurchaseReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();


        public ActionResult GetReport(string option, string search, int? pageNumber, string sort)
        {
            ViewBag.SortByName = string.IsNullOrEmpty(sort) ? "descending name" : "";
            ViewBag.SortByGender = sort == "Gender" ? "descending gender" : "Gender";
            ViewBag.SortByWallet = sort == "Wallet" ? "descending wallet" : "Wallet";
            ViewBag.SortByPointsCollecte = sort == "Points" ? "decending points" : "Points";

            Decimal usedWalletAmount = 0;

            long userID = Convert.ToInt64(Session["ID"]);
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            LeaderPurchaseReport objLeaderPurchaseReport = new LeaderPurchaseReport();
            List<PurchaseReportViewModel> objPurchaseList = objLeaderPurchaseReport.GetLeaderPurchaseReport(userID);// Added by sonali for call common method on 16-02-2019

            foreach (var item in objPurchaseList)
            {
                var customerID = db.CustomerOrders.Where(x => x.OrderCode == item.OrderCode).Select(y => y.ID).FirstOrDefault();

                item.DeliveryDate = db.OrderDeliveryScheduleDetails.Where(x => x.CustomerOrderID == customerID).Select(y => y.DeliveryDate).FirstOrDefault();

                ModelLayer.Models.CustomerOrder objCustomerOrder = db.CustomerOrders.Where(x => x.ID == customerID).FirstOrDefault();
                if (objCustomerOrder != null)
                {
                    usedWalletAmount = objCustomerOrder.MLMAmountUsed ?? 0;
                    item.WalletAmountUsed = Convert.ToDouble(usedWalletAmount);

                    if (objCustomerOrder.PaymentMode == "COD" && objCustomerOrder.MLMAmountUsed > 0)
                    {
                        item.paymentmode = "COD + EZEEMONEY";
                    }
                    else if (objCustomerOrder.PaymentMode == "ONLINE" && objCustomerOrder.MLMAmountUsed > 0)
                    {
                        item.paymentmode = "ONLINE + EZEEMONEY";
                    }
                }

            }
            //var idParam = new SqlParameter
            //{
            //    ParameterName = "UserID",
            //    Value = userID

            //};

            //List<PurchaseReportViewModel> objPurchaseList = db.Database.SqlQuery<PurchaseReportViewModel>("EXEC Purchase_Report_Select @UserID", idParam).OrderByDescending(x => x.OrderDate).ToList<PurchaseReportViewModel>();

            //foreach (var item in objPurchaseList)
            //{

            //    var customerID = db.CustomerOrders.Where(x => x.OrderCode == item.OrderCode).Select(y => y.ID).FirstOrDefault();

            //    var checkPartialStatus = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == customerID).Select(Y => Y.OrderStatus).Distinct().Count();


            //    if (item.OrdStatusNo == 7)
            //    {
            //        item.OrderStatus = "DELIVERED";

            //    }

            //    if (item.OrdStatusNo == 8)
            //    {
            //        if (checkPartialStatus > 1)
            //        {
            //            item.Status = "PARTIAL RETURNED";
            //            item.OrderStatus = "DELIVERED";
            //        }
            //        else if (checkPartialStatus == 1)
            //        {
            //            item.Status = "RETURNED";

            //        }
            //    }
            //    else if (item.OrdStatusNo == 9)
            //    {
            //        if (checkPartialStatus > 1)
            //        {
            //            item.Status = "PARTIAL CANCELLED";
            //            item.OrderStatus = "DELIVERED";
            //        }
            //        else if (checkPartialStatus == 1)
            //        {
            //            item.Status = "CANCELLED";
            //        }
            //    }
            //    else if (item.OrdStatusNo == 10)
            //    {
            //        item.Status = "ABANDONED";
            //        item.OrderStatus = "ABANDONED";
            //    }
            //    else
            //    {
            //        item.Status = item.OrderStatus;
            //    }
            //}
            switch (sort)
            {

                case "descending name":
                    objPurchaseList = objPurchaseList.OrderByDescending(x => x.OrderCode).ToList();
                    break;

                case "descending gender":
                    objPurchaseList = objPurchaseList.OrderByDescending(x => x.OrderAmt).ToList();
                    break;

                case "Gender":
                    objPurchaseList = objPurchaseList.OrderBy(x => x.OrderAmt).ToList();
                    break;
                case "descending wallet":
                    objPurchaseList = objPurchaseList.OrderByDescending(x => x.WalletAmountUsed).ToList();
                    break;

                case "Wallet":
                    objPurchaseList = objPurchaseList.OrderBy(x => x.WalletAmountUsed).ToList();
                    break;
                case "decending points":
                    objPurchaseList = objPurchaseList.OrderByDescending(x => x.TransactionPts).ToList();
                    break;
                case "Points":
                    objPurchaseList = objPurchaseList.OrderBy(x => x.TransactionPts).ToList();
                    break;
                default:
                    objPurchaseList = objPurchaseList.OrderByDescending(x => x.OrderCode).ToList();
                    break;

            }
            //return View(objPurchaseList.OrderByDescending(x=>x.OrderDate).ToPagedList(pageNumber ?? 1, 10));
            return View(objPurchaseList.ToPagedList(pageNumber ?? 1, 10));

        }

        public ActionResult SelectPurchaseReport(string option, string search, int? pageNumber, string sort)
        {

            ViewBag.SortByName = string.IsNullOrEmpty(sort) ? "descending name" : "";
            ViewBag.SortByGender = sort == "Gender" ? "descending gender" : "Gender";
            ViewBag.SortByPointsCollecte = sort == "Points" ? "decending points" : "Points";

            long userID = Convert.ToInt64(Session["ID"]);
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var idParam = new SqlParameter
            {
                ParameterName = "UserID",
                Value = userID

            };

            List<PurchaseReportViewModel> objPurchaseList = db.Database.SqlQuery<PurchaseReportViewModel>("EXEC Purchase_Report_Select @UserID", idParam).OrderByDescending(x => x.OrderDate).ToList<PurchaseReportViewModel>();

            switch (sort)
            {

                case "descending name":
                    objPurchaseList = objPurchaseList.OrderByDescending(x => x.OrderCode).ToList();
                    break;

                case "descending gender":
                    objPurchaseList = objPurchaseList.OrderByDescending(x => x.OrderAmt).ToList();
                    break;

                case "Gender":
                    objPurchaseList = objPurchaseList.OrderBy(x => x.OrderAmt).ToList();
                    break;
                case "decending points":
                    objPurchaseList = objPurchaseList.OrderByDescending(x => x.TransactionPts).ToList();
                    break;
                case "Points":
                    objPurchaseList = objPurchaseList.OrderBy(x => x.TransactionPts).ToList();
                    break;
                default:
                    objPurchaseList = objPurchaseList.OrderBy(x => x.OrderCode).ToList();
                    break;

            }
            return View(objPurchaseList.ToPagedList(pageNumber ?? 1, 10));

            // return View(objPurchaseList);
        }


        public ActionResult Index()
        {

            long userID = Convert.ToInt64(Session["ID"]);
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var idParam = new SqlParameter
            {
                ParameterName = "UserID",
                Value = userID

            };

            List<PurchaseReportViewModel> objPurchaseList = db.Database.SqlQuery<PurchaseReportViewModel>("EXEC Purchase_Report_Select @UserID", idParam).OrderByDescending(x => x.OrderDate).ToList<PurchaseReportViewModel>();

            foreach (var item in objPurchaseList)
            {
                if (item.OrdStatusNo == 7)
                {
                    item.OrderStatus = "Delivered";

                }

                if (item.OrdStatusNo == 8)
                {
                    item.Status = "Partial Returned";
                }
                else if (item.OrdStatusNo == 9)
                {
                    item.Status = "Partial Canceled";
                }
                else if (item.OrdStatusNo == 10)
                {
                    item.Status = "Abandoned";
                }
                else
                {
                    item.Status = "Success";
                }
            }

            return View(objPurchaseList);
        }


        public ActionResult OrderDetails(string orderCode, int? pageNumber, string sort)
        {
            ViewBag.SortByName = string.IsNullOrEmpty(sort) ? "descending name" : "";
            ViewBag.SortByGender = sort == "Gender" ? "descending gender" : "Gender";
            ViewBag.SortByPointsCollecte = sort == "Points" ? "decending points" : "Points";

            long userID = Convert.ToInt64(Session["ID"]);

            //string OrderCode = db.CustomerOrders.Where(x => x.UserLoginID == userID).Select(y => y.OrderCode).FirstOrDefault();
            List<LeadersOrderDetailsViewModel> OrderDetailsList = new List<LeadersOrderDetailsViewModel>();
            OrderDetailsList = db.CustomerOrders.Join(db.CustomerOrderDetails, co => co.ID, cod => cod.CustomerOrderID,
               (co, cod) => new
               {
                   orderCode = co.OrderCode,
                   Qty = cod.Qty,
                   Amount = cod.SaleRate,
                   TotalAmount = cod.TotalAmount,
                   BusinessPoints = cod.BusinessPoints,
                   ShopStockID = cod.ShopStockID,
                   OrderStatus = cod.OrderStatus

               }

                ).Join(db.ShopStocks, cod => cod.ShopStockID, sht => sht.ID,
                (cod, sht) => new
                {
                    orderCode = cod.orderCode,
                    Qty = cod.Qty,
                    Amount = cod.Amount,
                    TotalAmount = cod.TotalAmount,
                    BusinessPoints = cod.BusinessPoints,
                    ShopStockID = cod.ShopStockID,
                    OrderStatus = cod.OrderStatus,
                    ProductVarientID = sht.ProductVarientID,
                    MRP = sht.MRP

                }).Join(db.ProductVarients, sht => sht.ProductVarientID, pv => pv.ID,

                (sht, pv) => new LeadersOrderDetailsViewModel
                {
                    OrderCode = sht.orderCode,
                    Qty = sht.Qty,
                    Amount = sht.Amount,
                    TotalAmount = sht.TotalAmount,
                    BusinessPointsTotal = sht.BusinessPoints,
                    MRP = sht.MRP,
                    DiscountAmount = (sht.MRP - sht.Amount),
                    OrderStatus = sht.OrderStatus,
                    sku = sht.ProductVarientID,
                    ProductID = pv.ProductID


                }).ToList();
            OrderDetailsList = OrderDetailsList.Where(x => x.OrderCode == orderCode).ToList();
            foreach (var item in OrderDetailsList)
            {
                item.ItemImage = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                item.ItemDescription = db.Products.Where(x => x.ID == item.ProductID).Select(y => y.Name).FirstOrDefault();
                item.ItemDescription = item.ItemDescription.Replace("+", " "); //Added by Rumana on 23-04-2019
                if (item.OrderStatus == 0)
                {
                    item.OrdStatus = "Pending";
                }
                else if (item.OrderStatus == 1)
                {
                    item.OrdStatus = "Placed";

                }
                else if (item.OrderStatus == 2)
                {
                    item.OrdStatus = "Confirm";
                }

                else if (item.OrderStatus == 3)
                {
                    item.OrdStatus = "Packed";
                }
                else if (item.OrderStatus == 4)
                {
                    item.OrdStatus = "Dispach from shop";
                }
                else if (item.OrderStatus == 5)
                {
                    item.OrdStatus = "In Godown";
                }
                else if (item.OrderStatus == 6)
                {
                    item.OrdStatus = "Dispach from Godown";
                }
                else if (item.OrderStatus == 7)
                {
                    item.OrdStatus = "Delivered";
                }
                else if (item.OrderStatus == 8)
                {
                    item.OrdStatus = "Returned";
                }

                else if (item.OrderStatus == 9)
                {
                    item.OrdStatus = "Cancelled";
                }
                else
                {
                    item.OrdStatus = "Abandoned";
                }


            }

            ViewBag.OrderCode = orderCode;

            switch (sort)
            {

                case "descending name":
                    OrderDetailsList = OrderDetailsList.OrderByDescending(x => x.ItemDescription).ToList();
                    break;

                case "descending gender":
                    OrderDetailsList = OrderDetailsList.OrderByDescending(x => x.Qty).ToList();
                    break;

                case "Gender":
                    OrderDetailsList = OrderDetailsList.OrderBy(x => x.Qty).ToList();
                    break;
                case "decending points":
                    OrderDetailsList = OrderDetailsList.OrderByDescending(x => x.MRP).ToList();
                    break;
                case "Points":
                    OrderDetailsList = OrderDetailsList.OrderBy(x => x.MRP).ToList();
                    break;
                default:
                    OrderDetailsList = OrderDetailsList.OrderBy(x => x.OrderCode).ToList();
                    break;

            }
            return View(OrderDetailsList.ToPagedList(pageNumber ?? 1, 10));

            // return View(OrderDetailsList);
        }

        public ActionResult WalletSummary()
        {
            return View();
        }
    }


}