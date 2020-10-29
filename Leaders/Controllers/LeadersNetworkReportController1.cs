using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using Leaders.Filter;

namespace Leaders.Controllers
{
    public class LeadersNetworkReportController : Controller
    {
        //
        // GET: /LeadersNetworkReport/
        private GANDHIBAGHV22Context db = new GANDHIBAGHV22Context();
        private const int pageSize = 10;
        [SessionExpire]
        public ActionResult TablurView()
        {
            LeadersTabularViewParameterViewModel Param = new LeadersTabularViewParameterViewModel();
            Param.LevelId = 0;
            Param.LevelList = new SelectList(new[] 
            {
                new { ID = "0", Name = "All Levels" },
                new { ID = "1", Name = "Level: 1" },
                 new { ID = "2", Name = "Level: 2" },
                new { ID = "3", Name = "Level: 3" },
                 new { ID = "4", Name = "Level: 4" }
            },
            "ID", "Name", 1);
            Param.InActive = true;
            Param.Active = true;
            Param.Name = true;
            Param.EMail = true;
            Param.ParaenttName = true;
            Param.RefId = true;
            Param.JoinDate = true;
            Param.pageNo = 1;
            Param.list = GetData();
            return View(Param);
        }
        [HttpPost]
        public ActionResult GetReportTablurView(LeadersTabularViewParameterViewModel Param)
        {
            List<TabularView> list = GetData();
            if (Param.Active == true && Param.InActive == false)
            {
                list = list.Where(p => p.status == 1).ToList();
            }
            else if (Param.Active == false && Param.InActive == true)
            {
                list = list.Where(p => p.status == 0).ToList();
            }
            else
            {
                list = list.Where(p => p.status == 1 || p.status == 0).ToList();
            }

            if (Param.LevelId != 0)
            {
                list = list.Where(p => p.Level == Param.LevelId).ToList();
            }
            if (Param.SearchBy != "" && Param.SearchBy != null)
            {
                Param.SearchBy = Param.SearchBy.ToLower();
                if (Param.Name == true && Param.EMail == true && Param.RefId == true && Param.ParaenttName == true)
                {
                    list = list.Where(p => p.Name.ToLower().Contains(Param.SearchBy)
                        || p.Email.ToLower().Contains(Param.SearchBy)
                        || p.RefferalId.ToLower().Contains(Param.SearchBy)
                        || p.ParentName.ToLower().Contains(Param.SearchBy)
                        ).ToList();
                }
                else
                    if (Param.Name == true && Param.EMail == false && Param.RefId == false && Param.ParaenttName == false)
                    {
                        list = list.Where(p => p.Name.ToLower().Contains(Param.SearchBy)
                            ).ToList();
                    }
                    else
                        if (Param.Name == true && Param.EMail == true && Param.RefId == false && Param.ParaenttName == false)
                        {
                            list = list.Where(p => p.Name.ToLower().Contains(Param.SearchBy)
                                || p.Email.ToLower().Contains(Param.SearchBy)
                                ).ToList();
                        }
                        else
                            if (Param.Name == true && Param.EMail == true && Param.RefId == true && Param.ParaenttName == false)
                            {
                                list = list.Where(p => p.Name.ToLower().Contains(Param.SearchBy)
                                    || p.Email.ToLower().Contains(Param.SearchBy)
                                     || p.RefferalId.ToLower().Contains(Param.SearchBy)
                                    ).ToList();
                            }
                            else
                                if (Param.Name == true && Param.EMail == false && Param.RefId == false && Param.ParaenttName == true)
                                {
                                    list = list.Where(p => p.Name.ToLower().Contains(Param.SearchBy)
                                       || p.ParentName.ToLower().Contains(Param.SearchBy)
                                        ).ToList();
                                }
                                else
                                    if (Param.Name == true && Param.EMail == false && Param.RefId == true && Param.ParaenttName == false)
                                    {
                                        list = list.Where(p => p.Name.ToLower().Contains(Param.SearchBy)
                                           || p.RefferalId.ToLower().Contains(Param.SearchBy)
                                            ).ToList();
                                    }
                                    else if (Param.Name == false && Param.EMail == true && Param.RefId == false && Param.ParaenttName == false)
                                    {
                                        list = list.Where(p =>
                                             p.Email.ToLower().Contains(Param.SearchBy)
                                            ).ToList();
                                    }
                                    else if (Param.Name == false && Param.EMail == false && Param.RefId == true && Param.ParaenttName == false)
                                    {
                                        list = list.Where(p =>
                                             p.RefferalId.ToLower().Contains(Param.SearchBy)
                                            ).ToList();
                                    }
                                    else if (Param.Name == false && Param.EMail == false && Param.RefId == false && Param.ParaenttName == true)
                                    {
                                        list = list.Where(p =>
                                             p.ParentName.ToLower().Contains(Param.SearchBy)
                                            ).ToList();
                                    }
else  if (Param.Name == false && Param.EMail == false && Param.RefId == false && Param.ParaenttName == false && Param.JoinDate == true)
                {
                    DateTime dt = DateTime.Now.Date;
                    try
                    {
                        dt = Convert.ToDateTime(Param.SearchBy);
                    }
                    catch
                    {
                        string[] splitdt;
                        splitdt = Param.SearchBy.ToString().Split('-');
                        if (splitdt.Count() != 0)
                        {
                            dt = new DateTime(Convert.ToInt16(splitdt[2]), Convert.ToInt16(splitdt[1]), Convert.ToInt16(splitdt[0]));
                        }

                        splitdt = Param.JoinDate.ToString().Split('/');
                        if (splitdt.Count() != 0)
                        {
                            dt = new DateTime(Convert.ToInt16(splitdt[2]), Convert.ToInt16(splitdt[1]), Convert.ToInt16(splitdt[0]));
                        }
                    }
                    try
                    {
                        DateTime FromDate = dt;
                        DateTime ToDate = FromDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                        list = list.Where(p =>
                                              p.JoinDate >= FromDate && p.JoinDate <= ToDate
                                            ).ToList();
                    }
                    catch
                    {

                    }
                }
            }
            if (Param.pageNo == 0)
            {
                Param.pageNo = 1;
            }
            var model = list.ToPagedList(Param.pageNo, pageSize);
            return PartialView("_ReportTable", model);
        }




