using API.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class PremiumShopDisplayController : ApiController
    {
        // GET api/premiumshopdisplay
        [ApiException] 
        public List<FranchiseMenu_ShopPriorityListViewModel> Get(int ShopID, Int64 CategoryID)
        {
            object objReturn = new object();
            List<FranchiseMenu_ShopPriorityListViewModel> objList = new List<FranchiseMenu_ShopPriorityListViewModel>();

            BusinessLogicLayer.FranchiseMenu_ShopPriorityList obj = new BusinessLogicLayer.FranchiseMenu_ShopPriorityList();
            objList = obj.FranchiseMenu_ShopPriorityList_SecondLevelWise_FirstLevelCategory(System.Web.HttpContext.Current.Server, ShopID, CategoryID);

            return objList;
        }

        // GET api/premiumshopdisplay/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/premiumshopdisplay
        public void Post([FromBody]string value)
        {
        }

        // PUT api/premiumshopdisplay/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/premiumshopdisplay/5
        public void Delete(int id)
        {
        }
    }
}
