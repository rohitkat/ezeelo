using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ClosedXML;
using ClosedXML.Excel;
using System.IO;
using System.Reflection;
using System.Data.OleDb;
using System.Text;
using ModelLayer.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;


namespace BusinessLogicLayer
{
    public class ExportExcelCsv
    {
        public ExportExcelCsv(System.Web.HttpServerUtility server)
        {
        }

        public void ExportToExcel(DataTable dt, string ExcelName)
        {
            try
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    //wb.NamedRange("Product");
                    wb.Worksheets.Add(dt, "Sheet1");
                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.Buffer = true;
                    HttpContext.Current.Response.Charset = "";
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename= " + ExcelName + ".xlsx");

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
                throw new BusinessLogicLayer.MyException("[ExportToExcel]", "Can't Export Excel file !" + Environment.NewLine + ex.Message);
            }
        }
        //Export to csv file
        public void ExportToCSV(DataTable dt, string ExcelName)
        {
            try
            {
                HttpContext.Current.Response.ContentType = "Application/x-msexcel";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + ExcelName + ".csv");
                HttpContext.Current.Response.Write(ExportToCSVFile(dt));
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ExportToCSV]", "Can't Export CSV file !" + Environment.NewLine + ex.Message);
            }
        }
        //Get pdf file
        public void ExportToPDF(DataTable dt, string ExcelName)
        {
            try
            {
                Document pdfDoc = new Document(PageSize.A4, 30, 30, 40, 25);
                System.IO.MemoryStream mStream = new System.IO.MemoryStream();
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, mStream);
                int cols = dt.Columns.Count;
                int rows = dt.Rows.Count;
                pdfDoc.Open();

                iTextSharp.text.Table pdfTable = new iTextSharp.text.Table(cols, rows);
                pdfTable.BorderWidth = 1;
                pdfTable.Width = 100;
                pdfTable.Padding = 1;
                pdfTable.Spacing = 1;

                //creating table headers
                for (int i = 0; i < cols; i++)
                {
                    Cell cellCols = new Cell();
                    Font ColFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, Font.BOLD);
                    Chunk chunkCols = new Chunk(dt.Columns[i].ColumnName, ColFont);
                    cellCols.Add(chunkCols);
                    pdfTable.AddCell(cellCols);

                }
                //creating table data (actual result)
                for (int k = 0; k < rows; k++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        Cell cellRows = new Cell();
                        Font RowFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                        Chunk chunkRows = new Chunk(dt.Rows[k][j].ToString(), RowFont);
                        cellRows.Add(chunkRows);
                        pdfTable.AddCell(cellRows);

                    }
                }

                pdfDoc.Add(pdfTable);
                pdfDoc.Close();
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename= " + ExcelName + ".pdf");
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.BinaryWrite(mStream.ToArray());
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ExportToPDF]", "Can't Export PDF file !" + Environment.NewLine + ex.Message);
            }
        }



        private string ExportToCSVFile(DataTable dtTable)
        {
            StringBuilder sbldr = new StringBuilder();
            if (dtTable.Columns.Count != 0)
            {
                foreach (DataColumn col in dtTable.Columns)
                {
                    sbldr.Append(col.ColumnName + ',');
                }
                sbldr.Append("\r\n");
                foreach (DataRow row in dtTable.Rows)
                {
                    foreach (DataColumn column in dtTable.Columns)
                    {
                        sbldr.Append(row[column].ToString() + ',');
                    }
                    sbldr.Append("\r\n");
                }
            }
            return sbldr.ToString();
        }
    }
}
