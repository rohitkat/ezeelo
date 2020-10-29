using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class MerchantRegistrationAllDetailsViewModel
    {
        public long ID { get; set; }

        public PersonalDetailsViewModel PersonalViewModel { get; set; }

        public BussinessDetailsViewModel BussinessViewModel { get; set; }
    }
}
