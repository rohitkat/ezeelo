using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("MLMWalletTransaction")]
    public class MLMWalletTransaction
    {
        [Key]
        public long ID { get; set; }
        public long UserLoginID { get; set; }
        public long TransactionTypeID { get; set; }
        public decimal TransactionPoints { get; set; }
        public long MLMCoinRateID { get; set; }
        public decimal OrderAmount { get; set; }
        public string Reference { get; set; }
        public long? CustomerOrderID { get; set; }
        public bool IsAdded { get; set; }
        public decimal? CurrentWalletAmount { get; set; }
        public long AddOrSub { get; set; }
        public decimal? WalletAmountUsed { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long? ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }

    }
}
