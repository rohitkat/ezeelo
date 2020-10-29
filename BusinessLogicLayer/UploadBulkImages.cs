using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.Hosting;
using System.Net;
/*
 Handed over to Harshada
 */
namespace BusinessLogicLayer
{
    public class UploadBulkImages : ProductDisplay
    {
        public static string TEMPLATE_ROOTPATH = "BulkUploadTemplate";
        public UploadBulkImages(System.Web.HttpServerUtility server) : base(server) { }
        /// <summary>
        /// Upload product Images in Bulk
        /// </summary>
        /// <param name="bulkStockLogID">Bulk Log ID for Stock</param>
        /// <param name="shopID">Shop ID</param>
        /// <param name="fileUpload">List of Uploaded Files</param>
        /// <param name="failCounter">Failed to Upload Counter</param>
        /// <returns></returns>
        public StringBuilder UploadProductImages(int bulkStockLogID, long shopID, List<HttpPostedFileBase> fileUpload, out int failCounter)
        {
            StringBuilder msg = new StringBuilder();
            int successCounter = 0;
            failCounter = 0;
            StringBuilder faildFileNames = new StringBuilder();
            try
            {

                // Handling Attachments - 
                foreach (HttpPostedFileBase item in fileUpload)
                {
                    if (item != null)
                    {
                        //required File name : productID_ColorName_1.jpg  after uploading: gb_10001_001_ll.jpg

                        string[] seperator = { "_", "." };
                        string[] strSplitt = item.FileName.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                        if (strSplitt.Length > 3 && Int32.Parse(strSplitt[0]) == 0)
                        {
                            failCounter++;
                            faildFileNames.AppendLine(item.FileName + "; ");
                            continue;
                        }
                        long productID = Convert.ToInt64(strSplitt[0]);
                        string colorName = strSplitt[1];


                        //check prouctID exists aginst provided bulkLogID
                        EzeeloDBContext db = new EzeeloDBContext();
                        string CheckColor = string.Empty;

                        if (colorName == "Default")
                        {
                            CheckColor = "N/A";

                        }
                        else
                            CheckColor = colorName;


                        var ProductExists = (from SSBL in db.ShopStockBulkLogs
                                             join ss in db.TempShopStocks on SSBL.TempShopStockID equals ss.ID
                                             join pv in db.TempProductVarients on ss.ProductVarientID equals pv.ID
                                             join c in db.Colors on pv.ColorID equals c.ID
                                             where SSBL.BulkLogID == bulkStockLogID
                                             && pv.ProductID == productID
                                             && c.Name.Trim().ToLower().Equals(CheckColor)
                                             select new
                                             {
                                                 productID = pv.ProductID,
                                                 ColorName = c.Name

                                             }).FirstOrDefault();

                        if (ProductExists == null)
                        {
                            faildFileNames.AppendLine(item.FileName + "; ");
                            failCounter++;

                        }
                        else
                        {
                            List<HttpPostedFileBase> imgList = new List<HttpPostedFileBase>();
                            imgList.Add(item);
                            //Save  -  Each Attachment (HttpPostedFileBase item) 
                            if (Convert.ToInt32(strSplitt[2]) == 1)
                            {
                                CommonFunctions.UploadProductImages(imgList, string.Empty, productID, colorName, string.Empty, ProductUpload.IMAGE_TYPE.NonApproved, 0);
                            }
                            else
                            {
                                CommonFunctions.UploadProductImages(imgList, string.Empty, productID, colorName, string.Empty, ProductUpload.IMAGE_TYPE.NonApproved, -1);
                            }
                            successCounter++;
                            UpdateImageCount(productID, shopID, colorName, 1);
                        }
                    }

                }
                msg.AppendLine("<br /> " + successCounter + " file(s) Uploaded.");
                msg.AppendLine("<br /> " + failCounter + " files(s) could not be uploaded");
                if (failCounter > 0)
                {
                    msg.AppendLine("<br /> Following files are failed to upload due to invalid file name. Please <b>download Image Feed Template </b> to know correct image names.");
                    msg.AppendLine("<br /> Upload Failed file names : " + faildFileNames);
                }
            }
            catch (System.Web.HttpException ex)
            {
                msg.AppendLine(successCounter + " file(s) Uploaded.");
                msg.AppendLine(fileUpload.Count() - successCounter + " files(s) could not be uploaded");
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Uploading Product Images in bulk :" + ex.Message + ex.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

            }
            catch (Exception ex)
            {
                msg.AppendLine(successCounter + " file(s) Uploaded.");
                msg.AppendLine(fileUpload.Count() - successCounter + " files(s) could not be uploaded");
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Uploading Product Images in bulk :" + ex.Message + ex.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

            }
            return msg;

        }
        /// <summary>
        /// Upload images for a perticular stock only
        /// </summary>
        /// <param name="productID">Product ID</param>
        /// <param name="shopID">Shop ID</param>
        /// <param name="color">Color Name</param>
        /// <param name="fileUpload">List of Uploaded Files</param>
        /// <param name="failCounter">Failed Counter</param>
        /// <returns></returns>
        public StringBuilder UploadStockImages(long productID, long shopID, string color, List<HttpPostedFileBase> fileUpload, out int failCounter)
        {
            StringBuilder msg = new StringBuilder();
            int successCounter = 0;
            failCounter = 0;
            StringBuilder faildFileNames = new StringBuilder();
            try
            {
                int i = 0;
                // Handling Attachments - 
                foreach (HttpPostedFileBase item in fileUpload)
                {
                    if (item != null)
                    {
                        //like normal upload

                        List<HttpPostedFileBase> imgList = new List<HttpPostedFileBase>();
                        imgList.Add(item);
                        //Save  -  Each Attachment (HttpPostedFileBase item) 
                        //if (i == 0)
                        //{
                            CommonFunctions.UploadProductImages(imgList, string.Empty, productID, color.ToLower().Trim().Equals("n/a") ? "Default" : color.Trim(), string.Empty, ProductUpload.IMAGE_TYPE.NonApproved, i);
                        //}
                        successCounter++;
                        i = -1;
                    }
                    msg.AppendLine(successCounter + " file(s) Uploaded.");
                    msg.AppendLine(failCounter + " files(s) could not be uploaded");
                    if (failCounter > 0)
                    {
                        msg.AppendLine("Failed file names : " + faildFileNames);
                    }

                }
                UpdateImageCount(productID, shopID, color, successCounter);
            }
            catch (System.Web.HttpException ex)
            {
                failCounter = fileUpload.Count();
                msg.AppendLine(successCounter + " file(s) Uploaded.");
                msg.AppendLine(fileUpload.Count() - successCounter + " files(s) could not be uploaded");
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Uploading Product Images in bulk :" + ex.Message + ex.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

            }
            catch (Exception ex)
            {
                failCounter = fileUpload.Count();
                msg.AppendLine(successCounter + " file(s) Uploaded.");
                msg.AppendLine(fileUpload.Count() - successCounter + " files(s) could not be uploaded");
                BusinessLogicLayer.ErrorLog.ErrorLogFile("Problem in Uploading Product Images in bulk :" + ex.Message + ex.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);

            }
            return msg;

        }
        /// <summary>
        /// Update the imagecount field from table ShopStockBulkLogs for given product ID and colorName
        /// </summary>
        /// <param name="productID">Product ID</param>
        /// <param name="ShopID">Shop ID</param>
        /// <param name="colorName">Color Name</param>
        /// <param name="imageCount">Image Count</param>
        private void UpdateImageCount(long productID, long ShopID, string colorName, int imageCount)
        {
            EzeeloDBContext db = new EzeeloDBContext();
            string CheckColor = string.Empty;
            if (colorName == "Default")
            {
                CheckColor = "N/A";

            }
            else
                CheckColor = colorName;

            var color = db.Colors.Where(x => x.Name == CheckColor.Trim()).FirstOrDefault();

            if (color == null) return;

            //get Row for Product and color from ShopStockBulkLog table
            var shopStockLog = (from ssbl in db.ShopStockBulkLogs
                                join ss in db.TempShopStocks on ssbl.TempShopStockID equals ss.ID
                                join pv in db.TempProductVarients on ss.ProductVarientID equals pv.ID
                                join sp in db.TempShopProducts on ss.ShopProductID equals sp.ID
                                where sp.ShopID == ShopID && pv.ColorID == color.ID && pv.ProductID == productID
                                select ssbl).ToList();

            //update Image Count
            shopStockLog.ForEach(ssbl => ssbl.ImageCount += imageCount);
            db.SaveChanges();


        }
        /// <summary>
        /// Generate html table string for Image Template to be displayed in Pdf
        /// </summary>
        /// <param name="bulkLogID">Bulk Log ID</param>
        /// <param name="dtSavedProducts">Datatable for distinct colors against product</param>
        /// <returns></returns>
        public StringBuilder GenerateImageTemplate(int bulkLogID, DataTable dtSavedProducts)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("Product Image Feed Template");
            str.AppendLine("");

