using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using BusinessLogicLayer;
using System.IO;
using System.Text;
using System.Transactions;
using Administrator.Models;
using System.Data.Entity.Validation;
using System.Data;

namespace Administrator.Controllers
{
    public class BulkUpdatePriceController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
         [SessionExpire]
        public ActionResult Index()
        {
            try
            {
               
                    ViewBag.ddlFranchiseID = new SelectList((from f in db.Franchises
                                                             join
                                                                 bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                                                             where f.IsActive == true
                                                             select new FranchiseDetail
                                                             {
                                                                 ID = f.ID,
                                                                 Name = bd.Name + "-" + f.ContactPerson
                                                             }).ToList(), "ID", "Name");
                    List<SelectListItem> ShopList = new List<SelectListItem>();
                    ViewBag.ddlMerchant = ShopList;
              
               

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BulkUpdatePrice][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BulkUpdatePrice][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View("Index", new List<StockManagementReportViewModel>());
          
        }
        //
        // GET: /BulkUpdatePrice/

        [HttpPost]
        public ActionResult GetExcelIndex(int ddlFranchiseID, int ddlMerchant, string hdnShopName)
        {
            List<StockManagementReportViewModel> stockManagementReportViewModel = new List<StockManagementReportViewModel>();
            try
            {
                this.SetShopList(ddlFranchiseID, ddlMerchant);
                ViewBag.ShopID = ddlMerchant;
                DataTable tblProduct = new DataTable();
                //tblProduct.Columns.Add("ProductID", typeof(long));
                tblProduct.Columns.Add("Sr.No.", typeof(int));
                tblProduct.Columns.Add("ShopStockID", typeof(long));
                tblProduct.Columns.Add("ProductName", typeof(string));
                //tblProduct.Columns.Add("ProductVariantID", typeof(long));
                tblProduct.Columns.Add("Color", typeof(string));
                tblProduct.Columns.Add("Size", typeof(string));
                tblProduct.Columns.Add("Dimension", typeof(string));
                tblProduct.Columns.Add("Material", typeof(string));
                tblProduct.Columns.Add("Quantity", typeof(int));
                tblProduct.Columns.Add("ReorderLevel", typeof(int));
                tblProduct.Columns.Add("Mrp", typeof(decimal));
                tblProduct.Columns.Add("SaleRate", typeof(decimal));
                tblProduct.Columns.Add("WholeSaleRate", typeof(decimal));
                tblProduct.Columns.Add("NewMrp", typeof(decimal));
                tblProduct.Columns.Add("NewSaleRate", typeof(decimal));
                tblProduct.Columns.Add("NewWholeSaleRate", typeof(decimal));

                stockManagementReportViewModel = (from pv in db.ProductVarients
                                                  join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                                                  join p in db.Products on pv.ProductID equals p.ID
                                                  join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                                                  join s in db.Shops on sp.ShopID equals s.ID
                                                  join c in db.Colors on pv.ColorID equals c.ID
                                                  join d in db.Dimensions on pv.DimensionID equals d.ID
                                                  join m in db.Materials on pv.MaterialID equals m.ID
                                                  join sz in db.Sizes on pv.SizeID equals sz.ID
                                                  where s.ID == ddlMerchant && p.IsActive == true && ss.IsActive == true
                                                  select new StockManagementReportViewModel
                                     {
                                         //ProductID = p.ID,
                                         ShopStockId = ss.ID,
                                         ProductName = p.Name,
                                         //VariantId = ss.ProductVarientID,
                                         ColorName = c.Name,
                                         SizeName = sz.Name,
                                         DimensionName = d.Name,
                                         MaterialName = m.Name,
                                         Quantity = ss.Qty,
                                         ReorderLevel = ss.ReorderLevel,
                                         Mrp = ss.MRP,
                                         SaleRate = ss.RetailerRate,
                                         WholeSaleRate = ss.WholeSaleRate


                                     }).OrderBy(x => x.ProductName).ToList();
                int i = 0;
                foreach (var row in stockManagementReportViewModel)
                {
                    i = i + 1;
                    tblProduct.LoadDataRow(new object[] {i, row.ShopStockId, row.ProductName, row.ColorName, row.SizeName, row.DimensionName, row.MaterialName
                   ,row.Quantity,row.ReorderLevel,row.Mrp,row.SaleRate,row.WholeSaleRate}, false);
                }
                BulkUpdateProductPrice bulkUpdateProductPrice = new BulkUpdateProductPrice(System.Web.HttpContext.Current.Server);
                bulkUpdateProductPrice.ExportData(tblProduct, hdnShopName);
                //Response.Write("Success");

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                //Response.Write("err");
                ModelState.AddModelError("Error", "There's Something wrong Exporting Excel!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BulkUpdatePrice][POST:GetExcelIndex]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

            }
            catch (Exception ex)
            {
                //Response.Write("err1");
                ModelState.AddModelError("Error", "There's Something wrong in Exporting Excel!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BulkUpdatePrice][POST:GetExcelIndex]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            //Response.Write("<script>alert('Hello');</script>");
            return View("Index", stockManagementReportViewModel);
        }
        public void SetShopList(long? FranchiseID, long? ShopID)
        {
            try
            {
                ViewBag.ddlMerchant = new SelectList(from s in db.Shops
                                                     where s.FranchiseID == FranchiseID && s.IsActive == true && s.IsLive == true
                                                     select new { ID = s.ID, Name = s.Name }, "ID", "Name", ShopID);

                ViewBag.ddlFranchiseID = new SelectList((from f in db.Franchises
                                                         join
                                                             bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                                                         where f.IsActive == true
                                                         select new FranchiseDetail
                                                         {
                                                             ID = f.ID,
                                                             Name = bd.Name + "-" + f.ContactPerson
                                                         }).ToList(), "ID", "Name", FranchiseID);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in set shoplist!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BulkController][POST:SetShopList]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in set shoplist!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BulkController][POST:SetShopList]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
        }
        public JsonResult GetMerchantByFranchiseId(int FranchiseID)
        {
            ViewBag.FranchiseID = FranchiseID;
            List<StateCityFranchiseMerchantViewModel> merchant = new List<StateCityFranchiseMerchantViewModel>();
            try
            {
                var lMerchant = (from s in db.Shops
                                 where s.FranchiseID == FranchiseID && s.IsActive == true && s.IsLive == true
                                 select new StateCityFranchiseMerchantViewModel
                                 {
                                     MerchantName = s.Name,
                                     MerchantID = s.ID
                                 }).Distinct();
                //ViewBag.ddlMerchant = lMerchant;
                foreach (var c in lMerchant)
                {
                    StateCityFranchiseMerchantViewModel SCFM = new StateCityFranchiseMerchantViewModel();
                    SCFM.MerchantID = c.MerchantID;
                    SCFM.MerchantName = c.MerchantName;
                    merchant.Add(SCFM);
                }
                //ViewBag.ddlMerchant = ;
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling Merchant Dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BulkController][POST:GetMerchantByFranchiseId]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling Merchant Dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BulkController][POST:GetMerchantByFranchiseId]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }


            return Json(merchant.Distinct().OrderBy(x => x.MerchantName).ToList(), JsonRequestBehavior.AllowGet);
        }

         [SessionExpire]
        public ActionResult UploadExcel(int? hdnShopID, int? hdnFranchiseID)
        {

            ViewBag.ddlFranchiseID = new SelectList((from f in db.Franchises
                                                     join
                                                         bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                                                     where f.IsActive == true
                                                     select new FranchiseDetail
                                                     {
                                                         ID = f.ID,
                                                         Name = bd.Name + "-" + f.ContactPerson
                                                     }).ToList(), "ID", "Name");
            List<SelectListItem> ShopList = new List<SelectListItem>();
            ViewBag.ddlMerchant = ShopList;
            return View("Index");
        }
         [SessionExpire]
        [HttpPost]
        public ActionResult UploadExcel(HttpPostedFileBase file, int hdnShopID, int hdnFranchiseID)
        {
            List<StockManagementReportViewModel> listproduct = new List<StockManagementReportViewModel>();
            try
            {
                DataTable dtproduct = new DataTable();
                BulkUpdateProductPrice objbulk = new BulkUpdateProductPrice(System.Web.HttpContext.Current.Server);
                StringBuilder validationMsg = new StringBuilder();
                dtproduct = objbulk.UpdatePrice(file, hdnShopID, out validationMsg);
                TempData["ExcelValidationFailed"] = validationMsg.ToString();
                if (dtproduct.Rows.Count > 0)
                {
                    listproduct = (from DataRow dr in dtproduct.Rows
                                   select new StockManagementReportViewModel()
                                   {
                                       ProductName = dr["Name"].ToString(),
                                       ColorName = dr["ColorName"].ToString(),
                                       SizeName = dr["SizeName"].ToString(),
                                       DimensionName = dr["DimensionName"].ToString(),
                                       MaterialName = dr["MaterialName"].ToString(),
                                       Mrp = Convert.ToDecimal(dr["Mrp"]),
                                       SaleRate = Convert.ToDecimal(dr["RetailerRate"]),
                                       WholeSaleRate = dr["WholeSaleRate"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["WholeSaleRate"])

                                   }).ToList();
                }
                this.SetShopList(hdnFranchiseID, hdnShopID);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Uploading Excel!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BulkUpdatePrice][POST:UploadExcel]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Uploading Details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BulkUpdatePrice][POST:UploadExcel]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            //return RedirectToAction("Index", "BulkUpdatePrice", new { FranchiseID = hdnFranchiseID, ShopID = hdnShopID, stockDetail = listproduct });
            return View("Index", listproduct);
        }



    }
}
