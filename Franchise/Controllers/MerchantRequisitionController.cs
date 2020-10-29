//-----------------------------------------------------------------------
// <copyright file=" MerchantRequisitionController.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Snehal Shende</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Data.Entity;
using Franchise.Models;
using System.Net;

namespace Franchise.Controllers
{
    public class MerchantRequisitionController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        //

        //[SessionExpire]
        //[CustomAuthorize(Roles = "MerchantRequisition/CanRead")]
        //public ActionResult ShowFranchiseList()
        //{
        //    try
        //    {
        //        //if (Session["FranchiseID"] == null)
        //        //{
        //        var lfav = from ul in db.UserLogins
        //                   join bd in db.BusinessDetails on ul.ID equals bd.UserLoginID
        //                   join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
        //                   join f in db.Franchises on bd.ID equals f.BusinessDetailID
        //                   where bd.BusinessType.Prefix == "GBFR" && ul.IsLocked == false
        //                   select new FranchisePendingApprovalViewModel
        //                   {
        //                       UserLoginID = bd.UserLoginID,
        //                       BusinessTypePrefix = bd.BusinessType.Prefix,
        //                       Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName + " (" + bd.Name + ")",
        //                       OwnerId = f.ID,
        //                   };
        //        return View(lfav);
        //        //}
        //        //else
        //        //{
        //        //    return View("Index", new { franchiseID = Convert.ToInt32(Session["FranchiseID"]) });
        //        //}
        //    }
        //    catch (BusinessLogicLayer.MyException myEx)
        //    {
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
        //            + "[MerchantPendingApprovalsController][GET:ShowFranchiseList]" + myEx.EXCEPTION_PATH,
        //            BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
        //    }
        //    catch (Exception ex)
        //    {
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[MerchantPendingApprovalsController][GET:ShowFranchiseList]",
        //            BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
        //    }
        //    return View();
        //}

