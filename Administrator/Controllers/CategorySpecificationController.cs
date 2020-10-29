using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Transactions;
using BusinessLogicLayer;
using Administrator.Models;
namespace Administrator.Controllers
{
    /// <summary>
    /// Developed By:- Pradnyakar Badge
    /// To attached specification with category
    /// 
    /// </summary>
    [SessionExpire]
    public class CategorySpecificationController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /CategorySpecification/
        [CustomAuthorize(Roles = "CategorySpecification/CanRead")]
        public ActionResult Index()
        {
            var categoryspecifications = db.CategorySpecifications.Include(c => c.Category).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.Specification);

            return View(categoryspecifications.ToList());
        }

        // GET: /CategorySpecification/Details/5
        [CustomAuthorize(Roles = "CategorySpecification/CanRead")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategorySpecification categoryspecification = db.CategorySpecifications.Find(id);
            if (categoryspecification == null)
            {
                return HttpNotFound();
            }
            return View(categoryspecification);
        }

        // GET: /CategorySpecification/Create
        [CustomAuthorize(Roles = "CategorySpecification/CanWrite")]
        public ActionResult Create()
        {
            ViewBag.CategoryLevelOne = new SelectList(db.Categories.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name");

            List<SelectListItem> lData = new List<SelectListItem>();
            lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
            ViewBag.CategoryLevelTwo = new SelectList(lData, "Value", "Text");
            ViewBag.CategoryLevelThree = new SelectList(lData, "Value", "Text");


            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            //----------Changes by Mohit on 20/10/15--------------------------//
            ViewBag.SpecificationID = new SelectList(db.Specifications.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name");

            return View();
        }

        // POST: /CategorySpecification/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "CategorySpecification/CanWrite")]
        public ActionResult Create(ModelLayer.Models.ViewModel.SpecificationList pSpecificationList)
        {
            //if (ModelState.IsValid)
            //{
            //    db.CategorySpecifications.Add(categoryspecification);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

            //ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", categoryspecification.CategoryID);
            //ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", categoryspecification.CreateBy);
            //ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", categoryspecification.ModifyBy);
            //ViewBag.SpecificationID = new SelectList(db.Specifications, "ID", "Name", categoryspecification.SpecificationID);
            ViewBag.CategoryLevelOne = new SelectList(db.Categories.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name");

            List<SelectListItem> lData = new List<SelectListItem>();
            lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
            ViewBag.CategoryLevelTwo = new SelectList(lData, "Value", "Text");
            ViewBag.CategoryLevelThree = new SelectList(lData, "Value", "Text");
           // ViewBag.SpecificationID = new SelectList(db.Specifications.OrderBy(x => x.Name), "ID", "Name");
            //----------Changes by Mohit on 20/10/15--------------------------//
            ViewBag.SpecificationID = new SelectList(db.Specifications.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name");
            InsertRecordCategorySecification(pSpecificationList);

            CategorySpecification categoryspecification = new CategorySpecification();


            return View(categoryspecification);
        }

        /// <summary>
        /// to insert list of specification aginst given category 
        /// </summary>
        /// <param name="pSpecificationList">list of specification id details</param>
        private void InsertRecordCategorySecification(ModelLayer.Models.ViewModel.SpecificationList pSpecificationList)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                       List<Category> lCategoryList = new List<Category>();
                       if (pSpecificationList.ThreeLevelID == 0)
                       {
                           lCategoryList = db.Categories.Where(x => x.ParentCategoryID == pSpecificationList.TwoLevelID && x.Level == 3).ToList();
                       }

                       if (lCategoryList.Count() < 1 && pSpecificationList.TwoLevelID > 0)
                       {
                           //loop through list of provided specification id 
                           for (int i = 0; i < pSpecificationList.pSpecificationViewModel.Count(); i++)
                           {
                               // check is to be selected or not
                               if (pSpecificationList.pSpecificationViewModel[i].isSelected)
                               {
                                   int catID, specificationid;
                                   catID = pSpecificationList.ThreeLevelID;
                                   specificationid = pSpecificationList.pSpecificationViewModel[i].ID;

                                   //check wether specification is already exist with category 
                                   if (db.CategorySpecifications.Where(x => x.SpecificationID == specificationid && x.CategoryID == catID).ToList().Count() < 1)
                                   {

                                       CategorySpecification objCatSpecification = new CategorySpecification();
                                       objCatSpecification.SpecificationID = pSpecificationList.pSpecificationViewModel[i].ID;
                                       objCatSpecification.CategoryID = pSpecificationList.ThreeLevelID;
                                       objCatSpecification.IsActive = true;
                                       objCatSpecification.CreateBy =  CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                                       objCatSpecification.CreateDate = DateTime.UtcNow.AddHours(5.30);
                                       objCatSpecification.DeviceID = string.Empty;
                                       objCatSpecification.DeviceType = string.Empty;

                                       db.CategorySpecifications.Add(objCatSpecification);
                                       db.SaveChanges();
                                   }

                               }
                           }
                       }
                       else
                       {
                           for (int i = 0; i < lCategoryList.Count(); i++)
                           {
                               for (int j = 0; j < pSpecificationList.pSpecificationViewModel.Count(); j++)
                               {
                                   if (pSpecificationList.pSpecificationViewModel[j].isSelected)
                                   {
                                       int catID, specificationid;
                                       catID = lCategoryList[i].ID;
                                       specificationid = pSpecificationList.pSpecificationViewModel[j].ID;
                                       if (db.CategorySpecifications.Where(x => x.SpecificationID == specificationid && x.CategoryID == catID).ToList().Count() < 1)
                                       {
                                           CategorySpecification objCatSpecification = new CategorySpecification();
                                           objCatSpecification.SpecificationID = pSpecificationList.pSpecificationViewModel[j].ID;
                                           objCatSpecification.CategoryID = lCategoryList[i].ID;
                                           objCatSpecification.IsActive = true;
                                           objCatSpecification.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                                           objCatSpecification.CreateDate = DateTime.UtcNow.AddHours(5.30);
                                           objCatSpecification.DeviceID = string.Empty;
                                           objCatSpecification.DeviceType = string.Empty;

                                           db.CategorySpecifications.Add(objCatSpecification);
                                           db.SaveChanges();
                                       }
                                   }
                               }
                           }
                       }
                        ts.Complete();
                        ViewBag.Message = "Category Specification Inserted Successfully";
                    }
                    catch
                    {
                        ts.Dispose();
                        ViewBag.Message = "Problem in Category Specification to Insert Successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Problem in Category Specification to Insert Successfully";
            }
        }

        // GET: /CategorySpecification/Edit/5
        [CustomAuthorize(Roles = "CategorySpecification/CanWrite")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategorySpecification categoryspecification = db.CategorySpecifications.Find(id);
            if (categoryspecification == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories.OrderBy(x => x.Name), "ID", "Name", categoryspecification.CategoryID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", categoryspecification.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", categoryspecification.ModifyBy);
            //ViewBag.SpecificationID = new SelectList(db.Specifications.OrderBy(x => x.Name), "ID", "Name", categoryspecification.SpecificationID);
           //----------Changes by Mohit on 20/10/15--------------------------//
            ViewBag.SpecificationID = new SelectList(db.Specifications.Where(x=>x.Level == 2).OrderBy(x => x.Name), "ID", "Name", categoryspecification.SpecificationID);
            return View(categoryspecification);
        }

        // POST: /CategorySpecification/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "CategorySpecification/CanWrite")]
        public ActionResult Edit([Bind(Include="ID,CategoryID,SpecificationID,IsActive")] CategorySpecification categoryspecification)
        {
            CategorySpecification lcategorySpecification = new CategorySpecification();
            lcategorySpecification = db.CategorySpecifications.Where(x => x.ID == categoryspecification.ID).FirstOrDefault();
            categoryspecification.CreateBy = lcategorySpecification.CreateBy;
            categoryspecification.CreateDate = lcategorySpecification.CreateDate;
            categoryspecification.ModifyDate = DateTime.UtcNow.AddHours(5.30);
            categoryspecification.ModifyBy =  CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
            categoryspecification.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            categoryspecification.DeviceType = string.Empty;
            categoryspecification.DeviceID = string.Empty;

            if (ModelState.IsValid)
            {
                db.Entry(lcategorySpecification).CurrentValues.SetValues(categoryspecification);
                //db.Entry(categoryspecification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories.OrderBy(x => x.Name), "ID", "Name", categoryspecification.CategoryID);
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", categoryspecification.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", categoryspecification.ModifyBy);
            //ViewBag.SpecificationID = new SelectList(db.Specifications.OrderBy(x => x.Name), "ID", "Name", categoryspecification.SpecificationID);
            //----------Changes by Mohit on 20/10/15--------------------------//
            ViewBag.SpecificationID = new SelectList(db.Specifications.Where(x => x.Level == 2).OrderBy(x => x.Name), "ID", "Name", categoryspecification.SpecificationID);
            return View(categoryspecification);
        }

        // GET: /CategorySpecification/Delete/5
        [CustomAuthorize(Roles = "CategorySpecification/CanDelete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategorySpecification categoryspecification = db.CategorySpecifications.Find(id);
            if (categoryspecification == null)
            {
                return HttpNotFound();
            }
            return View(categoryspecification);
        }

        // POST: /CategorySpecification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "CategorySpecification/CanDelete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CategorySpecification categoryspecification = db.CategorySpecifications.Find(id);
            db.CategorySpecifications.Remove(categoryspecification);
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
        
        
        /// <summary>
        /// To retrive list of specification
        /// </summary>
        /// <param name="TwoLevelID"></param>
        /// <param name="ThreeLevelID"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SpecificationList(Int32 TwoLevelID, Int32 ThreeLevelID, int id)
        {
            //List<Specification> lspecification = new List<Specification>();
            //lspecification = db.Specifications.Where(x => x.ParentSpecificationID == id || x.ID == id).ToList();
            //ViewBag.TwoLevelID = TwoLevelID;
            //ViewBag.ThreeLevelID = ThreeLevelID;

            ModelLayer.Models.ViewModel.SpecificationList lspecification = new ModelLayer.Models.ViewModel.SpecificationList();
            //----------Changes by Mohit on 20/10/15--------------------------//
            lspecification.pSpecificationViewModel = (from n in db.Specifications
                                                      where n.ParentSpecificationID == id
                                                      select new ModelLayer.Models.ViewModel.SpecificationViewModel
                                                      {
                                                          Name = n.Name,
                                                          ID = n.ID,
                                                          level = n.Level,
                                                          isSelected = false
                                                      }).OrderBy(x => x.Name).ToList();
            //lspecification.pSpecificationViewModel = (from n in db.Specifications
            //                                         where n.ParentSpecificationID == id || n.ID == id
            //                                          select new ModelLayer.Models.ViewModel.SpecificationViewModel
            //                                         {
            //                                             Name = n.Name,
            //                                             ID = n.ID,
            //                                             level = n.Level,
            //                                             isSelected = false
            //                                         }).OrderBy(x => x.Name).ToList();
 

            return PartialView("_SpecificationList", lspecification);
        }

        /// <summary>
        /// to get second level category list name and id
        /// </summary>
        /// <param name="cateID">first level category id </param>
        /// <returns>second level categoy list</returns>
        public JsonResult SecondLevelCategory(int cateID)
        {
            List<CategoryList> lcategory = new List<CategoryList>();
            //lcategory = db.Categories.Where(x => x.ParentCategoryID == cateID && x.Level == 2).ToList();
            lcategory = (from n in db.Categories
                         where n.ParentCategoryID == cateID && n.Level == 2
                         select new CategoryList
             {
                 ID = n.ID,
                 name = n.Name
             }).OrderBy(x => x.name).ToList();

            return Json(lcategory, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// to get third level category list name and id
        /// </summary>
        /// <param name="cateID">second level category id </param>
        /// <returns>third level categoy list</returns>
        public JsonResult ThirdLevelCategory(int cateID)
        {
            List<CategoryList> lcategory = new List<CategoryList>();
            //lcategory = db.Categories.Where(x => x.ParentCategoryID == cateID && x.Level == 2).ToList();
            lcategory = (from n in db.Categories
                         where n.ParentCategoryID == cateID && n.Level == 3
                         select new CategoryList
                         {
                             ID = n.ID,
                             name = n.Name
                         }).OrderBy(x => x.name).ToList();
            return Json(lcategory, JsonRequestBehavior.AllowGet);

        }

        
    }

    public class CategoryList
    {
        public Int32 ID { get; set; }
        public string name { get; set; }
    }

   

}
