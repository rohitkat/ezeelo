using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using Administrator.Models;
using System.Net;
using System.Data.Entity;

namespace Administrator.Controllers
{
    public class DeliveryWeightSlabApprovalController : Controller
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
        // GET: /DeliveryWeightSlabApproval/
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryWeightSlabApproval/CanRead")]
        public ActionResult DPList()
        {
            try
            {
                var lMav = (from ul in db.UserLogins
                           join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
                           join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                           join dp in db.DeliveryPartners on bd.ID equals dp.BusinessDetailID
                           join ws in db.DeliveryWeightSlabs on dp.ID equals ws.DeliveryPartnerID
                           where bd.BusinessType.Prefix == "GBDP" && ul.IsLocked == false &&
                           dp.IsLive == true && dp.IsActive == true && ws.IsApproved == false 
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
                    + "[DeliveryPartnerApproval][GET:DPList]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryPartnerApproval][GET:DPList]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

       
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryWeightSlabApproval/CanRead")]
        public ActionResult Index(long UID, int? page, string SearchString = "")
        {
            SessionDetails(UID);
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;
            ViewBag.UID = UID;

            var deliveryweightslabs = db.DeliveryWeightSlabs.Include(d => d.DeliveryPartner).Include(d => d.PersonalDetail).Include(d => d.PersonalDetail1).Include(d => d.PersonalDetail2).Where(x => x.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID && x.IsApproved==false).ToList().OrderBy(x => x.MaxWeight);
            if (SearchString != "")
            {
                return View(deliveryweightslabs.Where(x => x.MaxWeight.ToString() == SearchString).ToPagedList(pageNumber, pageSize));

            }
            return View(deliveryweightslabs.ToPagedList(pageNumber, pageSize));
        }

        // GET: /DeliveryWeightSlabApproval/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryWeightSlabApproval/CanRead")]
        public ActionResult Details(int? id,long UID)
        {
            SessionDetails(UID);
            ViewBag.UID = UID;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryWeightSlab deliveryweightslab = db.DeliveryWeightSlabs.Find(id);
            if (deliveryweightslab == null)
            {
                return HttpNotFound();
            }
            if (deliveryweightslab.DeliveryPartnerID != deliveryPartnerSessionViewModel.DeliveryPartnerID)
            {
                return View("AccessDenied");
            }
            return PartialView("_Details", deliveryweightslab);
            //return View(deliveryweightslab);
        }


        // GET: /DeliveryWeightSlabApproval/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryWeightSlabApproval/CanRead")]
        public ActionResult Edit(int? id, long UID)
        {
            SessionDetails(UID);
            ViewBag.UID = UID;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeliveryWeightSlab deliveryweightslab = db.DeliveryWeightSlabs.Find(id);
            if (deliveryweightslab == null)
            {
                return HttpNotFound();
            }
            if (deliveryweightslab.DeliveryPartnerID != deliveryPartnerSessionViewModel.DeliveryPartnerID)
            {
                return View("AccessDenied");
            }
            ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", deliveryweightslab.DeliveryPartnerID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.CreateBy);
            ViewBag.ApprovedBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.ApprovedBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.ModifyBy);
            return View(deliveryweightslab);
        }

        // POST: /DeliveryWeightSlabApproval/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryWeightSlabApproval/CanWrite")]
        public ActionResult Edit([Bind(Include = "ID,MaxWeight,NormalRateWithinPincodeList,ExpressRateWithinPincodeList,ExpressRateOutOfPincodeList,NormalRateOutOfPincodeList,IsActive")] DeliveryWeightSlab deliveryweightslab, long UID)
        {
            SessionDetails(UID);
            try
            {
                EzeeloDBContext db1 = new EzeeloDBContext();
                DeliveryWeightSlab lDeliveryWeightSlab = db1.DeliveryWeightSlabs.Find(deliveryweightslab.ID);
                deliveryweightslab.CreateBy = lDeliveryWeightSlab.CreateBy;
                deliveryweightslab.CreateDate = lDeliveryWeightSlab.CreateDate;
                deliveryweightslab.ModifyBy = deliveryPartnerSessionViewModel.PersonalDetailID;
                deliveryweightslab.ModifyDate = DateTime.Now;
                deliveryweightslab.ApprovedBy = null;
                deliveryweightslab.IsApproved = false;
                deliveryweightslab.DeliveryPartnerID = lDeliveryWeightSlab.DeliveryPartnerID;
                db1.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(deliveryweightslab).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { UID = UID });
                }
                ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", deliveryweightslab.DeliveryPartnerID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.CreateBy);
                ViewBag.ApprovedBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.ApprovedBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.ModifyBy);
                return View(deliveryweightslab);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with the delivery weight slab values!");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryWeightSlab][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.DeliveryPartner, System.Web.HttpContext.Current.Server);

                ViewBag.DeliveryPartnerID = new SelectList(db.DeliveryPartners, "ID", "GodownAddress", deliveryweightslab.DeliveryPartnerID);
                ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.CreateBy);
                ViewBag.ApprovedBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.ApprovedBy);
                ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", deliveryweightslab.ModifyBy);
                return View(deliveryweightslab);
            }
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "DeliveryWeightSlabApproval/CanWrite")]
        public ActionResult Approve(int? id, long UID)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    DeliveryWeightSlab dw = db.DeliveryWeightSlabs.Find(id);
                    WriteToLogTable(dw, ModelLayer.Models.Enum.COMMAND.UPDATE);

                    dw.IsActive = true;
                    dw.IsApproved = true;
                    dw.ApprovedBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    dw.ModifyDate = DateTime.UtcNow;
                    dw.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    TryUpdateModel(dw);

                    if (ModelState.IsValid)
                    {
                        db.Entry(dw).State = EntityState.Modified;
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        return RedirectToAction("Index", new { UID = UID });
                    }
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[DeliveryWeightSlabApproval][POST:Approve]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[DeliveryWeightSlabApproval][POST:Approve]",
                        BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
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

        public void WriteToLogTable(DeliveryWeightSlab obj, ModelLayer.Models.Enum.COMMAND mode)
        {
            try
            {
                //Log Table Insertion
                //LogTable logTable = new LogTable();
                //logTable.TableName = "DeliveryWeightSlab";//table Name(Model Name)
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
                     + "[DeliveryWeightSlabApproval][WriteToLogTable]" + myEx.EXCEPTION_PATH,
                     BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeliveryWeightSlabApproval][WriteToLogTable]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
        }
	}
}