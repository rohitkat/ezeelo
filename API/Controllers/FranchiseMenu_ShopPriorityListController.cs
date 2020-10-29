using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    /// <summary>
    /// Franchise Menu with Shop Priority
    /// Pradnyakar Badge 
    /// 28-01-2016
    /// </summary>
    public class FranchiseMenu_ShopPriorityListController : ApiController
    {
        // GET api/franchisemenu_shopprioritylist
        public List<FranchiseMenu_ShopPriorityListViewModel> Get(Int64 CityID, Int64 CategoryID, int? FranchiseID = null, bool IsSecondLevel = false, int? Version = null)////Added Int? FranchiseID for Multiple MCO and int? Version=null for New App
        {
            //-- For Differentiate Old and New APP --//
            if (Version == null)
            {FranchiseID = null;}

            List<FranchiseMenu_ShopPriorityListViewModel> objList = new List<FranchiseMenu_ShopPriorityListViewModel>();
            BusinessLogicLayer.FranchiseMenu_ShopPriorityList obj = new BusinessLogicLayer.FranchiseMenu_ShopPriorityList();
            if (IsSecondLevel)
            {
                objList = obj.selectFranchiseMenu_ShopPriorityList_SecondLevelWise(System.Web.HttpContext.Current.Server, CityID, CategoryID, FranchiseID);////Added  FranchiseID for Multiple MCO
            }
            else
            {
                objList = obj.selectFranchiseMenu_ShopPriorityList(System.Web.HttpContext.Current.Server, CityID, CategoryID, FranchiseID);////Added FranchiseID for Multiple MCO
            }
            return objList;

        }

        // GET api/franchisemenu_shopprioritylist/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/franchisemenu_shopprioritylist
        public void Post([FromBody]string value)
        {
        }

        // PUT api/franchisemenu_shopprioritylist/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/franchisemenu_shopprioritylist/5
        public void Delete(int id)
        {
        }
    }
}
