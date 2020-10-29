using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//<copyright file="ProductApprovedViewModel.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
//    </copyright>
//    <author>Harshada Raghorte</author>
namespace ModelLayer.Models.ViewModel
{
    public class ProductApprovedViewModel
    {
        public long ProductID { get; set; }
        public long ShopID { get; set; }
        public string ProductName { get; set; }
        public string productDescription { get; set; }
        public string MerchantName { get; set; }
        public string ShopName { get; set; }
        public string ApprovalRemark { get; set; }
        public DateTime ProductUploadDate { get; set; }
        public DateTime ? ProductApprovedDate { get; set; }

        public DateTime ? ProductModifiedDate { get; set; }
        public long CatID { get; set; }
        public string CatName { get; set; }
        public Boolean IsActive { get; set; }

    }
}
