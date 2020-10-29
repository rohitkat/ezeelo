using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Models
{
    public class MLMWallet
    {
        public long ID { get; set; }
        public long UserLoginID { get; set; }
        public decimal Points { get; set; }
        public decimal Amount { get; set; }
        public bool IsMLMUser { get; set; }
        public long LastWalletTransactionID { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastModifyDate { get; set; }
        public long LastModifyBy { get; set; }
        [NotMapped]
        public string UsableWalletAmount { get; set; } //Yashaswi 7-9-2018
    }


    [Table("Mlm_User")]
    public class MLMUser
    {
        [Key]
        public long Id_Ref { get; set; }
        public long UserID { get; set; }
        public string Ref_Id { get; set; }
        public DateTime Join_date_ref { get; set; }
        public Nullable<bool> Status_ref { get; set; }
        public DateTime Activate_date_ref { get; set; }
        public string Refered_Id_ref { get; set; }
        public Nullable<bool> request { get; set; }
        public Nullable<bool> request_active { get; set; }
        public string ProfilePicture { get; set; }
        public int isroyaltyachiever { get; set; }
        public int islifestyleachiever { get; set; }

        public int DESIGNTAIONID { get; set; }
        public int CURRENTMONTHDESIGNTAIONID { get; set; }


    }

    [Table("Designation_Master")]
    public class DesignationMaster
    {
        [Key]

        public int Id { get; set; }
        public string Designation { get; set; }
        public int TotalMember { get; set; }

    }


    public class RefferalCodeGenerator
    {
        static Random rd = new Random();
        public string CreateCode(int stringLength, string allowedChars)
        {
            char[] chars = new char[stringLength];

            for (int i = 0; i < stringLength; i++)

            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }
    }
}
