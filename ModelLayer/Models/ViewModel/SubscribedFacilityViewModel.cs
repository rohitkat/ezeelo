using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class SubscribedFacilityViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public int BehaviorType { get; set; }
        public decimal FacilityValue { get; set; }
    }
}
