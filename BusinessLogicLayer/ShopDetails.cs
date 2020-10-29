//-----------------------------------------------------------------------
// <copyright file="ShopDetails" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using System.IO;
using System.Net;
/*
 Handed over to Mohit
 */
namespace BusinessLogicLayer
{
    public class ShopDetails : ShopDisplay
    {
        public ShopDetails(System.Web.HttpServerUtility server) : base(server) { }

        private EzeeloDBContext db = new EzeeloDBContext();
        /// <summary>
        /// Get Shop Basic Details
        /// </summary>
        /// <param name="shopID"></param>
        /// <returns></returns>
        public override ShopDetailsViewModel GetShopBasicDetails(long shopID)
        {
            ShopDetailsViewModel lshop = new ShopDetailsViewModel();
            //Get Shop details which is active and Live  from Shop Table
            lshop = (from sh in db.Shops
                                where sh.IsLive == true && sh.ID == shopID
                                select new ShopDetailsViewModel
                                    {
                                        Address = sh.Address,
                                        AreaID = sh.AreaID,
                                        BusinessDetailID = sh.BusinessDetailID,
                                        ClosingTime = sh.ClosingTime,
                                        OpeningTime= sh.OpeningTime,
                                        PincodeID = sh.PincodeID,
                                        ReturnDurationInDays = sh.ReturnDurationInDays,
                                        IsAgreedOnReturnProduct = sh.IsAgreedOnReturnProduct,
                                        ContactPerson = sh.ContactPerson,
                                        CurrentItSetup = sh.CurrentItSetup,
                                        Email = sh.Email,
                                        FAX = sh.FAX,
                                        InstitutionalMerchantPurchase = sh.InstitutionalMerchantPurchase,
                                        InstitutionalMerchantSale = sh.InstitutionalMerchantSale,
                                        IsFreeHomeDelivery = sh.IsFreeHomeDelivery,
                                        Landline = sh.Landline,
                                        Lattitude = sh.Lattitude,
                                        Longitude = sh.Longitude,
                                        MinimumAmountForFreeDelivery = sh.MinimumAmountForFreeDelivery,
                                        Mobile = sh.Mobile,
                                        Name = sh.Name,
                                        NearestLandmark = sh.NearestLandmark,
                                        NormalSale= sh.NormalSale,
                                        TIN = sh.TIN,
                                        VAT = sh.VAT,
                                        Website = sh.Website,
                                        WeeklyOff = sh.WeeklyOff,
                                        ShopDescription = sh.Description,
                                        ShopID = shopID
                                    }).FirstOrDefault();

            if (lshop != null)
            {
                //Get Shop Logo                
                //lshop.ShopLogoPath = ImageDisplay.LoadShopLogo(lshop.ShopID, ProductUpload.IMAGE_TYPE.Approved);
                //ReadConfig rcKey=new ReadConfig(System.Web.HttpContext.Current.Server);                
                //lshop.ShopLogoPath = rcKey.IMAGE_HTTP + ProductUpload.IMAGE_ROOTPATH + "/approved_images/shops/" + lshop.ShopID .ToString()+ "/logo.jpg";
                lshop.ShopLogoPath = ImageDisplay.LoadShopLogo(lshop.ShopID, ProductUpload.IMAGE_TYPE.Approved);
                //Get Shop Address details like pincode city and respective area
                lshop.Pincode = db.Pincodes.Find(lshop.PincodeID).Name;
                if (lshop.AreaID != null)
                    lshop.AreaName = db.Areas.Find(lshop.AreaID).Name;
                else
                    lshop.AreaName = string.Empty;
            }
            return lshop;
        }


