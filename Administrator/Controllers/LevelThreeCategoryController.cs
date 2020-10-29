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
using System.Text;
using BusinessLogicLayer;
using Administrator.Models;
using System.Web.Script.Serialization;
using System.Data.Entity.Validation;

namespace Administrator.Controllers
{
    [SessionExpire]
    public class LevelThreeCategoryController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
        Environment.NewLine
        + "ErrorLog Controller : AdvertisementController" + Environment.NewLine);
        // GET: /LevelThreeCategory/
        [CustomAuthorize(Roles = "LevelThreeCategory/CanRead")]
        public ActionResult Index()
        {
            try
            {
                var categories = db.Categories.Include(c => c.Category2).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Where(c => c.Level == 3);
                return View(categories.OrderBy(x => x.Name).ToList());
               // return View();
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

        // GET: /LevelThreeCategory/Details/5
        [CustomAuthorize(Roles = "LevelThreeCategory/CanRead")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }
                return View(category);
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

        // GET: /LevelThreeCategory/Create
        [CustomAuthorize(Roles = "LevelThreeCategory/CanWrite")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.ParentCategoryID = new SelectList(db.Categories.OrderBy(x => x.Name).Where(x => x.Level == 2).ToList(), "ID", "Name");
                return View();
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

        // POST: /LevelThreeCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "LevelThreeCategory/CanWrite")]
        public ActionResult Create([Bind(Include = "ID,Name,ParentCategoryID,Description,SearchKeyword,IsActive,IsExpire,ExpiryDate")] Category category)
        {
            try
            {
                ViewBag.ParentCategoryID = new SelectList(db.Categories.OrderBy(x => x.Name).Where(x => x.Level == 2).ToList(), "ID", "Name", category.ParentCategoryID);

                List<Category> lDuplicate = new List<Category>();

                lDuplicate = db.Categories.Where(x => x.Name == category.Name && x.Level == 3 && x.ParentCategoryID == category.ParentCategoryID).ToList();

                if (lDuplicate.Count() > 0)
                {
                    ViewBag.Messaage = "Unable to Inserted duplicate Category successfully";
                    return View(category);
                }
                if (category.IsExpire == true && category.ExpiryDate == null)
                {
                    ViewBag.Messaage = "Please Provide Expiry date..";
                    return View(category);
                }
                category.Level = 3;
                category.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                category.CreateDate = DateTime.UtcNow.AddHours(5.30);
                category.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                category.DeviceID = string.Empty;
                category.DeviceType = string.Empty;

                if (ModelState.IsValid)
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Level Three Category Detail Inserted Successfully";
                }

                

                return View(category);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Create[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Create View!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Create View " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Insert category Detail ";
                return View(category);
            }
        }

        // GET: /LevelThreeCategory/Edit/5
        [CustomAuthorize(Roles = "LevelThreeCategory/CanWrite")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ParentCategoryID = new SelectList(db.Categories.Where(x => x.Level == 2).ToList(), "ID", "Name", category.ParentCategoryID);

                return View(category);
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

        // POST: /LevelThreeCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "LevelThreeCategory/CanWrite")]
        public ActionResult Edit([Bind(Include = "ID,Name,ParentCategoryID,Description,SearchKeyword,IsActive,IsExpire,ExpiryDate")] Category category)
        {
            try
            {
                ViewBag.ParentCategoryID = new SelectList(db.Categories.Where(x => x.Level == 2).ToList(), "ID", "Name", category.ParentCategoryID);

                List<Category> lDuplicate = new List<Category>();

                lDuplicate = db.Categories.Where(x => x.Name == category.Name && x.Level == 3 && x.ParentCategoryID == category.ParentCategoryID && x.ID != category.ID).ToList();

                if (lDuplicate.Count() > 0)
                {
                    ViewBag.Messaage = "Unable to Inserted duplicate Category successfully";
                    return View(category);
                }
                if (category.IsExpire == true && category.ExpiryDate == null)
                {
                    ViewBag.Messaage = "Please Provide Expiry date..";
                    return View(category);
                }
                Category lData = db.Categories.Single(x => x.ID == category.ID);

                category.Level = 3;

                category.CreateBy = lData.CreateBy;
                category.CreateDate = lData.CreateDate;
                category.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])); 
                category.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                category.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                category.DeviceID = string.Empty;
                category.DeviceType = string.Empty;

                if (ModelState.IsValid)
                {
                    db.Entry(lData).CurrentValues.SetValues(category);
                    // db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Messaage = "Level Three Category Detail Updated Successfully";
                }
               

                return View(category);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Edit[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Update!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Update!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.Messaage = "Unable to Update LevelThreeCategory Detail ";
                return View(category);
            }
        }

        // GET: /LevelThreeCategory/Delete/5
        [CustomAuthorize(Roles = "LevelThreeCategory/CanDelete")]
        public ActionResult Delete(int? id)
        {
            try
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }
                return View(category);
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

        // POST: /LevelThreeCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "LevelThreeCategory/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Category category = db.Categories.Find(id);
                db.Categories.Remove(category);
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

                ViewBag.Messaage = "Unable to Delete LevelThreeCategory Detail ";
                return View(db.Categories.Where(x => x.ID == id).FirstOrDefault());
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



        public ActionResult AjaxHandler(jQueryDataTableParamModel param)
        {
            try
            {
                int DisplayLength, DisplayStart, SortCol; string SortDir, Search; int CategoryLevel;
                //DisplayLength = int.Parse(context.Request["iDisplayLength"]);
                //DisplayStart = int.Parse(context.Request["iDisplayStart"]);
                //SortCol = int.Parse(context.Request["iSortCol_0"]);
                //SortDir = context.Request["sSortDir_0"];
                //Search = context.Request["sSearch"];
                DisplayLength = param.iDisplayLength;
                DisplayStart = param.iDisplayStart;
                SortCol = param.iSortCol_0;
                SortDir = param.sSortDir_0;// param.sSortDir;
                Search = param.sSearch;

                DataTable dt = new DataTable();
                ReadConfig config = new ReadConfig(System.Web.HttpContext.Current.Server);
                DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
                List<object> paramValues = new List<object>();
                paramValues.Add(DisplayLength);
                paramValues.Add(DisplayStart);
                paramValues.Add(SortCol);
                paramValues.Add(SortDir);
                paramValues.Add(Search);
                paramValues.Add(3);
                dt = dbOpr.GetRecords("SelectCategory", paramValues);

                List<CategoryGirdData> lst = new List<CategoryGirdData>();
                //lst = (from n in dt.AsEnumerable()
                //       select new CategoryGirdData
                //       {
                //           C1_ID = Convert.ToInt64(n.Field<Int64>("C1_ID")),
                //           C1_Name = Convert.ToString(n.Field<string>("C1_Name")),
                //           C2_ID = Convert.ToInt64(n.Field<Int64>("C2_ID")),
                //           C2_Name = Convert.ToString(n.Field<string>("C2_Name")),
                //           C3_ID = Convert.ToInt64(n.Field<Int64>("C3_ID")),
                //           C3_Name = Convert.ToString(n.Field<string>("C3_Name"))
                //           //C3_IsActive = n.Field<bool>("IsActive"),
                //          // totalCount = Convert.ToInt32(n.Field<Int32>("TotalCount"))
                //       }).ToList();

                for (int i=0; i< dt.Rows.Count; i++)
                {
                    CategoryGirdData dd = new CategoryGirdData();
                    dd.C1_ID = Convert.ToInt64(dt.Rows[i]["C1_ID"].ToString());
                    dd.C1_Name = dt.Rows[i]["C1_Name"].ToString();
                    dd.C2_ID = Convert.ToInt64(dt.Rows[i]["C2_ID"].ToString());
                    dd.C2_Name = dt.Rows[i]["C2_Name"].ToString();
                    dd.C3_ID = Convert.ToInt64(dt.Rows[i]["C3_ID"].ToString());
                    dd.C3_Name = dt.Rows[i]["C3_Name"].ToString();
                    dd.totalCount = Convert.ToInt32(dt.Rows[i]["TotalCount"].ToString());
                    //dd.IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"].ToString());
                    lst.Add(dd);
                }

                var result = new
                {
                    sEcho=3,
                    iTotalRecords = lst[0].totalCount,
                    iTotalDisplayRecords = DisplayLength,
                    aaData = lst
                };
                return Json(new
                {
                    sEcho = 3,
                    iTotalRecords = 1135,
                    iTotalDisplayRecords = 10,
                    aaData = lst.ToList()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => new { x.ErrorMessage, x.PropertyName });

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            
        }
        
    }
    public class CategoryGirdData
    {
        public long C3_ID { get; set; }
        public string C3_Name { get; set; }
       // public bool IsActive { get; set; }
        public long C2_ID { get; set; }
        public string C2_Name { get; set; }
        public long C1_ID { get; set; }
        public string C1_Name { get; set; }
        public int totalCount { get; set; }
    }

    public class jQueryDataTableParamModel
    {
        /// <summary>
        /// Request sequence number sent by DataTable,
        /// same value must be returned in response
        /// </summary>       
        public string sSortDir_0 { get; set; }

        /// <summary>
        /// Text used for filtering
        /// </summary>
        public string sSearch { get; set; }

        /// <summary>
        /// Number of records that should be shown in table
        /// </summary>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int iDisplayStart { get; set; }

        /// <summary>
        /// Number of columns in table
        /// </summary>
     //   public int iColumns { get; set; }

        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int iSortCol_0 { get; set; }

        /// <summary>
        /// Comma separated list of column names
        /// </summary>
       // public string sColumns { get; set; }
    }
}
