using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
  public class GetShopWiseDeliveryChargesViewModel
    {
        public string Pincode { get; set; }
        public long CityID { get; set; }//Add for app
        public List<ShopWiseDeliveryCharges> ShopWiseDelivery { get; set; }
        public bool IsExpress { get; set; }
        
      

    }
}
