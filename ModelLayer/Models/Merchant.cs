using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace ModelLayer.Models
{
    //Added by Shaili Khatri on 04-01-2019
    public partial class Merchant
    {
        public long Id { get; set; }
        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper Mobile No.")]
        public string LeaderContactNo { get; set; }
        public string FranchiseName { get; set; }
        public string GSTINNo { get; set; }
        public string PANNo { get; set; }
        public string Address { get; set; }
        public long City { get; set; }
        public long? District { get; set; }
        public long State { get; set; }
        public string Country { get; set; }
        public string Pincode { get; set; }
        public long? ShopTiming { get; set; }
        public string Status { get; set; }
        public string ContactPerson { get; set; }

        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper Contact No.")]
        public string ContactNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public int? ValidityPeriod { get; set; }
        public long? Category { get; set; }
        public long Comission { get; set; }
        public string GoogleMapLink { get; set; }
        public string SpecialRemark { get; set; }
        public bool TermCondition { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public DateTime? AcceptDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string OnlineMerchantHash { get; set; }
        public bool? IsOnline { get; set; }
        [NotMapped]
        public string PhotoImageURL { get; set; }
        [NotMapped]
        public List<MerchantBanner> BannerImageURL { get; set; }
        [NotMapped]
        public int NoOfReviews { get; set; }
        [NotMapped]
        public decimal Rating { get; set; }
        [NotMapped]
        public bool IsKYCComplete { get; set; }
        [NotMapped]
        public string PreviewURL { get; set; }
        [NotMapped]
        public string Remark { get; set; }
        [NotMapped]
        public string DashboardLink { get; set; }
        [NotMapped]
        public string Status_ { get; set; }
        [NotMapped]
        public decimal RegistrationFee { get; set; }
        [NotMapped]
        public decimal LeaderCommision { get; set; }
        [NotMapped]
        public decimal CasbackToBuyer { get; set; }

        public virtual City CityDetail { get; set; }
        public virtual State StateDetail { get; set; }
        public virtual ShopTimingMaster ShopTimingMasterDetail { get; set; }
        public virtual ServiceMaster ServiceMasterDetail { get; set; }
        public virtual CommissionMaster CommissionMasterDetail { get; set; }
    }

    [Table("MerchantProfile")]
    public  class MerchantProfile
    {
        [Key]
        public long Id { get; set; }
        public long MerchantId { get; set; }
        public string FranchiseName { get; set; }
        public string GSTINNo { get; set; }
        public string PANNo { get; set; }
        public string Address { get; set; }
        public long City { get; set; }
        public long State { get; set; }
        public string Pincode { get; set; }
        public long? ShopTiming { get; set; }
        public string Status { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public long? Category { get; set; }
        public long Comission { get; set; }
        public string GoogleMapLink { get; set; }
        public string SpecialRemark { get; set; }
        public DateTime CreateDate { get; set; }
        [NotMapped]
        public string CityName { get; set; }
    }

    public partial class MerchantHoliday
    {
        public long ID { get; set; }
        public long MerchantID { get; set; }
        public long HolidayID { get; set; }
    }
    [Table("MerchantHolidayUpdateRequest")]
    public class MerchantHolidayUpdateRequest
    {
        [Key]
        public long ID { get; set; }
        public long MerchantProfileID { get; set; }
        public long HolidayID { get; set; }
    }
    [Table("MerchantBanner")]
    public class MerchantBanner
    {
        [Key]
        public long ID { get; set; }
        public long MerchantID { get; set; }
        public string BannerPath { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
    [Table("MerchantBannerUpdateRequest")]
    public class MerchantBannerUpdateRequest
    {
        [Key]
        public long ID { get; set; }
        public long MerchantID { get; set; }
        public long? MerchantBannerID { get; set; }
        public string ShopPath { get; set; }
        public string BannerPath { get; set; }
        public bool IsActive { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        [NotMapped]
        public string ShopName { get; set; }
        [NotMapped]
        public string Remark { get; set; }
        [NotMapped]
        public string Cityname { get; set; }
    }
    [Table("MerchantsLogin")]
    public class MerchantsLogin
    {
        [Key]
        public long ID { get; set; }
        public long MerchantID { get; set; }
        [Required(ErrorMessage ="Please enter your registered contact number.")]
        public string UserID { get; set; }

        [StringLength(50, MinimumLength = 6, ErrorMessage = "Use 6 characters or more for your password")]
        public string Password { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords do not match.")]
        [NotMapped]
        public string ConfirmPassword { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string NetworkIP { get; set; }
        public string ModifyBy { get; set; }
    }

    [Table("MerchantTransaction")]
    public class MerchantTransaction
    {
        [Key]
        public long ID { get; set; }
        public string TransactionCode { get; set; }
        public long MerchantID { get; set; }
        public long UserLoginID { get; set; }
        [Required]
        public string Service { get; set; }

        [Required]
        public decimal BillAmount { get; set; }
        public decimal Comission { get; set; }
        public string Remark { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string NetworkIP { get; set; }

        [NotMapped]
        public string MobileNo { get; set; }
        [NotMapped]
        public string Name { get; set; }
    }

    [Table("MerchantCommonValues")]
    public class MerchantCommonValues
    {
        [Key]
        public int ID { get; set; }
        public string AdminMobileNo { get; set; }
        public string AdminEmailID { get; set; }
        public decimal MerchantRegistrationFee { get; set; }
        public decimal LeaderCommission { get; set; }
        public string MerchantTopupMoney { get; set; }
        public int TopupMin { get; set; }
        public int TopupMax { get; set; }
        public string CompanyDetail { get; set; }
        public string AccountNo { get; set; }
        public string IFSCCode { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string NetworkIP { get; set; }
    }

    [Table("MerchantTopupRecharge")]
    public  class MerchantTopupRecharge
    {
        [Key]
        public long ID { get; set; }
        public long MerchantID { get; set; }
        public decimal TopupAmount { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }

    [Table("MerchantTopupRechargeLog")]
    public class MerchantTopupRechargeLog
    {
        [Key]
        public long ID { get; set; }
        public long MerchantID { get; set; }
        public decimal TopupAmount { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
        public string Mode { get; set; }
        [Column("TransactionID/CheckNo")]
        public string TransactionID_CheckNo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Remark { get; set; }

        [NotMapped]
        public string CompanyDetail { get; set; }
        [NotMapped]
        public string AccountNo { get; set; }
        [NotMapped]
        public string IFSCCode { get; set; }
        [NotMapped]
        public List<SelectListItem> myList { get; set; }
        [NotMapped]
        public string recharge { get; set; }
        [NotMapped]
        public List<RechargeList> list { get; set; }
        [NotMapped]
        public decimal? PendingTransaction { get; set; }
    }

    public class RechargeList
    {
        public string Date { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }

    [Table("MerchantDetails")]
    public class MerchantDetails
    {
        [Key]
        public long Id { get; set; }
        public long MerchantId { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]\d{0,9}(\.\d{1,2})?%?$", ErrorMessage = "Only number allowed with two decimal. eg 10,10.02")]
        [Range(0, 100, ErrorMessage = "Value must be between 0 to 100")]
        public decimal RMCommission { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]\d{0,9}(\.\d{1,2})?%?$", ErrorMessage = "Only number allowed with two decimal. eg 10,10.02")]
        [Range(0, 100, ErrorMessage = "Value must be between 0 to 100")]
        public decimal Company { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]\d{0,9}(\.\d{1,2})?%?$", ErrorMessage = "Only number allowed with two decimal. eg 10,10.02")]
        [Range(0, 100, ErrorMessage = "Value must be between 0 to 100")]
        public decimal Level0 { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]\d{0,9}(\.\d{1,2})?%?$", ErrorMessage = "Only number allowed with two decimal. eg 10,10.02")]
        [Range(0, 100, ErrorMessage = "Value must be between 0 to 100")]
        public decimal UptoLevel6 { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]\d{0,9}(\.\d{1,2})?%?$", ErrorMessage = "Only number allowed with two decimal. eg 10,10.02")]
        [Range(0, 100, ErrorMessage = "Value must be between 0 to 100")]
        public decimal GST { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]\d{0,9}(\.\d{1,2})?%?$", ErrorMessage = "Only number allowed with two decimal. eg 10,10.02")]
        [Range(0, 100, ErrorMessage = "Value must be between 0 to 100")]
        public decimal Part5th { get; set; }
        public bool IsGSTApply { get; set; }
        public decimal RegistrationFee { get; set; }
        public decimal LeaderCommisiion { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        [NotMapped]
        public string ShopName { get; set; }
    }


    [Table("MerchantTransactionDistribution")]
    public class MerchantTransactionDistribution
    {
        [Key]
        public long ID { get; set; }
        public long MerchantTransactionId { get; set; }
        public long MerchantId { get; set; }
        public decimal RelationshipManagerPercentage { get; set; }
        public decimal CompanyPercentage { get; set; }
        public decimal Level0Percentage { get; set; }
        public decimal UptoLevel6Percentage { get; set; }
        public decimal Part5thPercentage { get; set; }
        public decimal GST { get; set; }
        public bool IsApplied { get; set; }
        public decimal Commission { get; set; }
        public decimal CalculatedCommission { get; set; }
        public decimal Company { get; set; }
        public decimal Part5th { get; set; }
        public long RelationshipManager_UserloginId { get; set; }
        public decimal RelationshipManager { get; set; }
        public long Level0_UserLoginID { get; set; }
        public decimal Level0 { get; set; }
        public decimal UptoLevel6 { get; set; }
        public long Level1_UserLoginID { get; set; }
        public decimal Level1 { get; set; }
        public long Level2_UserLoginID { get; set; }
        public decimal Level2 { get; set; }
        public long Level3_UserLoginID { get; set; }
        public decimal Level3 { get; set; }
        public long Level4_UserLoginID { get; set; }
        public decimal Level4 { get; set; }
        public long Level5_UserLoginID { get; set; }
        public decimal Level5 { get; set; }
        public long Level6_UserLoginID { get; set; }
        public decimal Level6 { get; set; }
        public bool IsPaid { get; set; }
        public bool IsPayable { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
    
    [Table("MerchantPayout")]
    public class MerchantPayout
    {
        public long ID { get; set; }
        public long RefEzeeMoneyPayoutId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal RMERP { get; set; }
        public decimal L0ERP { get; set; }
        public decimal UptoL6ERP { get; set; }
        public decimal UnpaidERP { get; set; }
        public decimal RMInactivePoints { get; set; }
        public decimal PrevoiusUnpaidERP { get; set; }
        public decimal UptoL6InactivePoints { get; set; }
        public DateTime PaidDate { get; set; }
    }

    [Table("MerchantPayoutDetail")]
    public class MerchantPayoutDetail
    {
        public long ID { get; set; }
        public long MerchantPayoutId { get; set; }
        public long UserLoginId { get; set; }
        public decimal RMERP { get; set; }
        public decimal L0ERP { get; set; }
        public decimal UptoL6ERP { get; set; }
        public decimal RMInactivePoint { get; set; }
        public decimal UptoL6InactivePoint { get; set; }
        public decimal UnpaidERP { get; set; }
        public decimal PreviousUnpaidERP { get; set; }
        public decimal TotalERP { get; set; }
        public bool Status { get; set; }
        public bool IsPaid { get; set; }
        public bool RefMerchantPayoutId { get; set; }
    }

    [Table("MerchantSelfCBPoints")]
    public class MerchantSelfCBPoints
    {
        [Key]
        public long Id { get; set; }
        public long CityId { get; set; }
        public decimal CBPoints { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }
        [NotMapped]
        public string City { get; set; }
    }

    [Table("MerchantNotifications")]
    public class MerchantNotifications
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public int Type { get; set; }
        public DateTime CreateDate { get; set; }
        [NotMapped]
        public string Date { get; set; }
    }

    [Table("MerchantTransactionRequest")]
    public class MerchantTransactionRequest
    {
        [Key]
        public long Id { get; set; }
        public long MerchantId { get; set; }
        public long UserLoginId { get; set; }
        public string TransactionId { get; set; }
        public string RefTransactionId { get; set; }
        public decimal BillAmount { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Remark { get; set; }

        [NotMapped]
        public string UserName { get; set; }
        [ForeignKey("UserLoginId")]
        public virtual UserLogin User { get; set; }
    }
}
