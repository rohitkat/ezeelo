//-----------------------------------------------------------------------
// <copyright file="ShopWiseDeliveryCharges" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
  public class ShopWiseDeliveryCharges
    {
       public int ID { get; set; }
       [Required(ErrorMessage = "Please provide Shop ID")]
        public long ShopID { get; set; }
       [Required(ErrorMessage = "Please provide product weight")]
        public decimal Weight { get; set; }
      
       [Required(ErrorMessage = "Please provide Shop Order Amount")]
        public decimal OrderAmount { get; set; }
         [Required(ErrorMessage = "Please provide delivery charge against shop products")]
        public decimal DeliveryCharge { get; set; }
         [Required(ErrorMessage = "Please provide delivery type.e.g. Express/Normal")]
        public string DeliveryType { get; set; }
        public string ShopOrderCode { get; set; }
        public int DeliveryPartnerID { get; set; }

      //Added by Tejaswee (23-12-2015)
      
        public Nullable<long> CatID { get; set; }

    }
}
