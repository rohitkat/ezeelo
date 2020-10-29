using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class Deals
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        public List<OfferProducts> GetDealsProducts(long cityId, OfferStatus lOfferStatus, int catID, int pageIndex, int pageSize, long DealID, int? franchiseId = null)////added int? franchiseId
        {
            Offers lOffers = new Offers();
            List<OfferProducts> lOfferProducts = new List<OfferProducts>();
            // CategoryWiseOffersViewModel lCategoryWiseOffersViewModel = new CategoryWiseOffersViewModel();

            ReadConfig lReadCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            string query = "[Select_Deal_Product_CategoryWiseList]";

            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CityID", cityId);

            cmd.Parameters.AddWithValue("@FranchiseId", franchiseId);////added franchiseId 

            cmd.Parameters.AddWithValue("@offerStatus", lOfferStatus);
            cmd.Parameters.AddWithValue("@CategoryID", catID);
            cmd.Parameters.AddWithValue("@DealID", DealID);
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
                                    OfferPrice = lOffers.GetOfferPrice(Convert.ToDecimal(dr["SaleRate"]), Convert.ToInt32(dr["%Offer"]), Convert.ToInt32(dr["RsOffer"])),
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

        public List<DealBannerList> GetDealsBanners(long franchiseId, long DealId)
        {
            List<DealBannerList> objDealBannerList = new List<DealBannerList>();
            try
            {
                objDealBannerList = db.DealBannerLists.Where(x => x.DealId == DealId && x.FranchiseId == franchiseId && x.IsActive && EntityFunctions.TruncateTime(x.StartDate) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(x.EndDate) >= EntityFunctions.TruncateTime(DateTime.Now)).ToList();
            }
            catch (Exception ex)
            {

            }
            return objDealBannerList;
        }
        public List<DealCategoryList> GetDealsCategory(long franchiseId, long DealId)
        {
            List<DealCategoryList> objDealCategoryList = new List<DealCategoryList>();
            try
            {
                objDealCategoryList = db.DealCategoryLists.Where(x => x.DealId == DealId && x.FranchiseId == franchiseId && x.IsActive && EntityFunctions.TruncateTime(x.StartDate) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(x.EndDate) >= EntityFunctions.TruncateTime(DateTime.Now)).ToList();
            }
            catch (Exception ex)
            {

            }
            return objDealCategoryList;
        }
        public long? GetBannerIdByDealBanner(DealBannerList bannerList)
        {
            long? Id = null;
            try
            {
                if (bannerList.DisplayViewApp == "categorylist")
                    Id = bannerList.CategoryID ?? 0;
                else if (bannerList.DisplayViewApp == "productlist")
                    Id = bannerList.Id;
                else if (bannerList.DisplayViewApp == "bannerlist")
                    Id = bannerList.Id;
                else if (bannerList.DisplayViewApp == "OfferProductList")
                    Id = bannerList.Id;

            }
            catch (Exception ex)
            {

            }
            return Id;
        }

        public long? GetBannerIdByDealSubBanner(DealCategoryList SubBannerList)
        {
            long? Id = null;
            try
            {
                if (SubBannerList.DisplayViewApp == "categorylist")
                    Id = SubBannerList.CategoryID ?? 0;
                else if (SubBannerList.DisplayViewApp == "productlist")
                    Id = SubBannerList.Id;
                else if (SubBannerList.DisplayViewApp == "bannerlist")
                    Id = SubBannerList.Id;
                else if (SubBannerList.DisplayViewApp == "OfferProductList")
                    Id = SubBannerList.Id;

            }
            catch (Exception ex)
            {

            }
            return Id;
        }
        public long? GetBannerIdByBlockItemList(HomePageBlockItemsViewModel SubBannerList)
        {
            long? Id = null;
            try
            {
                if (SubBannerList.DisplayViewApp == "categorylist")
                    Id = SubBannerList.CategoryID ?? 0;
                else if (SubBannerList.DisplayViewApp == "productlist")
                    Id = SubBannerList.ID;
                else if (SubBannerList.DisplayViewApp == "bannerlist")
                    Id = SubBannerList.ID;
                else if (SubBannerList.DisplayViewApp == "OfferProductList")
                    Id = SubBannerList.ID;

            }
            catch (Exception ex)
            {

            }
            return Id;
        }

        public List<OfferProducts> GetHomeProductList(long cityId, OfferStatus lOfferStatus, int catID, int pageIndex, int pageSize, long HomePageSectionId, int? franchiseId = null)
        {
            Offers lOffers = new Offers();
            List<OfferProducts> lOfferProducts = new List<OfferProducts>();
            // CategoryWiseOffersViewModel lCategoryWiseOffersViewModel = new CategoryWiseOffersViewModel();

            ReadConfig lReadCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            string query = "[Select_HomePage_ProductList]";

            SqlCommand cmd = new SqlCommand(query);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CityID", cityId);

            cmd.Parameters.AddWithValue("@FranchiseId", franchiseId);////added franchiseId 

            cmd.Parameters.AddWithValue("@offerStatus", lOfferStatus);
            cmd.Parameters.AddWithValue("@CategoryID", catID);
            cmd.Parameters.AddWithValue("@HomePageSectionId", HomePageSectionId);
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
                                    //OfferPercent = Convert.ToInt32(dr["%Offer"]),
                                    //OfferRs = Convert.ToInt32(dr["RsOffer"]),
                                    OfferStartTime = Convert.ToDateTime(dr["OfferStartTime"]),
                                    OfferEndTime = Convert.ToDateTime(dr["OfferEndTime"]),
                                    OfferName = dr["OfferName"].ToString(),
                                    //OfferPrice = lOffers.GetOfferPrice(Convert.ToDecimal(dr["SaleRate"]), Convert.ToInt32(dr["%Offer"]), Convert.ToInt32(dr["RsOffer"])),
                                    ShortDescription = dr["shortDescription"].ToString(),
                                    MaterialName = dr["Material"].ToString(),
                                    ColorName = dr["Color"].ToString(),
                                    SizeName = dr["Size"].ToString(),
                                    DimensionName = dr["Dimension"].ToString(),
                                    ShopID = Convert.ToInt32(dr["ShopID"]),
                                    ShopName = dr["ShopName"].ToString(),
                                    RetailPoint = Convert.ToDecimal(dr["BusinessPoints"]),
                                    CashbackPoints = Convert.ToDecimal(dr["CashbackPoints"]),
                                    IsDisplayCB = Convert.ToInt16(dr["IsDisplayCB"]),
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
