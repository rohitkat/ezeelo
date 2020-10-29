using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Web;
using ModelLayer.Models;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;
using System.Data.Entity;
using System.Web.Configuration;
namespace BusinessLogicLayer
{
    public class CommonFunctions
    {
        private static EzeeloDBContext db = new EzeeloDBContext();
        /// <summary>
        /// Calculates/Detects the client device IP address and returns in the form of IP declaration 
        /// </summary>
        /// <returns>IP Address in String Formatting i.e. xxx.xxx.xxx.xxx </returns>
        public static string GetClientIP()
        {
            try
            {
                // Declare a container class for Internet host address information
                //IPHostEntry host;

                // Declare string variable for storing IP Address
                string lLocalIP = string.Empty;
                // Get Host Name
                //host = System.Net.Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["REMOTE_HOST"]);

                lLocalIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (lLocalIP == "" || lLocalIP == null)
                    lLocalIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                //Code Comment By mohit as below code not provide local ID Address of cilent
                // host = Dns.GetHostEntry(Dns.GetHostName());

                //foreach (IPAddress ip in host.AddressList)
                //{
                //    if (ip.AddressFamily == AddressFamily.InterNetwork)
                //    {
                //        // Set Local IP Address
                //        lLocalIP = ip.ToString();
                //        break;
                //    }
                //}
                // Return IP Address of client machine
                return lLocalIP;
            }
            catch (Exception ex)
            {
                throw new Exception("Can't get IP Address of client device!!" + Environment.NewLine + ex.InnerException);
            }
        }
        /// <summary>
        /// This method is used to get local Date and Time with respect to UTC Time
        /// </summary>
        /// <returns>DateTime Object</returns>
        public static DateTime GetLocalTime()
        {
            // Add local time zone to current UTC date
            return DateTime.UtcNow.AddHours(5.5);
        }

        /// <summary>
        /// This method is used to get local Date and time provided the date passed is UTC
        /// </summary>
        /// <param name="dt">Get UTC date</param>
        /// <returns>DateTime Object</returns>
        public static DateTime GetLocalTime(DateTime pUTCDate)
        {
            try
            {
                // Add local time zone to UTC date pass by user
                return pUTCDate.AddHours(5.5);
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid Datetime!!" + Environment.NewLine + ex.InnerException);
            }
        }

        /// <summary>
        /// This method is used to get local datetime object provided the parameters are passed with respect to UTC
        /// </summary>
        /// <param name="pDay">Days</param>
        /// <param name="pMonth">Month</param>
        /// <param name="pYear">Year</param>
        /// <param name="pHour">Hour</param>
        /// <param name="pMinute">Minute</param>
        /// <param name="pSecond">Second</param>
        /// <returns>DateTime Object</returns>
        /// <exception cref="Exception"></exception>
        public static DateTime GetLocalTime(int pDay, int pMonth, int pYear, int pHour, int pMinute, int pSecond)
        {
            int lFlag = 0, lDayInMonth;

            try
            {
                // Check whether month is valid or not
                if (pMonth < 1 || pMonth > 12)
                    lFlag = 1;

                // Check whether year is valid or not
                else if (pYear < 1000 || pYear > 9999)
                    lFlag = 2;

                // Get Days in month
                lDayInMonth = DateTime.DaysInMonth(pYear, pMonth);

                // Check whether days is valid or not
                if (pDay < 1 || pDay > lDayInMonth)
                    lFlag = 3;

                return new DateTime(pYear, pMonth, pDay, pHour, pMinute, pSecond).AddHours(5.5);
            }
            catch (Exception ex)
            {
                if (lFlag == 1)
                    throw new Exception("Invalid Month!!");
                else if (lFlag == 2)
                    throw new Exception("Invalid Year!!");
                else if (lFlag == 3)
                    throw new Exception("Invalid Days!!");
                else
                    throw ex;
            }
        }

        //-----------------------------------------------------------------------
        // <copyright file="CommonFunctions:UploadProductImages(),UploadShopImages(),UploadShopLogo()" company="Ezeelo Consumer Services Pvt. Ltd.">
        //     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
        // </copyright>
        // <author>Snehal Shende</author>
        //-----------------------------------------------------------------------
        # region Image and File

