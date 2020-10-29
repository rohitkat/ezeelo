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
/*
 Handed over to Harshada
 */
namespace BusinessLogicLayer
{

    public class UploadBulkProductExcel : ProductDisplay
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public UploadBulkProductExcel(System.Web.HttpServerUtility server) : base(server) { }
        /// <summary>
        /// Upload data from excel sheets in database
        /// </summary>
        /// <param name="file">Posted file</param>
        /// <param name="shopID">shop ID for which poducts to be uploaded</param>
        /// <param name="validationMsg">Validation faild message</param>
        /// <returns></returns>
        public DataTable UploadProductWithStock(HttpPostedFileBase file, long shopID, out StringBuilder validationMsg, out StringBuilder GbProductMsg)
        {
            validationMsg = new StringBuilder();
            GbProductMsg = new StringBuilder();

            //Shop is must be provided
            if (shopID == 0)
            {
                validationMsg.AppendLine("Please select Shop.");
                return new DataTable();

            }
            DataTable dtSavedProducts = new DataTable();
            DataSet ds = new DataSet();
            //save excel file on local server, in order to read file data
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

                        string fileLocation = HttpContext.Current.Server.MapPath("~/Content/") + "Shop_" + shopID + "_" + HttpContext.Current.Request.Files["file"].FileName + "_" + uniqueKey;

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
                        //check Product_Stock sheet
                        dv.RowFilter = "[TABLE_NAME]='Product_Stock$'";
                        if (dv.ToTable().Rows.Count > 0)
                        {
                            excelSheets[0] = dv.ToTable().Rows[0]["TABLE_NAME"].ToString();
                        }
                        else
                        {

                            validationMsg.AppendLine("Please upload valid excel file. Excel File must include sheet named Product Stock ");
                            return new DataTable();
                        }
                        dv = new DataView(dt);
                        //Check product sheet
                        dv.RowFilter = "[TABLE_NAME]='Product$'";
                        if (dv.ToTable().Rows.Count > 0)
                        {
                            excelSheets[1] = dv.ToTable().Rows[0]["TABLE_NAME"].ToString();
                        }
                        else
                        {

                            validationMsg.AppendLine("Please upload valid excel file. Excel File must include sheet named Product");
                            return new DataTable();
                        }

                        //Check sheet name is altered
                        if (excelSheets[0].ToString() != "Product_Stock$" || excelSheets[1].ToString() != "Product$")
                        {
                            validationMsg.AppendLine("Please upload valid excel file. Excel File must include two sheets named Product & Product Stock ");
                            return new DataTable();
                        }
                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);

                        string query = string.Format("Select * from [{0}]", excelSheets[1]);
                        DataTable dtProduct = new DataTable();
                        DataTable dtFinalProduct = new DataTable();
                        DataTable dtwithoutGbProd = new DataTable();
                        string str = "";
                        string strTempprod = "";
                        string strPropprod = "";
                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(dtProduct);

                            //Changes by Harshada for gbcatlog
                            //******************************************************************************************
                            //get All products from Product Table
                            var queryProd = (from p in db.Products
                                             select new
                                             {
                                                 p.Name
                                             }).ToList();
                            DataTable product = LINQResultToDataTable(queryProd);

                            //Query to remove product which are available in Product Table in Database
                            //means product of gbCatlog
                            var GbCatProdRows = dtProduct.AsEnumerable()
                                              .Where(ra => !product.AsEnumerable()
                                             .Any(rb => rb.Field<string>("Name") == ra.Field<string>("ProductName")));
                            if (GbCatProdRows.Count() > 0)
                            {

                                dtFinalProduct = dtProduct.AsEnumerable()
                                                  .Where(ra => !product.AsEnumerable()
                                                 .Any(rb => rb.Field<string>("Name") == ra.Field<string>("ProductName")))
                                                 .CopyToDataTable();
                                dtwithoutGbProd = dtFinalProduct.Copy();
                            }
                            else
                            {
                                dtwithoutGbProd = dtProduct.Copy();
                            }
                            //Query to get Products which are present in GbCatlog.These Product Name will be useful to show in Error Message.
                            var idsNotInB = dtProduct.AsEnumerable().Select(r => r.Field<string>("ProductName"))
                                           .Intersect(product.AsEnumerable().Select(r => r.Field<string>("Name")));
                            if (idsNotInB.Count() > 0)
                            {
                                DataTable dtwithGbCatProduct = (from row in dtProduct.AsEnumerable()
                                                                join id in idsNotInB
                                                                on row.Field<string>("ProductName") equals id
                                                                select row).CopyToDataTable();
                                if (dtwithGbCatProduct.Rows.Count > 0)
                                {
                                    foreach (DataRow dr in dtwithGbCatProduct.Rows)
                                    {
                                        str = str + "," + Convert.ToString(dr["ProductName"]);

                                    }
                                    str = str.Trim().TrimStart(',');
                                    GbProductMsg.AppendLine("'" + str + "' could not uploaded succesfully because they are the GBCatlog Products. Please Upload it one by one.");
                                }

                                if (dtProduct.Rows.Count == 1 || GbCatProdRows.Count() == 0)
                                {
                                    dtFinalProduct = dtFinalProduct.Clone();
                                    return dtFinalProduct;
                                }
                            }

                            //*****************************************************************************
                            //End of Gb Catlog

                            //Start of Check Of Proprietory Products
                            //*******************************************************************
                            //Query to remove product which are available in Priority product Table in Database
                            var queryProprietoryProd = (from pp in db.ProprietoryProducts
                                                        join p in db.Products on pp.ProductID equals p.ID
                                                        //where pp.ShopID == shopID
                                                        select new
                                                        {
                                                            p.Name
                                                        });
                            DataTable ProprietoryProd = LINQResultToDataTable(queryProprietoryProd);
                            var propquery = dtwithoutGbProd.AsEnumerable()
                                              .Where(ra => ProprietoryProd.AsEnumerable()
                                             .Any(rb => rb.Field<string>("Name") == ra.Field<string>("ProductName")));

                            if (propquery.Count() > 0)
                            {
                                dtFinalProduct = dtwithoutGbProd.AsEnumerable()
                                                  .Where(ra => ProprietoryProd.AsEnumerable()
                                                 .Any(rb => rb.Field<string>("Name") == ra.Field<string>("ProductName")))
                                                 .CopyToDataTable();
                                //dtwithoutGbProd = dtFinalProduct.Copy();
                            }

                            ////Query to get Products which are present in Proprietory table for selected shop
                            //var idinProprietory = dtwithoutGbProd.AsEnumerable().Select(r => r.Field<string>("ProductName"))
                            //                   .Intersect(ProprietoryProd.AsEnumerable().Select(r => r.Field<string>("Name")));

                            if (propquery.Count() > 0)
                            {
                                foreach (DataRow dr in dtFinalProduct.Rows)
                                {
                                    strPropprod = strPropprod + "," + dr["Name"];

                                }
                                strPropprod = strPropprod.Trim().TrimStart(',');
                                GbProductMsg.AppendLine("<br/>'" + strPropprod + "' could not uploaded succesfully because they are Proprietory product for other Shop.Please Upload them by other Name.");
                            }

                            //*******************************************************************
                            //End Of Proprietory Products


