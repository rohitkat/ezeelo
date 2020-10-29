using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class MsgInviteUserLeaderController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public object Get(long UserLoginId)
        {
            object obj = new object();
            try
            {
                if (UserLoginId == null || UserLoginId <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid data.", data = string.Empty };
                }
                bool IsMLMUser = db.MLMUsers.Where(x => x.UserID == UserLoginId).Any();
                PersonalDetail P = db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == UserLoginId);
                string Name = "";
                if (P != null)
                {
                    Name = P.FirstName + ((P.LastName == null) ? "" : " "+P.LastName);
                    Name = Name.Trim();
                }
                if (IsMLMUser)
                {
                    string ReferralId = db.MLMUsers.Where(x => x.UserID == UserLoginId).Select(x => x.Ref_Id).FirstOrDefault();
                    string Message = "Hi! I am " + Name + ", I would like to invite you to this awesome Leadership Opportunity by http://ezeelo.com/kanpur/1060/Login/Login?ReferalCode="+ ReferralId + " . Just click on this link https://ezeelo.page.link/app use referral code '" + ReferralId + "' for registration. And be ready to experience an amazing way to buy your Daily-Needs and Groceries.";
                    obj = new { Success = 1, Message = "Success", data = Message };
                }
                else
                {
                    obj = new { Success = 0, Message = "It is not Leader", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
