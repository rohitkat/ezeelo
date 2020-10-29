using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Gandhibagh.Controllers
{
    public class TestAPIController : Controller
    {
       
        // GET: /TestAPI/
        public ActionResult Test()
        {
            //BusinessLogicLayer.Review rObj = new BusinessLogicLayer.Review();
            //List<CustomerReviewViewModel> rCollection = rObj.GetReviews(4, Review.REVIEWS.PRODUCT); 

            return View("View1");

            //string thumb = ImageDisplay.LoadProductThumbnails(100, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);

            //ViewBag.thumbPath = thumb;
            //return View("View1");

            //CustomerDetailsViewModel cust = new CustomerDetailsViewModel();
            //cust.Address = "Ravi Nagar";
            //cust.EmailId = "HarshadaRaghorte@gmail.com";
            //cust.FirstName = "Harshada";
            //cust.LastName = "Raghorte";
            //cust.MobileNo = "80004521978";
            //cust.Password = "123456";
            //cust.PincodeID = 8304;
            //cust.SalutationID = 2;      
            


            //BusinessLogicLayer.CustomerDetails lcustDetails = new BusinessLogicLayer.CustomerDetails(System.Web.HttpContext.Current.Server);
            //int i = lcustDetails.CreateCustomer(cust);


            //ProductDetails prod = new ProductDetails(System.Web.HttpContext.Current.Server);
            //ProductViewBasicDetailsViewModels s = prod.GetBasicDetails(29);


            // EzeeloDBContext db = new EzeeloDBContext();
            ////GBOD15061200000
            //string newOrderCode = string.Empty;
            //int lYear = 0;
            //int lMonth = 0;
            //int lDay = 0;
            //int.TryParse(DateTime.UtcNow.AddHours(5.5).Year.ToString(), out lYear);
            //int.TryParse(DateTime.UtcNow.AddHours(5.5).Month.ToString(), out lMonth);
            //int.TryParse(DateTime.UtcNow.AddHours(5.5).Day.ToString(), out lDay);
            //string lOrderPrefix = "MROD" + lYear.ToString().Substring(2, 2) + lMonth.ToString("00") + lDay.ToString("00");
            //var lastOrder = db.CustomerOrderDetails.Where(x => x.ShopID == 11).OrderByDescending(x => x.ID).FirstOrDefault();
            //if (lastOrder != null)
            //{

            //    string OrderCode = lastOrder.ShopOrderCode;
            //    if (OrderCode.Length == 15)
            //    {

            //        if (lOrderPrefix.Equals(OrderCode.ToString().Substring(0,10)))
            //        {

            //            newOrderCode = lOrderPrefix + (Convert.ToInt32(OrderCode.ToString().Substring(10, 5)) + 1).ToString("00000");

            //        }
            //        else
            //            newOrderCode = lOrderPrefix + "00001";
            //    }
            //    else
            //        newOrderCode = lOrderPrefix + "00001";
               
            //}
            //else
            //{
            //    //FirstOrder
            //    newOrderCode = lOrderPrefix + "00001";

            //}

            //Product testProduct = db.Products.Single(x => x.ID == 29);

            //Get Customer Orders
            //BusinessLogicLayer.CustomerOrder co = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);             
            //ViewCustomerOrderViewModel vc = co.GetCustomerOrders(3); 

            
            //CustomerShippingDetails sAddr = new CustomerShippingDetails (System.Web.HttpContext.Current.Server);
            //List<CustomerShippingAddressesViewModel> shadree = sAddr.GetCustomerShippingAddresses(3); 

            //ProductDetails p = new ProductDetails(System.Web.HttpContext.Current.Server);
            //ProductList pl = new ProductList(System.Web.HttpContext.Current.Server);


            //ImageDisplay im = new ImageDisplay(System.Web.HttpContext.Current.Server);
            //PincodeVerification pin = new PincodeVerification();
           
            ////ProductViewBasicDetailsViewModels pd = new ProductViewBasicDetailsViewModels();
            //Product pp = new Product();
            ////pp = p.GetBasicDetails(29);

            //IEnumerable<ShopProductVarientViewModel> list = p.GetStockVarients(29);

            //List<ProductSellersViewModel> sellers = p.GetSellersDealsInProduct(29);

            //StockComponentsViewModel lComponents = p.GetStockComponentDetails(37,11);
            //ShopProductVarientViewModel ss = new ShopProductVarientViewModel();
            //ProductDetails prod = new ProductDetails(System.Web.HttpContext.Current.Server);
            //ss = prod.GetShopStockVarients(4, null);          

            //ProductSearchViewModel productSearch = new ProductSearchViewModel();
            //productSearch.BrandIDs = string.Empty;
            //productSearch.CategoryID = 27;
            //productSearch.CityID = 4968;
            //productSearch.ColorIDs = string.Empty;
            //productSearch.DimensionIDs = string.Empty;
            //productSearch.IsScroll = true;
            //productSearch.Keyword = "ring";
            //productSearch.MaterialIDs = string.Empty;
            //productSearch.MaxPrice = 0;
            //productSearch.MinPrice = 0;
            //productSearch.PageIndex = 2;
            //productSearch.PageSize = 12;
            //productSearch.ProductID = 0;
            //productSearch.ShopID = 0;
            //productSearch.SizeIDs = string.Empty;
            //productSearch.SpecificationIDs = string.Empty;
            //productSearch.SpecificationValues = string.Empty;

            //ProductWithRefinementViewModel lp = new ProductWithRefinementViewModel();
            //lp = pl.GetProductList(productSearch);

            //ProductStockVarientViewModel lp = new ProductStockVarientViewModel();

            //lp = pl.GetProductStockList(productSearch);

            //ImageDisplay im = new ImageDisplay(System.Web.HttpContext.Current.Server);
            //FileInfo[] lFileInfo = im.LoadProductImages(4, "Default", string.Empty);
           //string thumb = im.LoadProductThumbnails(4, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
           
            //List<StockImages> lImages = new List<StockImages>();
            //lImages = im.GetStockImages(4, string.Empty);             

            //ShoppingCartInitialization sci = new ShoppingCartInitialization();
            //ShopProductVarientViewModelCollection lShoppingCartCollection = sci.GetCookie();

            /*order */
            //OrderViewModel lOrder = new OrderViewModel();
            //BusinessLogicLayer.CustomerOrder lCustomerOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
            //int i = lCustomerOrder.PlaceCustomerOrder(lOrder);

            /*product Techical Specification*/
            //IEnumerable<ProductTechnicalSpecificationViewModel> lTechSpec = p.GetTechnicalSpecifications(29);

            /*Pincode verification*/
            //bool isExist = pin.IsDeliverablePincode("110017");

            /*ProductView */
            //ProductViewBasicDetailsViewModels pd = new ProductViewBasicDetailsViewModels();
            //pd = p.GetBasicDetails(29);

            /*Customer Wishlist*/
            //CustomerWishlist lCW = new CustomerWishlist(System.Web.HttpContext.Current.Server);
            //lCW.SetWishlist(1, 10);
            //lCW.SetWishlist(1, 4);
            //lCW.RemoveFromWishlist(1, 4);
            //List<ProductStockDetailViewModel> lw = new List<ProductStockDetailViewModel>();
            //lw = lCW.GetWishlist(1); 


            /*Similler Products*/
            //RelatedProductsViewModel rel = new RelatedProductsViewModel();
            //SearchSimilarProductViewModel search = new SearchSimilarProductViewModel();
            //search.CategoryID = 24;
            //search.CityID = 1;
            //search.PageIndex = 1;
            //search.PageSize = 12;
            //search.ProductID = 29;
            //search.ShopID = 11;
            //rel = p.GetSimillarProducts(search);

            /*Stock  Offer  */
            //PriceAndOffers ofr = new PriceAndOffers(System.Web.HttpContext.Current.Server);
            //ProductOffersViewModel ofrs = new ProductOffersViewModel();
            //ofrs = ofr.GetStockOffers(4);

            /* Component Offer */
            //PriceAndOffers lOfr = new PriceAndOffers(System.Web.HttpContext.Current.Server);
            //Decimal lSavedRs = 0;
            //Decimal lSavedPer = 0;

            //Decimal lFinalAmount = 0;
            //lFinalAmount = lOfr.GetComponentOfferAmount(37, 11, Convert.ToDecimal(24442), ref lSavedRs, ref lSavedPer); 

           //return View("View1");
        }
	}
}