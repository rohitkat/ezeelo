using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
  public class BulkProductViewModel
    {
        public int BulkLogID { get; set; }
        public int ExcelRowID { get; set; }
        public long TempProductID { get; set; }
        public string ExcelProductName { get; set; }
        public string Result { get; set; }
        public string ResultDescription { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int WeightInGram { get; set; }
        public int LengthInCm { get; set; }
        public bool isDescUploaded { get; set; }
        public int BreadthInCm { get; set; }
        public int HeightInCm { get; set; }
        public string Description { get; set; }
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public Nullable<int> TotalSuccess { get; set; }
        public Nullable<int> TotalFail { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }     
        public string SearchKeyword { get; set; }

    }
}
