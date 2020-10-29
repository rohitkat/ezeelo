//-----------------------------------------------------------------------
// <copyright file="ProductFlatDiscountOfferViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class ProductFlatDiscountOfferViewModel
    {
        public decimal  FlatDiscount { get; set; }

        public string Description { get; set; }

        public int OfferType { get; set; }
    }
}