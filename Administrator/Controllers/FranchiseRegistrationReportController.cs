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

//<copyright file="FranchiseRegistrationReport.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
//    </copyright>
//    <author>Harshada Raghorte</author>

namespace Administrator.Controllers
{
    public class FranchiseRegistrationReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 20;
        //
        // GET: /FranchiseRegistrationReport/
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseRegistrationReport/CanRead")]
        public ActionResult Index()
        {
            try
            {
                ViewBag.stateID = new SelectList(db.States, "ID", "Name");
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseRegistrationReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseRegistrationReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                     return View();
            }
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "FranchiseRegistrationReport/CanRead")]

        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate, long? cityID)
        {
            try
            {
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                int TotalCount = 0;
                int TotalPages = 0;
                ViewBag.cityID = cityID;
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
               // if (page == 1 && pagecount == 0)
                //{
                    tod = tod.AddDays(1);
                //}

                //ViewBag.toDate = tod;
                var FranchiseRegistrationReportModel = (from ul in db.UserLogins
                                                       join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                                       join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                                                       join fr in db.Franchises on bd.ID equals fr.BusinessDetailID
                                                       join pin in db.Pincodes on pd.PincodeID equals pin.ID
                                                       join ct in db.Cities on pin.CityID equals ct.ID
                                                       join dt in db.Districts on ct.DistrictID equals dt.ID
                                                       join st in db.States on dt.StateID equals st.ID
                                                       //join shp in db.Shops on bd.ID equals shp.ID
                                                       where (fr.CreateDate >= frmd &&
                                                       fr.CreateDate <= tod)
                                                      && pin.CityID == (cityID == 0 ? pin.CityID : cityID)&& bd.BusinessTypeID==2
                                                       select new FranchiseRegistrationReportViewModel
                                                       {
                                                           FranchiseName = bd.Name,
                                                           FranchiseRegistrationNo = fr.ID,
                                                           OwnerName = pd.FirstName + " " + pd.MiddleName + " " + pd.LastName,
                                                           MobileNo = bd.Mobile,
                                                           Email = bd.Email,
                                                           Address = fr.Address,
                                                           Pincode = pin.Name,
                                                           State = st.Name,
                                                           City = ct.Name,
                                                           ContactPerson = fr.ContactPerson,
                                                           ContactPersonEmail = fr.Email,
                                                           RegistrationDate = fr.CreateDate,

                                                       }).ToList();
                TotalCount = FranchiseRegistrationReportModel.Count();
                ViewBag.TotalCount = TotalCount;
                FranchiseRegistrationReportModel = FranchiseRegistrationReportModel.OrderBy(x => x.ID).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.PageSize = FranchiseRegistrationReportModel.Count;
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                ViewBag.TotalPages = TotalPages;
                return View(FranchiseRegistrationReportModel);

                return View(FranchiseRegistrationReportModel);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Franchise Registration Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseRegistrationReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Franchise Registration Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseRegistrationReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        public ActionResult Export(string fromDate, string toDate, long? cityID, int option, int print)
        {
            try
            {
                List<FranchiseRegistrationReportViewModel> FranchiseReport = new List<FranchiseRegistrationReportViewModel>();
                FranchiseReport = this.Getdata(fromDate, toDate, cityID);
                if (print == 1)
                {
                    return View("ForPrint", FranchiseReport);
                }
                DataTable dt = new DataTable();
                dt.Columns.Add("Sr.No.", typeof(long));
                dt.Columns.Add("Franchise Name", typeof(string));
                dt.Columns.Add(" Franchise Registration No", typeof(string));
                dt.Columns.Add("Mobile No", typeof(string));
                //tblProduct.Columns.Add("ProductVariantID", typeof(long));
                dt.Columns.Add("Email", typeof(string));
                dt.Columns.Add("Address", typeof(string));
                dt.Columns.Add("Landmark", typeof(string));
                dt.Columns.Add("Pincode", typeof(int));
                dt.Columns.Add("State", typeof(string));
                dt.Columns.Add("City", typeof(string));
                dt.Columns.Add("Contact Person", typeof(string));
                dt.Columns.Add("Contact Person Email", typeof(string));
                dt.Columns.Add("Registration Date", typeof(DateTime));
                //tblProduct.Columns.Add("Network IP", typeof(string));
                int i = 0;
                ViewBag.stateID = new SelectList(db.States, "ID", "Name");
                foreach (var row in FranchiseReport)
                {
                    i = i + 1;
                    dt.LoadDataRow(new object[] {i, row.FranchiseName, row.FranchiseRegistrationNo, row.MobileNo, row.Email, row.Address, row.Landmark
                   ,row.Pincode,row.State,row.City,row.ContactPerson,row.ContactPersonEmail,row.RegistrationDate}, false);
                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(dt, "Franchise Registration Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(dt, "Franchise Registration Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(dt, "Franchise Registration Report");
                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Exporting Franchise Registration Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerRegistrationReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Franchise Registration Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerRegistrationReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View("Index");

        }

        public List<FranchiseRegistrationReportViewModel> Getdata(string fromDate, string toDate, long? cityID)
        {
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;

            ViewBag.cityID = cityID;


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
            List<FranchiseRegistrationReportViewModel> FranchiseRegistrationReportModel = new List<FranchiseRegistrationReportViewModel>();
            FranchiseRegistrationReportModel = (from ul in db.UserLogins
                                                join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                                join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                                                join fr in db.Franchises on bd.ID equals fr.BusinessDetailID
                                                join pin in db.Pincodes on pd.PincodeID equals pin.ID
                                                join ct in db.Cities on pin.CityID equals ct.ID
                                                join dt in db.Districts on ct.DistrictID equals dt.ID
                                                join st in db.States on dt.StateID equals st.ID
                                                //join shp in db.Shops on bd.ID equals shp.ID
                                                where (fr.CreateDate >= frmd &&
                                                fr.CreateDate <= tod)
                                               && pin.CityID == (cityID == 0 ? pin.CityID : cityID) && bd.BusinessTypeID == 2
                                                select new FranchiseRegistrationReportViewModel
                                                {
                                                    FranchiseName = bd.Name,
                                                    FranchiseRegistrationNo = fr.ID,
                                                    OwnerName = pd.FirstName + " " + pd.MiddleName + " " + pd.LastName,
                                                    MobileNo = bd.Mobile,
                                                    Email = bd.Email,
                                                    Address = fr.Address,
                                                    Pincode = pin.Name,
                                                    State = st.Name,
                                                    City = ct.Name,
                                                    ContactPerson = fr.ContactPerson,
                                                    ContactPersonEmail = fr.Email,
                                                    RegistrationDate = fr.CreateDate,

                                                }).OrderBy(x=>x.FranchiseName).ToList();
            return FranchiseRegistrationReportModel;
        }
        public JsonResult GetCityByStateId(int stateID)
        {
            //var district = (List<District>)db.Districts.Where(u => u.StateID == stateID).ToList();
            //var district = from cust in db.States 
            //                        select cust;
            List<District> ldistrict = new List<District>();
            List<City> lcity = new List<City>();
            List<StateCityFranchiseMerchantViewModel> city = new List<StateCityFranchiseMerchantViewModel>();
            try {
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
                ModelState.AddModelError("Error", "There's Something wrong in filling City dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseRegistrationReportController][POST:GetCityByStateId]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling City dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseRegistrationReportController][POST:GetCityByStateId]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return Json(city.Distinct().OrderBy(x => x.CityName).ToList(), JsonRequestBehavior.AllowGet);
        }


        //
        // GET: /FranchiseRegistrationReport/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /FranchiseRegistrationReport/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /FranchiseRegistrationReport/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /FranchiseRegistrationReport/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /FranchiseRegistrationReport/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /FranchiseRegistrationReport/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /FranchiseRegistrationReport/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
