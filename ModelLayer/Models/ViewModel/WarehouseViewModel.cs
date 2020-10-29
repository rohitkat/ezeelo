using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    public class WarehouseViewModel
    {
        public long ID { get; set; }
        public long BusinessDetailID { get; set; }

        public string Name { get; set; }
        public string WarehouseCode { get; set; }

        public string GSTNumber { get; set; }
        public string ServiceNumber { get; set; }

        public int ServiceLevel;
        public long StateID { get; set; }
        public long CityID { get; set; }
        public string CityName { get; set; }
        [Required(ErrorMessage = "Pincode is required")]
        public int PincodeID { get; set; }
        public string NearbyTransport { get; set; }
        public string Measurement { get; set; }
        public string FloorSpace { get; set; }
        public string Volume { get; set; }
        public string CustomEntry { get; set; }
        public string CustomExit { get; set; }
        public bool IsFulfillmentCenter { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string FSSILicenseNo { get; set; }  ///Added by Priti
        public string InsecticidesLicenseNo { get; set; }    ///Added by Priti
        public string StateName { get; set; }         ///Added by Priti  
        public string PAN { get; set; }    ///Added by Priti  
        public string DeviceID { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please Enter Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual BusinessDetail BusinessDetails { get; set; }
        public virtual Warehouse lWarehouses { get; set; }
        public string ContactPerson { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Landline1 { get; set; }
        public string Landline2 { get; set; }
        public string FAX { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public virtual UserLogin UserLogins { get; set; }
        //public string Password { get; set; }

        public int[] FranchiseIds { get; set; }
        public virtual Pincode Pincodes { get; set; }
        public List<FranchiseModel> franchiseList { get; set; }
        public List<WarehouseFranchise> warehouseFranchise { get; set; }
        //Yashaswi 5/4/2018       
        public int Franchise { get; set; }
        //Yashaswi 5/4/2018
        public int? FC_Count { get; set; }
        //Yashaswi 5/4/20185
        public decimal? Margin { get; set; }
        //Yashaswi 5/4/20185       
        public List<Warehouse> FCList { get; set; }
        //Yashaswi 4-1-2018 For EVW
        public string Entity { get; set; }


        public SelectList CityList { get; set; }
    }

    public class EVWViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string GSTNo { get; set; }
        public string StateName { get; set; }
        public string Pincode { get; set; }
        public List<EVWV_List> ListForEVW { get; set; }
    }

    public class EVWV_List
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
