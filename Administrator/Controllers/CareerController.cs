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
using System.Globalization;
using Administrator.Models;

namespace Administrator.Controllers
{
    /// <summary>
    /// Developed By :- Pradnyakar Badge
    /// Purpose :- To Create New Opening that will availabe on the customer portal
    /// </summary>
    public class CareerController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
          Environment.NewLine
          + "ErrorLog Controller : CareerController" + Environment.NewLine);
        // GET: /Career/
        [SessionExpire]
        [CustomAuthorize(Roles = "Career/CanRead")]        
        public ActionResult Index()
        {
            return View(db.Careers.ToList());
        }

        // GET: /Career/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "Career/CanRead")]     
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Career career = db.Careers.Find(id);
            if (career == null)
            {
                return HttpNotFound();
            }
            return View(career);
        }

        // GET: /Career/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "Career/CanWrite")]     
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Career/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Career/CanWrite")]     
        public ActionResult Create([Bind(Include = "ID,Jobtitle,Education,ExperienceRequired,SkillRequired,NoOfOpening,Location,Domain,PostDate,ExpiryDate,Description,IsActive")] Career career, string txtPostDate, string txtExpiryDate)
        {

            try
            {
                
                if (!String.IsNullOrEmpty(txtPostDate))
                {
                    string from = txtPostDate.ToString();
                    string[] f = from.Split('/');
                    string[] ftime = f[2].Split(' ');
                    DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                    career.PostDate = Convert.ToDateTime(frmd.ToShortDateString());                   
                }
                else
                {
                    ViewBag.Messaage = "PostDate is Required";
                    return View(career);
                }

                if (!String.IsNullOrEmpty(txtExpiryDate))
                {
                    string from = txtExpiryDate.ToString();
                    string[] f = from.Split('/');
                    string[] ftime = f[2].Split(' ');
                    DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                    career.ExpiryDate = Convert.ToDateTime(frmd.ToShortDateString());
                }
                else
                {
                    ViewBag.Messaage = "ExpiryDate is Required";
                    return View(career);
                }


                career.CreateDate = DateTime.UtcNow.AddHours(5.3);
                career.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                career.NetworkIP = CommonFunctions.GetClientIP();

                if (ModelState.IsValid)
                {
                    db.Careers.Add(career);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }


                ViewBag.Messaage = "Career Detail Created Successfully";
                return View(career);
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

        // GET: /Career/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "Career/CanWrite")]     
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Career career = db.Careers.Find(id);
            if (career == null)
            {
                return HttpNotFound();
            }
            DateTime? dtPost = new DateTime();
            DateTime? dtExpire = new DateTime();
            dtPost = career.PostDate;
            if (dtPost != null)
            {
                ViewBag.txtPostDate = dtPost.Value.Day + "/" + dtPost.Value.Month + "/" + dtPost.Value.Year;
            }
            dtExpire = career.ExpiryDate;
            if (dtExpire != null)
            {
                ViewBag.txtExpiryDate = dtExpire.Value.Day + "/" + dtExpire.Value.Month + "/" + dtExpire.Value.Year;
            }
            return View(career);
        }

        // POST: /Career/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "Career/CanWrite")]     
        public ActionResult Edit([Bind(Include = "ID,Jobtitle,Education,ExperienceRequired,SkillRequired,NoOfOpening,Location,Domain,PostDate,ExpiryDate,Description,IsActive")] Career career, string txtPostDate, string txtExpiryDate)
        {
            try
            {

                if (!String.IsNullOrEmpty(txtPostDate))
                {
                    string from = txtPostDate.ToString();
                    string[] f = from.Split('/');
                    string[] ftime = f[2].Split(' ');
                    DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                    career.PostDate = Convert.ToDateTime(frmd.ToShortDateString());
                }
                else
                {
                    ViewBag.Messaage = "PostDate is Required";
                    return View(career);
                }

                if (!String.IsNullOrEmpty(txtExpiryDate))
                {
                    string from = txtExpiryDate.ToString();
                    string[] f = from.Split('/');
                    string[] ftime = f[2].Split(' ');
                    DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                    career.ExpiryDate = Convert.ToDateTime(frmd.ToShortDateString());
                }
                else
                {
                    ViewBag.Messaage = "ExpiryDate is Required";
                    return View(career);
                }


                Career lcareer = db.Careers.Find(career.ID);
                career.CreateDate = lcareer.CreateDate;
                career.CreateBy = lcareer.CreateBy;
                career.ModifyDate = DateTime.UtcNow.AddHours(5.5);
                career.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                career.NetworkIP = CommonFunctions.GetClientIP();
                career.DeviceType = string.Empty;
                career.DeviceID = string.Empty;

                if (ModelState.IsValid)
                {
                    //db.Entry(career).State = EntityState.Modified;
                    db.Entry(lcareer).CurrentValues.SetValues(career);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                }
                ViewBag.Messaage = "Career Detail Save Successfully";
                return View(career);
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

        // GET: /Career/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Career career = db.Careers.Find(id);
        //    if (career == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(career);
        //}

        //// POST: /Career/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Career career = db.Careers.Find(id);
        //    db.Careers.Remove(career);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
