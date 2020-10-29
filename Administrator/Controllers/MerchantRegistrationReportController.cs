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

//<copyright file="MerchantRegistrationReport.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
//    </copyright>
//    <author>Harshada Raghorte</author>
namespace Administrator.Controllers
{
    public class MerchantRegistrationReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 200;
        // GET: /MerchantRegistrationReport/
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantRegistrationReport/CanRead")]
        public ActionResult Index()
        {
            try
            {
                ViewBag.FranchiseList = new SelectList((from f in db.Franchises
                                                        join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                                                        join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
                                                        join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                        join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                                        where ul.IsLocked == false && f.IsActive == true && bt.Prefix == "GBFR" && f.ID != 1
                                                        select new { ID = f.ID, Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName + " (" + bd.Name + ")", }).ToList(), "ID", "Name");         // Name = bd.Name 
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "First Level" });
                List<CategoryDetail> ldata1 = new List<CategoryDetail>();
                ldata1.Add(new CategoryDetail { ID = 0, Name = "Second Level" });
                List<CategoryDetail> ldata2 = new List<CategoryDetail>();
                ldata2.Add(new CategoryDetail { ID = 0, Name = "Third Level" });

                ViewBag.CategoryList = new SelectList(ldata, "ID", "Name");
                ViewBag.LevelTwoCategoryList = new SelectList(ldata1, "ID", "Name");
                ViewBag.LevelThreeCategoryList = new SelectList(ldata2, "ID", "Name");


                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantRegistrationReportController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantRegistrationReportController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantRegistrationReport/CanRead")]
        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate, long? franchiseID, int approvedStatus, int CategoryID)
        {
            try
            {
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;

                int TotalCount = 0;
                int TotalPages = 0;
                ViewBag.franchiseID = franchiseID;
                ViewBag.approvedStatus = approvedStatus;
                ViewBag.CategoryID = CategoryID;
                int pageNumber = page;
                string from = fromDate.ToString();
                string[] f = from.Split('/');
                string[] ftime = f[2].Split(' ');
                DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);

                string to = toDate.ToString();
                string[] t = to.Split('/');
                string[] ttime = t[2].Split(' ');
                DateTime tod = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);

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
                                                                       bd.CreateDate <= tod) && shp.FranchiseID == (franchiseID == 0 ? shp.FranchiseID : franchiseID)
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
                    MerchantRegistrationApprovedReportModel = MerchantRegistrationApprovedReportModel.OrderByDescending(x => x.ID).Skip((page - 1) * PageSize).Take(PageSize).ToList();
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
                                                                          bd.CreateDate <= tod) && shp.FranchiseID == (franchiseID == 0 ? shp.FranchiseID : franchiseID)
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
                    MerchantRegistrationNonApprovedReportModel = MerchantRegistrationNonApprovedReportModel.OrderByDescending(x => x.ID).Skip((page - 1) * PageSize).Take(PageSize).ToList();
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

        public ActionResult Export(string fromDate, string toDate, long? franchiseID, int option, int print, int approvedStatus, int CategoryID)
        {
            try
            {
                List<MerchantRegistrationReportViewModel> FranchiseReport = new List<MerchantRegistrationReportViewModel>();
                FranchiseReport = this.Getdata(fromDate, toDate, franchiseID, approvedStatus, CategoryID);
                if (print == 1)
                {
                    return View("ForPrint", FranchiseReport);
                }
                DataTable dt = new DataTable();
                dt.Columns.Add("Sr.No.", typeof(long));
                dt.Columns.Add("Vendor Registration No", typeof(int));
                dt.Columns.Add("Vendor Name", typeof(string));
                dt.Columns.Add("Organisation Name", typeof(string));
                dt.Columns.Add("MobileNo", typeof(string));
                dt.Columns.Add("Email", typeof(string));
                dt.Columns.Add("Address", typeof(string));
                dt.Columns.Add("Registration Date", typeof(DateTime));

                int i = 0;
                ViewBag.stateID = new SelectList(db.States, "ID", "Name");
                foreach (var row in FranchiseReport)
                {
                    i = i + 1;
                    dt.LoadDataRow(new object[] {i, row.VendorRegistrationNo, row.VendorName,row.OrganisationName, row.MobileNo, row.Email, row.Address, row.RegistrationDate
                   }, false);
                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(dt, "Merchant Registration Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(dt, "Merchant Registration Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(dt, "Merchant Registration Report");
                }

                ViewBag.FranchiseList = new SelectList((from f in db.Franchises
                                                        join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                                                        join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
                                                        join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                        join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                                        where ul.IsLocked == false && f.IsActive == true && bt.Prefix == "GBFR" && f.ID != 1
                                                        select new { ID = f.ID, Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName + " (" + bd.Name + ")", }).ToList(), "ID", "Name");
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "First Level" });
                List<CategoryDetail> ldata1 = new List<CategoryDetail>();
                ldata1.Add(new CategoryDetail { ID = 0, Name = "Second Level" });
                List<CategoryDetail> ldata2 = new List<CategoryDetail>();
                ldata2.Add(new CategoryDetail { ID = 0, Name = "Third Level" });

                ViewBag.CategoryList = new SelectList(ldata, "ID", "Name");
                ViewBag.LevelTwoCategoryList = new SelectList(ldata1, "ID", "Name");
                ViewBag.LevelThreeCategoryList = new SelectList(ldata2, "ID", "Name");

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

        public List<MerchantRegistrationReportViewModel> Getdata(string fromDate, string toDate, long? franchiseID, int approvedStatus, int CategoryID)
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
            List<MerchantRegistrationReportViewModel> MerchantRegistrationReportModel = new List<MerchantRegistrationReportViewModel>();
            if (approvedStatus == 1)
            {
                MerchantRegistrationReportModel = (from ul in db.UserLogins
                                                   join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                                   join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                                                   join shp in db.Shops on bd.ID equals shp.BusinessDetailID
                                                   join op in db.OwnerPlans on shp.ID equals op.OwnerID
                                                   join P in db.Plans on op.PlanID equals P.ID
                                                   join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                                   where (bd.CreateDate >= frmd &&
                                                       bd.CreateDate <= tod) && shp.FranchiseID == (franchiseID == 0 ? shp.FranchiseID : franchiseID)
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
                                                   }).OrderBy(x=>x.VendorName).ToList();
            }
            else
            {
                MerchantRegistrationReportModel = (from ul in db.UserLogins
                                                   join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                                   join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                                                   join shp in db.Shops on bd.ID equals shp.BusinessDetailID
                                                   join op in db.OwnerPlans on shp.ID equals op.OwnerID
                                                   join P in db.Plans on op.PlanID equals P.ID
                                                   join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                                   where (bd.CreateDate >= frmd &&
                                                       bd.CreateDate <= tod) && shp.FranchiseID == (franchiseID == 0 ? shp.FranchiseID : franchiseID)
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
                                                   }).OrderBy(x=>x.VendorName).ToList();
            }
            return MerchantRegistrationReportModel;
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
                       }).ToList(); ;

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
