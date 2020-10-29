using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class InviteUserFromLeadersController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        public object Post(InviteUser user)
        {
            object obj = new object();
            try
            {
                if (user == null)
                {
                    return obj = new { Success = 0, Message = "Please provoid valid data", data = string.Empty };
                }
                SendEmail_InviteUser(user.Message, user.MobileNo, user.Name, user.Email, user.UserLoginId);
                SaveMLMUserInvites(user.Message, user.MobileNo, user.Name, user.Email, user.UserLoginId);
                obj = new { Success = 1, Message = "Successfull.", data = string.Empty };
            }
            catch (Exception ex)
            {
                obj = new { Success = 1, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        public void SendEmail_InviteUser(string msg, string Mobile, string uName, string Email, long LoginUserId)
        {
            try
            {
                var UserLogin = db.PersonalDetails.Where(u => u.UserLoginID == LoginUserId)
                    .Join(db.MLMUsers, u => u.UserLoginID, m => m.UserID, (u, m) => new
                    {
                        RefferalCode = m.Ref_Id,
                        Name = u.FirstName
                    }).ToList();
                if (UserLogin != null && UserLogin.Count() > 0)
                {
                    string Name = UserLogin.First().Name;
                    string RefferalCode = UserLogin.First().RefferalCode;
                    string URL = "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "nagpur/2/login?Phone=" + Mobile + "&ReferalCode=" + RefferalCode + "&Name=" + uName + "&Email=" + Email;
                    Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                    dictEmailValues.Add("<!--RefferalCode-->", RefferalCode);
                    dictEmailValues.Add("<!--UserName-->", Name);
                    dictEmailValues.Add("<!--Message-->", msg);
                    dictEmailValues.Add("<!--URL-->", URL);
                    BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                    string EmailID = Email;
                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.Leaders_InviteUser, new string[] { EmailID, "sales@ezeelo.com" }, dictEmailValues, true);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DashboardController][M:SendEmail_InviteUser]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DashboardController][M:SendEmail_InviteUser]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
        }

        public void SaveMLMUserInvites(string msg, string Mobile, string uName, string email, long LoginUserId)
        {
            bool isInviteIDUsed = false;
            string InviteID = "";
            int counter = 5;
            do
            {
                InviteID = GenerateInviteID("0123456789", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                isInviteIDUsed = db.MLMUserInvite.Any(p => p.InviteID == InviteID);
                InviteID = (isInviteIDUsed == true) ? "" : InviteID;
                counter = counter - 1;
            }
            while (isInviteIDUsed == true || counter == 0);

            if (InviteID != "")
            {
                MLMUserInvites obj = new MLMUserInvites();
                obj.CreateBy = LoginUserId;
                obj.CreateDate = DateTime.Now;
                obj.Email = email;
                obj.IsAccepted = false;
                obj.Message = msg;
                obj.Mobile = Mobile;
                obj.Name = uName;
                obj.UserLoginID = LoginUserId;
                obj.InviteID = InviteID;
                db.MLMUserInvite.Add(obj);
                db.SaveChanges();
            }
        }
        string GenerateInviteID(string No, string Alpha)
        {
            RefferalCodeGenerator objRefferalCodeGenerator = new RefferalCodeGenerator();
            string Characters = "";
            string Numbers = "";

            Numbers = No;
            Characters = Alpha;
            string CodeString = objRefferalCodeGenerator.CreateCode(5, Characters);
            string CodeNumeric = objRefferalCodeGenerator.CreateCode(4, Numbers);
            string RefferalCode = CodeString + CodeNumeric;
            return RefferalCode;
        }
    }
}
