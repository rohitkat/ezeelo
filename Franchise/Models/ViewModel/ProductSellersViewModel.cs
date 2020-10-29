using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franchise.Models.ViewModel
{
    public class ProductSellersViewModel
    {
        public string  LogoPath { get; set; }
        public  long ShopID { get; set; }
        public long ProductID { get; set; }
        public long ShopProductID { get; set; }  
        public long ShopStockID { get; set; }        
        public string ShopName { get; set; }
        public int OfferCount { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }

        public string ShopAddress { get; set; }
        public string ProductName { get; set; }
        public string ContactPerson { get; set; }

        public string ContactPhoneNo { get; set; }
        //public ProductOffersViewModel ProductOffer { get; set; }        

    }
}