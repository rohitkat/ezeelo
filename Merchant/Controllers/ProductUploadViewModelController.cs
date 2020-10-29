using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Net;
using System.Data.Entity;

namespace Merchant.Controllers
{
    public class ProductUploadViewModelController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        // GET: /ProductUploadViewModel/
        public ActionResult Index()
        {
            //List<ProductUploadViewModel> lProduct = new List<ProductUploadViewModel>();
            //return View(lProduct.ToList());

            var ProductUpload = from TP in db.TempProducts 
                       join TPV in db.TempProductVarients on TP.ID equals TPV.ProductID
                       join TSP in db.TempShopProducts on TP.ID equals TSP.ProductID
                       join TSS in db.TempShopStocks on TPV.ID equals TSS.ProductVarientID 
                     //---------------------------------------
                       join CTG in db.Categories  on TP.CategoryID equals CTG.ID 
                       join BRD in db.Brands on TP.BrandID equals BRD.ID 

                      
                    //----------------------------------------
                       select new ProductUploadViewModel
                       {
                           CategoryName=CTG.Name,
                           BrandName=BRD.Name,

                           ProductID=TP.ID,
                           ProductName = TP.Name,
                           CategoryID=TP.CategoryID,
                           //WeightInGram=TP.WeightInGram,
                           //LengthInCm=TP.LengthInCm,
                           //BreadthInCm=TP.BreadthInCm,
                           //HeightInCm=TP.HeightInCm,
                           //Description=TP.Description,
                           BrandID=TP.BrandID,
                           //SearchKeyword=TP.SearchKeyword,
                           //IsActive=TP.IsActive,

                           //ColorName=TPV.ColorName,
                           //SizeName=TPV.SizeName,
                           //DimensionName=TPV.DimensionName,
                           //MaterialName=TPV.MaterialName,

                           //DisplayProductFromDate=TSP.DisplayProductFromDate,

                           Qty=TSS.Qty,
                           //ReorderLevel=TSS.ReorderLevel,
                           PackSize=TSS.PackSize,
                           //PackUnitID=TSS.PackUnitID,
                           //MRP=TSS.MRP,
                           //WholeSaleRate=TSS.WholeSaleRate,
                           //RetailerRate=TSS.RetailerRate,
                           //IsInclusiveOfTax=TSS.IsInclusiveOfTax,
                       };
            return View(ProductUpload);
        }

        [HttpPost]
        //public ActionResult Index(ProductUpload pfile)
        //{
        //    ImageUpload img1 = new ImageUpload(System.Web.HttpContext.Current.Server);
        //        foreach (var file in pfile.Files)
        //        {
        //            if (file.ContentLength > 0)
        //            {
        //                img1.AddProductImage("ABC", "56", "DEFAULT", string.Empty, file, ProductUpload.IMAGE_TYPE.NONAPPROVED);
        //            }
        //        }

        //   // saveImages.AddShopLogo("1001", imgUpload, ImageEnums.IMAGE_TYPE.NONAPPROVED);

        //    return RedirectToAction("Index");
        //}

        //
        // GET: /ProductUploadViewModel/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /ProductUploadViewModel/Create
        public ActionResult Create()
        {
            ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name");
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            ViewBag.ColorID = new SelectList(db.Colors, "ID", "Name");
            ViewBag.SizeID = new SelectList(db.Sizes, "ID", "Name");
            ViewBag.DimensionID = new SelectList(db.Dimensions, "ID", "Name");
            ViewBag.MaterialID = new SelectList(db.Materials, "ID", "Name");
            ViewBag.PackUnitID = new SelectList(db.Units, "ID", "Name");
            return View();

        }

        //-------------------------------------
        //public ActionResult Create([Bind(Include = "ID,Name,CategoryID,WeightInGram,LengthInCm,BreadthInCm,HeightInCm,Description,BrandID,SearchKeyword,IsActive")] Product product)
        //{
        //    //  CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID
        //    product.CreateDate = DateTime.Now;
        //    product.CreateBy = 1;

        //    if (ModelState.IsValid)
        //    {
        //        db.Products.Add(product);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name", product.BrandID);
        //    ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", product.CategoryID);
        //    return View(product);
        //}

