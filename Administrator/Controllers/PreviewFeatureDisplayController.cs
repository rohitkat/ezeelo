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
using PagedList;
using PagedList.Mvc;

namespace Administrator.Controllers
{
    public class PreviewFeatureDisplayController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int pageSize = 50;

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
        Environment.NewLine
        + "ErrorLog Controller : PreviewFeatureDisplayController" + Environment.NewLine);

        [SessionExpire]
        [CustomAuthorize(Roles = "PreviewFeatureDisplay/CanRead")]
        // GET: /PreviewFeatureDisplay/
        public ActionResult Index(int? page, string searchString)
        {

           // int TotalCount = 0;
            int pageNumber = (page ?? 1);

            if (!String.IsNullOrEmpty(searchString))
            {
                ViewBag.CurrentFilter = searchString;
                return View(db.PreviewFeatureDisplays.Include(p => p.Category).Include(p => p.PreviewFeature).Where(x => x.Category.Name.Contains(searchString))
                    .ToList().ToPagedList(pageNumber, pageSize));
            }
            return View(db.PreviewFeatureDisplays.Include(p => p.Category).Include(p => p.PreviewFeature).ToList().ToPagedList(pageNumber, pageSize));
           // return View(previewfeaturedisplays.ToList());
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "PreviewFeatureDisplay/CanRead")]
        // GET: /PreviewFeatureDisplay/Details/5
        public ActionResult Details(int? id)
        {
            PreviewFeatureDisplay previewfeaturedisplay = new PreviewFeatureDisplay();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                previewfeaturedisplay = db.PreviewFeatureDisplays.Find(id);
                if (previewfeaturedisplay == null)
                {
                    return HttpNotFound();
                }
                return View(previewfeaturedisplay);
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
                ViewBag.Messaage = "Unable to Inserted Brand Detail ";
                return View(previewfeaturedisplay);

            }
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "PreviewFeatureDisplay/CanWrite")]
        // GET: /PreviewFeatureDisplay/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.ThirdLevelCatID = new SelectList(db.Categories.Where(x => x.Level == 3 && x.IsActive==true), "ID", "Name");
                ViewBag.PreviewFeatureID = new SelectList(db.PreviewFeatures.Where(x=>x.IsActive==true), "ID", "PreviewFeatureName");
                return View();
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
                ViewBag.Messaage = "Unable to Inserted Brand Detail ";
                return View();

            }
        }

        // POST: /PreviewFeatureDisplay/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "PreviewFeatureDisplay/CanWrite")]
        public ActionResult Create([Bind(Include="ID,ThirdLevelCatID,PreviewFeatureID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] PreviewFeatureDisplay previewfeaturedisplay)
        {
            try
            {
                previewfeaturedisplay.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                previewfeaturedisplay.CreateDate = DateTime.UtcNow.AddHours(5.30);
                previewfeaturedisplay.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                if (db.PreviewFeatureDisplays.Where(x => x.PreviewFeatureID == previewfeaturedisplay.PreviewFeatureID && x.ThirdLevelCatID==previewfeaturedisplay.ThirdLevelCatID).Count() > 0)
                {
                    ViewBag.Messaage = "You are already apply specification to selected category..!";
                    ViewBag.ThirdLevelCatID = new SelectList(db.Categories.Where(x => x.Level == 3 && x.IsActive == true), "ID", "Name", previewfeaturedisplay.ThirdLevelCatID);
                    ViewBag.PreviewFeatureID = new SelectList(db.PreviewFeatures.Where(x => x.IsActive == true), "ID", "PreviewFeatureName", previewfeaturedisplay.PreviewFeatureID);
                    ViewBag.Messaage = "Record already Present.....";
                    return View(previewfeaturedisplay);
                }

                if (ModelState.IsValid)
                {
                    db.PreviewFeatureDisplays.Add(previewfeaturedisplay);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.ThirdLevelCatID = new SelectList(db.Categories.Where(x => x.Level == 3 && x.IsActive == true), "ID", "Name", previewfeaturedisplay.ThirdLevelCatID);
                ViewBag.PreviewFeatureID = new SelectList(db.PreviewFeatures.Where(x => x.IsActive == true), "ID", "PreviewFeatureName", previewfeaturedisplay.PreviewFeatureID);
                return View(previewfeaturedisplay);
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
                ViewBag.Messaage = "Unable to Inserted Brand Detail ";
                return View(previewfeaturedisplay);

            }
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "PreviewFeatureDisplay/CanWrite")]
        // GET: /PreviewFeatureDisplay/Edit/5
        public ActionResult Edit(int? id)
        {
            PreviewFeatureDisplay previewfeaturedisplay = new PreviewFeatureDisplay();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                previewfeaturedisplay = db.PreviewFeatureDisplays.Find(id);
                if (previewfeaturedisplay == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ThirdLevelCatID = new SelectList(db.Categories.Where(x => x.Level == 3 && x.IsActive == true), "ID", "Name", previewfeaturedisplay.ThirdLevelCatID);
                ViewBag.PreviewFeatureID = new SelectList(db.PreviewFeatures.Where(x => x.IsActive == true), "ID", "PreviewFeatureName", previewfeaturedisplay.PreviewFeatureID);
                return View(previewfeaturedisplay);
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
                ViewBag.Messaage = "Unable to Inserted Brand Detail ";
                return View(previewfeaturedisplay);

            }
        }

        // POST: /PreviewFeatureDisplay/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "PreviewFeatureDisplay/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,ThirdLevelCatID,PreviewFeatureID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] PreviewFeatureDisplay previewfeaturedisplay)
        {
            try
            {
                previewfeaturedisplay.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                previewfeaturedisplay.CreateDate = DateTime.UtcNow.AddHours(5.30);
                previewfeaturedisplay.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                //if (db.PreviewFeatureDisplays.Where(x => x.PreviewFeatureID == previewfeaturedisplay.PreviewFeatureID && x.ThirdLevelCatID == previewfeaturedisplay.ThirdLevelCatID).Count() > 0)
                //{
                //    ViewBag.Messaage = "You are already apply specification to selected category..!";
                //    ViewBag.ThirdLevelCatID = new SelectList(db.Categories.Where(x => x.Level == 3 && x.IsActive == true), "ID", "Name", previewfeaturedisplay.ThirdLevelCatID);
                //    ViewBag.PreviewFeatureID = new SelectList(db.PreviewFeatures.Where(x => x.IsActive == true), "ID", "PreviewFeatureName", previewfeaturedisplay.PreviewFeatureID);
                //    return View(previewfeaturedisplay);
                //}
                if (ModelState.IsValid)
                {
                    db.Entry(previewfeaturedisplay).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.ThirdLevelCatID = new SelectList(db.Categories.Where(x => x.Level == 3 && x.IsActive == true), "ID", "Name", previewfeaturedisplay.ThirdLevelCatID);
                ViewBag.PreviewFeatureID = new SelectList(db.PreviewFeatures.Where(x => x.IsActive == true), "ID", "PreviewFeatureName", previewfeaturedisplay.PreviewFeatureID);
                return View(previewfeaturedisplay);
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
                ViewBag.Messaage = "Unable to Inserted Brand Detail ";
                return View(previewfeaturedisplay);

            }
        }


        [SessionExpire]
        [CustomAuthorize(Roles = "PreviewFeatureDisplay/CanDelete")]
        // GET: /PreviewFeatureDisplay/Delete/5
        public ActionResult Delete(int? id)
        {
            PreviewFeatureDisplay previewfeaturedisplay = new PreviewFeatureDisplay();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                previewfeaturedisplay = db.PreviewFeatureDisplays.Find(id);
                if (previewfeaturedisplay == null)
                {
                    return HttpNotFound();
                }
                return View(previewfeaturedisplay);
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
                ViewBag.Messaage = "Unable to Inserted Brand Detail ";
                return View(previewfeaturedisplay);

            }
        }


        [SessionExpire]
        [CustomAuthorize(Roles = "PreviewFeatureDisplay/CanDelete")]
        // POST: /PreviewFeatureDisplay/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PreviewFeatureDisplay previewfeaturedisplay=new PreviewFeatureDisplay();
            try
            {
                previewfeaturedisplay = db.PreviewFeatureDisplays.Find(id);
                db.PreviewFeatureDisplays.Remove(previewfeaturedisplay);
                db.SaveChanges();
                return RedirectToAction("Index");
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
                ViewBag.Messaage = "Unable to Inserted Brand Detail ";
                return View(previewfeaturedisplay);

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
    }
}
