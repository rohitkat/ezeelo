using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
   public partial class PurchaseOrderReply
    {
       public PurchaseOrderReply()
        {
            this.PurchaseOrderReplyDetails = new HashSet<PurchaseOrderReplyDetail>();          
        }
    
        public long ID { get; set; }
        public long PurchaseOrderID { get; set; }
        public string InvoiceCode { get; set; }                
        public Nullable<System.DateTime> ReplyDate { get; set; }

        [Required(ErrorMessage = "Please Select Delivery Date")]
        public System.DateTime DeliveryDateTime { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal ShippingCharge { get; set; }
        public decimal CustomDutyCharge { get; set; }
        public decimal OperatingCost { get; set; }
        public decimal AdditionalCost { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalAmount { get; set; }      
        public string Remark { get; set; }
        public Boolean IsReplied { get; set; }
        public long RepliedBy { get; set; }
        public Nullable<System.DateTime> DispatchDate { get; set; }
        public string DriverName { get; set; }
        public string DriverMobileNumber { get; set; }
        public string DriverLicenceNumber { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleType { get; set; }
        public string LogisticCompanyName { get; set; }
        public string LogisticCompanyAddress { get; set; }
        public string LogisticContactPerson { get; set; }
        public string LogisticContactNumber { get; set; }
        public string EWayBillNumber { get; set; }
        public string TrackingNumber { get; set; }
        public Boolean IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual ICollection<PurchaseOrderReplyDetail> PurchaseOrderReplyDetails { get; set; }      
      
    }
}
