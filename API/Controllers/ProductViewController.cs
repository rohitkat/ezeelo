//-----------------------------------------------------------------------
// <copyright file="ProductViewController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using System.Web.Http.Description;
using API.Models;
using System.IO;
using System.Data.Entity;

namespace API.Controllers
{
    public class ProductViewController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        /// <summary>
        /// Get Product Basic details like Brand, Category, Short description etc 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [ApiException]
        [HttpGet]
        public object ProductDetails(long ProductID, int FranchiseId, long? lCustLoginID)
        {
            object obj = new object();
            try
            {
                if (ProductID == null || ProductID == 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid productId.", data = string.Empty };
                }
                // long ShopId = db.Shops.Where()
                string clName = "default";
                ProductDetails p = new ProductDetails(System.Web.HttpContext.Current.Server);
                ProductDetailview detailview = new ProductDetailview();
                detailview.ProductbasicDetail = p.GetBasicDetails(ProductID);
                detailview.ProductVarient = p.GetStockVarients(ProductID, 0, lCustLoginID, FranchiseId);
                detailview.Images = ImageDisplay.GetStockImages(ProductID, string.IsNullOrEmpty(clName) || clName == "N/A" ? string.Empty : clName);
                detailview.ProductGeneralSpecification = p.GetGeneralDescription(ProductID, true);

                //if (!string.IsNullOrEmpty(detailview.ProductGeneralSpecification))
                //{
                //    using (WebClient client = new WebClient())
                //    {
                //        client.Headers.Add(HttpRequestHeader.UserAgent, "your useragent string");
                //        detailview.ProductGeneralSpecification = client.DownloadString(detailview.ProductGeneralSpecification);
                //    }

                //    //var httpWebRequest = (HttpWebRequest)WebRequest.Create(detailview.ProductGeneralSpecification);
                //    ////httpWebRequest.ContentType = "application/json";
                //    //httpWebRequest.Method = "GET";
                //    //var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //    //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //    //{
                //    //    var result = streamReader.ReadToEnd();
                //    //    detailview.ProductGeneralSpecification = result;
                //    //};
                //}
                if (detailview.ProductbasicDetail != null && detailview.ProductVarient != null)
                {
                    detailview.ProductbasicDetail.ProductName = detailview.ProductbasicDetail.ProductName.Replace("+", " ");
                    foreach (var item in detailview.ProductVarient)
                    {
                        item.ProductName = item.ProductName.Replace("+", " ");
                    }
                    if (lCustLoginID != null && lCustLoginID > 0)
                    {
                        RecentlyViewProduct IsAlreadyPresent = db.RecentlyViewProducts.Where(x => x.UserLoginId == lCustLoginID && x.FranchiseId == FranchiseId && x.ProductId == ProductID).FirstOrDefault();
                        if (IsAlreadyPresent == null)
                        {
                            var Count = db.RecentlyViewProducts.Where(x => x.UserLoginId == lCustLoginID && x.FranchiseId == FranchiseId).Count();
                            if (Count > 20)
                            {

                                RecentlyViewProduct objRecentlyViewProduct = db.RecentlyViewProducts.OrderByDescending(x => x.CreateDate).FirstOrDefault();
                                db.RecentlyViewProducts.Remove(objRecentlyViewProduct);
                                db.SaveChanges();

                                RecentlyViewProduct objProduct = new RecentlyViewProduct();
                                objProduct.FranchiseId = FranchiseId;
                                objProduct.ProductId = ProductID;
                                objProduct.UserLoginId = lCustLoginID.Value;
                                objProduct.CreateBy = lCustLoginID.Value;
                                objProduct.CreateDate = DateTime.Now;
                                objProduct.NetworkIP = CommonFunctions.GetClientIP();
                                objProduct.DeviceType = string.Empty;
                                objProduct.DeviceID = string.Empty;
                                db.RecentlyViewProducts.Add(objProduct);
                                db.SaveChanges();

                            }
                            else
                            {
                                RecentlyViewProduct objProduct = new RecentlyViewProduct();
                                objProduct.FranchiseId = FranchiseId;
                                objProduct.ProductId = ProductID;
                                objProduct.UserLoginId = lCustLoginID.Value;
                                objProduct.CreateBy = lCustLoginID.Value;
                                objProduct.CreateDate = DateTime.Now;
                                objProduct.NetworkIP = CommonFunctions.GetClientIP();
                                objProduct.DeviceType = string.Empty;
                                objProduct.DeviceID = string.Empty;
                                db.RecentlyViewProducts.Add(objProduct);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            IsAlreadyPresent.ModifyBy = lCustLoginID;
                            IsAlreadyPresent.ModifyDate = DateTime.Now;
                            IsAlreadyPresent.NetworkIP = CommonFunctions.GetClientIP();
                            IsAlreadyPresent.DeviceType = string.Empty;
                            IsAlreadyPresent.DeviceID = string.Empty;
                            db.Entry(IsAlreadyPresent).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                    }
                    obj = new { Success = 1, Message = "Product details are found.", data = detailview };
                }
                else
                {
                    obj = new { Success = 1, Message = "Product details not found.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
    public class ProductDetailview
    {
        public ProductViewBasicDetailsViewModels ProductbasicDetail { get; set; }
        public IEnumerable<ShopProductVarientViewModel> ProductVarient { get; set; }
        public List<ImageListViewModel> Images { get; set; }
        public string ProductGeneralSpecification { get; set; }
    }
}
