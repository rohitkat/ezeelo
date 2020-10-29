using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    
    public class Select_Franchise_MenuController : ApiController
    {
        // GET api/select_franchise_menu
        //[Etag(300, 240, true)]

        public HttpResponseMessage Get(Int64 cityID, Int32? FranchiseID = null, int? Version = null)////Added Int? FranchiseID for Multiple MCO and int? Version=null for New App
        {
            //-- For Differentiate Old and New APP --//
            if (Version == null)
            { FranchiseID = null; }

            BusinessLogicLayer.Select_Franchise_Menu objMethode = new BusinessLogicLayer.Select_Franchise_Menu(System.Web.HttpContext.Current.Server);
            List<ModelLayer.Models.ViewModel.FranchiseMenuListViewModel> obj = new List<ModelLayer.Models.ViewModel.FranchiseMenuListViewModel>();
            obj = objMethode.selectFrnchiseMenu(cityID, FranchiseID);////added FranchiseID for Multiple MCO in same city
            return Request.CreateResponse(HttpStatusCode.OK, obj);

        }

        public HttpResponseMessage Get(Int64 cityID, long CategoryID, int? FranchiseID = null, int? Version = null)////added  Int32? FranchiseID for Multiple MCO in same city  and int? Version=null for New App
        {
            //-- For Differentiate Old and New APP --//
            if (Version == null)
            { FranchiseID = null; }

            BusinessLogicLayer.Select_Franchise_Menu objMethode = new BusinessLogicLayer.Select_Franchise_Menu(System.Web.HttpContext.Current.Server);
            List<ModelLayer.Models.ViewModel.FranchiseMenuListViewModel> obj = new List<ModelLayer.Models.ViewModel.FranchiseMenuListViewModel>();
            obj = objMethode.selectFrnchiseMenu(cityID, CategoryID, FranchiseID);////added FranchiseID for Multiple MCO in same city

            return Request.CreateResponse(HttpStatusCode.OK, obj);

        }

    }
}
