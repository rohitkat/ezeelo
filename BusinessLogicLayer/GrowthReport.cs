using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models;
using System.Data;
using System.Data.Entity.Validation;
using ModelLayer.Models.ViewModel;
using System.Data.SqlClient;
namespace BusinessLogicLayer
{
    public class GrowthReport
    {
        private string serversting { get; set; }
        public GrowthReport(string server)
        {
            this.serversting = server;
         }
        public List<DashboardGrowthViewModel> DashboardGrowth(int? ThisMonth, int? FromMonth, int? ToMonth, int? FranchiseID, int? Year)
        {
            List<DashboardGrowthViewModel> lDashboardGrowthViewModels = new List<DashboardGrowthViewModel>();
            DataTable dt = new DataTable();
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(serversting);
            List<object> paramValues = new List<object>();
            paramValues.Add(ThisMonth);
            paramValues.Add(FromMonth);
            paramValues.Add(ToMonth);            
            paramValues.Add(Year);
            paramValues.Add(FranchiseID);
            dt = dbOpr.GetRecords("ReportDashBoardGrowth", paramValues);

            try
            {
                if (ThisMonth == 1)
                {


                    lDashboardGrowthViewModels = (from n in dt.AsEnumerable()
                                                  select new DashboardGrowthViewModel
                                                  {
                                                      ID = n.Field<int>("ID"),
                                                      Name = n.Field<string>("Name"),
                                                      MonthlyOrderTarget = n.Field<int>("MonthlyOrderTarget"),
                                                      ActualOrder = n.Field<int>("ActualOrder"),
                                                    ShortAccessOrder = n.Field<string>("ShortAccessOrder"),
                                                   MonthlyGMVTarget = n.Field<string>("MonthlyGMVTarget"),
                                                    ActualGMV = n.Field<string>("ActualGMV"),
                                                  ShortAccessGMV = n.Field<string>("ShortAccessGMV")
                                                  }).ToList();
                }
                else   {
                    lDashboardGrowthViewModels = (from n in dt.AsEnumerable()
                                                  select new DashboardGrowthViewModel
                                                  {
                                                      ID = n.Field<int>("ID"),
                                                      Name = n.Field<string>("Name"),
                                                      CumulativeOrderTarget = n.Field<string>("CumulativeOrderTarget"),
                                                      CumulativeActualOrder = n.Field<string>("CumulativeActualOrder"),
                                                      ShortAccessOrder = n.Field<string>("ShortAccessOrder"),
                                                      CumulativeGMVTarget = n.Field<string>("CumulativeGMVTarget"),
                                                      CumulativeActualGMV =n.Field<string>("CumulativeActualGMV"),
                                                      ShortAccessGMV = n.Field<string>("ShortAccessGMV")                                                                                                      
                                                     
                                                  }).ToList();
                }
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => new { x.ErrorMessage, x.PropertyName });

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
             return lDashboardGrowthViewModels;
        }
        public List<DashboardGrowthViewModel> GetIndividualMCO(int? ThisMonth, int? FromMonth, int? ToMonth, int? FranchiseID, int? Year)
        {
            List<DashboardGrowthViewModel> lDashboardGrowthViewModels = new List<DashboardGrowthViewModel>();
            DataTable dt = new DataTable();
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(serversting);
            List<object> paramValues = new List<object>();
            paramValues.Add(ThisMonth);
            paramValues.Add(FromMonth);
            paramValues.Add(ToMonth);
            paramValues.Add(Year);
            paramValues.Add(FranchiseID);
            dt = dbOpr.GetRecords("ReportDashBoardGrowthDetail", paramValues);

            try
            {
                //if (ThisMonth == 1)
                //{


                //    lDashboardGrowthViewModels = (from n in dt.AsEnumerable()
                //                                  select new DashboardGrowthViewModel
                //                                  {
                //                                      ID = n.Field<int>("FranchiseID"),
                //                                      Name = n.Field<string>("Name"),
                //                                      Date = n.Field<string>("Date"),
                //                                      OrderTarget = n.Field<int>("OrderTarget"),
                //                                      ActualOrder = n.Field<int>("ActualOrder"),
                //                                      ShortAccessOrderMCO = n.Field<int>("ShortAccessOrder"),
                //                                      GMVTargetMCO = n.Field<int>("GMVTarget"),
                //                                      ActualGMVMCO = n.Field<int>("ActualGMV"),
                //                                      ShortAccessGMVMCO = n.Field<int>("ShortAccessGMV"),
                //                                      AvgOrderTarget = n.Field<int>("avgOrder"),
                //                                      AvgOrder = n.Field<int>("avgOrderAchived"),
                //                                      AvgGMVTarget = n.Field<int>("avgGMV"),
                //                                      AvgGMV = n.Field<int>("avgGMVAchived")




                //                                  }).ToList();
                //}
                //else
                //{
                    lDashboardGrowthViewModels = (from n in dt.AsEnumerable()
                                                  select new DashboardGrowthViewModel
                                                  {
                                                      ID = n.Field<int>("FranchiseID"),
                                                      Name = n.Field<string>("Name"),
                                                      Date = n.Field<string>("Date"),
                                                      RunningOrderTarget = n.Field<int>("RunningOrderTarget"),
                                                      ActualOrder = n.Field<int>("ActualOrder"),
                                                      RunningActualOrder = n.Field<int>("RunningActualOrder"),
                                                      ShortAccessOrderMCO = n.Field<int>("ShortAccessOrder"),
                                                      RunningGMVTarget = n.Field<int>("RunningGMVTarget"),
                                                      ActualGMVMCO = n.Field<int>("ActualGMV"),
                                                      RunningActualGMV = n.Field<int>("RunningActualGMV"),
                                                      ShortAccessGMVMCO = n.Field<int>("ShortAccessGMV"),
                                                      AvgOrderTarget = n.Field<int>("avgOrder"),
                                                      AvgOrder = n.Field<int>("avgOrderAchived"),
                                                      AvgGMVTarget = n.Field<int>("avgGMV"),
                                                      AvgGMV = n.Field<int>("avgGMVAchived")

                                                  }).ToList();
               // }
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => new { x.ErrorMessage, x.PropertyName });

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            return lDashboardGrowthViewModels;
        }
        public DataSet GetMCOWiseRetenion(int? FranchiseID, string Mode, System.Web.HttpServerUtility server)
        {
            DataSet ds = new DataSet();
            try
            {
                ReadConfig config = new ReadConfig(server);
                string query = "ReportDashBoardCustomerType";
                SqlCommand cmd = new SqlCommand(query);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@CityID", FranchiseID);////hide
                cmd.Parameters.AddWithValue("@FranchiseID", FranchiseID);////added
                cmd.Parameters.AddWithValue("@Mode", Mode);

                using (SqlConnection con = new SqlConnection(config.DB_CONNECTION))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(ds);
                    }
                }
               
            }
            catch (Exception ex)
            {
                throw;
            }
            return ds;
        }
    }
}
