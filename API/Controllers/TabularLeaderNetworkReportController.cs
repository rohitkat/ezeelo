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
    public class TabularLeaderNetworkReportController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private const int pageSize = 10;

        [System.Web.Http.Route("api/TabularLeaderNetworkReport")]
        public object Get(long LoginUserId)
        {
            object obj = new object();
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
                // Param.list = GetData();
                //----amit----//
                LeadersDashboard objLeaderDashboard = new LeadersDashboard();
                List<TabularView> list = GetData(LoginUserId);

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

                //-----------amit--------------//
                //foreach (var itemList in list)
                //{
                //   // itemList.Mobile = db.UserLogins.Where(x => x.ID == itemList.UserLoginId).Select(y => y.Mobile).FirstOrDefault();
                //    itemList.ERPbySP = objLeaderDashboard.getERPBySP(itemList.UserLoginId, 1, 1, 0);
                //    //itemList.Email = string.Empty;
                //    //itemList.Designation = objLeaderDashboard.UserDesignation(Convert.ToInt32(itemList.UserLoginId)); //  added by Roshan 30 - 04 -2019
                //}
                //-------------end---------------//
                //if (Param.pageNo == 0)
                //{
                //    Param.pageNo = 1;
                //}
                //var model = list.ToPagedList(Param.pageNo, pageSize);

                obj = new { Success = 1, Message = "Successfull", data = list };
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
                Value = (new LeadersDashboard()).getHour()//WebConfigurationManager.AppSettings["Del_Hour"]
            };
            list = db.Database.SqlQuery<TabularView>("EXEC Leaders_NetworkReport_New @UserID, @Hour", idParam, Hour).ToList<TabularView>();
            return list;
        }
    }
}
