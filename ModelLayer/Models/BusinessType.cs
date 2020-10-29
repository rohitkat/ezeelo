using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class BusinessType
    {
        public BusinessType()
        {
            this.BusinessDetails = new List<BusinessDetail>();
            this.GandhibaghTransactions = new List<GandhibaghTransaction>();
            this.GandhibaghTransactions1 = new List<GandhibaghTransaction>();
            this.GandhibaghTransactions2 = new List<GandhibaghTransaction>();
            this.GoodwillOwnerPoints = new List<GoodwillOwnerPoint>();
            this.Notifications = new List<Notification>();
            this.Notifications1 = new List<Notification>();
            this.Offers = new List<Offer>();
            this.OwnerAdvertisements = new List<OwnerAdvertisement>();
            this.OwnerBanks = new List<OwnerBank>();
            this.Ratings = new List<Rating>();
            this.SchemeTypes = new List<SchemeType>();
            this.SEOs = new List<SEO>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual ICollection<BusinessDetail> BusinessDetails { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual ICollection<GandhibaghTransaction> GandhibaghTransactions { get; set; }
        public virtual ICollection<GandhibaghTransaction> GandhibaghTransactions1 { get; set; }
        public virtual ICollection<GandhibaghTransaction> GandhibaghTransactions2 { get; set; }
        public virtual ICollection<GoodwillOwnerPoint> GoodwillOwnerPoints { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Notification> Notifications1 { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
        public virtual ICollection<OwnerAdvertisement> OwnerAdvertisements { get; set; }
        public virtual ICollection<OwnerBank> OwnerBanks { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<SchemeType> SchemeTypes { get; set; }
        public virtual ICollection<SEO> SEOs { get; set; }
    }
}
