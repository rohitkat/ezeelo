using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Inventory.Controllers
{
    public class StockMisMatchReportController : Controller
    {
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController Obj_Common = new CommonController();
        //
        // GET: /StockMisMatchReportController @*created by Rumana on 01-06-2019*@/
        public ActionResult GetStockMisMatchReport(StockMisMatchReportViewModel obj1, int? Status)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            StockMisMatchReportViewModel obj = new StockMisMatchReportViewModel();
            List<StockMisMatchReportViewModelList> list = new List<StockMisMatchReportViewModelList>();

            if (Status == null)
            {
                Status = 0;
            }
            ViewBag.DropdownSelected = Status;
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }
            Warehouse objWarehouse = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseID);
            if (objWarehouse.Entity.Trim() == "EVW")
            {
                List<Warehouse> DVLISt = db.Warehouses.Where(w => w.IsActive == true && w.IsFulfillmentCenter == false && (db.EVWsDVs.Where(e => e.WarehouseId_EVW == WarehouseID && e.IsActive == true).Select(e => e.WarehouseId).Contains(w.ID))).ToList();
                List<Warehouse> FVLIst = new List<Warehouse>();
                foreach (var item in DVLISt)
                {
                    List<Warehouse> FVLIst_ = new List<Warehouse>();
                    FVLIst_ = db.Warehouses.Where(w => w.DistributorId == item.ID && w.IsFulfillmentCenter == true).ToList();
                    FVLIst.AddRange(FVLIst_);
                }
                obj.WarehouseList = new SelectList(FVLIst, "ID", "Name", "--Select--");
            }
            else
            {
                obj1.WarehouseID = WarehouseID;
                obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name", WarehouseID);
            }

            if (string.IsNullOrEmpty(obj1.FromDate))
            {
                //obj.FromDate = "07/01/2018";
                obj.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            }
            else
            {
                obj.FromDate = obj1.FromDate;
            }

            if (string.IsNullOrEmpty(obj1.ToDate))
            {
                obj.ToDate = DateTime.Now.AddHours(5.5).ToString("MM/dd/yyyy");
            }
            else
            {
                obj.ToDate = obj1.ToDate;
            }
            if (obj1.IsChecked == null)
            {
                obj1.IsChecked = true;
                obj.IsChecked = true;
            }
            else
            {
                obj.IsChecked = obj1.IsChecked;
            }

            DateTime fDate = Convert.ToDateTime(obj.FromDate).AddHours(23).AddMinutes(59).AddSeconds(59);
            DateTime tDate = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);
            obj.WarehouseID = obj1.WarehouseID;
            int Ischecked = 0;
            Ischecked = Convert.ToInt32(obj1.IsChecked);
            var reportList = 0;
            if (Status == 0)
            {
                obj.lStockMisMatchReportViewModelList = db.Database.SqlQuery<StockMisMatchReportViewModelList>(
               "exec dbo.[WrongQtyInShop] @WarehouseID,@FromDate,@ToDate,@Ischecked",
                  new Object[] { new SqlParameter("@WarehouseID", obj.WarehouseID) ,
            new SqlParameter("@FromDate", fDate),
           new SqlParameter("@ToDate", tDate),new SqlParameter("@Ischecked", Ischecked)}
       ).ToList();
                //obj.lStockMisMatchReportViewModelList = list0.ToList();
            }
            else
            {
                obj.lStockMisMatchReportViewModelList = db.Database.SqlQuery<StockMisMatchReportViewModelList>(
               "exec dbo.[WrongQtyInWarehouse] @WarehouseID,@FromDate,@ToDate,@Ischecked",
                  new Object[] { new SqlParameter("@WarehouseID", obj.WarehouseID) ,
            new SqlParameter("@FromDate", fDate),
           new SqlParameter("@ToDate", tDate),new SqlParameter("@Ischecked", Ischecked)}).ToList();
            }
           // obj.lStockMisMatchReportViewModelList = obj.lStockMisMatchReportViewModelList.Where(x => x.WarehouseID == WarehouseID).ToList();

            Session["StockMisMatchReportViewModelList"] = obj.lStockMisMatchReportViewModelList;
            return View(obj);

        }

        public ActionResult ExportToExcel()
        {
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }
            string FileName = "";
            Warehouse obj = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseID);
            if (obj != null)
            {
                FileName = obj.Name;
            }
            FileName = FileName + " Stock Mismatch List";
            List<ExporttoExcelStockMisMatchReportViewModelList> list = new List<ExporttoExcelStockMisMatchReportViewModelList>();
            if (Session["StockMisMatchReportViewModelList"] != null)
            {
                List<StockMisMatchReportViewModelList> objWRLVM = (List<StockMisMatchReportViewModelList>)Session["StockMisMatchReportViewModelList"];

                if (objWRLVM.Count != 0)
                {
                    var i = 0;
                    foreach (var item in objWRLVM)
                    {
                        ExporttoExcelStockMisMatchReportViewModelList o = new ExporttoExcelStockMisMatchReportViewModelList();
                        i = i + 1;
                        o.SrNo = i;
                        o.SKUID = item.SKUID;
                        o.SKUName = item.SKUName;
                        o.SKUUnit = item.SKUUnit;
                        o.BrandName = item.BrandName;
                        o.BatchCode = item.BatchCode;
                        o.InvoiceCode = item.InvoiceCode;
                        o.MRP = item.MRP;
                        o.InvoiceConfirmationDate = item.InvoiceConfirmationDate;
                        if (Session["Entity"].ToString() == "EVW")
                        {
                            o.EzeeloPurchasePrice = item.EzeeloPurchasePrice;
                        }
                        o.PurchaseRate = item.PurchaseRate;
                        o.SaleRate = item.SaleRate;
                        o.RetailPoints = item.RetailPoints;
                        o.ActiveBatch = item.ActiveBatch;
                        o.InitialQty = item.InitialQty;
                        o.WarehouseQty = item.WarehouseQty;
                        o.ShopQty = item.ShopQty;
                        o.ReservedQty = item.ReservedQty;
                        o.DeliveredQty = item.DeliveredQty;
                        o.ReturnToDvQty = item.ReturnToDvQty;
                        o.ReturnFromCustomerQty = item.ReturnFromCustomerQty;
                        o.CanceledQty = item.CanceledQty;
                        o.AbandonedQty = item.AbandonedQty;
                        o.PortalMRP = item.PortalMRP;
                        o.PortalSaleRate = item.PortalSaleRate;
                        o.PortalRetailPoint = item.PortalRetailPoint;
                        o.PendingQty = item.PendingQty;
                        o.PlacedQty = item.PlacedQty;
                        o.ConfirmQty = item.ConfirmQty;
                        o.PackedQty = item.PackedQty;
                        o.Dispatched_from_shopQty = item.Dispatched_from_shopQty;
                        o.In_godownQty = item.In_godownQty;
                        o.Dispatched_from_godownQty = item.Dispatched_from_godownQty;
                        o.ShopStockId = item.ShopStockId;
                        o.WarehouseStockId = item.WarehouseStockId;
                        
                        list.Add(o);
                    }
                }
            }


            var gv = new GridView();
            gv.DataSource = list;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + FileName + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("GetStockMisMatchReport");
        }
    }
}