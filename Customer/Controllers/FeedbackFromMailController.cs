using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gandhibagh.Controllers
{
    public class FeedbackFromMailController : Controller
    {

        private EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /FeedbackFromMail/
        //public ActionResult Index()
        //{
        //    int oprStatus = 0;
        //    ViewBag.loginID = 2;
        //    ViewBag.RatingPoint = 5;
        //    ViewBag.OrderID = 10;
            


        //    return View();
        //}

        //[HttpPost]
        public ActionResult Index(int RatingPoint, long loginID, long OrderID)
        { 
            try
            {
                this.LoginDetails(loginID);
                if (Session["UID"] != null)
                {

                    AlreadyGivenFeedBack(OrderID);
                    OrderDetail(OrderID);
                    //if (ViewBag.Product == null)
                    //{
                    //    List<CustomerRatingViewModel> lCustRatingList = new List<CustomerRatingViewModel>();
                    //    lCustRatingList = this.RatingType("GBPR");
                    //    foreach (var n in lCustRatingList)
                    //    {
                    //        this.InsertProductRating(RatingPoint, loginID, OrderID, n.RatingID, string.Empty);
                    //        this.UpdateDeedBackStatus(n.RatingID, OrderID);
                    //    }
                    //    AlreadyGivenFeedBack(OrderID);
                    //}
                }
                else
                {
                    Response.Redirect("Login/Login");
                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                         + Environment.NewLine +
                        ex.Message.ToString()
                         + Environment.NewLine
                         + "[Controller:-FeedbackFromMail][Action:-Index][Method Type :- httpGET]",
                         BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            
            return View();
        }

        private void OrderDetail(long OrderID)
        {
            string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
            DataTable dt = new DataTable();
            dt = BusinessLogicLayer.Feedback_From_Mail.OrderDetail(OrderID, Connection);
           
            if (dt.Rows.Count > 0)
            {
                ViewBag.OrderCode = dt.Rows[0]["OrderCode"].ToString();
                ViewBag.OrderDate = dt.Rows[0]["OrderDate"].ToString();
                ViewBag.DeliveryDate = dt.Rows[0]["DeliveryDate"].ToString();
            }
          
        }

        [HttpPost]
        public ActionResult Index(long loginID, long OrderID, string comment="No Comments", int hdnproductID = 0, int hdnShopID = 0, int hdnDeliveryID = 0, int hdnOverall = 0)
        {
            try
            {
                if (Session != null)
                {
                    AlreadyGivenFeedBack(OrderID);

                    List<CustomerRatingViewModel> lCustRatingListP = new List<CustomerRatingViewModel>();
                        lCustRatingListP = this.RatingType("GBPR");
                        foreach (var n in lCustRatingListP)
                        {
                            this.InsertProductRating(hdnproductID, loginID, OrderID, n.RatingID, string.Empty);
                            this.UpdateDeedBackStatus(n.RatingID, OrderID);
                        }
                   
                    List<CustomerRatingViewModel> lCustRatingList = new List<CustomerRatingViewModel>();
                    lCustRatingList = this.RatingType("GBMR");
                    foreach (var n in lCustRatingList)
                    {
                        this.InsertShopRating(hdnShopID, loginID, OrderID, n.RatingID, comment);

                        this.UpdateDeedBackStatus(n.RatingID, OrderID);
                    }
                    List<CustomerRatingViewModel> lCustRatingList1 = new List<CustomerRatingViewModel>();
                    lCustRatingList1 = this.RatingType("GBDP");
                    foreach (var n in lCustRatingList1)
                    {
                        this.InsertDeliveryRating(hdnDeliveryID, loginID, OrderID, n.RatingID, comment);

                        this.UpdateDeedBackStatus(n.RatingID, OrderID);
                    }

                    List<CustomerRatingViewModel> lCustRatingList2 = new List<CustomerRatingViewModel>();
                    lCustRatingList2 = this.RatingType("GBOD");
                    foreach (var n in lCustRatingList2)
                    {
                        this.InsertOverAllRating(hdnOverall, loginID, OrderID, n.RatingID, comment);

                        this.UpdateDeedBackStatus(n.RatingID, OrderID);
                    }

                    AlreadyGivenFeedBack(OrderID);
                }

            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                          + Environment.NewLine +
                         ex.Message.ToString()
                          + Environment.NewLine
                          + "[Controller:-FeedbackFromMail][Action:-Index][Method Type :- httpPost]",
                          BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            
            return View();
        }

        public void InsertProductRating(int RatingPoint, long loginID, long OrderID,int RatingTypeID, string comment)
        {

         
            try
            {
                ViewBag.OrderID = OrderID;
                List<ProductListCls> lProductListCls = new List<ProductListCls>();
                lProductListCls = (from n in db.CustomerOrderDetails
                                   join ss in db.ShopStocks on n.ShopStockID equals ss.ID
                                   join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                                   where n.CustomerOrderID == OrderID
                                   select new ProductListCls
                                   {
                                       CustomerOrderID = n.CustomerOrderID,
                                       productID = sp.ProductID
                                   }).ToList();

                foreach (var productls in lProductListCls)
                {
                    Int64 productID = productls.productID;
                    CustomerRatingAndFeedback custRate = new CustomerRatingAndFeedback();
                    custRate.PersonalDetailID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(loginID));
                    custRate.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(loginID));
                    custRate.CreateDate = CommonFunctions.GetLocalTime();
                    custRate.IsApproved = false;
                    custRate.OwnerID = productID;
                    custRate.Point = RatingPoint;
                    custRate.RatingID = RatingTypeID;
                    custRate.Feedback = comment;
                    db.CustomerRatingAndFeedbacks.Add(custRate);
                    db.SaveChanges();
                   

                 
                }

            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                      Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                            + Environment.NewLine +
                            "Property:" + validationError.PropertyName + ",Error:Enable to Insert Record " +
                            validationError.ErrorMessage
                            + Environment.NewLine
                            + "[FeedbackFromMail][InsertProductRating]",
                            BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                    }
                }
            }
        }

        public void InsertShopRating(int RatingPoint, long loginID, long OrderID, int RatingTypeID, string comment)
        {

           
            try
            {
                List<ProductListCls> lProductListCls = new List<ProductListCls>();
                lProductListCls = (from n in db.CustomerOrderDetails
                                   join ss in db.ShopStocks on n.ShopStockID equals ss.ID
                                   join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                                   where n.CustomerOrderID == OrderID
                                   group sp.ShopID by sp.ShopID into newGroup
                                   select new ProductListCls
                                   {
                                       productID = newGroup.Key
                                   }).ToList();

                foreach (var productls in lProductListCls)
                {

                    CustomerRatingAndFeedback custRate = new CustomerRatingAndFeedback();
                    custRate.PersonalDetailID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(loginID));
                    custRate.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(loginID));
                    custRate.CreateDate = CommonFunctions.GetLocalTime();
                    custRate.IsApproved = false;
                    custRate.OwnerID = productls.productID;
                    custRate.Point = Convert.ToInt32(RatingPoint);
                    custRate.RatingID = RatingTypeID;
                    custRate.Feedback = comment;
                    db.CustomerRatingAndFeedbacks.Add(custRate);
                    db.SaveChanges();
                  
                }

            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                            + Environment.NewLine +
                            "Property:" + validationError.PropertyName + ",Error: Enable to Insert Record" +
                            validationError.ErrorMessage
                            + Environment.NewLine
                              + "[FeedbackFromMail][InsertShopRating]",
                            BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                    }
                }
            }
        }

        public void InsertDeliveryRating(int RatingPoint, long loginID, long OrderID, int RatingTypeID, string comment)
        {

            try
            {
                List<ProductListCls> lProductListCls = new List<ProductListCls>();
                lProductListCls = (from n in db.CustomerOrderDetails
                                   join ss in db.ShopStocks on n.ShopStockID equals ss.ID
                                   join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                                   where n.CustomerOrderID == OrderID
                                   group sp.ShopID by sp.ShopID into newGroup
                                   select new ProductListCls
                                   {
                                       productID = newGroup.Key
                                   }).ToList();

                foreach (var productls in lProductListCls)
                {

                    CustomerRatingAndFeedback custRate = new CustomerRatingAndFeedback();
                    custRate.PersonalDetailID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(loginID));
                    custRate.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(loginID));
                    custRate.CreateDate = CommonFunctions.GetLocalTime();
                    custRate.IsApproved = false;
                    custRate.OwnerID = productls.productID;
                    custRate.Point = Convert.ToInt32(RatingPoint);
                    custRate.RatingID = RatingTypeID;
                    custRate.Feedback = comment;
                    db.CustomerRatingAndFeedbacks.Add(custRate);
                    db.SaveChanges();
                   
                }

            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);

                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                            + Environment.NewLine +
                            "Property:" + validationError.PropertyName + ",Error: Enable to Insert Record" +
                            validationError.ErrorMessage
                            + Environment.NewLine
                            + "[FeedbackFromMail][InsertDeliveryRating]",
                            BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                    }
                }
            }
        }

        public void InsertOverAllRating(int RatingPoint, long loginID, long OrderID, int RatingTypeID, string comment)
        {
            try
            {
                

                    CustomerRatingAndFeedback custRate = new CustomerRatingAndFeedback();
                    custRate.PersonalDetailID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(loginID));
                    custRate.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(loginID));
                    custRate.CreateDate = CommonFunctions.GetLocalTime();
                    custRate.IsApproved = false;
                    custRate.OwnerID = OrderID;
                    custRate.Point = Convert.ToInt32(RatingPoint);
                    custRate.RatingID = RatingTypeID;
                    custRate.Feedback = comment;
                    db.CustomerRatingAndFeedbacks.Add(custRate);
                    db.SaveChanges();
                
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                            + Environment.NewLine +
                            "Property:" + validationError.PropertyName + ",Error: Enable to Insert Record" +
                            validationError.ErrorMessage
                            + Environment.NewLine
                            + "[FeedbackFromMail][InsertOverAllRating]",
                            BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                    }
                }
            }
        }

        public List<CustomerRatingViewModel> RatingType(string prefix)
        {
           List<CustomerRatingViewModel> lCustRatingList = new List<CustomerRatingViewModel>();
            lCustRatingList = (from r in db.Ratings
                               join bt in db.BusinessTypes on r.BusinessTypeID equals bt.ID
                               where r.IsActive == true && bt.Prefix == prefix
                               select new CustomerRatingViewModel
                               {
                                   RatingID = r.ID,
                                   RatingName = r.Name
                               }).ToList();

            return lCustRatingList;
        }

        public void UpdateDeedBackStatus(int ratingID, long OrderID)
        {
            try
            {
                string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
                List<object> paramValues = new List<object>();
                paramValues.Add(null);
                paramValues.Add(ratingID);
                paramValues.Add(OrderID);
                DateTime dt = new DateTime();
                dt = DateTime.UtcNow.AddHours(5.30);
                paramValues.Add(dt);
                paramValues.Add(dt);
                paramValues.Add(dt);
                paramValues.Add(dt);
                paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                paramValues.Add(null);
                paramValues.Add(null);
                paramValues.Add("Mail Send Successfully");
                paramValues.Add("UPDATE");
                paramValues.Add(0);
                BusinessLogicLayer.Feedback_From_Mail.Insert_Update__FeedBack_From_Mail(Connection, paramValues);
            }
            catch(Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                              + Environment.NewLine +
                               ",Error: Enable to Insert Record" +
                              ex.Message.ToString()
                              + Environment.NewLine
                              + "[FeedbackFromMail][InsertOverAllRating]",
                              BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
              
        }

        private void LoginDetails(long UserLoginID)
        {
            Dictionary<string, string> lDictUserDetails = new Dictionary<string, string>();

            UserLogin userExist = new UserLogin();
            userExist = db.UserLogins.Find(UserLoginID);

            if (userExist != null)
            {
                Session["UID"] = userExist.ID;
                Session["UserName"] = userExist.Email;
            }
            
        }

        private void AlreadyGivenFeedBack(long OrderID)
        {
            string Connection = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
            DataTable dt = new DataTable();
            dt = BusinessLogicLayer.Feedback_From_Mail.AlreadyGivenFeedBack(OrderID, Connection);
            Int16 count = 0;
            if (dt.Rows.Count > 0)
            {
                //ViewBag.OrderCode 
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["Prefix"].ToString().Equals("GBPR") && ViewBag.Product == null)
                    {
                        ViewBag.Product = "GBPR";
                        ++count;
                    }
                    else if (dt.Rows[i]["Prefix"].ToString().Equals("GBMR") && ViewBag.Shop == null)
                    {
                        ViewBag.Shop = "GBMR";
                        ++count;
                    }
                    else if (dt.Rows[i]["Prefix"].ToString().Equals("GBOD") && ViewBag.Order == null)
                    {
                        ViewBag.Order = "GBOD";
                        ++count;
                    }
                    else if (dt.Rows[i]["Prefix"].ToString().Equals("GBDP") && ViewBag.DeliveryPartner == null)
                    {
                        ViewBag.DeliveryPartner = "GBDP";
                        ++count;
                    }
                }
            }
            if (count >= 4)
            {
                ViewBag.FeedBackMessage = "<label style='color: #D83603; font-weight: bold; line-height: 25px; font-size: 15px; display: block; text-align: justify;'>We are delight, <br /> You Spare time &  help use to improve <br /> <span style='color:#8AC100; display:block; line-height:75px; font-size:20px;'>Thank you for your review</span> <br /> Your feedback is valuable for us and help in being better delight us...!</label>";
            }


        }


	}

    public class ProductListCls
    {
        public long productID { get; set; }
        public long CustomerOrderID { get; set; }

    }
}