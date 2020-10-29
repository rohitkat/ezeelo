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

namespace Administrator.Controllers
{
    public class FrequentlyBuyTogetherProductController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET: /FrequentlyBuyTogetherProduct/
        public ActionResult Index()
        {
            var frequentlybuytogetherproducts = db.FrequentlyBuyTogetherProducts.Include(f => f.PersonalDetail).Include(f => f.PersonalDetail1).Include(f => f.Product).Include(f => f.Product1);
            return View(frequentlybuytogetherproducts.ToList());
        }

        // GET: /FrequentlyBuyTogetherProduct/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FrequentlyBuyTogetherProduct frequentlybuytogetherproduct = db.FrequentlyBuyTogetherProducts.Find(id);
            if (frequentlybuytogetherproduct == null)
            {
                return HttpNotFound();
            }
            return View(frequentlybuytogetherproduct);
        }

        // GET: /FrequentlyBuyTogetherProduct/Create
        public ActionResult Create()
        {
            ViewBag.CategoryLevelOne = new SelectList(db.Categories.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name");
            ViewBag.CatLevelOne = new SelectList(db.Categories.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name");

            List<SelectListItem> lData = new List<SelectListItem>();
            lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
            ViewBag.CategoryLevelTwo = new SelectList(lData, "Value", "Text");
            ViewBag.CategoryLevelThree = new SelectList(lData, "Value", "Text");
            ViewBag.CatLevelTwo = new SelectList(lData, "Value", "Text");
            ViewBag.CatLevelThree = new SelectList(lData, "Value", "Text");
            ViewBag.ProductID = new SelectList(lData, "Value", "Text");

            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");


            return View();
        }

        // POST: /FrequentlyBuyTogetherProduct/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelLayer.Models.ViewModel.MyProductList pMyProductList, long hdnProdID)
        {
            // long ID = ViewBag.ProIDD;
            ViewBag.CategoryLevelOne = new SelectList(db.Categories.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name");
            ViewBag.CatLevelOne = new SelectList(db.Categories.Where(x => x.Level == 1).OrderBy(x => x.Name).ToList(), "ID", "Name");
            //long PID = collection.proIDD;
            List<SelectListItem> lData = new List<SelectListItem>();
            lData.Add(new SelectListItem { Text = string.Empty, Value = "0" });
            ViewBag.CategoryLevelTwo = new SelectList(lData, "Value", "Text");
            ViewBag.CategoryLevelThree = new SelectList(lData, "Value", "Text");
            ViewBag.CatLevelTwo = new SelectList(lData, "Value", "Text");
            ViewBag.CatLevelThree = new SelectList(lData, "Value", "Text");

            ViewBag.ProductID = new SelectList(db.Products.OrderBy(x => x.Name).ToList(), "ID", "Name");
            InsertRecordFrequentlyBuyTogetherProduct(pMyProductList, hdnProdID);

            FrequentlyBuyTogetherProduct frequentlyBuyTogetherProduct = new FrequentlyBuyTogetherProduct();


            return View(frequentlyBuyTogetherProduct);
        }

        private void InsertRecordFrequentlyBuyTogetherProduct(ModelLayer.Models.ViewModel.MyProductList pMyProductList, long hdnProdID)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {

                        if (pMyProductList.TwoLevelID > 0)
                        {
                            for (int i = 0; i < pMyProductList.pMyProductViewModel.Count(); i++)
                            {
                                if (pMyProductList.pMyProductViewModel[i].isSelected)
                                {
                                    long productID;
                                    //catID = pMyProductList.ThreeLevelID;
                                    productID = pMyProductList.pMyProductViewModel[i].ID;
                                    if (db.FrequentlyBuyTogetherProducts.Where(x => x.ThisProductID == productID).ToList().Count() < 1)
                                    {

                                        FrequentlyBuyTogetherProduct frequentlyBuyTogetherProduct = new FrequentlyBuyTogetherProduct();
                                        frequentlyBuyTogetherProduct.ThisProductID = pMyProductList.pMyProductViewModel[i].ID;
                                        frequentlyBuyTogetherProduct.WithProductID = hdnProdID;
                                        frequentlyBuyTogetherProduct.IsActive = true;
                                        frequentlyBuyTogetherProduct.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                                        frequentlyBuyTogetherProduct.CreateDate = DateTime.UtcNow.AddHours(5.30);
                                        frequentlyBuyTogetherProduct.DeviceID = string.Empty;
                                        frequentlyBuyTogetherProduct.DeviceType = string.Empty;

                                        db.FrequentlyBuyTogetherProducts.Add(frequentlyBuyTogetherProduct);
                                        db.SaveChanges();
                                    }

                                }
                            }
                        }
                        else
                        {
                            //for (int i = 0; i < lCategoryList.Count(); i++)
                            //{
                            for (int j = 0; j < pMyProductList.pMyProductViewModel.Count(); j++)
                            {
                                if (pMyProductList.pMyProductViewModel[j].isSelected)
                                {
                                    long proid;
                                    // catID = lCategoryList[i].ID;
                                    proid = pMyProductList.pMyProductViewModel[j].ID;
                                    if (db.FrequentlyBuyTogetherProducts.Where(x => x.ThisProductID == proid).ToList().Count() < 1)
                                    {
                                        FrequentlyBuyTogetherProduct frequentlyBuyTogetherProduct = new FrequentlyBuyTogetherProduct();
                                        frequentlyBuyTogetherProduct.ThisProductID = pMyProductList.pMyProductViewModel[j].ID;
                                        frequentlyBuyTogetherProduct.WithProductID = hdnProdID;
                                        frequentlyBuyTogetherProduct.IsActive = true;
                                        frequentlyBuyTogetherProduct.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                                        frequentlyBuyTogetherProduct.CreateDate = DateTime.UtcNow.AddHours(5.30);
                                        frequentlyBuyTogetherProduct.DeviceID = string.Empty;
                                        frequentlyBuyTogetherProduct.DeviceType = string.Empty;

                                        db.FrequentlyBuyTogetherProducts.Add(frequentlyBuyTogetherProduct);
                                        db.SaveChanges();
                                    }
                                }
                            }
                            // }
                        }
                        ts.Complete();
                        ViewBag.Message = "Inserted Successfully";
                    }
                    catch
                    {
                        ts.Dispose();
                        ViewBag.Message = "Problem in List of Products to Insert Successfully";
                    }
                }

            }
            catch (Exception ex)
            {
                ViewBag.Message = "Problem in List of Products to Insert Successfully";
            }
        }

        // GET: /FrequentlyBuyTogetherProduct/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FrequentlyBuyTogetherProduct frequentlybuytogetherproduct = db.FrequentlyBuyTogetherProducts.Find(id);
            if (frequentlybuytogetherproduct == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", frequentlybuytogetherproduct.CreateBy);
            ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", frequentlybuytogetherproduct.ModifyBy);
            ViewBag.ThisProductID = new SelectList(db.Products, "ID", "Name", frequentlybuytogetherproduct.ThisProductID);
            ViewBag.WithProductID = new SelectList(db.Products, "ID", "Name", frequentlybuytogetherproduct.WithProductID);
            return View(frequentlybuytogetherproduct);
        }

        // POST: /FrequentlyBuyTogetherProduct/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,WithProductID,ThisProductID,IsActive")] FrequentlyBuyTogetherProduct frequentlybuytogetherproduct)
        {
            //EzeeloDBContext db1 = new EzeeloDBContext();

            ViewBag.ThisProductID = new SelectList(db.Products, "ID", "Name", frequentlybuytogetherproduct.ThisProductID);
            ViewBag.WithProductID = new SelectList(db.Products, "ID", "Name", frequentlybuytogetherproduct.WithProductID);

            FrequentlyBuyTogetherProduct lFrequentlyBuyTogetherProduct = db.FrequentlyBuyTogetherProducts.Find(frequentlybuytogetherproduct.ID);
          //  Int64 i = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
          //  lFrequentlyBuyTogetherProduct.ID = lFrequentlyBuyTogetherProduct.ID;
            frequentlybuytogetherproduct.CreateBy = lFrequentlyBuyTogetherProduct.CreateBy;
            frequentlybuytogetherproduct.CreateDate = lFrequentlyBuyTogetherProduct.CreateDate;
            frequentlybuytogetherproduct.ModifyDate = DateTime.UtcNow.AddHours(5.30);
            frequentlybuytogetherproduct.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
            frequentlybuytogetherproduct.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
            frequentlybuytogetherproduct.DeviceType = string.Empty;
            frequentlybuytogetherproduct.DeviceID = string.Empty;
            //db1.Dispose();
            if (ModelState.IsValid)
            {
                //db.Entry(frequentlybuytogetherproduct).State = EntityState.Modified;
                //db.Entry(frequentlybuytogetherproduct).CurrentValues.SetValues(lFrequentlyBuyTogetherProduct);
                db.Entry(lFrequentlyBuyTogetherProduct).CurrentValues.SetValues(frequentlybuytogetherproduct);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(frequentlybuytogetherproduct);


          
           // ViewBag.ThisProductID = new SelectList(db.Products, "ID", "Name", frequentlybuytogetherproduct.ThisProductID);
           // ViewBag.WithProductID = new SelectList(db.Products, "ID", "Name", frequentlybuytogetherproduct.WithProductID);

           // FrequentlyBuyTogetherProduct lFrequentlyBuyTogetherProduct = db.FrequentlyBuyTogetherProducts.Find(frequentlybuytogetherproduct.ID);
           //// lFrequentlyBuyTogetherProduct.ID = frequentlybuytogetherproduct.ID;
           // lFrequentlyBuyTogetherProduct.WithProductID = frequentlybuytogetherproduct.WithProductID;
           // lFrequentlyBuyTogetherProduct.ThisProductID = frequentlybuytogetherproduct.ThisProductID;
           // lFrequentlyBuyTogetherProduct.IsActive = frequentlybuytogetherproduct.IsActive;
           // lFrequentlyBuyTogetherProduct.CreateBy = frequentlybuytogetherproduct.CreateBy;
           // lFrequentlyBuyTogetherProduct.CreateDate = frequentlybuytogetherproduct.CreateDate;
           // lFrequentlyBuyTogetherProduct.ModifyDate = DateTime.UtcNow.AddHours(5.30);
           // lFrequentlyBuyTogetherProduct.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
           // lFrequentlyBuyTogetherProduct.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
           // lFrequentlyBuyTogetherProduct.DeviceType = string.Empty;
           // lFrequentlyBuyTogetherProduct.DeviceID = string.Empty;
           // db.SaveChanges();
           // return RedirectToAction("Index");
          



        }

        // GET: /FrequentlyBuyTogetherProduct/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FrequentlyBuyTogetherProduct frequentlybuytogetherproduct = db.FrequentlyBuyTogetherProducts.Find(id);
            if (frequentlybuytogetherproduct == null)
            {
                return HttpNotFound();
            }
            return View(frequentlybuytogetherproduct);
        }

        // POST: /FrequentlyBuyTogetherProduct/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            FrequentlyBuyTogetherProduct frequentlybuytogetherproduct = db.FrequentlyBuyTogetherProducts.Find(id);
            db.FrequentlyBuyTogetherProducts.Remove(frequentlybuytogetherproduct);
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

        public ActionResult ProductList(Int32 TwoLevelID, Int32 ThreeLevelID, int id, long WithProductID)
        {

            ViewBag.ProIDD = WithProductID;
            ModelLayer.Models.ViewModel.MyProductList lMyProductList = new ModelLayer.Models.ViewModel.MyProductList();

            lMyProductList.pMyProductViewModel = (from n in db.Products
                                                  where n.CategoryID == ThreeLevelID
                                                  select new ModelLayer.Models.ViewModel.MyProductViewModel
                                                  {
                                                      Name = n.Name,
                                                      ID = n.ID,
                                                      isSelected = false
                                                  }).OrderBy(x => x.Name).ToList();



            return PartialView("_ProductList", lMyProductList);
        }

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

        public JsonResult GetProduct(int cateID)
        {
            List<CategoryList> lcategory = new List<CategoryList>();
            //lcategory = db.Categories.Where(x => x.ParentCategoryID == cateID && x.Level == 2).ToList();
            lcategory = (from n in db.Products
                         where n.CategoryID == cateID
                         select new CategoryList
                         {
                             ID = n.ID,
                             name = n.Name
                         }).OrderBy(x => x.name).ToList();
            return Json(lcategory, JsonRequestBehavior.AllowGet);

        }

        public class CategoryList
        {
            public long ID { get; set; }
            public string name { get; set; }
        }
    }
}
