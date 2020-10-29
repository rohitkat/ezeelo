using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public partial class WarehouseFinancialTransaction
    {
        public long ID { get; set; }
        public long WarehouseID { get; set; }
        public long SupplierID { get; set; }
        public string AccountNumber { get; set; }
        public long InvoiceID { get; set; }
        public string ReceiptNumber { get; set; }
        public int TransactionTypeID { get; set; }
        public decimal TransactionAmount { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public string PaymentMode { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }       
        public virtual Invoice Invoice { get; set; }
        public virtual TransactionType TransactionType { get; set; }
    }
}
