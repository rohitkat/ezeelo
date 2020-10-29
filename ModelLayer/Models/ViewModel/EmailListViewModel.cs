using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class EmailListViewModel
    {
        public long ID { get; set; }
        public string EmailID { get; set; }
        public bool Send { get; set; }
    }
}
