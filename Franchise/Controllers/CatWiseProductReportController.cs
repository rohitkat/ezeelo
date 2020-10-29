using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.ObjectModel;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using Franchise.Models;
using System.Data;
using PagedList;
using PagedList.Mvc;
using ModelLayer.Models;

namespace Franchise.Controllers
{
    public class CatWiseProductReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 100;
        //
        // GET: /Default1/
        public ActionResult Index()
        {
            int pageNumber = 1;
            long FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
            // Name = bd.Name 
            ViewBag.CategoryList = new SelectList((from c in db.Categories
                                                   join pbc in db.PlanCategoryCharges on c.ID equals pbc.CategoryID
                                                   join p in db.Plans on pbc.PlanID equals p.ID
                                                   join op in db.OwnerPlans on p.ID equals op.PlanID
                                                   where op.OwnerID == FranchiseID && p.PlanCode.Substring(0, 4) == "GBFR"
                                                   && op.IsActive == true && pbc.IsActive == true && c.IsActive == true
                                                   select new CategoryproductDetail
                                                   {
                                                       ID = c.ID,
                                                       Name = c.Name
                                                   }).ToList(), "ID", "Name");

            List<CategoryproductDetail> ldata1 = new List<CategoryproductDetail>();
            ldata1.Add(new CategoryproductDetail { ID = 0, Name = "Second Level" });

            ViewBag.LevelTwoCategoryList = new SelectList(ldata1, "ID", "Name");

            List<CategoryproductDetail> lstDetail = new List<CategoryproductDetail>();

            return View(lstDetail.ToPagedList(pageNumber, PageSize));
        }

        [HttpPost]
        public JsonResult SelectLevelOneCategoryByFranchise(int id)
        {
            List<CategoryproductDetail> catList = new List<CategoryproductDetail>();
            catList = (from c in db.Categories
                       join pbc in db.PlanCategoryCharges on c.ID equals pbc.CategoryID
                       join p in db.Plans on pbc.PlanID equals p.ID
                       join op in db.OwnerPlans on p.ID equals op.PlanID
                       where op.OwnerID == id && p.PlanCode.Substring(0, 4) == "GBFR"
                       && op.IsActive == true && pbc.IsActive == true && c.IsActive == true
                       select new CategoryproductDetail
                       {
                           ID = c.ID,
                           Name = c.Name
                       }).ToList(); ;

            return Json(catList, JsonRequestBehavior.AllowGet);


        }

        public JsonResult SelectCategory(Int64 ParentCategory, int level)
        {

            List<CategoryproductDetail> cd = new List<CategoryproductDetail>();
            cd = (from n in db.Categories
                  where n.ParentCategoryID == ParentCategory && n.Level == level
                  && n.IsActive == true
                  select new CategoryproductDetail
                  {
                      ID = n.ID,
                      Name = n.Name
                  }).OrderBy(x => x.Name).ToList();

            return Json(cd, JsonRequestBehavior.AllowGet);

        }

        //public ActionResult GetThirdLevelCategory()
        //{
        //    return RedirectToAction("Index");
        //}


