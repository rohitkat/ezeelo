using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class BrandProduct
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public List<OfferProducts> GetBrandProductList(long FranchiseId, long BrandId)
        {
            List<OfferProducts> ProductList = new List<OfferProducts>();
            try
            {
                ProductList = (from SS in db.ShopStocks
                               join SP in db.ShopProducts on SS.ShopProductID equals SP.ProductID
                               join S in db.Shops on SP.ShopID equals S.ID
                               join P in db.Products on SP.ProductID equals P.ID
                               join PV in db.ProductVarients on SS.ProductVarientID equals PV.ID
                               where P.BrandID == BrandId && S.FranchiseID == FranchiseId && S.IsActive == true && S.IsLive == true && SS.Qty != 0 && SS.IsActive == true && SP.IsActive == true
                               select new OfferProducts
                               {
                                   ProductID = P.ID,
                                   ShopStockID = SS.ID,
                                   ProductName = P.Name,
                                   CategoryID = P.CategoryID,
                                   CategoryName = P.Category.Name,
                                   StockStatus = SS.StockStatus ? 1 : 0,
                                   StockQty = SS.Qty,
                                   MRP = SS.MRP,
                                   SaleRate = SS.RetailerRate,
                                   OfferID = 0,
                                   OfferPercent = 0,
                                   OfferRs = 0,
                                   OfferName = string.Empty,
                                   OfferPrice = 0,
                                   ShortDescription = P.Description,
                                   BrandName = P.Brand.Name,
                                   ColorName = PV.Color.Name,
                                   SizeName = PV.Size.Name,
                                   DimensionName = PV.Dimension.Name,
                                   MaterialName = PV.Material.Name,
                                   ShopID = (int)S.ID,
                                   ShopName = S.Name,
                                   RetailPoint = SS.BusinessPoints,
                                   BrandId = P.BrandID,//Added By sonali on 23-02-2019
                                   MaterialId = PV.MaterialID,//Added By sonali on 23-02-2019
                                   DimensionId = PV.DimensionID,//Added By sonali on 23-02-2019
                                   SizeId = PV.SizeID,//Added By sonali on 23-02-2019
                                   PackedUnit = SS.Unit.Name//Added By sonali on 23-02-2019
                               }).ToList();
                if (ProductList != null && ProductList.Count > 0)
                {
                    foreach (var item in ProductList)
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

            }
            catch (Exception ex)
            {

            }
            return ProductList;
        }

    }
}
