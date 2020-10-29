using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using CRM.Common;
using System.Web.Configuration;
using System.IO;
using System.Net;
using BusinessLogicLayer;
using System.Data.Entity;

namespace CRM.Controllers
{
    public class CommonController : Controller
    {
        //yashaswi 17-3-2018
        private EzeeloDBContext db = new EzeeloDBContext();
        public void WarehouseStockLog(long WarehouseStockId, int status,long personalDetailID, int Quantity)
        {
            try
            {
                WarehouseStock obj_WarehouseStocks = db.WarehouseStocks.First(p => p.ID == WarehouseStockId);
                WarehouseStockLog obj_WarehouseStockLog = new WarehouseStockLog();
                obj_WarehouseStockLog.CreateBy = personalDetailID;
                obj_WarehouseStockLog.CreateDate = obj_WarehouseStocks.CreateDate;
                obj_WarehouseStockLog.NetworkIp = obj_WarehouseStocks.NetworkIP;
                obj_WarehouseStockLog.Quantity = Quantity;
                obj_WarehouseStockLog.WarehouseStockId = WarehouseStockId;
                obj_WarehouseStockLog.Status = status;
                db.WarehouseStockLog.Add(obj_WarehouseStockLog);
                db.SaveChanges();                
            }
            catch
            {
               
            }
        }

        private long PersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            

