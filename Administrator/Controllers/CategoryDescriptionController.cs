using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using Administrator.Models;
using System.Net;
namespace Administrator.Controllers
{
    /// <summary>
    /// Developed By :- Pradnyakar Badge
    /// Purpose:- To Create Discription text file of category
    /// </summary>
    public class CategoryDescriptionController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();

        //
        // GET: /CategoryDescription/
        [CustomAuthorize(Roles = "CategoryDescription/CanRead")]
        public ActionResult Index()
        {

            try
            {
                this.FillViewBags();


            }
            catch (Exception ex)
            {
                ViewBag.Error = "File Not Found";
            }
            return View();
        }

        [CustomAuthorize(Roles = "CategoryDescription/CanWrite")]
        [HttpPost]

        [ValidateInput(false)]

        public ActionResult Index(string content, long? FirstLevelCategory, long? SecondLevelCategory, long? ThirdLevelCategory)
        {
            try
            {

                this.FillViewBags();

                if (FirstLevelCategory == null && SecondLevelCategory == null && ThirdLevelCategory == null)
                {
                    ViewBag.Error = "Select Atlest One of Any three Category Level";
                    return View(content);
                }

                string lCategoryName = string.Empty;
                int level = 0;

                if (ThirdLevelCategory > 0)
                {
                    lCategoryName = db.Categories.Where(x => x.ID == ThirdLevelCategory).FirstOrDefault().Name;
                    level = 3;
                }
                else if (SecondLevelCategory > 0)
                {
                    lCategoryName = db.Categories.Where(x => x.ID == SecondLevelCategory).FirstOrDefault().Name;
                    level = 2;
                }
                else if (FirstLevelCategory > 0)
                {
                    lCategoryName = db.Categories.Where(x => x.ID == FirstLevelCategory).FirstOrDefault().Name;
                    level = 1;
                }


                BusinessLogicLayer.ReadConfig readConfig = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(new Uri(getConnection(ConnectionType.FTP) + "/Level" + level + "/" + lCategoryName + ".txt"));
                request.Method = System.Net.WebRequestMethods.Ftp.UploadFile;

                //StreamReader sourceStream = new StreamReader(fileName);
                byte[] fileContents = Encoding.UTF8.GetBytes(content);
                //sourceStream.Close();
                request.ContentLength = fileContents.Length;

                // This example assumes the FTP site uses anonymous logon.
                //request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");
                request.Credentials = new NetworkCredential(readConfig.USER_NAME.ToString(), readConfig.PASSWORD.ToString());
                Stream requestStream = request.GetRequestStream();

                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();
                System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)request.GetResponse();
                //Console.WriteLine("Upload status: {0}", response.StatusDescription);

                response.Close();

                ViewBag.Content = content;

                ViewBag.Error = "SEO Description Content Inserted/Updated Successfully";

            }

            //catch (Exception ex)
            //{
            //    ViewBag.Error = "Enable to Update File - " + ex.InnerException.ToString();
            //}
            catch (WebException ex)
            {
                ViewBag.Error = "Enable to Update File - " + ((FtpWebResponse)ex.Response).StatusDescription; 
            }
            return View();
        }

        //to retrive category list in respect of parent category id and level
        public JsonResult getCategories(Int64 catID, int level)
        {
            List<CatList> lCategory = new List<CatList>();
            lCategory = (from n in db.Categories
                         where n.ParentCategoryID == catID && n.Level == level && n.IsActive == true
                         select new CatList
                         {
                             id = n.ID,
                             name = n.Name
                         }
                             ).OrderBy(x => x.name).ToList();


            return Json(lCategory, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// to get data from description file if exists
        /// </summary>
        /// <param name="catID">categoryID</param>
        /// <param name="level">category level</param>
        /// <returns>text data in json format</returns>
        [CustomAuthorize(Roles = "CategoryDescription/CanWrite")]
        public JsonResult getDescription(int catID, int level)
        {
            string Content = string.Empty;

            try
            {
                string lCategoryName = string.Empty;
                lCategoryName = db.Categories.Where(x => x.ID == catID).FirstOrDefault().Name;

                //System.Net.HttpWebRequest webReq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(new Uri(@"http://192.168.1.102:8282/cat_desc/Level1/" + lCategoryName + ".txt"));

                //Create Web Request object
                System.Net.HttpWebRequest webReq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(new Uri(getConnection(ConnectionType.HTTP) + "/Level" + level + "/" + lCategoryName + ".txt"));

                if (webReq.GetResponse().ContentLength > 0)
                {
                    //to read web request text stream by stream in bit
                    System.IO.StreamReader sr = new System.IO.StreamReader(webReq.GetResponse().GetResponseStream());
                    Content = sr.ReadToEnd();
                }
                else
                {
                    ViewBag.Error = "File Not Found";
                }

                return Json(Content, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Content, JsonRequestBehavior.AllowGet);
            }
        }

        //to get ftp connection from web config
        private string getConnection(ConnectionType con)
        {
            BusinessLogicLayer.ReadConfig readConfig = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
            if (con == ConnectionType.FTP)

                return readConfig.FILE_FTP;

            return readConfig.FILE_HTTP;

        }

        public enum ConnectionType
        {
            FTP = 0,
            HTTP = 1

        }

        public class CatList
        {
            public Int64 id { get; set; }
            public string name { get; set; }
        }

        //All required viewbag for dropdown on view
        public void FillViewBags()
        {
            List<CatList> lCategory = new List<CatList>();
            lCategory = (from n in db.Categories
                         where n.Level == 1 && n.IsActive == true
                         select new CatList
                         {
                             id = n.ID,
                             name = n.Name
                         }
                             ).OrderBy(x => x.name).ToList();

            ViewBag.FirstLevelCategory = new SelectList(lCategory, "id", "name");

            List<CatList> lCategoryTwo = new List<CatList>();
            lCategoryTwo.Add(new CatList { id = 0, name = "Select Category" });

            ViewBag.SecondLevelCategory = new SelectList(lCategoryTwo, "id", "name");
            ViewBag.ThirdLevelCategory = new SelectList(lCategoryTwo, "id", "name");


        }
    }
    
}