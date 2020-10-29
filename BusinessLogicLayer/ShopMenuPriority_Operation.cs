using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class ShopMenuPriority_Operation : RepositoryShopMenuPriority
    {
        public bool ShopMenuListUpdate(ModelLayer.Models.ViewModel.ShopMenuPriorityList ls, long ShopID, long ModifyBy, string server)
        {
            try
            {
                List<ShopMenuPriorityAttr> lobj = new List<ShopMenuPriorityAttr>();

                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("ShopID");
                dt.Columns.Add("CategoryID");
                dt.Columns.Add("SequenceOrder");

                lobj = (from n in ls.shopMenuPriorityList
                        select new ShopMenuPriorityAttr
                        {
                            ID = n.ID,
                            ShopID = ShopID,
                            CategoryID = n.CategoryID,
                            SequenceOrder = n.SequenceOrder
                        }).ToList();

                foreach (ShopMenuPriorityAttr pa in lobj)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = pa.ID;
                    dr["ShopID"] = pa.ShopID;
                    dr["CategoryID"] = pa.CategoryID;
                    dr["SequenceOrder"] = pa.SequenceOrder;
                    dt.Rows.Add(dr);
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(server))
                    {
                        SqlCommand sqlComm = new SqlCommand("UpdateShopMenuPriority", conn);
                        sqlComm.CommandType = CommandType.StoredProcedure;
                        sqlComm.Parameters.AddWithValue("@ShopID", SqlDbType.BigInt).Value = ShopID;
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

        public int Get_MAX_ShopMenuPrioritySequenceOrder(long ShopID, int Level, string server)
        {
            List<object> paramValues = new List<object>();
            paramValues.Add(ShopID);
            paramValues.Add(Level);

            DataTable dt = new DataTable();
            DataAccessLayer.DbOperations op = new DataAccessLayer.GetData(server);
            dt = op.GetRecords("Get_MAX_ShoMenuPrioritySequenceOrder", paramValues);
            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }

        public string Insertupdate_ShopMenuPriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, string server)
        {
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(server);
            return (ShopMenuPriorityMessageString.GetDisplayMessage(obj.SetRecords("Insertupdate_ShopMenuPriority", paramValues, opr), opr.ToString(), "ShopMenuPriority"));

        }

        public string Delete_ShopMenuPriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, out int resultCode, string server)
        {
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(server);
            resultCode = obj.SetRecords("Delete_ShopMenuPriority", paramValues, opr);
            return (ShopMenuPriorityMessageString.GetDisplayMessage(resultCode, opr.ToString(), "ShopMenuPriority"));
        }

        public DataTable Call_Select_Procedure(Int64 ShopID, Int64? CategoryID, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(ShopID);
            paramValues.Add(CategoryID);

            dt = dbOpr.GetRecords("ShopMenuPriorityList", paramValues);

            return dt;
        }

        public DataTable SelectShopFrom_ShopMenuPriority(int FranchiseID, Int64 ShopID, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseID);
            paramValues.Add(ShopID);

            dt = dbOpr.GetRecords("SelectShopFrom_ShopMenuPriority", paramValues);

            return dt;
        }


      
    }
}

interface RepositoryShopMenuPriority
{
    bool ShopMenuListUpdate(ModelLayer.Models.ViewModel.ShopMenuPriorityList ls, Int64 FranchiseID, Int64 ModifyBy, string server);
    Int32 Get_MAX_ShopMenuPrioritySequenceOrder(long ShopID, int Level, string server);
    string Insertupdate_ShopMenuPriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, string server);
    string Delete_ShopMenuPriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, out int resultCode, string server);
    DataTable Call_Select_Procedure(Int64 ShopID, Int64? CategoryID, System.Web.HttpServerUtility server);
    DataTable SelectShopFrom_ShopMenuPriority(int FranchiseID, Int64 ShopID, System.Web.HttpServerUtility server);

}

 public class ShopMenuPriorityAttr
    {
        public Int64 ID { get; set; }
        public Int64 ShopID { get; set; }
        public Int64 CategoryID { get; set; }
        public Int64 SequenceOrder { get; set; }
       
    }

    public class ShopMenuPriorityMessageString
    {
        public static string GetDisplayMessage(int QryResult, string opr, string value)
        {
            switch (QryResult)
            {
                case 0: return "No Operation Performed on \" " + value + "\"";
                case 1: return "\"" + value + "\" " + opr.ToString().ToUpper() + "ED Successfully!!";
                case 2: return "\"" + value + "\" " + opr.ToString().ToUpper() + "D Successfully!!";
                case 3: return "\"" + value + "\" " + opr.ToString().ToUpper() + "ED Successfully!!";
                case 4: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid Priority Level..!!";
                case 10: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid Franchise..!!";
                case 11: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid Category..!!";
                case 12: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid Shop..!!";
                case 13: return "The Record to perform " + opr.ToString().ToUpper() + " IsActive Can't null..!!";
                case 14: return "The Record to perform " + opr.ToString().ToUpper() + " Sequence Order Can't null..!!";
                case 15: return "The Record to perform " + opr.ToString().ToUpper() + " Is already Exists..!!";
                case 16: return "The Record to perform " + opr.ToString().ToUpper() + " Record Not Found..!!";
                default: return "No Operation Performed Perform";
            }
        }
    }