        public ActionResult GetThirdLevelCategory(int? LevelTwoCategoryList, int? Page)
        {
            int pageNumber = (Page ?? 1);
            ViewBag.PageNumber = pageNumber;
            int pagesize = 15;
            ViewBag.PageSize = pagesize;
            ViewBag.CatID = LevelTwoCategoryList;
            long FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
            List<CategoryproductDetail> cdApproved = new List<CategoryproductDetail>();
            List<CategoryproductDetail> cdNonApproved = new List<CategoryproductDetail>();
            List<CategoryproductDetail> cdFinal = new List<CategoryproductDetail>();
            try
            {
                if (LevelTwoCategoryList != null)
                {
                    cdApproved = (from cat in db.Categories
                                  join prd in db.Products on cat.ID equals prd.CategoryID into prd_join
                                  from prd in prd_join.DefaultIfEmpty()
                                  join shpPrd in db.ShopProducts on prd.ID equals shpPrd.ProductID into shpPrd_join
                                  from shpPrd in shpPrd_join.DefaultIfEmpty()
                                  where cat.ParentCategoryID == LevelTwoCategoryList && cat.IsActive == true && cat.Level == 3
                                  select new CategoryproductDetail
                                  {
                                      ID = cat.ID,
                                      Name = cat.Name,
                                      ApprovedCount = 1
                                  }).ToList();

                    cdApproved = (from lst in cdApproved
                                  group lst by new
                                  {
                                      lst.ID
                                  } into gcs
                                  select new CategoryproductDetail
                                  {
                                      ID = gcs.Key.ID,
                                      Name = gcs.FirstOrDefault(x => x.ID == gcs.Key.ID).Name,
                                      ApprovedCount = gcs.Count(x => x.ID == gcs.Key.ID)
                                  }).ToList();

                    cdNonApproved = (from cat in db.Categories
                                     join prd in db.TempProducts on cat.ID equals prd.CategoryID into prd_join
                                     from prd in prd_join.DefaultIfEmpty()
                                     join shpPrd in db.TempShopProducts on prd.ID equals shpPrd.ProductID into shpPrd_join
                                     from shpPrd in shpPrd_join.DefaultIfEmpty()
                                     where cat.ParentCategoryID == LevelTwoCategoryList && cat.IsActive == true && cat.Level == 3
                                     select new CategoryproductDetail
                                     {
                                         ID = cat.ID,
                                         Name = cat.Name,
                                         NonApprovedCount = 1
                                     }).ToList();
                    cdNonApproved = (from lst in cdNonApproved
                                     group lst by new
                                     {
                                         lst.ID
                                     } into gcs
                                     select new CategoryproductDetail
                                     {
                                         ID = gcs.Key.ID,
                                         Name = gcs.FirstOrDefault(x => x.ID == gcs.Key.ID).Name,
                                         NonApprovedCount = gcs.Count(x => x.ID == gcs.Key.ID)
                                     }).ToList();

                    cdFinal = (from ca in cdApproved
                               join
                                   cna in cdNonApproved on ca.ID equals cna.ID
                               select new CategoryproductDetail
                               {
                                   ID = ca.ID,
                                   Name = ca.Name,
                                   ApprovedCount = ca.ApprovedCount,
                                   NonApprovedCount = cna.NonApprovedCount
                               }).OrderBy(x => x.Name).ToList();
                }

                ViewBag.CategoryList = new SelectList((from c in db.Categories
                                                       join pbc in db.PlanCategoryCharges on c.ID equals pbc.CategoryID
                                                       join p in db.Plans on pbc.PlanID equals p.ID
                                                       join op in db.OwnerPlans on p.ID equals op.PlanID
                                                       where op.OwnerID == FranchiseID && p.PlanCode.Substring(0, 4) == "GBFR"
                                                       && op.IsActive == true && pbc.IsActive == true && c.IsActive == true
                                                       select new CategoryproductDetail
                                                       {
                                                           ID = c.ID,
                                                           Name = c.Name
                                                       }).ToList(), "ID", "Name");

                // Name = bd.Name 

                List<CategoryproductDetail> ldata1 = new List<CategoryproductDetail>();
                ldata1.Add(new CategoryproductDetail { ID = 0, Name = "Second Level" });
                ViewBag.LevelTwoCategoryList = new SelectList(ldata1, "ID", "Name");
            }
            catch (Exception ex)
            {

            }
            return View("Index", cdFinal.ToPagedList(pageNumber, pagesize));

        }

        public ActionResult GetProductInCategory(int CategoryID)
        {
            ViewBag.CategoryID = CategoryID;
            return View();
        }


