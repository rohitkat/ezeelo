using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;

namespace Gandhibagh.Controllers
{
    public class CareerApplyNowController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /CareerApplyNow/
        public ActionResult Index(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Career career = db.Careers.Find(id);
            if (career == null)
            {
                return HttpNotFound();
            }
            ViewBag.Career = career.Jobtitle.ToString();
            ViewBag.CareerID = career.ID;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index([Bind(Include = "ID,CareerID,Name,Email,Mobile,TotalExpience,CurrentCTC,ExpectedCTC,ResumePath,Remarks")] ApplicationDetail applicationdetail, HttpPostedFileBase file)
        {
            try
            {

                ViewBag.Career = db.Careers.Where(x => x.ID == applicationdetail.CareerID).FirstOrDefault().Jobtitle;
                ViewBag.CareerID = db.Careers.Where(x => x.ID == applicationdetail.CareerID).FirstOrDefault().ID;
                Guid g;
                g = Guid.NewGuid();
                string strMsg = this.UploadCV(file, g.ToString());

                if (!String.IsNullOrEmpty(strMsg))
                {
                    ViewBag.Message = strMsg;
                    return View();
                }
                var extension = Path.GetExtension(file.FileName);
                applicationdetail.ResumePath = g.ToString() + extension;
                applicationdetail.AppliedDate = DateTime.UtcNow.AddHours(5.30);

                if (ModelState.IsValid)
                {
                    db.ApplicationDetails.Add(applicationdetail);
                    db.SaveChanges();
                    TempData["SuccessUploadedResume"] = "Your CV Succesfully Uploaded";
                    this.SendEmail(applicationdetail.Email, applicationdetail.Name);
                    //return View(emptyObj);
                    if (Request.Cookies["CityCookie"] != null && !string.IsNullOrEmpty(Request.Cookies["CityCookie"].Value))
                    {
                        return RedirectToRoute("Career", new { city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower(), franchiseId = Request.Cookies["CityCookie"].Value.Split('$')[2] });////added franchiseId
                    }
                    else
                    {                        
                        return RedirectToRoute("Career", new { city = URLsFromConfig.GetDefaultData("CITY_NAME"), franchiseId = URLsFromConfig.GetDefaultData("FRANCHISE_ID") }); //Yashaswi 01/12/2018 Default City Change 
                    }
                    //return RedirectToAction("Career", "Home", new { city = "nagpur" });
                }
                return View(applicationdetail);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CareerApplyNow][Get:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View("HttpError");
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CareerApplyNow][Get:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return View("HttpError");
            }
        }

        public string UploadCV(HttpPostedFileBase file, string guid)
        {
            try
            {

                if (file == null)
                {
                    return ("Please Upload Your file");
                }
                else if (file.ContentLength > 0)
                {
                    int MaxContentLength = 1024 * 1024 * 3; //3 MB
                    string[] AllowedFileExtensions = new string[] { ".doc", ".pdf", ".docx" };

                    if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                    {
                        return ("Please file of type: " + string.Join(", ", AllowedFileExtensions));
                    }

                    else if (file.ContentLength > MaxContentLength)
                    {
                        return ("Your file is too large, maximum allowed size is: " + MaxContentLength + " MB");
                    }
                    else
                    {
                        //TO:DO
                        //var fileName = Path.GetFileName(file.FileName);
                        var extension = Path.GetExtension(file.FileName);
                        //var path = Path.Combine(Server.MapPath("~/Content/Upload"), guid + extension);
                        //file.SaveAs(path);
                        //return "";

                        CommonFunctions.UploadCareerFile(file, guid, extension);
                        return "";
                    }

                }

                return "";
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CareerApplyNow][UploadCV]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return "";
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CareerApplyNow][UploadCV]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return "";
            }
        }

        private void SendEmail(string EmailAddress, string CandidateName)
        {
            try
            {
                // Sending email to the user
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                Dictionary<string, string> emailParaMetres = new Dictionary<string, string>();               
                emailParaMetres.Add("<!--NAME-->", CandidateName);
                emailParaMetres.Add("<!--URL-->", "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "");
                ReadConfig lReadConfig = new ReadConfig(System.Web.HttpContext.Current.Server);

                string hrEmail = lReadConfig.HR_EMAIL;
                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CAREER_APPLICATION, new string[] { EmailAddress, hrEmail }, emailParaMetres, true);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Applicatioin Registered Succesfully, there might be problem sending email, please check your email or contact administrator!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send Email..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH, BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                // throw new Exception("Unable to Send SMS");
            }
        }
    }
}

        