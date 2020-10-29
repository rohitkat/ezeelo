using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Collections.ObjectModel;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using Administrator.Models;
using System.Data;

//<copyright file="ProductApprovedNonApprovedReport.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
//    </copyright>
//    <author>Harshada Raghorte</author>
namespace Administrator.Controllers
{
    public class ProductApprovedNonApprovedController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 300;
        //
        // GET: /ProductApprovedNonApproved/
        [SessionExpire]
        //[CustomAuthorize(Roles = "ProductApprovedNonApproved/CanRead")]
        public ActionResult Index()
        {
            try
            {
                List<StateCityFranchiseMerchantViewModel> obj = new List<StateCityFranchiseMerchantViewModel>();
                obj = this.GetFranchise();
                ViewBag.ddlfranchise = new SelectList(obj, "FranchiseID", "FranchiseName");
               //ViewBag.stateID = new SelectList(db.States, "ID", "Name");
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductApprovedNonApprovedController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApprovedNonApprovedController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        //[HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductApprovedNonApproved/CanRead")]
        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate, long? merchantID, long? franchiseID, long? cityID, int approvedStatus, int print)
        {
            try
            {
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;

                int TotalCount = 0;
                int TotalPages = 0;
                ViewBag.merchantID = merchantID;
                ViewBag.approvedStatus = approvedStatus;
                ViewBag.franchiseID = franchiseID;
                ViewBag.cityID = cityID;
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
                    var productApprovedReportModel = (from sp in db.ShopProducts
                                                      join p in db.Products on sp.ProductID equals p.ID
                                                      join s in db.Shops on sp.ShopID equals s.ID
                                                      join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                                                      join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                      join PD in db.PersonalDetails on bd.UserLoginID equals PD.UserLoginID
                                                      join c in db.Categories on p.CategoryID equals c.ID
                                                      where (p.CreateDate >= frmd &&
                                                      p.CreateDate <= tod) && sp.ShopID == (merchantID == 0 ? sp.ShopID : merchantID)
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
                                                         tp.CreateDate <= tod) && tsp.ShopID == (merchantID == 0 ? tsp.ShopID : merchantID) && s.FranchiseID == (franchiseID == 0 ? s.FranchiseID : franchiseID)

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
                                                      p.CreateDate <= tod) && sp.ShopID == (merchantID == 0 ? sp.ShopID : merchantID)
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
                                                         tp.CreateDate <= tod) && tsp.ShopID == (merchantID == 0 ? tsp.ShopID : merchantID) &&
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
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Product Approval Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApprovedNonApprovedController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();

        }

