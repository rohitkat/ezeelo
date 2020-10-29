using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class TerritoryByPincodeViewModel
    {
        public long StateID { get; set; }
        public string StateName { get; set; }

        public long DistrictID { get; set; }
        public string DistrictName { get; set; }

        public long CityID { get; set; }
        public string CityName { get; set; }

        public int PincodeID { get; set; }
        public string PincodeName { get; set; }

        //Sonali 27/8/2018
        public ICollection<Pincode> PincodeList { get; set; }
        public List<TeretoryViewModel> AreaList { get; set; }

        public List<TerritoryPincodeViewModel> Pincodes { get; set; }

    }
    //Sonali 27/8/2018
    public class TerritoryPincodeViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }
    }
}
