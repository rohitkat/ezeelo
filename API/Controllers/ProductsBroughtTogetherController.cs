using API.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class ProductsBroughtTogetherController : ApiController
    {

        private EzeeloDBContext db = new EzeeloDBContext();

        // POST api/productsbroughttogether
        /// <summary>
        /// Get the list of product which can be brought with selected product. e.g. Screen guared,flip Cover with mobile.
        /// </summary>
        /// <param name="searchFrequent">Object with members like CategoryID, ShopID, PageIndex, PageSize etc.</param>
        /// <returns></returns>
        [ApiException]
        [ValidateModel]
        public object Post(SearchFrequentlyBuyedProductViewModel searchFrequent)
        {
            object obj = new object();
            try
            {
                if (!ModelState.IsValid || searchFrequent == null)
                {
                    return obj = new { Success = 0, Message = "Invalid paramters", data = string.Empty };
                    //obj = new { HTTPStatusCode = "400", UserMessage = "Invalid paramters", ValidationError = "One of the parameter is not valid." };
                    //return Request.CreateResponse(HttpStatusCode.BadRequest, obj);
                }
                if (searchFrequent.ProductID == 0 || searchFrequent.ProductID == null)
                {
                    return obj = new { Success = 0, Message = "Invalid ProductId", data = string.Empty };
                }
                RelatedProductsViewModel relatedProducts = new RelatedProductsViewModel();
                ProductDetails productList = new ProductDetails(System.Web.HttpContext.Current.Server);
                var ShopStockId = (from p in db.Products
                                   join sp in db.ShopProducts on p.ID equals sp.ProductID
                                   join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                                   where p.ID == searchFrequent.ProductID
                                   select ss.ID).FirstOrDefault();
                if (ShopStockId != null && ShopStockId > 0)
                {
                    searchFrequent.ShopStockId = ShopStockId;
                    //RelatedProductsViewModel lBuyedTogetherProducts = new RelatedProductsViewModel();

                    string query = string.Empty;
                    query = "[Select_ProductbroughtTogether]";
                    ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                    SqlCommand cmd = new SqlCommand(query);
                    cmd.CommandType = CommandType.StoredProcedure;

                    //cmd.Parameters.AddWithValue("@CityID", searchFrequentlyBuyedProductViewModel.CityID);
                    cmd.Parameters.AddWithValue("@FranchiseID", searchFrequent.FranchiseID);////added
                    cmd.Parameters.AddWithValue("@ShopStockId", searchFrequent.ShopStockId);

                    DataSet ds = new DataSet();
                    using (SqlConnection con = new SqlConnection(readCon.DB_CONNECTION))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.Connection = con;
                            sda.SelectCommand = cmd;
                            sda.Fill(ds);
                        }
                    }

                    //Collect products in list
                    var lproductList = (from DataRow dr in ds.Tables[0].Rows
                                        select new SearchProductDetailsViewModel()
                                        {
                                            ProductThumbPath = string.Empty,
                                            ProductID = Convert.ToInt32(dr["ProductID"]),
                                            Name = dr["ProductName"].ToString(),
                                            CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                            MRP = Convert.ToDecimal(dr["MRP"]),
                                            SaleRate = Convert.ToDecimal(dr["SaleRate"]),
                                            StockStatus = Convert.ToInt32(dr["StockStatus"])
                                        }).ToList();
                    relatedProducts.ProductList = lproductList;

                    //retrive colors and get the thumbnail for first color
                    foreach (var item in relatedProducts.ProductList)
                    {

                        var color = (from pv in db.ProductVarients
                                     join c in db.Colors on pv.ColorID equals c.ID
                                     where pv.ProductID == item.ProductID

                                     select new
                                     {
                                         name = c.Name

                                     }).FirstOrDefault();
                        //if (color != null && color.name != "N/A")
                        //    item.ProductThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, color.name, string.Empty, ProductUpload.THUMB_TYPE.SD);
                        //else
                        //    item.ProductThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);

                        if (color != null && color.name != "N/A")
                            item.ProductThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, color.name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        else
                            item.ProductThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                    }
                    //get product Count
                    //if (ds.Tables[1] != null)
                    //{
                    //    SearchCountViewModel searchCount = new SearchCountViewModel();
                    //    searchCount.PageCount = Convert.ToInt32(cmd.Parameters["@PageCount"].Value);
                    //    searchCount.ProductCount = Convert.ToInt32(cmd.Parameters["@Productcount"].Value);

                    //    relatedProducts.SearchCount = searchCount;

                    //}
                }

                // relatedProducts = productList.GetFrequentlyBuyedProducts(searchFrequent);
                if (relatedProducts != null && relatedProducts.ProductList.Count > 0)
                {
                    obj = new { Success = 1, Message = "Frequently buy products are found.", data = relatedProducts };
                }
                else
                {
                    obj = new { Success = 0, Message = "Frequently buy products are not found.", data = string.Empty };
                }
                // return Request.CreateResponse(HttpStatusCode.OK, relatedProducts);

            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
