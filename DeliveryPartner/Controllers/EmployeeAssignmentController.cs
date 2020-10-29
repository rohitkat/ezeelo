using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using PagedList;
using PagedList.Mvc;
using DeliveryPartner.Models.ViewModel;
using System.Collections;
using DeliveryPartner.Models;
using BusinessLogicLayer;
using System.Transactions;



namespace DeliveryPartner.Controllers
{
   
    public class EmployeeAssignmentController : Controller
    {
        //---------------------------------------Hide EPOD from Ashish for Live----------------------------------------------------------

        //private EzeeloDBContext db = new EzeeloDBContext();
        //private DeliveryPartnerSessionViewModel deliveryPartnerSessionViewModel = new DeliveryPartnerSessionViewModel();
        //private int pageSize = 10;

        //public void SessionDetails()
        //{
        //    deliveryPartnerSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
        //    deliveryPartnerSessionViewModel.Username = Session["UserName"].ToString();
        //    Common.Common.GetAllLoginDetailFromSession(ref deliveryPartnerSessionViewModel);
        //}
        //[SessionExpire]
        //public ActionResult Index(string FromDate, string ToDate, int? page, long? DeliveryStatus,  string SearchString = "", string DeliveryType = "", string EmployeeListSearch = "", string AssignStatus="")
        //{
           
        //    SessionDetails();
        //    int pageNumber = (page ?? 1);
        //    ViewBag.PageNumber = pageNumber;
        //    ViewBag.PageSize = pageSize;
        //    ViewBag.SearchString = SearchString;
        //    ViewBag.FromDate = FromDate;
        //    ViewBag.ToDate = ToDate;
        //    ViewBag.DeliveryStatus1 = DeliveryStatus;
        //    ViewBag.DeliveryType1 = DeliveryType;
        //    //-- For Employee Dropdown List --//
        //    TempData["FromDate"] = FromDate;
        //    TempData["ToDate"] = ToDate;
        //    TempData["page"] = pageNumber;
        //    TempData["DeliveryStatus"] = DeliveryStatus;
        //    TempData["SearchString"] = SearchString;
        //    TempData["DeliveryType"] = DeliveryType;
        //    //-- For Search Employee and Assign/Unassign Dropdown List --//
        //    TempData["EmployeeCode"] = EmployeeListSearch;
        //    TempData["AssignStatus"] = AssignStatus;
        //    ViewBag.EmployeeCode1 = EmployeeListSearch;
        //    ViewBag.AssignStatus1 = AssignStatus;

        //    TempData.Keep();
        //    string UserLoginID = Convert.ToInt64(Session["USER_LOGIN_ID"]).ToString();
        //    long MerchantID = getOwnerIDUsingSession(CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["USER_LOGIN_ID"])));
        //    var lst = (from e in db.Employees
        //               join pd in db.PersonalDetails on e.UserLoginID equals pd.UserLoginID
        //               join ul in db.UserLogins on pd.UserLoginID equals ul.ID//added
        //               join ur in db.UserRoles on ul.ID equals ur.UserLoginID //added
        //               join dba in db.DeliveryBoyAttendances on e.UserLoginID equals dba.UserLoginID into ed
        //               from dba in ed.DefaultIfEmpty()//-- For Left Outer Join --//
        //               where (e.OwnerID == MerchantID && e.EmployeeCode.Substring(0, 4) == "GBDP" && ur.RoleID == 10) //RoleID==10 for "DELIVERY_EMPLOYEE" Role
        //               && e.IsActive == true && pd.IsActive == true && ur.IsActive == true && ul.IsLocked == false //added
        //               && (dba.LoginDateTime == (from dbt in db.DeliveryBoyAttendances where dbt.UserLoginID == e.UserLoginID select dbt.LoginDateTime).Max() //.Take(1) 
        //                 || dba.LoginDateTime == null //Assigning Task for both Login & Logout employee for tracking record in EmployeeAssignment table
        //                //&& dba.LogoutDateTime==null //This for only Login employee
        //                 )
        //               select new //Employee hide
        //               {
        //                   // LoginDateTime=dba.LoginDateTime, //for testing
        //                   ID = e.ID,
        //                   EmployeeCode = e.EmployeeCode,
        //                   OwnerID = e.OwnerID,
        //                   IsActive = e.IsActive,
        //                   EmployeeName = pd.FirstName +" "+pd.LastName,//using DeviceID string as EmployeeName
        //                   ////DeviceType = dba.LoginDateTime == null ? "Logout" : dba.LogoutDateTime == null ? "Login" : "Logout"
        //                   LoginStatus= dba.LoginDateTime == null ? "Logout" : dba.LogoutDateTime == null ? "Login" : "Logout"

        //                   //AssignStatus = e.EmployeeCode
        //               }).AsEnumerable().Select(x => new EmployeeAssignmentViewModel //Hide Employee //-- For converting LinQ to IEnumerable --//
        //               {
        //                   // ModifyDate=x.LoginDateTime,
        //                   ID = x.ID,
        //                   EmployeeCode = x.EmployeeCode,
        //                   OwnerID = x.OwnerID,
        //                   IsActive = x.IsActive,
        //                  //// DeviceID = x.EmployeeName.ToString(),//using DeviceID string as EmployeeName
        //                  //// DeviceType = x.DeviceType //using DeviceType string for "Login/Logout" string
        //                   EmployeeName = x.EmployeeName.ToString(),
        //                   LoginStatus = x.LoginStatus
        //                  // AssignStatus = x.AssignStatus
        //               }).ToList();
        //   //// ViewBag.EmployeeList = new SelectList(lst, "EmployeeCode", "DeviceID");
        //    ViewBag.EmployeeList = new SelectList(lst, "EmployeeCode", "EmployeeName");
        //    ViewBag.EmployeeListSearch = new SelectList(lst, "EmployeeCode", "EmployeeName");
        //    List<AssignStatusViewModel> AssignStatusViewModel = new List<AssignStatusViewModel>();
        //    AssignStatusViewModel.Add(new AssignStatusViewModel { ID = 1, Name = "ASSIGN" });
        //    //AssignStatusViewModel.Add(new AssignStatusViewModel { ID = 2, Name = "ASSIGN TO: GODOWN" });
        //    //AssignStatusViewModel.Add(new AssignStatusViewModel { ID = 3, Name = "ASSIGN TO: CUSTOMER" });
        //    AssignStatusViewModel.Add(new AssignStatusViewModel { ID = 2, Name = "ASSIGN FOR: PICKUP" });
        //    AssignStatusViewModel.Add(new AssignStatusViewModel { ID = 3, Name = "ASSIGN FOR: DELIVERY" });
        //    AssignStatusViewModel.Add(new AssignStatusViewModel { ID = 4, Name = "UNASSIGN" });
        //    ViewBag.AssignStatus = new SelectList(AssignStatusViewModel, "Name", "Name");
        //    //-- End of Employee Dropdown List --//
        //    List<DeliveryTypeViewModel> DeliveryTypeViewModels = new List<DeliveryTypeViewModel>();
        //    DeliveryTypeViewModels.Add(new DeliveryTypeViewModel { ID = 1, Name = "NORMAL" });
        //    DeliveryTypeViewModels.Add(new DeliveryTypeViewModel { ID = 2, Name = "EXPRESS" });
        //    ViewBag.DeliveryType = new SelectList(DeliveryTypeViewModels, "Name", "Name");

        //    var Status = from DeliveryPartner.Common.Constant.ORDER_STATUS_ASSIGN d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.ORDER_STATUS_ASSIGN))
        //                 select new { ID = (int)d, Name = d.ToString() };

        //    ViewBag.DeliveryStatus = new SelectList(Status.Where(x => x.ID >= (int)Common.Constant.ORDER_STATUS_ASSIGN.PACKED), "ID", "Name");

        //   var DeliverTo = from DeliveryPartner.Common.Constant.DELIVERY_TO d in Enum.GetValues(typeof(DeliveryPartner.Common.Constant.DELIVERY_TO))
        //                 select new { ID = (int)d, Name = d.ToString() };
        //   //ViewBag.DeliverTo = new SelectList(DeliverTo.Where(x => x.ID >= (int)Common.Constant.DELIVERY_TO.IN_GODOWN), "ID", "Name");
        //   ViewBag.DeliverTo = new SelectList(DeliverTo.Where(x => x.ID >= (int)Common.Constant.DELIVERY_TO.PICKUP), "ID", "Name");


        //   var deliveryorderdetails = (from DOD in db.DeliveryOrderDetails
        //                               join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                               join EA in db.EmployeeAssignment on DOD.ShopOrderCode equals EA.ShopOrderCode into LOJ
        //                               from EA in LOJ.DefaultIfEmpty()//-- For Left Outer Join --//
        //                               where DOD.ShopOrderCode == COD.ShopOrderCode &&
        //                               COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.PACKED &&
        //                               //COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_GODOWN && //hide
        //                               COD.OrderStatus <=7 &&
        //                               DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID
        //                               select new DeliveryIndexViewModel
        //                               {
        //                                   ID = DOD.ID,
        //                                   DeliveryPartnerID = DOD.DeliveryPartnerID,
        //                                   GandhibaghOrderID = COD.CustomerOrderID,//Added by Mohit 19-10-15
        //                                   GandhibaghOrderCode = COD.CustomerOrder.OrderCode,
        //                                   ShopOrderCode = DOD.ShopOrderCode,
        //                                   Weight = DOD.Weight,
        //                                   OrderAmount = DOD.OrderAmount,
        //                                   DeliveryCharge = DOD.DeliveryCharge,
        //                                   GandhibaghCharge = DOD.GandhibaghCharge,
        //                                   DeliveryType = DOD.DeliveryType,
        //                                   IsMyPincode = DOD.IsMyPincode,
        //                                   IsActive = DOD.IsActive,
        //                                   CreateDate = DOD.CreateDate,
        //                                   CreateBy = DOD.CreateBy,
        //                                   ModifyDate = DOD.ModifyDate,
        //                                   ModifyBy = DOD.ModifyBy,
        //                                   NetworkIP = DOD.NetworkIP,
        //                                   DeviceType = DOD.DeviceType,
        //                                   DeviceID = DOD.DeviceID,


        //                                   //----------------------------- Extra added -//
        //                                   OrderStatus = COD.OrderStatus,
        //                                   DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                   DeliveryScheduleName = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                   //EA.ShopOrderCode to EA.EmployeeCode
        //                                   EmployeeCode = EA.EmployeeCode == null ? "" : EA.EmployeeCode,
        //                                   DeliveredType=EA.DeliveredType,
        //                                   Assignment = EA.EmployeeCode == null ? "UNASSIGN" : "ASSIGN TO: " + (from E in db.Employees
        //                                                                                                        join PD in db.PersonalDetails on E.UserLoginID equals PD.UserLoginID
        //                                                                                                        where E.EmployeeCode == EA.EmployeeCode
        //                                                                                                     select new { Empname = PD.FirstName + " " + PD.LastName }).FirstOrDefault().Empname
        //                                   //GetPersonalDetails(EA.EmployeeCode)
        //                                  // Assignment = 
        //                                   //this.GetPersonalDetails(EA.EmployeeCode).ToString() 
        //                               }).Distinct().OrderByDescending(x => x.ID).ToList();

        //    //-----------------Added By Mohit----On 19-10-15---------------------------------//
        //    List<SubscriptionPlanUsedBy> lSubscriptionPlanUsedBies = new List<SubscriptionPlanUsedBy>();
        //    lSubscriptionPlanUsedBies = db.SubscriptionPlanUsedBies.ToList();

        //    foreach (DeliveryIndexViewModel lDeliveryIndexViewModel in deliveryorderdetails)
        //    {

        //        SubscriptionPlanUsedBy lSubscriptionPlanUsedBy = lSubscriptionPlanUsedBies.FirstOrDefault(x => x.CustomerOrderID == lDeliveryIndexViewModel.GandhibaghOrderID);

        //        if (lSubscriptionPlanUsedBy != null)
        //        {
        //            lDeliveryIndexViewModel.GandhibaghOrderCode = lDeliveryIndexViewModel.GandhibaghOrderCode + "*";
        //        }

        //    }
        //    //-----------------End of Code By Mohit----On 19-10-15---------------------------------//
        //    //var deliveryorderdetails = db.DeliveryOrderDetails.Include(d => d.DeliveryPartner).Include(d => d.PersonalDetail).Include(d => d.PersonalDetail1).ToList().Where(x => x.DeliveryPartnerID == fUserId);
        //    if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
        //    {
        //        //DateTime lFromDate = DateTime.Now;
        //        //if (DateTime.TryParse(FromDate, out lFromDate)) { }

        //        //DateTime lToDate = DateTime.Now;
        //        //if (DateTime.TryParse(ToDate, out lToDate)) { }

        //        DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
        //        DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);

