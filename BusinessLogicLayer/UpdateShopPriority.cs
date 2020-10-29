using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace BusinessLogicLayer
{
    /// <summary>
    /// Update Shop Priority in Bulk
    /// </summary>
    public class UpdateShopPriority
    {   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="CategoryID"></param>
        /// <param name="ModifyBy"></param>
        public bool ShopPriorityListUpdate(ModelLayer.Models.ViewModel.ShopPriorityList ls, Int64 CategoryID, Int64 ModifyBy, string server)
        {
            try
            {

                List<PriorityAttr> lobj = new List<PriorityAttr>();

                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("ShopID");
                dt.Columns.Add("CityID");
                dt.Columns.Add("CategoryID");
                dt.Columns.Add("PriorityLevel");
                dt.Columns.Add("FranchiseID");////added


                lobj = (from n in ls.shopListByPriority
                        select new PriorityAttr
                        {
                            ID = n.Id,
                            ShopID = n.ShopID,
                            CategoryID = CategoryID,
                            CityID = n.CityID,
                            PriorityLevel = n.Priority,
                            FranchiseID = n.FranchiseID ////added
                        }).ToList();

                foreach (PriorityAttr pa in lobj)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = pa.ID;
                    dr["ShopID"] = pa.ShopID;
                    dr["CityID"] = pa.CityID;
                    dr["CategoryID"] = pa.CategoryID;
                    dr["PriorityLevel"] = pa.PriorityLevel;
                    dr["FranchiseID"] = pa.FranchiseID; ////added
                    dt.Rows.Add(dr);
                }




                try
                {
                    using (SqlConnection conn = new SqlConnection(server))
                    {

                        SqlCommand sqlComm = new SqlCommand("UpdateShopPriority", conn);
                        sqlComm.CommandType = CommandType.StoredProcedure;
                        sqlComm.Parameters.AddWithValue("@CategoryID", SqlDbType.BigInt).Value = CategoryID;
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
            catch {
                return false;
            }



        }

        public Int32 Get_MAX_ShopPriority(Int32 FranchiseID, Int64 CategoryID, string server)////added Int64 CityId-> Int32 FranchiseID
        {
            //using (SqlConnection conn = new SqlConnection(server))
            //{
            //    DataSet ds = new DataSet();
            //    SqlDataAdapter da = new SqlDataAdapter("Get_MAX_ShopPriority", conn);
            //    conn.Open();
            //    da.SelectCommand.Parameters.AddWithValue("@cityID", SqlDbType.BigInt).Value = CityId;
            //    da.SelectCommand.Parameters.AddWithValue("@CategoryID", SqlDbType.BigInt).Value = CategoryID;
            //    da.Fill(ds);
            //    conn.Close();

            //   return Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());

            //}
            List<object> paramValues= new List<object>();
            paramValues.Add(FranchiseID); ////added CityId->
            paramValues.Add(CategoryID);

            DataTable dt = new DataTable();
            DataAccessLayer.DbOperations op = new DataAccessLayer.GetData(server);
            dt = op.GetRecords("Get_MAX_ShopPriority", paramValues);
            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }

        public static string Insertupdate_shoppriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, string server)
        {
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(server);
            return (MessageString.GetDisplayMessage(obj.SetRecords("Insertupdate_shoppriority", paramValues, opr), opr.ToString(), "ShopPriority"));
        }

        public static string Delete_Shoppriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, out int resultCode ,string server)
        {
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(server);
            resultCode = obj.SetRecords("Delete_Shoppriority", paramValues, opr);
            return (MessageString.GetDisplayMessage(resultCode, opr.ToString(), "ShopPriority"));
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class PriorityAttr
    { 
        public Int64 ID { get; set; }
        public Int64 ShopID { get; set; }
        public Int64 CityID { get; set; }
        public Int64 CategoryID { get; set; }
        public Int64 PriorityLevel {get; set;}
        public int FranchiseID { get; set; }////added

    }

    public class MessageString
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
                case 10: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid City..!!";
                case 11: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid Category..!!";
                case 12: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid Shop..!!";
                case 13: return "The Record to perform " + opr.ToString().ToUpper() + " IsActive Can't null..!!";
                case 14: return "The Record to perform " + opr.ToString().ToUpper() + " PriorityLevel Can't null..!!";
                case 15: return "The Record to perform " + opr.ToString().ToUpper() + " Is already Exists..!!";
                case 16: return "The Record to perform " + opr.ToString().ToUpper() + " Record Not Found..!!";
                default: return "No Operation Performed Perform";
            }
        }
    }

}
