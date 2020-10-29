using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
   public class MerchantTransactionDistributionLevel16
    {
        [Key]
        public long ID { get; set; }
        public long? MerchantTransactionId { get; set; }
        public long? OnlineMerchantId { get; set; }
        public long? MerchantId { get; set; }
        public decimal? RelationshipManagerPercentage { get; set; }
        public decimal? CompanyPercentage { get; set; }
        public decimal? Level0Percentage { get; set; }
        public decimal? UptoLevel6Percentage { get; set; }
        public decimal? Part5thPercentage { get; set; }
        public decimal? GST { get; set; }
        public bool? IsApplied { get; set; }
        public decimal? Commission { get; set; }
        public decimal? CalculatedCommission { get; set; }
        public decimal? Company { get; set; }
        public decimal? Part5th { get; set; }
        public decimal? Part16th { get; set; }
        public long? RelationshipManager_UserloginId { get; set; }
        public decimal? RelationshipManager { get; set; }
        public long? Level0_UserLoginID { get; set; }
        public decimal? Level0 { get; set; }
        public decimal? UptoLevel6 { get; set; }
        public decimal? UptoLevel16 { get; set; }
        public long? Level1_UserLoginID { get; set; }
        public decimal? Level1 { get; set; }
        public long? Level2_UserLoginID { get; set; }
        public decimal? Level2 { get; set; }
        public long? Level3_UserLoginID { get; set; }
        public decimal? Level3 { get; set; }
        public long? Level4_UserLoginID { get; set; }
        public decimal? Level4 { get; set; }
        public long? Level5_UserLoginID { get; set; }
        public decimal? Level5 { get; set; }
        public long? Level6_UserLoginID { get; set; }
        public decimal? Level6 { get; set; }
        public long? Level7_UserLoginID { get; set; }
        public decimal? Level7 { get; set; }
        public long? Level8_UserLoginID { get; set; }
        public decimal? Level8 { get; set; }
        public long? Level9_UserLoginID { get; set; }
        public decimal? Level9 { get; set; }
        public long? Level10_UserLoginID { get; set; }
        public decimal? Level10 { get; set; }
        public long? Level11_UserLoginID { get; set; }
        public decimal? Level11 { get; set; }
        public long? Level12_UserLoginID { get; set; }
        public decimal? Level12 { get; set; }
        public long? Level13_UserLoginID { get; set; }
        public decimal? Level13 { get; set; }
        public long? Level14_UserLoginID { get; set; }
        public decimal? Level14 { get; set; }
        public long? Level15_UserLoginID { get; set; }
        public decimal? Level15 { get; set; }
        public long? Level16_UserLoginID { get; set; }
        public decimal? Level16 { get; set; }
        public bool? IsPaid { get; set; }
        public bool? IsPayable { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }


    }
}
