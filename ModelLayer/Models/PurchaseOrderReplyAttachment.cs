using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
   public class PurchaseOrderReplyAttachment
    {
        public long ID { get; set; }
        public long PurchaseOrderReplyID { get; set; }
        public string FileName { get; set; }
        public string Extention { get; set; }
        public bool IsActive { get; set; }
        public virtual PurchaseOrderReply PurchaseOrderReply { get; set; }
    }
}
