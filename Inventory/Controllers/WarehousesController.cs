using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using Inventory.Models;
using System.Data.SqlClient;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using System.Transactions;
using System.Web.Configuration;

namespace Inventory.Controllers
{
    public class WarehousesController : Controller
    {
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();

        //
        // GET: /Warehouses/

        public ActionResult Index(bool flag, string Entity) /*Yashaswi 4-12-2018 For EVW add new Entity param*/
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["IsEzeeloLogin"].ToString() == "1")
            {
                List<WarehouseViewModel> lWarehouseViewModel = new List<WarehouseViewModel>();
                long WarehouseID = 0;
                if (Session["WarehouseID"] != null)
                {
                    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                }
                Warehouse objWarehouse = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseID);

                long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
                if (WarehouseID > 0 && WarehouseID != EzeeloWarehouseId && objWarehouse.Entity.Trim() != "EVW")
                {
                    flag = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseID).IsFulfillmentCenter;
                    lWarehouseViewModel = (from w in db.Warehouses
                                           join p in db.Pincodes on w.PincodeID equals p.ID
                                           join c in db.Cities on p.CityID equals c.ID
                                           where w.ID == WarehouseID
                                           select new WarehouseViewModel
                                           {
                                               ID = w.ID,
                                               Name = w.Name,
                                               WarehouseCode = w.WarehouseCode,
                                               GSTNumber = w.GSTNumber,
                                               ServiceNumber = w.ServiceNumber,
                                               CityName = c.Name,

                                               IsFulfillmentCenter = w.IsFulfillmentCenter,
                                               IsActive = w.IsActive,
                                               Entity = w.Entity.Trim() //Yashaswi 4-12-2018 For EVW
                                           }).ToList();

                }
                else
                {

                    lWarehouseViewModel = (from w in db.Warehouses
                                           join p in db.Pincodes on w.PincodeID equals p.ID
                                           join c in db.Cities on p.CityID equals c.ID
                                           select new WarehouseViewModel
                                           {
                                               ID = w.ID,
                                               Name = w.Name,
                                               WarehouseCode = w.WarehouseCode,
                                               GSTNumber = w.GSTNumber,

                                               ServiceNumber = w.ServiceNumber,
                                               CityName = c.Name,
                                               IsActive = w.IsActive,
                                               IsFulfillmentCenter = w.IsFulfillmentCenter,
                                               FC_Count = db.Warehouses.Count(m => m.DistributorId == w.ID),
                                               Entity = w.Entity.Trim() //Yashaswi 4-12-2018 For EVW
                                           }).OrderByDescending(w => w.ID).ToList();
                }
                if (Entity == "FV")
                {
                    ViewBag.Heading = "Fulfillment Vendors";
                    lWarehouseViewModel = lWarehouseViewModel.Where(p => p.IsFulfillmentCenter == true && p.Entity == Entity).ToList(); //Yashaswi 4-12-2018 For EVW
                }
                else if (Entity == "DV")
                {
                    ViewBag.Heading = "Distribution Vendors";
                    lWarehouseViewModel = lWarehouseViewModel.Where(p => p.IsFulfillmentCenter == false && p.Entity == Entity).ToList();
                }
                else if (Entity == "HO")
                {
                    ViewBag.Heading = "Ezeelo HO";
                    lWarehouseViewModel = lWarehouseViewModel.Where(p => p.IsFulfillmentCenter == false && p.Entity == "HO").ToList();
                }
                else
                {
                    ViewBag.Heading = "Ezeelo Warehouses";
                    lWarehouseViewModel = lWarehouseViewModel.Where(p => p.IsFulfillmentCenter == false && p.Entity == "EVW").ToList();
                }
                ViewBag.flag = flag;
                ViewBag.Entity = Entity; //Yashaswi 4-12-2018 For EVW
                return View("Index", lWarehouseViewModel);
            }
            else
            {
                Session["Warning"] = "Sorry! You are not authorized user.";
                return RedirectToAction("Index", "Home");
            }
        }

        //
        // GET: /Warehouses/Details/5

        public ViewResult Details(long id)
        {
            WarehouseViewModel objw = new WarehouseViewModel();
            Warehouse objWarehouses = db.Warehouses.Where(x => x.ID == id).FirstOrDefault();
            BusinessDetail objBusinessDetails = db.BusinessDetails.Where(x => x.ID == objWarehouses.BusinessDetailID).FirstOrDefault();
            objw.lWarehouses = objWarehouses;
            objw.BusinessDetails = objBusinessDetails;
            objw.IsFulfillmentCenter = objWarehouses.IsFulfillmentCenter; //Yashaswi 6/4/2018
            objw.ID = id;
            long cityId = db.Pincodes.Where(x => x.ID == objWarehouses.PincodeID).Select(x => x.CityID).First();

            string cityName = db.Cities.Where(x => x.ID == cityId).Select(x => x.Name).First();
            objw.CityName = cityName;
            objw.Entity = objWarehouses.Entity.Trim(); //Yashaswi 4-12-2018 For EVW
            var lFranchises = db.Franchises.Where(x => x.IsActive == true && x.ContactPerson != "").Select(x => new { x.ID, x.ContactPerson, x.IsActive })
                                                        .Distinct()
                                                       .ToList();

            var lWarehouseFranchise = db.WarehouseFranchises.Where(x => x.WarehouseID == id && x.IsActive == true).Select(x => new { x.ID, x.FranchiseID, x.IsActive }).ToList();

            List<FranchiseModel> lFranchiseList = new List<FranchiseModel>();
            //if (Session["WarehouseID"] != null && Convert.ToInt32(Session["WarehouseID"]) > 0)
            //{
            for (int i = 0; i < lFranchises.Count(); i++)
            {
                FranchiseModel lFranchise = new FranchiseModel();

                lFranchise.ID = Convert.ToInt32(lFranchises[i].ID);
                lFranchise.Name = CommonFunctions.ConvertStringToCamelCase(Convert.ToString(lFranchises[i].ContactPerson));

                if (lWarehouseFranchise != null)
                {
                    for (int j = 0; j < lWarehouseFranchise.Count(); j++)
                    {
                        if (lFranchise.ID == lWarehouseFranchise[j].FranchiseID)
                        {
                            lFranchise.IsSelected = true;
                            lFranchiseList.Add(lFranchise);
                            break;
                        }
                    }
                }
            }
            //}
            objw.franchiseList = lFranchiseList;
            return View(objw);
        }

        //
        // GET: /Warehouses/Create

        //Yashaswi 5/4/2018
        public ActionResult Create(bool flag, string Entity) /*Yashaswi 4-12-2018 For EVW add new Entity param*/
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["IsEzeeloLogin"].ToString() == "1")
            {
                /*Yashaswi 4-12-2018 For EVW*/
                //if (Entity == "HO")
                //{
                //    Entity = "EVW";
                //}
                WarehouseViewModel warehosue = new WarehouseViewModel();

                List<SelectListItem> listSelectListItems = new List<SelectListItem>();

                //Yashaswi 5/4/2018 For Sepration of FC & DV



                long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
                if (flag)
                {
                    var lFranchises = db.Franchises.Where(x => x.IsActive == true && x.ContactPerson != ""
                        && !(db.WarehouseFranchises.Join(db.Warehouses, wf => wf.WarehouseID, w => w.ID, (wf, w) =>
                            new
                            {
                                FranchaiseId = wf.FranchiseID,
                                WarehouseID = wf.WarehouseID,
                                IsFulfillmentCenter = w.IsFulfillmentCenter
                            }).Where(p => p.IsFulfillmentCenter == true && p.WarehouseID != EzeeloWarehouseId).ToList().Select(p => p.FranchaiseId)).Contains(x.ID))
                    .Select(x => new { x.ID, x.ContactPerson, x.IsActive })
                    .Distinct()
                    .ToList();
                    List<FranchiseModel> lFranchiseList = new List<FranchiseModel>();
                    if (lFranchises.Count() > 0)
                    {
                        for (int i = 0; i < lFranchises.Count(); i++)
                        {
                            FranchiseModel lFranchise = new FranchiseModel();

                            lFranchise.ID = Convert.ToInt32(lFranchises[i].ID);
                            lFranchise.Name = CommonFunctions.ConvertStringToCamelCase(Convert.ToString(lFranchises[i].ContactPerson));
                            lFranchiseList.Add(lFranchise);
                        }
                    }
                    warehosue.franchiseList = lFranchiseList;
                }
                else
                {
                    var lFranchises = db.Warehouses.Where(p => p.IsActive == true && p.IsFulfillmentCenter == true
                        && p.DistributorId == null).Select(p => new { p.ID, ContactPerson = p.Name }).ToList();
                    List<FranchiseModel> lFranchiseList = new List<FranchiseModel>();
                    if (lFranchises.Count() > 0)
                    {
                        for (int i = 0; i < lFranchises.Count(); i++)
                        {
                            FranchiseModel lFranchise = new FranchiseModel();

                            lFranchise.ID = Convert.ToInt32(lFranchises[i].ID);
                            lFranchise.Name = CommonFunctions.ConvertStringToCamelCase(Convert.ToString(lFranchises[i].ContactPerson));
                            lFranchiseList.Add(lFranchise);
                        }
                    }

                    warehosue.franchiseList = lFranchiseList;
                }
                //Start Yashaswi 4-12-2018 For EVW           
                if (Entity == "FV")
                {
                    ViewBag.Heading = "Fulfillment Vendors";
                }
                else if (Entity == "DV")
                {
                    ViewBag.Heading = "Distribution Vendors";
                }
                else if (Entity == "HO")
                {
                    ViewBag.Heading = "Ezeelo HO";
                }
                else
                {
                    ViewBag.Heading = "Ezeelo Warehouses";
                }
                //BY PRITI
                ViewBag.StateID = new SelectList(db.States.Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.CityID = new SelectList(db.Cities.Where(c => c.IsActive == true).ToList(), "ID", "Name");
                warehosue.Entity = Entity;
                //End Yashaswi 4-12-2018 For EVW            
                warehosue.IsActive = true;
                warehosue.IsFulfillmentCenter = flag;

                return View(warehosue);
            }
            else
            {
                Session["Warning"] = "Sorry! You are not authorized user.";
                return RedirectToAction("Index", "Home");
            }
        }



        //
        // POST: /Warehouses/Create

        [HttpPost]
        public ActionResult Create(WarehouseViewModel warehouse)
        {

            var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));
            using (TransactionScope tscope = new TransactionScope())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        long PersonalDetailID = 1;
                        if (Session["USER_LOGIN_ID"] != null)
                        {
                            long ID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
                            PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == ID).Select(x => x.ID).First());
                        }

                        if (db.Warehouses.Any(x => x.Name == warehouse.Name))
                        {
                            TempData["Message"] = "Error: Warehouse Name already exist!";
                            return View(warehouse);
                        }

                        if (db.UserLogins.Any(x => x.Email == warehouse.BusinessDetails.Email))
                        {
                            TempData["Message"] = "Error: Email already exists!";
                            return View(warehouse);
                        }

                        //Insert into UserLogin
                        UserLogin objLogin = new UserLogin();
                        objLogin.Mobile = warehouse.BusinessDetails.Mobile;
                        objLogin.Email = warehouse.BusinessDetails.Email;
                        objLogin.Password = warehouse.Password;
                        objLogin.IsLocked = false;
                        objLogin.CreateBy = PersonalDetailID;
                        objLogin.CreateDate = DateTime.UtcNow;
                        objLogin.NetworkIP = CommonFunctions.GetClientIP();
                        objLogin.DeviceID = "X";
                        objLogin.DeviceType = "X";
                        db.UserLogins.Add(objLogin);
                        db.SaveChanges();
                        long UserLoginID = objLogin.ID;

                        //Insert into BusinessDetail
                        BusinessDetail objBDetail = new BusinessDetail();
                        objBDetail.UserLoginID = UserLoginID;
                        objBDetail.Name = warehouse.Name;
                        objBDetail.BusinessTypeID = 15;
                        objBDetail.ContactPerson = warehouse.BusinessDetails.ContactPerson;
                        objBDetail.Mobile = warehouse.BusinessDetails.Mobile;
                        objBDetail.Email = warehouse.BusinessDetails.Email;
                        objBDetail.Landline1 = warehouse.BusinessDetails.Landline1;
                        objBDetail.Landline2 = warehouse.BusinessDetails.Landline2;
                        objBDetail.FAX = warehouse.BusinessDetails.FAX;
                        objBDetail.Address = warehouse.BusinessDetails.Address;

                        string pincode = warehouse.PincodeID.ToString();
                        var vpincodeId = db.Pincodes.Where(x => x.Name == pincode).Select(y => y.ID).FirstOrDefault();
                        if (vpincodeId != null && vpincodeId > 0)
                        {
                            objBDetail.PincodeID = vpincodeId;
                        }
                        else
                        {
                            View();
                        }

                        objBDetail.Website = warehouse.BusinessDetails.Website;
                        objBDetail.IsActive = true;
                        objBDetail.CreateBy = PersonalDetailID;
                        objBDetail.CreateDate = DateTime.UtcNow;
                        objBDetail.NetworkIP = CommonFunctions.GetClientIP();
                        objBDetail.DeviceID = "X";
                        objBDetail.DeviceType = "X";
                        db.BusinessDetails.Add(objBDetail);
                        db.SaveChanges();

                        long BusinessDetailID = objBDetail.ID;

                        //Insert into PersonalDetail
                        PersonalDetail objPDetail = new PersonalDetail();
                        objPDetail.UserLoginID = UserLoginID;
                        objPDetail.SalutationID = 1;
                        objPDetail.FirstName = warehouse.BusinessDetails.ContactPerson;
                        objPDetail.PincodeID = vpincodeId;
                        objPDetail.Address = warehouse.BusinessDetails.Address;
                        objPDetail.IsActive = true;
                        objPDetail.CreateBy = PersonalDetailID;
                        objPDetail.CreateDate = DateTime.UtcNow;
                        objPDetail.NetworkIP = CommonFunctions.GetClientIP();
                        objPDetail.DeviceID = "X";
                        objPDetail.DeviceType = "X";
                        db.PersonalDetails.Add(objPDetail);
                        db.SaveChanges();

                        //Insert into Warehouse
                        Warehouse objWarehouse = new Warehouse();
                        objWarehouse.Name = warehouse.Name;
                        objWarehouse.WarehouseCode = GetNextWarehouseCode();
                        objWarehouse.BusinessDetailID = BusinessDetailID;
                        objWarehouse.FSSILicenseNo = warehouse.FSSILicenseNo;  //by priti  ON  11-3-2018
                        objWarehouse.PAN = warehouse.PAN; //by priti 11-3-2018
                        objWarehouse.GSTNumber = warehouse.GSTNumber;
                        objWarehouse.ServiceNumber = warehouse.ServiceNumber;
                        objWarehouse.ServiceLevel = 1;
                        objWarehouse.PincodeID = vpincodeId;
                        objWarehouse.NearbyTransport = warehouse.NearbyTransport;
                        objWarehouse.Measurement = warehouse.Measurement;
                        objWarehouse.FloorSpace = warehouse.FloorSpace;
                        objWarehouse.Volume = warehouse.Volume;
                        objWarehouse.CustomEntry = warehouse.CustomEntry;
                        objWarehouse.CustomExit = warehouse.CustomExit;
                        objWarehouse.IsFulfillmentCenter = warehouse.IsFulfillmentCenter;
                        objWarehouse.IsActive = true;
                        objWarehouse.Entity = warehouse.Entity.Trim(); //Yashaswi 4-12-2018 For EVW
                        objWarehouse.CreateBy = PersonalDetailID;
                        objWarehouse.CreateDate = DateTime.UtcNow;
                        objWarehouse.NetworkIP = CommonFunctions.GetClientIP();
                        objWarehouse.DeviceID = "X";
                        objWarehouse.DeviceType = "X";
                        db.Warehouses.Add(objWarehouse);
                        db.SaveChanges();
                        long WarehouseID = objWarehouse.ID;


                        if (objWarehouse.Entity.Trim() == "DV" || objWarehouse.Entity.Trim() == "FV") //Yashaswi 4-12-2018 For EVW If condition add
                        {
                            var temp = (from n in warehouse.franchiseList
                                        where n.IsSelected == true
                                        select new WarehouseFranchise
                                        {
                                            WarehouseID = Convert.ToInt32(WarehouseID),
                                            FranchiseID = n.ID,
                                            IsActive = true,
                                        }
                            ).DefaultIfEmpty().ToList();
                            if (warehouse.IsFulfillmentCenter)
                            {
                                List<WarehouseFranchise> obj_WarehouseFranchise = new List<WarehouseFranchise>();
                                if (objWarehouse.IsFulfillmentCenter)
                                {
                                    WarehouseFranchise objWF = new WarehouseFranchise();
                                    objWF.WarehouseID = Convert.ToInt32(WarehouseID);
                                    objWF.FranchiseID = warehouse.Franchise;
                                    objWF.IsActive = true;
                                    obj_WarehouseFranchise.Add(objWF);
                                }
                                temp = (from n in obj_WarehouseFranchise
                                        select new WarehouseFranchise
                                        {
                                            WarehouseID = Convert.ToInt32(WarehouseID),
                                            FranchiseID = n.FranchiseID,
                                            IsActive = true,
                                        }).DefaultIfEmpty().ToList();
                            }
                            else
                            {

                                List<WarehouseFranchise> list_WarehouseFranchise = new List<WarehouseFranchise>();
                                foreach (WarehouseFranchise pa in temp)
                                {
                                    Warehouse obj = db.Warehouses.SingleOrDefault(p => p.ID == pa.FranchiseID);
                                    obj.DistributorId = WarehouseID;
                                    db.SaveChanges();
                                    WarehouseFranchise objWF = new WarehouseFranchise();
                                    objWF = db.WarehouseFranchises.FirstOrDefault(p => p.WarehouseID == pa.FranchiseID);
                                    list_WarehouseFranchise.Add(objWF);
                                }
                                temp = (from n in list_WarehouseFranchise
                                        select new WarehouseFranchise
                                        {
                                            WarehouseID = Convert.ToInt32(WarehouseID),
                                            FranchiseID = n.FranchiseID,
                                            IsActive = true,
                                        }).DefaultIfEmpty().ToList();

                            }

                            if (temp.Count > 0)
                            {
                                DataTable dt = new DataTable();
                                dt.Columns.Add("FranchiseID");
                                dt.Columns.Add("IsActive");
                                dt.Columns.Add("WarehouseID");


                                foreach (WarehouseFranchise pa in temp)
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["FranchiseID"] = pa.FranchiseID;
                                    dr["IsActive"] = pa.IsActive;
                                    dr["WarehouseID"] = pa.WarehouseID;

                                    dt.Rows.Add(dr);
                                }
                                string msg = InsertUpdate_WarehouseFranchise(0, Convert.ToInt32(WarehouseID), true, dt, "INSERT");

                            }
                        }
                        ViewBag.StateID = new SelectList(db.States.Where(c => c.IsActive == true).ToList(), "ID", "Name");
                        tscope.Complete();

                        //Yashaswi 31/5/2018
                        long? ID_ = warehouse.ID;

                        //BY PRITI

                        if (warehouse.IsFulfillmentCenter == true)
                        {
                            Session["Success"] = "Record Inserted Successfully.";
                            return RedirectToAction("Index", new { flag = warehouse.IsFulfillmentCenter, Entity = warehouse.Entity });//Yashaswi 4-12-2018 For EVW
                        }
                        else
                        {
                            //Start Yashaswi 4-12-2018 For EVW
                            if (warehouse.Entity.Trim() == "DV")
                            {
                                Session["Success"] = "Distribution Center Created Successfully. Please Add Respective Margin For DV and FV.";
                                return RedirectToAction("Index", new { flag = warehouse.IsFulfillmentCenter, Entity = warehouse.Entity });
                            }
                            else
                            {
                                Session["Success"] = "Ezeelo Warehouse Created Successfully. Please Add Respective Margin.";
                                return RedirectToAction("Index", new { flag = warehouse.IsFulfillmentCenter, Entity = warehouse.Entity });
                            }
                            //End Yashaswi 4-12-2018 For EVW
                        }
                    }
                }
                catch (Exception ex)
                {
                    Transaction.Current.Rollback();
                    tscope.Dispose();
                    throw ex;
                }

                return View(warehouse);
            }
        }



        private string GetNextWarehouseCode()
        {
            string newOrderCode = string.Empty;
            int lYear = 0;
            int lMonth = 0;
            int lDay = 0;
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Year.ToString(), out lYear);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Month.ToString(), out lMonth);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Day.ToString(), out lDay);
            string lOrderPrefix = "EZWH" + lYear.ToString().Substring(2, 2) + lMonth.ToString("00") + lDay.ToString("00");

            try
            {
                OrderManagement lOrderManagement = new OrderManagement();
                int lEZWH = GetNextEZWH();
                if (lEZWH > 0)
                {
                    newOrderCode = lOrderPrefix + lEZWH.ToString("00000");
                    return newOrderCode;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        private int GetNextEZWH()
        {
            int lEZSL = -1;

            try
            {
                DataTable lDataTableCustomerOrder = new DataTable();
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand sqlComm = new SqlCommand("SelectNextEZWH", con);
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
                throw new BusinessLogicLayer.MyException("[WarehouseController -> GetNextEZWH]", "Problem in getting EZWH" + Environment.NewLine + ex.Message);
            }
        }

        private string InsertUpdate_WarehouseFranchise(Int64 ID, int WarehouseID, bool IsActive, DataTable dt, string Operation)
        {
            try
            {
                string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(fConnectionString))
                {
                    SqlCommand sqlComm = new SqlCommand("InsertUpdate_WarehouseFranchise", conn);
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    sqlComm.Parameters.AddWithValue("@ID", SqlDbType.BigInt).Value = ID;
                    sqlComm.Parameters.AddWithValue("@WarehouseID", SqlDbType.Int).Value = WarehouseID;
                    sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.Structured).Value = dt;
                    sqlComm.Parameters.AddWithValue("@IsActive", SqlDbType.Bit).Value = IsActive;
                    sqlComm.Parameters.AddWithValue("@Mode", SqlDbType.VarChar).Value = Operation;
                    sqlComm.Parameters.AddWithValue("@QryResult", SqlDbType.Int).Value = 0;

                    conn.Open();
                    sqlComm.ExecuteNonQuery();
                    conn.Close();
                }
                return "Records Insert/Updation Successfully";
            }
            catch (Exception ex)
            {
                //pServerMsg += "\nError : " + (int)IsoftConstant.IS_ERROR_TYPE.EXCEPTION + " : " + ex.Message;
                return "Unable Records Insert/Updation Successfully";
            }
        }

        //
        // GET: /Warehouses/Edit/5

        public ActionResult Edit(long id) /*Yashaswi 4-12-2018 For EVW add new Entity param*/
        {

            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["IsEzeeloLogin"].ToString() == "1")
            {
                ViewBag.CityList = db.Cities.ToList();
                WarehouseViewModel objw = new WarehouseViewModel();
                Warehouse objWarehouses = db.Warehouses.Where(x => x.ID == id).FirstOrDefault();
                BusinessDetail objBusinessDetails = db.BusinessDetails.Where(x => x.ID == objWarehouses.BusinessDetailID).FirstOrDefault();
                string Password = db.UserLogins.Where(x => x.ID == objBusinessDetails.UserLoginID).Select(x => x.Password).FirstOrDefault();
                objw.CityID = objWarehouses.CityID;    ///added by Priti on 13-2-2019            
                objw.StateID = objWarehouses.StateID;     //Added by Priti on 14-2-2019


                objw.ID = objWarehouses.ID;
                objw.Name = objWarehouses.Name;
                objw.GSTNumber = objWarehouses.GSTNumber;
                objw.PAN = objWarehouses.PAN;  //by priti on 11-3-2019
                objw.FSSILicenseNo = objWarehouses.FSSILicenseNo; //by priti on 11-3-2019
                objw.ServiceNumber = objWarehouses.ServiceNumber;
                var vpincodeId = db.Pincodes.Where(x => x.ID == objWarehouses.PincodeID).Select(y => y.Name).FirstOrDefault();
                objw.PincodeID = Convert.ToInt32(vpincodeId);
                objw.NearbyTransport = objWarehouses.NearbyTransport;
                objw.Measurement = objWarehouses.Measurement;
                objw.FloorSpace = objWarehouses.FloorSpace;
                objw.Volume = objWarehouses.Volume;
                objw.Email = objBusinessDetails.Email;
                objw.CustomEntry = objWarehouses.CustomEntry;
                objw.CustomExit = objWarehouses.CustomExit;
                objw.IsFulfillmentCenter = objWarehouses.IsFulfillmentCenter;
                objw.IsActive = objWarehouses.IsActive;
                objw.Password = Password;
                objw.ConfirmPassword = Password;

                objw.BusinessDetails = objBusinessDetails;

                long cityId = db.Pincodes.Where(x => x.ID == objWarehouses.PincodeID).Select(x => x.CityID).First();
                objw.CityID = cityId;    //Added by Priti 

                var DistrictID = db.Cities.Where(x => x.ID == cityId).Select(R => R.DistrictID).FirstOrDefault();
                long StateID = db.Districts.Where(x => x.ID == DistrictID).Select(s => s.StateID).FirstOrDefault();

                objw.StateID = StateID;



                string cityName = db.Cities.Where(x => x.ID == cityId).Select(x => x.Name).First();
                objw.CityName = cityName;
                string StateName = db.States.Where(x => x.ID == StateID).Select(y => y.Name).First();
                objw.StateName = StateName;

                ViewBag.CityID = new SelectList(db.Cities.Where(c => c.IsActive == true).ToList(), "ID", "Name", objw.CityID);     ///Added by Priti on 14-2-2019
                ViewBag.StateID = new SelectList(db.States.Where(c => c.IsActive == true).ToList(), "ID", "Name", objw.StateID);       //Added by Priti on 14-2-2019
                                                                                                                                       //Yashaswi 5/4/2018
                long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
                if (objWarehouses.IsFulfillmentCenter)
                {
                    var lFranchises = db.Franchises.Where(x => x.IsActive == true && x.ContactPerson != ""
                        && !(db.WarehouseFranchises.Join(db.Warehouses, wf => wf.WarehouseID, w => w.ID, (wf, w) =>
                            new
                            {
                                FranchaiseId = wf.FranchiseID,
                                WarehouseID = wf.WarehouseID,
                                IsFulfillmentCenter = w.IsFulfillmentCenter
                            }).Where(p => p.IsFulfillmentCenter == true && p.WarehouseID != EzeeloWarehouseId).ToList()
                            .Select(p => p.FranchaiseId)).Contains(x.ID))
                    .Select(x => new { x.ID, x.ContactPerson, x.IsActive })
                    .Distinct()
                    .ToList();
                    List<FranchiseModel> lFranchiseList = new List<FranchiseModel>();
                    if (lFranchises.Count() > 0)
                    {
                        for (int i = 0; i < lFranchises.Count(); i++)
                        {
                            FranchiseModel lFranchise = new FranchiseModel();
                            lFranchise.ID = Convert.ToInt32(lFranchises[i].ID);
                            lFranchise.Name = CommonFunctions.ConvertStringToCamelCase(Convert.ToString(lFranchises[i].ContactPerson));
                            lFranchiseList.Add(lFranchise);
                        }
                        WarehouseFranchise oo = db.WarehouseFranchises.FirstOrDefault(p => p.WarehouseID == id);
                        if (oo != null)
                        {
                            FranchiseModel lFranchise_ = new FranchiseModel();
                            lFranchise_.ID = oo.FranchiseID;
                            lFranchise_.Name = db.Franchises.FirstOrDefault(p => p.ID == oo.FranchiseID).ContactPerson;
                            lFranchise_.IsSelected = true;
                            lFranchiseList.Add(lFranchise_);
                        }
                    }
                    objw.franchiseList = lFranchiseList;
                }
                else
                {
                    var lFranchises = db.Warehouses.Where(p => p.IsActive == true && p.IsFulfillmentCenter == true
                        && (p.DistributorId == null || p.DistributorId == id)
                        ).Select(p => new { p.ID, ContactPerson = p.Name, DistributorId = p.DistributorId }).ToList();


                    List<FranchiseModel> lFranchiseList = new List<FranchiseModel>();
                    if (lFranchises.Count() > 0)
                    {
                        for (int i = 0; i < lFranchises.Count(); i++)
                        {
                            FranchiseModel lFranchise = new FranchiseModel();
                            lFranchise.ID = Convert.ToInt32(lFranchises[i].ID);
                            lFranchise.Name = CommonFunctions.ConvertStringToCamelCase(Convert.ToString(lFranchises[i].ContactPerson));
                            Warehouse o = db.Warehouses.FirstOrDefault(p => p.ID == lFranchise.ID && p.DistributorId == id);
                            if (o != null)
                            {
                                lFranchise.IsSelected = true;
                            }
                            else
                            {
                                lFranchise.IsSelected = false;
                            }
                            lFranchiseList.Add(lFranchise);
                        }
                        objw.franchiseList = lFranchiseList;
                    }
                    objw.franchiseList = lFranchiseList;
                }
                objw.Entity = objWarehouses.Entity.Trim(); //Yashaswi 4-12-2018 For EVW

                //Start Yashaswi 4-12-2018 For EVW           
                if (objw.Entity == "FV")
                {
                    ViewBag.Heading = "Fulfillment Vendors";
                }
                else if (objw.Entity == "DV")
                {
                    ViewBag.Heading = "Distribution Vendors";
                }
                else if (objw.Entity == "HO")
                {
                    ViewBag.Heading = "Ezeelo HO";
                }
                else
                {
                    ViewBag.Heading = "Ezeelo Warehouses";
                }
                //End Yashaswi 4-12-2018 For EVW    


                return View(objw);
            }
            else
            {
                Session["Warning"] = "Sorry! You are not authorized user.";
                return RedirectToAction("Index", "Home");
            }
        }

        //
        // POST: /Warehouses/Edit/5

        [HttpPost]
        public ActionResult Edit(WarehouseViewModel warehouse)
        {
            var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));
            if (ModelState.IsValid)
            {
                long PersonalDetailID = 1;
                long ID = 1;
                if (Session["USER_LOGIN_ID"] != null)
                {
                    ID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
                    PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == ID).Select(x => x.ID).First());
                }

                long businessDetailID = Convert.ToInt64(db.Warehouses.Where(x => x.ID == warehouse.ID).Select(x => x.BusinessDetailID).First());

                string pincode = warehouse.PincodeID.ToString();
                var vpincodeId = db.Pincodes.Where(x => x.Name == pincode).Select(y => y.ID).FirstOrDefault();
                //ADDED BY Priti 13-2-2019

                ViewBag.CityList = db.Cities.ToList();
                //warehouse.CityList = new SelectList.Convert.ToInt64(db.Cities.Where(x => x.ID == vpincodeId).ToList(), "ID", "Name");
                if (vpincodeId != null && vpincodeId > 0)
                {

                }
                else
                {
                    View();
                }

                //Update Warehouse
                var lwarehouse = new Warehouse()
                {
                    ID = warehouse.ID,
                    Name = warehouse.Name,
                    GSTNumber = warehouse.GSTNumber,
                    FSSILicenseNo = warehouse.FSSILicenseNo,  //by Priti on 11-3-2019
                    PAN = warehouse.PAN,          //by Priti on 11-3-2019
                    ServiceNumber = warehouse.ServiceNumber,
                    PincodeID = vpincodeId,
                    NearbyTransport = warehouse.NearbyTransport,
                    Measurement = warehouse.Measurement,
                    FloorSpace = warehouse.FloorSpace,
                    Volume = warehouse.Volume,
                    CustomEntry = warehouse.CustomEntry,
                    CustomExit = warehouse.CustomExit,
                    IsFulfillmentCenter = warehouse.IsFulfillmentCenter,
                    IsActive = warehouse.IsActive,
                    ModifyBy = PersonalDetailID,
                    ModifyDate = DateTime.UtcNow,
                };

                db.Warehouses.Attach(lwarehouse);
                db.Entry(lwarehouse).Property(x => x.Name).IsModified = true;
                db.Entry(lwarehouse).Property(x => x.GSTNumber).IsModified = true;

                db.Entry(lwarehouse).Property(x => x.FSSILicenseNo).IsModified = true;//by Priti on 11-3-2019
                db.Entry(lwarehouse).Property(x => x.PAN).IsModified = true;//by Priti on 11-3-2019
                db.Entry(lwarehouse).Property(x => x.ServiceNumber).IsModified = true;
                db.Entry(lwarehouse).Property(x => x.NearbyTransport).IsModified = true;
                db.Entry(lwarehouse).Property(x => x.Measurement).IsModified = true;
                db.Entry(lwarehouse).Property(x => x.FloorSpace).IsModified = true;
                db.Entry(lwarehouse).Property(x => x.Volume).IsModified = true;
                db.Entry(lwarehouse).Property(x => x.PincodeID).IsModified = true;
                db.Entry(lwarehouse).Property(x => x.CustomEntry).IsModified = true;
                db.Entry(lwarehouse).Property(x => x.CustomExit).IsModified = true;
                db.Entry(lwarehouse).Property(x => x.IsFulfillmentCenter).IsModified = true;
                db.Entry(lwarehouse).Property(x => x.IsActive).IsModified = true;
                db.Entry(lwarehouse).Property(x => x.ModifyBy).IsModified = true;
                db.Entry(lwarehouse).Property(x => x.ModifyDate).IsModified = true;
                db.SaveChanges();

                long WarehouseID = warehouse.ID;

                var businessDetail = new BusinessDetail()
                {
                    ID = businessDetailID,
                    Name = warehouse.Name,
                    ContactPerson = warehouse.BusinessDetails.ContactPerson,
                    Mobile = warehouse.BusinessDetails.Mobile,
                    Email = warehouse.BusinessDetails.Email,
                    Landline1 = warehouse.BusinessDetails.Landline1,
                    Landline2 = warehouse.BusinessDetails.Landline2,
                    FAX = warehouse.BusinessDetails.FAX,
                    Address = warehouse.BusinessDetails.Address,
                    PincodeID = vpincodeId,
                    Website = warehouse.BusinessDetails.Website,
                    IsActive = warehouse.IsActive,
                    ModifyBy = PersonalDetailID,
                    ModifyDate = DateTime.UtcNow
                };

                db.BusinessDetails.Attach(businessDetail);
                db.Entry(businessDetail).Property(x => x.ContactPerson).IsModified = true;
                db.Entry(businessDetail).Property(x => x.Address).IsModified = true;
                db.Entry(businessDetail).Property(x => x.Name).IsModified = true;
                db.Entry(businessDetail).Property(x => x.Mobile).IsModified = true;
                db.Entry(businessDetail).Property(x => x.Email).IsModified = true;
                db.Entry(businessDetail).Property(x => x.Landline1).IsModified = true;
                db.Entry(businessDetail).Property(x => x.Landline2).IsModified = true;
                db.Entry(businessDetail).Property(x => x.FAX).IsModified = true;
                db.Entry(businessDetail).Property(x => x.PincodeID).IsModified = true;
                db.Entry(businessDetail).Property(x => x.Website).IsModified = true;
                db.Entry(businessDetail).Property(x => x.IsActive).IsModified = true;
                db.Entry(businessDetail).Property(x => x.ModifyBy).IsModified = true;
                db.Entry(businessDetail).Property(x => x.ModifyDate).IsModified = true;
                db.SaveChanges();


                long userLoginID = Convert.ToInt64(db.BusinessDetails.Where(x => x.ID == businessDetailID).Select(x => x.UserLoginID).First());

                long personalDetailID = Convert.ToInt64(db.PersonalDetails.Where(x => x.UserLoginID == userLoginID).Select(x => x.ID).First());

                var personalDetail = new PersonalDetail()
                {
                    ID = personalDetailID,
                    FirstName = warehouse.BusinessDetails.ContactPerson,
                    PincodeID = vpincodeId,
                    Address = warehouse.BusinessDetails.Address,
                    IsActive = warehouse.IsActive,
                    ModifyBy = PersonalDetailID,
                    ModifyDate = DateTime.Now
                };
                db.PersonalDetails.Attach(personalDetail);
                db.Entry(personalDetail).Property(x => x.FirstName).IsModified = true;
                db.Entry(personalDetail).Property(x => x.PincodeID).IsModified = true;
                db.Entry(personalDetail).Property(x => x.Address).IsModified = true;
                db.Entry(personalDetail).Property(x => x.IsActive).IsModified = true;
                db.Entry(personalDetail).Property(x => x.ModifyBy).IsModified = true;
                db.Entry(personalDetail).Property(x => x.ModifyDate).IsModified = true;
                db.SaveChanges();



                if (warehouse.Entity.Trim() == "DV" || warehouse.Entity.Trim() == "FV") //Yashaswi 4-12-2018 For EVW If condition add
                {
                    var temp = (from n in warehouse.franchiseList
                                where n.IsSelected == true
                                select new WarehouseFranchise
                                {
                                    WarehouseID = Convert.ToInt32(WarehouseID),
                                    FranchiseID = n.ID,
                                    IsActive = n.IsSelected,
                                }
                    ).ToList();
                    List<WarehouseFranchise> Delete_WarehouseFranchises = db.WarehouseFranchises.Where(p => p.WarehouseID == WarehouseID).ToList();
                    foreach (WarehouseFranchise obj in Delete_WarehouseFranchises)
                    {
                        db.WarehouseFranchises.Remove(obj);
                        db.SaveChanges();
                    }
                    if (warehouse.IsFulfillmentCenter)
                    {
                        List<WarehouseFranchise> obj_WarehouseFranchise = new List<WarehouseFranchise>();
                        if (lwarehouse.IsFulfillmentCenter)
                        {
                            WarehouseFranchise objWF = new WarehouseFranchise();
                            objWF.WarehouseID = Convert.ToInt32(WarehouseID);
                            objWF.FranchiseID = warehouse.Franchise;
                            objWF.IsActive = true;
                            obj_WarehouseFranchise.Add(objWF);
                        }
                        temp = (from n in obj_WarehouseFranchise
                                select new WarehouseFranchise
                                {
                                    WarehouseID = Convert.ToInt32(WarehouseID),
                                    FranchiseID = n.FranchiseID,
                                    IsActive = true,
                                }).ToList();
                    }
                    else
                    {
                        List<Warehouse> update_warehouse = db.Warehouses.Where(p => p.DistributorId == WarehouseID).ToList();
                        foreach (var item in update_warehouse)
                        {
                            Warehouse up_Warehouse = db.Warehouses.FirstOrDefault(p => p.ID == item.ID);
                            up_Warehouse.DistributorId = null;
                            db.SaveChanges();
                        }

                        List<WarehouseFranchise> list_WarehouseFranchise = new List<WarehouseFranchise>();
                        foreach (WarehouseFranchise pa in temp)
                        {
                            Warehouse obj = db.Warehouses.SingleOrDefault(p => p.ID == pa.FranchiseID);
                            obj.DistributorId = WarehouseID;
                            db.SaveChanges();
                            WarehouseFranchise objWF = new WarehouseFranchise();
                            objWF = db.WarehouseFranchises.FirstOrDefault(p => p.WarehouseID == pa.FranchiseID);
                            if (objWF != null)
                            {
                                list_WarehouseFranchise.Add(objWF);
                            }
                        }
                        temp = (from n in list_WarehouseFranchise
                                select new WarehouseFranchise
                                {
                                    WarehouseID = Convert.ToInt32(WarehouseID),
                                    FranchiseID = n.FranchiseID,
                                    IsActive = true,
                                }).ToList();

                    }


                    if (temp.Count > 0)
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("FranchiseID");
                        dt.Columns.Add("IsActive");
                        dt.Columns.Add("WarehouseID");

                        foreach (WarehouseFranchise pa in temp)
                        {
                            DataRow dr = dt.NewRow();
                            dr["FranchiseID"] = pa.FranchiseID;
                            dr["IsActive"] = pa.IsActive;
                            dr["WarehouseID"] = pa.WarehouseID;

                            dt.Rows.Add(dr);
                        }
                        string msg = InsertUpdate_WarehouseFranchise(0, Convert.ToInt32(WarehouseID), true, dt, "UPDATE");
                    }

                }



                Session["Success"] = "Record Updated Successfully.";//yashaswi 30/3/2018
                /*Yashaswi 4-12-2018 For EVW*/
                //if (warehouse.Entity == "HO")
                //{
                //    warehouse.Entity = "EVW";
                //}
                return RedirectToAction("Index", new { flag = warehouse.IsFulfillmentCenter, Entity = warehouse.Entity });//Yashaswi 4-12-2018 For EVW
            }

            return View(warehouse);
        }

        //
        // GET: /Warehouses/Delete/5

        public ActionResult Delete(long id)
        {
            Warehouse warehouse = db.Warehouses.Single(x => x.ID == id);
            return View(warehouse);
        }

        //
        // POST: /Warehouses/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            Warehouse warehouse = db.Warehouses.Single(x => x.ID == id);
            db.Warehouses.Remove(warehouse);
            db.SaveChanges();
            Session["Success"] = "Record Deleted Successfully."; //yashaswi 30/3/2018
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

        public JsonResult IsMobileAvailable(string Mobile)
        {
            if (db.UserLogins.Any(x => x.Mobile == Mobile))
            {
                return Json("1", JsonRequestBehavior.AllowGet);
            }

            return Json("0", JsonRequestBehavior.AllowGet);
        }


        //yashaswi2/4/2018
        public ActionResult WarehouseList()
        {
            //Check Session active or not
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["IsEzeeloLogin"].ToString() == "1")
            {
                List<WarehouseViewModel> lWarehouseViewModel = new List<WarehouseViewModel>();

                lWarehouseViewModel = (from w in db.Warehouses
                                       join p in db.Pincodes on w.PincodeID equals p.ID
                                       join c in db.Cities on p.CityID equals c.ID
                                       where w.IsFulfillmentCenter == false
                                       select new WarehouseViewModel
                                       {
                                           ID = w.ID,
                                           Name = w.Name,
                                           WarehouseCode = w.WarehouseCode,
                                           GSTNumber = w.GSTNumber,
                                           ServiceNumber = w.ServiceNumber,
                                           CityName = c.Name,
                                           IsActive = w.IsActive,
                                           FC_Count = db.Warehouses.Count(m => m.DistributorId == w.ID),
                                           Margin = w.Margin == null ? 0 : w.Margin
                                       }).OrderByDescending(w => w.ID).ToList();

                return View(lWarehouseViewModel);
            }
            else
            {
                Session["Warning"] = "Sorry! You are not authorized user.";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult SetMarginForDVFV(long id)
        {
            //Check Session active or not
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["IsEzeeloLogin"].ToString() == "1")
            {
                Warehouse obj_Warehouse = db.Warehouses.FirstOrDefault(p => p.ID == id);
                List<Warehouse> list_warehouse = db.Warehouses.Where(p => p.DistributorId == id).ToList();

                WarehouseViewModel obj_DC = new WarehouseViewModel();
                List<Warehouse> llist_FC = new List<Warehouse>();
                obj_DC.ID = obj_Warehouse.ID;
                obj_DC.WarehouseCode = obj_Warehouse.WarehouseCode;
                obj_DC.Name = obj_Warehouse.Name;
                obj_DC.Margin = obj_Warehouse.Margin == null ? 0 : obj_Warehouse.Margin;
                foreach (Warehouse item in list_warehouse)
                {
                    Warehouse obj_FC = new Warehouse();
                    obj_FC.ID = item.ID;
                    obj_FC.WarehouseCode = item.WarehouseCode;
                    obj_FC.Name = item.Name;
                    obj_FC.Margin = item.Margin == null ? 0 : item.Margin;
                    long Fran_id = db.WarehouseFranchises.FirstOrDefault(p => p.WarehouseID == item.ID).FranchiseID;
                    obj_FC.NetworkIP = db.Franchises.FirstOrDefault(a => a.ID == Fran_id).ContactPerson; //Use to show framchise name
                    llist_FC.Add(obj_FC);
                }
                obj_DC.FCList = llist_FC;
                return View(obj_DC);
            }
            else
            {
                Session["Warning"] = "Sorry! You are not authorized user.";
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult SetMarginForDVFV(WarehouseViewModel obj_DC)
        {
            Warehouse DCobj = db.Warehouses.FirstOrDefault(P => P.ID == obj_DC.ID);
            DCobj.Margin = obj_DC.Margin;
            foreach (var item in obj_DC.FCList)
            {
                Warehouse FCobj = db.Warehouses.FirstOrDefault(P => P.ID == item.ID);
                FCobj.Margin = item.Margin;
                db.SaveChanges();
            }
            Session["Success"] = "Margin Saved Successfully.";//yashaswi 30/3/2018
            return RedirectToAction("WarehouseList");
        }

        //start Yashaswi 18-6-2018
        [HttpGet]
        public ActionResult MarginDivisionList()
        {
            //Check Session active or not
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["IsEzeeloLogin"].ToString() == "1")
            {
                return View(db.MarginDivision.Where(p => p.IsActive == true).ToList());
            }
            else
            {
                Session["Warning"] = "Sorry! You are not authorized user.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult SaveMarginDivision()
        {
            //Check Session active or not
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["IsEzeeloLogin"].ToString() == "1")
            {
                return View(db.MarginDivision.Where(p => p.IsActive == true).ToList());
            }
            else
            {
                Session["Warning"] = "Sorry! You are not authorized user.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult SaveMarginDivision(List<MarginDivision> lMarDiv)
        {
            int TotalPer = lMarDiv.Sum(p => p.MarginInPercentage);
            if (TotalPer == 100)
            {
                long PersonalDetailID = 1;
                if (Session["USER_LOGIN_ID"] != null)
                {
                    long ID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
                    PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == ID).Select(x => x.ID).First());
                }
                foreach (var item in lMarDiv)
                {
                    MarginDivision obj = db.MarginDivision.SingleOrDefault(p => p.ID == item.ID);
                    obj.MarginInPercentage = item.MarginInPercentage;
                    obj.MarginFor = item.MarginFor;
                    obj.ModifyBy = PersonalDetailID;
                    obj.ModifyDate = DateTime.Now;
                    obj.DeviceID = "X";
                    obj.DeviceType = "X";
                    obj.NetworkIP = CommonFunctions.GetClientIP();
                    db.SaveChanges();
                }
                Session["Success"] = "Margin% Saved Successfully.";
                return RedirectToAction("MarginDivisionList");
            }
            else
            {
                ViewBag.ErrorMsg = "1";
            }
            return View(db.MarginDivision.Where(p => p.IsActive == true).ToList());
        }
        //End

        //Yashaswi 14-03-2019 To show EVW list to share DV
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
        public ActionResult AddOrViewDV(long Id)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
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
                            ListForEVW = db.Warehouses.Where(s => s.IsFulfillmentCenter == false && s.Entity == "DV" && !(db.EVWsDVs.Where(e => e.IsActive == true && e.WarehouseId_EVW != Id).Select(e => new { WarehouseId = e.WarehouseId }).Select(e => e.WarehouseId).Contains(s.ID)))
                                                .Select(s => new EVWV_List
                                                {
                                                    ID = s.ID,
                                                    Name = s.Name,
                                                    IsSelected = db.EVWsDVs.Any(e => e.IsActive == true && e.WarehouseId_EVW == Id && e.WarehouseId == s.ID),
                                                }).OrderBy(s => s.IsSelected).OrderBy(s => s.Name).ToList()
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
        public ActionResult AddOrViewDV(EVWViewModel obj)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                db.Configuration.ProxyCreationEnabled = false;
                try
                {
                    long PersonalDetailID = GetPersonalDetailID();
                    var list = db.EVWsDVs.Where(p => p.IsActive == true && p.WarehouseId_EVW == obj.ID).Select(p => new
                    {
                        WarehouseId_EVW = p.WarehouseId_EVW,
                        WarehouseId = p.WarehouseId
                    }).AsEnumerable()
                    .Where(p => !(obj.ListForEVW.Where(q => q.IsSelected).Select(q => q.ID).ToList()).Contains(p.WarehouseId)).ToList();

                    foreach (var item in list)
                    {
                        EVWsDV o = db.EVWsDVs.FirstOrDefault(p => p.WarehouseId_EVW == item.WarehouseId_EVW && p.WarehouseId == item.WarehouseId);
                        o.IsActive = false;
                        o.ModifyBy = PersonalDetailID;
                        o.ModifyDate = DateTime.Now;
                        o.NetworkIP = CommonFunctions.GetClientIP();
                        db.EVWsDVs.Attach(o);
                        db.Entry(o).Property(x => x.IsActive).IsModified = true;
                        db.Entry(o).Property(x => x.ModifyBy).IsModified = true;
                        db.Entry(o).Property(x => x.ModifyDate).IsModified = true;
                        db.Entry(o).Property(x => x.NetworkIP).IsModified = true;
                        db.SaveChanges();
                    }
                    if (obj.ListForEVW.Any(s => s.IsSelected))
                    {
                        foreach (var item in obj.ListForEVW.Where(s => s.IsSelected).ToList())
                        {
                            EVWsDV o = db.EVWsDVs.FirstOrDefault(p => p.WarehouseId_EVW == obj.ID && p.WarehouseId == item.ID);
                            if (o == null)
                            {
                                EVWsDV obj_EVWsDV = new EVWsDV();
                                obj_EVWsDV.IsActive = true;
                                obj_EVWsDV.NetworkIP = CommonFunctions.GetClientIP();
                                obj_EVWsDV.WarehouseId = item.ID;
                                obj_EVWsDV.WarehouseId_EVW = obj.ID;
                                obj_EVWsDV.CreateBy = PersonalDetailID;
                                obj_EVWsDV.CreateDate = DateTime.Now;
                                db.EVWsDVs.Add(obj_EVWsDV);
                                db.SaveChanges();
                            }
                            else
                            {
                                if (o.IsActive == false)
                                {
                                    o.IsActive = true;
                                    o.NetworkIP = CommonFunctions.GetClientIP();
                                    o.WarehouseId = item.ID;
                                    o.WarehouseId_EVW = obj.ID;
                                    o.ModifyBy = PersonalDetailID;
                                    o.ModifyDate = DateTime.Now;
                                    db.SaveChanges();
                                }
                            }

                        }
                    }
                    transactionScope.Complete();
                    transactionScope.Dispose();
                    Session["Success"] = "Data Save Successfully.";
                    return RedirectToAction("EVWList", "Warehouses");
                }
                catch (Exception ex)
                {
                    transactionScope.Dispose();
                    Session["Error"] = ex.Message;
                    return RedirectToAction("EVWList", "Warehouses");
                }
            }
        }

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
    }
}