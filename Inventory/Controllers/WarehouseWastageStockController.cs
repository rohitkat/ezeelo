using BusinessLogicLayer;
using Inventory.Common;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Inventory.Controllers
{
    public class WarehouseWastageStockController : Controller
    {

        private EzeeloDBContext db = new EzeeloDBContext();
        public WarehouseStockViewModel GetAllItems()
        {
            WarehouseStockViewModel objWS = new WarehouseStockViewModel();
            List<WarehouseReorderLevel> lWarehouseReorderLevel = new List<WarehouseReorderLevel>();
            List<WarehouseReorderLevelViewModel> objWRLVM = new List<WarehouseReorderLevelViewModel>();

            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            if (WarehouseID > 0)
            {
                lWarehouseReorderLevel = db.WarehouseReorderLevels.Where(x => x.WarehouseID == WarehouseID).OrderByDescending(x => x.AvailableQuantity).ToList();
                objWS.WarehouseName = db.Warehouses.Where(x => x.ID == WarehouseID).Select(x => x.Name).FirstOrDefault();
                var lWarehouseStocks = db.WarehouseStocks.Where(x => x.WarehouseID == WarehouseID).ToList();//Added by Sonali for Available stock qty on 18-03-2019
                foreach (var item in lWarehouseReorderLevel)
                {
                    WarehouseReorderLevelViewModel objPOD = new WarehouseReorderLevelViewModel();
                    objPOD.ID = item.ID;
                    objPOD.AvailableQuantity = lWarehouseStocks.Where(x => x.ProductVarientID == item.ProductVarientID).Select(x => x.AvailableQuantity).Sum();//Added by Sonali for Available stock qty on 18-03-2019
                    objPOD.ReorderLevel = item.ReorderLevel;
                    objPOD.ProductID = item.ProductID;
                    objPOD.ProductVarientID = Convert.ToInt64(item.ProductVarientID);

                    objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                    var itemName = (from p in db.Products
                                    join v in db.ProductVarients on p.ID equals v.ProductID
                                    join s in db.Sizes on v.SizeID equals s.ID
                                    where v.ID == item.ProductVarientID
                                    select new InvoiceDetailViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

                    foreach (var i in itemName)
                    {
                        objPOD.ItemName = i.ItemName.ToString();
                        objPOD.HSNCode = Convert.ToString(i.HSNCode);
                    }
                    objWRLVM.Add(objPOD);
                }

            }


            objWS.lWarehouseReorderLevelViewModel = objWRLVM;


            return objWS;
        }
        public ActionResult AllItems()
        {
            if (Session["USER_NAME"] != null)
            {
                return View("AllItems", GetAllItems());
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }


        }
        public List<WarehouseStockViewModel> GetItemBacthwise(long ID)
        {
            WarehouseStockViewModel objWS = new WarehouseStockViewModel();
            List<WarehouseStock> lWarehouseStock = new List<WarehouseStock>();
            List<WarehouseStockViewModel> objWRLVM = new List<WarehouseStockViewModel>();

            var query = db.WarehouseReorderLevels.Where(x => x.ID == ID).FirstOrDefault();
            objWS.WarehouseName = db.Warehouses.Where(x => x.ID == query.WarehouseID).Select(x => x.Name).FirstOrDefault();

            //Get all batches of product from stock
            lWarehouseStock = db.WarehouseStocks.Where(x => x.WarehouseID == query.WarehouseID && x.ProductID == query.ProductID && x.ProductVarientID == query.ProductVarientID && x.AvailableQuantity > 0).OrderBy(x => x.AvailableQuantity).ToList();

            if (query.WarehouseID > 0)
            {

                foreach (var item in lWarehouseStock)
                {
                    WarehouseStockViewModel objWarehouseStockViewModel = new WarehouseStockViewModel();
                    objWarehouseStockViewModel.ID = item.ID;
                    objWarehouseStockViewModel.BatchCode = item.BatchCode;
                    objWarehouseStockViewModel.ProductID = item.ProductID;
                    objWarehouseStockViewModel.ProductVarientID = Convert.ToInt64(item.ProductVarientID);
                    objWarehouseStockViewModel.MRP = Convert.ToDecimal(item.MRP);
                    objWarehouseStockViewModel.BuyRatePerUnit = item.BuyRatePerUnit;
                    objWarehouseStockViewModel.SaleRatePerUnit = Convert.ToDecimal(item.SaleRatePerUnit);
                    objWarehouseStockViewModel.InitialQuantity = item.InitialQuantity;
                    objWarehouseStockViewModel.AvailableQuantity = item.AvailableQuantity;
                    objWarehouseStockViewModel.ExpiryDate = item.ExpiryDate;


                    objWarehouseStockViewModel.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                    var itemName = (from p in db.Products
                                    join v in db.ProductVarients on p.ID equals v.ProductID
                                    join s in db.Sizes on v.SizeID equals s.ID
                                    where v.ID == item.ProductVarientID
                                    select new InvoiceDetailViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

                    foreach (var i in itemName)
                    {
                        objWarehouseStockViewModel.ItemName = i.ItemName.ToString();
                        objWarehouseStockViewModel.HSNCode = Convert.ToString(i.HSNCode);
                        ViewBag.ItemName = objWarehouseStockViewModel.ItemName;
                    }
                    objWRLVM.Add(objWarehouseStockViewModel);
                }
            }

            return objWRLVM;
        }

        public ActionResult ItemBatchwise(long ID, string imgproductimage, string ItemName, string AvailableQuantity, string ReorderLevel)
        {

            if (Session["USER_NAME"] != null)
            {
                ViewBag.imgproductimage = imgproductimage;
                ViewBag.ItemName = ItemName;
                ViewBag.AvailableQuantity = AvailableQuantity;
                ViewBag.ReorderLevel = ReorderLevel;
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

            return View("ItemBatchwise", GetItemBacthwise(ID));
        }

        [HttpGet]
        public ActionResult ItemWastageEntry(long ID, string imgproductimage, string ItemName, string AvailableQuantity, string ReorderLevel, string BatchCode, decimal BuyRatePerUnit, int InitialQuantity, int BatchAvailableQuantity, DateTime? ExpiryDate)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            WastageEntry objwastageEntry = new WastageEntry();
            //Get Item Current Location

            objwastageEntry.id = ID;
            objwastageEntry.item_current_location = "";
            objwastageEntry.item_image_Path = imgproductimage;
            objwastageEntry.item_name = ItemName;
            objwastageEntry.reason = 0;
            objwastageEntry.MainReasonlist = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId == null && p.IsActive && p.ParentCategoryId == (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.WASTAGE).ToList(), "Id", "Reason", objwastageEntry.reason);
            objwastageEntry.new_item_location = 0;
            objwastageEntry.reLocate = false;
            objwastageEntry.Remark = "";
            objwastageEntry.reorder_level = Convert.ToInt16(ReorderLevel);
            objwastageEntry.sub_reason = 0;
            objwastageEntry.SubReasonlist = new SelectList(db.WarehouseReasons.Where(p => p.ID == -1).ToList(), "Id", "Reason", objwastageEntry.sub_reason); //use -1 to get null
            objwastageEntry.total_amt = 0;
            objwastageEntry.total_item_qty_in_stock = Convert.ToInt16(AvailableQuantity);
            objwastageEntry.wastage_imgae_path = "";
            objwastageEntry.waste_qty = 0;
            objwastageEntry.batch_code = BatchCode;
            objwastageEntry.batch_avl_qty = BatchAvailableQuantity;
            objwastageEntry.batch_qty = InitialQuantity;
            objwastageEntry.buy_rate_per_unit = BuyRatePerUnit;
            objwastageEntry.expiry_date = ExpiryDate;

            //yashaswi 19/3/2018
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }
            WarehouseStock obj1 = db.WarehouseStocks.FirstOrDefault(p => p.ID == ID && p.WarehouseID == WarehouseID);
            WarehouseReorderLevel obj2 = db.WarehouseReorderLevels.FirstOrDefault(p => p.ProductID == obj1.ProductID && p.ProductVarientID == obj1.ProductVarientID && p.WarehouseID == obj1.WarehouseID);
            objwastageEntry.item_id = obj2.ID;
            //

            return View(objwastageEntry);
        }



        [HttpPost]
        public ActionResult ItemWastageEntry(WastageEntry objwastageEntry, HttpPostedFileBase file)
        {
            try
            {

                long ShopStock_Qty = -1;
                long WarehouseStockID = 0;
                long ShopStockID = 0;
                if (Session["USER_NAME"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (Session["USER_LOGIN_ID"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    if (objwastageEntry.waste_qty == 0)
                    {
                        Session["Warning"] = "Please insert wastage qty."; //yashaswi 31/3/2018
                        ModelState.AddModelError("waste_qty", "Please insert wastage qty");
                        objwastageEntry.MainReasonlist = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId == null && p.IsActive  && p.ParentCategoryId == (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.WASTAGE).ToList(), "Id", "Reason", objwastageEntry.reason); //Yashaswi 11/5/2018
                        objwastageEntry.SubReasonlist = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId == objwastageEntry.reason).ToList(), "Id", "Reason", objwastageEntry.sub_reason);
                        return View(objwastageEntry);
                    }
                    if (objwastageEntry.reason == 0)
                    {
                        Session["Warning"] = "Please select reason."; //yashaswi 31/3/2018
                        ModelState.AddModelError("reason", "Please select reason");
                        objwastageEntry.MainReasonlist = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId == null && p.IsActive && p.ParentCategoryId == (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.WASTAGE).ToList(), "Id", "Reason", objwastageEntry.reason); //Yashaswi 11/5/2018
                        objwastageEntry.SubReasonlist = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId == objwastageEntry.reason).ToList(), "Id", "Reason", objwastageEntry.sub_reason);
                        return View(objwastageEntry);
                    }
                    if (objwastageEntry.sub_reason == 0)
                    {
                        Session["Warning"] = "Please select Sub-reason."; //yashaswi 31/3/2018
                        ModelState.AddModelError("sub_reason", "Please select Sub-reason");
                        objwastageEntry.MainReasonlist = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId == null && p.IsActive && p.ParentCategoryId == (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.WASTAGE).ToList(), "Id", "Reason", objwastageEntry.reason); //Yashaswi 11/5/2018
                        objwastageEntry.SubReasonlist = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId == objwastageEntry.reason).ToList(), "Id", "Reason", objwastageEntry.sub_reason);
                        return View(objwastageEntry);
                    }
                    //var path = "";
                    decimal count = db.Database.SqlQuery<decimal>(@"SELECT IDENT_CURRENT ({0}) AS Current_Identity;", "WarehouseWastageStock").FirstOrDefault<decimal>();
                    if (count != 1)
                    {
                        count = count + 1;
                    }
                    CommonController obj_CommonController = new CommonController();
                    var allowedExtensions = new[] {
                    ".Jpg", ".png", ".jpg", "jpeg"
                    };
                    bool IsSaved = false;
                    string Filename = "";
                    string Ext = "";
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-ganesh.jpg)  
                        var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                        if (allowedExtensions.Contains(ext)) //check what type of extension  
                        {
                            IsSaved = obj_CommonController.UploadImage((int)Constants.Inventory_Image_Type.WASTAGE, file, (long)count, System.Web.HttpContext.Current.Server, out Filename, out Ext);

                        }
                        else
                        {
                            objwastageEntry.MainReasonlist = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId == null && p.IsActive && p.ParentCategoryId == (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.WASTAGE).ToList(), "Id", "Reason", objwastageEntry.reason); //Yashaswi 11/5/2018
                            objwastageEntry.SubReasonlist = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId == objwastageEntry.reason).ToList(), "Id", "Reason", objwastageEntry.sub_reason);

                            ModelState.AddModelError("wastage_imgae_path", "Please choose only Image file");
                            Session["Error"] = "Please choose only Image file."; //yashaswi 31/3/2018
                            return View(objwastageEntry);
                        }
                    }

                    using (TransactionScope tscope = new TransactionScope())
                    {
                        try
                        {
                            WarehouseWastageStock objwarehouseWastageStock = new WarehouseWastageStock();
                            objwarehouseWastageStock.LocationId = 0;// objwastageEntry.new_item_location;
                            objwarehouseWastageStock.SubReasonID = objwastageEntry.sub_reason;
                            objwarehouseWastageStock.WarehouseStockID = objwastageEntry.id;
                            objwarehouseWastageStock.WastageQuantity = objwastageEntry.waste_qty;
                            objwarehouseWastageStock.Img_Path = Filename;
                            objwarehouseWastageStock.Remark = objwastageEntry.Remark;
                            objwarehouseWastageStock.CreateDate = DateTime.Now.Date;
                            long PersonalDetailID = 1;
                            if (Session["USER_LOGIN_ID"] != null)
                            {
                                long ID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
                                PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == ID).Select(x => x.ID).First());
                            }

                            objwarehouseWastageStock.CreateBy = PersonalDetailID;
                            objwarehouseWastageStock.ModifyBy = null;
                            objwarehouseWastageStock.ModifyDate = null;
                            objwarehouseWastageStock.NetworkIP = CommonFunctions.GetClientIP();
                            objwarehouseWastageStock.DeviceID = "X";
                            objwarehouseWastageStock.DeviceType = "X";
                            db.WarehouseWastageStock.Add(objwarehouseWastageStock);
                            db.SaveChanges();
                            //// Stock Deduction
                            int wasteQty = objwarehouseWastageStock.WastageQuantity;
                            WarehouseStock obj_WarehouseStocks = new WarehouseStock();
                            obj_WarehouseStocks = db.WarehouseStocks.First(p => p.ID == objwarehouseWastageStock.WarehouseStockID);
                            obj_WarehouseStocks.AvailableQuantity = obj_WarehouseStocks.AvailableQuantity - wasteQty;
                            WarehouseReorderLevel obj_WarehouseReorderLevel = new WarehouseReorderLevel();
                            obj_WarehouseReorderLevel = db.WarehouseReorderLevels.First(p => p.WarehouseID == obj_WarehouseStocks.WarehouseID && p.ProductID == obj_WarehouseStocks.ProductID && p.ProductVarientID == obj_WarehouseStocks.ProductVarientID);
                            obj_WarehouseReorderLevel.AvailableQuantity = obj_WarehouseReorderLevel.AvailableQuantity - wasteQty;
                            db.Entry(obj_WarehouseStocks).State = EntityState.Modified;
                            db.Entry(obj_WarehouseReorderLevel).State = EntityState.Modified;
                            WarehouseStockID = objwarehouseWastageStock.WarehouseStockID;
                            obj_CommonController.ShopStockDeduction(objwarehouseWastageStock.WarehouseStockID, wasteQty, out ShopStock_Qty, out ShopStockID);

                            db.SaveChanges();
                            obj_CommonController.WarehouseStockLog(objwarehouseWastageStock.WarehouseStockID, (int)Inventory.Common.Constants.Warehouse_Stock_Log_Status.WASTAGE, PersonalDetailID, wasteQty); //Yashaswi 2/4/2018

                            tscope.Complete();
                            using (TransactionScope tscope1 = new TransactionScope())
                            {
                                obj_CommonController.AddBatchToShopStock(ShopStock_Qty, WarehouseStockID, ShopStockID);
                                tscope1.Complete();
                            }
                            Session["Success"] = "Item Added in Wastage Successfully."; //yashaswi 31/3/2018
                        }
                        catch (Exception ex)
                        {
                            Transaction.Current.Rollback();
                            tscope.Dispose();
                            throw ex;
                        }

                    }

                    return RedirectToAction("Index", "WarehouseWastageStock");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("waste_qty", ex.Message);
                Session["Error"] = ex.Message; //yashaswi 31/3/2018
                objwastageEntry.MainReasonlist = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId == null && p.IsActive && p.ParentCategoryId == (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.WASTAGE).ToList(), "Id", "Reason", objwastageEntry.reason); //Yashaswi 11/5/2018
                objwastageEntry.SubReasonlist = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId != objwastageEntry.reason).ToList(), "Id", "Reason", objwastageEntry.sub_reason);
                return View(objwastageEntry);
            }
        }

        [HttpPost]
        public ActionResult GetSubReason(string reason)
        {
            return Json(Subreason(reason), JsonRequestBehavior.AllowGet);
        }
        public IEnumerable<SelectListItem> Subreason(string mainReasonId)  // ShipCity is filtered based on the ShipCountry value  
        {
            long? id = Convert.ToInt16(mainReasonId);
            //var subreason = db.WarehouseReasons.Where(p => p.ParentReasonId == id).ToList();

            var subreason = db.WarehouseReasons.Where(p => p.ParentReasonId == id && p.IsActive != false).ToList();//Rumana 15/3/2019
            List<SelectListItem> type = new List<SelectListItem>();
            foreach (var rsn in subreason)
            {
                if (rsn != null)
                {
                    SelectListItem item = new SelectListItem() { Text = rsn.Reason.ToString(), Value = rsn.ID.ToString() };
                    type.Add(item);
                }
            }
            return type;
        }
        public ActionResult Index()
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<WastageEntry> obj_WastageEntry = new List<WastageEntry>();

            //yashaswi 19/3/2018
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }
            obj_WastageEntry = db.Database.SqlQuery<WastageEntry>(@"select Product.Name + ' '+ Size.Name as item_name " +
                             ",Product.HSNCode as HSNCode ,Product.ID as id,WarehouseWastageStock.ID  as waste_id " +
                             ",WarehouseWastageStock.WastageQuantity as waste_qty,WarehouseStock.BatchCode as batch_code " +
                             ",WarehouseWastageStock.CreateDate as createdDate,wr1.Reason+ ', '+ wr.Reason as Remark " +
                             "from WarehouseWastageStock " +
                             "join WarehouseStock     " +
                             "on WarehouseWastageStock.WarehouseStockid=WarehouseStock.id " +
                             "join Product " +
                             "on WarehouseStock.ProductID= Product.id " +
                             "join WarehouseReason as wr " +
                             "on wr.ID=WarehouseWastageStock.SubReasonId " +
                             "join WarehouseReason as wr1 " +
                             "on wr1.ID=wr.ParentReasonId " +
                             "join ProductVarient " +
                             "on ProductVarient.id=WarehouseStock.ProductVarientID " +
                             "join Size " +
                             "on Size.ID=ProductVarient.SizeID " +
                             "WHERE WarehouseStock.WarehouseID=" + WarehouseID + "" +   //yashaswi 19/3/2018
                             " order by waste_id desc").ToList<WastageEntry>();


            foreach (var item in obj_WastageEntry)
            {
                item.item_image_Path = ImageDisplay.SetProductThumbPath(item.id, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
            }
            obj_WastageEntry = obj_WastageEntry.OrderByDescending(p => p.waste_id).ToList();

            return View(obj_WastageEntry);
        }

        //yashaswi 17-3-2018
        public ActionResult Edit(long id)
        {

            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            WastageEntry obj_WastageEntry = db.Database.SqlQuery<WastageEntry>(@"select WarehouseWastageStock.ID as  waste_id, Product.Name + '('+ Size.Name+')' as item_name ,Product.ID as id ,
                    WarehouseWastageStock.WastageQuantity as waste_qty,WarehouseStock.BatchCode as batch_code ,
                    WarehouseWastageStock.CreateDate as createdDate,WarehouseWastageStock.Remark as Remark ,
                    WarehouseReorderLevel.AvailableQuantity as total_item_qty_in_stock, 
                    WarehouseReorderLevel.ReorderLevel as reorder_level,WarehouseStock.BuyRatePerUnit as buy_rate_per_unit,
                    WarehouseStock.InitialQuantity as batch_qty,WarehouseStock.AvailableQuantity as batch_avl_qty,
                    WarehouseStock.ExpiryDate as [expiry_date],--cast( WarehouseStock.BuyRatePerUnit*WarehouseWastageStock.WastageQuantity as decimal(15,2)) as total_amt,
                    '' item_current_location, 0 as new_item_location, WarehouseWastageStock.Img_Path as wastage_imgae_path
                    ,Product.HSNCode as HSNCode 
                    from WarehouseWastageStock 
                    join WarehouseStock     
                    on WarehouseWastageStock.WarehouseStockid=WarehouseStock.id 
                    join WarehouseReorderLevel
                    on WarehouseReorderLevel.ProductID=WarehouseStock.ProductID 
                    and WarehouseReorderLevel.productvarientid = WarehouseStock.productvarientid
                    and WarehouseReorderLevel.WarehouseID = WarehouseStock.WarehouseID
                    join Product 
                    on WarehouseStock.ProductID= Product.id                      
                    join ProductVarient 
                    on ProductVarient.id=WarehouseStock.ProductVarientID 
                    join Size 
                    on Size.ID=ProductVarient.SizeID
                    where WarehouseWastageStock.ID=" + id + "").FirstOrDefault<WastageEntry>();
            if (obj_WastageEntry != null)
            {
                obj_WastageEntry.item_image_Path = ImageDisplay.SetProductThumbPath(obj_WastageEntry.id, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                obj_WastageEntry.total_amt = Convert.ToDecimal(obj_WastageEntry.waste_qty * obj_WastageEntry.buy_rate_per_unit);
                //Yashaswi 30/5/2018
                ViewBag.WastageItemPath = (new CommonController()).GetFileNameWastage(obj_WastageEntry.wastage_imgae_path);
            }

            return View(obj_WastageEntry);


        }
        //Yashaswi 30/5/2018
        public FileStreamResult Download(string fileName)
        {
            string Path = WebConfigurationManager.AppSettings["INVENTORY_ROOT_IMAGE_HTTP"] + WebConfigurationManager.AppSettings["DIRECTORY"] + "/" + WebConfigurationManager.AppSettings["FOLDER_WASTAGE"] + "/";
            string aURL = Path + fileName;
            Stream rtn = null;
            HttpWebRequest aRequest = (HttpWebRequest)WebRequest.Create(aURL);
            HttpWebResponse aResponse = (HttpWebResponse)aRequest.GetResponse();
            rtn = aResponse.GetResponseStream();
            return File(rtn, "image/jpeg", fileName);
        }

        //yashaswi 17-3-2018
        [HttpPost]
        public ActionResult Edit(WastageEntry objwastageEntry)
        {
            long ShopStock_Qty = -1;
            long WarehouseStockID = 0;
            long ShopStockID = 0;
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long PersonalDetailID = 1;
            if (Session["USER_LOGIN_ID"] != null)
            {
                long ID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
                PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == ID).Select(x => x.ID).First());
            }
            WarehouseWastageStock obj_WarehouseWastageStock = db.WarehouseWastageStock.First(p => p.ID == objwastageEntry.waste_id);
            WarehouseStock obj_WarehouseStocks = db.WarehouseStocks.First(p => p.ID == obj_WarehouseWastageStock.WarehouseStockID);
            WarehouseReorderLevel obj_WarehouseReorderLevel = db.WarehouseReorderLevels.First(p => p.WarehouseID == obj_WarehouseStocks.WarehouseID && p.ProductID == obj_WarehouseStocks.ProductID && p.ProductVarientID == obj_WarehouseStocks.ProductVarientID);
            if (objwastageEntry.waste_qty < obj_WarehouseWastageStock.WastageQuantity)
            {
                using (TransactionScope tscope = new TransactionScope())
                {
                    try
                    {
                        /// if new qty is less then add extra qty in available qty of warehouse stock and reorder level 
                        int Rem_qty = obj_WarehouseWastageStock.WastageQuantity - objwastageEntry.waste_qty;
                        obj_WarehouseWastageStock.WastageQuantity = objwastageEntry.waste_qty;
                        obj_WarehouseWastageStock.Remark = objwastageEntry.Remark;
                        obj_WarehouseWastageStock.ModifyDate = DateTime.Now.Date;
                        obj_WarehouseWastageStock.ModifyBy = PersonalDetailID;
                        obj_WarehouseWastageStock.NetworkIP = CommonFunctions.GetClientIP();
                        obj_WarehouseStocks.AvailableQuantity = obj_WarehouseStocks.AvailableQuantity + Rem_qty;
                        obj_WarehouseReorderLevel.AvailableQuantity = obj_WarehouseReorderLevel.AvailableQuantity + Rem_qty;
                        db.Entry(obj_WarehouseWastageStock).State = EntityState.Modified;
                        db.Entry(obj_WarehouseStocks).State = EntityState.Modified;
                        db.Entry(obj_WarehouseReorderLevel).State = EntityState.Modified;
                        CommonController obj_CommonController = new CommonController();
                        WarehouseStockID = obj_WarehouseWastageStock.WarehouseStockID;
                        obj_CommonController.ShopStockDeduction(obj_WarehouseWastageStock.WarehouseStockID, Rem_qty * -1, out ShopStock_Qty, out ShopStockID);
                        obj_CommonController.WarehouseStockLog(obj_WarehouseWastageStock.WarehouseStockID, (int)Inventory.Common.Constants.Warehouse_Stock_Log_Status.WASTAGE_ADD_IN_STOCK, PersonalDetailID, objwastageEntry.waste_qty);//Yashaswi 2/4/2018
                        db.SaveChanges();
                        tscope.Complete();
                        Session["Success"] = "Item Quantity Added in stock successfully.";//yashaswi 31/3/2018
                        using (TransactionScope tscope1 = new TransactionScope())
                        {
                            obj_CommonController.AddBatchToShopStock(ShopStock_Qty, WarehouseStockID, ShopStockID);
                            tscope1.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        Transaction.Current.Rollback();
                        tscope.Dispose();
                        throw ex;
                    }
                }
            }

            return RedirectToAction("Index", "WarehouseWastageStock");

        }

    }
}