            long PersonalDetailID = 0;
            try
            {
                long UserLoginID = 1;

                if (Session["USER_LOGIN_ID"] != null)
                {
                    UserLoginID = Convert.ToInt64(Session["USER_LOGIN_ID"]);
                }

                if (UserLoginID > 0)
                {
                    PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[CommonController][GetPersonalDetailID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }

        // start yashaswi 22/3/2018
        //public bool UploadImage(int status, HttpPostedFileBase file, long ID, HttpServerUtility server, out string FileName, out string Ext)
        //{
        //    try
        //    { 
        //    FileName = "";
        //    string FolderName = "";
        //    if ((int)Constants.Inventory_Image_Type.WASTAGE == status)
        //    {
        //        FolderName = WebConfigurationManager.AppSettings["FOLDER_WASTAGE"];
        //    }
        //    else
        //    {
        //        FolderName = WebConfigurationManager.AppSettings["FOLDER_INVOICE"];
        //    }
        //    string FTP_Path = WebConfigurationManager.AppSettings["INVENTORY_ROOT_IMAGE_FTP"] + WebConfigurationManager.AppSettings["DIRECTORY"] + "/" + FolderName + "/";
        //    string User_name = WebConfigurationManager.AppSettings["USER_NAME"];
        //    string Password = WebConfigurationManager.AppSettings["PASSWORD"];
        //    var notAllowedExtensions = new[] { "" };
        //    var allowedExtensions = new[] { "" };
        //    var path = "";
        //    var fileName_ = Path.GetFileName(file.FileName);
        //    Ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  

        //    string name = Path.GetFileNameWithoutExtension(fileName_); //getting file name without extension  
        //    string myfile = name + "_" + ID + Ext; //appending the name with InvoiceID  
        //    path = Path.Combine(server.MapPath("~/Image"), myfile);
        //    file.SaveAs(path);
        //    string source;
        //    source = server.MapPath("~/Image");
        //    FileInfo[] lFileInfo = null;

        //    DirectoryInfo lDrInfo = new DirectoryInfo(server.MapPath("~/Image"));
        //    if (lDrInfo.Exists)
        //    {
        //        lFileInfo = lDrInfo.GetFiles();
        //    }

        //    foreach (FileInfo fl in lFileInfo)
        //    {
        //        FtpWebRequest req = (FtpWebRequest)WebRequest.Create(FTP_Path + fl.Name);
        //        req.UseBinary = true;
        //        req.KeepAlive = true;
        //        req.Method = WebRequestMethods.Ftp.UploadFile;
        //        req.Credentials = new NetworkCredential(User_name, Password);
        //        FileStream fs = System.IO.File.OpenRead(fl.DirectoryName + "\\" + fl.Name);
        //        byte[] buffer = new byte[fs.Length];
        //        fs.Read(buffer, 0, buffer.Length);
        //        fs.Close();

        //        Stream ftpstream = req.GetRequestStream();
        //        ftpstream.Write(buffer, 0, buffer.Length);
        //        ftpstream.Close();

        //        string filePath = server.MapPath("~/Image/" + fl.ToString());
        //        if (System.IO.File.Exists(filePath))
        //        {
        //            System.IO.File.Delete(filePath);
        //        }
        //        FileName = fl.Name; 
        //    }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;// new BusinessLogicLayer.MyException("[CommonController][GetPersonalDetailID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
        //    }
        //    return true;

        //}

        public string GetFileName(string FileName)
        {
            string Path = WebConfigurationManager.AppSettings["INVENTORY_ROOT_IMAGE_HTTP"] + WebConfigurationManager.AppSettings["DIRECTORY"] + "/" + WebConfigurationManager.AppSettings["FOLDER_INVOICE"] + "/";
            return Path + FileName;
        }
        //Yashaswi 30/5/2018
        public string GetFileNameWastage(string FileName)
        {
            string Path = WebConfigurationManager.AppSettings["INVENTORY_ROOT_IMAGE_HTTP"] + WebConfigurationManager.AppSettings["DIRECTORY"] + "/" + WebConfigurationManager.AppSettings["FOLDER_WASTAGE"] + "/";
            return Path + FileName;
        }

        public int GetMaxImageSize()
        {
            // used in Invoice Entry
            // 512000=512 KB
            return 512000;
        }

        public void DownloadFileInvoice(string FileNameToDownload, string loc)
        {
            string User_name = WebConfigurationManager.AppSettings["USER_NAME"];
            string Password = WebConfigurationManager.AppSettings["PASSWORD"];
            string Path = WebConfigurationManager.AppSettings["INVENTORY_ROOT_IMAGE_FTP"] + WebConfigurationManager.AppSettings["DIRECTORY"] + "/" + WebConfigurationManager.AppSettings["FOLDER_INVOICE"] + "/";
            DownloadFile(Path, FileNameToDownload, User_name, Password, loc);
        }

        public string DownloadFile(string FtpUrl, string FileNameToDownload,
                          string userName, string password, string tempDirPath)
        {
            string ResponseDescription = "";
            string PureFileName = new FileInfo(FileNameToDownload).Name;
            string DownloadedFilePath = tempDirPath + "/" + PureFileName;
            string downloadUrl = String.Format("{0}/{1}", FtpUrl, FileNameToDownload);
            FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(downloadUrl);
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            req.Credentials = new NetworkCredential(userName, password);
            req.UseBinary = true;
            req.Proxy = null;
            try
            {
                FtpWebResponse response = (FtpWebResponse)req.GetResponse();
                Stream stream = response.GetResponseStream();
                byte[] buffer = new byte[2048];
                FileStream fs = new FileStream(DownloadedFilePath, FileMode.Create);
                int ReadCount = stream.Read(buffer, 0, buffer.Length);
                while (ReadCount > 0)
                {
                    fs.Write(buffer, 0, ReadCount);
                    ReadCount = stream.Read(buffer, 0, buffer.Length);
                }
                ResponseDescription = response.StatusDescription;
                fs.Close();
                stream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ResponseDescription;
        }

        // End yashaswi 22/3/2018

        #region Custome Message Box Yashaswi 31/3/2018

        [HttpPost]
        public ActionResult Make_SuccessSessionNull()
        {
            Session["Success"] = null;
            return Json(new { status = "Success", message = "Success" });
        }

        [HttpPost]
        public ActionResult Make_WarningSessionNull()
        {
            Session["Warning"] = null;
            return Json(new { status = "Success", message = "Success" });
        }

        [HttpPost]
        public ActionResult Make_InfoSessionNull()
        {
            Session["Info"] = null;
            return Json(new { status = "Success", message = "Success" });
        }

        [HttpPost]
        public ActionResult Make_ErrorSessionNull()
        {
            Session["Error"] = null;
            return Json(new { status = "Success", message = "Success" });
        }

        #endregion

        //Yashaswi 4/4/2018
        public void ShopStockDeduction(long WarehouseStockId, int Qty, out long ShopStock_Qty, out long ShopStockID)
        {
            ShopStock_Qty = -1;
            ShopStockID = 0;
            ShopStock obj_ShopStock = db.ShopStocks.SingleOrDefault(p => p.WarehouseStockID == WarehouseStockId);
            if (obj_ShopStock != null)
            {
                obj_ShopStock.Qty = obj_ShopStock.Qty - Qty;
                if (obj_ShopStock.Qty < 0)
                {
                    obj_ShopStock.Qty = 0;
                }
                db.Entry(obj_ShopStock).State = EntityState.Modified;
                db.SaveChanges();
                ShopStock_Qty = obj_ShopStock.Qty;
                ShopStockID = obj_ShopStock.ID;
            }
        }

        public void AddBatchToShopStock(long ShopStock_Qty, long WarehouseStockID, long ShopStockID)
        {
            if (ShopStock_Qty == 0 && WarehouseStockID > 0)
            {
                BusinessLogicLayer.CustomerOrder obj = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                obj.SetNewBatchToShopStock(ShopStockID, WarehouseStockID);
            }
        }

        //Yashaswi 9/4/2018
        public List<Supplier> GetSupplierLIst(long WarehouseId)
        {
            try
            {
                long? DVId = 0;
                long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
                List<Supplier> ListSupplier = new List<Supplier>();
                ListSupplier = db.Suppliers.Where(s => s.IsActive == true && s.ContactPerson != "").ToList();
                if (WarehouseId > 0)
                {
                    Warehouse obj_Warehouse = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseId);
                    if (WarehouseId == EzeeloWarehouseId || obj_Warehouse.Entity.Trim() == "HO" || obj_Warehouse.Entity.Trim() == "EVW") //Yashaswi 4-12-2018 For EVW two new condition
                    {
                        ListSupplier = ListSupplier.Where(s => s.WarehouseID == null && s.IsActive == true).ToList();
                    }
                    else
                    {
                        if (obj_Warehouse.IsFulfillmentCenter == true)
                        {
                            DVId = obj_Warehouse.DistributorId;
                            ListSupplier = ListSupplier.Where(s => s.WarehouseID == DVId).ToList();
                            //Remove outside supplier suggested by Reena mam 18/4/2018
                            //ListSupplier = ListSupplier.Where(s => s.WarehouseID == null || s.WarehouseID == DVId && s.IsActive == true).ToList();
                        }
                        else
                        {
                            DVId = obj_Warehouse.ID;
                            //Remove outside supplier suggested by Reena mam 18/4/2018
                            //ListSupplier = ListSupplier.Where(s => s.WarehouseID == EzeeloWarehouseId).ToList();//Yashaswi 4-12-2018 For EVW
                            //ListSupplier = ListSupplier.Where(s => s.WarehouseID == null || s.WarehouseID == EzeeloWarehouseId && s.IsActive == true).ToList();

                            //Yashaswi 4-12-2018 For EVW
                            //ListSupplier = ListSupplier.Where(s => (db.Warehouses.Where(w => w.Entity == "HO" || w.Entity == "EVW").Select(w => w.ID).ToList()).Contains((long)s.WarehouseID)).ToList();

                            long StateId = 0;
                            //Get Login DV's State
                            var StateResult = from p in db.Pincodes
                                              join C in db.Cities on p.CityID equals C.ID
                                              join D in db.Districts on C.DistrictID equals D.ID
                                              join S in db.States on D.StateID equals S.ID
                                              where p.ID == obj_Warehouse.PincodeID
                                              select new
                                              {
                                                  S.ID
                                              };

                            if (StateResult != null && StateResult.Count() != 0)
                            {
                                StateId = StateResult.FirstOrDefault().ID;
                                //Filter supplier list by State
                                ListSupplier = (from ls in ListSupplier
                                                join P in db.Pincodes on ls.PincodeID equals P.ID
                                                join C in db.Cities on P.CityID equals C.ID
                                                join D in db.Districts on C.DistrictID equals D.ID
                                                join S in db.States on D.StateID equals S.ID
                                                where S.ID == StateId
                                                select new Supplier
                                                {
                                                    WarehouseID = ls.WarehouseID,
                                                    Name = ls.Name,
                                                    ID = ls.ID
                                                }).ToList();
                            }

                            ListSupplier = ListSupplier.Where(s => s.WarehouseID != null)
                                .Join(db.Warehouses.Where(w => w.Entity == "EVW"), s => s.WarehouseID, w => w.ID, (s, w) => new Supplier
                                {
                                    Name = s.Name,
                                    ID = s.ID
                                }).ToList();

                            //To add HO supplier
                            List<Supplier> ListSupplier1 = db.Suppliers.ToList();
                            ListSupplier.AddRange(ListSupplier1.Where(s => s.WarehouseID != null)
                               .Join(db.Warehouses.Where(w => w.Entity == "HO"), s => s.WarehouseID, w => w.ID, (s, w) => new Supplier
                               {
                                   Name = s.Name,
                                   ID = s.ID
                               }).ToList());
                        }
                    }
                }

                return ListSupplier;
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return null;
            }

        }
        public List<Warehouse> GetAllSupplierWarehouseList()
        {
            try
            {
                List<Warehouse> List_SupplierWarehouse = db.Warehouses.Where(w =>
                    !(db.Suppliers.Where(s => s.WarehouseID != null).Select(s => s.WarehouseID))
                    .Contains(w.ID) && w.IsActive == true && w.IsFulfillmentCenter == false).ToList();
                return List_SupplierWarehouse;
            }
            catch
            {
                return null;
            }

        }
        public List<Warehouse> GetFVList(long WarehouseId)
        {
            try
            {
                List<Warehouse> List_warehouse = db.Warehouses.Where(p => p.IsActive == true && p.DistributorId == WarehouseId).ToList();
                return List_warehouse;
            }
            catch
            {
                return null;
            }

        }

       
	}
}