using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessLogicLayer
{
    public class FtpImageUpload : ProductUpload
    {
        public FtpImageUpload(System.Web.HttpServerUtility server) : base(server) { }

        /// <summary>
        /// Check wheather directory exist or not & if not create directory
        /// </summary>
        /// <param name="imgPath">ReadConfig class object</param>
        /// <param name="lpath">Path of a directory</param>
        /// <returns>true or false</returns>
        public bool IsPathExists(BusinessLogicLayer.ReadConfig imgPath, string lpath)
        {
            //Comment
            /*
             * 1. check for each folder and subfolder is exist or not on ftp
             * 2. if folder is not create make folder on ftp.
             */

            try
            {
                FtpWebRequest reqFTP;

                string target = imgPath.IMAGE_FTP;

                string[] folders = lpath.Trim().Remove(0, 1).Split('/');

                foreach (string str in folders)
                {
                    //1------------------------------------------------------------------------
                    try
                    {
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
                            //2--------------------------------------------------------------------
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

        /// <summary>
        /// copy images from source path to ftp server
        /// </summary>
        /// <param name="source">Source path of images</param>
        /// <param name="target">destination path of images</param>
        public void CopyImagesToFTP(string source, string target)
        {
            //source = @"E:\\New Gandhibagh Working\\MvcGandhibagh.Com\\Gandhibagh\\Content\\NonApproved_Images\\Products\\30\\Default";
            //target = "/Content/NonApproved_Images/Products/60/Default";
            
            /*
             * 1. check target path exist on ftp server or not
             * 2. check local directory path for getting images
             * 3. Upload image one by one on ftp on target path.
             */
            
            try
            {
                FileInfo[] lFileInfo = null;
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                //1----------------------------------------------------------------------
                // Check wheather directory exist or not & if not create directory
                if (IsPathExists(imgPath, target))
                {
                    //2--------------------------------------------------------------
                    DirectoryInfo lDrInfo = new DirectoryInfo(source);
                    if (lDrInfo.Exists)
                    {
                        lFileInfo = lDrInfo.GetFiles();
                    }
                    //3-------------------------------------------------------------------
                    foreach (FileInfo fl in lFileInfo)
                    {
                        FtpWebRequest req = (FtpWebRequest)WebRequest.Create(imgPath.IMAGE_FTP + target + "/" + fl.Name);
                        req.UseBinary = true;
                        req.KeepAlive = true;
                        req.Method = WebRequestMethods.Ftp.UploadFile;
                        req.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                        FileStream fs = File.OpenRead(fl.DirectoryName + "\\" + fl.Name);
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        fs.Close();

                        Stream ftpstream = req.GetRequestStream();
                        ftpstream.Write(buffer, 0, buffer.Length);
                        ftpstream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[CopyImagesToFTP]", "Probem in Saving image to Ftp server!!" + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// This function used to delete shop image from ftp
        /// </summary>
        /// <param name="fileName">name of image file</param>
        /// <returns>true if deleted or false if not</returns>
        public bool DeleteFtpShopImage(string fileName)
        {
            /*
            * 1. check file path exist on ftp server or not
            * 2. if exist delete file from ftp
            */
            
            try
            {
                string target = fileName.Trim().Substring(fileName.ToLower().IndexOf("/" + ProductUpload.IMAGE_ROOTPATH.ToLower()));
                fileName = target.Substring(target.LastIndexOf('/'));
                target = target.Replace(fileName, "");
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                //1----------------------------------------------------------------------------------------------------
                //Check whether specified path is exists on server
                if (IsPathExists(imgPath, target, fileName))
                {
                    //2--------------------------------------------------------------------------------------------
                    FtpWebRequest reqFTP;
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + target + fileName));
                    reqFTP.KeepAlive = false;
                    reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                    reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());

                    string result = String.Empty;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    //long size = response.ContentLength;
                    //Stream datastream = response.GetResponseStream();
                    //StreamReader sr = new StreamReader(datastream);
                    //result = sr.ReadToEnd();
                    //sr.Close();
                    //datastream.Close();
                    response.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FtpImageUpload][DeleteFtpShopImage]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                //throw new BusinessLogicLayer.MyException("[DeleteFTPImage]", "Probem in Deleting image from Ftp server!!" + Environment.NewLine + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// This function used to delete product images from ftp
        /// </summary>
        /// <param name="fileName">name of image file</param>
        /// <param name="pId">product id</param>
        /// <param name="pName">product name</param>
        /// <param name="pSubFolderName">Sub folder Name of product according to stock (i.e. Color Name)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'DEFAULT'</param>
        /// <param name="pImgtype">Image type i.e Approved or NonApproved</param>
        /// <returns>true if deleted or false if not</returns>
        public bool DeleteFtpProductImage(string fileName, long pId, string pName, string pSubFolderName, string pSubFolderPrefix, ProductUpload.IMAGE_TYPE pImgtype)
        {/*
            * 1. check file path exist on ftp server or not
            * 2. if exist load all image path from selected folder on ftp server
            * 3. delete images related to specified name of image file from ftp
            */
            try
            {
                string target = fileName.Trim().Substring(fileName.ToLower().IndexOf("/" + ProductUpload.IMAGE_ROOTPATH.ToLower()));
                fileName = target.Substring(target.LastIndexOf('/'));
                target = target.Replace(fileName, "");

                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                //1------------------------------------------------------------------------------------------------
                //Check whether specified path is exists on server
                //if (IsPathExists(imgPath, target))
                if (IsPathExists(imgPath, target, fileName))
                {
                    //2----------------------------------------------------------------------------------------------
                    string[] productImages = ImageDisplay.LoadAllProductImages(pId, pSubFolderName, pSubFolderPrefix, pImgtype);
                    foreach (string str in productImages)
                    {
                        //3--------------------------------------------------------------------------------------------------
                        // delete four copies of image of selected image
                        if (str.Contains(fileName.Substring(1, fileName.IndexOf("_", pId.ToString().Trim().Length + 5))))
                        {
                            DeleteImage(imgPath.IMAGE_FTP + target + str.Substring(str.LastIndexOf('/')));
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[DeleteFTPImage]", "Probem in Deleting image from Ftp server!!" + Environment.NewLine + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// This function delete image from ftp
        /// </summary>
        /// <param name="fileName">name of image file with full path</param>
        public void DeleteImage(string fileName)
        {
            // delete requested image file from ftp server
            try
            {
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(fileName));
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FtpImageUpload][DeleteImage]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                //file not found or path not exists 
            }
        }

        /// <summary>
        /// This function used to delete all images of specified product id from ftp
        /// </summary>
        /// <param name="imgPath">ReadConfig class object</param>
        /// <param name="lpath">Path of a directory</param>
        /// <returns></returns>
        public bool DeleteAllProductImages(ReadConfig imgPath, string lpath)
        {
            try
            {        
                //getall file path from given path
                string[] prodImages = this.GetImages(imgPath, lpath);
                if (prodImages != null)
                {
                    //to delete files in directory   
                    foreach (string str in prodImages)
                    {
                        string ftpImgPath = imgPath.IMAGE_FTP + str.Trim().Substring(str.ToLower().IndexOf("/" + ProductUpload.IMAGE_ROOTPATH.ToLower()));
                        DeleteImage(ftpImgPath);
                    }
                }
                //getall Subdirectory from given path
                string[] subDirectory = this.GetSubDirectoryList(imgPath, lpath);
                if (subDirectory != null)
                {
                    //to delete files in sub-directory 
                    foreach (string fld in subDirectory)
                    {
                        this.DeleteSubFolderWithImages(imgPath, fld);
                    }
                }

                //Check whether specified path is exists on server
                if (IsPathExists(imgPath, lpath.Trim()))
                {
                    //to remove directory
                    FtpWebRequest reqFTP;
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath.Trim()));
                    reqFTP.KeepAlive = false;
                    reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                    reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    response.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FtpImageUpload][DeleteAllShopImages]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                //file not found or path not exists 
            }
            return false;
        }

        /// <summary>
        /// This function used to delete all images with directory of specified path
        /// </summary>        
        /// <param name="lpath">Path of a directory</param>
        public void DeleteSubFolderWithImages(ReadConfig imgPath, string lpath)
        {
            try
            {
                //getall file path from given path
                string[] prodImages = this.GetImages(imgPath, lpath.Trim().Substring(lpath.ToLower().IndexOf("/" + ProductUpload.IMAGE_ROOTPATH.ToLower())));
                foreach (string str in prodImages)
                {//to delete files
                    string ftpImgPath = imgPath.IMAGE_FTP + str.Trim().Substring(str.ToLower().IndexOf("/" + ProductUpload.IMAGE_ROOTPATH.ToLower()));
                    DeleteImage(ftpImgPath.ToLower());
                }
                //to delete sub directory
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath.Trim().Substring(lpath.ToLower().IndexOf("/" + ProductUpload.IMAGE_ROOTPATH.ToLower()))));
                reqFTP.KeepAlive = false;
                reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();

            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FtpImageUpload][DeleteAllShopImages]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                //file not found or path not exists 
            }
        }

        /// <summary>
        /// This function used to delete all images of specified shop id from ftp
        /// </summary>
        /// <param name="pId">product id</param>
        /// <param name="pImgtype">Image type i.e Approved or NonApproved</param>
        /// <returns></returns>
        public bool DeleteAllShopImages(long pId, ProductUpload.IMAGE_TYPE pImgtype)
        {
            //1. create path for shop images
            //2. get all images path from created shop folder path
            //3. delete all images in folder one by one from ftp server
            //4. remove directory from ftp
            try
            {
                //1-----------------------------------------------------------------
                string lpath = "/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Shops.GetDescription() + "/" + pId;
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                //2---------------------------------------------------------------------------
                string[] shopImages = this.GetImages(imgPath, lpath);
                //3-------------------------------------------------------------------------------------
                foreach (string str in shopImages)
                {
                    DeleteFtpShopImage(str);
                }

                //4----------------------------------------------------------------------------------------------
                //if (IsPathExists(imgPath, lpath.Trim()))
                {
                    FtpWebRequest reqFTP;
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath.Trim()));
                    reqFTP.UsePassive = true;
                    reqFTP.KeepAlive = false;
                    reqFTP.UseBinary = false;
                    reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                    reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    response.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FtpImageUpload][DeleteAllShopImages]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                //file not found or path not exists 
            }
            return false;
        }

        /// <summary>
        /// this function get all images from specified path from ftp server
        /// </summary>
        /// <param name="imgPath">ReadConfig class object</param>
        /// <param name="lpath">Path of a directory</param>
        /// <returns>array of image path</returns>
        public string[] GetImages(ReadConfig imgPath, string lpath)
        {
            //get listing of all files from specified path from ftp server
            StringBuilder lImgPath = new System.Text.StringBuilder();
            try
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
                    lImgPath.Append(imgPath.IMAGE_HTTP + lpath.ToString() + "/" + line);
                    lImgPath.Append("\n");
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
                    + "[FtpImageUpload][GetShopImages]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return null;
        }

        /// <summary>
        /// This function used to delete all images with directory of specified path
        /// </summary> 
        /// <param name="imgPath">ReadConfig class object</param>
        /// <param name="lpath">Path of a directory</param>        
        /// <returns></returns>        
        public string[] GetSubDirectoryList(ReadConfig imgPath, string lpath)
        {
            //get listing of all files from specified directory path from ftp server
            StringBuilder lImgPath = new System.Text.StringBuilder();
            try
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
                    lImgPath.Append(imgPath.IMAGE_HTTP + lpath.ToString() + "/" + line);
                    lImgPath.Append("\n");
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
                    + "[FtpImageUpload][GetShopImages]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return null;
        }

        /// <summary>
        /// copy file from source path to ftp server
        /// </summary>
        /// <param name="source">Source path of images</param>
        /// <param name="target">destination path of images</param>
        public void CopyFileToFTP(string source, string target)
        {
            /*
             * 1. check target path is exist on ftp server or not
             * 2. check source path is exist or not
             * 3. Upload file from source path (on local server) to target path (on ftp server)
             */

            //source = @"E:\\New Gandhibagh Working\\MvcGandhibagh.Com\\Gandhibagh\\Content\\NonApproved_Images\\Products\\30\\Default";
            //target = "/Content/NonApproved_Images/Products/60/Default";
            try
            {
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                //1---------------------------------------------------------------------------------------------
                if (IsPathExists(imgPath, target))
                {
                    //2-----------------------------------------------------------------
                    FileInfo lFileInfo = new FileInfo(source);
                    if (lFileInfo.Exists)
                    {
                        //if (IsFileExists(imgPath, target, lFileInfo.Name))
                        //{
                        //    DeleteFile(imgPath, target, lFileInfo.Name);
                        //}
                        //3------------------------------------------------------------------------------------------
                        FtpWebRequest req = (FtpWebRequest)WebRequest.Create(new Uri(imgPath.IMAGE_FTP + target + "/" + lFileInfo.Name));
                        req.EnableSsl = false;
                        req.UseBinary = true;
                        req.KeepAlive = true;
                        req.Method = WebRequestMethods.Ftp.UploadFile;
                        req.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                        FileStream fs = File.OpenRead(lFileInfo.DirectoryName + "\\" + lFileInfo.Name);
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        fs.Close();

                        Stream ftpstream = req.GetRequestStream();
                        ftpstream.Write(buffer, 0, buffer.Length);
                        ftpstream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[CopyImagesToFTP]", "Probem in Saving image to Ftp server!!" + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// copy description file from source path to ftp server
        /// </summary>
        /// <param name="source">Source path of images</param>
        /// <param name="target">destination path of images</param>
        /// <param name="filename">file name</param>
        public void CopyDescriptionFileToFTP(string source, string target, string filename)
        {
            /*
             * 1. check target path is exist on ftp server or not
             * 2. check source path is exist or not
             * 3. Check whether specified file is exists on server if yes, then delete that file.
             * 4. Upload file from source path (on local server) to target path (on ftp server)
             */
            //source = @"E:\\New Gandhibagh Working\\MvcGandhibagh.Com\\Gandhibagh\\Content\\NonApproved_Images\\Products\\30\\Default";
            //target = "/Content/NonApproved_Images/Products/60/Default";
            try
            {
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                //1-----------------------------------------------------------------------------
                if (IsPathExists(imgPath, target.Replace(filename, "")))
                {
                    //2------------------------------------------------------------------------------------
                    FileInfo lFileInfo = new FileInfo(source);
                    //Check whether specified path is exists on local
                    if (lFileInfo.Exists)
                    {
                        //3---------------------------------------------------
                        //Check whether specified path is exists on server
                        if (IsFileExists(imgPath, target, lFileInfo.Name))
                        {
                            DeleteImage(imgPath.IMAGE_FTP + target);
                        }
                        //4------------------------------------------------------------
                        FtpWebRequest req = (FtpWebRequest)WebRequest.Create(imgPath.IMAGE_FTP + target);
                        req.UseBinary = true;
                        req.KeepAlive = true;
                        req.Method = WebRequestMethods.Ftp.UploadFile;
                        req.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                        FileStream fs = File.OpenRead(lFileInfo.FullName);
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        fs.Close();

                        Stream ftpstream = req.GetRequestStream();
                        ftpstream.Write(buffer, 0, buffer.Length);
                        ftpstream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[CopyImagesToFTP]", "Probem in Saving image to Ftp server!!" + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// This function check whether given path of folder is exist or not on server 
        /// </summary>
        /// <param name="imgPath">ReadConfig class object</param>
        /// <param name="lpath">Path of a directory</param>
        /// <returns>true or false</returns>
        public bool IsFileExists(BusinessLogicLayer.ReadConfig imgPath, string lpath, string filename)
        {
            // check wheather specified file is exist on ftp server or not (for description file)
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath + "/" + filename));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                try
                {

                    //reqFTP.GetResponse();
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    //if (response.ContentLength > 0)
                    //{
                    //    return true;
                    //}
                    response.Close();
                    return true;
                    //return false;
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
        /// This function check whether given path of folder is exist or not on server (for images)
        /// </summary>
        /// <param name="imgPath">ReadConfig class object</param>
        /// <param name="lpath">Path of a directory</param>
        /// <returns>true or false</returns>
        public bool IsPathExists(BusinessLogicLayer.ReadConfig imgPath, string lpath, string filename)
        {
            // check wheather specified file is exist on ftp server or not (for images)
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath + "/" + filename));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                try
                {

                    //reqFTP.GetResponse();
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    response.Close();
                    return true;
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
        /// This function delete file from ftp
        /// </summary>
        /// <param name="imgPath">ReadConfig class object</param>
        /// <param name="lpath">Path of a directory</param>
        /// <param name="filename">file name</param>
        private void DeleteFile(BusinessLogicLayer.ReadConfig imgPath, string lpath, string filename)
        {
            // delete requested file from ftp server
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath + "/" + filename));
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FtpImageUpload][DeleteFile]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                //file not found or path not exists 
            }
        }

        /// <summary>
        /// copy images from source path to ftp server
        /// </summary>
        /// <param name="source">Source path of images</param>
        /// <param name="target">destination path of images</param>
        /// <param name="tempId">temp product id</param>
        /// <param name="newId">new product id</param>
        public void CopyProductImagesToFTP(string source, string target, long tempId, long newId)
        {
            /*
             * 1. check target path is exist on ftp server or not
             * 2. check source path is exist or not
             * 3. Upload file from source path (on local server) to target path (on ftp server)
             */
            //source = @"E:\\New Gandhibagh Working\\MvcGandhibagh.Com\\Gandhibagh\\Content\\NonApproved_Images\\Products\\30\\Default";
            //target = "/Content/NonApproved_Images/Products/60/Default";
            try
            {
                FileInfo[] lFileInfo = null;
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                //1----------------------------------------------------------------------------
                if (IsPathExists(imgPath, target))
                {
                    //2--------------------------------------------------------------------------------
                    DirectoryInfo lDrInfo = new DirectoryInfo(source);
                    if (lDrInfo.Exists)
                    {
                        lFileInfo = lDrInfo.GetFiles();
                    }
                    //3--------------------------------------------------------------------------------------------------
                    //Replace old product id with new id in image name
                    foreach (FileInfo fl in lFileInfo)
                    {
                        FtpWebRequest req = (FtpWebRequest)WebRequest.Create(imgPath.IMAGE_FTP + target + "/" + fl.Name.ToLower().Replace(("gb_" + tempId + "_").Trim(), ("gb_" + newId + "_").Trim()));
                        req.UseBinary = true;
                        req.KeepAlive = true;
                        req.Method = WebRequestMethods.Ftp.UploadFile;
                        req.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                        FileStream fs = File.OpenRead(fl.DirectoryName + "\\" + fl.Name);
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        fs.Close();

                        Stream ftpstream = req.GetRequestStream();
                        ftpstream.Write(buffer, 0, buffer.Length);
                        ftpstream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[CopyImagesToFTP]", "Probem in Saving image to Ftp server!!" + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// Delete all images of current product variant
        /// </summary>
        /// <param name="pId">product ID</param>
        /// <param name="pSubFolderName">Name of folder i.e. Stock color name or default (string)</param>
        /// <param name="pSubFolderPrefix">Folder Prefix like 'Default' (string)</param>
        /// <param name="pImgtype">Image Type i.e. Approved or NonApproved</param>
        public void DeleteProductImages(long pId, string pSubFolderName, string pSubFolderPrefix, ProductUpload.IMAGE_TYPE pImgtype)
        {
            // 1. create path for product images to delete
            // 2.delete images from created path

            //1----------------------------------------------------------
            IMAGE_FOR lImgfor = IMAGE_FOR.Products;
            StringBuilder lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + lImgfor.GetDescription());

            if (!string.IsNullOrEmpty(pId.ToString().Trim()))
            {
                lpath.Append("/" + pId.ToString().Trim());
                string foldeName = lpath.ToString();
                lpath.Append(string.IsNullOrEmpty(pSubFolderName.Trim()) ? "/default" : (string.IsNullOrEmpty(pSubFolderPrefix.Trim()) ? "/" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower()) : "/" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderPrefix.Trim().ToLower()) + "_" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pSubFolderName.Trim().ToLower())));
                
                //2----------------------------------------------------------------------
                FtpImageUpload ftpimg = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                ftpimg.DeleteAllProductImages(imgPath, lpath.ToString());
            }
        }

        /// <summary>
        /// Delete all images of current product 
        /// </summary>
        /// <param name="pId">product ID</param>
        /// <param name="pImgtype">Image Type i.e. Approved or NonApproved</param>
        public void DeleteProductImages(long pId, ProductUpload.IMAGE_TYPE pImgtype)
        {
            // 1. create path for product images to delete
            // 2.delete images from created path

            //1----------------------------------------------------
            IMAGE_FOR lImgfor = IMAGE_FOR.Products;
            StringBuilder lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + lImgfor.GetDescription());

            if (!string.IsNullOrEmpty(pId.ToString().Trim()))
            {
                lpath.Append("/" + pId.ToString().Trim());
                string foldeName = lpath.ToString();

                //2-----------------------------------------------------
                FtpImageUpload ftpimg = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                ftpimg.DeleteAllProductImages(imgPath, lpath.ToString());
            }
        }
    }

}
