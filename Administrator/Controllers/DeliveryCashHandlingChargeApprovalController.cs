using Administrator.Models;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Net;
using System.Data.Entity;
using BusinessLogicLayer;


namespace Administrator.Controllers
{
    public class DeliveryCashHandlingChargeApprovalController : Controller
    {
        

        private EzeeloDBContext db = new EzeeloDBContext();
        private DeliveryPartnerSessionViewModel deliveryPartnerSessionViewModel = new DeliveryPartnerSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails(long UID)
        {
            long PID = CommonFunctions.GetPersonalDetailsID(UID);
            deliveryPartnerSessionViewModel.UserLoginID = UID;
            deliveryPartnerSessionViewModel.Username = db.PersonalDetails.Find(PID).FirstName + " " + db.PersonalDetails.Find(PID).LastName;
            Common.GetAllLoginDetailFromSession(ref deliveryPartnerSessionViewModel);
        }

        //
        // GET: /DeliveryCashHandlingChargeApproval/
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryCashHandlingChargeApproval/CanRead")]
        public ActionResult DPList()
        {
            try
            {
                var lMav = (from ul in db.UserLogins
                            join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                            join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                            join dp in db.DeliveryPartners on bd.ID equals dp.BusinessDetailID
                            join chc in db.DeliveryCashHandlingCharges on dp.ID equals chc.DeliveryPartnerID
                            where bd.BusinessType.Prefix == "GBDP" && ul.IsLocked == false &&
                            dp.IsLive == true && dp.IsActive == true && chc.IsApproved == false && chc.IsActive==true
                            select new DeliveryPartnerApprovalViewModel
                            {
                                UserLoginID = bd.UserLoginID,
                                BusinessTypePrefix = bd.BusinessType.Prefix,
                                Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName,
                                OwnerId = dp.ID
                            }).Distinct();

                return View(lMav);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DeliveryCashHandlingChargeApproval][GET:DPList]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryCashHandlingChargeApproval][GET:DPList]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryCashHandlingChargeApproval/CanRead")]
        public ActionResult Index(long UID, int? page, string SearchString = "")
        {
            SessionDetails(UID);
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;
            ViewBag.UID = UID;
            var deliverycashhandlingcharges = db.DeliveryCashHandlingCharges.Include(d => d.DeliveryPartner).Include(d => d.PersonalDetail).Include(d => d.PersonalDetail1).Include(d => d.PersonalDetail2).Where(x => x.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID && x.IsApproved == false).ToList().OrderBy(x => x.MaxAmount);

            if (SearchString != "")
            {
                return View(deliverycashhandlingcharges.Where(x => x.MaxAmount.ToString() == SearchString).ToPagedList(pageNumber, pageSize));

            }
            return View(deliverycashhandlingcharges.ToPagedList(pageNumber, pageSize));
        }

        // GET: /DeliveryCashHandlingChargeApproval/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryCashHandlingChargeApproval/CanRead")]
        public ActionResult Details(long? id, long UID)
        {
            SessionDetails(UID);
            ViewBag.UID = UID;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DeliveryCashHandlingCharge deliverycashhandlingcharge = db.DeliveryCashHandlingCharges.Find(id);
            if (deliverycashhandlingcharge == null)
            {
                return HttpNotFound();
            }
            if (deliverycashhandlingcharge.DeliveryPartnerID != deliveryPartnerSessionViewModel.DeliveryPartnerID)
            {
                return View("AccessDenied");
            }
            return PartialView("_Details", deliverycashhandlingcharge);
        }


        // GET: /DeliveryCashHandlingChargeApproval/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryCashHandlingChargeApproval/CanRead")]
        public ActionResult Edit(long? id, long UID)
        {
            SessionDetails(UID);
            ViewBag.UID = UID;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryCashHandlingCharge deliverycashhandlingcharge = db.DeliveryCashHandlingCharges.Find(id);
            if (deliverycashhandlingcharge == null)
            {
                return HttpNotFound();
            }
            if (deliverycashhandlingcharge.DeliveryPartnerID != deliveryPartnerSessionViewModel.DeliveryPartnerID)
            {
                return View("AccessDenied");
            }

            ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", deliverycashhandlingcharge.DeliveryPartnerID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.CreateBy);
            ViewBag.ApprovedBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.ApprovedBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.ModifyBy);
            return View(deliverycashhandlingcharge);
        }

