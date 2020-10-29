//-----------------------------------------------------------------------
// <copyright file="ProductList.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Pradnyakar N. Badge</author>
//-----------------------------------------------------------------------
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
using System.Text;
using Administrator.Models;

namespace Administrator.Controllers
{

    public class OwnerAdvertisementController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /OwnerAdvertisement/
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerAdvertisement/CanRead")]
        public ActionResult Index()
        {
            try
            {
                var owneradvertisements = db.OwnerAdvertisements.OrderBy(x => x.NavigationUrl).Include(o => o.Advertisement)
                                            .Include(o => o.BusinessType)
                                            .Include(o => o.PersonalDetail)
                                            .Include(o => o.PersonalDetail1);

                return View(owneradvertisements.ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerAdvertisementController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerAdvertisementController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /OwnerAdvertisement/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerAdvertisement/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                OwnerAdvertisement owneradvertisement = db.OwnerAdvertisements.Find(id);
                if (owneradvertisement == null)
                {
                    return HttpNotFound();
                }
                return View(owneradvertisement);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerAdvertisementController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerAdvertisementController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /OwnerAdvertisement/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerAdvertisement/CanRead")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.AdvertisementID = new SelectList(db.Advertisements.OrderBy(x =>x.PageName), "ID", "PageName");
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes.OrderBy(x => x.Name), "ID", "Name");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.BusinessOwner = new SelectList(lData, "Value", "Text");

                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerAdvertisementController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerAdvertisementController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /OwnerAdvertisement/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerAdvertisement/CanWrite")]
        public ActionResult Create([Bind(Include = "ID,AdvertisementID,AdvertisementTitle,NavigationUrl,NoOfDays,NoOfHours,BusinessTypeID,OwnerID,FeesInRupee,PriorityLevel,IsLive,IsActive")] OwnerAdvertisement owneradvertisement, long? BusinessOwner)
        {
            try
            {
          

                ViewBag.AdvertisementID = new SelectList(db.Advertisements.OrderBy(x => x.PageName), "ID", "PageName", owneradvertisement.AdvertisementID);
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes.OrderBy(x => x.Name), "ID", "Name", owneradvertisement.BusinessTypeID);

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.BusinessOwner = new SelectList(lData, "Value", "Text");

                if (BusinessOwner == null || BusinessOwner < 1)
                {
                    ViewBag.Message = "Business Owner is Required";
                    return View(owneradvertisement);
                }

                if ( owneradvertisement.AdvertisementID < 1)
                {
                    ViewBag.Message = "Business Owner is Required";
                    return View(owneradvertisement);
                }
                var lValues = db.OwnerAdvertisements.Where(x => x.AdvertisementID == owneradvertisement.AdvertisementID && x.PriorityLevel == owneradvertisement.PriorityLevel);
                if (lValues.Count() > 0)
                {
                    ViewBag.Message = "Advertisement and Priority level already Exists";
                    return View(owneradvertisement);
                }

                owneradvertisement.OwnerID = Convert.ToInt64(BusinessOwner);
                owneradvertisement.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                owneradvertisement.CreateDate = DateTime.UtcNow.AddHours(5.30);
                owneradvertisement.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                owneradvertisement.DeviceID = string.Empty;
                owneradvertisement.DeviceType = string.Empty;



                db.OwnerAdvertisements.Add(owneradvertisement);
                db.SaveChanges();
                //return RedirectToAction("Index");

                ViewBag.Message = "Owner Advertisement Inserted Successfully";


                return View(owneradvertisement);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerAdvertisementController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Insert Owner Advertisement Detail ";
                return View(owneradvertisement);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerAdvertisementController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Insert Owner Advertisement Detail ";
                return View(owneradvertisement);
            }
            //return View();
        }

        // GET: /OwnerAdvertisement/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                OwnerAdvertisement owneradvertisement = db.OwnerAdvertisements.Find(id);
                if (owneradvertisement == null)
                {
                    return HttpNotFound();
                }
                ViewBag.AdvertisementID = new SelectList(db.Advertisements.OrderBy(x => x.PageName), "ID", "PageName", owneradvertisement.AdvertisementID);
                ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes.OrderBy(x => x.Name), "ID", "Name", owneradvertisement.BusinessTypeID);



