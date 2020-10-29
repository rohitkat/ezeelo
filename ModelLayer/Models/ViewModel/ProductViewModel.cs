using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.ViewModel
{
    public class ProductViewModel
    {
        public List<SearchProductDetailsViewModel> productList { get; set; }
        public List<RefineCategoryViewModel> categoryList { get; set; }
        public List<RefineBrandViewModel> brandList { get; set; }
        public List<RefineColorViewModel> colorList { get; set; }
        public List<RefineSizeViewModel> sizeList { get; set; }
        public List<RefineDimensionViewModel> dimensionList { get; set; }
        public List<RefineDetailHeadViewModel> detailHeadList { get; set; }
        public List<RefinePriceViewModel> priceList { get; set; }
        public ViewShopDetailsViewModel shopDetails { get; set; }
        public ProductStockVarientViewModel shopBestDeals { get; set; }

    }

    public class PartialProductListParam
    {
        public List<SearchProductDetailsViewModel> productList { get; set; }

      }
}