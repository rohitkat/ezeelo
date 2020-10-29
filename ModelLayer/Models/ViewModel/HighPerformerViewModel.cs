using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class HighPerformerViewModel
    {
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int UsersCont { get; set; }
        public decimal money { get; set; }
        public int searchBy { get; set; }
    }
}
