using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class TrackSearchReportViewModel
    {
        public long? ID { get; set; }
        public string UserName { get; set; }
        public string CategoryName { get; set; }
        public string ShopName { get; set; }
        public string ProductName { get; set; }
        public DateTime? Date { get; set; }
        public string City { get; set; }
        public int FranchiseID { get; set; }////added
        public string Franchises { get; set; }////added

    }
}
