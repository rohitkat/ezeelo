using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gandhibagh
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

            //Yashaswi For PartnerRequestModule 9/5/2018
            routes.MapRoute(
            name: "RequestForPartner",
            url: "{city}/{franchiseId}/RequestForPartner",
            defaults: new { controller = "PartnerRequest", action = "Create" }
            );

            /*Error Pages*/
            routes.MapRoute(
            name: "notfound",
            url: "{city}/{franchiseId}/not-found",
            defaults: new { controller = "Error", action = "NotFound" }
            );

            routes.MapRoute(
            name: "notexists",
            url: "{city}/{franchiseId}/not-exists",
            defaults: new { controller = "Error", action = "ErrorPage" }
            );


            /********************************************************************/

            /*All Shops*/
            routes.MapRoute(
            name: "AllShops",
            url: "{city}/{franchiseId}/top-stores",
            defaults: new { controller = "AllShops", action = "Index" }//done
            );
            /********************************************************************/

            ///Yashaswi new parmeter OldAreaId
            /* ChangeCity*/
            routes.MapRoute(
            name: "ChangeCity",
            //url: "{city}/{CityID}/{CityName}/chn-city", //old code hide
            url: "{city}/{CityID}/{CityName}/{FranchiseID}/{HelpLineNumber}/{OldAreaId}/chn-city",//added franchiseId by Ashish for multiple frnachise in same city
            // url: "{city}/{FranchiseID}/{HelpLineNumber}/chn-city",//added franchiseId by Ashish for multiple frnachise in same city
            defaults: new { controller = "ChangeCity", action = "Index" }//done
           );
            /********************************************************************/

            /*ShppersGuide*/
            routes.MapRoute(
            name: "ShppersGuide",
            url: "{city}/{franchiseId}/shppers-guide",
            defaults: new { controller = "home", action = "ShopperGuide" }//no change
            );
            /********************************************************************/

            /*PaymentGuide*/
            routes.MapRoute(
            name: "PaymentGuide",
            url: "{city}/{franchiseId}/payment-guide",
            defaults: new { controller = "home", action = "PaymentGuide" }//no change
            );
            /********************************************************************/

            /*CustomerGuide*/
            routes.MapRoute(
            name: "CustomerGuide",
            url: "{city}/{franchiseId}/customer-guide",
            defaults: new { controller = "home", action = "CustomerGuide" }//no change
            );
            /********************************************************************/

            /*MerchantGuide*/
            routes.MapRoute(
            name: "MerchantGuide",
            url: "{city}/{franchiseId}/merchant-guide",
            defaults: new { controller = "home", action = "MarchantGuide" }//no change
            );
            /********************************************************************/

            /*About Us*/
            routes.MapRoute(
            name: "AboutUs",
            url: "{city}/{franchiseId}/about-us",
            defaults: new { controller = "home", action = "About" }//no change
            );
            /********************************************************************/

            /*Contact Us*/
            routes.MapRoute(
            name: "ContactUs",
            url: "{city}/{franchiseId}/contact-us",
            defaults: new { controller = "home", action = "Contact" }//no change
            );
            /********************************************************************/

            /*DownloadApp*/
            routes.MapRoute(
            name: "DownloadApp",
            url: "{city}/{franchiseId}/mobileapp",
            defaults: new { controller = "home", action = "DownloadApp" }//no change
            );
            /********************************************************************/

            /*FAQ*/
            routes.MapRoute(
            name: "FAQ",
            url: "{city}/{franchiseId}/faq",
            defaults: new { controller = "home", action = "FAQ" }//no change
            );
            /********************************************************************/


            /*TERMS*/
            routes.MapRoute(
            name: "TERMS",
            url: "{city}/{franchiseId}/terms",
            defaults: new { controller = "home", action = "Terms" }//no change
            );
            /********************************************************************/

            /*FEEDBACK*/
            routes.MapRoute(
            name: "FEEDBACK",
            url: "{city}/{franchiseId}/feedback",
            defaults: new { controller = "home", action = "Feedback" }//no change
            );
            /********************************************************************/

            /*Refer*/
            routes.MapRoute(
            name: "Refer",
            url: "{city}/{franchiseId}/refer",
            defaults: new { controller = "ReferDetails", action = "Create" }//done
            );
            /********************************************************************/
            /*ReferDetails*/
            routes.MapRoute(
            name: "ReferDetails",
            url: "{city}/{franchiseId}/refer-details",
            defaults: new { controller = "ReferAndEarnReport", action = "Index" }//done
            );
            /********************************************************************/
            /*MobileIndex*/
            routes.MapRoute(
            name: "MobileIndex",
            url: "{city}/{franchiseId}/mobile",
            defaults: new { controller = "home", action = "MobileIndex" }//not found
            );
            /********************************************************************/
            /*Merchant Added by Shaili Khatri*/
            routes.MapRoute(
            name: "Merchant",
            url: "{city}/{franchiseId}/merchant/register",
            defaults: new { controller = "Merchant", action = "Create" }
            );
            routes.MapRoute(
            name: "MerchantKYC",
            url: "{city}/{franchiseId}/merchant/kyc/{id}",
            defaults: new { controller = "Merchant", action = "KYC", id = UrlParameter.Optional }
            );
            routes.MapRoute(
            name: "MerchantList",
            url: "{city}/{franchiseId}/merchant/list/{id}",
            defaults: new { controller = "Merchant", action = "List", id = UrlParameter.Optional }
            );
            routes.MapRoute(
           name: "MerchantKYCThankYou",
           url: "merchant/thankyou",
           defaults: new { controller = "merchant", action = "thankyou" }
           );
            routes.MapRoute(
           name: "MerchantLinkExpiry",
           url: "merchant/linkexpiry",
           defaults: new { controller = "merchant", action = "LinkExpiry" }
           );

            routes.MapRoute(
              name: "MerchantPreview",
              url: "merchant/Preview",
              defaults: new { controller = "merchant", action = "Preview", id = UrlParameter.Optional }
              );
            /********************************************************************/

            routes.MapRoute(
            name: "Product1",
            url: "{city}/{franchiseId}/products",
            defaults: new { controller = "Product", action = "Products" }//done
            );

            routes.MapRoute(
            name: "Product3",
            url: "{city}/{franchiseId}/{item}/{shopID}/products",
            defaults: new { controller = "Product", action = "Products", item = UrlParameter.Optional, shopID = UrlParameter.Optional }//done
            );

            routes.MapRoute(
            name: "ProductsByCategory",
            url: "{city}/{franchiseId}/{item}/{parentCategoryId}/cat-products",
            defaults: new { controller = "Product", action = "Products", item = UrlParameter.Optional, parentCategoryId = UrlParameter.Optional }//done
            );

            routes.MapRoute(
            name: "ProductsByCategoryFromMenu",
            url: "{city}/{franchiseId}/{itemName}/{parentCategoryId}/category-products",
            defaults: new { controller = "Product", action = "ProductsByCategory", itemName = UrlParameter.Optional, parentCategoryId = UrlParameter.Optional }//done in products.cshtml
            );

            //for angular view
            routes.MapRoute(
            name: "Managed",
            url: "{city}/{franchiseId}/{CatName}/{ParentCatID}/{Level}/{cityId}/mmp",
            defaults: new { controller = "Managed", action = "Index" }//done doubt
            );


            //routes.MapRoute(
            //name: "ShopSearch",
            //url: "{city}/shop/{shopName}",
            //defaults: new { controller = "Product", action = "ShopSearch", shopName = UrlParameter.Optional }
            //);

            routes.MapRoute(
           name: "ShopSearch",
           url: "{city}/{franchiseId}/shop",
           defaults: new { controller = "Product", action = "ShopSearch" }//done
           );

            routes.MapRoute(
            name: "ShopSearch1",
            url: "{city}/{franchiseId}/{shopID}/{item}/shop",
            defaults: new { controller = "Product", action = "Products", shopID = UrlParameter.Optional, item = string.Empty }//done
            );

            routes.MapRoute(
            name: "ShopSearch2",
            url: "{city}/{franchiseId}/{shopName}/{shopID}/shop-products",
            defaults: new { controller = "Product", action = "Products", shopID = UrlParameter.Optional, shopName = UrlParameter.Optional }//done
            );

            routes.MapRoute(
            name: "ShopProducts",
            url: "{city}/{franchiseId}/{shopName}/{catName}/{shopID}/{selectedCategoryId}/shop-cat-products",
            defaults: new { controller = "Product", action = "Products", shopName = UrlParameter.Optional, selectedCategoryId = UrlParameter.Optional }//done
            );

            routes.MapRoute(
            name: "ShopProducts2",
            url: "{city}/{franchiseId}/{shopName}/{shopID}/{selectedCategoryId}/shop-cat-products",
            defaults: new { controller = "Product", action = "Products", shopName = UrlParameter.Optional, selectedCategoryId = UrlParameter.Optional }//done
            );

            routes.MapRoute(
            name: "ShopProducts3",
            url: "{city}/{franchiseId}/{shopName}/{shopID}/{selectedCategoryId}/shop-selected-cat-products",
            defaults: new { controller = "Product", action = "Products", shopName = UrlParameter.Optional }//done
            );

            /********************************************************************/


            /*Login*/
            routes.MapRoute(
            name: "Login",
            url: "{city}/{franchiseId}/login",
            defaults: new { controller = "Login", action = "login", Phone = UrlParameter.Optional, ReferalCode = UrlParameter.Optional, Name = UrlParameter.Optional, Email = UrlParameter.Optional }//done
            );
            /********************************************************************/

            /*CustomerOrder*/
            routes.MapRoute(
            name: "CustomerOrder/myorders",
            url: "{city}/{franchiseId}/cust-o/my-order",
            defaults: new { controller = "CustomerOrder", action = "myorders" }//done
            );

            routes.MapRoute(
            name: "CustomerOrder/OrderStatus",
            url: "{city}/{franchiseId}/cust-o/order-s",
            defaults: new { controller = "CustomerOrder", action = "OrderStatus" }//done
            );

            routes.MapRoute(
            name: "CustomerOrder/OrderHistory",
            url: "{city}/{franchiseId}/{orderId}/{uid}/cust-o/order-h",
            defaults: new { controller = "CustomerOrder", action = "OrderHistory", orderId = UrlParameter.Optional, uid = UrlParameter.Optional }//done
            );

            routes.MapRoute(
            name: "CancelOrder",
            url: "{city}/{franchiseId}/{orderID}/{shopStockID}/cust-o/cancel-order",
            defaults: new { controller = "CustomerOrder", action = "CancelOrder", shopStockID = UrlParameter.Optional }//done
            );

            /********************************************************************/

            /* WishList */
            routes.MapRoute(
            name: "MyWishList",
            url: "{city}/{franchiseId}/my-wishlist",
            defaults: new { controller = "WishList", action = "Index" }//no change,change in Views
            );

            /********************************************************************/

            /* UserLogin */
            routes.MapRoute(
            name: "ChangePwd",
            url: "{city}/{franchiseId}/{pUserLoginID}/chn-pwd",
            defaults: new { controller = "UserLogin", action = "Edit", pUserLoginID = UrlParameter.Optional }//no change,change in Views
            );

            /********************************************************************/

            /* SubscriptionPlanPurchasedBy */
            routes.MapRoute(
            name: "SubPlanPurchasedBy",
            url: "{city}/{franchiseId}/sub-plan-purchased-by",
            defaults: new { controller = "SubscriptionPlanPurchasedBy", action = "Index" }//no change
            );

            /********************************************************************/

            /* SubscriptionPlanHistory */
            routes.MapRoute(
            name: "SubPlanHistory",
            url: "{city}/{franchiseId}/sub-plan-history",
            defaults: new { controller = "SubscriptionPlanHistory", action = "Index" }//no change
            );

            /********************************************************************/

            /*PreviewItem*/
            //routes.MapRoute(
            //name: "preview-product",
            //url: "{city}/{itemID}/preview",
            //defaults: new { controller = "PreviewItem", action = "Index", itemID = UrlParameter.Optional }
            //);

            routes.MapRoute(
            name: "pre-prod-name",
            url: "{city}/{franchiseId}/{itemName}/{itemID}/preview",
            defaults: new { controller = "PreviewItem", action = "Index" }//done
            );

            routes.MapRoute(
            name: "preview-shop-product",
            url: "{city}/{franchiseId}/{itemName}/{itemId}/{shopID}/shop-preview",
            defaults: new { controller = "PreviewItem", action = "Index" }//done
            );

            routes.MapRoute(
             name: "preview-stock",
             url: "{city}/{franchiseId}/{itemName}/{itemId}/{shopStockID}/{shopID}/stock-preview",
             defaults: new { controller = "PreviewItem", action = "Index" }//done
             );

            //routes.MapRoute(
            //name: "preview",
            //url: "{city}/{itemId}/{shopID}/{shopStockID}/preview",
            //defaults: new { controller = "PreviewItem", action = "Index", itemId = UrlParameter.Optional, shopID = UrlParameter.Optional, shopStockID = UrlParameter.Optional }
            //);


            routes.MapRoute(
            name: "preview-pin-verify",
            url: "{city}/{franchiseId}/{pincode}/preview-pin-verify",
            defaults: new { controller = "PreviewItem", action = "CheckDeliveryPincode", pincode = UrlParameter.Optional }//done
            );

            /********************************************************************/

            routes.MapRoute(
                         name: "express-buy",
                         url: "{city}/{franchiseId}/{ShopStockID}/{itemID}/{productQuantity}/{couponCode}/{couponAmount}/{couponPercent}/express-buy",
                         defaults: new { controller = "PreviewItem", action = "ExpressBuy", ShopStockID = UrlParameter.Optional, itemID = UrlParameter.Optional, productQuantity = UrlParameter.Optional, couponCode = UrlParameter.Optional, couponAmount = UrlParameter.Optional, couponPercent = UrlParameter.Optional }//done
                        );

            /*Feedback*/
            routes.MapRoute(
                         name: "Product_rating",
                         url: "{city}/{franchiseId}/{ID}/p-rating",
                         defaults: new { controller = "Feedback", action = "ProductRating", ID = UrlParameter.Optional }//done
                        );

            routes.MapRoute(
                         name: "shop_rating",
                         url: "{city}/{franchiseId}/{ID}/{prodId}/s-rating",
                         defaults: new { controller = "Feedback", action = "ShopRating", ID = UrlParameter.Optional, prodId = UrlParameter.Optional }//done
                        );
            /********************************************************************/

            /*Shopping Cart */
            routes.MapRoute(
            name: "cart",
            url: "{city}/{franchiseId}/cart",
            defaults: new { controller = "ShoppingCart", action = "Index" }//done
            );

            routes.MapRoute(
            name: "cart-pin-verify",
            url: "{city}/{franchiseId}/{txtPinCode}/cart-pin-chk",
            defaults: new { controller = "ShoppingCart", action = "VerifyPincode", txtPinCode = UrlParameter.Optional }//done
            );

            routes.MapRoute(
            name: "cart-coupon-verify",
            url: "{city}/{franchiseId}/{txtCouponCode}/cart-coupon-chk",
            defaults: new { controller = "ShoppingCart", action = "VerifyCouponcode", txtCouponCode = UrlParameter.Optional }//done
            );

            routes.MapRoute(
            name: "CHN-QTY-CART",
            url: "{city}/{franchiseId}/{shopStockID}/{txtQuantity}/cart-chn-qty",
            defaults: new { controller = "ShoppingCart", action = "ChangeCartItemQuantity" }//done
            );

            routes.MapRoute(
            name: "DELETE-CART-ITEM",
            url: "{city}/{franchiseId}/{shopStockID}/cart",
            defaults: new { controller = "ShoppingCart", action = "DeleteItemFromCart" }//done
            );

            //- Change made by Avi Verma and Pradnyakar Sir. Date :- 02-July-2016.
            //- Reason :- SEO PPC required static URL for getting results.
            //- As said by Reena ma'am, Bhavan and Bhusan.

            //routes.MapRoute(
            //name: "PurchaseComplete",
            //url: "{city}/{OrderId}/oder-placed",
            //defaults: new { controller = "CustomerOrder", action = "PurchaseComplete" }
            //);
            routes.MapRoute(
            name: "PurchaseFailure",
            url: "{city}/{franchiseId}/order-failed",
            defaults: new { controller = "CustomerOrder", action = "PurchaseFailure" }//done
            );

            routes.MapRoute(
            name: "PurchaseComplete",
            url: "{city}/{franchiseId}/order-placed",
            defaults: new { controller = "CustomerOrder", action = "PurchaseComplete" }//done
            );
            /********************************************************************/

            /*PaymentProcess*/

            routes.MapRoute(
            name: "PaymentProcess",
            url: "{city}/{franchiseId}/payment-process",
            defaults: new { controller = "PaymentProcess", action = "CustomerPaymentProcess" }//done 
            );

            routes.MapRoute(
            name: "PP-Login",
            url: "{city}/{franchiseId}/payment-process-login",
            defaults: new { controller = "Login", action = "login" }//done
            );

            routes.MapRoute(
            name: "ThankyouCCAvenue",
            url: "{city}/{franchiseId}/thankyou-ccavenue",
            defaults: new { controller = "PaymentProcess", action = "ThankyouCCAvenue" }//done
            );

            /*********************************************************************/
            /********************************************************************/

            /*Compare Product*/

            routes.MapRoute(
            name: "CompareProduct",
            url: "{city}/{franchiseId}/{categoryName}/{categoryID}/compare-product",
            defaults: new { controller = "ProductCompare", action = "Index", categoryName = UrlParameter.Optional, categoryID = UrlParameter.Optional }//done
            );

            routes.MapRoute(
            name: "CompareProduct1",
            url: "{city}/{franchiseId}/compare-product",
            defaults: new { controller = "ProductCompare", action = "Index" }//done
            );


            /*********************************************************************/
            /* Home*/
            routes.MapRoute(
            name: "About-Us",
            url: "{city}/{franchiseId}/about",
            defaults: new { controller = "home", action = "About" }//no change 
            );

            routes.MapRoute(
           name: "Exclusive",
           url: "{city}/{franchiseId}/exclusive-product",
           defaults: new { controller = "home", action = "Exclusive" }//no change 
           );

            routes.MapRoute(
            name: "Nursery",
            url: "{city}/{franchiseId}/nursery",
            defaults: new { controller = "home", action = "Nursery" }//no change 
            );

            routes.MapRoute(
                    name: "Holioffers",
                    url: "{city}/{franchiseId}/holi-offers",
                    defaults: new { controller = "home", action = "Holioffers" }//no change 
                    );

            /*************************************************************************/

            /* Home - Category*/

            //routes.MapRoute(
            //name: "CategoryPages",
            ////url: "{city}/{catName}/{catID}/category",//hide
            //url: "{city}/{franchiseId}/{catName}/{catID}/category",//added
            //defaults: new { controller = "home", action = "CategoryPage" }//done
            //);

            //routes.MapRoute(
            //name: "CategoryPages",
            ////url: "{city}/{catName}/{catID}/category",//hide
            //url: "{city}/{franchiseId}/{catName}/{catID}/{secLvlCatID}/category",//added
            //defaults: new { controller = "home", action = "CategoryPage", secLvlCatID = UrlParameter.Optional }//done
            //);

            routes.MapRoute(
            name: "CategoryPages",
            url: "{city}/{franchiseId}/{catName}/{catID}/category",
            defaults: new { controller = "home", action = "CategoryPage" }
            );

            routes.MapRoute(
            name: "SecondLevelCategoryPages",
            url: "{city}/{franchiseId}/{FirstLvtName}/{catName}/{catID}/{secLvlCatID}/category",
            defaults: new { controller = "home", action = "CategoryPage", secLvlCatID = UrlParameter.Optional }

            );


            /********************************Offer page************************************/

            /*Offers*/
            routes.MapRoute(
            name: "Offers",
            url: "{city}/{franchiseId}/offers",
            defaults: new { controller = "CategoryWiseOffers", action = "Index" }//done
            );

            routes.MapRoute(
            name: "HotDeals",
            url: "{city}/{franchiseId}/hot-deals",
            defaults: new { controller = "home", action = "OfferPage" }//no change
            );

            routes.MapRoute(
            name: "Festiv",
            url: "{city}/{franchiseId}/festiv",
            defaults: new { controller = "home", action = "Festiv" }//no change
            );

            routes.MapRoute(
            name: "DealOfDay",
            url: "{city}/{franchiseId}/deal-Of-the-day",
            defaults: new { controller = "home", action = "DealOfDay" }//no change
            );

            routes.MapRoute(
            name: "FourtyEightHour",
            url: "{city}/{franchiseId}/fourty-eight-hour",
            defaults: new { controller = "home", action = "FourtyEightHour" }//no change
            );
            /********************************************************************/

            /*Customer*/
            routes.MapRoute(
            name: "cust-reg",
            url: "{city}/{franchiseId}/register",
            defaults: new { controller = "Customer", action = "create" }//done
            );
            /*Edit Customer Details*/
            routes.MapRoute(
           name: "my-profile",
           url: "{city}/{franchiseId}/{id}/my-profile",
           defaults: new { controller = "Customer", action = "Edit" }//done
           );

            /*Logout*/
            routes.MapRoute(
            name: "Logout",
            url: "{city}/{franchiseId}/logout",
            defaults: new { controller = "Customer", action = "Logout" }//done
            );
            /********************************************************************/

            /*SiteMapMaker*/
            routes.MapRoute(
             name: "SiteMapMaker",
             url: "{city}/{franchiseId}/site-map-generator",
             defaults: new { controller = "SiteMapMaker", action = "Create" }//IP
             );



            /******************Policy***************/
            routes.MapRoute(
           name: "Policies",
           url: "{city}/{franchiseId}/policies",
           defaults: new { controller = "Policy", action = "Policies" }//no change
             );
            /********************************************************************/

            /******************Career***************/
            routes.MapRoute(
           name: "Career",
           url: "{city}/{franchiseId}/join-us",
           defaults: new { controller = "Home", action = "Career" }//no change
             );

            /********************************************************************/

            /*Career Section*/

            routes.MapRoute(
            name: "CareerApplyNow",
            url: "{city}/{franchiseId}/{id}/career",
            defaults: new { controller = "CareerApplyNow", action = "Index" }//done
            );
            /********************************************************************/




            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
            /**************My profile****************************/
            /*tell a friend*/
            routes.MapRoute(
          name: "TellAFriend",
          url: "{city}/{franchiseId}/tell-a-friend",
          defaults: new { controller = "TellAFriend", action = "Index" }//done
          );
            /*********************************************/


            /*default*/

            //routes.MapRoute(
            //name: "Default1",
            //url: "{city}/{controller}/{action}/{name}/{id}",
            //defaults: new { controller = "home", action = "Index", name = UrlParameter.Optional, id = UrlParameter.Optional }
            //);

            routes.MapRoute(
            name: "Home",
            //url: "{city}",//hide
            url: "{city}/{franchiseId}",//added franchiseId by Ashish for multiple frnachise in same city
            defaults: new { controller = "home", action = "Index" }//done
            );

            routes.MapRoute(
            name: "Default",
            //url: "{city}/{controller}/{action}/{id}",//hide
            url: "{city}/{franchiseId}/{controller}/{action}/{id}",//added franchiseId by Ashish for multiple frnachise in same city
            //defaults: new { controller = "home", action = "Index", id = UrlParameter.Optional, city = UrlParameter.Optional}////hide
            defaults: new { controller = "home", action = "Index", id = UrlParameter.Optional, city = UrlParameter.Optional, franchiseId = UrlParameter.Optional }////added done
            );



            /********************************************************************/
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}#",
            //    defaults: new { controller = "Managed" }
            //);
            /********************************************************************/
        }
    }
}
