//-----------------------------------------------------------------------
// <copyright file="ProductList.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Pradnyakar N. Badge</author>
//-----------------------------------------------------------------------
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using System.Transactions;
using System.Data.Entity;
using System.Text;
using BusinessLogicLayer;
using Administrator.Models;
using System.Net;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;

namespace  Administrator.Controllers
{
    
    public class RoleManagementController : Controller
    {

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
           Environment.NewLine
           + "ErrorLog Controller : RoleManagementController" + Environment.NewLine);

        private EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /RoleManagement/

        /// <summary>
        /// Return All the detail of Roles Who have assign Menu Access
        /// </summary>
        /// <returns>List of Role</returns>
        [SessionExpire]
        [CustomAuthorize(Roles = "RoleManagement/CanRead")]
        public ActionResult Index()
        {
            try
            {


                //Inner Join with Role and Role Menu to Find All Role having assign Menu
                var lDate = (from pd in db.Roles
                             join or in db.RoleMenus on pd.ID equals or.RoleID
                             select new
                             {
                                 pd.Name,
                                 pd.ID,
                                 pd.ParentID,
                                 pd.Level,
                                 pd.IsActive
                             }).Distinct().ToList();

                //Convert var data into List of Role Type
                List<Role> ls = new List<Role>();
                foreach (var dd in lDate)
                {
                    Role pp = new Role();
                    pp.Name = dd.Name;
                    pp.ID = dd.ID;
                    pp.ParentID = dd.ParentID;
                    pp.Level = dd.Level;
                    pp.IsActive = dd.IsActive;

                    ls.Add(pp);
                }

                //Pass Argument to View
                return View(ls);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Index[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Index view!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }

        }

        /// <summary>
        /// Get Request to Create Command
        /// </summary>
        /// <returns>retun Create Role View </returns>        
        [SessionExpire]
        [CustomAuthorize(Roles = "RoleManagement/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                TempData["ReturnAction"] = "Create";

                List<Role> lroleData = db.Roles.ToList();
                //View Bag to Fill Role DropDownList
                ViewBag.RoleList = new SelectList(lroleData, "ID", "Name");
                ViewBag.MasterMenu = new SelectList(db.Menus.Where(x=>x.ParentID == null).ToList(), "ID", "Name");


                RoleManagerViewModel ls = new RoleManagerViewModel();

                List<CustomeRoleModel> dbCustomerModel = new List<CustomeRoleModel>();
                //Get All List of Menu
                List<Menu> lData = db.Menus.ToList();
               

                dbCustomerModel = (from dd in lData
                                  
                                   select new CustomeRoleModel
                                  {
                                      MenuID = dd.ID,
                                      MenuName = dd.DisplayName,
                                      MenuParentID = dd.ParentID == null ? 0 : dd.ParentID 
                                  }).ToList();


                //Assign Collectuion of CustomeModule Data
                ls.rolemenuCollection = dbCustomerModel;

                return View(ls);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Create[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Create View " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }

        }

        /// <summary>
        /// Post Request to Insert Data in RoleMenu Table
        /// </summary>
        /// <param name="ls">Role and List of and Menu with their Access Permission(Read,Write,Print,Import,Export etc)</param>
        /// <param name="RoleList">Role ID</param>
        /// <returns>Retun View</returns>
        //[HttpPost]        
        //[SessionExpire]
        //[CustomAuthorize(Roles = "RoleManagement/CanRead")]
        //public ActionResult Create(RoleManagerViewModel ls, int RoleList)
        //{
        //    try
        //    {

        //        Role roleMenu = db.Roles.Find(RoleList);
        //        if (roleMenu == null)
        //        {
        //            return HttpNotFound();
        //        }
              
        //        //Check whether role exist int rolemenu or no
        //        if (db.RoleMenus.Count(x => x.RoleID == RoleList) > 0)
        //        {
        //            ViewBag.pMessage = "Role Already Exists!!";
        //            List<Role> lroleData = db.Roles.ToList();
        //            ViewBag.RoleList = new SelectList(lroleData, "ID", "Name");
        //            return View(ls);
        //        }
        //        else
        //        {
        //            List<Role> lroleData = db.Roles.ToList();
        //            ViewBag.RoleList = new SelectList(lroleData, "ID", "Name", RoleList);
        //            InserupdateRoleMenu(ls, RoleList);

                   

        //            return View(ls);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        errStr.Append("Method Name[Http Request] :- Create[HttpPost]" + Environment.NewLine +
        //                       "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
        //                           ex.Message.ToString() + Environment.NewLine +
        //                 "====================================================================================="
        //                       );
        //        //ViewBag.Message = "Sorry! Problem in customer registration!!";
        //        ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
        //        ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine + errStr.ToString()
        //            , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

        //        return View();
        //    }

        //}

        /// <summary>
        /// Method to check whether an role already access Menu or not
        /// use to call by ajax method
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns></returns>
        public JsonResult RoleExistOrNot(int id)
        {
            //json result to ajax call
            return Json(db.RoleMenus.Count(x => x.RoleID == id), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// view for Update to Existing roleMenu
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>List of Menu assigne to Role</returns>        
        [SessionExpire]
        [CustomAuthorize(Roles = "RoleManagement/CanWrite")]        
        public ActionResult Edit(int? id)
        {

            try
            {
                TempData["ReturnAction"] = "Edit/" + id.ToString();
                ViewBag.MasterMenu = new SelectList(db.Menus.Where(x => x.ParentID == null).ToList(), "ID", "Name");

                RoleMenu roleMenu = db.RoleMenus.Where(x => x.RoleID == id).FirstOrDefault();
                if (roleMenu == null)
                {
                    return HttpNotFound();
                }
                //View bag to fill role dropdown again
                List<Role> lroleData = db.Roles.Where(x => x.ID == id).ToList();
                ViewBag.RoleList = new SelectList(lroleData, "ID", "Name", id);

                RoleManagerViewModel ls = new RoleManagerViewModel();

               

                return View(ls);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Edit[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Update!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Update!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }

        }

        /// <summary>
        /// Post Request for eding the existing Record
        /// </summary>
        /// <param name="ls">List of Menu and right to Role</param>
        /// <param name="id">RoleID</param>
        /// <returns></returns>
        //[HttpPost]
        //[SessionExpire]
        //[CustomAuthorize(Roles = "RoleManagement/CanWrite")]
        //public ActionResult Edit(RoleManagerViewModel ls, int id)
        //{
        //    try
        //    {
        //        ViewBag.RoleList = id;
        //        ViewBag.MasterMenu = new SelectList(db.Menus, "ID", "Name");
        //        RoleMenu roleMenu = db.RoleMenus.Where(x => x.RoleID == id).FirstOrDefault();
        //        if (roleMenu == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        //check wether role is exist in role menu or not
        //        if (db.RoleMenus.Count(x => x.RoleID == id) < 0)
        //        {
        //            //Message if role doesn't present in RoleMenu Table
        //            ViewBag.pMessage = "Role Is Not Exists!!";
        //            //View bag to fill role dropdown again
        //            List<Role> lroleData = db.Roles.ToList();
        //            ViewBag.RoleList = new SelectList(lroleData, "ID", "Name", id);

        //            return View(ls);
        //        }
        //        else
        //        {
        //            //View bag to fill role dropdown again
        //            List<Role> lroleData = db.Roles.ToList();
        //            ViewBag.RoleList = new SelectList(lroleData, "ID", "Name", id);
        //            InserupdateRoleMenu(ls,id);
                   
        //            return View(ls);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        errStr.Append("Method Name[Http Request] :- Edit[HttpPost]" + Environment.NewLine +
        //                       "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
        //                           ex.Message.ToString() + Environment.NewLine +
        //                 "====================================================================================="
        //                       );
        //        //ViewBag.Message = "Sorry! Problem in customer registration!!";
        //        ModelState.AddModelError("Message", "Sorry! Problem in Record Updation!!");
        //        ErrorLog.ErrorLogFile("Sorry! Problem in Record Updation!!" + Environment.NewLine + errStr.ToString()
        //            , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

        //        return View();
        //    }


        //}

        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
        private void InserupdateRoleMenu(RoleManagerViewModel ls, int id)
        {

            DataTable lDataTable = new DataTable();
            lDataTable.Columns.Add("MenuID");
            lDataTable.Columns.Add("MenuName");

            lDataTable.Columns.Add("CanRead");
            lDataTable.Columns.Add("CanWrite");
            lDataTable.Columns.Add("CanDelete");
            lDataTable.Columns.Add("CanPrint");
            lDataTable.Columns.Add("CanImport");
            lDataTable.Columns.Add("CanExport");
            lDataTable.Columns.Add("IsActive");

            for (int i = 0; i < ls.rolemenuCollection.Count(); i++)
            {   
                DataRow dr = lDataTable.NewRow();
                dr[0] = ls.rolemenuCollection[i].MenuID;
                dr[1] = ls.rolemenuCollection[i].MenuName;
                dr[2] = ls.rolemenuCollection[i].CanRead;
                dr[3] = ls.rolemenuCollection[i].CanWrite;
                dr[4] = ls.rolemenuCollection[i].CanDelete;
                dr[5] = ls.rolemenuCollection[i].CanPrint;
                dr[6] = ls.rolemenuCollection[i].CanImport;
                dr[7] = ls.rolemenuCollection[i].CanExport;
                dr[8] = ls.rolemenuCollection[i].IsActive;
                lDataTable.Rows.Add(dr);
            }

            using (SqlConnection conn = new SqlConnection(fConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand("InsertUpdateRoleMenu", conn);
                sqlComm.CommandType = CommandType.StoredProcedure;
                sqlComm.Parameters.AddWithValue("@RoleID", SqlDbType.BigInt).Value = id;
                sqlComm.Parameters.AddWithValue("@RoleMenu", SqlDbType.Structured).Value = lDataTable;
                sqlComm.Parameters.AddWithValue("@CreateBy", SqlDbType.BigInt).Value = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                sqlComm.Parameters.AddWithValue("@CreateDate", SqlDbType.DateTime2).Value = DateTime.UtcNow.AddHours(5.5);
                sqlComm.Parameters.AddWithValue("@ModifiedBy", SqlDbType.BigInt).Value = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                sqlComm.Parameters.AddWithValue("@ModifiedDate", SqlDbType.DateTime2).Value = DateTime.UtcNow.AddHours(5.5);

                conn.Open();
                sqlComm.ExecuteNonQuery();

                conn.Close();
                ViewBag.Message = "Role Menu Save Successfully!!";
            }
        }

        //[SessionExpire]
        //[CustomAuthorize(Roles = "RoleManagement/CanWrite")]
        //public ActionResult AddMore1(Int32 id)
        //{
        //    List<Role> lroleData = db.Roles.Where(x => x.ID == id).ToList();
        //    ViewBag.RoleList = new SelectList(lroleData, "ID", "Name", id);

        //    RoleManagerViewModel ls = new RoleManagerViewModel();

        //    List<CustomeRoleModel> dbCustomerModel = new List<CustomeRoleModel>();
        //    //Get All List of Menu
        //    List<Menu> lData = db.Menus.ToList();
        //    //List<RoleMenu> lRoleMenu = db.RoleMenus.Where()
        //    dbCustomerModel = (from dd in lData
        //                       where
        //                       !db.RoleMenus.Any(x => x.RoleID == id && dd.ParentID != null && x.MenuID==dd.ID)            
        //                       select new CustomeRoleModel
        //                       {
        //                           MenuID = dd.ID,
        //                           MenuName = dd.DisplayName,
        //                           MenuParentID = dd.ParentID == null ? 0 : dd.ParentID
        //                       }).ToList();


        //    //Assign Collectuion of CustomeModule Data
        //    ls.rolemenuCollection = dbCustomerModel;

        //    return View(ls);
        //}

        //[HttpPost]
        //public ActionResult AddMore1(RoleManagerViewModel ls, int RoleList)
        //{
        //    try
        //    {

        //        Role roleMenu = db.Roles.Find(RoleList);
        //        if (roleMenu == null)
        //        {
        //            return HttpNotFound();
        //        }

               
        //        else
        //        {
                   
        //            InserupdateRoleMenu(ls, RoleList);
        //            ViewBag.Message = "Sorry! Problem in Inserting Menu for Role!!";
        //            return View(ls);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        errStr.Append("Method Name[Http Request] :- AddMore[HttpPost]" + Environment.NewLine +
        //                       "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
        //                           ex.Message.ToString() + Environment.NewLine +
        //                 "====================================================================================="
        //                       );
        //        //ViewBag.Message = "Sorry! Problem in customer registration!!";
        //        ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
        //        ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine + errStr.ToString()
        //            , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

        //        return View();
        //    }
        //}

        /// <summary>
        /// To Get List of Menu assing to Role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>List of Menu Assign to Role</returns>
        private RoleManagerViewModel getMenuDetail(int MasterMenu,int id)
        {
            try
            {

                RoleManagerViewModel ls = new RoleManagerViewModel();

                List<CustomeRoleModel> dbCustomerModel = new List<CustomeRoleModel>();

                //Join with Role, Menu, RoleMenu Table
                var q = (from pd in db.RoleMenus
                         join od in db.Menus on pd.MenuID equals od.ID 
                         join or in db.Roles on pd.RoleID equals or.ID
                         where od.ParentID == MasterMenu && or.ID == id
                         select new
                         {
                             RoleMenuID = pd.ID == null ? 0 : pd.ID,
                             MenuID = pd.MenuID == null ? 0 : pd.MenuID,
                             RoleID = pd.RoleID == null ? 0 : pd.RoleID,
                             ParentID = od.ParentID == null ? 0 : od.ParentID,
                             CanDelete = pd.CanDelete == null ? false : pd.CanDelete,
                             CanExport = pd.CanExport == null ? false : pd.CanExport,
                             CanImport = pd.CanImport == null ? false : pd.CanImport,
                             CanPrint = pd.CanPrint == null ? false : pd.CanPrint,
                             CanRead = pd.CanRead == null ? false : pd.CanRead,
                             CanWrite = pd.CanWrite == null ? false : pd.CanWrite,
                             IsActive = pd.IsActive == null ? false : pd.IsActive,
                             od.DisplayName,
                             or.Name
                         }).Where(x => x.RoleID == id).OrderBy(x => x.ParentID).ToList();



                dbCustomerModel = (from dd in q
                                   select new CustomeRoleModel
                                       {

                                           RoleMenuID = dd.RoleMenuID,
                                           MenuID = Convert.ToInt16(dd.MenuID),
                                           MenuName = dd.DisplayName,
                                           MenuParentID = Convert.ToInt16(dd.ParentID),
                                           CanDelete = Convert.ToBoolean(dd.CanDelete),
                                           CanExport = Convert.ToBoolean(dd.CanExport),
                                           CanImport = Convert.ToBoolean(dd.CanImport),
                                           CanPrint = Convert.ToBoolean(dd.CanPrint),
                                           CanRead = Convert.ToBoolean(dd.CanRead),
                                           CanWrite = Convert.ToBoolean(dd.CanWrite),
                                           RoleID = Convert.ToInt16(dd.RoleID),
                                           IsActive = Convert.ToBoolean(dd.IsActive),
                                       }).ToList();

                //Assinge Entire List of dbCustomerModel Modle to the RoleManagerViewModel 
                ls.rolemenuCollection = dbCustomerModel;

                return ls;
            }
            catch
            {
                throw new Exception("Unable to Generate dbCustomerModel Modle to the RoleManagerViewModel");
            }

        }

        private RoleManagerViewModel getAddMoreMenuDetail(int id)
        {
            try
            {

                RoleManagerViewModel ls = new RoleManagerViewModel();

                List<CustomeRoleModel> dbCustomerModel = new List<CustomeRoleModel>();

                //Join with Role, Menu, RoleMenu Table
                dbCustomerModel = (from pd in db.RoleMenus
                                   join od in db.Menus on pd.MenuID equals od.ID
                                   join or in db.Roles on pd.RoleID equals or.ID
                                   where !db.RoleMenus.Any(x => x.MenuID == pd.ID)
                                   && pd.RoleID == id && od.ParentID == null
                                   select new CustomeRoleModel
                         {
                             RoleMenuID = pd.ID == null ? 0 : pd.ID,
                             MenuID = pd.MenuID == null ? 0 : (Int32)pd.MenuID,
                             RoleID = pd.RoleID == null ? 0 : pd.RoleID,
                             MenuParentID = od.ParentID == null ? 0 : od.ParentID,
                             CanDelete = pd.CanDelete == null ? false : (bool)pd.CanDelete,
                             CanExport = pd.CanExport == null ? false : (bool)pd.CanExport,
                             CanImport = pd.CanImport == null ? false : (bool)pd.CanImport,
                             CanPrint = pd.CanPrint == null ? false : (bool)pd.CanPrint,
                             CanRead = pd.CanRead == null ? false : (bool)pd.CanRead,
                             CanWrite = pd.CanWrite == null ? false : (bool)pd.CanRead,
                             IsActive = pd.IsActive == null ? false : pd.IsActive,

                         }).ToList();





                //Assinge Entire List of dbCustomerModel Modle to the RoleManagerViewModel 
                ls.rolemenuCollection = dbCustomerModel;

                return ls;
            }
            catch
            {
                throw new Exception("Unable to Generate dbCustomerModel Modle to the RoleManagerViewModel");
            }

        }

        [SessionExpire]
        [CustomAuthorize(Roles = "RoleManagement/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                RoleMenu roleMenu = db.RoleMenus.Where(x => x.RoleID == id).FirstOrDefault();
                if (roleMenu == null)
                {
                    return HttpNotFound();
                }

                
                List<Role> lroleData = db.Roles.Where(x => x.ID == id).ToList();
                ViewBag.RoleList = new SelectList(lroleData, "ID", "Name", id);


                RoleManagerViewModel ls = new RoleManagerViewModel();

               

                return View(ls);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Details[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Detail!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Detail!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        public JsonResult MenuRoleList(int id)
        {
            RoleManagerViewModel ls = new RoleManagerViewModel();
            return Json(ls, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RoleMenu(int id)
        {
            TempData["ReturnAction"] = "RoleMenu/" + id.ToString();
            ViewBag.RoleList = new SelectList(db.Roles, "ID", "Name", id);
            ViewBag.MasterMenu = new SelectList(db.Menus.Where(x => x.ParentID == null).ToList(), "ID", "Name");
            return View();
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "RoleManagement/CanWrite")]
        public ActionResult PartialRoleMenu(int MasterMenu, int RoleList)
        {
            List<CustomeRoleModel> dbCustomerModel = new List<CustomeRoleModel>();
            //Get All List of Menu
            List<Menu> lData = db.Menus.ToList();
            BusinessLogicLayer.RoleMenuList obj = new RoleMenuList();
            DataTable dt = new DataTable();
            dt = obj.SelectRoleMenu(MasterMenu, RoleList, System.Web.HttpContext.Current.Server);
            dbCustomerModel = (from n in dt.AsEnumerable()
                               select new CustomeRoleModel
                               {
                                   MenuID = n.Field<Int32>("MenuID"),
                                   MenuName = n.Field<string>("MenuName"),
                                   MenuParentID = n.Field<Int32?>("MenuParentID")
                               }).ToList();

            RoleManagerViewModel ls = new RoleManagerViewModel();

            //Assign Collectuion of CustomeModule Data
            ls.rolemenuCollection = dbCustomerModel;
            ViewBag.RoleList = RoleList;
            return PartialView("_RoleMenu", ls);

        }


        [SessionExpire]
        [CustomAuthorize(Roles = "RoleManagement/CanWrite")]
        public ActionResult EditPartialRoleMenu(int MasterMenu, int RoleList)
        {
            
            ViewBag.RoleList = RoleList;
            RoleManagerViewModel ls = new RoleManagerViewModel();
            ls = getMenuDetail(MasterMenu, Convert.ToInt16(RoleList));
            //return PartialView("_RoleMenu", ls);
            //string ActionUrl = TempData["ReturnAction"].ToString();
            //TempData.Keep();
            return PartialView("_RoleMenu", ls);

        }

        [SessionExpire]
        [CustomAuthorize(Roles = "RoleManagement/CanWrite")]
        public ActionResult AddMore(RoleManagerViewModel ls, int RoleList)
        {
            try
            {

                ViewBag.MasterMenu = new SelectList(db.Menus, "ID", "Name");
                ViewBag.RoleList = new SelectList(db.Roles.Where(x => x.ID == RoleList).ToList(), "ID", "Name", RoleList);
                Role roleMenu = db.Roles.Find(RoleList);
                if (roleMenu == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    InserupdateRoleMenu(ls, RoleList);
                    ViewBag.Message = "Menu for Role Inserted Successfully!!";
                    string ActionUrl = TempData["ReturnAction"].ToString();
                    return RedirectToAction(ActionUrl);
                }
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- AddMore[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }
    
    }
}
  
