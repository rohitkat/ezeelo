//-----------------------------------------------------------------------
// <copyright file="OwnerPlanCategoryChargeController.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Snehal Shende</author>
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
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Text;
using Administrator.Models;

namespace Administrator.Controllers
{
    public class OwnerPlanCategoryChargeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /OwnerPlanCategoryCharge/
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlanCategoryCharge/CanRead")]
        public ActionResult Index(long? ownerId)
        {
            try
            {
                //int ownerPlanId = (from o in db.OwnerPlans where o.OwnerID == ownerId select o.ID).FirstOrDefault();
                if (ownerId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    //return View(db.OwnerPlans.ToList());
                }
                ViewBag.ownerId = ownerId;
                BusinessLogicLayer.GetPlanCatCharge obj = new BusinessLogicLayer.GetPlanCatCharge();
                List<OwnerPlanCategoryChargeViewModel> opccList = null;
                opccList = obj.GetOwnerPlanCategoryCharge(ownerId);
                if (opccList == null || opccList.Count == 0)
                {
                    return RedirectToAction("MessageText", new { msg = "Please!! Select the Plan for Merchant first Or Apply Charge to Plan first...." });
                }

                long BusinessDetailId = db.Franchises.Where(x => x.ID == ownerId).Select(x => x.BusinessDetailID).FirstOrDefault();
                long UID = db.BusinessDetails.Where(x => x.ID == BusinessDetailId && x.BusinessType.Prefix == "GBFR").Select(x => x.UserLoginID).FirstOrDefault();
                ViewBag.IsLocked = db.UserLogins.Where(x => x.ID == UID).Select(x => x.IsLocked).FirstOrDefault();

                return View(opccList);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        //redirect to Messages page
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlanCategoryCharge/CanRead")]
        public ActionResult MessageText(string msg)
        {
            try
            {
                //TempData["alertMessage"] = msg;
                ViewBag.Message = msg;
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][GET:MessageText]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][GET:MessageText]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlanCategoryCharge/CanRead")]
        public ActionResult ApplyChargesToMany(long? ownerId, int? CatID)
        {
            try
            {
                if (ownerId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                ViewBag.ownerId = ownerId;
                int? franchiseID = (from s in db.Shops where s.ID == ownerId select s.FranchiseID).FirstOrDefault();
                List<OwnerPlanCategoryChargeViewModel> opccList = null;
                if (franchiseID != null)
                {
                    ViewBag.FranchiseID = franchiseID;
                    BusinessLogicLayer.GetPlanCatCharge obj = new BusinessLogicLayer.GetPlanCatCharge();
                    opccList = obj.GetOwnerPlanCategoryCharge(ownerId);
                    TempData.Remove("FrnCatList");
                    TempData.Add("FrnCatList", opccList);
                }

                List<int> catlist = opccList.Select(x => x.CategoryID).ToList();
                var category_1 = (from c in db.Categories
                                  where catlist.Contains(c.ID) && c.Level == 1
                                  select new { ID = c.ID, Name = c.Name }).Distinct().OrderBy(x => x.Name);
                ViewBag.Category_1 = new SelectList(category_1, "ID", "Name", CatID);


            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][GET:ApplyChargesToMany]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][GET:ApplyChargesToMany]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }


        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlanCategoryCharge/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyChargesToMany(FormCollection frm, long? ownerId, bool IsActive)
        {
            try
            {
                ViewBag.Message = null;
                if (ownerId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ViewBag.ownerId = ownerId;
                decimal chargeInPercent = 0;
                decimal chargeInRupee = 0;
                int catId = 0;

                decimal.TryParse(frm.GetValue("ChargeInPercent").AttemptedValue, out chargeInPercent);
                decimal.TryParse(frm.GetValue("ChargeInRupee").AttemptedValue, out chargeInRupee);
                int.TryParse(frm.GetValue("CatID").AttemptedValue, out catId);

                List<OwnerPlanCategoryChargeViewModel> opccList = (List<OwnerPlanCategoryChargeViewModel>)TempData.Peek("FrnCatList");

                List<int> catlist = opccList.Select(x => x.CategoryID).ToList();
                var category_1 = (from c in db.Categories
                                  where catlist.Contains(c.ID) && c.Level == 1
                                  select new { ID = c.ID, Name = c.Name }).Distinct().OrderBy(x => x.Name);
                ViewBag.Category_1 = new SelectList(category_1, "ID", "Name", catId);

                if (catId > 0)
                {
                    ViewBag.PCatId = catId;
                    List<OwnerPlanCategoryChargeViewModel> catOpccList = (from cl in opccList
                                                                          join c in db.Categories on cl.CategoryID equals c.ID
                                                                          where c.ID == Convert.ToInt32(catId)
                                                                          select cl
                                                                      ).ToList();

                    opccList = new List<OwnerPlanCategoryChargeViewModel>(catOpccList);
                }

                if ((chargeInPercent > 0 && chargeInRupee > 0) || (chargeInPercent == 0 && chargeInRupee == 0))
                {
                    ModelState.AddModelError("CustomError", "Please Enter Charge either in % or in Rs...");
                    return View();
                }
                else
                {
                    InsertUpdateOwnerPlanCategoryCharge(chargeInPercent, chargeInRupee, IsActive, opccList);
                    //return RedirectToAction("Index", new { ownerId = ownerId });
                    ViewBag.Message = "Chages is Apply to the Category";
                    return View();
                }
                //long BusinessDetailId = db.Shops.Where(x => x.ID == ownerId).Select(x => x.BusinessDetailID).FirstOrDefault();
                //long UID = db.BusinessDetails.Where(x => x.ID == BusinessDetailId && x.BusinessType.Prefix == "GBMR").Select(x => x.UserLoginID).FirstOrDefault();
                //ViewBag.IsLocked = db.UserLogins.Where(x => x.ID == UID).Select(x => x.IsLocked).FirstOrDefault();

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][POST:ApplyChargesToMany]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][POST:ApplyChargesToMany]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        private void InsertUpdateOwnerPlanCategoryCharge(decimal chargeInPercent, decimal chargeInRupee, bool isactive, List<OwnerPlanCategoryChargeViewModel> catList)
        {
            try
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    foreach (OwnerPlanCategoryChargeViewModel opccv in catList)
                    {
                        string mode = "edit";
                        OwnerPlanCategoryCharge opcc = db.OwnerPlanCategoryCharges.Find(opccv.ID);
                        if (opcc == null)
                        {
                            mode = "insert";
                            opcc = new OwnerPlanCategoryCharge();
                        }
                        if (mode == "insert")
                        {
                            opcc.OwnerPlanID = opccv.OwnerPlanID;
                            opcc.CategoryID = opccv.CategoryID;
                            opcc.ChargeInPercent = chargeInPercent;
                            opcc.ChargeInRupee = chargeInRupee;
                            opcc.IsActive = isactive;
                            opcc.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                            opcc.CreateDate = DateTime.UtcNow;
                            opcc.NetworkIP = CommonFunctions.GetClientIP();
                            db.OwnerPlanCategoryCharges.Add(opcc);
                            db.SaveChanges();
                        }
                        else
                        {
                            opcc.ChargeInPercent = chargeInPercent;
                            opcc.ChargeInRupee = chargeInRupee;
                            opcc.IsActive = isactive;
                            opcc.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                            opcc.ModifyDate = DateTime.UtcNow;
                            opcc.NetworkIP = CommonFunctions.GetClientIP();
                            db.SaveChanges();
                        }
                    }
                    dbContextTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[insertUpdatePlanCategoryCharegs]", "Can't Insert or Update PlanCategoryCharegs!" + Environment.NewLine + ex.Message);
            }
        }

        // GET: /OwnerPlanCategoryCharge/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlanCategoryCharge/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                OwnerPlanCategoryCharge ownerplancategorycharge = db.OwnerPlanCategoryCharges.Find(id);
                if (ownerplancategorycharge == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ownerId = (from opc in db.OwnerPlanCategoryCharges
                                   join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                                   where opc.ID == ownerplancategorycharge.ID
                                   select op.OwnerID).FirstOrDefault();
                return View(ownerplancategorycharge);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /OwnerPlanCategoryCharge/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlanCategoryCharge/CanRead")]
        public ActionResult Create(int id, int ownerPlanID)
        {
            try
            {
                PlanCategoryCharge pcc = db.PlanCategoryCharges.Find(id);

                ViewBag.CategoryID = new SelectList(db.Categories.Where(a => a.ID.Equals(pcc.CategoryID)), "ID", "Name", pcc.CategoryID);
                FillPlan(pcc.PlanID);
                FillOwner(ownerPlanID);

                OwnerPlanCategoryCharge opcc = new OwnerPlanCategoryCharge();
                opcc.OwnerPlanID = ownerPlanID;
                opcc.CategoryID = pcc.CategoryID;
                opcc.ChargeInPercent = pcc.ChargeInPercent;
                opcc.ChargeInRupee = pcc.ChargeInRupee;

                return View(opcc);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /OwnerPlanCategoryCharge/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlanCategoryCharge/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,OwnerPlanID,CategoryID,ChargeInPercent,ChargeInRupee,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType")] OwnerPlanCategoryCharge ownerplancategorycharge, OwnerPlanCategoryChargeViewModel charges, int id, int ownerPlanID)
        {
            try
            {
                if ((ownerplancategorycharge.ChargeInPercent > 0 && ownerplancategorycharge.ChargeInRupee > 0) ||
                    ((ownerplancategorycharge.ChargeInPercent == 0 || ownerplancategorycharge.ChargeInPercent == null) &&
                        (ownerplancategorycharge.ChargeInRupee == 0 || ownerplancategorycharge.ChargeInRupee == null)))
                {
                    ModelState.AddModelError("CustomError", "Plaese Enter Charge either in % or in Rs...");
                }
                else if (ownerplancategorycharge.ChargeInPercent > 0)
                {
                    ownerplancategorycharge.ChargeInRupee = 0;
                }
                else
                {
                    ownerplancategorycharge.ChargeInPercent = 0;
                }

                ownerplancategorycharge.CategoryID = charges.CategoryID;
                ownerplancategorycharge.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                ownerplancategorycharge.CreateDate = DateTime.UtcNow;
                ownerplancategorycharge.NetworkIP = CommonFunctions.GetClientIP();

                //if (ModelState.IsValid)
                {
                    db.OwnerPlanCategoryCharges.Add(ownerplancategorycharge);
                    db.SaveChanges();
                    ViewBag.ownerId = (from opc in db.OwnerPlanCategoryCharges
                                       join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                                       where opc.ID == ownerplancategorycharge.ID
                                       select op.OwnerID).FirstOrDefault();
                    return RedirectToAction("Index", new { ownerId = @ViewBag.ownerId });
                }

                ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", ownerplancategorycharge.CategoryID);
                //int planID = (from opc in db.OwnerPlanCategoryCharges
                //                  join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                //                  where opc.ID == ownerplancategorycharge.ID
                //                  select op.PlanID).FirstOrDefault();
                // FillPlan(planID);
                FillOwner(ownerplancategorycharge.OwnerPlanID);
                //ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerplancategorycharge.CreateBy);
                //ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerplancategorycharge.ModifyBy);
                return View(ownerplancategorycharge);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /OwnerPlanCategoryCharge/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlanCategoryCharge/CanRead")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                OwnerPlanCategoryCharge ownerplancategorycharge = db.OwnerPlanCategoryCharges.Find(id);
                if (ownerplancategorycharge == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CategoryID = new SelectList(db.Categories.Where(x => x.ID == ownerplancategorycharge.CategoryID), "ID", "Name", ownerplancategorycharge.CategoryID);
                FillOwner(ownerplancategorycharge.OwnerPlanID);
                ViewBag.ownerId = (from opc in db.OwnerPlanCategoryCharges
                                   join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                                   where opc.ID == ownerplancategorycharge.ID
                                   select op.OwnerID).FirstOrDefault();
                // ViewBag.OwnerPlanID = new SelectList(db.OwnerPlans, "ID", "NetworkIP", ownerplancategorycharge.OwnerPlanID);
                //ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerplancategorycharge.CreateBy);
                //ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerplancategorycharge.ModifyBy);
                return View(ownerplancategorycharge);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /OwnerPlanCategoryCharge/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlanCategoryCharge/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,OwnerPlanID,CategoryID,ChargeInPercent,ChargeInRupee,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType")] OwnerPlanCategoryCharge ownerplancategorycharge)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    OwnerPlanCategoryCharge lOwnerPlanCategoryCharge = db.OwnerPlanCategoryCharges.Find(ownerplancategorycharge.ID);

                    if (lOwnerPlanCategoryCharge == null)
                    {
                        return View("Error");
                    }

                    if ((ownerplancategorycharge.ChargeInPercent > 0 && ownerplancategorycharge.ChargeInRupee > 0) ||
                    ((ownerplancategorycharge.ChargeInPercent == 0 || ownerplancategorycharge.ChargeInPercent == null) &&
                        (ownerplancategorycharge.ChargeInRupee == 0 || ownerplancategorycharge.ChargeInRupee == null)))
                    {
                        ModelState.AddModelError("CustomError", "Plaese Enter Charge either in % or in Rs...");
                    }
                    else if (ownerplancategorycharge.ChargeInPercent > 0)
                    {
                        ownerplancategorycharge.ChargeInRupee = 0;
                    }
                    else
                    {
                        ownerplancategorycharge.ChargeInPercent = 0;
                    }

                    WriteToLogTable(lOwnerPlanCategoryCharge, ModelLayer.Models.Enum.COMMAND.UPDATE);

                    ownerplancategorycharge.CreateDate = Convert.ToDateTime(lOwnerPlanCategoryCharge.CreateDate);
                    ownerplancategorycharge.CreateBy = Convert.ToInt64(lOwnerPlanCategoryCharge.CreateBy);
                    ownerplancategorycharge.ModifyDate = DateTime.UtcNow;
                    ownerplancategorycharge.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    TryUpdateModel(ownerplancategorycharge);

                    if (ModelState.IsValid)
                    {
                        db.Entry(lOwnerPlanCategoryCharge).CurrentValues.SetValues(ownerplancategorycharge);
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        ViewBag.ownerId = (from opc in db.OwnerPlanCategoryCharges
                                           join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                                           where opc.ID == ownerplancategorycharge.ID
                                           select op.OwnerID).FirstOrDefault();
                        return RedirectToAction("Index", new { ownerId = @ViewBag.ownerId });
                    }
                    ViewBag.CategoryID = new SelectList(db.Categories.Where(x => x.ID == ownerplancategorycharge.CategoryID), "ID", "Name", ownerplancategorycharge.CategoryID);
                    FillOwner(ownerplancategorycharge.OwnerPlanID);
                    //ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerplancategorycharge.CreateBy);
                    //ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", ownerplancategorycharge.ModifyBy);
                    return View(ownerplancategorycharge);
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[OwnerPlanCategoryChargeController][POST:Edit]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[OwnerPlanCategoryChargeController][POST:Edit]",
                        BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                }
            }
            return View();
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlanCategoryCharge/CanRead")]
        public ActionResult ViewCharge(long? ownerId)
        {
            try
            {
                if (ownerId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                ViewBag.ownerId = ownerId;
                List<OwnerPlanCategoryChargeViewModel> opccList = null;
                opccList = (from opcc in db.OwnerPlanCategoryCharges
                            join op in db.OwnerPlans on opcc.OwnerPlanID equals op.ID
                            join pl in db.Plans on op.PlanID equals pl.ID
                            where opcc.OwnerPlan.OwnerID == ownerId &&
                            opcc.OwnerPlan.Plan.PlanCode.StartsWith("GBFR") &&
                            opcc.OwnerPlan.IsActive == true
                            select new OwnerPlanCategoryChargeViewModel
                            {
                                ID = opcc.ID,
                                OwnerPlanName = pl.ShortName,
                                OwnerPlanID = opcc.OwnerPlanID,
                                PlanName = pl.ShortName,
                                PlanID = op.PlanID,
                                CategoryID = opcc.CategoryID,
                                CategoryName = opcc.Category.Name,
                                ChargeInPercent = opcc.ChargeInPercent,
                                ChargeInRupee = opcc.ChargeInRupee,
                                IsActive = opcc.IsActive
                            }).Distinct().OrderBy(x => x.CategoryName).ToList();

                long BusinessDetailId = db.Franchises.Where(x => x.ID == ownerId).Select(x => x.BusinessDetailID).FirstOrDefault();
                long UID = db.BusinessDetails.Where(x => x.ID == BusinessDetailId && x.BusinessType.Prefix == "GBFR").Select(x => x.UserLoginID).FirstOrDefault();
                ViewBag.IsLocked = db.UserLogins.Where(x => x.ID == UID).Select(x => x.IsLocked).FirstOrDefault();

                if (opccList == null || opccList.Count == 0)
                {
                    return RedirectToAction("MessageText", new { msg = "Please!! Select the Plan for Merchant first Or Apply Charge to Plan first...." });
                }
                return View(opccList);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /OwnerPlanCategoryCharge/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlanCategoryCharge/CanRead")]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                OwnerPlanCategoryCharge ownerplancategorycharge = db.OwnerPlanCategoryCharges.Find(id);
                if (ownerplancategorycharge == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ownerId = (from opc in db.OwnerPlanCategoryCharges
                                   join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                                   where opc.ID == ownerplancategorycharge.ID
                                   select op.OwnerID).FirstOrDefault();
                return View(ownerplancategorycharge);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /OwnerPlanCategoryCharge/Delete/5
        [HttpPost, ActionName("Delete")]
        [SessionExpire]
        [CustomAuthorize(Roles = "OwnerPlanCategoryCharge/CanWrite")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    OwnerPlanCategoryCharge ownerplancategorycharge = db.OwnerPlanCategoryCharges.Find(id);

                    WriteToLogTable(ownerplancategorycharge, ModelLayer.Models.Enum.COMMAND.DELETE);

                    ViewBag.ownerId = (from opc in db.OwnerPlanCategoryCharges
                                       join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                                       where opc.ID == ownerplancategorycharge.ID
                                       select op.OwnerID).FirstOrDefault();

                    db.OwnerPlanCategoryCharges.Remove(ownerplancategorycharge);
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    return RedirectToAction("Index", new { ownerId = @ViewBag.ownerId });
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[OwnerPlanCategoryChargeController][POST:Delete]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[OwnerPlanCategoryChargeController][POST:Delete]",
                        BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
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

        protected void FillPlan(int? planID)
        {
            try
            {
                ViewBag.PlanID = new SelectList(db.Plans.Where(x => x.ID == planID), "ID", "PlanCode", planID);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[FillPlan]", "Can't Fill Plan! in Method !" + Environment.NewLine + ex.Message);
            }
        }

        protected void FillOwner(int? ownerPlanID)
        {
            try
            {
                if (ownerPlanID != null)
                {
                    var lOwnerPlanID = from p in db.PersonalDetails
                                       join bd in db.BusinessDetails on p.UserLoginID equals bd.UserLoginID
                                       join f in db.Franchises on bd.ID equals f.BusinessDetailID
                                       join op in db.OwnerPlans on f.ID equals op.OwnerID
                                       join pl in db.Plans on op.PlanID equals pl.ID
                                       where op.ID == ownerPlanID && pl.PlanCode.StartsWith("GBFR")
                                       select new
                                       {
                                           Id = op.ID,
                                           Name = pl.PlanCode
                                       };
                    ViewBag.OwnerPlanID = new SelectList(lOwnerPlanID, "Id", "Name", ownerPlanID);
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[FillOwner]", "Can't Fill Owner! in Method !" + Environment.NewLine + ex.Message);
            }
        }

        protected void GetOwnerName(int? ownerPlanID)
        {
            try
            {
                ViewBag.OwnerPlanID = from p in db.PersonalDetails
                                      join bd in db.BusinessDetails on p.UserLoginID equals bd.UserLoginID
                                      join f in db.Franchises on bd.ID equals f.BusinessDetailID
                                      join op in db.OwnerPlans on f.ID equals op.OwnerID
                                      join pl in db.Plans on op.PlanID equals pl.ID
                                      where op.ID == ownerPlanID
                                      select p.Salutation.Name + " " + p.FirstName + " " + p.LastName;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[GetOwnerName]", "Can't Get Owner Name! in Method !" + Environment.NewLine + ex.Message);
            }
        }

        public void WriteToLogTable(OwnerPlanCategoryCharge obj, ModelLayer.Models.Enum.COMMAND mode)
        {
            try
            {
                //Log Table Insertion
                //LogTable logTable = new LogTable();
                //logTable.TableName = "OwnerPlanCategoryCharge";//table Name(Model Name)
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
                     + "[OwnerPlanCategoryChargeController][WriteToLogTable]" + myEx.EXCEPTION_PATH,
                     BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OwnerPlanCategoryChargeController][WriteToLogTable]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
        }
    }
}
