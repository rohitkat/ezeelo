using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Administrator.Models
{
    public static class Common
    {
        private static EzeeloDBContext db = new EzeeloDBContext();        
        public static Boolean GetAllLoginDetailFromSession(ref DeliveryPartnerSessionViewModel deliveryPartnerSessionViewModel)
        {
            long lUserLoginId = deliveryPartnerSessionViewModel.UserLoginID;
            ModelLayer.Models.PersonalDetail lPersonalDetail = db.PersonalDetails.SingleOrDefault(x => x.UserLoginID == lUserLoginId);
            if (lPersonalDetail == null)
            {
                return false;
            }
            deliveryPartnerSessionViewModel.PersonalDetailID = lPersonalDetail.ID;

            ModelLayer.Models.BusinessDetail lBusinessDetail = db.BusinessDetails.SingleOrDefault(x => x.UserLoginID == lUserLoginId && x.BusinessTypeID == (int)Constant.BUSINESS_TYPE.DELIVERY_PARTNER);
            if (lBusinessDetail == null)
            {
                return false;
            }
            deliveryPartnerSessionViewModel.BusinessTypeId = lBusinessDetail.BusinessTypeID;
            deliveryPartnerSessionViewModel.BusinessDetailID = lBusinessDetail.ID;

            ModelLayer.Models.DeliveryPartner lDeliveryPartner = db.DeliveryPartners.SingleOrDefault(x => x.BusinessDetailID == lBusinessDetail.ID && x.IsActive == true);
            if (lDeliveryPartner == null)
            {
                return false;
            }
            deliveryPartnerSessionViewModel.DeliveryPartnerID = lDeliveryPartner.ID;
            return true;
        }

    }
}