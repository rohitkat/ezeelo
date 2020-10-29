using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gandhibagh.Controllers
{
    public class PartnerRequestController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();

        public PartnerRequestController()
        {

        }
        public class ForLoopClass //----------------use this class for loop purpose in below functions--------------
        {
            public long ID { get; set; }
            public string Name { get; set; }
        }

        [HttpPost]
        public ActionResult Create(PartnerRequest objPartnerRequest)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            PartnerRequest obj = new PartnerRequest();

            //This list used in Admin Module-> PartenerRequestController->Details
            obj.InvestmentCapacityList = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Selected = false, Text = "5-10 Lacs", Value = ((int)1).ToString()},
                    new SelectListItem { Selected = false, Text = "10-15 Lacs", Value = ((int)2).ToString()},
                    new SelectListItem { Selected = false, Text = "15-20 Lacs", Value = ((int)3).ToString()},
                    new SelectListItem { Selected = false, Text = "20 Lac plus", Value = ((int)4).ToString()},
                }, "Value", "Text", 1);
            obj.StateList = new SelectList(db.States.Where(s => s.IsActive == true).OrderBy(s => s.Name).ToList(), "ID", "Name");
            obj.CityList = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Selected = true, Text = "-- Select City -- ", Value = "-1"},
                }, "Value", "Text", 1);
            obj.AreaList = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Selected = true, Text = "-- Select Area -- ", Value = "-1"},
                }, "Value", "Text", 1);
            return View(obj);
        }

        [HttpPost]
        public ActionResult SavePartnerRequest(string Name, string EmailId, string ContactNo, long StateID, string CityName, int InvestmentCapacity, string Space, string ExistingBusiness)
        {
            PartnerRequest obj = new PartnerRequest();
            obj.Name = Name;
            obj.ContactNo = ContactNo;
            obj.EmailId = EmailId;
            obj.StateID = StateID;
            obj.CityName = CityName;
            obj.InvestmentCapacity = InvestmentCapacity;
            obj.Space = Space;
            obj.ExistingBusiness = ExistingBusiness;
            obj.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            long result = obj.SaveData(obj);
            SendEmail(result);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CheckEmail(string EmailId)
        {
            bool obj = db.PartnerRequests.Any(p => p.EmailId == EmailId);
            string Result = obj ? "1" : "0";
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CheckContactNo(string ContactNo)
        {
            bool obj = db.PartnerRequests.Any(p => p.ContactNo == ContactNo);
            string Result = obj ? "1" : "0";
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult BindCity(long StateID)
        //{
        //    List<ForLoopClass> forloopclasses = db.Cities.Where(c => db.Districts.Where(d => d.StateID == StateID).Select(d => d.ID).Contains(c.DistrictID)).Select(c =>
        //        new ForLoopClass
        //        {
        //            Name = c.Name,
        //            ID = c.ID
        //        }).OrderBy(c => c.Name).ToList();
        //    return Json(forloopclasses.ToList(), JsonRequestBehavior.AllowGet);
        //}


        //[HttpPost]
        //public JsonResult BindArea(long CityID)
        //{
        //    List<ForLoopClass> forloopclasses = db.Areas.Where(c => db.Pincodes.Where(d => d.CityID == CityID).Select(d => d.ID).Contains(c.PincodeID)).Select(c =>
        //        new ForLoopClass
        //        {
        //            Name = c.Name,
        //            ID = c.ID
        //        }).OrderBy(c => c.Name).ToList();
        //    return Json(forloopclasses.ToList(), JsonRequestBehavior.AllowGet);
        //}

        public void SendEmail(long ID)
        {
            SendMailForUser(ID);
            List<MailReceiver> EmailList = db.MailReceivers.Where(p => p.IsActive == true).ToList();
            foreach (var item in EmailList)
            {
                SendMailForAdmin(ID, item.EmailID);
            }
        }

        public ActionResult SendMailForUser(long ID)
        {
            try
            {
                PartnerRequest obj = db.PartnerRequests.FirstOrDefault(p => p.ID == ID);

                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();

                dictEmailValues.Add("<!--Name-->", obj.Name);
                dictEmailValues.Add("<!--City-->", obj.CityName);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                string EmailID = obj.EmailId;

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.PARTNER_REQUEST_USER, new string[] { EmailID, "sales@ezeelo.com" }, dictEmailValues, true);
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
            return View();
        }
        public ActionResult SendMailForAdmin(long ID, string Email)
        {
            try
            {
                PartnerRequest obj = db.PartnerRequests.FirstOrDefault(p => p.ID == ID);
                obj.State = db.States.FirstOrDefault(s => s.ID == obj.StateID).Name;
                
                obj.InvestmentCapacityList = new SelectList(
                   new List<SelectListItem>
                {
                    new SelectListItem { Selected = false, Text = "5-10 Lacs", Value = ((int)1).ToString()},
                    new SelectListItem { Selected = false, Text = "10-15 Lacs", Value = ((int)2).ToString()},
                    new SelectListItem { Selected = false, Text = "15-20 Lacs", Value = ((int)3).ToString()},
                    new SelectListItem { Selected = false, Text = "20 Lac plus", Value = ((int)4).ToString()},
                }, "Value", "Text", 1);
                string CapcityId = obj.InvestmentCapacity.ToString();
                obj.Investment_Capacity = obj.InvestmentCapacityList.FirstOrDefault(p => p.Value == CapcityId).Text;
                obj.DisplayDate = obj.RegistrationDateTime.ToString("dd/MM/yyyy HH:mm");


                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();

                dictEmailValues.Add("<!--RegistrationDateTime-->", obj.DisplayDate);
                dictEmailValues.Add("<!--PartnerCode-->", obj.PartnerCode);
                dictEmailValues.Add("<!--Name-->", obj.Name);
                dictEmailValues.Add("<!--EmailId-->", obj.EmailId);
                dictEmailValues.Add("<!--ContactNo-->", obj.ContactNo);
                dictEmailValues.Add("<!--State-->", obj.State);
                dictEmailValues.Add("<!--City-->", obj.CityName);
                dictEmailValues.Add("<!--Area-->", obj.Area);
                dictEmailValues.Add("<!--Investment_Capacity-->", obj.Investment_Capacity);
                dictEmailValues.Add("<!--Space-->", obj.Space);
                dictEmailValues.Add("<!--ExistingBusiness-->", obj.ExistingBusiness);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                string EmailID = Email;

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.PARTNER_REQUEST_ADMIN, new string[] { EmailID, "sales@ezeelo.com" }, dictEmailValues, true);
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
            return View();
        }
    }
}