        // GET: /MerchantRequisition/
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantRequisition/CanRead")]
        public ActionResult Index()
        {
            try
            {
                int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                if (franchiseID == null)
                {
                    return View("Error");
                }

                var lShop = (from s in db.Shops
                             join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                             join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
                             join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                             join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                             where s.IsActive == true && bt.Prefix == "GBMR"
                                    && s.FranchiseID == franchiseID
                             select new MerchantRequisitionViewModel
                             {
                                 UserLoginID = ul.ID,
                                 ShopID = s.ID,
                                 ShopName = s.Name,
                                 MerchantName = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName + " (" + s.Name + ")",
                                 Mobile = ul.Mobile,
                                 Email = ul.Email,
                                 TIN = s.TIN,
                                 Pincode = bd.Pincode.Name,
                                 FranchiseID = s.FranchiseID,
                                 IsPaymentMode = (db.ShopPaymentModes.Where(x => x.ShopID == s.ID).Select(x => x.ID).Count() > 0) ? true : false
                             }).OrderBy(x => x.MerchantName).ToList();

                return View(lShop);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantRequisitionController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantRequisitionController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        //
        // GET: /MerchantRequisition/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantRequisition/CanRead")]
        public ActionResult Create(long id)
        {
            try
            {
                //ViewBag.FranchiseID = new SelectList(
                //    (from f in db.Franchises
                //     join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                //     join pd in db.PersonalDetails on bd.UserLoginID equals pd.UserLoginID
                //     join ul in db.UserLogins on pd.UserLoginID equals ul.ID
                //     where f.IsActive == true && ul.IsLocked == false && f.ID != 1
                //     select new { ID = f.ID, Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName + " (" + bd.Name + ")"}).ToList(), "ID", "Name", franchiseId);
                int franchiseId = Convert.ToInt32(Session["FRANCHISE_ID"]);

                var lShop = (from s in db.Shops
                             join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                             join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
                             join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                             join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                             where s.IsActive == true && bt.Prefix == "GBMR"
                                    && s.ID == id
                             select new MerchantRequisitionViewModel
                             {
                                 UserLoginID = ul.ID,
                                 ShopID = s.ID,
                                 ShopName = s.Name,
                                 MerchantName = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName+ " (" + s.Name + ")",
                                 Mobile = ul.Mobile,
                                 Email = ul.Email,
                                 TIN = s.TIN,
                                 Pincode = bd.Pincode.Name,
                                 FranchiseID = franchiseId
                             }).FirstOrDefault();

                List<CustomPaymentMode> lst = new List<CustomPaymentMode>();
                List<PaymentMode> lstPM = db.PaymentModes.Where(x => x.IsActive == true).ToList();
                foreach (PaymentMode PM in lstPM)
                {
                    CustomPaymentMode CPM = new CustomPaymentMode();
                    CPM.ID = PM.ID;
                    CPM.Name = PM.Name;
                    CPM.IsSelected = false;
                    lst.Add(CPM);
                }

                lShop.PaymentModeList = lst;

                return View(lShop);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantRequisition][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantRequisition][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }

            return View();
        }

        //
        // POST: /MerchantRequisition/Create
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantRequisition/CanWrite")]
        public ActionResult Create([Bind(Include = "UserLoginID,ShopID,ShopName,MerchantName,Mobile,Email,TIN,Pincode,FranchiseID,PaymentModeList")] MerchantRequisitionViewModel merchantRequisition, int?[] chkBox)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    // TODO: Add insert logic here
                    //ViewBag.FranchiseID = new SelectList(
                    //(from f in db.Franchises
                    // join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                    // join pd in db.PersonalDetails on bd.UserLoginID equals pd.UserLoginID
                    // join ul in db.UserLogins on pd.UserLoginID equals ul.ID
                    // where f.IsActive == true && ul.IsLocked == false && f.ID != 1
                    // select new { ID = f.ID, Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName + " (" + bd.Name + ")"}).ToList(), "ID", "Name", merchantRequisition.FranchiseID);

                    //ViewBag.PaymentModeID = new SelectList(db.PaymentModes, "ID", "Name", merchantRequisition.PaymentModeID);

                    //if (merchantRequisition.FranchiseID != null)
                    //{
                        if (chkBox.Count() > 0)
                        {                            
                            //foreach (CustomPaymentMode CPM in merchantRequisition.PaymentModeList)
                            //{
                            foreach (int id in chkBox)
                            {
                                ShopPaymentMode spm = new ShopPaymentMode();
                                spm.ShopID = merchantRequisition.ShopID;
                                spm.PaymentModeID = id;//CPM.ID; ;
                                spm.IsActive = true;
                                spm.CreateDate = DateTime.UtcNow;
                                spm.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                                spm.NetworkIP = CommonFunctions.GetClientIP();
                                spm.DeviceID = "x";
                                spm.DeviceType = "x";
                                db.ShopPaymentModes.Add(spm);
                                db.SaveChanges();
                            }
                            //}

                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            ModelState.AddModelError("CustomError", "Payment Mode Required.");
                            return View(merchantRequisition);
                        }
                    //}
                    //else
                    //{
                    //    ModelState.AddModelError("CustomError", "Franchise Required.");
                    //    return View(merchantRequisition);
                    //}
                    return RedirectToAction("Index");
                }
                catch (BusinessLogicLayer.MyException myEx)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                        + "[MerchantRequisition][POST:Create]" + myEx.EXCEPTION_PATH,
                        BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                }
                catch (Exception ex)
                {
                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                        + Environment.NewLine + ex.Message + Environment.NewLine
                        + "[MerchantRequisition][POST:Create]",
                        BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                }
            }
            return View();
        }

        //
        // GET: /MerchantRequisition/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantRequisition/CanRead")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (db.ShopPaymentModes.Where(x => x.ShopID == id).Count() == 0)
                {
                    return HttpNotFound();
                }

                //ViewBag.FranchiseID = new SelectList(
                //    (from f in db.Franchises
                //     join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                //     join pd in db.PersonalDetails on bd.UserLoginID equals pd.UserLoginID
                //     join ul in db.UserLogins on pd.UserLoginID equals ul.ID
                //     where f.IsActive == true && ul.IsLocked == false && f.ID != 1
                //     select new { ID = f.ID, Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName+ " (" + bd.Name + ")" }).ToList(), "ID", "Name", franchiseId);
                int franchiseId = Convert.ToInt32(Session["FRANCHISE_ID"]);

                var lShop = (from s in db.Shops
                             join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                             join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
                             join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                             join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                             where s.IsActive == true && bt.Prefix == "GBMR"
                                    && s.ID == id
                             select new MerchantRequisitionViewModel
                             {
                                 UserLoginID = ul.ID,
                                 ShopID = s.ID,
                                 ShopName = s.Name,
                                 MerchantName = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName+ " (" + s.Name + ")",
                                 Mobile = ul.Mobile,
                                 Email = ul.Email,
                                 TIN = s.TIN,
                                 Pincode = bd.Pincode.Name,
                                 FranchiseID = franchiseId
                             }).FirstOrDefault();

                List<CustomPaymentMode> lst = new List<CustomPaymentMode>();
                List<PaymentMode> lstPM = db.PaymentModes.Where(x => x.IsActive == true).ToList();
                foreach (PaymentMode PM in lstPM)
                {
                    CustomPaymentMode CPM = new CustomPaymentMode();
                    CPM.ID = PM.ID;
                    CPM.Name = PM.Name;
                    CPM.IsSelected = (db.ShopPaymentModes.Where(x => x.ShopID == id && x.PaymentModeID == PM.ID).Count() > 0) ? true : false;
                    lst.Add(CPM);
                }

                lShop.PaymentModeList = lst;

                return View(lShop);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantRequisition][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantRequisition][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        //
        // POST: /MerchantRequisition/Create
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "MerchantRequisition/CanWrite")]
        public ActionResult Edit([Bind(Include = "UserLoginID,ShopID,ShopName,MerchantName,Mobile,Email,TIN,Pincode,FranchiseID,PaymentModeList")] MerchantRequisitionViewModel merchantRequisition, int?[] chkBox)
        {
            try
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    //ViewBag.FranchiseID = new SelectList(
                    //    (from f in db.Franchises
                    //     join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                    //     join pd in db.PersonalDetails on bd.UserLoginID equals pd.UserLoginID
                    //     join ul in db.UserLogins on pd.UserLoginID equals ul.ID
                    //     where f.IsActive == true && ul.IsLocked == false && f.ID != 1
                    //     select new { ID = f.ID, Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName+ " (" + bd.Name + ")" }).ToList(), "ID", "Name", merchantRequisition.FranchiseID);

                    if (chkBox != null)
                    {
                        Shop s = db.Shops.Find(merchantRequisition.ShopID);
                        if (s == null)
                        {
                            return View("Error");
                        }

                        RemoveShopPaymentMode(merchantRequisition.PaymentModeList, merchantRequisition.ShopID, chkBox);
                        InsertShopPaymentMode(merchantRequisition.ShopID, chkBox);

                        dbContextTransaction.Commit();
                    }
                    else
                    {
                        ModelState.AddModelError("CustomError", "Can't Svae!! At least Single Payment Mode Required.");
                        return View(merchantRequisition);
                    }
                    return RedirectToAction("Index");
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantRequisition][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantRequisition][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        private void InsertShopPaymentMode(long shopId, int?[] chkBox)
        {
            try
            {
                foreach (int id in chkBox)
                {
                    if (db.ShopPaymentModes.Where(x => x.PaymentModeID == id && x.ShopID == shopId).Count() == 0)
                    {
                        ShopPaymentMode spm = new ShopPaymentMode();
                        spm.ShopID = shopId;
                        spm.PaymentModeID = id;//CPM.ID; ;
                        spm.IsActive = true;
                        spm.CreateDate = DateTime.UtcNow;
                        spm.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                        spm.NetworkIP = CommonFunctions.GetClientIP();
                        spm.DeviceID = "x";
                        spm.DeviceType = "x";
                        db.ShopPaymentModes.Add(spm);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[InsertShopPaymentMode]", "Can't Insert Shop Payment Mode !" + Environment.NewLine + ex.Message);
            }
        }

        private void RemoveShopPaymentMode(List<CustomPaymentMode> list, long shopId, int?[] chkBox)
        {
            try
            {
                foreach (CustomPaymentMode CPM in list)
                {
                    foreach (int id in chkBox)
                    {
                        if (CPM.ID != id && CPM.IsSelected)
                        {
                            ShopPaymentMode sp = db.ShopPaymentModes.Where(x => x.PaymentModeID == CPM.ID && x.ShopID == shopId).FirstOrDefault();
                            db.ShopPaymentModes.Remove(sp);
                            db.SaveChanges();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[RemoveShopPaymentMode]", "Can't Remove Shop Payment Mode !" + Environment.NewLine + ex.Message);
            }
        }

        public void WriteToLogTable(Shop obj, ModelLayer.Models.Enum.COMMAND mode)
        {
            try
            {
                //Log Table Insertion
                //LogTable logTable = new LogTable();
                //logTable.TableName = "Shop";//table Name(Model Name)
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
                     + "[MerchantRequisition][WriteToLogTable]" + myEx.EXCEPTION_PATH,
                     BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantRequisition][WriteToLogTable]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
        }

    }
}
