using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models.ViewModel
{
   public class InvoiceViewModel
    {
        public long InvoiceID { get; set; }
        public long PurchaseOrderID { get; set; }
        public string PurchaseOrderCode { get; set; }       

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
        public int TotalItems { get; set; }
        public string Remark { get; set; }
        public bool IsApproved { get; set; }
        public long ApprovedBy { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public long WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public long SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string SupplierGSTNumber { get; set; }
        public string SupplierFax { get; set; }
        public string SupplierEmail { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierContactPerson { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierMobile { get; set; }
        public string SupplierPincode { get; set; }
        public Nullable<long> ShopID { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public bool IsFulfillmentCenter { get; set; }
        public Nullable<decimal> AdditionalCharge { get; set; }//yashaswi 19/03/2018
        public Nullable<decimal> TranportationCharge { get; set; }//yashaswi 19/03/2018
        public Nullable<decimal> TotalReturnAmout { get; set; }//yashaswi 19/03/2018
        public long? ReturnId { get; set; }//yashaswi 20/03/2018
        public string Invoice_Attachment { get; set; } ///yashaswi 26/3/2018
        public string Invoice_AttachmentFileName { get; set; } ///yashaswi 26/3/2018
        public virtual ICollection<InvoiceAttachment> InvoiceAttachments { get; set; }
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual ICollection<InvoiceExtraItem> InvoiceExtraItems { get; set; }
        public List<InvoiceDetailViewModel> lInvoiceDetailViewModels { get; set; }
        public List<WarehouseReorderLevel> lWarehouseReorderLevels { get; set; }
        public List<WarehouseReturnStockDetailsViewModel> lWarehouseReturnStockDetailsViewModel { get; set; } //yashaswi 19/03/2018

        //Yashaswi Inventory Return 20-12-2018
        public long InvoiceId_Supp { get; set; }
        public string InvoiceCode_Supp { get; set; }
        public DateTime InvoiceDate_Supp { get; set; }
        public decimal OrderAmount_Supp { get; set; }
        public decimal TotalAmount_Supp { get; set; }
    }
}
