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
using CRM.Models;


using PagedList;
using PagedList.Mvc;

using System.IO;
using System.Data;
using System.Reflection;
using System.Globalization;
using System.Text;
using BusinessLogicLayer;

 

namespace CRM.Controllers
{
    public class FVPatnerAccountsController : Controller
    {
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();

        //
        // GET: /FVPatnerAccounts/
        public ActionResult Index(FVAccountsPatnerViewModelList obj1)
        
        {
            FVAccountsPatnerViewModelList obj = new FVAccountsPatnerViewModelList();
            List<FVAccountsPatnerViewModel> list= new List<FVAccountsPatnerViewModel>();


            //long FrenchiseID = 0;
            //if (Session["FRANCHISE_ID"] != null)
            //{
            //    FrenchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
            //}


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
        

            //DateTime fDate = Convert.ToDateTime(obj1.FromDate);
            //DateTime tDate = Convert.ToDateTime(obj1.ToDate);
            //DateTime fdate = Convert.ToDateTime(obj1.FromDate, new DateTimeFormatInfo { FullDateTimePattern = "mm/dd/yyyy" });
            //DateTime tdate = Convert.ToDateTime(obj1.ToDate, new DateTimeFormatInfo { FullDateTimePattern = "mm/dd/yyyy" });

            ////orignal code ////


     //     var CreateDate=  Convert.ToDateTime("06-13-2012", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
     //       //var CreateDate = DateTime.Parse("03/17/2017 ");
     //       //var FrenchiseID = Convert.ToInt64(10);
     //       var list3 = db.Database.SqlQuery<FVAccountsPatnerViewModel>(
     //     "exec dbo.[FVAccountPatnerReport] @FranchiseID,@CreateDate",
     //     new Object[] { new SqlParameter("@FranchiseID", 10),
     //            new SqlParameter("@CreateDate", CreateDate)}
     //).ToList();

            DateTime? DeliveryDate = DateTime.Now;

            /////change code for date ////

          //  var FromDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
           // var ToDate=     Convert.ToDateTime("08-23-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });





            DateTime _FromDate = ConvertDateFromStringToDate(obj.FromDate);
            DateTime _ToDate = ConvertDateFromStringToDate(obj.ToDate);
            DateTime fDate1 = Convert.ToDateTime(_FromDate);
            DateTime tDate1 = Convert.ToDateTime(_ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);




            var TotalGstAmount=0;
            var TotalAmount = 0;


            var CreateDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
            //var CreateDate = DateTime.Parse("03/17/2017 ");
            //var FrenchiseID = Convert.ToInt64(10);
            var list3 = db.Database.SqlQuery<FVAccountsPatnerViewModel>(
          "exec dbo.[NewFVAccountPatnerReport] @CreateDate,@FromDate,@ToDate",
          new Object[] {

              //new SqlParameter("@FranchiseID",FrenchiseID),
                 new SqlParameter("@CreateDate", CreateDate),
           new SqlParameter("@FromDate", fDate1),
           new SqlParameter("@ToDate", tDate1)}
     ).ToList();


       
   
            foreach (var row in list3)
            {
                var Result1 = row.TotalAmount - row.TotalGstAmount;

            }

            //if (string.IsNullOrEmpty(obj1.FromDate))
            //{
            //    obj.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            //}
            //else
            //{
            //    obj.FromDate = obj1.FromDate;
            //}
            //if (string.IsNullOrEmpty(obj1.ToDate))
            //{
            //    obj.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
            //}
            //else
            //{
            //    obj.ToDate = obj1.ToDate;
            //}




            //var reportList = db.Database.SqlQuery<FVAccountsPatnerViewModel>("EXEC  FVAccountPatnerReport").ToList<FVAccountsPatnerViewModel>();
            obj.lFVAccountViewModel = list3;

            //DateTime FromDate_ = Convert.ToDateTime(fDate);
            //DateTime ToDate_ = Convert.ToDateTime(tDate);
            //var b = list.Where(x => x.Date >= FromDate && x.Date <= ToDate);
            //var QueryNew = list3.Where(x => x.FromDate_ >= fDate && x.ToDate_ <= tDate).ToList();
           // DateTime pDate = DateTime.ParseExact("05/28/2013 12:00:00 AM", "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            //var dateOnly = pDate.ToString("dd/MM/yyyy");





            //DateTime _FromDate = ConvertDateFromStringToDate(obj.FromDate);
            //DateTime _ToDate = ConvertDateFromStringToDate(obj.ToDate);
            //DateTime fDate1 = Convert.ToDateTime(_FromDate);
            //DateTime tDate1 = Convert.ToDateTime(_ToDate).AddHours(23);

         

            var result = obj.lFVAccountViewModel.Where(x => x.OrderPlacedDate >= fDate1 &&
                                       x.OrderPlacedDate <= tDate1).ToList();
            obj.lFVAccountViewModel = result.ToList();

            return View(obj);
        }


