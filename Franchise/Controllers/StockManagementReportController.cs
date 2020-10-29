using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Collections.ObjectModel;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using Franchise.Models;
using System.Data;



namespace Franchise.Controllers
{
    public class StockManagementReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 300;
        //
        // GET: /StockManagementReport/
        //[SessionExpire]
        //[CustomAuthorize(Roles = "StockManagementReport/CanRead")]
        public ActionResult Index()
        {
            long FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);//GetFranchiseID();
            try
            {
                List<ShopViewModel> lShop = (from s in db.Shops
                                             join sp in db.ShopProducts on s.ID equals sp.ShopID
                                             join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                                             where s.BusinessDetail.UserLogin.IsLocked == false && s.IsActive == true
                                             && s.BusinessDetail.BusinessType.Prefix == "GBMR" && s.FranchiseID == FranchiseID
                                             select new ShopViewModel
                                             {
                                                 ID = s.ID,
                                                 Name = s.Name.Trim(),
                                                 FranchiseID = s.FranchiseID,
                                                 NonApproveProductCount = ss.Qty
                                             }).Distinct().OrderBy(x => x.Name).ToList();

                return View("MerchantList", lShop);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[StockManagementReportController][GET:MerchantList]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[StockManagementReportController][GET:MerchantList]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
          
            return View();
        }

        public ActionResult GetView(long shopId, string shopName)
        {
            ViewBag.shopID = shopId;
            ViewBag.shopName = shopName;
            return View("Index");
        }
        //[HttpPost]
        //[SessionExpire]
        //[CustomAuthorize(Roles = "StockManagementReport/CanRead")]
        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate, long? shopID, int StockStatus,int print)
        {
            try
            {
                long FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //GetFranchiseID();
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                ViewBag.shopID = shopID;
                ViewBag.StockStatus = StockStatus;
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
                                                where s.ID == shopID && (ss.ModifyDate >= frmd &&
                                                     ss.ModifyDate <= tod)
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
                                                where s.ID == shopID && (ss.ModifyDate >= frmd &&
                                                     ss.ModifyDate <= tod) && ss.Qty > 0
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
                                                where s.ID == shopID && (ss.ModifyDate >= frmd &&
                                                     ss.ModifyDate <= tod) && ss.Qty == 0
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
                if (print == 1)
                {
                    return View("ForPrint", lstStockManagementReport);
                }
                TotalCount = lstStockManagementReport.Count();
                ViewBag.TotalCount = TotalCount;
                lstStockManagementReport = lstStockManagementReport.OrderByDescending(x => x.ProductUploadDate).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.PageSize = lstStockManagementReport.Count;
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                ViewBag.TotalPages = TotalPages;
                return View(lstStockManagementReport);



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

        public ActionResult Export(string fromDate, string toDate, long? shopID, int StockStatus, int option, int print)
        {
            try
            {
                List<StockManagementReportViewModel> StockReport = new List<StockManagementReportViewModel>();
                StockReport = this.Getdata(fromDate, toDate, shopID, StockStatus);
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
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Stock Management Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[StockManagementReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View("Index");

        }


        public List<StockManagementReportViewModel> Getdata(string fromDate, string toDate, long? shopID, int StockStatus)
        {
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;

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
                                            where s.ID == shopID && (ss.ModifyDate >= frmd &&
                                                 ss.ModifyDate <= tod)
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
                                            where s.ID == shopID && (ss.ModifyDate >= frmd &&
                                                 ss.ModifyDate <= tod) && ss.Qty > 0
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
                                            where s.ID == shopID && (ss.ModifyDate >= frmd &&
                                                 ss.ModifyDate <= tod) && ss.Qty == 0
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
            return lstStockManagementReport;
        }
     
    }
}
