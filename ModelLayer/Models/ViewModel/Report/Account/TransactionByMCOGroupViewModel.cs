using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel.Report.Account
{
    public class TransactionByMCOGroupViewModel
    {
        public int QtyAskedByCustomer { get; set; }
        public int QtyShopOut { get; set; }
        public int QtyReceivedByCustomer { get; set; }
        public decimal MCOTotalMRP { get; set; }
        public decimal MCOTotalSaleRate { get; set; }
        public decimal MCOCustomerReceivable { get; set; }
        public decimal MCOShopReceivable { get; set; }
        public decimal MCODeliveryReceivable { get; set; }
        public decimal AmountRemaining { get; set; }
        public string ProcessRemark { get; set; }
    }
}
