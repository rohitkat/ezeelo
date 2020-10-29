//-----------------------------------------------------------------------
// <copyright file="ShopRatingViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class ShopRatingViewModel
    {
       public ShopDetailsViewModel ShopDetails { get; set; }
        public List<CustomerRatingViewModel> RatingList { get; set; }

       //Added by Tejaswee
        public long productId { get; set; }
    }
}
