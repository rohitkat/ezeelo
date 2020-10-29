using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace API.Controllers
{
    public class TotalPlaceOrder_DPController : ApiController
    {
        EzeeloDBContext db = new EzeeloDBContext();
        public object Get()
        {
            Object obj = new object();
            try
            {
                //List<DeliveryTypeViewModel> DeliveryTypeViewModels = new List<DeliveryTypeViewModel>();
                //DeliveryTypeViewModels.Add(new DeliveryTypeViewModel { ID = 1, Name = "NORMAL" });
                //DeliveryTypeViewModels.Add(new DeliveryTypeViewModel { ID = 2, Name = "EXPRESS" });
                //ViewBag.DeliveryType = new SelectList(DeliveryTypeViewModels, "Name", "Name");

                //var Status = from DeliveryPartner.Common.Constant.ORDER_STATUS d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.ORDER_STATUS))
                //             select new { ID = (int)d, Name = d.ToString() };

                //var DeliveryStatus = new SelectList(Status.Where(x => x.ID >= (int)Common.Constant.ORDER_STATUS.PACKED), "ID", "Name");



                var deliveryorderdetails = (from DOD in db.DeliveryOrderDetails
                                            join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
                                            join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID //Add By Ashish Nagrale for getting PaymentMode
                                            where DOD.ShopOrderCode == COD.ShopOrderCode &&
                                            COD.OrderStatus == 3 &&
                                            DOD.DeliveryPartnerID == 1
                                            && COD.ShopID == 20736
                                            select new 
                                            {
                                                ID = DOD.ID,
                                                DeliveryPartnerID = DOD.DeliveryPartnerID,
                                                GandhibaghOrderID = COD.CustomerOrderID,//Added by Mohit 19-10-15
                                                GandhibaghOrderCode = COD.CustomerOrder.OrderCode,
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
                                                DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
                                                DeliveryScheduleName = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
                                                PaymentMode = CO.PaymentMode//Add bt Ashish Nagrale
                                            }).Distinct().OrderByDescending(x => x.ID).ToList();

                //-----------------Added By Mohit----On 19-10-15---------------------------------//
                //List<SubscriptionPlanUsedBy> lSubscriptionPlanUsedBies = new List<SubscriptionPlanUsedBy>();
                //lSubscriptionPlanUsedBies = db.SubscriptionPlanUsedBies.ToList();

                //foreach (DeliveryIndexViewModel lDeliveryIndexViewModel in deliveryorderdetails)
                //{

                //    SubscriptionPlanUsedBy lSubscriptionPlanUsedBy = lSubscriptionPlanUsedBies.FirstOrDefault(x => x.CustomerOrderID == lDeliveryIndexViewModel.GandhibaghOrderID);

                //    if (lSubscriptionPlanUsedBy != null)
                //    {
                //        lDeliveryIndexViewModel.GandhibaghOrderCode = lDeliveryIndexViewModel.GandhibaghOrderCode + "*";
                //    }

                //}
                //-----------------End of Code By Mohit----On 19-10-15---------------------------------//
                //var deliveryorderdetails = db.DeliveryOrderDetails.Include(d => d.DeliveryPartner).Include(d => d.PersonalDetail).Include(d => d.PersonalDetail1).ToList().Where(x => x.DeliveryPartnerID == fUserId);
                //if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
                //{
                //    // DateTime lFromDate = DateTime.Now;
                //    //if (DateTime.TryParse(FromDate, out lFromDate)) { }
                //    // DateTime lToDate = DateTime.Now;
                //    //if (DateTime.TryParse(ToDate, out lToDate)) { }

                //    DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
                //    DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);
                //    deliveryorderdetails = deliveryorderdetails.Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).ToList();
                //}
                //SearchString = SearchString.Trim();
                //if (SearchString != "")
                //{
                //    deliveryorderdetails = deliveryorderdetails.Where(x => x.ShopOrderCode.ToString().Contains(SearchString) || x.GandhibaghOrderCode.ToString().Contains(SearchString)).ToList();

                //}
                //if (DeliveryType != "")
                //{
                //    deliveryorderdetails = deliveryorderdetails.Where(x => x.DeliveryType.ToUpper().ToString() == DeliveryType.ToUpper()).ToList();
                //}
                //if (DeliveryStatus != null)
                //{
                //    deliveryorderdetails = deliveryorderdetails.Where(x => x.OrderStatus == DeliveryStatus).ToList();
                //}
                obj = new { Success = 1, Message = "Success", data = deliveryorderdetails };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
