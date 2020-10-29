using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    public  class PaymentData1
    {
        public int Id { get; set; }
        public string TxnId { get; set; }
        public long UserId { get; set; }
        public string City { get; set; }
        public int  FranchiesId { get; set; }
    }
}
