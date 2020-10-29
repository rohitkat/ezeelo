using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class CorporateDetail
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int Quantity { get; set; }
        public string PrimaryMob { get; set; }
        public string SecondaryMob { get; set; }
        public int PincodeID { get; set; }
        public int AreaID { get; set; }
        public decimal DeliveryCharges { get; set; }
        public string FromAddress { get; set; }
        public string FromPrimaryMob { get; set; }
        public string FromSecondaryMob { get; set; }
        public int FromPincodeID { get; set; }
        public int FromAreaID { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalOrderAmount { get; set; }
        public decimal TotalProductAmount { get; set; }
        public decimal TotalFacilityCharge { get; set; }

        public decimal TotalDeliveryCharge { get; set; }

    }
}
