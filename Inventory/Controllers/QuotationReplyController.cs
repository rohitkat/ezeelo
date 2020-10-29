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

namespace Inventory.Controllers
{
    public class QuotationReplyController : Controller
    {
        //
        // GET: /QuotationReply/
        //

        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController Obj_Common = new CommonController();
        //
        // GET: /QuotationReply/

        public ActionResult Index()
        {
            QuotationViewModelList objQ = new QuotationViewModelList();
            if (Session["USER_NAME"] != null)
            {

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
                  
            List<QuotationViewModel> lQuotationViewModel = new List<QuotationViewModel>();
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID && x.IsActive==true).Select(x=>x.ID).FirstOrDefault();

            if (SupplierID != null && Convert.ToInt64(SupplierID) > 0)
            {
                lQuotationViewModel = (from o in db.Quotations
                                       join sl in db.QuotationSupplierLists on o.ID equals sl.QuotationID
                                       join w in db.Warehouses on o.RequestFromWarehouseID equals w.ID
                                       where sl.SupplierID == SupplierID && sl.IsActive==true
                                       select new QuotationViewModel
                                       {
                                           QuotationID = o.ID,
                                           WarehouseName = w.Name,
                                           QuotationCode = o.QuotationCode,
                                           IsReplied = sl.IsReplied,
                                           TotalItems = db.QuotationItemDetails.Where(x => x.QuotationID == o.ID).Select(x => x.ID).Count(),
                                           QuotationRequestDate = o.QuotationRequestDate,
                                           ExpectedReplyDate = o.ExpectedReplyDate,
                                           ReplyItemCount = db.QuotationReplyItems.Where(x => x.QuotationSupplierListID == sl.ID).Select(x => x.ID).Count(),
                                           QuotationSupplierListID = sl.ID,
                                           SupplierID = sl.SupplierID
                                       }).OrderByDescending(o => o.QuotationRequestDate).OrderBy(sl=>sl.IsReplied).ToList();               
            }
                        
            Session["lQuotationItemDetailViewModel"] = null;
            objQ.lQuotationViewModel = lQuotationViewModel.ToList();
            //yashaswi 9/4/2018          
            ViewBag.PossibleWarehouses = Obj_Common.GetFVList(WarehouseID);

            return View("Index", objQ);
        }

        public ActionResult Index1(long? RequestFromWarehouseID)
        {
            QuotationViewModelList objQ = new QuotationViewModelList();
            if (Session["USER_NAME"] != null)
            {

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

            List<QuotationViewModel> lQuotationViewModel = new List<QuotationViewModel>();
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID && x.IsActive == true).Select(x => x.ID).FirstOrDefault();

            if (SupplierID != null && Convert.ToInt64(SupplierID) > 0)
            {
                if (RequestFromWarehouseID != null && RequestFromWarehouseID > 0)
                {
                    lQuotationViewModel = (from o in db.Quotations
                                           join sl in db.QuotationSupplierLists on o.ID equals sl.QuotationID
                                           join w in db.Warehouses on o.RequestFromWarehouseID equals w.ID
                                           where sl.SupplierID == SupplierID && sl.IsActive == true
                                           && o.RequestFromWarehouseID == RequestFromWarehouseID
                                           select new QuotationViewModel
                                           {
                                               QuotationID = o.ID,
                                               WarehouseName = w.Name,
                                               QuotationCode = o.QuotationCode,
                                               IsReplied = sl.IsReplied,
                                               TotalItems = db.QuotationItemDetails.Where(x => x.QuotationID == o.ID).Select(x => x.ID).Count(),
                                               QuotationRequestDate = o.QuotationRequestDate,
                                               ExpectedReplyDate = o.ExpectedReplyDate,
                                               ReplyItemCount = db.QuotationReplyItems.Where(x => x.QuotationSupplierListID == sl.ID).Select(x => x.ID).Count(),
                                               QuotationSupplierListID = sl.ID,
                                               SupplierID = sl.SupplierID
                                           }).OrderByDescending(o => o.QuotationRequestDate).OrderBy(sl => sl.IsReplied).ToList();
                }
                else
                {                                       
                    return RedirectToAction("Index");
                }
            }


            Session["lQuotationItemDetailViewModel"] = null;
            objQ.lQuotationViewModel = lQuotationViewModel.ToList();
            ViewBag.PossibleWarehouses = db.Warehouses.Where(x => x.ID != WarehouseID);
            //ViewBag.PossibleWarehouses = new SelectList(db.Warehouses.Where(x => x.ID != WarehouseID), "WarehouseID", "Name", RequestFromWarehouseID);

            return View("Index", objQ);
        }

