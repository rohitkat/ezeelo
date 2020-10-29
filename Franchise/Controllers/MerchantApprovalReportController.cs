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
    public class MerchantApprovalReportController : Controller
    {
        public class CategoryDetail
        {
            public Int64 ID { get; set; }
            public string Name { get; set; }
        }
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 200;
        //
        // GET: /MerchantApprovalReport/
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantApprovalReport/CanRead")]
        public ActionResult Index()
        {
            int FranchiseID = GetFranchiseID();
            ViewBag.CategoryList = new SelectList((from c in db.Categories
                                                   join pbc in db.PlanCategoryCharges on c.ID equals pbc.CategoryID
                                                   join p in db.Plans on pbc.PlanID equals p.ID
                                                   join op in db.OwnerPlans on p.ID equals op.PlanID
                                                   where op.OwnerID == FranchiseID && p.PlanCode.Substring(0, 4) == "GBFR"
                                                   && op.IsActive == true && pbc.IsActive == true && c.IsActive == true
                                                   select new CategoryDetail
                                                   {
                                                       ID = c.ID,
                                                       Name = c.Name
                                                   }).ToList(), "ID", "Name");
            List<CategoryDetail> ldata1 = new List<CategoryDetail>();
            ldata1.Add(new CategoryDetail { ID = 0, Name = "Second Level" });
            List<CategoryDetail> ldata2 = new List<CategoryDetail>();
            ldata2.Add(new CategoryDetail { ID = 0, Name = "Third Level" });

            //ViewBag.CategoryList = new SelectList(ldata, "ID", "Name");
            ViewBag.LevelTwoCategoryList = new SelectList(ldata1, "ID", "Name");
            ViewBag.LevelThreeCategoryList = new SelectList(ldata2, "ID", "Name");
            return View();
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantApprovalReport/CanRead")]
        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate, int approvedStatus, int CategoryID)
        {
            try
            {
                int FranchiseID = GetFranchiseID();
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                ViewBag.approvedStatus = approvedStatus;
                ViewBag.CategoryID = CategoryID;
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


                tod = tod.AddDays(1);

                if (approvedStatus == 1)
                {
                    var MerchantRegistrationApprovedReportModel = (from ul in db.UserLogins
                                                                   join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                                                   join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                                                                   join shp in db.Shops on bd.ID equals shp.BusinessDetailID
                                                                   join op in db.OwnerPlans on shp.ID equals op.OwnerID
                                                                   join P in db.Plans on op.PlanID equals P.ID
                                                                   join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                                                   where (bd.CreateDate >= frmd &&
                                                                       bd.CreateDate <= tod) && shp.FranchiseID == (FranchiseID == 0 ? shp.FranchiseID : FranchiseID)
                                                                       && ul.IsLocked == false && bd.BusinessTypeID == 1 && pcc.CategoryID == CategoryID && P.PlanCode.StartsWith("GBMR")
                                                                   select new MerchantRegistrationReportViewModel
                                                                   {
                                                                       VendorName = pd.FirstName + " " + pd.MiddleName + " " + pd.LastName,
                                                                       VendorRegistrationNo = shp.ID,
                                                                       MobileNo = bd.Mobile,
                                                                       RegistrationDate = bd.CreateDate,
                                                                       OrganisationName = bd.Name,
                                                                       Email = bd.Email,
                                                                       Address = shp.Address
                                                                   }).ToList();
                    TotalCount = MerchantRegistrationApprovedReportModel.Count();
                    ViewBag.TotalCount = TotalCount;
                    MerchantRegistrationApprovedReportModel = MerchantRegistrationApprovedReportModel.OrderByDescending(x => x.RegistrationDate).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                    ViewBag.PageSize = MerchantRegistrationApprovedReportModel.Count;
                    TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                    ViewBag.TotalPages = TotalPages;
                    return View(MerchantRegistrationApprovedReportModel);
                }
                else
                {
                    var MerchantRegistrationNonApprovedReportModel = (from ul in db.UserLogins
                                                                      join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                                                      join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                                                                      join shp in db.Shops on bd.ID equals shp.BusinessDetailID
                                                                      join op in db.OwnerPlans on shp.ID equals op.OwnerID
                                                                      join P in db.Plans on op.PlanID equals P.ID
                                                                      join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                                                      where (bd.CreateDate >= frmd &&
                                                                          bd.CreateDate <= tod) && shp.FranchiseID == (FranchiseID == 0 ? shp.FranchiseID : FranchiseID)
                                                                          && ul.IsLocked == true && bd.BusinessTypeID == 1 && pcc.CategoryID == CategoryID && P.PlanCode.StartsWith("GBMR")
                                                                      select new MerchantRegistrationReportViewModel
                                                                      {
                                                                          VendorName = pd.FirstName + " " + pd.MiddleName + " " + pd.LastName,
                                                                          VendorRegistrationNo = shp.ID,
                                                                          MobileNo = bd.Mobile,
                                                                          RegistrationDate = bd.CreateDate,
                                                                          OrganisationName = bd.Name,
                                                                          Email = bd.Email,
                                                                          Address = shp.Address
                                                                      }).ToList();
                    TotalCount = MerchantRegistrationNonApprovedReportModel.Count();
                    ViewBag.TotalCount = TotalCount;
                    MerchantRegistrationNonApprovedReportModel = MerchantRegistrationNonApprovedReportModel.OrderByDescending(x => x.RegistrationDate).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                    ViewBag.PageSize = MerchantRegistrationNonApprovedReportModel.Count;
                    TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                    ViewBag.TotalPages = TotalPages;

                    return View(MerchantRegistrationNonApprovedReportModel);

                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Merchant Registration Report!!");

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
            return View();
        }

        private int GetFranchiseID()
        {
            long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
            long BusinessDetailID = 0;
            int FranchiseID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    BusinessDetailID = Convert.ToInt32(db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                    FranchiseID = Convert.ToInt32(db.Franchises.Where(x => x.BusinessDetailID == BusinessDetailID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PlacedController][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
            }
            return FranchiseID;
        }

        public ActionResult Export(string fromDate, string toDate, int option, int print, int approvedStatus, int CategoryID)
        {
            try
            {
                List<MerchantRegistrationReportViewModel> MerchantApprovedReport = new List<MerchantRegistrationReportViewModel>();
                MerchantApprovedReport = this.Getdata(fromDate, toDate, approvedStatus, CategoryID);
                if (print == 1)
                {
                    return View("ForPrint", MerchantApprovedReport);
                }
                DataTable dt = new DataTable();
                dt.Columns.Add("Sr.No.", typeof(long));
                dt.Columns.Add("Vendor Name", typeof(string));
                dt.Columns.Add("Organisation Name", typeof(string));
                dt.Columns.Add("Buisness Nature", typeof(string));
                dt.Columns.Add("Address", typeof(string));
                dt.Columns.Add("Landmark", typeof(string));
                dt.Columns.Add("Registration Date", typeof(DateTime));

                int i = 0;
                foreach (var row in MerchantApprovedReport)
                {
                    i = i + 1;
                    dt.LoadDataRow(new object[] { i, row.VendorName, row.OrganisationName, row.BuisnessNature, row.Address, row.Landmark, row.RegistrationDate, }, false);
                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(dt, "Merchant Approved/NonApproved Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(dt, "Merchant Approved/NonApproved Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(dt, "Merchant Approved/NonApproved Report");
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

        public List<MerchantRegistrationReportViewModel> Getdata(string fromDate, string toDate, int approvedStatus, int CategoryID)
        {
            int FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
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
            List<MerchantRegistrationReportViewModel> MerchantApprovedReport = new List<MerchantRegistrationReportViewModel>();
            if (approvedStatus == 1)
            {
                MerchantApprovedReport = (from ul in db.UserLogins
                                          join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                          join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                                          join shp in db.Shops on bd.ID equals shp.BusinessDetailID
                                          join op in db.OwnerPlans on shp.ID equals op.OwnerID
                                          join P in db.Plans on op.PlanID equals P.ID
                                          join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                          where (bd.CreateDate >= frmd &&
                                              bd.CreateDate <= tod) && shp.FranchiseID == (FranchiseID == 0 ? shp.FranchiseID : FranchiseID)
                                              && ul.IsLocked == false && bd.BusinessTypeID == 1 && pcc.CategoryID == CategoryID && P.PlanCode.StartsWith("GBMR")
                                          select new MerchantRegistrationReportViewModel
                                          {
                                              VendorName = pd.FirstName + " " + pd.MiddleName + " " + pd.LastName,
                                              VendorRegistrationNo = shp.ID,
                                              MobileNo = bd.Mobile,
                                              RegistrationDate = bd.CreateDate,
                                              OrganisationName = bd.Name,
                                              Email = bd.Email,
                                              Address = shp.Address
                                          }).ToList();
            }
            else
            {
                MerchantApprovedReport = (from ul in db.UserLogins
                                          join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                          join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                                          join shp in db.Shops on bd.ID equals shp.BusinessDetailID
                                          join op in db.OwnerPlans on shp.ID equals op.OwnerID
                                          join P in db.Plans on op.PlanID equals P.ID
                                          join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                          where (bd.CreateDate >= frmd &&
                                              bd.CreateDate <= tod) && shp.FranchiseID == (FranchiseID == 0 ? shp.FranchiseID : FranchiseID)
                                              && ul.IsLocked == true && bd.BusinessTypeID == 1 && pcc.CategoryID == CategoryID && P.PlanCode.StartsWith("GBMR")
                                          select new MerchantRegistrationReportViewModel
                                          {
                                              VendorName = pd.FirstName + " " + pd.MiddleName + " " + pd.LastName,
                                              VendorRegistrationNo = shp.ID,
                                              MobileNo = bd.Mobile,
                                              RegistrationDate = bd.CreateDate,
                                              OrganisationName = bd.Name,
                                              Email = bd.Email,
                                              Address = shp.Address
                                          }).ToList();

            }
            ViewBag.CategoryList = new SelectList((from c in db.Categories
                                                   join pbc in db.PlanCategoryCharges on c.ID equals pbc.CategoryID
                                                   join p in db.Plans on pbc.PlanID equals p.ID
                                                   join op in db.OwnerPlans on p.ID equals op.PlanID
                                                   where op.OwnerID == FranchiseID && p.PlanCode.Substring(0, 4) == "GBFR"
                                                   && op.IsActive == true && pbc.IsActive == true && c.IsActive == true
                                                   select new CategoryDetail
                                                   {
                                                       ID = c.ID,
                                                       Name = c.Name
                                                   }).ToList(), "ID", "Name");
            List<CategoryDetail> ldata1 = new List<CategoryDetail>();
            ldata1.Add(new CategoryDetail { ID = 0, Name = "Second Level" });
            List<CategoryDetail> ldata2 = new List<CategoryDetail>();
            ldata2.Add(new CategoryDetail { ID = 0, Name = "Third Level" });

            //ViewBag.CategoryList = new SelectList(ldata, "ID", "Name");
            ViewBag.LevelTwoCategoryList = new SelectList(ldata1, "ID", "Name");
            ViewBag.LevelThreeCategoryList = new SelectList(ldata2, "ID", "Name");
            return MerchantApprovedReport;
        }

        [HttpPost]
        public JsonResult SelectLevelOneCategoryByFranchise(int id)
        {
            List<CategoryDetail> catList = new List<CategoryDetail>();
            catList = (from c in db.Categories
                       join pbc in db.PlanCategoryCharges on c.ID equals pbc.CategoryID
                       join p in db.Plans on pbc.PlanID equals p.ID
                       join op in db.OwnerPlans on p.ID equals op.PlanID
                       where op.OwnerID == id && p.PlanCode.Substring(0, 4) == "GBFR"
                       && op.IsActive == true && pbc.IsActive == true && c.IsActive == true
                       select new CategoryDetail
                       {
                           ID = c.ID,
                           Name = c.Name
                       }).ToList();

            return Json(catList, JsonRequestBehavior.AllowGet);


        }

        public JsonResult SelectCategory(Int64 ParentCategory, int level)
        {

            List<CategoryDetail> cd = new List<CategoryDetail>();
            cd = (from n in db.Categories
                  where n.ParentCategoryID == ParentCategory && n.Level == level
                  && n.IsActive == true
                  select new CategoryDetail
                  {
                      ID = n.ID,
                      Name = n.Name
                  }).OrderBy(x => x.Name).ToList();

            return Json(cd, JsonRequestBehavior.AllowGet);

        }

    }
}