        //        deliveryorderdetails = deliveryorderdetails.Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).ToList();
        //    }
        //    SearchString = SearchString.Trim();
        //    if (SearchString != "")
        //    {
        //        //deliveryorderdetails = deliveryorderdetails.Where(x => x.ShopOrderCode.ToString().Contains(SearchString)).ToList();
        //        deliveryorderdetails = deliveryorderdetails.Where(x => x.ShopOrderCode.ToString().Contains(SearchString) || x.GandhibaghOrderCode.ToString().Contains(SearchString)).ToList();
        //    }
        //    if (EmployeeListSearch != "")
        //    {
        //        deliveryorderdetails = deliveryorderdetails.Where(x => x.EmployeeCode.ToString() == EmployeeListSearch).ToList();
        //    }
        //    if (AssignStatus != "")
        //    {
        //        if (AssignStatus == "ASSIGN FOR: PICKUP")
        //            deliveryorderdetails = deliveryorderdetails.Where(x => x.Assignment.ToString() != "UNASSIGN" && x.DeliveredType.ToString() == "PICKUP").ToList();
        //        else if (AssignStatus == "ASSIGN FOR: DELIVERY")
        //            deliveryorderdetails = deliveryorderdetails.Where(x => x.Assignment.ToString() != "UNASSIGN" && x.DeliveredType.ToString() == "DELIVERY").ToList();
        //        else if (AssignStatus == "UNASSIGN")
        //            deliveryorderdetails = deliveryorderdetails.Where(x => x.Assignment.ToString() == AssignStatus).ToList();
        //        else 
        //            deliveryorderdetails = deliveryorderdetails.Where(x => x.Assignment.ToString() != "UNASSIGN").ToList();
        //    }
        //    if (DeliveryType != "")
        //    {
        //        deliveryorderdetails = deliveryorderdetails.Where(x => x.DeliveryType.ToUpper().ToString() == DeliveryType.ToUpper()).ToList();
        //    }
        //    if (DeliveryStatus != null)
        //    {
        //        deliveryorderdetails = deliveryorderdetails.Where(x => x.OrderStatus == DeliveryStatus).ToList();
        //    }
        //    else //Add by Ashish
        //    {
        //        deliveryorderdetails = deliveryorderdetails.Where(x => x.OrderStatus >= 3 && x.OrderStatus<7).ToList();
        //    }



        //    return View(deliveryorderdetails.ToPagedList(pageNumber, pageSize));
        //}
        
        //[HttpPost]
        //[SessionExpire]
        //public ActionResult Assign(string EmployeeName, string DeliverTo, string ShopOrderCode)
        //{
        //    string FromDate1 = TempData["FromDate"] != null?TempData["FromDate"].ToString():null;
        //    string ToDate1 = TempData["ToDate"] != null ? TempData["ToDate"].ToString() : null;
        //    string SearchString1 = TempData["SearchString"] != null ? TempData["SearchString"].ToString() : "";
        //    string DeliveryType1 = TempData["DeliveryType"] != null ? TempData["DeliveryType"].ToString() : "";
        //    int? page1 = TempData["page"] != null ? (int)TempData["page"] : 1;
        //    long? DeliveryStatus1 = TempData["DeliveryStatus"] != null ? (long)TempData["DeliveryStatus"] : DeliveryStatus1 = null;
        //    string EmployeeCode1 = TempData["EmployeeCode"] != null ? TempData["EmployeeCode"].ToString() : "";
        //    string AssignStatus1 = TempData["AssignStatus"] != null ? TempData["AssignStatus"].ToString() : "";
        //    TempData.Keep();

        //    long UserLoginID = Convert.ToInt64(Session["USER_LOGIN_ID"]);
        //    var IsTableEmpty = from ea in db.EmployeeAssignment select ea.ID;
        //    //List<DeliveryTypeViewModel> DeliveryTypeViewModels = new List<DeliveryTypeViewModel>();
        //    string NewShopOrderCode = "";
        //    string UpdateOldShopOrderCode = "";
        //    int count = 0;
        //    if (DeliverTo == "PICKUP")
        //    {
        //       //-- Check For without Empty Table --//
        //        if (IsTableEmpty.Count() > 0)
        //        {
        //            var IsContain = (from EA in db.EmployeeAssignment
        //                             where (EA.DeliveredType == "DELIVERY" || EA.DeliveredType == "PICKUP") && EA.OrderStatus >= 3 && EA.OrderStatus <= 6 
        //                             && ShopOrderCode.Contains(EA.ShopOrderCode)  
        //                             select EA.ShopOrderCode
        //                               ).ToList();
        //            var SOC_Count=ShopOrderCode.Split(',');
        //            //-- For checking selected item present in EmployeeAssignment --//
        //            if (IsContain.Count() > 0 )
        //            {
        //                if (IsContain.Count() < SOC_Count.Count())
        //                {
        //                    for (int i = 0; i < SOC_Count.Count(); i++)
        //                    {
        //                        count = 0;
        //                        for (int j = 0; j < IsContain.Count(); j++)
        //                        {
                                    
        //                            if (SOC_Count[i] == IsContain[j])
        //                            {
        //                                UpdateOldShopOrderCode += SOC_Count[i] + ',';
        //                                count++;
        //                            }
        //                            else if (j == IsContain.Count()-1 && count==0)
        //                            {

        //                                NewShopOrderCode += SOC_Count[i] + ',';
        //                            }
        //                        }
        //                    }
        //                    if (NewShopOrderCode != "")
        //                    {
        //                       /* using (TransactionScope ts = new TransactionScope())
        //                        {
        //                            try
        //                            {*/
        //                                NewShopOrderCode = NewShopOrderCode.Substring(0, NewShopOrderCode.Count() - 1);
        //                                //--------- Add New Task List To Employee For PICKUP ---------//
        //                                GodownAssignInsert(EmployeeName, DeliverTo, NewShopOrderCode, UserLoginID);
        //                                //------------------------------------------------------------------//
        //                           /*     ts.Complete();
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                  //Incase of Insertion fail Message to be Display
        //                                  ViewBag.Message = "Sorry! Problem in Inserting Record!!";
        //                                //RollBack All Transaction
        //                                ts.Dispose();

        //                                ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine
        //                                    , ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
        //                            }
        //                        }*/
        //                    }
        //                    if (UpdateOldShopOrderCode != "")
        //                    {
        //                      /*  using (TransactionScope ts = new TransactionScope())
        //                        {
        //                            try
        //                            {*/
        //                                UpdateOldShopOrderCode = UpdateOldShopOrderCode.Substring(0, UpdateOldShopOrderCode.Count() - 1);
        //                                //--------- Update Task List To Employee For PICKUP ---------//
        //                                GodownAssignUpdate(EmployeeName, DeliverTo, UpdateOldShopOrderCode, UserLoginID);
        //                                //------------------------------------------------------------------//
        //                           /*     ts.Complete();
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                //Incase of Insertion fail Message to be Display
        //                                ViewBag.Message = "Sorry! Problem in Inserting Record!!";
        //                                //RollBack All Transaction
        //                                ts.Dispose();

        //                                ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine
        //                                    , ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
        //                            }
        //                        }*/
        //                    }
        //                }
        //                else//-- If selected item count equals to item present in EmployeeAssignment then Update it --//
        //                {
        //                   /* using (TransactionScope ts = new TransactionScope())
        //                    {
        //                        try
        //                        {*/
        //                            //--------- Update Task List To Employee For PICKUP ---------//
        //                            GodownAssignUpdate(EmployeeName, DeliverTo, ShopOrderCode, UserLoginID);
        //                            //------------------------------------------------------------------//
        //                       /*     ts.Complete();
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            //Incase of Insertion fail Message to be Display
        //                            ViewBag.Message = "Sorry! Problem in Inserting Record!!";
        //                            //RollBack All Transaction
        //                            ts.Dispose();

        //                            ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine
        //                                , ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
        //                        }
        //                    }*/ 
        //                }
        //            }
        //            else//-- For New items selected --//
        //            {
        //               /* using (TransactionScope ts = new TransactionScope())
        //                {
        //                    try
        //                    {*/
        //                        //--------- Add New Task List To Employee For PICKUP ---------//
        //                        GodownAssignInsert(EmployeeName, DeliverTo, ShopOrderCode, UserLoginID);
        //                        //------------------------------------------------------------------//
        //                   /*     ts.Complete();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        //Incase of Insertion fail Message to be Display
        //                        ViewBag.Message = "Sorry! Problem in Inserting Record!!";
        //                        //RollBack All Transaction
        //                        ts.Dispose();

        //                        ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine
        //                            , ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
        //                    }
        //                }*/
        //            }
        //         }
        //        else  //-- For Empty Table --//
        //        {
        //           /* using (TransactionScope ts = new TransactionScope())
        //            {
        //                try
        //                {*/
        //                    //--------- Add New Task List To Employee For PICKUP ---------//
        //                    GodownAssignInsert(EmployeeName, DeliverTo, ShopOrderCode, UserLoginID);
        //                    //------------------------------------------------------------------
        //              /*      ts.Complete();
        //                }
        //                catch (Exception ex)
        //                {
        //                    //Incase of Insertion fail Message to be Display
        //                    ViewBag.Message = "Sorry! Problem in Inserting Record!!";
        //                    //RollBack All Transaction
        //                    ts.Dispose();

        //                    ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine
        //                        , ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
        //                }
        //            }*/
        //        }
        //    }
        //    else if (DeliverTo == "DELIVERY")
        //    {
        //        //-- Check For without Empty Table --//
        //        if (IsTableEmpty.Count() > 0)
        //        {
        //            var IsContain = (from EA in db.EmployeeAssignment
        //                             where (EA.DeliveredType == "DELIVERY" || EA.DeliveredType == "PICKUP") && EA.OrderStatus >= 3 && EA.OrderStatus <= 6
        //                             && ShopOrderCode.Contains(EA.ShopOrderCode)
        //                             select EA.ShopOrderCode
        //                               ).ToList();
        //            var SOC_Count = ShopOrderCode.Split(',');
        //            //-- For checking selected item present in EmployeeAssignment --//
        //            if (IsContain.Count() > 0)
        //            {
        //                if (IsContain.Count() < SOC_Count.Count())
        //                {
        //                    for (int i = 0; i < SOC_Count.Count(); i++)
        //                    {
        //                        count = 0;
        //                        for (int j = 0; j < IsContain.Count(); j++)
        //                        {

        //                            if (SOC_Count[i] == IsContain[j])
        //                            {
        //                                UpdateOldShopOrderCode += SOC_Count[i] + ',';
        //                                count++;
        //                            }
        //                            else if (j == IsContain.Count() - 1 && count == 0)
        //                            {

        //                                NewShopOrderCode += SOC_Count[i] + ',';
        //                            }
        //                        }
        //                    }
        //                    if (NewShopOrderCode != "")
        //                    {
        //                        /*using (TransactionScope ts = new TransactionScope())
        //                        {
        //                            try
        //                            {*/
        //                                NewShopOrderCode = NewShopOrderCode.Substring(0, NewShopOrderCode.Count() - 1);
        //                                //--------- Add New Task List To Employee For PICKUP ---------//
        //                                CustomerAssignInsert(EmployeeName, DeliverTo, NewShopOrderCode, UserLoginID);
        //                                //------------------------------------------------------------------//
        //                          /*      ts.Complete();
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                //Incase of Insertion fail Message to be Display
        //                                ViewBag.Message = "Sorry! Problem in Inserting Record!!";
        //                                //RollBack All Transaction
        //                                ts.Dispose();

        //                                ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine
        //                                    , ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
        //                            }
        //                        }*/
        //                    }
        //                    if (UpdateOldShopOrderCode != "")
        //                    {
        //                        /*using (TransactionScope ts = new TransactionScope())
        //                        {
        //                            try
        //                            {*/
        //                                UpdateOldShopOrderCode = UpdateOldShopOrderCode.Substring(0, UpdateOldShopOrderCode.Count() - 1);
        //                                //--------- Update Task List To Employee For PICKUP ---------//
        //                                CustomerAssignUpdate(EmployeeName, DeliverTo, UpdateOldShopOrderCode, UserLoginID);
        //                                //------------------------------------------------------------------//
        //                           /*     ts.Complete();
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                //Incase of Insertion fail Message to be Display
        //                                ViewBag.Message = "Sorry! Problem in Inserting Record!!";
        //                                //RollBack All Transaction
        //                                ts.Dispose();

        //                                ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine
        //                                    , ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
        //                            }
        //                        }*/
        //                    }
        //                }
        //                else//-- If selected item count equals to item present in EmployeeAssignment then Update it --//
        //                {
        //                   /* using (TransactionScope ts = new TransactionScope())
        //                    {
        //                        try
        //                        {*/
        //                            //--------- Update Task List To Employee For PICKUP ---------//
        //                            CustomerAssignUpdate(EmployeeName, DeliverTo, ShopOrderCode, UserLoginID);
        //                            //------------------------------------------------------------------//
        //                        /*    ts.Complete();
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            //Incase of Insertion fail Message to be Display
        //                            ViewBag.Message = "Sorry! Problem in Inserting Record!!";
        //                            //RollBack All Transaction
        //                            ts.Dispose();

        //                            ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine
        //                                , ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
        //                        }
        //                    }*/
        //                }
        //            }
        //            else//-- For New items selected --//
        //            {
        //                /*using (TransactionScope ts = new TransactionScope())
        //                {
        //                    try
        //                    {*/
        //                        //--------- Add New Task List To Employee For PICKUP ---------//
        //                        CustomerAssignInsert(EmployeeName, DeliverTo, ShopOrderCode, UserLoginID);
        //                        //------------------------------------------------------------------//
        //                  /*      ts.Complete();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        //Incase of Insertion fail Message to be Display
        //                        ViewBag.Message = "Sorry! Problem in Inserting Record!!";
        //                        //RollBack All Transaction
        //                        ts.Dispose();

