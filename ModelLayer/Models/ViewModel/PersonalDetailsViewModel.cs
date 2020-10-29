using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class PersonalDetailsViewModel
    {

        public long ID { get; set; }

        public int SalutationID { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string DOB { get; set; }

        public string Address { get; set; }

        public string PinCode { get; set; }

        public string Gender { get; set; }

    }
}
