//-----------------------------------------------------------------------
// <copyright file="OwnerPlanViewModel.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class OwnerPlanViewModel
    {
        //public OwnerPlanViewModel()
        //{
        //    this.OwnerPlanCategoryCharges = new List<OwnerPlanCategoryCharge>();
        //}

        public int ID { get; set; }

        [Required(ErrorMessage = "Plan Name is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Plan Name")]
        public int PlanID { get; set; }

        [Required(ErrorMessage = "Owner Name is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Owner Name")]
        public long OwnerID { get; set; }
        public string OwnerName { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public System.DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public System.DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual Plan Plan { get; set; }
    }
}
