using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class TrackCartReportViewModel
    {
        public long? ID { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public string ProductSize { get; set; }
        public string ProductColor { get; set; }
        public string Mobile { get; set; }
        public string Stage { get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }
        public decimal Price { get; set; }
        public decimal SaleRate { get; set; }
        public decimal LandingPrice { get; set; }

        public DateTime? Date { get; set; }
        public string City { get; set; }
        public int FranchiseID { get; set; }////added
        public string Franchises { get; set; }////added

       

        //- Added by Avi Verma. Date : 30-Aug-2016.
        //- Reason : CR : GB-CR-70 Abandoned Cart 
        public Nullable<long> CartID { get; set; }
        public Nullable<int> Qty { get; set; }

    }
}
