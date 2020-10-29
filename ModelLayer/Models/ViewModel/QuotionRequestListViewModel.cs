using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class QuotionRequestListViewModel
    {
        public List<QuotationViewModel> lQuotationViewModel = new List<QuotationViewModel>();
        public List<SupplierModel> SupplierList { get; set; }
        public long SupplierID { get; set; }
        public string SupplierName { get; set; }
    }
}
