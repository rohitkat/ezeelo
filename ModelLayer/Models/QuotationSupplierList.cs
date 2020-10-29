using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
  public class QuotationSupplierList
    {
      public long ID { get; set; }
      public long QuotationID { get; set; }
      public long SupplierID { get; set; }
      public bool IsReplied { get; set; }
      public Nullable<System.DateTime> QuotationReplyDate { get; set; }
      public Nullable<decimal> Amount { get; set; }
      public Nullable<decimal> GSTAmount { get; set; }
      public Nullable<decimal> ShippingCharge { get; set; }
      public Nullable<decimal> AdditionalCost { get; set; }
      public Nullable<decimal> TotalAmount { get; set; }
      public string Remark { get; set; }
      public bool IsActive { get; set; }
      public Nullable<long> RepliedBy { get; set; }
      public Nullable<long> ModifyBy { get; set; }
      public Nullable<System.DateTime> ModifyDate { get; set; }
    }
}
