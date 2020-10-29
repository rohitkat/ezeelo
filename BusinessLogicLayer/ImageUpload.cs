//-----------------------------------------------------------------------
// <copyright file="ImageUpload.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Snehal Shende</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Globalization;


namespace BusinessLogicLayer
{
    public class ImageUpload : ProductUpload
    {

        public ImageUpload(System.Web.HttpServerUtility server) : base(server) { }

        /// <summary>
        /// This Function is used to upload the images for products
        /// </summary>
        /// <param name="pName">product name</param>
        /// <param name="pId">product id</param>
        /// <param name="pSubFolderName">Name of folder i.e. Stock color name or default (string)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'Default' (string)</param>
        /// <param name="pfile">Image file</param>       
        /// <param name="pImgtype">Image type i.e Approved or NonApproved</param>
        /// <returns>path of local folder</returns>      
        public string AddProductImage(string pName, long pId, string pSubFolderName, string pSubFolderPrefix, HttpPostedFileBase pfile, IMAGE_TYPE pImgtype, bool IsThumb)
        {

            //comment 
            //1. create path for image to be upload
            //2. create directory of above set path on local
            //3. check image file extension and size (extension may jpg,jpeg,png & size is always less than 100 KB)
            //4. Find product image index for new image.... (suppose, if two iimages for same product id is already uploaded i.e. 001 & 002 then new image index will be 003)
            //5. Create image filename for new image
            //6. Save new image on local server
            //7. Create 3 new thumbnails of uplaoded image (i.e sd,ss,mm)
            //8. Upload imges from local server to FTP server.

            try
            {
                DIMENSION lDimension = DIMENSION.LARGE;
                IMAGE_FOR lImgfor = IMAGE_FOR.Products;
                //is file present
                if (pfile != null && pfile.ContentLength > 0)
                {
                    //1------------------------------------------------------------------------------------------------------
                    StringBuilder lpath = new StringBuilder("~/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + lImgfor.GetDescription());

                    int lMaxContent = this.GetMaxImageSize();

                    if (!string.IsNullOrEmpty(pId.ToString().Trim()))
                    {
                        lpath.Append("/" + pId.ToString().Trim());
                        string foldeName = lpath.ToString();
                        lpath.Append(string.IsNullOrEmpty(pSubFolderName.Trim()) ? "/Default" : (string.IsNullOrEmpty(pSubFolderPrefix.Trim()) ? "/" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()) : "/" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderPrefix.Trim().ToLower()) + "_" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower())));
                        
                        //2---------------------------------------------------------------------
                        DirectoryInfo lDrInfo = this.CreateDirectory(this.server.MapPath(lpath.ToString()));

                        //3-------------------------------------------------------------------------------------
                        string[] file_extensions = { ".jpg", ".jpeg", ".png", ".JPEG", ".JPG", ".PNG" };
                        if (file_extensions.Contains(Path.GetExtension(pfile.FileName)))
                        {
                            //file size is in limit or not
                            if (pfile.ContentLength <= lMaxContent)       //1 KB = 1024 Bytes, 100 KB = 102400 Bytes
                            {
                                //4---------------------------------------------------------------------------------------------
                                string lImgIndex = GetProductImageIndex(lpath.ToString().Remove(0, 1));
                                
                                //5-----------------------------------------
                                var filename = GetFileName(lDimension, pId.ToString().Trim(), lImgIndex.Trim()) + Path.GetExtension(pfile.FileName);
                                
                                //6------------------------------------------------
                                string rootpath = Path.Combine(this.server.MapPath(lpath.ToString()), filename);
                                pfile.SaveAs(rootpath);

                                //7-------------------------------------------------------
                                CreateThumbnails(this.server.MapPath(lpath.ToString()), pId.ToString().Trim(), lImgIndex.Trim(), pfile, IsThumb);

                                //8---------------------------------------------------------------------------------
                                FtpImageUpload fimg = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                                fimg.CopyImagesToFTP(this.server.MapPath(lpath.ToString()), lpath.ToString().Remove(0, 1));

                                return this.server.MapPath(foldeName);
                            }
                            else
                            {
                                //    //ErrorMessage = "Your File is too large, maximum allowed size is : " + (lMaxContent / 1024).ToString() + "KB";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[AddProductImage]", "Can't Add Product's Image in Method !" + Environment.NewLine + ex.Message);
            }
            return string.Empty;
        }

        //***New Method added for upload image from FTP server By Roshan***//
        public bool UploadHomePageDynamicImageOnServer(HttpPostedFileBase pfile, Int64 cityId, Int64 FranchiseID,int HomepageDynamicSectionID,  string fileName)////added Int32 FranchiseID
        {
            //comment 
            //1. Check contentlength is less than max limit
            //2. Check image extension
            //3. Create path for image to be upload and create directory of set path on local server
            //4. Save new image on local server
            //5. Create directory on ftp server
            //6. Upload imges from local server to FTP server.
            try
            {
                int lMaxContent = GetDyHomepageMaxImageSize();
                //1-------------------------------------------
                if (pfile.ContentLength <= lMaxContent)       //1 KB = 1024 Bytes, 100 KB = 102400 Bytes
                {
                    //2------------------------------------------------
                    string[] file_extensions = { ".jpg", ".jpeg", ".png", ".gif", ".JPEG", ".JPG", ".PNG", ".GIF" };
                    if (file_extensions.Contains(Path.GetExtension(pfile.FileName)))
                    {
                        BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                        //3 & 4----------------------------------------------------------------------------------------------------
                        DirectoryInfo lDrInfo = this.CreateDirectory(Path.Combine(this.server.MapPath("~/" + ProductUpload.IMAGE_ROOTPATH), cityId.ToString()));////doubt
                        string rootpath = Path.Combine(Path.Combine(this.server.MapPath("~/" + ProductUpload.IMAGE_ROOTPATH), cityId.ToString()), fileName);////doubt

                        pfile.SaveAs(rootpath);

                        //5----------------------------------------------------------------------
                        // string targetfolderPath = cityId.ToString() + "/" + blockTypeId.ToString();////hide
                        string targetfolderPath = cityId.ToString() + "/" + FranchiseID.ToString() + "/" + HomepageDynamicSectionID;////added
                        if (CreateDirectoryOnFtp(imgPath.HomePageDynamicSection_IMAGE_FTP, targetfolderPath))
                        {
                            //6-----------------------------------------------------------------------------

                            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(imgPath.HomePageDynamicSection_IMAGE_FTP + "/" + targetfolderPath + "/" + fileName);
                            req.UseBinary = true;
                            //-------
                            //req.EnableSsl = true;
                            //req.UsePassive = true;
                            //-----
                            req.Method = WebRequestMethods.Ftp.UploadFile;
                            req.Credentials = new NetworkCredential(imgPath.USER_NAME, imgPath.PASSWORD);

                            byte[] fileData = File.ReadAllBytes(rootpath);
                            req.ContentLength = fileData.Length;
                            Stream reqStream = req.GetRequestStream();
                            reqStream.Write(fileData, 0, fileData.Length);
                            reqStream.Close();
                        }
                        lDrInfo.Delete(true);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //file not found or path not exists 
                return false;
                throw new BusinessLogicLayer.MyException("[UploadOfferSectionImageOnServer]", "Can't Upload Offer Section  Image On Server in Method !" + Environment.NewLine + ex.Message);
              
            }
            return false;
        }


        public bool DeleteHomePageDynamicImageOnServer(HttpPostedFileBase pfile, Int64 cityId, Int64 FranchiseID, Int64 HomepageDynamicSectionID, string fileName)////added Int32 FranchiseID
        {
            //comment 
            //1. Check contentlength is less than max limit
            //2. Check image extension
            //3. Create path for image to be upload and create directory of set path on local server
            //4. Save new image on local server
            //5. Create directory on ftp server
            //6. Upload imges from local server to FTP server.
            try
            {
                int lMaxContent = GetDyHomepageMaxImageSize();
                //1-------------------------------------------
                if (pfile.ContentLength <= lMaxContent)       //1 KB = 1024 Bytes, 100 KB = 102400 Bytes
                {
                    //2------------------------------------------------
                    string[] file_extensions = { ".jpg", ".jpeg", ".png", ".gif", ".JPEG", ".JPG", ".PNG", ".GIF" };
                    if (file_extensions.Contains(Path.GetExtension(pfile.FileName)))
                    {
                        BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                        //3 & 4----------------------------------------------------------------------------------------------------
                        DirectoryInfo lDrInfo = this.CreateDirectory(Path.Combine(this.server.MapPath("~/" + ProductUpload.IMAGE_ROOTPATH), cityId.ToString()));////doubt
                        string rootpath = Path.Combine(Path.Combine(this.server.MapPath("~/" + ProductUpload.IMAGE_ROOTPATH), cityId.ToString()), fileName);////doubt

                        pfile.SaveAs(rootpath);

                        //5----------------------------------------------------------------------
                        // string targetfolderPath = cityId.ToString() + "/" + blockTypeId.ToString();////hide
                        string targetfolderPath = cityId.ToString() + "/" + FranchiseID.ToString() + "/" + HomepageDynamicSectionID;////added
                        if (CreateDirectoryOnFtp(imgPath.HOME_IMAGE_FTP, targetfolderPath))
                        {
                            //6-----------------------------------------------------------------------------

                            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(imgPath.HOME_IMAGE_FTP + "/" + targetfolderPath + "/" + fileName);
                            string path = imgPath.HOME_IMAGE_FTP + "/" + targetfolderPath + "/" + fileName;
                            bool IsFileExist = CheckIfFileExistsOnServer(path, imgPath.USER_NAME, imgPath.PASSWORD);

                            if (IsFileExist)
                            {
                                req.Method = WebRequestMethods.Ftp.DeleteFile;
                                req.Credentials = new NetworkCredential(imgPath.USER_NAME, imgPath.PASSWORD);

                                using (FtpWebResponse response = (FtpWebResponse)req.GetResponse())
                                {
                                    return true;
                                }
                            }

                        }
                        lDrInfo.Delete(true);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //file not found or path not exists 
                //return false;
                throw new BusinessLogicLayer.MyException("[UploadHomePageImageOnServer]", "Can't Upload Home Page Image On Server in Method !" + Environment.NewLine + ex.Message);
            }
            return false;
        }



        /// <summary>
        /// This Function is used to upload the images for shops
        /// </summary>        
        /// <param name="pId">shop id</param>              
        /// <param name="pfile">Image file</param>           
        /// <param name="pImgtype">Image type i.e Approved or Nonapproved</param>  
        ///  <returns>path of local folder</returns>
        public string AddShopImage(long pId, HttpPostedFileBase pfile, IMAGE_TYPE pImgtype)
        {
            //comment 
            //1. create path for image to be upload
            //2. create directory of above set path on local
            //3. check image file extension and size (extension may jpg,jpeg,png & size is always less than 100 KB)
            //4. Find shop image index for new image.... (suppose, if two iimages for same shop id is already uploaded i.e. 001 & 002 then new image index will be 003)           
            //5. Save new image on local server
            //6. Upload imges from local server to FTP server.

            try
            {
                IMAGE_FOR lImgfor = IMAGE_FOR.Shops;

                if (pfile != null && pfile.ContentLength > 0)
                {
                    //1------------------------------
                    StringBuilder lpath = new StringBuilder("~/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + lImgfor.GetDescription());

                    int lMaxContent = this.GetMaxImageSize();

                    if (!string.IsNullOrEmpty(pId.ToString().Trim()))
                    {
                        lpath.Append("/" + pId.ToString().Trim());

                        //2--------------------------------------------
                        //Check if Folder exists or not
                        DirectoryInfo lDrInfo = this.CreateDirectory(this.server.MapPath(lpath.ToString()));

                        //3------------------------------------------------
                        string[] file_extensions = { ".jpg", ".jpeg", ".png", ".JPEG", ".JPG", ".PNG" };
                        //file size is in limit or not
                        if (file_extensions.Contains(Path.GetExtension(pfile.FileName)))
                        {
                            if (pfile.ContentLength <= lMaxContent)       //1 KB = 1024 Bytes, 100 KB = 102400 Bytes
                            {
                                //4---------------------------
                                string lImgIndex = GetShopImageIndex(lpath.ToString().Remove(0, 1));
                                var filename = lImgIndex.Trim() + Path.GetExtension(pfile.FileName);
                                //5--------------------------------------------------
                                string rootpath = Path.Combine(this.server.MapPath(lpath.ToString()), filename);
                                pfile.SaveAs(rootpath);
                                //6------------------------------------------
                                FtpImageUpload fimg = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                                fimg.CopyImagesToFTP(this.server.MapPath(lpath.ToString()), lpath.ToString().Remove(0, 1));

                                return this.server.MapPath(lpath.ToString());
                            }
                            else
                            {
                                //    //ErrorMessage = "Your File is too large, maximum allowed size is : " + (lMaxContent / 1024).ToString() + "KB";
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[AddShopImage]", "Can't Add Shop Image in Method !" + Environment.NewLine + ex.Message);
            }
            return string.Empty;
        }

        /// <summary>
        /// This Function is used to upload the Logo for shop
        /// </summary>        
        /// <param name="pId">shop id</param>              
        /// <param name="pImgLogo">Logo image to be upload</param>        
        /// <param name="pImgtype">Image type i.e Approved or Nonapproved</param>        
        public void AddShopLogo(long pId, HttpPostedFileBase pImgLogo, IMAGE_TYPE pImgtype)
        {
            //comment 
            //1. create path for image to be upload
            //2. create directory of above set path on local
            //3. check image file extension and size (extension may jpg,jpeg,png & size is always less than 100 KB)
            //4. Save new image on local server
            //5. Upload imges from local server to FTP server.

            try
            {
                IMAGE_FOR lImgfor = IMAGE_FOR.Shops;

                if (pImgLogo != null && pImgLogo.ContentLength > 0)
                {
                    //1------------------------------------------------------------------------------------------------------
                    StringBuilder lpath = new StringBuilder("~/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + lImgfor.GetDescription());

                    int lMaxContent = this.GetMaxImageSize();

                    if (!string.IsNullOrEmpty(pId.ToString().Trim()))
                    {
                        lpath.Append("/" + pId.ToString().Trim());

                        //2------------------------------------------------------------------------------------------------------
                        //Check if Folder exists or not
                        DirectoryInfo lDrInfo = this.CreateDirectory(this.server.MapPath(lpath.ToString()));

                        //3--------------------------------------------------------------------------------------------
                        string[] file_extensions = { ".jpg", ".jpeg", ".png", ".JPEG", ".JPG", ".PNG" };
                        //file size is in limit or not
                        if (file_extensions.Contains(Path.GetExtension(pImgLogo.FileName)))
                        {
                            if (pImgLogo.ContentLength <= lMaxContent)       //1 KB = 1024 Bytes, 100 KB = 102400 Bytes
                            {
                                //4--------------------------------------------------------------------------------
                                var filename = "LOGO" + Path.GetExtension(pImgLogo.FileName);
                                string rootpath = Path.Combine(this.server.MapPath(lpath.ToString()), filename);
                                pImgLogo.SaveAs(rootpath);

                                //5------------------------------------------------------------------------------------------------
                                FtpImageUpload fimg = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                                fimg.CopyImagesToFTP(this.server.MapPath(lpath.ToString()), lpath.ToString().Remove(0, 1));
                                lDrInfo.Delete(true);
                            }
                            else
                            {
                                //    //ErrorMessage = "Your File is too large, maximum allowed size is : " + (lMaxContent / 1024).ToString() + "KB";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[AddShopLogo]", "Probem in Saving Shop Logo!! !" + Environment.NewLine + ex.Message);
            }
        }

        public void AddMerchantProfile(long pId, HttpPostedFileBase pImgLogo, IMAGE_TYPE pImgtype)
        {
            try
            {
                IMAGE_FOR lImgfor = IMAGE_FOR.Shops;

                if (pImgLogo != null && pImgLogo.ContentLength > 0)
                {
                    StringBuilder lpath = new StringBuilder("~/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + lImgfor.GetDescription());

                    int lMaxContent = this.GetMaxImageSize();

                    if (!string.IsNullOrEmpty(pId.ToString().Trim()))
                    {
                        lpath.Append("/" + pId.ToString().Trim());

                        //Check if Folder exists or not
                        DirectoryInfo lDrInfo = this.CreateDirectory(this.server.MapPath(lpath.ToString()));

                        string[] file_extensions = { ".jpg", ".jpeg", ".png" };

                        if (file_extensions.Contains(Path.GetExtension(pImgLogo.FileName)))
                        {//file size is in limit or not
                            if (pImgLogo.ContentLength <= lMaxContent)       //1 KB = 1024 Bytes, 100 KB = 102400 Bytes
                            {
                                var filename = "PROFILE" + Path.GetExtension(pImgLogo.FileName);
                                string rootpath = Path.Combine(this.server.MapPath(lpath.ToString()), filename);
                                pImgLogo.SaveAs(rootpath);

                                FtpImageUpload fimg = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                                fimg.CopyImagesToFTP(this.server.MapPath(lpath.ToString()), lpath.ToString().Remove(0, 1));
                                lDrInfo.Delete(true);
                            }
                            else
                            {
                                //    //ErrorMessage = "Your File is too large, maximum allowed size is : " + (lMaxContent / 1024).ToString() + "KB";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[AddMerchantProfile]", "Probem in Saving Merchant Profile!! !" + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// This functions is used to create and upload three different thumbnail of image
        /// </summary>
        /// <param name="pPath">New Path for image storage</param>
        /// <param name="pName">name of product of image</param>
        /// <param name="pImgIndex">image index</param>
        /// <param name="pImgUpload">Image to create thumbnails</param>
        private void CreateThumbnails(string pPath, string pName, string pImgIndex, HttpPostedFileBase pfile, bool IsThumb)
        {
            try
            {

                // Generate small size image
                string filename = GetFileName(DIMENSION.SMALL, pName.Trim(), pImgIndex.Trim()) + Path.GetExtension(pfile.FileName);
                string lNewPath = Path.Combine(pPath.ToString(), filename);
                this.GenerateThumbnail((double)DIMENSION.SMALL, pfile.InputStream, lNewPath);

                // Generate Thumb Image 
                filename = GetFileName(DIMENSION.THUMB, pName.Trim(), pImgIndex.Trim()) + Path.GetExtension(pfile.FileName);
                lNewPath = Path.Combine(pPath.ToString(), filename);
                this.GenerateThumbnail((double)DIMENSION.THUMB, pfile.InputStream, lNewPath);

                // Generate mobile Thumb Image 
                filename = GetFileName(DIMENSION.MOBILE, pName.Trim(), pImgIndex.Trim()) + Path.GetExtension(pfile.FileName);
                lNewPath = Path.Combine(pPath.ToString(), filename);
                this.GenerateThumbnail((double)DIMENSION.MOBILE, pfile.InputStream, lNewPath);

                // Generate product thumb Images
                if (IsThumb)
                {
                    filename = GetProductThumbName(pName.Trim());
                    lNewPath = Path.Combine(pPath.ToString(), filename);
                    this.GenerateThumbnail((double)DIMENSION.THUMB, pfile.InputStream, lNewPath);
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[CreateThumbnails]", "Probem in Creating thumbnail images!!" + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// This functions is used to get maximum memory size of image to be upload in bytes
        /// </summary>
        /// <returns></returns>
        private int GetMaxImageSize()
        {
            // 102400=100 KB
            //return 102400;
            //return 102400; 
            //256000kb = 256MB
             return 256000;
           
        }


        //This function is used Maximum memory size of image of Dynamic Home page Addedd by Roshan
        private int GetDyHomepageMaxImageSize()
        {
            // 102400=100 KB
            //return 102400;
            //return 102400; 
            //256000kb = 256MB
            return 900000;
        }
        /// <summary>
        /// this function is used to create file name of product
        /// </summary>
        /// <param name="dimension">Dimension of product image</param>
        /// <param name="pName">Product name</param>
        /// <param name="pImgIndex">Image index of product</param>
        /// <returns>filename of product</returns>
        private string GetFileName(DIMENSION dimension, string pId, string pImgIndex)
        {
            //Date: 26/08/2015
            //new change for image name : gb_[product_unique_id]_[image_serial]_[type_code].jpg
            //e.g. gb_3568_001_ll.jpg
            string filename = "gb_" + pId + "_";
            try
            {
                switch (dimension)
                {
                    case DIMENSION.LARGE:               ////gb_3568_001_ll.extenstion.
                        filename = filename + pImgIndex + "_LL".Trim().ToLower();
                        break;
                    case DIMENSION.SMALL:               ////gb_3568_001_ss.extenstion.
                        filename = filename + pImgIndex + "_" + "SS".Trim().ToLower();
                        break;
                    case DIMENSION.THUMB:               ////gb_3568_001_sd.extenstion.
                        filename = filename + pImgIndex + "_SD".Trim().ToLower();
                        break;
                    case DIMENSION.MOBILE:               ////gb_3568_001_mm.extenstion.
                        filename = filename + pImgIndex + "_MM".Trim().ToLower();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetFileName]", "Probem in Generating Filename for product image!!" + Environment.NewLine + ex.Message);
            }
            return filename;
        }

        /// <summary>
        /// this function is used to create file name of product thumb
        /// </summary>
        /// <param name="pId">Product ID</param>
        /// <returns></returns>
        public string GetProductThumbName(string pId)
        {
            string filename = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(pId))
                {
                    filename = "gb_" + pId + "_thumb.jpg";
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetProductThumbName]", "Probem in Generating Filename for Product Thumb image!!" + Environment.NewLine + ex.Message);
            }
            return filename;
        }

        /// <summary>
        /// This function identify that directory exists or not, if not exists create directory. 
        /// </summary>
        /// <param name="path">Path of directory</param>
        /// <returns>DirectoryInfo of specified path</returns>
        public DirectoryInfo CreateDirectory(string path)
        {
            // check if directory is exist or not, if not exist then create directory.
            DirectoryInfo lDrInfo = new DirectoryInfo(path.Trim());
            try
            {
                if (!lDrInfo.Exists)
                {
                    lDrInfo = System.IO.Directory.CreateDirectory(path.Trim());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[CreateDirectory]", "Probem in Creating Directory!!" + Environment.NewLine + ex.Message);
            }

            return lDrInfo;
        }

        /// <summary>
        /// this function genrate the thumbnail image
        /// </summary>
        /// <param name="pWidth">width of thumbnail to be created</param>
        /// <param name="sourcePath">source path of image of which thumbnail to be created</param>
        /// <param name="targetPath">destination path fo thumbnail image to save</param>
        private void GenerateThumbnail(double pWidth, Stream sourcePath, string targetPath)
        {
            try
            {
                // Generate image with said pWidth and save images to target path.
                using (var image = Image.FromStream(sourcePath))
                {
                    //var newWidth = (int)(image.Width * scaleFactor);
                    //var newHeight = (int)(image.Height * scaleFactor);
                    var newWidth = (int)(pWidth);
                    var newHeight = (int)(pWidth);
                    var thumbnailImg = new Bitmap(newWidth, newHeight);
                    var thumbGraph = Graphics.FromImage(thumbnailImg);
                    thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                    thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                    thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                    thumbGraph.DrawImage(image, imageRectangle);
                    thumbnailImg.Save(targetPath, image.RawFormat);
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GenerateThumbnail]", "Probem in Generating thumbnail image!!" + Environment.NewLine + ex.Message);
            }
        }


        /// <summary>
        /// to Get new image index for saving image file of Shop
        /// </summary>
        /// <param name="lpath">Path of Shop image folder in which image to be placed</param>
        /// <returns>new image index</returns>
        private string GetShopImageIndex(string lpath)
        {
            //comment
            /*1. set filecount initially to 1
             *2. this function is used to access ftp folder with said lpath 
             *   if exist then count ll type image in folder to set new index
             *   if no path exist on ftp then image index set to 1
             */
            string lImgIndex = string.Empty;
            try
            {
                //int filesCount = pDirectoryInfo.GetFiles().Count() - pDirectoryInfo.GetFiles("LOGO.*").Count();
                int filesCount = 0;

                filesCount++;
                int ldigits = filesCount.ToString().Length;
                lImgIndex = GetFilenameStartString(ldigits);
                lImgIndex = lImgIndex + filesCount.ToString();

                FtpImageUpload ftpImg = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);

                //if images already exists in folder get appropriate index
                //check path exists or not
                if (ftpImg.IsPathExists(imgPath, lpath))
                {
                    FtpWebRequest reqFTP;
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath.ToString()));
                    reqFTP.UseBinary = true;
                    reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                    reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        if (!line.ToUpper().Contains("LOGO"))
                        {
                            filesCount++;
                            ldigits = filesCount.ToString().Length;
                            lImgIndex = GetFilenameStartString(ldigits) + filesCount.ToString();
                        }
                        line = reader.ReadLine();
                    }
                    reader.Close();
                    response.Close();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetShopImageIndex]", "Probem in Getting Shop image index!!" + Environment.NewLine + ex.Message);
            }
            return lImgIndex;
        }


        /// <summary>
        /// to Get new image index for saving image file of Product
        /// </summary>
        /// <param name="lpath">Path of Product image folder in which image to be placed</param>
        /// <returns>new image index</returns>
        private string GetProductImageIndex(string lpath)
        {
            //comment
            /*1. set filecount initially to 1
             *2. this function is used to access ftp folder with said lpath 
             *   if exist then count ll type image in folder to set new index
             *   if no path exist on ftp then image index set to 1
             */
            string lImgIndex = string.Empty;
            try
            {
                //int filesCount = pDirectoryInfo.GetFiles().Count() - pDirectoryInfo.GetFiles("LOGO.*").Count();
                int filesCount = 0;

                filesCount++;
                int ldigits = filesCount.ToString().Length;
                lImgIndex = GetFilenameStartString(ldigits);
                lImgIndex = lImgIndex + filesCount.ToString();

                FtpImageUpload ftpImg = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);

                if (ftpImg.IsPathExists(imgPath, lpath))
                {
                    FtpWebRequest reqFTP;
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath.ToString()));
                    reqFTP.UseBinary = true;
                    reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                    reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        if (line.ToUpper().Contains("_LL"))
                        {
                            if (line.ToUpper().Contains(lImgIndex + "_LL"))
                            {
                                filesCount++;
                                ldigits = filesCount.ToString().Length;
                                lImgIndex = GetFilenameStartString(ldigits) + filesCount.ToString();
                            }
                            else
                            {
                                break;
                            }
                        }
                        line = reader.ReadLine();
                    }
                    reader.Close();
                    response.Close();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetProductImageIndex]", "Probem in Getting product image index!!" + Environment.NewLine + ex.Message);
            }
            return lImgIndex;
        }

        /// <summary>
        /// this function specify initial string for file name index
        /// </summary>
        /// <param name="ldigits">digit of new image index</param>
        /// <returns>initial string of image index</returns>
        private string GetFilenameStartString(int ldigits)
        {
            string lImgIndex = string.Empty;
            try
            {
                switch (ldigits)
                {
                    case 1: lImgIndex = "00";
                        break;
                    case 2: lImgIndex = "0";
                        break;
                    case 3: lImgIndex = string.Empty;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetFilenameStartString]", "Probem in Getting image string!!" + Environment.NewLine + ex.Message);
            }
            return lImgIndex;
        }

        /// <summary>
        /// This Function is used to edit thumb image of product
        /// </summary>
        /// <param name="pId">product id</param>
        /// <param name="thumbPath">full path of image of which thumb images is to be created</param>       
        /// <returns>message (if fails to edit)</returns>      
        public string EditProductThumb(long pId, string thumbPath)
        {
            //Comment:
            /*
             * check source file is exist on ftp server or not
             * if exist, download image on local server from ftp
             * delete thumb file if  already exist
             * upload downloaded file with thumb name.
             */
            string msg = string.Empty;

            try
            {
                if (IsFileExists(thumbPath))
                {
                    BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                    string lpath = string.Empty;
                    if (imgPath.IMAGE_HTTP.EndsWith("/"))
                    {
                        lpath = thumbPath.Replace(imgPath.IMAGE_HTTP, "/");
                    }
                    else
                    {
                        lpath = thumbPath.Replace(imgPath.IMAGE_HTTP, "");
                    }
                    DownloadImage(lpath.ToLower().Trim());
                    FileInfo lFInfo = new FileInfo(System.Web.HttpContext.Current.Server.MapPath(lpath.ToLower().Trim()));
                    if (lFInfo.Exists)
                    {
                        FileStream fs = File.OpenRead(lFInfo.FullName);
                        byte[] fileContents = new byte[fs.Length];
                        fs.Read(fileContents, 0, fileContents.Length);
                        fs.Close();

                        FtpImageUpload fiup = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                        fiup.DeleteImage(imgPath.IMAGE_FTP + lpath.Remove(lpath.LastIndexOf("/")).ToLower().Trim() + "/" + GetProductThumbName(pId.ToString()));

                        System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath.Remove(lpath.LastIndexOf("/")).ToLower().Trim() + "/" + GetProductThumbName(pId.ToString())));
                        //request.UseBinary = true;
                        //request.KeepAlive = true;
                        request.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                        request.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                        request.ContentLength = fileContents.Length;

                        // This example assumes the FTP site uses anonymous logon.
                        //request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");
                        Stream requestStream = request.GetRequestStream();
                        requestStream.Write(fileContents, 0, fileContents.Length);
                        requestStream.Close();

                        lFInfo.Delete();
                    }
                    else
                    {
                        msg = "File not exist on local to create thumb image!!";
                    }
                }
                else
                {
                    msg = "File not exist on server to create thumb image!!";
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[EditProductThumb]", "Can't Edit Product's Thumb Image in Method !" + Environment.NewLine + ex.Message);
            }
            return msg;
        }

        /// <summary>
        /// This function check whether given path of file is exist or not on server 
        /// </summary>
        /// <param name="imgPath">ReadConfig class object</param>
        /// <param name="lpath">Path of a directory</param>
        /// <returns>true or false</returns>
        public bool IsFileExists(string lpath)
        {
            // check whterther given path is exist or not
            try
            {
                HttpWebRequest reqFTP;
                reqFTP = (HttpWebRequest)HttpWebRequest.Create(new Uri(lpath));
                try
                {

                    //reqFTP.GetResponse();
                    HttpWebResponse response = (HttpWebResponse)reqFTP.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                    response.Close();
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

        /// <summary>
        /// This function download images from ftp server to local
        /// </summary>
        /// <param name="imgPath">ReadConfig Object to access key from web.config</param>
        /// <param name="lpath">path of folder (string)</param>
        public static void DownloadImage(string lpath)
        {
            //Download the file from ftp to local server
            FtpWebRequest reqFTP;
            try
            {
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                ImageUpload imgu = new ImageUpload(System.Web.HttpContext.Current.Server);
                DirectoryInfo lDrInfo = imgu.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(lpath.Remove(lpath.LastIndexOf("/"))));
                //filePath = <<The full path where the file is to be created.>>, 
                //fileName = <<Name of the file to be created(Need not be the name of the file on FTP server).>>

                FileStream outputStream = new FileStream(System.Web.HttpContext.Current.Server.MapPath(lpath), FileMode.Create);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();

            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                   + Environment.NewLine + ex.Message + Environment.NewLine
                   + "[ImageUpload][DownloadImage]",
                   BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

                throw new BusinessLogicLayer.MyException("[DownloadImage]", "Probem in Downloading image file !!" + Environment.NewLine + ex.Message);
            }
        }


        #region UploadCategoryImage

        public bool UploadCategoryImageOnServer(HttpPostedFileBase pfile, Int64 cityId, Int32 FranchiseID, string fileName)////added Int32 FranchiseID
        {
            //comment 
            //1. create path for image to be upload and create directory of set path on local server
            //2. Save new image on local server
            //3. Upload imges from local server to FTP server.
            try
            {
                int lMaxContent = GetMaxImageSize();
                if (pfile.ContentLength <= lMaxContent)       //1 KB = 1024 Bytes, 100 KB = 102400 Bytes
                {
                    BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                    //1------------------------------------------------------------------------------------------------------
                    DirectoryInfo lDrInfo = this.CreateDirectory(Path.Combine(this.server.MapPath("~/" + ProductUpload.IMAGE_ROOTPATH), cityId.ToString()));////doubt
                    string rootpath = Path.Combine(Path.Combine(this.server.MapPath("~/" + ProductUpload.IMAGE_ROOTPATH), cityId.ToString()), fileName);////doubt
                    //2---------------------------------
                    pfile.SaveAs(rootpath);

                    //3--------------------------------------------------------------------------------------------------------------------
                    //FtpWebRequest req = (FtpWebRequest)WebRequest.Create(imgPath.CATEGORY_IMAGE_FTP + "/" + cityId.ToString() + "/" + fileName);////hide
                    FtpWebRequest req = (FtpWebRequest)WebRequest.Create(imgPath.CATEGORY_IMAGE_FTP + "/" + cityId.ToString() + "/" + FranchiseID + "/" + fileName);////added FranchiseID
                    req.UseBinary = true;
                    req.Method = WebRequestMethods.Ftp.UploadFile;
                    req.Credentials = new NetworkCredential(imgPath.USER_NAME, imgPath.PASSWORD);
                    
                    byte[] fileData = File.ReadAllBytes(rootpath);
                    req.ContentLength = fileData.Length;
                    Stream reqStream = req.GetRequestStream();
                    reqStream.Write(fileData, 0, fileData.Length);
                    reqStream.Close();

                    lDrInfo.Delete(true);
                    return true;                    
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

        #region UploadHomePageImage

        public bool UploadHomePageImageOnServer(HttpPostedFileBase pfile, Int64 cityId,Int32 FranchiseID,Int64 blockTypeId, string fileName)////added Int32 FranchiseID
        {
            //comment 
            //1. Check contentlength is less than max limit
            //2. Check image extension
            //3. Create path for image to be upload and create directory of set path on local server
            //4. Save new image on local server
            //5. Create directory on ftp server
            //6. Upload imges from local server to FTP server.
            try
            {
                int lMaxContent = GetMaxImageSize();
                //1-------------------------------------------
                if (pfile.ContentLength <= lMaxContent)       //1 KB = 1024 Bytes, 100 KB = 102400 Bytes
                {
                    //2------------------------------------------------
                    string[] file_extensions = { ".jpg", ".jpeg", ".png",".gif", ".JPEG", ".JPG", ".PNG",".GIF" };
                    if (file_extensions.Contains(Path.GetExtension(pfile.FileName)))
                    {
                        BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                        //3 & 4----------------------------------------------------------------------------------------------------
                        DirectoryInfo lDrInfo = this.CreateDirectory(Path.Combine(this.server.MapPath("~/" + ProductUpload.IMAGE_ROOTPATH), cityId.ToString()));////doubt
                        string rootpath = Path.Combine(Path.Combine(this.server.MapPath("~/" + ProductUpload.IMAGE_ROOTPATH), cityId.ToString()), fileName);////doubt
                                               
                        pfile.SaveAs(rootpath);

                        //5----------------------------------------------------------------------
                       // string targetfolderPath = cityId.ToString() + "/" + blockTypeId.ToString();////hide
                        string targetfolderPath = cityId.ToString() + "/" + FranchiseID + "/" + blockTypeId.ToString();////added
                        if (CreateDirectoryOnFtp(imgPath.HOME_IMAGE_FTP, targetfolderPath))
                        {
                            //6-----------------------------------------------------------------------------
                          
                            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(imgPath.HOME_IMAGE_FTP + "/" + targetfolderPath + "/" + fileName);
                            req.UseBinary = true;
                            //-------
                            //req.EnableSsl = true;
                            //req.UsePassive = true;
                            //-----
                            req.Method = WebRequestMethods.Ftp.UploadFile;
                            req.Credentials = new NetworkCredential(imgPath.USER_NAME, imgPath.PASSWORD);

                            byte[] fileData = File.ReadAllBytes(rootpath);
                            req.ContentLength = fileData.Length;
                            Stream reqStream = req.GetRequestStream();
                            reqStream.Write(fileData, 0, fileData.Length);
                            reqStream.Close();
                        }
                        lDrInfo.Delete(true);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //file not found or path not exists 
                //return false;
                throw new BusinessLogicLayer.MyException("[UploadHomePageImageOnServer]", "Can't Upload Home Page Image On Server in Method !" + Environment.NewLine + ex.Message);
            }
            return false;
        }

        //***New Method added for deleting existing image from FTP server By Harshada2***//
        public bool DeleteHomePageImageOnServer(HttpPostedFileBase pfile, Int64 cityId, Int32 FranchiseID, Int64 blockTypeId, string fileName)////added Int32 FranchiseID
        {
            //comment 
            //1. Check contentlength is less than max limit
            //2. Check image extension
            //3. Create path for image to be upload and create directory of set path on local server
            //4. Save new image on local server
            //5. Create directory on ftp server
            //6. Upload imges from local server to FTP server.
            try
            {
                int lMaxContent = GetMaxImageSize();
                //1-------------------------------------------
                if (pfile.ContentLength <= lMaxContent)       //1 KB = 1024 Bytes, 100 KB = 102400 Bytes
                {
                    //2------------------------------------------------
                    string[] file_extensions = { ".jpg", ".jpeg", ".png", ".gif", ".JPEG", ".JPG", ".PNG", ".GIF" };
                    if (file_extensions.Contains(Path.GetExtension(pfile.FileName)))
                    {
                        BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                        //3 & 4----------------------------------------------------------------------------------------------------
                        DirectoryInfo lDrInfo = this.CreateDirectory(Path.Combine(this.server.MapPath("~/" + ProductUpload.IMAGE_ROOTPATH), cityId.ToString()));////doubt
                        string rootpath = Path.Combine(Path.Combine(this.server.MapPath("~/" + ProductUpload.IMAGE_ROOTPATH), cityId.ToString()), fileName);////doubt

                        pfile.SaveAs(rootpath);

                        //5----------------------------------------------------------------------
                        // string targetfolderPath = cityId.ToString() + "/" + blockTypeId.ToString();////hide
                        string targetfolderPath = cityId.ToString() + "/" + FranchiseID + "/" + blockTypeId.ToString();////added
                        if (CreateDirectoryOnFtp(imgPath.HOME_IMAGE_FTP, targetfolderPath))
                        {
                            //6-----------------------------------------------------------------------------

                            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(imgPath.HOME_IMAGE_FTP + "/" + targetfolderPath + "/" + fileName);
                           string path=imgPath.HOME_IMAGE_FTP + "/" + targetfolderPath + "/" + fileName;
                           bool IsFileExist= CheckIfFileExistsOnServer(path, imgPath.USER_NAME, imgPath.PASSWORD);

                            if (IsFileExist)
                            {
                                req.Method = WebRequestMethods.Ftp.DeleteFile;
                                req.Credentials = new NetworkCredential(imgPath.USER_NAME, imgPath.PASSWORD);

                                using (FtpWebResponse response = (FtpWebResponse)req.GetResponse())
                                {
                                    return true;
                                }
                            }

                        }
                        lDrInfo.Delete(true);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //file not found or path not exists 
                //return false;
                throw new BusinessLogicLayer.MyException("[UploadHomePageImageOnServer]", "Can't Upload Home Page Image On Server in Method !" + Environment.NewLine + ex.Message);
            }
            return false;
        }

        //***New Method added to Check image is exist or not on FTP server***//
        public bool CheckIfFileExistsOnServer(string path,string user, string passowrd)
        {
            var request = (FtpWebRequest)WebRequest.Create(path);
            request.Credentials = new NetworkCredential(user, passowrd);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return false;
            }
            return false;
        }

        private bool CreateDirectoryOnFtp(string ftpPath, string targetfolderPath)
        {
            // check one by one all the directory specified in path is exist or not on ftp server.
            // if not exist then make directory on ftp.
            try
            {
                FtpWebRequest reqFTP;
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                string target = ftpPath;

                string[] folders = targetfolderPath.Trim().Split('/');

                foreach (string str in folders)
                {
                    try
                    {
                        //1---------------------------------------------------
                        target += "/" + str;
                        reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(target + "/"));
                        reqFTP.UseBinary = true;
                        reqFTP.KeepAlive = true;
                        reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                        reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                        reqFTP.GetResponse();
                    }
                    catch (WebException wex)
                    {
                        FtpWebResponse response = (FtpWebResponse)wex.Response;
                        if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                        {
                            //2-------------------------------------------------------
                            try
                            {
                                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(target));
                                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                                reqFTP.UseBinary = true;
                                reqFTP.KeepAlive = true;
                                reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                                response = (FtpWebResponse)reqFTP.GetResponse();
                                Stream ftpStream = response.GetResponseStream();

                                ftpStream.Close();
                                response.Close();
                            }
                            catch
                            {
                                //file not found or path not exists 
                                return false;
                            }
                        }
                    }
                }
            }
            catch
            {
                //file not found or path not exists 
                return false;
            }
            return true;
        }
        #endregion
    }
}
