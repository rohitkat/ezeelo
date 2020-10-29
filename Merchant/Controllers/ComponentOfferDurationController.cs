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

    public class CFrmhours
    {
        public long ID { get; set; }
        public string Name { get; set; }
    }

    public class CTohours
    {
        public long ID { get; set; }
        public string Name { get; set; }
    }
    public class ComponentOfferDurationController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public static long ShopID;
        List<CFrmhours> lFrmhours = new List<CFrmhours>();
        List<CFrmhours> lFrommin = new List<CFrmhours>();
        List<CTohours> lTohours = new List<CTohours>();
        List<CTohours> lTomin = new List<CTohours>();
        // GET: /ComponentOfferDuration/
        [SessionExpire]
        public ActionResult Index()
        {
            try
            {
                ShopID = GetShopID();
                var stockcomponentofferdurations = (from CO in db.ComponentOffers
                                                    join
                                                        SCOD in db.StockComponentOfferDurations on CO.ID equals SCOD.ComponentOfferID
                                                    where CO.ShopID == ShopID
                                                    select new ProductOfferViewModel
                                                    {
                                                        OfferID = CO.ID,
                                                        OfferName = CO.ShortName,
                                                        StartDateTime = SCOD.StartDateTime,
                                                        EndDateTime = SCOD.EndDateTime,

                                                    }).ToList();
                return View(stockcomponentofferdurations.ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Loading Index view of Component Offer Duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ ComponentOfferDuration][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in Loading Index view of Component Offer Duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ ComponentOfferDuration][POST:Index]",
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
        // GET: /ComponentOfferDuration/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockComponentOfferDuration stockcomponentofferduration = db.StockComponentOfferDurations.Find(id);
            if (stockcomponentofferduration == null)
            {
                return HttpNotFound();
            }
            return View(stockcomponentofferduration);
        }

        // GET: /ComponentOfferDuration/Create
        [SessionExpire]
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
                ViewBag.OfferID = new SelectList((from CO in db.ComponentOffers
                                                  where CO.ShopID == ShopID
                                                  select new
                                                  {
                                                      ID = CO.ID,
                                                      ShortName = CO.ShortName

                                                  }).ToList(), "ID", "ShortName");
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Loading Create view of Component Offer Duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ ComponentOfferDuration][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in Loading Component Create view of Offer Duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ ComponentOfferDuration][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();

        }

        // POST: /ComponentOfferDuration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,OfferID,ddlFrmHour,ddlFrmMin,ddlFrmAmPm,ddlToHour,ddlToMin,ddlToAmPm,StartDateTime,EndDateTime,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] StockComponentOfferDuration stockcomponentofferduration, FormCollection form)
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

                ViewBag.OfferID = new SelectList(db.ComponentOffers, "ID", "ShortName", stockcomponentofferduration.ComponentOfferID);
                for (var i = 1; i <= 12; i++)
                    Frmhours.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                for (var i = 0; i <= 60; i++)
                    Frmmin.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                ViewBag.Frmhours = Frmhours;
                ViewBag.Frmmin = Frmmin;
                ViewBag.Tohours = Frmhours;
                ViewBag.Tommin = Frmmin;
                //****************************************************************************************
                int OfferID = Convert.ToInt32(form["OfferID"]);
                stockcomponentofferduration.ComponentOfferID = OfferID;
                stockcomponentofferduration.StartDateTime = strDate;
                stockcomponentofferduration.EndDateTime = EndDate;
                stockcomponentofferduration.CreateDate = DateTime.UtcNow;
                stockcomponentofferduration.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["USER_LOGIN_ID"]));
                stockcomponentofferduration.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                stockcomponentofferduration.DeviceID = "x";
                stockcomponentofferduration.DeviceType = "x";
                //if (ModelState.IsValid)
                //{

                long ID = db.StockComponentOfferDurations.Where(x => x.ComponentOfferID == OfferID).Select(x => x.ID).FirstOrDefault();
                if (ID == 0)
                {
                    db.StockComponentOfferDurations.Add(stockcomponentofferduration);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Error", "Duration for this offer is alraedy exist.Please select other offer.");
                    return View(stockcomponentofferduration);
                }
                //return View(stockcomponentofferduration);
                //}

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Loading Create view of Component Offer Duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ ComponentOfferDuration][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in Loading Component Create view of Offer Duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ ComponentOfferDuration][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /OfferDuration/Edit/5
        // GET: /ComponentOfferDuration/Edit/5
        [SessionExpire]
        public ActionResult Edit(int? id)
        {


            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                StockComponentOfferDuration stockcomponentofferduration = db.StockComponentOfferDurations.Where(x => x.ComponentOfferID == id).FirstOrDefault();
                if (stockcomponentofferduration == null)
                {
                    return HttpNotFound();
                }
                string strtDate = stockcomponentofferduration.StartDateTime.ToString();
                string[] startDate = strtDate.Split(' ');
                string[] starthour = startDate[1].Split(':');
                int sthr = Convert.ToInt32(starthour[0]);
                int stmin = Convert.ToInt32(starthour[1]);
                string stAmPm = startDate[2];


                string endDate = stockcomponentofferduration.EndDateTime.ToString();
                string[] enDate = endDate.Split(' ');
                string[] endhour = enDate[1].Split(':');
                int ehr = Convert.ToInt32(endhour[0]);
                int emin = Convert.ToInt32(endhour[1]);
                string eAmPm = enDate[2];


                int[] numberhr = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                int[] numbermin = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39
                ,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60};
                lFrmhours = (from n in numberhr
                             select new CFrmhours
                                 {
                                     ID = n,
                                     Name = n.ToString()
                                 }).ToList();
                ViewBag.ddlFrmHour = new SelectList(lFrmhours, "ID", "Name", sthr);

                lFrommin = (from n in numbermin
                            select new CFrmhours
                            {
                                ID = n,
                                Name = n.ToString()
                            }).ToList();
                ViewBag.ddlFrmMin = new SelectList(lFrommin, "ID", "Name", stmin);
                ViewBag.ddlFrmAmPm = stAmPm;

                lTohours = (from n in numberhr
                            select new CTohours
                            {
                                ID = n,
                                Name = n.ToString()
                            }).ToList();
                ViewBag.ddlToHour = new SelectList(lTohours, "ID", "Name", ehr);

                lTomin = (from n in numbermin
                          select new CTohours
                            {
                                ID = n,
                                Name = n.ToString()
                            }).ToList();
                ViewBag.ddlToMin = new SelectList(lTomin, "ID", "Name", emin);
                ViewBag.OfferID = new SelectList(db.ComponentOffers, "ID", "ShortName", stockcomponentofferduration.ComponentOfferID);
                ViewBag.ddlToAmPm = eAmPm;
                return View(stockcomponentofferduration);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Loading Edit view of Component Offer Duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ ComponentOfferDuration][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in Loading Component Edit view of Offer Duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ ComponentOfferDuration][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();

        }


        // POST: /ComponentOfferDuration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,OfferID,ddlFrmHour,ddlFrmMin,ddlFrmAmPm,ddlToHour,ddlToMin,ddlToAmPm,StartDateTime,EndDateTime,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] StockComponentOfferDuration stockcomponentofferduration, FormCollection form)
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

                long ComponentOfferID = Convert.ToInt64(form["ComponentOfferID"]);
                StockComponentOfferDuration StockComponentOfferDuration = db.StockComponentOfferDurations.Where(x => x.ComponentOfferID == ComponentOfferID).FirstOrDefault();
                stockcomponentofferduration.ID = StockComponentOfferDuration.ID;
                stockcomponentofferduration.ComponentOfferID = StockComponentOfferDuration.ComponentOfferID;
                stockcomponentofferduration.StartDateTime = strDate;
                stockcomponentofferduration.EndDateTime = EndDate;
                stockcomponentofferduration.CreateDate = StockComponentOfferDuration.CreateDate;
                stockcomponentofferduration.CreateBy = StockComponentOfferDuration.CreateBy;
                stockcomponentofferduration.ModifyDate = DateTime.UtcNow;
                stockcomponentofferduration.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["USER_LOGIN_ID"]));

                //******************************************************************************//


                int[] numberhr = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                int[] numbermin = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39
                ,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60};
                lFrmhours = (from n in numberhr
                             select new CFrmhours
                             {
                                 ID = n,
                                 Name = n.ToString()
                             }).ToList();
                ViewBag.ddlFrmHour = new SelectList(lFrmhours, "ID", "Name", FrmHr);

                lFrommin = (from n in numbermin
                            select new CFrmhours
                            {
                                ID = n,
                                Name = n.ToString()
                            }).ToList();
                ViewBag.ddlFrmMin = new SelectList(lFrommin, "ID", "Name", FrmMin);
                ViewBag.ddlFrmAmPm = FrmAmPm;

                lTohours = (from n in numberhr
                            select new CTohours
                            {
                                ID = n,
                                Name = n.ToString()
                            }).ToList();
                ViewBag.ddlToHour = new SelectList(lTohours, "ID", "Name", ToHr);

                lTomin = (from n in numbermin
                          select new CTohours
                          {
                              ID = n,
                              Name = n.ToString()
                          }).ToList();
                ViewBag.ddlToMin = new SelectList(lTomin, "ID", "Name", ToMin);
                ViewBag.OfferID = new SelectList(db.ComponentOffers, "ID", "ShortName", stockcomponentofferduration.ComponentOfferID);
                ViewBag.ddlToAmPm = ToAmPm;
                //******************************************************************************//
                //if (ModelState.IsValid)
                //{
                db.Entry(StockComponentOfferDuration).CurrentValues.SetValues(stockcomponentofferduration);
                //db.Entry(offerduration).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.OfferID = new SelectList(db.ComponentOffers, "ID", "ShortName", stockcomponentofferduration.ComponentOfferID);
                return RedirectToAction("Index");
                //}


                //return View(stockcomponentofferduration);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in  Edit view of Component Offer Duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ ComponentOfferDuration][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in  Edit view of Component Offer Duration!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ ComponentOfferDuration][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /ComponentOfferDuration/Delete/5
        [SessionExpire]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockComponentOfferDuration stockcomponentofferduration = db.StockComponentOfferDurations.Find(id);
            if (stockcomponentofferduration == null)
            {
                return HttpNotFound();
            }
            return View(stockcomponentofferduration);
        }

        // POST: /ComponentOfferDuration/Delete/5
        [SessionExpire]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StockComponentOfferDuration stockcomponentofferduration = db.StockComponentOfferDurations.Find(id);
            db.StockComponentOfferDurations.Remove(stockcomponentofferduration);
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
