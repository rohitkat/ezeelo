using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class HomePageBlockItemsList
    {

        public DataTable Select_BlockItemsList(Int64 FranchiseID, Int64 BlockTypeID, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseID);
            paramValues.Add(BlockTypeID);

            dt = dbOpr.GetRecords("Select_BlockItemsList", paramValues);

            return dt;
        }

        public static string Insertupdate_BlockItemsList(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, out int resultCode, System.Web.HttpServerUtility server)
        {
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.SetData(config.DB_CONNECTION);
            resultCode = dbOpr.SetRecords("Insert_Update_BlockItemsList", paramValues, opr);
            return (GetDisplayMessage(resultCode, opr.ToString(), "BlockItem"));
        }

        public string UpdateSeuence_BlockItemsList(ModelLayer.Models.ViewModel.HomePageBlockItemsList bils, Int32 FranchiseID, Int64 BlockTypeID, Int64 ModifyBy, System.Web.HttpServerUtility server)
        {
            try
            {
                List<BlockItemsAttr> lobj = new List<BlockItemsAttr>();

                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("FranchiseID");
                dt.Columns.Add("DesignBlockTypeID");
                dt.Columns.Add("SequenceOrder");

                lobj = (from n in bils.blockItemsList
                        select new BlockItemsAttr
                        {
                            ID = n.ID,
                            FranchiseID = FranchiseID,
                            DesignBlockTypeID = BlockTypeID,
                            SequenceOrder = n.SequenceOrder
                        }).ToList();

                foreach (BlockItemsAttr pa in lobj)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = pa.ID;
                    dr["FranchiseID"] = pa.FranchiseID;
                    dr["DesignBlockTypeID"] = pa.DesignBlockTypeID;
                    dr["SequenceOrder"] = pa.SequenceOrder;
                    dt.Rows.Add(dr);
                }

                try
                {
                    ReadConfig config = new ReadConfig(server);
                    DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.SetData(config.DB_CONNECTION);
                    List<object> paramValues = new List<object>();
                    paramValues.Add(ModifyBy);
                    paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                    paramValues.Add(dt);
                    dbOpr.SetRecords("UpdateBlockItemsListSequence", paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE);
                    return "Sequence Order Set Successfully";
                }
                catch (Exception ex)
                {
                    //pServerMsg += "\nError : " + (int)IsoftConstant.IS_ERROR_TYPE.EXCEPTION + " : " + ex.Message;
                    return "Sorry Unable to set Sequence ........";
                }
            }
            catch
            {
                return "Sorry Unable to set Sequence ........";
            }
        }

        public static string Delete_BlockItem(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, out int resultCode, System.Web.HttpServerUtility server)
        {
            EzeeloDBContext db = new EzeeloDBContext();
            string imgPath = db.BlockItemsLists.Find(Convert.ToInt64(paramValues[0])).ImageName;

            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(config.DB_CONNECTION);
            resultCode = obj.SetRecords("Delete_BlockItemsList", paramValues, opr);
            string msg = FMenuMessageString.GetDisplayMessage(resultCode, opr.ToString(), "BlockItem");

            if (resultCode == 3)
            {
                try
                {
                    FtpImageUpload fiup = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                    fiup.DeleteImage(config.HOME_IMAGE_FTP + imgPath);
                }
                catch
                {
                    msg = msg + " Problem in Deleting Block Item Image...";
                }
            }

            return msg;
        }
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
                case 11: return "The Record to perform " + opr.ToString().ToUpper() + " on Invalid Block Type..!!";
                case 12: return "The Record to perform " + opr.ToString().ToUpper() + " IsActive Can't null..!!";
                case 13: return "The Record to perform " + opr.ToString().ToUpper() + " Sequence Order Can't null..!!";
                case 15: return "The Record to perform " + opr.ToString().ToUpper() + " Is already Exists..!!";
                case 16: return "The Record to perform " + opr.ToString().ToUpper() + " Record Not Found..!!";
                default: return "No Operation Performed Perform";
            }
        }


        /* select block type in customer mod.*/
        public static DataSet GetHomeIndexItemList(Int64 franchiseID, System.Web.HttpServerUtility server)////added Int64 cityID-> Int64 frnachiseID
        {
            DataSet ds = new DataSet();
            try
            {
                ReadConfig config = new ReadConfig(server);
                string query = "Select_HomePageBlockItemList";
                SqlCommand cmd = new SqlCommand(query);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@CityID", cityID);////hide
                cmd.Parameters.AddWithValue("@FranchiseID", franchiseID);////added

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
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[HomePageBlockItemsList][GetHomeIndexItemList]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return ds;
        }

        /// <summary>
        /// Added by Yashaswi 20-2-2019
        /// Execute stored procedure 'SP_HomePageDynamicSections' and return HomePageDynamicSection
        /// Stored procedure return dataset contain 3 Table
        /// 1. HomePageDynamicSection
        /// 2. HomePageDynamicSectionBanners
        /// 3. HomePageDynamicSectionProduct
        /// </summary>
        /// <param name="franchiseID"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public static DataSet GetHomeDynamicSections(Int64 franchiseID, System.Web.HttpServerUtility server)
        {
            DataSet ds = new DataSet();
            try
            {
                ReadConfig config = new ReadConfig(server);
                string query = "SP_HomePageDynamicSections";
                SqlCommand cmd = new SqlCommand(query);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FranchiseID", franchiseID);

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
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[HomePageBlockItemsList][GetHomeDynamicSections]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return ds;
        }
    }
    public class BlockItemsAttr
    {
        public Int64 ID { get; set; }
        public Int32 FranchiseID { get; set; }
        public Int64 DesignBlockTypeID { get; set; }
        public Int64 SequenceOrder { get; set; }

    }
}
