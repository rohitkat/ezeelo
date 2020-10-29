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
using CRM.Models.ViewModel;
using CRM.Models;

namespace CRM.Controllers
{
    public class MyProfileController : Controller
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
        public ActionResult Index()
        {
            SessionDetails();
            MyProfileViewModel lMyProfileViewModel = new MyProfileViewModel();
            lMyProfileViewModel.ID = customerCareSessionViewModel.UserLoginID;
            //lMyProfileViewModel.UserTypeID = fUserTypeId;
            PersonalDetail lPersonalDetail = db.PersonalDetails.SingleOrDefault(x => x.UserLoginID == customerCareSessionViewModel.UserLoginID);
            if (lPersonalDetail != null)
            {
                lMyProfileViewModel.PersonalDetailID = lPersonalDetail.ID;
            }
            //BusinessDetail lBusinessDetail = db.BusinessDetails.SingleOrDefault(x => x.UserLoginID == customerCareSessionViewModel.UserLoginID);
            //if (lBusinessDetail != null)
            //{
            //    lMyProfileViewModel.BusinessDetailID = lBusinessDetail.ID;
            //}

            //ModelLayer.Models.DeliveryPartner lDeliveryPartner = db.DeliveryPartners.SingleOrDefault(x => x.BusinessDetailID == lBusinessDetail.ID);
            //if (lDeliveryPartner != null)
            //{
            //    lMyProfileViewModel.DeliveryPartnerID = lDeliveryPartner.ID;
            //}
            return View(lMyProfileViewModel);
        }
    }
}
