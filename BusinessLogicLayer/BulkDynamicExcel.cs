using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System.Reflection;
using System.Data.Entity.Validation;
using ClosedXML;
using ClosedXML.Excel;
using System.IO;


namespace BusinessLogicLayer
{
    public class BulkDynamicExcel
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        public class lCCategory
        {
            public int CategoryID { get; set; }
            public string Category { get; set; }
        }
        public class lCBrand
        {
            public string BrandName { get; set; }
        }
        public class lCColor
        {
            public string ColorName { get; set; }
        }
        public class lCSize
        {
            public string SizeName { get; set; }
        }
        public class lCDimension
        {
            public string DimensionName { get; set; }
        }
        public class lCMaterial
        {
            public string MaterialName { get; set; }
        }
        public class lCUnit
        {
            public string UnitName { get; set; }
        }
        public void GenerateExcel(int ShopID, string ExcelName)
        {

            try
            {
                DataTable dtVarient = new DataTable();
                dtVarient.Columns.Add("BrandName", typeof(string));
                dtVarient.Columns.Add("Color", typeof(string));
                dtVarient.Columns.Add("Size", typeof(string));
                dtVarient.Columns.Add("Dimension", typeof(string));
                dtVarient.Columns.Add("Material", typeof(string));
                //dtVarient.Columns.Add("Unit", typeof(string));
                dtVarient.Columns.Add("Category", typeof(string));

                int lColor = 0;
                int lSize = 0;
                int lDimension = 0;
                int lMaterial = 0;
                int lUnit = 0;
                int lCat = 0;
                //get all active brands from database
                var brand = (from b in db.Brands
                             where b.IsActive == true
                             select new lCBrand
                             {

                                 BrandName = b.Name
                             }).OrderBy(x => x.BrandName).ToList();
                foreach (var row in brand)
                    dtVarient.LoadDataRow(new object[] { row.BrandName }, false);

                //get all active colors from database
                var color = (from c in db.Colors
                             where c.IsActive == true
                             select new lCColor
                             {
                                 ColorName = c.Name

                             }).OrderBy(x => x.ColorName).ToList();
                foreach (var row in color)
                {
                    dtVarient.Rows[lColor]["Color"] = row.ColorName;
                    lColor = lColor + 1;
                }

                //get all active size from database
                var size = (from s in db.Sizes
                            where s.IsActive == true
                            select new lCSize
                            {
                                SizeName = s.Name

                            }).OrderBy(x => x.SizeName).ToList();
                foreach (var row in size)
                {
                    dtVarient.Rows[lSize]["Size"] = row.SizeName;
                    lSize = lSize + 1;
                }
                //get all active dimension from database
                var dimension = (from d in db.Dimensions
                                 where d.IsActive == true
                                 select new lCDimension
                                 {
                                     DimensionName = d.Name

                                 }).OrderBy(x => x.DimensionName).ToList();
                foreach (var row in dimension)
                {
                    dtVarient.Rows[lDimension]["Dimension"] = row.DimensionName;
                    lDimension = lDimension + 1;
                }
                //get all active material from database
                var material = (from m in db.Materials
                                where m.IsActive == true
                                select new lCMaterial
                                {
                                    MaterialName = m.Name

                                }).OrderBy(x => x.MaterialName).ToList();
                foreach (var row in material)
                {
                    dtVarient.Rows[lMaterial]["Material"] = row.MaterialName;
                    lMaterial = lMaterial + 1;
                }
                //get all active Units from database
                //var unit = (from u in db.Units
                //            where u.IsActive == true
                //            select new lCUnit
                //            {
                //                UnitName = u.Name

                //            }).OrderBy(x => x.UnitName).ToList();
                //foreach (var row in unit)
                //{
                //    dtVarient.Rows[lUnit]["Unit"] = row.UnitName;
                //    lUnit = lUnit + 1;
                //}


                //get all active Categories from database

                var category = (from op in db.OwnerPlans
                                 join p in db.Plans on op.PlanID equals p.ID
                                 join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                 join C3 in db.Categories on pcc.CategoryID equals C3.ID
                                 join C2 in db.Categories on C3.ParentCategoryID equals C2.ID
                                 where pcc.IsActive == true && op.IsActive == true && op.OwnerID == ShopID && C3.IsActive == true && C2.IsActive==true
                                && op.Plan.PlanCode.StartsWith("GBMR")
                                 select new lCCategory
                                 {
                                     CategoryID = pcc.ID,
                                     Category = C2.Name + " - " + C3.Name
                                 }).OrderBy(x => x.Category).Distinct().ToList();

                //var category = (from op in db.OwnerPlans
                //                join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                //                join C3 in db.Categories on pcc.CategoryID equals C3.ID
                //                join C2 in db.Categories on C3.ParentCategoryID equals C2.ID
                //                where op.OwnerID == ShopID && C3.IsActive == true && C2.IsActive == true &&
                //                op.Plan.PlanCode.StartsWith("GBMR")
                //                select new lCCategory
                //                {
                //                    CategoryID = pcc.ID,
                //                    Category = C2.Name + " - " + C3.Name
                //                }).OrderBy(x => x.CategoryID).Distinct().ToList();

                foreach (var row in category)
                {

                    dtVarient.Rows[lCat]["Category"] = row.Category;
                    lCat = lCat + 1;
                }
                DataTable dtProduct = GetProductTable();
                DataTable dtProductStock = GetProductStockTable();
                ExportData(dtVarient, dtProduct, dtProductStock, category, brand, color, size, dimension, material, ExcelName);
            }
            catch (Exception ex)
            {

                if (ex.InnerException != null)
                {

                    BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Generating Excel :" + ex.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

                }
                else if (ex.Message != null)
                {

                    BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Generating Excel :" + ex.Message, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

                }


            }
        }

