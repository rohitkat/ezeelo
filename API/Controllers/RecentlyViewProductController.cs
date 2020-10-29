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
    public class RecentlyViewProductController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        public object Get(int FranchiseId, long UserLoginId)
        {
            object obj = new object();
            try
            {
                if (FranchiseId == null || FranchiseId <= 0 || UserLoginId == null || UserLoginId <= 0)
                {
                    obj = new { Success = 0, Message = "Enter valid data", data = string.Empty };
                }
                RelatedProductsViewModel relatedProducts = new RelatedProductsViewModel();
                ProductDetails productDetails = new ProductDetails(System.Web.HttpContext.Current.Server);
                relatedProducts = productDetails.GetRecentlyViewProduct(UserLoginId, FranchiseId);
                if (relatedProducts != null)
                {
                    obj = new { Success = 1, Message = "Recently view product list found.", data = relatedProducts };
                }
                else
                {
                    obj = new { Success = 0, Message = "Recently view product list are not found.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
