//-----------------------------------------------------------------------
// <copyright file="AverageRatingPointsViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
 public class AverageRatingPointsViewModel
    {
     public Nullable<double> AvgRatingPonts { get; set; }
     public int Count { get; set; }
     public long OwnerID { get; set; }
    }
}
