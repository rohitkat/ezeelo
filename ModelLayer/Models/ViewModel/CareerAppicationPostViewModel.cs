using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class CareerAppicationPostViewModel
    {
        public Int64 ID { get; set; }
        public Int32 TotalCount { get; set; }
        public Int32 CareerID { get; set; }
        public string Jobtitle { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string TotalExpience { get; set; }
        public string CurrentCTC { get; set; }
        public string ExpectedCTC { get; set; }
        public string ResumePath { get; set; }
        public string Remarks { get; set; }
        public string serverPath { get; set; }

    }
}
