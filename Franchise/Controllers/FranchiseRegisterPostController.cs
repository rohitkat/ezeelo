
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Franchise.Controllers
{
    public class FranchiseRegisterPostController : Controller
    {
        private ModelLayer.Models.EzeeloDBContext db = new ModelLayer.Models.EzeeloDBContext();

        //
        // GET: /MerchantRegistrationPost/
        
        public ActionResult Register()
        {
            try
            {
                ModelLayer.Models.ViewModel.FranchiseRegisterNowViewModel franchiseRegister = (ModelLayer.Models.ViewModel.FranchiseRegisterNowViewModel)TempData["FRNVM"];

                if (franchiseRegister != null)
               {
                   ModelLayer.Models.ViewModel.PersonalDetailsViewModel pdvm = this.FillTextControl(franchiseRegister);

                   pdvm.ID = franchiseRegister.ID;

                   this.FillDropDown(franchiseRegister.SalutationID);

                   TempData["PDVM"] = pdvm;
                   TempData.Keep();

                   return View(pdvm);
               }
               else
               {
                   return RedirectToAction("Register", "FranchiseRegisterNow");
               }
                
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseRegistrationPostController][GET:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseRegistrationPostController][GET:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }

            return View();           
        }

        private ModelLayer.Models.ViewModel.PersonalDetailsViewModel FillTextControl(ModelLayer.Models.ViewModel.FranchiseRegisterNowViewModel frnvm)
        {
            ModelLayer.Models.ViewModel.PersonalDetailsViewModel pdvm = new ModelLayer.Models.ViewModel.PersonalDetailsViewModel();
            pdvm.FirstName = frnvm.FirstName;
            pdvm.MiddleName = frnvm.MiddleName;
            pdvm.LastName = frnvm.LastName;
            pdvm.Email = frnvm.Email;
            pdvm.Mobile = frnvm.Mobile;
            pdvm.PinCode = frnvm.PinCode;
            return pdvm;
        }

        [HttpPost]
        public ActionResult Register(ModelLayer.Models.ViewModel.PersonalDetailsViewModel personalDetails, int Salutation, string Gender)
        {
            try
            {

                this.UpdatePersonalDetails(personalDetails, Salutation, Gender);

                TempData.Keep();

                this.FillDropDown(Salutation);

                return RedirectToAction("CreateBusinessDetails", "FranchiseRegisterPost");

                //return View(personalDetails);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseRegistrationPostController][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseRegistrationPostController][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            
            return View();
        }

        public ActionResult CreateBusinessDetails()
        {
            ModelLayer.Models.ViewModel.FranchiseRegisterNowViewModel mrnvm = (ModelLayer.Models.ViewModel.FranchiseRegisterNowViewModel)TempData["FRNVM"];
            ModelLayer.Models.ViewModel.PersonalDetailsViewModel pdvm = (ModelLayer.Models.ViewModel.PersonalDetailsViewModel)TempData["PDVM"];
            ModelLayer.Models.ViewModel.BussinessDetailsViewModel bdvm = new ModelLayer.Models.ViewModel.BussinessDetailsViewModel();

            bdvm = this.FillBusinessDetails(mrnvm, pdvm);
            bdvm.ID = mrnvm.BusinessID;

            bdvm.ID = mrnvm.BusinessID;

            TempData.Keep();

            return View(bdvm);
        }
        
        private ModelLayer.Models.ViewModel.BussinessDetailsViewModel FillBusinessDetails(ModelLayer.Models.ViewModel.FranchiseRegisterNowViewModel mrnvm, ModelLayer.Models.ViewModel.PersonalDetailsViewModel pdvm)
        {
            try
            {
                ModelLayer.Models.ViewModel.BussinessDetailsViewModel bdvm = new ModelLayer.Models.ViewModel.BussinessDetailsViewModel();

                int lBusinestypeID = 0;

                ModelLayer.Models.BusinessType businesType = new ModelLayer.Models.BusinessType();

                ViewBag.BussinessType = new SelectList(db.BusinessTypes, "ID", "Name", 0);

              List<ModelLayer.Models.SourceOfInfo> ldata = new List<ModelLayer.Models.SourceOfInfo>();
                ldata = db.SourceOfInfoes.ToList();
                ViewBag.InfoSource = new SelectList(ldata, "ID", "Name", 0);


                bdvm.ContactPerson = mrnvm.FirstName + " " + mrnvm.MiddleName + " " + mrnvm.LastName;
                bdvm.Mobile = pdvm.Mobile;
                bdvm.Email = pdvm.Email;

                bdvm.Address = pdvm.Address;
                bdvm.PinCode = pdvm.PinCode;
                //bdvm.Website = mrnvm.Website;

                bdvm.LoginID = mrnvm.LoginID;
                bdvm.BussinessName = "";//mrnvm.BussinessName;
                bdvm.BussinessTypeID = lBusinestypeID;



                return bdvm;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[M:FillBusinessDetails]", "Can't fill deafult text values!" + Environment.NewLine + ex.Message);
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

                return RedirectToAction("CreateFranchise", "FranchiseRegisterPost");

                //return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseRegistrationPostController][POST:CreateBusinessDetails]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseRegistrationPostController][POST:CreateBusinessDetails]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }

            return View();
        }

        public ActionResult CreateFranchise()
        {

            ModelLayer.Models.ViewModel.BussinessDetailsViewModel bdvm = (ModelLayer.Models.ViewModel.BussinessDetailsViewModel)TempData["BDVM"];
            ModelLayer.Models.ViewModel.FranchiseDetailsViewModel fdvm = new ModelLayer.Models.ViewModel.FranchiseDetailsViewModel();

            fdvm.BusinessDetailsID = bdvm.ID;

            fdvm = this.FillFranchiseView(bdvm);
            TempData["FDVM"] = fdvm;

            TempData.Keep();

            return View(fdvm);
        }

        private ModelLayer.Models.ViewModel.FranchiseDetailsViewModel FillFranchiseView(ModelLayer.Models.ViewModel.BussinessDetailsViewModel bdvm)
        {
            try
            {
                ModelLayer.Models.ViewModel.FranchiseDetailsViewModel fdvm = new ModelLayer.Models.ViewModel.FranchiseDetailsViewModel();

                fdvm.Address = bdvm.Address;

                fdvm.ContactPerson = bdvm.ContactPerson;

                fdvm.Email = bdvm.Email;
                fdvm.FAX = bdvm.Fax;
                fdvm.Landline = bdvm.Landline1;
                fdvm.Mobile = bdvm.Mobile;
                fdvm.Pincode = bdvm.PinCode;
                               
                return fdvm;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[FranchiseRegisterPostController][FillFranchiseView]", "Can't fill deafult text values!" + Environment.NewLine + ex.Message);
            }

        }

        [HttpPost]
        public ActionResult CreateFranchise(ModelLayer.Models.ViewModel.FranchiseDetailsViewModel franchiseDetails)
        {
            try
            {

                this.InsertFranchise(franchiseDetails);
                TempData["FDVM"] = franchiseDetails;

                TempData.Keep();

                return RedirectToAction("CreateBankDetails", "FranchiseRegisterPost");
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchhiseRegisterPostController][POST:CreateFranchise]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the dropDown values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchhiseRegisterPostController][POST:CreateFranchise]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }

            return View();
        }
        
        public ActionResult CreateBankDetails()
        {
            ModelLayer.Models.ViewModel.BankDetailsViewModel bdvm = new ModelLayer.Models.ViewModel.BankDetailsViewModel();

            ModelLayer.Models.ViewModel.FranchiseRegisterNowViewModel mrnvm = (ModelLayer.Models.ViewModel.FranchiseRegisterNowViewModel)TempData["FRNVM"];

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
                throw new BusinessLogicLayer.MyException("[FillBankView]", "Can't feel drop down list!" + Environment.NewLine + ex.Message);
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

                RedirectToAction("Index", "Home");
                //return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with bank values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[FranchiseRegistrationPostController][POST:CreateBankDetails]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the bank values!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[FranchiseRegistrationPostController][POST:CreateBankDetails]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();

        }

        private void InsertBank(ModelLayer.Models.ViewModel.BankDetailsViewModel bdvm, int bank, int accountTypes)
        {
            try
            {
                ModelLayer.Models.ViewModel.BussinessDetailsViewModel businessDetails = (ModelLayer.Models.ViewModel.BussinessDetailsViewModel)TempData["BDVM"];

                ModelLayer.Models.ViewModel.FranchiseDetailsViewModel fdvm = (ModelLayer.Models.ViewModel.FranchiseDetailsViewModel)TempData["FDVM"];

                ModelLayer.Models.OwnerBank ownerBank = new ModelLayer.Models.OwnerBank();

                ownerBank.AccountNumber = bdvm.AccountNumber;
                ownerBank.BankAccountTypeID = accountTypes;
                ownerBank.BankID = bank;

                ownerBank.OwnerID = fdvm.ID;

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
                throw new BusinessLogicLayer.MyException("[InsertBank]", "Can't update delivery details!" + Environment.NewLine + ex.Message);
            }
            
        }

        private void InsertFranchise(ModelLayer.Models.ViewModel.FranchiseDetailsViewModel fdvm)
        {
            try
            {
                ModelLayer.Models.ViewModel.BussinessDetailsViewModel businessDetails = (ModelLayer.Models.ViewModel.BussinessDetailsViewModel)TempData["BDVM"];
                
                ModelLayer.Models.Franchise franchise = new ModelLayer.Models.Franchise();

                franchise.BusinessDetailID = businessDetails.ID;
                franchise.ServiceNumber = fdvm.ServiceNumber;
                franchise.ContactPerson = fdvm.ContactPerson;
                franchise.Mobile = fdvm.Mobile;
                franchise.Email = fdvm.Email;
                franchise.Landline = fdvm.Landline;
                franchise.FAX = fdvm.FAX;

                ModelLayer.Models.Pincode pin = new ModelLayer.Models.Pincode();
                pin = db.Pincodes.Where(x => x.Name == fdvm.Pincode).FirstOrDefault();

                if (pin == null)
                {
                    franchise.PincodeID = null;
                }
                else
                {
                    franchise.PincodeID = pin.ID;
                }
                
                franchise.Address = fdvm.Address;

                franchise.IsActive = false;
                franchise.CreateBy = 1;
                franchise.CreateDate = DateTime.UtcNow.AddHours(5.30);
                franchise.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                franchise.DeviceID = string.Empty;
                franchise.DeviceType = string.Empty;


                db.Franchises.Add(franchise);
                db.SaveChanges();

              
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[InsertFranchise]", "Can't update delivery details!" + Environment.NewLine + ex.Message);
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
                lBusiness.BusinessTypeID = businessDetails.BussinessTypeID;

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
                throw new BusinessLogicLayer.MyException("[UpdateBusinessDetails]", "Can't update business details!" + Environment.NewLine + ex.Message);
            }
        }

        private void UpdatePersonalDetails(ModelLayer.Models.ViewModel.PersonalDetailsViewModel personalDetails, int Salutation, string Gender)
        {
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
                lPersonal.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                lPersonal.DeviceID = string.Empty;
                lPersonal.DeviceType = string.Empty;

                //db.Entry(personalDetails).CurrentValues.SetValues(lPersonal);
                db.Entry(lPersonal).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[UpdatePersonalDetails]", "Can't update personal details!" + Environment.NewLine + ex.Message);
            }
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
                throw new BusinessLogicLayer.MyException("[FillDropDown]", "Can't feel drop down list!" + Environment.NewLine + ex.Message);
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