        /// <summary>
        /// Get Shop Basic Details
        /// </summary>
        /// <param name="shopID"></param>
        /// <returns></returns>
        public ShopDetailsViewModel GetShopBasicDetails(long shopID, int franchiseId)//added cityId->franchiseId
        {
            Shop shop=db.Shops.Find(shopID);
            //var shopCityId = db.Pincodes.Where(x => x.ID == shop.PincodeID).Select(x => x.CityID).FirstOrDefault();//hide
            var shopFranchiseId = db.Shops.Where(x => x.ID == shopID).Select(x => x.FranchiseID).FirstOrDefault();//--added by Ashish for multiple franchise in same city--//
            ShopDetailsViewModel lshop = new ShopDetailsViewModel();
            //Get Shop details which is active and Live  from Shop Table
            lshop = (from sh in db.Shops
                     where sh.IsLive == true && sh.ID == shopID 
                     //&& shopCityId==cityId////hide
                     && shopFranchiseId == franchiseId ////added
                     select new ShopDetailsViewModel
                     {
                         Address = sh.Address,
                         AreaID = sh.AreaID,
                         BusinessDetailID = sh.BusinessDetailID,
                         ClosingTime = sh.ClosingTime,
                         OpeningTime = sh.OpeningTime,
                         PincodeID = sh.PincodeID,
                         ReturnDurationInDays = sh.ReturnDurationInDays,
                         IsAgreedOnReturnProduct = sh.IsAgreedOnReturnProduct,
                         ContactPerson = sh.ContactPerson,
                         CurrentItSetup = sh.CurrentItSetup,
                         Email = sh.Email,
                         FAX = sh.FAX,
                         InstitutionalMerchantPurchase = sh.InstitutionalMerchantPurchase,
                         InstitutionalMerchantSale = sh.InstitutionalMerchantSale,
                         IsFreeHomeDelivery = sh.IsFreeHomeDelivery,
                         Landline = sh.Landline,
                         Lattitude = sh.Lattitude,
                         Longitude = sh.Longitude,
                         MinimumAmountForFreeDelivery = sh.MinimumAmountForFreeDelivery,
                         Mobile = sh.Mobile,
                         Name = sh.Name,
                         NearestLandmark = sh.NearestLandmark,
                         NormalSale = sh.NormalSale,
                         TIN = sh.TIN,
                         VAT = sh.VAT,
                         Website = sh.Website,
                         WeeklyOff = sh.WeeklyOff,
                         ShopDescription = sh.Description,
                         ShopID = shopID
                     }).FirstOrDefault();

            if (lshop != null)
            {
                //Get Shop Logo                
                lshop.ShopLogoPath = ImageDisplay.LoadShopLogo(lshop.ShopID, ProductUpload.IMAGE_TYPE.Approved);
                //Get Shop Address details like pincode city and respective area
                lshop.Pincode = db.Pincodes.Find(lshop.PincodeID).Name;
                if (lshop.AreaID != null)
                    lshop.AreaName = db.Areas.Find(lshop.AreaID).Name;
                else
                    lshop.AreaName = string.Empty;
            }
            return lshop;
        }
        /// <summary>
        /// Get Shop Description html file path
        /// </summary>
        /// <param name="shopID">Shop ID</param>
        /// <returns>http file Path</returns>
        //public override string GetShopDescriptionFilePath(long shopID)
        //{
        //    ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
        //    string existsFilePath = string.Empty;
        //    existsFilePath = imgPath.IMAGE_HTTP + "/Content/" + ProductUpload.IMAGE_TYPE.Approved + "_Images/" + ProductUpload.IMAGE_FOR.Shops + "/" + shopID + "/description.html";
        //    if (IsFileExists(existsFilePath))
        //    {
        //        return existsFilePath;
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //    //bool isPathExists = false;
        //    //string filePath = string.Empty;
        //    //ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
        //    //StringBuilder lpath = new StringBuilder("/Content/" + ProductUpload.IMAGE_TYPE.Approved + "_Images/" + ProductUpload.IMAGE_FOR.Shops + "/" + shopID);
        //    ////Check path exists on ftp
        //    //string existsFilePath = imgPath.IMAGE_FTP + lpath.ToString() + "/description.html";
        //    //try
        //    //{
        //    //    FtpWebRequest reqFTP;
        //    //    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(existsFilePath));
        //    //    reqFTP.UseBinary = true;
        //    //    reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;

