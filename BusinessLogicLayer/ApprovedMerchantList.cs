using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class ApprovedMerchantList
    {
        public List<ApprovedMerchantListByFranchise> MerchantList(List<object> paramValues, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);

            dt = dbOpr.GetRecords("ApprovedMerchantList", paramValues);
            List<ApprovedMerchantListByFranchise> AML = new List<ApprovedMerchantListByFranchise>();

            for (int item = 0; item < dt.Rows.Count; item++)
            {
                ApprovedMerchantListByFranchise obj = new ApprovedMerchantListByFranchise();
                obj.BusinessName = dt.Rows[item]["BusinessName"].ToString();
                obj.ContactPersone = dt.Rows[item]["ContactPerson"].ToString();
                obj.MerchantName = dt.Rows[item]["MerchantName"].ToString();
                obj.ShopName = dt.Rows[item]["ShopName"].ToString();
                obj.MerchantAddress = dt.Rows[item]["MerchantAddress"].ToString();
                obj.Pincode = dt.Rows[item]["Pincode"].ToString();
                obj.CityName = dt.Rows[item]["CityName"].ToString();
                obj.PlanName = dt.Rows[item]["PlanName"].ToString();
                obj.UserLoginID = Convert.ToInt64(dt.Rows[item]["UserLoginID"].ToString());
                obj.MerchantID = Convert.ToInt64(dt.Rows[item]["MerchantID"].ToString());
                obj.Email = dt.Rows[item]["Email"].ToString();
                obj.Mobile = dt.Rows[item]["Mobile"].ToString();
                obj.SendBy = Convert.ToInt64(dt.Rows[item]["SendBy"].ToString());
                obj.LetterDate = dt.Rows[item]["LetterDate"].ToString();
                AML.Add(obj);
            }

            return AML;
        }

        public List<MerchantPlanCategories> MerchantCategoryList(List<object> paramValues, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);

            dt = dbOpr.GetRecords("Select_welComeLetterCategory", paramValues);
            List<MerchantPlanCategories> AML = new List<MerchantPlanCategories>();

            for (int item = 0; item < dt.Rows.Count; item++)
            {
                MerchantPlanCategories obj = new MerchantPlanCategories();
                obj.categoryID = Convert.ToInt64(dt.Rows[item]["CategoryID"].ToString());
                obj.categoryName = dt.Rows[item]["CategoryName"].ToString();
                obj.chargesInPercentage = dt.Rows[item]["ChargeInPercent"].ToString();
                obj.chargesInRs = dt.Rows[item]["ChargeInRupee"].ToString();                
                AML.Add(obj);
            }

            return AML;
        }

    }

    public class ApprovedMerchantListByFranchise
    {
        public string BusinessName { get; set; }
        public string ContactPersone { get; set; }
        public string MerchantName { get; set; }
        public string ShopName { get; set; }
        public string MerchantAddress { get; set; }
        public string Pincode { get; set; }
        public string CityName { get; set; }
        public string PlanName { get; set; }
        public long UserLoginID { get; set; }
        public long MerchantID { get; set; }
        public long? SendBy { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string LetterDate { get; set; }
        //public string parentCategory { get; set; }
        //public long parentCategoryID { get; set; }
        //List<MerchantPlanCategories> merchantPlanCategories { get; set; }

    }

    public class MerchantPlanCategories
    {
        public string parentCategory { get; set; }
        //public long parentCategoryID { get; set; }
        public long categoryID { get; set; }
        public string categoryName { get; set; }
        public string chargesInPercentage { get; set; }
        public string chargesInRs { get; set; }
    }

}
