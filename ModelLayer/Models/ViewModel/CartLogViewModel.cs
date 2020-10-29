using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class CartLogViewModel
    {
        public long ID { get; set; }
        public long CartID { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }

        //- Extra Added.
        public string StatusVal { get; set; }
        public string CreateByUsername { get; set; }
        public string  CreateByPersonName { get; set; }
    }
}
