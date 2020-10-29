
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
using PagedList;
using PagedList.Mvc;
using System.Text;
using BusinessLogicLayer;
using Administrator.Models;
namespace Administrator.Controllers
{
    /// <summary>
    /// Developed By :- Pradnyakar Badge
    /// To Create Area within Pincode Record
    /// </summary>
    
    public class AreaController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
         Environment.NewLine
         + "ErrorLog Controller : AreaController" + Environment.NewLine);

        // GET: /Area/
        [SessionExpire]
        [CustomAuthorize(Roles = "Area/CanRead")]
        public ActionResult Index(int? page, int? district, int? state, Int64? city)
        {
            try
            {


                if (state > 0)
                {
                    ViewBag.district = new SelectList(db.Districts.Where(x => x.StateID == state).ToList(), "ID", "Name");
                    ViewBag.State = new SelectList(db.States.ToList(), "ID", "Name", state);
                }
                else
                {
                    List<SelectListItem> lData = new List<SelectListItem>();
                    lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                    ViewBag.district = new SelectList(lData, "Value", "Text");
                    ViewBag.State = new SelectList(db.States.ToList(), "ID", "Name");

                }

                List<Area> AreaList = new List<Area>();

                if (district > 0 && city > 0)
                {
                    ViewBag.city = new SelectList(db.Cities.Where(x => x.DistrictID == district).ToList(), "ID", "Name", city);
                    AreaList = db.Areas.Where(x => x.Pincode.CityID == city).ToList();

                }
                else
                {
                    List<SelectListItem> lData = new List<SelectListItem>();
                    lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                    ViewBag.city = new SelectList(lData, "Value", "Text");

                    AreaList = db.Areas.Where(x => x.Pincode.City.District.State.ID == 1).Include(p => p.Pincode).ToList();
                }



                return View(AreaList.ToList());

                //var areas = db.Areas.OrderBy(x => x.Name).Include(c => c.Pincode);
                //return View(areas.ToList().ToPagedList(page ?? 1, 10));
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

            //var areas = db.Areas.Include(a => a.Pincode);
            //return View(areas.ToList());
        }

        // GET: /Area/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "Area/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Area area = db.Areas.Find(id);
                if (area == null)
                {
                    return HttpNotFound();
                }
                return View(area);
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

        [SessionExpire]
        [CustomAuthorize(Roles = "Area/CanWrite")]
        // GET: /Area/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.State = new SelectList(db.States, "ID", "Name");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.District = new SelectList(lData, "Value", "Text");

                ViewBag.City = new SelectList(lData, "Value", "Text");

                ViewBag.PincodeID = new SelectList(lData, "Value", "Text");


                //ViewBag.PincodeID = selListItemPincode;
                return View();
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

        // POST: /Area/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [CustomAuthorize(Roles = "Area/CanWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Name,PincodeID,IsActive")] Area area)
        {
            try
            {
                area.CreateDate = DateTime.UtcNow.AddHours(5.30);
                area.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                area.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();

                if (ModelState.IsValid)
                {
                    db.Areas.Add(area);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    //return View();
                    ViewBag.Messaage = "Area Created Successfully";
                }

                ViewBag.State = new SelectList(db.States, "ID", "Name");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.District = new SelectList(lData, "Value", "Text");

                ViewBag.City = new SelectList(lData, "Value", "Text");

                ViewBag.PincodeID = new SelectList(lData, "Value", "Text");

               
                
                    //ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", area.PincodeID);
                return View(area);
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

                ViewBag.Messaage = "Unable to insert Area detail";
                return View(area);
            }
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "Area/CanWrite")]
        // GET: /Area/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Area area = db.Areas.Find(id);
                if (area == null)
                {
                    return HttpNotFound();
                }
                ViewBag.State = new SelectList(db.States, "ID", "Name");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.District = new SelectList(lData, "Value", "Text");

                ViewBag.City = new SelectList(lData, "Value", "Text");

                ViewBag.PincodeID = new SelectList(lData, "Value", "Text", area.PincodeID);

                //ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", area.PincodeID);
                return View(area);
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

        // POST: /Area/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [CustomAuthorize(Roles = "Area/CanWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Name,PincodeID,IsActive")] Area area)
        {
            try
            {

                Area lArea = db.Areas.Find(area.ID);

                area.CreateDate = lArea.CreateDate;
                area.CreateBy = lArea.CreateBy;
                area.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                area.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                area.NetworkIP = CommonFunctions.GetClientIP();
                

                if (ModelState.IsValid)
                {
                    //db.Entry(area).State = EntityState.Modified;
                    db.Entry(lArea).CurrentValues.SetValues(area);
                    db.SaveChanges();
                   // return RedirectToAction("Index");
                    ViewBag.Messaage = "Area detail modified Successfully";
                }

                ViewBag.State = new SelectList(db.States.OrderBy(x => x.Name), "ID", "Name");

                List<SelectListItem> lData = new List<SelectListItem>();
                lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                ViewBag.District = new SelectList(lData, "Value", "Text");

                ViewBag.City = new SelectList(lData, "Value", "Text");

                ViewBag.PincodeID = new SelectList(lData, "Value", "Text");

              

                //ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", area.PincodeID);
                return View(area);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Edit[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Updation!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Updation!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to modified Area detail";
                return View(area);
            }
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "Area/CanDelete")]
        // GET: /Area/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Area area = db.Areas.Find(id);
                if (area == null)
                {
                    return HttpNotFound();
                }
                return View(area);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Delete[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Deletion!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        // POST: /Area/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        [SessionExpire]
        [CustomAuthorize(Roles = "Area/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Area area = db.Areas.Find(id);
                db.Areas.Remove(area);
                db.SaveChanges();
                ViewBag.Messaage = "Area detail deleted Successfully";
                //return RedirectToAction("Index");
                return View();

            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Delete[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Deletion!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to delete Area detail";
                return View(db.Areas.Where(x => x.ID == id).FirstOrDefault());
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// to retrive district within state
        /// </summary>
        /// <param name="StateID">StateID</param>
        /// <returns>District  List Name & ID</returns>
        public JsonResult getDistrict(int StateID)
        {
            List<tempData> objODP = new List<tempData>();

            objODP = db.Districts
                .Where(x => x.StateID == StateID)
                .Select(x => new tempData { text = x.Name, value = x.ID }
                ).OrderBy(x => x.text)
                .ToList();

            return Json(objODP, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// to retrive City within district
        /// </summary>
        /// <param name="StateID">districtID</param>
        /// <returns>City  List Name & ID</returns>
        public JsonResult getCity(int DistrictID)
        {
            List<tempData> objODP = new List<tempData>();

            objODP = db.Cities
                .Where(x => x.DistrictID == DistrictID)
                .Select(x => new tempData { text = x.Name, value = x.ID }
                ).OrderBy(x => x.text)
                .ToList();

            return Json(objODP, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// to retrive Pincode within City
        /// </summary>
        /// <param name="StateID">CityID</param>
        /// <returns>Pincode  List Name & ID</returns>
        public JsonResult getPincode(int CityID)
        {
            List<tempData> objODP = new List<tempData>();

            objODP = db.Pincodes
                .Where(x => x.CityID == CityID)
                .Select(x => new tempData { text = x.Name, value = x.ID }
                ).OrderBy(x => x.text)
                .ToList();

            return Json(objODP, JsonRequestBehavior.AllowGet);
        }

        public class tempData
        {
            public Int64 value;
            public string text;
        }
      
    }
}
