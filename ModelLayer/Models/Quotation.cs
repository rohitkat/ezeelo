using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ModelLayer.Models
{
   public class Quotation
    {           
        public long ID { get; set; }
        public long RequestFromWarehouseID { get; set; }
        public string QuotationCode { get; set; }
        public System.DateTime QuotationRequestDate { get; set; }
        public System.DateTime ExpectedReplyDate { get; set; }
        public bool IsSent { get; set; }
        public string Remark { get; set; }
        public Boolean IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual ICollection<QuotationItemDetail> QuotationItemDetails { get; set; }       
        public virtual IEnumerable<SelectListItem> lSuppliers { get; set; }
        public int[] SupplierIds { get; set; }
        public List<SupplierModel> SupplierList { get; set; }
        public List<QuotationSupplierList> lQuotationSupplierList { get; set; }
    }

   public class SupplierModel
   {
       public int ID { get; set; }
       public string Name { get; set; }
       public bool IsSelected { get; set; }
   }
}
