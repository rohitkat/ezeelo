using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ClaimAmountTowardsDVViewModel
    {
        public long ID { get; set; }
        public string ProductName { get; set; }
        public long ProductId { get; set; }
        public long VarientId { get; set; }
        public string VarientName { get; set; }
        public string Batch { get; set; }
        public long Qty { get; set; }
        public decimal DecidedSalePrice { get; set; }
        public decimal DiversionInSalePrice { get; set; }
        public decimal ClaimAmount { get; set; }
        public long WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public DateTime POReplyDate { get; set; }
        public string OrderCode { get; set; }
    }
}
