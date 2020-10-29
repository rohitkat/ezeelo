using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public class MerchantRating
    {        
        public long ID { get; set; }
        public long MerchantID { get; set; }
        public long CustomerID { get; set; }
        public decimal Rating { get; set; }
        public string Review { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsDisplay { get; set; }
        public virtual UserLogin UserLoginDetail { get; set; }
    }
}
