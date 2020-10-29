using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("PayubizTransaction")]
    public class PayubizTransaction
    {
        [Key]
        public long ID { get; set; }
        public string Payid { get; set; }
        public string Mode { get; set; }
        public string Status { get; set; }
        public string Key { get; set; }
        public string TxtnId { get; set; }
        public decimal? Amount { get; set; }
        public string CardCategory { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Net_Amount_Debit { get; set; }
        public DateTime? AddonDate { get; set; }
        public string FirstName { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public string Payment_source { get; set; }
        public string PG_TYPE { get; set; }
        public string Bank_ref_num { get; set; }
        public string Bankcode { get; set; }
        public string Name_on_card { get; set; }
        public string Error_Message { get; set; }
        public string CardNum { get; set; }
        public int? Device_type { get; set; }
        public long? UserLoginId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? OrderId { get; set; }
        public string responceString { get; set; }
        public string UnMappedStatus { get; set; }
    }

   
}
