using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Data;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using Administrator.Models;

namespace Administrator.Controllers
{
    public class CategoryWiseSaleAnalisysMISController : Controller
    {
        //
        // GET: /CategoryWiseSaleAnalisysMIS/

        EzeeloDBContext db = new EzeeloDBContext();
        
        /// <summary>
        /// Developed By :- Pradnyakar Badge
        /// Purpose :- To retrive list of category with shop and their sale status with sale amount
        /// </summary>
        /// <returns>View</returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "CategoryWiseSaleAnalisysMIS/CanRead")]
        public ActionResult Index()
        {
            List<ModelLayer.Models.ViewModel.DropdownList> ls = new List<ModelLayer.Models.ViewModel.DropdownList>();
            ViewBag.FranchiseID = new SelectList(db.Franchises.Where(x => x.IsActive == true), "ID", "ContactPerson");
            ViewBag.ShopID = new SelectList(ls, "ID", "Name");
            ViewBag.CategoryID = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true), "ID", "Name");
            return View();
        }

       /// <summary>
       /// Developed by: Pradnyakar Badge
       /// it is the webMethod To retrive data from this methored with combination of given parameter
       /// Call by Ajax 
       /// </summary>
       /// <param name="paramObject"></param>
       /// <returns></returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "CategoryWiseSaleAnalisysMIS/CanRead")]
        public ActionResult GetReport(CategoryWiseSaleAnalisysMISLocal paramObject)
        {
            List<SaleAnalisysMISViewModel> ls = new List<SaleAnalisysMISViewModel>();
            List<object> paramValues = new List<object>();
            DataTable dt = new DataTable();
            BusinessLogicLayer.MISReports obj = new BusinessLogicLayer.MISReports();

            DateTime fDate = new DateTime();
            DateTime tDate = new DateTime();

            //Convert From Date string To Datetime
            if (!String.IsNullOrEmpty(paramObject.fromDate))
            {
                string from = paramObject.fromDate.ToString();
                string[] f = from.Split('/');
                string[] ftime = f[2].Split(' ');
                DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                fDate = Convert.ToDateTime(frmd.ToShortDateString());
                paramValues.Add(fDate);
            }
            else
            {
                paramValues.Add(DBNull.Value);
            }
            //Convert to Date string To Datetime
            if (!String.IsNullOrEmpty(paramObject.toDate))
            {
                string to = paramObject.toDate.ToString();
                string[] f = to.Split('/');
                string[] ftime = f[2].Split(' ');
                DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                tDate = Convert.ToDateTime(frmd.ToShortDateString());
                paramValues.Add(tDate);
            }
            else
            {
                paramValues.Add(DBNull.Value);
            }
            paramValues.Add(paramObject.FranchiseID);
            paramValues.Add(paramObject.merchantID);
            paramValues.Add(paramObject.CategoryID);

            //Call Store Procedure
            ls = obj.ShopSaleAnalisys_MIS(paramValues, System.Web.HttpContext.Current.Server);

            return PartialView("_CategoryWiseSaleAnalisysMISGetReport", ls);
            //return Json(ls, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// to retrive list of Shop within provided franchise parameter
        /// </summary>
        /// <param name="franchiseID"></param>
        /// <returns></returns>
        public JsonResult getShop(int franchiseID)
        {
            List<ModelLayer.Models.ViewModel.DropdownList> ls = new List<ModelLayer.Models.ViewModel.DropdownList>();
            ls = (from n in db.Shops
                  where n.IsActive == true && n.IsLive == true && n.FranchiseID == franchiseID 
                  select new ModelLayer.Models.ViewModel.DropdownList
                  {
                      ID = n.ID,
                      Name = n.Name
                  }).ToList();
            return (Json(ls, JsonRequestBehavior.AllowGet));
        }

        /// <summary>
        /// to expoert data in desired format 
        /// </summary>
        /// <param name="FranchiseID">franchise ID</param>
        /// <param name="merchantID">Shop ID</param>
        /// <param name="fromDate">from Date</param>
        /// <param name="toDate">To Date</param>
        /// <param name="CategoryID">Category ID</param>
        /// <param name="option">EXCEL/CSV/PDF</param>
        /// <returns></returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "CategoryWiseSaleAnalisysMIS/CanExport")]
        public ActionResult Export(int? FranchiseID, Int64? merchantID, string fromDate, string toDate, int? CategoryID, int option)
        {
            try
            {
                List<ModelLayer.Models.ViewModel.DropdownList> lsVieBag = new List<ModelLayer.Models.ViewModel.DropdownList>();
                ViewBag.FranchiseID = new SelectList(db.Franchises.Where(x => x.IsActive == true), "ID", "ContactPerson");
                ViewBag.ShopID = new SelectList(lsVieBag, "ID", "Name");
                ViewBag.CategoryID = new SelectList(db.Categories.Where(x => x.Level == 1 && x.IsActive == true), "ID", "Name");

                List<SaleAnalisysMISViewModel> ls = new List<SaleAnalisysMISViewModel>();
                List<object> paramValues = new List<object>();

                BusinessLogicLayer.MISReports obj = new BusinessLogicLayer.MISReports();

                DateTime fDate = new DateTime();
                DateTime tDate = new DateTime();

                if (!String.IsNullOrEmpty(fromDate))
                {
                    string from = fromDate.ToString();
                    string[] f = from.Split('/');
                    string[] ftime = f[2].Split(' ');
                    DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                    fDate = Convert.ToDateTime(frmd.ToShortDateString());
                    paramValues.Add(fDate);
                }
                else
                {
                    paramValues.Add(DBNull.Value);
                }

                if (!String.IsNullOrEmpty(toDate))
                {
                    string to = toDate.ToString();
                    string[] f = to.Split('/');
                    string[] ftime = f[2].Split(' ');
                    DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                    tDate = Convert.ToDateTime(frmd.ToShortDateString());
                    paramValues.Add(tDate);
                }
                else
                {
                    paramValues.Add(DBNull.Value);
                }
                paramValues.Add(FranchiseID);
                paramValues.Add(merchantID);
                paramValues.Add(CategoryID);
                paramValues.Add("asc");
               
                ls = obj.ShopSaleAnalisys_MIS(paramValues, System.Web.HttpContext.Current.Server);


                DataTable dt = new DataTable();
                dt.Columns.Add("Sr.No", typeof(string));
                dt.Columns.Add("ShopName", typeof(string));
                dt.Columns.Add("CityName", typeof(string));
                dt.Columns.Add("CategoryName", typeof(string));
                dt.Columns.Add("SaleAmount", typeof(decimal));


                int i = 0;

                foreach (SaleAnalisysMISViewModel row in ls)
                {
                    i = i + 1;
                    dt.LoadDataRow(new object[] { i, row.ShopName, 
                        row.CityName, 
                        row.CategoryName, 
                       row.SaleAmount}, false);
                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(dt, "Shop Performance in Category Statistics Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(dt, "Shop Performance in Category Statistics Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(dt, "Shop Performance in Category Statistics Report");
                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Exporting Order Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CategoryWiseSaleAnalisysMIS][POST:Export]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Exporting Order Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CategoryWiseSaleAnalisysMIS][POST:Export]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }

            return View("Index");

        }

    }
    /// <summary>
    /// To receive parameter values as argument from ajax to web methode
    /// </summary>
    public class CategoryWiseSaleAnalisysMISLocal
    {
        public int? FranchiseID { get; set; }
        public Int64? merchantID { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int? CategoryID { get; set; }       
        public int? option { get; set; }
    }

}