using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class GBDealsController : ApiController
    {
        // GET api/gbdeals
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/gbdeals/5
        public List<GBDealsViewModel> Get(int cityID, int? FranchiseID=null) ////added int? FranchiseID for multiple MCO
        {
         //string ss =  ConfigurationManager.AppSettings["sdds"];
        // string[] mykey = ConfigurationManager.AppSettings["GB_DEALS_"+cityID].Split(',');
            string[] mykey;
            if (FranchiseID != null )
            {
                //-- For New APP for multiple MCO --//
                mykey = ConfigurationManager.AppSettings["GB_DEALS_" + cityID + "_" + FranchiseID].Split(',');////added + "_" + FranchiseID for multiple MCO
            }
            else
            {
                //-- For Old App --//
                mykey = ConfigurationManager.AppSettings["GB_DEALS_" + cityID ].Split(',');
            }
         GBDealsViewModel lGBDealsViewModel = new GBDealsViewModel();

         List<GBDealsViewModel> listGBDealsViewModel = new List<GBDealsViewModel>();
         foreach (var item in mykey)
         {
             if(item!=string.Empty)
             {
                 string[] val= item.Split('/');
                 lGBDealsViewModel.CatID = Convert.ToInt32(val[0]);
                 lGBDealsViewModel.ShopID = Convert.ToInt64(val[1]);

                 BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);
                 //item.ImagePath = rcKey.CATEGORY_IMAGE_HTTP + "/" + cityId + "/" + item.FirstLevelCatID + "_ll" + ".png";
                 
                 if (FranchiseID != null )
                 {
                     //-- For New APP for multiple MCO --//
                     lGBDealsViewModel.DealThumbPath = rcKey.GB_DEALS_IMAGE + "/" + cityID + "/" + FranchiseID + "/" + Convert.ToInt32(val[0]) + ".png";  ////added  "/" + FranchiseID for multiple MCO
                 }
                 else
                 {
                     //-- For Old App --//
                     lGBDealsViewModel.DealThumbPath = rcKey.GB_DEALS_IMAGE + "/" + cityID + "/" + Convert.ToInt32(val[0]) + ".png"; 
                 }
                 listGBDealsViewModel.Add(lGBDealsViewModel);
             }
         }
         return listGBDealsViewModel;
        }

        // POST api/gbdeals
        public void Post([FromBody]string value)
        {
        }

        // PUT api/gbdeals/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/gbdeals/5
        public void Delete(int id)
        {
        }
    }
}
