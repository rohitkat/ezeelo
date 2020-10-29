using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.ViewModel;
using System.Data;
using System.Data.Entity.Validation;
namespace BusinessLogicLayer
{
    public class OrderWiseCouponTransactionReport
    {
        private string serversting { get; set; }
        public OrderWiseCouponTransactionReport(string server)
        {
            this.serversting = server;
        }
        public List<OrderWiseCouponTransactionViewModel> GetOrderWiseCouponTransaction(string FromDate, string ToDate, string CouponCode, int? CouponSchemeID, int Mode,long FranchiseId)
        {
            List<OrderWiseCouponTransactionViewModel> lDashboardGrowthViewModels = new List<OrderWiseCouponTransactionViewModel>();
            DataTable dt = new DataTable();
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(serversting);
            List<object> paramValues = new List<object>();
            paramValues.Add(FromDate);
            paramValues.Add(ToDate);
            paramValues.Add(CouponCode);
            paramValues.Add(CouponSchemeID);
            paramValues.Add(Mode);
            paramValues.Add(FranchiseId);
           
            dt = dbOpr.GetRecords("ReportOrderWiseCouponTransaction", paramValues);

            try
            {               
                    lDashboardGrowthViewModels = (from n in dt.AsEnumerable()
                                                  select new OrderWiseCouponTransactionViewModel
                                                  {
                                                      CouponCode = n.Field<string>("CoupenCode"),
                                                      CouponScheme = n.Field<string>("CouponScheme"),
                                                      CouponAmount = n.Field<decimal>("CoupenAmount"),
                                                      OrderCode = n.Field<string>("OrderCode"),
                                                      CustomerName = n.Field<string>("Customer"),
                                                      PrimaryMobile = n.Field<string>("PrimaryMobile"),
                                                      OrderDate = n.Field<DateTime>("CreateDate"),
                                                      OrderAmount = n.Field<decimal>("OrderAmount"),
                                                      PayableAmount = n.Field<decimal>("PayableAmount")                                                 
                                                     
                                                  }).ToList();
                
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => new { x.ErrorMessage, x.PropertyName });

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
             return lDashboardGrowthViewModels;
        }
    }
}
