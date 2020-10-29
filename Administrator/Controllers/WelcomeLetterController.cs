using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogicLayer;
using Administrator.Models;
using System.Data.Entity;
using System.Text;
namespace Administrator.Controllers
{
    [SessionExpire]
    public class WelcomeLetterController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /WelcomeLetter/
        [CustomAuthorize(Roles = "WelcomeLetter/CanRead")]
        public ActionResult Index()        
        {
           
            ViewData["Franchise"] = new SelectList(this.ShowFranchiseList(), "OwnerID", "Name");
            ViewBag.ddlSecondLevelCategory = new SelectList(db.Categories.Where(x => x.Level == 2).ToList(), "ID", "Name");
           
            List<SelectListItem> lData = new List<SelectListItem>();
            lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
            ViewBag.ddlThirdLevelCategory = new SelectList(lData, "Value", "Text");
            List<ApprovedMerchantListByFranchise> MerchantList = new List<ApprovedMerchantListByFranchise>();

            return View(MerchantList.OrderBy(x => x.ShopName).ToList());
       
        }
        [HttpPost]
        [CustomAuthorize(Roles = "WelcomeLetter/CanRead")]     
        public ActionResult Index(int? franchiseID)
        {
            ViewData["Franchise"] = new SelectList(this.ShowFranchiseList(), "OwnerID", "Name");
            ViewBag.ddlSecondLevelCategory = new SelectList(db.Categories.Where(x => x.Level == 2).ToList(), "ID", "Name");

            List<SelectListItem> lData = new List<SelectListItem>();
            lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
            ViewBag.ddlThirdLevelCategory = new SelectList(lData, "Value", "Text");

            if (franchiseID == null)
            {  
                List<ApprovedMerchantListByFranchise> MerchantList = new List<ApprovedMerchantListByFranchise>();
                return View(MerchantList.OrderBy(x => x.ShopName).ToList());
            }

            try
            {
                List<ApprovedMerchantListByFranchise> MerchantList = new List<ApprovedMerchantListByFranchise>();
                BusinessLogicLayer.ApprovedMerchantList obj = new ApprovedMerchantList();
                List<object> paramValues = new List<object>();
                paramValues.Add(franchiseID);
                paramValues.Add(0);
                MerchantList = obj.MerchantList(paramValues, System.Web.HttpContext.Current.Server);

                return View(MerchantList.OrderBy(x => x.ShopName).ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[WelcomeLetterController][GET:ApprovedMerchant]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[WelcomeLetterController][GET:ApprovedMerchant]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();

        }
        [CustomAuthorize(Roles = "WelcomeLetter/CanRead")]
        public ActionResult NewIndex()
        {
            ViewData["Franchise"] = new SelectList(this.ShowFranchiseList(), "OwnerID", "Name");
            List<SelectListItem> lData = new List<SelectListItem>();
            lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
            ViewData["MerchantID"] = new SelectList(lData, "Value", "Text");
            return View();
        }
        public ActionResult WelcomeLetter_Containt(int franchiseID, long MerchantID)
        {
            try
            {
                List<ApprovedMerchantListByFranchise> MerchantList = new List<ApprovedMerchantListByFranchise>();
                BusinessLogicLayer.ApprovedMerchantList obj = new ApprovedMerchantList();
                List<object> paramValues = new List<object>();
                paramValues.Add(franchiseID);
                paramValues.Add(MerchantID);
                MerchantList = obj.MerchantList(paramValues, System.Web.HttpContext.Current.Server);

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewData["PlanCategory"] = new SelectList(lData, "Value", "Text");

                return PartialView("_WelcomeLetter", MerchantList.OrderBy(x => x.ShopName).ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[WelcomeLetterController][GET:ApprovedMerchant]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[WelcomeLetterController][GET:ApprovedMerchant]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return PartialView();
        }
        public List<FranchisePendingApprovalViewModel> ShowFranchiseList()
        {

            List<FranchisePendingApprovalViewModel> lfav = new List<FranchisePendingApprovalViewModel>();
            lfav = ((from ul in db.UserLogins
                    join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                    join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                    join f in db.Franchises on bd.ID equals f.BusinessDetailID
                    where bd.BusinessType.Prefix == "GBFR" && ul.IsLocked == false
                    select new FranchisePendingApprovalViewModel
                    {
                        UserLoginID = bd.UserLoginID,
                        BusinessTypePrefix = bd.BusinessType.Prefix,
                        Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName,
                        OwnerId = f.ID,
                    }).Distinct()).ToList();
            return lfav;
        }
        [CustomAuthorize(Roles = "WelcomeLetter/CanWrite")]
        public JsonResult SendMailToMerchant_SelectedCategory(long MerchantID, string tableData)
        {
            try
            {
                //tableData = "<form action='mailto:admin@example.com' enctype='text/plain' method='post'>" +
                //            "<p>Name: <input name='Name' type='text' id='Name' size='40'></p>" +
                //            "<p>E-mail address: <input name='E-mail' type='text' id='E-mail' size='40'></p>" +
                //            "<p>Comment:</p>" +
                //            "<p><textarea name='Comment' cols='55' rows='5' id='Comment'></textarea></p>" +
                //            "<p><input type='submit' name='Submit' value='Submit'></p></form>";
                //string tableData = this.BindTableString(MerchantID);

                ApprovedMerchantListByFranchise MerchantList = new ApprovedMerchantListByFranchise();
                BusinessLogicLayer.ApprovedMerchantList obj = new ApprovedMerchantList();
                List<object> paramValues = new List<object>();
                paramValues.Add(0);
                paramValues.Add(MerchantID);
                MerchantList = (obj.MerchantList(paramValues, System.Web.HttpContext.Current.Server)).FirstOrDefault();


                // Sending email to the user
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                Dictionary<string, string> emailValues = new Dictionary<string, string>();
                emailValues.Add("<!--MerchantName-->", MerchantList.MerchantName);
                emailValues.Add("<!--ShopName-->", MerchantList.ShopName);
                emailValues.Add("<!--Address-->", MerchantList.MerchantAddress);
                emailValues.Add("<!--City-->", MerchantList.CityName);
                emailValues.Add("<!--Pincode-->", MerchantList.Pincode);
                emailValues.Add("<!--PlanName-->", MerchantList.PlanName);
                emailValues.Add("<!--TodayDate-->", DateTime.UtcNow.AddHours(5.30).ToShortDateString());
                emailValues.Add("<!--CategoryTable-->", tableData);

                BusinessLogicLayer.ReadConfig readConfig = new ReadConfig(System.Web.HttpContext.Current.Server);

               string[] emailArray = new string[] { MerchantList.Email.ToString(), readConfig.DEFAULT_EMAIL };//, readConfig.DEFAULT_ALL_EMAIL };
               // string[] emailArray = new string[] { "pradnyakar786@gmail.com" };
                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.MER_FINAL, emailArray, emailValues, true);

                UpdateLetterDetail(MerchantID);

                return Json("Mail Sent Successfully", JsonRequestBehavior.AllowGet);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Employee Registered Succesfully, there might be problem sending SMS, please check your mobile or contact administrator!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send SMS..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH, BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return Json("Problem in sending Mail", JsonRequestBehavior.AllowGet);
                // throw new Exception("Unable to Send Email");
            }
        }
        [CustomAuthorize(Roles = "WelcomeLetter/CanWrite")]
        public JsonResult SendMailToMerchant(long MerchantID)
        {
            try
            {
                //tableData = "<form action='mailto:admin@example.com' enctype='text/plain' method='post'>" +
                //            "<p>Name: <input name='Name' type='text' id='Name' size='40'></p>" +
                //            "<p>E-mail address: <input name='E-mail' type='text' id='E-mail' size='40'></p>" +
                //            "<p>Comment:</p>" +
                //            "<p><textarea name='Comment' cols='55' rows='5' id='Comment'></textarea></p>" +
                //            "<p><input type='submit' name='Submit' value='Submit'></p></form>";
                string tableData = this.BindTableString(MerchantID);

                ApprovedMerchantListByFranchise MerchantList = new ApprovedMerchantListByFranchise();
                BusinessLogicLayer.ApprovedMerchantList obj = new ApprovedMerchantList();
                List<object> paramValues = new List<object>();
                paramValues.Add(0);
                paramValues.Add(MerchantID);
                MerchantList = (obj.MerchantList(paramValues, System.Web.HttpContext.Current.Server)).FirstOrDefault();


                // Sending email to the user
                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);

                Dictionary<string, string> emailValues = new Dictionary<string, string>();
                emailValues.Add("<!--MerchantName-->", MerchantList.MerchantName);
                emailValues.Add("<!--ShopName-->", MerchantList.ShopName);
                emailValues.Add("<!--Address-->", MerchantList.MerchantAddress);
                emailValues.Add("<!--City-->", MerchantList.CityName);
                emailValues.Add("<!--Pincode-->", MerchantList.Pincode);
                emailValues.Add("<!--PlanName-->", MerchantList.PlanName);
                emailValues.Add("<!--TodayDate-->", DateTime.UtcNow.AddHours(5.30).ToShortDateString());
                emailValues.Add("<!--CategoryTable-->", tableData);

                BusinessLogicLayer.ReadConfig readConfig = new ReadConfig(System.Web.HttpContext.Current.Server);

                string[] emailArray = new string[] { MerchantList.Email.ToString(), readConfig.DEFAULT_EMAIL };//, readConfig.DEFAULT_ALL_EMAIL };
               // string[] emailArray = new string[] { "pradnyakar786@gmail.com" };
                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.MER_FINAL, emailArray, emailValues, true);

                UpdateLetterDetail(MerchantID);

                return Json("Mail Sent Successfully", JsonRequestBehavior.AllowGet);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Employee Registered Succesfully, there might be problem sending SMS, please check your mobile or contact administrator!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString() + Environment.NewLine + "Can't send SMS..! " + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.EXCEPTION_PATH, BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return Json("Problem in sending Mail", JsonRequestBehavior.AllowGet);
                // throw new Exception("Unable to Send Email");
            }
        }
        public List<Category> SecondLevelCategory()
        {
            List<Category> lcategory = new List<Category>();
            lcategory = db.Categories.Where(x => x.Level == 2).OrderBy(x => x.Name).ToList();
            return lcategory;
        }
        public JsonResult ThirdLevelCategory(long CategoryID)
        {
            List<Category> lcategory = new List<Category>();
            lcategory = db.Categories.Where(x => x.Level == 3 && x.ParentCategoryID == CategoryID).OrderBy(x => x.Name).ToList();
            return Json(lcategory, JsonRequestBehavior.AllowGet);
        }
        public void UpdateLetterDetail(long MerchantID)
        {

            Shop lshop = new Shop();
            lshop = db.Shops.Where(x => x.ID == MerchantID).FirstOrDefault();
            lshop.LetterDate = DateTime.UtcNow.AddHours(5.5);
            lshop.SendBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));

