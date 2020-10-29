using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using PagedList;
using PagedList.Mvc;
using System.Collections;
using CRM.Models.ViewModel;
using System.Collections.Generic;
using System;
using CRM.Models;

namespace CRM.Controllers
{
    public class ProductSellersController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            customerCareSessionViewModel.Username = Session["UserName"].ToString();
            Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel);
        }



        [SessionExpire]
        public ActionResult Index(int? page, int? pID, int? cID, int? aID, string SearchString = "")
        {
            SessionDetails();
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;
            List<ProductSellersViewModel> lSellers = new List<ProductSellersViewModel>();
            //PriceAndOffers ofr = new PriceAndOffers(System.Web.HttpContext.Current.Server);

            if (cID == null && aID == null)
            {
                var lProductSellers = (
                                from sp in db.ShopProducts
                                join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                                join s in db.Shops on sp.ShopID equals s.ID
                                join p in db.Products on sp.ProductID equals p.ID
                                where sp.ProductID == pID 
                                //&& s.IsLive == true && s.IsActive == true && sp.IsActive == true && ss.IsActive == true
                                group ss by new { ss.ShopProductID, sp.ProductID, sp.ShopID, s.Address,s.Name, s.ContactPerson, s.Mobile, productname=p.Name ,ss.MRP,ss.RetailerRate} into temp
                                select new ProductSellersViewModel
                                {
                                    ProductID = temp.Key.ProductID,
                                    ProductName = temp.Key.productname,
                                    ShopID = temp.Key.ShopID,
                                    ShopName = temp.Key.Name,
                                    ShopAddress = temp.Key.Address,
                                    ContactPerson = temp.Key.ContactPerson,
                                    ContactPhoneNo = temp.Key.Mobile,
                                    ShopProductID = temp.Key.ShopProductID,
                                    MRP = temp.Key.MRP,
                                    SaleRate = temp.Key.RetailerRate

                                //select new ProductSellersViewModel
                                //{
                                //    ProductID = sp.ID,
                                //    ProductName = p.Name,
                                //    ShopID = sp.ShopID,
                                //    ShopName = s.Name,
                                //    ShopAddress = s.Address,
                                //    ContactPerson =  s.ContactPerson ,
                                //    ContactPhoneNo = s.Mobile,
                                //    ShopProductID = ss.ShopProductID,
                                //    MRP = ss.MRP,
                                //    SaleRate = ss.RetailerRate
                                }).OrderBy(x => x.SaleRate).ToList();


                //- distict records from list by shop, then, order by sell rate
                //lProductSellers = lProductSellers
                //                  .GroupBy(p => new { p.ShopID })
                //                  .Select(g => g.First())
                //                  .OrderBy(x => x.SaleRate).ToList();








                foreach (var item in lProductSellers)
                {
                    item.ShopStockID = db.ShopStocks.Select(x => new { x.ID, x.RetailerRate }).OrderByDescending(x => x.RetailerRate).ToList().Select(x => x.ID).Last();
                    // item.ProductOffer = ofr.GetStockOffers(item.ShopStockID);
                }
                lSellers = lProductSellers;
            }
            //else// if (cID > 0 && aID == 0)
            //{
            //    var lProductSellers = (
            //                  from sp in db.ShopProducts
            //                  join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
            //                  join s in db.Shops on sp.ShopID equals s.ID
            //                  where sp.ProductID == pID && s.IsLive == true && s.IsActive == true && sp.IsActive == true && ss.IsActive == true
            //                  group ss by new { ss.ShopProductID, sp.ProductID, sp.ShopID, s.Name } into temp
            //                  select new ProductSellersViewModel
            //                  {
            //                      ProductID = temp.Key.ProductID,
            //                      ShopID = temp.Key.ShopID,
            //                      ShopName = temp.Key.Name,
            //                      ShopProductID = temp.Key.ShopProductID,
            //                      MRP = temp.Min(x => x.MRP),
            //                      SaleRate = temp.Min(x => x.RetailerRate),

            //                  }).ToList();
            //    foreach (var item in lProductSellers)
            //    {
            //        item.ShopStockID = db.ShopStocks.Select(x => new { x.ID, x.RetailerRate }).OrderByDescending(x => x.RetailerRate).ToList().Select(x => x.ID).Last();
            //        //item.ProductOffer = ofr.GetStockOffers(item.ShopStockID);
            //    }
            //    lSellers = lProductSellers;
            //}
            //else if (aID > 0)
            //{
            //    var lProductSellers = (
            //                  from sp in db.ShopProducts
            //                  join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
            //                  join s in db.Shops on sp.ShopID equals s.ID
            //                  where sp.ProductID == pID && s.IsLive == true && s.IsActive == true && sp.IsActive == true && ss.IsActive == true
            //                  group ss by new { ss.ShopProductID, sp.ProductID, sp.ShopID, s.Name } into temp
            //                  select new ProductSellersViewModel
            //                  {
            //                      ProductID = temp.Key.ProductID,
            //                      ShopID = temp.Key.ShopID,
            //                      ShopName = temp.Key.Name,
            //                      ShopProductID = temp.Key.ShopProductID,
            //                      MRP = temp.Min(x => x.MRP),
            //                      SaleRate = temp.Min(x => x.RetailerRate),

            //                  }).ToList();
            //    foreach (var item in lProductSellers)
            //    {
            //        item.ShopStockID = db.ShopStocks.Select(x => new { x.ID, x.RetailerRate }).OrderByDescending(x => x.RetailerRate).ToList().Select(x => x.ID).Last();
            //        // item.ProductOffer =ofr.GetStockOffers(item.ShopStockID);
            //    }
            //    lSellers = lProductSellers;
            //}

            //return lSellers;
            return View(lSellers.ToPagedList(pageNumber, pageSize));
        }


        //[SessionExpire]
        ////
        //// GET: /ProductSellersViewModel/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //[SessionExpire]
        ////
        //// GET: /ProductSellersViewModel/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        
        ////
        //// POST: /ProductSellersViewModel/Create
        //[SessionExpire]
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        ////
        //// GET: /ProductSellersViewModel/Edit/5
        //[SessionExpire]
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        ////
        //// POST: /ProductSellersViewModel/Edit/5
        //[SessionExpire]
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
