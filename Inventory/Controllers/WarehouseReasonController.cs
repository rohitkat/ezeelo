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
using ModelLayer.Models.Enum;

namespace Inventory.Controllers
{
    public class WarehouseReasonController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /WarehouseReason/
        public ActionResult Index()
        {
            List<WarehouseReason> list = db.WarehouseReasons.Where(p => p.ParentReasonId == null && p.IsActive != false).OrderByDescending(p => p.ID).ToList();//Rumana 15/3/2019
            foreach (var item in list)
            {
                item.Reason = item.Reason + "(" + GetCategory(item.ParentCategoryId) + ")";
            }
            return View(list);
        }

        // GET: /WarehouseReason/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WarehouseReason warehousereason = db.WarehouseReasons.Find(id);


            PersonalDetail obj = new PersonalDetail();
            obj = db.PersonalDetails.SingleOrDefault(P => P.ID == warehousereason.CreateBy);
            if (obj != null)
            {
                ViewBag.CreatedBy = obj.FirstName + " " + obj.LastName;
            }
            if (warehousereason.ModifyBy != null)
            {
                obj = db.PersonalDetails.SingleOrDefault(P => P.ID == warehousereason.ModifyBy);
                ViewBag.ModifyBy = obj.FirstName + " " + obj.LastName;
            }
            if (warehousereason == null)
            {
                return HttpNotFound();
            }
            if (warehousereason.ParentCategoryId != 0)
            {
                switch (warehousereason.ParentCategoryId)
                {
                    case (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.RETURN:
                        ViewBag.Category = "RETURN";
                        break;
                    case (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.WASTAGE:
                        ViewBag.Category = "WASTAGE";
                        break;
                }
            }

            return View(warehousereason);
        }

        // GET: /WarehouseReason/Create Yashaswi 4/5/2018
        public ActionResult Create()
        {
            WarehouseReason objReason = new WarehouseReason();
            Reason_ParentCategory obj = new Reason_ParentCategory();
            objReason.CategoryList = obj.GetListFromEnum();

            return View(objReason);
        }

        // POST: /WarehouseReason/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WarehouseReason warehousereason)
        {
            if (ModelState.IsValid)
            {
                long PersonalDetailID = 1;
                if (Session["USER_LOGIN_ID"] != null)
                {
                    long ID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
                    PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == ID).Select(x => x.ID).First());
                }
                bool isSaved = false;
                foreach (var item in warehousereason.CategoryList)
                {
                    if (item.IsChecked)
                    {
                        warehousereason.IsActive = true;
                        warehousereason.CreateDate = DateTime.Now.Date;
                        warehousereason.CreateBy = PersonalDetailID;
                        warehousereason.ModifyBy = null;
                        warehousereason.ModifyDate = null;
                        warehousereason.NetworkIP = CommonFunctions.GetClientIP();
                        warehousereason.DeviceID = "X";
                        warehousereason.DeviceType = "X";
                        warehousereason.ParentReasonId = null;
                        warehousereason.ParentCategoryId = item.ID;
                        db.WarehouseReasons.Add(warehousereason);
                        db.SaveChanges();
                        isSaved = true;
                    }
                }
                if (isSaved)
                {
                    Session["Success"] = "Record Inserted Successfully.";//yashaswi 30/3/2018
                }
                return RedirectToAction("Index");
            }