            str.Append("<table>");
            str.Append("<tr>");
            str.Append("<th>Sr.No</th>");
            str.Append("<th>ProductName</th>");
            str.Append("<th>ColorName</th>");
            str.Append("<th>Image_1</th>");
            str.Append("<th>Image_n</th>");
            str.Append("</tr>");
            int i = 0;
            foreach (DataRow item in dtSavedProducts.Rows)
            {
                str.Append("<tr>");
                str.Append("<td>" + ++i + "</td>");
                str.Append("<td>" + item["ProductName"].ToString() + "</td>");

                str.Append("<td>" + item["ColorName"].ToString() + "</td>");
                if (item["ColorName"].ToString() == "N/A")
                {
                    str.Append("<td>" + item["TempProductID"].ToString() + "_Default_" + 1 + "</td>");
                    str.Append("<td>" + item["TempProductID"].ToString() + "_Default_n</td>");
                }
                else
                {
                    str.Append("<td>" + item["TempProductID"].ToString() + "_" + item["ColorName"].ToString() + "_" + 1 + "</td>");
                    str.Append("<td>" + item["TempProductID"].ToString() + "_" + item["ColorName"].ToString() + "_n</td>");
                }
                str.Append("</tr>");
            }
            str.Append("</table>");


            return str;

        }
        /// <summary>
        /// Generate html table string for saved distinct products
        /// </summary>
        /// <param name="bulkLogID">bulk log id </param>
        /// <param name="dtSavedProducts">Datatable for products</param>
        /// <returns></returns>
        public StringBuilder GenerateDescriptionTemplate(int bulkLogID, DataTable dtSavedProducts)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("Product Description Feed Template");
            str.AppendLine("");

