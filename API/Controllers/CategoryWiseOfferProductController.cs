using API.Models;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class CategoryWiseOfferProductController : ApiController
    {
        // GET api/categorywiseofferproduct
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/categorywiseofferproduct/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/categorywiseofferproduct
        //[TokenVerification]
        //[ApiException]
        //[ValidateModel]
        public object Post(AppOfferParameters paramValues)
        {
            //-- For Differentiate Old and New APP --//
            if (paramValues.Version == null)
            { paramValues.FranchiseID = null; }

            //-- Passing FranchiseID in place of CityID --//
            if (paramValues.FranchiseID <= 0)////added CityID->FranchiseID
            {
                object obj = new object();
                obj = new { HTTPStatusCode = "400", UserMessage = "Invalid paramters", ValidationError = "Invalid Franchise" };////added "Invalid City"->"Invalid Franchise"
                return Request.CreateResponse(HttpStatusCode.BadRequest, obj);
            }

            Offers lOffers = new Offers();
            List<OfferProducts> lProductList = lOffers.GetOfferProducts(paramValues.CityID, paramValues.lOfferStatus, paramValues.catID, paramValues.pageIndex, paramValues.pageSize, paramValues.FranchiseID);////added FranchiseID for Multiple MCO

            return Request.CreateResponse(HttpStatusCode.OK, lProductList); 
            //BusinessLogicLayer.CustomerOrder lCustOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
            //int oprStatus = lCustOrder.CancelCustomerOrder(orderID, shopStockID, lCustLoginID);
            //object obj = new object();
            //if (oprStatus == 103)
            //    obj = new { HTTPStatusCode = "200", UserMessage = "Order has been cancelled successfully." };
            //if (oprStatus == 500)
            //    obj = new { HTTPStatusCode = "500", UserMessage = "Internal server error." };
            //if (oprStatus == 106)
            //    obj = new { HTTPStatusCode = "400", UserMessage = "Invalid request. The order you want to cancel is not present for given customer login ID." };

            //return obj;
        }



        // PUT api/categorywiseofferproduct/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/categorywiseofferproduct/5
        public void Delete(int id)
        {
        }
    }

    public class AppOfferParameters
    {
        public int CityID { get; set; }
        public OfferStatus lOfferStatus { get; set; } 
        public int catID { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int? FranchiseID { get; set; }  ////added by Ashish FranchiseID for Multiple MCO
        public int? Version { get; set; }//// Added by Ashish For New App
    }
}
