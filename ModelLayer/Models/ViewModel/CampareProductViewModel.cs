using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class CampareProductViewModel
    {
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public string ThumImagePath { get; set; }
        public long CategoryID { get; set; }
        public string CategoryName { get; set; }

        List<CompareProductSpecification> compareProductSpecification = new List<CompareProductSpecification>();
    }
}