            str.Append("<table>");
            str.Append("<tr>");
            str.Append("<th>Sr.No</th>");
            str.Append("<th>ProductName</th>");
            str.Append("<th>Description File Name</th>");
            str.Append("</tr>");
            int count = 0;

            //var SavedUniqueProuctList = dtSavedProducts.AsEnumerable()
            //                             .GroupBy(i => new
            //                             {
            //                                 ProductName = i.Field<string>("ProductName"),  
            //                                 ProductID = i.Field<Int64>("TempProductID")   

            //                             }).Where(g => g.Count() >0)
            //                               .Select(g => new
            //                               {
            //                                   g.Key.ProductName,
            //                                   g.Key.ProductID
            //                               }).ToList();


            //A Query change by harshada to get unique product IDs to upload description file
            var SavedUniqueProuctList = (from table in dtSavedProducts.AsEnumerable()
                                         group table by new
                                         {
                                             ProductID = table["TempProductID"],
                                             ProductName = table["ProductName"]
                                         } into groupby
                                         select new
                                         {
                                             groupby.Key.ProductID,
                                             groupby.Key.ProductName

                                         }).ToList();

            foreach (var item in SavedUniqueProuctList)
            {
                str.Append("<tr>");
                str.Append("<td>" + ++count + "</td>");
                str.Append("<td>" + item.ProductName + "</td>");
                str.Append("<td>" + item.ProductID + ".html</td>");
                str.Append("</tr>");
            }
            str.Append("</table>");


