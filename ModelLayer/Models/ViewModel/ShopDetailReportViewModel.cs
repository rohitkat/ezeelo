using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ShopDetailReportViewModel
    {
        public long UserLoginID { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public long ShopID { get; set; }
        public string ShopName { get; set; }
        public string Address { get; set; }
        public string AlternateMobile { get; set; }
        public string MerchantName { get; set; }
        public string LandLine { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean IsLive { get; set; }
        public DateTime ShopCreateDate { get; set; }
       
    }
}
