using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
   public class CareerDetails
    {
       public List<CareerAppicationPostViewModel> getApplicationList(List<object> paramValues,System.Web.HttpServerUtility server)
       {
           DataTable dt = new DataTable();
           ReadConfig config = new ReadConfig(server);
           DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
           //List<object> paramValues = new List<object>();
           //paramValues.Add(postID);           
           dt = dbOpr.GetRecords("getApplicationList", paramValues);

           List<CareerAppicationPostViewModel> ls = new List<CareerAppicationPostViewModel>();

           ls = (from n in dt.AsEnumerable()
                 select new CareerAppicationPostViewModel
                 {
                     ID = n.Field<Int64>("ID"),
                     CareerID = n.Field<Int32>("CareerID"),
                     Jobtitle = n.Field<string>("Jobtitle"),
                     Name = n.Field<string>("Name"),
                     Email = n.Field<string>("Email"),
                     Mobile = n.Field<string>("Mobile"),
                     TotalExpience = n.Field<string>("TotalExpience"),
                     CurrentCTC = n.Field<string>("CurrentCTC"),
                     ExpectedCTC = n.Field<string>("ExpectedCTC"),
                     ResumePath = n.Field<string>("ResumePath"),
                     Remarks = n.Field<string>("Remarks"),
                     serverPath = config.IMAGE_HTTP + "/" + ProductUpload.CAREER_ROOTPATH + "/" + n.Field<string>("ResumePath")
                     // TotalCount = n.Field<Int32>("TotalCount")
                 }).ToList();

               return ls;
       }
    }
}
