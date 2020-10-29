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
    public class OffersController : ApiController
    {

        // GET api/offers/5
        public object Get(int cityId, int? franchiseId = null, int? Version = null)////added  int? FranchiseID for Multiple MCO/Old App and int? Version=null for New App
        {
            //-- For Differentiate Old and New APP --//
            object obj = new object();
            try
            {

                if (Version == null)
                { franchiseId = null; }

                if (cityId == null || cityId <= 0)
                {
                    return obj = new { Success = 0, Message = "Invalid paramters", data = string.Empty };
                }

                Offers lOffers = new Offers();
                List<OfferCategoryList> lCategoryList = new List<OfferCategoryList>();
                lCategoryList = lOffers.GetOfferCategory(cityId, franchiseId);////added franchiseId for Multiple MCO

                if (franchiseId != null)////For New APP
                {
                    if (ConfigurationManager.AppSettings["GB_DEALS_" + cityId + "_" + franchiseId] != null) ////added + "_" + franchiseId
                    {

                        //gb banners
                        string[] mykey = ConfigurationManager.AppSettings["GB_DEALS_" + cityId + "_" + franchiseId].Split(','); ////added + "_" + franchiseId
                        OfferCategoryList lGBDealsViewModel = new OfferCategoryList();
                        foreach (var item in mykey)
                        {
                            if (item != string.Empty)
                            {
                                string[] val = item.Split('/');
                                lGBDealsViewModel.FirstLevelCatID = Convert.ToInt32(val[0]);
                                lGBDealsViewModel.ShopID = Convert.ToInt32(val[1]);

                                BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);
                                lGBDealsViewModel.ImagePath = rcKey.GB_DEALS_IMAGE + "/" + cityId + "/" + franchiseId + "/" + Convert.ToInt32(val[0]) + ".png"; ////added  "/" + franchiseId +
                                lCategoryList.Add(lGBDealsViewModel);
                            }
                        }


                    }
                }
                else
                {
                    if (ConfigurationManager.AppSettings["GB_DEALS_" + cityId] != null) ////For Old APP
                    {

                        //gb banners
                        string[] mykey = ConfigurationManager.AppSettings["GB_DEALS_" + cityId].Split(',');
                        OfferCategoryList lGBDealsViewModel = new OfferCategoryList();
                        foreach (var item in mykey)
                        {
                            if (item != string.Empty)
                            {
                                string[] val = item.Split('/');
                                lGBDealsViewModel.FirstLevelCatID = Convert.ToInt32(val[0]);
                                lGBDealsViewModel.ShopID = Convert.ToInt32(val[1]);

                                BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);
                                lGBDealsViewModel.ImagePath = rcKey.GB_DEALS_IMAGE + "/" + cityId + "/" + Convert.ToInt32(val[0]) + ".png";
                                lCategoryList.Add(lGBDealsViewModel);
                            }
                        }

                    }
                }

                List<OfferProducts> lOfferProducts = lOffers.GetOfferProducts(cityId, OfferStatus.AVAILABLEDEALS, 0, 1, 12, franchiseId);////added FranchiseID for Mutiple MCO
                if (lCategoryList != null && lOfferProducts != null)
                {
                    obj = new { Success = 1, Message = "Offers found!!", data = new { CategoryList = lCategoryList, ProductList = lOfferProducts } };
                }
                else
                {
                    obj = new { Success = 0, Message = "Offer not found on product", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        // POST api/offers
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/offers/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/offers/5
        //public void Delete(int id)
        //{
        //}
    }
}
