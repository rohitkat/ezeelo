using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using Merchant.Models;

namespace Merchant.Controllers
{
    public class Frmhours
    {
        public long ID { get; set; }
        public string Name { get; set; }
    }

    public class Tohours
    {
        public long ID { get; set; }
        public string Name { get; set; }
    }
    public class OfferDurationController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public static long ShopID;

        List<Frmhours> lFrmhours = new List<Frmhours>();
        List<Frmhours> lFrommin = new List<Frmhours>();
        List<Tohours> lTohours = new List<Tohours>();
        List<Tohours> lTomin = new List<Tohours>();
        // GET: /OfferDuration/

        [SessionExpire]
        public ActionResult Index()
        {
            try
            {
                ShopID = GetShopID();
                var offerdurations = (from O in db.Offers
                                      join
                                          OD in db.OfferDurations on O.ID equals OD.OfferID
                                      where O.OwnerID == ShopID
                                      select new ProductOfferViewModel
                                      {
                                          OfferID = O.ID,
                                          OfferName = O.ShortName,
                                          StartDateTime = OD.StartDateTime,
                                          EndDateTime = OD.EndDateTime,

                                      }).ToList();
                return View(offerdurations.ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Loading Index view of Offer Duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ OfferDuration][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in Loading Index view of Offer Duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ OfferDuration][POST:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        private long GetShopID()
        {

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
                throw new BusinessLogicLayer.MyException("[OfferDuration][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
            }
            return ShopID;
        }

        // GET: /OfferDuration/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfferDuration offerduration = db.OfferDurations.Find(id);
            if (offerduration == null)
            {
                return HttpNotFound();
            }
            return View(offerduration);
        }
        [SessionExpire]
        // GET: /OfferDuration/Create
        public ActionResult Create()
        {
            try
            {
                var Frmhours = new List<SelectListItem>();
                var Frmmin = new List<SelectListItem>();

                for (var i = 1; i <= 12; i++)
                    Frmhours.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                for (var i = 0; i <= 60; i++)
                    Frmmin.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                ViewBag.Frmhours = Frmhours;
                ViewBag.Frmmin = Frmmin;
                ViewBag.Tohours = Frmhours;
                ViewBag.Tommin = Frmmin;
                ViewBag.OfferID = new SelectList((from O in db.Offers
                                                  where O.OwnerID == ShopID
                                                  select new
                                                  {
                                                      ID = O.ID,
                                                      ShortName = O.ShortName

                                                  }).ToList(), "ID", "ShortName");
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Loading Create view of Offer Duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ OfferDuration][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in Loading Create view of Offer Duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ OfferDuration][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /OfferDuration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [HttpPost]

        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OfferID,ddlFrmHour,ddlFrmMin,ddlFrmAmPm,ddlToHour,ddlToMin,ddlToAmPm,StartDateTime,EndDateTime,IsActive")] OfferDuration offerduration, FormCollection form)
        {
            try
            {
                int FrmHr = 0;
                int FrmMin = 0;
                string FrmAmPm = "";
                int ToHr = 0;
                int ToMin = 0;
                string ToAmPm = "";
                string FromDate = "";
                DateTime strDate = new DateTime();
                string ToDate = "";
                DateTime EndDate = new DateTime();

                var Frmhours = new List<SelectListItem>();
                var Frmmin = new List<SelectListItem>();
                if (form["ddlFrmHour"] != null)
                {
                    FrmHr = Convert.ToInt32(form["ddlFrmHour"]);
                }
                if (form["ddlFrmMin"] != null)
                {
                    FrmMin = Convert.ToInt32(form["ddlFrmMin"]);
                }
                if (form["ddlFrmAmPm"] != "")
                {
                    FrmAmPm = form["ddlFrmAmPm"];
                }
                if (form["ddlToHour"] != "")
                {
                    ToHr = Convert.ToInt32(form["ddlToHour"]);
                }
                if (form["ddlToMin"] != "")
                {
                    ToMin = Convert.ToInt32(form["ddlToMin"]);
                }
                if (form["ddlToAmPm"] != "")
                {
                    ToAmPm = form["ddlToAmPm"];
                }
                //section for to get from Date
                //***********************************************************
                if (form["StartDateTime"] != null)
                {
                    FromDate = form["StartDateTime"].ToString();
                    string[] frmDate = FromDate.Split('/');
                    int frmyear = Convert.ToInt32(frmDate[2]);
                    int frmMonth = Convert.ToInt32(frmDate[1]);
                    int frmday = Convert.ToInt32(frmDate[0]);
                    if (FrmHr == 12)
                    {
                        FrmHr = FrmHr % 12;
                    }
                    FrmHr = FrmAmPm == "AM" ? FrmHr : (FrmHr % 12) + 12; //convert 12-hour time to 24-hour
                    strDate = new DateTime(frmyear, frmMonth, frmday, FrmHr, FrmMin, 0);
                }

                //****************************************************************************************
                //section for to get To Date
                //***********************************************************
                if (form["EndDateTime"] != null)
                {
                    ToDate = form["EndDateTime"].ToString();
                    string[] tDate = ToDate.Split('/');
                    int tyear = Convert.ToInt32(tDate[2]);
                    int tMonth = Convert.ToInt32(tDate[1]);
                    int tday = Convert.ToInt32(tDate[0]);
                    if (ToHr == 12)
                    {
                        ToHr = FrmHr % 12;
                    }
                    ToHr = ToAmPm == "AM" ? ToHr : (ToHr % 12) + 12; //convert 12-hour time to 24-hour
                    EndDate = new DateTime(tyear, tMonth, tday, ToHr, ToMin, 0);
                }

                ViewBag.OfferID = new SelectList(db.Offers, "ID", "ShortName", offerduration.OfferID);
                for (var i = 1; i <= 12; i++)
                    Frmhours.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                for (var i = 0; i <= 60; i++)
                    Frmmin.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                ViewBag.Frmhours = Frmhours;
                ViewBag.Frmmin = Frmmin;
                ViewBag.Tohours = Frmhours;
                ViewBag.Tommin = Frmmin;
                //****************************************************************************************
                offerduration.StartDateTime = strDate;
                offerduration.EndDateTime = EndDate;
                offerduration.CreateDate = DateTime.UtcNow;
                offerduration.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["USER_LOGIN_ID"]));
                offerduration.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                offerduration.DeviceID = "x";
                offerduration.DeviceType = "x";
                //if (ModelState.IsValid)
                //{
                long OfferID = Convert.ToInt64(form["OfferID"]);
                long ID = db.OfferDurations.Where(x => x.OfferID == OfferID).Select(x => x.ID).FirstOrDefault();
                if (ID == 0)
                {
                    db.OfferDurations.Add(offerduration);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Error", "Duration for this offer is alraedy exist.Please select other offer.");
                    return View(offerduration);
                }
                //}


                //return View(offerduration);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in saving offer duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ OfferDuration][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in saving offer duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ OfferDuration][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /OfferDuration/Edit/5
        [SessionExpire]
        public ActionResult Edit(int? id)
        {
            //ViewBag.OfferID = db.Offers.Where(x => x.ID == offerduration.OfferID).Select(x => x.ShortName).FirstOrDefault();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                OfferDuration offerduration = db.OfferDurations.Where(x => x.OfferID == id).FirstOrDefault();
                if (offerduration == null)
                {
                    return HttpNotFound();
                }
                string strtDate = offerduration.StartDateTime.ToString();
                string[] startDate = strtDate.Split(' ');
                string[] starthour = startDate[1].Split(':');
                int sthr = Convert.ToInt32(starthour[0]);
                int stmin = Convert.ToInt32(starthour[1]);
                string stAmPm = startDate[2];


                string endDate = offerduration.EndDateTime.ToString();
                string[] enDate = endDate.Split(' ');
                string[] endhour = enDate[1].Split(':');
                int ehr = Convert.ToInt32(endhour[0]);
                int emin = Convert.ToInt32(endhour[1]);
                string eAmPm = enDate[2];


                int[] numberhr = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                int[] numbermin = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39
                ,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60};
                lFrmhours = (from n in numberhr
                             select new Frmhours
                                 {
                                     ID = n,
                                     Name = n.ToString()
                                 }).ToList();
                ViewBag.ddlFrmHour = new SelectList(lFrmhours, "ID", "Name", sthr);

                lFrommin = (from n in numbermin
                            select new Frmhours
                            {
                                ID = n,
                                Name = n.ToString()
                            }).ToList();
                ViewBag.ddlFrmMin = new SelectList(lFrommin, "ID", "Name", stmin);
                ViewBag.ddlFrmAmPm = stAmPm;

                lTohours = (from n in numberhr
                            select new Tohours
                            {
                                ID = n,
                                Name = n.ToString()
                            }).ToList();
                ViewBag.ddlToHour = new SelectList(lTohours, "ID", "Name", ehr);

                lTomin = (from n in numbermin
                          select new Tohours
                            {
                                ID = n,
                                Name = n.ToString()
                            }).ToList();
                ViewBag.ddlToMin = new SelectList(lTomin, "ID", "Name", emin);
                ViewBag.OfferID = new SelectList(db.Offers, "ID", "ShortName", offerduration.OfferID);
                ViewBag.ddlToAmPm = eAmPm;
                return View(offerduration);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Edit view of offer duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ OfferDuration][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in Edit view of offer duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ OfferDuration][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /OfferDuration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,OfferID,ddlFrmHour,ddlFrmMin,ddlFrmAmPm,ddlToHour,ddlToMin,ddlToAmPm,StartDateTime,EndDateTime,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] OfferDuration offerduration, FormCollection form)
        {
            try
            {

                int FrmHr = 0;
                int FrmMin = 0;
                string FrmAmPm = "";
                int ToHr = 0;
                int ToMin = 0;
                string ToAmPm = "";
                string FromDate = "";
                DateTime strDate = new DateTime();
                string ToDate = "";
                DateTime EndDate = new DateTime();

                var Frmhours = new List<SelectListItem>();
                var Frmmin = new List<SelectListItem>();
                if (form["ddlFrmHour"] != null)
                {
                    FrmHr = Convert.ToInt32(form["ddlFrmHour"]);
                }
                if (form["ddlFrmMin"] != null)
                {
                    FrmMin = Convert.ToInt32(form["ddlFrmMin"]);
                }
                if (form["ddlFrmAmPm"] != "")
                {
                    FrmAmPm = form["ddlFrmAmPm"];
                }
                if (form["ddlToHour"] != "")
                {
                    ToHr = Convert.ToInt32(form["ddlToHour"]);
                }
                if (form["ddlToMin"] != "")
                {
                    ToMin = Convert.ToInt32(form["ddlToMin"]);
                }
                if (form["ddlToAmPm"] != "")
                {
                    ToAmPm = form["ddlToAmPm"];
                }
                //section for to get from Date
                //***********************************************************
                if (form["StartDateTime"] != null)
                {
                    FromDate = form["StartDateTime"].ToString();
                    string[] frmDate = FromDate.Split('/');
                    int frmyear = Convert.ToInt32(frmDate[2]);
                    int frmMonth = Convert.ToInt32(frmDate[1]);
                    int frmday = Convert.ToInt32(frmDate[0]);
                    if (FrmHr == 12)
                    {
                        FrmHr = FrmHr % 12;
                    }
                    FrmHr = FrmAmPm == "AM" ? FrmHr : (FrmHr % 12) + 12; //convert 12-hour time to 24-hour
                    strDate = new DateTime(frmyear, frmMonth, frmday, FrmHr, FrmMin, 0);
                }
                //****************************************************************************************
                //section for to get To Date
                //***********************************************************
                if (form["EndDateTime"] != null)
                {
                    ToDate = form["EndDateTime"].ToString();
                    string[] tDate = ToDate.Split('/');
                    int tyear = Convert.ToInt32(tDate[2]);
                    int tMonth = Convert.ToInt32(tDate[1]);
                    int tday = Convert.ToInt32(tDate[0]);
                    if (ToHr == 12)
                    {
                        ToHr = FrmHr % 12;
                    }
                    ToHr = ToAmPm == "AM" ? ToHr : (ToHr % 12) + 12; //convert 12-hour time to 24-hour
                    EndDate = new DateTime(tyear, tMonth, tday, ToHr, ToMin, 0);
                }

                long OfferID = Convert.ToInt64(form["OfferID"]);
                OfferDuration OfferDuration = db.OfferDurations.Where(x => x.OfferID == OfferID).FirstOrDefault();
                offerduration.ID = OfferDuration.ID;
                offerduration.StartDateTime = strDate;
                offerduration.EndDateTime = EndDate;
                offerduration.CreateDate = OfferDuration.CreateDate;
                offerduration.CreateBy = OfferDuration.CreateBy;
                offerduration.ModifyDate = DateTime.UtcNow;
                offerduration.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["USER_LOGIN_ID"]));



                //******************************************************************************//


                int[] numberhr = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                int[] numbermin = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39
                ,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60};
                lFrmhours = (from n in numberhr
                             select new Frmhours
                             {
                                 ID = n,
                                 Name = n.ToString()
                             }).ToList();
                ViewBag.ddlFrmHour = new SelectList(lFrmhours, "ID", "Name", FrmHr);

                lFrommin = (from n in numbermin
                            select new Frmhours
                            {
                                ID = n,
                                Name = n.ToString()
                            }).ToList();
                ViewBag.ddlFrmMin = new SelectList(lFrommin, "ID", "Name", FrmMin);
                ViewBag.ddlFrmAmPm = FrmAmPm;

                lTohours = (from n in numberhr
                            select new Tohours
                            {
                                ID = n,
                                Name = n.ToString()
                            }).ToList();
                ViewBag.ddlToHour = new SelectList(lTohours, "ID", "Name", ToHr);

                lTomin = (from n in numbermin
                          select new Tohours
                          {
                              ID = n,
                              Name = n.ToString()
                          }).ToList();
                ViewBag.ddlToMin = new SelectList(lTomin, "ID", "Name", ToMin);
                //ViewBag.OfferID = new SelectList(db.Offers, "ID", "ShortName", offerduration.OfferID);
                ViewBag.ddlToAmPm = ToAmPm;
                //******************************************************************************//
                //if (ModelState.IsValid)
                //{
                db.Entry(OfferDuration).CurrentValues.SetValues(offerduration);
                db.SaveChanges();
                ViewBag.OfferID = new SelectList(db.Offers, "ID", "ShortName", offerduration.OfferID);
                return RedirectToAction("Index");
                //}


                //return View(offerduration);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Edit view of offer duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ OfferDuration][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in Edit view of offer duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ OfferDuration][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /OfferDuration/Delete/5
        [SessionExpire]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfferDuration offerduration = db.OfferDurations.Find(id);
            if (offerduration == null)
            {
                return HttpNotFound();
            }
            return View(offerduration);
        }

        // POST: /OfferDuration/Delete/5
        [SessionExpire]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OfferDuration offerduration = db.OfferDurations.Find(id);
            db.OfferDurations.Remove(offerduration);
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
    }
}
