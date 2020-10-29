using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusinessLogicLayer;
using Merchant.Models;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;

namespace Merchant.Controllers
{
    public class MasterComponentOfferController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public static long ShopID;
        // GET: /MasterComponentOffer/
        [SessionExpire]
        public ActionResult Index()
        {
            try
            {
                ShopID = GetShopID();
                var componentoffers = db.ComponentOffers.Where(x => x.ShopID == ShopID);
                return View(componentoffers.ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading component Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterComponentOffer][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in loading component Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterComponentOffer][POST:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();

        }
        [SessionExpire]
        private long GetShopID()
        {
            EzeeloDBContext db = new EzeeloDBContext();
            //Session["USER_LOGIN_ID"] = 2;
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
                throw new BusinessLogicLayer.MyException("[MasterOfferController][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
            }
            return ShopID;
        }
        // GET: /MasterComponentOffer/Details/5

        [SessionExpire]

        // GET: /MasterComponentOffer/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.ComponentID = new SelectList((from c in db.Components
                                                      join scp in db.ShopComponentPrices
                                                          on c.ID equals scp.ComponentID
                                                      where scp.ShopID == ShopID
                                                      select new ProductComponentViewModel { ComponentID = c.ID, ComponentName = c.Name }).ToList(), "ComponentID", "ComponentName");
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in creating component Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterComponentOffer][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in creating component Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterComponentOffer][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /MasterComponentOffer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ShortName,ShopID,ComponentID,OfferInRs,OfferInPercent,Description,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] ComponentOffer componentoffer)
        {
            try
            {
                componentoffer.ShopID = GetShopID();
                componentoffer.CreateDate = DateTime.UtcNow;
                componentoffer.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["USER_LOGIN_ID"]));
                componentoffer.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                componentoffer.DeviceID = "x";
                componentoffer.DeviceType = "x";
                if (ModelState.IsValid)
                {
                    db.ComponentOffers.Add(componentoffer);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }


                ViewBag.ComponentID = new SelectList((from c in db.Components
                                                      join scp in db.ShopComponentPrices
                                                          on c.ID equals scp.ComponentID
                                                      where scp.ShopID == ShopID
                                                      select new ProductComponentViewModel { ComponentID = c.ID, ComponentName = c.Name }).ToList(), "ComponentID", "ComponentName");

                return View(componentoffer);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in creating component Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterComponentOffer][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in creating component Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterComponentOffer][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /MasterComponentOffer/Edit/5
        [SessionExpire]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ComponentOffer componentoffer = db.ComponentOffers.Find(id);
                if (componentoffer == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ComponentID = new SelectList(db.Components, "ID", "Name", componentoffer.ComponentID);

                return View(componentoffer);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Editing component Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterComponentOffer][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in Editing component Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterComponentOffer][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /MasterComponentOffer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ShortName,ShopID,ComponentID,OfferInRs,OfferInPercent,Description,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] ComponentOffer componentoffer)
        {
            try
            {
                ComponentOffer Offer = db.ComponentOffers.Find(componentoffer.ID);
                componentoffer.ShopID = Offer.ShopID;
                componentoffer.ComponentID = Offer.ComponentID;
                componentoffer.CreateDate = Offer.CreateDate;
                componentoffer.CreateBy = Offer.CreateBy;
                componentoffer.ModifyDate = DateTime.UtcNow;
                componentoffer.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["USER_LOGIN_ID"]));
                if (ModelState.IsValid)
                {
                    db.Entry(Offer).CurrentValues.SetValues(componentoffer);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.ComponentID = new SelectList(db.Components, "ID", "Name", componentoffer.ComponentID);

                return View(componentoffer);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Editing component Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterComponentOffer][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in Editing component Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterComponentOffer][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /MasterComponentOffer/Delete/5
        [SessionExpire]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ComponentOffer componentoffer = db.ComponentOffers.Find(id);
                if (componentoffer == null)
                {
                    return HttpNotFound();
                }
                return View(componentoffer);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Deleting component Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterComponentOffer][POST:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in Deleting component Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterComponentOffer][POST:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /MasterComponentOffer/Delete/5
        [SessionExpire]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                ComponentOffer componentoffer = db.ComponentOffers.Find(id);
                long ID = db.StockComponentOffers.Where(x => x.ComponentOfferID == id).Select(x => x.ID).FirstOrDefault();
                long OfferDurationID = db.StockComponentOfferDurations.Where(x => x.ComponentOfferID == id).Select(x => x.ID).FirstOrDefault();
                if (ID > 0)
                {
                    ModelState.AddModelError("Error", "You can not delete this Offer because it is applied on various product.");
                    return View(componentoffer);

                }
                else if (OfferDurationID > 0)
                {
                    ModelState.AddModelError("Error", "You can not delete this Offer because its duration is present");
                    return View(componentoffer);
                }
                else
                {
                    db.ComponentOffers.Remove(componentoffer);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Deleting component Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MasterComponentOffer][POST:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in Deleting component Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MasterComponentOffer][POST:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