        [NonAction]
        public DateTime ConvertDateFromStringToDate(string _date)
        {

            DateTime datetime=DateTime.Now;
            String[] SplitStringDate=_date.Split('/');
            datetime=new DateTime (Convert.ToInt32(SplitStringDate[2]),Convert.ToInt32(SplitStringDate[1]),Convert.ToInt32(SplitStringDate[0]));
            return datetime;
        }

        public ActionResult Export(string FromDate, string ToDate, string option, FVAccountsPatnerViewModelList obj1) //, int? print
        {
            int optionvalue = Convert.ToInt32(1);
            try
            {
                FVAccountsPatnerViewModelList obj = new FVAccountsPatnerViewModelList();
                List<FVAccountsPatnerViewModel> list = new List<FVAccountsPatnerViewModel>();


                //long FrenchiseID = 0;
                //if (Session["FRANCHISE_ID"] != null)
                //{
                //    FrenchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
                //}




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


      
                //DateTime FromDate1 = Convert.ToDateTime(FromDate);
                //DateTime ToDate1 = Convert.ToDateTime(ToDate);

                //DateTime fDate = Convert.ToDateTime(obj.FromDate);
                //DateTime tDate = Convert.ToDateTime(obj.ToDate);
                //if (WarehouseID > 0)
                //{
                //    list = GetRecord(fDate, tDate, WarehouseID);
                //}
                ViewBag.PossibleSuppliers = new SelectList(db.Warehouses.ToList(), "ID", "Name");
                //ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
                //obj.lWastageReportViewModel = list.ToList();

                //obj.lFVAccountViewModel = lFVAccountViewModel.ToList();




                //var FromDate1 = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
                //var ToDate1 = Convert.ToDateTime("08-23-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });


                var FromDate1 = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
               var ToDate1 = Convert.ToDateTime("08-23-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
               DateTime _FromDate = ConvertDateFromStringToDate(FromDate);
               DateTime _ToDate = ConvertDateFromStringToDate(ToDate);
               DateTime fDate1 = Convert.ToDateTime(_FromDate);
               DateTime tDate1 = Convert.ToDateTime(_ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);


                var CreateDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
                //var CreateDate = DateTime.Parse("03/17/2017 ");
                //var FrenchiseID = Convert.ToInt64(10);
                var list3 = db.Database.SqlQuery<FVAccountsPatnerViewModel>(
              "exec dbo.[NewFVAccountPatnerReport]  @CreateDate,@FromDate,@ToDate",
              new Object[] {

                  //new SqlParameter("@FranchiseID", FrenchiseID),
                 new SqlParameter("@CreateDate", CreateDate),
           new SqlParameter("@FromDate", fDate1),
           new SqlParameter("@ToDate", tDate1)}
         ).ToList();



                obj.lFVAccountViewModel = list3;



                //DateTime _FromDate = ConvertDateFromStringToDate(FromDate);
                //DateTime _ToDate = ConvertDateFromStringToDate(ToDate);
                //DateTime fDate1 = Convert.ToDateTime(_FromDate);
                //DateTime tDate1 = Convert.ToDateTime(_ToDate).AddHours(23);



                //if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
                //{
                //    DateTime lFromDate = CommonFunctions.GetProperDateTime(FromDate);
                //    DateTime lToDate = CommonFunctions.GetProperDateTime(ToDate);

                //    var result1 = obj.lFVAccountViewModel.Where(x => x.OrderPlacedDate >= _FromDate &&
                //                       x.OrderPlacedDate <= _ToDate).ToList();
                //}

                var result = obj.lFVAccountViewModel.Where(x => x.OrderPlacedDate >= fDate1 &&
                                      x.OrderPlacedDate <= tDate1).ToList();
                obj.lFVAccountViewModel = result.ToList();

                //var result = obj.lFVAccountViewModel.Where(x => x.OrderPlacedDate >= _FromDate &&
                //                     x.OrderPlacedDate <= _ToDate).ToList();
                //obj.lFVAccountViewModel = result.ToList();

                if (optionvalue != null && optionvalue > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Sr.No.", typeof(long));
                    dt.Columns.Add("City", typeof(string));  
                    dt.Columns.Add("Franchise", typeof(string));
                    dt.Columns.Add("OrderCode", typeof(string));
                    dt.Columns.Add("ShopOrderCode", typeof(string));
                    dt.Columns.Add("Shop", typeof(string));
                    dt.Columns.Add("Customer", typeof(string));
                    //dt.Columns.Add("City", typeof(string));
                    dt.Columns.Add("PincodeID", typeof(int));
                    dt.Columns.Add("Email", typeof(string));
                    dt.Columns.Add("Mobile", typeof(string));
                    dt.Columns.Add("Product", typeof(string));
                    dt.Columns.Add("SKUID", typeof(long));
                    dt.Columns.Add("SKUUnit", typeof(string));
                    dt.Columns.Add("OrderPlacedDate", typeof(DateTime));
                    dt.Columns.Add("PaymentMode", typeof(string));
                    dt.Columns.Add(" SaleRate", typeof(decimal));
                    dt.Columns.Add("DeliveryDate", typeof(DateTime));
                    dt.Columns.Add("ShippingAddress", typeof(string));
                    dt.Columns.Add("JoiningDate", typeof(DateTime));
                    dt.Columns.Add("Category3", typeof(string));
                    dt.Columns.Add("Category2", typeof(string));
                    dt.Columns.Add("Category1", typeof(string));
                    dt.Columns.Add("HSNCode", typeof(string));
                  
                    dt.Columns.Add("CustomerOrderID", typeof(long));
                    dt.Columns.Add("Qty", typeof(int));
                    //dt.Columns.Add("SaleRate", typeof(string));
                    dt.Columns.Add("OrderStatus", typeof(string));
                    dt.Columns.Add("Taxable Amount", typeof(decimal));
                    //dt.Columns.Add("HSNCode", typeof(string));
                    //dt.Columns.Add("SaleRate", typeof(string));
                    //dt.Columns.Add("PaymentMode", typeof(string));

                    dt.Columns.Add("CGST %", typeof(decimal));
                    dt.Columns.Add("CGSTAmt", typeof(decimal));
                    dt.Columns.Add("SGST %", typeof(decimal));
                    dt.Columns.Add("SGSTAmt", typeof(decimal));
                   
               
                    dt.Columns.Add("TotalAmount", typeof(decimal));

                  
                    int i = 0;
                    foreach (var row in obj.lFVAccountViewModel)
                    {
                        i = i + 1;
                    


                        dt.LoadDataRow(new object[] { i,row.City,row.FranchiseName, row.OrderCode, row.ShopOrderCode,row.Shop ,
                       row.Customer, row.PincodeID,row.Email,row.Mobile,
                        row.Product ,row.SKUID, row.SKUUnit,row.OrderPlacedDate ,row.PaymentMode,row.SaleRate,
                          row.DeliveryDate,row.ShippingAddress ,row.JoiningDate ,row.Category3,
                         row.Category2 ,row.Category1 , row.HSNCode,row.CustomerOrderDetailID,row.Qty,
                     row.Status,row.GrossSaleAmount,row.CGST,
                         row.CGSTAmt,row.SGST,row.SGSTAmt,row.TotalAmount }, false);


                    }

                    ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (optionvalue == 1)
                    {
                        ExportExcelCsv.ExportToExcel(dt, "Index");
                    }
                    else if (optionvalue == 2)
                    {
                        ExportExcelCsv.ExportToCSV(dt,"Index");
                    }
                    else if (optionvalue == 3)
                    {
                        ExportExcelCsv.ExportToPDF(dt,"Index");
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


