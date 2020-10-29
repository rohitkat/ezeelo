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
using BusinessLogicLayer;

namespace Leaders.Controllers
{
    public class LeadersNetworkReportController : Controller
    {

        private EzeeloDBContext db = new EzeeloDBContext();
        private LeadersDashboard objLeaderDashboard = new LeadersDashboard();//Added By Sonali for commonFunction on 04-02-2019
        private const int pageSize = 10;

        // DashboardController objDashboardController = new DashboardController();//Added By Sonali for commonFunction on 04-02-2019

        [SessionExpire]
        public ActionResult ListView()
        {
            long userID = Convert.ToInt64(Session["ID"]);

            //objLeaderDashboard.Distribute_Designation(Convert.ToInt32(userID));

            LeadersTabularViewParameterViewModel Param = new LeadersTabularViewParameterViewModel();
            Param.LevelId = 0;
            Param.LevelList = new SelectList(new[]
            {
                new { ID = "0", Name = "All Levels" },
                new { ID = "1", Name = "Level: 1" },
                new { ID = "2", Name = "Level: 2" },
                new { ID = "3", Name = "Level: 3" },
                new { ID = "4", Name = "Level: 4" },
                new { ID = "5", Name = "Level: 5" },
                new { ID = "6", Name = "Level: 6" }
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

            List<TabularView> list = Param.list;

            long LoginUserId = 0;
            LoginUserId = Convert.ToInt64(Session["ID"]);

                // DashboardController objDashboardController = new DashboardController();//Added By Sonali for commonFunction on 04-02-2019
                //objDashboardController.InitializeController(this.Request.RequestContext);//Added By Sonali for commonFunction on 04-02-2019
           
            
            
            //    foreach (var objlist in list)
            //{
            //    objlist.Mobile = db.UserLogins.Where(x => x.ID == objlist.UserLoginId).Select(y => y.Mobile).FirstOrDefault();
            //    objlist.ERPbySP = objLeaderDashboard.getERPBySP(objlist.UserLoginId, 1, 1, 0);//Added By Sonali for commonFunction on 04-02-2019
            //    objlist.Designation = objLeaderDashboard.UserDesignation(Convert.ToInt32(objlist.UserLoginId));

            //}
            //--------added by amit on 2-1-19 for display mobile------//
            LeadersMobileDisplay_Downline displayMobile = db.LeadersMobileDisplay_Downlines.FirstOrDefault();

            if (displayMobile.IsMobileDisplay == true)
            {
                Session["DisplayMobile"] = 1;
            }

            if (displayMobile.IsEmailDisplay == true)
            {
                Session["DisplayEmail"] = 1;
            }

            //--------End by amit on 2-1-19 for display mobile------//
            List<TabularView> level1 = list.Where(p => p.Level == 1)
                                    .ToList();
            List<TabularView> level2 = list.Where(p => p.Level == 2).ToList();

            foreach (var item in level1)
            {
                List<TabularView> list_ = list.Where(p => p.ParentId == item.UserLoginId).ToList();

                item.DownLineLevel = list_;

                foreach (var item1 in list_)
                {
                    List<TabularView> list__ = list.Where(p => p.ParentId == item1.UserLoginId).ToList();
                    item1.DownLineLevel2 = list__;

                    foreach (var item2 in list__)
                    {
                        List<TabularView> list___ = list.Where(p => p.ParentId == item2.UserLoginId).ToList();
                        item2.DownLineLevel3 = list___;

                        foreach (var item3 in list___)
                        {
                            List<TabularView> list____ = list.Where(p => p.ParentId == item3.UserLoginId).ToList();
                            item3.DownLineLevel4 = list____;

                            foreach (var item4 in list____)
                            {
                                List<TabularView> list_____ = list.Where(p => p.ParentId == item4.UserLoginId).ToList();
                                item4.DownLineLevel5 = list_____;

                                foreach (var item5 in list_____)
                                {
                                    List<TabularView> list______ = list.Where(p => p.ParentId == item5.UserLoginId).ToList();
                                    item5.DownLineLevel6 = list______;

                                }


                            }

                        }
                    }
                }
            }


            //foreach (var item in level1)
            //{
            //    List<TabularView> list_ = list.Where(p => p.ParentId == item.UserLoginId).ToList();

            //    item.DownLineLevel = list_;

            //    foreach (var item1 in list_)
            //    {
            //        List<TabularView> list__ = list.Where(p => p.ParentId == item1.UserLoginId).ToList();
            //        item.DownLineLevel2 = list__;

            //        foreach (var item2 in item.DownLineLevel2)
            //        {
            //            List<TabularView> list___ = list.Where(p => p.ParentId == item2.UserLoginId).ToList();
            //            item.DownLineLevel3 = list___;

            //            foreach (var item3 in list___)
            //            {
            //                List<TabularView> list____ = list.Where(p => p.ParentId == item3.UserLoginId).ToList();
            //                item.DownLineLevel4 = list____;
            //            }
            //        }
            //    }
            //}


            ViewBag.Email = db.UserLogins.Where(x => x.ID == userID).Select(y => y.Email).FirstOrDefault();
            ViewBag.RefferedBy = db.MLMUsers.Where(x => x.UserID == userID).Select(x => x.Refered_Id_ref).FirstOrDefault();
            ViewBag.JoinDate = db.MLMUsers.Where(x => x.UserID == userID).Select(y => y.Join_date_ref).FirstOrDefault();
            ViewBag.LastTransaction = db.CustomerOrders.Where(x => x.UserLoginID == userID ).OrderByDescending(x => x.CreateDate).Select(y => y.CreateDate).FirstOrDefault().ToString(string.Format("dd/MM/yyyy"));


            ViewBag.FullName = db.PersonalDetails.Where(x => x.UserLoginID == userID).Select(y => y.FirstName + " " + y.LastName).FirstOrDefault();
            ViewBag.RefferalID = db.MLMUsers.Where(y => y.UserID == userID).Select(x => x.Ref_Id).FirstOrDefault();

            ViewBag.TotalMembers = objLeaderDashboard.GetTotalMemberCount(userID);//Added By Sonali for call commonFunction on 04-02-2019
            ViewBag.QRP = objLeaderDashboard.GetQRP(userID);//Added By Sonali for call commonFunction on 04-02-2019
            ViewBag.ERP = objLeaderDashboard.getERPBySP(userID, 1, 1, 0);//Added By Sonali for call commonFunction on 04-02-2019
            ViewBag.Designation = objLeaderDashboard.UserDesignation(Convert.ToInt32(userID));

            return View(level1);
        }

        [SessionExpire]
        public ActionResult TablurView()
        {
            LeadersTabularViewParameterViewModel Param = new LeadersTabularViewParameterViewModel();
            //long userID = Convert.ToInt64(Session["ID"]);
            //objLeaderDashboard.Distribute_Designation(Convert.ToInt32(userID));
            Param.LevelId = 0;
            Param.LevelList = new SelectList(new[]
           {
                 new { ID = "0", Name = "All Levels" },
                 new { ID = "1", Name = "Level: 1" },
                 new { ID = "2", Name = "Level: 2" },
                 new { ID = "3", Name = "Level: 3" },
                 new { ID = "4", Name = "Level: 4" },

                 new { ID = "5", Name = "Level: 5" },
                 new { ID = "6", Name = "Level: 6" }
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
            // Param.list = GetData();
            //----amit----//
            List<TabularView> list = GetData();
            //foreach (var item in list)
            //{
            //   // item.Mobile = db.UserLogins.Where(x => x.ID == item.UserLoginId).Select(y => y.Mobile).FirstOrDefault();
            //    item.ERPbySP = objLeaderDashboard.getERPBySP(item.UserLoginId, 1, 1, 0);//Added By Sonali for commonFunction on 04-02-2019
            //   // item.Designation = objLeaderDashboard.UserDesignation(Convert.ToInt32(item.UserLoginId));

            //}
            Param.list = list;
            //----end----//


            //--------added by amit on 2-1-19 for display email------//
            LeadersMobileDisplay_Downline displayMobile = db.LeadersMobileDisplay_Downlines.FirstOrDefault();

            if (displayMobile.IsMobileDisplay == true)
            {
                Session["DisplayMobile"] = 1;
            }

            if (displayMobile.IsEmailDisplay == true)
            {
                Session["DisplayEmail"] = 1;
            }

            //--------End by amit on 2-1-19 for display email------//
            return View(Param);
        }




        public ActionResult Downlinestatus()
        {

            LeadersTabularViewParameterViewModel Param = new LeadersTabularViewParameterViewModel();

            //long userID = Convert.ToInt64(Session["ID"]);
            //objLeaderDashboard.Distribute_Designation(Convert.ToInt32(userID));

            Param.LevelId = 0;
            Param.LevelList = new SelectList(new[]
           {
                 new { ID = "0", Name = "All Levels" },
                 new { ID = "1", Name = "Level: 1" },
                 new { ID = "2", Name = "Level: 2" },
                 new { ID = "3", Name = "Level: 3" },
                 new { ID = "4", Name = "Level: 4" },
                 new { ID = "5", Name = "Level: 5" },
                 new { ID = "6", Name = "Level: 6" }
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
         
            List<Downline> list = GetDataDownline();
            foreach (var item in list)
  
            Param.listDownline = list;
           
            LeadersMobileDisplay_Downline displayMobile = db.LeadersMobileDisplay_Downlines.FirstOrDefault();

            if (displayMobile.IsMobileDisplay == true)
            {
                Session["DisplayMobile"] = 1;
            }

            if (displayMobile.IsEmailDisplay == true)
            {
                Session["DisplayEmail"] = 1;
            }

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
                else if (Param.Name == false && Param.EMail == false && Param.RefId == false && Param.ParaenttName == false && Param.JoinDate == true)
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

            ////-----------amit--------------//
            //foreach (var itemList in list)
            //{
            //   // itemList.Mobile = db.UserLogins.Where(x => x.ID == itemList.UserLoginId).Select(y => y.Mobile).FirstOrDefault();
            //    itemList.ERPbySP = objLeaderDashboard.getERPBySP(itemList.UserLoginId, 1, 1, 0);//Added By Sonali for commonFunction on 04-02-2019

            // //   itemList.Designation = objLeaderDashboard.UserDesignation(Convert.ToInt32(itemList.UserLoginId)); //  added by lokesh 30 - 04 -2019

            //}
            //-------------end-------------- -//
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
                Value = (new LeadersDashboard()).getHour()//WebConfigurationManager.AppSettings["Del_Hour"]
            };
            list = db.Database.SqlQuery<TabularView>("EXEC Leaders_NetworkReport_New @UserID, @Hour", idParam, Hour).ToList<TabularView>();
            return list;
        }

        public List<Downline> GetDataDownline()
        {
            List<Downline> list = new List<Downline>();
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
            
            list = db.Database.SqlQuery<Downline>("EXEC spmlmdownlinestatus @UserID", idParam).ToList<Downline>();
            return list;
        }


    }
}