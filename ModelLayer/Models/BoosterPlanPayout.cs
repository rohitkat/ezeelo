using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("BoosterPlanPayout")]
    public class BoosterPlanPayout
    {
        [Key]
        public long ID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal CoinRate { get; set; }
        public int TotalDeliveries { get; set; }
        public int PendingDeliveries { get; set; }
        public int TotalSubscriber { get; set; }
        public int TotalActiveSubscriber { get; set; }
        public decimal TotalERP { get; set; }
        public decimal PayableERP { get; set; }
        public decimal PendingERP { get; set; }
        public decimal TotalPayableERP { get; set; }
        public DateTime CreateDate { get; set; }
    }

    [Table("BoosterPlanPayoutDetails")]
    public class BoosterPlanPayoutDetails
    {
        [Key]
        public long ID { get; set; }
        public long BoosterPlanPayoutID { get; set; }
        public long UserLoginID { get; set; }
        public bool Status { get; set; }
        public decimal ERP { get; set; }
        public bool IsPaid { get; set; }
        public decimal PendingERP { get; set; }
        public decimal TotalERP { get; set; }
        public long Paid_PendingBoosterPlanPayoutID { get; set; }
    }
}
