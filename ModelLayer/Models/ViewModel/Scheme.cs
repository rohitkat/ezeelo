using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class Scheme
    {
        //public int ID { get; set; }
        //public SchemeType lSchemeType { get; set; }
        //public TodayScheme lTodayScheme { get; set; }

        public int ID { get; set; }
        public string Name { get; set; }
        public decimal ValueInRs { get; set; }
        public Nullable<decimal> ApplicableOnPurchaseOfRs { get; set; }
        public string SchemeCode { get; set; }
        public int OwnerId { get; set; }

        public string OwnerName { get; set; }
        public string BussinessTypeName { get; set; }
        public int BusinessTypeID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }

        //Property of Today Scheme table
        public Nullable<System.DateTime> StartDatetime { get; set; }
        public Nullable<System.DateTime> EndDatetime { get; set; }
    }

    public class SchemeUsers
    {
        public long OwnerID { get; set; }
        public string UserName { get; set; }
    }
}
