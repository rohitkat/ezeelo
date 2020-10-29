using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace ModelLayer.Models
{
    public class Warehouse
    {
        public long ID { get; set; }
        public long BusinessDetailID { get; set; }

        [Required(ErrorMessage = "Please enter warehouse name.")]
        [MaxLength(150, ErrorMessage = "Name cannot be longer than 150 characters.")]
        public string Name { get; set; }
        public string WarehouseCode { get; set; }

        [MaxLength(25, ErrorMessage = "GST Number cannot be longer than 25 characters.")]
        public string GSTNumber { get; set; }

        [MaxLength(25, ErrorMessage = "Service Number cannot be longer than 25 characters.")]
        public string ServiceNumber { get; set; }

        public int ServiceLevel;
        public long StateID { get; set; }
        public long CityID { get; set; }
        
        public string FSSILicenseNo { get; set; }  ///Added by Priti
        public string InsecticidesLicenseNo { get; set; }    ///Added by Priti
            public string PAN { get; set; } ///Added by Priti

        [Required(ErrorMessage = "Please enter pincode")]
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
        public string DeviceID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual BusinessDetail BusinessDetails { get; set; }
        public virtual IEnumerable<SelectListItem> lFranchises { get; set; }
        public int[] FranchiseIds { get; set; }
        public virtual Pincode Pincodes { get; set; }
        public List<FranchiseModel> franchiseList { get; set; }
        public List<WarehouseFranchise> warehouseFranchise { get; set; }
        //public virtual FranchiseViewModel franchiseViewModel { get; set; }

        //Yashaswi 5/4/20185
        public long? DistributorId { get; set; }
        //Yashaswi 5/4/20185
        public decimal? Margin { get; set; }

        //Yashaswi 4-1-2018 For EVW
        public string Entity { get; set; }
        
    }


    public class FranchiseModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