        //[SessionExpire]
        //[CustomAuthorize(Roles = "CatWiseProductReport/CanRead")]
        [HttpPost]
        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate, int approvedStatus, int CategoryID)
        {

            try
            {
                int? PAGE = page;
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                ViewBag.Page = PageSize;
                int pageNumber = (PAGE ?? 1);
                ViewBag.PageNumber = pageNumber;
                ViewBag.CategoryID = CategoryID;
                int TotalCount = 0;
                int TotalPages = 0;
                ViewBag.approvedStatus = approvedStatus;
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
                    var productApprovedReportModel = (from sp in db.ShopProducts
                                                      join p in db.Products on sp.ProductID equals p.ID
                                                      join s in db.Shops on sp.ShopID equals s.ID
                                                      join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                                                      join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                      join PD in db.PersonalDetails on bd.UserLoginID equals PD.UserLoginID
                                                      join c in db.Categories on p.CategoryID equals c.ID
                                                      where (p.CreateDate >= frmd &&
                                                      p.CreateDate <= tod) && c.ID == CategoryID && c.Level == 3

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

                    TotalCount = productApprovedReportModel.Count();
                    ViewBag.TotalCount = TotalCount;
                    productApprovedReportModel = productApprovedReportModel.OrderByDescending(x => x.ProductUploadDate).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                    ViewBag.PageSize = productApprovedReportModel.Count;
                    TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                    ViewBag.TotalPages = TotalPages;
                    return View(productApprovedReportModel);
                }
                else if (approvedStatus == 2)
                {
                    ViewBag.uploadprod = 3;
                    var productNonApprovedReportModel = (from tsp in db.TempShopProducts
                                                         join tp in db.TempProducts on tsp.ProductID equals tp.ID
                                                         join s in db.Shops on tsp.ShopID equals s.ID
                                                         join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                                                         join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                         join PD in db.PersonalDetails on bd.UserLoginID equals PD.UserLoginID
                                                         join c in db.Categories on tp.CategoryID equals c.ID
                                                         where (tp.CreateDate >= frmd &&
                                                         tp.CreateDate <= tod) && c.ID == CategoryID && c.Level == 3

                                                         select new ProductApprovedViewModel
                                                         {
                                                             ProductID = tp.ID,
                                                             ProductName = tp.Name,
                                                             productDescription = tp.Description,
                                                             ProductUploadDate = tp.CreateDate,
                                                             ProductModifiedDate = tp.ModifyDate,
                                                             MerchantName = PD.FirstName + " " + PD.MiddleName + " " + PD.LastName,
                                                             ShopName = s.Name,
                                                             IsActive = tp.IsActive,
                                                             CatID = c.ID,
                                                             CatName = c.Name
                                                         }).ToList();

                    TotalCount = productNonApprovedReportModel.Count();
                    ViewBag.TotalCount = TotalCount;
                    productNonApprovedReportModel = productNonApprovedReportModel.OrderByDescending(x => x.ProductName).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                    ViewBag.PageSize = productNonApprovedReportModel.Count;
                    TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                    ViewBag.TotalPages = TotalPages;
                    return View(productNonApprovedReportModel);


                }
                else
                {
                    ViewBag.uploadprod = 1;
                    var productApprovedReportModel = (from sp in db.ShopProducts
                                                      join p in db.Products on sp.ProductID equals p.ID
                                                      join s in db.Shops on sp.ShopID equals s.ID
                                                      join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                                                      join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                      join PD in db.PersonalDetails on bd.UserLoginID equals PD.UserLoginID
                                                      join c in db.Categories on p.CategoryID equals c.ID
                                                      where (p.CreateDate >= frmd &&
                                                      p.CreateDate <= tod) && c.ID == CategoryID && c.Level == 3

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
                                                         where (tp.CreateDate >= frmd &&
                                                         tp.CreateDate <= tod) && c.ID == CategoryID && c.Level == 3
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
                    var UploadedProduct = productApprovedReportModel.Union(productNonApprovedReportModel).ToList();

                    TotalCount = UploadedProduct.Count();
                    ViewBag.TotalCount = TotalCount;
                    UploadedProduct = UploadedProduct.OrderByDescending(x => x.ProductName).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                    ViewBag.PageSize = UploadedProduct.Count;
                    TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                    ViewBag.TotalPages = TotalPages;
                    return View(UploadedProduct);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading CategoryWise Product Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CatWiseProductReport][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading CategoryWise Product Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CatWiseProductReport][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();

        }


        public ActionResult Export(string fromDate, string toDate, int option, int print, int approvedStatus, int CategoryID)
        {
            try
            {
                List<ProductApprovedViewModel> ApprovedReport = new List<ProductApprovedViewModel>();
                ApprovedReport = this.Getdata(fromDate, toDate, approvedStatus, CategoryID);
                if (print == 1)
                {
                    return View("ForPrint", ApprovedReport);
                }
                DataTable dt = new DataTable();
                dt.Columns.Add("Sr.No.", typeof(long));
                dt.Columns.Add("Product ID", typeof(int));
                dt.Columns.Add("Product Name", typeof(string));
                dt.Columns.Add("Category ID", typeof(string));
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
                        dt.LoadDataRow(new object[] {i, row.ProductID, row.ProductName,row.CatID, row.CatName, row.MerchantName, row.ShopName, row.ProductApprovedDate,
                       row.ProductModifiedDate,ActDect}, false);
                    }
                    else if (approvedStatus == 2)
                    {
                        dt.LoadDataRow(new object[] {i, row.ProductID, row.ProductName,row.CatID, row.CatName, row.MerchantName, row.ShopName, row.ProductUploadDate,
                       row.ProductModifiedDate,ActDect}, false);
                    }
                    else if (approvedStatus == 3)
                    {
                        dt.LoadDataRow(new object[] {i, row.ProductID, row.ProductName,row.CatID, row.CatName, row.MerchantName, row.ShopName, row.ProductUploadDate,
                       row.ProductApprovedDate,row.ApprovalRemark,ActDect}, false);
                    }

                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(dt, "CategoryWise Product Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(dt, "CategoryWise Product Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(dt, "CategoryWise Product Report");
                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Exporting CategoryWise product Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CatWiseProductReportController][POST:Export]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Exporting CategoryWise product Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CatWiseProductReportController][POST:Export]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            ViewBag.CategoryID = CategoryID;
            return View("GetProductInCategory");


        }

        public List<ProductApprovedViewModel> Getdata(string fromDate, string toDate, int approvedStatus, int CategoryID)
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
                                  where (p.CreateDate >= frmd &&
                                  p.CreateDate <= tod) && c.ID == CategoryID && c.Level == 3

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
                                  where (tp.CreateDate >= frmd &&
                                  tp.CreateDate <= tod) && c.ID == CategoryID && c.Level == 3

                                  select new ProductApprovedViewModel
                                  {
                                      ProductID = tp.ID,
                                      ProductName = tp.Name,
                                      productDescription = tp.Description,
                                      ProductUploadDate = tp.CreateDate,
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
                var productApprovedReportModel = (from sp in db.ShopProducts
                                                  join p in db.Products on sp.ProductID equals p.ID
                                                  join s in db.Shops on sp.ShopID equals s.ID
                                                  join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                                                  join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                  join PD in db.PersonalDetails on bd.UserLoginID equals PD.UserLoginID
                                                  join c in db.Categories on p.CategoryID equals c.ID
                                                  where (p.CreateDate >= frmd &&
                                                  p.CreateDate <= tod) && c.ID == CategoryID && c.Level == 3

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
                                                     where (tp.CreateDate >= frmd &&
                                                     tp.CreateDate <= tod) && c.ID == CategoryID && c.Level == 3
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
                ApprovedReport = productApprovedReportModel.Union(productNonApprovedReportModel).ToList();


            }
            return ApprovedReport;
        }
    }
}
