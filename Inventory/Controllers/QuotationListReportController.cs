using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Inventory.Controllers
{
    public class QuotationListReportController : Controller
    {


        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController Obj_Common = new CommonController();

        
        // GET: /PurchaseRequisitionListReport/
        public ActionResult GetQuotationListReport(QuotationListReportViewModelList obj1)
        {
            QuotationListReportViewModelList obj = new QuotationListReportViewModelList();
            List<QuotationListReportViewModel> lQuotationListReportViewModel = new List<QuotationListReportViewModel>();


            if (string.IsNullOrEmpty(obj1.FromDate))
            {
                obj.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            }
            else
            {
                obj.FromDate = obj1.FromDate;
            }

            if (string.IsNullOrEmpty(obj1.ToDate))
            {
                obj.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
            }
            else
            {
                obj.ToDate = obj1.ToDate;
            }
            DateTime fDate = Convert.ToDateTime(obj.FromDate);
            DateTime tDate = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59);
            obj.WarehouseID = obj1.WarehouseID;
            
            //obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");

            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID && x.IsActive == true).Select(x => x.ID).FirstOrDefault();

            if (SupplierID != null && Convert.ToInt64(SupplierID) > 0)
            {
                lQuotationListReportViewModel = (from o in db.Quotations
                                                 join sl in db.QuotationSupplierLists on o.ID equals sl.QuotationID
                                                 join qt in db.QuotationItemDetails on o.ID equals qt.QuotationID
                                                 join p in db.Products on qt.ProductID equals p.ID
                                                 join pv in db.ProductVarients on qt.ProductVarientID equals pv.ID
                                                 join  su in db.Suppliers on sl.SupplierID equals su.ID
                                                 join  cu in db.Categories on p.CategoryID  equals  cu.ID  
                                                 join cs in db.Categories on  cu.ParentCategoryID equals cs.ID
                                                 join sce in db.Categories on cs.ParentCategoryID equals sce.ID

                      
                                                 join w in db.Warehouses on o.RequestFromWarehouseID equals w.ID
                                                 join s in db.Sizes on pv.SizeID equals s.ID
                                                 where sl.SupplierID == SupplierID && sl.IsActive == true
                                                 select new QuotationListReportViewModel
                                                 {
                                                     QuotationID = o.ID,
                                                     WarehouseName = w.Name,
                                                     SKUName = p.Name,
                                                     SKUID = pv.ID,
                                                     SKUUnit = s.Name,
                                                     HSNCode=p.HSNCode,
                                                     SupplierName=su.Name,
                                                     Category3=cu.Name,
                                                     Category2=cs.Name,
                                                     Category1=sce.Name,
                                                     QuantityRequired=qt.Quantity,
                                                     Remark=qt.Remark,
                                                    //STATUS = sl.IsReplied,
                                                     QuotationCode = o.QuotationCode,
                                                     IsReplied = sl.IsReplied,
                                                     TotalItems = db.QuotationItemDetails.Where(x => x.QuotationID == o.ID).Select(x => x.ID).Count(),
                                                     QuotationRequestDate = o.QuotationRequestDate,
                                                     ExpectedReplyDate = o.ExpectedReplyDate,
                                                     ReplyItemCount = db.QuotationReplyItems.Where(x => x.QuotationSupplierListID == sl.ID).Select(x => x.ID).Count(),
                                                     QuotationSupplierListID = sl.ID,
                                                     SupplierID = sl.SupplierID
                                                 }).OrderByDescending(o => o.QuotationRequestDate).OrderBy(sl => sl.IsReplied).ToList();

                //    List<QuotationListReportViewModel> lbc = new List<QuotationListReportViewModel>();
                //        lbc = db.Suppliers.Where(x => x.WarehouseID ).ToList();
                //        lbc.AddRange(lQuotationListReportViewModel);

                //       // QuotationListReportViewModel obj3 = new QuotationListReportViewModel();
                //    foreach (QuotationListReportViewModel item in lbc)
                //    {
                //            QuotationListReportViewModel obj3 = new QuotationListReportViewModel();


                //            var itemName = (from p in db.Products
                //                            join v in db.ProductVarients on p.ID equals v.ProductID
                //                            join s in db.Sizes on v.SizeID equals s.ID
                //                            where v.ID == item.ProductVarientID  
                //                       select new QuotationListReportViewModel { SKUName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();

                //        foreach (var i in itemName)
                //        {
                //            obj3.SKUName = i.SKUName.ToString();
                //            obj3.HSNCode = Convert.ToString(i.HSNCode);
                //        }

                //            lQuotationListReportViewModel.Add(obj3);
                //    }
                //}

            }

        obj.lQuotationListReportViewModel = lQuotationListReportViewModel.ToList();
             
            if (obj1.WarehouseID != 0)
            {
                obj.lQuotationListReportViewModel = lQuotationListReportViewModel.Where(p => p.WarehouseID == obj.WarehouseID).ToList();
            }


            if (obj1.FromDate == null && obj1.ToDate == null)
            {
                if (obj1.WarehouseID != 0)
                {
                    obj.lQuotationListReportViewModel = lQuotationListReportViewModel.Where(p => p.WarehouseID == obj.WarehouseID).ToList();
                }
            }
            else
            {
                DateTime fDate1 = Convert.ToDateTime(obj.FromDate);
                DateTime tDate1 = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59);


                var result = obj.lQuotationListReportViewModel.Where(x => x.CreateDate >= fDate1 &&
                                      x.CreateDate <= tDate1).ToList();
                obj.lQuotationListReportViewModel = result.ToList();

            }
            ViewBag.PossibleWarehouses = Obj_Common.GetFVList(WarehouseID);
          

            return View(obj);
        }


        public ActionResult Export(string FromDate, string ToDate, int option, long WarehouseID) //, int? print
        {
            try
            {
                QuotationListReportViewModelList obj = new QuotationListReportViewModelList();
                List<QuotationListReportViewModel> list = new List<QuotationListReportViewModel>();


                // long WarehouseID = 0;
                //if (Session["WarehouseID"] != null)
                //{
                //    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                //}


                if (string.IsNullOrEmpty(FromDate))
                {
                    obj.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
                }
                else
                {
                    obj.FromDate = FromDate;
                }

                if (string.IsNullOrEmpty(ToDate))
                {
                    obj.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
                }
                else
                {
                    obj.ToDate = ToDate;
                }

                DateTime fDate = Convert.ToDateTime(obj.FromDate);
                DateTime tDate = Convert.ToDateTime(obj.ToDate);

                if (Session["USER_NAME"] != null)
                { }
                else
                {
                    return RedirectToAction("Index", "Login");
                }



                if (string.IsNullOrEmpty(FromDate))
                {
                    obj.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
                }
                else
                {
                    obj.FromDate = FromDate;
                }

                if (string.IsNullOrEmpty(ToDate))
                {
                    obj.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
                }
                else
                {
                    obj.ToDate = ToDate;
                }

                obj.WarehouseID = WarehouseID;
                
                obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");
                
                obj.WarehouseID = obj.WarehouseID;
             


                //   var FromDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
                //    var ToDate=     Convert.ToDateTime("08-23-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });


                // var reportList = db.Database.SqlQuery<ReorderLevelReportViewModel>("exec ReorderLevelReport").ToList();


                var reportList = db.Database.SqlQuery<QuotationListReportViewModel>(
                 "exec dbo.[QuotationListReport] @WarehouseID,@FromDate,@ToDate",
                    new Object[] { new SqlParameter("@WarehouseID", WarehouseID) ,
            new SqlParameter("@FromDate", fDate),
           new SqlParameter("@ToDate", tDate)}
         ).ToList();

                obj.lQuotationListReportViewModel = reportList;

                if (obj.WarehouseID != null)
                {
                    obj.lQuotationListReportViewModel= obj.lQuotationListReportViewModel.Where(p => p.WarehouseID == obj.WarehouseID).ToList();
                }

                DateTime fDate1 = Convert.ToDateTime(obj.FromDate);
                DateTime tDate1 = Convert.ToDateTime(obj.ToDate).AddHours(23);


                var result = obj.lQuotationListReportViewModel.Where(x => x.CreateDate >= fDate1 &&
                                      x.CreateDate <= tDate1).ToList();
                obj.lQuotationListReportViewModel= result.ToList();


                if (option != null && option > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Sr.No.", typeof(long));
                    dt.Columns.Add("SKUID", typeof(long));
                    dt.Columns.Add("SKUName", typeof(string));
                
                    dt.Columns.Add("SKUUnit", typeof(string));
                    dt.Columns.Add("HSNCode", typeof(string));
                    dt.Columns.Add("QuotationCode", typeof(string));
                    dt.Columns.Add("Supplier", typeof(string));
                    dt.Columns.Add("Category1", typeof(string));
                    dt.Columns.Add("Category2", typeof(string));
                    dt.Columns.Add("Category3", typeof(string));
                  

                    //dt.Columns.Add("ProductVarient", typeof(string));
                    dt.Columns.Add("QuantityRequired", typeof(int));
                  
                    dt.Columns.Add("QuotationRequestDate", typeof(DateTime));
                    dt.Columns.Add("ExpectedReplyDate", typeof(DateTime));
                    dt.Columns.Add("STATUS", typeof(string));
                    dt.Columns.Add("Remark", typeof(string));
                    dt.Columns.Add("CreateDate", typeof(string));

                    int i = 0;
                    foreach (var row in obj.lQuotationListReportViewModel)
                    {
                        i = i + 1;
                        dt.LoadDataRow(new object[] { i,row.SKUID, row.SKUName, row.SKUUnit,row.HSNCode, row.QuotationCode,row.SupplierName ,
                        row.Category1, row.Category2,row.Category3,row.QuantityRequired,row.ExpectedReplyDate,row.STATUS,row.Remark,row.CreateDate}, false);
                    }

                    ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (option == 1)
                    {
                        ExportExcelCsv.ExportToExcel(dt, "GetQuotationListReport");
                    }
                    else if (option == 2)
                    {
                        ExportExcelCsv.ExportToCSV(dt, "GetQuotationListReport");
                    }
                    else if (option == 3)
                    {
                        ExportExcelCsv.ExportToPDF(dt, "GetQuotationListReport");
                    }
                }
                else
                {
                    return View("GetQuotationListReport", obj);
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