        // POST: /DeliveryCashHandlingChargeApproval/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryCashHandlingChargeApproval/CanWrite")]
        public ActionResult Edit([Bind(Include = "ID,DeliveryPartnerID,MaxAmount,PerHourCharge,IsActive")] DeliveryCashHandlingCharge deliverycashhandlingcharge, long UID)
        {
            SessionDetails(UID);
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                DeliveryCashHandlingCharge lDeliveryCashHandlingCharge = db1.DeliveryCashHandlingCharges.Find(deliverycashhandlingcharge.ID);
                deliverycashhandlingcharge.CreateBy = lDeliveryCashHandlingCharge.CreateBy;
                deliverycashhandlingcharge.CreateDate = lDeliveryCashHandlingCharge.CreateDate;
                deliverycashhandlingcharge.ModifyBy = deliveryPartnerSessionViewModel.PersonalDetailID;
                deliverycashhandlingcharge.ModifyDate = DateTime.Now;
                deliverycashhandlingcharge.IsApproved = false;
                deliverycashhandlingcharge.ApprovedBy = null;
                deliverycashhandlingcharge.DeliveryPartnerID = lDeliveryCashHandlingCharge.DeliveryPartnerID;
                db1.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(deliverycashhandlingcharge).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { UID = UID });
                }
                ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", deliverycashhandlingcharge.DeliveryPartnerID);
                ViewBag.ApprovedBy = new SelectList(db.UserLogins, "ID", "Mobile", deliverycashhandlingcharge.ApprovedBy);
                return View(deliverycashhandlingcharge);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the delivery cash handling charge values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryCashHandlingChargeApproval][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);

                ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", deliverycashhandlingcharge.DeliveryPartnerID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.CreateBy);
                ViewBag.ApprovedBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.ApprovedBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliverycashhandlingcharge.ModifyBy);
                return View(deliverycashhandlingcharge);
            }
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryCashHandlingChargeApproval/CanWrite")]
        public ActionResult Approve(int? id, long UID)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    DeliveryCashHandlingCharge dchc = db.DeliveryCashHandlingCharges.Find(id);
                    WriteToLogTable(dchc, ModelLayer.Models.Enum.COMMAND.UPDATE);
                    dchc.IsApproved = true;
                    dchc.ApprovedBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    dchc.ModifyDate = DateTime.UtcNow;
                    dchc.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    TryUpdateModel(dchc);

                    if (ModelState.IsValid)
                    {
                        db.Entry(dchc).State = EntityState.Modified;
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        return RedirectToAction("Index", new { UID = UID });
                    }
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[DeliveryCashHandlingChargeApproval][POST:Approve]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[DeliveryCashHandlingChargeApproval][POST:Approve]",
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                }
            }
            return RedirectToAction("Index", new { UID = UID });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void WriteToLogTable(DeliveryCashHandlingCharge obj, ModelLayer.Models.Enum.COMMAND mode)
        {
            try
            {
                //Log Table Insertion
                //LogTable logTable = new LogTable();
                //logTable.TableName = "DeliveryCashHandlingCharge";//table Name(Model Name)
                //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(obj);
                //logTable.TableRowID = obj.ID;
                //logTable.Command = mode.ToString();
                //long? rowOwnerID = (obj.ModifyBy >= 0 ? obj.ModifyBy : obj.CreateBy);
                //logTable.RowOwnerID = (long)rowOwnerID;
                //logTable.CreateDate = DateTime.UtcNow;
                //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                //db.LogTables.Add(logTable);
                /**************************************/
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                     + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                     + "[DeliveryCashHandlingCharge][WriteToLogTable]" + myEx.EXCEPTION_PATH,
                     BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryCashHandlingCharge][WriteToLogTable]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
        }
	}
}