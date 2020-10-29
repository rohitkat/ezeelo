using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class FranchiseMenuList
    {
        public List<FranchiseMenuViewModel> franchiseMenuList { get; set; }
    }
    public class FranchiseMenuViewModel
    {
        public Int64 ID { get; set; }
        public int SequenceOrder { get; set; }
        public string FranchiseCategoryName { get; set; }
        public int CategoryID { get; set; }
        public int FranchiseID { get; set; }
        public string CategoryName { get; set; }
        public string ImageName { get; set; }       
        public bool IsActive { get; set; }


        //Added by Tejaswee For Category Expiration
        public DateTime? ExpiryDate { get; set; }       
        public bool IsExpire { get; set; }
        //Added by Tejaswee For Category Expiration
    }
}
