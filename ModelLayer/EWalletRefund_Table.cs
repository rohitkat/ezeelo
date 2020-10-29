using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer
{
    [Table("EwalletRefund_Table")]

    public class EWalletRefund_Table

    {
       

        public long ID { get; set; }

        public decimal RefundAmt { get; set; }

        public decimal RequsetAmt { get; set; }

        public string Comment { get; set; }

        public DateTime Date { get; set; }

        public long CustomerOrderId { get; set; }

        public int? Status { get; set; }

        public long UserLoginId { get; set; }

        public bool? Isactive { get; set; }

        public long? Createdby { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string NetworkID { get; set; }

        public string DeviceType { get; set; }

        public string DeviceID { get; set; }
        
        [NotMapped]

        public string ReturnStatus { get; set; }

    }
}
