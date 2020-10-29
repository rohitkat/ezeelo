using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
    public class DashboardViewModel
    {
        public long Referrals { get; set; }
        public decimal Withdrawn { get; set; }
        public double ERP { get; set; }
        public double Inactive_Points { get; set; }
        public decimal Pending_EzeeMoney { get; set; }
        public decimal Payout_Requested { get; set; }
        public long TOTAL_MEMBERS { get; set; }
        public decimal EZEE_MONEY { get; set; }
        public long QUALIFYING_RETAIL_POINTS { get; set; }
        public int DAYS_LEFT { get; set; }
        public DateTime CYCLE_START_DATE { get; set; }
        public DateTime CYCLE_LAST_DATE { get; set; }
        public long MY_PURCHASES { get; set; }
        public decimal EXPECTED_ERP { get; set; }
        public decimal RP_ON_MY_PURCHASE { get; set; }
        public long INACTIVE_MEMBERS { get; set; }
        public bool isUserActive { get; set; }
        public InviteUser objInviteUser { get; set; }
        public List<LeadersRecentJoineesViewModel> listRecentJoinees { get; set; }
        public HighPerformer objHighPerformer { get; set; }
        public RefferalByFilter objRefferalByFilter { get; set; }
        public List<HighPerformerViewModel> TopRecruiterList { get; set; }//Sonali Added for Api_20-11-2018
        public List<HighPerformerViewModel> TopBuyersList { get; set; }//Sonali Added for Api_20-11-2018
        public List<HighPerformerViewModel> TopEarnersList { get; set; }//Sonali Added for Api_20-11-2018
        public long Referrals_5Month { get; set; }//Sonali Added for Api_20-11-2018
        public long Withdrawn_5Month { get; set; }//Sonali Added for Api_20-11-2018
        public long ERP_5Month { get; set; }//Sonali Added for Api_20-11-2018

        public decimal CasbackPoints { get; set; }
        public decimal CasbackEzeeMoney { get; set; }
        public string UserDesignation { get; set; }

        public List<NetworkUserViewModel> userlist { get; set; } 

    }

    public class HighPerformer
    {
        public int searchId { get; set; }
        public SelectList SearchParameter { get; set; }
        public List<HighPerformerViewModel> listHighPerformerViewModel { get; set; }
    }

    public class RefferalByFilter
    {
        public int searchId { get; set; }
        public SelectList SearchParameter { get; set; }
        public long Count { get; set; }
    }

    public class InviteUser
    {
        [Required(ErrorMessage = "Please Enter Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please Enter the Email Id")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                        @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                        @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                        ErrorMessage = "Email is not valid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please Enter Mobile No.")]
        [RegularExpression(@"^([5-9]{1}[0-9]{9})$", ErrorMessage = "Mobile is not valid")]
        public string MobileNo { get; set; }
        [Required(ErrorMessage = "Please Enter Message")]
        public string Message { get; set; }
        [NotMapped]//Added by Sonali for Api_17-11-2018
        public long UserLoginId { get; set; }//Added by Sonali for Api_17-11-2018
    }

    public class ERPValue
    {
        public decimal ERP { get; set; }
    }
}
