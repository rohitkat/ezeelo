﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class DynamicCategoryProductList
    {
        public List<DynamicCategoryProductViewModel> categoryProductList { get; set; }
    }

    public class DynamicCategoryProductViewModel
    {
        public Int64 ID { get; set; }
        public int SequenceOrder { get; set; }
        public int CategoryID { get; set; }
        public int FranchiseID { get; set; }
        public string CategoryName { get; set; }
        public Int64 ProductID { get; set; }
        public string ProductName { get; set; }
        public bool IsActive { get; set; }

    }
}
