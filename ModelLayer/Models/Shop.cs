using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class Shop
    {
        public Shop()
        {
            this.BulkLogs = new List<BulkLog>();
            this.ComponentOffers = new List<ComponentOffer>();
            this.CustomerOrderDetails = new List<CustomerOrderDetail>();
            this.ShopComponentPrices = new List<ShopComponentPrice>();
            this.ShopMarkets = new List<ShopMarket>();
            this.ShopProducts = new List<ShopProduct>();
            this.ShopProductCharges = new List<ShopProductCharge>();
            this.TempShopProducts = new List<TempShopProduct>();
            this.ProprietoryProducts = new List<ProprietoryProduct>();
            this.ShopMenuPriorities = new List<ShopMenuPriority>();
            //------Added by Pradnyakar on 11-02-16----------------//
            this.PremiumShopsPriorities = new List<PremiumShopsPriority>();
            //------------End  ----------------------------//
        }

        public long ID { get; set; }
        public long BusinessDetailID { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }

        [RegularExpression(@" ^[-+]?([1-8]?\d(\.\d+)?|90(\.0+)?),\s*[-+]?(180(\.0+)?|((1[0-7]\d)|([1-9]?\d))(\.\d+)?)$", ErrorMessage = "Please enter proper Latitude")]
        public string Lattitude { get; set; }

        [RegularExpression(@" ^[-+]?([1-8]?\d(\.\d+)?|90(\.0+)?),\s*[-+]?(180(\.0+)?|((1[0-7]\d)|([1-9]?\d))(\.\d+)?)$", ErrorMessage = "Please enter proper Longitude")]
        public string Longitude { get; set; }
        //[StringLength(50, ErrorMessage = "Address must be less then 150.")]
        public string Address { get; set; }
        public string NearestLandmark { get; set; }
        public int PincodeID { get; set; }
        public Nullable<int> AreaID { get; set; }
        public Nullable<System.TimeSpan> OpeningTime { get; set; }
        public Nullable<System.TimeSpan> ClosingTime { get; set; }
        public string ContactPerson { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper Mobile No.")]
        public string Mobile { get; set; }
        [RegularExpression(@"^\d{3}([ -]\d\d|\d[ -]\d|\d\d[ -])\d{6}$", ErrorMessage = "Please enter proper Landline No. Like 0712-7585689")]
        public string Landline { get; set; }
        public string FAX { get; set; }
        public string VAT { get; set; }
        public string TIN { get; set; }
        public string PAN { get; set; }
        public string WeeklyOff { get; set; }
        public bool CurrentItSetup { get; set; }
        public bool InstitutionalMerchantPurchase { get; set; }
        public bool InstitutionalMerchantSale { get; set; }
        public bool NormalSale { get; set; }
        public bool IsDeliveryOutSource { get; set; }
        public bool IsFreeHomeDelivery { get; set; }
        public decimal MinimumAmountForFreeDelivery { get; set; }
        public Nullable<int> DeliveryPartnerId { get; set; }
        public Nullable<int> FranchiseID { get; set; }
        public bool IsLive { get; set; }
        public bool IsManageInventory { get; set; }
        public string SearchKeywords { get; set; }
        public bool IsAgreedOnReturnProduct { get; set; }
        public int ReturnDurationInDays { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        /*WelComeLetter*/
        public Nullable<System.DateTime> LetterDate { get; set; }
        public Nullable<long> SendBy { get; set; }
        /**/
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Area Area { get; set; }
        public virtual List<BulkLog> BulkLogs { get; set; }
        public virtual BusinessDetail BusinessDetail { get; set; }
        public virtual List<ComponentOffer> ComponentOffers { get; set; }
        public virtual List<CustomerOrderDetail> CustomerOrderDetails { get; set; }
        public virtual DeliveryPartner DeliveryPartner { get; set; }
        public virtual Franchise Franchise { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        /*WelComeLetter*/
        public virtual PersonalDetail PersonalDetail2 { get; set; }
        /**/
        public virtual Pincode Pincode { get; set; }
        public virtual List<ShopComponentPrice> ShopComponentPrices { get; set; }
        public virtual List<ShopMarket> ShopMarkets { get; set; }
        public virtual List<ShopProduct> ShopProducts { get; set; }
        public virtual List<ShopProductCharge> ShopProductCharges { get; set; }
        public virtual List<TempShopProduct> TempShopProducts { get; set; }
        public virtual List<ProprietoryProduct> ProprietoryProducts { get; set; }

        //------Added by Pradnyakar on 03-02-16----------------//
        public virtual ICollection<ShopMenuPriority> ShopMenuPriorities { get; set; }
        //------------End  ----------------------------//

        //------Added by Pradnyakar on 11-02-16----------------//
        public virtual ICollection<PremiumShopsPriority> PremiumShopsPriorities { get; set; }
        //------------End  ----------------------------//
    }
}
