//-----------------------------------------------------------------------
// <copyright file="ViewShopDetailsViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
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
    public class ViewShopDetailsViewModel
    {
        public ShopDetailsViewModel ShopBasicDetails { get; set; }
        public string ShopDescriptionFilePath { get; set; }
        public List<ImageListViewModel>  ShopImageList { get; set; } 

    }
}
