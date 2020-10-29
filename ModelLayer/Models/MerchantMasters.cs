using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
    [Table("ServiceMaster")]
    public class ServiceMaster
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public long? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string NetworkIP { get; set; }

        //------Added by Shaili on 16-07-19----------------//
        public virtual ICollection<Merchant> Merchant { get; set; }
        //------------End--------------------------------//
    }

    [Table("CommissionMaster")]
    public class CommissionMaster
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]\d{0,9}(\.\d{1,2})?%?$", ErrorMessage = "Only numbers allowed with two decimal. eg 10,10.02")]
        [Range(0, 100, ErrorMessage = "Value must be between 0 to 100")]
        public decimal Commission { get; set; }
        public bool IsActive { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public long? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string NetworkIP { get; set; }

        //------Added by Shaili on 16-07-19----------------//
        public virtual ICollection<Merchant> Merchant { get; set; }
        //------------End--------------------------------//
    }

    [Table("ShopTimingMaster")]
    public class ShopTimingMaster
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string FromTime { get; set; }
        [Required]
        public string ToTime { get; set; }
        public bool IsActive { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public long? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string NetworkIP { get; set; }

        //------Added by Shaili on 16-07-19----------------//
        public virtual ICollection<Merchant> Merchant { get; set; }
        //------------End--------------------------------//
    }

    [Table("HolidayMaster")]
    public class HolidayMaster
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime? HolidayDate { get; set; }
        public bool IsActive { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public long? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string NetworkIP { get; set; }
    }

    [Table("ServiceIncomeMaster")]
    public class ServiceIncomeMaster
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]\d{0,9}(\.\d{1,2})?%?$", ErrorMessage = "Only number allowed with two decimal. eg 10,10.02")]
        [Range(0, 100, ErrorMessage = "Value must be between 0 to 100")]
        public decimal Company { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]\d{0,9}(\.\d{1,2})?%?$", ErrorMessage = "Only number allowed with two decimal. eg 10,10.02")]
        [Range(0, 100, ErrorMessage = "Value must be between 0 to 100")]
        public decimal RelationshipManager { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]\d{0,9}(\.\d{1,2})?%?$", ErrorMessage = "Only number allowed with two decimal. eg 10,10.02")]
        [Range(0, 100, ErrorMessage = "Value must be between 0 to 100")]
        public decimal UserLevel0 { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]\d{0,9}(\.\d{1,2})?%?$", ErrorMessage = "Only number allowed with two decimal. eg 10,10.02")]
        [Range(0, 100, ErrorMessage = "Value must be between 0 to 100")]
        public decimal UptoLevel6 { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]\d{0,9}(\.\d{1,2})?%?$", ErrorMessage = "Only number allowed with two decimal. eg 10,10.02")]
        [Range(0, 100, ErrorMessage = "Value must be between 0 to 100")]
        public decimal Part5th { get; set; }
        public bool IsActive { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public long? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string NetworkIP { get; set; }
    }

    [Table("ServiceIncomeMaster_Log")]
    public class ServiceIncomeMaster_Log
    {
        [Key]
        public long Id { get; set; }
        public decimal Company { get; set; }
        public decimal RelationshipManager { get; set; }
        public decimal UserLevel0 { get; set; }
        public decimal UptoLevel6 { get; set; }
        public decimal Part5th { get; set; }
        public bool IsActive { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public long? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string NetworkIP { get; set; }
    }
}
