using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using BusinessLogicLayer;
using System.IO;
using System.Text;
using Merchant.Models;

namespace Merchant.Controllers
{
    public class ShopProfileController : Controller
    {
        public class ForLoopClass
        {
            public long ID { get; set; }
            public string Name { get; set; }
        }

        #region Genral Code
        private EzeeloDBContext db = new EzeeloDBContext();

        private long GetShopID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
            long BusinessDetailID = 0;
            long ShopID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    BusinessDetailID = Convert.ToInt32(db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                    ShopID = Convert.ToInt32(db.Shops.Where(x => x.BusinessDetailID == BusinessDetailID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PlacedController][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
            }
            return ShopID;
        }
        private long GetPersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
            long PersonalDetailID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PlacedController][GetShopID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }

        #endregion


        [SessionExpire]
        [Authorize(Roles = "ShopProfile/CanRead")]
        public ActionResult ShopImageUpload()
        {
            long ShopID = GetShopID();
           
            ProductUploadTempViewModel productuploadtempviewmodel = new ProductUploadTempViewModel();
            Shop sp = db.Shops.Find(ShopID);
            productuploadtempviewmodel.Description=sp.Description;

            string[] src = ImageDisplay.LoadShopImages(ShopID, ProductUpload.IMAGE_TYPE.Approved);
            ViewBag.ImageURL = src;
            productuploadtempviewmodel.Path = src;
            return View(productuploadtempviewmodel);
        }

        [HttpPost]
        [SessionExpire]
        [Authorize(Roles = "ShopProfile/CanWrite")]
        public ActionResult ShopImageUpload(ProductUploadTempViewModel productuploadtempviewmodel, List<HttpPostedFileBase> Files, string submit)
        {

            string strValue = submit;
            string[] strTemp = strValue.Split('$');

            var val1 = strTemp[0];
            var val2 = strTemp[1];
            if (val2 == "")
            {
                val2 = "1";
            }

            submit = val1.ToString();

            int StrID = Convert.ToInt32(val2);


            try
            {
                switch (submit)
                {
                    case "Save":

                        long ShopID = GetShopID();
                        long PersonalDetailID = GetPersonalDetailID();

                        try
                        {
                            CommonFunctions.UploadShopImages(Files, ShopID, ProductUpload.IMAGE_TYPE.Approved);
                        }
                        catch (BusinessLogicLayer.MyException myEx)
                        {
                            ModelState.AddModelError("Error", "There's Something wrong with the Image!!");

                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                + "[ShopProfile][POST:Create]" + myEx.EXCEPTION_PATH,
                                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                        }
                        catch (Exception ex)
                        {

                            ModelState.AddModelError("Error", "There's Something wrong with the Image!!");

                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + ex.Message + Environment.NewLine
                                + "[ShopProfile][POST:Create]",
                                BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                        }

                        EzeeloDBContext db1 = new EzeeloDBContext();

                        Shop sp = db1.Shops.Find(ShopID);

                        sp.Description= productuploadtempviewmodel.Description;
                        sp.ModifyDate = DateTime.UtcNow;
                        sp.ModifyBy = PersonalDetailID;
                        sp.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        db1.SaveChanges();

                        return RedirectToAction("ShopImageUpload");

                    case "Remove":
                        //CommonFunctions.DeleteProductImages(productuploadtempviewmodel.Path[StrID], productuploadtempviewmodel.ID, productuploadtempviewmodel.Name, "Default", string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                        CommonFunctions.DeleteShopImages(productuploadtempviewmodel.Path[StrID]);
                        return RedirectToAction("ShopImageUpload");
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Image!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShopProfile][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Image!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShopProfile][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return RedirectToAction("ShopImageUpload");
        }

        [SessionExpire]
        [Authorize(Roles = "ShopProfile/CanRead")]
        public ActionResult MerchantProfileUpload()
        {
            return View();
        }

        [HttpPost]
        [SessionExpire]
        [Authorize(Roles = "ShopProfile/CanWrite")]
        public ActionResult MerchantProfileUpload(HttpPostedFileBase Files)
        {

            try
            {
                long ShopID = GetShopID();

                CommonFunctions.UploadMerchantProfile(Files, ShopID, ProductUpload.IMAGE_TYPE.Approved);
                //string src = ImageDisplay.LoadShopLogo(ShopID, ProductUpload.IMAGE_TYPE.NonApproved);
                return RedirectToAction("Index", "Home");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Merchant Profile Image!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShopProfile][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Merchant Profile Image!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShopProfile][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }
    }

}