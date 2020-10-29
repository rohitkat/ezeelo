using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using Gandhibagh.Models;

namespace Gandhibagh.Controllers
{
    public class CategoryWiseOffersController : Controller
    {
        //
        // GET: /Offers/
        public ActionResult Index()
        {
            URLCookie.SetCookies();
            long cityID = 0;
            int FranchiseID = 0;////added
            if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
            {
                cityID = Convert.ToInt16(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0]);
                FranchiseID = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added
            }
            Offers lOffers = new Offers();
            CategoryWiseOffersViewModel lCategoryWiseOffersViewModel = new CategoryWiseOffersViewModel();
            List<OfferCategoryList> lCategoryList = lOffers.GetOfferCategory(cityID,FranchiseID);////added FranchiseID
            List<OfferProducts> lOfferProducts = lOffers.GetOfferProducts(cityID, OfferStatus.AVAILABLEDEALS, 0, 1, 12, FranchiseID);////added FranchiseID for Mutiple MCO
            lCategoryWiseOffersViewModel.CatList = lCategoryList;
            lCategoryWiseOffersViewModel.prodList = lOfferProducts;
            lCategoryWiseOffersViewModel.flag = 1;
            ViewBag.offerStatus = 1; //Available deals
            return View(lCategoryWiseOffersViewModel);
        }

        public ActionResult GetOfferStatusWiseProducts(WebmethodParams myParam)
        {
            try
            {
                
            Offers lOffers = new Offers();
            //myParam.partialFlag = true;
            string str;
            //if (Enum.IsDefined(typeof(OfferStatus), myParam.offerStatus))
            //    str = ((OfferStatus)myParam.offerStatus).ToString();
            List<OfferProducts> lOfferProducts = new List<OfferProducts>();
            switch(myParam.offerStatus)
            {
                case 0: lOfferProducts = lOffers.GetOfferProducts(myParam.cityId, OfferStatus.MISSEDDEALS, myParam.categoryId, 1, 12, myParam.franchiseId);////added myParam.franchiseId for Mutiple MCO

                    break;
                case 1: lOfferProducts = lOffers.GetOfferProducts(myParam.cityId, OfferStatus.AVAILABLEDEALS, myParam.categoryId, 1, 12, myParam.franchiseId);////added myParam.franchiseId for Mutiple MCO
                    break;
                case 2: lOfferProducts = lOffers.GetOfferProducts(myParam.cityId, OfferStatus.UPCOMINGDEALS, myParam.categoryId, 1, 12, myParam.franchiseId);////added myParam.franchiseId for Mutiple MCO
                    break;
            }

            CategoryWiseOffersViewModel lCategoryWiseOffersViewModel = new CategoryWiseOffersViewModel();
            lCategoryWiseOffersViewModel.prodList = lOfferProducts;

                if(myParam.categoryId>0)
                {
                    //ViewBag.offerCatID = myParam.categoryId;
                    TempData["offerCatID"] = myParam.categoryId;
                }
                else
                {
                    //ViewBag.offerCatID = 0;
                    TempData["offerCatID"] = 0;
                }
            ViewBag.offerStatus = myParam.offerStatus;

            // Load Category wise partial view
            if (myParam.partialFlag == true)  
            {
                //This field used to assign differnt id to carousel
                lCategoryWiseOffersViewModel.flag = 2;
                return PartialView("_OfferCategoryWiseProducts", lCategoryWiseOffersViewModel); 
            }
            //Load Offer product (Which show all products) partial view
            else
            {
                lCategoryWiseOffersViewModel.flag = 1;
                return PartialView("_OfferProducts", lCategoryWiseOffersViewModel);
            }

            

            }
            catch (Exception ex)
            {

                throw;
            }
        }


        

	}

    public class WebmethodParams
    {
        public int cityId { get; set; }
        public int franchiseId { get; set; }////added
        public int offerStatus { get; set; }
        public int categoryId { get; set; }
        public bool partialFlag { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
}