        //                        ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine
        //                            , ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
        //                    }
        //                }*/
        //            }
        //        }
        //        else  //-- For Empty Table --//
        //        {
        //           /* using (TransactionScope ts = new TransactionScope())
        //            {
        //                try
        //                {*/
        //                    //--------- Add New Task List To Employee For PICKUP ---------//
        //                    CustomerAssignInsert(EmployeeName, DeliverTo, ShopOrderCode, UserLoginID);
        //                    //------------------------------------------------------------------
        //               /*     ts.Complete();
        //                }
        //                catch (Exception ex)
        //                {
        //                    //Incase of Insertion fail Message to be Display
        //                    ViewBag.Message = "Sorry! Problem in Inserting Record!!";
        //                    //RollBack All Transaction
        //                    ts.Dispose();

        //                    ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine
        //                        , ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
        //                }
        //            }*/
        //        }
        //    }
        //    //Response.Write("<script>parent.location.reload();</script>");
        //   // return View(EmployeeAssignment);
        //   // return RedirectToAction("Index", "EmployeeAssignment", new { FromDate = FromDate1, ToDate = ToDate1, page = page1, DeliveryStatus = DeliveryStatus1, SearchString = SearchString1, DeliveryType = DeliveryType1 });
        //    return View("Index", "EmployeeAssignment", new { FromDate = FromDate1, ToDate = ToDate1, page = page1, DeliveryStatus = DeliveryStatus1, SearchString = SearchString1, DeliveryType = DeliveryType1, EmployeeCode = EmployeeCode1,AssignStatus= AssignStatus1 });

        //    //return View("Index", new { FromDate = FromDate1, ToDate = ToDate1, page = page1, DeliveryStatus = DeliveryStatus1, SearchString = SearchString1, DeliveryType = DeliveryType1, EmployeeCode = EmployeeCode1, AssignStatus = AssignStatus1 });

        //}
        //[HttpPost]
        //[SessionExpire]
        //public ActionResult UnAssign(string ShopOrderCode)
        //{
        //    string FromDate1 = TempData["FromDate"] != null ? TempData["FromDate"].ToString() : null;
        //    string ToDate1 = TempData["ToDate"] != null ? TempData["ToDate"].ToString() : null;
        //    string SearchString1 = TempData["SearchString"] != null ? TempData["SearchString"].ToString() : "";
        //    string DeliveryType1 = TempData["DeliveryType"] != null ? TempData["DeliveryType"].ToString() : "";
        //    int? page1 = TempData["page"] != null ? (int)TempData["page"] : 1;
        //    long? DeliveryStatus1 = TempData["DeliveryStatus"] != null ? (long)TempData["DeliveryStatus"] : DeliveryStatus1 = null;
        //    string EmployeeCode1 = TempData["EmployeeCode"] != null ? TempData["EmployeeCode"].ToString() : "";
        //    string AssignStatus1 = TempData["AssignStatus"] != null ? TempData["AssignStatus"].ToString() : "";
           
        //    TempData.Keep();

        //    long UserLoginID = Convert.ToInt64(Session["USER_LOGIN_ID"]);
        //    var IsTableEmpty = from ea in db.EmployeeAssignment select ea.ID;
        //    //List<DeliveryTypeViewModel> DeliveryTypeViewModels = new List<DeliveryTypeViewModel>();

        //    //-- For without Empty Table --//
        //    if (IsTableEmpty.Count() > 0)
        //    {
        //        var IsContain = (from EA in db.EmployeeAssignment
        //                         where EA.OrderStatus >= 3 && EA.OrderStatus <= 6
        //                         && ShopOrderCode.Contains(EA.ShopOrderCode)
        //                         select EA.ShopOrderCode
        //                              ).ToList();
        //       // var SOC_Count = ShopOrderCode.Split(',');
        //        if (IsContain.Count() > 0)
        //        {
        //           /* using (TransactionScope ts = new TransactionScope())
        //            {
        //                try
        //                {*/
        //                    //------------- UnAssign Task List To Employee -------------//
        //                    Unassign(ShopOrderCode, UserLoginID);
        //                    //------------------------------------------------------------//
        //               /*    ts.Complete();
        //                }
        //                catch (Exception ex)
        //                {
        //                    //Incase of Insertion fail Message to be Display
        //                    ViewBag.Message = "Sorry! Problem in Inserting Record!!";
        //                    //RollBack All Transaction
        //                    ts.Dispose();

        //                    ErrorLog.ErrorLogFile("Sorry! Problem in Record Creation!!" + Environment.NewLine
        //                        , ErrorLog.Module.API, System.Web.HttpContext.Current.Server);
        //                }
        //            }*/
        //        }
        //    }

        //   // return RedirectToAction("Index", "EmployeeAssignment", new { FromDate = FromDate1, ToDate = ToDate1, page = page1, DeliveryStatus = DeliveryStatus1, SearchString = SearchString1, DeliveryType = DeliveryType1 });
        //    return RedirectToAction("Index", "EmployeeAssignment", new { FromDate = FromDate1, ToDate = ToDate1, page = page1, DeliveryStatus = DeliveryStatus1, SearchString = SearchString1, DeliveryType = DeliveryType1, EmployeeCode = EmployeeCode1, AssignStatus = AssignStatus1 });
        //    // return RedirectToAction()
        //    //return View();

        //}
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        //#region -----GENERAL METHOD -----

        //private long getOwnerIDUsingSession(long PersonalID)
        //{
        //    try
        //    {
        //        long LoginID = db.PersonalDetails.Where(x => x.ID == PersonalID).FirstOrDefault().UserLoginID;
        //        long BusinessDetailID = db.BusinessDetails.Where(x => x.UserLoginID == LoginID).FirstOrDefault().ID;
        //        long merchant = db.DeliveryPartners.Where(x => x.BusinessDetailID == BusinessDetailID).FirstOrDefault().ID;
        //        var MerchantID = (from S in db.DeliveryPartners
        //                          join bd in db.BusinessDetails on S.BusinessDetailID equals bd.ID
        //                          join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
        //                          where bd.UserLoginID == LoginID && bt.Prefix == "GBDP"
        //                          select new
        //                          {
        //                              S.ID
        //                          }
        //                               ).FirstOrDefault();

        //        return Convert.ToInt64(MerchantID.ID);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Unable to GetDeliveryPartner ID from Method : getOwnerIDUsingSession" + ex.InnerException.ToString());
        //    }
        //}

        //private void GodownAssignInsert(string EmployeeName, string DeliverTo, string ShopOrderCode, long UserLoginID)
        //{
        //    //-- Insert into EmployeeAssignment table --//
        //    var employeeAssignInGodown = (from DOD in db.DeliveryOrderDetails
        //                                  join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                                  join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID
        //                                  join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                                  join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                  join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                  join S in db.Shops on COD.ShopID equals S.ID
        //                                  join PC in db.Pincodes on S.PincodeID equals PC.ID
        //                                  where //DOD.ShopOrderCode == COD.ShopOrderCode// &&
        //                                      /* COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.PACKED &&
        //                                       COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_GODOWN &&
        //                                       DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&*/
        //                                 ShopOrderCode.Contains(DOD.ShopOrderCode) && DOD.IsActive == true && COD.IsActive == true
        //                                 && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true && PD.IsActive == true
        //                                  select new
        //                                  {
        //                                      ID = DOD.ID,
        //                                      OrderCode = CO.OrderCode,
        //                                      ShopOrderCode = DOD.ShopOrderCode,
        //                                      EmployeeCode = EmployeeName,
        //                                      OrderStatus = COD.OrderStatus,
        //                                      FromAddress = S.Address.ToUpper() + " - " + PC.Name,
        //                                      DeliveredType = DeliverTo,
        //                                      DeliveryType = DOD.DeliveryType,
        //                                      DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                      DeliverySchedule = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                      ToAddress = DP.GodownAddress.ToUpper() + " - " + DP.PincodeID,
        //                                      IsActive=COD.IsActive,
        //                                      // DeliveredTime=null,
        //                                      // CreateDate =CommonFunctions.GetLocalTime(),
        //                                      CreateBy = PD.ID,
        //                                      // ModifyDate = DOD.ModifyDate,
        //                                      // ModifyBy = DOD.ModifyBy,
        //                                      NetworkIP = DOD.NetworkIP,
        //                                      DeviceType = DOD.DeviceType,
        //                                      DeviceID = DOD.DeviceID,
        //                                      X = S.Lattitude,
        //                                      Y = S.Longitude
        //                                  }).Distinct();
        //    if (employeeAssignInGodown.Count() > 0)
        //    {
        //        try
        //        {
        //            EzeeloDBContext db1 = new EzeeloDBContext();
        //            foreach (var item in employeeAssignInGodown)
        //            {
        //                ModelLayer.Models.EmployeeAssignment empAssign = new ModelLayer.Models.EmployeeAssignment();
        //                empAssign.ID = 0;
        //                empAssign.OrderCode = item.OrderCode;
        //                empAssign.ShopOrderCode = item.ShopOrderCode;
        //                empAssign.EmployeeCode = item.EmployeeCode;
        //                empAssign.OrderStatus = item.OrderStatus;
        //                empAssign.FromAddress = item.FromAddress;
        //                empAssign.DeliveredType = item.DeliveredType;
        //                empAssign.DeliveryType = item.DeliveryType;
        //                empAssign.DeliveryDate = item.DeliveryDate;
        //                empAssign.DeliverySchedule = item.DeliverySchedule;
        //                empAssign.ToAddress = item.ToAddress;
        //                empAssign.IsActive = item.IsActive;
        //                // empAssign.DeliveredTime=null;
        //                empAssign.CreateDate = CommonFunctions.GetLocalTime();
        //                empAssign.CreateBy = item.CreateBy;

        //                empAssign.NetworkIP = item.NetworkIP;
        //                empAssign.DeviceType = item.DeviceType;
        //                empAssign.DeviceID = item.DeviceID;
        //                empAssign.X = item.X;
        //                empAssign.Y = item.Y;

        //                db1.EmployeeAssignment.Add(empAssign);
        //            }

        //            db1.SaveChanges();
        //            db1.Dispose();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Unable to Insert Recored in EmployeeAssignment :- " + ex.InnerException);

        //        }
        //    }
        //    //-- End Insert into EmployeeAssignment table --//

        //    //-- Insert into EmployeeAssignmentHistory table --//
        //    var employeeAssignHistoryInGodown = (from DOD in db.DeliveryOrderDetails
        //                                         join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                                         join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID
        //                                         join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                                         join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                         join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                         join S in db.Shops on COD.ShopID equals S.ID
        //                                         join PC in db.Pincodes on S.PincodeID equals PC.ID
        //                                         join EA in db.EmployeeAssignment on DOD.ShopOrderCode equals EA.ShopOrderCode
        //                                         where //DOD.ShopOrderCode == COD.ShopOrderCode// &&
        //                                             /* COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.PACKED &&
        //                                              COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_GODOWN &&
        //                                              DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&*/
        //                                        ShopOrderCode.Contains(DOD.ShopOrderCode) && DOD.IsActive == true && COD.IsActive == true
        //                                        && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true && PD.IsActive == true
        //                                         select new
        //                                         {
        //                                             ID = DOD.ID,
        //                                             AssignID = EA.ID,
        //                                             OrderCode = CO.OrderCode,
        //                                             ShopOrderCode = DOD.ShopOrderCode,
        //                                             EmployeeCode = EmployeeName,
        //                                             OrderStatus = COD.OrderStatus,
        //                                             FromAddress = S.Address.ToUpper() + " - " + PC.Name,
        //                                             DeliveredType = DeliverTo,
        //                                             DeliveryType = DOD.DeliveryType,
        //                                             DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                             DeliverySchedule = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                             ToAddress = DP.GodownAddress.ToUpper() + " - " + DP.PincodeID,
        //                                             IsActive = COD.IsActive,
        //                                             // DeliveredTime=null,
        //                                             // CreateDate =CommonFunctions.GetLocalTime(),
        //                                             CreateBy = PD.ID,
        //                                             // ModifyDate = DOD.ModifyDate,
        //                                             // ModifyBy = DOD.ModifyBy,
        //                                             NetworkIP = DOD.NetworkIP,
        //                                             DeviceType = DOD.DeviceType,
        //                                             DeviceID = DOD.DeviceID,
        //                                             X = S.Lattitude,
        //                                             Y = S.Longitude
        //                                         }).Distinct();
        //    if (employeeAssignHistoryInGodown.Count() > 0)
        //    {
        //        try
        //        {
        //            EzeeloDBContext db2 = new EzeeloDBContext();
        //            foreach (var item in employeeAssignHistoryInGodown)
        //            {
        //                ModelLayer.Models.EmployeeAssignmentHistory empHistoryAssign = new ModelLayer.Models.EmployeeAssignmentHistory();
        //                empHistoryAssign.ID = 0;
        //                empHistoryAssign.AssignID = item.AssignID;
        //                empHistoryAssign.OrderCode = item.OrderCode;
        //                empHistoryAssign.ShopOrderCode = item.ShopOrderCode;
        //                empHistoryAssign.EmployeeCode = item.EmployeeCode;
        //                empHistoryAssign.OrderStatus = item.OrderStatus;
        //                empHistoryAssign.FromAddress = item.FromAddress;
        //                empHistoryAssign.DeliveredType = item.DeliveredType;
        //                empHistoryAssign.DeliveryType = item.DeliveryType;
        //                empHistoryAssign.DeliveryDate = item.DeliveryDate;
        //                empHistoryAssign.DeliverySchedule = item.DeliverySchedule;
        //                empHistoryAssign.ToAddress = item.ToAddress;
        //                empHistoryAssign.IsActive = item.IsActive;
        //                // empAssign.DeliveredTime=null;
        //                empHistoryAssign.CreateDate = CommonFunctions.GetLocalTime();
        //                empHistoryAssign.CreateBy = item.CreateBy;
        //                empHistoryAssign.NetworkIP = item.NetworkIP;
        //                empHistoryAssign.DeviceType = item.DeviceType;
        //                empHistoryAssign.DeviceID = item.DeviceID;
        //                empHistoryAssign.X = item.X;
        //                empHistoryAssign.Y = item.Y;

