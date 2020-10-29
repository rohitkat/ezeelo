using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using Franchise.Models;
using Franchise.Models.ViewModel;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class DeliveryChargeViewModel
    {
        public string ShopOrderCode { get; set; }
        public Decimal DeliveryCharge { get; set; }
    }
    
    public class CustomerOrderDetailController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            try
            {
                customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
                customerCareSessionViewModel.Username = Session["USER_NAME"].ToString();
                //Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[SessionDetails]", "Can't assign Session Details..!" + Environment.NewLine + myEx.Message);
            }
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrderDetail/CanRead")]
        public ActionResult Index(long CustomerOrderID)
        {
            try
            {
                SessionDetails();
                int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                var customerorderdetails = db.CustomerOrderDetails.Include(c => c.CustomerOrder).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.Shop).Include(c => c.ShopStock).Where(x => x.CustomerOrderID == CustomerOrderID).OrderBy(x => x.ShopOrderCode).ToList();


                List<ProductByShopStockViewModel> ProductByShopStockViewModels = (from PR in db.Products
                                                                                  join ShopPr in db.ShopProducts on PR.ID equals ShopPr.ProductID
                                                                                  join ShopSt in db.ShopStocks on ShopPr.ID equals ShopSt.ShopProductID
                                                                                  join CuOrDt in db.CustomerOrderDetails on ShopSt.ID equals CuOrDt.ShopStockID
                                                                                  where (CuOrDt.CustomerOrderID == CustomerOrderID) && ShopPr.Shop.FranchiseID == franchiseID
                                                                                  select new ProductByShopStockViewModel
                                                                                  {
                                                                                      ShopStockID = ShopSt.ID,
                                                                                      ShopProductID = ShopPr.ID,
                                                                                      ProductID = PR.ID,
                                                                                      ProductName = PR.Name
                                                                                  }).ToList();

                ViewBag.ProductByShopStockViewModels = ProductByShopStockViewModels;

                List<DeliveryOrderDetailViewModel> DeliveryOrderDetailViewModels = (from DOD in db.DeliveryOrderDetails
                                                                                    join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
                                                                                    join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
                                                                                    join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
                                                                                    join SS in db.ShopStocks on COD.ShopStockID equals SS.ID
                                                                                    join SP in db.ShopProducts on SS.ShopProductID equals SP.ID
                                                                                    join S in db.Shops on SP.ShopID equals S.ID
                                                                                    where S.FranchiseID == franchiseID && (COD.CustomerOrderID == CustomerOrderID)
                                                                                    select new DeliveryOrderDetailViewModel
                                                                                    {
                                                                                        ID = DOD.ID,
                                                                                        ShopOrderCode = DOD.ShopOrderCode,
                                                                                        DeliveryPartnerID = DOD.DeliveryPartnerID,
                                                                                        DeliveryPartnerName = BD.Name,
                                                                                        DeliveryPartnerVM = DOD.DeliveryPartner
                                                                                    }).ToList();
                ViewBag.DeliveryOrderDetails = DeliveryOrderDetailViewModels;

                ViewBag.Units = db.Units.ToList();


                return View(customerorderdetails.ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderDetail][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderDetail][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrderDetail/CanRead")]
        public ActionResult IndexForCancelledOrderDetails(long CustomerOrderID)
        {
            try
            {
                SessionDetails();
                int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                var customerorderdetails = db.CustomerOrderDetails.Include(c => c.CustomerOrder).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.Shop).Include(c => c.ShopStock).Where(x => x.CustomerOrderID == CustomerOrderID).ToList();

                List<ProductByShopStockViewModel> ProductByShopStockViewModels = (from PR in db.Products
                                                                                  join ShopPr in db.ShopProducts on PR.ID equals ShopPr.ProductID
                                                                                  join ShopSt in db.ShopStocks on ShopPr.ID equals ShopSt.ShopProductID
                                                                                  join CuOrDt in db.CustomerOrderDetails on ShopSt.ID equals CuOrDt.ShopStockID
                                                                                  where (CuOrDt.CustomerOrderID == CustomerOrderID) && ShopPr.Shop.FranchiseID == franchiseID
                                                                                  select new ProductByShopStockViewModel
                                                                                  {
                                                                                      ShopStockID = ShopSt.ID,
                                                                                      ShopProductID = ShopPr.ID,
                                                                                      ProductID = PR.ID,
                                                                                      ProductName = PR.Name
                                                                                  }).ToList();

                ViewBag.ProductByShopStockViewModels = ProductByShopStockViewModels;

                List<DeliveryOrderDetailViewModel> DeliveryOrderDetailViewModels = (from DOD in db.DeliveryOrderDetails
                                                                                    join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
                                                                                    join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
                                                                                    join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
                                                                                    join SS in db.ShopStocks on COD.ShopStockID equals SS.ID
                                                                                    join SP in db.ShopProducts on SS.ShopProductID equals SP.ID
                                                                                    join S in db.Shops on SP.ShopID equals S.ID
                                                                                    where S.FranchiseID == franchiseID && (COD.CustomerOrderID == CustomerOrderID)
                                                                                    select new DeliveryOrderDetailViewModel
                                                                                    {
                                                                                        ID = DOD.ID,
                                                                                        ShopOrderCode = DOD.ShopOrderCode,
                                                                                        DeliveryPartnerID = DOD.DeliveryPartnerID,
                                                                                        DeliveryPartnerName = BD.Name,
                                                                                        DeliveryPartnerVM = DOD.DeliveryPartner
                                                                                    }).ToList();
                ViewBag.DeliveryOrderDetails = DeliveryOrderDetailViewModels;


                return View(customerorderdetails.ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderDetail][GET:IndexForCancelledOrderDetails]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderDetail][GET:IndexForCancelledOrderDetails]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrderDetail/CanRead")]
        // GET: /CustomerOrderDetail/Details/5
        public ActionResult Details(long? id)
        {
            try
            {
                SessionDetails();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CustomerOrderDetail customerorderdetail = db.CustomerOrderDetails.Find(id);
                if (customerorderdetail == null)
                {
                    return HttpNotFound();
                }
                return View(customerorderdetail);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderDetail][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderDetail][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        //// GET: /CustomerOrderDetail/Create
        //public ActionResult Create()
        //{
        //    ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode");
        //    ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
        //    ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
        //    ViewBag.ShopID = new SelectList(db.Shops, "ID", "Name");
        //    ViewBag.ShopStockID = new SelectList(db.ShopStocks, "ID", "NetworkIP");
        //    return View();
        //}

        //// POST: /CustomerOrderDetail/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include="ID,ShopOrderCode,ReferenceShopOrderCode,CustomerOrderID,ShopStockID,ShopID,Qty,OrderStatus,MRP,SaleRate,OfferPercent,OfferRs,IsInclusivOfTax,TotalAmount,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] CustomerOrderDetail customerorderdetail)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.CustomerOrderDetails.Add(customerorderdetail);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode", customerorderdetail.CustomerOrderID);
        //    ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorderdetail.CreateBy);
        //    ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorderdetail.ModifyBy);
        //    ViewBag.ShopID = new SelectList(db.Shops, "ID", "Name", customerorderdetail.ShopID);
        //    ViewBag.ShopStockID = new SelectList(db.ShopStocks, "ID", "NetworkIP", customerorderdetail.ShopStockID);
        //    return View(customerorderdetail);
        //}

        //// GET: /CustomerOrderDetail/Edit/5
        //public ActionResult Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CustomerOrderDetail customerorderdetail = db.CustomerOrderDetails.Find(id);
        //    if (customerorderdetail == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode", customerorderdetail.CustomerOrderID);
        //    ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorderdetail.CreateBy);
        //    ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorderdetail.ModifyBy);
        //    ViewBag.ShopID = new SelectList(db.Shops, "ID", "Name", customerorderdetail.ShopID);
        //    ViewBag.ShopStockID = new SelectList(db.ShopStocks, "ID", "NetworkIP", customerorderdetail.ShopStockID);
        //    return View(customerorderdetail);
        //}

        //// POST: /CustomerOrderDetail/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="ID,ShopOrderCode,ReferenceShopOrderCode,CustomerOrderID,ShopStockID,ShopID,Qty,OrderStatus,MRP,SaleRate,OfferPercent,OfferRs,IsInclusivOfTax,TotalAmount,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] CustomerOrderDetail customerorderdetail)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(customerorderdetail).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.CustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode", customerorderdetail.CustomerOrderID);
        //    ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorderdetail.CreateBy);
        //    ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorderdetail.ModifyBy);
        //    ViewBag.ShopID = new SelectList(db.Shops, "ID", "Name", customerorderdetail.ShopID);
        //    ViewBag.ShopStockID = new SelectList(db.ShopStocks, "ID", "NetworkIP", customerorderdetail.ShopStockID);
        //    return View(customerorderdetail);
        //}
        public ActionResult ForPrint(long CustomerOrderID)
        {
            var customerorderdetails = db.CustomerOrderDetails.Include(c => c.CustomerOrder).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.Shop).Include(c => c.ShopStock).Where(x => x.CustomerOrderID == CustomerOrderID).OrderByDescending(x => x.ID).ToList();
            //-- Start Tax Include on 31-march-2016 , By Avi Verma. 
            List<TaxOnOrderViewModel> lTaxOnOrderViewModels = (from cod in customerorderdetails
                                                               join too in db.TaxOnOrders on cod.ID equals too.CustomerOrderDetailID
                                                               join PrdTax in db.ProductTaxes on too.ProductTaxID equals PrdTax.ID
                                                               join taxMas in db.TaxationMasters on PrdTax.TaxID equals taxMas.ID
                                                               select new TaxOnOrderViewModel
                                                               {
                                                                   TaxOnOrderID = too.ID,
                                                                   CustomerOrderDetailID = cod.ID,
                                                                   ProductTaxID = PrdTax.ID,
                                                                   TaxAmount = too.Amount,
                                                                   TaxID = PrdTax.TaxID,
                                                                   TaxPrefix = taxMas.Prefix,
                                                                   TaxName = taxMas.Name
                                                               }).ToList();

            ViewBag.TaxOnOrderViewModels = lTaxOnOrderViewModels;
            //-- End Tax Include on 31-march-2016 , By Avi Verma. 

            decimal lTotalDeliveryCharge = 0;

            List<DeliveryChargeViewModel> lDeliveryChargeViewModel = (from dod in db.DeliveryOrderDetails
                                                                      join cod in db.CustomerOrderDetails on dod.ShopOrderCode equals cod.ShopOrderCode
                                                                      join co in db.CustomerOrders on cod.CustomerOrderID equals co.ID
                                                                      where co.ID == CustomerOrderID
                                                                      select new DeliveryChargeViewModel
                                                                      {
                                                                          ShopOrderCode = dod.ShopOrderCode,
                                                                          DeliveryCharge = dod.GandhibaghCharge,
                                                                      }).Distinct().ToList();


            decimal lDeliveryCharge = lDeliveryChargeViewModel.Sum(x => x.DeliveryCharge);
            ViewBag.DeliveryCharge = lDeliveryCharge;

            //List<ProductByShopStockViewModel> ProductByShopStockViewModels = (from PR in db.Products
            //                                                                  join ShopPr in db.ShopProducts on PR.ID equals ShopPr.ProductID
            //                                                                  join ShopSt in db.ShopStocks on ShopPr.ID equals ShopSt.ShopProductID
            //                                                                  join CuOrDt in db.CustomerOrderDetails on ShopSt.ID equals CuOrDt.ShopStockID
            //                                                                  select new ProductByShopStockViewModel
            //                                                                  {
            //                                                                      ShopStockID = ShopSt.ID,
            //                                                                      ShopProductID = ShopPr.ID,
            //                                                                      ProductID = PR.ID,
            //                                                                      ProductName = PR.Name
            //                                                                  }).ToList();

            //ViewBag.ProductByShopStockViewModels = ProductByShopStockViewModels;

            //List<DeliveryOrderDetailViewModel> DeliveryOrderDetailViewModels = (from DOD in db.DeliveryOrderDetails
            //                                                                    join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
            //                                                                    join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
            //                                                                    join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
            //                                                                    where (COD.CustomerOrderID == CustomerOrderID)
            //                                                                    select new DeliveryOrderDetailViewModel
            //                                                                    {
            //                                                                        ID = DOD.ID,
            //                                                                        ShopOrderCode = DOD.ShopOrderCode,
            //                                                                        DeliveryPartnerID = DOD.DeliveryPartnerID,
            //                                                                        DeliveryPartnerName = BD.Name,
            //                                                                        DeliveryPartnerVM = DOD.DeliveryPartner
            //                                                                    }).ToList();
            //ViewBag.DeliveryOrderDetails = DeliveryOrderDetailViewModels;

            //ViewBag.Units = db.Units.ToList();

            if (db.EarnDetails.FirstOrDefault(x => x.CustomerOrderID == CustomerOrderID) != null)
            {
                ViewBag.EarnedAmount = db.EarnDetails.FirstOrDefault(x => x.CustomerOrderID == CustomerOrderID).UsedAmount;
            }
            


            return View("_ForPrint", customerorderdetails.ToList());
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
