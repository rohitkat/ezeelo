using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessLogicLayer
{
    public class DescriptionFileUpload : ProductUpload
    {
        public DescriptionFileUpload(System.Web.HttpServerUtility server) : base(server) { }

        /// <summary>
        /// To Upload Description file 
        /// </summary>
        /// <param name="pId">Product Id</param>
        /// <param name="txt">text to upload</param>
        /// <param name="pImgtype">File type i.e Approved or NonApproved</param>
        public void FileUpload(long pId, string txt, IMAGE_TYPE pImgtype)
        {
            //comment 
            //1. create path for file to be upload
            //2. create directory of above set path on local server         
            //3. Save new file on local server
            //4. Upload file from local server to FTP server.
            try
            {
                if (!string.IsNullOrEmpty(txt.Trim()) && pId > 0)
                {
                    IMAGE_FOR lImgfor = IMAGE_FOR.Products;
                    string lpath = "~/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + lImgfor.GetDescription() + "/" + pId;
                    DirectoryInfo lDrInfo = this.CreateDirectory(this.server.MapPath(lpath.ToString()));
                    if (SaveDescriptionFile(pId, txt, lpath.ToString()))
                    {
                        FtpImageUpload fimg = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                        fimg.CopyFileToFTP(Path.Combine(this.server.MapPath(lpath.ToString()), "description.html"), lpath.ToString().Remove(0, 1));
                    }
                    lDrInfo.Delete(true);
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[FileUpload]", "Can't Upload file !" + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">HttpPostedFileBase</param>
        /// <param name="guid">FileName Uniq Generated</param>
        /// <param name="extension">File Extension</param>
        public void UploadCareerFile(HttpPostedFileBase file, string guid, string extension)
        {
            try
            {
                if (!string.IsNullOrEmpty(guid.Trim()))
                {

                    string lpath = "~/" + ProductUpload.CAREER_ROOTPATH + "/" + guid + extension;

                    string filePath = System.Web.HttpContext.Current.Server.MapPath(lpath.ToString());
                    //DirectoryInfo lDrInfo = this.cre(System.Web.HttpContext.Current.Server.MapPath(lpath.ToString()));
                    file.SaveAs(filePath);
                    //if (SaveCareerFile(guid, extension, lpath.ToString()))
                    
                        FtpImageUpload fimg = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                        fimg.CopyFileToFTP(filePath, lpath.ToString().Remove(0, 1).Replace("/" + guid + extension, string.Empty));

                        FileInfo obj = new FileInfo(filePath);
                        obj.Delete();
                    

                    //lDrInfo.Delete(true);
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[FileUpload]", "Can't Upload file !" + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// To Upload Description file on FTP server
        /// </summary>
        /// <param name="pId">Product Id</param>
        /// <param name="txt">local file path from which description file to be copied on ftp server</param>
        /// <param name="pImgtype">File type i.e Approved or NonApproved</param>
        public void ProductDescriptionFileUploadOnFTPServer(long pId, HttpPostedFileBase file, IMAGE_TYPE pImgtype)
        {
            //comment 
            //1. create path for file to be upload
            //2. create directory of above set path on local server         
            //3. Save new file on local server
            //4. Upload file from local server to FTP server.
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    IMAGE_FOR lImgfor = IMAGE_FOR.Products;
                    string localFilePath = "~/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + lImgfor.GetDescription() + "/" + pId;

                    if (!string.IsNullOrEmpty(localFilePath.Trim()) && pId > 0)
                    {

                        string ftpPath = "~/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + lImgfor.GetDescription() + "/" + pId;
                        DirectoryInfo lDrInfo = this.CreateDirectory(this.server.MapPath(localFilePath.ToString()));
                        if (SaveProductDescriptionFileOnLocalServer(pId, file, pImgtype))
                        {
                            FtpImageUpload fimg = new FtpImageUpload(System.Web.HttpContext.Current.Server);
                            fimg.CopyFileToFTP(Path.Combine(this.server.MapPath(localFilePath.ToString()),"description.html"), ftpPath.ToString().Remove(0, 1));
                        }
                        lDrInfo.Delete(true);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[FileUpload]", "Can't Upload file !" + Environment.NewLine + ex.Message);
            }
        }

      /// <summary>
      /// Save file on local server
      /// </summary>
      /// <param name="pId">Product ID</param>
      /// <param name="file">Posted File</param>
      /// <param name="pImgtype">Image Type : Approved or Non- Approved </param>
      /// <returns></returns>
        public bool SaveProductDescriptionFileOnLocalServer(long pId, HttpPostedFileBase file, IMAGE_TYPE pImgtype)
        {
            // craete path to save file on local server and save the file with name description.html
            try
            {
                if (file != null && file.ContentLength > 0)
                {

                    string localPath = "~/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/"+ProductUpload.IMAGE_FOR.Products.GetDescription()+"/" + pId;

                    var path = Path.Combine(this.server.MapPath(localPath), ProductUpload.DESC_FILE_NAME);
                    file.SaveAs(path);

                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
                throw new BusinessLogicLayer.MyException("[SaveDescriptionFile]", "Can't Save Description File !" + Environment.NewLine + ex.Message);
            }
        }
        /// <summary>
        /// Save File to local
        /// </summary>
        /// <param name="pId">Product Id</param>
        /// <param name="text">Text to save</param>
        /// <param name="lpath">path of folder (string)</param>
        /// <returns>true or false</returns>
        public bool SaveDescriptionFile(long pId, string text, string lpath)
        {
            //Crate file on given path 
            //Save text in file 
            try
            {
                string filePath = Path.Combine(lpath.ToString(), ProductUpload.DESC_FILE_NAME);

                File.Create(this.server.MapPath(filePath)).Close();

                //this code segment write data to file.
                System.IO.StreamWriter file = new System.IO.StreamWriter(this.server.MapPath(filePath));

                file.WriteLine(text);

                file.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new BusinessLogicLayer.MyException("[SaveDescriptionFile]", "Can't Save Description File !" + Environment.NewLine + ex.Message);
            }
        }

        public bool SaveCareerFile(string guid, string extension, string lpath)
        {
            try
            {
                string filePath = System.Web.HttpContext.Current.Server.MapPath(lpath.ToString());

                File.Create(filePath).Close();

                //this code segment write data to file.
               // System.IO.StreamWriter file = new System.IO.StreamWriter(filePath);

                //file.WriteLine(guid + extension);

                //file.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new BusinessLogicLayer.MyException("[SaveDescriptionFile]", "Can't Save Description File !" + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// This function identify that directory exists or not, if not exists create directory. 
        /// </summary>
        /// <param name="path">Path of directory</param>
        /// <returns>DirectoryInfo of specified path</returns>
        public DirectoryInfo CreateDirectory(string path)
        {
            // check directory exist or not
            //if not then create directory
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
        /// this function load the file 
        /// </summary>
        /// <param name="pId">Product Id</param>
        /// <param name="pImgtype">File type i.e Approved or NonApproved</param>
        /// <returns>path of file</returns>
        public string LoadProductDescFile(long pId, ProductUpload.IMAGE_TYPE pImgtype)
        {
            // create path for file
            // get the file from set path from ftp
            string lImgPath = string.Empty;
            BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
            StringBuilder lpath = null;
            try
            {
                lpath = new StringBuilder("/" + ProductUpload.IMAGE_ROOTPATH + "/" + pImgtype.GetDescription() + "/" + ProductUpload.IMAGE_FOR.Products.GetDescription() + "/" + pId.ToString());

                lImgPath = GetProductDescFile(imgPath, lpath);

                return lImgPath;
            }
            catch (Exception ex)
            {
                lImgPath = string.Empty;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DescriptionFileUpload][LoadDescFile]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return lImgPath;
        }

        /// <summary>
        /// this function load the file from server
        /// </summary>
        /// <param name="imgPath">ReadConfig Class Object</param>
        /// <param name="lpath">path of file</param>
        /// <returns>full path of file</returns>
        private string GetProductDescFile(ReadConfig imgPath, System.Text.StringBuilder lpath)
        {            
            StringBuilder lImgPath = new StringBuilder();
            try
            {
                string filename = ProductUpload.DESC_FILE_NAME;
                if (IsFileExists(imgPath, lpath.ToString(), filename))
                {
                    DownloadDescFile(imgPath, lpath.ToString(), filename);

                    if (File.Exists(Path.Combine(System.Web.HttpContext.Current.Server.MapPath(lpath.ToString()),filename)))
                    {
                        //this code segment read data from the file.
                        FileStream fs2 = new FileStream(Path.Combine(System.Web.HttpContext.Current.Server.MapPath(lpath.ToString()),filename), FileMode.OpenOrCreate, FileAccess.Read);
                        StreamReader reader = new StreamReader(fs2);

                        lImgPath.Append(reader.ReadToEnd());
                        reader.Close();

                        File.Delete(Path.Combine(System.Web.HttpContext.Current.Server.MapPath(lpath.ToString()), filename));
                    }
                }
            }
            catch (Exception ex)
            {
                lImgPath = null;
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DescriptionFileUpload][GetProductDescFile]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return lImgPath.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imgPath"></param>
        /// <param name="lpath"></param>
        /// <returns></returns>
        private bool IsFileExists(BusinessLogicLayer.ReadConfig imgPath, string lpath, string filename)
        {
            // check if file exist on ftp or not
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath + "/" + filename));
                reqFTP.UseBinary = true;
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                try
                {

                    reqFTP.GetResponse();
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
        /// Download file from server to local
        /// </summary>
        /// <param name="imgPath">ReadConfig Class Object</param>
        /// <param name="lpath">path of file</param>
        /// <param name="filename">Filename</param>
        public void DownloadDescFile(ReadConfig imgPath, string lpath, string filename)
        {
            //Download the file from ftp to local server
            FtpWebRequest reqFTP;
            try
            {
                ImageUpload imgu = new ImageUpload(System.Web.HttpContext.Current.Server);
                DirectoryInfo lDrInfo = imgu.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(lpath.Trim()));
                //filePath = <<The full path where the file is to be created.>>, 
                //fileName = <<Name of the file to be created(Need not be the name of the file on FTP server).>>

                FileStream outputStream = new FileStream(System.Web.HttpContext.Current.Server.MapPath(lpath.Trim()) + "\\" + filename, FileMode.Create);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(imgPath.IMAGE_FTP + lpath.ToString() + "/" + filename));
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
                   + "[DescriptionFileUpload][DownloadDescFile]",
                   BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                throw new BusinessLogicLayer.MyException("[DownloadDescFile]", "Probem in Downloading Description file !!" + Environment.NewLine + ex.Message);
            }
        }
    }
}
