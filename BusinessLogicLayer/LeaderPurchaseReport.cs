using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    /// -----<summary>
    /// Created By Sonali
    /// Date : 16-02-2019
    /// Use : Load Leaders Purchase Report
    /// Called From : Leader Module, API module
    /// </summary>
    public class LeaderPurchaseReport
    {
        EzeeloDBContext db = new EzeeloDBContext();
        #region Purchase list
        /// <summary>
        /// Return purchase report 
        /// </summary>
        /// <param name="LoginUserId"></param>
        /// <returns> purchase list</returns>
        public List<PurchaseReportViewModel> GetLeaderPurchaseReport(long userID)
        {
            List<PurchaseReportViewModel> purchasereportlist = new List<PurchaseReportViewModel>();
            try
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "UserID",
                    Value = userID

                };
                purchasereportlist = db.Database.SqlQuery<PurchaseReportViewModel>("EXEC Purchase_Report_Select @UserID", idParam).OrderByDescending(x => x.OrderDate).ToList<PurchaseReportViewModel>();

                foreach (var item in purchasereportlist)
                {

                    var customerID = db.CustomerOrders.Where(x => x.OrderCode == item.OrderCode).Select(y => y.ID).FirstOrDefault();

                    var checkPartialStatus = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == customerID).Select(Y => Y.OrderStatus).Distinct().Count();


                    if (item.OrdStatusNo == 7)
                    {
                        item.OrderStatus = "DELIVERED";

                    }

                    if (item.OrdStatusNo == 8)
                    {
                        if (checkPartialStatus > 1)
                        {
                            item.Status = "PARTIAL RETURNED";
                            item.OrderStatus = "DELIVERED";
                        }
                        else if (checkPartialStatus == 1)
                        {
                            item.Status = "RETURNED";

                        }
                    }
                    else if (item.OrdStatusNo == 9)
                    {
                        if (checkPartialStatus > 1)
                        {
                            item.Status = "PARTIAL CANCELLED";
                            item.OrderStatus = "DELIVERED";
                        }
                        else if (checkPartialStatus == 1)
                        {
                            item.Status = "CANCELLED";
                        }
                    }
                    else if (item.OrdStatusNo == 10)
                    {
                        item.Status = "ABANDONED";
                        item.OrderStatus = "ABANDONED";
                    }
                    else
                    {
                        item.Status = item.OrderStatus;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return purchasereportlist;
        }
        #endregion
    }
}
