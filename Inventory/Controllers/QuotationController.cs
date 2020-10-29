using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using Inventory.Models;
using ModelLayer.Models.ViewModel;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Web.Services;
using BusinessLogicLayer;
using System.Transactions;
using System.Web.Configuration;

namespace Inventory.Controllers
{   
    public class QuotationController : Controller
    {
       
        //
        // GET: /Quotation/
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController Obj_Common = new CommonController();
        //
        // GET: /PurchaseOrders/

        public ActionResult Index()
        {
            try
            {
                if (Session["USER_NAME"] != null)
                {

                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                QuotionRequestListViewModel obj = new QuotionRequestListViewModel();

                List<QuotationViewModel> lQuotationViewModel = new List<QuotationViewModel>();
                long WarehouseID = 0;
                if (Session["WarehouseID"] != null)
                {
                    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                }


                //string str = string.Join(",", db.PurchaseOrderDetails.Where(p => p.PurchaseOrderID == 4)
                //                                 .Select(p => p.ProductNickname.ToString()));

                if (WarehouseID > 0)
                {
                    lQuotationViewModel = (from o in db.Quotations
                                           join w in db.Warehouses on o.RequestFromWarehouseID equals w.ID
                                           where w.ID == WarehouseID
                                           select new QuotationViewModel
                                             {
                                                 QuotationID = o.ID,
                                                 WarehouseName = w.Name,
                                                 QuotationCode = o.QuotationCode,
                                                 IsSent = o.IsSent,
                                                 TotalItems = db.QuotationItemDetails.Where(x => x.QuotationID == o.ID).Select(x => x.ID).Count(),
                                                 QuotationRequestDate = o.QuotationRequestDate,
                                                 ExpectedReplyDate = o.ExpectedReplyDate
                                             }).OrderByDescending(o => o.QuotationRequestDate).ToList();
                }
                else
                {
                    lQuotationViewModel = (from o in db.Quotations
                                           join w in db.Warehouses on o.RequestFromWarehouseID equals w.ID
                                           select new QuotationViewModel
                                           {
                                               QuotationID = o.ID,
                                               WarehouseName = w.Name,
                                               QuotationCode = o.QuotationCode,
                                               IsSent = o.IsSent,
                                               TotalItems = db.QuotationItemDetails.Where(x => x.QuotationID == o.ID).Select(x => x.ID).Count(),
                                               QuotationRequestDate = o.QuotationRequestDate,
                                               ExpectedReplyDate = o.ExpectedReplyDate
                                           }).OrderByDescending(o => o.QuotationRequestDate).ToList();
                }
                Session["lQuotationItemDetailViewModel"] = null;

                obj.lQuotationViewModel = lQuotationViewModel.ToList();

                var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID).Select(x => x.ID).FirstOrDefault();

                //yashaswi 9/4/2018
                ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
                return View("Index", obj);
            }
            catch(Exception ex)
            {

            }
            return RedirectToAction("Index");
        }


        public ActionResult Search(long? SupplierID)
        {
            QuotionRequestListViewModel obj = new QuotionRequestListViewModel();
            List<QuotationViewModel> lQuotationViewModel = new List<QuotationViewModel>();
            try
            {
                long WarehouseID = 0;
                if (Session["WarehouseID"] != null)
                {
                    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                }

                if (SupplierID != null && SupplierID > 0)
                {
                    lQuotationViewModel = (from o in db.Quotations
                                           join w in db.Warehouses on o.RequestFromWarehouseID equals w.ID
                                           join qs in db.QuotationSupplierLists on o.ID equals qs.QuotationID                                           
                                           where w.ID == WarehouseID && qs.SupplierID==SupplierID
                                           select new QuotationViewModel
                                           {
                                               QuotationID = o.ID,
                                               WarehouseName = w.Name,
                                               QuotationCode = o.QuotationCode,
                                               IsSent = o.IsSent,
                                               TotalItems = db.QuotationItemDetails.Where(x => x.QuotationID == o.ID).Select(x => x.ID).Count(),
                                               QuotationRequestDate = o.QuotationRequestDate,
                                               ExpectedReplyDate = o.ExpectedReplyDate
                                           }).OrderByDescending(o => o.QuotationRequestDate).ToList();

                }
                else
                {
                    return RedirectToAction("Index");
                }

                if (Session["USER_NAME"] != null)
                {

                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

                Session["lQuotationItemDetailViewModel"] = null;
                obj.lQuotationViewModel = lQuotationViewModel.ToList();

                var wSupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID).Select(x => x.ID).FirstOrDefault();               

                ViewBag.PossibleSuppliers = db.Suppliers.Where(x => x.IsActive == true && x.ContactPerson != "" && x.ID != wSupplierID);
                ViewBag.PossibleSuppliers = db.Suppliers;
            }
            catch (Exception)
            {
                //Transaction.Current.Rollback();
                //tscope.Dispose();
            }
            //return Json(objWS, JsonRequestBehavior.AllowGet);
            return View("Index", obj);
        }

        //
        // GET: /Quotation/Details/5

        public ViewResult Details(long id)
        {
            QuotationViewModel QuotationViewModel = new QuotationViewModel();

            int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);

            Session["lQuotationItemDetailViewModel"] = null;

            QuotationViewModel = (from po in db.Quotations                                     
                                      join w in db.Warehouses on po.RequestFromWarehouseID equals w.ID
                                      where po.ID == id
                                      select new QuotationViewModel
                                      {
                                          QuotationID = po.ID,
                                          QuotationCode = po.QuotationCode,
                                          WarehouseName = w.Name,
                                          QuotationRequestDate = po.QuotationRequestDate,
                                          ExpectedReplyDate = po.ExpectedReplyDate,
                                          Remark = po.Remark,
                                          IsActive = po.IsActive
                                      }).FirstOrDefault();

            List<QuotationItemDetail> lQuotationItemDetaillist = new List<QuotationItemDetail>();
            lQuotationItemDetaillist = db.QuotationItemDetails.Where(x => x.QuotationID == id).ToList();

