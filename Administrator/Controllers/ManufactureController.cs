
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using BusinessLogicLayer;
using System.Text;
using Administrator.Models;
using PagedList;
using PagedList.Mvc;


namespace Administrator.Controllers
{
    
    [SessionExpire]
    public class ManufactureController : Controller
    {


        private EzeeloDBContext db = new EzeeloDBContext();

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
       Environment.NewLine
       + "ErrorLog Controller : ManufactureController" + Environment.NewLine);
        [SessionExpire]
        //[CustomAuthorize(Roles = " Manufacture/CanRead")]
        // GET: Manufacture
        public ActionResult Index(int? page)
        {

            try
            {
                return View(db.Manufactures.OrderBy(x => x.Name).ToList());//.ToPagedList(page ?? 1, 20));
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
        //[CustomAuthorize(Roles = "Manufacture/CanRead")]
        public ActionResult Details( int ?id)
        {


            try
            {
                ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Manufacture manufacture = db.Manufactures.Find(id);
                if(manufacture==null)
                {
                    return HttpNotFound();
                }
                return View(manufacture);


               
            }
            catch(Exception ex)
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
        //[CustomAuthorize(Roles = "Manufacture/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                return View();
            }
            catch(Exception ex)
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
         [HttpPost]
         [ValidateAntiForgeryToken]
         //[SessionExpire]

        //[CustomAuthorize(Roles = "Manufacture/CanWrite")]
        public ActionResult Create([Bind(Include = "ID,Name,Description,IsActive,BrandID")] Manufacture manufacture, string newBRAND)
        {
            try
            {
                //int BrandID = 0; 
                ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name");
              
               string BrandName1 = db.Brands.Where(x => x.ID == manufacture.BrandID).Select(Y => Y.Name).First();

                manufacture.BrandName = BrandName1;
              
                string brandnew = newBRAND;
                manufacture.BrandIDNew = Convert.ToInt32(brandnew);
                manufacture.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                manufacture.CreateDate = DateTime.UtcNow.AddHours(5.30);
                manufacture.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                if (db.Manufactures.Where(x => x.Name == manufacture.Name).Count() > 0)
                {
                    ViewBag.Messaage = "Can not Save because Manufacture Name Already Exists..!";
                    return View(manufacture);
                }

                if(ModelState.IsValid)
                {
                    db.Manufactures.Add(manufacture);
                    db.SaveChanges();
                    ViewBag.Messaage = "Manufacture Detail Inserted Successfully";
                }
                
                    return View(manufacture);
            }

            catch(Exception ex)
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
                ViewBag.Messaage = "Unable to Inserted Manufacture Detail ";
                return View(manufacture);

            }
        }


        public ActionResult Edit(int? id)
        {
            try
            {
                ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.BrandIDNew = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Manufacture manufacture = db.Manufactures.Find(id);
                if (manufacture == null)
                {
                    return HttpNotFound();
                }
                return View(manufacture);
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        //[SessionExpire]
        //[CustomAuthorize(Roles = "Manufacture/CanWrite")]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,IsActive,BrandID,BrandIDNew")] Manufacture manufacture,  int newBRAND)
        {
            try
            {
                Manufacture lManufacture = db.Manufactures.Find(manufacture.ID);
                int brandnew = newBRAND;

                ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name");
                ViewBag.BrandIDNew = new SelectList(db.Brands, "ID", "Name");
                var ih = manufacture.BrandIDNew;
                string BrandName1 = db.Brands.Where(x => x.ID == manufacture.BrandID).Select(Y => Y.Name).First();
                string BrandName12 = db.Brands.Where(x => x.ID == manufacture.BrandIDNew).Select(Y => Y.Name).FirstOrDefault();
                manufacture.BrandName = BrandName1;
                manufacture.BrandIDNew = Convert.ToInt32(brandnew);

                ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name");
                if (lManufacture == null)
                {
                    return View("Error");
                }

                if (db.Manufactures.Where(x => x.Name == manufacture.Name && x.ID != manufacture.ID).Count() > 0)
                {
                    ViewBag.Messaage = "Can not Save because Manufacture Name Already Exists..!";
                    return View(manufacture);
                }

                manufacture.CreateBy = lManufacture.CreateBy;
                manufacture.CreateDate = lManufacture.CreateDate;
                manufacture.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                manufacture.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                manufacture.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                manufacture.DeviceID = string.Empty;
                manufacture.DeviceType = string.Empty;


                //TryUpdateModel(mANUFECTURER);
                if (ModelState.IsValid)
                {
                    db.Entry(lManufacture).CurrentValues.SetValues(manufacture);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Manufacture Detail Updated Successfully";
                }
                return View(manufacture);
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

                ViewBag.Messaage = "Unable to Update Brand Detail ";
                return View(manufacture);
            }
        }

        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Manufacture manufacture = db.Manufactures.Find(id);
                if (manufacture == null)
                {
                    return HttpNotFound();
                }
                return View(manufacture);
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

    
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[SessionExpire]
        //[CustomAuthorize(Roles = "Manufacture/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Manufacture manufacture = db.Manufactures.Find(id);

                if (manufacture == null)
                {
                    return View("Error");
                }
                db.Manufactures.Remove(manufacture);
                db.SaveChanges();
                return RedirectToAction("Index");
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

                ViewBag.Messaage = "Unable to Delete Manufactures Detail";
                return View(db.Manufactures.Where(x => x.ID == id));
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