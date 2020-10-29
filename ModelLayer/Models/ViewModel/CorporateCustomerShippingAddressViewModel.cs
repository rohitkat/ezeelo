using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class CorporateCustomerShippingAddressViewModel
    {
        
        public string ToName { get; set; }
        public decimal DeliveryCharges { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public int Quantity { get; set; }
        public string PrimaryMobile { get; set; }
        public string SecondaryMobile { get; set; }
        public string ShippingAddress { get; set; }
        public int PincodeID { get; set; }
        public int AreaID { get; set; }        

    }

    public class CorporateFacilityDetailsViewModel
    {
       public Int64 FacilityID { get; set; }
        public Int64 CustomerOrderDetailID { get; set; }
        public decimal ShippingFacilityCharges { get; set; }
    }


}
