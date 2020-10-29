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
    public class UpdateFranchiseCategorySequence
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="CategoryID"></param>
        /// <param name="ModifyBy"></param>
        public bool FranchiseMenuListUpdate(ModelLayer.Models.ViewModel.FranchiseMenuList ls, Int64 FranchiseID, Int64 ModifyBy, string server)
        {
            try
            {
                List<FMenuAttr> lobj = new List<FMenuAttr>();

                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("FranchiseID");
                dt.Columns.Add("CategoryID");
                dt.Columns.Add("SequenceOrder");

                lobj = (from n in ls.franchiseMenuList
                        select new FMenuAttr
                        {
                            ID = n.ID,
                            FranchiseID = FranchiseID,
                            CategoryID = n.CategoryID,
                            SequenceOrder = n.SequenceOrder
                        }).ToList();

                foreach (FMenuAttr pa in lobj)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = pa.ID;
                    dr["FranchiseID"] = pa.FranchiseID;
                    dr["CategoryID"] = pa.CategoryID;
                    dr["SequenceOrder"] = pa.SequenceOrder;
                    dt.Rows.Add(dr);
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(server))
                    {
                        SqlCommand sqlComm = new SqlCommand("UpdateFranchiseMenuPriority", conn);
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

        public Int32 Get_MAX_FMSequenceOrder(int FranchiseID, int? parentCategoryID, int Level, string server)
        {

            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseID);
            paramValues.Add(parentCategoryID);
            paramValues.Add(Level);

            DataTable dt = new DataTable();
            DataAccessLayer.DbOperations op = new DataAccessLayer.GetData(server);
            dt = op.GetRecords("Get_MAX_FMSequenceOrder", paramValues);
            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }

        public static string Insertupdate_FranchiseMenu(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, string server)
        {
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(server);
            return (FMenuMessageString.GetDisplayMessage(obj.SetRecords("Insertupdate_FranchiseMenu", paramValues, opr), opr.ToString(), "FranchiseMenu"));
        }

        public static string Delete_FranchiseMenu(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, out int resultCode, string server)
        {
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(server);
            resultCode = obj.SetRecords("Delete_FranchiseMenu", paramValues, opr);
            return (FMenuMessageString.GetDisplayMessage(resultCode, opr.ToString(), "FranchiseMenu"));
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class FMenuAttr
    {
        public Int64 ID { get; set; }
        public Int64 FranchiseID { get; set; }
        public Int64 CategoryID { get; set; }
        public Int64 SequenceOrder { get; set; }

    }

    public class FMenuMessageString
    {
        public static string GetDisplayMessage(int QryResult, string opr, string value)
        {
            switch (QryResult)
            {
                case 0: return "No Operation Performed on \" " + value + "\"";
                case 1: return "\"" + value + "\" " + opr.ToString().ToUpper() + "ED Successfully!!";
                case 2: return "\"" + value + "\" " + opr.ToString().ToUpper() + "D Successfully!!";
                case 3: return "\"" + value + "\" " + opr.ToString().ToUpper() + "ED Successfully!!";
                //case 4: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid Sequence Order..!!";
                case 4: return "Please Enter Valid Sequence Order..!!";
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
}
