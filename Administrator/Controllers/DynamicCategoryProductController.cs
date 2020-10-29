using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Administrator.Models;
using System.Data;
using BusinessLogicLayer;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Net;

namespace Administrator.Controllers
{
    public class DynamicCategoryProductController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /DynamicCategoryProduct/
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
         Environment.NewLine
         + "ErrorLog Controller : DynamicCategoryProductController" + Environment.NewLine);

        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();

        #region Index
        //
        // GET: /DynamicCategoryProduct/
        [SessionExpire]
        [CustomAuthorize(Roles = "DynamicCategoryProduct/CanRead")]
        public ActionResult Index()
        {
            var franchiseList = db.Franchises.Where(x => x.IsActive == true && x.ID != 1).ToList().OrderBy(x => x.BusinessDetail.Name);
            return View(franchiseList);
        }

        #endregion

        #region Create

        [SessionExpire]
        [CustomAuthorize(Roles = "DynamicCategoryProduct/CanRead")]
        public ActionResult Create()
        {
            List<CategoryDetail> ldata = new List<CategoryDetail>();
            ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
            ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");
            ViewBag.CategoryList = new SelectList(ldata, "ID", "Name"); 
            ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");
            ViewBag.LevelThreeCategoryList = new SelectList(ldata, "ID", "Name");
            return View();
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "DynamicCategoryProduct/CanWrite")]
        public ActionResult Create(string CaltegoryList, int FranchiseList, string SDate, string EDate)
        {
            try
            {
                List<CategoryDetail> ldata = new List<CategoryDetail>();
                ldata.Add(new CategoryDetail { ID = 0, Name = "Select Category" });
                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");
                ViewBag.CategoryList = new SelectList(ldata, "ID", "Name"); ;
                ViewBag.LevelTwoCategoryList = new SelectList(ldata, "ID", "Name");
                ViewBag.LevelThreeCategoryList = new SelectList(ldata, "ID", "Name");

                string[] strProducts;

                strProducts = CaltegoryList.Split(',');

                DateTime lSDate = CommonFunctions.GetProperDateTime(SDate);
                DateTime lEDate = CommonFunctions.GetProperDateTime(EDate);

                this.InsertCategoryProducts(strProducts, FranchiseList, lSDate, lEDate, CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])), BusinessLogicLayer.CommonFunctions.GetClientIP());


                ModelState.Clear();
                // ModelState.AddModelError("Message", "Done! Product Added Successfully!!");
                ViewBag.Message = "Done! Product Added Successfully!!";
            }

            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Plan Bind registration!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DynamicCategoryProduct][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                var catList = db.Categories.Where(c => c.Level == 0);
                return View(catList.ToList());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Plan Bind registration!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DynamicCategoryProduct][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                var catList = db.Categories.Where(c => c.Level == 0);
                return View(catList.ToList());
            }

            return View();

        }

        #endregion

        #region Details

        [SessionExpire]
        [CustomAuthorize(Roles = "DynamicCategoryProduct/CanRead")]
        public ActionResult Details(int id)  // it is franchise id
        {
            var dcpList = db.DynamicCategoryProducts.Where(x => x.FranchiseID == id).ToList().OrderBy(x => x.Product.Name);
            return View(dcpList);
        }

        #endregion

        #region Add More Product

        [SessionExpire]
        [CustomAuthorize(Roles = "DynamicCategoryProduct/CanRead")]
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
        [CustomAuthorize(Roles = "DynamicCategoryProduct/CanWrite")]
        public ActionResult AddMoreProduct(string CaltegoryList, int FranchiseList, string SDate, string EDate)
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

                strProducts = CaltegoryList.Split(',');

                DateTime lSDate = CommonFunctions.GetProperDateTime(SDate);
                DateTime lEDate = CommonFunctions.GetProperDateTime(EDate);

                this.InsertCategoryProducts(strProducts, FranchiseList, lSDate, lEDate, CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])), BusinessLogicLayer.CommonFunctions.GetClientIP());


                ModelState.Clear();
                // ModelState.AddModelError("Message", "Done! Product Added Successfully!!");
                ViewBag.Message = "Done! Product Added Successfully!!";
            }

            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Plan Bind registration!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DynamicCategoryProduct][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                var catList = db.Categories.Where(c => c.Level == 0);
                return View(catList.ToList());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Message", "Sorry! Problem in Plan Bind registration!!");
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DynamicCategoryProduct][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                var catList = db.Categories.Where(c => c.Level == 0);
                return View(catList.ToList());
            }


            return View();

        }

        #endregion

        #region Delete
        //
        // GET: /DynamicCategoryProduct/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "DynamicCategoryProduct/CanDelete")]
        public ActionResult Delete(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DynamicCategoryProduct lDynamicCategoryProduct = db.DynamicCategoryProducts.Find(id);
                if (lDynamicCategoryProduct == null)
                {
                    return HttpNotFound();
                }

                return View(lDynamicCategoryProduct);
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
        // POST: /DynamicCategoryProduct/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "DynamicCategoryProduct/CanDelete")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                DynamicCategoryProduct lDynamicCategoryProduct = db.DynamicCategoryProducts.Find(id);
                
                List<object> paramValues = new List<object>();
                paramValues.Add(id);
                paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.DELETE.ToString());
                paramValues.Add(DBNull.Value);

                int resultCode;
                ViewBag.Messaage = BusinessLogicLayer.DynamicProductList.Delete_DynamicProducts(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.DELETE, out resultCode, fConnectionString);

                if (resultCode == 3)
                {
                    TempData["Message"] = ViewBag.Messaage;
                    return RedirectToAction("Details", new { id = lDynamicCategoryProduct.FranchiseID });
                }
                else
                    return View(lDynamicCategoryProduct);
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

        #region Edit
        //
        // GET: /DynamicCategoryProduct/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "DynamicCategoryProduct/CanRead")]
        public ActionResult Edit(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DynamicCategoryProduct lDynamicCategoryProduct = db.DynamicCategoryProducts.Find(id);
                ViewBag.SDate = lDynamicCategoryProduct.StartDate.ToString("dd/MM/yyyy");
                ViewBag.EDate = lDynamicCategoryProduct.EndDate.ToString("dd/MM/yyyy");

                if (lDynamicCategoryProduct == null)
                {
                    return HttpNotFound();
                }

                return View(lDynamicCategoryProduct);
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
        // POST: /DynamicCategoryProduct/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "DynamicCategoryProduct/CanWrite")]
        [HttpPost]
        public ActionResult Edit(int id, string SDate, string EDate, bool IsActive)
        {
            try
            {
                DateTime lSDate = CommonFunctions.GetProperDateTime(SDate);
                DateTime lEDate = CommonFunctions.GetProperDateTime(EDate);

                DynamicCategoryProduct lDynamicCategoryProduct = db.DynamicCategoryProducts.Find(id);
                lDynamicCategoryProduct.StartDate = lSDate;
                lDynamicCategoryProduct.EndDate = lEDate;
                lDynamicCategoryProduct.IsActive = IsActive;
                //DynamicCategoryProduct dcp = db.DynamicCategoryProducts.Find(id);
                //db.Entry(dcp).CurrentValues.SetValues(lDynamicCategoryProduct);
                db.SaveChanges();
                TempData["Message"] = "Product Updated Successfully";
                return RedirectToAction("Details", new { id = lDynamicCategoryProduct.FranchiseID });
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
        [CustomAuthorize(Roles = "DynamicCategoryProduct/CanRead")]
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

        public ActionResult CategoryProductSequence(ModelLayer.Models.ViewModel.DynamicCategoryProductList ls, int Franchise)
        {
            try
            {

                ViewBag.FranchiseList = Franchise;
                List<CategoryDetail> catList = new List<CategoryDetail>();
                catList = (from c in db.Categories
                           join pbc in db.PlanCategoryCharges on c.ID equals pbc.CategoryID
                           join p in db.Plans on pbc.PlanID equals p.ID
                           join op in db.OwnerPlans on p.ID equals op.PlanID
                           where op.OwnerID == Franchise && p.PlanCode.Substring(0, 4) == "GBFR"
                           && op.IsActive == true && pbc.IsActive == true && c.IsActive == true
                           select new CategoryDetail
                           {
                               ID = c.ID,
                               Name = c.Name
                           }).ToList();
                ViewBag.CategoryList = new SelectList(catList.OrderBy(x => x.Name), "ID", "Name");

                BusinessLogicLayer.DynamicProductList obj = new BusinessLogicLayer.DynamicProductList();
                Int64 userID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));

                if (obj.DynamicProductSequenceUpdate(ls, Franchise, userID, fConnectionString))
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
                                    (from dcp in db.DynamicCategoryProducts
                                     where dcp.FranchiseID == FranchiseID && dcp.Product.CategoryID == CategoryID
                                     select new
                                     {
                                         dcp.ProductID
                                     }).Contains(new { ProductID = sp.ProductID })
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

        private void InsertCategoryProducts(string[] strProducts, int FranchiseList, DateTime StartDate, DateTime EndDate, Int64 createdBY, string IP)
        {

            DataTable lDataTable = new DataTable();
            lDataTable.Columns.Add("ProductID");
            foreach (string val in strProducts)
            {
                int v = Convert.ToInt32(val);
                DataRow dr = lDataTable.NewRow();
                dr[0] = v;
                lDataTable.Rows.Add(dr);
            }

            using (SqlConnection conn = new SqlConnection(fConnectionString))
            {
                SqlCommand sqlComm = new SqlCommand("InsertUpdate_CategoryProducts", conn);
                sqlComm.CommandType = CommandType.StoredProcedure;

                sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.BigInt).Value = FranchiseList;
                sqlComm.Parameters.AddWithValue("@ProductList", SqlDbType.Structured).Value = lDataTable;
                sqlComm.Parameters.AddWithValue("@StartDate", SqlDbType.DateTime2).Value = StartDate;
                sqlComm.Parameters.AddWithValue("@EndDate", SqlDbType.DateTime2).Value = EndDate;
                sqlComm.Parameters.AddWithValue("@SequenceOrder", SqlDbType.Int).Value = DBNull.Value;
                sqlComm.Parameters.AddWithValue("@IsActive", SqlDbType.Bit).Value = true;
                sqlComm.Parameters.AddWithValue("@CreateDate", SqlDbType.DateTime2).Value = DateTime.UtcNow;
                sqlComm.Parameters.AddWithValue("@CreatedBy", SqlDbType.BigInt).Value = createdBY;
                sqlComm.Parameters.AddWithValue("@ModifyDate", SqlDbType.DateTime2).Value = DBNull.Value;
                sqlComm.Parameters.AddWithValue("@DeviceType", SqlDbType.VarChar).Value = "Web Browser";
                sqlComm.Parameters.AddWithValue("@DeviceID", SqlDbType.VarChar).Value = "x";
                sqlComm.Parameters.AddWithValue("@NetworkIP", SqlDbType.VarChar).Value = IP;
                sqlComm.Parameters.AddWithValue("@Remarks", SqlDbType.VarChar).Value = DBNull.Value;

                conn.Open();
                sqlComm.ExecuteNonQuery();

                conn.Close();
                ViewBag.Message = "Done! Product Added Successfully!!";
            }
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
            dt = dbOpr.GetRecords("Select_DynamicCategoryProductList", paramValues);

            ProductList = (from n in dt.AsEnumerable()
                           select new DynamicProductListViewModel
                                    {
                                        ID = n.Field<Int64>("ProductID"),
                                        Name = n.Field<string>("Name")
                                    }).ToList();
            return ProductList;
        }

        public ActionResult GetDynamicCategoryProductList(Int64 FranchiseID, Int64 CategoryID)
        {
            ModelLayer.Models.ViewModel.DynamicCategoryProductList ls = new ModelLayer.Models.ViewModel.DynamicCategoryProductList();

            try
            {
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.DynamicProductList obj = new BusinessLogicLayer.DynamicProductList();
                DataTable dt = new DataTable();
                dt = obj.Call_Select_Procedure(FranchiseID, CategoryID, System.Web.HttpContext.Current.Server);

                /*Select All the Shop By Franchise */
                ls.categoryProductList = (from n in dt.AsEnumerable()
                                        select new ModelLayer.Models.ViewModel.DynamicCategoryProductViewModel
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
                return PartialView("_EditCategoryProductsSequence", ls);

            }
            catch
            {
                return PartialView("_EditCategoryProductsSequence", ls);
            }
        }
        #endregion
    }



    public class DynamicProductListViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
    }

}