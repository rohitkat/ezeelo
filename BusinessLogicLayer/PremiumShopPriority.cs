using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class PremiumShopPriority : RepositoryPremiumShopPriority, RepositoryPremiumShopPriority_LevelOneCategoryWise, RepositoryPremiumShopPrimaryMarket, RepositoryPremiumShopPriority_LevelTwoCategoryWise
    {

        public bool UpdatePremiumShopPriority(ModelLayer.Models.ViewModel.PremiumShopPriorityList ls, long ShopID, long ModifyBy, string server)
        {
            try
            {
                List<PremiumShopPriorityAttr> lobj = new List<PremiumShopPriorityAttr>();

                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("ShopID");
                dt.Columns.Add("FranchiseID");
                dt.Columns.Add("CategoryID");
                dt.Columns.Add("PriorityLevel");

                lobj = (from n in ls.premiumShopPriorityList
                        select new PremiumShopPriorityAttr
                        {
                            ID = n.ID,
                            ShopID = ShopID,
                            franchiseID= n.FranchiseID,
                            CategoryID = n.CategoryID,
                            SequenceOrder = n.Priority
                        }).ToList();

                foreach (PremiumShopPriorityAttr pa in lobj)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = pa.ID;
                    dr["ShopID"] = pa.ShopID;
                    dr["CategoryID"] = pa.CategoryID;
                    dr["FranchiseID"] = pa.franchiseID;
                    dr["PriorityLevel"] = pa.SequenceOrder;
                    dt.Rows.Add(dr);
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(server))
                    {
                        SqlCommand sqlComm = new SqlCommand("UpdatePremiumShopPriority", conn);
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

        public int Get_MAX_PremiumShopsPriorityOrder(long ShopID, int CategoryID, string server)
        {
            List<object> paramValues = new List<object>();
            paramValues.Add(ShopID);
            paramValues.Add(CategoryID);

            DataTable dt = new DataTable();
            DataAccessLayer.DbOperations op = new DataAccessLayer.GetData(server);
            dt = op.GetRecords("Get_MAX_PremiumShopsPriorityOrder", paramValues);
            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }

        public string Insertupdate_PremiumShopsPriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, string server)
        {
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(server);
            return (ShopMenuPriorityMessageString.GetDisplayMessage(obj.SetRecords("Insertupdate_PremiumShopsPriority", paramValues, opr), opr.ToString(), "ShopMenuPriority"));

        }

        public string Delete_PremiumShopsPriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, out int resultCode, string server)
        {
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(server);
            resultCode = obj.SetRecords("Delete_PremiumShopsPriority", paramValues, opr);
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

            dt = dbOpr.GetRecords("PremiumShopPriorityList_ThirdlevelCategory", paramValues);

            return dt;
        }

        public DataTable SelectShopFrom_PremiumShop(int FranchiseID, Int64 ShopID, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseID);
            paramValues.Add(ShopID);

            dt = dbOpr.GetRecords("SelectShopFrom_PremiumShop", paramValues);

            return dt;
        }

        /* FirstLevel Category Wise Shop Detail in Premium Shop
         * 02-03-2016
         */
        public bool LevelOneWise_UpdatePremiumShopPriority(ModelLayer.Models.ViewModel.PremiumShopPriorityList ls, int FranchiseID, long CategoryID, long ModifyBy, string server)
        {
            try
            {
                List<PremiumShopPriorityAttr> lobj = new List<PremiumShopPriorityAttr>();

                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("ShopID");
                dt.Columns.Add("FranchiseID");
                dt.Columns.Add("CategoryID");
                dt.Columns.Add("PriorityLevel");

                lobj = (from n in ls.premiumShopPriorityList
                        select new PremiumShopPriorityAttr
                        {
                            ID = n.ID,
                            ShopID = n.ShopID,
                            franchiseID = FranchiseID,
                            CategoryID = CategoryID,
                            SequenceOrder = n.Priority
                        }).ToList();

                foreach (PremiumShopPriorityAttr pa in lobj)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = pa.ID;
                    dr["ShopID"] = pa.ShopID;
                    dr["CategoryID"] = pa.CategoryID;
                    dr["FranchiseID"] = pa.franchiseID;
                    dr["PriorityLevel"] = pa.SequenceOrder;
                    dt.Rows.Add(dr);
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(server))
                    {
                        SqlCommand sqlComm = new SqlCommand("LevelOne_UpdatePremiumShopPriority", conn);
                        sqlComm.CommandType = CommandType.StoredProcedure;
                        //sqlComm.Parameters.AddWithValue("@ShopID", SqlDbType.BigInt).Value = ShopID;
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

        public int LevelOneWise_Get_MAX_PremiumShopsPriorityOrder(long CategoryID, int FranchiseID, string server)
        {
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseID);
            paramValues.Add(CategoryID);

            DataTable dt = new DataTable();
            DataAccessLayer.DbOperations op = new DataAccessLayer.GetData(server);
            dt = op.GetRecords("Get_MAX_LevelOnePremiumShopsPriorityOrder", paramValues);
            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }

        public string LevelOneWise_Insertupdate_PremiumShopsPriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, string server)
        {
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(server);
            return (ShopMenuPriorityMessageString.GetDisplayMessage(obj.SetRecords("LevelOneWise_Insertupdate_PremiumShopsPriority", paramValues, opr), opr.ToString(), "ShopMenuPriority"));

        }

        public string LevelOneWise_Delete_PremiumShopsPriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, out int resultCode, string server)
        {
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(server);
            resultCode = obj.SetRecords("LevelOne_Delete_PremiumShopsPriority", paramValues, opr);
            return (ShopMenuPriorityMessageString.GetDisplayMessage(resultCode, opr.ToString(), "ShopMenuPriority"));
        }

        public DataTable LevelOneWise_Call_Select_Procedure(int franchiseID, long CategoryID, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(franchiseID);
            paramValues.Add(CategoryID);

            dt = dbOpr.GetRecords("FranchisePremium_levelCategory", paramValues);

            return dt;
        }

        public DataTable LevelOneWise_SelectFirstLevelCategoryFrom_PremiumShop(Int32 FranchiseID, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseID);

            dt = dbOpr.GetRecords("SelectFirstLevelCategoryFrom_PremiumShop", paramValues);

            return dt;
        }
        public DataTable LevelOneWise_SelectShopFrom_PremiumShop(Int32 FranchiseID, Int64 CategoryID, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseID);
            paramValues.Add(CategoryID);
            dt = dbOpr.GetRecords("ShopByLevelOneCategory", paramValues);

            return dt;
        }

        /* Premium Shop Market Primary Market
         * 07-03-2016
         * Pradnyakar Badge
         */
        public bool UpdatePremiumShopPrimaryMarket(ModelLayer.Models.ViewModel.PremiumShopPriorityList ls,int FranchiseID,  long ModifyBy, string server)
        {
            try
            {
                List<PremiumShopPriorityAttr> lobj = new List<PremiumShopPriorityAttr>();

                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("ShopID");
                dt.Columns.Add("FranchiseID");
                dt.Columns.Add("CategoryID");
                dt.Columns.Add("PriorityLevel");

                lobj = (from n in ls.premiumShopPriorityList
                        select new PremiumShopPriorityAttr
                        {
                            ID = n.ID,
                            ShopID = 0,
                            franchiseID = FranchiseID,
                            CategoryID = n.CategoryID,
                            SequenceOrder = n.Priority
                        }).ToList();

                foreach (PremiumShopPriorityAttr pa in lobj)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = pa.ID;
                    dr["ShopID"] = pa.ShopID;
                    dr["CategoryID"] = pa.CategoryID;
                    dr["FranchiseID"] = pa.franchiseID;
                    dr["PriorityLevel"] = pa.SequenceOrder;
                    dt.Rows.Add(dr);
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(server))
                    {
                        SqlCommand sqlComm = new SqlCommand("Update_PremiumShopPrimaryMarketOrder", conn);
                        sqlComm.CommandType = CommandType.StoredProcedure;                       
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

        public int Get_MAX_PremiumShopPrimaryMarketOrder(int FranchiseID, string server)
        {
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseID);
           

            DataTable dt = new DataTable();
            DataAccessLayer.DbOperations op = new DataAccessLayer.GetData(server);
            dt = op.GetRecords("Get_MAX_PremiumShopPrimaryMarketOrder", paramValues);
            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }

        public string Insertupdate_PremiumShopPrimaryMarket(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, string server)
        {
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(server);
            return (ShopMenuPriorityMessageString.GetDisplayMessage(obj.SetRecords("Insertupdate_PremiumShopPrimaryMarket", paramValues, opr), opr.ToString(), "PremiumShopPrimaryMarket"));
        }

        public string Delete_PremiumShopPrimaryMarket(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, out int resultCode, string server)
        {

            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(server);
            resultCode = obj.SetRecords("Delete_PremiumShopPrimaryMarket", paramValues, opr);
            return (ShopMenuPriorityMessageString.GetDisplayMessage(resultCode, opr.ToString(), "PrimaryShopMarket"));
        }

        public DataTable PremiumShopPrimaryMarket_Call_Select_Procedure(Int32 FranchiseID, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseID);
            paramValues.Add(0);
            dt = dbOpr.GetRecords("SelectPremiumShopPrimaryMarket", paramValues);

            return dt;
        }

        public List<ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel> SelectPremiumShopPrimaryMarket(Int64 ID, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(0);
            paramValues.Add(ID);
            dt = dbOpr.GetRecords("SelectPremiumShopPrimaryMarket", paramValues);

            List<ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel> premiumShopPrimaryMarketViewModel = new List<ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel>();

            premiumShopPrimaryMarketViewModel = (from n in dt.AsEnumerable()
                                                 select new ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel
                                                 {
                                                     ID = n.Field<Int64>("ID"),
                                                     FranchiseID = n.Field<Int32>("FranchiseID"),
                                                     CategoryID = n.Field<Int32>("CategoryID"),
                                                     CategoryName = n.Field<string>("CategoryName"),
                                                     PriorityLevel = n.Field<Int32>("PriorityLevel"),
                                                     IsActive = n.Field<bool>("IsActive")
                                                     //CreateDate = n.Field<DateTime?>("CreateDate"),
                                                     //ModifyDate = n.Field<DateTime?>("ModifyDate"),
                                                     //CreateBy = n.Field<Int32?>("CreatedBy"),
                                                     //ModifyBy = n.Field<Int32?>("ModifyBy"),
                                                     ////NetworkIP = n.Field<string>("NetworkIP"),
                                                     //DeviceType = n.Field<string>("DeviceType"),
                                                     //DeviceID = n.Field<string>("DeviceID"),
                                                     //Level = n.Field<Int32>("Level")
                                                 }).ToList();

            return premiumShopPrimaryMarketViewModel;
        }
    
        /*Pradnyakar Badge
         * Second Level Category For Premium Shop
         */



        public bool LevelTwoWise_UpdatePremiumShopPriority(ModelLayer.Models.ViewModel.PremiumShopPriorityList ls, long ShopID, long ModifyBy, string server)
        {
            try
            {
                List<PremiumShopPriorityAttr> lobj = new List<PremiumShopPriorityAttr>();

                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("ShopID");
                dt.Columns.Add("FranchiseID");
                dt.Columns.Add("CategoryID");
                dt.Columns.Add("PriorityLevel");

                lobj = (from n in ls.premiumShopPriorityList
                        select new PremiumShopPriorityAttr
                        {
                            ID = n.ID,
                            ShopID = ShopID,
                            franchiseID = n.FranchiseID,
                            CategoryID = n.CategoryID,
                            SequenceOrder = n.Priority
                        }).ToList();

                foreach (PremiumShopPriorityAttr pa in lobj)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = pa.ID;
                    dr["ShopID"] = pa.ShopID;
                    dr["CategoryID"] = pa.CategoryID;
                    dr["FranchiseID"] = pa.franchiseID;
                    dr["PriorityLevel"] = pa.SequenceOrder;
                    dt.Rows.Add(dr);
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(server))
                    {
                        SqlCommand sqlComm = new SqlCommand("UpdatePremiumShopPriority_SecondLevel", conn);
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

        public int LevelTwoWise_Get_MAX_PremiumShopsPriorityOrder(int FranchiseID, long ShopID, int Level, string server)
        {
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseID);
            paramValues.Add(ShopID);

            DataTable dt = new DataTable();
            DataAccessLayer.DbOperations op = new DataAccessLayer.GetData(server);
            dt = op.GetRecords("Get_MAX_LevelTwoPremiumShopsPriorityOrder", paramValues);
            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }

        public string LevelTwoWise_Insertupdate_PremiumShopsPriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, string server)
        {
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(server);
            return (ShopMenuPriorityMessageString.GetDisplayMessage(obj.SetRecords("Insertupdate_SecondlevelCategoryShopsPriority", paramValues, opr), opr.ToString(), "ShopMenuPriority"));

        }

        public string LevelTwoWise_Delete_PremiumShopsPriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, out int resultCode, string server)
        {
            DataAccessLayer.DbOperations obj = new DataAccessLayer.SetData(server);
            resultCode = obj.SetRecords("Delete_PremiumShopsPriority_SecondLevelCategory", paramValues, opr);
            return (ShopMenuPriorityMessageString.GetDisplayMessage(resultCode, opr.ToString(), "ShopMenuPriority"));
        }

        public DataTable LevelTwoWise_Call_Select_Procedure(long ShopID, long CategoryID, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(ShopID);
            paramValues.Add(CategoryID);

            dt = dbOpr.GetRecords("PremiumShopPriorityList", paramValues);

            return dt;
        }

        public DataTable LevelTwoWise_SelectShopFrom_PremiumShop(int FranchiseID, long CategoryID, System.Web.HttpServerUtility server)
        {
            throw new NotImplementedException();
        }

        public DataTable LevelTwoWise_SelectFirstLevelCategoryFrom_PremiumShop(int FranchiseID, System.Web.HttpServerUtility server)
        {
            throw new NotImplementedException();
        }

        public DataTable Select_SecondLevelCategoryBySchop(int FranchiseID,long ShopID, System.Web.HttpServerUtility server)
        {
           
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseID);
            paramValues.Add(ShopID);
            dt = dbOpr.GetRecords("Select_SecondLevelCategoryBySchop", paramValues);

            return dt;
        }
    }

    interface RepositoryPremiumShopPriority
    {
        bool UpdatePremiumShopPriority(ModelLayer.Models.ViewModel.PremiumShopPriorityList ls, Int64 ShopID, Int64 ModifyBy, string server);
        Int32 Get_MAX_PremiumShopsPriorityOrder(long ShopID, int Level, string server);
        string Insertupdate_PremiumShopsPriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, string server);
        string Delete_PremiumShopsPriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, out int resultCode, string server);
        DataTable Call_Select_Procedure(Int64 ShopID, Int64? CategoryID, System.Web.HttpServerUtility server);
        DataTable SelectShopFrom_PremiumShop(int FranchiseID, Int64 ShopID, System.Web.HttpServerUtility server);

    }
    interface RepositoryPremiumShopPriority_LevelOneCategoryWise
    {
        bool LevelOneWise_UpdatePremiumShopPriority(ModelLayer.Models.ViewModel.PremiumShopPriorityList ls, int FranchiseID, long CategoryID, long ModifyBy, string server);
        Int32 LevelOneWise_Get_MAX_PremiumShopsPriorityOrder(long ShopID, int Level, string server);
        string LevelOneWise_Insertupdate_PremiumShopsPriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, string server);
        string LevelOneWise_Delete_PremiumShopsPriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, out int resultCode, string server);
        DataTable LevelOneWise_Call_Select_Procedure(int franchiseID, long CategoryID, System.Web.HttpServerUtility server);
        DataTable LevelOneWise_SelectShopFrom_PremiumShop(Int32 FranchiseID, Int64 CategoryID, System.Web.HttpServerUtility server);

        DataTable LevelOneWise_SelectFirstLevelCategoryFrom_PremiumShop(int FranchiseID, System.Web.HttpServerUtility server);

    }

    interface RepositoryPremiumShopPrimaryMarket
    {
        bool UpdatePremiumShopPrimaryMarket(ModelLayer.Models.ViewModel.PremiumShopPriorityList ls, int FranchiseID, Int64 ModifyBy, string server);
        int Get_MAX_PremiumShopPrimaryMarketOrder(int FranchiseID, string server);
        string Insertupdate_PremiumShopPrimaryMarket(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, string server);
        string Delete_PremiumShopPrimaryMarket(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, out int resultCode, string server);
        DataTable PremiumShopPrimaryMarket_Call_Select_Procedure(Int32 FranchiseID, System.Web.HttpServerUtility server);
        List<ModelLayer.Models.ViewModel.PremiumShopPrimaryMarket_ViewModel> SelectPremiumShopPrimaryMarket(Int64 ID, System.Web.HttpServerUtility server);
    }

    interface RepositoryPremiumShopPriority_LevelTwoCategoryWise
    {
        bool LevelTwoWise_UpdatePremiumShopPriority(ModelLayer.Models.ViewModel.PremiumShopPriorityList ls, long ShopID, long ModifyBy, string server);
        Int32 LevelTwoWise_Get_MAX_PremiumShopsPriorityOrder(int FranchiseID, long ShopID, int Level, string server);
        string LevelTwoWise_Insertupdate_PremiumShopsPriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, string server);
        string LevelTwoWise_Delete_PremiumShopsPriority(List<object> paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS opr, out int resultCode, string server);
        DataTable LevelTwoWise_Call_Select_Procedure(long ShopID, long CategoryID, System.Web.HttpServerUtility server);
        DataTable LevelTwoWise_SelectShopFrom_PremiumShop(Int32 FranchiseID, Int64 CategoryID, System.Web.HttpServerUtility server);
        DataTable LevelTwoWise_SelectFirstLevelCategoryFrom_PremiumShop(int FranchiseID, System.Web.HttpServerUtility server);

        DataTable Select_SecondLevelCategoryBySchop(int FranchiseID, long ShopID, System.Web.HttpServerUtility server);

    }
    public class PremiumShopPriorityAttr
    {
        public Int64 ID { get; set; }
        public Int64 ShopID { get; set; }
        public int franchiseID { get; set; }
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

}
