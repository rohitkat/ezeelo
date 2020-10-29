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


namespace BusinessLogicLayer
{
    public class BulkUpdateProductPrice
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public BulkUpdateProductPrice(System.Web.HttpServerUtility server)
        {
        }
        public void ExportData(DataTable tblProduct, string ExcelName)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                //wb.NamedRange("Product");
                wb.Worksheets.Add(tblProduct, "Product");
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

        public DataTable UpdatePrice(HttpPostedFileBase file, int shopID, out StringBuilder validationMsg)
        {
            validationMsg = new StringBuilder();
            DataTable dtSavedProduct = new DataTable();
            try
            {

                if (HttpContext.Current.Request.Files["file"].ContentLength > 0)
                {
                    string fileExtension =
                                         System.IO.Path.GetExtension(HttpContext.Current.Request.Files["file"].FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        //Save Excel file uploaded by merchant with unique name
                        Guid randomId = Guid.NewGuid();
                        string uniqueKey = randomId.ToString().Substring(0, 15).ToUpper();

                        string fileLocation = HttpContext.Current.Server.MapPath("~/Content/") + "ShopUpdatePrice_" + shopID + "_" + HttpContext.Current.Request.Files["file"].FileName + "_" + uniqueKey;

                        //Check if file already exists
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }

                        HttpContext.Current.Request.Files["file"].SaveAs(fileLocation);

                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        //connection String for xls file format.
                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        //connection String for xlsx file format.
                        else if (fileExtension == ".xlsx")
                        {

                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        //Create Connection to Excel work book and add oledb namespace
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }

                        String[] excelSheets = new String[dt.Rows.Count];

                        //Check Valid File Uploaded                  
                        DataView dv = new DataView(dt);

                        dv.RowFilter = "[TABLE_NAME]='Product$'";
                        if (dv.ToTable().Rows.Count > 0)
                        {
                            excelSheets[0] = dv.ToTable().Rows[0]["TABLE_NAME"].ToString();
                        }
                        else
                        {

                            validationMsg.AppendLine("Please upload valid excel file. Excel File must include sheet named Product");
                            return new DataTable();
                        }


                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);

                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        DataTable dtProduct = new DataTable();
                        DataTable dtfinalProduct = new DataTable();
                        DataTable tblProduct = new DataTable();
                        //tblProduct.Columns.Add("ProductID", typeof(long));
                        tblProduct.Columns.Add("ShopStockID", typeof(long));
                        tblProduct.Columns.Add("Mrp", typeof(decimal));
                        tblProduct.Columns.Add("SaleRate", typeof(decimal));
                        tblProduct.Columns.Add("WholeSaleRate", typeof(decimal));
                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(dtProduct);
                            //get Shopstock Id's of specific shop
                            //var ShopStockID = (from s in db.Shops
                            //                   join shps in db.ShopProducts on s.ID equals shps.ShopID
                            //                   join ss in db.ShopStocks on shps.ID equals ss.ShopProductID
                            //                   where s.ID == shopID
                            //                   select new
                            //                   {
                            //                       ss.ID
                            //                   }).FirstOrDefault();
                            //if (ShopStockID.ID > 0)
                            //{
                            //    long Id = Convert.ToInt64(ShopStockID.ID);
                            //    //find shopstockid get from above query is present in dtproduct table or not.If not present then it means excel sheet which we are gng to upload is not of selected shop
                            //    DataRow[] FindShopStock = dtProduct.Select("ShopStockID = " + Id + "");
                            //    if (FindShopStock.Count() == 0)
                            //    {
                            //        validationMsg.AppendLine("Please upload valid excel file. This Excel File doesnt related to selected Shop.");
                            //        return new DataTable();
                            //    }
                            //}

                            //get those rows from datatable which are not null.
                            var rows = from row in dtProduct.AsEnumerable()
                                       where row["NewWholeSaleRate"].GetType().Name != "DBNull" ||
                                       row["NewMrp"].GetType().Name != "DBNull" ||
                                       row["NewSaleRate"].GetType().Name != "DBNull"
                                       select row;
                            if (rows.Count() > 0)
                            {
                                dtfinalProduct = rows.CopyToDataTable();
                            }
                            else
                            {
                                validationMsg.AppendLine("Please upload valid excel file. Please Update Mrp,Salerate or WholeSaleRate of any product");
                                return new DataTable();
                            }
                            if (dtfinalProduct.Columns.Count == 15)
                            {
                                if (!dtfinalProduct.Columns.Contains("Sr#No#") || !dtfinalProduct.Columns.Contains("ShopStockID") || !dtfinalProduct.Columns.Contains("ProductName") || !dtfinalProduct.Columns.Contains("Color")
                                    || !dtfinalProduct.Columns.Contains("Size") || !dtfinalProduct.Columns.Contains("Dimension") || !dtfinalProduct.Columns.Contains("Material")
                                    || !dtfinalProduct.Columns.Contains("Quantity") || !dtfinalProduct.Columns.Contains("ReorderLevel") || !dtfinalProduct.Columns.Contains("Mrp")
                                    || !dtfinalProduct.Columns.Contains("SaleRate") || !dtfinalProduct.Columns.Contains("WholeSaleRate")
                                    || !dtfinalProduct.Columns.Contains("NewMrp") || !dtfinalProduct.Columns.Contains("NewSaleRate") || !dtfinalProduct.Columns.Contains("NewWholeSaleRate"))
                                {

                                    validationMsg.AppendLine("Please upload valid excel file. Product Sheet does not contains the required columns");
                                    return new DataTable();

                                }

                            }
                            else
                            {
                                validationMsg.AppendLine("Please upload valid excel file. Excel file does not contain the valid Product Sheet");
                                return new DataTable();
                            }



                            //If new value is not entered then repalace it with old value..
                            foreach (DataRow dr in dtfinalProduct.Rows)
                            {
                                object NewMrp = dr["NewMrp"];
                                if (NewMrp == DBNull.Value)
                                {
                                    object Mrp = dr["Mrp"];
                                    dr["NewMrp"] = Convert.ToDecimal(Mrp);
                                }

                                object NewSaleRate = dr["NewSaleRate"];
                                if (NewSaleRate == DBNull.Value)
                                {
                                    object SaleRate = dr["SaleRate"];
                                    dr["NewSaleRate"] = Convert.ToDecimal(SaleRate);
                                }

                                object NewWholeSaleRate = dr["NewWholeSaleRate"];
                                if (NewWholeSaleRate == DBNull.Value)
                                {
                                    object WholeSaleRate = dr["WholeSaleRate"];
                                    if (WholeSaleRate == DBNull.Value)
                                    {
                                        dr["NewWholeSaleRate"] = WholeSaleRate;
                                    }
                                    else
                                    {
                                        dr["NewWholeSaleRate"] = Convert.ToDecimal(WholeSaleRate);
                                    }
                                }
                            }
                            validationMsg = this.ValidatePrice(dtfinalProduct);
                            if (validationMsg.Length > 0)
                            {
                                return new DataTable();
                            }

                            //Select only those column which we wanted to pass to database.
                            var selectedColumn = from m in dtfinalProduct.AsEnumerable()
                                                 select new
                                                 {
                                                     ShopstockID = m.Field<object>("ShopStockID"),
                                                     Mrp = m.Field<object>("NewMrp"),
                                                     SaleRate = m.Field<object>("NewSaleRate"),
                                                     WholeSaleRate = m.Field<object>("NewWholeSaleRate")
                                                 };

                            foreach (var row in selectedColumn)
                                tblProduct.LoadDataRow(new object[] { row.ShopstockID, row.Mrp, row.SaleRate, row.WholeSaleRate }, false);
                            long PersonalID = GetPersonalDetailID();
                            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                            string conn = readCon.DB_CONNECTION;
                            SqlConnection con = new SqlConnection(conn);
                            string sp = "[BulkUpdateProductStock]";

                            try
                            {
                                using (SqlConnection connection = new SqlConnection(conn))
                                {
                                    SqlCommand cmd = new SqlCommand(sp, con);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    //cmd.Parameters.AddWithValue("@ShopID", shopID);
                                    cmd.Parameters.AddWithValue("@tblBulkProductStock", tblProduct);
                                    cmd.Parameters.AddWithValue("@ModifyBy", 1);
                                    cmd.Parameters.AddWithValue("@NetworkIP", CommonFunctions.GetClientIP());
                                    using (SqlDataAdapter sda = new SqlDataAdapter())
                                    {
                                        cmd.Connection = con;
                                        sda.SelectCommand = cmd;
                                        sda.Fill(dtSavedProduct);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                validationMsg = new StringBuilder();
                                if (ex.InnerException != null)
                                {
                                    validationMsg.AppendLine("Server Error :" + ex.InnerException.ToString());
                                    BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Uploading Product with Stock in Bulk :" + ex.InnerException, ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                                }
                                else if (ex.Message != null)
                                {
                                    validationMsg.AppendLine("Server Error :" + ex.Message.ToString());
                                    BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Uploading Product with Stock in Bulk :" + ex.Message, ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                                }

                                return new DataTable();
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                // validationMsg = new StringBuilder();
                if (ex.InnerException != null)
                {
                    //validationMsg.AppendLine("Server Error :" + ex.InnerException.ToString());
                    BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Uploading Product with Stock in Bulk :" + ex.InnerException, ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                }
                else if (ex.Message != null)
                {
                    //validationMsg.AppendLine("Server Error :" + ex.Message.ToString());
                    BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Uploading Product with Stock in Bulk :" + ex.Message, ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                }

                return new DataTable();
            }
            return dtSavedProduct;

        }


        private StringBuilder ValidatePrice(DataTable dtproduct)
        {
            StringBuilder msg = new StringBuilder();
            decimal mrp = 0;
            decimal salerate = 0;
            var count = 0;
            int SrNo = 0;
            StringBuilder sbsr = new StringBuilder();
            foreach (DataRow row in dtproduct.Rows)
            {
                mrp = Convert.ToDecimal(row["NewMrp"]);
                salerate = Convert.ToDecimal(row["NewSaleRate"]);
                if (mrp < salerate)
                {
                    count++;
                    sbsr.Append(row["Sr#No#"] + ",");
                    SrNo++;
                }
            }
            if (count > 0)
            {
                msg.AppendLine("<br/> SaleRate greater than MRP Found : ");
                msg.Append("(" + count + ")");
                msg.AppendLine("Refer Sr.No. :" + sbsr + " ");
            }
            //var Rates = (from row in dtproduct.AsEnumerable()
            //             where (row.Field<decimal?>("NewMrp") < row.Field<decimal?>("NewSaleRate"))
            //             select row).ToList();


            //if (Rates.Count() > 0)
            //{
            //    msg.AppendLine("<br/> SaleRate greater than MRP Found : ");
            //    msg.Append("(" + Rates.Count() + ")");
            //    msg.AppendLine("Refer Sr.No. : ");
            //    foreach (var item in Rates)
            //    {
            //        msg.Append(item.Field<double>("Sr#No#") + " ; ");
            //    }

            //}

            //Rates = (from row in dtproduct.AsEnumerable()
            //         where (row.Field<double>("NewMrp") < row.Field<double>("NewWholeSaleRate"))
            //         select row).ToList();
            //if (Rates.Count() > 0)
            //{
            //    msg.AppendLine("<br/> WholeSaleRate greater than MRP Found : ");
            //    msg.Append("(" + Rates.Count() + ")");
            //    msg.AppendLine("Refer Sr.No. : ");
            //    foreach (var item in Rates)
            //    {
            //        msg.Append(item.Field<double>("Sr#No#") + " ; ");
            //    }

            //}

            //Rates = (from row in dtproduct.AsEnumerable()
            //         where (row.Field<double>("NewMrp") < row.Field<double>("NewWholeSaleRate"))
            //         select row).ToList();
            //if (Rates.Count() > 0)
            //{
            //    msg.AppendLine("<br/> WholeSaleRate greater than Sale Rate Found : ");
            //    msg.Append("(" + Rates.Count() + ")");
            //    msg.AppendLine("Refer Sr.No. : ");
            //    foreach (var item in Rates)
            //    {
            //        msg.Append(item.Field<double>("Sr#No#") + " ; ");
            //    }

            //}
            return msg;
        }
        private long GetPersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            long UserLoginID = Convert.ToInt32(System.Web.HttpContext.Current.Session["USER_LOGIN_ID"]);
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
                throw new BusinessLogicLayer.MyException("[BulkUpdateProductPrice][GetPersonalDetailID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }
    }



}

