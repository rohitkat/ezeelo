using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogicLayer;

namespace API.Controllers
{
    public class TrackSearchAPIController : ApiController
    {
        // GET api/tracksearchapi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/tracksearchapi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/tracksearchapi
        public object Post(List<TrackSearchParameters> paramValues)
        {
            object obj = new object();
            try
            {
                if (paramValues == null || paramValues.Count <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid paramters", data = string.Empty };
                }
                foreach (var item in paramValues)
                {
                    //-- For Differentiate Old and New APP --//
                    if (item.Version == null)
                    { item.FranchiseID = null; }

                    TrackSearchBusiness.InsertSearchDetails(item.UserLoginID, item.CategoryID, item.ShopID, item.ProductName, item.Lattitude, item.Longitude, item.DeviceType, item.DeviceID, item.City, item.IMEI_NO, item.FranchiseID);////added params int FranchiseID for Multiple MCO/Old App
                }
                obj = new { Success = 1, Message = "Successfully insert data.", data = string.Empty };
                //List<string> res = new List<string> { "Opration Perform1", "Opration Perform2" };
                //return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
            //if (paramValues.UserLoginID <= 0)
            //{
            //    object obj = new object();
            //    obj = new { HTTPStatusCode = "400", UserMessage = "Invalid paramters", ValidationError = "Invalid City" };
            //    return Request.CreateResponse(HttpStatusCode.BadRequest, obj);
            //}
        }
        // PUT api/tracksearchapi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/tracksearchapi/5
        public void Delete(int id)
        {
        }
    }

    public class TrackSearchParameters
    {
        public long UserLoginID { get; set; }
        public long CategoryID { get; set; }
        public long ShopID { get; set; }
        public string ProductName { get; set; }
        public string Lattitude { get; set; }
        public string Longitude { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public string City { get; set; }
        public int? FranchiseID { get; set; }////added by Ashish for Multiple MCO
        public string IMEI_NO { get; set; }
        public int? Version { get; set; }//// Added by Ashish For New App
    }
}
