using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class ImageApproval
    {
        /// <summary>
        /// this function upload the imges of product from non-approved location to approved location
        /// </summary>
        /// <param name="tempProductId">Product id in tempproduct table</param>
        /// <param name="newProductId">Product id in product table </param>
        /// <param name="pSubFolderName">Sub folder Name of product according to stock (i.e. Color Name or default)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'Default'</param>
        /// <returns>path of local folder crated for download purpose</returns>
        public static string ProductImagesApproval(long tempProductId, long newProductId, string pSubFolderName, string pSubFolderPrefix)
        {
            // 1. create source path with tempproductid to download the files
            // 2. check whether source path is exist on server.
            // 3. create target path with newproductid to upload the files
            // 4. if exist, download the source files on local server
            // 5. Copy downloaded files to target path on ftp server
            // 6. Delete downloaded files from local server.
            try
            {
                //1-----------------------------------------------------------------------
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                FtpImageUpload fimg = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                StringBuilder lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + ProductUpload.IMAGE_TYPE.NonApproved.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + tempProductId);

                string foldeName = lpath.ToString();
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
                //2-----------------------------------------------------------------
                //Check whether specified path is exists on server
                if (IsPathExists(imgPath, lpath, pSubFolderName, pSubFolderPrefix))
                {
                    //3---------------------------------------------------------------------------
                    StringBuilder newPath = new StringBuilder(lpath.ToString());
                    newPath.Replace("/" + ProductUpload.IMAGE_ROOTPATH + "/" + ProductUpload.IMAGE_TYPE.NonApproved.GetDescription(), "/" + ProductUpload.IMAGE_ROOTPATH + "/" + ProductUpload.IMAGE_TYPE.Approved.GetDescription());
                    newPath.Replace("/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + tempProductId, "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + newProductId);
                    
                    //4-----------------------------------------------------------------------------------
                    DownloadImages(imgPath, lpath.ToString().Trim());                                               //download images to local 
                    
                    //5---------------------------------------------------------------------------------
                    fimg.CopyProductImagesToFTP(System.Web.HttpContext.Current.Server.MapPath(lpath.ToString().Trim()), newPath.ToString().Trim(),tempProductId,newProductId);      //copy images from local to ftp server
                   
                    //6---------------------------------------------------------------------------------------
                    bool isDeleted = fimg.DeleteAllProductImages(imgPath, lpath.ToString().Trim());                 //delete images from old ftp location

                    return System.Web.HttpContext.Current.Server.MapPath(foldeName.Trim());

                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductImagesApproval]", "Probem in Approving Product image !!" + Environment.NewLine + ex.Message);
            }
            return string.Empty;
        }

        /// <summary>
        /// this function upload the imges of shop from non-approved location to approved location
        /// </summary>
        /// <param name="pId">Shop ID</param>
        /// <returns>path of local folder crated for download purpose</returns>
        public static string ShopImagesApproval(long pId)
        {
            // 1. create source path to download the files
            // 2. create target path to upload the files
            // 3. check whether source path is exist on server.
            // 4. if exist, download the source files on local server
            // 5. Copy downloaded files to target path on ftp server
            // 6. Delete downloaded files from local server.
            try
            {
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                FtpImageUpload fimg = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                //1-------------------------------------------------------------------
                string lpath = "/" + ProductUpload.IMAGE_ROOTPATH + "/" + ProductUpload.IMAGE_TYPE.NonApproved.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Shops.GetDescription() + "/" + pId;
                //2-------------------------------------------------------------------------
                string newPath = "/" + ProductUpload.IMAGE_ROOTPATH + "/" + ProductUpload.IMAGE_TYPE.Approved.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Shops.GetDescription() + "/" + pId;
                //3--------------------------------------------------------------------------------------
                //Check whether specified path is exists on server
                if (IsPathExists(imgPath.IMAGE_FTP + lpath.Trim()))
                {
                    //4----------------------------------------------------
                    DownloadImages(imgPath, lpath.Trim());                                 //download images to local 
                    //5-----------------------------------------------------------
                    fimg.CopyImagesToFTP(System.Web.HttpContext.Current.Server.MapPath(lpath.Trim()), newPath.Trim());   //copy images from local to ftp server
                    //6---------------------------------------------------------------------------------------------------
                    bool isDeleted = fimg.DeleteAllShopImages(pId, ProductUpload.IMAGE_TYPE.NonApproved);           //delete images from old ftp location
                    return System.Web.HttpContext.Current.Server.MapPath(lpath.Trim());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShopImagesApproval]", "Probem in Approving Shop image !!" + Environment.NewLine + ex.Message);
            }
            return string.Empty;
        }

        /// <summary>
        /// this function upload the description of shop from non-approved location to approved location
        /// </summary>
        /// <param name="tempProductId">Product id in tempproduct table</param>
        /// <param name="newProductId">Product id in product table</param>
        /// <returns>path of local folder crated for download purpose</returns>
        public static string ProductDescriptionApproval(long tempProductId, long newProductId)
        {
            // 1. create source path with tempproductid to download the file
            // 2. create target path with newproductid to upload the file
            // 3. check whether source file is exist on server.
            // 4. if exist, download the source file on local server
            // 5. Copy downloaded file to target path on ftp server
            // 6. Delete downloaded file from local server.

            try
            {
                string filename = "/" + ProductUpload.DESC_FILE_NAME;
                //1---------------------------------------------------------------------
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                FtpImageUpload fimg = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                StringBuilder lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + ProductUpload.IMAGE_TYPE.NonApproved.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + tempProductId + filename);

                string foldeName = lpath.ToString().Replace(filename, "");

                //2---------------------------------------------------------------------------------
                StringBuilder newPath = new StringBuilder(lpath.ToString());
                newPath.Replace("/" + ProductUpload.IMAGE_ROOTPATH + "/" + ProductUpload.IMAGE_TYPE.NonApproved.GetDescription(), "/" + ProductUpload.IMAGE_ROOTPATH + "/" + ProductUpload.IMAGE_TYPE.Approved.GetDescription());
                newPath.Replace("/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + tempProductId, "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + newProductId);
               
                //3-------------------------------------------------------------------------------
                //Check whether specified path is exists on server
                if (fimg.IsFileExists(imgPath, lpath.ToString().Trim().Replace(filename, ""), filename))
                {
                    //4----------------------------------------------------------------------------
                    DownloadDescriptionFile(imgPath, lpath.ToString().Trim(), filename);           //download file to local 
                    //5--------------------------------------------------------------------------------------------
                    fimg.CopyDescriptionFileToFTP(System.Web.HttpContext.Current.Server.MapPath(lpath.ToString().Trim()), newPath.ToString().Trim(), filename);  //copy description file from local to ftp server
                    //6------------------------------------------------------------------------------------------------------------
                    bool isDeleted = fimg.DeleteFtpShopImage(lpath.ToString().Trim());               //delete file from old ftp location
                }
                return System.Web.HttpContext.Current.Server.MapPath(foldeName.Trim());
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductDescriptionApproval]", "Probem in Product Description Approval !!" + Environment.NewLine + ex.Message);
            }
            return string.Empty;
        }

        /// <summary>
        /// This function check whether given path of folder is exist or not on server
        /// </summary>
        /// <param name="lpath">path string</param>
        /// <returns>boolean value indicating path is exists or not</returns>
        public static bool IsPathExists(string lpath)
        {
            // check wheather specified path is exist on ftp server or not
            try
            {
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(lpath + "/"));
                reqFTP.UseBinary = true;
                reqFTP.KeepAlive = true;
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                reqFTP.GetResponse();
                return true;
            }
            catch (WebException wex)
            {
                FtpWebResponse response = (FtpWebResponse)wex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    //file not found or path not exists 
                    return false;
                }
            }
            catch (Exception ex)
            {
                //file not found or path not exists 
                return false;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ImageApproval][IsPathExists]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return false;
        }

        /// <summary>
        /// This function check whether given path of folder is exist or not on server
        /// </summary>
        /// <param name="imgPath">ReadConfig Object to access key from web.config</param>
        /// <param name="lpath">path of folder (StringBuilder)</param>
        /// <param name="pSubFolderName">Name of folder i.e. Stock color name or default (string)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'Default' (string)</param>
        /// <returns>boolean value indicating path is exists or not</returns>
        private static bool IsPathExists(BusinessLogicLayer.ReadConfig imgPath, System.Text.StringBuilder lpath, string pSubFolderName, string pSubFolderPrefix)
        {
            //Comment
            /*
             * 1. check wheather folder is exist on ftp server or not 
             * 2. if not present check for other combination "default_foldername"
             */
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath.ToString() + "/"));
                reqFTP.UseBinary = true;
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                // checking for two different combination  
                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        reqFTP.GetResponse();
                        return true;
                    }
                    catch (WebException wex)
                    {
                        if (wex.Response != null)
                        {
                            FtpWebResponse response = (FtpWebResponse)wex.Response;
                            //if file not found or path not exists 
                            if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                            {
                                //check for other combination "default_foldername"
                                if (i == 0 && pSubFolderPrefix.Equals(string.Empty) && pSubFolderName.ToLower() != "default")
                                {
                                    lpath.Replace(pSubFolderName, "Default_" + pSubFolderName);
                                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath.ToString()));
                                    reqFTP.UseBinary = true;
                                    reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                                    reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                   + Environment.NewLine + ex.Message + Environment.NewLine
                   + "[ImageApproval][IsPathExists]",
                   BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
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
        public static void DownloadImages(ReadConfig imgPath, string lpath)
        {
            //Download the image file from ftp to local server
            FtpWebRequest reqFTP;
            try
            {
                string[] prodImgs = GetImages(imgPath, lpath.Trim());
                ImageUpload imgu = new ImageUpload(System.Web.HttpContext.Current.Server);
                DirectoryInfo lDrInfo = imgu.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(lpath.Trim()));
                //filePath = <<The full path where the file is to be created.>>, 
                //fileName = <<Name of the file to be created(Need not be the name of the file on FTP server).>>
                foreach (string img in prodImgs)
                {
                    FileStream outputStream = new FileStream(System.Web.HttpContext.Current.Server.MapPath(lpath.Trim()) + "\\" + img, FileMode.Create);

                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath.ToString() + "/" + img));
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
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                   + Environment.NewLine + ex.Message + Environment.NewLine
                   + "[ImageApproval][DownloadImages]",
                   BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

                throw new BusinessLogicLayer.MyException("[DownloadImages]", "Probem in Downloading image !!" + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// This function download description file from ftp server to local
        /// </summary>
        /// <param name="imgPath">ReadConfig Object to access key from web.config</param>
        /// <param name="lpath">path of folder (string)</param>
        /// <param name="filename">description file name</param>
        public static void DownloadDescriptionFile(ReadConfig imgPath, string lpath,string filename)
        {
            //Download the description file from ftp to local server
            FtpWebRequest reqFTP;
            try
            {
                //string[] prodImgs = GetImages(imgPath, lpath.Trim());
                ImageUpload imgu = new ImageUpload(System.Web.HttpContext.Current.Server);
                DirectoryInfo lDrInfo = imgu.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(lpath.Trim().Replace(filename, "")));
                //filePath = <<The full path where the file is to be created.>>, 
                //fileName = <<Name of the file to be created(Need not be the name of the file on FTP server).>>

                FileStream outputStream = new FileStream(System.Web.HttpContext.Current.Server.MapPath(lpath.Trim()), FileMode.Create);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath.ToString()));
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
                   + "[ImageApproval][DownloadDescriptionFile]",
                   BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

                throw new BusinessLogicLayer.MyException("[DownloadDescriptionFile]", "Probem in Download Description File !!" + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// this function get images from ftp server
        /// </summary>
        /// <param name="imgPath">ReadConfig Object to access key from web.config</param>
        /// <param name="lpath">path of folder (string)</param>
        /// <returns>array of image path</returns>
        public static string[] GetImages(ReadConfig imgPath, string lpath)
        {
            //get listing of all files from specified path from ftp server
            StringBuilder lImgPath = new System.Text.StringBuilder();
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath.ToString()));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse(); ;
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    //if (!line.Contains("LOGO"))
                    //{
                    lImgPath.Append(line);
                    lImgPath.Append("\n");
                    //}
                    line = reader.ReadLine();
                }
                if (lImgPath.Length > 0)
                {
                    lImgPath.Remove(lImgPath.ToString().LastIndexOf('\n'), 1);
                    reader.Close();
                    response.Close();
                    return lImgPath.ToString().Split('\n');
                }
            }
            catch (Exception ex)
            {
                lImgPath = null;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                   + Environment.NewLine + ex.Message + Environment.NewLine
                   + "[ImageApproval][GetImages]",
                   BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

                throw new BusinessLogicLayer.MyException("[GetImages]", "Probem in Getting image !!" + Environment.NewLine + ex.Message);
            }
            return null;
        }
    }
}
