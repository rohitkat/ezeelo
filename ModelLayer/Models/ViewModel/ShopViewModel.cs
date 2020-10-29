using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ShopViewModel
    {
       
        public long ID { get; set; }
        public long BusinessDetailID { get; set; }
        public string Name { get; set; }
        public Nullable<int> FranchiseID { get; set; }

        public bool IsActive { get; set; }
        
        public int NonApproveProductCount { get; set; }
        public int ApproveProductCount { get; set; }
    }
}
