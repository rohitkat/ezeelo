using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class MerchantApprovedViewModel
    {
        public Int64 ID { get; set; }
        public Int64 UserLoginID { get; set; }
        public string BusinessTypePrefix { get; set; }
        public string Name { get; set; }
        public Int64 OwnerId { get; set; }
        public string ShopName { get; set; }
        public string mobile { get; set; }
        public string Email { get; set; }
        public bool IsLock { get; set; }
        public string PlanName { get; set; }
        public int NoOfProducttAllowed { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime StartDate { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime EndDate { get; set; }
    }
}
