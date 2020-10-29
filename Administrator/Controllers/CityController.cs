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
namespace  Administrator.Controllers
{
    /// <summary>
    /// To Create master entry of City Name
    /// </summary>
    [SessionExpire]
    public class CityController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
       
        // GET: /City/
        [SessionExpire]
        [CustomAuthorize(Roles = "City/CanRead")]
        public ActionResult Index(int? page, int? district, int? state)
        {
            try
            {

                ViewBag.State = new SelectList(db.States.ToList(), "ID", "Name");
                List<SelectListItem> lData = new List<SelectListItem>();
                if (state > 0)
                {
                    

                    ViewBag.district = new SelectList(db.Districts.Where(x => x.StateID == state).ToList(), "ID", "Name");
                }
                else
                {
                    lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
                    ViewBag.district = new SelectList(lData, "Value", "Text");
                }

                List<City> cities = new List<City>();

                if (district > 0)
                {
                    cities = db.Cities.Where(x => x.DistrictID == district).Include(c => c.District).ToList();
                    
                }
                else
                {
                    cities = db.Cities.Include(c => c.District).ToList();
                }
                return View(cities.OrderBy(x => x.Name).ToList().ToPagedList(page ?? 1, 2000));
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Index view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CityController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Generate Index view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CityController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // GET: /City/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "City/CanRead")]
        public ActionResult Details(long? id)
        {
            try
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                City city = db.Cities.Find(id);
                if (city == null)
                {
                    return HttpNotFound();
                }
                return View(city);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Detail!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CityController][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Detail!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CityController][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // GET: /City/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "City/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.DistrictID = new SelectList(db.Districts.OrderBy(x => x.Name), "ID", "Name");
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Creation!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CityController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Creation!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CityController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // POST: /City/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "City/CanWrite")]
        public ActionResult Create([Bind(Include="ID,Name,DistrictID,IsActive")] City city)
        {
            try
            {
                city.CreateDate = DateTime.UtcNow.AddHours(5.30);
                city.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                city.ModifyDate = null;
                city.ModifyBy = null;
                city.NetworkIP = string.Empty;
                city.DeviceType = string.Empty;
                city.DeviceID = string.Empty;

                TryUpdateModel(city);

                if (ModelState.IsValid)
                {
                    db.Cities.Add(city);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "City Detail Inserted Successfully";
                }

                ViewBag.DistrictID = new SelectList(db.Districts.OrderBy(x => x.Name), "ID", "Name", city.DistrictID);
                return View(city);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Creation!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CityController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Inserted City Detail ";
                return View(city);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Creation!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CityController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Inserted City Detail ";
                return View(city);
            }

        }

        // GET: /City/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "City/CanWrite")]
        public ActionResult Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                City city = db.Cities.Find(id);
                if (city == null)
                {
                    return HttpNotFound();
                }
                ViewBag.DistrictID = new SelectList(db.Districts.OrderBy(x => x.Name), "ID", "Name", city.DistrictID);
                return View(city);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CityController][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CityController][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // POST: /City/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "City/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,Name,DistrictID,IsActive")] City city)
        {
            try
            {
                City lCity = db.Cities.Find(city.ID);

                city.CreateDate = lCity.CreateDate;
                city.CreateBy = lCity.CreateBy;
                city.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                city.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                city.NetworkIP = string.Empty;
                city.DeviceType = string.Empty;
                city.DeviceID = string.Empty;

                //TryUpdateModel(city);

                if (ModelState.IsValid)
                {
                    //db.Entry(city).State = EntityState.Modified;
                    db.Entry(lCity).CurrentValues.SetValues(city);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "City Detail Update Successfully";
                }
                ViewBag.DistrictID = new SelectList(db.Districts.OrderBy(x => x.Name), "ID", "Name", city.DistrictID);
                return View(city);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CityController][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                
                ViewBag.Messaage = "Unable to Updated City Detail ";
                return View(city);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Update!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CityController][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Updated City Detail ";
                return View(city);
            }
        }

        // GET: /City/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "City/CanDelete")]
        public ActionResult Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                City city = db.Cities.Find(id);
                if (city == null)
                {
                    return HttpNotFound();
                }
                return View(city);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CityController][GET:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CityController][GET:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        // POST: /City/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "City/CanDelete")]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                City city = db.Cities.Find(id);
                db.Cities.Remove(city);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CityController][POST:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Delete City Detail ";
                return View(db.Cities.Where(x => x.ID == id).FirstOrDefault());

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Sorry! Problem in Record Deletion!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CityController][POST:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Delete City Detail ";
                return View(db.Cities.Where(x => x.ID == id).FirstOrDefault());
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
        /// to retrive list of district under state
        /// </summary>
        /// <param name="stateid">State ID</param>
        /// <returns>districtList</returns>
        public JsonResult DistrictList(int stateid)
        {
            List<DsList> lData = new List<DsList>();
            lData = (from n in db.Districts
                     where n.StateID == stateid
                     select new DsList
                     {
                         ID = n.ID,
                         Name = n.Name
                     }).ToList();



            return Json(lData, JsonRequestBehavior.AllowGet);
        }


    }

    public class DsList
    {
        public Int64 ID { get; set; }

        public string Name { get; set; }
    }
}
