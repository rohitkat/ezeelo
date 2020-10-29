using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class DynamicProductList
    {
        public DataSet Select_DynamicProducts(int FranchiseID, int CatID, System.Web.HttpServerUtility server)////added Int64 CityID->int FranchiseID
        {
            DataSet ds = new DataSet();
            try
            {
                ReadConfig config = new ReadConfig(server);
                string query = "Select_DynamicCategoryProducts";
                SqlCommand cmd = new SqlCommand(query);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@CityID", FranchiseID);////hide
                cmd.Parameters.AddWithValue("@FranchiseID", FranchiseID);////added
                cmd.Parameters.AddWithValue("@CategoryID", CatID);

                using (SqlConnection con = new SqlConnection(config.DB_CONNECTION))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(ds);
                    }
                }

                //DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
                //List<object> paramValues = new List<object>();
                //paramValues.Add(CityID);
                //paramValues.Add(CatID);
                //ds = dbOpr.GetRecordsDataSet("Select_DynamicCategoryProducts", paramValues);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DynamicProductList][Select_DynamicCategoryProducts]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return ds;
        }

        public DataTable Call_Select_Procedure(Int64 FranchiseID, Int64 CategoryID, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseID);
            paramValues.Add(CategoryID);

            dt = dbOpr.GetRecords("Select_DynamicProductsSequence", paramValues);

            return dt;
        }

        public bool DynamicProductSequenceUpdate(ModelLayer.Models.ViewModel.DynamicCategoryProductList ls, Int64 FranchiseID, Int64 ModifyBy, string server)
        {
            try
            {
                List<DCPAttr> lobj = new List<DCPAttr>();

                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("FranchiseID");
                dt.Columns.Add("ProductID");
                dt.Columns.Add("SequenceOrder");

                lobj = (from n in ls.categoryProductList
                        select new DCPAttr
                        {
                            ID = n.ID,
                            FranchiseID = FranchiseID,
                            ProductID = n.ProductID,
                            SequenceOrder = n.SequenceOrder
                        }).ToList();

                foreach (DCPAttr pa in lobj)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = pa.ID;
                    dr["FranchiseID"] = pa.FranchiseID;
                    dr["ProductID"] = pa.ProductID;
                    dr["SequenceOrder"] = pa.SequenceOrder;
                    dt.Rows.Add(dr);
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(server))
                    {
                        SqlCommand sqlComm = new SqlCommand("UpdateDynamicProductSequence", conn);
                        sqlComm.CommandType = CommandType.StoredProcedure;
                        sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.BigInt).Value = FranchiseID;
                        sqlComm.Parameters.AddWithValue("@ModifyBy", SqlDbType.BigInt).Value = ModifyBy;
                        sqlComm.Parameters.AddWithValue("@ModifyDate", SqlDbType.DateTime2).Value = DateTime.UtcNow.AddHours(5.5);
                        sqlComm.Parameters.AddWithValue("@PriorityList", SqlDbType.Structured).Value = dt;
                        conn.Open();
                        sqlComm.ExecuteNonQuery();
                        conn.Close();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    //pServerMsg += "\nError : " + (int)IsoftConstant.IS_ERROR_TYPE.EXCEPTION + " : " + ex.Message;
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static string Delete_DynamicProducts(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, out int resultCode, string server)
        {
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(server);
            resultCode = obj.SetRecords("Delete_DynamicCategoryProduct", paramValues, opr);
            return ("Product Deleted Successfully.");
        }
    }

    public class DCPAttr
    {
        public Int64 ID { get; set; }
        public Int64 FranchiseID { get; set; }
        public Int64 ProductID { get; set; }
        public Int64 SequenceOrder { get; set; }

    }
}
