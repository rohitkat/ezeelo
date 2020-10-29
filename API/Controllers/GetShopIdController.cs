using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class GetShopIdController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        public object Get(long FranchiseId)
        {
            object obj = new object();
            try
            {
                if (FranchiseId == null || FranchiseId <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid parameter.", data = string.Empty };
                }
                var ShopId = db.Shops.Where(x => x.FranchiseID == FranchiseId).Select(x => x.ID).FirstOrDefault();
                if (ShopId != null)
                    obj = new { Success = 1, Message = "Successfull", data = new { ShopId = ShopId } };
                else
                    obj = new { Success = 0, Message = "Something went wrong please try again.", data = new { ShopId = ShopId } };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
