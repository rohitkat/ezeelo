using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using Gandhibagh.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using ModelLayer.Models;

namespace Gandhibagh.Controllers
{
    public class SiteMapMakerController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /SiteMapMaker/

        #region CREATE SITEMAP
        public ActionResult Create()
        {
            try
            {
                List<string> IDs = new List<string>();
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["SITEMAP_ID"]))
                {
                    IDs = System.Configuration.ConfigurationManager.AppSettings["SITEMAP_ID"].Trim().ToString().Split(',').ToList();
                }
                if (Session["UID"] != null && IDs.Contains(Session["UID"].ToString()))
                {

                    SelectList Cities = new SelectList((from f in db.Franchises
                                                        where f.ID != 1 && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                                                        select new { ID = f.BusinessDetail.Pincode.CityID, Name = f.BusinessDetail.Pincode.City.Name }).Distinct().ToList().OrderBy(x => x.Name), "ID", "Name");


                    ViewBag.City = Cities;

                    return View();
                }
                else
                {
                    return View("HttpError");
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SiteMapMaker][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return View("HttpError");
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SiteMapMaker][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return View("HttpError");
            }
        }

        [HttpPost]
        public ActionResult Create(long? City, int? sitemapType, int SelectArea)////added int SelectArea as FranchiseID
        {
            try
            {
                SelectList Cities = new SelectList((from f in db.Franchises
                                                    where f.ID != 1 && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                                                    select new { ID = f.BusinessDetail.Pincode.CityID, Name = f.BusinessDetail.Pincode.City.Name }).Distinct().ToList().OrderBy(x => x.Name), "ID", "Name", City);

                ViewBag.City = Cities;
                string CityName = string.Empty;
                int FranchiseID = SelectArea;////added
                if (City == null && sitemapType == 4)
                {
                    ;
                }
                else if (City == null || sitemapType == null)
                {
                    ViewBag.Message = "Error in Generating SiteMap..";
                    return View();
                }
                else
                {
                    CityName = db.Cities.Find(City).Name;
                }

                if (sitemapType > 0)
                {
                    switch (sitemapType)
                    {
                        //Category
                        case 1: List<SiteMapViewModel> lCategories = GetAllCategoryList((long)City,(int)FranchiseID, CityName); ////added (long)City->(int)FranchiseID chnage by Vaibhav
                            GenerateXml(lCategories.Select(x => x.URL).ToList(), CityName, FranchiseID, "categories.xml");////added FranchiseID
                            break;

                        //Products
                        case 2: List<string> lProducts = GetAllProductList((int)FranchiseID, CityName);////added (long)City->(int)FranchiseID
                            GenerateXml(lProducts.ToList(), CityName, FranchiseID, "products.xml");////added FranchiseID
                            break;
                        //Shop
                        case 3: List<string> lShops = GetAllShopList((int)FranchiseID, CityName);////added (long)City->(int)FranchiseID
                            GenerateXml(lShops.ToList(), CityName, FranchiseID, "shops.xml");////added FranchiseID
                            break;
                        //Image
                        case 4: List<string> lImages = GetImages();
                            GenerateXml(lImages.ToList(), string.Empty, FranchiseID, "images.xml");//doubt
                            break;
                    }
                }
                ViewBag.Message = "SiteMap Generated Successfully..";
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[SiteMapMaker][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return View("HttpError");
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[SiteMapMaker][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return View("HttpError");
            }
        }

        #endregion

        #region METHOD For Products
        private List<string> GetAllProductList(int FranchiseID, string CityName)////added long CityID->int FranchiseID
        {

            List<SiteMapViewModel> lProducts = new List<SiteMapViewModel>();
            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
            BusinessLogicLayer.SitemapObjectList obj = new BusinessLogicLayer.SitemapObjectList();
            DataTable dt = new DataTable();
            dt = obj.Select_FranchiseProductList(FranchiseID, System.Web.HttpContext.Current.Server);////added CityID->FranchiseID
            HomeController objHome = new HomeController();
            /*Select All Menu By Franchise */
            lProducts = (from n in dt.AsEnumerable()
                         select new SiteMapViewModel
                         {
                             ID = n.Field<Int64>("ID"),
                             //RouteName = Regex.Replace(n.Field<string>("ProductName").Substring(0, Math.Min(n.Field<string>("ProductName").Length, 30)), @"[\/\\#,+()$~%.':*?<>{} ]", "-").Replace("&", "and").ToLower(),
                             RouteName = objHome.GetURLStructureName(n.Field<string>("ProductName")).ToLower(), //Added by Zubair for sitemap new URL Structure on 24-07-2017
                             Level = 0
                         }).OrderBy(x => x.Level).ToList();

            lProducts = GetProductUrls(lProducts, CityName, FranchiseID);////added CityID->FranchiseID

            return lProducts.Select(x => x.URL).ToList();
        }

        private List<SiteMapViewModel> GetProductUrls(List<SiteMapViewModel> lProducts, string City, int FranchiseID)////added int FranchiseID
        {
            /* For Products*/
            //http://www.ezeelo.com/nagpur/mango-laalbagh/63429/preview
            lProducts.ForEach(x => x.URL = "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + City.Trim().ToLower() + "/" + FranchiseID + "/" + x.RouteName + "/" + x.ID + "/preview");////added  + FranchiseID + "/"
            return lProducts;
        }

        #endregion

        #region METHOD For Categories
        private List<SiteMapViewModel> GetAllCategoryList(long CityID,int FranchiseID, string CityName)////added int FranchiseID
        {
            List<SiteMapViewModel> lCategories = new List<SiteMapViewModel>();
            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
            BusinessLogicLayer.FranchiseMenuList obj = new BusinessLogicLayer.FranchiseMenuList();
            DataTable dt = new DataTable();
            dt = obj.Select_FranchiseMenu(CityID,FranchiseID, System.Web.HttpContext.Current.Server);////added FranchiseID

            /*Select All Menu By Franchise */
            lCategories = (from n in dt.AsEnumerable()
                           select new SiteMapViewModel
                           {
                               ID = n.Field<Int32>("ID"),
                               RouteName = Regex.Replace(n.Field<string>("CategoryName").Substring(0, Math.Min(n.Field<string>("CategoryName").Length, 30)), @"[\/\\#,+()$~%.':*?<>{} ]", "-").Replace("&", "and").ToLower(),
                               Level = n.Field<int>("Level"),
                               IsManaged = n.Field<bool>("IsManaged"),
                               ParentCategoryID = n.Field<object>("ParentCategoryID") == null ? -1 : n.Field<int>("ParentCategoryID") 
                           //}).Where(x=>x.IsManaged==true).OrderBy(x => x.Level).ToList();//added by harshada where(x=>IsManaged==true) on 20/1/2017 for only active category 
                            }).OrderBy(x => x.Level).ToList();//added by harshada where(x=>IsManaged==true) on 20/1/2017 for only active category 

            lCategories = GetCategoryUrls(lCategories, CityName, FranchiseID);////added CityID->FranchiseID

            return lCategories;
        }

        private List<SiteMapViewModel> GetCategoryUrls(List<SiteMapViewModel> lCategories, string City, int FranchiseID)////added int FranchiseID
        {
            //- Changes made by Avi Verma.
            //- Date : 04-May-2017.
            //- Reason : As required by Bhushan , He needs to list of all levels categories in XML.
            //- Previously, As required by SEO Team, they only needs, 1st level and 3rd level only.
            //- So, I changed the where clause and return directly.
            //- In case , if he wants the old version of code. then just comments my code.

            /* For Level-1 Category*/
            //http://www.ezeelo.com/nagpur/sweet-and-confectionery/1450/category
            List<SiteMapViewModel> lLevelOneCategory = lCategories.Where(x => x.Level == 1).ToList();
            lLevelOneCategory.ForEach(x => x.URL = "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + City.Trim().ToLower() + "/" + FranchiseID + "/" + x.RouteName + "/" + x.ID + "/category");////added  + FranchiseID + "/"

            /* For Level-2 Category*/
            //http://www.ezeelo.com/nagpur/sweet-and-confectionery/1450/category
            List<SiteMapViewModel> lLevelTwoCategory = lCategories.Where(x => x.Level == 2).ToList();
            lLevelTwoCategory.ForEach(x => x.URL = "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + City.Trim().ToLower() + "/" + FranchiseID + "/" +
                                      lCategories.First(z => z.ID == x.ParentCategoryID).RouteName + "/" + x.RouteName + "/" + 
                                      x.ParentCategoryID + "/" + x.ID + "/category");////added  + FranchiseID + "/"
            
            /* For Level-3 Category*/            
            List<SiteMapViewModel> lLevelThreeCategory = lCategories.Where(x => x.Level == 3).ToList();
            lLevelThreeCategory.ForEach(x => x.URL = "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + City.Trim().ToLower() + "/" + FranchiseID + "/" + x.RouteName + "/" + x.ID + "/cat-products");////added  + FranchiseID + "/"

            List<SiteMapViewModel> lCategoriesWithUrl = new List<SiteMapViewModel>();
            lCategoriesWithUrl.AddRange(lLevelOneCategory);
            lCategoriesWithUrl.AddRange(lLevelTwoCategory);
            lCategoriesWithUrl.AddRange(lLevelThreeCategory);
            return lCategoriesWithUrl;
            
            //- Ends new Changes By Avi verma. Date : 04-May-2017.



            /* For Level-1 Category*/
            //http://www.ezeelo.com/nagpur/sweet-and-confectionery/1450/category
            //List<SiteMapViewModel> lLevelOneCategory = lCategories.Where(x => x.Level == 1).ToList();
            //lLevelOneCategory.ForEach(x => x.URL = "http://www.ezeelo.com/" + City.Trim().ToLower() + "/" + FranchiseID + "/" + x.RouteName + "/" + x.ID + "/category");////added  + FranchiseID + "/"

            /* For Level-3 Category*/
            //For MMP : http://www.ezeelo.com/nagpur/all-fruits/1281/3/4968/mmp
            //List<SiteMapViewModel> lLevelThreeCategoryMMP = lCategories.Where(x => x.Level == 3 && x.IsManaged == true).ToList();
            //lLevelThreeCategoryMMP.ForEach(x => x.URL = "http://www.ezeelo.com/" + City.Trim().ToLower() + "/" + x.RouteName + "/" + x.ID + "/3/" + CityID + "/mmp");

            //For OMP : http://www.ezeelo.com/nagpur/proteins/434/cat-products             
            //List<SiteMapViewModel> lLevelThreeCategoryOMP = lCategories.Where(x => x.Level == 3 && x.IsManaged == false).ToList();
            //lLevelThreeCategoryOMP.ForEach(x => x.URL = "http://www.ezeelo.com/" + City.Trim().ToLower() + "/" + FranchiseID + "/" + x.RouteName + "/" + x.ID + "/cat-products");////added  + FranchiseID + "/"

            //List<SiteMapViewModel> lCategoriesWithUrl = new List<SiteMapViewModel>();
            //lCategoriesWithUrl.AddRange(lLevelOneCategory);
            ////lCategoriesWithUrl.AddRange(lLevelThreeCategoryMMP);
            //lCategoriesWithUrl.AddRange(lLevelThreeCategoryOMP);

            //return lCategoriesWithUrl;
        }

        #endregion

        #region METHOD For Shops
        private List<string> GetAllShopList(int FranchiseID, string CityName)////added long CityID->int FranchiseID
        {
            List<SiteMapViewModel> lShops = new List<SiteMapViewModel>();
            BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
            BusinessLogicLayer.SitemapObjectList obj = new BusinessLogicLayer.SitemapObjectList();
            DataTable dt = new DataTable();
            dt = obj.Select_FranchiseShopList(FranchiseID, System.Web.HttpContext.Current.Server);////added CityID->FranchiseID

            /*Select All Menu By Franchise */
            lShops = (from n in dt.AsEnumerable()
                      select new SiteMapViewModel
                      {
                          ID = n.Field<Int64>("ID"),
                          RouteName = Regex.Replace(n.Field<string>("ShopName").Substring(0, Math.Min(n.Field<string>("ShopName").Length, 30)), @"[\/\\#,+()$~%.':*?<>{} ]", "-").Replace("&", "and").ToLower(),
                          Level = 0
                      }).OrderBy(x => x.Level).ToList();

            lShops = GetShopUrls(lShops, CityName, FranchiseID);////added CityID->FranchiseID

            return lShops.Select(x => x.URL).ToList();
        }

        private List<SiteMapViewModel> GetShopUrls(List<SiteMapViewModel> lShops, string City, int FranchiseID)////added int FranchiseID
        {
            /* For Shops*/
            //http://www.ezeelo.com/nagpur/ajit-bakery/10500/shop-products
            lShops.ForEach(x => x.URL = "" + (new URLsFromConfig()).GetURL("CUSTOMER") + "" + City.Trim().ToLower() + "/" + FranchiseID + "/" + x.RouteName + "/" + x.ID + "/shop-products");////added  + FranchiseID + "/"
            return lShops;
        }

        #endregion

        #region METHOD For Images
        private List<string> GetImages()
        {
            List<string> lImgIndex = new List<string>();
            List<string> lDirList = new List<string>();
            List<string> lFileList = new List<string>();
            try
            {
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                string lPath = imgPath.IMAGE_FTP;
                lFileList = GetFiles(lPath, lFileList);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetImages]", "Probem in Getting images!!" + Environment.NewLine + ex.Message);
            }
            return lFileList;
        }

        private List<string> GetFiles(string pDirName, List<string> lFileList)
        {
            try
            {
                List<string> lDirList = new List<string>();
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                string httpPath = imgPath.IMAGE_HTTP.ToString().Trim().EndsWith("/") ? imgPath.IMAGE_HTTP.ToString().Trim().Remove(imgPath.IMAGE_HTTP.ToString().Trim().Length - 1, 1) : imgPath.IMAGE_HTTP.ToString().Trim();
                string lPath = pDirName.Replace(imgPath.IMAGE_FTP, httpPath);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(pDirName));
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                lDirList = (reader.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)).ToList();

                if (lDirList.Count > 0)
                {
                    foreach (string item in lDirList)
                    {
                        if (item.Trim().ToLower().Contains("dir"))
                        {
                            lFileList = GetFiles(pDirName + "/" + item.ToLower().Split(new string[] { "<dir>" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim(), lFileList);
                        }
                        else if (item.Trim().ToLower().Contains(".jpg") || item.Trim().ToLower().Contains(".jpeg") || item.Trim().ToLower().Contains(".png"))
                        {
                            lFileList.Add(lPath + "/" + item.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[3]);
                        }
                    }
                    responseStream.Close();
                    response.Close();
                }
                return lFileList;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetFiles]", "Probem in Reading Files!!" + Environment.NewLine + ex.Message);
            }
        }

        #endregion

        #region METHOD For Xml
        private void GenerateXml(List<string> Url, string CityName, int FranchiseID, string fileName)////added int FranchiseID  //doubt
        {
            
            //- Changes made by Avi Verma.
            //- Date : 05-July-2016. Reason : Changes made by SEO Team (Bhushan). Reference : GB-CR-56.
            string lPath = "";
            //lPath =  Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/sitemap"), CityName.Trim().ToLower());
            lPath = System.Web.HttpContext.Current.Server.MapPath("~/sitemap") + "\\" + CityName.Trim().ToLower() + "-" + FranchiseID + "-" + fileName;  ////added "\\" + FranchiseID +  //added by harshada path changed from CityName.Trim().ToLower() + "/" + FranchiseID + "-" + fileName

            //- Changes made by Avi Verma.
            //- Date : 05-July-2016. Reason : Changes made by SEO Team (Bhushan). Reference : GB-CR-56.
            //XmlTextWriter writer = new XmlTextWriter(Path.Combine(lPath, fileName), System.Text.Encoding.UTF8);
            XmlTextWriter writer = new XmlTextWriter(lPath, System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
           // writer.Indentation = 2;
            writer.WriteStartElement("urlset");
            writer.WriteAttributeString("xmlns", "", null, "http://www.sitemaps.org/schemas/sitemap/0.9");
            writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xmlns", "schemaLocation", null, "http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");
            foreach (string item in Url)
            {
                createNode(item, writer);
            }

            writer.WriteEndElement();
            
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
            //  MessageBox.Show("XML File created ! ");
        }

        private void createNode(string pUrl, XmlTextWriter writer)
        {
            writer.WriteStartElement("url");
            writer.WriteStartElement("loc");
            writer.WriteString(pUrl);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        #endregion

    }
}