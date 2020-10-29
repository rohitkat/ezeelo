//-----------------------------------------------------------------------
// <copyright file="ViewCustomerOrderViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
   public class ViewCustomerOrderViewModel
    {
     public List<CustomerOrderViewModel> Orders { get; set; }
     public List<CustomerOrderDetailViewModel> OrderProducts { get; set; }

     public List<CalculatedTaxList> lCalculatedTaxList { get; set; }

    }

}
