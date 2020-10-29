using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class QuotationSupplierListViewModel
    {
       public long QuotationID { get; set; }
       public string QuotationCode { get; set; }
       public long SupplierID { get; set; }
       public string SupplierName { get; set; }
       public System.DateTime QuotationRequestDate { get; set; }
       public System.DateTime ExpectedReplyDate { get; set; }
       public int TotalItems { get; set; }
       public bool IsSent { get; set; }
       public bool IsReplied { get; set; }
       public Nullable<System.DateTime> QuotationReplyDate { get; set; }

    }
}
