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
using PagedList;
using PagedList.Mvc;
using System.Data;

namespace Franchise.Controllers
{
    public class ShopDetailsReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int pageSize = 50;
        //
        // GET: /ShopDetailsReport/
        public ActionResult Index()
        {
            return View();
        }
        //[HttpPost]
        //public ActionResult GetReport(int? page, int Status, string ShopName, int? print)
        //{
        //    try
        //    {
        //        long FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //GetFranchiseID();
        //        int pageNumber = (page ?? 1);
        //        ViewBag.PageNumber = pageNumber;
        //        ViewBag.PageSize = pageSize;
        //        ViewBag.Status = Status;
        //        ViewBag.ShopName = ShopName;
        //        int TotalCount = 0;
        //        Boolean IsLive = false;

        //        if (Status == 1)
        //        {
        //            IsLive = true;
        //        }
        //        List<ShopDetailReportViewModel> ShopReport = new List<ShopDetailReportViewModel>();
        //        if (Status == 0 && ShopName == null)
        //        {
        //            ShopReport = (from s in db.Shops
        //                          //join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
        //                          //join ul in db.UserLogins on bd.UserLoginID equals ul.ID
        //                          join pd in db.PersonalDetails on s.BusinessDetail.UserLogin.ID equals pd.UserLoginID
        //                          where s.FranchiseID == FranchiseID && s.IsActive == true
        //                          select new ShopDetailReportViewModel
        //                          {
        //                              UserLoginID = s.BusinessDetail.UserLogin.ID,
        //                              Email = s.BusinessDetail.UserLogin.Email,
        //                              Mobile = s.BusinessDetail.UserLogin.Mobile,
        //                              Password = s.BusinessDetail.UserLogin.Password,
        //                              ShopID = s.ID,
        //                              ShopName = s.Name,
        //                              MerchantName = pd.FirstName + " " + pd.LastName,
        //                              AlternateMobile = s.Mobile,
        //                              LandLine = s.Landline,
        //                              Address = s.Address,
        //                              IsActive = s.IsActive,
        //                              IsLive = s.IsLive,
        //                              ShopCreateDate = s.CreateDate
        //                          }).OrderBy(x => x.ShopName).ToList();
        //        }
        //        else if (Status > 0 && ShopName == null)
        //        {
        //            ShopReport = (from s in db.Shops
        //                          //join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
        //                          //join ul in db.UserLogins on bd.UserLoginID equals ul.ID
        //                          join pd in db.PersonalDetails on s.BusinessDetail.UserLogin.ID equals pd.UserLoginID
        //                          where s.FranchiseID == FranchiseID && s.IsLive == IsLive &&s.IsActive == true
        //                          select new ShopDetailReportViewModel
        //                          {
        //                              UserLoginID = s.BusinessDetail.UserLogin.ID,
        //                              Email = s.BusinessDetail.UserLogin.Email,
        //                              Mobile = s.BusinessDetail.UserLogin.Mobile,
        //                              Password = s.BusinessDetail.UserLogin.Password,
        //                              ShopID = s.ID,
        //                              ShopName = s.Name,
        //                              MerchantName = pd.FirstName + " " + pd.LastName,
        //                              AlternateMobile = s.Mobile,
        //                              LandLine = s.Landline,
        //                              Address = s.Address,
        //                              IsActive = s.IsActive,
        //                              IsLive = s.IsLive,
        //                              ShopCreateDate = s.CreateDate
        //                          }).OrderBy(x => x.ShopName).ToList();
        //        }
        //        else if (Status == 0 && ShopName != null)
        //        {
        //            ShopReport = (from s in db.Shops
        //                          //join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
        //                          //join ul in db.UserLogins on bd.UserLoginID equals ul.ID
        //                          join pd in db.PersonalDetails on s.BusinessDetail.UserLogin.ID equals pd.UserLoginID
        //                          where s.FranchiseID == FranchiseID
        //                          where s.Name.Contains(ShopName) && s.IsActive == true
        //                          select new ShopDetailReportViewModel
        //                          {
        //                              UserLoginID = s.BusinessDetail.UserLogin.ID,
        //                              Email = s.BusinessDetail.UserLogin.Email,
        //                              Mobile = s.BusinessDetail.UserLogin.Mobile,
        //                              Password = s.BusinessDetail.UserLogin.Password,
        //                              ShopID = s.ID,
        //                              ShopName = s.Name,
        //                              MerchantName = pd.FirstName + " " + pd.LastName,
        //                              AlternateMobile = s.Mobile,
        //                              LandLine = s.Landline,
        //                              Address = s.Address,
        //                              IsActive = s.IsActive,
        //                              IsLive = s.IsLive,
        //                              ShopCreateDate = s.CreateDate
        //                          }).OrderBy(x => x.ShopName).ToList();
        //        }
        //        else if (Status > 0 && ShopName != null)
        //        {
        //            ShopReport = (from s in db.Shops
        //                          //join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
        //                          //join ul in db.UserLogins on bd.UserLoginID equals ul.ID
        //                          join pd in db.PersonalDetails on s.BusinessDetail.UserLogin.ID equals pd.UserLoginID
        //                          where s.FranchiseID == FranchiseID && s.IsLive == IsLive && s.Name.Contains(ShopName)
        //                          && s.IsActive == true
        //                          select new ShopDetailReportViewModel
        //                          {
        //                              UserLoginID = s.BusinessDetail.UserLogin.ID,
        //                              Email = s.BusinessDetail.UserLogin.Email,
        //                              Mobile = s.BusinessDetail.UserLogin.Mobile,
        //                              Password = s.BusinessDetail.UserLogin.Password,
        //                              ShopID = s.ID,
        //                              ShopName = s.Name,
        //                              MerchantName = pd.FirstName + " " + pd.LastName,
        //                              AlternateMobile = s.Mobile,
        //                              LandLine = s.Landline,
        //                              Address = s.Address,
        //                              IsActive = s.IsActive,
        //                              IsLive = s.IsLive,
        //                              ShopCreateDate = s.CreateDate
        //                          }).OrderBy(x => x.ShopName).ToList();
        //        }
        //        TotalCount = ShopReport.Count();
        //        ViewBag.TotalCount = TotalCount;
        //        if (print == 1 && print != null)
        //        {
        //            return View("ForPrint",ShopReport.ToList().OrderBy(x => x.ShopName));
        //        }
                
