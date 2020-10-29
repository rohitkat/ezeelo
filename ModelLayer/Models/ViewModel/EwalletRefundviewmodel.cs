using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{

    public class EwalletRefundvm
    {
        public List<EwalletRefundviewmodel> ewallets { get; set; }
    }

    public class EwalletRefundviewmodel
    {
        public long ID { get; set; }
        public Decimal RefundAmt { get; set; }
        public Decimal RequsetAmt { get; set; }
        public string Comment { get; set; }
        public string Name { get; set; }
        public string OrderCode { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
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
        public string UserName { get; set; }
        public List<MlmWalletlog> MlmWalletlogViewModel { get; set; }

    }
}
