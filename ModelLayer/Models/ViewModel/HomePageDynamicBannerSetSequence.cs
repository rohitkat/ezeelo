using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class HomePageDynamicBannerSetSequence
    {
        public List<HomePageDynamicBannerSetSequenceViewModel> HomeDynamicBannersetsequence { get; set; }
    }
    public class HomePageDynamicBannerSetSequenceViewModel
    {
        public Int64 ID { get; set; }
        public Int64 FranchiseID { get; set; }
        public int SequenceOrder { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public string Imagename { get; set; }
        public string LinkUrl { get; set; }
        public string Tooltip { get; set; }
        public bool IsActive { get; set; }

        public Nullable<bool> IsBanner { get; set; }

        public Nullable<long> ShopId { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public Nullable<long> CreatedBy { get; set; }

        public Nullable<DateTime> ModifyDate { get; set; }

        public Nullable<long> ModifyBy { get; set; }



    }
}
