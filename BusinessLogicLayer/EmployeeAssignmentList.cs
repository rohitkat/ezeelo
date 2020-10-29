using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public partial class EmployeeAssignmentList
    {
       
        public long ID { get; set; }
        public string OrderCode { get; set; }
        public string ShopOrderCode { get; set; }
        public Nullable<int> GodownCode { get; set; }
        public string EmployeeCode { get; set; }
        public Nullable<int> OrderStatus { get; set; }
        public string Name { get; set; }
        public string FromAddress { get; set; }
        public string DeliveredType { get; set; }
        public string DeliveryType { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string DeliverySchedule { get; set; }
        public Nullable<System.DateTime> DeliveredTime { get; set; }

        public string ToAddress { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public string X { get; set; }
        public string Y { get; set; }

        public string PrimaryMobile { get; set; }
        public string SecondaryMobile { get; set; }
        public string PaymentMode { get; set; }
        public decimal Amount { get; set; }
        public List<AssignmentReceiveDetail> MerchantCopy { get; set; }
        public List<AssignmentDeliveryDetail> CustomerCopy { get; set; }
        public List<AssignmentTaxOnOrder> TaxOnOrder { get; set; }

       
    }
}
