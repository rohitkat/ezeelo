using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class GetBrandBannerDataController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public object Get(long id, long FranchiseId)
        {

            object obj = new object();
            try
            {
                if (id == null || id <= 0 || FranchiseId == null || FranchiseId <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid Parameter.", data = string.Empty };
                }
                var result = (from f in db.Franchises
                              join p in db.Pincodes on f.PincodeID equals p.ID
                              where f.ID == FranchiseId && f.IsActive == true
                              select new { CityId = (int)p.CityID }).FirstOrDefault();
                //This data fetch is temprorely by Sonali
                ProductSearchViewModel productSearch = new ProductSearchViewModel();
                if (result != null)
                {
                    productSearch.CityID = result.CityId;
                }
                productSearch.FranchiseID = (int)FranchiseId;
                productSearch.ShopID = 0;
                productSearch.Keyword = db.Brands.Where(x => x.ID == id).Select(x => x.Name).FirstOrDefault();
                productSearch.CategoryID = 0;
                productSearch.BrandIDs = id.ToString();
                productSearch.ProductID = 0;
                productSearch.ColorIDs = string.Empty;
                productSearch.SizeIDs = string.Empty;
                productSearch.DimensionIDs = string.Empty;
                productSearch.MaterialIDs = string.Empty;
                productSearch.SpecificationIDs = string.Empty;
                productSearch.SpecificationValues = string.Empty;
                productSearch.MinPrice = 0;
                productSearch.MaxPrice = 0;
                productSearch.PageIndex = 1;
                productSearch.PageSize = 50;
                productSearch.Version = 1;
                productSearch.IsScroll = false;
                productSearch.CustLoginID = 0;
                productSearch.SearchInCategoryOnly = false;
                productSearch.ImageType = string.Empty;
                productSearch.IsVarientRestricted = false;
                productSearch.isListVarient = false;
                productSearch.SortVal = 0;
                ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
                ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                productWithRefinementViewModel = productList.GetProductList(productSearch, false);
                if (productWithRefinementViewModel != null)
                {
                    obj = new { Success = 1, Message = "Product list are found.", data = new { id = 1, productWithRefinementViewModel } };
                }
                //This data fetch is temprorely by Sonali
                //BrandProduct brandproduct = new BrandProduct();
                //ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
                //List<OfferProducts> productList = brandproduct.GetBrandProductList(FranchiseId,id);
                //obj = new { Success = 1, Message = "Product List Found", data = productList };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
