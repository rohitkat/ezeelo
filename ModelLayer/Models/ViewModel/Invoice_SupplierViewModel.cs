using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    public class Invoice_SupplierViewModel
    {
        public SelectList SupplierList { get; set; }
        public long SupplierId { get; set; }
        public List<Invoice> InvoiceList { get; set; }
        public List<InvoiceViewModel> obj_InvoiceViewModel { get; set; }

    }
}
