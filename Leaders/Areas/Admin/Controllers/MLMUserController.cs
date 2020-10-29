using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using ModelLayer.Models.ViewModel;
using System.Data.SqlClient;
using System.Data;
using Leaders.Filter;

namespace Leaders.Areas.Admin.Controllers
{
    [AdminSessionExpire]
    public class MLMUserController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult ListBetweenDate(string startDate, string endDate)
        {

            DateTime sDate = Convert.ToDateTime(startDate);
            DateTime eDate = Convert.ToDateTime(endDate);
            var mlmList = GetUser().Where(x => x.CreateDate >= sDate && x.CreateDate <= eDate).ToList();
            return PartialView("partialUserList", mlmList);
        }

        public ActionResult partialUserList()
        {
            //DateTime now = DateTime.Now;
            //var startDate = new DateTime(now.Year, now.Month, 1);
            //var endDate = startDate.AddMonths(1).AddDays(-1);
            var mlmList = GetUser().ToList();

            return PartialView("partialUserList", mlmList);

        }
        public ActionResult UserList()
        {
            return View();
        }

        public IList<MLMUserViewModel> GetUser()
        {
            //Decimal? currentQRP = db.QRPMasters.Select(y => y.Current_QRP).FirstOrDefault();
            //int currentMonth = DateTime.Now.Month;
            List<MLMUserViewModel> userList = new List<MLMUserViewModel>();

            userList = db.Database.SqlQuery<MLMUserViewModel>("EXEC Sp_MLMUsers_Leaders").ToList<MLMUserViewModel>();

            //userList = db.UserLogins.Join(db.PersonalDetails.Where(h => h.IsActive == true), u => u.ID, p => p.UserLoginID,
            //(u, p) => new
            //{
            //    UserID = u.ID,
            //    FirstName = p.FirstName,
            //    MiddelName = p.MiddleName,
            //    LastName = (p.LastName == null ? "" : p.LastName),
            //    Email = u.Email,
            //    Mobile = u.Mobile,
            //    CreateDate = u.CreateDate

            //}).Join(db.MLMUsers, d => d.UserID, m => m.UserID,
            //(d, m) => new MLMUserViewModel
            //{



            //    UserID = d.UserID,
            //    FullName = d.FirstName + " " + d.LastName,

            //    Email = d.Email,
            //    Mobile = d.Mobile,
            //    Ref_Id = m.Ref_Id,
            //    Join_date_ref = m.Join_date_ref,
            //    Status_ref = m.Status_ref,
            //    Activate_date_ref = m.Activate_date_ref,
            //    Refered_Id_ref = m.Refered_Id_ref,
            //    request = m.request,
            //    request_active = m.request_active,
            //    CreateDate = d.CreateDate,

            //    //ParentName = UserDesignation(Convert.ToInt32(GetUpLine(d.UserID))),

            //RP = db.MLMWalletTransactions.Where(p => p.UserLoginID == d.UserID && p.CreateDate.Month == currentMonth && p.TransactionTypeID == 7 && p.OrderAmount > 0).Sum(y => y.TransactionPoints),
            //    LeftQRP = (currentQRP - db.MLMWalletTransactions.Where(p => p.UserLoginID == d.UserID && p.CreateDate.Month == currentMonth && p.TransactionTypeID == 7 && p.OrderAmount > 0).Sum(y => y.TransactionPoints)),
            //    ERP = db.MLMWalletTransactions.Where(p => p.UserLoginID == d.UserID && p.TransactionTypeID == 11 && p.CreateDate.Month == currentMonth).Sum(y => y.TransactionPoints)

            //}

            //).OrderByDescending(x => x.CreateDate).ToList();
            ////Started Added for code optimization by sonali on 12-04-2019
            //List<string> Refered_Id_refList = userList.Select(x => x.Refered_Id_ref).Distinct().ToList();
            //List<MLMUser> MlmUserList = db.MLMUsers.Where(x => Refered_Id_refList.Contains(x.Ref_Id)).ToList();
            //List<PersonalDetail> PersonalDetailsList = new List<PersonalDetail>();
            //if (MlmUserList != null && MlmUserList.Count > 0)
            //{
            //    List<long> Ids = MlmUserList.Select(x => x.UserID).Distinct().ToList();
            //    PersonalDetailsList = db.PersonalDetails.Where(x => Ids.Contains(x.UserLoginID)).ToList();
            //}
            ////Ended Added for code optimization by sonali on 12-04-2019
            //foreach (var item in userList)
            //{
            //    if ( item.RP == null)
            //    {
            //        item.RP = 0;
            //    }
            //    if (item.ERP == null)
            //    {
            //        item.ERP = 0;
            //    }
            //    if (item.LeftQRP == null)
            //    {
            //        item.LeftQRP = 0;
            //    }
            //    if(item.Refered_Id_ref != null)
            //    {
            //        long UID = MlmUserList.Where(x => x.Ref_Id == item.Refered_Id_ref).Select(x => x.UserID).FirstOrDefault();//Added by sonali for code optimization on 12-04-2019
            //        item.ParentName = PersonalDetailsList.Where(x => x.UserLoginID == UID).Select(x => x.FirstName).FirstOrDefault();//Added by sonali for code optimization on 12-04-2019
            //        item.Designation = UserDesignation(Convert.ToInt32(item.UserID));
            //        //item.ParentName = db.PersonalDetails.Where(x => x.UserLoginID == UID).Select(x => x.FirstName).FirstOrDefault();
            //    }   //item.Level = userlevelget(Convert.ToInt32(item.UserID));
            //        //long UID = db.MLMUsers.Where(b => b.Ref_Id == item.Refered_Id_ref).Select(x => x.UserID).FirstOrDefault();

            //}

            return userList;
        }



