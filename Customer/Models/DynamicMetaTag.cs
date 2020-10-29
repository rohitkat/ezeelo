using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Globalization;
namespace Gandhibagh.Models
{
    public class DynamicMetaTag : ActionFilterAttribute
    {
        /// <summary>
        /// To Generate SEO Meta Data 
        /// This will be generate meta tag for 
        /// Category Page
        /// ProductPreview Page
        /// Shop Page etc.....
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            StringBuilder sb = new StringBuilder();
            string cName = string.Empty;

            EzeeloDBContext db = new EzeeloDBContext();
            //check city is selected or not i.e. in cookies
            if (HttpContext.Current.Request.Cookies["CityCookie"] != null && HttpContext.Current.Request.Cookies["CityCookie"].Value != string.Empty)
            {
                cName = HttpContext.Current.Request.Cookies["CityCookie"].Value.Split('$')[1];
                cName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cName.ToLower());
            }

            //For category page            
            Int64 categoryID = 0;
            if (filterContext.ActionParameters.ContainsKey("CatID") && filterContext.ActionParameters["CatID"] != null)
            {    
                //***Added by harshada on 20/1/2017 to chk condition for 2nd level category matatag ***//
                if (filterContext.ActionParameters.ContainsKey("secLvlCatID") && filterContext.ActionParameters["secLvlCatID"] != null)
                {
                    categoryID = Convert.ToInt64(filterContext.ActionParameters["secLvlCatID"].ToString());
                }
                else
                {
                    categoryID = Convert.ToInt64(filterContext.ActionParameters["CatID"].ToString()); //doubt CatID
                }
                
            }
            else if (filterContext.ActionParameters.ContainsKey("ParentCatID") && filterContext.ActionParameters["ParentCatID"] != null)
            {
                categoryID = Convert.ToInt64(filterContext.ActionParameters["ParentCatID"].ToString());
            }
            else if (filterContext.ActionParameters.ContainsKey("parentCategoryId"))
            {
                if (filterContext.ActionParameters["parentCategoryId"] != null)
                {
                    categoryID = Convert.ToInt64(filterContext.ActionParameters["parentCategoryId"].ToString());
                }
                    // For Shop page Meta tag
                else if (filterContext.ActionParameters["shopID"] != null)
                {
                    Int64 shopID = Convert.ToInt64(filterContext.ActionParameters["shopID"].ToString());

                    if (shopID > 0)
                    {
                        string name = "description";
                        string shopName = db.Shops.Find(shopID).Name;
                        sb.Append(" <title>" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(shopName.ToLower()) + " | eZeelo - " + cName + " </title>" + Environment.NewLine);
                        //sb.Append(" <meta name='description' content='eZeelo - India's biggest online shopping portal for  " + cName + ". We offer best deals from reputed local shops & providers like " + shopName + ".' >" + Environment.NewLine);
                        sb.Append(" <meta name=\"description\" content=\"eZeelo - India's biggest online shopping portal for  " + cName + ". We offer best deals from reputed local shops & providers like " + shopName + ".\" >" + Environment.NewLine);
                    }
                }
            }
            //Category Meta Tag
            if (categoryID > 0)
            {
                SEO ldata = new SEO();
                ldata = db.SEOs.Where(x => x.EntityID == categoryID && x.BusinessType.Prefix == "CTHD").FirstOrDefault();

                if (db.SEOs.Where(x => x.EntityID == categoryID && x.BusinessType.Prefix == "CTHD").Count() > 0)
                {
                    sb.Append(" <meta name=\"keywords\" content=\"" + ldata.MetaKeyword + "\" >" + Environment.NewLine);
                    sb.Append(" <meta name=\"description\" content=\"" + ldata.Description + "\" >" + Environment.NewLine);

                    if (ldata.Metatag == null || ldata.Metatag.Equals(string.Empty))
                    { }
                    else
                    {
                         
                        
                        // sb.Append(" <title>" + ldata.Metatag + "</title>" + Environment.NewLine);
                        //Commented and Changes by Zubair on 27-07-2017 for SEO
                        //sb.Append(" <title>" + ldata.Metatag + " | eZeelo - " + cName + " </title>" + Environment.NewLine);
                        sb.Append(" <title>" + ldata.Metatag + " " + cName + " | eZeelo.com" + " </title>" + Environment.NewLine);
                    }
                }
                else
                {
                    string catName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(db.Categories.Find(categoryID).Name.ToLower());
                    sb.Append(" <meta name=\"keywords\" content=\"online Shopping, Online Store, beauty products online, buy fresh fruits, Vegetables, Groceries, Home Appliances, Sweets, supermarket, India,Kanpur \" >" + Environment.NewLine); //Yashaswi 01/12/2018 Default City Change remove other city name
                    //Commented and Changes by Zubair on 27-07-2017 for SEO
                    //sb.Append(" <title>" + catName + " Store Online | eZeelo - " + cName + " </title>" + Environment.NewLine);
                    //sb.Append(" <meta name= \"description\" content=\"Buy " + catName + " products from " + cName + "’s Biggest online store at best price. We provide wide range of " + catName + " products with best offers and discounts.\" >" + Environment.NewLine);
                    sb.Append(" <title>" + catName + " Store Online " + cName + " | eZeelo.com" + " </title>" + Environment.NewLine);
                    sb.Append(" <meta name= \"description\" content=\"Buy " + catName + " products from biggest online store at lowest price in " + cName + ". We provide wide range of " + catName + " with great discounts\" >" + Environment.NewLine);
                }
            }

