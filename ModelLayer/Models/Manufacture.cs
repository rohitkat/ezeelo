using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ModelLayer.Models
{
    public partial class Manufacture
    {

        public Manufacture()
        {
            //this.Products = new List<Product>();
            //this.TempProducts = new List<TempProduct>();
        }
        
        
        public int ID { get; set; }
        [Required (ErrorMessage = "Manufacture Name is Required ")]
        [StringLength(50,MinimumLength =1,ErrorMessage = "Manufacture Name must be between 1 - 50")]
        public string Name { get; set; }
        public int? BrandID { get; set; }  
        public string BrandName { get; set; }
        public int? BrandIDNew { get; set; }


        public string Description { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        //public virtual PersonalDetail PersonalDetail { get; set; }
        //public virtual PersonalDetail PersonalDetail1 { get; set; }
        //public virtual List<Product> Products { get; set; }
        //public virtual List<TempProduct>TempProducts { get; set; }

    }

}
