//-----------------------------------------------------------------------
// <copyright file="DisplayReviewsViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
  public  class DisplayReviewsViewModel
    {
      public List<CustomerReviewViewModel> CustomerReviewList { get; set; }
      public AverageRatingPointsViewModel AvgPoints { get; set; }
      public string ThumbPath { get; set; }

      public long prodId { get; set; }

      public bool IsDisplay { get; set; }
    }
}
