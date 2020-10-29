//-----------------------------------------------------------------------
// <copyright file="FranchisePendingApprovalViewModel.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Snehal Shende</author>
//-----------------------------------------------------------------------
    
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.ViewModel
{    
    public class FranchisePendingApprovalViewModel
    {
        public Int64 UserLoginID { get; set; }        
        public string BusinessTypePrefix { get; set; }
        public string Name { get; set; }
        public Int64? OwnerId { get; set; }
    }
}