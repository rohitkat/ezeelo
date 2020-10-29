//-----------------------------------------------------------------------
// <copyright file="ProductStockVarientViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    //This View Model is used for showing productList on mobile and searchCount
  public class ProductStockVarientViewModel
    {
        public SearchCountViewModel searchCount { get; set; }
        public List<ProductStockDetailViewModel> ProductInfo { get; set; }       
       
    }

  /*Added By Pradnyakar Badge
       * 13-06-2016
       * For New Product List in Mobile APP
       */
  public class Mobile_ProductStockVarientViewModel
  {
      public SearchCountViewModel searchCount { get; set; }
      public List<Mobile_ProductStockDetailViewModel> ProductInfo { get; set; }

  }
}