        //
        // POST: /ProductUploadViewModel/Create
        [HttpPost]
        public ActionResult Create(ProductUploadViewModel collection)
        {
            try
            {
                //--------------------
                // public ActionResult Create(ProductUploadViewModel collection,ProductUpload pfile)
                //------------------------------------
                //ImageUpload img1 = new ImageUpload(System.Web.HttpContext.Current.Server);
                //foreach (var file in pfile.Files)
                //{
                //    if (file.ContentLength > 0)
                //    {
                //        img1.AddProductImage("ABC", "56", "DEFAULT", string.Empty, file, ProductUpload.IMAGE_TYPE.NONAPPROVED);
                //    }
                //}
                 //TODO: Add insert logic here

                TempProduct tempProduct=new TempProduct();
                TempProductVarient tempProductVarient = new TempProductVarient();
                TempShopStock tempShopStock = new TempShopStock();
                TempShopProduct tempShopProduct = new TempShopProduct();

                tempProduct.Name = collection.ProductName;
                tempProduct.CategoryID =collection.CategoryID ;
                tempProduct.WeightInGram =collection.WeightInGram;
                tempProduct.LengthInCm=collection.LengthInCm ;
                tempProduct.BreadthInCm=collection.BreadthInCm;
                tempProduct.HeightInCm = collection.HeightInCm;
                tempProduct.Description = collection.Description;
                tempProduct.BrandID = collection.BrandID;
                tempProduct.SearchKeyword = collection.SearchKeyword;
                tempProduct.IsActive = collection.IsActive; 
                tempProduct.CreateDate = DateTime.Now;
                tempProduct.CreateBy = 1;
                //tempProduct.ModifyDate = Convert.ToDateTime(collection["ModifyDate"]);
                //tempProduct.ModifyBy = Convert.ToInt32(collection["ModifyBy"]);
                //tempProduct.NetworkIP = collection["NetworkIP"];
                //tempProduct.DeviceType = collection["DeviceType"];
                //tempProduct.DeviceID = collection["DeviceID"];

                if (ModelState.IsValid)
                {
                    db.TempProducts.Add(tempProduct);
                    db.SaveChanges();  
                }

                //tempProductVarient.ProductID = db.TempProducts.ID;
                Int64 id = tempProduct.ID;

                tempProductVarient.ProductID = id;
                tempProductVarient.ColorID = collection.ColorID;
                tempProductVarient.SizeID = collection.SizeID;
                tempProductVarient.DimensionID = collection.DimensionID;
                tempProductVarient.MaterialID = collection.MaterialID;

                tempProductVarient.IsActive = collection.IsActive ;
                tempProductVarient.CreateDate = DateTime.Now;
                tempProductVarient.CreateBy = 1;

                if (ModelState.IsValid)
                {
                    db.TempProductVarients.Add(tempProductVarient);
                    db.SaveChanges();
                }
                Int64 id1 = tempProductVarient.ID;

                 tempShopProduct.ShopID = 5;
                 tempShopProduct.ProductID = id;
                 tempShopProduct.IsActive = collection.IsActive;
                 tempShopProduct.DisplayProductFromDate = collection.DisplayProductFromDate;
                 tempShopProduct.CreateDate = DateTime.Now;
                 tempShopProduct.CreateBy = 1;

                 if (ModelState.IsValid)
                 {
                     db.TempShopProducts.Add(tempShopProduct);
                     db.SaveChanges();
                 }

                
                 Int64 id3 = tempShopProduct.ID;

                tempShopStock.ShopProductID = id3;
                tempShopStock.ProductVarientID = id1;
                tempShopStock.Qty = collection.Qty;
                tempShopStock.ReorderLevel = collection.ReorderLevel;
                tempShopStock.StockStatus = collection.StockStatus;
                tempShopStock.PackSize = collection.PackSize;
                tempShopStock.PackUnitID = collection.PackUnitID;
                tempShopStock.MRP = collection.MRP;
                tempShopStock.WholeSaleRate = collection.WholeSaleRate;
                tempShopStock.RetailerRate = collection.RetailerRate;
                tempShopStock.IsInclusiveOfTax = collection.IsInclusiveOfTax;

                tempShopStock.IsActive = collection.IsActive;
                tempShopStock.CreateDate = DateTime.Now;
                tempShopStock.CreateBy = 1;

                if (ModelState.IsValid)
                {
                    db.TempShopStocks.Add(tempShopStock);
                    db.SaveChanges();
                }
               // return View(collection);
              
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw;
            }

            //try
            //{
                // TODO: Add insert logic here

            //    TempProduct tempProduct=new TempProduct();
            //    TempProductVarient tempProductVarient = new TempProductVarient();
            //    TempShopStock tempShopStock = new TempShopStock();

            //    tempProduct.Name=collection["ProductName"];
            //    tempProduct.CategoryID = Convert.ToInt32 (collection["CategoryID"]);
            //    tempProduct.WeightInGram = Convert.ToInt32(collection["WeightInGram"]);
            //    tempProduct.LengthInCm=Convert.ToInt32(collection["LengthInCm"]);
            //    tempProduct.BreadthInCm=Convert.ToInt32(collection["BreadthInCm"]);
            //    tempProduct.HeightInCm=Convert.ToInt32(collection["HeightInCm"]);
            //    tempProduct.Description = collection["Description"];
            //    tempProduct.BrandID = Convert.ToInt32(collection["BrandID"]);
            //    tempProduct.SearchKeyword =collection["SearchKeyword"];
            //    tempProduct.IsActive =Convert.ToBoolean(collection["IsActive"]);
            //    tempProduct.CreateDate = DateTime.Now;
            //    tempProduct.CreateBy = 1;
            //    //tempProduct.ModifyDate = Convert.ToDateTime(collection["ModifyDate"]);
            //    //tempProduct.ModifyBy = Convert.ToInt32(collection["ModifyBy"]);
            //    //tempProduct.NetworkIP = collection["NetworkIP"];
            //    //tempProduct.DeviceType = collection["DeviceType"];
            //    //tempProduct.DeviceID = collection["DeviceID"];

            //    if (ModelState.IsValid)
            //    {
            //        db.TempProducts.Add(tempProduct);
            //        db.SaveChanges();  
            //    }

            //    tempProductVarient.ColorName = collection["ColorName"];
            //    tempProductVarient.SizeName = collection["SizeName"];
            //    tempProductVarient.DimensionName = collection["DimensionName"];
            //    tempProductVarient.MaterialName = collection["MaterialName"];

            //    if (ModelState.IsValid)
            //    {
            //        db.TempProductVarients.Add(tempProductVarient);
            //        db.SaveChanges();
            //    }

            //    tempShopStock.Qty = Convert.ToInt32(collection["Qty"]);
            //    tempShopStock.ReorderLevel = Convert.ToInt32(collection["ReorderLevel"]);
            //    tempShopStock.StockStatus = Convert.ToBoolean(collection["StockStatus"]);
            //    tempShopStock.PackSize = Convert.ToDecimal(collection["PackSize"]);
            //    tempShopStock.PackUnitID = Convert.ToInt32(collection["PackUnitID"]);
            //    tempShopStock.MRP = Convert.ToDecimal(collection["MRP"]);
            //    tempShopStock.WholeSaleRate = Convert.ToDecimal(collection["WholeSaleRate"]);
            //    tempShopStock.RetailerRate = Convert.ToDecimal(collection["RetailerRate"]);
            //    tempShopStock.IsInclusiveOfTax = Convert.ToBoolean(collection["IsInclusiveOfTax"]);

            //    if (ModelState.IsValid)
            //    {
            //        db.TempShopStocks.Add(tempShopStock);
            //        db.SaveChanges();
            //    }

            //    return RedirectToAction("Index");
            //}
            //catch
            //{
            //    return View();
            //}

                //return View();
        }

