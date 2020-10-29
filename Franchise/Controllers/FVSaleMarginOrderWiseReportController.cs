using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;

using System.Data.SqlClient;
using System.Data.SqlTypes;

using BusinessLogicLayer.Account;
using ModelLayer.Models.ViewModel.Report.Account;
using Franchise.Models;


using PagedList;
using PagedList.Mvc;
using ClosedXML.Excel;
using System.IO;
using System.Data;
using System.Reflection;
using System.Globalization;
using System.Text;
using BusinessLogicLayer;


namespace Franchise.Controllers
{
    public class FVSaleMarginOrderWiseReportController : Controller
    {
        
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();


        // GET: FVSaleMarginOrderWiseReport
        public ActionResult Index(FVSaleMarginOrderWiseReportViewModelList obj1)
        {
            FVSaleMarginOrderWiseReportViewModelList obj = new FVSaleMarginOrderWiseReportViewModelList();

            List<FVSaleMarginOrderWiseReportViewModel> list = new List<FVSaleMarginOrderWiseReportViewModel>();

            long FrenchiseID = 0;
            if (Session["FRANCHISE_ID"] != null)
            {
                FrenchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
            }

            if (string.IsNullOrEmpty(obj1.FromDate))
            {
                obj.FromDate = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
            }
            else
            {
                obj.FromDate = obj1.FromDate;
            }

            if (string.IsNullOrEmpty(obj1.ToDate))
            {
                obj.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("dd/MM/yyyy");
            }
            else
            {
                obj.ToDate = obj1.ToDate;
            }


            DateTime _FromDate = ConvertDateFromStringToDate(obj.FromDate);
            DateTime _ToDate = ConvertDateFromStringToDate(obj.ToDate);
            DateTime fDate1 = Convert.ToDateTime(_FromDate);
            DateTime tDate1 = Convert.ToDateTime(_ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);

            var TotalGstAmount = 0;
            var TotalAmount = 0;


            var CreateDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
            //var CreateDate = DateTime.Parse("03/17/2017 ");
            //var FrenchiseID = Convert.ToInt64(10);
            var list3 = db.Database.SqlQuery<FVSaleMarginOrderWiseReportViewModel>(
          "exec dbo.[FVSaleMarginOrderWiseReport] @FranchiseID,@FromDate,@ToDate",
          new Object[] { new SqlParameter("@FranchiseID",FrenchiseID),

           new SqlParameter("@FromDate", fDate1),
           new SqlParameter("@ToDate", tDate1)}
     ).ToList();


            obj.lFVSaleMarginOrderWiseReportViewModel = list3;

            //var result = obj.lFVSaleMarginOrderWiseReportViewModel.Where(x => x.CREA >= fDate1 &&
            //                           x.OrderPlacedDate <= tDate1).ToList();
            //obj.lFVSaleMarginOrderWiseReportViewModel = result.ToList();

            return View(obj);

        }



        [NonAction]
        public DateTime ConvertDateFromStringToDate(string _date)
        {

            DateTime datetime = DateTime.Now;
            String[] SplitStringDate = _date.Split('/');
            datetime = new DateTime(Convert.ToInt32(SplitStringDate[2]), Convert.ToInt32(SplitStringDate[1]), Convert.ToInt32(SplitStringDate[0]));
            return datetime;
        }

