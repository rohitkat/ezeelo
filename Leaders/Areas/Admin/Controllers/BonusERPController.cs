using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusinessLogicLayer;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.Xml;
using System.Transactions;
using Leaders.Filter;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class BonusERPController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public ActionResult Index()
        {
            ERPBonusPayoutViewModel objERPBonusPayoutViewModel = new ERPBonusPayoutViewModel();
            EzeeMoneyPayout objEzeeMoneyPayout = db.EzeeMoneyPayouts.Where(p => p.IsPaid == true).OrderByDescending(p => p.Id).Take(1).FirstOrDefault();
            if (objEzeeMoneyPayout != null)
            {
                PromotionalERPPayout objPromotionalERPPayoutsL0 = db.PromotionalERPPayouts.FirstOrDefault(p => p.EzeeMoneyPayoutId == objEzeeMoneyPayout.Id && p.Level == 0 && p.IsPaid == false);
                PromotionalERPPayout objPromotionalERPPayoutsL1 = db.PromotionalERPPayouts.FirstOrDefault(p => p.EzeeMoneyPayoutId == objEzeeMoneyPayout.Id && p.Level == 1 && p.IsPaid == false);

                if (objPromotionalERPPayoutsL0 != null)
                {
                    objERPBonusPayoutViewModel.Level0PromotionalERPPayout = objPromotionalERPPayoutsL0.Id;
                }
                if (objPromotionalERPPayoutsL1 != null)
                {
                    objERPBonusPayoutViewModel.Level1PromotionalERPPayout = objPromotionalERPPayoutsL1.Id;
                }
                objERPBonusPayoutViewModel.listEzeeMoneyPayoutDetails = db.EzeeMoneyPayoutDetail.Where(p => p.EzeeMoneyPayoutID == objEzeeMoneyPayout.Id).ToList();
                objERPBonusPayoutViewModel.EzeeMoneyPayoutId = objEzeeMoneyPayout.Id;
                ViewBag.EzMnyPayout = objEzeeMoneyPayout.FromDate.ToString("dd/MMM/yyyy") + " To " + objEzeeMoneyPayout.ToDate.ToString("dd/MMM/yyyy");
            }



            return View(objERPBonusPayoutViewModel);
        }

        [HttpPost]
        public ActionResult Index(ERPBonusPayoutViewModel objERPBonusPayoutViewModel)
        {
            return RedirectToAction("PromotionalERPPayoutUserList", objERPBonusPayoutViewModel);
        }

        public ActionResult PromotionalERPPayoutUserList(ERPBonusPayoutViewModel objERPBonusPayoutViewModel)
        {
            List<PromotionalERPPayoutUserList> listPromotionalERPPayoutUserList = new List<PromotionalERPPayoutUserList>();
            if (objERPBonusPayoutViewModel.Level == 0)
            {
                listPromotionalERPPayoutUserList = db.EzeeMoneyPayoutDetail.Where(p => p.EzeeMoneyPayoutID == objERPBonusPayoutViewModel.EzeeMoneyPayoutId).ToList()
                    .Select(p => new PromotionalERPPayoutUserList
                    {
                        DelOrdCount = p.DelOrdCount,
                        EmailId = p.EmailId,
                        ERP = p.ERP,
                        EzeeMoney = p.EzeeMoney,
                        EzeeMoneyPayoutID = p.EzeeMoneyPayoutID,
                        ID = p.ID,
                        Name = p.Name,
                        PhoneNo = p.PhoneNo,
                        PromotionalERP = (objERPBonusPayoutViewModel.ActiveUser == true) ? (p.Status == true) ? objERPBonusPayoutViewModel.ERP : 0 : objERPBonusPayoutViewModel.ERP,
                        PromotionalEzeeMoney = (objERPBonusPayoutViewModel.ActiveUser == true) ? (p.Status == true) ? objERPBonusPayoutViewModel.ERP * (decimal)db.MLMCoinRates.Where(q => q.IsActive == true).Take(1).FirstOrDefault().Rate : 0 : objERPBonusPayoutViewModel.ERP * (decimal)db.MLMCoinRates.Where(q => q.IsActive == true).Take(1).FirstOrDefault().Rate,
                        QRP = p.QRP,
                        Status = (p.Status == true) ? "Yes" : "No",
                        TotalOrdAmt = p.TotalOrdAmt,
                        TotalRetailPoints = p.TotalRetailPoints,
                        UserLoginId = p.UserLoginId
                    }).ToList();
            }
            else
            {
                listPromotionalERPPayoutUserList = GetPromotionalERPPayoutL1UserList(objERPBonusPayoutViewModel);
            }
            listPromotionalERPPayoutUserList = listPromotionalERPPayoutUserList.Where(p => p.PromotionalERP != 0).ToList();

            //Yashaswi 06/12/2018 Promo ERP
            EzeeMoneyPayout objEzeeMoneyPayout = db.EzeeMoneyPayouts.FirstOrDefault(p => p.Id == objERPBonusPayoutViewModel.EzeeMoneyPayoutId);
            if (objEzeeMoneyPayout != null)
            {
                listPromotionalERPPayoutUserList = listPromotionalERPPayoutUserList.Where(p =>
                    (db.CustomerOrders.
                    Where(co => co.CreateDate >= objEzeeMoneyPayout.FromDate &&
                    co.CreateDate <= objEzeeMoneyPayout.ToDate
                    && db.Pincodes.Where(pp => pp.CityID == objERPBonusPayoutViewModel.City)
                    .Select(pp => pp.ID).Contains(co.PincodeID))
                   .Select(co => co.UserLoginID).ToList().Contains((long)p.UserLoginId))).ToList();
            }
            //Yashaswi 06/12/2018 Promo ERP

            Session["PromotionalERPPayoutUserList"] = listPromotionalERPPayoutUserList;
            Session["Level"] = objERPBonusPayoutViewModel.Level;
            objERPBonusPayoutViewModel.Amount = objERPBonusPayoutViewModel.ERP * (decimal)db.MLMCoinRates.Where(q => q.IsActive == true).Take(1).FirstOrDefault().Rate;
            objERPBonusPayoutViewModel.listPromotionalERPPayoutUserList = listPromotionalERPPayoutUserList;
            return View(objERPBonusPayoutViewModel);
        }

        [HttpPost]
        public ActionResult SavePromotionalERPPayoutUserList(ERPBonusPayoutViewModel objERPBonusPayoutViewModel)
        {
            try
            {
                objERPBonusPayoutViewModel.listPromotionalERPPayoutUserList = null;
                List<PromotionalERPPayoutUserList> listPromotionalERPPayoutUserList = new List<ModelLayer.Models.ViewModel.PromotionalERPPayoutUserList>();
                if (objERPBonusPayoutViewModel.Level == 0)
                {
                    listPromotionalERPPayoutUserList = db.EzeeMoneyPayoutDetail.Where(p => p.EzeeMoneyPayoutID == objERPBonusPayoutViewModel.EzeeMoneyPayoutId).ToList()
                       .Select(p => new PromotionalERPPayoutUserList
                       {
                           DelOrdCount = p.DelOrdCount,
                           EmailId = p.EmailId,
                           ERP = p.ERP,
                           EzeeMoney = p.EzeeMoney,
                           EzeeMoneyPayoutID = p.EzeeMoneyPayoutID,
                           ID = p.ID,
                           Name = p.Name,
                           PhoneNo = p.PhoneNo,
                           PromotionalERP = (objERPBonusPayoutViewModel.ActiveUser == true) ? (p.Status == true) ? objERPBonusPayoutViewModel.ERP : 0 : objERPBonusPayoutViewModel.ERP,
                           PromotionalEzeeMoney = (objERPBonusPayoutViewModel.ActiveUser == true) ? (p.Status == true) ? objERPBonusPayoutViewModel.ERP * (decimal)db.MLMCoinRates.Where(q => q.IsActive == true).Take(1).FirstOrDefault().Rate : 0 : objERPBonusPayoutViewModel.ERP * (decimal)db.MLMCoinRates.Where(q => q.IsActive == true).Take(1).FirstOrDefault().Rate,
                           QRP = p.QRP,
                           Status = (p.Status == true) ? "Yes" : "No",
                           TotalOrdAmt = p.TotalOrdAmt,
                           TotalRetailPoints = p.TotalRetailPoints,
                           UserLoginId = p.UserLoginId
                       }).ToList();
                }
                else
                {
                    listPromotionalERPPayoutUserList = GetPromotionalERPPayoutL1UserList(objERPBonusPayoutViewModel);
                }
                listPromotionalERPPayoutUserList = listPromotionalERPPayoutUserList.Where(p => p.PromotionalERP != 0).ToList();
                if (listPromotionalERPPayoutUserList.Count() != 0)
                {
                    //Yashaswi 06/12/2018 Promo ERP
                    EzeeMoneyPayout objEzeeMoneyPayout = db.EzeeMoneyPayouts.FirstOrDefault(p => p.Id == objERPBonusPayoutViewModel.EzeeMoneyPayoutId);
                    if (objEzeeMoneyPayout != null)
                    {
                        listPromotionalERPPayoutUserList = listPromotionalERPPayoutUserList.Where(p =>
                            (db.CustomerOrders.
                            Where(co => co.CreateDate >= objEzeeMoneyPayout.FromDate &&
                            co.CreateDate <= objEzeeMoneyPayout.ToDate
                            && db.Pincodes.Where(pp => pp.CityID == objERPBonusPayoutViewModel.City)
                            .Select(pp => pp.ID).Contains(co.PincodeID))
                           .Select(co => co.UserLoginID).ToList().Contains((long)p.UserLoginId))).ToList();
                    }
                    //Yashaswi 06/12/2018 Promo ERP

                    EzeeloDBContext db1 = new EzeeloDBContext();
                    //For Password
                    RefferalCodeGenerator objRefferalCodeGenerator = new RefferalCodeGenerator();
                    string Characters = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    string Password = objRefferalCodeGenerator.CreateCode(5, Characters);


                    PromotionalERPPayout objPromotionalERPPayout = new PromotionalERPPayout();
                    objPromotionalERPPayout.ActiveUser = objERPBonusPayoutViewModel.ActiveUser;
                    objPromotionalERPPayout.Amount = objERPBonusPayoutViewModel.ERP * (decimal)db.MLMCoinRates.Where(q => q.IsActive == true).Take(1).FirstOrDefault().Rate;
                    objPromotionalERPPayout.ERP = objERPBonusPayoutViewModel.ERP;
                    objPromotionalERPPayout.EzeeMoneyPayoutId = objERPBonusPayoutViewModel.EzeeMoneyPayoutId;
                    objPromotionalERPPayout.FreezeBy = Convert.ToInt64(Session["ID"]);
                    objPromotionalERPPayout.FreezeDate = DateTime.Now;
                    objPromotionalERPPayout.IsPaid = false;
                    objPromotionalERPPayout.NetworkIp = CommonFunctions.GetClientIP();
                    objPromotionalERPPayout.PaidBy = null;
                    objPromotionalERPPayout.PaidDate = null;
                    objPromotionalERPPayout.ReferenceText = objERPBonusPayoutViewModel.ReferenceText;
                    objPromotionalERPPayout.TotalAmount = listPromotionalERPPayoutUserList.Sum(p => p.PromotionalEzeeMoney).Value;
                    objPromotionalERPPayout.TotalERP = listPromotionalERPPayoutUserList.Sum(p => p.PromotionalERP).Value;
                    objPromotionalERPPayout.VerficationCode = Password;
                    objPromotionalERPPayout.Level = objERPBonusPayoutViewModel.Level;
                    objPromotionalERPPayout.Cities = objERPBonusPayoutViewModel.City; //Yashaswi 06/12/2018 Promo ERP
                    db1.PromotionalERPPayouts.Add(objPromotionalERPPayout);
                    db1.SaveChanges();


                    foreach (var item in listPromotionalERPPayoutUserList)
                    {
                        PromotionalERPPayoutDetails objPromotionalERPPayoutDetails = new PromotionalERPPayoutDetails();
                        objPromotionalERPPayoutDetails.ERP = (decimal)item.PromotionalERP;
                        objPromotionalERPPayoutDetails.PromotionalERPPayoutId = objPromotionalERPPayout.Id;
                        objPromotionalERPPayoutDetails.EzeeMoney = (decimal)item.PromotionalEzeeMoney;
                        objPromotionalERPPayoutDetails.UserLoginId = (long)item.UserLoginId;
                        db1.PromotionalERPPayoutDetails.Add(objPromotionalERPPayoutDetails);
                        db1.SaveChanges();
                    }
                    SendEmail(objPromotionalERPPayout.Id, objPromotionalERPPayout.Level);
                    TempData["Result"] = "Data Freez Sussesfully";
                    return RedirectToAction("PromotionalERPPayout", new { PromotionalERPPayoutId = objPromotionalERPPayout.Id });
                }
            }
            catch
            {

            }
            return View();
        }
        public ActionResult ExportToExcel()
        {
            var gv = new GridView();
            string FileName = "";
            List<PromotionalERPPayoutUserList> listPromotionalERPPayoutUserList = new List<ModelLayer.Models.ViewModel.PromotionalERPPayoutUserList>();
            if (Session["PromotionalERPPayoutUserList"] != null)
            {
                listPromotionalERPPayoutUserList = (List<PromotionalERPPayoutUserList>)Session["PromotionalERPPayoutUserList"];
            }
            if (Convert.ToInt16(Session["Level"]) == 0)
            {
                FileName = "Promotional ERP Payout Report for Level 0 User";
                gv.DataSource = listPromotionalERPPayoutUserList.Select(p => new { p.UserLoginId, p.Name, p.EmailId, p.PhoneNo, p.Status, p.DelOrdCount, p.TotalOrdAmt, p.TotalRetailPoints, p.QRP, p.ERP, p.EzeeMoney, p.PromotionalERP, p.PromotionalEzeeMoney });
                gv.DataBind();
            }
            else
            {
                FileName = "Promotional ERP Payout Report for Level 1 User";
                gv.DataSource = listPromotionalERPPayoutUserList.Select(p => new { p.UserLoginId, p.Name, p.EmailId, p.PhoneNo, p.Status, p.DelOrdCount, p.TotalOrdAmt, p.TotalRetailPoints, p.QRP, p.ERP, p.EzeeMoney, p.PromotionalERP, p.PromotionalEzeeMoney, p.L1UserList });
                gv.DataBind();
            }
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + FileName + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);

            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("Index");
        }

        public void SendEmail(long PromotionalERPPayoutId, int Level)
        {
            try
            {
                PromotionalERPPayout objPromotionalERPPayout = db.PromotionalERPPayouts.FirstOrDefault(p => p.Id == PromotionalERPPayoutId);
                if (objPromotionalERPPayout != null)
                {
                    EzeeMoneyPayout objEzeeMoneyPayout = db.EzeeMoneyPayouts.FirstOrDefault(p => p.Id == objPromotionalERPPayout.EzeeMoneyPayoutId);
                    if (objEzeeMoneyPayout != null)
                    {
                        DateTime dt = objEzeeMoneyPayout.FromDate;
                        DateTime dt1 = objEzeeMoneyPayout.ToDate;
                        string datestr = dt.ToString("MMM") + " " + dt.ToString("yyyy") + " from " + dt.ToString("dd") + "-" + dt1.ToString("dd") + " " + dt.ToString("MMM") + " " + dt.ToString("yyyy");
                        //datestr = "Oct 2018 from 01-31 Oct 2018";
                        List<PromotionalERPPayoutDetails> listPromotionalERPPayoutDetails = db.PromotionalERPPayoutDetails.Where(p => p.PromotionalERPPayoutId == PromotionalERPPayoutId).ToList();
                        //For Url
                        string url = "http://leaders.ezeelo.com/Admin/BonusERP/GetPromotionalERPPayoutReport?PromotionalERPPayoutId=" + PromotionalERPPayoutId;

                        Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                        dictEmailValues.Add("<!--Date-->", datestr);
                        dictEmailValues.Add("<!--url-->", url);
                        dictEmailValues.Add("<!--Level-->", Level.ToString());
                        dictEmailValues.Add("<!--password-->", objPromotionalERPPayout.VerficationCode);
                        BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                        string EmailID = WebConfigurationManager.AppSettings["EmailId"];

                        gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.PROMO_ERP_PAYOUT, new string[] { EmailID, "sales@ezeelo.com" }, dictEmailValues, true);
                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DashboardController][M:SendEmail_InviteUser]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DashboardController][M:SendEmail_InviteUser]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
        }

        public ActionResult GetPromotionalERPPayoutReport(long PromotionalERPPayoutId)
        {
            List<PromotionalERPPayoutReport> list = new List<PromotionalERPPayoutReport>();
            try
            {

                var idParam = new SqlParameter
                {
                    ParameterName = "PromotionalERPPayoutId",
                    Value = PromotionalERPPayoutId
                };

                list = db.Database.SqlQuery<PromotionalERPPayoutReport>("EXEC GetPromotionalERPPayoutReport @PromotionalERPPayoutId", idParam).ToList<PromotionalERPPayoutReport>();
            }
            catch
            {
            }

            return View(list);
        }

        public ActionResult PromotionalERPPayout(long PromotionalERPPayoutId)
        {
            PromotionalERPPayout obj_PromotionalERPPayout = db.PromotionalERPPayouts.FirstOrDefault(p => p.Id == PromotionalERPPayoutId);
            EzeeMoneyPayout objEzeeMoneyPayout = db.EzeeMoneyPayouts.FirstOrDefault(p => p.Id == obj_PromotionalERPPayout.EzeeMoneyPayoutId);
            if (objEzeeMoneyPayout != null)
            {
                DateTime dt = objEzeeMoneyPayout.FromDate;
                DateTime dt1 = objEzeeMoneyPayout.ToDate;
                string datestr = dt.ToString("MMM") + " " + dt.ToString("yyyy") + " from " + dt.ToString("dd") + "-" + dt1.ToString("dd") + " " + dt.ToString("MMM") + " " + dt.ToString("yyyy");
                obj_PromotionalERPPayout.NetworkIp = datestr;
            }
            return View(obj_PromotionalERPPayout);
        }
        [HttpPost]
        public ActionResult PromotionalERPPayout(PromotionalERPPayout objPromotionalERPPayout)
        {
            PromotionalERPPayout obj_PromotionalERPPayout = db.PromotionalERPPayouts.FirstOrDefault(p => p.Id == objPromotionalERPPayout.Id);
            if (objPromotionalERPPayout.VerficationCode == obj_PromotionalERPPayout.VerficationCode)
            {
                obj_PromotionalERPPayout.PaidBy = Convert.ToInt64(Session["ID"]);
                obj_PromotionalERPPayout.PaidDate = DateTime.Now;
                obj_PromotionalERPPayout.IsPaid = true;
                obj_PromotionalERPPayout.NetworkIp = CommonFunctions.GetClientIP();
                db.SaveChanges();

                List<PromotionalERPPayoutDetails> listPromotionalERPPayoutDetails = db.PromotionalERPPayoutDetails.Where(p => p.PromotionalERPPayoutId == objPromotionalERPPayout.Id).ToList();
                foreach (var item in listPromotionalERPPayoutDetails)
                {
                    MLMWallet objMLMWallet = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == item.UserLoginId);
                    if (objMLMWallet != null)
                    {
                        objMLMWallet.Points = objMLMWallet.Points + item.ERP;
                        objMLMWallet.Amount = objMLMWallet.Amount + item.EzeeMoney;
                        objMLMWallet.LastModifyDate = DateTime.Now;
                        objMLMWallet.LastModifyBy = Convert.ToInt64(Session["ID"]);
                        db.SaveChanges();
                    }
                }
            }
            TempData["Result"] = "Payout Process Done Sussesfullt.";
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        public List<PromotionalERPPayoutUserList> GetPromotionalERPPayoutL1UserList(ERPBonusPayoutViewModel objERPBonusPayoutViewModel)
        {
            decimal PromotionalERP = objERPBonusPayoutViewModel.ERP;

            List<PromotionalERPPayoutUserList> listPromotionalERPPayoutUserList = db.EzeeMoneyPayoutDetail.Where(p => p.EzeeMoneyPayoutID == objERPBonusPayoutViewModel.EzeeMoneyPayoutId).ToList()
               .Select(p => new PromotionalERPPayoutUserList
               {
                   DelOrdCount = p.DelOrdCount,
                   EmailId = p.EmailId,
                   ERP = p.ERP,
                   EzeeMoney = p.EzeeMoney,
                   EzeeMoneyPayoutID = p.EzeeMoneyPayoutID,
                   ID = p.ID,
                   Name = p.Name,
                   PhoneNo = p.PhoneNo,
                   PromotionalERP = 0,
                   PromotionalEzeeMoney = 0,
                   QRP = p.QRP,
                   Status = (p.Status == true) ? "Yes" : "No",
                   TotalOrdAmt = p.TotalOrdAmt,
                   TotalRetailPoints = p.TotalRetailPoints,
                   UserLoginId = p.UserLoginId
               }).ToList();


            List<MLMWalletDetails> objMLMWalletDetails = db.MLMWalletDetails.Where(p => p.EzeeMoneyPayoutId == objERPBonusPayoutViewModel.EzeeMoneyPayoutId).ToList();
            foreach (var item in objMLMWalletDetails)
            {
                MLMUser objMLMUser = db.MLMUsers.FirstOrDefault(p => p.UserID == item.UserLoginId);
                if (objMLMUser != null)
                {
                    decimal CalculatedPErp;
                    string RefCode = objMLMUser.Ref_Id;
                    //Get All Level1 User
                    List<MLMUser> listMLMUser = db.MLMUsers.Where(p => p.Refered_Id_ref == RefCode).ToList();

                    //Get Only Level1 Active User
                    List<Int64> UserList = objMLMWalletDetails.Select(p => p.UserLoginId).Intersect(listMLMUser.Select(q => q.UserID)).ToList();

                    //Calulate Total Promotional ERP
                    CalculatedPErp = PromotionalERP * UserList.Count();

                    //Get Level1 Active User Name and Id
                    List<L1UserList> objL1UserList = UserList.Join(db.PersonalDetails.ToList(), p => p, q => q.UserLoginID,
                        (p, q) => new L1UserList
                        {
                            UserLoginId = q.UserLoginID,
                            Name = q.FirstName + " " + ((q.LastName == null) ? "" : q.LastName.Trim())
                        }).ToList();
                    string L1UserName = String.Join(", ", objL1UserList.Select(p => p.UserLoginId + ":" + p.Name));

                    //Update Detail in List
                    listPromotionalERPPayoutUserList.Where(S => S.UserLoginId == item.UserLoginId)
                    .Select(S =>
                    {
                        S.PromotionalERP = CalculatedPErp;
                        S.PromotionalEzeeMoney = CalculatedPErp * (decimal)db.MLMCoinRates.Where(q => q.IsActive == true).Take(1).FirstOrDefault().Rate;
                        S.L1UserList = L1UserName;
                        return S;
                    }).ToList();

                }
            }

            return listPromotionalERPPayoutUserList;

        }

        public ActionResult GetFreezReport(long PromotionalERPPayoutId)
        {

            if (PromotionalERPPayoutId != 0)
            {
                ERPBonusPayoutViewModel objERPBonusPayoutViewModel = new ERPBonusPayoutViewModel();
                PromotionalERPPayout obj_PromotionalERPPayout = db.PromotionalERPPayouts.FirstOrDefault(p => p.Id == PromotionalERPPayoutId);

                long id = obj_PromotionalERPPayout.EzeeMoneyPayoutId;
                List<PromotionalERPPayoutUserList> listPromotionalERPPayoutUserList = db.PromotionalERPPayoutDetails.Where(p => p.PromotionalERPPayoutId == PromotionalERPPayoutId)
                    .Join(db.EzeeMoneyPayoutDetail.Where(p => p.EzeeMoneyPayoutID == id), q => q.UserLoginId, p => p.UserLoginId,
                    (q, p) => new PromotionalERPPayoutUserList
                    {
                        DelOrdCount = p.DelOrdCount,
                        EmailId = p.EmailId,
                        ERP = p.ERP,
                        EzeeMoney = p.EzeeMoney,
                        EzeeMoneyPayoutID = p.EzeeMoneyPayoutID,
                        ID = p.ID,
                        Name = p.Name,
                        PhoneNo = p.PhoneNo,
                        PromotionalERP = q.ERP,
                        PromotionalEzeeMoney = q.EzeeMoney,
                        QRP = p.QRP,
                        Status = (p.Status == true) ? "Yes" : "No",
                        TotalOrdAmt = p.TotalOrdAmt,
                        TotalRetailPoints = p.TotalRetailPoints,
                        UserLoginId = p.UserLoginId
                    }).ToList();

                Session["PromotionalERPPayoutUserList"] = listPromotionalERPPayoutUserList;
                Session["Level"] = obj_PromotionalERPPayout.Level;
                objERPBonusPayoutViewModel.listPromotionalERPPayoutUserList = listPromotionalERPPayoutUserList;
                return View(objERPBonusPayoutViewModel);
            }
            else
            {
                ERPBonusPayoutViewModel objERPBonusPayoutViewModel = new ERPBonusPayoutViewModel();
                // PromotionalERPPayout obj_PromotionalERPPayout = db.PromotionalERPPayouts.FirstOrDefault(p => p.Id == PromotionalERPPayoutId);

                // long id = obj_PromotionalERPPayout.EzeeMoneyPayoutId;
                List<PromotionalERPPayoutUserList> listPromotionalERPPayoutUserList = db.PromotionalERPPayoutDetails
                    .Join(db.EzeeMoneyPayoutDetail, q => q.UserLoginId, p => p.UserLoginId,
                    (q, p) => new PromotionalERPPayoutUserList
                    {
                        DelOrdCount = p.DelOrdCount,
                        EmailId = p.EmailId,
                        ERP = p.ERP,
                        EzeeMoney = p.EzeeMoney,
                        EzeeMoneyPayoutID = p.EzeeMoneyPayoutID,
                        ID = p.ID,
                        Name = p.Name,
                        PhoneNo = p.PhoneNo,
                        PromotionalERP = q.ERP,
                        PromotionalEzeeMoney = q.EzeeMoney,
                        QRP = p.QRP,
                        Status = (p.Status == true) ? "Yes" : "No",
                        TotalOrdAmt = p.TotalOrdAmt,
                        TotalRetailPoints = p.TotalRetailPoints,
                        UserLoginId = p.UserLoginId
                    }).ToList();

                Session["PromotionalERPPayoutUserList"] = listPromotionalERPPayoutUserList;
                //Session["Level"] = obj_PromotionalERPPayout.Level;
                objERPBonusPayoutViewModel.listPromotionalERPPayoutUserList = listPromotionalERPPayoutUserList;
                return View(objERPBonusPayoutViewModel);
            }

        }

        public ActionResult Userlist()
        {
            L1UserList list = new L1UserList();
            return View();
        }

        [HttpPost]
        public ActionResult Userlist(UserListPromo list)
        {
            try
            {
                List<UserListPromo> l;
                if (Session["L1UserList"] == null)
                {
                    l = new List<UserListPromo>();
                }
                else
                {
                    l = (List<UserListPromo>)Session["L1UserList"];
                }
                if (l.Any(p => p.UserLoginId == list.UserLoginId))
                {
                    TempData["Result"] = "UserLoginId Already Exist!";
                    return View();
                }
                else
                {
                    if (db.PersonalDetails.Any(p => p.UserLoginID == list.UserLoginId))
                    {
                        UserListPromo Userlist = new UserListPromo();
                        Userlist.UserLoginId = list.UserLoginId;
                        Userlist.EzzeMoney = list.EzzeMoney;
                        Userlist.Name = db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == list.UserLoginId).FirstName + ' ' + db.PersonalDetails.FirstOrDefault(p => p.UserLoginID == list.UserLoginId).LastName;
                        l.Add(Userlist);
                        Session["L1UserList"] = l;
                    }
                    else
                    {
                        TempData["Result"] = "Invalid Userlogin Id";
                        return View();
                    }
                }
                TempData["Result"] = "Add Successfully!";
                return View();
            }
            catch (Exception ex)
            {
                TempData["Result"] = "Something Went wrong!" + ex.Message;
                return View();

            }
            //return RedirectToAction("ShowUserList");
        }

        public ActionResult ShowUserList()
        {
            UserExtraPay obj = new UserExtraPay();
            List<UserListPromo> l = new List<UserListPromo>();
            if (Session["L1UserList"] == null)
            {
                l = new List<UserListPromo>();
            }
            else
            {
                l = (List<UserListPromo>)Session["L1UserList"];
            }
            obj.list = l;
            return View(obj);
        }

        [HttpPost]
        public ActionResult ShowUserList(UserExtraPay obj)
        {
            List<UserListPromo> l = new List<UserListPromo>();
            if (Session["L1UserList"] == null)
            {
                l = new List<UserListPromo>();
            }
            else
            {
                l = (List<UserListPromo>)Session["L1UserList"];
            }

            if (obj.ReferenceText == "" || obj.ReferenceText == null)
            {
                obj.list = l;
                TempData["Result"] = "Please add Reference Text!!!";
                return View(obj);
            }
            PromotionalERPPayout obj_PromotionalERPPayout = new PromotionalERPPayout();
            obj_PromotionalERPPayout.ActiveUser = true;
            obj_PromotionalERPPayout.Level = 0;
            obj_PromotionalERPPayout.EzeeMoneyPayoutId = 0;
            obj_PromotionalERPPayout.Amount = 0;
            obj_PromotionalERPPayout.ERP = 0;
            obj_PromotionalERPPayout.TotalERP = 0;
            obj_PromotionalERPPayout.TotalAmount = 0;
            obj_PromotionalERPPayout.ReferenceText = obj.ReferenceText;
            obj_PromotionalERPPayout.FreezeBy = Convert.ToInt64(Session["ID"]);
            obj_PromotionalERPPayout.FreezeDate = DateTime.Now;
            obj_PromotionalERPPayout.IsPaid = true;
            obj_PromotionalERPPayout.NetworkIp = CommonFunctions.GetClientIP();
            obj_PromotionalERPPayout.PaidBy = Convert.ToInt64(Session["ID"]);
            obj_PromotionalERPPayout.PaidDate = DateTime.Now;
            obj_PromotionalERPPayout.NetworkIp = CommonFunctions.GetClientIP();
            db.PromotionalERPPayouts.Add(obj_PromotionalERPPayout);
            db.SaveChanges();

            foreach (var item in l)
            {
                PromotionalERPPayoutDetails objPromotionalERPPayoutDetails = new PromotionalERPPayoutDetails();
                objPromotionalERPPayoutDetails.ERP = (item.EzzeMoney * 10);
                objPromotionalERPPayoutDetails.PromotionalERPPayoutId = obj_PromotionalERPPayout.Id;
                objPromotionalERPPayoutDetails.EzeeMoney = item.EzzeMoney;
                objPromotionalERPPayoutDetails.UserLoginId = item.UserLoginId;
                db.PromotionalERPPayoutDetails.Add(objPromotionalERPPayoutDetails);
                db.SaveChanges();
            }
            foreach (var item in l)
            {
                MLMWallet objMLMWallet = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == item.UserLoginId);
                if (objMLMWallet != null)
                {
                    objMLMWallet.Points = objMLMWallet.Points + (item.EzzeMoney * 10);
                    objMLMWallet.Amount = objMLMWallet.Amount + item.EzzeMoney;
                    objMLMWallet.LastModifyDate = DateTime.Now;
                    objMLMWallet.LastModifyBy = Convert.ToInt64(Session["ID"]);
                    db.SaveChanges();
                }
            }
            obj.list = l;
            TempData["Result"] = "Paid Successfully!";
            Session["L1UserList"] = null;

            return RedirectToAction("Userlist"); ;
        }

        [HttpGet]
        public ActionResult ImportUserList()
        {
            List<EzeeMoneyPayoutDetails> Userlist = new List<EzeeMoneyPayoutDetails>();
            return View(Userlist);
        }

        [HttpPost]
        public ActionResult ImportUserList(HttpPostedFileBase file)
        {
            List<EzeeMoneyPayoutDetails> Userlist = new List<EzeeMoneyPayoutDetails>();
            try
            {
                DataSet ds = new DataSet();
                if (Request.Files["file"].ContentLength > 0)
                {
                    string fileExtension = Path.GetExtension(Request.Files["file"].FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }
                        Request.Files["file"].SaveAs(fileLocation);
                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        //connection String for xls file format.
                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        //connection String for xlsx file format.
                        else if (fileExtension == ".xlsx")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
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
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(ds);
                        }
                        excelConnection.Close();
                        excelConnection.Dispose();
                    }
                    if (ds.Tables.Count != 0)
                    {
                        DataTable UserListdt = ds.Tables[0];

                        var list = (from DataRow dr in UserListdt.Rows
                                    select new
                                    {
                                        UserLoginId = Convert.ToInt64(dr["UserLoginId"]),
                                        EzeeMoney = Convert.ToDecimal(dr["EzeeMoney"])
                                    }).ToList();
                        list = list.GroupBy(l => l.UserLoginId).Select(g => g.First()).ToList();


                        Userlist = list.Join(db.PersonalDetails, l => l.UserLoginId, p => p.UserLoginID, (l, p) => new
                        {
                            Name = p.FirstName + " " + p.MiddleName ?? "" + " " + p.LastName ?? "",
                            UserloginId = l.UserLoginId,
                            EzeeMoney = l.EzeeMoney
                        }).ToList()
                         .Join(db.UserLogins, p => p.UserloginId, u => u.ID, (p, u) => new EzeeMoneyPayoutDetails
                         {
                             UserLoginId = p.UserloginId,
                             Name = p.Name,
                             EzeeMoney = p.EzeeMoney,
                             EmailId = u.Email,
                             PhoneNo = u.Mobile
                         }).ToList();
                    }
                }
                Session["UserWisePayoutList"] = Userlist;
            }
            catch (Exception ex)
            {
                ViewBag.Msg = "Failed to load List from execl sheet. Please make sure column name UserLoginId and EzeeMoney are present. " + ex.Message;
            }
            return View(Userlist);
        }

        [HttpPost]
        public ActionResult UserwisePayout(string RefText)
        {
            string msg = "";
            if (Session["UserWisePayoutList"] != null)
            {
                List<EzeeMoneyPayoutDetails> Userlist = Session["UserWisePayoutList"] as List<EzeeMoneyPayoutDetails>;
                if (Userlist.Count == 0)
                {
                    msg = "No Record found for payout!!!";
                }
                else
                {
                    using (TransactionScope tscope = new TransactionScope())
                    {
                        try
                        {
                            PromotionalERPPayout obj_PromotionalERPPayout = new PromotionalERPPayout();
                            obj_PromotionalERPPayout.ActiveUser = true;
                            obj_PromotionalERPPayout.Level = 0;
                            obj_PromotionalERPPayout.EzeeMoneyPayoutId = 0;
                            obj_PromotionalERPPayout.Amount = Userlist.Sum(p => p.EzeeMoney);
                            obj_PromotionalERPPayout.ERP = Userlist.Sum(p => p.ERP);
                            obj_PromotionalERPPayout.TotalERP = Userlist.Sum(p => p.ERP);
                            obj_PromotionalERPPayout.TotalAmount = Userlist.Sum(p => p.EzeeMoney);
                            obj_PromotionalERPPayout.ReferenceText = RefText;
                            obj_PromotionalERPPayout.FreezeBy = Convert.ToInt64(Session["ID"]);
                            obj_PromotionalERPPayout.FreezeDate = DateTime.Now;
                            obj_PromotionalERPPayout.IsPaid = true;
                            obj_PromotionalERPPayout.NetworkIp = CommonFunctions.GetClientIP();
                            obj_PromotionalERPPayout.PaidBy = Convert.ToInt64(Session["ID"]);
                            obj_PromotionalERPPayout.PaidDate = DateTime.Now;
                            obj_PromotionalERPPayout.NetworkIp = CommonFunctions.GetClientIP();
                            db.PromotionalERPPayouts.Add(obj_PromotionalERPPayout);
                            db.SaveChanges();
                            decimal point = Convert.ToDecimal(db.MLMCoinRates.Select(p => p.Rate).FirstOrDefault() * 100);
                            foreach (var item in Userlist)
                            {
                                PromotionalERPPayoutDetails objPromotionalERPPayoutDetails = new PromotionalERPPayoutDetails();
                                objPromotionalERPPayoutDetails.ERP = (item.EzeeMoney * point);
                                objPromotionalERPPayoutDetails.PromotionalERPPayoutId = obj_PromotionalERPPayout.Id;
                                objPromotionalERPPayoutDetails.EzeeMoney = item.EzeeMoney;
                                objPromotionalERPPayoutDetails.UserLoginId = item.UserLoginId;
                                db.PromotionalERPPayoutDetails.Add(objPromotionalERPPayoutDetails);
                                db.SaveChanges();
                            }
                            foreach (var item in Userlist)
                            {
                                MLMWallet objMLMWallet = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == item.UserLoginId);
                                if (objMLMWallet != null)
                                {
                                    objMLMWallet.Points = objMLMWallet.Points + (item.EzeeMoney * point);
                                    objMLMWallet.Amount = objMLMWallet.Amount + item.EzeeMoney;
                                    objMLMWallet.LastModifyDate = DateTime.Now;
                                    objMLMWallet.LastModifyBy = Convert.ToInt64(Session["ID"]);
                                    db.SaveChanges();
                                }
                            }
                            tscope.Complete();
                            msg = "Userwise Payout done succefully!!";
                        }
                        catch (Exception ex)
                        {
                            tscope.Dispose();
                            msg = "Transcation failed. Please try after some time. " + ex.Message;
                        }
                    }
                }
            }
            else
            {
                msg = "Session expired. Please try again.";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
    }
}