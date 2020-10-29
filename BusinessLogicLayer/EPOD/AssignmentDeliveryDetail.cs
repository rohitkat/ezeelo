using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class AssignmentDeliveryDetail
    {
        public long ID { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Pincode { get; set; }
        public string Area { get; set; }
        public string Mobile { get; set; }
        public string OTP { get; set; }
        public decimal OTPAmount { get; set; }


        public List<AssignedTrackOrderDelivery> AssignedTrackOrderDelivery { get; set; }

    }
}
