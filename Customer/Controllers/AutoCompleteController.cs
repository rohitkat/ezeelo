//-----------------------------------------------------------------------
// <copyright file="AutoCompleteController" company="Ezeelo Consumer Services Pvt. Ltd.">
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
using ModelLayer.Models.ViewModel;
using ModelLayer.Models; 


namespace Gandhibagh.Controllers
{
    public class AutoCompleteController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        public JsonResult Index(string term, int searchBy)
        {
           string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
            string[] arr = cookieValue.Split('$');
            int cityId = Convert.ToInt32(arr[0]);
            int franchiseId = Convert.ToInt32(arr[2]);////added by Ashish for multiple franchise in same city.
            //int cityId = 4968;
            AutoSearch lSearch = new AutoSearch();

            //List<AutoSuggestViewModel> lSearchList = lSearch.GetSearchMetaData(term, (AutoSearch.SEARCHBY)searchBy, cityId).ToList();////hide
            List<AutoSuggestViewModel> lSearchList = lSearch.GetSearchMetaData(term, (AutoSearch.SEARCHBY)searchBy, cityId, franchiseId).ToList();////added params franchiseId for Multiple MCO
            AutoSuggestViewModel productItem = new AutoSuggestViewModel();

            List<AutoSuggestViewModel> lAutoSearchList = new List<AutoSuggestViewModel>();
            string lPreSepratorStyle ="<div class='breakDiv'><hr/><b><span style='Font-weight:500;'>";
            string lPostSepratorStyle = "</span></b></div>";
            string lImageUrl = string.Empty;
            string imgStyle = "style='height:60px;width:60px;'";
            string productImgStyle = "style='height:85px;width:100px;'";
           
