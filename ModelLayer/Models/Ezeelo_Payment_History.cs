using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("Ezeelo_Payment_History")]


    // Added by Amit
   public class Ezeelo_Payment_History
    {
        [Key]
        public long ID { get; set; }
        public long UserID { get; set; }
        public string Payout_Month { get; set; }
        public string Payout_Year { get; set; }
        public DateTime? Payment_Date { get; set; }
        public decimal? Payment_Amount { get; set; }
        public decimal? ERP { get; set; }
        public decimal? Total_Order_Amount { get; set; }
        public decimal? Total_Business_Points { get; set; }
        public decimal? Coin_Rate { get; set; }

    }
}
