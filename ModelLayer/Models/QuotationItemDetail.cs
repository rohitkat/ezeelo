using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
   public class QuotationItemDetail
    {
       public long ID { get; set; }
       public long QuotationID { get; set; }
       public long ProductID { get; set; }
       public string ProductNickname { get; set; }
       public long ProductVarientID { get; set; }
       public int Quantity { get; set; }
       public string Remark { get; set; }
    }
}
