using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CRM.Models.ViewModel;
using ModelLayer.Models;
using System.Collections.ObjectModel;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using CRM.Models;
using System.Data;

using System.Data.SqlClient;
using CRM.SalesOrder;

namespace CRM.Controllers
{
    public class GoodReceiptReportsController : Controller
    {
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        //CommonController Obj_Common = new CommonController();

        //
        // GET: /GoodReceiptReports/
        [HandleError(View = "Error")]
        public ActionResult GoodReceiptNote(PurchaseOrderViewModelList objPOV) //, int? print
        {

            PurchaseOrderViewModelList objPO = new PurchaseOrderViewModelList();
            try
            {
                //PurchaseOrderViewModelList objPO = new PurchaseOrderViewModelList();
                List<PurchaseOrderViewModel> lPurchaseOrderViewModel = new List<PurchaseOrderViewModel>();

                //if (Session["USER_NAME"] != null)
                //{ }
                //else
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                //long WarehouseID = 0;
                //if (Session["WarehouseID"] != null)
                //{
                //    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                //}

                if (string.IsNullOrEmpty(objPOV.FromDate))
                {
                    objPO.FromDate = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
                }
                else
                {
                    objPO.FromDate = objPOV.FromDate;
                }

                if (string.IsNullOrEmpty(objPOV.ToDate))
                {
                    objPO.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("dd/MM/yyyy");
                }
                else
                {
                    objPO.ToDate = objPOV.ToDate;
                }

                DateTime _FromDate = ConvertDateFromStringToDate(objPO.FromDate);
                DateTime _ToDate = ConvertDateFromStringToDate(objPO.ToDate);
                DateTime fDate1 = Convert.ToDateTime(_FromDate);
                DateTime tDate1 = Convert.ToDateTime(_ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                if (objPO.WarehouseID == 0){


                    lPurchaseOrderViewModel = GetRecord(fDate1, tDate1, objPOV.SupplierID);
                }
                
                 
                


                //ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);

                ///added by priti
                //List<Supplier> ONJ = Obj_Common.GetSupplierLIst(WarehouseID);
                //ONJ.Add(new Supplier
                //{
                //    Name = "All Supplier",
                //    ID = 0

                //});

                //ViewBag.PossibleSuppliers = ONJ.ToList();
             
                
                ///end by Priti
                objPO.lPurchaseOrderViewModel = lPurchaseOrderViewModel.ToList();

                if (objPO.lPurchaseOrderViewModel.Count > 0)
                {
                    return View("GoodReceiptNote", objPO);
                }
            }
            catch (Exception ex)
            {
                new Exception("Some unknown error encountered!");
            }
            return View(objPO);
        }

        [NonAction]
        public DateTime ConvertDateFromStringToDate(string _date)
        {

            DateTime datetime = DateTime.Now;
            String[] SplitStringDate = _date.Split('/');
            datetime = new DateTime(Convert.ToInt32(SplitStringDate[2]), Convert.ToInt32(SplitStringDate[1]), Convert.ToInt32(SplitStringDate[0]));
            return datetime;
        }

        public ActionResult Export(string FromDate, string ToDate, int option, int SupplierID) //, int? print
        {
            try
            {
                PurchaseOrderViewModelList objPO = new PurchaseOrderViewModelList();
                List<PurchaseOrderViewModel> lPurchaseOrderViewModel = new List<PurchaseOrderViewModel>();

                //if (Session["USER_NAME"] != null)
                //{ }
                //else
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                //long WarehouseID = 0;
                //if (Session["WarehouseID"] != null)
                //{
                //    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                //}

                if (string.IsNullOrEmpty(FromDate))
                {
                    objPO.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
                }
                else
                {
                    objPO.FromDate = FromDate;
                }

                if (string.IsNullOrEmpty(ToDate))
                {
                    objPO.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
                }
                else
                {
                    objPO.ToDate = ToDate;
                }

                DateTime fDate = Convert.ToDateTime(objPO.FromDate);
                DateTime tDate = Convert.ToDateTime(objPO.ToDate);  
                if (objPO.WarehouseID == 0 )
                {
                    lPurchaseOrderViewModel = GetRecord(fDate, tDate, objPO.SupplierID);
                }

                //ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
                objPO.lPurchaseOrderViewModel = lPurchaseOrderViewModel.ToList();

                if (option != null && option > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Sr.No.", typeof(long));
                    dt.Columns.Add("PO Code ", typeof(string));
                    dt.Columns.Add("Create Date", typeof(string));
                    dt.Columns.Add("Supplier ", typeof(string));
                    dt.Columns.Add("SKUID", typeof(long));      /// Added by Priti
                    dt.Columns.Add("Item", typeof(string));
                    dt.Columns.Add("Qty Ordered", typeof(int));
                    dt.Columns.Add("Received Date", typeof(string));
                    dt.Columns.Add("Invoice Code", typeof(string));
                    dt.Columns.Add("MRP", typeof(decimal));
                    dt.Columns.Add("Unit Price", typeof(decimal));
                    dt.Columns.Add("Unit Price", typeof(decimal));
                    dt.Columns.Add("GSTInPer", typeof(int));
                    dt.Columns.Add("GSTAmount", typeof(decimal));
                    dt.Columns.Add("Entity", typeof(string));
                    dt.Columns.Add("City", typeof(string));
                    dt.Columns.Add("Franchise", typeof(string));


                    int i = 0;
                    foreach (var row in lPurchaseOrderViewModel)
                    {
                        i = i + 1;
                        dt.LoadDataRow(new object[] { i, row.PurchaseOrderCode, row.OrderDate.ToString("dd/MM/yyyy"),row.SupplierName,
                   row.SKUID, row.ItemName,row.OrderedQuantity, row.ReceivedDate.ToString("dd/MM/yyyy"),row.InvoiceCode,row.MRP,row.UnitPrice,row.GSTInPer,row.GSTAmount, row.ReceivedQuantity,row.Amount,row.Entity,row.City,row.Franchises }, false);
                    }
                  
                    ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (option == 1)
                    {
                        ExportExcelCsv.ExportToExcel(dt, "GoodReceiptNoteReport");
                    }
                    else if (option == 2)
                    {
                        ExportExcelCsv.ExportToCSV(dt, "GoodReceiptNoteReport");
                    }
                    else if (option == 3)
                    {
                        ExportExcelCsv.ExportToPDF(dt, "GoodReceiptNoteReport");
                    }
                }
                else
                {
                    return View("GoodReceiptNote", objPO);
                }

            }
            catch (Exception ex)
            {
                new Exception("Some unknown error encountered!");
            }
            return View();
        }


        private List<PurchaseOrderViewModel> GetRecord(DateTime fDate, DateTime tDate, long SupplierID)
        {
            PurchaseOrderViewModelList objPO = new PurchaseOrderViewModelList();
            List<PurchaseOrderViewModel> lPurchaseOrderViewModel = new List<PurchaseOrderViewModel>();
                               
            try
            {              
                //long WarehouseID = 0;
                //if (Session["WarehouseID"] != null)
                //{
                //    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                //}                             
                               
                    lPurchaseOrderViewModel = (from o in db.PurchaseOrders
                                               join pod in db.PurchaseOrderDetails on o.ID equals pod.PurchaseOrderID
                                               //join rt in db.RateCalculations on pod.RateCalculationId equals rt.ID    //join added by Priti on 14/9/2018
                                               join inv in db.Invoices on o.ID equals inv.PurchaseOrderID
                                               join ind in db.InvoiceDetails on inv.ID equals ind.InvoiceID
                                               join w in db.Warehouses on o.WarehouseID equals w.ID
                                               join s in db.Suppliers on o.SupplierID equals s.ID
                                               join p in db.Products on ind.ProductID equals p.ID
                                               join v in db.ProductVarients on p.ID equals v.ProductID
                                               join z in db.Sizes on v.SizeID equals z.ID
                                               where 
                                       pod.ProductVarientID == v.ID && pod.ProductID == ind.ProductID && pod.ProductVarientID == ind.ProductVarientID
                                               && System.Data.Entity.DbFunctions.TruncateTime(inv.InvoiceDate) >= fDate.Date && System.Data.Entity.DbFunctions.TruncateTime(inv.InvoiceDate) <= tDate.Date
                                               select new PurchaseOrderViewModel
                                               {
                                                   PurchaseOrderID = o.ID,
                                                   SKUID = v.ID,///Added by Priti
                                                   CGSTAmount = ind.CGSTAmount,  ///Added by Priti
                                                   SGSTAmount = ind.CGSTAmount,  ///Added by Priti
                                                   GSTInPer = ind.GSTInPer,///Added by Priti
                                                                           ///
                                                      BuyRatePerUnit=ind.BuyRatePerUnit,////ADDED BY pRITI                     ///
                                                   InvoiceID = inv.ID,
                                                   WarehouseName = w.Name,
                                                   SupplierName = s.Name,
                                                   PurchaseOrderCode = o.PurchaseOrderCode,
                                                   InvoiceCode = inv.InvoiceCode,
                                                   OrderDate = o.OrderDate,
                                                   ReceivedDate = inv.InvoiceDate,
                                                   ExpetedDeliveryDate = o.ExpetedDeliveryDate,
                                                   ItemName = p.Name + " (" + z.Name + ")",
                                                   HSNCode = p.HSNCode,
                                                   OrderedQuantity = pod.Quantity,
                                                   ReceivedQuantity = ind.ReceivedQuantity,
                                                   UnitPrice = ind.BuyRatePerUnit,
                                                   Amount = ind.Amount,
                                                   MRP = ind.MRP,
                                                   Entity= w.Entity,
                                                   IsFulfillmentCenter=w.IsFulfillmentCenter,
                                                   WarehouseID= w.ID
                                                  
                                               }).OrderByDescending(x => x.InvoiceID).ToList(); //.OrderByDescending(o => o.OrderDate)
                    foreach (PurchaseOrderViewModel CR in lPurchaseOrderViewModel)
                    {

                        decimal amtForGST = Math.Round(CR.BuyRatePerUnit / (decimal)(1 + ((decimal)CR.GSTInPer / 100)), 2);
                    //    CR.GSTAmount = Math.Round(CR.BuyRatePerUnit - amtForGST, 2) * CR.OrderedQuantity;
             
                        //CR.GSTAmount =(CR.CGSTAmount +CR.SGSTAmount)*CR.OrderedQuantity;
                        CR.GSTAmount = (CR.CGSTAmount + CR.SGSTAmount) * CR.OrderedQuantity;

                        
                    }





                //Add by Priti for fv and dv Franchise CITY
                List<PurchaseOrderViewModel> lPurchaseOrderViewModel1 = new List<PurchaseOrderViewModel>();
                var EntityFV = lPurchaseOrderViewModel.Select(x => x.IsFulfillmentCenter).ToList();
                if (lPurchaseOrderViewModel.Where(x => x.IsFulfillmentCenter).Select(x => x.IsFulfillmentCenter).ToList().Count > 0)
                {



                    lPurchaseOrderViewModel1 = (from z in db.WarehouseFranchises
                                                  join wf in db.Franchises on z.FranchiseID equals wf.ID
                                                  join W in db.Warehouses on z.WarehouseID equals W.ID
                                                  join sh in db.Pincodes on W.PincodeID equals sh.ID
                                                  join c in db.Cities on sh.CityID equals c.ID

                                                  select new PurchaseOrderViewModel
                                                  {
                                                      Franchises = wf.ContactPerson,
                                                      City = c.Name,
                                                      WarehouseID = W.ID
                                                  }).ToList();
                    if (lPurchaseOrderViewModel1 != null && lPurchaseOrderViewModel1.Count > 0)
                    {
                        foreach (var item in lPurchaseOrderViewModel)
                        {
                            item.City = lPurchaseOrderViewModel1.FirstOrDefault(x => x.WarehouseID == item.WarehouseID)?.City;
                            item.Franchises = lPurchaseOrderViewModel1.FirstOrDefault(x => x.WarehouseID == item.WarehouseID)?.Franchises;
                        }
                    }


                    lPurchaseOrderViewModel = lPurchaseOrderViewModel;
                }

                return lPurchaseOrderViewModel;
            }
             
            catch (Exception ex)
            {
                new Exception("Some unknown error encountered!");
            }
            return lPurchaseOrderViewModel;
        }

                 

        [HandleError(View = "Error")]
        public ActionResult GoodReceiptNotePrint(PurchaseOrderViewModelList objPOV) //, int? print
        {
            try
            {
                PurchaseOrderViewModelList objPO = new PurchaseOrderViewModelList();
                List<PurchaseOrderViewModel> lPurchaseOrderViewModel = new List<PurchaseOrderViewModel>();

                //if (Session["USER_NAME"] != null)
                //{ }
                //else
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                //long WarehouseID = 0;
                //if (Session["WarehouseID"] != null)
                //{
                //    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                //}

                if (string.IsNullOrEmpty(objPOV.FromDate))
                {
                    objPO.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
                }
                else
                {
                    objPO.FromDate = objPOV.FromDate;
                }

                if (string.IsNullOrEmpty(objPOV.ToDate))
                {
                    objPO.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
                }
                else
                {
                    objPO.ToDate = objPOV.ToDate;
                }

                DateTime fDate = Convert.ToDateTime(objPO.FromDate);
                DateTime tDate = Convert.ToDateTime(objPO.ToDate);

                if (objPO.WarehouseID == 0)
                {
                    lPurchaseOrderViewModel = GetRecord(fDate, tDate, objPOV.SupplierID);
                }
                               
                //ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
                objPO.lPurchaseOrderViewModel = lPurchaseOrderViewModel.ToList();

                return View("GoodReceiptNotePrint", objPO);
            }
            catch (Exception ex)
            {
                new Exception("Some unknown error encountered!");
            }
            return View();
        }

	}
}