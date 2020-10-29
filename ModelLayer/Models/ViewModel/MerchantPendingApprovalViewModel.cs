//-----------------------------------------------------------------------
// <copyright file="MerchantPendingApprovalViewModel.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Snehal Shende</author>
//-----------------------------------------------------------------------
    
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{    
    public class MerchantPendingApprovalViewModel
    {
        public Int64 ID { get; set; }
        public Int64 UserLoginID { get; set; }
        public string BusinessTypePrefix { get; set; }
        public string Name { get; set; }
        public Int64 OwnerId { get; set; }
        public string ShopName { get; set; }
        public string mobile { get; set; }
        public string Email { get; set; }
        public bool IsLock { get; set; }
    }
}
