using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class OnlinePaymentTransactionViewModel
    {

        [Required]
        public string PaymentMode { get; set; }
        [Required]
        public long CustomerLoginID { get; set; }
        [Required] 
        public string PaymentGateWayTransactionId { get; set; }
        [Required]
        public int PaymentStatus { get; set; }
        [Required]
        public long CustomerOrderID { get; set; }

        public string Description { get; set; }

        public string DeviceType { get; set; }
    }
}