                if (owneradvertisement.BusinessTypeID > 0)
                {
                    OwnerDetailByPrefix objODP = new OwnerDetailByPrefix();
                    List<OwnerDetailByPrefix> lData = new List<OwnerDetailByPrefix>();
                    lData = objODP.OwnerDetail(owneradvertisement.BusinessTypeID, System.Web.HttpContext.Current.Server);
                    ViewBag.BusinessOwner = new SelectList(lData.OrderBy(x => x.Name), "ID", "Name", owneradvertisement.OwnerID);
                }
                else
                {
                    List<SelectListItem> lData = new List<SelectListItem>();
                    lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                    ViewBag.BusinessOwner = new SelectList(lData, "Value", "Text");
                }

                return View(owneradvertisement);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerAdvertisementController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerAdvertisementController][GET:CEditreate]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /OwnerAdvertisement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerAdvertisement/CanWrite")]
        public ActionResult Edit([Bind(Include = "ID,AdvertisementID,AdvertisementTitle,NavigationUrl,NoOfDays,NoOfHours,BusinessTypeID,OwnerID,FeesInRupee,PriorityLevel,IsLive,IsActive")] OwnerAdvertisement owneradvertisement, long? BusinessOwner)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    ViewBag.AdvertisementID = new SelectList(db.Advertisements.OrderBy(x => x.PageName), "ID", "PageName", owneradvertisement.AdvertisementID);
                    ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes.OrderBy(x => x.Name), "ID", "Name", owneradvertisement.BusinessTypeID);



                    if (owneradvertisement.BusinessTypeID > 0)
                    {
                        OwnerDetailByPrefix objODP = new OwnerDetailByPrefix();
                        List<OwnerDetailByPrefix> lDataView = new List<OwnerDetailByPrefix>();
                        lDataView = objODP.OwnerDetail(owneradvertisement.BusinessTypeID, System.Web.HttpContext.Current.Server);
                        ViewBag.BusinessOwner = new SelectList(lDataView.OrderBy(x => x.Name), "ID", "Name", owneradvertisement.OwnerID);
                    }
                    else
                    {
                        List<SelectListItem> lDataView = new List<SelectListItem>();
                        lDataView.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                        ViewBag.BusinessOwner = new SelectList(lDataView, "Value", "Text");
                    }

                    if (BusinessOwner == null || BusinessOwner < 1)
                    {
                        ViewBag.Message = "Business Owner is Required";
                        return View(owneradvertisement);
                    }

                    if (owneradvertisement.AdvertisementID < 1)
                    {
                        ViewBag.Message = "Business Owner is Required";
                        return View(owneradvertisement);
                    }

                    var lValues = db.OwnerAdvertisements.Where(x => x.AdvertisementID == owneradvertisement.AdvertisementID && x.PriorityLevel == owneradvertisement.PriorityLevel && x.ID != owneradvertisement.ID);
                    if (lValues.Count() > 0)
                    {
                        ViewBag.Message = "Advertisement and Priority level already Exists";
                        return View(owneradvertisement);
                    }
                    OwnerAdvertisement lData = db.OwnerAdvertisements.Single(x => x.ID == owneradvertisement.ID);

                    //Log Table Insertion
                    //LogTable logTable = new LogTable();
                    //logTable.TableName = "OwnerAdvertisement";//table Name(Model Name)
                    //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(lData);
                    //logTable.TableRowID = lData.ID;
                    //logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                    //long? rowOwnerID = (lData.ModifyBy >= 0 ? lData.ModifyBy : lData.CreateBy);
                    //logTable.RowOwnerID = (long)rowOwnerID;
                    //logTable.CreateDate = DateTime.UtcNow;
                    //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                    //db.LogTables.Add(logTable);
                    /**************************************/

                    owneradvertisement.OwnerID = Convert.ToInt64(BusinessOwner);
                    owneradvertisement.CreateBy = lData.CreateBy;
                    owneradvertisement.CreateDate = lData.CreateDate;
                    owneradvertisement.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    owneradvertisement.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                    owneradvertisement.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                    owneradvertisement.DeviceID = string.Empty;
                    owneradvertisement.DeviceType = string.Empty;

                    //if (ModelState.IsValid)
                    //{
                    db.Entry(lData).CurrentValues.SetValues(owneradvertisement);
                    //db.Entry(owneradvertisement).State = EntityState.Modified;
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Owner Advertisement Updated Successfully";
                    // }
                    ViewBag.AdvertisementID = new SelectList(db.Advertisements.OrderBy(x => x.PageName), "ID", "PageName", owneradvertisement.AdvertisementID);
                    ViewBag.BusinessTypeID = new SelectList(db.BusinessTypes.OrderBy(x => x.Name), "ID", "Name", owneradvertisement.BusinessTypeID);

                    List<SelectListItem> lDataOwnerDI = new List<SelectListItem>();
                    lDataOwnerDI.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                    ViewBag.BusinessOwner = new SelectList(lDataOwnerDI, "Value", "Text");

                    return View(owneradvertisement);
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[OwnerAdvertisementController][POST:Edit]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                    ViewBag.Messaage = "Unable to Update Owner Advertisement Detail ";
                    return View(owneradvertisement);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[OwnerAdvertisementController][POST:Edit]",
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                    ViewBag.Messaage = "Unable to Update Owner Advertisement Detail ";
                    return View(owneradvertisement);
                }
            }
            //return View();
        }

        // GET: /OwnerAdvertisement/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerAdvertisement/CanRead")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                OwnerAdvertisement owneradvertisement = db.OwnerAdvertisements.Find(id);
                if (owneradvertisement == null)
                {
                    return HttpNotFound();
                }
                return View(owneradvertisement);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerAdvertisementController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerAdvertisementController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /OwnerAdvertisement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerAdvertisement/CanWrite")]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    OwnerAdvertisement owneradvertisement = db.OwnerAdvertisements.Find(id);

                    //Log Table Insertion
                    //LogTable logTable = new LogTable();
                    //logTable.TableName = "OwnerAdvertisement";//table Name(Model Name)
                    //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(owneradvertisement);
                    //logTable.TableRowID = owneradvertisement.ID;
                    //logTable.Command = ModelLayer.Models.Enum.COMMAND.UPDATE.ToString();
                    //long? rowOwnerID = (owneradvertisement.ModifyBy >= 0 ? owneradvertisement.ModifyBy : owneradvertisement.CreateBy);
                    //logTable.RowOwnerID = (long)rowOwnerID;
                    //logTable.CreateDate = DateTime.UtcNow;
                    //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                    //db.LogTables.Add(logTable);
                    /**************************************/

                    db.OwnerAdvertisements.Remove(owneradvertisement);
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[OwnerAdvertisementController][POST:Delete]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                    ViewBag.Messaage = "Unable to Delete Owner Advertisement Detail ";
                    return View(db.OwnerAdvertisements.Where( x=> x.ID == id).FirstOrDefault());
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[OwnerAdvertisementController][POST:Delete]",
                        BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);

                    ViewBag.Messaage = "Unable to Delete Owner Advertisement Detail ";
                    return View(db.OwnerAdvertisements.Where(x => x.ID == id).FirstOrDefault());
                }
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

        public JsonResult getOwnerID(int? businessTypeID)
        {
            try
            {
                OwnerDetailByPrefix objODP = new OwnerDetailByPrefix();
                List<OwnerDetailByPrefix> lownerType = new List<OwnerDetailByPrefix>();

                lownerType = objODP.OwnerDetail(businessTypeID, System.Web.HttpContext.Current.Server);
                return Json(lownerType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[getOwnerID]", "Can't Get Owner ID! in Method !" + Environment.NewLine + ex.Message);
            }
        }

    }
}
