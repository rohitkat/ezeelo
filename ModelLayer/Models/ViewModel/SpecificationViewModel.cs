using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class SpecificationViewModel
    {
        public string Name { get; set; }
        public Int32 ID { get; set; }
        public int level { get; set; }
        public bool isSelected { get; set; }
    }

    public class SpecificationList
    {
        public Int32 TwoLevelID { get; set; }
        public Int32 ThreeLevelID { get; set; }

        public List<SpecificationViewModel> pSpecificationViewModel { get; set; }
    }
}
