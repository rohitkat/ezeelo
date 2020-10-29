using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class Franchise
    {
        public Franchise()
        {
            this.FranchiseLocations = new List<FranchiseLocation>();
            //------Added by mohit on 14-01-16----------------//
            this.FranchiseMenus = new List<FranchiseMenu>();
            //------------End--------------------------------//
            this.Shops = new List<Shop>();
            //------Added by Snehal on 03-02-16----------------//
            this.DynamicCategoryProducts = new List<DynamicCategoryProduct>();
            //------------End--------------------------------//
            //------Added by Pradnyakar on 11-02-16----------------//
            this.PremiumShopsPriorities = new List<PremiumShopsPriority>();
            //------------End  ----------------------------//
            //------Added by Snehal on 07-03-16----------------//
            this.BlockItemsLists = new List<BlockItemsList>();
            //------------End--------------------------------//
            //------Added by Ashish on 24-10-16----------------//
            this.Weekly_Seasona_Festival_Message = new List<Weekly_Seasona_Festival_Message>();
            //------------End--------------------------------//
            //------Added by Ashish on 11-11-16----------------//
            this.FranchiseOrderGMVTargets = new List<FranchiseOrderGMVTarget>();
            //------------End--------------------------------//

        }

        public int ID { get; set; }

        [Display(Name = "Business Detail Name")]
        public long BusinessDetailID { get; set; }

        [Display(Name = "PAN")]
        public string ServiceNumber { get; set; }

         [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

         [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper Mobile No.")]
        public string Mobile { get; set; }

        [RegularExpression(@"^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$", ErrorMessage = "Please enter proper email")]
        public string Email { get; set; }
        public string Landline { get; set; }
        public string FAX { get; set; }
        public string Address { get; set; }

        [Display(Name = "Pincode")]
        public Nullable<int> PincodeID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual BusinessDetail BusinessDetail { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual Pincode Pincode { get; set; }
        public virtual List<FranchiseLocation> FranchiseLocations { get; set; }

        /*14-03-2016
           * Prdnyakar Badge 
           * For Taxation Work
          */
        public virtual ICollection<FranchiseTaxDetail> FranchiseTaxDetails { get; set; }


        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<Shop> Shops { get; set; }
        //------Added by mohit on 14-01-16----------------//
        public virtual ICollection<FranchiseMenu> FranchiseMenus { get; set; }
        //------------End--------------------------------//
        //------Added by Snehal on 03-02-16----------------//
        public virtual List<DynamicCategoryProduct> DynamicCategoryProducts { get; set; }
        //------------End--------------------------------//

        //------Added by Pradnyakar on 11-02-16----------------//
        public virtual ICollection<PremiumShopsPriority> PremiumShopsPriorities { get; set; }
        //------------End  ----------------------------//

        //------Added by Snehal on 07-03-16----------------//
        public virtual ICollection<BlockItemsList> BlockItemsLists { get; set; }


        //------Added by Shobhit on 27-08-19----------------//
        public virtual ICollection<DeliveryBoy> DeliveryBoys { get; set; }
        //------------End--------------------------------//

        //------Added by Ashish on 30-08-16----------------//
        public virtual ICollection<HelpDeskDetails> HelpDeskDetails { get; set; }
        //------------End--------------------------------//

        //------Added by Ashish on 24-10-16----------------//
        public virtual ICollection<Weekly_Seasona_Festival_Message> Weekly_Seasona_Festival_Message { get; set; }
        //------------End--------------------------------//

        //------Added by Ashish on 11-11-16----------------//
        public virtual ICollection<FranchiseOrderGMVTarget> FranchiseOrderGMVTargets { get; set; }
        //------------End--------------------------------//
    }
}
