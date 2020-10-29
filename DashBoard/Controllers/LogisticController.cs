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
    public class LogisticController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        DateTime frmd, tod;
        //
        // GET: /Logistic/
        public ActionResult Index()
        {
            try
            {
               // var a = db.Franchises.Select(x => x.ContactPerson);
                ViewBag.FranchisesID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false
                 && x.BusinessDetail.Pincode.City.IsActive == true), "ID", "ContactPerson");
          
                //ViewBag.ParentCategoryId = new SelectList(db.Categories, "ID", "Name");
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[LogisticController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[LogisticController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        public ActionResult GetReport(string Submit, long? FranchisesID, string FromDate, string ToDate)
        {
            string FrequencyFlag = "";

            DashboardFranchiseViewModel dashboardFranchiseViewModel = new DashboardFranchiseViewModel();
            //List<DashboardLogisticViewModel> dashboardLogisticViewModel = new List<DashboardLogisticViewModel>();

            try
            {
                ViewBag.FranchisesID = new SelectList(db.Franchises.Where(x => x.ID != 1 && x.IsActive == true && x.BusinessDetail.UserLogin.IsLocked == false
                 && x.BusinessDetail.Pincode.City.IsActive == true), "ID", "ContactPerson");
          
                ViewBag.FranchisesSelID = FranchisesID;
                //ViewBag.FromDate = FromDate;
                //ViewBag.ToDate = ToDate;
               

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
                    FrequencyFlag = "";

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
                    if (FromDate != "" && ToDate != "")
                    {
                        //DataSet lLogisticds = new DataSet(); 
                        ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                        string conn = readCon.DB_CONNECTION;
                        SqlConnection con = new SqlConnection(conn);
                        SqlCommand sqlComm = new SqlCommand("ReportDashBoardLogistics", con);
                        sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.Int).Value = FranchisesID;
                        sqlComm.Parameters.AddWithValue("@FrequencyFlag", SqlDbType.Int).Value = FrequencyFlag;
                        sqlComm.Parameters.AddWithValue("@FromDate", SqlDbType.Int).Value = frmd;
                        sqlComm.Parameters.AddWithValue("@ToDate", SqlDbType.Int).Value = tod;
                        sqlComm.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        sqlComm.ExecuteNonQuery();
                        SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                        DataSet lLogisticds = new DataSet();
                        da.Fill(lLogisticds);
                        if (lLogisticds.Tables.Count > 0)
                        {
                            dashboardFranchiseViewModel.TOTALFRANCHISE_NAME = lLogisticds.Tables[0].Rows[0][0].ToString();
                            if (lLogisticds.Tables[0].Rows[0][1].ToString() != string.Empty)
                            {
                                dashboardFranchiseViewModel.TOTALPALCED = Convert.ToInt32(lLogisticds.Tables[0].Rows[0][1].ToString());
                            }
                            else
                            {
                                dashboardFranchiseViewModel.TOTALPALCED = 0;
                            }
                            if (lLogisticds.Tables[0].Rows[0][2].ToString() != string.Empty)
                            {
                                dashboardFranchiseViewModel.TOTALDELIVERED = Convert.ToInt32(lLogisticds.Tables[0].Rows[0][2].ToString());
                            }
                            else
                            {
                                dashboardFranchiseViewModel.TOTALDELIVERED = 0;
                            }

                        }
                        if (lLogisticds.Tables.Count > 1)
                        {
                            dashboardFranchiseViewModel.dashboardLogisticViewModel = BusinessLogicLayer.Helper.CreateListFromTable<DashboardLogisticViewModel>(lLogisticds.Tables[1]);
                        }
                        if(lLogisticds.Tables.Count>2)
                        {
                            dashboardFranchiseViewModel.dashboardFranchisePincodeViewModel = BusinessLogicLayer.Helper.CreateListFromTable<DashboardFranchisePincodeViewModel>(lLogisticds.Tables[2]);
                        }
                        //ViewBag.dashboardLogisticViewModels = dashboardLogisticViewModel;
                        
                    }
                    else
                    {
                        ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                        string conn = readCon.DB_CONNECTION;
                        SqlConnection con = new SqlConnection(conn);
                        SqlCommand sqlComm = new SqlCommand("ReportDashBoardLogistics", con);
                        sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.Int).Value = FranchisesID;
                        sqlComm.Parameters.AddWithValue("@FrequencyFlag", SqlDbType.Int).Value = FrequencyFlag;
                        sqlComm.Parameters.AddWithValue("@FromDate", SqlDbType.Int).Value = null;
                        sqlComm.Parameters.AddWithValue("@ToDate", SqlDbType.Int).Value = null;
                        sqlComm.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        sqlComm.ExecuteNonQuery();
                        SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                        DataSet lLogisticds = new DataSet();
                        da.Fill(lLogisticds);
                        // dashboardFranchiseViewModel = BusinessLogicLayer.Helper.CreateListFromTable<DashboardFranchiseViewModel>(lLogisticds.Tables[0]);
                        if (lLogisticds.Tables.Count>0)
                        {
                            dashboardFranchiseViewModel.TOTALFRANCHISE_NAME = lLogisticds.Tables[0].Rows[0][0].ToString();
                            if (lLogisticds.Tables[0].Rows[0][1].ToString() != string.Empty)
                            {
                                dashboardFranchiseViewModel.TOTALPALCED = Convert.ToInt32(lLogisticds.Tables[0].Rows[0][1].ToString());
                            }
                            else
                            {
                                dashboardFranchiseViewModel.TOTALPALCED = 0;
                            }
                            if (lLogisticds.Tables[0].Rows[0][2].ToString()!=string.Empty)
                            {
                                dashboardFranchiseViewModel.TOTALDELIVERED = Convert.ToInt32(lLogisticds.Tables[0].Rows[0][2].ToString());
                            }
                            else
                            {
                                dashboardFranchiseViewModel.TOTALDELIVERED = 0;
                            }
                            
                        }
                        if (lLogisticds.Tables.Count > 1)
                        {
                            dashboardFranchiseViewModel.dashboardLogisticViewModel = BusinessLogicLayer.Helper.CreateListFromTable<DashboardLogisticViewModel>(lLogisticds.Tables[1]);
                        }
                        if (lLogisticds.Tables.Count > 2)
                        {
                            dashboardFranchiseViewModel.dashboardFranchisePincodeViewModel = BusinessLogicLayer.Helper.CreateListFromTable<DashboardFranchisePincodeViewModel>(lLogisticds.Tables[2]);
                        }
                        //ViewBag.dashboardLogisticViewModels = dashboardLogisticViewModel;
                    }
                                      
                       //DataTable lDataTableCustomerOrder = new DataTable();      
                
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                            
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View("Index", dashboardFranchiseViewModel);
         }
	}
 }
	
