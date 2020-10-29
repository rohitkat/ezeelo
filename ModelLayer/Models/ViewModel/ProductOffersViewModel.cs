//-----------------------------------------------------------------------
// <copyright file="ProductOffersViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.ViewModel
{
    public class ProductOffersViewModel
    {
        public ProductFlatDiscountOfferViewModel FlatOffer { get; set; }        
        public ProductSpecialOfferViewModel  FlatFreOffer { get; set; }
        public ProductComponentOfferViewModel ComponentOffer { get; set; }
    }
}