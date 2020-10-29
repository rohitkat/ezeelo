//-----------------------------------------------------------------------
// <copyright file="RelatedProductsViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class RelatedProductsViewModel
    {
        public SearchCountViewModel SearchCount { get; set; }
        public List<SearchProductDetailsViewModel> ProductList { get; set; }
    }
}