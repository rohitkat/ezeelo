//-----------------------------------------------------------------------
// <copyright file="BestDealProducts.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Tejaswee Taktewale</author>
//This class file is used to display shop's offer produucts
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;

namespace BusinessLogicLayer
{
    public class BestDealProducts
    {

        public ProductStockVarientViewModel GetProductList(BestDealProductSearchViewModel bestDealProductSearchViewModel)
        {
            ProductStockVarientViewModel productStockVarientViewModel = new ProductStockVarientViewModel();

            List<ProductStockDetailViewModel> productList = new List<ProductStockDetailViewModel>();
            SearchCountViewModel searchCount = new SearchCountViewModel();

            DataSet ds = GetBestDealProducts(bestDealProductSearchViewModel);


            if (ds.Tables[0] != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {

                    List<ProductRefinementsViewModel> lproductRefinements = new List<ProductRefinementsViewModel>();

                    productList = (from DataRow dr in ds.Tables[0].Rows
                                   select new ProductStockDetailViewModel()
                                           {

                                               BrandID = Convert.ToInt32(dr["BrandID"]),
                                               BrandName = dr["BrandName"].ToString(),
                                               CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                               CategoryName = dr["CategoryName"].ToString(),
                                               CityID = Convert.ToInt32(dr["CityID"]),
                                               ColorID = Convert.ToInt32(dr["ColorID"]),
                                               ColorName = dr["Color"].ToString(),
                                               DimensionID = Convert.ToInt32(dr["DimensionID"]),
                                               DimensionName = dr["Dimension"].ToString(),
                                               MaterialID = Convert.ToInt32(dr["MaterialID"]),
                                               MaterialName = dr["Material"].ToString(),
                                               MRP = Convert.ToDecimal(dr["MRP"]),
                                               ProductID = Convert.ToInt64(dr["ProductID"]),
                                               ProductName = dr["ProductName"].ToString(),
                                               SaleRate = Convert.ToDecimal(dr["SaleRate"]),
                                               ShopID = Convert.ToInt64(dr["ShopID"]),
                                               ShopName = dr["ShopName"].ToString(),
                                               SizeID = Convert.ToInt32(dr["SizeID"]),
                                               SizeName = dr["Size"].ToString(),

                                           }).ToList();


                    foreach (var item in productList)
                    {
                        if (item.ColorName != "N/A")
                        {
                            //    item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName, string.Empty, ProductUpload.THUMB_TYPE.SD);
                            item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.ColorName, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        }
                        else
                        {
                            //    item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                            item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        }
                    }

                    productStockVarientViewModel.ProductInfo = productList;
                }
                if (ds.Tables[1] != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        searchCount.PageCount = Convert.ToInt32(ds.Tables[1].Rows[0]["PageCount"]);
                        searchCount.ProductCount = Convert.ToInt32(ds.Tables[1].Rows[0]["Productcount"]);

                        productStockVarientViewModel.searchCount = searchCount;
                    }
                }
            }
            return productStockVarientViewModel;

        }
        private DataSet GetBestDealProducts(BestDealProductSearchViewModel bestDealProductSearchViewModel)
        {
            EzeeloDBContext db = new EzeeloDBContext();
            string query = string.Empty;
            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);

            query = "[Search_BestDealProducts]";
            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CityID", bestDealProductSearchViewModel.CityID);
            cmd.Parameters.AddWithValue("@offerID", bestDealProductSearchViewModel.OfferID);
            cmd.Parameters.AddWithValue("@offerType", bestDealProductSearchViewModel.OfferType);
            cmd.Parameters.AddWithValue("@ShopID", bestDealProductSearchViewModel.ShopID);

            cmd.Parameters.AddWithValue("@PageIndex", bestDealProductSearchViewModel.PageIndex);
            cmd.Parameters.AddWithValue("@PageSize", bestDealProductSearchViewModel.PageSize);
            cmd.Parameters.AddWithValue("@CategoryID", bestDealProductSearchViewModel.CategoryID);
            cmd.Parameters.AddWithValue("@BrandID", bestDealProductSearchViewModel.BrandID);

            cmd.Parameters.Add("@PageCount", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@Productcount", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
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
            DataTable dt = new DataTable();
            dt.Columns.Add("PageCount", typeof(int));
            dt.Columns.Add("ProductCount", typeof(int));
            dt.Rows.Add(cmd.Parameters["@PageCount"].Value, cmd.Parameters["@Productcount"].Value);
            ds.Tables.Add(dt);
            var qry = from products in db.Products
                      select products;
            return ds;
        }
    }
}
