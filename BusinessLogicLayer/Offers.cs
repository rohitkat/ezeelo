//-----------------------------------------------------------------------
// <copyright file="Offers.cs" company="eZeelo">
// Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Tejaswee Taktewale</author>
//<date>22-Jan-2016</date>
//<Summary> This file is sed for showing "Deal of the Day", "Weekly Deal" etc </Summary>
//-----------------------------------------------------------------------

using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer;
using ModelLayer.Models;

namespace BusinessLogicLayer
{

    public class Offers
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        /// <summary>
        /// Get list of categories on which offer applicable for "Deal of the Day"
        /// Currently deal price is a sale rate  (No difference)
        /// </summary>
        /// <param name="searchDealProducts"></param>
        /// <returns></returns>
        public List<OfferCategoryList> GetOfferCategory(long cityId, int? franchiseId = null)////added int? franchiseId for Multiple MCO/Old App
        {
            List<OfferCategoryList> lCategoryList = new List<OfferCategoryList>();
            // CategoryWiseOffersViewModel lCategoryWiseOffersViewModel = new CategoryWiseOffersViewModel();

            ReadConfig lReadCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            string query = "[Select_Offer_Category_List]";

            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CityID", cityId);
            cmd.Parameters.AddWithValue("@FranchiseId", franchiseId);////added franchiseId
            //cmd.Parameters.AddWithValue("@FromDate", System.DateTime.UtcNow);
            //cmd.Parameters.AddWithValue("@ToDate", cityId);

            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(lReadCon.DB_CONNECTION))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    sda.Fill(ds);
                }
            }

            var lCatList = (from DataRow dr in ds.Tables[0].Rows
                            select new OfferCategoryList()
                                {
                                    FirstLevelCatID = Convert.ToInt32(dr["categoryIDFirstLevel"]),
                                    FirstLevelCatName = Convert.ToString(dr["NameFirstLevel"]),
                                    //StartDate = Convert.ToDateTime(dr["StartDateTime"]),
                                    //EndDate = Convert.ToDateTime(dr["EndDateTime"]),

                                }).ToList();
            if (lCatList != null)
            {
                if (franchiseId != null)////For New APP
                {
                    foreach (var item in lCatList)
                    {
                        BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);
                        item.ImagePath = rcKey.CATEGORY_IMAGE_HTTP + "/" + cityId + "/" + franchiseId + "/" + item.FirstLevelCatID + "_ll" + ".png";////added "/" + franchiseId +
                    }
                }
                else
                {
                    foreach (var item in lCatList)////For Old APP
                    {
                        BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);
                        item.ImagePath = rcKey.CATEGORY_IMAGE_HTTP + "/" + cityId + "/" + item.FirstLevelCatID + "_ll" + ".png";
                    }
                }
            }
            lCategoryList = lCatList;

            return lCategoryList;
        }

        public List<OfferProducts> GetOfferProducts(long cityId, OfferStatus lOfferStatus, int catID, int pageIndex, int pageSize, int? franchiseId = null)////added int? franchiseId
        {
            List<OfferProducts> lOfferProducts = new List<OfferProducts>();
            // CategoryWiseOffersViewModel lCategoryWiseOffersViewModel = new CategoryWiseOffersViewModel();

            ReadConfig lReadCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            string query = "[Select_Offer_Product_CategoryWiseList]";

            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CityID", cityId);

            cmd.Parameters.AddWithValue("@FranchiseId", franchiseId);////added franchiseId 

            cmd.Parameters.AddWithValue("@offerStatus", lOfferStatus);
            cmd.Parameters.AddWithValue("@CategoryID", catID);
            cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(lReadCon.DB_CONNECTION))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    sda.Fill(ds);
                }
            }

            var lproductList = (from DataRow dr in ds.Tables[0].Rows
                                select new OfferProducts()
                                {
                                    ProductID = Convert.ToInt64(dr["ProductID"]),
                                    ShopStockID = Convert.ToInt64(dr["ShopStockID"]),
                                    ProductName = dr["ProductName"].ToString(),
                                    CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                    CategoryName = dr["CategoryName"].ToString(),
                                    StockStatus = Convert.ToInt32(dr["StockStatus"]),
                                    StockQty = Convert.ToInt32(dr["StockQty"]),
                                    MRP = Convert.ToDecimal(dr["MRP"]),
                                    SaleRate = Convert.ToDecimal(dr["SaleRate"]),
                                    OfferID = Convert.ToInt64(dr["OfferID"]),
                                    OfferPercent = Convert.ToInt32(dr["%Offer"]),
                                    OfferRs = Convert.ToInt32(dr["RsOffer"]),
                                    OfferStartTime = Convert.ToDateTime(dr["OfferStartTime"]),
                                    OfferEndTime = Convert.ToDateTime(dr["OfferEndTime"]),
                                    OfferName = dr["OfferName"].ToString(),
                                    OfferPrice = GetOfferPrice(Convert.ToDecimal(dr["SaleRate"]), Convert.ToInt32(dr["%Offer"]), Convert.ToInt32(dr["RsOffer"])),
                                    ShortDescription = dr["shortDescription"].ToString(),
                                    MaterialName = dr["Material"].ToString(),
                                    ColorName = dr["Color"].ToString(),
                                    SizeName = dr["Size"].ToString(),
                                    DimensionName = dr["Dimension"].ToString(),
                                    ShopID = Convert.ToInt32(dr["ShopID"]),
                                    ShopName = dr["ShopName"].ToString(),
                                    RetailPoint = Convert.ToDecimal(dr["BusinessPoints"]),
                                    OfferType = Convert.ToInt32(dr["OfferType"]),//Sonali _17-11-2018
                                    CashbackPoints = Convert.ToDecimal(dr["CashbackPoints"]),
                                    IsDisplayCB = Convert.ToInt16(dr["IsDisplayCB"])
                                    //Retail Point Yashaswi 9-7-2018
                                    //StartDate = Convert.ToDateTime(dr["StartDateTime"]),
                                    //EndDate = Convert.ToDateTime(dr["EndDateTime"]),

                                }).ToList();
            if (lproductList != null)
            {
                foreach (var item in lproductList)
                {
                    var color = (from pv in db.ProductVarients
                                 join c in db.Colors on pv.ColorID equals c.ID
                                 where pv.ProductID == item.ProductID

                                 select new
                                 {
                                     name = c.Name

                                 }).FirstOrDefault();
                    if (color != null && color.name != "N/A")
                    {
                        //pD.ThumbPath  = ImageDisplay.LoadProductThumbnails(pID, color.name, string.Empty, ProductUpload.THUMB_TYPE.SD);
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, color.name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        item.StockSmallImagePath = ImageDisplay.LoadProductThumbnails(item.ProductID, color.name, string.Empty, ProductUpload.THUMB_TYPE.LL);
                    }
                    else
                    {
                        //pD.ThumbPath = ImageDisplay.LoadProductThumbnails(pID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        item.StockSmallImagePath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.LL);
                    }
                    //Added for SEO URL Structure RULE by AShish
                    item.ProductName = item.ProductName.Replace("+", " ");
                }
            }
            lOfferProducts = lproductList;

            return lOfferProducts;
        }


        //public List<OfferCategoryList> GetGBDeals(int cityID)
        //{
        //    //string ss =  ConfigurationManager.AppSettings["sdds"];
        //    string[] mykey =  System.Configuration.ConfigurationManager.AppSettings["GB_DEALS_" + cityID].Split(',');
        //    GBDealsViewModel lGBDealsViewModel = new GBDealsViewModel();

        //    List<GBDealsViewModel> listGBDealsViewModel = new List<GBDealsViewModel>();
        //    foreach (var item in mykey)
        //    {
        //        if (item != string.Empty)
        //        {
        //            string[] val = item.Split('/');
        //            lGBDealsViewModel.CatID = Convert.ToInt32(val[0]);
        //            lGBDealsViewModel.ShopID = Convert.ToInt64(val[1]);

        //            BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);
        //            //item.ImagePath = rcKey.CATEGORY_IMAGE_HTTP + "/" + cityId + "/" + item.FirstLevelCatID + "_ll" + ".png";
        //            lGBDealsViewModel.DealThumbPath = rcKey.GB_DEALS_IMAGE + "/" + cityID + "/" + Convert.ToInt32(val[0]) + ".png";
        //            listGBDealsViewModel.Add(lGBDealsViewModel);
        //        }
        //    }
        //    return listGBDealsViewModel;
        //}

        public long GetOfferPrice(decimal saleRate, decimal offerPer, decimal offerRs)//Change private to public by Sonali on 18-12-2018
        {
            decimal offerPrice = Convert.ToInt64(saleRate);
            try
            {
                if (offerPer > 0)
                {
                    offerPrice = offerPrice - ((offerPer / 100) * offerPrice);
                }
                else if (offerRs > 0)
                {
                    offerPrice = offerPrice - offerRs;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Convert.ToInt64(offerPrice);
        }

        //Sonali_27-12-2018
        public List<OfferProducts> GetDealsProducts(long cityId, OfferStatus lOfferStatus, int catID, int pageIndex, int pageSize, int? franchiseId = null)////added int? franchiseId
        {
            List<OfferProducts> lOfferProducts = new List<OfferProducts>();
            // CategoryWiseOffersViewModel lCategoryWiseOffersViewModel = new CategoryWiseOffersViewModel();

            ReadConfig lReadCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            string query = "[Select_Offer_Product_CategoryWiseList]";

            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CityID", cityId);

            cmd.Parameters.AddWithValue("@FranchiseId", franchiseId);////added franchiseId 

            cmd.Parameters.AddWithValue("@offerStatus", lOfferStatus);
            cmd.Parameters.AddWithValue("@CategoryID", catID);
            cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(lReadCon.DB_CONNECTION))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    sda.Fill(ds);
                }
            }

            var lproductList = (from DataRow dr in ds.Tables[0].Rows
                                select new OfferProducts()
                                {
                                    ProductID = Convert.ToInt64(dr["ProductID"]),
                                    ShopStockID = Convert.ToInt64(dr["ShopStockID"]),
                                    ProductName = dr["ProductName"].ToString(),
                                    CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                    CategoryName = dr["CategoryName"].ToString(),
                                    StockStatus = Convert.ToInt32(dr["StockStatus"]),
                                    StockQty = Convert.ToInt32(dr["StockQty"]),
                                    MRP = Convert.ToDecimal(dr["MRP"]),
                                    SaleRate = Convert.ToDecimal(dr["SaleRate"]),
                                    OfferID = Convert.ToInt64(dr["OfferID"]),
                                    OfferPercent = Convert.ToInt32(dr["%Offer"]),
                                    OfferRs = Convert.ToInt32(dr["RsOffer"]),
                                    OfferStartTime = Convert.ToDateTime(dr["OfferStartTime"]),
                                    OfferEndTime = Convert.ToDateTime(dr["OfferEndTime"]),
                                    OfferName = dr["OfferName"].ToString(),
                                    OfferPrice = GetOfferPrice(Convert.ToDecimal(dr["SaleRate"]), Convert.ToInt32(dr["%Offer"]), Convert.ToInt32(dr["RsOffer"])),
                                    ShortDescription = dr["shortDescription"].ToString(),
                                    MaterialName = dr["Material"].ToString(),
                                    ColorName = dr["Color"].ToString(),
                                    SizeName = dr["Size"].ToString(),
                                    DimensionName = dr["Dimension"].ToString(),
                                    ShopID = Convert.ToInt32(dr["ShopID"]),
                                    ShopName = dr["ShopName"].ToString(),
                                    RetailPoint = Convert.ToDecimal(dr["BusinessPoints"])
                                    //Retail Point Yashaswi 9-7-2018
                                    //StartDate = Convert.ToDateTime(dr["StartDateTime"]),
                                    //EndDate = Convert.ToDateTime(dr["EndDateTime"]),

                                }).ToList();
            if (lproductList != null)
            {
                foreach (var item in lproductList)
                {
                    var color = (from pv in db.ProductVarients
                                 join c in db.Colors on pv.ColorID equals c.ID
                                 where pv.ProductID == item.ProductID

                                 select new
                                 {
                                     name = c.Name

                                 }).FirstOrDefault();
                    if (color != null && color.name != "N/A")
                    {
                        //pD.ThumbPath  = ImageDisplay.LoadProductThumbnails(pID, color.name, string.Empty, ProductUpload.THUMB_TYPE.SD);
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, color.name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        item.StockSmallImagePath = ImageDisplay.LoadProductThumbnails(item.ProductID, color.name, string.Empty, ProductUpload.THUMB_TYPE.LL);
                    }
                    else
                    {
                        //pD.ThumbPath = ImageDisplay.LoadProductThumbnails(pID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                        item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        item.StockSmallImagePath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.LL);
                    }
                    //Added for SEO URL Structure RULE by AShish
                    item.ProductName = item.ProductName.Replace("+", " ");
                }
            }
            lOfferProducts = lproductList;

            return lOfferProducts;
        }


    }
}



public enum OfferStatus
{
    MISSEDDEALS = 0,

    AVAILABLEDEALS = 1,

    UPCOMINGDEALS = 2

};
