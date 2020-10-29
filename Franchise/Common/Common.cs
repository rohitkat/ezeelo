using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using Franchise.Models.ViewModel;
using ModelLayer.Models;
using Franchise.Models;

namespace Franchise.Common
{
    static public class Common
    {
        private static EzeeloDBContext db = new EzeeloDBContext();

        public static string GetDeliveryStatus(int pDeliveryStatusID)
        {
            Dictionary<int, string> lOrderStatus = new Dictionary<int, string>();
            lOrderStatus.Add(1, "Placed");
            lOrderStatus.Add(2, "Confirm");
            lOrderStatus.Add(3, "Packed");
            lOrderStatus.Add(4, "Dispatch from Shop");
            lOrderStatus.Add(5, "In Godown");
            lOrderStatus.Add(6, "Dispatch from Godown");
            lOrderStatus.Add(7, "Delivered");
            lOrderStatus.Add(8, "Returned");
            lOrderStatus.Add(9, "Cancelled");
            lOrderStatus.Add(10, "Abandoned"); //Added by Zubair on 23-11-2017
            if (pDeliveryStatusID != null && pDeliveryStatusID > 0)
            {
                return lOrderStatus[pDeliveryStatusID];
            }
            return lOrderStatus[1];
        }
        public static List<SelectListItem> DropDownListOrderStatus()
        {
            List<OrderStatusViewModel> OrderStatusViewModels = new List<OrderStatusViewModel>() { 
                                                new OrderStatusViewModel(){ID = 1, Name = "Placed"},
                                                new OrderStatusViewModel(){ID = 2, Name = "Confirm"},
                                                new OrderStatusViewModel(){ID = 3, Name = "Packed"},
                                                new OrderStatusViewModel(){ID = 4, Name = "Dispatch from Shop"},
                                                new OrderStatusViewModel(){ID = 5, Name = "In Godown"},
                                                new OrderStatusViewModel(){ID = 6, Name = "Dispatch from Godown"},
                                                new OrderStatusViewModel(){ID = 7, Name = "Delivered"},
                                                new OrderStatusViewModel(){ID = 8, Name = "Returned"},
                                                new OrderStatusViewModel(){ID = 9, Name = "Cancelled"},
                                                };
            var OrderStatus = from OS in OrderStatusViewModels select new { ID = OS.ID, Name = OS.Name };
            List<SelectListItem> DDlOrderStatus = new List<SelectListItem>();
            foreach (var item in OrderStatus)
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = item.Name;
                selectListItem.Value = item.ID.ToString();
                DDlOrderStatus.Add(selectListItem);
            }
            return DDlOrderStatus;
        }

        public static Boolean GetAllLoginDetailFromSession(ref CustomerCareSessionViewModel customerCareSessionViewModel)
        {
            long lUserLoginId = customerCareSessionViewModel.UserLoginID;
            ModelLayer.Models.PersonalDetail lPersonalDetail = db.PersonalDetails.SingleOrDefault(x => x.UserLoginID == lUserLoginId);
            if (lPersonalDetail == null)
            {
                return false;
            }
            customerCareSessionViewModel.PersonalDetailID = lPersonalDetail.ID;

            ModelLayer.Models.Employee lEmployee = db.Employees.SingleOrDefault(x => x.UserLoginID == lUserLoginId && x.OwnerID == Constant.CRM_OWNER_ID && x.EmployeeCode.Contains(Constant.CRM_EMP_CODE));
            if (lEmployee == null)
            {
                return false;
            }
            try
            {
                customerCareSessionViewModel.BusinessTypeId = db.BusinessDetails.FirstOrDefault(x => x.UserLoginID == lUserLoginId).BusinessTypeID;
            }
            catch (Exception)
            {
                
                //throw;
            }
            
            customerCareSessionViewModel.EmployeeCode = lEmployee.EmployeeCode;
            return true;
        }

        public static Boolean IsShopHandleDeliveryProcess(long ShopId, ref int DeliveryPartnerID)
        {
            Boolean lIsShopHandleDeliveryProcess = false;
            Shop lShop = db.Shops.Find(ShopId);
            if (lShop == null)
            {
                return false;
            }
            if (lShop.IsDeliveryOutSource) //-- Means eZeelo is responsible for that delivery.
            {
                Pincode lpincode = db.Pincodes.Find(lShop.PincodeID);
                long CityId = lpincode.CityID;
                int FranchiseID = (int)lShop.FranchiseID;
                ////added && FranchiseID==2 for multiple MCO in same city
                if (CityId == 4968 && FranchiseID == 2) //---For Nagpur 
                {
                    DeliveryPartnerID = 2;
                    lIsShopHandleDeliveryProcess = false;
                }
                ////added && FranchiseID==4 for multiple MCO in same city
                if (CityId == 10908 && FranchiseID == 4) //---For Varanasi
                {
                    DeliveryPartnerID = 3;
                    lIsShopHandleDeliveryProcess = false;
                }
                ////added && FranchiseID== for multiple MCO in same city
                if (CityId == 10909 && FranchiseID == 3) //---For Kanpur
                {
                    DeliveryPartnerID = 4;
                    lIsShopHandleDeliveryProcess = false;
                }
                ////added && FranchiseID==8 for multiple MCO in same city
                if (CityId == 7536 && FranchiseID == 8) //--- For Wardha
                {
                    DeliveryPartnerID = 5;
                    lIsShopHandleDeliveryProcess = false;
                }
            }
            else
            {
                DeliveryPartnerID = Convert.ToInt32(lShop.DeliveryPartnerId);
                lIsShopHandleDeliveryProcess = true;
            }
            return lIsShopHandleDeliveryProcess;
        }

        public static decimal GetDeliveryCharges(long DeliveryID, decimal DeliveryCharges, string DeviceType, decimal Weight)
        {
            if ("Express" == DeviceType)
            {
                return DeliveryCharges = db.DeliveryWeightSlabs.Where(x => x.DeliveryPartnerID == DeliveryID && x.MaxWeight <= Weight).Select(x => x.ExpressRateWithinPincodeList).FirstOrDefault();
            }
            else
            {
                return DeliveryCharges = db.DeliveryWeightSlabs.Where(x => x.DeliveryPartnerID == DeliveryID && x.MaxWeight <= Weight).Select(x => x.NormalRateWithinPincodeList).FirstOrDefault();
            }
        }

        public static string GetRefOrderCode(long? RefOrderID)
        {
            string RefOrderCode = string.Empty;
            if (RefOrderID != null)
            {
                RefOrderCode = db.CustomerOrders.Find(RefOrderID).OrderCode.Trim().ToUpper();
            }
            return RefOrderCode;
        }
    }
}