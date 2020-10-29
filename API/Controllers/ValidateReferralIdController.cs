using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class ValidateReferralIdController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public object Get(string ReferralCode)
        {
            object obj = new object();
            try
            {
                if (string.IsNullOrEmpty(ReferralCode))
                {
                    return obj = new { Success = 0, Message = "Enter valid Referral code.", data = string.Empty };
                }
                bool chkReferralId = db.MLMUsers.Any(p => p.Ref_Id == ReferralCode);
                if (chkReferralId != true)
                {
                    return obj = new { Success = 0, Message = "Invalid Referral ID | Referral ID “" + ReferralCode + "” entered by you does not exist. | Please add a valid Referral ID.", data = string.Empty };
                }
                else if (!db.MLMUsers.Where(x => x.Ref_Id == ReferralCode).FirstOrDefault().Ref_Id.Equals(ReferralCode, StringComparison.CurrentCulture))
                {
                    return obj = new { Success = 0, Message = "Invalid Referral ID | Referral ID “" + ReferralCode + "” entered by you does not exist. | Please add a valid Referral ID.", data = string.Empty };
                }
                string UserName = " under " + db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == (db.MLMUsers.FirstOrDefault(m => m.Ref_Id == ReferralCode).UserID)).FirstName;
                obj = new { Success = 1, Message = "You are going to join Ezeelo" + UserName, data = string.Empty };

            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
