using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    public class ERPBonusPayoutViewModel
    {
        public long EzeeMoneyPayoutId { get; set; }
        public decimal Amount { get; set; }
        public decimal ERP { get; set; }
        public bool IsPaid { get; set; }
        public bool ActiveUser { get; set; }
        public DateTime FreezeDate { get; set; }
        public long FreezeBy { get; set; }
        public DateTime? PaidDate { get; set; }
        public long? PaidBy { get; set; }
        public string NetworkIp { get; set; }
        public string VerficationCode { get; set; }
        public int Level { get; set; }
        public string ReferenceText { get; set; }
        public List<EzeeMoneyPayoutDetails> listEzeeMoneyPayoutDetails { get; set; }
        public List<PromotionalERPPayoutUserList> listPromotionalERPPayoutUserList { get; set; }
        public long? Level0PromotionalERPPayout { get; set; }
        public long? Level1PromotionalERPPayout { get; set; }
        public long City { get; set; }  //Yashaswi 06/12/2018 Promo ERP
    }
    public class PromotionalERPPayoutUserList
    {
        public long? ID { get; set; }
        public long? UserLoginId { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string PhoneNo { get; set; }
        public int? DelOrdCount { get; set; }
        public decimal? TotalOrdAmt { get; set; }
        public decimal? TotalRetailPoints { get; set; }
        public decimal? ERP { get; set; }
        public string Status { get; set; }
        public decimal? EzeeMoney { get; set; }
        public decimal? QRP { get; set; }
        public long EzeeMoneyPayoutID { get; set; }
        public decimal? PromotionalERP { get; set; }
        public decimal? PromotionalEzeeMoney { get; set; }
        public string  L1UserList { get; set; }
    }

    public class PromotionalERPPayoutReport
    {
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public decimal PromotionalERP { get; set; }
        public decimal EzeeMoney { get; set; }
    }
   

    public class L1UserList
    {
        public long? UserLoginId { get; set; }
        public string Name { get; set; }
    }

    public class UserExtraPay
    {
        public string ReferenceText { get; set; }
        public List<UserListPromo> list { get; set; }
    }

    public class UserListPromo
    {
        [Required]
        public long UserLoginId { get; set; }
        public string Name { get; set; }
        [Required]
        public decimal EzzeMoney { get; set; }
    }



}