        //                db2.EmployeeAssignmentHistory.Add(empHistoryAssign);
        //            }

        //            db2.SaveChanges();
        //            db2.Dispose();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Unable to Insert Recored in EmployeeAssignmentHistory :- " + ex.InnerException);

        //        }
        //    }
        //    //-- End of Insert into EmployeeAssignmentHistory table --//
        //}

        //private void GodownAssignUpdate(string EmployeeName, string DeliverTo, string ShopOrderCode, long UserLoginID)
        //{
        //    //-- Updated into EmployeeAssignment table --//
        //    var employeeAssignInGodown = (from DOD in db.DeliveryOrderDetails
        //                                  join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                                  join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID
        //                                  join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                                  join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                  join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                  join S in db.Shops on COD.ShopID equals S.ID
        //                                  join PC in db.Pincodes on S.PincodeID equals PC.ID
        //                                  join EA in db.EmployeeAssignment on DOD.ShopOrderCode equals EA.ShopOrderCode
        //                                  where //DOD.ShopOrderCode == COD.ShopOrderCode// &&
        //                                      /* COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.PACKED &&
        //                                       COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_GODOWN &&
        //                                       DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&*/
        //                                 ShopOrderCode.Contains(DOD.ShopOrderCode) && DOD.IsActive == true && COD.IsActive == true
        //                                 && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true && PD.IsActive == true
        //                                  select new
        //                                  {
        //                                      ID = EA.ID,
        //                                      OrderCode = CO.OrderCode,
        //                                      ShopOrderCode = DOD.ShopOrderCode,
        //                                      EmployeeCode = EmployeeName,
        //                                      OrderStatus = COD.OrderStatus,
        //                                      FromAddress = S.Address.ToUpper() + " - " + PC.Name,
        //                                      DeliveredType = DeliverTo,
        //                                      DeliveryType = DOD.DeliveryType,
        //                                      DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                      DeliverySchedule = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                      ToAddress = DP.GodownAddress.ToUpper() + " - " + DP.PincodeID,
        //                                      IsActive = COD.IsActive,
        //                                      // DeliveredTime=null,
        //                                      CreateDate =EA.CreateDate,
        //                                      CreateBy = EA.CreateBy,
        //                                      // ModifyDate = DOD.ModifyDate,
        //                                      ModifyBy = PD.ID,
        //                                      NetworkIP = DOD.NetworkIP,
        //                                      DeviceType = DOD.DeviceType,
        //                                      DeviceID = DOD.DeviceID,
        //                                      X = S.Lattitude,
        //                                      Y = S.Longitude
        //                                  }).Distinct();
        //    if (employeeAssignInGodown.Count() > 0)
        //    {
        //        try
        //        {
        //            EzeeloDBContext db1 = new EzeeloDBContext();
        //            foreach (var item in employeeAssignInGodown)
        //            {
        //                ModelLayer.Models.EmployeeAssignment empAssign = new ModelLayer.Models.EmployeeAssignment();
        //                empAssign.ID = item.ID;
        //                empAssign.OrderCode = item.OrderCode;
        //                empAssign.ShopOrderCode = item.ShopOrderCode;
        //                empAssign.EmployeeCode = item.EmployeeCode;
        //                empAssign.OrderStatus = item.OrderStatus;
        //                empAssign.FromAddress = item.FromAddress;
        //                empAssign.DeliveredType = item.DeliveredType;
        //                empAssign.DeliveryType = item.DeliveryType;
        //                empAssign.DeliveryDate = item.DeliveryDate;
        //                empAssign.DeliverySchedule = item.DeliverySchedule;
        //                empAssign.ToAddress = item.ToAddress;
        //                empAssign.IsActive = item.IsActive;
        //                // empAssign.DeliveredTime=null;
        //                empAssign.CreateDate = item.CreateDate;
        //                empAssign.CreateBy = item.CreateBy;
        //                empAssign.ModifyDate = CommonFunctions.GetLocalTime();
        //                empAssign.ModifyBy = item.ModifyBy;

        //                empAssign.NetworkIP = item.NetworkIP;
        //                empAssign.DeviceType = item.DeviceType;
        //                empAssign.DeviceID = item.DeviceID;
        //                empAssign.X = item.X;
        //                empAssign.Y = item.Y;

        //                db1.Entry(empAssign).State = EntityState.Modified;
        //                // db1.Entry(empAssign).State = EntityState.Added;
        //                db1.SaveChanges();
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Unable to Update Recored in EmployeeAssignment :- " + ex.InnerException);

        //        }
        //    }
        //    //-- End Update into EmployeeAssignment table --//

        //    //-- Insert into EmployeeAssignmentHistory table --//

        //    var employeeAssignHistoryInGodown = (from DOD in db.DeliveryOrderDetails
        //                                         join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                                         join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID
        //                                         join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                                         join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                         join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                         join S in db.Shops on COD.ShopID equals S.ID
        //                                         join PC in db.Pincodes on S.PincodeID equals PC.ID
        //                                         join EA in db.EmployeeAssignment on DOD.ShopOrderCode equals EA.ShopOrderCode
        //                                         where //DOD.ShopOrderCode == COD.ShopOrderCode// &&
        //                                             /* COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.PACKED &&
        //                                              COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_GODOWN &&
        //                                              DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&*/
        //                                        ShopOrderCode.Contains(DOD.ShopOrderCode) && DOD.IsActive == true && COD.IsActive == true
        //                                        && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true && PD.IsActive == true
        //                                         select new
        //                                         {
        //                                             ID = 0,
        //                                             AssignID = EA.ID,
        //                                             OrderCode = CO.OrderCode,
        //                                             ShopOrderCode = DOD.ShopOrderCode,
        //                                             EmployeeCode = EmployeeName,
        //                                             OrderStatus = COD.OrderStatus,
        //                                             FromAddress = S.Address.ToUpper() + " - " + PC.Name,
        //                                             DeliveredType = DeliverTo,
        //                                             DeliveryType = DOD.DeliveryType,
        //                                             DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                             DeliverySchedule = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                             ToAddress = DP.GodownAddress.ToUpper() + " - " + DP.PincodeID,
        //                                             IsActive = COD.IsActive,
        //                                             // DeliveredTime=null,
        //                                             //CreateDate =EAH.CreateDate,
        //                                             CreateBy = PD.ID,
        //                                             // ModifyDate = DOD.ModifyDate,
        //                                             // ModifyBy = PD.ID,
        //                                             NetworkIP = DOD.NetworkIP,
        //                                             DeviceType = DOD.DeviceType,
        //                                             DeviceID = DOD.DeviceID,
        //                                             X = S.Lattitude,
        //                                             Y = S.Longitude
        //                                         }).Distinct();
        //    if (employeeAssignHistoryInGodown.Count() > 0)
        //    {
        //        try
        //        {
        //            EzeeloDBContext db2 = new EzeeloDBContext();
        //            foreach (var item in employeeAssignHistoryInGodown)
        //            {
        //                ModelLayer.Models.EmployeeAssignmentHistory empHistoryAssign = new ModelLayer.Models.EmployeeAssignmentHistory();
        //                empHistoryAssign.ID = item.ID;
        //                empHistoryAssign.AssignID = item.AssignID;
        //                empHistoryAssign.OrderCode = item.OrderCode;
        //                empHistoryAssign.ShopOrderCode = item.ShopOrderCode;
        //                empHistoryAssign.EmployeeCode = item.EmployeeCode;
        //                empHistoryAssign.OrderStatus = item.OrderStatus;
        //                empHistoryAssign.FromAddress = item.FromAddress;
        //                empHistoryAssign.DeliveredType = item.DeliveredType;
        //                empHistoryAssign.DeliveryType = item.DeliveryType;
        //                empHistoryAssign.DeliveryDate = item.DeliveryDate;
        //                empHistoryAssign.DeliverySchedule = item.DeliverySchedule;
        //                empHistoryAssign.ToAddress = item.ToAddress;
        //                empHistoryAssign.IsActive = item.IsActive;
        //                // empAssign.DeliveredTime=null;
        //                empHistoryAssign.CreateDate = CommonFunctions.GetLocalTime();
        //                empHistoryAssign.CreateBy = item.CreateBy;
        //                //empHistoryAssign.ModifyDate = CommonFunctions.GetLocalTime();
        //                //empHistoryAssign.ModifyBy = item.ModifyBy;

        //                empHistoryAssign.NetworkIP = item.NetworkIP;
        //                empHistoryAssign.DeviceType = item.DeviceType;
        //                empHistoryAssign.DeviceID = item.DeviceID;
        //                empHistoryAssign.X = item.X;
        //                empHistoryAssign.Y = item.Y;

        //                // db2.Entry(empHistoryAssign).State = EntityState.Modified;
        //                //db2.SaveChanges();
        //                db2.EmployeeAssignmentHistory.Add(empHistoryAssign);

        //            }

        //            db2.SaveChanges();
        //            db2.Dispose();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Unable to Insert Recored in EmployeeAssignmentHistory :- " + ex.InnerException);

        //        }
        //    }
        //    //-- End of Insert into EmployeeAssignmentHistory table --//
        //}

        //private void Unassign(string ShopOrderCode, long UserLoginID)
        //{
        //    try
        //    {
        //        //-- Update into EmployeeAssignment table --//
        //        var employeeAssignInGodown = (from DOD in db.DeliveryOrderDetails
        //                                      join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                                      join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID
        //                                      join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                                      join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                      join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                      join S in db.Shops on COD.ShopID equals S.ID
        //                                      join PC in db.Pincodes on S.PincodeID equals PC.ID
        //                                      join EA in db.EmployeeAssignment on DOD.ShopOrderCode equals EA.ShopOrderCode
        //                                      where //DOD.ShopOrderCode == COD.ShopOrderCode// &&
        //                                          /* COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.PACKED &&
        //                                           COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_GODOWN &&
        //                                           DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&*/
        //                                     ShopOrderCode.Contains(DOD.ShopOrderCode) && DOD.IsActive == true && COD.IsActive == true
        //                                     && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true && PD.IsActive == true
        //                                      select new
        //                                      {
        //                                          ID = EA.ID,
        //                                          OrderCode = CO.OrderCode,
        //                                          ShopOrderCode = DOD.ShopOrderCode,
        //                                          //EmployeeCode = EmployeeName,
        //                                          OrderStatus = COD.OrderStatus,
        //                                          FromAddress = S.Address.ToUpper() + " - " + PC.Name,
        //                                          DeliveredType = EA.DeliveredType,
        //                                          DeliveryType = DOD.DeliveryType,
        //                                          DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                          DeliverySchedule = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                          ToAddress = DP.GodownAddress.ToUpper() + " - " + DP.PincodeID,
        //                                          IsActive = COD.IsActive,
        //                                          // DeliveredTime=null,
        //                                          CreateDate = EA.CreateDate,
        //                                          CreateBy = EA.CreateBy,
        //                                          // ModifyDate = DOD.ModifyDate,
        //                                          ModifyBy = PD.ID,
        //                                          NetworkIP = DOD.NetworkIP,
        //                                          DeviceType = DOD.DeviceType,
        //                                          DeviceID = DOD.DeviceID,
        //                                          X = S.Lattitude,
        //                                          Y = S.Longitude
        //                                      }).Distinct();