            return View(warehousereason);
        }

        // GET: /WarehouseReason/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WarehouseReason warehousereason = db.WarehouseReasons.Find(id);
            Reason_ParentCategory obj = new Reason_ParentCategory();
            warehousereason.CategoryList = obj.GetListFromEnum();
            foreach (var item in warehousereason.CategoryList)
            {
                if (item.ID == warehousereason.ParentCategoryId)
                {
                    item.IsChecked = true;
                }
            }
            if (warehousereason == null)
            {
                return HttpNotFound();
            }
            return View(warehousereason);
        }

        // POST: /WarehouseReason/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Reason,IsActive")] WarehouseReason warehousereason)
        {
            if (ModelState.IsValid)
            {
                Reason_ParentCategory obj = new Reason_ParentCategory();
                warehousereason.CategoryList = obj.GetListFromEnum();
                WarehouseReason objwarehousereason = db.WarehouseReasons.Find(warehousereason.ID);
                ///to check id is used by wastage
                List<WarehouseReason> wr = db.WarehouseReasons.Where(p => p.ParentReasonId == warehousereason.ID && p.IsActive != false).ToList();//Rumana 15/3/2019
                bool isUsed = false;
                if (wr.Count != 0)
                {
                    foreach (var item in wr)
                    {
                        List<WarehouseWastageStock> list = db.WarehouseWastageStock.Where(p => p.SubReasonID == item.ID).ToList();
                        if (list.Count != 0)
                        {
                            isUsed = true;
                        }
                    }
                }
                //
                if (isUsed == true && objwarehousereason.Reason != warehousereason.Reason.Trim())
                {
                    ModelState.AddModelError("Reason", "Reason is in used. You can't change its text");
                    Session["Warning"] = "Reason is in used. You can't change its text";//yashaswi 30/3/2018
                    return View(warehousereason);
                }
                else
                {
                    objwarehousereason.Reason = warehousereason.Reason;
                    objwarehousereason.IsActive = warehousereason.IsActive;

                    long PersonalDetailID = 1;
                    if (Session["USER_LOGIN_ID"] != null)
                    {
                        long ID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
                        PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == ID).Select(x => x.ID).First());
                    }
                    objwarehousereason.ModifyBy = PersonalDetailID;
                    objwarehousereason.ModifyDate = DateTime.Now.Date;
                    db.Entry(objwarehousereason).State = EntityState.Modified;
                    db.SaveChanges();
                    Session["Success"] = "Record Updated Successfully.";//yashaswi 30/3/2018
                    return RedirectToAction("Index");
                }
            }
            return View(warehousereason);
        }

        // GET: /WarehouseReason/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WarehouseReason warehousereason = db.WarehouseReasons.Find(id);
            if (warehousereason == null)
            {
                return HttpNotFound();
            }
            return View(warehousereason);
        }

        // POST: /WarehouseReason/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WarehouseReason warehousereason = db.WarehouseReasons.Find(id);
            db.WarehouseReasons.Remove(warehousereason);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult IndexSubReason()
        {
            List<WarehouseReason> listReason = db.WarehouseReasons.Where(p => p.ParentReasonId != null && p.IsActive != false).ToList();//Rumana 15/3/2019
            foreach (var item in listReason)
            {
                WarehouseReason obj = db.WarehouseReasons.FirstOrDefault(p => p.ID == item.ParentReasonId);
                item.DeviceType = obj.Reason + "(" + GetCategory(obj.ParentCategoryId) + ")";
            }
            return View(listReason);
        }
        public ActionResult CreateSubReason()
        {
            WarehouseReason obj = new WarehouseReason();
            obj.WarehouseReasonlist = new SelectList(GetMainReasonList(), "Id", "Reason");
            return View(obj);
        }

        public List<WarehouseReason> GetMainReasonList()
        {
            List<WarehouseReason> list = db.WarehouseReasons.Where(p => p.ParentReasonId == null && p.IsActive != false).ToList();//Rumana 15/3/2019
            foreach (var item in list)
            {
                item.Reason = item.Reason + "(" + GetCategory(item.ParentCategoryId) + ")";
            }
            return list;
        }

        // POST: /WarehouseReason/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSubReason([Bind(Include = "ID,Reason,ParentReasonId")] WarehouseReason warehousereason)
        {
            if (ModelState.IsValid)
            {
                if (warehousereason.ParentReasonId == null)
                {
                    ModelState.AddModelError("ParentReasonId", "Please Select Main Reason");
                    warehousereason.WarehouseReasonlist = new SelectList(GetMainReasonList(), "Id", "Reason");
                    return View(warehousereason);
                }
                warehousereason.IsActive = true;
                warehousereason.CreateDate = DateTime.Now.Date;

                long PersonalDetailID = 1;
                if (Session["USER_LOGIN_ID"] != null)
                {
                    long ID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
                    PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == ID).Select(x => x.ID).First());
                }

                warehousereason.CreateBy = PersonalDetailID;
                warehousereason.ModifyBy = null;
                warehousereason.ModifyDate = null;
                warehousereason.NetworkIP = CommonFunctions.GetClientIP();
                warehousereason.DeviceID = "X";
                warehousereason.DeviceType = "X";
                db.WarehouseReasons.Add(warehousereason);
                db.SaveChanges();
                Session["Success"] = "Record Inserted Successfully.";//yashaswi 30/3/2018
                return RedirectToAction("IndexSubReason");
            }
            warehousereason.WarehouseReasonlist = new SelectList(GetMainReasonList(), "Id", "Reason", warehousereason.ParentReasonId);
            return View(warehousereason);
        }


        public ActionResult EditSubReason(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WarehouseReason warehousereason = db.WarehouseReasons.Find(id);
            if (warehousereason == null)
            {
                return HttpNotFound();
            }
            warehousereason.WarehouseReasonlist = new SelectList(GetMainReasonList(), "Id", "Reason", warehousereason.ParentReasonId);
            WarehouseReason warehousereason_ = db.WarehouseReasons.FirstOrDefault(p => p.ID == warehousereason.ParentReasonId);
            if (warehousereason_.ParentCategoryId != 0)
            {
                switch (warehousereason_.ParentCategoryId)
                {
                    case (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.RETURN:
                        ViewBag.Category = "RETURN";
                        break;
                    case (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.WASTAGE:
                        ViewBag.Category = "WASTAGE";
                        break;
                    default:
                        ViewBag.Category = "";
                        break;
                }
            }
            return View(warehousereason);
        }

        // POST: /WarehouseReason/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSubReason([Bind(Include = "ID,Reason,IsActive,ParentReasonId")] WarehouseReason warehousereason)
        {
            if (ModelState.IsValid)
            {
                if (warehousereason.ParentReasonId == null)
                {
                    ModelState.AddModelError("ParentReasonId", "Please Select Main Reason");
                    warehousereason.WarehouseReasonlist = new SelectList(GetMainReasonList(), "Id", "Reason");
                    Session["Warning"] = "Reason is in used. You can't change its text";//yashaswi 30/3/2018
                    return View(warehousereason);
                }
                bool isUsed = false;
                List<WarehouseWastageStock> list = db.WarehouseWastageStock.Where(p => p.SubReasonID == warehousereason.ID).ToList();
                if (list.Count != 0)
                {
                    isUsed = true;
                }


                WarehouseReason objwarehousereason = db.WarehouseReasons.Find(warehousereason.ID);
                if (isUsed == true && (objwarehousereason.Reason != warehousereason.Reason.Trim() || objwarehousereason.ParentReasonId != warehousereason.ParentReasonId))
                {
                    warehousereason.WarehouseReasonlist = new SelectList(GetMainReasonList(), "Id", "Reason", warehousereason.ParentReasonId);
                    ModelState.AddModelError("Reason", "Reason is in used. You cant change its parent reason.");
                    Session["Warning"] = "Reason is in used. You cant change its parent reason.";//yashaswi 30/3/2018
                    return View(warehousereason);
                }
                objwarehousereason.Reason = warehousereason.Reason;
                objwarehousereason.IsActive = warehousereason.IsActive;

                long PersonalDetailID = 1;
                if (Session["USER_LOGIN_ID"] != null)
                {
                    long ID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
                    PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == ID).Select(x => x.ID).First());
                }
                objwarehousereason.ModifyBy = PersonalDetailID;
                objwarehousereason.ModifyDate = DateTime.Now.Date;
                objwarehousereason.ParentReasonId = warehousereason.ParentReasonId;
                db.Entry(objwarehousereason).State = EntityState.Modified;
                db.SaveChanges();
                Session["Success"] = "Record Updated Successfully.";//yashaswi 30/3/2018
                return RedirectToAction("IndexSubReason");
            }
            warehousereason.WarehouseReasonlist = new SelectList(GetMainReasonList(), "Id", "Reason", warehousereason.ParentReasonId);
            return View(warehousereason);
        }

        public ActionResult DeleteSubReason(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WarehouseReason warehousereason = db.WarehouseReasons.Find(id);
            if (warehousereason == null)
            {
                return HttpNotFound();
            }
            if (warehousereason.ParentReasonId != null)
            {
                ViewBag.MainReason = db.WarehouseReasons.Single(p => p.ID == warehousereason.ParentReasonId).Reason;
            }
            return View(warehousereason);
        }

        // POST: /WarehouseReason/Delete/5
        [HttpPost, ActionName("DeleteSubReason")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedSubReason(int id)
        {
            WarehouseReason warehousereason = db.WarehouseReasons.Find(id);
            db.WarehouseReasons.Remove(warehousereason);
            db.SaveChanges();
            Session["Success"] = "Record Deleted Successfully.";//yashaswi 30/3/2018
            return RedirectToAction("IndexSubReason");
        }

        public ActionResult DetailsSubReason(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WarehouseReason warehousereason = db.WarehouseReasons.Find(id);

            PersonalDetail obj = new PersonalDetail();
            obj = db.PersonalDetails.SingleOrDefault(P => P.ID == warehousereason.CreateBy);
            if (obj != null)
            {
                ViewBag.CreatedBy = obj.FirstName + " " + obj.LastName;
            }
            if (warehousereason.ModifyBy != null)
            {
                obj = db.PersonalDetails.SingleOrDefault(P => P.ID == warehousereason.ModifyBy);
                ViewBag.ModifyBy = obj.FirstName + " " + obj.LastName;
            }
            if (warehousereason == null)
            {
                return HttpNotFound();
            }
            if (warehousereason.ParentReasonId != null)
            {
                ViewBag.MainReason = db.WarehouseReasons.Single(p => p.ID == warehousereason.ParentReasonId).Reason;
            }
            WarehouseReason warehousereason_ = db.WarehouseReasons.FirstOrDefault(p => p.ID == warehousereason.ParentReasonId);
            if (warehousereason_.ParentCategoryId != 0)
            {
                switch (warehousereason_.ParentCategoryId)
                {
                    case (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.RETURN:
                        ViewBag.Category = "RETURN";
                        break;
                    case (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.WASTAGE:
                        ViewBag.Category = "WASTAGE";
                        break;
                    default:
                        ViewBag.Category = "";
                        break;
                }
            }
            return View(warehousereason);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public string GetCategory(long ID)
        {
            string Category = "";
            if (ID != 0)
            {
                switch (ID)
                {
                    case (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.RETURN:
                        Category = "RETURN";
                        break;
                    case (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.WASTAGE:
                        Category = "WASTAGE";
                        break;
                }
            }
            return Category;
        }

        public long GetCategoryId(long ID)
        {
            WarehouseReason obj = db.WarehouseReasons.FirstOrDefault(p => p.ID == ID);
            return obj.ParentCategoryId;
        }


        [HttpPost]
        public JsonResult Get_Category(long ID)
        {
            return Json(GetCategory(GetCategoryId(ID)), JsonRequestBehavior.AllowGet);
        }
    }
}
