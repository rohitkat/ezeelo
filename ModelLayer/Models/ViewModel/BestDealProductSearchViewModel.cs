//-----------------------------------------------------------------------
// <copyright file="BestDealProductSearchViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
  public  class BestDealProductSearchViewModel
    {
        public int CityID { get; set; }
        public long ShopID { get; set; }
      
      [Required(ErrorMessage = "Please Provide Offer ID. Numeric only.")]
        public int OfferID { get; set; }
       [Required(ErrorMessage = "Please Provide Offer Type e.g. FLAT_DISCOUNT_OFFER/FREE_OFFER/COMPONENT_OFFER.")]
        public string OfferType { get; set; }
        public long CategoryID { get; set; }
        public long BrandID { get; set; }
      [Required(ErrorMessage = "Please Provide PageIndex. Numeric only.")]
        public int PageIndex { get; set; }
      [Required(ErrorMessage = "Please Provide PageSize. Numeric only.")]
        public int PageSize { get; set; }
      public int FranchiseID { get; set; }////added

    }
}