        //        if (employeeAssignInGodown.Count() > 0)
        //        {
        //            EzeeloDBContext db1 = new EzeeloDBContext();
        //            foreach (var item in employeeAssignInGodown)
        //            {
        //                ModelLayer.Models.EmployeeAssignment empAssign = new ModelLayer.Models.EmployeeAssignment();
        //                empAssign.ID = item.ID;
        //                empAssign.OrderCode = item.OrderCode;
        //                empAssign.ShopOrderCode = item.ShopOrderCode;
        //                //empAssign.EmployeeCode = item.EmployeeCode;
        //                empAssign.OrderStatus = item.OrderStatus;
        //                empAssign.FromAddress = item.FromAddress;
        //                empAssign.DeliveredType = item.DeliveredType;
        //                empAssign.DeliveryType = item.DeliveryType;
        //                empAssign.DeliveryDate = item.DeliveryDate;
        //                empAssign.DeliverySchedule = item.DeliverySchedule;
        //                empAssign.ToAddress = item.ToAddress;
        //                empAssign.IsActive = item.IsActive;
        //                // empAssign.DeliveredTime=null;
        //                empAssign.CreateDate = item.CreateDate;
        //                empAssign.CreateBy = item.CreateBy;
        //                empAssign.ModifyDate = CommonFunctions.GetLocalTime();
        //                empAssign.ModifyBy = item.ModifyBy;

        //                empAssign.NetworkIP = item.NetworkIP;
        //                empAssign.DeviceType = item.DeviceType;
        //                empAssign.DeviceID = item.DeviceID;
        //                empAssign.X = item.X;
        //                empAssign.Y = item.Y;

        //                db1.Entry(empAssign).State = EntityState.Modified;
        //                // db1.Entry(empAssign).State = EntityState.Added;
        //                db1.SaveChanges();
        //            }

        //        }
        //        //-- End Update into EmployeeAssignment table --//

        //        //-- Insert into EmployeeAssignmentHistory table --//

        //        var employeeAssignHistoryInGodown = (from DOD in db.DeliveryOrderDetails
        //                                             join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                                             join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID
        //                                             join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                                             join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                             join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                             join S in db.Shops on COD.ShopID equals S.ID
        //                                             join PC in db.Pincodes on S.PincodeID equals PC.ID
        //                                             join EA in db.EmployeeAssignment on DOD.ShopOrderCode equals EA.ShopOrderCode
        //                                             where //DOD.ShopOrderCode == COD.ShopOrderCode// &&
        //                                                 /* COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.PACKED &&
        //                                                  COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_GODOWN &&
        //                                                  DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&*/
        //                                            ShopOrderCode.Contains(DOD.ShopOrderCode) && DOD.IsActive == true && COD.IsActive == true
        //                                            && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true && PD.IsActive == true
        //                                             select new
        //                                             {
        //                                                 ID = 0,
        //                                                 AssignID = EA.ID,
        //                                                 OrderCode = CO.OrderCode,
        //                                                 ShopOrderCode = DOD.ShopOrderCode,
        //                                                 //EmployeeCode = EmployeeName,
        //                                                 OrderStatus = COD.OrderStatus,
        //                                                 FromAddress = S.Address.ToUpper() + " - " + PC.Name,
        //                                                 DeliveredType = EA.DeliveredType,
        //                                                 DeliveryType = DOD.DeliveryType,
        //                                                 DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                                 DeliverySchedule = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                                 ToAddress = DP.GodownAddress.ToUpper() + " - " + DP.PincodeID,
        //                                                 IsActive = COD.IsActive,
        //                                                 // DeliveredTime=null,
        //                                                 //CreateDate =EAH.CreateDate,
        //                                                 CreateBy = PD.ID,
        //                                                 // ModifyDate = DOD.ModifyDate,
        //                                                 // ModifyBy = PD.ID,
        //                                                 NetworkIP = DOD.NetworkIP,
        //                                                 DeviceType = DOD.DeviceType,
        //                                                 DeviceID = DOD.DeviceID,
        //                                                 X = S.Lattitude,
        //                                                 Y = S.Longitude
        //                                             }).Distinct();
        //        if (employeeAssignHistoryInGodown.Count() > 0)
        //        {
        //            EzeeloDBContext db2 = new EzeeloDBContext();
        //            foreach (var item in employeeAssignHistoryInGodown)
        //            {
        //                ModelLayer.Models.EmployeeAssignmentHistory empHistoryAssign = new ModelLayer.Models.EmployeeAssignmentHistory();
        //                empHistoryAssign.ID = item.ID;
        //                empHistoryAssign.AssignID = item.AssignID;
        //                empHistoryAssign.OrderCode = item.OrderCode;
        //                empHistoryAssign.ShopOrderCode = item.ShopOrderCode;
        //                //empHistoryAssign.EmployeeCode = item.EmployeeCode;
        //                empHistoryAssign.OrderStatus = item.OrderStatus;
        //                empHistoryAssign.FromAddress = item.FromAddress;
        //                empHistoryAssign.DeliveredType = item.DeliveredType;
        //                empHistoryAssign.DeliveryType = item.DeliveryType;
        //                empHistoryAssign.DeliveryDate = item.DeliveryDate;
        //                empHistoryAssign.DeliverySchedule = item.DeliverySchedule;
        //                empHistoryAssign.ToAddress = item.ToAddress;
        //                empHistoryAssign.IsActive = item.IsActive;
        //                // empAssign.DeliveredTime=null;
        //                empHistoryAssign.CreateDate = CommonFunctions.GetLocalTime();
        //                empHistoryAssign.CreateBy = item.CreateBy;
        //                //empHistoryAssign.ModifyDate = CommonFunctions.GetLocalTime();
        //                //empHistoryAssign.ModifyBy = item.ModifyBy;

        //                empHistoryAssign.NetworkIP = item.NetworkIP;
        //                empHistoryAssign.DeviceType = item.DeviceType;
        //                empHistoryAssign.DeviceID = item.DeviceID;
        //                empHistoryAssign.X = item.X;
        //                empHistoryAssign.Y = item.Y;

        //                // db2.Entry(empHistoryAssign).State = EntityState.Modified;
        //                //db2.SaveChanges();
        //                db2.EmployeeAssignmentHistory.Add(empHistoryAssign);

        //            }

        //            db2.SaveChanges();
        //            db2.Dispose();
        //        }
        //        //-- End of Insert into EmployeeAssignmentHistory table --//
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Unable to Update Recored in CustomerOrderDetail :- " + ex.InnerException);

        //    }
        //}
  
        //private void GodownAssignAddNew(string EmployeeName, string DeliverTo, string ShopOrderCode, long UserLoginID)
        //{
        //    //-- Insert into EmployeeAssignment table --//
        //    /*var AssignGodownAddress = (from DP in db.DeliveryPartners
        //                               join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                               join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                               where BD.UserLoginID == UserLoginID && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true
        //                               select new
        //                               {
        //                                   ID = DP.ID,
        //                                   GodownCode = DP.ID,
        //                                   EmployeeCode = EmployeeName,
        //                                   Address = DP.GodownAddress,
        //                                   DeliveredType = DeliverTo,
        //                                   // CreateDate = CommonFunctions.GetLocalTime(),
        //                                   CreateBy = PD.ID,
        //                                   NetworkIP = DP.NetworkIP,
        //                                   DeviceType = DP.DeviceType,
        //                                   DeviceID = DP.DeviceID

        //                                   //X = S.Lattitude, // Need to add in DeliveryPartners table
        //                                   // Y = S.Longitude
        //                               }).Distinct();*/


        //    var employeeAssignInGodown = (from DOD in db.DeliveryOrderDetails
        //                                  join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                                  join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID
        //                                  join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                                  join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                  join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                  join S in db.Shops on COD.ShopID equals S.ID
        //                                  where //DOD.ShopOrderCode == COD.ShopOrderCode// &&
        //                                      /* COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.PACKED &&
        //                                       COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_GODOWN &&
        //                                       DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&*/
        //                                 ShopOrderCode.Contains(DOD.ShopOrderCode) && DOD.IsActive == true && COD.IsActive == true
        //                                 && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true && PD.IsActive == true
        //                                  select new
        //                                  {
        //                                      ID = DOD.ID,
        //                                      OrderCode = CO.OrderCode,
        //                                      ShopOrderCode = DOD.ShopOrderCode,
        //                                      EmployeeCode = EmployeeName,
        //                                      OrderStatus = COD.OrderStatus,
        //                                      FromAddress = S.Address,
        //                                      DeliveredType = DeliverTo,
        //                                      DeliveryType = DOD.DeliveryType,
        //                                      DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                      DeliverySchedule = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                      ToAddress = DP.GodownAddress,
        //                                      IsActive = COD.IsActive,
        //                                      // DeliveredTime=null,
        //                                      // CreateDate =CommonFunctions.GetLocalTime(),
        //                                      CreateBy = PD.ID,
        //                                      // ModifyDate = DOD.ModifyDate,
        //                                      // ModifyBy = DOD.ModifyBy,
        //                                      NetworkIP = DOD.NetworkIP,
        //                                      DeviceType = DOD.DeviceType,
        //                                      DeviceID = DOD.DeviceID,
        //                                      X = S.Lattitude,
        //                                      Y = S.Longitude
        //                                  }).Distinct();
        //    if (employeeAssignInGodown.Count() > 0)
        //    {
        //        try
        //        {
        //            EzeeloDBContext db1 = new EzeeloDBContext();
        //            foreach (var item in employeeAssignInGodown)
        //            {
        //                ModelLayer.Models.EmployeeAssignment empAssign = new ModelLayer.Models.EmployeeAssignment();
        //                empAssign.ID = 0;
        //                empAssign.OrderCode = item.OrderCode;
        //                empAssign.ShopOrderCode = item.ShopOrderCode;
        //                empAssign.EmployeeCode = item.EmployeeCode;
        //                empAssign.OrderStatus = item.OrderStatus;
        //                empAssign.FromAddress = item.FromAddress;
        //                empAssign.DeliveredType = item.DeliveredType;
        //                empAssign.DeliveryType = item.DeliveryType;
        //                empAssign.DeliveryDate = item.DeliveryDate;
        //                empAssign.DeliverySchedule = item.DeliverySchedule;
        //                empAssign.ToAddress = item.ToAddress;
        //                empAssign.IsActive = item.IsActive;
        //                // empAssign.DeliveredTime=null;
        //                empAssign.CreateDate = CommonFunctions.GetLocalTime();
        //                empAssign.CreateBy = item.CreateBy;
        //                // empAssign.ModifyDate = null;
        //                // empAssign.ModifyBy = null;
        //                empAssign.NetworkIP = item.NetworkIP;
        //                empAssign.DeviceType = item.DeviceType;
        //                empAssign.DeviceID = item.DeviceID;
        //                empAssign.X = item.X;
        //                empAssign.Y = item.Y;

        //                db1.EmployeeAssignment.Add(empAssign);
        //            }

        //            /* if (AssignGodownAddress.Count() > 0)
        //             {
        //                 foreach (var item in AssignGodownAddress)
        //                 {
        //                     ModelLayer.Models.EmployeeAssignment GodownAddress = new ModelLayer.Models.EmployeeAssignment();
        //                     GodownAddress.ID = 0;
        //                     GodownAddress.GodownCode = item.GodownCode;
        //                     GodownAddress.EmployeeCode = item.EmployeeCode;
        //                     GodownAddress.Address = item.Address;
        //                     GodownAddress.DeliveredType = item.DeliveredType;
        //                     GodownAddress.CreateDate = CommonFunctions.GetLocalTime();
        //                     GodownAddress.CreateBy = item.CreateBy;
        //                     GodownAddress.NetworkIP = item.NetworkIP;
        //                     GodownAddress.DeviceType = item.DeviceType;
        //                     GodownAddress.DeviceID = item.DeviceID;

        //                     db1.EmployeeAssignment.Add(GodownAddress);
        //                 }
        //             }*/
        //            db1.SaveChanges();
        //            db1.Dispose();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Unable to Insert Recored in EmployeeAssignment :- " + ex.InnerException);

        //        }
        //    }
        //    //-- End Insert into EmployeeAssignment table --//

        //    //-- Insert into EmployeeAssignmentHistory table --//
        //   /* var AssignHistoryGodownAddress = (from DP in db.DeliveryPartners
        //                                      join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                      join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                      join EA in db.EmployeeAssignment on DP.ID equals EA.GodownCode
        //                                      where BD.UserLoginID == UserLoginID && EA.EmployeeCode == EmployeeName && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true
        //                                      select new
        //                                      {
        //                                          ID = DP.ID,
        //                                          AssignID = EA.ID,
        //                                          GodownCode = DP.ID,
        //                                          EmployeeCode = EmployeeName,
        //                                          Address = DP.GodownAddress,
        //                                          DeliveredType = DeliverTo,
        //                                          // CreateDate = CommonFunctions.GetLocalTime(),
        //                                          CreateBy = PD.ID,
        //                                          NetworkIP = DP.NetworkIP,
        //                                          DeviceType = DP.DeviceType,
        //                                          DeviceID = DP.DeviceID

        //                                          //X = S.Lattitude, // Need to add in DeliveryPartners table
        //                                          // Y = S.Longitude
        //                                      }).Distinct();*/


