using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    // added by amit on 24-8-18
    [Table("LeadersPayoutRequest")]
    public class LeadersPayoutRequest
    {
        [Key]
        public long ID { get; set; }
        public long UserLoginID { get; set; }
        public Decimal? RedeamableAmount { get; set; }
        public Decimal? RequestedAmount { get; set; }
        public Decimal? GST { get; set; }
        public Decimal? GSTAmount { get; set; }
        public Decimal? TDSAmount { get; set; }
        public Decimal? TDS { get; set; }
        public Decimal? MinReserved { get; set; }
        public Decimal? MinReservedAmount { get; set; }
        public Decimal? Penalty { get; set; }
        public Decimal? PenaltyAmount { get; set; }
        public Decimal? ProcessingFees { get; set; }
        public Decimal? TotalAmount { get; set; }
        public DateTime? Create_Date { get; set; }
        public int? RequestStatus { get; set; }
        public DateTime? RequestStatus_Date { get; set; }
        public string Network_IP { get; set; }
        public string Device_IP { get; set; }


        public bool? IsActive { get; set; }


        [DisplayName("Payment Status")]
        [Required(ErrorMessage = "Please add payment status.")]
        public bool? PaymentStatus { get; set; }  // on 22-1-19
        [Required]
        public string Remark { get; set; } // on 22-1-19

        [Required(ErrorMessage = "Please add transaction ID.")]
        public string TransactionID { get; set; }

        public DateTime? TransactionDate { get; set; }  // on 5-2-19

        public int? RequestID { get; set; }  // on 13-2-19



    }
}
