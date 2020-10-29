//-----------------------------------------------------------------------
// <copyright file="OwnerPlanCategoryChargeViewModel.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Snehal Shende</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    
    public partial class OwnerPlanCategoryChargeViewModel
    {
        public int ID { get; set; }                
        public string OwnerPlanName { get; set; }        
        public int OwnerPlanID { get; set; }
        public string PlanName { get; set; }

        [Required(ErrorMessage = "Plan Name is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Plan Name")]
        public int PlanID { get; set; }
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Category is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Category")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Charge Name is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Charge Name")]
        public int ChargeID { get; set; }
        public string ChargeName { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Invalid Charge")]
        public decimal ChargeInPercent { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Invalid Charge")]
        public decimal ChargeInRupee { get; set; }
        public bool IsActive { get; set; }
        public int PlanCategoryChargeID { get; set; }
    }
}
