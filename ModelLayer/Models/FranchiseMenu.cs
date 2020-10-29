using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class FranchiseMenu
    {
        public long ID { get; set; }
        public int FranchiseID { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        public string CategoryName { get; set; }
        public int CategoryID { get; set; }
        public int Level { get; set; }
        public Nullable<int> SequenceOrder { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string ImageName { get; set; }
        public string Remarks { get; set; }
        public virtual Category Category { get; set; }
        public virtual Franchise Franchise { get; set; }

        /*======== Added by Tejaswee for Setting expiration date to some special Categories like Festival category ========*/
        public bool IsExpire { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }

        /*======== Added by Tejaswee for Setting expiration date to some special Categories like Festival category ========*/
    }
}