        //    //    for (int i = 0; i < 2; i++)
        //    //    {
        //    //        try
        //    //        {
        //    //            //get FTP Response
        //    //            reqFTP.GetResponse();
        //    //            isPathExists = true;
        //    //        }
        //    //        catch (WebException wex)
        //    //        {

        //    //            isPathExists = false;

        //    //        }
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //    //        + Environment.NewLine + ex.Message + Environment.NewLine
        //    //        + "[ProductDetails][GetGeneralDescription]",
        //    //        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
        //    //    //file not found or path not exists 
        //    //    isPathExists = false;

        //    //}

        //    //if (!isPathExists)
        //    //{
        //    //    existsFilePath = string.Empty;
        //    //}
        //    //else
        //    //{
        //    //    //assign http path
        //    //    existsFilePath = imgPath.IMAGE_HTTP + lpath.ToString() + "/description.html";

        //    //}
        //    //return existsFilePath;

        //}

        //private bool IsFileExists(string filename)
        //{
        //    try
        //    {
        //        //Creating the HttpWebRequest
        //        HttpWebRequest request = WebRequest.Create(filename) as HttpWebRequest;
        //        //Setting the Request method HEAD, you can also use GET too.
        //        request.Method = "HEAD";

        //        //Getting the Web Response
        //        HttpWebResponse response = request.GetResponse() as HttpWebResponse;

        //        //Returns TURE if the Status code == 200
        //        return (response.StatusCode == HttpStatusCode.OK);
        //    }
        //    catch
        //    {
        //        //file not found or path not exists 
        //        return false;
        //    }
        //}
        /// <summary>
        /// Get Shop Image Gallery
        /// </summary>
        /// <param name="shopID">Shop ID</param>
        /// <returns></returns>
        public override List<ImageListViewModel> GetShopImageGallery(long shopID)
        {
            List<ImageListViewModel> lImages = new List<ImageListViewModel>();
           //Get Image LIst
            string [] lImageFile = ImageDisplay.LoadShopImages(shopID);
            //Assign list of image path to ImageListViewModel
            if (lImageFile != null)
            {
                return lImages = (from str in lImageFile
                                  select new ImageListViewModel
                                  {

                                      ImgPath = str

                                  }).ToList();
            }
            else
                return lImages; 
          
        }
        /// <summary>
        /// Get Shop Complete details like Basic details, Description file path, Image Gallery
        /// </summary>
        /// <param name="shopID">Shop ID</param>
        /// <returns></returns>
        public ViewShopDetailsViewModel GetShopDetails(long shopID)
        {
            ViewShopDetailsViewModel lShopDetails = new ViewShopDetailsViewModel();
            // get Shop Basic Details
            lShopDetails.ShopBasicDetails = GetShopBasicDetails(shopID);
            //get Description File Path
            lShopDetails.ShopDescriptionFilePath = string.Empty;//GetShopDescriptionFilePath(shopID);
            //get Shop Image List
            lShopDetails.ShopImageList = GetShopImageGallery(shopID);

            //return Complete Shop Details
            return lShopDetails;
        }


        /// <summary>
        /// Get Shop Complete details like Basic details, Description file path, Image Gallery
        /// Overload added by Tejaswee
        /// After change city previous page loaded but if shop not belong to selected city that time redirect user on home page
        /// </summary>
        /// <param name="shopID">Shop ID</param>
        /// <returns></returns>
        public ViewShopDetailsViewModel GetShopDetails(long shopID, int franchiseId)//added cityId->franchiseId
        {
            ViewShopDetailsViewModel lShopDetails = new ViewShopDetailsViewModel();
            // get Shop Basic Details
            //--added by Ashish for multiple franchise in same city--//
            lShopDetails.ShopBasicDetails = GetShopBasicDetails(shopID, franchiseId);//added cityId->franchiseId
            if (lShopDetails.ShopBasicDetails != null)
            {
                //get Description File Path
                lShopDetails.ShopDescriptionFilePath = string.Empty;  // GetShopDescriptionFilePath(shopID);
                //get Shop Image List
                lShopDetails.ShopImageList = GetShopImageGallery(shopID);
            }

            //return Complete Shop Details
            return lShopDetails;
        }

    }
}
