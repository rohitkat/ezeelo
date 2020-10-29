using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class PurchaseReportController : ApiController
    {
       // private EzeeloDBContext db = new EzeeloDBContext();
        public object Get(long UserLoginId)
        {
            object obj = new object();
            try
            {
                if (UserLoginId == null || UserLoginId <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid CustomerId.", data = string.Empty };
                }
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                LeaderPurchaseReport objLeaderPurchaseReport = new LeaderPurchaseReport();
                List<PurchaseReportViewModel> objPurchaseList = objLeaderPurchaseReport.GetLeaderPurchaseReport(UserLoginId);// Added by sonali for call common method on 16-02-2019
                //var idParam = new SqlParameter
                //{
                //    ParameterName = "UserID",
                //    Value = UserLoginId

                //};
                //List<PurchaseReportViewModel> objPurchaseList = db.Database.SqlQuery<PurchaseReportViewModel>("EXEC Purchase_Report_Select @UserID", idParam).OrderByDescending(x => x.OrderDate).ToList<PurchaseReportViewModel>();
                obj = new { Success = 1, Message = "Success", data = objPurchaseList };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
