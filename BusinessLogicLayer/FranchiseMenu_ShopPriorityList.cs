using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class FranchiseMenu_ShopPriorityList
    {

        public List<FranchiseMenu_ShopPriorityListViewModel> selectFranchiseMenu_ShopPriorityList(System.Web.HttpServerUtility server, Int64 CityID, Int64 CategoryID, int? FranchiseID)////Added params Int32[] FranchiseID for Multiple MCO/Old App
        {
            List<FranchiseMenu_ShopPriorityListViewModel> obj = new List<FranchiseMenu_ShopPriorityListViewModel>();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(CityID);
            if (FranchiseID!=null) 
            {
                paramValues.Add(FranchiseID);//Added FranchiseID
            }
            else
            {
                paramValues.Add(null);
            }
            paramValues.Add(CategoryID);
            paramValues.Add(config.CATEGORY_IMAGE_HTTP);
            DataTable dt = new DataTable();
            dt = dbOpr.GetRecords("FranchiseMenu_ShopPriorityLsit", paramValues);

            obj = (from n in dt.AsEnumerable()
                   select new FranchiseMenu_ShopPriorityListViewModel
                   {
                       ShopName = n.Field<string>("ShopName"),
                       ShopID = n.Field<Int64?>("ShopID"),
                       FirstLevel_Name = n.Field<string>("FirstLevel_Name"),
                       FirstLevel_ID = n.Field<Int64?>("FirstLevel_ID"),
                       ThirdLevel_Name = n.Field<string>("ThirdLevel_Name"),
                       ThirdLevel_ID = n.Field<Int64?>("ThirdLevel_ID"),
                       FirstLevel_Image = n.Field<string>("FirstLevel_Image"),
                       PriorityLevel = n.Field<int?>("PriorityLevel"),
                       ShopPriorityLevel = n.Field<int?>("ShopPriorityLevel")

                      // ShopLogoPath = (CategoryID > 0) ? (ImageDisplay.LoadShopLogo((Int64)n.Field<Int64?>("ShopID"), ProductUpload.IMAGE_TYPE.Approved)) : string.Empty

                   }).ToList();

            return obj;
        }

        public List<FranchiseMenu_ShopPriorityListViewModel> selectFranchiseMenu_ShopPriorityList_SecondLevelWise(System.Web.HttpServerUtility server, Int64 CityID, Int64 CategoryID, int? FranchiseID=null)////Added Int? FranchiseID for Multiple MCO/Old App
        {
            List<FranchiseMenu_ShopPriorityListViewModel> obj = new List<FranchiseMenu_ShopPriorityListViewModel>();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(CityID);
            paramValues.Add(FranchiseID);////Added FranchiseID
            paramValues.Add(CategoryID);
            paramValues.Add(config.CATEGORY_IMAGE_HTTP);
            DataTable dt = new DataTable();
            dt = dbOpr.GetRecords("FranchiseMenu_ShopPriorityList_SecondLevelWise", paramValues);

            obj = (from n in dt.AsEnumerable()
                   select new FranchiseMenu_ShopPriorityListViewModel

                   {
                       ShopName = n.Field<string>("ShopName"),
                       ShopID = n.Field<Int64?>("ShopID"),
                       FirstLevel_Name = n.Field<string>("FirstLevel_Name"),
                       FirstLevel_ID = n.Field<Int64?>("FirstLevel_ID"),
                       SecondLevel_Name = n.Field<string>("ThirdLevel_Name"),
                       SecondLevel_ID = n.Field<Int64?>("ThirdLevel_ID"),
                       FirstLevel_Image = n.Field<string>("FirstLevel_Image"),
                       PriorityLevel = n.Field<int?>("PriorityLevel"),
                       ShopPriorityLevel = n.Field<int?>("ShopPriorityLevel")

                   }).ToList();

            return obj;
        }

        public List<FranchiseMenu_ShopPriorityListViewModel> FranchiseMenu_ShopPriorityList_SecondLevelWise_FirstLevelCategory(System.Web.HttpServerUtility server, Int64 ShopID, Int64 CategoryID)
        {
            List<FranchiseMenu_ShopPriorityListViewModel> obj = new List<FranchiseMenu_ShopPriorityListViewModel>();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(ShopID);
            paramValues.Add(CategoryID);
            paramValues.Add(config.CATEGORY_IMAGE_HTTP);
            DataTable dt = new DataTable();
            dt = dbOpr.GetRecords("FranchiseMenu_ShopPriorityList_SecondLevelWise_FirstLevelCategory", paramValues);

            obj = (from n in dt.AsEnumerable()
                   select new FranchiseMenu_ShopPriorityListViewModel
                   {
                       ShopName = n.Field<string>("ShopName"),
                       ShopID = n.Field<Int64?>("ShopID"),
                       FirstLevel_Name = n.Field<string>("FirstLevel_Name"),
                       FirstLevel_ID = n.Field<Int64?>("FirstLevel_ID"),
                       ThirdLevel_Name = n.Field<string>("ThirdLevel_Name"),
                       ThirdLevel_ID = n.Field<Int64?>("ThirdLevel_ID"),
                       FirstLevel_Image = n.Field<string>("FirstLevel_Image"),
                       PriorityLevel = n.Field<int?>("PriorityLevel"),
                       ShopLogoPath = n.Field<Int64?>("ShopID") == null ? "" + (new URLsFromConfig()).GetURL("IMG") + "/hpi/4968/1/1.png" : ImageDisplay.LoadShopLogo(n.Field<Int64>("ShopID"), ProductUpload.IMAGE_TYPE.Approved)
                   }).ToList();

            return obj;
        }
    
    }
}