            string startLine = string.Empty;

            
            if (searchBy == (int)AutoSearch.SEARCHBY.ALL)
            {

                string products = string.Empty; 
                foreach (var item in lSearchList.Where(x => x.Head == "Product"))
                {
                    if (lSearchList.Where(x => x.Head == "Product").First() == item)
                    {
                        products = "<ul>";
                        startLine = lPreSepratorStyle + " Top Products " + lPostSepratorStyle;
                        productItem.Seperator = startLine;
                    }

                    Int64 ProductID = Convert.ToInt64(item.ID);
                    var color =( from pv in db.ProductVarients 
                                 join c in db.Colors on pv.ColorID equals c.ID
                                 where pv.ProductID == ProductID

                                 select new 
                                 {
                                 name = c.Name
                                 
                                 }).FirstOrDefault();
                    //if (color != null && color.name != "N/A")
                    //    lImageUrl = ImageDisplay.LoadProductThumbnails(Convert.ToInt64(item.ID), color.name, string.Empty, ProductUpload.THUMB_TYPE.SD);
                    //else
                    //    lImageUrl = ImageDisplay.LoadProductThumbnails(Convert.ToInt64(item.ID), "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);

                    //Tejaswee (5-11-2015)
                    if (color != null && color.name != "N/A")
                        lImageUrl = ImageDisplay.SetProductThumbPath(Convert.ToInt64(item.ID), color.name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    else
                        lImageUrl = ImageDisplay.SetProductThumbPath(Convert.ToInt64(item.ID), "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                    //products += "<li id ='" + item.ID + "'  onclick='GotoPage(this);'><a><div><img src='" + "/Content/No_thumbnail.png" + "' alt='Image' " + productImgStyle + "/></br><div>" + (item.Name.Length > 17 ? item.Name.Substring(0, 12) + "..." : item.Name).ToString() + "</div></div></a></li>";
                   
                    //// string val = item.ID +"$"+ item.Name.ToLower().Substring(0, Math.Min(item.Name.Length, 30)); ///hide
                   //// products += "<li id ='" + val + "'  onclick='GotoPage(this);'><a><div><img src='" + lImageUrl.Trim() + "' alt='Image' " + productImgStyle + " onerror='this.src = getNoImgPath();' /></br><div>" + (item.Name.Length > 17 ? item.Name.Substring(0, 12) + "..." : item.Name).ToString() + "</div></div></a></li>";///hide
                   
                    //Added for SEO URL Structure RULE by AShish
                    string val = item.ID + "$" + GetURLStructureName(item.Name).ToLower();
                    products += "<li id ='" + val.Replace("+", " ") + "'  onclick='GotoPage(this);'><a><div><img src='" + lImageUrl.Trim() + "' alt='Image' " + productImgStyle + " onerror='this.src = getNoImgPath();' /></br><div>" +  item.Name.Replace("+", " ").ToString() + "</div></div></a></li>";
                   
                    if (lSearchList.Where(x => x.Head == "Product").Last() == item)                   
                    {
                        productItem.Products = products + "</ul>";
                        productItem.Head = "Product";
                        lAutoSearchList.Add(productItem);
                    }
                }

                
                foreach (var item in lSearchList.Where(x => x.Head == "Category"))
                {
                    if (lSearchList.Where(x => x.Head == "Category").First() == item)
                        startLine = "in ";
                    else
                        startLine = "in ";
                    item.Abbr = startLine;
                    lAutoSearchList.Add(item);

                    /*Changes For Foodbagh redirection*/
                    //long categoryID = Convert.ToInt64(item.ID);
                    //if (db.ShopPriorities.Where(x => x.CategoryID == categoryID).Count() > 0)
                    //{
                    //    item.IsManagedItem = true;
                    //    item.CategoryName = db.Categories.Where(x => x.ID == categoryID).FirstOrDefault().Name;
                    //    item.CategoryLevel = db.Categories.Where(x => x.ID == categoryID).FirstOrDefault().Level;

                    //}

                }
                foreach (var item in lSearchList.Where(x => x.Head == "Shop"))
                {
                    
                    if (lSearchList.Where(x => x.Head == "Shop").First() == item)
                 
                        startLine = lPreSepratorStyle + " Popular Shops " + lPostSepratorStyle;
                    else
                        startLine = string.Empty;

                    item.Seperator = startLine;
                    lImageUrl = ImageDisplay.LoadShopLogo(Convert.ToInt64(item.ID), ProductUpload.IMAGE_TYPE.Approved);
                    item.Abbr = "<img src='" + lImageUrl.Trim() + "' alt='Image' " + imgStyle + "  onerror='this.src = getNoImgPath();'/>";
                    lAutoSearchList.Add(item);


                   // var result = myList.GroupBy(test => test.id)
                   //.Select(grp => grp.First())
                   //.ToList();
                }
            }
            else if (searchBy == (int)AutoSearch.SEARCHBY.PRODUCT)
            {
                string products = string.Empty;
                foreach (var item in lSearchList.Where(x => x.Head == "Product"))
                {
                    if (lSearchList.Where(x => x.Head == "Product").First() == item)
                    {
                        products = "<ul>";
                        startLine = lPreSepratorStyle + " Top Products " + lPostSepratorStyle;
                        productItem.Seperator = startLine;
                    }
                    Int64 ProductID = Convert.ToInt64(item.ID);
                    var color = (from pv in db.ProductVarients
                                 join c in db.Colors on pv.ColorID equals c.ID
                                 where pv.ProductID == ProductID

                                 select new
                                 {
                                     name = c.Name

                                 }).FirstOrDefault();
                    //if (color != null && color.name != "N/A")
                    //    lImageUrl = ImageDisplay.LoadProductThumbnails(Convert.ToInt64(item.ID), color.name, string.Empty, ProductUpload.THUMB_TYPE.SD);
                    //else
                    //    lImageUrl = ImageDisplay.LoadProductThumbnails(Convert.ToInt64(item.ID), "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);


                    //Tejaswee (5-11-2015)
                    if (color != null && color.name != "N/A")
                        lImageUrl = ImageDisplay.SetProductThumbPath(Convert.ToInt64(item.ID), color.name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    else
                        lImageUrl = ImageDisplay.SetProductThumbPath(Convert.ToInt64(item.ID), "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    //products += "<li id ='" + item.ID + "'  onclick='GotoPage(this);'><a><div><img src='" + "/Content/No_thumbnail.png" + "' alt='Image' " + productImgStyle + "/></br><div>" + (item.Name.Length > 17 ? item.Name.Substring(0, 12) + "..." : item.Name).ToString() + "</div></div></a></li>";
                   // products += "<li id ='" + item.ID + "'  onclick='GotoPage(this);'><a><div><img src='" + lImageUrl.Trim() + "' alt='Image' " + productImgStyle + " onerror='this.src = getNoImgPath();'/></br><div>" + (item.Name.Length > 17 ? item.Name.Substring(0, 12) + "..." : item.Name).ToString() + "</div></div></a></li>";
                    //Added for SEO URL Structure RULE by AShish
                    products += "<li id ='" + item.ID + "'  onclick='GotoPage(this);'><a><div><img src='" + lImageUrl.Trim() + "' alt='Image' " + productImgStyle + " onerror='this.src = getNoImgPath();'/></br><div>" +  item.Name.Replace("+", " ").ToString() + "</div></div></a></li>";
                    if (lSearchList.Where(x => x.Head == "Product").Last() == item)
                    {
                        productItem.Products = products + "</ul>";
                        productItem.Head = "Product";
                        lAutoSearchList.Add(productItem);
                        /*Changes For Foodbagh redirection*/
                        long categoryID = Convert.ToInt64(item.ID);
                        if (db.ShopPriorities.Where(x => x.CategoryID == categoryID).Count() > 0)
                        {
                            item.IsManagedItem = true;
                            item.CategoryName = db.Categories.Where(x => x.ID == categoryID).FirstOrDefault().Name;
                            item.CategoryLevel = db.Categories.Where(x => x.ID == categoryID).FirstOrDefault().Level;

                        }
                    }
                }


               foreach (var item in lSearchList.Where(x=>x.Head =="Category"))
                {
                    if (lSearchList.Where(x => x.Head == "Category").First() == item)
                        startLine = "in ";
                    else
                        startLine = "in ";
                    item.Abbr = startLine;
                    lAutoSearchList.Add(item);
                 
                }
            }
            else
            {
                foreach (var item in lSearchList.Where(x => x.Head == "Shop"))
                {
                 
                    if (lSearchList.Where(x => x.Head == "Shop").First() == item)
                        startLine = lPreSepratorStyle + " Popular Shop " + lPostSepratorStyle ;
                    else
                        startLine = string.Empty;

                    item.Seperator = startLine;

                    string ShopLogoPath = ImageDisplay.LoadShopLogo(Convert.ToInt64(item.ID), ProductUpload.IMAGE_TYPE.Approved);
                    item.Abbr = "<img src='" + ShopLogoPath.Trim() + "' alt='Image' " + imgStyle + " onerror='this.src = getNoImgPath();'/>";

                    //lImageUrl = ImageDisplay.LoadShopLogo(Convert.ToInt64(item.ID), ProductUpload.IMAGE_TYPE.Approved);
                    //item.Abbr = "<img src='" + (string.IsNullOrEmpty(lImageUrl.Trim()) ? "/Content/no_image.png" : lImageUrl.Trim()).ToString() + "' alt='Image' " + imgStyle + "/>";
                    lAutoSearchList.Add(item);
                }
            
            }


            //return Json(lAutoSearchList.OrderBy(x=>x.Head) , JsonRequestBehavior.AllowGet); //commented by harshada 
            return Json(lAutoSearchList.GroupBy(test => test.Name).Select(grp => grp.First()).OrderBy(x => x.Head), JsonRequestBehavior.AllowGet);//added by harshada on 31/12/2016 for distinct shop name 
           
        }

        /// <summary>
        /// Added for ULR Structure RULE by AShish
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public string GetURLStructureName(string Name)
        {
            string str = Name;
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[\\\#$~%.':*?<>{} ]", " ").Replace("&", "and");
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+/g", " ");
            // string[] parts2 = Regex.Split(str, @"\+\/\-\,\(\)");
            ///////////////////
            string concat = "";
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[\/\+\-\,()]", "|");
            string[] strSplit = str.Split('|');
            for (int i = 0; i < strSplit.Length; i++)
            {
                if (concat.Length <= 30)
                {
                    concat = concat.Length == 0 ? strSplit[i].Trim() : concat + ' ' + strSplit[i].Trim();
                }
            }
            concat = System.Text.RegularExpressions.Regex.Replace(concat, @"\s+", " ");
            concat = System.Text.RegularExpressions.Regex.Replace(concat, @"[\/\\#,+()$~%.':*?<>{} ]", "-").Replace("&", "and");
            concat = concat.Trim(new[] { '-' });
            ////////////
            //string test = concat.Substring(0, 1);

            //if (test == "-")
            //{ concat = concat.Substring(1, concat.Length); }
            //var test2 = concat[concat.Length - 1];
            //if (test2 == '-')
            //{ concat = concat.Substring(0, concat.Length - 1); }
            ////////////
            return concat;
        } 
        public ActionResult Suggestion(string term, int searchBy)
        {
            string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
            string[] arr = cookieValue.Split('$');
            int cityId = Convert.ToInt32(arr[0]);
            int franchiseId = Convert.ToInt32(arr[2]);////added by Ashish for multiple franchise in same city.
            //int cityId = 4968;
            AutoSearch lSearch = new AutoSearch();
            ViewBag.term = term;
            //List<AutoSuggestViewModel> lSearchList = lSearch.GetSearchMetaData(term, (AutoSearch.SEARCHBY)searchBy, cityId).Where(x => x.Head.Equals("Category")).ToList();////hide
            List<AutoSuggestViewModel> lSearchList = lSearch.GetSearchMetaData(term, (AutoSearch.SEARCHBY)searchBy, cityId, franchiseId).Where(x => x.Head.Equals("Category")).ToList();////added params franchiseId for Multiple MCO
            return View("_Suggestion", lSearchList.OrderBy(x => x.Head));
        }      

        public ActionResult Test()
        {
            return PartialView("_SearchPartial");
        }
	}
}