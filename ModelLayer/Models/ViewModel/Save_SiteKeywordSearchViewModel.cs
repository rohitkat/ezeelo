using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
  public  class Save_SiteKeywordSearchViewModel
    {
      
        public long ID { get; set; }
        public long UserloginID { get; set; }
        public string Keyword { get; set; }
        public string Network_IP { get; set; }
        public string Device_ID { get; set; }
        public DateTime? Create_Date { get; set; }
        public bool? IsResult { get; set; }

        public string SearchResult { get; set; }

        public string DeviceType { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }

        public string CityName { get; set; }
        public string FranchiseName { get; set; }
        public string CategoryName { get; set; }

    }
}
