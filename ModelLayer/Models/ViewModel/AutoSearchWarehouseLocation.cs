using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class AutoSearchWarehouseLocation
    {
       public long ID { get; set; }
       public string Name { get; set; }
       public string LocationShortName { get;set; }
       public string Status { get; set; }
       public int? Quantity { get; set; }
       public Nullable<long> InvoiceDetailID { get; set; }
    }
}
