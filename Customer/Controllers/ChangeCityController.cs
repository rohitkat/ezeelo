//-----------------------------------------------------------------------
// <copyright file="ChangeCityController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogicLayer;
using Gandhibagh.Models;
using ModelLayer.Models;
//using Gandhibagh.Models;//addec

namespace Gandhibagh.Controllers
{
    public class ChangeCityController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();

        //
        // GET: /ChangeCity/
        //   [DynamicMetaTag] ////added
        //Yashaswi Add OldareaId Parameter 

        public ActionResult Index(int CityID, string CityName, int FranchiseId, string HelpLineNumber, long? OldAreaId)//--FranchiseId, HelpLineNumber Added for Multiple MCO by Ashish --//
        {
            /*============ On city change, clear shopping cart =======================*/
            /* int count = (from f in db.Franchises
                           where f.ID != 1 && f.IsActive == true && f.BusinessDetail.UserLogin.IsLocked == false && f.BusinessDetail.Pincode.City.IsActive == true
                           && f.BusinessDetail.Pincode.City.Name.ToLower().Trim() == CityName.ToLower().Trim()
                              select new {f.BusinessDetail.Pincode.City.Name}).ToList().Count();*/

            //-- Added for Multiple MCO by Ashish --//
            int count1 = 0;
            if (FranchiseId == 1) //Franchise Id 1 means redirecting to city for Merchant only
            {
                ShoppingCartInitialization lShoppingCartInitialization = new ShoppingCartInitialization();
                lShoppingCartInitialization.DeleteShoppingCartCookie();
            }
            else
            {
                count1 = (from f in db.Franchises
                          join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                          join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                          join pc in db.Pincodes on bd.PincodeID equals pc.ID
                          join c in db.Cities on pc.CityID equals c.ID
                          join hd in db.HelpDeskDetails on f.ID equals hd.FranchiseID into LOJ
                          from hd in LOJ.DefaultIfEmpty()//-- For Left Outer Join --//
                          where f.ID != 1 && f.IsActive == true && c.Name.ToLower().Trim() == CityName.ToLower().Trim()
                                && f.ID == FranchiseId && ul.IsLocked == false && c.IsActive == true
                          select new { c.Name }
                       ).ToList().Count();
            }
           
           


            //Yashaswi Check Franchise
            FranchiseLocation objFL = db.FranchiseLocations.FirstOrDefault(f => f.AreaID == OldAreaId);
            long OldFranchiseId = 0;
            if (objFL != null)
            {
                OldFranchiseId = (long)(objFL.FranchiseID ?? 0);
            }
            //End Yashaswi

            if (count1 > 0)
            {
                //Yashaswi check Franchise
                if (OldFranchiseId != FranchiseId)
                {
                    ShoppingCartInitialization lShoppingCartInitialization = new ShoppingCartInitialization();
                    lShoppingCartInitialization.DeleteShoppingCartCookie();
                }
            }

            HttpCookie CityCookie = new HttpCookie("CityCookie");
            
            //if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
            //{
            //    //Response.Cookies["CityCookie"].Expires = DateTime.Now.AddDays(-1);               
            //    //Delete whole cookie
            //    if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
            //    {
            //        CityCookie.Expires = DateTime.Now.AddDays(-1);
            //        ControllerContext.HttpContext.Response.Cookies.Add(CityCookie);
            //    }
            //    if (CityCookie.Expires < DateTime.Now)
            //    {
            //        ControllerContext.HttpContext.Request.Cookies.Remove("CityCookie");
            //    }
            //}
            string oldCityName = "";
            string oldCityNameurl = "/";////added
            if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && !string.IsNullOrEmpty(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value))
            {
                oldCityName = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                oldCityNameurl += ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();////added

            }