        public ActionResult Export(string fromDate, string toDate, long? merchantID, long? franchiseID, int option, int print, int approvedStatus)
        {
            try
            {
                List<ProductApprovedViewModel> ApprovedReport = new List<ProductApprovedViewModel>();
                ApprovedReport = this.Getdata(fromDate, toDate, merchantID, franchiseID, approvedStatus);
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
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Merchant Registration Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantRegistrationReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View("Index");

        }

        public List<ProductApprovedViewModel> Getdata(string fromDate, string toDate, long? merchantID, long? franchiseID, int approvedStatus)
        {
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;

            ViewBag.franchiseID = franchiseID;


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
                                                  p.CreateDate <= tod) && sp.ShopID == (merchantID == 0 ? sp.ShopID : merchantID)
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
                                  tp.CreateDate <= tod) && tsp.ShopID == (merchantID == 0 ? tsp.ShopID : merchantID) && s.FranchiseID == (franchiseID == 0 ? s.FranchiseID : franchiseID)

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
                                                  p.CreateDate <= tod) && sp.ShopID == (merchantID == 0 ? sp.ShopID : merchantID)
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
                                                     tp.CreateDate <= tod) && tsp.ShopID == (merchantID == 0 ? tsp.ShopID : merchantID) &&
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

            return ApprovedReport.OrderBy(x=>x.ProductName).ToList();
        }
        public JsonResult GetCityByStateId(int stateID)
        {
            List<District> ldistrict = new List<District>();
            List<City> lcity = new List<City>();
            List<StateCityFranchiseMerchantViewModel> city = new List<StateCityFranchiseMerchantViewModel>();
            try
            {
                ldistrict = db.Districts.Where(x => x.StateID == stateID).ToList();
                foreach (var x in ldistrict)
                {

                    lcity = db.Cities.Where(c => c.DistrictID == x.ID).ToList();
                    foreach (var c in lcity)
                    {
                        StateCityFranchiseMerchantViewModel SCFM = new StateCityFranchiseMerchantViewModel();
                        SCFM.CityID = c.ID;
                        SCFM.CityName = c.Name;
                        city.Add(SCFM);
                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling City Dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductApprovedNonApprovedController][POST:GetCityByStateId]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling City Dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApprovedNonApprovedController][POST:GetCityByStateId]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }


            return Json(city.Distinct().OrderBy(x => x.CityName).ToList(), JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetFranchiseByCityId(int cityID)
        {
            List<StateCityFranchiseMerchantViewModel> franchise = new List<StateCityFranchiseMerchantViewModel>();
            try
            {
                var lFranchise = from f in db.Franchises
                                 join pin in db.Pincodes on f.PincodeID equals pin.ID
                                 join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                                 join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                 join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                 join c in db.Cities on pin.CityID equals c.ID
                                 where c.ID == cityID
                                 select new StateCityFranchiseMerchantViewModel
                                 {
                                     FranchiseName = pd.FirstName + " " + pd.LastName,
                                     FranchiseID = f.ID
                                 };

                foreach (var c in lFranchise)
                {
                    StateCityFranchiseMerchantViewModel SCFM = new StateCityFranchiseMerchantViewModel();
                    SCFM.FranchiseID = c.FranchiseID;
                    SCFM.FranchiseName = c.FranchiseName;
                    franchise.Add(SCFM);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling franchise Dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductApprovedNonApprovedController][POST:GetFranchiseByCityId]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling franchise Dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApprovedNonApprovedController][POST:GetFranchiseByCityId]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }


            return Json(franchise.Distinct().OrderBy(x => x.FranchiseName).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMerchantByFranchiseId(int franchiseID)
        {

            List<StateCityFranchiseMerchantViewModel> merchant = new List<StateCityFranchiseMerchantViewModel>();
            try
            {
                var lMerchant = (from s in db.Shops
                                 where s.FranchiseID == franchiseID
                                 select new StateCityFranchiseMerchantViewModel
                                 {
                                     MerchantName = s.Name,
                                     MerchantID = s.ID
                                 }).Distinct();

                foreach (var c in lMerchant)
                {
                    StateCityFranchiseMerchantViewModel SCFM = new StateCityFranchiseMerchantViewModel();
                    SCFM.MerchantID = c.MerchantID;
                    SCFM.MerchantName = c.MerchantName;
                    merchant.Add(SCFM);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling Merchant Dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductApprovedNonApprovedController][POST:GetMerchantByFranchiseId]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling Merchant Dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApprovedNonApprovedController][POST:GetMerchantByFranchiseId]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }

            return Json(merchant.Distinct().OrderBy(x => x.MerchantName).ToList(), JsonRequestBehavior.AllowGet);
        }

        public List<StateCityFranchiseMerchantViewModel> GetFranchise()
        {
            List<StateCityFranchiseMerchantViewModel> franchise = new List<StateCityFranchiseMerchantViewModel>();
            try
            {
                var lFranchise = from f in db.Franchises
                                 join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                                 join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                 join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                 where f.IsActive==true
                                 select new StateCityFranchiseMerchantViewModel
                                 {
                                     FranchiseName = pd.FirstName + " " + pd.LastName,
                                     FranchiseID = f.ID
                                 };

                foreach (var c in lFranchise)
                {
                    StateCityFranchiseMerchantViewModel SCFM = new StateCityFranchiseMerchantViewModel();
                    SCFM.FranchiseID = c.FranchiseID;
                    SCFM.FranchiseName = c.FranchiseName;
                    franchise.Add(SCFM);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling franchise Dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductApprovedNonApprovedController][POST:GetFranchiseBy]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling franchise Dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApprovedNonApprovedController][POST:GetFranchiseBy]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }


            return franchise.Distinct().OrderBy(x => x.FranchiseName).ToList();
        }

    }
}
