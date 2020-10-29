using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class WarehouseReturnListViewModel
    {
        public long SupplierID { get; set; } //Yashaswi 27/3/2018
        public long WarehouseReturnStockId { get; set; }
        public string WarehouseName { get; set; }
        public string SupplierName { get; set; }
        public string InvoiceCode { get; set; }
        public int TotalItems { get; set; }
        public decimal Amount { get; set; }
        public DateTime ReturnDate { get; set; }

        public bool IsApproved { get; set; }
    }
}
