using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class HomeProductGallery
    {
        public static void InsertGalleryProducts(List<object> paramValues, System.Web.HttpServerUtility server)
        {
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.Enumerators.DB_OPERATIONS opr = DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT;
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.SetData(config.DB_CONNECTION);
            dbOpr.SetRecords("InsertUpdate_GalleryProducts", paramValues, opr);
           // return (GetDisplayMessage(resultCode, opr.ToString(), "BlockItem"));
        }

        public static void Delete_GalleryProduct(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, System.Web.HttpServerUtility server)
        {    
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(config.DB_CONNECTION);
            obj.SetRecords("Delete_GalleryProduct", paramValues, opr);
            //string msg = FMenuMessageString.GetDisplayMessage(resultCode, opr.ToString(), "BlockItem");
            //return msg;
        }

        public static DataTable Select_GalleryProducts(Int64 FranchiseID, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseID);

            dt = dbOpr.GetRecords("Select_HomeProductGallery", paramValues);

            return dt;
        }

        public bool ProductGallerySequenceUpdate(ModelLayer.Models.ViewModel.ProductGalleryList ls, Int64 FranchiseID,Int64 BlockTypeID, Int64 ModifyBy, System.Web.HttpServerUtility server)
        {
            try
            {
                ReadConfig config = new ReadConfig(server);
                List<DCPAttr> lobj = new List<DCPAttr>();

                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("FranchiseID");
                dt.Columns.Add("ProductID");
                dt.Columns.Add("SequenceOrder");

                lobj = (from n in ls.productList
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
                    using (SqlConnection conn = new SqlConnection(config.DB_CONNECTION))
                    {
                        SqlCommand sqlComm = new SqlCommand("UpdateProductGallerySequence", conn);
                        sqlComm.CommandType = CommandType.StoredProcedure;
                        sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.BigInt).Value = FranchiseID;
                        sqlComm.Parameters.AddWithValue("@DesignBlockTypeID", SqlDbType.BigInt).Value = BlockTypeID;
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
    }
}