            db.Entry(lshop).State = EntityState.Modified;
            db.SaveChanges();

        }
        public JsonResult MerchantCategory(long MerchantID)
        {
            List<MerchantPlanCategories> MerchantCategoryList = new List<MerchantPlanCategories>();
            BusinessLogicLayer.ApprovedMerchantList obj = new ApprovedMerchantList();
            List<object> paramValues = new List<object>();
            paramValues.Add(MerchantID);
            paramValues.Add(DBNull.Value);
            MerchantCategoryList = obj.MerchantCategoryList(paramValues, System.Web.HttpContext.Current.Server);
            if(MerchantCategoryList != null)
            {
                Int64 ThirdLevelcatId = MerchantCategoryList[0].categoryID;
                Int64 SecondLevelcatId = Convert.ToInt64(db.Categories.Where(x => x.ID == ThirdLevelcatId).FirstOrDefault().ParentCategoryID);
                string FirstLevelCategoryName = db.Categories.Where(x => x.ID == SecondLevelcatId).FirstOrDefault().Name;

                MerchantCategoryList[0].parentCategory = FirstLevelCategoryName;
            }
            return Json(MerchantCategoryList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult MerchantCategoryWise(long MerchantID, long CategoryID)
        {
            List<MerchantPlanCategories> MerchantCategoryList = new List<MerchantPlanCategories>();
            BusinessLogicLayer.ApprovedMerchantList obj = new ApprovedMerchantList();
            List<object> paramValues = new List<object>();
            paramValues.Add(MerchantID);
            paramValues.Add(CategoryID);
            MerchantCategoryList = obj.MerchantCategoryList(paramValues, System.Web.HttpContext.Current.Server);
            if (MerchantCategoryList != null)
            {
                Int64 ThirdLevelcatId = MerchantCategoryList[0].categoryID;
                Int64 SecondLevelcatId = Convert.ToInt64(db.Categories.Where(x => x.ID == ThirdLevelcatId).FirstOrDefault().ParentCategoryID);
                string FirstLevelCategoryName = db.Categories.Where(x => x.ID == SecondLevelcatId).FirstOrDefault().Name;

                MerchantCategoryList[0].parentCategory = FirstLevelCategoryName;
            }
            return Json(MerchantCategoryList.FirstOrDefault(), JsonRequestBehavior.AllowGet);

        }
        public string BindTableString(long MerchantID)
        {

            StringBuilder str= new StringBuilder();
            string FirstLevelCategoryName = string.Empty;
            List<MerchantPlanCategories> MerchantCategoryList = new List<MerchantPlanCategories>();
            BusinessLogicLayer.ApprovedMerchantList obj = new ApprovedMerchantList();
            List<object> paramValues = new List<object>();
            paramValues.Add(MerchantID);
            MerchantCategoryList = obj.MerchantCategoryList(paramValues, System.Web.HttpContext.Current.Server);

            if (MerchantCategoryList != null)
            {
                Int64 ThirdLevelcatId = MerchantCategoryList[0].categoryID;
                Int64 SecondLevelcatId = Convert.ToInt64(db.Categories.Where(x => x.ID == ThirdLevelcatId).FirstOrDefault().ParentCategoryID);
                FirstLevelCategoryName = db.Categories.Where(x => x.ID == SecondLevelcatId).FirstOrDefault().Name;
                //MerchantCategoryList[0].parentCategory = FirstLevelCategoryName;
            }

            str.Append("Main Category  :"+ FirstLevelCategoryName +"<span id='mainCategoryspan@(item.MerchantID)'></span>");

            str.Append("<thead>" +
                        "<tr>" +
                        "<th style='border: 1px solid black;' >" +
                        "<a href='#' id='" + MerchantID + "' class='CategoryEntry plusanchor'>+ Sub Category Name</a>" +
                        "</th>" +
                        "<th style='border: 1px solid black;'>Marketing Fees(%)</th>" +
                         "<th style='border: 1px solid black;'>Marketing Fees(Rs)</th>" +
                        "</tr>" +
                        "</thead>");
            foreach (var item in MerchantCategoryList)
            {
                str.Append("<tr><td style='border: 1px solid black; '>" + item.categoryName + "</td>" +
                                "<td style='border: 1px solid black; ' align='center'>" + item.chargesInPercentage + "</td>" +
                                "<td style='border: 1px solid black; ' align='center'>" + item.chargesInRs + "</td>" +
                                "</tr>");
            }

            return str.ToString();
        }
        public JsonResult MerchantList(Int64 FranchiseID)
        {
            List<MerchantList> lst = new List<MerchantList>();
            lst = (from s in db.Shops
                   join b in db.BusinessDetails on s.BusinessDetailID equals b.ID
                   join bt in db.BusinessTypes on b.BusinessTypeID equals bt.ID
                   join ul in db.UserLogins on b.UserLoginID equals ul.ID
                   where s.IsActive == true && b.IsActive == true && ul.IsLocked == false && bt.Prefix == "GBMR"
                   && s.FranchiseID == FranchiseID
                   select new MerchantList
                   {
                       MerchantID = s.ID,
                       MerchantName = s.Name

                   }).OrderBy(x => new { x.MerchantName }).ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

	}
    
    public class MerchantList
    {
        public long MerchantID { get; set; }
        public string MerchantName { get; set; }
    }
    
}


