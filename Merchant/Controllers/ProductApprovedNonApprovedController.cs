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
    public class ProductApprovedNonApprovedController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 100;
        //
        // GET: /ProductApprovedNonApproved/
        public ActionResult Index()
        {
            ViewBag.stateID = new SelectList(db.States, "ID", "Name");
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
                throw new BusinessLogicLayer.MyException("[ProductApprovedNonApprovedController][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
            }
            return ShopID;
        }

        [HttpPost]
        [SessionExpire]
        //[Authorize(Roles = "ProductApprovedNonApproved/CanRead")]
        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate, int approvedStatus)
        {
            List<ProductApprovedViewModel> productApprovedReportModel = new List<ProductApprovedViewModel>();
            try
            {
                long ShopID = GetShopID();
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                int TotalCount = 0;
                int TotalPages = 0;
                ViewBag.approvedStatus = approvedStatus;
                int pageNumber = page;
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

                if (approvedStatus == 1)
                {
                    ViewBag.uploadprod = 2;
                    productApprovedReportModel = (from sp in db.ShopProducts
                                                  join p in db.Products on sp.ProductID equals p.ID
                                                  join s in db.Shops on sp.ShopID equals s.ID
                                                  join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                                                  join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                  join PD in db.PersonalDetails on bd.UserLoginID equals PD.UserLoginID
                                                  join c in db.Categories on p.CategoryID equals c.ID
                                                  where (sp.CreateDate >= frmd &&
                                                  sp.CreateDate <= tod)
                                                  && sp.ShopID == ShopID
                                                  select new ProductApprovedViewModel
                                                  {
                                                      ProductID = p.ID,
                                                      ProductName = p.Name,
                                                      productDescription = p.Description,
                                                      ProductApprovedDate = p.CreateDate,
                                                      ProductModifiedDate = p.ModifyDate,
                                                      MerchantName = PD.FirstName + " " + PD.MiddleName + " " + PD.LastName,
                                                      ShopName = s.Name,
                                                      IsActive = p.IsActive,
                                                      CatID = c.ID,
                                                      CatName = c.Name

                                                  }).ToList();


                }
                else if (approvedStatus == 2)
                {
                    ViewBag.uploadprod = 3;
                    productApprovedReportModel = (from tsp in db.TempShopProducts
                                                  join tp in db.TempProducts on tsp.ProductID equals tp.ID
                                                  join s in db.Shops on tsp.ShopID equals s.ID
                                                  join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                                                  join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                  join PD in db.PersonalDetails on bd.UserLoginID equals PD.UserLoginID
                                                  join c in db.Categories on tp.CategoryID equals c.ID
                                                  where (tsp.CreateDate >= frmd &&
                                                  tsp.CreateDate <= tod)
                                                  && tsp.ShopID == ShopID
                                                  select new ProductApprovedViewModel
                                                  {
                                                      ProductID = tp.ID,
                                                      ProductName = tp.Name,
                                                      productDescription = tp.Description,
                                                      ProductApprovedDate = tp.CreateDate,
                                                      ProductModifiedDate = tp.ModifyDate,
                                                      MerchantName = PD.FirstName + " " + PD.MiddleName + " " + PD.LastName,
                                                      ShopName = s.Name,
                                                      IsActive = tp.IsActive,
                                                      CatID = c.ID,
                                                      CatName = c.Name
                                                  }).ToList();



                }
                else
                {
                    ViewBag.uploadprod = 1;
                    var productApprovedReport = (from sp in db.ShopProducts
                                                 join p in db.Products on sp.ProductID equals p.ID
                                                 join s in db.Shops on sp.ShopID equals s.ID
                                                 join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                                                 join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                 join PD in db.PersonalDetails on bd.UserLoginID equals PD.UserLoginID
                                                 join c in db.Categories on p.CategoryID equals c.ID
                                                 where (sp.CreateDate >= frmd &&
                                             sp.CreateDate <= tod)
                                             && sp.ShopID == ShopID

                                                 select new ProductApprovedViewModel
                                                 {
                                                     ProductID = p.ID,
                                                     ProductName = p.Name,
                                                     productDescription = p.Description,
                                                     ProductApprovedDate = p.CreateDate,
                                                     ProductModifiedDate = p.ModifyDate,
                                                     MerchantName = PD.FirstName + " " + PD.MiddleName + " " + PD.LastName,
                                                     ShopName = s.Name,
                                                     ApprovalRemark = "Approved",
                                                     IsActive = p.IsActive,
                                                     CatID = c.ID,
                                                     CatName = c.Name

                                                 }).ToList();

                    var productNonApprovedReportModel = (from tsp in db.TempShopProducts
                                                         join tp in db.TempProducts on tsp.ProductID equals tp.ID
                                                         join s in db.Shops on tsp.ShopID equals s.ID
                                                         join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                                                         join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                         join PD in db.PersonalDetails on bd.UserLoginID equals PD.UserLoginID
                                                         join c in db.Categories on tp.CategoryID equals c.ID
                                                         where (tsp.CreateDate >= frmd &&
                                                  tsp.CreateDate <= tod)
                                                  && tsp.ShopID == ShopID
                                                         select new ProductApprovedViewModel
                                                         {
                                                             ProductID = tp.ID,
                                                             ProductName = tp.Name,
                                                             productDescription = tp.Description,
                                                             ProductUploadDate = tp.CreateDate,
                                                             ProductModifiedDate = tp.ModifyDate,
                                                             MerchantName = PD.FirstName + " " + PD.MiddleName + " " + PD.LastName,
                                                             ShopName = s.Name,
                                                             ApprovalRemark = "Non-Approved",
                                                             IsActive = tp.IsActive,
                                                             CatID = c.ID,
                                                             CatName = c.Name
                                                         }).ToList();
                    productApprovedReportModel = productApprovedReport.Union(productNonApprovedReportModel).ToList();
                }
                TotalCount = productApprovedReportModel.Count();
                ViewBag.TotalCount = TotalCount;
                productApprovedReportModel = productApprovedReportModel.OrderByDescending(x => x.ProductUploadDate).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.PageSize = productApprovedReportModel.Count;
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                ViewBag.TotalPages = TotalPages;
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Product Approval Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductApprovedNonApprovedController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Product Approval Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApprovedNonApprovedController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View(productApprovedReportModel.OrderBy(x => x.ProductName).ToList());

        }

        public ActionResult Export(string fromDate, string toDate, int option, int print, int approvedStatus)
        {
            try
            {
                List<ProductApprovedViewModel> ApprovedReport = new List<ProductApprovedViewModel>();
                ApprovedReport = this.Getdata(fromDate, toDate, approvedStatus);
                if (print == 1)
                {
                    return View("ForPrint", ApprovedReport);
                }
                DataTable dt = new DataTable();
                dt.Columns.Add("Sr.No.", typeof(long));
                //dt.Columns.Add("Product ID", typeof(int));
                dt.Columns.Add("Product Name", typeof(string));
                dt.Columns.Add("Description", typeof(string));
                dt.Columns.Add("Category Name", typeof(string));
                dt.Columns.Add("Merchant Name", typeof(string));
                dt.Columns.Add("Shop Name", typeof(string));
                if (approvedStatus == 1)
                {
                    dt.Columns.Add("Product Approved Date", typeof(DateTime));
                    dt.Columns.Add("Product Modified Date", typeof(DateTime));
                }
                else if (approvedStatus == 2)
                {
                    dt.Columns.Add("Product Upload Date", typeof(DateTime));
                    dt.Columns.Add("Product Modified Date", typeof(DateTime));
                }
                else if (approvedStatus == 3)
                {
                    dt.Columns.Add("Product Upload Date", typeof(DateTime));
                    dt.Columns.Add("Product Approved Date", typeof(DateTime));
                    dt.Columns.Add("Status", typeof(string));
                }

                dt.Columns.Add("Active/Deactive Status", typeof(string));
                int i = 0;
                ViewBag.stateID = new SelectList(db.States, "ID", "Name");
                foreach (var row in ApprovedReport)
                {
                    i = i + 1;
                    string ActDect = "Active";
                    if (row.IsActive == false)
                    {
                        ActDect = "DeActive";
                    }
                    if (approvedStatus == 1)
                    {
                        dt.LoadDataRow(new object[] {i,row.ProductName,row.productDescription, row.CatName, row.MerchantName, row.ShopName, row.ProductApprovedDate,
                       row.ProductModifiedDate,ActDect}, false);
                    }
                    else if (approvedStatus == 2)
                    {
                        dt.LoadDataRow(new object[] {i, row.ProductName,row.productDescription,row.CatName, row.MerchantName, row.ShopName, row.ProductUploadDate,
                       row.ProductModifiedDate,ActDect}, false);
                    }
                    else if (approvedStatus == 3)
                    {
                        dt.LoadDataRow(new object[] {i, row.ProductName,row.productDescription,row.CatName, row.MerchantName, row.ShopName, row.ProductUploadDate,
                       row.ProductApprovedDate,row.ApprovalRemark,ActDect}, false);
                    }
                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(dt, "Product Approved/NonApproved  Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(dt, "Product Approved/NonApproved  Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(dt, "Product Approved/NonApproved  Report");
                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Exporting Merchant Registration Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantRegistrationReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Merchant Registration Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantRegistrationReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View("Index");

        }

        public List<ProductApprovedViewModel> Getdata(string fromDate, string toDate, int approvedStatus)
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
            List<ProductApprovedViewModel> ApprovedReport = new List<ProductApprovedViewModel>();
            if (approvedStatus == 1)
            {
                ViewBag.uploadprod = 2;
                ApprovedReport = (from sp in db.ShopProducts
                                  join p in db.Products on sp.ProductID equals p.ID
                                  join s in db.Shops on sp.ShopID equals s.ID
                                  join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                                  join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                  join PD in db.PersonalDetails on bd.UserLoginID equals PD.UserLoginID
                                  join c in db.Categories on p.CategoryID equals c.ID
                                  where (sp.CreateDate >= frmd &&
                                  sp.CreateDate <= tod)
                                  && sp.ShopID == shopID
                                  select new ProductApprovedViewModel
                                  {
                                      ProductID = p.ID,
                                      ProductName = p.Name,
                                      productDescription = p.Description,
                                      ProductApprovedDate = p.CreateDate,
                                      ProductModifiedDate = p.ModifyDate,
                                      MerchantName = PD.FirstName + " " + PD.MiddleName + " " + PD.LastName,
                                      ShopName = s.Name,
                                      IsActive = p.IsActive,
                                      CatID = c.ID,
                                      CatName = c.Name

                                  }).ToList();

            }
            else if (approvedStatus == 2)
            {
                ViewBag.uploadprod = 3;
                ApprovedReport = (from tsp in db.TempShopProducts
                                  join tp in db.TempProducts on tsp.ProductID equals tp.ID
                                  join s in db.Shops on tsp.ShopID equals s.ID
                                  join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                                  join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                  join PD in db.PersonalDetails on bd.UserLoginID equals PD.UserLoginID
                                  join c in db.Categories on tp.CategoryID equals c.ID
                                  where (tsp.CreateDate >= frmd &&
                                  tsp.CreateDate <= tod)
                                  && tsp.ShopID == shopID
                                  select new ProductApprovedViewModel
                                  {
                                      ProductID = tp.ID,
                                      ProductName = tp.Name,
                                      productDescription = tp.Description,
                                      ProductUploadDate = tp.CreateDate,
                                      ProductModifiedDate = tp.ModifyDate,
                                      MerchantName = PD.FirstName + " " + PD.MiddleName + " " + PD.LastName,
                                      ShopName = s.Name,
                                      ApprovalRemark = "Non-Approved",
                                      IsActive = tp.IsActive,
                                      CatID = c.ID,
                                      CatName = c.Name
                                  }).ToList();
            }
            else
            {
                ViewBag.uploadprod = 1;
                var productApprovedReport = (from sp in db.ShopProducts
                                             join p in db.Products on sp.ProductID equals p.ID
                                             join s in db.Shops on sp.ShopID equals s.ID
                                             join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                                             join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                             join PD in db.PersonalDetails on bd.UserLoginID equals PD.UserLoginID
                                             join c in db.Categories on p.CategoryID equals c.ID
                                             where (sp.CreateDate >= frmd &&
                                         sp.CreateDate <= tod)
                                         && sp.ShopID == shopID

                                             select new ProductApprovedViewModel
                                             {
                                                 ProductID = p.ID,
                                                 ProductName = p.Name,
                                                 productDescription = p.Description,
                                                 ProductApprovedDate = p.CreateDate,
                                                 ProductModifiedDate = p.ModifyDate,
                                                 MerchantName = PD.FirstName + " " + PD.MiddleName + " " + PD.LastName,
                                                 ShopName = s.Name,
                                                 ApprovalRemark = "Approved",
                                                 IsActive = p.IsActive,
                                                 CatID = c.ID,
                                                 CatName = c.Name

                                             }).ToList();

                var productNonApprovedReportModel = (from tsp in db.TempShopProducts
                                                     join tp in db.TempProducts on tsp.ProductID equals tp.ID
                                                     join s in db.Shops on tsp.ShopID equals s.ID
                                                     join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                                                     join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                     join PD in db.PersonalDetails on bd.UserLoginID equals PD.UserLoginID
                                                     join c in db.Categories on tp.CategoryID equals c.ID
                                                     where (tsp.CreateDate >= frmd &&
                                              tsp.CreateDate <= tod)
                                              && tsp.ShopID == shopID
                                                     select new ProductApprovedViewModel
                                                     {
                                                         ProductID = tp.ID,
                                                         ProductName = tp.Name,
                                                         productDescription = tp.Description,
                                                         ProductUploadDate = tp.CreateDate,
                                                         ProductModifiedDate = tp.ModifyDate,
                                                         MerchantName = PD.FirstName + " " + PD.MiddleName + " " + PD.LastName,
                                                         ShopName = s.Name,
                                                         ApprovalRemark = "Non-Approved",
                                                         IsActive = tp.IsActive,
                                                         CatID = c.ID,
                                                         CatName = c.Name
                                                     }).ToList();
                ApprovedReport = productApprovedReport.Union(productNonApprovedReportModel).ToList();
            }
          

            return ApprovedReport.OrderBy(x => x.ProductName).ToList();
        }
    }
}
