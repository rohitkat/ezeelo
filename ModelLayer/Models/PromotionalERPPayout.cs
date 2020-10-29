using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("PromotionalERPPayout")]
    public class PromotionalERPPayout
    {
        [Key]
        public long Id { get; set; }
        public bool ActiveUser { get; set; }
        public int Level { get; set; }
        public long EzeeMoneyPayoutId { get; set; }
        public decimal Amount { get; set; }
        public decimal ERP { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalERP { get; set; }
        public string ReferenceText { get; set; }
        public bool IsPaid { get; set; }
        public DateTime FreezeDate { get; set; }
        public long FreezeBy { get; set; }
        public DateTime? PaidDate { get; set; }
        public long? PaidBy { get; set; }
        public string NetworkIp { get; set; }
        public string VerficationCode { get; set; }
        public long Cities { get; set; } //Yashaswi 06/12/2018 Promo ERP
    }

    [Table("PromotionalERPPayoutDetails")]
    public class PromotionalERPPayoutDetails
    {
        [Key]
        public long Id { get; set; }
        public long PromotionalERPPayoutId { get; set; }
        public long UserLoginId { get; set; }
        public decimal ERP { get; set; }
        public decimal EzeeMoney { get; set; }
    }
}
