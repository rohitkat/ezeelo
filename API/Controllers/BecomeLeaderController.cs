using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ModelLayer.Models.ViewModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ModelLayer.Models;
using System.Text.RegularExpressions;
using BusinessLogicLayer;
using System.Data.Entity;

namespace API.Controllers
{
    public class BecomeLeaderController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        [ApiException]
        [ValidateModel]
        public object post(BecomeLeaderViewModel model)
        {
            object obj = new object();
            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Phone) || string.IsNullOrEmpty(model.Pincode))
            {
                return obj = new { Success = 0, Message = "Sorry! Please enter valid details.", data = string.Empty };
            }
            if (!Regex.IsMatch(model.Phone, @"^([5-9]{1}[0-9]{9})$"))//Sonali_24/10/2018
                return obj = new { Success = 0, Message = "Enter valid MobileNo.", data = string.Empty };
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (model.Email != null)
                {
                    if (model.Email.Trim().ToString().Equals(""))
                    {
                        model.Email = null;
                    }
                }
                ModelState.Clear();
                if (ModelState.IsValid)
                {
                    long ID = 0;
                    var objBecomeLeader = db.BecomeLeaders.Where(x => x.Email == model.Email || x.Phone == model.Phone).FirstOrDefault();
                    if (objBecomeLeader != null)
                    {
                        objBecomeLeader.ModifyDate = DateTime.UtcNow.AddHours(5.5);
                        db.Entry(objBecomeLeader).State = EntityState.Modified;
                        db.SaveChanges();
                        ID = objBecomeLeader.ID;
                        SendEmail(ID, true);
                        obj = new { Success = 1, Message = "We have already received your request and we are working on it. Our team will reach you in 24 hours.", data = string.Empty };
                    }
                    else
                    {
                        BecomeLeader Addbecomeleader = new BecomeLeader();
                        Addbecomeleader.Name = model.Name;
                        Addbecomeleader.Email = model.Email;
                        Addbecomeleader.Phone = model.Phone;
                        Addbecomeleader.Pincode = model.Pincode;
                        Addbecomeleader.CreateDate = DateTime.UtcNow.AddHours(5.5);
                        Addbecomeleader.CreateBy = 3;
                        Addbecomeleader.NetworkIP = CommonFunctions.GetClientIP();
                        Addbecomeleader.DeviceType = "Mobile";
                        Addbecomeleader.DeviceID = model.DeviceId;
                        db.BecomeLeaders.Add(Addbecomeleader);
                        db.SaveChanges();
                        ID = Addbecomeleader.ID;
                        SendEmail(ID, false);
                        obj = new { Success = 1, Message = "Thanks for expressing your interest to become a Leader with Ezeelo. We will reach you back in 72 hours.", data = string.Empty };
                    }

                }
                else
                {
                    obj = new { Success = 0, Message = "Sorry! Please enter valid details.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                //ViewBag.Message = "Sorry! Problem in customer registration!!";

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                obj = new { Success = 0, Message = "Sorry! Problem in become leader!!", data = string.Empty };
                //return obj = new { HTTPStatusCode = "400", UserMessage = "Sorry! Problem in customer registration!!", UserLoginID = 0, UserName = string.Empty };
                //return View();
            }
            return obj;
        }

        public void SendEmail(long ID, bool IsResend)
        {
            if (IsResend == false)
                SendMailForUser(ID);
            //List<MailReceiver> EmailList = db.MailReceivers.Where(p => p.IsActive == true).ToList();
            //foreach (var item in EmailList)
            //{
            SendMailForAdmin(ID, "support@ezeelo.com", IsResend);
            // }
        }

        public void SendMailForUser(long ID)
        {
            try
            {
                BecomeLeader obj = db.BecomeLeaders.FirstOrDefault(p => p.ID == ID);

                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                dictEmailValues.Add("<!--RequestId-->", obj.ID.ToString());
                dictEmailValues.Add("<!--Name-->", obj.Name);
                dictEmailValues.Add("<!--ContactNo-->", obj.Phone);
                dictEmailValues.Add("<!--RequestDateTime-->", obj.CreateDate.Date.ToString("dd MMMM yyyy"));
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                string EmailID = obj.Email;

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.BECOMELEADER_USER, new string[] { EmailID, "sales@ezeelo.com" }, dictEmailValues, true);
                //Session["Success"] = "Mail Sent Successfully.";

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[Admin:BecomeLeaderController][M:SendEmail]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
                //Session["Error"] = "Mail not sent! Please try again.";
            }
        }
        public void SendMailForAdmin(long ID, string Email, bool IsResend)
        {
            try
            {
                BecomeLeader obj = db.BecomeLeaders.FirstOrDefault(p => p.ID == ID);
                //obj.State = db.States.FirstOrDefault(s => s.ID == obj.StateID).Name;

                //obj.InvestmentCapacityList = new SelectList(
                //   new List<SelectListItem>
                //{
                //    new SelectListItem { Selected = false, Text = "5-10 Lacs", Value = ((int)1).ToString()},
                //    new SelectListItem { Selected = false, Text = "10-15 Lacs", Value = ((int)2).ToString()},
                //    new SelectListItem { Selected = false, Text = "15-20 Lacs", Value = ((int)3).ToString()},
                //    new SelectListItem { Selected = false, Text = "20 Lac plus", Value = ((int)4).ToString()},
                //}, "Value", "Text", 1);
                //string CapcityId = obj.InvestmentCapacity.ToString();
                //obj.Investment_Capacity = obj.InvestmentCapacityList.FirstOrDefault(p => p.Value == CapcityId).Text;
                //obj.DisplayDate = obj.RegistrationDateTime.ToString("dd/MM/yyyy HH:mm");
                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                dictEmailValues.Add("<!--RequestDateTime-->", obj.CreateDate.Date.ToString());
                dictEmailValues.Add("<!--Name-->", obj.Name);
                dictEmailValues.Add("<!--EmailId-->", obj.Email);
                dictEmailValues.Add("<!--ContactNo-->", obj.Phone);
                dictEmailValues.Add("<!--Pincode-->", obj.Pincode);
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                string EmailID = Email;

                if (IsResend == false)
                {
                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.BECOMELEADER_ADMIN, new string[] { EmailID, "sales@ezeelo.com" }, dictEmailValues, true);
                }
                else
                {
                    gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.BECOMELEADER_ADMIN_REMINDER, new string[] { EmailID, "sales@ezeelo.com" }, dictEmailValues, true);
                }
                //Session["Success"] = "Mail Sent Successfully.";

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[Admin:PartnerRequestController][M:SendEmail]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                //Session["Error"] = "Mail not sent! Please try again.";
            }
        }

        public class BecomeLeaderViewModel
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Pincode { get; set; }
            public string DeviceId { get; set; }
        }
    }
}
