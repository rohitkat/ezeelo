//-----------------------------------------------------------------------
// <copyright file="SearchSimilarProductViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.ViewModel
{
    public class SearchSimilarProductViewModel
    {
        public long CityID { get; set; }
        [Required]
        public int CategoryID { get; set; }
        public long ProductID { get; set; }
        public long ShopID { get; set; }
        [Required]
        public int PageIndex { get; set; }
        [Required]
        public int PageSize { get; set; }
        public int FranchiseID { get; set; }//added
    }
}