        //
        // GET: /QuotationReply/
        public ViewResult View(long id)
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
                                      Remark = po.Remark                                    
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
        // GET: /QuotationReply/Details/5

        public ViewResult Details(long id)
        {
           
            QuotationViewModel QuotationViewModel = new QuotationViewModel();

            Session["lQuotationItemDetailViewModel"] = null;

            int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
            var Quotation = db.Quotations.Single(x => x.ID == id);
            var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID && x.IsActive == true).Select(x => x.ID).FirstOrDefault();
            QuotationSupplierList QuotationSupplierListID = db.QuotationSupplierLists.Where(x => x.QuotationID == id && x.SupplierID == SupplierID && x.IsActive == true).FirstOrDefault();

            if (Quotation != null && Quotation.ToString() != "")
            {
                QuotationViewModel.QuotationID = id;
                QuotationViewModel.IsReplied = true;
                QuotationViewModel.SupplierID = SupplierID;
                QuotationViewModel.QuotationSupplierListID = QuotationSupplierListID.ID;
                QuotationViewModel.Amount = Convert.ToDecimal(QuotationSupplierListID.Amount);
                QuotationViewModel.ShippingCharge = QuotationSupplierListID.ShippingCharge;
                QuotationViewModel.AdditionalCost = QuotationSupplierListID.AdditionalCost;
                QuotationViewModel.Remark = QuotationSupplierListID.Remark;
                QuotationViewModel.QuotationCode = Quotation.QuotationCode;
                QuotationViewModel.QuotationReplyDate = QuotationSupplierListID.QuotationReplyDate;
                QuotationViewModel.ShippingCharge = QuotationSupplierListID.ShippingCharge;
                QuotationViewModel.AdditionalCost = QuotationSupplierListID.AdditionalCost;
            }
             
                List<QuotationItemDetailViewModel> lQuotationItemDetailViewModel = new List<QuotationItemDetailViewModel>();
            
                List<QuotationReplyItem> lQuotationReplyItemlist = new List<QuotationReplyItem>();
                lQuotationReplyItemlist = db.QuotationReplyItems.Where(x => x.QuotationID == id && x.ReplyFromSupplierID == SupplierID).ToList();

                foreach (QuotationReplyItem item in lQuotationReplyItemlist)
                {
                    QuotationItemDetailViewModel objPOD = new QuotationItemDetailViewModel();
                    objPOD.ItemID = item.ID;
                    objPOD.QuotationID = item.QuotationID;
                    objPOD.ProductID = item.ProductID;
                    objPOD.ProductVarientID = Convert.ToInt64(item.ProductVarientID);
                    objPOD.Quantity = item.Quantity;
                    objPOD.Nickname = item.ProductNickname;
                    objPOD.UnitPrice = item.UnitPrice;
                    objPOD.CGSTAmount = item.CGSTAmount;
                    objPOD.SGSTAmount = item.SGSTAmount;
                    objPOD.IGSTAmount = item.IGSTAmount;
                    objPOD.ProductAmount = item.Amount;
                    objPOD.ProductRemark = item.Remark == null ? "" : item.Remark;

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

                    lQuotationItemDetailViewModel.Add(objPOD);
                }