        //    var employeeAssignHistoryInGodown = (from DOD in db.DeliveryOrderDetails
        //                                         join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                                         join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID
        //                                         join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                                         join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                         join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                         join S in db.Shops on COD.ShopID equals S.ID
        //                                         join EA in db.EmployeeAssignment on DOD.ShopOrderCode equals EA.ShopOrderCode
        //                                         where //DOD.ShopOrderCode == COD.ShopOrderCode// &&
        //                                             /* COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.PACKED &&
        //                                              COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_GODOWN &&
        //                                              DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&*/
        //                                        ShopOrderCode.Contains(DOD.ShopOrderCode) && DOD.IsActive == true && COD.IsActive == true
        //                                        && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true && PD.IsActive == true
        //                                         select new
        //                                         {
        //                                             ID = DOD.ID,
        //                                             AssignID = EA.ID,
        //                                             OrderCode = CO.OrderCode,
        //                                             ShopOrderCode = DOD.ShopOrderCode,
        //                                             EmployeeCode = EmployeeName,
        //                                             OrderStatus = COD.OrderStatus,
        //                                             FromAddress = S.Address,
        //                                             DeliveredType = DeliverTo,
        //                                             DeliveryType = DOD.DeliveryType,
        //                                             DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                             DeliverySchedule = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                             ToAddress=DP.GodownAddress,
        //                                             IsActive = COD.IsActive,
        //                                             // DeliveredTime=null,
        //                                             // CreateDate =CommonFunctions.GetLocalTime(),
        //                                             CreateBy = PD.ID,
        //                                             // ModifyDate = DOD.ModifyDate,
        //                                             // ModifyBy = DOD.ModifyBy,
        //                                             NetworkIP = DOD.NetworkIP,
        //                                             DeviceType = DOD.DeviceType,
        //                                             DeviceID = DOD.DeviceID,
        //                                             X = S.Lattitude,
        //                                             Y = S.Longitude
        //                                         }).Distinct();
        //    if (employeeAssignHistoryInGodown.Count() > 0)
        //    {
        //        try
        //        {
        //            EzeeloDBContext db2 = new EzeeloDBContext();
        //            foreach (var item in employeeAssignHistoryInGodown)
        //            {
        //                ModelLayer.Models.EmployeeAssignmentHistory empHistoryAssign = new ModelLayer.Models.EmployeeAssignmentHistory();
        //                empHistoryAssign.ID = 0;
        //                empHistoryAssign.AssignID = item.AssignID;
        //                empHistoryAssign.OrderCode = item.OrderCode;
        //                empHistoryAssign.ShopOrderCode = item.ShopOrderCode;
        //                empHistoryAssign.EmployeeCode = item.EmployeeCode;
        //                empHistoryAssign.OrderStatus = item.OrderStatus;
        //                empHistoryAssign.FromAddress = item.FromAddress;
        //                empHistoryAssign.DeliveredType = item.DeliveredType;
        //                empHistoryAssign.DeliveryType = item.DeliveryType;
        //                empHistoryAssign.DeliveryDate = item.DeliveryDate;
        //                empHistoryAssign.DeliverySchedule = item.DeliverySchedule;
        //                empHistoryAssign.ToAddress = item.ToAddress;
        //                empHistoryAssign.IsActive = item.IsActive;
        //                // empAssign.DeliveredTime=null;
        //                empHistoryAssign.CreateDate = CommonFunctions.GetLocalTime();
        //                empHistoryAssign.CreateBy = item.CreateBy;
        //                // empAssign.ModifyDate = null;
        //                // empAssign.ModifyBy = null;
        //                empHistoryAssign.NetworkIP = item.NetworkIP;
        //                empHistoryAssign.DeviceType = item.DeviceType;
        //                empHistoryAssign.DeviceID = item.DeviceID;
        //                empHistoryAssign.X = item.X;
        //                empHistoryAssign.Y = item.Y;

        //                db2.EmployeeAssignmentHistory.Add(empHistoryAssign);
        //            }

        //            /*if (AssignHistoryGodownAddress.Count() > 0)
        //            {
        //                foreach (var item in AssignHistoryGodownAddress)
        //                {
        //                    ModelLayer.Models.EmployeeAssignmentHistory GodownHistoryAddress = new ModelLayer.Models.EmployeeAssignmentHistory();
        //                    GodownHistoryAddress.ID = 0;
        //                    GodownHistoryAddress.AssignID = item.AssignID;
        //                    GodownHistoryAddress.GodownCode = item.GodownCode;
        //                    GodownHistoryAddress.EmployeeCode = item.EmployeeCode;
        //                    GodownHistoryAddress.Address = item.Address;
        //                    GodownHistoryAddress.DeliveredType = item.DeliveredType;
        //                    GodownHistoryAddress.CreateDate = CommonFunctions.GetLocalTime();
        //                    GodownHistoryAddress.CreateBy = item.CreateBy;
        //                    GodownHistoryAddress.NetworkIP = item.NetworkIP;
        //                    GodownHistoryAddress.DeviceType = item.DeviceType;
        //                    GodownHistoryAddress.DeviceID = item.DeviceID;

        //                    db2.EmployeeAssignmentHistory.Add(GodownHistoryAddress);
        //                }
        //            }*/
        //            db2.SaveChanges();
        //            db2.Dispose();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Unable to Insert Recored in EmployeeAssignmentHistory :- " + ex.InnerException);

        //        }
        //    }
        //    //-- End of Insert into EmployeeAssignmentHistory table --//
        //}

        //private void CustomerAssignInsert(string EmployeeName, string DeliverTo, string ShopOrderCode, long UserLoginID)
        //{
        //    //-- Insert into EmployeeAssignment table --//
        //        //-- It will satisfied this query --//
        //       var employeeAssignFromShop = (from DOD in db.DeliveryOrderDetails
        //                                      join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                                      join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID
        //                                      join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                                      join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                      join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                      join S in db.Shops on COD.ShopID equals S.ID
        //                                     //join PC in db.Pincodes on S.PincodeID equals PC.ID//hide
        //                                     join PC in db.Pincodes on CO.PincodeID equals PC.ID//added
        //                                      where //DOD.ShopOrderCode == COD.ShopOrderCode// &&
        //                                          (COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.PACKED &&
        //                                           COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_SHOP) &&

        //                                      /* DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&*/
        //                                     ShopOrderCode.Contains(DOD.ShopOrderCode) && DOD.IsActive == true && COD.IsActive == true
        //                                     && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true && PD.IsActive == true
        //                                      select new
        //                                      {
        //                                          ID = DOD.ID,
        //                                          OrderCode = CO.OrderCode,
        //                                          ShopOrderCode = DOD.ShopOrderCode,
        //                                          EmployeeCode = EmployeeName,
        //                                          OrderStatus = COD.OrderStatus,
        //                                          FromAddress = S.Address.ToUpper()+" - "+S.PincodeID,
        //                                          DeliveredType = DeliverTo,
        //                                          DeliveryType = DOD.DeliveryType,
        //                                          DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                          DeliverySchedule = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                          ToAddress = CO.ShippingAddress.ToUpper() + " - " + PC.Name,// CO.PincodeID,
        //                                          IsActive = COD.IsActive,
        //                                          // DeliveredTime=null,
        //                                          // CreateDate =CommonFunctions.GetLocalTime(),
        //                                          CreateBy = PD.ID,
        //                                          // ModifyDate = DOD.ModifyDate,
        //                                          // ModifyBy = DOD.ModifyBy,
        //                                          NetworkIP = DOD.NetworkIP,
        //                                          DeviceType = DOD.DeviceType,
        //                                          DeviceID = DOD.DeviceID,
        //                                          X = S.Lattitude,
        //                                          Y = S.Longitude
        //                                      }).Distinct();
                
        //          //-- OR it will satisfied this query --//
        //        var employeeAssignFromGodown = (from DOD in db.DeliveryOrderDetails
        //                                      join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                                      join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID
        //                                      join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                                      join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                      join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                      join S in db.Shops on COD.ShopID equals S.ID
        //                                      join PC in db.Pincodes on CO.PincodeID equals PC.ID //added
        //                                      where //DOD.ShopOrderCode == COD.ShopOrderCode// &&
        //                                           (COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.IN_GODOWN &&
        //                                           COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_GODOWN) &&
        //                                            /* DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&*/
        //                                      ShopOrderCode.Contains(DOD.ShopOrderCode) && DOD.IsActive == true && COD.IsActive == true
        //                                     && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true && PD.IsActive == true
        //                                      select new
        //                                      {
        //                                          ID = DOD.ID,
        //                                          OrderCode = CO.OrderCode,
        //                                          ShopOrderCode = DOD.ShopOrderCode,
        //                                          EmployeeCode = EmployeeName,
        //                                          OrderStatus = COD.OrderStatus,
        //                                          FromAddress = DP.GodownAddress.ToUpper() + " - " + DP.PincodeID,
        //                                          DeliveredType = DeliverTo,
        //                                          DeliveryType = DOD.DeliveryType,
        //                                          DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                          DeliverySchedule = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                          ToAddress = CO.ShippingAddress.ToUpper() + " - " +  PC.Name, //CO.PincodeID,
        //                                          IsActive = COD.IsActive,
        //                                          // DeliveredTime=null,
        //                                          // CreateDate =CommonFunctions.GetLocalTime(),
        //                                          CreateBy = PD.ID,
        //                                          // ModifyDate = DOD.ModifyDate,
        //                                          // ModifyBy = DOD.ModifyBy,
        //                                          NetworkIP = DOD.NetworkIP,
        //                                          DeviceType = DOD.DeviceType,
        //                                          DeviceID = DOD.DeviceID,
        //                                          X = S.Lattitude,
        //                                          Y = S.Longitude
        //                                      }).Distinct();
        //  //  }
        //        if (employeeAssignFromShop.Count() > 0)
        //        {
        //            try
        //            {
        //                EzeeloDBContext db1 = new EzeeloDBContext();
        //                foreach (var item in employeeAssignFromShop)
        //                {
        //                    ModelLayer.Models.EmployeeAssignment empAssign = new ModelLayer.Models.EmployeeAssignment();
        //                    empAssign.ID = 0;
        //                    empAssign.OrderCode = item.OrderCode;
        //                    empAssign.ShopOrderCode = item.ShopOrderCode;
        //                    empAssign.EmployeeCode = item.EmployeeCode;
        //                    empAssign.OrderStatus = item.OrderStatus;
        //                    empAssign.FromAddress = item.FromAddress;
        //                    empAssign.DeliveredType = item.DeliveredType;
        //                    empAssign.DeliveryType = item.DeliveryType;
        //                    empAssign.DeliveryDate = item.DeliveryDate;
        //                    empAssign.DeliverySchedule = item.DeliverySchedule;
        //                    empAssign.ToAddress = item.ToAddress;
        //                    empAssign.IsActive = item.IsActive;
        //                    // empAssign.DeliveredTime=null;
        //                    empAssign.CreateDate = CommonFunctions.GetLocalTime();
        //                    empAssign.CreateBy = item.CreateBy;
        //                    empAssign.NetworkIP = item.NetworkIP;
        //                    empAssign.DeviceType = item.DeviceType;
        //                    empAssign.DeviceID = item.DeviceID;
        //                    empAssign.X = item.X;
        //                    empAssign.Y = item.Y;

        //                    db1.EmployeeAssignment.Add(empAssign);
        //                }

        //                db1.SaveChanges();
        //                db1.Dispose();
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Unable to Insert Recored in EmployeeAssignment :- " + ex.InnerException);

        //            }
        //        }
        //        if (employeeAssignFromGodown.Count() > 0)
        //        {
        //            try
        //            {
        //                EzeeloDBContext db2 = new EzeeloDBContext();
        //                foreach (var item in employeeAssignFromGodown)
        //                {
        //                    ModelLayer.Models.EmployeeAssignment empAssign = new ModelLayer.Models.EmployeeAssignment();
        //                    empAssign.ID = 0;
        //                    empAssign.OrderCode = item.OrderCode;
        //                    empAssign.ShopOrderCode = item.ShopOrderCode;
        //                    empAssign.EmployeeCode = item.EmployeeCode;
        //                    empAssign.OrderStatus = item.OrderStatus;
        //                    empAssign.FromAddress = item.FromAddress;
        //                    empAssign.DeliveredType = item.DeliveredType;
        //                    empAssign.DeliveryType = item.DeliveryType;
        //                    empAssign.DeliveryDate = item.DeliveryDate;
        //                    empAssign.DeliverySchedule = item.DeliverySchedule;
        //                    empAssign.ToAddress = item.ToAddress;
        //                    empAssign.IsActive = item.IsActive;
        //                    // empAssign.DeliveredTime=null;
        //                    empAssign.CreateDate = CommonFunctions.GetLocalTime();
        //                    empAssign.CreateBy = item.CreateBy;
        //                    empAssign.NetworkIP = item.NetworkIP;
        //                    empAssign.DeviceType = item.DeviceType;
        //                    empAssign.DeviceID = item.DeviceID;
        //                    empAssign.X = item.X;
        //                    empAssign.Y = item.Y;

        //                    db2.EmployeeAssignment.Add(empAssign);
        //                }

        //                db2.SaveChanges();
        //                db2.Dispose();
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Unable to Insert Recored in EmployeeAssignment :- " + ex.InnerException);

        //            }
        //        }

        //    //-- End Insert into EmployeeAssignment table --//

