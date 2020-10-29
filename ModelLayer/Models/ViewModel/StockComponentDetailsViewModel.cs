//-----------------------------------------------------------------------
// <copyright file="StockComponentDetailsViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
 public class StockComponentDetailsViewModel
    {
        public int ComponentID { get; set; }
        public string ComponentName { get; set; }
        public decimal ComponentWeight { get; set; }
        public int ComponentUnitID { get; set; }
        public string ComponentUnitName { get; set; }
        public decimal PerUnitRateInRs { get; set; }
        public decimal PerUnitRateInPer { get; set; }
        public int DependentOnComponentID { get; set; }
        public int DepComponentUnitID { get; set; }
    }
}
