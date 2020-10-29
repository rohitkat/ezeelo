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
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class ProductApprovedNonApprovedReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 300;
        //
        // GET: /ProductApprovedNonApprovedReport/
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductApprovedNonApprovedReport/CanRead")]
        public ActionResult Index()
        {
            long FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);//GetFranchiseID();
            ViewBag.shopID = new SelectList(from s in db.Shops
                                            where s.FranchiseID == FranchiseID && s.IsActive == true
                                            select new StateCityFranchiseMerchantViewModel
                                            {
                                                MerchantName = s.Name,
                                                MerchantID = s.ID
                                            }, "MerchantID", "MerchantName");
            return View();
        }

        //[HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductApprovedNonApprovedReport/CanRead")]
        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate, long? shopID, int approvedStatus,int print)
        {
            try
            {
                long FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //GetFranchiseID();
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                ViewBag.shopID = shopID;
                ViewBag.approvedStatus = approvedStatus;
                ViewBag.franchiseID = FranchiseID;
                
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
                                                      p.CreateDate <= tod) && sp.ShopID == (shopID == 0 ? sp.ShopID : shopID)
                                                      && s.FranchiseID == FranchiseID
                                                      
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
                    if (print == 1)
                    {
                        return View("ForPrint", productApprovedReportModel);
                    }
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
                                                         tp.CreateDate <= tod) && tsp.ShopID == (shopID == 0 ? tsp.ShopID : shopID) && s.FranchiseID == FranchiseID
                                                         
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
                    if (print == 1)
                    {
                        return View("ForPrint", productNonApprovedReportModel);
                    }
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
                                                      p.CreateDate <= tod) && sp.ShopID == (shopID == 0 ? sp.ShopID : shopID) && s.FranchiseID == FranchiseID
                                                       
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
                                                         tp.CreateDate <= tod) && tsp.ShopID == (shopID == 0 ? tsp.ShopID : shopID) && s.FranchiseID == FranchiseID
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
                    if (print == 1)
                    {
                        return View("ForPrint", UploadedProduct);
                    }
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
                ModelState.AddModelError("Error", "There's Something wrong in loading Product Approval Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductApprovedNonApprovedController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Product Approval Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApprovedNonApprovedController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();


        }

        public ActionResult Export(string fromDate, string toDate, long? shopID, int option, int print, int approvedStatus)
        {
            try
            {
                List<ProductApprovedViewModel> ApprovedReport = new List<ProductApprovedViewModel>();
                ApprovedReport = this.Getdata(fromDate, toDate, shopID, approvedStatus);
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
                    ExportExcelCsv.ExportToExcel(dt, "Product Approved/NonApproved Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(dt, "Product Approved/NonApproved Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(dt, "Product Approved/NonApproved Report");
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

        public List<ProductApprovedViewModel> Getdata(string fromDate, string toDate, long? shopID,int approvedStatus)
        {
            int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;

            //ViewBag.franchiseID = franchiseID;


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
                                  p.CreateDate <= tod) && sp.ShopID == (shopID == 0 ? sp.ShopID : shopID)
                                  && s.FranchiseID == (franchiseID == 0 ? s.FranchiseID : franchiseID)

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
                                  tp.CreateDate <= tod) && tsp.ShopID == (shopID == 0 ? tsp.ShopID : shopID) && s.FranchiseID == (franchiseID == 0 ? s.FranchiseID : franchiseID)

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
                                                  p.CreateDate <= tod) && sp.ShopID == (shopID == 0 ? sp.ShopID : shopID)
                                                  && s.FranchiseID == (franchiseID == 0 ? s.FranchiseID : franchiseID)

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
                                                     tp.CreateDate <= tod) && tsp.ShopID == (shopID == 0 ? tsp.ShopID : shopID) &&
                                                     s.FranchiseID == (franchiseID == 0 ? s.FranchiseID : franchiseID)
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
