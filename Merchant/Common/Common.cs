using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Collections;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;

namespace Merchant.Common
{
    public static class Common
    {
        private static EzeeloDBContext db = new EzeeloDBContext();

        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
        public static Boolean IsShopHandleDeliveryProcess(long ShopId, ref int DeliveryPartnerID)
        {
            Boolean lIsShopHandleDeliveryProcess = false;
            Shop lShop = db.Shops.Find(ShopId);
            if(lShop == null)
            {
                return false;
            }
            if(lShop.IsDeliveryOutSource) //-- Means eZeelo is responsible for that delivery.
            {
                Pincode lpincode =db.Pincodes.Find(lShop.PincodeID);
                long CityId= lpincode.CityID;
                int FranchiseID =(int)lShop.FranchiseID;
                ////added && FranchiseID==2 for multiple MCO in same city
                if (CityId == 4968 && FranchiseID==2) //---For Nagpur 
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

        public static decimal GetDeliveryCharges(long DeliveryID, decimal DeliveryCharges , string DeviceType, decimal Weight)
        {
            if ("Express" == DeviceType)
            {
                return DeliveryCharges = db.DeliveryWeightSlabs.Where(x => x.DeliveryPartnerID == DeliveryID && x.MaxWeight >= Weight).Select(x => x.ExpressRateWithinPincodeList).FirstOrDefault();
                //change x.MaxWeight <= Weight into x.MaxWeight >= Weight on 16-1-2016 by mohit
            }
            else
            {
                return DeliveryCharges = db.DeliveryWeightSlabs.Where(x => x.DeliveryPartnerID == DeliveryID && x.MaxWeight >= Weight).Select(x => x.NormalRateWithinPincodeList).FirstOrDefault();
                //change x.MaxWeight <= Weight into x.MaxWeight >= Weight on 16-1-2016 by mohit
            }
        }

    }
}