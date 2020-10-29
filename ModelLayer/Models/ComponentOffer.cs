using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class ComponentOffer
    {
        public ComponentOffer()
        {
            this.StockComponentOffers = new List<StockComponentOffer>();
            this.StockComponentOfferDurations = new List<StockComponentOfferDuration>();
        }

        public int ID { get; set; }
        [Required(ErrorMessage = "Offer Name is Required")]
        public string ShortName { get; set; }
        public long ShopID { get; set; }
        public int ComponentID { get; set; }
        public decimal OfferInRs { get; set; }
        public decimal OfferInPercent { get; set; }
        [Required(ErrorMessage = "Offer Description is Required")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Component Component { get; set; }
        public virtual Shop Shop { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<StockComponentOffer> StockComponentOffers { get; set; }
        public virtual List<StockComponentOfferDuration> StockComponentOfferDurations { get; set; }
    }
}