        //        //ShopReport = ShopReport.OrderBy(x => x.ShopName).Skip((page - 1) * PageSize).Take(PageSize).ToList();
        //        //ViewBag.PageSize = ShopReport.Count;
        //        //TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
        //        //ViewBag.TotalPages = TotalPages;
        //        //return View(ShopReport);
        //        return View(ShopReport.ToList().OrderBy(x => x.ShopName).ToPagedList(pageNumber, pageSize));

        //    }
        //    catch (BusinessLogicLayer.MyException myEx)
        //    {
        //        ModelState.AddModelError("Error", "There's Something wrong in loading Shop Detail Report!!");

        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
        //            + "[ShopDetailsReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
        //            BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("Error", "There's Something wrong in loading Shop Detail Report!!");

        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[ShopDetailsReportController][POST:GetReport]",
        //            BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
        //    }
        //    return View();
        //}

        public ActionResult GetReport(int? page, int Status, string ShopName, int? print, int? CatID)
        {
            try
            {
                long FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //GetFranchiseID();
                ViewBag.CatID = new SelectList(db.Categories.Where(x => x.Level == 1).ToList(), "ID", "Name");
                ViewBag.selectedCatID = CatID;
                int pageNumber = (page ?? 1);
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.Status = Status;
                ViewBag.ShopName = ShopName;
                //ViewBag.franchiseId = FranchiseList;
                int TotalCount = 0;
                Boolean IsLive = false;

                if (Status == 1)
                {
                    IsLive = true;
                }
                List<ShopDetailReportViewModel> ShopReport = new List<ShopDetailReportViewModel>();
                if (CatID != null)
                {
                    ShopReport = (from S in db.Shops
                                  join pd in db.PersonalDetails on S.BusinessDetail.UserLogin.ID equals pd.UserLoginID
                                  join OP in db.OwnerPlans on S.ID equals OP.OwnerID
                                  join P in db.Plans on OP.PlanID equals P.ID
                                  join OPC in db.PlanCategoryCharges on OP.PlanID equals OPC.PlanID
                                  join C in db.Categories on OPC.CategoryID equals C.ID
                                  join C1 in db.Categories on C.ParentCategoryID equals C1.ID
                                  join C2 in db.Categories on C1.ParentCategoryID equals C2.ID
                                  where S.FranchiseID == FranchiseID && C2.ID == CatID
                                  && S.IsActive == true && OP.IsActive == true && P.IsActive == true && OPC.IsActive == true
                                  && C.IsActive == true && C1.IsActive == true && C2.IsActive == true && OP.Plan.PlanCode.StartsWith("GBMR")
                                  select new ShopDetailReportViewModel
                                  {
                                      UserLoginID = S.BusinessDetail.UserLogin.ID,
                                      Email = S.BusinessDetail.UserLogin.Email,
                                      Mobile = S.BusinessDetail.UserLogin.Mobile,
                                      Password = S.BusinessDetail.UserLogin.Password,
                                      ShopID = S.ID,
                                      ShopName = S.Name,
                                      MerchantName = pd.FirstName + " " + pd.LastName,
                                      AlternateMobile = S.Mobile,
                                      LandLine = S.Landline,
                                      Address = S.Address,
                                      IsActive = S.IsActive,
                                      IsLive = S.IsLive,
                                      ShopCreateDate = S.CreateDate
                                  }).Distinct().OrderBy(x => x.ShopName).ToList();
                }
                else
                {
                    ShopReport = (from s in db.Shops
                                  join pd in db.PersonalDetails on s.BusinessDetail.UserLogin.ID equals pd.UserLoginID
                                  where s.FranchiseID == FranchiseID
                                  where s.Name.Contains(ShopName) && s.IsActive == true
                                  select new ShopDetailReportViewModel
                                  {
                                      UserLoginID = s.BusinessDetail.UserLogin.ID,
                                      Email = s.BusinessDetail.UserLogin.Email,
                                      Mobile = s.BusinessDetail.UserLogin.Mobile,
                                      Password = s.BusinessDetail.UserLogin.Password,
                                      ShopID = s.ID,
                                      ShopName = s.Name,
                                      MerchantName = pd.FirstName + " " + pd.LastName,
                                      AlternateMobile = s.Mobile,
                                      LandLine = s.Landline,
                                      Address = s.Address,
                                      IsActive = s.IsActive,
                                      IsLive = s.IsLive,
                                      ShopCreateDate = s.CreateDate
                                  }).Distinct().OrderBy(x => x.ShopName).ToList();
                }
                if (ShopName != null && ShopName != "")
                {
                    ShopReport = ShopReport.Where(x => x.ShopName != null && x.ShopName.ToLower().Trim().Contains(ShopName.ToLower().Trim())).Distinct().ToList();
                }
                if (Status == 1)
                {
                    ShopReport = ShopReport.Where(x => x.IsLive != null && x.IsLive == true).Distinct().ToList();
                }
                if (Status == 2)
                {
                    ShopReport = ShopReport.Where(x => x.IsLive != null && x.IsLive == false).Distinct().ToList();
                }
              
                TotalCount = ShopReport.Count();
                ViewBag.TotalCount = TotalCount;
                if (print == 1 && print != null)
                {
                    return View("ForPrint", ShopReport.ToList().Distinct().OrderBy(x => x.ShopName));
                }

                TempData["ShopData"] = ShopReport;

                return View("GetReport", ShopReport.ToList().Distinct().OrderBy(x => x.ShopName).ToPagedList(pageNumber, pageSize));

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Shop Detail Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShopDetailsReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Shop Detail Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShopDetailsReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }
        public ActionResult Export(int Status, int option, int print, string ShopName, int? CatID)
        {

            try
            {
                long FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //GetFranchiseID();
                ViewBag.CatID = new SelectList(db.Categories.Where(x => x.Level == 1).ToList(), "ID", "Name");
                ViewBag.selectedCatID = CatID;
                //int pageNumber = (page ?? 1);
                //ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.Status = Status;
                ViewBag.ShopName = ShopName;
                //ViewBag.franchiseId = FranchiseList;
                int TotalCount = 0;
                Boolean IsLive = false;

                if (Status == 1)
                {
                    IsLive = true;
                }
                List<ShopDetailReportViewModel> ShopReport = new List<ShopDetailReportViewModel>();
                if (CatID != null)
                {
                    ShopReport = (from S in db.Shops
                                  join pd in db.PersonalDetails on S.BusinessDetail.UserLogin.ID equals pd.UserLoginID
                                  join OP in db.OwnerPlans on S.ID equals OP.OwnerID
                                  join P in db.Plans on OP.PlanID equals P.ID
                                  join OPC in db.PlanCategoryCharges on OP.PlanID equals OPC.PlanID
                                  join C in db.Categories on OPC.CategoryID equals C.ID
                                  join C1 in db.Categories on C.ParentCategoryID equals C1.ID
                                  join C2 in db.Categories on C1.ParentCategoryID equals C2.ID
                                  where S.FranchiseID == FranchiseID && C2.ID == CatID
                                  && S.IsActive == true && OP.IsActive == true && P.IsActive == true && OPC.IsActive == true
                                  && C.IsActive == true && C1.IsActive == true && C2.IsActive == true && OP.Plan.PlanCode.StartsWith("GBMR")
                                  select new ShopDetailReportViewModel
                                  {
                                      UserLoginID = S.BusinessDetail.UserLogin.ID,
                                      Email = S.BusinessDetail.UserLogin.Email,
                                      Mobile = S.BusinessDetail.UserLogin.Mobile,
                                      Password = S.BusinessDetail.UserLogin.Password,
                                      ShopID = S.ID,
                                      ShopName = S.Name,
                                      MerchantName = pd.FirstName + " " + pd.LastName,
                                      AlternateMobile = S.Mobile,
                                      LandLine = S.Landline,
                                      Address = S.Address,
                                      IsActive = S.IsActive,
                                      IsLive = S.IsLive,
                                      ShopCreateDate = S.CreateDate
                                  }).Distinct().OrderBy(x => x.ShopName).ToList();
                }
                else
                {
                    ShopReport = (from s in db.Shops
                                  join pd in db.PersonalDetails on s.BusinessDetail.UserLogin.ID equals pd.UserLoginID
                                  where s.FranchiseID == FranchiseID
                                  where s.Name.Contains(ShopName) && s.IsActive == true
                                  select new ShopDetailReportViewModel
                                  {
                                      UserLoginID = s.BusinessDetail.UserLogin.ID,
                                      Email = s.BusinessDetail.UserLogin.Email,
                                      Mobile = s.BusinessDetail.UserLogin.Mobile,
                                      Password = s.BusinessDetail.UserLogin.Password,
                                      ShopID = s.ID,
                                      ShopName = s.Name,
                                      MerchantName = pd.FirstName + " " + pd.LastName,
                                      AlternateMobile = s.Mobile,
                                      LandLine = s.Landline,
                                      Address = s.Address,
                                      IsActive = s.IsActive,
                                      IsLive = s.IsLive,
                                      ShopCreateDate = s.CreateDate
                                  }).Distinct().OrderBy(x => x.ShopName).ToList();
                }
                if (ShopName != null && ShopName != "")
                {
                    ShopReport = ShopReport.Where(x => x.ShopName != null && x.ShopName.ToLower().Trim().Contains(ShopName.ToLower().Trim())).Distinct().ToList();
                }
                if (Status == 1)
                {
                    ShopReport = ShopReport.Where(x => x.IsLive != null && x.IsLive == true).Distinct().ToList();
                }
                if (Status == 2)
                {
                    ShopReport = ShopReport.Where(x => x.IsLive != null && x.IsLive == false).Distinct().ToList();
                }
              
                //List<ShopDetailReportViewModel> ShopReport = new List<ShopDetailReportViewModel>();
                //ShopReport = this.Getdata(Status);
                ViewBag.TotalCount = ShopReport.Count();
                if (print == 1)
                {
                    return View("ForPrint", ShopReport);
                }
                DataTable tblProduct = new DataTable();
                tblProduct.Columns.Add("Sr.No.", typeof(long));
                tblProduct.Columns.Add("Shop Name", typeof(string));
                tblProduct.Columns.Add("Merchant Name", typeof(string));
                tblProduct.Columns.Add("UserLogin ID", typeof(string));
                tblProduct.Columns.Add("Login Email-ID", typeof(string));
                tblProduct.Columns.Add("Login Mobile No", typeof(string));
                tblProduct.Columns.Add("Password", typeof(string));
                tblProduct.Columns.Add("IsLive", typeof(string));
                tblProduct.Columns.Add("Address", typeof(string));
                tblProduct.Columns.Add("Alternate Mobile", typeof(string));
                tblProduct.Columns.Add("Landline No", typeof(string));



                //tblProduct.Columns.Add("Network IP", typeof(string));
                int i = 0;
                ViewBag.stateID = new SelectList(db.States, "ID", "Name");
                foreach (var row in ShopReport)
                {
                    i = i + 1;
                    string live = "";
                    if (row.IsLive == true)
                    {
                        live = "live";
                    }
                    else
                    {
                        live = "Not live";
                    }
                    tblProduct.LoadDataRow(new object[] {i, row.ShopName, row.MerchantName, row.UserLoginID, row.Email, row.Mobile, row.Password,live
                   ,row.Address,row.AlternateMobile,row.LandLine}, false);
                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(tblProduct, "Shop Detail Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(tblProduct, "Shop Detail Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(tblProduct, "Shop Detail Report");
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



        public List<ShopDetailReportViewModel> Getdata(int Status)
        {
            Boolean IsLive = false;
            int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);

            if (Status == 1)
            {
                IsLive = true;
            }

            List<ShopDetailReportViewModel> ShopReport = new List<ShopDetailReportViewModel>();
            if (Status == 0)
            {
                 
                ShopReport = (from s in db.Shops
                              //join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                              //join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                              join pd in db.PersonalDetails on s.BusinessDetail.UserLogin.ID equals pd.UserLoginID
                              where s.FranchiseID == franchiseID && s.IsActive == true
                              select new ShopDetailReportViewModel
                              {
                                  UserLoginID = s.BusinessDetail.UserLogin.ID,
                                  Email = s.BusinessDetail.UserLogin.Email,
                                  Mobile = s.BusinessDetail.UserLogin.Mobile,
                                  Password = s.BusinessDetail.UserLogin.Password,
                                  ShopID = s.ID,
                                  ShopName = s.Name,
                                  MerchantName = pd.FirstName + " " + pd.LastName,
                                  AlternateMobile = s.Mobile,
                                  LandLine = s.Landline,
                                  Address = s.Address,
                                  IsActive = s.IsActive,
                                  IsLive = s.IsLive,
                                  ShopCreateDate = s.CreateDate
                              }).OrderBy(x => x.ShopName).ToList();
            }
            else if (Status > 0)
            {
                ShopReport = (from s in db.Shops
                              //join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                              //join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                              join pd in db.PersonalDetails on s.BusinessDetail.UserLogin.ID equals pd.UserLoginID
                              where s.FranchiseID == franchiseID && s.IsLive == IsLive && s.IsActive == true
                              select new ShopDetailReportViewModel
                              {
                                  UserLoginID = s.BusinessDetail.UserLogin.ID,
                                  Email = s.BusinessDetail.UserLogin.Email,
                                  Mobile = s.BusinessDetail.UserLogin.Mobile,
                                  Password = s.BusinessDetail.UserLogin.Password,
                                  ShopID = s.ID,
                                  ShopName = s.Name,
                                  MerchantName = pd.FirstName + " " + pd.LastName,
                                  AlternateMobile = s.Mobile,
                                  LandLine = s.Landline,
                                  Address = s.Address,
                                  IsActive = s.IsActive,
                                  IsLive = s.IsLive,
                                  ShopCreateDate = s.CreateDate
                              }).OrderBy(x => x.ShopName).ToList();
            }

            return ShopReport;
        }



    }
}

       
    

