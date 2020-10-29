//-----------------------------------------------------------------------
// <copyright file="ImageDisplay.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Snehal Shende</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using ModelLayer.Models.ViewModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace BusinessLogicLayer
{
    public static class ImageDisplay //: ProductDisplay
    {
        #region Shop Logo

        /// <summary>
        /// This function load the Logo image of perticular Shop
        /// </summary>
        /// <param name="pId">Shop Id</param>
        /// <param name="pImgtype">Image type i.e Approved or NonApproved</param>
        /// <returns>string image path</returns>
        public static string LoadShopLogo(long pId, ProductUpload.IMAGE_TYPE pImgtype)
        {
            //~/APPROVED_IMAGES/SHOPS/1001
            //Create path of logo for requested shop id
            //return set path
            string lImgPath = string.Empty;
            try
            {
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                StringBuilder lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Shops.GetDescription() + "/" + pId);   //pId is used for Shop Id
               // lImgPath = GetShopLogo(imgPath, lpath);
                lImgPath = imgPath.IMAGE_HTTP + lpath + "/logo.png";

                //If image is not present
                //if (string.IsNullOrEmpty(lImgPath))
                //{
                //    lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype + "_Images/" + ProductUpload.IMAGE_FOR.Shops + "/No_thumbnail.png");
                //    lImgPath = imgPath.IMAGE_HTTP + lpath;
                //}
                return lImgPath.ToLower().Trim();
            }
            catch (Exception ex)
            {
                lImgPath = null;
                // throw new Exception("Probem in loading Shop image!!" + Environment.NewLine + ex.InnerException);
            }
            return null;
        }
                
        /// <summary>
        /// this function is used to access path of shop logo from server (not in use)
        /// </summary>
        /// <param name="imgPath">ReadConfig Object to access key from web.config</param>
        /// <param name="lpath">fixed path of image upto shopid (StringBuilder)</param>        //According to hirerachy of image folder
        /// <returns>Path of Logo in string</returns>
        private static string GetShopLogo(ReadConfig imgPath, System.Text.StringBuilder lpath)
        {
            StringBuilder sb = new StringBuilder();
            string lImgPath = string.Empty;
            try
            {
                //Check whether specified path is exists on server
                if (IsPathExists(imgPath, lpath))
                {
                    byte[] buffer = new byte[150000];
                    string tempString = null;
                    int count = 0;

                    // Create the WebRequest Instance
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(imgPath.IMAGE_HTTP + lpath.ToString());
                    // Query for the response
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    // Response captured in data stream
                    Stream responseStream = response.GetResponseStream();
                    do
                    {
                        // Read the response stream
                        count = responseStream.Read(buffer, 0, buffer.Length);
                        if (count != 0)
                        {
                            // Convert from bytes to ASCII text
                            tempString = Encoding.ASCII.GetString(buffer, 0, count);
                            sb.Append(tempString);
                        }
                    }
                    while (count > 0);
                    responseStream.Close();
                    response.Close();

                    //string expression = @"A[^>]*?HREF\s*=\s*[""']?" + @"([^'"" >]+?)[ '""]?>";
                    string expression = @"<A[^>]*?HREF\s*=\s*[""']?" + @"([^'"" >]+?)[ '""]?>";
                    Regex regEx = new Regex(expression, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    MatchCollection matches = regEx.Matches(sb.ToString());
                    foreach (Match match in matches)
                    {
                        string href = Regex.Match(match.Value, "([\"\"'](?<url>.*?)[\"\"'])", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                        if (href.ToUpper().Contains("LOGO"))
                        {
                            href = href.Replace("\"", "");
                            //lImgPath = imgPath.IMAGE_HTTP + href;
                            lImgPath = "http://" + imgPath.IMAGE_HTTP.Remove(0, "http://".Length).Substring(0, imgPath.IMAGE_HTTP.LastIndexOf(@"/") - "http://".Length) + href;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lImgPath = string.Empty;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageDisplay][GetShopLogo]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return lImgPath.ToLower().Trim();
        }

        /// <summary>
        /// This function check whether given path of folder is exist or not on server (For Shop)
        /// </summary>
        /// <param name="imgPath">ReadConfig Object to access key from web.config</param>
        /// <param name="lpath">path of folder (StringBuilder)</param>
        /// <returns>boolean value indicating path is exists or not</returns>
        private static bool IsPathExists(BusinessLogicLayer.ReadConfig imgPath, System.Text.StringBuilder lpath)
        {
            // check whether specified path is exist on server using http
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(imgPath.IMAGE_HTTP + lpath.ToString() + "/");
                try
                {
                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    //if file exists
                    if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                }
                catch (WebException wex)
                {
                    return false;
                }
            }
            catch
            {
                //file not found or path not exists 
                return false;
            }
            return false;
        }
        #endregion

        # region Product Thumbnail

        /// <summary>
        /// This function load thumbnail image of perticular product as per selected thumb type (not in use)
        /// </summary>
        /// <param name="pId">Product ID</param>
        /// <param name="pSubFolderName">Sub folder Name of product according to stock (i.e. Color Name or default)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'Default'</param>
        /// <param name="pThumbType">Thumbnail type, for mobile : MM, for other device : SD </param>
        /// <param name="pImgtype">Image type i.e Approved or NonApproved</param>
        /// <returns>string image path</returns>
        public static string LoadProductThumbnails(long pId, string pSubFolderName, string pSubFolderPrefix, ProductUpload.THUMB_TYPE pThumbType, ProductUpload.IMAGE_TYPE pImgtype)
        {
            //comment
            /*
             * 1. create path for image to display
             * 2. concat the image lpath with file name
             * again contact image http server path with lpath 
             */

            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/DEFAULT
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/RED
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/DEFAULT_GREEN
            //CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dr2["CATEGORY NAME"].ToString().ToLower()) 
            string lImgPath = string.Empty;
            BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
            StringBuilder lpath = null;
            try
            {
                lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + pId.ToString());
                lpath.Append("/" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderPrefix.Trim().ToLower()));
                //folder prefix is empty
                if (string.IsNullOrEmpty(pSubFolderPrefix.Trim()))
                {
                    lpath.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()));
                }
                //folder prefix and folder name are not empty
                else if (!string.IsNullOrEmpty(pSubFolderName.Trim()))
                {
                    lpath.Append("_" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()));
                }

                lImgPath = GetProductThumbnail(imgPath, lpath, pSubFolderName, pSubFolderPrefix, pThumbType);
                //If image is not present
                if (string.IsNullOrEmpty(lImgPath))
                {
                    //lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype + "_Images/" + ProductUpload.IMAGE_FOR.Products + "/No_thumbnail.png");
                    //lImgPath = imgPath.IMAGE_HTTP + lpath;

                    ImageUpload imgu = new ImageUpload(System.Web.HttpContext.Current.Server);
                    lpath.Append("/" + imgu.GetProductThumbName(pId.ToString().Trim()));

                    if (imgPath.IMAGE_HTTP.EndsWith("/"))
                    {
                        lImgPath=imgPath.IMAGE_HTTP + lpath;
                    }
                    else
                    {
                        lImgPath=imgPath.IMAGE_HTTP + "/" + lpath;
                    }
                }
                return lImgPath.ToLower().Trim();
            }
            catch (Exception ex)
            {
                lImgPath = string.Empty;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageDisplay][LoadProductThumbnails]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return lImgPath;
        }

        /// <summary>
        /// This function load thumbnail image of perticular product as per selected thumb type (not in use)
        /// </summary>
        /// <param name="pId">Product ID</param>
        /// <param name="pSubFolderName">Sub folder Name of product according to stock (i.e. Color Name or default)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'Default'</param>
        /// <param name="pThumbType">Thumbnail type, for mobile : MM, for other device : SD </param>
        /// <param name="pImgtype">Image type i.e Approved or NonApproved</param>
        /// <param name="imgPathType">Type of return path of imges i.e. Url Path or Physical Path </param>
        /// <returns>string image path</returns>
        public static string LoadProductThumbnails(long pId, string pSubFolderName, string pSubFolderPrefix, ProductUpload.THUMB_TYPE pThumbType)
        {
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/DEFAULT
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/RED
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/DEFAULT_GREEN
            //CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dr2["CATEGORY NAME"].ToString().ToLower()) 
            string lImgPath = string.Empty;
            try
            {
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                StringBuilder lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + ProductUpload.IMAGE_TYPE.Approved.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + pId.ToString());
                lpath.Append("/" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderPrefix.Trim().ToLower()));
                //folder prefix is empty
                if (string.IsNullOrEmpty(pSubFolderPrefix.Trim()))
                {
                    lpath.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()));
                }
                //folder prefix and folder name are not empty
                else if (!string.IsNullOrEmpty(pSubFolderName.Trim()))
                {
                    lpath.Append("_" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()));
                }

                lImgPath = GetProductThumbnail(imgPath, lpath, pSubFolderName, pSubFolderPrefix, pThumbType);
                //If image is not present
                if (string.IsNullOrEmpty(lImgPath))
                {
                    //lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + ProductUpload.IMAGE_TYPE.Approved + "_Images/" + ProductUpload.IMAGE_FOR.Products + "/No_thumbnail.png");
                    //lImgPath = imgPath.IMAGE_HTTP + lpath;

                    ImageUpload imgu = new ImageUpload(System.Web.HttpContext.Current.Server);
                    lpath.Append("/" + imgu.GetProductThumbName(pId.ToString().Trim()));

                    if (imgPath.IMAGE_HTTP.EndsWith("/"))
                    {
                        lImgPath = imgPath.IMAGE_HTTP + lpath;
                    }
                    else
                    {
                        lImgPath = imgPath.IMAGE_HTTP + "/" + lpath;
                    }
                }
                return lImgPath.ToLower().Trim();
            }
            catch (Exception ex)
            {
                lImgPath = string.Empty;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageDisplay][LoadProductThumbnails]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return lImgPath;
        }

        /// <summary>
        /// this function gets the thumbnail path of product images as per thumb type from server (not in use)
        /// </summary>
        /// <param name="imgPath">ReadConfig Object to access key from web.config</param>
        /// <param name="lpath">fixed path of image upto product id (StringBuilder)</param>
        /// <param name="pSubFolderName">Sub folder Name of product according to stock (i.e. Color Name or default)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'Default'</param>
        /// <param name="pThumbType">Thumbnail type, for mobile : MM, for other device : SD</param>
        /// <returns>string thumbnail path</returns>
        private static string GetProductThumbnail(ReadConfig imgPath, System.Text.StringBuilder lpath, string pSubFolderName, string pSubFolderPrefix, ProductUpload.THUMB_TYPE pThumbType)
        {
            StringBuilder sb = new StringBuilder();
            string lImgPath = string.Empty;
            try
            {
                //Check whether specified path is exists on server
                if (IsPathExists(imgPath, ref lpath, pSubFolderName.Trim(), pSubFolderPrefix.Trim()))
                {
                    byte[] buffer = new byte[2000000];
                    string tempString = null;
                    int count = 0;

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(imgPath.IMAGE_HTTP + lpath.ToString());
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    do
                    {
                        count = responseStream.Read(buffer, 0, buffer.Length);
                        if (count != 0)
                        {
                            tempString = Encoding.ASCII.GetString(buffer, 0, count);
                            sb.Append(tempString);
                        }
                    }
                    while (count > 0);
                    responseStream.Close();
                    response.Close();

                    string expression = @"<A[^>]*?HREF\s*=\s*[""']?" + @"([^'"" >]+?)[ '""]?>";
                    Regex regEx = new Regex(expression, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    MatchCollection matches = regEx.Matches(sb.ToString());
                    foreach (Match match in matches)
                    {
                        string href = Regex.Match(match.Value, "([\"\"'](?<url>.*?)[\"\"'])", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                        if (href.ToLower().Contains("_" + pThumbType.ToString().ToLower() + "."))
                        {
                            href = href.Replace("\"", "");
                            lImgPath = "http://" + imgPath.IMAGE_HTTP.Remove(0, "http://".Length).Substring(0, imgPath.IMAGE_HTTP.LastIndexOf(@"/") - "http://".Length) + href;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lImgPath = string.Empty;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageDisplay][GetProductThumbnail]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return lImgPath.ToLower().Trim();
        }


        /// <summary>
        /// This function check whether given path of folder is exist or not on server (For Product)
        /// </summary>
        /// <param name="imgPath">ReadConfig Object to access key from web.config</param>
        /// <param name="lpath">path of folder (StringBuilder)</param>
        /// <param name="pSubFolderName">Name of folder i.e. Stock color name or default (string)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'Default' (string)</param>
        /// <returns>boolean value indicating path is exists or not</returns> 
        private static bool IsPathExists(BusinessLogicLayer.ReadConfig imgPath, ref System.Text.StringBuilder lpath, string pSubFolderName, string pSubFolderPrefix)
        {
            //Comment
            /*
             * 1. check wheather folder is exist on ftp server or not 
             * 2. if not present check for other combination "default_foldername"
             */
            try
            {
                lpath = new StringBuilder(lpath.ToString().ToLower().Trim());
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(imgPath.IMAGE_HTTP + lpath.ToString() + "/");
                // checking for two different combination        
                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        //if file exists
                        if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                        {
                            return true;
                        }
                    }
                    catch (WebException wex)
                    {
                        if (wex.Response != null)
                        {
                            HttpWebResponse response = (HttpWebResponse)wex.Response;
                            if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                //check for other combination "default_foldername"
                                if (i == 0 && pSubFolderPrefix.ToLower().Trim().Equals(string.Empty) && pSubFolderName.ToLower().Trim() != "default")
                                {
                                    lpath.Replace(pSubFolderName.ToLower().Trim(), "default_" + pSubFolderName.ToLower().Trim());
                                    httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(new Uri(imgPath.IMAGE_HTTP + lpath.ToString()));
                                }
                                ////check for "default" folder
                                //if (i == 1 && pSubFolderPrefix.ToLower().Trim().Equals(string.Empty) && pSubFolderName.ToLower().Trim() != "default")
                                //{
                                //    lpath.Replace("default_" + pSubFolderName.ToLower().Trim(), "default");
                                //    httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(new Uri(imgPath.IMAGE_HTTP + lpath.ToString()));
                                //}
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageDisplay][IsPathExists]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                //file not found or path not exists 
                return false;
            }
            return false;
        }

        /// <summary>
        /// This function set thumb image of perticular product (not in use)
        /// </summary>
        /// <param name="pId">Product ID</param>
        /// <param name="pSubFolderName">Sub folder Name of product according to stock (i.e. Color Name or default)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'Default'</param>
        /// <param name="pImgtype">Image type i.e Approved or NonApproved</param>
        /// <returns>string image path</returns>
        public static string SetProductThumbImage(long pId, string pSubFolderName, string pSubFolderPrefix, ProductUpload.IMAGE_TYPE pImgtype)
        {
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/DEFAULT
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/RED
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/DEFAULT_GREEN
            //CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dr2["CATEGORY NAME"].ToString().ToLower()) 
            string lImgPath = string.Empty;
            try
            {
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                StringBuilder lpath = new StringBuilder(ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + pId.ToString());
                lpath.Append("/" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderPrefix.Trim().ToLower()));
                //folder prefix is empty
                if (string.IsNullOrEmpty(pSubFolderPrefix.Trim()))
                {
                    lpath.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()));
                }
                //folder prefix and folder name are not empty
                else if (!string.IsNullOrEmpty(pSubFolderName.Trim()))
                {
                    lpath.Append("_" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()));
                }

                ImageUpload imgu = new ImageUpload(System.Web.HttpContext.Current.Server);
                lpath.Append("/" + imgu.GetProductThumbName(pId.ToString().Trim()));

                lImgPath = GetProductThumbImage(imgPath, lpath, pSubFolderName, pSubFolderPrefix);
                //If image is not present
                //if (string.IsNullOrEmpty(lImgPath))
                //{
                //    lpath = new StringBuilder(ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype + "_Images/" + ProductUpload.IMAGE_FOR.Products + "/No_thumbnail.png");
                //    lImgPath = imgPath.IMAGE_HTTP + lpath;
                //}
                return lImgPath.ToLower().Trim();
            }
            catch (Exception ex)
            {
                lImgPath = string.Empty;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageDisplay][LoadProductThumbnails]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return lImgPath;
        }

        /// <summary>
        /// this function gets the thumb path of product images from server (not in use)
        /// </summary>
        /// <param name="imgPath">ReadConfig Object to access key from web.config</param>
        /// <param name="lpath">fixed path of image upto product id (StringBuilder)</param>
        /// <param name="pSubFolderName">Sub folder Name of product according to stock (i.e. Color Name or default)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'Default'</param>
        /// <returns>string thumbnail path</returns>
        private static string GetProductThumbImage(ReadConfig imgPath, System.Text.StringBuilder lpath, string pSubFolderName, string pSubFolderPrefix)
        {
            StringBuilder sb = new StringBuilder();
            string lImgPath = string.Empty;
            try
            {
                //Check whether specified path is exists on server
                lpath = new StringBuilder(lpath.ToString().ToLower().Trim());
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(imgPath.IMAGE_HTTP + lpath.ToString());
                for (int i = 0; i < 1; i++)
                {
                    try
                    {
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        //if file exists
                        if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                        {
                            return imgPath.IMAGE_HTTP + lpath.ToString();
                        }
                    }
                    catch (WebException wex)
                    {
                        if (wex.Response != null)
                        {
                            HttpWebResponse response = (HttpWebResponse)wex.Response;
                            if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                ////check for "default" folder
                                //if (i == 0 && pSubFolderPrefix.ToLower().Trim().Equals(string.Empty) && pSubFolderName.ToLower().Trim() != "default")
                                //{
                                //    lpath.Replace("default_" + pSubFolderName.ToLower().Trim(), "default");
                                //    httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(new Uri(imgPath.IMAGE_HTTP + lpath.ToString()));
                                //}
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lImgPath = string.Empty;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageDisplay][GetProductThumbnail]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return lImgPath.ToLower().Trim();
        }

        //For customer product listing page purpose
        /// <summary>
        /// This function set thumb image path of perticular product 
        /// </summary>
        /// <param name="pId">Product ID</param>
        /// <param name="pSubFolderName">Sub folder Name of product according to stock (i.e. Color Name or default)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'Default'</param>
        /// <param name="pImgtype">Image type i.e Approved or NonApproved</param>
        /// <returns>string image path</returns>
        public static string SetProductThumbPath(long pId, string pSubFolderName, string pSubFolderPrefix, ProductUpload.IMAGE_TYPE pImgtype)
        {
            //comment
            /*
             * 1. create path for image to display
             * 2. concat the image lpath with file name
             * again contcat image http server path with lpath 
             */
            BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
            try
            {

                StringBuilder lpath = new StringBuilder(ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + pId.ToString());
                lpath.Append("/" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderPrefix.Trim().ToLower()));
                //folder prefix is empty
                if (string.IsNullOrEmpty(pSubFolderPrefix.Trim()))
                {
                    lpath.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()));
                }
                //folder prefix and folder name are not empty
                else if (!string.IsNullOrEmpty(pSubFolderName.Trim()))
                {
                    lpath.Append("_" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()));
                }

                ImageUpload imgu = new ImageUpload(System.Web.HttpContext.Current.Server);
                lpath.Append("/" + imgu.GetProductThumbName(pId.ToString().Trim()));

                if (imgPath.IMAGE_HTTP.EndsWith("/"))
                {
                    return (imgPath.IMAGE_HTTP + lpath.ToString().ToLower().Trim());
                }
                else
                {
                    return (imgPath.IMAGE_HTTP + "/" + lpath.ToString().ToLower().Trim());
                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageDisplay][LoadProductThumbnails]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            if (imgPath.IMAGE_HTTP.EndsWith("/"))
            {
                return (imgPath.IMAGE_HTTP + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/No_thumbnail.png").ToLower().Trim();
            }
            else
            { return (imgPath.IMAGE_HTTP + "/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/No_thumbnail.png").ToLower().Trim(); }
        }

        #endregion

        #region Product Images

        /// <summary>
        /// Get Product Images
        /// </summary>
        /// <param name="pId">Product ID</param>
        /// <param name="pSubFolderName">Sub folder Name of product according to stock (i.e. Color Name or default)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'Default'</param>
        /// <returns>array of string of image path</returns>
        public static string[] LoadProductImages(long pId, string pSubFolderName, string pSubFolderPrefix)
        {
            //1. create path to get images
            //2. Get Images from set path using http
            //3. if images present then return images path otherwise return thumb path for product 

            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/DEFAULT
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/RED
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/DEFAULT_GREEN

            StringBuilder lImgPath = new StringBuilder();
            try
            {
                //1-----------------------------------------------------------
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                StringBuilder lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + ProductUpload.IMAGE_TYPE.Approved.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + pId.ToString());
                //lpath.Append(string.IsNullOrEmpty(pSubFolderName) ? "/DEFAULT" : (string.IsNullOrEmpty(pSubFolderPrefix) ? "/" + pSubFolderName.Trim() : "/" + pSubFolderPrefix.Trim() + "_" + pSubFolderName.Trim()));

                lpath.Append("/" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderPrefix.Trim().ToLower()));
                //folder prefix is empty
                if (string.IsNullOrEmpty(pSubFolderPrefix.Trim()))
                {
                    lpath.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()));
                }
                //folder prefix and folder name are not empty
                else if (!string.IsNullOrEmpty(pSubFolderName.Trim()))
                {
                    lpath.Append("_" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()));
                }

                //2---------------------------------------------------------------------------------------
                lImgPath = GetProductImages(imgPath, lpath, pSubFolderName, pSubFolderPrefix);

                //3-----------------------------------------------------------------------------------------
                //If image is not present
                if (lImgPath.Length == 0)
                {
                    //lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + ProductUpload.IMAGE_TYPE.Approved + "_Images/" + ProductUpload.IMAGE_FOR.Products + "/No_thumbnail.png");
                    //lImgPath.Append(imgPath.IMAGE_HTTP + lpath);

                    ImageUpload imgu = new ImageUpload(System.Web.HttpContext.Current.Server);
                    lpath.Append("/" + imgu.GetProductThumbName(pId.ToString().Trim()));

                    if (imgPath.IMAGE_HTTP.EndsWith("/"))
                    {
                        lImgPath.Append(imgPath.IMAGE_HTTP + lpath); 
                    }
                    else
                    {
                        lImgPath.Append(imgPath.IMAGE_HTTP + "/" + lpath);
                    }
                }
                return lImgPath.ToString().ToLower().Trim().Split('\n');
            }
            catch (Exception ex)
            {
                lImgPath = null;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageDisplay][LoadProductImages] 3 Prams",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return null;
        }

        /// <summary>
        /// This function load the images of perticular product
        /// </summary>
        /// <param name="pId">Product Id</param>
        /// <param name="pSubFolderName">Sub folder Name of product according to stock (i.e. Color Name)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'DEFAULT'</param>
        /// <param name="pImgtype">Image type i.e Approved or NonApproved</param>
        /// <returns>Array of string which specifies image path</returns>
        public static string[] LoadProductImages(long pId, string pSubFolderName, string pSubFolderPrefix, ProductUpload.IMAGE_TYPE pImgtype)
        {
            //1. create path to get images
            //2. Get Images from set path using http
            //3. if images present then return images path otherwise return no thumbnail path

            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/DEFAULT
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/RED
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/DEFAULT_GREEN

            StringBuilder lImgPath = new StringBuilder();
            try
            {
                //1-------------------------------------------------------------------------------------
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                StringBuilder lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + pId.ToString());
                //lpath.Append(string.IsNullOrEmpty(pSubFolderName) ? "/DEFAULT" : (string.IsNullOrEmpty(pSubFolderPrefix) ? "/" + pSubFolderName.Trim() : "/" + pSubFolderPrefix.Trim() + "_" + pSubFolderName.Trim()));

                lpath.Append("/" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderPrefix.Trim().ToLower()));
                //folder prefix is empty
                if (string.IsNullOrEmpty(pSubFolderPrefix.Trim()))
                {
                    lpath.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()));
                }
                //folder prefix and folder name are not empty
                else if (!string.IsNullOrEmpty(pSubFolderName.Trim()))
                {
                    lpath.Append("_" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()));
                }

                //2------------------------------------------------------------------------------
                lImgPath = GetProductImages(imgPath, lpath, pSubFolderName, pSubFolderPrefix);

                //3---------------------------------------------------------------
                //If image is not present
                if (lImgPath.Length == 0)
                {
                    lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/No_thumbnail.png");
                    lImgPath.Append(imgPath.IMAGE_HTTP + lpath);
                }
                return lImgPath.ToString().ToLower().Trim().Split('\n');
            }
            catch (Exception ex)
            {
                lImgPath = null;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageDisplay][LoadProductImages]4 Params",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return null;
        }

        /// <summary>
        /// This function get the product images path from server
        /// </summary>
        /// <param name="imgPath">ReadConfig Object to access key from web.config</param>
        /// <param name="lpath">fixed path of image upto product id (StringBuilder)</param>
        /// <param name="pSubFolderName">Sub folder Name of product according to stock (i.e. Color Name or default)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'DEFAULT'</param>
        /// <returns>Array of string which specifies image path</returns>
        public static System.Text.StringBuilder GetProductImages(ReadConfig imgPath, System.Text.StringBuilder lpath, string pSubFolderName, string pSubFolderPrefix)
        {
            //1. Check whether specified path is exists on server
            //2. if exist, Get all "_LL" image path from above path using http

            StringBuilder sb = new StringBuilder();
            StringBuilder lImgPath = new System.Text.StringBuilder();
            try
            {
                //Check whether specified path is exists on server
                if (IsPathExists(imgPath, ref lpath, pSubFolderName.Trim(), pSubFolderPrefix.Trim()))
                {
                    byte[] buffer = new byte[2000000];
                    string tempString = null;
                    int count = 0;

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(imgPath.IMAGE_HTTP + lpath.ToString());
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    do
                    {
                        count = responseStream.Read(buffer, 0, buffer.Length);
                        if (count != 0)
                        {
                            tempString = Encoding.ASCII.GetString(buffer, 0, count);
                            sb.Append(tempString);
                        }
                    }
                    while (count > 0);
                    responseStream.Close();
                    response.Close();

                    string expression = @"<A[^>]*?HREF\s*=\s*[""']?" + @"([^'"" >]+?)[ '""]?>";
                    Regex regEx = new Regex(expression, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    MatchCollection matches = regEx.Matches(sb.ToString());
                    foreach (Match match in matches)
                    {
                        string href = Regex.Match(match.Value, "([\"\"'](?<url>.*?)[\"\"'])", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                        if (href.ToUpper().Contains("_LL."))
                        {
                            href = href.Replace("\"", "");
                            //lImgPath.Append(imgPath.IMAGE_HTTP + href);
                            lImgPath.Append("http://" + imgPath.IMAGE_HTTP.Remove(0, "http://".Length).Substring(0, imgPath.IMAGE_HTTP.LastIndexOf(@"/") - "http://".Length) + href);
                            lImgPath.Append("\n");
                        }
                    }

                    if (lImgPath.Length > 0)
                    {
                        lImgPath.Remove(lImgPath.ToString().LastIndexOf('\n'), 1);
                    }
                }
            }
            catch (Exception ex)
            {
                lImgPath = null;
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageDisplay][GetProductImages]," + lImgPath,
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return lImgPath;
        }

        /// <summary>
        /// This function load the unique images of perticular product
        /// </summary>
        /// <param name="pId">Product Id</param>
        /// <param name="pSubFolderName">Sub folder Name of product according to stock (i.e. Color Name)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'DEFAULT'</param>
        /// <param name="pThumbType">Thumbnail type, for mobile : MM, for other device : SD </param>
        /// <param name="pImgtype">Image type i.e Approved or NonApproved</param>
        /// <returns>Array of string which specifies imgae path</returns>
        public static string[] DisplayProductImages(long pId, string pSubFolderName, string pSubFolderPrefix, ProductUpload.THUMB_TYPE pThumbType, ProductUpload.IMAGE_TYPE pImgtype)
        {
            //1. create path to get images
            //2. Get Images from set path using http
            //3. if images present then return images path otherwise return no thumbnail path 

            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/DEFAULT
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/RED
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/DEFAULT_GREEN

            StringBuilder lImgPath = new StringBuilder();
            try
            {
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                StringBuilder lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + pId.ToString());
                //lpath.Append(string.IsNullOrEmpty(pSubFolderName) ? "/DEFAULT" : (string.IsNullOrEmpty(pSubFolderPrefix) ? "/" + pSubFolderName.Trim() : "/" + pSubFolderPrefix.Trim() + "_" + pSubFolderName.Trim()));

                lpath.Append("/" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderPrefix.Trim().ToLower()));
                //folder prefix is empty
                if (string.IsNullOrEmpty(pSubFolderPrefix.Trim()))
                {
                    lpath.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()));
                }
                //folder prefix and folder name are not empty
                else if (!string.IsNullOrEmpty(pSubFolderName.Trim()))
                {
                    lpath.Append("_" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()));
                }

                lImgPath = GetPImages(imgPath, lpath, pSubFolderName, pSubFolderPrefix, pThumbType);
                //If image is not present
                if (lImgPath.Length == 0)
                {
                    lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/No_thumbnail.png");
                    lImgPath.Append(imgPath.IMAGE_HTTP + lpath);
                }
                return lImgPath.ToString().ToLower().Trim().Split('\n');
            }
            catch (Exception ex)
            {

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageDisplay][DisplayProductImages]" + lImgPath,
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                lImgPath = null;
            }
            return null;
        }

        /// <summary>
        /// This function get path of the unique images of perticular product
        /// </summary>
        /// <param name="imgPath">ReadConfig Object to access key from web.config</param>
        /// <param name="lpath">fixed path of image upto product id (StringBuilder)</param>
        /// <param name="pSubFolderName">Sub folder Name of product according to stock (i.e. Color Name or default)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'DEFAULT'</param>
        /// <param name="pThumbType">Thumbnail type, for mobile : MM, for other device : SD </param>
        /// <returns>StringBuilder object which specifies imgae path</returns>
        private static System.Text.StringBuilder GetPImages(ReadConfig imgPath, System.Text.StringBuilder lpath, string pSubFolderName, string pSubFolderPrefix, ProductUpload.THUMB_TYPE pThumbType)
        {
            //1. Check whether specified path is exists on server
            //2. if exist, Get all images with requeted thumbtype path from above path using http

            StringBuilder sb = new StringBuilder();
            StringBuilder lImgPath = new System.Text.StringBuilder();
            try
            {
                //Check whether specified path is exists on server
                if (IsPathExists(imgPath, ref lpath, pSubFolderName.Trim(), pSubFolderPrefix.Trim()))
                {
                    byte[] buffer = new byte[2000000];
                    string tempString = null;
                    int count = 0;

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(imgPath.IMAGE_HTTP + lpath.ToString());
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    do
                    {
                        count = responseStream.Read(buffer, 0, buffer.Length);
                        if (count != 0)
                        {
                            tempString = Encoding.ASCII.GetString(buffer, 0, count);
                            sb.Append(tempString);
                        }
                    }
                    while (count > 0);
                    responseStream.Close();
                    response.Close();

                    string expression = @"<A[^>]*?HREF\s*=\s*[""']?" + @"([^'"" >]+?)[ '""]?>";
                    Regex regEx = new Regex(expression, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    MatchCollection matches = regEx.Matches(sb.ToString());
                    foreach (Match match in matches)
                    {
                        string href = Regex.Match(match.Value, "([\"\"'](?<url>.*?)[\"\"'])", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                        if (href.ToLower().Contains("_" + pThumbType.ToString().ToLower() + "."))
                        {
                            href = href.Replace("\"", "");
                            //lImgPath.Append(imgPath.IMAGE_HTTP + href);
                            lImgPath.Append("http://" + imgPath.IMAGE_HTTP.Remove(0, "http://".Length).Substring(0, imgPath.IMAGE_HTTP.LastIndexOf(@"/") - "http://".Length) + href);
                            lImgPath.Append("\n");
                        }
                    }

                    if (lImgPath.Length > 0)
                    {
                        lImgPath.Remove(lImgPath.ToString().LastIndexOf('\n'), 1);
                    }
                }
            }
            catch (Exception ex)
            {

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                  + Environment.NewLine + ex.Message + Environment.NewLine
                  + "[ImageDisplay][GetPImages] 6 Params " + imgPath.IMAGE_HTTP + "  " + imgPath.IMAGE_HTTP.LastIndexOf(@"/") + "  " + "http://".Length + " ",
                  BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                lImgPath = null;

                //throw new Exception("Probem in loading Productimage!!" + Environment.NewLine + ex.InnerException);

            }
                return lImgPath;
        }

        /// <summary>
        /// This function load the images of perticular product
        /// </summary>
        /// <param name="pId">Product Id</param>
        /// <param name="pSubFolderName">Sub folder Name of product according to stock (i.e. Color Name)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'DEFAULT'</param>
        /// <param name="pImgtype">Image type i.e Approved or NonApproved</param>
        /// <returns>Array of string which specifies image path</returns>
        public static string[] LoadAllProductImages(long pId, string pSubFolderName, string pSubFolderPrefix, ProductUpload.IMAGE_TYPE pImgtype)
        {
            //1. create path to get images
            //2. Get all Images from set path using http
            //3. if images present then return images path otherwise return no thumbnail path

            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/DEFAULT
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/RED
            // Path like ~/APPROVED_IMAGES/PRODUCTS/50/DEFAULT_GREEN

            StringBuilder lImgPath = new StringBuilder();
            try
            {
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                StringBuilder lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + pId.ToString());
                //lpath.Append(string.IsNullOrEmpty(pSubFolderName) ? "/DEFAULT" : (string.IsNullOrEmpty(pSubFolderPrefix) ? "/" + pSubFolderName.Trim() : "/" + pSubFolderPrefix.Trim() + "_" + pSubFolderName.Trim()));

                lpath.Append("/" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderPrefix.Trim().ToLower()));
                //folder prefix is empty
                if (string.IsNullOrEmpty(pSubFolderPrefix.Trim()))
                {
                    lpath.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()));
                }
                //folder prefix and folder name are not empty
                else if (!string.IsNullOrEmpty(pSubFolderName.Trim()))
                {
                    lpath.Append("_" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()));
                }

                lImgPath = GetAllProductImages(imgPath, lpath, pSubFolderName, pSubFolderPrefix);
                //If image is not present
                if (lImgPath.Length == 0)
                {
                    lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/No_thumbnail.png");
                    lImgPath.Append(imgPath.IMAGE_HTTP + lpath);
                }
                return lImgPath.ToString().ToLower().Trim().Split('\n');
            }
            catch (Exception ex)
            {
                lImgPath = null;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageDisplay][LoadAllProductImages]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return null;
        }

        /// <summary>
        /// This function get path of all images of perticular product
        /// </summary>
        /// <param name="imgPath">ReadConfig Object to access key from web.config</param>
        /// <param name="lpath">fixed path of image upto product id (StringBuilder)</param>
        /// <param name="pSubFolderName">Sub folder Name of product according to stock (i.e. Color Name or default)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'DEFAULT'</param>
        /// <returns>StringBuilder object which specifies path of images</returns>
        public static System.Text.StringBuilder GetAllProductImages(ReadConfig imgPath, System.Text.StringBuilder lpath, string pSubFolderName, string pSubFolderPrefix)
        {
            //1. Check whether specified path is exists on server
            //2. if exist, Get all images path from above path using http

            StringBuilder sb = new StringBuilder();
            StringBuilder lImgPath = new System.Text.StringBuilder();
            try
            {
                //Check whether specified path is exists on server
                if (IsPathExists(imgPath, ref lpath, pSubFolderName.Trim(), pSubFolderPrefix.Trim()))
                {
                    byte[] buffer = new byte[2000000];
                    string tempString = null;
                    int count = 0;

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(imgPath.IMAGE_HTTP + lpath.ToString());
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    do
                    {
                        count = responseStream.Read(buffer, 0, buffer.Length);
                        if (count != 0)
                        {
                            tempString = Encoding.ASCII.GetString(buffer, 0, count);
                            sb.Append(tempString);
                        }
                    }
                    while (count > 0);
                    responseStream.Close();
                    response.Close();

                    string expression = @"<A[^>]*?HREF\s*=\s*[""']?" + @"([^'"" >]+?)[ '""]?>";
                    Regex regEx = new Regex(expression, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    MatchCollection matches = regEx.Matches(sb.ToString());
                    foreach (Match match in matches)
                    {
                        string href = Regex.Match(match.Value, "([\"\"'](?<url>.*?)[\"\"'])", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                        href = href.Replace("\"", "");
                        if ((href.LastIndexOf('/') + 1) < href.Length)
                        {
                            //lImgPath.Append(imgPath.IMAGE_HTTP + href);
                            lImgPath.Append("http://" + imgPath.IMAGE_HTTP.Remove(0, "http://".Length).Substring(0, imgPath.IMAGE_HTTP.LastIndexOf(@"/") - "http://".Length) + href);
                            lImgPath.Append("\n");
                        }
                    }

                    if (lImgPath.Length > 0)
                    {
                        lImgPath.Remove(lImgPath.ToString().LastIndexOf('\n'), 1);
                    }
                }
            }
            catch (Exception ex)
            {
                lImgPath = null;
                //throw new Exception("Probem in loading Productimage!!" + Environment.NewLine + ex.InnerException);
            }
            return lImgPath;
        }

        #endregion

        #region Shop Images

        /// <summary>
        /// This function load the images of perticular product
        /// </summary>
        /// <param name="pId">Shop Id</param>
        /// <returns>Array of string which specifies image path</returns>
        public static string[] LoadShopImages(long pId)
        {
            //create path for shop images for requested shopID
            //get images path from above set path using http
            //~/APPROVED_IMAGES/SHOPS/1001
            StringBuilder lImgPath = new StringBuilder();
            try
            {
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                StringBuilder lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + ProductUpload.IMAGE_TYPE.Approved.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Shops.GetDescription() + "/" + pId);
                lImgPath = GetShopImages(imgPath, lpath);
                //If image is not present
                if (lImgPath.Length == 0)
                {
                    lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + ProductUpload.IMAGE_TYPE.Approved.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Shops.GetDescription() + "/No_thumbnail.png");
                    lImgPath.Append(imgPath.IMAGE_HTTP + lpath);
                }
                return lImgPath.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                lImgPath = null;
                // throw new Exception("Probem in loading Shop image!!" + Environment.NewLine + ex.InnerException);
            }
            return null;
        }

        /// <summary>
        /// This function load the images of perticular Shop
        /// </summary>
        /// <param name="pId">Shop Id</param>
        /// <param name="pImgtype">Image type i.e Approved or NonApproved</param>
        /// <returns>Array of string which specifies image path</returns>
        public static string[] LoadShopImages(long pId, ProductUpload.IMAGE_TYPE pImgtype)
        {
            //create path for shop images for requested shopID
            //get images path from above set path using http
            //~/APPROVED_IMAGES/SHOPS/1001
            StringBuilder lImgPath = new StringBuilder();
            try
            {
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                StringBuilder lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Shops.GetDescription() + "/" + pId);
                lImgPath = GetShopImages(imgPath, lpath);
                //If image is not present
                if (lImgPath.Length == 0)
                {
                    lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Shops.GetDescription() + "/No_thumbnail.png");
                    lImgPath.Append(imgPath.IMAGE_HTTP + lpath);
                }
                return lImgPath.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                lImgPath = null;
                // throw new Exception("Probem in loading Shop image!!" + Environment.NewLine + ex.InnerException);
            }
            return null;
        }

        /// <summary>
        /// This function get path of shop images
        /// </summary>
        /// <param name="imgPath">ReadConfig Object to access key from web.config</param>
        /// <param name="lpath">fixed path of image upto product id (StringBuilder)</param>
        /// <returns>StringBuilder object which specifies imgae path</returns>
        private static System.Text.StringBuilder GetShopImages(ReadConfig imgPath, System.Text.StringBuilder lpath)
        {
            //Check whether specified path is exists on server
            //if exist, get all images path from above set path using http

            StringBuilder sb = new StringBuilder();
            StringBuilder lImgPath = new System.Text.StringBuilder();
            try
            {
                //Check whether specified path is exists on server
                if (IsPathExists(imgPath, lpath))
                {
                    byte[] buffer = new byte[2000000];
                    string tempString = null;
                    int count = 0;

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(imgPath.IMAGE_HTTP + lpath.ToString());
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    do
                    {
                        count = responseStream.Read(buffer, 0, buffer.Length);
                        if (count != 0)
                        {
                            tempString = Encoding.ASCII.GetString(buffer, 0, count);
                            sb.Append(tempString);
                        }
                    }
                    while (count > 0);
                    responseStream.Close();
                    response.Close();

                    string expression = @"<A[^>]*?HREF\s*=\s*[""']?" + @"([^'"" >]+?)[ '""]?>";
                    Regex regEx = new Regex(expression, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    MatchCollection matches = regEx.Matches(sb.ToString());
                    foreach (Match match in matches)
                    {
                        string href = Regex.Match(match.Value, "([\"\"'](?<url>.*?)[\"\"'])", RegexOptions.Multiline | RegexOptions.IgnoreCase).Value;
                        //get images path excluding logo, description and profile
                        if (!href.ToUpper().Contains("LOGO") && !href.ToUpper().Contains("DESCRIPTION") && !href.ToUpper().Contains("PROFILE"))
                        {
                            href = href.Replace("\"", "");
                            if ((href.LastIndexOf('/') + 1) < href.Length)
                            {    //lImgPath.Append(imgPath.IMAGE_HTTP + href);
                                lImgPath.Append("http://" + imgPath.IMAGE_HTTP.Remove(0, "http://".Length).Substring(0, imgPath.IMAGE_HTTP.LastIndexOf(@"/") - "http://".Length) + href);
                                lImgPath.Append("\n");
                            }
                        }
                    }

                    if (lImgPath.Length > 0)
                    {
                        lImgPath.Remove(lImgPath.ToString().LastIndexOf('\n'), 1);
                    }
                }
            }
            catch (Exception ex)
            {
                lImgPath = null;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageDisplay][GetShopImages]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return lImgPath;
        }

        #endregion

        /******************************************************************************************************************************************************/

        //-----------------------------------------------------------------------
        // <copyright file="ImageDisplay.GetStockImages()" company="Ezeelo Consumer Services Pvt. Ltd.">
        //     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
        // </copyright>
        // <author>Sujata Kullarkar</author>
        //-----------------------------------------------------------------------
        /// <summary>
        /// Get all images of product
        /// </summary>
        /// <param name="pID">Product ID</param>
        /// <param name="clName">ColorName</param>
        /// <returns>list of images with their names and fullpath.</returns>
        public static List<ImageListViewModel> GetStockImages(long pID, string clName)
        {
            List<ImageListViewModel> lImages = new List<ImageListViewModel>();
            //ImageDisplay im = new ImageDisplay(System.Web.HttpContext.Current.Server);
            //string[] lImageFile = ImageDisplay.LoadProductImages(pID, clName, clName.Trim().Equals(string.Empty) ? "Default" : string.Empty);
            string[] lImageFile = ImageDisplay.LoadProductImages(pID, clName, string.Empty);

            if (lImageFile != null)
            {
                lImages = (from file in lImageFile
                           select new ImageListViewModel
                           {
                               // ImgName = file.Name,
                               ImgPath = file

                           }).ToList();
            }
            return lImages;
        }

        /// <summary>
        /// Return Credential of ftp request
        /// Tejaswee 05/08/2015
        /// </summary>
        /// <param name="siteurl"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="authtype"></param>
        /// <returns></returns>
        private static ICredentials BuildCredentials(string username, string password)
        {
            NetworkCredential cred;
            cred = new System.Net.NetworkCredential(username, password);

            //CredentialCache cache = new CredentialCache();
            //if (authtype.Contains(":"))
            //{
            //    authtype = authtype.Substring(authtype.IndexOf(":") + 1); //remove the TMG: prefix
            //}
            //cache.Add(new Uri(siteurl), authtype, cred);
            return cred;
        }

        public static string GetShopDescription(ReadConfig imgPath, System.Text.StringBuilder lpath)
        {
            string lDescriptonPath = string.Empty;
            try
            {
                if (IsPathExists(imgPath, lpath))
                {
                    FtpWebRequest reqFTP;
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath.ToString()));
                    reqFTP.UseBinary = true;
                    reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                    using (FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse())
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        string line = reader.ReadLine();
                        while (line != null)
                        {
                            if (line.Contains(ProductUpload.DESC_FILE_NAME.Split('.')[0]))
                            {
                                lDescriptonPath = imgPath.IMAGE_HTTP + lpath.ToString() + "/" + line;
                                break;
                            }
                            line = reader.ReadLine();
                        }
                        reader.Close();
                        response.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                lDescriptonPath = string.Empty;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageDisplay][GetShopLogo]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return lDescriptonPath;
        }
    }
}
