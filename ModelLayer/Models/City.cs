using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class City
    {
        public City()
        {
            this.CoupenLists = new List<CoupenList>();
            this.DeliverySchedules = new List<DeliverySchedule>();
            this.Pincodes = new List<Pincode>();

            /*14-03-2016
             * Prdnyakar Badge 
             * For Taxation Work
             */
            this.FranchiseTaxDetails = new List<FranchiseTaxDetail>();
            //------Added by Ashish on 11-11-16----------------//
            this.FranchiseOrderGMVTargets = new List<FranchiseOrderGMVTarget>();
            //------------End--------------------------------//
           
        }

        public long ID { get; set; }
        public string Name { get; set; }
        public long DistrictID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual District District { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual ICollection<DeliverySchedule> DeliverySchedules { get; set; }
        public virtual ICollection<CoupenList> CoupenLists { get; set; }
        /*14-03-2016
            * Prdnyakar Badge 
            * For Taxation Work
            */
        public virtual ICollection<FranchiseTaxDetail> FranchiseTaxDetails { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual ICollection<Pincode> Pincodes { get; set; }

        //------Added by Ashish on 30-08-16----------------//
        public virtual ICollection<HelpDeskDetails> HelpDeskDetails { get; set; }
        //------------End--------------------------------//
        //------Added by Ashish on 11-11-16----------------//
        public virtual ICollection<FranchiseOrderGMVTarget> FranchiseOrderGMVTargets { get; set; }
        //------------End--------------------------------//
        //------Added by Shaili on 16-07-19----------------//
        public virtual ICollection<Merchant> Merchant { get; set; }
        //------------End--------------------------------//
    }
}
