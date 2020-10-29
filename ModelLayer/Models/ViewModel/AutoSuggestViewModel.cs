//-----------------------------------------------------------------------
// <copyright file="AutoSuggestViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.ViewModel
{
    public class AutoSuggestViewModel
    {
        public string Name { get; set; }
        public string  ID { get; set; }
        public string Abbr { get; set; }
        public string  Head { get; set; }
        public string Seperator { get; set; }
        public string Products { get; set; }
        public string CategoryName { get; set; }
        public int CategoryLevel { get; set; }
        public Boolean IsManagedItem { get; set; }
        //Yashaswi 26-9-2018
        public string SaleRate { get; set; }
        public string VarientName { get; set; }
        public int FranchiseId { get; set; }//Added by Sonali_01-12-2018
        public int CityId { get; set; }//Added by Sonali_01-12-2018
        public long UserID { get; set; }//Added by Sonali_03-12-2018
    }
}