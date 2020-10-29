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
    public class MyRegionController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private DeliveryPartnerSessionViewModel deliveryPartnerSessionViewModel = new DeliveryPartnerSessionViewModel();
        private int pageSize = 100;//Yashaswi 12-10-2018 to disable paging 10;

        public void SessionDetails()
        {
            deliveryPartnerSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            deliveryPartnerSessionViewModel.Username = Session["UserName"].ToString();
            Common.Common.GetAllLoginDetailFromSession(ref deliveryPartnerSessionViewModel);
        }
        [SessionExpire]
        private int FindServiceLevel()
        {
            SessionDetails();
            int lServiceLevel = -1;
            ModelLayer.Models.DeliveryPartner lDeliveryPartner = db.DeliveryPartners.SingleOrDefault(x => x.BusinessDetail.UserLoginID == deliveryPartnerSessionViewModel.UserLoginID);
            if (lDeliveryPartner == null)
            {
                return -1;
            }
            lServiceLevel = lDeliveryPartner.ServiceLevel;
            return lServiceLevel;
        }

        [SessionExpire]
        public ActionResult Index()
        {
            SessionDetails();
            int lServiceLevel = FindServiceLevel();
            if (lServiceLevel < 0)
            {
                return View("Error");
            }
            ViewBag.lServiceLevel = lServiceLevel;
            return View();
        }

        [SessionExpire]
        public ActionResult State(int? page, int?[] chkBox, string SearchString = "")
        {
            SessionDetails();
            int lServiceLevel = FindServiceLevel();
            if (lServiceLevel < 0)
            {
                return View("Error");
            }
            ViewBag.lServiceLevel = lServiceLevel;

            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;

            var States = (from st in db.States
                          select new StateViewModel
                          {
                              ID = st.ID,
                              Name = st.Name
                          }).ToList();
            foreach (StateViewModel stateViewModel in States)
            {
                if (db.DeliveryPincodes.Any(x => x.Pincode.City.District.State.ID == stateViewModel.ID && x.IsActive == true && x.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID))
                {
                    stateViewModel.IsSelected = true;
                }
            }


            List<StateViewModel> StateViewModelDistinctList = States.OrderByDescending(x => x.IsSelected).GroupBy(x => x.ID).Select(y => y.First()).OrderBy(x => x.Name).ToList();


            if (SearchString != "")
            {
                return View("State", StateViewModelDistinctList.Where(x => x.Name.Contains(SearchString)).ToPagedList(pageNumber, pageSize));

            }
            return View("State", StateViewModelDistinctList.ToPagedList(pageNumber, pageSize));
        }

        [SessionExpire]
        public ActionResult District(int? StateID, int? page, string SearchString = "")
        {
            SessionDetails();
            int lServiceLevel = FindServiceLevel();
            if (lServiceLevel < 0)
            {
                return View("Error");
            }
            ViewBag.lServiceLevel = lServiceLevel;

            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;

            int lStateID = -1;
            if (StateID != null)
            {
                lStateID = Convert.ToInt32(StateID);
            }

            var Districts = (from dt in db.Districts
                             where dt.StateID == lStateID
                             select new DistrictViewModel
                             {
                                 ID = dt.ID,
                                 Name = dt.Name
                             }).ToList();
            foreach (DistrictViewModel districtViewModel in Districts)
            {
                if (db.DeliveryPincodes.Any(x => x.Pincode.City.DistrictID == districtViewModel.ID && x.IsActive == true && x.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID))
                {
                    districtViewModel.IsSelected = true;
                }
            }
            List<DistrictViewModel> DistrictViewModelDistinctList = Districts.OrderByDescending(x => x.IsSelected).GroupBy(x => x.ID).Select(y => y.First()).OrderBy(x => x.Name).ToList();
            if (SearchString != "")
            {
                return View("District", DistrictViewModelDistinctList.Where(x => x.Name.Contains(SearchString)).ToPagedList(pageNumber, pageSize));

            }
            return View("District", DistrictViewModelDistinctList.ToPagedList(pageNumber, pageSize));
        }

        [SessionExpire]
        public ActionResult City(int? DistrictID, int? page, string SearchString = "")
        {
            SessionDetails();
            int lServiceLevel = FindServiceLevel();
            if (lServiceLevel < 0)
            {
                return View("Error");
            }
            ViewBag.lServiceLevel = lServiceLevel;

            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;

            int lDistrictID = -1;
            if (DistrictID != null)
            {
                lDistrictID = Convert.ToInt32(DistrictID);
            }

            var Cities = (from ct in db.Cities
                          where ct.DistrictID == lDistrictID
                          select new CityViewModel
                          {
                              ID = ct.ID,
                              Name = ct.Name
                          }).ToList();
            foreach (CityViewModel cityViewModel in Cities)
            {
                if (db.DeliveryPincodes.Any(x => x.Pincode.City.ID == cityViewModel.ID && x.IsActive == true && x.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID))
                {
                    cityViewModel.IsSelected = true;
                }
            }

            List<CityViewModel> CityViewModelDistinctList = Cities.OrderByDescending(x => x.IsSelected).GroupBy(x => x.ID).Select(y => y.First()).OrderBy(x => x.Name).ToList();

            if (SearchString != "")
            {
                return View("City", CityViewModelDistinctList.Where(x => x.Name.Contains(SearchString)).ToPagedList(pageNumber, pageSize));

            }
            return View("City", CityViewModelDistinctList.ToPagedList(pageNumber, pageSize));
        }

        [SessionExpire]
        public ActionResult Pincode(int? CityID, int? page, string SearchString = "")
        {
            SessionDetails();
            int lServiceLevel = FindServiceLevel();
            if (lServiceLevel < 0)
            {
                return View("Error");
            }
            ViewBag.lServiceLevel = lServiceLevel;

            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;

            int lCityID = -1;
            if (CityID != null)
            {
                lCityID = Convert.ToInt32(CityID);
            }

            var Pincodes = (from pi in db.Pincodes
                            where pi.CityID == lCityID
                            select new PincodeViewModel
                            {
                                ID = pi.ID,
                                Name = pi.Name
                            }).ToList();
            foreach (PincodeViewModel pincodeViewModel in Pincodes)
            {
                if (db.DeliveryPincodes.Any(x => x.Pincode.ID == pincodeViewModel.ID && x.IsActive == true && x.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID))
                {
                    pincodeViewModel.IsSelected = true;
                }
            }



            List<PincodeViewModel> PincodeViewModelDistinctList = Pincodes.OrderByDescending(x => x.IsSelected).GroupBy(x => x.ID).Select(y => y.First()).OrderBy(x => x.Name).ToList();

            if (SearchString != "")
            {
                return View("Pincode", PincodeViewModelDistinctList.Where(x => x.Name.Contains(SearchString)).ToPagedList(pageNumber, pageSize));

            }
            return View("Pincode", PincodeViewModelDistinctList.ToPagedList(pageNumber, pageSize));
        }

        [SessionExpire]
        public ActionResult CreateState(int?[] chkBox)
        {
            SessionDetails();
            int lServiceLevel = FindServiceLevel();
            if (lServiceLevel < 0)
            {
                return View("Error");
            }
            ViewBag.lServiceLevel = lServiceLevel;

            if (TempData["lAllStateInThisPage"] == null)
            {
                return View("Error");
            }
            else //--------------------------------------- For all Check box that are not checked.
            {
                Dictionary<int, int> lAllStateInThisPage = (Dictionary<int, int>)TempData["lAllStateInThisPage"];
                ArrayList lArrayListChecked = new ArrayList();
                ArrayList lArrayListNotChecked = new ArrayList();

                if (chkBox != null)
                {
                    foreach (int checkedState in chkBox) //- remove items if it is checked already...
                    {
                        if (lAllStateInThisPage.ContainsValue(checkedState))
                        {
                            lAllStateInThisPage.Remove(checkedState);
                        }
                    }

                    foreach (int x in chkBox)
                    {
                        lArrayListChecked.Add(x);
                    }
                }

                foreach (KeyValuePair<int, int> x in lAllStateInThisPage)
                {
                    lArrayListNotChecked.Add(x.Value);
                }

                Common.Common.InsertUpdateDeliveryPincodeByState(deliveryPartnerSessionViewModel.DeliveryPartnerID, deliveryPartnerSessionViewModel.PersonalDetailID, lArrayListNotChecked, lArrayListChecked);
            }
            return RedirectToAction("state", "MyRegion");
        }

        [SessionExpire]
        public ActionResult CreateDistrict(int?[] chkBox)
        {
            SessionDetails();
            int lServiceLevel = FindServiceLevel();
            if (lServiceLevel < 0)
            {
                return View("Error");
            }
            ViewBag.lServiceLevel = lServiceLevel;

            if (TempData["lAllDistrictInThisPage"] == null)
            {
                return View("Error");
            }
            else //--------------------------------------- For all Check box that are not checked.
            {
                Dictionary<int, int> lAllDistrictInThisPage = (Dictionary<int, int>)TempData["lAllDistrictInThisPage"];
                ArrayList lArrayListChecked = new ArrayList();
                ArrayList lArrayListNotChecked = new ArrayList();

                if (chkBox != null)
                {
                    foreach (int checkedDistrict in chkBox) //- remove items if it is checked already...
                    {
                        if (lAllDistrictInThisPage.ContainsValue(checkedDistrict))
                        {
                            lAllDistrictInThisPage.Remove(checkedDistrict);
                        }
                    }

                    foreach (int x in chkBox)
                    {
                        lArrayListChecked.Add(x);
                    }
                }

                foreach (KeyValuePair<int, int> x in lAllDistrictInThisPage)
                {
                    lArrayListNotChecked.Add(x.Value);
                }

                Common.Common.InsertUpdateDeliveryPincodeByDistrict(deliveryPartnerSessionViewModel.DeliveryPartnerID, deliveryPartnerSessionViewModel.PersonalDetailID, lArrayListNotChecked, lArrayListChecked);
            }
            return RedirectToAction("District", "MyRegion");
        }
        [SessionExpire]
        public ActionResult CreateCity(int?[] chkBox)
        {
            SessionDetails();
            int lServiceLevel = FindServiceLevel();
            if (lServiceLevel < 0)
            {
                return View("Error");
            }
            ViewBag.lServiceLevel = lServiceLevel;

            if (TempData["lAllCityInThisPage"] == null)
            {
                return View("Error");
            }
            else //--------------------------------------- For all Check box that are not checked.
            {
                Dictionary<int, int> lAllCityInThisPage = (Dictionary<int, int>)TempData["lAllCityInThisPage"];
                ArrayList lArrayListChecked = new ArrayList();
                ArrayList lArrayListNotChecked = new ArrayList();

                if (chkBox != null)
                {
                    foreach (int checkedCity in chkBox) //- remove items if it is checked already...
                    {
                        if (lAllCityInThisPage.ContainsValue(checkedCity))
                        {
                            lAllCityInThisPage.Remove(checkedCity);
                        }
                    }

                    foreach (int x in chkBox)
                    {
                        lArrayListChecked.Add(x);
                    }
                }

                foreach (KeyValuePair<int, int> x in lAllCityInThisPage)
                {
                    lArrayListNotChecked.Add(x.Value);
                }

                Common.Common.InsertUpdateDeliveryPincodeByCity(deliveryPartnerSessionViewModel.DeliveryPartnerID, deliveryPartnerSessionViewModel.PersonalDetailID, lArrayListNotChecked, lArrayListChecked);
            }
            return RedirectToAction("City", "MyRegion");
        }

        [SessionExpire]
        public ActionResult CreatePincode(int?[] chkBox)
        {
            SessionDetails();
            int lServiceLevel = FindServiceLevel();
            if (lServiceLevel < 0)
            {
                return View("Error");
            }
            ViewBag.lServiceLevel = lServiceLevel;

            if (TempData["lAllPincodeInThisPage"] == null)
            {
                return View("Error");
            }
            else //--------------------------------------- For all Check box that are not checked.
            {
                Dictionary<int, int> lAllPincodeInThisPage = (Dictionary<int, int>)TempData["lAllPincodeInThisPage"];
                ArrayList lArrayListChecked = new ArrayList();
                ArrayList lArrayListNotChecked = new ArrayList();

                if (chkBox != null)
                {
                    foreach (int checkedPincode in chkBox) //- remove items if it is checked already...
                    {
                        if (lAllPincodeInThisPage.ContainsValue(checkedPincode))
                        {
                            lAllPincodeInThisPage.Remove(checkedPincode);
                        }
                    }

                    foreach (int x in chkBox)
                    {
                        lArrayListChecked.Add(x);
                    }
                }

                foreach (KeyValuePair<int, int> x in lAllPincodeInThisPage)
                {
                    lArrayListNotChecked.Add(x.Value);
                }

                Common.Common.InsertUpdateDeliveryPincodeByPincode(deliveryPartnerSessionViewModel.DeliveryPartnerID, deliveryPartnerSessionViewModel.PersonalDetailID, lArrayListNotChecked, lArrayListChecked);
            }
            return RedirectToAction("Pincode", "MyRegion");
        }
    }

}
