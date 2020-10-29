using Administrator.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Administrator.Controllers
{
    public class HomeProductGalleryController : Controller
    {
        //
        // GET: /HomeProductGallery/
        EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /HomeProductGallery/
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
         Environment.NewLine
         + "ErrorLog Controller : HomeProductGalleryController" + Environment.NewLine);

        #region Index
        //
        // GET: /HomeProductGallery/
        [SessionExpire]
        [CustomAuthorize(Roles = "HomeProductGallery/CanRead")]
        public ActionResult Index()
        {
            var franchiseList = db.Franchises.Where(x => x.IsActive == true && x.ID != 1).ToList().OrderBy(x => x.BusinessDetail.Name);
            return View(franchiseList);
        }

        #endregion

        #region Details

        [SessionExpire]
        [CustomAuthorize(Roles = "HomeProductGallery/CanRead")]
        public ActionResult Details(int id)  // it is franchise id
        {
            var biList = db.BlockItemsLists.Where(x => x.FranchiseID == id && x.DesignBlockType.Name.Trim().ToLower() == "product gallery").ToList();
            return View(biList.OrderBy(x=>x.SequenceOrder));
        }

        #endregion

        #region Add More Product

        [SessionExpire]
        [CustomAuthorize(Roles = "HomeProductGallery/CanRead")]
        public ActionResult AddMoreProduct(int id)
        {
            List<CategoryDetail> ldata = new List<CategoryDetail>();
            ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
            ViewBag.FranchiseList = id;
            List<CategoryDetail> catList = new List<CategoryDetail>();
            catList = (from c in db.Categories
                       join pbc in db.PlanCategoryCharges on c.ID equals pbc.CategoryID
                       join p in db.Plans on pbc.PlanID equals p.ID
                       join op in db.OwnerPlans on p.ID equals op.PlanID
                       where op.OwnerID == id && p.PlanCode.Substring(0, 4) == "GBFR"
                       && op.IsActive == true && pbc.IsActive == true && c.IsActive == true
                       select new CategoryDetail
                       {
                           ID = c.ID,
                           Name = c.Name
                       }).ToList();
            ViewBag.CategoryList = new SelectList(catList.OrderBy(x => x.Name), "ID", "Name");
            ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");
            ViewBag.LevelThreeCategoryList = new SelectList(ldata, "ID", "Name");
            return View();
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "HomeProductGallery/CanWrite")]
        public ActionResult AddMoreProduct(string ProductList, int FranchiseList, string SDate, string EDate)
        {
            try
            {
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.FranchiseList = FranchiseList;
                List<CategoryDetail> catList = new List<CategoryDetail>();
                catList = (from c in db.Categories
                           join pbc in db.PlanBindCategories on c.ID equals pbc.CategoryID
                           join pb in db.PlanBinds on pbc.PlanBindID equals pb.ID
                           join p in db.Plans on pb.PlanID equals p.ID
                           join op in db.OwnerPlans on p.ID equals op.PlanID
                           where op.OwnerID == FranchiseList && p.PlanCode.Substring(0, 4) == "GBFR"
                           && op.IsActive == true && pbc.IsActive == true && c.IsActive == true
                           select new CategoryDetail
                           {
                               ID = c.ID,
                               Name = c.Name
                           }).ToList();
                ViewBag.CategoryList = new SelectList(catList.OrderBy(x => x.Name), "ID", "Name");
                ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");
                ViewBag.LevelThreeCategoryList = new SelectList(ldata, "ID", "Name");

                string[] strProducts;
                strProducts = ProductList.Split(',');

                DataTable lDataTable = new DataTable();
                lDataTable.Columns.Add("ProductID");

                foreach (string val in strProducts)
                {
                    int v = Convert.ToInt32(val);
                    DataRow dr = lDataTable.NewRow();
                    dr["ProductID"] = v;
                    lDataTable.Rows.Add(dr);
                }

                DateTime lSDate = CommonFunctions.GetProperDate(SDate);
                DateTime lEDate = CommonFunctions.GetProperDate(EDate);

                List<object> paramValues = new List<object>();
                paramValues.Add(FranchiseList);
                paramValues.Add(db.DesignBlockTypes.Where(x => x.Name.Trim().ToLower() == "product gallery").FirstOrDefault().ID);
                paramValues.Add(lSDate);
                paramValues.Add(lEDate);
                paramValues.Add(DBNull.Value);
                paramValues.Add("-");
                paramValues.Add("-");
                paramValues.Add(lDataTable);
                paramValues.Add(1);
                paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                paramValues.Add("Net Browser");
                paramValues.Add("x");
                paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                paramValues.Add(DBNull.Value);
                paramValues.Add(DBNull.Value);
                                
                BusinessLogicLayer.HomeProductGallery.InsertGalleryProducts(paramValues, System.Web.HttpContext.Current.Server);

                ModelState.Clear();
                // ModelState.AddModelError("Message", "Done! Product Added Successfully!!");
                ViewBag.Message = "Done! Product Added Successfully!!";
            }

            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Plan Bind registration!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[HomeProductGallery][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                var catList = db.Categories.Where(c => c.Level == 0);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Plan Bind registration!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[HomeProductGallery][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                var catList = db.Categories.Where(c => c.Level == 0);
                return View();
            }


            return View();

        }

        #endregion

        #region Delete
        //
        // GET: /HomeProductGallery/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "HomeProductGallery/CanDelete")]
        public ActionResult Delete(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BlockItemsList lBlockItemsList = db.BlockItemsLists.Find(id);
                if (lBlockItemsList == null)
                {
                    return HttpNotFound();
                }

                return View(lBlockItemsList);
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

        //
        // POST: /HomeProductGallery/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "HomeProductGallery/CanDelete")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            BlockItemsList lBlockItemsList = db.BlockItemsLists.Find(id);
            try
            {
                List<object> paramValues = new List<object>();
                paramValues.Add(id);
                paramValues.Add(DBNull.Value);

                BusinessLogicLayer.HomeProductGallery.Delete_GalleryProduct(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.DELETE, System.Web.HttpContext.Current.Server);
                
                TempData["Message"] = "Done! Product Deleted Successfully!!";
                return RedirectToAction("Details", new { id = lBlockItemsList.FranchiseID });               
                    
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
                ViewBag.Messaage = "Unable to Delete Bank Detail :- " + ex.InnerException.ToString();
                
            }
            return View(lBlockItemsList);
        }

        #endregion

        #region Edit
        //
        // GET: /HomeProductGallery/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "HomeProductGallery/CanRead")]
        public ActionResult Edit(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BlockItemsList lBlockItemsList = db.BlockItemsLists.Find(id);
                ViewBag.SDate = lBlockItemsList.StartDate.ToString("dd/MM/yyyy");
                ViewBag.EDate = lBlockItemsList.EndDate.ToString("dd/MM/yyyy");

                if (lBlockItemsList == null)
                {
                    return HttpNotFound();
                }

                return View(lBlockItemsList);
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

        //
        // POST: /HomeProductGallery/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "HomeProductGallery/CanWrite")]
        [HttpPost]
        public ActionResult Edit(int id, string SDate, string EDate, bool IsActive)
        {
            try
            {
                DateTime lSDate = CommonFunctions.GetProperDateTime(SDate);
                DateTime lEDate = CommonFunctions.GetProperDateTime(EDate);

                BlockItemsList lBlockItemsList = db.BlockItemsLists.Find(id);
                lBlockItemsList.StartDate = lSDate;
                lBlockItemsList.EndDate = lEDate;
                lBlockItemsList.IsActive = IsActive;
                //DynamicCategoryProduct dcp = db.DynamicCategoryProducts.Find(id);
                //db.Entry(dcp).CurrentValues.SetValues(lDynamicCategoryProduct);
                db.SaveChanges();
                TempData["Message"] = "Product Updated Successfully";
                return RedirectToAction("Details", new { id = lBlockItemsList.FranchiseID });
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
                ViewBag.Messaage = "Unable to Delete Bank Detail :- " + ex.InnerException.ToString();
                return View(db.DynamicCategoryProducts.Find(id));
            }
        }

        #endregion

        #region Set Sequence

        [SessionExpire]
        [CustomAuthorize(Roles = "HomeProductGallery/CanRead")]
        public ActionResult SetSequence(int id)
        {
            try
            {
                ViewBag.FranchiseList = id;
                List<CategoryDetail> catList = new List<CategoryDetail>();
                catList = (from c in db.Categories
                           join pbc in db.PlanCategoryCharges on c.ID equals pbc.CategoryID
                           join p in db.Plans on pbc.PlanID equals p.ID
                           join op in db.OwnerPlans on p.ID equals op.PlanID
                           where op.OwnerID == id && p.PlanCode.Substring(0, 4) == "GBFR"
                           && op.IsActive == true && pbc.IsActive == true && c.IsActive == true
                           select new CategoryDetail
                           {
                               ID = c.ID,
                               Name = c.Name
                           }).ToList();
                ViewBag.CategoryList = new SelectList(catList.OrderBy(x => x.Name), "ID", "Name");
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");

                return View();
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

        public ActionResult ProductSequence(ModelLayer.Models.ViewModel.ProductGalleryList ls, int Franchise)
        {
            try
            {

                ViewBag.FranchiseList = Franchise;

                BusinessLogicLayer.HomeProductGallery obj = new BusinessLogicLayer.HomeProductGallery();
                Int64 BlockTypeID = db.DesignBlockTypes.Where(x => x.Name.Trim().ToLower() == "product gallery").FirstOrDefault().ID;
                Int64 userID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));

                if (obj.ProductGallerySequenceUpdate(ls, Franchise, BlockTypeID, userID, System.Web.HttpContext.Current.Server))
                {
                    TempData["Message"] = "Sequence Order Set Successfully";
                }
                else
                {
                    TempData["Message"] = "Sorry Unable to set Priority ........";
                }

                return RedirectToAction("SetSequence", new { id = Franchise });
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- LevelOnePriority[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate LevelOnePriority view!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        #endregion

        #region Methods
        public JsonResult SelectLevelOneCategoryByFranchise(int id)
        {
            List<CategoryDetail> catList = new List<CategoryDetail>();
            catList = (from c in db.Categories
                       join pbc in db.PlanCategoryCharges on c.ID equals pbc.CategoryID
                       join p in db.Plans on pbc.PlanID equals p.ID
                       join op in db.OwnerPlans on p.ID equals op.PlanID
                       where op.OwnerID == id && p.PlanCode.Substring(0, 4) == "GBFR"
                       && op.IsActive == true && pbc.IsActive == true && c.IsActive == true
                       select new CategoryDetail
                       {
                           ID = c.ID,
                           Name = c.Name
                       }).ToList();

            return Json(catList, JsonRequestBehavior.AllowGet);


        }

        public JsonResult SelectCategory(Int64 ParentCategory, int level)
        {

            List<CategoryDetail> cd = new List<CategoryDetail>();
            cd = (from n in db.Categories
                  where n.ParentCategoryID == ParentCategory && n.Level == level
                  && n.IsActive == true
                  select new CategoryDetail
                  {
                      ID = n.ID,
                      Name = n.Name
                  }).OrderBy(x => x.Name).ToList();

            return Json(cd, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public PartialViewResult GetProductList(int FranchiseID, int CategoryID)
        {
            var ProductList = (from sp in db.ShopProducts
                               where sp.Shop.FranchiseID == FranchiseID
                               && sp.Product.CategoryID == CategoryID
                               && sp.Product.IsActive == true && sp.IsActive == true && sp.Shop.IsLive == true && sp.Shop.IsActive == true
                               && !
                                    (from bi in db.BlockItemsLists
                                     where bi.FranchiseID == FranchiseID && bi.Product.CategoryID == CategoryID && bi.DesignBlockType.Name == "product gallery"
                                     select new
                                     {
                                         bi.ProductID
                                     }).Contains(new { ProductID = (long?)sp.ProductID })
                               select new DynamicProductListViewModel { ID = sp.ProductID, Name = sp.Product.Name }).Distinct().ToList();


            //var ProductList = (from sp in db.ShopProducts
            //                   join dcp in db.DynamicCategoryProducts on sp.Shop.FranchiseID equals dcp.FranchiseID
            //                   where sp.Shop.FranchiseID == FranchiseID
            //                   && sp.Product.CategoryID == CategoryID
            //                   && dcp.Product.CategoryID == sp.Product.CategoryID
            //                   && dcp.ProductID != sp.ProductID
            //                   && sp.Product.IsActive == true && sp.IsActive == true
            //                   select new DynamicProductListViewModel { ID = sp.ProductID, Name = sp.Product.Name }).Distinct().ToList();

            //return Json(ProductList, JsonRequestBehavior.AllowGet);
            return PartialView("_ProductListing", ProductList);
        }

        public JsonResult GetAddedProducts(int FranchiseID, string SDate, string EDate)
        {
            DateTime lSDate = CommonFunctions.GetProperDate(SDate);
            DateTime lEDate = CommonFunctions.GetProperDate(EDate);

            List<DynamicProductListViewModel> ProductList = new List<DynamicProductListViewModel>();
            ProductList = GetProductList(FranchiseID, lSDate, lEDate);

            //var ProductList = (from dcp in db.DynamicCategoryProducts
            //                   where dcp.FranchiseID == FranchiseID && dcp.StartDate >= lSDate && dcp.EndDate <= lEDate
            //                   select new DynamicProductListViewModel { ID = dcp.ID, Name = dcp.Product.Name }).ToList().OrderBy(x => x.Name);
            return Json(ProductList, JsonRequestBehavior.AllowGet);
        }

        public List<DynamicProductListViewModel> GetProductList(int FranchiseID, DateTime lSDate, DateTime lEDate)
        {
            List<DynamicProductListViewModel> ProductList = new List<DynamicProductListViewModel>();
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(System.Web.HttpContext.Current.Server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
            List<object> paramValues = new List<object>();
            paramValues.Add(FranchiseID);
            paramValues.Add(lSDate);
            paramValues.Add(lEDate);
            dt = dbOpr.GetRecords("Select_HomeProductGalleryList", paramValues);

            ProductList = (from n in dt.AsEnumerable()
                           select new DynamicProductListViewModel
                           {
                               ID = n.Field<Int64>("ProductID"),
                               Name = n.Field<string>("Name")
                           }).ToList();
            return ProductList;
        }

        public ActionResult GetProductListInGallery(Int64 FranchiseID)
        {
            ModelLayer.Models.ViewModel.ProductGalleryList ls = new ModelLayer.Models.ViewModel.ProductGalleryList();

            try
            {
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                DataTable dt = new DataTable();
                dt = BusinessLogicLayer.HomeProductGallery.Select_GalleryProducts(FranchiseID, System.Web.HttpContext.Current.Server);

                /*Select All the Shop By Franchise */
                ls.productList = (from n in dt.AsEnumerable()
                                          select new ModelLayer.Models.ViewModel.ProductGalleryViewModel
                                          {
                                              ID = n.Field<Int64>("ID"),
                                              FranchiseID = n.Field<int>("FranchiseID"),
                                              CategoryID = n.Field<int>("CategoryID"),
                                              CategoryName = n.Field<string>("CategoryName"),
                                              ProductID = n.Field<Int64>("ProductID"),
                                              ProductName = n.Field<string>("ProductName"),
                                              SequenceOrder = n.Field<int?>("SequenceOrder") == null ? 0 : Convert.ToInt32(n.Field<int?>("SequenceOrder")),
                                              IsActive = n.Field<bool>("IsActive")
                                          }).OrderBy(x => x.SequenceOrder).ToList();

                //ViewBag.CategoryID = CategoryID;
                ViewBag.Franchise = FranchiseID;
                return PartialView("_ProductGallerySequence", ls);

            }
            catch
            {
                return PartialView("_ProductGallerySequence", ls);
            }
        }
        #endregion
    }
}