        //
        // GET: /ProductUploadViewModel/Edit/5
        public ActionResult Edit(int? id, ProductUploadViewModel collection)
         {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var ProductUpload = (from TP in db.TempProducts
                                join TPV in db.TempProductVarients on TP.ID equals TPV.ProductID
                                join TSP in db.TempShopProducts on TP.ID equals TSP.ProductID
                                join TSS in db.TempShopStocks on TPV.ID equals TSS.ProductVarientID
                                //---------------------------------------
                                join CTG in db.Categories on TP.CategoryID equals CTG.ID
                                join BRD in db.Brands on TP.BrandID equals BRD.ID
                                where TP.ID==id

                                //----------------------------------------
                                select new ProductUploadViewModel
                                {
                                    CategoryName = CTG.Name,
                                    BrandName = BRD.Name,


                                    ProductName = TP.Name,
                                    CategoryID = TP.CategoryID,
                                    WeightInGram = TP.WeightInGram,
                                    LengthInCm = TP.LengthInCm,
                                    BreadthInCm = TP.BreadthInCm,
                                    HeightInCm = TP.HeightInCm,
                                    Description = TP.Description,
                                    BrandID = TP.BrandID,
                                    SearchKeyword = TP.SearchKeyword,
                                    IsActive = TP.IsActive,

                                    ColorID = TPV.ColorID,
                                    SizeID = TPV.SizeID,
                                    DimensionID = TPV.DimensionID,
                                    MaterialID = TPV.MaterialID,

                                    DisplayProductFromDate = TSP.DisplayProductFromDate,

                                    Qty = TSS.Qty,
                                    ReorderLevel = TSS.ReorderLevel,
                                    PackSize = TSS.PackSize,
                                    PackUnitID = TSS.PackUnitID,
                                    MRP = TSS.MRP,
                                    WholeSaleRate = TSS.WholeSaleRate,
                                    RetailerRate = TSS.RetailerRate,
                                    IsInclusiveOfTax = TSS.IsInclusiveOfTax,

                                    ProductVarientID=TPV.ID,
                                    ShopProductID=TSP.ID,
                                    ShopStockID=TSS.ID,

                                }).ToList();
            foreach (ProductUploadViewModel PUVM in ProductUpload)
            {
                collection.BrandID = PUVM.BrandID;
            }

            if (ProductUpload.Count() == null)
            {
                return HttpNotFound();
            }


            ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name", collection.BrandID );
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", collection.CategoryID);
            ViewBag.ColorID = new SelectList(db.Colors, "ID", "Name", collection.ColorID);
            ViewBag.SizeID = new SelectList(db.Sizes, "ID", "Name", collection.SizeID);
            ViewBag.DimensionID = new SelectList(db.Dimensions, "ID", "Name", collection.DimensionID);
            ViewBag.MaterialID = new SelectList(db.Materials, "ID", "Name", collection.MaterialID);
            ViewBag.PackUnitID = new SelectList(db.Units, "ID", "Name", collection.PackUnitID);

            return View(collection);
            
        }

