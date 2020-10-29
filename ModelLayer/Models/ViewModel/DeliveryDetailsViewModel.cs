using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class DeliveryDetailsViewModel
    {
        public long ID { get; set; }

        public int DeliveryPartnerID { get; set; }

        public int FranchiseID { get; set; }

        public string Website { get; set; }

        public string Lattitude { get; set; }

        public string Longitude { get; set; }

        public string Address { get; set; }

        public string NearestLandmark { get; set; }

        public bool IsDeliveryOutSource { get; set; }

        public bool FreeHomeDelivery { get; set; }

        public decimal MinimumAmount { get; set; }
    }
}
