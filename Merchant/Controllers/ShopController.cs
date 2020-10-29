using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using Merchant.Models;

namespace Merchant.Controllers
{
    public class ShopController : Controller
    {
      
        #region Genral Code
        private EzeeloDBContext db = new EzeeloDBContext();


        private long GetPersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
            long PersonalDetailID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PlacedController][GetShopID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }

        private long GetShopID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
            long BusinessDetailID = 0;
            long ShopID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    BusinessDetailID = Convert.ToInt32(db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                    ShopID = Convert.ToInt32(db.Shops.Where(x => x.BusinessDetailID == BusinessDetailID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PlacedController][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
            }
            return ShopID;
        }

        #endregion
        // GET: /Shop/
        public ActionResult Index()
        {
            var shops = db.Shops.Include(s => s.Area).Include(s => s.BusinessDetail).Include(s => s.DeliveryPartner).Include(s => s.Franchise).Include(s => s.PersonalDetail).Include(s => s.PersonalDetail1).Include(s => s.Pincode);
            return View(shops.ToList());
        }

        // GET: /Shop/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shop shop = db.Shops.Find(id);
            if (shop == null)
            {
                return HttpNotFound();
            }
            return View(shop);
        }

        // GET: /Shop/Create
        public ActionResult Create()
        {
            ViewBag.AreaID = new SelectList(db.Areas, "ID", "Name");
            ViewBag.BusinessDetailID = new SelectList(db.BusinessDetails, "ID", "Name");
            ViewBag.DeliveryPartnerId = new SelectList(db.DeliveryPartners, "ID", "GodownAddress");
            ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber");
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name");
            return View();
        }