      long GetUpLine(long UserLoginId)
{
    try
    {
        MLMUser obj = db.MLMUsers.FirstOrDefault(q => q.Ref_Id == db.MLMUsers.FirstOrDefault(p => p.UserID == UserLoginId).Refered_Id_ref);
        return (obj == null) ? 0 : obj.UserID;
    }
    catch (Exception ex)
    {
        throw new Exception("MLMWalletPoints : GetUpLine() " + ex.Message);
    }
}


public string UserDesignation(int LoginUserId)
        {

            string Designation = string.Empty;
            long value = 0; 
            try
            {
                //value = db.MLMUsers.FirstOrDefault(q => q.UserID == UserLoginId).islifestyleachiever;
                value = db.MLMUsers.FirstOrDefault(x => x.UserID == LoginUserId).CURRENTMONTHDESIGNTAIONID;

                if (value == 0)
                {
                    Designation = "";
                }
                else
                {
                    Designation = db.DesignationMaster.FirstOrDefault(x => x.Id == value).Designation;

                    return Designation;
                }

            }
            catch (Exception ex)
            {

            }


            return Designation;
        }

        public int userlevelget(int LoginUserId)
        {

            int userlevel = 0;
            long value = 0;
            try
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "UserID",
                    Value = LoginUserId

                };

                var TotalPoints = new SqlParameter
                {
                    ParameterName = "TotalPoints",
                    Direction = ParameterDirection.Output,
                    DbType = DbType.Int32,
                    Precision = 18,
                    Scale = 4
                };
                db.Database.ExecuteSqlCommand("Leaders_NetworkUsers_Level @UserID,@TotalPoints output", idParam, TotalPoints);
                if (TotalPoints != null)
                {
                    userlevel = Convert.ToInt32(TotalPoints.Value);
                }

                //db.Database.ExecuteSqlCommand("Leaders_NetworkUsers @UserID", idParam);

                //var report2List = db.Database.SqlQuery<NetworkReportViewModel>("Leaders_NetworkUsers @UserID", idParam).ToList<NetworkReportViewModel>();

            }
            catch (Exception ex)
            {

            }


            return userlevel;
        }




        public ActionResult LeaderUserList()
        {
            return Json(new { data = GetUser() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportToExcel(string fromDate, string toDate)
        {
            List<MLMUserViewModel> uList = GetUser().ToList();
            ExcelLeadersUserViewModel objExcel = new ExcelLeadersUserViewModel();
            List<ExcelLeadersUserViewModel> leadersList = new List<ExcelLeadersUserViewModel>();
            if (toDate != "" && fromDate != "")
            {
                DateTime sDate = Convert.ToDateTime(fromDate);
                DateTime eDate = Convert.ToDateTime(toDate);

                if (fromDate == toDate)
                {
                    var SDate = new DateTime(sDate.Year, sDate.Month, sDate.Day, sDate.Hour, sDate.Minute, 1);
                    var EDate = SDate.AddDays(1).AddMinutes(-1);
                    uList = GetUser().Where(x => x.CreateDate >= SDate && x.CreateDate <= EDate).ToList();
                }


                uList = GetUser().Where(x => x.CreateDate >= sDate && x.CreateDate <= eDate).ToList();
            }
            foreach (var item in uList)
            {

                leadersList.Add(new ExcelLeadersUserViewModel
                {

                    FullName = item.FullName,
                    Email = item.Email,
                    CreateDate = item.CreateDate,
                    Activate_date_ref = item.Activate_date_ref,
                    Join_date_ref = item.Join_date_ref,
                    Mobile = item.Mobile,
                    Ref_Id = item.Ref_Id,
                    Refered_Id_ref = item.Refered_Id_ref,
                    RP = item.RP ?? 0,
                    ERP = item.ERP ?? 0,
                    LeftQRP = item.LeftQRP ?? 0

                });

            }

            leadersList.Add(objExcel);


            var gv = new GridView();
            gv.DataSource = leadersList;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=UserListExcel.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("UserList");
        }
    }
}