        /// <summary>
        /// This functions upload the product images
        /// </summary>
        /// <param name="Files"> One or more HttpPostedFileBase files of images</param>
        /// <param name="pName">Product Name</param>
        /// <param name="pId">Product Id</param>
        /// <param name="pSubFolderName">Folder Name i.e. Red or Default</param>
        /// <param name="pSubFolderPrefix">Folder Prefix  i.e. Default or string.empty</param>
        /// <param name="pImgtype">Image Type i.e. Approved or NonApproved</param>
        ///<param name="IsThumb">Is Product Thumb Image</param>
        public static void UploadProductImages(List<HttpPostedFileBase> Files, string pName, long pId, string pSubFolderName, string pSubFolderPrefix, ProductUpload.IMAGE_TYPE pImgtype, int ThumbIndex)
        {
            try
            {
                ImageUpload img = new ImageUpload(System.Web.HttpContext.Current.Server);
                string rootpath = string.Empty;

                int i = 0;
                if (Files != null) //Added by Zubair on 05-06-2018
                {
                    foreach (HttpPostedFileBase file in Files)
                    {
                        if (file != null)
                        {
                            bool IsThumb = false;
                            if (file.ContentLength > 0)
                            {
                                if (i == ThumbIndex)
                                {
                                    IsThumb = true;
                                }
                                rootpath = img.AddProductImage(pName, pId, pSubFolderName, pSubFolderPrefix, file, pImgtype, IsThumb);
                            }
                        }
                        i++;
                    }
                }
                if (!string.IsNullOrEmpty(rootpath))
                {
                    DirectoryInfo lDrInfo = img.CreateDirectory(rootpath);
                    lDrInfo.Delete(true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in Uploading Product image!!" + Environment.NewLine + ex.InnerException);
            }
        }

        /// <summary>
        /// This functions edit the product thumb image
        /// </summary>
        /// <param name="pId">Product Id</param>
        /// <param name="source">full path of image of which thumb images is to be created</param>
        /// <returns>message (if fails to edit)</returns>   
        public static string EditProductThumb(long pId, string source)
        {
            string msg = string.Empty;
            try
            {
                ImageUpload img = new ImageUpload(System.Web.HttpContext.Current.Server);
                msg = img.EditProductThumb(pId, source);
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in Editing Product thumb image!!" + Environment.NewLine + ex.InnerException);
            }
            return msg;
        }


        /// <summary>
        /// This functions upload the Shop images
        /// </summary>
        /// <param name="Files">One or more HttpPostedFileBase files of images</param>
        /// <param name="pId">Shop Id</param>
        /// <param name="pImgtype">Image Type i.e. Approved or NonApproved</param>
        public static void UploadShopImages(List<HttpPostedFileBase> Files, long pId, ProductUpload.IMAGE_TYPE pImgtype)
        {
            try
            {
                ImageUpload img = new ImageUpload(System.Web.HttpContext.Current.Server);
                string rootpath = string.Empty;
                foreach (HttpPostedFileBase file in Files)
                {
                    if (file.ContentLength > 0)
                    {
                        rootpath = img.AddShopImage(pId, file, pImgtype);
                    }
                }
                DirectoryInfo lDrInfo = img.CreateDirectory(rootpath);
                lDrInfo.Delete(true);
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in Uploading Shop image!!" + Environment.NewLine + ex.InnerException);
            }
        }

        /// <summary>
        /// This functions upload the Shop Logo
        /// </summary>
        /// <param name="pFile">HttpPostedFileBase file of image</param>
        /// <param name="pId">Shop Id</param>
        /// <param name="pImgtype">Image Type i.e. Approved or NonApproved</param>
        public static void UploadShopLogo(HttpPostedFileBase pFile, long pId, ProductUpload.IMAGE_TYPE pImgtype)
        {
            try
            {
                ImageUpload img = new ImageUpload(System.Web.HttpContext.Current.Server);
                if (pFile.ContentLength > 0)
                {
                    img.AddShopLogo(pId, pFile, pImgtype);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in Uploading Shop image!!" + Environment.NewLine + ex.InnerException);
            }
        }

        public static void UploadMerchantProfile(HttpPostedFileBase pFile, long pId, ProductUpload.IMAGE_TYPE pImgtype)
        {
            try
            {
                ImageUpload img = new ImageUpload(System.Web.HttpContext.Current.Server);
                if (pFile.ContentLength > 0)
                {
                    img.AddMerchantProfile(pId, pFile, pImgtype);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in Uploading Shop image!!" + Environment.NewLine + ex.InnerException);
            }
        }

        /// <summary>
        /// This function delete shop image
        /// </summary>
        /// <param name="fileName">Filename with full path</param>
        /// <returns>true or false</returns>
        public static bool DeleteShopImages(string fileName)
        {
            try
            {
                FtpImageUpload fimg = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                return (fimg.DeleteFtpShopImage(fileName));
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception("Problem in Deleting Shop Image!!" + Environment.NewLine + ex.InnerException);
            }
        }

        /// <summary>
        /// This function delete product image
        /// </summary>
        /// <param name="fileName">Image Name</param>
        /// <param name="pId">product Id</param>
        /// <param name="pName">Product Name</param>
        /// <param name="pSubFolderName">Name of folder i.e. Stock color name or default (string)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'Default' (string)</param>
        /// <param name="pImgtype">Image Type i.e. Approved or NonApproved</param>
        /// <returns>true or false</returns>
        public static bool DeleteProductImages(string fileName, long pId, string pName, string pSubFolderName, string pSubFolderPrefix, ProductUpload.IMAGE_TYPE pImgtype)
        {
            try
            {
                FtpImageUpload fimg = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                return (fimg.DeleteFtpProductImage(fileName, pId, pName, pSubFolderName, pSubFolderPrefix, pImgtype));
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception("Problem in Deleting Product Image!!" + Environment.NewLine + ex.InnerException);
            }
        }

        /// <summary>
        /// Approves shop images
        /// </summary>
        /// <param name="pId">Shop Id</param>
        public static void ApproveShopImages(long pId)
        {
            try
            {
                ImageUpload img = new ImageUpload(System.Web.HttpContext.Current.Server);
                string rootpath = string.Empty;
                rootpath = ImageApproval.ShopImagesApproval(pId);
                DirectoryInfo lDrInfo = img.CreateDirectory(rootpath);
                lDrInfo.Delete(true);
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in Approving Shop image!!" + Environment.NewLine + ex.InnerException);
            }
        }

        /// <summary>
        /// Approves product images
        /// </summary>
        /// <param name="tempProductId">Temp product id</param>
        /// <param name="newProductId">New Product Id</param>
        /// <param name="pSubFolderName">Name of folder i.e. Stock color name or default (string)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'Default' (string)</param>
        public static void ApproveProductImages(long tempProductId, long newProductId, string pSubFolderName, string pSubFolderPrefix)
        {
            try
            {
                ImageUpload img = new ImageUpload(System.Web.HttpContext.Current.Server);
                string rootpath = string.Empty;
                rootpath = ImageApproval.ProductImagesApproval(tempProductId, newProductId, pSubFolderName.Trim(), pSubFolderPrefix.Trim());
                DirectoryInfo lDrInfo = img.CreateDirectory(rootpath);
                lDrInfo.Delete(true);
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in Approving Product image!!" + Environment.NewLine + ex.InnerException);
            }
        }

        /// <summary>
        /// Uplaod Product description
        /// </summary>
        /// <param name="pId">Product Id</param>
        /// <param name="txt">text to upload</param>
        /// <param name="pImgtype">Image Type i.e. Approved or NonApproved</param>
        public static void UploadDescFile(long pId, string txt, ProductUpload.IMAGE_TYPE pImgtype)
        {
            try
            {
                DescriptionFileUpload DescFile = new DescriptionFileUpload(System.Web.HttpContext.Current.Server);
                DescFile.FileUpload(pId, txt, pImgtype);
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in Uploading Description File !!" + Environment.NewLine + ex.InnerException);
            }
        }

        public static void UploadCareerFile(HttpPostedFileBase file, string guid, string extension)
        {
            try
            {
                DescriptionFileUpload DescFile = new DescriptionFileUpload(System.Web.HttpContext.Current.Server);
                DescFile.UploadCareerFile(file, guid, extension);
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in Uploading File !!" + Environment.NewLine + ex.InnerException);
            }
        }

        /// <summary>
        /// Loads the description file
        /// </summary>
        /// <param name="pId">Product id</param>
        /// <param name="pImgtype">Image Type i.e. Approved or NonApproved</param>
        /// <returns>path of file</returns>
        public static string LoadDescFile(long pId, ProductUpload.IMAGE_TYPE pImgtype)
        {
            try
            {
                DescriptionFileUpload DescFile = new DescriptionFileUpload(System.Web.HttpContext.Current.Server);
                return DescFile.LoadProductDescFile(pId, pImgtype);
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in Loading Description File !!" + Environment.NewLine + ex.InnerException);
            }
        }

        /// <summary>
        /// Approves the product description
        /// </summary>
        /// <param name="tempProductId">Temp product id</param>
        /// <param name="newProductId">New Product Id</param>
        public static void ApproveProductDescription(long tempProductId, long newProductId)
        {
            try
            {
                ImageUpload img = new ImageUpload(System.Web.HttpContext.Current.Server);
                string rootpath = string.Empty;
                rootpath = ImageApproval.ProductDescriptionApproval(tempProductId, newProductId);
                DirectoryInfo lDrInfo = img.CreateDirectory(rootpath);
                lDrInfo.Delete(true);
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in Approving Product Description file !!" + Environment.NewLine + ex.InnerException);
            }
        }

        /// <summary>
        /// Delete all images of current product variant
        /// </summary>
        /// <param name="pId">product ID</param>
        /// <param name="pSubFolderName">Name of folder i.e. Stock color name or default (string)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'Default' (string)</param>
        /// <param name="pImgtype">Image Type i.e. Approved or NonApproved</param>
        public static void DeleteVariantImages(long pId, string pSubFolderName, string pSubFolderPrefix, ProductUpload.IMAGE_TYPE pImgtype)
        {
            try
            {
                FtpImageUpload img = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                img.DeleteProductImages(pId, pSubFolderName, pSubFolderPrefix, pImgtype);
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in Deleting Product image!!" + Environment.NewLine + ex.InnerException);
            }
        }

        /// <summary>
        /// Delete all images of current product 
        /// </summary>
        /// <param name="pId">product ID</param>
        /// <param name="pImgtype">Image Type i.e. Approved or NonApproved</param>
        public static void DeleteAllProductImages(long pId, ProductUpload.IMAGE_TYPE pImgtype)
        {
            try
            {
                FtpImageUpload img = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                img.DeleteProductImages(pId, pImgtype);
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in Deleting Product image!!" + Environment.NewLine + ex.InnerException);
            }
        }

        public static bool UploadCategoryImage(HttpPostedFileBase pFile, Int64 cityId, Int32 FranchiseID, string fileName)////added Int32 FranchiseID
        {
            bool IsUploaded = false;
            try
            {
                ImageUpload img = new ImageUpload(System.Web.HttpContext.Current.Server);
                if (pFile.ContentLength > 0)
                {
                    IsUploaded = img.UploadCategoryImageOnServer(pFile, cityId, FranchiseID, fileName);////added  FranchiseID
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in Uploading Shop image!!" + Environment.NewLine + ex.InnerException);
            }
            return IsUploaded;
        }

        public static bool UploadHomePageDynamicSectionImage(HttpPostedFileBase pFile, Int64 cityId, Int64 FranchiseID, int HomepageDynamicSectionID, string fileName)////added Int32 FranchiseID
        {
            bool IsUploaded = false;
            try
            {
                ImageUpload img = new ImageUpload(System.Web.HttpContext.Current.Server);
                if (pFile.ContentLength > 0)
                {

                    IsUploaded = img.UploadHomePageDynamicImageOnServer(pFile, cityId, FranchiseID, HomepageDynamicSectionID, fileName);////added  FranchiseID
                }
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorLogFile("Problem in Banner upload :" + ex.Message + ex.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                throw new Exception("Problem in Uploading Home Page image!!" + Environment.NewLine + ex.InnerException);

            }
            return IsUploaded;
        }
        public static bool DeleteHomePageDynamicSectionImage(HttpPostedFileBase pFile, Int64 cityId, Int64 FranchiseID, int HomepageDynamicSectionID, string fileName)////added Int32 FranchiseID
        {
            bool IsUploaded = false;
            try
            {
                ImageUpload img = new ImageUpload(System.Web.HttpContext.Current.Server);
                if (pFile.ContentLength > 0)
                {

                    IsUploaded = img.DeleteHomePageDynamicImageOnServer(pFile, cityId, FranchiseID, HomepageDynamicSectionID, fileName);////added  FranchiseID
                }
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorLogFile("Problem in Banner upload :" + ex.Message + ex.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                throw new Exception("Problem in Uploading Home Page image!!" + Environment.NewLine + ex.InnerException);

            }
            return IsUploaded;
        }




        public static bool UploadHomePageImage(HttpPostedFileBase pFile, Int64 cityId, Int32 FranchiseID, Int64 blockTypeId, string fileName)////added Int32 FranchiseID
        {
            bool IsUploaded = false;
            try
            {
                ImageUpload img = new ImageUpload(System.Web.HttpContext.Current.Server);
                if (pFile.ContentLength > 0)
                {

                    IsUploaded = img.UploadHomePageImageOnServer(pFile, cityId, FranchiseID, blockTypeId, fileName);////added  FranchiseID
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in Uploading Home Page image!!" + Environment.NewLine + ex.InnerException);
            }
            return IsUploaded;
        }

        //***New Method added for deleting existing image from FTP server by Harshada2***//
        public static bool DeleteHomePageImage(HttpPostedFileBase pFile, Int64 cityId, Int32 FranchiseID, Int64 blockTypeId, string fileName)////added Int32 FranchiseID
        {
            bool IsUploaded = false;
            try
            {
                ImageUpload img = new ImageUpload(System.Web.HttpContext.Current.Server);
                if (pFile.ContentLength > 0)
                {

                    IsUploaded = img.DeleteHomePageImageOnServer(pFile, cityId, FranchiseID, blockTypeId, fileName);////added  FranchiseID
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in Uploading Home Page image!!" + Environment.NewLine + ex.InnerException);
            }
            return IsUploaded;
        }

        #endregion
        /******************************************************************************************************/

        public static long GetPersonalDetailsID(long userLoginID)
        {
            long personalDetailID = db.PersonalDetails.Where(x => x.UserLoginID == userLoginID).FirstOrDefault().ID;

            return personalDetailID;
        }

        public static bool IsValidEmailId(string pInputEmail)
        {
            //Regex To validate Email Address
            if (pInputEmail != string.Empty && pInputEmail != null)
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(pInputEmail);
                if (match.Success)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        public static bool IsValidMobile(string pInputMobile)
        {
            //Regex To validate Email Address

            if (pInputMobile != string.Empty && pInputMobile != null)
            {
                Regex regex = new Regex(@"^[6-9]{1}[0-9]{9}$");
                Match match = regex.Match(pInputMobile);
                if (match.Success)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static string ConvertStringToCamelCase(string pTitle)
        {
            try
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                pTitle = textInfo.ToTitleCase(pTitle.ToLower());

                return pTitle;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[BusinessLogicLayer][C:CommonFunctions]", "Can't convert string to camel case !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[BusinessLogicLayer][C:CommonFunctions]", "Can't convert string to camel case !" + Environment.NewLine + ex.Message);
            }
        }

        public static string CheckUserDetails(string pEmail, string pMobile)
        {

            //Contains message for unique key exists or not
            string lMessage = string.Empty;
            try
            {
                // Check email and mobile exists or not
                //Previous code
                //var lEmailAndMobileInDB = db.UserLogins.Count(x => x.Email == pEmail && x.Mobile == pMobile);
                //if (lEmailAndMobileInDB > 0)
                //{
                //    //lMessage = "Email and Mobile No. already exist!!";
                //    lMessage = "You are already registered with eZeelo. Please login to continue!!";
                //    return lMessage;
                //}

                //Code change by Tejaswee
                var lEmailAndMobileInDB = (from user in db.UserLogins
                                           where user.Email == pEmail && user.Mobile == pMobile
                                           select new
                                           {
                                               user.ID,
                                               user.Email,
                                               user.Mobile
                                           }).ToList().FirstOrDefault();

                if (lEmailAndMobileInDB != null)
                {
                    var CustDetails = db.PersonalDetails.Where(x => x.UserLoginID == lEmailAndMobileInDB.ID).FirstOrDefault();
                    lMessage = "Hi, " + CustDetails.FirstName + "You are already registered with " + lEmailAndMobileInDB.Email;
                    //lMessage = "You are already registered with eZeelo. Please login to continue!!";

                    return lMessage;
                }

                // Check email exists or not

                /*to all null in email
                 * Pradnyakar Badge                 
                 */
                if (pEmail != null)
                {
                    //var lEmailInDB = db.UserLogins.Count(x => x.Email == pEmail);
                    var lEmailInDB = (from user in db.UserLogins
                                      where user.Email == pEmail
                                      select new
                                      {
                                          user.ID,
                                          user.Email,
                                      }).ToList().FirstOrDefault();
                    if (lEmailInDB != null)
                    {
                        var CustDetails = db.PersonalDetails.Where(x => x.UserLoginID == lEmailInDB.ID).FirstOrDefault();
                        lMessage = "Hi, " + CustDetails.FirstName + "You are already registered with " + lEmailInDB.Email;
                        //lMessage = "You are already registered with eZeelo. Please login to continue!!";
                        return lMessage;
                    }
                }
                // Check mobile exists or not
                if (pMobile != null && pMobile != "")
                {
                    //var lMobileInDB = db.UserLogins.Count(x => x.Mobile == pMobile);
                    var lMobileInDB = (from user in db.UserLogins
                                       where user.Mobile == pMobile
                                       select new
                                       {
                                           user.ID,
                                           user.Mobile,
                                       }).ToList().FirstOrDefault();
                    if (lMobileInDB != null)
                    {
                        var CustDetails = db.PersonalDetails.Where(x => x.UserLoginID == lMobileInDB.ID).FirstOrDefault();
                        lMessage = "Hi, " + CustDetails.FirstName + "You are already registered with " + lMobileInDB.Mobile;
                        //lMessage = "You are already registered with eZeelo. Please login to continue!!";
                        return lMessage;
                    }
                }
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[BusinessLogicLayer][C:CommonFunctions]", "Can't check user details!" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[BusinessLogicLayer][C:CommonFunctions]", "Can't check user details!" + Environment.NewLine + ex.Message);
            }

            return lMessage;
        }


        //created by tejaswee
        public static string CheckUserDetails(string pEmail, string pMobile, out string mobEmail)
        {

            //Contains message for unique key exists or not
            string lMessage = string.Empty;
            mobEmail = string.Empty;
            try
            {
                // Check email and mobile exists or not
                //Previous code
                //var lEmailAndMobileInDB = db.UserLogins.Count(x => x.Email == pEmail && x.Mobile == pMobile);
                //if (lEmailAndMobileInDB > 0)
                //{
                //    //lMessage = "Email and Mobile No. already exist!!";
                //    lMessage = "You are already registered with eZeelo. Please login to continue!!";
                //    return lMessage;
                //}
                if (pEmail != null && pMobile != null)
                {
                    //Code change by Tejaswee
                    var lEmailAndMobileInDB = (from user in db.UserLogins
                                               where user.Email == pEmail && user.Mobile == pMobile
                                               select new
                                               {
                                                   user.ID,
                                                   user.Email,
                                                   user.Mobile
                                               }).ToList().FirstOrDefault();

                    if (lEmailAndMobileInDB != null)
                    {
                        var CustDetails = db.PersonalDetails.Where(x => x.UserLoginID == lEmailAndMobileInDB.ID).FirstOrDefault();
                        lMessage = "Hi, " + CustDetails.FirstName + " you are already registered with " + lEmailAndMobileInDB.Email;
                        //lMessage = "You are already registered with eZeelo. Please login to continue!!";
                        mobEmail = lEmailAndMobileInDB.Email;
                        return lMessage;
                    }
                }
                // Check email exists or not

                /*to all null in email
                 * Pradnyakar Badge                 
                 */
                if (pEmail != null)
                {
                    //var lEmailInDB = db.UserLogins.Count(x => x.Email == pEmail);
                    var lEmailInDB = (from user in db.UserLogins
                                      where user.Email == pEmail
                                      select new
                                      {
                                          user.ID,
                                          user.Email,
                                      }).ToList().FirstOrDefault();
                    if (lEmailInDB != null)
                    {
                        var CustDetails = db.PersonalDetails.Where(x => x.UserLoginID == lEmailInDB.ID).FirstOrDefault();
                        lMessage = "Hi, " + CustDetails.FirstName + " you are already registered with " + lEmailInDB.Email;
                        //lMessage = "You are already registered with eZeelo. Please login to continue!!";
                        mobEmail = lEmailInDB.Email;
                        return lMessage;
                    }
                }
                // Check mobile exists or not
                if (pMobile != null && pMobile != "")
                {
                    //var lMobileInDB = db.UserLogins.Count(x => x.Mobile == pMobile);
                    var lMobileInDB = (from user in db.UserLogins
                                       where user.Mobile == pMobile
                                       select new
                                       {
                                           user.ID,
                                           user.Mobile,
                                       }).ToList().FirstOrDefault();
                    if (lMobileInDB != null)
                    {
                        var CustDetails = db.PersonalDetails.Where(x => x.UserLoginID == lMobileInDB.ID).FirstOrDefault();
                        lMessage = "Hi, " + CustDetails.FirstName + " you are already registered with " + lMobileInDB.Mobile;
                        //lMessage = "You are already registered with eZeelo. Please login to continue!!";
                        mobEmail = lMobileInDB.Mobile;
                        return lMessage;
                    }
                }
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[BusinessLogicLayer][C:CommonFunctions]", "Can't check user details!" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[BusinessLogicLayer][C:CommonFunctions]", "Can't check user details!" + Environment.NewLine + ex.Message);
            }

            return lMessage;
        }

        public static DateTime? GetDate(string d)
        {
            try
            {
                if (!string.IsNullOrEmpty(d))
                {
                    string[] arr = d.Split(new char[] { '/', '-' }, StringSplitOptions.RemoveEmptyEntries);
                    return new DateTime(Convert.ToInt16(arr[2]), Convert.ToInt16(arr[1]), Convert.ToInt16(arr[0]));
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public static DateTime GetProperDate(string d)
        {
            try
            {
                if (!string.IsNullOrEmpty(d))
                {
                    string[] arr = d.Split(new char[] { '/', '-' }, StringSplitOptions.RemoveEmptyEntries);
                    return new DateTime(Convert.ToInt16(arr[2]), Convert.ToInt16(arr[1]), Convert.ToInt16(arr[0]));
                }
            }
            catch
            {
                return DateTime.Now;
            }
            return DateTime.Now;
        }

        public static DateTime? GetDateTime(string d)
        {
            try
            {
                if (!string.IsNullOrEmpty(d))
                {
                    string[] arr = d.Split(new char[] { '/', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    return new DateTime(Convert.ToInt16(arr[2]), Convert.ToInt16(arr[1]), Convert.ToInt16(arr[0]), DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public static DateTime GetProperDateTime(string d)
        {
            try
            {
                string[] arr = d.Split(new char[] { '/', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return new DateTime(Convert.ToInt16(arr[2]), Convert.ToInt16(arr[1]), Convert.ToInt16(arr[0]), DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public static DateTime GetExactDateTime(string d)
        {
            try
            {
                string[] arr = d.Split(new char[] { '/', '-', ' ', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
                string newDate = Convert.ToInt32(arr[2]) + "/" + Convert.ToInt16(arr[1]) + "/" + Convert.ToInt16(arr[0]) + " " + Convert.ToInt16(arr[3]) + ":" + Convert.ToInt16(arr[4]) + ":00 " + arr[5];
                DateTime dt = Convert.ToDateTime(newDate);
                // return new DateTime(Convert.ToInt32(arr[2]), Convert.ToInt16(arr[1]), Convert.ToInt16(arr[0]), Convert.ToInt16(arr[3]), Convert.ToInt16(arr[4]), 0);
                return dt;
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public static int GetCatNameID(long pID, ref string cName)
        {
            try
            {
                int cID = 0;
                var catDetails = db.Products.Find(pID).Category;
                cID = catDetails.ID;
                cName = catDetails.Name;
                return cID;
            }
            catch
            {
                return 0;
            }
        }

        public static string GetCategoryDescription(string CategoryName, SEOManagement.CATEGEORY_LEVEL level)
        {
            try
            {
                BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);
                return GetDescriptionContent(rcKey.FILE_HTTP + "/" + level + "/" + CategoryName);
            }
            catch { return string.Empty; }
        }

        private static string GetDescriptionContent(string name)
        {
            try
            {
                string contentUri = name + ".txt";

                System.Net.HttpWebRequest webReq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(new Uri(contentUri));

                if (webReq.GetResponse().ContentLength > 0)
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(webReq.GetResponse().GetResponseStream());

                    return sr.ReadToEnd();
                }

                return string.Empty;

            }
            catch
            {
                // no content found or theres something wrong getting seo content
                return string.Empty; // return string.empty coz there should not be page blocker error due to seo exceptions
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string GetCustCareNo()
        {
            string telNo = string.Empty;
            try
            {
                long cityID = 4968;
                if (HttpContext.Current.Request.Cookies["CityCookie"] != null && HttpContext.Current.Request.Cookies["CityCookie"].Value != "")
                {
                    cityID = Convert.ToInt32(HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[0]);
                }
                telNo = db.HelpDeskDetails.Where(x => x.CityID == cityID).Select(x => x.HelpLineNumber).FirstOrDefault();
                //TempData["CustCareNo"] = telNo;
            }
            catch (Exception)
            {

                throw;
            }
            return telNo;
        }
        /// <summary>
        /// Sent OTP
        /// </summary>
        /// <param name="EmailId"></param>
        /// <param name="MobileNo"></param>
        /// <param name="Name"></param>
        /// <param name="SentOtpToEmail"></param>
        /// <param name="SentOtpToMobile"></param>
        /// <returns></returns>
        public static int SendOTP(string EmailId, string MobileNo, string Name, bool SentOtpToEmail, bool SentOtpToMobile)
        {
            EzeeloDBContext db = new EzeeloDBContext();
            int Returnvalue = 0;

            int OTPExpireTimeInMinutes = 15;
            int OTPLength = 6;
            int RegenerateOTPLimit = 5;
            if (WebConfigurationManager.AppSettings["OTP_LENGTH"] != null)
            {
                OTPLength = Convert.ToInt32(WebConfigurationManager.AppSettings["OTP_LENGTH"]);
            }
            if (WebConfigurationManager.AppSettings["OTP_EXPIRE_TIME_IN_MINUTES"] != null)
            {
                OTPExpireTimeInMinutes = Convert.ToInt32(WebConfigurationManager.AppSettings["OTP_EXPIRE_TIME_IN_MINUTES"]);
            }
            if (WebConfigurationManager.AppSettings["REGENERATE_OTP_LIMIT"] != null)
            {
                RegenerateOTPLimit = Convert.ToInt32(WebConfigurationManager.AppSettings["REGENERATE_OTP_LIMIT"]);
            }
            List<OTPLog> OldLogs = db.OTPLogs.Where(x => (x.Email == EmailId || x.Mobile == MobileNo) && DbFunctions.TruncateTime(x.CreateDate) == DbFunctions.TruncateTime(DateTime.Now)).ToList();

            if (OldLogs.Where(x => x.IsValidated).Count() > 0)
            {
                return Returnvalue;
            }
            string MobileOTP = GetRandomNumberOTP(OTPLength);
            string EmailOTP = MobileOTP; //Send same otp on EMail as in SMS //GetRandomNumberOTP(OTPLength);
            if (Convert.ToInt32(OldLogs.Count) < RegenerateOTPLimit)
            {

                foreach (OTPLog o in OldLogs)
                {
                    o.IsActive = false;
                }
                SendOtpToEmailnMobileNumber(EmailId, MobileNo, Name, EmailOTP, MobileOTP, SentOtpToEmail, SentOtpToMobile);


                OTPLog RegisterationOTPLog = new OTPLog();
                RegisterationOTPLog.Email = EmailId;
                RegisterationOTPLog.Mobile = MobileNo;
                RegisterationOTPLog.Name = Name;
                RegisterationOTPLog.EmailOTP = EmailOTP;
                RegisterationOTPLog.MobileOTP = MobileOTP;
                RegisterationOTPLog.IsActive = true;
                RegisterationOTPLog.CreateDate = CommonFunctions.GetLocalTime();
                RegisterationOTPLog.OTPExpire = DateTime.Now.AddMinutes(OTPExpireTimeInMinutes);
                RegisterationOTPLog.CreateBy = 1;
                RegisterationOTPLog.NetworkIP = CommonFunctions.GetClientIP();
                db.OTPLogs.Add(RegisterationOTPLog);
                Returnvalue = 1;
            }
            else if (Convert.ToInt32(OldLogs.Count) == RegenerateOTPLimit)
            {
                //"OTP regenerate limit exceeds. Send last OTP to Admin with increased validity. Send sms to customer notify limit has been exceeded.";

                OTPLog RegisterationOTPLog = new OTPLog();
                RegisterationOTPLog.Email = EmailId;
                RegisterationOTPLog.Mobile = MobileNo;
                RegisterationOTPLog.Name = Name;
                RegisterationOTPLog.EmailOTP = EmailOTP;
                RegisterationOTPLog.MobileOTP = MobileOTP;
                RegisterationOTPLog.IsActive = true;
                RegisterationOTPLog.CreateDate = CommonFunctions.GetLocalTime();
                RegisterationOTPLog.OTPExpire = DateTime.Now.AddMinutes(60);
                RegisterationOTPLog.CreateBy = 1;
                RegisterationOTPLog.NetworkIP = CommonFunctions.GetClientIP();
                db.OTPLogs.Add(RegisterationOTPLog);
                SendOtpToEmailnMobileNumberToAdmin(EmailId, MobileNo, Name, EmailOTP, MobileOTP, RegisterationOTPLog.OTPExpire.ToString());
                SendLimitExceedsSMSToCustomer(MobileNo, Name);

                Returnvalue = 2;

            }
            else
            {
                Returnvalue = -3;
            }
            if (Returnvalue > 0)
            {
                foreach (OTPLog o in OldLogs)
                {
                    o.IsActive = false;
                }
                db.SaveChanges();
            }

            return Returnvalue;
        }
        /// <summary>
        /// Send OTP to email and mobile
        /// </summary>
        /// <param name="EmailID"></param>
        /// <param name="PhoneNumber"></param>
        /// <param name="Name"></param>
        /// <param name="EmailOTP"></param>
        /// <param name="MobileOTP"></param>
        /// <returns></returns>
        static bool SendOtpToEmailnMobileNumber(string EmailID, string PhoneNumber, string Name, string EmailOTP, string MobileOTP, bool SentOtpToEmail, bool SentOtpToMobile)
        {
            bool IsSend = false;

            try
            {

                if (SentOtpToEmail)
                {
                    try
                    {

                        Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                        dictEmailValues.Add("<!--Name-->", Name);
                        dictEmailValues.Add("<!--OTP-->", EmailOTP);
                        BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                        gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH,
                            BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.CUST_OTP_REG,
                            new string[] { EmailID }, dictEmailValues, true);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                if (SentOtpToMobile)
                {
                    try
                    {


                        Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                        BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                        dictSMSValues.Add("#--NAME--#", Name);
                        dictSMSValues.Add("#--OTP--# ", MobileOTP);
                        gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
                            BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.OTP_CUST_REG,
                            new string[] { PhoneNumber }, dictSMSValues);

                    }
                    catch (Exception ex)
                    {
                    }
                }
                IsSend = true;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        static bool SendOtpToEmailnMobileNumberToAdmin(string EmailID, string PhoneNumber, string Name, string EmailOTP, string MobileOTP, string OPTExpired)
        {
            bool IsSend = false;

            try
            {


                try
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(WebConfigurationManager.AppSettings["ADMIN_EMAIL"])))
                    {
                        Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();

                        dictEmailValues.Add("<!--CUSTOMER_EMAIL-->", EmailID);
                        dictEmailValues.Add("<!--CUSTOMER_NAME-->", Name);
                        dictEmailValues.Add("<!--CUSTOMER_MOBILE-->", PhoneNumber);
                        dictEmailValues.Add("<!--CUSTOMER_EMAIL_OTP-->", EmailOTP);
                        dictEmailValues.Add("<!--CUSTOMER_MOBILE_OTP-->", MobileOTP);
                        dictEmailValues.Add("<!--CUSTOMER_OTP_EXPIRED-->", OPTExpired);
                        BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                        gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH,
                            BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.ADM_CUST_REG_LIM_EXDS,
                            new string[] { Convert.ToString(WebConfigurationManager.AppSettings["ADMIN_EMAIL"]) }, dictEmailValues, true);
                    }
                }
                catch (Exception ex)
                {
                }

                try
                {

                    if (!string.IsNullOrEmpty(Convert.ToString(WebConfigurationManager.AppSettings["ADMIN_MOBILE_NUMBER"])))
                    {
                        Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                        BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                        dictSMSValues.Add("#--CUSTOMER_NAME--#", Name);
                        dictSMSValues.Add("#--CUSTOMER_MOBILE--#", PhoneNumber);
                        dictSMSValues.Add("#--CUSTOMER_EMAIL--#", EmailID);
                        dictSMSValues.Add("#--CUSTOMER_EMAIL_OTP--#", EmailOTP);
                        dictSMSValues.Add("#--CUSTOMER_MOBILE_OTP--#", MobileOTP);
                        dictSMSValues.Add("#--CUSTOMER_OTP_EXPIRED--#", OPTExpired);
                        gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
                            BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.ADM_CUST_REG_LIM_EXDS,
                            new string[] { Convert.ToString(WebConfigurationManager.AppSettings["ADMIN_MOBILE_NUMBER"]) }, dictSMSValues);
                    }


                }
                catch (Exception ex)
                {
                }

                IsSend = true;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }

        static bool SendLimitExceedsSMSToCustomer(string PhoneNumber, string Name)
        {
            bool IsSend = false;

            try
            {




                try
                {

                    if (!string.IsNullOrEmpty(PhoneNumber))
                    {
                        Dictionary<string, string> dictSMSValues = new Dictionary<string, string>();
                        BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.SMS(System.Web.HttpContext.Current.Server);
                        dictSMSValues.Add("#--NAME--#", Name);
                        gateWay.SendSMS(BusinessLogicLayer.GateWay.SMSGateWays.SUMIT,
                            BusinessLogicLayer.GateWay.SMSOptions.SINGLE, BusinessLogicLayer.GateWay.SMSTypes.CUST_REG_LIM_EXDS,
                            new string[] { PhoneNumber }, dictSMSValues);
                    }


                }
                catch (Exception ex)
                {
                }

                IsSend = true;
            }
            catch (Exception ex)
            {
                IsSend = false;
            }
            return IsSend;
        }
        /// <summary>
        /// Generates Random number for OTP
        /// </summary>
        /// <param name="Length">Number of characters to return</param>
        /// <returns></returns>
        public static string GetRandomNumberOTP(int Length)
        {
            //string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //string small_alphabets = "abcdefghijklmnopqrstuvwxyz";
            string numbers = "1234567890";

            //string characters = alphabets + small_alphabets + numbers;


            string otp = string.Empty;
            for (int i = 0; i < Length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, numbers.Length);
                    character = numbers.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }
            return otp;
        }
    }
}
