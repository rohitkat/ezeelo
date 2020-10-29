using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Collections.ObjectModel;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Data;
using System.Data.SqlClient;


namespace DashBoard.Controllers
{
    public class FranchiseWiseDataController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /FranchiseWiseData/
        DateTime frmd, tod;
        public ActionResult Index()
        {
            
            try
            {
               // var a = db.Franchises.Select(x => x.ContactPerson);
                 
                ViewBag.FranchisesID = new SelectList(db.Franchises.Where(x=>x.ID!=1 && x.IsActive==true && x.BusinessDetail.UserLogin.IsLocked==false
                   && x.BusinessDetail.Pincode.City.IsActive==true), "ID", "ContactPerson");
          
                //ViewBag.ParentCategoryId = new SelectList(db.Categories, "ID", "Name");
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseWiseDataController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseWiseDataController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        public ActionResult GetReport(string Submit, long? FranchisesID,string FromDate, string ToDate)
        {
           ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
           string conn = readCon.DB_CONNECTION;
           SqlConnection con = new SqlConnection(conn);
           int TotalCustomerCount=0;
           string FrequencyFlag="",Mode=" ";
          
            //List<DashboardFranchiseViewModel> dashboardFranchiseViewModel = new List<DashboardFranchiseViewModel>();
            FranchiseDashboardCustomerRetention lFranchiseDashboardList = new FranchiseDashboardCustomerRetention();
            FranchiseDshboardCustomerRet lFranchiseDshboardCustomerRet = new FranchiseDshboardCustomerRet();
            try
            {
                ViewBag.FranchisesID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false
                 && x.BusinessDetail.Pincode.City.IsActive == true), "ID", "ContactPerson");
          
                ViewBag.FranchisesSelID = FranchisesID;
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                Mode="ALL";
                if (Submit == "DAY")
                {
                    FrequencyFlag = Submit;
                }
                else if (Submit == "WEEK")
                {
                    FrequencyFlag = Submit;
                }
                else if (Submit == "MONTH")
                {
                    FrequencyFlag = Submit;
                }
                else if (Submit == "SEARCH")
                {
                    FrequencyFlag = "NONE";

                    string from = FromDate.ToString();
                    string[] f = from.Split('/');
                    string[] ftime = f[2].Split(' ');
                    frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                    //frmd = CommonFunctions.GetDate(from);
                    frmd = Convert.ToDateTime(frmd.ToShortDateString());

                    string to = ToDate.ToString();
                    string[] t = to.Split('/');
                    string[] ttime = t[2].Split(' ');
                    tod = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);
                    //tod = CommonFunctions.GetProperDateTime(to);
                    tod = Convert.ToDateTime(ToDate);
                    tod = Convert.ToDateTime(tod.ToShortDateString());
                    tod = tod.AddDays(1);
                }
                try
                {
                    if (FrequencyFlag != "NONE")
                    {
                        SqlCommand sqlComm = new SqlCommand("ReportDashBoardGetOrdersFranchiseWise", con);
                        sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.Int).Value = FranchisesID;
                        sqlComm.Parameters.AddWithValue("@FrequencyFlag", SqlDbType.Int).Value = FrequencyFlag;                   
                        sqlComm.Parameters.Add("@TotalCustomerCount", SqlDbType.Int).Direction = ParameterDirection.Output;
                        sqlComm.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        sqlComm.ExecuteNonQuery();
                        TotalCustomerCount = Convert.ToInt32(sqlComm.Parameters["@TotalCustomerCount"].Value.ToString());
                        //return TotalCustomerCount;                      
                        SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        con.Close();
                        lFranchiseDshboardCustomerRet.listDashboardFranchiseViewModel = BusinessLogicLayer.Helper.CreateListFromTable<DashboardFranchiseViewModel>(dt);
                    }
                    else
                    {
                        
                        SqlCommand sqlComm = new SqlCommand("ReportDashBoardGetOrdersFranchiseWise", con);
                        sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.Int).Value = FranchisesID;
                        sqlComm.Parameters.AddWithValue("@FrequencyFlag", SqlDbType.Int).Value = FrequencyFlag;
                        sqlComm.Parameters.AddWithValue("@FromDate", SqlDbType.Int).Value = frmd;
                        sqlComm.Parameters.AddWithValue("@ToDate", SqlDbType.Int).Value = tod;
                        sqlComm.Parameters.Add("@TotalCustomerCount", SqlDbType.Int).Direction = ParameterDirection.Output;
                        sqlComm.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        sqlComm.ExecuteNonQuery();
                        TotalCustomerCount = Convert.ToInt32(sqlComm.Parameters["@TotalCustomerCount"].Value.ToString());
                        //return TotalCustomerCount;

                        SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        con.Close();
                        lFranchiseDshboardCustomerRet.listDashboardFranchiseViewModel = BusinessLogicLayer.Helper.CreateListFromTable<DashboardFranchiseViewModel>(dt);
                    }

