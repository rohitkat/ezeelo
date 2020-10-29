using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class PurchaseCompleteViewModel
    {
        public ViewCustomerOrderViewModel CustomerOrder { get; set; }
        public RelatedProductsViewModel FrequentlyProducts { get; set; }

        //Added by Tejaswee
        //Set tax wise total (Ex: service tax=20, cess=10, lbt=40  )
        public List<CalculatedTaxList> lCalculatedTaxList { get; set; }
    }
}