                            //Change to get product which are already present in database for particular shop or selected shop (TempProduct Table)
                            //Remove that product and pass others product to process

                            //*****************************************************************************************
                            //Get All products in TempShopProduct
                            var queryTempShopProd = (from tsp in db.TempShopProducts
                                                     where tsp.ShopID == shopID
                                                     select new
                                                     {
                                                         ID = tsp.ProductID
                                                     });
                            var tempProd = (from tp in db.TempProducts
                                            join qts in queryTempShopProd on tp.ID equals qts.ID
                                            select new
                                            {

                                                tp.Name

                                            }).OrderByDescending(x => x.Name).ToList();

                            DataTable tempproduct = LINQResultToDataTable(tempProd);//Convert Linq result to datatable

                            //Query to remove product which are available in tempProduct Table in Database
                            var rows = dtwithoutGbProd.AsEnumerable()
                                                  .Where(ra => !tempproduct.AsEnumerable()
                                                 .Any(rb => rb.Field<string>("Name") == ra.Field<string>("ProductName")));


                            //Query to get Products which are present in Tempproduct for selected shop
                            var idinTemp = dtwithoutGbProd.AsEnumerable().Select(r => r.Field<string>("ProductName"))
                                               .Intersect(tempproduct.AsEnumerable().Select(r => r.Field<string>("Name")));

                            if (idinTemp.Count() > 0)
                            {
                                foreach (string s in idinTemp)
                                {
                                    strTempprod = strTempprod + "," + s;

                                }
                                strTempprod = strTempprod.Trim().TrimStart(',');
                                GbProductMsg.AppendLine("<br/>'" + strTempprod + "' could not uploaded succesfully because they are Already Prsent in database for selected Shop.Please add those product one by one from GbCatlog");
                            }
                            //if finally we got product to upload.if rows count is greater than 0 then in dtFinalproduct we will get products otherwise pass empty datatable.
                            if (rows.Count() > 0)
                            {
                                dtFinalProduct = dtwithoutGbProd.AsEnumerable()
                                                      .Where(ra => !tempproduct.AsEnumerable()
                                                     .Any(rb => rb.Field<string>("Name") == ra.Field<string>("ProductName")))
                                                     .CopyToDataTable();

                            }

                            else
                            {
                                dtFinalProduct = dtFinalProduct.Clone();
                                return dtFinalProduct;
                            }

                            //End of Same product for particular shop(tempproduct)

