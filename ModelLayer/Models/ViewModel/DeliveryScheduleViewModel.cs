using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{

    public class DeliveryScheduleViewModel
    {
        public string date { get; set; }

        public string time { get; set; }

        public string delScheduleId { get; set; }
    }
    public class APIDeliveryScheduleViewModel
    {
        public string ScheduleDate { get; set; }
        public string timeFrom { get; set; }
        public string timeTo { get; set; }
        public int ScheduleId { get; set; }
        public string ScheduleDisplay { get; set; }
    }

}
