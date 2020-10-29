//-----------------------------------------------------------------------
// <copyright file="StockComponentsViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
   public class StockComponentsViewModel
    {
        public List<DisplayCompTodaysRateViewModel> TodaysRate { get; set; }
        public StockComponentsValueViewModel ComponentValue { get; set; }
    }
}
