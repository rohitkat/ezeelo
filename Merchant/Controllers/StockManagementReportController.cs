using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Collections.ObjectModel;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using Merchant.Models;
using System.Data;

namespace Merchant.Controllers
{
    public class StockManagementReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 300;
        //
        // GET: /StockManagementReport/
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        //[SessionExpire]
        //[CustomAuthorize(Roles = "StockManagementReport/CanRead")]
        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate, int StockStatus)
        {
            try
            {
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                long shopID = GetShopID();
                //ViewBag.approvedStatus = approvedStatus;
                int TotalCount = 0;
                int TotalPages = 0;
                int pageNumber = page;
                string from = fromDate.ToString();
                string[] f = from.Split('/');
                string[] ftime = f[2].Split(' ');
                DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                frmd = Convert.ToDateTime(frmd.ToShortDateString());

                //ViewBag.fromDate = frmd;
                string to = toDate.ToString();
                string[] t = to.Split('/');
                string[] ttime = t[2].Split(' ');
                DateTime tod = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);
                tod = Convert.ToDateTime(tod.ToShortDateString());

                //if (page == 1 && pagecount == 0)
                // {
                tod = tod.AddDays(1);
                //}

                //ViewBag.toDate = tod;
                ViewBag.uploadedProd = 0;
                List<StockManagementReportViewModel> lstStockManagementReport = new List<StockManagementReportViewModel>();
                //var StockManagementReport = ();
                if (StockStatus == 0)
                {
                    lstStockManagementReport = (from pv in db.ProductVarients
                                                join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                                                join p in db.Products on pv.ProductID equals p.ID
                                                join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                                                join s in db.Shops on sp.ShopID equals s.ID
                                                join c in db.Colors on pv.ColorID equals c.ID
                                                join d in db.Dimensions on pv.DimensionID equals d.ID
                                                join m in db.Materials on pv.MaterialID equals m.ID
                                                join sz in db.Sizes on pv.SizeID equals sz.ID
                                                where s.ID == shopID && (p.CreateDate >= frmd &&
                                                     p.CreateDate <= tod)
                                                select new StockManagementReportViewModel
                                                {
                                                    ProductName = p.Name,
                                                    Quantity = ss.Qty,
                                                    ReorderLevel = ss.ReorderLevel,
                                                    ColorName = c.Name,
                                                    SizeName = sz.Name,
                                                    DimensionName = d.Name,
                                                    MaterialName = m.Name,
                                                    SaleRate = ss.RetailerRate,
                                                    Mrp = ss.MRP,
                                                    ProductUploadDate = p.CreateDate,
                                                    ProductModifiedDate = ss.ModifyDate
                                                }).ToList();
                }
                else if (StockStatus == 1)
                {
                    lstStockManagementReport = (from pv in db.ProductVarients
                                                join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                                                join p in db.Products on pv.ProductID equals p.ID
                                                join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                                                join s in db.Shops on sp.ShopID equals s.ID
                                                join c in db.Colors on pv.ColorID equals c.ID
                                                join d in db.Dimensions on pv.DimensionID equals d.ID
                                                join m in db.Materials on pv.MaterialID equals m.ID
                                                join sz in db.Sizes on pv.SizeID equals sz.ID
                                                where s.ID == shopID && (p.CreateDate >= frmd &&
                                                     p.CreateDate <= tod) && ss.Qty > 0
                                                select new StockManagementReportViewModel
                                                {
                                                    ProductName = p.Name,
                                                    Quantity = ss.Qty,
                                                    ReorderLevel = ss.ReorderLevel,
                                                    ColorName = c.Name,
                                                    SizeName = sz.Name,
                                                    DimensionName = d.Name,
                                                    MaterialName = m.Name,
                                                    SaleRate = ss.RetailerRate,
                                                    Mrp = ss.MRP,
                                                    ProductUploadDate = p.CreateDate,
                                                    ProductModifiedDate = ss.ModifyDate
                                                }).ToList();
                }
                else
                {
                    lstStockManagementReport = (from pv in db.ProductVarients
                                                join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                                                join p in db.Products on pv.ProductID equals p.ID
                                                join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                                                join s in db.Shops on sp.ShopID equals s.ID
                                                join c in db.Colors on pv.ColorID equals c.ID
                                                join d in db.Dimensions on pv.DimensionID equals d.ID
                                                join m in db.Materials on pv.MaterialID equals m.ID
                                                join sz in db.Sizes on pv.SizeID equals sz.ID
                                                where s.ID == shopID && (p.CreateDate >= frmd &&
                                                     p.CreateDate <= tod) && ss.Qty == 0
                                                select new StockManagementReportViewModel
                                                {
                                                    ProductName = p.Name,
                                                    Quantity = ss.Qty,
                                                    ReorderLevel = ss.ReorderLevel,
                                                    ColorName = c.Name,
                                                    SizeName = sz.Name,
                                                    DimensionName = d.Name,
                                                    MaterialName = m.Name,
                                                    SaleRate = ss.RetailerRate,
                                                    Mrp = ss.MRP,
                                                    ProductUploadDate = p.CreateDate,
                                                    ProductModifiedDate = ss.ModifyDate
                                                }).ToList();
                }
                TotalCount = lstStockManagementReport.Count();
                ViewBag.TotalCount = TotalCount;
                lstStockManagementReport = lstStockManagementReport.OrderByDescending(x => x.ProductUploadDate).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.PageSize = lstStockManagementReport.Count;
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                ViewBag.TotalPages = TotalPages;
                return View(lstStockManagementReport.OrderBy(x=>x.ProductName).ToList());



            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Stock Management Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[StockManagementReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Stock Management Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[StockManagementReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();


        }


        private long GetShopID()
        {
            EzeeloDBContext db = new EzeeloDBContext();
            //Session["USER_LOGIN_ID"] = 2;
            long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
            long BusinessDetailID = 0;
            long ShopID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    BusinessDetailID = Convert.ToInt32(db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                    ShopID = Convert.ToInt32(db.Shops.Where(x => x.BusinessDetailID == BusinessDetailID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[StockManagementReportController][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
            }
            return ShopID;
        }
        public ActionResult Export(string fromDate, string toDate, int option, int print)
        {
            try
            {
                List<StockManagementReportViewModel> StockReport = new List<StockManagementReportViewModel>();
                StockReport = this.Getdata(fromDate, toDate);
                if (print == 1)
                {
                    return View("ForPrint", StockReport);
                }
                DataTable tblProduct = new DataTable();
                tblProduct.Columns.Add("Sr.No.", typeof(long));
                tblProduct.Columns.Add("Product Name", typeof(string));
                tblProduct.Columns.Add("Quantity", typeof(int));
                tblProduct.Columns.Add("Reorder Level", typeof(int));
                tblProduct.Columns.Add("Color", typeof(string));
                tblProduct.Columns.Add("Size", typeof(string));
                tblProduct.Columns.Add("Dimension", typeof(string));
                tblProduct.Columns.Add("Material", typeof(string));
                tblProduct.Columns.Add("Sale Rate", typeof(decimal));
                tblProduct.Columns.Add("Mrp", typeof(decimal));
                tblProduct.Columns.Add("Product Upload Date", typeof(DateTime));
                tblProduct.Columns.Add("Stock Modified Date", typeof(DateTime));
                tblProduct.Columns.Add("Stock Status", typeof(string));


                //tblProduct.Columns.Add("Network IP", typeof(string));
                int i = 0;
                ViewBag.stateID = new SelectList(db.States, "ID", "Name");
                foreach (var row in StockReport)
                {
                    i = i + 1;
                    string SStatus = "";
                    if (row.Quantity == 0)
                    {
                        SStatus = "Out Of Stock";
                    }
                    else
                    {
                        SStatus = "In Stock";
                    }
                    tblProduct.LoadDataRow(new object[] {i, row.ProductName, row.Quantity, row.ReorderLevel, row.ColorName, row.SizeName, row.DimensionName,row.MaterialName
                   ,row.SaleRate,row.Mrp,row.ProductUploadDate,row.ProductModifiedDate,SStatus}, false);
                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(tblProduct, "Stock Management Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(tblProduct, "Stock Management Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(tblProduct, "Stock Management Report");
                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Stock Management Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[StockManagementReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Stock Management Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[StockManagementReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View("Index");

        }


        public List<StockManagementReportViewModel> Getdata(string fromDate, string toDate)
        {
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            long shopID = GetShopID();
            string from = fromDate.ToString();
            string[] f = from.Split('/');
            string[] ftime = f[2].Split(' ');
            DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
            frmd = Convert.ToDateTime(frmd.ToShortDateString());


            string to = toDate.ToString();
            string[] t = to.Split('/');
            string[] ttime = t[2].Split(' ');
            DateTime tod = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);
            tod = Convert.ToDateTime(tod.ToShortDateString());

            tod = tod.AddDays(1);

            ViewBag.uploadedProd = 0;
            List<StockManagementReportViewModel> lstStockManagementReport = new List<StockManagementReportViewModel>();


            lstStockManagementReport = (from pv in db.ProductVarients
                                        join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                                        join p in db.Products on pv.ProductID equals p.ID
                                        join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                                        join s in db.Shops on sp.ShopID equals s.ID
                                        join c in db.Colors on pv.ColorID equals c.ID
                                        join d in db.Dimensions on pv.DimensionID equals d.ID
                                        join m in db.Materials on pv.MaterialID equals m.ID
                                        join sz in db.Sizes on pv.SizeID equals sz.ID
                                        where s.ID == shopID && (p.CreateDate >= frmd &&
                                             p.CreateDate <= tod)
                                        select new StockManagementReportViewModel
                                        {
                                            ProductName = p.Name,
                                            Quantity = ss.Qty,
                                            ReorderLevel = ss.ReorderLevel,
                                            ColorName = c.Name,
                                            SizeName = sz.Name,
                                            DimensionName = d.Name,
                                            MaterialName = m.Name,
                                            SaleRate = ss.RetailerRate,
                                            Mrp = ss.MRP,
                                            ProductUploadDate = p.CreateDate,
                                            ProductModifiedDate = ss.ModifyDate
                                        }).OrderBy(x=>x.ProductName).ToList();
            return lstStockManagementReport;


        }

    }
}

