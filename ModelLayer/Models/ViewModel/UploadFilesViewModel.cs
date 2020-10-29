//-----------------------------------------------------------------------
// <copyright file="UploadFilesViewModel.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
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
   public class UploadFilesViewModel
    {
        public int MyProperty { get; set; }
        public string FilesToBeUploaded { get; set; }
        public long productID { get; set; }
        public long shopStockID { get; set; }
        public string ColorName { get; set; }
        public int BulkStockLogID { get; set; }
        public int BulkProductLogID { get; set; }

    }
}