        public List<TabularView> GetData()
        {
            List<TabularView> list = new List<TabularView>();
            long LoginUserId = 0;
            if (Session["ID"] != null)
            {
                LoginUserId = Convert.ToInt64(Session["ID"]);
            }
            var idParam = new SqlParameter
            {
                ParameterName = "UserID",
                Value = LoginUserId
            };
            var Hour = new SqlParameter
            {
                ParameterName = "Hour",
                Value = WebConfigurationManager.AppSettings["Del_Hour"]
            };
            list = db.Database.SqlQuery<TabularView>("EXEC Leaders_NetworkReport @UserID, @Hour", idParam, Hour).ToList<TabularView>();
            return list;
        }
        [SessionExpire]
        public ActionResult ListView()
        {
            long userID = Convert.ToInt64(Session["ID"]);
            LeadersTabularViewParameterViewModel Param = new LeadersTabularViewParameterViewModel();
            Param.LevelId = 0;
            Param.LevelList = new SelectList(new[] 
            {
                new { ID = "0", Name = "All Levels" },
                new { ID = "1", Name = "Level: 1" },
                 new { ID = "2", Name = "Level: 2" },
                new { ID = "3", Name = "Level: 3" },
                 new { ID = "4", Name = "Level: 4" }
            },

            "ID", "Name", 1);
            Param.InActive = true;
            Param.Active = true;
            Param.Name = true;
            Param.EMail = true;
            Param.ParaenttName = true;
            Param.RefId = true;
            Param.JoinDate = true;
            Param.pageNo = 1;
            Param.list = GetData();

            List<TabularView> list = GetData();



            List<TabularView> level1 = list.Where(p => p.Level == 1)
                                    .ToList();


            foreach (var item in level1)
            {
                List<TabularView> list_ = list.Where(p => p.ParentId == item.UserLoginId).ToList();
                item.DownLineLevel = list_;

                foreach (var item1 in list_)
                {
                    List<TabularView> list__ = list.Where(p => p.ParentId == item1.UserLoginId).ToList();
                    item.DownLineLevel2 = list__;

                    foreach (var item2 in list__)
                    {
                        List<TabularView> list___ = list.Where(p => p.ParentId == item2.UserLoginId).ToList();
                        item.DownLineLevel3 = list___;

                        foreach (var item3 in list___)
                        {
                            List<TabularView> list____ = list.Where(p => p.ParentId == item3.UserLoginId).ToList();
                            item.DownLineLevel4 = list____;
                        }
                    }
                }
            }


            ViewBag.Email = db.UserLogins.Where(x => x.ID == userID).Select(y => y.Email).FirstOrDefault();
            ViewBag.RefferedBy = db.MLMUsers.Where(x => x.UserID == userID).Select(x => x.Refered_Id_ref).FirstOrDefault();
            ViewBag.JoinDate = db.MLMUsers.Where(x => x.UserID == userID).Select(y => y.Join_date_ref).FirstOrDefault();
            ViewBag.LastTransaction = db.MLMWalletTransactions.Where(x => x.UserLoginID == userID && x.TransactionTypeID == 1).Select(y => y.CreateDate).FirstOrDefault();
            ViewBag.FullName = db.PersonalDetails.Where(x => x.UserLoginID == userID).Select(y => y.FirstName + " " + y.LastName).FirstOrDefault();
            ViewBag.RefferalID = db.MLMUsers.Where(y => y.UserID == userID).Select(x => x.Ref_Id).FirstOrDefault();


            DashboardController objDashboard = new DashboardController();
            objDashboard.InitializeController(this.Request.RequestContext);

            ViewBag.TotalMembers = objDashboard.GetTotalMemberCount();
            ViewBag.QRP = objDashboard.GetQRP();

            return View(level1);
        }
    }
}