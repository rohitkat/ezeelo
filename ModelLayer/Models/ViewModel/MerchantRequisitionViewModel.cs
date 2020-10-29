using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class MerchantRequisitionViewModel
    {
        public long UserLoginID { get; set; }
        public long ShopID { get; set; }

        [Display(Name = "Shop Name")]
        public string ShopName { get; set; }

        [Display(Name = "Owner Name")]
        public string MerchantName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string TIN { get; set; }
        public string Pincode { get; set; }

        [Display(Name = "Franchise")]
        public Nullable<int> FranchiseID { get; set; }

        [Display(Name = "Payment Mode")]
        public bool IsPaymentMode { get; set; }

        public List<CustomPaymentMode> PaymentModeList { get; set; }

    }

   public class CustomPaymentMode
   {
       public int ID { get; set; }
       public string Name { get; set; }
       public bool IsSelected { get; set; }
   }
}