            string oldCityID = "";
            string oldCityIDurl = "/";////added
            if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && !string.IsNullOrEmpty(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value))
            {
                oldCityID = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0].Trim();
                oldCityIDurl += ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0].Trim();////added
            }
            //-- Added for Multiple MCO by Ashish --//
            string oldFranchiseId = "";
            string oldFranchiseIdurl = "/";////added
            if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && !string.IsNullOrEmpty(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value))
            {
                oldFranchiseId = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2].Trim();
                oldFranchiseIdurl += ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2].Trim();////added
            }
            string oldHelpLineNumber = "";
            if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && !string.IsNullOrEmpty(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value))
            {
                oldHelpLineNumber = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[3].Trim();
            }
            ////ControllerContext.HttpContext.Response.Cookies["CityCookie"].Value = CityID.ToString() + '$' + CityName;////hide old code
            ControllerContext.HttpContext.Response.Cookies["CityCookie"].Value = CityID.ToString() + '$' + CityName + '$' + FranchiseId.ToString() + '$' + HelpLineNumber;
            //-- End Added for Multiple MCO by Ashish --//

            ControllerContext.HttpContext.Response.Cookies.Add(ControllerContext.HttpContext.Response.Cookies["CityCookie"]);
            //ControllerContext.HttpContext.Response.Cookies["CityCookie"].Expires.AddDays(7);
            ControllerContext.HttpContext.Response.Cookies["CityCookie"].Expires = System.DateTime.Now.AddDays(7);

            string AbsolutePath = ControllerContext.HttpContext.Request.UrlReferrer.AbsolutePath.ToString();
            string[] count2 = AbsolutePath.Split('/');
            string NFranchiseId = "";
            string NCityID = "";
            string NCityNameurl = "";
            if (count2.Length > 3)
            {
                oldFranchiseIdurl += "/";
                NFranchiseId = "/" + FranchiseId.ToString() + "/";////added
                oldCityIDurl += "/";
                NCityID = "/" + CityID.ToString() + "/"; ////added
                oldCityNameurl += "/";
                NCityNameurl = "/" + CityName.ToLower() + "/"; ////added

            }
            else
            {
                NCityID = "/" + CityID.ToString();////added
                NFranchiseId = "/" + FranchiseId.ToString();////added
                NCityNameurl = "/" + CityName.ToLower();////added
            }
            string city = "";
            int FranchiseID = 0;////added by Ashish for multiple MCO in same city
            if (Request.Cookies["CityCookie"] != null && (Request.Cookies["CityCookie"].Value != null || Request.Cookies["CityCookie"].Value != string.Empty))
            {
                city = Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                FranchiseID = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added by Ashish for multiple MCO in same city
                Session["SelectedCity"] = city;
                Session["SelectedFranchiseId"] = FranchiseID;
            }
            //Url rewritng change by harshada on date 29/12/2015
            //return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            //Boolean flag = false;
            if (Session["UID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            //Yashaswi If url contain payment-process then replace it with cart
            
                if (string.IsNullOrEmpty(oldCityName))
                {

                    if (!ControllerContext.HttpContext.Request.UrlReferrer.ToString().Contains(oldCityID) || oldCityID.Length == 0)
                    {
                        return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString().Replace("payment-process", "cart"));
                    }
                    else
                    {
                        // return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString().Replace(oldCityID, CityID.ToString()));////hide
                        return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString().Replace(oldCityIDurl, NCityID).Replace(oldFranchiseIdurl, NFranchiseId).Replace(oldHelpLineNumber, HelpLineNumber.ToString()).Replace("payment-process", "cart"));// New added
                    }

                }
                else
                {
                    if (!ControllerContext.HttpContext.Request.UrlReferrer.ToString().Contains(oldCityID) || oldCityID.Length == 0)
                    {
                        //return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString().Replace(oldCityName, CityName.ToLower()));////hide
                        return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString().Replace(oldCityNameurl, NCityNameurl).Replace(oldFranchiseIdurl, NFranchiseId).Replace("payment-process", "cart"));////added
                    }
                    else
                    {
                        //return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString().Replace(oldCityName, CityName.ToLower()).Replace(oldCityID, CityID.ToString()));////hide
                        return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString().Replace(oldCityNameurl, NCityNameurl).Replace(oldCityIDurl, NCityID).Replace(oldFranchiseIdurl, NFranchiseId).Replace(oldHelpLineNumber, HelpLineNumber.ToString()).Replace("payment-process", "cart"));////added
                    }
                }

         

            //End Yashaswi
            //change by Harshada
            //return RedirectToAction("Index","Home");
        }
    }
}