        public ActionResult Export(string FromDate, string ToDate, string option, FVSaleMarginOrderWiseReportViewModelList obj1) //, int? print
        {
            int optionvalue = Convert.ToInt32(1);
            try
            {
                FVSaleMarginOrderWiseReportViewModelList obj = new FVSaleMarginOrderWiseReportViewModelList();

                List<FVSaleMarginOrderWiseReportViewModel> list = new List<FVSaleMarginOrderWiseReportViewModel>();

                long FrenchiseID = 0;
                if (Session["FRANCHISE_ID"] != null)
                {
                    FrenchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
                }




                if (string.IsNullOrEmpty(obj.FromDate))
                {
                    obj.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
                }
                else
                {
                    obj.FromDate = obj1.FromDate;
                }
                if (string.IsNullOrEmpty(obj1.ToDate))
                {
                    obj1.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
                }
                else
                {
                    obj.ToDate = obj1.ToDate;
                }


                ViewBag.PossibleSuppliers = new SelectList(db.Warehouses.ToList(), "ID", "Name");

                var FromDate1 = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
                var ToDate1 = Convert.ToDateTime("08-23-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
                DateTime _FromDate = ConvertDateFromStringToDate(FromDate);
                DateTime _ToDate = ConvertDateFromStringToDate(ToDate);
                DateTime fDate1 = Convert.ToDateTime(_FromDate);
                DateTime tDate1 = Convert.ToDateTime(_ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);


                var CreateDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });

                var list3 = db.Database.SqlQuery<FVSaleMarginOrderWiseReportViewModel>(
          "exec dbo.[FVSaleMarginOrderWiseReport] @FranchiseID,@FromDate,@ToDate",
          new Object[] { new SqlParameter("@FranchiseID",FrenchiseID),

           new SqlParameter("@FromDate", fDate1),
           new SqlParameter("@ToDate", tDate1)}
     ).ToList();

                obj.lFVSaleMarginOrderWiseReportViewModel = list3;
                //var result = obj.lFVSaleMarginOrderWiseReportViewModel.Where(x => x.OrderPlacedDate >= fDate1 &&
                //                      x.OrderPlacedDate <= tDate1).ToList();
                //obj.lFVSaleMarginOrderWiseReportViewModel = result.ToList();


                if (optionvalue != null && optionvalue > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Sr.No.", typeof(long));
                    dt.Columns.Add("Order Date", typeof(DateTime));

                    dt.Columns.Add("Order Number", typeof(string));
                    dt.Columns.Add("Order Delivery Date", typeof(DateTime));
                    //dt.Columns.Add("ShopOrderCode", typeof(string));
                    //dt.Columns.Add("BatchCode", typeof(string)); //new 

                    //dt.Columns.Add("Shop", typeof(string));
                    dt.Columns.Add("Customer Name", typeof(string));
                    
                    dt.Columns.Add("PaymentMode", typeof(string));
                    dt.Columns.Add("Product Total Amount", typeof(decimal));
                    dt.Columns.Add("Delivery Charge", typeof(decimal));
                  
                   dt.Columns.Add("Wallet Amount Used", typeof(decimal));

                    dt.Columns.Add(" MarginAmount", typeof(decimal));
                    //dt.Columns.Add("SKUID", typeof(long));
                    //dt.Columns.Add("HSNCode", typeof(string));
                    //dt.Columns.Add("SKU Name", typeof(string));
                    //dt.Columns.Add("SKUUnit", typeof(string));
                    //dt.Columns.Add("BrandName", typeof(string));
                    //dt.Columns.Add("Qty", typeof(int));
                    //dt.Columns.Add("PurchaseAmount", typeof(decimal));
                    //dt.Columns.Add(" SaleRate", typeof(decimal));
                    //dt.Columns.Add(" BuyRatePerUnit", typeof(decimal));

                    //dt.Columns.Add("MRP", typeof(decimal));
                    //dt.Columns.Add("Taxable Amount", typeof(decimal));
                    //dt.Columns.Add("CGST %", typeof(decimal));
                    //dt.Columns.Add("CGSTAmt", typeof(decimal));
                    //dt.Columns.Add("SGST %", typeof(decimal));
                    //dt.Columns.Add("SGSTAmt", typeof(decimal));

                    dt.Columns.Add("Retail Points on Order", typeof(decimal));
                    //dt.Columns.Add("DV Invoice Code", typeof(string));
                    //dt.Columns.Add("DV Invoice Date", typeof(DateTime));

                    //dt.Columns.Add("InitialQuantity", typeof(int));
                    //dt.Columns.Add("AvailableInStock", typeof(int));

                    //dt.Columns.Add("Received Date", typeof(DateTime));
                    //dt.Columns.Add("Supplier Name", typeof(string));
                    //dt.Columns.Add("Last Purchase Date", typeof(DateTime));
                    //dt.Columns.Add("PO Code", typeof(string));

                    //dt.Columns.Add("City", typeof(string));
                    //dt.Columns.Add("PincodeID", typeof(int));
                    //dt.Columns.Add("Email", typeof(string));
                    //dt.Columns.Add("Mobile", typeof(string));

                    //dt.Columns.Add("ShippingAddress", typeof(string));
                    //dt.Columns.Add("JoiningDate", typeof(DateTime));


                    //dt.Columns.Add("CustomerOrderID", typeof(long));

                    //dt.Columns.Add("SaleRate", typeof(string));
                    dt.Columns.Add("OrderStatus", typeof(string));

                    int i = 0;
                    foreach (var row in obj.lFVSaleMarginOrderWiseReportViewModel)
                    {
                        i = i + 1;



                        dt.LoadDataRow(new object[] { i,    row.OrderPlacedDate ,row.OrderCode,
                            row.DeliveryDate,
                          //row.ShopOrderCode,
                          //  row.BatchCode,
                          //  row.Shop ,
                       row.Customer,
                       row.PaymentMode,

                         row.PayableAmount,
                         row.DeliveryCharge,

                        
                       row.WalletAmountUsed,
                        row.MarginAmount,
                      // row.SKUID,
                       //row.HSNCode,
                       //  row.Product ,
                       //  row.SKUUnit,
                       //  row.BrandName,
                       //  row.Qty,
                       //  row.PurchaseAmount,
                       //  row.SaleRate,
                       //  row.BuyRatePerUnit,
                       
                         //row.MRP,
                         //row.GrossSaleAmount,
                         //row.CGST,
                         //row.CGSTAmt,
                         //   row.SGST,
                         //   row.SGSTAmt,
                          
                            row.RetailPointsonOrder,
                         //   row.DVInvoiceCode,
                         //   row.DVInvoiceDate,
                         //   row.InitialQuantity,
                         //   row.AvailableQuantity,
                         //   row.ReceivedDate,
                         //   row.SupplierName,
                         //   row.LastPurchaseDate,
                         //   row.POCode,





                         //   row.City,row.PincodeID,row.Email,row.Mobile,

                         //row.ShippingAddress ,row.JoiningDate , row.CustomerOrderDetailID,
                   /*  row.OrderStatus,*/}, false);


                    }

                    ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (optionvalue == 1)
                    {
                        ExportExcelCsv.ExportToExcel(dt, "Index");
                    }
                    else if (optionvalue == 2)
                    {
                        ExportExcelCsv.ExportToCSV(dt, "Index");
                    }
                    else if (optionvalue == 3)
                    {
                        ExportExcelCsv.ExportToPDF(dt, "Index");
                    }
                }
                else
                {
                    return RedirectToAction("Index", obj);
                }

            }
            catch (Exception ex)
            {
                new Exception("Some unknown error encountered!");
            }
            return View();
        }


    }
}




