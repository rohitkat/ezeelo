using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System.Data;
namespace BusinessLogicLayer
{
    public class MISReports
    {
        /// <summary>
        /// Developed By:- Pradnyakar Badge
        /// To retrive Shop sale Analysis within category 
        /// </summary>
        /// <param name="paramValues">Franchise ID, fromDate, ToDate,FirstLevel Category</param>
        /// <param name="server"></param>
        /// <returns></returns>
        public List<SaleAnalisysMISViewModel> ShopSaleAnalisys_MIS(List<object> paramValues, System.Web.HttpServerUtility server)
        {
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            dt = dbOpr.GetRecords("ShopSaleAnalisys_MIS", paramValues);

            List<SaleAnalisysMISViewModel> ls = new List<SaleAnalisysMISViewModel>();
            ls = (from n in dt.AsEnumerable()
                  select new SaleAnalisysMISViewModel
                  {
                      CategoryName = n.Field<string>("CategoryName"),
                      CityName = n.Field<string>("CityName"),
                      SaleAmount = n.Field<decimal>("SaleAmount"),
                      ShopName = n.Field<string>("ShopName")

                  }).ToList();

            return ls;
        }
    }
}
