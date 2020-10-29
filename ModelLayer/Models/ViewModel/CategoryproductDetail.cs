using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class CategoryproductDetail
    {
        public Int64 ID { get; set; }
        public string Name { get; set; }
        public int ApprovedCount { get; set; }
        public int NonApprovedCount { get; set; }
    }
}
