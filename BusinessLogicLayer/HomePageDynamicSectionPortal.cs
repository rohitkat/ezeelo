using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models;

namespace BusinessLogicLayer
{
    public class HomePageDynamicSectionPortal
    {
        EzeeloDBContext db = new EzeeloDBContext();

        public static DataTable Select_GalleryProducts(Int64 FranchiseID, Int64 HomepageDynamicSectionID, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseID);
            paramValues.Add(HomepageDynamicSectionID);

            dt = dbOpr.GetRecords("Select_HomeProductGallerysetsequence", paramValues);

            return dt;
        }

        public string HomePageBannerSequenceUpdate(ModelLayer.Models.ViewModel.HomePageDynamicBannerSetSequence ls, Int64 FranchiseID, Int64 userID, System.Web.HttpServerUtility server)
        {
            try
            {
                foreach (var item in ls.HomeDynamicBannersetsequence)
                {

                    HomePageDynamicSectionBanner objBannerList = db.HomePageDynamicSectionBanner.FirstOrDefault(p => p.ID == item.ID);
                    objBannerList.ModifyDate = DateTime.Now;

                    objBannerList.SequenceOrder = item.SequenceOrder;
                    //objBannerList.IsActive = item.IsActive;
                    db.SaveChanges();
                }
                return "Sequence Order Set Successfully";
            }
            catch (Exception ex)
            {
                return "Sorry Unable to set Sequence ........";
            }
        }

        public DataSet GetHomePageDynamicSection(int FranchiseId, bool ShowInApp)
        {
            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);

            string query = "[SP_HomePageDynamicSections]";
            SqlCommand cmd = new SqlCommand(query);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FranchiseID", FranchiseId);
            cmd.Parameters.AddWithValue("@ShowInApp", ShowInApp);
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(readCon.DB_CONNECTION))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    sda.Fill(ds);
                }
            }
            return ds;
        }
    }
}