                if (lQuotationItemDetailViewModel.Count > 0)
            {
                Session["lQuotationItemDetailViewModel"] = lQuotationItemDetailViewModel;
                QuotationViewModel.lQuotationItemDetailViewModels = lQuotationItemDetailViewModel;
            }
            ViewBag.QuotationID = id;
            return View(QuotationViewModel);
        }

        //
        // QuotationReply/InsertUpdateQuotationSupplierList       

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

            Session["lQuotationItemDetailViewModel"] = null;

            int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
            var Quotation = db.Quotations.Single(x => x.ID == id);
            var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID && x.IsActive==true).Select(x=>x.ID).FirstOrDefault();
            QuotationSupplierList QuotationSupplierListID = db.QuotationSupplierLists.Where(x => x.QuotationID == id && x.SupplierID == SupplierID && x.IsActive == true).FirstOrDefault();
            
            if (Quotation != null && Quotation.ToString() != "")
            {
                QuotationViewModel.QuotationID = id;
                QuotationViewModel.IsReplied = db.QuotationSupplierLists.Where(x => x.QuotationID == id && x.SupplierID == SupplierID && x.IsActive==true).Select(x => x.IsReplied).FirstOrDefault();
                QuotationViewModel.SupplierID = SupplierID;
                QuotationViewModel.QuotationSupplierListID = QuotationSupplierListID.ID;
                QuotationViewModel.Amount = Convert.ToDecimal(QuotationSupplierListID.Amount);
                QuotationViewModel.ShippingCharge = QuotationSupplierListID.ShippingCharge;
                QuotationViewModel.AdditionalCost = QuotationSupplierListID.AdditionalCost;
                QuotationViewModel.Remark = QuotationSupplierListID.Remark;
                
            }

            var CheckRecordExists = db.QuotationReplyItems.Where(x => x.QuotationSupplierListID == QuotationSupplierListID.ID).Select(x => x.ID).FirstOrDefault();

            List<QuotationItemDetailViewModel> lQuotationItemDetailViewModel = new List<QuotationItemDetailViewModel>();
           
            
            if (CheckRecordExists != null && Convert.ToInt32(CheckRecordExists) > 0)
            {                
                List<QuotationReplyItem> lQuotationReplyItemlist = new List<QuotationReplyItem>();
                lQuotationReplyItemlist = db.QuotationReplyItems.Where(x => x.QuotationID == id && x.ReplyFromSupplierID == SupplierID).ToList();

                foreach (QuotationReplyItem item in lQuotationReplyItemlist)
                {
                    QuotationItemDetailViewModel objPOD = new QuotationItemDetailViewModel();
                    objPOD.ItemID = item.ID;
                    objPOD.QuotationID = item.QuotationID;
                    objPOD.ProductID = item.ProductID;
                    objPOD.ProductVarientID = Convert.ToInt64(item.ProductVarientID);
                    objPOD.Quantity = item.Quantity;
                    objPOD.Nickname = item.ProductNickname;
                    objPOD.UnitPrice = item.UnitPrice;
                    objPOD.MRP = item.MRP;
                    objPOD.SaleRate = item.SaleRate;
                    objPOD.CGSTAmount = item.CGSTAmount;
                    objPOD.SGSTAmount = item.SGSTAmount;
                    objPOD.IGSTAmount = item.IGSTAmount;
                    objPOD.ProductAmount = item.Amount;
                    objPOD.ProductRemark = item.Remark;

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

                    lQuotationItemDetailViewModel.Add(objPOD);
                }       
            }
            else
            {
                List<QuotationItemDetail> lQuotationItemDetaillist = new List<QuotationItemDetail>();
                lQuotationItemDetaillist = db.QuotationItemDetails.Where(x => x.QuotationID == id).ToList();

                foreach (QuotationItemDetail item in lQuotationItemDetaillist)
                {
                    QuotationItemDetailViewModel objPOD = new QuotationItemDetailViewModel();
                    objPOD.ItemID = item.ID;
                    objPOD.QuotationID = item.QuotationID;
                    objPOD.ProductID = item.ProductID;
                    objPOD.ProductVarientID = Convert.ToInt64(item.ProductVarientID);
                    objPOD.Quantity = item.Quantity;
                    objPOD.Nickname = item.ProductNickname;
                    objPOD.UnitPrice = 0;
                    objPOD.MRP = 0;
                    objPOD.SaleRate = 0;
                    objPOD.CGSTAmount = 0;
                    objPOD.SGSTAmount = 0;
                    objPOD.IGSTAmount = 0;
                    objPOD.ProductAmount = 0;
                    objPOD.ProductRemark = "";

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

                    lQuotationItemDetailViewModel.Add(objPOD);
                }       
            }
                     
            if (lQuotationItemDetailViewModel.Count > 0)
            {
                Session["lQuotationItemDetailViewModel"] = lQuotationItemDetailViewModel;
                QuotationViewModel.lQuotationItemDetailViewModels = lQuotationItemDetailViewModel;
            }                

            ViewBag.QuotationID = id;
            return View(QuotationViewModel);
        }

        //
        // POST: /Quotation/Edit/5

        [HttpPost]
        public ActionResult Edit(QuotationViewModel Quotation)
        {
            List<QuotationItemDetailViewModel> lQuotationItemDetailViewModel = new List<QuotationItemDetailViewModel>();
            int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
            if (Session["lQuotationItemDetailViewModel"] != null)
            {
                lQuotationItemDetailViewModel = Quotation.lQuotationItemDetailViewModels;
               
                if (lQuotationItemDetailViewModel.Count > 0)
                {
                    if (ModelState.IsValid)
                    {
                        using (TransactionScope tscope = new TransactionScope())
                        {
                            try
                            {
                                if (Quotation.Amount > 0)
                                {
                                var CheckRecordExists = db.QuotationReplyItems.Where(x => x.QuotationSupplierListID == Quotation.QuotationSupplierListID).Select(x => x.ID).FirstOrDefault();
                             
                               
                                QuotationReplyItem objQuotationReplyItem = new QuotationReplyItem();
                                decimal TotalGSTAmount = 0;
                                if (CheckRecordExists != null && Convert.ToInt64(CheckRecordExists) > 0)
                                {
                                    foreach (QuotationItemDetailViewModel item in lQuotationItemDetailViewModel)
                                    {
                                        TotalGSTAmount = TotalGSTAmount + Convert.ToDecimal(item.CGSTAmount) + Convert.ToDecimal(item.SGSTAmount) + Convert.ToDecimal(item.IGSTAmount);
                                        //Update QuotationReplyItem
                                        var lQuotationReplyItem = new QuotationReplyItem()
                                        {
                                        ID = item.ItemID,
                                        QuotationID = Quotation.QuotationID,
                                        QuotationSupplierListID = Quotation.QuotationSupplierListID,
                                        ReplyFromSupplierID = Quotation.SupplierID,
                                        ReplyFromWarehouseID = WarehouseID,
                                        ProductID = item.ProductID,
                                        ProductNickname = item.Nickname,
                                        ProductVarientID = item.ProductVarientID,
                                        Quantity = item.Quantity,
                                        UnitPrice = item.UnitPrice,
                                        MRP = item.MRP,
                                        SaleRate = item.SaleRate,
                                        CGSTAmount = item.CGSTAmount,
                                        SGSTAmount = item.SGSTAmount,
                                        IGSTAmount = item.IGSTAmount,
                                        Amount = item.ProductAmount,
                                        Remark = item.ProductRemark
                                        };

                                        db.QuotationReplyItems.Attach(lQuotationReplyItem);
                                        db.Entry(lQuotationReplyItem).Property(x => x.UnitPrice).IsModified = true;
                                        db.Entry(lQuotationReplyItem).Property(x => x.MRP).IsModified = true;
                                        db.Entry(lQuotationReplyItem).Property(x => x.SaleRate).IsModified = true;
                                        db.Entry(lQuotationReplyItem).Property(x => x.CGSTAmount).IsModified = true;
                                        db.Entry(lQuotationReplyItem).Property(x => x.SGSTAmount).IsModified = true;
                                        db.Entry(lQuotationReplyItem).Property(x => x.IGSTAmount).IsModified = true;
                                        db.Entry(lQuotationReplyItem).Property(x => x.Amount).IsModified = true;
                                        db.Entry(lQuotationReplyItem).Property(x => x.Remark).IsModified = true;                                        
                                        db.SaveChanges();
                                    }
                                }
                                else  //Insert into QuotationReplyItem Table
                                {
                                    foreach (QuotationItemDetailViewModel item in lQuotationItemDetailViewModel)
                                    {
                                        TotalGSTAmount = TotalGSTAmount + Convert.ToDecimal(item.CGSTAmount) + Convert.ToDecimal(item.SGSTAmount) + Convert.ToDecimal(item.IGSTAmount);

                                        objQuotationReplyItem.QuotationID = Quotation.QuotationID;
                                        objQuotationReplyItem.QuotationSupplierListID = Quotation.QuotationSupplierListID;
                                        objQuotationReplyItem.ReplyFromSupplierID = Quotation.SupplierID;
                                        objQuotationReplyItem.ReplyFromWarehouseID = WarehouseID;
                                        objQuotationReplyItem.ProductID = item.ProductID;
                                        objQuotationReplyItem.ProductNickname = item.Nickname;
                                        objQuotationReplyItem.ProductVarientID = item.ProductVarientID;
                                        objQuotationReplyItem.Quantity = item.Quantity;
                                        objQuotationReplyItem.UnitPrice = item.UnitPrice;
                                        objQuotationReplyItem.MRP = item.MRP;
                                        objQuotationReplyItem.SaleRate = item.SaleRate;
                                        objQuotationReplyItem.CGSTAmount = item.CGSTAmount;
                                        objQuotationReplyItem.SGSTAmount = item.SGSTAmount;
                                        objQuotationReplyItem.IGSTAmount = item.IGSTAmount;
                                        objQuotationReplyItem.Amount = item.ProductAmount;
                                        objQuotationReplyItem.Remark = item.ProductRemark;
                                        db.QuotationReplyItems.Add(objQuotationReplyItem);
                                        db.SaveChanges();
                                    }
                                }
                                           

                                //Update QuotationSupplierList
                                var lQuotation = new QuotationSupplierList()
                                {
                                    ID = Quotation.QuotationSupplierListID,
                                    QuotationID = Quotation.QuotationID,
                                    SupplierID = Quotation.SupplierID,
                                    IsReplied = Quotation.IsReplied,  
                                    QuotationReplyDate = System.DateTime.Now,
                                    Amount = Quotation.Amount,
                                    GSTAmount = TotalGSTAmount,     
                                    ShippingCharge = Convert.ToDecimal(Quotation.ShippingCharge),
                                    AdditionalCost = Convert.ToDecimal(Quotation.AdditionalCost),
                                    TotalAmount = Convert.ToDecimal(Quotation.Amount) + Convert.ToDecimal(Quotation.ShippingCharge)+Convert.ToDecimal(Quotation.AdditionalCost),
                                    RepliedBy = GetPersonalDetailID(),
                                    ModifyBy=GetPersonalDetailID(),
                                    ModifyDate = DateTime.Now,
                                    Remark = Quotation.Remark
                                };

                                db.QuotationSupplierLists.Attach(lQuotation);
                                db.Entry(lQuotation).Property(x => x.IsReplied).IsModified = true;                                
                                db.Entry(lQuotation).Property(x => x.Amount).IsModified = true;
                                db.Entry(lQuotation).Property(x => x.GSTAmount).IsModified = true;
                                db.Entry(lQuotation).Property(x => x.ShippingCharge).IsModified = true;
                                db.Entry(lQuotation).Property(x => x.AdditionalCost).IsModified = true;
                                db.Entry(lQuotation).Property(x => x.TotalAmount).IsModified = true;
                                if(Quotation.IsReplied==true)
                                {
                                    db.Entry(lQuotation).Property(x => x.RepliedBy).IsModified = true;
                                    db.Entry(lQuotation).Property(x => x.QuotationReplyDate).IsModified = true;
                                }
                                db.Entry(lQuotation).Property(x => x.ModifyBy).IsModified = true;
                                db.Entry(lQuotation).Property(x => x.ModifyDate).IsModified = true;
                                db.Entry(lQuotation).Property(x => x.Remark).IsModified = true;    
                                db.SaveChanges();             

                                tscope.Complete();
                                Session["Success"] = "Price Quotation Created Successfully."; //Yashaswi 30/3/2018
                                }
                                     else
                        {
                            Session["Error"] = "Total amount should be greater than 0";
                            return View(Quotation);
                        }
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

            return View(Quotation);
        }

        public ActionResult Send(long QuotationSupplierListID)
        {
            using (TransactionScope tscope = new TransactionScope())
            {
                try
                {
                    //Update QuotationSupplierList
                    var lQuotationSupplierList = new QuotationSupplierList()
                    {
                        ID = QuotationSupplierListID,
                        IsReplied = true,
                        RepliedBy = GetPersonalDetailID(),
                        QuotationReplyDate = System.DateTime.Now
                    };

                    db.QuotationSupplierLists.Attach(lQuotationSupplierList);
                    db.Entry(lQuotationSupplierList).Property(x => x.IsReplied).IsModified = true;
                    db.Entry(lQuotationSupplierList).Property(x => x.RepliedBy).IsModified = true;
                    db.Entry(lQuotationSupplierList).Property(x => x.QuotationReplyDate).IsModified = true;
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

        
        //
        // GET: /Quotation/Print/5
        public ViewResult Print(long QuotationID, long SupplierID)
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
                                      Remark = s.Remark,   //Yashaswi 31/3/2018
                                      IsActive = po.IsActive,
                                      SupplierCode = sp.SupplierCode,
                                      SupplierContactPerson = sp.ContactPerson,
                                      SupplierAddress = sp.Address,
                                      SupplierMobile = sp.Mobile,
                                      SupplierFax = sp.FAX,
                                      SupplierEmail = sp.Email,
                                      SupplierGSTNumber = sp.GSTNumber,
                                      WarehouseContactPerson = b.ContactPerson,
                                      WarehouseAddress = b.Address,
                                      WarehouseMobile = b.Mobile,
                                      WarehosueEmail = b.Email,
                                      WarehouseGSTNumber = w.GSTNumber,
                                      WarehouseFax = b.FAX,
                                      QuotationReplyDate = s.QuotationReplyDate,
                                      Amount = s.Amount,
                                      ShippingCharge = s.ShippingCharge,
                                      AdditionalCost = s.AdditionalCost,
                                      GSTAmount = s.GSTAmount,
                                      TotalAmount = s.TotalAmount
                                  }).FirstOrDefault();
                     

            List<QuotationItemDetailViewModel> lQuotationItemDetailViewModel = new List<QuotationItemDetailViewModel>();

            List<QuotationReplyItem> lQuotationReplyItemlist = new List<QuotationReplyItem>();
            lQuotationReplyItemlist = db.QuotationReplyItems.Where(x => x.QuotationID == QuotationID && x.ReplyFromSupplierID == SupplierID).ToList();

            foreach (QuotationReplyItem item in lQuotationReplyItemlist)
            {
                QuotationItemDetailViewModel objPOD = new QuotationItemDetailViewModel();
                objPOD.ItemID = item.ID;
                objPOD.QuotationID = item.QuotationID;
                objPOD.ProductID = item.ProductID;
                objPOD.ProductVarientID = Convert.ToInt64(item.ProductVarientID);
                objPOD.Quantity = item.Quantity;
                objPOD.MRP = item.MRP; //30/5/2018
                objPOD.Nickname = item.ProductNickname;
                objPOD.UnitPrice = item.UnitPrice;
                objPOD.CGSTAmount = item.CGSTAmount;
                objPOD.SGSTAmount = item.SGSTAmount;
                objPOD.IGSTAmount = item.IGSTAmount;
                objPOD.ProductAmount = item.Amount;
                objPOD.ProductRemark = item.Remark == null ? "" : item.Remark;

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

                lQuotationItemDetailViewModel.Add(objPOD);
            }

            if (lQuotationItemDetailViewModel.Count > 0)
            {                
                QuotationViewModel.lQuotationItemDetailViewModels = lQuotationItemDetailViewModel;
            }

            ViewBag.QuotationID = QuotationID;
            return View(QuotationViewModel);
        }


        public ActionResult PrintList()
        {
            if (Session["USER_NAME"] != null)
            {

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

            QuotationViewModel objQ = new QuotationViewModel();
            List<QuotationSupplierListViewModel> lQuotationSupplierListViewModel = new List<QuotationSupplierListViewModel>();
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            lQuotationSupplierListViewModel = (from o in db.Quotations
                                               join qs in db.QuotationSupplierLists on o.ID equals qs.QuotationID
                                               join s in db.Suppliers on qs.SupplierID equals s.ID
                                               where o.RequestFromWarehouseID == WarehouseID && qs.IsReplied==true
                                               select new QuotationSupplierListViewModel
                                               {
                                                   QuotationID = o.ID,
                                                   QuotationCode = o.QuotationCode,
                                                   IsReplied = qs.IsReplied,
                                                   TotalItems = db.QuotationItemDetails.Where(x => x.QuotationID == o.ID).Select(x => x.ID).Count(),
                                                   QuotationRequestDate = o.QuotationRequestDate,
                                                   QuotationReplyDate = qs.QuotationReplyDate,
                                                   SupplierID = qs.SupplierID,
                                                   SupplierName = s.Name
                                               }).OrderByDescending(o=>o.QuotationID).ToList();

            Session["lQuotationItemDetailViewModel"] = null;

            //var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID).Select(x => x.ID).FirstOrDefault();
            //Yashaswi 9/4/2018
            ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
            objQ.lQuotationSupplierListViewModel = lQuotationSupplierListViewModel.ToList();

            return View("PrintList", objQ);
        }

        public ActionResult Search(long? SupplierID)
        {
            QuotationViewModel objQ = new QuotationViewModel();
            try
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
                //var wSupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID).Select(x => x.ID).FirstOrDefault();

                if (SupplierID != null && SupplierID > 0)
                {
                    lQuotationSupplierListViewModel = (from o in db.Quotations
                                                       join qs in db.QuotationSupplierLists on o.ID equals qs.QuotationID
                                                       join s in db.Suppliers on qs.SupplierID equals s.ID
                                                       where o.RequestFromWarehouseID == WarehouseID && qs.IsReplied == true
                                                       && qs.SupplierID == SupplierID
                                                       select new QuotationSupplierListViewModel
                                                       {
                                                           QuotationID = o.ID,
                                                           QuotationCode = o.QuotationCode,
                                                           IsReplied = qs.IsReplied,
                                                           TotalItems = db.QuotationItemDetails.Where(x => x.QuotationID == o.ID).Select(x => x.ID).Count(),
                                                           QuotationRequestDate = o.QuotationRequestDate,
                                                           QuotationReplyDate = qs.QuotationReplyDate,
                                                           SupplierID = qs.SupplierID,
                                                           SupplierName = s.Name
                                                       }).OrderByDescending(o => o.QuotationID).ToList();
                }
                else
                {                    
                    return RedirectToAction("PrintList");
                }

                Session["lQuotationItemDetailViewModel"] = null;

                //Yashaswi 9/4/2018
                ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
                objQ.lQuotationSupplierListViewModel = lQuotationSupplierListViewModel.ToList();
            }
            catch (Exception)
            {
                //Transaction.Current.Rollback();
                //tscope.Dispose();
            }
            return View("PrintList", objQ);
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
	}
}