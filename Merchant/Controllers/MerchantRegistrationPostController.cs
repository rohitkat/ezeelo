
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Merchant.Controllers
{
    public class MerchantRegistrationPostController : Controller
    {
        private ModelLayer.Models.EzeeloDBContext db = new ModelLayer.Models.EzeeloDBContext();
        //
        // GET: /MerchantRegistrationPost/
        
        public ActionResult Create()
        {
            try
            {
               ModelLayer.Models.ViewModel.MerchantRegisterNowViewModel obj = (ModelLayer.Models.ViewModel.MerchantRegisterNowViewModel)TempData["merchantRegisterNow"];

               if (obj != null)
               {
                   ModelLayer.Models.ViewModel.PersonalDetailsViewModel pdvm = this.FillTextControl(obj);

                   pdvm.ID = obj.ID;

                   this.FillDropDown(obj.SalutationID);

                   TempData["PDVM"] = pdvm;
                   TempData.Keep();

                   return View(pdvm);
               }
               else
               {
                  
                  return RedirectToAction("Create", "MerchantRegisterNow");
               }
                
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantRegistrationPostController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantRegistrationPostController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }

            return View();           
        }

        private ModelLayer.Models.ViewModel.PersonalDetailsViewModel FillTextControl(ModelLayer.Models.ViewModel.MerchantRegisterNowViewModel obj)
        {
            ModelLayer.Models.ViewModel.PersonalDetailsViewModel pdvm = new ModelLayer.Models.ViewModel.PersonalDetailsViewModel();
            pdvm.FirstName = obj.FirstName;
            pdvm.MiddleName = obj.MiddleName;
            pdvm.LastName = obj.LastName;
            pdvm.Email = obj.Email;
            pdvm.Mobile = obj.Mobile;
            pdvm.PinCode = obj.PinCode;
            return pdvm;
        }

        [HttpPost]
        public ActionResult Create(ModelLayer.Models.ViewModel.PersonalDetailsViewModel personalDetails, int Salutation, string Gender)
        {
            try
            {

                this.UpdatePersonalDetails(personalDetails, Salutation, Gender);

                TempData.Keep();

                this.FillDropDown(Salutation);

                return RedirectToAction("CreateBusinessDetails", "MerchantRegistrationPost");
                
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantRegistrationPostController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantRegistrationPostController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            
            return View();
        }

        public ActionResult CreateBusinessDetails()
        {
            ModelLayer.Models.ViewModel.MerchantRegisterNowViewModel mrnvm = (ModelLayer.Models.ViewModel.MerchantRegisterNowViewModel)TempData["merchantRegisterNow"];
            ModelLayer.Models.ViewModel.PersonalDetailsViewModel pdvm = (ModelLayer.Models.ViewModel.PersonalDetailsViewModel)TempData["PDVM"];
            ModelLayer.Models.ViewModel.BussinessDetailsViewModel bdvm = new ModelLayer.Models.ViewModel.BussinessDetailsViewModel();

            bdvm = this.FillBusinessDetails(mrnvm, pdvm);

            bdvm.ID = mrnvm.BusinessID;

            TempData.Keep();

            return View(bdvm);
        }

        private ModelLayer.Models.ViewModel.BussinessDetailsViewModel FillBusinessDetails(ModelLayer.Models.ViewModel.MerchantRegisterNowViewModel mrnvm, ModelLayer.Models.ViewModel.PersonalDetailsViewModel pdvm)
        {
            try
            {
                ModelLayer.Models.ViewModel.BussinessDetailsViewModel bdvm = new ModelLayer.Models.ViewModel.BussinessDetailsViewModel();

                int lBusinestypeID = 0;

                ModelLayer.Models.BusinessType businesType = new ModelLayer.Models.BusinessType();

                ViewBag.BussinessType = new SelectList(db.BusinessTypes, "ID", "Name", 0);
                ViewBag.InfoSource = new SelectList(db.SourceOfInfoes, "ID", "Name", 0);


                bdvm.ContactPerson = mrnvm.FirstName + " " + mrnvm.MiddleName + " " + mrnvm.LastName;
                bdvm.Mobile = pdvm.Mobile;
                bdvm.Email = pdvm.Email;

                bdvm.Address = pdvm.Address;
                bdvm.PinCode = pdvm.PinCode;
                bdvm.Website = mrnvm.Website;

                bdvm.LoginID = mrnvm.LoginID;
                bdvm.BussinessName = mrnvm.BussinessName;
                bdvm.BussinessTypeID = lBusinestypeID;


                return bdvm;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[MerchantRegistrationPostController][FillBusinessDetails]", "Can't fill deafult text values!" + Environment.NewLine + ex.Message);
            }

        }

        [HttpPost]
        public ActionResult CreateBusinessDetails(ModelLayer.Models.ViewModel.BussinessDetailsViewModel businessDetails, int?  BussinessType, int? InfoSource)
        {
            try
            {
                ViewBag.BussinessType = new SelectList(db.BusinessTypes, "ID", "Name", BussinessType);

                ViewBag.InfoSource = new SelectList(db.SourceOfInfoes, "ID", "Name", InfoSource);

                businessDetails.BussinessTypeID = Convert.ToInt32(BussinessType);

                this.UpdateBusinessDetails(businessDetails);

                TempData["BDVM"] = businessDetails;

                TempData.Keep();

                return RedirectToAction("CreateShopDetails", "MerchantRegistrationPost");

                //return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantRegistrationPostController][POST:CreateBusinessDetails]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantRegistrationPostController][POST:CreateBusinessDetails]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }

            return View();
        }

        public ActionResult CreateShopDetails()
        {

            ModelLayer.Models.ViewModel.MerchantRegisterNowViewModel mrnvm = (ModelLayer.Models.ViewModel.MerchantRegisterNowViewModel)TempData["merchantRegisterNow"];

            ModelLayer.Models.ViewModel.BussinessDetailsViewModel bdvm = (ModelLayer.Models.ViewModel.BussinessDetailsViewModel)TempData["BDVM"];

            ModelLayer.Models.ViewModel.ShopDetailsPostViewModel sdpvm = new ModelLayer.Models.ViewModel.ShopDetailsPostViewModel();

            sdpvm = this.FillShopDetails(mrnvm, bdvm);

            sdpvm.ID = mrnvm.ShopID;

            TempData.Keep();

            return View(sdpvm);
        }

        private ModelLayer.Models.ViewModel.ShopDetailsPostViewModel FillShopDetails(ModelLayer.Models.ViewModel.MerchantRegisterNowViewModel mrnvm, ModelLayer.Models.ViewModel.BussinessDetailsViewModel bdvm)
        {

            try
            {
                ModelLayer.Models.ViewModel.ShopDetailsPostViewModel sdpvm = new ModelLayer.Models.ViewModel.ShopDetailsPostViewModel();

                // Fill Weekly Off DropDown
                List<SelectListItem> weeklyOff = new List<SelectListItem>();
                weeklyOff.Add(new SelectListItem { Text = "NO OFF", Value = "0", Selected = true });
                weeklyOff.Add(new SelectListItem { Text = "Monday", Value = "1" });
                weeklyOff.Add(new SelectListItem { Text = "Tuesday", Value = "2" });
                weeklyOff.Add(new SelectListItem { Text = "Wednesday", Value = "3" });
                weeklyOff.Add(new SelectListItem { Text = "Thursday", Value = "4" });
                weeklyOff.Add(new SelectListItem { Text = "Friday", Value = "5" });
                weeklyOff.Add(new SelectListItem { Text = "Saturday", Value = "6" });
                weeklyOff.Add(new SelectListItem { Text = "Sunday", Value = "7" });

                ViewBag.WeeklyOff = new SelectList(weeklyOff, "Value", "Text");

                // Fill Inventory Dropdown
                List<SelectListItem> isInvemtory = new List<SelectListItem>();
                isInvemtory.Add(new SelectListItem { Text = "NO", Value = "0", Selected = true });
                isInvemtory.Add(new SelectListItem { Text = "YES", Value = "1" });

                ViewBag.ManageInventory = new SelectList(isInvemtory, "Value", "Text");

                sdpvm.BusinessName = bdvm.BussinessName;
                sdpvm.BusinessDetailsID = bdvm.ID;

                sdpvm.ContactPerson = bdvm.ContactPerson;

                sdpvm.Mobile = bdvm.Mobile;
                sdpvm.Email = bdvm.Email;
                sdpvm.Landline = bdvm.Landline1;
                sdpvm.FAX = bdvm.Fax;

                sdpvm.VAT = mrnvm.WAT;
                sdpvm.TIN = mrnvm.TIN;
                sdpvm.PAN = mrnvm.PAN;

                sdpvm.Pincode = bdvm.PinCode;

                sdpvm.Address = bdvm.Address;

                sdpvm.Website = mrnvm.Website;

                return sdpvm;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[MerchantRegistrationPostController][FillShopDetails]", "Can't fill deafult text values!" + Environment.NewLine + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult CreateShopDetails(ModelLayer.Models.ViewModel.ShopDetailsPostViewModel shopDetails)
        {   
            try
            {
                // Fill Weekly Off DropDown
                List<SelectListItem> weeklyOff = new List<SelectListItem>();
                weeklyOff.Add(new SelectListItem { Text = "NO OFF", Value = "0", Selected = true });
                weeklyOff.Add(new SelectListItem { Text = "Monday", Value = "1" });
                weeklyOff.Add(new SelectListItem { Text = "Tuesday", Value = "2" });
                weeklyOff.Add(new SelectListItem { Text = "Wednesday", Value = "3" });
                weeklyOff.Add(new SelectListItem { Text = "Thursday", Value = "4" });
                weeklyOff.Add(new SelectListItem { Text = "Friday", Value = "5" });
                weeklyOff.Add(new SelectListItem { Text = "Saturday", Value = "6" });
                weeklyOff.Add(new SelectListItem { Text = "Sunday", Value = "7" });

                ViewBag.WeeklyOff = new SelectList(weeklyOff, "Value", "Text");

                // Fill Inventory Dropdown
                List<SelectListItem> isInvemtory = new List<SelectListItem>();
                isInvemtory.Add(new SelectListItem { Text = "NO", Value = "0", Selected = true });
                isInvemtory.Add(new SelectListItem { Text = "YES", Value = "1" });

                ViewBag.ManageInventory = new SelectList(isInvemtory, "Value", "Text");

                ModelLayer.Models.ViewModel.MerchantRegisterNowViewModel mrnvm = (ModelLayer.Models.ViewModel.MerchantRegisterNowViewModel)TempData["merchantRegisterNow"];
                ModelLayer.Models.ViewModel.PersonalDetailsViewModel pdvm = (ModelLayer.Models.ViewModel.PersonalDetailsViewModel)TempData["PDVM"];
                ModelLayer.Models.ViewModel.BussinessDetailsViewModel bdvm = (ModelLayer.Models.ViewModel.BussinessDetailsViewModel)TempData["BDVM"];
                             

                this.UpdateShopDetails(shopDetails);

                TempData["SDPVM"] = shopDetails;

                TempData.Keep();

                return RedirectToAction("CreateDeliveryDetails", "MerchantRegistrationPost");
               // return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantRegistrationPostController][POST:CreateShopDetails]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantRegistrationPostController][POST:CreateShopDetails]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }

            return View();
        }

        public ActionResult CreateDeliveryDetails()
        {
            ModelLayer.Models.ViewModel.DeliveryDetailsViewModel ddvm = new ModelLayer.Models.ViewModel.DeliveryDetailsViewModel();

            this.FillDeliveryView();

            TempData.Keep();

            return View(ddvm);
        }

        private void FillDeliveryView()
        {
            try
            {

                var lDpList = (from bt in db.BusinessTypes
                               join bd in db.BusinessDetails on bt.ID equals bd.BusinessTypeID
                               where bt.Prefix == "GBDP" && bd.IsActive == true
                               select new {bd.ID,bd.Name}).ToList();

                if (lDpList.Count > 0)
                    ViewBag.deliveryPartner = new SelectList(lDpList, "ID", "Name");
                else
                {
                    List<SelectListItem> dp = new List<SelectListItem>();
                    dp.Add(new SelectListItem { Text = "No Delivery Partner Exists", Value = "0", Selected = true });
                   
                    ViewBag.deliveryPartner = new SelectList(dp, "Value", "Text", 0);
                }


                var lFrList = (from  bd in db.BusinessDetails
                               join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
                               join fc in db.Franchises on bd.ID equals fc.BusinessDetailID
                               where bt.Prefix == "GBFR" && bd.IsActive == true
                               select new { fc.ID, bd.Name }).ToList();

                if (lFrList.Count > 0)
                    ViewBag.franchise = new SelectList(lFrList, "ID", "Name");
                else
                {
                    List<SelectListItem> fr = new List<SelectListItem>();
                    fr.Add(new SelectListItem { Text = "No Franchise Exists", Value = "0", Selected = true });
                    
                    ViewBag.franchise = new SelectList(fr, "Value", "Text", 0);
                }


                

            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[MerchantRegistrationPostController][FillDeliveryView]", "Can't feel drop down list!" + Environment.NewLine + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult CreateDeliveryDetails(ModelLayer.Models.ViewModel.DeliveryDetailsViewModel ddvm, int deliveryPartner, int franchise)
        {
            try
            {
                ModelLayer.Models.ViewModel.ShopDetailsPostViewModel pdvm = (ModelLayer.Models.ViewModel.ShopDetailsPostViewModel)TempData["SDPVM"];

                this.UpdateDeliveryDetails(ddvm, pdvm.ID, deliveryPartner, franchise);

                TempData["DDVM"] = ddvm;

                TempData.Keep();

                return RedirectToAction("CreateBankDetails", "MerchantRegistrationPost");
               
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantRegistrationPostController][POST:CreateDeliveryDetails]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantRegistrationPostController][POST:CreateDeliveryDetails]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }

            return View();
           
        }

        public ActionResult CreateBankDetails()
        {
            ModelLayer.Models.ViewModel.BankDetailsViewModel bdvm = new ModelLayer.Models.ViewModel.BankDetailsViewModel();

            ModelLayer.Models.ViewModel.MerchantRegisterNowViewModel mrnvm = (ModelLayer.Models.ViewModel.MerchantRegisterNowViewModel)TempData["merchantRegisterNow"];

            ModelLayer.Models.ViewModel.BussinessDetailsViewModel bdv = (ModelLayer.Models.ViewModel.BussinessDetailsViewModel)TempData["BDVM"];

            this.FillBankView();

            bdvm.OwnerID = mrnvm.ID;
            bdvm.BusinessTypeID = bdv.BussinessTypeID;

            TempData.Keep();

            return View(bdvm);
        }

        private void FillBankView()
        {
            try
            {

                var lBankList = (from bd in db.Banks  where bd.IsActive == true
                               select new { bd.ID, bd.Name }).ToList();

                if (lBankList.Count > 0)
                    ViewBag.bank = new SelectList(lBankList, "ID", "Name");
                else
                {
                    List<SelectListItem> bd = new List<SelectListItem>();
                    bd.Add(new SelectListItem { Text = "No Bank Exists", Value = "0", Selected = true });

                    ViewBag.bank = new SelectList(bd, "Value", "Text", 0);
                }

                var lAccountType = (from ba in db.BankAccountTypes
                                 where ba.IsActive == true
                                 select new { ba.ID, ba.Name }).ToList();

                if (lAccountType.Count > 0)
                    ViewBag.accountTypes = new SelectList(lAccountType, "ID", "Name");
                else
                {
                    List<SelectListItem> bd = new List<SelectListItem>();
                    bd.Add(new SelectListItem { Text = "No Types", Value = "0", Selected = true });

                    ViewBag.accountTypes = new SelectList(bd, "Value", "Text", 0);
                }



            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[MerchantRegistrationPostController][FillBankView]", "Can't feel drop down list!" + Environment.NewLine + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult CreateBankDetails(ModelLayer.Models.ViewModel.BankDetailsViewModel bdvm, int bank, int accountTypes)
        {
            
            this.FillBankView();

            try
            {
                TempData.Keep();

                this.InsertBank(bdvm, bank, accountTypes);

                bdvm = null;

                ModelState.AddModelError("MSG", "Congratulations!!" + Environment.NewLine + "You are successfully registered as merchant with eZeelo!!");

               // RedirectToAction("Index", "Home");
                //return View();
                return RedirectToAction("Create", "MerchantRegisterNow");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with bank values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MerchantRegistrationPostController][POST:CreateBankDetails]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the bank values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MerchantRegistrationPostController][POST:CreateBankDetails]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();

        }

        private void InsertBank(ModelLayer.Models.ViewModel.BankDetailsViewModel bdvm, int bank, int accountTypes)
        {
            try
            {
                ModelLayer.Models.ViewModel.BussinessDetailsViewModel businessDetails = (ModelLayer.Models.ViewModel.BussinessDetailsViewModel)TempData["BDVM"];

                ModelLayer.Models.OwnerBank ownerBank = new ModelLayer.Models.OwnerBank();
                ModelLayer.Models.ViewModel.PersonalDetailsViewModel pdvm = (ModelLayer.Models.ViewModel.PersonalDetailsViewModel)TempData["PDVM"];
                
                ownerBank.OwnerID = pdvm.ID;
                ownerBank.AccountNumber = bdvm.AccountNumber;
                ownerBank.BankAccountTypeID = accountTypes;
                ownerBank.BankID = bank;
                ownerBank.BranchName = bdvm.BranchName;
                ownerBank.BusinessTypeID = businessDetails.BussinessTypeID;
                ownerBank.IFSCCode = bdvm.IFSCcode;
                ownerBank.MICRCode = bdvm.MICRCode;

                ownerBank.IsActive = true;
                ownerBank.CreateBy = 1;
                ownerBank.CreateDate = DateTime.UtcNow.AddHours(5.30);
                ownerBank.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                ownerBank.DeviceID = string.Empty;
                ownerBank.DeviceType = string.Empty;


                db.OwnerBanks.Add(ownerBank);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[MerchantRegistrationPostController][UpdateDeliveryDetails]", "Can't update delivery details!" + Environment.NewLine + ex.Message);
            }
            
        }

        private void UpdateDeliveryDetails(ModelLayer.Models.ViewModel.DeliveryDetailsViewModel ddvm, long pShopID, int deliveryPartner, int franchise)
        {
            try
            {
                ModelLayer.Models.Shop lShop = new ModelLayer.Models.Shop();

                lShop = db.Shops.Where(x => x.ID == pShopID).FirstOrDefault();

                //lShop.DeliveryPartnerId = deliveryPartner;
                lShop.FranchiseID = franchise;
                lShop.IsFreeHomeDelivery = ddvm.FreeHomeDelivery;
                lShop.IsDeliveryOutSource = ddvm.IsDeliveryOutSource;
                lShop.MinimumAmountForFreeDelivery = ddvm.MinimumAmount;

                //lShop.FranchiseID = ddvm.FranchiseID;

                lShop.IsActive = true;
                lShop.ModifyBy = 1;
                lShop.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                lShop.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                lShop.DeviceID = string.Empty;
                lShop.DeviceType = string.Empty;

                db.Entry(lShop).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[MerchantRegistrationPostController][UpdateDeliveryDetails]", "Can't update delivery details!" + Environment.NewLine + ex.Message);
            }
        }

        private void UpdateShopDetails(ModelLayer.Models.ViewModel.ShopDetailsPostViewModel shopDetails)
        {
            try
            {
               ModelLayer.Models.Pincode pin = new ModelLayer.Models.Pincode();
                pin = db.Pincodes.Where(x => x.Name == shopDetails.Pincode).FirstOrDefault();

                if (pin == null)
                {
                    throw new Exception("Pincod is not valid or does not exist");
                }


                ModelLayer.Models.Shop lShop = new ModelLayer.Models.Shop();

                lShop = db.Shops.Where(x => x.ID == shopDetails.ID).FirstOrDefault();

                lShop.PincodeID = Convert.ToInt32(pin.ID);
                lShop.Address = shopDetails.Address;
                lShop.CurrentItSetup = shopDetails.CurrentItSetup;

                lShop.IsActive = true;
                lShop.ModifyBy = 1;
                lShop.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                lShop.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                lShop.DeviceID = string.Empty;
                lShop.DeviceType = string.Empty;


                db.Entry(lShop).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[MerchantRegistrationPostController][UpdateBusinessDetails]", "Can't update business details!" + Environment.NewLine + ex.Message);
            }
        }
        
        private void UpdateBusinessDetails(ModelLayer.Models.ViewModel.BussinessDetailsViewModel businessDetails)
        {
            try
            {

                ModelLayer.Models.Pincode pin = new ModelLayer.Models.Pincode();
                pin = db.Pincodes.Where(x => x.Name == businessDetails.PinCode).FirstOrDefault();

                if (pin == null)
                {
                    throw new Exception("Pincod is not valid or does not exist");
                }


                ModelLayer.Models.BusinessDetail lBusiness = new ModelLayer.Models.BusinessDetail();

                lBusiness = db.BusinessDetails.Where(x => x.ID == businessDetails.ID).FirstOrDefault();

                lBusiness.PincodeID = pin.ID;
                lBusiness.Address = businessDetails.Address;
                lBusiness.BusinessTypeID =businessDetails.BussinessTypeID;

                lBusiness.IsActive = true;
                lBusiness.ModifyBy = 1;
                lBusiness.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                lBusiness.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                lBusiness.DeviceID = string.Empty;
                lBusiness.DeviceType = string.Empty;



                db.Entry(lBusiness).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[MerchantRegistrationPostController][UpdateBusinessDetails]", "Can't update business details!" + Environment.NewLine + ex.Message);
            }
        }

        private void UpdatePersonalDetails(ModelLayer.Models.ViewModel.PersonalDetailsViewModel personalDetails, int Salutation, string Gender)
        {
            ViewBag.Salutation = new SelectList(db.Salutations.ToList(), "ID", "Name", 1);

            try
            {
                ModelLayer.Models.Pincode pin = new ModelLayer.Models.Pincode();
                pin = db.Pincodes.Where(x => x.Name == personalDetails.PinCode).FirstOrDefault();

                if (pin == null)
                {
                    throw new Exception("Pincod is not valid or does not exist");
                }

                ModelLayer.Models.PersonalDetail lPersonal = new ModelLayer.Models.PersonalDetail();

                lPersonal = db.PersonalDetails.Where(x => x.ID == personalDetails.ID).FirstOrDefault();

                lPersonal.PincodeID = pin.ID;
                lPersonal.Address = personalDetails.Address;
                lPersonal.IsActive = true;
                lPersonal.ModifyBy = 1;
                lPersonal.ModifyDate = DateTime.UtcNow.AddHours(5.30);
                //-------------------------------------------------------

                lPersonal.SalutationID = Salutation;
                lPersonal.FirstName = personalDetails.FirstName;
                lPersonal.MiddleName = personalDetails.MiddleName;
                lPersonal.LastName = personalDetails.LastName;
                lPersonal.DOB = Convert.ToDateTime(personalDetails.DOB);
               
                //-------------------------------------------------------
                lPersonal.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                lPersonal.DeviceID = string.Empty;
                lPersonal.DeviceType = string.Empty;

                //this.TestException();

                //db.Entry(personalDetails).CurrentValues.SetValues(lPersonal);
                db.Entry(lPersonal).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[MerchantRegistrationPostController][UpdatePersonalDetails]" + myEx.EXCEPTION_PATH, "Can't update personal details!" + myEx.EXCEPTION_MSG + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[MerchantRegistrationPostController][UpdatePersonalDetails]", "Can't update personal details!" + Environment.NewLine + ex.Message);
            }
        }

        private void TestException()
        {
            throw new BusinessLogicLayer.MyException("[MyController][TestException]", "My Test Exception");
        }

        private void FillDropDown(int salutation)
        {
            try
            {
                if (salutation > 0)
                {
                    ViewBag.Salutation = new SelectList(db.Salutations, "ID", "Name", salutation);
                }
                else
                {
                    ViewBag.Salutation = new SelectList(db.Salutations, "ID", "Name");
                }

                List<SelectListItem> Gender1 = new List<SelectListItem>();
                Gender1.Add(new SelectListItem { Text = "Male", Value = "Male", Selected = true });
                Gender1.Add(new SelectListItem { Text = "Female", Value = "Female" });
                Gender1.Add(new SelectListItem { Text = "Other", Value = "Other" });

                ViewBag.Gender = new SelectList(Gender1, "Value", "Text");
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[MerchantRegistrationPostController][FillDropDown]", "Can't feel drop down list!" + Environment.NewLine + ex.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
	}
}