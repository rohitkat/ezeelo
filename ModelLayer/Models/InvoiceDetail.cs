using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class InvoiceDetail
    {
        public long ID { get; set; }
        public long InvoiceID { get; set; }
        public long PurchaseOrderDetailID { get; set; }
        public long ProductID { get; set; }
        public long ProductVarientID { get; set; }
        public bool IsExtraItem { get; set; }
        public decimal BuyRatePerUnit { get; set; }

       [Required (ErrorMessage="MRP is Requied!"),Range(1,100000)]
        public Nullable<decimal> MRP { get; set; }

        [Required(ErrorMessage = "Sale Rate is Requied!"), Range(1, 100000)]
        public Nullable<decimal> SaleRate { get; set; }
        public int ReceivedQuantity { get; set; }        
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> GSTInPer { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public string Remark { get; set; }
        public Nullable<DateTime> ExpiryDate { get; set; }
        public bool IsActive { get; set; }

        public virtual Invoice Invoice { get; set; }
    }
}
