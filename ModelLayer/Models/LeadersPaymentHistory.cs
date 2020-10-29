using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{

    // Added by Amit on 4/8/2018

    [Table("Leaders_Payment_History")]
  public  class LeadersPaymentHistory
    {
        [Key]
        public long ID { get; set; }
        public long UserID { get; set; }
        public string Payout_Month { get; set; }
        public string Payout_Year { get; set; }
        public DateTime? Payment_date { get; set; }
        public decimal? Payout_Amount { get; set; }
        public string Min_Resereved { get; set; }
        public decimal? GST_Amount { get; set; }
        public decimal? TDS_Amount { get; set; }
        public decimal? Processing_Fees { get; set; }
        public string Penalty { get; set; }
        public decimal? ERP { get; set; }
        public decimal? Total_Order_Amount { get; set; }
        public DateTime? Create_Date { get; set; }
        public DateTime? Modify_Date { get; set; }

    }
}
