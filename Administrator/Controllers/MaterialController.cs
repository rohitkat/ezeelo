using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Text;
using Administrator.Models;
using BusinessLogicLayer;

namespace Administrator.Controllers
{
    public class MaterialController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
        Environment.NewLine
        + "ErrorLog Controller : MaterialController" + Environment.NewLine);
      
        [SessionExpire]
        [CustomAuthorize(Roles = "Material/CanRead")]        
        // GET: /Material/
        public ActionResult Index()
        {
            try
            {
                var materials = db.Materials.Include(m => m.PersonalDetail).Include(m => m.PersonalDetail1);
                return View(materials.ToList());
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
        [CustomAuthorize(Roles = "Material/CanRead")]    
        // GET: /Material/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Material material = db.Materials.Find(id);
                if (material == null)
                {
                    return HttpNotFound();
                }
                return View(material);
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

        [SessionExpire]
        [CustomAuthorize(Roles = "Material/CanWrite")]
        // GET: /Material/Create
        public ActionResult Create()
        {            
            return View();
        }

        // POST: /Material/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Material/CanWrite")]
        public ActionResult Create([Bind(Include = "Name,IsActive")] Material material)
        {
            try
            {

                if (db.Materials.Where(x => x.Name == material.Name).Count() > 0)
                {
                    ViewBag.lblError = "Material name is already Exist..!!";
                    return View(material);
                }


                material.CreateDate = DateTime.UtcNow.AddHours(5.3);
                material.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                material.NetworkIP = CommonFunctions.GetClientIP();


                if (ModelState.IsValid)
                {
                    db.Materials.Add(material);
                    db.SaveChanges();
                    // return RedirectToAction("Index");
                }
                ViewBag.Messaage = "material Detail Created Successfully";
                return View(material);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Create[HttpPost]" + Environment.NewLine +
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

        [SessionExpire]
        [CustomAuthorize(Roles = "Material/CanWrite")]
        // GET: /Material/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Material material = db.Materials.Find(id);
                if (material == null)
                {
                    return HttpNotFound();
                }

                return View(material);
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

        // POST: /Material/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Material/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,Name,IsActive")] Material material)
        {

            try
            {

                if (db.Materials.Where(x => x.Name == material.Name && x.ID != material.ID).Count() > 0)
                {
                    ViewBag.lblError = "Material name is already Exist..!!";
                    return View(material);
                }

                Material lMaterial = db.Materials.Find(material.ID);
                material.CreateDate = lMaterial.CreateDate;
                material.CreateBy = lMaterial.CreateBy;
                material.ModifyDate = DateTime.UtcNow.AddHours(5.5);
                material.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                material.NetworkIP = CommonFunctions.GetClientIP();
                material.DeviceType = string.Empty;
                material.DeviceID = string.Empty;

                // TryUpdateModel(Material);

                if (ModelState.IsValid)
                {
                    db.Entry(lMaterial).CurrentValues.SetValues(material);
                    db.SaveChanges();

                    // return RedirectToAction("Index");
                }
                ViewBag.Messaage = "material Detail Modified Successfully";
                return View(material);
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

                return View();
            }
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "Material/CanDelete")]
        // GET: /Material/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Material material = db.Materials.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "Material/CanDelete")]
        // POST: /Material/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Material material = db.Materials.Find(id);
            db.Materials.Remove(material);
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
    }
}