            List<QuotationItemDetailViewModel> lQuotationItemDetailViewModelList = new List<QuotationItemDetailViewModel>();

            foreach (QuotationItemDetail item in lQuotationItemDetaillist)
            {
                QuotationItemDetailViewModel objPOD = new QuotationItemDetailViewModel();
                objPOD.Quantity = item.Quantity;
                objPOD.Nickname = item.ProductNickname;
                objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                var itemName = (from p in db.Products
                                join v in db.ProductVarients on p.ID equals v.ProductID
                                join s in db.Sizes on v.SizeID equals s.ID
                                where v.ID == item.ProductVarientID
                                select new PurchaseOrderDetailsViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

                foreach (var i in itemName)
                {
                    objPOD.ItemName = i.ItemName.ToString();
                    objPOD.HSNCode = Convert.ToString(i.HSNCode);
                }

                lQuotationItemDetailViewModelList.Add(objPOD);
            }

            if (lQuotationItemDetailViewModelList.Count > 0)
            {
                Session["lQuotationItemDetailViewModel"] = lQuotationItemDetailViewModelList;
            }
            ViewBag.QuotationID = id;
            return View(QuotationViewModel);
        }

        //
        // GET: /PurchaseOrders/Create

        public ActionResult Create()
        {
            QuotationViewModel objPO = new QuotationViewModel();
            if (Session["WarehouseID"] != null && Convert.ToInt32(Session["WarehouseID"]) > 0)
            {
                int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
                ViewBag.PossibleWarehouses = db.Warehouses.Where(x => x.ID == WarehouseID);
                ViewBag.WarehouseID = WarehouseID;
                var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID).Select(x => x.ID).FirstOrDefault();
                List<Category> lCategory = new List<Category>();
                List<ForLoopClass> forloopclasses = new List<ForLoopClass>();

                lCategory = db.Categories.Where(x => x.ParentCategoryID == null && x.IsActive == true).ToList();

                foreach (var c in lCategory)
                {
                    ForLoopClass av = new ForLoopClass();
                    av.ID = c.ID;
                    av.Name = c.Name;
                    forloopclasses.Add(av);
                }

                ViewBag.PossiblePrentCategory = forloopclasses.ToList();               

                List<SelectListItem> listSelectListItems = new List<SelectListItem>();

                //Yashaswi 9/4/2018
                var lSuppliers = Obj_Common.GetSupplierLIst(WarehouseID)
                                 .Select(x => new { x.ID, x.Name, x.ContactPerson, x.IsActive })
                                 .Distinct().ToList();
                
                List<SupplierModel> lSupplierList = new List<SupplierModel>();
                if (lSuppliers.Count() > 0)
                {
                    for (int i = 0; i < lSuppliers.Count(); i++)
                    {
                        SupplierModel lSupplier = new SupplierModel();

                        lSupplier.ID = Convert.ToInt32(lSuppliers[i].ID);
                        lSupplier.Name = CommonFunctions.ConvertStringToCamelCase(Convert.ToString(lSuppliers[i].Name) + " (" + Convert.ToString(lSuppliers[i].ContactPerson)+")");
                        lSupplierList.Add(lSupplier);
                    }
                }
                objPO.SupplierList = lSupplierList;
                objPO.IsActive = true;
            }
            else
            {
                if (Session["USER_NAME"] != null)
                {
                    ViewBag.PossibleWarehouses = db.Warehouses;
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }

            objPO.QuotationRequestDate = DateTime.Now;
            objPO.ExpectedReplyDate = DateTime.Now;          
            return View(objPO);
        }

