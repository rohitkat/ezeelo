using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using ModelLayer.Models;
using System.Collections.ObjectModel;
using Administrator.Models;

namespace Administrator.Controllers
{
    public class FranchiseLocationController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 20;
        //
        // GET: /FranchiseLocation/
        [SessionExpire]
        public ActionResult Index()
        {
            try
            {
                ViewBag.franchise = new SelectList((from f in db.Franchises
                                                    join b in db.BusinessDetails on f.BusinessDetailID equals b.ID
                                                    join u in db.UserLogins on b.UserLoginID equals u.ID
                                                    join pd in db.PersonalDetails on u.ID equals pd.UserLoginID
                                                    where f.IsActive==true && f.ID != 1 
                                                    select new
                                                    {
                                                        franchiseID = f.ID,
                                                        franchiseName = pd.FirstName + " " + pd.LastName,
                                                    }).ToList(), "franchiseID", "franchiseName");
                ViewBag.stateID = new SelectList(db.States, "ID", "Name");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling State and franchise dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseLocation][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling State and franchise dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseLocation][POST:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [SessionExpire]
        public JsonResult GetCityByStateId(int stateID)
        {
            //var district = (List<District>)db.Districts.Where(u => u.StateID == stateID).ToList();
            //var district = from cust in db.States 
            //                        select cust;
            List<District> ldistrict = new List<District>();
            List<City> lcity = new List<City>();
            List<StateCityFranchiseMerchantViewModel> city = new List<StateCityFranchiseMerchantViewModel>();
            try
            {
                ldistrict = db.Districts.Where(x => x.StateID == stateID).ToList();
                foreach (var x in ldistrict)
                {

                    lcity = db.Cities.Where(c => c.DistrictID == x.ID).ToList();
                    foreach (var c in lcity)
                    {
                        StateCityFranchiseMerchantViewModel SCFM = new StateCityFranchiseMerchantViewModel();
                        SCFM.CityID = c.ID;
                        SCFM.CityName = c.Name;
                        city.Add(SCFM);
                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling City dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseLocation][POST:GetCityByStateId]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling City dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseLocation][POST:GetCityByStateId]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return Json(city.Distinct().OrderBy(x => x.CityName).ToList(), JsonRequestBehavior.AllowGet);
        }

        [SessionExpire]
        public ActionResult GetAreaByCity(int cityID, int page, int pagecount, int FranchiseID)
        {
            int TotalCount = 0;
            int TotalPages = 0;
            ViewBag.cityID = cityID;
            ViewBag.FranchiseID = FranchiseID;
            int pageNumber = page;

            List<Pincode> lpincode = new List<Pincode>();
            List<Area> lArea = new List<Area>();
            List<StateCityFranchiseMerchantViewModel> Area = new List<StateCityFranchiseMerchantViewModel>();
            try
            {
                lpincode = db.Pincodes.Where(x => x.CityID == cityID).ToList();

                foreach (var x in lpincode)
                {


                    lArea = db.Areas.Where(c => c.PincodeID == x.ID).ToList();
                    foreach (var c in lArea)
                    {
                        StateCityFranchiseMerchantViewModel SCFM = new StateCityFranchiseMerchantViewModel();
                        SCFM.AreaID = c.ID;
                        SCFM.AreaName = c.Name;
                        Area.Add(SCFM);
                    }
                }

                Area = (from a in Area
                        where !(from f in db.FranchiseLocations
                                where f.FranchiseID == FranchiseID
                                select f.AreaID).ToList()
                           .Contains(a.AreaID)
                        select new StateCityFranchiseMerchantViewModel { AreaID = a.AreaID, AreaName = a.AreaName }).ToList();



                TotalCount = Area.Count();
                ViewBag.TotalCount = TotalCount;
                //Area = Area.OrderBy(x => x.AreaID).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.PageSize = Area.Count;
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                ViewBag.TotalPages = TotalPages;
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling Areas!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseRegistrationReportController][POST:GetCityByStateId]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling Areas!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseRegistrationReportController][POST:GetCityByStateId]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View("_Area", Area.Distinct().OrderBy(x => x.AreaName).ToList());
        }

        [SessionExpire]
        public ActionResult GetAddedArea(int FranchiseID)
        {
            int TotalCount = 0;
            int TotalPages = 0;
            try
            {

                var Area = (from fl in db.FranchiseLocations
                            join a in db.Areas on fl.AreaID equals a.ID
                            where fl.FranchiseID == FranchiseID
                            select new StateCityFranchiseMerchantViewModel { AreaID = a.ID, AreaName = a.Name }).ToList();

                //TotalCount = Area.Count();
                //ViewBag.TotalCount = TotalCount;
                //Area = Area.OrderBy(x => x.AreaID).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                //ViewBag.PageSize = Area.Count;
                //TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                //ViewBag.TotalPages = TotalPages;
                return View("_GetAddedArea", Area.Distinct().OrderBy(x => x.AreaName).ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling Added Areas in database!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseRegistrationReportController][POST:GetAddedArea]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling Added Areas in database!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseRegistrationReportController][POST:GetAddedArea]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }
        [SessionExpire]
        public JsonResult AddLocationToDatabase(int FranchiseID, int AreaID)
        {
            string Message = "";
            try
            {
                long ID = db.FranchiseLocations.Where(x => x.FranchiseID == FranchiseID && x.AreaID == AreaID).Select(x => x.ID).FirstOrDefault();
                if (ID > 0)
                {
                    Message = "Already exist";
                }
                else
                {
                    FranchiseLocation franchiselocation = new FranchiseLocation();

                    franchiselocation.FranchiseID = FranchiseID;
                    franchiselocation.AreaID = AreaID;
                    franchiselocation.IsActive = true;
                    franchiselocation.CreateDate = DateTime.UtcNow;
                    franchiselocation.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    franchiselocation.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    franchiselocation.DeviceID = "x";
                    franchiselocation.DeviceType = "x";
                    db.FranchiseLocations.Add(franchiselocation);
                    db.SaveChanges();
                    Message = "Saved";
                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong while Saving Area!!");
                Message = "Error in Save";
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseLocation][POST:AddLocationToDatabase]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return Json(Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Saving Area!!");
                Message = "Error in Save";
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseLocation][POST:AddLocationToDatabase]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return Json(Message, JsonRequestBehavior.AllowGet);
            }

            return Json(Message, JsonRequestBehavior.AllowGet);
        }

        [SessionExpire]
        public JsonResult DeleteLocationFromDatabase(int FranchiseID, int AreaID)
        {
            string Message = "";
            try
            {

                FranchiseLocation franchiselocation = db.FranchiseLocations.Where(x => x.FranchiseID == FranchiseID && x.AreaID == AreaID).FirstOrDefault();
                db.FranchiseLocations.Remove(franchiselocation);
                db.SaveChanges();
                Message = "Removed";

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong while deleting Area!!");
                Message = "Error in Save";
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseLocation][POST:DeleteLocationFromDatabase]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return Json(Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while deleting Area!!");
                Message = "Error in Save";
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseLocation][POST:DeleteLocationFromDatabase]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                return Json(Message, JsonRequestBehavior.AllowGet);
            }

            return Json(Message, JsonRequestBehavior.AllowGet);

        }
        //
        // GET: /FranchiseLocation/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /FranchiseLocation/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /FranchiseLocation/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /FranchiseLocation/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /FranchiseLocation/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /FranchiseLocation/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /FranchiseLocation/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
