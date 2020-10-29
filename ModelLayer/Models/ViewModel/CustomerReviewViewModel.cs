//-----------------------------------------------------------------------
// <copyright file="CustomerReviewViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
   public class CustomerReviewViewModel
    {
       public long ID { get; set; }
       public long CustomerPersonalDetailID { get; set;}
       public string CustomerName { get; set; }
       public Nullable<double> AvgPointsPerCustomer { get; set; }

       public int ReviewCount { get; set; }
       public string Comment { get; set; }
       public System.DateTime ReviewDate { get; set; }
     

    }
}