        public PartialViewResult Add(int ProductID, long ProductVarientID, int Quantity, string Nickname)
        {
            List<QuotationItemDetailViewModel> lQuotationItemDetailViewModel = new List<QuotationItemDetailViewModel>();

            if (Session["lQuotationItemDetailViewModel"] != null)
            {
                lQuotationItemDetailViewModel = (List<QuotationItemDetailViewModel>)Session["lQuotationItemDetailViewModel"];
            }

            QuotationItemDetailViewModel objd = new QuotationItemDetailViewModel();

            var itemName = (from p in db.Products
                            join v in db.ProductVarients on p.ID equals v.ProductID
                            join s in db.Sizes on v.SizeID equals s.ID
                            where v.ID == ProductVarientID
                            select new QuotationItemDetailViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

            objd.StockThumbPath = ImageDisplay.SetProductThumbPath(ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
            objd.QuotationItemDetailID = 0;
            objd.ProductID = ProductID;
            objd.ProductVarientID = ProductVarientID;

            foreach (var item in itemName)
            {
                objd.ItemName = item.ItemName.ToString();
                objd.HSNCode = Convert.ToString(item.HSNCode);
            }
            objd.Quantity = Quantity;
            objd.Nickname = Nickname;

            var vId = lQuotationItemDetailViewModel.Find(x => x.ProductVarientID == ProductVarientID);
            if (vId == null)
            {
                lQuotationItemDetailViewModel.Add(objd);
            }


            Session["lQuotationItemDetailViewModel"] = lQuotationItemDetailViewModel;

            return PartialView("ItemDetails", lQuotationItemDetailViewModel);
        }


        public PartialViewResult Remove(long ProductVarientID)
        {
            List<QuotationItemDetailViewModel> lQuotationItemDetailViewModel = new List<QuotationItemDetailViewModel>();

            if (Session["lQuotationItemDetailViewModel"] != null)
            {
                lQuotationItemDetailViewModel = (List<QuotationItemDetailViewModel>)Session["lQuotationItemDetailViewModel"];
                var id = lQuotationItemDetailViewModel.First(x => x.ProductVarientID == ProductVarientID);

                decimal UnitPrice = id.UnitPrice;
                decimal Quantity = id.Quantity;               

                lQuotationItemDetailViewModel.Remove(id);
                Session["lQuotationItemDetailViewModel"] = lQuotationItemDetailViewModel;
            }

            return PartialView("ItemDetails", lQuotationItemDetailViewModel);
        }

        //
        // POST: /PurchaseOrders/Create


        [HttpPost]
        public ActionResult Create(QuotationViewModel Quotation)
        {
            List<QuotationItemDetailViewModel> lQuotationItemDetailViewModel = new List<QuotationItemDetailViewModel>();
            int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
            if (Session["lQuotationItemDetailViewModel"] != null)
            {
                lQuotationItemDetailViewModel = (List<QuotationItemDetailViewModel>)Session["lQuotationItemDetailViewModel"];
                if (lQuotationItemDetailViewModel.Count > 0)
                {
                    if (ModelState.IsValid)
                    {
                        using (TransactionScope tscope = new TransactionScope())
                        {
                            try
                            {
                                //Insert into Quotation Table
                                Quotation objQuotation = new Quotation();
                                objQuotation.RequestFromWarehouseID = Quotation.RequestFromWarehouseID;
                                objQuotation.QuotationCode = GetNextQuotationCode();
                                objQuotation.QuotationRequestDate = DateTime.Now; ;
                                objQuotation.ExpectedReplyDate = Quotation.ExpectedReplyDate;
                                objQuotation.IsSent = Quotation.IsSent;
                                objQuotation.Remark = Quotation.Remark;
                                objQuotation.IsActive = true;
                                objQuotation.CreateDate = DateTime.Now;
                                objQuotation.CreateBy = GetPersonalDetailID();
                                objQuotation.NetworkIP = CommonFunctions.GetClientIP();
                                objQuotation.DeviceID = "X";
                                objQuotation.DeviceType = "X";
                                db.Quotations.Add(objQuotation);
                                db.SaveChanges();

                                long QuotationID = objQuotation.ID;

                                //Insert into QuotationItemDetail Table
                                QuotationItemDetail objQuotationItemDetail = new QuotationItemDetail();
                                foreach (QuotationItemDetailViewModel item in lQuotationItemDetailViewModel)
                                {
                                    objQuotationItemDetail.QuotationID = QuotationID;
                                    objQuotationItemDetail.ProductID = item.ProductID;
                                    objQuotationItemDetail.ProductNickname = item.Nickname;
                                    objQuotationItemDetail.ProductVarientID = item.ProductVarientID;
                                    objQuotationItemDetail.Quantity = item.Quantity;
                                    db.QuotationItemDetails.Add(objQuotationItemDetail);
                                    db.SaveChanges();
                                }


                                var temp = (from n in Quotation.SupplierList
                                            where n.IsSelected == true
                                            select new QuotationSupplierList
                                            {
                                                QuotationID = Convert.ToInt32(QuotationID),
                                                SupplierID = n.ID,
                                                IsActive = true,
                                            }).ToList();

                                if (temp.Count > 0)
                                {
                                    DataTable dt = new DataTable();
                                    dt.Columns.Add("SupplierID");
                                    dt.Columns.Add("IsActive");
                                    dt.Columns.Add("QuotationID");


                                    foreach (QuotationSupplierList pa in temp)
                                    {
                                        DataRow dr = dt.NewRow();
                                        dr["SupplierID"] = pa.SupplierID;
                                        dr["IsActive"] = pa.IsActive;
                                        dr["QuotationID"] = pa.QuotationID;

                                        dt.Rows.Add(dr);
                                    }
                                    string msg = InsertUpdateQuotationSupplierList(0, Convert.ToInt32(QuotationID), dt, "INSERT");
                                }
                                else
                                {
                                    ViewBag.PossibleWarehouses = db.Warehouses;
                                    Session["Error"] = "You have not selected any supplier! Please select.";
                                    return View(Quotation);
                                }
                                tscope.Complete();
                                Session["Success"] = "Quotation Request Created Successfully."; //Yashaswi 30/3/2018
                            }
                            catch (Exception)
                            {
                                Transaction.Current.Rollback();
                                tscope.Dispose();
                            }
                        }
                        ViewBag.PossibleWarehouses = db.Warehouses;                      
                        return RedirectToAction("Index");
                    }
                }
            }
            ViewBag.PossibleWarehouses = db.Warehouses;           
            return View(Quotation);
        }

        private string InsertUpdateQuotationSupplierList(Int64 ID, int QuotationID, DataTable dt, string Operation)
        {
            try
            {
                string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(fConnectionString))
                {
                    SqlCommand sqlComm = new SqlCommand("InsertUpdateQuotationSupplierList", conn);
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    sqlComm.Parameters.AddWithValue("@ID", SqlDbType.BigInt).Value = ID;
                    sqlComm.Parameters.AddWithValue("@QuotationID", SqlDbType.Int).Value = QuotationID;
                    sqlComm.Parameters.AddWithValue("@SupplierList", SqlDbType.Structured).Value = dt;
                    sqlComm.Parameters.AddWithValue("@Mode", SqlDbType.VarChar).Value = Operation;
                    sqlComm.Parameters.AddWithValue("@QryResult", SqlDbType.Int).Value = 0;

                    conn.Open();
                    sqlComm.ExecuteNonQuery();
                    conn.Close();
                }
                return "Records Inserted Successfully";
            }
            catch (Exception ex)
            {
                //pServerMsg += "\nError : " + (int)IsoftConstant.IS_ERROR_TYPE.EXCEPTION + " : " + ex.Message;
                return "Unable to insert records!";
            }
        }

        private string GetNextQuotationCode()
        {
            string newOrderCode = string.Empty;
            
            string lOrderPrefix = "QT";

            try
            {
                OrderManagement lOrderManagement = new OrderManagement();
                int lQT = GetNextQuotationCodeSequence();
                if (lQT > 0)
                {
                    newOrderCode = lOrderPrefix + lQT.ToString("00000");
                    return newOrderCode;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        private int GetNextQuotationCodeSequence()
        {
            int lWHPO = -1;

            try
            {
                DataTable lDataTableCustomerOrder = new DataTable();
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand sqlComm = new SqlCommand("SelectNextQuotationCode", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                //sqlComm.Parameters.AddWithValue("@pFranchiseID", SqlDbType.Int).Value = pFranchiseID;
                con.Open();
                //object o = sqlComm.ExecuteScalar();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    lWHPO = Convert.ToInt32(dt.Rows[0][0]);
                }
                con.Close();
                return lWHPO;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[Quotation -> GetNextQuotationCode]", "Problem in getting QT" + Environment.NewLine + ex.Message);
            }
        }

        public long GetPersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            long UserLoginID = Convert.ToInt64(Session["USER_LOGIN_ID"]);
            long PersonalDetailID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[Quotation][GetPersonalDetailID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }

        //
        // GET: /Quotation/Edit/5

        public ActionResult Edit(long id)
        {
            //Check Session active or not
            if (Session["USER_NAME"] != null)
            { }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            //End

            QuotationViewModel QuotationViewModel = new QuotationViewModel();
            if (Session["WarehouseID"] != null && Convert.ToInt32(Session["WarehouseID"]) > 0)
            {
                int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
                ViewBag.PossibleWarehouses = db.Warehouses.Where(x => x.ID == WarehouseID);
                ViewBag.WarehouseID = WarehouseID;

                List<Category> lCategory = new List<Category>();
                List<ForLoopClass> forloopclasses = new List<ForLoopClass>();

                lCategory = db.Categories.Where(x => x.ParentCategoryID == null && x.IsActive == true).ToList();

                foreach (var c in lCategory)
                {
                    ForLoopClass av = new ForLoopClass();
                    av.ID = c.ID;
                    av.Name = c.Name;
                    forloopclasses.Add(av);
                }

                ViewBag.PossiblePrentCategory = forloopclasses.ToList();
            }
            else
            {
                ViewBag.PossibleWarehouses = db.Warehouses;
                //return RedirectToAction("Index", "Login");
            }

            Session["lQuotationItemDetailViewModel"] = null;

            var Quotation = db.Quotations.Single(x => x.ID == id);

            if (Quotation != null && Quotation.ToString() != "")
            {
                QuotationViewModel.QuotationID = id;
                QuotationViewModel.RequestFromWarehouseID = Quotation.RequestFromWarehouseID;
                QuotationViewModel.ExpectedReplyDate = Quotation.ExpectedReplyDate;
                QuotationViewModel.IsSent = Quotation.IsSent;
                QuotationViewModel.Remark = Quotation.Remark;
                QuotationViewModel.IsActive = Quotation.IsActive;
            }

            List<QuotationItemDetail> lQuotationItemDetaillist = new List<QuotationItemDetail>();
            lQuotationItemDetaillist = db.QuotationItemDetails.Where(x => x.QuotationID == id).ToList();

            List<QuotationItemDetailViewModel> lQuotationItemDetailViewModelList = new List<QuotationItemDetailViewModel>();

            foreach (QuotationItemDetail item in lQuotationItemDetaillist)
            {
                QuotationItemDetailViewModel objPOD = new QuotationItemDetailViewModel();
                objPOD.QuotationItemDetailID = item.ID;
                objPOD.ProductID = item.ProductID;
                objPOD.ProductVarientID = Convert.ToInt64(item.ProductVarientID);
                objPOD.Quantity = item.Quantity;
                objPOD.Nickname = item.ProductNickname;
                objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                var itemName = (from p in db.Products
                                join v in db.ProductVarients on p.ID equals v.ProductID
                                join s in db.Sizes on v.SizeID equals s.ID
                                where v.ID == item.ProductVarientID
                                select new QuotationItemDetailViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

                foreach (var i in itemName)
                {
                    objPOD.ItemName = i.ItemName.ToString();
                    objPOD.HSNCode = Convert.ToString(i.HSNCode);
                }

                lQuotationItemDetailViewModelList.Add(objPOD);
            }

            if (lQuotationItemDetailViewModelList.Count > 0)
            {
                Session["lQuotationItemDetailViewModel"] = lQuotationItemDetailViewModelList;
            }

            List<SelectListItem> listSelectListItems = new List<SelectListItem>();

            var lSuppliers = Obj_Common.GetSupplierLIst(Quotation.RequestFromWarehouseID)
                             .Select(x => new { x.ID, x.Name, x.ContactPerson, x.IsActive })
                             .Distinct().ToList();

            var lQuotationSupplierLists = db.QuotationSupplierLists.Where(x => x.QuotationID == id && x.IsActive == true).Select(x => new { x.ID, x.SupplierID, x.IsActive }).ToList();
            List<SupplierModel> lSupplierList = new List<SupplierModel>();

            if (lSuppliers.Count() > 0)
                {
                    for (int i = 0; i < lSuppliers.Count(); i++)
                    {
                        SupplierModel lSupplier = new SupplierModel();
                        lSupplier.ID = Convert.ToInt32(lSuppliers[i].ID);
                        lSupplier.Name = CommonFunctions.ConvertStringToCamelCase(Convert.ToString(lSuppliers[i].ContactPerson));

                        if (lQuotationSupplierLists != null)
                        {
                            for (int j = 0; j < lQuotationSupplierLists.Count(); j++)
                            {
                                if (lSupplier.ID == lQuotationSupplierLists[j].SupplierID)
                                {
                                    lSupplier.IsSelected = true;
                                    break;
                                }
                            }
                        }
                        lSupplierList.Add(lSupplier);
                    }
                }
           
                QuotationViewModel.SupplierList = lSupplierList;
           

            ViewBag.QuotationID = id;
            return View(QuotationViewModel);
        }

        //
        // POST: /Quotation/Edit/5

        [HttpPost]
        public ActionResult Edit(QuotationViewModel Quotation)
        {
            List<QuotationItemDetailViewModel> lQuotationItemDetailViewModel = new List<QuotationItemDetailViewModel>();

            if (Session["lQuotationItemDetailViewModel"] != null)
            {
                lQuotationItemDetailViewModel = (List<QuotationItemDetailViewModel>)Session["lQuotationItemDetailViewModel"];
                if (lQuotationItemDetailViewModel.Count > 0)
                {
                    if (ModelState.IsValid)
                    {
                        using (TransactionScope tscope = new TransactionScope())
                        {
                            try
                            {
                                //Update Quotation
                                var lQuotation = new Quotation()
                                {
                                    ID = Quotation.QuotationID,
                                    RequestFromWarehouseID = Quotation.RequestFromWarehouseID,
                                    ExpectedReplyDate = Quotation.ExpectedReplyDate,
                                    IsSent = Quotation.IsSent,
                                    Remark = Quotation.Remark,
                                    ModifyDate = DateTime.Now,
                                    ModifyBy = GetPersonalDetailID(),
                                    IsActive = true,
                                    NetworkIP = CommonFunctions.GetClientIP(),
                                    DeviceID = "X",
                                    DeviceType = "X"
                                };

                                db.Quotations.Attach(lQuotation);
                                db.Entry(lQuotation).Property(x => x.RequestFromWarehouseID).IsModified = true;
                                db.Entry(lQuotation).Property(x => x.ExpectedReplyDate).IsModified = true;
                                db.Entry(lQuotation).Property(x => x.IsSent).IsModified = true;
                                db.Entry(lQuotation).Property(x => x.Remark).IsModified = true;
                                db.Entry(lQuotation).Property(x => x.ModifyDate).IsModified = true;
                                db.Entry(lQuotation).Property(x => x.ModifyBy).IsModified = true;
                                db.Entry(lQuotation).Property(x => x.IsActive).IsModified = true;
                                db.Entry(lQuotation).Property(x => x.NetworkIP).IsModified = true;
                                db.Entry(lQuotation).Property(x => x.DeviceID).IsModified = true;
                                db.Entry(lQuotation).Property(x => x.DeviceType).IsModified = true;
                                db.SaveChanges();


                                DataTable dtQuotationItemDetail = new DataTable();
                                dtQuotationItemDetail.Columns.Add(new DataColumn("QuotationItemDetailID", typeof(long)));
                                dtQuotationItemDetail.Columns.Add(new DataColumn("QuotationID", typeof(long)));
                                dtQuotationItemDetail.Columns.Add(new DataColumn("ProductID", typeof(long)));
                                dtQuotationItemDetail.Columns.Add(new DataColumn("ProductNickname", typeof(string)));
                                dtQuotationItemDetail.Columns.Add(new DataColumn("ProductVarientID", typeof(long)));
                                dtQuotationItemDetail.Columns.Add(new DataColumn("Quantity", typeof(int)));
                                dtQuotationItemDetail.Columns.Add(new DataColumn("Remark", typeof(string)));


                                //Insert/Update into QuotationItemDetail Table
                                QuotationItemDetail objQuotationItemDetail = new QuotationItemDetail();
                                foreach (QuotationItemDetailViewModel item in lQuotationItemDetailViewModel)
                                {
                                    DataRow dr = dtQuotationItemDetail.NewRow();
                                    dr["QuotationItemDetailID"] = item.QuotationItemDetailID;
                                    dr["QuotationID"] = Quotation.QuotationID;
                                    dr["ProductID"] = item.ProductID;
                                    dr["ProductNickname"] = item.Nickname;
                                    dr["ProductVarientID"] = item.ProductVarientID;
                                    dr["Quantity"] = item.Quantity;                                   
                                    dr["Remark"] = item.ProductRemark;

                                    dtQuotationItemDetail.Rows.Add(dr);
                                }

                                SqlConnection con = new SqlConnection(fConnectionString);
                                SqlCommand cmd = new SqlCommand("UpdateQuotationItemDetail", con);
                                cmd.CommandType = CommandType.StoredProcedure;

                                SqlParameter[] objParam = new SqlParameter[2];
                                objParam[0] = new SqlParameter("@QuotationID", Quotation.QuotationID);
                                objParam[1] = new SqlParameter("@TblQuotationItemDetail", dtQuotationItemDetail);

                                int i = 0;
                                for (i = 0; i < objParam.Length; i++)
                                {
                                    cmd.Parameters.Add(objParam[i]);
                                }

                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();


                                var temp = (from n in Quotation.SupplierList
                                            select new QuotationSupplierList
                                            {
                                                QuotationID = Convert.ToInt32(Quotation.QuotationID),
                                                SupplierID = n.ID,
                                                IsActive = n.IsSelected,
                                            }).ToList();

                                if (temp.Count > 0)
                                {
                                    DataTable dt = new DataTable();
                                    dt.Columns.Add("SupplierID");
                                    dt.Columns.Add("IsActive");
                                    dt.Columns.Add("QuotationID");

                                    foreach (QuotationSupplierList pa in temp)
                                    {
                                        DataRow dr = dt.NewRow();
                                        dr["SupplierID"] = pa.SupplierID;
                                        dr["IsActive"] = pa.IsActive;
                                        dr["QuotationID"] = pa.QuotationID;

                                        dt.Rows.Add(dr);
                                    }
                                    string msg = InsertUpdateQuotationSupplierList(0, Convert.ToInt32(Quotation.QuotationID), dt, "UPDATE");
                                }

                                tscope.Complete();
                                Session["Success"] = "Quotation Request Updated Successfully."; //Yashaswi 30/3/2018
                            }
                            catch (Exception)
                            {
                                Transaction.Current.Rollback();
                                tscope.Dispose();
                            }
                        }

                        return RedirectToAction("Index");
                    }
                }
            }
            ViewBag.PossibleWarehouses = db.Warehouses;
            int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);            

            return View(Quotation);
        }


        public ActionResult Send(long id)
        {
            using (TransactionScope tscope = new TransactionScope())
            {
                try
                {
                    //Update Quotation
                    var lQuotation = new Quotation()
                    {
                        ID = id,
                        IsSent = true,
                        ModifyDate = DateTime.Now,
                        ModifyBy = GetPersonalDetailID()
                    };

                    db.Quotations.Attach(lQuotation);
                    db.Entry(lQuotation).Property(x => x.IsSent).IsModified = true;
                    db.Entry(lQuotation).Property(x => x.ModifyDate).IsModified = true;
                    db.Entry(lQuotation).Property(x => x.ModifyBy).IsModified = true;
                    db.SaveChanges();

                    tscope.Complete();
                }
                catch (Exception)
                {
                    Transaction.Current.Rollback();
                    tscope.Dispose();
                }
            }
                            
             return RedirectToAction("Index");
        }



        public ActionResult PrintList(long id)
        {

            if (Session["USER_NAME"] != null)
            {

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

            List<QuotationSupplierListViewModel> lQuotationSupplierListViewModel = new List<QuotationSupplierListViewModel>();
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }
            
            lQuotationSupplierListViewModel = (from o in db.Quotations
                                       join qs in db.QuotationSupplierLists on o.ID equals qs.QuotationID
                                       join s in db.Suppliers on qs.SupplierID equals s.ID
                                               where qs.QuotationID == id && qs.IsActive == true
                                               select new QuotationSupplierListViewModel
                                       {
                                           QuotationID = o.ID,                                        
                                           QuotationCode = o.QuotationCode,
                                           IsSent = o.IsSent,
                                           TotalItems = db.QuotationItemDetails.Where(x => x.QuotationID == o.ID).Select(x => x.ID).Count(),
                                           QuotationRequestDate = o.QuotationRequestDate,
                                           ExpectedReplyDate = o.ExpectedReplyDate,
                                           SupplierID = qs.SupplierID,
                                           SupplierName = s.Name
                                       }).ToList();
           
            Session["lQuotationItemDetailViewModel"] = null;

            return View("PrintList", lQuotationSupplierListViewModel);
        }

        //
        // GET: /Quotation/Print/5
        public ViewResult Print(long SupplierID,long QuotationID)
        {
            QuotationViewModel QuotationViewModel = new QuotationViewModel();

            int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);

            Session["lQuotationItemDetailViewModel"] = null;

            QuotationViewModel = (from po in db.Quotations
                                  join s in db.QuotationSupplierLists on po.ID equals s.QuotationID
                                  join sp in db.Suppliers on s.SupplierID equals sp.ID
                                  join w in db.Warehouses on po.RequestFromWarehouseID equals w.ID
                                  join b in db.BusinessDetails on w.BusinessDetailID equals b.ID
                                  where po.ID == QuotationID && s.SupplierID == SupplierID

                                  select new QuotationViewModel
                                  {
                                      QuotationID = po.ID,
                                      QuotationCode = po.QuotationCode,
                                      WarehouseName = w.Name,
                                      SupplierName = sp.Name,
                                      QuotationRequestDate = po.QuotationRequestDate,
                                      ExpectedReplyDate = po.ExpectedReplyDate,
                                      Remark = po.Remark,
                                      IsActive = po.IsActive,
                                      SupplierCode = sp.SupplierCode,
                                      SupplierContactPerson = sp.ContactPerson,
                                      SupplierAddress = sp.Address,
                                      SupplierMobile = sp.Mobile,
                                      SupplierEmail = sp.Email,
                                      WarehouseContactPerson = b.ContactPerson,
                                      WarehouseAddress = b.Address,
                                      WarehouseMobile = b.Mobile,
                                      WarehosueEmail = b.Email,
                                      WarehouseGSTNumber = w.GSTNumber,
                                      WarehouseFax = b.FAX
                                  }).FirstOrDefault();

            
            List<QuotationItemDetail> lQuotationItemDetaillist = new List<QuotationItemDetail>();
            lQuotationItemDetaillist = db.QuotationItemDetails.Where(x => x.QuotationID == QuotationID).ToList();


            List<QuotationItemDetailViewModel> lQuotationItemDetailViewModelList = new List<QuotationItemDetailViewModel>();

            foreach (QuotationItemDetail item in lQuotationItemDetaillist)
            {
                QuotationItemDetailViewModel objPOD = new QuotationItemDetailViewModel();
                objPOD.Quantity = item.Quantity;
                objPOD.Nickname = item.ProductNickname;
                objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                var itemName = (from p in db.Products
                                join v in db.ProductVarients on p.ID equals v.ProductID
                                join s in db.Sizes on v.SizeID equals s.ID
                                where v.ID == item.ProductVarientID
                                select new PurchaseOrderDetailsViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

                foreach (var i in itemName)
                {
                    objPOD.ItemName = i.ItemName.ToString();
                    objPOD.HSNCode = Convert.ToString(i.HSNCode);
                }

                lQuotationItemDetailViewModelList.Add(objPOD);
            }

            if (lQuotationItemDetailViewModelList.Count > 0)
            {
                Session["lQuotationItemDetailViewModel"] = lQuotationItemDetailViewModelList;
            }


            ViewBag.QuotationID = QuotationID;
            return View(QuotationViewModel);
        }


        
        // GET: /PurchaseOrders/Delete/5

        public ActionResult Delete(long id)
        {
            PurchaseOrder purchaseorder = db.PurchaseOrders.Single(x => x.ID == id);
            return View(purchaseorder);
        }

        //
        // POST: /PurchaseOrders/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            PurchaseOrder purchaseorder = db.PurchaseOrders.Single(x => x.ID == id);
            db.PurchaseOrders.Remove(purchaseorder);
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


        #region Methods called in jquery
        public JsonResult BindParentCategory(int WarehouseID)
        {
            List<Category> lCategory = new List<Category>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();
            if (WarehouseID > 0)
            {
                lCategory = db.Categories.Where(x => x.ParentCategoryID == null && x.IsActive == true).ToList();

                foreach (var c in lCategory)
                {
                    ForLoopClass av = new ForLoopClass();
                    av.ID = c.ID;
                    av.Name = c.Name;
                    forloopclasses.Add(av);
                }
            }
            return Json(forloopclasses.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNextLevelCategory(int categoryID)
        {
            List<Category> lCategory = new List<Category>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();
            //var ParentCat = db.Categories.Where(x => x.ParentCategoryID == categoryID).FirstOrDefault();
            lCategory = db.Categories.Where(x => x.ParentCategoryID == categoryID && x.IsActive == true).ToList();

            foreach (var c in lCategory)
            {
                ForLoopClass av = new ForLoopClass();
                av.ID = c.ID;
                av.Name = c.Name;
                forloopclasses.Add(av);
            }

            return Json(forloopclasses.ToList(), JsonRequestBehavior.AllowGet);
        }


        //Yashaswi 2/5/2018 Method Used in Quotation and Purchase Order For Product List
        [HttpGet]
        public JsonResult AutoComplete(string term, int? categoryID)
        {

            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);

            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            term = term.Replace("=", "+");
            List<AutoSearchProductViewModel> lSearchList = new List<AutoSearchProductViewModel>();
            var query = (from p in db.Products
                         join sp in db.ShopProducts on p.ID equals sp.ProductID
                         join s in db.Shops on sp.ShopID equals s.ID
                         join f in db.Franchises on s.FranchiseID equals f.ID
                         where p.IsActive == true
                         && sp.IsActive == true && s.IsActive == true && f.IsActive == true && p.Name.Contains(term)
                         select new
                         {
                             ID = p.ID,
                             Name = p.Name,
                             CategoryID = p.CategoryID,
                             franchiseId = f.ID
                         }).Distinct();


            //If Filter by Category 
            if (categoryID != null && categoryID != 0)
            {
                int catID = Convert.ToInt16(categoryID);
                query = (from p in query
                         where p.CategoryID == catID
                         select new
                         {
                             ID = p.ID,
                             Name = p.Name,
                             CategoryID = p.CategoryID,
                             franchiseId = p.franchiseId
                         }).Distinct();
            }


            //For DV And FV
            if (WarehouseID != EzeeloWarehouseId)
            {
                var fList = db.WarehouseFranchises.Where(x => x.WarehouseID == WarehouseID && x.IsActive == true).Select(x => x.FranchiseID).ToList();

                if (fList.Count > 0)
                {
                    query = (from q in query
                             where fList.Contains(q.franchiseId)
                             select new
                             {
                                 ID = q.ID,
                                 Name = q.Name,
                                 CategoryID = q.CategoryID,
                                 franchiseId = q.franchiseId
                             }).Distinct();
                }
            }

            query = (from q in query
                     select new
                     {
                         ID = q.ID,
                         Name = q.Name,
                         CategoryID = q.CategoryID,
                         franchiseId = 0
                     }).Distinct().Take(15);

            lSearchList = query.ToList().Select(p => new AutoSearchProductViewModel
            {
                ID = p.ID.ToString(),
                Name = p.Name,
                Cat3 = p.CategoryID
            }).ToList();

            lSearchList = lSearchList.Select(p => new AutoSearchProductViewModel
            {
                ID = p.ID,
                Name = p.Name,
                Cat3 = p.Cat3,
                Cat2 = (from dd in db.Categories
                        join dd1 in db.Categories on dd.ID equals dd1.ParentCategoryID
                        where dd1.ID == p.Cat3
                        select new { dd.ID }).First().ID
            }).Select(p => new AutoSearchProductViewModel
            {
                ID = p.ID,
                Name = p.Name,
                Cat3 = p.Cat3,
                Cat2 = p.Cat2,
                Cat1 = (from dd in db.Categories
                        join dd1 in db.Categories on dd.ID equals dd1.ParentCategoryID
                        where dd1.ID == p.Cat2
                        select new { dd.ID }).First().ID
            })
                   .ToList();


            return Json(lSearchList, JsonRequestBehavior.AllowGet);
        }

        //Yashaswi 2/5/2018
        public JsonResult GetCategoryAfterProductSelect(int categoryID)
        {
            List<Category> lCategory = new List<Category>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();
            lCategory = db.Categories.Where(x => x.ParentCategoryID == (db.Categories.FirstOrDefault(c => c.ID == categoryID).ParentCategoryID) && x.IsActive == true).ToList();

            foreach (var c in lCategory)
            {
                ForLoopClass av = new ForLoopClass();
                av.ID = c.ID;
                av.Name = c.Name;
                forloopclasses.Add(av);
            }
            return Json(forloopclasses.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductList(int categoryID, int WarehouseID)
        {
            List<Product> lProduct = new List<Product>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();
            //Yashaswi 9/4/2018
            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
            if (WarehouseID != EzeeloWarehouseId)
            {
                var fList = db.WarehouseFranchises.Where(x => x.WarehouseID == WarehouseID && x.IsActive == true).Select(x => x.FranchiseID).ToList();

                if (fList.Count > 0)
                {
                    var query = (from p in db.Products
                                 join sp in db.ShopProducts on p.ID equals sp.ProductID
                                 join s in db.Shops on sp.ShopID equals s.ID
                                 join f in db.Franchises on s.FranchiseID equals f.ID
                                 where (fList.Contains(f.ID)) && p.CategoryID == categoryID && p.IsActive == true
                                 && sp.IsActive == true && s.IsActive == true && f.IsActive == true
                                 select new { Name = p.Name, ID = p.ID }).Distinct();

                    lProduct = query.ToList().Select(p => new Product
                    {
                        ID = p.ID,
                        Name = p.Name
                    }).ToList();


                    //lProduct = db.Products.Where(x => x.CategoryID == categoryID && x.IsActive == true).ToList();

                    foreach (var c in lProduct)
                    {
                        ForLoopClass av = new ForLoopClass();
                        av.ID = c.ID;
                        av.Name = c.Name;
                        forloopclasses.Add(av);
                    }
                }
            }
            else
            {
                //var fList = db.WarehouseFranchises.Where(x => x.IsActive == true).Select(x => x.FranchiseID).ToList();

                //if (fList.Count > 0)
                //{
                var query = (from p in db.Products
                             join sp in db.ShopProducts on p.ID equals sp.ProductID
                             join s in db.Shops on sp.ShopID equals s.ID
                             join f in db.Franchises on s.FranchiseID equals f.ID
                             where p.CategoryID == categoryID && p.IsActive == true
                             && sp.IsActive == true && s.IsActive == true && f.IsActive == true
                             select new { Name = p.Name, ID = p.ID }).Distinct();

                lProduct = query.ToList().Select(p => new Product
                {
                    ID = p.ID,
                    Name = p.Name
                }).ToList();

                //lProduct = db.Products.Where(x => x.CategoryID == categoryID && x.IsActive == true).ToList();

                foreach (var c in lProduct)
                {
                    ForLoopClass av = new ForLoopClass();
                    av.ID = c.ID;
                    av.Name = c.Name;
                    forloopclasses.Add(av);
                }
                //}
            }
            return Json(forloopclasses.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductVarientList(int ProductID)
        {
            List<Product> lProduct = new List<Product>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();


            var query = (from vp in db.ProductVarients
                         join p in db.Products on vp.ProductID equals p.ID
                         join sp in db.ShopProducts on p.ID equals sp.ProductID
                         join sh in db.Shops on sp.ShopID equals sh.ID
                         join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                         join s in db.Sizes on vp.SizeID equals s.ID
                         where vp.ProductID == ProductID && sp.ProductID == ProductID && ss.ProductVarientID == vp.ID && vp.IsActive == true && s.IsActive == true
                         select new { VarientName = p.Name + " (" + s.Name + ")", ID = vp.ID }).Distinct();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

            lProduct = query.ToList().Select(p => new Product
            {
                ID = p.ID,
                Name = p.VarientName
            }).ToList();


            foreach (var c in lProduct)
            {
                ForLoopClass av = new ForLoopClass();
                av.ID = c.ID;
                av.Name = c.Name;
                forloopclasses.Add(av);
            }

            return Json(forloopclasses.ToList(), JsonRequestBehavior.AllowGet);
        }
        public string GetProductVarientDetail(int ProductVarientID)
        {

            //String daresult = null;
            DataTable dt = new DataTable();
            SqlConnection objcon = new SqlConnection(fConnectionString);
            SqlCommand objcmd = new SqlCommand("GetProductVarientDetail", objcon);
            objcmd.CommandType = CommandType.StoredProcedure;
            SqlParameter[] objParam = new SqlParameter[2];
            objParam[0] = new SqlParameter("@ProductVarientID", ProductVarientID);
            objParam[1] = new SqlParameter("@WarehouseID", Convert.ToInt64(Session["WarehouseID"].ToString()));

            int i = 0;
            for (i = 0; i < objParam.Length; i++)
                objcmd.Parameters.Add(objParam[i]);

            objcon.Open();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = objcmd;
            da.Fill(dt);

            objcon.Close();
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            //daresult = DataSetToJSON(ds);
            return ds.GetXml();
        }
        public string DataSetToJSON(DataSet ds)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (DataTable dt in ds.Tables)
            {
                object[] arr = new object[dt.Rows.Count + 1];

                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    arr[i] = dt.Rows[i].ItemArray;
                }

                dict.Add(dt.TableName, arr);
            }

            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(dict);
        }

        #endregion End Methods

        //Yashaswi 30/3/2018
        #region E-Mail

        public ActionResult SendMailForQuotationRequest(long ID)
        {
            try
            {
                Quotation obj_quotation = db.Quotations.SingleOrDefault(p => p.ID == ID);
                if (obj_quotation != null)
                {
                    var QuotationCode = obj_quotation.QuotationCode;
                    var QuotationDate = obj_quotation.QuotationRequestDate;
                    var QuotationReplyDate = obj_quotation.ExpectedReplyDate;
                    var WarehouseName = db.Warehouses.SingleOrDefault(p => p.ID == obj_quotation.RequestFromWarehouseID).Name;

                    var SupplierName = "";

                    List<QuotationSupplierList> list_QuotationSupplierLists = db.QuotationSupplierLists.Where(p => p.QuotationID == ID).ToList();
                    foreach (var item in list_QuotationSupplierLists)
                    {
                        var URI = @"" + WebConfigurationManager.AppSettings["INVENTORY_ROOT"] + "Quotation/Print?SupplierID=" + item.SupplierID + "&QuotationID=" + ID + "";
                        SupplierName = db.Suppliers.SingleOrDefault(p => p.ID == item.SupplierID).Name;
                        long userLoginId = 0;
                        if (Session["USER_LOGIN_ID"] != null)
                        {
                            userLoginId = Convert.ToInt64(Session["USER_LOGIN_ID"]);
                        }

                        Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                        ///<!--QUOTATION_NO-->,<!--SUPPLIER_NAME-->,<!--QUOTATION_DATE-->,<!--QUOTATION_REPLY_DATE-->,
                        ///<!--WAREHOUSE_NAME-->,<!--URL-->

                        dictEmailValues.Add("<!--QUOTATION_NO-->", QuotationCode);
                        dictEmailValues.Add("<!--SUPPLIER_NAME-->", SupplierName);
                        dictEmailValues.Add("<!--QUOTATION_DATE-->", QuotationDate.ToShortDateString());
                        dictEmailValues.Add("<!--QUOTATION_REPLY_DATE-->", QuotationReplyDate.ToShortDateString());
                        dictEmailValues.Add("<!--WAREHOUSE_NAME-->", WarehouseName);
                        dictEmailValues.Add("<!--URL-->", URI);

                        BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                        string EmailID = db.Suppliers.SingleOrDefault(p => p.ID == item.SupplierID).Email;

                        gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.QUOT_REQUEST, new string[] { EmailID, "sales@ezeelo.com" }, dictEmailValues, true);
                        Session["Success"] = "Mail Send Successfully."; //yashaswi 31/3/2018
                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[QuotationController][M:SendMailForQuotationRequest]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[QuotationController][M:SendMailForQuotationRequest]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            return RedirectToAction("Index");
        }
        #endregion
	}
}