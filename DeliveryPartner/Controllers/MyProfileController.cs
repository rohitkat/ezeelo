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
    public class MyProfileController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private DeliveryPartnerSessionViewModel deliveryPartnerSessionViewModel = new DeliveryPartnerSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            deliveryPartnerSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            deliveryPartnerSessionViewModel.Username = Session["UserName"].ToString();
            Common.Common.GetAllLoginDetailFromSession(ref deliveryPartnerSessionViewModel);
        }
        [SessionExpire]
        public ActionResult Index()
        {
            SessionDetails();
            MyProfileViewModel lMyProfileViewModel = new MyProfileViewModel();
            lMyProfileViewModel.ID = deliveryPartnerSessionViewModel.UserLoginID;
            lMyProfileViewModel.UserTypeID = deliveryPartnerSessionViewModel.BusinessTypeId;
            lMyProfileViewModel.OwnerBankID = deliveryPartnerSessionViewModel.OwnerBankID;
            PersonalDetail lPersonalDetail = db.PersonalDetails.SingleOrDefault(x => x.UserLoginID == deliveryPartnerSessionViewModel.UserLoginID);
            if (lPersonalDetail != null)
            {
                lMyProfileViewModel.PersonalDetailID = lPersonalDetail.ID;
            }
            BusinessDetail lBusinessDetail = db.BusinessDetails.SingleOrDefault(x => x.UserLoginID == deliveryPartnerSessionViewModel.UserLoginID && x.BusinessTypeID == (int)Common.Constant.BUSINESS_TYPE.DELIVERY_PARTNER);
            if (lBusinessDetail != null)
            {
                lMyProfileViewModel.BusinessDetailID = lBusinessDetail.ID;
            }

            ModelLayer.Models.DeliveryPartner lDeliveryPartner = db.DeliveryPartners.SingleOrDefault(x => x.BusinessDetailID == lBusinessDetail.ID);
            if (lDeliveryPartner != null)
            {
                lMyProfileViewModel.DeliveryPartnerID = lDeliveryPartner.ID;
            }

            OwnerBank lOwnerBank = db.OwnerBanks.SingleOrDefault(x => x.OwnerID == lDeliveryPartner.ID && x.BusinessTypeID == (int)Common.Constant.BUSINESS_TYPE.DELIVERY_PARTNER);
            if(lOwnerBank != null)
            {
                lMyProfileViewModel.OwnerBankID = lOwnerBank.ID;
            }
            return View(lMyProfileViewModel);
        }
    }
}