        // POST: /Shop/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,BusinessDetailID,Name,Website,Lattitude,Longitude,Address,NearestLandmark,PincodeID,AreaID,OpeningTime,ClosingTime,ContactPerson,Email,Mobile,Landline,FAX,VAT,TIN,PAN,WeeklyOff,CurrentItSetup,InstitutionalMerchantPurchase,InstitutionalMerchantSale,NormalSale,IsDeliveryOutSource,IsFreeHomeDelivery,MinimumAmountForFreeDelivery,DeliveryPartnerId,FranchiseID,IsLive,IsManageInventory,SearchKeywords,IsAgreedOnReturnProduct,ReturnDurationInDays,Description,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] Shop shop)
        {
            if (ModelState.IsValid)
            {
                db.Shops.Add(shop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AreaID = new SelectList(db.Areas, "ID", "Name", shop.AreaID);
            ViewBag.BusinessDetailID = new SelectList(db.BusinessDetails, "ID", "Name", shop.BusinessDetailID);
            ViewBag.DeliveryPartnerId = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", shop.DeliveryPartnerId);
            ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber", shop.FranchiseID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", shop.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", shop.ModifyBy);
            ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", shop.PincodeID);
            return View(shop);
        }

        // GET: /Shop/Edit/5
         [SessionExpire]
        public ActionResult Edit()
        {
            long id = GetShopID();
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shop shop = db.Shops.Find(id);
            if (shop == null)
            {
                return HttpNotFound();
            }
            Pincode lPincode = db.Pincodes.Find(shop.PincodeID);
            if (lPincode != null)
            {
                ViewBag.Pincode = lPincode.Name;
            }
            //GetAddress(lPincode.Name);
              var NewArea = db.Areas.Where(x => x.PincodeID == shop.PincodeID).ToList();
            //if(shop.AreaID > 0)
            ViewBag.AreaID = new SelectList(NewArea, "ID", "Name", shop.AreaID);
            ViewBag.BusinessDetailID = new SelectList(db.BusinessDetails, "ID", "Name", shop.BusinessDetailID);
            ViewBag.DeliveryPartnerId = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", shop.DeliveryPartnerId);
            ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber", shop.FranchiseID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", shop.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", shop.ModifyBy);
            ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", shop.PincodeID);
            return View(shop);
        }

        // POST: /Shop/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Pincode,[Bind(Include="ID,BusinessDetailID,Name,Website,Lattitude,Longitude,Address,NearestLandmark,PincodeID,AreaID,OpeningTime,ClosingTime,ContactPerson,Email,Mobile,Landline,FAX,VAT,TIN,PAN,WeeklyOff,CurrentItSetup,InstitutionalMerchantPurchase,InstitutionalMerchantSale,NormalSale,IsDeliveryOutSource,IsFreeHomeDelivery,MinimumAmountForFreeDelivery,IsLive,IsManageInventory,SearchKeywords,IsAgreedOnReturnProduct,ReturnDurationInDays,Description")] Shop shop, int? Area)
        {
          
            try
            {

                EzeeloDBContext db1 = new EzeeloDBContext();
                
                long PersonalDetailID = GetPersonalDetailID();

                ViewBag.AreaID = new SelectList(db.Areas, "ID", "Name", shop.AreaID);
                ViewBag.BusinessDetailID = new SelectList(db.BusinessDetails, "ID", "Name", shop.BusinessDetailID);
                ViewBag.DeliveryPartnerId = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", shop.DeliveryPartnerId);
                ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber", shop.FranchiseID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", shop.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", shop.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", shop.PincodeID);

                Shop sp = db1.Shops.Find(shop.ID);

                shop.CreateBy = sp.CreateBy;
                shop.CreateDate = sp.CreateDate;
                shop.DeliveryPartnerId = sp.DeliveryPartnerId;
                shop.FranchiseID = sp.FranchiseID;
                shop.ModifyDate = DateTime.UtcNow;
                shop.ModifyBy = PersonalDetailID;
                shop.BusinessDetailID = sp.BusinessDetailID;
                shop.IsActive = sp.IsActive;
                if (Area != null)
                {
                    shop.AreaID = Area;
                }
                Pincode lPincode = db1.Pincodes.Single(x => x.Name == Pincode);
                if (lPincode == null)
                {
                    return View("Error");
                }
                shop.PincodeID = lPincode.ID;
                //sp.BusinessDetailID = shop.BusinessDetailID;
                //sp.Name = shop.Name;
                //sp.Website = shop.Website;
                //sp.Lattitude = shop.Lattitude;
                //sp.Longitude = shop.Longitude;
                //sp.Address = shop.Address;
                //sp.NearestLandmark = shop.NearestLandmark;

                //Pincode lPincode = db.Pincodes.Single(x => x.Name == Pincode);
                //if (lPincode == null)
                //{
                //    return View("Error");
                //}
                //shop.PincodeID = lPincode.ID;

                //sp.PincodeID = shop.PincodeID;
                //sp.AreaID = shop.AreaID;
                //sp.OpeningTime = shop.OpeningTime;
                //sp.ClosingTime = shop.ClosingTime;
                //sp.ContactPerson = shop.ContactPerson;
                //sp.Email = shop.Email;
                //sp.Mobile = shop.Mobile;
                //sp.Landline = shop.Landline;
                //sp.FAX = shop.FAX;
                //sp.VAT = shop.VAT;
                //sp.TIN = shop.TIN;
                //sp.PAN = shop.PAN;
                //sp.WeeklyOff = shop.WeeklyOff;
                //sp.CurrentItSetup = shop.CurrentItSetup;
                //sp.InstitutionalMerchantPurchase = shop.InstitutionalMerchantPurchase;
                //sp.InstitutionalMerchantSale = shop.InstitutionalMerchantSale;
                //sp.NormalSale = shop.NormalSale;
                //sp.IsDeliveryOutSource = shop.IsDeliveryOutSource;
                //sp.IsFreeHomeDelivery = shop.IsFreeHomeDelivery;
                //sp.MinimumAmountForFreeDelivery = shop.MinimumAmountForFreeDelivery;
                ////sp.DeliveryPartnerId = shop.DeliveryPartnerId;
                ////sp.FranchiseID =  shop.FranchiseID;
                //sp.IsLive = shop.IsLive;
                //sp.IsManageInventory = shop.IsManageInventory;
                //sp.SearchKeywords = shop.SearchKeywords;
                //sp.IsAgreedOnReturnProduct = shop.IsAgreedOnReturnProduct;
                //sp.ReturnDurationInDays = shop.ReturnDurationInDays;
                //sp.Description = shop.Description;
                //sp.IsActive = true;
                //sp.CreateBy = shop.CreateBy;
                //sp.CreateDate = shop.CreateDate;
                //sp.ModifyBy = 1;// PersonalDetailID;
                //sp.ModifyDate=DateTime.UtcNow;
                //sp.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                //sp.DeviceID = "x";
                //sp.DeviceType = "x";
                db1.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(shop).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Message = "Record Saved Successfully.";
                    return View(shop);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);



                //GetAddress(lPincode.Name);
                var NewArea = db.Areas.Where(x => x.PincodeID == shop.PincodeID).ToList();
                //if(shop.AreaID > 0)
                ViewBag.AreaID = new SelectList(NewArea, "ID", "Name", shop.AreaID);
                //ViewBag.AreaID = new SelectList(db.Areas, "ID", "Name", shop.AreaID);
                ViewBag.BusinessDetailID = new SelectList(db.BusinessDetails, "ID", "Name", shop.BusinessDetailID);
                ViewBag.DeliveryPartnerId = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", shop.DeliveryPartnerId);
                ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber", shop.FranchiseID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", shop.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", shop.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", shop.PincodeID);
                
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                //GetAddress(lPincode.Name);
                var NewArea = db.Areas.Where(x => x.PincodeID == shop.PincodeID).ToList();
                //if(shop.AreaID > 0)
                ViewBag.AreaID = new SelectList(NewArea, "ID", "Name", shop.AreaID);
               // ViewBag.AreaID = new SelectList(db.Areas, "ID", "Name", shop.AreaID);
                ViewBag.BusinessDetailID = new SelectList(db.BusinessDetails, "ID", "Name", shop.BusinessDetailID);
                ViewBag.DeliveryPartnerId = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", shop.DeliveryPartnerId);
                ViewBag.FranchiseID = new SelectList(db.Franchises, "ID", "ServiceNumber", shop.FranchiseID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", shop.CreateBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", shop.ModifyBy);
                ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", shop.PincodeID);
                
            }
            return View(shop);
        }

        // GET: /Shop/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shop shop = db.Shops.Find(id);
            if (shop == null)
            {
                return HttpNotFound();
            }
            return View(shop);
        }

        // POST: /Shop/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Shop shop = db.Shops.Find(id);
            db.Shops.Remove(shop);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult GetAddress(string Pincode)
        {

            if (!(db.Pincodes.Any(p => p.Name == Pincode)))
            {
                var errorMsg = "Pincode Dose Not Exist, Please Contact Admin!";
                return View(new { success = false, Error = errorMsg});
                //return Json("1", JsonRequestBehavior.AllowGet);
            }
            //Pincode lPincode = db.Pincodes.Find(Pincode);
           // GetAreaIDByPincodeID(lPincode.ID);
           long PincodeId = db.Pincodes.SingleOrDefault(x => x.Name == Pincode).ID;
           var NewArea = db.Areas.Where(x => x.PincodeID == PincodeId).ToList();
          // ViewBag.Area = db.Cities.Single(c => c.ID == CityId).Name.ToString();
           ViewBag.Area = new SelectList(NewArea, "ID", "Name");
           //ViewBag.AreaID = new SelectList(NewArea, "ID", "Name");
            //return Json("0", JsonRequestBehavior.AllowGet);
           return PartialView("_Address");
           //return View();
        }

        public JsonResult GetAreaIDByPincodeID(int Pincode)
        {
           
            List<Area> larea = new List<Area>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();
            larea = db.Areas.Where(x => x.PincodeID == Pincode).ToList();
            foreach (var c in larea)
            {
                ForLoopClass av = new ForLoopClass();
                av.ID = c.ID;
                av.Name = c.Name;
                forloopclasses.Add(av);
            }

            return Json(forloopclasses.Distinct().OrderBy(x => x.Name).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}
