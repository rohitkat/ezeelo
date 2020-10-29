using Administrator.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Administrator.Controllers
{
    public class ProductTaxController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();

        [SessionExpire]
        [CustomAuthorize(Roles = "ProductTax/CanRead")]
        public ActionResult Index()
        {
            ProductTaxViewModel ls = new ProductTaxViewModel();
            List<DropdownList> dropdown = new List<DropdownList>();
            ViewBag.Franchise = new SelectList((from f in db.Franchises where f.ID != 1 
                                                select new  DropdownList
                                                {
                                                    ID = f.ID,
                                                    Name= f.BusinessDetail.Name }).ToList(),"ID","Name");

            dropdown.Add(new DropdownList{ID=0, Name=""});

            ViewBag.Shop = new SelectList(dropdown,"ID","Name","Select Shop");
            ViewBag.ShopCategory = new SelectList(dropdown,"ID","Name","Select Category");
            ViewBag.TaxMaster = new SelectList(db.TaxationMasters.ToList(), "ID", "Name", "Select Category");
            return View(ls);
        
        }
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductTax/CanRead")]
        public ActionResult TaxIndex(long Shop, long ShopCategory, int TaxMasterID)
        {

            ProductTaxIndexView ls = new ProductTaxIndexView();
            TaxationManagement obj = new TaxationManagement(fConnectionString);
            ls.ProductVarientByCategoryAndShop = obj.ProductTaxIndex(ShopCategory, Shop, TaxMasterID, false);
            
            ls.TaxMasterID = TaxMasterID;
            return PartialView("_TaxIndex", ls);
        }

        public ActionResult ProductTaxCreateIndex(long Shop, long ShopCategory, int TaxMasterID, int ShowOnlyBranded)
        {

            ProductTaxIndexView ls = new ProductTaxIndexView();
            TaxationManagement obj = new TaxationManagement(fConnectionString);
            ls.ProductVarientByCategoryAndShop = obj.ProductVarientByCategoryAndShop(ShopCategory, Shop, TaxMasterID, false, ShowOnlyBranded);
            ls.TaxMasterID = TaxMasterID;
            return PartialView("_ProductTaxCreateIndex", ls);
        }

        // GET: /ProductTax/Details/5
       [SessionExpire]
        [CustomAuthorize(Roles = "ProductTax/CanRead")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductTax productTax = db.ProductTaxes.Find(id);
            if (productTax == null)
            {
                return HttpNotFound();
            }

            ProductTaxDetail sProductTaxDetail = (from n in db.ProductTaxes
                                                  join ss in db.ShopStocks on n.ShopStockID equals ss.ID
                                                  join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                                                  join s in db.Shops on sp.ShopID equals s.ID
                                                  join f in db.Franchises on s.FranchiseID equals f.ID
                                                  join p in db.Products on sp.ProductID equals p.ID
                                                  join t in db.TaxationMasters on n.TaxID equals t.ID
                                                  where n.ID == id
                                                  select new ProductTaxDetail
                                                  {
                                                      ID =n.ID,
                                                      FranchiseID = s.FranchiseID,
                                                      FranmchiseName = f.ContactPerson,
                                                      shopID = s.ID,
                                                      ShopName = s.Name,
                                                      ProductName = p.Name,
                                                      ShopstockID = ss.ID,
                                                      TaxID = t.ID,
                                                      TaxName =t.Name,
                                                      IsActive= n.IsActive

                                                  }).FirstOrDefault();

            return View(sProductTaxDetail);
        }

        // GET: /ProductTax/Create
      
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductTax/CanWrite")]
        public ActionResult Create()
        {
            ProductTaxViewModel ls = new ProductTaxViewModel();
            List<DropdownList> dropdown = new List<DropdownList>();
            ViewBag.Franchise = new SelectList((from f in db.Franchises
                                                where f.ID != 1
                                                select new DropdownList
                                                {
                                                    ID = f.ID,
                                                    Name = f.BusinessDetail.Name
                                                }).ToList(), "ID", "Name");

            dropdown.Add(new DropdownList { ID = 0, Name = "" });

            ViewBag.Shop = new SelectList(dropdown, "ID", "Name", "Select Shop");
            ViewBag.ShopCategory = new SelectList(dropdown, "ID", "Name", "Select Category");
            ViewBag.TaxMaster = new SelectList(db.TaxationMasters.ToList(), "ID", "Name", "Select Category");
            return View();
        }

        // POST: /ProductTax/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductTax/CanWrite")]
        public ActionResult Create(ModelLayer.Models.ViewModel.ProductTaxIndexView pProductTax, int? TaxMasterID)
        {
            List<TempShopStockAndTax> temp = new List<TempShopStockAndTax>();

            temp = (from n in pProductTax.ProductVarientByCategoryAndShop
                    where n.isSelected == true
                    select new TempShopStockAndTax
                    {
                         ShopStockID = n.ShopStockID,
                         TaxID = Convert.ToInt32(TaxMasterID),
                         
                    }
                   ).ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add("ShopStockID");
            dt.Columns.Add("TaxID");


            foreach (TempShopStockAndTax pa in temp)
            {
                DataRow dr = dt.NewRow();
                dr["ShopStockID"] = pa.ShopStockID;
                dr["TaxID"] = pa.TaxID;                
                dt.Rows.Add(dr);
            }
            ProductTaxViewModel ls = new ProductTaxViewModel();
            List<DropdownList> dropdown = new List<DropdownList>();
            ViewBag.Franchise = new SelectList((from f in db.Franchises
                                                where f.ID != 1
                                                select new DropdownList
                                                {
                                                    ID = f.ID,
                                                    Name = f.BusinessDetail.Name
                                                }).ToList(), "ID", "Name");

            dropdown.Add(new DropdownList { ID = 0, Name = "" });

            ViewBag.Shop = new SelectList(dropdown, "ID", "Name", "Select Shop");
            ViewBag.ShopCategory = new SelectList(dropdown, "ID", "Name", "Select Category");
            ViewBag.TaxMaster = new SelectList(db.TaxationMasters.ToList(), "ID", "Name", "Select Category");


            BusinessLogicLayer.TaxationManagement obj = new TaxationManagement(fConnectionString);
            string msg = obj.InsertUpdate_ProductTax(0,Convert.ToInt32(TaxMasterID),true, DateTime.UtcNow.AddHours(5.3), 1, DateTime.UtcNow.AddHours(5.3), 1, "test", "Web", "Not Known", dt, "INSERT");
            return View(pProductTax);
        }


        // GET: /ProductTax/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductTax/CanWrite")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductTax sProductTax = db.ProductTaxes.Find(id);
            if (sProductTax == null)
            {
                return HttpNotFound();
            }
            ProductTaxDetail sProductTaxDetail = (from n in db.ProductTaxes
                                                  join ss in db.ShopStocks on n.ShopStockID equals ss.ID
                                                  join sp in db.ShopProducts on ss.ShopProductID equals sp.ID
                                                  join s in db.Shops on sp.ShopID equals s.ID
                                                  join f in db.Franchises on s.FranchiseID equals f.ID
                                                  join p in db.Products on sp.ProductID equals p.ID
                                                  join t in db.TaxationMasters on n.TaxID equals t.ID
                                                  where n.ID == id
                                                  select new ProductTaxDetail
                                                  {
                                                      ID = n.ID,
                                                      FranchiseID = s.FranchiseID,
                                                      FranmchiseName = f.ContactPerson,
                                                      shopID = s.ID,
                                                      ShopName = s.Name,
                                                      ProductName = p.Name,
                                                      ShopstockID = ss.ID,
                                                      TaxID = t.ID,
                                                      TaxName = t.Name,
                                                      IsActive = n.IsActive

                                                  }).FirstOrDefault();
            return View(sProductTaxDetail);
        }

        // POST: /ProductTax/Edit/5
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductTax/CanWrite")]
        public ActionResult Edit(ModelLayer.Models.ViewModel.ProductTaxDetail sProductTax)
        {

            ProductTax lProductTax = new ProductTax();
            lProductTax = db.ProductTaxes.Find(sProductTax.ID);

            lProductTax.IsActive = sProductTax.IsActive;

            if (ModelState.IsValid)
            {
                //db.Entry(sProductTax).CurrentValues.SetValues(lProductTax);
                db.Entry(lProductTax).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lProductTax);
        }

        // GET: /ProductTax/Delete/5    
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductTax/CanDelete")]
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

        // POST: /ProductTax/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductTax/CanDelete")]
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

        public JsonResult FranchiseShop(int franchiseID)
        {
            List<DropdownList> shopddl = new List<DropdownList>();

            shopddl = (from n in db.Shops
                       where n.FranchiseID == franchiseID
                       select new DropdownList
                       {
                           ID = n.ID,
                           Name = n.Name
                       }).ToList();
           return Json(shopddl, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ShopCategoryList(Int64 ShopID)
        {
            List<ShopList> ls = new List<ShopList>();

            ls = (from op in db.OwnerPlans
                  join p in db.Plans on op.PlanID equals p.ID
                  join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                  join c in db.Categories on pcc.CategoryID equals c.ID
                  where pcc.IsActive == true && op.IsActive == true && c.IsActive == true
                  && c.Level == 3 && p.IsActive == true && op.OwnerID == ShopID
                  && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                  select new ShopList { Name = c.Name, ID = c.ID }).ToList();

            return Json(ls.ToList().OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
        }
	}
}