        //    //-- Insert into EmployeeAssignmentHistory table --//
        //        //-- It will satisfied this query --//
        //       var employeeAssignHistoryFromMerchant = (from DOD in db.DeliveryOrderDetails
        //                                         join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                                         join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID
        //                                         join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                                         join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                         join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                         join S in db.Shops on COD.ShopID equals S.ID
        //                                         join EA in db.EmployeeAssignment on DOD.ShopOrderCode equals EA.ShopOrderCode
        //                                         where //DOD.ShopOrderCode == COD.ShopOrderCode// &&
        //                                              (COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.PACKED &&
        //                                              COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_SHOP) &&
        //                                               /*DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&*/
        //                                        ShopOrderCode.Contains(DOD.ShopOrderCode) && DOD.IsActive == true && COD.IsActive == true
        //                                        && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true && PD.IsActive == true
        //                                         select new
        //                                         {
        //                                             ID = DOD.ID,
        //                                             AssignID = EA.ID,
        //                                             OrderCode = CO.OrderCode,
        //                                             ShopOrderCode = DOD.ShopOrderCode,
        //                                             EmployeeCode = EmployeeName,
        //                                             OrderStatus = COD.OrderStatus,
        //                                             FromAddress = S.Address,
        //                                             DeliveredType = DeliverTo,
        //                                             DeliveryType = DOD.DeliveryType,
        //                                             DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                             DeliverySchedule = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                             ToAddress = CO.ShippingAddress,
        //                                             IsActive = COD.IsActive,
        //                                             // DeliveredTime=null,
        //                                             // CreateDate =CommonFunctions.GetLocalTime(),
        //                                             CreateBy = PD.ID,
        //                                             // ModifyDate = DOD.ModifyDate,
        //                                             // ModifyBy = DOD.ModifyBy,
        //                                             NetworkIP = DOD.NetworkIP,
        //                                             DeviceType = DOD.DeviceType,
        //                                             DeviceID = DOD.DeviceID,
        //                                             X = S.Lattitude,
        //                                             Y = S.Longitude
        //                                         }).Distinct();

        //        //-- OR it will satisfied this query --//
        //       var employeeAssignHistoryFromGodown = (from DOD in db.DeliveryOrderDetails
        //                                                join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                                                join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID
        //                                                join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                                                join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                                join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                                join S in db.Shops on COD.ShopID equals S.ID
        //                                                join EA in db.EmployeeAssignment on DOD.ShopOrderCode equals EA.ShopOrderCode
        //                                                where //DOD.ShopOrderCode == COD.ShopOrderCode// &&
        //                                                     (COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.IN_GODOWN &&
        //                                                     COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_GODOWN) &&
        //                                                     /*DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&*/
        //                                               ShopOrderCode.Contains(DOD.ShopOrderCode) && DOD.IsActive == true && COD.IsActive == true
        //                                               && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true && PD.IsActive == true
        //                                                select new
        //                                                {
        //                                                    ID = DOD.ID,
        //                                                    AssignID = EA.ID,
        //                                                    OrderCode = CO.OrderCode,
        //                                                    ShopOrderCode = DOD.ShopOrderCode,
        //                                                    EmployeeCode = EmployeeName,
        //                                                    OrderStatus = COD.OrderStatus,
        //                                                    FromAddress = DP.GodownAddress,
        //                                                    DeliveredType = DeliverTo,
        //                                                    DeliveryType = DOD.DeliveryType,
        //                                                    DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                                    DeliverySchedule = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                                    ToAddress = CO.ShippingAddress,
        //                                                    IsActive = COD.IsActive,
        //                                                    // DeliveredTime=null,
        //                                                    // CreateDate =CommonFunctions.GetLocalTime(),
        //                                                    CreateBy = PD.ID,
        //                                                    // ModifyDate = DOD.ModifyDate,
        //                                                    // ModifyBy = DOD.ModifyBy,
        //                                                    NetworkIP = DOD.NetworkIP,
        //                                                    DeviceType = DOD.DeviceType,
        //                                                    DeviceID = DOD.DeviceID,
        //                                                    X = S.Lattitude,
        //                                                    Y = S.Longitude
        //                                                }).Distinct();
        //       if (employeeAssignHistoryFromMerchant.Count() > 0)
        //    {
        //        try
        //        {
        //            EzeeloDBContext db3 = new EzeeloDBContext();
        //            foreach (var item in employeeAssignHistoryFromMerchant)
        //            {
        //                ModelLayer.Models.EmployeeAssignmentHistory empHistoryAssign = new ModelLayer.Models.EmployeeAssignmentHistory();
        //                empHistoryAssign.ID = 0;
        //                empHistoryAssign.AssignID = item.AssignID;
        //                empHistoryAssign.OrderCode = item.OrderCode;
        //                empHistoryAssign.ShopOrderCode = item.ShopOrderCode;
        //                empHistoryAssign.EmployeeCode = item.EmployeeCode;
        //                empHistoryAssign.OrderStatus = item.OrderStatus;
        //                empHistoryAssign.FromAddress = item.FromAddress;
        //                empHistoryAssign.DeliveredType = item.DeliveredType;
        //                empHistoryAssign.DeliveryType = item.DeliveryType;
        //                empHistoryAssign.DeliveryDate = item.DeliveryDate;
        //                empHistoryAssign.DeliverySchedule = item.DeliverySchedule;
        //                empHistoryAssign.ToAddress = item.ToAddress;
        //                empHistoryAssign.IsActive = item.IsActive;
        //                // empAssign.DeliveredTime=null;
        //                empHistoryAssign.CreateDate = CommonFunctions.GetLocalTime();
        //                empHistoryAssign.CreateBy = item.CreateBy;
        //                empHistoryAssign.NetworkIP = item.NetworkIP;
        //                empHistoryAssign.DeviceType = item.DeviceType;
        //                empHistoryAssign.DeviceID = item.DeviceID;
        //                empHistoryAssign.X = item.X;
        //                empHistoryAssign.Y = item.Y;

        //                db3.EmployeeAssignmentHistory.Add(empHistoryAssign);
        //            }

        //            db3.SaveChanges();
        //            db3.Dispose();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Unable to Insert Recored in EmployeeAssignmentHistory :- " + ex.InnerException);

        //        }

        //    }
        //       if (employeeAssignHistoryFromGodown.Count() > 0)
        //       {
        //           try
        //           {
        //               EzeeloDBContext db4 = new EzeeloDBContext();
        //               foreach (var item in employeeAssignHistoryFromGodown)
        //               {
        //                   ModelLayer.Models.EmployeeAssignmentHistory empHistoryAssign = new ModelLayer.Models.EmployeeAssignmentHistory();
        //                   empHistoryAssign.ID = 0;
        //                   empHistoryAssign.AssignID = item.AssignID;
        //                   empHistoryAssign.OrderCode = item.OrderCode;
        //                   empHistoryAssign.ShopOrderCode = item.ShopOrderCode;
        //                   empHistoryAssign.EmployeeCode = item.EmployeeCode;
        //                   empHistoryAssign.OrderStatus = item.OrderStatus;
        //                   empHistoryAssign.FromAddress = item.FromAddress;
        //                   empHistoryAssign.DeliveredType = item.DeliveredType;
        //                   empHistoryAssign.DeliveryType = item.DeliveryType;
        //                   empHistoryAssign.DeliveryDate = item.DeliveryDate;
        //                   empHistoryAssign.DeliverySchedule = item.DeliverySchedule;
        //                   empHistoryAssign.ToAddress = item.ToAddress;
        //                   empHistoryAssign.IsActive = item.IsActive;
        //                   // empAssign.DeliveredTime=null;
        //                   empHistoryAssign.CreateDate = CommonFunctions.GetLocalTime();
        //                   empHistoryAssign.CreateBy = item.CreateBy;
        //                   empHistoryAssign.NetworkIP = item.NetworkIP;
        //                   empHistoryAssign.DeviceType = item.DeviceType;
        //                   empHistoryAssign.DeviceID = item.DeviceID;
        //                   empHistoryAssign.X = item.X;
        //                   empHistoryAssign.Y = item.Y;

        //                   db4.EmployeeAssignmentHistory.Add(empHistoryAssign);
        //               }

        //               db4.SaveChanges();
        //               db4.Dispose();
        //           }
        //           catch (Exception ex)
        //           {
        //               throw new Exception("Unable to Insert Recored in EmployeeAssignmentHistory :- " + ex.InnerException);

        //           }

        //       }
        //    //-- End of Insert into EmployeeAssignmentHistory table --//
        //}

        //private void CustomerAssignUpdate(string EmployeeName, string DeliverTo, string ShopOrderCode, long UserLoginID)
        //{
        //    //-- Updated into EmployeeAssignment table --//
        //        //-- It will satisfied this query --//
        //    var employeeAssignFromShop = (from DOD in db.DeliveryOrderDetails
        //                                  join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                                  join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID
        //                                  join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                                  join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                  join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                  join S in db.Shops on COD.ShopID equals S.ID
        //                                  join PC in db.Pincodes on S.PincodeID equals PC.ID
        //                                  join EA in db.EmployeeAssignment on DOD.ShopOrderCode equals EA.ShopOrderCode
        //                                  where //DOD.ShopOrderCode == COD.ShopOrderCode// &&
        //                                        (COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.PACKED &&
        //                                         COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_SHOP) &&
        //                                      /* DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&*/
        //                                  ShopOrderCode.Contains(DOD.ShopOrderCode) && DOD.IsActive == true && COD.IsActive == true
        //                                 && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true && PD.IsActive == true
        //                                  select new
        //                                  {
        //                                      ID = EA.ID,
        //                                      OrderCode = CO.OrderCode,
        //                                      ShopOrderCode = DOD.ShopOrderCode,
        //                                      EmployeeCode = EmployeeName,
        //                                      OrderStatus = COD.OrderStatus,
        //                                      FromAddress = S.Address.ToUpper() + " - " + PC.Name,
        //                                      DeliveredType = DeliverTo,
        //                                      DeliveryType = DOD.DeliveryType,
        //                                      DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                      DeliverySchedule = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                      ToAddress = CO.ShippingAddress.ToUpper() + " - " + CO.PincodeID,
        //                                      IsActive = COD.IsActive,
        //                                      // DeliveredTime=null,
        //                                      CreateDate = EA.CreateDate,
        //                                      CreateBy = EA.CreateBy,
        //                                      // ModifyDate = DOD.ModifyDate,
        //                                      ModifyBy = PD.ID,
        //                                      NetworkIP = DOD.NetworkIP,
        //                                      DeviceType = DOD.DeviceType,
        //                                      DeviceID = DOD.DeviceID,
        //                                      X = S.Lattitude,
        //                                      Y = S.Longitude
        //                                  }).Distinct();
            
        //    //-- OR it will satisfied this query --//
        //    var employeeAssignFromGodown = (from DOD in db.DeliveryOrderDetails
        //                                      join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                                      join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID
        //                                      join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                                      join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                      join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                      join S in db.Shops on COD.ShopID equals S.ID
        //                                      join EA in db.EmployeeAssignment on DOD.ShopOrderCode equals EA.ShopOrderCode
        //                                      where //DOD.ShopOrderCode == COD.ShopOrderCode// &&
        //                                            (COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.IN_GODOWN &&
        //                                             COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_GODOWN) &&
        //                                          /* DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&*/
        //                                     ShopOrderCode.Contains(DOD.ShopOrderCode) && DOD.IsActive == true && COD.IsActive == true
        //                                     && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true && PD.IsActive == true
        //                                      select new
        //                                      {
        //                                          ID = EA.ID,
        //                                          OrderCode = CO.OrderCode,
        //                                          ShopOrderCode = DOD.ShopOrderCode,
        //                                          EmployeeCode = EmployeeName,
        //                                          OrderStatus = COD.OrderStatus,
        //                                          FromAddress = DP.GodownAddress.ToUpper() + " - " + DP.PincodeID,
        //                                          DeliveredType = DeliverTo,
        //                                          DeliveryType = DOD.DeliveryType,
        //                                          DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                          DeliverySchedule = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                          ToAddress = CO.ShippingAddress.ToUpper() + " - " + CO.PincodeID,
        //                                          IsActive = COD.IsActive,
        //                                          // DeliveredTime=null,
        //                                          CreateDate = EA.CreateDate,
        //                                          CreateBy = EA.CreateBy,
        //                                          // ModifyDate = DOD.ModifyDate,
        //                                          ModifyBy = PD.ID,
        //                                          NetworkIP = DOD.NetworkIP,
        //                                          DeviceType = DOD.DeviceType,
        //                                          DeviceID = DOD.DeviceID,
        //                                          X = S.Lattitude,
        //                                          Y = S.Longitude
        //                                      }).Distinct();

