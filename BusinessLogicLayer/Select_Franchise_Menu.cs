using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class Select_Franchise_Menu
    {
        /// <summary>
        /// Server Name property
        /// </summary>
        protected System.Web.HttpServerUtility server;
        /// <summary>
        /// Connection Sever
        /// </summary>
        /// <param name="serverCon"></param>
        public Select_Franchise_Menu(System.Web.HttpServerUtility serverCon)
        {
            this.server = serverCon;
        }

        /// <summary>
        /// Franchise Menu List
        /// </summary>
        /// <param name="CityID">CityID</param>
        /// <returns>List Of Categorty Menu</returns>
        public List<FranchiseMenuListViewModel> selectFrnchiseMenu(Int64 CityID, Int32? FranchiseID=null)////added params Int32 FranchiseID for Multiple MCO in same city
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(CityID);
            paramValues.Add(FranchiseID);////added FranchiseID
            paramValues.Add(config.CATEGORY_IMAGE_HTTP);
            dt = dbOpr.GetRecords("Select_Franchise_Menu", paramValues);

            List<FranchiseMenuListViewModel> obj = new List<FranchiseMenuListViewModel>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                FranchiseMenuListViewModel dd = new FranchiseMenuListViewModel();
                dd.LevelOneID = Convert.ToInt64(dt.Rows[i]["LevelOneID"].ToString());
                dd.LevelTwoID = Convert.ToInt64(dt.Rows[i]["LevelTwoID"].ToString());
                dd.LevelThreeID = Convert.ToInt64(dt.Rows[i]["LevelThreeID"].ToString());
                dd.LevelOneName = dt.Rows[i]["LevelOneName"].ToString();
                dd.LevelTwoName = dt.Rows[i]["LevelTwoName"].ToString();
                dd.LevelThreeName = dt.Rows[i]["LevelThreeName"].ToString();
                dd.ImagePath = dt.Rows[i]["ImagePath"].ToString();
                dd.ImagePath = dt.Rows[i]["Thumb_ImagePath"].ToString();
                //dd.LevelThreeIsActive = Convert.ToBoolean(dt.Rows[i]["LevelThreeIsActive"].ToString());

                obj.Add(dd);
            }

                return obj;
        }

        /// <summary>
        /// Franchise Menu -- first level category wise filter
        /// </summary>
        /// <param name="CityID">CityIID</param>
        /// <param name="CategoryID">Category ID</param>
        /// <returns>List of Category Menu</returns>
        public List<FranchiseMenuListViewModel> selectFrnchiseMenu(Int64 CityID, Int64 CategoryID, int? FranchiseID=null)////added params Int32 FranchiseID for Multiple MCO in same city
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(CityID);
            paramValues.Add(FranchiseID);//////added 

            paramValues.Add(CategoryID);
            paramValues.Add(config.CATEGORY_IMAGE_HTTP);
            dt = dbOpr.GetRecords("Select_Franchise_Menu_ByCategory", paramValues);

            List<FranchiseMenuListViewModel> obj = new List<FranchiseMenuListViewModel>();
            if (CategoryID == 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    FranchiseMenuListViewModel dd = new FranchiseMenuListViewModel();
                    dd.LevelOneID = Convert.ToInt64(dt.Rows[i]["LevelOneID"].ToString());
                    dd.LevelTwoID = 0;
                    dd.LevelThreeID = 0;
                    dd.LevelOneName = dt.Rows[i]["LevelOneName"].ToString();
                    dd.LevelTwoName = string.Empty;
                    dd.LevelThreeName = string.Empty;
                    dd.ImagePath = string.Empty;
                    dd.Thumb_ImagePath = dt.Rows[i]["Thumb_ImagePath"].ToString();
                    dd.LevelOneSequenceOrder = Convert.ToInt32(dt.Rows[i]["LevelOneSequenceOrder"].ToString());
                    dd.LevelTwoSequenceOrder = Convert.ToInt32(dt.Rows[i]["LevelTwoSequenceOrder"].ToString());
                    dd.LevelthreeSequenceOrder = Convert.ToInt32(dt.Rows[i]["LevelthreeSequenceOrder"].ToString());
                    //dd.LevelThreeIsActive = Convert.ToBoolean(dt.Rows[i]["LevelThreeIsActive"].ToString());
                    obj.Add(dd);
                }
            }
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    FranchiseMenuListViewModel dd = new FranchiseMenuListViewModel();
                    dd.LevelOneID = Convert.ToInt64(dt.Rows[i]["LevelOneID"].ToString());
                    dd.LevelTwoID = Convert.ToInt64(dt.Rows[i]["LevelTwoID"].ToString());
                    dd.LevelThreeID = Convert.ToInt64(dt.Rows[i]["LevelThreeID"].ToString());
                    dd.LevelOneName = dt.Rows[i]["LevelOneName"].ToString();
                    dd.LevelTwoName = dt.Rows[i]["LevelTwoName"].ToString();
                    dd.LevelThreeName = dt.Rows[i]["LevelThreeName"].ToString();
                    dd.ImagePath = dt.Rows[i]["ImagePath"].ToString();
                    dd.Thumb_ImagePath = string.Empty;
                    //dd.LevelOneSequenceOrder = Convert.ToInt32(dt.Rows[i]["LevelOneSequenceOrder"].ToString());////hide
                    //dd.LevelTwoSequenceOrder = Convert.ToInt32(dt.Rows[i]["LevelTwoSequenceOrder"].ToString());////hide
                    //dd.LevelthreeSequenceOrder = Convert.ToInt32(dt.Rows[i]["LevelthreeSequenceOrder"].ToString());////hide

                    //This below change done for Category=5 (Electricals and Ligting) for Kanpur ("LevelTwoSequenceOrder" was coimg null)
                    dd.LevelOneSequenceOrder = String.IsNullOrEmpty(dt.Rows[i]["LevelOneSequenceOrder"].ToString()) ? 1 : Convert.ToInt32(dt.Rows[i]["LevelOneSequenceOrder"].ToString());
                    dd.LevelTwoSequenceOrder = String.IsNullOrEmpty(dt.Rows[i]["LevelTwoSequenceOrder"].ToString()) ? 1 : Convert.ToInt32(dt.Rows[i]["LevelTwoSequenceOrder"].ToString());
                    dd.LevelthreeSequenceOrder = String.IsNullOrEmpty(dt.Rows[i]["LevelthreeSequenceOrder"].ToString()) ? 1 : Convert.ToInt32(dt.Rows[i]["LevelthreeSequenceOrder"].ToString());
                

                    obj.Add(dd);
                }
            }

            return obj;
        }

    }
}
