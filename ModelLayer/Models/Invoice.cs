using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class Invoice
    {
        public Invoice()
        {
            this.InvoiceAttachments = new HashSet<InvoiceAttachment>();
            this.InvoiceDetails = new HashSet<InvoiceDetail>();
            this.InvoiceExtraItems = new HashSet<InvoiceExtraItem>();
        }

        public long ID { get; set; }
        public long PurchaseOrderID { get; set; }

        [Required(ErrorMessage = "Please select invoice date")]
        public System.DateTime InvoiceDate { get; set; }

         [Required(ErrorMessage = "Please enter Invoice Code")]
        public string InvoiceCode { get; set; }
        public Nullable<decimal> TotalDiscountAmount { get; set; }

        [Required(ErrorMessage = "Please enter order amount")]
        public decimal OrderAmount { get; set; }       
        public Nullable<decimal> ShippingCharge { get; set; }
        public Nullable<decimal> CustomDutyCharge { get; set; }
        public Nullable<decimal> OperatingCost { get; set; }
        public Nullable<decimal> AdditionalCost { get; set; }
        public decimal TotalAmount { get; set; }
        public string Remark { get; set; }
        public bool IsApproved { get; set; }
        public Nullable<long> ApprovedBy { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }

        public virtual ICollection<InvoiceAttachment> InvoiceAttachments { get; set; }
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual ICollection<InvoiceExtraItem> InvoiceExtraItems { get; set; }
        public virtual ICollection<WarehouseFinancialTransaction> WarehouseFinancialTransactions { get; set; }
    }
}
