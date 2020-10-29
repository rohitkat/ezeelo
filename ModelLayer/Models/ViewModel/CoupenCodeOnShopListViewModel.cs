using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class CoupenCodeOnShopListViewModel
    {
       public List<ShopWiseDeliveryCharges> ShopWiseDelivery { get; set; }       
       [Required]
       public string CoupenCode { get; set; }

       public long CustomerLoginID { get; set; }

       public long CityID { get; set; }
       public int? FranchiseID { get; set; }////added by Ashish FranchiseID for Multiple MCO
       public int? Version { get; set; }////Added by Ashish For New App

    }
}
