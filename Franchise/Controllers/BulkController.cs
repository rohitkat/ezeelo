using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogicLayer;
using System.Data;
using ModelLayer.Models.ViewModel;
using System.Text;
using ModelLayer.Models;
using Franchise.Models;

namespace Franchise.Controllers
{
    public class BulkController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        long FranchiseID = 0;
        [SessionExpire]
        // GET: /Bulk/
        /// <summary>
        /// Load list of Active franchise
        /// </summary>
        /// <param name="ShopID">if shop ID present, It displays bulk History for that shop</param>
        /// <returns></returns>
        public ActionResult Index(long? ShopID, string ShopName)
        {
            List<BulkLog> listBulk = new List<BulkLog>();
            TempData["ShopName"] = ShopName;
            try
            {
                FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
                //List<SelectListItem> ShopList = new List<SelectListItem>();
                ViewBag.ddlMerchant = new SelectList((from s in db.Shops
                                                      where s.FranchiseID == FranchiseID && s.IsActive == true && s.IsLive == true
                                                      select new { ID = s.ID, Name = s.Name }).OrderBy(x => x.Name).ToList(), "ID", "Name", ShopID);
                if (ShopID > 0)
                {

                    Session["BulkUploadShopID"] = ShopID;
                    Session["BulkUploadFranchiseID"] = FranchiseID;
                    UploadBulkProductExcel objBulkProject = new UploadBulkProductExcel(System.Web.HttpContext.Current.Server);

                    listBulk = objBulkProject.GetExcelUploadHstory(Convert.ToInt64(Session["BulkUploadShopID"]), 0);
                    this.SetShopList(FranchiseID, ShopID);
                    return View("Index", listBulk);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BulkController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BulkController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View("Index", new List<BulkLog>());

        }

        public void SetShopList(long? FranchiseID, long? ShopID)
        {
            try
            {
                ViewBag.ddlMerchant = new SelectList((from s in db.Shops
                                                      where s.FranchiseID == FranchiseID && s.IsActive == true && s.IsLive == true
                                                      select new { ID = s.ID, Name = s.Name }).OrderBy(x => x.Name).ToList(), "ID", "Name", ShopID);


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
         [SessionExpire]
        /// <summary>
        /// load product or stock details depending on bulk type 1: for product 2: fro Stock
        /// </summary>
        /// <param name="bulkLogID">bulkLogID</param>
        /// <param name="bulkType">bulkType 1: for product 2: fro Stock</param>
        /// <param name="RowID">RowID is required to scroll down the window on tr having this RowID</param>
        /// <returns></returns>
        public ActionResult Details(int bulkLogID, int bulkType, int? RowID)
        {
            try
            {
                if (RowID != null) ViewData["RowID"] = RowID; else ViewData["RowID"] = 0;

                UploadBulkProductExcel objBulkProject = new UploadBulkProductExcel(System.Web.HttpContext.Current.Server);
                if (bulkType == 1)
                {
                    List<BulkProductViewModel> listProductBulkDetail = objBulkProject.GetProductBulkDetails(bulkLogID);
                    return View("ProductBulkDetail", listProductBulkDetail);
                }
                else
                {
                    BulkStockListViewModel bulkStockList = objBulkProject.GetStockBulkDetails(bulkLogID);
                    return View("StockBulkDetails", bulkStockList);

                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BulkController][POST:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BulkController][POST:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        /// <summary>
        /// Upload Excel File
        /// </summary>
        /// <param name="file">Posted file</param>
        /// <returns></returns>
        /// 
         [SessionExpire]
        public ActionResult GetExcel(int ddlMerchant)
        {
            try
            {
                string hdnShopName = TempData["ShopName"].ToString();
                BulkDynamicExcel bulkDynamicExcel = new BulkDynamicExcel();
                bulkDynamicExcel.GenerateExcel(ddlMerchant, hdnShopName);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Uploading Excel!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BulkController][POST:GetExcel]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Uploading Excel!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BulkController][POST:GetExcel]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();

        }

        [HttpPost]
        [SessionExpire]
        public ActionResult Index(HttpPostedFileBase file)
        {
            try
            {
                UploadBulkProductExcel objBulkProject = new UploadBulkProductExcel(System.Web.HttpContext.Current.Server);
                DataTable dtProductsSaved = new DataTable();
                StringBuilder validationMsg = new StringBuilder();
                StringBuilder GbProductMsg = new StringBuilder();
                dtProductsSaved = objBulkProject.UploadProductWithStock(file, Convert.ToInt64(Session["BulkUploadShopID"]), out validationMsg, out GbProductMsg);

                TempData["ExcelValidationFailed"] = validationMsg.ToString();
                //changes by Harshada
                TempData["GbProductMsg"] = GbProductMsg.ToString();

                string controllerName = "Bulk", actionName = "Index";
                if (dtProductsSaved.Rows.Count > 0 && validationMsg.ToString().Trim().Equals(string.Empty))
                {
                    TempData["BulkImageTemplate"] = dtProductsSaved;

                    actionName = "Details";
                    //BulkType = 2 for StockBulkUpload
                    ViewBag.BulkType = "2";

                    //Get BulkLogID

                    int thisProductBulkLogID = Convert.ToInt32(dtProductsSaved.Rows[0]["ProductBulkLogID"]);
                    int thisStockBulkLogID = Convert.ToInt32(dtProductsSaved.Rows[0]["StockBulkLogID"]);

                    //genrate Image Feed Template for this BulkLogID
                    UploadBulkImages objImages = new UploadBulkImages(System.Web.HttpContext.Current.Server);
                    objImages.printpdf(objImages.GenerateImageTemplate(thisProductBulkLogID, (DataTable)TempData["BulkImageTemplate"]), "BulkLog-" + thisStockBulkLogID + "-ImageTemplate");
                    //generate Description Feed Template  for this BulkLogID
                    objImages.printpdf(objImages.GenerateDescriptionTemplate(thisProductBulkLogID, (DataTable)TempData["BulkImageTemplate"]), "BulkLog-" + thisProductBulkLogID + "-DescriptionTemplate");

                    return RedirectToAction(actionName, controllerName, new { bulkLogID = thisStockBulkLogID, bulkType = 2 });
                }
                else
                {

                    return Redirect(this.Request.UrlReferrer.ToString());
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BulkController][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BulkController][POST:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();

        }
        /// <summary>
        /// Upload images in bulk
        /// </summary>
        /// <param name="fileUpload">List of posted files</param>
        /// <param name="BulkStockLogID">Bulk log Id for stok</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Images(List<HttpPostedFileBase> fileUpload, string BulkStockLogID)
        {
            try
            {
                UploadBulkImages objBulkProject = new UploadBulkImages(System.Web.HttpContext.Current.Server);
                StringBuilder msg = new StringBuilder();
                int failCounter = 0;
                msg = objBulkProject.UploadProductImages(Convert.ToInt32(BulkStockLogID), Convert.ToInt64(Session["BulkUploadShopID"]), fileUpload, out failCounter);

                if (failCounter > 0) TempData["ProductImagesFailureMsg"] = msg.ToString();
                else TempData["ProductImagesSuccessMsg"] = msg.ToString();

                TempData.Keep();
                return Redirect(this.Request.UrlReferrer.ToString());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Uploading Images!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BulkController][POST:Images]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Uploading Images!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BulkController][POST:Images]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        //public ActionResult MultipleImages()
        //{
        //    //UploadBulkImages objBulkProject = new UploadBulkImages(System.Web.HttpContext.Current.Server);
        //    //StringBuilder msg = new StringBuilder();
        //    //msg = objBulkProject.UploadProductImages(model, Convert.ToInt64(Session["BulkUploadShopID"]), fileUpload);

        //    //string url = this.Request.UrlReferrer.ToString();

        //    //return Redirect(url);
        //    //return RedirectToAction("Details", "Bulk", new { bulkLogID = model.BulkLogID, bulkType = 2, RowID = 1});
        //    return View();

        //}

        //[HttpPost]
        //public ActionResult MultipleImages(ColorImages ColorImages)
        //{
        //    //UploadBulkImages objBulkProject = new UploadBulkImages(System.Web.HttpContext.Current.Server);
        //    //StringBuilder msg = new StringBuilder();
        //    //msg = objBulkProject.UploadProductImages(model, Convert.ToInt64(Session["BulkUploadShopID"]), fileUpload);

        //    //string url = this.Request.UrlReferrer.ToString();

        //    //return Redirect(url);
        //    //return RedirectToAction("Details", "Bulk", new { bulkLogID = model.BulkLogID, bulkType = 2, RowID = 1});
        //    return RedirectToAction("MultipleImages", "Bulk");

        //}

        /// <summary>
        /// Upload Images for a stock
        /// </summary>
        /// <param name="model">model contains product ID and color Name</param>
        /// <param name="fileUpload">List of posted images</param>
        /// <returns></returns>
        public ActionResult StockImages(BulkStockViewModel model, List<HttpPostedFileBase> fileUpload)
        {
            try
            {
                UploadBulkImages objBulkProject = new UploadBulkImages(System.Web.HttpContext.Current.Server);
                StringBuilder msg = new StringBuilder();
                int failCounter = 0;
                msg = objBulkProject.UploadStockImages(model.ProductID, Convert.ToInt64(Session["BulkUploadShopID"]), model.ColorName.ToLower(), fileUpload, out failCounter);

                TempData["UploadMessage"] = model.ExcelRowID + "$" + failCounter;

                TempData.Keep();
                return RedirectToAction("Details", "Bulk", new { bulkLogID = model.BulkLogID, bulkType = 2, RowID = model.ExcelRowID });
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Uploading Stock Images!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BulkController][POST:StockImages]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Uploading Stock Images!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BulkController][POST:StockImages]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();

        }

        /// <summary>
        /// Upload product description files(format: ProductID.html) in bulk as per the template
        /// </summary>
        /// <param name="fileUpload">list of posted files</param>
        /// <param name="BulkProductLogID">BulkProductLogID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DescriptionFiles(List<HttpPostedFileBase> fileUpload, string BulkProductLogID)
        {
            try
            {
                UploadBulkProductExcel objBulkProject = new UploadBulkProductExcel(System.Web.HttpContext.Current.Server);
                StringBuilder msg = new StringBuilder();
                int failCounter = 0;
                msg = objBulkProject.UploadDescriptionFile(Convert.ToInt32(BulkProductLogID), fileUpload, out failCounter);

                if (failCounter > 0) TempData["ProductDescFailureMsg"] = msg.ToString();
                else TempData["ProductDescSuccessMsg"] = msg.ToString();

                TempData.Keep();
                return Redirect(this.Request.UrlReferrer.ToString());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Uploading Description file!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BulkController][POST:DescriptionFiles]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Uploading Description file!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BulkController][POST:DescriptionFiles]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

    }
}
