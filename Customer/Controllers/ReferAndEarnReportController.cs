using BusinessLogicLayer;
using Gandhibagh.Models;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gandhibagh.Controllers
{
    public class ReferAndEarnReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /EarnAndReferDetailReport/
        public ActionResult Index()
        {
            List<EarnAndReferDetailReportViewModel> lEarnAndReferDetailReportViewModel = new List<EarnAndReferDetailReportViewModel>();
            try
            {
                URLCookie.SetCookies();
                //ViewBag.totalEarn = new SelectList(db.EarnDetails, "ID", "Name");
                if (Session["UID"] != null)
                {
                    long UID = Convert.ToInt64(Session["UID"]);
                    var RemainingAmount = db.EarnDetails.OrderByDescending(u => u.ModifyDate).Where(x => x.EarnUID == UID && x.IsActive==true).Select(x => x.RemainingAmount).FirstOrDefault();
                    ViewBag.RemainingAmount = Convert.ToInt64(RemainingAmount);
                    ViewBag.totalEarnAmount = Convert.ToInt64(db.EarnDetails.Where(x => x.EarnUID == UID && x.IsActive == true).Sum(x => x.EarnAmount));
                    ViewBag.totalUsedAmount = Convert.ToInt64(db.EarnDetails.Where(x => x.EarnUID == UID && x.IsActive == true).Sum(x => x.UsedAmount));
                    DataTable dt = new DataTable();
                    ReadConfig config = new ReadConfig(System.Web.HttpContext.Current.Server);
                    DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
                    List<object> paramValues = new List<object>();
                    paramValues.Add(Convert.ToInt64(Session["UID"]));
                    dt = dbOpr.GetRecords("Select_ReferAndEarnReport", paramValues);

                    lEarnAndReferDetailReportViewModel = (from n in dt.AsEnumerable()
                                                          select new EarnAndReferDetailReportViewModel
                                                          {

                                                              UserName = n.Field<string>("UserName"),
                                                              Email = n.Field<string>("Email"),
                                                              Mobile = n.Field<string>("Mobile"),
                                                              ReferenceID = n.Field<long?>("ReferenceID"),
                                                              EarnAmount = n.Field<decimal?>("EarnAmount"),
                                                              SchemeName = n.Field<string>("SchemeName"),

                                                          }).ToList();
                    long cityId = 0;
                    int franchiseId = 0;//added
                   
                    if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value != string.Empty)
                    {
                        cityId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0].Trim());
                        franchiseId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2].Trim());////added
                    }
                    //this.GetBudgetDetails(db.ReferAndEarnSchemas.Where(x=>x.IsActive==true && x.CityID==cityId).Select(x=>x.ID).FirstOrDefault());////hide
                    this.GetBudgetDetails(db.ReferAndEarnSchemas.Where(x => x.IsActive == true && x.FranchiseID == franchiseId).Select(x => x.ID).FirstOrDefault());//--added by Ashish for multiple franchise in same city--//
                }
                else
                {
                    return RedirectToRoute("Login");
                }



            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the ReferAndEarnDtailReport!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[EarnAndReferDetailReport][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the RReferAndEarnDtailReport!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[EarnAndReferDetailReport][POST:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            return View(lEarnAndReferDetailReportViewModel);
        }

        private void GetBudgetDetails(long schemeID)
        {
            try
            {
                ViewBag.RemBudgetAmt = db.SchemeBudgetTransactions.Where(x => x.ReferAndEarnSchemaID == schemeID).Select(x => x.RemainingAmt).FirstOrDefault();
                //ViewBag.ExpiryDate = db.SchemeBudgetTransactions.Where(x => x.ReferAndEarnSchemaID == schemeID).Select(x => x.ExpiryDate).FirstOrDefault() - DateTime.Now;
                DateTime date = db.SchemeBudgetTransactions.Where(x => x.ReferAndEarnSchemaID == schemeID).Select(x => x.ExpiryDate).FirstOrDefault();
                if (date > DateTime.Now)
                {
                    ViewBag.ExpiryDate = date.Subtract(DateTime.Now).Days.ToString();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
	}
}