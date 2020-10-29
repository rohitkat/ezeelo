using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class InactivePayoutFreezeDataReportViewModel
    {
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string PhoneNo { get; set; }
        public decimal InactivePoint { get; set; }
        public decimal EzeeMoney { get; set; }
    }
}
