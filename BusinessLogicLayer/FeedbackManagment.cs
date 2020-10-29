using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.ViewModel;
using System.Data;

namespace BusinessLogicLayer
{
    public class FeedbackManagment
    {
       public List<FeedBackManagmentViewModel> FeedBackMIS(int ID, System.Web.HttpServerUtility server)
        {
            List<object> paramValues = new List<object>();
            paramValues.Add(ID);

            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);

            dt = dbOpr.GetRecords("Select_FeedbackManagment", paramValues);
            List<FeedBackManagmentViewModel> objFeedBackManagmentViewModel = new List<FeedBackManagmentViewModel>();

            for (int item = 0; item < dt.Rows.Count; item++)
            {
                FeedBackManagmentViewModel obj = new FeedBackManagmentViewModel();
                obj.ID = Convert.ToInt32(dt.Rows[item]["ID"].ToString());
                obj.FeedBackDetail = dt.Rows[item]["FeedBackDetail"].ToString();
                obj.CreateDate =Convert.ToDateTime(dt.Rows[item]["CreateDate"].ToString());

                objFeedBackManagmentViewModel.Add(obj);
            }

            return objFeedBackManagmentViewModel;
           }
    }
}
