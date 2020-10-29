using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class MerchantUserDashboardViewModel
    {
        public decimal RM { get; set; }
        public decimal L0 { get; set; }
        public decimal L6 { get; set; }
        public decimal Unpaid { get; set; }
        public int Days { get; set; }
        public decimal EzeeMoney { get; set; }
        public decimal PendingEzeeMoney { get; set; }
        public decimal InactivePoints { get; set; }
        public decimal PendingTransAmount { get; set; }
    }

    public class LeaderERPFromMerchant
    {
        public long SrNo { get; set; }
        public string Shop { get; set; }
        public string City { get; set; }
        public string UserName { get; set; }
        public decimal ERP { get; set; }
    }
}