            return str;

        }

        /// <summary>
        /// This method is used for saving pdf file on provided path using iTestshap.dll
        /// </summary>
        /// <param name="html">generated html table string</param>
        /// <param name="fileName">file name e.g. pdf-BulkLog-2-ImageTemplatepdf for image template & pdf-BulkLog-55-DescriptionTemplate for product discription template.</param>
        public virtual void printpdf(StringBuilder html, string fileName)
        {
            String htmlText = html.ToString();
            Document document = new Document();
            ImageUpload img = new ImageUpload(System.Web.HttpContext.Current.Server);
            string filePath = HostingEnvironment.MapPath("~/Content/");
            PdfWriter.GetInstance(document, new FileStream(filePath + "\\pdf-" + fileName + ".pdf", FileMode.Create));

            document.Open();
            iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);
            hw.Parse(new StringReader(htmlText));
            document.Close();
            //Changes by Harshada to save image and decription template on server
            string source = Path.Combine(filePath, "pdf-" + fileName + ".pdf");
            string target = "/" + TEMPLATE_ROOTPATH + "/pdf-" + fileName + ".pdf";
            this.CopyTemplateToFTP(source, target);
            FileInfo lDrInfo = new FileInfo(source);
            lDrInfo.Delete();
        }

        /// <summary>
        /// Use to Upload image and description templates on server
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void CopyTemplateToFTP(string source, string target)
        {
            
            try
            {
                //FileInfo[] lFileInfo = null;
                BusinessLogicLayer.ReadConfig imgPath = new ReadConfig(System.Web.HttpContext.Current.Server);
                
                   
                        FtpWebRequest req = (FtpWebRequest)WebRequest.Create(imgPath.IMAGE_FTP + target);
                        req.UseBinary = true;
                        req.KeepAlive = true;
                        req.Method = WebRequestMethods.Ftp.UploadFile;
                        req.Credentials = new NetworkCredential(imgPath.USER_NAME.Trim(), imgPath.PASSWORD.Trim());
                        FileStream fs = File.OpenRead(source);
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        fs.Close();

                        Stream ftpstream = req.GetRequestStream();
                        ftpstream.Write(buffer, 0, buffer.Length);
                        ftpstream.Close();
               // }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[CopyTemplateToFTP]", "Probem in Saving image to Ftp server!!" + Environment.NewLine + ex.Message);
            }
        }
        
    }
    /// <summary>
    /// This class is used for footer designing purpose, but we have not used it for bulk upload.
    /// </summary>
    public partial class Footer : PdfPageEventHelper
    {
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            Paragraph footer = new Paragraph("Print Date " + System.DateTime.UtcNow.AddHours(5).AddMinutes(30), FontFactory.GetFont(FontFactory.TIMES, 10, iTextSharp.text.Font.NORMAL));

            footer.Alignment = Element.ALIGN_LEFT;

            PdfPTable footerTbl = new PdfPTable(1);

            footerTbl.TotalWidth = document.PageSize.Width;

            footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cell = new PdfPCell(footer);

            cell.Border = 1;

            cell.PaddingLeft = 10;

            footerTbl.AddCell(cell);

            footerTbl.WriteSelectedRows(0, -1, 0, 30, writer.DirectContent);
        }
    }
}
