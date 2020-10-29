using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models.Enum;
using ModelLayer.Models;
//using BusinessLogicLayer;
//using System.Reflection;

namespace Merchant.Controllers
{
    public class ConformController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();



        public ActionResult Temp() 
        {
            //List<CustomerOrder> objCustomerOrder = new List<CustomerOrder> 
            //{
            //    new CustomerOrder{},
            //    new CustomerOrder{}
            //};

            //List<CustomerOrderDetail> objCustomerOrderDetail = new List<CustomerOrderDetail> 
            //{
            //    new CustomerOrderDetail{ID=1,ShopOrderCode="GBMR20152605001",CustomerOrderID=1,ShopID=1,ShopStockID=1,Qty=2,OrderStatus= (int)OrderStatus.Placed,MRP=10},
            //    new CustomerOrderDetail{ID=1,ShopOrderCode="GBMR20152605002",CustomerOrderID=1,ShopID=1,ShopStockID=2,Qty=2,OrderStatus= (int)OrderStatus.Placed,MRP=10},
            //    new CustomerOrderDetail{ID=1,ShopOrderCode="GBMR20152605003",CustomerOrderID=1,ShopID=1,ShopStockID=3,Qty=2,OrderStatus= (int)OrderStatus.Placed,MRP=10},
            //    new CustomerOrderDetail{ID=1,ShopOrderCode="GBMR20152605004",CustomerOrderID=1,ShopID=2,ShopStockID=4,Qty=2,OrderStatus= (int)OrderStatus.Placed,MRP=10},
            //    new CustomerOrderDetail{ID=1,ShopOrderCode="GBMR20152605005",CustomerOrderID=1,ShopID=2,ShopStockID=5,Qty=2,OrderStatus= (int)OrderStatus.Placed,MRP=10},
            //};


            //var cust = objCustomerOrderDetail.Where(x => x.ShopID == 1).ToList();
                //db.CustomerOrderDetails.Where(x => x.ShopID == 1).ToList();
           
            //return View(cust);
            return View();
        }

        // GET: /Conform/
        public ActionResult Index()
        {
            
            var CustomerOrderDetailList = new List<CustomerOrderDetail>
            { new CustomerOrderDetail { ID=1,ShopOrderCode="1",CustomerOrderID=1,ShopStockID=1,Qty=12},
              new CustomerOrderDetail { ID=2,ShopOrderCode="2",CustomerOrderID=2,ShopStockID=2,Qty=13}
            };

            var CustomerOrderList = new List<CustomerOrder>();

            List<TrackOrderViewModel> listTrackOrder = new List<TrackOrderViewModel>();

            if (CustomerOrderDetailList != null)
            {
                foreach (CustomerOrderDetail customerOrderDetail in CustomerOrderDetailList)
                {
                    listTrackOrder.Add(new TrackOrderViewModel
                       {
                           ID = customerOrderDetail.ID,
                           //ShopOrderID = customerOrderDetail.ShopOrderCode,
                           //CustomerOrderId = customerOrderDetail.CustomerOrderID,
                           //ShopStockId = customerOrderDetail.ShopStockID,
                           //CheckBoxStatus = true,
                           //Qty = customerOrderDetail.Qty

                       });
                }

            }

            TrackOrderViewModelList trackOrderViewModelList = new TrackOrderViewModelList();
            trackOrderViewModelList.LtrackOrderViewModelList = listTrackOrder;
            return View(trackOrderViewModelList);
        }
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "Conform")]
        [ValidateAntiForgeryToken]
        public ActionResult ConformIndex(TrackOrderViewModelList trackOrderViewModelList)
        {
            if (trackOrderViewModelList.LtrackOrderViewModelList != null)
            {
                foreach (TrackOrderViewModel TrackOrderViewModel in trackOrderViewModelList.LtrackOrderViewModelList)
                {
                    if (TrackOrderViewModel.CheckBoxStatus)
                    {
                        TrackOrderViewModel.CheckBoxStatus = true;
                    }
                }
            }
            return RedirectToAction("Index");
            //return View("Thank You");
        }
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "Cancel")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelIndex(TrackOrderViewModelList trackOrderViewModelList)
        {
            if (trackOrderViewModelList.LtrackOrderViewModelList != null)
            {
                foreach (TrackOrderViewModel TrackOrderViewModel in trackOrderViewModelList.LtrackOrderViewModelList)
                {
                    if (TrackOrderViewModel.CheckBoxStatus)
                    {
                        TrackOrderViewModel.CheckBoxStatus = true;
                    }
                }
            }
            return RedirectToAction("Index");
            //return View("Thank You");
        }
        // GET: /Conform/Details/5

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
