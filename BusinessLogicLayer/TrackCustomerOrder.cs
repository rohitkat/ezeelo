//-----------------------------------------------------------------------
// <copyright file="TrackCustomerOrder.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------

using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Enum;
using ModelLayer.Models.ViewModel;
/*
 Handed over to Pradnyakar Sir
 */
namespace BusinessLogicLayer
{
  public class TrackCustomerOrder: CustomerManagement 
    {

        /// <summary>
        /// Base class Constructor
        /// </summary>
        /// <param name="server">System.Web.HttpContext.Current.Server</param>
        public TrackCustomerOrder(System.Web.HttpServerUtility server): base(server)
        {
        }

        /// <summary>
        /// Track order status for shopstock id in a order
        /// </summary>
        /// <param name="orderID">Order ID</param>
        /// <param name="shopStockID">Shop Stock ID</param>
        /// <returns></returns>
        public List<TrackShipmentViewModel> GetOrderProductHistory(long orderID, long shopStockID)
        {
            //retrive order status other than InGodown and Dispatch from Godown
            List<TrackShipmentViewModel> lHistory = new List<TrackShipmentViewModel>();
                        lHistory = (from oh in db.CustomerOrderHistories
                        where oh.CustomerOrderID == orderID && oh.ShopStockID == shopStockID
                        && oh.Status != 5 && oh.Status != 6                        
                        select new TrackShipmentViewModel
                        {
                            CreateDate = oh.CreateDate,
                            Status = oh.Status
                        }).GroupBy(x => x.Status).Select(x => x.FirstOrDefault()).OrderBy(x => x.Status).ToList();        
     
            return lHistory;          

        }

    }
}
