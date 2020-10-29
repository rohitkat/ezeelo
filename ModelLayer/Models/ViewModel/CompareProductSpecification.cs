using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class CompareProductSpecification
    {
        public long ParentSpecificationID { get; set; }
        public string ParentSpecificationValues { get; set; }
        List<ChildSpecification> childSpecification = new List<ChildSpecification>();
    }

    public class ChildSpecification
    {
        public long SpecificationID { get; set; }
        public string SpecificationValues { get; set; }
    }


}
