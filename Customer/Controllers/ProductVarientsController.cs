using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;

namespace Gandhibagh.Controllers
{
    public class ProductVarientsController : ApiController
    {
        // GET api/productvarients
        public IEnumerable<ShopProductVarientViewModel> GetProductVarientList(long productID)
        {
            ProductDetails prod = new ProductDetails(System.Web.HttpContext.Current.Server);
            return prod.GetStockVarients(productID, null);
        }
        public IEnumerable<ShopProductVarientViewModel> GetProductVarientList(long productID, long shopID)
        {
            ProductDetails prod = new ProductDetails(System.Web.HttpContext.Current.Server);
            return prod.GetStockVarients(productID,shopID,null);
        }

        // GET api/productvarients/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/productvarients
        public void Post([FromBody]string value)
        {
            
        }

        // PUT api/productvarients/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/productvarients/5
        public void Delete(int id)
        {
        }
    }
}
