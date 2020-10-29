using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class PayubizTransactionViewModel
    {
        public string mihpayid { get; set; }
        public string Mode { get; set; }
        public string Status { get; set; }
        public string unmappedstatus { get; set; }
        public string Key { get; set; }
        public string TxtnId { get; set; }
        public decimal Amount { get; set; }
        public string CardCategory { get; set; }
        public decimal Discount { get; set; }
        public decimal Net_Amount_Debit { get; set; }
        public DateTime addedon { get; set; }
        public string productinfo { get; set; }
        public string FirstName { get; set; }
        public string lastname { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string Country { get; set; }
        public string zipcode { get; set; }
        public string Email { get; set; }
        public string phone { get; set; }
        public string udf1 { get; set; }
        public string udf2 { get; set; }
        public string udf3 { get; set; }
        public string udf4 { get; set; }
        public string udf5 { get; set; }
        public string udf6 { get; set; }
        public string udf7 { get; set; }
        public string udf8 { get; set; }
        public string udf9 { get; set; }
        public string udf10 { get; set; }
        public string Hash { get; set; }
        public string field1 { get; set; }
        public string field2 { get; set; }
        public string field3 { get; set; }
        public string field4 { get; set; }
        public string field5 { get; set; }
        public string field6 { get; set; }
        public string field7 { get; set; }
        public string field8 { get; set; }
        public string field9 { get; set; }
        public string Payment_source { get; set; }
        public string PG_TYPE { get; set; }
        public string Bank_ref_num { get; set; }
        public string Bankcode { get; set; }
        public string Name_on_card { get; set; }
        public string Error_Message { get; set; }
        public string CardNum { get; set; }
        public string cardhash { get; set; }
        public int Device_type { get; set; }
    }
}
