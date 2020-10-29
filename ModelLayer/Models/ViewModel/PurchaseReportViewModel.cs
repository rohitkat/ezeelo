using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class PurchaseReportViewModel
    {
        public string OrderCode { get; set; }
        public Double? OrderAmt { get; set; }
        public string paymentmode { get; set; }
        public Double? TransactionPts { get; set; }
        public string OrderStatus { get; set; }

       [DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy HH:mm}",
            ApplyFormatInEditMode = true)]
        public DateTime? OrderDate { get; set; }

        public int OrdStatusNo { get; set; } // added by amit on 7-2-19

        public string Status { get; set; } // added by amit on 7-2-19
        [DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy HH:mm}",
            ApplyFormatInEditMode = true)]
        public DateTime? DeliveryDate { get; set; } // added by amit 

        public Double? WalletAmountUsed { get; set; } // added by amit 

        // public List<CustomerOrderDetail> OrderDetailsList { get; set; }



    }
}
