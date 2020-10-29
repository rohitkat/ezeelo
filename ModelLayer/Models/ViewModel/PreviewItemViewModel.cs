using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class PreviewItemViewModel
    {
        public ProductViewBasicDetailsViewModels BasicDetails { get; set; }
        public IEnumerable<ShopProductVarientViewModel> StockVarient { get; set; }
        public List<ProductSellersViewModel> ProductSellers { get; set; }
        public ProductOffersViewModel ProductOffers { get; set; }
        public PayablePricesViewModel PayablePrice { get; set; }
        public StockComponentsViewModel StockComponents { get; set; }
        public RelatedProductsViewModel SimilarProducts { get; set; }
        public RelatedProductsViewModel FrequentlyBuyedProducts { get; set; }
        public IEnumerable<ProductTechnicalSpecificationViewModel> TechnicalSpecification { get; set; }
        public string GeneralSpecifications { get; set; }
        public RecentlyViewedCollectionViewModel RecentlyViewedItems { get; set; }
        public DisplayReviewsViewModel ProductReviews { get; set; }
        public DisplayReviewsViewModel ShopReviews { get; set; }
        public BuyThisProductViewModel BuyProduct { get; set; }
        
        //Added by Manoj 
        public long? DeliveryTime { get; set; }

        //Added for Preview page landing changes
        public List<RefineCategoryViewModel> lCatList { get; set; }

    }
}
