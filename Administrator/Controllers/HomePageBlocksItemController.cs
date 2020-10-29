using Administrator.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Administrator.Controllers
{
    public class HomePageBlocksItemController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /FranchiseMenu/
        StringBuilder errStr = new StringBuilder("=====================================================================================" +
         Environment.NewLine
         + "ErrorLog Controller : HomePageBlocksItemController" + Environment.NewLine);

        #region Index
        //
        // GET: /HomePageBlocksItem/
        [SessionExpire]
        [CustomAuthorize(Roles = "HomePageBlocksItem/CanRead")]
        public ActionResult Index(int? FranchiseList, long? BlockTypeID, string SDate, string EDate)
        {
            List<BlockTypeViewModel> BlockTypeList = new List<BlockTypeViewModel>();
            try
            {
                long btId = 0;
                long.TryParse(Convert.ToString(BlockTypeID), out btId);
                ViewBag.BlockTypeID = btId;                
                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", FranchiseList);
                
                if ((!string.IsNullOrEmpty(SDate) || !string.IsNullOrEmpty(EDate)) && FranchiseList != null)
                {
                    DateTime lSDate = CommonFunctions.GetProperDate(SDate);
                    DateTime lEDate = CommonFunctions.GetProperDate(EDate);                    
                    ViewBag.SDate = SDate;
                    ViewBag.EDate = EDate;

                    BlockTypeList = (from dbt in db.DesignBlockTypes
                                     where dbt.IsActive == true
                                     select new BlockTypeViewModel { ID = dbt.ID, Name = dbt.Name, ItemCount = db.BlockItemsLists.Where(x => x.DesignBlockTypeID == dbt.ID && x.FranchiseID == FranchiseList && x.StartDate >= lSDate && x.EndDate <= lEDate && x.DesignBlockType.Name.ToLower().Trim() != "product gallery").Select(x => x.ID).Count() }).ToList();
                    
                }
                else if (FranchiseList != null)
                {
                    BlockTypeList = (from dbt in db.DesignBlockTypes
                                     where dbt.IsActive == true
                                     select new BlockTypeViewModel { ID = dbt.ID, Name = dbt.Name, ItemCount = db.BlockItemsLists.Where(x => x.DesignBlockTypeID == dbt.ID && x.FranchiseID == FranchiseList && x.DesignBlockType.Name.ToLower().Trim() != "product gallery").Select(x => x.ID).Count() }).ToList();

                }
                return View(BlockTypeList.OrderBy(x=>x.Name));
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Index[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Index view!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "HomePageBlocksItem/CanRead")]
        public PartialViewResult ItemListing(Int32 FranchiseID, Int64 BlockTypeID, string SDate, string EDate)
        {
            List<BlockItemsList> blockItemsList = new List<BlockItemsList>();
            try
            {
                ViewBag.BlockTypeID = BlockTypeID;
                ViewBag.FranchiseID = FranchiseID;

                blockItemsList = db.BlockItemsLists.Where(x => x.DesignBlockTypeID == BlockTypeID && x.FranchiseID == FranchiseID).ToList();
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                foreach (BlockItemsList item in blockItemsList)
                {
                    item.ImageName = rcKey.HOME_IMAGE_HTTP + item.ImageName;
                }
                if (!string.IsNullOrEmpty(SDate) || !string.IsNullOrEmpty(EDate))
                {
                    DateTime lSDate = CommonFunctions.GetProperDate(SDate);
                    DateTime lEDate = CommonFunctions.GetProperDate(EDate);
                    blockItemsList = blockItemsList.Where(x=>x.StartDate >= lSDate && x.EndDate <= lEDate).ToList();
                }

                return PartialView("_ItemListing", blockItemsList.OrderBy(x => x.SequenceOrder));
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Index[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Index view!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return PartialView("_ItemListing", blockItemsList);
            }
        }

        #endregion

        #region Set Sequence

        [SessionExpire]
        [CustomAuthorize(Roles = "HomePageBlocksItem/CanRead")]
        public ActionResult SetSequence(int? FranchiseList, long? BlockTypeList)
        {
            try
            {
                ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", FranchiseList);
                ViewBag.BlockTypeList = new SelectList(db.DesignBlockTypes.Where(x => x.IsActive == true && x.Name.ToLower().Trim() != "product gallery").ToList().OrderBy(x => x.Name), "ID", "Name", BlockTypeList);

                return View();
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Index[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Index view!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Index view!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        #endregion

        #region Details

        //
        // GET: /HomePageBlocksItem/Details/5
        [SessionExpire]
        [CustomAuthorize(Roles = "HomePageBlocksItem/CanRead")]
        public ActionResult Details(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BlockItemsList item = db.BlockItemsLists.Find(id);
                if (item == null)
                {
                    return HttpNotFound();
                }

                BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);
                item.ImageName = rcKey.HOME_IMAGE_HTTP + item.ImageName;
                //item.StartDate=item.StartDate.
                ViewBag.FranchiseName = db.Franchises.Where(x => x.ID == item.FranchiseID).FirstOrDefault().BusinessDetail.Name;
                ViewBag.BlockTypeName = item.DesignBlockType.Name;
                ViewBag.ImageWidth = item.DesignBlockType.ImageWidth + "px";
                ViewBag.ImageHeight = item.DesignBlockType.ImageHeight + "px";

                return View(item);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Details[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Detail!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Detail!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }
        #endregion

        #region Create
        //
        // GET: /HomePageBlocksItem/Create
        [SessionExpire]
        [CustomAuthorize(Roles = "HomePageBlocksItem/CanWrite")]
        public ActionResult Create()
        {
            ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");
            ViewBag.BlockTypeID = new SelectList(db.DesignBlockTypes.Where(x => x.IsActive == true && x.Name.ToLower().Trim() != "product gallery").ToList().OrderBy(x => x.Name), "ID", "Name");

            return View();
        }

        //
        // POST: /HomePageBlocksItem/Create
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "HomePageBlocksItem/CanWrite")]
        public ActionResult Create(BlockItemsList blockItems, Int32 FranchiseID, Int64 BlockTypeID, HttpPostedFileBase file, string SDate, string EDate)
        {
            try
            {
                // TODO: Add insert logic here
                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name");
                ViewBag.BlockTypeID = new SelectList(db.DesignBlockTypes.Where(x => x.IsActive == true && x.Name.ToLower().Trim() != "product gallery").ToList().OrderBy(x => x.Name), "ID", "Name");
                DateTime lSDate = CommonFunctions.GetProperDate(SDate);
                DateTime lEDate = CommonFunctions.GetProperDate(EDate);
                ViewBag.SDate = lSDate.ToString("dd/MM/yyyy");
                ViewBag.EDate = lEDate.ToString("dd/MM/yyyy");

                blockItems.DesignBlockTypeID = BlockTypeID;
                blockItems.FranchiseID = FranchiseID;
                blockItems.StartDate = lSDate;
                blockItems.EndDate = lEDate;

                Int64 CityID = db.Franchises.Where(x => x.ID == FranchiseID).FirstOrDefault().BusinessDetail.Pincode.CityID;
                int imgNo = db.BlockItemsLists.Where(x => x.DesignBlockTypeID == BlockTypeID).Count() + 1;
               // blockItems.ImageName = "/" + CityID.ToString().Trim() + "/" + blockItems.DesignBlockTypeID.ToString().Trim() + "/" + imgNo.ToString().Trim() + Path.GetExtension(file.FileName);////hide
                blockItems.ImageName = "/" + CityID.ToString().Trim() + "/" + FranchiseID + "/" + blockItems.DesignBlockTypeID.ToString().Trim() + "/" + imgNo.ToString().Trim() + Path.GetExtension(file.FileName);////added
                string msg;
                if (this.IsValidated(out msg, blockItems))
                {
                    List<object> paramValues = new List<object>();
                    paramValues.Add(DBNull.Value);
                    paramValues.Add(blockItems.FranchiseID);
                    paramValues.Add(blockItems.DesignBlockTypeID);
                    paramValues.Add(blockItems.StartDate);
                    paramValues.Add(blockItems.EndDate);
                    paramValues.Add(blockItems.SequenceOrder);
                    paramValues.Add(blockItems.ImageName);
                    paramValues.Add(blockItems.LinkUrl);
                    paramValues.Add(blockItems.Tooltip);
                    paramValues.Add(blockItems.IsActive);
                    paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                    paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                    paramValues.Add(DBNull.Value);
                    paramValues.Add(DBNull.Value);
                    paramValues.Add("Net Browser");
                    paramValues.Add("x");
                    paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                    paramValues.Add(DBNull.Value);
                    paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT);
                    paramValues.Add(DBNull.Value);

                    int resultCode;
                    ViewBag.Messaage = BusinessLogicLayer.HomePageBlockItemsList.Insertupdate_BlockItemsList(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.INSERT, out resultCode, System.Web.HttpContext.Current.Server);

                    if (resultCode != 1)
                    {
                        TempData["Messaage"] = ViewBag.Messaage;
                        return View(blockItems);
                    }

                    bool IsUploaded = false;
                    if (file != null && resultCode == 1)
                    {
                        //upload cat image here
                        IsUploaded = CommonFunctions.UploadHomePageImage(file, CityID, FranchiseID, blockItems.DesignBlockTypeID, imgNo + Path.GetExtension(file.FileName));////added FranchiseID
                    }
                    if (!IsUploaded && file != null && resultCode == 1)
                    {
                        ViewBag.Messaage = ViewBag.Messaage + " Problem in Uploading Block Item Image...";
                    }
                }
                else
                {
                    ViewBag.Messaage = msg;
                }
                
                TempData["Messaage"] = ViewBag.Messaage;
                return RedirectToAction("Create");
                // return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Create[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Creation!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                               
            }
            return View(blockItems);
        }

        #endregion

        #region Edit

        //
        // GET: /HomePageBlocksItem/Edit/5
        [SessionExpire]
        [CustomAuthorize(Roles = "HomePageBlocksItem/CanRead")]
        public ActionResult Edit(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BlockItemsList item = db.BlockItemsLists.Find(id);
                if (item == null)
                {
                    return HttpNotFound();
                }

                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", item.FranchiseID);
                ViewBag.BlockTypeID = new SelectList(db.DesignBlockTypes.Where(x => x.IsActive == true && x.Name.ToLower().Trim() != "product gallery").ToList().OrderBy(x => x.Name), "ID", "Name", item.DesignBlockTypeID);
                BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);
                item.ImageName = rcKey.HOME_IMAGE_HTTP + item.ImageName;
                ViewBag.SDate = item.StartDate.ToString("dd/MM/yyyy");
                ViewBag.EDate = item.EndDate.ToString("dd/MM/yyyy");
                return View(item);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Edit[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Update!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Update!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        //
        // POST: /HomePageBlocksItem/Edit/5
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "HomePageBlocksItem/CanWrite")]
        public ActionResult Edit(BlockItemsList blockitem, HttpPostedFileBase file, string SDate, string EDate)
        {
            try
            {
                BlockItemsList lblockitem = db.BlockItemsLists.Find(blockitem.ID);
                ViewBag.FranchiseID = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", lblockitem.FranchiseID);
                ViewBag.BlockTypeID = new SelectList(db.DesignBlockTypes.Where(x => x.IsActive == true && x.Name.ToLower().Trim() != "product gallery").ToList().OrderBy(x => x.Name), "ID", "Name", lblockitem.DesignBlockTypeID);
                DateTime lSDate = CommonFunctions.GetProperDate(SDate);
                DateTime lEDate = CommonFunctions.GetProperDate(EDate);

                blockitem.StartDate = lSDate;
                blockitem.EndDate = lEDate;
               // blockitem.ImageName = lblockitem.ImageName;

               //----------- added by Ashwini Meshram 06-jan-2016 to update logo-----------------------------------------------


                Int32 FranchiseID = blockitem.FranchiseID;////added
                Int64 CityID = db.Franchises.Where(x => x.ID == FranchiseID).FirstOrDefault().BusinessDetail.Pincode.CityID;
               
                //int imgNo = db.BlockItemsLists.Where(x => x.DesignBlockTypeID == lblockitem.DesignBlockTypeID).Count() + 1; //commented by harshada on 14/02/2017
              
                // blockitem.ImageName = "/" + CityID.ToString().Trim() + "/" + lblockitem.DesignBlockTypeID.ToString().Trim() + "/" + imgNo.ToString().Trim() + Path.GetExtension(file.FileName);////hide
                
                //***added by harshada on 14/02/2017 ***//
                string[] arrImageName = lblockitem.ImageName.Split('/', '.'); 
                int imgNo = int.Parse(arrImageName[arrImageName.Length - 2]);//added by harshada on 14/02/2017
                
               // if (file == null)
               //  {
                    // blockitem.ImageName = "/" + CityID.ToString().Trim() + "/" + blockitem.FranchiseID + "/" + lblockitem.DesignBlockTypeID.ToString().Trim() + "/" + imgNo.ToString().Trim() + Path.GetExtension(file.FileName);////added
              //   }
               //  else
             //    {
                     blockitem.ImageName = lblockitem.ImageName;
                     
              //   }
                //***end of code by harshada on 14/02/2017 ***//
                //**********************Commented by Ashwini Meshram**********************************************************************************//

                 //string[] src = lblockitem.ImageName.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                 //if (file != null)
                 //{
                 //    blockitem.ImageName = lblockitem.ImageName.Split('.')[0] + Path.GetExtension(file.FileName);
                 //}
                //------------------------------------------------------------------------------------------------------------------------------------
                string msg;
                if (this.IsValidated(out msg, blockitem))
                {
                    List<object> paramValues = new List<object>();
                    paramValues.Add(blockitem.ID);
                    paramValues.Add(lblockitem.FranchiseID);
                    paramValues.Add(lblockitem.DesignBlockTypeID);
                    paramValues.Add(blockitem.StartDate);
                    paramValues.Add(blockitem.EndDate);
                    paramValues.Add(blockitem.SequenceOrder);
                    paramValues.Add(blockitem.ImageName);
                    paramValues.Add(blockitem.LinkUrl);
                    paramValues.Add(blockitem.Tooltip);
                    paramValues.Add(blockitem.IsActive);
                    paramValues.Add(lblockitem.CreateDate);
                    paramValues.Add(lblockitem.CreatedBy);
                    paramValues.Add(DateTime.UtcNow.AddHours(5.5));
                    paramValues.Add(BusinessLogicLayer.CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"])));
                    paramValues.Add("Net Browser");
                    paramValues.Add("x");
                    paramValues.Add(BusinessLogicLayer.CommonFunctions.GetClientIP());
                    paramValues.Add(DBNull.Value);
                    paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE);
                    paramValues.Add(DBNull.Value);

                    int resultCode;
                    ViewBag.Messaage = BusinessLogicLayer.HomePageBlockItemsList.Insertupdate_BlockItemsList(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.UPDATE, out resultCode, System.Web.HttpContext.Current.Server);
                                       
                    bool IsUploaded = false;
                    bool IsDeleted = false;
                    if (file != null && resultCode == 2)
                    {
                       //****** For Editing Image, we delete image file from FTP server if Exist ********//
                        IsDeleted = CommonFunctions.DeleteHomePageImage(file, CityID, FranchiseID, lblockitem.DesignBlockTypeID, imgNo + Path.GetExtension(file.FileName));
                        //upload cat image here
                        //IsUploaded = CommonFunctions.UploadHomePageImage(file, Convert.ToInt64(src[0]), lblockitem.DesignBlockTypeID, src[2].Split('.')[0] + Path.GetExtension(file.FileName));////hide
                      
                        //-------Chages Done by AShwini Meshram To Update Logo------------------
                      
                        //IsUploaded = CommonFunctions.UploadHomePageImage(file, Convert.ToInt64(src[0]), FranchiseID, lblockitem.DesignBlockTypeID, src[2].Split('.')[0] + Path.GetExtension(file.FileName));////added
                        IsUploaded = CommonFunctions.UploadHomePageImage(file, CityID, FranchiseID, lblockitem.DesignBlockTypeID, imgNo + Path.GetExtension(file.FileName));
                    }
                    if (!IsUploaded && file != null && resultCode == 2)
                    {
                        ViewBag.Messaage = ViewBag.Messaage + " Problem in Uploading Block Item Image...";
                    }
                }
                else
                {
                    ViewBag.Messaage = msg;
                }


                // this.ViewBagList(franchiseMenu.ID);
                //return View();
                TempData["Message"] = ViewBag.Messaage;
                return RedirectToAction("Index", new { FranchiseList = lblockitem.FranchiseID });
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Edit[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Updation!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Updation!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        #endregion

        #region Delete

        //
        // GET: /FranchiseMenu/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "HomePageBlocksItem/CanDelete")]
        public ActionResult Delete(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BlockItemsList item = db.BlockItemsLists.Find(id);
                if (item == null)
                {
                    return HttpNotFound();
                }

                BusinessLogicLayer.ReadConfig rcKey = new ReadConfig(System.Web.HttpContext.Current.Server);
                item.ImageName = rcKey.HOME_IMAGE_HTTP + item.ImageName;
                //item.StartDate=item.StartDate.
                ViewBag.FranchiseName = db.Franchises.Where(x => x.ID == item.FranchiseID).FirstOrDefault().BusinessDetail.Name;
                ViewBag.BlockTypeName = item.DesignBlockType.Name;
                ViewBag.ImageWidth = item.DesignBlockType.ImageWidth + "px";
                ViewBag.ImageHeight = item.DesignBlockType.ImageHeight + "px";

                return View(item);
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Delete[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Deletion!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                return View();
            }
        }

        //
        // POST: /FranchiseMenu/Delete/5
        [SessionExpire]
        [CustomAuthorize(Roles = "HomePageBlocksItem/CanDelete")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                BlockItemsList item = db.BlockItemsLists.Find(id);

                List<object> paramValues = new List<object>();
                paramValues.Add(id);
                paramValues.Add(DataAccessLayer.Enumerators.DB_OPERATIONS.DELETE.ToString());
                paramValues.Add(DBNull.Value);

                int resultCode;
                ViewBag.Messaage = BusinessLogicLayer.HomePageBlockItemsList.Delete_BlockItem(paramValues, DataAccessLayer.Enumerators.DB_OPERATIONS.DELETE, out resultCode, System.Web.HttpContext.Current.Server);

                if (resultCode == 3)
                {
                    TempData["Message"] = ViewBag.Messaage;
                    return RedirectToAction("Index", new { FranchiseList = item.FranchiseID });
                }
                else
                    return View(item);
                //return View();
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[Http Request] :- Delete[HttpPost]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                //ViewBag.Message = "Sorry! Problem in customer registration!!";
                ModelState.AddModelError("Message", "Sorry! Problem in Record Deletion!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Record Deletion!!" + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                ViewBag.Messaage = "Unable to Delete Bank Detail :- " + ex.InnerException.ToString();
                return View();
            }

        }

        #endregion

        #region Methods
        public PartialViewResult GetBlocksItemsList(Int32 FranchiseID, Int64 BlockTypeID)
        {
            ModelLayer.Models.ViewModel.HomePageBlockItemsList bils = new ModelLayer.Models.ViewModel.HomePageBlockItemsList();
            try
            {
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                BusinessLogicLayer.HomePageBlockItemsList obj = new BusinessLogicLayer.HomePageBlockItemsList();
                DataTable dt = new DataTable();
                dt = obj.Select_BlockItemsList(FranchiseID, BlockTypeID, System.Web.HttpContext.Current.Server);

                /*Select All the Shop By Franchise */
                bils.blockItemsList = (from n in dt.AsEnumerable()
                                       select new ModelLayer.Models.ViewModel.HomePageBlockItemsViewModel
                                       {
                                           ID = n.Field<Int64>("ID"),
                                           FranchiseID = n.Field<int>("FranchiseID"),
                                           SequenceOrder = n.Field<int?>("SequenceOrder") == null ? 0 : Convert.ToInt32(n.Field<int?>("SequenceOrder")),
                                           ImageName = rcKey.HOME_IMAGE_HTTP + n.Field<string>("ImageName"),
                                           LinkUrl = n.Field<string>("LinkUrl"),
                                           Tooltip = n.Field<string>("Tooltip") == null ? string.Empty : n.Field<string>("Tooltip"),
                                           IsActive = n.Field<bool>("IsActive")
                                       }).OrderBy(x => x.SequenceOrder).ToList();

                ViewBag.BlockType = BlockTypeID;
                ViewBag.Franchise = FranchiseID;
                return PartialView("_EditBlocksItemsList", bils);
            }
            catch
            {
                return PartialView("_EditBlocksItemsList", bils);
            }

        }

        private bool IsValidated(out string msg, BlockItemsList objSp)
        {
            msg = string.Empty;
            try
            {
                System.Text.StringBuilder str = new System.Text.StringBuilder("Following Errors Are Found" + Environment.NewLine);
                int Count = 0;
                if (objSp.FranchiseID < 0)
                {
                    ++Count;
                    str.Append(Count.ToString() + ") Invalid Franchise" + Environment.NewLine);
                }
                if (objSp.DesignBlockTypeID < 0)
                {
                    ++Count;
                    str.Append(Count.ToString() + ")  Invalid Block Type" + Environment.NewLine);
                }
                if (objSp.SequenceOrder < 0)
                {
                    ++Count;
                    str.Append(Count.ToString() + ") Invalid Sequence" + Environment.NewLine);
                }
                if (objSp.IsActive == null)
                {
                    ++Count;
                    str.Append(Count.ToString() + ") Invalid IsActive Value" + Environment.NewLine);
                }

                if (Count > 0)
                {
                    msg = str.ToString();
                    return false;
                }
                else
                {

                    return true;
                }
            }
            catch
            {

                return false;
            }
        }

        public JsonResult GetBlockDetails(Int64 BlockTypeID)
        {
            var blockDetail = (from dbt in db.DesignBlockTypes where dbt.ID == BlockTypeID select new { dbt.ImageHeight, dbt.ImageWidth }).FirstOrDefault();
            return Json(blockDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateSequence(ModelLayer.Models.ViewModel.HomePageBlockItemsList bils, Int32 Franchise, Int64 BlockType)
        {
            ViewBag.FranchiseList = new SelectList((from f in db.Franchises where f.ID != 1 select new { f.ID, f.BusinessDetail.Name }).ToList().OrderBy(x => x.Name), "ID", "Name", Franchise);
            ViewBag.BlockTypeList = new SelectList(db.DesignBlockTypes.Where(x => x.IsActive == true).ToList().OrderBy(x => x.Name), "ID", "Name", BlockType);

            BusinessLogicLayer.HomePageBlockItemsList obj = new BusinessLogicLayer.HomePageBlockItemsList();
            Int64 userID = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));

            TempData["Message"] = obj.UpdateSeuence_BlockItemsList(bils, Franchise, BlockType, userID, System.Web.HttpContext.Current.Server);

            return RedirectToAction("Index");
        }

        #endregion
    }


}