                       //----------------------------To Show Customer Retenion Detail---------------------------------------------------
                      
                        SqlCommand sqlCommand = new SqlCommand("ReportDashBoardCustomerType", con);
                        sqlCommand.Parameters.AddWithValue("@Mode", SqlDbType.Int).Value = Mode;
                        sqlCommand.Parameters.AddWithValue("@FranchiseID", SqlDbType.Int).Value = FranchisesID;
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        sqlCommand.ExecuteNonQuery();                      
                        //return TotalCustomerCount;
                        SqlDataAdapter lda = new SqlDataAdapter(sqlCommand);
                        DataSet CustomerRetentionds = new DataSet();
                        lda.Fill(CustomerRetentionds);
                        con.Close();
                        if (CustomerRetentionds.Tables[1].Rows.Count > 0)
                        {
                            lFranchiseDashboardList.NewCustomer = Convert.ToInt32(CustomerRetentionds.Tables[1].Rows[0][0].ToString());
                        }
                        if (CustomerRetentionds.Tables[2].Rows.Count > 0)
                        {
                            lFranchiseDashboardList.DriftCustomer = Convert.ToInt32(CustomerRetentionds.Tables[2].Rows[0][0].ToString());
                        }
                        if (CustomerRetentionds.Tables[3].Rows.Count > 0)
                        {
                            lFranchiseDashboardList.PromisiongCustomer =  Convert.ToInt32(CustomerRetentionds.Tables[3].Rows[0][0].ToString());
                        }
                        if (CustomerRetentionds.Tables[4].Rows.Count > 0)
                        {
                            lFranchiseDashboardList.LoyalCustomer = Convert.ToInt32(CustomerRetentionds.Tables[4].Rows[0][0].ToString());
                        }
                        if (CustomerRetentionds.Tables[5].Rows.Count > 0)
                        {
                            lFranchiseDashboardList.RedAlertCustomer = Convert.ToInt32(CustomerRetentionds.Tables[5].Rows[0][0].ToString());
                        }
                        if (CustomerRetentionds.Tables[6].Rows.Count > 0)
                        {
                            lFranchiseDashboardList.SleepersCustomer = Convert.ToInt32(CustomerRetentionds.Tables[6].Rows[0][0].ToString()); 
                        }
                        lFranchiseDshboardCustomerRet.lFranchiseDashboardCustomerRetention = lFranchiseDashboardList;
                        
                        //ViewBag.franchiseDashboardCustomerRetention = franchiseDashboardCustomerRetention;             
                    
                    //dashboardFranchiseViewModel = dashboardFranchiseViewModel.Where(x => x.CreateDate >= frmd &&
                    //                           x.CreateDate <= tod && x.ShopID == (merchantID == 0 ? x.ShopID : merchantID)).ToList();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                
                ViewBag.TotalCustomerCount = TotalCustomerCount;
                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View("Index", lFranchiseDshboardCustomerRet);
        
        }
	}
}