                            foreach (DataRow dr in dtFinalProduct.Rows)
                            {
                                StringBuilder Desc = new StringBuilder();
                                string arr = dr["Description"].ToString();
                                string[] Description = arr.Split(',');
                                Desc.Append("<div>");
                                Desc.Append("<Ul>");
                                foreach (var v in Description)
                                {
                                    Desc.Append("<li>" + v + "</li>");
                                    //Desc.Append("<li><span style=font-size: 13.3333px; font-family: Arial, Verdana;>" + v + "</span></li>");
                                }
                                //foreach (var v in Description)
                                //{
                                //    Desc.Append("<li><span style=font-size: 13.3333px; font-family: Arial, Verdana;>" + v + "</span></li>");
                                //}
                                Desc.Append("</Ul>");
                                Desc.Append("</div>");
                                dr["Description"] = Desc;
                            }
                            ds.Tables.Add(dtFinalProduct.Rows.Cast<DataRow>()
                                                   .Where(row => !row.ItemArray.All(field => field is System.DBNull))
                                                   .CopyToDataTable());

                        }

                        //check Sheet1 Columns are not altered and contains required column
                        if (dtFinalProduct.Columns.Count == 14)
                        {
                            if (!dtFinalProduct.Columns.Contains("Sr#No#") || !dtFinalProduct.Columns.Contains("ProductName") || !dtFinalProduct.Columns.Contains("CategoryName")
                                || !dtFinalProduct.Columns.Contains("WeightInGram") || !dtFinalProduct.Columns.Contains("LengthInCm") || !dtFinalProduct.Columns.Contains("BreadthInCm")
                                || !dtFinalProduct.Columns.Contains("HeightInCm") || !dtFinalProduct.Columns.Contains("Description") || !dtFinalProduct.Columns.Contains("BrandName")
                                || !dtFinalProduct.Columns.Contains("SearchKeyword") || !dtFinalProduct.Columns.Contains("DeliveryTime")
                                || !dtFinalProduct.Columns.Contains("DeliveryRate") || !dtFinalProduct.Columns.Contains("TaxRate(Rs)") || !dtFinalProduct.Columns.Contains("TaxRate(Per)"))
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

                        query = string.Format("Select * from [{0}]", excelSheets[0]);
                        DataTable dtProductStock = new DataTable();
                        DataTable dtGbProductStock = new DataTable();

                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(dtProductStock);
                            dtProductStock.Columns.Add("PackSize Unit", typeof(string)).SetOrdinal(9);

                            foreach (DataRow dr in dtProductStock.Rows)
                            {

                                dr["PackSize Unit"] = "QTY";
                            }
                            //Query to get stock of those product only which are present in ProductSheet(dtFinalProduct)
                            var idsNotInB = dtFinalProduct.AsEnumerable().Select(r => r.Field<string>("ProductName"))
                                          .Intersect(dtProductStock.AsEnumerable().Select(r => r.Field<string>("Select Product")));
                            if (idsNotInB.Count() > 0)
                            {

                                dtGbProductStock = (from row in dtProductStock.AsEnumerable()
                                                    join id in idsNotInB
                                                    on row.Field<string>("Select Product") equals id
                                                    select row).CopyToDataTable();
                            }


                            ds.Tables.Add(dtGbProductStock.Rows.Cast<DataRow>()
                                         .Where(row => !row.ItemArray.All(field => field is System.DBNull))
                                         .CopyToDataTable());

                        }



                        //check Sheet2 Columns are not altered and contains required column
                        if (dtGbProductStock.Columns.Count == 13)
                        {
                            if (!dtGbProductStock.Columns.Contains("Sr#No#") || !dtGbProductStock.Columns.Contains("Select Product") || !dtGbProductStock.Columns.Contains("Select Color")
                                || !dtGbProductStock.Columns.Contains("Select Size") || !dtGbProductStock.Columns.Contains("Select Dimension") || !dtGbProductStock.Columns.Contains("Select Material")
                                || !dtGbProductStock.Columns.Contains("Stock Qty") || !dtGbProductStock.Columns.Contains("Reorder Level") || !dtGbProductStock.Columns.Contains("PackSize")
                                || !dtGbProductStock.Columns.Contains("PackSize Unit") || !dtGbProductStock.Columns.Contains("MRP(Rs)") || !dtGbProductStock.Columns.Contains("Sale Rate(Rs)") || !dtGbProductStock.Columns.Contains("WholeSaleRate"))
                            {

                                validationMsg.AppendLine("Please upload valid excel file. Product Stock Sheet does not contains the required columns");
                                return new DataTable();

                            }

                        }
                        else
                        {
                            validationMsg.AppendLine("Please upload valid excel file. Excel file does not contain the valid Product Stock Sheet");
                            return new DataTable();
                        }
                    }
                    if (fileExtension.ToString().ToLower().Equals(".xml"))
                    {
                        string fileLocation = HttpContext.Current.Server.MapPath("~/Content/") + HttpContext.Current.Request.Files["FileUpload"].FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }

                        HttpContext.Current.Request.Files["FileUpload"].SaveAs(fileLocation);
                        XmlTextReader xmlreader = new XmlTextReader(fileLocation);
                        // DataSet ds = new DataSet();
                        ds.ReadXml(xmlreader);
                        xmlreader.Close();
                    }

                    //Validate Data            
                    validationMsg.AppendLine(ValidateProductSheet(ds.Tables[0]).ToString());
                    validationMsg.AppendLine(ValidateStockSheet(ds.Tables[1]).ToString());

                    //Check If stock is available for all product present in product sheet and vice versa
                    if (validationMsg.ToString().Trim().Equals(string.Empty))
                    {
                        validationMsg.AppendLine(ValidateStockPresentForProduct(ds.Tables[0], ds.Tables[1]).ToString());
                    }
                    else return new DataTable();

                    //Check data entered in product sheet whether present in database tables and active
                    if (validationMsg.ToString().Trim().Equals(string.Empty))
                    {
                        validationMsg.AppendLine(ValidateProductSheetData(ds.Tables[0], shopID).ToString());
                    }
                    else return new DataTable();

                    //Check data entered in Stock sheet whether present in database tables and active
                    if (validationMsg.ToString().Trim().Equals(string.Empty))
                    {
                        validationMsg.AppendLine(ValidateStockSheetData(ds.Tables[1], shopID).ToString());
                    }
                    else return new DataTable();


                    if (!validationMsg.ToString().Trim().Equals(string.Empty)) return new DataTable();


                    if (ds.Tables.Count < 2) return null;
                    //send these data to store procedure
                    ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                    string conn = readCon.DB_CONNECTION;
                    SqlConnection con = new SqlConnection(conn);
                    string sp = "[BulkUploadProductAndStock]";

                    try
                    {
                        using (SqlConnection connection = new SqlConnection(conn))
                        {
                            SqlCommand cmd = new SqlCommand(sp, con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ShopID", shopID);
                            cmd.Parameters.AddWithValue("@tblBulkProduct", ds.Tables[0]);
                            cmd.Parameters.AddWithValue("@tblBulkShopStock", ds.Tables[1]);
                            cmd.Parameters.AddWithValue("@ExcelSheetName", "sheet");
                            cmd.Parameters.AddWithValue("@CreateBy", 1);
                            cmd.Parameters.AddWithValue("@NetworkIP", CommonFunctions.GetClientIP());
                            cmd.Parameters.AddWithValue("@DeviceType", "Desktop");
                            cmd.Parameters.AddWithValue("@DeviceID", null);

                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                cmd.Connection = con;
                                sda.SelectCommand = cmd;
                                sda.Fill(dtSavedProducts);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        validationMsg = new StringBuilder();
                        if (ex.InnerException != null)
                        {
                            validationMsg.AppendLine("Server Error :" + ex.InnerException.ToString());
                            BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Uploading Product with Stock in Bulk :" + ex.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

                        }
                        else if (ex.Message != null)
                        {
                            validationMsg.AppendLine("Server Error :" + ex.Message.ToString());
                            BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Uploading Product with Stock in Bulk :" + ex.Message, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

                        }

                        return new DataTable();
                    }

                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Uploading Product with Stock in Bulk :" + ex.Message, ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return dtSavedProducts;

        }

        public DataTable LINQResultToDataTable<T>(IEnumerable<T> Linqlist)
        {

            DataTable dt = new DataTable();
            try
            {

                PropertyInfo[] columns = null;

                if (Linqlist == null) return dt;

                foreach (T Record in Linqlist)
                {

                    if (columns == null)
                    {
                        columns = ((Type)Record.GetType()).GetProperties();
                        foreach (PropertyInfo GetProperty in columns)
                        {
                            Type colType = GetProperty.PropertyType;

                            if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                            == typeof(Nullable<>)))
                            {
                                colType = colType.GetGenericArguments()[0];
                            }

                            dt.Columns.Add(new DataColumn("Name", colType));
                        }
                    }

                    DataRow dr = dt.NewRow();

                    foreach (PropertyInfo pinfo in columns)
                    {
                        dr[pinfo.Name] = pinfo.GetValue(Record, null) == null ? DBNull.Value : pinfo.GetValue
                        (Record, null);
                    }

                    dt.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[UploadBulkProduct][LINQResultToDataTable]", "Unable to conver linq result to datatable. !" + Environment.NewLine + ex.Message);
            }
            return dt;
        }
        /// <summary>
        /// Check for for every product there should be stock present in Product_Stock Sheet
        /// </summary>
        /// <param name="product">Product dataTable</param>
        /// <param name="stock">Stock dataTable</param>
        /// <returns></returns>
        private StringBuilder ValidateStockPresentForProduct(DataTable product, DataTable stock)
        {

            StringBuilder msg = new StringBuilder();
            try
            {
                var productList = from n1 in product.AsEnumerable() select n1.Field<string>("ProductName");

                var stockList = from n in stock.AsEnumerable() select n.Field<string>("Select Product");


                //product for which stock not available
                var result = productList.Except(stockList);
                if (result != null && result.Count() > 0)
                {
                    msg.AppendLine("<br/> Products Without Stock Found i.e There is no entry for products in product_Stock Sheet for all products in Product Sheet: ");
                    msg.Append("(" + result.Count() + ") ");
                    msg.AppendLine("<br/> Refer Product Name in Product Sheet : ");
                    foreach (var item in result)
                    {
                        msg.Append(item + " ; ");
                    }

                }
                //Stock for which product details not available
                result = stockList.Except(productList);
                if (result != null && result.Count() > 0)
                {
                    msg.AppendLine("<br/> Stock Without Product Details Found : ");
                    msg.Append("(" + result.Count() + ") ");
                    msg.AppendLine("<br/> Refer Product Name in Product-Stock Sheet : ");
                    foreach (var item in result)
                    {
                        msg.Append(item + " ; ");
                    }

                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in validating stock present in product :" + ex.Message, ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return msg;


        }
        /// <summary>
        /// validate product sheet uploaded
        /// </summary>
        /// <param name="product">product datatable</param>
        /// <returns></returns>
        private StringBuilder ValidateProductSheet(DataTable product)
        {
            StringBuilder msg = new StringBuilder();
            try
            {
                if (product.Rows.Count == 0)
                {
                    msg.AppendLine("<br/> Please fill product sheet.");
                    return msg;
                }

                var isAppropriateSrNo = (from row in product.AsEnumerable()
                                         where (row.Field<object>("Sr#No#") == null)
                                         select row).ToList();
                if (isAppropriateSrNo.Count == 0)
                {
                    isAppropriateSrNo = (from row in product.AsEnumerable()
                                         where (row.Field<double>("Sr#No#") == 0)
                                         select row).ToList();
                    if (isAppropriateSrNo.Count > 0)
                    {
                        msg.AppendLine("<br/> Please check Sr.No. It cannot be empty.");
                    }
                    else
                    {
                        //Check Required validations
                        //ProductName Not entered
                        var requiredValidation = (from row in product.AsEnumerable()
                                                .Where(x => x.Field<object>("ProductName") == null)
                                                  select row).ToList();

                        if (requiredValidation.Count() > 0)
                        {
                            msg.AppendLine("<br/> Product Name Required Validation Failed in Product Sheet : ");
                            msg.Append("(" + requiredValidation.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in requiredValidation)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                            return msg;
                        }

                        var duplicateProdcts = product.AsEnumerable().GroupBy(i => new { ProductName = i.Field<string>("ProductName") }).Where(g => g.Count() > 1).Select(g => new { g.Key.ProductName }).ToList();
                        if (duplicateProdcts.Count > 0)
                        {
                            msg.AppendLine("<br/> Duplicate Products found : ");
                            msg.Append("(" + duplicateProdcts.Count() + ") ");
                            msg.AppendLine("Refer Product Name : ");
                            foreach (var item in duplicateProdcts)
                            {
                                msg.AppendLine(item.ProductName + " ; ");
                            }
                        }
                        //Product Name
                        requiredValidation = (from row in product.AsEnumerable()
                                                 .Where(x => x.Field<string>("ProductName") == string.Empty)
                                              select row).ToList();

                        if (requiredValidation.Count() > 0)
                        {
                            msg.AppendLine("<br/> Product Name Required Validation Failed : ");
                            msg.Append("(" + requiredValidation.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in requiredValidation)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                        }


                        //CategoryName Not entered
                        requiredValidation = (from row in product.AsEnumerable()
                                               .Where(x => x.Field<object>("CategoryName") == null)
                                              select row).ToList();

                        if (requiredValidation.Count() > 0)
                        {
                            msg.AppendLine("<br/> Category Name Required Validation Failed in Product Sheet : ");
                            msg.Append("(" + requiredValidation.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in requiredValidation)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                            return msg;
                        }

                        //CategoryName Invalid
                        requiredValidation = (from row in product.AsEnumerable()
                                                 .Where(x => x.Field<string>("CategoryName") == string.Empty)
                                              select row).ToList();

                        if (requiredValidation.Count() > 0)
                        {
                            msg.AppendLine("<br/> Category Name Required Validation Failed : ");
                            msg.Append("(" + requiredValidation.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in requiredValidation)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                        }

                        //Check weights empty i.e not entered
                        var weightsUnavailable = (from row in product.AsEnumerable()
                                                  where (row.Field<object>("WeightInGram") == null)
                                                  || (row.Field<object>("LengthInCm") == null)
                                                  || (row.Field<object>("BreadthInCm") == null)
                                                  || (row.Field<object>("HeightInCm") == null)
                                                  select row).ToList();


                        if (weightsUnavailable.Count() > 0)
                        {
                            msg.AppendLine("<br/> Weight and Other Dimensions Required Validation Failed: ");
                            msg.Append("(" + weightsUnavailable.Count() + ") ");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in weightsUnavailable)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                        }
                        else
                        {
                            //entered but wrong
                            weightsUnavailable = (from row in product.AsEnumerable()
                                                  where (row.Field<object>("WeightInGram") != null && row.Field<double>("WeightInGram") < 0)
                                                       || (row.Field<object>("LengthInCm") != null && row.Field<double>("LengthInCm") < 0)
                                                       || (row.Field<object>("BreadthInCm") != null && row.Field<double>("BreadthInCm") < 0)
                                                       || (row.Field<object>("HeightInCm") != null && row.Field<double>("HeightInCm") < 0)
                                                  select row).ToList();
                            if (weightsUnavailable.Count() > 0)
                            {
                                msg.AppendLine("<br/> Negative Weight and Other Dimensions Found : ");
                                msg.Append("(" + weightsUnavailable.Count() + ") ");
                                msg.AppendLine("Refer Sr.No. : ");
                                foreach (var item in weightsUnavailable)
                                {
                                    msg.Append(item.Field<double>("Sr#No#") + " ; ");
                                }

                            }
                        }

                    }
                }
                else
                {

                    msg.AppendLine("Please check Sr.No. It cannot be empty. Empty rows are not allowed in sheet.");
                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in validating  product sheet :" + ex.Message, ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return msg;

        }
        /// <summary>
        /// validate product_stock sheet uploaded
        /// </summary>
        /// <param name="product">Stock datatable</param>
        /// <returns></returns>
        private StringBuilder ValidateStockSheet(DataTable stock)
        {
            StringBuilder msg = new StringBuilder();
            try
            {
                if (stock.Rows.Count == 0)
                {
                    msg.AppendLine("<br/> Please fill Product Stock sheet.");
                    return msg;
                }


                var isAppropriateSrNo = (from row in stock.AsEnumerable()
                                         where (row.Field<object>("Sr#No#") == null)
                                         select row).ToList();
                if (isAppropriateSrNo.Count == 0)
                {
                    isAppropriateSrNo = (from row in stock.AsEnumerable()
                                         where (row.Field<double>("Sr#No#") == 0)
                                         select row).ToList();
                    if (isAppropriateSrNo.Count > 0)
                    {
                        msg.AppendLine("<br/> Please check Sr.No. It cannot be empty.");
                    }
                    else
                    {

                        //Check Required validations
                        //ProductName Not entered
                        var requiredValidation = (from row in stock.AsEnumerable()
                                                .Where(x => x.Field<object>("Select Product") == null)
                                                  select row).ToList();

                        if (requiredValidation.Count() > 0)
                        {
                            msg.AppendLine("<br/> Product Name Required Validation Failed in Product Stock Sheet : ");
                            msg.Append("(" + requiredValidation.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in requiredValidation)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                            return msg;
                        }


                        // invalid productname like empty/blank
                        requiredValidation = (from row in stock.AsEnumerable()
                                                    .Where(x => x.Field<object>("Select Product") != null && x.Field<string>("Select Product") == string.Empty)
                                              select row).ToList();

                        if (requiredValidation.Count() > 0)
                        {
                            msg.AppendLine("<br/> Invalid Product Name in Product Stock Sheet Found : ");
                            msg.Append("(" + requiredValidation.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in requiredValidation)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                        }
                        //ColorName Not entered
                        requiredValidation = (from row in stock.AsEnumerable()
                                                .Where(x => x.Field<object>("Select Color") == null)
                                              select row).ToList();

                        if (requiredValidation.Count() > 0)
                        {
                            msg.AppendLine("<br/> Color Name Required Validation Failed : ");
                            msg.Append("(" + requiredValidation.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in requiredValidation)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                            return msg;
                        }

                        //Invalid color name e.g. Empty
                        requiredValidation = (from row in stock.AsEnumerable()
                                                 .Where(
                                                x => x.Field<object>("Select Color") != null && x.Field<string>("Select Color") == string.Empty)
                                              select row).ToList();

                        if (requiredValidation.Count() > 0)
                        {
                            msg.AppendLine("<br/> Invalid Color Name Found : ");
                            msg.Append("(" + requiredValidation.Count() + ") ");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in requiredValidation)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                        }
                        //SizeName Not entered
                        requiredValidation = (from row in stock.AsEnumerable()
                                                .Where(x => x.Field<object>("Select Size") == null && x.Field<string>("Select Size") == string.Empty)
                                              select row).ToList();

                        if (requiredValidation.Count() > 0)
                        {
                            msg.AppendLine("<br/> Size Name Required Validation Failed : ");
                            msg.Append("(" + requiredValidation.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in requiredValidation)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                            return msg;
                        }

                        //Invalid size
                        requiredValidation = (from row in stock.AsEnumerable()
                                               .Where(
                                             x => x.Field<object>("Select Size") != null && x.Field<string>("Select Size") == string.Empty)
                                              select row).ToList();

                        if (requiredValidation.Count() > 0)
                        {
                            msg.AppendLine("<br/> Invalid Size Name Found : ");
                            msg.Append("(" + requiredValidation.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in requiredValidation)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                        }
                        //Dimension Not entered
                        requiredValidation = (from row in stock.AsEnumerable()
                                                .Where(x => x.Field<object>("Select Dimension") == null)
                                              select row).ToList();

                        if (requiredValidation.Count() > 0)
                        {
                            msg.AppendLine("<br/> Dimension Name Required Validation Failed : ");
                            msg.Append("(" + requiredValidation.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in requiredValidation)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                            return msg;
                        }

                        //Invalid Dimension
                        requiredValidation = (from row in stock.AsEnumerable()
                                               .Where(
                                               x => x.Field<object>("Select Dimension") != null && x.Field<string>("Select Dimension") == string.Empty)
                                              select row).ToList();

                        if (requiredValidation.Count() > 0)
                        {
                            msg.AppendLine("<br/> Invalid Dimension Name Found : ");
                            msg.Append("(" + requiredValidation.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in requiredValidation)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                        }

                        //Material Not entered
                        requiredValidation = (from row in stock.AsEnumerable()
                                                .Where(x => x.Field<object>("Select Material") == null)
                                              select row).ToList();

                        if (requiredValidation.Count() > 0)
                        {
                            msg.AppendLine("<br/> Material Name Required Validation Failed : ");
                            msg.Append("(" + requiredValidation.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in requiredValidation)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                            return msg;
                        }

                        //Invalid Material
                        requiredValidation = (from row in stock.AsEnumerable()
                                             .Where(
                                             x => x.Field<object>("Select Material") != null && x.Field<string>("Select Material") == string.Empty)
                                              select row).ToList();

                        if (requiredValidation.Count() > 0)
                        {
                            msg.AppendLine("<br/> Invalid Material Name Found : ");
                            msg.Append("(" + requiredValidation.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in requiredValidation)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                        }
                        //Check for Unique key (ProductName,ColorName, SizeName,DimensionName, MateialName)
                        var duplicateStock = stock.AsEnumerable()
                                               .GroupBy(i => new
                                               {
                                                   ProductName = i.Field<string>("Select Product"),
                                                   ColorName = i.Field<string>("Select Color"),
                                                   SizeName = i.Field<string>("Select Size"),
                                                   DimensionName = i.Field<string>("Select Dimension"),
                                                   MaterialName = i.Field<string>("Select Material"),

                                               }).Where(g => g.Count() > 1)
                                                 .Select(g => new
                                                 {
                                                     g.Key.ProductName,
                                                     g.Key.ColorName,
                                                     g.Key.SizeName,
                                                     g.Key.DimensionName,
                                                     g.Key.MaterialName
                                                 }).ToList();
                        if (duplicateStock.Count > 0)
                        {
                            msg.AppendLine("<br/> Dulicate Stock Variants Found : ");
                            msg.Append("(" + duplicateStock.Count() + ") ");

                            foreach (var item in duplicateStock)
                            {
                                msg.AppendLine("(" + item.ProductName + " , " + item.ColorName + " , " + item.SizeName + " , " + item.DimensionName + " , " + item.MaterialName + ")" + " ; ");
                            }
                        }

                        //Stock Qty & ReorderLevel  not entered       
                        var stockQty = (from row in stock.AsEnumerable()
                                        where (row.Field<object>("Stock Qty") == null)
                                        || (row.Field<object>("Reorder Level") == null)
                                        select row).ToList();


                        if (stockQty.Count() > 0)
                        {
                            msg.AppendLine("<br/> Stock Qty and Rreorder Level Required Validation Failed : ");
                            msg.Append("(" + stockQty.Count() + ") ");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in stockQty)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                            return msg;
                        }

                        //Invalid Stock Qty & ReorderLevel         
                        stockQty = (from row in stock.AsEnumerable()
                                    where (row.Field<object>("Stock Qty") != null && row.Field<double>("Stock Qty") < 0)
                                    || (row.Field<object>("Reorder Level") != null && row.Field<double>("Reorder Level") < 0)
                                    select row).ToList();


                        if (stockQty.Count() > 0)
                        {
                            msg.AppendLine("<br/> Invalid Entry For Stock Qty and Rreorder Level Found : ");
                            msg.Append("(" + stockQty.Count() + ") ");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in stockQty)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                        }


                        //PackSize and PackUnit not entered
                        //var Packs = (from row in stock.AsEnumerable()
                        //             where (row.Field<object>("PackSize") == null)
                        //                || (row.Field<object>("PackSize Unit") == null)
                        //             select row).ToList();

                        var Packs = (from row in stock.AsEnumerable()
                                     where (row.Field<object>("PackSize") == null)
                                     select row).ToList();
                        if (Packs.Count() > 0)
                        {
                            msg.AppendLine("<br/> PackSize Required Validation Faild : ");
                            msg.Append("(" + Packs.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in Packs)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                            return msg;
                        }
                        //Invalid PackSize and PackUnit
                        //Packs = (from row in stock.AsEnumerable()
                        //         where (row.Field<object>("PackSize") != null && row.Field<double>("PackSize") < 0)
                        //            || (row.Field<object>("PackSize Unit") != null && row.Field<string>("PackSize Unit") == string.Empty)
                        //         select row).ToList();
                        Packs = (from row in stock.AsEnumerable()
                                 where (row.Field<object>("PackSize") != null && row.Field<double>("PackSize") < 0)
                                 select row).ToList();

                        if (Packs.Count() > 0)
                        {
                            msg.AppendLine("<br/> Invalid Entry For PackSize Found : ");
                            msg.Append("(" + Packs.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in Packs)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                        }

                        //MRP and SaleRate not entered
                        var Rates = (from row in stock.AsEnumerable()
                                     where row.Field<object>("MRP(Rs)") == null
                                        || row.Field<object>("Sale Rate(Rs)") == null
                                     select row).ToList();


                        if (Rates.Count() > 0)
                        {
                            msg.AppendLine("<br/> MRP and SaleRate Found Required Validation Faild: ");
                            msg.Append("(" + Rates.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in Rates)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                            return msg;
                        }


                        //Invalid MRP and SaleRate
                        Rates = (from row in stock.AsEnumerable()
                                 where (row.Field<object>("MRP(Rs)") != null && row.Field<double>("MRP(Rs)") < 0)
                                    || (row.Field<object>("Sale Rate(Rs)") != null && row.Field<double>("Sale Rate(Rs)") < 0)
                                 select row).ToList();


                        if (Rates.Count() > 0)
                        {
                            msg.AppendLine("<br/> Invalid Entry For MRP and SaleRate Found : ");
                            msg.Append("(" + Rates.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in Rates)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                            return msg;
                        }


                        //Check for MRP > SaleRate
                        Rates = (from row in stock.AsEnumerable()
                                 where (row.Field<double>("MRP(Rs)") < row.Field<double>("Sale Rate(Rs)"))
                                 select row).ToList();


                        if (Rates.Count() > 0)
                        {
                            msg.AppendLine("<br/> Stock with SaleRate greater than MRP Found : ");
                            msg.Append("(" + Rates.Count() + ")");
                            msg.AppendLine("Refer Sr.No. : ");
                            foreach (var item in Rates)
                            {
                                msg.Append(item.Field<double>("Sr#No#") + " ; ");
                            }
                            return msg;
                        }

                    }
                }
                else
                {

                    msg.AppendLine("Please check Sr.No. It cannot be empty. Empty rows are not allowed in sheet.");
                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in validating  stock sheet :" + ex.Message, ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }

            return msg;

        }

        /// <summary>
        /// Check data entered in product sheet whether present in database tables and active
        /// </summary>
        /// <param name="product">Product Table</param>
        /// <param name="shopID">Shop ID</param>
        /// <returns></returns>
        private StringBuilder ValidateProductSheetData(DataTable product, long shopID)
        {
            StringBuilder msg = new StringBuilder();
            try
            {
                EzeeloDBContext db = new EzeeloDBContext();

                //Max lenth for product name allowed is 300
                List<string> productList = new List<string>();
                //Max lenth for product Description allowed is 500
                List<string> productDescriptionList = new List<string>();
                //Max lenth for product Search Keyword allowed is 500
                List<string> producSearchKeywordList = new List<string>();
                foreach (DataRow item in product.Rows)
                {

                    if (item["ProductName"].ToString().Length > 300)
                    {
                        productList.Add(item["ProductName"].ToString());
                    }
                    if (item["Description"].ToString().Length > 500)
                    {
                        productDescriptionList.Add(item["ProductName"].ToString());
                    }
                    if (item["SearchKeyword"].ToString().Length > 300)
                    {
                        producSearchKeywordList.Add(item["ProductName"].ToString());
                    }

                }
                if (productList.Count() > 0)
                {
                    msg.AppendLine("<br/> Max length for product name allowed is 300, there may be spaces added in product Name");
                    msg.Append("(" + productList.Count() + ") ");
                    msg.AppendLine("<br/> Refer Product Name in ProductName : ");
                    foreach (var item in productList)
                    {
                        msg.Append(item + " ; ");
                    }
                    return msg;
                }
                if (productDescriptionList.Count() > 0)
                {
                    msg.AppendLine("<br/> Max length for product Description allowed is 500, there may be spaces added in Description");
                    msg.Append("(" + productDescriptionList.Count() + ") ");
                    msg.AppendLine("<br/> Refer Product Name in Description : ");
                    foreach (var item in productList)
                    {
                        msg.Append(item + " ; ");
                    }
                    return msg;
                }

                if (producSearchKeywordList.Count() > 0)
                {
                    msg.AppendLine("<br/> Max length for product Search Keyword allowed is 500, there may be spaces added in Product Search Keyword");
                    msg.Append("(" + producSearchKeywordList.Count() + ") ");
                    msg.AppendLine("<br/> Refer Product Name in SearchKeyword : ");
                    foreach (var item in productList)
                    {
                        msg.Append(item + " ; ");
                    }
                    return msg;
                }

                //check whether product category included in owner plan category charge
                //string[] seperator = { " - " };
                // var dataValidation = from n1 in product.AsEnumerable() select n1.Field<string>("CategoryName").Split(seperator, StringSplitOptions.RemoveEmptyEntries)[1].ToString();
                //change on 2/22/2016 for Category
                var dataValidation = from n1 in product.AsEnumerable() select n1.Field<string>("CategoryName").Split(new string[] { " - " }, 2, StringSplitOptions.None)[1].ToString();
                var dbOwnerPlanIncludedActiveCategories = (
                    from OP in db.OwnerPlans
                    join P in db.Plans on OP.PlanID equals P.ID
                    join PCC in db.PlanCategoryCharges on P.ID equals PCC.PlanID
                    join C in db.Categories on PCC.CategoryID equals C.ID
                    where P.PlanCode.Substring(0, 4).ToString().Equals("GBMR")
                    && OP.OwnerID == shopID && OP.IsActive == true && PCC.IsActive == true && C.Level == 3 && C.IsActive == true
                    select new
                    {
                        CategoryName = C.Name

                    }).ToList();

                var result = dataValidation.Except(from n2 in dbOwnerPlanIncludedActiveCategories.AsEnumerable() select n2.CategoryName);

                if (result != null && result.Count() > 0)
                {
                    msg.AppendLine("<br/> Categories entered for product sheet does not include in merchant plan, please select categories which are included in selected merchant plan : ");
                    msg.Append("(" + result.Count() + ") ");
                    msg.AppendLine("<br/> Refer Category Name Product Sheet : ");
                    foreach (var item in result)
                    {
                        msg.Append(item + " ; ");
                    }
                    return msg;
                }

                //check whether entered brand is Active 
                dataValidation = from n1 in product.AsEnumerable() select n1.Field<string>("BrandName");

                var dbActiveBrands = (from b in db.Brands
                                      where b.IsActive == true
                                      select new
                                      {
                                          BrandName = b.Name

                                      }).ToList();

                result = dataValidation.Except(from n2 in dbActiveBrands.AsEnumerable() select n2.BrandName);

                if (result != null && result.Count() > 0)
                {
                    msg.AppendLine("<br/> Brand entered for product sheet is not active in master entries for brands, please contact to admin to diplay correct Brand list in Variant sheet(3 rd sheet in uploaded Excel file) and select appropriate Brand : ");
                    msg.Append("(" + result.Count() + ") ");
                    msg.AppendLine("<br/> Refer Brand Name in Product Sheet : ");
                    foreach (var item in result)
                    {
                        msg.Append(item + " ; ");
                    }
                    return msg;
                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in validating  product sheet data :" + ex.Message, ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return msg;
        }
        /// <summary>
        /// Check data entered in Stock sheet whether present in database tables and active
        /// </summary>
        /// <param name="product">Stock Table</param>
        /// <param name="shopID">Shop ID</param>
        /// <returns></returns>
        private StringBuilder ValidateStockSheetData(DataTable stock, long shopID)
        {
            StringBuilder msg = new StringBuilder();
            try
            {
                EzeeloDBContext db = new EzeeloDBContext();

                //Max lenth for product name allowed is 300
                List<string> productList = new List<string>();
                foreach (DataRow item in stock.Rows)
                {
                    if (item["Select Product"].ToString().Length > 300)
                    {
                        productList.Add(item["Select Product"].ToString());
                    }
                }
                if (productList.Count() > 0)
                {
                    msg.AppendLine("<br/> Max lenth for product allowed is 300, there may be spaces added in product Name");
                    msg.Append("(" + productList.Count() + ") ");
                    msg.AppendLine("<br/> Refer Product Name in Product_Stock Sheet : ");
                    foreach (var item in productList)
                    {
                        msg.Append(item + " ; ");
                    }
                    return msg;
                }

                //check whether Selected color is Active 
                var dataValidation = from n1 in stock.AsEnumerable() select n1.Field<string>("Select Color");

                var dbActiveColors = (from c in db.Colors
                                      where c.IsActive == true
                                      select new
                                      {
                                          ColorName = c.Name

                                      }).ToList();

                var result = dataValidation.Except(from n2 in dbActiveColors.AsEnumerable() select n2.ColorName);

                if (result != null && result.Count() > 0)
                {
                    msg.AppendLine("<br/> Colors selected for Stock Sheet is not active in master entries for colors, please contact to admin to diplay correct Color list in Variant sheet(3 rd sheet in uploaded Excel file) and select appropriate Color : ");
                    msg.Append("(" + result.Count() + ") ");
                    msg.AppendLine("<br/> Refer Color Name in Stock Sheet : ");
                    foreach (var item in result)
                    {
                        msg.Append(item + " ; ");
                    }
                    return msg;
                }


                //check whether selected Size is Active 
                dataValidation = from n1 in stock.AsEnumerable() select n1.Field<string>("Select Size");

                var dbActiveSizes = (from c in db.Sizes
                                     where c.IsActive == true
                                     select new
                                     {
                                         SizeName = c.Name

                                     }).ToList();

                result = dataValidation.Except(from n2 in dbActiveSizes.AsEnumerable() select n2.SizeName);

                if (result != null && result.Count() > 0)
                {
                    msg.AppendLine("<br/> Size selected for Stock Sheet is not active in master entries for Size, please contact to admin to display correct Size list in Variant sheet(3 rd sheet in uploaded Excel file) and select appropriate SizeName : ");
                    msg.Append("(" + result.Count() + ") ");
                    msg.AppendLine("<br/> Refer Size Name in Stock Sheet : ");
                    foreach (var item in result)
                    {
                        msg.Append(item + " ; ");
                    }
                    return msg;
                }

                //check whether selected Dimension is Active 
                dataValidation = from n1 in stock.AsEnumerable() select n1.Field<string>("Select Dimension");

                var dbActiveDimensions = (from c in db.Dimensions
                                          where c.IsActive == true
                                          select new
                                          {
                                              DimensionName = c.Name

                                          }).ToList();

                result = dataValidation.Except(from n2 in dbActiveDimensions.AsEnumerable() select n2.DimensionName);

                if (result != null && result.Count() > 0)
                {
                    msg.AppendLine("<br/> Dimension selected for Stock Sheet is not active in master entries for Dimension, please contact to admin to diplay correct Dimension list in Variant sheet(3 rd sheet in uploaded Excel file) and select appropriate Dimension Name : ");
                    msg.Append("(" + result.Count() + ") ");
                    msg.AppendLine("<br/> Refer Dimension Name in Stock Sheet : ");
                    foreach (var item in result)
                    {
                        msg.Append(item + " ; ");
                    }
                    return msg;
                }

                //check whether selected Material is Active 
                dataValidation = from n1 in stock.AsEnumerable() select n1.Field<string>("Select Material");

                var dbActiveMaterial = (from c in db.Materials
                                        where c.IsActive == true
                                        select new
                                        {
                                            MaterialName = c.Name

                                        }).ToList();

                result = dataValidation.Except(from n2 in dbActiveMaterial.AsEnumerable() select n2.MaterialName);

                if (result != null && result.Count() > 0)
                {
                    msg.AppendLine("<br/> Material selected for Stock Sheet is not active in master entries for Material, please contact to admin to diplay correct Material list in Variant sheet(3 rd sheet in uploaded Excel file) and select appropriate Material Name : ");
                    msg.Append("(" + result.Count() + ") ");
                    msg.AppendLine("<br/> Refer Material Name in Stock Sheet : ");
                    foreach (var item in result)
                    {
                        msg.Append(item + " ; ");
                    }
                    return msg;
                }
                //Commented by harshada because we r removing packsize unit
                //check whether selected Unit is Active 
                //dataValidation = from n1 in stock.AsEnumerable() select n1.Field<string>("PackSize Unit");

                //var dbActiveUnits = (from c in db.Units
                //                     where c.IsActive == true
                //                     select new
                //                     {
                //                         UnitName = c.Name

                //                     }).ToList();

                //result = dataValidation.Except(from n2 in dbActiveUnits.AsEnumerable() select n2.UnitName);

                //if (result != null && result.Count() > 0)
                //{
                //    msg.AppendLine("<br/> Pack size Unit selected for Stock Sheet is not active in master entries for Units, please contact to admin to diplay correct Unit list in Variant sheet(3 rd sheet in uploaded Excel file) and select appropriate Unit Name : ");
                //    msg.Append("(" + result.Count() + ") ");
                //    msg.AppendLine("<br/> Refer PackSize Unit in Stock Sheet : ");
                //    foreach (var item in result)
                //    {
                //        msg.Append(item + " ; ");
                //    }
                //    return msg;
                //}
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in validating  stock sheet data :" + ex.Message, ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return msg;
        }

        /// <summary>
        /// Get the history of excel sheets uploaded by merchants
        /// </summary>
        /// <param name="shopID">Shop ID</param>
        /// <param name="bulkType">Product: 1; ProductStock: 2</param>
        /// <returns>List contains History of excel sheets for provided bulk type</returns>
        public List<BulkLog> GetExcelUploadHstory(long shopID, int bulkType)
        {
            EzeeloDBContext db = new EzeeloDBContext();

            List<BulkLog> listBulkLog = new List<BulkLog>();
            if (bulkType == 0)
                listBulkLog = db.BulkLogs.Where(x => x.ShopID == shopID).OrderByDescending(x => x.CreateDate).ToList();
            else
                listBulkLog = db.BulkLogs.Where(x => x.ShopID == shopID && x.BulkType == bulkType).OrderByDescending(x => x.CreateDate).ToList();

            return listBulkLog;
        }
        /// <summary>
        /// Get stock bulkLog transaction details for provided bulkLogID 
        /// </summary>
        /// <param name="bulkLogID"></param>
        /// <returns>Object containing the list of transaction details</returns>
        public BulkStockListViewModel GetStockBulkDetails(int bulkStockLogID)
        {
            object obj = new object();
            EzeeloDBContext db = new EzeeloDBContext();

            BulkStockListViewModel bulkStockList = new BulkStockListViewModel();
            //Bulk Stock Details
            List<ShopStockBulkLog> stockBulkDetail = new List<ShopStockBulkLog>();
            try
            {

                stockBulkDetail = db.ShopStockBulkLogs.Where(x => x.BulkLogID == bulkStockLogID).OrderBy(x => x.ExcelRowID).ToList();

                List<BulkStockViewModel> listStockBulkDetail = new List<BulkStockViewModel>();

                UploadFilesViewModel uploadImageFiles = new UploadFilesViewModel();
                uploadImageFiles.BulkStockLogID = bulkStockLogID;
                bulkStockList.UploadFiles = uploadImageFiles;
                //BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in getting stock bulk Details 1 :" , ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                listStockBulkDetail = (from STBL in db.ShopStockBulkLogs
                                       join TP in db.TempProducts on STBL.TempProductID equals TP.ID
                                       join TSS in db.TempShopStocks on STBL.TempShopStockID equals TSS.ID
                                       join TPV in db.TempProductVarients on TSS.ProductVarientID equals TPV.ID
                                       join S in db.Sizes on TPV.SizeID equals S.ID
                                       join D in db.Dimensions on TPV.DimensionID equals D.ID
                                       join C in db.Colors on TPV.ColorID equals C.ID
                                       join M in db.Materials on TPV.MaterialID equals M.ID
                                       join U in db.Units on TSS.PackUnitID equals U.ID
                                       where STBL.BulkLogID == bulkStockLogID && TPV.ProductID == TP.ID
                                       orderby STBL.ExcelRowID
                                       select new BulkStockViewModel
                                       {
                                           ProductName = TP.Name,
                                           BrandID = TP.BrandID,
                                           ColorName = C.Name,
                                           DimensionName = D.Name,
                                           SizeName = S.Name,
                                           ExcelRowID = STBL.ExcelRowID,
                                           MaterialName = M.Name,
                                           MRP = TSS.MRP,
                                           SaleRate = TSS.RetailerRate,
                                           WholeSaleRate = TSS.WholeSaleRate,
                                           ShopStockID = TSS.ID,
                                           StockQty = TSS.Qty,
                                           ReorderLevel = TSS.ReorderLevel,
                                           StockStatus = TSS.StockStatus,
                                           PackSize = TSS.PackSize,
                                           PackUnit = U.Name,
                                           ProductID = TP.ID,
                                           BulkLogID = bulkStockLogID,
                                           ImageCount = STBL.ImageCount

                                       }).ToList();
                //BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in getting stock bulk Details new :"+listStockBulkDetail.Count(), ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                // BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in getting stock bulk Details 2 :", ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                //stockBulkDetail = db.ShopStockBulkLogs.Where(x => x.BulkLogID == bulkLogID).OrderBy(x => x.ExcelRowID).ToList();
                bulkStockList.listBulkStock = listStockBulkDetail;
                //BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in getting stock bulk Details 3 :", ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in getting stock bulk Details :" + ex.Message, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }

            return bulkStockList;

        }
        /// <summary>
        /// Get product bulkLog transaction details for provided bulkLogID 
        /// </summary>
        /// <param name="bulkLogID"></param>
        /// <returns>Object containing the list of transaction details</returns>
        public List<BulkProductViewModel> GetProductBulkDetails(int bulkLogID)
        {
            object obj = new object();
            EzeeloDBContext db = new EzeeloDBContext();


            //Bulk Product Details
            List<BulkProductViewModel> productBulkDetail = new List<BulkProductViewModel>();
            try
            {
                productBulkDetail = (from bl in db.ProductBulkDetails
                                     join tp in db.TempProducts on bl.TempProductID equals tp.ID
                                     join C in db.Categories on tp.CategoryID equals C.ID
                                     join B in db.Brands on tp.BrandID equals B.ID

                                     where bl.BulkLogID == bulkLogID
                                     select new BulkProductViewModel
                                     {
                                         ExcelRowID = bl.ExcelRowID,
                                         BulkLogID = bl.BulkLogID,
                                         BrandID = tp.BrandID,
                                         BrandName = B.Name,
                                         CategoryID = tp.CategoryID,
                                         CategoryName = C.Name,
                                         ExcelProductName = tp.Name,
                                         WeightInGram = tp.WeightInGram,
                                         LengthInCm = tp.LengthInCm,
                                         BreadthInCm = tp.BreadthInCm,
                                         HeightInCm = tp.HeightInCm,
                                         Description = tp.Description,
                                         SearchKeyword = tp.SearchKeyword,
                                         Result = bl.Result,
                                         ResultDescription = bl.Description,
                                         isDescUploaded = bl.IsDescUpload

                                     }).ToList();


            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in getting product bulk details :" + ex.Message, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            return productBulkDetail.OrderBy(x => x.ExcelRowID).ToList();
        }
        /// <summary>
        /// Upload description files for product in bulk
        /// </summary>
        /// <param name="bulkProductLogID">bulk Product LogID</param>
        /// <param name="files">List of posted files</param>
        /// <param name="failCounter">Failurre counter</param>
        /// <returns></returns>
        public StringBuilder UploadDescriptionFile(long bulkProductLogID, List<HttpPostedFileBase> files, out int failCounter)
        {

            StringBuilder msg = new StringBuilder();
            int successCounter = 0;
            failCounter = 0;
            StringBuilder faildFileNames = new StringBuilder();
            try
            {

                // Handling Attachments - 
                foreach (HttpPostedFileBase item in files)
                {
                    if (item != null)
                    {
                        //required File name : productID.html  e.g : 1.html after uploading Description.html under the folder of productID
                        string[] seperator = { "." };
                        string[] strSplitt = item.FileName.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                        if (strSplitt.Length > 2 && Int32.Parse(strSplitt[0]) == 0)
                        {
                            failCounter++;
                            faildFileNames.AppendLine(item.FileName + "; ");
                            continue;
                        }
                        long productID = Convert.ToInt64(strSplitt[0]);


                        //check prouctID exists aginst provided bulkLogID
                        EzeeloDBContext db = new EzeeloDBContext();

                        var ProductExists = (from PBL in db.ProductBulkDetails
                                             join P in db.TempProducts on PBL.TempProductID equals P.ID
                                             where PBL.BulkLogID == bulkProductLogID
                                             select new
                                             {
                                                 productID = P.ID

                                             }).FirstOrDefault();

                        if (ProductExists == null)
                        {
                            faildFileNames.AppendLine(item.FileName + "; ");
                            failCounter++;

                        }
                        else
                        {

                            //Save  -  Each Attachment (HttpPostedFileBase item) 
                            DescriptionFileUpload descriptionFileUpload = new DescriptionFileUpload(System.Web.HttpContext.Current.Server);
                            descriptionFileUpload.ProductDescriptionFileUploadOnFTPServer(productID, item, ProductUpload.IMAGE_TYPE.NonApproved);
                            successCounter++;
                            UpdateUploadDescFileUploadCount(productID, bulkProductLogID);
                        }
                    }

                }
                msg.AppendLine("<br /> " + successCounter + " file(s) Uploaded.");
                msg.AppendLine("<br /> " + failCounter + " files(s) could not be uploaded");
                if (failCounter > 0)
                {
                    msg.AppendLine("<br /> Following files are failed to upload due to invalid file name. Please <b>download Description Feed Template </b> to know correct File names.");
                    msg.AppendLine("<br /> Upload Failed file names : " + faildFileNames);
                }
            }
            catch (System.Web.HttpException ex)
            {
                failCounter = files.Count() - successCounter;
                msg.AppendLine("<br /> " + successCounter + " file(s) Uploaded.");
                msg.AppendLine("<br /> " + (files.Count() - successCounter) + " files(s) could not be uploaded");
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Uploading Product description files in bulk :" + ex.Message + ex.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

            }
            catch (Exception ex)
            {
                failCounter = files.Count() - successCounter;
                msg.AppendLine(successCounter + " file(s) Uploaded.");
                msg.AppendLine(files.Count() - successCounter + " files(s) could not be uploaded");
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Uploading Product description files in bulk :" + ex.Message + ex.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

            }
            return msg;

        }
        /// <summary>
        /// Update IsDescUpload = true for description file uploaded aginst product
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="bulkProductLogID"></param>
        private void UpdateUploadDescFileUploadCount(long productID, long bulkProductLogID)
        {

            EzeeloDBContext db = new EzeeloDBContext();
            try
            {
                var ProductBulkLog = db.ProductBulkDetails.Where(x => x.BulkLogID == bulkProductLogID && x.TempProductID == productID).FirstOrDefault();
                if (ProductBulkLog == null) return;

                ProductBulkLog.IsDescUpload = true;
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => new { x.ErrorMessage, x.PropertyName });

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }



    }
}
