using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Inventory.Controllers
{
    public class ProductDetailsSPController : Controller
    {

        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController Obj_Common = new CommonController();

        //
        // GET: /InventoryReports/
        public ActionResult GetProductDetailsSPReport(ProductDetailsSPViewModelList obj1, string ExportOption)
        {
            ProductDetailsSPViewModelList obj = new ProductDetailsSPViewModelList();
            List<ProductDetailsSPViewModel> list = new List<ProductDetailsSPViewModel>();

            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            if (string.IsNullOrEmpty(obj1.FromDate))
            {
                obj.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            }
            else
            {
                obj.FromDate = obj1.FromDate;
            }

            if (string.IsNullOrEmpty(obj1.ToDate))
            {
                obj.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
            }
            else
            {
                obj.ToDate = obj1.ToDate;
            }

            DateTime fDate = Convert.ToDateTime(obj.FromDate);
            DateTime tDate = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);
            obj.WarehouseID = obj1.WarehouseID;


            //ViewBag.SelectedWarehouse = 0;
            //if (Session["IsEzeeloLogin"].ToString() == "1" || Session["BusinessTypeID"].ToString() == "5")
            //{
            //    //display all warehouse Or Selected
            //    if (WarehouseID != null && WarehouseID != 0)
            //    {


            //        obj.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");
            //        list = list.Where(p => p.WarehouseID == WarehouseID).ToList();
            //        ViewBag.SelectedWarehouse = WarehouseID;
            //    }
            //    ViewBag.DV = "0";
            //}
            //else
            //{
            //    ViewBag.DV = "1";
            //    list = list.Where(p => p.WarehouseID == WarehouseID).ToList();
            //}

            ///  obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");
            ///  
            var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID && x.IsActive == true).Select(x => x.ID).FirstOrDefault();

            obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");


            // var reportList = db.Database.SqlQuery<InvoicesReceivedReportViewModel>(
            // "exec dbo.[InvoicesReceived] @WarehouseID,,@FromDate,@ToDate",
            //  new Object[] { new SqlParameter("@WarehouseID",WarehouseID),
            //        new SqlParameter("@FromDate", fDate),
            //new SqlParameter("@ToDate", tDate)}
            //).ToList();



            var reportList = db.Database.SqlQuery<ProductDetailsSPViewModel>(
 "exec dbo.[ProductDetailsSP] @WarehouseID ,@FromDate,@ToDate",
 new Object[] {
                  new SqlParameter("@WarehouseID", WarehouseID), 
           new SqlParameter("@FromDate", fDate),
           new SqlParameter("@ToDate", tDate)}
    ).ToList();

            foreach (var item in reportList)
            {
                item.item_image_Path = ImageDisplay.SetProductThumbPath(item.ProductId, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
            }
            obj.lProductDetailsSPViewModel = obj.lProductDetailsSPViewModel.OrderByDescending(x => x.WarehouseID).ToList();

            obj.lProductDetailsSPViewModel = reportList.ToList();

            //if (obj1.WarehouseID != null)
            //{
            //    obj.lInvoicesReceivedReportViewModel = obj.lInvoicesReceivedReportViewModel.Where(p => p.WarehouseID == WarehouseID).ToList();
            //}

            DateTime fDate1 = Convert.ToDateTime(obj.FromDate);
            DateTime tDate1 = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59);


            var result = obj.lProductDetailsSPViewModel.Where(x => x.CreateDate >= fDate1 &&
                                  x.CreateDate <= tDate1).ToList();
            obj.lProductDetailsSPViewModel = result.OrderByDescending(p => p.RetailPoint).ToList();
            Session["lProductDetailsSPViewModel"] = result;
            return View(obj);

        }



        //public ActionResult Export() //, int? print
        //{
        //    try
        //    {
        //        ProductDetailsSPViewModelList obj = new ProductDetailsSPViewModelList();
        //        if (Session["lProductDetailsSPViewModel"] != null)
        //        {
        //            obj.lProductDetailsSPViewModel = (List<ProductDetailsSPViewModel>)Session["lProductDetailsSPViewModel"];

        //            DataTable dt = new DataTable();
        //            dt.Columns.Add("Sr.No.", typeof(long));
        //            dt.Columns.Add("Img", typeof(string));
        //            dt.Columns.Add("SKUID", typeof(long));
        //            dt.Columns.Add("SKUName", typeof(string));
        //            dt.Columns.Add("SKUUnit", typeof(string));
        //            dt.Columns.Add("SaleRate", typeof(decimal));
        //            dt.Columns.Add("MRP", typeof(decimal));
        //            dt.Columns.Add("RetailPoint", typeof(decimal));
        //            //dt.Columns.Add("SaleQty", typeof(int));
        //            dt.Columns.Add("DiscountPer", typeof(decimal));
        //            int i = 0;
        //            foreach (var row in obj.lProductDetailsSPViewModel)
        //            {
        //                i = i + 1;
        //                dt.LoadDataRow(new object[] { i, row.item_image_Path,row.SKUID, row.SKUName, row.SKUUnit,
        //          //          row.Manifecturer
        //          //          ,row.BatchCode ,
        //          //row.InvoiceCode, 
        //          //row.InitialQuantity,
        //          //row.AvailableInStock,
        //          //row.SupplierName,
        //          //row.Amount,
        //          //row.CreateDate,
        //          //      row.BuyRatePerUnit,
        //                row.SaleRate,
        //                row.MRP,
        //                row.RetailPoint,
        //                //row.ExpiryDate,
        //                //row.SaleQty,
        //                //row.SaleValue,
        //                //row.InventoryValue,
        //                row.DiscountPer}, false);
        //            }


        //            Response.AddHeader("content-disposition", "attachment; filename=InventoryReports.xls");

        //            ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
        //            if (option == 1)
        //            {
        //                ExportExcelCsv.ExportToExcel(dt, "GetProductDetailsSPReport");
        //            }
        //            else if (option == 2)
        //            {
        //                ExportExcelCsv.ExportToCSV(dt, "GetProductDetailsSPReport");
        //            }
        //            else if (option == 3)
        //            {
        //                ExportExcelCsv.ExportToPDF(dt, "GetProductDetailsSPReport");
        //            }
        //        }
        //        else
        //        {
        //            return RedirectToAction("GetProductDetailsSPReport", obj);
        //        }

        //    }
        //    catch (BusinessLogicLayer.MyException myEx)
        //    {
        //        ModelState.AddModelError("Error", "There's Something wrong in Exporting Order Transaction Report!!");

        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
        //            + "[InventoryReportsController][POST:GetInventoryItemReport]" + myEx.EXCEPTION_PATH,
        //            BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("Error", "There's Something wrong in Exporting Order Transaction Report!!");

        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[InventoryReportsController][POST:GetInventoryItemReport]",
        //            BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
        //    }
        //    //catch (Exception ex)
        //    //{
        //    //    new Exception("Some unknown error encountered!");
        //    //}

        //    return View();
        //}

        public ActionResult Export()
        {
            var gv = new GridView();
            string FileName = "";
            List<PromotionalERPPayoutUserList> listPromotionalERPPayoutUserList = new List<ModelLayer.Models.ViewModel.PromotionalERPPayoutUserList>();
            if (Session["PromotionalERPPayoutUserList"] != null)
            {
                listPromotionalERPPayoutUserList = (List<PromotionalERPPayoutUserList>)Session["PromotionalERPPayoutUserList"];
            }
            ProductDetailsSPViewModelList obj = new ProductDetailsSPViewModelList();
            if (Session["lProductDetailsSPViewModel"] != null)
            {
                obj.lProductDetailsSPViewModel = (List<ProductDetailsSPViewModel>)Session["lProductDetailsSPViewModel"];
            }

            FileName = "Product List Report";
            gv.DataSource = obj.lProductDetailsSPViewModel.Select(p => new { p.item_image_Path, p.SKUID, p.SKUName, p.SKUUnit, p.SaleRate, p.MRP, p.RetailPoint, p.DiscountPer, p.BusinessPoints });
            gv.DataBind();


            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + FileName + ".xls");
            Response.ContentType = "application/ms-excel";

            ////aMT
            ////String imagepath = "<img src='http://localhost:56705/WebSite12/images/logo.jpg' width='70' height='80'/>";

            //String imagepath =Url.Content("E:\\Working Code of 23-11-2018\\Identical Code(Live)\\HH\\Inventory\\Image\\gb_25886_thumb.jpg");
            //Response.Output.Write("\n<body>\n<html>");

            //Response.Output.Write("<table width='800' align='center' style='text-align:center'");
            //Response.Output.Write("<tr><td colspan='10' align='center'><div align='center' style='text-align:center'>" + imagepath + "</div></td></tr>");

            //Response.Output.Write("</table>");
            //Response.Output.Write("\n</body>\n</html>");

            //Response.Flush();
            //Response.End();
            ////END AMT
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);

            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("GetProductDetailsSPReport");
        }


        //   protected FileContentResult ViewPdf(string pageTitle, string viewName, object model)
        //{
        //    // Render the view html to a string.
        //    string htmlText = this.htmlViewRenderer.RenderViewToString(this, viewName, model);

        //    // Let the html be rendered into a PDF document through iTextSharp.
        //    byte[] buffer = standardPdfRenderer.Render(htmlText, pageTitle);

        //    // Return the PDF as a binary stream to the client.
        //    return File(buffer, "application/pdf","file.pdf");
        //}
        //        protected void ExportToPDF(object sender, EventArgs e)
        //        {
        //            //Get the data from database into datatable
        //            string strQuery = "select CustomerID, ContactName, City, PostalCode" +
        //              " from customers";
        //            SqlCommand cmd = new SqlCommand(strQuery);
        //            DataTable dt = GetData(cmd);

        //            //Create a dummy GridView
        //            GridView GridView1 = new GridView();
        //            GridView1.AllowPaging = false;
        //            GridView1.DataSource = dt;
        //            GridView1.DataBind();

        //            Response.ContentType = "application/pdf";
        //            Response.AddHeader("content-disposition",
        //              "attachment;filename=DataTable.pdf");
        //            Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //            StringWriter sw = new StringWriter();
        //            HtmlTextWriter hw = new HtmlTextWriter(sw);
        //            GridView1.RenderControl(hw);
        //            StringReader sr = new StringReader(sw.ToString());
        //            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
        //            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
        //            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        //            pdfDoc.Open();
        //            htmlparser.Parse(sr);
        //            pdfDoc.Close();
        //            Response.Write(pdfDoc);
        //            Response.End();
        //        }





    }
}