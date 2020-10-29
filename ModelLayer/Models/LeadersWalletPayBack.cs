using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("LeadersWalletPayBack")]
    public class LeadersWalletPayBack
    {
        [Key]
        public long Id { get; set; }
        public long MLMWalletTransactionId { get; set; }
        public int Status { get; set; }
        public decimal ReturnAmount { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long? ModifyBy { get; set; }
        public string NetworkIP { get; set; }
    }

    public class LeadersWalletPayBackOrderDataViewModel
    {
        public long UserLoginID { get; set; }
        public long CustomerOrderId { get; set; }
        public string Name { get; set; }
        public string MobilNo { get; set; }
        public string OrderCode { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal RetailPoints { get; set; }
        public decimal WalletAmountUsed { get; set; }
        public int Status { get; set; }
        public DateTime TranDate { get; set; }
        public decimal ReturnAmount { get; set; }
        public bool IsChecked { get; set; }
        public long MLMWalletTransactionId { get; set; }
    }

    public class WalletPayBackViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int callType { get; set; }
        public List<LeadersWalletPayBackOrderDataViewModel> list { get; set; }
    }
}
