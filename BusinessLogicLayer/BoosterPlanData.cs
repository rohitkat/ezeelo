using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class BoosterPlanData
    {
        public DataSet GetData(string SPName, List<SqlParameter> parameter)
        {
            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            string conn = readCon.DB_CONNECTION;
            SqlConnection con = new SqlConnection(conn);
            SqlCommand sqlComm = new SqlCommand(SPName, con);
            sqlComm.CommandTimeout = 1000;
            sqlComm.Parameters.AddRange(parameter.ToArray());
            sqlComm.CommandType = CommandType.StoredProcedure;
            con.Open();

            SqlDataAdapter da = new SqlDataAdapter(sqlComm);
            DataSet ds = new DataSet();
            da.Fill(ds);
            con.Close();
            return ds;
        }

        public List<BBPWeeklyEarning> GetData_Earning(long UserLoginId)
        {
            DataSet ds = new DataSet();
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@UserLoginId", SqlDbType = SqlDbType.BigInt, Value= UserLoginId},
            };
            ds = GetData("BoosterPlanWeeklyEarning", sp);
            List<BBPWeeklyEarning> earnings = new List<BBPWeeklyEarning>();
            if (ds.Tables.Count > 0)
            {
                earnings = BusinessLogicLayer.Helper.CreateListFromTable<BBPWeeklyEarning>(ds.Tables[0]);
            }
            return earnings;
        }

        public List<BBPTabularview> GetData_Tabular(long UserLoginId,int ?PayoutId)
        {
            DataSet ds = new DataSet();
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@UserLoginId", SqlDbType = SqlDbType.BigInt, Value= UserLoginId},
                new SqlParameter() {ParameterName = "@BoosterPayoutId", SqlDbType = SqlDbType.BigInt, Value= PayoutId},
            };
            ds = GetData("BoosterPlanTabularView", sp);

            List<BBPTabularview> Tabluar = new List<BBPTabularview>();
            if (ds.Tables.Count > 0)
            {
                Tabluar = BusinessLogicLayer.Helper.CreateListFromTable<BBPTabularview>(ds.Tables[0]);
            }
            return Tabluar;
        }

        public BBPNetworkViewVeiwModel GetData_Network(long BoosterPlanPayoutId, long UserLoginId, int flag, int level)
        {
            BBPNetworkViewVeiwModel obj = new BBPNetworkViewVeiwModel();
            DataSet ds = new DataSet();
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@BoosterPlanPayoutId", SqlDbType = SqlDbType.BigInt, Value= BoosterPlanPayoutId},
                new SqlParameter() {ParameterName = "@UserLoginId", SqlDbType = SqlDbType.BigInt, Value = UserLoginId},
                new SqlParameter() {ParameterName = "@Flag", SqlDbType = SqlDbType.Int, Value = flag},
                new SqlParameter() {ParameterName = "@Level", SqlDbType = SqlDbType.Int, Value = level}
            };

            ds = GetData("BoosterPlanNetworkView", sp);
            List<BBPNetworkViewMain> main = new List<BBPNetworkViewMain>();
            List<BBPNetworkView> details = new List<BBPNetworkView>();
            if (ds.Tables.Count > 0)
            {
                if (flag == 0)
                {
                    main = BusinessLogicLayer.Helper.CreateListFromTable<BBPNetworkViewMain>(ds.Tables[0]);
                }
                else
                {
                    details = BusinessLogicLayer.Helper.CreateListFromTable<BBPNetworkView>(ds.Tables[0]);
                }
            }
            obj.main = main;
            obj.details = details;
            return obj;
        }

        public BBPUserDashboard GetData_Dashboard(long UserLoginId)
        {
            DateTime baseDate = DateTime.Today;
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            DateTime thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
            
            DataSet ds = new DataSet();
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@UserLoginId", SqlDbType = SqlDbType.BigInt, Value= UserLoginId},
                new SqlParameter() {ParameterName = "@PayoutToDate", SqlDbType = SqlDbType.Date, Value= thisWeekEnd}
            };
            ds = GetData("BoosterPlanUserDashboard", sp);

            List<BBPUserDashboard> dash = new List<BBPUserDashboard>();
            if (ds.Tables.Count > 0)
            {
                dash = BusinessLogicLayer.Helper.CreateListFromTable<BBPUserDashboard>(ds.Tables[0]);
            }
            return dash.FirstOrDefault();
        }

        public BBPUserStatusData GetData_UserStatusReport(long UserLoginId)
        {
            DateTime baseDate = DateTime.Today;
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            DateTime thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@UserLoginID", SqlDbType = SqlDbType.BigInt, Value= UserLoginId},
                new SqlParameter() {ParameterName = "@PayoutToDate", SqlDbType = SqlDbType.DateTime, Value= thisWeekEnd}
            };
            DataSet ds = new DataSet();
            ds = GetData("BoosterUserStausDescriber", sp);
            BBPUserStatusData data = new BBPUserStatusData();
            List<BBPUserStatusReport> report = new List<BBPUserStatusReport>();
            List<BBPUserStatusResult> result = new List<BBPUserStatusResult>();
            List<BBPUserStatusOrdReport> order = new List<BBPUserStatusOrdReport>();
            if (ds.Tables.Count > 0)
            {
                report = BusinessLogicLayer.Helper.CreateListFromTable<BBPUserStatusReport>(ds.Tables[0]);
            }
            if (ds.Tables.Count > 1)
            {
                result = BusinessLogicLayer.Helper.CreateListFromTable<BBPUserStatusResult>(ds.Tables[1]);
            }
            if (ds.Tables.Count > 2)
            {
                order = BusinessLogicLayer.Helper.CreateListFromTable<BBPUserStatusOrdReport>(ds.Tables[2]);
            }
            data.order = order;
            data.result = result;
            data.report = report;
            return data;
        }
    }


}