        //
        // POST: /ProductUploadViewModel/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, ProductUploadViewModel collection)
        {
            try
            {
                //EzeeloDBContext db1 = new EzeeloDBContext();
                //TempProduct lTempProduct = db.TempProducts.Find(collection.ProductID);

                TempProduct tempProduct = db.TempProducts.Find(id);
               

                tempProduct.Name = collection.ProductName;
                tempProduct.CategoryID = collection.CategoryID;
                tempProduct.WeightInGram = collection.WeightInGram;
                tempProduct.LengthInCm = collection.LengthInCm;
                tempProduct.BreadthInCm = collection.BreadthInCm;
                tempProduct.HeightInCm = collection.HeightInCm;
                tempProduct.Description = collection.Description;
                tempProduct.BrandID = collection.BrandID;
                tempProduct.SearchKeyword = collection.SearchKeyword;
                tempProduct.IsActive = collection.IsActive;
                tempProduct.CreateDate = tempProduct.CreateDate;
                tempProduct.CreateBy = tempProduct.CreateBy ;
                tempProduct.ModifyDate = DateTime.Now;
                tempProduct.ModifyBy = 1;
                //tempProduct.ModifyDate = Convert.ToDateTime(collection["ModifyDate"]);
                //tempProduct.ModifyBy = Convert.ToInt32(collection["ModifyBy"]);
                //tempProduct.NetworkIP = collection["NetworkIP"];
                //tempProduct.DeviceType = collection["DeviceType"];
                //tempProduct.DeviceID = collection["DeviceID"];
               // db1.Dispose();
                TryUpdateModel(tempProduct);
                if (ModelState.IsValid)
                {
                    //db.Entry().CurrentValues.
                    db.Entry(tempProduct).CurrentValues.SetValues(tempProduct);
                    // db.Entry(tempProduct).State = EntityState.Modified;
                    //db.TempProducts.Add(tempProduct);
                    db.SaveChanges();
                }

                //tempProductVarient.ProductID = db.TempProducts.ID;

                Int64 id4 = tempProduct.ID;

                //EzeeloDBContext db2 = new EzeeloDBContext();
                TempProductVarient tempProductVarient = db.TempProductVarients.Where(x => x.ProductID == id).First();
                //collection.ProductVarientID = db.TempProductVarients.Where(x=>x.ID==id).Select(x=>x.ID).First();

                tempProductVarient.ProductID = id4;
                tempProductVarient.ColorID = collection.ColorID;
                tempProductVarient.SizeID = collection.SizeID;
                tempProductVarient.DimensionID = collection.DimensionID;
                tempProductVarient.MaterialID = collection.MaterialID;

                tempProductVarient.IsActive = collection.IsActive;
                tempProductVarient.CreateDate = tempProductVarient.CreateDate;
                tempProductVarient.CreateBy = tempProductVarient.CreateBy;
                tempProductVarient.ModifyDate = DateTime.Now;
                tempProductVarient.ModifyBy = 1;
               // db2.Dispose();
                if (ModelState.IsValid)
                {
                   // db.Entry(tempProductVarient).State = EntityState.Modified;
                    db.Entry(tempProductVarient).CurrentValues.SetValues(tempProductVarient);
                    db.SaveChanges();
                }

                Int64 id1 = tempProductVarient.ID;

               // EzeeloDBContext db3 = new EzeeloDBContext();
                //TempShopProduct tempShopProduct = db.TempShopProducts.Find(collection.ShopProductID);
                TempShopProduct tempShopProduct = db.TempShopProducts.Where(x => x.ProductID == id).First();

                tempShopProduct.ShopID = 5;
                tempShopProduct.ProductID = id;
                tempShopProduct.IsActive = collection.IsActive;
                tempShopProduct.DisplayProductFromDate = collection.DisplayProductFromDate;
                tempShopProduct.CreateDate = tempShopProduct.CreateDate;
                tempShopProduct.CreateBy = tempShopProduct.CreateBy;
                tempShopProduct.ModifyDate = DateTime.Now;
                tempShopProduct.ModifyBy = 1;
                //db3.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(tempShopProduct).CurrentValues.SetValues(tempShopProduct);
                   // db.Entry(tempShopProduct).State = EntityState.Modified;
                    db.SaveChanges();
                }

                Int64 id3 = tempShopProduct.ID;

               // EzeeloDBContext db4 = new EzeeloDBContext();
                //TempShopStock tempShopStock = db.TempShopStocks.Find(collection.ShopStockID);
                TempShopStock tempShopStock = db.TempShopStocks.Where(x => x.ProductVarientID == id1).First();

                tempShopStock.ShopProductID = id3;
                tempShopStock.ProductVarientID = id1;
                tempShopStock.Qty = collection.Qty;
                tempShopStock.ReorderLevel = collection.ReorderLevel;
                tempShopStock.StockStatus = collection.StockStatus;
                tempShopStock.PackSize = collection.PackSize;
                tempShopStock.PackUnitID = collection.PackUnitID;
                tempShopStock.MRP = collection.MRP;
                tempShopStock.WholeSaleRate = collection.WholeSaleRate;
                tempShopStock.RetailerRate = collection.RetailerRate;
                tempShopStock.IsInclusiveOfTax = collection.IsInclusiveOfTax;

                tempShopStock.IsActive = collection.IsActive;
                tempShopStock.CreateDate = tempShopStock.CreateDate;
                tempShopStock.CreateBy = tempShopStock.CreateBy;
                tempShopStock.ModifyDate = DateTime.Now;
                tempShopStock.ModifyBy = 1;
                //db4.Dispose();
                if (ModelState.IsValid)
                {
                    db.Entry(tempShopStock).CurrentValues.SetValues(tempShopStock);
                   // db.Entry(tempShopStock).State = EntityState.Modified;
                    db.SaveChanges();
                }
                // return View(collection);



                ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name", collection.BrandID);
                ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", collection.CategoryID);
                ViewBag.ColorID = new SelectList(db.Colors, "ID", "Name", collection.ColorID);
                ViewBag.SizeID = new SelectList(db.Sizes, "ID", "Name", collection.SizeID);
                ViewBag.DimensionID = new SelectList(db.Dimensions, "ID", "Name", collection.DimensionID);
                ViewBag.MaterialID = new SelectList(db.Materials, "ID", "Name", collection.MaterialID);
                ViewBag.PackUnitID = new SelectList(db.Units, "ID", "Name", collection.PackUnitID);
                return RedirectToAction("Index");

               
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ProductUploadViewModel/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ProductUploadViewModel/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Search(string SearchItem)
        {

            List<TempProduct> product = (List<TempProduct>)db.TempProducts .Where(u => u.Name.Contains(SearchItem)).ToList();

            //return View("Index", employees);
            return RedirectToAction("Index", product);
        }

        public System.Data.Entity.Infrastructure.DbPropertyValues chargestage { get; set; }
    }
}
