using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;

namespace API.Controllers
{
    public class ListViewLeaderNetworkReportController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private const int pageSize = 10;
        public object Get(long userID)
        {
            object obj = new object();
            LeadersDashboard objLeaderDashoboard = new LeadersDashboard();
            try
            {
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
                Param.list = GetData(userID);

                List<TabularView> list = Param.list; //GetData(userID);
                // DashboardController objDashboardController = new DashboardController();
                //  objDashboardController.InitializeController(this.Request.RequestContext);
                //foreach (var objlist in list)
                //{
                //    //objlist.Mobile = db.UserLogins.Where(x => x.ID == objlist.UserLoginId).Select(y => y.Mobile).FirstOrDefault();
                //    objlist.ERPbySP = objLeaderDashoboard.getERPBySP(objlist.UserLoginId, 1, 1, 0);
                //    objlist.Email = string.Empty;
                //}
                AppLeadernetworkReport objAppleaderNetwork = new AppLeadernetworkReport();

                objAppleaderNetwork.Level = list.Where(p => p.Level == 1).ToList();
                List<TabularView> level2 = list.Where(p => p.Level == 2).ToList();

                foreach (var item in objAppleaderNetwork.Level)
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



                objAppleaderNetwork.Email = db.UserLogins.Where(x => x.ID == userID).Select(y => y.Email).FirstOrDefault();
                objAppleaderNetwork.RefferedBy = db.MLMUsers.Where(x => x.UserID == userID).Select(x => x.Refered_Id_ref).FirstOrDefault();
                objAppleaderNetwork.JoinDate = db.MLMUsers.Where(x => x.UserID == userID).Select(y => y.Join_date_ref).FirstOrDefault();
                objAppleaderNetwork.LastTransaction = db.CustomerOrders.Where(x => x.UserLoginID == userID).OrderByDescending(x => x.CreateDate).Select(y => y.CreateDate).FirstOrDefault();
                objAppleaderNetwork.FullName = db.PersonalDetails.Where(x => x.UserLoginID == userID).Select(y => y.FirstName + " " + y.LastName).FirstOrDefault();
                objAppleaderNetwork.RefferalID = db.MLMUsers.Where(y => y.UserID == userID).Select(x => x.Ref_Id).FirstOrDefault();
                objAppleaderNetwork.TotalMembers = objLeaderDashoboard.GetTotalMemberCount(userID);
                objAppleaderNetwork.QRP = objLeaderDashoboard.GetQRP(userID);
                objAppleaderNetwork.ERP = objLeaderDashoboard.getERPBySP(userID, 1, 1, 0);
                objAppleaderNetwork.Designation = objLeaderDashoboard.UserDesignation(Convert.ToInt32(userID));
                obj = new { Success = 1, Message = "Successfull", data = objAppleaderNetwork };


            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        public List<TabularView> GetData(long LoginUserId)
        {
            List<TabularView> list = new List<TabularView>();
            var idParam = new SqlParameter
            {
                ParameterName = "UserID",
                Value = LoginUserId
            };
            var Hour = new SqlParameter
            {
                ParameterName = "Hour",
                Value = (new LeadersDashboard()).getHour() // WebConfigurationManager.AppSettings["Del_Hour"]
            };
            list = db.Database.SqlQuery<TabularView>("EXEC Leaders_NetworkReport_New @UserID, @Hour", idParam, Hour).ToList<TabularView>();
            return list;
        }

        public class AppLeadernetworkReport
        {
            public List<TabularView> Level { get; set; }
            public string Email { get; set; }
            public string RefferedBy { get; set; }
            public DateTime JoinDate { get; set; }
            public DateTime LastTransaction { get; set; }
            public string FullName { get; set; }
            public string RefferalID { get; set; }
            public long TotalMembers { get; set; }
            public long QRP { get; set; }
            public double ERP { get; set; }
            public string Designation { get; set; }
        }
    }
}
