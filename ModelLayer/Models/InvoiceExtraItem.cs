using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public partial class InvoiceExtraItem
    {
        public long ID { get; set; }
        public long InvoiceID { get; set; }
        public long ProductID { get; set; }
        public long ProductVarientID { get; set; }
        public string ProductNickname { get; set; }
        public bool IsActive { get; set; }

        public virtual Invoice Invoice { get; set; }
    }
}
