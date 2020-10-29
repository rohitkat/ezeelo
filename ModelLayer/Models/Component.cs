using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class Component
    {
        public Component()
        {
            this.ComponentOffers = new List<ComponentOffer>();
            this.ShopComponentPrices = new List<ShopComponentPrice>();
            this.ShopComponentPrices1 = new List<ShopComponentPrice>();
            this.StockComponents = new List<StockComponent>();
            this.TempStockComponents = new List<TempStockComponent>();
        }

        public int ID { get; set; }
        [Required(ErrorMessage = "Title is Required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Componant Name must be between 5 to 50 Chatacter")]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual List<ComponentOffer> ComponentOffers { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<ShopComponentPrice> ShopComponentPrices { get; set; }
        public virtual List<ShopComponentPrice> ShopComponentPrices1 { get; set; }
        public virtual List<StockComponent> StockComponents { get; set; }
        public virtual List<TempStockComponent> TempStockComponents { get; set; }
    }
}
