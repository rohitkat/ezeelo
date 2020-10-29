//-----------------------------------------------------------------------
// <copyright file=" MerchantPlanCategoryChargeController.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
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
using Franchise.Models;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class MerchantPlanCategoryChargeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /MerchantPlanCategoryCharge/
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantPlanCategoryCharge/CanRead")]
        public ActionResult Index(long? ownerId)
        {
            try
            {
                if (ownerId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                int? franchiseID = (from s in db.Shops where s.ID == ownerId select s.FranchiseID).FirstOrDefault();
                List<OwnerPlanCategoryChargeViewModel> opccList = null;
                if (franchiseID != null)
                {
                    ViewBag.FranchiseID = franchiseID;

                    opccList = (from opcc in db.OwnerPlanCategoryCharges
                                join op in db.OwnerPlans on opcc.OwnerPlanID equals op.ID
                                join pl in db.Plans on op.PlanID equals pl.ID
                                where opcc.OwnerPlan.OwnerID == ownerId &&
                                opcc.OwnerPlan.Plan.PlanCode.StartsWith("GBMR") &&
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
                                }).Distinct().ToList();

                    //BusinessLogicLayer.GetPlanCatCharge obj = new BusinessLogicLayer.GetPlanCatCharge();
                    //opccList = obj.SelectMerchantPlanCategoryCharge(ownerId, franchiseID);
                }

                if (opccList == null || opccList.Count == 0)
                {
                    return RedirectToAction("MessageText", new { msg = "Please!! Select the Plan for Merchant first Or Apply Charge to Plan first....", franchiseID = franchiseID });
                }
                return View(opccList);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        //redirect to Messages page
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantPlanCategoryCharge/CanRead")]
        public ActionResult MessageText(string msg, int? franchiseID)
        {
            try
            {
                //TempData["alertMessage"] = msg;
                ViewBag.Message = msg;
                ViewBag.FranchiseID = franchiseID;
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][GET:MessageText]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][GET:MessageText]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /MerchantPlanCategoryCharge/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantPlanCategoryCharge/CanRead")]
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
                    + "[MerchantPlanCategoryChargeController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /MerchantPlanCategoryCharge/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantPlanCategoryCharge/CanRead")]
        public ActionResult Create(int id, int ownerPlanID)
        {
            try
            {
                PlanCategoryCharge pcc = db.PlanCategoryCharges.Find(id);

                ViewBag.CategoryID = new SelectList(db.Categories.Where(a => a.ID.Equals(pcc.CategoryID)), "ID", "Name", pcc.CategoryID);
                ViewBag.FranchiseID = (from s in db.Shops
                                       join op in db.OwnerPlans on s.ID equals op.OwnerID
                                       where op.ID == ownerPlanID
                                       select s.FranchiseID).FirstOrDefault();
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
                    + "[MerchantPlanCategoryChargeController][Create:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][Create:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /MerchantPlanCategoryCharge/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantPlanCategoryCharge/CanWrite")]
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
                FillOwner(ownerplancategorycharge.OwnerPlanID);
                return View(ownerplancategorycharge);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /MerchantPlanCategoryCharge/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantPlanCategoryCharge/CanRead")]
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
                var getIds = (from s in db.Shops
                              join op in db.OwnerPlans on s.ID equals op.OwnerID
                              where op.ID == ownerplancategorycharge.OwnerPlanID
                              select new { s.FranchiseID, op.OwnerID }).FirstOrDefault();

                ViewBag.FranchiseID = getIds.FranchiseID;
                ViewBag.ownerId = getIds.OwnerID;
                return View(ownerplancategorycharge);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /MerchantPlanCategoryCharge/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpire]
        //[CustomAuthorize(Roles = "MerchantPlanCategoryCharge/CanWrite")]
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
                    //TryUpdateModel(ownerplancategorycharge);

                    if (ModelState.IsValid)
                    {
                        db.Entry(lOwnerPlanCategoryCharge).CurrentValues.SetValues(ownerplancategorycharge);
                        db.SaveChanges();

                        decimal chargeInPercent = 0, chargeInRupee = 0;
                        decimal.TryParse(ownerplancategorycharge.ChargeInPercent.ToString(), out chargeInPercent);
                        decimal.TryParse(ownerplancategorycharge.ChargeInRupee.ToString(), out chargeInRupee);

                        UpdateProductCharges(ownerplancategorycharge.OwnerPlanID, ownerplancategorycharge.CategoryID, chargeInPercent, chargeInRupee, ownerplancategorycharge.IsActive);


                        dbContextTransaction.Commit();

                        ViewBag.ownerId = (from opc in db.OwnerPlanCategoryCharges
                                           join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                                           where opc.ID == ownerplancategorycharge.ID
                                           select op.OwnerID).FirstOrDefault();
                        return RedirectToAction("Index", new { ownerId = @ViewBag.ownerId });
                    }
                    ViewBag.CategoryID = new SelectList(db.Categories.Where(x => x.ID == ownerplancategorycharge.CategoryID), "ID", "Name", ownerplancategorycharge.CategoryID);
                    FillOwner(ownerplancategorycharge.OwnerPlanID);
                    return View(ownerplancategorycharge);
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[MerchantPlanCategoryChargeController][POST:Edit]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[MerchantPlanCategoryChargeController][POST:Edit]",
                        BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                }
            }
            return View();
        }

        private void UpdateProductCharges(int ownerPlanId, int catId, decimal chargeInPercent, decimal chargeInRupee, bool isactive)
        {
            try
            {

                long shopId = db.OwnerPlans.Where(x => x.ID == ownerPlanId).Select(x => x.OwnerID).FirstOrDefault();
                List<ShopProduct> productList = db.ShopProducts.Where(x => x.ShopID == shopId && x.Product.CategoryID == catId).ToList();

                foreach (ShopProduct sp in productList)
                {
                    ShopProductCharge spc = db.ShopProductCharges.Where(x => x.ProductID == sp.ProductID && x.ShopID == shopId).FirstOrDefault();
                    if (spc != null)
                    {
                        spc.ChargeInPercent = chargeInPercent;
                        spc.ChargeInRs = chargeInRupee;
                        spc.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                        spc.ModifyDate = DateTime.UtcNow;
                        spc.NetworkIP = CommonFunctions.GetClientIP();
                        spc.DeviceID = "x";
                        spc.DeviceType = "x";
                        db.SaveChanges();
                    }

                    sp.IsActive = isactive;
                    sp.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    sp.ModifyDate = DateTime.UtcNow;
                    sp.NetworkIP = CommonFunctions.GetClientIP();
                    sp.DeviceID = "x";
                    sp.DeviceType = "x";
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[UpdateProductCharges]", "Can't Update Product Charges! in Method !" + Environment.NewLine + ex.Message);
            }
        }

        // GET: /MerchantPlanCategoryCharge/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantPlanCategoryCharge/CanRead")]
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
                    + "[MerchantPlanCategoryChargeController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /MerchantPlanCategoryCharge/Delete/5
        [HttpPost, ActionName("Delete")]
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantPlanCategoryCharge/CanWrite")]
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
                        + "[MerchantPlanCategoryChargeController][POST:Delete]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[MerchantPlanCategoryChargeController][POST:Delete]",
                        BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
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

        //public List<OwnerPlanCategoryChargeViewModel> GetOwnerPlanCategoryCharge(long? ownerId)
        //{
        //    List<OwnerPlanCategoryChargeViewModel> ownerPlanCategoryCharge = new List<OwnerPlanCategoryChargeViewModel>();
        //    try
        //    {
        //        ReadConfig rc = new ReadConfig(System.Web.HttpContext.Current.Server);
        //        DbOperations dbOpr = new GetData(rc.DB_CONNECTION);
        //        List<object> parametervalues = new List<object>();

        //        parametervalues.Add(ownerId);
        //        parametervalues.Add("GBMR");

        //        DataTable dt = dbOpr.GetRecords("SelectOwnerPlanCategoryCharge", parametervalues);

        //        if (dt.Rows.Count > 0)
        //        {
        //            ownerPlanCategoryCharge = (from DataRow dr in dt.Rows
        //                                       select new OwnerPlanCategoryChargeViewModel
        //                                       {
        //                                           CategoryName = Convert.ToString(dr["CategoryName"]),
        //                                           ChargeName = Convert.ToString(dr["ChargeName"]),
        //                                           PlanName = Convert.ToString(dr["PlanName"]),
        //                                           CategoryID = (int)dr["CategoryID"],
        //                                           ID = (dr["ID"] == DBNull.Value) ? 0 : (int)dr["ID"],
        //                                           OwnerPlanID = (dr["OwnerPlanID"] == DBNull.Value) ? 0 : (int)dr["OwnerPlanID"],
        //                                           PlanID = (int)dr["PlanID"],
        //                                           ChargeID = (int)dr["ChargeID"],
        //                                           ChargeInPercent = (decimal)dr["ChargeInPercent"],
        //                                           ChargeInRupee = (decimal)dr["ChargeInRupee"],
        //                                           IsActive = (dr["IsActive"] == DBNull.Value) ? false : (bool)dr["IsActive"],
        //                                           PlanCategoryChargeID = (dr["PlanCategoryChargeID"] == DBNull.Value) ? 0 : (int)dr["PlanCategoryChargeID"]
        //                                       }).OrderBy(x => x.CategoryName).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return ownerPlanCategoryCharge;
        //}

        protected void FillPlan(int? planID)
        {
            try
            {
                ViewBag.PlanID = new SelectList(db.Plans.Where(x => x.ID == planID), "ID", "ShortName", planID);
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
                                       join s in db.Shops on bd.ID equals s.BusinessDetailID
                                       join op in db.OwnerPlans on s.ID equals op.OwnerID
                                       join pl in db.Plans on op.PlanID equals pl.ID
                                       where op.ID == ownerPlanID && pl.PlanCode.StartsWith("GBMR")
                                       select new
                                       {
                                           Id = op.ID,
                                           Name = pl.ShortName
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
                                      join s in db.Shops on bd.ID equals s.BusinessDetailID
                                      join op in db.OwnerPlans on s.ID equals op.OwnerID
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
                LogTable logTable = new LogTable();
                logTable.TableName = "OwnerPlanCategoryCharge";//table Name(Model Name)
                logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(obj);
                logTable.TableRowID = obj.ID;
                logTable.Command = mode.ToString();
                long? rowOwnerID = (obj.ModifyBy >= 0 ? obj.ModifyBy : obj.CreateBy);
                logTable.RowOwnerID = (long)rowOwnerID;
                logTable.CreateDate = DateTime.UtcNow;
                logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                db.LogTables.Add(logTable);
                /**************************************/
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                     + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                     + "[MerchantPlanCategoryChargeController][WriteToLogTable]" + myEx.EXCEPTION_PATH,
                     BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantPlanCategoryChargeController][WriteToLogTable]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
        }
    }
}
