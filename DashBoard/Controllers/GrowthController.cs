using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using BusinessLogicLayer;
using System.Web.Configuration;
using ModelLayer.Models.ViewModel;
using System.Dynamic;
using System.Data;



namespace DashBoard.Controllers
{
    public class Month
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Year
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class GrowthController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private static string fConnectionString = System.Configuration.ConfigurationSettings.AppSettings["DB_CON"].ToString();

        List<DashboardGrowthViewModel> lDashboardGrowthViewModels1 = new List<DashboardGrowthViewModel>();
        List<DashboardGrowthViewModel> lDashboardGrowthViewModels2 = new List<DashboardGrowthViewModel>();
        public void GetDropDowns()
        {
            List<Month> lMonths = new List<Month>();
            string[] Months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            int i = 1;
            foreach (var item in Months)
            {
                lMonths.Add(new Month() { ID = i, Name = item });
                i++;
            }

            List<Year> lYears = new List<Year>();
            for (int y = 2015; y <= DateTime.Now.Year; y++)
            {
                lYears.Add(new Year() { ID = y, Name = y.ToString() });
            }

            ViewBag.Years = new SelectList(lYears, "ID", "Name");
            ViewBag.Months = new SelectList(lMonths, "ID", "Name");
        }
        public void GetFranchiseId()
        {
            ViewBag.FranchisesID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false
                     && x.BusinessDetail.Pincode.City.IsActive == true), "ID", "ContactPerson");
        }
        //
        // GET: /Growth/
        public ActionResult Index()
        {
            GetFranchiseId();
            GetDropDowns();
            return View();
        }
        [HttpPost]
        public ActionResult GetReport(string submit, string ReqType, int? FromMonth, int? ToMonth, int? FranchiseID, int? Year)
        {
            int lFranchiseIDCounter = 0;
            int lThisMonth;
            int lActualOrder = 0, lActualOrderAvg = 0, lActualGMVAvg = 0, lActualGMV = 0;
            int lCumulativeActualOrder = 0;
            GrowthReport lGrowthReport = new GrowthReport(fConnectionString);
            ViewBag.ReqType = ReqType;
            ViewBag.FromMonth = FromMonth;
            ViewBag.ToMonth = ToMonth;
            ViewBag.FranchiseID = FranchiseID;
            ViewBag.Year = Year;
            ViewBag.ReqTypeAll = "All";
            if (submit == "Month")
            {
                if (FranchiseID == null)
                {
                    Session.Clear();
                }

                lThisMonth = 1;
                ViewBag.ThisMonth = lThisMonth;
            }
            else
            {
                lThisMonth = 0;
                ViewBag.ThisMonth = lThisMonth;
            }
            if (FranchiseID == null)
            {
                FranchiseID = null;
            }
            if (Year == 0)
            {
                Year = null;

            }

            if (FranchiseID == null)
            {
                lDashboardGrowthViewModels1 = lGrowthReport.DashboardGrowth(lThisMonth, FromMonth, ToMonth, FranchiseID, Year);

                foreach (var row in lDashboardGrowthViewModels1)
                {
                    if (lThisMonth == 1)
                    {

                        if (row.ActualOrder != null)
                        {
                            lActualOrder = lActualOrder + row.ActualOrder;
                        }

                        if (row.ActualGMV != null)
                        {
                            lActualGMV = lActualGMV + int.Parse(row.ActualGMV);
                        }
                    }
                    else
                    {
                        if (row.CumulativeActualOrder != null)
                        {
                            lActualOrder = lActualOrder + int.Parse(row.CumulativeActualOrder);
                        }

                        if (row.CumulativeActualGMV != null)
                        {
                            lActualGMV = lActualGMV + int.Parse(row.CumulativeActualGMV);
                        }
                    }

                    lFranchiseIDCounter++;

                }
                if (lActualOrder != 0 || lFranchiseIDCounter != 0)
                {
                    lActualOrderAvg = (lActualOrder / lFranchiseIDCounter);
                }
                if (lActualGMV != 0 || lFranchiseIDCounter != 0)
                {
                    lActualGMVAvg = lActualGMV / lFranchiseIDCounter;
                }
              
                DashboardGrowthViewModel lDashboardGrowthViewModel = new DashboardGrowthViewModel();
                lDashboardGrowthViewModel.ActualOrder = lActualOrder;
                lDashboardGrowthViewModel.ActualGMV = lActualGMV.ToString();
                lDashboardGrowthViewModel.AvgOrder = lActualOrderAvg;
                lDashboardGrowthViewModel.AvgGMV = lActualGMVAvg;
                lDashboardGrowthViewModels1.Add(lDashboardGrowthViewModel);
                //TempData["DashboardGrowthViewModel"] = lDashboardGrowthViewModels1;
                //ViewBag.lDashboardGrowthViewModels1 = lDashboardGrowthViewModels1;
                Session["dList"] = lDashboardGrowthViewModels1;
                GetDropDowns();
                GetFranchiseId();
                return View("Index", lDashboardGrowthViewModels1);

            }
            else
            {

                lDashboardGrowthViewModels2 = lGrowthReport.DashboardGrowth(lThisMonth, FromMonth, ToMonth, FranchiseID, Year);
                //mymodel.lDashboardGrowthViewModels2 = lGrowthReport.DashboardGrowth(lThisMonth, FromMonth, ToMonth, FranchiseID, Year);
                foreach (var row in lDashboardGrowthViewModels2)
                {
                    if (lThisMonth == 1)
                    {
                        if (row.ActualOrder != null)
                        {
                            lActualOrder = lActualOrder + row.ActualOrder;
                        }

                        if (row.ActualGMV != null)
                        {
                            lActualGMV = lActualGMV + int.Parse(row.ActualGMV);
                        }
                    }
                    else
                    {
                        if (row.CumulativeActualGMV != null)
                        {
                            lActualOrder = lActualOrder + int.Parse(row.CumulativeActualOrder);
                        }

                        if (row.CumulativeActualGMV != null)
                        {
                            lActualGMV = lActualGMV + int.Parse(row.CumulativeActualGMV);
                        }
                    }



                }

                ViewBag.lDashboardGrowthViewModels2 = lDashboardGrowthViewModels2;
                GetFranchiseId();
                GetDropDowns();
                return View("Index", lDashboardGrowthViewModels2);
                // return View("Index", new DashboardGrowthViewModelList { DashboardGrowthViewModelList1 = lDashboardGrowthViewModels1, DashboardGrowthViewModelList2 = lDashboardGrowthViewModels1 });

            }
        }
        public ActionResult GetReportIndividualMCO(string submit, int? FromMonth, int? ToMonth, int? FranchisesID, int? Year)
        {
            string lMode = "All";
            int lThisMonth;
            int lOrderTarget = 0, lActualOrder = 0, lOrderTargetAvg = 0, lGMVTarget = 0, lActualGMV = 0, lGMVTargetAvg = 0;

            GrowthReport lGrowthReport = new GrowthReport(fConnectionString);
            ViewBag.FromMonth = FromMonth;
            ViewBag.ToMonth = ToMonth;
            ViewBag.FranchiseID = FranchisesID;
            ViewBag.Year = Year;
            ViewBag.ReqTypeAll = "MCOWise";
            if (submit == "Month")
            {
                lThisMonth = 1;
                ViewBag.ThisMonth = lThisMonth;
            }
            else
            {
                lThisMonth = 0;
                ViewBag.ThisMonth = lThisMonth;
            }
            if (Year == 0)
            {
                Year = null;

            }
            List<DashboardGrowthViewModel> lDashboardGrowthViewModels = new List<DashboardGrowthViewModel>();
            lDashboardGrowthViewModels = lGrowthReport.GetIndividualMCO(lThisMonth, FromMonth, ToMonth, FranchisesID, Year);

            //DataSet CustomerRetentionds = new DataSet();
            //CustomerRetentionds = lGrowthReport.GetMCOWiseRetenion(FranchisesID, lMode,System.Web.HttpContext.Current.Server);
            //if (CustomerRetentionds.Tables[1].Rows.Count > 0)
            //{
            //    ViewBag.NewCustomer = Convert.ToInt32(CustomerRetentionds.Tables[1].Rows[0][0].ToString());
            //}
            //if (CustomerRetentionds.Tables[2].Rows.Count > 0)
            //{
            //    ViewBag.DriftCustomer = Convert.ToInt32(CustomerRetentionds.Tables[2].Rows[0][0].ToString());
            //}
            //if (CustomerRetentionds.Tables[3].Rows.Count > 0)
            //{
            //    ViewBag.PromisiongCustomer = Convert.ToInt32(CustomerRetentionds.Tables[3].Rows[0][0].ToString());
            //}
            //if (CustomerRetentionds.Tables[4].Rows.Count > 0)
            //{
            //    ViewBag.LoyalCustomer = Convert.ToInt32(CustomerRetentionds.Tables[4].Rows[0][0].ToString());
            //}
            //if (CustomerRetentionds.Tables[5].Rows.Count > 0)
            //{
            //    ViewBag.RedAlertCustomer = Convert.ToInt32(CustomerRetentionds.Tables[5].Rows[0][0].ToString());
            //}
            //if (CustomerRetentionds.Tables[6].Rows.Count > 0)
            //{
            //    ViewBag.SleepersCustomer = Convert.ToInt32(CustomerRetentionds.Tables[6].Rows[0][0].ToString());
            //}
            GetDropDowns();
            GetFranchiseId();
            return View("Index", lDashboardGrowthViewModels);

        }


        //
        // GET: /Growth/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Growth/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Growth/Create
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
        // GET: /Growth/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Growth/Edit/5
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
        // GET: /Growth/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Growth/Delete/5
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
