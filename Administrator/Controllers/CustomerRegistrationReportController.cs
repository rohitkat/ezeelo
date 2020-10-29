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
using PagedList;
using PagedList.Mvc;
using System.Data;


//<copyright file="CustomerRegistrationReport.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
//    </copyright>
//    <author>Harshada Raghorte</author>

namespace Administrator.Controllers
{


    public class CustomerRegistrationReportController : Controller
    {

        //Session["USER_LOGIN_ID"] = 1;
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 30;
        // GET: /CustomerRegistrationReport/
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerRegistrationReport/CanRead")]
        public ActionResult Index()
        {
            try
            {

                ViewBag.stateID = new SelectList(db.States, "ID", "Name");

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerRegistrationReportController][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerRegistrationReportController][POST:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            return View();
        }

        //[HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerRegistrationReport/CanRead")]
        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate, long? cityID, int? FranchiseID)////added int? FranchiseID
        {
            try
            {
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                int TotalCount = 0;
                int TotalPages = 0;
                ViewBag.cityID = cityID;
                ViewBag.FranchiseID = FranchiseID; ////added

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
                List<CustomerRegistrationReportViewModel> CustomerRegistrationReportModel = new List<CustomerRegistrationReportViewModel>();
                CustomerRegistrationReportModel = this.Getdata(fromDate, toDate, cityID, FranchiseID);////added  FranchiseID
                TotalCount = CustomerRegistrationReportModel.Count();
                ViewBag.TotalCount = TotalCount;
                CustomerRegistrationReportModel = CustomerRegistrationReportModel.OrderBy(x => x.CustomerID).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.PageSize = CustomerRegistrationReportModel.Count;
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                ViewBag.TotalPages = TotalPages;
                return View(CustomerRegistrationReportModel);
                //ViewBag.cityID = new SelectList(db.Plans, "ID", "planCode");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Customer Registration Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerRegistrationReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Customer Registration Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerRegistrationReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();

        }


        public ActionResult Export(string fromDate, string toDate, long? cityID, int option, int print, int? FranchiseID)////added int? FranchiseID
        {
            try
            {
                List<CustomerRegistrationReportViewModel> CustomerReport = new List<CustomerRegistrationReportViewModel>();
                CustomerReport = this.Getdata(fromDate, toDate, cityID, FranchiseID);////added FranchiseID
                if (print == 1)
                {
                    return View("ForPrint", CustomerReport);
                }
                DataTable tblProduct = new DataTable();
                tblProduct.Columns.Add("Sr.No.", typeof(long));
                tblProduct.Columns.Add("Customer ID", typeof(int));
                tblProduct.Columns.Add("Full Name", typeof(string));
                tblProduct.Columns.Add("Email", typeof(string));
                //tblProduct.Columns.Add("ProductVariantID", typeof(long));
                tblProduct.Columns.Add("Mobile No", typeof(string));
                tblProduct.Columns.Add("Address", typeof(string));
                tblProduct.Columns.Add("City Name", typeof(string));
               // tblProduct.Columns.Add("FranchiseID", typeof(string));////added
                tblProduct.Columns.Add("Franchise", typeof(string));////added
                tblProduct.Columns.Add("Registration Date", typeof(DateTime));
                
                //tblProduct.Columns.Add("Network IP", typeof(string));
                int i = 0;
                ViewBag.stateID = new SelectList(db.States, "ID", "Name");
                foreach (var row in CustomerReport)
                {
                    i = i + 1;
                    tblProduct.LoadDataRow(new object[] {i, row.CustomerID, row.FullName, row.Email, row.MobileNo, row.Address, row.CityName
                   ,row.Franchises,row.RegistrationDate}, false);////added ,row.FranchiseID
                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(tblProduct, "Customer Registration Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(tblProduct, "Customer Registration Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(tblProduct, "Customer Registration Report");
                }
               
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Customer Registration Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerRegistrationReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Customer Registration Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerRegistrationReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View("Index");

        }

        public List<CustomerRegistrationReportViewModel> Getdata(string fromDate, string toDate, long? cityID, int? FranchiseID)////added  int? FranchiseID
        {
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;

            ViewBag.cityID = cityID;
            ViewBag.FranchiseID = FranchiseID;////added


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
            //if (page == 1 && pagecount==0)
            //{
            tod = tod.AddDays(1);
            //}

            //ViewBag.toDate = tod;
            List<CustomerRegistrationReportViewModel> CustomerRegistrationReportModel = new List<CustomerRegistrationReportViewModel>();
            CustomerRegistrationReportModel = (from ul in db.UserLogins
                                               join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                               join pin in db.Pincodes on pd.PincodeID equals pin.ID
                                               join c in db.Cities on pin.CityID equals c.ID
                                               join fran in db.Franchises on pin.ID equals fran.PincodeID ////added
                                               where pd.CreateDate >= frmd &&
                                               pd.CreateDate <= tod && c.ID == (cityID == 0 ? c.ID : cityID)
                                               && fran.ID == (FranchiseID == 0 ? fran.ID : FranchiseID) ////added
                                               && !
                                                   (from B in db.BusinessDetails
                                                    select new
                                                    {
                                                        B.UserLoginID
                                                    }).Contains(new { UserLoginID = pd.UserLoginID })
                                               select new CustomerRegistrationReportViewModel
                                               {
                                                   FullName = pd.FirstName + " " + pd.MiddleName + " " + pd.LastName,
                                                   CustomerID = ul.ID,
                                                   MobileNo = ul.Mobile,
                                                   RegistrationDate = pd.CreateDate,
                                                   NetworkIP = ul.NetworkIP,
                                                   Address = pd.Address,
                                                   CityName = c.Name,
                                                   //FranchiseID=fran.ID, ////added
                                                   Franchises = db.Franchises.Where(x => x.ID == fran.ID).Select(x => x.ContactPerson).FirstOrDefault().ToString(),  ////added
                                                   Email = ul.Email


                                               }).OrderBy(x=>x.FullName).ToList();
            return CustomerRegistrationReportModel;
        }

        public JsonResult GetCityByStateId(int stateID)
        {
            List<District> ldistrict = new List<District>();
            List<City> lcity = new List<City>();
            List<StateCityFranchiseMerchantViewModel> city = new List<StateCityFranchiseMerchantViewModel>();
            try
            {
                //var district = (List<District>)db.Districts.Where(u => u.StateID == stateID).ToList();
                //var district = from cust in db.States 
                //                        select cust;

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
                ModelState.AddModelError("Error", "There's Something wrong in fill city!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerRegistrationReportController][POST:GetCityByStateId]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in fill city!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerRegistrationReportController][POST:GetCityByStateId]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }

            return Json(city.Distinct().OrderBy(x => x.CityName).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult getFranchise(int CityID)////added
        {
            List<tempData> objODP = new List<tempData>();

            objODP = db.Franchises
                    .Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false && x.BusinessDetail.Pincode.City.IsActive == true
                    && x.BusinessDetail.Pincode.City.ID == CityID)
                    //.Select(x => new tempData { text = x.ID.ToString(), value = x.ID } ////ContactPerson->ID
                    .Select(x => new tempData { text = x.ContactPerson.ToString(), value = x.ID }
                    ).OrderBy(x => x.text)
                    .ToList();

            return Json(objODP, JsonRequestBehavior.AllowGet);
        }
        public class tempData////added
        {
            public Int64 value;
            public string text;
        }



      
    }
}