        //    if (employeeAssignFromShop.Count() > 0)
        //    {
        //        try
        //        {
        //            EzeeloDBContext db1 = new EzeeloDBContext();
        //            foreach (var item in employeeAssignFromShop)
        //            {
        //                ModelLayer.Models.EmployeeAssignment empAssign = new ModelLayer.Models.EmployeeAssignment();
        //                empAssign.ID = item.ID;
        //                empAssign.OrderCode = item.OrderCode;
        //                empAssign.ShopOrderCode = item.ShopOrderCode;
        //                empAssign.EmployeeCode = item.EmployeeCode;
        //                empAssign.OrderStatus = item.OrderStatus;
        //                empAssign.FromAddress = item.FromAddress;
        //                empAssign.DeliveredType = item.DeliveredType;
        //                empAssign.DeliveryType = item.DeliveryType;
        //                empAssign.DeliveryDate = item.DeliveryDate;
        //                empAssign.DeliverySchedule = item.DeliverySchedule;
        //                empAssign.ToAddress = item.ToAddress;
        //                empAssign.IsActive = item.IsActive;
        //                // empAssign.DeliveredTime=null;
        //                empAssign.CreateDate = item.CreateDate;
        //                empAssign.CreateBy = item.CreateBy;
        //                empAssign.ModifyDate = CommonFunctions.GetLocalTime();
        //                empAssign.ModifyBy = item.ModifyBy;

        //                empAssign.NetworkIP = item.NetworkIP;
        //                empAssign.DeviceType = item.DeviceType;
        //                empAssign.DeviceID = item.DeviceID;
        //                empAssign.X = item.X;
        //                empAssign.Y = item.Y;

        //                db1.Entry(empAssign).State = EntityState.Modified;
        //                // db1.Entry(empAssign).State = EntityState.Added;
        //                db1.SaveChanges();
        //            }
        //            db1.Dispose();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Unable to Update Recored in EmployeeAssignment :- " + ex.InnerException);

        //        }
        //    }
        //    if (employeeAssignFromGodown.Count() > 0)
        //    {
        //        try
        //        {
        //            EzeeloDBContext db2 = new EzeeloDBContext();
        //            foreach (var item in employeeAssignFromGodown)
        //            {
        //                ModelLayer.Models.EmployeeAssignment empAssign = new ModelLayer.Models.EmployeeAssignment();
        //                empAssign.ID = item.ID;
        //                empAssign.OrderCode = item.OrderCode;
        //                empAssign.ShopOrderCode = item.ShopOrderCode;
        //                empAssign.EmployeeCode = item.EmployeeCode;
        //                empAssign.OrderStatus = item.OrderStatus;
        //                empAssign.FromAddress = item.FromAddress;
        //                empAssign.DeliveredType = item.DeliveredType;
        //                empAssign.DeliveryType = item.DeliveryType;
        //                empAssign.DeliveryDate = item.DeliveryDate;
        //                empAssign.DeliverySchedule = item.DeliverySchedule;
        //                empAssign.ToAddress = item.ToAddress;
        //                empAssign.IsActive = item.IsActive;
        //                // empAssign.DeliveredTime=null;
        //                empAssign.CreateDate = item.CreateDate;
        //                empAssign.CreateBy = item.CreateBy;
        //                empAssign.ModifyDate = CommonFunctions.GetLocalTime();
        //                empAssign.ModifyBy = item.ModifyBy;

        //                empAssign.NetworkIP = item.NetworkIP;
        //                empAssign.DeviceType = item.DeviceType;
        //                empAssign.DeviceID = item.DeviceID;
        //                empAssign.X = item.X;
        //                empAssign.Y = item.Y;

        //                db2.Entry(empAssign).State = EntityState.Modified;
        //                // db1.Entry(empAssign).State = EntityState.Added;
        //                db2.SaveChanges();
        //            }
        //            db2.Dispose();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Unable to Update Recored in EmployeeAssignment :- " + ex.InnerException);

        //        }
        //    }
        //    //-- End Update into EmployeeAssignment table --//

        //    //-- Insert into EmployeeAssignmentHistory table --//
        //        //-- It will satisfied this query --//
        //    var employeeAssignHistoryFromShop = (from DOD in db.DeliveryOrderDetails
        //                                         join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                                         join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID
        //                                         join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                                         join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                         join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                         join S in db.Shops on COD.ShopID equals S.ID
        //                                         join PC in db.Pincodes on S.PincodeID equals PC.ID
        //                                         join EA in db.EmployeeAssignment on DOD.ShopOrderCode equals EA.ShopOrderCode
        //                                         where //DOD.ShopOrderCode == COD.ShopOrderCode// &&
        //                                              (COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.PACKED &&
        //                                                COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_SHOP) &&
        //                                             /* DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&*/
        //                                        ShopOrderCode.Contains(DOD.ShopOrderCode) && DOD.IsActive == true && COD.IsActive == true
        //                                        && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true && PD.IsActive == true
        //                                         select new
        //                                         {
        //                                             ID = 0,
        //                                             AssignID = EA.ID,
        //                                             OrderCode = CO.OrderCode,
        //                                             ShopOrderCode = DOD.ShopOrderCode,
        //                                             EmployeeCode = EmployeeName,
        //                                             OrderStatus = COD.OrderStatus,
        //                                             FromAddress = S.Address.ToUpper() + " - " + PC.Name,
        //                                             DeliveredType = DeliverTo,
        //                                             DeliveryType = DOD.DeliveryType,
        //                                             DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                             DeliverySchedule = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                             ToAddress = CO.ShippingAddress.ToUpper() + " - " + CO.PincodeID,
        //                                             IsActive = COD.IsActive,
        //                                             // DeliveredTime=null,
        //                                             //CreateDate =EAH.CreateDate,
        //                                             CreateBy = PD.ID,
        //                                             // ModifyDate = DOD.ModifyDate,
        //                                             // ModifyBy = PD.ID,
        //                                             NetworkIP = DOD.NetworkIP,
        //                                             DeviceType = DOD.DeviceType,
        //                                             DeviceID = DOD.DeviceID,
        //                                             X = S.Lattitude,
        //                                             Y = S.Longitude
        //                                         }).Distinct();
            
        //        //-- OR it will satisfied this query --//
        //    var employeeAssignHistoryFromGodown = (from DOD in db.DeliveryOrderDetails
        //                                         join COD in db.CustomerOrderDetails on DOD.ShopOrderCode equals COD.ShopOrderCode
        //                                         join CO in db.CustomerOrders on COD.CustomerOrderID equals CO.ID
        //                                         join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                                         join BD in db.BusinessDetails on DP.BusinessDetailID equals BD.ID
        //                                         join PD in db.PersonalDetails on BD.UserLoginID equals PD.UserLoginID
        //                                         join S in db.Shops on COD.ShopID equals S.ID
        //                                         join EA in db.EmployeeAssignment on DOD.ShopOrderCode equals EA.ShopOrderCode
        //                                         where //DOD.ShopOrderCode == COD.ShopOrderCode// &&
        //                                             (COD.OrderStatus >= (int)Common.Constant.ORDER_STATUS_ASSIGN.IN_GODOWN &&
        //                                             COD.OrderStatus <= (int)Common.Constant.ORDER_STATUS_ASSIGN.DISPATCHED_FROM_GODOWN) &&
        //                                             /* DOD.DeliveryPartnerID == deliveryPartnerSessionViewModel.DeliveryPartnerID &&*/
        //                                        ShopOrderCode.Contains(DOD.ShopOrderCode) && DOD.IsActive == true && COD.IsActive == true
        //                                        && DP.IsLive == true && DP.IsActive == true && BD.IsActive == true && PD.IsActive == true
        //                                         select new
        //                                         {
        //                                             ID = 0,
        //                                             AssignID = EA.ID,
        //                                             OrderCode = CO.OrderCode,
        //                                             ShopOrderCode = DOD.ShopOrderCode,
        //                                             EmployeeCode = EmployeeName,
        //                                             OrderStatus = COD.OrderStatus,
        //                                             FromAddress = DP.GodownAddress.ToUpper() + " - " + DP.PincodeID,
        //                                             DeliveredType = DeliverTo,
        //                                             DeliveryType = DOD.DeliveryType,
        //                                             DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
        //                                             DeliverySchedule = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
        //                                             ToAddress = CO.ShippingAddress.ToUpper() + " - " + CO.PincodeID,
        //                                             IsActive = COD.IsActive,
        //                                             // DeliveredTime=null,
        //                                             //CreateDate =EAH.CreateDate,
        //                                             CreateBy = PD.ID,
        //                                             // ModifyDate = DOD.ModifyDate,
        //                                             // ModifyBy = PD.ID,
        //                                             NetworkIP = DOD.NetworkIP,
        //                                             DeviceType = DOD.DeviceType,
        //                                             DeviceID = DOD.DeviceID,
        //                                             X = S.Lattitude,
        //                                             Y = S.Longitude
        //                                         }).Distinct();

        //    if (employeeAssignHistoryFromShop.Count() > 0)
        //    {
        //        try
        //        {
        //            EzeeloDBContext db3 = new EzeeloDBContext();
        //            foreach (var item in employeeAssignHistoryFromShop)
        //            {
        //                ModelLayer.Models.EmployeeAssignmentHistory empHistoryAssign = new ModelLayer.Models.EmployeeAssignmentHistory();
        //                empHistoryAssign.ID = item.ID;
        //                empHistoryAssign.AssignID = item.AssignID;
        //                empHistoryAssign.OrderCode = item.OrderCode;
        //                empHistoryAssign.ShopOrderCode = item.ShopOrderCode;
        //                empHistoryAssign.EmployeeCode = item.EmployeeCode;
        //                empHistoryAssign.OrderStatus = item.OrderStatus;
        //                empHistoryAssign.FromAddress = item.FromAddress;
        //                empHistoryAssign.DeliveredType = item.DeliveredType;
        //                empHistoryAssign.DeliveryType = item.DeliveryType;
        //                empHistoryAssign.DeliveryDate = item.DeliveryDate;
        //                empHistoryAssign.DeliverySchedule = item.DeliverySchedule;
        //                empHistoryAssign.ToAddress = item.ToAddress;
        //                empHistoryAssign.IsActive = item.IsActive;
        //                // empAssign.DeliveredTime=null;
        //                empHistoryAssign.CreateDate = CommonFunctions.GetLocalTime();
        //                empHistoryAssign.CreateBy = item.CreateBy;
        //                //empHistoryAssign.ModifyDate = CommonFunctions.GetLocalTime();
        //                //empHistoryAssign.ModifyBy = item.ModifyBy;

        //                empHistoryAssign.NetworkIP = item.NetworkIP;
        //                empHistoryAssign.DeviceType = item.DeviceType;
        //                empHistoryAssign.DeviceID = item.DeviceID;
        //                empHistoryAssign.X = item.X;
        //                empHistoryAssign.Y = item.Y;

        //                // db2.Entry(empHistoryAssign).State = EntityState.Modified;
        //                //db2.SaveChanges();
        //                db3.EmployeeAssignmentHistory.Add(empHistoryAssign);

        //            }

        //            db3.SaveChanges();
        //            db3.Dispose();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Unable to Insert Recored in EmployeeAssignmentHistory :- " + ex.InnerException);

        //        }
        //    }
        //    if (employeeAssignHistoryFromGodown.Count() > 0)
        //    {
        //        try
        //        {
        //            EzeeloDBContext db4 = new EzeeloDBContext();
        //            foreach (var item in employeeAssignHistoryFromGodown)
        //            {
        //                ModelLayer.Models.EmployeeAssignmentHistory empHistoryAssign = new ModelLayer.Models.EmployeeAssignmentHistory();
        //                empHistoryAssign.ID = item.ID;
        //                empHistoryAssign.AssignID = item.AssignID;
        //                empHistoryAssign.OrderCode = item.OrderCode;
        //                empHistoryAssign.ShopOrderCode = item.ShopOrderCode;
        //                empHistoryAssign.EmployeeCode = item.EmployeeCode;
        //                empHistoryAssign.OrderStatus = item.OrderStatus;
        //                empHistoryAssign.FromAddress = item.FromAddress;
        //                empHistoryAssign.DeliveredType = item.DeliveredType;
        //                empHistoryAssign.DeliveryType = item.DeliveryType;
        //                empHistoryAssign.DeliveryDate = item.DeliveryDate;
        //                empHistoryAssign.DeliverySchedule = item.DeliverySchedule;
        //                empHistoryAssign.ToAddress = item.ToAddress;
        //                empHistoryAssign.IsActive = item.IsActive;
        //                // empAssign.DeliveredTime=null;
        //                empHistoryAssign.CreateDate = CommonFunctions.GetLocalTime();
        //                empHistoryAssign.CreateBy = item.CreateBy;
        //                //empHistoryAssign.ModifyDate = CommonFunctions.GetLocalTime();
        //                //empHistoryAssign.ModifyBy = item.ModifyBy;

        //                empHistoryAssign.NetworkIP = item.NetworkIP;
        //                empHistoryAssign.DeviceType = item.DeviceType;
        //                empHistoryAssign.DeviceID = item.DeviceID;
        //                empHistoryAssign.X = item.X;
        //                empHistoryAssign.Y = item.Y;

        //                // db2.Entry(empHistoryAssign).State = EntityState.Modified;
        //                //db2.SaveChanges();
        //                db4.EmployeeAssignmentHistory.Add(empHistoryAssign);

        //            }

        //            db4.SaveChanges();
        //            db4.Dispose();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Unable to Insert Recored in EmployeeAssignmentHistory :- " + ex.InnerException);

        //        }
        //    }
        //    //-- End of Insert into EmployeeAssignmentHistory table --//
        //}

        //#endregion
    
    //--------------------------------------------------------------------------------------------------------------------------------------
    }

}
