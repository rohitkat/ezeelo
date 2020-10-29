using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//<copyright file="StateFranchiseMerchantViewModel.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
//    </copyright>
//    <author>Harshada Raghorte</author>
namespace ModelLayer.Models.ViewModel
{
    public class StateCityFranchiseMerchantViewModel
    {
        public long CityID { get; set; }
        public string CityName { get; set; }

        public int AreaID { get; set; }
        public string AreaName { get; set; }
        public long FranchiseID { get; set; }
        public string FranchiseName { get; set; }
        public long MerchantID { get; set; }
        public string MerchantName { get; set; }
        public long ShopID { get; set; }
        public string ShopName { get; set; }
        public long CategoryID { get; set; }
        public string CategoryName { get; set; }
    }
}
