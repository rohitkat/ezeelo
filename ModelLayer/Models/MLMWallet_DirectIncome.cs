using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("MLMWallet_DirectIncome")]
    public class MLMWallet_DirectIncome
    {
        [Key]
        public long ID { get; set; }
        public long MLMWalletTransactionId { get; set; }
        public double TransactionPoints { get; set; }
        public double CurrentLevel { get; set; }
        public long CurrentLevel_UserLoginId { get; set; }
        public bool? CurrentLevel_IsPaid { get; set; }
        public DateTime? CurrentLevel_PayoutDate { get; set; }
        public double UpLine1 { get; set; }
        public long UpLine1_UserLoginId { get; set; }
        public bool? UpLine1_IsPaid { get; set; }
        public DateTime? UpLine1_PayoutDate { get; set; }
        public double UpLine2 { get; set; }
        public long UpLine2_UserLoginId { get; set; }
        public bool? UpLine2_IsPaid { get; set; }
        public DateTime? UpLine2_PayoutDate { get; set; }
        public double UpLine3 { get; set; }
        public long UpLine3_UserLoginId { get; set; }
        public bool? UpLine3_IsPaid { get; set; }
        public DateTime? UpLine3_PayoutDate { get; set; }
        public double UpLine4 { get; set; }
        public long UpLine4_UserLoginId { get; set; }
        public bool? UpLine4_IsPaid { get; set; }
        public DateTime? UpLine4_PayoutDate { get; set; }
        public double UpLine5 { get; set; }
        public long UpLine5_UserLoginId { get; set; }
        public bool? UpLine5_IsPaid { get; set; }
        public DateTime? UpLine5_PayoutDate { get; set; }
        public double UpLine6 { get; set; }
        public long UpLine6_UserLoginId { get; set; }
        public bool? UpLine6_IsPaid { get; set; }
        public DateTime? UpLine6_PayoutDate { get; set; }

        public double UpLine7 { get; set; }
        public double UpLineR7 { get; set; }
        public long UpLine7_UserLoginId { get; set; }
        public bool? UpLine7_IsPaid { get; set; }
        public DateTime? UpLine7_PayoutDate { get; set; }

        public double UpLine8 { get; set; }
        public double UpLineR8 { get; set; }
        public long UpLine8_UserLoginId { get; set; }
        public bool? UpLine8_IsPaid { get; set; }
        public DateTime? UpLine8_PayoutDate { get; set; }

        public double UpLine9 { get; set; }
        public double UpLineR9 { get; set; }
        public long UpLine9_UserLoginId { get; set; }
        public bool? UpLine9_IsPaid { get; set; }
        public DateTime? UpLine9_PayoutDate { get; set; }

        public double UpLine10 { get; set; }
        public double UpLineR10 { get; set; }
        public long UpLine10_UserLoginId { get; set; }
        public bool? UpLine10_IsPaid { get; set; }
        public DateTime? UpLine10_PayoutDate { get; set; }

        public double UpLine11 { get; set; }
        public double UpLineR11 { get; set; }
        public long UpLine11_UserLoginId { get; set; }
        public bool? UpLine11_IsPaid { get; set; }
        public DateTime? UpLine11_PayoutDate { get; set; }


        public double UpLine12 { get; set; }
        public double UpLineR12 { get; set; }
        public long UpLine12_UserLoginId { get; set; }
        public bool? UpLine12_IsPaid { get; set; }
        public DateTime? UpLine12_PayoutDate { get; set; }

        public double UpLine13 { get; set; }
        public double UpLineR13 { get; set; }
        public long UpLine13_UserLoginId { get; set; }
        public bool? UpLine13_IsPaid { get; set; }
        public DateTime? UpLine13_PayoutDate { get; set; }

        public double UpLine14 { get; set; }
        public double UpLineR14 { get; set; }
        public long UpLine14_UserLoginId { get; set; }
        public bool? UpLine14_IsPaid { get; set; }
        public DateTime? UpLine14_PayoutDate { get; set; }

        public double UpLine15 { get; set; }
        public double UpLineR15 { get; set; }
        public long UpLine15_UserLoginId { get; set; }
        public bool? UpLine15_IsPaid { get; set; }
        public DateTime? UpLine15_PayoutDate { get; set; }

        public double UpLine16 { get; set; }
        public double UpLineR16 { get; set; }
        public long UpLine16_UserLoginId { get; set; }
        public bool? UpLine16_IsPaid { get; set; }
        public DateTime? UpLine16_PayoutDate { get; set; }

        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public long? ModifyBy { get; set; }
    }
}
