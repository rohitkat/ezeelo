using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class FranchiseDetailsViewModel
    {
        public int ID { get; set; }

        public long BusinessDetailsID { get; set; }

        public string ServiceNumber { get; set; }

        public string ContactPerson { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Landline { get; set; }
        public string FAX { get; set; }
        public string Address { get; set; }
        public string Pincode { get; set; }

    }
}
