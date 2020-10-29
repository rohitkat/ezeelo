using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using System.Web.Mvc;
namespace API.Controllers
{
    public class FeedbackManagementController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        // GET api/feedbackmanagement
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/feedbackmanagement/5
        // [TokenVerification]
        // [ApiException]
        // [LoginSuccess]
        public object Get(int id)
        {

            if (id <= 0)
            {
                object obj = new object();
                obj = new { HTTPStatusCode = "400", UserMessage = "ID not valid", ValidationError = "" };
                return Request.CreateResponse(HttpStatusCode.BadRequest, obj);
            }

            BusinessLogicLayer.FeedbackManagment objFeedbackManagment = new BusinessLogicLayer.FeedbackManagment();
            List<FeedBackManagmentViewModel> objFeedBackManagmentViewModel = new List<FeedBackManagmentViewModel>();

            objFeedBackManagmentViewModel = objFeedbackManagment.FeedBackMIS(id, System.Web.HttpContext.Current.Server);


            return new { HTTPStatusCode = "200", objFeedBackManagmentViewModel };

        }

        // POST api/feedbackmanagement
        public object Post([FromBody]ModelLayer.Models.FeedbackManagment feedbackmanagment)
        {
            object obj = new object();
            try
            {
                if (!ModelState.IsValid)
                {
                    return obj = new { Success = 0, Message = "Enter valid details.", data = string.Empty };
                }
                if (string.IsNullOrEmpty(feedbackmanagment.Email) || string.IsNullOrEmpty(feedbackmanagment.Mobile) || string.IsNullOrEmpty(feedbackmanagment.Message) || feedbackmanagment.FeedbackCategaryID <= 0 || feedbackmanagment.FeedbackCategaryID == null)
                {
                    return obj = new { Success = 0, Message = "Enter valid details.", data = string.Empty };
                }
                if (feedbackmanagment.CityID == null || feedbackmanagment.CityID == 0 || feedbackmanagment.FranchiseID == null || feedbackmanagment.FranchiseID == 0)
                {
                    return obj = new { Success = 0, Message = "Please provide a valid CityId and FranchiseId", data = string.Empty };
                }
                //long cityId = 0;
                //int franchiseId = 0;////added
                feedbackmanagment.IsActive = true;
                feedbackmanagment.CreateDate = BusinessLogicLayer.CommonFunctions.GetLocalTime();
                //feedbackmanagment.CityID = cityId;
                //feedbackmanagment.FranchiseID = franchiseId;////added
                db.FeedbackManagments.Add(feedbackmanagment);
                db.SaveChanges();
                SendEmailAlertOnFeedback(feedbackmanagment);
                this.SendEmailToCustomer(feedbackmanagment.Email);
                obj = new { Success = 1, Message = "Thank you for your valuable feedback!", data = new { ID = feedbackmanagment.ID } };
                //return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        // PUT api/feedbackmanagement/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/feedbackmanagement/5
        public void Delete(int id)
        {
        }

        private void SendEmailAlertOnFeedback(ModelLayer.Models.FeedbackManagment feedbackManagment)
        {
            /* This method is added by Avi Verma.
             * Date : 26-Oct-2015.
             * As discussed with Mahesh Sir.. on date : 23-Oct-2015. 
             * When a customer gives feedback, it is reflected in CRM Module. 
             * But, the same feedback should also be send by mail to CRM person and respective employee, manager etc.
             */
            try
            {
                FeedbackCategary lFeedbackCategary = db.FeedbackCategaries.Find(feedbackManagment.FeedbackCategaryID);
                string lFeedbackCategoryName = "";
                if (lFeedbackCategary != null)
                {
                    lFeedbackCategoryName = lFeedbackCategary.Name;
                }

                string lPersonalDetailName = "";
                PersonalDetail lPersonalDetail = db.PersonalDetails.Find(feedbackManagment.CreateBy);
                if (lPersonalDetail != null)
                {
                    lPersonalDetailName = lPersonalDetail.FirstName;
                }

                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();

                dictEmailValues.Add("<!--Email-->", feedbackManagment.Email);
                dictEmailValues.Add("<!--Mobile-->", feedbackManagment.Mobile);
                dictEmailValues.Add("<!--Category-->", feedbackManagment.FeedbackCategary.Name);
                dictEmailValues.Add("<!--Message-->", feedbackManagment.Message);
                dictEmailValues.Add("<!--Type-->", lFeedbackCategoryName);
                dictEmailValues.Add("<!--CreatedDate-->", feedbackManagment.CreateDate.ToString());

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.FEEDBACK_MANAGEMENT, new string[] { "feedback@ezeelo.com", "crm@ezeelo.com" }, dictEmailValues, true);
            }
            catch (Exception)
            {

                //throw; //Sonali_05-12-2018
            }

        }

        private void SendEmailToCustomer(string email)
        {
            try
            {

                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.FEEDBACK_CUSTOMER, new string[] { email }, dictEmailValues, true);
            }
            catch (Exception)
            {

                //throw; //Sonali_05-12-2018
            }

        }

    }
}
