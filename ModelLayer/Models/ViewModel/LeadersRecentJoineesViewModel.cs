using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class LeadersRecentJoineesViewModel
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public int NetworkLevel { get; set; }
        public DateTime JoinDate { get; set; }
        public string ParantUser { get; set; }
        public int ParantUserLevel { get; set; }
    }
}
