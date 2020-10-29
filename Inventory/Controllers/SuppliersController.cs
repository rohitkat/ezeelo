using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory;
//using Inventory.Models;
using ModelLayer.Models;
using BusinessLogicLayer;
using System.Data.SqlClient;
using ModelLayer.Models.ViewModel;

namespace Inventory.Controllers
{
    public class SuppliersController : Controller
    {
        private string fConnectionString = System.Configuration.ConfigurationSettings.AppSettings["DB_CON"].ToString();
        private EzeeloDBContext db = new EzeeloDBContext();

        public object EVWV_SupplierViewModel { get; private set; }

        //
        // GET: /Suppliers/

        public ActionResult Index()
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["IsEzeeloLogin"].ToString() == "1")
            {
                //Added by Priti
                List<Supplier> OBH = db.Suppliers.ToList();
                foreach (var ITEM in OBH)
                {

                    var pinid = ITEM.PincodeID;
                    ITEM.CityID = db.Pincodes.Where(x => x.ID == pinid).Select(l => l.CityID).FirstOrDefault();
                    ITEM.CityName = db.Cities.Where(x => x.ID == ITEM.CityID).Select(s => s.Name).FirstOrDefault();
                    var DistrictID = db.Cities.Where(x => x.ID == ITEM.CityID).Select(R => R.DistrictID).FirstOrDefault();
                    var StateID = db.Districts.Where(x => x.ID == DistrictID).Select(s => s.StateID).FirstOrDefault();
                    ITEM.StateName = db.States.Where(x => x.ID == StateID).Select(k => k.Name).FirstOrDefault();
                }
                return View(OBH);
            }
            else
            {
                Session["Warning"] = "Sorry! You are not authorized user.";
                return RedirectToAction("Index", "Home");
            }
            //return View(db.Suppliers.ToList());
            //return View(lSupplier);
        }

        //
        // GET: /Suppliers/Details/5

        public ActionResult Details(long id)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["IsEzeeloLogin"].ToString() == "1")
            {
                Supplier supplier = db.Suppliers.Single(x => x.ID == id);
                var vpincodeId = db.Pincodes.Where(x => x.ID == supplier.PincodeID).Select(y => y.Name).FirstOrDefault();

                //Added by Priti on 13-2-2019
                var cityid = db.Pincodes.Where(x => x.ID == supplier.PincodeID).Select(l => l.CityID).FirstOrDefault();
                string CityName = db.Cities.Where(x => x.ID == cityid).Select(s => s.Name).FirstOrDefault();
                var DistrictID = db.Cities.Where(x => x.ID == cityid).Select(R => R.DistrictID).FirstOrDefault();
                var StateID = db.Districts.Where(x => x.ID == DistrictID).Select(s => s.StateID).FirstOrDefault();
                string StateName = db.States.Where(x => x.ID == StateID).Select(k => k.Name).FirstOrDefault();
                supplier.StateName = StateName;
                supplier.CityName = CityName;
                ///end by Priti on 13-2-2019
                ViewBag.WarehouseNameAsSupplier = db.Warehouses.Where(x => x.ID == supplier.WarehouseID).Select(m => m.Name).FirstOrDefault();
                supplier.PincodeID = Convert.ToInt32(vpincodeId);
                return View(supplier);
            }
            else
            {
                Session["Warning"] = "Sorry! You are not authorized user.";
                return RedirectToAction("Index", "Home");
            }
        }

        //
        // GET: /Suppliers/Create

        public ActionResult Create()
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["IsEzeeloLogin"].ToString() == "1")
            {
                //Yashaswi 19/4/2018
                var WarehouseList = (new CommonController()).GetAllSupplierWarehouseList();
                if (WarehouseList != null)
                {
                    ViewBag.PossibleWarehouses = WarehouseList;
                }
                else
                {
                    ViewBag.PossibleWarehouses = db.Warehouses.Where(p => p.IsFulfillmentCenter == false && p.IsActive == true).ToList();
                }
                //var query = db.Suppliers.Where(x => x.WarehouseID != null).Select(x => x.WarehouseID).ToList();
                //if (query != null && query.Count>0)
                //{
                //    var WarehouseList = db.Warehouses.ToList();
                //    foreach(var id in query)
                //    {
                //        WarehouseList = WarehouseList.Where(x=>x.ID !=id).ToList();
                //    }
                //    ViewBag.PossibleWarehouses = WarehouseList;  
                //}
                //else
                //{
                //    ViewBag.PossibleWarehouses = db.Warehouses;   
                //}
                ViewBag.PaymentInTerms = new SelectList(db.PaymentInTerms.ToList(), "ID", "Name");  ///Added by Priti on 10/10/2018        
                ViewBag.PossibleStates = db.States;
                ViewBag.PossibleCities = db.Cities;

                ViewBag.PossibleCities = new SelectList(db.Cities.ToList(), "ID", "Name");  ///Added by Priti on 10/10/2018        


                ViewBag.PossiblePincodes = db.Pincodes;
                Supplier supplier = new Supplier();
                supplier.CreateDate = DateTime.UtcNow;
                supplier.IsActive = true;
                supplier.CreateBy = GetPersonalDetailID();
                return View(supplier);
            }
            else
            {
                Session["Warning"] = "Sorry! You are not authorized user.";
                return RedirectToAction("Index", "Home");
            }
        }

        //
        // POST: /Suppliers/Create

        [HttpPost]
        public ActionResult Create(Supplier supplier, string ExportOption)
        {
            var query = db.Suppliers.Where(x => x.WarehouseID == supplier.WarehouseID && x.IsActive == true && x.WarehouseID != null).Select(x => x.WarehouseID).ToList();
            if (query != null && query.Count > 0)
            {
                ViewBag.Message = "Please select another Warehouse! This is already assigned to a supplier.";
                TempData["Message"] = "Please select another Warehouse! This is already assigned to a supplier.";
                //ViewBag.PossibleWarehouses = db.Warehouses;
                //Yashaswi 19/4/2018
                var WarehouseList = (new CommonController()).GetAllSupplierWarehouseList();
                if (WarehouseList != null)
                {
                    ViewBag.PossibleWarehouses = WarehouseList;
                }
                else
                {
                    ViewBag.PossibleWarehouses = db.Warehouses.Where(p => p.IsFulfillmentCenter == false && p.IsActive == true).ToList();
                }

                return View(supplier);
            }
            else
            {
                ViewBag.Message = null;
                TempData["Message"] = null;
            }

            long PersonalDetailID = GetPersonalDetailID();
            if (ModelState.IsValid)
            {
                Supplier objSupplier = new Supplier();
                objSupplier.Name = supplier.Name;
                objSupplier.FSSILicenseNo = supplier.FSSILicenseNo;  //Added by Priti
                objSupplier.InsecticidesLicenseNo = supplier.InsecticidesLicenseNo;  //Added by Priti
                objSupplier.CityID = supplier.CityID;//Added by Priti on 13-2-2019
                objSupplier.StateID = supplier.StateID; //Added by Priti on 13-2-2019
                objSupplier.CityName = supplier.CityName;  //Added by Priti on 13-2-2019
                ViewBag.CITYNAME = objSupplier.CityName;
                objSupplier.StateName = supplier.StateName;  //Added by Priti on 13-2-2019
                objSupplier.PaymentInTermsID = Convert.ToInt64(ExportOption);////Added by Priti
                objSupplier.ContactPerson = supplier.ContactPerson;
                objSupplier.Mobile = supplier.Mobile;
                objSupplier.Landline = supplier.Landline;
                objSupplier.GSTNumber = supplier.GSTNumber;
                objSupplier.Email = supplier.Email;
                objSupplier.FAX = supplier.FAX;
                objSupplier.PAN = supplier.PAN;
                objSupplier.Website = supplier.Website;
                objSupplier.CC = supplier.CC;
                objSupplier.NetworkIP = CommonFunctions.GetClientIP();
                objSupplier.DeviceID = "X";
                objSupplier.DeviceType = "X";

                string pincode = supplier.PincodeID.ToString();
                //Pincode lPincode = db.Pincodes.Single(x => x.Name == Convert.ToString(supplier.PincodeID));
                var vpincodeId = db.Pincodes.Where(x => x.Name == pincode).Select(y => y.ID).FirstOrDefault();


                //Added by Priti on 13-2-2019
                var cityid = db.Pincodes.Where(x => x.ID == supplier.PincodeID).Select(l => l.CityID).FirstOrDefault();
                objSupplier.CityID = cityid;
                string CityName = db.Cities.Where(x => x.ID == objSupplier.CityID).Select(s => s.Name).FirstOrDefault();
                objSupplier.CityName = supplier.CityName;       //Added by Priti on 13-2-2019

                var DistrictID = db.Cities.Where(x => x.ID == cityid).Select(R => R.DistrictID).FirstOrDefault();
                var StateID = db.Districts.Where(x => x.ID == DistrictID).Select(s => s.StateID).FirstOrDefault();
                string StateName = db.States.Where(x => x.ID == StateID).Select(k => k.Name).FirstOrDefault();
                //end by Priti on 13-2-2019
                if (vpincodeId != null && vpincodeId > 0)
                {
                    objSupplier.PincodeID = vpincodeId;


                }
                else
                {
                    View();
                }

                //Added by Priti on 13-2-2019


                if (vpincodeId != null && vpincodeId > 0)
                {
                    objSupplier.CityName = CityName;
                }

                else
                {
                    View();
                }
                //Added by Priti on 13-2-2019
                if (vpincodeId != null && vpincodeId > 0)
                {
                    objSupplier.StateName = StateName;
                }

                else
                {
                    View();
                }



                objSupplier.CityName = supplier.CityName;       //Added by Priti on 13-2-2019
                objSupplier.StateName = supplier.StateName;       //Added by Priti on 13-2-2019
                objSupplier.Address = supplier.Address;
                objSupplier.WarehouseID = supplier.WarehouseID;
                objSupplier.IsActive = supplier.IsActive;
                objSupplier.CreateDate = DateTime.UtcNow;
                objSupplier.CreateBy = PersonalDetailID;
                objSupplier.SupplierCode = GetNextSupplierCode();
                db.Suppliers.Add(objSupplier);

                db.SaveChanges();
                Session["Success"] = "Record Inserted Successfully.";//yashaswi 30/3/2018
                return RedirectToAction("Index");
            }

            //ViewBag.PossibleWarehouses = db.Warehouses;
            //Yashaswi 19/4/2018
            var WarehouseList_ = (new CommonController()).GetAllSupplierWarehouseList();
            if (WarehouseList_ != null)
            {
                ViewBag.PossibleWarehouses = WarehouseList_;
            }
            else
            {
                ViewBag.PossibleWarehouses = db.Warehouses.Where(p => p.IsFulfillmentCenter == false && p.IsActive == true).ToList();
            }
            ViewBag.PossibleStates = db.States;
            ViewBag.PossibleCities = db.Cities;
            ViewBag.PossiblePincodes = db.Pincodes;
            return View(supplier);
        }

        //
        // GET: /Supplier/Edit/5

        public ActionResult Edit(long id)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["IsEzeeloLogin"].ToString() == "1")
            {
                Supplier supplier = db.Suppliers.Single(x => x.ID == id);
                var vpincodeId = db.Pincodes.Where(x => x.ID == supplier.PincodeID).Select(y => y.Name).FirstOrDefault();
                //Added by Priti on 13-2-2019
                var cityid = db.Pincodes.Where(x => x.ID == supplier.PincodeID).Select(l => l.CityID).FirstOrDefault();
                string CityName = db.Cities.Where(x => x.ID == cityid).Select(s => s.Name).FirstOrDefault();

                var DistrictID = db.Cities.Where(x => x.ID == cityid).Select(R => R.DistrictID).FirstOrDefault();
                var StateID = db.Districts.Where(x => x.ID == DistrictID).Select(s => s.StateID).FirstOrDefault();
                string StateName = db.States.Where(x => x.ID == StateID).Select(k => k.Name).FirstOrDefault();

                supplier.PincodeID = Convert.ToInt32(vpincodeId);
                supplier.CityName = CityName;    ///added by Priti on 13-2-2019
                supplier.StateName = StateName;           ///added by Priti on 13-2-2019



                ViewBag.PaymentInTerms = new SelectList(db.PaymentInTerms.ToList(), "ID", "Name");   //ADDED BY Priti
                ViewBag.PossibleStates = db.States;
                ViewBag.PossibleCities = db.Cities;

                ViewBag.PossibleCities = new SelectList(db.Cities.ToList(), "ID", "Name");  ///Added by Priti on 13/02/2018        


                ViewBag.PossiblePincodes = db.Pincodes;
                //ViewBag.PossibleWarehouses = db.Warehouses;
                //Yashaswi 19/4/2018
                var WarehouseList = (new CommonController()).GetAllSupplierWarehouseList();
                if (WarehouseList != null)
                {
                    WarehouseList = WarehouseList.Concat(db.Warehouses.Where(w => w.ID == supplier.WarehouseID).ToList()).ToList();
                    ViewBag.PossibleWarehouses = WarehouseList;
                }
                else
                {
                    ViewBag.PossibleWarehouses = db.Warehouses.Where(p => p.IsFulfillmentCenter == false && p.IsActive == true).ToList();
                }
                return View(supplier);
            }
            else
            {
                Session["Warning"] = "Sorry! You are not authorized user.";
                return RedirectToAction("Index", "Home");
            }
        }

        //
        // POST: /Supplier/Edit/5

        [HttpPost]
        public ActionResult Edit(Supplier supplier, string ExportOption)
        {

            var query = db.Suppliers.Where(x => x.WarehouseID != null && x.ID != supplier.ID).Select(x => x.WarehouseID).ToList();
            if (query != null && query.Count > 0)
            {
                foreach (var id in query)
                {
                    if (id == supplier.WarehouseID)
                    {
                        ViewBag.Message = "Please select another Warehouse! This is already assigned to a supplier.";
                        TempData["Message"] = "Please select another Warehouse! This is already assigned to a supplier.";
                        //ViewBag.PossibleWarehouses = db.Warehouses;
                        //Yashaswi 19/4/2018
                        var WarehouseList = (new CommonController()).GetAllSupplierWarehouseList();
                        if (WarehouseList != null)
                        {
                            WarehouseList = WarehouseList.Concat(db.Warehouses.Where(w => w.ID == id).ToList()).ToList();
                            ViewBag.PossibleWarehouses = WarehouseList;
                        }
                        else
                        {
                            ViewBag.PossibleWarehouses = db.Warehouses.Where(p => p.IsFulfillmentCenter == false && p.IsActive == true).ToList();
                        }
                        return View(supplier);
                    }
                    else
                    {
                        ViewBag.Message = null;
                        TempData["Message"] = null;
                    }
                }
            }
            else
            {
                ViewBag.Message = null;
            }

            if (ModelState.IsValid)
            {
                long PersonalDetailID = GetPersonalDetailID();
                string pincode = supplier.PincodeID.ToString();
                var vpincodeId = db.Pincodes.Where(x => x.Name == pincode).Select(y => y.ID).FirstOrDefault();

                //Added by Priti on 13-2-2019

                var cityid = db.Pincodes.Where(x => x.ID == vpincodeId).Select(l => l.CityID).FirstOrDefault();
                supplier.CityID = cityid;
                string CityName = db.Cities.Where(x => x.ID == cityid).Select(s => s.Name).FirstOrDefault();
                supplier.CityName = CityName;
                var DistrictID = db.Cities.Where(x => x.ID == cityid).Select(R => R.DistrictID).FirstOrDefault();
                var StateID = db.Districts.Where(x => x.ID == DistrictID).Select(s => s.StateID).FirstOrDefault();
                string StateName = db.States.Where(x => x.ID == StateID).Select(k => k.Name).FirstOrDefault();

                // end Added by Priti on 13-2-2019

                if (vpincodeId != null && vpincodeId > 0)
                {
                    supplier.PincodeID = vpincodeId;
                }
                else
                {
                    return View(supplier);
                }

                //Added by Priti on 13-2-2019
                if (vpincodeId != null && vpincodeId > 0)
                {
                    supplier.CityName = CityName;
                }
                else
                {
                    return View(supplier);
                }
                //Added by Priti on 13-2-2019
                if (vpincodeId != null && vpincodeId > 0)
                {
                    supplier.StateName = StateName;
                }
                else
                {
                    return View(supplier);
                }

                supplier.PaymentInTermsID = Convert.ToInt64(ExportOption);   ///aDDED BY Priti
                supplier.SupplierCode = supplier.SupplierCode;


                supplier.CityName = supplier.CityName;       //Added by Priti on 13-2-2019
                supplier.StateName = supplier.StateName;       //Added by Priti on 13-2-2019
                supplier.WarehouseID = supplier.WarehouseID;
                supplier.ModifyBy = PersonalDetailID;
                supplier.ModifyDate = DateTime.UtcNow;
                supplier.NetworkIP = CommonFunctions.GetClientIP();
                supplier.DeviceID = "X";
                supplier.DeviceType = "X";
                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();

                Session["Success"] = "Record Updated Successfully.";//yashaswi 30/3/2018
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        //
        // GET: /Supplier/Delete/5

        public ActionResult Delete(long id)
        {
            Supplier supplier = db.Suppliers.Single(x => x.ID == id);
            return View(supplier);
        }

        //
        // POST: /Supplier/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            Supplier supplier = db.Suppliers.Single(x => x.ID == id);
            db.Suppliers.Remove(supplier);
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


        #region Private Methods
        public long GetPersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            long UserLoginID = Convert.ToInt64(Session["USER_LOGIN_ID"]);
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


        public ActionResult GetAddress(string Pincode)
        {
            var pincode = db.Pincodes.Where(x => x.Name == Pincode).Select(y => y.ID).FirstOrDefault();
            if (pincode == null || pincode == 0)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("1", JsonRequestBehavior.AllowGet);
            }
        }


        private string GetNextSupplierCode()
        {
            string newOrderCode = string.Empty;
            int lYear = 0;
            int lMonth = 0;
            int lDay = 0;
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Year.ToString(), out lYear);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Month.ToString(), out lMonth);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Day.ToString(), out lDay);
            string lOrderPrefix = "EZSL" + lYear.ToString().Substring(2, 2) + lMonth.ToString("00") + lDay.ToString("00");

            try
            {
                OrderManagement lOrderManagement = new OrderManagement();
                int lEZSL = GetNextEZSL();
                if (lEZSL > 0)
                {
                    newOrderCode = lOrderPrefix + lEZSL.ToString("00000");
                    return newOrderCode;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }


        private int GetNextEZSL()
        {
            int lEZSL = -1;

            try
            {
                DataTable lDataTableCustomerOrder = new DataTable();
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand sqlComm = new SqlCommand("SelectNextEZSL", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                //sqlComm.Parameters.AddWithValue("@pFranchiseID", SqlDbType.Int).Value = pFranchiseID;
                con.Open();
                //object o = sqlComm.ExecuteScalar();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    lEZSL = Convert.ToInt32(dt.Rows[0][0]);
                }
                con.Close();
                return lEZSL;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[SuppliersController -> GetNextEZSL]", "Problem in getting EZSL" + Environment.NewLine + ex.Message);
            }

        }
        #endregion


        //Yashaswi 14-03-2019 To show EVW list to share outside supplier
        public ActionResult EVWList()
        {
            try
            {
                if (Session["USER_NAME"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (Session["IsEzeeloLogin"].ToString() == "1")
                {
                    List<EVWViewModel> listEVWViewModel = db.Warehouses.Where(p => p.Entity == "EVW" && p.IsActive == true)
                         .Select(p => new EVWViewModel
                         {
                             ID = p.ID,
                             Name = p.Name,
                             GSTNo = p.GSTNumber,
                             Pincode = db.Pincodes.FirstOrDefault(q => q.ID == p.PincodeID).Name,
                             StateName = db.States.FirstOrDefault(s => s.ID == db.Districts.FirstOrDefault(d => d.ID == db.Cities.FirstOrDefault(c => c.ID == db.Pincodes.FirstOrDefault(q => q.ID == p.PincodeID).CityID).DistrictID).StateID).Name,
                         }).ToList();
                    return View(listEVWViewModel);
                }
                else
                {
                    Session["Warning"] = "Sorry! You are not authorized user.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        //Yashaswi 15-03-2019 To Add or View share supplier
        public ActionResult AddOrViewSupplier(long Id)
        {
            try
            {
                if (Session["USER_NAME"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (Session["IsEzeeloLogin"].ToString() == "1")
                {
                    EVWViewModel EVWViewModel = db.Warehouses.Where(p => p.Entity == "EVW" && p.IsActive == true)
                        .Where(p => p.ID == Id)
                        .Select(p => new EVWViewModel
                        {
                            ID = p.ID,
                            Name = p.Name,
                            GSTNo = p.GSTNumber,
                            Pincode = db.Pincodes.FirstOrDefault(q => q.ID == p.PincodeID).Name,
                            StateName = db.States.FirstOrDefault(s => s.ID == db.Districts.FirstOrDefault(d => d.ID == db.Cities.FirstOrDefault(c => c.ID == db.Pincodes.FirstOrDefault(q => q.ID == p.PincodeID).CityID).DistrictID).StateID).Name,
                            ListForEVW = db.Suppliers.Where(s => s.WarehouseID == null)
                                                .Select(s => new EVWV_List
                                                {
                                                    ID = s.ID,
                                                    Name = s.Name,
                                                    IsSelected = db.EVWsSuppliers.Any(e => e.IsActive == true && e.SupplierId == s.ID && e.WarehouseId == Id),
                                                }).OrderBy(s => s.Name).ToList()
                        }).FirstOrDefault();
                    return View(EVWViewModel);
                }
                else
                {
                    Session["Warning"] = "Sorry! You are not authorized user.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddOrViewSupplier(EVWViewModel obj)
        {
            try
            {
                long PersonalDetailID = GetPersonalDetailID();
                var list = db.EVWsSuppliers.Where(p => p.IsActive == true && p.WarehouseId == obj.ID).Select(p => new
                {
                    WarehouseId = p.WarehouseId,
                    SupplierId = p.SupplierId
                }).AsEnumerable()
                .Where(p => !(obj.ListForEVW.Where(q => q.IsSelected).Select(q => q.ID).ToList()).Contains(p.SupplierId)).ToList();

                foreach (var item in list)
                {
                    EVWsSupplier o = db.EVWsSuppliers.FirstOrDefault(p => p.WarehouseId == item.WarehouseId && p.SupplierId == item.SupplierId);
                    o.IsActive = false;
                    o.ModifyBy = PersonalDetailID;
                    o.ModifyDate = DateTime.Now;
                    o.NetworkIP = CommonFunctions.GetClientIP();
                    db.SaveChanges();
                }
                if (obj.ListForEVW.Any(s => s.IsSelected))
                {
                    foreach (var item in obj.ListForEVW.Where(s => s.IsSelected).ToList())
                    {
                        EVWsSupplier o = db.EVWsSuppliers.FirstOrDefault(p => p.WarehouseId == obj.ID && p.SupplierId == item.ID);
                        if (o == null)
                        {
                            EVWsSupplier obj_EVWsSupplier = new EVWsSupplier();
                            obj_EVWsSupplier.IsActive = true;
                            obj_EVWsSupplier.NetworkIP = CommonFunctions.GetClientIP();
                            obj_EVWsSupplier.SupplierId = item.ID;
                            obj_EVWsSupplier.WarehouseId = obj.ID;
                            obj_EVWsSupplier.CreateBy = PersonalDetailID;
                            obj_EVWsSupplier.CreateDate = DateTime.Now;
                            db.EVWsSuppliers.Add(obj_EVWsSupplier);
                            db.SaveChanges();
                        }
                        else
                        {
                            if (o.IsActive == false)
                            {
                                o.IsActive = true;
                                o.NetworkIP = CommonFunctions.GetClientIP();
                                o.SupplierId = item.ID;
                                o.WarehouseId = obj.ID;
                                o.ModifyBy = PersonalDetailID;
                                o.ModifyDate = DateTime.Now;
                                db.EVWsSuppliers.Add(o);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                Session["Success"] = "Data Save Successfully.";
                return RedirectToAction("EVWList", "Suppliers");
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message;
                return RedirectToAction("EVWList", "Suppliers");
            }
        }
    }
}