            // For product-preview page
            if (filterContext.ActionParameters.ContainsKey("itemId"))
            {

                if (filterContext.ActionParameters["itemId"] != null)
                {
                    Int64 ItemID = Convert.ToInt64(filterContext.ActionParameters["itemId"].ToString());
                    if (ItemID > 0)
                    {
                        SEO ldata = new SEO();

                        ldata = db.SEOs.Where(x => x.EntityID == ItemID && x.BusinessType.Prefix == "MTDT").FirstOrDefault();
                        if (db.SEOs.Where(x => x.EntityID == ItemID && x.BusinessType.Prefix == "MTDT").Count() > 0)
                        {
                            sb.Append(" <meta name=\"keywords\" content=\"" + ldata.MetaKeyword + "\" >" + Environment.NewLine);
                            sb.Append(" <meta name=\"description\" content=\"" + ldata.Description + "\" >" + Environment.NewLine);
                            if (ldata.Metatag == null || ldata.Metatag.Equals(string.Empty))
                            { }
                            else
                            {
                                // sb.Append(" <title>" + ldata.Metatag + " </title>" + Environment.NewLine);
                                //Commented and Changes by Zubair on 27-07-2017 for SEO
                                //sb.Append(" <title>" + ldata.Metatag + " | eZeelo - " + cName + " </title>" + Environment.NewLine);
                                sb.Append(" <title>" + ldata.Metatag + " " + cName + " | eZeelo.com" + " </title>" + Environment.NewLine);
                            }
                        }
                        else
                        {
                            string prodName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(db.Products.Find(ItemID).Name.ToLower());
                            //Added for SEO URL Structure RULE by AShish
                            prodName = prodName.Replace("+", " ").Replace("/"," ");//added As per Bhushan Suggestion
                            sb.Append(" <meta name=\"keywords\" content=\"online Shopping, Online Store, beauty products online, buy fresh fruits, Vegetables, Groceries, Home Appliances, Sweets, supermarket, India,Lucknow, Kanpur\" >" + Environment.NewLine); //added by harshada on 03/02/2017 for avoiding repeated meta keyword tag
                            //Commented and Changes by Zubair on 27-07-2017 for SEO
                            //sb.Append(" <title>" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(prodName.ToLower()) + " Online | eZeelo - " + cName + " </title>" + Environment.NewLine);
                            //sb.Append(" <meta name=\"description\" content=\"Buy " + prodName + " from our online store at " + cName + " With best price. We provide offers and discounts on every single product.\" >" + Environment.NewLine);
                            sb.Append(" <title>" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(prodName.ToLower()) + " Online " + cName + " | eZeelo.com" + " </title>" + Environment.NewLine);
                            sb.Append(" <meta name=\"description\" content=\"Buy " + prodName + " from our online store at " + cName + " With best price. We provide best offers & discounts on all products\" >" + Environment.NewLine);
                        }
                    }
                }
            }

            filterContext.Controller.ViewData["MetaData"] = sb.ToString();

        }
    }
}