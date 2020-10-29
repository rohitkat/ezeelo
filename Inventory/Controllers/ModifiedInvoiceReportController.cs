using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Inventory.Controllers
{
    public class ModifiedInvoiceReportController : Controller
    {
        //yashaswi 14-01-2020 
        //Invoice Report by considering Return
        private EzeeloDBContext db = new EzeeloDBContext();
        public ActionResult Index()
        {
            InvoicesReceivedReportViewModelList obj = new InvoicesReceivedReportViewModelList();
            List<InvoicesReceivedReportViewModel> list = new List<InvoicesReceivedReportViewModel>();
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }
            obj.WarehouseID = WarehouseID;
            obj.Fdate = DateTime.Now.AddMonths(-1);
            obj.TDate = DateTime.Now;
            var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID && x.IsActive == true).Select(x => x.ID).FirstOrDefault();
            obj.SupplierID = SupplierID;
            list = GetData(obj.SupplierID, obj.Fdate, obj.TDate);
            obj.lInvoicesReceivedReportViewModel = list;
            return View(obj);
        }

        [HttpPost]
        public ActionResult Index(InvoicesReceivedReportViewModelList obj)
        {
            List<InvoicesReceivedReportViewModel> list = new List<InvoicesReceivedReportViewModel>();            
            list = GetData(obj.SupplierID, obj.Fdate, obj.TDate);
            obj.lInvoicesReceivedReportViewModel = list;
            return View(obj);
        }

        public List<InvoicesReceivedReportViewModel> GetData(long SupplierId,DateTime FromDate, DateTime ToDate)
        {
            List<InvoicesReceivedReportViewModel> list = new List<InvoicesReceivedReportViewModel>();
            list = db.Database.SqlQuery<InvoicesReceivedReportViewModel>(
                            "exec dbo.[ModifiedInvoiceReport] @SupplierID ,@FromDate,@ToDate",
                             new Object[] {
                                        new SqlParameter("@SupplierID", SupplierId),
                                        new SqlParameter("@FromDate", FromDate),
                                        new SqlParameter("@ToDate", ToDate)
                             }).ToList();
            foreach (InvoicesReceivedReportViewModel item in list)
            {
                decimal? CGSTAmount = item.CGSTAmount * Convert.ToDecimal(item.Quantity);
                decimal? SGSTAmount = item.SGSTAmount * Convert.ToDecimal(item.Quantity);
                decimal? ProductAmount = item.Quantity * item.UnitPrice;
                decimal? TaxableValue = (ProductAmount * 100) / ((item.GSTInPer ?? 0) + 100);
                TaxableValue = Math.Round((decimal)TaxableValue, 2);
                decimal? GSTAmount = ProductAmount - TaxableValue;
                item.SGSTAmount = GSTAmount / 2;
                item.CGSTAmount = GSTAmount / 2;
            }
            return list;
        }

     
    }
}