        private void ExportData(DataTable dtVarient, DataTable dtProduct, DataTable dtProductStock, List<lCCategory> category, List<lCBrand> brand, List<lCColor> color, List<lCSize> size, List<lCDimension> dimension, List<lCMaterial> material, string ExcelName)
        {
            try
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    //wb.NamedRange("Product");
                    wb.Worksheets.Add(dtProduct, "Product");
                    wb.Worksheets.Add(dtProductStock, "Product_Stock");
                    wb.Worksheets.Add(dtVarient, "Varients");
                    int countCat = category.Count() + 1;
                    int countBrand = brand.Count() + 1;
                    int countColor = color.Count() + 1;
                    int countSize = size.Count() + 1;
                    int countDimension = dimension.Count() + 1;
                    int countMaterial = material.Count() + 1;

                    wb.Worksheet("Varients").Style.Protection.SetLocked(true);
                    wb.Worksheet("Varients").Style.Protection.Locked = true;

                    string rangecat = "'Varients'!F2:F" + countCat + "";
                    wb.Worksheet("Product").Column(3).SetDataValidation().IgnoreBlanks = true;
                    wb.Worksheet("Product").Column(3).SetDataValidation().List(rangecat);

                    string cellcat = wb.Worksheet("Varients").Column(6).Cell(2).Value.ToString();
                    wb.Worksheet("Product").Column(3).Cell(2).Value = cellcat;
                    wb.Worksheet("Product").Column(3).Cell(3).Value = cellcat;

                    string rangebrand = "'Varients'!A2:A" + countBrand + "";
                    wb.Worksheet("Product").Column(9).SetDataValidation().IgnoreBlanks = true;
                    wb.Worksheet("Product").Column(9).SetDataValidation().List(rangebrand);

                    string cellbrand = wb.Worksheet("Varients").Column(1).Cell(2).Value.ToString();
                    wb.Worksheet("Product").Column(9).Cell(2).Value = cellbrand;
                    wb.Worksheet("Product").Column(9).Cell(3).Value = cellbrand;

                    string rangecolor = "'Varients'!B2:B" + countColor + "";
                    wb.Worksheet("Product_Stock").Column(3).SetDataValidation().IgnoreBlanks = true;
                    wb.Worksheet("Product_Stock").Column(3).SetDataValidation().List(rangecolor);

                    string cellcolor = wb.Worksheet("Varients").Column(2).Cell(2).Value.ToString();
                    string cellcolor1 = wb.Worksheet("Varients").Column(2).Cell(3).Value.ToString();
                    wb.Worksheet("Product_Stock").Column(3).Cell(2).Value = cellcolor;
                    wb.Worksheet("Product_Stock").Column(3).Cell(3).Value = cellcolor1;

                    string rangesize = "'Varients'!C2:C" + countSize + "";
                    wb.Worksheet("Product_Stock").Column(4).SetDataValidation().IgnoreBlanks = true;
                    wb.Worksheet("Product_Stock").Column(4).SetDataValidation().List(rangesize);

                    string cellsize = wb.Worksheet("Varients").Column(3).Cell(7).Value.ToString();
                    wb.Worksheet("Product_Stock").Column(4).Cell(2).Value = cellsize;
                    wb.Worksheet("Product_Stock").Column(4).Cell(3).Value = cellsize;

                    string rangedimension = "'Varients'!D2:D" + countDimension + "";
                    wb.Worksheet("Product_Stock").Column(5).SetDataValidation().IgnoreBlanks = true;
                    wb.Worksheet("Product_Stock").Column(5).SetDataValidation().List(rangedimension);

                    string celldimension = wb.Worksheet("Varients").Column(4).Cell(2).Value.ToString();
                    wb.Worksheet("Product_Stock").Column(5).Cell(2).Value = celldimension;
                    wb.Worksheet("Product_Stock").Column(5).Cell(3).Value = celldimension;

                    string rangematerial = "'Varients'!E2:E" + countMaterial + "";
                    wb.Worksheet("Product_Stock").Column(6).SetDataValidation().IgnoreBlanks = true;
                    wb.Worksheet("Product_Stock").Column(6).SetDataValidation().List(rangematerial);

                    string cellmaterial = wb.Worksheet("Varients").Column(5).Cell(2).Value.ToString();
                    wb.Worksheet("Product_Stock").Column(6).Cell(2).Value = cellmaterial;
                    wb.Worksheet("Product_Stock").Column(6).Cell(3).Value = cellmaterial;

                    string rangeproduct = "'Product'!B2:B1000";
                    wb.Worksheet("Product_Stock").Column(2).SetDataValidation().IgnoreBlanks = true;
                    wb.Worksheet("Product_Stock").Column(2).SetDataValidation().List(rangeproduct);

                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.Buffer = true;
                    HttpContext.Current.Response.Charset = "";
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=BulkUpload_" + ExcelName + ".xlsx");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                        HttpContext.Current.Response.Flush();
                        HttpContext.Current.Response.End();
                    }
                }
            }
            catch (Exception ex)
            {

                if (ex.InnerException != null)
                {

                    BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in function ExportData:" + ex.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

                }
                else if (ex.Message != null)
                {

                    BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in function ExportData :" + ex.Message, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

                }


            }
        }



        public DataTable GetProductTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Sr.No.", typeof(int));
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("CategoryName", typeof(string));
            dt.Columns.Add("WeightInGram", typeof(int));
            dt.Columns.Add("LengthInCm", typeof(int));
            dt.Columns.Add("BreadthInCm", typeof(int));
            dt.Columns.Add("HeightInCm", typeof(int));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("BrandName", typeof(string));
            dt.Columns.Add("SearchKeyword", typeof(string));
            dt.Columns.Add("DeliveryTime", typeof(long));
            dt.Columns.Add("DeliveryRate", typeof(decimal));
            dt.Columns.Add("TaxRate(Rs)", typeof(decimal));
            dt.Columns.Add("TaxRate(Per)", typeof(decimal));
            for (int i = 1; i < 3; i++)
            {
                DataRow row = dt.NewRow();
                row["Sr.No."] = i;
                row["WeightInGram"] = 1;
                row["LengthInCm"] = 1;
                row["BreadthInCm"] = 1;
                row["HeightInCm"] = 1;
                row["TaxRate(Rs)"] = 100;
                row["TaxRate(Per)"] = 10;
                if (i == 1)
                {
                    row["ProductName"] = "Product1";
                    row["Description"] = "This is Product 1";
                    row["SearchKeyword"] = ",Product1,";
                    row["DeliveryTime"] = 1;
                    row["DeliveryRate"] = 50;
                }
                else
                {
                    row["ProductName"] = "Product2";
                    row["Description"] = "This is Product 2";
                    row["SearchKeyword"] = ",Product2,";
                    row["DeliveryTime"] = 2;
                    row["DeliveryRate"] = 100;

                }
                dt.Rows.Add(row);
            }
            return dt;
        }
        public DataTable GetProductStockTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Sr.No.", typeof(int));
            dt.Columns.Add("Select Product", typeof(string));
            dt.Columns.Add("Select Color", typeof(string));
            dt.Columns.Add("Select Size", typeof(string));
            dt.Columns.Add("Select Dimension", typeof(string));
            dt.Columns.Add("Select Material", typeof(string));
            dt.Columns.Add("Stock Qty", typeof(int));
            dt.Columns.Add("Reorder Level", typeof(int));
            dt.Columns.Add("PackSize", typeof(decimal));
            dt.Columns.Add("MRP(Rs)", typeof(decimal));
            dt.Columns.Add("Sale Rate(Rs)", typeof(decimal));
            dt.Columns.Add("WholeSaleRate", typeof(decimal));

            for (int i = 1; i < 3; i++)
            {
                DataRow row = dt.NewRow();
                if (i == 1)
                {
                    row["Select Product"] = "Product1";
                    row["Stock Qty"] = 1;
                }
                else
                {
                    row["Select Product"] = "Product2";
                    row["Stock Qty"] = 2;
                }
                row["Sr.No."] = i;
                row["Reorder Level"] = 1;
                row["PackSize"] = 1;
                row["Sale Rate(Rs)"] = 100;
                row["MRP(Rs)"] = 110;
                row["WholeSaleRate"] = 100;
                dt.Rows.Add(row);
            }
            return dt;
        }

    }
}
