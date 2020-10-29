using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using PagedList;
using PagedList.Mvc;
using DeliveryPartner.Models.ViewModel;
using System.Collections;
using DeliveryPartner.Models;

namespace DeliveryPartner.Controllers
{
  
    public class DashboardController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private DeliveryPartnerSessionViewModel deliveryPartnerSessionViewModel = new DeliveryPartnerSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            deliveryPartnerSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            deliveryPartnerSessionViewModel.Username = Session["UserName"].ToString();
            UserLogin lUserLogin = db.UserLogins.SingleOrDefault(x => x.ID == deliveryPartnerSessionViewModel.UserLoginID && x.IsLocked == false);
            if (lUserLogin == null)
            {
                if (Session["ID"] != null)
                {
                    Session["ID"] = null;
                }
                TempData["ServerMsg"] = "You Account is In-Active. Please contact Admin";
                Response.Redirect(System.Web.Configuration.WebConfigurationManager.AppSettings["UrlForInvalidDeliveryPartner"]);
            }
            else if (!Common.Common.GetAllLoginDetailFromSession(ref deliveryPartnerSessionViewModel))
            {
                if (Session["ID"] != null)
                { 
                    Session["ID"] = null;
                }
                TempData["ServerMsg"] = "You are not a Delivery Partner";
                Response.Redirect(System.Web.Configuration.WebConfigurationManager.AppSettings["UrlForInvalidDeliveryPartner"]);
            }
        }

        [SessionExpire]
        public ActionResult Index()
        {
            SessionDetails();
            DashBoardViewModel lDashBoardViewModel = new DashBoardViewModel();
            lDashBoardViewModel.ID = deliveryPartnerSessionViewModel.DeliveryPartnerID;
            lDashBoardViewModel.Name = deliveryPartnerSessionViewModel.Username;
            string lFromDate; string lToDate;

            var deliveryorderdetails = (from DOD in db.DeliveryOrderDetails
                                        join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
                                        //// join EA in db.EmployeeAssignment on DOD.ShopOrderCode equals EA.ShopOrderCode into LOJ //Add by Ashish //Hide EPOD from Ashish for Live
                                        ////  from EA in LOJ.DefaultIfEmpty()//-- For Left Outer Join --// //Add by Ashish  //Hide EPOD from Ashish for Live
                                        where DOD.ShopOrderCode == COD.ShopOrderCode &&
                                        DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID
                                        select new DeliveryIndexViewModel
                                        {
                                            ID = DOD.ID,
                                            DeliveryPartnerID = DOD.DeliveryPartnerID,
                                            ShopOrderCode = DOD.ShopOrderCode,
                                            Weight = DOD.Weight,
                                            OrderAmount = DOD.OrderAmount,
                                            DeliveryCharge = DOD.DeliveryCharge,
                                            GandhibaghCharge = DOD.GandhibaghCharge,
                                            DeliveryType = DOD.DeliveryType,
                                            IsMyPincode = DOD.IsMyPincode,
                                            IsActive = DOD.IsActive,
                                            CreateDate = DOD.CreateDate,
                                            CreateBy = DOD.CreateBy,
                                            ModifyDate = DOD.ModifyDate,
                                            ModifyBy = DOD.ModifyBy,
                                            NetworkIP = DOD.NetworkIP,
                                            DeviceType = DOD.DeviceType,
                                            DeviceID = DOD.DeviceID,


                                            //----------------------------- Extra added -//
                                            OrderStatus = COD.OrderStatus,
                                            ////Assignment = EA.EmployeeCode == null ? "UNASSIGN" : "ASSIGN" //Add by Ashish //Hide EPOD from Ashish for Live
                                        }).Distinct().ToList();


            //------------------------------------Code for get count of all order status------------------------------------//
            int lPendingOrder = deliveryorderdetails.Count(x => x.OrderStatus >= (int)Common.Constant.ORDER_STATUS.PACKED && x.OrderStatus < (int)Common.Constant.ORDER_STATUS.DELIVERED);
            int lDeliverdOrder = deliveryorderdetails.Count(x => x.OrderStatus == (int)Common.Constant.ORDER_STATUS.DELIVERED);
            int lReturnedOrder = deliveryorderdetails.Count(x => x.OrderStatus == (int)Common.Constant.ORDER_STATUS.RETURNED);
            int lCancelledOrder = deliveryorderdetails.Count(x => x.OrderStatus == (int)Common.Constant.ORDER_STATUS.CANCELLED);

            //-- Add by Ashish --//
            //Hide EPOD from Ashish for Live
           /* int lAssignOrder = deliveryorderdetails.Count(x => x.OrderStatus >= (int)Common.Constant.ORDER_STATUS.PACKED && x.OrderStatus < (int)Common.Constant.ORDER_STATUS.DELIVERED && x.Assignment == "ASSIGN");
            int lUnassignOrder = deliveryorderdetails.Count(x => x.OrderStatus >= (int)Common.Constant.ORDER_STATUS.PACKED && x.OrderStatus < (int)Common.Constant.ORDER_STATUS.DELIVERED && x.Assignment == "UNASSIGN");
            */
            //-- End Add --//

            Dictionary<string, int> lTodayStatus = new Dictionary<string, int>();
            lTodayStatus.Add("PendingOrder", lPendingOrder);
            lTodayStatus.Add("OrderDelivered", lDeliverdOrder);
            lTodayStatus.Add("OrderReturned", lReturnedOrder);
            lTodayStatus.Add("OrderCancelled", lCancelledOrder);
            //-- Add by Ashish --//
            //Hide EPOD from Ashish for Live
            /*lTodayStatus.Add("OrderAssigned", lAssignOrder);
            lTodayStatus.Add("OrderUnassigned", lUnassignOrder);*/
            //-- End Add --//
            lDashBoardViewModel.TodayStatus = lTodayStatus;
            //---------------------------------------------End of Code for get count of all order status---------------------//

            //------------------------------------Code for get count of Donot chart------------------------------------//
            int lPacked = deliveryorderdetails.Count(x => x.OrderStatus == (int)Common.Constant.ORDER_STATUS.PACKED);
            //-- Add by Ashish --//
            //Hide EPOD from Ashish for Live
            /*int lAssigned = deliveryorderdetails.Count(x => x.OrderStatus >= (int)Common.Constant.ORDER_STATUS.PACKED && x.OrderStatus < (int)Common.Constant.ORDER_STATUS.DELIVERED && x.Assignment == "ASSIGN");
            int lUnassigned = deliveryorderdetails.Count(x => x.OrderStatus >= (int)Common.Constant.ORDER_STATUS.PACKED && x.OrderStatus < (int)Common.Constant.ORDER_STATUS.DELIVERED && x.Assignment == "UNASSIGN");
            */
            //-- End Add --//
            int lDispatchedFromShop = deliveryorderdetails.Count(x => x.OrderStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_SHOP);
            int lInGodown = deliveryorderdetails.Count(x => x.OrderStatus == (int)Common.Constant.ORDER_STATUS.IN_GODOWN);
            int lDispatchedFromGodown = deliveryorderdetails.Count(x => x.OrderStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_GODOWN);
            int lDelivered = deliveryorderdetails.Count(x => x.OrderStatus == (int)Common.Constant.ORDER_STATUS.DELIVERED);
            int lReturned = deliveryorderdetails.Count(x => x.OrderStatus == (int)Common.Constant.ORDER_STATUS.RETURNED);
            int lCancelled = deliveryorderdetails.Count(x => x.OrderStatus == (int)Common.Constant.ORDER_STATUS.CANCELLED);

            Dictionary<string, int> lDonutStatus = new Dictionary<string, int>();
            lDonutStatus.Add("Packed", lPacked);
            //-- Add by Ashish --//
            //Hide EPOD from Ashish for Live
            /*lDonutStatus.Add("Assigned", lAssigned);
            lDonutStatus.Add("Unassigned", lUnassigned);*/
            //-- End Add --//
            lDonutStatus.Add("DispatchedFromShop", lDispatchedFromShop);
            lDonutStatus.Add("InGodown", lInGodown);
            lDonutStatus.Add("DispatchedFromGodown", lDispatchedFromGodown);
            lDonutStatus.Add("Delivered", lDelivered);
            lDonutStatus.Add("Returned", lReturned);
            lDonutStatus.Add("Cancelled", lCancelled);
            lDashBoardViewModel.DonutStatus = lDonutStatus;
            return View(lDashBoardViewModel);
        }
	}
}